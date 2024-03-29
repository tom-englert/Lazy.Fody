﻿namespace Lazy.Fody
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FodyTools;

    using global::Fody;

    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;

    internal static class Processor
    {
        internal static void Process(this ModuleDefinition moduleDefinition, ILogger logger, SystemReferences coreReferences)
        {
            var allTypes = moduleDefinition.GetTypes();

            var allClasses = allTypes
                .Where(x => x.IsClass && (x.BaseType != null))
                .ToArray();

            var weavedMethods = new Dictionary<MethodDefinition, MethodDefinition>();

            foreach (var classDefinition in allClasses)
            {
                ProcessClass(classDefinition, coreReferences, logger, weavedMethods);
            }

            var injectedMethods = new HashSet<MethodDefinition>(weavedMethods.Values);

            foreach (var classDefinition in allClasses)
            {
                PostProcessClass(classDefinition, logger, weavedMethods, injectedMethods);
            }
        }

        private static void PostProcessClass(TypeDefinition classDefinition, ILogger logger, IReadOnlyDictionary<MethodDefinition, MethodDefinition> weavedMethods, HashSet<MethodDefinition> injectedMethods)
        {
            var allMethods = classDefinition.Methods
                .Where(method => method.HasBody)
                .ToArray();

            foreach (var method in allMethods)
            {
                PostProcessMethod(method, logger, weavedMethods, injectedMethods);
            }
        }

        private static void PostProcessMethod(MethodDefinition method, ILogger logger, IReadOnlyDictionary<MethodDefinition, MethodDefinition> weavedMethods, HashSet<MethodDefinition> injectedMethods)
        {
            if (injectedMethods == null)
                throw new ArgumentNullException(nameof(injectedMethods));
            if (injectedMethods.Contains(method))
                return;

            var instructions = method.Body.Instructions;

            for (var index = 0; index < instructions.Count; index++)
            {
                var instruction = instructions[index];

                if ((instruction.OpCode != OpCodes.Call) && (instruction.OpCode != OpCodes.Callvirt))
                    continue;

                if (!(instruction.Operand is MethodDefinition calledMethod))
                    continue;

                if (!weavedMethods.TryGetValue(calledMethod, out var newMethod))
                    continue;

                logger.LogDebug($"{method.FullName}@{index}: Replace call of {calledMethod} with {newMethod}");

                instructions[index].Operand = newMethod;
            }
        }

        private static void ProcessClass(TypeDefinition classDefinition, SystemReferences systemReferences, ILogger logger, IDictionary<MethodDefinition, MethodDefinition> weavedMethods)
        {
            foreach (var property in classDefinition.Properties)
            {
                ProcessProperty(property, systemReferences, logger, weavedMethods);
            }
        }

        private static void ProcessProperty(PropertyDefinition property, SystemReferences systemReferences, ILogger logger, IDictionary<MethodDefinition, MethodDefinition> weavedMethods)
        {
            if (!ConsumeLazyAttribute(property, out var threadingMode))
                return;

            if (property.HasParameters)
                throw new WeavingException($"Unsupported property {property} => property has parameters");

            if (property.SetMethod != null)
                throw new WeavingException($"Unsupported property {property} => property has setter");

            var originalMethod = property.GetMethod;
            if (originalMethod == null || !originalMethod.HasBody)
                throw new WeavingException($"Unsupported property {property} => property has no getter");

            var isStatic = originalMethod.IsStatic;

            var classDefinition = originalMethod.DeclaringType;

            logger.LogInfo($"Weave Lazy into {property.FullName}");

            var propertyType = property.PropertyType;

            var lazyTypeReference = systemReferences.LazyTypeReference;
            var funcTypeReference = systemReferences.FuncTypeReference;

            var genericLazyTypeInstance = lazyTypeReference.MakeGenericInstanceType(propertyType);
            var genericFuncTypeInstance = funcTypeReference.MakeGenericInstanceType(propertyType);

            var lazyFieldName = $"<{property.Name}>_Lazy_Fody_BackingField";
            var lazyMethodName = $"<{property.Name}>_Lazy_Fody_Method";

            // replace the property getter with a new method: { return _Lazy_Fody_BackingField.Value }

            // -- create field of type: System.Lazy<property type>
            var lazyField = new FieldDefinition(lazyFieldName, FieldAttributes.Private, genericLazyTypeInstance);
            if (isStatic)
                lazyField.IsStatic = true;
            classDefinition.Fields.Add(lazyField);

            // -- create 
            var originalMethodName = originalMethod.Name;
            var wrapperMethod = new MethodDefinition(originalMethodName, originalMethod.Attributes, originalMethod.ReturnType);

            var lazyValueGetter = systemReferences.LazyValueGetterReference.OnGenericType(genericLazyTypeInstance);

            var wrapperMethodInstructions = wrapperMethod.Body.Instructions;

            // -- rename old method
            originalMethod.IsSpecialName = false;
            originalMethod.Name = lazyMethodName;

            MethodReference originalMethodReference = originalMethod;
            FieldReference lazyFieldReference = lazyField;

            if (classDefinition.HasGenericParameters)
            {
                var classReference = classDefinition.MakeGenericInstanceType(classDefinition.GenericParameters.OfType<TypeReference>().ToArray());
                originalMethodReference = originalMethod.OnGenericType(classReference);
                lazyFieldReference = new FieldReference(lazyField.Name, lazyField.FieldType, classReference);
            }

            if (isStatic)
            {
                wrapperMethodInstructions.AddRange(
                    Instruction.Create(OpCodes.Ldsfld, lazyFieldReference),
                    Instruction.Create(OpCodes.Call, lazyValueGetter),
                    Instruction.Create(OpCodes.Ret));

            }
            else
            {
                wrapperMethodInstructions.AddRange(
                    Instruction.Create(OpCodes.Ldarg_0),
                    Instruction.Create(OpCodes.Ldfld, lazyFieldReference),
                    Instruction.Create(OpCodes.Callvirt, lazyValueGetter),
                    Instruction.Create(OpCodes.Ret)
                );
            }


            // -- add new wrapper method
            property.GetMethod = wrapperMethod;
            classDefinition.Methods.Add(wrapperMethod);

            // -- inject initialization of lazy field into constructor(s)
            var funcConstructor = systemReferences.FuncConstructorReference.OnGenericType(genericFuncTypeInstance);
            var lazyConstructor = systemReferences.LazyConstructorReference.OnGenericType(genericLazyTypeInstance);

            if (isStatic)
            {
                classDefinition.InsertIntoStaticConstructor(
                    Instruction.Create(OpCodes.Ldnull),
                    Instruction.Create(OpCodes.Ldftn, originalMethodReference),
                    Instruction.Create(OpCodes.Newobj, funcConstructor),
                    Instruction.Create(OpCodes.Ldc_I4, threadingMode),
                    Instruction.Create(OpCodes.Newobj, lazyConstructor),
                    Instruction.Create(OpCodes.Stsfld, lazyFieldReference));
            }
            else
            {
                classDefinition.InsertIntoConstructors(() => new[] {
                    Instruction.Create(OpCodes.Ldarg_0),
                    Instruction.Create(OpCodes.Ldarg_0),
                    Instruction.Create(OpCodes.Ldftn, originalMethodReference),
                    Instruction.Create(OpCodes.Newobj, funcConstructor),
                    Instruction.Create(OpCodes.Ldc_I4, threadingMode),
                    Instruction.Create(OpCodes.Newobj, lazyConstructor),
                    Instruction.Create(OpCodes.Stfld, lazyFieldReference)
                });
            }

            weavedMethods[originalMethod] = wrapperMethod;
        }

        private static bool ConsumeLazyAttribute(ICustomAttributeProvider attributeProvider, out int mode)
        {
            mode = default;

            const string attributeName = "Lazy.LazyAttribute";

            var attribute = attributeProvider.GetAttribute(attributeName);

            if (attribute == null)
                return false;

            mode = attribute.GetValueTypeConstructorArgument<int>() ?? default;

            attributeProvider.CustomAttributes.Remove(attribute);

            return true;
        }
    }
}


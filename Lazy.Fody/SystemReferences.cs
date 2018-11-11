#pragma warning disable CS1720 // Expression will always cause a System.NullReferenceException because the type's default value is null

namespace Lazy.Fody
{
    using System;
    using System.Linq;
    using System.Threading;

    using FodyTools;

    using global::Fody;

    using JetBrains.Annotations;

    using Mono.Cecil;
    using Mono.Cecil.Rocks;

    public class SystemReferences
    {
        public global::Fody.TypeSystem TypeSystem { get; }

        public TypeReference LazyTypeReference { get; }

        public TypeReference FuncTypeReference { get; }

        public TypeReference LazyThreadSafetyModeReference { get; }

        public MethodReference LazyConstructorReference { get; }

        public MethodReference FuncConstructorReference { get; }

        public MethodReference LazyValueGetterReference { get; }

        public SystemReferences([NotNull] ITypeSystem typeSystem)
        {
            TypeSystem = typeSystem.TypeSystem;
            
            LazyTypeReference = typeSystem.ImportType<Lazy<T>>();
            LazyThreadSafetyModeReference = typeSystem.ImportType<LazyThreadSafetyMode>();
            LazyConstructorReference = typeSystem.ImportMethod(() => new Lazy<T>(() => default(T), default(LazyThreadSafetyMode)));
            LazyValueGetterReference = typeSystem.ImportPropertyGet(() => default(Lazy<T>).Value);
            
            FuncTypeReference = typeSystem.ImportType<Func<T>>();
            FuncConstructorReference = typeSystem.ImportMethod<Func<T>, object, IntPtr>(".ctor");
        }

        private class T { }
    }
}
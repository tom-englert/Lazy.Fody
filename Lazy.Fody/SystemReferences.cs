#pragma warning disable CS1720 // Expression will always cause a System.NullReferenceException because the type's default value is null

namespace Lazy.Fody
{
    using System;
    using System.Threading;

    using FodyTools;

    using Mono.Cecil;

    internal class SystemReferences
    {
        public TypeReference LazyTypeReference { get; }

        public TypeReference FuncTypeReference { get; }

        public MethodReference LazyConstructorReference { get; }

        public MethodReference FuncConstructorReference { get; }

        public MethodReference LazyValueGetterReference { get; }

        public SystemReferences(ITypeSystem typeSystem)
        {
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            LazyTypeReference = typeSystem.ImportType<Lazy<T>>();
            LazyConstructorReference = typeSystem.ImportMethod(() => new Lazy<T>(() => default, default(LazyThreadSafetyMode)));
            LazyValueGetterReference = typeSystem.ImportPropertyGet(() => default(Lazy<T>).Value);
            
            FuncTypeReference = typeSystem.ImportType<Func<T>>();
            FuncConstructorReference = typeSystem.ImportMethod<Func<T>, object, IntPtr>(".ctor");
        }

        private class T { }
    }
}
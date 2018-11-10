namespace Lazy
{
    using System;
    using System.Threading;

    [AttributeUsage(AttributeTargets.Property)]
    public class LazyAttribute : Attribute
    {
        public LazyThreadSafetyMode Mode { get; }

        public LazyAttribute(LazyThreadSafetyMode mode = default(LazyThreadSafetyMode))
        {
            Mode = mode;
        }
    }
}

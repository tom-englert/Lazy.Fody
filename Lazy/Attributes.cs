namespace Lazy
{
    using System;
    using System.Threading;

    /// <summary>
    /// Apply this attribute to a read only property to implement the lazy pattern using <see cref="Lazy{T}"/>.<para/>
    /// See <see href="https://github.com/tom-englert/Lazy.Fody"/> for details.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LazyAttribute : Attribute
    {
        /// <summary>
        /// Gets the threading mode passed to the constructor of <see cref="Lazy{T}"/>.
        /// </summary>
        public LazyThreadSafetyMode Mode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyAttribute"/> class.<para/>
        /// Apply this attribute to a read only property to implement the lazy pattern using <see cref="Lazy{T}"/>.<para/>
        /// See https://github.com/tom-englert/Lazy.Fody for details.<para/>
        /// </summary>
        /// <param name="mode">The threading mode passed to the constructor of <see cref="Lazy{T}"/>.</param>
        public LazyAttribute(LazyThreadSafetyMode mode = default(LazyThreadSafetyMode))
        {
            Mode = mode;
        }
    }
}

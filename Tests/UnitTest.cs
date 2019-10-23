// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedParameter.Local
// ReSharper disable NotNullMemberIsNotInitialized
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword

namespace Tests
{
    using System;
    using System.Threading;

    using Lazy;

    using Xunit;

    public class UnitTest
    {
        private int _getValueCalls;

        private readonly Lazy<int> _lazy;

        public UnitTest()
        {
            _lazy = new Lazy<int>(GetValue);
        }

        [Fact]
        public void IsLazyApplied1()
        {
            Assert.Equal(0, _getValueCalls);
            Assert.Equal(1, Test1);
            Assert.Equal(1, _getValueCalls);
            Assert.Equal(1, Test1);
            Assert.Equal(1, _getValueCalls);
            Assert.Equal(1, Test1);
            Assert.Equal(1, _getValueCalls);
            Assert.Equal(1, Test1);
        }

        [Fact]
        public void IsLazyApplied()
        {
            Assert.Equal(0, _getValueCalls);
            Assert.Equal(1, Test);
            Assert.Equal(1, _getValueCalls);
            Assert.Equal(1, Test);
            Assert.Equal(1, _getValueCalls);
            Assert.Equal(1, Test);
            Assert.Equal(1, _getValueCalls);
            Assert.Equal(1, Test);
        }

        [Lazy]
        public int Test => GetValue();

        public int Test1 => _lazy.Value;

        private int GetValue()
        {
            return Interlocked.Increment(ref _getValueCalls);
        }
    }

    public class Sample
    {
        private int _getValueCalls;

        private readonly Lazy<int> _lazy;

        public Sample()
        {
            _lazy = new Lazy<int>(GetValue);
        }

        public Sample(int someParam)
            : this()
        {
        }

        [Lazy(LazyThreadSafetyMode.ExecutionAndPublication)] 
        public int Test => GetValue();

        public int Test1 => _lazy.Value;

        private int GetValue()
        {
            return Interlocked.Increment(ref _getValueCalls);
        }
    }

    public class Sample2 : Sample
    {
        private int _getValueCalls;

        private readonly Lazy<int> _lazy;

        public Sample2()
            : base(SomeStaticMethod())
        {
        }

        public Sample2(int something)
            : base(something)
        {
            _lazy = new Lazy<int>(() => something);
        }

        public Sample2(double x)
            : this((int)x)
        {
        }


        [Lazy]
        public int Test => GetValue();

        [Lazy]
        public double Test2 => 2.5;

        [Lazy]
        public Func<Type> Test3 => () => default(Type);

        public int Test1 => _lazy.Value;

        private int GetValue()
        {
            return Interlocked.Increment(ref _getValueCalls);
        }

        private static int SomeStaticMethod()
        {
            return 5;
        }
    }
}

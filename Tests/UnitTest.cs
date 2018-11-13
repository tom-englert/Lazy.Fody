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

        private System.Lazy<int> _lazy;

        static UnitTest()
        {

        }

        public UnitTest()
        {
            _lazy = new System.Lazy<int>(GetValue);
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

        private System.Lazy<int> _lazy;

        public Sample()
        {
            _lazy = new System.Lazy<int>(GetValue);
        }

        public Sample(int someParam)
            : this()
        {

        }

        [Lazy]
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

        private readonly System.Lazy<int> _lazy;

        public Sample2()
            : base(3)
        {
        }

        public Sample2(int something)
            : base(something)
        {
            _lazy = new System.Lazy<int>(() => something);
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
    }
}

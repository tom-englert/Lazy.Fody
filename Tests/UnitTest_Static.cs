namespace Tests
{
    using System;
    using System.Threading;

    using Lazy;

    using Xunit;

    public class UnitTest_Static_1
    {
        private static int _getValueCalls;
        private static readonly Lazy<int> _lazy = new Lazy<int>(GetValue);

        private static int GetValue()
        {
            return Interlocked.Increment(ref _getValueCalls);
        }

        private static int Lazy_System => _lazy.Value;

        [Fact]
        public void IsLazyApplied_System()
        {
            Assert.Equal(0, _getValueCalls);
            Assert.Equal(1, Lazy_System);
            Assert.Equal(1, _getValueCalls);
            Assert.Equal(1, Lazy_System);
            Assert.Equal(1, _getValueCalls);
            Assert.Equal(1, Lazy_System);
            Assert.Equal(1, _getValueCalls);
            Assert.Equal(1, Lazy_System);
        }
    }

    public class UnitTest_Static_2
    {
        private static int _getValueCalls;
        
        private static int GetValue()
        {
            return Interlocked.Increment(ref _getValueCalls);
        }

        [Lazy]
        private static int Lazy_Fody_Static => GetValue();

        [Fact]
        public void IsLazyApplied_Fody()
        {
            Assert.Equal(0, _getValueCalls);
            Assert.Equal(1, Lazy_Fody_Static);
            Assert.Equal(1, _getValueCalls);
            Assert.Equal(1, Lazy_Fody_Static);
            Assert.Equal(1, _getValueCalls);
            Assert.Equal(1, Lazy_Fody_Static);
            Assert.Equal(1, _getValueCalls);
            Assert.Equal(1, Lazy_Fody_Static);
        }
    }

    public class UnitTest_Static_3
    {
        private static int _getValueCalls;

        private static int GetValue()
        {
            return Interlocked.Increment(ref _getValueCalls);
        }

        [Lazy]
        private static int Lazy_Fody_Static => GetValue();

        [Lazy]
        private int Lazy_Fody_Instance => GetValue();

        [Fact]
        public void IsLazyFody_Instance_SameAs_Static()
        {
            var lazy_Fody_Local = new Lazy<int>(GetValue);

            Assert.Equal(0, _getValueCalls);
            Assert.Equal(1, Lazy_Fody_Static);

            Assert.Equal(1, _getValueCalls);
            Assert.Equal(2, Lazy_Fody_Instance);

            Assert.Equal(2, _getValueCalls);
            Assert.Equal(1, Lazy_Fody_Static);

            Assert.Equal(2, _getValueCalls);
            Assert.Equal(2, Lazy_Fody_Instance);

            Assert.Equal(2, _getValueCalls);
            Assert.Equal(3, lazy_Fody_Local.Value);

            Assert.Equal(3, _getValueCalls);
            Assert.Equal(1, Lazy_Fody_Static);

            Assert.Equal(3, _getValueCalls);
            Assert.Equal(2, Lazy_Fody_Instance);

            Assert.Equal(3, _getValueCalls);
            Assert.Equal(3, lazy_Fody_Local.Value);

            Assert.Equal(3, _getValueCalls);
            Assert.Equal(1, Lazy_Fody_Static);

            Assert.Equal(3, _getValueCalls);
            Assert.Equal(2, Lazy_Fody_Instance);

            Assert.Equal(3, _getValueCalls);
            Assert.Equal(3, lazy_Fody_Local.Value);
        }
    }
}

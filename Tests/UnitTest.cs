namespace Tests
{
    using System.Threading;

    using Lazy;

    using Xunit;

    public class UnitTest
    {
        private int _getValueCalls;

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

        private int GetValue()
        {
            return Interlocked.Increment(ref _getValueCalls);
        }
    }
}

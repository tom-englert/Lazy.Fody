namespace Tests
{
    using System;
    using System.Threading;

    class Reference
    {
        public double Value => GetValue();

        private double GetValue()
        {
            return 1;
        }
    }

    class ReferenceWeaved
    {
        private Lazy<double> _lazy;

        public ReferenceWeaved()
        {
            _lazy = new Lazy<double>(GetValue, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public double Value => _lazy.Value;

        private double GetValue()
        {
            return 1;
        }
    }
}

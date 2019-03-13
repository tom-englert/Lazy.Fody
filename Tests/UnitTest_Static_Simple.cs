using System;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword

namespace Tests
{
    using System.Threading;
    using Lazy;
    using Xunit;

    public class UnitTest_Static_Simple {               

        static UnitTest_Static_Simple() {            
        }

        public UnitTest_Static_Simple() {            
        }

        [Fact] public void IsLazyApplied_Sys() {
            var initVal = Test_SysLazy;
            Thread.Sleep(10);
            Assert.NotEqual(initVal, GetValue());
            Assert.Equal(initVal, Test_SysLazy);
            Assert.NotEqual(initVal, GetValue());
            Assert.Equal(initVal, Test_SysLazy);
            Assert.NotEqual(initVal, GetValue());
            Assert.Equal(initVal, Test_SysLazy);
            Assert.NotEqual(initVal, GetValue());
            Assert.Equal(initVal, Test_SysLazy);
        }

        [Fact] public void IsLazyApplied_Fody() {
            var initVal = Test_FodyLazy;
            Thread.Sleep(20);
            Assert.NotEqual(initVal, GetValue());
            Assert.Equal(initVal, Test_FodyLazy);
            Assert.NotEqual(initVal, GetValue());
            Assert.Equal(initVal, Test_FodyLazy);
            Assert.NotEqual(initVal, GetValue());
            Assert.Equal(initVal, Test_FodyLazy);
            Assert.NotEqual(initVal, GetValue());
            Assert.Equal(initVal, Test_FodyLazy);
        }

        [Lazy] public static DateTime Test_FodyLazy => GetValue();

        static readonly System.Lazy<DateTime> _test_SysLazy = new System.Lazy<DateTime>(GetValue);
        public static DateTime Test_SysLazy => _test_SysLazy.Value;

        static DateTime GetValue() => DateTime.Now;
    }
}

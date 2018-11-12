using System;

namespace Xunit
{
    public static class Assert
    {
        public static void Equal(object a, object b)
        {
        }
        public static void NotEqual(object a, object b)
        {
        }
    }

    public class FactAttribute : Attribute
    {
    }

    public class TheoryAttribute : Attribute
    {
    }
}
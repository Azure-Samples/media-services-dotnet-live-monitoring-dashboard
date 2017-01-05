using System;

namespace MediaDashboard.Common
{
    /*
     * Simple implementation of validation functions to validate input arguments.
     * Feel free to add new validation methods.
     */

    public static class Validate
    {
        public static void NotNullOrEmpty(string arg, string argName)
        {
            if(string.IsNullOrEmpty(arg))
                throw new ArgumentNullException(argName);
            //if (null == arg)
            //    throw new ArgumentNullException(argName);
            //if (string.Empty == arg)
            //    throw new ArgumentException(argName);
        }

        public static void NotNull(object arg, string argName)
        {
            if (null == arg)
                throw new ArgumentNullException(argName);
        }

        public static void GreaterThanZero(int arg, string argName)
        {
            if (arg <= 0)
                throw new ArgumentOutOfRangeException(argName);
        }

        public static void NonNegative(int arg, string argName)
        {
            if (arg < 0)
                throw new ArgumentOutOfRangeException(argName);
        }

        public static void IsTrue(Func<bool> func, string argName)
        {
            if (!func())
                throw new ArgumentException(argName);
        }

        public static void IsTrue(bool valid, string argName)
        {
            if (!valid)
                throw new ArgumentException(argName);
        }

        public static void AreEqual<T>(T expected, T value, string argName)
        {
            if (!expected.Equals(value))
                throw new ArgumentOutOfRangeException(argName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helpers
{
    public class NumberHelper
    {
        /// <summary>
        /// returns the given int if it's a power of two or the previous power of 2
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static int GetPowerOfTwoLessThanOrEqualTo(int x)
        {
            return (x <= 0 ? 0 : (1 << (int)Math.Log(x, 2)));
        }

        /// <summary>
        /// returns true if the given int is a power of 2
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(int x)
        {
            return (((x & (~x + 1)) == x) && (x > 0));
        }


        /// <summary>
        /// Try to parse a string to a double
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <see cref="http://msdn.microsoft.com/de-de/library/f02979c7(v=vs.110).aspx"/>
        public static double TryToParseStringToDouble(string value)
        {
            double number;
            //Resolve localization problems with comma and point separated decimals.
            value = value.Replace(".", ",");
            bool result = Double.TryParse(value, out number);
            if (result)
            {
                return number;
            }
            else
            {
                if (value == null) value = "";
                throw new Exception("Conversion of string to double failed");
            }
        }
    }
}

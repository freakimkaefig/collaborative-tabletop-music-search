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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helpers
{
    public class NumberHelper
    {
        public static int GetPowerOfTwoLessThanOrEqualTo(int x)
        {
            return (x <= 0 ? 0 : (1 << (int)Math.Log(x, 2)));
        }

        public static bool IsPowerOfTwo(int x)
        {
            return (((x & (~x + 1)) == x) && (x > 0));
        }
    }
}

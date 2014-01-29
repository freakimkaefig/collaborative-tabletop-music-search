using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helpers
{
    public static class CodeHelpers
    {
        public static string GetTimeStamp()
        {
            DateTime timestamp = DateTime.Now;

            // HH zeigt 24-Stunden-Format, hh zeigt 12-Stunden-Format.         
            return timestamp.ToString("yyyy-MM-dd-HH:mm:sss");
        }
    }
}

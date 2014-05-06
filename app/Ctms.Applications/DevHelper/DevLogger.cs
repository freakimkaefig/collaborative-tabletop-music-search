using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ctms.Applications.Common;

namespace Ctms.Applications.DevHelper
{
    public static class DevLogger
    {
        public static bool WriteFile = true;
        public static bool WriteOuput = true;

        public static void Log(string message)
        {
            if (WriteOuput == true) Console.WriteLine(message);
            if (WriteFile == true)  Configurator.Log.Debug(message);
        }
    }
}

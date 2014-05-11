using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Helpers
{
    public class KeyboardHelper
    {
        public static void ShowKeyboard()
        {
            Process.Start("C:/Program Files/Common Files/Microsoft Shared/ink/tabtip.exe");
        }
    }
}

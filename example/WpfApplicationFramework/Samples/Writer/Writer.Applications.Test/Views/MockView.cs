using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;

namespace Waf.Writer.Applications.Test.Views
{
    public class MockView : IView
    {
        public object DataContext { get; set; }
    }
}

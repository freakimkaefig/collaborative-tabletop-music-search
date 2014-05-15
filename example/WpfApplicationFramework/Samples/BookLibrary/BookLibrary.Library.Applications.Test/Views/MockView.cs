using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;

namespace Waf.BookLibrary.Library.Applications.Test.Views
{
    public abstract class MockView : IView
    {
        public object DataContext { get; set; }
    }
}

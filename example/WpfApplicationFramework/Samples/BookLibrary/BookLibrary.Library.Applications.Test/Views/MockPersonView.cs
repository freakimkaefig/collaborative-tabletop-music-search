using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.BookLibrary.Library.Applications.Views;
using System.ComponentModel.Composition;

namespace Waf.BookLibrary.Library.Applications.Test.Views
{
    [Export(typeof(IPersonView)), Export]
    public class MockPersonView : MockView, IPersonView
    {
    }
}

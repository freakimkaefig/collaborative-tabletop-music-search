using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.BookLibrary.Library.Applications.Test.Views;
using Waf.BookLibrary.Reporting.Applications.Views;
using System.ComponentModel.Composition;

namespace Waf.BookLibrary.Reporting.Applications.Test.Views
{
    [Export(typeof(IReportView))]
    public class MockReportView : MockView, IReportView
    {
    }
}

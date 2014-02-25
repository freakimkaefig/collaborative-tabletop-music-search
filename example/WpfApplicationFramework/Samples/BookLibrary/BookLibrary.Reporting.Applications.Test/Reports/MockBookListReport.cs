using System.ComponentModel.Composition;
using Waf.BookLibrary.Reporting.Applications.Reports;

namespace Waf.BookLibrary.Reporting.Applications.Test.Reports
{
    [Export(typeof(IBookListReport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockBookListReport : MockReport, IBookListReport
    {
    }
}

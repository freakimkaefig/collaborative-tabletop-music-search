using System.ComponentModel.Composition;
using Waf.BookLibrary.Reporting.Applications.Reports;

namespace Waf.BookLibrary.Reporting.Applications.Test.Reports
{
    [Export(typeof(IBorrowedBooksReport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockBorrowedBooksReport : MockReport, IBorrowedBooksReport
    {
    }
}

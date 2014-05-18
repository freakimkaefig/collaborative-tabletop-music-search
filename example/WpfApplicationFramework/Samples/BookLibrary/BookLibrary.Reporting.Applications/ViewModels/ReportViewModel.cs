using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.BookLibrary.Reporting.Applications.Views;

namespace Waf.BookLibrary.Reporting.Applications.ViewModels
{
    [Export]
    public class ReportViewModel : ViewModel<IReportView>
    {
        private object report;
        private ICommand createBookListReportCommand;
        private ICommand createBorrowedBooksReportCommand;


        [ImportingConstructor]
        public ReportViewModel(IReportView view) : base(view)
        {
        }


        public object Report
        {
            get { return report; }
            set
            {
                if (report != value)
                {
                    report = value;
                    RaisePropertyChanged("Report");
                }
            }
        }

        public ICommand CreateBookListReportCommand
        {
            get { return createBookListReportCommand; }
            set
            {
                if (createBookListReportCommand != value)
                {
                    createBookListReportCommand = value;
                    RaisePropertyChanged("CreateBookListReportCommand");
                }
            }
        }
        
        public ICommand CreateBorrowedBooksReportCommand
        {
            get { return createBorrowedBooksReportCommand; }
            set
            {
                if (createBorrowedBooksReportCommand != value)
                {
                    createBorrowedBooksReportCommand = value;
                    RaisePropertyChanged("CreateBorrowedBooksReportCommand");
                }
            }
        }
    }
}

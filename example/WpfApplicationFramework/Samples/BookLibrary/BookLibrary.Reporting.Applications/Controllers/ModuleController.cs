using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Waf.Applications;
using Waf.BookLibrary.Library.Applications.Controllers;
using Waf.BookLibrary.Library.Applications.Services;
using Waf.BookLibrary.Reporting.Applications.DataModels;
using Waf.BookLibrary.Reporting.Applications.Reports;
using Waf.BookLibrary.Reporting.Applications.ViewModels;

namespace Waf.BookLibrary.Reporting.Applications.Controllers
{
    [Export(typeof(IModuleController)), Export]
    internal class ModuleController : IModuleController
    {
        private readonly CompositionContainer container;
        private readonly IShellService shellService;
        private readonly IEntityService entityService;
        private readonly Lazy<ReportViewModel> reportViewModel;
        private readonly DelegateCommand createBookListReportCommand;
        private readonly DelegateCommand createBorrowedBooksReportCommand;


        [ImportingConstructor]
        public ModuleController(CompositionContainer container, IShellService shellService, IEntityService entityService, Lazy<ReportViewModel> reportViewModel)
        {
            this.container = container;
            this.shellService = shellService;
            this.entityService = entityService;
            this.reportViewModel = reportViewModel;
            this.createBookListReportCommand = new DelegateCommand(CreateBookListReport);
            this.createBorrowedBooksReportCommand = new DelegateCommand(CreateBorrowedBooksReport);
        }


        private ReportViewModel ReportViewModel { get { return reportViewModel.Value; } }


        public void Initialize()
        {
            shellService.IsReportingEnabled = true;
            shellService.LazyReportingView = new Lazy<object>(InitializeReportView);
        }

        public void Run()
        {
        }

        public void Shutdown()
        {
        }

        private object InitializeReportView()
        {
            ReportViewModel.CreateBookListReportCommand = createBookListReportCommand;
            ReportViewModel.CreateBorrowedBooksReportCommand = createBorrowedBooksReportCommand;
            return ReportViewModel.View;
        }

        private void CreateBookListReport()
        {
            IBookListReport bookListReport = container.GetExportedValue<IBookListReport>();
            bookListReport.ReportData = new BookListReportDataModel(entityService.Books);
            ReportViewModel.Report = bookListReport.Report;
        }

        private void CreateBorrowedBooksReport()
        {
            IBorrowedBooksReport borrowedBooksReport = container.GetExportedValue<IBorrowedBooksReport>();
            borrowedBooksReport.ReportData = new BorrowedBooksReportDataModel(entityService.Books);
            ReportViewModel.Report = borrowedBooksReport.Report;
        }
    }
}

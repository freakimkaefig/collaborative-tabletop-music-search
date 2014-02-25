using System.Linq;
using System.Waf.Applications;
using System.Waf.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.Writer.Applications.Services;
using Waf.Writer.Applications.Test.Services;
using Waf.Writer.Applications.ViewModels;

namespace Waf.Writer.Applications.Test.Controllers
{
    [TestClass]
    public class PrintControllerTest : TestClassBase
    {
        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();
            InitializePrintController();
        }

        protected override void OnTestCleanup()
        {
            ShutdownPrintController();
            base.OnTestCleanup();
        }


        [TestMethod]
        public void PrintPreviewTest()
        {
            ShellViewModel shellViewModel = Container.GetExportedValue<ShellViewModel>();
            MainViewModel mainViewModel = Container.GetExportedValue<MainViewModel>();

            // When no document is available then the command cannot be executed
            Assert.IsFalse(mainViewModel.PrintPreviewCommand.CanExecute(null));

            // Create a new document and check that we can execute the PrintPreview command
            mainViewModel.FileService.NewCommand.Execute(null);
            Assert.IsTrue(mainViewModel.PrintPreviewCommand.CanExecute(null));
            
            // Execute the PrintPreview command and check the the PrintPreviewView is visible inside the ShellView
            mainViewModel.PrintPreviewCommand.Execute(null);
            PrintPreviewViewModel printPreviewViewModel = ViewHelper.GetViewModel<PrintPreviewViewModel>((IView)shellViewModel.ContentView);
            
            // Execute the Close command and check that the MainView is visible again
            printPreviewViewModel.CloseCommand.Execute(null);
            Assert.AreEqual(ViewHelper.GetViewModel((IView)shellViewModel.ContentView), mainViewModel);
        }

        [TestMethod]
        public void PrintTest()
        {
            MainViewModel mainViewModel = Container.GetExportedValue<MainViewModel>();
            Assert.IsFalse(mainViewModel.PrintCommand.CanExecute(null));

            mainViewModel.FileService.NewCommand.Execute(null);
            Assert.IsTrue(mainViewModel.PrintCommand.CanExecute(null));

            IFileService fileService = Container.GetExportedValue<IFileService>();
            MockPrintDialogService printDialogService = 
                (MockPrintDialogService)Container.GetExportedValue<IPrintDialogService>();

            printDialogService.ShowDialogResult = true;
            mainViewModel.PrintCommand.Execute(null);
            Assert.IsNotNull(printDialogService.DocumentPaginator);
            Assert.AreEqual(fileService.ActiveDocument.FileName, printDialogService.Description);
            
            printDialogService.ShowDialogResult = false;
            mainViewModel.PrintCommand.Execute(null);
            Assert.IsNull(printDialogService.DocumentPaginator);
            Assert.IsNull(printDialogService.Description);
        }

        [TestMethod]
        public void UpdateCommandsTest()
        {
            IFileService fileService = Container.GetExportedValue<IFileService>();
            MainViewModel mainViewModel = Container.GetExportedValue<MainViewModel>();

            fileService.NewCommand.Execute(null);
            fileService.NewCommand.Execute(null);
            fileService.ActiveDocument = null;

            AssertHelper.CanExecuteChangedEvent(mainViewModel.PrintPreviewCommand, () =>
                fileService.ActiveDocument = fileService.Documents.First());
            AssertHelper.CanExecuteChangedEvent(mainViewModel.PrintCommand, () =>
                fileService.ActiveDocument = fileService.Documents.Last());
        }
    }
}

using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using System.Waf.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.Writer.Applications.Documents;
using Waf.Writer.Applications.Test.Controllers;
using Waf.Writer.Applications.Test.Services;
using Waf.Writer.Applications.ViewModels;
using Waf.Writer.Applications.Services;
using Waf.Writer.Applications.Test.Documents;
using System.Windows.Input;

namespace Waf.Writer.Applications.Test.ViewModels
{
    [TestClass]
    public class MainViewModelTest : TestClassBase
    {
        protected override void OnTestInitialize()
        {
            base.OnTestInitialize();
            InitializePrintController();
        }

        
        [TestMethod]
        public void DocumentViewTest()
        {
            IFileService documentManager = Container.GetExportedValue<IFileService>();
            MainViewModel mainViewModel = Container.GetExportedValue<MainViewModel>();

            Assert.IsFalse(mainViewModel.DocumentViews.Any());
            Assert.IsNull(mainViewModel.ActiveDocumentView);

            mainViewModel.FileService.NewCommand.Execute(null);

            Assert.AreEqual(mainViewModel.DocumentViews.Single(), mainViewModel.ActiveDocumentView);

            mainViewModel.FileService.NewCommand.Execute(null);

            Assert.AreEqual(mainViewModel.DocumentViews.Last(), mainViewModel.ActiveDocumentView);
            Assert.AreEqual(2, mainViewModel.DocumentViews.Count);

            mainViewModel.ActiveDocumentView = mainViewModel.DocumentViews.First();
            mainViewModel.FileService.CloseCommand.Execute(null);

            Assert.AreEqual(1, mainViewModel.DocumentViews.Count);
            Assert.IsNull(mainViewModel.ActiveDocumentView);

            mainViewModel.ActiveDocumentView = mainViewModel.DocumentViews.Single();
            mainViewModel.FileService.CloseCommand.Execute(null);

            Assert.IsFalse(mainViewModel.DocumentViews.Any());
            Assert.IsNull(mainViewModel.ActiveDocumentView);
        }

        [TestMethod]
        public void PropertiesWithNotification()
        {
            MainViewModel mainViewModel = Container.GetExportedValue<MainViewModel>();

            object startView = new object();
            AssertHelper.PropertyChangedEvent(mainViewModel, x => x.StartView, () => mainViewModel.StartView = startView);
            Assert.AreEqual(startView, mainViewModel.StartView);

            ICommand printPreviewCommand = new DelegateCommand(() => { });
            AssertHelper.PropertyChangedEvent(mainViewModel, x => x.PrintPreviewCommand, () => mainViewModel.PrintPreviewCommand = printPreviewCommand);
            Assert.AreEqual(printPreviewCommand, mainViewModel.PrintPreviewCommand);

            ICommand printCommand = new DelegateCommand(() => { });
            AssertHelper.PropertyChangedEvent(mainViewModel, x => x.PrintCommand, () => mainViewModel.PrintCommand = printCommand);
            Assert.AreEqual(printCommand, mainViewModel.PrintCommand);

            ICommand exitCommand = new DelegateCommand(() => { });
            AssertHelper.PropertyChangedEvent(mainViewModel, x => x.ExitCommand, () => mainViewModel.ExitCommand = exitCommand);
            Assert.AreEqual(exitCommand, mainViewModel.ExitCommand);
        }

        [TestMethod]
        public void UpdateShellServiceDocumentNameTest()
        {
            IFileService fileService = Container.GetExportedValue<IFileService>();
            IShellService shellService = Container.GetExportedValue<IShellService>();
            MainViewModel mainViewModel = Container.GetExportedValue<MainViewModel>();

            fileService.NewCommand.Execute(null);
            fileService.ActiveDocument = fileService.Documents.First();
            AssertHelper.PropertyChangedEvent(shellService, x => x.DocumentName, 
                () => fileService.ActiveDocument.FileName = "Unit Test.rtf");
            Assert.AreEqual("Unit Test.rtf", shellService.DocumentName);
        }

        [TestMethod]
        public void SelectLanguageTest()
        {
            MainViewModel mainViewModel = Container.GetExportedValue<MainViewModel>();
            Assert.IsNull(mainViewModel.NewLanguage);
            
            mainViewModel.GermanCommand.Execute(null);
            Assert.AreEqual("de-DE", mainViewModel.NewLanguage.Name);

            mainViewModel.EnglishCommand.Execute(null);
            Assert.AreEqual("en-US", mainViewModel.NewLanguage.Name);
        }
    }
}

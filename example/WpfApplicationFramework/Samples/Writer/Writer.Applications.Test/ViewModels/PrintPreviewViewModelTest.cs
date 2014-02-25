using System;
using System.Windows.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.Writer.Applications.Test.Views;
using Waf.Writer.Applications.ViewModels;
using Waf.Writer.Applications.Services;

namespace Waf.Writer.Applications.Test.ViewModels
{
    [TestClass]
    public class PrintPreviewViewModelTest
    {
        [TestMethod]
        public void ExecutePrintCommand()
        {
            MockPrintPreviewView printPreviewView = new MockPrintPreviewView();
            ShellService shellService = new ShellService();
            MockDocumentPaginatorSource document = new MockDocumentPaginatorSource();

            PrintPreviewViewModel printPreviewViewModel = new PrintPreviewViewModel(printPreviewView, shellService, document);
            Assert.AreEqual(document, printPreviewViewModel.Document);

            printPreviewViewModel.PrintCommand.Execute(null);
            Assert.IsTrue(printPreviewView.PrintCalled);
        }


        private class MockDocumentPaginatorSource : IDocumentPaginatorSource
        {
            public DocumentPaginator DocumentPaginator
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}

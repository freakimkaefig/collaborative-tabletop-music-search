using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.Writer.Applications.ViewModels;
using Waf.Writer.Applications.Test.Views;
using Waf.Writer.Applications.Services;
using Waf.Writer.Applications.Test.Documents;
using Waf.Writer.Applications.Documents;

namespace Waf.Writer.Applications.Test.ViewModels
{
    [TestClass]
    public class SaveChangesViewModelTest
    {
        [TestMethod]
        public void SaveChangesViewModelCloseTest()
        {
            MockDocumentType documentType = new MockDocumentType("Mock Document", ".mock");
            IEnumerable<IDocument> documents = new IDocument[] 
            {
                documentType.New(),
                documentType.New(),
                documentType.New()
            };

            MockSaveChangesView view = new MockSaveChangesView();
            SaveChangesViewModel viewModel = new SaveChangesViewModel(view, documents);
            
            // In this case it tries to get the title of the unit test framework which is ""
            Assert.AreEqual("", SaveChangesViewModel.Title);
            Assert.AreEqual(documents, viewModel.Documents);
            
            object owner = new object();
            Assert.IsFalse(view.IsVisible);
            view.ShowDialogAction = v => 
            {
                Assert.AreEqual(owner, v.Owner);
                Assert.IsTrue(v.IsVisible);    
            };
            bool? dialogResult = viewModel.ShowDialog(owner);
            Assert.IsNull(dialogResult);
            Assert.IsFalse(view.IsVisible);

            view.ShowDialogAction = v =>
            {
                viewModel.YesCommand.Execute(null);
            };
            dialogResult = viewModel.ShowDialog(owner);            
            Assert.AreEqual(true, dialogResult);

            view.ShowDialogAction = v =>
            {
                viewModel.NoCommand.Execute(null);
            };
            dialogResult = viewModel.ShowDialog(owner);
            Assert.AreEqual(false, dialogResult);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.Writer.Applications.ViewModels;
using Waf.Writer.Applications.Views;
using Waf.Writer.Applications.Test.Views;
using Waf.Writer.Applications.Services;
using Waf.Writer.Applications.Documents;
using System.Waf.UnitTesting;

namespace Waf.Writer.Applications.Test.ViewModels
{
    [TestClass]
    public class RichTextViewModelTest
    {
        [TestMethod]
        public void PropertyChangedEventTest()
        {
            RichTextDocumentType documentType = new RichTextDocumentType();
            RichTextDocument document = new RichTextDocument(documentType);
            
            IRichTextView view = new MockRichTextView();
            RichTextViewModel viewModel = new RichTextViewModel(view, new ShellService(), document);

            Assert.AreEqual(document, viewModel.Document);

            AssertHelper.PropertyChangedEvent(viewModel, x => x.IsBold, () => viewModel.IsBold = true);
            Assert.IsTrue(viewModel.IsBold);

            AssertHelper.PropertyChangedEvent(viewModel, x => x.IsItalic, () => viewModel.IsItalic = true);
            Assert.IsTrue(viewModel.IsItalic);

            AssertHelper.PropertyChangedEvent(viewModel, x => x.IsUnderline, () => viewModel.IsUnderline = true);
            Assert.IsTrue(viewModel.IsUnderline);

            AssertHelper.PropertyChangedEvent(viewModel, x => x.IsNumberedList, () => viewModel.IsNumberedList = true);
            Assert.IsTrue(viewModel.IsNumberedList);

            AssertHelper.PropertyChangedEvent(viewModel, x => x.IsBulletList, () => viewModel.IsBulletList = true);
            Assert.IsTrue(viewModel.IsBulletList);

            AssertHelper.PropertyChangedEvent(viewModel, x => x.IsSpellCheckEnabled, () => viewModel.IsSpellCheckEnabled = true);
            Assert.IsTrue(viewModel.IsSpellCheckEnabled);
        }
    }
}

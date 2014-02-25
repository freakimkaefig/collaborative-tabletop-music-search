using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Waf.Applications;
using Waf.Writer.Applications.Documents;
using Waf.Writer.Applications.ViewModels;
using Waf.Writer.Applications.Views;
using Waf.Writer.Applications.Services;

namespace Waf.Writer.Applications.Controllers
{
    /// <summary>
    /// Responsible to synchronize RTF Documents with RichTextViewModels.
    /// </summary>
    [Export]
    internal class RichTextDocumentController : DocumentController
    {
        private readonly CompositionContainer container;
        private readonly IFileService fileService;
        private readonly IShellService shellService;
        private readonly MainViewModel mainViewModel;
        private readonly Dictionary<RichTextDocument, RichTextViewModel> richTextViewModels;

        
        [ImportingConstructor]
        public RichTextDocumentController(CompositionContainer container, IFileService fileService, IShellService shellService,
            MainViewModel mainViewModel) 
            : base(fileService)
        {
            this.container = container;
            this.fileService = fileService;
            this.shellService = shellService;
            this.mainViewModel = mainViewModel;
            this.richTextViewModels = new Dictionary<RichTextDocument, RichTextViewModel>();
            AddWeakEventListener(mainViewModel, MainViewModelPropertyChanged);
        }

        
        protected override void OnDocumentAdded(IDocument document)
        {
            RichTextDocument richTextDocument = document as RichTextDocument;
            if (richTextDocument != null)
            {
                IRichTextView richTextView = container.GetExportedValue<IRichTextView>();
                RichTextViewModel richTextViewModel = new RichTextViewModel(richTextView, shellService, richTextDocument);
                richTextViewModels.Add(richTextDocument, richTextViewModel);
                mainViewModel.DocumentViews.Add(richTextViewModel.View);
            }
        }

        protected override void OnDocumentRemoved(IDocument document)
        {
            RichTextDocument richTextDocument = document as RichTextDocument;
            if (richTextDocument != null)
            {
                mainViewModel.DocumentViews.Remove(richTextViewModels[richTextDocument].View);
                richTextViewModels.Remove(richTextDocument);
            }
        }

        protected override void OnActiveDocumentChanged(IDocument activeDocument)
        {
            if (activeDocument == null)
            {
                mainViewModel.ActiveDocumentView = null;
            }
            else
            {
                RichTextDocument richTextDocument = activeDocument as RichTextDocument;
                if (richTextDocument != null)
                {
                    mainViewModel.ActiveDocumentView = richTextViewModels[richTextDocument].View;
                }
            }
        }

        private void MainViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveDocumentView")
            {
                IView richTextView = mainViewModel.ActiveDocumentView as IView;
                if (richTextView != null)
                {
                    RichTextViewModel richTextViewModel = ViewHelper.GetViewModel<RichTextViewModel>(richTextView);
                    if (richTextViewModel != null)
                    {
                        fileService.ActiveDocument = richTextViewModel.Document;
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using Waf.Writer.Applications.Views;
using Waf.Writer.Applications.Documents;
using System.Waf.Applications.Services;
using Waf.Writer.Applications.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Globalization;
using System.ComponentModel.Composition;
using System.ComponentModel;
using Waf.Writer.Applications.Properties;
using System.IO;
using System.Collections.Specialized;

namespace Waf.Writer.Applications.ViewModels
{
    [Export]
    public class MainViewModel : ViewModel<IMainView>
    {
        private readonly IMessageService messageService;
        private readonly IShellService shellService;
        private readonly IFileService fileService;
        private readonly ObservableCollection<object> documentViews;
        private readonly DelegateCommand englishCommand;
        private readonly DelegateCommand germanCommand;
        private readonly DelegateCommand aboutCommand;
        private object startView;
        private ICommand printPreviewCommand;
        private ICommand printCommand;
        private ICommand exitCommand;
        private IDocument activeDocument;
        private object activeDocumentView;
        private CultureInfo newLanguage;


        [ImportingConstructor]
        public MainViewModel(IMainView view, IMessageService messageService, IShellService shellService, IFileService fileService) 
            : base(view)
        {
            this.messageService = messageService;
            this.shellService = shellService;
            this.fileService = fileService;
            this.documentViews = new ObservableCollection<object>();
            this.englishCommand = new DelegateCommand(() => SelectLanguage(new CultureInfo("en-US")));
            this.germanCommand = new DelegateCommand(() => SelectLanguage(new CultureInfo("de-DE")));
            this.aboutCommand = new DelegateCommand(ShowAboutMessage);

            AddWeakEventListener(documentViews, DocumentViewsCollectionChanged);
            AddWeakEventListener(fileService, FileServicePropertyChanged);
        }


        public IFileService FileService { get { return fileService; } }
        
        public object StartView
        {
            get { return startView; }
            set
            {
                if (startView != value)
                {
                    startView = value;
                    RaisePropertyChanged("StartView");
                }
            }
        }

        public ObservableCollection<object> DocumentViews { get { return documentViews; } }

        public object ActiveDocumentView
        {
            get { return activeDocumentView; }
            set
            {
                if (activeDocumentView != value)
                {
                    activeDocumentView = value;
                    RaisePropertyChanged("ActiveDocumentView");
                }
            }
        }

        public CultureInfo NewLanguage { get { return newLanguage; } }

        public ICommand PrintPreviewCommand
        {
            get { return printPreviewCommand; }
            set
            {
                if (printPreviewCommand != value)
                {
                    printPreviewCommand = value;
                    RaisePropertyChanged("PrintPreviewCommand");
                }
            }
        }

        public ICommand PrintCommand 
        { 
            get { return printCommand; }
            set
            {
                if (printCommand != value)
                {
                    printCommand = value;
                    RaisePropertyChanged("PrintCommand");
                }
            }
        }

        public ICommand ExitCommand
        {
            get { return exitCommand; }
            set
            {
                if (exitCommand != value)
                {
                    exitCommand = value;
                    RaisePropertyChanged("ExitCommand");
                }
            }
        }

        public ICommand EnglishCommand { get { return englishCommand; } }

        public ICommand GermanCommand { get { return germanCommand; } }

        public ICommand AboutCommand { get { return aboutCommand; } }


        private void SelectLanguage(CultureInfo uiCulture)
        {
            if (!uiCulture.Equals(CultureInfo.CurrentUICulture))
            {
                messageService.ShowMessage(shellService.ShellView, Resources.RestartApplication + "\n\n" +
                    Resources.ResourceManager.GetString("RestartApplication", uiCulture));
            }
            newLanguage = uiCulture;
        }

        private void ShowAboutMessage()
        {
            messageService.ShowMessage(shellService.ShellView, string.Format(CultureInfo.CurrentCulture, Resources.AboutText, 
                ApplicationInfo.ProductName, ApplicationInfo.Version));
        }

        private void DocumentViewsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!documentViews.Any())
            {
                ViewCore.ContentViewState = ContentViewState.StartViewVisible;
            }
            else
            {
                ViewCore.ContentViewState = ContentViewState.DocumentViewVisible;
            }
        }

        private void FileServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveDocument")
            {
                if (activeDocument != null) { RemoveWeakEventListener(activeDocument, ActiveDocumentPropertyChanged); }

                activeDocument = fileService.ActiveDocument;

                if (activeDocument != null) { AddWeakEventListener(activeDocument, ActiveDocumentPropertyChanged); }

                UpdateShellServiceDocumentName();
            }
        }

        private void ActiveDocumentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FileName")
            {
                UpdateShellServiceDocumentName();
            }
        }

        private void UpdateShellServiceDocumentName()
        {
            if (fileService.ActiveDocument != null)
            {
                shellService.DocumentName = Path.GetFileName(fileService.ActiveDocument.FileName);
            }
            else
            {
                shellService.DocumentName = null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using System.ComponentModel.Composition;
using Waf.Writer.Applications.Documents;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Waf.Writer.Applications.Services
{
    [Export(typeof(IFileService)), Export]
    internal class FileService : DataModel, IFileService
    {
        private readonly ObservableCollection<IDocument> documents;
        private readonly ReadOnlyObservableCollection<IDocument> readOnlyDocuments;
        private IDocument activeDocument;
        private RecentFileList recentFileList;
        private ICommand newCommand;
        private ICommand openCommand;
        private ICommand closeCommand;
        private ICommand saveCommand;
        private ICommand saveAsCommand;


        [ImportingConstructor]
        public FileService()
        {
            this.documents = new ObservableCollection<IDocument>();
            this.readOnlyDocuments = new ReadOnlyObservableCollection<IDocument>(documents);
        }


        public ReadOnlyObservableCollection<IDocument> Documents { get { return readOnlyDocuments; } }

        public IDocument ActiveDocument
        {
            get { return activeDocument; }
            set
            {
                if (activeDocument != value)
                {
                    if (value != null && !documents.Contains(value))
                    {
                        throw new ArgumentException("value is not an item of the Documents collection.");
                    }
                    activeDocument = value;
                    RaisePropertyChanged("ActiveDocument");
                }
            }
        }

        public RecentFileList RecentFileList
        {
            get { return recentFileList; }
            set
            {
                if (recentFileList != value)
                {
                    recentFileList = value;
                    RaisePropertyChanged("RecentFileList");
                }
            }
        }

        public ICommand NewCommand
        {
            get { return newCommand; }
            set
            {
                if (newCommand != value)
                {
                    newCommand = value;
                    RaisePropertyChanged("NewCommand");
                }
            }
        }

        public ICommand OpenCommand
        {
            get { return openCommand; }
            set
            {
                if (openCommand != value)
                {
                    openCommand = value;
                    RaisePropertyChanged("OpenCommand");
                }
            }
        }

        public ICommand CloseCommand
        {
            get { return closeCommand; }
            set
            {
                if (closeCommand != value)
                {
                    closeCommand = value;
                    RaisePropertyChanged("CloseCommand");
                }
            }
        }

        public ICommand SaveCommand
        {
            get { return saveCommand; }
            set
            {
                if (saveCommand != value)
                {
                    saveCommand = value;
                    RaisePropertyChanged("SaveCommand");
                }
            }
        }

        public ICommand SaveAsCommand
        {
            get { return saveAsCommand; }
            set
            {
                if (saveAsCommand != value)
                {
                    saveAsCommand = value;
                    RaisePropertyChanged("SaveAsCommand");
                }
            }
        }


        public void AddDocument(IDocument document)
        {
            documents.Add(document);
        }

        public void RemoveDocument(IDocument document)
        {
            documents.Remove(document);
        }
    }
}

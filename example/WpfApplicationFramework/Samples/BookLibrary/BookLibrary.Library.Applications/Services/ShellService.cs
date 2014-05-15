using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;

namespace Waf.BookLibrary.Library.Applications.Services
{
    [Export(typeof(IShellService)), Export]
    internal class ShellService : DataModel, IShellService
    {
        private object shellView;
        private object bookListView;
        private object bookView;
        private object personListView;
        private object personView;
        private bool isReportingEnabled;
        private Lazy<object> lazyReportingView;
        

        public object ShellView
        {
            get { return shellView; }
            set
            {
                if (shellView != value)
                {
                    shellView = value;
                    RaisePropertyChanged("ShellView");
                }
            }
        }

        public object BookListView
        {
            get { return bookListView; }
            set
            {
                if (bookListView != value)
                {
                    bookListView = value;
                    RaisePropertyChanged("BookListView");
                }
            }
        }

        public object BookView
        {
            get { return bookView; }
            set
            {
                if (bookView != value)
                {
                    bookView = value;
                    RaisePropertyChanged("BookView");
                }
            }
        }

        public object PersonListView
        {
            get { return personListView; }
            set
            {
                if (personListView != value)
                {
                    personListView = value;
                    RaisePropertyChanged("PersonListView");
                }
            }
        }

        public object PersonView
        {
            get { return personView; }
            set
            {
                if (personView != value)
                {
                    personView = value;
                    RaisePropertyChanged("PersonView");
                }
            }
        }

        public bool IsReportingEnabled
        {
            get { return isReportingEnabled; }
            set
            {
                if (isReportingEnabled != value)
                {
                    isReportingEnabled = value;
                    RaisePropertyChanged("IsReportingEnabled");
                }
            }
        }

        public Lazy<object> LazyReportingView
        {
            get { return lazyReportingView; }
            set
            {
                if (lazyReportingView != value)
                {
                    lazyReportingView = value;
                    RaisePropertyChanged("LazyReportingView");
                }
            }
        }
    }
}

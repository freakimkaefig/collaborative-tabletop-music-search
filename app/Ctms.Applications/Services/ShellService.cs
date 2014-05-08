using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;

namespace Ctms.Applications.Services
{
    [Export(typeof(IShellService)), Export]
    internal class ShellService : DataModel, IShellService
    {
        private object shellView;
        private object detailView;
        private object searchView;
        private object playlistView;
        private object resultView;
        private object menuView;
        private object infoView;
        private bool isReportingEnabled;
        private Lazy<object> lazyReportingView;
        private object searchTagView;
        private object _resultStyleResources;


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

        public object DetailView
        {
            get { return detailView; }
            set
            {
                if (detailView != value)
                {
                    detailView = value;
                    RaisePropertyChanged("DetailView");
                }
            }
        }

        public object PlaylistView
        {
            get { return playlistView; }
            set
            {
                if (playlistView != value)
                {
                    playlistView = value;
                    RaisePropertyChanged("PlaylistView");
                }
            }
        }

        public object ResultView
        {
            get { return resultView; }
            set
            {
                if (resultView != value)
                {
                    resultView = value;
                    RaisePropertyChanged("ResultView");
                }
            }
        }

        public object SearchView
        {
            get { return searchView; }
            set
            {
                if (searchView != value)
                {
                    searchView = value;
                    RaisePropertyChanged("SearchView");
                }
            }
        }

        public object SearchTagView
        {
            get { return searchTagView; }
            set
            {
                if (searchTagView != value)
                {
                    searchTagView = value;
                    RaisePropertyChanged("SearchTagView");
                }
            }
        }

        public object MenuView
        {
            get { return menuView; }
            set
            {
                if (menuView != value)
                {
                    menuView = value;
                    RaisePropertyChanged("MenuView");
                }
            }
        }

        public object InfoView
        {
            get { return infoView; }
            set
            {
                if (infoView != value)
                {
                    infoView = value;
                    RaisePropertyChanged("InfoView");
                }
            }
        }

        public object ResultStyleResources
        {
            get { return _resultStyleResources; }
            set
            {
                if (_resultStyleResources != value)
                {
                    _resultStyleResources = value;
                    RaisePropertyChanged("ResultStyleResources");
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

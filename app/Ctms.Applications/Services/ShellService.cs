﻿using System;
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
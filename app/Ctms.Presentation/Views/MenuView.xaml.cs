using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Ctms.Applications.Views;
using Ctms.Applications.ViewModels;
using System.Waf.Applications;

namespace Ctms.Presentation.Views
{
    [Export(typeof(IMenuView))]
    public partial class MenuView : UserControl, IMenuView
    {
        private readonly Lazy<MenuViewModel> _lazyVm;

        //VisualStates
        private VisualState _loginDialogVisible;
        private VisualState _loginDialogInvisible;
        private VisualState _visualStateRotate0_LoginDialog;
        private VisualState _visualStateRotate180_LoginDialog;
        private VisualState _visualStateRotate0_OpenPlaylistDialog;
        private VisualState _visualStateRotate180_OpenPlaylistDialog;
        private VisualState _visualStateRotate0_CreatePlaylistDialog;
        private VisualState _visualStateRotate180_CreatePlaylistDialog;

        public MenuView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<MenuViewModel>(() => ViewHelper.GetViewModel<MenuViewModel>(this));

            //Visual States
            _loginDialogVisible = LoginDialogVisible;
            _loginDialogInvisible = LoginDialogInvisible;
            _visualStateRotate0_LoginDialog = Rotate0_LoginDialog;
            _visualStateRotate180_LoginDialog = Rotate180_LoginDialog;
            _visualStateRotate0_OpenPlaylistDialog = Rotate0_OpenPlaylistDialog;
            _visualStateRotate180_OpenPlaylistDialog = Rotate180_OpenPlaylistDialog;
            _visualStateRotate0_CreatePlaylistDialog = Rotate0_CreatePlaylistDialog;
            _visualStateRotate180_CreatePlaylistDialog = Rotate180_CreatePlaylistDialog;
        }

        public VisualState VisualStateLoginDialogVisible { get { return _loginDialogVisible; } set { } }
        public VisualState VisualStateLoginDialogInvisible { get { return _loginDialogInvisible; } set { } }
        public VisualState VisualStateRotate0_LoginDialog { get { return _visualStateRotate0_LoginDialog; } set { } }
        public VisualState VisualStateRotate180_LoginDialog { get { return _visualStateRotate180_LoginDialog; } set { } }
        public VisualState VisualStateRotate0_OpenPlaylistDialog { get { return _visualStateRotate0_OpenPlaylistDialog; } set { } }
        public VisualState VisualStateRotate180_OpenPlaylistDialog { get { return _visualStateRotate180_OpenPlaylistDialog; } set { } }
        public VisualState VisualStateRotate0_CreatePlaylistDialog { get { return _visualStateRotate0_CreatePlaylistDialog; } set { } }
        public VisualState VisualStateRotate180_CreatePlaylistDialog { get { return _visualStateRotate180_CreatePlaylistDialog; } set { } }

        // Provides this view's viewmodel
        private MenuViewModel _viewModel { get { return _lazyVm.Value; } }
    }
}

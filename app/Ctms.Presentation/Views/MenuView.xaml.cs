using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel.Composition;
using Ctms.Applications.Views;
using Ctms.Applications.ViewModels;
using System.Waf.Applications;
using Microsoft.Surface;
using Microsoft.Surface.Presentation.Input;
using Helpers;

namespace Ctms.Presentation.Views
{
    [Export(typeof(IMenuView))]
    public partial class MenuView : UserControl, IMenuView
    {
        private readonly Lazy<MenuViewModel> _lazyVm;

        //VisualStates
        private VisualState _visualStateMenuVisible;
        private VisualState _visualStateMenuInvisible;

        private VisualState _visualStateMenuRotate0;
        private VisualState _visualStateMenuRotate180;

        private VisualState _loginDialogVisible;
        private VisualState _loginDialogInvisible;

        private VisualState _newPlaylistDialogVisible;
        private VisualState _newPlaylistDialogInvisible;

        private VisualState _openPlaylistDialogVisible;
        private VisualState _openPlaylistDialogInvisible;

        public MenuView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<MenuViewModel>(() => ViewHelper.GetViewModel<MenuViewModel>(this));

            //Visual States
            _visualStateMenuVisible = MenuVisible;
            _visualStateMenuInvisible = MenuInvisible;
            _loginDialogVisible = LoginDialogVisible;
            _loginDialogInvisible = LoginDialogInvisible;
            _newPlaylistDialogVisible = NewPlaylistDialogVisible;
            _newPlaylistDialogInvisible = NewPlaylistDialogInvisible;
            _openPlaylistDialogVisible = OpenPlaylistDialogVisible;
            _openPlaylistDialogInvisible = OpenPlaylistDialogInvisible;
            _visualStateMenuRotate0 = Rotate0;
            _visualStateMenuRotate180 = Rotate180;
        }

        public VisualState VisualStateMenuVisible { get { return _visualStateMenuVisible; } set { } }
        public VisualState VisualStateMenuInvisible { get { return _visualStateMenuInvisible; } set { } }

        public VisualState VisualStateMenuRotate0 { get { return _visualStateMenuRotate0; } set { } }
        public VisualState VisualStateMenuRotate180 { get { return _visualStateMenuRotate180; } set { } }

        public VisualState VisualStateLoginDialogVisible { get { return _loginDialogVisible; } set { } }
        public VisualState VisualStateLoginDialogInvisible { get { return _loginDialogInvisible; } set { } }

        public VisualState VisualStateNewPlaylistVisible { get { return _newPlaylistDialogVisible; } set { } }
        public VisualState VisualStateNewPlaylistInvisible { get { return _newPlaylistDialogInvisible; } set { } }

        public VisualState VisualStateOpenPlaylistVisible { get { return _openPlaylistDialogVisible; } set { } }
        public VisualState VisualStateOpenPlaylistInvisible { get { return _openPlaylistDialogInvisible; } set { } }

        // Provides this view's viewmodel
        private MenuViewModel _viewModel { get { return _lazyVm.Value; } }

        private void InputField_GotFocus(object sender, RoutedEventArgs e)
        {
            KeyboardHelper.ShowKeyboard();
        }
    }
}

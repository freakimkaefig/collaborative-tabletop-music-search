using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Ctms.Applications.Views;
using System.Collections.ObjectModel;
using SpotifySharp;
using Ctms.Domain.Objects;
using System.Windows;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class MenuViewModel : ViewModel<IMenuView>
    {
        private IMenuView _view;
        private bool isValid = true;
        private bool _canLogin = true;
        private string _loginButtonContent = "Spotify Login";
        //Commands
        private ICommand _exitAppCommand;
        private ICommand _loginCommand;
        private ICommand _cancelLoginCommand;
        private ICommand _logoutCommand;
        private ICommand _openPlaylistCommand;
        private ICommand _newPlaylistCommand;
        private ICommand _rotateMenuCommand;

        private ICommand _displayLoginDialog;
        private ICommand _displayNewPlaylist;
        private ICommand _displayOpenPlaylist;

        //
        private string _spotifyUsernameInput = "mybleton";
        private string _loginLogMessage;
        private string _playlistName = null;
        private bool _loginDialogEnabled = true;
        private bool _isLoggingIn = false;
        private bool _isLoggedIn = false;
        private bool _menuIsVisible = false;
        private bool _menuIsRotate = false;
        private bool _loginDialogVisible = false;
        private bool _newPlaylistVisible = false;
        private bool _openPlaylistVisible = false;
        private ObservableCollection<SpotifyPlaylist> _playlists;


        [ImportingConstructor]
        public MenuViewModel(IMenuView view) : base(view)
        {
            _playlists = new ObservableCollection<SpotifyPlaylist>();
            _view = view;
            ShowLoginDialog(true);
        }

        #region Public Methods

        public void ShowLoginDialog(bool state)
        {
            if (state == true)
            {
                VisualState temp1 = _view.VisualStateMenuVisible;
                VisualStateManager.GoToState((FrameworkElement)_view, temp1.Name, true);
                VisualState temp2 = _view.VisualStateLoginDialogVisible;
                VisualStateManager.GoToState((FrameworkElement)_view, temp2.Name, true);
                LoginDialogVisible = true;
            }
            else
            {
                VisualState temp2 = _view.VisualStateLoginDialogInvisible;
                VisualStateManager.GoToState((FrameworkElement)_view, temp2.Name, true);
                LoginDialogVisible = false;
            }
        }

        public void RotateMenu()
        {
            VisualState temp;
            if (MenuIsRotate)
            {
                temp = _view.VisualStateMenuRotate0;
                MenuIsRotate = false;
            }
            else
            {
                temp = _view.VisualStateMenuRotate180;
                MenuIsRotate = true;
            }

            VisualStateManager.GoToState((FrameworkElement)_view, temp.Name, true);
        }

        public void ToggleLoginDialog()
        {
            if (NewPlaylistVisible)
            {
                VisualState temp = _view.VisualStateNewPlaylistInvisible;
                VisualStateManager.GoToState((FrameworkElement)_view, temp.Name, true);
                NewPlaylistVisible = false;
            }
            if (OpenPlaylistVisible)
            {
                VisualState temp = _view.VisualStateOpenPlaylistInvisible;
                VisualStateManager.GoToState((FrameworkElement)_view, temp.Name, true);
                OpenPlaylistVisible = false;
            }

            VisualState loginDialog;
            if (LoginDialogVisible)
            {
                loginDialog = _view.VisualStateLoginDialogInvisible;
                LoginDialogVisible = false;
            }
            else
            {
                loginDialog = _view.VisualStateLoginDialogVisible;
                LoginDialogVisible = true;
            }
            VisualStateManager.GoToState((FrameworkElement)_view, loginDialog.Name, true);
        }

        public void ToggleNewPlaylist()
        {
            if (LoginDialogVisible)
            {
                VisualState temp = _view.VisualStateLoginDialogInvisible;
                VisualStateManager.GoToState((FrameworkElement)_view, temp.Name, true);
                LoginDialogVisible = false;
            }
            if (OpenPlaylistVisible)
            {
                VisualState temp = _view.VisualStateOpenPlaylistInvisible;
                VisualStateManager.GoToState((FrameworkElement)_view, temp.Name, true);
                OpenPlaylistVisible = false;
            }

            VisualState newPlaylist;
            if (NewPlaylistVisible)
            {
                newPlaylist = _view.VisualStateNewPlaylistInvisible;
                NewPlaylistVisible = false;
            }
            else
            {
                newPlaylist = _view.VisualStateNewPlaylistVisible;
                NewPlaylistVisible = true;
            }
            VisualStateManager.GoToState((FrameworkElement)_view, newPlaylist.Name, true);
        }

        public void ToggleOpenPlaylist()
        {
            if (LoginDialogVisible)
            {
                VisualState temp = _view.VisualStateLoginDialogInvisible;
                VisualStateManager.GoToState((FrameworkElement)_view, temp.Name, true);
                LoginDialogVisible = false;
            }
            if (NewPlaylistVisible)
            {
                VisualState temp = _view.VisualStateNewPlaylistInvisible;
                VisualStateManager.GoToState((FrameworkElement)_view, temp.Name, true);
                NewPlaylistVisible = false;
            }

            VisualState openPlaylist;
            if (OpenPlaylistVisible)
            {
                openPlaylist = _view.VisualStateOpenPlaylistInvisible;
                OpenPlaylistVisible = false;
            }
            else
            {
                openPlaylist = _view.VisualStateOpenPlaylistVisible;
                OpenPlaylistVisible = true;
            }
            VisualStateManager.GoToState((FrameworkElement)_view, openPlaylist.Name, true);
        }

        public bool IsEnabled { get { return true; } }

        #endregion Public Methods

        #region Properties

        public bool IsValid
        {
            get { return isValid; }
            set
            {
                if (isValid != value)
                {
                    isValid = value;
                    RaisePropertyChanged("IsValid");
                }
            }
        }

        public bool CanLogin
        {
            get { return _canLogin; }
            set
            {
                if (_canLogin != value)
                {
                    _canLogin = value;
                    RaisePropertyChanged("CanLogin");
                }
            }
        }

        public string LoginButtonContent
        {
            get { return _loginButtonContent; }
            set
            {
                if (_loginButtonContent != value)
                {
                    _loginButtonContent = value;
                    RaisePropertyChanged("LoginButtonContent");
                }
            }
        }

        public string SpotifyUsernameInput
        {
            get { return _spotifyUsernameInput; }
            set
            {
                if (_spotifyUsernameInput != value)
                {
                    _spotifyUsernameInput = value;
                    RaisePropertyChanged("SpotifyUsernameInput");
                }
            }
        }

        public string PlaylistName
        {
            get { return _playlistName; }
            set
            {
                if (_playlistName != value)
                {
                    _playlistName = value;
                    RaisePropertyChanged("PlaylistName");
                }
            }
        }

        public string LoginLogMessage
        {
            get { return _loginLogMessage; }
            set
            {
                if (_loginLogMessage != value)
                {
                    _loginLogMessage = value;
                    RaisePropertyChanged("LoginLogMessage");
                }
            }
        }

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                if (_isLoggedIn != value)
                {
                    _isLoggedIn = value;
                    RaisePropertyChanged("IsLoggedIn");
                }
            }
        }

        public bool LoginDialogEnabled
        {
            get { return _loginDialogEnabled; }
            set
            {
                if (_loginDialogEnabled != value)
                {
                    _loginDialogEnabled = value;
                    RaisePropertyChanged("LoginDialogEnabled");
                }
            }
        }

        public bool IsLoggingIn
        {
            get { return _isLoggingIn; }
            set
            {
                if (_isLoggingIn != value)
                {
                    _isLoggingIn = value;
                    RaisePropertyChanged("IsLoggingIn");
                }
            }
        }

        public bool MenuIsVisible
        {
            get { return _menuIsVisible; }
            set
            {
                if (_menuIsVisible != value)
                {
                    _menuIsVisible = value;
                    RaisePropertyChanged("MenuIsVisible");
                }
            }
        }

        public bool MenuIsRotate
        {
            get { return _menuIsRotate; }
            set
            {
                if (_menuIsRotate != value)
                {
                    _menuIsRotate = value;
                    RaisePropertyChanged("MenuIsRotate");
                }
            }
        }

        public bool LoginDialogVisible
        {
            get { return _loginDialogVisible; }
            set
            {
                if (_loginDialogVisible != value)
                {
                    _loginDialogVisible = value;
                    RaisePropertyChanged("LoginDialogVisible");
                }
            }
        }

        public bool NewPlaylistVisible
        {
            get { return _newPlaylistVisible; }
            set
            {
                if (_newPlaylistVisible != value)
                {
                    _newPlaylistVisible = value;
                    RaisePropertyChanged("NewPlaylistVisible");
                }
            }
        }

        public bool OpenPlaylistVisible
        {
            get { return _openPlaylistVisible; }
            set
            {
                if (_openPlaylistVisible != value)
                {
                    _openPlaylistVisible = value;
                    RaisePropertyChanged("OpenPlaylistVisible");
                }
            }
        }

        public ObservableCollection<SpotifyPlaylist> Playlists
        {
            get { return _playlists; }
            set
            {
                if (_playlists != value)
                {
                    _playlists = value;
                    RaisePropertyChanged("Playlists");
                }
            }
        }

        #endregion Properties

        #region Commands

        public ICommand ExitAppCommand
        {
            get { return _exitAppCommand; }
            set
            {
                if (_exitAppCommand != value)
                {
                    _exitAppCommand = value;
                    RaisePropertyChanged("ExitAppCommand");
                }
            }
        }

        public ICommand LoginCommand
        {
            get { return _loginCommand; }
            set
            {
                if (_loginCommand != value)
                {
                    _loginCommand = value;
                    RaisePropertyChanged("LoginCommand");
                }
            }
        }

        public ICommand CancelLoginCommand
        {
            get { return _cancelLoginCommand; }
            set
            {
                if (_cancelLoginCommand != value)
                {
                    _cancelLoginCommand = value;
                    RaisePropertyChanged("CancelLoginCommand");
                }
            }
        }

        public ICommand RotateMenuCommand
        {
            get { return _rotateMenuCommand; }
            set
            {
                if (_rotateMenuCommand != value)
                {
                    _rotateMenuCommand = value;
                    RaisePropertyChanged("RotateMenuCommand");
                }
            }
        }

        public ICommand LogoutCommand
        {
            get { return _logoutCommand; }
            set
            {
                if (_logoutCommand != value)
                {
                    _logoutCommand = value;
                    RaisePropertyChanged("LogoutCommand");
                }
            }
        }

        public ICommand OpenPlaylistCommand
        {
            get { return _openPlaylistCommand; }
            set
            {
                if (_openPlaylistCommand != value)
                {
                    _openPlaylistCommand = value;
                    RaisePropertyChanged("OpenPlaylistCommand");
                }
            }
        }

        public ICommand NewPlaylistCommand
        {
            get { return _newPlaylistCommand; }
            set
            {
                if (_newPlaylistCommand != value)
                {
                    _newPlaylistCommand = value;
                    RaisePropertyChanged("NewPlaylistCommand");
                }
            }
        }

        public ICommand DisplayLoginDialog
        {
            get { return _displayLoginDialog; }
            set
            {
                if (_displayLoginDialog != value)
                {
                    _displayLoginDialog = value;
                    RaisePropertyChanged("DisplayLoginDialog");
                }
            }
        }

        public ICommand DisplayNewPlaylist
        {
            get { return _displayNewPlaylist; }
            set
            {
                if (_displayNewPlaylist != value)
                {
                    _displayNewPlaylist = value;
                    RaisePropertyChanged("DisplayNewPlaylist");
                }
            }
        }

        public ICommand DisplayOpenPlaylist
        {
            get { return _displayOpenPlaylist; }
            set
            {
                if (_displayOpenPlaylist != value)
                {
                    _displayOpenPlaylist = value;
                    RaisePropertyChanged("DisplayOpenPlaylist");
                }
            }
        }

        #endregion Commands

    }
}

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
        private ICommand _rotateLoginDialogCommand;
        private ICommand _logoutCommand;
        private ICommand _openPlaylistCommand;
        private ICommand _rotateOpenPlaylistCommand;
        private ICommand _newPlaylistCommand;
        private ICommand _rotateNewPlaylistCommand;
        //
        private string _spotifyUsernameInput = "mybleton";
        private string _loginLogMessage;
        private string _playlistName = null;
        private bool _loginDialogEnabled = true;
        private bool _isLoggingIn = false;
        private bool _isLoggedIn = false;
        private bool _menuIsVisible = false;
        private bool _loginDialogIsRotate = false;
        private bool _openPlaylistDialogIsRotate = false;
        private bool _createPlaylistDialogIsRotate = false;
        private ObservableCollection<SpotifyPlaylist> _playlists;


        [ImportingConstructor]
        public MenuViewModel(IMenuView view)
            : base(view)
        {
            _playlists = new ObservableCollection<SpotifyPlaylist>();
            _view = view;
            DisplayLoginDialog(true);
        }

        #region Public Methods

        public void DisplayLoginDialog(bool state)
        {
            if (state == true)
            {
                VisualState temp = _view.VisualStateLoginDialogVisible;
                VisualStateManager.GoToState((FrameworkElement)_view, temp.Name, true);
            }
            else
            {
                VisualState temp = _view.VisualStateLoginDialogInvisible;
                VisualStateManager.GoToState((FrameworkElement)_view, temp.Name, true);
            }
        }

        public bool IsEnabled { get { return true; } }//!! Has to be adjusted

        public void RotateLoginDialog()
        {
            VisualState visualState;
            if (LoginDialogIsRotate == false)
            {
                visualState = _view.VisualStateRotate180_LoginDialog;
                LoginDialogIsRotate = true;
            }
            else
            {
                visualState = _view.VisualStateRotate0_LoginDialog;
                LoginDialogIsRotate = false;
            }

            VisualStateManager.GoToState((FrameworkElement)_view, visualState.Name, true);
        }

        public void RotateOpenPlaylistDialog()
        {
            VisualState visualState;
            if (OpenPlaylistDialogIsRotate == false)
            {
                visualState = _view.VisualStateRotate180_OpenPlaylistDialog;
                OpenPlaylistDialogIsRotate = true;
            }
            else
            {
                visualState = _view.VisualStateRotate0_OpenPlaylistDialog;
                OpenPlaylistDialogIsRotate = false;
            }

            VisualStateManager.GoToState((FrameworkElement)_view, visualState.Name, true);
        }

        public void RotateCreatePlaylistDialog()
        {
            VisualState visualState;
            if (CreatePlaylistDialogIsRotate == false)
            {
                visualState = _view.VisualStateRotate180_CreatePlaylistDialog;
                CreatePlaylistDialogIsRotate = true;
            }
            else
            {
                visualState = _view.VisualStateRotate0_CreatePlaylistDialog;
                CreatePlaylistDialogIsRotate = false;
            }

            VisualStateManager.GoToState((FrameworkElement)_view, visualState.Name, true);
        }

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

        public bool LoginDialogIsRotate
        {
            get { return _loginDialogIsRotate; }
            set
            {
                if (_loginDialogIsRotate != value)
                {
                    _loginDialogIsRotate = value;
                    RaisePropertyChanged("LoginDialogIsRotate");
                }
            }
        }

        public bool OpenPlaylistDialogIsRotate
        {
            get { return _openPlaylistDialogIsRotate; }
            set
            {
                if (_openPlaylistDialogIsRotate != value)
                {
                    _openPlaylistDialogIsRotate = value;
                    RaisePropertyChanged("OpenPlaylistDialogIsRotate");
                }
            }
        }

        public bool CreatePlaylistDialogIsRotate
        {
            get { return _createPlaylistDialogIsRotate; }
            set
            {
                if (_createPlaylistDialogIsRotate != value)
                {
                    _createPlaylistDialogIsRotate = value;
                    RaisePropertyChanged("CreatePlaylistDialogIsRotate");
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

        public ICommand RotateLoginDialogCommand
        {
            get { return _rotateLoginDialogCommand; }
            set
            {
                if (_rotateLoginDialogCommand != value)
                {
                    _rotateLoginDialogCommand = value;
                    RaisePropertyChanged("RotateLoginDialogCommand");
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

        public ICommand RotateOpenPlaylistCommand
        {
            get { return _rotateOpenPlaylistCommand; }
            set
            {
                if (_rotateOpenPlaylistCommand != value)
                {
                    _rotateOpenPlaylistCommand = value;
                    RaisePropertyChanged("RotateOpenPlaylistCommand");
                }
            }
        }

        public ICommand RotateCreatePlaylistCommand
        {
            get { return _rotateNewPlaylistCommand; }
            set
            {
                if (_rotateNewPlaylistCommand != value)
                {
                    _rotateNewPlaylistCommand = value;
                    RaisePropertyChanged("RotateCreatePlaylistCommand");
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

        #endregion Commands

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Ctms.Applications.Views;
using Ctms.Domain.Objects;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class MenuViewModel : ViewModel<IMenuView>
    {
        private bool isValid = true;
        private Playlist _playlist;
        private ICommand _exitAppCommand;
        private ICommand _loginCommand;
        private ICommand _goCommand;
        private string _loginLogMessage;
        private string _goInput;
        private bool _isLoggedIn = false;
        private bool _playlistContainerLoaded = false;


        [ImportingConstructor]
        public MenuViewModel(IMenuView view)
            : base(view)
        {
        }

        public bool IsEnabled { get { return true; } }//!! Has to be adjusted

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

        public bool PlaylistContainerLoaded
        {
            get { return _playlistContainerLoaded; }
            set
            {
                if (_playlistContainerLoaded != value)
                {
                    _playlistContainerLoaded = value;
                    RaisePropertyChanged("PlaylistContainerLoaded");
                }
            }
        }
    }
}

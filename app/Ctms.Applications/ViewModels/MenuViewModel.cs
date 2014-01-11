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
        private Playlist playlist;
        private ICommand exitAppCommand;
        private ICommand loginCommand;
        private string loginLogMessage;
        private bool _isLoggedIn = false;


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
            get { return exitAppCommand; }
            set
            {
                if (exitAppCommand != value)
                {
                    exitAppCommand = value;
                    RaisePropertyChanged("ExitAppCommand");
                }
            }
        }

        public ICommand LoginCommand
        {
            get { return loginCommand; }
            set
            {
                if (loginCommand != value)
                {
                    loginCommand = value;
                    RaisePropertyChanged("LoginCommand");
                }
            }
        }

        public string LoginLogMessage
        {
            get { return loginLogMessage; }
            set
            {
                if (loginLogMessage != value)
                {
                    loginLogMessage = value;
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
    }
}

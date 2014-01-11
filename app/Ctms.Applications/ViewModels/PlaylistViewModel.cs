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
    public class PlaylistViewModel : ViewModel<IPlaylistView>
    {
        private bool isValid = true;
        private Playlist playlist;
        private ICommand loginCommand;
        private string logMessage;


        [ImportingConstructor]
        public PlaylistViewModel(IPlaylistView view)
            : base(view)
        {
        }


        public bool IsEnabled { get { return true; } }//Playlist != null;//!! Has to be adjusted

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

        public Playlist Playlist
        {
            get { return playlist; }
            set
            {
                if (playlist != value)
                {
                    playlist = value;
                    RaisePropertyChanged("Playlist");
                }
            }
        }

        public string LogMessage
        {
            get { return logMessage; }
            set
            {
                if (logMessage != value)
                {
                    logMessage = value;
                    RaisePropertyChanged("LogMessage");
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
    }
}

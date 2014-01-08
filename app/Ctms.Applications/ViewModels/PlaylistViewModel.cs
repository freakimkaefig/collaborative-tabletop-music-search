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
        private ICommand playCommand;


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

        public ICommand PlayCommand
        {
            get { return playCommand; }
            set
            {
                if (playCommand != value)
                {
                    playCommand = value;
                    RaisePropertyChanged("PlayCommand");
                }
            }
        }
    }
}

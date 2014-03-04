using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Ctms.Applications.Views;
using Ctms.Domain.Objects;
using System.Collections.ObjectModel;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class PlaylistViewModel : ViewModel<IPlaylistView>
    {
        private bool _isValid = true;
        private Playlist _playlist;
        private ICommand _selectCmd;
        private ObservableCollection<Playlist> _playlists;


        [ImportingConstructor]
        public PlaylistViewModel(IPlaylistView view)
            : base(view)
        {
            _playlists = new ObservableCollection<Playlist>()
            {
                new Playlist() { Id = 1, Name = "Playlist1" },
                new Playlist() { Id = 2, Name = "Playlist2" },
            };
        }


        public bool IsEnabled { get { return true; } }//Playlist != null;//!! Has to be adjusted

        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    RaisePropertyChanged("IsValid");
                }
            }
        }

        public ObservableCollection<Playlist> Playlists
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

        public Playlist Playlist
        {
            get { return _playlist; }
            set
            {
                if (_playlist != value)
                {
                    _playlist = value;
                    RaisePropertyChanged("Playlist");
                }
            }
        }

        public ICommand SelectCmd
        {
            get { return _selectCmd; }
            set
            {
                if (_selectCmd != value)
                {
                    _selectCmd = value;
                    RaisePropertyChanged("SelectCmd");
                }
            }
        }
    }
}

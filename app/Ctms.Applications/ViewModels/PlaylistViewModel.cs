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
using SpotifySharp;
using Ctms.Applications.DataModels;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class PlaylistViewModel : ViewModel<IPlaylistView>
    {
        private bool _isValid = true;
        private bool _readyForPlayback = false;
        private bool _playing = false;
        private bool _prelistening = false;

        //Commands
        private ICommand _playCommand;
        private ICommand _pauseCommand;
        private ICommand _stopCommand;
        private ICommand _addTrackCommand;
        //
        private ObservableCollection<ResultDataModel> _playlistResults;
        private Playlist _currentPlaylist = null;

        [ImportingConstructor]
        public PlaylistViewModel(IPlaylistView view)
            : base(view)
        {
            _playlistResults = new ObservableCollection<ResultDataModel>();
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

        {
            get { return _readyForPlayback; }
            set
            {
                if (_readyForPlayback != value)
                {
                    _readyForPlayback = value;
                    RaisePropertyChanged("ReadyForPlayback");
                }
            }
        }

        public bool Playing
        {
            get { return _playing; }
            set
            {
                if (_playing != value)
                {
                    _playing = value;
                    RaisePropertyChanged("Playing");
                }
            }
        }

        public bool Prelistening
        {
            get { return _prelistening; }
            set
            {
                if (_prelistening != value)
                {
                    _prelistening = value;
                    RaisePropertyChanged("Prelistening");
                }
            }
        }

        public ObservableCollection<ResultDataModel> ResultsForPlaylist
        {
            get { return _playlistResults; }
            set
            {
                if (_playlistResults != value)
                {
                    _playlistResults = value;
                    RaisePropertyChanged("ResultsForPlaylist");
                }
            }
        }

        public Playlist CurrentPlaylist
        {
            get { return _currentPlaylist; }
            set
            {
                if (_currentPlaylist != value)
                {
                    _currentPlaylist = value;
                    RaisePropertyChanged("CurrentPlaylist");
                }
            }
        }

        public ICommand PlayCommand
        {
            get { return _playCommand; }
            set
            {
                if (_playCommand != value)
                {
                    _playCommand = value;
                    RaisePropertyChanged("PlayCommand");
                }
            }
        }

        public ICommand PauseCommand
        {
            get { return _pauseCommand; }
            set
            {
                if (_pauseCommand != value)
                {
                    _pauseCommand = value;
                    RaisePropertyChanged("PauseCommand");
                }
            }
        }

        public ICommand StopCommand
        {
            get { return _stopCommand; }
            set
            {
                if (_stopCommand != value)
                {
                    _stopCommand = value;
                    RaisePropertyChanged("StopCommand");
                }
            }
        }

        public ICommand AddTrackCommand
        {
            get { return _addTrackCommand; }
            set
            {
                if (_addTrackCommand != value)
                {
                    _addTrackCommand = value;
                    RaisePropertyChanged("AddTrackCommand");
                }
            }
        }
    }
}

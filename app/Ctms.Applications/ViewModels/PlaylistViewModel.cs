using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Ctms.Applications.Views;
using Ctms.Domain.Objects;
using SpotifySharp;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class PlaylistViewModel : ViewModel<IPlaylistView>
    {
        private bool isValid = true;
        private bool _canPlayTrack = true;
        private bool _canPauseTrack = true;
        private bool _canStopPlayback = true;

        private Ctms.Domain.Objects.Playlist _playlist;
        private Track _currentTrack;

        //Commands
        private ICommand _playCommand;
        private ICommand _pauseCommand;
        private ICommand _stopCommand;

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

        public bool CanPlayTrack
        {
            get { return _canPlayTrack; }
            set
            {
                if (_canPlayTrack != value)
                {
                    _canPlayTrack = value;
                    RaisePropertyChanged("CanPlayTrack");
                }
            }
        }

        public bool CanPauseTrack
        {
            get { return _canPauseTrack; }
            set
            {
                if (_canPauseTrack != value)
                {
                    _canPauseTrack = value;
                    RaisePropertyChanged("CanPauseTrack");
                }
            }
        }

        public bool CanStopPlayback
        {
            get { return _canStopPlayback; }
            set
            {
                if (_canStopPlayback != value)
                {
                    _canStopPlayback = value;
                    RaisePropertyChanged("CanStopPlayback");
                }
            }
        }

        public Ctms.Domain.Objects.Playlist Playlist
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

        public Track CurrentTrack
        {
            get { return _currentTrack; }
            set
            {
                if (_currentTrack != value)
                {
                    _currentTrack = value;
                    RaisePropertyChanged("CurrentTrack");
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
    }
}

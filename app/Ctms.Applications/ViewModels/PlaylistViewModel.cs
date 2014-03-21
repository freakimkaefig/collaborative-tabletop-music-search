﻿using System;
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
using System.Windows;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class PlaylistViewModel : ViewModel<IPlaylistView>
    {
        private IPlaylistView _view;
        private bool _isValid = true;
        private bool _readyForPlayback = false;
        private bool _playing = false;
        private bool _prelistening = false;
        private bool _isRotate = false;

        //Commands
        private ICommand _playCommand;
        private ICommand _pauseCommand;
        private ICommand _stopCommand;
        private ICommand _addTrackCommand;
        private ICommand _jumpToTrackCommand;
        private ICommand _rotateCommand;
        private ICommand _reorderTrackCommand;
        //
        private Playlist _currentPlaylist = null;
        private ObservableCollection<ResultDataModel> _playlistResults;

        [ImportingConstructor]
        public PlaylistViewModel(IPlaylistView view)
            : base(view)
        {
            _view = view;
            _playlistResults = new ObservableCollection<ResultDataModel>();
        }


        public void RotatePlaylistView()
        {
            VisualState visualState;
            if (IsRotate == false)
            {
                visualState = _view.VisualStateRotate180;
                IsRotate = true;
            }
            else
            {
                visualState = _view.VisualStateRotate0;
                IsRotate = false;
            }
            
            VisualStateManager.GoToState((FrameworkElement)_view, visualState.Name, true);
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

        public bool ReadyForPlayback
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

        public bool IsRotate
        {
            get { return _isRotate; }
            set
            {
                if (_isRotate != value)
                {
                    _isRotate = value;
                    RaisePropertyChanged("IsRotate");
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

        public ICommand JumpToTrackCommand
        {
            get { return _jumpToTrackCommand; }
            set
            {
                if (_jumpToTrackCommand != value)
                {
                    _jumpToTrackCommand = value;
                    RaisePropertyChanged("JumpToTrackCommand");
                }
            }
        }

        public ICommand RotateCommand
        {
            get { return _rotateCommand; }
            set
            {
                if (_rotateCommand != value)
                {
                    _rotateCommand = value;
                    RaisePropertyChanged("RotateCommand");
                }
            }
        }

        public ICommand ReorderTrackCommand
        {
            get { return _reorderTrackCommand; }
            set
            {
                if (_reorderTrackCommand != value)
                {
                    _reorderTrackCommand = value;
                    RaisePropertyChanged("ReorderTrackCommand");
                }
            }
        }
    }
}

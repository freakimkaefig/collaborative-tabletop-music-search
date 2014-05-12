using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using Ctms.Applications.Properties;
using Ctms.Applications.Services;
using Ctms.Applications.ViewModels;
using Ctms.Domain;
using System.IO;
using System.Data.EntityClient;
using System.Data.Common;
using System.ComponentModel.Composition.Hosting;
using Ctms.Applications.Views;
using MusicStream;
using Ctms.Domain.Objects;
using Ctms.Applications.Workers;
using Ctms.Domain.Objects;
using Ctms.Applications.DataModels;
using System.Windows;


namespace Ctms.Applications.Controllers
{
    //!!Note: The content of this class is just an example and has to be adjusted.

    /// <summary>
    /// Responsible for the playlist management.
    /// </summary>
    [Export]
    internal class PlaylistController : Controller
    {
        private readonly CompositionContainer container;
        //Services
        private readonly IShellService shellService;
        private readonly EntityService entityService;
        private readonly IMessageService _messageService;
        //ViewModels
        private PlaylistViewModel _playlistViewModel;
        //Worker
        private StreamingWorker _streamingWorker;
        private PlaylistWorker _playlistWorker;
        private SearchWorker _searchWorker;
        //Commands
        private readonly DelegateCommand _playPauseCommand;
        private readonly DelegateCommand _stopCommand;
        private readonly DelegateCommand _removeTrackCommand;
        private readonly DelegateCommand _jumpToTrackCommand;
        private readonly DelegateCommand _rotateCommand;
        private readonly DelegateCommand _reorderTrackCommand;
        private readonly DelegateCommand _shuffleCommand;
        private readonly DelegateCommand _repeatCommand;
        private readonly DelegateCommand _loadDetailsCommand;
        private readonly DelegateCommand _showPlaylistCommand;

        //Further vars
        //private SynchronizingCollection<BookDataModel, Book> bookDataModels;

        [ImportingConstructor]
        public PlaylistController(CompositionContainer container, IShellService shellService, EntityService entityService,
            PlaylistViewModel playlistViewModel, StreamingWorker streamingWorker, PlaylistWorker playlistWorker, SearchWorker searchWorker)
        {
            this.container = container;
            //Services
            this.shellService = shellService;
            this.entityService = entityService;
            //ViewModels
            _playlistViewModel = playlistViewModel;
            //Worker
            _streamingWorker = streamingWorker;
            _playlistWorker = playlistWorker;
            _searchWorker = searchWorker;
            //Commands
            this._playPauseCommand = new DelegateCommand(_streamingWorker.PlaylistPlayPause, _playlistWorker.CanPlay);
            this._stopCommand = new DelegateCommand(_streamingWorker.StopPlayback, _streamingWorker.Playing);
            this._removeTrackCommand = new DelegateCommand((index) => _playlistWorker.RemoveTrackFromPlaylist((int)index));
            this._jumpToTrackCommand = new DelegateCommand((index) => _playlistWorker.JumpToTrack((int)index));
            this._rotateCommand = new DelegateCommand(_playlistViewModel.RotatePlaylistView);
            this._reorderTrackCommand = new DelegateCommand((data) => _playlistWorker.ReorderTrack((object[])data));
            this._shuffleCommand = new DelegateCommand(_playlistWorker.ToggleShuffle, _streamingWorker.CanStream);
            this._repeatCommand = new DelegateCommand(_playlistWorker.ToggleRepeat, _streamingWorker.CanStream);
            this._loadDetailsCommand = new DelegateCommand((result) => _searchWorker.LoadDetails((ResultDataModel)result));
            this._showPlaylistCommand = new DelegateCommand(_playlistViewModel.ShowPlaylist);
        }

        public void Initialize()
        {
            //_playlistViewModel = new PlaylistViewModel(playlistView);
            //Commands
            _playlistViewModel.PlayPauseCommand = _playPauseCommand;
            _playlistViewModel.StopCommand = _stopCommand;
            _playlistViewModel.RemoveTrackCommand = _removeTrackCommand;
            _playlistViewModel.JumpToTrackCommand = _jumpToTrackCommand;
            _playlistViewModel.RotateCommand = _rotateCommand;
            _playlistViewModel.ReorderTrackCommand = _reorderTrackCommand;
            _playlistViewModel.ShuffleCommand = _shuffleCommand;
            _playlistViewModel.RepeatCommand = _repeatCommand;
            _playlistViewModel.LoadDetailsCommand = _loadDetailsCommand;
            _playlistViewModel.ShowPlaylistCommand = _showPlaylistCommand;
            AddWeakEventListener(_playlistViewModel, PlaylistViewModelPropertyChanged);

            shellService.PlaylistView = _playlistViewModel.View;

            _playlistViewModel.PlayPauseIcon = (System.Windows.Media.DrawingBrush)Application.Current.Resources["play"];
        }

        private void UpdateCommands()
        {
            _playPauseCommand.RaiseCanExecuteChanged();
            _stopCommand.RaiseCanExecuteChanged();
            _shuffleCommand.RaiseCanExecuteChanged();
            _repeatCommand.RaiseCanExecuteChanged();
        }

        private void PlaylistViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSong")//SelectedSong is just an example
            {
                //...
                UpdateCommands();
            }

            if (e.PropertyName == "Prelistening")
            {
                //
            }

            if (e.PropertyName == "CurrentTrack")//SelectedSong is just an example
            {
                //...
                UpdateCommands();
            }

            if (e.PropertyName == "ReadyForPlayback")
            {
                //...
                UpdateCommands();
            }
            if (e.PropertyName == "PlayPauseCommand")
            {
                //...
                UpdateCommands();
            }

            if (e.PropertyName == "Playing")
            {
                if (!_playlistViewModel.Playing)
                {
                    _playlistViewModel.PlayPauseIcon = (System.Windows.Media.DrawingBrush)Application.Current.Resources["play"];
                }
                else
                {
                    foreach (var result in _playlistViewModel.ResultsForPlaylist)
                    {
                        result.IsPlaying = false;
                    }
                    _playlistViewModel.PlayPauseIcon = (System.Windows.Media.DrawingBrush)Application.Current.Resources["pause"];
                }
                UpdateCommands();
            }

            if (e.PropertyName == "CanPlay")
            {
                UpdateCommands();
            }
        }
    }
}

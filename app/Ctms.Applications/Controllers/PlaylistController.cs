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
        //Commands
        private readonly DelegateCommand _playPauseCommand;
        private readonly DelegateCommand _stopCommand;
        private readonly DelegateCommand _addTrackCommand;
        private readonly DelegateCommand _jumpToTrackCommand;
        private readonly DelegateCommand _rotateCommand;
        private readonly DelegateCommand _reorderTrackCommand;
        private readonly DelegateCommand _shuffleCommand;
        private readonly DelegateCommand _repeatCommand;

        //Further vars
        //private SynchronizingCollection<BookDataModel, Book> bookDataModels;

        [ImportingConstructor]
        public PlaylistController(CompositionContainer container, IShellService shellService, EntityService entityService,
            PlaylistViewModel playlistViewModel, StreamingWorker streamingWorker, PlaylistWorker playlistWorker)
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
            //Commands
            this._playPauseCommand = new DelegateCommand(_streamingWorker.PlaylistPlayPause, _streamingWorker.CanStream);
            this._stopCommand = new DelegateCommand(_streamingWorker.StopPlayback, _streamingWorker.Playing);
            this._addTrackCommand = new DelegateCommand((data) => _playlistWorker.AddTrackToPlaylist((object[])data));
            this._jumpToTrackCommand = new DelegateCommand((index) => _playlistWorker.JumpToTrack((int)index));
            this._rotateCommand = new DelegateCommand(_playlistViewModel.RotatePlaylistView);
            this._reorderTrackCommand = new DelegateCommand((data) => _playlistWorker.ReorderTrack((object[])data));
            this._shuffleCommand = new DelegateCommand(_playlistWorker.ToggleShuffle, _streamingWorker.CanStream);
            this._repeatCommand = new DelegateCommand(_playlistWorker.ToggleRepeat, _streamingWorker.CanStream);
        }

        public void Initialize()
        {
            //_playlistViewModel = new PlaylistViewModel(playlistView);
            //Commands
            _playlistViewModel.PlayPauseCommand = _playPauseCommand;
            _playlistViewModel.StopCommand = _stopCommand;
            _playlistViewModel.AddTrackCommand = _addTrackCommand;
            _playlistViewModel.JumpToTrackCommand = _jumpToTrackCommand;
            _playlistViewModel.RotateCommand = _rotateCommand;
            _playlistViewModel.ReorderTrackCommand = _reorderTrackCommand;
            _playlistViewModel.ShuffleCommand = _shuffleCommand;
            _playlistViewModel.RepeatCommand = _repeatCommand;
            AddWeakEventListener(_playlistViewModel, PlaylistViewModelPropertyChanged);

            shellService.PlaylistView = _playlistViewModel.View;
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
                    _playlistViewModel.PlayPauseText = "Play";
                }
                else
                {
                    _playlistViewModel.PlayPauseText = "Pause";
                }
                UpdateCommands();
            }
        }
    }
}

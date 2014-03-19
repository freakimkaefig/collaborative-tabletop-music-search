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
        private readonly DelegateCommand _playCommand;
        private readonly DelegateCommand _pauseCommand;
        private readonly DelegateCommand _stopCommand;
        private readonly DelegateCommand _addTrackCommand;
        private readonly DelegateCommand _jumpToTrackCommand;
        private readonly DelegateCommand _rotateCommand;

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
            this._playCommand = new DelegateCommand(_streamingWorker.PlayCurrentTrack, _streamingWorker.CanStream);
            this._pauseCommand = new DelegateCommand(_streamingWorker.PauseCurrentTrack, _streamingWorker.Playing);
            this._stopCommand = new DelegateCommand(_streamingWorker.StopPlayback, _streamingWorker.Playing);
            this._addTrackCommand = new DelegateCommand((result) => _playlistWorker.AddTrackToPlaylist((ResultDataModel)result));
            this._jumpToTrackCommand = new DelegateCommand((index) => _playlistWorker.JumpToTrack((int)index));
            this._rotateCommand = new DelegateCommand(_playlistViewModel.RotatePlaylistView);
        }

        public void Initialize()
        {
            //_playlistViewModel = new PlaylistViewModel(playlistView);
            //Commands
            _playlistViewModel.PlayCommand = _playCommand;
            _playlistViewModel.PauseCommand = _pauseCommand;
            _playlistViewModel.StopCommand = _stopCommand;
            _playlistViewModel.AddTrackCommand = _addTrackCommand;
            _playlistViewModel.JumpToTrackCommand = _jumpToTrackCommand;
            _playlistViewModel.RotateCommand = _rotateCommand;
            AddWeakEventListener(_playlistViewModel, PlaylistViewModelPropertyChanged);

            shellService.PlaylistView = _playlistViewModel.View;
        }

        private void UpdateCommands()
        {
            _playCommand.RaiseCanExecuteChanged();
            _pauseCommand.RaiseCanExecuteChanged();
            _stopCommand.RaiseCanExecuteChanged();
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
        }
    }
}

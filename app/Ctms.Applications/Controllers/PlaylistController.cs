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
using Ctms.Applications.Workers;


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
        private readonly IEntityService entityService;
        //ViewModels
        private PlaylistViewModel _playlistViewModel;
        //Worker
        private StreamingWorker _streamingWorker;
        //Commands
        private readonly DelegateCommand _playCommand;
        private readonly DelegateCommand _pauseCommand;
        private readonly DelegateCommand _stopCommand;

        //Further vars
        //private SynchronizingCollection<BookDataModel, Book> bookDataModels;

        [ImportingConstructor]
        public PlaylistController(CompositionContainer container, IShellService shellService, IEntityService entityService,
            PlaylistViewModel playlistViewModel, StreamingWorker streamingWorker)
        {
            this.container = container;
            //Services
            this.shellService = shellService;
            this.entityService = entityService;
            //ViewModels
            _playlistViewModel = playlistViewModel;
            //Worker
            _streamingWorker = streamingWorker;
            //Commands
            this._playCommand = new DelegateCommand(_streamingWorker.PlayCurrentTrack, _streamingWorker.CanStream);
            this._pauseCommand = new DelegateCommand(_streamingWorker.PauseCurrentTrack, _streamingWorker.Playing);
            this._stopCommand = new DelegateCommand(_streamingWorker.StopPlayback, _streamingWorker.Playing);
        }

        public void Initialize()
        {
            IPlaylistView playlistView = container.GetExportedValue<IPlaylistView>();
            //_playlistViewModel = new PlaylistViewModel(playlistView);
            //Commands
            _playlistViewModel.PlayCommand = _playCommand;
            _playlistViewModel.PauseCommand = _pauseCommand;
            _playlistViewModel.StopCommand = _stopCommand;
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

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
        private PlaylistViewModel playlistViewModel;
        //Commands
        private readonly DelegateCommand loginCommand;
        //Further vars
        //private SynchronizingCollection<BookDataModel, Book> bookDataModels;
        private MusicStreamManager musicStreamManager;

        [ImportingConstructor]
        public PlaylistController(CompositionContainer container, IShellService shellService, IEntityService entityService,
            PlaylistViewModel playlistViewModel)
        {
            this.container = container;
            //Services
            this.shellService = shellService;
            this.entityService = entityService;
            //ViewModels
            this.playlistViewModel = playlistViewModel;
            //Commands
            this.loginCommand = new DelegateCommand(Login, CanLogin);
        }

        public void Initialize()
        {
            AddWeakEventListener(playlistViewModel, PlaylistViewModelPropertyChanged);

            IPlaylistView playlistView = container.GetExportedValue<IPlaylistView>();
            playlistViewModel = new PlaylistViewModel(playlistView);
            playlistViewModel.LoginCommand = loginCommand;
            AddWeakEventListener(playlistViewModel, PlaylistViewModelPropertyChanged);

            shellService.PlaylistView = playlistViewModel.View;
        }


        private bool CanLogin() { return playlistViewModel.IsValid; }

        private void Login()
        {
            //do stuff
            musicStreamManager = new MusicStreamManager();
            musicStreamManager.receiveLogMessage = ReceiveLogMessage;
            musicStreamManager.Login();
        }

        public void ReceiveLogMessage(string logMessage)
        {
            playlistViewModel.LogMessage += "\n" + logMessage;
        }

        private void UpdateCommands()
        {

        }

        private void PlaylistViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSong")//SelectedSong is just an example
            {
                //...
                UpdateCommands();
            }
        }
    }
}

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
using Ctms.Domain.Objects;


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
        private PlaylistViewModel _playlistVm;
        //Commands
        private readonly DelegateCommand _selectCmd;
        //Further vars
        //private SynchronizingCollection<BookDataModel, Book> bookDataModels;

        [ImportingConstructor]
        public PlaylistController(CompositionContainer container, IShellService shellService, EntityService entityService,
            PlaylistViewModel playlistVm, IMessageService messageService)
        {
            this.container = container;
            //Services
            this.shellService = shellService;
            this.entityService = entityService;
            _messageService = messageService;
            //ViewModels
            _playlistVm = playlistVm;
            //Commands
            _selectCmd = new DelegateCommand((playlist) => SelectPlaylist((Playlist)playlist));
        }

        public void Initialize()
        {
            AddWeakEventListener(_playlistVm, PlaylistViewModelPropertyChanged);

            _playlistVm.SelectCmd = _selectCmd;

            shellService.PlaylistView = _playlistVm.View;
        }

        private void UpdateCommands()
        {

        }

        public void SelectPlaylist(Playlist playlist)
        {
            MessageServiceExtensions.ShowMessage(_messageService, "Clicked " + playlist.Name);
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

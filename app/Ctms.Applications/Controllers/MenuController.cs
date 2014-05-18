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
using Helpers;
using Microsoft.Surface.Presentation.Controls;
using SpotifySharp;
using Ctms.Domain.Objects;


namespace Ctms.Applications.Controllers
{

    /// <summary>
    /// Responsible for the menu management.
    /// </summary>
    [Export]
    internal class MenuController : Controller
    {
        private readonly CompositionContainer _container;
        //Services
        private readonly IShellService _shellService;
        private readonly EntityService _entityService;
        //ViewModels
        private MenuViewModel _menuViewModel;
        private ShellViewModel _shellViewModel;
        //Worker
        private MusicStreamAccountWorker _musicStreamAccountWorker;
        //Commands
        private readonly DelegateCommand _exitAppCommand;
        private readonly DelegateCommand _loginCommand;
        private readonly DelegateCommand _cancelLoginCommand;
        private readonly DelegateCommand _logoutCommand;
        private readonly DelegateCommand _openPlaylistCommand;
        private readonly DelegateCommand _newPlaylistCommand;
        private readonly DelegateCommand _rotateMenuCommand;

        private readonly DelegateCommand _displayLoginDialog;
        private readonly DelegateCommand _displayNewPlaylistCommand;
        private readonly DelegateCommand _displayOpenPlaylistCommand;
        //Further vars
        
        //private SynchronizingCollection<BookDataModel, Book> bookDataModels;

        [ImportingConstructor]
        public MenuController(CompositionContainer container, IShellService shellService, EntityService entityService,
            MenuViewModel menuViewModel, ShellViewModel shellViewModel, MusicStreamAccountWorker musicStreamAccountWorker)
        {
            this._container = container;
            //Services
            this._shellService = shellService;
            this._entityService = entityService;
            //ViewModels
            this._menuViewModel = menuViewModel;
            this._shellViewModel = shellViewModel;
            //Worker
            _musicStreamAccountWorker = musicStreamAccountWorker;
            //Commands
            this._exitAppCommand = new DelegateCommand(ExitApp, CanExitApp);
            this._loginCommand = new DelegateCommand((password) => _musicStreamAccountWorker.Login((SurfacePasswordBox)password));
            this._cancelLoginCommand = new DelegateCommand(_musicStreamAccountWorker.CancelLogin);
            this._logoutCommand = new DelegateCommand(_musicStreamAccountWorker.Logout);
            this._openPlaylistCommand = new DelegateCommand((playlist) => _musicStreamAccountWorker.OpenPlaylist((SpotifyPlaylist)playlist));
            this._newPlaylistCommand = new DelegateCommand((name) => _musicStreamAccountWorker.CreateNewPlaylist((SurfaceTextBox)name));
            this._rotateMenuCommand = new DelegateCommand(_menuViewModel.RotateMenu);

            _displayLoginDialog = new DelegateCommand(_menuViewModel.ToggleLoginDialog);
            _displayNewPlaylistCommand = new DelegateCommand(_menuViewModel.ToggleNewPlaylist);
            _displayOpenPlaylistCommand = new DelegateCommand(_menuViewModel.ToggleOpenPlaylist);
        }

        public void Initialize()
        {
            IMenuView menuView = _container.GetExportedValue<IMenuView>();
            //Commands
            _menuViewModel.ExitAppCommand = _exitAppCommand;
            _menuViewModel.LoginCommand = _loginCommand;
            _menuViewModel.CancelLoginCommand = _cancelLoginCommand;
            _menuViewModel.LogoutCommand = _logoutCommand;
            _menuViewModel.OpenPlaylistCommand = _openPlaylistCommand;
            _menuViewModel.NewPlaylistCommand = _newPlaylistCommand;
            _menuViewModel.RotateMenuCommand = _rotateMenuCommand;

            _menuViewModel.DisplayLoginDialog = _displayLoginDialog;
            _menuViewModel.DisplayNewPlaylist = _displayNewPlaylistCommand;
            _menuViewModel.DisplayOpenPlaylist = _displayOpenPlaylistCommand;

            AddWeakEventListener(_menuViewModel, MenuViewModelPropertyChanged);

            _shellService.MenuView = _menuViewModel.View;
        }

        private void UpdateCommands()
        {
            _loginCommand.RaiseCanExecuteChanged();
            _logoutCommand.RaiseCanExecuteChanged();
            _displayOpenPlaylistCommand.RaiseCanExecuteChanged();
        }

        private void ExitApp()
        {
            _menuViewModel.LogoutCommand.Execute(null);
            _shellViewModel.ExitCommand.Execute(null);
        }

        private bool CanExitApp() { return _shellViewModel.IsValid; }

        private void MenuViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsLoggedIn")
            {
                if (_menuViewModel.IsLoggedIn == true)
                {
                    _menuViewModel.CanLogin = false;
                    _menuViewModel.LoginButtonContent = "Spotify Logout";
                }
                else
                {
                    _menuViewModel.CanLogin = true;
                    _menuViewModel.LoginButtonContent = "Spotify Login";
                }
                UpdateCommands();
            }

            if (e.PropertyName == "CanLogin")
            {

            }

            if (e.PropertyName == "MenuIsVisible")
            {
                //
            }

            if (e.PropertyName == "SpotifyUsernameInput")
            {
                //
            }

            if (e.PropertyName == "SpotifyPasswordInput")
            {
                //
            }

            if (e.PropertyName == "Playlists")
            {
                //
            }

            if (e.PropertyName == "PlaylistName")
            {
                //
            }
        }

        /*
        private void ShellViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSong")//SelectedSong is just an example
            {
                //...
                UpdateCommands();
            }
        }
        */ 
    }
}

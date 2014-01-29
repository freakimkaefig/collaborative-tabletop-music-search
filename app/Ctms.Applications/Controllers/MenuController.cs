﻿using System;
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
    internal class MenuController : Controller
    {
        private readonly CompositionContainer _container;
        //Services
        private readonly IShellService _shellService;
        private readonly IEntityService _entityService;
        //ViewModels
        private MenuViewModel _menuViewModel;
        private ShellViewModel _shellViewModel;
        //Worker
        private MusicStreamAccountWorker _musicStreamAccountWorker;
        //Commands
        private readonly DelegateCommand _exitAppCommand;
        private readonly DelegateCommand _loginCommand;
        //Further vars
        
        //private SynchronizingCollection<BookDataModel, Book> bookDataModels;

        [ImportingConstructor]
        public MenuController(CompositionContainer container, IShellService shellService, IEntityService entityService,
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
            this._loginCommand = new DelegateCommand(_musicStreamAccountWorker.Login, _musicStreamAccountWorker.CanLogin);
        }

        public void Initialize()
        {
            IMenuView menuView = _container.GetExportedValue<IMenuView>();
            _menuViewModel.ExitAppCommand = _exitAppCommand;
            _menuViewModel.LoginCommand = _loginCommand;
            AddWeakEventListener(_menuViewModel, MenuViewModelPropertyChanged);

            _shellService.MenuView = _menuViewModel.View;
        }

        private void UpdateCommands()
        {

        }

        private void ExitApp()
        {
            //!!Best option?
            _shellViewModel.ExitCommand.Execute(null);
        }

        private bool CanExitApp() { return _shellViewModel.IsValid; }

        private void MenuViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsLoggedIn")
            {
                //
            }

            if (e.PropertyName == "PlaylistContainerLoaded")
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

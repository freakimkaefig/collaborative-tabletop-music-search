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


namespace Ctms.Applications.Controllers
{
    //!!Note: The content of this class is just an example and has to be adjusted.

    /// <summary>
    /// Responsible for the playlist management.
    /// </summary>
    [Export]
    internal class MenuController : Controller
    {
        private readonly CompositionContainer container;
        //Services
        private readonly IShellService shellService;
        private readonly IEntityService entityService;
        //ViewModels
        private MenuViewModel menuViewModel;
        private ShellViewModel shellViewModel;
        //Commands
        private readonly DelegateCommand exitAppCommand;
        //Further vars
        //private SynchronizingCollection<BookDataModel, Book> bookDataModels;

        [ImportingConstructor]
        public MenuController(CompositionContainer container, IShellService shellService, IEntityService entityService,
            MenuViewModel menuViewModel, ShellViewModel shellViewModel)
        {
            this.container = container;
            //Services
            this.shellService = shellService;
            this.entityService = entityService;
            //ViewModels
            this.menuViewModel = menuViewModel;
            this.shellViewModel = shellViewModel;
            //Commands
            this.exitAppCommand = new DelegateCommand(ExitApp, CanExitApp);
        }

        public void Initialize()
        {
            AddWeakEventListener(menuViewModel, MenuViewModelPropertyChanged);
            //AddWeakEventListener(shellViewModel, ShellViewModelPropertyChanged);

            IMenuView menuView = container.GetExportedValue<IMenuView>();
            menuViewModel = new MenuViewModel(menuView);
            menuViewModel.ExitAppCommand = exitAppCommand;
            AddWeakEventListener(menuViewModel, MenuViewModelPropertyChanged);

            shellService.MenuView = menuViewModel.View;
        }

        private void UpdateCommands()
        {

        }

        private void ExitApp()
        {
            //!!Best option?
            shellViewModel.ExitCommand.Execute(null);
        }

        private bool CanExitApp() { return shellViewModel.IsValid; }

        private void MenuViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSong")//SelectedSong is just an example
            {
                //...
                UpdateCommands();
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

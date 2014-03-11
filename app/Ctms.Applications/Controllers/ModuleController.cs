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
using System.Diagnostics;
using Ctms.Applications.Views;

namespace Ctms.Applications.Controllers
{
    /// <summary>
    /// Responsible for the module lifecycle. Starting point of this module. Intializes the other controllers.
    /// </summary>
    [Export(typeof(IModuleController)), Export]
    internal class ModuleController : Controller, IModuleController
    {
        //Controllers
        private readonly IMessageService    messageService;
        private readonly IEntityController  entityController;
        private readonly SearchController   searchController;
        private readonly ResultController   resultController;
        private readonly PlaylistController playlistController;
        private readonly DetailController   detailController;
        private readonly MenuController     menuController;
        //Services
        private readonly IShellService      shellService;
        //ViewModels
        private readonly ShellViewModel     shellViewModel;
        //Commands
        private readonly DelegateCommand    exitCommand;
        //Furthers vars
        private SearchTagVisualizationService _tagVisualitationService;

        [ImportingConstructor]
        public ModuleController(IMessageService messageService, IPresentationService presentationService,
            IEntityController entityController, SearchController searchController, ResultController resultController,
            PlaylistController playlistController, DetailController detailController, MenuController menuController,
            ShellService shellService, ShellViewModel shellViewModel)
        {
            presentationService.InitializeCultures();
            //Controller
            this.entityController   = entityController;
            this.searchController   = searchController;
            this.resultController   = resultController;
            this.playlistController = playlistController;
            this.detailController   = detailController;
            this.menuController     = menuController;
            //Services
            this.messageService     = messageService;
            this.shellService       = shellService;
            //ViewModels
            this.shellViewModel     = shellViewModel;
            //Views
            shellService.ShellView = shellViewModel.View;
            //Events & Commands
            this.shellViewModel.Closing += ShellViewModelClosing;
            this.exitCommand = new DelegateCommand(Close);
        }

        public void Initialize()
        {
            shellViewModel.ExitCommand = exitCommand;

            entityController.Initialize();
            searchController.Initialize();
            resultController.Initialize();
            playlistController.Initialize();
            detailController.Initialize();
            menuController.Initialize();
        }

        public void Run()
        {
            shellViewModel.Show();
        }

        public void Shutdown()
        {
            entityController.Shutdown();

            try
            {
                Settings.Default.Save();
            }
            catch (Exception)
            {
                // When more application instances are closed at the same time then an exception occurs.
            }
        }

        private void ShellViewModelClosing(object sender, CancelEventArgs e)
        {
            if (entityController.HasChanges)
            {
                if (entityController.CanSave())
                {
                    bool? result = messageService.ShowQuestion(shellService.ShellView, "Änderungen speichern?");
                    if (result == true)
                    {
                        if (!entityController.Save())
                        {
                            e.Cancel = true;
                        }
                    }
                    else if (result == null)
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    e.Cancel = !messageService.ShowYesNoQuestion(shellService.ShellView, "Änderungen verwerfen?");
                }
            }
        }

        private void Close()
        {
            shellViewModel.Close();
        }
    }
}

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
using Ctms.Applications.Workers;
using Ctms.Applications.DevHelper;

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
        private readonly InfoController     infoController;
        //Services
        private readonly IShellService      shellService;
        //ViewModels
        private readonly ShellViewModel     shellViewModel;
        //Commands
        private readonly DelegateCommand    exitCommand;
        //Workers
        private InfoWorker infoWorker;

        [ImportingConstructor]
        public ModuleController(IMessageService messageService, IPresentationService presentationService,
            IEntityController entityController, ResultController resultController, SearchController searchController, 
            PlaylistController playlistController, DetailController detailController, MenuController menuController,
            InfoController infoController,
            ShellService shellService, ShellViewModel shellViewModel, InfoWorker infoWorker)
        {
            presentationService.InitializeCultures();
            //Controller
            this.entityController   = entityController;
            this.resultController   = resultController;
            this.searchController   = searchController;
            this.playlistController = playlistController;
            this.detailController   = detailController;
            this.menuController     = menuController;
            this.infoController     = infoController;
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
            //Workers
            this.infoWorker = infoWorker;
        }

        public void Initialize()
        {
            shellViewModel.ExitCommand = exitCommand;

            entityController.Initialize();
            resultController.Initialize();
            searchController.Initialize();
            playlistController.Initialize();
            detailController.Initialize();
            menuController.Initialize();
            infoController.Initialize();
        }

        public void Run()
        {
            shellViewModel.Show();
        }

        public void HandleException(Exception exception)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}{1}{2}{3}{4}{5}", exception.InnerException, Environment.NewLine,
                exception.Message, Environment.NewLine, 
                exception.StackTrace, Environment.NewLine);

            infoWorker.ShowCommonInfo("Error occurred", errorMessage, "Ok");
            //infoWorker.ShowCommonInfo("Error occured", "Sorry, please try again.", "Ok");
            //DevLogger.Log(errorMessage);
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

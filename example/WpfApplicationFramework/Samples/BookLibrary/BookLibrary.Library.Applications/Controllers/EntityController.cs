using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using Waf.BookLibrary.Library.Applications.Properties;
using Waf.BookLibrary.Library.Applications.Services;
using Waf.BookLibrary.Library.Applications.ViewModels;
using Waf.BookLibrary.Library.Domain;
using System.IO;
using System.Data.EntityClient;
using System.Data.Common;

namespace Waf.BookLibrary.Library.Applications.Controllers
{
    /// <summary>
    /// Responsible for the database connection and the save operation.
    /// </summary>
    [Export(typeof(IEntityController))]
    internal class EntityController : Controller, IEntityController
    {
        private const string ResourcesDirectoryName = "Resources";
        
        private readonly EntityService entityService;
        private readonly IMessageService messageService;
        private readonly IShellService shellService;
        private readonly ShellViewModel shellViewModel;
        private readonly DelegateCommand saveCommand;
        private BookLibraryEntities entities;


        [ImportingConstructor]
        public EntityController(EntityService entityService, IMessageService messageService, IShellService shellService, 
            ShellViewModel shellViewModel)
        {
            this.entityService = entityService;
            this.messageService = messageService;
            this.shellService = shellService;
            this.shellViewModel = shellViewModel;
            this.saveCommand = new DelegateCommand(() => Save(), CanSave);
        }


        public bool HasChanges
        {
            get { return entities != null && entities.HasChanges; }
        }


        public void Initialize()
        {
            // Create directory for the database.
            string dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ApplicationInfo.Company, ApplicationInfo.ProductName);
            if (!Directory.Exists(Path.Combine(dataDirectory, ResourcesDirectoryName)))
            {
                Directory.CreateDirectory(Path.Combine(dataDirectory, ResourcesDirectoryName));
            }

            // Set |DataDirectory| macro to our own path. This macro is used within the connection string.
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

            entities = new BookLibraryEntities();

            // Copy the template database file into the DataDirectory when it doesn't exists.
            DbConnection connection = entities.Connection;
            string dataSourcePath = connection.DataSource.Replace("|DataDirectory|", dataDirectory);
            if (!File.Exists(dataSourcePath))
            {
                string dbFile = Path.GetFileName(dataSourcePath);
                File.Copy(Path.Combine(ApplicationInfo.ApplicationPath, ResourcesDirectoryName, dbFile), dataSourcePath);
            }

            entityService.Entities = entities;

            AddWeakEventListener(shellViewModel, ShellViewModelPropertyChanged);
            shellViewModel.SaveCommand = saveCommand;
            shellViewModel.DatabasePath = dataSourcePath;
        }

        public void Shutdown()
        {
            entities.Dispose();
        }

        public bool CanSave() { return shellViewModel.IsValid; }
        
        public bool Save()
        {
            bool saved = false;
            if (!CanSave()) 
            { 
                throw new InvalidOperationException("You must not call Save when CanSave returns false."); 
            }
            try
            {
                entities.SaveChanges();
                saved = true;
            }
            catch (ValidationException e)
            {
                messageService.ShowError(shellService.ShellView, string.Format(CultureInfo.CurrentCulture, 
                    Resources.SaveErrorInvalidEntities, e.Message));
            }
            catch (UpdateException e)
            {
                messageService.ShowError(shellService.ShellView, string.Format(CultureInfo.CurrentCulture, 
                    Resources.SaveErrorInvalidFields, e.InnerException.Message));
            }
            return saved;
        }

        private void ShellViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsValid")
            {
                saveCommand.RaiseCanExecuteChanged();
            }
        }
    }
}

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
using Ctms.Applications.Common;
using Ctms.Applications.Data;
using Ctms.Applications.DevHelper;

namespace Ctms.Applications.Controllers
{
    /// <summary>
    /// Responsible for the database connection and the save operation.
    /// </summary>
    [Export(typeof(IEntityController))]
    internal class EntityController : Controller, IEntityController
    {
        private const string ResourcesDirectoryName = "Resources";

        private readonly EntityService _entityService;
        private readonly IMessageService _messageService;
        private readonly IShellService _shellService;
        private readonly ShellViewModel _shellViewModel;
        private readonly Repository _repository;
        private readonly DelegateCommand _saveCommand;
        private CtmsEntities _entities;


        [ImportingConstructor]
        public EntityController(EntityService entityService, IMessageService messageService, IShellService shellService,
            ShellViewModel shellViewModel, Repository repository)
        {
            Configurator.Init();
            DevDataProvider.Initialize(_repository);
            DevDataProvider.LoadTags();

            _entityService = entityService;
            _messageService = messageService;
            _shellService = shellService;
            _shellViewModel = shellViewModel;
            _saveCommand = new DelegateCommand(() => Save(), CanSave);

            _repository = repository;
        }

        
        public bool HasChanges
        {
            //!!get { return entities != null && entities.HasChanges; }
            get { return false; }
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

            //entities = new CtmsEntities();
            /*//!!
            // Copy the template database file into the DataDirectory when it doesn't exists.
            DbConnection connection = entities.Connection;
            string dataSourcePath = connection.DataSource.Replace("|DataDirectory|", dataDirectory);
            if (!File.Exists(dataSourcePath))
            {
                string dbFile = Path.GetFileName(dataSourcePath);
                File.Copy(Path.Combine(ApplicationInfo.ApplicationPath, ResourcesDirectoryName, dbFile), dataSourcePath);
            }

            entityService.Entities = entities;
            */
            AddWeakEventListener(_shellViewModel, ShellViewModelPropertyChanged);
            _shellViewModel.SaveCommand = _saveCommand;
            //!!shellViewModel.DatabasePath = dataSourcePath;

        }

        public void Shutdown()
        {
            //!!entities.Dispose();
        }

        public bool CanSave() { return _shellViewModel.IsValid; }

        public bool Save()
        {
            bool saved = false;
            if (!CanSave())
            {
                throw new InvalidOperationException("You must not call Save when CanSave returns false.");
            }
            try
            {
                //!!entities.SaveChanges();
                saved = true;
            }
            catch (ValidationException e)
            {
                _messageService.ShowError(_shellService.ShellView, string.Format(CultureInfo.CurrentCulture,
                    Resources.SaveErrorInvalidEntities, e.Message));
            }
            catch (UpdateException e)
            {
                _messageService.ShowError(_shellService.ShellView, string.Format(CultureInfo.CurrentCulture,
                    Resources.SaveErrorInvalidFields, e.InnerException.Message));
            }
            return saved;
        }

        private void ShellViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsValid")
            {
                _saveCommand.RaiseCanExecuteChanged();
            }
        }
    }
}

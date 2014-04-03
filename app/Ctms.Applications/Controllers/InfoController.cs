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
using Ctms.Applications.DataFactories;
using Ctms.Applications.Data;
using System.Windows;
using Ctms.Applications.Workers;


namespace Ctms.Applications.Controllers
{
    //!!Note: The content of this class is just an example and has to be adjusted.

    /// <summary>
    /// Responsible for the detail management.
    /// </summary>
    [Export]
    internal class InfoController : Controller
    {
        //Services
        private readonly IShellService _shellService;
        private readonly EntityService _entityService;
        //ViewModels
        private InfoViewModel _infoVm;
        //Commands
        private readonly DelegateCommand _confirmTagInfoCmd;
        //Further vars
        private Repository _repository;
        private InfoWorker _infoWorker;

        //private SynchronizingCollection<BookDataModel, Book> bookDataModels;

        [ImportingConstructor]
        public InfoController(CompositionContainer container, IShellService shellService, EntityService entityService,
            Repository repository, InfoViewModel infoViewModel, InfoWorker infoWorker)
        {
            //Services
            _shellService       = shellService;
            _entityService      = entityService;
            //ViewModels
            _infoVm         = infoViewModel;
            _infoWorker     = infoWorker;
            //Commands
            _confirmTagInfoCmd = new DelegateCommand((tagId) => _infoWorker.ConfirmTagInfo((int)tagId));
            //Further vars
            _repository = repository;
        }

        public void Initialize()
        {
            // views
            _shellService.InfoView = _infoVm.View;

            // assign commands
            _infoVm.ConfirmTagInfoCmd = _confirmTagInfoCmd;

            AddWeakEventListener(_infoVm, InfoViewModelPropertyChanged);

            //ShowCommonInfo("CommonInfoMain", "InfoSub");
            //ShowTagInfo("TagInfoMain", "InfoSub", 0);
            //ShowTutorialInfo("TutorialInfoMain", "InfoSub");

            _infoWorker.Initialize();
        }

       
        private void UpdateCommands()
        {

        }

        private bool CanSelectDetail() { return _infoVm.IsValid; }

        private void SelectDetail()
        {
        }

        private void InfoViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSong")//SelectedSong is just an example
            {
                //...
                UpdateCommands();
            }
        }
    }
}

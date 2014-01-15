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
    /// Responsible for the detail management.
    /// </summary>
    [Export]
    internal class DetailController : Controller
    {
        private readonly CompositionContainer   container;
        //Services
        private readonly IShellService shellService;
        private readonly IEntityService entityService;
        //ViewModels
        private DetailViewModel detailViewModel;
        //Commands
        private readonly DelegateCommand doTestCommand;
        //Further vars
        //private SynchronizingCollection<BookDataModel, Book> bookDataModels;

        [ImportingConstructor]
        public DetailController(CompositionContainer container, IShellService shellService, IEntityService entityService,
            DetailViewModel detailViewModel)
        {
            this.container          = container;
            //Services
            this.shellService       = shellService;
            this.entityService      = entityService;
            //ViewModels
            this.detailViewModel    = detailViewModel;
            //Commands
            this.doTestCommand      = new DelegateCommand(SelectDetail, CanSelectDetail);
        }

        public void Initialize()
        {
            AddWeakEventListener(detailViewModel, DetailViewModelPropertyChanged);

            IDetailView detailView = container.GetExportedValue<IDetailView>();
            detailViewModel = new DetailViewModel(detailView);

            detailViewModel.DoTestCommand = doTestCommand;
            shellService.DetailView = detailViewModel.View;
        }

        private void UpdateCommands()
        {

        }

        private bool CanSelectDetail() { return detailViewModel.IsValid; }

        private void SelectDetail()
        {
        }

        private void DetailViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSong")//SelectedSong is just an example
            {
                //...
                UpdateCommands();
            }
        }
    }
}

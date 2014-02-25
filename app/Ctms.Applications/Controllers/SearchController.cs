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
//using System.IO;
using System.Data.EntityClient;
using System.Data.Common;
using System.ComponentModel.Composition.Hosting;
using Ctms.Applications.Views;
using MusicSearch.Managers;
using Ctms.Applications.DataFactories;
using MusicSearch.ResponseObjects;
using Ctms.Domain.Objects;
using Ctms.Applications.Workers;
using MusicSearch.SearchObjects;
using System.Collections.Generic;


namespace Ctms.Applications.Controllers
{
    //!!Note: The content of this class is just an example and has to be adjusted.

    /// <summary>
    /// Responsible for the search management.
    /// </summary>
    [Export]
    internal class SearchController : Controller
    {
        private readonly CompositionContainer _container;
        //Services
        private readonly IShellService _shellService;
        private readonly IEntityService _entityService;
        private readonly SearchTagVisualizationService _tagVisualizationService;
        //ViewModels
        private SearchViewModel _searchVm;
        private SearchTagViewModel _searchTagVm;
        private ResultViewModel _resultVm;
        //Workers
        private SearchWorker _searchWorker;
        private SearchOptionWorker _searchOptionWorker;
        private ResultWorker _resultWorker;
        //Commands
        private readonly DelegateCommand _startSearchCmd;
        private readonly DelegateCommand _selectOptionCmd;
        //Further vars

        [ImportingConstructor]
        public SearchController(CompositionContainer container, IShellService shellService, IEntityService entityService,
            SearchViewModel searchVm, 
            SearchTagViewModel searchTagVm, 
            ResultViewModel resultVm,
            SearchWorker searchWorker, ResultWorker resultWorker, SearchOptionWorker searchOptionWorker)
        {
            _searchWorker = searchWorker;
            _resultWorker = resultWorker;
            _searchOptionWorker = searchOptionWorker;

            _container = container;
            //Services
            _shellService = shellService;
            _entityService = entityService;
            _tagVisualizationService = new SearchTagVisualizationService(searchVm);//, searchTagVm);
            //ViewModels
            _searchVm = searchVm;
            _searchTagVm = searchTagVm;
            _resultVm = resultVm;
            //Commands
            _startSearchCmd = new DelegateCommand(_searchWorker.StartSearch, _searchWorker.CanStartSearch);
            //_selectOptionCmd = new DelegateCommand(t => _searchOptionWorker.SelectOption((string)t));
            _selectOptionCmd = new DelegateCommand((id) => _searchOptionWorker.SelectOption((int)id));
        }

        public void Initialize()
        {
            //Commands
            _searchVm.StartSearchCmd = _startSearchCmd;
            //Views
            _shellService.SearchView = _searchVm.View;
            _shellService.SearchTagView = _searchTagVm.View;
            //Listeners
            AddWeakEventListener(_searchVm, SearchViewModelPropertyChanged);

            _searchTagVm.SelectOptionCmd = _selectOptionCmd;
            //_searchVm.SelectOptionCmd = _selectOptionCmd;

            InitiateTags();

            _tagVisualizationService.InitTagDefinitions();
        }

        private void InitiateTags()
        {
            List<Tag> tags = new List<Tag>()
            {
                new Tag()
                {
                    Id = 0,
                    SearchOptions = new List<SearchOption>()
                    {
                        new SearchOption()
                        {
                            Id = 1,
                            Header = "Fireworks",
                            SubHeader = "Katy Perry"
                        },
                        new SearchOption()
                        {
                            Id = 2,
                            Header = "Fireworker",
                            SubHeader = "ACDC"
                        }
                    }
                },
                new Tag()
                {
                    Id = 1,
                    SearchOptions = new List<SearchOption>()
                    {
                        new SearchOption()
                        {
                            Id = 3,
                            Header = "The Baseballs"
                        },
                        new SearchOption()
                        {
                            Id = 4,
                            Header = "Baseball Fighters"
                        }
                    }
                }
            };

            _searchVm.Tags = tags;
        }

        private void UpdateCommands()
        {

        }        

        private void SearchViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "InputValue")//SelectedSong is just an example
            {
                //...
                UpdateCommands();
            }
        }
    }
}

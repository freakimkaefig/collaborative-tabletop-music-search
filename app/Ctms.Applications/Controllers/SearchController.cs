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
using System.Data.EntityClient;
using System.Data.Common;
using System.ComponentModel.Composition.Hosting;
using Ctms.Applications.Views;
using MusicSearch.Managers;
using Ctms.Applications.DataFactories;
using MusicSearch.ResponseObjects;
using Ctms.Domain.Objects;
using Ctms.Applications.Workers;
using System.Collections.Generic;
using Ctms.Applications.DataModels;
using Ctms.Applications.Data;
using System.Collections.Specialized;


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
        private readonly EntityService _entityService;
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
        private readonly DelegateCommand _getSuggestionsCmd;
        //Further vars
        private SearchManager _searchManager;
        private DelegateCommand _selectCircleOptionCmd;
        private IMessageService _messageService;
        private Repository _repository;
        private SynchronizingCollection<TagDataModel, Tag> _tagDataModels;

        [ImportingConstructor]
        public SearchController(CompositionContainer container, IShellService shellService, EntityService entityService,
            IMessageService messageService,
            SearchViewModel searchVm, 
            SearchTagViewModel searchTagVm, 
            ResultViewModel resultVm,
            SearchWorker searchWorker, ResultWorker resultWorker, SearchOptionWorker searchOptionWorker,
            Repository repository)
        {
            _container                  = container;
            _repository                 = repository;
            //Workers
            _searchWorker               = searchWorker;
            _resultWorker               = resultWorker;
            _searchOptionWorker         = searchOptionWorker;
            //Services
            _shellService               = shellService;
            _entityService              = entityService;
            _tagVisualizationService    = new SearchTagVisualizationService(searchVm, _repository);//, searchTagVm);
            _messageService             = messageService;
            //ViewModels
            _searchVm                   = searchVm;
            _searchTagVm                = searchTagVm;
            _resultVm                   = resultVm;
            //Commands
            _startSearchCmd             = new DelegateCommand(_searchWorker.StartSearch, _searchWorker.CanStartSearch);
            _selectOptionCmd            = new DelegateCommand((tagId) => _searchOptionWorker.SelectOption((int)tagId));
            //_selectCircleOptionCmd = new DelegateCommand((id) => _searchOptionWorker.SelectCircleOption((int)id));
            _selectCircleOptionCmd      = new DelegateCommand((tagId) => SelectCircleOption((int)tagId));
            
            _getSuggestionsCmd          = new DelegateCommand((tagId) => _searchOptionWorker.LoadSuggestions((int)tagId));
            //Further vars
            _searchManager              = new SearchManager();
        }

        public void Initialize()
        {
            _repository.Initialize(_searchManager);

            //Commands
            _searchVm.StartSearchCmd = _startSearchCmd;
            _searchVm.SelectCircleOptionCmd = _selectCircleOptionCmd;
            //Views
            _shellService.SearchView = _searchVm.View;
            _shellService.SearchTagView = _searchTagVm.View;
            //Listeners
            AddWeakEventListener(_searchVm, SearchViewModelPropertyChanged);

            AddWeakEventListener((INotifyCollectionChanged)_searchVm.Tags, TagsChanged);
            /*
            foreach(var tag in _searchVm.Tags)
            {
                AddWeakEventListener((INotifyCollectionChanged)tag.Tag.TagOptions, TagsChanged);
            }*/

            //_searchTagVm.SelectOptionCmd = _selectOptionCmd;
            _searchVm.SelectOptionCmd   = _selectOptionCmd;
            _searchVm.GetSuggestionsCmd = _getSuggestionsCmd;

            InitKeywords();

            // set  default tag values
            _tagVisualizationService.InitTagDefinitions();

            _searchVm.Tags = _repository.GetAllTagDMs();

            _searchOptionWorker.Initialize(_searchManager);
            foreach (var tag in _searchVm.Tags)
            {
                _searchOptionWorker.UpdateOptions(tag.Tag.Id);
            }
        }

        private void TagsChanged(object sender, NotifyCollectionChangedEventArgs e) 
        { 
            //UpdateItemCount(); 
        
        }


        private void SelectCircleOption(int id)
        {
            MessageServiceExtensions.ShowMessage(_messageService, "Selected Circle option: " + id);
        }

        private void InitKeywords()
        {
            List<Tag> tags = new List<Tag>()
            {
                /*
                new Tag()
                {
                    Id = 0,
                    TagOptions = new List<TagOption>()
                    {
                        new DoubleTextTagOption()
                        {
                            Id          = 1,
                            MainText    = "Fireworks",
                            SubText     = "Katy Perry"
                        },
                        new DoubleTextTagOption()
                        {
                            Id          = 2,
                            MainText    = "Fireworker",
                            SubText     = "ACDC"
                        }
                    },
                    SelectedKeyword = new Artist("Korn")
                },
                new Tag()
                {
                    Id = 1,
                    TagOptions = new List<TagOption>()
                    {
                        new SingleTextTagOption()
                        {
                            Id = 3,
                            Text = "The Baseballs"
                        },
                        new SingleTextTagOption()
                        {
                            Id = 4,
                            Text = "Baseball Fighters"
                        }
                    }
                }*/
            };
            //_searchVm.Tags = tags;
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
            else if (e.PropertyName == "Tags")//SelectedSong is just an example
            {

            }
            else if (e.PropertyName == "AddedVisualization" && _searchVm.AddedVisualization == true)//SelectedSong is just an example
            {
                //_searchVm.AddedVisualization = false;
                //_searchOptionWorker.UpdateOptions();
            }
        }
    }
}

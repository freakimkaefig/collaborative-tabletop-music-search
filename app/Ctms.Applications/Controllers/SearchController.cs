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
        private readonly DelegateCommand _goBreadcrumbCmd;
        private readonly DelegateCommand _getSuggestionsCmd;
        private readonly DelegateCommand _editCmd;
        private readonly DelegateCommand _goHomeCmd;
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
            _selectOptionCmd            = new DelegateCommand((tagOptionId) => _searchOptionWorker.SelectOption((int)tagOptionId));
            _goBreadcrumbCmd            = new DelegateCommand((tagOptionId) => _searchOptionWorker.GoToBreadcrumb((int)tagOptionId));
            _getSuggestionsCmd          = new DelegateCommand((tagOptionId) => _searchOptionWorker.LoadSuggestions((int)tagOptionId));
            _editCmd                    = new DelegateCommand((tagId) => _searchOptionWorker.EditTag((int)tagId));
            _goHomeCmd  = new DelegateCommand((tagId) => _searchOptionWorker.GoHome((int)tagId));
            //Further vars
            _searchManager              = new SearchManager();
        }

        public void Initialize()
        {
            _repository.Initialize(_searchManager);

            //Commands
            _searchVm.StartSearchCmd = _startSearchCmd;
            //Views
            _shellService.SearchView = _searchVm.View;
            _shellService.SearchTagView = _searchTagVm.View;
            //Listeners
            AddWeakEventListener(_searchVm, SearchViewModelPropertyChanged);

            //AddWeakEventListener((INotifyCollectionChanged)_searchVm.Tags, TagsChanged);
            /*
            foreach(var tag in _searchVm.Tags)
            {
                AddWeakEventListener((INotifyCollectionChanged)tag.Tag.TagOptions, TagsChanged);
            }*/

            _searchVm.SelectOptionCmd   = _selectOptionCmd;
            _searchVm.GetSuggestionsCmd = _getSuggestionsCmd;
            _searchVm.GoBreadcrumbCmd   = _goBreadcrumbCmd;
            _searchVm.EditCmd           = _editCmd;
            _searchVm.GoHomeCmd         = _goHomeCmd;

            // set  default tag values
            _tagVisualizationService.InitTagDefinitions();

            _searchVm.Tags = _repository.GetAllTagDMs();

            _searchWorker.Initialize(_searchManager);
            _searchOptionWorker.Initialize(_searchManager, _searchVm.Tags);
        }
        

        private void UpdateCommands()
        {

        }        

        private void SearchViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Tags")//SelectedSong is just an example
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

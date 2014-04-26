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
using MusicSearch.Objects;
using Ctms.Domain.Objects;
using Ctms.Applications.Workers;
using System.Collections.Generic;
using Ctms.Applications.DataModels;
using Ctms.Applications.Data;
using System.Collections.Specialized;
using Ctms.Applications.Common;
using System.Collections.ObjectModel;


namespace Ctms.Applications.Controllers
{
    //!!Note: The content of this class is just an example and has to be adjusted.

    /// <summary>
    /// Responsible for the search management.
    /// </summary>
    [Export]
    internal class SearchController : Controller
    {
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
        private FftWorker _fftWorker;
        private TagCombinationWorker _tagCombinationWorker;
        //Commands
        private readonly DelegateCommand _startSearchCmd;
        private readonly DelegateCommand _selectOptionCmd;
        private readonly DelegateCommand _selectAttributeCmd;
        private readonly DelegateCommand _selectGenreCmd;
        private readonly DelegateCommand _selectArtistCmd;
        private readonly DelegateCommand _selectTitleCmd;
        private readonly DelegateCommand _goBreadcrumbCmd;
        private readonly DelegateCommand _confirmBreadcrumbCmd;
        private readonly DelegateCommand _confirmInputCmd;
        private readonly DelegateCommand _lowerInputCmd;
        private readonly DelegateCommand _raiseInputCmd;
        private readonly DelegateCommand _editCmd;
        private readonly DelegateCommand _goHomeCmd;
        //Further vars
        private SearchManager _searchManager;
        private DelegateCommand _selectCircleOptionCmd;
        private IMessageService _messageService;
        private Repository _repository;
        private DelegateCommand _checkTagPositionsCmd;
        

        [ImportingConstructor]
        public SearchController(
            IShellService shellService,
            SearchViewModel searchVm, 
            SearchTagViewModel searchTagVm, 
            ResultViewModel resultVm,
            SearchWorker searchWorker, 
            ResultWorker resultWorker, 
            SearchOptionWorker searchOptionWorker,
            Repository repository,
            FftWorker fftWorker,
            TagCombinationWorker tagCombinationWorker
            )
        {
            _repository                 = repository;
            //Workers
            _searchWorker               = searchWorker;
            _resultWorker               = resultWorker;
            _searchOptionWorker         = searchOptionWorker;
            _fftWorker                  = fftWorker;
            _tagCombinationWorker       = tagCombinationWorker;
            //Services
            _shellService               = shellService;
            _tagVisualizationService    = new SearchTagVisualizationService(searchVm, _repository);
            //ViewModels
            _searchVm                   = searchVm;
            _searchTagVm                = searchTagVm;
            _resultVm                   = resultVm;
            //Commands
            _startSearchCmd             = new DelegateCommand(_searchWorker.StartSearch, _searchWorker.CanStartSearch);
            _selectOptionCmd            = new DelegateCommand((tagOptionId) => _searchOptionWorker.SelectOption((int)tagOptionId));
            
            _selectAttributeCmd         = new DelegateCommand(
                (tagOptionId) => _searchOptionWorker.SelectKeywordType((int)tagOptionId, KeywordTypes.Attribute));
            _selectGenreCmd             = new DelegateCommand(
                (tagOptionId) => _searchOptionWorker.SelectKeywordType((int)tagOptionId, KeywordTypes.Genre));
            _selectArtistCmd            = new DelegateCommand(
                (tagOptionId) => _searchOptionWorker.SelectKeywordType((int)tagOptionId, KeywordTypes.Artist));
            _selectTitleCmd             = new DelegateCommand(
                (tagOptionId) => _searchOptionWorker.SelectKeywordType((int)tagOptionId, KeywordTypes.Title));

            _goBreadcrumbCmd            = new DelegateCommand((tagOptionId) => _searchOptionWorker.GoBreadcrumb((int)tagOptionId)); new DelegateCommand((tagOptionId) => _searchOptionWorker.GoBreadcrumb((int)tagOptionId));
            _checkTagPositionsCmd       = new DelegateCommand((tagId) => _tagCombinationWorker.CheckMovedTagCombi((int)tagId));
            _confirmInputCmd = new DelegateCommand((tagId) => _searchOptionWorker.ConfirmInput((int)tagId));
            _lowerInputCmd = new DelegateCommand((tagId) => _searchOptionWorker.ConfirmInput((int)tagId));
            _raiseInputCmd = new DelegateCommand((tagOptionId) => _searchOptionWorker.ConfirmInput((int)tagOptionId));
            
            _editCmd                    = new DelegateCommand((tagId) => _searchOptionWorker.EditTag((int)tagId));
            _goHomeCmd                  = new DelegateCommand((tagId) => _searchOptionWorker.GoHome((int)tagId));
            //Further vars
            _searchManager              = new SearchManager();
        }

        public void Initialize()
        {
            _repository.Initialize(_searchManager);

            // views
            _shellService.SearchView = _searchVm.View;
            _shellService.SearchTagView = _searchTagVm.View;

            // assign commands
            _searchVm.StartSearchCmd    = _startSearchCmd;
            _searchVm.SelectOptionCmd   = _selectOptionCmd;
            _searchVm.SelectAttributeCmd= _selectAttributeCmd;
            _searchVm.SelectGenreCmd    = _selectGenreCmd;
            _searchVm.SelectArtistCmd   = _selectArtistCmd;
            _searchVm.SelectTitleCmd    = _selectTitleCmd;
            _searchVm.CheckTagPositionsCmd = _checkTagPositionsCmd;
            _searchVm.ConfirmInputCmd = _confirmInputCmd;
            _searchVm.GoBreadcrumbCmd = _goBreadcrumbCmd;
            //_searchVm.ConfirmBreadcrumbCmd = _confirmBreadcrumbCmd;
            _searchVm.EditCmd           = _editCmd;
            _searchVm.GoHomeCmd         = _goHomeCmd;

            // init tag definitions
            _tagVisualizationService.InitTagDefinitions();

            // init workers
            _searchWorker.Initialize(_searchManager);
            _searchOptionWorker.Initialize(_searchManager, _searchVm.Tags);
            _tagCombinationWorker.Initialize();

            // listeners
            AddWeakEventListener(_searchVm, SearchViewModelPropertyChanged);
            foreach (var tag in _searchVm.Tags)
            {   // add listener to each tag
                AddWeakEventListener(tag, TagDMChanged);
            }
            /*
            _searchVm.TagCombinations.Add(new TagCombinationDataModel(20)
            {
                CenterX = 200,
                CenterY = 200,
                Tags = new ObservableCollection<TagDataModel>()
                {
                    new TagDataModel()
                    {
                        Tag = new Tag()
                        {
                            PositionX = 150,
                            PositionY = 150
                        }
                    }
                }
            });*/
            
            _searchVm.TagCombinations = new ObservableCollection<TagCombinationDataModel>()
            {
                new TagCombinationDataModel(20)
                {
                    CenterX = 200,
                    CenterY = 200,
                    Tags = new ObservableCollection<TagDataModel>()
                    {
                        new TagDataModel()
                        {
                            Tag = new Tag()
                            {
                                PositionX = 150,
                                PositionY = 150
                            }
                        }
                    }
                }
            };
        }

        private void UpdateCommands()
        {

        }

        /// <summary>
        /// Called when a property of TagDataModel has changed
        /// </summary>
        /// <param name="sender">Object which contains the changed property</param>
        /// <param name="e">PropertyChangedEventArgs</param>
        private void TagDMChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VisibleOptions")
            {   // when visible options changed the tag visualization has to be updated
                _searchVm.UpdateVisuals((TagDataModel)sender);
            }
            
        }

        /// <summary>
        /// Called when a property of SearchViewModel has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
    }
}

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
using MusicSearch.Managers;
using Ctms.Applications.DataFactories;
using MusicSearch.ResponseObjects;
using Ctms.Domain.Objects;


namespace Ctms.Applications.Controllers
{
    //!!Note: The content of this class is just an example and has to be adjusted.

    /// <summary>
    /// Responsible for the search management.
    /// </summary>
    [Export]
    internal class SearchController : Controller
    {
        private readonly CompositionContainer container;
        //Services
        private readonly IShellService shellService;
        private readonly IEntityService entityService;
        //ViewModels
        private SearchViewModel searchViewModel;
        private ResultViewModel resultViewModel;
        //Commands
        private readonly DelegateCommand startSearchCmd;
        private readonly DelegateCommand setSelectionCmd;
        //Further vars
        //private SynchronizingCollection<BookDataModel, Book> bookDataModels;

        [ImportingConstructor]
        public SearchController(CompositionContainer container, IShellService shellService, IEntityService entityService,
            SearchViewModel searchViewModel, ResultViewModel resultViewModel)
        {
            this.container = container;
            //Services
            this.shellService = shellService;
            this.entityService = entityService;
            //ViewModels
            this.searchViewModel = searchViewModel;
            this.resultViewModel = resultViewModel;
            //Commands
            this.startSearchCmd = new DelegateCommand(StartSearch, CanStartSearch);
            this.setSelectionCmd = new DelegateCommand(StartSearch, CanStartSearch);
        }

        public void Initialize()
        {
            AddWeakEventListener(searchViewModel, SearchViewModelPropertyChanged);

            searchViewModel.StartSearchCommand = startSearchCmd;
            AddWeakEventListener(searchViewModel, SearchViewModelPropertyChanged);

            shellService.SearchView = searchViewModel.View;
        }

        private bool CanStartSearch() { return searchViewModel.IsValid; }

        private void StartSearch()
        {
            var searchController = new SearchManager();
            searchController.StartSearch();
            RefreshResults(searchController.ResponseContainer);
        }

        private void RefreshResults(ResponseContainer responseContainer)
        {
            //Example of how to read a resulting song and assign it to viewmodel
            SongFactory factory = new SongFactory();
            var result = new Result();
            Random rnd = new Random();
            int index = rnd.Next(0, responseContainer.response.songs.Count);
            result.Song = factory.Create(responseContainer.response.songs[index]);
            resultViewModel.Result = result;
        }

        private void UpdateCommands()
        {

        }        

        private void SearchViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSong")//SelectedSong is just an example
            {
                //...
                UpdateCommands();
            }
        }
    }
}

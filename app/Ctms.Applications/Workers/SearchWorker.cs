using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers;
using MusicSearch.Managers;
using System.ComponentModel;
using MusicSearch.SearchObjects;
using Ctms.Applications.ViewModels;
using System.ComponentModel.Composition;

namespace Ctms.Applications.Workers
{
    [Export]
    public class SearchWorker
    {
        private BackgroundWorkHelper _backgroundWorker;
        private SearchManager _searchManager;
        private ResultWorker _resultWorker;
        private SearchViewModel _searchViewModel;

        [ImportingConstructor]
        public SearchWorker(SearchViewModel searchViewModel, ResultWorker resultWorker)
        {
            //ViewModels
            _searchViewModel = searchViewModel;
            //Workers
            _resultWorker = resultWorker;
            //Managers
            _searchManager = new SearchManager();
        }

        public bool CanStartSearch() { return _searchViewModel.IsValid; }

        public void StartSearch()
        {
            var searchManager = new SearchManager();
            searchManager.StartSearch();
            _resultWorker.RefreshResults(searchManager.ResponseContainer);

            //RefreshResults(searchManager.ResponseContainer);

            _backgroundWorker = new BackgroundWorkHelper();
            //Tell which method to execute in background in what to do after completion
            _backgroundWorker.DoInBackground(searchManager.Start, SearchCompleted);
        }

        private void SearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}

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
using MusicSearch.ResponseObjects;

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
            //Helpers
            _backgroundWorker = new BackgroundWorkHelper();
        }

        public bool CanStartSearch() { return _searchViewModel.IsValid; }

        public void StartSearch()
        {
            //Starte Testsuche mit Testliste
            _backgroundWorker.DoInBackground(StartSearchWorker, StartSearchCompleted);
        }

        private void StartSearchWorker(object sender, DoWorkEventArgs e)
        {
            e.Result = _searchManager.SearchQuery(_searchViewModel.SearchObjectsList);
        }
        private void StartSearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _resultWorker.RefreshResults((List<ResponseContainer.ResponseObj.Song>)e.Result);
        }
    }
}

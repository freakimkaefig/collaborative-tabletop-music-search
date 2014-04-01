using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers;
using MusicSearch.Managers;
using System.ComponentModel;
using Ctms.Applications.ViewModels;
using System.ComponentModel.Composition;
using MusicSearch.ResponseObjects;
using Ctms.Domain;

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
        }

            //Helpers
            _backgroundWorker = new BackgroundWorkHelper();
        public void Initialize(SearchManager searchManager)
        {
            _searchManager = searchManager;
        }

        public bool CanStartSearch() { return _searchViewModel.IsValid; }

        public void StartSearch()
        {
            var tags = _searchViewModel.Tags;
            var searchObjects = new List<searchObjects>();
            var usedTags =  tags.Where(t => t.Tag.AssignedKeyword != null && !String.IsNullOrEmpty(t.Tag.AssignedKeyword.SearchId));
            foreach (var tag in usedTags)
            {
                var searchObject = new searchObjects();
                searchObject.originId = tag.Id;

                var keyword = tag.Tag.AssignedKeyword;

                if (keyword.Type == KeywordTypes.Artist)
                {
                    searchObject.artist_id = keyword.SearchId;
                }
                else if (keyword.Type == KeywordTypes.Title)
                {
                    searchObject.title_id = keyword.SearchId;
                }
                else if (keyword.Type == KeywordTypes.Genre)
                {
                    searchObject.genre = keyword.SearchId;
                }

                searchObjects.Add(searchObject);
            }

            var songs = _searchManager.SearchQuery(searchObjects);
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

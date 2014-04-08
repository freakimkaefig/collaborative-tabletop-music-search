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
        private InfoWorker _infoWorker;
        private SearchViewModel _searchViewModel;

        [ImportingConstructor]
        public SearchWorker(SearchViewModel searchViewModel, ResultWorker resultWorker, InfoWorker infoWorker)
        {
            //ViewModels
            _searchViewModel = searchViewModel;
            //Workers
            _resultWorker = resultWorker;
            _infoWorker = infoWorker;
            //Helpers
            _backgroundWorker = new BackgroundWorkHelper();
        }

            
        public void Initialize(SearchManager searchManager)
        {
            _searchManager = searchManager;
        }

        public bool CanStartSearch() { return _searchViewModel.IsValid; }

        public void StartSearch()
        {
            var loadingInfoId = _infoWorker.ShowCommonInfo("Loading results...", "Please wait a moment");

            var backgrWorker = new BackgroundWorkHelper();
            backgrWorker.DoInBackground(StartSearch, StartSearchCompleted, loadingInfoId);
        }

        public void StartSearch(object sender, DoWorkEventArgs e)
        {
            var tags = _searchViewModel.Tags;
            var searchObjects = new List<searchObjects>();
            var usedTags = tags.Where(t => t.Tag.AssignedKeyword != null /*&& !String.IsNullOrEmpty(t.Tag.AssignedKeyword.SearchId)*/);
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
                    searchObject.genre = keyword.Name;
                }

                searchObjects.Add(searchObject);
            }

            //var songs = _searchManager.SearchQuery(searchObjects);
            var songs = _searchManager.SearchQuery(_searchViewModel.SearchObjectsList); //TestTag mit "Rock"
            var infoId = (int) e.Argument;
            e.Result = new List<object>() { songs, infoId };
        }

        private void StartSearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                _infoWorker.ShowCommonInfo("Cancelled search by user", "", "Ok");
            }
            else if (e.Error != null)
            {
                _infoWorker.ShowCommonInfo("Search has failed", e.Error.Message, "Ok");
            }
            else
            {
                var resultSongs     = (List<ResponseContainer.ResponseObj.Song>)(((List<object>)e.Result)[0]);
                var loadingInfoId   = (int)((List<object>)e.Result)[1];

                _resultWorker.RefreshResults(resultSongs);
                _infoWorker.ConfirmCommonInfo(loadingInfoId);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers;
using MusicSearch.Managers;
using System.ComponentModel;
using Ctms.Applications.ViewModels;
using System.ComponentModel.Composition;
using MusicSearch.Objects;
using Ctms.Domain;
using Ctms.Applications.DataModels;
using Ctms.Applications.Data;

namespace Ctms.Applications.Workers
{
    [Export]
    public class SearchWorker
    {
        private Repository _repository;
        private BackgroundWorkHelper _backgroundWorker;
        private SearchManager _searchManager;
        private ResultWorker _resultWorker;
        private InfoWorker _infoWorker;
        private SearchViewModel _searchViewModel;

        [ImportingConstructor]
        public SearchWorker(Repository repository, SearchViewModel searchViewModel, ResultWorker resultWorker, InfoWorker infoWorker)
        {
            _repository = repository;
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

        public void LoadDetails(ResultDataModel result)
        {
            _backgroundWorker.DoInBackground(LoadDetailsWorker, LoadDetailsCompleted, result);
        }

        //Background worker methods
        public void StartSearch(object sender, DoWorkEventArgs e)
        {
            var combinedResults = DoCombinedSearch();

            //!! todo: get and parse combined results and create results
            var uncombinedSongs = DoUncombinedSearch();

            //!! todo: add uncombinedSongs
            var allSongs = uncombinedSongs;

            var infoId = (int)e.Argument;
            e.Result = new List<object>() { allSongs, infoId };
        }

        private List<ResponseContainer.ResponseObj.combinedQuery> DoCombinedSearch()
        {
            var combinedSearchObjects = new List<combinedSearchObject>();

            // do search for uncombined tags
            foreach (var tag in _repository.GetUncombinedTags())
            {
                //!!
            }

            var songs = _searchManager.combinedSearchQuery(combinedSearchObjects);

            return songs;
        }

        private List<ResponseContainer.ResponseObj.Song> DoUncombinedSearch()
        {
            var searchObjects = new List<searchObject>();

            //!!used tags außer combined tags! sonst doppelte anfrage
            var usedTags = _repository.GetAssignedTagDMs();

            foreach (var tag in usedTags)
            {
                var searchObject = new searchObject();
                searchObject.originId = tag.Id;

                var keyword = tag.Tag.AssignedKeyword;

                if (keyword.KeywordType == KeywordTypes.Artist)
                {
                    searchObject.artist_id = keyword.Key;
                }
                else if (keyword.KeywordType == KeywordTypes.Title)
                {
                    searchObject.title_id = keyword.Key;
                }
                else if (keyword.KeywordType == KeywordTypes.Genre)
                {
                    searchObject.genre = keyword.DisplayName;
                }
                else if (keyword.KeywordType == KeywordTypes.Attribute)
                {
                    //searchObject.
                }

                searchObjects.Add(searchObject);
            }

            var songs = _searchManager.SearchQuery(searchObjects);

            return songs;
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

        private void LoadDetailsWorker(object sender, DoWorkEventArgs e)
        {
            ResultDataModel result = (ResultDataModel)e.Argument;
            String artistName = result.Result.Song.ArtistName;
            String artistId = result.Result.Song.ArtistId;
            e.Result = new List<object>() { _searchManager.getDetailInfo(artistName, artistId), result };
        }
        private void LoadDetailsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                _infoWorker.ShowCommonInfo("Cancelled lookup for by user", "", "Ok");
            }
            if (e.Error != null)
            {
                _infoWorker.ShowCommonInfo("Lookup for detail has failed", e.Error.Message, "Ok");
            }
            else
            {
                List<ResponseContainer.ResponseObj.ArtistInfo> resultDetails = (List<ResponseContainer.ResponseObj.ArtistInfo>)(((List<object>)e.Result)[0]);
                ResultDataModel result = (ResultDataModel)(((List<object>)e.Result)[1]);
                _resultWorker.RefreshDetails(resultDetails, result);
            }
        }
    }
}

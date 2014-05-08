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
using Ctms.Domain.Objects;

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
            var loadingInfoId = _infoWorker.ShowCommonInfo("Loading results...", "Please wait a moment", "Ok", "Cancel", true);

            var backgrWorker = new BackgroundWorkHelper();
            backgrWorker.DoInBackground(StartSearch, StartSearchCompleted, loadingInfoId);
        }

        public void LoadDetails(ResultDataModel result)
        {
            if (!result.IsDetailLoaded)
            {
                _backgroundWorker.DoInBackground(LoadDetailsWorker, LoadDetailsCompleted, result);
            }
        }

        //Background worker methods
        public void StartSearch(object sender, DoWorkEventArgs e)
        {
            var combinedResults = DoCombinedSearch(e);

            //!! todo: get and parse combined results and create results
            var uncombinedSongs = DoUncombinedSearch(e);

            //!! todo: add combinedSong results (tracks)
            var allSongs = uncombinedSongs;

            /*for (var i = 0; i < allSongs.Count; i++) {
                DevHelper.DevLogger.Log("SearchWorker:74 - " + allSongs[i].ToString());
            }*/

            var infoId = (int)e.Argument;
            e.Result = new List<object>() { allSongs, infoId };
        }

        /// <summary>
        /// Do search for tag combinations
        /// </summary>
        /// <returns></returns>
        private List<ResponseContainer.ResponseObj.combinedQuery> DoCombinedSearch(DoWorkEventArgs e)
        {
            var combinedSearchObjects = new List<combinedSearchObject>();

            foreach(var combi in _repository.GetTagCombinations())
            {
                var combinedSearchObject = new combinedSearchObject();

                // collect tagIds to state the origin of combined search query
                combinedSearchObject.originIds = combi.Tags.Select(t => t.Tag.Id).ToList();

                // get combi and copy relevant values to search object
                ParseCombi(combi, combinedSearchObject, combi.CombinationType);

                // add to list
                combinedSearchObjects.Add(combinedSearchObject);
            }

            var songs = _searchManager.combinedSearchQuery(combinedSearchObjects);

            return songs;
        }


        /// <summary>
        /// Read values of combi and set concerning properties of combinedSearchObject, depending form combiType
        /// </summary>
        /// <param name="combi">The combination to read data from</param>
        /// <param name="combinedSearchObject">The object whose data has to be set</param>
        /// <param name="combiType">The type of combination</param>
        private void ParseCombi(TagCombinationDataModel combi, combinedSearchObject combinedSearchObject, CombinationTypes combiType)
        {
            // check type
            KeywordTypes keywordType;
            if(combiType == CombinationTypes.Genre)
                keywordType = KeywordTypes.Genre;
            else
                keywordType = KeywordTypes.Artist;

            if (keywordType == KeywordTypes.Genre)
            {
                var genreAttributes = new List<GenreParameter>();

                // get tags which are assigned with a keyword of type genre  (can only be up to five)
                var genreTags   = combi.Tags.Where(t => t.Tag.AssignedKeyword.KeywordType == KeywordTypes.Genre).ToList();

                // collect genres
                combinedSearchObject.genre = new String[genreTags.Count];                
                for (var i = 0; i < genreTags.Count; i++)
                {
                    combinedSearchObject.genre[i] = genreTags[i].Tag.AssignedKeyword.Key;
                }

                // collect genre attributes
                foreach (var attrTag in combi.Tags.Where(ct => ct.Tag.AssignedKeyword.KeywordType == KeywordTypes.Attribute))
                {
                    var genreParam = new GenreParameter();

                    SetAttributeProperty(attrTag, genreParam);

                    genreAttributes.Add(genreParam);
                }
                // copy genre parameters
                combinedSearchObject.GenreParameter = genreAttributes;                
            }
            else if (keywordType == KeywordTypes.Attribute)
            {
                var keywordAttributes = new List<ArtistParameter>();

                // get tag of type artist (can only be one)
                var artistTag = combi.Tags.FirstOrDefault(ct => ct.Tag.AssignedKeyword.KeywordType == KeywordTypes.Artist);

                // copy artist id
                if (artistTag != null) combinedSearchObject.artist_id = artistTag.Tag.AssignedKeyword.Key;

                // collect artist attributes
                foreach (var attributeTag in combi.Tags.Where(ct => ct.Tag.AssignedKeyword.KeywordType == KeywordTypes.Attribute))
                {
                    var artistParam = new ArtistParameter();

                    SetAttributeProperty(attributeTag, artistParam);

                    keywordAttributes.Add(artistParam);
                }
                combinedSearchObject.ArtistParameter = keywordAttributes;                
            }
        }

        /// <summary>
        /// Set value of a property of an artist or genre attribute
        /// </summary>
        /// <param name="attributeTag">The tag containing attribute's value</param>
        /// <param name="searchObjectParameter">The artistParameter or genreParameter</param>
        private void SetAttributeProperty(TagDataModel attributeTag, object searchObjectParameter)
        {
            // get property
            var genreProperties     = searchObjectParameter.GetType().GetProperties();
            var genreName           = attributeTag.Tag.AssignedKeyword.AttributeType.ToString();
            var concerningProperty  = genreProperties.FirstOrDefault(a => a.Name == genreName);

            // set value of property
            concerningProperty.SetValue(
                searchObjectParameter,
                Convert.ChangeType(attributeTag.Tag.AssignedKeyword.Value, concerningProperty.PropertyType),
                null);
        }

        private List<ResponseContainer.ResponseObj.Song> DoUncombinedSearch(DoWorkEventArgs e)
        {
            var searchObjects = new List<searchObject>();

            // get tags are not in combinations and are assigned and added (not removed or in edit mode)
            var tags = _repository.GetUncombinedTags()
                .Where(t => t.AssignState == TagDataModel.AssignStates.Assigned)
                .Where(t => t.ExistenceState == TagDataModel.ExistenceStates.Added);

            foreach (var tag in tags)
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

                searchObjects.Add(searchObject);
            }

            var songs = _searchManager.SearchQuery(searchObjects);

            return songs;
        }

        private void StartSearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var loadingInfoId   = (int)((List<object>)e.Result)[1];
            _infoWorker.ConfirmCommonInfo(loadingInfoId);

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

                _resultWorker.RefreshResults(resultSongs);
            }
        }

        private void LoadDetailsWorker(object sender, DoWorkEventArgs e)
        {
            ResultDataModel result = (ResultDataModel)e.Argument;
            result.IsDetailLoading = true;
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

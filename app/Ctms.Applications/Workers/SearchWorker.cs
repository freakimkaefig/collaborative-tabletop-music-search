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
using Newtonsoft.Json;
using System.IO;
using System.Globalization;

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
        private ResultViewModel _resultVm;

        [ImportingConstructor]
        public SearchWorker(Repository repository, SearchViewModel searchViewModel, ResultWorker resultWorker, InfoWorker infoWorker,
                 ResultViewModel resultVm)
        {
            _repository = repository;
            //ViewModels
            _searchViewModel = searchViewModel;
            _resultVm = resultVm;
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
            if(_repository.GetAddedAndAssignedTagDMs().Count() > 0)
            {
                var loadingInfoId = _infoWorker.ShowCommonInfo("Searching for songs...", "Please wait a moment", null, "Cancel", true, null, null);
                
                var backgrWorker = new BackgroundWorkHelper();
                backgrWorker.DoInBackground(StartSearchInBackground, StartSearchCompleted, loadingInfoId);
            }
            else if(_resultVm.Results != null && _resultVm.Results.Any())
            {
                _resultWorker.InitResults();
            }
            else
            {
                _infoWorker.ShowCommonInfo("No keywords defined", "Please place a tag on the table and select a keyword", "Ok");
            }
        }

        public void LoadDetails(ResultDataModel result)
        {
            if (!result.IsDetailLoaded)
            {
                _backgroundWorker.DoInBackground(LoadDetailsWorker, LoadDetailsCompleted, result);
            }
        }

        public void PrelistenTrackFromDetailView(String trackId)
        {
            _backgroundWorker.DoInBackground(PrelistenTrackFromDetailViewWorker, PrelistenTrackFromDetailViewCompleted, trackId);
        }

        //Background worker methods
        public void StartSearchInBackground(object sender, DoWorkEventArgs e)
        {
            var combinedSongs = DoCombinedSearch(e);

            // get and parse combined results and create results
            var uncombinedSongs = DoUncombinedSearch(e);

            // add combinedSong results (tracks)
            var allSongs = new List<ResponseContainer.ResponseObj.Song>();
            if(combinedSongs != null && combinedSongs.Any()) allSongs.AddRange(combinedSongs);
            if (uncombinedSongs != null && uncombinedSongs.Any()) allSongs.AddRange(uncombinedSongs);

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
        private List<ResponseContainer.ResponseObj.Song> DoCombinedSearch(DoWorkEventArgs e)
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
            else if (keywordType == KeywordTypes.Artist)
            {
                var artistAttributes = new List<ArtistParameter>();

                // get tag of type artist (can only be one)
                var artistTag = combi.Tags.FirstOrDefault(ct => ct.Tag.AssignedKeyword.KeywordType == KeywordTypes.Artist);

                // copy artist id
                if (artistTag != null) combinedSearchObject.artist_id = artistTag.Tag.AssignedKeyword.Key;

                // collect artist attributes
                foreach (var attrTag in combi.Tags.Where(ct => ct.Tag.AssignedKeyword.KeywordType == KeywordTypes.Attribute))
                {
                    var artistParam = new ArtistParameter();

                    SetAttributeProperty(attrTag, artistParam);

                    artistAttributes.Add(artistParam);
                }
                combinedSearchObject.ArtistParameter = artistAttributes;                
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
            var searchObjectProperties  = searchObjectParameter.GetType().GetProperties();
            var searchObjectName        = attributeTag.Tag.AssignedKeyword.Key.ToString();
            var concerningProperty      = searchObjectProperties.FirstOrDefault(a => a.Name == searchObjectName);

            var c = new CultureInfo("en-US");

            // set value of property
            concerningProperty.SetValue(
                searchObjectParameter,
                Convert.ChangeType(attributeTag.Tag.AssignedKeyword.Value, concerningProperty.PropertyType, c),
                null);
        }


        /// <summary>
        /// Do search for all tags that are not in a combination
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
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
                var loadingInfoId = (int)((List<object>)e.Result)[1];
                _infoWorker.ConfirmCommonInfo(loadingInfoId);

                var resultSongs = (List<ResponseContainer.ResponseObj.Song>)(((List<object>)e.Result)[0]);

                _resultWorker.RefreshResults(resultSongs);  

                //string json = JsonConvert.SerializeObject(resultSongs, Formatting.Indented);
                //File.WriteAllText("../../../Ctms.Applications/DevHelper/resultSongs.json", json);
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

        private void PrelistenTrackFromDetailViewWorker(object sender, DoWorkEventArgs e)
        {
            e.Result = _searchManager.getSpotifyId((String)e.Argument);
        }
        private void PrelistenTrackFromDetailViewCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _resultWorker.PrelistenFromDetailView((String)e.Result);
        }
    }
}

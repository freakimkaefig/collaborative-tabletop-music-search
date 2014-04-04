using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using MusicSearch.Managers;
using Ctms.Applications.ViewModels;
using System.Waf.Applications.Services;
using Ctms.Applications.Data;
using Ctms.Domain.Objects;
using Helpers;
using Ctms.Applications.Services;
using Ctms.Applications.DataModels;
using Ctms.Applications.DataFactories;
using Ctms.Applications.Views;
using Ctms.Domain;
using MusicSearch.ResponseObjects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace Ctms.Applications.Workers
{
    [Export]
    public class SearchOptionWorker
    {
        private SearchViewModel _searchVM;
        private SearchTagViewModel _searchTagVM;
        private IMessageService _messageService;
        private Repository _repository;
        private SearchManager _searchManager;
        private TagFactory _tagFactory;
        private InfoWorker _infoWorker;

        [ImportingConstructor]
        public SearchOptionWorker(SearchViewModel searchVM, SearchTagViewModel searchTagVm, IMessageService messageService,
            Repository repository, InfoWorker infoWorker)
        {
            //ViewModels
            _searchVM = searchVM;
            _searchTagVM = searchTagVm;
            //Services
            _messageService = messageService;
            //Data
            _repository = repository;
            //Workers

            //Other vars
            _infoWorker = infoWorker;
        }

        public void Initialize(SearchManager searchManager, ObservableCollection<TagDataModel> tagDMs)
        {
            _searchManager = searchManager;
            _tagFactory     = new TagFactory(_repository);

            foreach (var tagDM in tagDMs)
            {
                LoadKeywordTypes(tagDM.Id);
            }
        }

        public bool CanSelectOption() { return _searchVM.IsValid; }

        public void LoadKeywordTypes(int tagId)
        {
            var tagDM = _searchVM.Tags[tagId];

            tagDM.Tag.BreadcrumbOptions.Clear();

            UpdateActiveLayerNumber(tagDM, 0);

            var tagOpts = tagDM.Tag.TagOptions;
            tagOpts.Clear();

            var keywordTypes = Enum.GetValues(typeof(KeywordTypes));
            foreach (KeywordTypes keywordType in keywordTypes)
            {
                if (keywordType == KeywordTypes.None) continue;

                // create Keyword, e.g. Artist, Type or Genre
                var keyword     = _tagFactory.CreateKeyword(keywordType.ToString(), keywordType);

                // create TagOption for this keyword type at layer 0
                var tagOption = _tagFactory.CreateTagOption(keyword, tagDM.Tag.CurrentLayerNr);

                tagOpts.Add(tagOption);
            }
        }

        public void UpdateActiveLayerNumber(TagDataModel tagDM, int currentLayerNr)
        {
            tagDM.Tag.CurrentLayerNr = currentLayerNr;
            tagDM.RefreshLayer();
        }

        public void SelectOption(int tagOptionId)
        {
            var tagDM       = _repository.GetTagDMByTagOption(tagOptionId);
            var tagId       = tagDM.Id;
            var tagOption   = _repository.GetTagOptionById(tagOptionId);
            var keywordType = tagOption.Keyword.Type;

            // add breadcrumb only if the next stop is not assignKeyword
            if(tagOption.LayerNr != 2) AddBreadcrumb(tagDM, tagOption);

            UpdateActiveLayerNumber(tagDM, tagDM.Tag.CurrentLayerNr + 1);

            switch (tagDM.Tag.CurrentLayerNr)
            {
                case 1: // ---layer 1---
                {
                    if (keywordType == KeywordTypes.Artist || keywordType == KeywordTypes.Title)
                    {
                        // init selected keyword of tag and set its type
                        tagDM.Tag.AssignedKeyword = _tagFactory.CreateKeyword(tagOption.Keyword.Name, tagOption.Keyword.Type);

                        SetInputIsVisible(tagDM, true);
                    }
                    if (keywordType == KeywordTypes.Genre)
                    {   // load top genres
                        var genres = _searchManager.getGenres();

                        foreach (var genre in genres)
                        {
                            var keyword = _tagFactory.CreateKeyword(genre.genre_name, KeywordTypes.Genre);

                            var genreOption = _tagFactory.CreateTagOption(keyword, tagDM.Tag.CurrentLayerNr);

                            tagDM.Tag.TagOptions.Add(genreOption);
                        }
                    }
                    else if (keywordType == KeywordTypes.Attribute)
                    {   // load attributes

                    }

                    //_searchVM.UpdateVisuals(tag);

                    break;
                }
                case 2: // ---layer 2---
                {
                    // for artist and genre LoadSuggestions() is responsible for this layer

                    if (keywordType == KeywordTypes.Genre)
                    {   // load subgenres

                        var genre = _searchManager.getGenres().FirstOrDefault(g => g.genre_name == tagOption.Keyword.Name);

                        foreach (var subGenre in genre.Subgenres)
                        {
                            var keyword = _tagFactory.CreateKeyword(subGenre.name, KeywordTypes.Genre);

                            var genreOption = _tagFactory.CreateTagOption(keyword, tagDM.Tag.CurrentLayerNr);

                            tagDM.Tag.TagOptions.Add(genreOption);
                        }
                    }
                    else if (keywordType == KeywordTypes.Attribute)
                    {   // load attributes

                    }

                    break;
                }
                case 3: // ---layer 3---
                {
                    AssignKeyword(tagDM, tagOption);

                    tagDM.State = TagDataModel.States.Assigned;

                    break;
                }
            }

            // update menu
            _searchVM.UpdateVisuals(tagDM);
        }

        public void LoadSuggestions(int tagId)
        {
            var tagDM       = _repository.GetTagDMById(tagId);
            var terms       = _repository.GetTagDMById(tagId).InputTerms;
            var keywordType = tagDM.Tag.AssignedKeyword.Type;

            var termKeyword   = _tagFactory.CreateKeyword(terms, keywordType);
            var termTagOption = _tagFactory.CreateTagOption(termKeyword, tagDM.Tag.CurrentLayerNr);

            _repository.AddTagOption(tagDM, termTagOption);

            AddBreadcrumb(tagDM, termTagOption);

            UpdateActiveLayerNumber(tagDM, tagDM.Tag.CurrentLayerNr + 1);

            // get all options of this tag
            var tagOptions = _repository.GetTagOptionsByTagId(tagId);

            // remove previous options at this layer
            tagOptions.RemoveAll(to => to.LayerNr == tagDM.Tag.CurrentLayerNr);

            if (keywordType == KeywordTypes.Artist)
            {
                // get artist suggestions in background
                var backgrWorker = new BackgroundWorkHelper();
                backgrWorker.DoInBackground(GetArtistsSuggestionsInBackgr, GetArtistsSuggestionsCompleted, tagDM);
            }
            else if (keywordType == KeywordTypes.Title)
            {
                var suggestions = _searchManager.getTitleSuggestions(tagId, terms);

                foreach (var suggestion in suggestions)
                {
                    // create keyword out of this suggestion
                    var keyword = _tagFactory.CreateKeyword(suggestion.title, keywordType, suggestion.artist_name);
                    keyword.SearchId = suggestion.id;

                    // create option with this keyword
                    var tagOption = _tagFactory.CreateTagOption(keyword, tagDM.Tag.CurrentLayerNr);

                    tagOptions.Add(tagOption);
                    _repository.AddTagOption(tagDM, tagOption);
                }
            }
            SetInputIsVisible(tagDM, false);

            _searchVM.UpdateVisuals(tagDM);
        }

        public void GetArtistsSuggestionsInBackgr(object sender, DoWorkEventArgs e)
        {
            var tagDM = (TagDataModel) e.Argument;

            e.Result = _searchManager.getArtistSuggestions(tagDM.Id, tagDM.InputTerms);
        }

        private void GetArtistsSuggestionsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
            }
            else if (e.Error != null)
            {
            }
            else
            {
                var suggestions = (List<ResponseContainer.ResponseObj.ArtistSuggestion>) e.Result;

                TagDataModel tagDM = null;
                var firstSuggestion = suggestions.FirstOrDefault();
                if (suggestions != null && suggestions.Any())
                {
                    tagDM = _repository.GetTagDMById(suggestions.FirstOrDefault().originId);
                    if(String.IsNullOrEmpty(suggestions.FirstOrDefault().name))
                    {
                        _infoWorker.ShowTagInfo("No artists found", "Please adjust your terms", tagDM.Id);
                    }
                }

                for (var i = 0; i < suggestions.Count; i++)
                {
                    // create keyword out of this suggestion
                    var keyword = _tagFactory.CreateKeyword(suggestions[i].name, tagDM.Tag.AssignedKeyword.Type);
                    keyword.SearchId = suggestions[i].id;

                    // create option with this keyword
                    var tagOption = _tagFactory.CreateTagOption(keyword, tagDM.Tag.CurrentLayerNr);

                    _repository.AddTagOption(tagDM, tagOption);
                }

                SetInputIsVisible(tagDM, false);

                _searchVM.UpdateVisuals(tagDM);
            }
        }

        /// <summary>
        /// Add tagOption to breadcrumbOptions
        /// </summary>
        /// <param name="tagDM"></param>
        /// <param name="tagOption"></param>
        ///
        private void AddBreadcrumb(TagDataModel tagDM, TagOption tagOption)
        {
            tagDM.Tag.BreadcrumbOptions.Add(tagOption);
        }

        /// <summary>
        /// Remove previous breadcrumbOptions from higher or equal layer
        /// </summary>
        ///
        private void RemovePreviousBreadcrumbs(TagDataModel tag)
        {
            if (tag.Tag.BreadcrumbOptions != null)
            {
                tag.Tag.BreadcrumbOptions.RemoveAll(p => p.LayerNr >= tag.Tag.CurrentLayerNr);
            }
        }

        /// <summary>
        /// Remove previous breadcrumbOptions from higher or equal layer
        /// </summary>
        ///
        private void RemovePreviousOptions(TagDataModel tag)
        {
            if (tag.Tag.TagOptions != null)
            {
                tag.Tag.TagOptions.RemoveAll(p => p.LayerNr > tag.Tag.CurrentLayerNr);
            }
        }


        /// <summary>
        /// Go to option of breadcrumb and update items
        /// </summary>
        /// <param name="breadcrumbOptionId">Id of selected breadcrumb</param>
        public void GoBreadcrumb(int breadcrumbOptionId)
        {
            var tagDM               = _repository.GetTagDMByTagOption(breadcrumbOptionId);
            var tagOptions          = tagDM.Tag.TagOptions;
            var breadcrumbOption    = _repository.GetTagOptionById(breadcrumbOptionId);

            // update current LayerNr
            var currentLayerNr = breadcrumbOption.LayerNr;
            tagDM.Tag.CurrentLayerNr = currentLayerNr;

            // remove options of higher layers
            RemovePreviousBreadcrumbs(tagDM);
            RemovePreviousOptions(tagDM);

            //_searchVM.UpdateVisuals(tag);
            SelectOption(breadcrumbOption.Id);
        }

        public void GoHome(int tagId)
        {
            var tagDM = _repository.GetTagDMById(tagId);
            tagDM.Tag.TagOptions.Clear();

            SetInputIsVisible(tagDM, false);

            LoadKeywordTypes(tagId);

            _searchVM.UpdateVisuals(tagDM);
        }

        // Enables editing for tag
        public void EditTag(int tagId)
        {
            var tagDM = _repository.GetTagDMById(tagId);

            tagDM.State = TagDataModel.States.Editing;

            SetMenuIsVisible(tagDM, true);
            SetEditIsVisible(tagDM, false);
            SetKeywordIsVisible(tagDM, false);

            // set last layer
            UpdateActiveLayerNumber(tagDM, tagDM.Tag.CurrentLayerNr - 1);

            _searchVM.UpdateVisuals(tagDM);
        }


        /// <summary>
        /// Assign selected keyword to tag and show it
        /// </summary>        
        public void AssignKeyword(TagDataModel tagDM, TagOption tagOption)
        {
            // assign keyword to tag
            tagDM.Tag.AssignedKeyword = tagOption.Keyword;

            // show keyword
            SetMenuIsVisible(tagDM, false);
            SetKeywordIsVisible(tagDM, true);
            SetEditIsVisible(tagDM, true);
        }


        #region Visibilities

        private void SetInputIsVisible(TagDataModel tagDM, bool visibility)
        {
            tagDM.IsInputVisible = visibility;
        }

        private void SetMenuIsVisible(TagDataModel tagDM, bool visibility)
        {
            tagDM.IsMenuVisible = visibility;
        }

        private void SetEditIsVisible(TagDataModel tagDM, bool visibility)
        {
            tagDM.IsEditVisible = visibility;
        }

        private void SetKeywordIsVisible(TagDataModel tagDM, bool isKeywordVisible)
        {
            // show or hide keyword
            tagDM.IsAssignedKeywordVisible = isKeywordVisible;
        }

        #endregion Visibilities
    }
}

﻿using System;
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

        [ImportingConstructor]
        public SearchOptionWorker(SearchViewModel searchVM, SearchTagViewModel searchTagVm, IMessageService messageService,
            Repository repository)
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
        }

        public void Initialize(SearchManager searchManager, ObservableCollection<TagDataModel> tags)
        {
            _searchManager = searchManager;
            _tagFactory     = new TagFactory(_repository);

            foreach (var tag in tags)
            {
                LoadKeywordTypes(tag.Id);
            }
        }

        public bool CanSelectOption() { return _searchVM.IsValid; }

        public void LoadKeywordTypes(int tagId)
        {
            var tag = _searchVM.Tags[tagId];

            tag.Tag.PreviousOptions.Clear();

            tag.Tag.CurrentLayerNr = 0;

            var tagOpts = tag.Tag.TagOptions;
            tagOpts.Clear();

            var keywordTypes = Enum.GetValues(typeof(KeywordTypes));
            foreach (KeywordTypes keywordType in keywordTypes)
            {
                if (keywordType == KeywordTypes.None) continue;

                // create Keyword, e.g. Artist, Type or Genre
                var keyword     = _tagFactory.CreateKeyword(keywordType.ToString(), keywordType);

                // create TagOption for this keyword type at layer 0
                var tagOption = _tagFactory.CreateTagOption(keyword, tag.Tag.CurrentLayerNr);

                tagOpts.Add(tagOption);
            }
        }

        public void SelectOption(int tagOptionId)
        {
            var tag         = _repository.GetTagDMByTagOption(tagOptionId);
            var tagId       = tag.Id;
            var tagOption   = _repository.GetTagOptionById(tagOptionId);
            var keywordType = tagOption.Keyword.Type;


            AddBreadcrumb(tag, tagOption.Keyword.Name);

            tag.Tag.CurrentLayerNr++;

            switch (tag.Tag.CurrentLayerNr)
            {
                case 1: // ---layer 1---
                {
                    if (keywordType == KeywordTypes.Artist || keywordType == KeywordTypes.Title)
                    {
                        // init selected keyword of tag and set its type
                        tag.Tag.AssignedKeyword = _tagFactory.CreateKeyword(tagOption.Keyword.Name, tagOption.Keyword.Type);

                        SetInputIsVisible(tag, true);
                    }
                    if (keywordType == KeywordTypes.Genre)
                    {   // load top genres
                        var genres = _searchManager.getGenres();

                        foreach (var genre in genres)
                        {
                            var keyword = _tagFactory.CreateKeyword(genre.genre_name, KeywordTypes.Genre);

                            var genreOption = _tagFactory.CreateTagOption(keyword, tag.Tag.CurrentLayerNr);

                            tag.Tag.TagOptions.Add(genreOption);
                        }
                    }
                    else if (keywordType == KeywordTypes.Attribute)
                    {   // load attributes

                    }

                    _searchVM.UpdateVisuals(tag);

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

                            var genreOption = _tagFactory.CreateTagOption(keyword, tag.Tag.CurrentLayerNr);

                            tag.Tag.TagOptions.Add(genreOption);
                        }
                    }

                    _searchVM.UpdateVisuals(tag);

                    break;
                }
                case 3: // ---layer 3---
                {
                    AssignKeyword(tag, tagOption);

                    break;
                }
            }
            // update menu
            _searchVM.UpdateVisuals(tag);
        }

        public void LoadSuggestions(int tagId)
        {
            var tag             = _repository.GetTagDMById(tagId);
            var terms           = _repository.GetTagDMById(tagId).InputTerms;
            var keywordType     = tag.Tag.AssignedKeyword.Type;

            AddBreadcrumb(tag, terms);

            tag.Tag.CurrentLayerNr++;

            // get all options of this tag
            var tagOptions = _repository.GetTagOptionsByTagId(tagId);

            // remove previous options at this layer
            tagOptions.ToList().RemoveAll(to => to.LayerNr == tag.Tag.CurrentLayerNr);

            if (keywordType == KeywordTypes.Artist)
            {
                //var helper = new BackgroundWorkHelper();
                //helper.DoInBackground(_searchManager.getArtistSuggestions(tagId, terms), , 

                var suggestions = new List<ResponseContainer.ResponseObj.ArtistSuggestion>()
                {
                    new ResponseContainer.ResponseObj.ArtistSuggestion()
                    {
                        id = "MyId1",
                        name = "Haxen",
                        originId = 0
                    },
                    new ResponseContainer.ResponseObj.ArtistSuggestion()
                    {
                        id = "MyId2",
                        name = "Haxen2",
                        originId = 0
                    }
                };

                //var suggestions = _searchManager.getArtistSuggestions(tagId, terms);

                foreach (var suggestion in suggestions)
                {
                    // create keyword out of this suggestion
                    var keyword = _tagFactory.CreateKeyword(suggestion.name, keywordType);
                    keyword.SearchId = suggestion.id;
                    // create option with this keyword
                    var tagOption = _tagFactory.CreateTagOption(keyword, tag.Tag.CurrentLayerNr);

                    tagOptions.Add(tagOption);
                }
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
                    var tagOption = _tagFactory.CreateTagOption(keyword, tag.Tag.CurrentLayerNr);

                    tagOptions.Add(tagOption);
                }
            }
            SetInputIsVisible(tag, false);

            _searchVM.UpdateVisuals(tag);
        }

        public static void GetArtistsInBackgr(object sender, DoWorkEventArgs e)
        {

        }

        private void CollectingCmsDataCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
            }
            else if (e.Error != null)
            {
            }
            else
            {
            }
        }
        

        private void AddBreadcrumb(TagDataModel tag, string name)
        {
            var breadcrumbKeyword = _tagFactory.CreateKeyword(name, KeywordTypes.None);
            var breadcrumbTagOption = _tagFactory.CreateTagOption(breadcrumbKeyword, tag.Tag.CurrentLayerNr);

            tag.Tag.PreviousOptions.Add(breadcrumbTagOption);
        }

        private void RemovePreviousBreadcrumbs(TagDataModel tag)
        {
            if (tag.Tag.PreviousOptions != null)
            {
                tag.Tag.PreviousOptions.ToList().RemoveAll(p => p.LayerNr >= tag.Tag.CurrentLayerNr);
            }
        }

        public void GoToBreadcrumb(int tagOptionId)
        {
            var tag = _repository.GetTagDMByTagOption(tagOptionId);
            var tagOptions = tag.Tag.TagOptions;
            var tagOption = _repository.GetTagOptionById(tagOptionId);


            // update current LayerNr
            var currentLayerNr = tagOption.LayerNr;
            tag.Tag.CurrentLayerNr = currentLayerNr;

            RemovePreviousBreadcrumbs(tag);

            // remove TagOptions of higher layers
            tagOptions.ToList().RemoveAll(to => to.LayerNr > currentLayerNr);

            _searchVM.UpdateVisuals(tag);
        }

        public void GoHome(int tagId)
        {
            var tag = _repository.GetTagDMById(tagId);
            tag.Tag.TagOptions.Clear();
            //tag.Suggestions.Clear();

            SetInputIsVisible(tag, false);

            LoadKeywordTypes(tagId);

            _searchVM.UpdateVisuals(tag);
        }

        // Enables editing for tag
        public void EditTag(int tagId)
        {
            var tag = _repository.GetTagDMById(tagId);

            tag.Tag.AssignedKeyword.SearchId    = null;
            tag.Tag.AssignedKeyword.Name        = null;

            SetMenuIsVisible(tag, true);
            SetEditIsVisible(tag, false);
            SetKeywordIsVisible(tag, false);

            // set last layer
            tag.Tag.CurrentLayerNr--;

            RemovePreviousBreadcrumbs(tag);

            _searchVM.UpdateVisuals(tag);
        }


        /// <summary>
        /// Assign selected keyword to tag and show it
        /// </summary>        
        public void AssignKeyword(TagDataModel tag, TagOption tagOption)
        {
            // assign keyword to tag
            tag.Tag.AssignedKeyword = tagOption.Keyword;

            // show keyword
            SetMenuIsVisible(tag, false);
            SetKeywordIsVisible(tag, true);
            SetEditIsVisible(tag, true);
        }


        #region Visibilities

        private void SetInputIsVisible(TagDataModel tag, bool visibility)
        {
            tag.IsInputVisible = visibility;
        }

        private void SetMenuIsVisible(TagDataModel tag, bool visibility)
        {
            tag.IsMenuVisible = visibility;
        }

        private void SetEditIsVisible(TagDataModel tag, bool visibility)
        {
            tag.IsEditVisible = visibility;
        }

        private void SetKeywordIsVisible(TagDataModel tag, bool isKeywordVisible)
        {
            // show or hide keyword
            tag.IsAssignedKeywordVisible = isKeywordVisible;
        }

        #endregion Visibilities
    }
}

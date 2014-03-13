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

        public void Initialize(SearchManager searchManager)
        {
            _searchManager = searchManager;
            _tagFactory     = new TagFactory(_repository);
        }

        public bool CanSelectOption() { return _searchVM.IsValid; }

        public void LoadSuggestions(int tagId)
        {
            var tag         = _repository.GetTagDMById(tagId);
            var terms       = _repository.GetTagDMById(tagId).InputTerms;
            var keywordType = tag.Tag.AssignedKeyword.Type;

            if (keywordType == KeywordTypes.Artist)
            {
                var artistSuggestions = _searchManager.getArtistSuggestions(tagId, terms);
                CreateSuggestions(tagId, tag, artistSuggestions, keywordType);
            }
            else if (keywordType == KeywordTypes.Title)
            {
                //var artistSuggestions = _searchManager.getSongSuggestions(tagId, terms);
                //CreateSuggestions(tagId, tag, artistSuggestions);
            }
            SetInputIsVisible(tag, false);
        }

        private void CreateSuggestions(int tagId, TagDataModel tag, List<ResponseContainer.ResponseObj.ArtistSuggestion> suggestions, KeywordTypes keywordType)
        {
            // suggestions are shown in layer 2
            tag.Tag.CurrentLayerNr = 2;

            // get all options of this tag
            var tagOptions = _repository.GetTagOptionsByTagId(tagId);

            // remove previous options at this layer
            tagOptions.ToList().RemoveAll(to => to.LayerNr == tag.Tag.CurrentLayerNr);

            foreach (var suggestion in suggestions)
            {
                // create keyword out of this suggestion
                var keyword = _tagFactory.CreateKeyword(suggestion.name, keywordType);

                // create option with this keyword
                var tagOption   = _tagFactory.CreateTagOption(keyword, tag.Tag.CurrentLayerNr);

                tagOptions.Add(tagOption);
            }
            _searchVM.UpdateVisuals(tag);
        }

        public void SelectOption(int tagOptionId)
        {
            var tag         = _repository.GetTagDMByTagOption(tagOptionId);
            var tagOption   = _repository.GetTagOptionById(tagOptionId);
            var keywordType = tagOption.Keyword.Type;

            if (tag.Tag.CurrentLayerNr == 0)
            {
                // init selected keyword of tag so that the type is clear
                tag.Tag.AssignedKeyword = _tagFactory.CreateKeyword(tagOption.Keyword.Name, tagOption.Keyword.Type);

                UpdateOptions(tag.Id, tagOption);
            }
            else if (keywordType == KeywordTypes.Genre)
            {

            }
            else
            {
                if (tag.Tag.CurrentLayerNr == 1)
                {
                    UpdateOptions(tag.Id, tagOption);
                }
                else if (tag.Tag.CurrentLayerNr == 2)
                {
                    AssignKeyword(tag, tagOption);
                }
            }
        }

        /// <summary>
        /// Load next options for a tag.
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="selectedOption">Is null if there hasn't been a previous selection</param>
        public void UpdateOptions(int tagId, TagOption selectedOption = null)
        {
            // Load selection types
            if (selectedOption == null)
            {
                LoadKeywordTypes(tagId);
                return; // break method execution
            }

            var tag             = _repository.GetTagDMById(tagId);
            var keyword         = selectedOption.Keyword;
            var type            = selectedOption.Keyword.Type;
            var currentLayerNr  = tag.Tag.CurrentLayerNr;

            // if previously KeywordType was selected in layer 0, next LayerNr is 1
            if (currentLayerNr == 0) currentLayerNr = 1;

            if (currentLayerNr == 1 && type == KeywordTypes.Artist)
            {
                SetInputIsVisible(tag, true);
                // SetKeyword(selectedOption);
                // Event: Keyword gesetzt
            }
            else if (currentLayerNr == 1 && type == KeywordTypes.Title)
            {
                SetInputIsVisible(tag, true);
            }
            else if (currentLayerNr == 1 && type == KeywordTypes.Genre)
            {

            }
        }

        private void SetInputIsVisible(TagDataModel tag, bool visibility)
        {
            tag.IsInputVisible = visibility;
        }

        private void SetKeyword(TagOption selectedOption)
        {
            var artist = selectedOption.Keyword;
            _searchManager.SongsByArtistIDQuery(artist.Name, selectedOption.Id);

            _searchVM.ShowKeyword(selectedOption);
        }

        public void LoadKeywordTypes(int tagId)
        {
            var tag                 = _searchVM.Tags[tagId];

            // keyword type selection is in layer 0
            var layerNumber         = 0;
            tag.Tag.CurrentLayerNr  = layerNumber;

            var tagOpts             = tag.Tag.TagOptions;
            tagOpts.Clear();

            var keywordTypes = Enum.GetValues(typeof(KeywordTypes));
            foreach (KeywordTypes keywordType in keywordTypes)
            {
                if (keywordType == KeywordTypes.None) continue;

                // create Keyword, e.g. Artist, Type or Genre
                var keyword = _tagFactory.CreateKeyword(keywordType.ToString(), keywordType);

                // create TagOption for this keyword
                var tagOption   = _tagFactory.CreateTagOption(keyword, layerNumber);

                tagOpts.Add(tagOption);
            }

            //_searchVM.UpdateMenuItems((ISearchTagView)tag.TagVisDef, tag);
        }

        /// <summary>
        /// Assign selected keyword to tag and show it
        /// </summary>
        public void AssignKeyword(TagDataModel tag, TagOption tagOption)
        {
            // assign keyword to tag
            tag.Tag.AssignedKeyword = tagOption.Keyword;

            // show keyword
            SetKeywordIsVisible(tag, true);
        }

        private void SetKeywordIsVisible(TagDataModel tag, bool isKeywordVisible)
        {
            // show or hide keyword
            tag.IsAssignedKeywordVisible = isKeywordVisible;

            // hide menu if keyword is visible, show menu if keyword is invisible
            tag.IsMenuVisible = isKeywordVisible == true ? false : true;

            _searchVM.UpdateVisuals(tag);
        }
    }
}

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
using MusicSearch.Objects;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Ctms.Applications.Common;

namespace Ctms.Applications.Workers
{
    /*
        This worker controls the control flow of the tangibles' options.
        It separates the possible options into layers.
        There are four different keyword types: artist, title, genre, attribute.
        The control flows for artist and title are the same, but for genre and attribute different.
        This is an overview of the flow through the layers:
        
        layer0 | load types (artist, title, genre, attribute)
     
                artist&title    |   genre       |  attribute     
        layer1 | show input     | showTopGenres | showAttrType
        layer2 | loadSuggestions| showSubGenres | showAttributes
        layer3 | assignSugg.    | assignGenre   | showInput
        layer4 | -              | -             | confirmInput    
     
        TagOptions are all options that are displayed in the circle and can be selected 
        during the control flow. They contain keywords from which one will be assigned to a tag.
        
    */


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
                SetKeywordTypesIsVisible(tagDM, true);
                SetIsMenuVisible(tagDM, false);
                SetIsCircleMenuVisible(tagDM, false);
            }
        }

        public bool CanSelectOption() { return _searchVM.IsValid; }

        public void LoadKeywordTypes(int tagId)
        {
            var tagDm = _searchVM.Tags[tagId];

            tagDm.Tag.BreadcrumbOptions.Clear();

            UpdateActiveLayerNumber(tagDm, 0);

            var tagOpts = tagDm.Tag.TagOptions;
            tagOpts.Clear();

            var keywordTypes = Enum.GetValues(typeof(KeywordTypes));
            foreach (KeywordTypes keywordType in keywordTypes)
            {
                if (keywordType == KeywordTypes.None) continue;

                // create Keyword, e.g. Artist, Type or Genre
                var keyword     = _tagFactory.CreateKeyword(keywordType.ToString(), keywordType);

                // create TagOption for this keyword type at layer 0
                var tagOption = _tagFactory.CreateTagOption(keyword, tagDm.Tag.CurrentLayerNr);

                tagOpts.Add(tagOption);
            }

            SetIsMenuVisible(tagDm, true);
            SetIsCircleMenuVisible(tagDm, false);
        }

        public void UpdateActiveLayerNumber(TagDataModel tagDM, int currentLayerNr)
        {
            tagDM.Tag.CurrentLayerNr = currentLayerNr;
            tagDM.RefreshLayer();
        }

        public void SelectKeywordType(int tagId, KeywordTypes keywordType)
        {
            var tagOption = _repository.GetTagOption(tagId, keywordType);
            if (tagOption != null) SelectOption(tagOption.Id);
        }

        public void SelectOption(int tagOptionId)
        {
            var tagDm               = _repository.GetTagDMByTagOption(tagOptionId);
            var tagId               = tagDm.Id;
            var selectedTagOption   = _repository.GetTagOptionById(tagOptionId);
            var keywordType         = selectedTagOption.Keyword.KeywordType;

            // add breadcrumb only if the next stop is not assignKeyword
            if (keywordType != KeywordTypes.Attribute && selectedTagOption.LayerNr != 2)
                AddBreadcrumb(tagDm, selectedTagOption);
            else if (keywordType == KeywordTypes.Attribute && selectedTagOption.LayerNr != 3)
                AddBreadcrumb(tagDm, selectedTagOption);

            // remove previously shown infos
            _infoWorker.RemoveTagInfo(tagId);

            UpdateActiveLayerNumber(tagDm, tagDm.Tag.CurrentLayerNr + 1);

            switch (tagDm.Tag.CurrentLayerNr)
            {
                case 1: // ---layer 1---
                {
                    SetKeywordTypesIsVisible(tagDm, false);

                    if (keywordType == KeywordTypes.Artist || keywordType == KeywordTypes.Title)
                    {
                        // init selected keyword of tag and set its type
                        tagDm.Tag.AssignedKeyword = _tagFactory.CreateKeyword(selectedTagOption.Keyword.DisplayName, selectedTagOption.Keyword.KeywordType);
                        tagDm.InputTerms = "";

                        var hint = "Type \"" + keywordType.ToString() + "\" here";
                        SetIsInputVisible(tagDm, true, hint);
                        SetIsMenuVisible(tagDm, true);
                        SetIsCircleMenuVisible(tagDm, false);

                        if (keywordType == KeywordTypes.Artist) tagDm.BackgrImageSource = CommonVal.ImageSource_TagBackgrArtist;
                        if (keywordType == KeywordTypes.Title) tagDm.BackgrImageSource  = CommonVal.ImageSource_TagBackgrTitle;
                    }
                    else if (keywordType == KeywordTypes.Genre)
                    {   // load top genres
                        var genres = _searchManager.getGenres();

                        // load genres into tagoptions
                        foreach (var genre in genres)
                        {
                            var keyword = _tagFactory.CreateKeyword(genre.genre_name, KeywordTypes.Genre);

                            var genreOption = _tagFactory.CreateTagOption(keyword, tagDm.Tag.CurrentLayerNr);

                            tagDm.Tag.TagOptions.Add(genreOption);
                        }

                        tagDm.BackgrImageSource = CommonVal.ImageSource_TagBackgrGenre;
                        SetIsMenuVisible(tagDm, true);
                        SetIsCircleMenuVisible(tagDm, true);
                    }
                    else if (keywordType == KeywordTypes.Attribute)
                    {   // load attributes
                        var artistTagOption = _tagFactory.CreateTagOption(AttributeTypes.Artist.ToString(), KeywordTypes.Attribute, tagDm.Tag.CurrentLayerNr);
                        tagDm.Tag.TagOptions.Add(artistTagOption);

                        var titleTagOption = _tagFactory.CreateTagOption(AttributeTypes.Genre.ToString(), KeywordTypes.Attribute, tagDm.Tag.CurrentLayerNr);
                        tagDm.Tag.TagOptions.Add(titleTagOption);

                        tagDm.BackgrImageSource = CommonVal.ImageSource_TagBackgrAttribute;

                        SetIsMenuVisible(tagDm, true);
                        SetIsCircleMenuVisible(tagDm, true);
                    }

                    break;
                }
                case 2: // ---layer 2---
                {   // for artist and genre LoadSuggestions() is responsible for this layer

                    if (keywordType == KeywordTypes.Genre)
                    {   // load subgenres

                        var genre = _searchManager.getGenres().FirstOrDefault(g => g.genre_name == selectedTagOption.Keyword.DisplayName);

                        foreach (var subGenre in genre.Subgenres)
                        {
                            var keyword = _tagFactory.CreateKeyword(subGenre.name, KeywordTypes.Genre);

                            var genreOption = _tagFactory.CreateTagOption(keyword, tagDm.Tag.CurrentLayerNr);

                            tagDm.Tag.TagOptions.Add(genreOption);
                        }
                        SetIsMenuVisible(tagDm, true);
                        SetIsCircleMenuVisible(tagDm, true);
                    }
                    else if (keywordType == KeywordTypes.Attribute)
                    {   // load attributes
                        var attributeType = (AttributeTypes)Enum.Parse(typeof(AttributeTypes), selectedTagOption.Keyword.DisplayName);

                        var attributes = _searchManager.getCombinedSearchAttributes(attributeType);
                        foreach (var attribute in attributes)
                        {
                            var tagOption = _tagFactory.CreateTagOption(attribute.Value.description, KeywordTypes.Attribute, tagDm.Tag.CurrentLayerNr);
                            tagOption.Keyword.Key = attribute.Key;
                            tagOption.Keyword.AttributeType = attributeType;
                            tagDm.Tag.TagOptions.Add(tagOption);
                        }
                    }

                    SetIsMenuVisible(tagDm, true);
                    SetIsCircleMenuVisible(tagDm, true);
                    SetIsKeywordVisible(tagDm, false);

                    break;
                }
                case 3: // ---layer 3---
                {
                    if (keywordType == KeywordTypes.Attribute)
                    {
                        tagDm.Tag.AssignedKeyword = selectedTagOption.Keyword;

                        var hint = "Define \"" + tagDm.Tag.AssignedKeyword.DisplayName + "\" here";
                        SetIsInputVisible(tagDm, true, hint);
                        SetIsInputControlVisible(tagDm, true);
                        SetIsMenuVisible(tagDm, true);
                        SetIsCircleMenuVisible(tagDm, false);

                        // get assigned attribute obj
                        var attribute = GetAttributeObj(tagDm.Tag.AssignedKeyword);

                        // set default value
                        if (attribute.option1 != null) tagDm.InputTerms = attribute.option1;
                        else tagDm.InputTerms = attribute.min.ToString();


                        if (attribute != null)
                        {
                            ShowAttributeRangeHint(true, attribute, tagId);
                        }
                    }
                    else
                    {
                        AssignKeyword(tagDm, selectedTagOption);
                    }
                    break;
                }
            }
            // update menu
            _searchVM.UpdateVisuals(tagDm);
        }

        public AttributeObj GetAttributeObj(Keyword keyword)
        {
            var attributes      = _searchManager.getCombinedSearchAttributes(keyword.AttributeType);
            if (attributes.ContainsKey(keyword.Key))
            {
                return attributes[keyword.Key];
            }
            else return null;
        }

        /// <summary>
        /// Show a hint for this tagOption's valid attribute input range
        /// </summary>
        /// <param name="isInputInvalid">Is input not valid</param>
        /// <param name="attribute">The attribute</param>
        /// <param name="tagId">The concerning tag</param>
        public void ShowAttributeRangeHint(bool isInputValid, AttributeObj attribute, int tagId)
        {
            string min;
            string max;
            string range;

            if (attribute.option1 != null)
            {   // binary min/max
                min = attribute.option1;
                max = attribute.option2;
                range = String.Format("{0}-{1}", min, max);
            }
            else
            {   // numeric min/max
                min = attribute.min.ToString();
                max = attribute.max.ToString();
                range = String.Format("{0} or {1}", min, max);
            }

            // if given input is invalid mark info as warning
            var mainText = isInputValid == false ? "Invalid input" : "";

            _infoWorker.ShowTagInfo(mainText, "Choose a value of " + range, tagId);         
        }

        /// <summary>
        /// Load artist or title suggestions for a tag
        /// </summary>
        public void ConfirmInput(int tagId)
        {
            var tagDm       = _repository.GetTagDMById(tagId);
            var terms       = _repository.GetTagDMById(tagId).InputTerms;
            var keywordType = tagDm.Tag.AssignedKeyword.KeywordType;

            // remove previously shown infos
            _infoWorker.RemoveTagInfo(tagDm.Id);

            Keyword keyword;
            TagOption tagOption;

            if(keywordType == KeywordTypes.Artist || keywordType == KeywordTypes.Title)
            {
                keyword = _tagFactory.CreateKeyword(terms, keywordType);
                tagOption = _tagFactory.CreateTagOption(keyword, tagDm.Tag.CurrentLayerNr);

                _repository.AddTagOption(tagDm, tagOption);
            
                AddBreadcrumb(tagDm, tagOption);
            }

            UpdateActiveLayerNumber(tagDm, tagDm.Tag.CurrentLayerNr + 1);

            // get all options of this tag
            var tagOptions = _repository.GetTagOptionsByTagId(tagId);

            // remove previous options at this layer
            tagOptions.RemoveAll(to => to.LayerNr == tagDm.Tag.CurrentLayerNr);

            if (keywordType == KeywordTypes.Artist)
            {
                // get artist suggestions in background
                var backgrWorker = new BackgroundWorkHelper();
                backgrWorker.DoInBackground(GetArtistsSuggestionsInBackgr, GetArtistsSuggestionsCompleted, tagDm);

                SetIsCircleMenuVisible(tagDm, true);
                SetIsInputVisible(tagDm, false);
            }
            else if (keywordType == KeywordTypes.Title)
            {
                // get title suggestions in background
                var backgrWorker = new BackgroundWorkHelper();
                backgrWorker.DoInBackground(GetTitleSuggestionsInBackgr, GetTitleSuggestionsCompleted, tagDm);

                SetIsCircleMenuVisible(tagDm, true);
                SetIsInputVisible(tagDm, false);
            }
            else if (keywordType == KeywordTypes.Attribute)
            {
                tagDm.Tag.AssignedKeyword.Value = terms != null ? terms : "";

                // set values of keyword
                keyword = _tagFactory.CreateKeyword(
                    tagDm.Tag.AssignedKeyword.DisplayName,
                    tagDm.Tag.AssignedKeyword.KeywordType, 
                    tagDm.Tag.AssignedKeyword.Value.ToString()
                );
                keyword.AttributeType = tagDm.Tag.AssignedKeyword.AttributeType;
                keyword.DisplayDescription = terms;
                keyword.Key = tagDm.Tag.AssignedKeyword.Key;

                // creat tagOption for this keyword
                tagOption = _tagFactory.CreateTagOption(keyword, tagDm.Tag.CurrentLayerNr);
                
                // if input is valid assign keyword, else show hint that input is valid
                var attribute = GetAttributeObj(keyword);
                if (attribute != null && IsAttributeInputValid(tagDm, attribute, terms))
                {
                    SetIsInputControlVisible(tagDm, false);
                    SetIsInputVisible(tagDm, false);

                    AssignKeyword(tagDm, tagOption);
                }
            }
            _searchVM.UpdateVisuals(tagDm);
        }

        private bool IsAttributeInputValid(TagDataModel tagDM, AttributeObj attribute, object terms)
        {
            if(attribute.option1 != null)
            {
                if ((string) terms != attribute.option1 && (string) terms != attribute.option2)
                {
                    ShowAttributeRangeHint(false, attribute, tagDM.Id);
                    return false;
                }
            }
            else
            {   // min and max are defined
                var min = attribute.min;
                var max = attribute.max;

                double termsNr;
                if (terms is string) termsNr = ConvertInputToDouble(tagDM, attribute, (string)terms);
                else termsNr = (double) terms;

                if (termsNr < min)
                {   // input is too low
                    ShowAttributeRangeHint(false, attribute, tagDM.Id);
                    return false;
                }
                else if (termsNr > max)
                {   // input is too high
                    ShowAttributeRangeHint(false, attribute, tagDM.Id);
                    return false;
                }
            }
            return true;
        }

        private double ConvertInputToDouble(TagDataModel tagDM, AttributeObj attribute, string terms)
        {
            double termsNr;
            try
            {   // convert string to int
                termsNr = NumberHelper.TryToParseStringToDouble(terms);
                return termsNr;
            }
            catch (Exception)
            {
                ShowAttributeRangeHint(false, attribute, tagDM.Id);
                return 0;
            }
        }

        public void EditInput(int tagId, string editType)
        {
            // read values
            var tagDM = _repository.GetTagDMById(tagId);
            var terms = _repository.GetTagDMById(tagId).InputTerms;

            var keywordType     = tagDM.Tag.AssignedKeyword.KeywordType;
            var keywordKey      = tagDM.Tag.AssignedKeyword.Key;
            var keywordName     = tagDM.Tag.AssignedKeyword.DisplayName;
            var attributeType   = tagDM.Tag.AssignedKeyword.AttributeType;

            // remove previously shown infos
            _infoWorker.RemoveTagInfo(tagId);

            // get attributes of current attribute type
            var attributes = _searchManager.getCombinedSearchAttributes(attributeType);

            // test if key is existent in dictionary
            if (attributes.ContainsKey(keywordKey) == true)
            {   // read values
                var attribute = attributes[keywordKey];

                if (attribute != null)
                {
                    var description = attribute.description;

                    // check if option1 and option2 are defined or min and max
                    if (attribute.option1 != null && attribute.option2 != null)
                    {   // there is only a binary option choice                        
                        var option1 = attribute.option1;
                        var option2 = attribute.option2;

                        if (editType == "Lower" && terms == option2)
                        {   // set input to lower option
                            tagDM.InputTerms = option1;
                            _infoWorker.RemoveTagInfo(tagId);
                        }
                        else if (editType == "Raise" && terms == option1)
                        {   // set input to higher option
                            tagDM.InputTerms = option2;
                            _infoWorker.RemoveTagInfo(tagId);                         
                        }
                        else
                        {
                            ShowAttributeRangeHint(false, attribute, tagId);
                        }
                    }
                    else
                    {   // numberic min and max are defined
                        var termsNr = ConvertInputToDouble(tagDM, attribute, terms);

                        if (editType == "Lower" && IsAttributeInputValid(tagDM, attribute, (termsNr - 1)))
                        {   // lower is valid
                            if (attribute.max == 1.0 && IsAttributeInputValid(tagDM, attribute, (termsNr - 0.1)))
                            {   // range is 0.0 till 1.0 -> small steps
                                termsNr -= 0.1;
                            }
                            else if (IsAttributeInputValid(tagDM, attribute, (termsNr - 1)))
                                termsNr--; // large steps

                            // remove previously shown infos
                            _infoWorker.RemoveTagInfo(tagId);
                        }
                        else if (editType == "Raise")
                        {   // raise is valid
                            if (attribute.max == 1.0 && IsAttributeInputValid(tagDM, attribute, (termsNr + 0.1)))
                                // range is 0.0 till 1.0 -> small steps
                                termsNr += 0.1;
                            else if (IsAttributeInputValid(tagDM, attribute, (termsNr + 1)))
                                termsNr++; // large steps

                            // remove previously shown infos
                            _infoWorker.RemoveTagInfo(tagId);
                        }
                        else
                        {   // lower and raise aren't valid. show warning.
                            ShowAttributeRangeHint(false, attribute, tagId);
                        }
                        // update input field terms on UI
                        tagDM.InputTerms = termsNr.ToString().Replace(",", ".");
                    }
                }
            }
        }

        /// <summary>
        /// Do work of getting suggestions for artists in background
        /// </summary>
        public void GetArtistsSuggestionsInBackgr(object sender, DoWorkEventArgs e)
        {
            var tagDM = (TagDataModel) e.Argument;

            // set result to tagDM if backgr work cancelles/throws an error
            e.Result = tagDM;

            SetIsLoadingInfoVisible(tagDM, true);            

            e.Result = _searchManager.getArtistSuggestions(tagDM.Id, tagDM.InputTerms);
        }

        private void GetArtistsSuggestionsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TagDataModel tagDM = null;

            // if result has been cancelled or thrown an error result is just the tagDM
            if(e.Result is TagDataModel) tagDM = (TagDataModel) e.Result;

            if (e.Cancelled)
            {
            }
            else if (e.Error != null)
            {
            }
            else
            {
                var suggestions = (List<ResponseContainer.ResponseObj.ArtistSuggestion>) e.Result;

                var firstSuggestion = suggestions.FirstOrDefault();
                if (suggestions != null && suggestions.Any())
                {
                    tagDM = _repository.GetTagDMById(suggestions.FirstOrDefault().originId);
                    if(String.IsNullOrEmpty(suggestions.FirstOrDefault().name))
                    {
                        _infoWorker.ShowTagInfo("No artists found", "Please adjust your terms", tagDM.Id, "Ok");
                    }
                }

                for (var i = 0; i < suggestions.Count; i++)
                {
                    // create keyword out of this suggestion
                    var keyword = _tagFactory.CreateKeyword(suggestions[i].name, tagDM.Tag.AssignedKeyword.KeywordType);
                    keyword.Key = suggestions[i].id;

                    // create option with this keyword
                    var tagOption = _tagFactory.CreateTagOption(keyword, tagDM.Tag.CurrentLayerNr);

                    _repository.AddTagOption(tagDM, tagOption);
                }
                SetIsInputVisible(tagDM, false);

                _searchVM.UpdateVisuals(tagDM);
            }
            SetIsLoadingInfoVisible(tagDM, false);
        }

        public void GetTitleSuggestionsInBackgr(object sender, DoWorkEventArgs e)
        {
            var tagDM = (TagDataModel)e.Argument;

            // set result to tagDM if backgr work cancelles/throws an error
            e.Result = tagDM;

            SetIsLoadingInfoVisible(tagDM, true);   

            e.Result = _searchManager.getTitleSuggestions(tagDM.Id, tagDM.InputTerms);
        }

        private void GetTitleSuggestionsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TagDataModel tagDM = null;

            // if result has been cancelled or thrown an error result is just the tagDM
            if (e.Result is TagDataModel) tagDM = (TagDataModel)e.Result;

            if (e.Cancelled)
            {
            }
            else if (e.Error != null)
            {
            }
            else
            {
                var suggestions = (List<ResponseContainer.ResponseObj.TitleSuggestion>)e.Result;

                var firstSuggestion = suggestions.FirstOrDefault();
                if (suggestions != null && suggestions.Any())
                {
                    tagDM = _repository.GetTagDMById(suggestions.FirstOrDefault().originId);
                    if (String.IsNullOrEmpty(suggestions.FirstOrDefault().title))
                    {
                        _infoWorker.ShowTagInfo("No titles found", "Please adjust your terms", tagDM.Id, "Ok");
                    }
                }

                for (var i = 0; i < suggestions.Count; i++)
                {
                    // create keyword out of this suggestion
                    var keyword = _tagFactory.CreateKeyword(suggestions[i].title, tagDM.Tag.AssignedKeyword.KeywordType);
                    keyword.Key = suggestions[i].id;

                    // create option with this keyword
                    var tagOption = _tagFactory.CreateTagOption(keyword, tagDM.Tag.CurrentLayerNr);

                    _repository.AddTagOption(tagDM, tagOption);
                }
                SetIsInputVisible(tagDM, false);

                _searchVM.UpdateVisuals(tagDM);
            }
            SetIsLoadingInfoVisible(tagDM, false);
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

            // do nothing if breadcrumb option of the last layer would be selected
            // (would repeat last select option and load currently displayed data again)
            if (breadcrumbOption.LayerNr < tagDM.Tag.CurrentLayerNr - 1)
            {   
                // update current LayerNr
                tagDM.Tag.CurrentLayerNr = breadcrumbOption.LayerNr;

                // remove options of higher layers
                RemovePreviousBreadcrumbs(tagDM);
                RemovePreviousOptions(tagDM);

                SetIsInputVisible(tagDM, false);
                SetIsInputControlVisible(tagDM, false);

                //_searchVM.UpdateVisuals(tag);
                SelectOption(breadcrumbOption.Id);
            }
        }

        public void GoHome(int tagId)
        {
            var tagDM = _repository.GetTagDMById(tagId);
            tagDM.Tag.TagOptions.Clear();

            // remove previously shown infos
            _infoWorker.RemoveTagInfo(tagId);

            LoadKeywordTypes(tagId);

            SetKeywordTypesIsVisible(tagDM, true);
            SetIsKeywordVisible(tagDM, false);
            SetIsInputVisible(tagDM, false);
            SetIsInputControlVisible(tagDM, false);
            SetIsMenuVisible(tagDM, false);

            _searchVM.UpdateVisuals(tagDM);
        }

        /// <summary>
        /// Set tag into edit state, to the last layer
        /// </summary>
        public void EditTag(int tagId)
        {
            var tagDm = _repository.GetTagDMById(tagId);

            tagDm.AssignState = TagDataModel.AssignStates.Editing;

            // remove previously shown infos
            _infoWorker.RemoveTagInfo(tagId);

            SetIsMenuVisible(tagDm, true);
            SetIsCircleMenuVisible(tagDm, true);
            SetIsEditVisible(tagDm, false);
            SetIsKeywordVisible(tagDm, false);

            if (tagDm.Tag.AssignedKeyword.KeywordType == KeywordTypes.Attribute)
            {
                SetIsCircleMenuVisible(tagDm, false);
                SetIsInputVisible(tagDm, true, "Define \"" + tagDm.Tag.AssignedKeyword.DisplayName + "\" here");
                SetIsInputControlVisible(tagDm, true);
            }

            // set last layer
            UpdateActiveLayerNumber(tagDm, tagDm.Tag.CurrentLayerNr - 1);

            _searchVM.UpdateVisuals(tagDm);
        }


        /// <summary>
        /// Assign selected keyword to tag and show it
        /// </summary>        
        public void AssignKeyword(TagDataModel tagDm, TagOption tagOption)
        {
            // assign keyword to tag
            tagDm.Tag.AssignedKeyword = tagOption.Keyword;

            // remove previously shown infos
            _infoWorker.RemoveTagInfo(tagDm.Id);

            // show keyword
            SetIsMenuVisible(tagDm, false);
            SetIsCircleMenuVisible(tagDm, false);
            SetIsKeywordVisible(tagDm, true);
            SetIsEditVisible(tagDm, true);

            tagDm.AssignState = TagDataModel.AssignStates.Assigned;
        }

        #region Visibilities

        private void SetKeywordTypesIsVisible(TagDataModel tagDM, bool isKeywordTypeVisible)
        {
            // show or hide keyword
            tagDM.IsKeywordTypesVisible = isKeywordTypeVisible;

            //SetIsMenuVisible(tagDM, !isKeywordTypeVisible);
        }

        private void SetIsInputVisible(TagDataModel tagDM, bool visibility, string inputTypeHint = null)
        {
            tagDM.IsInputVisible = visibility;

            tagDM.InputTypeHint = inputTypeHint != null ? inputTypeHint : "";
        }

        private void SetIsInputControlVisible(TagDataModel tagDM, bool visibility)
        {
            tagDM.IsInputControlVisible = visibility;
        }

        private void SetIsMenuVisible(TagDataModel tagDM, bool visibility)
        {
            tagDM.IsMenuVisible = visibility;
        }

        private void SetIsCircleMenuVisible(TagDataModel tagDM, bool visibility)
        {
            tagDM.IsCircleMenuVisible = visibility;
        }

        private void SetIsEditVisible(TagDataModel tagDM, bool visibility)
        {
            tagDM.IsEditVisible = visibility;
        }

        private void SetIsKeywordVisible(TagDataModel tagDM, bool isKeywordVisible)
        {
            // show or hide keyword
            tagDM.IsAssignedKeywordVisible = isKeywordVisible;
        }

        private void SetIsLoadingInfoVisible(TagDataModel tagDM, bool isLoadingHintVisible)
        {
            // show or hide loading info
            tagDM.IsLoadingInfoVisible = isLoadingHintVisible;
        }

        private void SetConfirmBreadcrumbIsVisible(TagDataModel tagDM, bool isConfirmBreadcrumbVisible)
        {
            // show or hide breadcrumb
            tagDM.IsConfirmBreadcrumbVisible = isConfirmBreadcrumbVisible;
        }

        #endregion Visibilities
    }
}

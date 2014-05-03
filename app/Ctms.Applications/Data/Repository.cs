using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicSearch.Objects;
using Ctms.Applications.Services;
using System.ComponentModel.Composition;
using Helpers;
using Ctms.Domain.Objects;
using Ctms.Applications.DataModels;
using System.Collections.ObjectModel;
using System.Waf.Foundation;
using Ctms.Applications.ViewModels;
using MusicSearch.Managers;
using Ctms.Domain;

namespace Ctms.Applications.Data
{
    [Export]
    //Provides methods to retain filtered and unfiltered data from the entity service. Uses LINQ to filter data.
    public class Repository : Model
    {
        private SearchViewModel     _searchVm;
        private DetailViewModel     _detailVm;
        private MenuViewModel       _menuVm;
        private PlaylistViewModel   _playlistVm;
        private ResultViewModel     _resultVm;
        private SearchTagViewModel  _searchTagVm;
        private InfoViewModel       _infoVm;
        private SearchManager       _searchManager;

        [ImportingConstructor]
        public Repository(SearchViewModel searchVm, DetailViewModel detailVm,
            MenuViewModel menuVm, PlaylistViewModel playlistVm, ResultViewModel resultVm, SearchTagViewModel searchTagVm,
            InfoViewModel infoVm, ShellViewModel shellVm)
        {
            _searchVm       = searchVm;
            _detailVm       = detailVm;
            _menuVm         = menuVm;
            _playlistVm     = playlistVm;
            _resultVm       = resultVm;
            _infoVm         = infoVm;
            _searchTagVm    = searchTagVm;
        }

        public void Initialize(SearchManager searchManager)
        {
            _searchManager = searchManager;
        }

        /// <summary>
        /// Get all styles
        /// </summary>
        /// <returns></returns>
        public List<Style> GetAllStyles()
        {
            //!! ToDo: _searchManager..getStyles
            // read and set styles
            var styles = XmlProvider.ReadStyles();
            //Convert to Observable?
            return styles.ToList();
        }

        /// <summary>
        /// Get all sub styles of a style
        /// </summary>
        /// <param name="styleId">id of parent style</param>
        /// <returns>sub styles</returns>
        public List<Style> GetSubStyles(int styleId)
        {
            return GetAllStyles().FirstOrDefault(s => s.Id == styleId).SubStyles;
        }


        #region Infos

        public ObservableCollection<InfoDataModel> GetAllCommonInfos()
        {
            return _infoVm.CommonInfos;
        }

        public ObservableCollection<TagInfoDataModel> GetAllTagInfos()
        {
            return _infoVm.TagInfos;
        }

        public ObservableCollection<InfoDataModel> GetAllTutorialInfos()
        {
            return _infoVm.TutorialInfos;
        }

        /// <summary>
        /// Get all tag infos
        /// </summary>
        public InfoDataModel GetCommonInfoById (int tagId)
        {
            return _infoVm.CommonInfos.FirstOrDefault(i => i.Info.Id == tagId);
        }

        /// <summary>
        /// Remove common info
        /// </summary>
        public void RemoveCommonInfoById(int infoId)
        {
            var commonInfo = GetCommonInfoById(infoId);
            _infoVm.CommonInfos.Remove(commonInfo);
        }

        /// <summary>
        /// Get all tag infos
        /// </summary>
        public TagInfoDataModel GetTagInfoByTagId(int tagId)
        {
            return _infoVm.TagInfos.FirstOrDefault(i => i.TagId == tagId);
        }

        /// <summary>
        /// Remove info from tag
        /// </summary>
        public void RemoveTagInfoById(int tagId)
        {
            var tagInfo = GetTagInfoByTagId(tagId);
            _infoVm.TagInfos.Remove(tagInfo);
        }

        /// <summary>
        /// Get all tag infos
        /// </summary>
        public InfoDataModel GetTutorialInfoById(int tagId)
        {
            return _infoVm.TutorialInfos.FirstOrDefault(i => i.Info.Id == tagId);
        }

        /// <summary>
        /// Remove tutorial info
        /// </summary>
        public void RemoveTutorialInfoById(int infoId)
        {
            var tutInfo = GetTutorialInfoById(infoId);
            _infoVm.TutorialInfos.Remove(tutInfo);
        }

        #endregion Infos

        #region Tags

        /// <summary>
        /// Get all tag Ids
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllTagIds()
        {
            var ids = new List<int>();
            foreach (var tag in GetAllTagDMs())
            {
                ids.Add(tag.Tag.Id);
            }
            return ids;
        }

        #endregion Tags

        #region TagDataModels

        /// <summary>
        /// Add tag data model
        /// </summary>
        /// <returns></returns>
        public void AddTagDMs(TagDataModel tag)
        {
            _searchVm.Tags.Add(tag);
        }

        /// <summary>
        /// Get all tags
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<TagDataModel> GetAllTagDMs()
        {
            return _searchVm.Tags;
        }


        /// <summary>
        /// Get all visible tags
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TagDataModel> GetAddedTagDMs()
        {
            return _searchVm.Tags.Where(t => t.ExistenceState == TagDataModel.ExistenceStates.Added);
        }

        /// <summary>
        /// Get all assigned tags
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TagDataModel> GetAssignedTagDMs()
        {
            return _searchVm.Tags.Where(t => t.AssignState == TagDataModel.AssignStates.Assigned);
        }

        /// <summary>
        /// Get all added and assigned tags
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TagDataModel> GetAddedAndAssignedTagDMs()
        {
            return _searchVm.Tags.Where(t => t.ExistenceState == TagDataModel.ExistenceStates.Added
                && t.AssignState == TagDataModel.AssignStates.Assigned);
        }

        /// <summary>
        /// Get all TagDataModels
        /// </summary>
        /// <returns></returns>
        public TagDataModel GetTagDMById(int id)
        {
            return _searchVm.Tags.FirstOrDefault(t => t.Tag.Id == id);
        }


        /// <summary>
        /// Get TagOption by id
        /// </summary>
        /// <returns></returns>
        public TagDataModel GetTagDMByTagOption(int tagOptionId)
        {
            return _searchVm.Tags
                        .FirstOrDefault(t => t.Tag.TagOptions
                            .Contains(t.Tag.TagOptions.FirstOrDefault(to => to.Id == tagOptionId)));
        }

        #endregion TagDataModels

        #region TagOptions

        /// <summary>
        /// Add tag option
        /// </summary>
        /// <returns></returns>
        public void AddTagOption(TagDataModel tag, TagOption tagOption)
        {
            tag.Tag.TagOptions.Add(tagOption);
        }

        /// <summary>
        /// Get tag option by id
        /// </summary>
        /// <returns></returns>
        public TagOption GetTagOptionById(int tagOptionId)
        {
            return GetAllTagOptions().FirstOrDefault(to => to.Id == tagOptionId);
        }

        /// <summary>
        /// Get tag options by tid
        /// </summary>
        public ObservableCollection<TagOption> GetTagOptionsByTagId(int tagId)
        {
            return GetTagDMById(tagId).Tag.TagOptions;
        }


        /// <summary>
        /// Get tag option by id
        /// </summary>
        public List<TagOption> GetAllTagOptions()
        {
            return _searchVm.Tags.SelectMany(t => t.Tag.TagOptions).Distinct().ToList();
        }

        /// <summary>
        /// Get tag option by tag Id and keywordType
        /// </summary>
        public TagOption GetTagOption(int tagId, KeywordTypes type)
        {
            return GetTagDMById(tagId).Tag.TagOptions.FirstOrDefault(to => to.Keyword.KeywordType == type && to.Keyword.DisplayName == type.ToString());
        }

        #endregion TagOptions


        #region TagCombinations

        /// <summary>
        /// Add tag combination data model
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<TagCombinationDataModel> GetAllTagCombinations()
        {
            return _searchVm.TagCombinations;
        }

        /// <summary>
        /// Add tag combination data model
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<TagCombinationDataModel> GetTagCombinationContainingTags()
        {
            //return _searchVm.TagCombinations.Where(t => t.Tags.Contains());
            return null;
        }

        /// <summary>
        /// Add tag combination data model
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<TagCombinationDataModel> GetUncombinedTags()
        {
            //return _searchVm.TagCombinations.Where(t => t.Tags.Contains());
            return null;
        }

        /// <summary>
        /// Add tag combination data model
        /// </summary>
        /// <returns></returns>
        public void AddTagCombination(TagCombinationDataModel tagCombination)
        {
            _searchVm.TagCombinations.Add(tagCombination);
        }

        /// <summary>
        /// Remove tag combination data model
        /// </summary>
        /// <returns></returns>
        public void RemoveTagCombination(TagCombinationDataModel tagCombination)
        {
            _searchVm.TagCombinations.Remove(tagCombination);
        }

        #endregion TagCombinations


        #region BreadcrumbOptions

        /// <summary>
        /// Get tag option by id
        /// </summary>
        public List<TagOption> GetAllBreadcrumbOptions()
        {
            return _searchVm.Tags.SelectMany(t => t.Tag.BreadcrumbOptions).Distinct().ToList();
        }

        /// <summary>
        /// Get tag option by id
        /// </summary>
        /// <returns></returns>
        public TagOption GetBreadcrumbOptionById(int breadcrumbOptionId)
        {
            return GetAllBreadcrumbOptions().FirstOrDefault(po => po.Id == breadcrumbOptionId);
        }

        #endregion BreadcrumbOptions

        #region Keywords


        /// <summary>
        /// Get tag option by id
        /// </summary>
        public ObservableCollection<Keyword> GetAllKeywords()
        {
            return EntitiesHelper.ToObservableCollection<Keyword>
                                    (GetAllTagOptions().Select(to => to.Keyword));
        }

        #endregion Keywords


        /*
        Example:         
        public Result GetResult(int id)
        {
            get { return entityService.Results.FirstOrDefault(t => t.Id == id); }
        }
        */
    }
}

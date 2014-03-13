using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicSearch.ResponseObjects;
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
        private EntityService _entityService;

        private SearchViewModel     _searchVm;
        private DetailViewModel     _detailVm;
        private MenuViewModel       _menuVm;
        private PlaylistViewModel   _playlistVm;
        private ResultViewModel     _resultVm;
        private SearchTagViewModel  _searchTagVm;
        private SearchManager       _searchManager;

        [ImportingConstructor]
        public Repository(EntityService entityService, SearchViewModel searchVm, DetailViewModel detailVm,
            MenuViewModel menuVm, PlaylistViewModel playlistVm, ResultViewModel resultVm, SearchTagViewModel searchTagVm,
            ShellViewModel shellVm)
        {
            _entityService = entityService;

            _searchVm       = searchVm;
            _detailVm       = detailVm;
            _menuVm         = menuVm;
            _playlistVm     = playlistVm;
            _resultVm       = resultVm;
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

        /// <summary>
        /// Get all tags
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<TagDataModel> GetAllTagDMs()
        {
            return _searchVm.Tags;
        }

        /// <summary>
        /// Get all tags
        /// </summary>
        /// <returns></returns>
        public TagDataModel GetTagDMById(int id)
        {
            return _searchVm.Tags.FirstOrDefault(t => t.Tag.Id == id);
        }


        /// <summary>
        /// Get tag option by id
        /// </summary>
        /// <returns></returns>
        public TagDataModel GetTagDMByTagOption(int tagOptionId)
        {
            return _searchVm.Tags
                        .FirstOrDefault(t => t.Tag.TagOptions
                            .Contains(t.Tag.TagOptions.FirstOrDefault(to => to.Id == tagOptionId)));
        }


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


        /// <summary>
        /// Add tag data model
        /// </summary>
        /// <returns></returns>
        public void AddTagDMs(TagDataModel tag)
        {
            _searchVm.Tags.Add(tag);
        }

        /// <summary>
        /// Get tag option by id
        /// </summary>
        /// <returns></returns>
        public TagOption GetTagOptionById(int tagOptionId)
        {
            //var tag = _searchVm.Tags.
            //                FirstOrDefault(t => t.Tag.TagOptions.
            //                    FirstOrDefault(to => to.Id == tagOptionId) != null);
            
            //return tag.Tag.TagOptions.FirstOrDefault(to => to.Id == tagOptionId);

            return GetAllTagOptions().FirstOrDefault(to => to.Id == tagOptionId);
        }

        /// <summary>
        /// Get tag options by tid
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<TagOption> GetTagOptionsByTagId(int tagId)
        {
            return GetTagDMById(tagId).Tag.TagOptions;
        }


        /// <summary>
        /// Get tag option by id
        /// </summary>
        /// <returns></returns>
        public List<TagOption> GetAllTagOptions()
        {
            return _searchVm.Tags.SelectMany(t => t.Tag.TagOptions).Distinct().ToList();
        }

        /*
        Example:         
        public Result GetResult(int id)
        {
            get { return entityService.Results.FirstOrDefault(t => t.Id == id); }
        }
        */
    }
}

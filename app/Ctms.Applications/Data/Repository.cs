using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicSearch.ResponseObjects;
using MusicSearch.SearchObjects;
using Ctms.Applications.Services;
using System.ComponentModel.Composition;
using Helpers;

namespace Ctms.Applications.Data
{
    [Export]
    //Provides methods to retain filtered and unfiltered data from the entity service. Uses LINQ to filter data.
    public class Repository
    {
        private EntityService _entityService;

        [ImportingConstructor]
        public Repository(EntityService entityService)
        {
            _entityService = entityService;
        }

        /// <summary>
        /// Get all styles
        /// </summary>
        /// <returns></returns>
        public List<Style> GetAllStyles()
        {
            return _entityService.Styles.ToList();
        }

        /// <summary>
        /// Get all sub styles of a style
        /// </summary>
        /// <param name="styleId">id of parent style</param>
        /// <returns>sub styles</returns>
        public List<Style> GetSubStyles(int styleId)
        {
            return _entityService.Styles.FirstOrDefault(s => s.KeywordId == styleId).SubStyles;
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

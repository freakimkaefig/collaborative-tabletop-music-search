using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ctms.Domain.Objects;
using MusicSearch.Objects;
using Microsoft.Surface.Presentation.Controls;
using Ctms.Applications.DataModels;
using Ctms.Applications.Data;
using Helpers;
using System.Collections.ObjectModel;
using Ctms.Domain;

namespace Ctms.Applications.DataFactories
{
    //Provides methods for CRUD-operations on the database-object playlist
    public class TagFactory : BaseFactory
    {
        public TagFactory(Repository repository)
            : base(repository)
        {
        }
        
        public TagDataModel CreateTagDataModel(TagVisualizationDefinition tagVisDef, int id)
        {
            // create Tag
            Tag tag = new Tag()
            {
                Id = id
            };

            // create TagDataModel wrapper for tag
            TagDataModel newTag = new TagDataModel(tag)
            {
                // add reference to TagVisualizationDefinition
                TagVisDef = tagVisDef
            };

            return newTag;
        }

        public TagOption CreateTagOption(Keyword keyword, int layerNumber)
        {
            var tagOptions = _repository.GetAllTagOptions();
            var nextFreeId = EntitiesHelper.CalcNextId<TagOption>(tagOptions, (t => t.Id));

            var tagOption = new TagOption(nextFreeId)
            {
                Keyword = keyword,
                LayerNr = layerNumber
            };

            return tagOption;
        }

        public TagOption CreateTagOption(string keywordName, KeywordTypes keywordType, int layerNumber, string keywordDescription = null)
        {
            var tagOptions = _repository.GetAllTagOptions();
            var nextFreeId = EntitiesHelper.CalcNextId<TagOption>(tagOptions, (t => t.Id));

            var keyword = CreateKeyword(keywordName, keywordType, keywordDescription);

            var tagOption = new TagOption(nextFreeId)
            {
                Keyword = keyword,
                LayerNr = layerNumber
            };

            return tagOption;
        }

        public Keyword CreateKeyword(string name, KeywordTypes type, string description = null)
        {
            var tagOptions = _repository.GetAllKeywords();
            var nextFreeId = EntitiesHelper.CalcNextId<Keyword>(tagOptions, (t => t.Id));

            var keyword = new Keyword(nextFreeId, name, type);

            return keyword;
        }

        public void Delete()
        {
        }        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ctms.Domain.Objects;
using MusicSearch.ResponseObjects;
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
                Id = id,
                //TagOptions = new ObservableCollection<TagOption>()
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
            var tagOption = new TagOption()
            {
                Keyword     = keyword,
                LayerNr = layerNumber
            };

            var tagOptions = _repository.GetAllTagOptions();

            var nextFreeId = CollectionHelper.CalcNextId<TagOption>(tagOptions, (t => t.Id));

            tagOption.Id = nextFreeId;

            return tagOption;
        }

        public void Delete()
        {
        }        
    }
}

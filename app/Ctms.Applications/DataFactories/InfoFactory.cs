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
    public class InfoFactory : BaseFactory
    {
        public InfoFactory(Repository repository)
            : base(repository)
        {
        }
        
        public InfoDataModel CreateCommonInfoDataModel()
        {
            var infos = _repository.GetAllCommonInfos();
            var nextFreeId = EntitiesHelper.CalcNextId<InfoDataModel>(infos, (t => t.Info.Id));

            // create Tag
            var info = new Info()
            {
                Id = nextFreeId,
                //TagOptions = new ObservableCollection<TagOption>()
            };

            // create TagDataModel wrapper for tag
            var newInfo = new InfoDataModel(info)
            {
                Info = info
            };

            return newInfo;
        }

        public InfoDataModel CreateTagInfoDataModel()
        {
            var infos = _repository.GetAllCommonInfos();
            var nextFreeId = EntitiesHelper.CalcNextId<InfoDataModel>(infos, (t => t.Info.Id));

            // create Tag
            var info = new Info()
            {
                Id = nextFreeId,
                //TagOptions = new ObservableCollection<TagOption>()
            };

            // create TagDataModel wrapper for tag
            var newInfo = new InfoDataModel(info)
            {
                Info = info
            };

            return newInfo;
        }

        public InfoDataModel CreateTutorialInfoDataModel()
        {
            var infos = _repository.GetAllCommonInfos();
            var nextFreeId = EntitiesHelper.CalcNextId<InfoDataModel>(infos, (t => t.Info.Id));

            // create Tag
            var info = new Info()
            {
                Id = nextFreeId,
                //TagOptions = new ObservableCollection<TagOption>()
            };

            // create TagDataModel wrapper for tag
            var newInfo = new InfoDataModel(info)
            {
                Info = info
            };

            return newInfo;
        }
    }
}

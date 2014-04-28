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
    public class InfoFactory : BaseFactory
    {
        public InfoFactory(Repository repository)
            : base(repository)
        {
        }

        public InfoDataModel CreateCommonInfo(string mainText, string subText)
        {
            // calc id
            var infos = _repository.GetAllCommonInfos();
            var nextFreeId = EntitiesHelper.CalcNextId<InfoDataModel>(infos, (t => t.Info.Id));

            // create info
            var info = new CommonInfo(nextFreeId, mainText, subText);

            // create TagDataModel wrapper for tag
            var newInfo = new InfoDataModel(info)
            {
                Info = info
            };
            return newInfo;
        }

        public TagInfoDataModel CreateTagInfo(string mainText, string subText, int tagId)
        {
            // calc id
            var infos = _repository.GetAllTagInfos();
            var nextFreeId = EntitiesHelper.CalcNextId<TagInfoDataModel>(infos, (t => t.Info.Id));

            // create info
            var info = new TagInfo(nextFreeId, mainText, subText);

            // create TagDataModel wrapper for tag
            var newInfo = new TagInfoDataModel(info)
            {
                TagId = tagId,
                Info = info
            };
            return newInfo;
        }

        public InfoDataModel CreateTutorialInfo(string mainText, string subText)
        {
            // calc id
            var infos       = _repository.GetAllTutorialInfos();
            var nextFreeId  = EntitiesHelper.CalcNextId<InfoDataModel>(infos, (t => t.Info.Id));

            // create info
            var info = new TutorialInfo(nextFreeId, mainText, subText);

            // create TagDataModel wrapper for tag
            var newInfo = new InfoDataModel(info)
            {
                Info = info
            };
            return newInfo;
        }

    }
}

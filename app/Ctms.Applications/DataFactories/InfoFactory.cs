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

        public InfoDataModel CreateInfoDm(string mainText, string subText, InfoTypes type)
        {
            // calc id
            var infos = _repository.GetAllInfos(type);
            var nextFreeId = EntitiesHelper.CalcNextId<InfoDataModel>(infos, (t => t.Info.Id));

            // create info
            Info info = null;
            if (type == InfoTypes.CommonInfo) info = new CommonInfo(nextFreeId, mainText, subText);
            else if (type == InfoTypes.TagInfo) info = new TagInfo(nextFreeId, mainText, subText);
            else if (type == InfoTypes.TutorialInfo) info = new TutorialInfo(nextFreeId, mainText, subText);

            // create TagDataModel wrapper for tag
            var newInfo = new InfoDataModel(info)
            {
                Info = info
            };
            return newInfo;
        }

    }
}

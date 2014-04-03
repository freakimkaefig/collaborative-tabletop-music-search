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


        private InfoDataModel CreateInfoDm(Info info)
        {
            // create TagDataModel wrapper for tag
            var newInfo = new InfoDataModel(info)
            {
                Info = info
            };
            return newInfo;
        }

        public InfoDataModel CreateCommonInfoDm(string mainText, string subText)
        {
            var infos = _repository.GetAllCommonInfos();
            var nextFreeId = EntitiesHelper.CalcNextId<InfoDataModel>(infos, (t => t.Info.Id));

            // create Tag
            var info = new CommonInfo(mainText, subText)
            {
                Id = nextFreeId,
                //TagOptions = new ObservableCollection<TagOption>()
            };

            return CreateInfoDm(info);
        }

    }
}

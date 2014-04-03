using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Ctms.Applications.ViewModels;
using System.Waf.Applications.Services;
using Ctms.Applications.Data;
using Ctms.Applications.DataFactories;
using System.Windows;
using Ctms.Domain.Objects;

namespace Ctms.Applications.Workers
{
    [Export]
    public class InfoWorker
    {
        private InfoViewModel _infoVm;
        private ShellViewModel _shellVm;
        private IMessageService _messageService;
        private Repository _repository;
        private InfoFactory _infoFactory;

        [ImportingConstructor]
        public InfoWorker(InfoViewModel infoVm, IMessageService messageService, ShellViewModel shellVm,
            Repository repository)
        {
            //ViewModels
            _infoVm = infoVm;
            _shellVm = shellVm;
            //Services
            _messageService = messageService;
            //Data
            _repository = repository;
            //Workers
            //Other vars
            _infoFactory = new InfoFactory(_repository);
        }

        public void Initialize()
        {
            /*
            foreach (var tagDM in tagDMs)
            {
                LoadKeywordTypes(tagDM.Id);
            }*/


            //ShowCommonInfo("CommonInfoMain", "InfoSub");
        }

        public void ConfirmCommonInfo(int infoId)
        {
            //_repository.
        }

        public void ConfirmTagInfo(int infoId)
        {
            //_repository.
        }

        public void ConfirmTutorialInfo(int infoId)
        {
            //_repository.
        }

        public void ShowCommonInfo(string mainText, string subText)
        {
            var info = _infoFactory.CreateInfoDm(mainText, subText, InfoTypes.CommonInfo);
            info.IsVisible = true;
            info.Info.MainText = mainText;
            info.Info.SubText = subText;
            _infoVm.CommonInfos.Add(info);
        }

        public void ShowTagInfo(string mainText, string subText, int tagId)
        {
            var info = _infoFactory.CreateInfoDm(mainText, subText, InfoTypes.TagInfo);
            info.IsVisible = true;
            info.Info.MainText  = mainText;
            info.Info.SubText   = subText;

            var tagDm = _repository.GetTagDMById(tagId);
            var infoHeight = 50.0F;//!!read->set globally
            info.Info.PositionX = tagDm.Tag.PositionX;
            if (_shellVm.WindowHeight < tagDm.Tag.PositionY + tagDm.Height + infoHeight)
            {
                info.Info.PositionY = tagDm.Tag.PositionY - infoHeight;
            }
            else
	        {
                info.Info.PositionY = tagDm.Tag.PositionY + tagDm.Height;
	        }
            _infoVm.TagInfos.Add(info);
        }

        public void ShowTutorialInfo(string mainText, string subText)
        {
            var info = _infoFactory.CreateInfoDm(mainText, subText, InfoTypes.TutorialInfo);
            info.IsVisible = true;
            _infoVm.TutorialInfos.Add(info);
            info.Info.MainText = mainText;
            info.Info.SubText = subText;
            //info.Info.PositionX = 
            //info.Info.PositionY = 
            _infoVm.CommonInfos.Add(info);
        }

    }
}

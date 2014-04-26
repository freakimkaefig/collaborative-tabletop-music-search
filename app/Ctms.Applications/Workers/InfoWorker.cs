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
using Ctms.Applications.DataModels;

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
        }

        public void ConfirmCommonInfo(int infoId)
        {
            _repository.GetAllInfos(InfoTypes.CommonInfo).Remove(_repository.GetCommonInfoById(infoId));
        }

        public void ConfirmTagInfo(int infoId)
        {
            _repository.GetAllInfos(InfoTypes.TagInfo).Remove(_repository.GetTagInfoById(infoId));
        }

        public void ConfirmTutorialInfo(int infoId)
        {
            _repository.GetAllInfos(InfoTypes.TutorialInfo).Remove(_repository.GetTutorialInfoById(infoId));
        }

        public InfoDataModel CreateInfo(string mainText, string subText, InfoTypes infotype)
        {
            var info = _infoFactory.CreateInfoDm(mainText, subText, infotype);
            info.IsVisible = true;
            info.Info.MainText = mainText;
            info.Info.SubText = subText;

            return info;
        }

        public int ShowCommonInfo(string mainText, string subText, string confirmText = null)
        {
            var info = CreateInfo(mainText, subText, InfoTypes.CommonInfo);

            CalcButtons(confirmText, info);
            _infoVm.CommonInfos.Add(info);

            return info.Info.Id;
        }

        private static void CalcButtons(string confirmText, InfoDataModel info)
        {
            if (confirmText != null)
            {
                info.IsConfirmable = true;
                info.ConfirmText = confirmText;
            }
        }

        public void ShowTagInfo(string mainText, string subText, int tagId, string confirmText = null)
        {
            var info = CreateInfo(mainText, subText, InfoTypes.TagInfo);

            var tagDm = _repository.GetTagDMById(tagId);
            //!!better to position info the side?
            var infoHeight = 80.0F;//!!read->set globally
            info.Info.PositionX = tagDm.Tag.PositionX;
            if (_shellVm.WindowHeight < tagDm.Tag.PositionY + tagDm.Height + infoHeight)
            {
                info.Info.PositionY = tagDm.Tag.PositionY - infoHeight;
            }
            else
	        {
                info.Info.PositionY = tagDm.Tag.PositionY + tagDm.Height;
	        }
            _repository.RemoveTagInfoById(tagId);
            _infoVm.TagInfos.Add(info);
        }

        public void ShowTutorialInfo(string mainText, string subText)
        {
            var info = CreateInfo(mainText, subText, InfoTypes.TutorialInfo);

            _infoVm.TutorialInfos.Add(info);
        }

    }
}

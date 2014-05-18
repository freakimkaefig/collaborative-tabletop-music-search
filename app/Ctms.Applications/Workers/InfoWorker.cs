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

        /// <summary>
        /// Confirm and close info
        /// </summary>
        /// <param name="commonInfoId"></param>
        public void ConfirmCommonInfo(int commonInfoId)
        {
            var info = _repository.GetCommonInfoById(commonInfoId);

            if (info != null && info.ConfirmAction != null && info.ConfirmParameters != null)
                info.ConfirmAction.Invoke(info.ConfirmParameters);

            _repository.RemoveCommonInfoById(commonInfoId);
        }

        /// <summary>
        /// Confirm and close info
        /// </summary>
        /// <param name="commonInfoId"></param>
        public void ConfirmTagInfo(int tagId)
        {
            _repository.RemoveTagInfoById(tagId);
        }

        /// <summary>
        /// Confirm and close info
        /// </summary>
        /// <param name="commonInfoId"></param>
        public void ConfirmTutorialInfo(int infoId)
        {
            _repository.RemoveTutorialInfoById(infoId);
        }

        /// <summary>
        /// Cancel and close info
        /// </summary>
        /// <param name="commonInfoId"></param>
        public void CancelCommonInfo(int commonInfoId)
        {
            var info = _repository.GetCommonInfoById(commonInfoId);

            if (info != null && info.CancelAction != null) 
                info.CancelAction.Invoke(info.CancelParameters);

            _repository.RemoveCommonInfoById(commonInfoId);
        }

        /// <summary>
        /// Cancel and close info
        /// </summary>
        /// <param name="commonInfoId"></param>
        public void CancelTutorialInfo(int commonInfoId)
        {
            var info = _repository.GetTutorialInfoById(commonInfoId);

            if (info != null && info.CancelAction != null && info.CancelParameters != null)
                info.CancelAction.Invoke(info.CancelParameters);

            _repository.RemoveTutorialInfoById(commonInfoId);
        }

        /// <summary>
        /// Show common info (shown to both sides of the table)
        /// </summary>
        /// <param name="mainText">The header</param>
        /// <param name="subText">The subtext, normally longer</param>
        /// <param name="confirmText">Text for confirm button</param>
        /// <param name="cancelText">Text for cancel button</param>
        /// <param name="isLoading">Shall loading gif be shown</param>
        /// <param name="confirmAction">Which action shall be called when confirm is pressed</param>
        /// <param name="cancelAction">Which action shall be called when cancel is pressed</param>
        /// <returns>Id of info</returns>
        public int ShowCommonInfo(string mainText, string subText, string confirmText = null, string cancelText = null, bool isLoading = false,
            Action<object> confirmAction = null, Action<object> cancelAction = null)
        {
            var info = _infoFactory.CreateCommonInfo(mainText, subText);

            info.IsVisible = true;
            info.Info.MainText = mainText;
            info.Info.SubText = subText;
            info.IsLoadingVisible = isLoading;
            info.ConfirmAction  = confirmAction;
            info.CancelAction   = cancelAction;

            CalcButtons(confirmText, cancelText, info);
            _infoVm.CommonInfos.Add(info);

            return info.Info.Id;
        }

        /// <summary>
        /// Calculate if confirm and cancel buttons shall be visible
        /// </summary>
        /// <param name="confirmText"></param>
        /// <param name="cancelText"></param>
        /// <param name="info"></param>
        private static void CalcButtons(string confirmText, string cancelText, InfoDataModel info)
        {
            if (confirmText != null)
            {
                info.IsConfirmable = true;
                info.ConfirmText = confirmText;
            }
            if (cancelText != null)
            {
                info.IsCancellable = true;
                info.CancelText = cancelText;
            }
        }

        /// <summary>
        /// Show info next to a tag
        /// </summary>
        /// <param name="mainText">Header</param>
        /// <param name="subText">Sub text, normally longer</param>
        /// <param name="tagId">Id of tag</param>
        /// <param name="confirmText">text for confirm button</param>
        public void ShowTagInfo(string mainText, string subText, int tagId, string confirmText = null)
        {
            var info = _infoFactory.CreateTagInfo(mainText, subText, tagId);

            var tagDm = _repository.GetTagDMById(tagId);
            var tagPosY = tagDm.Tag.PositionY;
            var tagPosX = tagDm.Tag.PositionX;

            var infoHeight = 40.0F;

            // tag is rotated about 180°
            if (tagDm.Tag.Orientation >= 180)
            {
                var infoPadding = 30.0F;
                info.Info.PositionX = tagPosX - tagDm.Width / 2.0F;

                if (0 < tagPosY - tagDm.Height / 2.0 - infoHeight + infoPadding)
                {
                    // place the info above the tag because below there's no space
                    info.Info.PositionY = tagPosY - tagDm.Height / 2.0F + infoHeight - 15.0F;
                }
                else
                {
                    // place the info below the tag because there's space
                    info.Info.PositionY = tagPosY + tagDm.Height / 2.0F + infoPadding - 15.0F;
                }
            }
            else // tag is not rotated
            {   
                info.Info.PositionX = tagPosX - tagDm.Width / 2.0F;

                if (_shellVm.WindowHeight > tagPosY + tagDm.Height / 2.0 + infoHeight - 60.0F)
                {   // place the info below the tag because there's space
                    info.Info.PositionY = tagPosY + tagDm.Height / 2.0F - 60.0F;
                }
                else
                {   // place the info above the tag because below there's no space
                    info.Info.PositionY = tagPosY - tagDm.Height / 2.0F - 30.0F;
                }
            }
            RemoveTagInfo(tagId);
            _infoVm.TagInfos.Add(info);
        }

        public void RemoveTagInfo(int tagId)
        {
            _repository.RemoveTagInfoById(tagId);
        }

        public void ShowTutorialInfo(string mainText, string subText)
        {
            var info = _infoFactory.CreateTutorialInfo(mainText, subText);

            _infoVm.TutorialInfos.Add(info);
        }

        public void RemoveTutorialInfo(int infoId)
        {
            _repository.RemoveTutorialInfoById(infoId);
        }

    }
}

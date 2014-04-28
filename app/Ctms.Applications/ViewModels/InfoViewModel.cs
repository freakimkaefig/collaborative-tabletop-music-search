using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Ctms.Applications.Views;
using Ctms.Domain.Objects;
using System.Collections.ObjectModel;
using Ctms.Applications.DataModels;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class InfoViewModel : ViewModel<IInfoView>
    {
        private bool isValid = true;
        private ObservableCollection<InfoDataModel> _commonInfos;
        private ObservableCollection<TagInfoDataModel> _tagInfos;
        private ObservableCollection<InfoDataModel> _tutorialInfos;
        private ICommand _confirmCommonInfoCmd;
        private ICommand _confirmTagInfoCmd;
        private ICommand _confirmTutorialInfoCmd;
        
        [ImportingConstructor]
        public InfoViewModel(IInfoView view)
            : base(view)
        {
            _commonInfos = new ObservableCollection<InfoDataModel>();
            _tagInfos = new ObservableCollection<TagInfoDataModel>();
            _tutorialInfos = new ObservableCollection<InfoDataModel>();
        }


        public bool IsEnabled { get { return true; } }//Detail != null;//!! Has to be adjusted

        public bool IsValid
        {
            get { return isValid; }
            set
            {
                if (isValid != value)
                {
                    isValid = value;
                    RaisePropertyChanged("IsValid");
                }
            }
        }

        public ObservableCollection<InfoDataModel> CommonInfos { get { return _commonInfos; } }

        public ObservableCollection<TagInfoDataModel> TagInfos { get { return _tagInfos; } }

        public ObservableCollection<InfoDataModel> TutorialInfos { get { return _tutorialInfos; } }

        public ICommand ConfirmCommonInfoCmd
        {
            get { return _confirmCommonInfoCmd; }
            set
            {
                if (_confirmCommonInfoCmd != value)
                {
                    _confirmCommonInfoCmd = value;
                    RaisePropertyChanged("ConfirmCommonInfoCmd");
                }
            }
        }

        public ICommand ConfirmTagInfoCmd
        {
            get { return _confirmTagInfoCmd; }
            set
            {
                if (_confirmTagInfoCmd != value)
                {
                    _confirmTagInfoCmd = value;
                    RaisePropertyChanged("ConfirmTagInfoCmd");
                }
            }
        }

        public ICommand ConfirmTutorialInfoCmd
        {
            get { return _confirmTutorialInfoCmd; }
            set
            {
                if (_confirmTutorialInfoCmd != value)
                {
                    _confirmTutorialInfoCmd = value;
                    RaisePropertyChanged("ConfirmTutorialInfoCmd");
                }
            }
        }
    }
}

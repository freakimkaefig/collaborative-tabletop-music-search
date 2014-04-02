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
        private ObservableCollection<InfoDataModel> _infos;
        
        [ImportingConstructor]
        public InfoViewModel(IInfoView view)
            : base(view)
        {
            _infos = new ObservableCollection<InfoDataModel>();
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

        public ObservableCollection<InfoDataModel> Infos { get { return _infos; } }

        //public Detail Detail
        //{
        //    get { return detail; }
        //    set
        //    {
        //        if (detail != value)
        //        {
        //            detail = value;
        //            RaisePropertyChanged("Detail");
        //            RaisePropertyChanged("IsEnabled");
        //        }
        //    }
        //}
    }
}

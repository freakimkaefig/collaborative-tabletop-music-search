using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Ctms.Applications.Views;
using Ctms.Domain.Objects;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class DetailViewModel : ViewModel<IDetailView>
    {
        private bool isValid = true;
        private Detail detail;
        private ICommand doTestCommand;

        
        [ImportingConstructor]
        public DetailViewModel(IDetailView view)
            : base(view)
        {
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

        public Detail Detail
        {
            get { return detail; }
            set
            {
                if (detail != value)
                {
                    detail = value;
                    RaisePropertyChanged("Detail");
                    RaisePropertyChanged("IsEnabled");
                }
            }
        }

        public ICommand DoTestCommand
        {
            get { return doTestCommand; }
            set
            {
                if (doTestCommand != value)
                {
                    doTestCommand = value;
                    RaisePropertyChanged("DoTestCommand");
                }
            }
        }
    }
}

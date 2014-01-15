using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Ctms.Domain.Objects;
using System.Windows.Input;
using Ctms.Applications.Views;
using System.Waf.Applications;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class TagVisualizationViewModel : ViewModel<ITagVisualizationView>
    {
        private bool isValid = true;
        private Detail detail;
        private ICommand selectCommand;

        
        [ImportingConstructor]
        public TagVisualizationViewModel(ITagVisualizationView view)
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

        private void RaisePropertyChanged(string p)
        {
            throw new NotImplementedException();
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

        public ICommand SelectCommand
        {
            get { return selectCommand; }
            set
            {
                if (selectCommand != value)
                {
                    selectCommand = value;
                    RaisePropertyChanged("SelectCommand");
                }
            }
        }
    }
}

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
    public class SearchTagViewModel : ViewModel<ISearchTagView>
    {
        private bool isValid = true;
        private Detail detail;
        private ICommand selectOptionCmd;
        private string item1Header;
        
        [ImportingConstructor]
        public SearchTagViewModel(ISearchTagView view)
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

        public string Breadcrumb { get {return "Breadcrumb";}  }

        /*
        public string Item1Header
        {
            get {
                if (item1Header == null) return "My dynamic song";
                else { return item1Header; };
            }
            set
            {
                if (item1Header != value)
                {
                    item1Header = value;
                    RaisePropertyChanged("Item1Header");
                }
            }
        }
        */

        public ICommand SelectOptionCmd
        {
            get { return selectOptionCmd; }
            set
            {
                if (selectOptionCmd != value)
                {
                    selectOptionCmd = value;
                    RaisePropertyChanged("SelectOptionCmd");
                }
            }
        }

        public ISearchTagView MyView { get; set; }

        public void DoSth()
        {
            MyView.DoSth();
        }
    }
}

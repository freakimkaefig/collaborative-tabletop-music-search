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
        private bool _isValid = true;
        private Detail _detail;
        private ICommand _selectOptionCmd;
        private string _item1Header;
        private int _id;
        private string _keyword;
        private List<Tag> _tags;
        
        [ImportingConstructor]
        public SearchTagViewModel(ISearchTagView view)
            : base(view)
        {
            _tags = new List<Tag>();
        }

        public bool IsEnabled { get { return true; } }//Detail != null;//!! Has to be adjusted

        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    RaisePropertyChanged("IsValid");
                }
            }
        }

        public string Breadcrumb { get { return "Breadcrumb2"; } }

        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

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
            get { return _selectOptionCmd; }
            set
            {
                if (_selectOptionCmd != value)
                {
                    _selectOptionCmd = value;
                    RaisePropertyChanged("SelectOptionCmd");
                }
            }
        }

        public ISearchTagView MyView { get; set; }

        public List<Tag> Tags
        {
            get { return _tags; }
            set
            {
                if (_tags != value)
                {
                    _tags = value;
                    RaisePropertyChanged("Tags");
                }
            }
        }

        public string Keyword
        {
            get { return "MyKeyword"; }
            set
            {
                if (_keyword != value)
                {
                    _keyword = value;
                    RaisePropertyChanged("Keyword");
                }
            }
        }

        public void DoSth()
        {
            MyView.DoSth();
        }
    }
}

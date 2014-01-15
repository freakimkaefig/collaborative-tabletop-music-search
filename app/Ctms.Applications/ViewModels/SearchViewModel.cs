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
    public class SearchViewModel : ViewModel<ISearchView>
    {
        private bool _isValid = true;
        private Search _search;
        private string _keywordType;
        private ICommand _startSearchCommand;
        private ICommand selectOptionCmd;


        [ImportingConstructor]
        public SearchViewModel(ISearchView view)
            : base(view)
        {
        }


        public bool IsEnabled { get { return true; } }

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

        public Search Search
        {
            get { return _search; }
            set
            {
                if (_search != value)
                {
                    _search = value;
                    RaisePropertyChanged("Search");
                }
            }
        }

        public string KeywordType
        {
            get { return _keywordType; }
            set
            {
                if (_keywordType != value)
                {
                    _keywordType = value;
                    RaisePropertyChanged("KeywordType");
                }
            }
        }

        public ICommand StartSearchCommand
        {
            get { return _startSearchCommand; }
            set
            {
                if (_startSearchCommand != value)
                {
                    _startSearchCommand = value;
                    RaisePropertyChanged("StartSearchCommand");
                }
            }
        }

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
    }
}

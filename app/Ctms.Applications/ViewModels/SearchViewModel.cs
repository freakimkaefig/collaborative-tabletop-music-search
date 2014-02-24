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
        private bool        _isValid = true;
        private Search      _search;
        private string      _keywordType;
        private ICommand    _startSearchCommand;
        private ICommand    _selectOptionCmd;
        private string      _inputValue;
        private string      _item1Header;
        private string      _item2Header;
        private int         _mainItemId;


        [ImportingConstructor]
        public SearchViewModel(ISearchView view)
            : base(view)
        {
            _mainItemId = 666;
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

        public string InputValue
        {
            get { return _inputValue; }
            set
            {
                if (_inputValue != value)
                {
                    _inputValue = value;
                    RaisePropertyChanged("InputValue");
                }
            }
        }

        public ICommand StartSearchCmd
        {
            get { return _startSearchCommand; }
            set
            {
                if (_startSearchCommand != value)
                {
                    _startSearchCommand = value;
                    RaisePropertyChanged("StartSearchCmd");
                }
            }
        }

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

        public string Item1Header
        {
            get
            {
                if (_item1Header == null) return "My dynamic song";
                else { return _item1Header; };
            }
            set
            {
                if (_item1Header != value)
                {
                    _item1Header = value;
                    RaisePropertyChanged("Item1Header");
                }
            }
        }

        public string Item2Header
        {
            get
            {
                if (_item2Header == null) return "My dynamic song2";
                else { return _item2Header; };
            }
            set
            {
                if (_item2Header != value)
                {
                    _item2Header = value;
                    RaisePropertyChanged("Item2Header");
                 }
            }
        }

        public int MainItemId
        {
            get { return _mainItemId; }
            set
            {
                if (_mainItemId != value)
                {
                    _mainItemId = value;
                    RaisePropertyChanged("MainItemId");
                }
            }
        }
    }
}

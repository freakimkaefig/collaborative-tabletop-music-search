using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Ctms.Applications.Views;
using Ctms.Domain.Objects;
using Microsoft.Surface.Presentation.Controls;
using System.Collections.ObjectModel;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class SearchViewModel : ViewModel<ISearchView>
    {
        private bool        _isValid = true;
        private string      _keywordType;
        private ICommand    _startSearchCommand;
        private ICommand    _selectOptionCmd;
        private string      _inputValue;
        private string      _item1Header;
        private string      _item2Header;
        private int         _mainItemId;
        //private readonly IEnumerable<SearchTagViewModel> _searchTags;


        [ImportingConstructor]
        public SearchViewModel(ISearchView view)
            : base(view)
        {
            _mainItemId = 666;
            Tags = new List<Tag>();
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
        /*
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
        */
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

        public List<Tag> Tags { get; set; }

        public void OnVisualizationAdded(TagVisualization tagVisualization)
        {
            var tagValue    = tagVisualization.VisualizedTag.Value;
            var tagVizual   = tagVisualization.VisualizedTag;
            var simpleTag   = tagVisualization;
        }
        /*
        public List<SearchTagViewModel> SearchTags
        {
            get { return _searchTags; }
            set
            {
                if (_searchTags != value)
                {
                    _searchTags = value;
                    RaisePropertyChanged("SearchTags");
                }
            }
        }
        */
        //public IEnumerable<SearchTagViewModel> SearchTags { get { return _searchTags; } }

    }
}

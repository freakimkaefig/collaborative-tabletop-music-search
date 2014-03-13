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
using Ctms.Applications.DataModels;
using System.Threading;
using System.Windows.Data;
using Ctms.Domain;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class SearchViewModel : ViewModel<ISearchView>
    {
        private bool        _isValid = true;
        private string      _keywordType;
        private ICommand    _startSearchCommand;
        private ICommand    _selectOptionCmd;
        private ICommand    _getSuggestionsCmd;
        private string      _inputValue;
        private string      _item1Header;
        private string      _item2Header;
        private int         _mainItemId;
        private List<TagVisualization> _tagVisualizations;
        private ISearchView _searchView;
        private Keyword _assignedKeyword;
        private ICommand _selectCircleOptionCmd;
        private ObservableCollection<TagDataModel> _tags;
        private ICommand _goHomeCmd;
        private ICommand    _goBreadcrumbCmd;
        private bool        _addedVisualization;
        //private readonly IEnumerable<SearchTagViewModel> _searchTags;

        [ImportingConstructor]
        public SearchViewModel(ISearchView view)
            : base(view)
        {
            _tags = new ObservableCollection<TagDataModel>();
            _searchView = view;
        }

        public SearchViewModel() : base(null)
        {

        }

        public void ShowKeyword(TagOption tagOption)
        {
            //ListVisibility = ...
            //KeywordVisibility = ...
            AssignedKeyword = tagOption.Keyword;
        }

        #region Properties

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

        public Keyword AssignedKeyword
        {
            get { return _assignedKeyword; }
            set
            {
                if (_assignedKeyword != value)
                {
                    _assignedKeyword = value;
                    RaisePropertyChanged("AssignedKeyword");
                }
            }
        }

        public ObservableCollection<TagDataModel> Tags
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

        public bool AddedVisualization
        {
            get { return _addedVisualization; }
            set
            {
                if (_addedVisualization != value)
                {
                    _addedVisualization = value;
                    RaisePropertyChanged("AddedVisualization");
                }
            }
        }



        #endregion


        #region Commands

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

        public ICommand GoHomeCmd
        {
            get { return _goHomeCmd; }
            set
            {
                if (_goHomeCmd != value)
                {
                    _goHomeCmd = value;

                    RaisePropertyChanged("GoHomeCmd");
                }
            }
        }

        public ICommand GoBreadcrumbCmd
        {
            get { return _goBreadcrumbCmd; }
            set
            {
                if (_goBreadcrumbCmd != value)
                {
                    _goBreadcrumbCmd = value;

                    RaisePropertyChanged("GoBreadcrumbCmd");
                }
            }
        }


        public ICommand SelectCircleOptionCmd
        {
            get { return _selectCircleOptionCmd; }
            set
            {
                if (_selectCircleOptionCmd != value)
                {
                    _selectCircleOptionCmd = value;

                    RaisePropertyChanged("SelectCircleOptionCmd");
                }
            }
        }
        

        public ICommand GetSuggestionsCmd
        {
            get { return _getSuggestionsCmd; }
            set
            {
                if (_getSuggestionsCmd != value)
                {
                    _getSuggestionsCmd = value;
                    RaisePropertyChanged("GetSuggestionsCmd");
                }
            }
        }

        #endregion Commands  

        public void OnVisualizationAdded(TagVisualization tagVisualization)
        {
            /*
            var tagValue    = tagVisualization.VisualizedTag.Value;
            var tagVizual   = tagVisualization.VisualizedTag;
            var simpleTag   = tagVisualization;
            */
            //TagVisualizations.Add(tagVisualization);
            /*
            var tagVisualization    = (SearchTagView) e.TagVisualization;
            var tagId               = (int) tagVisualization.VisualizedTag.Value;
            /*
            var pieMenu             = ((PieMenu) tagVisualization.PieMenu1);//!!
            var pieMenuItems        = (ItemCollection) pieMenu.Items;*/

            //AddedVisualization = true;
        }

        public void UpdateVisuals(TagDataModel tagDM)
        {
            _searchView.UpdateVisual(tagDM.Id);
        }
    }
}

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
using MusicSearch.SearchObjects;
using MusicSearch.Managers;

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
        private List<Entry> _entries;
        private ObservableCollection<TagOption>   tagOptions;
        private List<TagVisualization> _tagVisualizations;

        private List<searchObjects> _searchObjectsList; //Liste an SearchObjects (Michl, die musst du dann befüllen!)
        
        //private readonly IEnumerable<SearchTagViewModel> _searchTags;


        [ImportingConstructor]
        public SearchViewModel(ISearchView view)
            : base(view)
        {
            tagOptions = new ObservableCollection<TagOption>();
            TagVisualizations = new List<TagVisualization>();

            _entries = new List<Entry>()
            {
                new Entry { Id = 0, Header = "Hypocratics", SubHeader = "Audioslave" },
                new Entry { Id = 1, Header = "Hypocratics", SubHeader = "Katy Perry" },
                new Entry { Id = 2, Header = "Hypocratics", SubHeader = "Anyone" },
                new Entry { Id = 3, Header = "Hypocratics", SubHeader = "John Wayne" },
                new Entry { Id = 4, Header = "Hypocratics", SubHeader = "Justin Bieber" }
            };

            //Testweise hinzufügen eines SearchObjects
                //MICHL: befüllen durch Tangibles die auf dem Tisch stehen!
                //       Einträge auch wieder löschen, wenn Tangible vom Tisch genommen wird (über originId)
            _searchObjectsList = new List<searchObjects>();
            _searchObjectsList.Add(new searchObjects
            {
                genre = "rock",
                originId = 1

            });
        }

        public List<searchObjects> SearchObjectsList
        {
            get { return _searchObjectsList; }
            set
            {
                if (_searchObjectsList != value)
                {
                    _searchObjectsList = value;
                    RaisePropertyChanged("SearchObjectsList");
                }
            }
        }

        public string Breadcrumb { get { return "SVM"; } }
        public string Breadcrumb2 { get { return "SVM2"; } }

        public List<Entry> Entries
        {
            get { return _entries; } 
        }

        public class Entry {
            public int      Id { get; set; }
            public string   Header { get; set; }
            public string   SubHeader { get; set; }
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

        public List<TagVisualization> TagVisualizations
        {
            get { return _tagVisualizations; }
            set
            {
                if (_tagVisualizations != value)
                {
                    _tagVisualizations = value;
                    RaisePropertyChanged("TagVisualizations");
                }
            }
        }

        public ObservableCollection<TagOption> TagOptions
        {
            get { return tagOptions; }
            set
            {
                if (tagOptions != value)
                {
                    tagOptions = value;
                    RaisePropertyChanged("TagOptions");
                }
            }
        }

        public void OnVisualizationAdded(TagVisualization tagVisualization)
        {
            //var tagValue    = tagVisualization.VisualizedTag.Value;
            //var tagVizual   = tagVisualization.VisualizedTag;
            //var simpleTag   = tagVisualization;

            TagVisualizations.Add(tagVisualization);
            /*
            var tagVisualization    = (SearchTagView) e.TagVisualization;
            var tagId               = (int) tagVisualization.VisualizedTag.Value;
            /*
            var pieMenu             = ((PieMenu) tagVisualization.PieMenu1);//!!
            var pieMenuItems        = (ItemCollection) pieMenu.Items;*/
        }

        public void OnVisualizationRemoved(TagVisualization tagVisualization)
        {
            TagVisualizations.Remove(tagVisualization);
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

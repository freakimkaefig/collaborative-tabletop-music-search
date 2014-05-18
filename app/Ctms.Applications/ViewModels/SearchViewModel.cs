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
using MusicSearch.Managers;
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
        private ICommand    _startSearchCmd;
        private ICommand    _selectOptionCmd;
        private ICommand    _confirmInputCmd;
        private ISearchView _searchView;
        private Keyword _assignedKeyword;
        private ObservableCollection<TagDataModel> _tags;
        private ICommand _goHomeCmd;
        private ICommand    _goBreadcrumbCmd;
        private bool        _addedVisualization;
        private ICommand _editCmd;
        private ICommand _fftData;
        private List<searchObject> _searchObjectsList;
        //private readonly IEnumerable<SearchTagViewModel> _searchTags;

        //FFT Values
        private int _fft1Value = 0;
        private int _fft2Value = 0;
        private int _fft3Value = 0;
        private int _fft4Value = 0;
        private int _fft5Value = 0;
        private int _fft6Value = 0;
        private int _fft7Value = 0;
        private int _fft8Value = 0;
        private int _fft9Value = 0;
        private int _fft10Value = 0;
        private int _fft11Value = 0;
        private int _fft12Value = 0;
        private int _fft13Value = 0;
        private int _fft14Value = 0;
        private int _fft15Value = 0;

        private List<System.Windows.Shapes.Rectangle> _fftRectangle;
        private ICommand _selectAttributeCmd;
        private ICommand _selectArtistCmd;
        private ICommand _selectSongCmd;
        private ICommand _selectGenreCmd;
        private string _searchViewLog;

        private int _logCount;
        private ICommand _checkTagPositionsCmd;
        private ObservableCollection<TagCombinationDataModel> _tagCombinations;
        private ICommand _lowerInputCmd;
        private ICommand _raiseInputCmd;
        private ICommand _removeTagFromCombi;

        [ImportingConstructor]
        public SearchViewModel(ISearchView view)
            : base(view)
        {
            _tags = new ObservableCollection<TagDataModel>();
            _searchView = view;
            _searchView.InitializeRectangles();

            _searchObjectsList = new List<searchObject>();
            _tagCombinations = new ObservableCollection<TagCombinationDataModel>();
        }

        public List<searchObject> SearchObjectsList
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

        public SearchViewModel() : base(null)
        {

        }

        #region FFTValues

        public List<System.Windows.Shapes.Rectangle> FftRectangle
        {
            get { return _fftRectangle; }
            set
            {
                if (_fftRectangle != value)
                {
                    _fftRectangle = value;
                    RaisePropertyChanged("FftRectangle");
                }
            }
        }
        public int Fft1Value
        {
            get { return _fft1Value; }
            set
            {
                if (_fft1Value != value)
                {
                    _fft1Value = value;
                    RaisePropertyChanged("Fft1Value");
                }
            }
        }
        public int Fft2Value
        {
            get { return _fft2Value; }
            set
            {
                if (_fft2Value != value)
                {
                    _fft2Value = value;
                    RaisePropertyChanged("Fft2Value");
                }
            }
        }
        public int Fft3Value
        {
            get { return _fft3Value; }
            set
            {
                if (_fft3Value != value)
                {
                    _fft3Value = value;
                    RaisePropertyChanged("Fft3Value");
                }
            }
        }
        public int Fft4Value
        {
            get { return _fft4Value; }
            set
            {
                if (_fft4Value != value)
                {
                    _fft4Value = value;
                    RaisePropertyChanged("Fft4Value");
                }
            }
        }
        public int Fft5Value
        {
            get { return _fft5Value; }
            set
            {
                if (_fft5Value != value)
                {
                    _fft5Value = value;
                    RaisePropertyChanged("Fft5Value");
                }
            }
        }
        public int Fft6Value
        {
            get { return _fft6Value; }
            set
            {
                if (_fft6Value != value)
                {
                    _fft6Value = value;
                    RaisePropertyChanged("Fft6Value");
                }
            }
        }
        public int Fft7Value
        {
            get { return _fft7Value; }
            set
            {
                if (_fft7Value != value)
                {
                    _fft7Value = value;
                    RaisePropertyChanged("Fft7Value");
                }
            }
        }
        public int Fft8Value
        {
            get { return _fft8Value; }
            set
            {
                if (_fft8Value != value)
                {
                    _fft8Value = value;
                    RaisePropertyChanged("Fft8Value");
                }
            }
        }
        public int Fft9Value
        {
            get { return _fft9Value; }
            set
            {
                if (_fft9Value != value)
                {
                    _fft9Value = value;
                    RaisePropertyChanged("Fft9Value");
                }
            }
        }
        public int Fft10Value
        {
            get { return _fft10Value; }
            set
            {
                if (_fft10Value != value)
                {
                    _fft10Value = value;
                    RaisePropertyChanged("Fft10Value");
                }
            }
        }
        public int Fft11Value
        {
            get { return _fft11Value; }
            set
            {
                if (_fft11Value != value)
                {
                    _fft11Value = value;
                    RaisePropertyChanged("Fft11Value");
                }
            }
        }
        public int Fft12Value
        {
            get { return _fft12Value; }
            set
            {
                if (_fft12Value != value)
                {
                    _fft12Value = value;
                    RaisePropertyChanged("Fft12Value");
                }
            }
        }
        public int Fft13Value
        {
            get { return _fft13Value; }
            set
            {
                if (_fft13Value != value)
                {
                    _fft13Value = value;
                    RaisePropertyChanged("Fft13Value");
                }
            }
        }
        public int Fft14Value
        {
            get { return _fft14Value; }
            set
            {
                if (_fft14Value != value)
                {
                    _fft14Value = value;
                    RaisePropertyChanged("Fft14Value");
                }
            }
        }
        public int Fft15Value
        {
            get { return _fft15Value; }
            set
            {
                if (_fft15Value != value)
                {
                    _fft15Value = value;
                    RaisePropertyChanged("Fft15Value");
                }
            }
        }

        #endregion


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

        public ObservableCollection<TagCombinationDataModel> TagCombinations
        {
            get { return _tagCombinations; }
            set
            {
                if (_tagCombinations != value)
                {
                    _tagCombinations = value;
                    RaisePropertyChanged("TagCombinations");
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

        public string SearchViewLog
        {
            get { return _searchViewLog; }
            set
            {
                if (_searchViewLog != value)
                {
                    _searchViewLog = value;
                    RaisePropertyChanged("SearchViewLog");
                }
            }
        }

        public void AddLog(string logMessage)
        {
            _logCount++;
            SearchViewLog += _logCount + logMessage + Environment.NewLine;
        }



        #endregion


        #region Commands

        public ICommand CheckTagPositionsCmd
        {
            get { return _checkTagPositionsCmd; }
            set
            {
                if (_checkTagPositionsCmd != value)
                {
                    _checkTagPositionsCmd = value;
                    RaisePropertyChanged("CheckTagPositionsCmd");
                }
            }
        }

        public ICommand RemoveTagFromCombi
        {
            get { return _removeTagFromCombi; }
            set
            {
                if (_removeTagFromCombi != value)
                {
                    _removeTagFromCombi = value;
                    RaisePropertyChanged("RemoveTagFromCombi");
                }
            }
        }

        public ICommand StartSearchCmd
        {
            get { return _startSearchCmd; }
            set
            {
                if (_startSearchCmd != value)
                {
                    _startSearchCmd = value;
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
        

        public ICommand ConfirmInputCmd
        {
            get { return _confirmInputCmd; }
            set
            {
                if (_confirmInputCmd != value)
                {
                    _confirmInputCmd = value;
                    RaisePropertyChanged("ConfirmInputCmd");
                }
            }
        }

        public ICommand LowerInputCmd
        {
            get { return _lowerInputCmd; }
            set
            {
                if (_lowerInputCmd != value)
                {
                    _lowerInputCmd = value;
                    RaisePropertyChanged("LowerInputCmd");
                }
            }
        }

        public ICommand RaiseInputCmd
        {
            get { return _raiseInputCmd; }
            set
            {
                if (_raiseInputCmd != value)
                {
                    _raiseInputCmd = value;
                    RaisePropertyChanged("RaiseInputCmd");
                }
            }
        }

        public ICommand EditCmd
        {
            get { return _editCmd; }
            set
            {
                if (_editCmd != value)
                {
                    _editCmd = value;
                    RaisePropertyChanged("EditCmd");
                }
            }
        }

        public ICommand FftDataAvailabe
        {
            get { return _fftData; }
            set
            {
                if (_fftData != value)
                {
                    _fftData = value;
                    RaisePropertyChanged("FftDataAvailabe");
                }
            }
        }

        public ICommand SelectAttributeCmd
        {
            get { return _selectAttributeCmd; }
            set
            {
                if (_selectAttributeCmd != value)
                {
                    _selectAttributeCmd = value;
                    RaisePropertyChanged("SelectAttributeCmd");
                }
            }
        }

        public ICommand SelectGenreCmd
        {
            get { return _selectGenreCmd; }
            set
            {
                if (_selectGenreCmd != value)
                {
                    _selectGenreCmd = value;
                    RaisePropertyChanged("SelectGenreCmd");
                }
            }
        }

        public ICommand SelectArtistCmd
        {
            get { return _selectArtistCmd; }
            set
            {
                if (_selectArtistCmd != value)
                {
                    _selectArtistCmd = value;
                    RaisePropertyChanged("SelectArtistCmd");
                }
            }
        }

        public ICommand SelectTitleCmd
        {
            get { return _selectSongCmd; }
            set
            {
                if (_selectSongCmd != value)
                {
                    _selectSongCmd = value;
                    RaisePropertyChanged("SelectTitleCmd");
                }
            }
        }

        #endregion Commands  



        #region methods

        public void UpdateVisuals(TagDataModel tagDM)
        {
            _searchView.UpdateVisual(tagDM.Id);
        }

        /// <summary>
        /// Tag has been removed from table. Remove from combi.
        /// </summary>
        public void OnVisualizationRemoved(TagDataModel tagDm)
        {
            // update state
            tagDm.ExistenceState = TagDataModel.ExistenceStates.Removed;

            // remove tag from possible combinations
            RemoveTagFromCombi.Execute(tagDm.Id);
            //CheckTagPositionsCmd.Execute(tagDm.Id);
        }

        /// <summary>
        /// Raise Property Changed from outside. This is needed for observablecollections that don't
        /// update. (e.g. after using LINQ adding/removing items doesn't raispropertychanged anymore)
        /// </summary>
        /// <param name="propName">Name of the property</param>
        public void RaisePropertyChangedManually(string propName)
        {
            RaisePropertyChanged(propName);
        }

        #endregion methods
    }
}

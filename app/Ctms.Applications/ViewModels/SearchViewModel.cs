﻿using System;
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
        private ICommand    _startSearchCommand;
        private ICommand    _selectOptionCmd;
        private ICommand    _getSuggestionsCmd;
        private string      _item1Header;
        private string      _item2Header;
        private int         _mainItemId;
        private List<TagVisualization> _tagVisualizations;
        private ISearchView _searchView;
        private Keyword _assignedKeyword;
        private ObservableCollection<TagDataModel> _tags;
        private ICommand _goHomeCmd;
        private ICommand    _goBreadcrumbCmd;
        private bool        _addedVisualization;
        private ICommand _addVisualizationCmd;
        private ICommand _editCmd;
        private List<searchObjects> _searchObjectsList;
        //private readonly IEnumerable<SearchTagViewModel> _searchTags;

        //FFT Values
        private int _fft1Value;
        private int _fft2Value;
        private int _fft3Value;
        private int _fft4Value;
        private int _fft5Value;
        private int _fft6Value;
        private int _fft7Value;
        private int _fft8Value;
        private int _fft9Value;
        private int _fft10Value;

        [ImportingConstructor]
        public SearchViewModel(ISearchView view)
            : base(view)
        {
            _tags = new ObservableCollection<TagDataModel>();
            _searchView = view;

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

        public SearchViewModel() : base(null)
        {

        }

        #region FFTValues

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

        #endregion Commands  

        public void OnVisualizationAdded(int tagId)
        {
            AddVisualization.Execute(tagId);
        }

        public ICommand AddVisualization
        {
            get { return _addVisualizationCmd; }
            set
            {
                if (_addVisualizationCmd != value)
                {
                    _addVisualizationCmd = value;
                    RaisePropertyChanged("AddVisualization");
                }
            }
        }

        public void UpdateVisuals(TagDataModel tagDM)
        {
            _searchView.UpdateVisual(tagDM.Id);
        }
    }
}

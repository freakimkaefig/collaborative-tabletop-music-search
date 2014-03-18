using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using Ctms.Domain.Objects;
using Microsoft.Surface.Presentation.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;

namespace Ctms.Applications.DataModels
{
    /// <summary>
    /// This DataModel serves for extending object with visual-specific variables
    /// </summary>
    public class TagDataModel : DataModel
    {
        private Tag _tag;
        private string _inputTerms;
        private bool _isInputVisible;
        private bool _isAssignedKeywordVisible;
        private bool _isMenuVisible = true;
        private bool _isEditVisible = false;

        public TagDataModel(Tag tag)
        {
            if (tag == null) { throw new ArgumentNullException("tag"); }

            _tag        = tag;
            _inputTerms = "";
        }

        // default constructor needed to be usable as dynamic resource in view
        public TagDataModel() { }

        public int  Id { get { return Tag.Id; } }

        public Tag Tag
        {
            get { return _tag; }
            set
            {
                if (_tag != value)
                {
                    _tag = value;
                    RaisePropertyChanged("Tag");
                }
            }
        }

        public ObservableCollection<MusicSearch.ResponseObjects.ResponseContainer.ResponseObj.Suggestion> Suggestions { get; set; }

        // visualization of this tag
        public TagVisualizationDefinition TagVisDef { get; set; }

        // terms that are inserted for artist/title search
        public string InputTerms { get { return _inputTerms; } set { _inputTerms = value; } }

        // is input field visible
        public bool IsInputVisible 
        { 
            get 
            { 
                return _isInputVisible; 
            } 
            set 
            {
                _isInputVisible = value;
                RaisePropertyChanged("IsInputVisible");
            } 
        }

        public bool IsAssignedKeywordVisible
        {
            get
            {
                return _isAssignedKeywordVisible;
            }
            set
            {
                _isAssignedKeywordVisible = value;
                RaisePropertyChanged("IsAssignedKeywordVisible");
            }
        }

        public bool IsMenuVisible
        {
            get
            {
                return _isMenuVisible;
            }
            set
            {
                _isMenuVisible = value;
                RaisePropertyChanged("IsMenuVisible");
            }
        }

        public bool IsEditVisible
        {
            get
            {
                return _isEditVisible;
            }
            set
            {
                if (_isEditVisible != value)
                {
                    _isEditVisible = value;
                    RaisePropertyChanged("IsEditVisible");
                }
            }
        }
    }
}

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
using Ctms.Applications.Common;
using Helpers;
using System.Diagnostics;
using Ctms.Domain;
using MusicSearch.Objects;

namespace Ctms.Applications.DataModels
{
    /// <summary>
    /// This DataModel serves for extending object with visual-specific variables
    /// </summary>
    public class TagDataModel : DataModel
    {
        private Tag _tag;
        private States _state;
        private string _inputTerms;
        private bool _isInputVisible;
        private bool _isAssignedKeywordVisible;
        private bool _isMenuVisible = true;
        private bool _isEditVisible = false;
        private float _height;
        private float _width;
        private short lastHandledAngle = 0;
        private int activeOptionsIndex;
        private bool _isKeywordTypesVisible;
        private bool _isConfirmBreadcrumbVisible;
        private string _backgrImageSource;
        private float _confirmCircleOpacity;

        public TagDataModel(Tag tag)
        {
            if (tag == null) { throw new ArgumentNullException("tag"); }

            _tag        = tag;
            _inputTerms = "";
            _state      = States.Editing;
        }

        // default constructor needed to be usable as dynamic resource in view
        public TagDataModel() { }

        public int CurrentOptionsIndex { get { return (int)activeOptionsIndex; } }

        /// <summary>
        /// Compute which options shall be visible, regarding tag angle and count of option placeholders
        /// </summary>
        public void UpdateVisibleOptions()
        {
            var difference = lastHandledAngle - Tag.Angle;

            // check if tag has been rotated over the switching part of 360° and 0°
            // e.g. 355 - 5 = 350 change to: 355 - 365 = -10
            if (difference > 180) difference = difference - 360;
            // e.g. 5 - 355 = -350 change to: 365 - 355 = 10
            if (difference < -180) difference = difference + 360;

            if (Math.Abs(difference) > CommonVal.Tag_OptionsStepAngle)
            {   // absolute difference is big enough to scroll to next options

                if (difference < 0)
                {   // turned tag clockwise
                    if (activeOptionsIndex + CommonVal.Tag_VisibleOptionsCount < ActiveLayerOptions.Count())
                    {   // index can be raised without getting over the top
                        activeOptionsIndex++;

                        lastHandledAngle = Tag.Angle;

                        // raise changed event so controller can react
                        RaisePropertyChanged("VisibleOptions");
                    }
                }
                else if (difference > 0)
                {   // turned tag anti-clockwise

                    // index must be at least 0
                    activeOptionsIndex = activeOptionsIndex > 1 ? activeOptionsIndex - 1 : 0;

                    lastHandledAngle = Tag.Angle;

                    // raise changed event so controller can react
                    RaisePropertyChanged("VisibleOptions");
                }
            }
        }

        private IEnumerable<TagOption> ActiveLayerOptions
        {
            get { return Tag.TagOptions.Where(to => to.LayerNr == Tag.CurrentLayerNr); }
        }

        private void LogOptions()
        {
            var names = "";
            foreach (var option in VisibleOptions)
            {
                names += option.Keyword.Name + " ";
            }
            Debug.WriteLine(names);
        }

        public void RefreshLayer()
        {
            activeOptionsIndex = 0;
        }

        public ObservableCollection<TagOption> VisibleOptions
        {
            get 
            {
                // select only some options of this layer, corresponding to current options index
                // select some options by their index in the list
                var optionsList = ActiveLayerOptions.Skip(activeOptionsIndex).Take(CommonVal.Tag_VisibleOptionsCount).ToList();

                return EntitiesHelper.ToObservableCollection<TagOption>(optionsList);
            }
        }

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

        public ObservableCollection<ResponseContainer.ResponseObj.Suggestion> Suggestions { get; set; }

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

        public bool IsKeywordTypesVisible
        {
            get
            {
                return _isKeywordTypesVisible;
            }
            set
            {
                if (_isKeywordTypesVisible != value)
                {
                    _isKeywordTypesVisible = value;
                    RaisePropertyChanged("IsKeywordTypesVisible");
                }
            }
        }

        public bool IsConfirmBreadcrumbVisible
        {
            get
            {
                return _isConfirmBreadcrumbVisible;
            }
            set
            {
                if (_isConfirmBreadcrumbVisible != value)
                {
                    _isConfirmBreadcrumbVisible = value;
                    RaisePropertyChanged("IsConfirmBreadcrumbVisible");
                }
            }
        }

        public States State
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    RaisePropertyChanged("State");
                }
            }
        }

        public float Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    RaisePropertyChanged("Height");
                }
            }
        }

        public float Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    RaisePropertyChanged("Width");
                }
            }
        }

        public string BackgrImageSource
        {
            get
            {
                return _backgrImageSource;
            }
            set
            {
                if (_backgrImageSource != value)
                {
                    _backgrImageSource = value;
                    RaisePropertyChanged("BackgrImageSource");
                }
            }
        }

        public int CombineCircleDiameter
        {
            get
            {
                return CommonVal.Tag_CombineCircleDiameter;
            }
        }

        public float ConfirmCircleOpacity
        {
            get
            {
                return _confirmCircleOpacity;
            }
            set
            {
                if (_confirmCircleOpacity != value)
                {
                    _confirmCircleOpacity = value;
                    RaisePropertyChanged("ConfirmCircleOpacity");
                }
            }
        }

        public enum States
        {
            Assigned,
            Editing,
            Removed
        }
    }
}

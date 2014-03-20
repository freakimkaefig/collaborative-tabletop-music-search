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
        private double _rotation;

        public TagDataModel(Tag tag)
        {
            if (tag == null) { throw new ArgumentNullException("tag"); }

            _tag        = tag;
            _inputTerms = "";
        }

        // default constructor needed to be usable as dynamic resource in view
        public TagDataModel() { }

        private short lastAngle = 0;

        private int currentOptionsIndex = 0;

        public void UpdateVisibleOptions()
        {
            if (Tag.Angle - lastAngle > CommonVal.Tag_OptionsStepAngle)
            {   // turned tag clockwise
                currentOptionsIndex++;
                //Console.WriteLine("currentOptionsIndex++ last angle: {0} current: {1}", lastAngle, Tag.Angle);
                lastAngle = Tag.Angle;
                RaisePropertyChanged("VisibleOptions");
                //Debug.WriteLine("VisibleOptions.Count: " + VisibleOptions.Count() + ", currentOptionsIndex++: " + currentOptionsIndex);
            }
            else if (lastAngle - Tag.Angle > CommonVal.Tag_OptionsStepAngle)
            {   // turned tag anti-clockwise
                currentOptionsIndex--;
                //Console.WriteLine("currentOptionsIndex-- last angle: {0} current: {1}", lastAngle, Tag.Angle);
                lastAngle = Tag.Angle;
                RaisePropertyChanged("VisibleOptions");
                //Debug.WriteLine("VisibleOptions.Count: " + VisibleOptions.Count() + ", currentOptionsIndex--: " + currentOptionsIndex);
                //LogOptions();
            }
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
            currentOptionsIndex = 0;
        }

        public ObservableCollection<TagOption> VisibleOptions
        {
            get 
            {
                var tagOptions      = Tag.TagOptions.Where(to => to.LayerNr == Tag.CurrentLayerNr);
                var optionsCount    = tagOptions.Count();

                //Debug.WriteLine("currentOptionsIndex: " + currentOptionsIndex);

                if (currentOptionsIndex < 0)
                {   // index must be at least 0
                    currentOptionsIndex = 0;
                }
                else if (currentOptionsIndex + CommonVal.Tag_VisibleOptionsCount > optionsCount)
                {   // index is over the head, regarding maximum count of options to display
                    currentOptionsIndex = currentOptionsIndex - 1;
                    //Debug.WriteLine("currentOptionsIndex decreased: " + currentOptionsIndex);
                }

                // select options of this layer and select only a few, corresponding to current options index
                // select some options by their index in the list
                var tagOptionsList = tagOptions.Skip(currentOptionsIndex).Take(CommonVal.Tag_VisibleOptionsCount).ToList();

                return EntitiesHelper.ToObservableCollection<TagOption>(tagOptionsList);
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

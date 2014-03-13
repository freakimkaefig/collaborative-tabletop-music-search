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
        //private readonly Tag _tag;
        private Tag _tag;
        private string _inputTerms;
        private bool _isInputVisible = false;
        //private readonly ObservableCollection<PieMenuItem> _pieMenuItems;

        public TagDataModel(Tag tag)
        {
            if (tag == null) { throw new ArgumentNullException("tag"); }

            _tag        = tag;
            _inputTerms = "";
            //_pieMenuItems = new ObservableCollection<PieMenuItem>();
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
        /*
        public ObservableCollection<PieMenuItem> PieMenuItems
        {
            get 
            {
                _pieMenuItems.Clear();
                
                foreach (var tagOption in Tag.TagOptions)
                {
                    var brush = (Brush)(new BrushConverter().ConvertFrom("#5555"));

                    var pieMenuItem = new PieMenuItem()
                    {
                        Header = tagOption.Keyword.Name,
                        SubHeader = tagOption.Keyword.Description,
                        BorderThickness = new Thickness(0.0),
                        FontSize = 16,
                        CenterText = true,
                        Background = brush
                    };
                    _pieMenuItems.Add(pieMenuItem);
                }
                return _pieMenuItems; 
            }
        }
        */
        // visualization of this tag
        public TagVisualizationDefinition TagVisDef { get; set; }

        // terms that are inserted for artist/title search
        public string   InputTerms { get { return _inputTerms; } set { _inputTerms = value; } }
        // is input field visible
        public bool     IsInputVisible { 
            get { 
                return _isInputVisible; 
            } 
            set {
                _isInputVisible = value;
                RaisePropertyChanged("IsInputVisible");
            } 
        }


    }
}

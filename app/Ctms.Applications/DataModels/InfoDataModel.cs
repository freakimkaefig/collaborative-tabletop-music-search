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

namespace Ctms.Applications.DataModels
{
    /// <summary>
    /// This DataModel serves for extending object with visual-specific variables
    /// </summary>
    public class InfoDataModel : DataModel
    {
        private Info _info;
        private bool _isVisible = false;
        private bool _isConfirmable;
        private string _confirmText;
        private int _tagId;
        private bool _isLoadingVisible;

        public InfoDataModel(Info info)
        {
            if (info == null) { throw new ArgumentNullException("info"); }
            _info = info;
        }

        // default constructor needed to be usable as dynamic resource in view
        public InfoDataModel() { }

        public Info Info { get; set; }
        
        // is info visible
        public bool IsVisible 
        { 
            get 
            {
                return _isVisible; 
            } 
            set 
            {
                _isVisible = value;
                RaisePropertyChanged("IsVisible");
            } 
        }

        public bool IsLoadingVisible
        {
            get { return _isLoadingVisible; }
            set
            {
                if (_isLoadingVisible != value)
                {
                    _isLoadingVisible = value;
                    RaisePropertyChanged("IsLoadingVisible");
                }
            }
        }

        // is info confirmable
        public bool IsConfirmable
        {
            get
            {
                return _isConfirmable;
            }
            set
            {
                _isConfirmable = value;
                RaisePropertyChanged("IsConfirmable");
            }
        }

        // is info confirmable
        public string ConfirmText
        {
            get
            {
                return _confirmText;
            }
            set
            {
                _confirmText = value;
                RaisePropertyChanged("ConfirmText");
            }
        }
    }
}

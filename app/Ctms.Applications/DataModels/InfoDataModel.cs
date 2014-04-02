﻿using System;
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

        public InfoDataModel(Info info)
        {
            if (info == null) { throw new ArgumentNullException("info"); }
            _info = info;
        }

        // default constructor needed to be usable as dynamic resource in view
        public InfoDataModel() { }

        public Info Info { get; set; }
        
        // is input field visible
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

    }
}
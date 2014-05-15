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
        private bool _isLoadingVisible;
        private TagDataModel _tag;
        private bool _isCancellable;
        private string _cancelText;

        public InfoDataModel(Info info)
        {
            if (info == null) { throw new ArgumentNullException("info"); }
            _info = info;
        }

        // default constructor needed to be usable as dynamic resource in view
        public InfoDataModel() { }

        public Info Info { get; set; }

        public TagDataModel Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
                RaisePropertyChanged("Tag");
            }
        }
        
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

        // is info cancellable
        public bool IsCancellable
        {
            get
            {
                return _isCancellable;
            }
            set
            {
                _isCancellable = value;
                RaisePropertyChanged("IsCancellable");
            }
        }

        // text for confirm button
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

        // text for cancel button
        public string CancelText
        {
            get
            {
                return _cancelText;
            }
            set
            {
                _cancelText = value;
                RaisePropertyChanged("CancelText");
            }
        }

        public Action<List<object>> ConfirmAction { get; set; }

        public Action<List<object>> CancelAction { get; set; }

        public List<object> ConfirmParameters { get; set; }

        public List<object> CancelParameters { get; set; }

    }
}

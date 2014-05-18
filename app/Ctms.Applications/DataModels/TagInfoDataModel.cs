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
    public class TagInfoDataModel : InfoDataModel
    {
        private int _tagId;

        public TagInfoDataModel(Info info) : base(info)
        {
        }

        public int TagId
        {
            get
            {
                return _tagId;
            }
            set
            {
                _tagId = value;
                RaisePropertyChanged("TagId");
            }
        }
    }
}

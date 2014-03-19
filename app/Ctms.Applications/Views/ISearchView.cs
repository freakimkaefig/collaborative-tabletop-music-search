using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using Microsoft.Surface.Presentation.Controls;
using Ctms.Domain.Objects;
using Ctms.Applications.DataModels;

namespace Ctms.Applications.Views
{
    public interface ISearchView : IView
    {
        TagVisualizer TagVisualizer { get; set; }

        void UpdateVisual(int tagId);
    }
}

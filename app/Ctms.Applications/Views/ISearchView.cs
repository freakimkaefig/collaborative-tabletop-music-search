using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using Microsoft.Surface.Presentation.Controls;
using Ctms.Domain.Objects;
using Ctms.Applications.DataModels;
using System.Windows.Shapes;

namespace Ctms.Applications.Views
{
    public interface ISearchView : IView
    {
        TagVisualizer TagVisualizer { get; set; }
        Rectangle GetFft1 { get; set; }
        Rectangle GetFft2 { get; set; }
        Rectangle GetFft3 { get; set; }
        Rectangle GetFft4 { get; set; }
        Rectangle GetFft5 { get; set; }
        Rectangle GetFft6 { get; set; }
        Rectangle GetFft7 { get; set; }
        Rectangle GetFft8 { get; set; }
        Rectangle GetFft9 { get; set; }
        Rectangle GetFft10 { get; set; }
        Rectangle GetFft11 { get; set; }
        Rectangle GetFft12 { get; set; }
        Rectangle GetFft13 { get; set; }
        Rectangle GetFft14 { get; set; }
        Rectangle GetFft15 { get; set; }

        void UpdateVisual(int tagId);
        void InitializeRectangles();
        void LogScrollToEnd();
    }
}

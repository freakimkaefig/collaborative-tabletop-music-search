using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using System.Windows;

namespace Ctms.Applications.Views
{
    public interface IPlaylistView : IView
    {
        VisualState VisualStateRotate0 { get; set; }
        VisualState VisualStateRotate180 { get; set; }
    }
}

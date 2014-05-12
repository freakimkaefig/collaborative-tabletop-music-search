using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using System.Windows;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Controls;

namespace Ctms.Applications.Views
{
    public interface IPlaylistView : IView
    {
        VisualState VisualStateRotate0 { get; set; }
        VisualState VisualStateRotate180 { get; set; }

        VisualState VisualStatePlaylistVisible { get; set; }
        VisualState VisualStatePlaylistInvisible { get; set; }

        SurfaceButton GetDropTargetLeft { get; set; }
        SurfaceButton GetDropTargetRight { get; set; }

        Image GetPlusImageLeft { get; set; }
        Image GetPlusImageRight { get; set; }
    }
}

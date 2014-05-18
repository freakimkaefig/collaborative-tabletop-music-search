using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using System.Windows;

namespace Ctms.Applications.Views
{
    public interface IMenuView : IView
    {
        VisualState VisualStateMenuVisible { get; set; }
        VisualState VisualStateMenuInvisible { get; set; }

        VisualState VisualStateMenuRotate0 { get; set; }
        VisualState VisualStateMenuRotate180 { get; set; }

        VisualState VisualStateLoginDialogVisible { get; set; }
        VisualState VisualStateLoginDialogInvisible { get; set; }

        VisualState VisualStateNewPlaylistVisible { get; set; }
        VisualState VisualStateNewPlaylistInvisible { get; set; }

        VisualState VisualStateOpenPlaylistVisible { get; set; }
        VisualState VisualStateOpenPlaylistInvisible { get; set; }
    }
}

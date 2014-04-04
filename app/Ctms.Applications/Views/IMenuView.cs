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
        VisualState VisualStateLoginDialogVisible { get; set; }
        VisualState VisualStateLoginDialogInvisible { get; set; }
        VisualState VisualStateRotate0_LoginDialog { get; set; }
        VisualState VisualStateRotate180_LoginDialog { get; set; }
        VisualState VisualStateRotate0_OpenPlaylistDialog { get; set; }
        VisualState VisualStateRotate180_OpenPlaylistDialog { get; set; }
        VisualState VisualStateRotate0_CreatePlaylistDialog { get; set; }
        VisualState VisualStateRotate180_CreatePlaylistDialog { get; set; }
    }
}

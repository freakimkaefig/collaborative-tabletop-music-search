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
    }
}

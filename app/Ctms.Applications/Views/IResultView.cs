using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;

namespace Ctms.Applications.Views
{
    public interface IResultView : IView
    {
        void AddResources();
    }
}

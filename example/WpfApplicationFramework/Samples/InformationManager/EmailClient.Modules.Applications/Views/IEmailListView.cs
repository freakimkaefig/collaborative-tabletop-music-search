﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;

namespace Waf.InformationManager.EmailClient.Modules.Applications.Views
{
    public interface IEmailListView : IView
    {
        void FocusItem();
    }
}

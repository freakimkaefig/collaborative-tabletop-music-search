﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using Microsoft.Surface.Presentation.Controls;

namespace Ctms.Applications.Views
{
    public interface ITagVisualizationView : IView
    {
        TagVisualizer TagVisualizer { get; set; }
    }
}

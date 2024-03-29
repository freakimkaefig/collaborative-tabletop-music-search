﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Ctms.Applications.Services;
using System.Windows;
using System.Globalization;
using System.Windows.Markup;

namespace Ctms.Presentation.Services
{
    [Export(typeof(IPresentationService))]
    public class PresentationService : IPresentationService
    {
        public double VirtualScreenWidth { get { return SystemParameters.VirtualScreenWidth; } }

        public double VirtualScreenHeight { get { return SystemParameters.VirtualScreenHeight; } }


        public void InitializeCultures()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }
    }
}

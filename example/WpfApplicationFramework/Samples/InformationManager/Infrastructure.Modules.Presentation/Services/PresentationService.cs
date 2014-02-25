using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Waf.InformationManager.Common.Applications.Services;

namespace Waf.InformationManager.Infrastructure.Modules.Presentation.Services
{
    [Export(typeof(IPresentationService))]
    internal class PresentationService : IPresentationService
    {
        public void Initialize()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));


            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            mergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/Waf.InformationManager.Infrastructure.Modules.Presentation;component/Resources/ModuleResources.xaml")
            });
        }
    }
}

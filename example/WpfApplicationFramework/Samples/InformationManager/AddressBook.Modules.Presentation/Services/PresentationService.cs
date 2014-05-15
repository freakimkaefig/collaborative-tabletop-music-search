using System;
using System.ComponentModel.Composition;
using System.Windows;
using Waf.InformationManager.Common.Applications.Services;

namespace Waf.InformationManager.AddressBook.Modules.Presentation.Services
{
    [Export(typeof(IPresentationService))]
    internal class PresentationService : IPresentationService
    {
        public void Initialize()
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            mergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/Waf.InformationManager.AddressBook.Modules.Presentation;component/Resources/ModuleResources.xaml")
            });
        }
    }
}

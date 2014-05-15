using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.EmailClient.Modules.Applications.Views;
using System.ComponentModel.Composition;

namespace Test.InformationManager.EmailClient.Modules.Applications.Views
{
    [Export(typeof(IPop3SettingsView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockPop3SettingsView : IPop3SettingsView
    {
        public object DataContext { get; set; }
    }
}

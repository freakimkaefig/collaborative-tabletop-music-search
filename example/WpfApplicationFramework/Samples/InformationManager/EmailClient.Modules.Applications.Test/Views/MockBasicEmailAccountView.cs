using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.EmailClient.Modules.Applications.Views;
using System.ComponentModel.Composition;

namespace Test.InformationManager.EmailClient.Modules.Applications.Views
{
    [Export(typeof(IBasicEmailAccountView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockBasicEmailAccountView : IBasicEmailAccountView
    {
        public object DataContext { get; set; }
    }
}

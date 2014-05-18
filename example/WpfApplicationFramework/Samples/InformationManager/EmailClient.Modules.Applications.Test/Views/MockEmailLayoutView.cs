using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.EmailClient.Modules.Applications.Views;
using System.ComponentModel.Composition;

namespace Test.InformationManager.EmailClient.Modules.Applications.Views
{
    [Export(typeof(IEmailLayoutView)), Export]
    public class MockEmailLayoutView : IEmailLayoutView
    {
        public object DataContext { get; set; }
    }
}

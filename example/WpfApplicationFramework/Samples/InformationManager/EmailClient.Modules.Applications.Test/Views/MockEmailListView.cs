using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.EmailClient.Modules.Applications.Views;
using System.ComponentModel.Composition;

namespace Test.InformationManager.EmailClient.Modules.Applications.Views
{
    [Export(typeof(IEmailListView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockEmailListView : IEmailListView
    {
        public object DataContext { get; set; }

        public Action<MockEmailListView> FocusItemAction { get; set; }
        

        public void FocusItem()
        {
            FocusItemAction(this);
        }
    }
}

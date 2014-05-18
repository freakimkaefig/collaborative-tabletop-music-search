using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.EmailClient.Modules.Applications.Views;
using System.ComponentModel.Composition;

namespace Test.InformationManager.EmailClient.Modules.Applications.Views
{
    [Export(typeof(IEmailAccountsView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockEmailAccountsView : IEmailAccountsView
    {
        public static Action<MockEmailAccountsView> ShowDialogAction { get; set; }
        
        public object DataContext { get; set; }

        public object Owner { get; private set; }

        
        public void ShowDialog(object owner)
        {
            this.Owner = owner;
            ShowDialogAction(this);
            this.Owner = null;
        }
    }
}

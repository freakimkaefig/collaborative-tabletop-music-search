using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.EmailClient.Modules.Applications.Views;
using System.ComponentModel.Composition;

namespace Test.InformationManager.EmailClient.Modules.Applications.Views
{
    [Export(typeof(IEditEmailAccountView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockEditEmailAccountView : IEditEmailAccountView
    {
        public static Action<MockEditEmailAccountView> ShowDialogAction;
        
        public object DataContext { get; set; }

        public object Owner { get; private set; }

        public bool IsVisible { get; private set; }


        public void ShowDialog(object owner)
        {
            this.Owner = owner;
            this.IsVisible = true;
            ShowDialogAction(this);
            this.Owner = null;
            this.IsVisible = false;
        }

        public void Close()
        {
            this.Owner = null;
            this.IsVisible = false;
        }
    }
}

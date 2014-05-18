using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.EmailClient.Modules.Applications.Views;
using System.ComponentModel.Composition;

namespace Test.InformationManager.EmailClient.Modules.Applications.Views
{
    [Export(typeof(INewEmailView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockNewEmailView : INewEmailView
    {
        public static Action<MockNewEmailView> ShowAction { get; set; }
        
        public object DataContext { get; set; }

        public object Owner { get; private set; }

        public bool IsVisible { get; private set; }


        public void Show(object owner)
        {
            this.Owner = owner;
            this.IsVisible = true;
            if (ShowAction != null) { ShowAction(this); }
        }

        public void Close()
        {
            this.Owner = null;
            this.IsVisible = false;
        }
    }
}

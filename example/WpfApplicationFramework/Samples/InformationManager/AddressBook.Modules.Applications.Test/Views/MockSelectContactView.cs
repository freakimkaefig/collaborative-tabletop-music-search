using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Waf.InformationManager.AddressBook.Modules.Applications.Views;

namespace Test.InformationManager.AddressBook.Modules.Applications.Views
{
    [Export(typeof(ISelectContactView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockSelectContactView : ISelectContactView
    {
        public static Action<MockSelectContactView> ShowDialogAction { get; set; }
        
        public object DataContext { get; set; }
        
        public bool IsVisible { get; private set; }
        
        public object Owner { get; private set; }


        public void ShowDialog(object owner)
        {
            Owner = owner;
            IsVisible = true;
            ShowDialogAction(this);
        }

        public void Close()
        {
            Owner = null;
            IsVisible = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.AddressBook.Modules.Applications.Views;
using System.ComponentModel.Composition;

namespace Test.InformationManager.AddressBook.Modules.Applications.Views
{
    [Export(typeof(IContactListView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockContactListView : IContactListView
    {
        public Action<MockContactListView> FocusItemAction { get; set; }
        
        public object DataContext { get; set; }
        

        public void FocusItem()
        {
            FocusItemAction(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.AddressBook.Modules.Applications.Views;
using System.ComponentModel.Composition;

namespace Test.InformationManager.AddressBook.Modules.Applications.Views
{
    [Export(typeof(IContactView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockContactView : IContactView
    {
        public object DataContext { get; set; }
    }
}

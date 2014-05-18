using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.AddressBook.Modules.Applications.Views;
using System.ComponentModel.Composition;

namespace Test.InformationManager.AddressBook.Modules.Applications.Views
{
    [Export(typeof(IContactLayoutView))]
    public class MockContactLayoutView : IContactLayoutView
    {
        public object DataContext { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.BookLibrary.Library.Applications.Views;
using System.ComponentModel.Composition;

namespace Waf.BookLibrary.Library.Applications.Test.Views
{
    [Export(typeof(ILendToView)), Export]
    public class MockLendToView : MockView, ILendToView
    {
        public Action<MockLendToView> ShowDialogAction { get; set; }

        public bool IsVisible { get; private set; }

        public object Owner { get; private set; }

        
        public void ShowDialog(object owner)
        {
            Owner = owner;
            IsVisible = true;
            if (ShowDialogAction != null) { ShowDialogAction(this); }
            IsVisible = false;
            Owner = false;
        }

        public void Close()
        {
            IsVisible = false;
            Owner = null;
        }
    }
}

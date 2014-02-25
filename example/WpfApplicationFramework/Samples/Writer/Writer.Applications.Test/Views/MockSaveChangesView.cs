using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using Waf.Writer.Applications.Views;
using System.ComponentModel.Composition;
using Waf.Writer.Applications.ViewModels;

namespace Waf.Writer.Applications.Test.Views
{
    [Export(typeof(ISaveChangesView)), Export]
    public class MockSaveChangesView : MockView, ISaveChangesView
    {
        public Action<MockSaveChangesView> ShowDialogAction { get; set; }
        public bool IsVisible { get; private set; }
        public object Owner { get; private set; }
        public SaveChangesViewModel ViewModel { get { return ViewHelper.GetViewModel<SaveChangesViewModel>(this); } }


        public void ShowDialog(object owner)
        {
            Owner = owner;
            IsVisible = true;
            if (ShowDialogAction != null) { ShowDialogAction(this); }
            IsVisible = false;
            Owner = null;
        }

        public void Close()
        {
            IsVisible = false;
            Owner = null;
        }
    }
}

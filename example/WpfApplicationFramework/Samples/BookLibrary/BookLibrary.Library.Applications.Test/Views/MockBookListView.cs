using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.BookLibrary.Library.Applications.Views;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using Waf.BookLibrary.Library.Applications.ViewModels;

namespace Waf.BookLibrary.Library.Applications.Test.Views
{
    [Export(typeof(IBookListView)), Export]
    public class MockBookListView : MockView, IBookListView
    {
        public bool FirstCellHasFocus { get; set; }


        public void FocusFirstCell() { FirstCellHasFocus = true; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.BookLibrary.Library.Applications.Views;
using System.ComponentModel.Composition;

namespace Waf.BookLibrary.Library.Applications.Test.Views
{
    [Export(typeof(IPersonListView)), Export]
    public class MockPersonListView : MockView, IPersonListView
    {
        public bool FirstCellHasFocus { get; set; }


        public void FocusFirstCell() { FirstCellHasFocus = true; }
    }
}

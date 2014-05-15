using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.Writer.Applications.Views;
using System.ComponentModel.Composition;

namespace Waf.Writer.Applications.Test.Views
{
    [Export(typeof(IPrintPreviewView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockPrintPreviewView : MockView, IPrintPreviewView
    {
        public bool PrintCalled { get; set; }


        public void Print()
        {
            PrintCalled = true;    
        }
    }
}

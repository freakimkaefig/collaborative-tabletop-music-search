using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.Writer.Applications.Views;
using System.ComponentModel.Composition;

namespace Waf.Writer.Applications.Test.Views
{
    [Export(typeof(IRichTextView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MockRichTextView : MockView, IRichTextView
    {
    }
}

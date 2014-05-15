using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Foundation;
using Waf.Writer.Applications.Services;
using System.Waf.Applications;

namespace Waf.Writer.Presentation.DesignData
{
    public class MockShellService : DataModel, IShellService
    {
        public MockShellService()
        {
            Zoom = 1;
            DocumentName = "Document 1";
        }


        public IEnumerable<string> DefaultZooms { get { return new[] { "200%", "150%", "125%", "100%", "75%", "50%" }; } }

        public double Zoom { get; set; }

        public string DocumentName { get; set; }

        public object ShellView { get; set; }
    }
}

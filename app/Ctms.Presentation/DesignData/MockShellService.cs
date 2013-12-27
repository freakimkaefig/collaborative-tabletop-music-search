using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Foundation;
using Ctms.Applications.Services;

namespace Ctms.Presentation.DesignData
{
    class MockShellService : Model, IShellService
    {
        public object ShellView     { get; set; }
        public object DetailView    { get; set; }
        public object MenuView      { get; set; }
        public object PlaylistView  { get; set; }
        public object ResultView    { get; set; }
        public object SearchView    { get; set; }
    }
}

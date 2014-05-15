using System.Collections.Generic;
using System.ComponentModel;

namespace Waf.Writer.Applications.Services
{
    public interface IShellService : INotifyPropertyChanged
    {
        object ShellView { get; }

        string DocumentName { get; set; }
        
        IEnumerable<string> DefaultZooms { get; }

        double Zoom { get; set; }
    }
}

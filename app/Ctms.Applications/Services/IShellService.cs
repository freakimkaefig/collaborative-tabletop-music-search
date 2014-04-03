using System;
using System.ComponentModel;

namespace Ctms.Applications.Services
{
    public interface IShellService : INotifyPropertyChanged
    {
        object ShellView { get; }

        object DetailView { get; set; }

        object PlaylistView { get; set; }

        object ResultView { get; set; }

        object SearchView { get; set; }

        object SearchTagView { get; set; }

        object MenuView { get; set; }

        object InfoView { get; set; }
    }
}

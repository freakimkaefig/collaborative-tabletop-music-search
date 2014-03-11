using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Ctms.Applications.Views;
using Ctms.Applications.ViewModels;
using System.Waf.Applications;
using Ctms.Domain.Objects;
using Ctms.Applications.DataModels;

namespace Ctms.Presentation.Views
{
    [Export(typeof(IPlaylistView))]
    public partial class PlaylistView : UserControl, IPlaylistView
    {
        private readonly Lazy<PlaylistViewModel> _lazyVm;

        public PlaylistView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<PlaylistViewModel>(() => ViewHelper.GetViewModel<PlaylistViewModel>(this));
        }

        private PlaylistViewModel _viewModel { get { return _lazyVm.Value; } }

        private void PlaylistAddDropTarget_DragEnter(object sender, Microsoft.Surface.Presentation.SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = "DragEnter";
        }

        private void PlaylistAddDropTarget_DragLeave(object sender, Microsoft.Surface.Presentation.SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = null;
        }

        private void PlaylistAddDropTarget_Drop(object sender, Microsoft.Surface.Presentation.SurfaceDragDropEventArgs e)
        {
            object result = e.Cursor.Data as ResultDataModel;
            _viewModel.AddTrackCommand.Execute(result);
        }
    }
}

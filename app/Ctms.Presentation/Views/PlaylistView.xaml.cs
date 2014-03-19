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
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation;

namespace Ctms.Presentation.Views
{
    [Export(typeof(IPlaylistView))]
    public partial class PlaylistView : UserControl, IPlaylistView
    {
        private readonly Lazy<PlaylistViewModel> _lazyVm;

        //VisualStates
        private VisualState _rotate180;
        private VisualState _rotate0;

        public PlaylistView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<PlaylistViewModel>(() => ViewHelper.GetViewModel<PlaylistViewModel>(this));

            _rotate0 = Rotate0;
            _rotate180 = Rotate180;
        }

        public VisualState VisualStateRotate0 { get { return _rotate0; } set { } }
        public VisualState VisualStateRotate180 { get { return _rotate180; } set { } }

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

        private void surfaceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.JumpToTrackCommand.Execute(surfaceListBox.SelectedIndex);
        }

        private void Playlist_PreviewInputDeviceDown(object sender, TouchEventArgs e)
        {
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            SurfaceListBoxItem draggedElement = null;

            // Find the touched SurfaceListBoxItem object.
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as SurfaceListBoxItem) == null)
                {
                    findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                }
            }

            if (draggedElement == null)
            {
                return;
            }

            // Create the cursor visual.
            ContentControl cursorVisual = new ContentControl()
            {
                Content = draggedElement.DataContext,
                Style = FindResource("CursorStyle") as Style
            };

            // Add a handler. This will enable the application to change the visual cues.
            SurfaceDragDrop.AddTargetChangedHandler(cursorVisual, OnTargetChanged);

            // Create a list of input devices. Add the touches that
            // are currently captured within the dragged element and
            // the current touch (if it isn't already in the list).
            List<InputDevice> devices = new List<InputDevice>();
            devices.Add(e.TouchDevice);
            foreach (TouchDevice touch in draggedElement.TouchesCapturedWithin)
            {
                if (touch != e.TouchDevice)
                {
                    devices.Add(touch);
                }
            }

            // Get the drag source object
            ItemsControl dragSource = ItemsControl.ItemsControlFromItemContainer(draggedElement);

            SurfaceDragCursor startDragOkay =
                SurfaceDragDrop.BeginDragDrop(
                  dragSource,                 // The SurfaceListBox object that the cursor is dragged out from.
                  draggedElement,             // The SurfaceListBoxItem object that is dragged from the drag source.
                  cursorVisual,               // The visual element of the cursor.
                  draggedElement.DataContext, // The data associated with the cursor.
                  devices,                    // The input devices that start dragging the cursor.
                  DragDropEffects.Move);      // The allowed drag-and-drop effects of the operation.

            // If the drag began successfully, set e.Handled to true. 
            // Otherwise SurfaceListBoxItem captures the touch 
            // and causes the drag operation to fail.
            e.Handled = (startDragOkay != null);
        }

        private void OnTargetChanged(object sender, TargetChangedEventArgs e)
        {
            if (e.Cursor.CurrentTarget != null)
            {
                ResultDataModel data = e.Cursor.Data as ResultDataModel;
                //e.Cursor.Visual.Tag = (data.CanDrop) ? "CanDrop" : "CannotDrop";
            }
            else
            {
                e.Cursor.Visual.Tag = null;
            }
        }

        private void Playlist_OnDragCanceled(object sender, Microsoft.Surface.Presentation.SurfaceDragDropEventArgs e)
        {

        }

        private void Playlist_OnDragCompleted(object sender, Microsoft.Surface.Presentation.SurfaceDragCompletedEventArgs e)
        {

        }

        private void Playlist_DragEnter(object sender, Microsoft.Surface.Presentation.SurfaceDragDropEventArgs e)
        {
            ResultDataModel data = e.Cursor.Data as ResultDataModel;

            /*if (!data.CanDrop)
            {
                e.Effects = DragDropEffects.None;
            }*/
        }

        private void Playlist_DragLeave(object sender, Microsoft.Surface.Presentation.SurfaceDragDropEventArgs e)
        {
            e.Effects = e.Cursor.AllowedEffects;
        }

        private void Playlist_Drop(object sender, Microsoft.Surface.Presentation.SurfaceDragDropEventArgs e)
        {
            //Res.Add(e.Cursor.Data as DataItem);
        }
    }
}

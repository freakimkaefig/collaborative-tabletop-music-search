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
using Blake.NUI.WPF.Gestures;
using System.Windows.Media.Animation;
using System.Numerics;

namespace Ctms.Presentation.Views
{
    [Export(typeof(IPlaylistView))]
    public partial class PlaylistView : UserControl, IPlaylistView
    {
        private readonly Lazy<PlaylistViewModel> _lazyVm;

        //VisualStates
        private VisualState _rotate180;
        private VisualState _rotate0;

        private SurfaceButton _dropTargetLeft;
        private SurfaceButton _dropTargetRight;

        private Image _plusImageLeft;
        private Image _plusImageRight;

        private FrameworkElement _trashBin;

        public PlaylistView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<PlaylistViewModel>(() => ViewHelper.GetViewModel<PlaylistViewModel>(this));

            _rotate0 = Rotate0;
            _rotate180 = Rotate180;

            Events.RegisterGestureEventSupport(this);

            _trashBin = TrashBin;

            _dropTargetLeft = DropTargetLeft;
            _dropTargetRight = DropTargetRight;

            _plusImageLeft = PlusImageLeft;
            _plusImageRight = PlusImageRight;
        }

        public VisualState VisualStateRotate0 { get { return _rotate0; } set { } }
        public VisualState VisualStateRotate180 { get { return _rotate180; } set { } }

        public SurfaceButton GetDropTargetLeft { get { return _dropTargetLeft; } set { } }
        public SurfaceButton GetDropTargetRight { get { return _dropTargetRight; } set { } }

        public Image GetPlusImageLeft { get { return _plusImageLeft; } set { } }
        public Image GetPlusImageRight { get { return _plusImageRight; } set { } }

        private PlaylistViewModel _viewModel { get { return _lazyVm.Value; } }

        private void surfaceListBox_DoubleTapGesture(object sender, GestureEventArgs e)
        {
            if (surfaceListBox.SelectedIndex != -1)
            {
                _viewModel.JumpToTrackCommand.Execute(surfaceListBox.SelectedIndex);
            }
        }


        //Drag & Drop inside PlaylistView(SurfaceListBox)
        private void Playlist_HoldGesture(object sender, GestureEventArgs e)
        {
            if (e.TouchDevice.Captured is SurfaceScrollViewer)
            {
                //if touch isn't captured over SurfaceListBoxItem
                _viewModel.TrashVisible = false;
            }
            else
            {
                _viewModel.TrashVisible = true;

                FrameworkElement findSource = e.OriginalSource as FrameworkElement;
                SurfaceListBoxItem draggedElement = e.TouchDevice.Target as SurfaceListBoxItem;

                //content.Opacity = 0.5;

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
                    Style = FindResource("PlaylistItemCursorStyle") as Style
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

                //Remove item from list
                //_viewModel.ResultsForPlaylist.Remove(draggedElement.Content as ResultDataModel);
            }
        }

        private void OnTargetChanged(object sender, TargetChangedEventArgs e)
        {
            if (e.Cursor.CurrentTarget != null)
            {
                ResultDataModel data = e.Cursor.Data as ResultDataModel;
                e.Cursor.Visual.Tag = (data.CanDrop) ? "CanDrop" : "CannotDrop";
                if (e.Cursor.CurrentTarget == _trashBin)
                {
                    e.Cursor.Visual.Tag = "DragEnter";
                }
            }
            else
            {
                e.Cursor.Visual.Tag = null;
            }
        }

        private void Playlist_OnDragCanceled(object sender, SurfaceDragDropEventArgs e)
        {
            _viewModel.TrashVisible = false;
        }

        private void Playlist_OnDragCompleted(object sender, SurfaceDragCompletedEventArgs e)
        {
            _viewModel.TrashVisible = false;
        }

        private void Playlist_OnDragOver(object sender, SurfaceDragDropEventArgs e)
        {
            ResultDataModel data = e.Cursor.Data as ResultDataModel;

            SurfaceListBox target = e.Cursor.CurrentTarget as SurfaceListBox;
            int index = -1;
            SurfaceListBoxItem sListBoxItem = null;
            for (int i = 0; i < _viewModel.ResultsForPlaylist.Count; i++)
            {
                sListBoxItem = target.ItemContainerGenerator.ContainerFromIndex(i) as SurfaceListBoxItem;
                if (sListBoxItem == null) continue;
                if (IsMouseOverTarget(sListBoxItem, e.Cursor.GetPosition((IInputElement)sListBoxItem)) == 1)
                {
                    index = i;
                    //textBox.Text += "Index: " + index + " | Position: up\n";
                    //textBox.ScrollToEnd();
                    break;
                }
                else if (IsMouseOverTarget(sListBoxItem, e.Cursor.GetPosition((IInputElement)sListBoxItem)) == 2)
                {
                    index = i + 1;
                    //textBox.Text += "Index: " + index + " | Position: down\n";
                    //textBox.ScrollToEnd();
                    break;
                }
            }
            if (index != -1)
            {
                if (_viewModel.ResultsForPlaylist.IndexOf(null) != -1)
                {
                    _viewModel.ResultsForPlaylist.Remove(null);
                }

                try
                {
                    int hideFrom = _viewModel.ResultsForPlaylist.IndexOf(data);
                    _viewModel.ResultsForPlaylist.Insert(index, null);
                }
                catch (ArgumentOutOfRangeException exception)
                {
                    //textBox.Text += exception.Message + "\n";
                    //textBox.ScrollToEnd();
                }
                //_viewModel.ResultsForPlaylist.ElementAt(hideFrom).Opacity = 0.5;
            }
        }

        private void Playlist_DragEnter(object sender, SurfaceDragDropEventArgs e)
        {
            ResultDataModel data = e.Cursor.Data as ResultDataModel;
            if (data != null)
            {
                if (!data.CanDrop)
                {
                    e.Effects = DragDropEffects.None;
                }
            }
        }

        private void Playlist_DragLeave(object sender, SurfaceDragDropEventArgs e)
        {
            e.Effects = e.Cursor.AllowedEffects;
        }

        private void Playlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private static int IsMouseOverTarget(Visual target, Point point)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            Rect up = new Rect(bounds.TopLeft.X, bounds.TopLeft.Y, bounds.Width, bounds.Height/2);
            Rect down = new Rect(bounds.TopLeft.X, bounds.TopLeft.Y + up.Height, bounds.Width, bounds.Height / 2);
            if (up.Contains(point))
            {
                return 1;
            }
            else if (down.Contains(point))
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }

        private void Playlist_Drop(object sender, SurfaceDragDropEventArgs e)
        {
            _viewModel.TrashVisible = false;

            ResultDataModel droppedItem = e.Cursor.Data as ResultDataModel;
            SurfaceListBox target = e.Cursor.CurrentTarget as SurfaceListBox;
            int insertIndex = _viewModel.ResultsForPlaylist.IndexOf(null);
            if (insertIndex == -1)
            {
                insertIndex = _viewModel.ResultsForPlaylist.Count;
            }
            int removeIndex = _viewModel.ResultsForPlaylist.IndexOf(droppedItem);
            _viewModel.ResultsForPlaylist.Insert(insertIndex, droppedItem);
            _viewModel.ResultsForPlaylist.Remove(droppedItem);
            _viewModel.ResultsForPlaylist.Remove(null);

            object[] data = new object[] { removeIndex, (insertIndex) };
            _viewModel.ReorderTrackCommand.Execute(data);
        }

        private void PlaylistRemoveDropTarget_DragOver(object sender, SurfaceDragDropEventArgs e)
        {

        }

        private void PlaylistRemoveDropTarget_DragEnter(object sender, SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = "DragEnter";
        }

        private void PlaylistRemoveDropTarget_DragLeave(object sender, SurfaceDragDropEventArgs e)
        {
            e.Cursor.Visual.Tag = null;
        }

        private void PlaylistRemoveDropTarget_Drop(object sender, SurfaceDragDropEventArgs e)
        {
            //Remove dragged track from playlist
            ResultDataModel droppedItem = e.Cursor.Data as ResultDataModel;
            int removeIndex = _viewModel.ResultsForPlaylist.IndexOf(droppedItem);
            _viewModel.ResultsForPlaylist.Remove(null);

            _viewModel.RemoveTrackCommand.Execute(removeIndex);
        }

        private void Playlist_TapGesture(object sender, GestureEventArgs e)
        {
            if (surfaceListBox.SelectedIndex != -1)
            {
                _viewModel.CurrentTrack = _viewModel.ResultsForPlaylist[surfaceListBox.SelectedIndex];
                _viewModel.LoadDetailsCommand.Execute(_viewModel.ResultsForPlaylist[surfaceListBox.SelectedIndex]);
            }
        }

        private void TabItem_TouchDown(object sender, TouchEventArgs e)
        {
            TabItem tab = sender as TabItem;
            TabControl control = tab.Parent as TabControl;
            control.SelectedItem = tab;
            e.Handled = true;
        }
    }
}

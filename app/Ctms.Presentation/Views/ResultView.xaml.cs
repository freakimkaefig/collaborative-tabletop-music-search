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
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation;
using Ctms.Domain.Objects;
using Ctms.Applications.DataModels;
using Blake.NUI.WPF.Gestures;

namespace Ctms.Presentation.Views
{
    [Export(typeof(IResultView))]
    public partial class ResultView : UserControl, IResultView
    {
        private readonly Lazy<ResultViewModel> _lazyVm;

        public ResultView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<ResultViewModel>(() => ViewHelper.GetViewModel<ResultViewModel>(this));

            Events.RegisterGestureEventSupport(this);
        }

        public void AddResources()
        {
            this.Resources["ViewModel"] = _lazyVm.Value;
            this.Resources["Results"] = _lazyVm.Value.Results;
        }

        // Provides this view's viewmodel
        private ResultViewModel _viewModel { get { return _lazyVm.Value; } }

        private void Results_OnDragCompleted(object sender, Microsoft.Surface.Presentation.SurfaceDragCompletedEventArgs e)
        {

        }

        private void Results_OnDragCanceled(object sender, Microsoft.Surface.Presentation.SurfaceDragDropEventArgs e)
        {
            ResultDataModel data = e.Cursor.Data as ResultDataModel;
            ScatterViewItem item = data.DraggedElement as ScatterViewItem;
            if (item != null)
            {
                item.Visibility = Visibility.Visible;
                item.Orientation = e.Cursor.GetOrientation(this);
                item.Center = e.Cursor.GetPosition(this);
            }
        }

        private void Results_HoldGesture(object sender, GestureEventArgs e)
        {
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            ScatterViewItem draggedElement = null;

            // Find the ScatterViewItem object that is being touched.
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as ScatterViewItem) == null)
                {
                    findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                }
            }

            if (draggedElement == null)
            {
                return;
            }

            ResultDataModel data = draggedElement.Content as ResultDataModel;

            // Set the dragged element. This is needed in case the drag operation is canceled.
            data.DraggedElement = draggedElement;

            // Create the cursor visual.
            ContentControl cursorVisual = new ContentControl()
            {
                Content = draggedElement.DataContext,
                Style = FindResource("ResultCursorStyle") as Style
            };

            // Create a list of input devices, 
            // and add the device passed to this event handler.
            List<InputDevice> devices = new List<InputDevice>();
            devices.Add(e.TouchDevice);

            // If there are touch devices captured within the element,
            // add them to the list of input devices.
            foreach (InputDevice device in draggedElement.TouchesCapturedWithin)
            {
                if (device != e.TouchDevice)
                {
                    devices.Add(device);
                }
            }

            // Get the drag source object.
            ItemsControl dragSource = ItemsControl.ItemsControlFromItemContainer(draggedElement);

            // Start the drag-and-drop operation.
            SurfaceDragCursor cursor =
                SurfaceDragDrop.BeginDragDrop(
                // The ScatterView object that the cursor is dragged out from.
                  dragSource,
                // The ScatterViewItem object that is dragged from the drag source.
                  draggedElement,
                // The visual element of the cursor.
                  cursorVisual,
                // The data attached with the cursor.
                  draggedElement.DataContext,
                // The input devices that start dragging the cursor.
                  devices,
                // The allowed drag-and-drop effects of the operation.
                  DragDropEffects.Copy);

            // If the cursor was created, the drag-and-drop operation was successfully started.
            if (cursor != null)
            {
                // Hide the ScatterViewItem.
                //draggedElement.Visibility = Visibility.Hidden;

                // This event has been handled.
                e.Handled = true;
            }

            e.Handled = (cursor != null);
        }

        private void Results_DoubleTapGesture(object sender, GestureEventArgs e)
        {
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            ScatterViewItem clickedElement = null;

            // Find the ScatterViewItem object that is being touched.
            while (clickedElement == null && findSource != null)
            {
                if ((clickedElement = findSource as ScatterViewItem) == null)
                {
                    findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                }
            }

            if (clickedElement == null)
            {
                return;
            }

            ResultDataModel data = clickedElement.Content as ResultDataModel;

            _viewModel.PrelistenCommand.Execute((object)data);
        }

        private void Results_ContainerDeactivated(object sender, RoutedEventArgs e)
        {
            if (_viewModel.PlaylistOpened == true)
            {
                FrameworkElement dropTargetLeft = _viewModel.DropTargetLeft as FrameworkElement;
                FrameworkElement dropTargetRight = _viewModel.DropTargetRight as FrameworkElement;

                ScatterView scatterView = e.Source as ScatterView;

                FrameworkElement findSource = e.OriginalSource as FrameworkElement;
                ScatterViewItem draggedElement = null;

                // Find the ScatterViewItem object that is being touched.
                while (draggedElement == null && findSource != null)
                {
                    if ((draggedElement = findSource as ScatterViewItem) == null)
                    {
                        findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                    }
                }

                if (draggedElement == null)
                {
                    return;
                }

                Rect dropLeftBounds = VisualTreeHelper.GetDescendantBounds(dropTargetLeft);
                Rect dropRightBounds = VisualTreeHelper.GetDescendantBounds(dropTargetRight);
                Rect dragRect = VisualTreeHelper.GetDescendantBounds(draggedElement);

                GeneralTransform transformLeft = scatterView.TransformToVisual(dropTargetLeft);
                GeneralTransform transformRight = scatterView.TransformToVisual(dropTargetRight);

                bool leftC = dropLeftBounds.Contains(transformLeft.Transform(draggedElement.ActualCenter));
                bool rightC = dropRightBounds.Contains(transformRight.Transform(draggedElement.ActualCenter));

                if (dropLeftBounds.Contains(transformLeft.Transform(draggedElement.ActualCenter)))
                {
                    ResultDataModel result = draggedElement.Content as ResultDataModel;

                    object[] data = new object[]
                    {
                        draggedElement.Content as ResultDataModel,
                        _viewModel.PlusImageLeft as Image,
                    };

                    _viewModel.AddTrackCommand.Execute(data);
                    draggedElement.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (dropRightBounds.Contains(transformRight.Transform(draggedElement.ActualCenter)))
                {
                    ResultDataModel result = draggedElement.Content as ResultDataModel;

                    object[] data = new object[]
                    {
                        draggedElement.Content as ResultDataModel,
                        _viewModel.PlusImageRight as Image,
                    };

                    _viewModel.AddTrackCommand.Execute(data);
                    draggedElement.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void MainScatterView_ContainerManipulationStarted(object sender, ContainerManipulationStartedEventArgs e)
        {
            FrameworkElement scatterViewItem = e.OriginalSource as FrameworkElement;
            ResultDataModel result = scatterViewItem.DataContext as ResultDataModel;
        }

        private void MainScatterView_ContainerManipulationDelta(object sender, ContainerManipulationDeltaEventArgs e)
        {
            FrameworkElement scatterViewItem = e.OriginalSource as FrameworkElement;
            ResultDataModel result = scatterViewItem.DataContext as ResultDataModel;
            double scaleFactor = e.ScaleFactor;
            double offset = 300.0;

            if (scatterViewItem.ActualWidth >= result.StdWidth && scatterViewItem.ActualHeight >= result.StdHeight)
            {
                result.Width = scatterViewItem.ActualWidth;
                result.Height = scatterViewItem.ActualHeight;

                if (scatterViewItem.ActualHeight > result.StdHeight + offset || scatterViewItem.ActualWidth > result.StdWidth + offset)
                {
                    if (!result.IsDetail)
                    {
                        result.IsDetail = true;
                        _viewModel.LoadDetailsCommand.Execute(result);
                    }
                }
                else
                {
                    result.IsDetail = false;
                }
            }
            else
            {
                scatterViewItem.Width = result.StdWidth;
                scatterViewItem.Height = result.StdHeight;
            }
        }

        private void MainScatterView_ContainerManipulationCompleted(object sender, ContainerManipulationCompletedEventArgs e)
        {
            FrameworkElement scatterViewItem = e.OriginalSource as FrameworkElement;
            ResultDataModel result = scatterViewItem.DataContext as ResultDataModel;

            if (e.ScaleFactor > 1.4)
            {
                result.IsDetail = true;
            }
        }
    }
}

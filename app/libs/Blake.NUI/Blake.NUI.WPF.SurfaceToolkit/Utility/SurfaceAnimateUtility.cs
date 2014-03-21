using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Blake.NUI.WPF.Utility;
using Microsoft.Surface.Presentation.Controls;

namespace Blake.NUI.WPF.SurfaceToolkit.Utility
{
    public class SurfaceAnimateUtility : AnimateUtility
    {
        #region Public Static Methods

        public static AnimationClock ThrowSVI(ScatterViewItem svi, Point targetPoint, double targetOrientation, double fromTime, double toTime, IEasingFunction ease = null)
        {
            AnimateElementPoint(svi, ScatterViewItem.CenterProperty,
                                                targetPoint, fromTime, toTime, ease);
            AnimationClock clock = AnimateElementDouble(svi, ScatterViewItem.OrientationProperty,
                                                targetOrientation, fromTime, toTime, ease);

            svi.ContainerActivated += svi_ContainerActivated;
            
            clock.Completed += (s, e) =>
            {
                svi.ContainerActivated -= svi_ContainerActivated;
            };

            return clock;
        }

        static void svi_ContainerActivated(object sender, RoutedEventArgs e)
        {
            ScatterViewItem svi = sender as ScatterViewItem;
            if (svi == null)
                return;

            StopAnimation(svi, ScatterViewItem.CenterProperty);
            StopAnimation(svi, ScatterViewItem.OrientationProperty);
        }

        #endregion

        protected SurfaceAnimateUtility()
        {
        }
    }
}

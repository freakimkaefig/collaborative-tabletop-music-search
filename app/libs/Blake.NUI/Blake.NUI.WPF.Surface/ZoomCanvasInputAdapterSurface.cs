using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Blake.NUI.WPF.Touch;

namespace Blake.NUI.WPF.Surface
{
    public class ZoomCanvasInputAdapterSurface : ZoomCanvasInputAdapterTouch
    {
        protected override bool GetIsManipulatorAcceptable(IManipulator manipulator)
        {
            SurfaceTouchDevice device = manipulator as SurfaceTouchDevice;

            if (device == null)
                return true;

            return device.Contact.IsFingerRecognized;
        }
    }
}

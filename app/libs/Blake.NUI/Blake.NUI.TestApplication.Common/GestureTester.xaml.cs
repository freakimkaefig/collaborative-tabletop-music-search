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
using Blake.NUI.WPF.Gestures;

namespace Blake.NUI.TestApplication.Common
{
    /// <summary>
    /// Interaction logic for GestureTester.xaml
    /// </summary>
    public partial class GestureTester : UserControl
    {
        public GestureTester()
        {
            InitializeComponent();
            Events.RegisterGestureEventSupport(this);
        }

        private void UpdateStatus(string gesture, object source)
        {
            var txt = String.Format("{0} {1} ({2})", gesture, source.GetType().Name, DateTime.Now.ToLongTimeString());
            lstOldGestures.Items.Insert(0, txt);
        }

        private void Grid_HoldGesture(object sender, WPF.Gestures.GestureEventArgs e)
        {
            UpdateStatus("Hold",e.OriginalSource);
        }

        private void Grid_DoubleTapGesture(object sender, WPF.Gestures.GestureEventArgs e)
        {
            UpdateStatus("Double Tap", e.OriginalSource);
        }

        private void Grid_TapGesture(object sender, WPF.Gestures.GestureEventArgs e)
        {
            UpdateStatus("Tap", e.OriginalSource);
        }
    }
}

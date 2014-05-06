using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Ctms.Applications.DataModels;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Ctms.Domain.Objects;
using System.Windows.Media.Imaging;

namespace Ctms.Presentation.Resources
{
    public partial class ResultStyleResources : ResourceDictionary
    {
        public ResultStyleResources()
        {
            InitializeComponent();
        }

        private void TabItem_TouchDown(object sender, TouchEventArgs e)
        {
            TabItem tab = sender as TabItem;
            TabControl control = tab.Parent as TabControl;
            control.SelectedItem = tab;
            e.Handled = true;
        }

        private void SurfaceButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

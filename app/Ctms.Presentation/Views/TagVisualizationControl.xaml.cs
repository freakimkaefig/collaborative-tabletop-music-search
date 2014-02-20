using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using Ctms.Applications.Views;
using System.ComponentModel.Composition;

namespace Ctms.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class TagVisualizationControl
    {
        public TagVisualizationControl()
        {
            InitializeComponent();
            Trace.WriteLine("Hello","Cat");
        }

        public void SimpleVisualization_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}

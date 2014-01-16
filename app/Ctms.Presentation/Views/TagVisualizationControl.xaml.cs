using System.Windows;
using System.Diagnostics;

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

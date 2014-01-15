using System.Windows.Controls;
using System.Windows;
using System.ComponentModel.Composition;
using Ctms.Applications.Views;

namespace Ctms.Presentation.Tags
{
   // [Export(typeof(ISearchView))]
    public partial class SimpleVisualization
    {
        public SimpleVisualization()
        {
            InitializeComponent();
        }


        public void SimpleVisualization_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}

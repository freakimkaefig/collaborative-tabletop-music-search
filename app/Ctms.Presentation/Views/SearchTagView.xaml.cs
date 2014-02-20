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
    [Export(typeof(ISearchTagView))]
    public partial class SearchTagView : ISearchTagView
    {
        public SearchTagView()
        {
            InitializeComponent();
            Trace.WriteLine("Hello","Cat");
        }

        public void DoSth()
        {

        }

        public void SimpleVisualization_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}

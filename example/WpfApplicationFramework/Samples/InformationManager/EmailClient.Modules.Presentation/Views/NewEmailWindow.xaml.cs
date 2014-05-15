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
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Waf.InformationManager.EmailClient.Modules.Applications.Views;

namespace Waf.InformationManager.EmailClient.Modules.Presentation.Views
{
    [Export(typeof(INewEmailView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class NewEmailWindow : Window, INewEmailView
    {
        public NewEmailWindow()
        {
            InitializeComponent();
            Loaded += LoadedHandler;
        }


        public void Show(object owner)
        {
            Owner = owner as Window;
            Show();
        }

        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
            toBox.Focus();
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            // The Send button is a toolbar button which doesn't take the focus. Move the focus
            // so that the Binding updates the source before calling the SendCommand.
            CommitChanges();
        }

        private static void CommitChanges()
        {
            FrameworkElement element = Keyboard.FocusedElement as FrameworkElement;
            if (element != null)
            {
                element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                element.Focus();
            }
        }
    }
}

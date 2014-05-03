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
using WPFKeyboard.Keyboard;

namespace WPFKeyboard.Sample
{
    /// <summary>
    /// Interaktionslogik für KeyboardSample.xaml
    /// </summary>
    public partial class KeyboardSample : UserControl, CustomKeyboardViewListener
    {

        private TextBox focusedElement;
        private KeyboardController keyboard;

        public KeyboardSample()
        {
            InitializeComponent();
            focusedElement = this.SampleTextBox;
            initKeyboard(); 
        }

        private void initKeyboard()
        {
            keyboard = this.CustomKeyboard;
            keyboard.setFocusedElement(focusedElement);

            //OPTIONAL
            keyboard.setCustomKeyboardListener(this);
        }

        /// <summary>
        /// only for demonstration
        /// </summary>
        /// <returns></returns>
        public TextBox getFocusedElement()
        {
            return focusedElement;
        }

        /// <summary>
        /// an element with no caret needed is focused
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotFocusableElement_TouchDown(object sender, TouchEventArgs e)
        {
            focusedElement = null;
            keyboard.setFocusedElement(focusedElement);
        }

        private void TextBox_TouchUp(object sender, TouchEventArgs e)
        {
            focusedElement = sender as TextBox;
            keyboard.setFocusedElement(focusedElement);
        }

        /*
         * COMPLETELY OPTIONAL -> only if you want to knwo which key was pressed
         */
        /// <summary>
        /// normal key/ new line/ space was pressed
        /// </summary>
        /// <param name="key">key (can be parsed in char) or "\n" or " "</param>
        public void typedKey(string key)
        {
            
        }

        /// <summary>
        /// backspace was typed
        /// </summary>
        public void typedBackSpace()
        {
            
        }

        /// <summary>
        /// one of the arrows was typed
        /// </summary>
        /// <param name="arrowIndex"> -1 for left-arrow; 1 for right arrow</param>
        public void typedArrow(int arrowIndex)
        {
            
        }
    }
}

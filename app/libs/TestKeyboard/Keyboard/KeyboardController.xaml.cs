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
using Microsoft.Surface.Presentation.Controls;

namespace WPFKeyboard.Keyboard
{
    /// <summary>
    /// Interaktionslogik für KeyboardController.xaml ---DEPRICATED---
    /// </summary>
    public partial class KeyboardController : UserControl, CustomKeyboardListener
    {
        /*
         * keyboard-variables
         */
        private CustomKeyboard keyboard;
        private Caret caret;
        
        /*
         * element on which the keyboard-input is currently binded
         * and its parent because position for caret is relative
         */
        private TextBox focusedElement;
        private Panel parentPanel;

        /*
         * listener to register what was pressed 
         * (if you need information about what was pressed)
         */
        CustomKeyboardViewListener listener;

        private Grid controllerGrid;

        public KeyboardController()
        {
            InitializeComponent();

            controllerGrid = this.MainGrid;
            this.CustomKeyboard.setCustomKeyboardListener(this);
        }

        private void initCaret(Panel relativeTo)
        {
            caret = new Caret(relativeTo);
            caret.update(focusedElement, "normal");
        }

        private void initAndSetKeyboard()
        {
            keyboard = new CustomKeyboard();
            controllerGrid.Children.Add(keyboard);
        }

        public void setCustomKeyboardListener(CustomKeyboardViewListener listener)
        {
            this.listener = listener;
        }

        public void setFocusedElement(UIElement focusedElement)
        {
            if (this.focusedElement == null && focusedElement as TextBox != null)
            {
                handleUpdateFocusedElement(focusedElement);
            }
            else if (this.focusedElement != null && this.focusedElement != focusedElement as TextBox && focusedElement != null)
            {
                handleUpdateFocusedElement(focusedElement);
            }
            else if (this.focusedElement != null && focusedElement == null)
            {
                removeRegisteredEventsFromElement();
                this.focusedElement = focusedElement as TextBox;
            }
            caret.update(this.focusedElement, "normal");
        }

        private void handleUpdateFocusedElement(UIElement newFocusedElement)
        {
            removeRegisteredEventsFromElement();
            this.focusedElement = newFocusedElement as TextBox;
            registerEventOnElement();
            parentPanel = findParentPanel(focusedElement);
            if (caret == null)
            {
                initCaret(parentPanel);
            }
            else
            {
                caret.setNewRelativeTo(parentPanel);
            } 
        }

        /*
         * remove events since there is a new element to "listen" to
         */
        private void removeRegisteredEventsFromElement()
        {
            if (focusedElement != null)
            {
                focusedElement.TouchUp -= TextElement_TouchUp;
                focusedElement.Loaded -= TextElement_Loaded;
                focusedElement.RemoveHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(TextElement_ScrollChanged));
            }  
        }

        /*
         * listen to specific events to update caret
         */
        private void registerEventOnElement()
        {
            if (focusedElement != null)
            {
                focusedElement.TouchUp += TextElement_TouchUp;
                focusedElement.Loaded += TextElement_Loaded;
                focusedElement.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(TextElement_ScrollChanged));
            } 
        }

        /*
         * search for the parent Panel in which the Textbox is located
         */
        private Panel findParentPanel(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            //parent is the end of the tree (it is the panel itself)
            if (parent == null) return null;

            Panel parentPanel = parent as Panel;
            if (parentPanel != null)
            {
                return parentPanel;
            }
            else
            {
                return findParentPanel(parent);
            }
        }


        /*
         * interface-method
         */
        public void keyPressed(SurfaceButton pressedButton)
        {
            //check if arrows were pressed
            if (pressedButton.Name == "ArrowLeft")
            {
                arrowWasTyped(-1);
            }
            else if (pressedButton.Name == "ArrowRight")
            {
                arrowWasTyped(+1);
            }
            // return-Key
            else if (pressedButton.Name == "BackSpace")
            {
                backSpaceWasTyped();
            }
            //normal letter-buttons
            else
            {
                string key = "" + pressedButton.Content;
                if (pressedButton.Name == "Enter")
                {
                    key = "\n";
                }
                normalKeyWasTyped(key);
            }
        }

        /*
         * normal key was typed
         */
        private void normalKeyWasTyped(string key)
        {
            if (focusedElement != null)
            {
                int caretPosition = focusedElement.CaretIndex;
                focusedElement.Text = focusedElement.Text.Insert(caretPosition, key);
                focusedElement.CaretIndex = caretPosition + 1;

                caret.update(focusedElement, "normal");
            }   
            //send message to listener
            if (listener != null)
            {
                listener.typedKey(key);
            }
        }

        /*
         * backspace was typed
         */
        private void backSpaceWasTyped()
        {
            if (focusedElement != null)
            {
                int caretPosition = focusedElement.CaretIndex;

                if (caretPosition > 0 && focusedElement.SelectionLength > 0)
                {
                    int startPosition = focusedElement.SelectionStart;
                    focusedElement.SelectedText = "";
                    focusedElement.CaretIndex = startPosition;
                }
                else if (caretPosition > 0)
                {
                    focusedElement.Text = focusedElement.Text.Remove(caretPosition - 1, 1);
                    focusedElement.CaretIndex = caretPosition - 1;
                }

                caret.update(focusedElement, "normal");
            }
            //send message to listener
            if (listener != null)
            {
                listener.typedBackSpace();
            }
        }

        /*
         * one of the arrow-keys was typed
         * --> arrowIndex: -1 : left arrow
         *                  1 : right arrow
         */
        private void arrowWasTyped(int arrowIndex)
        {
            if (focusedElement != null)
            {
                if (focusedElement.CaretIndex > 0 || arrowIndex > 0)
                {
                    focusedElement.CaretIndex += arrowIndex;
                }
                caret.update(focusedElement, "normal");
            }
            //send message to listener
            if (listener != null)
            {
                listener.typedArrow(arrowIndex);
            }
        }


        private void TextElement_TouchUp(object sender, TouchEventArgs e)
        {
            caret.update(focusedElement, "normal");
        }


        private void TextElement_Loaded(object sender, RoutedEventArgs e)
        {
            focusedElement.CaretIndex = focusedElement.Text.Length;
            caret.update(focusedElement, "normal");
        }

        private void TextElement_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            caret.update(focusedElement, "scroll");
        }
    }


    /*
     * interface for view (optional)
     */
    public interface CustomKeyboardViewListener
    {
        void typedKey(String key);
        void typedBackSpace();
        void typedArrow(int arrowIndex);
    }
}

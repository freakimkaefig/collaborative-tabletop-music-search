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
using Microsoft.Surface;
using System.Windows.Threading;
using Microsoft.Surface.Presentation.Input;

namespace WPFKeyboard.Keyboard
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class CustomKeyboard : UserControl
    {

        private char[] smallLetters = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'ä', 'ö', 'ü', 'ß', ',', '.', '?', '!' };
        private char[] bigLetters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'Ä', 'Ö', 'Ü', 'ß', ';', ':', '-', '"' };

        private char[] digitsAndSymbols = new char[] { '!', '(', '"', '$', '3', '%', '&', '+', '8', '?', '/', '}', '-', ')', '9', '0', '1', '4', '@', '5', '7', '\'', '2', '_', '*', '6', '#', '{', '\\', '>', ':', ';', '<', '=' };

        private bool areSmallLetters;
        private bool areLetters;

        /*
         * Timer
         */
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private bool timerIsRunning;
        private TimeSpan startInterval = TimeSpan.FromMilliseconds(500);
        private TimeSpan runningInterval = TimeSpan.FromMilliseconds(150);


        /*
         * KeyboardController
         */
        private CustomKeyboardListener keyBoardController;

        public CustomKeyboard()
        {
            InitializeComponent();
            areSmallLetters = true;
            areLetters = true;
            initDispatcherTimer();
        }

        private void initDispatcherTimer()
        {
            dispatcherTimer.Interval = startInterval;
            dispatcherTimer.Tick += new EventHandler(timer_Tick);
            timerIsRunning = false;
        }

        public void setCustomKeyboardListener(CustomKeyboardListener listener)
        {
            keyBoardController = listener;
        }


        private void Key_Pressed(object sender, RoutedEventArgs e)
        {
            SurfaceButton pressedButton = sender as SurfaceButton;
            List<SurfaceButton> buttons = getKeys();

            if (pressedButton.Name == "Shift1" || pressedButton.Name == "Shift2")
            {

                if (areSmallLetters)
                {
                    changeLetterSize(buttons, bigLetters);
                    areSmallLetters = false;
                }
                else
                {
                    changeLetterSize(buttons, smallLetters);
                    areSmallLetters = true;
                }
            }

            if (pressedButton.Name == "Symbols")
            {
                if (areLetters)
                {
                    changeLetterSize(buttons, digitsAndSymbols);
                    toogleShiftVisibility();
                    pressedButton.Content = "abc";
                    areLetters = false;
                }
                else
                {
                    if (areSmallLetters)
                    {
                        changeLetterSize(buttons, smallLetters);
                    }
                    else
                    {
                        changeLetterSize(buttons, bigLetters);
                    }
                    toogleShiftVisibility();
                    pressedButton.Content = "&123";
                    areLetters = true;
                }
            }

        }


        private void toogleShiftVisibility()
        {
            if (this.Shift1.Visibility == Visibility.Visible)
            {
                this.Shift1.Visibility = Visibility.Hidden;
                this.Shift2.Visibility = Visibility.Hidden;
            }
            else
            {
                this.Shift1.Visibility = Visibility.Visible;
                this.Shift2.Visibility = Visibility.Visible;
            }
        }

        /*
         * change Content of buttons
         */
        private void changeLetterSize(List<SurfaceButton> buttons, char[] keyArray)
        {
            string[] delimiter = new string[] { "Key" };
            SurfaceButton currentKey;
            int keyNumber;

            for (int i = 0; i < buttons.Count; i++)
            {
                currentKey = buttons[i];
                string[] result = currentKey.Name.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                if (!int.TryParse(result[0], out keyNumber))
                {
                    keyNumber = -1;
                }
                if (keyNumber >= 0)
                {
                    currentKey.Content = keyArray[keyNumber];
                }
            }
        }

        /*
         * get all children (all buttons)
         */
        private List<SurfaceButton> getKeys()
        {
            List<SurfaceButton> buttons = new List<SurfaceButton>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(this.KeyboardGrid); i++)
            {
                DependencyObject button = VisualTreeHelper.GetChild(this.KeyboardGrid, i);
                if (button != null && button is SurfaceButton)
                {
                    buttons.Add((SurfaceButton)button);
                }
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(this.SecondRow); i++)
            {
                DependencyObject button = VisualTreeHelper.GetChild(this.SecondRow, i);
                if (button != null && button is SurfaceButton)
                {
                    buttons.Add((SurfaceButton)button);
                }
            }

            return buttons;
        }


        /*
         * fires when a "normal" key is touched down --> send to focused Label
         */
        private void Keys_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            SurfaceButton pressedButton = sender as SurfaceButton;
            keyBoardController.keyPressed(pressedButton);

            //after shift was pressed -> letters go small automatically
            if (!areSmallLetters && areLetters)
            {
                changeLetterSize(getKeys(), smallLetters);
                areSmallLetters = true;
            }

            /*
             * handle DispatcherTimer-behaviour and interval
             */
            dispatcherTimer.Tag = pressedButton;
            dispatcherTimer.Stop();

            if (timerIsRunning)
            {
                dispatcherTimer.Interval = runningInterval;
            }
            timerIsRunning = true;
            dispatcherTimer.Start();
        }



        private void timer_Tick(object sender, EventArgs e)
        {
            Keys_PreviewTouchDown(dispatcherTimer.Tag, null);
        }

        private void Keys_PreviewTouchEnded(object sender, TouchEventArgs e)
        {
            /*
             * stop Timer
             */
            dispatcherTimer.Tag = null;
            dispatcherTimer.Stop();
            dispatcherTimer.Interval = startInterval;
            timerIsRunning = false;
        }


        /*
         * rects to make the fill the area for the buttons
         */
        private void Rect_TouchDown(object sender, TouchEventArgs e)
        {
            string[] delimiter = { "Rect" };
            string[] result = (sender as Rectangle).Name.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string keyCode = result[0];
            string keyPrefix = "Key";

            if (keyCode == "BackSpace" || keyCode == "Space" || keyCode == "ArrowLeft" || keyCode == "ArrowRight")
            {
                keyPrefix = "";
            }
            object button = this.KeyboardGrid.FindName(keyPrefix + keyCode);
            if (button != null)
            {
                Keys_PreviewTouchDown(button, null);
                e.Handled = true;
            }
        }

        private void Rect_TouchUp(object sender, TouchEventArgs e)
        {
            Keys_PreviewTouchEnded(null, e);
        }

        private void KeyboardGrid_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (!e.TouchDevice.GetIsFingerRecognized())
            {
                e.Handled = true;
            }
        }
    }

    /*
     * interface for controller
     */
    public interface CustomKeyboardListener
    {
        void keyPressed(SurfaceButton pressedButton);
    }
}

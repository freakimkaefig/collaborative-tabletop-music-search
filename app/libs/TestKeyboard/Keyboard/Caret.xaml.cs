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
using System.Windows.Threading;

namespace WPFKeyboard.Keyboard
{
    /// <summary>
    /// Interaktionslogik für Caret.xaml
    /// </summary>
    public partial class Caret : UserControl
    {
        /*
         * Timer
         */
        private DispatcherTimer dispatcherTimer;
        private double height;
        private double width;

        /*
         * relativeTo: caret-position within Textbox is relative, so the origin of Textbox is needed
         */
        private Panel relativeTo;
       

        public Caret(Panel relativeTo)
        {
            InitializeComponent();
            this.relativeTo = relativeTo;
            if (relativeTo != null)
            {
                relativeTo.Children.Add(this);
                checkPanel();
            }
            initCaret();
            base.VerticalAlignment = VerticalAlignment.Top;
            base.HorizontalAlignment = HorizontalAlignment.Left;
        }

        private void initCaret()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(500);
            dispatcherTimer.Tick += new EventHandler(timer_Tick);
            setCaretSize(30);
            startBlinking();    
        }

        /*
         * if relative-to UI-element has changed
         */
        public void setNewRelativeTo(Panel newRelativeTo)
        {
            if (newRelativeTo != null)
            {
                relativeTo.Children.Remove(this);
                relativeTo = newRelativeTo;
                relativeTo.Children.Add(this);
                checkPanel();
            } 
        }

        /*
         * updates caret if necessary (inits placing caret in the rigth position or hides caret)
         */
        public void update(TextBox focusedElement, string type)
        {
            if (focusedElement != null)
            {
                placeCaret(focusedElement, type);
            }
            else
            {
                stopBlinking();
            }

        }

        /*
         * if panel is a grid there must be a 
         * set the rowspan and coloumspan
         */
        private void checkPanel()
        {
            Grid panel = relativeTo as Grid;
            if (panel != null)
            {
                int rowSpan = panel.RowDefinitions.Count;
                int coloumnSpan = panel.ColumnDefinitions.Count;
                if (rowSpan > 0)
                {
                    Grid.SetRowSpan(this, rowSpan);
                }
                if (coloumnSpan > 0)
                {
                    Grid.SetColumnSpan(this, coloumnSpan);
                }
            }
        }

        /*
         * place caret to the right position
         * @params 
         * focusedElement: TextBox where caret shold be placed (must be Textbox because property 'caretIndex' is needed)
         * 
         */
        private void placeCaret(TextBox focusedElement, string type)
        {
            int caretPosition = focusedElement.CaretIndex;
            Point positionOfFocusedElement = focusedElement.TranslatePoint(new Point(0, 0), relativeTo);
            Rect caretPositionRect = focusedElement.GetRectFromCharacterIndex(caretPosition);
            if (!caretPositionRect.IsEmpty && isCaretVisible(focusedElement, caretPositionRect, type))
            {
                setCaretSize(caretPositionRect.Height);
                setCaretColor(focusedElement.Foreground);
                double posX = positionOfFocusedElement.X + caretPositionRect.X;
                double posY = positionOfFocusedElement.Y + caretPositionRect.Y;
                base.Margin = new Thickness(posX, posY, 0, 0);
                startBlinking();
            }
            else
            {
                stopBlinking();
            }
        }

        private void setCaretSize(double newHeight)
        {
            if (newHeight > 0)
            {
                height = newHeight;
                width = newHeight / 15;
                if (width < 2) width = 2;

                this.CaretRect.Width = width;
                this.CaretRect.Height = height;
            }  
        }

        private void setCaretColor(Brush color)
        {
            this.CaretRect.Fill = color;
        }

        /*
         * checks if current caret-position is visible
         */
        private bool isCaretVisible(TextBox focusedElement, Rect caretPositionRect, string type)
        {
            if (!(caretPositionRect.Y >= 0 && caretPositionRect.Y <= (focusedElement.ActualHeight - caretPositionRect.Height) &&
                (caretPositionRect.X >= 0 && caretPositionRect.X <= (focusedElement.ActualWidth - caretPositionRect.Width))))
            {
                if (type != "scroll")
                {
                    focusedElement.ScrollToLine(focusedElement.GetLineIndexFromCharacterIndex(focusedElement.CaretIndex));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void startBlinking()
        {
            dispatcherTimer.Start();
        }

        private void stopBlinking()
        {
            this.CaretRect.Visibility = Visibility.Hidden;
            dispatcherTimer.Stop();
        }

        /*
         * is called from DispatcherTimer in interval 500ms
         */
        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.CaretRect.Visibility == Visibility.Hidden)
            {
                this.CaretRect.Visibility = Visibility.Visible;
            }
            else
            {
                this.CaretRect.Visibility = Visibility.Hidden;
            }
        }

        
    }
}

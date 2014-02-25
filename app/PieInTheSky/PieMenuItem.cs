using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PieInTheSky
{
    public class PieMenuItem : HeaderedItemsControl
    {
        public event RoutedEventHandler Click;

        double _size;

        // Dependency properties.  Are set in xaml pie menu element.
        public static readonly DependencyProperty SubMenuSectorProperty;
        public static readonly DependencyProperty SectorRadiusProperty;
        public static readonly DependencyProperty CommandProperty;
        public static readonly DependencyProperty CenterTextProperty;
        public static readonly DependencyProperty SubHeaderProperty;
        public static readonly DependencyProperty IdProperty;


        #region Properties

        [Bindable(true)]
        public double SubMenuSector
        {
            get
            {
                return (double)base.GetValue(PieMenuItem.SubMenuSectorProperty);
            }
            set
            {
                base.SetValue(PieMenuItem.SubMenuSectorProperty, value);
            }
        }

        [Bindable(true)]
        public double SectorRadius
        {
            get
            {
                return (double)base.GetValue(PieMenuItem.SectorRadiusProperty);
            }
            set
            {
                base.SetValue(PieMenuItem.SectorRadiusProperty, value);
            }
        }

        [Bindable(true)]
        public ICommand Command
        {
            get
            {
                return (ICommand)base.GetValue(PieMenuItem.CommandProperty);
            }
            set
            {
                base.SetValue(PieMenuItem.CommandProperty, value);
            }
        }

        [Bindable(true)]
        public bool CenterText
        {
            get
            {
                return (bool)base.GetValue(PieMenuItem.CenterTextProperty);
            }
            set
            {
                base.SetValue(PieMenuItem.CenterTextProperty, value);
            }
        }

        [Bindable(true)]
        public string SubHeader
        {
            get
            {
                return (string)base.GetValue(PieMenuItem.SubHeaderProperty);
            }
            set
            {
                base.SetValue(PieMenuItem.SubHeaderProperty, value);
            }
        }

        [Bindable(true)]
        public int Id
        {
            get
            {
                return (int)base.GetValue(PieMenuItem.IdProperty);
            }
            set
            {
                base.SetValue(PieMenuItem.IdProperty, value);
            }
        }


        #endregion Properties


        static PieMenuItem()
        {
            PieMenuItem.SubMenuSectorProperty = DependencyProperty.Register("SubMenuSector", typeof(double), typeof(PieMenuItem), new FrameworkPropertyMetadata(120.0));
            PieMenuItem.CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(PieMenuItem), new FrameworkPropertyMetadata(null));
            PieMenuItem.CenterTextProperty = DependencyProperty.Register("CenterText", typeof(bool), typeof(PieMenuItem), new FrameworkPropertyMetadata(false));
            PieMenuItem.SubHeaderProperty = DependencyProperty.Register("SubHeader", typeof(string), typeof(PieMenuItem), new FrameworkPropertyMetadata(""));
            PieMenuItem.IdProperty = DependencyProperty.Register("Id", typeof(int), typeof(PieMenuItem), new FrameworkPropertyMetadata(0));
        }

        public double CalculateSize(double s, double d)
        {
            // size of current level 
            double ss = s + d;

            foreach (UIElement i in Items)
            {
               ss = Math.Max(ss, (i as PieMenuItem).CalculateSize(s + d, d));
            }

            _size = ss;
    
            return _size;
        }

        protected override Size MeasureOverride(Size availablesize)
        {
            foreach (UIElement i in Items) 
            {
                i.Measure(availablesize);
            }

            return new Size(_size, _size);
        }

        protected override Size ArrangeOverride(Size finalsize)
        {
            return finalsize;
        }

        public void OnClick()
        {
            if (Command != null && Command.CanExecute(null))
            {
                //Command.Execute(Header);
                Command.Execute(Id);
            }

            if (Click != null)
            {
                Click(this, new RoutedEventArgs());
            }
        }
    }
}

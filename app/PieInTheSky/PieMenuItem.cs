using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;

namespace PieInTheSky
{
    public class PieMenuItem : HeaderedItemsControl, INotifyPropertyChanged
    {
        public event RoutedEventHandler Click;

        double _size;

        // Dependency properties.  Are set in xaml pie menu element.
        public static readonly DependencyProperty SubMenuSectorProperty;
        public static readonly DependencyProperty SectorRadiusProperty;
        public static readonly DependencyProperty CommandProperty;
        public static readonly DependencyProperty CenterTextVerticalProperty;
        public static readonly DependencyProperty CenterTextHorizontalProperty;
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
        public bool CenterTextVertically
        {
            get
            {
                return (bool)base.GetValue(PieMenuItem.CenterTextVerticalProperty);
            }
            set
            {
                base.SetValue(PieMenuItem.CenterTextVerticalProperty, value);
            }
        }

        [Bindable(true)]
        public bool CenterTextHorizontal
        {
            get
            {
                return (bool)base.GetValue(PieMenuItem.CenterTextHorizontalProperty);
            }
            set
            {
                base.SetValue(PieMenuItem.CenterTextHorizontalProperty, value);
            }
        }
        
        [Bindable(true)]
        public string SubHeader
        {
            get
            {
                var value = (string)base.GetValue(PieMenuItem.SubHeaderProperty);
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
            PieMenuItem.CenterTextVerticalProperty = DependencyProperty.Register("CenterTextVertical", typeof(bool), typeof(PieMenuItem), new FrameworkPropertyMetadata(false));
            PieMenuItem.CenterTextHorizontalProperty = DependencyProperty.Register("CenterTextHorizontal", typeof(bool), typeof(PieMenuItem), new FrameworkPropertyMetadata(false));
            PieMenuItem.IdProperty = DependencyProperty.Register("Id", typeof(int), typeof(PieMenuItem), new FrameworkPropertyMetadata(0));

            //PieMenuItem.SubHeaderProperty = DependencyProperty.Register("SubHeader", typeof(string), typeof(PieMenuItem), new FrameworkPropertyMetadata(""));
            
            //PieMenuItem.SubHeaderProperty = DependencyProperty.Register("SubHeader", typeof(string), typeof(PieMenuItem),
            //    new UIPropertyMetadata(string.Empty, DoSth));
            
            PieMenuItem.SubHeaderProperty = DependencyProperty.Register("SubHeader", typeof(string), typeof(PieMenuItem), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender));
             
            
            //PieMenuItem.SubHeaderProperty = DependencyProperty.Register(
            //    "SubHeader",  typeof(string), typeof(PieMenuItem),  new FrameworkPropertyMetadata("", (FrameworkPropertyMetadataOptions.AffectsRender), new PropertyChangedCallback(DoSth)));
        
            //PieMenuItem.SubHeaderProperty = DependencyProperty.Register(
            //    "SubHeader",  typeof(string), typeof(PieMenuItem),  
            //    new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        
        }

        private static void UsernamePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Debug.Print("OldValue: {0}", e.OldValue);
            //Debug.Print("NewValue: {0}", e.NewValue);


            //Command.Execute(0);
        }

        public static void DoSth(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = (string)e.NewValue;
            var oldValue = (string)e.OldValue;
            ((PieMenuItem)d).SetValue(SubHeaderProperty, value);
            ((PieMenuItem)d).UpdateLayout();
            /*
            var p = (PieMenuItem)d;
            var be = p.GetBindingExpression(PieMenuItem.SubHeaderProperty);
            //be.UpdateSource();
            be.UpdateTarget();
            ((PieMenuItem)d).GetBindingExpression(PieMenuItem.SubHeaderProperty).UpdateTarget();
            //var val = ((PieMenuItem)d).ReadLocalValue(SubHeaderProperty);*/
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
            if (Command != null && Command.CanExecute(null))//!!
            {
                Command.Execute(Id);
            }

            if (Click != null)
            {
                Click(this, new RoutedEventArgs());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string sProp)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(sProp));
            }
        }
    }
}

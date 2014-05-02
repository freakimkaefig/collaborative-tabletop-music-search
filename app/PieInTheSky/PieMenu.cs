using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Collections;
using Ctms.Domain.Objects;
//using Ctms.Domain.Objects;

namespace PieInTheSky
{
    public class PieMenu : ItemsControl
    {
        #region DependencyProperties

        // Dependency properties. Are set in xaml pie menu element.
        public static readonly DependencyProperty RadiusProperty;
        public static readonly DependencyProperty InnerRadiusProperty;
        public static readonly DependencyProperty SectorGapProperty;
        public static readonly DependencyProperty GapProperty;
        public static readonly DependencyProperty MenuSectorProperty;
        public static readonly DependencyProperty SelectedBackgroundProperty;
        public static readonly DependencyProperty RotationProperty;
        public static readonly DependencyProperty RotateTextProperty;
        public static readonly DependencyProperty RotateTextAngleProperty;
        public static readonly DependencyProperty AdditionalTextProperty;

        // Variables providing dependency property values

        [Bindable(true)]
        public double Radius
        { 
            get
            {
                return (double)base.GetValue(PieMenu.RadiusProperty);
            }
            set
            {
                base.SetValue(PieMenu.RadiusProperty, value);
            }
        }

        [Bindable(true)]
        public double InnerRadius
        {
            get
            {
                return (double)base.GetValue(PieMenu.InnerRadiusProperty);
            }
            set
            {
                base.SetValue(PieMenu.InnerRadiusProperty, value);
            }
        }

        [Bindable(true)]
        public double SectorGap
        {
            get
            {
                return (double)base.GetValue(PieMenu.SectorGapProperty);
            }
            set
            {
                base.SetValue(PieMenu.SectorGapProperty, value);
            }
        }

        [Bindable(true)]
        public double Gap
        {
            get
            {
                return (double)base.GetValue(PieMenu.GapProperty);
            }
            set
            {
                base.SetValue(PieMenu.GapProperty, value);
            }
        }

        [Bindable(true)]
        public double MenuSector
        {
            get
            {
                return (double)base.GetValue(PieMenu.MenuSectorProperty);
            }
            set
            {
                base.SetValue(PieMenu.MenuSectorProperty, value);
            }
        }

        [Bindable(true)]
        public Brush SelectedBackground
        {
            get 
            {
                return (Brush)base.GetValue(PieMenu.SelectedBackgroundProperty);
            }
            set
            {
                base.SetValue(PieMenu.SelectedBackgroundProperty, value);
            }
        }

        [Bindable(true)]
        public double Rotation
        {
            get
            {
                return (double)base.GetValue(PieMenu.RotationProperty);
            }
            set
            {
                base.SetValue(PieMenu.RotationProperty, value);
            }
        }

        [Bindable(true)]
        public bool RotateText
        {
            get
            {
                return (bool)base.GetValue(PieMenu.RotateTextProperty);
            }
            set
            {
                base.SetValue(PieMenu.RotateTextProperty, value);
            }
        }

        [Bindable(true)]
        public double RotateTextAngle
        {
            get
            {
                return (double)base.GetValue(PieMenu.RotateTextAngleProperty);
            }
            set
            {
                base.SetValue(PieMenu.RotateTextAngleProperty, value);
            }
        }

        [Bindable(true)]
        public string AdditionalText
        {
            get
            {
                return (string)base.GetValue(PieMenu.AdditionalTextProperty);
            }
            set
            {
                base.SetValue(PieMenu.AdditionalTextProperty, value);
            }
        }

        #endregion DependencyProperties


        #region fields

        private List<List<Tuple<double, double>>> _sectors;
        private Selection _selection = new Selection();

        private int _current_level = -1;
        private int _current_selection = -1;
        private bool _was_selected = false;
        private double _size;
        public DrawingContext DrawingContext { get; set; }
        public ItemsControl ItemsControl { get; set; }

        #endregion fields


        #region constructor

        static PieMenu()
        {
            // Reference dependency properties with default values to class properties
            PieMenu.RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(PieMenu), new FrameworkPropertyMetadata(50.0));
            PieMenu.InnerRadiusProperty = DependencyProperty.Register("InnerRadius", typeof(double), typeof(PieMenu), new FrameworkPropertyMetadata(10.0));
            PieMenu.SectorGapProperty = DependencyProperty.Register("SectorGap", typeof(double), typeof(PieMenu), new FrameworkPropertyMetadata(5.0));
            PieMenu.GapProperty = DependencyProperty.Register("Gap", typeof(double), typeof(PieMenu), new FrameworkPropertyMetadata(5.0));
            PieMenu.MenuSectorProperty = DependencyProperty.Register("MenuSector", typeof(double), typeof(PieMenu), new FrameworkPropertyMetadata(360.0));
            PieMenu.SelectedBackgroundProperty = DependencyProperty.Register("SelectedBackground", typeof(Brush), typeof(PieMenu), new FrameworkPropertyMetadata(Brushes.Gray));
            PieMenu.RotationProperty = DependencyProperty.Register("Rotation", typeof(double), typeof(PieMenu), new FrameworkPropertyMetadata(0.0));
            PieMenu.RotateTextProperty = DependencyProperty.Register("RotateText", typeof(bool), typeof(PieMenu), new FrameworkPropertyMetadata(true));
            PieMenu.RotateTextAngleProperty = DependencyProperty.Register("RotateTextAngle", typeof(double), typeof(PieMenu), new FrameworkPropertyMetadata(90.0));
            PieMenu.AdditionalTextProperty = DependencyProperty.Register("AdditionalText", typeof(string), typeof(PieMenu), new FrameworkPropertyMetadata(""));
        }

        #endregion constructor


        #region overriding properties

        protected override Size MeasureOverride(Size availablesize)
        {
            // The "thickness" of each "layer" of menu
            double d = 2.0 * (Radius - InnerRadius + Gap);

            // fictious size of "empty" menu
            double s = 2.0 * (InnerRadius - Gap);

            // find size as maximum size of menu items
            double ss = 0;

            foreach (UIElement i in Items)
            {
                ss = Math.Max(ss, (i as PieMenuItem).CalculateSize(s, d));
            }
            
            foreach (UIElement i in Items)
            {
                i.Measure(availablesize);
            }

            _size = ss;

            return new Size(ss, ss);
        }

        public void Render()
        {
            OnRender(DrawingContext);
        }

        protected override Size ArrangeOverride(Size finalsize)
        {
            return finalsize;
        }

        #endregion overriding properties


        #region event methods

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            Point click_point = e.MouseDevice.GetPosition(this);
           
            Down(click_point);

            e.MouseDevice.Capture(this);

            InvalidateVisual();
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            Point point = e.MouseDevice.GetPosition(this);

            Up(point);

            // release mouse device
            e.MouseDevice.Capture(null);

            InvalidateVisual();
        }

        protected override void OnTouchDown(System.Windows.Input.TouchEventArgs e)
        {
            base.OnTouchDown(e);

            Point touch_point = e.TouchDevice.GetTouchPoint(this).Position;

            Down(touch_point);

            e.TouchDevice.Capture(this);

            e.Handled = true;

            InvalidateVisual();
        }

        protected override void OnTouchUp(System.Windows.Input.TouchEventArgs e)
        {
            base.OnTouchUp(e);

            Point point = e.TouchDevice.GetTouchPoint(this).Position;
            
            Up(point);
            
            // release touch device
            e.TouchDevice.Capture(null);

            e.Handled = true;

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawingContext = drawingContext;

            var maxRotation = 360.0;//!!auf 180.0?

            // "normalize" rotation to avoid problems elsewhere
            double rotation = this.Rotation;
            while (rotation < 0.0) rotation += maxRotation;
            while (rotation > maxRotation) rotation -= maxRotation;

            // initialize data structure for remembering sectors at each level
            _sectors = new List<List<Tuple<double, double>>>();

            ItemsControl = this;

            // Draw the menu (level 0)
            DrawCircle(this, 0, rotation, this.MenuSector, drawingContext);
        }

        #endregion event methods

        #region Touch handling methods

        private void Down(Point point)
        {
            // find the menu item on which the point is
            Tuple<int, int> t = FindMenuItem(point);
            int level = t.Item1;
            int selection = t.Item2;

            if (selection != -1)
            {
                // remember the selection
                _current_level = level;
                _current_selection = selection;

                if (selection != _selection.GetSelection(level))
                {
                    // select if it is not already selected
                    _selection.SetSelection(level, selection);
                    _was_selected = false;
                }
                else
                {
                    // remember that is was already selected
                    _was_selected = true;
                }
            }
        }

        private void Up(Point point)
        {
            if (_current_level == -1 || _current_selection == -1) return;

            // find the menu item on which the (release) point is
            Tuple<int, int> t = FindMenuItem(point);
            int level = t.Item1;
            int selection = t.Item2;

            if (level == _current_level && selection == _current_selection)
            {
                // release happens in same menu item as the current selection

                // Find menu item and trigger actions
                ItemsControl items_control = this;
                for (int i = 0; i <= level; i++)
                {
                    int sel = _selection.GetSelection(i);
                   
                    items_control = (ItemsControl)items_control.Items[sel];
                }
                (items_control as PieMenuItem).OnClick();

                if (items_control.Items.Count == 0)
                {
                    // If no children (means it is a leaf and we have come to our end), then deselect whole menu
                    _selection.SetSelection(0, -1);
                }
                else if (_was_selected)
                {
                    // deselect if it was already selected
                    _selection.SetSelection(level, -1);
                }
            }
            else
            {
                // release happens somewhere else than pressdown

                // Deselect and else do nothing
                _selection.SetSelection(_current_level, -1);
            }

            // reset current selection
            _current_level = -1;
            _current_selection = -1;
            _was_selected = false;
        }

        private Tuple<int, int> FindMenuItem(Point point)
        {
            // coordinates of point relative to center
            double x = point.X - _size / 2.0;
            double y = point.Y - _size / 2.0;

            // distance from center
            double m = Math.Sqrt(x * x + y * y);

            // find the level in the menu of the part clicked 
            int level = 0;
            double l = Radius;

            // a small buffer just to avoid getting a level that is not there
            double g = (Gap > 0 ? Gap / 2.0 : 1);
            while (m >= l + g)
            {
                level++;
                l += Gap + Radius - InnerRadius;
            }

            if (level >= _sectors.Count)
            {
                // point is outside higest level of visible menu
                return new Tuple<int, int>(-1, -1);
            }

            // angle in radians
            double theta = Math.Acos(Math.Abs(x / m));

            // convert angle to degrees
            double angle = 0;
            if (x >= 0 && y >= 0) angle = theta * (360.0 / (2.0 * Math.PI));
            else if (x >= 0 && y < 0) angle = 360.0 - theta * (360.0 / (2.0 * Math.PI));
            else if (x < 0 && y >= 0) angle = 180.0 - theta * (360.0 / (2.0 * Math.PI));
            else if (x < 0 && y < 0) angle = 180 + theta * (360.0 / (2.0 * Math.PI));

            // run through the visible sectors at present level to see in which sector the angle belongs
            // must consider the possibility that angles defining sectors might be less that 0 and greater than 360
            // if rotation is less than -180 or greater than 360 there may be some problems, but this is handled in initial call to DrawMenu

            int selection = -1;

            for (int i = 0; i < _sectors[level].Count; i++)
            {
                double a1 = _sectors[level][i].Item1;
                double a2 = _sectors[level][i].Item2;

                // we have as invariant a1 < a2

                if (a1 < 0.0 && a2 >= 0.0)
                {
                    if (a1 + 360.0 <= angle || angle <= a2)
                    {
                        selection = i;
                        break;
                    }
                }
                else if (a1 < 0.0 && a2 < 0.0)
                {
                    if (a1 + 360.0 <= angle && angle <= a2 + 360.0)
                    {
                        selection = i;
                        break;
                    }
                }
                else if (a1 > 360.0)
                {
                    if (a1 - 360.0 <= angle && angle <= a2 - 360.0)
                    {
                        selection = i;
                        break;
                    }
                }
                else if (a2 > 360.0)  // Must have: 0.0 <= a1 <= 360.0
                {
                    if (a1 <= angle || angle <= a2 - 360.0)
                    {
                        selection = i;
                        break;
                    }
                }
                else // 0.0 <= a1 <= 360.0 && 0.0 <= a2 <= 360.0
                {
                    if (a1 <= angle && angle <= a2)
                    {
                        selection = i;
                        break;
                    }
                }
            }

            return new Tuple<int, int>(level, selection);
        }

        #endregion Touch handling methods


        #region Display methods

        private void DrawCircle(ItemsControl items_control, int level, double angle, double sector, DrawingContext drawingContext)
        {
            // Make sure sector is limited to available place
            double full_sector = Math.Min(sector, 360.0);

            // find number of menu items at current level
            // return if none
            int count = items_control.Items.Count;
            if (count == 0) return;

            // Add list to remember sectors at this level
            _sectors.Add(new List<Tuple<double, double>>());

            // Coordinates of center of (full) menu
            double x = _size / 2.0;
            double y = _size / 2.0;

            // calculate inner and outer radius for this level
            double inner_radius = InnerRadius + (Radius - InnerRadius + Gap) * level;
            if (inner_radius < SectorGap) inner_radius = SectorGap;
            double outer_radius = Radius + (Radius - InnerRadius + Gap) * level;

            // Sector gap is given as a length. Find the angle giving this gap on inner and outer radius
            double inner_gap_angle;
            if (inner_radius == 0) inner_gap_angle = 0;
            else inner_gap_angle = 2.0 * Math.Asin(SectorGap / (2.0 * inner_radius)) * 360.0 / (2.0 * Math.PI);
            double outer_gap_angle = 2.0 * Math.Asin(SectorGap / (2.0 * outer_radius)) * 360.0 / (2.0 * Math.PI);

            // numbers of sector gaps (one more if we are using full circle)
            int c = (full_sector < 360.0 ? count - 1 : count);

            // Calculate the inner and outer arc of each menu item
            double inner_angle = (full_sector - inner_gap_angle * c) / count;
            double outer_angle = (full_sector - outer_gap_angle * c) / count;

            // draw each menu item 
            for (int i = 0; i < count; i++)
            {
                // calculate the boundaries of menu item as angle of the inner and outer arcs
                double start_inner_angle = angle + i * (full_sector / count) + inner_gap_angle / 2.0;
                double end_inner_angle = start_inner_angle + (full_sector / count) - inner_gap_angle;
                double start_outer_angle = angle + i * (full_sector / count) + outer_gap_angle / 2.0;
                double end_outer_angle = start_outer_angle + (full_sector / count) - outer_gap_angle;

                // remeber the boundaries (as sector angles) of the menu item
                _sectors[level].Add(new Tuple<double, double>(start_outer_angle, end_outer_angle));
               
                // Calculate the corners of the sector
                Point p1 = CalculatePoint(x, y, start_inner_angle, inner_radius);
                Point p2 = CalculatePoint(x, y, end_inner_angle, inner_radius);
                Point p3 = CalculatePoint(x, y, end_outer_angle, outer_radius);
                Point p4 = CalculatePoint(x, y, start_outer_angle, outer_radius);

                // find center of the corners 
                Point center = CalculatePoint(x, y, (start_inner_angle + end_inner_angle) / 2.0, (inner_radius + outer_radius) / 2.0);

                // specify the figure representing the menu item
                PathFigure pathFigure = new PathFigure();
                pathFigure.Segments = new PathSegmentCollection();
                pathFigure.StartPoint = p1;
                pathFigure.Segments.Add(new ArcSegment(p2, new Size(inner_radius, inner_radius), end_inner_angle - start_inner_angle, end_inner_angle - start_inner_angle > 180.0, SweepDirection.Clockwise, true));
                pathFigure.Segments.Add(new LineSegment(p3, true));
                pathFigure.Segments.Add(new ArcSegment(p4, new Size(outer_radius, outer_radius), end_outer_angle - start_outer_angle, end_outer_angle - start_outer_angle > 180.0, SweepDirection.Counterclockwise, true));
                pathFigure.IsClosed = true;
                pathFigure.IsFilled = true;

                // Create geometry object and add the figure
                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(pathFigure);

                // Get the menu item to extract properties
                PieMenuItem menu_item = items_control.Items[i] as PieMenuItem;               
                
                // find color for backgound and border 
                Brush background_brush = menu_item.Background;
                if (background_brush == null) background_brush = this.Background;
               
                if (_selection.GetSelection(level) == i) 
                {
                    background_brush = this.SelectedBackground;
                }
                
                //var background_brush = Brushes.Transparent;
                var border_brush = menu_item.BorderBrush;

                // Draw the geometry representing the menu item
                drawingContext.DrawGeometry(background_brush, new Pen(border_brush, menu_item.BorderThickness.Left), geometry);

                // Get header of menu item as string and make a formatted text based on properties of menu item
                var header = ((String)menu_item.Header);
                var subHeader = ((String)menu_item.SubHeader);

                header      = header == null ? "" : header;
                subHeader   = subHeader == null ? "" : subHeader;

                //if (i == 0) menu_item.Foreground = (Brush)(new BrushConverter().ConvertFrom("#f00"));
                //if (i == count-1) menu_item.Foreground = (Brush)(new BrushConverter().ConvertFrom("#00f"));

                FormattedText headerText = new FormattedText(header,
                                CultureInfo.CurrentCulture,
                                FlowDirection.LeftToRight,
                                new Typeface(menu_item.FontFamily, menu_item.FontStyle, menu_item.FontWeight, menu_item.FontStretch),
                                menu_item.FontSize,
                                menu_item.Foreground);

                FormattedText subHeaderText = new FormattedText(subHeader,
                                CultureInfo.CurrentCulture,
                                FlowDirection.LeftToRight,
                                new Typeface(menu_item.FontFamily, menu_item.FontStyle, menu_item.FontWeight, menu_item.FontStretch),
                                menu_item.FontSize,
                                menu_item.Foreground);

                var boxLength = 0.0;
                if (RotateTextAngle == -90.0 || RotateTextAngle == 270.0)
                {   // text is displayed viewing away from the center
                    //!! Länge aus Umfang berechnen?
                    boxLength = 120.0;
                }
                else
	            {
                    boxLength = Radius - InnerRadius;
	            }

                // calculate maximum width for text and cut too long texts with "..."
                headerText.MaxTextWidth = boxLength;
                headerText.MaxTextHeight = menu_item.FontSize + 10;
                headerText.Trimming = TextTrimming.CharacterEllipsis;

                subHeaderText.MaxTextWidth = boxLength;
                subHeaderText.MaxTextHeight = menu_item.FontSize + 10;
                subHeaderText.Trimming = TextTrimming.CharacterEllipsis;

                var bottomMargin = 0.0;
                var innerMargin  = 0.0;

                var headerTextPosX = 0.0;
                var subTextPosX = 0.0;

                if (menu_item.CenterTextVertically == true)
                {
                    bottomMargin = 10.0;
                }

                if (menu_item.CenterTextHorizontal == true)
                {   // center text horizontally
                    headerTextPosX  = center.X - headerText.Width / 2.0;
                    subTextPosX     = center.X - subHeaderText.Width / 2.0;
                }
                else
                {   // left-align text
                    headerTextPosX = center.X - (Radius / 2.0) + InnerRadius + innerMargin;
                    subTextPosX = center.X - (Radius / 2.0) + InnerRadius + innerMargin;
                }

                var textDistance = 18.0;

                // Calculate placement of text in  center
                Point headerTextPoint = new Point(headerTextPosX, center.Y - headerText.Height / 2.0 + textDistance / 2.0 - bottomMargin);
                Point subTextPoint = new Point(subTextPosX, center.Y - subHeaderText.Height / 2.0 - textDistance / 2.0 - bottomMargin);

                // Draw the text as name of menu item
                // Added rotation angle adjustment of text
                var defaultTextRotation = 90.0;
                if (this.RotateTextAngle != 90.0) defaultTextRotation = this.RotateTextAngle;

                if (this.RotateText) drawingContext.PushTransform(new RotateTransform((start_inner_angle + end_inner_angle) / 2.0 + defaultTextRotation, center.X, center.Y));
                drawingContext.DrawText(headerText, headerTextPoint);
                drawingContext.DrawText(subHeaderText, subTextPoint);
                if (this.RotateText) drawingContext.Pop();
            }
        }        

        private Point CalculatePoint(double centerX, double centerY, double angle, double radius)
        {
            double x = centerX + Math.Cos((Math.PI / 180.0) * angle) * radius;
            double y = centerY + Math.Sin((Math.PI / 180.0) * angle) * radius;
            return new Point(x, y);
        }

        #endregion Display methods
    }

    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace SurLaRoute.Rooms
{
    class Room: Shape
    {
  
        private double x;
        private double y;
        private Boolean active;
        public readonly Color BACKGROUND_COLOR = Color.FromArgb(255,200,200,200);
        public readonly Color ACTIVE_COLOR = Color.FromArgb(255,255,0,255);
        private Canvas parent;

        public Room(int x, int y, double width, double height, Canvas parent)
        {
            this.Fill = new SolidColorBrush(BACKGROUND_COLOR);
            this.x = x;
            this.y = y;
            this.parent = parent;
            this.Width = width;
            this.Height = height;

            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new RectangleGeometry(new Rect(0,0,Width,Height));
            }
        }

        public void setActive()
        {
            active = true;
            this.Fill = new SolidColorBrush(ACTIVE_COLOR);
        }

        public void setPassive()
        {
            active = false;
            this.Fill = new SolidColorBrush(BACKGROUND_COLOR);
        }

    }
}

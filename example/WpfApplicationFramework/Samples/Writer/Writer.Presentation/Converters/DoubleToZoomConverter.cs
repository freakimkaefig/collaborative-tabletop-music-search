using System;
using System.Globalization;
using System.Windows.Data;

namespace Waf.Writer.Presentation.Converters
{
    public class DoubleToZoomConverter : IValueConverter
    {
        private static readonly DoubleToZoomConverter defaultInstance = new DoubleToZoomConverter();

        public static DoubleToZoomConverter Default { get { return defaultInstance; } }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value) * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value) / 100;
        }
    }
}

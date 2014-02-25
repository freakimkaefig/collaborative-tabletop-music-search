using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace Waf.Writer.Presentation.Converters
{
    public class PercentConverter : IValueConverter
    {
        private static readonly PercentConverter defaultInstance = new PercentConverter();

        public static PercentConverter Default { get { return defaultInstance; } }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format(culture, "{0:P0}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            double d;
            if (double.TryParse(((string)value).Replace(culture.NumberFormat.PercentSymbol, ""),
                NumberStyles.Float | NumberStyles.AllowThousands, culture, out d))
            {
                return d / 100d;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace Waf.BookLibrary.Library.Presentation.Converters
{
    public class StringToUriConverter : IValueConverter
    {
        private static readonly StringToUriConverter defaultInstance = new StringToUriConverter();

        public static StringToUriConverter Default { get { return defaultInstance; } }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Uri uri;
            if (Uri.TryCreate(value as string ?? "", UriKind.RelativeOrAbsolute, out uri))
            {
                return uri;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            if (s != null) { return s; }

            return ((Uri)value).OriginalString;
        }
    }
}

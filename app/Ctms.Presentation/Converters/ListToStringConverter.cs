using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace Ctms.Presentation.Converters
{
    public class ListToStringConverter : IValueConverter
    {
        private static readonly ListToStringConverter defaultInstance = new ListToStringConverter();

        public static ListToStringConverter Default { get { return defaultInstance; } }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(String))
                throw new InvalidOperationException("The target must be a String");

            return String.Join(", ", ((List<String>)value).ToArray());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

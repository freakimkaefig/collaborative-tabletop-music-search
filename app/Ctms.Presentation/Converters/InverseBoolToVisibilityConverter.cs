using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ctms.Presentation.Converters
{
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        private static readonly InverseBoolToVisibilityConverter defaultInstance = new InverseBoolToVisibilityConverter();

        public static InverseBoolToVisibilityConverter Default { get { return defaultInstance; } }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
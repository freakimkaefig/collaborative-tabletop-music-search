using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Ctms.Presentation.Converters
{
    class BoolToStateConverter : IValueConverter
    {
        private static readonly BoolToStateConverter defaultInstance = new BoolToStateConverter();

        public static BoolToStateConverter Default { get { return defaultInstance; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "#00FFFF" : "#222222";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

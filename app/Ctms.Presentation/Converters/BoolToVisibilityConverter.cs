using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Ctms.Presentation.Converters
{
    //http://wpftutorial.net/ValueConverters.html
    //Converter which simplifies use of visibile-attribute for xaml elements
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from bool to visibility
            if ((bool)value == true)
            {
                return "Visible";
            }
            else
            {
                return "Hidden";
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            if ((string)value == "Visible")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

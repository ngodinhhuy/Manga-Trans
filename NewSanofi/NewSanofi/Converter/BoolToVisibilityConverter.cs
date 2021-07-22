using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NewSanofi.Converter
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            // Setting default values
            Visibility visible = Visibility.Visible;
            var visibleIfFalse = Visibility.Collapsed;
            var visiblerIfTrue = Visibility.Visible;

            if ((bool)value)
            {
                visible = visiblerIfTrue;
            }
            else
            {
                visible = visibleIfFalse;
            }
            return visible;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

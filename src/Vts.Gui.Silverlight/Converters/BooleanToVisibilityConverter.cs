using System;
using System.Windows;
using System.Windows.Data;

namespace Vts.SiteVisit.Converters
{
    ///
    /// Converts a boolean to visibility value.
    ///
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility) value == Visibility.Collapsed ? false : true;
        }
    }
}

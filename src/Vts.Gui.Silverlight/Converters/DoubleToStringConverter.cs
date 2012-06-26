using System;
using System.Windows.Data;

namespace Vts.Gui.Silverlight.Converters
{
    public class DoubleToStringConverter: IValueConverter
    {

        #region IValueConverter Members
        // Summary:
        //     Modifies the source data before passing it to the target for display in the
        //     UI.
        //
        // Parameters:
        //   value:
        //     The source data being passed to the target.
        //
        //   targetType:
        //     The System.Type of data expected by the target dependency property.
        //
        //   parameter:
        //     An optional parameter to be used in the converter logic.
        //
        //   culture:
        //     The culture of the conversion.
        //
        // Returns:
        //     The value to be passed to the target dependency property.

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is double))
                throw new ArgumentException("Value must be a double");

            if (parameter != null)
            {
                return ((double)value).ToString((string)parameter);
            }
            else
            {
                return ((double)value).ToString();
            }
                
            //string numberUnformatted = value.ToString();
            //int ind = numberUnformatted.IndexOf(".");
            //int numberOfDecimals = 2;

            //string integerPart;
            //string decimalPart;

            //if (ind <= 0)
            //{
            //    integerPart = numberUnformatted;
            //    decimalPart = "0";
            //}
            //else
            //{
            //    integerPart = numberUnformatted.Substring(0, ind);
            //    decimalPart = numberUnformatted.Substring(ind + 1);
            //}
            //if (decimalPart.Length > 3)
            //    decimalPart = decimalPart.Substring(0, numberOfDecimals);

            //return integerPart + "." + decimalPart; ;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            return value;
        }

        #endregion
    }
}

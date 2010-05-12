using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MoCS.Client.Model
{
    public enum ModelState
    {
        Fectching,
        Invalid,
        Active
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class FormattingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            string formatString = parameter as string;

            if (value.GetType() == typeof(DateTime))
            {
                if (((DateTime)value) == DateTime.MinValue)
                {
                    return "";
                }
            }

            if (formatString != null)
            {
                return string.Format(culture, formatString, value);
            }
            else
            {
                return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            // we don't intend this to ever be called
            return null;
        }
    }




}

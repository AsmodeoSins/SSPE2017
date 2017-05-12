
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MVVMShared.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateConverter : IValueConverter
    {
        private const string _format = "dd-MM-yyyy";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
                return Fechas.fechaLetra((DateTime)value);
            return string.Empty;
        }
        /*
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToString(_format);
        }
        */
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime DateTimeValue;
            if (DateTime.TryParse((string)value, out DateTimeValue))
                return DateTimeValue;
            return value;
        }
    }


    [ValueConversion(typeof(string), typeof(string))]
    public class TituloConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null) return value;

            string titulo = string.Empty;

            for (int i = 0; i < value.ToString().Length; i++)
            {
                if (i == 0)
                    titulo += value.ToString()[i].ToString().ToUpper();
                else titulo += value.ToString()[i].ToString().ToLower();
            }

            return titulo;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
    
}

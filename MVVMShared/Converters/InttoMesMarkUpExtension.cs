using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;

namespace MVVMShared.Converters
{
    [ValueConversion(typeof(string),typeof(int))]
    public class InttoMesMarkUpExtension:MarkupExtension,IValueConverter
    {
        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var _month = System.Convert.ToInt32(value);
            if (_month <= 0 && _month > 12)
                throw new Exception("No es un valor valido para convertir");
            DateTimeFormatInfo datetimeInfo = culture.DateTimeFormat;
            return datetimeInfo.MonthNames[_month - 1].ToUpper();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTimeFormatInfo datetimeInfo = culture.DateTimeFormat;
            return datetimeInfo.MonthNames.ToList().IndexOf(value.ToString().ToLower()) + 1;
        }

        #endregion

        #region MarkupExtension

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion
    }
}

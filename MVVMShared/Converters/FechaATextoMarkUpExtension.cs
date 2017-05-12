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
    [ValueConversion(typeof(DateTime),typeof(string))]
    public class FechaATextoMarkUpExtension : MarkupExtension, IValueConverter
    {
        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var _date = System.Convert.ToDateTime(value);
            DateTimeFormatInfo datetimeInfo = culture.DateTimeFormat;
            return datetimeInfo.GetDayName(_date.DayOfWeek).ToUpper() + " " +_date.Day + " DE " + datetimeInfo.MonthNames[_date.Month].ToUpper() + ", " + _date.Year;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
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

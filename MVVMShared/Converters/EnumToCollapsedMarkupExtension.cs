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
    [ValueConversion(typeof(Enum), typeof(Enum))]
    public class EnumToCollapsedMarkupExtension : MarkupExtension, IValueConverter
    {
        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var params1 = (int[]) parameter;
            foreach (var item in params1)
                if (item.Equals((int)value))
                    return Visibility.Visible;
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("No two way conversion, one way binding only.");
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

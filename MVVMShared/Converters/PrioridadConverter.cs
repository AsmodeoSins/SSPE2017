using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace MVVMShared.Converters
{
    [ValueConversion(typeof(short), typeof(string))]
    public class PrioridadConverterMarkupExtension :MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (short.Parse(value.ToString()))
            {
                case 1:
                    return "ALTA";
                case 2:
                    return "MEDIA";
                case 3:
                    return "BAJA";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }

        #region MarkupExtension

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion
    }
}

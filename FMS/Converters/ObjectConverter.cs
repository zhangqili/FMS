using System;
using System.Linq;
using System.Windows.Data;

namespace FMS.Converters
{
    /// <summary>
    /// CommandParameter 多参数传递
    /// </summary>
    public class ObjectConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public static object ConverterObject;

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values.ToArray();
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
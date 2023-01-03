using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using FMS.Models;

namespace FMS.Converters
{
    internal class ChangeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int change = (int)values[0];
            StatusCode status = (StatusCode)values[1];
            switch (status)
            {
                case StatusCode.NA:
                    return "N/A";
                case StatusCode.NORM:
                    if (change==0)
                    {
                        return "-";
                    }
                    else if (change > 0)
                    {
                        return "▼" + change.ToString();
                    }
                    else
                    {
                        return "▲" + (-change).ToString();

                    }
                case StatusCode.NEW:
                    return "NEW";
                case StatusCode.OUT:
                    return "OUT";
                case StatusCode.BACK:
                    return "BACK";
                default:
                    return "N/A";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using FMS.Lib;
using FMS.Models;

namespace FMS.Converters
{
    public class NameItemObservableCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new ObservableCollection<NameItem>((List<NameItem>)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ObservableCollection<NameItem>)value).ToList();
        }
    }
}

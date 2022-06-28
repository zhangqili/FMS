using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using FMS.Lib;
using FMS.Models;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;

namespace FMS.Converters
{
    internal class OxyPlotRankListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PlotModel model = new();
            var leftLinearAxis = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Left,
                Maximum = 50,
                Minimum = 0,
                StartPosition = 1,
                EndPosition = 0,
                IsAxisVisible = false
            };
            var bottomLinearAxis = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Bottom,
                Maximum = Global.Core.Dates.Count,
                Minimum = Global.Core.Dates.FindIndex(
                    x=>x.DigitalDate==Global.Core.DateItems.First().DigitalDate)+1,
                IsAxisVisible = false

            };
            if (value != null)
            {
                NameItem nameItem = (value as NameItem);
                int peak = nameItem.Peak;
                bool peakflag = true;
                bool firstflag = true;
                List<Item> items = nameItem.ListByName;
                LineSeries series = new LineSeries();
                series.Color = OxyColors.Blue;
                int i = 0;
                foreach (Item item in items)
                {
                    i++;
                    series.Points.Add(item.Score == 0 ? DataPoint.Undefined : new DataPoint(i, item.Rank));

                }
                model.Series.Add(series);
                model.Axes.Add(leftLinearAxis);
                model.Axes.Add(bottomLinearAxis);
            }
            return model;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

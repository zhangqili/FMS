using FMS.Commands;
using FMS.Lib;
using FMS.Models;
using NPOI.SS.Formula.Functions;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FMS.ViewModels
{
    internal class OxyPlotWindowViewModel : NotificationObject
    {
        private PlotModel scoreModel;

        public PlotModel ScoreModel
        {
            get { return scoreModel; }
            set
            {
                scoreModel = value;
                OnPropertyChanged(nameof(ScoreModel));
            }
        }
        private PlotModel rankModel;

        public PlotModel RankModel
        {
            get { return rankModel; }
            set
            {
                rankModel = value;
                OnPropertyChanged(nameof(RankModel));
            }
        }

        public void AddSeries(NameItem nameItem)
        {
            var textbox = new TextBox();
            LineSeries scoreSeries = new LineSeries() { Title = nameItem.Name, MarkerType = MarkerType.Diamond };
            LineSeries rankSeries = new LineSeries() { Title = nameItem.Name, MarkerType = MarkerType.Diamond };
            int i = 0;
            foreach (Item item in nameItem.ListByName)
            {
                i++;
                if (item.Point == 0)
                {
                    scoreSeries.Points.Add(DataPoint.Undefined);
                    rankSeries.Points.Add(DataPoint.Undefined);
                }
                else
                {
                    scoreSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(Core.DigitalDateToDateTime(item.DigitalDate)), item.Point));
                    //rankSeries.Points.Add(new DataPoint(i, item.Rank));
                    rankSeries.Points.Add(new DataPoint(i, 51 - Math.Log10(50 - item.Rank) / Math.Log10(50) * 50));
                }
            }
            ScoreModel.Series.Add(scoreSeries);
            RankModel.Series.Add(rankSeries);
            //SetAxes(flag);
            ScoreModel.InvalidatePlot(true);
            RankModel.InvalidatePlot(true);
        }

        public OxyPlotWindowViewModel()
        {
            Global.OxyPlotWindowViewModel = this;
            ScoreModel = new PlotModel();
            RankModel = new PlotModel();

            var logarithmicAxis1 = new LogarithmicAxis()
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Maximum = Global.Core.Items.Max(x => x.Point),
                Minimum = Global.Core.Items.Where(x => x.Point > 0).Min(x => x.Point),
                Position = AxisPosition.Left,
            };
            ScoreModel.Axes.Add(logarithmicAxis1);

            /*
            var linearAxis1 = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Left
            };
            */
            //ScoreModel.Axes.Add(linearAxis1);
            //var linearAxis2 = new LinearAxis
            //{
            //    MajorGridlineStyle = LineStyle.Solid,
            //    MinorGridlineStyle = LineStyle.Dot,
            //    Position = AxisPosition.Bottom,
            //    Maximum = Global.Core.Dates.Count,
            //    Minimum = Global.Core.Dates.FindIndex(
            //        x => x.DigitalDate == Global.Core.DateItems.First().DigitalDate) + 1
            //};
            //ScoreModel.Axes.Add(linearAxis2);
            var dateTimeAxis1 = new DateTimeAxis()
            {
                Minimum = DateTimeAxis.ToDouble(Core.DigitalDateToDateTime(Global.Core.Dates.First().DigitalDate)),
                Maximum = DateTimeAxis.ToDouble(Core.DigitalDateToDateTime(Global.Core.Dates.Last().DigitalDate)),
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            };
            ScoreModel.Axes.Add(dateTimeAxis1);
            /*
            var logarithmicAxis2 = new LogarithmicAxis()
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position =AxisPosition.Left,
                Maximum = 50,
                Minimum = 1,
                StartPosition = 1,
                EndPosition = 0,
            };
            RankModel.Axes.Add(logarithmicAxis2);
            */
            var linearAxis3 = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Left,
                Maximum = 50,
                Minimum = 1,
                StartPosition = 1,
                EndPosition = 0
            };
            RankModel.Axes.Add(linearAxis3);
            
            var linearAxis4 = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Bottom,
                Maximum = Global.Core.Dates.Count,
                Minimum = Global.Core.Dates.FindIndex(
                    x => x.DigitalDate == Global.Core.DateItems.First().DigitalDate) + 1
            };
            RankModel.Axes.Add(linearAxis4);
            
            
            ScoreModel.Legends.Add(new Legend
            {
                LegendBorder = OxyColors.Black,
                LegendPosition = LegendPosition.LeftTop,
                LegendPlacement = LegendPlacement.Outside,
                LegendFontSize = 20,
                LegendFont = "微软雅黑"
            });
            RankModel.Legends.Add(new Legend
            {
                LegendBorder = OxyColors.Black,
                LegendPosition = LegendPosition.LeftTop,
                LegendPlacement = LegendPlacement.Outside,
                LegendFontSize = 20,
                LegendFont= "微软雅黑"
            });
        }
    }
}

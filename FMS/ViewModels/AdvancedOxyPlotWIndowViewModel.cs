using FMS.Lib;
using FMS.Models;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.ViewModels
{
    internal class AdvancedOxyPlotWIndowViewModel : NotificationObject
    {
        private string title;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

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

        private double averageRank;

        public double AverageRank
        {
            get { return averageRank; }
            set
            {
                averageRank = value;
                OnPropertyChanged(nameof(AverageRank));
            }
        }

        private double medianRank;

        public double MedianRank
        {
            get { return medianRank; }
            set
            {
                medianRank = value;
                OnPropertyChanged(nameof(MedianRank));
            }
        }

        private double betaRank;

        public double BetaRank
        {
            get { return betaRank; }
            set
            {
                betaRank = value;
                OnPropertyChanged(nameof(BetaRank));
            }
        }

        private double AlphaRank;

        public double alphaRank
        {
            get { return alphaRank; }
            set
            {
                alphaRank = value;
                OnPropertyChanged(nameof(AlphaRank));
            }
        }


        private double averageScore;

        public double AverageScore
        {
            get { return averageScore; }
            set
            {
                averageScore = value;
                OnPropertyChanged(nameof(AverageScore));
            }
        }

        private double medianScore;

        public double MedianScore
        {
            get { return medianScore; }
            set
            {
                medianScore = value;
                OnPropertyChanged(nameof(MedianScore));
            }
        }

        private double betaScore;

        public double BetaScore
        {
            get { return betaScore; }
            set
            {
                betaScore = value;
                OnPropertyChanged(nameof(BetaScore));
            }
        }

        private double alphaScore;

        public double AlphaScore
        {
            get { return alphaScore; }
            set
            {
                alphaScore = value;
                OnPropertyChanged(nameof(AlphaScore));
            }
        }

        public RectangleAnnotation RankRange { get; set; }
        public RectangleAnnotation ScoreRange { get; set; }
        public FunctionSeries RankRegression { get; set; }
        public FunctionSeries ScoreRegression { get; set; }
        public NameItem NameItem { get; set; }

        public AdvancedOxyPlotWIndowViewModel()
        {
            Initialize();
            Title = "分析";
        }

        public AdvancedOxyPlotWIndowViewModel(NameItem nameItem)
        {
            Initialize();
            NameItem = nameItem;
            Title = "分析 - "+nameItem.Name;
            AddSeries(NameItem);
            AlphaRank = 0;
        }
        public void AddSeries(NameItem nameItem)
        {
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
                    rankSeries.Points.Add(new DataPoint(i, item.Rank));
                }
            }
            ScoreModel.Series.Add(scoreSeries);
            RankModel.Series.Add(rankSeries);
            //SetAxes(flag);
            ScoreModel.InvalidatePlot(true);
            RankModel.InvalidatePlot(true);
        }

        private void Initialize()
        {

            ScoreModel = new PlotModel();
            RankModel = new PlotModel();
            var linearAxis1 = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Maximum = Global.Core.Items.Max(x=>x.Point),
                Minimum = Global.Core.Items.Where(x=>x.Point>0).Min(x=>x.Point),
                Position = AxisPosition.Left
            };
            ScoreModel.Axes.Add(linearAxis1);
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

            RankRange = new RectangleAnnotation { Fill = OxyColor.FromAColor(120, OxyColors.SkyBlue), MinimumX = 0, MaximumX = 0 };
            RankModel.Annotations.Add(RankRange);
            double startx = double.NaN;
            RankModel.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    startx = RankRange.InverseTransform(e.Position).X;
                    RankRange.MinimumX = startx;
                    RankRange.MaximumX = startx;
                    RankModel.InvalidatePlot(true);
                    e.Handled = true;
                }
            };
            RankModel.MouseMove += (s, e) =>
            {
                if (!double.IsNaN(startx))
                {
                    var x = RankRange.InverseTransform(e.Position).X;
                    RankRange.MinimumX = Math.Min(x, startx);
                    RankRange.MaximumX = Math.Max(x, startx);
                    //RankModel.Subtitle = string.Format("Integrating from {0:0.00} to {1:0.00}", RankRange.MinimumX, RankRange.MaximumX);
                    RankCalculate();
                    RankModel.InvalidatePlot(true);
                    e.Handled = true;
                }
            };
            RankModel.MouseUp += (s, e) =>
            {
                startx = double.NaN;
            };


            ScoreRange = new RectangleAnnotation { Fill = OxyColor.FromAColor(120, OxyColors.SkyBlue), MinimumX = 0, MaximumX = 0 };
            ScoreModel.Annotations.Add(ScoreRange);
            ScoreModel.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    startx = ScoreRange.InverseTransform(e.Position).X;
                    ScoreRange.MinimumX = startx;
                    ScoreRange.MaximumX = startx;
                    ScoreModel.InvalidatePlot(true);
                    e.Handled = true;
                }
            };
            ScoreModel.MouseMove += (s, e) =>
            {
                if (!double.IsNaN(startx))
                {
                    var x = ScoreRange.InverseTransform(e.Position).X;
                    ScoreRange.MinimumX = Math.Min(x, startx);
                    ScoreRange.MaximumX = Math.Max(x, startx);
                    //ScoreModel.Subtitle = string.Format("Integrating from {0:0.00} to {1:0.00}", ScoreRange.MinimumX, ScoreRange.MaximumX);
                    ScoreCalculate();
                    ScoreModel.InvalidatePlot(true);
                    e.Handled = true;
                }
            };
            ScoreModel.MouseUp += (s, e) =>
            {
                startx = double.NaN;
            };
        }

        private void ScoreCalculate()
        {
            //if (!double.IsNaN(RankRange.MaximumX)&& !double.IsNaN(RankRange.MinimumX))

            var series = ScoreModel.Series[0] as LineSeries;
            var points = series.Points.FindAll(x=>x.X>ScoreRange.MinimumX&&x.X<ScoreRange.MaximumX);
            int range = points.Count;
            if (range > 0)
            {
                AverageScore = points.Average(x => (double)x.Y);
            }
            else
            {
                return;
            }
            double xiyisum = 0;
            double xisum = 0;
            double yisum = 0;
            double xi2sum = 0;
            int count = 0;
            foreach(var item in points)
            {
                if (item.Y > 0)
                {
                    xiyisum += item.X * item.Y;
                    xisum += item.X;
                    yisum += item.Y;
                    xi2sum += item.X * item.X;
                    count++;
                }
            }
            double averageX = xisum / count;
            AlphaScore = (count * xiyisum - xisum * yisum) / (count * xi2sum - xisum * xisum);
            BetaScore = AverageScore - (averageX) * AlphaScore;
            if (ScoreModel.Series.Contains(ScoreRegression))
            {
                ScoreModel.Series.Remove(ScoreRegression);
            }
            ScoreRegression = new FunctionSeries(ScoreFunction, points.First().X, points.Last().X, 0.1);
            ScoreModel.Series.Add(ScoreRegression);
            MedianScore = 0;
            ScoreRange.Text = string.Format("k = {0}\n平均：{1}", AlphaScore, AverageScore);

        }

        private void RankCalculate()
        {
            var series = RankModel.Series[0] as LineSeries;
            var points = series.Points.FindAll(x => x.X > RankRange.MinimumX && x.X < RankRange.MaximumX);

            //if (!double.IsNaN(RankRange.MaximumX)&& !double.IsNaN(RankRange.MinimumX))

            var items = NameItem.ListByName;
            int range = points.Count;
            if (range > 0)
            {
                AverageRank = points.Average(x => x.Y);
            }
            double xiyisum = 0;
            double xisum = 0;
            double yisum = 0;
            double xi2sum = 0;
            int count=0;
            foreach (var item in points)
            {
                xiyisum += item.X * item.Y;
                xisum += item.X;
                yisum += item.Y;
                xi2sum += item.X * item.X;
                count++;
            }

            double averageX = xisum / count;
            AlphaRank = (count * xiyisum - xisum * yisum) / (count * xi2sum-xisum*xisum);
            BetaRank = AverageRank - averageX * AlphaRank;
            if (RankModel.Series.Contains(RankRegression))
            {
                RankModel.Series.Remove(RankRegression);
            }
            RankRegression = new FunctionSeries(RankFunction, 0, NameItem.ListByName.Count+1, 0.1);
            RankModel.Series.Add(RankRegression);
            MedianRank = 0;
            RankRange.Text = string.Format("k = {0}\n平均：{1}", AlphaRank,AverageRank);
        }
        private double RankFunction(double arg)
        {
            return arg * AlphaRank+BetaRank;
        }

        private double ScoreFunction(double arg)
        {
            return arg * AlphaScore + BetaScore;
        }
        private double ToTimestamp(int x)
        {
            return DateTimeAxis.ToDouble(Core.DigitalDateToDateTime(x));
        }
    }
}

using FMS.Commands;
using FMS.Lib;
using FMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace FMS.ViewModels
{
    public class DateItemViewModel : NotificationObject
    {
        private ObservableCollection<DateItem> dateItems;

        public ObservableCollection<DateItem> DateItems
        {
            get { return dateItems; }
            set
            {
                dateItems = value;
                OnPropertyChanged(nameof(DateItems));
            }
        }
        private string filterText;

        public string FilterText
        {
            get { return filterText; }
            set
            {
                Filter(value);
                filterText = value;
                OnPropertyChanged(nameof(FilterText));
            }
        }
        private ObservableCollection<Item> weightedOverallRankingList;

        public ObservableCollection<Item> WeightedOverallRankingList
        {
            get { return weightedOverallRankingList; }
            set
            {
                weightedOverallRankingList = value;
                OnPropertyChanged(nameof(WeightedOverallRankingList));
            }
        }
        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                GetRankingList(value);
                selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }
        private double adjustValue;

        public double AdjustValue
        {
            get { return adjustValue; }
            set
            {
                adjustValue = value;
                GetRankingList(SelectedIndex);
                OnPropertyChanged(nameof(AdjustValue));
            }
        }

        public DelegateCommand FilterCommand { get; set; }

        private void Filter(object parameter)
        {
            if (FilterText == "")
            {
                DateItems = Global.Core.ObservableCollectionOfDateItems;
            }
            else
            {
                DateItems = new ObservableCollection<DateItem>
                    (Global.Core.ObservableCollectionOfDateItems.ToList().FindAll
                    (x => x.Title.Contains(FilterText) | x.DigitalDate.ToString().Contains(FilterText)));
            }
        }
        private void GetRankingList(int n)
        {
            List<Item> items = new();
            for (int i = 0; i < Global.Core.NameItems.Count; i++)
            {

                double sum = 0.0;
                double day = (Core.DigitalDateToDateTime(Global.Core.NameItems[i].ListByName[n].DigitalDate) - Core.DigitalDateToDateTime(Global.Core.NameItems[i].ListByName[0].DigitalDate)).Days;
                for (int j = 0; j <= n; j++)
                {
                    double dayc = (Core.DigitalDateToDateTime(Global.Core.NameItems[i].ListByName[j].DigitalDate) - Core.DigitalDateToDateTime(Global.Core.NameItems[i].ListByName[0].DigitalDate)).Days;
                    //sum += ListByName[i].Point * Convert.ToDouble(i) / ListByName.Count;
                    sum += (Global.Core.NameItems[i].ListByName[j].Point+AdjustValue) * dayc / day;
                }

                if (sum > 0)
                {
                    items.Add(new Item(){Name = Global.Core.NameItems[i].Name, Point = sum});
                }
            }

            WeightedOverallRankingList = new(items.OrderByDescending(x => x.Point));
        }
        private void Filter(string f)
        {
            if (f == "")
            {
                DateItems = Global.Core.ObservableCollectionOfDateItems;
            }
            else
            {
                DateItems = new ObservableCollection<DateItem>
                    (Global.Core.ObservableCollectionOfDateItems.ToList().FindAll
                    (x => x.Title.ToLower().Contains(f.ToLower()) | x.DigitalDate.ToString().Contains(f)));
            }
        }

        public DateItemViewModel()
        {
            Global.DateItemViewModel = this;
            FilterCommand = new DelegateCommand(Filter);
            DateItems = Global.Core.ObservableCollectionOfDateItems;
            AdjustValue = 0;
            FilterText = "";
        }
    }
}

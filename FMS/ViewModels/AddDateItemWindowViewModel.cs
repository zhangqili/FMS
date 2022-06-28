using FMS.Commands;
using FMS.Lib;
using FMS.Models;
using FMS.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FMS.ViewModels
{
    public class AddDateItemWindowViewModel : NotificationObject
    {
        private List<Item> OldListByDate { get; set; }
        private List<Item> newListByDate;

        public List<Item> NewListByDate
        {
            get { return newListByDate; }
            set
            {
                newListByDate = value;
                OnPropertyChanged(nameof(NewListByDate));
            }
        }
        private ObservableCollection<Item> oldSourceObservableCollectionByDate;
        private ObservableCollection<Item> oldObservableCollectionByDate;

        public ObservableCollection<Item> OldObservableCollectionByDate
        {
            get { return oldObservableCollectionByDate; }
            set
            {
                oldObservableCollectionByDate = value;
                OnPropertyChanged(nameof(OldObservableCollectionByDate));
            }
        }

        private ObservableCollection<Item> sourceObservableCollectionByDate;
        private ObservableCollection<Item> observableCollectionByDate;

        public ObservableCollection<Item> ObservableCollectionByDate
        {
            get { return observableCollectionByDate; }
            set
            {
                observableCollectionByDate = value;
                OnPropertyChanged(nameof(ObservableCollectionByDate));
            }
        }

        private string filterText;

        public string FilterText
        {
            get { return filterText; }
            set
            {
                filterText = value;
                Filter(value);
                OnPropertyChanged(nameof(FilterText));
            }
        }

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
        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        public DelegateCommand RefreshCommand { get; set; }

        private void Refresh(object parameter)
        {
            //NewListByDate = NewListByDate.OrderByDescending(x => x.Score).ToList();
            //for (int i = 0; i < NewListByDate.Count; i++)
            //{
            //    if (NewListByDate[i].Score != 0)
            //    {
            //        NewListByDate[i].Rank = i + 1;
            //    }
            //    else
            //    {
            //        NewListByDate[i].Rank = 0;
            //    }
            //}
            ObservableCollectionByDate = new ObservableCollection<Item>
                (ObservableCollectionByDate.OrderByDescending(x => x.Score));
            for (int i = 0; i < ObservableCollectionByDate.Count; i++)
            {
                if (ObservableCollectionByDate[i].Score != 0)
                {
                    ObservableCollectionByDate[i].Rank = i + 1;
                }
                else
                {
                    ObservableCollectionByDate[i].Rank = 0;
                }
            }
            //Core.AnalyzeChange(NewListByDate, OldListByDate);
            Global.Core.AnalyzeChange(ObservableCollectionByDate.ToList(), OldListByDate);
        }
        public DelegateCommand OKCommand { get; set; }

        private void OK(object parameter)
        {
            Refresh(null);
            DateItem dateItem = new DateItem()
            {
                Date = Date,
                Title = Title,
                ListByDate = sourceObservableCollectionByDate.ToList(),
                DigitalDate = Convert.ToInt32(Date.ToString("yyyyMMdd"))
            };
            Global.Core.AddDateItem(dateItem);
        }
        public DelegateCommand FilterCommand { get; set; }

        private void Filter(object parameter)
        {
        }
        private void Filter(string f)
        {
            if (f == "")
            {
                ObservableCollectionByDate = sourceObservableCollectionByDate;
                OldObservableCollectionByDate = oldSourceObservableCollectionByDate;
            }
            else
            {
                ObservableCollectionByDate = new ObservableCollection<Item>
                    (ObservableCollectionByDate.ToList().FindAll(x => x.Name.Contains(f)));
                OldObservableCollectionByDate = new ObservableCollection<Item>
                    (OldObservableCollectionByDate.ToList().FindAll(x => x.Name.Contains(f)));
            }
        }
        public DelegateCommand ImportCommand { get; set; }
        private void Import(object parameter)
        {
            List<Item> items = observableCollectionByDate.ToList();
            ImportFromTextWindow importFromTextWindow = new ImportFromTextWindow();
            importFromTextWindow.ShowDialog();
            if (importFromTextWindow.ReturnList!=null)
            {
                Core.UpdateItems(items, importFromTextWindow.ReturnList);
                foreach (Item item in importFromTextWindow.ReturnList)
                {
                    if (!Global.Core.NameItems.Exists(x => x.Name == item.Name))
                    {
                        Global.Core.AddNameItem(item.Name);
                    }
                }
                items.ForEach(x => x.DigitalDate = Convert.ToInt32(Date.ToString("yyyyMMdd")));
                ObservableCollectionByDate = new ObservableCollection<Item>(items);
                sourceObservableCollectionByDate = new ObservableCollection<Item>(items);
            }
            Refresh(null);
        }
        public AddDateItemWindowViewModel()
        {
            RefreshCommand = new DelegateCommand(Refresh);
            OKCommand = new DelegateCommand(OK);
            ImportCommand = new DelegateCommand(Import);
            DateItem dateItem = Global.Core.DateItems.Last();
            //NewListByDate = new List<Item>();
            ObservableCollectionByDate = new ObservableCollection<Item>();
            OldListByDate = dateItem.ListByDate;
            OldObservableCollectionByDate = new ObservableCollection<Item>(dateItem.ListByDate);
            foreach (Item item in dateItem.ListByDate)
            {
                ObservableCollectionByDate.Add(new Item()
                {
                    Name = item.Name,
                    DigitalDate = item.DigitalDate,
                    Score = item.Score,
                    Rank = item.Rank,
                    Change = item.Change
                });
            }
            oldSourceObservableCollectionByDate = OldObservableCollectionByDate;
            sourceObservableCollectionByDate = ObservableCollectionByDate;
            Refresh(null);
            Date = DateTime.Today;
            DateItem newDateItem = new DateItem();
        }

    }
}

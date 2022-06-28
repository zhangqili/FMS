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
            FilterCommand = new DelegateCommand(Filter);
            DateItems = Global.Core.ObservableCollectionOfDateItems;
            FilterText = "";
        }
    }
}

using FMS.Models;
using FMS.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Commands;
using System.Collections.ObjectModel;
using FMS.Views;

namespace FMS.ViewModels
{
    public class NameItemViewModel:NotificationObject
    {
        private ObservableCollection<NameItem> nameItems;

        public ObservableCollection<NameItem> NameItems
        {
            get { return nameItems; }
            set
            {
                nameItems = value;
                OnPropertyChanged(nameof(NameItems));
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
        private object selectedItem;

        public object SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
        private void Filter(string f)
        {
            if (f == "")
            {
                NameItems = Global.Core.ObservableCollectionOfNameItems;
            }
            else
            {
                NameItems = new ObservableCollection<NameItem>
                    (Global.Core.ObservableCollectionOfNameItems.ToList().FindAll(x => x.Name.Contains(f)));
            }
        }
        public DelegateCommand AddToChartCommand { get; set; }
        private void AddToChart(object parameter)
        {
            if (SelectedItem!=null)
            {
                if (Global.OxyPlotWindowViewModel == null)
                {
                    new OxyPlotWindow().Show();
                    Global.OxyPlotWindowViewModel.AddSeries((NameItem)SelectedItem);
                }
                else
                {
                    Global.OxyPlotWindowViewModel.AddSeries((NameItem)SelectedItem);
                }
            }
        }

        public DelegateCommand AnalyseCommand { get; set; }
        private void Analyse(object parameter)
        {
            if (SelectedItem!=null)
            {
                new AdvancedOxyPlotWindow((NameItem)SelectedItem).Show();
            }
        }
        
        public NameItemViewModel()
        {
            AddToChartCommand = new DelegateCommand(AddToChart);
            AnalyseCommand = new DelegateCommand(Analyse);
            NameItems = Global.Core.ObservableCollectionOfNameItems;
        }
    }
}

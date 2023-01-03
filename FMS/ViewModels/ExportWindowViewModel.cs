using FMS.Commands;
using FMS.Lib;
using FMS.Models;
using FMS.Views;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace FMS.ViewModels
{
    internal class ExportWindowViewModel:NotificationObject
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
        private int dateItemCount;

        public int DateItemCount
        {
            get { return dateItemCount; }
            set
            {
                dateItemCount = value;
                Count = dateItemCount * NameItemCount;
                OnPropertyChanged(nameof(DateItemCount));
            }
        }
        private int nameItemCount;

        public int NameItemCount
        {
            get { return nameItemCount; }
            set
            {
                nameItemCount = value;
                Count = nameItemCount * DateItemCount;
                OnPropertyChanged(nameof(NameItemCount));
            }
        }
        private int count;

        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                OnPropertyChanged(nameof(Count));
            }
        }
        private bool isDateItemAllSelected;

        public bool IsDateItemAllSelected
        {
            get { return isDateItemAllSelected; }
            set
            {
                isDateItemAllSelected = value;
                OnPropertyChanged(nameof(IsDateItemAllSelected));
            }
        }
        private bool isNameItemAllSelected;

        public bool IsNameItemAllSelected
        {
            get { return isNameItemAllSelected; }
            set
            {
                isNameItemAllSelected = value;
                OnPropertyChanged(nameof(IsNameItemAllSelected));
            }
        }
        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        /*
        private IList selectedDateItems;

        public IList SelectedDateItems
        {
            get { return SelectedDateItems; }
            set
            {
                selectedDateItems = value;
                OnPropertyChanged(nameof(SelectedDateItems));
            }
        }
        private List<NameItem> selectedNameItems;

        public List<NameItem> SelectedNameItems
        {
            get { return SelectedNameItems; }
            set
            {
                selectedNameItems = value;
                OnPropertyChanged(nameof(SelectedNameItems));
            }
        }
        */
        public DelegateCommand OKCommand { get; set; }
        private void OK(object parameter)
        {
            object[] objects = parameter as object[];
            IList dateItems1 = objects[0] as IList;
            IList nameItems1 = objects[1] as IList;
            List<DateItem> dateItems2 = new List<DateItem>();
            List<Item> items = new List<Item>();
            List<string> names = new List<string>();
            foreach (var VARIABLE in dateItems1)
            {
                dateItems2.Add(VARIABLE as DateItem);
            }
            foreach (var VARIABLE in nameItems1)
            {
                names.Add((VARIABLE as NameItem).Name);
            }
            var selectedDates = DateItems.Where(x => dateItems2.Contains(x));
            foreach (var dateItem in selectedDates)
            {
                items.AddRange(dateItem.ListByDate);
            }
            items = items.Where(x => names.Contains(x.Name)).ToList();
            Global.Core.Export(FilePath,items);
        }
        public DelegateCommand RefreshDateItemCommand { get; set; }
        private void RefreshDateItem(object parameter)
        {
            DateItemCount = (int)parameter;
            IsDateItemAllSelected = DateItemCount == DateItems.Count ? true : false;
        }
        public DelegateCommand RefreshNameItemCommand { get; set; }
        private void RefreshNameItem(object parameter)
        {
            NameItemCount = (int)parameter;
            IsNameItemAllSelected = NameItemCount == NameItems.Count ? true : false;
        }
        public DelegateCommand SelectPathCommand { get; set; }
        private void SelectPath(object parameter)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = FilePath;
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel 工作簿|*.xlsx";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                FilePath=dlg.FileName;
            }
        }
        public ExportWindowViewModel()
        {
            DateItems = Global.Core.ObservableCollectionOfDateItems;
            NameItems = Global.Core.ObservableCollectionOfNameItems;
            IsDateItemAllSelected = true;
            IsNameItemAllSelected = true;
            DateItemCount = DateItems.Count;
            NameItemCount = NameItems.Count;
            OKCommand = new DelegateCommand(OK);
            RefreshDateItemCommand = new DelegateCommand(RefreshDateItem);
            RefreshNameItemCommand = new DelegateCommand(RefreshNameItem);
            SelectPathCommand = new DelegateCommand(SelectPath);
            SaveFileDialog dlg = new SaveFileDialog();
            FilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + 
            "result" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
        }
    }
}

using FMS.Commands;
using FMS.Lib;
using FMS.Models;
using Microsoft.Win32;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;

namespace FMS.ViewModels
{
    public class ImportWindowViewModel:NotificationObject
    {
        private List<string> names;

        public List<string> Names
        {
            get { return names; }
            set
            {
                names = value;
                OnPropertyChanged(nameof(Names));
            }
        }
        private List<int> dates;

        public List<int> Dates
        {
            get { return dates; }
            set
            {
                dates = value;
                OnPropertyChanged(nameof(Dates));
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
        

        public DelegateCommand OKCommand { get; set; }

        private void OK(object parameter)
        {
            if (filePath == null)
            {
                return;
            }
            if (MessageBox.Show("此操作将覆盖源文件","警告", MessageBoxButton.OKCancel, MessageBoxImage.Warning) ==MessageBoxResult.OK)
            {
                Global.Core.Import(FilePath);
                Global.DateItemViewModel.DateItems = Global.Core.ObservableCollectionOfDateItems;
                Global.NameItemViewModel.NameItems = Global.Core.ObservableCollectionOfNameItems;
                /*
                Process p = new Process();
                p.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + "FMS.exe";
                p.StartInfo.UseShellExecute = false;
                p.Start();
                Application.Current.Shutdown();
                */
            }
        }

        private void ShowDetail(string filePath)
        {
            List<string> vs1;
            List<int> vs2;
            Global.Core.CheckFile(filePath,out vs1,out vs2);
            Names = vs1;
            Dates = vs2;
            Count = Names.Count * Dates.Count;
        }

        public ImportWindowViewModel()
        {
            OKCommand = new DelegateCommand(OK);
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel 工作簿|*.xlsx";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                FilePath = dlg.FileName;
                ShowDetail(FilePath);
            }
        }
        public ImportWindowViewModel(string filePath)
        {
            FilePath=filePath;
            OKCommand = new DelegateCommand(OK);
            ShowDetail(FilePath);
        }
    }
}

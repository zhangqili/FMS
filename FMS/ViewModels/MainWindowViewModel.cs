﻿using FMS.Commands;
using FMS.Lib;
using FMS.Models;
using FMS.Views;
using Microsoft.Win32;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FMS.ViewModels
{
    public class MainWindowViewModel:NotificationObject
    {
        private int memory;

        public int Memory
        {
            get { return memory; }
            set
            {
                memory = value;
                OnPropertyChanged(nameof(Memory));
            }
        }


        public DelegateCommand AddDateItemCommand { get; set; }

        private void AddDateItem(object parameter)
        {
            new AddDateItemWindow().ShowDialog();
        }
        public DelegateCommand AddNameItemCommand { get; set; }

        private void AddNameItem(object parameter)
        {
            new AddNameItemWindow().ShowDialog();
        }
        public DelegateCommand SaveCommand { get; set; }

        private void Save(object parameter)
        {
            Global.Core.Save();
            if (MessageBox.Show("是否需要重启", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Process p = new Process();
                p.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "FMS.exe";
                p.StartInfo.UseShellExecute = false;
                p.Start();
                Application.Current.Shutdown();
            }
        }
        public DelegateCommand ImportCommand { get; set; }

        private void Import(object parameter)
        {
            new ImportWindow().Show();
        }

        public DelegateCommand OpenOxyPlotCommand { get; set; }
        private void OpenOxyPlot(object parameter)
        {
            if (Global.OxyPlotWindowViewModel == null)
            {
                new OxyPlotWindow().Show();
            }
        }
        public DelegateCommand LegacyExportCommand { get; set; }

        private void LegacyExport(object parameter)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "export" +
                           DateTime.Now.ToString("yyyyMMdd");
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel 工作簿|*.xlsx";
            dlg.Title = "导出";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                Global.Core.LegacyExport(dlg.FileName);
                ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe");
                psi.Arguments = "/e,/select," + dlg.FileName;
                Process.Start(psi);
            }
        }
        public DelegateCommand ExportCommand { get; set; }
        private void Export(object parameter)
        {
            new ExportWindow().ShowDialog();
        }
        public DelegateCommand RestartCommand { get; set; }
        private void Restart(object parameter)
        {
            Process p = new Process();
            p.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + "FMS.exe";
            p.StartInfo.UseShellExecute = false;
            p.Start();
            Application.Current.Shutdown();
            
        }

        public DelegateCommand AboutCommand { get; set; }

        private void About(object parameter)
        {
            new AboutWindow().Show();
        }
        public DelegateCommand SettingsCommand { get; set; }

        private void Settings(object parameter)
        {
            new SettingsWindow().Show();
        }
        public DelegateCommand DebugCommand { get; set; }

        private void Debug(object parameter)
        {
            Window window = new Window();
            TextBox tb = new TextBox();
            tb.Text = "";
            //foreach (var item in Global.Core.NameItems)
            //{
            //    tb.Text += string.Format("{0}\t{1}\n", item.Name,
            //        item.EffectiveListByName.Max(x=>x.Point));
            //}
            ////foreach (var item in Global.Core.DateItems)
            //{
            //    tb.Text += string.Format("{0}\t{1}\t{2}\n", item.DigitalDate.ToString(),
            //        item.EffectiveListByDate.First().Name, item.EffectiveListByDate.First().Point.ToString());
            //}
            //
            //MessageBox.Show(Global.Core.Dates.Contains(new Date {DigitalDate = 20220422, Title = "rebirth"})
            //    .ToString());
            //for (int i = 0; i < 10; i++)
            //{
            //    DateItem dateItem = Core.DateItems[i];
            //    tb.Text += string.Format("{0}.{1}.{2}");
            //    foreach (var item in dateItem.EffectiveListByDate)
            //    {
            //        tb.Text += item.Rank.ToString();
            //        tb.Text += item.Name;
            //        tb.Text += item.Point.ToString();
            //        tb.Text += " ";
            //        if (item.Change == StatusCode.NEW || item.Change == StatusCode.BACK)
            //        {
            //            tb.Text += item.Change;
            //        }
            //        tb.Text += "\n";
            //    }

            //    tb.Text += "==========\n";
            //}
            //tb.Text += (Process.GetCurrentProcess().WorkingSet64/1024/1024).ToString();
            window.Content = tb;
            window.Show();
        }
        public DelegateCommand OpenInSystemCommand { get; set; }

        private void OpenInSystem(object parameter)
        {
            Process open = new Process();
            open.StartInfo.UseShellExecute = true;
            open.StartInfo.FileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "core.xlsx";
            open.Start();
        }
        public DelegateCommand ExitCommand { get; set; }

        private void Exit(object parameter)
        {
            Environment.Exit(0);
        }
        private double startTime;

        public double StartTime
        {
            get { return startTime; }
            set
            {
                startTime = value;
                OnPropertyChanged(nameof(StartTime));
            }
        }
        void GetStartTime()
        {
            StartTime = (DateTime.Now - Process.GetCurrentProcess().StartTime).TotalSeconds;
        }
        public DelegateCommand RawViewCommand { get; set; }

        private void RawView(object parameter)
        {
            new RawViewWindow().Show();
        }

        public DelegateCommand SQLCommand { get; set; }

        private void SQL(object parameter)
        {
            new SQLWindow().Show();
        }
        public DelegateCommand RefreshCommand { get; set; }

        private void Refresh(object parameter)
        {
            Global.Core = new Core(new DataBase());
        }
        public MainWindowViewModel()
        {
            AddDateItemCommand = new DelegateCommand(AddDateItem);
            AddNameItemCommand = new DelegateCommand(AddNameItem);
            SaveCommand = new DelegateCommand(Save);
            ImportCommand = new DelegateCommand(Import);
            ExportCommand = new DelegateCommand(Export);
            AboutCommand = new DelegateCommand(About);
            DebugCommand = new DelegateCommand(Debug);
            OpenInSystemCommand = new DelegateCommand(OpenInSystem);
            ExitCommand = new DelegateCommand(Exit);
            RestartCommand = new DelegateCommand(Restart);
            SettingsCommand = new DelegateCommand(Settings);
            LegacyExportCommand = new DelegateCommand(LegacyExport);
            OpenOxyPlotCommand = new DelegateCommand(OpenOxyPlot);
            RawViewCommand = new DelegateCommand(RawView);
            SQLCommand = new DelegateCommand(SQL);
            RefreshCommand = new DelegateCommand(Refresh);
            GetStartTime();
            Memory = (int)(Process.GetCurrentProcess().WorkingSet64 / 1024/1024);
        }
    }
}

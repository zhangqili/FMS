using FMS.Commands;
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
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "result" +
                DateTime.Now.ToString("yyyyMMdd");
            dlg.DefaultExt = ".xlsx";
            dlg.Filter ="Excel 工作簿|*.xlsx";
            dlg.Title="输出总榜";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                System.IO.File.WriteAllText(dlg.FileName, string.Empty);
                int count = Global.Core.DateItems.Count;
                XSSFWorkbook workbook = new XSSFWorkbook();
                //创建表
                ISheet sheet = workbook.CreateSheet();
                sheet.CreateFreezePane(1, 1, 1, 1);
                IRow topRow = sheet.CreateRow(0);
                for (int i = 0; i < count; i++)
                {
                    topRow.CreateCell(i + 1).SetCellValue(Global.Core.DateItems[i].DigitalDate);
                }
                topRow.CreateCell(count + 1).SetCellValue("上榜次数");
                topRow.CreateCell(count + 2).SetCellValue("总和");

                int time = 0;

                foreach (NameItem nameItem in Global.Core.NameItems)
                {
                    IRow cells = sheet.CreateRow(time + 1);
                    cells.CreateCell(0).SetCellValue(nameItem.Name);
                    if (nameItem.ListByName != null)
                        for (int i = 0; i < nameItem.ListByName.Count; i++)
                        {
                            cells.CreateCell(i + 1).SetCellValue(nameItem.ListByName[i].Score);
                            cells.CreateCell(count + 1).SetCellValue(nameItem.Count);
                        }
                    ICell sumCell = cells.CreateCell(count + 2);
                    sumCell.SetCellValue(nameItem.Sum);
                    time++;
                }

                using (FileStream fileStream = File.OpenWrite(dlg.FileName))
                {
                    workbook.Write(fileStream);
                }
                ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe");
                psi.Arguments = "/e,/select," + dlg.FileName;
                Process.Start(psi);
            }
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
            foreach (var item in Global.Core.InteralList)
            {
                tb.Text += string.Format("{0}\t{1}\t{2}\n", item.Name,
                    item.Rank,item.DigitalDate);
            }
            //foreach (var item in Global.Core.NameItems)
            //{
            //    tb.Text += string.Format("{0}\t{1}\n", item.Name,
            //        item.EffectiveListByName.Max(x=>x.Score));
            //}
            ////foreach (var item in Global.Core.DateItems)
            //{
            //    tb.Text += string.Format("{0}\t{1}\t{2}\n", item.DigitalDate.ToString(),
            //        item.EffectiveListByDate.First().Name, item.EffectiveListByDate.First().Score.ToString());
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
            //        tb.Text += item.Score.ToString();
            //        tb.Text += " ";
            //        if (item.Change == "NEW" || item.Change == "BACK")
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
            GetStartTime();
            Memory = (int)(Process.GetCurrentProcess().WorkingSet64 / 1024/1024);
        }
    }
}

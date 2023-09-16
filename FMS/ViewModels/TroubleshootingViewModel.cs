using FMS.Commands;
using FMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Lib;
using Microsoft.Win32;
using NPOI.HPSF;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace FMS.ViewModels
{
    internal class TroubleshootingViewModel
    {
        public DelegateCommand RebuildDatabaseCommand { get; set; }

        public DelegateCommand ChooseDatabaseCommand { get; set; }

        private void RebuildDatabase(object parameter)
        {
            DataBase.RebuildDatabase();
            Global.Core = new Core(new DataBase());
        }
        public DelegateCommand RefreshNameItemCommand { get; set; }
        private void ChooseDatabase(object parameter)
        {

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "SQLite 数据库文件|*.db";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {

                FileInfo file = new FileInfo(dlg.FileName);
                if (file.Exists) //可以判断源文件是否存在
                {
                    // 这里是true的话覆盖
                    file.CopyTo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase+"FMS.db", true);
                }

                try
                {
                    Global.Core = new Core(new DataBase());
                }
                catch (Exception e)
                {
                    MessageBox.Show("数据库文件错误");
                    Process.GetCurrentProcess().Kill();
                }
            }
        }
        public TroubleshootingViewModel()
        {
            RebuildDatabaseCommand = new DelegateCommand(RebuildDatabase);
            ChooseDatabaseCommand = new DelegateCommand(ChooseDatabase);
        }
    }
}

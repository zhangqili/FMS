using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FMS.Commands;
using FMS.Lib;
using Microsoft.Win32;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace FMS.ViewModels
{
    internal class SQLWindowViewModel :NotificationObject
    {
        public DataBase DataBase { get; set; }
        private DataTable dataTable;

        public DataTable DataTable
        {
            get { return dataTable; }
            set
            {
                dataTable = value;
                OnPropertyChanged(nameof(DataTable));
            }
        }

        private string sQLCode;

        public string SQLCode
        {
            get { return sQLCode; }
            set
            {
                sQLCode = value;
                OnPropertyChanged(nameof(SQLCode));
            }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private ObservableCollection<string> codes;
        public ObservableCollection<string> Codes
        {
            get { return codes; }
            set
            {
                codes = value;
                OnPropertyChanged(nameof(Codes));
            }
        }

        public DelegateCommand ExecuteCommand { get; set; }

        void Execute(object parameter)
        {
            try
            {
                DataTable.Dispose();
                DataTable = DataBase.ExecuteReader(SQLCode);
                Message = "SQL query succeeded\n";
            }
            catch (Exception e)
            {
                Message = e.Message + "\n";
            }
        }
        public DelegateCommand ExportCommand { get; set; }

        void Export(object parameter)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "SQL_" +
                DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel 工作簿|*.xlsx";
            dlg.Title = "输出";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                System.IO.File.WriteAllText(dlg.FileName, string.Empty);
                int count = Global.Core.DateItems.Count;
                XSSFWorkbook workbook = new XSSFWorkbook();
                //创建表
                ISheet sheet = workbook.CreateSheet();
                sheet.CreateFreezePane(0,1);
                IRow topRow = sheet.CreateRow(0);
                for (int i = 0; i < DataTable.Columns.Count; i++)
                {
                    topRow.CreateCell(i).SetCellValue(DataTable.Columns[i].ColumnName);
                }

                for (int i = 0; i < DataTable.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    for (int j = 0; j < DataTable.Columns.Count; j++)
                    {
                        double t;
                        if (double.TryParse(DataTable.Rows[i][j].ToString(),out t))
                            row.CreateCell(j).SetCellValue(t);
                        else
                            row.CreateCell(j).SetCellValue(DataTable.Rows[i][j].ToString());
                    }
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

        public DelegateCommand AddCommand { get; set; }

        void Add(object parameter)
        {
            if (!Codes.ToList().Exists(x => x == SQLCode))
            {
                Codes.Add(SQLCode);
                Global.Core.DataBase.AddSQLCode(SQLCode);
            };
        }
        public DelegateCommand RemoveCommand { get; set; }
        void Remove(object parameter)
        {
            Codes.Remove(SQLCode);
            Global.Core.DataBase.RemoveSQLCode(SQLCode);
        }
        public DelegateCommand SelectCommand { get; set; }
        void Select(object parameter)
        {
            SQLCode = (string)parameter;
        }
        internal SQLWindowViewModel()
        {
            DataTable = new();
            DataBase = Global.Core.DataBase;
            Codes = new ObservableCollection<string>(Global.Core.DataBase.GetCodes());
            Message = "";
            SQLCode = "SELECT * FROM source";
            Execute(null);
            ExecuteCommand = new DelegateCommand(Execute);
            ExportCommand = new DelegateCommand(Export);
            AddCommand = new DelegateCommand(Add);
            RemoveCommand = new DelegateCommand(Remove);
            SelectCommand = new DelegateCommand(Select);
        }
    }
}

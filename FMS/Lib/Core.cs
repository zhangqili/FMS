using FMS.Lib;
using FMS.Models;
using Microsoft.Win32;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using NPOI.SS.Formula.Functions;

namespace FMS.Lib
{
    public class Core
    {
        public XSSFWorkbook Workbook { get; set; }
        public ISheet CoreSheet { get; set; }
        public ISheet TitleSheet { get; set; }
        public ISheet SpecialSheet { get; set; }
        public ISheet BlackListSheet { get; set; }
        public ISheet FavoriteGroupSheet { get; set; }
        public string BackupUrl
        {
            get
            {
                return AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "core.backup.xlsx";
            }
            set
            {

            }
        }
        public List<string> Names { get; set; }
        public List<Date> Dates { get; set; }
        private List<string> TempNames { get; set; }
        private List<int> TempDigitalDates { get; set; }
        public List<Item> Items { get; set; }
        public List<Item> SpecialList { get; set; }
        public List<DateItem> DateItems { get; set; }
        public List<DateItem> DescDateItems { get; set; }
        public List<NameItem> NameItems { get; set; }
        public Dictionary<string, List<string>> FavoriteGroups { get; set; }
        public List<string> BlackList { get; set; }
        public List<Item> InteralList { get; set; }
        public ObservableCollection<DateItem> ObservableCollectionOfDateItems { get; set; }
        public ObservableCollection<NameItem> ObservableCollectionOfNameItems { get; set; }
        public DataBase DataBase { get; set; }

        public Core(string url)
        {
            Backup(url);
            OpenFile(url);
            Items = GetItems();
            Dates = GetDates();
            SpecialList = GetSpecialList();
            NameItems = GroupByName(Items);
            AnalyzeChange(NameItems);
            DateItems = GroupByDate(Items);
            Names = GetNames();
            //DescDateItems = DateItems.OrderByDescending(x => x.DigitalDate).ToList();
            FavoriteGroups = GetFavoriteGroups();
            BlackList = GetBlackList();
            ObservableCollectionOfDateItems = new ObservableCollection<DateItem>(DateItems);
            ObservableCollectionOfNameItems = new ObservableCollection<NameItem>(NameItems);
            new DataBase().Update(Items, Dates);
            //BenchmarkPoint = 0;
            //MessageBox.Show(Items.Count.ToString());
        }

        public Core(DataBase db)
        {
            DataBase = db;
            Items = db.GetItems();
            Dates = db.GetDates();
            //SpecialList = GetSpecialList();
            SpecialList = new List<Item>();
            NameItems = GroupByName(Items);
            AnalyzeChange(NameItems);
            DateItems = GroupByDate(Items);
            Names = GetNames();
            //DescDateItems = DateItems.OrderByDescending(x => x.DigitalDate).ToList();
            //FavoriteGroups = GetFavoriteGroups();
            FavoriteGroups = new Dictionary<string, List<string>>();
            //BlackList = GetBlackList();
            BlackList = new List<string>();
            ObservableCollectionOfDateItems = new ObservableCollection<DateItem>(DateItems);
            ObservableCollectionOfNameItems = new ObservableCollection<NameItem>(NameItems);
            //BenchmarkPoint = 0;
            //MessageBox.Show(Items.Count.ToString());
        }

        private static void AnalyzeChanges(List<DateItem> dateItems)
        {
            foreach (Item item in dateItems[0].ListByDate)
            {
                switch (item.Point)
                {
                    case > 0:
                        item.Code = StatusCode.NEW;
                        break;
                    case <= 0:
                        item.Code = StatusCode.NA;
                        break;
                    default:
                        break;
                }
            }
            for (int i = 1; i < dateItems.Count; i++)
            {
                List<Item> itemsBefore = dateItems[i - 1].ListByDate;
                List<Item> items = dateItems[i].ListByDate;

            }
        }

        public void AnalyzeChange(List<Item> items, List<Item> itemsBefore)
        {
            foreach (Item item in items)
            {
                Item oldItem = itemsBefore.Find(x => x.Name == item.Name);
                NameItem nameItem = FindNameItemByName(item.Name);
                bool Existed = nameItem.FirstDate != DateTime.MinValue;
                if (item.Rank == 0)
                {
                    if (Existed == true)
                    {
                        if (oldItem.Rank == 0)
                        {
                            item.Code = StatusCode.NA;
                        }
                        else
                        {
                            item.Code = StatusCode.OUT;
                        }
                    }
                    else
                    {
                        item.Code = StatusCode.NA;
                    }
                }
                else
                {
                    if (Existed == false)
                    {
                        Existed = true;
                        item.Code = StatusCode.NEW;
                        goto labellabellabel;
                    }
                    if (oldItem.Rank == 0)
                    {
                        item.Code = StatusCode.BACK;
                        goto labellabellabel;
                    }
                    int change = item.Rank - oldItem.Rank;
                    switch (change)
                    {
                        case > 0:
                            item.Change = change;
                            break;
                        case < 0:
                            item.Change = change;
                            break;
                        case 0:
                            item.Change = 0;
                            break;
                        default:
                            //break;
                    }
                }
            labellabellabel:;

            }
        }

        public static void AnalyzeChange(List<NameItem> nameItems)
        {
            foreach (NameItem nameItem in nameItems)
            {
                bool Existed = false;
                if (nameItem.ListByName != null)
                    for (int i = 0; i < nameItem.ListByName.Count; i++)
                    {
                        Item item = nameItem.ListByName[i];
                        if (item.Rank == 0)
                        {
                            if (Existed == true)
                            {
                                if (nameItem.ListByName[i - 1].Rank == 0)
                                {
                                    item.Code = StatusCode.NA;
                                }
                                else
                                {
                                    item.Code = StatusCode.OUT;
                                }
                            }
                            else
                            {
                                item.Code = StatusCode.NA;
                            }
                        }
                        else
                        {
                            if (Existed == false)
                            {
                                Existed = true;
                                item.Code = StatusCode.NEW;
                                goto labellabellabel;
                            }
                            if (nameItem.ListByName[i - 1].Rank == 0)
                            {
                                item.Code = StatusCode.BACK;
                                goto labellabellabel;
                            }
                            int change = item.Rank - nameItem.ListByName[i - 1].Rank;
                            switch (change)
                            {
                                case > 0:
                                    item.Change = change;
                                    item.Code = StatusCode.NORM;
                                    break;
                                case < 0:
                                    item.Change = change;
                                    item.Code = StatusCode.NORM;
                                    break;
                                case 0:
                                    item.Change = 0;
                                    item.Code = StatusCode.NORM;
                                    break;
                                default:
                                    //break;
                            }
                        }
                    labellabellabel:;

                    }
            }
        }

        public void CheckFile(string filePath, out List<string> names, out List<int> digitalDates)
        {
            names = new List<string>();
            digitalDates = new List<int>();
            try
            {
                XSSFWorkbook Workbook = new XSSFWorkbook(new FileStream(filePath, FileMode.Open, FileAccess.Read));
                ISheet sourceSheet = Workbook.GetSheetAt(0);
                //建立姓名列表
                for (int x = 0; x < sourceSheet.GetRow(0).LastCellNum; x += 3)
                {
                    for (int y = 1; y <= sourceSheet.LastRowNum; y++)
                    {
                        if (Core.Cell(sourceSheet, x, y) != null && Cell(sourceSheet, x + 1, y) != null)
                        {
                            string s = Core.Cell(sourceSheet, x, y).ToString();
                            if (s != "")
                            {
                                if (!names.Exists(x => x == s))
                                {
                                    names.Add(s);
                                }
                            }
                        }
                    }
                }
                //建立数字日期列表
                for (int x = 0; x < sourceSheet.GetRow(0).LastCellNum; x += 3)
                {
                    ICell cell = Cell(sourceSheet, x, 0);
                    if (cell.ToString() != "")
                        digitalDates.Add(Convert.ToInt32(cell.ToString()));
                }
                TempNames = names;
                TempDigitalDates = digitalDates;
            }
            catch (Exception)
            {
                MessageBox.Show("文件错误");
            }
        }
        public void Import(string path)
        {
            XSSFWorkbook sourceSheets = new XSSFWorkbook(new FileStream(path, FileMode.Open, FileAccess.Read));
            ISheet sourceSheet = sourceSheets.GetSheetAt(0);
            //初始化所有item，所有分数及排名为0
            Initialize(TempDigitalDates, TempNames);
            //填充分数及排名
            for (int x = 0; x < sourceSheet.GetRow(0).LastCellNum; x += 3)
            {
                ICell cell = Cell(sourceSheet, x, 0);
                if (cell.ToString() != "")
                {
                    int digitalDate = Convert.ToInt32(cell.ToString());
                    for (int y = 1; y <= sourceSheet.LastRowNum; y++)
                    {
                        if (Cell(sourceSheet, x, y) != null && Cell(sourceSheet, x + 1, y) != null)
                        {
                            string name = Cell(sourceSheet, x, y).ToString();
                            if (y == 50)
                            {
                                Console.WriteLine();
                            }
                            if (name != "")
                            {
                                Item item = Items.Find(d => d.Name == name && d.DigitalDate == digitalDate);
                                item.Point = Cell(sourceSheet, x + 1, y).NumericCellValue;
                                item.Rank = y;
                            }
                        }
                        else if (Cell(sourceSheet, x, y) != null)
                        {
                            AddDate(digitalDate, Cell(sourceSheet, x, y).StringCellValue);
                        }
                    }
                }
            }
            NameItems = GroupByName(Items);
            AnalyzeChange(NameItems);
            Save();
        }

        public void LegacyExport(string path)
        {
            File.WriteAllText(path, string.Empty);
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            int maxHeight = DateItems.Max(x => x.Count) + 2;
            for (int i = 0; i < maxHeight; i++)
            {
                sheet.CreateRow(i);
            }
            IRow topRow = sheet.GetRow(0);
            for (int i = 0; i < DateItems.Count; i++)
            {
                DateItem item = DateItems[i];
                topRow.CreateCell(i * 3).SetCellValue(item.DigitalDate);
                for (int j = 0; j < item.Count; j++)
                {
                    sheet.GetRow(j + 1).CreateCell(i * 3).SetCellValue(item.EffectiveListByDate[j].Name);
                    sheet.GetRow(j + 1).CreateCell(i * 3 + 1).SetCellValue(item.EffectiveListByDate[j].Point);
                }
                sheet.GetRow(item.Count + 1).CreateCell(i * 3).SetCellValue(item.Title);
            }
            using (FileStream fileStream = File.OpenWrite(path))
            {
                workbook.Write(fileStream);
            }
        }

        public void Export(string url)
        {
            System.IO.File.WriteAllText(url, string.Empty);
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
                        cells.CreateCell(i + 1).SetCellValue(nameItem.ListByName[i].Point);
                        cells.CreateCell(count + 1).SetCellValue(nameItem.Count);
                    }
                ICell sumCell = cells.CreateCell(count + 2);
                sumCell.SetCellValue(nameItem.Sum);
                time++;
            }

            using (FileStream fileStream = File.OpenWrite(url))
            {
                workbook.Write(fileStream);
            }
            ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + url;
            Process.Start(psi);

        }
        public static List<Item> ImportListFromText(string mass)
        {
            int startValue = 1;
            List<Item> items = new List<Item>();
            try
            {
                mass = mass.ToLower();
                mass = mass.Replace("new", "");
                mass = mass.Replace("back", "");
                mass = mass.Replace("out", "");
                mass = mass.Replace(" ", "");
                mass = mass.Replace("\t", "");
                string[] list = mass.Split('\n');
                int digitalDate = 0;
                int dateEnd = 0;
                try
                {
                    string topLine = list[0];
                    char[] topvs = topLine.ToCharArray();
                    for (int i = 0; i < topvs.Length; i++)
                    {
                        if (topvs[i] <= '9' && topvs[i] >= '0' && i < 12)
                        {
                            dateEnd = i;
                        }
                    }
                    string[] dateCollection = topLine.Substring(0, dateEnd + 1).Split(".");
                    int year, month, day;
                    year = int.Parse(dateCollection[0]);
                    month = int.Parse(dateCollection[1]);
                    day = int.Parse(dateCollection[2]);
                    digitalDate = year * 10000 + month * 100 + day;
                }
                catch (Exception)
                {
                    startValue = 0;
                }
                for (int i = startValue; i < list.Length; i++)
                {
                    int nameStart = 0, nameEnd = 0;
                    char[] vs = list[i].ToArray();
                    string line = list[i];
                    double point;
                    string name;
                    for (int j = 0; j < vs.Length; j++)
                    {
                        if (!(vs[j] >= '0' && vs[j] <= '9' || vs[j] == '.' || vs[j] == '\r'))
                        {
                            nameStart = j;
                            break;
                        }
                    }
                    for (int j = vs.Length - 1; j >= 0; j--)
                    {
                        if (!(vs[j] >= '0' && vs[j] <= '9' || vs[j] == '.' || vs[j] == '\r'))
                        {
                            nameEnd = j + 1;
                            break;
                        }
                    }
                    if (nameEnd > 0)
                    {
                        try
                        {
                            name = line.Substring(nameStart, nameEnd - nameStart);
                            point = double.Parse(line.Remove(0, nameEnd));
                            Item item = new Item() { Name = name, Point = point, DigitalDate = digitalDate };
                            items.Add(item);
                        }
                        catch (Exception)
                        {
                            Item item = new Item() { Name = "null", Point = 0, DigitalDate = digitalDate };
                            items.Add(item);
                        }
                    }
                }
                items = items.OrderByDescending(x => x.Point).ToList();
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Point != 0)
                    {
                        items[i].Rank = i + 1;
                    }
                }
            }
            catch (Exception)
            {
                List<Item> items1 = new List<Item>();
                items1.Add(new Item() { Name = "null" });
                return items1;
            }
            return items;
        }
        public static List<Item> UpdateItems(List<Item> newItems, List<Item> items)
        {
            foreach (Item item in newItems)
            {
                item.DigitalDate = items[0].DigitalDate;
            }
            for (int i = 0; i < newItems.Count; i++)
            {
                if (items.Exists(x => x.Name == newItems[i].Name))
                {
                    newItems[i] = items.Find(x => x.Name == newItems[i].Name);
                    items.Remove(newItems[i]);
                }
                else
                {
                    newItems[i].Point = 0;
                    newItems[i].Rank = 0;
                }
            }
            foreach (Item item in items)
            {
                newItems.Add(item);
            }
            return newItems;
        }
        public void Save()
        {
            XSSFWorkbook newWorkbook = new XSSFWorkbook();
            newWorkbook.CreateSheet("源");
            newWorkbook.CreateSheet("标题");
            if (SpecialSheet != null)
                SpecialSheet.CopyTo(newWorkbook, "特殊", true, true);
            if (BlackListSheet != null)
                BlackListSheet.CopyTo(newWorkbook, "黑名单", true, true);
            if (FavoriteGroupSheet != null)
                FavoriteGroupSheet.CopyTo(newWorkbook, "分组", true, true);
            //源
            ISheet sheet0 = newWorkbook.GetSheetAt(0);
            IRow topRow0 = sheet0.CreateRow(0);
            topRow0.CreateCell(0).SetCellValue("姓名");
            topRow0.CreateCell(1).SetCellValue("分数");
            topRow0.CreateCell(2).SetCellValue("排名");
            topRow0.CreateCell(3).SetCellValue("日期");
            topRow0.CreateCell(4).SetCellValue("变化");
            topRow0.CreateCell(5).SetCellValue("代码");
            for (int i = 0; i < Items.Count; i++)
            {
                Item item = Items[i];
                IRow cells = sheet0.CreateRow(i + 1);
                cells.CreateCell(0).SetCellValue(item.Name);
                cells.CreateCell(1).SetCellValue(item.Point);
                cells.CreateCell(2).SetCellValue(item.Rank);
                cells.CreateCell(3).SetCellValue(item.DigitalDate);
                if (item.Code != null)
                {
                    cells.CreateCell(4).SetCellValue(item.Change);
                    cells.CreateCell(5).SetCellValue((int)item.Code);
                }
                else
                {
                    cells.CreateCell(4).SetCellValue("null");
                    cells.CreateCell(5).SetCellValue("null");
                }
            }
            //标题
            ISheet sheet1 = newWorkbook.GetSheetAt(1);
            IRow topRow1 = sheet1.CreateRow(0);
            Dates = Dates.OrderBy(x => x.DigitalDate).ToList();
            topRow1.CreateCell(0).SetCellValue("日期");
            topRow1.CreateCell(1).SetCellValue("标题");
            for (int i = 0; i < Dates.Count; i++)
            {
                Date item = Dates[i];
                IRow cells = sheet1.CreateRow(i + 1);
                cells.CreateCell(0).SetCellValue(item.DigitalDate);
                cells.CreateCell(1).SetCellValue(item.Title);
            }
            try
            {
                File.WriteAllText(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "core.xlsx", string.Empty);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            using (FileStream fileStream = File.OpenWrite(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "core.xlsx"))
            {
                newWorkbook.Write(fileStream);
            }
            DataBase.Update(Items, Dates);
        }

        private void AddItems(ISheet sheet)
        {
            string name;
            IRow topRow = sheet.GetRow(0);
            for (int y = 1; y < sheet.LastRowNum; y++)
            {
                IRow cells = sheet.GetRow(y);
                name = cells.GetCell(0).StringCellValue;
                for (int x = 1; x < topRow.LastCellNum; x++)
                {
                    Item item = Items.Find(d => d.Name == name && d.DigitalDate == Convert.ToInt32(topRow.GetCell(x).NumericCellValue));
                    item.Point = Cell(CoreSheet, x, y).NumericCellValue;
                }
            }
        }
        public void AddNameItem(string name)
        {
            if (Names.Exists(x => x == name))
            {
                MessageBox.Show("已有此姓名");
                return;
            }
            NameItem nameItem = new NameItem() { Name = name, ListByName = new List<Item>() };
            foreach (DateItem dateItem in DateItems)
            {
                Item item = new Item() { Name = name, DigitalDate = dateItem.DigitalDate, Rank = 0, Point = 0, Code = StatusCode.NA };
                Items.Add(item);
                dateItem.ListByDate.Add(item);
                nameItem.ListByName.Add(item);
            }
            NameItems.Add(nameItem);
            ObservableCollectionOfNameItems.Add(nameItem);
            Names.Add(name);
        }

        public void AddDateItem(DateItem dateItem)
        {
            if (DateItems.Exists(x => x.DigitalDate == dateItem.DigitalDate))
            {
                DeleteDateItem(dateItem.DigitalDate);
            }
            foreach (Item item in dateItem.ListByDate)
            {
                item.DigitalDate = dateItem.DigitalDate;
            }
            DateItems.Add(dateItem);
            ObservableCollectionOfDateItems.Add(dateItem);
            Items = Items.Concat(dateItem.ListByDate).ToList();
            foreach (Item item in dateItem.ListByDate)
            {
                NameItems.Find(x => x.Name == item.Name).ListByName.Add(item);
            }
            Dates.Add(new Date() { DigitalDate = dateItem.DigitalDate, Title = dateItem.Title });
        }

        public void AddDate(Date date)
        {
            if (!Dates.Contains(date))
                Dates.Add(date);
        }

        public void AddDate(int ddate, string title = "")
        {
            Date date = new Date() { DigitalDate = ddate, Title = title };
            if (!Dates.Contains(date))
                Dates.Add(date);
        }
        public void DeleteDateItem(int digitalDate)
        {
            Items.RemoveAll(x => x.DigitalDate == digitalDate);
            foreach (DateItem dateItem in ObservableCollectionOfDateItems)
            {
                if (dateItem.DigitalDate == digitalDate)
                {
                    ObservableCollectionOfDateItems.Remove(dateItem);
                    break;
                }
            }
            Dates.RemoveAll(x => x.DigitalDate == digitalDate);
            DateItems.RemoveAll(x => x.DigitalDate == digitalDate);
        }


        private void OpenFile(string url)
        {
            try
            {
                Workbook = new XSSFWorkbook(new FileStream(url, FileMode.Open, FileAccess.Read));
                CoreSheet = Workbook.GetSheetAt(0);
                TitleSheet = Workbook.GetSheetAt(1);
                SpecialSheet = Workbook.GetSheetAt(2);
                BlackListSheet = Workbook.GetSheetAt(3);
                FavoriteGroupSheet = Workbook.GetSheetAt(4);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }
        }
        void Backup(string url)
        {
            File.Copy(url, BackupUrl, true);
        }
        void Recovery(string url)
        {
            File.Copy(BackupUrl, url, true);
        }
        List<Item> GetItems()
        {
            List<Item> items = new List<Item>(CoreSheet.LastRowNum);
            for (int i = 1; i <= CoreSheet.LastRowNum; i++)
            {
                IRow cells = CoreSheet.GetRow(i);
                items.Add(new Item()
                {
                    Name = cells.Cells[0].StringCellValue,
                    Point = cells.Cells[1].NumericCellValue,
                    Rank = Convert.ToInt32(cells.Cells[2].NumericCellValue),
                    DigitalDate = Convert.ToInt32(cells.Cells[3].NumericCellValue),
                    //Change = cells.Cells[4].ToString()
                });
            }
            return items;
        }
        List<string> GetNames()
        {
            List<string> vs = new List<string>();
            foreach (NameItem item in NameItems)
            {
                vs.Add(item.Name);
            }
            return vs;
        }
        private List<Date> GetDates()
        {
            List<Date> dates = new List<Date>();
            for (int i = 1; i <= TitleSheet.LastRowNum; i++)
            {
                IRow cells = TitleSheet.GetRow(i);
                dates.Add(new Date()
                {
                    DigitalDate = Convert.ToInt32(cells.GetCell(0).NumericCellValue),
                    Title = cells.GetCell(1) != null ? cells.GetCell(1).StringCellValue : ""
                });
            }
            return dates;
        }
        internal string GetDateTitle(int digitalDate)
        {
            return Dates.Find(x => { return x.DigitalDate == digitalDate; }).Title;
        }
        private List<Item> GetSpecialList()
        {
            List<Item> items = new List<Item>();
            for (int i = 1; i <= SpecialSheet.LastRowNum; i++)
            {
                IRow cells = SpecialSheet.GetRow(i);
                if (cells.GetCell(0).ToString() != "")
                    items.Add(new Item()
                    {
                        Name = cells.GetCell(0).StringCellValue,
                        Point = Convert.ToInt32(cells.GetCell(1).NumericCellValue)
                    });
            }
            return items;
        }
        private List<string> GetBlackList()
        {
            List<string> vs = new List<string>();
            if (BlackListSheet.GetRow(0) != null && BlackListSheet.GetRow(0).GetCell(0) != null)
                for (int i = 0; i <= BlackListSheet.LastRowNum; i++)
                {

                    vs.Add(BlackListSheet.GetRow(i).GetCell(0).StringCellValue);
                }
            return vs;
        }
        private Dictionary<string, List<string>> GetFavoriteGroups()
        {
            Dictionary<string, List<string>> keyValuePairs = new Dictionary<string, List<string>>();
            for (int i = 0; i <= FavoriteGroupSheet.LastRowNum; i++)
            {
                IRow cells = FavoriteGroupSheet.GetRow(i);
                List<string> vs = new List<string>();
                string groupName = cells.Cells[0].ToString();
                for (int j = 1; j < cells.LastCellNum; j++)
                {
                    vs.Add(cells.Cells[j].StringCellValue);
                }
                keyValuePairs.Add(groupName, vs);
            }
            return keyValuePairs;
        }
        private NameItem FindNameItemByName(string name)
        {
            if (NameItems.Exists(x => x.Name == name))
                return NameItems.Find(x => x.Name == name);
            else
                return null;
        }
        public List<DateItem> GroupByDate(List<Item> items)
        {
            List<DateItem> dateItems = new List<DateItem>();
            var groups = from x in items
                         orderby x.DigitalDate
                         group x by x.DigitalDate;
            foreach (var group in groups)
            {
                DateItem dateItem = new DateItem();
                dateItem.DigitalDate = group.Key;
                if (group.Key == 0)
                {
                    foreach (var item in group)
                    {
                        MessageBox.Show(item.Name + item.Point.ToString());
                    }
                }
                dateItem.Date = DigitalDateToDateTime(group.Key);
                dateItem.Title = GetDateTitle(group.Key);
                dateItem.ListByDate = (from x in @group
                                       orderby x.Point descending
                                       select x).ToList();
                dateItems.Add(dateItem);
            }
            return dateItems;
        }
        public List<NameItem> GroupByName(List<Item> items)
        {
            List<Item> localSpecialList = new List<Item>(SpecialList);
            List<NameItem> nameItems = new List<NameItem>();
            var groups = from x in items
                         group x by x.Name;
            foreach (var group in groups)
            {

                List<Item> temp = group.ToList();
                NameItem nameItem = new NameItem();
                nameItem.Name = group.Key;
                nameItem.ListByName = (from x in temp select x).ToList();
                if (SpecialList.Exists(x => x.Name == group.Key))
                {
                    nameItem.CustomValue += SpecialList.Find(x => x.Name == group.Key).Point;
                }
                for (int i = 0; i < localSpecialList.Count; i++)
                {
                    if (localSpecialList[i].Name == group.Key)
                    {
                        localSpecialList.RemoveAt(i);
                    }
                }
                nameItems.Add(nameItem);
            }
            foreach (Item item in localSpecialList)
            {
                NameItem nameItem = new NameItem()
                {
                    Name = item.Name,
                    CustomValue = item.Point,
                    FirstDate = DateTime.MinValue
                };
                nameItems.Add(nameItem);
            }
            return (from x in nameItems
                    orderby x.Sum descending
                    select x).ToList();
        }
        void Initialize(List<int> cDates, List<string> cNames)
        {
            Items = new List<Item>();
            foreach (int digitalDate in cDates)
            {
                foreach (string name in cNames)
                {
                    Items.Add(new Item() { Name = name, DigitalDate = digitalDate });
                }
            }
        }
        public void CalculateIntegalRank()
        {
            InteralList = new List<Item>();
            List<NameItem> nameItems = new List<NameItem>();
            foreach (var item in NameItems)
            {
                nameItems.Add(new NameItem() { Name = item.Name });
            }
            for (int i = 0; i < DateItems.Count; i++)
            {
                foreach (var item in DateItems[i].EffectiveListByDate)
                {
                    nameItems.First(x => x.Name == item.Name).CustomValue += item.Point;
                }
                nameItems = nameItems.OrderByDescending(x => x.CustomValue).ToList();
                for (int j = 0; j < nameItems.Count; j++)
                {
                    var item = new Item { Name = nameItems[j].Name, Rank = j + 1, DigitalDate = DateItems[i].DigitalDate, Point = nameItems[j].CustomValue };
                    if (nameItems[j].CustomValue == 0)
                    {
                        item.Rank = 0;
                    }
                    InteralList.Add(item);
                }
            }
        }
        public static ICell Cell(ISheet vs, int x, int y)
        {
            if (vs.GetRow(y).GetCell(x) != null)
                return vs.GetRow(y).GetCell(x);
            else
                return null;
        }

        public static DateTime DigitalDateToDateTime(int x)
        {
            return DateTime.ParseExact(x.ToString(), "yyyyMMdd", null);
        }
    }
}

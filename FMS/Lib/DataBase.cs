using FMS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FMS.Lib
{
    public class DataBase
    {
        public SQLiteConnection Connection { get; set; }
        public SQLiteCommand Command { get; set; }
        public string BackupUrl { get; set; }
        public string FilePath { get; set; }
        public DataBase(string url)
        {
            string cs = @"URI=file:" + url;
            Connection = new SQLiteConnection(cs);
            Connection.Open();
        }
        public DataBase()
        {
            FilePath =  AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "FMS.db";
            Connection = new SQLiteConnection(@"URI=file:" + FilePath);
            Connection.Open();
            Command = new SQLiteCommand(Connection);
            BackupUrl = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "FMS.backup.db";
            Backup(BackupUrl);
        }

        public List<string> GetNames()
        {
            List<string> names = new List<string>();
            string stm = "SELECT DISTINCT name FROM source";
            var cmd = new SQLiteCommand(stm, Connection);
            DataTable dataTable = new DataTable();
            SQLiteDataReader rdr = cmd.ExecuteReader();
            dataTable.Load(rdr);

            foreach (DataRow row in dataTable.Rows)
            {
                names.Add((string)row["name"]);
            }

            return names;
        }

        public NameItem GetNameItem(string name)
        {
            return null;
        }

        public List<DateItem> GetDateItems(List<Date> dates)
        {
            List<DateItem> dateItems = new List<DateItem>();
            foreach (var VARIABLE in dates)
            {
                dateItems.Add(GetDateItem(VARIABLE.DigitalDate));
            }
            return dateItems;
        }
        public DateItem GetDateItem(int digitalDate)
        {
            DateItem dateItem = new DateItem();
            DataTable dataTable = ExecuteReader("SELECT * FROM source where date = " + digitalDate.ToString());
            var vs = new List<Item>();
            foreach (DataRow item in dataTable.Rows)
            {
                vs.Add(ConvertToItem(item));
            }
            dateItem.ListByDate = vs;
            dateItem.DigitalDate = digitalDate;
            dateItem.Date = DigitalDateToDateTime(digitalDate);
            dateItem.Title = GetTitle(digitalDate);
            return dateItem;
        }
        public string GetTitle(int digitalDate)
        {
            DataTable dataTable = ExecuteReader("select title from title where date = " + digitalDate.ToString());
            return Convert.ToString(dataTable.Rows[0]["title"]);
        }
        private Item ConvertToItem(DataRow r)
        {
            Item item = new Item();
            item.Name = Convert.ToString(r["name"]);
            item.Point = Convert.ToDouble(r["point"]);
            item.Rank = Convert.ToInt32(r["rank"]);
            item.DigitalDate = Convert.ToInt32(r["date"]);
            item.Change = r["change"] == DBNull.Value ? 0 : Convert.ToInt32(r["change"]);
            item.Code = (StatusCode)(r["code"] == DBNull.Value ? 0 : Convert.ToInt32(r["code"]));
            return item;
        }
        public DataTable ExecuteReader(string stm)
        {
            Command.CommandText = stm;
            SQLiteDataReader rdr = Command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rdr);
            return dt;
        }

        public int ExecuteNonQuery(string stm)
        {
            Command.CommandText = stm;
            return Command.ExecuteNonQuery();
        }

        public void Update(List<Item> items, List<Date> dates)
        {

            ExecuteNonQuery("delete from source");
            ExecuteNonQuery("delete from title");
            SQLiteCommand cmd = new SQLiteCommand(Connection);
            cmd.CommandText = "pragma synchronous = 0";
            cmd.ExecuteNonQuery();
            using (SQLiteTransaction tran = Connection.BeginTransaction())
            {
                try
                {
                    using (SQLiteCommand command = new SQLiteCommand("insert into source values( @name, @point, @rank, @date, @change, @code )", Connection))
                    {
                        foreach (var VARIABLE in items)
                        {
                            if (VARIABLE.Code != null)
                            {
                                command.Parameters.Add(new SQLiteParameter("@name", VARIABLE.Name));
                                command.Parameters.Add(new SQLiteParameter("@point", VARIABLE.Point));
                                command.Parameters.Add(new SQLiteParameter("@rank", VARIABLE.Rank));
                                command.Parameters.Add(new SQLiteParameter("@date", VARIABLE.DigitalDate));
                                command.Parameters.Add(new SQLiteParameter("@change", VARIABLE.Change));
                                command.Parameters.Add(new SQLiteParameter("@code", VARIABLE.Code));
                                command.ExecuteNonQuery();
                                command.Parameters.Clear();
                            }
                        }
                    }
                    using (SQLiteCommand command = new SQLiteCommand("insert into title values( @date, @title)", Connection))
                    {
                        foreach (var VARIABLE in dates)
                        {
                            command.Parameters.Add(new SQLiteParameter("@date", VARIABLE.DigitalDate));
                            command.Parameters.Add(new SQLiteParameter("@title", VARIABLE.Title));
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                        }
                    }
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public void Insert(List<Item> items)
        {
            foreach (var VARIABLE in items)
            {
                if (VARIABLE.Code != null)
                {
                    string stm = string.Format("insert into source values(\'{0}\',{1},{2},{3},{4},{5})", VARIABLE.Name, VARIABLE.Point, VARIABLE.Rank, VARIABLE.DigitalDate, VARIABLE.Change, (int)VARIABLE.Code);
                    ExecuteNonQuery(stm);
                }
            }
        }

        public List<Item> GetItems()
        {
            return Transformat.ToItems(ExecuteReader(@"select * from source"));
        }
        public List<Date> GetDates()
        {
            return Transformat.ToDates(ExecuteReader(@"select * from title"));
        }
        public static DateTime DigitalDateToDateTime(int x)
        {
            return DateTime.ParseExact(x.ToString(), "yyyyMMdd", null);
        }

        public static void RebuildDatabase()
        {
            string cmd;
            string cs = @"URI=file:" + AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "FMS.db";
            SQLiteConnection sqLiteConnection = new SQLiteConnection(cs);
            sqLiteConnection.Open();
            SQLiteCommand sqLiteCommand = new SQLiteCommand(sqLiteConnection);
            cmd =
                "DROP TABLE IF EXISTS source";
            sqLiteCommand.CommandText = cmd;
            sqLiteCommand.ExecuteNonQuery();
            cmd =
                "DROP TABLE IF EXISTS title";
            sqLiteCommand.CommandText = cmd;
            sqLiteCommand.ExecuteNonQuery();
            cmd =
                "CREATE TABLE \"source\" (\r\n\t\"name\"\tTEXT,\r\n\t\"point\"\tNUMERIC,\r\n\t\"rank\"\tINTEGER,\r\n\t\"date\"\tINTEGER,\r\n\t\"change\"\tINTEGER\r\n, \"code\"\tINTEGER)";
            sqLiteCommand.CommandText = cmd;
            sqLiteCommand.ExecuteNonQuery();
            cmd = "CREATE TABLE \"title\" (\r\n\t\"date\"\tINTEGER,\r\n\t\"title\"\tTEXT\r\n)";
            sqLiteCommand.CommandText = cmd;
            sqLiteCommand.ExecuteNonQuery();
            sqLiteConnection.Close();
            sqLiteConnection.Dispose();
        }

        public void Backup(string url)
        {
            File.Copy(FilePath, url, true);
        }
    }

    public class Transformat
    {
        public static List<Item> ToItems(DataTable dt)
        {
            List<Item> items = new List<Item>();
            foreach (DataRow dr in dt.Rows)
            {
                Item item = new Item();
                item.Name = dr["Name"].ToString();
                item.Point = Convert.ToDouble(dr["point"]);
                item.Rank = Convert.ToInt32(dr["rank"]);
                item.DigitalDate = Convert.ToInt32(dr["date"]);
                item.Change = Convert.ToInt32(dr["change"]);
                item.Code = (StatusCode)Convert.ToInt32(dr["code"]);
                items.Add(item);
            }
            return items;
        }
        public static List<Date> ToDates(DataTable dt)
        {
            List<Date> dates = new List<Date>();
            foreach (DataRow dr in dt.Rows)
            {
                Date d = new Date();
                d.DigitalDate= Convert.ToInt32(dr["date"]);
                d.Title = dr["title"].ToString();
                dates.Add(d);
            }
            return dates;
        }
    }
}

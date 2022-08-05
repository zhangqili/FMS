using FMS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Libs
{
    internal class DataBase
    {
        public SQLiteConnection Connection { get; set; }
        internal DataBase(string url)
        {
            string cs = @"URI=file:" + url;
            Connection = new SQLiteConnection(cs);
            Connection.Open();
        }
        internal DataBase()
        {
            string cs = @"URI=file:" + AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "FMS.db";
            Connection = new SQLiteConnection(cs);
            Connection.Open();
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
            DataTable dataTable = new DataTable();
            var cmd = new SQLiteCommand(stm, Connection);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            dataTable.Load(rdr);
            return dataTable;
        }

        public int ExecuteNonQuery(string stm)
        {
            var cmd = new SQLiteCommand(stm, Connection);
            return cmd.ExecuteNonQuery();
        }

        public void Update(List<Item> items, List<Date> dates)
        {

            ExecuteNonQuery("delete from source");
            ExecuteNonQuery("delete from title");
            foreach (var VARIABLE in dates)
            {
                string stm = string.Format("insert into title values({0},\'{1}\')", VARIABLE.DigitalDate, VARIABLE.Title);
                ExecuteNonQuery(stm);
            }
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
        public static DateTime DigitalDateToDateTime(int x)
        {
            return DateTime.ParseExact(x.ToString(), "yyyyMMdd", null);
        }
    }

    internal class Transfer
    {
        internal static List<Item> ToItems(DataTable dt)
        {
            List<Item> items = new List<Item>();
            return items;
        }
    }
}

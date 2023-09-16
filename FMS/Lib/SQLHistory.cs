using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Models;

namespace FMS.Lib
{
    internal class SQLHistory
    {
        public ObservableCollection<SQLHistoryItem> HistoryItems { get; set; }

        public void AddHistoryItem(SQLHistoryItem item)
        {

        }
    }
}

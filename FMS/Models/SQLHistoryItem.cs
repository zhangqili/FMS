using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Models
{
    internal class SQLHistoryItem
    {
        public string Command { get; set; }
        public DateTime Time { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Models
{
    public enum StatusCode
    {
        NA=-1,
        NORM,
        NEW,
        OUT,
        BACK
    }
    public class Item
    {
        public string Name { get; set; }
        public int DigitalDate { get; set; }
        public double Point { get; set; }
        public int Rank { get; set; }
        public int Change { get; set; }
        public StatusCode Code { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Models
{
    public class DateItem
    {
        private List<Item> listByDate;
        public List<Item> ListByDate
        {
            get
            {
                return listByDate;
            }
            set
            {
                listByDate = (from x in value
                             orderby x.Point descending
                             select x).ToList();
                for (int i = 0; i < listByDate.Count; i++)
                {
                    if (listByDate[i].Point != 0)
                    {
                        listByDate[i].Rank = i + 1;
                    }
                    else
                    {
                        listByDate[i].Rank = 0;
                    }
                }
                EffectiveListByDate = value.Where(x => x.Point > 0 || x.Change == "OUT").ToList();
            }
        }
        public List<Item> EffectiveListByDate { get; set; }
        private int digitalDate;

        public int DigitalDate
        {
            get { return digitalDate; }
            set { digitalDate = value; }
        }
        private int count;

        public int Count
        {
            get { return EffectiveListByDate.Count(x=>x.Point>0); }
            set { count = value; }
        }

        public DateTime Date { get; set; }
        public string Title { get; set; }
    }
}

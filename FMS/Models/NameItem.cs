﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Lib;

namespace FMS.Models
{
    public class NameItem
    {
        private List<Item> listByName;
        public List<Item> ListByName
        {
            get
            {
                return listByName;
            }
            set
            {
                listByName = value;
                EffectiveListByName = (from x in value
                                       where x.Point > 0
                                       select x).ToList();
            }
        }
        public List<Item> EffectiveListByName { get; set; }
        public string Name { get; set; }
        private double sum;

        public double Sum
        {
            get
            {
                if (ListByName != null)
                {
                    /*
                    double sum = 0.0;
                    double day = (Core.DigitalDateToDateTime(ListByName.Last().DigitalDate)- Core.DigitalDateToDateTime(ListByName[0].DigitalDate)).Days;
                    for (int i = 0; i < ListByName.Count; i++)
                    {
                        double dayc = (Core.DigitalDateToDateTime(ListByName[i].DigitalDate) - Core.DigitalDateToDateTime(ListByName[0].DigitalDate)).Days;
                        //sum += ListByName[i].Point * Convert.ToDouble(i) / ListByName.Count;
                        sum += ListByName[i].Point * dayc / day;
                    }
                    */
                    return (EffectiveListByName.Sum(x => x.Point) + CustomValue);
                    //return sum+ CustomValue;
                }
                else
                    return CustomValue;
            }
            set { sum = value; }
        }

        private int count;

        public int Count
        {
            get
            {
                if (ListByName != null)
                    if (EffectiveListByName.Count != 0)
                        return EffectiveListByName.Count;
                    else
                        return 0;
                else
                    return 0;
            }
            set { count = value; }
        }

        private int peak;

        public int Peak
        {
            get
            {
                if (ListByName != null)
                    if (EffectiveListByName.Count != 0)
                        return EffectiveListByName.Min(x => x.Rank);
                    else
                        return 0;
                else
                    return 0;
            }
            set { peak = value; }
        }
        private DateTime firstDate;

        public DateTime FirstDate
        {
            get
            {
                if (ListByName != null)
                    if (EffectiveListByName.Count != 0)
                        return DateTime.ParseExact(EffectiveListByName.Find(x => x.Code == StatusCode.NEW).DigitalDate.ToString(), "yyyyMMdd", null);
                    else
                        return DateTime.MinValue;
                else
                    return DateTime.MinValue;
            }
            set { firstDate = value; }
        }
        public double CustomValue { get; set; }
        private double peakPoint;

        public double PeakPoint
        {
            get
            {
                Item item= EffectiveListByName[0];
                foreach (var VARIABLE in EffectiveListByName)
                {
                    if (VARIABLE.Rank>0 && (VARIABLE.Rank <= item.Rank && VARIABLE.Point > item.Point) || VARIABLE.Rank<item.Rank)
                    {
                        item = VARIABLE;
                    }
                }
                return item.Point;
            }
            set { peakPoint = value; }
        }
        private double highestPoint;

        public double HighestPoint
        {
            get { return EffectiveListByName.Max(x=>x.Point); }
            set { highestPoint = value; }
        }

        public NameItem()
        {
            CustomValue = 0;
        }
    }
}

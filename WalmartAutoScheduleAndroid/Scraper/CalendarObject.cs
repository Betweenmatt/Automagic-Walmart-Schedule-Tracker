using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WalmartAutoScheduleAndroid.Scraper
{
    class CalendarObject
    {
        public long Id { get; }
        public string DisplayName { get; }
        public int Color { get; }
        public string Type { get; }

        public CalendarObject(long id, string dn, int color, string type)
        {
            Id = id;
            DisplayName = dn;
            Color = color;
            Type = type;
        }
        public override string ToString()
        {
            return $"{Id} : {DisplayName} : {Color}";
        }
        
    }
}
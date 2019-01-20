using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;

namespace WalmartAutoScheduleAndroid
{
    static class Utilities
    {
        public static bool CheckCalendarPermissions(Context context)
        {
            if (ContextCompat.CheckSelfPermission(context, Android.Manifest.Permission.ReadCalendar)
                    == Android.Content.PM.Permission.Granted
                && ContextCompat.CheckSelfPermission(context, Android.Manifest.Permission.WriteCalendar)
                    == Android.Content.PM.Permission.Granted)
                return true;
            return false;
        }

        public static int GetWalmartWeek(DateTime date)
        {
            var thisyear = date.Year;
            //if month is january, then we need to get the previous year
            //because walmarts first week falls on feb 1st
            if (date.Month == 1)
                thisyear -= 1;
            var febfirst = new DateTime(thisyear,2,1);
            switch (febfirst.DayOfWeek)
            {
                case (DayOfWeek.Sunday):
                    febfirst = febfirst.AddDays(-1);
                    break;
                case (DayOfWeek.Monday):
                    febfirst = febfirst.AddDays(-2);
                    break;
                case (DayOfWeek.Tuesday):
                    febfirst = febfirst.AddDays(-3);
                    break;
                case (DayOfWeek.Wednesday):
                    febfirst = febfirst.AddDays(-4);
                    break;
                case (DayOfWeek.Thursday):
                    febfirst = febfirst.AddDays(-5);
                    break;
                case (DayOfWeek.Friday):
                    febfirst = febfirst.AddDays(-6);
                    break;
            }
            TimeSpan difference = date - febfirst;
            var output = ((int)difference.TotalDays / 7) + 1;
            return output > 52 ? 1 : output;//quick fix for the week 53 bug
        }
    }
}
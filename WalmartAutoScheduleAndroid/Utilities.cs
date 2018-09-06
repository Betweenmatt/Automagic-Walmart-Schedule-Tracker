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
    }
}
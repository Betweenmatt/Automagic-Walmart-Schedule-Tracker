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

namespace WalmartAutoScheduleAndroid
{
    class AppRateReminder
    {
        private const int _daysUntilPrompt = 3;
        private const int _launchesUntilPrompt = 3;
        private const string _appTitle = "Automagic Walmart Schedule Tracker";
        private const string _appPackageName = "com.andrewsstudios.automagicwalmartscheduletracker";

        private const string _dontShowAgain = "dontShowAgain";
        private const string _launchCount = "launchCount";
        private const string _dateFirstLaunched = "dateFirstLaunch";
        private const string _appRateReminder = "appRateReminder";

        public static void Launched(Context context)
        {
            ISharedPreferences prefs = context.GetSharedPreferences(_appRateReminder, 0);
            if (prefs.GetBoolean(_dontShowAgain, false))
            {
                return;
            }
            ISharedPreferencesEditor editor = prefs.Edit();

            long launchCount = prefs.GetLong(_launchCount, 0) + 1;
            editor.PutLong(_launchCount, launchCount);

            long dateFirstLaunch = prefs.GetLong(_dateFirstLaunched, 0);
            if(dateFirstLaunch == 0)
            {
                dateFirstLaunch = DateTime.Now.ToBinary();
                editor.PutLong(_dateFirstLaunched, dateFirstLaunch);
            }
            if(launchCount >= _launchesUntilPrompt)
            {
                var span = DateTime.Now - DateTime.FromBinary(dateFirstLaunch);
                if(span.Days >= _daysUntilPrompt)
                {
                    ShowRateDialog(context, editor);
                }
            }
            editor.Commit();
        }
        public static void ShowRateDialog(Context context, ISharedPreferencesEditor editor)
        {
            AlertDialog.Builder dialog = new AlertDialog.Builder(context);
            dialog.SetTitle($"Rate {_appTitle}");
            dialog.SetMessage("If you find this app useful, please take a moment to rate it! This app was created by a Walmart associate in his freetime, and good ratings mean the world to him! Thank you!");
            dialog.SetPositiveButton("Rate now", (s, e) =>
            {
                context.StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=" + _appPackageName)));
                editor.PutBoolean(_dontShowAgain, true);
                editor.Commit();
            });
            dialog.SetNegativeButton("Do not show again", (s, e) =>
             {
                 editor.PutBoolean(_dontShowAgain, true);
                 editor.Commit();
             });
            dialog.SetNeutralButton("Remind me later", (s, e) =>
            {});
            dialog.Show();
        }
    }
}
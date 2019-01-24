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
using WalmartAutoScheduleAndroid.Activities;

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

        private const string _changesAreComingReminder = "changesAreComingReminder";

        public static bool HasChangesAreComingTriggered(Context context)
        {
            ISharedPreferences prefs = context.GetSharedPreferences(_appRateReminder, 0);
            return prefs.GetBoolean(_changesAreComingReminder, false);
        }

        public static void SetChangesAreComingTriggered(Context context)
        {
            ISharedPreferences prefs = context.GetSharedPreferences(_appRateReminder, 0);

            bool changesAreComingBool = prefs.GetBoolean(_changesAreComingReminder, false);
            ISharedPreferencesEditor editor = prefs.Edit();
            if (!changesAreComingBool)
            {
                editor.PutBoolean(_changesAreComingReminder, true);
                editor.Commit();
            }
        }

        public static void Launched(Context context)
        {
            ISharedPreferences prefs = context.GetSharedPreferences(_appRateReminder, 0);

            bool changesAreComingBool = prefs.GetBoolean(_changesAreComingReminder, false);
            ISharedPreferencesEditor editor = prefs.Edit();
            if (!changesAreComingBool)
            {
                ChangesAreComingDialog(context, editor);
            }






            if (prefs.GetBoolean(_dontShowAgain, false))
            {
                return;
            }

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
        public static void ChangesAreComingDialog(Context context, ISharedPreferencesEditor editor)
        {
            AlertDialog.Builder dialog = new AlertDialog.Builder(context);
            dialog.SetTitle($"Important!");
            dialog.SetCancelable(false);
            dialog.SetMessage("There are some important changes coming that will stop this app from working soon. Please click Ok to read more about them!");
            dialog.SetPositiveButton("Ok", (s, e) =>
            {
                //this is hijacking the changes dialog to add push/status notifications flags to settings lol
                //since they will be automatically off for anyone who updates the app 
                Settings.NotificationFlags |= NotificationFlag.Status | NotificationFlag.Push;
                Settings.SaveAllSettings(context);
                //
                editor.PutBoolean(_changesAreComingReminder, false);
                editor.Commit();
                context.StartActivity(new Intent(context, typeof(ChangesActivity)));
            });
            dialog.Show();
        }
    }
}
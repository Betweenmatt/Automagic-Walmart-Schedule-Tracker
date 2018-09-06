using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ColorPicker;

namespace WalmartAutoScheduleAndroid.Activities
{
    class SettingsFragment : PreferenceFragment
    {
        EditTextPreference un;
        EditTextPreference pw;
        EditTextPreference title;
        EditTextPreference calendarid;
        ListPreference calendarList;
        ColorPickerPreference eventcolor;
        ColorPickerPreference updateeventcolor;
        TimePreference timepref;
        SwitchPreference addNotification;
        SwitchPreference deleteNotification;
        SwitchPreference updateNotification;
        SwitchPreference errorNotification;
        ListPreference reminderList;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Add  preferences.
            AddPreferencesFromResource(Resource.Xml.settings);

            Settings.CalendarObjects = new CalManager(Utilities.CheckCalendarPermissions(this.Context)).GetCalendars(this.Context);

            un = (EditTextPreference)FindPreference("username");
            pw = (EditTextPreference)FindPreference("password");
            title = (EditTextPreference)FindPreference("title");
            calendarid = (EditTextPreference)FindPreference("calendarid");
            calendarList = (ListPreference)FindPreference("calendar");
            eventcolor = (ColorPickerPreference)FindPreference("eventcolor");
            timepref = (TimePreference)FindPreference("timesetting");
            updateeventcolor = (ColorPickerPreference)FindPreference("updateeventcolor");
            addNotification = (SwitchPreference)FindPreference("addshiftnotification");
            updateNotification = (SwitchPreference)FindPreference("updateshiftnotification");
            deleteNotification = (SwitchPreference)FindPreference("deleteshiftnotification");
            errorNotification = (SwitchPreference)FindPreference("errornotification");
            reminderList = (ListPreference)FindPreference("reminder");
            Preference deleteAllEvents = (Preference)FindPreference("deleteAllEvents");
            Preference support = FindPreference("support");
            
            un.Text = Settings.UserName;
            pw.Text = Settings.Password;
            title.Text = Settings.EventTitle;

            if (Settings.NotificationFlags.HasFlag(NotificationFlag.AddShift))
                addNotification.Checked = true;
            else
                addNotification.Checked = false;
            if (Settings.NotificationFlags.HasFlag(NotificationFlag.UpdateShift))
                updateNotification.Checked = true;
            else
                updateNotification.Checked = false;
            if (Settings.NotificationFlags.HasFlag(NotificationFlag.DeleteShift))
                deleteNotification.Checked = true;
            else
                deleteNotification.Checked = false;
            if (Settings.NotificationFlags.HasFlag(NotificationFlag.Error))
                errorNotification.Checked = true;
            else
                errorNotification.Checked = false;

            var entries = GetEntries();
            calendarList.SetEntries(entries[0]);
            calendarList.SetEntryValues(entries[1]);
            var calendarIndex = calendarList.FindIndexOfValue(Settings.CalendarId.ToString());
            calendarList.SetValueIndex(calendarIndex);

            reminderList.SetEntries(new string[] 
            {
                "None", "15 Minutes", "30 Minutes", "60 Minutes", "90 Minutes", "2 Hours", "6 Hours"
            });
            reminderList.SetEntryValues(new string[]
            {
                "0", "15", "30", "60", "90", "120", "360"
            });
            var reminderIndex = reminderList.FindIndexOfValue(Settings.Reminder);
            reminderList.SetValueIndex(reminderIndex);

            deleteAllEvents.PreferenceClick += (s, e) =>
            {
                AlertDialog.Builder dialog = new AlertDialog.Builder(this.Activity);
                dialog.SetTitle("Confirm");
                dialog.SetMessage("Are you sure you want to do this? Keep in mind, it will only delete the last 40 days or so of generated events and cannot be undone.");
                dialog.SetNegativeButton("No", (ss, ee) =>{});
                dialog.SetPositiveButton("Yes", (ss, ee) =>
                {
                    new CalManager(Utilities.CheckCalendarPermissions(this.Context)).DeleteAllEntries(this.Activity);
                });
                dialog.Show();
            };

            support.PreferenceClick += (s, e) =>
            {
                string addr = "mailto:automagicwalmartschedule@gmail.com";
                Android.Net.Uri uri = Android.Net.Uri.Parse(addr);
                Intent intent = new Intent(Intent.ActionSendto,uri);
                //intent.PutExtra(Intent.ExtraEmail, new string[] { "automagicwalmartschedule@gmail.com" });
                //intent.PutExtra(Intent.ExtraSubject, "Automagic App");

                StartActivity(Intent.CreateChooser(intent, "Send Email"));
            };

            eventcolor.SetIndex(Settings.EventColorId);
            updateeventcolor.SetIndex(Settings.UpdateEventColorId);
            
        }
        private string[][] GetEntries()
        {
            List<string> entries = new List<string>();
            List<string> values = new List<string>();
            foreach(var x in Settings.CalendarObjects)
            {
                entries.Add(x.DisplayName);
                values.Add(x.Id.ToString());
            }
            return new string[][] { entries.ToArray(), values.ToArray() };
        }
        public override void OnDestroy()
        {
            PrefToSettings();
            base.OnDestroy();
        }
        public override void OnPause()
        {
            PrefToSettings();
            base.OnPause();
        }
        private void PrefToSettings()
        {
            Settings.UserName = un.Text;
            Settings.Password = pw.Text;
            Settings.CalendarId = long.Parse(calendarList.Value);
            Settings.EventTitle = title.Text;
            Settings.UpdateEventColorId = updateeventcolor.GetIndex();
            Settings.EventColorId = eventcolor.GetIndex();
            Settings.SaveAllSettings(this.Activity);
            Settings.Reminder = reminderList.Value;

            //check if calendar chosen is google.
            //This is needed because the event colors only work with google, so a check is needed when creating events.
            var calobj = Settings.CalendarObjects.FirstOrDefault(f => f.Id == Settings.CalendarId);
            Settings.IsCalendarGoogle = calobj.Type == "com.google" ? true : false;

            Settings.NotificationFlags = NotificationFlag.None;
            if (addNotification.Checked)
                Settings.NotificationFlags |= NotificationFlag.AddShift;
            if (updateNotification.Checked)
                Settings.NotificationFlags |= NotificationFlag.UpdateShift;
            if (deleteNotification.Checked)
                Settings.NotificationFlags |= NotificationFlag.DeleteShift;
            if (errorNotification.Checked)
                Settings.NotificationFlags |= NotificationFlag.Error;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text.Method;
using Android.Text.Util;
using Android.Views;
using Android.Widget;
using ColorPicker;
using WalmartAutoScheduleAndroid.Scraper;

namespace WalmartAutoScheduleAndroid.Activities
{
    class SettingsFragment : PreferenceFragment
    {
        //EditTextPreference un;
        //EditTextPreference pw;
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
        SwitchPreference showDaysOffSwitch;
        SwitchPreference statusNotification;
        SwitchPreference pushNotification;
        ColorPickerPreference dayOffColor;
        ListPreference reminderList;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Add  preferences.
            AddPreferencesFromResource(Resource.Xml.settings);

            Settings.CalendarObjects = new CalManager(Utilities.CheckCalendarPermissions(this.Context)).GetCalendars(this.Context);

            //un = (EditTextPreference)FindPreference("username");
            //pw = (EditTextPreference)FindPreference("password");
            title = (EditTextPreference)FindPreference("title");
            calendarid = (EditTextPreference)FindPreference("calendarid");
            calendarList = (ListPreference)FindPreference("calendar");
            eventcolor = (ColorPickerPreference)FindPreference("eventcolor");
            timepref = (TimePreference)FindPreference("timesetting");
            updateeventcolor = (ColorPickerPreference)FindPreference("updateeventcolor");
            dayOffColor = (ColorPickerPreference)FindPreference("dayOffColor");
            addNotification = (SwitchPreference)FindPreference("addshiftnotification");
            updateNotification = (SwitchPreference)FindPreference("updateshiftnotification");
            deleteNotification = (SwitchPreference)FindPreference("deleteshiftnotification");
            statusNotification = (SwitchPreference)FindPreference("statusnotification");
            pushNotification = (SwitchPreference)FindPreference("pushnotification");
            errorNotification = (SwitchPreference)FindPreference("errornotification");
            reminderList = (ListPreference)FindPreference("reminder");
            showDaysOffSwitch = (SwitchPreference)FindPreference("showDaysOff");
            Preference deleteAllEvents = FindPreference("deleteAllEvents");
            Preference support = FindPreference("support");
            Preference loginButton = FindPreference("loginButton");
            Preference about = FindPreference("about");
            about.Summary = $"Version:{Settings.Version}\nCreated by Matthew Andrews";

            
            about.OnPreferenceClickListener = new FiveTapListener(()=>
            {
                AlertDialog.Builder dialog = new AlertDialog.Builder(this.Activity);
                dialog.SetTitle("Confirm");
                dialog.SetCancelable(false);
                dialog.SetMessage("Are you sure you want to do this? This action will delete all the events in your calendar that have the same name as the name you have set in settings. This may cause unwanted behavior, and may take multiple tries to get rid of all the events. Only use this as a last result! The app will hang while the operation happens.");
                dialog.SetNegativeButton("No", (ss, ee) => { });
                dialog.SetPositiveButton("Yes", (ss, ee) =>
                {
                    new CalManager(Utilities.CheckCalendarPermissions(this.Context)).DeleteAllEntriesWithName(this.Activity, new SettingsObject());
                });
                dialog.Show();
            });
            loginButton.PreferenceClick += (s, e) =>
            {
                var dialog = new Dialog(this.Activity);
                dialog.SetContentView(Resource.Layout.popup_login_dialog);
                dialog.SetCancelable(true);
                var un = dialog.FindViewById<EditText>(Resource.Id.etEmail);
                var pw = dialog.FindViewById<EditText>(Resource.Id.etPassword);
                var login = dialog.FindViewById<Button>(Resource.Id.btnLogin);
                var help = dialog.FindViewById<TextView>(Resource.Id.helpLink);
                help.MovementMethod = LinkMovementMethod.Instance;
                //Linkify.AddLinks(help, MatchOptions.All);
                un.Text = Settings.UserName;
                pw.Text = Settings.Password;
                dialog.Show();
                login.Click += (ss, ee) =>
                {
                    login.Enabled = false;
                    Settings.UserName = un.Text;
                    Settings.Password = pw.Text;
                    var prog = new ProgressDialog(this.Activity);
                    prog.SetTitle("Logging in");
                    prog.SetMessage("Please wait...");
                    prog.Indeterminate = true;
                    prog.SetCancelable(false);
                    prog.Show();
                    new Thread(() =>
                    {
                        var ret = new SiteScraper(new SettingsObject()).LoginCheck();
                        prog.Dismiss();
                        this.Activity.RunOnUiThread(() =>
                        {
                            if (ret.Status == SiteScraperReturnStatus.Error)
                            {
                                Settings.WalmartOneStatus = WalmartOneStatus.Offline;
                                AlertDialog.Builder alert = new AlertDialog.Builder(this.Activity);
                                alert.SetTitle("Error!");
                                alert.SetMessage("There was an error connecting to WalmartOne. The WalmartOne service is most likely down. Please try logging in at a later time!");
                                alert.SetCancelable(false);
                                alert.SetPositiveButton("Ok", (sss, eee) =>
                                {
                                    dialog.Dismiss();
                                });
                                alert.Show();
                            }
                            else if (ret.Status == SiteScraperReturnStatus.WrongLogin)
                            {
                                Settings.WalmartOneStatus = WalmartOneStatus.LoginInfoWrong;
                                AlertDialog.Builder alert = new AlertDialog.Builder(this.Activity);
                                alert.SetTitle("Wrong Login!");
                                alert.SetCancelable(false);
                                alert.SetMessage("The login info you provided is not correct! Please double check your information and try again.");
                                alert.SetPositiveButton("Ok", (sss, eee) => { });
                                alert.Show();
                                Settings.UserName = "";
                                Settings.Password = "";
                                un.Text = "";
                                pw.Text = "";
                            }
                            else
                            {
                                Settings.WalmartOneStatus = WalmartOneStatus.Online;
                                AlertDialog.Builder alert = new AlertDialog.Builder(this.Activity);
                                alert.SetTitle("Success!");
                                alert.SetCancelable(false);
                                alert.SetMessage("Success! The login info you provided is correct. You may proceed!");
                                alert.SetPositiveButton("Ok", (sss, eee) =>
                                {
                                    dialog.Dismiss();
                                });
                                alert.Show();
                            }
                            login.Enabled = true;

                        });
                    }).Start();
                };
            };
            
            //un.Text = Settings.UserName;
            //pw.Text = Settings.Password;
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
            if (Settings.NotificationFlags.HasFlag(NotificationFlag.Status))
                statusNotification.Checked = true;
            else
                statusNotification.Checked = false;
            if (Settings.NotificationFlags.HasFlag(NotificationFlag.Push))
                pushNotification.Checked = true;
            else
                pushNotification.Checked = false;
            if (Settings.ShowDaysOff)
                showDaysOffSwitch.Checked = true;
            else
                showDaysOffSwitch.Checked = false;

            var entries = GetEntries();
            calendarList.SetEntries(entries[0]);
            calendarList.SetEntryValues(entries[1]);
            if(entries[0].Length < 1)
            {
                Toast.MakeText(this.Context, "This device does not have any calendars! Please create a calendar with your calendar app first.", ToastLength.Long).Show();
                return;
            }
            var calendarIndex = calendarList.FindIndexOfValue(Settings.CalendarId.ToString());
            //this should fix issues with the saved calendar id not existing
            if (calendarIndex == -1)
            {
                int.TryParse(entries[1]?[0], out int result);
                Settings.CalendarId = result;
                calendarIndex = calendarList.FindIndexOfValue(Settings.CalendarId.ToString());
            }
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
            if(reminderIndex == -1)
            {
                Settings.Reminder = "0";
                reminderIndex = reminderList.FindIndexOfValue(Settings.Reminder);
            }
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
            try
            {
                eventcolor.SetIndex(Settings.EventColorId);
                updateeventcolor.SetIndex(Settings.UpdateEventColorId);
                dayOffColor.SetIndex(Settings.DayOffColorId);
            }
            catch
            {
                Toast.MakeText(this.Context, "There was an issue getting the colors. If this happens continuously, please email support!", ToastLength.Long).Show();
            }
            
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
            //PrefToSettings();
            base.OnDestroy();
        }
        public override void OnPause()
        {
           PrefToSettings();
            base.OnPause();
        }
        private void PrefToSettings()
        {
            //Settings.UserName = un.Text;
            //Settings.Password = pw.Text;
            try
            {

                if (long.TryParse(calendarList.Value, out long res))
                    Settings.CalendarId = res;
                else
                    Settings.CalendarId = Settings.Consts.CalendarIdDef;

                Settings.EventTitle = title.Text;
                Settings.UpdateEventColorId = updateeventcolor.GetIndex();
                Settings.DayOffColorId = dayOffColor.GetIndex();
                Settings.EventColorId = eventcolor.GetIndex();
                Settings.Reminder = reminderList.Value;
                Settings.ShowDaysOff = showDaysOffSwitch.Checked;

                //check if calendar chosen is google.
                //This is needed because the event colors only work with google, so a check is needed when creating events.
                var calobj = Settings.CalendarObjects.FirstOrDefault(f => f.Id == Settings.CalendarId);
                Settings.IsCalendarGoogle = calobj?.Type == "com.google" ? true : false;

                Settings.NotificationFlags = NotificationFlag.None;
                if (addNotification.Checked)
                    Settings.NotificationFlags |= NotificationFlag.AddShift;
                if (updateNotification.Checked)
                    Settings.NotificationFlags |= NotificationFlag.UpdateShift;
                if (deleteNotification.Checked)
                    Settings.NotificationFlags |= NotificationFlag.DeleteShift;
                if (errorNotification.Checked)
                    Settings.NotificationFlags |= NotificationFlag.Error;
                if (pushNotification.Checked)
                    Settings.NotificationFlags |= NotificationFlag.Push;
                if (statusNotification.Checked)
                    Settings.NotificationFlags |= NotificationFlag.Status;


                Settings.SaveAllSettings(this.Activity);
            }catch
            {
                Toast.MakeText(this.Activity, "There was an issue saving your settings. Please try again.", ToastLength.Long);
            }
        }
    }
}
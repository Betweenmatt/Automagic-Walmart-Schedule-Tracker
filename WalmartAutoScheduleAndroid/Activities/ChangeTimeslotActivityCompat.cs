using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Android.Views;
using Android.Widget;
using WalmartAutoScheduleAndroid.Scraper;

namespace WalmartAutoScheduleAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    class ChangeTimeslotActivityCompat : AppCompatActivity
    {
        public static Day WorkingObj { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_updateTimeslotCompat);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    NavUtils.NavigateUpFromSameTask(this);
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
    [Register("com.andrewsstudios.automagicwalmartscheduletracker.walmartAutoScheduleAndroid.ChangeTimeslotFragmentCompat")]
    public class ChangeTimeslotFragmentCompat : PreferenceFragmentCompat
    {
        const string _saveKey = "save";
        const string _startTimeKey = "starttime";
        const string _endTimeKey = "endtime";
        DateTime _start;
        DateTime _end;


        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.updateTimeslotSettingsCompat);
            EditTextPreference shift = (EditTextPreference)FindPreference("shifttype");
            EditTextPreference meal = (EditTextPreference)FindPreference("meal");
            Preference starttime = FindPreference(_startTimeKey);
            Preference endtime = FindPreference(_endTimeKey);
            Preference save = FindPreference(_saveKey);
            Preference delete = FindPreference("delete");

            shift.Summary = ChangeTimeslotActivityCompat.WorkingObj.Shift;
            meal.Summary = ChangeTimeslotActivityCompat.WorkingObj.Meal;
            starttime.Summary = ChangeTimeslotActivityCompat.WorkingObj.Start.ToString();
            endtime.Summary = ChangeTimeslotActivityCompat.WorkingObj.End.ToString();

            _start = ChangeTimeslotActivityCompat.WorkingObj.Start;
            _end = ChangeTimeslotActivityCompat.WorkingObj.End;
            shift.Text = "";
            meal.Text = "";
            

            shift.PreferenceChange += (s, e) =>
            {
                shift.Summary = shift.Text;
            };

            meal.PreferenceChange += (s, e) =>
            {
                meal.Summary = meal.Text;
            };

            starttime.PreferenceClick += (s, e) =>
            {
                Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this.Activity);
                builder.SetTitle("Shift Start Time");
                builder.SetCancelable(true);
                var time = new TimePicker(this.Activity)
                {
                    Minute = _start.Minute,
                    Hour = _start.Hour
                };
                
                var layout = new LinearLayout(this.Activity)
                {
                    Orientation = Orientation.Vertical
                };
                layout.AddView(time);
                builder.SetView(layout);
                builder.SetPositiveButton("Ok", (ss, ee) =>
                {
                    _start = new DateTime(
                        _start.Year,
                        _start.Month,
                        _start.Day,
                        time.Hour,
                        time.Minute,
                        0
                        );
                    starttime.Summary = _start.ToString();
                });
                builder.Show();
            };

            endtime.PreferenceClick += (s, e) =>
            {
                Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this.Activity);
                builder.SetTitle("Shift End Time");
                builder.SetCancelable(true);
                var time = new TimePicker(this.Activity)
                {
                    Minute = _end.Minute,
                    Hour = _end.Hour
                };
                var layout = new LinearLayout(this.Activity)
                {
                    Orientation = Orientation.Vertical
                };
                layout.AddView(time);
                builder.SetView(layout);
                builder.SetPositiveButton("Ok", (ss, ee) =>
                {
                    _end = new DateTime(
                        _end.Year,
                        _end.Month,
                        _end.Day,
                        time.Hour,
                        time.Minute,
                        0
                        );
                    endtime.Summary = _end.ToString();
                });
                builder.Show();
            };

            save.PreferenceClick += (s, e) =>
            {
                if ((_end.TimeOfDay < _start.TimeOfDay))
                {
                    if (_start.Date == _end.Date)
                        _end = _end.AddDays(1);
                }
                else
                {
                    if (_start.Date != _end.Date)
                        _end = _end.AddDays(-1);
                }

                var workingobj = ChangeTimeslotActivityCompat.WorkingObj;
                workingobj.Start = _start;
                workingobj.End = _end;
                workingobj.Shift = shift.Text == "" ? workingobj.Shift : shift.Text;
                workingobj.Meal = meal.Text == "" ? workingobj.Meal : meal.Text;
                new CalManager(Utilities.CheckCalendarPermissions(this.Activity)).ChangeTimeslot(this.Activity, new SettingsObject(), workingobj, Settings.EventColorId);
                this.Activity.Finish();
            };

            delete.PreferenceClick += (s, e) =>
            {
                Android.Support.V7.App.AlertDialog.Builder dialog = new Android.Support.V7.App.AlertDialog.Builder(this.Activity);
                dialog.SetTitle("Confirm");
                dialog.SetMessage("Are you sure you want to do this? This cannot be undone!");
                dialog.SetNegativeButton("No", (ss, ee) => { });
                dialog.SetPositiveButton("Yes", (ss, ee) =>
                {
                    var workingobj = ChangeTimeslotActivityCompat.WorkingObj;
                    workingobj.Ignore = true;
                    new CalManager(Utilities.CheckCalendarPermissions(this.Activity))
                        .DeleteTimeslot(this.Activity, workingobj);
                    this.Activity.Finish();
                });
                dialog.Show();
            };

        }
    }
}
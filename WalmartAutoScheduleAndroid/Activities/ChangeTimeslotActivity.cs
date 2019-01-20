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
using WalmartAutoScheduleAndroid.Scraper;

namespace WalmartAutoScheduleAndroid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    class ChangeTimeslotActivity : PreferenceActivity
    {
        public static Day WorkingObj { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            FragmentManager.BeginTransaction().Replace(Android.Resource.Id.Content,
                    new ChangeTimeSlotFragment()).Commit();
        }
    }
    class ChangeTimeSlotFragment : PreferenceFragment
    {
        const string _saveKey = "save";
        const string _startTimeKey = "starttime";
        const string _endTimeKey = "endtime";
        DateTime _start;
        DateTime _end;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AddPreferencesFromResource(Resource.Xml.updateTimeslotSettings);
            EditTextPreference shift = (EditTextPreference)FindPreference("shifttype");
            EditTextPreference meal = (EditTextPreference)FindPreference("meal");
            Preference starttime = FindPreference(_startTimeKey);
            Preference endtime = FindPreference(_endTimeKey);
            Preference save = FindPreference(_saveKey);
            ColorPickerPreference color = (ColorPickerPreference)FindPreference("eventcolor");
            Preference delete = FindPreference("delete");
            
            shift.Summary = ChangeTimeslotActivity.WorkingObj.Shift;
            meal.Summary = ChangeTimeslotActivity.WorkingObj.Meal;
            starttime.Summary = ChangeTimeslotActivity.WorkingObj.Start.ToString();
            endtime.Summary = ChangeTimeslotActivity.WorkingObj.End.ToString();

            _start = ChangeTimeslotActivity.WorkingObj.Start;
            _end = ChangeTimeslotActivity.WorkingObj.End;
            shift.Text = "";
            meal.Text = "";

            color.SetIndex(Settings.UpdateEventColorId);

            shift.PreferenceChange += (s, e) =>
            {
                shift.Summary = shift.EditText.Text;
            };

            meal.PreferenceChange += (s, e) =>
            {
                meal.Summary = meal.EditText.Text;
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

                var workingobj = ChangeTimeslotActivity.WorkingObj;
                workingobj.Start = _start;
                workingobj.End = _end;
                workingobj.Shift = shift.Text == "" ? workingobj.Shift : shift.Text;
                workingobj.Meal = meal.Text == "" ? workingobj.Meal : meal.Text;
                workingobj.OverrideColor = color.GetIndex();
                workingobj.ManualAdjustment = true;
                new CalManager(Utilities.CheckCalendarPermissions(this.Activity)).ChangeTimeslot(this.Activity, new SettingsObject(), workingobj,color.GetIndex());
                this.Activity.Finish();
            };

            delete.PreferenceClick += (s, e) =>
            {
                AlertDialog.Builder dialog = new AlertDialog.Builder(this.Activity);
                dialog.SetTitle("Confirm");
                dialog.SetMessage("Are you sure you want to do this? This cannot be undone!");
                dialog.SetNegativeButton("No", (ss, ee) => { });
                dialog.SetPositiveButton("Yes", (ss, ee) =>
                {
                    var workingobj = ChangeTimeslotActivity.WorkingObj;
                    workingobj.Ignore = true;
                    new CalManager(Utilities.CheckCalendarPermissions(this.Activity))
                        .DeleteTimeslot(this.Activity,workingobj);
                    this.Activity.Finish();
                });
                dialog.Show();
            };


        }
        
    }
}
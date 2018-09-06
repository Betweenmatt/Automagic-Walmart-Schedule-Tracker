using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WalmartAutoScheduleAndroid
{
    [Service(Name = "walmartAutoScheduleAndroid.walmartAutoScheduleAndroid.walmartAutoScheduleAndroid.ReminderService",
               Permission = "android.permission.BIND_JOB_SERVICE")]
    public class ReminderService : JobService
    {
        public override bool OnStartJob(JobParameters @params)
        {
            new Thread(() =>
            {
                Settings.LoadAllSettings(this.ApplicationContext);
                var settings = new SettingsObject();
                new CalManager(Utilities.CheckCalendarPermissions(this.ApplicationContext)).SetReminders(this.ApplicationContext, settings);
                JobFinished(@params, false);
            }).Start();
            return true;
        }

        public override bool OnStopJob(JobParameters @params)
        {
            return true;
        }
    }
}
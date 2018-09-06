using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WalmartAutoScheduleAndroid.Scraper;

namespace WalmartAutoScheduleAndroid
{
    [Service(Name = "walmartAutoScheduleAndroid.walmartAutoScheduleAndroid.walmartAutoScheduleAndroid.EventService",
            Permission = "android.permission.BIND_JOB_SERVICE")]
    public class EventService : JobService
    {
        //
        const int _periodic = (60 * 1000) * 60 * 3;
        //const int _periodic = (60 * 1000) * 15;
        const int _reminderPeriodic = _periodic + (60 * 1000) * 5;
        const int _jobId = 3215974;
        const int _reminderJobId = _jobId + 1;
        private static Context _context;
        public static void StartService(Context context)
        {
            StopService(context);
            _context = context;
            JobInfo jobInfo = new JobInfo.Builder(_jobId, new Android.Content.ComponentName(context, Java.Lang.Class.FromType(typeof(EventService))))
                .SetPeriodic(_periodic).SetPersisted(true).Build();
            
            JobInfo reminderJobInfo = new JobInfo.Builder(_reminderJobId, new Android.Content.ComponentName(context,
                Java.Lang.Class.FromType(typeof(ReminderService))))
                .SetPeriodic(_reminderPeriodic).SetPersisted(true).Build();

            JobScheduler jobScheduler = (JobScheduler)context.GetSystemService(Android.Content.Context.JobSchedulerService);
            jobScheduler.Schedule(jobInfo);
            jobScheduler.Schedule(reminderJobInfo);
        }
        public static void StopService(Context context)
        {
            JobScheduler scheduler = (JobScheduler)context.GetSystemService(Android.Content.Context.JobSchedulerService);
            foreach(JobInfo job in scheduler.AllPendingJobs)
            {
                if (job.Id == _jobId)
                    scheduler.Cancel(_jobId);
                if (job.Id == _reminderJobId)
                    scheduler.Cancel(_reminderJobId);
            }
        }
        /// <summary>
        /// Check if the service is already running
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsServiceRunning(Context context)
        {
            JobScheduler scheduler = (JobScheduler)context.GetSystemService(Android.Content.Context.JobSchedulerService);
            foreach(JobInfo job in scheduler.AllPendingJobs)
            {
                if (job.Id == _jobId)
                    return true;
            }
            return false;
        }
        public override bool OnStartJob(JobParameters @params)
        {

            new Thread(() =>
            {
                Settings.LoadAllSettings(this.ApplicationContext);
                var settings = new SettingsObject();
                var sitedata = new SiteScraper(settings).Execute();
                if (sitedata.Status == SiteScraperReturnStatus.Success)
                {
                    Console.WriteLine("WMS:CalManager Called");
                    new CalManager(Utilities.CheckCalendarPermissions(this.ApplicationContext)).Execute(this, settings, sitedata.Data);
                }
                if (sitedata.Status == SiteScraperReturnStatus.Success)
                {
                    Settings.WalmartOneStatus = WalmartOneStatus.Online;
                    Settings.SaveAllSettings(this);
                    Console.WriteLine("WMS:Job Success");
                }
                else if (sitedata.Status == SiteScraperReturnStatus.WrongLogin)
                {
                    Console.WriteLine("WMS:Login Failed");
                }
                else if (sitedata.Status == SiteScraperReturnStatus.Error)
                {
                    Settings.WalmartOneStatus = WalmartOneStatus.Offline;
                    Settings.SaveAllSettings(this);
                    Console.WriteLine("WMS:WalmartOne Error Status");
                }
                Console.WriteLine("WMS:JOB DONE");
                //send a notification to main activity if it exists. This feels cheap and sloppy, but since
                //its only needed for a one-off on first install and then shouldn't be used again I think it 
                //may be worth cutting the corner.
                if (_context != null)
                {
                    if(_context is MainActivity m)
                    {
                        m.RunOnUiThread(() =>
                        {
                            m.RefreshListAdapter();
                        });
                    }
                }
                _context = null;

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace WalmartAutoScheduleAndroid
{
    [Service(Name = "com.andrewsstudios.automagicwalmartscheduletracker.walmartAutoScheduleAndroid.ServerCheckService",
               Permission = "android.permission.BIND_JOB_SERVICE")]
    class ServerCheckService : JobService
    {
        const int _periodic = (60 * 1000) * 60 * 1;
        //const int _periodic = (60 * 1000) * 1;
        const int _jobId = 481516;
        public static void StartService(Context context)
        {
            StopService(context);
            JobInfo jobInfo = new JobInfo.Builder(_jobId, new Android.Content.ComponentName(context, Java.Lang.Class.FromType(typeof(ServerCheckService))))
                .SetPeriodic(_periodic).SetPersisted(true).Build();
            

            JobScheduler jobScheduler = (JobScheduler)context.GetSystemService(Android.Content.Context.JobSchedulerService);
            jobScheduler.Schedule(jobInfo);
        }
        public static void StopService(Context context)
        {
            JobScheduler scheduler = (JobScheduler)context.GetSystemService(Android.Content.Context.JobSchedulerService);
            foreach (JobInfo job in scheduler.AllPendingJobs)
            {
                if (job.Id == _jobId)
                    scheduler.Cancel(_jobId);
            }
        }
        public override bool OnStartJob(JobParameters @params)
        {
            Console.WriteLine("WMAST: Starting Server Check");
            new Thread(() =>
            {
                Settings.LoadAllSettings(this.ApplicationContext);
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        if(Settings.NotificationFlags.HasFlag(NotificationFlag.Status))
                            StatusCheck(client);
                        if(Settings.NotificationFlags.HasFlag(NotificationFlag.Push))
                            PushNotificationCheck(client);
                    }
                }catch(Exception e)
                {
                    Console.WriteLine("catch: " + e);
                }
                Console.WriteLine("WMAST:Server check finished");
                JobFinished(@params, false);
            }).Start();
            return true;
        }
        private void PushNotificationCheck(HttpClient client)
        {
            var vals = new Dictionary<string, string>
                    {
                        {"version", Settings.VersionCode.ToString() }
                    };

            var content = new FormUrlEncodedContent(vals);
            var response = client.PostAsync("http://www.andrewsstudios.us/api/wmast/pushNotifications.php", content);
            var responseString = response.Result.Content.ReadAsStringAsync();
            if (responseString.Result == "" || responseString.Result == "500")
                return;
            var obj = JsonConvert.DeserializeObject<PushNotificationResult>(responseString.Result);
            Console.WriteLine(obj.Id);
            if (obj != null && !Settings.PushNotificationIds.Contains(obj.Id))
            {
                new NotificationFactory(this.ApplicationContext, obj.Title, obj.Content, NotificationFlag.None, PendingIntentType.MainActivity);
                Settings.PushNotificationIds.Add(obj.Id);
                
                Settings.SaveAllSettings(this.ApplicationContext);
            }
        }
        private void StatusCheck(HttpClient client)
        {
            var vals = new Dictionary<string, string>
                    {
                        {"version", Settings.VersionCode.ToString() }
                    };

            var content = new FormUrlEncodedContent(vals);
            var response = client.PostAsync("http://www.andrewsstudios.us/api/wmast/status.php", content);
            var responseString = response.Result.Content.ReadAsStringAsync();
            int.TryParse(responseString.Result, out int output);
            if (output == 0)
                output = 500;
            var serverStatus = (ServerStatus)output;
            Console.WriteLine("Status = " + responseString.Result);
            if (Settings.ServerStatus != serverStatus)
            {
                if (serverStatus != ServerStatus.Error)
                    Settings.ServerStatus = serverStatus;
                switch (serverStatus)
                {
                    case (ServerStatus.Down):
                        new NotificationFactory(this.ApplicationContext, "Tracker is Down!", "The connection to Walmarts servers is down. Please don't rely on this app until you're notified again!", NotificationFlag.None, PendingIntentType.MainActivity);
                        Settings.WalmartOneStatus = WalmartOneStatus.Offline;
                        break;
                    case (ServerStatus.Error):
                        Settings.WalmartOneStatus = WalmartOneStatus.Unknown;
                        break;
                    case (ServerStatus.Ok):
                        Settings.WalmartOneStatus = WalmartOneStatus.Online;
                        new NotificationFactory(this.ApplicationContext, "Tracker is Up!", "The connection to Walmarts servers is back up!", NotificationFlag.None, PendingIntentType.MainActivity);
                        break;

                    case (ServerStatus.Update):
                        Settings.WalmartOneStatus = WalmartOneStatus.Offline;
                        new NotificationFactory(this.ApplicationContext, "App needs to be updated!", "This app needs to be updated! Please click on this notification to go to the google play store", NotificationFlag.None, PendingIntentType.MainActivity);
                        break;
                }
                Settings.SaveAllSettings(this.ApplicationContext);
            }
        }
        public override bool OnStopJob(JobParameters @params)
        {
            return true;
        }
    }
}
using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.App.Job;
using WalmartAutoScheduleAndroid.Scraper;
using System.Threading.Tasks;
using WalmartAutoScheduleAndroid.Activities;
using Android.Support.V4.Content;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using WalmartAutoScheduleAndroid.EventRecycler;

namespace WalmartAutoScheduleAndroid
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
        private EventAdapter _adapter;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            
            Settings.LoadAllSettings(this);

            if (!Settings.IntroComplete)
            {
                StartActivity(new Android.Content.Intent(this, typeof(IntroActivity)));
            }
            else if(Settings.UserName == "")
            {
                Toast.MakeText(this, "It appears your settings aren't set, lets go to the settings menu.", ToastLength.Long).Show();
                if(!Utilities.CheckCalendarPermissions(this))
                    RequestPermissions(new string[] { Android.Manifest.Permission.ReadCalendar, Android.Manifest.Permission.WriteCalendar }, 0);
                StartActivity(new Android.Content.Intent(this, typeof(SettingsActivity)));
            }

            RequestPermissions(new string[] { Android.Manifest.Permission.ReadCalendar, Android.Manifest.Permission.WriteCalendar }, 0);



            Switch schedule = FindViewById<Switch>(Resource.Id.scheduleSwitch);
            schedule.Checked = EventService.IsServiceRunning(this);

            schedule.CheckedChange += (s, e) =>
            {
                if (e.IsChecked)
                {
                    EventService.StartService(this);
                    Toast.MakeText(this, "The automagic service has started.\nPlease wait a few minutes for it to complete!", ToastLength.Long).Show();
                }
                else
                {
                    EventService.StopService(this);
                    Toast.MakeText(this, "The automagic service has been stopped.", ToastLength.Long).Show();
                }
            };
            //create notification channel on 8.0 and above
            if(Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                string name = "Automagic Walmart Schedule Tracker Notification Channel";
                string description = "Notification channel for Automagic Walmart Schedule Tracker";
                NotificationChannel channel = new NotificationChannel(NotificationFactory.Channel, name, NotificationImportance.Default);
                channel.Description = description;

                NotificationManager notifManager = (NotificationManager)GetSystemService(Java.Lang.Class.FromType(typeof(NotificationManager)));
                notifManager.CreateNotificationChannel(channel);
            }
            
            var recycler = FindViewById<Android.Support.V7.Widget.RecyclerView>(Resource.Id.agendaRecycler);
            recycler.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));
            _adapter = new EventAdapter(this);
            recycler.SetAdapter(_adapter);
            SetStatus();
            
		}
        protected override void OnResume()
        {
            base.OnResume();
            RefreshListAdapter();
        }
        public void RefreshListAdapter()
        {
            if (_adapter != null)
            {
                _adapter.NotifyChange(this);
            }
        }
        public void SetStatus()
        {
            var v = FindViewById<TextView>(Resource.Id.wmstatus);
            switch (Settings.WalmartOneStatus)
            {
                case (WalmartOneStatus.Unknown):
                    v.Text = "WalmartOne Status is Unknown.";
                    break;
                case (WalmartOneStatus.Online):
                    v.Text = "WalmartOne is Online.";
                    break;
                case (WalmartOneStatus.Offline):
                    v.Text = "WalmartOne is Offline.";
                    break;
                case (WalmartOneStatus.LoginInfoWrong):
                    v.Text = "Your Login Info is Wrong!";
                    break;
            }
        }
		public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                StartActivity(new Android.Content.Intent(this, typeof(Activities.SettingsActivity)));
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }
        
	}
}


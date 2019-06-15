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
using Android.Text;
using Android.Views;
using Android.Widget;
using VipAccess;

namespace WalmartAutoScheduleAndroid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    class ChangesActivity : AppCompatActivity
    {
        private EditText _secret;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.changes_activity);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            if(Settings.OtpKey == "")
            {
                var rq = new GenerateRequest();
                Settings.OtpKey = rq.GetKey();
                Settings.OtpSecret = rq.GetSecret();
                Settings.SaveAllSettings(this);
            }

            
            _secret = FindViewById<EditText>(Resource.Id.secret);
            TextView output = FindViewById<TextView>(Resource.Id.otp);
            _secret.Text = Settings.OtpKey;
            AppRateReminder.SetChangesAreComingTriggered(this);
            var generate = FindViewById<Button>(Resource.Id.generateButton);
            generate.Click += (s, e) => 
            {
                var goog = new GoogleAuthenticator();
                var pin = goog.GeneratePin(Base32Encoding.ToBytes(Settings.OtpSecret));
                output.Text = pin;
            };
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }
        protected override void OnPause()
        {
            Settings.SecretKey = _secret.Text;
            Settings.SaveAllSettings(this);
            base.OnPause();
        }
    }
}
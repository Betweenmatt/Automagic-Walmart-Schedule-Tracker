using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AppIntro;
using WalmartAutoScheduleAndroid.Fragments;

namespace WalmartAutoScheduleAndroid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class IntroActivity : AppIntro.AppIntro
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Settings.LoadAllSettings(this);
            skipButtonEnabled = false;
            AddSlide(AppIntroFragment.NewInstance("Welcome",
                "Welcome to the Automagic Walmart Schedule Tracker!",
                Resource.Mipmap.Walmart_Spark_white, Color.CornflowerBlue));
            AddSlide(AppIntroFragment.NewInstance("Save time checking your schedule",
                "By using some fancy computer voodoo, this app automagically pulls your schedule from WalmartOne and inputs all your shifts into a calendar you select.",
                Resource.Mipmap.gears, Color.LightCoral));
            AddSlide(AppIntroFragment.NewInstance("Customize your experience",
                "There are a few neat settings that you can choose from as well, such as the color of the event, the color of updated events, and notifications when shifts change!",
                Resource.Mipmap.customization_img, Color.PaleVioletRed));
            AddSlide(AppIntroFragment.NewInstance("Some stuff to keep in mind",
                "Just a heads up, this app is written and maintained by a mere Walmart associate and is not endorsed by Walmart at all. Its functionality may change or fail at any time due to Walmarts changes to their system. I will try my best to keep it alive!",
                Resource.Mipmap.reminder, Color.MediumTurquoise));
            AddSlide(AppIntroFragment.NewInstance("Let's get started",
                "Once you click done, you'll be brought to the settings screen where you will enter your WalmartOne information, as well as where you can customize your events. Once you're done, press the back button to be brought to the main screen. At the main screen you can enable or disable the automagic system!",
                Resource.Mipmap.getstarted, Color.LightSeaGreen));
           
            SetFadeAnimation();
        }

        public override void OnDonePressed()
        {
            base.OnDonePressed();
            Settings.IntroComplete = true;
            Settings.SaveAllSettings(this);
            StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();
        }
    }
}
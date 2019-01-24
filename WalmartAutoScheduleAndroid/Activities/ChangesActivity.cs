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

namespace WalmartAutoScheduleAndroid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    class ChangesActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.changes_activity);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SetTextContent();
            AppRateReminder.SetChangesAreComingTriggered(this);
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
        private void SetTextContent()
        {
            var textView = FindViewById<TextView>(Resource.Id.changesText);

            string text = @"<h1>Welcome!</h1>
1/24/2019
<br />Due to the changes Walmart is rolling out soon, my wonderful app that you all have come to rely on will stop functioning.
<br /><br />I want to assure you that this will only be temporary! I will need a significant amount of time to implement the changes needed for this app to continue. The current plan is about a week of downtime, but could possibly be longer depending on the exact nature of Walmart's changes. In addition to the 2 factor authentication that they're introducing, they are also combining WalmartOne with the Wire. This kind of change could possibly break every aspect of schedule tracking I use, and I won't know until it is actually released.
<br /><br />The most important aspect of this message is to warn you that as of <font color='red'><b>February 1st 2019</b></font> your schedule will no longer update with my app. Once I have made the changes needed, you will have to update my app through the Google Play Store for things to return to normal. Like I have said, this will take a significant amount of time - so please have some patience! I am literally doing everything in my power to ensure the smoothest transition possible.
<br /><br />In this current version that you just updated to, I added some features that will be able to keep you notified of any changes or interruptions in service. These notifications of status and push notifications can be turned off in the settings screen. I strongly recommend you keep them turned on! They will be letting you know when the service is inturrupted. 

<br /><br />I understand that this is extremely inconvinient; I also use this app on a daily basis and have come to rely on it. I apologize that I can not remedy the situation any faster than stated.
<br /><br />If you have any questions or concerns, please feel free to email me through the support button in the settings screen.
<br /><br />Thank you all for your support over the past few months. You have made all the work I've put into this app 100% worth it!
<br /><br />Matthew";
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                textView.SetText(Html.FromHtml(text,
                    FromHtmlOptions.SeparatorLineBreakBlockquote |
                    FromHtmlOptions.SeparatorLineBreakDiv |
                    FromHtmlOptions.SeparatorLineBreakHeading |
                    FromHtmlOptions.SeparatorLineBreakList |
                    FromHtmlOptions.SeparatorLineBreakListItem |
                    FromHtmlOptions.SeparatorLineBreakParagraph), TextView.BufferType.Spannable);
            else
                textView.SetText(Html.FromHtml(text),TextView.BufferType.Spannable);
            
                
        }
    }
}
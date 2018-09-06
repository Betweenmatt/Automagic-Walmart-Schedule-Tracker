using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace ColorPicker
{
    class SettingsPickerActivity : PreferenceActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // In the simplified UI, fragments are not used at all and we instead
            // use the older PreferenceActivity APIs.

            // Add 'general' preferences.
            //addPreferencesFromResource(R.xml.pref_calendarcolor);
            FragmentManager.BeginTransaction().Replace(Android.Resource.Id.Content,
                    new SettingsPickerFragment()).Commit();
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
}
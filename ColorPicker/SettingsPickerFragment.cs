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

namespace ColorPicker
{
    class SettingsPickerFragment : PreferenceFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Add  preferences.
            AddPreferencesFromResource(Resource.Xml.pref_calendarcolor);
        }
    }
}
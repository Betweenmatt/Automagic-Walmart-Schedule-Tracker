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

namespace WalmartAutoScheduleAndroid
{
    class FiveTapListener : Java.Lang.Object, Preference.IOnPreferenceClickListener//View.IOnTouchListener
    {
        private int _numberOfTaps = 0;
        private DateTime _lastTapTimeMs;
        private Action _action;

        public FiveTapListener(Action action)
        {
            _action = action;
            _lastTapTimeMs = DateTime.Now;
        }

        public bool OnPreferenceClick(Preference preference)
        {
            DateTime now = DateTime.Now;
            if (_numberOfTaps >= 5)
            {
                _action.Invoke();
                _numberOfTaps = 0;
                _lastTapTimeMs = now;
            }
            if((now - _lastTapTimeMs).TotalMilliseconds < 5000)
            {
                _lastTapTimeMs = now;
                _numberOfTaps++;
            }
            else
            {
                _lastTapTimeMs = now;
                _numberOfTaps = 0;
            }
            Console.WriteLine(_numberOfTaps);
            Console.WriteLine(_lastTapTimeMs);
            return true;
        }

        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}
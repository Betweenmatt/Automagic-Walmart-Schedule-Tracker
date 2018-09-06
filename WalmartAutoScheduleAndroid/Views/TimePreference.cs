using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace WalmartAutoScheduleAndroid
{
    [Register("walmartAutoScheduleAndroid.walmartAutoScheduleAndroid.walmartAutoScheduleAndroid.TimePreference")]
    class TimePreference : DialogPreference
    {
        private int lastHour = 0;
        private int lastMinute = 0;
        private TimePicker picker = null;

        public static int GetHour(String time)
        {
            String[] pieces = time.Split(':');

            return (int.Parse(pieces[0]));
        }

        public static int GetMinute(String time)
        {
            String[] pieces = time.Split(':');

            return (int.Parse(pieces[1]));
        }

        public TimePreference(Context ctxt, IAttributeSet attrs)
           : base(ctxt, attrs)
        {
        }

        protected override View OnCreateDialogView()
        {
            picker = new TimePicker(Context);
            //picker.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            return (picker);
        }

        protected override void OnBindDialogView(View v)
        {
            base.OnBindDialogView(v);

            picker.Hour = (lastHour);
            picker.Minute = (lastMinute);
        }

        protected override void OnDialogClosed(bool positiveResult)
        {
            base.OnDialogClosed(positiveResult);

            if (positiveResult)
            {
                lastHour = picker.Hour;
                lastMinute = picker.Minute;

                string time = lastHour.ToString() + ":" + lastMinute.ToString();

                if (CallChangeListener(time))
                {
                    PersistString(time);
                }
            }
        }

        protected override Java.Lang.Object OnGetDefaultValue(TypedArray a, int index)
        {
            return (a.GetString(index));
        }

        protected void OnSetInitialValue(bool restoreValue, Object defaultValue)
        {
            String time = null;

            if (restoreValue)
            {
                if (defaultValue == null)
                {
                    time = GetPersistedString("00:00");
                }
                else
                {
                    time = GetPersistedString(defaultValue.ToString());
                }
            }
            else
            {
                time = defaultValue.ToString();
            }

            lastHour = GetHour(time);
            lastMinute = GetMinute(time);
        }
    }
}
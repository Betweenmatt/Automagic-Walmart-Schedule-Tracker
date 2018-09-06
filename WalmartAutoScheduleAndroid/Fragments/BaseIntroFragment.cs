using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace WalmartAutoScheduleAndroid.Fragments
{
    class BaseIntroFragment : Android.Support.V4.App.Fragment
    {
        private static string ARG_LAYOUT_RES_ID = "layoutResId";
        private int layoutResId;

        public static BaseIntroFragment NewInstance(int layoutResId)
        {
            BaseIntroFragment sampleSlide = new BaseIntroFragment();

            Bundle args = new Bundle();
            args.PutInt(ARG_LAYOUT_RES_ID, layoutResId);
            sampleSlide.Arguments = (args);

            return sampleSlide;
        }


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Arguments != null && Arguments.ContainsKey(ARG_LAYOUT_RES_ID))
            {
                layoutResId = Arguments.GetInt(ARG_LAYOUT_RES_ID);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
                                 Bundle savedInstanceState)
        {
            return inflater.Inflate(layoutResId, container, false);
        }
    }
    class IntroFragment1 : BaseIntroFragment
    {
        public EditText Username { get; private set; }
        public EditText Password { get; private set; }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view =  base.OnCreateView(inflater, container, savedInstanceState);
            //Init(view);
            return view;
        }
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            Username = View.FindViewById<EditText>(Resource.Id.username);
            Password = View.FindViewById<EditText>(Resource.Id.password);
            Username.AddTextChangedListener(new UsernameListener());
            Password.AddTextChangedListener(new PasswordListener());
            var button = View.FindViewById<Button>(Resource.Id.testbutton);
            button.Click += (s, e) =>
            {
                Console.WriteLine(Username.Text);
            };
        }
        private void Init(View v)
        {
            Username = v.FindViewById<EditText>(Resource.Id.username);
            Password = v.FindViewById<EditText>(Resource.Id.password);
            Username.AddTextChangedListener(new UsernameListener());
            Password.AddTextChangedListener(new PasswordListener());
            var button = v.FindViewById<Button>(Resource.Id.testbutton);
            button.Click += (s, e) =>
            {
                Console.WriteLine(Username.Text);
            };
        }

        private class UsernameListener : Java.Lang.Object, ITextWatcher
        {
            public void AfterTextChanged(IEditable s)
            {
            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
            {
            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
                Settings.UserName = s.ToString();
                Console.WriteLine(s.ToString());
            }
        }
        private class PasswordListener : Java.Lang.Object, ITextWatcher
        {
            public void AfterTextChanged(IEditable s)
            {
            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
            {
            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
                Settings.Password = s.ToString();
            }
        }
    }
    class IntroFragment2 : BaseIntroFragment
    {
        public Spinner CalendarIdSpinner { get; private set; }
        public List<long> IdList { get; private set; } = new List<long>();
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            Init(view);
            return view;
        }
        private void Init(View v)
        {

            CalendarIdSpinner = v.FindViewById<Spinner>(Resource.Id.calendarSelector);
            List<string> nameList = new List<string>();
            foreach (var x in Settings.CalendarObjects)
            {
                nameList.Add(x.DisplayName);
                IdList.Add(x.Id);
            }
            ArrayAdapter<string> spinneradapter =
                new ArrayAdapter<string>(this.Context, Android.Resource.Layout.SimpleSpinnerItem, nameList);
            spinneradapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            CalendarIdSpinner.Adapter = spinneradapter;
            CalendarIdSpinner.OnItemSelectedListener = new SpinnerListener(IdList);

        }
        private class SpinnerListener : Java.Lang.Object, AdapterView.IOnItemSelectedListener
        {
            private List<long> _idList;
            public SpinnerListener(List<long> idlist) => _idList = idlist;
            public void OnItemSelected(AdapterView parent, View view, int position, long id)
            {
                Settings.CalendarId = _idList[position];
            }

            public void OnNothingSelected(AdapterView parent)
            {
            }
        }
    }
}
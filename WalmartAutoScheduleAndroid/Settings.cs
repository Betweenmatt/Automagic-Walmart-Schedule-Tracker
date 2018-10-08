using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WalmartAutoScheduleAndroid.Scraper;

namespace WalmartAutoScheduleAndroid
{
    class Settings
    {
        public static bool IntroComplete { get; set; }
        private static bool _introComplete;

        public static bool ServiceRunning { get; set; }
        private static bool _serviceRunning;

        public static string EventTitle { get; set; }
        private static string _eventTitleLast;

        public static long CalendarId { get; set; }
        private static long _calendarIdLast;

        public static string UserName { get; set; }
        private static string _userNameLast;

        public static string Password { get; set; }
        private static string _passwordLast;

        public static int EventColorId { get; set; }
        private static int _eventColorIdLast;

        public static int UpdateEventColorId { get; set; }
        private static int _updateEventColorId;

        public static string Reminder { get; set; }
        private static string _reminder;

        public static bool IsCalendarGoogle { get; set; }
        private static bool _isCalendarGoogle;
        

        public static NotificationFlag NotificationFlags { get; set; }
        private static NotificationFlag _notificationFlags;

        public static WalmartOneStatus WalmartOneStatus { get; set; }
        private static WalmartOneStatus _walmartOneStatus;

        public static bool ShowDaysOff { get; set; }
        private static bool _showDaysOff;

        public static int DayOffColorId { get; set; }
        private static int _dayOffColorId;

        public static List<CalendarObject> CalendarObjects { get; set; }



        internal class Consts
        {
            public const string EventTitle = "EventTitle";
            public const string EventTitleDef = "Walmart - Work";
            public const string CalendarId = "CalendarId";
            public const long CalendarIdDef = 1;
            public const string UserName = "UserName";
            public const string UserNameDef = "";
            public const string Password = "Password";
            public const string PasswordDef = "";
            public const string EventColorId = "EventColorId";
            public const int EventColorIdDef = 6;
            public const string UpdateEventColorId = "";
            public const int UpdateEventColorIdDef = 10;
            public const string NotificationFlags = "NotificationFlags";
            public const NotificationFlag NotificationFlagsDef =
                NotificationFlag.AddShift |
                NotificationFlag.DeleteShift |
                NotificationFlag.UpdateShift |
                NotificationFlag.Error;
            public const string IntroComplete = "IntroComplete";
            public const bool IntroCompleteDef = false;
            public const string ServiceRunning = "ServiceRunning";
            public const bool ServiceRunningDef = false;
            public const string WalmartOneStatus = "WalmartOneStatus";
            public const WalmartOneStatus WalmartOneStatusDef = WalmartAutoScheduleAndroid.WalmartOneStatus.Unknown;
            public const string Reminder = "Reminder";
            public const string ReminderDef = "0";
            public const string IsCalendarGoogle = "IsCalendarGoogle";
            public const bool IsCalendarGoogleDef = false;
            public const string ShowDaysOff = "ShowDaysOff";
            public const bool ShowDaysOffDef = false;
            public const string DayOffColorId = "DayOffColorId";
            public const int DayOffColorIdDef = 7;
        }

        public static void SaveAllSettings(Context context)
        {
            ISharedPreferencesEditor editor = GetSettings(context).Edit();
            if (EventTitle != _eventTitleLast)
                editor.PutString(Consts.EventTitle, _eventTitleLast = EventTitle);
            if (CalendarId != _calendarIdLast)
                editor.PutLong(Consts.CalendarId, _calendarIdLast = CalendarId);
            if (UserName != _userNameLast)
                editor.PutString(Consts.UserName, _userNameLast = UserName);
            if (Password != _passwordLast)
                editor.PutString(Consts.Password, _passwordLast = Password);
            if (EventColorId != _eventColorIdLast)
                editor.PutInt(Consts.EventColorId, _eventColorIdLast = EventColorId);
            if (UpdateEventColorId != _updateEventColorId)
                editor.PutInt(Consts.UpdateEventColorId, _updateEventColorId = UpdateEventColorId);
            if (NotificationFlags != _notificationFlags)
                editor.PutInt(Consts.NotificationFlags, (int)(_notificationFlags = NotificationFlags));
            if (IntroComplete != _introComplete)
                editor.PutBoolean(Consts.IntroComplete, _introComplete = IntroComplete);
            if (ServiceRunning != _serviceRunning)
                editor.PutBoolean(Consts.ServiceRunning, _serviceRunning = ServiceRunning);
            if (WalmartOneStatus != _walmartOneStatus)
                editor.PutInt(Consts.WalmartOneStatus, (int)(_walmartOneStatus = WalmartOneStatus));
            if (Reminder != _reminder)
                editor.PutString(Consts.Reminder, _reminder = Reminder);
            if (IsCalendarGoogle != _isCalendarGoogle)
                editor.PutBoolean(Consts.IsCalendarGoogle, _isCalendarGoogle = IsCalendarGoogle);
            if (ShowDaysOff != _showDaysOff)
                editor.PutBoolean(Consts.ShowDaysOff, _showDaysOff = ShowDaysOff);
            if (DayOffColorId != _dayOffColorId)
                editor.PutInt(Consts.DayOffColorId, _dayOffColorId = DayOffColorId);
            editor.Apply();
        }

        public static void LoadAllSettings(Context context)
        {
            ISharedPreferences settings = GetSettings(context);
            EventTitle = _eventTitleLast = settings.GetString(Consts.EventTitle, Consts.EventTitleDef);
            CalendarId = _calendarIdLast = settings.GetLong(Consts.CalendarId, Consts.CalendarIdDef);
            UserName = _userNameLast = settings.GetString(Consts.UserName, Consts.UserNameDef);
            Password = _passwordLast = settings.GetString(Consts.Password, Consts.PasswordDef);
            EventColorId = _eventColorIdLast = settings.GetInt(Consts.EventColorId, Consts.EventColorIdDef);
            UpdateEventColorId = _updateEventColorId = settings.GetInt(Consts.UpdateEventColorId, Consts.UpdateEventColorIdDef);
            NotificationFlags = _notificationFlags = 
                (NotificationFlag)settings.GetInt(Consts.NotificationFlags, (int)Consts.NotificationFlagsDef);
            IntroComplete = _introComplete = settings.GetBoolean(Consts.IntroComplete, Consts.IntroCompleteDef);
            ServiceRunning = _serviceRunning = settings.GetBoolean(Consts.ServiceRunning, Consts.ServiceRunningDef);
            WalmartOneStatus = _walmartOneStatus = (WalmartOneStatus)settings.GetInt(Consts.WalmartOneStatus, (int)Consts.WalmartOneStatusDef);
            Reminder = _reminder = settings.GetString(Consts.Reminder, Consts.ReminderDef);
            IsCalendarGoogle = _isCalendarGoogle = settings.GetBoolean(Consts.IsCalendarGoogle, Consts.IsCalendarGoogleDef);
            ShowDaysOff = _showDaysOff = settings.GetBoolean(Consts.ShowDaysOff, Consts.ShowDaysOffDef);
            DayOffColorId = _dayOffColorId = settings.GetInt(Consts.DayOffColorId, Consts.DayOffColorIdDef);
        }

        private static ISharedPreferences GetSettings(Context context)
        {
            return context.GetSharedPreferences("settings", FileCreationMode.Private);
        }
    }
    class SettingsObject
    {
        public string EventTitle { get; }

        public long CalendarId { get; }

        public string UserName { get; }

        public string Password { get; }

        public int EventColorId { get; }

        public int UpdatedColorId { get; }

        public NotificationFlag NotificationFlags { get; }

        public string Reminder { get; }

        public bool IsCalendarGoogle { get; }
        

        public SettingsObject()
        {
            EventTitle = Settings.EventTitle;
            CalendarId = Settings.CalendarId;
            UserName = Settings.UserName;
            Password = Settings.Password;
            EventColorId = Settings.EventColorId;
            UpdatedColorId = Settings.UpdateEventColorId;
            NotificationFlags = Settings.NotificationFlags;
            Reminder = Settings.Reminder;
            IsCalendarGoogle = Settings.IsCalendarGoogle;
        }
    }
}
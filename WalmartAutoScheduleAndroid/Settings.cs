﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
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


        public static ServerStatus ServerStatus { get; set; }
        private static ServerStatus _serverStatus;

        public static List<int> PushNotificationIds { get; set; }
        private static List<int> _pushNotificationIds;

        public static string SecretKey { get; set; }
        private static string _secretKey;

        public static string OtpKey { get; set; }
        private static string _otpKey;
        
        public static string OtpSecret { get; set; }
        private static string _otpSecret;

        public static bool Wm1Flag { get; set; }
        private static bool _wm1Flag;

        public static string WinNumber { get; set; }
        private static string _winNumber;

        public static string StoreNumber { get; set; }
        private static string _storeNumber;



        public const string Version = "1.0.13";
        public const int VersionCode = 14;


        internal class Consts
        {
            public const string EventTitle = "EventTitle";
            public const string EventTitleDef = "Walmart - Work";
            public const string CalendarId = "CalendarId";
            public const long CalendarIdDef = 1;
            public const string UserName = "WINnumber";
            public const string UserNameDef = "";
            public const string Password = "StoreNumber";
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
                NotificationFlag.Error |
                NotificationFlag.Status |
                NotificationFlag.Push;
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
            public const string ServerStatusString = "ServerStatus";
            public const ServerStatus ServerStatusDef = ServerStatus.Ok;
            public const string PushNotificationIds = "PushNotificationIds";
            public const string PushNotificationIdsDef = "";
            public const string SecretKey = "SecretKey";
            public const string SecretKeyDef = "";
            public const string OtpKey = "OtpKey";
            public const string OtpKeyDef = "";
            public const string OtpSecret = "OtpSecret";
            public const string OtpSecretDef = "";
            public const string Wm1Flag = "WmOneFlag";
            public const bool Wm1FlagDef = false;
            public const string WinNumber = "WinNumber";
            public const string WinNumberDef = "";
            public const string StoreNumber = "StoreNumber";
            public const string StoreNumberDef = "";
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
            if (ServerStatus != _serverStatus)
                editor.PutInt(Consts.ServerStatusString, (int)(_serverStatus = ServerStatus));
            if (SecretKey != _secretKey)
                editor.PutString(Consts.SecretKey, _secretKey = SecretKey);
            if (OtpKey != _otpKey)
                editor.PutString(Consts.OtpKey, _otpKey = OtpKey);
            if (OtpSecret != _otpSecret)
                editor.PutString(Consts.OtpSecret, _otpSecret = OtpSecret);
            if (Wm1Flag != _wm1Flag)
                editor.PutBoolean(Consts.Wm1Flag, _wm1Flag = Wm1Flag);
            if (WinNumber != _winNumber)
                editor.PutString(Consts.WinNumber, _winNumber = WinNumber);
            if (StoreNumber != _storeNumber)
                editor.PutString(Consts.StoreNumber, _storeNumber = StoreNumber);
            if (PushNotificationIds.Count != _pushNotificationIds.Count)
            {
                var json = JsonConvert.SerializeObject(PushNotificationIds);
                editor.PutString(Consts.PushNotificationIds, JsonConvert.SerializeObject(_pushNotificationIds = JsonConvert.DeserializeObject<List<int>>(json)));
            }
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
            ServerStatus = _serverStatus = (ServerStatus)settings.GetInt(Consts.ServerStatusString, (int)Consts.ServerStatusDef);
            PushNotificationIds = JsonConvert.DeserializeObject<List<int>>(settings.GetString(Consts.PushNotificationIds, Consts.PushNotificationIdsDef));
            _pushNotificationIds = JsonConvert.DeserializeObject<List<int>>(settings.GetString(Consts.PushNotificationIds, Consts.PushNotificationIdsDef));
            _secretKey = SecretKey = settings.GetString(Consts.SecretKey, Consts.SecretKeyDef);
            _otpKey = OtpKey = settings.GetString(Consts.OtpKey, Consts.OtpKeyDef);
            _otpSecret = OtpSecret = settings.GetString(Consts.OtpSecret, Consts.OtpSecretDef);
            _wm1Flag = Wm1Flag = settings.GetBoolean(Consts.Wm1Flag, Consts.Wm1FlagDef);
            _winNumber = WinNumber = settings.GetString(Consts.WinNumber, Consts.WinNumberDef);
            _storeNumber = StoreNumber = settings.GetString(Consts.StoreNumber, Consts.StoreNumberDef);
            if (PushNotificationIds == null)
            {
                PushNotificationIds = new List<int>();
                _pushNotificationIds = new List<int>();
            }
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
        
        public bool Wm1Flag { get; }

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
            Wm1Flag = Settings.Wm1Flag;
        }
    }
}
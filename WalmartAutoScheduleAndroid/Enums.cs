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

namespace WalmartAutoScheduleAndroid
{
    [Flags]
    enum NotificationFlag
    {
        None = 0,
        UpdateShift = 1 << 0,
        DeleteShift = 1 << 1,
        AddShift = 1 << 2,
        Error = 1 << 3,
        Status = 1 << 4,
        Push = 1 << 5
    }
    enum EventDataType
    {
        Skip,
        Insert,
        Update,
        Delete
    }
    enum SiteScraperReturnStatus
    {
        Success,
        WrongLogin,
        Error
    }
    enum WalmartOneStatus
    {
        Unknown,
        Online,
        Offline,
        LoginInfoWrong
    }
    enum ServerStatus
    {
        Error = 500,
        Ok = 200,
        Update = 100,
        Down = 300
    }
}
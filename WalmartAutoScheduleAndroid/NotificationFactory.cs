using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;

namespace WalmartAutoScheduleAndroid
{
    class NotificationFactory
    {
        public const string Channel = "WMASNOTIFCHANNEL";
        public NotificationFactory(Context context, string title, string text, NotificationFlag type)
        {
            PendingIntent intent = GetOpenCalendarIntent(context);
            //pop the notification
            NotificationCompat.Builder notif = new NotificationCompat.Builder(context, Channel);
            notif.SetSmallIcon(Resource.Drawable.ic_wmautomagic);
            notif.SetContentTitle(title);
            notif.SetContentText(text);
            notif.SetContentIntent(intent);
            notif.SetDefaults((int)NotificationDefaults.All);
            notif.SetPriority((int)NotificationPriority.Default);
            notif.SetAutoCancel(true);
            notif.SetColor(ContextCompat.GetColor(context, Resource.Color.colorPrimary));
            NotificationManagerCompat notificationManager = NotificationManagerCompat.From(context);
            notificationManager.Notify((int)type, notif.Build());
        }

        private PendingIntent GetOpenCalendarIntent(Context context)
        {
            //intent to open the default calendar app
            Android.Net.Uri calendarUri = CalendarContract.ContentUri
            .BuildUpon()
            .AppendPath("time")
            .Build();
            PendingIntent intent = PendingIntent.GetActivity(context, 0, new Intent(Intent.ActionView, calendarUri), 0);
            return intent;
        }
    }
}
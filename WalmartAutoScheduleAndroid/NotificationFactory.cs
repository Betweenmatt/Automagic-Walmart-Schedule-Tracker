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
using WalmartAutoScheduleAndroid.Activities;

namespace WalmartAutoScheduleAndroid
{
    class NotificationFactory
    {
        public const string Channel = "WMASNOTIFCHANNEL";
        public NotificationFactory(Context context, string title, string text, NotificationFlag type)
        {
            PendingIntent intent = GetOpenCalendarIntent(context);
            ShipNotificationFactory(context, title, text, type, intent);
        }
        public NotificationFactory(Context context, string title, string text, NotificationFlag type, PendingIntentType intentType)
        {
            PendingIntent intent = null;
            switch (intentType)
            {
                case (PendingIntentType.Changes):
                    intent = OpenChanges(context);
                    break;
                case (PendingIntentType.MainActivity):
                    intent = OpenMainActivity(context);
                    break;
            }
            if (intent != null)
                ShipNotificationFactory(context, title, text, type, intent);
        }

        private void ShipNotificationFactory(Context context, string title, string text, NotificationFlag type, PendingIntent intent)
        {
            //pop the notification
            NotificationCompat.Builder notif = new NotificationCompat.Builder(context, Channel);
            notif.SetSmallIcon(Resource.Drawable.ic_wmautomagic);
            notif.SetContentTitle(title);
            notif.SetContentText(text);
            notif.SetContentIntent(intent);
            notif.SetDefaults((int)NotificationDefaults.All);
            notif.SetPriority((int)NotificationPriority.Default);
            notif.SetAutoCancel(true);
            notif.SetStyle(new NotificationCompat.BigTextStyle().BigText(text));
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
        private PendingIntent OpenChanges(Context context)
        {
            var intent = new Intent(context, typeof(ChangesActivity));
            PendingIntent output = PendingIntent.GetActivity(context, 0, intent, 0);
            return output;
        }
        private PendingIntent OpenMainActivity(Context context)
        {
            var intent = new Intent(context, typeof(MainActivity));
            PendingIntent output = PendingIntent.GetActivity(context, 0, intent, 0);
            return output;
        }
    }
    public enum PendingIntentType
    {
        Changes,
        Settings,
        MainActivity
    }
}
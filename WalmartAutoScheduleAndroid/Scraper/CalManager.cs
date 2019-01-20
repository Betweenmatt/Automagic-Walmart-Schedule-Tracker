using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using SQLite;
using WalmartAutoScheduleAndroid.Scraper;

namespace WalmartAutoScheduleAndroid
{
    class CalManager
    {
        private string _dbPath =
            System.IO.Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                "wmAutoScheduler.db3");
        /// <summary>
        /// allows calendar related operations if true, which is passed when calendar r/w permissions are active
        /// </summary>
        private bool _permissions;
        public CalManager(bool permissions)
        {
            _permissions = permissions;
            if (!File.Exists(_dbPath))
            {
                SQLiteConnection db = Connection();
                db.CreateTable<Day>();
            }
        }

        private SQLiteConnection Connection() => new SQLiteConnection(_dbPath, true);

        public void DeleteAllEntries(Context context)
        {
            if (!_permissions)
                return;
            SQLiteConnection db = Connection();
            var list = db.Table<Day>();
            foreach (var x in list)
            {
                try
                {
                    var eventUri = ContentUris.WithAppendedId(CalendarContract.Events.ContentUri, long.Parse(x.EventId));
                    context.ContentResolver.Delete(eventUri, null, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                
            }
            db.DeleteAll<Day>();
            Toast.MakeText(context, "All entries deleted", ToastLength.Long).Show();
        }

        public void DeleteAllEntriesWithName(Context context, SettingsObject settings)
        {
            try
            {
                List<string> listOfIds = new List<string>();
                ICursor cursor = context.ContentResolver.Query(
                    CalendarContract.Events.ContentUri, new string[] {
                    CalendarContract.Events.InterfaceConsts.CalendarId,
                    CalendarContract.Events.InterfaceConsts.Title,
                    CalendarContract.Events.InterfaceConsts.Id
                    }, null, null, null);
                cursor.MoveToFirst();
                for (int i = 0; i < cursor.Count; i++)
                {
                    long calid = cursor.GetLong(0);
                    string title = cursor.GetString(1);
                    string id = cursor.GetString(2);

                    if (calid == settings.CalendarId)
                    {
                        if (title == settings.EventTitle)
                        {
                            listOfIds.Add(id);
                        }
                    }
                    cursor.MoveToNext();
                }
                cursor.Close();
                foreach (var x in listOfIds)
                {
                    var eventUri = ContentUris.WithAppendedId(CalendarContract.Events.ContentUri, long.Parse(x));
                    context.ContentResolver.Delete(eventUri, null, null);
                }
                Toast.MakeText(context, "All events have been deleted.", ToastLength.Short).Show();
            }catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Execute(Context context, SettingsObject settings, List<Day> days)
        {
            if (!_permissions)
                return;
            SQLiteConnection db = Connection();
            //clean db every run
            try
            {
                db.Execute("DELETE FROM Day WHERE Start < ?", DateTime.Now.AddDays(-38).Ticks.ToString());
            }
            catch { }
            var tbl = db.Table<Day>().ToList();

            List<DateTime> insertDateList = new List<DateTime>();
            List<DateTime> updateDateList = new List<DateTime>();
            List<DateTime> deleteDateList = new List<DateTime>();
            List<DateTime> errorDateList = new List<DateTime>();
            //the list of inserted objects that need the reminders removed
            //since calendarcontract is a poop head with reminders
            List<Day> addedList = new List<Day>();
            foreach (var d in days)
            {
                EventDataType type = EventDataType.Insert;
                var getsamedate = tbl.FirstOrDefault(f => f.BackupStart.Date == d.Start.Date);
                
                if (getsamedate != null)
                {
                    if (getsamedate.BackupStart == d.Start && getsamedate.BackupEnd == d.End)
                        type = EventDataType.Skip;
                    else
                        type = EventDataType.Update;
                }

                if (type == EventDataType.Insert)
                {
                    var values = GetContentValues(settings.CalendarId, settings.EventTitle == "" ? d.Shift : settings.EventTitle, $"{d.Shift}\n{d.Meal}", d.Start, d.End, d.IsError ? settings.UpdatedColorId : settings.EventColorId,settings.IsCalendarGoogle);
                    var uri = context.ContentResolver.Insert(CalendarContract.Events.ContentUri, values);
                    var id = uri.LastPathSegment;
                    d.EventId = id;
                    d.Start = d.Start;


                    db.Insert(d);
                    addedList.Add(d);
                    if (d.Start.Date > DateTime.Now)
                        insertDateList.Add(d.Start);
                    if (d.IsError)
                        errorDateList.Add(d.Start.Date);
                }
                else if (type == EventDataType.Update)
                {
                    var eventUri = ContentUris.WithAppendedId(CalendarContract.Events.ContentUri, long.Parse(getsamedate.EventId));
                    var values = GetContentValues(settings.CalendarId, settings.EventTitle == "" ? d.Shift : settings.EventTitle, $"{d.Shift}\n{d.Meal}", d.Start, d.End, settings.UpdatedColorId, settings.IsCalendarGoogle);
                    context.ContentResolver.Update(eventUri, values, null, null);
                    getsamedate.ChangeAutoSetTimes(d.Start, d.End);
                    db.Update(getsamedate);
                    if (d.Start.Date > DateTime.Now)
                        updateDateList.Add(d.Start);
                    if (d.IsError)
                        errorDateList.Add(d.Start.Date);
                }
                else if (type == EventDataType.Skip)
                {
                    //do nothing, yay!
                }
            }
            //check for deleted shifts
            foreach (var x in tbl)
            {
                if (x.Start.Date < DateTime.Now.Date || x.ManualAdjustment)
                    continue;
                var getdate = days.FirstOrDefault(f => f.Start.Date == x.Start.Date);
                if (getdate == null)
                {
                    var eventUri = ContentUris.WithAppendedId(CalendarContract.Events.ContentUri, long.Parse(x.EventId));
                    context.ContentResolver.Delete(eventUri, null, null);
                    db.Delete(x);
                    deleteDateList.Add(x.Start.Date);
                }
            }
            //pop notifications when needed
            if (updateDateList.Count > 0)
            {
                if (settings.NotificationFlags.HasFlag(NotificationFlag.UpdateShift))
                    new NotificationFactory(context, "Walmart Schedule Updated",
                        $"Your shift(s) on {DateTimeListToString(updateDateList)} have been changed! Click here to open your calandar app.",
                        NotificationFlag.UpdateShift);
            }
            if (insertDateList.Count > 0)
            {
                if (settings.NotificationFlags.HasFlag(NotificationFlag.AddShift))
                    new NotificationFactory(context, "Walmart Schedule Updated",
                        $"You have newly added shifts on {DateTimeListToString(insertDateList)}! Click here to open your calandar app.",
                        NotificationFlag.AddShift);
            }
            if (deleteDateList.Count > 0)
            {
                if (settings.NotificationFlags.HasFlag(NotificationFlag.DeleteShift))
                    new NotificationFactory(context, "Walmart Schedule Updated",
                        $"Your shift(s) on {DateTimeListToString(deleteDateList)} has been removed! Click here to open your calandar app.",
                        NotificationFlag.DeleteShift);
            }
            if (errorDateList.Count > 0)
            {
                if (settings.NotificationFlags.HasFlag(NotificationFlag.Error))
                    new NotificationFactory(context, "Walmart Schedule Updated",
                        $"There was an error pulling shift(s): {DateTimeListToString(errorDateList)}. Please check the WalmartOne site for these specific dates.",
                        NotificationFlag.Error);
            }
            Console.WriteLine("DONE-------------------");

        }
        public void ChangeTimeslot(Context context, SettingsObject settings, Day obj, int color)
        {
            if (!_permissions)
                return;

            var db = Connection();
            if (obj.EventId != null)
            {
                var eventUri = ContentUris.WithAppendedId(CalendarContract.Events.ContentUri, long.Parse(obj.EventId));
                var values = GetContentValues(settings.CalendarId,
                    settings.EventTitle == "" ? obj.Shift : settings.EventTitle, $"{obj.Shift}\n{obj.Meal}",
                    obj.Start, obj.End,
                    color,
                    settings.IsCalendarGoogle);
                context.ContentResolver.Update(eventUri, values, null, null);

                if (obj.DayId != -1)
                    db.Update(obj);
                else
                {
                    obj.DayId = 0;
                    db.Insert(obj);
                }
            }
            else
            {
                var values = GetContentValues(settings.CalendarId,
                    settings.EventTitle == "" ? obj.Shift : settings.EventTitle, $"{obj.Shift}\n{obj.Meal}",
                    obj.Start, obj.End,
                    color,
                    settings.IsCalendarGoogle);
                var uri = context.ContentResolver.Insert(CalendarContract.Events.ContentUri, values);
                var id = uri.LastPathSegment;
                obj.EventId = id;
                obj.Start = obj.Start;
                
                db.Insert(obj);
            }
        }
        public void SetReminders(Context context, SettingsObject settings)
        {
            if (!_permissions)
                return;
            Console.WriteLine($"Reminder: {settings.Reminder}");
            var list = this.GetEventCollection();
            List<Day> updatedList = new List<Day>();
            foreach(var x in list)
            {
                if(x.Reminder != settings.Reminder)
                {
                    context.ContentResolver
                        .Delete(CalendarContract.Reminders.ContentUri,
                        CalendarContract.Reminders.InterfaceConsts.EventId + "=?", new string[] { x.EventId });
                    if(settings.Reminder != "0")
                    {
                        ContentValues vals = new ContentValues();
                        vals.Put(CalendarContract.Reminders.InterfaceConsts.EventId, x.EventId);
                        vals.Put(CalendarContract.Reminders.InterfaceConsts.Minutes, settings.Reminder);
                        vals.Put(CalendarContract.Reminders.InterfaceConsts.Method, (int)Android.Provider.RemindersMethod.Alert);
                        context.ContentResolver
                            .Insert(CalendarContract.Reminders.ContentUri,
                            vals);
                    }
                    x.Reminder = settings.Reminder;
                    updatedList.Add(x);
                }
            }
            if(updatedList.Count > 0)
                Connection().UpdateAll(updatedList);
            Console.WriteLine("WMS: Reminder update complete.");
        }
        private string DateTimeListToString(List<DateTime> list)
        {
            string output = "";
            if (list.Count == 1)
                return output = list[0].Date.ToShortDateString();
            else
            {
                foreach(var x in list)
                {
                    output += x.Date.ToShortDateString() + ", ";
                }
            }
            return output.Substring(0,output.Length -2);
        }
        private ContentValues GetContentValues(long calid, string title, string desc, DateTime start, DateTime end, int color, bool isgoogle)
        {
            ContentValues values = new ContentValues();
            values.Put(CalendarContract.Events.InterfaceConsts.CalendarId, calid);
            values.Put(CalendarContract.Events.InterfaceConsts.Title, title);
            values.Put(CalendarContract.Events.InterfaceConsts.Description, desc);
            values.Put(CalendarContract.Events.InterfaceConsts.Dtstart, GetDateTimeMs(start));
            values.Put(CalendarContract.Events.InterfaceConsts.Dtend, GetDateTimeMs(end));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                Android.Icu.Util.TimeZone tx = Android.Icu.Util.TimeZone.Default;
                values.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, tx.ID);
            }
            else
            {
                Java.Util.TimeZone tx = Java.Util.TimeZone.Default;
                values.Put(CalendarContract.Events.InterfaceConsts.EventTimezone, tx.ID);
            }
            values.Put(CalendarContract.Events.InterfaceConsts.HasAlarm, 0);
            if(isgoogle)
                values.Put(CalendarContract.Events.InterfaceConsts.EventColorKey, (color + 1).ToString());
            return values;
        }
        private long GetDateTimeMs(DateTime d)
        {
            return (long)d
                .ToUniversalTime()
                .Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;
        }
        /// <summary>
        /// Method for returning all the calendars on the current device.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<CalendarObject> GetCalendars(Context context)
        {
            if (!_permissions)
                return new List<CalendarObject>();
            string[] EVENT_PROJECTION = new string[]{
                CalendarContract.Calendars.InterfaceConsts.Id,
                CalendarContract.Calendars.InterfaceConsts.CalendarDisplayName,
                CalendarContract.Calendars.InterfaceConsts.CalendarColor,
                CalendarContract.Calendars.InterfaceConsts.AccountType
            };

            ContentResolver cr = context.ContentResolver;
            Android.Net.Uri uri = CalendarContract.Calendars.ContentUri;
            ICursor cur = cr.Query(uri, EVENT_PROJECTION, null, null, null);
            List<CalendarObject> list = new List<CalendarObject>();

            while (cur.MoveToNext())
            {
                var id = cur.GetLong(0);
                var name = cur.GetString(1);
                var type = cur.GetString(3);
                if (list.FirstOrDefault(f => f.DisplayName == name) == null)
                {
                    list.Add(new CalendarObject(
                        id,
                        name,
                        cur.GetInt(2),
                        type
                        ));
                }
            }
            cur.Close();
            return list;
        }
        public List<Day> GetEventCollection()
        {
            SQLiteConnection db = Connection();
            return db.Table<Day>().ToList();   
        }
        public void DeleteTimeslot (Context context, Day obj)
        {
            var eventUri = ContentUris.WithAppendedId(CalendarContract.Events.ContentUri, long.Parse(obj.EventId));
            context.ContentResolver.Delete(eventUri, null, null);
            SQLiteConnection db = Connection();
            db.Update(obj);
        }
    }
}
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
using SQLite;

namespace WalmartAutoScheduleAndroid.Scraper
{
    public class Day
    {
        [PrimaryKey,AutoIncrement]
        public int DayId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Shift { get; set; }
        public string Meal { get; set; }
        public string Identifier { get; set; }
        public string EventId { get; set; }
        public bool IsError { get; set; }
        public bool IsUpdated { get; set; }
        public string Reminder { get; set; }
        public int OverrideColor { get; set; } = -1;
        public bool Ignore { get; set; }
        [Ignore]
        public double TotalHours { get; set; }

        public bool ManualAdjustment { get; set; }
        public DateTime BackupStart { get; set; }
        public DateTime BackupEnd { get; set; }

        public Day() { }
        /// <summary>
        /// Constructor to indicate there was an error pulling the shift. This will cause a notification as well as
        /// create an all day event in the calendar on the day in question so the user will know to check WM1 schedule.
        /// </summary>
        /// <param name="date"></param>
        public Day(string date)
            :this(date, "1pm - 5pm", "Error!","There was an error pulling this shift. Please check WalmartOne to see the actual schedule!")
        {
            IsError = true;
            Start = Start.Date;
            End = Start.Date.AddDays(1);

            BackupEnd = End;
            BackupStart = Start;
        }
        public Day(string date, string times, string shift, string meal)
        {
            Shift = shift.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            Meal = meal.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            var getTimes = times.Replace(" ", "");
            if (getTimes != "")
            {
                var splitTimes = getTimes.Split('-');
                string start = "";
                string end = "";
                if (splitTimes[0].Contains("am"))
                    start = splitTimes[0].Replace("am", " AM");
                else
                    start = splitTimes[0].Replace("pm", " PM");

                if (splitTimes[1].Contains("am"))
                    end = splitTimes[1].Replace("am", " AM");
                else
                    end = splitTimes[1].Replace("pm", " PM");
                date = date.Insert(6, "/").Insert(4, "/");
                Start = GetDate(date, start);
                End = GetDate(date, end);

                if (End < Start)
                    End = End.AddDays(1);

                //////test cases
                //UpdatingTestCase();
                //DeletingTestCase();
                ///////
                BackupStart = Start;
                BackupEnd = End;

                Identifier = this.ToString();
            }
        }
        /// <summary>
        /// A quick test case method for quickly seeing the changes brought on by updated schedules.
        /// this should incur schedule updates at random, but common intervals to see how the app
        /// responds and ensure everything is working as intended. dont run this on live bro lol
        /// </summary>
        private void UpdatingTestCase()
        {
            var rng = new Random();
            if(rng.Next(0,4) == 0)
            {
                Start = Start.AddHours(1);
                Console.WriteLine($"WMS: {Start.Date} :: Random update test shift");
            }
        }
        /// <summary>
        /// This quick test case method should be testing for random new shifts/deleted shifts to ensure
        /// notifications are working as intended.
        /// </summary>
        private void DeletingTestCase()
        {
            var rng = new Random();
            if(rng.Next(0,15) == 0)
            {
                Start = Start.AddDays(1);
                Console.WriteLine($"WMS: {Start.Date} :: Random delete test shift");
            }
        }
        /// <summary>
        /// This should not be used for manual changes
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void ChangeAutoSetTimes(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
            BackupStart = start;
            BackupEnd = end;
            IsUpdated = true;
        }
        private DateTime GetDate(string d, string t)
        {
            var ds = d.Split('/');
            var zone = t.Split(' ');
            var ts = zone[0].Split(':');
            var arr = new string[] { ds[0], ds[1], ds[2], ts[0], (ts.Length > 1) ? ts[1] : "00", zone[1] };
            var obj = new DateTimeConverter(arr).Convert();
            return obj;
        }
        public double GetTotalHours()
        {
            if (IsError || Ignore || DayId == -1 || DayId == -2)
                return 0;
            var diff = End - Start;
            return diff.TotalHours - GetMealTime();
        }
        private double GetMealTime()
        {
            if (!Meal.Any(char.IsDigit))
                return 0;

            var obj = Meal.ToLower().Replace("meal", "").Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", "");
            try
            {
                var split = obj.Split('-');
                var zoneS = "AM";
                var zoneE = "AM";
                if (split[0].ElementAt(split[0].Length - 2).ToString() == "p")
                    zoneS = "PM";
                if (split[1].ElementAt(split[1].Length - 2).ToString() == "p")
                    zoneE = "PM";

                var startTimeSplit = split[0].Replace("am", "").Replace("pm", "").Split(':');
                var endTimeSplit = split[1].Replace("am", "").Replace("pm", "").Split(':');

                var arrS = new string[] { startTimeSplit[0], (startTimeSplit.Length > 1) ? startTimeSplit[1] : "00", zoneS };
                var arrE = new string[] { endTimeSplit[0], (endTimeSplit.Length > 1) ? endTimeSplit[1] : "00", zoneE };
                var startObj = new DateTimeConverter(arrS).Convert();
                var endObj = new DateTimeConverter(arrE).Convert();
                var timespan = endObj - startObj;
                return timespan.TotalHours;
            }
            catch{
                return 0;
            }
        }
        public override string ToString()
        {
            return $"{Start.ToString()}-{End.ToString()}";
        }

        private class DateTimeConverter
        {
            public int Hour { get; }
            public int Minute { get; }
            public string Zone { get; }
            public int Day { get; }
            public int Month { get; }
            public int Year { get; }
            public DateTimeConverter(string[] s)
            {
                if (s.Length == 6)
                {
                    Year = int.Parse(s[0]);
                    Month = int.Parse(s[1]);
                    Day = int.Parse(s[2]);

                    Hour = int.Parse(s[3]);
                    Minute = int.Parse(s[4]);
                    Zone = s[5];
                }
                else if(s.Length == 3)
                {
                    Year = 2069;
                    Month = 6;
                    Day = 6;
                    Hour = int.Parse(s[0]);
                    Minute = int.Parse(s[1]);
                    Zone = s[2];
                }
            }
            public override string ToString()
            {
                return $"{Month}{Day}{Year}";
            }

            public System.DateTime Convert()
            {
                if (Zone == "AM")
                {
                    if (Hour == 12)
                        return new System.DateTime(Year, Month, Day, 0, Minute, 0);
                    else
                        return new System.DateTime(Year, Month, Day, Hour, Minute, 0);
                }
                else
                {
                    if (Hour == 12)
                        return new System.DateTime(Year, Month, Day, 12, Minute, 0);
                    else
                        return new System.DateTime(Year, Month, Day, Hour + 12, Minute, 0);
                }
            }
        }
    }

}
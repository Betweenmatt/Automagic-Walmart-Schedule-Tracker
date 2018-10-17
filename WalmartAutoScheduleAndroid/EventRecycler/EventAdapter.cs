using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using WalmartAutoScheduleAndroid.Scraper;

namespace WalmartAutoScheduleAndroid.EventRecycler
{
    class EventAdapter : RecyclerView.Adapter
    {
        private List<Day> _list = new List<Day>();
        public override int ItemCount => _list.Count;
        private int _nextShiftPos;
        //this is saved when attached, so that scrolltoposition works from this class. i dont think this will cause a memory problem
        //but if it does i'll research other solutions
        private RecyclerView _recycler;

        private List<string> _calendarcolorlist = new List<string>
        {
            "#A4BDFC",
            "#7AE7BF",
            "#DBADFF",
            "#FF887C",
            "#FBD75B",
            "#FFB878",
            "#46D6DB",
            "#d6d6d6",
            "#5484ED",
            "#51B749",
            "#DC2127"
        };

        public EventAdapter(Context context)
        {
            try
            {
                _list = CreateList(context);
            }
            catch
            {
                _list = new List<Day>();
                Toast.MakeText(context, "There was an issue getting the agenda view. Please try again. If this issue occurs often, please email support!", ToastLength.Long).Show();
            }
        }
        public void NotifyChange(Context context)
        {
            try
            {
                _list = CreateList(context);
            }
            catch
            {
                _list = new List<Day>();
                Toast.MakeText(context, "There was an issue getting the agenda view. Please try again. If this issue occurs often, please email support!", ToastLength.Long).Show();
            }
            NotifyDataSetChanged();
            ScrollTo();
        }

        private List<Day> CreateList(Context context)
        {
            List<Day> temp = new List<Day>();
            temp = new CalManager(Utilities.CheckCalendarPermissions(context)).GetEventCollection().Where(w => !w.Ignore).ToList();
            if (temp.Count < 1)
                return temp;
            if (Settings.ShowDaysOff)
            {
                try
                {
                    int length = temp.Count + 21;
                    DateTime startDate = temp[0].Start.Date;
                    DateTime lastPossibleDay = GetLastFriday(temp[temp.Count - 1].Start);
                    for(int i = 0; i < length; i++)
                    {
                        DateTime current = startDate.AddDays(i);
                        if (current.Date > lastPossibleDay.Date)
                            break;
                        if(temp.FirstOrDefault(f=>f.Start.Date == current.Date) == null)
                        {
                            temp.Add(new Day()
                            {
                                DayId = -1,
                                Start = current,
                                End = current.AddHours(23),
                                BackupStart = current,
                                BackupEnd = current.AddHours(23)
                            });
                        }
                    }
                }
                catch {
                    Toast.MakeText(context, "There was an issue showing days off, so they have been ignored.", ToastLength.Long).Show();
                }
            }
            temp = temp.OrderBy(o => o.Start).ToList();

            //add the current week object for each week
            int currentweek = 0;
            List<Day> tempAfterWeeks = new List<Day>();
            for(int i = 0; i < temp.Count; i++)
            {
                int getweek = Utilities.GetWalmartWeek(temp[i].BackupStart);
                if(getweek != currentweek)
                {
                    currentweek = getweek;
                    tempAfterWeeks.Add(new Day()
                    {
                        DayId = -2,
                        Shift = $"Week {getweek.ToString()}"
                    });
                }
                tempAfterWeeks.Add(temp[i]);
            }

            return tempAfterWeeks;
        }
        /// <summary>
        /// gets the last friday of the last week in the calendar(when given the last day in the database)
        /// </summary>
        /// <param name="lastDay"></param>
        /// <returns></returns>
        private DateTime GetLastFriday(DateTime lastDay)
        {
            switch (lastDay.DayOfWeek)
            {
                case (DayOfWeek.Friday):
                    return lastDay;
                case (DayOfWeek.Saturday):
                    return lastDay.AddDays(6);
                case (DayOfWeek.Sunday):
                    return lastDay.AddDays(5);
                case (DayOfWeek.Monday):
                    return lastDay.AddDays(4);
                case (DayOfWeek.Tuesday):
                    return lastDay.AddDays(3);
                case (DayOfWeek.Wednesday):
                    return lastDay.AddDays(2);
                case (DayOfWeek.Thursday):
                    return lastDay.AddDays(1);
            }
            return lastDay;
        }

        private void ScrollTo()
        {
            var obj = _list.FirstOrDefault(f => f.End >= DateTime.Now);
            _nextShiftPos = _list.IndexOf(obj);
            if(_recycler != null)
                _recycler.ScrollToPosition(_nextShiftPos);
        }
        public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
        {
            base.OnAttachedToRecyclerView(recyclerView);
            _recycler = recyclerView;
            ScrollTo();
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var obj = _list[position];
            if (holder is EventViewHolder h)
            {
                h.MainCard.Visibility = ViewStates.Visible;
                h.HeaderView.Visibility = ViewStates.Gone;
                if (obj.DayId == -1)
                {
                    h.Date.Text = obj.Start.Date.ToString("dddd, MMMM d") + DateSuffix(obj.Start);
                    h.Time.Text = "Unscheduled";
                    h.Shift.Text = obj.Shift;
                    h.Meal.Text = obj.Meal;
                    Color bg = Color.ParseColor(_calendarcolorlist[Settings.DayOffColorId]);
                    h.ElevatedCard.SetCardBackgroundColor(bg);
                    h.Date.SetTextColor(Color.White);
                    h.Date.SetTextSize(Android.Util.ComplexUnitType.Sp, 18);
                    h.Time.SetTextSize(Android.Util.ComplexUnitType.Sp, 18);
                    h.Time.SetTextColor(Color.White);
                }
                else if(obj.DayId == -2)
                {
                    h.MainCard.Visibility = ViewStates.Gone;
                    h.HeaderView.Visibility = ViewStates.Visible;
                    h.HeaderText.Text = obj.Shift;
                }
                else
                {
                    h.Date.Text = obj.Start.Date.ToString("dddd, MMMM d") + DateSuffix(obj.Start);
                    h.Time.Text = $"{obj.Start.ToShortTimeString()} - {obj.End.ToShortTimeString()}";
                    h.Shift.Text = obj.Shift;//.Replace("\r","").Replace("\n", "").Replace("\t", "");
                    h.Meal.Text = obj.Meal;//.Replace("\r", "").Replace("\n", "").Replace("\t", "");

                    Color bg = Color.ParseColor(
                         (obj.OverrideColor == -1) ?
                            ((obj.IsError || obj.IsUpdated)
                            ? _calendarcolorlist[Settings.UpdateEventColorId]
                            : _calendarcolorlist[Settings.EventColorId])
                        : _calendarcolorlist[obj.OverrideColor]
                        );
                    h.ElevatedCard.SetCardBackgroundColor(bg);
                    h.Date.SetTextColor(Color.White);
                    h.Date.SetTextSize(Android.Util.ComplexUnitType.Sp, 18);
                    h.Time.SetTextSize(Android.Util.ComplexUnitType.Sp, 18);
                    h.Time.SetTextColor(Color.White);
                }

                if (position == _nextShiftPos)
                {
                    h.MainCard.SetCardBackgroundColor(Color.PaleGreen);
                }
                else
                {
                    h.MainCard.SetCardBackgroundColor(Color.Transparent);
                }
                h.WorkingObject = obj;
            }
        }

        private string DateSuffix(DateTime d)
        {
            switch (d.Day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.recycler_layout_view, parent, false);
            return new EventViewHolder(v);
        }
    }
}
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
        private List<Day> _list;
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
            "#E1E1E1",
            "#5484ED",
            "#51B749",
            "#DC2127"
        };

        public EventAdapter(Context context)
        {
            _list = new CalManager(Utilities.CheckCalendarPermissions(context)).GetEventCollection()
                .Where(w => !w.Ignore).ToList();
        }
        public void NotifyChange(Context context)
        {
            _list = new CalManager(Utilities.CheckCalendarPermissions(context)).GetEventCollection()
                .Where(w => !w.Ignore).ToList();
            NotifyDataSetChanged();
            ScrollTo();
        }
        private void ScrollTo()
        {
            var obj = _list.FirstOrDefault(f => f.Start >= DateTime.Now);
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
            if(holder is EventViewHolder h)
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
                if(position == _nextShiftPos)
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using WalmartAutoScheduleAndroid.Activities;
using WalmartAutoScheduleAndroid.Scraper;

namespace WalmartAutoScheduleAndroid.EventRecycler
{
    class EventViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
    {
        public TextView Date { get; }
        public TextView Time { get; }
        public TextView Shift { get; }
        public TextView Meal { get; }
        public CardView MainCard { get; }
        public CardView ElevatedCard { get; }
        public LinearLayout HeaderView { get; }
        public TextView HeaderText { get; }
        public Day WorkingObject { get; set; }

        public EventViewHolder(View itemView) : base(itemView)
        {
            Date = itemView.FindViewById<TextView>(Resource.Id.date);
            Time = itemView.FindViewById<TextView>(Resource.Id.time);
            Shift = itemView.FindViewById<TextView>(Resource.Id.shift);
            Meal = itemView.FindViewById<TextView>(Resource.Id.meal);
            MainCard = itemView.FindViewById<CardView>(Resource.Id.card_view);
            ElevatedCard = itemView.FindViewById<CardView>(Resource.Id.elevated_card);
            HeaderView = itemView.FindViewById<LinearLayout>(Resource.Id.header_view);
            HeaderText = itemView.FindViewById<TextView>(Resource.Id.header_text);
            itemView.SetOnClickListener(this);
        }
        public void RemoveListener()
        {
            ItemView.SetOnClickListener(null);
        }
        public void OnClick(View v)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                ChangeTimeslotActivity.WorkingObj = WorkingObject;
                v.Context.StartActivity(new Android.Content.Intent(v.Context, typeof(ChangeTimeslotActivity)));
            }
            else
            {
                //too many issues atm with timepicker in <= M
                //time picker is throwing an exception that shouldn't be happening
                //
                //ChangeTimeslotActivityCompat.WorkingObj = WorkingObject;
                //v.Context.StartActivity(new Android.Content.Intent(v.Context, typeof(ChangeTimeslotActivityCompat)));
            }
        }
    }
}
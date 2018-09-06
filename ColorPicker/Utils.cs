using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ColorPicker
{
    class Utils
    {
        public static bool IsTablet(Context context)
        {
            //return (context.Resources.Configuration.ScreenLayout
            //        & Configuration.SCREENLAYOUT_SIZE_MASK)
            //       >= Configuration.SCREENLAYOUT_SIZE_LARGE;
            return false;
        }
        public static int[] ColorChoice(Context context)
        {

            int[] mColorChoices = null;
            String[] color_array = context.Resources.GetStringArray(Resource.Array.default_color_choice_values);

            if (color_array != null && color_array.Length > 0)
            {
                mColorChoices = new int[color_array.Length];
                for (int i = 0; i < color_array.Length; i++)
                {
                    mColorChoices[i] = Color.ParseColor(color_array[i]);
                }
            }
            return mColorChoices;
        }
    }
}
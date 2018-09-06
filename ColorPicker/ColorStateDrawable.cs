using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ColorPicker
{
    class ColorStateDrawable : LayerDrawable
    {
        private static float PRESSED_STATE_MULTIPLIER = 0.70f;

        private int mColor;

        public ColorStateDrawable(Drawable[] layers, int color) : base(layers)
        {
            mColor = color;
        }

        protected override bool OnStateChange(int[] states)
        {
            bool pressedOrFocused = false;
            foreach (int state in states)
            {
                if (state == Android.Resource.Attribute.StatePressed || state == Android.Resource.Attribute.StateFocused)
                {
                    pressedOrFocused = true;
                    break;
                }
            }

            if (pressedOrFocused)
            {
                base.SetColorFilter(new Color(GetPressedColor(mColor)), PorterDuff.Mode.SrcAtop);
            }
            else
            {
                base.SetColorFilter(new Color(mColor), PorterDuff.Mode.SrcAtop);
            }

            return base.OnStateChange(states);
        }

        /**
         * Given a particular color, adjusts its value by a multiplier.
         */
        private int GetPressedColor(int color)
        {
            float[] hsv = new float[3];
            Color.ColorToHSV(new Color(color), hsv);
            hsv[2] = hsv[2] * PRESSED_STATE_MULTIPLIER;
            return Color.HSVToColor(hsv);
        }

        public override bool IsStateful
        {
            get { return true; }
        }
    }
}
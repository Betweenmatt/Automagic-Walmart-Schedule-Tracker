using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ColorPicker
{
    class ColorPickerSwatch : FrameLayout, View.IOnClickListener
    {
        private int mColor;
        private ImageView mSwatchImage;
        private ImageView mCheckmarkImage;
        private IOnColorSelectedListener mOnColorSelectedListener;

        /**
         * Interface for a callback when a color square is selected.
         */
        public interface IOnColorSelectedListener
        {

            /**
             * Called when a specific color square has been selected.
             */
            void OnColorSelected(int color);
        }

        public ColorPickerSwatch(Context context, int color, bool check,
                IOnColorSelectedListener listener):base(context)
        {
            mColor = color;
            mOnColorSelectedListener = listener;

            LayoutInflater.From(context).Inflate(Resource.Layout.calendar_color_picker_swatch, this);
            mSwatchImage = (ImageView)FindViewById(Resource.Id.color_picker_swatch);
            mCheckmarkImage = (ImageView)FindViewById(Resource.Id.color_picker_checkmark);
            SetColor(color);
            SetChecked(check);
            SetOnClickListener(this);
        }

        protected void SetColor(int color)
        {
            Drawable[] colorDrawable = new Drawable[]
                    {Context.Resources.GetDrawable(Resource.Drawable.calendar_color_picker_swatch)};
            mSwatchImage.SetImageDrawable(new ColorStateDrawable(colorDrawable, color));
        }

        private void SetChecked(bool check)
        {
            if (check) {
                mCheckmarkImage.Visibility = (ViewStates.Visible);
            }
            else
            {
                mCheckmarkImage.Visibility = (ViewStates.Gone);
            }
        }

        public void OnClick(View v)
        {
            if (mOnColorSelectedListener != null)
            {
                mOnColorSelectedListener.OnColorSelected(mColor);
            }
        }
    }
}
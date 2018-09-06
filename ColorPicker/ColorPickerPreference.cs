using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace ColorPicker
{
    [Register("com.iammatt.colorpicker.colorPicker.ColorPickerPreference")]
    public class ColorPickerPreference : Preference
    {
        private int[] mColorChoices = { };
        private int mValue = 0;
        private int mItemLayoutId = Resource.Layout.calendar_grid_item_color;
        private int mNumColumns = 5;
        private View mPreviewView;

        public ColorPickerPreference(Context context)
            :base(context)
        {
            initAttrs(null, 0);
        }

        public ColorPickerPreference(Context context, IAttributeSet attrs)
            :base(context, attrs)
        {
            initAttrs(attrs, 0);
        }

        public ColorPickerPreference(Context context, IAttributeSet attrs, int defStyle)
            :base(context,attrs,defStyle)
        {
            initAttrs(attrs, defStyle);
        }

        private void initAttrs(IAttributeSet attrs, int defStyle)
        {
            Listener = new ColorSelectedListener(this); 
            TypedArray a = Context.Theme.ObtainStyledAttributes(
                    attrs, Resource.Styleable.ColorPickerPreference, defStyle, defStyle);

            try
            {
                mItemLayoutId = a.GetResourceId(Resource.Styleable.ColorPickerPreference_cal_itemLayout, mItemLayoutId);
                mNumColumns = a.GetInteger(Resource.Styleable.ColorPickerPreference_cal_numColumns, mNumColumns);
                int choicesResId = a.GetResourceId(Resource.Styleable.ColorPickerPreference_cal_choices,
                        Resource.Array.default_color_choice_values);
                if (choicesResId > 0)
                {
                    string[] choices = a.Resources.GetStringArray(choicesResId);
                    mColorChoices = new int[choices.Length];
                    for (int i = 0; i < choices.Length; i++)
                    {
                        mColorChoices[i] = Color.ParseColor(choices[i]);
                    }
                }

            }
            finally
            {
                a.Recycle();
            }
            WidgetLayoutResource = mItemLayoutId;
        }

        protected override void OnBindView(View view)
        {
            base.OnBindView(view);
            mPreviewView = view.FindViewById(Resource.Id.calendar_color_view);
            SetColorViewValue(mPreviewView, mValue);
        }
        public void SetValue(int value)
        {
            if (CallChangeListener(value))
            {
                mValue = value;
                PersistInt(value);
                NotifyChanged();
            }
        }
        public void SetIndex(int ind)
        {
            System.Diagnostics.Debug.WriteLine(ind);
            string[] color_array = this.Context.Resources.GetStringArray(Resource.Array.default_color_choice_values);
            var selected = color_array[ind];
            SetValue((int)Color.ParseColor(selected));
        }
        public int GetIndex()
        {
            string[] color_array = this.Context.Resources.GetStringArray(Resource.Array.default_color_choice_values);
            for (var i = 0; i < color_array.Length; i++)
            {
                if ((int)Color.ParseColor(color_array[i]) == mValue)
                    return i;
            }
            return -1;
        }

        protected override void OnClick()
        {
            base.OnClick();

            ColorPickerDialog colorcalendar = (ColorPickerDialog)ColorPickerDialog.NewInstance(Resource.String.color_picker_default_title,
                    mColorChoices, GetValue(), mNumColumns, Utils.IsTablet(Context) ? ColorPickerDialog.SIZE_LARGE : ColorPickerDialog.SIZE_SMALL);

            //colorcalendar.setPreference(this);

            Activity activity = (Activity)Context;
            activity.FragmentManager.BeginTransaction()
                    .Add(colorcalendar, GetFragmentTag())
                    .Commit();

            colorcalendar.SetOnColorSelectedListener(Listener);
        }
        
        ColorPickerSwatch.IOnColorSelectedListener Listener;
        private class ColorSelectedListener : ColorPickerSwatch.IOnColorSelectedListener
        {
            private ColorPickerPreference _parent;
            public ColorSelectedListener(ColorPickerPreference parent) => _parent = parent;
            public void OnColorSelected(int color)
            {
                _parent.SetValue(color);
            }
        }
        protected override void OnAttachedToActivity()
        {
            base.OnAttachedToActivity();

            Activity activity = (Activity)Context;
            ColorPickerDialog colorcalendar = (ColorPickerDialog)activity
                    .FragmentManager.FindFragmentByTag(GetFragmentTag());
            if (colorcalendar != null)
            {
                // re-bind listener to fragment
                colorcalendar.SetOnColorSelectedListener(Listener);
            }
        }
        
        protected override Java.Lang.Object OnGetDefaultValue(TypedArray a, int index)
        {
            return a.GetInt(index, 0);
        }
        protected override void OnSetInitialValue(bool restorePersistedValue, Java.Lang.Object defaultValue)
        {
            SetValue(restorePersistedValue ? GetPersistedInt(0) : (int)defaultValue);
        }

        public string GetFragmentTag()
        {
            return "color_" + Key;
        }

        public int GetValue()
        {
            return mValue;
        }

        private static void SetColorViewValue(View view, int color)
        {
            Color c = new Color(color);
            if (view is ImageView)
            {
                ImageView imageView = (ImageView)view;
                Resources res = imageView.Context.Resources;

                Drawable currentDrawable = imageView.Drawable;
                GradientDrawable colorChoiceDrawable;
                if (currentDrawable != null && currentDrawable is GradientDrawable)
                {
                    // Reuse drawable
                    colorChoiceDrawable = (GradientDrawable)currentDrawable;
                }
                else
                {
                    colorChoiceDrawable = new GradientDrawable();
                    colorChoiceDrawable.SetShape(ShapeType.Oval);
                }
                // Set stroke to dark version of color
                Color darkenedColor = Color.Rgb(
                        c.R * 192 / 256,
                        c.G * 192 / 256,
                        c.B * 192 / 256);

                colorChoiceDrawable.SetColor(color);
                colorChoiceDrawable.SetStroke((int)TypedValue.ApplyDimension(
                        ComplexUnitType.Dip, 1, res.DisplayMetrics), darkenedColor);
                imageView.SetImageDrawable(colorChoiceDrawable);

            }
            else if (view is TextView)
            {
                ((TextView)view).SetTextColor(c);
            }
        }
    }
}
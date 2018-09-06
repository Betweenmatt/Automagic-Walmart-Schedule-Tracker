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

namespace ColorPicker
{
    class ColorPickerDialog : DialogFragment, ColorPickerSwatch.IOnColorSelectedListener
    {
        public static int SIZE_LARGE = 1;
        public static int SIZE_SMALL = 2;

        protected AlertDialog mAlertDialog;

        protected static String KEY_TITLE_ID = "title_id";
        protected static String KEY_COLORS = "colors";
        protected static String KEY_SELECTED_COLOR = "selected_color";
        protected static String KEY_COLUMNS = "columns";
        protected static String KEY_SIZE = "size";

        protected int mTitleResId = Resource.String.color_picker_default_title;
        protected int[] mColors = null;
        protected int mSelectedColor;
        protected int mColumns;
        protected int mSize;

        private ColorPickerPalette mPalette;
        private ProgressBar mProgress;

        protected ColorPickerSwatch.IOnColorSelectedListener mListener;

        public ColorPickerDialog()
        {
            // Empty constructor required for dialog fragments.
        }

        public static ColorPickerDialog NewInstance(int titleResId, int[] colors, int selectedColor,
                int columns, int size)
        {
            ColorPickerDialog ret = new ColorPickerDialog();
            ret.Initialize(titleResId, colors, selectedColor, columns, size);
            return ret;
        }

        public void Initialize(int titleResId, int[] colors, int selectedColor, int columns, int size)
        {
            SetArguments(titleResId, columns, size);
            SetColors(colors, selectedColor);
        }

        public void SetArguments(int titleResId, int columns, int size)
        {
            Bundle bundle = new Bundle();
            bundle.PutInt(KEY_TITLE_ID, titleResId);
            bundle.PutInt(KEY_COLUMNS, columns);
            bundle.PutInt(KEY_SIZE, size);
            Arguments = (bundle);
        }

        public void SetOnColorSelectedListener(ColorPickerSwatch.IOnColorSelectedListener listener)
        {
            mListener = listener;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Arguments != null)
            {
                mTitleResId = Arguments.GetInt(KEY_TITLE_ID);
                mColumns = Arguments.GetInt(KEY_COLUMNS);
                mSize = Arguments.GetInt(KEY_SIZE);
            }

            if (savedInstanceState != null)
            {
                mColors = savedInstanceState.GetIntArray(KEY_COLORS);
                mSelectedColor = (int)savedInstanceState.GetInt(KEY_SELECTED_COLOR);
            }
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            Activity activity = Activity;

            View view = LayoutInflater.From(Activity).Inflate(Resource.Layout.calendar_color_picker_dialog, null);
            mProgress = (ProgressBar)view.FindViewById(Android.Resource.Id.Progress);
            mPalette = (ColorPickerPalette)view.FindViewById(Resource.Id.color_picker);
            mPalette.Init(mSize, mColumns, this);

            if (mColors != null)
            {
                ShowPaletteView();
            }

            mAlertDialog = new AlertDialog.Builder(activity)
                .SetTitle(mTitleResId)
                .SetView(view)
                .Create();

            return mAlertDialog;
        }

        public void OnColorSelected(int color)
        {
            if (mListener != null)
            {
                mListener.OnColorSelected(color);
            }

            if (TargetFragment is ColorPickerSwatch.IOnColorSelectedListener)
            {
                ColorPickerSwatch.IOnColorSelectedListener listener =
                        (ColorPickerSwatch.IOnColorSelectedListener)TargetFragment;
                listener.OnColorSelected(color);
            }

            if (color != mSelectedColor)
            {
                mSelectedColor = color;
                // Redraw palette to show checkmark on newly selected color before dismissing.
                mPalette.DrawPalette(mColors, mSelectedColor);
            }

            Dismiss();
        }

        public void ShowPaletteView()
        {
            if (mProgress != null && mPalette != null)
            {
                mProgress.Visibility = ViewStates.Gone;
                RefreshPalette();
                mPalette.Visibility = ViewStates.Visible;
            }
        }

        public void ShowProgressBarView()
        {
            if (mProgress != null && mPalette != null)
            {
                mProgress.Visibility = ViewStates.Visible;
                mPalette.Visibility = ViewStates.Gone;
            }
        }

        public void SetColors(int[] colors, int selectedColor)
        {
            if (mColors != colors || mSelectedColor != selectedColor)
            {
                mColors = colors;
                mSelectedColor = selectedColor;
                RefreshPalette();
            }
        }

        public void SetColors(int[] colors)
        {
            if (mColors != colors)
            {
                mColors = colors;
                RefreshPalette();
            }
        }

        public void SetSelectedColor(int color)
        {
            if (mSelectedColor != color)
            {
                mSelectedColor = color;
                RefreshPalette();
            }
        }

        private void RefreshPalette()
        {
            if (mPalette != null && mColors != null)
            {
                mPalette.DrawPalette(mColors, mSelectedColor);
            }
        }

        public int[] GetColors()
        {
            return mColors;
        }

        public int GetSelectedColor()
        {
            return mSelectedColor;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutIntArray(KEY_COLORS, mColors);
            outState.PutInt(KEY_SELECTED_COLOR, mSelectedColor);
        }
    }
}
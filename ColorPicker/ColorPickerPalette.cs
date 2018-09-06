using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace ColorPicker
{
    [Register("com.iammatt.colorpicker.colorPicker.ColorPickerPalette")]
    class ColorPickerPalette : TableLayout
    {
        public ColorPickerSwatch.IOnColorSelectedListener mOnColorSelectedListener;

        private string mDescription;
        private string mDescriptionSelected;

        private int mSwatchLength;
        private int mMarginSize;
        private int mNumColumns;

        public ColorPickerPalette(Context context, IAttributeSet attrs):base(context,attrs)
        {
        }

        public ColorPickerPalette(Context context):base(context)
        { 
        }

        /**
         * Initialize the size, columns, and listener.  Size should be a pre-defined size (SIZE_LARGE
         * or SIZE_SMALL) from ColorPickerDialogFragment.
         */
        public void Init(int size, int columns, ColorPickerSwatch.IOnColorSelectedListener listener)
        {
            mNumColumns = columns;
            Resources res = Resources;
            if (size == ColorPickerDialog.SIZE_LARGE)
            {
                mSwatchLength = res.GetDimensionPixelSize(Resource.Dimension.color_swatch_large);
                mMarginSize = res.GetDimensionPixelSize(Resource.Dimension.color_swatch_margins_large);
            }
            else
            {
                mSwatchLength = res.GetDimensionPixelSize(Resource.Dimension.color_swatch_small);
                mMarginSize = res.GetDimensionPixelSize(Resource.Dimension.color_swatch_margins_small);
            }
            mOnColorSelectedListener = listener;

            mDescription = res.GetString(Resource.String.color_swatch_description);
            mDescriptionSelected = res.GetString(Resource.String.color_swatch_description_selected);
        }

        private TableRow CreateTableRow()
        {
            TableRow row = new TableRow(Context);
            ViewGroup.LayoutParams p = new ViewGroup.LayoutParams(LayoutParams.WrapContent,
                    LayoutParams.WrapContent);
            row.LayoutParameters = (p);
            return row;
        }

        /**
         * Adds swatches to table in a serpentine format.
         */
        public void DrawPalette(int[] colors, int selectedColor)
        {
            if (colors == null)
            {
                return;
            }

            this.RemoveAllViews();
            int tableElements = 0;
            int rowElements = 0;
            int rowNumber = 0;

            // Fills the table with swatches based on the array of colors.
            TableRow row = CreateTableRow();
            foreach (int color in colors)
            {
                tableElements++;

                View colorSwatch = CreateColorSwatch(color, selectedColor);
                SetSwatchDescription(rowNumber, tableElements, rowElements, color == selectedColor,
                        colorSwatch);
                AddSwatchToRow(row, colorSwatch, rowNumber);

                rowElements++;
                if (rowElements == mNumColumns)
                {
                    AddView(row);
                    row = CreateTableRow();
                    rowElements = 0;
                    rowNumber++;
                }
            }

            // Create blank views to fill the row if the last row has not been filled.
            if (rowElements > 0)
            {
                while (rowElements != mNumColumns)
                {
                    AddSwatchToRow(row, CreateBlankSpace(), rowNumber);
                    rowElements++;
                }
                AddView(row);
            }
        }

        /**
         * Appends a swatch to the end of the row for even-numbered rows (starting with row 0),
         * to the beginning of a row for odd-numbered rows.
         */
        private void AddSwatchToRow(TableRow row, View swatch, int rowNumber)
        {
            if (rowNumber % 2 == 0)
            {
                row.AddView(swatch);
            }
            else
            {
                row.AddView(swatch, 0);
            }
        }

        /**
         * Add a content description to the specified swatch view. Because the colors get added in a
         * snaking form, every other row will need to compensate for the fact that the colors are added
         * in an opposite direction from their left->right/top->bottom order, which is how the system
         * will arrange them for accessibility purposes.
         */
        private void SetSwatchDescription(int rowNumber, int index, int rowElements, bool selected,
                View swatch)
        {
            int accessibilityIndex;
            if (rowNumber % 2 == 0)
            {
                // We're in a regular-ordered row
                accessibilityIndex = index;
            }
            else
            {
                // We're in a backwards-ordered row.
                int rowMax = ((rowNumber + 1) * mNumColumns);
                accessibilityIndex = rowMax - rowElements;
            }

            String description;
            if (selected)
            {
                description = string.Format(mDescriptionSelected, accessibilityIndex);
            }
            else
            {
                description = string.Format(mDescription, accessibilityIndex);
            }
            swatch.ContentDescription = (description);
        }

        /**
         * Creates a blank space to fill the row.
         */
        private ImageView CreateBlankSpace()
        {
            ImageView view = new ImageView(Context);
            TableRow.LayoutParams p = new TableRow.LayoutParams(mSwatchLength, mSwatchLength);
        p.SetMargins(mMarginSize, mMarginSize, mMarginSize, mMarginSize);
            view.LayoutParameters = (p);
            return view;
        }

        /**
         * Creates a color swatch.
         */
        private ColorPickerSwatch CreateColorSwatch(int color, int selectedColor)
        {
            ColorPickerSwatch view = new ColorPickerSwatch(Context, color,
                    color == selectedColor, mOnColorSelectedListener);
            TableRow.LayoutParams p = new TableRow.LayoutParams(mSwatchLength, mSwatchLength);
        p.SetMargins(mMarginSize, mMarginSize, mMarginSize, mMarginSize);
            view.LayoutParameters = (p);
            return view;
        }
    }
}
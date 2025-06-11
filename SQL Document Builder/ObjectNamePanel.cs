using System;
using System.Drawing;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The object name panel.
    /// </summary>
    internal class ObjectNamePanel : System.Windows.Forms.Panel
    {
        private ObjectName? _objectName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectNamePanel"/> class.
        /// </summary>
        public ObjectNamePanel()
        {
            DoubleBuffered = true;
        }

        /// <summary>
        /// Opens the database object.
        /// </summary>
        /// <param name="objectName">The object name.</param>
        public void Open(ObjectName? objectName)
        {
            _objectName = objectName;
            this.Invalidate();
        }

        /// <summary>
        /// Ons the paint.
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // clear the background
            e.Graphics.Clear(this.BackColor);

            if (_objectName == null)
            {
                return;
            }

            int wh = this.Height - 9;
            if (wh < 8)
            {
                wh = 8; // Ensure minimum size for image
            }

            // draw the object image
            Bitmap? image = GetImage();
            string objectNameText = _objectName.FullName ?? string.Empty;

            //using Font font = new("Microsoft Sans Serif", 11.25f, FontStyle.Regular);
            SizeF textSize = e.Graphics.MeasureString(objectNameText, this.Font);

            int imageWidth = wh;
            int imageHeight = wh;
            int spacing = 6; // space between image and text

            // Calculate total content height
            float contentHeight = Math.Max(imageHeight, textSize.Height);

            // Calculate top Y to center content
            float y = (this.Height - contentHeight) / 2f;

            // Draw image vertically centered
            if (image != null)
            {
                e.Graphics.DrawImage(image, 0, y, imageWidth, imageHeight);
            }

            // Draw text vertically centered
            if (!string.IsNullOrEmpty(objectNameText))
            {
                using Brush brush = new SolidBrush(this.ForeColor);
                float textY = y + (imageHeight - textSize.Height) / 2f;
                if (textSize.Height > imageHeight)
                    textY = y; // If text is taller, align to top of content area

                e.Graphics.DrawString(objectNameText, this.Font, brush, imageWidth + 3, textY);
            }
        }
        /// <summary>
        /// Clears the.
        /// </summary>
        internal void Clear()
        {
            _objectName = null;
            this.Invalidate();
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <returns>A Bitmap? .</returns>
        private Bitmap? GetImage()
        {
            return _objectName?.ObjectType switch
            {
                ObjectName.ObjectTypeEnums.Table => Properties.Resources.table_view,
                ObjectName.ObjectTypeEnums.View => Properties.Resources.view_16,
                ObjectName.ObjectTypeEnums.StoredProcedure => Properties.Resources.sp_16,
                ObjectName.ObjectTypeEnums.Function => Properties.Resources.function_16,
                ObjectName.ObjectTypeEnums.Trigger => Properties.Resources.dbtrigger24,
                ObjectName.ObjectTypeEnums.Synonym => Properties.Resources.table_view,
                _ => null
            };
        }
    }
}
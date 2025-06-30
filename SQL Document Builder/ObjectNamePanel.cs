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
        /// Copies the.
        /// </summary>
        public void Copy()
        {
            // Copy the object name to the clipboard
            if (_objectName != null)
            {
                string objectNameText = _objectName.FullName ?? string.Empty;
                Clipboard.SetText(objectNameText);
            }
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

            // Clear the background
            e.Graphics.Clear(this.BackColor);

            if (_objectName == null) { return; }

            // Prepare image and text
            Bitmap? image = GetImage();
            string objectNameText = _objectName.FullName ?? string.Empty;

            // Set image size to font height
            int imageHeight = (int)Math.Ceiling(this.Font.GetHeight(e.Graphics));
            if (imageHeight < 8) imageHeight = 8; // Minimum image size

            int imageWidth = imageHeight;
            int spacing = 3; // Space between image and text

            // Measure text
            SizeF textSize = e.Graphics.MeasureString(objectNameText, this.Font);

            // Calculate total block size (width and height)
            float totalHeight = Math.Max(imageHeight, textSize.Height);

            // Calculate top-left position to center the block
            float startY = (this.Height - totalHeight) / 2f;
            float startX = startY; // Padding from left

            // Draw image (vertically centered within the block)
            if (image != null)
            {
                float imageY = startY;
                e.Graphics.DrawImage(image, startX, imageY, imageWidth, imageHeight);
                startX += imageWidth + spacing;
            }

            // Draw text (vertically centered within the block)
            if (!string.IsNullOrEmpty(objectNameText))
            {
                using Brush brush = new SolidBrush(this.ForeColor);
                e.Graphics.DrawString(objectNameText, this.Font, brush, startX, startY);
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
                ObjectName.ObjectTypeEnums.Synonym => Properties.Resources.synonym_24,
                _ => null
            };
        }
    }
}
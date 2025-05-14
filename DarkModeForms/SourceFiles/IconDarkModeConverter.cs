using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkModeForms.SourceFiles
{
    /// <summary>
    /// The icon dark mode converter.
    /// </summary>
    internal static class IconDarkModeConverter
    {
        /// <summary>
        /// Converts an icon's colors for dark mode by inverting its colors.
        /// Preserves the alpha channel.
        /// </summary>
        /// <param name="originalIcon">The original Icon object.</param>
        /// <returns>A new Icon object with inverted colors, suitable for dark mode.</returns>
        public static Icon ConvertToDarkMode_Invert(Icon originalIcon)
        {
            if (originalIcon == null)
                throw new ArgumentNullException(nameof(originalIcon));

            Bitmap originalBitmap = originalIcon.ToBitmap();
            Bitmap invertedBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height, PixelFormat.Format32bppArgb);

            for (int y = 0; y < originalBitmap.Height; y++)
            {
                for (int x = 0; x < originalBitmap.Width; x++)
                {
                    Color originalColor = originalBitmap.GetPixel(x, y);
                    Color invertedColor = Color.FromArgb(
                        originalColor.A, // Preserve Alpha
                        255 - originalColor.R,
                        255 - originalColor.G,
                        255 - originalColor.B
                    );
                    invertedBitmap.SetPixel(x, y, invertedColor);
                }
            }
            // For some reason, Icon.FromHandle created directly from invertedBitmap.GetHicon()
            // can sometimes have transparency issues. Saving to a memory stream and reloading
            // can be a more robust way to create the new icon.
            using (MemoryStream ms = new MemoryStream())
            {
                invertedBitmap.Save(ms, ImageFormat.Png); // PNG supports transparency well
                ms.Seek(0, SeekOrigin.Begin);
                // Create a new bitmap from the stream to ensure it's a clean copy
                // before creating the icon, especially if the original icon was not 32bppArgb.
                using (Bitmap streamBitmap = new Bitmap(ms))
                {
                    // Getting Hicon and creating Icon from it.
                    // Be aware that GetHicon() can sometimes be problematic with transparency
                    // depending on the system's handling.
                    IntPtr hIcon = streamBitmap.GetHicon();
                    Icon newIcon = Icon.FromHandle(hIcon);
                    // It's important to destroy the GDI handle created by GetHicon
                    // See: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-destroyicon
                    DestroyIcon(hIcon); // Platform invoke for DestroyIcon
                    return newIcon;
                }
            }
        }

        /// <summary>
        /// Converts a Bitmap's colors for dark mode by inverting its colors.
        /// Preserves the alpha channel.
        /// </summary>
        /// <param name="originalBitmap">The original Bitmap object.</param>
        /// <returns>A new Bitmap object with inverted colors.</returns>
        public static Bitmap ConvertToDarkMode_Invert(Bitmap originalBitmap)
        {
            if (originalBitmap == null)
                throw new ArgumentNullException(nameof(originalBitmap));

            Bitmap invertedBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height, PixelFormat.Format32bppArgb);

            // Ensure we are working with a 32bppArgb bitmap for GetPixel/SetPixel with alpha
            if (originalBitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                Bitmap temp = new Bitmap(originalBitmap.Width, originalBitmap.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(temp))
                {
                    g.DrawImage(originalBitmap, new Rectangle(0, 0, temp.Width, temp.Height));
                }
                originalBitmap = temp; // Use the 32bppArgb version
            }


            for (int y = 0; y < originalBitmap.Height; y++)
            {
                for (int x = 0; x < originalBitmap.Width; x++)
                {
                    Color originalColor = originalBitmap.GetPixel(x, y);
                    Color invertedColor = Color.FromArgb(
                        originalColor.A, // Preserve Alpha
                        255 - originalColor.R,
                        255 - originalColor.G,
                        255 - originalColor.B
                    );
                    invertedBitmap.SetPixel(x, y, invertedColor);
                }
            }
            return invertedBitmap;
        }

        // Required for cleaning up GDI resources
        /// <summary>
        /// Destroys the icon.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns>A bool.</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);
    }
}



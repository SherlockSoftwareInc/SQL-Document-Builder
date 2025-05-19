using System;
using System.Windows.Forms;

namespace SQL_Document_Builder.ScintillaNetUtils
{
    /// <summary>
    /// The hot key manager.
    /// </summary>
    internal class HotKeyManager
    {
        public static bool Enable = true;

        /// <summary>
        /// Adds the hot key.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="function">The function to execute when the hotkey is pressed.</param>
        /// <param name="key">The key.</param>
        /// <param name="ctrl">If true, requires Ctrl.</param>
        /// <param name="shift">If true, requires Shift.</param>
        /// <param name="alt">If true, requires Alt.</param>
        public static void AddHotKey(Form form, Action function, Keys key, bool ctrl = false, bool shift = false, bool alt = false)
        {
            form.KeyPreview = true;

            // Use a local handler to avoid multiple subscriptions of the same handler
            KeyEventHandler? handler = null;
            handler = (object? sender, KeyEventArgs e) =>
            {
                if (!Enable)
                    return;

                if (IsHotkey(e, key, ctrl, shift, alt))
                {
                    e.Handled = true;
                    function();
                }
            };

            // Remove any previous identical handler to avoid stacking delegates
            form.KeyDown -= handler;
            form.KeyDown += handler;
        }

        /// <summary>
        /// Are the hotkey.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="key">The key.</param>
        /// <param name="ctrl">If true, ctrl.</param>
        /// <param name="shift">If true, shift.</param>
        /// <param name="alt">If true, alt.</param>
        /// <returns>A bool.</returns>
        public static bool IsHotkey(KeyEventArgs eventData, Keys key, bool ctrl = false, bool shift = false, bool alt = false)
        {
            return eventData.KeyCode == key && eventData.Control == ctrl && eventData.Shift == shift && eventData.Alt == alt;
        }
    }
}
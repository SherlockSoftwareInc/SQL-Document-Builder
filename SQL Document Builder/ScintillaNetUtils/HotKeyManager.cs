using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// <param name="function">The function.</param>
        /// <param name="key">The key.</param>
        /// <param name="ctrl">If true, ctrl.</param>
        /// <param name="shift">If true, shift.</param>
        /// <param name="alt">If true, alt.</param>
        public static void AddHotKey(Form form, Action function, Keys key, bool ctrl = false, bool shift = false, bool alt = false)
        {
            form.KeyPreview = true;

            form.KeyDown += delegate (object sender, KeyEventArgs e) {
                if (IsHotkey(e, key, ctrl, shift, alt))
                {
                    function();
                }
            };
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

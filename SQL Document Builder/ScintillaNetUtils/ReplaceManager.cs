using ScintillaNET;
using System.Windows.Forms;

namespace SQL_Document_Builder.ScintillaNetUtils
{
    /// <summary>
    /// The replace manager.
    /// </summary>
    internal class ReplaceManager
    {
        public static Scintilla TextArea;
        public static TextBox SearchBox;
        public static TextBox ReplaceBox;

        public static string LastSearch = "";
        public static int LastSearchIndex;

        /// <summary>
        /// Finds the next occurrence of the search text.
        /// </summary>
        /// <param name="next">If true, find next; otherwise, find previous.</param>
        /// <param name="incremental">If true, incremental search.</param>
        public static void Find(bool next, bool incremental)
        {
            bool first = LastSearch != SearchBox.Text;

            LastSearch = SearchBox.Text;
            if (LastSearch.Length > 0)
            {
                if (next)
                {
                    // SEARCH FOR THE NEXT OCCURRENCE
                    TextArea.TargetStart = LastSearchIndex - 1;
                    TextArea.TargetEnd = LastSearchIndex + (LastSearch.Length + 1);
                    TextArea.SearchFlags = SearchFlags.None;

                    if (!incremental || TextArea.SearchInTarget(LastSearch) == -1)
                    {
                        TextArea.TargetStart = TextArea.CurrentPosition;
                        TextArea.TargetEnd = TextArea.TextLength;
                        TextArea.SearchFlags = SearchFlags.None;

                        if (TextArea.SearchInTarget(LastSearch) == -1)
                        {
                            TextArea.TargetStart = 0;
                            TextArea.TargetEnd = TextArea.TextLength;

                            if (TextArea.SearchInTarget(LastSearch) == -1)
                            {
                                TextArea.ClearSelections();
                                return;
                            }
                        }
                    }
                }
                else
                {
                    // SEARCH FOR THE PREVIOUS OCCURRENCE
                    TextArea.TargetStart = 0;
                    TextArea.TargetEnd = TextArea.CurrentPosition;
                    TextArea.SearchFlags = SearchFlags.None;

                    if (TextArea.SearchInTarget(LastSearch) == -1)
                    {
                        TextArea.TargetStart = TextArea.CurrentPosition;
                        TextArea.TargetEnd = TextArea.TextLength;

                        if (TextArea.SearchInTarget(LastSearch) == -1)
                        {
                            TextArea.ClearSelections();
                            return;
                        }
                    }
                }

                LastSearchIndex = TextArea.TargetStart;
                TextArea.SetSelection(TextArea.TargetEnd, TextArea.TargetStart);
                TextArea.ScrollCaret();
            }

            SearchBox.Focus();
        }

        /// <summary>
        /// Replaces the current selection if it matches the search text.
        /// </summary>
        public static void Replace()
        {
            if (TextArea == null || string.IsNullOrEmpty(SearchBox.Text))
                return;

            if (TextArea.SelectedText == SearchBox.Text)
            {
                TextArea.ReplaceSelection(ReplaceBox.Text);
            }
            Find(true, false);
        }

        /// <summary>
        /// Replaces all occurrences of the search text with the replace text.
        /// </summary>
        public static void ReplaceAll()
        {
            if (TextArea == null || string.IsNullOrEmpty(SearchBox.Text))
                return;

            int count = 0;
            int pos = 0;
            TextArea.TargetStart = 0;
            TextArea.TargetEnd = TextArea.TextLength;
            TextArea.SearchFlags = SearchFlags.None;

            while (TextArea.SearchInTarget(SearchBox.Text) != -1)
            {
                TextArea.ReplaceTarget(ReplaceBox.Text);
                pos = TextArea.TargetEnd;
                TextArea.TargetStart = pos;
                TextArea.TargetEnd = TextArea.TextLength;
                count++;
            }
        }
    }
}
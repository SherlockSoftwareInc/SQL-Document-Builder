using ScintillaNET;
using System.Windows.Forms;

namespace SQL_Document_Builder.ScintillaNetUtils
{
    /// <summary>
    /// The search manager.
    /// </summary>
    internal class SearchManager
    {
        public static ScintillaNET.Scintilla TextArea;
        public static TextBox SearchBox;

        public static string LastSearch = "";

        public static int LastSearchIndex;

        /// <summary>
        /// Finds the.
        /// </summary>
        /// <param name="next">If true, next.</param>
        /// <param name="incremental">If true, incremental.</param>
        public static void Find(bool next, bool incremental)
        {
            bool first = LastSearch != SearchBox.Text;

            LastSearch = SearchBox.Text;
            if (LastSearch.Length > 0)
            {
                if (next)
                {
                    // SEARCH FOR THE NEXT OCCURRENCE

                    // Search the document at the last search index
                    TextArea.TargetStart = LastSearchIndex - 1;
                    TextArea.TargetEnd = LastSearchIndex + (LastSearch.Length + 1);
                    TextArea.SearchFlags = SearchFlags.None;

                    // Search, and if not found..
                    if (!incremental || TextArea.SearchInTarget(LastSearch) == -1)
                    {
                        // Search the document from the caret onwards
                        TextArea.TargetStart = TextArea.CurrentPosition;
                        TextArea.TargetEnd = TextArea.TextLength;
                        TextArea.SearchFlags = SearchFlags.None;

                        // Search, and if not found..
                        if (TextArea.SearchInTarget(LastSearch) == -1)
                        {
                            // Search again from top
                            TextArea.TargetStart = 0;
                            TextArea.TargetEnd = TextArea.TextLength;

                            // Search, and if not found..
                            if (TextArea.SearchInTarget(LastSearch) == -1)
                            {
                                // clear selection and exit
                                TextArea.ClearSelections();
                                return;
                            }
                        }
                    }
                }
                else
                {
                    // SEARCH FOR THE PREVIOUS OCCURRENCE

                    int lastFound = -1;
                    int searchStart = 0;
                    // Use the start of the selection if there is a selection, otherwise use the caret position
                    int caretPos = TextArea.SelectionStart;

                    while (searchStart < caretPos)
                    {
                        TextArea.TargetStart = searchStart;
                        TextArea.TargetEnd = caretPos;
                        TextArea.SearchFlags = SearchFlags.None;

                        int found = TextArea.SearchInTarget(LastSearch);
                        if (found == -1 || TextArea.TargetEnd > caretPos)
                        {
                            break;
                        }
                        lastFound = TextArea.TargetStart;
                        searchStart = TextArea.TargetStart + 1;
                    }

                    if (lastFound == -1)
                    {
                        // Wrap around: search from caret to end, then up to caret
                        searchStart = 0;
                        caretPos = TextArea.TextLength;
                        int wrapFound = -1;
                        while (searchStart < TextArea.SelectionStart)
                        {
                            TextArea.TargetStart = searchStart;
                            TextArea.TargetEnd = TextArea.SelectionStart;
                            TextArea.SearchFlags = SearchFlags.None;

                            int found = TextArea.SearchInTarget(LastSearch);
                            if (found == -1 || TextArea.TargetEnd > TextArea.SelectionStart)
                            {
                                break;
                            }
                            wrapFound = TextArea.TargetStart;
                            searchStart = TextArea.TargetStart + 1;
                        }
                        if (wrapFound == -1)
                        {
                            TextArea.ClearSelections();
                            return;
                        }
                        lastFound = wrapFound;
                    }

                    // Select the occurrence
                    TextArea.TargetStart = lastFound;
                    TextArea.TargetEnd = lastFound + LastSearch.Length;
                }
                // Select the occurrence
                LastSearchIndex = TextArea.TargetStart;
                TextArea.SetSelection(TextArea.TargetEnd, TextArea.TargetStart);
                TextArea.ScrollCaret();
            }

            SearchBox.Focus();
        }
    }
}
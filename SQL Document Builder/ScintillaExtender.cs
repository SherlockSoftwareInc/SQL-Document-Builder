using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Document_Builder
{
    /// <summary>
    /// ScintillaNET Editor extender for code intellisense display
    /// </summary>
    /// <remarks>(c) 2015 Daniel Medeiros</remarks>
    public static class ScintillaExtender
    {
        /// <summary>
        /// The sql key words.
        /// </summary>
        private const string SQL_KeyWords = "add alter as authorization backup begin bigint binary bit break browse bulk by cascade case catch check checkpoint close clustered column commit compute constraint containstable continue create current cursor cursor database date datetime datetime2 datetimeoffset dbcc deallocate decimal declare default delete deny desc disk distinct distributed double drop dump else end errlvl escape except exec execute exit external fetch file fillfactor float for foreign freetext freetexttable from full function goto grant group having hierarchyid holdlock identity identity_insert identitycol if image index insert int intersect into key kill lineno load merge money national nchar nocheck nocount nolock nonclustered ntext numeric nvarchar of off offsets on open opendatasource openquery openrowset openxml option order over percent plan precision primary print proc procedure public raiserror read readtext real reconfigure references replication restore restrict return revert revoke rollback rowcount rowguidcol rule save schema securityaudit select set setuser shutdown smalldatetime smallint smallmoney sql_variant statistics table table tablesample text textsize then time timestamp tinyint to top tran transaction trigger truncate try union unique uniqueidentifier update updatetext use user values varbinary varchar varying view waitfor when where while with writetext xml go ";

        // database columns
        private static readonly List<DBColumn> dbColumnList = new List<DBColumn>();


        private static string dbColumns = "";

        // database schema loaded flag
        private static bool dbSchemaLoaded = false;

        // database objects name for autocomplete
        private static string dbSchemas = "";
        private static string dbTables = "";
        private static string defaultSuggestion = "";

        /// <summary>
        /// Enables the code folding.
        /// </summary>
        public static void EnableCodeFolding(this Scintilla scintilla)
        {             // Enable code folding
            scintilla.SetProperty("fold", "1");
            scintilla.SetProperty("fold.compact", "1");

            scintilla.Margins[0].Width = 20;

            // Configure a margin to display folding symbols
            scintilla.Margins[1].Type = MarginType.Symbol;
            scintilla.Margins[1].Mask = Marker.MaskFolders;
            scintilla.Margins[1].Sensitive = true;
            scintilla.Margins[1].Width = 20;

            // Reset folder markers
            for (int i = Marker.FolderEnd; i <= Marker.FolderOpen; i++)
            {
                scintilla.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                scintilla.Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Configure folding markers with respective symbols
            scintilla.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            scintilla.Markers[Marker.Folder].SetBackColor(SystemColors.ControlText);
            scintilla.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            scintilla.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            scintilla.Markers[Marker.FolderEnd].SetBackColor(SystemColors.ControlText);
            scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintilla.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            scintilla.AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;
        }

        /// <summary>
        /// Gets the last word typed in the editor.
        /// </summary>
        /// <param name="scintilla"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetLastWord(this Scintilla scintilla)
        {
            string lastWord = "";
            int currentPos = scintilla.CurrentPosition;
            while (currentPos > 0)
            {
                char currentChar = (char)scintilla.GetCharAt(currentPos - 1);
                if (char.IsWhiteSpace(currentChar) || currentChar == '.' || currentChar == '(' || currentChar == ')' || currentChar == '\r' || currentChar == '\n' || currentChar == '\v' || currentChar == '[' || currentChar == ']')
                {
                    break;
                }
                lastWord = currentChar + lastWord;
                currentPos--;
            }
            return lastWord;
        }

        /// <summary>
        /// Sets the column margins to correctly show line numbers.
        /// </summary>
        /// <param name="scintilla"></param>
        /// <remarks></remarks>
        public static void SetColumnMargins(this Scintilla scintilla)
        {
            int maxLineNumberCharLength = scintilla.Lines.Count.ToString().Length;
            const int padding = 2;
            scintilla.Margins[0].Width = scintilla.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
        }

        /// <summary>
        /// Changes the font.
        /// </summary>
        /// <param name="scintilla">The scintilla.</param>
        /// <param name="fontname">The fontname.</param>
        /// <param name="fontsize">The fontsize.</param>
        public static void ChangeFont(this Scintilla scintilla, Font font)
        {
            if (scintilla != null)
            {
                for (int i = 0; i < scintilla.Styles.Count; i++)
                {
                    scintilla.Styles[i].Font = font.Name;
                    scintilla.Styles[i].SizeF = font.Size;
                }

                // Refresh the text to apply the new font size
                scintilla.Refresh();
            }
        }

        /// <summary>
        /// Matcheds the words.
        /// </summary>
        /// <param name="allWords">The all words.</param>
        /// <param name="searchFor">The search for.</param>
        /// <returns>A string.</returns>
        private static string MatchedWords(string allWords, string searchFor)
        {
            string results = "";

            if (allWords.Length == 0 || searchFor.Length == 0)
            {
                return results;
            }

            //find all words from the allWords that contains the searchFor
            string[] words = allWords.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Filter the words that contain the enteredString (case-insensitive)
            List<string> matchingWords = words
                .Where(word => word.IndexOf(searchFor, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (matchingWords.Count > 0)
            {
                foreach (string word in matchingWords)
                {
                    results += word + " ";
                }
            }

            return results;
        }
    }
}

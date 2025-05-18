using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            InitScintilla();
        }

        private void InitScintilla()
        {
            var sci = scintilla1; // Your Scintilla control

            // ... (existing setup code)

            // Code Folding (Example for SQL BEGIN/END blocks - may need refinement based on SQL dialect)
            sci.SetProperty("fold", "1");
            sci.SetProperty("fold.compact", "1");
            sci.SetProperty("fold.sql.at.else", "1");
            sci.SetProperty("fold.sql.only.begin", "1");

            sci.Margins[2].Type = MarginType.Symbol;
            sci.Margins[2].Mask = Marker.MaskFolders;
            sci.Margins[2].Sensitive = true;
            sci.Margins[2].Width = 60;

            // Set the folding margin background to sky blue
            sci.Margins[2].BackColor = Color.Red;

            // Define folder markers (adjust symbols and colors for dark theme)
            sci.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            sci.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            sci.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            sci.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;
            sci.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            sci.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            sci.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;

            for (int i = Marker.FolderEnd; i <= Marker.FolderOpen; i++)
            {
                sci.Markers[i].SetForeColor(sci.Styles[Style.Default].BackColor);
                sci.Markers[i].SetBackColor(Color.FromArgb(128, 128, 128));
            }

            sci.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

            // ... (rest of your setup code)

            sci.Refresh();
        }

        private void InitScintilla1()
        {
            var sci = scintilla1; // Your Scintilla control

            // 1. SET LEXER
            // Set the lexer to SQL first
            sci.LexerName = "sql";

            // 2. CLEAR ALL EXISTING STYLES
            // It's good practice to do this after setting the lexer and before defining new styles.
            sci.StyleClearAll();

            // 3. CONFIGURE DEFAULT STYLE
            // This style is used for all text if no other specific style is applied.
            sci.Styles[Style.Default].Font = "Consolas";
            sci.Styles[Style.Default].Size = 10;
            sci.Styles[Style.Default].BackColor = Color.FromArgb(30, 30, 30);
            sci.Styles[Style.Default].ForeColor = Color.FromArgb(220, 220, 220);
            // Apply this default style to all styles again, ensuring a consistent base
            // before applying specific lexer styles.
            sci.StyleClearAll(); // Yes, call it again here to make sure the default font/size applies everywhere initially.
                                 // Then re-apply the default colors.
            sci.Styles[Style.Default].BackColor = Color.FromArgb(30, 30, 30);
            sci.Styles[Style.Default].ForeColor = Color.FromArgb(220, 220, 220);


            // 4. CONFIGURE LINE NUMBER MARGIN
            sci.Styles[Style.LineNumber].BackColor = Color.FromArgb(40, 40, 40); // Slightly different from main background for visibility
            sci.Styles[Style.LineNumber].ForeColor = Color.FromArgb(43, 145, 175);
            sci.Margins[0].Type = MarginType.Number;
            sci.Margins[0].Width = 40;

            // 5. SET CARET AND SELECTION COLORS
            sci.CaretForeColor = Color.White;
            sci.SetSelectionBackColor(true, Color.FromArgb(51, 153, 255));
            sci.SetSelectionForeColor(true, Color.White);

            // 6. CONFIGURE SQL LEXER STYLES (for dark mode)
            // Comments
            sci.Styles[Style.Sql.Comment].ForeColor = Color.FromArgb(87, 166, 74); // Green
            sci.Styles[Style.Sql.CommentLine].ForeColor = Color.FromArgb(87, 166, 74); // Green
            sci.Styles[Style.Sql.CommentDoc].ForeColor = Color.FromArgb(87, 166, 74); // Green
            sci.Styles[Style.Sql.CommentLineDoc].ForeColor = Color.FromArgb(87, 166, 74); // Green
                                                                                          // Numbers
            sci.Styles[Style.Sql.Number].ForeColor = Color.FromArgb(181, 206, 168); // Light green/blue
                                                                                    // Strings and Characters
            sci.Styles[Style.Sql.String].ForeColor = Color.FromArgb(214, 157, 133); // Orange/brown
            sci.Styles[Style.Sql.Character].ForeColor = Color.FromArgb(214, 157, 133); // Orange/brown
            sci.Styles[Style.Sql.SqlPlus].ForeColor = Color.FromArgb(214, 157, 133); // For SQL*Plus prompts, can use string color
                                                                                     // SQL Keywords
            sci.Styles[Style.Sql.Word].ForeColor = Color.FromArgb(86, 156, 214); // Blue
                                                                                 // Secondary Keywords (Functions, Data Types, etc.)
            sci.Styles[Style.Sql.Word2].ForeColor = Color.FromArgb(78, 201, 176); // Teal/Cyan
                                                                                  // Operators
            sci.Styles[Style.Sql.Operator].ForeColor = Color.FromArgb(220, 220, 170); // Light Yellow
                                                                                      // Identifiers (table names, column names etc.) - can leave as default or style if desired
            sci.Styles[Style.Sql.Identifier].ForeColor = Color.FromArgb(212, 212, 212); // Off-white or a slightly different shade
                                                                                        // Quoted Identifiers (e.g., [column name] or "table name")
            sci.Styles[Style.Sql.QuotedIdentifier].ForeColor = Color.FromArgb(214, 157, 133); // Similar to strings

            // 7. SET SQL KEYWORDS
            // Primary keywords (adjust as needed for your SQL dialect)
            sci.SetKeywords(0, "select from where and or not in is null like between exists all any " +
                               "insert into values update set delete truncate create alter drop table view index procedure function trigger " +
                               "begin end commit rollback declare case when then else union group by order by having limit " +
                               "join inner left right outer on as distinct count avg sum min max cast convert " +
                               "go exec sp_ execute"); // Add GO, EXEC, sp_ for T-SQL like dialects

            // Secondary keywords (Data Types, Functions - adjust as needed)
            sci.SetKeywords(1, "int varchar nvarchar char text datetime date time smallint bigint bit decimal numeric float real " +
                               "primary key foreign references constraint unique default check " +
                               "getdate() current_timestamp system_user session_user user " +
                               "isnull coalesce nullif");


            // 8. OPTIONAL FEATURES

            // Indentation Guides
            sci.IndentationGuides = IndentView.LookBoth;
            sci.Styles[Style.IndentGuide].ForeColor = Color.FromArgb(60, 60, 60);
            sci.Styles[Style.IndentGuide].BackColor = sci.Styles[Style.Default].BackColor; // Match default background

            // Brace Matching (for parentheses, etc.)
            // Style for matching brace
            sci.Styles[Style.BraceLight].BackColor = Color.FromArgb(70, 70, 100);
            sci.Styles[Style.BraceLight].ForeColor = Color.White;
            sci.Styles[Style.BraceLight].Bold = true;
            // Style for non-matching brace
            sci.Styles[Style.BraceBad].BackColor = Color.FromArgb(150, 50, 50);
            sci.Styles[Style.BraceBad].ForeColor = Color.White;

            // You'll need to call sci.BraceMatch(); in an UpdateUI event handler:
            //sci.UpdateUI += (s, ev) =>
            //{
            //    var currentPos = sci.CurrentPosition;
            //    var bracePos1 = -1;
            //    var bracePos2 = -1;

            //    // Check for brace at current position
            //    if (currentPos > 0)
            //    {
            //        bracePos1 = sci.SafeGetCharAt(currentPos - 1);
            //        bracePos2 = sci.SafeGetCharAt(currentPos);

            //        if (sci.IsBrace(bracePos1))
            //            bracePos2 = sci.BraceMatch(currentPos - 1, 0);
            //        else if (sci.IsBrace(bracePos2))
            //            bracePos1 = sci.BraceMatch(currentPos, 0);
            //        else
            //        {
            //            bracePos1 = -1;
            //            bracePos2 = -1;
            //        }
            //    }

            //    sci.BraceHighlight(bracePos1, bracePos2);
            //};


            // Code Folding (Example for SQL BEGIN/END blocks - may need refinement based on SQL dialect)
            sci.SetProperty("fold", "1");
            sci.SetProperty("fold.compact", "1");
            sci.SetProperty("fold.sql.at.else", "1"); // Fold after ELSE
            sci.SetProperty("fold.sql.only.begin", "1"); // Crucial for BEGIN/END folding in T-SQL like dialects

            sci.Margins[2].Type = MarginType.Symbol;
            sci.Margins[2].Mask = Marker.MaskFolders;
            sci.Margins[2].Sensitive = true;
            sci.Margins[2].Width = 20;

            // Define folder markers (adjust symbols and colors for dark theme)
            sci.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            sci.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            sci.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            sci.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;
            sci.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            sci.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            sci.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;

            for (int i = Marker.FolderEnd; i <= Marker.FolderOpen; i++)
            {
                sci.Markers[i].SetForeColor(sci.Styles[Style.Default].BackColor); // Make lines match background
                sci.Markers[i].SetBackColor(Color.FromArgb(128, 128, 128));       // Symbol color (e.g., grey)
            }

            sci.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

            // Optional: Display whitespace
            // sci.ViewWhitespace = WhitespaceMode.VisibleAlways;
            // sci.SetWhitespaceForeColor(true, Color.FromArgb(50, 50, 50)); // Dark grey for whitespace dots/arrows

            // Optional: Edge line (to indicate max line length)
            // sci.EdgeColumn = 80;
            // sci.EdgeMode = EdgeMode.Line;
            // sci.EdgeColor = Color.FromArgb(70, 70, 70);

            // Optional: Long line indicator
            // sci.LongLineIndicator = LongLine.Strict;
            // sci.LongLinePen = new Pen(Color.Red); // Be careful with GDI objects, manage disposal if needed

            // Optional: Set zoom level
            // sci.Zoom = 0; // Default zoom

            // Ensure that the control is redrawn to apply changes
            sci.Refresh();
        }

        // Helper for Brace Matching in UpdateUI (if you use it)
        // Make sure your Scintilla control has an event handler for UpdateUI:
        // scintilla1.UpdateUI += scintilla1_UpdateUI;

        private void scintilla1_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            var sci = sender as Scintilla;
            if (sci == null) return;

            // Brace matching
            int currentPos = sci.CurrentPosition;
            int bracePos1 = -1;

            // Is there a brace to the left or right of the caret?
            if (currentPos > 0 && IsBrace(sci.GetCharAt(currentPos - 1)))
                bracePos1 = currentPos - 1;
            else if (IsBrace(sci.GetCharAt(currentPos)))
                bracePos1 = currentPos;

            if (bracePos1 != -1)
            {
                int bracePos2 = sci.BraceMatch(bracePos1);
                if (bracePos2 == Scintilla.InvalidPosition)
                {
                    sci.BraceBadLight(bracePos1);
                    sci.HighlightGuide = 0;
                }
                else
                {
                    sci.BraceHighlight(bracePos1, bracePos2);
                    sci.HighlightGuide = sci.GetColumn(bracePos1);
                }
            }
            else
            {
                // No brace nearby
                sci.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
                sci.HighlightGuide = 0;
            }
        }

        private bool IsBrace(int c)
        {
            switch (c)
            {
                case '(':
                case ')':
                case '[':
                case ']':
                case '{':
                case '}':
                case '<': // You might not want < > for SQL
                case '>':
                    return true;
            }
            return false;
        }

    }
}



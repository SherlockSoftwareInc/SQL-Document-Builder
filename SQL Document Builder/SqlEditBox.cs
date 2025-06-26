using ScintillaNET;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The sql edit box.
    /// </summary>
    internal class SqlEditBox : ScintillaNET.Scintilla
    {
        private DocumentTypeEnums _documentType = DocumentTypeEnums.empty;
        private bool _darkMode = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlEditBox"/> class.
        /// </summary>
        public SqlEditBox()
        {
            // INITIAL VIEW CONFIG
            this.WrapMode = WrapMode.None;
            this.IndentationGuides = IndentView.LookBoth;

            //// STYLING
            ////InitColors();
            //InitSyntaxColoringSQL();

            //// NUMBER MARGIN
            //InitNumberMargin();

            //// BOOKMARK MARGIN
            //InitBookmarkMargin();

            //// CODE FOLDING MARGIN
            //InitCodeFolding();

            // DRAG DROP
            InitDragDropFile();

            // DEFAULT FILE
            //LoadDataFromFile("../../MainForm.cs");

            // INIT HOTKEYS
            //InitHotkeys();
            // remove conflicting hotkeys from scintilla
            this.ClearCmdKey(Keys.Control | Keys.F);
            this.ClearCmdKey(Keys.Control | Keys.R);
            this.ClearCmdKey(Keys.Control | Keys.H);
            this.ClearCmdKey(Keys.Control | Keys.L);
            this.ClearCmdKey(Keys.Control | Keys.U);

            this.ClearCmdKey(Keys.Control | Keys.A);
            this.ClearCmdKey(Keys.Control | Keys.C);
            this.ClearCmdKey(Keys.Control | Keys.V);
            this.ClearCmdKey(Keys.Control | Keys.X);

            //this.ClearCmdKey(Keys.Control | Keys.Shift | Keys.L);
            //this.ClearCmdKey(Keys.Control | Keys.Shift | Keys.U);
        }

        public event EventHandler? FileNameChanged;

        public event EventHandler? QueryTextChanged;

        public event EventHandler? QueryTextFontChanged;

        public event EventHandler? QueryTextValidated;

        //public event EventHandler<QueryScriptGeneratedArgs> QueryScriptGenerated;

        /// <summary>
        /// The document type enums.
        /// </summary>
        public enum DocumentTypeEnums
        {
            empty,
            Sql,
            Text,
            Xml,
            Json,
            Html,
            Css,
            JavaScript,
            CSharp,
            VBScript,
            PowerShell,
            BatchFile,
            Markdown,
            Wiki
        }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Alias { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the data source name.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? DataSourceName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the default style font.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Drawing.Font DefaultStyleFont
        {
            get => this.Font;
            set
            {
                this.Font = value;

                for (int i = 0; i < this.Styles.Count; i++)
                {
                    this.Styles[i].Font = value.Name;
                    this.Styles[i].SizeF = value.Size;
                }

                // Refresh the text to apply the new font size
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the document type.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DocumentTypeEnums DocumentType
        {
            get => _documentType;
            set
            {
                if (_documentType != value)
                {
                    _documentType = value;
                    switch (_documentType)
                    {
                        case DocumentTypeEnums.Sql:
                            InitSyntaxColoringSQL();
                            break;

                        case DocumentTypeEnums.Html:
                        case DocumentTypeEnums.Xml:
                            InitSyntaxColoringHtml();
                            break;

                        case DocumentTypeEnums.Markdown:
                            InitSyntaxColoringMarkdown();
                            break;

                        case DocumentTypeEnums.Json:
                            InitSyntaxColoringJson();
                            break;

                        default:
                            InitSyntaxColoringDefault();
                            break;
                    }
                }
                // NUMBER MARGIN
                InitNumberMargin();

                // BOOKMARK MARGIN
                InitBookmarkMargin();

                // CODE FOLDING MARGIN
                InitCodeFolding();
            }
        }

        public void ChangeDocumentType(DocumentTypeEnums documentType)
        { 
        }

        /// <summary>
        /// File name of current query script
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Return file name without full path
        /// </summary>
        public string FileNameOnly
        {
            get
            {
                if (FileName.Length > 0)
                {
                    if (System.IO.File.Exists(FileName))
                        return System.IO.Path.GetFileName(FileName);
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the edit box id.
        /// </summary>
        public string ID { get; private set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Return a title for display as tab text and windows menu item
        /// When the length of the file name > 20, cut the 15 chars on the head and last 2 chars to build a short display name
        /// </summary>
        public string Title
        {
            get
            {
                if (Alias.Length > 0)
                {
                    return Alias.Length <= 20 ? Alias : string.Concat(Alias.AsSpan(0, 15), "...", Alias.AsSpan(Alias.Length - 2, 2));
                }

                if (FileName.Length > 0)
                {
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(FileName);
                    return fileName.Length <= 20 ? fileName : string.Concat(fileName.AsSpan(0, 15), "...", fileName.AsSpan(fileName.Length - 2, 2));
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether dark mode.
        /// </summary>
        public bool DarkMode
        {
            set
            {
                if (_darkMode != value)
                {
                    _darkMode = value;

                    // reset the Scintilla styles and colors
                    this.StyleResetDefault();
                    if (_darkMode)
                    {
                        this.BackColor = IntToColor(0x212121);
                        this.ForeColor = IntToColor(0xFFFFFF);
                    }
                    else
                    {
                        this.BackColor = Color.White;
                        this.ForeColor = Color.Black;
                    }

                    // STYLING
                    // Reinitialize colors and styles
                    InitColors();

                    // syntax coloring based on document type
                    switch (_documentType)
                    {
                        case DocumentTypeEnums.Sql:
                            InitSyntaxColoringSQL();
                            break;

                        case DocumentTypeEnums.Html:
                        case DocumentTypeEnums.Xml:
                            InitSyntaxColoringHtml();
                            break;

                        case DocumentTypeEnums.Markdown:
                            InitSyntaxColoringMarkdown();
                            break;

                        case DocumentTypeEnums.Json:
                            InitSyntaxColoringJson();
                            break;

                        default:
                            InitSyntaxColoringDefault();
                            break;
                    }

                    // NUMBER MARGIN
                    InitNumberMargin();

                    // BOOKMARK MARGIN
                    InitBookmarkMargin();

                    // CODE FOLDING MARGIN
                    InitCodeFolding();
                }
            }
        }

        /// Handles the text changed event of the SQL text box:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        internal void OnTextChanged(object? sender, EventArgs e)
        {
            //Changed = true;
            //statusToolStripStatusLabe.Text = string.Empty;

            SetColumnMargins();
        }

        /// <summary>
        /// Inits the colors.
        /// </summary>
        private void InitColors()
        {
            // Set selection background color using the new property
            this.SelectionBackColor = IntToColor(0x114D9C);
        }

        /// <summary>
        /// Inits the hotkeys.
        /// </summary>
        private void InitHotkeys()
        {
            // register the hotkeys with the form
            //HotKeyManager.AddHotKey(this, OpenSearch, Keys.F, true);
            //HotKeyManager.AddHotKey(this, OpenFindDialog, Keys.F, true, false, true);
            //HotKeyManager.AddHotKey(this, OpenReplaceDialog, Keys.R, true);
            //HotKeyManager.AddHotKey(this, OpenReplaceDialog, Keys.H, true);
            //HotKeyManager.AddHotKey(this, Uppercase, Keys.U, true);
            //HotKeyManager.AddHotKey(this, Lowercase, Keys.L, true);
            //HotKeyManager.AddHotKey(this, ZoomIn, Keys.Oemplus, true);
            //HotKeyManager.AddHotKey(this, ZoomOut, Keys.OemMinus, true);
            //HotKeyManager.AddHotKey(this, ZoomDefault, Keys.D0, true);
            //HotKeyManager.AddHotKey(this, CloseSearch, Keys.Escape);

            // remove conflicting hotkeys from scintilla
            this.ClearCmdKey(Keys.Control | Keys.F);
            this.ClearCmdKey(Keys.Control | Keys.R);
            this.ClearCmdKey(Keys.Control | Keys.H);
            this.ClearCmdKey(Keys.Control | Keys.L);
            this.ClearCmdKey(Keys.Control | Keys.U);
        }

        /// <summary>
        /// Initializes syntax coloring for default/plain text documents.
        /// </summary>
        private void InitSyntaxColoringDefault()
        {
            // Use the null lexer for plain text
            this.LexerName = "null";

            // Configure the default style
            this.StyleResetDefault();
            this.Styles[Style.Default].Font = "Cascadia Mono";
            this.Styles[Style.Default].Size = 10;
            if (_darkMode)
            {
                this.Styles[Style.Default].BackColor = IntToColor(0x212121);
                this.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);
                this.StyleClearAll();

                this.CaretForeColor = Color.White;
            }
            else
            {
                this.Styles[Style.Default].BackColor = Color.White;
                this.Styles[Style.Default].ForeColor = Color.Black;
                this.StyleClearAll();
                this.CaretForeColor = Color.Black;
            }
        }

        /// <summary>
        /// Initializes syntax coloring for HTML documents.
        /// </summary>
        private void InitSyntaxColoringHtml()
        {
            // Use the HTML lexer
            this.LexerName = "xml";

            // Configure the default style
            this.StyleResetDefault();
            this.Styles[Style.Default].Font = "Cascadia Mono";
            this.Styles[Style.Default].Size = 10;

            if (_darkMode)
            {
                this.Styles[Style.Default].BackColor = IntToColor(0x212121);
                this.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);
                this.StyleClearAll();

                // HTML styles
                this.Styles[Style.Html.Default].ForeColor = Color.White;
                this.Styles[Style.Html.Tag].ForeColor = Color.DeepSkyBlue;           // <tag>
                this.Styles[Style.Html.TagEnd].ForeColor = Color.DeepSkyBlue;        // </tag>
                this.Styles[Style.Html.TagUnknown].ForeColor = Color.Red;
                this.Styles[Style.Html.Attribute].ForeColor = Color.Orange;          // attribute=
                this.Styles[Style.Html.AttributeUnknown].ForeColor = Color.Red;
                this.Styles[Style.Html.Number].ForeColor = Color.LightGreen;
                this.Styles[Style.Html.DoubleString].ForeColor = Color.LightYellow;  // "string"
                this.Styles[Style.Html.SingleString].ForeColor = Color.LightYellow;  // 'string'
                this.Styles[Style.Html.Other].ForeColor = Color.LightGray;
                this.Styles[Style.Html.Comment].ForeColor = Color.MediumSeaGreen;    // <!-- comment -->
                this.Styles[Style.Html.Entity].ForeColor = Color.Violet;             // &entity;
                this.Styles[Style.Html.Value].ForeColor = Color.LightYellow;

                // Script/CSS blocks inside HTML
                this.Styles[Style.Html.Script].ForeColor = Color.LightPink;
                this.Styles[Style.Html.Asp].ForeColor = Color.LightSkyBlue;
                this.Styles[Style.Html.CData].ForeColor = Color.LightSlateGray;

                this.CaretForeColor = Color.White;
            }
            else
            {
                this.Styles[Style.Default].BackColor = Color.White;
                this.Styles[Style.Default].ForeColor = Color.Black;
                this.StyleClearAll();

                // HTML styles for light mode
                this.Styles[Style.Html.Default].ForeColor = Color.Black;
                this.Styles[Style.Html.Tag].ForeColor = Color.Blue;                  // <tag>
                this.Styles[Style.Html.TagEnd].ForeColor = Color.Blue;               // </tag>
                this.Styles[Style.Html.TagUnknown].ForeColor = Color.Red;
                this.Styles[Style.Html.Attribute].ForeColor = Color.Brown;           // attribute=
                this.Styles[Style.Html.AttributeUnknown].ForeColor = Color.Red;
                this.Styles[Style.Html.Number].ForeColor = Color.DarkCyan;
                this.Styles[Style.Html.DoubleString].ForeColor = Color.DarkGreen;    // "string"
                this.Styles[Style.Html.SingleString].ForeColor = Color.DarkGreen;    // 'string'
                this.Styles[Style.Html.Other].ForeColor = Color.Gray;
                this.Styles[Style.Html.Comment].ForeColor = Color.Green;             // <!-- comment -->
                this.Styles[Style.Html.Entity].ForeColor = Color.Purple;             // &entity;
                this.Styles[Style.Html.Value].ForeColor = Color.DarkGoldenrod;

                // Script/CSS blocks inside HTML
                this.Styles[Style.Html.Script].ForeColor = Color.MediumVioletRed;
                this.Styles[Style.Html.Asp].ForeColor = Color.MediumBlue;
                this.Styles[Style.Html.CData].ForeColor = Color.Gray;

                this.CaretForeColor = Color.Black;
            }
        }

        /// <summary>
        /// Inits the syntax coloring json.
        /// </summary>
        private void InitSyntaxColoringJson()
        {
            this.LexerName = "json";

            // Configure the default style
            this.StyleResetDefault();
            this.Styles[Style.Default].Font = "Cascadia Mono";
            this.Styles[Style.Default].Size = 10;

            if (_darkMode)
            {
                this.Styles[Style.Default].BackColor = IntToColor(0x212121);
                this.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);
                this.StyleClearAll();

                // JSON styles
                this.Styles[Style.Json.Default].ForeColor = Color.White;
                this.Styles[Style.Json.Number].ForeColor = Color.LightGreen;
                this.Styles[Style.Json.String].ForeColor = Color.LightYellow;
                this.Styles[Style.Json.PropertyName].ForeColor = Color.DeepSkyBlue;
                this.Styles[Style.Json.EscapeSequence].ForeColor = Color.Violet;
                this.Styles[Style.Json.Keyword].ForeColor = Color.DodgerBlue; // true, false, null
                this.Styles[Style.Json.LdKeyword].ForeColor = Color.Orange;   // JSON-LD keywords
                this.Styles[Style.Json.Operator].ForeColor = Color.LightGray; // : , { } [ ]
                this.Styles[Style.Json.BlockComment].ForeColor = Color.MediumSeaGreen;
                this.Styles[Style.Json.LineComment].ForeColor = Color.MediumSeaGreen;

                this.CaretForeColor = Color.White;
            }
            else
            {
                this.Styles[Style.Default].BackColor = Color.White;
                this.Styles[Style.Default].ForeColor = Color.Black;
                this.StyleClearAll();

                // JSON styles for light mode
                this.Styles[Style.Json.Default].ForeColor = Color.Black;
                this.Styles[Style.Json.Number].ForeColor = Color.DarkCyan;
                this.Styles[Style.Json.String].ForeColor = Color.Brown;
                this.Styles[Style.Json.PropertyName].ForeColor = Color.RoyalBlue;
                this.Styles[Style.Json.EscapeSequence].ForeColor = Color.Purple;
                this.Styles[Style.Json.Keyword].ForeColor = Color.Blue; // true, false, null
                this.Styles[Style.Json.LdKeyword].ForeColor = Color.DarkOrange;   // JSON-LD keywords
                this.Styles[Style.Json.Operator].ForeColor = Color.Gray; // : , { } [ ]
                this.Styles[Style.Json.BlockComment].ForeColor = Color.ForestGreen;
                this.Styles[Style.Json.LineComment].ForeColor = Color.ForestGreen;

                this.CaretForeColor = Color.Black;
            }
        }

        /// <summary>
        /// Initializes syntax coloring for Markdown documents.
        /// </summary>
        private void InitSyntaxColoringMarkdown()
        {
            this.LexerName = "markdown";

            // Configure the default style
            this.StyleResetDefault();
            this.Styles[Style.Default].Font = "Cascadia Mono";
            this.Styles[Style.Default].Size = 10;

            if (_darkMode)
            {
                this.Styles[Style.Default].BackColor = IntToColor(0x212121);
                this.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);
                this.StyleClearAll();

                // Markdown styles
                this.Styles[Style.Markdown.Default].ForeColor = Color.White;
                this.Styles[Style.Markdown.LineBegin].ForeColor = Color.Gray;
                this.Styles[Style.Markdown.Strong1].ForeColor = Color.Orange;      // Bold **
                this.Styles[Style.Markdown.Strong2].ForeColor = Color.Orange;      // Bold __
                this.Styles[Style.Markdown.Em1].ForeColor = Color.Gold;            // Italic *
                this.Styles[Style.Markdown.Em2].ForeColor = Color.Gold;            // Italic _
                this.Styles[Style.Markdown.Header1].ForeColor = Color.DeepSkyBlue;
                this.Styles[Style.Markdown.Header2].ForeColor = Color.DodgerBlue;
                this.Styles[Style.Markdown.Header3].ForeColor = Color.LightSkyBlue;
                this.Styles[Style.Markdown.Header4].ForeColor = Color.LightBlue;
                this.Styles[Style.Markdown.Header5].ForeColor = Color.LightSteelBlue;
                this.Styles[Style.Markdown.Header6].ForeColor = Color.SkyBlue;
                this.Styles[Style.Markdown.PreChar].ForeColor = Color.LightGreen;  // Code block marker (`)
                this.Styles[Style.Markdown.UListItem].ForeColor = Color.LightSalmon;
                this.Styles[Style.Markdown.OListItem].ForeColor = Color.LightSalmon;
                this.Styles[Style.Markdown.BlockQuote].ForeColor = Color.MediumSeaGreen;
                this.Styles[Style.Markdown.Strikeout].ForeColor = Color.LightGray;
                this.Styles[Style.Markdown.HRule].ForeColor = Color.DarkGray;
                this.Styles[Style.Markdown.Link].ForeColor = Color.Violet;
                this.Styles[Style.Markdown.Code].ForeColor = Color.LightGreen;
                this.Styles[Style.Markdown.Code2].ForeColor = Color.LightGreen;
                this.Styles[Style.Markdown.CodeBk].BackColor = Color.FromArgb(40, 60, 40);

                this.CaretForeColor = Color.White;
            }
            else
            {
                this.Styles[Style.Default].BackColor = Color.White;
                this.Styles[Style.Default].ForeColor = Color.Black;
                this.StyleClearAll();

                // Markdown styles for light mode
                this.Styles[Style.Markdown.Default].ForeColor = Color.Black;
                this.Styles[Style.Markdown.LineBegin].ForeColor = Color.DarkGray;
                this.Styles[Style.Markdown.Strong1].ForeColor = Color.DarkOrange;      // Bold **
                this.Styles[Style.Markdown.Strong2].ForeColor = Color.DarkOrange;      // Bold __
                this.Styles[Style.Markdown.Em1].ForeColor = Color.Goldenrod;           // Italic *
                this.Styles[Style.Markdown.Em2].ForeColor = Color.Goldenrod;           // Italic _
                this.Styles[Style.Markdown.Header1].ForeColor = Color.RoyalBlue;
                this.Styles[Style.Markdown.Header2].ForeColor = Color.MediumBlue;
                this.Styles[Style.Markdown.Header3].ForeColor = Color.SteelBlue;
                this.Styles[Style.Markdown.Header4].ForeColor = Color.CadetBlue;
                this.Styles[Style.Markdown.Header5].ForeColor = Color.SlateGray;
                this.Styles[Style.Markdown.Header6].ForeColor = Color.Gray;
                this.Styles[Style.Markdown.PreChar].ForeColor = Color.ForestGreen;     // Code block marker (`)
                this.Styles[Style.Markdown.UListItem].ForeColor = Color.OrangeRed;
                this.Styles[Style.Markdown.OListItem].ForeColor = Color.OrangeRed;
                this.Styles[Style.Markdown.BlockQuote].ForeColor = Color.SeaGreen;
                this.Styles[Style.Markdown.Strikeout].ForeColor = Color.Gray;
                this.Styles[Style.Markdown.HRule].ForeColor = Color.DarkGray;
                this.Styles[Style.Markdown.Link].ForeColor = Color.Purple;
                this.Styles[Style.Markdown.Code].ForeColor = Color.ForestGreen;
                this.Styles[Style.Markdown.Code2].ForeColor = Color.ForestGreen;
                this.Styles[Style.Markdown.CodeBk].BackColor = Color.FromArgb(230, 230, 210);

                this.CaretForeColor = Color.Black;
            }
        }

        /// <summary>
        /// Inits the syntax coloring.
        /// </summary>
        private void InitSyntaxColoringSQL()
        {
            this.LexerName = "sql";

            // Configure the default style
            this.StyleResetDefault();
            this.Styles[Style.Default].Font = "Cascadia Mono";   // "Consolas";
            this.Styles[Style.Default].Size = 10;

            if (_darkMode)
            {
                this.Styles[Style.Default].BackColor = IntToColor(0x212121);
                this.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);
                this.StyleClearAll();

                // Configure the SQL lexer styles
                this.Styles[Style.Sql.Comment].ForeColor = Color.RosyBrown; // IntToColor(0xBD758B);
                this.Styles[Style.Sql.CommentLine].ForeColor = Color.MediumSeaGreen; // IntToColor(0x40BF57);
                this.Styles[Style.Sql.CommentDoc].ForeColor = Color.SeaGreen; // IntToColor(0x2FAE35);
                this.Styles[Style.Sql.Number].ForeColor = Color.Yellow; // IntToColor(0xFFFF00);
                this.Styles[Style.Sql.String].ForeColor = Color.Yellow; // IntToColor(0xFFFF00);
                this.Styles[Style.Sql.Character].ForeColor = Color.IndianRed; // IntToColor(0xE95454);
                this.Styles[Style.Sql.Operator].ForeColor = Color.LightGray; // IntToColor(0xE0E0E0);
                this.Styles[Style.Sql.Identifier].ForeColor = Color.LightSteelBlue; // IntToColor(0xD0DAE2);
                this.Styles[Style.Sql.CommentLineDoc].ForeColor = Color.CornflowerBlue; // IntToColor(0x77A7DB);
                this.Styles[Style.Sql.Word].ForeColor = Color.DodgerBlue; // IntToColor(0x48A8EE);
                this.Styles[Style.Sql.Word2].ForeColor = Color.DarkOrange; // IntToColor(0xF98906);
                this.Styles[Style.Sql.CommentDocKeyword].ForeColor = Color.DarkSeaGreen; // IntToColor(0xB3D991);
                this.Styles[Style.Sql.CommentDocKeywordError].ForeColor = Color.Red; // IntToColor(0xFF0000);
                this.Styles[Style.Sql.SqlPlus].ForeColor = Color.LightGray; // IntToColor(0xD3D3D3);
                this.Styles[Style.Sql.SqlPlusPrompt].ForeColor = Color.LightGreen;  // IntToColor(0x90EE90);
                this.Styles[Style.Sql.SqlPlusComment].ForeColor = Color.DarkGray;   // IntToColor(0xA9A9A9);
                this.Styles[Style.Sql.User1].ForeColor = Color.LightBlue;   // IntToColor(0xADD8E6);
                this.Styles[Style.Sql.User2].ForeColor = Color.LightPink;   // IntToColor(0xFFB6C1);
                this.Styles[Style.Sql.User3].ForeColor = Color.Orange;  // IntToColor(0xFFA500);
                this.Styles[Style.Sql.User4].ForeColor = Color.Cyan;    // IntToColor(0x00FFFF);
                this.Styles[Style.Sql.QuotedIdentifier].ForeColor = Color.LightYellow;  // IntToColor(0xFFFF00);
                this.Styles[Style.Sql.QOperator].ForeColor = Color.Magenta; // IntToColor(0xFF00FF);

                this.CaretForeColor = Color.White;
            }
            else
            {
                this.Styles[Style.Default].BackColor = Color.White;
                this.Styles[Style.Default].ForeColor = Color.Black;
                this.StyleClearAll();

                // Configure the SQL lexer styles for light mode
                this.Styles[Style.Sql.Comment].ForeColor = Color.Green;
                this.Styles[Style.Sql.CommentLine].ForeColor = Color.Green;
                this.Styles[Style.Sql.CommentDoc].ForeColor = Color.ForestGreen;
                this.Styles[Style.Sql.Number].ForeColor = Color.DarkCyan;
                this.Styles[Style.Sql.String].ForeColor = Color.Brown;
                this.Styles[Style.Sql.Character].ForeColor = Color.DarkRed;
                this.Styles[Style.Sql.Operator].ForeColor = Color.DarkBlue;
                this.Styles[Style.Sql.Identifier].ForeColor = Color.Black;
                this.Styles[Style.Sql.CommentLineDoc].ForeColor = Color.Teal;
                this.Styles[Style.Sql.Word].ForeColor = Color.Blue;
                this.Styles[Style.Sql.Word2].ForeColor = Color.DarkMagenta;
                this.Styles[Style.Sql.CommentDocKeyword].ForeColor = Color.OliveDrab;
                this.Styles[Style.Sql.CommentDocKeywordError].ForeColor = Color.Red;
                this.Styles[Style.Sql.SqlPlus].ForeColor = Color.Gray;
                this.Styles[Style.Sql.SqlPlusPrompt].ForeColor = Color.DarkGreen;
                this.Styles[Style.Sql.SqlPlusComment].ForeColor = Color.Gray;
                this.Styles[Style.Sql.User1].ForeColor = Color.MediumBlue;
                this.Styles[Style.Sql.User2].ForeColor = Color.MediumVioletRed;
                this.Styles[Style.Sql.User3].ForeColor = Color.DarkOrange;
                this.Styles[Style.Sql.User4].ForeColor = Color.DarkCyan;
                this.Styles[Style.Sql.QuotedIdentifier].ForeColor = Color.DarkGoldenrod;
                this.Styles[Style.Sql.QOperator].ForeColor = Color.Purple;

                this.CaretForeColor = Color.Black;
            }

            const string SQL_KeyWords = "add alter as authorization backup begin break browse bulk by cascade case catch check checkpoint close clustered column commit compute constraint containstable continue create current cursor database dbcc deallocate declare default delete deny desc disk distinct distributed double drop dump else end errlvl escape except exec execute exit external fetch file fillfactor for foreign freetext freetexttable from full function go goto grant group having holdlock identity identity_insert identitycol if index insert intersect into key kill lineno load merge national nocheck nocount nolock nonclustered of off offsets on open opendatasource openquery openrowset openxml option order over percent plan precision primary print proc procedure public raiserror read readtext reconfigure references replication restore restrict return revert revoke rollback rowcount rowguidcol rule save schema securityaudit select set setuser shutdown statistics table tablesample textsize then to top tran transaction trigger truncate try union unique update updatetext use user values varying view waitfor when where while with writetext ";

            const string SQL_KeyWords1 = "bigint binary bit char date datetime datetime2 datetimeoffset decimal float geography geometry hierarchyid image int money nchar ntext numeric nvarchar real smalldatetime smallint smallmoney sql_variant text time timestamp tinyint uniqueidentifier varbinary varchar xml ";

            const string SQL_KeyWords2 = "@@version abs ascii avg cast ceiling char charindex coalesce concat convert count current_timestamp current_user datalength dateadd datediff datename datepart day floor getdate getutcdate isdate isnull isnumeric lag lead left len lower ltrim max min month nchar nullif patindex rand replace right round rtrim session_user sessionproperty sign space str stuff substring sum system_user try_cast try_convert upper user_name year ";

            this.SetKeywords(0, SQL_KeyWords + SQL_KeyWords1); // Add the primary keywords
            this.SetKeywords(1, SQL_KeyWords2); // Add the data types as secondary keywords

            // User1 = 4
            this.SetKeywords(4, "all and any between cross exists in inner is join left like not null or outer pivot right some unpivot ( ) * ");
            // User2 = 5
            this.SetKeywords(5, "sys objects sysobjects ");
        }

        /// <summary>
        /// Sets the column margins.
        /// </summary>
        private void SetColumnMargins()
        {
            int maxLineNumber = this.Lines.Count.ToString().Length;
            //int numberWidth = TextRenderer.MeasureText(maxLineNumber.ToString(), this.Font).Width;
            this.Margins[NUMBER_MARGIN].Width = this.TextWidth(Style.LineNumber, new string('9', maxLineNumber + 1)) + 5;
        }

        #region Numbers, Bookmarks, Code Folding

        /// <summary>
        /// the background color of the text area
        /// </summary>
        private const int BACK_COLOR = 0x2A211C;

        /// <summary>
        /// change this to whatever margin you want the bookmarks/breakpoints to show in
        /// </summary>
        private const int BOOKMARK_MARGIN = 2;

        /// <summary>
        /// The bookmark marker.
        /// </summary>
        private const int BOOKMARK_MARKER = 2;

        /// <summary>
        /// set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
        /// </summary>
        private const bool CODEFOLDING_CIRCULAR = true;

        /// <summary>
        /// change this to whatever margin you want the code folding tree (+/-) to show in
        /// </summary>
        private const int FOLDING_MARGIN = 3;

        /// <summary>
        /// default text color of the text area
        /// </summary>
        private const int FORE_COLOR = 0xB7B7B7;

        /// <summary>
        /// change this to whatever margin you want the line numbers to show in
        /// </summary>
        private const int NUMBER_MARGIN = 1;

        /// <summary>
        /// Inits the bookmark margin.
        /// </summary>
        private void InitBookmarkMargin()
        {
            //scintilla1.SetFoldMarginColor(true, IntToColor(BACK_COLOR));

            var margin = this.Margins[BOOKMARK_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = (1 << BOOKMARK_MARKER);
            //margin.Cursor = MarginCursor.Arrow;

            var marker = this.Markers[BOOKMARK_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            if(_darkMode)
            {
                //marker.SetBackColor(IntToColor(0xFF003B));
                //marker.SetForeColor(IntToColor(0xFFFFFF));
                marker.SetBackColor(IntToColor(BACK_COLOR));
                marker.SetForeColor(IntToColor(0xFFFFFF));
            }
            else
            {
                marker.SetBackColor(Color.Red);
                marker.SetForeColor(Color.White);
            }
            marker.SetAlpha(100);
        }

        /// <summary>
        /// Inits the code folding.
        /// </summary>
        private void InitCodeFolding()
        {
            // Set margin background for folding
            if (_darkMode)
            {
                this.SetFoldMarginColor(true, IntToColor(BACK_COLOR));
                this.SetFoldMarginHighlightColor(true, IntToColor(BACK_COLOR));
            }
            else
            {
                this.SetFoldMarginColor(true, Color.White);
                this.SetFoldMarginHighlightColor(true, Color.White);
            }

            // Enable code folding
            this.SetProperty("fold", "1");
            this.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            var foldingMargin = this.Margins[FOLDING_MARGIN];
            foldingMargin.Type = MarginType.Symbol;
            foldingMargin.Mask = Marker.MaskFolders;
            foldingMargin.Sensitive = true;
            foldingMargin.Width = 20;

            // Set colors and symbols for all folding markers
            if (_darkMode)
            {
                for (int i = 25; i <= 31; i++)
                {
                    this.Markers[i].SetForeColor(IntToColor(BACK_COLOR)); // Foreground for [+] and [-]
                    this.Markers[i].SetBackColor(IntToColor(FORE_COLOR)); // Background for [+] and [-]
                }
            }
            else
            {
                for (int i = 25; i <= 31; i++)
                {
                    this.Markers[i].SetForeColor(Color.White); // Foreground for [+] and [-]
                    this.Markers[i].SetBackColor(Color.Gray);  // Background for [+] and [-]
                }
            }

            // Configure folding markers with respective symbols
            this.Markers[Marker.Folder].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
            this.Markers[Marker.FolderOpen].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
            this.Markers[Marker.FolderEnd].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
            this.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            this.Markers[Marker.FolderOpenMid].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
            this.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            this.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            this.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }
        /// <summary>
        /// Inits the number margin.
        /// </summary>
        private void InitNumberMargin()
        {
            if(_darkMode)
            {
                this.Styles[Style.LineNumber].BackColor = IntToColor(BACK_COLOR);
                this.Styles[Style.LineNumber].ForeColor = IntToColor(FORE_COLOR);
                this.Styles[Style.IndentGuide].ForeColor = IntToColor(FORE_COLOR);
                this.Styles[Style.IndentGuide].BackColor = IntToColor(BACK_COLOR);
            }
            else
            {
                this.Styles[Style.LineNumber].BackColor = Color.White;
                this.Styles[Style.LineNumber].ForeColor = Color.Gray;
                this.Styles[Style.IndentGuide].ForeColor = Color.LightGray;
                this.Styles[Style.IndentGuide].BackColor = Color.White;
            }

            var nums = this.Margins[NUMBER_MARGIN];
            nums.Width = 20;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            this.MarginClick += Scintilla1_MarginClick;
        }

        /// <summary>
        /// Handles the scintilla1 margin click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Scintilla1_MarginClick(object sender, MarginClickEventArgs e)
        {
            if (e.Margin == BOOKMARK_MARGIN)
            {
                // Do we have a marker for this line?
                const uint mask = (1 << BOOKMARK_MARKER);
                var line = this.Lines[this.LineFromPosition(e.Position)];
                if ((line.MarkerGet() & mask) > 0)
                {
                    // Remove existing bookmark
                    line.MarkerDelete(BOOKMARK_MARKER);
                }
                else
                {
                    // Add bookmark
                    line.MarkerAdd(BOOKMARK_MARKER);
                }
            }
        }

        #endregion Numbers, Bookmarks, Code Folding

        #region Drag & Drop File

        /// <summary>
        /// Inits the drag drop file.
        /// </summary>
        public void InitDragDropFile()
        {
            this.AllowDrop = true;
            this.DragEnter += (sender, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files != null && files.Length > 0)
                    {
                        string ext = Path.GetExtension(files[0]).ToLowerInvariant();
                        if (ext == ".sql" || ext == ".txt")
                            e.Effect = DragDropEffects.Copy;
                        else
                            e.Effect = DragDropEffects.None;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            };

            this.DragDrop += (sender, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files != null && files.Length > 0)
                    {
                        string path = files[0];
                        string ext = Path.GetExtension(path).ToLowerInvariant();
                        if (ext == ".sql" || ext == ".txt")
                        {
                            try
                            {
                                if (System.IO.File.Exists(path))
                                {
                                    //_fileName = path;
                                    //SetTitle(_fileName);

                                    //this.Text = File.ReadAllText(_fileName);
                                    //_changed = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Failed to load file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Only .sql and .txt files are supported.", "Unsupported File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            };
        }

        #endregion Drag & Drop File

        #region Utils

        /// <summary>
        /// Ints the to color.
        /// </summary>
        /// <param name="rgb">The rgb.</param>
        /// <returns>A Color.</returns>
        public static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        /// <summary>
        /// Invokes the if needed.
        /// </summary>
        /// <param name="action">The action.</param>
        public void InvokeIfNeeded(Action action)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Opens the file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        internal void OpenFile(string fileName)
        {
            // check if file exists
            if (System.IO.File.Exists(fileName))
            {
                FileName = fileName;
                this.Text = System.IO.File.ReadAllText(FileName);
                this.EmptyUndoBuffer(); // Clear undo history so undo can't go back to blank
                this.SetSavePoint();    // Mark the current state as clean

                // using file extension to set the document type
                string ext = System.IO.Path.GetExtension(FileName).ToLowerInvariant();
                switch (ext)
                {
                    case ".sql":
                        DocumentType = DocumentTypeEnums.Sql;
                        break;

                    case ".txt":
                        DocumentType = DocumentTypeEnums.Text;
                        break;

                    case ".xml":
                        DocumentType = DocumentTypeEnums.Xml;
                        break;

                    case ".json":
                        DocumentType = DocumentTypeEnums.Json;
                        break;

                    case ".html":
                    case ".htm":
                        DocumentType = DocumentTypeEnums.Html;
                        break;

                    case ".css":
                        DocumentType = DocumentTypeEnums.Css;
                        break;

                    case ".js":
                        DocumentType = DocumentTypeEnums.JavaScript;
                        break;

                    case ".cs":
                        DocumentType = DocumentTypeEnums.CSharp;
                        break;

                    case ".vb":
                        DocumentType = DocumentTypeEnums.VBScript;
                        break;

                    case ".ps1":
                        DocumentType = DocumentTypeEnums.PowerShell;
                        break;

                    case ".bat":
                    case ".cmd":
                        DocumentType = DocumentTypeEnums.BatchFile;
                        break;

                    case ".md":
                    case ".markdown":
                        DocumentType = DocumentTypeEnums.Markdown;
                        break;
                }

                FileNameChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Saves the current file.
        /// </summary>
        internal DialogResult Save()
        {
            try
            {
                if (FileName.Length > 0)
                {
                    System.IO.File.WriteAllTextAsync(FileName, this.Text);
                    this.SetSavePoint();
                }
                else
                {
                    return SaveAs();
                }
            }
            catch (Exception ex)
            {
                // show error message if the file cannot be saved
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }
            return DialogResult.OK;
        }

        /// <summary>
        /// Saves the as.
        /// </summary>
        internal DialogResult SaveAs()
        {
            DialogResult result = DialogResult.None;

            try
            {
                var oFile = new SaveFileDialog
                {
                    // set the filter based on the document type
                    Filter = DocumentType switch
                    {
                        DocumentTypeEnums.Sql => "SQL files(*.sql)|*.sql|Text files(*.txt)|*.txt|All files(*.*)|*.*",
                        DocumentTypeEnums.Text => "Text files(*.txt)|*.txt|All files(*.*)|*.*",
                        DocumentTypeEnums.Markdown => "Markdown files(*.md)|*.md|All files(*.*)|*.*",
                        DocumentTypeEnums.Xml => "XML files(*.xml)|*.xml|All files(*.*)|*.*",
                        DocumentTypeEnums.Json => "JSON files(*.json)|*.json|All files(*.*)|*.*",
                        DocumentTypeEnums.Html => "HTML files(*.html)|*.html|All files(*.*)|*.*",
                        DocumentTypeEnums.Css => "CSS files(*.css)|*.css|All files(*.*)|*.*",
                        DocumentTypeEnums.JavaScript => "JavaScript files(*.js)|*.js|All files(*.*)|*.*",
                        DocumentTypeEnums.CSharp => "C# files(*.cs)|*.cs|All files(*.*)|*.*",
                        DocumentTypeEnums.VBScript => "VBScript files(*.vb)|*.vb|All files(*.*)|*.*",
                        DocumentTypeEnums.PowerShell => "PowerShell files(*.ps1)|*.ps1|All files(*.*)|*.*",
                        DocumentTypeEnums.BatchFile => "Batch files(*.bat)|*.bat|All files(*.*)|*.*",
                        _ => "All files(*.*)|*.*",
                    }
                };
                result = oFile.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (FileName != oFile.FileName)
                    {
                        FileName = oFile.FileName;
                        FileNameChanged?.Invoke(this, EventArgs.Empty);
                    }

                    System.IO.File.WriteAllText(FileName, Text);
                    this.SetSavePoint();
                }
            }
            catch (Exception ex)
            {
                // show error message if the file cannot be saved
                Common.MsgBox(ex.Message, MessageBoxIcon.Error);
            }

            return result;
        }

        /// <summary>
        /// Checks whether the current query text needs to be saved
        /// </summary>
        /// <returns></returns>
        internal DialogResult SaveCheck()
        {
            if (Modified)
            {
                switch (MessageBox.Show("Do you want to save the changes?", "SQL Document Builder", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        Save();
                        return DialogResult.Yes;

                    case DialogResult.Cancel:
                        return DialogResult.Cancel;

                    default:
                        return DialogResult.No;
                }
            }
            return DialogResult.No;
        }

        #endregion Utils
    }
}
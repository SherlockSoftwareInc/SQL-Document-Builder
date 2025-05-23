using ScintillaNET;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>
    /// The sql edit box.
    /// </summary>
    internal class SqlEditBox : ScintillaNET.Scintilla
    {
        /// <summary>
        /// The sql key words 1.
        /// </summary>
        private const string SQL_KeyWords = "add alter as authorization backup begin bigint binary bit break browse bulk by cascade case catch check checkpoint close clustered column commit compute constraint containstable continue create current cursor cursor database date datetime datetime2 datetimeoffset dbcc deallocate decimal declare default delete deny desc disk distinct distributed double drop dump else end errlvl escape except exec execute exit external fetch file fillfactor float for foreign freetext freetexttable from full function goto grant group having hierarchyid holdlock identity identity_insert identitycol if image index insert int intersect into key kill lineno load merge money national nchar nocheck nocount nolock nonclustered ntext numeric nvarchar of off offsets on open opendatasource openquery openrowset openxml option order over percent plan precision primary print proc procedure public raiserror read readtext real reconfigure references replication restore restrict return revert revoke rollback rowcount rowguidcol rule save schema securityaudit select set setuser shutdown smalldatetime smallint smallmoney sql_variant statistics table table tablesample text textsize then time timestamp tinyint to top tran transaction trigger truncate try union unique uniqueidentifier update updatetext use user values varbinary varchar varying view waitfor when where while with writetext xml go ";

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlEditBox"/> class.
        /// </summary>
        public SqlEditBox()
        {
            // INITIAL VIEW CONFIG
            this.WrapMode = WrapMode.None;
            this.IndentationGuides = IndentView.LookBoth;

            // STYLING
            InitColors();
            InitSyntaxColoring();

            // NUMBER MARGIN
            InitNumberMargin();

            // BOOKMARK MARGIN
            InitBookmarkMargin();

            // CODE FOLDING MARGIN
            InitCodeFolding();

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
            Markdown
        }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        public string Alias { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the default style font.
        /// </summary>
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
        public DocumentTypeEnums DocumentType { get; set; } = DocumentTypeEnums.empty;

        /// <summary>
        /// File name of current query script
        /// </summary>
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
        /// Gets or sets a value indicating whether text changed.
        /// </summary>
        internal bool Changed { get; set; } = false;

        /// <summary>
        /// Handles the text changed event of the SQL text box:
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        internal void OnTextChanged(object sender, EventArgs e)
        {
            Changed = true;
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
        /// Inits the syntax coloring.
        /// </summary>
        private void InitSyntaxColoring()
        {
            //this.Lexer = Lexer.Sql;
            this.LexerName = "sql";

            // Configure the default style
            this.StyleResetDefault();
            this.Styles[Style.Default].Font = "Cascadia Mono";   // "Consolas";
            this.Styles[Style.Default].Size = 10;
            this.Styles[Style.Default].BackColor = IntToColor(0x212121);
            this.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);
            this.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            //this.Styles[Style.Cpp.Identifier].ForeColor = IntToColor(0xD0DAE2);
            //this.Styles[Style.Cpp.Comment].ForeColor = IntToColor(0xBD758B);
            //this.Styles[Style.Cpp.CommentLine].ForeColor = IntToColor(0x40BF57);
            //this.Styles[Style.Cpp.CommentDoc].ForeColor = IntToColor(0x2FAE35);
            //this.Styles[Style.Cpp.Number].ForeColor = IntToColor(0xFFFF00);
            //this.Styles[Style.Cpp.String].ForeColor = IntToColor(0xFFFF00);
            //this.Styles[Style.Cpp.Character].ForeColor = IntToColor(0xE95454);
            //this.Styles[Style.Cpp.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
            //this.Styles[Style.Cpp.Operator].ForeColor = IntToColor(0xE0E0E0);
            //this.Styles[Style.Cpp.Regex].ForeColor = IntToColor(0xff00ff);
            //this.Styles[Style.Cpp.CommentLineDoc].ForeColor = IntToColor(0x77A7DB);
            //this.Styles[Style.Cpp.Word].ForeColor = IntToColor(0x48A8EE);
            //this.Styles[Style.Cpp.Word2].ForeColor = IntToColor(0xF98906);
            //this.Styles[Style.Cpp.CommentDocKeyword].ForeColor = IntToColor(0xB3D991);
            //this.Styles[Style.Cpp.CommentDocKeywordError].ForeColor = IntToColor(0xFF0000);
            //this.Styles[Style.Cpp.GlobalClass].ForeColor = IntToColor(0x48A8EE);

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
            this.Styles[Style.Sql.SqlPlus].ForeColor = Color.LightGray;	// IntToColor(0xD3D3D3);
            this.Styles[Style.Sql.SqlPlusPrompt].ForeColor = Color.LightGreen;	// IntToColor(0x90EE90);
            this.Styles[Style.Sql.SqlPlusComment].ForeColor = Color.DarkGray;	// IntToColor(0xA9A9A9);
            this.Styles[Style.Sql.User1].ForeColor = Color.LightBlue;	// IntToColor(0xADD8E6);
            this.Styles[Style.Sql.User2].ForeColor = Color.LightPink;	// IntToColor(0xFFB6C1);
            this.Styles[Style.Sql.User3].ForeColor = Color.Orange;	// IntToColor(0xFFA500);
            this.Styles[Style.Sql.User4].ForeColor = Color.Cyan;	// IntToColor(0x00FFFF);
            this.Styles[Style.Sql.QuotedIdentifier].ForeColor = Color.LightYellow;	// IntToColor(0xFFFF00);
            this.Styles[Style.Sql.QOperator].ForeColor = Color.Magenta; // IntToColor(0xFF00FF);

            this.CaretForeColor = Color.White;

            //this.SetKeywords(0, "select from where and or not in is null like between exists all any " +
            //       "insert into values update set delete truncate create alter drop table view index procedure function trigger " +
            //       "begin end commit rollback declare case when then else union group by order by having limit " +
            //       "join inner left right outer on as distinct count avg sum min max cast convert " +
            //       "go exec sp_ execute"); // Add GO, EXEC, sp_ for T-SQL like dialects

            this.SetKeywords(0, SQL_KeyWords); // Add your custom keywords here

            // Secondary keywords (Data Types, Functions - adjust as needed)
            //this.SetKeywords(1, "int varchar nvarchar char text datetime date time smallint bigint bit decimal numeric float real " +
            //                   "primary key foreign references constraint unique default check " +
            //                   "getdate() current_timestamp system_user session_user user " +
            //                   "isnull coalesce nullif");

            // Word2 = 1
            this.SetKeywords(1, "ascii cast char charindex ceiling coalesce collate contains convert current_date current_time current_timestamp current_user floor isnull max min nullif object_id session_user substring system_user tsequal ");
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
            marker.SetBackColor(IntToColor(0xFF003B));
            marker.SetForeColor(IntToColor(0x000000));
            marker.SetAlpha(100);
        }

        /// <summary>
        /// Inits the code folding.
        /// </summary>
        private void InitCodeFolding()
        {
            this.SetFoldMarginColor(true, IntToColor(BACK_COLOR));
            this.SetFoldMarginHighlightColor(true, IntToColor(BACK_COLOR));

            // Enable code folding
            this.SetProperty("fold", "1");
            this.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            this.Margins[FOLDING_MARGIN].Type = MarginType.Symbol;
            this.Margins[FOLDING_MARGIN].Mask = Marker.MaskFolders;
            this.Margins[FOLDING_MARGIN].Sensitive = true;
            this.Margins[FOLDING_MARGIN].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                this.Markers[i].SetForeColor(IntToColor(BACK_COLOR)); // styles for [+] and [-]
                this.Markers[i].SetBackColor(IntToColor(FORE_COLOR)); // styles for [+] and [-]
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
            this.Styles[Style.LineNumber].BackColor = IntToColor(BACK_COLOR);
            this.Styles[Style.LineNumber].ForeColor = IntToColor(FORE_COLOR);
            this.Styles[Style.IndentGuide].ForeColor = IntToColor(FORE_COLOR);
            this.Styles[Style.IndentGuide].BackColor = IntToColor(BACK_COLOR);

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
                this.Changed = false;

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

            //_fileName = oFile.FileName;
            //SetTitle(_fileName);

            //CurrentEditBox.Text = File.ReadAllText(_fileName);
            //CurrentEditBox.Changed = false;
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
                    Changed = false;
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
                    Changed = false;
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
            if (Changed)
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
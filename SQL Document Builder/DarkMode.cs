using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    /// <summary>This tries to automatically apply Windows Dark Mode (if enabled) to a Form.
    /// <para>Author: BlueMystic (bluemystic.play@gmail.com)  2024</para></summary>
    public class DarkMode
    {
        #region Win32 API Declarations

        public struct DWMCOLORIZATIONcolors
        {
            public uint ColorizationColor,
                ColorizationAfterglow,
                ColorizationColorBalance,
                ColorizationAfterglowBalance,
                ColorizationBlurBalance,
                ColorizationGlassReflectionIntensity,
                ColorizationOpaqueBlend;
        }

        /// <summary>
        /// The DWMWINDOWATTRIBUTE.
        /// </summary>
        [Flags]
        public enum DWMWINDOWATTRIBUTE : uint
        {
            /// <summary>
            /// Use with DwmGetWindowAttribute. Discovers whether non-client rendering is enabled. The retrieved value is of type BOOL. TRUE if non-client rendering is enabled; otherwise, FALSE.
            /// </summary>
            DWMWA_NCRENDERING_ENABLED = 1,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Sets the non-client rendering policy. The pvAttribute parameter points to a value from the DWMNCRENDERINGPOLICY enumeration.
            /// </summary>
            DWMWA_NCRENDERING_POLICY,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Enables or forcibly disables DWM transitions. The pvAttribute parameter points to a value of type BOOL. TRUE to disable transitions, or FALSE to enable transitions.
            /// </summary>
            DWMWA_TRANSITIONS_FORCEDISABLED,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Enables content rendered in the non-client area to be visible on the frame drawn by DWM. The pvAttribute parameter points to a value of type BOOL. TRUE to enable content rendered in the non-client area to be visible on the frame; otherwise, FALSE.
            /// </summary>
            DWMWA_ALLOW_NCPAINT,

            /// <summary>
            /// Use with DwmGetWindowAttribute. Retrieves the bounds of the caption button area in the window-relative space. The retrieved value is of type RECT. If the window is minimized or otherwise not visible to the user, then the value of the RECT retrieved is undefined. You should check whether the retrieved RECT contains a boundary that you can work with, and if it doesn't then you can conclude that the window is minimized or otherwise not visible.
            /// </summary>
            DWMWA_CAPTION_BUTTON_BOUNDS,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Specifies whether non-client content is right-to-left (RTL) mirrored. The pvAttribute parameter points to a value of type BOOL. TRUE if the non-client content is right-to-left (RTL) mirrored; otherwise, FALSE.
            /// </summary>
            DWMWA_NONCLIENT_RTL_LAYOUT,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Forces the window to display an iconic thumbnail or peek representation (a static bitmap), even if a live or snapshot representation of the window is available. This value is normally set during a window's creation, and not changed throughout the window's lifetime. Some scenarios, however, might require the value to change over time. The pvAttribute parameter points to a value of type BOOL. TRUE to require a iconic thumbnail or peek representation; otherwise, FALSE.
            /// </summary>
            DWMWA_FORCE_ICONIC_REPRESENTATION,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Sets how Flip3D treats the window. The pvAttribute parameter points to a value from the DWMFLIP3DWINDOWPOLICY enumeration.
            /// </summary>
            DWMWA_FLIP3D_POLICY,

            /// <summary>
            /// Use with DwmGetWindowAttribute. Retrieves the extended frame bounds rectangle in screen space. The retrieved value is of type RECT.
            /// </summary>
            DWMWA_EXTENDED_FRAME_BOUNDS,

            /// <summary>
            /// Use with DwmSetWindowAttribute. The window will provide a bitmap for use by DWM as an iconic thumbnail or peek representation (a static bitmap) for the window. DWMWA_HAS_ICONIC_BITMAP can be specified with DWMWA_FORCE_ICONIC_REPRESENTATION. DWMWA_HAS_ICONIC_BITMAP normally is set during a window's creation and not changed throughout the window's lifetime. Some scenarios, however, might require the value to change over time. The pvAttribute parameter points to a value of type BOOL. TRUE to inform DWM that the window will provide an iconic thumbnail or peek representation; otherwise, FALSE. Windows Vista and earlier: This value is not supported.
            /// </summary>
            DWMWA_HAS_ICONIC_BITMAP,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Do not show peek preview for the window. The peek view shows a full-sized preview of the window when the mouse hovers over the window's thumbnail in the taskbar. If this attribute is set, hovering the mouse pointer over the window's thumbnail dismisses peek (in case another window in the group has a peek preview showing). The pvAttribute parameter points to a value of type BOOL. TRUE to prevent peek functionality, or FALSE to allow it. Windows Vista and earlier: This value is not supported.
            /// </summary>
            DWMWA_DISALLOW_PEEK,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Prevents a window from fading to a glass sheet when peek is invoked. The pvAttribute parameter points to a value of type BOOL. TRUE to prevent the window from fading during another window's peek, or FALSE for normal behavior. Windows Vista and earlier: This value is not supported.
            /// </summary>
            DWMWA_EXCLUDED_FROM_PEEK,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Cloaks the window such that it is not visible to the user. The window is still composed by DWM. Using with DirectComposition: Use the DWMWA_CLOAK flag to cloak the layered child window when animating a representation of the window's content via a DirectComposition visual that has been associated with the layered child window. For more details on this usage case, see How to animate the bitmap of a layered child window. Windows 7 and earlier: This value is not supported.
            /// </summary>
            DWMWA_CLOAK,

            /// <summary>
            /// Use with DwmGetWindowAttribute. If the window is cloaked, provides one of the following values explaining why. DWM_CLOAKED_APP (value 0x0000001). The window was cloaked by its owner application. DWM_CLOAKED_SHELL(value 0x0000002). The window was cloaked by the Shell. DWM_CLOAKED_INHERITED(value 0x0000004). The cloak value was inherited from its owner window. Windows 7 and earlier: This value is not supported.
            /// </summary>
            DWMWA_CLOAKED,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Freeze the window's thumbnail image with its current visuals. Do no further live updates on the thumbnail image to match the window's contents. Windows 7 and earlier: This value is not supported.
            /// </summary>
            DWMWA_FREEZE_REPRESENTATION,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Enables a non-UWP window to use host backdrop brushes. If this flag is set, then a Win32 app that calls Windows::UI::Composition APIs can build transparency effects using the host backdrop brush (see Compositor.CreateHostBackdropBrush). The pvAttribute parameter points to a value of type BOOL. TRUE to enable host backdrop brushes for the window, or FALSE to disable it. This value is supported starting with Windows 11 Build 22000.
            /// </summary>
            DWMWA_USE_HOSTBACKDROPBRUSH,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Allows the window frame for this window to be drawn in dark mode colors when the dark mode system setting is enabled. For compatibility reasons, all windows default to light mode regardless of the system setting. The pvAttribute parameter points to a value of type BOOL. TRUE to honor dark mode for the window, FALSE to always use light mode. This value is supported starting with Windows 10 Build 17763.
            /// </summary>
            DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Allows the window frame for this window to be drawn in dark mode colors when the dark mode system setting is enabled. For compatibility reasons, all windows default to light mode regardless of the system setting. The pvAttribute parameter points to a value of type BOOL. TRUE to honor dark mode for the window, FALSE to always use light mode. This value is supported starting with Windows 11 Build 22000.
            /// </summary>
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Specifies the rounded corner preference for a window. The pvAttribute parameter points to a value of type DWM_WINDOW_CORNER_PREFERENCE. This value is supported starting with Windows 11 Build 22000.
            /// </summary>
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Specifies the color of the window border. The pvAttribute parameter points to a value of type COLORREF. The app is responsible for changing the border color according to state changes, such as a change in window activation. This value is supported starting with Windows 11 Build 22000.
            /// </summary>
            DWMWA_BORDER_COLOR,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Specifies the color of the caption. The pvAttribute parameter points to a value of type COLORREF. This value is supported starting with Windows 11 Build 22000.
            /// </summary>
            DWMWA_CAPTION_COLOR,

            /// <summary>
            /// Use with DwmSetWindowAttribute. Specifies the color of the caption text. The pvAttribute parameter points to a value of type COLORREF. This value is supported starting with Windows 11 Build 22000.
            /// </summary>
            DWMWA_TEXT_COLOR,

            /// <summary>
            /// Use with DwmGetWindowAttribute. Retrieves the width of the outer border that the DWM would draw around this window. The value can vary depending on the DPI of the window. The pvAttribute parameter points to a value of type UINT. This value is supported starting with Windows 11 Build 22000.
            /// </summary>
            DWMWA_VISIBLE_FRAME_BORDER_THICKNESS,

            /// <summary>
            /// The maximum recognized DWMWINDOWATTRIBUTE value, used for validation purposes.
            /// </summary>
            DWMWA_LAST,
        }

        /// <summary>
        /// The DWM_WINDOW_CORNER_PREFERENCE.
        /// </summary>
        [Flags]
        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            /// <summary>
            /// Tos the rectangle.
            /// </summary>
            /// <returns>A Rectangle.</returns>
            public Rectangle ToRectangle()
            {
                return Rectangle.FromLTRB(Left, Top, Right, Bottom);
            }
        }

        /// <summary>
        /// The EM_SETCUEBANNER const.
        /// </summary>
        public const int EM_SETCUEBANNER = 5377;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("DwmApi")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string? pszSubIdList);

        [DllImport("dwmapi.dll", EntryPoint = "#127")]
        public static extern void DwmGetColorizationParameters(ref DWMCOLORIZATIONcolors colors);

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        [DllImport("user32")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32")]
        private static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr hdc);

        public static IntPtr GetHeaderControl(ListView list)
        {
            const int LVM_GETHEADER = 0x1000 + 31;
            return SendMessage(list.Handle, LVM_GETHEADER, IntPtr.Zero, "");
        }

        // Required for cleaning up GDI resources
        /// <summary>
        /// Destroys the icon.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns>A bool.</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        #endregion Win32 API Declarations

        #region Static Local Members

        /// <summary>
        /// prevents applying a theme multiple times to the same control
        /// without this, it happens at least is some MDI forms
        /// currently, only Key is being used, the Value is not.
        /// Using ConditionalWeakTable because I found no suitable ISet<Control> implementation
        /// </summary>
        private static ConditionalWeakTable<Control, object> ControlsProcessed = new ConditionalWeakTable<Control, object>();

        #endregion Static Local Members

        #region Public Static Members

        /// <summary>
        /// setting to false (ideally before any Form is created) disables DarkMode
        /// </summary>
        public static bool IsDarkModeEnabled { get; set; } = true;

        #endregion Public Static Members

        #region Public Members

        /// <summary>'true' if Dark Mode Color is set in Windows's Settings.</summary>
        public bool IsDarkMode { get; set; } = false;

        /// <summary>Option to re-colorize all Icons in Toolbars and Menus.</summary>
        public bool ColorizeIcons { get; set; } = true;

        /// <summary>Option to make all Panels Borders Rounded</summary>
        public bool RoundedPanels { get; set; } = false;

        /// <summary>The PArent form for them all.</summary>
        public Form OwnerForm { get; set; }

        /// <summary>Windows Colors. Can be customized.</summary>
        public OSThemeColors OScolors { get; set; }

        #endregion Public Members

        #region Constructors

        /// <summary>This tries to automatically apply Windows Dark Mode (if enabled) to a Form.</summary>
        /// <param name="_Form">The Form to become Dark</param>
        /// <param name="_ColorizeIcons">[OPTIONAL] re-colorize all Icons in Toolbars and Menus.</param>
        /// <param name="_RoundedPanels">[OPTIONAL] make all Panels Borders Rounded</param>
        public DarkMode(Form _Form, bool _ColorizeIcons = true, bool _RoundedPanels = false)
        {
            //Sets the Properties:
            OwnerForm = _Form;
            ColorizeIcons = _ColorizeIcons;
            RoundedPanels = _RoundedPanels;
            //IsDarkMode = IsDarkModeEnabled && GetWindowsColorMode() <= 0 ? true : false;
            IsDarkMode = IsDarkModeEnabled ? true : false;

            //if (!IsDarkModeEnabled) return;

            ApplyTheme(IsDarkMode);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>Apply the Theme into the Window and all its controls.</summary>
        /// <param name="pIsDarkMode">'true': apply Dark Mode, 'false': apply Clear Mode</param>
        public void ApplyTheme(bool pIsDarkMode = true)
        {
            try
            {
                IsDarkMode = pIsDarkMode;
                OScolors = GetSystemColors(OwnerForm, pIsDarkMode ? 0 : 1);
                ControlsProcessed = new ConditionalWeakTable<Control, object>();

                if (OScolors != null)
                {
                    //Apply Window's Dark Mode to the Form's Title bar:
                    ApplySystemDarkTheme(OwnerForm, pIsDarkMode);

                    OwnerForm.BackColor = OScolors.Background;
                    OwnerForm.ForeColor = OScolors.TextInactive;

                    if (OwnerForm != null && OwnerForm.Controls != null)
                    {
                        foreach (Control _control in OwnerForm.Controls)
                        {
                            ThemeControl(_control);
                        }
                        OwnerForm.ControlAdded += (object sender, ControlEventArgs e) =>
                        {
                            ThemeControl(e.Control);
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Recursively apply the Colors from 'OScolors' to the Control and all its childs.</summary>
        /// <param name="control">Can be a Form or any Winforms Control.</param>
        public void ThemeControl(Control control)
        {
            //prevent applying a theme multiple times to the same control
            //without this, it happens at least is some MDI forms
            if (ControlsProcessed.TryGetValue(control, out _)) return;
            ControlsProcessed.Add(control, null);

            BorderStyle BStyle = (IsDarkMode ? BorderStyle.FixedSingle : BorderStyle.Fixed3D);
            FlatStyle FStyle = (IsDarkMode ? FlatStyle.Flat : FlatStyle.Standard);

            control.HandleCreated += (object sender, EventArgs e) =>
            {
                ApplySystemDarkTheme(control, IsDarkMode);
            };
            control.ControlAdded += (object sender, ControlEventArgs e) =>
            {
                ThemeControl(e.Control);
            };

            string Mode = IsDarkMode ? "DarkMode_Explorer" : "ClearMode_Explorer";
            SetWindowTheme(control.Handle, Mode, null);

            control.GetType().GetProperty("BackColor")?.SetValue(control, OScolors.Control);
            control.GetType().GetProperty("ForeColor")?.SetValue(control, OScolors.TextActive);

            if (control is Label label)
            {
                if(label.Parent != null)
                {
                    label.BackColor = label.Parent.BackColor;
                }
                label.BorderStyle = BorderStyle.None;
            }
            else if (control is LinkLabel linkLabel)
            {
                linkLabel.LinkColor = OScolors.AccentLight;
                linkLabel.VisitedLinkColor = OScolors.Primary;
            }
            else if (control is TextBox textBox)
            {
                textBox.BorderStyle = BStyle;
            }
            else if (control is Button button)
            {
                SetButtonTheme(button);
            }
            else if (control is Panel panel)
            {
                SetPanelTheme(panel);
            }
            else if (control is GroupBox groupBox)
            {
                if(groupBox.Parent != null)
                {
                    groupBox.BackColor = groupBox.Parent.BackColor;
                }
                groupBox.ForeColor = OScolors.TextInactive;
            }
            else if (control is TableLayoutPanel tableLayoutPanel)
            {
                if(tableLayoutPanel.Parent != null)
                {
                    tableLayoutPanel.BackColor = tableLayoutPanel.Parent.BackColor;
                }
                tableLayoutPanel.ForeColor = OScolors.TextInactive;
                tableLayoutPanel.BorderStyle = BorderStyle.None;
            }
            else if (control is TabControl tab)
            {
                SetTabControlTheme(tab);
                if (control is ClosableTabControl ctab)
                {
                    ctab.DarkMode = true;
                }
            }
            else if (control is PictureBox pictureBox)
            {
                if(pictureBox.Parent != null)
                {
                    pictureBox.BackColor = pictureBox.Parent.BackColor;
                }
                pictureBox.ForeColor = OScolors.TextActive;
                pictureBox.BorderStyle = BorderStyle.None;
            }
            else if (control is ListView lView)
            {
                SetListViewTheme(lView);
            }
            else if (control is CheckBox checkBox)
            {
                if(checkBox.Parent != null)
                {
                    checkBox.BackColor = checkBox.Parent.BackColor;
                }
            }
            else if (control is RadioButton radioButton)
            {
                if(radioButton.Parent != null)
                {
                    radioButton.BackColor = radioButton.Parent.BackColor;
                }
            }
            else if (control is ComboBox comboBox)
            {
                _ = SetWindowTheme(comboBox.Handle, IsDarkMode ? "DarkMode_CFD" : "ClearMode_CFD", null);
            }
            else if (control is PropertyGrid pGrid)
            {
                pGrid.BackColor = OScolors.Control;
                pGrid.ViewBackColor = OScolors.Control;
                pGrid.LineColor = OScolors.Surface;
                pGrid.ViewForeColor = OScolors.TextActive;
                pGrid.ViewBorderColor = OScolors.ControlDark;
                pGrid.CategoryForeColor = OScolors.TextActive;
                pGrid.CategorySplitterColor = OScolors.ControlLight;
            }
            else if (control is TreeView tree)
            {
                //SetTreeViewTheme(tree);
            }
            else if (control is DataGridView grid)
            {
                SetDataGridViewTheme(grid);
            }
            else if (control is MenuStrip menuStrip)
            {
                menuStrip.RenderMode = ToolStripRenderMode.Professional;
                menuStrip.Renderer = new MyRenderer(new CustomColorTable(OScolors), ColorizeIcons) { MyColors = OScolors };
            }
            else if (control is ToolStrip toolStrip)
            {
                toolStrip.RenderMode = ToolStripRenderMode.Professional;
                toolStrip.Renderer = new MyRenderer(new CustomColorTable(OScolors), ColorizeIcons) { MyColors = OScolors };
            }
            else if (control is ToolStripPanel toolStripPanel)
            {
                if(toolStripPanel.Parent != null)
                    toolStripPanel.BackColor = toolStripPanel.Parent.BackColor;
            }
            else if (control is ToolStripDropDown toolStripDropDown)
            {
                toolStripDropDown.Opening += Tsdd_Opening;
            }
            else if (control is ToolStripDropDownMenu toolStripDropDownMenu)
            {
                toolStripDropDownMenu.Opening += Tsdd_Opening;
            }
            else if (control is ContextMenuStrip contextMenuStrip)
            {
                contextMenuStrip.RenderMode = ToolStripRenderMode.Professional;
                contextMenuStrip.Renderer = new MyRenderer(new CustomColorTable(OScolors), ColorizeIcons) { MyColors = OScolors };
                contextMenuStrip.Opening += Tsdd_Opening;
            }
            else if (control is StatusStrip statusStrip)
            {
                statusStrip.RenderMode = ToolStripRenderMode.Professional;
                statusStrip.Renderer = new MyRenderer(new CustomColorTable(OScolors), ColorizeIcons) { MyColors = OScolors };
            }
            else if (control is SplitContainer splitContainer)
            {
                if(splitContainer.Parent !=null) splitContainer.BackColor = splitContainer.Parent.BackColor;
                splitContainer.Panel1.BackColor = OScolors.Surface;
                splitContainer.Panel2.BackColor = OScolors.Surface;
            }
            else if (control is Splitter splitter)
            {
                splitter.BackColor = OScolors.ControlLight;
            }
            else if (control is NumericUpDown numericUpDown)
            {
                numericUpDown.BorderStyle = BStyle;
            }
            else if (control is MonthCalendar monthCalendar)
            {
                monthCalendar.BackColor = OScolors.Control;
                monthCalendar.ForeColor = OScolors.TextActive;
            }
            else if (control is MdiClient mdiClient)
            {
                mdiClient.BackColor = OScolors.Surface;
            }
            else if (control is SqlEditBox editBox)
            {
                editBox.DarkMode = true;
            }
            else
            {
                //control.GetType().GetProperty("BackColor")?.SetValue(control, OScolors.Control);
                //control.GetType().GetProperty("ForeColor")?.SetValue(control, OScolors.TextActive);
                //control.GetType().GetProperty("BorderStyle")?.SetValue(control, BStyle);

                System.Diagnostics.Debug.Print(control.GetType().Name + " not themed yet: " + control.Name);
            }

            //switch (control.GetType().Name)
            //{
            //    case "FlatTabControl":
            //        control.GetType().GetProperty("BackColor")?.SetValue(control, OScolors.Background);
            //        control.GetType().GetProperty("TabColor")?.SetValue(control, OScolors.Surface);
            //        control.GetType().GetProperty("SelectTabColor")?.SetValue(control, OScolors.Control);
            //        control.GetType().GetProperty("SelectedForeColor")?.SetValue(control, OScolors.TextActive);
            //        control.GetType().GetProperty("ForeColor")?.SetValue(control, OScolors.TextInactive);
            //        control.GetType().GetProperty("LineColor")?.SetValue(control, OScolors.Background);
            //        break;

            //    default:
            //        break;
            //}

            //Debug.Print(string.Format("{0}: {1}", control.Name, control.GetType().Name));

            if (control.ContextMenuStrip != null)
                ThemeControl(control.ContextMenuStrip);

            foreach (Control childControl in control.Controls)
            {
                // Recursively process its children
                ThemeControl(childControl);
            }
        }

        /// <summary>
        /// Sets the data grid view theme.
        /// </summary>
        /// <param name="grid">The grid.</param>
        private void SetDataGridViewTheme(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.BackgroundColor = OScolors.Control;
            grid.GridColor = OScolors.Control;

            //paint the bottom right corner where the scrollbars meet
            grid.Paint += (object? sender, PaintEventArgs e) =>
            {
                DataGridView dgv = sender as DataGridView;

                //get the value of dgv.HorizontalScrollBar protected property
                HScrollBar hs = (HScrollBar)typeof(DataGridView).GetProperty("HorizontalScrollBar", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(dgv);
                if (hs.Visible)
                {
                    //get the value of dgv.VerticalScrollBar protected property
                    VScrollBar vs = (VScrollBar)typeof(DataGridView).GetProperty("VerticalScrollBar", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(dgv);

                    if (vs.Visible)
                    {
                        //only when both the scrollbars are visible, do the actual painting
                        Brush brush = new SolidBrush(OScolors.SurfaceDark);
                        var w = vs.Size.Width;
                        var h = hs.Size.Height;
                        e.Graphics.FillRectangle(brush, dgv.ClientRectangle.X + dgv.ClientRectangle.Width - w - 1,
                            dgv.ClientRectangle.Y + dgv.ClientRectangle.Height - h - 1, w, h);
                    }
                }
            };

            grid.DefaultCellStyle.BackColor = OScolors.Surface;
            grid.DefaultCellStyle.ForeColor = OScolors.TextActive;

            grid.ColumnHeadersDefaultCellStyle.BackColor = OScolors.Surface;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = OScolors.TextActive;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = OScolors.AccentOpaque;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersHeight = 140;

            grid.RowHeadersDefaultCellStyle.BackColor = OScolors.Surface;
            grid.RowHeadersDefaultCellStyle.ForeColor = OScolors.TextActive;
            grid.RowHeadersDefaultCellStyle.SelectionBackColor = OScolors.AccentOpaque;
            grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        }

        /// <summary>
        /// Sets the tree view theme.
        /// </summary>
        /// <param name="tree">The tree.</param>
        private void SetTreeViewTheme(TreeView tree)
        {
            tree.GetType().GetProperty("BorderStyle")?.SetValue(tree, BorderStyle.None);
            tree.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            tree.DrawNode += (object? sender, DrawTreeNodeEventArgs e) =>
            {
                // Draw background
                Color backColor = e.Node.IsSelected ? OScolors.Accent : tree.BackColor;
                using (Brush backBrush = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(backBrush, e.Bounds);
                }

                // Draw image if present
                if (tree.ImageList != null && e.Node.ImageIndex >= 0 && e.Node.ImageIndex < tree.ImageList.Images.Count)
                {
                    Image origImage = tree.ImageList.Images[e.Node.ImageIndex];
                    Color imageColor = e.Node.IsSelected ? OScolors.TextInAccent : OScolors.TextActive;
                    using (Image img = DarkMode.ChangeToColor(origImage, imageColor))
                    {
                        int imgY = e.Bounds.Y + (e.Bounds.Height - img.Height) / 2;
                        e.Graphics.DrawImage(img, e.Bounds.X, imgY, img.Width, img.Height);
                    }
                    // Offset text to the right of the image
                    int textOffset = tree.ImageList.ImageSize.Width + 3;
                    Rectangle textRect = new Rectangle(e.Bounds.X + textOffset, e.Bounds.Y, e.Bounds.Width - textOffset, e.Bounds.Height);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, tree.Font, textRect, OScolors.TextActive, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                }
                else
                {
                    // Draw text only
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, tree.Font, e.Bounds, OScolors.TextActive, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                }
                e.DrawDefault = false;
            };
        }

        /// <summary>
        /// Sets the list view theme.
        /// </summary>
        /// <param name="lView">The l view.</param>
        private void SetListViewTheme(System.Windows.Forms.ListView lView)
        {
            var mode = IsDarkMode ? "DarkMode_ItemsView" : "ClearMode_ItemsView";
            _ = SetWindowTheme(lView.Handle, mode, null);

            if (lView.View == View.Details)
            {
                lView.OwnerDraw = true;
                lView.DrawColumnHeader += (object sender, DrawListViewColumnHeaderEventArgs e) =>
                {
                    //e.DrawDefault = true;
                    //e.DrawBackground();
                    //e.DrawText();

                    using SolidBrush backBrush = new SolidBrush(OScolors.ControlLight);
                    using SolidBrush foreBrush = new SolidBrush(OScolors.TextActive);
                    using var sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    e.Graphics.FillRectangle(backBrush, e.Bounds);
                    e.Graphics.DrawString(e.Header.Text, lView.Font, foreBrush, e.Bounds, sf);
                };
                lView.DrawItem += (sender, e) => { e.DrawDefault = true; };
                lView.DrawSubItem += (sender, e) =>
                {
                    e.DrawDefault = true;

                    //IntPtr headerControl = GetHeaderControl(lView);
                    //IntPtr hdc = GetDC(headerControl);
                    //Rectangle rc = new Rectangle(
                    //  e.Bounds.Right, //<- Right instead of Left - offsets the rectangle
                    //  e.Bounds.Top,
                    //  e.Bounds.Width,
                    //  e.Bounds.Height
                    //);
                    //rc.Width += 200;

                    //using (SolidBrush backBrush = new SolidBrush(OScolors.ControlLight))
                    //{
                    //  e.Graphics.FillRectangle(backBrush, rc);
                    //}

                    //ReleaseDC(headerControl, hdc);
                };

                _ = SetWindowTheme(lView.Handle, mode, null);
            }
        }

        /// <summary>
        /// Sets the closable tab control theme.
        /// </summary>
        /// <param name="ctab">The ctab.</param>
        private void SetClosableTabControlTheme(ClosableTabControl ctab)
        {
            ctab.Appearance = TabAppearance.Normal;
            ctab.DrawMode = TabDrawMode.OwnerDrawFixed;

            // Calculate and set the correct tab width for each tab
            int maxTabHeight = 0;
            for (int i = 0; i < ctab.TabPages.Count; i++)
            {
                var tabPage = ctab.TabPages[i];
                Size textSize = TextRenderer.MeasureText(tabPage.Text, tabPage.Font);
                int tabWidth = 8 + textSize.Width + 6 + 16 + 8; // left padding + text + margin + X + right padding
                if (tabWidth > ctab.ItemSize.Width)
                    ctab.ItemSize = new Size(tabWidth, ctab.ItemSize.Height > 0 ? ctab.ItemSize.Height : textSize.Height + 8);
                if (textSize.Height > maxTabHeight)
                    maxTabHeight = textSize.Height;
            }
            if (ctab.ItemSize.Height < maxTabHeight + 8)
                ctab.ItemSize = new Size(ctab.ItemSize.Width, maxTabHeight + 8);

            ctab.DrawItem += (object sender, DrawItemEventArgs e) =>
            {
                // Use dark mode colors from OScolors
                var tabPage = ctab.TabPages[e.Index];
                var tabRect = ctab.GetTabRect(e.Index);

                // Determine background color for active/inactive tab
                //Color backColor = (e.Index == ctab.SelectedIndex)
                //    ? OScolors.Surface
                //    : OScolors.ControlDark;
                Color backColor = (e.Index == ctab.SelectedIndex)
                    ? Color.Red
                    : OScolors.ControlDark;

                using (var backBrush = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(backBrush, tabRect);
                }

                //// Draw tab text with appropriate color
                //Color textColor = (e.Index == ctab.SelectedIndex)
                //    ? OScolors.TextActive
                //    : OScolors.TextInactive;

                //TextRenderer.DrawText(
                //    e.Graphics,
                //    tabPage.Text,
                //    tabPage.Font,
                //    tabRect,
                //    textColor,
                //    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                //// Calculate close button rectangle
                //int closeButtonSize = 9;
                //int closeButtonMargin = 3;
                //Rectangle closeButton = new(
                //    tabRect.Right - closeButtonSize - closeButtonMargin - 2,
                //    tabRect.Top + (tabRect.Height - closeButtonSize) / 2 + 1,
                //    closeButtonSize,
                //    closeButtonSize);

                //// Draw close button (X) with accent or text color
                //Color closeColor = (e.Index == ctab.SelectedIndex)
                //    ? OScolors.Accent
                //    : OScolors.TextInactive;

                //using Pen pen = new(closeColor, 1.6f);
                //e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                //e.Graphics.DrawLine(pen, closeButton.Left, closeButton.Top, closeButton.Right, closeButton.Bottom);
                //e.Graphics.DrawLine(pen, closeButton.Right, closeButton.Top, closeButton.Left, closeButton.Bottom);
            };
        }

        /// <summary>
        /// Sets the tab control theme.
        /// </summary>
        /// <param name="tab">The tab.</param>
        private void SetTabControlTheme(TabControl tab)
        {
            tab.Appearance = TabAppearance.Normal;
            tab.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            tab.DrawItem += (object sender, DrawItemEventArgs e) =>
            {
                //Draw the background of the main control
                using (SolidBrush backColor = new(tab.Parent.BackColor))
                {
                    e.Graphics.FillRectangle(backColor, tab.ClientRectangle);
                }

                using Brush tabBack = new SolidBrush(OScolors.Surface);
                for (int i = 0; i < tab.TabPages.Count; i++)
                {
                    TabPage tabPage = tab.TabPages[i];
                    tabPage.BackColor = OScolors.Surface;
                    tabPage.BorderStyle = BorderStyle.FixedSingle;
                    tabPage.ControlAdded += (object _s, ControlEventArgs _e) =>
                    {
                        ThemeControl(_e.Control);
                    };

                    var tBounds = e.Bounds;
                    //tBounds.Inflate(100, 100);

                    bool IsSelected = (tab.SelectedIndex == i);
                    if (IsSelected)
                    {
                        e.Graphics.FillRectangle(tabBack, tBounds);
                        TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font, e.Bounds, OScolors.TextActive);
                    }
                    else
                    {
                        TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font, tab.GetTabRect(i), OScolors.TextInactive);
                    }
                }
            };
        }

        /// <summary>
        /// Sets the panel theme.
        /// </summary>
        /// <param name="panel">The panel.</param>
        private void SetPanelTheme(Panel panel)
        {
            panel.BackColor = OScolors.Surface;
            panel.BorderStyle = BorderStyle.None;
            if (!(panel.Parent is TabControl) || !(panel.Parent is TableLayoutPanel))
            {
                if (RoundedPanels)
                {
                    SetRoundBorders(panel, 6, OScolors.SurfaceDark, 1);
                }
            }

            // set theme for all child controls
            foreach (Control child in panel.Controls)
            {
                ThemeControl(child);
            }
        }

        /// <summary>
        /// Sets the theme for the button.
        /// </summary>
        /// <param name="button">The button.</param>
        private void SetButtonTheme(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.CheckedBackColor = OScolors.Accent;
            button.BackColor = OScolors.Control;
            button.FlatAppearance.BorderColor = (OwnerForm.AcceptButton == button) ?
                OScolors.Accent : OScolors.Control;

            // Redraw the image if it exists
            if (button.Image != null)
            {
                // Optionally recolor the image for dark mode
                var recolored = IsDarkMode
                    ? DarkMode.ChangeToColor(new Bitmap(button.Image), OScolors.TextActive)
                    : new Bitmap(button.Image);
                button.Image = recolored;
            }
        }

        /// <summary>Returns Windows Color Mode for Applications.
        /// <para>0=dark theme, 1=light theme</para>
        /// </summary>
        public static int GetWindowsColorMode(bool GetSystemColorModeInstead = false)
        {
            try
            {
                return (int)Microsoft.Win32.Registry.GetValue(
                    @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                    GetSystemColorModeInstead ? "SystemUsesLightTheme" : "AppsUseLightTheme",
                    -1);
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>Returns the Accent Color used by Windows.</summary>
        /// <returns>a Color</returns>
        public static Color GetWindowsAccentColor()
        {
            DWMCOLORIZATIONcolors colors = new DWMCOLORIZATIONcolors();
            DwmGetColorizationParameters(ref colors);

            //get the theme --> only if Windows 10 or newer
            if (IsWindows10orGreater())
            {
                var color = colors.ColorizationColor;

                var colorValue = long.Parse(color.ToString(), System.Globalization.NumberStyles.HexNumber);

                var transparency = (colorValue >> 24) & 0xFF;
                var red = (colorValue >> 16) & 0xFF;
                var green = (colorValue >> 8) & 0xFF;
                var blue = (colorValue >> 0) & 0xFF;

                return Color.FromArgb((int)transparency, (int)red, (int)green, (int)blue);
            }
            else
            {
                return Color.CadetBlue;
            }
        }

        /// <summary>Returns the Accent Color used by Windows.</summary>
        /// <returns>an opaque Color</returns>
        public static Color GetWindowsAccentOpaqueColor()
        {
            DWMCOLORIZATIONcolors colors = new DWMCOLORIZATIONcolors();
            DwmGetColorizationParameters(ref colors);

            //get the theme --> only if Windows 10 or newer
            if (IsWindows10orGreater())
            {
                var color = colors.ColorizationColor;

                var colorValue = long.Parse(color.ToString(), System.Globalization.NumberStyles.HexNumber);

                var red = (colorValue >> 16) & 0xFF;
                var green = (colorValue >> 8) & 0xFF;
                var blue = (colorValue >> 0) & 0xFF;

                return Color.FromArgb(255, (int)red, (int)green, (int)blue);
            }
            else
            {
                return Color.CadetBlue;
            }
        }

        /// <summary>Returns Windows's System Colors for UI components following Google Material Design concepts.</summary>
        /// <param name="Window">[OPTIONAL] Applies DarkMode (if set) to this Window Title and Background.</param>
        /// <returns>List of Colors:  Background, OnBackground, Surface, OnSurface, Primary, OnPrimary, Secondary, OnSecondary</returns>
        public static OSThemeColors GetSystemColors(Form Window = null, int ColorMode = 0) //<- O: DarkMode, 1: LightMode
        {
            OSThemeColors _ret = new OSThemeColors();

            //bool IsDarkMode = IsDarkModeEnabled && (GetWindowsColorMode() <= 0); //<- O: DarkMode, 1: LightMode
            //if (IsDarkMode)
            if (ColorMode <= 0)
            {
                _ret.Background = Color.FromArgb(32, 32, 32);   //<- Negro Claro
                _ret.BackgroundDark = Color.FromArgb(18, 18, 18);
                _ret.BackgroundLight = ControlPaint.Light(_ret.Background);

                _ret.Surface = Color.FromArgb(43, 43, 43);      //<- Gris Oscuro
                _ret.SurfaceLight = Color.FromArgb(50, 50, 50);
                _ret.SurfaceDark = Color.FromArgb(29, 29, 29);

                _ret.TextActive = Color.White;
                _ret.TextInactive = Color.FromArgb(176, 176, 176);  //<- Blanco Palido
                _ret.TextInAccent = GetReadableColor(_ret.Accent);

                _ret.Control = Color.FromArgb(55, 55, 55);       //<- Gris Oscuro
                _ret.ControlDark = ControlPaint.Dark(_ret.Control);
                _ret.ControlLight = Color.FromArgb(67, 67, 67);

                _ret.Primary = Color.FromArgb(3, 218, 198);   //<- Verde Pastel
                _ret.Secondary = Color.MediumSlateBlue;         //<- Magenta Claro
            }

            return _ret;
        }

        /// <summary>Apply Round Corners to the indicated Control or Form.</summary>
        /// <param name="_Control">the one who will have rounded Corners. Set BorderStyle = None.</param>
        /// <param name="Radius">Radious for the Corners</param>
        /// <param name="borderColor">Color for the Border</param>
        /// <param name="borderSize">Size in pixels of the border line</param>
        /// <param name="underlinedStyle"></param>
        public static void SetRoundBorders(Control _Control, int Radius = 10, Color? borderColor = null, int borderSize = 2, bool underlinedStyle = false)
        {
            try
            {
                borderColor = borderColor ?? Color.MediumSlateBlue;

                if (_Control != null)
                {
                    _Control.GetType().GetProperty("BorderStyle")?.SetValue(_Control, BorderStyle.None);
                    _Control.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, _Control.Width, _Control.Height, Radius, Radius));
                    _Control.Paint += (object sender, PaintEventArgs e) =>
                    {
                        //base.OnPaint(e);
                        Graphics graph = e.Graphics;

                        if (Radius > 1)//Rounded TextBox
                        {
                            //-Fields
                            var rectBorderSmooth = _Control.ClientRectangle;
                            var rectBorder = Rectangle.Inflate(rectBorderSmooth, -borderSize, -borderSize);
                            int smoothSize = borderSize > 0 ? borderSize : 1;

                            using (GraphicsPath pathBorderSmooth = GetFigurePath(rectBorderSmooth, Radius))
                            using (GraphicsPath pathBorder = GetFigurePath(rectBorder, Radius - borderSize))
                            using (Pen penBorderSmooth = new Pen(_Control.Parent.BackColor, smoothSize))
                            using (Pen penBorder = new Pen((Color)borderColor, borderSize))
                            {
                                //-Drawing
                                _Control.Region = new Region(pathBorderSmooth);//Set the rounded region of UserControl
                                if (Radius > 15) //Set the rounded region of TextBox component
                                {
                                    using (GraphicsPath pathTxt = GetFigurePath(_Control.ClientRectangle, borderSize * 2))
                                    {
                                        _Control.Region = new Region(pathTxt);
                                    }
                                }
                                graph.SmoothingMode = SmoothingMode.AntiAlias;
                                penBorder.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
                                //if (isFocused) penBorder.Color = borderFocusColor;

                                if (underlinedStyle) //Line Style
                                {
                                    //Draw border smoothing
                                    graph.DrawPath(penBorderSmooth, pathBorderSmooth);
                                    //Draw border
                                    graph.SmoothingMode = SmoothingMode.None;
                                    graph.DrawLine(penBorder, 0, _Control.Height - 1, _Control.Width, _Control.Height - 1);
                                }
                                else //Normal Style
                                {
                                    //Draw border smoothing
                                    graph.DrawPath(penBorderSmooth, pathBorderSmooth);
                                    //Draw border
                                    graph.DrawPath(penBorder, pathBorder);
                                }
                            }
                        }
                    };
                }
            }
            catch { throw; }
        }

        /// <summary>Colorea una imagen usando una Matrix de Color.</summary>
        /// <param name="bmp">Imagen a Colorear</param>
        /// <param name="c">Color a Utilizar</param>
        public static Bitmap ChangeToColor(Bitmap bmp, Color c)
        {
            return ConvertToDarkMode_Invert(bmp);

            /*
                        Bitmap bmp2 = new Bitmap(bmp.Width, bmp.Height);
                        using (Graphics g = Graphics.FromImage(bmp2))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.SmoothingMode = SmoothingMode.HighQuality;

                            float tR = c.R / 255f;
                            float tG = c.G / 255f;
                            float tB = c.B / 255f;

                            System.Drawing.Imaging.ColorMatrix colorMatrix = new System.Drawing.Imaging.ColorMatrix(new float[][]
                            {
                                new float[] { 1,    0,  0,  0,  0 },
                                new float[] { 0,    1,  0,  0,  0 },
                                new float[] { 0,    0,  1,  0,  0 },
                                new float[] { 0,    0,  0,  1,  0 },  //<- not changing alpha
                                new float[] { tR,   tG, tB, 0,  1 }
                            });

                            System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
                            attributes.SetColorMatrix(colorMatrix);

                            g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height),
                                0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                        return bmp2;
            */
        }

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

        /// <summary>
        /// Changes the to color.
        /// </summary>
        /// <param name="bmp">The bmp.</param>
        /// <param name="c">The c.</param>
        /// <returns>An Image.</returns>
        public static Image ChangeToColor(Image bmp, Color c) => (Image)ChangeToColor((Bitmap)bmp, c);

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// handle hierarchical context menus (otherwise, only the root level gets themed)
        /// </summary>
        private void Tsdd_Opening(object? sender, CancelEventArgs e)
        {
            if(sender == null) return; //should not occur
            var tsdd = sender as ToolStripDropDown;
            if (tsdd == null) return; //should not occur

            foreach (ToolStripMenuItem toolStripMenuItem in tsdd.Items.OfType<ToolStripMenuItem>())
            {
                toolStripMenuItem.DropDownOpening += Tsmi_DropDownOpening;
            }
        }

        /// <summary>
        /// handle hierarchical context menus (otherwise, only the root level gets themed)
        /// </summary>
        private void Tsmi_DropDownOpening(object? sender, EventArgs e)
        {
            if (sender == null) return; //should not occur
            var tsmi = sender as ToolStripMenuItem;
            if (tsmi == null) return; //should not occur

            if (tsmi.DropDown.Items.Count > 0) ThemeControl(tsmi.DropDown);

            //once processed, remove itself to prevent multiple executions (when user leaves and reenters the sub-menu)
            tsmi.DropDownOpening -= Tsmi_DropDownOpening;
        }

        /// <summary>Attemps to apply Window's Dark Style to the Control and all its childs.</summary>
        /// <param name="control"></param>
        private static void ApplySystemDarkTheme(Control control = null, bool IsDarkMode = true)
        {
            //if (!IsDarkModeEnabled) return;
            /*
			DWMWA_USE_IMMERSIVE_DARK_MODE:   https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute

			Use with DwmSetWindowAttribute. Allows the window frame for this window to be drawn in dark mode colors when the dark mode system setting is enabled.
			For compatibility reasons, all windows default to light mode regardless of the system setting.
			The pvAttribute parameter points to a value of type BOOL. TRUE to honor dark mode for the window, FALSE to always use light mode.

			This value is supported starting with Windows 11 Build 22000.

			SetWindowTheme:     https://learn.microsoft.com/en-us/windows/win32/api/uxtheme/nf-uxtheme-setwindowtheme
			Causes a window to use a different set of visual style information than its class normally uses.
		   */
            int[] DarkModeOn = IsDarkMode ? new[] { 0x01 } : new[] { 0x00 }; //<- 1=True, 0=False
            string Mode = IsDarkMode ? "DarkMode_Explorer" : "ClearMode_Explorer";

            SetWindowTheme(control.Handle, Mode, null); //DarkMode_Explorer, ClearMode_Explorer, DarkMode_CFD, DarkMode_ItemsView,

            if (DwmSetWindowAttribute(control.Handle, (int)DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, DarkModeOn, 4) != 0)
                DwmSetWindowAttribute(control.Handle, (int)DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, DarkModeOn, 4);

            foreach (Control child in control.Controls)
            {
                if (child.Controls.Count != 0)
                    ApplySystemDarkTheme(child, IsDarkMode);
            }
        }

        /// <summary>
        /// Are the windows10or greater.
        /// </summary>
        /// <returns>A bool.</returns>
        private static bool IsWindows10orGreater()
        {
            if (WindowsVersion() >= 10)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Windows the version.
        /// </summary>
        /// <returns>An int.</returns>
        private static int WindowsVersion()
        {
            //for .Net4.8 and Minor
            int result;
            try
            {
                var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                string[] productName = reg.GetValue("ProductName").ToString().Split((char)32);
                int.TryParse(productName[1], out result);
            }
            catch (Exception)
            {
                OperatingSystem os = Environment.OSVersion;
                result = os.Version.Major;
            }

            return result;

            //fixed .Net6
            //return System.Environment.OSVersion.Version.Major;
        }

        /// <summary>
        /// Gets the readable color.
        /// </summary>
        /// <param name="backgroundColor">The background color.</param>
        /// <returns>A Color.</returns>
        private static Color GetReadableColor(Color backgroundColor)
        {
            // Calculate the relative luminance of the background color.
            // Normalize values to 0-1 range first.
            double normalizedR = backgroundColor.R / 255.0;
            double normalizedG = backgroundColor.G / 255.0;
            double normalizedB = backgroundColor.B / 255.0;
            double luminance = 0.299 * normalizedR + 0.587 * normalizedG + 0.114 * normalizedB;

            // Choose a contrasting foreground color based on the luminance,
            // with a slight bias towards lighter colors for better readability.
            return luminance < 0.5 ? Color.FromArgb(182, 180, 215) : Color.FromArgb(34, 34, 34); // Dark gray for light backgrounds
        }

        // For Rounded Corners:
        /// <summary>
        /// Gets the figure path.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>A GraphicsPath.</returns>
        private static GraphicsPath GetFigurePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        #endregion Private Methods
    }

    /// <summary>Windows 10+ System Colors for Clear Color Mode.</summary>
    public class OSThemeColors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OSThemeColors"/> class.
        /// </summary>
        public OSThemeColors()
        {
        }

        /// <summary>For the very back of the Window</summary>
        public System.Drawing.Color Background { get; set; } = SystemColors.Control;

        /// <summary>For Borders around the Background</summary>
        public System.Drawing.Color BackgroundDark { get; set; } = SystemColors.ControlDark;

        /// <summary>For hightlights over the Background</summary>
        public System.Drawing.Color BackgroundLight { get; set; } = SystemColors.ControlLight;

        /// <summary>For Container above the Background</summary>
        public System.Drawing.Color Surface { get; set; } = SystemColors.ControlLightLight;

        /// <summary>For Borders around the Surface</summary>
        public System.Drawing.Color SurfaceDark { get; set; } = SystemColors.ControlLight;

        /// <summary>For Highligh over the Surface</summary>
        public System.Drawing.Color SurfaceLight { get; set; } = Color.White;

        /// <summary>For Main Texts</summary>
        public System.Drawing.Color TextActive { get; set; } = SystemColors.ControlText;

        /// <summary>For Inactive Texts</summary>
        public System.Drawing.Color TextInactive { get; set; } = SystemColors.GrayText;

        /// <summary>For Hightligh Texts</summary>
        public System.Drawing.Color TextInAccent { get; set; } = SystemColors.HighlightText;

        /// <summary>For the background of any Control</summary>
        public System.Drawing.Color Control { get; set; } = SystemColors.ButtonFace;

        /// <summary>For Bordes of any Control</summary>
        public System.Drawing.Color ControlDark { get; set; } = SystemColors.ButtonShadow;

        /// <summary>For Highlight elements in a Control</summary>
        public System.Drawing.Color ControlLight { get; set; } = SystemColors.ButtonHighlight;

        /// <summary>Windows 10+ Chosen Accent Color</summary>
        public System.Drawing.Color Accent { get; set; } = DarkMode.GetWindowsAccentColor();

        public System.Drawing.Color AccentOpaque { get; set; } = DarkMode.GetWindowsAccentOpaqueColor();

        public System.Drawing.Color AccentDark
        { get { return ControlPaint.Dark(Accent); } }

        public System.Drawing.Color AccentLight
        { get { return ControlPaint.Light(Accent); } }

        /// <summary>the color displayed most frequently across your app's screens and components.</summary>
        public System.Drawing.Color Primary { get; set; } = SystemColors.Highlight;

        public System.Drawing.Color PrimaryDark
        { get { return ControlPaint.Dark(Primary); } }

        public System.Drawing.Color PrimaryLight
        { get { return ControlPaint.Light(Primary); } }

        /// <summary>to accent select parts of your UI.</summary>
        public System.Drawing.Color Secondary { get; set; } = SystemColors.HotTrack;

        public System.Drawing.Color SecondaryDark
        { get { return ControlPaint.Dark(Secondary); } }

        public System.Drawing.Color SecondaryLight
        { get { return ControlPaint.Light(Secondary); } }
    }

    /* Custom Renderers for Menus and ToolBars */

    /// <summary>
    /// The my renderer.
    /// </summary>
    public class MyRenderer : ToolStripProfessionalRenderer
    {
        /// <summary>
        /// Gets or sets a value indicating whether colorize icons.
        /// </summary>
        public bool ColorizeIcons { get; set; } = true;

        /// <summary>
        /// Gets or sets the my colors.
        /// </summary>
        public OSThemeColors MyColors { get; set; } //<- Your Custom Colors Colection

        /// <summary>
        /// Initializes a new instance of the <see cref="MyRenderer"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="pColorizeIcons">If true, p colorize icons.</param>
        public MyRenderer(ProfessionalColorTable table, bool pColorizeIcons = true) : base(table)
        {
            ColorizeIcons = pColorizeIcons;
        }

        /// <summary>
        /// Draws the title bar.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="rect">The rect.</param>
        private void DrawTitleBar(Graphics g, Rectangle rect)
        {
            // Assign the image for the grip.
            //Image titlebarGrip = titleBarGripBmp;

            // Fill the titlebar.
            // This produces the gradient and the rounded-corner effect.
            //g.DrawLine(new Pen(titlebarColor1), rect.X, rect.Y, rect.X + rect.Width, rect.Y);
            //g.DrawLine(new Pen(titlebarColor2), rect.X, rect.Y + 1, rect.X + rect.Width, rect.Y + 1);
            //g.DrawLine(new Pen(titlebarColor3), rect.X, rect.Y + 2, rect.X + rect.Width, rect.Y + 2);
            //g.DrawLine(new Pen(titlebarColor4), rect.X, rect.Y + 3, rect.X + rect.Width, rect.Y + 3);
            //g.DrawLine(new Pen(titlebarColor5), rect.X, rect.Y + 4, rect.X + rect.Width, rect.Y + 4);
            //g.DrawLine(new Pen(titlebarColor6), rect.X, rect.Y + 5, rect.X + rect.Width, rect.Y + 5);
            //g.DrawLine(new Pen(titlebarColor7), rect.X, rect.Y + 6, rect.X + rect.Width, rect.Y + 6);

            // Center the titlebar grip.
            //g.DrawImage(
            //  titlebarGrip,
            //  new Point(rect.X + ((rect.Width / 2) - (titlebarGrip.Width / 2)),
            //  rect.Y + 1));
        }

        /// <summary>
        /// This method handles the RenderGrip event.
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
        {
            DrawTitleBar(
                e.Graphics,
                new Rectangle(0, 0, e.ToolStrip.Width, 7));
        }

        /// <summary>
        /// This method handles the RenderToolStripBorder event.
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            DrawTitleBar(
                e.Graphics,
                new Rectangle(0, 0, e.ToolStrip.Width, 7));
        }

        /// <summary>
        /// Background of the whole ToolBar Or MenuBar:
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.ToolStrip.BackColor = MyColors.Background;
            base.OnRenderToolStripBackground(e);
        }

        /// <summary>
        /// For Normal Buttons on a ToolBar:
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);

            Color gradientBegin = MyColors.Background;
            Color gradientEnd = MyColors.Background;

            Pen BordersPencil = new Pen(MyColors.Background);

            ToolStripButton button = e.Item as ToolStripButton;
            if (button.Pressed || button.Checked)
            {
                gradientBegin = MyColors.Control;
                gradientEnd = MyColors.Control;
            }
            else if (button.Selected)
            {
                gradientBegin = MyColors.Accent;
                gradientEnd = MyColors.Accent;
            }

            using (Brush b = new LinearGradientBrush(
                bounds,
                gradientBegin,
                gradientEnd,
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(b, bounds);
            }

            e.Graphics.DrawRectangle(BordersPencil, bounds);

            g.DrawLine(BordersPencil, bounds.X, bounds.Y, bounds.Width - 1, bounds.Y);
            g.DrawLine(BordersPencil, bounds.X, bounds.Y, bounds.X, bounds.Height - 1);

            if (!(button.Owner.GetItemAt(button.Bounds.X, button.Bounds.Bottom + 1) is ToolStripButton nextItem))
            {
                g.DrawLine(BordersPencil, bounds.X, bounds.Height - 1, bounds.X + bounds.Width - 1, bounds.Height - 1);
            }

            // Do NOT draw the image here; let OnRenderItemImage handle it.
        }

        /// <summary>
        /// Re-Colors the Icon Images to a Clear color
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            // Handle ToolStripButton image recoloring
            if (e.Item is ToolStripButton button && ColorizeIcons && MyColors != null && button.Image != null)
            {
                Color imageColor = e.Item.Enabled ? MyColors.TextActive : MyColors.TextInactive;
                using Image adjustedImage = DarkMode.ChangeToColor(new Bitmap(button.Image), imageColor);
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.DrawImage(adjustedImage, e.ImageRectangle);
                return; // Suppress base drawing
            }

            // Handle ToolStripStatusLabel image recoloring
            if (e.Item is ToolStripStatusLabel statusLabel && ColorizeIcons && MyColors != null && statusLabel.Image != null)
            {
                Color imageColor = e.Item.Enabled ? MyColors.TextActive : MyColors.TextInactive;
                using Image adjustedImage = DarkMode.ChangeToColor(new Bitmap(statusLabel.Image), imageColor);
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.DrawImage(adjustedImage, e.ImageRectangle);
                return; // Suppress base drawing
            }

            // Default behavior for other cases
            base.OnRenderItemImage(e);
        }

        //protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        //{
        //    if (e.Item is ToolStripStatusLabel && e.Image != null)
        //    {
        //        // Use TextActive for enabled, TextInactive for disabled
        //        Color imageColor = e.Item.Enabled ? MyColors.TextActive : MyColors.TextInactive;
        //        using (Image adjustedImage = DarkMode.ChangeToColor(e.Image, imageColor))
        //        {
        //            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        //            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        //            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        //            e.Graphics.DrawImage(adjustedImage, e.ImageRectangle);
        //        }
        //        return;
        //    }
        //    else
        //    {
        //        //System.Diagnostics.Debug.Print("MyRenderer: " + e.Item.GetType().FullName);
        //    }

        //    // Existing logic for other items
        //    base.OnRenderItemImage(e);
        //}


        /// <summary>
        /// For DropDown Buttons on a ToolBar
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
            Color gradientBegin = MyColors.Background; // Color.FromArgb(203, 225, 252);
            Color gradientEnd = MyColors.Background;

            Pen BordersPencil = new Pen(MyColors.Background);

            //1. Determine the colors to use:
            if (e.Item.Pressed)
            {
                gradientBegin = MyColors.Control;
                gradientEnd = MyColors.Control;
            }
            else if (e.Item.Selected)
            {
                gradientBegin = MyColors.Accent;
                gradientEnd = MyColors.Accent;
            }

            //2. Draw the Box around the Control
            using (Brush b = new LinearGradientBrush(
                bounds,
                gradientBegin,
                gradientEnd,
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(b, bounds);
            }

            //3. Draws the Chevron:

            #region Chevron

            //int Padding = 2; //<- From the right side
            //Size cSize = new Size(8, 4); //<- Size of the Chevron: 8x4 px
            //Pen ChevronPen = new Pen(MyColors.TextInactive, 2); //<- Color and Border Width
            //Point P1 = new Point(bounds.Width - (cSize.Width + Padding), (bounds.Height / 2) - (cSize.Height / 2));
            //Point P2 = new Point(bounds.Width - Padding, (bounds.Height / 2) - (cSize.Height / 2));
            //Point P3 = new Point(bounds.Width - (cSize.Width / 2 + Padding), (bounds.Height / 2) + (cSize.Height / 2));

            //e.Graphics.DrawLine(ChevronPen, P1, P3);
            //e.Graphics.DrawLine(ChevronPen, P2, P3);

            #endregion Chevron
        }

        /// <summary>
        /// For SplitButtons on a ToolBar
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
            Color gradientBegin = MyColors.Background; // Color.FromArgb(203, 225, 252);
            Color gradientEnd = MyColors.Background;

            //1. Determine the colors to use:
            if (e.Item.Pressed)
            {
                gradientBegin = MyColors.Control;
                gradientEnd = MyColors.Control;
            }
            else if (e.Item.Selected)
            {
                gradientBegin = MyColors.Accent;
                gradientEnd = MyColors.Accent;
            }

            //2. Draw the Box around the Control
            using (Brush b = new LinearGradientBrush(
                bounds,
                gradientBegin,
                gradientEnd,
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(b, bounds);
            }

            //3. Draws the Chevron:

            #region Chevron

            int Padding = 2; //<- From the right side
            Size cSize = new Size(8, 4); //<- Size of the Chevron: 8x4 px
            Pen ChevronPen = new Pen(MyColors.TextInactive, 2); //<- Color and Border Width
            Point P1 = new Point(bounds.Width - (cSize.Width + Padding), (bounds.Height / 2) - (cSize.Height / 2));
            Point P2 = new Point(bounds.Width - Padding, (bounds.Height / 2) - (cSize.Height / 2));
            Point P3 = new Point(bounds.Width - (cSize.Width / 2 + Padding), (bounds.Height / 2) + (cSize.Height / 2));

            e.Graphics.DrawLine(ChevronPen, P1, P3);
            e.Graphics.DrawLine(ChevronPen, P2, P3);

            #endregion Chevron
        }

        /// <summary>
        /// For the Text Color of all Items:
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.Item is ToolStripStatusLabel)
            {
                e.TextColor = e.Item.Enabled ? MyColors.TextActive : MyColors.TextInactive;
            }
            else if (e.Item is ToolStripMenuItem)
            {
                // Use accent color for selected, otherwise normal
                if (e.Item.Selected)
                    e.TextColor = MyColors.TextInAccent;
                else
                    e.TextColor = e.Item.Enabled ? MyColors.TextActive : MyColors.TextInactive;
            }
            else
            {
                e.TextColor = e.Item.Enabled ? MyColors.TextActive : MyColors.TextInactive;
            }
            base.OnRenderItemText(e);
        }

        /// <summary>
        /// draws the background of the label.
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item is ToolStripStatusLabel)
            {
                using (SolidBrush backBrush = new SolidBrush(MyColors.Control))
                {
                    e.Graphics.FillRectangle(backBrush, e.Item.Bounds);
                }
            }
            else
            {
                base.OnRenderLabelBackground(e);
            }
        }

        /// <summary>
        /// overrides the OnRenderItemBackground method to draw a border around ComboBox items.
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderItemBackground(e);

            // Only draw border for ComboBox items
            if (e.Item is ComboBox)
            {
                Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
                e.Graphics.DrawRectangle(new Pen(MyColors.ControlLight, 1), rect);
            }
        }

        /// <summary>
        /// For Menu Items BackColor
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);

            Color gradientBegin = MyColors.Background; // Color.FromArgb(203, 225, 252);
            Color gradientEnd = MyColors.Background; // Color.FromArgb(125, 165, 224);

            bool DrawIt = false;
            var _menu = e.Item as ToolStripItem;
            if (_menu.Pressed)
            {
                gradientBegin = MyColors.Control; // Color.FromArgb(254, 128, 62);
                gradientEnd = MyColors.Control; // Color.FromArgb(255, 223, 154);
                DrawIt = true;
            }
            else if (_menu.Selected)
            {
                gradientBegin = MyColors.Accent;// Color.FromArgb(255, 255, 222);
                gradientEnd = MyColors.Accent; // Color.FromArgb(255, 203, 136);
                DrawIt = true;
            }

            if (DrawIt)
            {
                using (Brush b = new LinearGradientBrush(
                bounds,
                gradientBegin,
                gradientEnd,
                LinearGradientMode.Vertical))
                {
                    g.FillRectangle(b, bounds);
                }
            }
        }
    }

    /// <summary>
    /// The custom color table.
    /// </summary>
    public class CustomColorTable : ProfessionalColorTable
    {
        /// <summary>
        /// Gets or sets the colors.
        /// </summary>
        public OSThemeColors Colors { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomColorTable"/> class.
        /// </summary>
        /// <param name="_Colors">The _ colors.</param>
        public CustomColorTable(OSThemeColors _Colors)
        {
            Colors = _Colors;
            base.UseSystemColors = false;
        }

        /// <summary>
        /// Gets the image margin gradient begin.
        /// </summary>
        public override Color ImageMarginGradientBegin => Colors.Control;

        /// <summary>
        /// Gets the image margin gradient middle.
        /// </summary>
        public override Color ImageMarginGradientMiddle => Colors.Control;

        /// <summary>
        /// Gets the image margin gradient end.
        /// </summary>
        public override Color ImageMarginGradientEnd => Colors.Control;
    }
}
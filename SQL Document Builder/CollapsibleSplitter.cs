using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    #region Enums

    /// <summary>
    /// Enumeration to sepcify the visual style to be applied to the CollapsibleSplitter control
    /// </summary>
    public enum VisualStyles
    {
        Mozilla = 0,
        XP,
        Win9x,
        DoubleDots,
        Lines
    }

    /// <summary>
    /// Enumeration to specify the current animation state of the control.
    /// </summary>
    public enum SplitterState
    {
        Collapsed = 0,
        Expanding,
        Expanded,
        Collapsing
    }

    #endregion Enums

    //[DesignerAttribute(typeof(CollapsibleSplitterDesigner))]
    /// <summary>
    /// A custom collapsible splitter that can resize, hide and show associated form controls
    /// </summary>
    [ToolboxBitmap(typeof(CollapsibleSplitter))]
    public class CollapsibleSplitter : Splitter
    {
        public event EventHandler? StateChanged;

        #region Private Properties

        // declare and define some base properties
        private bool hot;

        private readonly Color hotColor = CalculateColor(SystemColors.Highlight, SystemColors.Window, 70);
        private Rectangle rr;
        private Form? parentForm;
        private VisualStyles visualStyle;

        // Border added in version 1.3
        private Border3DStyle borderStyle = System.Windows.Forms.Border3DStyle.Flat;

        // animation controls introduced in version 1.22
        private readonly Timer animationTimer;

        private int controlWidth;
        private int controlHeight;
        private int parentFormWidth;
        private int parentFormHeight;
        private SplitterState currentState = SplitterState.Expanded;

        public SplitterState State
        {
            get { return currentState; }
        }

        #endregion Private Properties

        private void ChangeSate(SplitterState state)
        {
            //if (currentState != state)
            //{
            currentState = state;
            StateChanged?.Invoke(this, EventArgs.Empty);
            //}
        }

        #region Public Properties

        /// <summary>
        /// The initial state of the Splitter. Set to True if the control to hide is not visible by default
        /// </summary>
        [Bindable(true), Category("Collapsing Options"), DefaultValue("False"),
        Description("The initial state of the Splitter. Set to True if the control to hide is not visible by default")]
        public bool IsCollapsed
        {
            get
            {
                return ControlToHide != null ? !ControlToHide.Visible : true;
            }
        }

        /// <summary>
        /// The System.Windows.Forms.Control that the splitter will collapse
        /// </summary>
        [Bindable(true), Category("Collapsing Options"), DefaultValue(""),
        Description("The System.Windows.Forms.Control that the splitter will collapse")]
        public Control? ControlToHide { get; set; }

        /// <summary>
        /// Determines if the collapse and expanding actions will be animated
        /// </summary>
        [Bindable(true), Category("Collapsing Options"), DefaultValue("True"),
        Description("Determines if the collapse and expanding actions will be animated")]
        public bool UseAnimations { get; set; }

        /// <summary>
        /// The delay in millisenconds between animation steps
        /// </summary>
        [Bindable(true), Category("Collapsing Options"), DefaultValue("20"),
        Description("The delay in millisenconds between animation steps")]
        public int AnimationDelay { get; set; } = 20;

        /// <summary>
        /// The amount of pixels moved in each animation step
        /// </summary>
        [Bindable(true), Category("Collapsing Options"), DefaultValue("20"),
        Description("The amount of pixels moved in each animation step")]
        public int AnimationStep { get; set; } = 20;

        /// <summary>
        /// When true the entire parent form will be expanded and collapsed, otherwise just the contol to expand will be changed
        /// </summary>
        [Bindable(true), Category("Collapsing Options"), DefaultValue("False"),
        Description("When true the entire parent form will be expanded and collapsed, otherwise just the contol to expand will be changed")]
        public bool ExpandParentForm { get; set; }

        /// <summary>
        /// The visual style that will be painted on the control
        /// </summary>
        [Bindable(true), Category("Collapsing Options"), DefaultValue("VisualStyles.XP"),
        Description("The visual style that will be painted on the control")]
        public VisualStyles VisualStyle
        {
            get { return this.visualStyle; }
            set
            {
                this.visualStyle = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// An optional border style to paint on the control. Set to Flat for no border
        /// </summary>
        [Bindable(true), Category("Collapsing Options"), DefaultValue("System.Windows.Forms.Border3DStyle.Flat"),
        Description("An optional border style to paint on the control. Set to Flat for no border")]
        public Border3DStyle BorderStyle3D
        {
            get { return this.borderStyle; }
            set
            {
                this.borderStyle = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets splitter distance (size of the control)
        /// </summary>
        public int SplitterDistance
        {
            get
            {
                if (this.ControlToHide == null)
                {
                    return 0;
                }
                else
                {
                    if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                    {
                        return this.ControlToHide.Width;
                    }
                    else
                    {
                        return this.ControlToHide.Height;
                    }
                }
            }
            set
            {
                if (this.ControlToHide != null && value >= 0)
                {
                    if (this.currentState == SplitterState.Expanded)
                    {
                        if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                        {
                            this.ControlToHide.Width = value;
                        }
                        else
                        {
                            this.ControlToHide.Height = value;
                        }
                    }
                }
            }
        }

        #endregion Public Properties

        #region "Public Methods"

        public void ToggleState()
        {
            this.ToggleSplitter();
        }

        public void Collapse()
        {
            //if (currentState != SplitterState.Collapsed && ControlToHide != null)
            if (!IsCollapsed && ControlToHide != null)
            {
                //currentState = SplitterState.Collapsed;
                ControlToHide.Visible = false;
                if (ExpandParentForm && parentForm != null)
                {
                    if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                    {
                        parentForm.Width -= ControlToHide.Width;
                    }
                    else
                    {
                        parentForm.Width -= ControlToHide.Height;
                    }
                }
                ChangeSate(SplitterState.Collapsed);
            }
        }

        public void Expand()
        {
            if (IsCollapsed)
            //if (currentState != SplitterState.Expanded && ControlToHide != null)
            {
                //currentState = SplitterState.Expanded;
                if (ControlToHide != null)
                {
                    ControlToHide.Visible = true;
                    if (ExpandParentForm && parentForm != null)
                    {
                        if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                        {
                            parentForm.Width += ControlToHide.Width;
                        }
                        else
                        {
                            parentForm.Width += ControlToHide.Height;
                        }
                    }
                    ChangeSate(SplitterState.Expanded);
                }
            }
        }

        #endregion "Public Methods"

        #region Constructor

        public CollapsibleSplitter()
        {
            // Register mouse events
            this.Click += OnClick;
            this.Resize += OnResize;
            this.MouseLeave += OnMouseLeave;
            this.MouseMove += OnMouseMove;

            // Setup the animation timer control
            this.animationTimer = new System.Windows.Forms.Timer
            {
                Interval = AnimationDelay
            };
            this.animationTimer.Tick += this.AnimationTimerTick;
        }

        #endregion Constructor

        #region Overrides

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.parentForm = this.FindForm();

            // set the current state
            if (this.ControlToHide != null)
            {
                if (this.ControlToHide.Visible)
                {
                    this.currentState = SplitterState.Expanded;
                }
                else
                {
                    this.currentState = SplitterState.Collapsed;
                }
            }
        }

        protected override void OnEnabledChanged(System.EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.Invalidate();
        }

        #endregion Overrides

        #region Event Handlers

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // if the hider control isn't hot, let the base resize action occur
            if (this.ControlToHide != null)
            {
                if (!this.hot && this.ControlToHide.Visible)
                {
                    base.OnMouseDown(e);
                }
            }
        }

        private void OnResize(object? sender, System.EventArgs e)
        {
            this.Invalidate();
        }

        // this method was updated in version 1.11 to fix a flickering problem
        // discovered by John O'Byrne
        private void OnMouseMove(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            // check to see if the mouse cursor position is within the bounds of our control
            if (e.X >= rr.X && e.X <= rr.X + rr.Width && e.Y >= rr.Y && e.Y <= rr.Y + rr.Height)
            {
                if (!this.hot)
                {
                    this.hot = true;
                    this.Cursor = Cursors.Hand;
                    this.Invalidate();
                }
            }
            else
            {
                if (this.hot)
                {
                    this.hot = false;
                    this.Invalidate(); ;
                }

                this.Cursor = Cursors.Default;

                if (ControlToHide != null)
                {
                    if (!ControlToHide.Visible)
                    {
                        this.Cursor = Cursors.Default;
                    }
                    else // Changed in v1.2 to support Horizontal Splitters
                    {
                        if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                        {
                            this.Cursor = Cursors.VSplit;
                        }
                        else
                        {
                            this.Cursor = Cursors.HSplit;
                        }
                    }
                }
            }
        }

        private void OnMouseLeave(object? sender, System.EventArgs e)
        {
            // ensure that the hot state is removed
            this.hot = false;
            this.Invalidate();
        }

        private void OnClick(object? sender, System.EventArgs e)
        {
            if (ControlToHide != null && hot &&
                currentState != SplitterState.Collapsing &&
                currentState != SplitterState.Expanding)
            {
                ToggleSplitter();
            }
        }

        private void ToggleSplitter()
        {
            // if an animation is currently in progress for this control, drop out
            if (currentState == SplitterState.Collapsing || currentState == SplitterState.Expanding)
            {
                return;
            }

            if (ControlToHide != null)
            {
                controlWidth = ControlToHide.Width;
                controlHeight = ControlToHide.Height;

                if (ControlToHide.Visible)
                {
                    if (UseAnimations)
                    {
                        currentState = SplitterState.Collapsing;

                        if (parentForm != null)
                        {
                            if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                            {
                                parentFormWidth = parentForm.Width - controlWidth;
                            }
                            else
                            {
                                parentFormHeight = parentForm.Height - controlHeight;
                            }
                        }

                        this.animationTimer.Enabled = true;
                    }
                    else
                    {
                        // no animations, so just toggle the visible state
                        //currentState = SplitterState.Collapsed;
                        ControlToHide.Visible = false;
                        if (ExpandParentForm && parentForm != null)
                        {
                            if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                            {
                                parentForm.Width -= ControlToHide.Width;
                            }
                            else
                            {
                                parentForm.Height -= ControlToHide.Height;
                            }
                        }
                        ChangeSate(SplitterState.Collapsed);
                    }
                }
                else
                {
                    // control to hide is collapsed
                    if (UseAnimations)
                    {
                        currentState = SplitterState.Expanding;

                        if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                        {
                            if (parentForm != null)
                            {
                                parentFormWidth = parentForm.Width + controlWidth;
                            }
                            ControlToHide.Width = 0;
                        }
                        else
                        {
                            if (parentForm != null)
                            {
                                parentFormHeight = parentForm.Height + controlHeight;
                            }
                            ControlToHide.Height = 0;
                        }
                        ControlToHide.Visible = true;
                        this.animationTimer.Enabled = true;
                    }
                    else
                    {
                        // no animations, so just toggle the visible state
                        //currentState = SplitterState.Expanded;
                        ControlToHide.Visible = true;
                        if (ExpandParentForm && parentForm != null)
                        {
                            if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                            {
                                parentForm.Width += ControlToHide.Width;
                            }
                            else
                            {
                                parentForm.Height += ControlToHide.Height;
                            }
                        }
                        ChangeSate(SplitterState.Expanded);
                    }
                }
            }
        }

        #endregion Event Handlers

        #region Implementation

        #region Animation Timer Tick

        private void AnimationTimerTick(object? sender, System.EventArgs e)
        {
            if (ControlToHide != null && parentForm != null)
            {
                switch (currentState)
                {
                    case SplitterState.Collapsing:

                        if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                        {
                            // vertical splitter
                            if (ControlToHide.Width > AnimationStep)
                            {
                                if (ExpandParentForm && parentForm.WindowState != FormWindowState.Maximized
                                    && parentForm != null)
                                {
                                    parentForm.Width -= AnimationStep;
                                }
                                ControlToHide.Width -= AnimationStep;
                            }
                            else
                            {
                                if (ExpandParentForm && parentForm.WindowState != FormWindowState.Maximized
                                    && parentForm != null)
                                {
                                    parentForm.Width = parentFormWidth;
                                }
                                ControlToHide.Visible = false;
                                animationTimer.Enabled = false;
                                ControlToHide.Width = controlWidth;
                                //currentState = SplitterState.Collapsed;
                                ChangeSate(SplitterState.Collapsed);
                                this.Invalidate();
                            }
                        }
                        else
                        {
                            // horizontal splitter
                            if (ControlToHide.Height > AnimationStep)
                            {
                                if (ExpandParentForm && parentForm.WindowState != FormWindowState.Maximized
                                    && parentForm != null)
                                {
                                    parentForm.Height -= AnimationStep;
                                }
                                ControlToHide.Height -= AnimationStep;
                            }
                            else
                            {
                                if (ExpandParentForm && parentForm.WindowState != FormWindowState.Maximized
                                    && parentForm != null)
                                {
                                    parentForm.Height = parentFormHeight;
                                }
                                ControlToHide.Visible = false;
                                animationTimer.Enabled = false;
                                ControlToHide.Height = controlHeight;
                                //currentState = SplitterState.Collapsed;
                                ChangeSate(SplitterState.Collapsed);
                                this.Invalidate();
                            }
                        }
                        break;

                    case SplitterState.Expanding:

                        if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                        {
                            // vertical splitter
                            if (ControlToHide.Width < (controlWidth - AnimationStep))
                            {
                                if (ExpandParentForm && parentForm.WindowState != FormWindowState.Maximized
                                    && parentForm != null)
                                {
                                    parentForm.Width += AnimationStep;
                                }
                                ControlToHide.Width += AnimationStep;
                            }
                            else
                            {
                                if (ExpandParentForm && parentForm.WindowState != FormWindowState.Maximized
                                    && parentForm != null)
                                {
                                    parentForm.Width = parentFormWidth;
                                }
                                ControlToHide.Width = controlWidth;
                                ControlToHide.Visible = true;
                                animationTimer.Enabled = false;
                                //currentState = SplitterState.Expanded;
                                ChangeSate(SplitterState.Expanded);
                                this.Invalidate();
                            }
                        }
                        else
                        {
                            // horizontal splitter
                            if (ControlToHide.Height < (controlHeight - AnimationStep))
                            {
                                if (ExpandParentForm && parentForm.WindowState != FormWindowState.Maximized
                                    && parentForm != null)
                                {
                                    parentForm.Height += AnimationStep;
                                }
                                ControlToHide.Height += AnimationStep;
                            }
                            else
                            {
                                if (ExpandParentForm && parentForm.WindowState != FormWindowState.Maximized
                                    && parentForm != null)
                                {
                                    parentForm.Height = parentFormHeight;
                                }
                                ControlToHide.Height = controlHeight;
                                ControlToHide.Visible = true;
                                animationTimer.Enabled = false;
                                //currentState = SplitterState.Expanded;
                                ChangeSate(SplitterState.Expanded);
                                this.Invalidate();
                            }
                        }
                        break;
                }
            }
        }

        #endregion Animation Timer Tick

        #region Paint the control

        // OnPaint is now an override rather than an event in version 1.1
        protected override void OnPaint(PaintEventArgs e)
        {
            // create a Graphics object
            Graphics g = e.Graphics;

            // find the rectangle for the splitter and paint it
            Rectangle r = this.ClientRectangle; // fixed in version 1.1
            g.FillRectangle(new SolidBrush(this.BackColor), r);

            #region Vertical Splitter

            // Check the docking style and create the control rectangle accordingly
            if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
            {
                // create a new rectangle in the vertical center of the splitter for our collapse control button
                rr = new Rectangle(r.X, (int)r.Y + ((r.Height - 115) / 2), 8, 115);
                // force the width to 8px so that everything always draws correctly
                this.Width = 8;

                // draw the background color for our control image
                if (hot)
                {
                    g.FillRectangle(new SolidBrush(hotColor), new Rectangle(rr.X + 1, rr.Y, 6, 115));
                }
                else
                {
                    g.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(rr.X + 1, rr.Y, 6, 115));
                }

                // draw the top & bottom lines for our control image
                g.DrawLine(new Pen(SystemColors.ControlDark, 1), rr.X + 1, rr.Y, rr.X + rr.Width - 2, rr.Y);
                g.DrawLine(new Pen(SystemColors.ControlDark, 1), rr.X + 1, rr.Y + rr.Height, rr.X + rr.Width - 2, rr.Y + rr.Height);

                if (this.Enabled)
                {
                    // draw the arrows for our control image
                    // the ArrowPointArray is a point array that defines an arrow shaped polygon
                    g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), ArrowPointArray(rr.X + 2, rr.Y + 3));
                    g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), ArrowPointArray(rr.X + 2, rr.Y + rr.Height - 9));
                }

                // draw the dots for our control image using a loop
                int x = rr.X + 3;
                int y = rr.Y + 14;

                // Visual Styles added in version 1.1
                switch (visualStyle)
                {
                    case VisualStyles.Mozilla:

                        for (int i = 0; i < 30; i++)
                        {
                            // light dot
                            g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y + (i * 3), x + 1, y + 1 + (i * 3));
                            // dark dot
                            g.DrawLine(new Pen(SystemColors.ControlDarkDark), x + 1, y + 1 + (i * 3), x + 2, y + 2 + (i * 3));
                            // overdraw the background color as we actually drew 2px diagonal lines, not just dots
                            if (hot)
                            {
                                g.DrawLine(new Pen(hotColor), x + 2, y + 1 + (i * 3), x + 2, y + 2 + (i * 3));
                            }
                            else
                            {
                                g.DrawLine(new Pen(this.BackColor), x + 2, y + 1 + (i * 3), x + 2, y + 2 + (i * 3));
                            }
                        }
                        break;

                    case VisualStyles.DoubleDots:
                        for (int i = 0; i < 30; i++)
                        {
                            // light dot
                            g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x, y + 1 + (i * 3), 1, 1);
                            // dark dot
                            g.DrawRectangle(new Pen(SystemColors.ControlDark), x - 1, y + (i * 3), 1, 1);
                            i++;
                            // light dot
                            g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 2, y + 1 + (i * 3), 1, 1);
                            // dark dot
                            g.DrawRectangle(new Pen(SystemColors.ControlDark), x + 1, y + (i * 3), 1, 1);
                        }
                        break;

                    case VisualStyles.Win9x:

                        g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x + 2, y);
                        g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x, y + 90);
                        g.DrawLine(new Pen(SystemColors.ControlDark), x + 2, y, x + 2, y + 90);
                        g.DrawLine(new Pen(SystemColors.ControlDark), x, y + 90, x + 2, y + 90);
                        break;

                    case VisualStyles.XP:

                        for (int i = 0; i < 18; i++)
                        {
                            // light dot
                            g.DrawRectangle(new Pen(SystemColors.ControlLight), x, y + (i * 5), 2, 2);
                            // light light dot
                            g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1, y + 1 + (i * 5), 1, 1);
                            // dark dark dot
                            g.DrawRectangle(new Pen(SystemColors.ControlDarkDark), x, y + (i * 5), 1, 1);
                            // dark fill
                            g.DrawLine(new Pen(SystemColors.ControlDark), x, y + (i * 5), x, y + (i * 5) + 1);
                            g.DrawLine(new Pen(SystemColors.ControlDark), x, y + (i * 5), x + 1, y + (i * 5));
                        }
                        break;

                    case VisualStyles.Lines:

                        for (int i = 0; i < 44; i++)
                        {
                            g.DrawLine(new Pen(SystemColors.ControlDark), x, y + (i * 2), x + 2, y + (i * 2));
                        }

                        break;
                }

                // Added in version 1.3
                if (this.borderStyle != System.Windows.Forms.Border3DStyle.Flat)
                {
                    // Paint the control border
                    ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Left);
                    ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Right);
                }
            }

            #endregion Vertical Splitter

            // Horizontal Splitter support added in v1.2

            #region Horizontal Splitter

            else if (this.Dock == DockStyle.Top || this.Dock == DockStyle.Bottom)
            {
                // create a new rectangle in the horizontal center of the splitter for our collapse control button
                rr = new Rectangle((int)r.X + ((r.Width - 115) / 2), r.Y, 115, 8);
                // force the height to 8px
                this.Height = 8;

                // draw the background color for our control image
                if (hot)
                {
                    g.FillRectangle(new SolidBrush(hotColor), new Rectangle(rr.X, rr.Y + 1, 115, 6));
                }
                else
                {
                    g.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(rr.X, rr.Y + 1, 115, 6));
                }

                // draw the left & right lines for our control image
                g.DrawLine(new Pen(SystemColors.ControlDark, 1), rr.X, rr.Y + 1, rr.X, rr.Y + rr.Height - 2);
                g.DrawLine(new Pen(SystemColors.ControlDark, 1), rr.X + rr.Width, rr.Y + 1, rr.X + rr.Width, rr.Y + rr.Height - 2);

                if (this.Enabled)
                {
                    // draw the arrows for our control image
                    // the ArrowPointArray is a point array that defines an arrow shaped polygon
                    g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), ArrowPointArray(rr.X + 3, rr.Y + 2));
                    g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), ArrowPointArray(rr.X + rr.Width - 9, rr.Y + 2));
                }

                // draw the dots for our control image using a loop
                int x = rr.X + 14;
                int y = rr.Y + 3;

                // Visual Styles added in version 1.1
                switch (visualStyle)
                {
                    case VisualStyles.Mozilla:

                        for (int i = 0; i < 30; i++)
                        {
                            // light dot
                            g.DrawLine(new Pen(SystemColors.ControlLightLight), x + (i * 3), y, x + 1 + (i * 3), y + 1);
                            // dark dot
                            g.DrawLine(new Pen(SystemColors.ControlDarkDark), x + 1 + (i * 3), y + 1, x + 2 + (i * 3), y + 2);
                            // overdraw the background color as we actually drew 2px diagonal lines, not just dots
                            if (hot)
                            {
                                g.DrawLine(new Pen(hotColor), x + 1 + (i * 3), y + 2, x + 2 + (i * 3), y + 2);
                            }
                            else
                            {
                                g.DrawLine(new Pen(this.BackColor), x + 1 + (i * 3), y + 2, x + 2 + (i * 3), y + 2);
                            }
                        }
                        break;

                    case VisualStyles.DoubleDots:

                        for (int i = 0; i < 30; i++)
                        {
                            // light dot
                            g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1 + (i * 3), y, 1, 1);
                            // dark dot
                            g.DrawRectangle(new Pen(SystemColors.ControlDark), x + (i * 3), y - 1, 1, 1);
                            i++;
                            // light dot
                            g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1 + (i * 3), y + 2, 1, 1);
                            // dark dot
                            g.DrawRectangle(new Pen(SystemColors.ControlDark), x + (i * 3), y + 1, 1, 1);
                        }
                        break;

                    case VisualStyles.Win9x:

                        g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x, y + 2);
                        g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x + 88, y);
                        g.DrawLine(new Pen(SystemColors.ControlDark), x, y + 2, x + 88, y + 2);
                        g.DrawLine(new Pen(SystemColors.ControlDark), x + 88, y, x + 88, y + 2);
                        break;

                    case VisualStyles.XP:

                        for (int i = 0; i < 18; i++)
                        {
                            // light dot
                            g.DrawRectangle(new Pen(SystemColors.ControlLight), x + (i * 5), y, 2, 2);
                            // light light dot
                            g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1 + (i * 5), y + 1, 1, 1);
                            // dark dark dot
                            g.DrawRectangle(new Pen(SystemColors.ControlDarkDark), x + (i * 5), y, 1, 1);
                            // dark fill
                            g.DrawLine(new Pen(SystemColors.ControlDark), x + (i * 5), y, x + (i * 5) + 1, y);
                            g.DrawLine(new Pen(SystemColors.ControlDark), x + (i * 5), y, x + (i * 5), y + 1);
                        }
                        break;

                    case VisualStyles.Lines:

                        for (int i = 0; i < 44; i++)
                        {
                            g.DrawLine(new Pen(SystemColors.ControlDark), x + (i * 2), y, x + (i * 2), y + 2);
                        }

                        break;
                }

                // Added in version 1.3
                if (this.borderStyle != System.Windows.Forms.Border3DStyle.Flat)
                {
                    // Paint the control border
                    ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Top);
                    ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Bottom);
                }
            }

            #endregion Horizontal Splitter

            else
            {
                throw new Exception("The Collapsible Splitter control cannot have the Filled or None Dockstyle property");
            }

            // dispose the Graphics object
            g.Dispose();
        }

        #endregion Paint the control

        #region Arrow Polygon Array

        // This creates a point array to draw a arrow-like polygon
        private Point[] ArrowPointArray(int x, int y)
        {
            Point[] point = new Point[3];

            if (ControlToHide != null)
            {
                // decide which direction the arrow will point
                if (
                    (this.Dock == DockStyle.Right && ControlToHide.Visible)
                    || (this.Dock == DockStyle.Left && !ControlToHide.Visible)
                    )
                {
                    // right arrow
                    point[0] = new Point(x, y);
                    point[1] = new Point(x + 3, y + 3);
                    point[2] = new Point(x, y + 6);
                }
                else if (
                    (this.Dock == DockStyle.Right && !ControlToHide.Visible)
                    || (this.Dock == DockStyle.Left && ControlToHide.Visible)
                    )
                {
                    // left arrow
                    point[0] = new Point(x + 3, y);
                    point[1] = new Point(x, y + 3);
                    point[2] = new Point(x + 3, y + 6);
                }

                // Up/Down arrows added in v1.2
                else if (
                    (this.Dock == DockStyle.Top && ControlToHide.Visible)
                    || (this.Dock == DockStyle.Bottom && !ControlToHide.Visible)
                    )
                {
                    // up arrow
                    point[0] = new Point(x + 3, y);
                    point[1] = new Point(x + 6, y + 4);
                    point[2] = new Point(x, y + 4);
                }
                else if (
                    (this.Dock == DockStyle.Top && !ControlToHide.Visible)
                    || (this.Dock == DockStyle.Bottom && ControlToHide.Visible)
                    )
                {
                    // down arrow
                    point[0] = new Point(x, y);
                    point[1] = new Point(x + 6, y);
                    point[2] = new Point(x + 3, y + 3);
                }
            }

            return point;
        }

        #endregion Arrow Polygon Array

        #region Color Calculator

        // this method was borrowed from the RichUI Control library by Sajith M
        private static Color CalculateColor(Color front, Color back, int alpha)
        {
            // solid color obtained as a result of alpha-blending

            Color frontColor = Color.FromArgb(255, front);
            Color backColor = Color.FromArgb(255, back);

            float frontRed = frontColor.R;
            float frontGreen = frontColor.G;
            float frontBlue = frontColor.B;
            float backRed = backColor.R;
            float backGreen = backColor.G;
            float backBlue = backColor.B;

            float fRed = frontRed * alpha / 255 + backRed * ((float)(255 - alpha) / 255);
            byte newRed = (byte)fRed;
            float fGreen = frontGreen * alpha / 255 + backGreen * ((float)(255 - alpha) / 255);
            byte newGreen = (byte)fGreen;
            float fBlue = frontBlue * alpha / 255 + backBlue * ((float)(255 - alpha) / 255);
            byte newBlue = (byte)fBlue;

            return Color.FromArgb(255, newRed, newGreen, newBlue);
        }

        #endregion Color Calculator

        #endregion Implementation
    }

    ///// <summary>
    ///// A simple designer class for the CollapsibleSplitter control to remove
    ///// unwanted properties at design time.
    ///// </summary>
    //public class CollapsibleSplitterDesigner : System.Windows.Forms.Design.ControlDesigner
    //{
    //    public CollapsibleSplitterDesigner()
    //    {
    //    }

    //    protected override void PreFilterProperties(System.Collections.IDictionary properties)
    //    {
    //        properties.Remove("IsCollapsed");
    //        properties.Remove("BorderStyle");
    //        properties.Remove("Size");
    //    }
    //}
}
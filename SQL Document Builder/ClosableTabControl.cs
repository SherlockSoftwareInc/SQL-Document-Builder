using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// The closable tab control.
/// </summary>
public class ClosableTabControl : TabControl
{
    /// <summary>
    /// The close button margin.
    /// </summary>
    private const int CloseButtonMargin = 3;

    /// <summary>
    /// The close button size.
    /// </summary>
    private const int CloseButtonSize = 9;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClosableTabControl"/> class.
    /// </summary>
    public ClosableTabControl()
    {
        Appearance = TabAppearance.Normal;
        DrawMode = TabDrawMode.OwnerDrawFixed;
        Padding = new Point(CloseButtonSize + CloseButtonMargin, 3);
    }

    /// <summary>
    /// Occurs when a tab close is requested.
    /// </summary>
    public event EventHandler<TabCloseRequestedEventArgs>? TabCloseRequested;

    private bool _darkmode = false;

    /// <summary>
    /// Gets or sets a value indicating whether dark mode.
    /// </summary>
    public bool DarkMode
    {
        get { return _darkmode; }
        set
        {
            _darkmode = value;
            Invalidate();
        }
    }

    /// <summary>
    /// Ons the draw item.
    /// </summary>
    /// <param name="e">The e.</param>
    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        //Draw the background of the main control
        using (SolidBrush backColor = new(Parent.BackColor))
        {
            e.Graphics.FillRectangle(backColor, ClientRectangle);
        }

        using Brush tabBack = new SolidBrush(_darkmode
            ? Color.FromArgb(60, 60, 70) // or another dark color of your choice
            : SystemColors.ControlLightLight);

        for (int i = 0; i < TabPages.Count; i++)
        {
            TabPage tabPage = this.TabPages[i];
            tabPage.BorderStyle = BorderStyle.FixedSingle;

            var tBounds = e.Bounds;

            bool isSelected = (this.SelectedIndex == i);

            // Choose colors based on dark mode
            Color tabColor, textColor, closeColor;
            if (_darkmode)
            {
                tabColor = Color.FromArgb(60, 60, 70);
                textColor = isSelected ? SystemColors.HighlightText : SystemColors.GrayText;
                closeColor = textColor;
            }
            else
            {
                tabColor = SystemColors.Control;
                textColor = this.ForeColor;
                closeColor = isSelected ? this.ForeColor : Color.Gray;
            }

            tabPage.BackColor = tabColor;

            // Draw tab background 
            if (isSelected) e.Graphics.FillRectangle(tabBack, tBounds);

            // Draw tab text
            TextRenderer.DrawText(
                e.Graphics,
                tabPage.Text,
                this.Font,
                GetTabRect(i),
                textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            var tabRect = this.GetTabRect(i);
            // Calculate close button rectangle
            Rectangle closeButton = new(
                tabRect.Right - CloseButtonSize - CloseButtonMargin - 2,
                tabRect.Top + (tabRect.Height - CloseButtonSize) / 2 + 1,
                CloseButtonSize,
                CloseButtonSize);

            // Draw close button
            using Pen pen = new(closeColor, 1.6f);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawLine(pen, closeButton.Left, closeButton.Top, closeButton.Right, closeButton.Bottom);
            e.Graphics.DrawLine(pen, closeButton.Right, closeButton.Top, closeButton.Left, closeButton.Bottom);
        }
    }

    /// <summary>
    /// Ons the mouse down.
    /// </summary>
    /// <param name="e">The e.</param>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        for (int i = 0; i < this.TabPages.Count; i++)
        {
            Rectangle tabRect = this.GetTabRect(i);
            Rectangle closeButton = new(
                tabRect.Right - CloseButtonSize - CloseButtonMargin - 2,
                tabRect.Top + (tabRect.Height - CloseButtonSize) / 2 + 1,
                CloseButtonSize,
                CloseButtonSize);

            if (closeButton.Contains(e.Location))
            {
                var args = new TabCloseRequestedEventArgs(this.TabPages[i]);
                TabCloseRequested?.Invoke(this, args);
                //this.TabPages.RemoveAt(i);
                break;
            }
        }
        base.OnMouseDown(e);
    }
}

/// <summary>
/// Event arguments for tab close requests.
/// </summary>
public class TabCloseRequestedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TabCloseRequestedEventArgs"/> class.
    /// </summary>
    /// <param name="tabPage">The tab page.</param>
    public TabCloseRequestedEventArgs(TabPage tabPage)
    {
        TabPage = tabPage;
        Cancel = false;
    }

    /// <summary>
    /// Gets or sets a value indicating whether cancel.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Gets the tab page.
    /// </summary>
    public TabPage TabPage { get; }
}
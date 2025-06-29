using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace SoukeyControl.CustomControl
{
    public partial class cMyTabControl : TabControl
    {
        public cMyTabControl() 
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            PreRemoveTabPage = null;
            this.DrawMode = TabDrawMode.OwnerDrawFixed;

            this.ResizeRedraw = true;
        }

        #region 关闭tab的操作
        public delegate bool PreRemoveTab(int indx);
        public PreRemoveTab PreRemoveTabPage;

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Point p = e.Location;
            for (int i = 0; i < TabCount; i++)
            {
                Rectangle r = GetTabRect(i);
                r.Offset(r.Width - 10, r.Top + 2);
                r.Width = 10;
                r.Height =10;
                if (r.Contains(p))
                {
                    //触发关闭事件
                    if (e_CloseTab != null)
                    {
                        e_CloseTab(this, new cCloseTabEvent(i));
                        break;
                    }
                }
            }
        }

        public void CloseTabPage(int i)
        {
            if (PreRemoveTabPage != null)
            {
                bool closeIt = PreRemoveTabPage(i);
                if (!closeIt)
                    return;
            }
            TabPages.Remove(TabPages[i]);
        }

        #endregion

        public cMyTabControl(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            this.ResizeRedraw = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            //   Paint the Background
            this.PaintTransparentBackground(e.Graphics, this.ClientRectangle);

            this.PaintAllTheTabs(e);
            this.PaintTheTabPageBorder(e);
            this.PaintTheSelectedTab(e);
        }

        /// <summary>
        /// 画背景
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (this.DesignMode == true)
            {
                LinearGradientBrush backBrush = new LinearGradientBrush(
                            this.Bounds,
                            SystemColors.ControlLightLight,
                            SystemColors.ControlLight,
                            LinearGradientMode.Vertical);
                pevent.Graphics.FillRectangle(backBrush, this.Bounds);
                backBrush.Dispose();
            }
            else
            {
                this.PaintTransparentBackground(pevent.Graphics, this.ClientRectangle);
            }
        }

        #region
        protected void PaintTransparentBackground(Graphics g, Rectangle clipRect)
        {
            if ((this.Parent != null))
            {
                clipRect.Offset(this.Location);
                PaintEventArgs e = new PaintEventArgs(g, clipRect);
                GraphicsState state = g.Save();
                g.SmoothingMode = SmoothingMode.HighSpeed;
                try
                {
                    g.TranslateTransform((float)-this.Location.X, (float)-this.Location.Y);
                    this.InvokePaintBackground(this.Parent, e);
                    this.InvokePaint(this.Parent, e);
                }

                finally
                {
                    g.Restore(state);
                    clipRect.Offset(-this.Location.X, -this.Location.Y);
                }
            }
            else
            {
                System.Drawing.Drawing2D.LinearGradientBrush backBrush = new System.Drawing.Drawing2D.LinearGradientBrush(this.Bounds, SystemColors.ControlLightLight, SystemColors.ControlLight, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                g.FillRectangle(backBrush, this.Bounds);
                backBrush.Dispose();
            }
        }

        private void PaintAllTheTabs(System.Windows.Forms.PaintEventArgs e)
        {
            if (this.TabCount > 0)
            {
                for (int index = 0; index < this.TabCount; index++)
                {
                    this.PaintTab(e, index);
                }
            }
        }

        private void PaintTab(System.Windows.Forms.PaintEventArgs e, int index)
        {
            GraphicsPath path = this.GetPath(index);
            this.PaintTabBackground(e.Graphics, index, path);
            this.PaintTabBorder(e.Graphics, index, path);
            this.PaintTabText(e.Graphics, index);
            this.PaintTabImage(e.Graphics, index);
        }

        private void PaintTabBackground(System.Drawing.Graphics graph, int index, System.Drawing.Drawing2D.GraphicsPath path)
        {
            Rectangle rect = this.GetTabRect(index);
            System.Drawing.Brush buttonBrush;


            if (index == this.SelectedIndex)
            {
                buttonBrush = new System.Drawing.SolidBrush(SystemColors.InactiveBorder);
            }
            else
            {
                buttonBrush = new System.Drawing.SolidBrush(SystemColors.Control);
            }

            graph.FillPath(buttonBrush, path);
            buttonBrush.Dispose();
        }

        private void PaintTabBorder(System.Drawing.Graphics graph, int index, System.Drawing.Drawing2D.GraphicsPath path)
        {


            if (index != this.SelectedIndex)
            {
                Pen borderPen = new Pen(SystemColors.ControlDark);
                borderPen = new Pen(SystemColors.ControlDark);

                if (path.PathPoints.Length >= 6)
                    graph.DrawLine(borderPen, path.PathPoints[4], path.PathPoints[5]);

                borderPen.Dispose();
            }

        }



        private void PaintTheTabPageBorder(System.Windows.Forms.PaintEventArgs e)
        {
            if (this.TabCount > 0)
            {
                Rectangle borderRect = this.TabPages[0].Bounds;
                borderRect.Inflate(1, 1);
                ControlPaint.DrawBorder(e.Graphics, borderRect, SystemColors.Control, ButtonBorderStyle.None);
                //Pen borderPen = new Pen(SystemColors.ControlDark);
                //Point p1=new Point (borderRect.Left+borderRect.Width ,borderRect.Top );
                //Point p2=new Point (borderRect.Left+borderRect.Width,borderRect.Top +borderRect.Height );
                //e.Graphics.DrawLine(borderPen, p1, p2);
            }
        }

        private void PaintTheSelectedTab(System.Windows.Forms.PaintEventArgs e)
        {
            Rectangle selrect;
            int selrectRight = 0;
            if (this.SelectedIndex == -1)
                return;

            selrect = this.GetTabRect(this.SelectedIndex);
            selrectRight = selrect.Right;
            //e.Graphics.DrawLine(SystemPens.ControlDarkDark, selrect.Left,
            //selrect.Bottom + 1, selrectRight - 2, selrect.Bottom + 1);

            ControlPaint.DrawBorder(e.Graphics, selrect, SystemColors.Control, ButtonBorderStyle.Inset);
        }

        private void PaintTabImage(System.Drawing.Graphics graph, int index)
        {
            Image tabImage = null;
            if (this.TabPages[index].ImageIndex > -1 && this.ImageList != null)
            {
                tabImage = this.ImageList.Images[this.TabPages[index].ImageIndex];
            }
            else if (this.TabPages[index].ImageKey.Trim().Length > 0 && this.ImageList != null)
            {
                tabImage = this.ImageList.Images[this.TabPages[index].ImageKey];
            }
            if (tabImage != null)
            {
                Rectangle rect = this.GetTabRect(index);
                graph.DrawImage(tabImage, rect.Left + 4, rect.Top + 2, tabImage.Width, tabImage.Height);
            }

            //画关闭按钮
            Rectangle r = GetTabRect(index);
            r.Offset(r.Width-10,r.Top+2);
            r.Width = 5;
            r.Height = 5;
            Brush b = new SolidBrush(Color.Black);
            Pen p = new Pen(b);
            graph.DrawLine(p, r.X, r.Y, r.X + r.Width, r.Y + r.Height);
            graph.DrawLine(p, r.X + r.Width, r.Y, r.X, r.Y + r.Height);
        }

        private void PaintTabText(System.Drawing.Graphics graph, int index)
        {
            Rectangle rect = this.GetTabRect(index);
            Rectangle rect2;
            if (this.TabPages[index].ImageIndex > -1 && this.ImageList != null)
                rect2 = new Rectangle(rect.Left + rect.Height, rect.Top + 1, rect.Width - rect.Height, rect.Height);
            else
                rect2 = new Rectangle(rect.Left + 8, rect.Top + 1, rect.Width - 6, rect.Height);


            string tabtext = this.TabPages[index].Text;

            System.Drawing.StringFormat format = new System.Drawing.StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.EllipsisCharacter;

            Brush forebrush = null;

            if (this.TabPages[index].Enabled == false)
            {
                forebrush = SystemBrushes.ControlDark;
            }
            else
            {
                forebrush = SystemBrushes.ControlText;
            }

            Font tabFont = this.Font;
            if (index == this.SelectedIndex)
            {
                tabFont = new Font(this.Font, FontStyle.Regular);
                if (index == 0)
                {
                    rect2 = new Rectangle(rect.Left + rect.Height, rect.Top + 1, rect.Width - rect.Height + 5, rect.Height);
                }
            }

            graph.DrawString(tabtext, tabFont, forebrush, rect2, format);

        }


        private System.Drawing.Drawing2D.GraphicsPath GetPath(int index)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.Reset();

            Rectangle rect = this.GetTabRect(index);


            if (index == this.SelectedIndex)
            {
                path.AddLine(rect.Left, rect.Top, rect.Left, rect.Bottom + 1);
                path.AddLine(rect.Left, rect.Top, rect.Right, rect.Top);
                path.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom + 1);
                path.AddLine(rect.Right, rect.Bottom + 1, rect.Left, rect.Bottom + 1);
            }
            else
            {
                path.AddLine(rect.Left, rect.Top, rect.Left, rect.Bottom + 1);
                path.AddLine(rect.Left, rect.Top, rect.Right, rect.Top);
                path.AddLine(rect.Right + 2, rect.Top, rect.Right + 2, rect.Bottom + 1);
                path.AddLine(rect.Right, rect.Bottom + 1, rect.Left, rect.Bottom + 1);
            }


            return path;
        }

        #endregion

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_SETFONT = 0x30;
        private const int WM_FONTCHANGE = 0x1d;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.OnFontChanged(EventArgs.Empty);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            IntPtr hFont = this.Font.ToHfont();
            SendMessage(this.Handle, WM_SETFONT, hFont, (IntPtr)(-1));
            SendMessage(this.Handle, WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero);
            this.UpdateStyles();
            this.ItemSize = new Size(0, 20);
        }

        #region 事件
        /// <summary>
        /// 采集任务初始化事件
        /// </summary>
        private readonly object e_CloseTabLock = new object();
        private event EventHandler<cCloseTabEvent> e_CloseTab;
        public event EventHandler<cCloseTabEvent> CloseTab
        {
            add { lock (e_CloseTabLock) { e_CloseTab += value; } }
            remove { lock (e_CloseTabLock) { e_CloseTab -= value; } }
        }
        #endregion

    }

    public class cCloseTabEvent : EventArgs
    {

        public cCloseTabEvent(int index)
        {
            m_Index = index;
        }

        /// <param name="cancel">是否取消事件</param>
        public cCloseTabEvent(bool cancel,int index)
        {
            m_Cancel = cancel;
            m_Index = index;
        }

        private int m_Index;
        public int Index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

        private bool m_Cancel;
        /// <summary>
        /// 是否取消事件
        /// </summary>
        public bool Cancel
        {
            get { return m_Cancel; }
            set { m_Cancel = value; }
        }
    }
}

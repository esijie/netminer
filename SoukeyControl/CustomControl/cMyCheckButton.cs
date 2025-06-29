using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SoukeyControl.CustomControl
{
    public partial class cMyCheckButton : System.Windows.Forms.Button
    {

        public cMyCheckButton()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        public cMyCheckButton(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private bool m_Checked;
        [Browsable(true), Category("Checked"), Description("表示当前是否选中")]
        public bool Checked
        {
            get { return m_Checked; }
            set { m_Checked = value; Invalidate(); }
        }

        private bool isHovered;
        private bool isFocused;
        private bool isFocusedByKey;
        private bool isKeyDown;
        private bool isMouseDown;
        private bool isPressed { get { return isKeyDown || (isMouseDown && isHovered); } }

        private int direction;

        protected override void OnPaint(PaintEventArgs pevent)
        {
            DrawButtonBackground(pevent.Graphics);
            if (this.Checked == true)
                PaintTheSelectedTab(pevent.Graphics);

            PaintTabText(pevent.Graphics);
            PaintTabImage(pevent.Graphics);
        }

        private void DrawButtonBackground(Graphics g)
        {
            Brush ContentBrush =null;

          
                if (this.Checked == true)
                    ContentBrush = new System.Drawing.SolidBrush(SystemColors.InactiveBorder);
                else
                    ContentBrush = new System.Drawing.SolidBrush(SystemColors.Control);
            


            GraphicsPath ContentPath = new GraphicsPath();

            Rectangle rect;
            rect = ClientRectangle;
            ContentPath.AddRectangle(rect);

            //content
            g.FillPath(ContentBrush, ContentPath);

        }

        private void PaintTheSelectedTab(Graphics g)
        {
            Rectangle selrect=ClientRectangle;

            ControlPaint.DrawBorder(g, selrect, SystemColors.Control, ButtonBorderStyle.Inset);
        }

        private void PaintTabText(System.Drawing.Graphics graph)
        {
            string tabtext = this.Text;

            System.Drawing.StringFormat format = new System.Drawing.StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.EllipsisCharacter;

            Brush forebrush = null;
            forebrush = SystemBrushes.ControlText;

            Font tabFont = this.Font;

            Rectangle rect;
            rect = ClientRectangle;
            Rectangle rect2;
            if (this.Image !=null)
                rect2 = new Rectangle(rect.Left + rect.Height, rect.Top + 1, rect.Width - rect.Height, rect.Height);
            else
                rect2 = new Rectangle(rect.Left + 4, rect.Top + 1, rect.Width - 6, rect.Height);

            graph.DrawString(tabtext, tabFont, forebrush, rect2, format);

        }

        private void PaintTabImage(System.Drawing.Graphics graph)
        {
            Image tabImage = this.Image;
         
            if (tabImage != null)
            {
                Rectangle rect = ClientRectangle;
                graph.DrawImage(tabImage, rect.Left + 4, rect.Top + 3, tabImage.Width, tabImage.Height);
            }

         
        }

        #region " Overrided Methods "

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Click" /> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected override void OnClick(EventArgs e)
        {
            isKeyDown = isMouseDown = false;

            if (this.Checked == false)
                this.Checked = true;
          

            Invalidate();
            base.OnClick(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Enter" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnEnter(EventArgs e)
        {
            isFocused = isFocusedByKey = true;
            base.OnEnter(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Leave" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            isFocused = isFocusedByKey = isKeyDown = isMouseDown = false;
            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="M:System.Windows.Forms.ButtonBase.OnKeyUp(System.Windows.Forms.KeyEventArgs)" /> event.
        /// </summary>
        /// <param name="kevent">A <see cref="T:System.Windows.Forms.KeyEventArgs" /> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (kevent.KeyCode == Keys.Space)
            {
                isKeyDown = true;
                Invalidate();
            }
            base.OnKeyDown(kevent);
        }

        /// <summary>
        /// Raises the <see cref="M:System.Windows.Forms.ButtonBase.OnKeyUp(System.Windows.Forms.KeyEventArgs)" /> event.
        /// </summary>
        /// <param name="kevent">A <see cref="T:System.Windows.Forms.KeyEventArgs" /> that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            if (isKeyDown && kevent.KeyCode == Keys.Space)
            {
                isKeyDown = false;
                Invalidate();
            }
            base.OnKeyUp(kevent);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!isMouseDown && e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                isFocusedByKey = false;
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isMouseDown)
            {
                isMouseDown = false;
                Invalidate();
            }
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Raises the <see cref="M:System.Windows.Forms.Control.OnMouseMove(System.Windows.Forms.MouseEventArgs)" /> event.
        /// </summary>
        /// <param name="mevent">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);
            if (mevent.Button != MouseButtons.None)
            {
                if (!ClientRectangle.Contains(mevent.X, mevent.Y))
                {
                    if (isHovered)
                    {
                        isHovered = false;
                        Invalidate();
                    }
                }
                else if (!isHovered)
                {
                    isHovered = true;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseEnter" /> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave" /> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }
     
        #endregion

      
    }
}

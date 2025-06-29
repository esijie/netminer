using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using NetMiner.Common;

namespace MinerSpider
{
    public partial class frmFirst : Form
    {

        public frmFirst()
        {
            InitializeComponent();

            this.CloseTab += this.CloseWindows;
        }

        #region 画背景及关闭按钮
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

                PaintBkImage(pevent.Graphics);
            }
        }

        protected void PaintTransparentBackground(Graphics g, Rectangle clipRect)
        {
            
            System.Drawing.Drawing2D.LinearGradientBrush backBrush = new System.Drawing.Drawing2D.LinearGradientBrush(this.Bounds, SystemColors.ControlLightLight, SystemColors.ControlLight, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
            g.FillRectangle(backBrush, this.Bounds);
            backBrush.Dispose();
            
        }

        private void PaintBkImage(System.Drawing.Graphics graph)
        {
            Image bImage = (Image)Properties.Resources.ResourceManager.GetObject("bg");

            Rectangle rect = new Rectangle(this.Left, this.Top, this.Width, this.Height);
            graph.DrawImage(bImage,0, 0, bImage.Width, bImage.Height);

            //画边框
            Pen borderPen = new Pen(SystemColors.ControlDark);
            borderPen = new Pen(SystemColors.ControlDark);

            GraphicsPath path = GetPath();

            graph.DrawLine(borderPen, path.PathPoints[4], path.PathPoints[5]);

            borderPen.Dispose();

            //画关闭按钮
            DrawClose(graph,Color.White);
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    Graphics g = e.Graphics;

        //    Point p = MousePosition;
        //    p.X = p.X - this.Left;
        //    p.Y = p.Y - this.Top;

        //    Rectangle r = new Rectangle(0, 0, this.Width, this.Height);
        //    r.Offset(r.Width - 20, r.Top + 12);
        //    r.Width = 10;
        //    r.Height = 10;
        //    if (r.Contains(p))
        //    {
        //        DrawClose(g,Color.Red);
        //    }
        //    else
        //        DrawClose(g,Color.White);
        //}

        private void DrawClose(System.Drawing.Graphics graph, Color cColor)
        {
            Rectangle r = new Rectangle(0, 0, this.Width, this.Height);
            r.Offset(r.Width - 20, r.Top + 8);
            r.Width = 8;
            r.Height = 8;
            Brush b = new SolidBrush(cColor);
            Pen p = new Pen(b, 2);
            graph.DrawLine(p, r.X, r.Y, r.X + r.Width, r.Y + r.Height);
            graph.DrawLine(p, r.X + r.Width, r.Y, r.X, r.Y + r.Height);
        }

        private System.Drawing.Drawing2D.GraphicsPath GetPath()
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.Reset();

            Rectangle rect = new Rectangle(this.Left, this.Top, this.Width, this.Height);


           
            path.AddLine(rect.Left, rect.Top, rect.Left, rect.Bottom + 1);
            path.AddLine(rect.Left, rect.Top, rect.Right, rect.Top);
            path.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom + 1);
            path.AddLine(rect.Right, rect.Bottom + 1, rect.Left, rect.Bottom + 1);
         


            return path;
        }

        private void glassButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmFirst_Load(object sender, EventArgs e)
        {
            if (Program.RegisterUser == "sominer")
            {
                this.label2.Visible = true ;
                this.glassButton2.Visible = true ;
            }
            else
            {
                this.label2.Visible = false;
                this.glassButton2.Visible = false;
            }
        }

        #endregion

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Point p = e.Location;

            Rectangle r = new Rectangle(0,0, this.Width, this.Height);
            r.Offset(r.Width - 20, r.Top + 8);
            r.Width = 10;
            r.Height = 10;
            if (r.Contains(p))
            {
                //触发关闭事件
                if (e_CloseTab != null)
                {
                    e_CloseTab(this, new cCloseEvent());
                }
            }
            
        }

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    Point p = e.Location;
            
        //    Rectangle r = new Rectangle(0, 0, this.Width, this.Height);
        //    r.Offset(r.Width - 20, r.Top + 12);
        //    r.Width = 10;
        //    r.Height = 10;
        //    if (r.Contains(p))
        //    {
        //        DrawClose(Color.Red);
        //    }
        //    else
        //        DrawClose(Color.White);
        //}

        private void CloseWindows(object sender, cCloseEvent e)
        {
            this.Close();
        }

        #region 事件
        /// <summary>
        /// 采集任务初始化事件
        /// </summary>
        private readonly object e_CloseTabLock = new object();
        private event EventHandler<cCloseEvent> e_CloseTab;
        public event EventHandler<cCloseEvent> CloseTab
        {
            add { lock (e_CloseTabLock) { e_CloseTab += value; } }
            remove { lock (e_CloseTabLock) { e_CloseTab -= value; } }
        }
        #endregion

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            cXmlSConfig m_config = null;
            m_config = new cXmlSConfig(Program.getPrjPath());
            m_config.IsFirstRun = !this.checkBox1.Checked;
            m_config = null;
        }

       
    }

    public class cCloseEvent : EventArgs
    {
        public cCloseEvent()
        {
        }

        /// <param name="cancel">是否取消事件</param>
        public cCloseEvent(bool cancel)
        {
            m_Cancel = cancel;
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

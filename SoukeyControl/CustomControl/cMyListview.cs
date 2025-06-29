using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SoukeyControl.CustomControl
{
    public partial class cMyListview : ListView
    {
        public cMyListview()
        {
            InitializeComponent();

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles(); 
        }

        public cMyListview(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles(); 
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Point p = e.Location;
            for (int i=0;i<this.Items.Count ;i++)
            {
                Rectangle r = GetItemRect(i);
                r.Width = 16;
                r.Height = 16;

                if (r.Contains(p))
                {
                    //触发点击图片事件
                    if (e_ImageClick != null)
                    {
                        e_ImageClick(this, new cImageClickEvent(i));
                        break;
                    }
                }
            }

            base.OnMouseClick(e);
        }

        private readonly object e_ImageClickLock = new object();
        private event EventHandler<cImageClickEvent> e_ImageClick;
        public event EventHandler<cImageClickEvent> ImageClick
        {
            add { lock (e_ImageClickLock) { e_ImageClick += value; } }
            remove { lock (e_ImageClickLock) { e_ImageClick -= value; } }
        }
    }

    public class cImageClickEvent : EventArgs
    {
        public cImageClickEvent(int index)
        {
            m_Index = index;
        }

        private int m_Index;
        public int Index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

    }
}

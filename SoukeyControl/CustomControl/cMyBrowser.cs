using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Gecko;

namespace SoukeyControl.CustomControl
{
    public partial class cMyBrowser :  Gecko.GeckoWebBrowser
    {
        private bool m_BlockEvent = false;

        private const int WM_SETFONT = 0x30;
        private const int WM_FONTCHANGE = 0x1d;
        private const int WM_MBUTTONDBLCLK = 0x209;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_MENUGETOBJECT = 0x124;
        private const int WM_MENURBUTTONUP = 0x122;
        private const int WM_MOUSEACTIVATE = 0x21;
        private const int WM_MOUSEHOVER = 0x2A1;
        private const int WM_MOUSELEAVE = 0x2A3;
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_SETCURSOR = 0x0020;
        private const int WM_LBUTTONDOWN = 0x0201;

        public bool BlockEvent
        {
            get { return m_BlockEvent; }
            set { m_BlockEvent = value; }
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m_BlockEvent == false)
            {
                switch (m.Msg)
                {
                    case WM_SETCURSOR:
                        base.WndProc(ref m);
                        break;
                    case WM_MOUSEACTIVATE:

                        break;
                    case WM_LBUTTONDOWN:

                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }

            }
        }

        public cMyBrowser()
        {
            InitializeComponent();
        }

        public cMyBrowser(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}

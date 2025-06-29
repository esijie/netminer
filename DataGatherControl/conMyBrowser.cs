using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SHDocVw;
using System.Runtime.InteropServices;
using System.Xml.XPath;

namespace DataGatherControl
{
    [ComVisible(true), StructLayout(LayoutKind.Sequential)]
    public struct tagRECT
    {
        [MarshalAs(UnmanagedType.I4)]
        public int Left;
        [MarshalAs(UnmanagedType.I4)]
        public int Top;
        [MarshalAs(UnmanagedType.I4)]
        public int Right;
        [MarshalAs(UnmanagedType.I4)]
        public int Bottom;

        public tagRECT(int left_, int top_, int right_, int bottom_)
        {
            Left = left_;
            Top = top_;
            Right = right_;
            Bottom = bottom_;
        }

    }

    public partial class conMyBrowser : UserControl
    {
        private enum MiscCommandTarget
        {
            Find = 1001,
            ViewSource = 1002,
            Options = 1003,
        }

        private Guid cmdGuid = new Guid("ED016940-BD5B-11CF-BA4E-00C04FD70816");
        private bool _FirstShown;
        private AxSHDocVw.AxWebBrowser _ActiveWebBrowser;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern bool InvalidateRect(IntPtr hwnd, ref tagRECT lpRect, bool bErase);

        IntPtr m_hwnd;
        Pen m_browserPen;

        Rectangle m_elemRect;
        HtmlElement m_curElem;

        XPathDocument xDoc;

        HtmlDocument m_hDoc;

        public conMyBrowser()
        {
            InitializeComponent();

            _ActiveWebBrowser = this.axWebBrowser1;
        }

        public void Navigate(string url)
        {
         
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Object o = null;

                _ActiveWebBrowser.Navigate(url, ref o, ref o, ref o, ref o);

            }
            finally
            {

                Cursor.Current = Cursors.Default;
            }
        }

        private string m_Title;
        public string Title
        {
            get { return m_Title; }
        }


        public string GetHtmlSource(int index)
        {
            mshtml.HTMLDocument hDoc = GetDocument();
            return hDoc.documentElement.innerHTML;
        }

        private void axWebBrowser1_ProgressChange(object sender, AxSHDocVw.DWebBrowserEvents2_ProgressChangeEvent e)
        {
            AxSHDocVw.AxWebBrowser axWebBrowser1 = (AxSHDocVw.AxWebBrowser)sender;
            HE_WebBrowserTag _HE_WebBrowserTag = (HE_WebBrowserTag)axWebBrowser1.Tag;

            ProgressBar1.Visible = true;
            if ((e.progress > 0) && (e.progressMax > 0))
            {
                ProgressBar1.Maximum = e.progressMax;
                ProgressBar1.Step = e.progress;
                ProgressBar1.PerformStep();
            }
            else if (axWebBrowser1.ReadyState == SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
            {
                ProgressBar1.Value = 0;
                ProgressBar1.Visible = false;
            }
        }

        private void axWebBrowser1_BeforeNavigate2(object sender, AxSHDocVw.DWebBrowserEvents2_BeforeNavigate2Event e)
        {
            HE_WebBrowserTag _HE_WebBrowserTag = (HE_WebBrowserTag)_ActiveWebBrowser.Tag;

            this.staInfo.Text = "正在加载，请等待......";
        }

        private void axWebBrowser1_DocumentComplete(object sender, AxSHDocVw.DWebBrowserEvents2_DocumentCompleteEvent e)
        {
            if (_ActiveWebBrowser.ReadyState == SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
            {
                //if the selected tab matches the current webbrowser then update status text with the
                //current URL location.
                HE_WebBrowserTag _HE_WebBrowserTag = (HE_WebBrowserTag)_ActiveWebBrowser.Tag;
                this.staInfo.Text = _ActiveWebBrowser.LocationURL; 

                ProgressBar1.Value = 0;
                ProgressBar1.Visible = false;

            }
        }

        private void axWebBrowser1_StatusTextChange(object sender, AxSHDocVw.DWebBrowserEvents2_StatusTextChangeEvent e)
        {
            AxSHDocVw.AxWebBrowser _AxWebBrowser = (AxSHDocVw.AxWebBrowser)sender;
            HE_WebBrowserTag _HE_WebBrowserTag = (HE_WebBrowserTag)_AxWebBrowser.Tag;

            this.staInfo.Text = e.text;
        }

        private mshtml.HTMLDocument GetDocument()
        {
            try
            {
                mshtml.HTMLDocument htm = (mshtml.HTMLDocument)_ActiveWebBrowser;
                return htm;

            }
            catch
            {

            }

            return null;
        }

        private void WebBrowserViewSource()
        {
            IOleCommandTarget cmdt;
            Object o = new object();
            try
            {
                cmdt = (IOleCommandTarget)GetDocument();
                cmdt.Exec(ref cmdGuid, (uint)MiscCommandTarget.ViewSource,
                    (uint)SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref o, ref o);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }

        private void ExecCommandID(SHDocVw.OLECMDID _OLECMDID)
        {
            int response = (int)_ActiveWebBrowser.QueryStatusWB(_OLECMDID);
            bool IsOK = (response & (int)SHDocVw.OLECMDF.OLECMDF_ENABLED) != 0 ? true : false;
            if (IsOK == false) { return; }

            Object o = null;
            _ActiveWebBrowser.ExecWB(_OLECMDID, SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref o, ref o);
        }

        private void axWebBrowser1_NavigateError(object sender, AxSHDocVw.DWebBrowserEvents2_NavigateErrorEvent e)
        {
            HE_WebBrowserTag _HE_WebBrowserTag = (HE_WebBrowserTag)_ActiveWebBrowser.Tag;

            Cursor.Current = Cursors.Default;
        }

        private void axWebBrowser1_NavigateComplete2(object sender, AxSHDocVw.DWebBrowserEvents2_NavigateComplete2Event e)
        {
            HE_WebBrowserTag _HE_WebBrowserTag = (HE_WebBrowserTag)_ActiveWebBrowser.Tag;

            this.staInfo.Text = "";
        }

        private void axWebBrowser1_NewWindow2(object sender, AxSHDocVw.DWebBrowserEvents2_NewWindow2Event e)
        {
            e.ppDisp = this._ActiveWebBrowser.Application;
            _ActiveWebBrowser.RegisterAsBrowser = true;
        }

        private void axWebBrowser1_CommandStateChange(object sender, AxSHDocVw.DWebBrowserEvents2_CommandStateChangeEvent e)
        {
            AxSHDocVw.AxWebBrowser _AxWebBrowser = (AxSHDocVw.AxWebBrowser)sender;
            HE_WebBrowserTag _HE_WebBrowserTag;
            _HE_WebBrowserTag = (HE_WebBrowserTag)_AxWebBrowser.Tag;

            //Set enabled property for the Forward/Backward functions.
            switch (e.command)
            {
                case ((int)CommandStateChangeConstants.CSC_NAVIGATEFORWARD):
                    break;

                case ((int)CommandStateChangeConstants.CSC_NAVIGATEBACK):
                    break;

                default:
                    break;
            }
            _AxWebBrowser.Tag = _HE_WebBrowserTag;
        }

        private void axWebBrowser1_TitleChange(object sender, AxSHDocVw.DWebBrowserEvents2_TitleChangeEvent e)
        {
            //Set corresponding tab text with the corresponding location name
            AxSHDocVw.AxWebBrowser _AxWebBrowser = (AxSHDocVw.AxWebBrowser)sender;
            HE_WebBrowserTag _HE_WebBrowserTag;
            _HE_WebBrowserTag = (HE_WebBrowserTag)_AxWebBrowser.Tag;
            m_Title = e.text;
        }

    
    }
}

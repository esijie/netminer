using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SHDocVw;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct OLECMDTEXT
{
    public uint cmdtextf;
    public uint cwActual;
    public uint cwBuf;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
    public char rgwz;
}

[StructLayout(LayoutKind.Sequential)]
public struct OLECMD
{
    public uint cmdID;
    public uint cmdf;
}

[ComImport,
Guid("b722bccb-4e68-101b-a2bc-00aa00404770"),
InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IOleCommandTarget
{
 
    void QueryStatus(ref Guid pguidCmdGroup, UInt32 cCmds,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] OLECMD[] prgCmds, ref OLECMDTEXT CmdText);
    void Exec(ref Guid pguidCmdGroup, uint nCmdId, uint nCmdExecOpt, ref object pvaIn, ref object pvaOut);
}

namespace DataGatherControl
{
    public partial class conBrowser : UserControl
    {
        private enum MiscCommandTarget
        { 
            Find = 1001, 
            ViewSource=1002, 
            Options=1003, 
        }

        private Guid cmdGuid = new Guid("ED016940-BD5B-11CF-BA4E-00C04FD70816");
        private bool _FirstShown;
        private System.Windows.Forms.MenuItem mniPageSetup;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.ToolBarButton tblNewTab;
        public StatusBar statusBar1;
        private AxSHDocVw.AxWebBrowser _ActiveWebBrowser;

        /// <summary>
        /// 构造函数
        /// </summary>
        public conBrowser()
        {
            InitializeComponent();

            axWebBrowser1.Tag = new HE_WebBrowserTag();
            SetActiveWebBrowser();
        }

        /// <summary>
        /// 指定url，打开网页
        /// </summary>
        /// <param name="url">网页地址</param>
        /// <param name="isNewWindow">是否在新窗口打开，true-新窗口打开；false-在原窗口打开</param>
        public void GoUrl(string url,bool isNewWindow)
        {
            if (url == "" && this.txtUrl.Text == "")
                return;

            if (this.txtUrl.Text == "")
                this.txtUrl.Text = url;
            else if (url == "")
                url = this.txtUrl.Text;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Object o = null;

                if (isNewWindow == true)
                {
                    AxSHDocVw.AxWebBrowser nBrowser= CreateNewWebBrowser();
                    nBrowser .Navigate(url, ref o, ref o, ref o, ref o);
                }
                else
                {
                    _ActiveWebBrowser.Navigate(url, ref o, ref o, ref o, ref o);
                }

            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public void LoginWebSite(string loginUrl, string user, string pwd,int index)
        {
            if (index > WebbrowserCount)
                return;

            TabPage _TabPage;
            _TabPage = tab.TabPages[index];

            if (index == this.tab.SelectedIndex)
                this.txtUrl.Text = loginUrl;

            for (int i = 0; i < _TabPage.Controls.Count; i++)
            {
                //获取浏览器控件
                if (_TabPage.Controls[i].GetType().Name == "AxWebBrowser")
                {
                    _ActiveWebBrowser = (AxSHDocVw.AxWebBrowser)_TabPage.Controls[i];
                    HE_WebBrowserTag _HE_WebBrowserTag;

                    _HE_WebBrowserTag = (HE_WebBrowserTag)_ActiveWebBrowser.Tag;
                    _HE_WebBrowserTag._TabIndex = index;

                    staInfo.Text = _ActiveWebBrowser.LocationURL;
                    txtUrl.Text = _ActiveWebBrowser.LocationURL;
                   
                    break;
                }
            }

            try
            {
                Object o = null;
                _ActiveWebBrowser.Navigate(loginUrl, ref o, ref o, ref o, ref o);

            }
            catch { }
        }

        /// <summary>
        /// 在制定顺序的浏览器上打开制定的地址
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="index"></param>
        public void GoUrl(string Url, int index)
        {
            if (index > WebbrowserCount)
                return;

            TabPage _TabPage;
            _TabPage = tab.TabPages[index];

            if (index == this.tab.SelectedIndex)
                this.txtUrl.Text = Url;

            for (int i = 0; i < _TabPage.Controls.Count; i++)
            {
                //获取浏览器控件
                if (_TabPage.Controls[i].GetType().Name == "AxWebBrowser")
                {
                    _ActiveWebBrowser = (AxSHDocVw.AxWebBrowser)_TabPage.Controls[i];
                    HE_WebBrowserTag _HE_WebBrowserTag;

                    _HE_WebBrowserTag = (HE_WebBrowserTag)_ActiveWebBrowser.Tag;
                    _HE_WebBrowserTag._TabIndex = index;

                    staInfo.Text = _ActiveWebBrowser.LocationURL;
                    txtUrl.Text = _ActiveWebBrowser.LocationURL;
                   
                    break;
                }
            }

            try
            {
                Object o = null;
                _ActiveWebBrowser.Navigate(Url, ref o, ref o, ref o, ref o);
            }
            catch { }

        }

        /// <summary>
        /// 关闭指定的浏览器
        /// </summary>
        /// <param name="index"></param>
        public void CloseWebbrowser(int index)
        {
            tab.TabPages[index].Dispose();
        }

        /// <summary>
        /// 关闭所有浏览器，只留下第一个浏览器，第一个浏览器默认不能被关闭
        /// </summary>
        public void CloseAllWebbrowser()
        {
            for (int i = 1; i < tab.TabPages.Count; i++)
            {
                tab.TabPages[i].Dispose();
            }
        }

        /// <summary>
        /// 返回浏览器个数
        /// </summary>
        public int WebbrowserCount
        {
            get { return tab.TabPages.Count; }
        }

        /// <summary>
        /// 获取指定浏览器显示页面的源码
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetHtmlSource(int index)
        {
            mshtml.HTMLDocument hDoc = GetDocument(index );
            //return hDoc.documentElement.innerHTML;
            return hDoc.documentElement.outerHTML;
        }

        public SHDocVw.tagREADYSTATE GetBrowserState(int index)
        {
            TabPage _TabPage;
            _TabPage = tab.TabPages[index];

            for (int i = 0; i < _TabPage.Controls.Count; i++)
            {
                //获取浏览器控件
                if (_TabPage.Controls[i].GetType().Name == "AxWebBrowser")
                {
                    return ((AxSHDocVw.AxWebBrowser)_TabPage.Controls[i]).ReadyState;
                }
            }

            return tagREADYSTATE.READYSTATE_COMPLETE;
           
        }

        private void SetActiveWebBrowser()
        {
            TabPage _TabPage;
            _TabPage = tab.SelectedTab;

            for (int i = 0; i < _TabPage.Controls.Count; i++)
            {
                //获取浏览器控件
                if (_TabPage.Controls[i].GetType().Name == "AxWebBrowser")
                {
                    _ActiveWebBrowser = (AxSHDocVw.AxWebBrowser)_TabPage.Controls[i];
                    HE_WebBrowserTag _HE_WebBrowserTag;

                    _HE_WebBrowserTag = (HE_WebBrowserTag)_ActiveWebBrowser.Tag;
                    _HE_WebBrowserTag._TabIndex = tab.SelectedIndex;
                    staInfo.Text = _ActiveWebBrowser.LocationURL;
                    txtUrl.Text = _ActiveWebBrowser.LocationURL;
                    break;
                }
            }
        }

        private void axWebBrowser1_ProgressChange(object sender, AxSHDocVw.DWebBrowserEvents2_ProgressChangeEvent e)
        {
            AxSHDocVw.AxWebBrowser axWebBrowser1 = (AxSHDocVw.AxWebBrowser)sender;
            HE_WebBrowserTag _HE_WebBrowserTag = (HE_WebBrowserTag)axWebBrowser1.Tag;

            //If current tab page does not match current webbrowser then leave it.
            if (_HE_WebBrowserTag._TabIndex != tab.SelectedIndex) { return; }

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
            if (_HE_WebBrowserTag._TabIndex != tab.SelectedIndex) { return; }

            this.staInfo.Text = "正在加载，请等待......";
        }

        private void axWebBrowser1_DocumentComplete(object sender, AxSHDocVw.DWebBrowserEvents2_DocumentCompleteEvent e)
        {
            if (_ActiveWebBrowser.ReadyState == SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
            {
                //if the selected tab matches the current webbrowser then update status text with the
                //current URL location.
                HE_WebBrowserTag _HE_WebBrowserTag = (HE_WebBrowserTag)_ActiveWebBrowser.Tag;
                if (_HE_WebBrowserTag._TabIndex == tab.SelectedIndex)
                { this.staInfo.Text = _ActiveWebBrowser.LocationURL; }

                ProgressBar1.Value = 0;
                ProgressBar1.Visible = false;

            }	
        }

        private void axWebBrowser1_StatusTextChange(object sender, AxSHDocVw.DWebBrowserEvents2_StatusTextChangeEvent e)
        {
            AxSHDocVw.AxWebBrowser _AxWebBrowser = (AxSHDocVw.AxWebBrowser)sender;
            HE_WebBrowserTag _HE_WebBrowserTag = (HE_WebBrowserTag)_AxWebBrowser.Tag;
            if (_HE_WebBrowserTag._TabIndex != tab.SelectedIndex)
            {
                this.staInfo.Text = "";
                return;
            }
            else
                this.staInfo.Text = e.text;
        }

        /// <summary>
        /// 指定获取某个浏览器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public mshtml.HTMLDocument GetDocument(int index)
        {
            try
            {

                TabPage _TabPage;
                _TabPage = tab.TabPages[index];

                for (int i = 0; i < _TabPage.Controls.Count; i++)
                {
                    //获取浏览器控件
                    if (_TabPage.Controls[i].GetType().Name == "AxWebBrowser")
                    {
                        mshtml.HTMLDocument htm = (mshtml.HTMLDocument)((AxSHDocVw.AxWebBrowser)_TabPage.Controls[i]).Document ;
                        return htm;
                    }
                }
                
            }
            catch
            {
                
            }

            return null;
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

        private  void WebBrowserViewSource()
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

        private  void WebBrowserInternetOptions()
        {
            IOleCommandTarget cmdt;
            Object o = new object();
            try
            {
                cmdt = (IOleCommandTarget)GetDocument();
                cmdt.Exec(ref cmdGuid, (uint)MiscCommandTarget.Options,
                    (uint)SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref o, ref o);
            }
            catch
            {
                // NOTE: Because of the way that this CMDID is handled in Internet Explorer,
                // this catch block will always fire, even though the dialog box
                // and its operations completed successfully. You can suppress this
                // error without causing any damage to your host.
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
            if (_HE_WebBrowserTag._TabIndex != tab.SelectedIndex) { return; }

            Cursor.Current = Cursors.Default;
         
        }

        private void axWebBrowser1_NavigateComplete2(object sender, AxSHDocVw.DWebBrowserEvents2_NavigateComplete2Event e)
        {
            HE_WebBrowserTag _HE_WebBrowserTag = (HE_WebBrowserTag)_ActiveWebBrowser.Tag;
            if (_HE_WebBrowserTag._TabIndex != tab.SelectedIndex) { return; }
         
            this.staInfo.Text = "";
        }

        /// <summary>
        /// 创建一个新的浏览器
        /// </summary>
        /// <returns></returns>
        private AxSHDocVw.AxWebBrowser CreateNewWebBrowser()
        {
            AxSHDocVw.AxWebBrowser _axWebBrowser = new AxSHDocVw.AxWebBrowser();
            _axWebBrowser.Tag = new HE_WebBrowserTag();
            TabPage _TabPage = new TabPage();
            _TabPage.Controls.Add(_axWebBrowser);

            _axWebBrowser.Dock = DockStyle.Fill;
            _axWebBrowser.BeforeNavigate2 += new AxSHDocVw.DWebBrowserEvents2_BeforeNavigate2EventHandler(this.axWebBrowser1_BeforeNavigate2);
            _axWebBrowser.DocumentComplete += new AxSHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(this.axWebBrowser1_DocumentComplete);
            _axWebBrowser.NavigateComplete2 += new AxSHDocVw.DWebBrowserEvents2_NavigateComplete2EventHandler(this.axWebBrowser1_NavigateComplete2);
            _axWebBrowser.NavigateError += new AxSHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(this.axWebBrowser1_NavigateError);
            _axWebBrowser.NewWindow2 += new AxSHDocVw.DWebBrowserEvents2_NewWindow2EventHandler(this.axWebBrowser1_NewWindow2);
            _axWebBrowser.ProgressChange += new AxSHDocVw.DWebBrowserEvents2_ProgressChangeEventHandler(this.axWebBrowser1_ProgressChange);
            _axWebBrowser.StatusTextChange += new AxSHDocVw.DWebBrowserEvents2_StatusTextChangeEventHandler(this.axWebBrowser1_StatusTextChange);
            _axWebBrowser.TitleChange += new AxSHDocVw.DWebBrowserEvents2_TitleChangeEventHandler(this.axWebBrowser1_TitleChange);
            _axWebBrowser.CommandStateChange += new AxSHDocVw.DWebBrowserEvents2_CommandStateChangeEventHandler(this.axWebBrowser1_CommandStateChange);

            tab.TabPages.Add(_TabPage);
            tab.SelectedTab = _TabPage;
           
            return _axWebBrowser;
        }

        private void axWebBrowser1_NewWindow2(object sender, AxSHDocVw.DWebBrowserEvents2_NewWindow2Event e)
        {
            AxSHDocVw.AxWebBrowser _axWebBrowser = CreateNewWebBrowser();
            e.ppDisp = _axWebBrowser.Application;
            _axWebBrowser.RegisterAsBrowser = true;
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
            tab.TabPages[_HE_WebBrowserTag._TabIndex].Text = e.text;
        }

        private void tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetActiveWebBrowser();
        }

        private void txtUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _ActiveWebBrowser.Navigate(this.txtUrl.Text);
            }
        }
    }

    /// <summary>
    /// tab浏览器信息记录类，记录当前的索引
    /// </summary>
    public class HE_WebBrowserTag
    {
        public int _TabIndex = 0;
    }
}

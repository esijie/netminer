using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace NetMiner.Gather.Url
{
    public partial class frmExcuteScript : Form
    {
        private string m_Script = string.Empty;
        //public delegate void ReturnValue(object pValue);
        //public ReturnValue rValue;
        private object m_pValue = string.Empty;

        private bool m_isWorking = false;

        public frmExcuteScript(string strScript)
        {
            InitializeComponent();

            m_Script = strScript;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);

        }

        private void frmExcuteScript_Load(object sender, EventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (this.webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            {
                return;
            }

            //完成后加载
            HtmlElement script = webBrowser1.Document.CreateElement("script");
            script.SetAttribute("type", "text/javascript");
            script.SetAttribute("text", "function _func(){return window.aes;}");
            HtmlElement head = webBrowser1.Document.Body.AppendChild(script);

            m_pValue = webBrowser1.Document.InvokeScript("_func", null);

            m_isWorking = false;
        }

        public object GotoUrl(string htmlSource)
        {
            m_isWorking = true;
            //this.webBrowser1.Navigate("http://weixin.sogou.com/gzh?openid=oIWsFtyedmfKjMMHQl9W-L8MeiJo");
            this.webBrowser1.DocumentText = htmlSource;
            while (m_isWorking)
            {
                Thread.Sleep(50);
                Application.DoEvents();
            }

            ////完成后加载
            //HtmlElement script = webBrowser1.Document.CreateElement("script");
            //script.SetAttribute("type", "text/javascript");
            //script.SetAttribute("text", "function _func(){return window.aes;}");
            //HtmlElement head = webBrowser1.Document.Body.AppendChild(script);

            //m_pValue = webBrowser1.Document.InvokeScript("_func", null);

            return m_pValue;
        }
    }
}

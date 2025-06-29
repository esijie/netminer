using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetMiner.Gather;
using NetMiner.Gather.Control;
using NetMiner.Resource;
using System.Web;
using System.Net;
using NetMiner.Core.Proxy;
using NetMiner.Common;
using NetMiner.Core.gTask.Entity;

namespace SMHttpComposer
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

      

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Request();
        }

        private void Request()
        {
            string url=this.txtUrl.Text ;
            if (!url.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) && !url.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
            {
                url = "http://" + url;
            }

            if (raPOST.Checked ==true )
            {
                url+="<POST:" + this.comPCode.Text + ">" + this.txtPostData.Text.Trim () + "</POST>";
            }

            cGlobalParas.WebCode wCode=cGlobalParas.WebCode.auto;
            switch (this.comwCode.Text )
            {
                case "Auto":
                    wCode=cGlobalParas.WebCode.auto;
                    break ;
                case"UTF-8":
                    wCode =cGlobalParas.WebCode.utf8 ;
                    break ;
                case"gb2312":
                    wCode =cGlobalParas.WebCode.gb2312 ;
                    break ;
                case"big5":
                    wCode =cGlobalParas.WebCode.big5 ;
                    break ;
                case"gbk":
                    wCode =cGlobalParas.WebCode.gbk ;
                    break ;
            }

            string webSource="";

            cGatherWeb g = null;

            if (raCustomProxy.Checked == true)
            {
                cProxyControl pControl = new cProxyControl(Program.getPrjPath () , this.txtProxyAddress.Text.Trim(),int.Parse (this.txtProxyPort.Text .Trim ()),"","");
                g = new cGatherWeb(Program.getPrjPath());
            }
            else
               g= new cGatherWeb(Program.getPrjPath ());

            string cookie=string.Empty , referUrl=string.Empty ;
            //g.IsUrlAutoRedirect = this.isA.Checked;

            //g.IsCustomHeader = true;

            List<eHeader> headers=new List<eHeader> ();

            string[] hs = this.txtHeader.Text.Split('\r');

            for (int i = 0; i < hs.Length; i++)
            {
                string s = hs[i];
                s = s.Replace("\r", "").Replace("\n", "").Trim ();
                if (s != "")
                {
                    eHeader h = new eHeader();
                    string label = s.Substring(0, s.IndexOf(":"));
                   string  LabelValue = s.Substring(s.IndexOf(":") + 1, s.Length - s.IndexOf(":") - 1);

                   if (label.ToLower() == "cookie")
                   {
                       cookie = LabelValue;
                   }
                   else if (label.ToLower() == "refer")
                   {
                       referUrl = LabelValue;
                   }
                   else
                   {
                       h.Label = label;
                       h.LabelValue = LabelValue;
                       headers.Add(h);
                   }
                }
            }

            HttpWebResponse wReq=null;
            try
            {
                //wReq = g.GetHtmlTest(url, wCode, out webSource, cookie, referUrl);
            }

            catch (System.Exception ex)
            {
                MessageBox.Show("请求发生错误，错误信息：" + ex.Message, "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.txtWebSource.Text = webSource;
            this.txtResponseHeader.Text = wReq.Headers.ToString();

            this.webBrowser1.AllowNavigation = this.isA.Checked;
            webSource = ToolUtil.ConvertToAbsoluteUrls(webSource, new Uri(url));
            this.webBrowser1.DocumentText = webSource;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.comwCode.Items.Add("Auto");
            this.comwCode.Items.Add("UTF-8");
            this.comwCode.Items.Add("gb2312");
            this.comwCode.Items.Add("big5");
            this.comwCode.Items.Add("gbk");
            this.comwCode.SelectedIndex = 0;

            this.comPCode.Items.Add("ASCII");
            this.comPCode.Items.Add("UTF-8");
            this.comPCode.Items.Add("gb2312");
            this.comPCode.Items.Add("big5");
            this.comPCode.Items.Add("gbk");
            this.comPCode.SelectedIndex = 0;
        }

        private void raGet_CheckedChanged(object sender, EventArgs e)
        {
            if (raGet.Checked == true)
                this.txtPostData.Enabled = false;
        }

        private void raPOST_CheckedChanged(object sender, EventArgs e)
        {
            if (raPOST.Checked == true)
                this.txtPostData.Enabled = true;
        }

        private void raSystemProxy_CheckedChanged(object sender, EventArgs e)
        {
            if (raSystemProxy.Checked == true)
            {
                this.txtProxyAddress.Enabled = false;
                this.txtProxyPort.Enabled = false;
            }

        }

        private void raCustomProxy_CheckedChanged(object sender, EventArgs e)
        {
            if (raCustomProxy.Checked == true)
            {
                this.txtProxyAddress.Enabled = true  ;
                this.txtProxyPort.Enabled = true ;
            }
        }

        private void txtUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Request();
            }
        }
    }
}

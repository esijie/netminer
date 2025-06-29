using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Web;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace MinerSpider
{
    public partial class frmAddProxy : Form
    {
        private ResourceManager rm;

        public delegate void ReturnProxy(ListViewItem cItem);
        public ReturnProxy rProxy;

        public frmAddProxy()
        {
            InitializeComponent();
        }

        private void butTestProxy_Click(object sender, EventArgs e)
        {
            bool IsSucceed = false;
            try
            {
                Uri m_uri = new Uri(this.txtProxyTestUrl.Text);
                IsSucceed = VerifyProxy(m_uri);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (IsSucceed == true)
            {
                MessageBox.Show(rm.GetString("Info229"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(rm.GetString("Info230"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //验证代理服务器是否可以正常通讯
        private bool VerifyProxy(Uri m_Uri)
        {
            //判断网页编码
            Encoding wCode = Encoding.Default;

            HttpWebRequest wReq;

            //WebProxy w = new WebProxy("http://" + this.txtProxyServer.Text.Trim() + ":" + this.txtProxyPort.Text.Trim(), true);
            WebProxy w = new WebProxy(this.txtProxyServer.Text.Trim(), int.Parse(this.txtProxyPort.Text.Trim()));
            w.BypassProxyOnLocal = true;
            if (this.txtProxyUser.Text.Trim() != "")
            {
                w.Credentials = new NetworkCredential(this.txtProxyUser.Text.Trim(), this.txtProxyPwd.Text);
            }
            string url = m_Uri.AbsolutePath;

            wReq = (HttpWebRequest)WebRequest.Create(m_Uri);

            wReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50215;)";
            wReq.Headers.Add("Accept-Language", "zh-cn,en-us;");
            wReq.KeepAlive = true;
            wReq.AllowAutoRedirect = true;

            wReq.Proxy = w;

            Match a = Regex.Match(url, @"(http://).[^/]*[?=/]", RegexOptions.IgnoreCase);
            string url1 = a.Groups[0].Value.ToString();
            wReq.Referer = url1;
            wReq.Method = "GET";


            //设置页面超时时间为12秒
            wReq.Timeout = 12000;

            HttpWebResponse wResp = (HttpWebResponse)wReq.GetResponse();

            System.IO.Stream respStream = wResp.GetResponseStream();
            string strWebData = "";

            System.IO.StreamReader reader;
            reader = new System.IO.StreamReader(respStream, wCode);
            strWebData = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();

            m_Uri = null;

            return true;

        }

        private void txtProxyPort_Leave(object sender, EventArgs e)
        {
            if (Regex.IsMatch(this.txtProxyPort.Text, "[^0-9]"))
            {
                MessageBox.Show(rm.GetString("Error31"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtProxyPort.Focus();
                this.txtProxyPort.SelectAll();
                return;
            }
        }

        private void frmAddProxy_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));
        }

        private void frmAddProxy_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (this.txtProxyServer.Text == "")
            {
                MessageBox.Show(rm.GetString("Error45"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtProxyPort.Focus();
                this.txtProxyPort.SelectAll();
                return;
            }

            if (this.txtProxyPort.Text == "")
            {
                //如果为空，则默认为80
                this.txtProxyPort.Text = "80";
            }

            ListViewItem cItem = new ListViewItem();
            cItem.Name = this.txtProxyServer.Text;
            cItem.Text = this.txtProxyServer.Text;
            cItem.SubItems.Add(this.txtProxyPort.Text);
            cItem.SubItems.Add(this.txtProxyUser.Text);
            cItem.SubItems.Add(this.txtProxyPwd.Text);
            cItem.SubItems.Add("");
            cItem.SubItems.Add("");
            rProxy(cItem);

            this.Close();

        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
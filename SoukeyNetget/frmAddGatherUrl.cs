using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Soukey;
using System.Text.RegularExpressions;
using SoukeyNetget.WebBrowser;
using System.Reflection;
using System.Resources;
using Soukey.Task;
using Soukey.Proxy;

namespace SoukeyNetget
{
    public partial class frmAddGatherUrl : Form
    {
        //返回配置的网址信息
        public delegate void RuturnUrl(string Url,bool IsNavi,DataGridView dNavi,bool IsNext,string NextPageRule);
        public RuturnUrl RUrl;

        //定义一个访问资源文件的变量
        private ResourceManager rm;

        //定义一个值为任务类型
        private cGlobalParas.TaskType m_TaskType;

        private bool m_IsUrlEncoding;
        public bool IsUrlEncoding
        {
            get { return m_IsUrlEncoding; }
            set { m_IsUrlEncoding = value; }
        }

        private cGlobalParas.WebCode m_UrlEncoding;
        public cGlobalParas.WebCode UrlEncoding
        {
            get { return m_UrlEncoding; }
            set { m_UrlEncoding = value; }
        }

        private bool m_IsProxy;
        public bool IsProxy
        {
            get { return m_IsProxy; }
            set { m_IsProxy = value; }
        }

        public cGlobalParas.TaskType TaskType
        {
            get { return m_TaskType; }
            set { m_TaskType = value; }
        }

        private cGlobalParas.FormState m_FormState;
        public cGlobalParas.FormState FormState
        {
            get { return m_FormState; }
            set { m_FormState = value; }
        }

        private cGlobalParas.WebCode m_WebCode;
        public cGlobalParas.WebCode WebCode
        {
            get { return m_WebCode; }
            set { m_WebCode = value; }
        }

        private string m_Cookie;
        public string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }

        public frmAddGatherUrl()
        {
            InitializeComponent();
        }

        //加载采集网址信息
        public void LoadInfo(string Url, bool IsNavi, cNavigRules dNavi, bool IsNext, string NextPageRule)
        {
            this.txtWebLink.Text = Url;
            this.IsNavigPage.Checked = IsNavi;
            this.IsAutoNextPage.Checked = IsNext;
            this.txtNextPage.Text = NextPageRule;

            if (dNavi != null)
            {
                for (int j = 0; j < dNavi.NavigRule.Count; j++)
                {
                    string strIsGather = "";
                    string strIsNavNext = "";
                    string strIsNext = "";

                    if (dNavi.NavigRule[j].IsGather == true)
                        strIsGather = "Y";
                    else
                        strIsGather = "N";

                    if (dNavi.NavigRule[j].IsNaviNextPage == true)
                        strIsNavNext = "Y";
                    else
                        strIsNavNext = "N";

                    if (dNavi.NavigRule[j].IsNext == true)
                        strIsNext = "Y";
                    else
                        strIsNext = "N";

                    this.dataNRule.Rows.Add(dNavi.NavigRule[j].Level, strIsNext, dNavi.NavigRule[j].NextRule,
                            dNavi.NavigRule[j].NavigRule, dNavi.NavigRule[j].NaviStartPos, dNavi.NavigRule[j].NaviEndPos,
                         strIsGather, dNavi.NavigRule[j].GatherStartPos, dNavi.NavigRule[j].GatherEndPos,
                         strIsNavNext, dNavi.NavigRule[j].NaviNextPage);

                }
            }

        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            if (this.m_FormState == cGlobalParas.FormState.New)
            {
                RUrl(this.txtWebLink.Text, this.IsNavigPage.Checked, this.dataNRule, this.IsAutoNextPage.Checked, this.txtNextPage.Text);
            }
            else if (this.m_FormState == cGlobalParas.FormState.Edit)
            {
                RUrl(this.txtWebLink.Text, this.IsNavigPage.Checked, this.dataNRule, this.IsAutoNextPage.Checked, this.txtNextPage.Text);
            }
            this.Close();
        }

        private void cmdNext_Click(object sender, EventArgs e)
        {
            if (this.panel1.Visible == true)
            {
                this.panel1.Visible = false;
                this.panel2.Visible = true;
                this.panel3.Visible = false;

                this.cmdPre.Enabled = true;

                if (this.IsNavigPage.Checked == true)
                    this.cmdNext.Enabled = true;
                else
                    this.cmdNext.Enabled = false;

            }
            else if (this.panel2.Visible == true)
            {
                this.panel1.Visible = false;
                this.panel2.Visible = false;
                this.panel3.Visible = true;

                this.cmdNext.Enabled = false;
            }
            else if (this.panel3.Visible == true)
            {
                
            }
        }

        private void cmdPre_Click(object sender, EventArgs e)
        {
            if (this.panel3.Visible == true)
            {
                this.panel1.Visible = false;
                this.panel2.Visible = true;
                this.panel3.Visible = false;

                this.cmdNext.Enabled = true;
            }
            else if (this.panel2.Visible == true)
            {
                this.panel1.Visible = true;
                this.panel2.Visible = false;
                this.panel3.Visible = false;

                this.cmdPre.Enabled = false;
                this.cmdNext.Enabled = true;
            }
            else if (this.panel3.Visible == true)
            {

            }
        }

        private void IsNavigPage_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsNavigPage.Checked == true)
            {
                this.IsAutoNextPage.Enabled = false;
                this.txtNextPage.Enabled = false;

                this.cmdNext.Enabled = true;
            }
            else
            {
                this.IsAutoNextPage.Enabled = true ;
                this.txtNextPage.Enabled = true ;

                this.cmdNext.Enabled = false;
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmAddGatherUrl_Load(object sender, EventArgs e)
        {
            //初始化资源文件的参数
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());
        }

        private void cmdAddNRule_Click(object sender, EventArgs e)
        {
            frmAddNavPageRule f = new frmAddNavPageRule();
            f.fState = cGlobalParas.FormState.New;
            f.rNavRule = new frmAddNavPageRule.ReturnNavRule(GetNavRule);
            f.ShowDialog();
            f.Dispose();
        }

        private void GetNavRule(cGlobalParas.FormState fState, bool IsNext, string NextRule, string NextMaxPage,
            string NavRule, string SPos, string EPos, bool IsGather, string GSPos, string GEPos, 
            bool IsNaviNext, string NaviNextRule,string NaviNextMaxPage)
        {

            for (int i = 0; i < this.dataNRule.Rows.Count; i++)
            {
                this.dataNRule.Rows[i].Cells[0].Value = i + 1;
            }

            int MaxLevel = 0;
            if (this.dataNRule.Rows.Count == 0)
                MaxLevel = 1;
            else
                MaxLevel = this.dataNRule.Rows.Count + 1;

            string strIsNaviNext = "";
            string strIsGather = "";

            //导航翻页参数
            string strIsNext = "";

            if (IsGather == true)
                strIsGather = "Y";
            else
                strIsGather = "N";

            if (IsNaviNext == true)
                strIsNaviNext = "Y";
            else
                strIsNaviNext = "N";

            if (IsNext == true)
                strIsNext = "Y";
            else
                strIsNext = "N";

            if (fState == cGlobalParas.FormState.New)
            {
                this.dataNRule.Rows.Add(MaxLevel, strIsNext, NextRule,NextMaxPage , NavRule, SPos, EPos, 
                    strIsGather, GSPos, GEPos, strIsNaviNext, NaviNextRule);
            }
            else if (fState == cGlobalParas.FormState.Edit)
            {
                int curr = this.dataNRule.CurrentCell.RowIndex;
                this.dataNRule.Rows[curr].Cells[1].Value = strIsNext;
                this.dataNRule.Rows[curr].Cells[2].Value = NextRule;
                this.dataNRule.Rows[curr].Cells[3].Value = NavRule;
                this.dataNRule.Rows[curr].Cells[4].Value = SPos;
                this.dataNRule.Rows[curr].Cells[5].Value = EPos;
                this.dataNRule.Rows[curr].Cells[6].Value = strIsGather;
                this.dataNRule.Rows[curr].Cells[7].Value = GSPos;
                this.dataNRule.Rows[curr].Cells[8].Value = GEPos;
                this.dataNRule.Rows[curr].Cells[9].Value = strIsNaviNext;
                this.dataNRule.Rows[curr].Cells[10].Value = NaviNextRule;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Show(this.button2, 0, 21);
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            
        }

        private void contextMenuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
           
        }

        private void GetPData(string strCookie, string pData)
        {
            this.txtWebLink.Text += "<POST>" + pData + "</POST>";
        }

        private void frmAddGatherUrl_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            this.contextMenuStrip1.Items.Clear();

            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu1") + "{Num:1,100,1}", null, null, "rmenuAddNum"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu2") + "{Num:100,1,-1}", null, null, "rmenuDegreNum"));
            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu3") + "{Letter:a,z}", null, null, "rmenuAddLetter"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu4") + "{Letter:z,a}", null, null, "rmenuDegreLetter"));
            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());

            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu17") + "{ShortDate:" + System.DateTime.Now.Year + "-1-1," + System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + "}", null, null, "rmenuDate"));

            string strDate = "";
            strDate = System.DateTime.Now.Year + "-";

            if (int.Parse(System.DateTime.Now.Month.ToString()) < 10)
                strDate += "0" + System.DateTime.Now.Month;
            else
                strDate += System.DateTime.Now.Month;

            strDate += "-";

            if (int.Parse(System.DateTime.Now.Day.ToString()) < 10)
                strDate += "0" + System.DateTime.Now.Day;
            else
                strDate += System.DateTime.Now.Day;

            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu18") + "{LongDate:" + System.DateTime.Now.Year + "-01-01," + strDate + "}", null, null, "rmenuDate"));


            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu5") + "<POST>", null, null, "rmenuPostPrefix"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu6") + "</POST>", null, null, "rmenuPostSuffix"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu7"), null, null, "rmenuGetPostData"));
            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());

            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu15") + "<BASE64>", null, null, "rmenuBase64Prefix"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu16") + "</BASE64>", null, null, "rmenuBase64Suffix"));
            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());

            if (TaskType == cGlobalParas.TaskType.ExternalPara)
            {
                this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu13") + "<EPara>", null, null, "rmenuGetEParaPrefix"));
                this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu14") + "</EPara>", null, null, "rmenuGetEParaSuffix"));
                this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
            }

            //初始化字典菜单的项目
            cDict d = new cDict();
            int count = d.GetDictClassCount();


            for (int i = 0; i < count; i++)
            {
                this.contextMenuStrip1.Items.Add(rm.GetString("rmenu8") + ":{Dict:" + d.GetDictClassName(i).ToString() + "}");
            }
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "rmenuGetPostData")
            {
                frmBrowser wftm = new frmBrowser();
                wftm.getFlag = 1;
                wftm.rPData = new frmBrowser.ReturnPOST(GetPData);
                wftm.ShowDialog();
                wftm.Dispose();

                return;
            }

            Match s;

            if (Regex.IsMatch(e.ClickedItem.ToString(), "[{].*[}]"))
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[{].*[}]");
            }
            else
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[<].*[>]");
            }

            int startPos = this.txtWebLink.SelectionStart;
            int l = this.txtWebLink.SelectionLength;

            this.txtWebLink.Text = this.txtWebLink.Text.Substring(0, startPos) + s.Groups[0].Value + this.txtWebLink.Text.Substring(startPos + l, this.txtWebLink.Text.Length - startPos - l);

            this.txtWebLink.SelectionStart = startPos + s.Groups[0].Value.Length;
            this.txtWebLink.ScrollToCaret();
        }

        private void IsAutoNextPage_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsAutoNextPage.Checked == true)
            {
                this.txtNextPage.Enabled = true;
                this.txtNextPage.Text = rm.GetString("NextPage");
            }
            else
            {
                if (this.txtNextPage.Text == rm.GetString("NextPage"))
                {
                    this.txtNextPage.Text = "";
                }
                this.txtNextPage.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this.dataNRule.Rows.Count == 0)
            {
                MessageBox.Show(rm.GetString("Error5"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<cNavigRule> cns;
            cNavigRule cn;

            string nUrl = this.txtWebLink.Text;

            //先行进行网址编码和Base64的问题
            //获取网址后，首先进行Url编码和Base64的处理
            if (this.m_IsUrlEncoding  == true)
                nUrl = cTool.UrlEncode(nUrl, this.m_UrlEncoding);

            //在此处理是否需要进行Base64编码的的问题
            if (Regex.IsMatch(nUrl, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
            {

                Match s = Regex.Match(nUrl, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
                string sBase64 = s.Groups[0].Value.ToString();
                sBase64 = cTool.Base64Encoding(sBase64);

                //将base64编码部分进行url替换
                Regex.Replace(nUrl, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            }

            //需判断当前的网址是否含有参数，如果有参数则需要分解参数
            //当前的做法是是否含有参数都去分解一次，如果没有参数，则返回原址

            cProxyControl pControl = new cProxyControl(0);

            cUrlAnalyze cu = new cUrlAnalyze(ref pControl, this.m_IsProxy, true );

            List<string> nUrls = cu.SplitWebUrl(nUrl);
            cu = null;

            nUrl = nUrls[0].ToString();
            nUrls = null;

            if (this.m_IsUrlEncoding == true)
                nUrl = cTool.UrlEncode(nUrl,this.m_UrlEncoding);


            for (int m = 0; m < this.dataNRule.Rows.Count; m++)
            {
                cns = new List<cNavigRule>();

                cn = new cNavigRule();
                cn.Url = nUrl;
                cn.Level = 1;
                cn.NavigRule = this.dataNRule.Rows[m].Cells[3].Value.ToString();
                cn.NaviStartPos = this.dataNRule.Rows[m].Cells[4].Value.ToString();
                cn.NaviEndPos = this.dataNRule.Rows[m].Cells[5].Value.ToString();
                cns.Add(cn);

                nUrl = GetTestUrl(nUrl, cns);
            }

            string Url = nUrl;

            if (!Regex.IsMatch(Url, @"(http|https|ftp)+://[^\s]*"))
            {
                MessageBox.Show(rm.GetString("Error6"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(Url);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            pControl = null;
        }

        private delegate string delegateGNavUrl(string webLink, List<cNavigRule> NavRule, Soukey.cGlobalParas.WebCode webCode, string cookie);
        private string GetTestUrl(string webLink, List<cNavigRule> NavRule)
        {
            //定义一个获取导航网址的委托
            delegateGNavUrl sd = new delegateGNavUrl(this.GetNavUrl);

            //开始调用函数,可以带参数 
            IAsyncResult ir = sd.BeginInvoke(webLink, NavRule, this.m_WebCode, this.m_Cookie, null, null);

            //显示等待的窗口 
            frmWaiting fWait = new frmWaiting(rm.GetString("Info117"));
            fWait.Text = rm.GetString("Info117");
            fWait.Show(this);

            //刷新这个等待的窗口 
            Application.DoEvents();

            //循环检测是否完成了异步的操作 
            while (true)
            {
                if (ir.IsCompleted)
                {
                    //完成了操作则关闭窗口
                    fWait.Close();
                    break;
                }

            }

            //取函数的返回值 
            string rUrl = sd.EndInvoke(ir);

            return rUrl;
        }

        //每次仅能获取一次导航，不能获取多次导航的网址，即当前只支持一层导航，多层导航的探测
        //需要重复调用此地址
        private string GetNavUrl(string webLink, List<cNavigRule> NavRule,Soukey.cGlobalParas.WebCode webCode,string cookie)
        {
            List<string> Urls;

            cUrlAnalyze gUrl = new cUrlAnalyze();

            Urls = gUrl.ParseUrlRule(webLink, NavRule, webCode, cookie);

            gUrl = null;

            if (Urls == null || Urls.Count ==0)
                return "";

            //string isReg="[\"\\s]";
            string isReg = "[\\s]";
            string Url="";

            for (int m=0 ;m<Urls.Count ;m++)
            {
                if (!Regex.IsMatch (Urls[m].ToString (),isReg ))
                {
                    Url = Urls[m].ToString();
                    break ;
                }
            }

            if (Url == "")
                return "";

            string PreUrl = "";

            //需要判断网页地址前后存在单引号或双引号
            if (Url.Substring(0, 1) == "'" || Url.Substring(0, 1) == "\"")
            {
                Url = Url.Substring(1, Url.Length - 1);
            }

            if (Url.Substring(Url.Length - 1, 1) == "'" || Url.Substring(Url.Length - 1, 1) == "\"")
            {
                Url = Url.Substring(0, Url.Length - 1);
            }

            //去除了相对网址表示，通过程序进行判断
            ///************以下代码被注释，应没有价值，只要是导航，返回的地址都是绝对地址，无需
            ///再判断是否为相对地址了**********************************************************
            ///修改时间：2010-7-8
            //if (string.Compare (Url.Substring (0,4),"http",true)!=0)
            //{
            //    if (Url.Substring(0, 1) == "/")
            //    {
            //        PreUrl = webLink.Substring(7, webLink.Length  - 7);
            //        PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));
            //        PreUrl = "http://" + PreUrl;
            //    }
            //    else
            //    {
            //        Match aa = Regex.Match(webLink, ".*/");
            //        PreUrl = aa.Groups[0].Value.ToString();
            //    }
            //}
            ///************以下代码被注释，应没有价值，只要是导航，返回的地址都是绝对地址，无需
            ///再判断是否为相对地址了**********************************************************

            return PreUrl + Url;

        }

        private void dataNRule_DoubleClick(object sender, EventArgs e)
        {
            if (this.dataNRule.Rows.Count == 0)
            {
                return;
            }

            frmAddNavPageRule f = new frmAddNavPageRule();
            f.fState = cGlobalParas.FormState.Edit;
            int curr = this.dataNRule.CurrentCell.RowIndex;
            bool isG;
            bool isNaviNext;
            bool isN;

            if (this.dataNRule.Rows[curr].Cells[6].Value.ToString() == "Y")
                isG = true;
            else
                isG = false;

            if (this.dataNRule.Rows[curr].Cells[9].Value.ToString() == "Y")
                isNaviNext = true;
            else
                isNaviNext = false;

            if (this.dataNRule.Rows[curr].Cells[1].Value.ToString() == "Y")
                isN = true;
            else
                isN = false;

            f.FillData(isN, this.dataNRule.Rows[curr].Cells[2].Value.ToString(),this.dataNRule.Rows[curr].Cells[3].Value.ToString(),
                this.dataNRule.Rows[curr].Cells[3].Value.ToString(), this.dataNRule.Rows[curr].Cells[4].Value.ToString(),
                this.dataNRule.Rows[curr].Cells[5].Value.ToString(),
                isG, this.dataNRule.Rows[curr].Cells[7].Value.ToString(), this.dataNRule.Rows[curr].Cells[8].Value.ToString(),
                isNaviNext, this.dataNRule.Rows[curr].Cells[10].Value.ToString(), this.dataNRule.Rows[curr].Cells[12].Value.ToString());

            f.rNavRule = new frmAddNavPageRule.ReturnNavRule(GetNavRule);
            f.ShowDialog();
            f.Dispose();
        }


    }
}
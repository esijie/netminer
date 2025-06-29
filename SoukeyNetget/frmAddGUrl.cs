using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetMiner.Gather.Task;
using NetMiner.Core.Proxy;
using System.Text.RegularExpressions;
using System.Resources;
using System.Reflection;
using SoukeyControl.WebBrowser;
using NetMiner.Resource;
using NetMiner.Common;
using System.Threading;
using System.IO;
using NetMiner.Core.Dict;
using NetMiner.Core.gTask.Entity;
using NetMiner.Gather.Url;

namespace MinerSpider
{
    public partial class frmAddGUrl : Form
    {
        //定义一个访问资源文件的变量
        private ResourceManager rm;

        public delegate void ReturnGatherUrl(cGlobalParas.FormState fState, cGatherUrlFormConfig fgUrlRule);
        public ReturnGatherUrl rGUrl;

        private string m_getUrlPara = string.Empty;

        //定义一个ToolTip
        ToolTip HelpTip = new ToolTip();

        private int startIndex = 0;
        private int selectLength = 0;

        private bool m_isIniDictMenu = false;
        private Dictionary<string,string> m_dictMenu;

        public frmAddGUrl()
        {
            InitializeComponent();

            this.txtWebLink.MaxLength = 80000;

            //初始化资源文件的参数
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));
        }

        private cGlobalParas.FormState m_fState;
        public cGlobalParas.FormState fState
        {
            get { return m_fState; }
            set { m_fState = value; }
        }

        private string m_wCode;
        public string wCode
        {
            get { return m_wCode; }
            set { m_wCode = value; }
        }

        private string m_UrlCode;
        public string UrlCode
        {
            get { return m_UrlCode; }
            set { m_UrlCode = value; }
        }

        private bool m_IsUrlEncoding;
        public bool IsUrlEncoding
        {
            get { return m_IsUrlEncoding; }
            set { m_IsUrlEncoding = value; }
        }

        private string m_Cookie;
        public string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }

        private bool m_IsProxy;
        public bool IsProxy
        {
            get { return m_IsProxy; }
            set { m_IsProxy = value; }
        }

        private bool m_IsFirstProxy;
        public bool IsFirstProxy
        {
            get { return m_IsFirstProxy; }
            set { m_IsFirstProxy = value; }
        }

        private cGlobalParas.TaskType m_TaskType;
        public cGlobalParas.TaskType TaskType
        {
            get { return m_TaskType; }
            set { m_TaskType = value; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Show(this.button2, 0, 21);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmDict dfrm = new frmDict();
            dfrm.ShowDialog();
            dfrm.Dispose();
        }

        private void IsNavigPage_CheckedChanged(object sender, EventArgs e)
        {
            SetNaviTabPos();
        }

        private void IsTabGather_CheckedChanged(object sender, EventArgs e)
        {
            SetNaviTabPos();
        }

        private void SetNaviTabPos()
        {
            if (this.IsTabGather.Checked == true )
            {
                this.groupBox14.Width = 441;
                this.groupBox14.Enabled = true;
                this.groupBox14.Visible = true ;
                this.groupBox13.Visible = true;
                this.groupBox13.Location = new Point(457, 147);
                this.groupBox13.Width = 253;
            }
            else if (this.IsTabGather.Checked == true )
            {
                this.groupBox14.Enabled = false ;
                this.groupBox14.Visible = false;
                this.groupBox13.Visible = true;
                this.groupBox13.Width = 699;
                this.groupBox13.Location = new Point(11, 147);
            }
            else if (this.IsTabGather.Checked == false  )
            {
                this.groupBox14.Width = 699;
                this.groupBox14.Enabled = true  ;
                this.groupBox14.Visible = true;
                this.groupBox13.Visible = false;
            }
            else if (this.IsTabGather.Checked == false)
            {
                this.groupBox14.Width = 699;
                this.groupBox14.Enabled = false;
                this.groupBox14.Visible = true;
                this.groupBox13.Visible = false;
            }
        }

        private void cmdAddNRule_Click(object sender, EventArgs e)
        {
            AddNavRule();
        }

        private void AddNavRule()
        {
            frmAddNavPageRule f = new frmAddNavPageRule();
            f.fState = cGlobalParas.FormState.New;
            f.TestUrl = this.txtWebLink.Text;
            f.Cookie = this.m_Cookie;
            f.rNavRule = new frmAddNavPageRule.ReturnNavRule(GetNavRule);
            f.ShowDialog();
            f.Dispose();
        }

        public void IniData(cGatherUrlFormConfig uConfig)
        {
            this.txtWebLink.Text = uConfig.Url;

            //下一页的填充
            if (uConfig.IsNext == false)
            {
                this.IsAutoNextPage.Checked = false;
                this.txtNextPage.Text = "";
                this.numMaxNextPage.Value = 0;
            }
            else
            {
                this.IsAutoNextPage.Checked = true;
                this.txtNextPage.Text = uConfig.NextRule;
                this.numMaxNextPage.Value = uConfig.MaxPageCount;
            }


            if (uConfig.IsNav == false )
            {
                this.dataNRule.Rows.Clear();
            }
            else
            {
                //this.txtNag.Text = "";

                //添加导航规则

                this.dataNRule.Rows.Clear();

               
                for (int j = 0; j < uConfig.nRules.NavigRule.Count; j++)
                {
                    string strIsGather = "";
                    string strIsNavNext = "";
                    string strIsNext = "";
                    string strIsNextDoPostBack = "";
                    string strIsNaviNextDoPostBack = "";

                    if (uConfig.nRules.NavigRule[j].IsGather == true)
                        strIsGather = "Y";
                    else
                        strIsGather = "N";

                    if (uConfig.nRules.NavigRule[j].IsNaviNextPage == true)
                        strIsNavNext = "Y";
                    else
                        strIsNavNext = "N";

                    if (uConfig.nRules.NavigRule[j].IsNext == true)
                        strIsNext = "Y";
                    else
                        strIsNext = "N";

                    if (j == 0 && strIsNext=="Y")
                    {
                        this.IsAutoNextPage.Checked = true;
                        this.txtNextPage.Text = uConfig.nRules.NavigRule[j].NextRule;
                        this.numMaxNextPage.Value =int.Parse ( uConfig.nRules.NavigRule[j].NextMaxPage);

                        this.dataNRule.Rows.Add(uConfig.nRules.NavigRule[j].Level, "N", "", "N", "0",
                                uConfig.nRules.NavigRule[j].NavigRule, uConfig.nRules.NavigRule[j].NaviStartPos, uConfig.nRules.NavigRule[j].NaviEndPos,
                                strIsGather, uConfig.nRules.NavigRule[j].GatherStartPos, uConfig.nRules.NavigRule[j].GatherEndPos,
                                strIsNavNext, uConfig.nRules.NavigRule[j].NaviNextPage, strIsNaviNextDoPostBack, uConfig.nRules.NavigRule[j].NaviNextMaxPage, (int)uConfig.nRules.NavigRule[j].RunRule, uConfig.nRules.NavigRule[j].OtherNaviRule);
                    }
                    else
                    {
                        this.dataNRule.Rows.Add(uConfig.nRules.NavigRule[j].Level, strIsNext, uConfig.nRules.NavigRule[j].NextRule, strIsNextDoPostBack, uConfig.nRules.NavigRule[j].NextMaxPage,
                                uConfig.nRules.NavigRule[j].NavigRule, uConfig.nRules.NavigRule[j].NaviStartPos, uConfig.nRules.NavigRule[j].NaviEndPos,
                                strIsGather, uConfig.nRules.NavigRule[j].GatherStartPos, uConfig.nRules.NavigRule[j].GatherEndPos,
                                strIsNavNext, uConfig.nRules.NavigRule[j].NaviNextPage, strIsNaviNextDoPostBack, uConfig.nRules.NavigRule[j].NaviNextMaxPage, (int)uConfig.nRules.NavigRule[j].RunRule, uConfig.nRules.NavigRule[j].OtherNaviRule);
                    }
                }
                  

            }

            //填充多页采集的数据
            if (uConfig.IsMulti  == true)
            {
                this.IsTabGather.Checked = true;
                this.is121.Checked = uConfig.isMulti121;

                this.dataMultiPage.Rows.Clear();

               
                for (int j = 0; j < uConfig.MRules.MultiPageRule.Count ; j++)
                {
                    this.dataMultiPage.Rows.Add(uConfig.MRules.MultiPageRule[j].RuleName,uConfig.MRules.MultiPageRule[j].mLevel,
                        uConfig.MRules.MultiPageRule[j].Rule);
                }
                   
            }
            else
            {
                this.IsTabGather.Checked = false;
            }
        }

        private void GetNavRule(string strNavRule)
        {
            this.txtNextPage.Text = strNavRule;
        }

        private void GetNavRule(cGlobalParas.FormState fState, cNaviRuleFormConfig fNaviRule)
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

            string strIsNextDoPostBack = "";
            string strIsNaviNextDoPostBack = "";

            if (fNaviRule.IsGather == true)
                strIsGather = "Y";
            else
                strIsGather = "N";

            if (fNaviRule.IsNaviNext == true)
                strIsNaviNext = "Y";
            else
                strIsNaviNext = "N";

            if (fNaviRule.IsNext == true)
                strIsNext = "Y";
            else
                strIsNext = "N";

            if (fNaviRule.IsNextDoPostBack == true)
                strIsNextDoPostBack = "Y";
            else
                strIsNextDoPostBack = "N";

            if (fNaviRule.IsNaviNextDoPostBack == true)
                strIsNaviNextDoPostBack = "Y";
            else
                strIsNaviNextDoPostBack = "N";

            if (fState == cGlobalParas.FormState.New)
            {
                this.dataNRule.Rows.Add(MaxLevel, strIsNext, fNaviRule.NextRule, strIsNextDoPostBack, fNaviRule.NextMaxPage,
                    fNaviRule.NavRule, fNaviRule.SPos, fNaviRule.EPos, strIsGather, fNaviRule.GSPos, fNaviRule.GEPos,
                    strIsNaviNext, fNaviRule.NaviNextRule, strIsNaviNextDoPostBack, fNaviRule.NaviNextMaxPage,(int)fNaviRule.RunRule,fNaviRule.OtherNaviRule);
            }
            else if (fState == cGlobalParas.FormState.Edit)
            {
                int curr = this.dataNRule.CurrentCell.RowIndex;
                this.dataNRule.Rows[curr].Cells[1].Value = strIsNext;
                this.dataNRule.Rows[curr].Cells[2].Value = fNaviRule.NextRule;
                this.dataNRule.Rows[curr].Cells[3].Value = strIsNextDoPostBack;
                this.dataNRule.Rows[curr].Cells[4].Value = fNaviRule.NextMaxPage;
                this.dataNRule.Rows[curr].Cells[5].Value = fNaviRule.NavRule;
                this.dataNRule.Rows[curr].Cells[6].Value = fNaviRule.SPos;
                this.dataNRule.Rows[curr].Cells[7].Value = fNaviRule.EPos;
                this.dataNRule.Rows[curr].Cells[8].Value = strIsGather;
                this.dataNRule.Rows[curr].Cells[9].Value = fNaviRule.GSPos;
                this.dataNRule.Rows[curr].Cells[10].Value = fNaviRule.GEPos;
                this.dataNRule.Rows[curr].Cells[11].Value = strIsNaviNext;
                this.dataNRule.Rows[curr].Cells[12].Value = fNaviRule.NaviNextRule;
                this.dataNRule.Rows[curr].Cells[13].Value = strIsNaviNextDoPostBack;
                this.dataNRule.Rows[curr].Cells[14].Value = fNaviRule.NaviNextMaxPage;
                this.dataNRule.Rows[curr].Cells[15].Value = (int)fNaviRule.RunRule;
                this.dataNRule.Rows[curr].Cells[16].Value = fNaviRule.OtherNaviRule;
            }

        }

        private void GetNavRule(cGlobalParas.FormState fState, string[] strs)
        {
            if (fState == cGlobalParas.FormState.New)
                this.dataMultiPage.Rows.Add(strs[0].ToString(), strs[1].ToString(),strs[2].ToString());
            else if (fState == cGlobalParas.FormState.Edit)
            {
                int curr = this.dataMultiPage.CurrentCell.RowIndex;
                this.dataMultiPage.Rows[curr].Cells[0].Value = strs[0].ToString();
                this.dataMultiPage.Rows[curr].Cells[1].Value = strs[1].ToString();
                this.dataMultiPage.Rows[curr].Cells[2].Value = strs[2].ToString();
            }

        }

        private void cmdDelNRule_Click(object sender, EventArgs e)
        {
            this.dataNRule.Focus();
            SendKeys.Send("{Del}");
        }

        private void dataNRule_DoubleClick(object sender, EventArgs e)
        {
            EditNavRule();
        }

        private void EditNavRule()
        {
            if (this.dataNRule.Rows.Count == 0)
            {
                return;
            }

            frmAddNavPageRule f = new frmAddNavPageRule();
            cNaviRuleFormConfig fNaviRule = new cNaviRuleFormConfig();

            f.fState = cGlobalParas.FormState.Edit;
            f.TestUrl = this.txtWebLink.Text;
            f.Cookie = this.m_Cookie;

            int curr = this.dataNRule.CurrentCell.RowIndex;
            bool isG;
            bool isNaviNext;
            bool isN;
            bool IsNextDoPostBack;
            bool IsNaviNextDoPostBack;


            if (this.dataNRule.Rows[curr].Cells[8].Value.ToString() == "Y")
                fNaviRule.IsGather = true;
            else
                fNaviRule.IsGather = false;

            if (this.dataNRule.Rows[curr].Cells[11].Value.ToString() == "Y")
                fNaviRule.IsNaviNext = true;
            else
                fNaviRule.IsNaviNext = false;

            if (this.dataNRule.Rows[curr].Cells[1].Value.ToString() == "Y")
                fNaviRule.IsNext = true;
            else
                fNaviRule.IsNext = false;

            if (this.dataNRule.Rows[curr].Cells[3].Value.ToString() == "Y")
                fNaviRule.IsNextDoPostBack = true;
            else
                fNaviRule.IsNextDoPostBack = false;

            if (this.dataNRule.Rows[curr].Cells[13].Value.ToString() == "Y")
                fNaviRule.IsNaviNextDoPostBack = true;
            else
                fNaviRule.IsNaviNextDoPostBack = false;

            fNaviRule.NextRule = this.dataNRule.Rows[curr].Cells[2].Value.ToString();
            fNaviRule.NextMaxPage = this.dataNRule.Rows[curr].Cells[4].Value.ToString();
            fNaviRule.NavRule = this.dataNRule.Rows[curr].Cells[5].Value.ToString();
            fNaviRule.SPos = this.dataNRule.Rows[curr].Cells[6].Value.ToString();
            fNaviRule.EPos = this.dataNRule.Rows[curr].Cells[7].Value.ToString();
            fNaviRule.GSPos = this.dataNRule.Rows[curr].Cells[9].Value.ToString();
            fNaviRule.GEPos = this.dataNRule.Rows[curr].Cells[10].Value.ToString();
            fNaviRule.NaviNextRule = this.dataNRule.Rows[curr].Cells[12].Value.ToString();
            fNaviRule.NaviNextMaxPage = this.dataNRule.Rows[curr].Cells[14].Value.ToString();
            fNaviRule.RunRule =(cGlobalParas.NaviRunRule) int.Parse (this.dataNRule.Rows[curr].Cells[15].Value.ToString());
            fNaviRule.OtherNaviRule= this.dataNRule.Rows[curr].Cells[16].Value.ToString();

            f.FillData(fNaviRule);

            f.rNavRule = new frmAddNavPageRule.ReturnNavRule(GetNavRule);
            f.ShowDialog();
            f.Dispose();
        }

        private void IsAutoNextPage_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsAutoNextPage.Checked == true)
            {
                this.txtNextPage.Enabled = false;
                this.txtNextPage.BackColor = Color.White;
                this.numMaxNextPage.Enabled = true;
                this.button15.Enabled = true;
            }
            else
            {
                this.txtNextPage.Enabled = false;
                this.txtNextPage.BackColor = Color.FromArgb(240, 240, 240);
                this.numMaxNextPage.Enabled = false;
                this.button15.Enabled = false;
            }
        }

        private void cmdAddPage_Click(object sender, EventArgs e)
        {
            AddMultiRule();
        }

        private void AddMultiRule()
        {
            //取最大导航级别的网址
            int maxLevel = 0;
            maxLevel = this.dataNRule.Rows.Count;

            frmAddMultiPage fn = new frmAddMultiPage("", "", "", maxLevel);
            fn.fState = cGlobalParas.FormState.New;
            fn.rNavRule = new frmAddMultiPage.ReturnNavRule(GetNavRule);
            fn.ShowDialog();
            fn.Dispose();
        }

        private void cmdDelPage_Click(object sender, EventArgs e)
        {
            this.dataMultiPage.Focus();
            SendKeys.Send("{Del}");
        }

        //多页规则测试
        private void button16_Click(object sender, EventArgs e)
        {
            //先获取正常的网址
            //List<string> urls = GetDemoUrl();

            //if (urls == null || urls.Count == 0)
            //    return;

            //string url = urls[0].ToString();

            ////增加导航规则，尽管是多页，但还是按照导航的逻辑进行处理

            //string DemoUrl = "";

            //List<cNavigRule> tempcns;
            //cNavigRule tempcn;
            //for (int i = 0; i < this.dataMultiPage.Rows.Count; i++)
            //{

            //    tempcns = new List<cNavigRule>();
            //    tempcn = new cNavigRule();

            //    tempcn.Url = url;
            //    tempcn.Level = 1;


            //    tempcn.NaviStartPos = "";
            //    tempcn.NaviEndPos = "";
            //    tempcn.NavigRule = this.dataMultiPage.Rows[i].Cells[1].Value.ToString();


            //    tempcns.Add(tempcn);

            //    DemoUrl = AddDemoUrl(url, true, tempcns);


            //}

            //if (!Regex.IsMatch(DemoUrl, @"(http|https|ftp)+://[^\s]*"))
            //{
            //    MessageBox.Show(rm.GetString("Error6") + " 多页规则匹配结果：" + DemoUrl, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            //try
            //{
            //    System.Diagnostics.Process.Start(DemoUrl);
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

        }

        private void button15_Click(object sender, EventArgs e)
        {
            frmAddNextRule f = new frmAddNextRule(this.txtNextPage.Text);
            f.TestUrl = this.txtWebLink.Text;
            f.Cookie = this.m_Cookie;
            f.rNavRule = new frmAddNextRule.ReturnNavRule(GetNavRule);
            f.ShowDialog();
            f.Dispose();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this.dataNRule.Rows.Count == 0)
            {
                MessageBox.Show(rm.GetString("Error5"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<eNavigRule> cns;
            eNavigRule cn;

            string nUrl = this.txtWebLink.Text;

            //先把字典表中的数据提取重来，这样做的目的是为了在编码和base64的时候
            //出现将字典分类也错误编码造成错误
            string strMatch = "(?<={Dict:)[^}]*(?=})";
            Match ss = Regex.Match(nUrl, strMatch, RegexOptions.IgnoreCase);
            string UrlPara = ss.Groups[0].Value;

            if (UrlPara != "")
            {
                NetMiner.Core.Url.cUrlParse cua = new NetMiner.Core.Url.cUrlParse(Program.getPrjPath());
                List<string> uDicts = cua.getListUrl("Dict:" + UrlPara);
                cua = null;

                nUrl = Regex.Replace(nUrl, @"(?={Dict:)[^}]*(})", uDicts[0].ToString(), RegexOptions.IgnoreCase | RegexOptions.Multiline);

            }

            //先行进行网址编码和Base64的问题
            //获取网址后，首先进行Url编码和Base64的处理
            if (this.IsUrlEncoding == true)
                nUrl = ToolUtil.UrlEncode(nUrl, EnumUtil.GetEnumName<cGlobalParas.WebCode> (this.UrlCode));

            //在此处理是否需要进行Base64编码的的问题
            if (Regex.IsMatch(nUrl, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
            {

                Match s = Regex.Match(nUrl, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
                string sBase64 = s.Groups[0].Value.ToString();
                sBase64 = ToolUtil.Base64Encoding(sBase64);

                //将base64编码部分进行url替换
                nUrl = Regex.Replace(nUrl, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            }

            //需判断当前的网址是否含有参数，如果有参数则需要分解参数
            //当前的做法是是否含有参数都去分解一次，如果没有参数，则返回原址

            cProxyControl pControl = new cProxyControl(Program.getPrjPath (), 0);

            NetMiner.Core.Url.cUrlParse cu = new NetMiner.Core.Url.cUrlParse(Program.getPrjPath ());

            List<string> nUrls = cu.SplitWebUrl(nUrl);
            cu = null;

            if (nUrls == null || nUrls.Count == 0)
            {
                MessageBox.Show(rm.GetString("Error6"), "有可能导航规则出错，未能导航出网址！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            nUrl = nUrls[0].ToString();
            nUrls = null;

            if (this.IsUrlEncoding== true)
                nUrl = ToolUtil.UrlEncode(nUrl,EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.UrlCode));

            //测试导航的时候，无需处理分页的一些问题
            for (int m = 0; m < this.dataNRule.Rows.Count; m++)
            {
                cns = new List<eNavigRule>();

                cn = new eNavigRule();
                cn.Url = nUrl;
                cn.Level = 1;
                //cn.NavigRule = this.dataNRule.Rows[m].Cells[5].Value.ToString();
                //cn.NaviStartPos = this.dataNRule.Rows[m].Cells[6].Value.ToString();
                //cn.NaviEndPos = this.dataNRule.Rows[m].Cells[7].Value.ToString();

                cn.NaviStartPos = this.dataNRule.Rows[m].Cells[6].Value.ToString();
                cn.NaviEndPos = this.dataNRule.Rows[m].Cells[7].Value.ToString();
                cn.NavigRule = this.dataNRule.Rows[m].Cells[5].Value.ToString();

                //cn.IsNaviNextPage = cns[j].IsNaviNextPage;
                //cn.NaviNextPage = cns[j].NaviNextPage;
                //cn.IsGather = cns[j].IsGather;
                //cn.GatherStartPos = cns[j].GatherStartPos;
                //cn.GatherEndPos = cns[j].GatherEndPos;

                //cn.IsNext = cns[j].IsNext;
                //cn.NextRule = cns[j].NextRule;

                //cn.IsNextDoPostBack = cns[j].IsNextDoPostBack;
                //cn.IsNaviNextDoPostBack = cns[j].IsNaviNextDoPostBack;

                cns.Add(cn);

                try
                {
                    //nUrl = GetTestUrl(nUrl, cns);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string Url = nUrl;

            if (!Regex.IsMatch(Url, @"(http|https|ftp)+://[^\s]*"))
            {
                MessageBox.Show(rm.GetString("Error6") + " 导航匹配结果为：" + Url, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {

                System.Diagnostics.Process.Start(Url);
                //this.txtWeblinkDemo.Text = Url;

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            pControl = null;
        }

        private void frmAddGUrl_Load(object sender, EventArgs e)
        {
            //对Tooltip进行初始化设置
            HelpTip.ToolTipIcon = ToolTipIcon.Info;
            HelpTip.ForeColor = Color.YellowGreen;
            HelpTip.BackColor = Color.LightGray;
            HelpTip.AutoPopDelay = 20000;
            HelpTip.ShowAlways = true;
            HelpTip.ToolTipTitle = "小矿提示";

            SetHelpTip();

            if (Program.SominerVersion == cGlobalParas.VersionType.Free)
            {
                this.IsTabGather.Enabled = false;
            }
        }

        private void SetHelpTip()
        {
            HelpTip.SetToolTip(this.txtWebLink, @"在此输入需要采集的网页地址，必须包含：http://， " + "\r\n" + "如果预计要输入多个网址，请参考右侧按钮参数，判断是否可以通过参数来替换以简化网址输入，" + "\r\n" + "注意：如果无法通过参数替换，再次仅能输入一条网址，多条网址需要多次添加");
            HelpTip.SetToolTip(this.button2, @"点击此按钮可以插入网址的参数，通过网址参数可以简化成批网址的输入操作，同时可以通过字典来替换网址参数，" + "\r\n" + "注意：如果是POST方式，也许在此选择输入POST标签，同时可以手工修改POST数据的编码");
            HelpTip.SetToolTip(this.IsAutoNextPage, @"选中此选项，则可以配置当前输入网址的翻页操作");
            HelpTip.SetToolTip(this.button15, @"点击此按钮开始配置翻页规则");
            HelpTip.SetToolTip(this.numMaxNextPage, @"默认情况下系统会根据翻页规则一直翻页至最后一页，" + "\r\n" + "您也可在此输入一个最大限制的翻页页码，来控制翻页的操作");
            HelpTip.SetToolTip(this.IsTabGather, @"选中此选项，则支持多页采集操作，多页");
            HelpTip.SetToolTip(this.button15, @"点击此按钮开始配置翻页规则");
            HelpTip.SetToolTip(this.cmdAddNRule, @"点击此按钮开始添加导航规则");
            HelpTip.SetToolTip(this.cmdDelNRule, @"点击此按钮删除已经选中的导航规则");
            HelpTip.SetToolTip(this.cmdAddPage, @"点击此按钮开始添加多页规则");
            HelpTip.SetToolTip(this.cmdDelPage, @"点击此按钮删除已经选中的多页规则");
            HelpTip.SetToolTip(this.is121, @"选中此选项，可以强制采集页采集的数据与多页采集的数据合并时采用一对一的合并方式，" + "\r\n" + "否则，如果多页采集的数据是多条的情况下，会进行一对多的合并操作");
        }

        private void frmAddGUrl_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.txtWebLink.Text = this.txtWebLink.Text.Trim();

            if (this.txtWebLink.Text.ToString() == null || this.txtWebLink.Text.Trim().ToString() == "" || this.txtWebLink.Text.Trim().ToString() == "http://")
            {
                MessageBox.Show(rm.GetString("Error1"), "网络矿工 错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtWebLink.Focus();
                return;
            }
            else
            {
                if (!Regex.IsMatch(this.txtWebLink.Text.Trim().ToString(), "http://|https://", RegexOptions.IgnoreCase)
                    && !Regex.IsMatch(this.txtWebLink.Text.Trim().ToString(), "{DbUrl:", RegexOptions.IgnoreCase))
                {
                    MessageBox.Show(rm.GetString("Error2"), "网络矿工 错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.txtWebLink.Focus();
                    return;
                }
            }

            //if (this.IsNavigPage.Checked == true)
            //{
            //    if (this.dataNRule.Rows.Count == 0)
            //    {
            //        MessageBox.Show("选择了导航采集，但未添加导航规则，请添加导航规则，或取消导航采集！", "网络矿工 错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        this.cmdAddPage.Focus();
            //        return;
            //    }
            //}

            if (this.IsTabGather.Checked == true)
            {
                if (this.dataMultiPage.Rows.Count == 0)
                {
                    MessageBox.Show("选择了多页采集，但未添加多页规则，请添加多页规则，或取消多页采集！", "网络矿工 错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.cmdAddPage.Focus();
                    return;
                }
            }

            cGatherUrlFormConfig cURule = new cGatherUrlFormConfig();

            cURule.Url = this.txtWebLink.Text.ToString();

            if (this.dataNRule.Rows.Count>0)
            {
                //添加带有导航规则的网址

                cURule.IsNav =true ;
                
                eNavigRule cn;

                for (int m = 0; m < this.dataNRule.Rows.Count; m++)
                {
                    cn = new eNavigRule();
                    cn.Url = this.txtWebLink.Text;
                    cn.Level = m + 1;

                    if (m == 0 && this.IsAutoNextPage.Checked == true)
                    {
                        cn.IsNext = true;
                        cn.NextRule = this.txtNextPage.Text.ToString();

                        cn.NextMaxPage =this.numMaxNextPage.Value.ToString();
                    }
                    else
                    {
                        if (this.dataNRule.Rows[m].Cells[1].Value.ToString() == "Y")
                            cn.IsNext = true;
                        else
                            cn.IsNext = false;

                        cn.NextRule = this.dataNRule.Rows[m].Cells[2].Value.ToString();
                      

                        cn.NextMaxPage = this.dataNRule.Rows[m].Cells[4].Value.ToString();
                    }

                    cn.NavigRule = this.dataNRule.Rows[m].Cells[5].Value.ToString();
                    cn.NaviStartPos = this.dataNRule.Rows[m].Cells[6].Value.ToString();
                    cn.NaviEndPos = this.dataNRule.Rows[m].Cells[7].Value.ToString();

                    if (this.dataNRule.Rows[m].Cells[8].Value.ToString() == "Y")
                        cn.IsGather = true;
                    else
                        cn.IsGather = false;

                    cn.GatherStartPos = this.dataNRule.Rows[m].Cells[9].Value.ToString();
                    cn.GatherEndPos = this.dataNRule.Rows[m].Cells[10].Value.ToString();

                    if (this.dataNRule.Rows[m].Cells[11].Value.ToString() == "Y")
                        cn.IsNaviNextPage = true;
                    else
                        cn.IsNaviNextPage = false;

                    cn.NaviNextPage = this.dataNRule.Rows[m].Cells[12].Value.ToString();
                   

                    cn.NaviNextMaxPage = this.dataNRule.Rows[m].Cells[14].Value.ToString();
                    cn.RunRule=(cGlobalParas.NaviRunRule)int.Parse ( this.dataNRule.Rows[m].Cells[15].Value.ToString());
                    cn.OtherNaviRule=this.dataNRule.Rows[m].Cells[16].Value.ToString();

                    cURule.nRules.Url =this.txtWebLink.Text;
                    cURule.nRules.NavigRule.Add (cn);
                }

              
            }
            else
            {
                //添加普通网址

                cURule.IsNav = false;

                if (this.IsAutoNextPage.Checked == true)
                {
                    cURule.IsNext = true;
                    cURule.NextRule = this.txtNextPage.Text.ToString();
                    cURule.MaxPageCount = int.Parse (this.numMaxNextPage.Value.ToString());
                }
                
            }



            //判断是否带有多页采集规则

            if (this.IsTabGather.Checked == true)
            {
                cURule.IsMulti =true ;
                cURule.isMulti121 = this.is121.Checked;

                for (int i = 0; i < this.dataMultiPage.Rows.Count; i++)
                {
                    eMultiPageRule mPage = new eMultiPageRule();
                    mPage.RuleName = this.dataMultiPage.Rows[i].Cells[0].Value.ToString();
                    mPage.mLevel = int.Parse (this.dataMultiPage.Rows[i].Cells[1].Value.ToString());
                    mPage.Rule = this.dataMultiPage.Rows[i].Cells[2].Value.ToString();

                    cURule.MRules.Url = this.txtWebLink.Text;

                    cURule.MRules.MultiPageRule.Add (mPage );
                }


            }
            else
            {
                cURule.IsMulti =false ;
            }


            rGUrl (this.fState ,cURule);
            this.Close ();

        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataMultiPage_DoubleClick(object sender, EventArgs e)
        {
            EditMultiRule();
        }

        private void EditMultiRule()
        {
            if (this.dataMultiPage.Rows.Count == 0)
            {
                return;
            }

            int curr = this.dataMultiPage.CurrentCell.RowIndex;

            int maxLevel = 0;
            maxLevel = this.dataNRule.Rows.Count;

            frmAddMultiPage f = new frmAddMultiPage(this.dataMultiPage.Rows[curr].Cells[0].Value.ToString(), 
                this.dataMultiPage.Rows[curr].Cells[1].Value.ToString(),
                this.dataMultiPage.Rows[curr].Cells[2].Value.ToString(),maxLevel);

            f.fState = cGlobalParas.FormState.Edit;
            f.rNavRule = new frmAddMultiPage.ReturnNavRule(this.GetNavRule);
            f.ShowDialog();
            f.Dispose();
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

            string uPara = string.Empty;

            if (Regex.IsMatch(e.ClickedItem.ToString(), "[{].*[}]"))
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[{].*[}]");

                string str = s.Value.ToString();

                if (!str.StartsWith("{Dict", StringComparison.CurrentCultureIgnoreCase) &&
                    !str.StartsWith("{CurrentDate", StringComparison.CurrentCultureIgnoreCase) &&
                    !str.StartsWith("{Date", StringComparison.CurrentCultureIgnoreCase) &&
                    !str.StartsWith("{Timestamp", StringComparison.CurrentCultureIgnoreCase) &&
                    !str.StartsWith("{Refer", StringComparison.CurrentCultureIgnoreCase))
                {
                    frmAddUrlPara f = new frmAddUrlPara(str);
                    f.rUrlPara = this.GetUrlPara;
                    if (f.ShowDialog() == DialogResult.Cancel)
                    {
                        f.Dispose();
                        return;
                    }
                    f.Dispose();
                    uPara = this.m_getUrlPara;
                }
                else
                    uPara = s.Groups[0].Value;
            }
            else
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[<].*[>]");
                uPara = s.Groups[0].Value;
            }



            //int startPos = this.startIndex;
            //int l = this.selectLength;

            int startPos  = this.txtWebLink.SelectionStart;
            int l = this.txtWebLink.SelectionLength;

            this.txtWebLink.Text = this.txtWebLink.Text.Substring(0, startPos) + uPara + this.txtWebLink.Text.Substring(startPos + l, this.txtWebLink.Text.Length - startPos - l);

            this.txtWebLink.SelectionStart = startPos + uPara.Length;
            this.txtWebLink.ScrollToCaret();
        }

        private void GetUrlPara(string str)
        {
            this.m_getUrlPara = str;
        }

        private void GetPData(string strCookie, string pData)
        {
            //this.txtCookie.Text = strCookie;
            this.txtWebLink.Text += "<POST:ASCII>" + pData + "</POST>";
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            this.contextMenuStrip1.Items.Clear();

            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu1") + "{Num:1,100,1}", null, null, "rmenuAddNum"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu2") + "{Num:100,1,-1}", null, null, "rmenuDegreNum"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("递增变量自动补零" + "{NumZero:1,100,1}", null, null, "rmenuAddNumZero"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("递减变量自动补零" + "{NumZero:100,1,-1}", null, null, "rmenuDegreNumZero"));
            
            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("同步变量(仅与第一个参数同步)" + "{Syn:1,100,1}", null, null, "rmenuDegreNumZero"));

            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu3") + "{Letter:a,z}", null, null, "rmenuAddLetter"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu4") + "{Letter:z,a}", null, null, "rmenuDegreLetter"));
            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("时间戳{Timestamp:" + ToolUtil.GetTimestamp ().ToString () + "}", null, null, "rmenuTimestamp"));
            //this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("ReferUrl{Refer:ReferUrl}", null, null, "rmenuRefer"));
            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());

            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("自定义日期范围" + "{Date(yyyy-MM-dd):" + System.DateTime.Now.ToString("yyyy-MM-dd") + "," + DateTime.Now.AddMonths(1).ToString ("yyyy-MM-dd") + "}", null, null, "rmenuDate"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("当前日期" + "{CurrentDate:yyyyMMdd}", null, null, "rmenuDate"));

            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu5") + "<POST:ASCII> 可手动修改编码，譬如：UTF8", null, null, "rmenuPostPrefix"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu6") + "</POST>", null, null, "rmenuPostSuffix"));

            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu7"), null, null, "rmenuGetPostData"));
            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());

            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu15") + "<BASE64>", null, null, "rmenuBase64Prefix"));
            this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu16") + "</BASE64>", null, null, "rmenuBase64Suffix"));
            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());

            if (Program.SominerVersion != cGlobalParas.VersionType.Free)
            {
                if (this.m_TaskType == cGlobalParas.TaskType.ExternalPara)
                {
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu13") + "<EPara>", null, null, "rmenuGetEParaPrefix"));
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu14") + "</EPara>", null, null, "rmenuGetEParaSuffix"));
                    this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                }

                if (this.m_isIniDictMenu)
                {
                    ToolStripMenuItem dictMenu = new ToolStripMenuItem(rm.GetString("rmenu8"), null, null, "rmenuDict");
                    dictMenu.DropDownItemClicked += this.contextMenuStrip1_ItemClicked;
                    foreach (KeyValuePair<string, string> kv in this.m_dictMenu)
                    {
                        dictMenu.DropDownItems.Add(rm.GetString("rmenu8") + ":{Dict:" + kv.Value + "}");
                    }
                    this.contextMenuStrip1.Items.Add(dictMenu);
                }
                else
                {
                    m_dictMenu = new Dictionary<string, string>();

                    //初始化字典菜单的项目
                    oDict d = new oDict(Program.getPrjPath());

                    ToolStripMenuItem dictMenu = new ToolStripMenuItem(rm.GetString("rmenu8"), null, null, "rmenuDict");
                    dictMenu.DropDownItemClicked += this.contextMenuStrip1_ItemClicked;

                    foreach (string s in d.GetDictClass())
                    {
                        dictMenu.DropDownItems.Add(rm.GetString("rmenu8") + ":{Dict:" + s + "}");
                        m_dictMenu.Add(s, s);
                    }

                    this.contextMenuStrip1.Items.Add(dictMenu);

                    d.Dispose();
                    d = null;

                    this.m_isIniDictMenu = true;
                }
                
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(this.txtWebLink.Text.ToString());
        }

        private void txtWebLink_Leave(object sender, EventArgs e)
        {
            this.txtWebLink.Text = this.txtWebLink.Text.Replace("\\r\\n", "");
            this.txtWebLink.Text = this.txtWebLink.Text.Trim();
        }

        private void rmenuAddNRule_Click(object sender, EventArgs e)
        {
            AddNavRule();
        }

        private void rmenuEditNRule_Click(object sender, EventArgs e)
        {
            EditNavRule();
        }

        private void rmenuDelNRule_Click(object sender, EventArgs e)
        {
            this.dataNRule.Focus();
            SendKeys.Send("{Del}");
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if (this.dataNRule.SelectedRows.Count == 0)
            {
                this.rmenuEditNRule.Enabled = false;
                this.rmenuDelNRule.Enabled = false;
            }
            else
            {
                this.rmenuEditNRule.Enabled = true ;
                this.rmenuDelNRule.Enabled = true ;
            }
        }

        private void rmenuAddMRule_Click(object sender, EventArgs e)
        {
            AddMultiRule();
        }

        private void rmenuEditMRule_Click(object sender, EventArgs e)
        {
            EditMultiRule();
        }

        private void rmenuDelMRule_Click(object sender, EventArgs e)
        {
            this.dataMultiPage.Focus();
            SendKeys.Send("{Del}");
        }

        private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
        {
            if (this.dataMultiPage.SelectedRows.Count == 0)
            {
                this.rmenuEditMRule.Enabled = false;
                this.rmenuDelMRule.Enabled = false;
            }
            else
            {
                this.rmenuEditMRule.Enabled = true ;
                this.rmenuDelMRule.Enabled = true ;
            }
        }

        private void dataNRule_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && this.dataNRule.SelectedRows.Count > 0)
            {
                EditNavRule();
            }
        }

        private void dataNRule_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataNRule.Rows.Count <=1)
            {
                this.cmdUp.Enabled =false ;
                this.cmdDown .Enabled =false ;
                
            }
            else if (e.RowIndex ==0)
            {
                this.cmdUp.Enabled = false;
                this.cmdDown.Enabled = true ;
            }
            else if (e.RowIndex == this.dataNRule.Rows.Count - 1)
            {
                this.cmdUp.Enabled = true ;
                this.cmdDown.Enabled = false;
            }
            else
            {
                this.cmdUp.Enabled = true;
                this.cmdDown.Enabled = true ;
            }
        }

        private void cmdUp_Click(object sender, EventArgs e)
        {
            int rowIndex = this.dataNRule.SelectedCells[0].RowIndex;
            if (rowIndex == 0)
            {
                MessageBox.Show("已经到第一层导航，无法再继续上移！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow r = new DataGridViewRow();
            r = this.dataNRule.SelectedRows[0];

            this.dataNRule.Rows.RemoveAt(rowIndex);

            this.dataNRule.Rows.Insert(rowIndex-1,r);
            this.dataNRule.Rows[rowIndex - 1].Selected = true;
            for (int i = 0; i < this.dataNRule.Rows.Count; i++)
            {
                this.dataNRule.Rows[i].Cells[0].Value = i + 1;
            }
        }

        private void cmdDown_Click(object sender, EventArgs e)
        {
            int rowIndex = this.dataNRule.SelectedCells[0].RowIndex;
            if (rowIndex == this.dataNRule.Rows.Count -1)
            {
                MessageBox.Show("已经到最底层导航，无法再继续下移！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow r = new DataGridViewRow();
            r = this.dataNRule.SelectedRows[0];

            this.dataNRule.Rows.RemoveAt(rowIndex);

            this.dataNRule.Rows.Insert(rowIndex +1, r);
            this.dataNRule.Rows[rowIndex + 1].Selected = true;
            for (int i = 0; i < this.dataNRule.Rows.Count; i++)
            {
                this.dataNRule.Rows[i].Cells[0].Value = i + 1;
            }
        }

        private void cmdImportUrlByDb_Click(object sender, EventArgs e)
        {
            frmAddUrlFromDb f = new frmAddUrlFromDb();
            f.fState = cGlobalParas.FormState.New;
            f.rGUrl = new frmAddUrlFromDb.ReturnGatherUrl(this.AddDbUrl);
            f.ShowDialog();
            f.Dispose();
        }

        private void AddDbUrl(cGlobalParas.FormState fState, cGatherUrlFormConfig fConfig)
        {
            string strDb = fConfig.Url;

            int startPos = this.txtWebLink.SelectionStart;
            int l = this.txtWebLink.SelectionLength;

            this.txtWebLink.Text = this.txtWebLink.Text.Substring(0, startPos) + strDb 
                + this.txtWebLink.Text.Substring(startPos + l, this.txtWebLink.Text.Length - startPos - l);

            this.txtWebLink.SelectionStart = startPos + strDb.Length;
            this.txtWebLink.ScrollToCaret();

            //this.txtWebLink.Text = fConfig.Url;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "请选择需要导入网址的文件";
            this.openFileDialog1.Filter = "文本文件(*.txt)|*.txt";

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fName = this.openFileDialog1.FileName;

                //判断文件是否存在
                if (!System.IO.File.Exists(fName))
                {
                    MessageBox.Show("您选择的文件不存在，请重新选择！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                //显示等待的窗口 
                frmWaiting fWait = new frmWaiting("正在导入网址,请等待...");
                new Thread((ThreadStart)delegate
                {
                    Application.Run(fWait);
                }).Start();
                //fWait.Show(this);


                int retValue = ImportUrl(fName);

                if (fWait.IsHandleCreated)
                    fWait.Invoke((EventHandler)delegate { fWait.Close(); });
                fWait = null;

                if (retValue > 2000 || retValue == 0)
                {
                    MessageBox.Show("共导入" + retValue + "个网址！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private int ImportUrl(string fName)
        {
            StreamReader fileReader = null;

            try
            {
                fileReader = new StreamReader(fName, System.Text.Encoding.GetEncoding("gb2312"));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("文件打开失败，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return 0;
            }
            string str = fileReader.ReadToEnd();
            string[] Urls = Regex.Split(str, "\r\n");

            ListViewItem[] items = new ListViewItem[Urls.Length];
            int i = 0;

            StringBuilder sb = new StringBuilder();

            for (i = 0; i < Urls.Length; i++)
            {
                sb.Append ( Urls[i] + "\r\n");
            }

            this.txtWebLink.AppendText( sb.ToString());

            fileReader.Close();
            fileReader = null;

            Urls = null;

            return i + 1;
        }


    }
}

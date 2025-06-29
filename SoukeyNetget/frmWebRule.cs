using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using NetMiner.Common;
using NetMiner.Resource;
using NetMiner.Publish.Rule;
using System.Collections;
using System.Text.RegularExpressions;
using SoukeyControl.WebBrowser;
using System.Reflection;
using System.Resources;
using NetMiner.Common.HttpSocket;
using System.IO;
using NetMiner.Core.pTask.Entity;

namespace MinerSpider
{
    public partial class frmWebRule : Form
    {
        private cGlobalParas.FormState m_fState;
        private bool IsSaveTemp = false;
        private string m_TName;

        public delegate void ReturnTemplate(string tName,cGlobalParas.PublishTemplateType tType,string remark);
        public ReturnTemplate RTemplate;

        private ResourceManager rm;

        private string m_InsertGParaText = "";

        public frmWebRule()
        {
            InitializeComponent();
            m_fState = cGlobalParas.FormState.New;

            //初始化资源文件的参数
            rm = new ResourceManager("NetMiner.Resource.Resources.globalPara", Assembly.Load("NetMiner.Resource"));

            this.comCode.Items.Add(rm.GetString("WebCode1"));
            this.comCode.Items.Add(rm.GetString("WebCode2"));
            this.comCode.Items.Add(rm.GetString("WebCode3"));
            this.comCode.Items.Add(rm.GetString("WebCode4"));
            this.comCode.Items.Add(rm.GetString("WebCode5"));
            this.comCode.Items.Add(rm.GetString("WebCode6"));

            this.comCode.SelectedIndex = 0;

        }

        private void frmWebRule_Load(object sender, EventArgs e)
        {
            this.txtLPara.Items.Add("{登录动态参数：用户名}");

            this.comLParaValue.Items.Add("{登录参数:用户名}");
            this.comLParaValue.Items.Add("{登录参数:密码}");
            this.comLParaValue.Items.Add("{登录参数:验证码}");
            this.comLParaValue.Items.Add("{系统参数:取来路页面表单值}");


            this.comUploadValue.Items.Add("{系统参数:分类}");
            this.comUploadValue.Items.Add("{上传参数:上传文件目录}");
            this.comUploadValue.Items.Add("{上传参数:上传文件}");
            this.comUploadValue.Items.Add("{上传参数:当前上传的文件名}");
            this.comUploadValue.Items.Add("{{系统参数正则:}");
        }

        public void IniData(cGlobalParas.FormState fState,string tName)
        {
            this.m_fState = fState;

            if (fState == cGlobalParas.FormState.New)
            {
                this.txtUploadReplace.Text = "{替换参数:}";
                return;
            }
            else
            {
                LoadTemplate(tName);
            }
        }

        private void LoadTemplate(string Name)
        {

            cTemplate temp = new cTemplate(Program.getPrjPath());
            temp.LoadTemplate(Name);

            //加载界面数据
            this.txtName.Text = temp.TempName;
            this.m_TName = temp.TempName;
            this.txtName.Tag = temp.TempType;
            this.comCode.Text = temp.uCode.GetDescription();
            this.txtDomain.Text = temp.Domain;
            this.txtRemark.Text = temp.Remark;
            this.udpInterval.Value = temp.PublishInterval;
            this.txtDemoDomain.Text = temp.TestDomain;
            this.txtDemoUser.Text = temp.TestUser;
            this.txtDemoPwd.Text = temp.TestPwd;
            this.txtPublishData.Text = temp.TestData;

            this.txtLoginUrl.Text  = temp.LoginUrl;
            this.txtLoginRUrl.Text = temp.LoginRUrl;
            this.txtLoginVCodeUrl.Text = temp.LoginVCodeUrl;
            foreach (KeyValuePair<string, string> de in temp.LoginParas)
            {
                ListViewItem cItem = new ListViewItem();
                cItem.Text = de.Key.ToString ();
                cItem.SubItems.Add(de.Value.ToString());
                this.listLoginParas.Items.Add(cItem);
            }
            this.txtLoginSuccess.Text  = temp.LoginSuccess;
            this.txtLoginFail.Text = temp.LoginFail;

            this.txtClassUrl.Text  = temp.ClassUrl;
            this.txtClassRUrl.Text = temp.ClassRUrl;
            this.txtClassStart.Text = temp.ClassStartCut;
            this.txtClassEnd.Text = temp.ClassEndCut;
            this.txtClassRegex.Text = temp.ClassRegex;

            this.txtPublishUrl.Text = temp.PublishUrl;
            this.txtPublishRUrl.Text = temp.PublishRUrl;

            foreach (KeyValuePair<string, string> de in temp.PublishParas)
            {
                ListViewItem cItem = new ListViewItem();
                cItem.Text = de.Key.ToString();
                cItem.SubItems.Add(de.Value.ToString());
                this.listPublishParas.Items.Add(cItem);
            }
            this.txtPublishSuccess.Text = temp.PublishSuccess;
            this.txtPublishFail.Text = temp.PublishFail;

            this.txtUploadUrl.Text = temp.UploadUrl;
            this.txtUploadRUrl.Text = temp.UploadRUrl;

            foreach (KeyValuePair<string, string> de in temp.UploadParas)
            {
                ListViewItem cItem = new ListViewItem();
                cItem.Text = de.Key.ToString();
                cItem.SubItems.Add(de.Value.ToString());
                this.listUpload.Items.Add(cItem);
            }

            this.txtUploadReplace.Text = temp.UploadReplace;

            this.IsHeader.Checked=temp.IsHeader ;
            this.txtHeader.Text=temp.Headers ;

            for (int i = 0; i < temp.pgPara.Count; i++)
            {
                ListViewItem cItem = new ListViewItem();
                cItem.Text = temp.pgPara[i].Label;
                cItem.SubItems.Add(temp.pgPara[i].pgPage.GetDescription());
                cItem.SubItems.Add(temp.pgPara[i].Value);

                this.listMPara.Items.Add(cItem);
            }

            if (temp.RUrlPageType == cGlobalParas.RUrlPageType.CurrentPage)
            {
                this.raRUrlCurrentPage.Checked = true;
                this.raRUrlPage.Checked = false;
            }
            else if (temp.RUrlPageType == cGlobalParas.RUrlPageType.CustomPage)
            {
                this.raRUrlCurrentPage.Checked = false;
                this.raRUrlPage.Checked = true;
            }
            this.txtRUrlPage.Text = temp.RUrlPage;
            this.txtRUrlRule.Text = temp.RUrlRule;
            this.IsPlugin.Checked = temp.IsVCodePlugin;
            this.txtVCodePlugin.Text = temp.VCodePlugin;

            temp = null;

            this.cmdApply.Enabled = false;

            this.IsSave.Text = "false";
        }

        private void IsSave_TextChanged(object sender, EventArgs e)
        {
            if (this.IsSave.Text == "True" && this.m_fState != cGlobalParas.FormState.Browser)
            {
                this.cmdApply.Enabled = true;
            }
            else if (this.IsSave.Text == "false")
            {
                this.cmdApply.Enabled = false;
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            if (IsSaveTemp == true)
            {
                RTemplate(this.txtName.Text, cGlobalParas.PublishTemplateType.Web, this.txtRemark.Text);
            }

       
            this.Close();

        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtDomain_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtLoginUrl_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Uri i = new Uri(this.txtLoginUrl.Text.Trim());
                if (i.Port ==80)
                    this.txtDomain.Text ="http://" + i.Host.ToString() ;
                else
                    this.txtDomain.Text = "http://" + i.Host.ToString() + ":"+ i.Port;
            }
            catch { }
            this.IsSave.Text = "True";
        }

        private void txtLoginRUrl_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtLoginVCodeUrl_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void cmdAddLoginPara_Click(object sender, EventArgs e)
        {
            ListViewItem cItem = new ListViewItem();
            cItem.Text = this.txtLPara.Text;
            cItem.SubItems.Add(this.comLParaValue.Text);
            this.listLoginParas.Items.Add(cItem);

            this.txtLPara.Text = "";
            this.comLParaValue.Text = "";

            this.IsSave.Text = "True";
        }

        private void cmdDelLoginPara_Click(object sender, EventArgs e)
        {
            if (this.listLoginParas.SelectedItems.Count == 0)
                return;

            this.listLoginParas.Items.Remove(this.listLoginParas.SelectedItems[0]);
            this.IsSave.Text = "True";
        }

        private void txtLoginSuccess_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtLoginFail_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtClassUrl_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtClassRUrl_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private string CreateRegexRule(string s,string e)
        {
            string reg = @"(?<=";

            s = ToolUtil.ClearFlag(s);

            s = ToolUtil.RegexReplaceTrans(s, false);

            s = Regex.Replace(s, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            s = s.Replace(@"\r\n", "");

            e = ToolUtil.ClearFlag(e);
            e = ToolUtil.RegexReplaceTrans(e, false);

            e = Regex.Replace(e, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            e = e.Replace(@"\r\n", "");

            string m = string.Empty;

            if (m == string.Empty)
            {
                m = ".";
            }

            reg += s + ")[" + m + "]+?(?=";
            reg += e + ")";

            return reg;
        }

        private void txtPublishUrl_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtPublishRUrl_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void cmdAddPublish_Click(object sender, EventArgs e)
        {
            ListViewItem cItem = new ListViewItem();
            cItem.Text = this.txtPublishLabel.Text;
            cItem.SubItems.Add(this.comPublishValue.Text);
            this.listPublishParas.Items.Add(cItem);

            this.txtPublishLabel.Text = "";
            this.comPublishValue.Text = "";

            LoadPublishLabel();

            this.IsSave.Text = "True";
        }

        private void cndDelPublish_Click(object sender, EventArgs e)
        {
            this.listPublishParas.Items.Remove(this.listPublishParas.SelectedItems[0]);

            LoadPublishLabel();

            this.IsSave.Text = "True";
        }

        private void txtPublishSuccess_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtPublishFail_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtRemark_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            if (!CheckInputvalidity())
            {
                return;
            }

            try
            {

                if (!SaveTemplate())
                {
                    return;
                }

                this.IsSave.Text = "false";

                IsSaveTemp = true;

                if (this.m_fState == cGlobalParas.FormState.New)
                {
                    this.m_fState = cGlobalParas.FormState.Edit;
                    this.m_TName = this.txtName.Text.Trim();
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("保存发生错误，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private bool CheckInputvalidity()
        {
            if (this.txtName.Text.Trim() == "")
            {
                MessageBox.Show("模版名称不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true ;
        }

        private bool SaveTemplate()
        {
            cTemplate temp = new cTemplate(Program.getPrjPath());

            if (this.m_fState == cGlobalParas.FormState.New)
            {
                this.m_TName = this.txtName.Text.Trim();
                this.m_fState = cGlobalParas.FormState.Edit;
            }
            else if (this.m_fState == cGlobalParas.FormState.Edit)
            {
                //删除原有任务的主要目的是为了备份，但如果发生错误，则忽略
                temp.DeleTemplate(this.m_TName);
                 
            }

            temp.TempName = this.txtName.Text.Trim();
            temp.TempType =cGlobalParas.PublishTemplateType.Web;
            temp.uCode =EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comCode.Text);
            temp.Remark =this.txtRemark.Text ;
            temp.PublishInterval = int.Parse(this.udpInterval.Value.ToString());
            temp.TestDomain = this.txtDemoDomain.Text;
            temp.TestUser = this.txtDemoUser.Text;
            temp.TestPwd = this.txtDemoPwd.Text;
            temp.TestData = this.txtPublishData.Text;
            
            temp.Domain =this.txtDomain.Text  ;
            temp.LoginUrl= this.txtLoginUrl.Text  ;
            temp.LoginRUrl= this.txtLoginRUrl.Text  ;
            temp.LoginVCodeUrl= this.txtLoginVCodeUrl.Text  ;
            
            for(int i=0;i<this.listLoginParas.Items.Count ;i++)
            {
                temp.LoginParas.Add(this.listLoginParas.Items[i].Text, this.listLoginParas.Items[i].SubItems[1].Text);
            }

            temp.LoginSuccess =this.txtLoginSuccess.Text ;
            temp.LoginFail=this.txtLoginFail.Text ;

            temp.ClassUrl =this.txtClassUrl.Text ;
            temp.ClassRUrl=this.txtClassRUrl.Text  ;
            temp.ClassStartCut = this.txtClassStart.Text;
            temp.ClassEndCut = this.txtClassEnd.Text;
            temp.ClassRegex = this.txtClassRegex.Text;

            temp.PublishUrl =this.txtPublishUrl.Text ;
            temp.PublishRUrl=this.txtPublishRUrl.Text ;
            
            for (int i = 0; i < this.listPublishParas.Items.Count ; i++)
            {
                temp.PublishParas.Add(this.listPublishParas.Items[i].Text, this.listPublishParas.Items[i].SubItems[1].Text);
            }
            
            temp.PublishSuccess=this.txtPublishSuccess.Text ;
            temp.PublishFail=this.txtPublishFail.Text ;

            temp.UploadUrl = this.txtUploadUrl.Text;
            temp.UploadRUrl = this.txtUploadRUrl.Text;
            for (int i = 0; i < this.listUpload.Items.Count; i++)
            {
                temp.UploadParas.Add(this.listUpload.Items[i].Text, this.listUpload.Items[i].SubItems[1].Text);
            }

            temp.UploadReplace = this.txtUploadReplace.Text;

            temp.IsHeader = this.IsHeader.Checked;
            temp.Headers = this.txtHeader.Text;

            for (int i = 0; i < this.listMPara.Items.Count; i++)
            {
                cPublishGlobalPara gPara = new cPublishGlobalPara();
                gPara.Label = this.listMPara.Items[i].Text;
                gPara.pgPage = EnumUtil.GetEnumName<cGlobalParas.PublishGlobalParaPage>(this.listMPara.Items[i].SubItems [1] .Text);
                gPara.Value = this.listMPara.Items[i].SubItems [2].Text ;
                temp.pgPara.Add(gPara);
            }

            if (this.raRUrlCurrentPage.Checked == true)
                temp.RUrlPageType = cGlobalParas.RUrlPageType.CurrentPage;
            else if (this.raRUrlPage.Checked == true)
                temp.RUrlPageType = cGlobalParas.RUrlPageType.CustomPage;
            temp.RUrlPage = this.txtRUrlPage.Text;
            temp.RUrlRule = this.txtRUrlRule.Text;
            temp.IsVCodePlugin = this.IsPlugin.Checked;
            temp.VCodePlugin = this.txtVCodePlugin.Text;

            temp.Save(this.txtName.Text.Trim());
            temp = null;

            return true;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (this.IsSave.Text == "false")
            {
                RTemplate(this.txtName.Text, cGlobalParas.PublishTemplateType.Web, this.txtRemark.Text);
                this.Close();
                return;
            }

            if (!CheckInputvalidity())
            {
                return;
            }

            try
            {

                if (!SaveTemplate())
                {
                    return;
                }

                this.IsSave.Text = "false";

                IsSaveTemp = true;

                if (this.m_fState == cGlobalParas.FormState.New)
                {
                    this.m_fState = cGlobalParas.FormState.Edit;
                    this.m_TName = this.txtName.Text.Trim();
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("保存发生错误，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RTemplate( this.txtName.Text, cGlobalParas.PublishTemplateType.Web, this.txtRemark.Text);

            this.Close();
        }

        private void txtLPara_TextChanged(object sender, EventArgs e)
        {
            SetCmdAddLogin();
        }

        private void SetCmdAddLogin()
        {
            if (txtLPara.Text.Trim() != "")
            {
                this.cmdAddLoginPara.Enabled = true;
            }
            else
            {
                this.cmdAddLoginPara.Enabled = false ;
            }
        }

        private void listLoginParas_Click(object sender, EventArgs e)
        {
            if (this.listLoginParas.SelectedItems.Count == 0)
            {
                this.cmdDelLoginPara.Enabled = false;
                this.cmdEditLoginPara.Enabled = false;
            }
            else
            {
                this.cmdEditLoginPara.Enabled = true;
                this.cmdDelLoginPara.Enabled = true;
                this.txtLPara.Text = this.listLoginParas.SelectedItems[0].Text;
                this.comLParaValue.Text = this.listLoginParas.SelectedItems[0].SubItems [1].Text ;
            }
        }

        private void comLParaValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comLParaValue_TextChanged(object sender, EventArgs e)
        {
            SetCmdAddLogin();
        }

        private void listPublishParas_Click(object sender, EventArgs e)
        {
            if (this.listPublishParas.SelectedItems.Count == 0)
            {
                this.cmdDelPublish.Enabled = false;
                this.cmdEditPublish.Enabled = false;
            }
            else
            {
                this.cmdDelPublish.Enabled = true;
                this.cmdEditPublish.Enabled = true;
                if (this.listPublishParas.SelectedItems[0].Text.IndexOf("{系统参数：上传文件}") > -1)
                {
                    this.raUplaodPPara.Checked = true;
                }
                else if (this.listPublishParas.SelectedItems[0].Text.IndexOf("{动态参数") > -1)
                {
                    this.raPParaToken.Checked=true ;
                }
                else
                {
                    this.raPPara.Checked = true;
                }
                this.txtPublishLabel.Text = this.listPublishParas.SelectedItems[0].Text;
                this.comPublishValue.Text = this.listPublishParas.SelectedItems[0].SubItems[1].Text;
                
            }
        }

      
        private void SetCmdAddPublish()
        {
            if (this.txtPublishLabel.Text.Trim() != "")
                this.cmdAddPublish.Enabled = true;
            else
                this.cmdAddPublish.Enabled =false;
        }

        private void txtPublishLabel_TextChanged(object sender, EventArgs e)
        {
            SetCmdAddPublish();
        }

        private void comPublishValue_TextChanged(object sender, EventArgs e)
        {
            SetCmdAddPublish();
        }

        private void listLoginParas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (this.listLoginParas.SelectedItems.Count != 0)
                {
                    this.listLoginParas.Items.Remove(this.listLoginParas.SelectedItems[0]);
                    this.IsSave.Text = "True";
                }
            }
        }

       

        private void listPublishParas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (this.listPublishParas.SelectedItems.Count > 0)
                {
                    this.listPublishParas.Items.Remove(this.listPublishParas.SelectedItems[0]);
                    this.IsSave.Text = "True";
                }
            }
        }

        private void cmdAutoLogin_Click(object sender, EventArgs e)
        {
            frmBrowser wftm = new frmBrowser(this.txtLoginUrl.Text);
            wftm.getFlag = 1;
            wftm.rPData = new frmBrowser.ReturnPOST(GetPData);
            wftm.ShowDialog();
            wftm.Dispose();
        }

        private void GetPData(string strCookie, string pData)
        {
            if (pData.EndsWith("\0"))
            {
                pData = pData.Substring(0, pData.Length - 1);
            }

           
            string[] paras = pData.Split('&');
            for (int i = 0; i < paras.Length; i++)
            {
                string label = paras[i].Substring(0, paras[i].IndexOf("="));
                string labelvalues = paras[i].Substring(paras[i].IndexOf("=") + 1, paras[i].Length - paras[i].IndexOf("=") - 1);

                ListViewItem cItem = new ListViewItem();
                cItem.Text = label;
                cItem.SubItems.Add(labelvalues);
                this.listLoginParas.Items.Add(cItem);
            }
            

            this.IsSave.Text = "True";

        }

        private void cmdEditLoginPara_Click(object sender, EventArgs e)
        {
            this.listLoginParas.SelectedItems[0].Text = this.txtLPara.Text;
            this.listLoginParas.SelectedItems[0].SubItems[1].Text =this.comLParaValue.Text;
       
            this.txtLPara.Text = "";
            this.comLParaValue.Text = "";

            this.IsSave.Text = "True";
        }

        private void cmdEditPublish_Click(object sender, EventArgs e)
        {
            this.listPublishParas.SelectedItems[0].Text = this.txtPublishLabel.Text;
            this.listPublishParas.SelectedItems[0].SubItems[1].Text =this.comPublishValue.Text;

            this.txtPublishLabel.Text = "";
            this.comPublishValue.Text = "";

            LoadPublishLabel();

            this.IsSave.Text = "True";
        }

        private void cmdAutoPublish_Click(object sender, EventArgs e)
        {
            
        }

        private void GetPostData(string header, string pData)
        {
            string[] headers=header.Split ('\n');
            string boundary = "";

            for (int i = 0; i < headers.Length ; i++)
            {
                string ss = headers[i].ToString();

                if (ss.StartsWith("Content-Type"))
                {
                    if (ss.Contains("multipart/form-data"))
                    {
                        boundary = ss.Substring(ss.IndexOf("boundary=")+9, ss.Length - ss.IndexOf("boundary=")-9);
                        boundary = boundary.Replace("\r", "");
                        boundary = boundary.Replace("\n", "");
                        boundary = "--" + boundary;
                    }
                }
            }

            if (this.tabControl1.SelectedIndex == 2)
            {
                if (boundary == "")
                {
                    string[] paras = pData.Split('&');
                    for (int i = 0; i < paras.Length; i++)
                    {
                        string label = paras[i].Substring(0, paras[i].IndexOf("="));
                        string labelvalues = paras[i].Substring(paras[i].IndexOf("=") + 1, paras[i].Length - paras[i].IndexOf("=") - 1);

                        ListViewItem cItem = new ListViewItem();
                        cItem.Text = label;
                        cItem.SubItems.Add(labelvalues);
                        this.listPublishParas.Items.Add(cItem);
                    }
                }
                else
                {
                    string[] paras = Regex.Split(pData, boundary);

                    for (int i = 0; i < paras.Length; i++)
                    {
                        string s = paras[i];

                        Match charSetMatch = Regex.Match(paras[i], "(?<=name=\").+?(?=\")", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                        string label = charSetMatch.ToString();
                        string labelvalues = "";

                        string[] ss = s.Split('\n');
                        for (int j = 0; j < ss.Length; j++)
                        {
                            string s1 = ss[j].Replace("\r", "");
                            if (!s1.StartsWith("Content-Disposition: form-data", StringComparison.CurrentCultureIgnoreCase))
                            {
                                labelvalues += s1;
                            }
                        }

                        if (label != "")
                        {
                            ListViewItem cItem = new ListViewItem();
                            cItem.Text = label;
                            cItem.SubItems.Add(labelvalues);
                            this.listPublishParas.Items.Add(cItem);
                        }
                    }
                }
            }
            else if (this.tabControl1.SelectedIndex == 3)
            {
                if (boundary == "")
                {
                    string[] paras = pData.Split('&');
                    for (int i = 0; i < paras.Length; i++)
                    {
                        string label = paras[i].Substring(0, paras[i].IndexOf("="));
                        string labelvalues = paras[i].Substring(paras[i].IndexOf("=") + 1, paras[i].Length - paras[i].IndexOf("=") - 1);

                        ListViewItem cItem = new ListViewItem();
                        cItem.Text = label;
                        cItem.SubItems.Add(labelvalues);
                        this.listUpload.Items.Add(cItem);
                    }
                }
                else
                {
                    string[] paras = Regex.Split(pData, boundary);

                    for (int i = 0; i < paras.Length; i++)
                    {
                        string s = paras[i];

                        Match charSetMatch = Regex.Match(paras[i], "(?<=name=\").+?(?=\")", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                        string label = charSetMatch.ToString();
                        string labelvalues = "";

                        string[] ss = s.Split('\n');
                        for (int j = 0; j < ss.Length; j++)
                        {
                            string s1 = ss[j].Replace("\r", "");
                            if (!s1.StartsWith("Content-Disposition: form-data", StringComparison.CurrentCultureIgnoreCase))
                            {
                                labelvalues += s1;
                            }
                        }

                        if (label != "")
                        {
                            ListViewItem cItem = new ListViewItem();
                            cItem.Text = label;
                            cItem.SubItems.Add(labelvalues);
                            this.listUpload.Items.Add(cItem);
                        }
                    }
                }
            }

            this.IsSave.Text = "True";
        }

        private void frmWebRule_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.IsSave.Text == "True")
            {
                if (MessageBox.Show("已经对规则进行了修改，是否不保存直接退出？","网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    e.Cancel = true;
                return;
            }
            
        }

        private void cmdAddUpload_Click(object sender, EventArgs e)
        {
            ListViewItem cItem = new ListViewItem();
            cItem.Text = this.txtUploadLabel.Text;
            cItem.SubItems.Add(this.comUploadValue.Text);
            this.listUpload.Items.Add(cItem);

            this.txtUploadLabel.Text = "";
            this.comUploadValue.Text = "";

            this.IsSave.Text = "True";
        }

        private void cmdEditUpload_Click(object sender, EventArgs e)
        {
            this.listUpload.SelectedItems[0].Text = this.txtUploadLabel.Text;
            this.listUpload.SelectedItems[0].SubItems[1].Text = this.comUploadValue.Text;

            this.txtUploadLabel.Text = "";
            this.comUploadValue.Text = "";

            this.IsSave.Text = "True";
        }

        private void cmdDelUpload_Click(object sender, EventArgs e)
        {
            if (this.listUpload.SelectedItems.Count == 0)
                return;

            this.listUpload.Items.Remove(this.listUpload.SelectedItems[0]);
            this.IsSave.Text = "True";
        }

        private void listUpload_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (this.listUpload.SelectedItems.Count != 0)
                {
                    this.listUpload.Items.Remove(this.listUpload.SelectedItems[0]);
                    this.IsSave.Text = "True";
                }
            }
        }

        private void listUpload_Click(object sender, EventArgs e)
        {
            if (this.listUpload.SelectedItems.Count == 0)
            {
                this.cmdEditUpload.Enabled = false;
                this.cmdDelUpload .Enabled = false;
            }
            else
            {
                this.cmdEditUpload.Enabled = true;
                this.cmdDelUpload.Enabled = true;
                this.txtUploadLabel.Text = this.listUpload.SelectedItems[0].Text;
                this.comUploadValue.Text = this.listUpload.SelectedItems[0].SubItems[1].Text;
            }
        }

        private void cmdAutoUpload_Click(object sender, EventArgs e)
        {
           
           
        
        }

        private void txtUploadUrl_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtUploadRUrl_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtUploadReplace_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void frmWebRule_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void comCode_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void IsHeader_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsHeader.Checked == true)
            {
                //this.txtHeader.Enabled = true;
                this.button1.Enabled = true;
            }
            else
            {
                //this.txtHeader.Enabled = false;
                this.button1.Enabled = false;
            }
            this.IsSave.Text = "True";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmSetHeader f = new frmSetHeader();
            f.strHeader = this.txtHeader.Text;
            f.rHeader = GetHttpHeader;
            f.ShowDialog();
            f.Dispose();
        }

        private void GetHttpHeader(string strHeader)
        {
            this.txtHeader.Text = strHeader;
        }

        private void txtHeader_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void udpInterval_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtLoginUrl_Leave(object sender, EventArgs e)
        {
           
        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Show(this.button2, 0, 22);
        }

        private void txtGetIDStart_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtGetIDEnd_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
        }

        private void rmenuWildcard_Click(object sender, EventArgs e)
        {
            string s="<Wildcard>(.+?)</Wildcard>";

            int startPos = this.txtClassRegex.SelectionStart;
            int l = this.txtClassRegex.SelectionLength;

            this.txtClassRegex.Text = this.txtClassRegex.Text.Substring(0, startPos) + s
                + this.txtClassRegex.Text.Substring(startPos + l, this.txtClassRegex.Text.Length - startPos - l);

            this.txtClassRegex.SelectionStart = startPos + s.Length ;
            this.txtClassRegex.ScrollToCaret();
        }

        private void rmenuClassID_Click(object sender, EventArgs e)
        {
            string s="{Para:ClassID}";

            int startPos = this.txtClassRegex.SelectionStart;
            int l = this.txtClassRegex.SelectionLength;

            this.txtClassRegex.Text = this.txtClassRegex.Text.Substring(0, startPos) + s
                + this.txtClassRegex.Text.Substring(startPos + l, this.txtClassRegex.Text.Length - startPos - l);

            this.txtClassRegex.SelectionStart = startPos + s.Length ;
            this.txtClassRegex.ScrollToCaret();
        }

        private void rmenuClass_Click(object sender, EventArgs e)
        {
            string s="{Para:Class}";

            int startPos = this.txtClassRegex.SelectionStart;
            int l = this.txtClassRegex.SelectionLength;

            this.txtClassRegex.Text = this.txtClassRegex.Text.Substring(0, startPos) + s
                + this.txtClassRegex.Text.Substring(startPos + l, this.txtClassRegex.Text.Length - startPos - l);

            this.txtClassRegex.SelectionStart = startPos + s.Length ;
            this.txtClassRegex.ScrollToCaret();
        }

        private void txtUploadLabel_TextChanged(object sender, EventArgs e)
        {
            if (txtUploadLabel.Text.Trim() != "")
            {
                this.cmdAddUpload.Enabled = true;
            }
            else
            {
                this.cmdAddUpload.Enabled = false;
            }
        }

        private void CreateNaviRule()
        {
            string reg = @"(?<=";

            this.txtUploadStart.Text = ToolUtil.ClearFlag(this.txtUploadStart.Text);

            string s = ToolUtil.RegexReplaceTrans(txtUploadStart.Text, false);

            s = Regex.Replace(s, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            s = s.Replace(@"\r\n", "");

            this.txtUploadEnd.Text = ToolUtil.ClearFlag(this.txtUploadEnd.Text);
            string e = ToolUtil.RegexReplaceTrans(txtUploadEnd.Text, false);

            e = Regex.Replace(e, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            e = e.Replace(@"\r\n", "");

            string m = string.Empty;
           
            if (m == string.Empty)
            {
                m = ".*";
            }

            reg += s + ")" + m + "(?=";
            reg += e + ")";

            string start = "";
            string end = "";
            string sTest=this.txtUploadReplace.Text ;

            start = sTest.Substring(0, sTest.IndexOf("{替换参数:"));
            end = sTest.Substring(sTest.IndexOf("}") + 1, sTest.Length - sTest.IndexOf("}") - 1);


            this.txtUploadReplace.Text = start + "{替换参数:" + reg + "}" + end;
        }

        private void txtUploadStart_TextChanged(object sender, EventArgs e)
        {
            try
            {
                CreateNaviRule();
            }
            catch { }
        }

        private void txtUploadEnd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                CreateNaviRule();
            }
            catch { }
        }

        private void raPPara_CheckedChanged(object sender, EventArgs e)
        {
            if (raPPara.Checked == true)
            {
                this.txtPublishLabel.Text = "";
            }
        }

        private void raUplaodPPara_CheckedChanged(object sender, EventArgs e)
        {
            if (raUplaodPPara.Checked == true)
            {
                this.txtPublishLabel.Text = "{系统参数：上传文件}";
                this.comPublishValue.Text = "";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.listMPara.Items.Remove(this.listMPara.SelectedItems[0]);
            this.IsSave.Text = "True";
        }

        private void listMPara_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                this.listMPara.Items.Remove(this.listMPara.SelectedItems[0]);
                this.IsSave.Text = "True";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ListViewItem cItem = new ListViewItem();
            cItem.Text = this.txtMLabel.Text.Trim();
            if (this.raLogin.Checked == true)
                cItem.SubItems.Add(rm.GetString("PublishGlobalParaPage1").ToString());
            else if (this.raPublish.Checked ==true )
                cItem.SubItems.Add(rm.GetString("PublishGlobalParaPage2").ToString());
            cItem.SubItems.Add(this.txtMValue.Text.Trim());
            this.listMPara.Items.Add(cItem);

            this.txtMLabel.Text = "";
            this.txtMValue.Text = "";
            this.raLogin.Checked = true;

            this.IsSave.Text = "True";
        }

        private void SetCmdMPara()
        {
            if (this.txtMLabel.Text.Trim() != "" && this.txtMValue.Text.Trim ()!="")
                this.button5.Enabled = true;
            else
                this.button5.Enabled = false;
        }

        private void txtMLabel_TextChanged(object sender, EventArgs e)
        {
            SetCmdMPara();
        }

        private void txtMValue_TextChanged(object sender, EventArgs e)
        {
            SetCmdMPara();
        }

        private void listMPara_Click(object sender, EventArgs e)
        {
            if (this.listMPara.SelectedItems.Count == 0)
            {
                this.button3.Enabled = false;
                this.button4.Enabled = false;
            }
            else
            {
                this.button3.Enabled = true;
                this.button4.Enabled = true;


                this.txtMLabel.Text = this.listMPara.SelectedItems[0].Text;
                if (EnumUtil.GetEnumName<cGlobalParas.PublishGlobalParaPage> (this.listMPara.SelectedItems[0].SubItems[1].Text) == cGlobalParas.PublishGlobalParaPage.LoginPage)
                    this.raLogin.Checked = true;
                else
                    this.raPublish.Checked = true;
                this.txtMValue.Text = this.listMPara.SelectedItems[0].SubItems[2].Text;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.listMPara.SelectedItems[0].Text = this.txtMLabel.Text.Trim();
            if (this.raLogin.Checked == true)
                this.listMPara.SelectedItems[0].SubItems[1].Text= rm.GetString("PublishGlobalParaPage1").ToString();
            else if (this.raPublish.Checked == true)
                this.listMPara.SelectedItems[0].SubItems[1].Text =rm.GetString("PublishGlobalParaPage2").ToString();
            this.listMPara.SelectedItems[0].SubItems[2].Text =this.txtMValue.Text.Trim();

            this.IsSave.Text = "True";
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            //this.contextMenuStrip2.Items.Clear();

            //for (int i = 0; i < this.listMPara.Items.Count; i++)
            //{
            //    this.contextMenuStrip2.Items.Add("{全局参数:" + this.listMPara.Items[i].Text + "}");

            //}

            //this.contextMenuStrip2.Items.Add(new ToolStripSeparator());
            //this.contextMenuStrip2.Items.Add("{系统参数:分类编号}");
            //this.contextMenuStrip2.Items.Add("{系统参数:分类名称}");
            //this.contextMenuStrip2.Items.Add(new ToolStripSeparator());
            //this.contextMenuStrip2.Items.Add("{系统参数:随机数}");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            m_InsertGParaText = "txtPublishUrl";
            this.contextMenuStrip2.Show(this.button6, 0, 21);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            m_InsertGParaText = "txtPublishRUrl";
            this.contextMenuStrip2.Show(this.button7, 0, 21);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            m_InsertGParaText = "txtUploadUrl";
            this.contextMenuStrip2.Show(this.button9, 0, 21);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            m_InsertGParaText = "txtUploadRUrl";
            this.contextMenuStrip2.Show(this.button8, 0, 21);
        }

        private void contextMenuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string s = e.ClickedItem.Text;

            TextBox tBox = GetTextBox(this, this.m_InsertGParaText);

            int startPos = tBox.SelectionStart;
            int l = tBox.SelectionLength;

            tBox.Text = tBox.Text.Substring(0, startPos) + s
                + tBox.Text.Substring(startPos + l, tBox.Text.Length - startPos - l);

            tBox.SelectionStart = startPos + s.Length;
            tBox.ScrollToCaret();
        }

        private TextBox GetTextBox(Control ctrl,string tName)
        {
            foreach ( Control c in ctrl.Controls )
            {
               if ( c.Name ==tName )
              {
                    return (TextBox)c;
              }
              if ( c.Controls.Count != 0 )
              {
                 TextBox t= GetTextBox(c,tName );
                 if (t != null)
                     return t;
              }
           }

            return null; 
        }

        private void button11_Click(object sender, EventArgs e)
        {
            m_InsertGParaText = "txtClassUrl";
            this.contextMenuStrip2.Show(this.button11, 0, 21);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            m_InsertGParaText = "txtClassRUrl";
            this.contextMenuStrip2.Show(this.button10, 0, 21);
        }

        private void comPublishValue_DropDown(object sender, EventArgs e)
        {
            this.comPublishValue.Items.Clear();

            this.comPublishValue.Items.Add("{系统参数:分类}");
            this.comPublishValue.Items.Add("{打码参数:在此输入图片地址}");
            this.comPublishValue.Items.Add("{系统参数:取上传第一张图片}");

            this.comPublishValue.Items.Add("{系统参数:取来路页面表单值}");
            this.comPublishValue.Items.Add("{系统参数正则:}");

            this.comPublishValue.Items.Add("{上传参数:上传文件}");

            this.comPublishValue.Items.Add("{发布参数:标题}");
            this.comPublishValue.Items.Add("{发布参数:正文}");
            this.comPublishValue.Items.Add("{发布参数:发布时间}");
            this.comPublishValue.Items.Add("{发布参数:分类}");
            this.comPublishValue.Items.Add("{发布参数:来源}");
            this.comPublishValue.Items.Add("{发布参数:作者}");
            this.comPublishValue.Items.Add("{发布参数:点击量}");
            this.comPublishValue.Items.Add("{发布参数:浏览量}");
            this.comPublishValue.Items.Add("{发布参数:品名}");
            this.comPublishValue.Items.Add("{发布参数:单位}");
            this.comPublishValue.Items.Add("{发布参数:价格}");
            this.comPublishValue.Items.Add("{发布参数:单价}");
            this.comPublishValue.Items.Add("{发布参数:数量}");
            this.comPublishValue.Items.Add("{系统参数:时间}");
        }

        private void contextMenuStrip2_Opened(object sender, EventArgs e)
        {
            this.contextMenuStrip2.Items.Clear();

            for (int i = 0; i < this.listMPara.Items.Count; i++)
            {
                this.contextMenuStrip2.Items.Add("{全局参数:" + this.listMPara.Items[i].Text + "}");
            }
            this.contextMenuStrip2.Items.Add(new ToolStripSeparator());

            //需要排重
            Hashtable ht = new Hashtable();
            for (int i = 0; i < this.listPublishParas.Items.Count; i++)
            {
                if (this.listPublishParas.Items[i].SubItems[1].Text != "" 
                    && this.listPublishParas.Items[i].SubItems[1].Text.StartsWith("{发布参数")
                    && !ht.ContainsKey (this.listPublishParas.Items[i].SubItems[1].Text))
                {
                    ht.Add(this.listPublishParas.Items[i].SubItems[1].Text, this.listPublishParas.Items[i].SubItems[1].Text);
                    this.contextMenuStrip2.Items.Add(this.listPublishParas.Items[i].SubItems[1].Text);
                }
            }

            this.contextMenuStrip2.Items.Add(new ToolStripSeparator());
            this.contextMenuStrip2.Items.Add("{系统参数:分类编号}");
            this.contextMenuStrip2.Items.Add("{系统参数:分类名称}");
            //this.contextMenuStrip2.Items.Add(new ToolStripSeparator());
            //this.contextMenuStrip2.Items.Add("{系统参数:随机数}");

        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            if (!CheckInputvalidity())
            {
                return;
            }

            try
            {

                if (!SaveTemplate())
                {
                    return;
                }

                this.IsSave.Text = "false";

                IsSaveTemp = true;

                if (this.m_fState == cGlobalParas.FormState.New)
                {
                    this.m_fState = cGlobalParas.FormState.Edit;
                    this.m_TName = this.txtName.Text.Trim();
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("保存发生错误，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void cmdBrowserPlugins3_Click(object sender, EventArgs e)
        {

        }

        private void cmdBrowserPlugins3_Click_1(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "请选择插件文件";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "插件(*.dll)|*.dll";


            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtVCodePlugin.Text = this.openFileDialog1.FileName;
            }
        }

        private void IsPlugin_CheckedChanged(object sender, EventArgs e)
        {
            if (IsPlugin.Checked == true)
            {
                this.label39.Enabled = true;
                this.txtVCodePlugin.Enabled = true;
                this.cmdBrowserPlugins3.Enabled = true;
                this.cmdSetPlugins3.Enabled = true;
            }
            else
            {
                this.label39.Enabled = false ;
                this.txtVCodePlugin.Enabled = false ;
                this.cmdBrowserPlugins3.Enabled = false ;
                this.cmdSetPlugins3.Enabled = false ;
            
            }
            this.IsSave.Text = "True";
        }

        private void txtRUrlPage_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtRUrlRule_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtVCodePlugin_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void raRUrlCurrentPage_CheckedChanged(object sender, EventArgs e)
        {
            if (raRUrlCurrentPage.Checked == true)
            {
                this.txtRUrlPage.Enabled = false;
                this.button13.Enabled = false;
            }
            else
            {
                this.txtRUrlPage.Enabled = true;
                this.button13.Enabled = true;
            }

            this.IsSave.Text = "True";
        }

        private void raRUrlPage_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void cmdRurlRule_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip3.Show(this.cmdRurlRule, 0, 21);
        }

        private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
        {
            this.contextMenuStrip3.Items.Clear();

            for (int i = 0; i < this.listPublishParas.Items.Count; i++)
            {
                if (this.listPublishParas.Items[i].SubItems [1].Text !="")
                this.contextMenuStrip3.Items.Add("根据指定发布的内容获取连接：" + this.listPublishParas.Items[i].Text );
            }
        }

        private void contextMenuStrip3_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string ss=e.ClickedItem.Text ;
            string s = "{RUrlPara:" + ss.Substring(ss.IndexOf("：") + 1, ss.Length - ss.IndexOf("：") - 1) + "}";


            int startPos = this.txtRUrlRule.SelectionStart;
            int l = this.txtRUrlRule.SelectionLength;

            this.txtRUrlRule.Text = this.txtRUrlRule.Text.Substring(0, startPos) + s
                + this.txtRUrlRule.Text.Substring(startPos + l, this.txtRUrlRule.Text.Length - startPos - l);

            this.txtRUrlRule.SelectionStart = startPos + s.Length;
            this.txtRUrlRule.ScrollToCaret();



        }

        private void LoadPublishLabel()
        {
            this.txtPublishData.Text ="";

            for (int i = 0; i < this.listPublishParas.Items.Count; i++)
            {
                string ss = this.listPublishParas.Items[i].SubItems[1].Text;
                if (ss.Trim ().Length >0 && ss.IndexOf("}") > 0 && ss.IndexOf("参数") > 0)
                {
                    
                    Match a = Regex.Match(ss, "(?<=参数:).+?(?=})", RegexOptions.IgnoreCase);
                    
                    string value = a.Groups[0].Value.ToString();

                    //if (value == "分类")
                    //    value = "系统分类";
                    this.txtPublishData.Text +="[" + value + "]:" + "\r\n";
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            string tmpFile=string.Empty ;

            if (this.IsSave.Text == "True")
            {
                MessageBox.Show("请先保存发布模板，再进行发布测试！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                this.txtpLog.Text = "启动测试发布操作" + "\r\n";
                Application.DoEvents();

                //构建发布数据
                string[] ss = this.txtPublishData.Text.Trim().Split('\r');

                DataTable d = new DataTable();
                for (int i = 0; i < ss.Length; i++)
                {
                    if (ss[i].Trim() != "")
                    {
                        string s1 = ss[i].Replace("\n", "");
                        d.Columns.Add(s1.Substring(1, s1.IndexOf("]") - 1));
                    }
                }
                DataRow dRow = d.NewRow();
                for (int i = 0; i < ss.Length; i++)
                {
                    if (ss[i].Trim() != "")
                        dRow[i] = ss[i].Substring(ss[i].IndexOf("]") + 2, ss[i].Length - ss[i].IndexOf("]") - 2);
                }
                d.Rows.Add(dRow);

                this.txtpLog.Text += "发布数据构建完成" + "\r\n";
                Application.DoEvents();

                cTemplate template = new cTemplate(Program.getPrjPath());
                template.LoadTemplate(this.txtName.Text);

                string webSource = string.Empty;
                string cookie = string.Empty;

                //开始进行登录操作
                bool isLogin = cHttpSocket.Login(this.txtDemoUser.Text, this.txtDemoPwd.Text, template.uCode,
                    this.txtDemoDomain.Text, template.Domain, template.LoginUrl, template.LoginRUrl, template.LoginVCodeUrl, template.LoginSuccess, template.LoginFail,
                    template.LoginParas,template.IsVCodePlugin ,template.VCodePlugin, out cookie, out webSource);

                tmpFile=Program.getPrjPath () + "data\\~tmpLogin.html";
                SaveTempFile(tmpFile, webSource, this.comCode.Text);

                if (isLogin == true)
                {
                    this.txtpLog.Text += "登录成功 登录返回信息文件：" + "file:///" + tmpFile + "\r\n";
                }
                else
                {
                    this.txtpLog.Text += "登录失败，退出发布操作。登录返回信息文件：" + "file:///" + tmpFile + "\r\n";
                    return;
                }
                Application.DoEvents();

                Hashtable globalPara = new Hashtable();

                if (template.pgPara.Count > 0)
                {
                    globalPara = new Hashtable();

                    for (int i = 0; i < template.pgPara.Count; i++)
                    {
                        if (template.pgPara[i].pgPage == cGlobalParas.PublishGlobalParaPage.LoginPage)
                        {
                            string label = template.pgPara[i].Label;
                            string strReg = template.pgPara[i].Value;

                            Match a = Regex.Match(webSource, strReg, RegexOptions.IgnoreCase);
                            string value = a.Groups[0].Value.ToString();
                            globalPara.Add(label, value);
                        }
                    }
                }

                this.txtpLog.Text += "全局参数获取完毕" + "\r\n";
                Application.DoEvents();

                NetMiner.Publish.cPublishData cp = new NetMiner.Publish.cPublishData(Program.getPrjPath());

                Hashtable WebClass = cp.GetWebClass(this.txtName.Text,this.txtDemoDomain.Text , cookie, globalPara);

                List<ePublishData> pParas = new List<ePublishData>();
                //增加发布参数
                for (int i = 0; i < d.Columns.Count; i++)
                {
                    ePublishData pPara = new ePublishData();

                    pPara.DataType = cGlobalParas.PublishParaType.Custom;
                    pPara.DataLabel = d.Columns[i].ColumnName;
                    pPara.DataValue = d.Rows[0][i].ToString();

                    pParas.Add(pPara);
                }

                this.txtpLog.Text += "分类数据获取完毕" + "\r\n";
                Application.DoEvents();

                string result = string.Empty;
                object odrow= d.Rows[0].ItemArray.Clone();
                try
                {

                    result = cp.PublishByWebTemplate(d.Columns, odrow, this.txtName.Text, cookie,
                                                this.txtDemoDomain.Text, WebClass, pParas, globalPara,this.txtVCodePlugin.Text, out webSource );

                    tmpFile = Program.getPrjPath() + "data\\~tmpPublish.html";
                    SaveTempFile(tmpFile, webSource,this.comCode.Text );
                }
                catch (System.Exception ex)
                {
                    tmpFile = Program.getPrjPath() + "data\\~tmpPublish.html";
                    SaveTempFile(tmpFile, webSource, this.comCode.Text);

                    this.txtpLog.Text += "发布出现错误，错误信息：" + "file:///" + tmpFile + "\r\n";
                    Application.DoEvents();
                    cp = null;
                    return;
                }
                
                this.txtpLog.Text += "发布获取信息文件：" + "file:///" + tmpFile + "\r\n";

                if (result == "")
                {
                    this.txtpLog.Text += "发布成功，但未获取到回链地址" + "\r\n";
                    Application.DoEvents();
                }
                else
                {
                    this.txtpLog.Text += "发布成功，回链地址:" + result + "\r\n";
                    Application.DoEvents();
                }
                cp = null;
            }
            catch (System.Exception ex)
            {
                this.txtpLog.Text += "发布失败，错误信息:" + ex.Message  + "\r\n";
            }
        }

    

        private void txtDemoDomain_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtDemoUser_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtDemoPwd_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtPublishData_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void cmdSetPlugins3_Click(object sender, EventArgs e)
        {
            if (this.txtVCodePlugin.Text.Trim() == "")
            {
                MessageBox.Show("请先选择插件，然后再进行插件设置，选择插件请点击浏览按钮，选择插件dll文件！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtVCodePlugin.Focus();
                return;
            }

            NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
            rPlugin.CallSetConfig(this.txtVCodePlugin.Text);
            rPlugin = null;
        }

        private void button13_Click(object sender, EventArgs e)
        {

            m_InsertGParaText = "txtRUrlPage";
            this.contextMenuStrip2.Show(this.button13, 0, 21);
        
        }

        private void txtpLog_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void SaveTempFile(string fileName ,string Source,string code)
        {
            FileStream fStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Write);

            Encoding eCode=Encoding.Default ;

            if (code == "自动")
                eCode = Encoding.ASCII;
            else 
                eCode =Encoding.GetEncoding(code);

            StreamWriter sw = new StreamWriter(fStream, eCode);
            sw.Write(Source);
            sw.Close();
            sw.Dispose();
            fStream.Close();
            fStream.Dispose();
        }

        private void raPParaToken_CheckedChanged(object sender, EventArgs e)
        {
            if (raPParaToken.Checked == true)
            {
                this.txtPublishLabel.Text = "{动态参数：在此输入正则}";
                this.comPublishValue.Text = "";
            }
        }
       
    }
}

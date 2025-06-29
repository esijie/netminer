using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using NetMiner.Gather;
using System.Reflection;
using System.Resources;
using NetMiner.Resource;
using System.Net;
using NetMiner.Common;
using System.Text.RegularExpressions;

namespace MinerSpider
{
    public partial class frmHelpInfo : DockContent 
    {
        private ResourceManager rm;

        //����һ��ֵ����ʾ��ǰ�Ƿ������˿�ݲ�������
        private bool m_IsHideHelp = false;

        //����һ��������ȡע�ἤ��Ľ��
        private cGlobalParas.RegisterResult m_rResult;

        //����һ��ֵ����ʾ��ǰ�Ƿ��Ѿ��ƶ��״�
        private bool m_IsRadarStarted = false;

        public frmHelpInfo()
        {
            InitializeComponent();

            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));
        }

        delegate void MyDelegate();
        public void IniData()
        {
            MyDelegate openUrl = new MyDelegate(this.OpenHomePage);
            openUrl();
        }

        private void OpenHomePage()
        {
            //this.webBrowser.Navigate("http://www.netminer.cn/bbs/index.asp");
        }

        private void frmHelpInfo_Load(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Free && DateTime.Compare(DateTime.Now, Program.g_EndServiceDate) > 0)
            {
                this.labVersion.Text = "";

            }

            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise  ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate )
            {
                this.labVersion.Text = rm.GetString("Info225");
                this.labRegister.Visible = false;
            }
            else if (Program.SominerVersion == cGlobalParas.VersionType.Free)
            {
                this.linkBuy.Visible = true;
                this.labRegister.Visible = true;
            }
            //this.splitContainer1.SplitterDistance = 180;
           if (Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                this.labVer.Text = Program.SominerVersionEnter;

            }
            else if (Program.SominerVersion == cGlobalParas.VersionType.Ultimate)
            {
                this.labVer.Text = Program.SominerVersionUltimate;

            }
            else if (Program.SominerVersion == cGlobalParas.VersionType.Program)
            {
                this.labVer.Text = Program.SominerVersionProgram;
            }
           else if (Program.SominerVersion == cGlobalParas.VersionType.Cloud)
           {
               this.labVer.Text = Program.SominerVersionCloud;
           }
            else 
            {
                this.labVer.Text = Program.SominerVersionFree;
            }

            this.labTaskVersion.Text = Program.SominerTaskVersionCode;

            if (Program.SominerVersion == cGlobalParas.VersionType.Enterprise ||
               Program.SominerVersion == cGlobalParas.VersionType.Ultimate)
            {
                //this.treeMenu.Nodes["nodRadarRule"]. = true;
            }
            else
            {
                //���������״���ʾ
                this.label5.Visible = false;
                this.linkLabel3.Visible = false;
                this.label7.Visible = false;
                this.linkLabel1.Visible = false;
                this.groupBox3.Visible = false;
            }
        }

        private void frmHelpInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        #region �����¼� ���ڸ���������������Ϣ
        private readonly Object m_eventLock = new Object();

        private event EventHandler<ExcuteFunctionEvent> e_ExcuteFunction;
        public event EventHandler<ExcuteFunctionEvent> ExcuteFunction
        {
            add { lock (m_eventLock) { e_ExcuteFunction += value; } }
            remove { lock (m_eventLock) { e_ExcuteFunction -= value; } }
        }

        #endregion

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("NewTask", null));

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("NewPlan", null));
        }

        private void linkBuy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.netminer.cn/shop/index.aspx");
        }

      

        /// <summary>
        /// �����״����������ʾ��Ϣ
        /// </summary>
        /// <param name="IsStarted">true Ϊ����</param>
        public void SetRadarInfo(bool IsStarted)
        {
            if (IsStarted == true)
            {
                this.label7.Text = rm.GetString("Info246");
                this.linkLabel1.Text= rm.GetString ("Info249");
                m_IsRadarStarted = true;
            }
            else
            {
                this.label7.Text = rm.GetString("Info247");
                this.linkLabel1.Text = rm.GetString("Info248");
                m_IsRadarStarted = false;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
              Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {

                if (m_IsRadarStarted == true)
                {
                    SetRadarInfo(false);
                    if (e_ExcuteFunction != null)
                        e_ExcuteFunction(this, new ExcuteFunctionEvent("SetRadarState", new object[] { false }));
                }
                else
                {
                    SetRadarInfo(true);
                    if (e_ExcuteFunction != null)
                        e_ExcuteFunction(this, new ExcuteFunctionEvent("SetRadarState", new object[] { true }));
                }
            }
            else
            {
                MessageBox.Show("��ǰ�汾��֧�������״���ݼ�أ����ܣ����ȡ��ȷ�İ汾��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.toolmenuSilentRun.Checked = false;
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = "";
           
        }

        private void frmHelpInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("NewRadar", null));
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("NewTaskClass", null));
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("OpenHelp", null));
        }

        //private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    if (this.m_IsHideHelp == false)
        //    {
        //        //this.splitContainer1.SplitterDistance = 45;
        //        this.m_IsHideHelp = true;
        //        this.linkLabel6.Image = Bitmap.FromFile(Program.getPrjPath() + "img\\A60.gif");
        //        this.linkLabel6.Text = rm.GetString("Info244");
        //    }
        //    else
        //    {
        //        //this.splitContainer1.SplitterDistance = 178;
        //        this.m_IsHideHelp = false;
        //        this.linkLabel6.Image = Bitmap.FromFile(Program.getPrjPath() + "img\\A59.gif");
        //        this.linkLabel6.Text = rm.GetString("Info245");

        //    }
        //}

        private void butHelp1_Click(object sender, EventArgs e)
        {
            //frmHelp f = new frmHelp("�ɼ�����");
            //f.ShowDialog();
            //f.Dispose();

            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("NewTaskClass", null));
        }

        private void butHelp2_Click(object sender, EventArgs e)
        {
            //frmHelp f = new frmHelp("�ɼ�����");
            //f.ShowDialog();
            //f.Dispose();
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("NewTask", null));
        }

        private void butHelp3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("������ɼ�������࣬ѡ��ɼ������������ɣ������ɲ鿴�������еĲɼ�������Ҫ��ת�����������С�ô��"
                , "����� ѯ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (e_ExcuteFunction != null)
                    e_ExcuteFunction(this, new ExcuteFunctionEvent("ClickRunningNode", null));
            }
        }

        private void butHelp4_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("ClickCompletedNode", null));
        }

        private void butHelp5_Click(object sender, EventArgs e)
        {
            frmHelp f = new frmHelp("�������ݵ��ļ�");
            f.ShowDialog();
            f.Dispose();
        }

        private void butHelp6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("SoukeyDataPublish.exe"); 
        }

        private void butHelp7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("SoukeyDataPublish.exe"); 
        }

        private void butHelp8_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("NewPlan", null));
        }

        private void butHelp9_Click(object sender, EventArgs e)
        {
            //frmHelp f = new frmHelp("�ɼ��״�");
            //f.ShowDialog();
            //f.Dispose();
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("NewRadar", null));
        }

        private void butHelp10_Click(object sender, EventArgs e)
        {
           
        }

        private void isAutoCloseComputer_CheckedChanged(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("CheckIsCloseComputer",new object[]{this.isAutoCloseComputer.Checked}));
        }

        private void glassButton2_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("RunPublishTemplate", null));
        }

        private void glassButton1_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("RunDataPublish",null));
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("ImportTask", null));
        }

        //private void txtSearch_TextChanged(object sender, EventArgs e)
        //{
        //    this.listSearchWord.Items.Clear();

        //    string sWord = this.txtSearch.Text.Trim();

        //    if (sWord == "")
        //    {
        //        this.listSearchWord.Visible = false;
        //        return;
        //    }

        //    try
        //    {
        //        string url = Program.g_SerarchUrl + System.Web.HttpUtility.UrlEncode(sWord, Encoding.UTF8);
                
        //        WebClient myWebClient = new WebClient();
        //        myWebClient.Credentials = CredentialCache.DefaultCredentials;
        //        byte[] myDataBuffer;
              
        //        //����Դ�������ݲ������ֽ����顣����@����Ϊ��ַ�м���"/"���ţ�   
        //        myDataBuffer = myWebClient.DownloadData(@url);
        //        string webSource = Encoding.UTF8.GetString(myDataBuffer);
        //        myDataBuffer = null;
        //        myWebClient.Dispose();
        //        myWebClient = null;

        //        string[] ss = webSource.Split(',');
        //        if (ss.Length > 0)
        //        {
        //            for (int i = 0; i < ss.Length; i++)
        //            {
        //                this.listSearchWord.Items.Add(Regex.Replace(ss[i], "\\[|\\]|\"", ""));
        //            }

        //            this.listSearchWord.ItemHeight = 24;

        //            int h= this.listSearchWord.GetItemHeight(0);
        //            if (ss.Length >= 10)
        //                this.listSearchWord.Height = h * 10;
        //            else
        //                this.listSearchWord.Height = h * ss.Length;
                    
        //            this.listSearchWord.Visible = true;
        //        }
        //    }
        //    catch { }
        //}

        //private void listSearchWord_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    this.txtSearch.Text = this.listSearchWord.SelectedItem.ToString();

        //    string url = Program.g_SearchResult + System.Web.HttpUtility.UrlEncode(this.txtSearch.Text, Encoding.UTF8);

        //    OpenSearch(url);
        //    this.listSearchWord.Visible = false;
        //}

        private void OpenSearch(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        //private void butSearch_Click(object sender, EventArgs e)
        //{
        //    if (this.txtSearch.Text.Trim() == "")
        //    {
        //        MessageBox.Show("��������Ҫ�����Ĺؼ��ʣ�", "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        this.txtSearch.Focus();
        //        return;
        //    }

        //    this.listSearchWord.Visible = false;

        //    string url = Program.g_SearchResult + System.Web.HttpUtility.UrlEncode(this.txtSearch.Text.Trim(), Encoding.UTF8);

        //    OpenSearch(url);

        //}

        private void txtSearch_Leave(object sender, EventArgs e)
        {
           
        }

        private void glassButton3_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("RunLogLook", null));
        }

        private void labRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmRegister f = new frmRegister();
            f.RRResult = new frmRegister.ReturnRResult(this.GetrResult);
            f.ShowDialog();
            f.Dispose();
        }

        private void GetrResult(cGlobalParas.RegisterResult rResult)
        {
            m_rResult = rResult;
        }

        public void UpdateVersion()
        {
            if (Program.SominerVersion!=cGlobalParas.VersionType.Free)
            {
                this.labVersion.Text = rm.GetString("Info225");
                this.labRegister.Visible = false;

                if (Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
                {
                    this.labVer.Text = Program.SominerVersionEnter;

                }
                else if (Program.SominerVersion == cGlobalParas.VersionType.Ultimate)
                {
                    this.labVer.Text = Program.SominerVersionUltimate;

                }
                else if (Program.SominerVersion == cGlobalParas.VersionType.Program)
                {
                    this.labVer.Text = Program.SominerVersionProgram;
                }
                else if (Program.SominerVersion == cGlobalParas.VersionType.Cloud)
                {
                    this.labVer.Text = Program.SominerVersionCloud;
                }
                else
                {
                    this.labVer.Text = Program.SominerVersionFree;
                }

                this.labTaskVersion.Text = Program.SominerTaskVersionCode;
            }
        }

    }
}
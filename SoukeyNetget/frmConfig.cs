using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using NetMiner.Gather;
using NetMiner.Resource;
using NetMiner.Common;
using System.IO;

namespace MinerSpider
{
    public partial class frmConfig : Form
    {
        private ResourceManager rm;

        public frmConfig()
        {
            InitializeComponent();
        }

        private void treeMenu_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Name)
            {
                case "nodNormal":
                    this.panel1.Visible =true ;
                    this.panel2.Visible =false ;
                    this.panel3.Visible = false;
                    this.panel4.Visible = false;
                    this.panel5.Visible = false;
                    this.panel6.Visible = false;
                    this.panel7.Visible = false;
                    this.panel8.Visible = false;
                    break;

                case "nodRegex":
                    this.panel1.Visible = false;
                    this.panel2.Visible = false;
                    this.panel3.Visible = false;
                    this.panel4.Visible = true;
                    this.panel5.Visible = false;
                    this.panel6.Visible = false;
                    this.panel7.Visible = false;
                    this.panel8.Visible = false;
                    break;

                case "nodEmail":
                    this.panel1.Visible = false;
                    this.panel2.Visible = false;
                    this.panel3.Visible = false;
                    this.panel4.Visible = false;
                    this.panel5.Visible = true;
                    this.panel6.Visible = false;
                    this.panel7.Visible = false;
                    this.panel8.Visible = false;
                    break;
                case "nodMonitorData":
                    this.panel1.Visible = false;
                    this.panel2.Visible = false;
                    this.panel3.Visible = false;
                    this.panel4.Visible = false;
                    this.panel5.Visible = false;
                    this.panel6.Visible = true;
                    this.panel7.Visible = false;
                    this.panel8.Visible = false;
                    break;

                case "nodExit":
                    this.panel1.Visible =false ;
                    this.panel2.Visible =true ;
                    this.panel3.Visible = false;
                    this.panel4.Visible = false;
                    this.panel5.Visible = false;
                    this.panel6.Visible = false;
                    this.panel7.Visible = false;
                    this.panel8.Visible = false;
                    break;

                case "nodProxy":
                    this.panel1.Visible = false;
                    this.panel2.Visible = false;
                    this.panel3.Visible = true;
                    this.panel4.Visible = false;
                    this.panel5.Visible = false;
                    this.panel6.Visible = false;
                    this.panel7.Visible = false;
                    this.panel8.Visible = false;
                    break;

                case "nodLog":
                    this.panel1.Visible = false;
                    this.panel2.Visible = false;
                    this.panel3.Visible = false;
                    this.panel4.Visible = false;
                    this.panel5.Visible = false;
                    this.panel6.Visible = false;
                    this.panel7.Visible = true;
                    this.panel8.Visible = false;
                    break;

                case "nodRemote":
                    this.panel1.Visible = false;
                    this.panel2.Visible =false ;
                    this.panel3.Visible = false;
                    this.panel4.Visible = false;
                    this.panel5.Visible = false;
                    this.panel6.Visible = false;
                    this.panel7.Visible = false;
                    this.panel8.Visible = true;
                    break;
                default :
                    break;
            }
        }

        //保存配置信息
        private void SaveConfigData()
        {
            #region 保存配置信息
            try
            {
                cXmlSConfig Config = new cXmlSConfig(Program.getPrjPath());
                Config.IsInstantSave = false;

                if (this.raMin.Checked == true)
                    Config.ExitSelected = 0;
                else
                    Config.ExitSelected = 1;

                if (this.checkBox1.Checked == true)
                    Config.ExitIsShow = true;
                else
                    Config.ExitIsShow = false;

                if (this.IsAutoSystemLog.Checked == true)
                    Config.AutoSaveLog = true;
                else
                    Config.AutoSaveLog = false;

                if (this.isAutoUpdateProxy.Checked == true)
                    Config.IsAutoUpdateProxy = true;
                else
                    Config.IsAutoUpdateProxy = false;

                if (this.IsAutoSaveRadarLog.Checked == true)
                    Config.AutoSaveRadarLog = true;
                else
                    Config.AutoSaveRadarLog=false ;

                Config.LogMaxNumber = int.Parse ( this.LogMaxNumber.Value.ToString ());

                if (this.comUILanguage.SelectedIndex == 0)
                    Config.CurrentLanguage = cGlobalParas.CurLanguage.Auto;
                else if (this.comUILanguage.SelectedIndex == 1)
                    Config.CurrentLanguage = cGlobalParas.CurLanguage.zhCN;
                else if (this.comUILanguage.SelectedIndex == 2)
                    Config.CurrentLanguage = cGlobalParas.CurLanguage.zhCN;

                if (this.IsAutoStartRadar.Checked == true)
                    Config.IsAutoStartRadar = true;
                else
                    Config.IsAutoStartRadar = false;

                Config.MonitorInterval =int.Parse (this.numMonitorInterval.Value.ToString ());

                //存储新建任务的模式
                if (this.raNormal.Checked == true)
                    Config.NewTaskType = (int)cGlobalParas.NewTaskType.Normal;
                else if (this.raWizard.Checked == true)
                    Config.NewTaskType = (int)cGlobalParas.NewTaskType.Wizard;

                if (this.IsShowNewTask.Checked == true)
                    Config.NewTaskIsShow = true;
                else
                    Config.NewTaskIsShow = false;

                //存代理信息
                

                //存正则表达式的信息
                Config.RegexNextPage = this.txtRegexNextpage.Text;

                //存雷达数据库信息
                if (this.raMySql.Checked == true)
                    Config.DataType = (int)cGlobalParas.DatabaseType.MySql;
                else if (this.raMSSqlserver.Checked == true)
                    Config.DataType = (int)cGlobalParas.DatabaseType.MSSqlServer;

                //存网络矿工运行模式的信息
                if (this.raNormalRunning.Checked == true)
                    Config.SoMinerRunningModel = (int)cGlobalParas.SoMinerRunningModel.Normail;
                else if (this.raSilentRunning.Checked == true)
                    Config.SoMinerRunningModel = (int)cGlobalParas.SoMinerRunningModel.Silent;

                Config.DataConnection = this.txtDbCon.Text;

                //存电子邮件信息
                Config.Email = this.txtEmail.Text;
                Config.EmailPopServer = this.txtPop.Text;
                if (this.txtPopPort.Text.Trim() == "")
                    Config.EmailPopPort = 25;
                else 
                    Config.EmailPopPort = int.Parse(this.txtPopPort.Text);

                Config.IsPopVerfy = this.IsPop.Checked;
                Config.EmailUser = this.txtEmailUser.Text;
                Config.EmailPwd = this.txtEmailPwd.Text;

                Config.IsAllowRemoteGather = this.IsAllowRemoteGather.Checked;
                Config.RemoteServer = this.txtRemoteServer.Text;
                Config.RemoteServerUser = this.txtRemoteUser.Text;
                Config.RemoteServerLicence = this.txtRemoteLicence.Text;
                Config.MaxRemoteTaskCount = (int)this.MaxRemoteCount.Value;
                Config.IsAuthen = this.isAuthen.Checked;
                Config.windowsUser = this.windowUser.Text;
                Config.windowsPwd = this.windowsPWD.Text;

                Config.Save();
                Config = null;

                this.IsSave.Text = "false";
            }
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString("Info76"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            #endregion

           
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            this.comUILanguage.Items.Add(rm.GetString ("Label24"));
            //this.comUILanguage.Items.Add(rm.GetString("Label25"));
            this.comUILanguage.Items.Add(rm.GetString("Label26"));

            this.txtLogPath.Text = Program.getPrjPath() + "log";
            this.txtRadarLogPath.Text = Program.getPrjPath() + "log";

            this.txtSilentSaveLogPath.Text = Program.getPrjPath() + "log";

            try
            {
                #region 加载配置信息

                cXmlSConfig Config = new cXmlSConfig(Program.getPrjPath());

                if (Config.ExitSelected == 0)
                    this.raMin.Checked = true;
                else
                    this.raExit.Checked = true;

                if (Config.ExitIsShow == true)
                    this.checkBox1.Checked = true;
                else
                    this.checkBox1.Checked = false;

                this.IsAutoSystemLog.Checked = Config.AutoSaveLog;
                this.IsAutoSaveRadarLog.Checked = Config.AutoSaveRadarLog;
                this.LogMaxNumber.Value = Config.LogMaxNumber;

                this.isAutoUpdateProxy.Checked = Config.IsAutoUpdateProxy;

                this.IsAutoStartRadar.Checked = Config.IsAutoStartRadar;
                
                this.numMonitorInterval.Value = Config.MonitorInterval;

                switch (Config.CurrentLanguage)
                {
                    case cGlobalParas.CurLanguage .Auto :
                        this.comUILanguage.SelectedIndex = 0;
                        break;
                    case cGlobalParas.CurLanguage.enUS:
                        this.comUILanguage.SelectedIndex = 1;
                        break;
                    case cGlobalParas.CurLanguage .zhCN :
                        this.comUILanguage.SelectedIndex =1;
                        break;
                    default :
                        break ;
                }

                //添加选择新建采集任务的方式
                this.IsShowNewTask.Checked = Config.NewTaskIsShow;
                if (Config.NewTaskType == (int)cGlobalParas.NewTaskType.Normal)
                {
                    this.raNormal.Checked = true;
                    this.raWizard.Checked = false;
                }
                else if (Config.NewTaskType == (int)cGlobalParas.NewTaskType.Wizard)
                {
                    this.raWizard.Checked = true;
                    this.raNormal.Checked = false;
                }

                //添加代理信息
               

                //添加正则表达式的信息
                this.txtRegexNextpage.Text = ToolUtil.ShowTrans( Config.RegexNextPage);

                //添加运行模式的信息
                if ((cGlobalParas.SoMinerRunningModel)Config.SoMinerRunningModel == cGlobalParas.SoMinerRunningModel.Normail)
                    this.raNormalRunning.Checked = true;
                else if ((cGlobalParas.SoMinerRunningModel)Config.SoMinerRunningModel == cGlobalParas.SoMinerRunningModel.Silent)
                    this.raSilentRunning.Checked = true;


                //数据库信息
                if ((cGlobalParas.DatabaseType)Config.DataType == cGlobalParas.DatabaseType.MySql)
                    this.raMySql.Checked = true;
                else if ((cGlobalParas.DatabaseType)Config.DataType == cGlobalParas.DatabaseType.MSSqlServer)
                    this.raMSSqlserver.Checked = true;

                //电子邮件信息
                this.txtEmail.Text = Config.Email;
                this.txtEmailUser.Text = Config.EmailUser;
                this.txtEmailPwd.Text = Config.EmailPwd;
                this.txtPop.Text = Config.EmailPopServer;
                this.IsPop.Checked = Config.IsPopVerfy;
                this.txtPopPort.Text = Config.EmailPopPort.ToString();

                this.txtDbCon.Text = Config.DataConnection;

                if (Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
                {
                    TreeNode tNode=new TreeNode ();
                    tNode.Name ="nodRemote";
                    tNode.Text ="远程服务器配置";
                    tNode.ImageIndex=8;
                    tNode.SelectedImageIndex=8;
                    this.treeMenu.Nodes.Insert(6,tNode );
                    this.panel8.Visible = true ;

                    this.IsAllowRemoteGather.Checked = Config.IsAllowRemoteGather;
                    this.txtRemoteServer.Text = Config.RemoteServer;
                    this.txtRemoteUser.Text = Config.RemoteServerUser;
                    this.txtRemoteLicence.Text = Config.RemoteServerLicence;
                    this.MaxRemoteCount.Value = Config.MaxRemoteTaskCount;

                    this.isAuthen.Checked = Config.IsAuthen;
                    this.windowUser.Text = Config.windowsUser;
                    this.windowsPWD.Text = Config.windowsPwd;

                }
                else
                {
                    this.panel8.Visible =false ;
                }
                
                Config = null;

                #endregion 

               

                this.cmdApply.Enabled = false;
                this.IsSave.Text = "false";

            }
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString("Info76"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void IsSave_TextChanged(object sender, EventArgs e)
        {
            if (this.IsSave.Text == "true" )
            {
                this.cmdApply.Enabled = true;
            }
            else if (this.IsSave.Text == "false")
            {
                this.cmdApply.Enabled = false;
            }
        }

        private void IsAutoSystemLog_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void raMin_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void raExit_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            SaveConfigData();
            this.cmdApply.Enabled = false;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (this.cmdApply.Enabled == true)
                SaveConfigData();

            this.Close();
        }

        private void frmConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void comUILanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtLogPath_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtProxyServer_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtProxyPort_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtProxyUser_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtProxyPwd_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtRegexNextpage_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsAutoStartRadar_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsAutoStartRadar.Checked == true)
            {
                //判断当前版本
                if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                    Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                    Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
                {

                }
                else
                {
                    this.IsAutoStartRadar.Checked = false;
                    MessageBox.Show("只有旗舰版以上方可支持此功能，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

             this.IsSave.Text = "true";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmSetData fSD = new frmSetData();

            if (this.raMySql.Checked == true)
                fSD.FormState = 2;
            else if (this.raMSSqlserver.Checked == true)
                fSD.FormState = 1;

            fSD.rDataSource = new frmSetData.ReturnDataSource(GetDataSource);
            fSD.ShowDialog();
            fSD.Dispose();
        }

        private void GetDataSource(string strDataConn)
        {
            this.txtDbCon.Text = strDataConn;
        }

        private void raMSSqlserver_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }
        
        private void txtDbCon_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtPop_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsPop_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtPopPort_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtEmailUser_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtEmailPwd_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsShowNewTask_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void raWizard_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void raNormal_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cmdAddWeblink_Click(object sender, EventArgs e)
        {
            frmAddProxy f = new frmAddProxy();
            f.rProxy = this.AddProxy;
            f.ShowDialog();
            f.Dispose();
        }

        private void AddProxy(ListViewItem cItem)
        {
            if (cItem != null)
            {
                this.listProxy.Items.Add(cItem);
                this.IsSave.Text = "true";
            }
        }

        private void cmdDelWeblink_Click(object sender, EventArgs e)
        {
            DelProxy();
        }

        private void DelProxy()
        {
            if (this.listProxy.SelectedItems.Count > 0)
            {
                this.listProxy.Items.Remove(this.listProxy.SelectedItems[0]);
                this.IsSave.Text = "true";
            }
        }

        private void listProxy_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DelProxy();
            }
        }

        private void raNormalRunning_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void raSilentRunning_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void LogMaxNumber_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsAutoSaveRadarLog_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void numMonitorInterval_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show ("验证代理IP过程时间较长，请耐心等待......")

            this.button2.Enabled = false;

            for (int i = 0; i < this.listProxy.Items.Count; i++)
            {
                this.listProxy.Items[i].SubItems[4].Text = "正在验证，请等待...";
                Application.DoEvents();

                bool isSucceed = false;

                try
                {
                    isSucceed = vProxy(this.listProxy.Items[i].Text.ToString(), this.listProxy.Items[i].SubItems[1].Text.ToString(),
                        this.listProxy.Items[i].SubItems[2].Text.ToString(), this.listProxy.Items[i].SubItems[3].Text.ToString());
                }
                catch (System.Exception ex)
                {
                    this.listProxy.Items[i].SubItems[4].Text = ex.Message;
                }

                Application.DoEvents();

                if (isSucceed == true)
                {
                    this.listProxy.Items[i].SubItems[4].Text = "验证通过";
                }
                else
                    this.listProxy.Items[i].SubItems[4].Text = "验证失败";

            }

            this.button2.Enabled = true;

        }

        private delegate bool delegateVerifyProxy(string IP, string port, string User, string pwd);

        private bool vProxy(string IP, string port, string User, string pwd)
        {
            //定义一个修改分类名称的委托
            delegateVerifyProxy sd = new delegateVerifyProxy(this.VerifyProxy);

            //开始调用函数,可以带参数 
            IAsyncResult ir = sd.BeginInvoke( IP,port,User,pwd, null, null);

            Application.DoEvents();

            //循环检测是否完成了异步的操作 
            while (true)
            {
                if (ir.IsCompleted)
                {
                    //完成了操作则关闭窗口 
                    break;
                }
            }

            //取函数的返回值 
            bool retValue = sd.EndInvoke(ir);

            return retValue;
        }

        //验证代理IP
        private bool VerifyProxy( string IP, string port, string User, string pwd)
        {
            //判断网页编码
            Encoding wCode = Encoding.Default;

            HttpWebRequest wReq;

            WebProxy w = new WebProxy(IP , int.Parse(port));
            w.BypassProxyOnLocal = true;
            if (User != "")
            {
                w.Credentials = new NetworkCredential(User, pwd);
            }

            Uri m_Uri = new Uri("http://www.netminer.cn");

            string url = m_Uri.AbsolutePath;

            try
            {

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
            catch (System.Exception)
            {
                return false;
            }

        }

        private void txtRemoteServer_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void IsAllowRemoteGather_CheckedChanged(object sender, EventArgs e)
        {
            if (IsAllowRemoteGather.Checked == true)
                this.MaxRemoteCount.Enabled = true;
            else
                this.MaxRemoteCount.Enabled = false;
            this.IsSave.Text = "true";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string fName = string.Empty;

            this.openFileDialog1.Title = "选择IP文件[每行一个代理IP]";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "All Files(*.txt)|*.txt";

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fName = this.openFileDialog1.FileName;

                StreamReader fileReader = null;

                try
                {
                    fileReader = new StreamReader(fName, System.Text.Encoding.GetEncoding("gb2312"));
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("文件打开失败，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return ;
                }
                string str = fileReader.ReadToEnd();
                fileReader.Close();
                fileReader = null;

                try
                {
                    string[] Urls = Regex.Split(str, "\r\n");

                    ListViewItem[] items = new ListViewItem[Urls.Length];
                    int i = 0;

                    for (i = 0; i < Urls.Length; i++)
                    {
                        string ss = Urls[i].Replace("\r", "").Replace("\n", "").Trim();
                        if (ss != "")
                        {
                            items[i] = new ListViewItem();
                            items[i].Text = Urls[i].Substring(0, Urls[i].IndexOf(":"));
                            items[i].SubItems.Add(Urls[i].Substring(Urls[i].IndexOf(":") + 1, Urls[i].Length - Urls[i].IndexOf(":") - 1));
                            items[i].SubItems.Add("");
                            items[i].SubItems.Add("");
                            items[i].SubItems.Add("");
                        }
                    }

                    this.listProxy.Items.AddRange(items);
                    Urls = null;
                }
                catch
                {
                    MessageBox.Show("文档格式不合法，每行一个IP地址+端口号，不能有空行，尤其是结尾处！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }

                this.IsSave.Text = "true";
            }
        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void txtRemoteUser_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtRemoteLicence_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void MaxRemoteCount_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void isAutoUpdateProxy_CheckedChanged(object sender, EventArgs e)
        {
            if (this.isAutoUpdateProxy.Checked == true)
            {
                //判断当前版本
                if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                    Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                    Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
                {

                }
                else
                {
                    this.isAutoUpdateProxy.Checked = false;
                    MessageBox.Show("只有旗舰版以上方可支持此功能，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            this.IsSave.Text = "true";
        }

        private void windowUser_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void windowsPWD_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void isAuthen_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
            if (this.isAuthen.Checked == true)
            {
                this.windowUser.Enabled = true;
                this.windowsPWD.Enabled = true;
            }
            else
            {
                this.windowUser.Enabled = false ;
                this.windowsPWD.Enabled = false ;
            }
        }

        private void raMySql_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using System.Text.RegularExpressions;
using NetMiner.Gather;
using NetMiner.Resource;
using System.Threading;
using System.Net;
using NetMiner.Common.Tool;

namespace MinerSpider
{
    public partial class frmRegister : Form
    {
        //定义一个代理，返回注册结果
        public delegate void ReturnRResult(cGlobalParas.RegisterResult rResult);
        public ReturnRResult RRResult;

        //定义一个访问资源文件的变量
        private ResourceManager rm;

        private cGlobalParas.RegisterResult m_retValue;

        public frmRegister()
        {
            InitializeComponent();
        }

        private delegate cGlobalParas.RegisterResult delegateRegister(string User, string LicenceID);
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.txtUser.Text.Trim() == "" || this.txtKeys.Text.Trim() == "")
            {
                MessageBox.Show(rm.GetString("Error29"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!Regex.IsMatch(this.txtKeys.Text, @"\w\w\w\w-\w\w\w\w-\w\w\w\w"))
            {
                MessageBox.Show(rm.GetString("Error30"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Again:

            //定义一个修改分类名称的委托
            delegateRegister sd = new delegateRegister(this.Register);

            //开始调用函数,可以带参数 
            IAsyncResult ir = sd.BeginInvoke(this.txtUser.Text, this.txtKeys.Text, null, null);

            //显示等待的窗口 
            frmWaiting fWait = new frmWaiting("正在激活软件请等待...");
            new Thread((ThreadStart)delegate
            {
                Application.Run(fWait);
            }).Start();

            try
            {
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
                m_retValue = sd.EndInvoke(ir);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fWait.IsHandleCreated)
                    fWait.Invoke((EventHandler)delegate { fWait.Close(); });
                fWait = null;
            }

            if (m_retValue == cGlobalParas.RegisterResult.Succeed)
            {
                MessageBox.Show(rm.GetString("Info228"),rm.GetString ("MessageboxInfo"),MessageBoxButtons.OK ,MessageBoxIcon.Information );
                this.Close();
            }
            else if (m_retValue == cGlobalParas.RegisterResult.Error)
            {
                MessageBox.Show(rm.GetString ("Error27"),rm.GetString ("MessageboxError"),MessageBoxButtons.OK,MessageBoxIcon.Error );
                return;
            }
            else if (m_retValue == cGlobalParas.RegisterResult.Failed)
            {
                MessageBox.Show(rm.GetString("Error28"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (m_retValue == cGlobalParas.RegisterResult.NoMatch)
            {
                MessageBox.Show("当前正在使用的版本与注册码激活的版本不匹配，请确认当前使用的版本与注册码激活版本一致！","网络矿工 信息",MessageBoxButtons.OK ,MessageBoxIcon.Error );
                return;
            }
            else if (m_retValue == cGlobalParas.RegisterResult.Upgrade)
            {
                if (MessageBox.Show("注册码激活的版本低于当前使用的版本，您可自动升级注册码，以重新激活，是否自动升级注册码？", "网络矿工 询问",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //开始远程升级注册号
                    string newVersion = GetHtmlSource("http://www.netminer.cn/hander/autoupdate.ashx",
                        "mode=upgradeversion&user=" + this.txtUser.Text
                        + "&code=" + this.txtKeys.Text + "&version=" + Program.SominerVersionCode);

                    if (newVersion.ToLower() == "true")
                        goto Again;
                    else
                    {
                        MessageBox.Show("注册码升级失败，可能超过免费升级服务周期，请与网络矿工支持联系，谢谢！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                    return ;
            }

        }

        private string GetHtmlSource(string url, string postString)
        {
            if (url == "")
                return "";

            string charSet = "";
            WebClient myWebClient = new WebClient();
            myWebClient.Proxy = null;

            //获取或设置用于对向 Internet 资源的请求进行身份验证的网络凭据。   
            myWebClient.Credentials = CredentialCache.DefaultCredentials;


            byte[] myDataBuffer;
            string strWebData;

            try
            {
                //从资源下载数据并返回字节数组。（加@是因为网址中间有"/"符号）   

                if (!string.IsNullOrEmpty(postString))
                {
                    byte[] postData = Encoding.UTF8.GetBytes(postString);
                    myWebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    myDataBuffer = myWebClient.UploadData(@url, "POST", postData);
                }
                else
                    myDataBuffer = myWebClient.DownloadData(@url);

                strWebData = Encoding.UTF8.GetString(myDataBuffer);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //获取此页面的编码格式
            Match charSetMatch = Regex.Match(strWebData, "<meta([^<]*)charset=([^<]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string webCharSet = charSetMatch.Groups[2].Value;
            if (charSet == null || charSet == "")
                charSet = webCharSet;

            if (charSet != null && charSet != "" && Encoding.GetEncoding(charSet) != Encoding.Default)
                strWebData = Encoding.GetEncoding(charSet).GetString(myDataBuffer);

            return strWebData;

        }
        
        private cGlobalParas.RegisterResult Register(string User,string LicenceID)
        {
            try
            {
                cVersion v = new cVersion(Program.getPrjPath());

                //首先先验证输入的许可号与用户名是否匹配
                if (!v.VerifyLicence(User, LicenceID))
                {
                    v = null;
                    return cGlobalParas.RegisterResult.Failed;
                }

                //本地验证通过后，需到服务器上去验证是否合法，也就是这个序列号是否发了，不发CPU表示是第一次注册
                cGlobalParas.RegisterResult rResult = (cGlobalParas.RegisterResult)v.VerfyOnline(User, LicenceID, Program.SominerVersionCode);
                v = null;

                return rResult;
                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return cGlobalParas.RegisterResult.Error;
                
            }

        }

        private void frmRegister_Load(object sender, EventArgs e)
        {
            //初始化资源文件的参数
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            //根据版本设置窗体显示
           if (Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                this.Text = "网络矿工数据采集软件" + Program.SominerVersionEnter + " 在线激活";

            }
            else if (Program.SominerVersion == cGlobalParas.VersionType.Ultimate )
            {
                this.Text = "网络矿工数据采集软件" + Program.SominerVersionUltimate + " 在线激活";

            }
            else if (Program.SominerVersion == cGlobalParas.VersionType.Program)
            {
                this.Text = "网络矿工数据采集软件" + Program.SominerVersionProgram + " 在线激活";
            }
           else if (Program.SominerVersion==cGlobalParas.VersionType.Cloud)
           {
                this.Text = "网络矿工数据采集软件" + Program.SominerVersionCloud + " 在线激活";
           }
           else
           {
               this.Text = "网络矿工数据采集软件" + Program.SominerVersionFree + " 在线激活";
           }

        }

        private void frmRegister_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
            RRResult(m_retValue );
        }
    }
}
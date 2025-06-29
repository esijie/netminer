using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using NetMiner.Gather;
using NetMiner.Resource;
using System.Threading;
using NetMiner.Common.Tool;

///功能：关于窗体，此内容禁止修改，如果您再发布了您的产品，此部也要予以保留，进行相关版权说明
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace MinerSpider
{

    public partial class frmAbout : Form
    {

        //定义一个访问资源文件的变量
        private ResourceManager rm;

        private bool m_IsShow;

        public frmAbout()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.netminer.cn"); 
        }

        private delegate cVersion.StructRegisterInfo delegateCheckVersion();
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.m_IsShow ==false )
            {
                this.panel2.Height = 234;
                this.groupBox1.Top = 252;
                this.button1.Top = 263;
                this.button2.Top = 263;
                this.Height = 323;
                this.button2.Text = rm.GetString("button2");

                frmWaiting fWait = new frmWaiting("正在检测授权状态...");

                //定义一个修改分类名称的委托
                delegateCheckVersion sd = new delegateCheckVersion(this.GetRegInfo);
                //开始调用函数,可以带参数 
                IAsyncResult ir = sd.BeginInvoke(null, null);

                try
                {
                    //显示等待的窗口 
                    new Thread((ThreadStart)delegate
                    {
                        Application.Run(fWait);
                    }).Start();
                    //fWait.Show(this);

                    //刷新这个等待的窗口 
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
                }
                catch { }
                finally
                {
                    if (fWait.IsHandleCreated)
                        fWait.Invoke((EventHandler)delegate { fWait.Close(); });
                    fWait = null;
                }

                //取函数的返回值 
                cVersion.StructRegisterInfo retValue = sd.EndInvoke(ir);

              if ((cGlobalParas.VersionType)retValue.SominerVersion == cGlobalParas.VersionType.Cloud)
                {
                    this.LicenseInfo.Text = Program.SominerVersionCloud + " " + rm.GetString("Info225");
                    this.User.Text = retValue.User;
                    this.LicenseID.Text = retValue.Keys;
                }
                else if ((cGlobalParas.VersionType)retValue.SominerVersion == cGlobalParas.VersionType.Enterprise)
                {
                    this.LicenseInfo.Text = Program.SominerVersionEnter + " " + rm.GetString("Info225");
                    this.User.Text = retValue.User;
                    this.LicenseID.Text = retValue.Keys;
                }
                  else if ((cGlobalParas.VersionType)retValue.SominerVersion == cGlobalParas.VersionType.Program)
                  {
                      this.LicenseInfo.Text = Program.SominerVersionProgram + " " + rm.GetString("Info225");
                      this.User.Text = retValue.User;
                      this.LicenseID.Text = retValue.Keys;
                  }
                else if ((cGlobalParas.VersionType)retValue.SominerVersion == cGlobalParas.VersionType.Ultimate)
                {
                    this.LicenseInfo.Text = Program.SominerVersionUltimate + " " + rm.GetString("Info225");
                    this.User.Text = retValue.User;
                    this.LicenseID.Text = retValue.Keys;
                }
                else if ((cGlobalParas.VersionType)retValue.SominerVersion == cGlobalParas.VersionType.Free)
                {
                    this.LicenseInfo.Text = "免费授权";
                    this.User.Text = "";
                    this.LicenseID.Text = "";
                }
             

               
                this.m_IsShow = true;
                
            }
            else if (this.m_IsShow ==true )
            {
                this.panel2.Height = 150;
                this.groupBox1.Top = 168;
                this.button1.Top = 180;
                this.button2.Top = 180;
                this.Height = 241;
                this.button2.Text = rm.GetString("button1");

                this.m_IsShow = false;
            }
        }

        private cVersion.StructRegisterInfo GetRegInfo()
        {
            cVersion.StructRegisterInfo rInfo = new cVersion.StructRegisterInfo();

            if (Program.g_isLogin)
            {
                rInfo.User = Program.RegisterUser;
                rInfo.SominerVersion = Program.SominerVersion;
                rInfo.Keys = Program.g_EndServiceDate.ToString("yyyy-MM-dd HH:mm");
            }
            else
            {
                cVersion sVer;

                try
                {
                    sVer = new cVersion(Program.getPrjPath());

                    sVer.ReadRegisterInfo();

                    rInfo = sVer.RegisterInfo;

                }
                catch (System.Exception)
                {
                    rInfo.SominerVersion = cGlobalParas.VersionType.Free;
                    return rInfo;
                }
                rInfo = sVer.RegisterInfo;

                sVer = null;
            }

            return rInfo;
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            //初始化资源文件的参数
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            m_IsShow = false;

            string lVersion = Program.SominerVersionCode;
            lVersion = lVersion.Substring(0, lVersion.LastIndexOf("."));
            this.labVersion.Text = lVersion;
            this.SVersion.Text = lVersion;

            //if (Program.GetCurrentVersion () == cGlobalParas.VersionType.Personal)
            //{
            //    this.labVersion.Text = Program.SominerVersionPersonal;
            //}

            //else if (Program.GetCurrentVersion() == cGlobalParas.VersionType.Enterprise)
            //{
            //    this.labVersion.Text = Program.SominerVersionEnter;

            //}
            //else if (Program.GetCurrentVersion() == cGlobalParas.VersionType.Ultimate)
            //{
            //    this.labVersion.Text = Program.SominerVersionUltimate;

            //}
            //else if (Program.GetCurrentVersion() == cGlobalParas.VersionType.Professional)
            //{
            //    this.labVersion.Text = Program.SominerVersionProfessional;
            //}
            //else
            //{
            //    this.labVersion.Text = Program.SominerVersionFree;
            //}
        }

        private void frmAbout_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.netminer.cn/product/index.aspx?aid=1006"); 
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.apache.org/licenses/LICENSE-2.0.html"); 
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.opensource.org/licenses/mit-license.php"); 
        }
      
       
    }
}
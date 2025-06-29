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

///���ܣ����ڴ��壬�����ݽ�ֹ�޸ģ�������ٷ��������Ĳ�Ʒ���˲�ҲҪ���Ա�����������ذ�Ȩ˵��
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace MinerSpider
{

    public partial class frmAbout : Form
    {

        //����һ��������Դ�ļ��ı���
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

                frmWaiting fWait = new frmWaiting("���ڼ����Ȩ״̬...");

                //����һ���޸ķ������Ƶ�ί��
                delegateCheckVersion sd = new delegateCheckVersion(this.GetRegInfo);
                //��ʼ���ú���,���Դ����� 
                IAsyncResult ir = sd.BeginInvoke(null, null);

                try
                {
                    //��ʾ�ȴ��Ĵ��� 
                    new Thread((ThreadStart)delegate
                    {
                        Application.Run(fWait);
                    }).Start();
                    //fWait.Show(this);

                    //ˢ������ȴ��Ĵ��� 
                    Application.DoEvents();

                    //ѭ������Ƿ�������첽�Ĳ��� 
                    while (true)
                    {
                        if (ir.IsCompleted)
                        {
                            //����˲�����رմ��� 

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

                //ȡ�����ķ���ֵ 
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
                    this.LicenseInfo.Text = "�����Ȩ";
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
            //��ʼ����Դ�ļ��Ĳ���
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
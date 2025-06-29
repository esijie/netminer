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
        //����һ����������ע����
        public delegate void ReturnRResult(cGlobalParas.RegisterResult rResult);
        public ReturnRResult RRResult;

        //����һ��������Դ�ļ��ı���
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

            //����һ���޸ķ������Ƶ�ί��
            delegateRegister sd = new delegateRegister(this.Register);

            //��ʼ���ú���,���Դ����� 
            IAsyncResult ir = sd.BeginInvoke(this.txtUser.Text, this.txtKeys.Text, null, null);

            //��ʾ�ȴ��Ĵ��� 
            frmWaiting fWait = new frmWaiting("���ڼ��������ȴ�...");
            new Thread((ThreadStart)delegate
            {
                Application.Run(fWait);
            }).Start();

            try
            {
                //ѭ������Ƿ�������첽�Ĳ��� 
                while (true)
                {
                    if (ir.IsCompleted)
                    {
                        //����˲�����رմ���
                        break;
                    }
                }

                //ȡ�����ķ���ֵ 
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
                MessageBox.Show("��ǰ����ʹ�õİ汾��ע���뼤��İ汾��ƥ�䣬��ȷ�ϵ�ǰʹ�õİ汾��ע���뼤��汾һ�£�","����� ��Ϣ",MessageBoxButtons.OK ,MessageBoxIcon.Error );
                return;
            }
            else if (m_retValue == cGlobalParas.RegisterResult.Upgrade)
            {
                if (MessageBox.Show("ע���뼤��İ汾���ڵ�ǰʹ�õİ汾�������Զ�����ע���룬�����¼���Ƿ��Զ�����ע���룿", "����� ѯ��",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //��ʼԶ������ע���
                    string newVersion = GetHtmlSource("http://www.netminer.cn/hander/autoupdate.ashx",
                        "mode=upgradeversion&user=" + this.txtUser.Text
                        + "&code=" + this.txtKeys.Text + "&version=" + Program.SominerVersionCode);

                    if (newVersion.ToLower() == "true")
                        goto Again;
                    else
                    {
                        MessageBox.Show("ע��������ʧ�ܣ����ܳ�����������������ڣ����������֧����ϵ��лл��", "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            //��ȡ���������ڶ��� Internet ��Դ��������������֤������ƾ�ݡ�   
            myWebClient.Credentials = CredentialCache.DefaultCredentials;


            byte[] myDataBuffer;
            string strWebData;

            try
            {
                //����Դ�������ݲ������ֽ����顣����@����Ϊ��ַ�м���"/"���ţ�   

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

            //��ȡ��ҳ��ı����ʽ
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

                //��������֤�������ɺ����û����Ƿ�ƥ��
                if (!v.VerifyLicence(User, LicenceID))
                {
                    v = null;
                    return cGlobalParas.RegisterResult.Failed;
                }

                //������֤ͨ�����赽��������ȥ��֤�Ƿ�Ϸ���Ҳ����������к��Ƿ��ˣ�����CPU��ʾ�ǵ�һ��ע��
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
            //��ʼ����Դ�ļ��Ĳ���
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            //���ݰ汾���ô�����ʾ
           if (Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                this.Text = "��������ݲɼ����" + Program.SominerVersionEnter + " ���߼���";

            }
            else if (Program.SominerVersion == cGlobalParas.VersionType.Ultimate )
            {
                this.Text = "��������ݲɼ����" + Program.SominerVersionUltimate + " ���߼���";

            }
            else if (Program.SominerVersion == cGlobalParas.VersionType.Program)
            {
                this.Text = "��������ݲɼ����" + Program.SominerVersionProgram + " ���߼���";
            }
           else if (Program.SominerVersion==cGlobalParas.VersionType.Cloud)
           {
                this.Text = "��������ݲɼ����" + Program.SominerVersionCloud + " ���߼���";
           }
           else
           {
               this.Text = "��������ݲɼ����" + Program.SominerVersionFree + " ���߼���";
           }

        }

        private void frmRegister_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
            RRResult(m_retValue );
        }
    }
}
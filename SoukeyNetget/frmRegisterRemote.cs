using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Resources;
using NetMiner.Resource;

namespace MinerSpider
{
    public partial class frmRegisterRemote : Form
    {
        //定义一个访问资源文件的变量
        private ResourceManager rm;

        public frmRegisterRemote()
        {
            InitializeComponent();
        }

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

            this.button1.Enabled = false;
            this.label3.Visible = true;
            Application.DoEvents();

            //localhost.NetMinerWebService sweb = new localhost.NetMinerWebService();
            //sweb.Url = Program.ConnectServer + "/NetMiner.WebService.asmx";

            //if (Program.g_IsAuthen == true)
            //    sweb.Credentials = new System.Net.NetworkCredential(Program.g_WindowsUser, Program.g_WindowsPwd);

            int result;
            //try
            //{
            //    //注册不在提供版本，由服务器提供的注册号来获取版本
            //    result = sweb.Register(this.txtUser.Text, this.txtKeys.Text);
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    this.label3.Visible = false;
            //    this.button1.Enabled = true;
            //    return;
            //}

            //switch ((cGlobalParas.RegisterResult)result)
            //{
            //    case cGlobalParas.RegisterResult.Succeed :
            //        MessageBox.Show("远程服务器激活成功！","网络矿工 信息",MessageBoxButtons.OK ,MessageBoxIcon.Information );
            //        this.Close();
            //        break;
            //    case  cGlobalParas.RegisterResult.Error :
            //        MessageBox.Show("远程服务器激活发生错误，请稍候重试！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //        break;
            //    case cGlobalParas.RegisterResult.Failed :
            //        MessageBox.Show("远程服务器激活失败，请确认您的激活码是否正确！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error );

            //        break;
            //    case cGlobalParas.RegisterResult.Registed:
            //         MessageBox.Show("远程服务器已经激活！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
            //         this.Close();
            //        break;
            //}

            this.label3.Visible = false;
            this.button1.Enabled = true;
        }

        private void frmRegisterRemote_Load(object sender, EventArgs e)
        {
            //初始化资源文件的参数
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

        }
    }
}

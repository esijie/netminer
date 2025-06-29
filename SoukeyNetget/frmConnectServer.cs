using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetMiner.Resource;

namespace MinerSpider
{
    public partial class frmConnectServer : Form
    {
        public delegate void ReturnServer(string server,bool isAuthen,string user,string pwd);

        public ReturnServer RServer;

        public frmConnectServer()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim() == "")
            {
                MessageBox.Show("请输入远程服务器的地址！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.textBox1.Focus();
                return;
            }

            string rUrl = this.textBox1.Text;
            if (!rUrl.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show("请输入合法的远程服务器地址，且必须包含http://", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (rUrl.EndsWith("/"))
            {
                rUrl = rUrl.Substring(0, rUrl.Length - 1);
            }

            RServer(rUrl.Trim(), this.isAuthen.Checked, this.windowUser.Text, this.windowsPWD.Text);
            this.Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //localhost.NetMinerWebService sWebServer = new localhost.NetMinerWebService();

            //string rUrl = this.textBox1.Text;
            //if (!rUrl.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    MessageBox.Show("请输入合法的远程服务器地址，且必须包含http://","网络矿工 错误",MessageBoxButtons.OK ,MessageBoxIcon.Error );
            //    return;
            //}

            //if (rUrl.EndsWith("/"))
            //{
            //    rUrl = rUrl.Substring(0, rUrl.Length - 1);
            //}
            //sWebServer.Url = rUrl + "/NetMinerWebService.asmx";

            //if (this.isAuthen.Checked == true)
            //    sWebServer.Credentials = new System.Net.NetworkCredential(this.windowUser.Text, this.windowsPWD.Text);
            
           

            //try
            //{
            //    int cResult = sWebServer.ConnectServer(Program.RegisterUser);

            //    switch ((cGlobalParas.RegResult)cResult)
            //    {
            //        case cGlobalParas.RegResult.Succeed:
            //            MessageBox.Show("远程服务器连接成功，可以实现远程采集管理！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //            break;
            //        case cGlobalParas.RegResult.UnReg:
            //            MessageBox.Show("远程服务器连接成功，但远程服务器没有激活，无法进行数据采集工作，请激活远程服务器！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //            break;
            //        default:
            //            MessageBox.Show("远程服务器连接失败!", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            break;
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

        }

        private void frmConnectServer_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = Program.ConnectServer;
        }

        private void isAuthen_CheckedChanged(object sender, EventArgs e)
        {
            if (this.isAuthen.Checked == true)
            {
                this.windowUser.Enabled = true;
                this.windowsPWD.Enabled = true;
            }
            else
            {
                this.windowUser.Enabled = false;
                this.windowsPWD.Enabled = false ;
            }
        }
    }
}

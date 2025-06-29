using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using NetMiner.Resource;

namespace MinerSpider
{
    
    public partial class frmLogin : Form
    {
        public delegate void returnUser(string User);
        public returnUser rUser;

        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Leave(object sender, EventArgs e)
        {
            
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.netminer.cn/register.aspx");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //提交验证
            string user = this.txtUser.Text.Trim();
            if (string.IsNullOrEmpty(user))
            {
                MessageBox.Show("用户名不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtUser.Focus();
                return;
            }

            string pwd = this.txtPwd.Text;
            if (string.IsNullOrEmpty(pwd))
            {
                MessageBox.Show("密码不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtPwd.Focus();
                return;
            }

            string result = NetMiner.Common.ToolUtil .GetHtmlSource("http://www.netminer.cn/hander/login.ashx", "u=" + user + "&p=" + pwd);
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("无此账户，请先注册！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                rUser("");
                return;
            }

            result = NetMiner.Common.ToolUtil.AesDecrypt(result, Program.g_AESKey);
            DataTable d= JsonConvert.DeserializeObject<DataTable>(result);

            string loginResult = d.Rows[0]["Result"].ToString();

            if (loginResult.ToLower() == "false")
            {
                MessageBox.Show("用户名或密码错误！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DateTime eDateTime = DateTime.Parse ( d.Rows[0]["EndDateTime"].ToString());

            Program.g_EndServiceDate =eDateTime;
            if (Program.g_EndServiceDate < System.DateTime.Now)
            {
                rUser(this.txtUser.Text);
                if (MessageBox.Show("产品授权时限已过期，请续费后重新登录，是否进行续费？", "网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    return;
                }
                else
                {
                    this.Close();
                }
            }

            try
            {
                //获取注册的版本
                Program.SominerVersion = (NetMiner.Resource.cGlobalParas.VersionType)int.Parse(d.Rows[0]["Version"].ToString());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("版本获取失败，请确认您的账户处于有效期！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            rUser(this.txtUser.Text);

            if (this.checkBox1.Checked==true)
            {
                //写入注册文件
                if (Program.SominerVersion == cGlobalParas.VersionType.Cloud)
                {
                    Program.SominerVersionCode = Program.SominerVersionCode.Substring(0, Program.SominerVersionCode.LastIndexOf(".")) + ".03";
                }
                else if (Program.SominerVersion == cGlobalParas.VersionType.Ultimate)
                {
                    Program.SominerVersionCode = Program.SominerVersionCode.Substring(0, Program.SominerVersionCode.LastIndexOf(".")) + ".04";
                }
                else if (Program.SominerVersion == cGlobalParas.VersionType.Program)
                {
                    Program.SominerVersionCode = Program.SominerVersionCode.Substring(0, Program.SominerVersionCode.LastIndexOf(".")) + ".01";
                }

                NetMiner.Common.Tool.cVersion version = new NetMiner.Common.Tool.cVersion(Program.getPrjPath());
                version.Registe(this.txtUser.Text, Program.g_EndServiceDate.ToString("yyyy-MM-dd HH:mm:ss"), Program.SominerVersionCode);
                version = null;
            }


            this.Close();
            this.DialogResult = DialogResult.OK;
        }
    }
}

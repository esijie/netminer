using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MinerSpider
{
    public partial class frmAddCodePara : Form
    {
        public delegate void ReturnScript(string Url);
        public ReturnScript rCodeRule;

        public frmAddCodePara()
        {
            InitializeComponent();
        }

        private void cmdBrowserPlugins1_Click(object sender, EventArgs e)
        {

            this.openFileDialog1.Title = "请选择插件文件";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "插件(*.dll)|*.dll";


            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtDMPlugin.Text = this.openFileDialog1.FileName;
            }
        }

        private void cmdSetPlugins1_Click(object sender, EventArgs e)
        {
            if (this.txtDMPlugin.Text.Trim() == "")
            {
                MessageBox.Show("请先选择插件，然后再进行插件设置，选择插件请点击浏览按钮，选择插件dll文件！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmdBrowserPlugins1.Focus();
                return;
            }

            NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
            rPlugin.CallSetConfig(this.txtDMPlugin.Text);
            rPlugin = null;
        }

        private void isAutoDM_CheckedChanged(object sender, EventArgs e)
        {
            if (this.isAutoDM.Checked == true)
            {
                this.txtDMPlugin.Enabled = true;
                this.cmdBrowserPlugins1.Enabled = true;
                this.cmdSetPlugins1.Enabled = true;
            }
            else
            {
                this.txtDMPlugin.Enabled = false;
                this.cmdBrowserPlugins1.Enabled = false;
                this.cmdSetPlugins1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
           string  str = "[Url:" + this.txtDMUrl.Text  + ",Auto:" + this.isAutoDM.Checked.ToString () 
               + ",Plugin:" + this.txtDMPlugin.Text + "]";

           rCodeRule(str);
           this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

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

namespace MinerSpider
{
    public partial class frmAddPlugins : Form
    {
        private ResourceManager rmPara;
        public delegate void ReturnPlugin(string pName,string pFile,cGlobalParas.PluginsType pType);
        public ReturnPlugin rPlugin;


        public frmAddPlugins()
        {
            InitializeComponent();
            rmPara = new ResourceManager("NetMiner.Resource.Resources.globalPara", Assembly.Load("NetMiner.Resource"));
        }

        private void frmAddPlugins_Load(object sender, EventArgs e)
        {
            this.comType.Items.Add(rmPara.GetString("PluginType1"));
            this.comType.Items.Add(rmPara.GetString("PluginType2"));
            this.comType.Items.Add(rmPara.GetString("PluginType3"));
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmAddPlugins_FormClosing(object sender, FormClosingEventArgs e)
        {
            rmPara = null;
        }

        private void cmdBrowser_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "请选择插件";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "Plugin Files(*.dll)|*.dll";

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtPlugin.Text = this.openFileDialog1.FileName;
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (this.txtPluginName.Text.Trim() == "")
            {
                MessageBox.Show("请输入插件的名称！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtPluginName.Focus();
                return;
            }

            if (this.txtPlugin .Text.Trim() == "")
            {
                MessageBox.Show("插件不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtPlugin.Focus();
                return;
            }

            if (this.comType.Text.Trim() == "")
            {
                MessageBox.Show("插件处理规则不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.comType.Focus();
                return;
            }

            rPlugin(this.txtPluginName.Text, this.txtPlugin.Text, EnumUtil.GetEnumName<cGlobalParas.PluginsType>( this.comType.Text));
            
            this.Close();
        }
    }
}

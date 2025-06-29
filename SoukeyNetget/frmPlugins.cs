using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetMiner.Gather;
using NetMiner.Core.Plugin;
using NetMiner.Resource;

namespace MinerSpider
{
    public partial class frmPlugins : Form
    {
        public frmPlugins()
        {
            InitializeComponent();
        }

        private void cmdAddPlugins_Click(object sender, EventArgs e)
        {
            frmAddPlugins f = new frmAddPlugins();
            f.rPlugin = this.AddPlugin;
            f.ShowDialog();
            f.Dispose();

        }

        private void AddPlugin(string pName,string pFile,cGlobalParas.PluginsType pType)
        {
            string sXml = "<Name>" + pName + "</Name>";
            sXml += "<File>" + pFile + "</File>";
            sXml += "<Type>" + (int)pType + "</Type>";

            cPlugin p = new cPlugin(Program.getPrjPath());
            p.InsertPlugin(sXml);
            p = null;

            ListViewItem cItem = new ListViewItem();

            cItem.Text = pName;
            cItem.SubItems.Add(pFile);
            cItem.SubItems.Add(pType.GetDescription ());

            this.listPlugin.Items.Add(cItem);

        }


        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPlugins_Load(object sender, EventArgs e)
        {
            cPlugin p = new cPlugin(Program.getPrjPath());
            int count = p.GetCount();
            for (int i = 0; i < count; i++)
            {
                ListViewItem cItem = new ListViewItem();

                cItem.Text = p.GetPluginName(i);
                cItem.SubItems.Add(p.GetPlugin(i));
                cItem.SubItems.Add(p.GetPluginType(i).GetDescription());

                this.listPlugin.Items.Add(cItem);
            }

        }

        private void cmdDelPlugins_Click(object sender, EventArgs e)
        {
            Del();
        }

        private void Del()
        {
            if (this.listPlugin.SelectedItems.Count == 0)
                return;

            if (MessageBox.Show("是否取消插件：" + this.listPlugin.SelectedItems[0].Text + "的注册？", "网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.No)
                return;

            cPlugin p = new cPlugin(Program.getPrjPath());
            p.DelPlugin(this.listPlugin.SelectedItems[0].Text);
            p = null;

            this.listPlugin.Items.Remove(this.listPlugin.SelectedItems[0]);
        }

        private void listPlugin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && this.listPlugin.SelectedItems.Count > 0)
            {
                Del();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TestPlugin();
        }

        private void TestPlugin()
        {
            if (this.listPlugin.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择需要测试的插件！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            frmTestPlugin f = new frmTestPlugin();
            f.txtPlugin.Text = this.listPlugin.SelectedItems[0].Text;
            f.txtPlugin.Tag = this.listPlugin.SelectedItems[0].SubItems[1].Text;
            f.pType = EnumUtil.GetEnumName<cGlobalParas.PluginsType>(this.listPlugin.SelectedItems[0].SubItems[2].Text);
            f.ShowDialog();
            f.Dispose();
        }

        private void listPlugin_DoubleClick(object sender, EventArgs e)
        {
            TestPlugin();
        }

     
    }
}

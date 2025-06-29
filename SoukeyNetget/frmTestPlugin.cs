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
    public partial class frmTestPlugin : Form
    {
        public cGlobalParas.PluginsType pType;
        public frmTestPlugin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
            rPlugin.CallSetConfig(this.txtPlugin.Tag.ToString ());
            rPlugin = null;
        }

        private void frmTestPlugin_Load(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Add();
            this.dataGridView1.Rows[0].Cells[0].Value = "1";
            this.dataGridView1.Rows[0].Cells[1].Value = "这是一个测试数据的标题";
            this.dataGridView1.Rows[0].Cells[2].Value = "网络矿工数据采集软件（以下简称：网络矿工）是一款强大的专业数据采集器，通过用户自定义配置，可快捷的将网页数据结构化存储到本地，并可输出到数据库、发布到网站。网络矿工可应用于数据挖掘、垂直搜索引擎、网站信息聚合、企业口碑监测、舆情信息监测等领域。";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.txtLog.Text = "";

            ExportLog("开始测试插件，请等待......");
            NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();

            
            switch (this.pType)
            {
                case cGlobalParas.PluginsType.GetCookie :
                    ExportLog("插件类型为获取cookie，请等待获取的cookie信息");
                    try
                    {
                        if (this.txtPlugin.Text.Trim() == "")
                        {
                            MessageBox.Show("请输入插件文件地址！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.txtPlugin.Focus();
                            return;
                        }

                        string strCookie = rPlugin.CallGetCookie(this.txtLoginUrl.Text ,"", this.txtPlugin.Tag.ToString());

                        ExportLog("获取cookie成功，如下\r\n" + strCookie);
                    }
                    catch (System.Exception ex)
                    {
                        ExportLog("测试失败，失败信息：" + ex.Message );
                    }
                    break;
                case cGlobalParas.PluginsType.DealData :
                    ExportLog("插件类型为数据加工类，请等待......");
                    try
                    {
                        if (this.txtPlugin.Text.Trim() == "")
                        {
                            MessageBox.Show("请输入插件文件地址！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.txtPlugin.Focus();
                            return;
                        }

                        DataTable d = getData();
                        d=rPlugin.CallDealData(d,this.txtPlugin.Tag.ToString());
                        ExportData(d);

                        ExportLog("测试成功，请查看上面的表格信息，看数据处理结果是否正确！");
                    }
                    catch (System.Exception ex)
                    {
                        ExportLog("测试失败，失败信息：" + ex.Message );
                    }
                    break;
                case cGlobalParas.PluginsType.PublishData :
                    ExportLog("插件类型为发布数据类，请等待......");
                    try
                    {
                        if (this.txtPlugin.Text.Trim() == "")
                        {
                            MessageBox.Show("请输入插件文件地址！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.txtPlugin.Focus();
                            return;
                        }

                        DataTable d = getData();
                        rPlugin.CallPublishData (d,this.txtPlugin.Tag.ToString());

                        ExportLog("测试成功，请查看发布规则指定的发布目的地，查看数据是否发布正确！");
                    }
                    catch (System.Exception ex)
                    {
                        ExportLog("测试失败，失败信息：" + ex.Message );
                    }
                    break;
            }
            rPlugin = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ExportLog(string strLog)
        {
            this.txtLog.Text = strLog + "\r\n" + this.txtLog.Text;
            Application.DoEvents();   
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frmAddField f = new frmAddField();
            f.rFName = this.AddField;
            f.ShowDialog();
            f.Dispose();
        }

        private void AddField(string str)
        {
            this.dataGridView1.Columns.Add(str, str);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedCells.Count == 0)
                return;

            DataGridViewColumn dCol = this.dataGridView1.Columns[this.dataGridView1.SelectedCells[0].ColumnIndex];
            this.dataGridView1.Columns.Remove(dCol);
        }

        private DataTable getData()
        {
            DataTable d = new DataTable();
            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                d.Columns.Add(this.dataGridView1.Columns[i].Name);
            }

            for (int j = 0; j < this.dataGridView1.Rows.Count-1; j++)
            {
                 DataRow r = d.NewRow();
                for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
                {
                    if (this.dataGridView1.Rows[j].Cells[i].Value == null)
                        break;

                    r[i] = this.dataGridView1.Rows[j].Cells[i].Value.ToString();
                }
                d.Rows.Add(r);
            }
            return d;
        }

        private void ExportData(DataTable d)
        {
            this.dataGridView1.Rows.Clear();

            for (int j = 0; j < d.Rows.Count; j++)
            {
                this.dataGridView1.Rows.Add();
                for (int i = 0; i < d.Columns.Count; i++)
                {
                    this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 2].Cells[i].Value = d.Rows[j][i];
                }
            }

        }
      
    }
}

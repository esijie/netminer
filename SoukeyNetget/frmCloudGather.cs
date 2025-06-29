using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using NetMiner.Gather.Task;
using NetMiner.Core.gTask;

namespace MinerSpider
{
    public partial class frmCloudGather : DockContent 
    {
        private string m_User;
        private string m_Integral;
        private bool m_isConnected;

        public frmCloudGather()
        {
            InitializeComponent();
        }

        private void frmCloudGather_Load(object sender, EventArgs e)
        {
            //连接远程服务器
            Cloud.Cloud sc = new Cloud.Cloud();
            sc.Url = Program.g_CloudServer + "/Cloud.asmx";
            DataTable d = null;
            try
            {
                d = sc.ConnectServer(Program.RegisterUser);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("云采集服务器连接失败，请与网络矿工客服人员联系！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.labUser.Text = "";
                this.labIntegral.Text = "";
                this.labAllowCount.Text = "0";
                m_isConnected = false;
                return;
            }

            this.labUser.Text = d.Rows[0]["User"].ToString();
            this.labIntegral.Text = d.Rows[0]["Integral"].ToString();
            this.labAllowCount.Text = d.Rows[0]["RowsCount"].ToString();
            m_isConnected = true;

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AddTask();
        }

        private void AddTask()
        {
            frmAddSominerTask f = new frmAddSominerTask();
            f.RTask = new frmAddSominerTask.ReturnTask(GetTaskInfo);
            f.ShowDialog();
            f.Dispose();
        }

        private void GetTaskInfo(string TaskClass, string TaskName, string Field)
        {
            ListViewItem cItem = new ListViewItem();

            oTaskClass tc = new oTaskClass(Program.getPrjPath());
            string tPath = tc.GetTaskClassPathByName(TaskClass);
            tc = null;
            string fileName = Program.getPrjPath() + tPath + "\\" + TaskName + ".smt";

            //上传采集任务
            Cloud.Cloud sc = new Cloud.Cloud();
            sc.Url = Program.g_CloudServer + "/Cloud.asmx";
            DataTable d = null;
            try
            {
                d = sc.ConnectServer(Program.RegisterUser);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("云采集服务器连接失败，请与网络矿工客服人员联系！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.labUser.Text = "";
                this.labIntegral.Text = "";
                this.labAllowCount.Text = "0";
                m_isConnected = false;
                return;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetMiner.Gather;
using NetMiner.Gather.Task;
using NetMiner.Core.pTask;
using NetMiner.Core.pTask.Entity;

namespace MinerSpider
{
    public partial class frmSelectPublishRule : Form
    {
        public delegate void ReturnSelectPRule(string pName);
        public ReturnSelectPRule rPName;

        public frmSelectPublishRule()
        {
            InitializeComponent();
        }

        private void frmSelectPublishRule_Load(object sender, EventArgs e)
        {
            //加载任务发布模板信息
            oPublishTask cP = new oPublishTask(Program.getPrjPath());
            IEnumerable< ePublishTask> eps= cP.LoadPublishData();
         

            foreach(ePublishTask ep in eps)
            {
                this.listBox1.Items.Add(ep.pName);
            }
            cP = null;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItems.Count == 0)
                return;

            rPName(this.listBox1.SelectedItem.ToString());
            this.Close();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItems.Count == 0)
                return;

            rPName(this.listBox1.SelectedItem.ToString());
            this.Close();
        }
    }
}

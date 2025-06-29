using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MinerSpider
{
    public partial class frmAddRemoteTaskClass : Form
    {

        public delegate void ReturnTaskClass(string TaskClassName);

        public ReturnTaskClass RTaskClass;

        public frmAddRemoteTaskClass()
        {
            InitializeComponent();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text .Trim ()=="")
            {
                MessageBox.Show("请输入任务分类的名称！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.textBox1.Focus();
                return;
            }

            RTaskClass(this.textBox1.Text.Trim());
            this.Close();
        }
    }
}

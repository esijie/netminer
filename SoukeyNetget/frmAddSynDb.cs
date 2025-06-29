using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MinerSpider
{
    public partial class frmAddSynDb : Form
    {
        public delegate void ReturnSName(string sName);
        public ReturnSName rSName;

        public frmAddSynDb()
        {
            InitializeComponent();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim() == "")
            {
                MessageBox.Show("同义词库名称不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.textBox1.Focus();
                return;
            }

            rSName(this.textBox1.Text.Trim());

            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SominerServerMonitor
{
    public partial class frmAddConnnect : Form
    {
        public delegate void ReturnUrl(string url);
        public ReturnUrl rUrl;
        public frmAddConnnect()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim() == "")
            {
                MessageBox.Show("分布式采集引擎地址不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!this.textBox1.Text.StartsWith ("http://",StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show("分布式采集引擎是一个web地址，请修改后重新添加！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            rUrl(this.textBox1.Text);
            this.Close();
        }
    }
}

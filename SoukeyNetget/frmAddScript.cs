using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MinerSpider
{
    public partial class frmAddScript : Form
    {
        public delegate void ReturnScript(string Url);
        public ReturnScript rScript;

        public frmAddScript()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rScript(this.richTextBox1.Text);
            this.Close();
        }
    }
}

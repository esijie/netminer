using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MinerSpider.Help;
using System.Text.RegularExpressions;

namespace MinerSpider
{
    public partial class frmHelp : Form
    {
        public frmHelp()
        {
            InitializeComponent();
        }

        public frmHelp(string keyword)
        {
            InitializeComponent();

            cHelpInfo  hc = new cHelpInfo();
            cHelpContent s = hc.GetByKey(keyword);
            hc = null;

            string str="";
            if (s != null)
                str =s.Content;

            string[] ss = str.Split(new char[] { '\\', 'n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < ss.Length; i++)
            {
                this.richTextBox1.AppendText(ss[i].ToString() + "\n");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

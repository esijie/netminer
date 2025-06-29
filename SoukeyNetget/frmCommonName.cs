using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MinerSpider
{
    public partial class frmCommonName : Form
    {
        public frmCommonName()
        {
            InitializeComponent();
        }

        private void frmCommonName_Load(object sender, EventArgs e)
        {
            string fName = Program.getPrjPath() + "dict\\rulename.txt";
            if (File.Exists(fName))
            {
                //打开此文件
                StreamReader fileReader = new StreamReader(fName, System.Text.Encoding.UTF8);
                string strLine = "";
                StringBuilder sb = new StringBuilder();

                while (strLine != null)
                {
                    strLine = fileReader.ReadLine();
                    if (strLine != null && strLine.Length > 0)
                    {
                        sb.Append(strLine + "\r\n");
                    }
                }

                fileReader.Close();
                fileReader = null;

                this.textBox1.Text = sb.ToString();
                this.cmdOK.Enabled = false ;
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.cmdOK.Enabled = true;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            string fName = Program.getPrjPath() + "dict\\rulename.txt";

            File.Delete(fName);

            StreamWriter sw = new StreamWriter(fName, true, System.Text.Encoding.UTF8);
            string[] ss=this.textBox1.Text.Split ('\r');
            for (int i = 0; i < ss.Length ; i++)
            {
                sw.WriteLine(ss[i].Replace("\n", ""));
            }

            sw.Close();
            sw.Dispose();
            this.cmdOK.Enabled = false;
        }
    }
}

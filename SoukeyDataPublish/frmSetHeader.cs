using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;

namespace SoukeyDataPublish
{
    public partial class frmSetHeader : Form
    {
        private ResourceManager rm;

        public delegate void ReturnHeader(string DataSource);
        public ReturnHeader rHeader;

        public frmSetHeader()
        {
            InitializeComponent();
        }

        private string m_strHeader;
        public string strHeader
        {
            get { return m_strHeader; }
            set { m_strHeader = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.errorProvider1.Clear();

            if (this.comboBox1.SelectedIndex == -1)
            {
                this.errorProvider1.SetError(this.comboBox1, rm.GetString("Error39"));
                this.comboBox1.Focus();
                return;
            }

            if (this.textBox1.Text =="")
            {
                this.errorProvider1.SetError(this.textBox1, rm.GetString("Error39"));
                this.textBox1.Focus();
                return;
            }

            ListViewItem cItem = new ListViewItem();

            cItem.Text = this.comboBox1.SelectedItem.ToString ();
            cItem.SubItems.Add(this.textBox1.Text);

            this.listView1.Items.Add(cItem);

            cItem = null;

            this.comboBox1.SelectedIndex = -1;
            this.textBox1.Text = "";
        }

        private void frmSetHeader_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            if (this.strHeader != "")
            {
                ListViewItem cItem;

                foreach (string sc in strHeader.Split('\r'))
                {
                    cItem = new ListViewItem();

                    string ss = sc.Trim();
                    cItem.Text= ss.Substring(0, ss.IndexOf(":"));
                    cItem.SubItems.Add ( ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1));

                    this.listView1.Items.Add(cItem);
                }
            }
        }

        private void frmSetHeader_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count ==0)
                return;

            this.listView1.Items.Remove(this.listView1.SelectedItems[0]);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string strHeader="";

            for (int i = 0; i<this.listView1.Items.Count; i++)
            {
                strHeader += this.listView1.Items[i].Text + ":";

                if (i==this.listView1.Items.Count -1)
                    strHeader += this.listView1.Items[i].SubItems[1].Text.ToString();
                else
                    strHeader += this.listView1.Items[i].SubItems[1].Text.ToString() + "\r\n";
            }

            rHeader(strHeader);

            this.Close();
            
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && this.listView1.SelectedItems.Count > 0)
            {
                this.listView1.Items.Remove(this.listView1.SelectedItems[0]);
            }
        }
    }
}
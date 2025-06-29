using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using NetMiner.Resource;

namespace MinerSpider
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

        private int m_MaxLevel;
        public int MaxLevel
        {
            get { return m_MaxLevel; }
            set { m_MaxLevel = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.errorProvider1.Clear();

            if (this.comboBox1.Text.Trim () == "")
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
            string ss = string.Empty;

            cItem.Text = this.comboBox1.Text .ToString ();

            if (this.comboBox2.Text == "所有")
            {
                ss = "[All]";
            }
            else if (this.comboBox2.Text == "导航")
            {
                ss = "[Na:" + this.comboBox3.Text + "]";
            }
            else if (this.comboBox2.Text == "采集")
            {
                ss = "[Ga]";
            }

            cItem.SubItems.Add(ss);
            cItem.SubItems.Add(this.textBox1.Text);
            this.listView1.Items.Add(cItem);

            cItem = null;

            this.comboBox1.SelectedIndex = -1;
            this.comboBox1.Text = "";
            this.textBox1.Text = "";
            this.comboBox2.SelectedIndex = 0;
            this.comboBox3.Enabled = false;
            this.comboBox3.SelectedIndex = -1;
        }

        private void frmSetHeader_Load(object sender, EventArgs e)
        {

            this.comboBox2.SelectedIndex = 0;

            if (MaxLevel == 0)
            {
                this.comboBox2.Text = "所有";
                this.comboBox2.Enabled = false;
                this.comboBox3.Enabled = false;
            }
            else
            {
                this.comboBox3.Items.Clear();
                for (int i = 1; i <= MaxLevel; i++)
                {
                    this.comboBox3.Items.Add(i);
                }
            }

            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            if (this.strHeader != "")
            {
                ListViewItem cItem;

                foreach (string sc in strHeader.Split('\r'))
                {
                    cItem = new ListViewItem();

                    string ss = sc.Trim();

                    if (ss.StartsWith("["))
                    {
                        //表示有作用域信息
                        string s1 = ss.Substring(0, ss.IndexOf("]")+1);
                        ss = ss.Replace(s1, "");

                        cItem.Text = ss.Substring(0, ss.IndexOf(":"));
                        cItem.SubItems.Add(s1);
                        cItem.SubItems.Add(ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1));
                    }
                    else
                    {
                        cItem.Text = ss.Substring(0, ss.IndexOf(":"));
                        cItem.SubItems.Add("[All]");
                        cItem.SubItems.Add(ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1));
                    }

                    this.listView1.Items.Add(cItem);
                }
            }

            if (Program.SominerVersion == cGlobalParas.VersionType.Free)
            {
                this.comboBox2.Enabled = false;
                this.comboBox3.Enabled = false;
                this.button5.Enabled = false;
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
                strHeader +=this.listView1.Items[i].SubItems[1].Text.ToString() + this.listView1.Items[i].Text + ":";

                if (i==this.listView1.Items.Count -1)
                    strHeader += this.listView1.Items[i].SubItems[2].Text.ToString();
                else
                    strHeader += this.listView1.Items[i].SubItems[2].Text.ToString() + "\r\n";
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

        private void button5_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Show(this.button5, 0, 21);
            
        }

        private void rmenuGetCookie_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "{Cookie:}";
        }

        private void rmenuGetRule_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "{ReferData:在此输入正则}";
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox2.Text == "所有" || this.comboBox2.Text == "采集")
            {
                this.comboBox3.Enabled = false;
                this.comboBox3.SelectedIndex = -1;
            }
            else
            {
                this.comboBox3.Enabled = true;
                this.comboBox3.SelectedIndex = 0;
            }
        }
    }
}
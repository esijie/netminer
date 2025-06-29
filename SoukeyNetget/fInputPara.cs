using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MinerSpider
{
    public partial class fInputPara : Form
    {
        public delegate void ReturnUrl(string Url);
        public ReturnUrl rUrl;
        private string m_Url;

        //由外部调用，传递一个带有参数的Url，并把参数分解
        public void GetPara(string Url)
        {
            m_Url = Url;

            Regex re = new Regex("(?<=<EPara>).*?(?=</EPara>)", RegexOptions.None);
            MatchCollection mc = re.Matches(Url);

            this.dataGridView1.Rows.Clear();

            foreach (Match ma in mc)
            {
                this.dataGridView1.Rows.Add(ma.ToString());
            }
        }

        public fInputPara()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                m_Url=m_Url.Replace(this.dataGridView1.Rows[i].Cells[0].Value.ToString(), this.dataGridView1.Rows[i].Cells[1].Value.ToString());
            }

            m_Url = m_Url.Replace("<EPara>", "");
            m_Url = m_Url.Replace("</EPara>", "");
            rUrl(m_Url);

            this.Close();

        }
    }
}
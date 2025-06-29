using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MinerSpider
{
    public partial class frmInput : Form
    {
        public delegate void ReturnValue(string strValue);
        public ReturnValue rValue;

        private bool m_isJoin = false;
        private string m_str = string.Empty;

        public frmInput(bool  isJoin ,string str)
        {
            InitializeComponent();
            m_isJoin = isJoin;
            m_str = str;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim() == "")
            {
                MessageBox.Show("请输入相关的内容！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string ss = this.textBox1.Text.Trim();
            if (m_isJoin == true)
            {
                //需要合并字符串
                if (m_str.IndexOf("{")==0 && m_str.IndexOf("}")==m_str.Length -1  && m_str.IndexOf(":") > -1)
                {
                    ss = m_str.Substring(0, m_str.IndexOf(":")+1) + ss + "}";
                }
                else
                    ss = m_str + ss;
            }

            rValue(ss);
            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;

namespace SoukeyControl
{
    public partial class frmFilterCondition : Form
    {
        //定义一个访问资源文件的变量
        private ResourceManager rm;

        public delegate void ReturnValue(string NavRule);
        public ReturnValue rValue;

        private bool m_IsHoldClose = false;

        //private cGlobalParas.FormatLimit m_FormatLimit;
        //public cGlobalParas.FormatLimit FormatLimit
        //{
        //    get { return m_FormatLimit; }
        //    set { m_FormatLimit = value; }
        //}

        public frmFilterCondition()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m_IsHoldClose = false;
            rValue("");
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.txtValue.Text.Trim() == "")
            {
                MessageBox.Show(rm.GetString("Info119"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                m_IsHoldClose = true;
                return;
            }

            rValue(this.txtValue.Text);
            this.Close();
        }

        private void frmFilterCondition_Load(object sender, EventArgs e)
        {
            //初始化资源文件的参数
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

        }

        private void frmFilterCondition_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void frmFilterCondition_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
            {
                if (m_IsHoldClose == true)
                    e.Cancel = true;
            }
        }
    }
}
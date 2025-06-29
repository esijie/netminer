using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MinerSpider
{
    public partial class frmTestData : Form
    {
        //返回信息 0-保存退出；1-继续配置
        public delegate void RuturnResult(int result);
        public RuturnResult RResult;

        public frmTestData()
        {
            InitializeComponent();
        }

        private void frmTestData_Load(object sender, EventArgs e)
        {

        }

        public void LoadData(DataTable d)
        {
            this.dataTestGather.DataSource = d;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RResult(0);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RResult(1);
            this.Close();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetMiner.Resource;

///���ܣ�ϵͳ�����ȴ�����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace MinerSpider
{
    public partial class frmStart : Form
    {
        public frmStart()
        {
            InitializeComponent();
            string lVersion=Program.SominerShowVersion;

            this.labVersion.Text = lVersion;
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
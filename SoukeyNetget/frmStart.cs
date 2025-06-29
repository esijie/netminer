using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetMiner.Resource;

///功能：系统启动等待窗体
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
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
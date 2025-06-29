using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetMiner.Resource;

namespace MinerSpider
{
    public partial class frmDispose : Form
    {
        public delegate void ReturnExitPara(cGlobalParas.ExitPara ePara);
        public ReturnExitPara RExitPara;

        public frmDispose()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cGlobalParas.ExitPara ePara = cGlobalParas.ExitPara.MinForm;

            if (this.raMin.Checked == true)
            {
                ePara = cGlobalParas.ExitPara.MinForm;
            }
            else if (this.raExit.Checked == true)
            {
                ePara = cGlobalParas.ExitPara.Exit;
            }

            RExitPara(ePara);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

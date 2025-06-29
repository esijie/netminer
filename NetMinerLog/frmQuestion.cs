using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetMiner.Resource;

namespace SoukeyLog
{
    public partial class frmQuestion : Form
    {
        public delegate void ReturnPara(cGlobalParas.LogExitPara ePara);
        public ReturnPara RPara;

        public frmQuestion()
        {
            InitializeComponent();
        }

        private void frmQuestion_Load(object sender, EventArgs e)
        {

        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            this.Close();
            RPara(cGlobalParas.LogExitPara.Cancel);
        }

        private void cmdClear_Click(object sender, EventArgs e)
        {
            this.Close();
            RPara(cGlobalParas.LogExitPara.Clear );
        }

        private void cmdClearBackup_Click(object sender, EventArgs e)
        {
            this.Close();
            RPara(cGlobalParas.LogExitPara.BackupAndClear );
        }
    }
}
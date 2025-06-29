using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using NetMiner.Gather;
using NetMiner.Resource;

namespace SoukeyDataPublish
{
    public partial class frmDataOp : DockContent 
    {
        public frmDataOp()
        {
            InitializeComponent();
        }

        private void frmDataOp_Load(object sender, EventArgs e)
        {
            this.myData.IniControl();
        }

        public void LoadData(cGlobalParas.DatabaseType dType,string dSource,string dSql)
        {
            if (dType == cGlobalParas.DatabaseType.SoukeyData)
                this.Text = dSource;
            else
                this.Text = dSource;

            this.myData.LoadData(dType,dSource,dSql);
            
        }
    }
}
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

namespace MinerSpider
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

        public void LoadData(cGlobalParas.DatabaseType dType,string dSource,string dSql,string TaskName)
        {
            if (dType == cGlobalParas.DatabaseType.SoukeyData)
                this.Text = "手动导出数据：" + TaskName;
            else
                this.Text = "手动导出数据";

            this.myData.LoadData(dType,dSource,dSql);
            
        }
    }
}
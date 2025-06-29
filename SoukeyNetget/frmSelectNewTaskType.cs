using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetMiner.Gather;
using System.Resources;
using System.Reflection;
using NetMiner.Resource;
using NetMiner.Common;

namespace MinerSpider
{
    public partial class frmSelectNewTaskType : Form
    {
        public delegate void ReturnExitPara(cGlobalParas.NewTaskType ePara);
        public ReturnExitPara RNewPara;

        private ResourceManager rm;

        public frmSelectNewTaskType()
        {
            InitializeComponent();
        }

        private void frmSelectNewTaskType_Load(object sender, EventArgs e)
        {
            try
            {
                cXmlSConfig Config = new cXmlSConfig(Program.getPrjPath());
                if (Config.NewTaskType == (int)cGlobalParas.NewTaskType.Wizard )
                    this.raWizard.Checked = true;
                else if (Config.NewTaskType == (int)cGlobalParas.NewTaskType.Normal )
                    this.raNormal.Checked = true;

                if (Config.NewTaskIsShow == true)
                    this.IsShow.Checked = false;
                else
                    this.IsShow.Checked = true;

                Config = null;
            }
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString("Info76"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmSelectNewTaskType_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void IsShow_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfigData();
        }

        private void SaveConfigData()
        {
            try
            {
                cXmlSConfig Config = new cXmlSConfig(Program.getPrjPath());
                if (this.raNormal.Checked == true)
                    Config.NewTaskType = (int)cGlobalParas.NewTaskType.Normal;
                else if (this.raWizard.Checked == true)
                    Config.NewTaskType = (int)cGlobalParas.NewTaskType.Wizard;

                if (this.IsShow.Checked == true)
                    Config.NewTaskIsShow = false;
                else
                    Config.NewTaskIsShow = true;
                Config = null;
            }
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString("Info76"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void raWizard_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfigData();
        }

        private void raNormal_CheckedChanged(object sender, EventArgs e)
        {
            SaveConfigData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RNewPara(cGlobalParas.NewTaskType.Cancel);
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();

            if (this.raWizard.Checked == true)
                RNewPara(cGlobalParas.NewTaskType.Wizard);
            else if (this.raNormal.Checked == true)
                RNewPara(cGlobalParas.NewTaskType.Normal);
            
        }
    }
}
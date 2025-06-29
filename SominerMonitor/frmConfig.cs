using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SominerMonitor
{
    public partial class frmConfig : Form
    {
        public frmConfig()
        {
            InitializeComponent();
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            cXmlSConfig sCon = new cXmlSConfig();
            this.isAutoConnect.Checked = sCon.AutoConnect;
            this.txtBindAddress.Text = sCon.BindAddress;
            this.txtBindPort.Text = sCon.BindPort.ToString ();
            this.txtGatherAddress.Text = sCon.GatherAddress;
            this.txtGatherPort.Text = sCon.GatherPort.ToString();
            sCon = null;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            cXmlSConfig sCon = new cXmlSConfig();
            sCon.AutoConnect = this.isAutoConnect.Checked;
            sCon.BindAddress = this.txtBindAddress.Text;
            sCon.GatherAddress = this.txtGatherAddress.Text;
            try
            {
                sCon.BindPort = int.Parse(this.txtBindPort.Text);
                sCon.GatherPort = int.Parse(this.txtGatherPort.Text);
            }
            catch 
            {
                MessageBox.Show("端口必须是数字，请重新输入！","网络矿工",MessageBoxButtons.OK ,MessageBoxIcon.Error );
                sCon = null;
                return;
            }
            
            sCon.Save();
            sCon = null;

            this.Close();
        }
    }
}

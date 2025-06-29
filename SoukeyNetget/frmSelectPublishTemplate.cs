using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetMiner.Resource;

namespace MinerSpider
{
    
    public partial class frmSelectPublishTemplate : Form
    {
        public delegate void ReturnPType(cGlobalParas.PublishTemplateType pType);
        public ReturnPType RPType;

        public frmSelectPublishTemplate()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();

            if (this.raWeb.Checked == true)
                RPType(cGlobalParas.PublishTemplateType.Web);
            else if (this.raDB.Checked == true)
                RPType(cGlobalParas.PublishTemplateType.DB);

            
        }
    }
}

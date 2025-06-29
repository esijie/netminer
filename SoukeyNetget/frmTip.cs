using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MinerSpider
{
    public partial class frmTip : Form
    {
        public frmTip()
        {
            InitializeComponent();
        }

        public void LoadHelp(string key)
        {
            Help.cHelpContent hContent;
            hContent = Program.SominerHelp.GetByKey(key);

            if (hContent != null)
            {
                this.txtFindKey.Text = hContent.Title;
                this.txtFindKey.ForeColor = Color.Black;
                this.txtHelp.Text = hContent.Content;
            }
        }

        private void txtFindKey_Enter(object sender, EventArgs e)
        {
            if (this.txtFindKey.Text.Trim() == "请输入查询问题的关键字")
            {
                this.txtFindKey.Text = "";
                this.txtFindKey.ForeColor = Color.Black;
            }
        }

        private void txtFindKey_Leave(object sender, EventArgs e)
        {
            if (this.txtFindKey.Text.Trim() == "")
            {
                this.txtFindKey.Text = "请输入查询问题的关键字";
                this.txtFindKey.ForeColor = Color.FromArgb(224, 224, 224);
            }
        }

        private void frmTip_Load(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MinerSpider
{
    public partial class frmAutoExit : Form
    {
        //窗体关闭延迟时间，默认30秒
        private int DelayTime = 30000;

        public frmAutoExit()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (DelayTime == 0 || DelayTime < 0)
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }
            else
                DelayTime = DelayTime - 1000;

            this.labDelay.Text = ((int)(DelayTime / 1000)).ToString ();
        }

        private void frmAutoExit_Load(object sender, EventArgs e)
        {
            this.timer1.Enabled = true;
            this.labDelay.Visible = true;

            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height ;
            //this.ststate = 0;
        }

        private void frmAutoExit_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetMiner.Gather;
using NetMiner.Resource;

namespace MinerSpider
{
    public partial class frmTrialInfo : Form
    {
        //����ر��ӳ�ʱ�䣬Ĭ��30��
        private int DelayTime = 30000;

        //����һ��������ȡע�ἤ��Ľ�����ڴ˴����ã���Ϊ�˱�֤���ýӿ�ͳһ����
        private cGlobalParas.RegisterResult m_rResult;

        public frmTrialInfo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmRegister f = new frmRegister();
            f.RRResult = new frmRegister.ReturnRResult(this.GetrResult);
            this.timer1.Enabled = false;

            f.ShowDialog();
            f.Dispose();

            if (m_rResult == cGlobalParas.RegisterResult.Succeed)
            {
               
            }

            this.timer1.Enabled = true;
            this.Dispose();
           
        }

        private void GetrResult(cGlobalParas.RegisterResult rResult)
        {
            m_rResult = rResult;
        }

        private void frmTrialInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void frmTrialInfo_Load(object sender, EventArgs e)
        {
            this.timer1.Enabled = true;
            this.labDelay.Visible = true;

            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
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

            this.labDelay.Text = ((int)(DelayTime / 1000)).ToString();
        }
    }
}
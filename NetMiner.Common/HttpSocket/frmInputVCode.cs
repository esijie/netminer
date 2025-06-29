using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace NetMiner.Common.HttpSocket
{
    public partial class frmInputVCode : Form
    {
        public delegate void ReturnVCode(string code,string cookie);
        public ReturnVCode RVCode;
        private string m_CodeUrl = "";
        private string m_cookie = "";
        private string m_workPath = string.Empty;

        public frmInputVCode()
        {
            InitializeComponent();

        }

        private void frmInputVCode_Load(object sender, EventArgs e)
        {

        }

        public void iniData(string url,string cookie)
        {
            //请求验证码图片
            m_CodeUrl = url;
            m_cookie = cookie;

            try
            {
                string fName = NetMiner.Common.HttpSocket.cHttpSocket.GetImage(this.m_workPath, url, ref cookie);
                FileStream fs = new FileStream(fName, FileMode.Open, FileAccess.Read);
                Image bmp = System.Drawing.Bitmap.FromStream(fs);
                fs.Close();
                fs.Dispose();
                m_cookie = cookie;

                this.pictureBox1.Image = bmp;
            }
            catch (System.Exception)
            {
                this.pictureBox1.Image = null;
            }
            
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            RVCode(this.textBox1.Text.Trim(),m_cookie);
            this.Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string fName = NetMiner.Common.HttpSocket.cHttpSocket.GetImage(Path.GetTempPath().ToString (), m_CodeUrl,ref this.m_cookie);


            FileStream fs = new FileStream(fName, FileMode.Open, FileAccess.Read);
            Image bmp = System.Drawing.Bitmap.FromStream(fs);
            fs.Close();
            fs.Dispose();

            this.pictureBox1.Image = bmp;
        }
    }
}

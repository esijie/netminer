using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;

namespace SoukeyDataPublish
{
    public partial class frmAddPTask : Form
    {
        //定义一个访问资源文件的变量
        private ResourceManager rm;

        public delegate void ReturnName(string strName,bool isOverwrite);
        public ReturnName rName;



        public frmAddPTask()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            rName("",false );
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text =="")
            {
                MessageBox.Show (rm.GetString ("Info2244"),rm.GetString ("MessageboxError"),MessageBoxButtons.OK ,MessageBoxIcon.Error );
                return ;
            }

            rName(this.textBox1.Text,false );
            this.Close();

        }
    

        private void cAddPTask_Load(object sender, EventArgs e)
        {
            //初始化资源文件的参数
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));
        }

        private void cAddPTask_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm=null;
        }
    
    }
}
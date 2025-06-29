using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Resources;
using NetMiner.Gather.Task;
using NetMiner.Common;
using NetMiner;
using NetMiner.Core.gTask;

///功能：采集任务分类信息处理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace MinerSpider
{
    public partial class frmTaskClass : Form
    {
        public delegate void ReturnTaskClass(int fID, int TaskClassID, string TaskClassName,string TaskClassPath);

        public ReturnTaskClass RTaskClass;

        private string  DefaultPath="";

        private ResourceManager rm;

        private bool m_IsHoldClose = false;

        public frmTaskClass(int fID,string fName)
        {
            InitializeComponent();

            string tPath = string.Empty;

            if (string.IsNullOrEmpty(fName))
            {
                this.textBox3.Text ="";
                tPath = Program.getPrjPath() + "tasks\\";
                this.textBox2.Text = tPath;
            }
            else
            {
                this.textBox3.Text = fName;
                oTaskClass tc = new oTaskClass(Program.getPrjPath());
                tPath = tc.GetTaskClassPathByID(fID) + "\\";
                this.textBox2.Text = tPath;
                tc = null;
            }
            this.textBox3.Tag = fID.ToString();

        
            DefaultPath = tPath;
           
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            m_IsHoldClose = false;
            this.Dispose();
        }

        private void frmTaskClass_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

          
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            int TaskClassID = 0;

            if (this.textBox1.Text.Trim().ToString() == "")
            {
                MessageBox.Show(rm.GetString ("Info88"), rm.GetString("MessageboxError"),MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_IsHoldClose = true;
                this.textBox1.Focus();
                return;
            }

            if (this.textBox1.Text.Trim() ==Program.g_RemoteTaskClass)
            {
                MessageBox.Show("此分类名称为系统所用，请更换其他的名称！", rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_IsHoldClose = true;
                this.textBox1.Focus();
                return;
            }

            try
            {
                oTaskClass cTClass = new oTaskClass(Program.getPrjPath());

                bool isExist = false;
                //带上上一级的分类名称，以判断此分类是否已经存在
                if (this.textBox3.Text.Trim() == "")
                {
                    isExist = cTClass.IsExist(this.textBox1.Text);  

                    if (isExist)
                    {
                        cTClass.Dispose();
                        cTClass = null;
                        MessageBox.Show( "已经存在此任务分类，请修改任务分类名称后重试！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    TaskClassID = cTClass.AddTaskClass(this.textBox1.Text.Trim());//, this.textBox2.Text);
                }
                else
                {
                    isExist = cTClass.IsExist(this.textBox3.Text.Trim() + "/" + this.textBox1.Text.Trim());

                    if (isExist)
                    {
                        cTClass.Dispose();
                        cTClass = null;
                        MessageBox.Show("已经存在此任务分类，请修改任务分类名称后重试！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    TaskClassID = cTClass.AddTaskClass(this.textBox3.Text.Trim() + "/" + this.textBox1.Text.Trim());//, this.textBox2.Text);
                }

                cTClass.Dispose();
                cTClass = null;

                //开始创建索引文件
                oTaskIndex ti = new oTaskIndex(Program.getPrjPath());
                string newClassPath = (this.textBox3.Text.Trim() + "/" + this.textBox1.Text.Trim()).Replace("/", "\\");
                ti.NewIndexFile(Program.getPrjPath() + NetMiner.Constant.g_TaskPath + "\\" + newClassPath);
                ti.Dispose();
                ti = null;

            }
            catch (NetMinerException ex)
            {
                MessageBox.Show(ex.Message, rm.GetString ("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_IsHoldClose = true;
                return;
            }
            catch (System.Exception  ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_IsHoldClose = true;
                return;
            }


            string tClassPath = this.textBox2.Text.Replace( Program.getPrjPath(),"");

            RTaskClass(int.Parse ( this.textBox3.Tag.ToString ()), TaskClassID, this.textBox1.Text, tClassPath);

            this.Dispose();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    folderBrowserDialog1.SelectedPath = Program.getPrjPath();
        //    folderBrowserDialog1.ShowNewFolderButton = true;
        //    folderBrowserDialog1.Description = "请选择任务分类存储的目录";
        //    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
        //    {
        //        this.textBox2.Text= folderBrowserDialog1.SelectedPath + "\\";
        //        DefaultPath = this.textBox2.Text;

        //    }
        //}

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.textBox2.Text = DefaultPath + this.textBox1.Text;
            this.textBox2.Select(this.textBox2.Text.Length, 0);
            this.textBox2.ScrollToCaret();
        }

        private void frmTaskClass_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void frmTaskClass_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
            {
                if (m_IsHoldClose == true)
                    e.Cancel = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox3.Text = "";
            string tPath = Program.getPrjPath() + "tasks\\";
            this.textBox2.Text = tPath;

            this.textBox3.Tag = "0";

            DefaultPath = tPath;
        }

    }
}
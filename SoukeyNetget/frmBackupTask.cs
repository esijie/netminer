using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.IO;
using System.Text.RegularExpressions;
using NetMiner.Common;

namespace MinerSpider
{
    public partial class frmBackupTask : Form
    {
        private ResourceManager rm;
        public int StartFindFile;

        public frmBackupTask()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.Description = rm.GetString("Info263");
            this.folderBrowserDialog1.SelectedPath = Program.getPrjPath();
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtSavePath.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmBackupTask_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));
        }

        private void frmBackupTask_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.txtSavePath.Text.Trim() == "")
            {
                MessageBox.Show(rm.GetString("Info263"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.button1.Focus();
                return;
            }

            try
            {
                if (!Directory.Exists(this.txtSavePath.Text))
                {
                    MessageBox.Show (rm.GetString("Info264"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                 MessageBox.Show (rm.GetString("Info265"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error );
                 return;
            }

            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;

            this.progressBar1.Visible = true;
            this.label2.Visible = true;

            StartFindFile = 0;

            string taskPath = Program.getPrjPath() + "tasks";

            FindFile(taskPath,this.txtSavePath.Text);

            this.button3.Enabled = true;

            this.progressBar1.Visible = false;
            this.label2.Visible = false ;

            MessageBox.Show(rm.GetString("Info267"), rm.GetString("MessageboxInfo"),MessageBoxButtons.OK ,MessageBoxIcon.Information );
            
        }

        private void FindFile(string tPath, string newPath)
        {
            DirectoryInfo di = new DirectoryInfo(tPath);
            DirectoryInfo[] diA = di.GetDirectories();
            if (StartFindFile == 0)
            {
                FileInfo[] fis2 = di.GetFiles();   //有关目录下的文件   

                this.progressBar1.Maximum = fis2.Length;
                this.progressBar1.Minimum = 0;
                this.progressBar1.Value = 0;

                for (int i2 = 0; i2 < fis2.Length; i2++)
                {
                    string ss = fis2[i2].FullName;

                    string oldFile = ss;
                    string newFile=newPath + "\\" + Path.GetFileName (ss);

                    if (Path.GetExtension(oldFile) == ".smt")
                    {
                        bool isSucceed = delegateBTask(oldFile, newFile);
                    }

                    this.progressBar1.Value = i2;

                    this.label2.Text = rm.GetString("Info266") + ":" + fis2[i2].FullName;
                    Application.DoEvents();
                }
            }

            for (int i = 0; i < diA.Length; i++)
            {
                StartFindFile++;

                //新建目录

                Match charSetMatch;
                charSetMatch = Regex.Match(diA[i].FullName, "(?<=" + ToolUtil.RegexReplaceTrans(tPath,true ) + ").*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string path1 = charSetMatch.Groups[0].ToString();

                string nPath = newPath + path1;

                if (!Directory.Exists(nPath))
                {
                    Directory.CreateDirectory(nPath);
                }

                DirectoryInfo di1 = new DirectoryInfo(diA[i].FullName);
                DirectoryInfo[] diA1 = di1.GetDirectories();
                FileInfo[] fis1 = di1.GetFiles();   //有关目录下的文件   

                this.progressBar1.Maximum = fis1.Length;
                this.progressBar1.Minimum = 0;
                this.progressBar1.Value = 0;

                for (int ii = 0; ii < fis1.Length; ii++)
                {
                    string ss = fis1[ii].FullName;

                    string oldFile = ss;
                    string newFile = nPath + "\\" + Path.GetFileName (ss);
                    if (Path.GetExtension(oldFile) == ".smt")
                    {
                        bool isSucceed = delegateBTask(oldFile, newFile);
                    }

                    this.progressBar1.Value = ii;
                    this.label2.Text = rm.GetString("Info266") + ":" + fis1[ii].FullName;
                    Application.DoEvents();

                }

                FindFile(diA[i].FullName,nPath);

            }
        }

        private delegate bool delegateBackupTask(string sPath,string NewFileName);
        private bool delegateBTask(string sPath, string NewFileName)
        {
            //定义一个修改分类名称的委托

            string errMessage = "";

            delegateBackupTask sd = new delegateBackupTask(this.BackupTask);

            try
            {
                //开始调用函数,可以带参数 
                IAsyncResult ir = sd.BeginInvoke(sPath, NewFileName, null, null);

                Application.DoEvents();

                //循环检测是否完成了异步的操作 
                while (true)
                {
                    if (ir.IsCompleted)
                    {
                        //完成了操作则关闭窗口 
                        break;
                    }
                }

                //取函数的返回值 
                bool isSucceed = sd.EndInvoke( ir);

                if (isSucceed == false)
                {
                }
                else
                {
                }

                return isSucceed;

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private bool BackupTask(string sPath,string NewFileName)
        {
            if (System.IO.File.Exists(NewFileName))
            {
                return false;
            }
            else
            {
                try
                {
                    if (File.Exists(NewFileName))
                    {
                        File.Delete(NewFileName);
                    }
                    System.IO.File.Copy(sPath, NewFileName);
                }
                catch (System.Exception)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
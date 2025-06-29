using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetMiner.Gather;
using NetMiner.Gather.Task;
using NetMiner.Core.gTask;

namespace MinerSpider
{
    public partial class frmAddSominerTask : Form
    {
        public delegate void ReturnTask(string TaskClass,string TaskName,string Field);
        public ReturnTask RTask;

        public frmAddSominerTask()
        {
            InitializeComponent();
        }

        private void frmAddSominerTask_Load(object sender, EventArgs e)
        {
            int i;

            oTaskClass xmlTClass = new oTaskClass(Program.getPrjPath());
            TreeNode newNode = new TreeNode();

            for (i = 0; i < xmlTClass.TaskClasses.Count; i++)
            {
                newNode = new TreeNode();
                newNode.Tag = xmlTClass.TaskClasses[i].tPath;
                newNode.Name = "C" + xmlTClass.TaskClasses[i].ID;
                newNode.Text = xmlTClass.TaskClasses[i].Name;
                newNode.ImageIndex = 15;
                newNode.SelectedImageIndex = 15;

                if (xmlTClass.TaskClasses[i].Children != null && xmlTClass.TaskClasses[i].Children.Count > 0)
                {
                    //加载子分类
                    LoadTreeClass(newNode, xmlTClass.TaskClasses[i].Children);
                }

                this.treeTClass.Nodes.Add(newNode);
                newNode = null;
            }
            xmlTClass = null;
            

           

        }
        private void LoadTreeClass(TreeNode tNode, List<NetMiner.Core.gTask.Entity. eTaskClass> ets)
        {
            for (int i = 0; i < ets.Count; i++)
            {
                TreeNode newNode = new TreeNode();
                newNode.Tag = ets[i].tPath;
                newNode.Name = "C" + ets[i].ID;
                newNode.Text = ets[i].Name;
                newNode.ImageIndex = 15;
                newNode.SelectedImageIndex = 15;

                if (ets[i].Children != null && ets[i].Children.Count > 0)
                {
                    //加载子分类
                    LoadTreeClass(newNode, ets[i].Children);
                }

                tNode.Nodes.Add(newNode);
                newNode = null;
            }
        }



        private void comTaskClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            OK();
        }

        private void OK()
        {
            if (this.listTask.SelectedItems.Count <= 0)
                return;

            oTaskClass tc = new oTaskClass(Program.getPrjPath ());
            string tClassID=this.comTaskClass.Tag.ToString ();

            string TaskClass = tc.GetTaskClassNameByID(tClassID);
            tc = null;
            RTask(TaskClass, this.listTask.SelectedItems[0].Text, this.listTask.SelectedItems[0].SubItems[1].Text.ToString());
            this.Close();
        }

        private void listTask_DoubleClick(object sender, EventArgs e)
        {
            OK();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.treeTClass.Visible == true)
                this.treeTClass.Visible = false;
            else
                this.treeTClass.Visible = true;
        }

        private void treeTClass_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.comTaskClass.Text = this.treeTClass.SelectedNode.Text;
            this.comTaskClass.Tag = this.treeTClass.SelectedNode.Name.Substring(1, this.treeTClass.SelectedNode.Name.Length - 1);
            this.treeTClass.Visible = false;

            this.listTask.Items.Clear();

            try
            {

                ListViewItem litem;
                int TaskClassID = 0;


                TaskClassID = int.Parse(this.comTaskClass.Tag.ToString());

                oTaskClass tClass = new oTaskClass(Program.getPrjPath());
                string TaskClassPath = tClass.GetTaskClassPathByID(TaskClassID);
                tClass = null;

                oTaskIndex xmlTasks = new oTaskIndex(Program.getPrjPath());
                IEnumerable<NetMiner.Core.gTask.Entity.eTaskIndex> eIndexs= xmlTasks.GetTaskDataByClass(TaskClassID);

                //开始初始化此分类下的任务
                int count = xmlTasks.GetTaskCount();
                this.listTask.Items.Clear();

                oTask t;

                foreach(NetMiner.Core.gTask.Entity.eTaskIndex et in eIndexs)
                {
                    t = new oTask(Program.getPrjPath());

                    litem = new ListViewItem();
                    litem.Name = "S" + et.ID;
                    litem.Text = et.TaskName;

                    t.LoadTask(TaskClassPath + "\\" + et.TaskName + ".smt");

                    string strRule = "";
                    for (int m = 0; m < t.TaskEntity.WebpageCutFlag.Count; m++)
                    {
                        strRule += t.TaskEntity.WebpageCutFlag[m].Title.ToString() + ",";
                    }

                    t = null;

                    if (strRule != "")
                        strRule = strRule.Substring(0, strRule.Length - 1);

                    litem.SubItems.Add(strRule);

                    litem.ImageIndex = 0;
                    this.listTask.Items.Add(litem);
                    litem = null;
                }
                xmlTasks = null;
            }

            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.Message, "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using NetMiner.Gather.Task;
using NetMiner.Core.gTask.Entity;
using NetMiner.Resource;
using NetMiner.Core.gTask;
using NetMiner;

namespace MinerSpider
{
    public partial class frmSelectTask : Form
    {
        public delegate void ReturnTaskItem(ListView.SelectedListViewItemCollection lItems);

        public ReturnTaskItem RTaskItem;

        private ResourceManager rm;

        public frmSelectTask()
        {
            InitializeComponent();
        }

        private void frmSelectTask_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            //每次点击任务分类要根据任务分类的编号索引任务，但combobox无法保存编号
            //所以采用这种方法
            oTaskClass xmlTClass = new oTaskClass(Program.getPrjPath());
            TreeNode newNode = new TreeNode();

            for (int i = 0; i < xmlTClass.TaskClasses.Count; i++)
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
            this.listTask.Items.Clear();

            try
            {

                ListViewItem litem;
                int TaskClassID = 0;

                TaskClassID = int.Parse(this.comTaskClass.Tag.ToString ());

                oTaskIndex xmlTasks = new oTaskIndex(Program.getPrjPath());
                IEnumerable<NetMiner.Core.gTask.Entity.eTaskIndex> eIndexs= xmlTasks.GetTaskDataByClass(TaskClassID);

                //开始初始化此分类下的任务
                //int count = xmlTasks.GetTaskClassCount();
                this.listTask.Items.Clear();

                foreach(NetMiner.Core.gTask.Entity.eTaskIndex et in eIndexs)
                {
                    litem = new ListViewItem();
                    litem.Name = "S" + et.ID;
                    litem.Text = et.TaskName;
                    litem.SubItems.Add(this.comTaskClass.Tag.ToString ());
                    litem.SubItems.Add(et.TaskType.GetDescription ());
                    litem.SubItems.Add(et.TaskRunType.GetDescription());
                    litem.ImageIndex = 0;
                    this.listTask.Items.Add(litem);
                    litem = null;
                }
                xmlTasks = null;
            }

            catch (System.IO.IOException)
            {
                MessageBox.Show(rm.GetString("Info72"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString("Info73"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
        }

        private void ReturnValue()
        {
            if (this.listTask.Items.Count == 0)
            {
                this.errorProvider1.SetError(this.listTask, rm.GetString("Info83"));
                return;
            }
            //int tClassID = int.Parse(this.listTask.SelectedItems[0].Name.Substring(1, this.listTask.SelectedItems[0].Name.Length - 1).ToString());
            //string tClassName = this.listTask.SelectedItems[0].Text.ToString();
            RTaskItem(this.listTask.SelectedItems);
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            ReturnValue();
            this.Close();
        }

        private void listTask_DoubleClick(object sender, EventArgs e)
        {
            ReturnValue();
            this.Close();
        }

        private void frmSelectTask_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.treeTClass.Visible == true)
                this.treeTClass.Visible = false;
            else
                this.treeTClass.Visible = false;
        }

        private void treeTClass_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.comTaskClass.Text = this.treeTClass.SelectedNode.Text;
            this.comTaskClass.Tag = this.treeTClass.SelectedNode.Name.Substring(1, this.treeTClass.SelectedNode.Name.Length - 1);
            this.treeTClass.Visible = false;
        }

       
    }
}
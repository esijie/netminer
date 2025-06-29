using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Reflection ;
using System.Resources ;
using NetMiner.Gather;
using NetMiner.Gather.Task;
using NetMiner.Core.gTask.Entity;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Core.gTask;

namespace MinerSpider
{
    public partial class frmAddPlanTask : Form
    {

        public delegate void ReturnRunTask(cGlobalParas.RunTaskType rType,string TaskName,string Para);
        public ReturnRunTask RTask;

        private ResourceManager rm;

        public frmAddPlanTask()
        {
            InitializeComponent();
        }

        private void frmAddPlanTask_Load(object sender, EventArgs e)
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

            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));
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


        private void raSoukeyTask_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raSoukeyTask.Checked == true)
            {
                this.groupBox1.Text = rm.GetString("Label16");
                this.panel1.Visible = true;
                this.panel2.Visible = false;
                this.panel3.Visible = false;
            }
        }

        private void raOtherTask_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raOtherTask.Checked == true)
            {
                this.groupBox1.Text = rm.GetString("Label17");
                this.panel1.Visible = false;
                this.panel2.Visible = true;
                this.panel3.Visible = false;
            }
        }

        private void raDataTask_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raDataTask.Checked == true)
            {
                this.groupBox1.Text = rm.GetString("Label18");
                this.panel1.Visible = false;
                this.panel2.Visible = false;
                this.panel3.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            this.openFileDialog1.Title = rm.GetString("Info74");

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "All Files(*.*)|*.*";

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtFileName.Text = this.openFileDialog1.FileName;
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            SelectTask();
        }

        private void SelectTask()
        {
            if (this.raSoukeyTask.Checked == true)
            {
                if (this.listTask .SelectedItems.Count ==0 && this.listTask.Items.Count >0)
                    listTask.Items[0].Selected = true;

                RTask(cGlobalParas.RunTaskType.SoukeyTask, this.listTask.SelectedItems[0].SubItems[1].Text + "\\" + this.listTask.SelectedItems[0].Text, "");
            }
            else if (this.raOtherTask.Checked == true)
            {
                RTask(cGlobalParas.RunTaskType.OtherTask, this.txtFileName.Text, this.txtPara.Text);
            }
            else if (this.raDataTask.Checked == true)
            {
                if (this.listTask.SelectedItems.Count == 0 && this.listTask.Items.Count > 0)
                    listTask.Items[0].Selected = true;

                if (this.raAccessTask.Checked == true)
                {
                    RTask(cGlobalParas.RunTaskType.DataTask,  cGlobalParas.DatabaseType.Access.GetDescription(), this.txtDataSource.Text + "Para=" + this.comTableName.SelectedItem.ToString());
                }
                else if (this.raMSSQLTask.Checked == true)
                {
                    RTask(cGlobalParas.RunTaskType.DataTask, cGlobalParas.DatabaseType.MSSqlServer.GetDescription (), this.txtDataSource.Text + "Para=" + this.comTableName.SelectedItem.ToString());
                }
                else if (this.raMySqlTask.Checked == true)
                {
                    RTask(cGlobalParas.RunTaskType.DataTask, cGlobalParas.DatabaseType.MySql.GetDescription (), this.txtDataSource.Text + "Para=" + this.comTableName.SelectedItem.ToString());
                }
            }

            this.Close();
        }

        private void listTask_DoubleClick(object sender, EventArgs e)
        {
            SelectTask();
        }
       
        private void GetDataSource(string strDataConn)
        {
            this.txtDataSource.Text = strDataConn;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            frmSetData fSD = new frmSetData();

            if (this.raAccessTask.Checked == true)
                fSD.FormState = 0;
            else if (this.raMSSQLTask.Checked == true)
                fSD.FormState = 1;
            else if (this.raMySqlTask.Checked == true)
                fSD.FormState = 2;

            fSD.rDataSource = new frmSetData.ReturnDataSource(GetDataSource);
            fSD.ShowDialog();
            fSD.Dispose();
        }

        private void comTableName_DropDown(object sender, EventArgs e)
        {
            if (this.raAccessTask.Checked == true)
            {
                FillAccessQuery();
            }
            else if (this.raMSSQLTask.Checked == true)
            {
                FillMSSqlQuery();
            }
            else if (this.raMySqlTask.Checked == true)
            {
                FillMySqlQuery();
            }
        }

        private void FillAccessQuery()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtDataSource.Text);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info75") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] Restrictions = new string[4];
            Restrictions[3] = "VIEW";

            DataTable tb = conn.GetSchema("Tables",Restrictions );

            foreach (DataRow r in tb.Rows)
            {
                 this.comTableName.Items.Add(r[2].ToString());
            }

        }

        private void FillMSSqlQuery()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon ( this.txtDataSource.Text);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info75") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Procedures");

            foreach (DataRow r in tb.Rows)
            {

                this.comTableName.Items.Add(r[5].ToString());

            }
        }

        private void FillMySqlQuery()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtDataSource.Text);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info75") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Procedures");

            foreach (DataRow r in tb.Rows)
            {

                this.comTableName.Items.Add(r[3].ToString());

            }
        }

        private void frmAddPlanTask_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void button7_Click(object sender, EventArgs e)
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

            //开始加载采集任务
            this.listTask.Items.Clear();

            try
            {

                ListViewItem litem;
                int TaskClassID = 0;


                TaskClassID = int.Parse(this.comTaskClass.Tag.ToString());

                oTaskIndex xmlTasks = new oTaskIndex(Program.getPrjPath());
                IEnumerable<NetMiner.Core.gTask.Entity.eTaskIndex> eIndex= xmlTasks.GetTaskDataByClass(TaskClassID);

                //开始初始化此分类下的任务
                //int count = xmlTasks.GetTaskClassCount();
                this.listTask.Items.Clear();

                oTaskClass tc = new oTaskClass(Program.getPrjPath());

                foreach(NetMiner.Core.gTask.Entity.eTaskIndex et in eIndex)
                {
                    litem = new ListViewItem();
                    litem.Name = "S" + et.ID;
                    litem.Text = et.TaskName;
                    litem.SubItems.Add(tc.GetTaskClassNameByID(this.comTaskClass.Tag.ToString()));
                    litem.SubItems.Add(et.TaskType.GetDescription());
                    litem.SubItems.Add(et.TaskRunType.GetDescription());
                    litem.ImageIndex = 0;
                    this.listTask.Items.Add(litem);
                    litem = null;
                }

                tc = null;

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
    }
}
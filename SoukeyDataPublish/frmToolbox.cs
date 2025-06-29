using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Core;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;
using System.Collections.Generic;

namespace SoukeyDataPublish
{
    public partial class frmToolbox : DockContent
    {
        public TreeNode SelectNode
        {
            get { return this.treeMenu.SelectedNode; }
        }

        public frmToolbox()
        {
            InitializeComponent();

            this.DockAreas = DockAreas.DockLeft;
        }

        private void frmToolbox_Load(object sender, EventArgs e)
        {
            IniData();
        }

        private void IniData()
        {
            //加载已完成的数据信息
            oTaskComplete t = new oTaskComplete(Program.getPrjPath());
            IEnumerable<eTaskCompleted> ecs= t.LoadTaskData();

            TreeNode newNode;

            foreach(eTaskCompleted ec in ecs)
            {
                newNode = new TreeNode();
                newNode.Tag = ec.TempFile;
                newNode.Name = "C" + ec.TaskID;
                newNode.Text = ec.TaskName;
                newNode.ImageIndex = 5;
                newNode.SelectedImageIndex = 5;
                this.treeMenu.Nodes["SoukeyData"].Nodes.Add(newNode);
                newNode = null; ;
            }

            

            //加载数据库信息
            #region 加载雷达数据连接信息
            cGlobalParas.VersionType rVersion= ToolUtil.GetRegisterVersion (Program.getPrjPath());
            if (rVersion == cGlobalParas.VersionType.Cloud ||
                rVersion == cGlobalParas.VersionType.Ultimate ||
                rVersion == cGlobalParas.VersionType.Enterprise)
            {
                cXmlSConfig sc = new cXmlSConfig(Program.getPrjPath());

                cGlobalParas.DatabaseType dtype = (cGlobalParas.DatabaseType)sc.DataType;
                string conn = ToolUtil.DecodingDBCon(sc.DataConnection);
                GetDataSource(conn, dtype, false);

                sc = null;
            }

            #endregion


            this.treeMenu.ExpandAll();

            this.treeMenu.SelectedNode = this.treeMenu.Nodes[0];

        }

        private void GetDataSource(string strCon, cGlobalParas.DatabaseType dType, bool IsInsert)
        {
            if (strCon == "")
                return;

            TreeNode newNode = new TreeNode();

            Match charSetMatch;


            switch (dType)
            {
                case cGlobalParas.DatabaseType.MSSqlServer:

                    string dbserver = "";

                    //获取服务器地址
                    charSetMatch = Regex.Match(strCon, "(?<=source=).+?(?=;)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    newNode.Name = "MSSql-" + charSetMatch.Groups[0].ToString();
                    newNode.Text = "MSSql(" + charSetMatch.Groups[0].ToString() + ")";
                    dbserver = charSetMatch.Groups[0].ToString();

                    newNode.Tag = strCon;
                    newNode.ImageIndex = 8;
                    newNode.SelectedImageIndex = 8;

                    //获取数据库名称
                    TreeNode dbNode = new TreeNode();
                    charSetMatch = Regex.Match(strCon, "(?<=catalog=).+?(?=;)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    dbNode.Name = "MSSqlDB-" + charSetMatch.Groups[0].ToString();
                    dbNode.Text = dbserver + "-" + charSetMatch.Groups[0].ToString();
                    dbNode.Tag = strCon;
                    dbNode.ImageIndex = 3;
                    dbNode.SelectedImageIndex = 3;

                    newNode.Nodes.Add(dbNode);

                    this.treeMenu.Nodes["nodRadar"].Nodes.Add(newNode);

                    LoadMSSqlTable(dbNode, strCon);

                    break;
                case cGlobalParas.DatabaseType.MySql:
                    charSetMatch = Regex.Match(strCon, "(?<=source=).+?(?=;)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    newNode.Name = "MySql-" + charSetMatch.Groups[0].ToString();
                    newNode.Text = "MySql(" + charSetMatch.Groups[0].ToString() + ")";
                    newNode.Tag = strCon;
                    newNode.ImageIndex = 9;
                    newNode.SelectedImageIndex = 9;

                    this.treeMenu.Nodes["nodRadar"].Nodes.Add(newNode);

                    LoadMySqlTable(newNode, strCon);
                    
                    break;

            }

        }

        private void LoadAccessTable(TreeNode pNode, string strCon)
        {

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = strCon;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(rm.GetString("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");

            TreeNode newNode;

            foreach (DataRow r in tb.Rows)
            {
                if (r[3].ToString() == "TABLE")
                {
                    newNode = new TreeNode();

                    newNode.Name ="RTable-" + r[2].ToString();
                    newNode.Text =r[2].ToString();
                    newNode.ImageIndex = 11;
                    newNode.SelectedImageIndex = 11;
                    pNode.Nodes.Add(newNode);

                }

            }

            conn.Close();
            conn.Dispose();

        }

        private void LoadMSSqlTable(TreeNode pNode, string strCon)
        {


            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = strCon;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(rm.GetString("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");

            TreeNode newNode;

            foreach (DataRow r in tb.Rows)
            {

                newNode = new TreeNode();

                newNode.Name ="RTable-" + r[2].ToString();
                newNode.Text = r[2].ToString();
                newNode.ImageIndex = 11;
                newNode.SelectedImageIndex = 11;
                pNode.Nodes.Add(newNode);

            }

            conn.Close();
            conn.Dispose();
        }

        private void LoadMySqlTable(TreeNode pNode, string strCon)
        {

            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon(strCon);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(rm.GetString("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");
            TreeNode newNode;

            foreach (DataRow r in tb.Rows)
            {

                newNode = new TreeNode();

                newNode.Name = r[2].ToString();
                newNode.Text = r[2].ToString();
                newNode.ImageIndex = 14;
                newNode.SelectedImageIndex = 14;
                pNode.Nodes.Add(newNode);

            }

            conn.Close();
            conn.Dispose();
        }

        /// <summary>
        /// 增加一个节点，外部调用
        /// </summary>
        /// <param name="pName">父节点名称</param>
        /// <param name="m_node">需要增加的节点</param>
        public void AddNode(string pName, TreeNode m_node)
        {
            switch (pName )
            {
                case "Access":
                    this.treeMenu.Nodes["OtherData"].Nodes["AccessData"].Nodes.Add(m_node);
                    break;
                case "SqlServer":
                    this.treeMenu.Nodes["OtherData"].Nodes["SqlServerData"].Nodes.Add(m_node);
                    break ;
                case "MySql":
                    this.treeMenu.Nodes["OtherData"].Nodes["MySqlData"].Nodes.Add(m_node);
                    break;
                default :
                    break;
             }
        }

        #region 事件
        private readonly Object m_eventLock = new Object();

        private event EventHandler<LoadDataEventArgs> e_LoadDataEvent;
        public event EventHandler<LoadDataEventArgs> LoadDataEvent
        {
            add { lock (m_eventLock) { e_LoadDataEvent += value; } }
            remove { lock (m_eventLock) { e_LoadDataEvent -= value; } }
        }

        #endregion

        private void treeMenu_DoubleClick(object sender, EventArgs e)
        {
            if (this.treeMenu.SelectedNode.Name != "SoukeyData" && this.treeMenu.SelectedNode.Name != "OtherData")
                e_LoadDataEvent(this, new LoadDataEventArgs(this.treeMenu.SelectedNode));
        }

        private void rmenuDelData_Click(object sender, EventArgs e)
        {
            if (this.treeMenu.SelectedNode == null)
                return;

            string sName = this.treeMenu.SelectedNode.Name;
            Int64 TaskID = Int64.Parse(sName.Substring (1,sName.Length -1));
            string TaskName = this.treeMenu.SelectedNode.Text;

            //删除taskcomplete节点
            oTaskComplete tc = new oTaskComplete(Program.getPrjPath());
            tc.LoadTaskData();
            tc.DelTask(TaskID);
            tc = null;

            //删除run中的任务实例文件
            string FileName = Program.getPrjPath() + "data\\" + TaskName + "-" + TaskID + ".xml";
            System.IO.File.Delete(FileName);

            this.treeMenu.Nodes["SoukeyData"].Nodes.Remove(this.treeMenu.SelectedNode);

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.treeMenu.SelectedNode == null)
            {
                this.rmenuDelData.Enabled = false;
                return;
            }

            if (this.treeMenu.SelectedNode.Parent == null)
            {
                this.rmenuDelData.Enabled = false;
                return;
            }

            if (this.treeMenu.SelectedNode.Parent.Name == "SoukeyData")
                this.rmenuDelData.Enabled = true;
            else
                this.rmenuDelData.Enabled = false;

        }

        private void rmenuReferData_Click(object sender, EventArgs e)
        {
            this.treeMenu.Nodes["SoukeyData"].Nodes.Clear();

            //加载已完成的数据信息
            oTaskComplete t = new oTaskComplete(Program.getPrjPath());
            IEnumerable<eTaskCompleted> ecs= t.LoadTaskData();

            TreeNode newNode;

            foreach(eTaskCompleted ec in ecs)
            {
                newNode = new TreeNode();
                newNode.Tag = ec.TempFile;
                newNode.Name = "C" + ec.TaskID;
                newNode.Text = ec.TaskName;
                newNode.ImageIndex = 5;
                newNode.SelectedImageIndex = 5;
                this.treeMenu.Nodes["SoukeyData"].Nodes.Add(newNode);
                newNode = null; ;
            }
        }

     
    }
}
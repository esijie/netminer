using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using NetMiner.Gather.Task;
using NetMiner.Core.gTask.Entity;
using System.Reflection;
using System.Resources;
using NetMiner.Core.Log;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Core.gTask;

namespace MinerSpider
{
    public partial class frmTreeMenu : DockContent 
    {
        private ResourceManager rm;
        private TreeNode Old_SelectedNode;
        private TreeNode m_SelectedNode;

        private ToolTip tTip;

        //�Ƿ񱣴�ϵͳ��־���,Ĭ�ϲ�����
        private bool m_IsAutoSaveLog = false;

        //��¼�������������־����
        private int m_MaxLogNumber;

        //����һ��ȫ�ֱ��������ڴ洢��Ҫ�޸���������ƻ�ƻ�������
        private string OldName = "";

        public bool IsAutoSaveLog
        {
            get { return m_IsAutoSaveLog; }
            set { m_IsAutoSaveLog = value; }
        }

        public int MaxLogNumber
        {
            get { return m_MaxLogNumber; }
            set { m_MaxLogNumber = value; }
        }

        public frmTreeMenu()
        {
            InitializeComponent();

            this.DockAreas = DockAreas.DockLeft;

            //������־��ʾ
            this.splitContainer3.Panel2Collapsed = true;

            //������������ʾ����Դ�ļ�
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));
        }

        public TreeNode SelectedNode
        {
            get { return m_SelectedNode; }
            set 
            { 
                m_SelectedNode = value;
                this.treeMenu.SelectedNode = m_SelectedNode;
            }
        }

        public void IniData()
        {

            //����ϵͳ����ʱ�䵽��־
            ExportLog(cGlobalParas.LogType.Info ,cGlobalParas.LogClass.System, DateTime.Now.ToString () , rm.GetString("InfoStart"));

            //����tooltip��Ϣ
            SetToolTip();

            TreeNode newNode;

            //�ж��Ƿ�Ϊ�������汾������ǣ������Զ��Ŀ¼
            if (Program.SominerVersion==cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                newNode = new TreeNode();
                newNode.Tag = Program.g_RemoteTaskPath;
                newNode.Name = "nodRemoteTaskClass";
                newNode.Text = Program.g_RemoteTaskClass;
                newNode.ImageIndex = 28;
                newNode.SelectedImageIndex = 28;
                //this.treeMenu.SelectedNode = newNode;
                this.treeMenu.Nodes["nodTaskClass"].Nodes.Add(newNode);
                newNode = null;

                IniRemoteData();


                //ÿ������ʱ����Ҫ��Զ������ִ�е��������һ���ж�
                //����ɼ������м�¼�Ļ����Լ������У����Զ�������
                //������ǣ���ɾ��
                if (Program.IsConnectRemote == cGlobalParas.RegResult.Succeed)
                {
                    //��ʼ��Զ�������������
                    oTaskIndex xmlTasks = new oTaskIndex(Program.getPrjPath());
                    IEnumerable< NetMiner.Core.gTask.Entity.eTaskIndex> eIndexs= xmlTasks.GetTaskDataByClass(NetMiner.Constant.g_RemoteTaskClass);
                    //int tCount = xmlTasks.GetTaskClassCount();

                    foreach(NetMiner.Core.gTask.Entity.eTaskIndex et in eIndexs)
                    {
                        //localhost.NetMinerWebService sweb = new localhost.NetMinerWebService();
                        //sweb.Url = Program.ConnectServer + "/NetMiner.WebService.asmx";
                        //if (Program.g_IsAuthen == true)
                        //    sweb.Credentials = new System.Net.NetworkCredential(Program.g_WindowsUser, Program.g_WindowsPwd);

                        //�����������������Զ�����񣬰���ֹͣ�ļ�����δֹͣ������û�ǿ���˳�����������δֹͣ��
                        oTaskRun tr = new oTaskRun(Program.getPrjPath());
                        IEnumerable<eTaskRun> ers= tr.LoadTaskRunData();
                        foreach(eTaskRun er in ers)
                        {
                            if (er.TaskName == et.TaskName)
                            {
                                string rTaskID = er.TaskID.ToString();
                                //ɾ��taskrun�ڵ�
                                tr.DelTask(long.Parse(rTaskID));

                                //ɾ��run�е�����ʵ���ļ�
                                string FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + rTaskID + ".rst";
                                System.IO.File.Delete(FileName);

                                //ɾ��run�е�����ʵ�����ؿ��ļ�
                                FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + rTaskID + ".db";
                                System.IO.File.Delete(FileName);

                                //ɾ��run�еĲɼ����������ļ�
                                FileName = Program.getPrjPath() + "Tasks\\run\\" + "data" + rTaskID + ".db";
                                System.IO.File.Delete(FileName);

                                //ɾ��run�е�����ʵ���ļ�
                                FileName = Program.getPrjPath() + "data\\" + er.TaskName + "-" + rTaskID + ".xml";
                                System.IO.File.Delete(FileName);
                            }

                        }
                        tr = null;

                        //bool isExist = sweb.CheckSubTask(Program.RegisterUser, et.TaskName);
                        ////��������Ѿ��ڷ������в����ڣ���������Ҳ����ɾ��
                        //if (isExist == false)
                        //{
                        //    //ɾ����������ɾ��������������
                        //    oTask t = new oTask(Program.getPrjPath());
                        //    oTaskClass tc = new oTaskClass(Program.getPrjPath());
                        //    string tPath = Program.getPrjPath() + tc.GetTaskClassPathByName(Program.g_RemoteTaskClass);
                        //    tc = null;
                        //    t.DeleTask(tPath, et.TaskName);
                        //    t = null;
                        //}
                    }
                }

                if (Program.SominerVersion==cGlobalParas.VersionType.Cloud)
                    //��������˰棬Ҳ�Ƴ��˵�
                    this.treeMenu.Nodes["nodRemote"].Remove();
            }
            else
            {
                //��ͨ�汾�����Ƴ�Զ�̵Ĳ˵�
                this.treeMenu.Nodes["nodRemote"].Remove();
            }

            try
            {
                //��ʼ��ʼ�����νṹ,ȡxml�е�����,��ȡ�������
                oTaskClass xmlTClass = new oTaskClass(Program.getPrjPath());
                for (int i = 0; i < xmlTClass.TaskClasses.Count;i++ )
                {
                    newNode = new TreeNode();
                    newNode.Tag =xmlTClass.TaskClasses[i].tPath;
                    newNode.Name = "C" + xmlTClass.TaskClasses[i].ID;
                    newNode.Text = xmlTClass.TaskClasses[i].Name;
                    newNode.ImageIndex = 15;
                    newNode.SelectedImageIndex = 15;

                    if (xmlTClass.TaskClasses[i].Children != null && xmlTClass.TaskClasses[i].Children.Count >0)
                    {
                        //�����ӷ���
                        LoadTreeClass(newNode, xmlTClass.TaskClasses[i].Children);
                    }

                    this.treeMenu.Nodes["nodTaskClass"].Nodes.Add(newNode);
                    newNode = null;
                }

                    //int TClassCount = xmlTClass.GetTaskClassCount();

                    //for (int i = 0; i < TClassCount; i++)
                    //{
                    //    newNode = new TreeNode();
                    //    newNode.Tag = xmlTClass.GetTaskClassPathByID(xmlTClass.GetTaskClassID(i));
                    //    newNode.Name = "C" + xmlTClass.GetTaskClassID(i);
                    //    newNode.Text = xmlTClass.GetTaskClassName(i);
                    //    newNode.ImageIndex = 15;
                    //    newNode.SelectedImageIndex = 15;
                    //    //this.treeMenu.SelectedNode = newNode;
                    //    this.treeMenu.Nodes["nodTaskClass"].Nodes.Add(newNode);
                    //    newNode = null;
                    //}
                    xmlTClass = null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Error14"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //��������ڵ㸳·��ֵ
            this.treeMenu.Nodes["nodTaskClass"].Tag = "tasks";

            //����Ĭ��ѡ������νṹ�ڵ�Ϊ���ɼ��������
            TreeNode SelectNode = new TreeNode();
            SelectNode = this.treeMenu.Nodes["nodTaskClass"];
            this.treeMenu.SelectedNode = SelectNode;
            this.m_SelectedNode = SelectNode;
            SelectNode = null;


            //����ɾ����Ϊ���νṹ
            if (e_DelInfo != null)
                e_DelInfo(this, new DelInfoEvent(this.treeMenu.Name));

            //�����Խṹչ��
            //this.treeMenu.Nodes["nodSnap"].Expand();
            //this.treeMenu.Nodes["nodAdvance"].Expand();
            this.treeMenu.Nodes["nodTaskClass"].Expand();
            if (this.treeMenu.Nodes.Contains (this.treeMenu.Nodes["nodRemote"]))
                this.treeMenu.Nodes["nodRemote"].Expand();

            this.treeMenu.Nodes["nodPublishTemplate"].Expand();

            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion  == cGlobalParas.VersionType.Enterprise ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate)
            {
                //this.treeMenu.Nodes["nodRadarRule"]. = true;
            }
            else
            {
                //ɾ���״�Ľڵ�
                //this.treeMenu.Nodes["nodAdvance"].Nodes["nodRadarRule"].Remove();
            }

        }

        //�����Ӳɼ�����
        private void LoadTreeClass(TreeNode tNode,List<NetMiner.Core.gTask.Entity. eTaskClass > ets)
        {
            for (int i=0;i<ets.Count ;i++)
            {
                TreeNode newNode = new TreeNode();
                newNode.Tag = ets[i].tPath;
                newNode.Name = "C" + ets[i].ID;
                newNode.Text = ets[i].Name;
                newNode.ImageIndex = 15;
                newNode.SelectedImageIndex = 15;

                if (ets[i].Children != null && ets[i].Children.Count > 0)
                {
                    //�����ӷ���
                    LoadTreeClass(newNode, ets[i].Children);
                }

                tNode.Nodes.Add(newNode);
                newNode = null;
            }
        }

        /// <summary>
        /// ��ʼ��Զ�̷��������Ӳ���
        /// </summary>
        private void IniRemoteData()
        {

            if (Program.ConnectServer == "")
                return;

            //����Զ�̷�����
            //localhost.NetMinerWebService sweb = new localhost.NetMinerWebService();
            //sweb.Url = Program.ConnectServer + "/NetMinerWebService.asmx";

            //if (Program.g_IsAuthen == true)
            //    sweb.Credentials = new System.Net.NetworkCredential(Program.g_WindowsUser, Program.g_WindowsPwd);


            //if (Program.g_IsAuthen == true)
            //    sweb.Credentials = new System.Net.NetworkCredential(Program.g_WindowsUser, Program.g_WindowsPwd);

            int cResult;
            //try
            //{
            //    cResult = sweb.ConnectServer(Program.RegisterUser);
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show("Զ�̷����������޷����ӣ�", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //switch ((cGlobalParas.RegResult)cResult)
            //{
            //    case cGlobalParas.RegResult.Succeed:
            //        this.treeMenu.Nodes["nodRemote"].Text = "Զ�̷�����(" + Program.ConnectServer + ")";
            //        if (e_SetControlProperty != null)
            //        {
            //            e_SetControlProperty(this, new SetControlPropertyEvent("toolmenuRegRemote", "Enabled", "false"));
            //            e_SetControlProperty(this, new SetControlPropertyEvent("toolManageRemoteServer", "Enabled", "false"));

            //        }
            //        Program.IsConnectRemote = cGlobalParas.RegResult.Succeed;
            //        break;
            //    case cGlobalParas.RegResult.UnReg:
            //        this.treeMenu.Nodes["nodRemote"].Text = "Զ�̷�����(δע��)";
            //        if (e_SetControlProperty != null)
            //        {
            //            e_SetControlProperty(this, new SetControlPropertyEvent("toolmenuRegRemote", "Enabled", "true"));
            //            e_SetControlProperty(this, new SetControlPropertyEvent("toolManageRemoteServer", "Enabled", "false"));
            //        }
            //        Program.IsConnectRemote = cGlobalParas.RegResult.UnReg;
            //        break;
            //    default:
            //        this.treeMenu.Nodes["nodRemote"].Text = "Զ�̷�����(δ����)";
            //        Program.IsConnectRemote = cGlobalParas.RegResult.Faild;
            //        break;
            //}

            //�ж�Զ�̷������Ƿ�������������״̬�޸���ҳ��İ�ť
            //int sState;
            //try
            //{
            //    sState = sweb.GetServerState();
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show("Զ�̷����������޷����ӣ�", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            //switch (sState)
            //{
            //    case (int)cGlobalParas.ServerState.Running:
            //        if (e_SetControlProperty != null)
            //        {
            //            e_SetControlProperty(this, new SetControlPropertyEvent("toolMenuStartServer", "Enabled", "false"));
            //            e_SetControlProperty(this, new SetControlPropertyEvent("toolMenuStopServer", "Enabled", "true"));
            //        }
            //        break;
            //    case (int)cGlobalParas.ServerState.Stopped:
            //        if (e_SetControlProperty != null)
            //        {
            //            e_SetControlProperty(this, new SetControlPropertyEvent("toolMenuStartServer", "Enabled", "true"));
            //            e_SetControlProperty(this, new SetControlPropertyEvent("toolMenuStopServer", "Enabled", "false"));
            //        }
            //        break;
            //    case (int)cGlobalParas.ServerState.UnSetup:
            //        if (e_SetControlProperty != null)
            //        {
            //            e_SetControlProperty(this, new SetControlPropertyEvent("toolMenuStartServer", "Enabled", "false"));
            //            e_SetControlProperty(this, new SetControlPropertyEvent("toolMenuStopServer", "Enabled", "false"));
            //        }
            //        break;
            //}

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

                    //��ȡ��������ַ
                    charSetMatch = Regex.Match(strCon, "(?<=source=).+?(?=;)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    newNode.Name = "MSSql-" + charSetMatch.Groups[0].ToString();
                    newNode.Text = "MSSql(" + charSetMatch.Groups[0].ToString() + ")";
                    dbserver = charSetMatch.Groups[0].ToString();

                    newNode.Tag = strCon;
                    newNode.ImageIndex = 22;
                    newNode.SelectedImageIndex = 22;

                    //��ȡ���ݿ�����
                    TreeNode dbNode = new TreeNode();
                    charSetMatch = Regex.Match(strCon, "(?<=catalog=).+?(?=;)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    dbNode.Name = "MSSqlDB-" + charSetMatch.Groups[0].ToString();
                    dbNode.Text = dbserver + "-" + charSetMatch.Groups[0].ToString();
                    dbNode.Tag = strCon;
                    dbNode.ImageIndex = 20;
                    dbNode.SelectedImageIndex = 20;

                    newNode.Nodes.Add(dbNode);

                    this.treeMenu.Nodes["nodRadar"].Nodes["nodRadarData"].Nodes.Add(newNode);

                    LoadMSSqlTable(dbNode, strCon);

                    break;
                case cGlobalParas.DatabaseType.MySql:
                    charSetMatch = Regex.Match(strCon, "(?<=source=).+?(?=;)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    newNode.Name = "MySql-" + charSetMatch.Groups[0].ToString();
                    newNode.Text = "MySql(" + charSetMatch.Groups[0].ToString() + ")";
                    newNode.Tag = strCon;
                    newNode.ImageIndex = 29;
                    newNode.SelectedImageIndex = 29;

                    this.treeMenu.Nodes["nodRadar"].Nodes["nodRadarData"].Nodes.Add(newNode);

                    LoadMySqlTable(newNode, strCon);
                    
                    break;
              
            }

        }


        private void LoadAccessTable(TreeNode pNode, string strCon)
        {

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (strCon);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                ExportLog(cGlobalParas.LogType.Error ,cGlobalParas.LogClass.Data,System.DateTime.Now.ToString (), rm.GetString("Error12") + ex.Message);
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

                    newNode.Name = r[2].ToString();
                    newNode.Text = r[2].ToString();
                    newNode.ImageIndex = 14;
                    newNode.SelectedImageIndex = 14;
                    pNode.Nodes.Add(newNode);

                }

            }

            conn.Close();
            conn.Dispose();

        }

        private void LoadMSSqlTable(TreeNode pNode, string strCon)
        {


            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (strCon);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                ExportLog(cGlobalParas.LogType.Error ,cGlobalParas.LogClass.Data,System.DateTime.Now.ToString (), rm.GetString("Error12") + ex.Message);
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

        private void LoadMySqlTable(TreeNode pNode, string strCon)
        {

            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (strCon);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void treeMenu_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (this.treeMenu.SelectedNode.Name.Substring(0, 1) != "C" || e.Label == "" || e.Label == null)
            {
                e.CancelEdit = true;
                return;
            }

            //����һ���޸ķ������Ƶ�ί��
            delegateRenameTaskClass sd = new delegateRenameTaskClass(this.RenameTaskClass);

            //��ʼ���ú���,���Դ����� 
            IAsyncResult ir = sd.BeginInvoke(GetTaskFullClassName( e.Node), e.Node.Tag.ToString (), e.Label, null, null);

            //��ʾ�ȴ��Ĵ��� 
            frmWaiting fWait = new frmWaiting(rm.GetString("Info62"));
            fWait.Text = rm.GetString("Info62");

            fWait.Show(this);
            


            //ѭ������Ƿ�������첽�Ĳ��� 
            while (true)
            {
                if (ir.IsCompleted)
                {
                    //����˲�����رմ��� 
                    fWait.Close();
                    break;
                }
            }

            //ȡ�����ķ���ֵ 
            bool retValue = sd.EndInvoke(ir);

            if (retValue == false)
                e.CancelEdit = true;
        }

        //�޸�����������ƵĴ��� 
        private delegate bool delegateRenameTaskClass(string OldName,string oldPath, string NewName);
        private bool RenameTaskClass(string OldName, string oldPath, string NewName)
        {
            cTaskManage tManage = new cTaskManage(Program.getPrjPath());

            try
            {
                tManage.RenameTaskClass(OldName,oldPath, NewName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info63") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                tManage = null;
            }

            return true;
        }


        private void treeMenu_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataGridViewSelectedRowCollection)))
            {
                Point p = this.treeMenu.PointToClient(new Point(e.X, e.Y));
                TreeViewHitTestInfo index = this.treeMenu.HitTest(p);

                if (index.Node != null)
                {
                    if (index.Node.Name.Substring(0, 1) == "C" || index.Node.Name == "nodTaskClass")
                    {
                        DataGridViewSelectedRowCollection drs = (DataGridViewSelectedRowCollection)e.Data.GetData(typeof(DataGridViewSelectedRowCollection));

                        for (int i = 0; i < drs.Count; i++)
                        {
                            DataGridViewRow drv = (DataGridViewRow)drs[i];
                            string TaskName = drv.Cells[5].Value.ToString();

                            //string oldClassName = "";

                            //if (Old_SelectedNode.Name == "nodTaskClass")
                            //    oldClassName = "";
                            //else
                            //    oldClassName = GetTaskFullClassName( Old_SelectedNode);

                            //string NewClassName = "";
                            //if (index.Node.Name == "nodTaskClass")
                            //    NewClassName = "";
                            //else
                            //    NewClassName = GetTaskFullClassName( index.Node);

                            string oldTaskClassPath = Old_SelectedNode.Tag.ToString();
                            string newTaskClassPath = index.Node.Tag.ToString();

                            if (oldTaskClassPath == newTaskClassPath)
                            {
                                this.treeMenu.SelectedNode = Old_SelectedNode;
                                return;
                            }

                            cTaskManage mTask = new cTaskManage(Program.getPrjPath());

                            try
                            {
                                mTask.CopyTask(oldTaskClassPath, newTaskClassPath, TaskName, cGlobalParas.CopyType.Move);
                            }
                            catch (System.Exception ex)
                            {
                                MessageBox.Show(rm.GetString("Info49") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.treeMenu.SelectedNode = Old_SelectedNode;
                                return;
                            }
                            finally
                            {
                                mTask = null;
                            }

                            if (e_DelRow != null)
                                e_DelRow(this, new DelDatagridRowEvent(drv));

                        }

                    }
                    else
                    {
                        MessageBox.Show(rm.GetString("Info50") + index.Node.Text, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                this.treeMenu.SelectedNode = Old_SelectedNode;
            }
        }

        private void treeMenu_DragEnter(object sender, DragEventArgs e)
        {
            Old_SelectedNode = this.treeMenu.SelectedNode;
            this.treeMenu.Focus();
            e.Effect = DragDropEffects.Copy;
        }

        private void treeMenu_DragLeave(object sender, EventArgs e)
        {
            this.treeMenu.SelectedNode = Old_SelectedNode;
        }

        private void treeMenu_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataGridViewSelectedRowCollection)))
            {
                Point p = this.treeMenu.PointToClient(new Point(e.X, e.Y));
                TreeViewHitTestInfo index = this.treeMenu.HitTest(p);

                if (index.Node != null)
                {
                    if (index.Node.Name.Substring(0, 1) == "C" || index.Node.Name == "nodTaskClass")
                    {
                        e.Effect = DragDropEffects.Copy;
                        this.treeMenu.SelectedNode = index.Node;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }
            }
        }

        private void treeMenu_Enter(object sender, EventArgs e)
        {
            if (e_DelInfo != null)
                e_DelInfo(this, new DelInfoEvent(this.treeMenu.Name));
        }

        private void treeMenu_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    try
                    {
                        DelTaskClass();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }
                    break;
                case Keys.F2:
                    if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C")
                        this.treeMenu.SelectedNode.BeginEdit();

                    break;

            }
        }

        private void treeMenu_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //���õ�ǰѡ�еĽڵ�
                this.m_SelectedNode = e.Node;

                if (e_TreeNodeMouseClick != null)
                    e_TreeNodeMouseClick(this,new TreeNodeMouseClickEvent (e.Node));
            }
        }

        public void DelTaskClass()
        {
            if (this.treeMenu.SelectedNode.Name.Substring(0, 1) != "C")
            {
                if (this.treeMenu.SelectedNode.Name == "nodPlanCompleted")
                {
                    if (e_ExcuteFunction != null)
                        e_ExcuteFunction(this, new ExcuteFunctionEvent("DelPlanLog", null));

                    return;
                }
                else
                {
                    MessageBox.Show(rm.GetString("Info20") + this.treeMenu.SelectedNode.Text, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }


            if (MessageBox.Show(rm.GetString("Info21") + this.treeMenu.SelectedNode.Text + "\r\n" + rm.GetString("Info22"),
               rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    oTaskClass tClass = new oTaskClass(Program.getPrjPath());
                    string tClassID = this.treeMenu.SelectedNode.Name.Substring(1, this.treeMenu.SelectedNode.Name.Length - 1);
                    if (tClass.DelTaskClass(tClassID))
                    {
                        tClass.Dispose();
                        tClass = null;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(rm.GetString("Info23") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }

                this.treeMenu.Nodes.Remove(this.treeMenu.SelectedNode);

                treeMenu_NodeMouseClick(this.treeMenu, new TreeNodeMouseClickEventArgs(this.treeMenu.SelectedNode, MouseButtons.Left, 0, 0, 0));
            }



        }

        private void frmTreeMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm =null;
        }

        public void ExportLog(cGlobalParas.LogType lType, cGlobalParas.LogClass lClass, string DTime, string strLog)
        {
           
                this.txtLog.Text = DTime + " " + strLog + "\r\n" + this.txtLog.Text;

                if (m_IsAutoSaveLog == true)
                {
                    try
                    {
                        cSystemLog sl = new cSystemLog(Program.getPrjPath());
                        sl.WriteLog(lType, lClass, DTime, strLog);
                        sl = null;
                    }
                    catch (System.Exception ex)
                    {
                        this.txtLog.Text = rm.GetString("Info51") + ex.Message + "\r\n" + this.txtLog.Text;
                    }
                }

                int i = this.txtLog.Lines.Length;

                if (i > this.m_MaxLogNumber)
                {
                    this.txtLog.Clear();
                }
            
        }

        //���ô�����ʵ��tooltip��Ϣ
        private void SetToolTip()
        {
            tTip = new ToolTip();
            this.tTip.SetToolTip(this.cmdCloseInfo, rm.GetString("Info52"));
            this.tTip.SetToolTip(this.treeMenu, rm.GetString("Info53"));
            //this.
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C")
            {
                //this.rmmenuStopTask.Enabled = false;
                this.rmenuAddTaskClass.Enabled = true;
                this.rmenuDelTaskClass.Enabled = true;

                this.rmenuRenameTaskClass.Enabled = true;
            }
            else if (this.treeMenu.SelectedNode.Name == "nodRadar" || this.treeMenu.SelectedNode.Name =="nodRadarRule")
            {
               this.menuRadarData.Enabled = true;
            }
            else
            {
                //this.rmmenuStopTask.Enabled = true;
                this.rmenuAddTaskClass.Enabled = true;
                this.rmenuDelTaskClass.Enabled = false;

                this.rmenuRenameTaskClass.Enabled = false;
            }
        
        }

        private void rmenuAddTaskClass_Click(object sender, EventArgs e)
        {
            NewTaskClass();
        }

        private void rmenuRenameTaskClass_Click(object sender, EventArgs e)
        {
            if (this.treeMenu.SelectedNode.Name.Substring(0, 1) == "C")
                this.treeMenu.SelectedNode.BeginEdit();
        }

        private void rmenuDelTaskClass_Click(object sender, EventArgs e)
        {
            DelTaskClass();
        }

        private void menuAddTask_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("NewTask", null));
        }

        private void menuAddTaskPlan_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("NewPlan", null));
        }

        public void NewTaskClass()
        {
            frmTaskClass frmTClass =null;

            if (!this.treeMenu.SelectedNode.Name .StartsWith ("C"))
            {
                //��һ��Ŀ¼
                frmTClass = new frmTaskClass(0, "");
            }
            else
            {
                string fid = this.treeMenu.SelectedNode.Name.Substring(1, this.treeMenu.SelectedNode.Name.Length - 1);
                frmTClass = new frmTaskClass(int.Parse (fid), GetTaskFullClassName(this.treeMenu.SelectedNode));
            }
            frmTClass.RTaskClass = new frmTaskClass.ReturnTaskClass(AddTaskClassNode);
            frmTClass.ShowDialog();
            frmTClass.Dispose();
        }

        //������������󣬸�������ӵ���Ϣ����ʼ�������������νṹ
        private void AddTaskClassNode(int fID, int TaskClassID, string TaskClassName, string TaskClassPath)
        {
            TreeNode newNode = new TreeNode();
            if (fID == 0)
            {
                
                newNode.Tag = TaskClassPath;
                newNode.Name = "C" + TaskClassID;
                newNode.Text = TaskClassName;
                newNode.ImageIndex = 15;
                newNode.SelectedImageIndex = 15;
                this.treeMenu.Nodes["nodTaskClass"].Nodes.Add(newNode);

                this.treeMenu.SelectedNode = newNode;

                treeMenu_NodeMouseClick(this.treeMenu, new TreeNodeMouseClickEventArgs(this.treeMenu.SelectedNode, MouseButtons.Left, 0, 0, 0));
            }
            else
            {
                newNode.Tag = TaskClassPath;
                newNode.Name = "C" + TaskClassID;
                newNode.Text = TaskClassName;
                newNode.ImageIndex = 15;
                newNode.SelectedImageIndex = 15;

                TreeNode tNode = FindtreeNode(this.treeMenu.Nodes["nodTaskClass"], fID);
                tNode.Nodes.Add(newNode);

                this.treeMenu.SelectedNode = newNode;

                treeMenu_NodeMouseClick(this.treeMenu, new TreeNodeMouseClickEventArgs(this.treeMenu.SelectedNode, MouseButtons.Left, 0, 0, 0));
            }

        }

        private TreeNode FindtreeNode(TreeNode pNode , int cID)
        {
            TreeNode tNode = null;

            if (pNode.Nodes.Count >0)
            {
                for (int i=0;i<pNode.Nodes.Count ;i++)
                {
                    if (pNode.Nodes[i].Name == "C" + cID.ToString())
                    {
                        tNode = pNode.Nodes[i];
                        break;
                    }

                    if (pNode.Nodes[i].Nodes.Count >0)
                    {
                        tNode = FindtreeNode(pNode.Nodes[i], cID);
                        if (tNode != null)
                            break;
                    }
                  
                }
            }

            return tNode;
        }


       //��������ڵ�
        public void AddTaskClassNode(TreeNode nNode)
        {
            this.treeMenu.Nodes["nodTaskClass"].Nodes.Add(nNode );
        }

        public void refreshNode(string TClass)
        {
            if (TClass == "")
            {
                TreeNode SelectNode = new TreeNode();
                SelectNode = this.treeMenu.Nodes[1];
                this.treeMenu.SelectedNode = SelectNode;

                //LoadTask(this.treeMenu.Nodes["nodTaskClass"]);
                return;
            }

            foreach (TreeNode a in this.treeMenu.Nodes["nodTaskClass"].Nodes)
            {
                if (a.Text == TClass)
                {
                    this.treeMenu.SelectedNode = a;

                    if (e_TreeNodeMouseClick != null)
                        e_TreeNodeMouseClick(this, new TreeNodeMouseClickEvent(a));

                    break;
                }
            }

        }

        public void ShowLog(bool IsShow)
        {
            if (IsShow == true)
                this.splitContainer3.Panel2Collapsed = true;
            else
                this.splitContainer3.Panel2Collapsed = false;
        }

        #region ������Ҫ������������¼�
        private readonly Object m_eventLock = new Object();

        private event EventHandler<SetControlPropertyEvent> e_SetControlProperty;
        public event EventHandler<SetControlPropertyEvent> SetControlProperty
        {
            add { lock (m_eventLock) { e_SetControlProperty += value; } }
            remove { lock (m_eventLock) { e_SetControlProperty -= value; } }
        }

        private event EventHandler<ExcuteFunctionEvent> e_ExcuteFunction;
        public event EventHandler<ExcuteFunctionEvent> ExcuteFunction
        {
            add { lock (m_eventLock) { e_ExcuteFunction += value; } }
            remove { lock (m_eventLock) { e_ExcuteFunction -= value; } }
        }

        private event EventHandler<TreeNodeMouseClickEvent> e_TreeNodeMouseClick;
        public event EventHandler<TreeNodeMouseClickEvent> TreeNodeMouseClick
        {
            add { lock (m_eventLock) { e_TreeNodeMouseClick += value; } }
            remove { lock (m_eventLock) { e_TreeNodeMouseClick -= value; } }
        }

        private event EventHandler<SetToolbarStateEvent> e_SetToolbarState;
        public event EventHandler<SetToolbarStateEvent> SetToolbarState
        {
            add { lock (m_eventLock) { e_SetToolbarState += value; } }
            remove { lock (m_eventLock) { e_SetToolbarState -= value; } }
        }

        private event EventHandler<DelInfoEvent> e_DelInfo;
        public event EventHandler<DelInfoEvent> DelInfo
        {
            add { lock (m_eventLock) { e_DelInfo += value; } }
            remove { lock (m_eventLock) { e_DelInfo -= value; } }
        }

        private event EventHandler<DelDatagridRowEvent> e_DelRow;
        public event EventHandler<DelDatagridRowEvent> DelRow
        {
            add { lock (m_eventLock) { e_DelRow += value; } }
            remove { lock (m_eventLock) { e_DelRow -= value; } }
        } 

        #endregion

        private void cmdCloseInfo_Click(object sender, EventArgs e)
        {
            ShowLog(true );
            if (e_SetControlProperty != null)
                e_SetControlProperty(this, new SetControlPropertyEvent("toolLookInfo","Checked","false"));
        }

        private void frmTreeMenu_Load(object sender, EventArgs e)
        {

        }

        private void rmenuAddRadarRule_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("NewRadar", null));
        }

        private void menuRadarData_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("SoukeyDataPublish.exe");
        }

        private void treeMenu_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void menuEditTask_Click(object sender, EventArgs e)
        {
            //frmBatchTask f = new frmBatchTask(this.treeMenu.SelectedNode.Name,this.treeMenu.SelectedNode.Text);
            //f.ShowDialog();
            //f.Dispose();
        }


        private string GetTaskFullClassName(TreeNode eNode)
        {
            string cName = string.Empty;

            if (eNode.Parent.Name == "nodTaskClass")
            {
                return eNode.Text;
            }
            else
            {
                cName = GetTaskFullClassName(eNode.Parent) + "/" + eNode.Text;
            }
            return cName;
        }

        private string GetTaskFullClassID(TreeNode eNode)
        {
            string cName = string.Empty;

            if (eNode.Parent.Name == "nodTaskClass")
            {
                return eNode.Name.Substring (1,eNode.Name.Length -1);
            }
            else
            {
                cName = GetTaskFullClassID(eNode.Parent) + "/" + eNode.Name.Substring(1, eNode.Name.Length - 1);
            }
            return cName;
        }
    }
}
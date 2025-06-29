using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NetMiner.Resource;
using System.Linq;

namespace SoukeyLog
{
    public partial class frmMain : Form
    {
        private cGlobalParas.LogExitPara m_LogExit;
        private ListViewColumnSorter lvwColumnSorter;


        public frmMain()
        {
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            this.listLog.ListViewItemSorter = lvwColumnSorter;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        public void IniForm()
        {
            this.treeMenu.ExpandAll();
        }

        private void treeMenu_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                switch (this.treeMenu.SelectedNode.Name)
                {
                    case "nodRoot":
                        this.listLog.Items.Clear();
                        break;
                    case "nodSystemLog":
                        ListSystemLog();
                        break;
                    case "nodRadarLog":
                        ListRadarLog();
                        break;

                    case "nodSilentLog":
                        ListGatherLog();
                        break;
                        
                    default:
                        if (this.treeMenu.SelectedNode.Name.StartsWith("Log"))
                        {
                            delegateLoadSLog(this.treeMenu.SelectedNode.Tag.ToString());
                        }
                        else if (this.treeMenu.SelectedNode.Name.StartsWith("Task"))
                        {
                            delegateLoadGLog(this.treeMenu.SelectedNode.Tag.ToString());
                        }

                        break;

                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("��־�ļ����ƻ����޷����أ���ɾ������־�ļ���" + this.treeMenu.SelectedNode.Tag.ToString () + "�������ԣ�������Ϣ��" + ex.Message,"����",MessageBoxButtons.OK ,MessageBoxIcon.Error );
            }
        }

        #region ���ݲɼ���־
        private void LoadGatherTaskLog()
        {
            if (this.treeMenu.SelectedNode.Nodes.Count == 0)
            {
                //��������
                for (int i = 0; i < this.listLog.Items.Count; i++)
                {
                    TreeNode tNode = new TreeNode();
                    tNode.ImageKey = "log";
                    tNode.SelectedImageKey = "log";
                    tNode.Name = "Task" + this.listLog.Items[i].SubItems[1].Text;
                    tNode.Tag = this.listLog.Items[i].SubItems[3].Text;
                    tNode.Text = this.listLog.Items[i].Text;
                    this.treeMenu.Nodes["nodRoot"].Nodes["nodSilentLog"].Nodes.Add(tNode);
                }
            }

            this.treeMenu.SelectedNode = this.treeMenu.Nodes["nodRoot"].Nodes["nodSilentLog"].Nodes["Task" + this.listLog.SelectedItems[0].SubItems[1].Text];
        
        }

        private void ListGatherLog()
        {
            this.listLog.Columns.Clear();
            this.listLog.Items.Clear();

            this.listLog.Columns.Add("��������", 160);
            this.listLog.Columns.Add("�ļ�", 160);
            this.listLog.Columns.Add("��С", 120);
            this.listLog.Columns.Add("λ��", 200);

            string lPath = Program.getPrjPath() + "log";

            //ϵͳ��־Ϊ8λ�ļ����ģ��ҿ�������ת�����ڵ�
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //�й�Ŀ¼�µ��ļ�   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                //��չ��һ��ΪCSV
                if (fName.StartsWith ("task",StringComparison.CurrentCultureIgnoreCase ) && eName.EndsWith("csv", StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        string tName=fName.Substring (4,fName.Length-4);

                        ListViewItem cItem = new ListViewItem();
                        cItem.ImageKey = "Log";
                        cItem.Text = tName ;
                        cItem.SubItems.Add (fName );
                        System.IO.FileInfo f = new FileInfo(fis2[i2].FullName);
                        Single fSize = (Single)f.Length / 1000;

                        if (fSize > 1000)
                        {
                            fSize = (Single)fSize / 1000;
                            cItem.SubItems.Add(fSize.ToString() + "MB");
                        }
                        else
                            cItem.SubItems.Add(fSize.ToString() + "KB");

                        cItem.SubItems.Add(fPath);
                        this.listLog.Items.Add(cItem);

                    }
                    catch (System.Exception)
                    {

                    }

                }

            }
        }

        private delegate DataTable delegateLoadGatherLog(string FileName);
        private void delegateLoadGLog(string FileName)
        {
            if (!File.Exists(FileName))
            {
                MessageBox.Show ("��־�ļ������ڣ��п����Ѿ���������ƻ���", "��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //����һ���޸ķ������Ƶ�ί��
            this.listLog.Columns.Clear();
            this.listLog.Items.Clear();

            this.listLog.Columns.Add("��������", 100);
            this.listLog.Columns.Add("ʱ��", 120);
            this.listLog.Columns.Add("���", 100);
            this.listLog.Columns.Add("��ϸ", 400);

            delegateLoadGatherLog sd = new delegateLoadGatherLog(this.OpenGatherLog);

            try
            {
                //��ʼ���ú���,���Դ����� 
                IAsyncResult ir = sd.BeginInvoke(FileName, null, null);

                Application.DoEvents();

                //ѭ������Ƿ�������첽�Ĳ��� 
                while (true)
                {
                    if (ir.IsCompleted)
                    {
                        //����˲�����رմ��� 
                        break;
                    }
                }

                //ȡ�����ķ���ֵ 
                DataTable dLog = sd.EndInvoke(ir);

                if (dLog == null)
                    return;

                for (int i = 0; i < dLog.Rows.Count; i++)
                {
                    ListViewItem cItem = new ListViewItem();

                    cItem.Text = dLog.Rows[i][0].ToString();
                    cItem.SubItems.Add(dLog.Rows[i][1].ToString());

                    cGlobalParas.LogType lType = (cGlobalParas.LogType)int.Parse(dLog.Rows[i][2].ToString());
                    switch (lType)
                    {
                        case cGlobalParas.LogType.Info:
                            cItem.ImageKey = "Info";
                            cItem.SubItems.Add ("��Ϣ");
                            break;
                        case cGlobalParas.LogType.Error:
                            cItem.ImageKey = "Error";
                            cItem.SubItems.Add("����");
                            break;
                        case cGlobalParas.LogType.Warning:
                            cItem.ImageKey = "Warning";
                            cItem.SubItems.Add("����");
                            break;
                        case cGlobalParas.LogType.GatherError :
                            cItem.ImageKey = "Error";
                            cItem.SubItems.Add("�ɼ�����");
                            break;
                        case cGlobalParas.LogType.PublishError :
                            cItem.ImageKey = "Error";
                            cItem.SubItems.Add("��������");
                            break;
                        case cGlobalParas.LogType.RunPlanTask:
                            cItem.ImageKey = "Error";
                            cItem.SubItems.Add("���мƻ�");
                            break;
                    }

                    cItem.SubItems.Add(dLog.Rows[i][3].ToString());

                    this.listLog.Items.Add(cItem);
                }

            }
            catch (System.Exception ex)
            {
                throw ex;
            }


        }

        private DataTable OpenGatherLog(string FileName)
        {


          

            var result= from Log in File.ReadAllLines(FileName)
                            where !Log.StartsWith("#")
                            let line = Log.Split(',')
                            select new
                            {
                                TaskName = line[0],
                                DateT = line[1],
                                gType = line[2],
                                Remark = line[3]
                            };

            DataTable db =NetMiner.Common.ToolUtil. CopyToDataTable(result);

            return db;

        }

        #endregion

        #region ��ʾ�״���־��Ϣ

        private void ListRadarLog()
        {
            this.listLog.Columns.Clear();
            this.listLog.Items.Clear();

            this.listLog.Columns.Add("�ļ�", 160);
            this.listLog.Columns.Add("��С", 120);
            this.listLog.Columns.Add("λ��", 200);

            string lPath = Program.getPrjPath() + "log";

            //ϵͳ��־ΪRadar + 8λ�ļ����ģ��ҿ�������ת�����ڵ�
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //�й�Ŀ¼�µ��ļ�   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                //��չ��һ��ΪCSV
                if (fName .StartsWith ("radar",StringComparison.CurrentCultureIgnoreCase ) && fName.Length == 13 && eName.EndsWith("csv", StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        DateTime d = DateTime.Parse(fName.Substring(5, 4) + "-" + fName.Substring(9, 2) + "-" + fName.Substring(11, 2));

                        ListViewItem cItem = new ListViewItem();
                        cItem.ImageKey = "Log";
                        cItem.Text = fName;
                        System.IO.FileInfo f = new FileInfo(fis2[i2].FullName);
                        Single fSize = (Single)f.Length / 1000;

                        if (fSize > 1000)
                        {
                            fSize = (Single)fSize / 1000;
                            cItem.SubItems.Add(fSize.ToString() + "MB");
                        }
                        else
                            cItem.SubItems.Add(fSize.ToString() + "KB");

                        cItem.SubItems.Add(fPath);
                        this.listLog.Items.Add(cItem);

                    }
                    catch (System.Exception)
                    {

                    }

                }

            }

        }

        private void LoadRadarLog()
        {
            if (this.treeMenu.SelectedNode.Nodes.Count == 0)
            {
                //��������
                for (int i = 0; i < this.listLog.Items.Count; i++)
                {
                    TreeNode tNode = new TreeNode();
                    tNode.ImageKey = "log";
                    tNode.SelectedImageKey = "log";
                    tNode.Name = "Log" + this.listLog.Items[i].Text;
                    tNode.Tag = this.listLog.Items[i].SubItems[2].Text;
                    tNode.Text = this.listLog.Items[i].Text.Substring(5, this.listLog.Items[i].Text.Length - 5);
                    this.treeMenu.Nodes["nodRoot"].Nodes["nodRadarLog"].Nodes.Add(tNode);
                }
            }

            this.treeMenu.SelectedNode = this.treeMenu.Nodes["nodRoot"].Nodes["nodRadarLog"].Nodes["Log" + this.listLog.SelectedItems[0].Text];

        }

        #endregion

        private void listLog_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listLog_DoubleClick(object sender, EventArgs e)
        {
            if (this.listLog.SelectedItems.Count <= 0)
                return;

            this.staSta.Text = "���ڼ�����־��Ϣ����ȴ�......";

            switch (this.treeMenu.SelectedNode.Name )
            {
                case "nodSystemLog":
                    LoadSystemLog();
                    break;
                case "nodRadarLog":
                    LoadRadarLog();
                    break;
                case "nodSilentLog":
                    LoadGatherTaskLog();
                    break;
            }

            this.staSta.Text = "��ǰ״̬������";

        }

        #region ����ϵͳ��־��Ϣ  �״���ϸ��־���乲��
        private void LoadSystemLog()
        {
            if (this.treeMenu.SelectedNode.Nodes.Count == 0)
            {
                //��������
                for (int i = 0; i < this.listLog.Items.Count; i++)
                {
                    TreeNode tNode = new TreeNode();
                    tNode.ImageKey = "log";
                    tNode.SelectedImageKey = "log";
                    tNode.Name = "Log" + this.listLog.Items[i].Text;
                    tNode.Tag = this.listLog.Items[i].SubItems[2].Text;
                    tNode.Text = this.listLog.Items[i].Text;
                    this.treeMenu.Nodes["nodRoot"].Nodes["nodSystemLog"].Nodes.Add(tNode);
                }
            }

            this.treeMenu.SelectedNode = this.treeMenu.Nodes["nodRoot"].Nodes["nodSystemLog"].Nodes["Log" + this.listLog.SelectedItems[0].Text];

        }

        private void ListSystemLog()
        {
            this.listLog.Columns.Clear();
            this.listLog.Items.Clear();

            this.listLog.Columns.Add("�ļ�", 160);
            this.listLog.Columns.Add("��С", 120);
            this.listLog.Columns.Add("λ��", 200);

            string lPath = Program.getPrjPath() + "log";

            //ϵͳ��־Ϊ8λ�ļ����ģ��ҿ�������ת�����ڵ�
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //�й�Ŀ¼�µ��ļ�   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                //��չ��һ��ΪCSV
                if (fName.Length == 8 && eName.EndsWith("csv", StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        DateTime d = DateTime.Parse(fName.Substring(0, 4) + "-" + fName.Substring(4, 2) + "-" + fName.Substring(6, 2));

                        ListViewItem cItem = new ListViewItem();
                        cItem.ImageKey = "Log";
                        cItem.Text = fName;
                        System.IO.FileInfo f = new FileInfo(fis2[i2].FullName);
                        Single fSize = (Single)f.Length / 1000;

                        if (fSize > 1000)
                        {
                            fSize = (Single)fSize / 1000;
                            cItem.SubItems.Add(fSize.ToString() + "MB");
                        }
                        else
                            cItem.SubItems.Add(fSize.ToString() + "KB");

                        cItem.SubItems.Add(fPath);
                        this.listLog.Items.Add(cItem);

                    }
                    catch (System.Exception)
                    {

                    }

                }

            }

        }

        private delegate DataTable  delegateLoadSystemLog(string FileName);
        private void delegateLoadSLog(string FileName)
        {
            if (!File.Exists(FileName))
            {
                MessageBox.Show ("��־�ļ������ڣ��п����Ѿ���������ƻ���","��Ϣ",MessageBoxButtons.OK ,MessageBoxIcon.Information );
                return;
            }

            //����һ���޸ķ������Ƶ�ί��
            this.listLog.Columns.Clear();
            this.listLog.Items.Clear();

            this.listLog.Columns.Add("����", 100);
            this.listLog.Columns.Add("ʱ��", 120);
            this.listLog.Columns.Add("��Դ", 100);
            this.listLog.Columns.Add("��ϸ", 400);

            delegateLoadSystemLog sd = new delegateLoadSystemLog(this.OpenSystemLog);

            try
            {
                //��ʼ���ú���,���Դ����� 
                IAsyncResult ir = sd.BeginInvoke(FileName, null, null);

                Application.DoEvents();

                //ѭ������Ƿ�������첽�Ĳ��� 
                while (true)
                {
                    if (ir.IsCompleted)
                    {
                        //����˲�����رմ��� 
                        break;
                    }
                }

                //ȡ�����ķ���ֵ 
                DataTable dLog = sd.EndInvoke(ir);

                if (dLog == null)
                    return;

                for (int i = 0; i < dLog.Rows.Count;i++ )
                {
                    ListViewItem cItem = new ListViewItem();
                    cGlobalParas.LogType lType = (cGlobalParas.LogType)int.Parse( dLog.Rows[i][0].ToString ());
                    switch (lType)
                    {
                        case cGlobalParas.LogType.Info:
                            cItem.ImageKey = "Info";
                            cItem.Text = "��Ϣ";
                            break;
                        case cGlobalParas.LogType.Error:
                            cItem.ImageKey = "Error";
                            cItem.Text = "����";
                            break;
                        case cGlobalParas.LogType.Warning:
                            cItem.ImageKey = "Warning";
                            cItem.Text = "����";
                            break;
                    }

                    cItem.SubItems.Add(dLog.Rows[i][2].ToString());

                    switch ((cGlobalParas.LogClass)int.Parse(dLog.Rows[i][1].ToString()))
                    {
                        case cGlobalParas.LogClass.Data:
                            cItem.SubItems.Add("���ݲ���");
                            break;
                        case cGlobalParas.LogClass.GatherLog:
                            cItem.SubItems.Add("���ݲɼ�");
                            break;
                        case cGlobalParas.LogClass.ListenPlan:
                            cItem.SubItems.Add("�����ƻ�");
                            break;
                        case cGlobalParas.LogClass.PublishLog:
                            cItem.SubItems.Add("���ݷ���");
                            break;
                        case cGlobalParas.LogClass.Radar:
                            cItem.SubItems.Add("����״�");
                            break;
                        case cGlobalParas.LogClass.System:
                            cItem.SubItems.Add("�����");
                            break;
                        case cGlobalParas.LogClass.Task:
                            cItem.SubItems.Add("�ɼ�����");
                            break;
                        case cGlobalParas.LogClass.Trigger:
                            cItem.SubItems.Add("������");
                            break;
                        case cGlobalParas.LogClass.RunTask :
                            cItem.SubItems.Add("ִ������");
                            break;
                    }

                    cItem.SubItems.Add(dLog.Rows[i][3].ToString());

                    this.listLog.Items.Add(cItem);
                }

            }
            catch (System.Exception ex)
            {
                throw ex;
            }

           
        }

        private DataTable OpenSystemLog(string FileName)
        {
          

            string fPath = System.IO.Path.GetDirectoryName(FileName);
            string fName = System.IO.Path.GetFileName(FileName);


            var result = from Log in File.ReadAllLines(FileName)
                     where !Log.StartsWith("#")
                     let line = Log.Split(',')
                     select new
                     {
                         Level = line[0],
                         Class = line[1],
                         DateT = line[2],
                         Remark = line[3]
                     };

            DataTable db = NetMiner.Common.ToolUtil.CopyToDataTable(result);
            return db;

        }

        #endregion

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            frmAbout f = new frmAbout();
            f.ShowDialog();
            f.Dispose();
        }

        private void toolClearLog_Click(object sender, EventArgs e)
        {
            ClearLog();
        }

        #region �����־
        private void ClearLog()
        {
            switch (this.treeMenu.SelectedNode.Name)
            {
                case "nodRoot":

                    break;
                case "nodSystemLog":
                    ClearLog(0);
                    break;
                case "nodRadarLog":
                    ClearLog(1);
                    break;
                case "nodSilentLog":
                    ClearLog(2);
                    break;
                default:
                    if (this.treeMenu.SelectedNode.Name.StartsWith("Log"))
                    {
                        bool isSucceed= ClearFileLog(this.treeMenu.SelectedNode.Tag.ToString());
                        if (isSucceed == false)
                            return;
                    }
                    else if (this.treeMenu.SelectedNode.Name.StartsWith("Task"))
                    {
                        bool isSucceed = ClearFileLog(this.treeMenu.SelectedNode.Tag.ToString());
                        if (isSucceed == false)
                            return;
                    }

                    //ɾ���ڵ�
                    this.treeMenu.SelectedNode.Parent.Nodes.Remove (this.treeMenu.SelectedNode);

                    MessageBox.Show("��־����ɹ���", "��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;
            }

        }

        //logType �����־�����ͣ�0-ϵͳ��־��1-�״���־��2-��Ĭִ����־
        private void ClearLog(int logType)
        {
            frmQuestion fq = new frmQuestion();
            fq.RPara = new frmQuestion.ReturnPara(GetLogBackup);
            fq.ShowDialog ();
            fq.Dispose ();

            if (this.m_LogExit == cGlobalParas.LogExitPara.Cancel)
            {
                return;
            }
            else if (this.m_LogExit == cGlobalParas.LogExitPara.BackupAndClear)
            {
                //������־
                BackupLog(logType);
            }

            string lPath = Program.getPrjPath() + "log";

            //ϵͳ��־Ϊ8λ�ļ����ģ��ҿ�������ת�����ڵ�
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //�й�Ŀ¼�µ��ļ�   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                switch (logType)
                {
                    case 0:
                        //��չ��һ��ΪCSV
                        if (fName.Length == 8 && eName.EndsWith("csv", StringComparison.CurrentCultureIgnoreCase))
                        {

                            System.IO.File.Delete(fis2[i2].FullName);
                        }
                        break;
                    case 1:
                        //��չ��һ��ΪCSV
                        if (fName.StartsWith("radar", StringComparison.CurrentCultureIgnoreCase) && fName.Length == 13 && eName.EndsWith("csv", StringComparison.CurrentCultureIgnoreCase))
                        {
                            System.IO.File.Delete(fis2[i2].FullName);
                           
                        }
                        break;
                    case 2:
                        if (fName.StartsWith("task", StringComparison.CurrentCultureIgnoreCase) && eName.EndsWith("csv", StringComparison.CurrentCultureIgnoreCase))
                        {
                            System.IO.File.Delete(fis2[i2].FullName);
                          
                        }
                        break;
                }

            }

            MessageBox.Show("��־����ɹ���", "��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        //�������һ����־�ļ�
        private bool ClearFileLog(string FileName)
        {
            frmQuestion fq = new frmQuestion();
            fq.RPara = new frmQuestion.ReturnPara(GetLogBackup);
            fq.ShowDialog();
            fq.Dispose();

            if (this.m_LogExit == cGlobalParas.LogExitPara.Cancel)
            {
                return false ;
            }
            else if (this.m_LogExit == cGlobalParas.LogExitPara.BackupAndClear)
            {
                //������־
                BackupFileLog(FileName);
            }

            System.IO.File.Delete(FileName);

            return true;
        }

        private void GetLogBackup(cGlobalParas.LogExitPara lp)
        {
            m_LogExit = lp;
        }

        #endregion

        #region ������־

        private void BackupLog()
        {
            switch (this.treeMenu.SelectedNode.Name)
            {
                case "nodRoot":
                    break;
                case "nodSystemLog":
                    BackupLog(0);
                    break;
                case "nodRadarLog":
                    BackupLog(1);
                    break;
                case "nodSilentLog":
                    BackupLog(2);
                    break;
                default:
                    if (this.treeMenu.SelectedNode.Name.StartsWith("Log"))
                    {
                        BackupFileLog(this.treeMenu.SelectedNode.Tag.ToString());
                    }
                    break;
            }
        }

        //logType �����־�����ͣ�0-ϵͳ��־��1-�״���־��2-��Ĭִ����־
        private void BackupLog(int logType)
        {
            string sPath = "";

            this.folderBrowserDialog1.Description = "��ѡ�񱣴���־��Ŀ¼";
            this.folderBrowserDialog1.SelectedPath = Program.getPrjPath();
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                sPath = this.folderBrowserDialog1.SelectedPath;
            }
            else
            {
                return;
            }

            switch (logType)
            {
                case 0:
                    BackupSystemLog(sPath);
                    break;
                case 1:
                    BackupRadarLog(sPath);
                    break;
                case 2:
                    BackupGatherLog(sPath);
                    break;
            }

        }

        private void BackupSystemLog(string sPath)
        {
            string lPath = Program.getPrjPath() + "log";

            //ϵͳ��־Ϊ8λ�ļ����ģ��ҿ�������ת�����ڵ�
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //�й�Ŀ¼�µ��ļ�   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                //��չ��һ��ΪCSV
                if (fName.Length == 8 && eName.EndsWith("csv", StringComparison.CurrentCultureIgnoreCase))
                {

                    string fname = Path.GetFileName(fis2[i2].FullName);
                    File.Copy(fis2[i2].FullName, sPath + "\\" + fname,true);
                }

            }
        }

        private void BackupRadarLog(string sPath)
        {
            string lPath = Program.getPrjPath() + "log";

            //ϵͳ��־Ϊ8λ�ļ����ģ��ҿ�������ת�����ڵ�
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //�й�Ŀ¼�µ��ļ�   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                //��չ��һ��ΪCSV
                if (fName.StartsWith("radar", StringComparison.CurrentCultureIgnoreCase) && fName.Length == 13 && eName.EndsWith("csv", StringComparison.CurrentCultureIgnoreCase))
                {
                    string fname = Path.GetFileName(fis2[i2].FullName);
                    File.Copy(fis2[i2].FullName, sPath + "\\" + fname,true);
                }

            }
        }

        private void BackupGatherLog(string sPath)
        {
            string lPath = Program.getPrjPath() + "log";

            //ϵͳ��־Ϊ8λ�ļ����ģ��ҿ�������ת�����ڵ�
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //�й�Ŀ¼�µ��ļ�   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                //��չ��һ��ΪCSV
                if (fName.StartsWith("task", StringComparison.CurrentCultureIgnoreCase) && eName.EndsWith("csv", StringComparison.CurrentCultureIgnoreCase))
                {
                    string fname = Path.GetFileName(fis2[i2].FullName);
                    File.Copy(fis2[i2].FullName, sPath + "\\" + fname, true);
                }

            }
        }

        private void BackupFileLog(string FileName)
        {
            string sPath="";

            this.folderBrowserDialog1.Description = "��ѡ�񱣴���־��Ŀ¼";
            this.folderBrowserDialog1.SelectedPath = Program.getPrjPath();
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                sPath = this.folderBrowserDialog1.SelectedPath;
            }
            else
            {
                return ;
            }

            string fname=Path.GetFileName (FileName );

            File.Copy(FileName, sPath + "\\" + fname);

        }

        #endregion

        private void toolExportLog_Click(object sender, EventArgs e)
        {
            BackupLog();
        }

        private void menuClearLog_Click(object sender, EventArgs e)
        {
            ClearLog();
        }

        private void menuExportLog_Click(object sender, EventArgs e)
        {
            BackupLog();

        }

        private void rmenuClearLog_Click(object sender, EventArgs e)
        {
            ClearLog();
        }

        private void rmenuExportLog_Click(object sender, EventArgs e)
        {
            BackupLog();
        }

        private void listLog_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // �������ô��е����򷽷�.
                if (lvwColumnSorter.Order == System.Windows.Forms.SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = System.Windows.Forms.SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = System.Windows.Forms.SortOrder.Ascending;
                }
            }
            else
            {
                // ���������У�Ĭ��Ϊ��������
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = System.Windows.Forms.SortOrder.Ascending;
            }

            // ���µ����򷽷���ListView����
            this.listLog.Sort();
        }

    }
}
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
                MessageBox.Show("日志文件被破坏，无法加载，请删除此日志文件“" + this.treeMenu.SelectedNode.Tag.ToString () + "”后重试！错误信息：" + ex.Message,"错误",MessageBoxButtons.OK ,MessageBoxIcon.Error );
            }
        }

        #region 数据采集日志
        private void LoadGatherTaskLog()
        {
            if (this.treeMenu.SelectedNode.Nodes.Count == 0)
            {
                //加载数据
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

            this.listLog.Columns.Add("任务名称", 160);
            this.listLog.Columns.Add("文件", 160);
            this.listLog.Columns.Add("大小", 120);
            this.listLog.Columns.Add("位置", 200);

            string lPath = Program.getPrjPath() + "log";

            //系统日志为8位文件名的，且可以正常转成日期的
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //有关目录下的文件   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                //扩展名一定为CSV
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
                MessageBox.Show ("日志文件不存在，有可能已经被清除或被破坏！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //定义一个修改分类名称的委托
            this.listLog.Columns.Clear();
            this.listLog.Items.Clear();

            this.listLog.Columns.Add("任务名称", 100);
            this.listLog.Columns.Add("时间", 120);
            this.listLog.Columns.Add("类别", 100);
            this.listLog.Columns.Add("详细", 400);

            delegateLoadGatherLog sd = new delegateLoadGatherLog(this.OpenGatherLog);

            try
            {
                //开始调用函数,可以带参数 
                IAsyncResult ir = sd.BeginInvoke(FileName, null, null);

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
                            cItem.SubItems.Add ("信息");
                            break;
                        case cGlobalParas.LogType.Error:
                            cItem.ImageKey = "Error";
                            cItem.SubItems.Add("错误");
                            break;
                        case cGlobalParas.LogType.Warning:
                            cItem.ImageKey = "Warning";
                            cItem.SubItems.Add("警告");
                            break;
                        case cGlobalParas.LogType.GatherError :
                            cItem.ImageKey = "Error";
                            cItem.SubItems.Add("采集错误");
                            break;
                        case cGlobalParas.LogType.PublishError :
                            cItem.ImageKey = "Error";
                            cItem.SubItems.Add("发布错误");
                            break;
                        case cGlobalParas.LogType.RunPlanTask:
                            cItem.ImageKey = "Error";
                            cItem.SubItems.Add("运行计划");
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

        #region 显示雷达日志信息

        private void ListRadarLog()
        {
            this.listLog.Columns.Clear();
            this.listLog.Items.Clear();

            this.listLog.Columns.Add("文件", 160);
            this.listLog.Columns.Add("大小", 120);
            this.listLog.Columns.Add("位置", 200);

            string lPath = Program.getPrjPath() + "log";

            //系统日志为Radar + 8位文件名的，且可以正常转成日期的
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //有关目录下的文件   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                //扩展名一定为CSV
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
                //加载数据
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

            this.staSta.Text = "正在加载日志信息，请等待......";

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

            this.staSta.Text = "当前状态：就绪";

        }

        #region 加载系统日志信息  雷达详细日志与其共用
        private void LoadSystemLog()
        {
            if (this.treeMenu.SelectedNode.Nodes.Count == 0)
            {
                //加载数据
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

            this.listLog.Columns.Add("文件", 160);
            this.listLog.Columns.Add("大小", 120);
            this.listLog.Columns.Add("位置", 200);

            string lPath = Program.getPrjPath() + "log";

            //系统日志为8位文件名的，且可以正常转成日期的
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //有关目录下的文件   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                //扩展名一定为CSV
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
                MessageBox.Show ("日志文件不存在，有可能已经被清除或被破坏！","信息",MessageBoxButtons.OK ,MessageBoxIcon.Information );
                return;
            }

            //定义一个修改分类名称的委托
            this.listLog.Columns.Clear();
            this.listLog.Items.Clear();

            this.listLog.Columns.Add("级别", 100);
            this.listLog.Columns.Add("时间", 120);
            this.listLog.Columns.Add("来源", 100);
            this.listLog.Columns.Add("详细", 400);

            delegateLoadSystemLog sd = new delegateLoadSystemLog(this.OpenSystemLog);

            try
            {
                //开始调用函数,可以带参数 
                IAsyncResult ir = sd.BeginInvoke(FileName, null, null);

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
                            cItem.Text = "信息";
                            break;
                        case cGlobalParas.LogType.Error:
                            cItem.ImageKey = "Error";
                            cItem.Text = "错误";
                            break;
                        case cGlobalParas.LogType.Warning:
                            cItem.ImageKey = "Warning";
                            cItem.Text = "警告";
                            break;
                    }

                    cItem.SubItems.Add(dLog.Rows[i][2].ToString());

                    switch ((cGlobalParas.LogClass)int.Parse(dLog.Rows[i][1].ToString()))
                    {
                        case cGlobalParas.LogClass.Data:
                            cItem.SubItems.Add("数据操作");
                            break;
                        case cGlobalParas.LogClass.GatherLog:
                            cItem.SubItems.Add("数据采集");
                            break;
                        case cGlobalParas.LogClass.ListenPlan:
                            cItem.SubItems.Add("监听计划");
                            break;
                        case cGlobalParas.LogClass.PublishLog:
                            cItem.SubItems.Add("数据发布");
                            break;
                        case cGlobalParas.LogClass.Radar:
                            cItem.SubItems.Add("监控雷达");
                            break;
                        case cGlobalParas.LogClass.System:
                            cItem.SubItems.Add("网络矿工");
                            break;
                        case cGlobalParas.LogClass.Task:
                            cItem.SubItems.Add("采集任务");
                            break;
                        case cGlobalParas.LogClass.Trigger:
                            cItem.SubItems.Add("触发器");
                            break;
                        case cGlobalParas.LogClass.RunTask :
                            cItem.SubItems.Add("执行任务");
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

        #region 清除日志
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

                    //删除节点
                    this.treeMenu.SelectedNode.Parent.Nodes.Remove (this.treeMenu.SelectedNode);

                    MessageBox.Show("日志清除成功！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;
            }

        }

        //logType 清除日志的类型：0-系统日志；1-雷达日志；2-静默执行日志
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
                //备份日志
                BackupLog(logType);
            }

            string lPath = Program.getPrjPath() + "log";

            //系统日志为8位文件名的，且可以正常转成日期的
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //有关目录下的文件   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                switch (logType)
                {
                    case 0:
                        //扩展名一定为CSV
                        if (fName.Length == 8 && eName.EndsWith("csv", StringComparison.CurrentCultureIgnoreCase))
                        {

                            System.IO.File.Delete(fis2[i2].FullName);
                        }
                        break;
                    case 1:
                        //扩展名一定为CSV
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

            MessageBox.Show("日志清除成功！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        //特制清除一个日志文件
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
                //备份日志
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

        #region 备份日志

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

        //logType 清除日志的类型：0-系统日志；1-雷达日志；2-静默执行日志
        private void BackupLog(int logType)
        {
            string sPath = "";

            this.folderBrowserDialog1.Description = "请选择保存日志的目录";
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

            //系统日志为8位文件名的，且可以正常转成日期的
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //有关目录下的文件   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                //扩展名一定为CSV
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

            //系统日志为8位文件名的，且可以正常转成日期的
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //有关目录下的文件   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                //扩展名一定为CSV
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

            //系统日志为8位文件名的，且可以正常转成日期的
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //有关目录下的文件   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                //扩展名一定为CSV
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

            this.folderBrowserDialog1.Description = "请选择保存日志的目录";
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
                // 重新设置此列的排序方法.
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
                // 设置排序列，默认为正向排序
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = System.Windows.Forms.SortOrder.Ascending;
            }

            // 用新的排序方法对ListView排序
            this.listLog.Sort();
        }

    }
}
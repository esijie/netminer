using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using NetMiner.Gather.Task;
using System.Reflection;
using SoukeyControl.CustomControl;
using System.IO;
using System.Resources;
using SoukeyControl.WebBrowser;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Common.Tool;
using NetMiner.Core.Dict;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;
using NetMiner;
using System.Text;

///功能：网络矿工主界面处理（包括线程响应事件）
///完成时间：2009-3-2
///作者：一孑
///遗留问题：代码还未整理，可能看起来比较乱
///开发计划：无
///说明：
///版本：01.10.00
///修订：无
///修订：2010-12-10 
///在系统启动的时候，即将代理列表信息加载进来，以供系统采集使用
///从V5.5开始，取消静默状态运行
namespace MinerSpider
{

    public partial class frmMain : Form
    {

        private string DelName;
       
        private cGlobalParas.ExitPara m_ePara = cGlobalParas.ExitPara.Exit;

        private ResourceManager rm;

        //定义一个变量获取注册激活的结果
        private cGlobalParas.RegisterResult m_rResult;

        //定义一个后台线程的计时器，根据时间定期去官网验证版本的合法性
        private System.Threading.Timer m_VerifyEngine;

        //如果是正版软件，则将注册信息写入此结构
        //cVersion.StructRegisterInfo m_RegInfo;

        //是否保存系统日志标记,默认不保存
        private bool m_IsAutoSaveLog = false;

        //是否保存雷达日志 默认不保存
        private bool m_IsAutoSaveRadarLog = false;

        //记录最大界面输出的日志数量
        private int m_MaxLogNumber;

        //是否进行合法性验证标识
        private bool m_IsVerifyFlag;

        #region  主窗口下显示的子窗口
        //定义两个窗体变量，用于显示加载子窗体内容
        private frmTreeMenu m_TreeMenu = new frmTreeMenu();
        //private frmRemoteMenu m_RemoteMenu = new frmRemoteMenu();

        private frmTaskContent m_TaskContent = new frmTaskContent();
        //创建一个正在运行任务的列表，重新布局页面
        private frmRunningTask m_frmRunningTask = new frmRunningTask();

        private frmCompletedTask m_frmCompletedTask = new frmCompletedTask();

        #endregion

        //private frmMonitor m_Monitor = new frmMonitor();
        private frmHelpInfo m_Help = new frmHelpInfo();
        //private frmRemoteServer m_RServer = new frmRemoteServer();
        private frmManageRemoteServer m_MServer;

        private frmCloudGather m_Cloud = new frmCloudGather();

        //定义日志窗体变量，此窗体可以被关闭
        private frmLog m_Log;

        //定义一个变量，记录当前雷达是否启动
        private bool m_IsRadarStarted = false;

        //定义一个变量，标识当前图标是否进行闪烁
        private bool m_IsIconFlicker = false;

        //定义一个代理控制器，并加载代理数据，以供系统采集时所需
        //这是一个资源类
        //private cProxyControl m_ProxyControl;

        //定义一个值，判断是否为重头搜索
        private bool m_FindStart =true;
        private int m_FindIndex = -1;

        //定义一个值，表示是否有任务运行完毕后，关闭计算机
        private bool m_CloseComputer = false;

        //定义一个值，记录是否启动了关闭计算机的标识，系统自动维护，无法人工控制
        //在m_CloseComputer=true时进行启动维护
        private bool m_AutoClose = false;

        public frmMain()
        {
            InitializeComponent();

            //加载主界面显示的资源文件
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            //加载托盘图标
            this.notifyIcon1.Visible = true;
            

            if (Program.GetCurrentVersion() == cGlobalParas.VersionType.Enterprise)
            {
                this.notifyIcon1.ShowBalloonTip(2, "网络矿工[" + Program.SominerVersionEnter + "]", rm.GetString("TrayInfo3"), ToolTipIcon.Info); 

            }
            else if (Program.GetCurrentVersion() == cGlobalParas.VersionType.Ultimate)
            {
                this.notifyIcon1.ShowBalloonTip(2, "网络矿工[" + Program.SominerVersionUltimate + "]", rm.GetString("TrayInfo3"), ToolTipIcon.Info);
            }
            else if (Program.GetCurrentVersion() == cGlobalParas.VersionType.Program)
            {
                this.notifyIcon1.ShowBalloonTip(2, "网络矿工[" + Program.SominerVersionProgram + "]", rm.GetString("TrayInfo3"), ToolTipIcon.Info);
            }
            else if (Program.GetCurrentVersion() == cGlobalParas.VersionType.Free)
            {
                this.notifyIcon1.ShowBalloonTip(2, "网络矿工[" + Program.SominerVersionFree + "]", rm.GetString("TrayInfo3"), ToolTipIcon.Info);
            }
            else if (Program.GetCurrentVersion()==cGlobalParas.VersionType.Cloud)
            {
                this.notifyIcon1.ShowBalloonTip(2,"网络矿工[" + Program.SominerVersionCloud + "]", rm.GetString("TrayInfo3"), ToolTipIcon.Info);
            }
            else
            {
                this.notifyIcon1.ShowBalloonTip(2, "网络矿工[" + Program.SominerVersionFree + "]", rm.GetString("TrayInfo3"), ToolTipIcon.Info);
            }


        }

        #region 窗体初始化操作

        //此方法主要进行界面初始化的操作，无参数
        //此方法主要是初始化界面显示的内容,包括
        //树形结构,默认节点,网页打开默认的页面等等
        //public void IniProxy()
        //{
        //    this.m_ProxyControl = new cProxyControl();
        //}

        public void IniForm()
        {

            //根据当前显示的语言，设置菜单的默认选项
            bool isAllDistrib=false ;
            try
            {
                cXmlSConfig Config = new cXmlSConfig(Program.getPrjPath());
                cGlobalParas.CurLanguage cl = Config.CurrentLanguage;

                #region  控制软件语言界面
                //switch (cl)
                //{
                //    case cGlobalParas.CurLanguage.Auto:
                //        this.toolmenuAuto.Checked = true;
                //        break;
                //    case cGlobalParas.CurLanguage.enUS:
                //        this.toolmenuAuto.Checked = false;
                //        this.toolmenuEnglish.Checked = true;
                //        this.toolmenuCHS.Checked = false;
                //        break;
                //    case cGlobalParas.CurLanguage.zhCN:
                //        this.toolmenuAuto.Checked = false;
                //        this.toolmenuCHS.Checked = true;
                //        this.toolmenuEnglish.Checked = false;
                //        break;
                //    default:
                //        break;
                //}
                #endregion

                this.m_IsRadarStarted = Config.IsAutoStartRadar;

                this.m_IsAutoSaveLog = Config.AutoSaveLog;
                this.m_IsAutoSaveRadarLog = Config.AutoSaveRadarLog;
                this.m_MaxLogNumber = Config.LogMaxNumber;
              
                isAllDistrib =Config.IsAllowRemoteGather;
                Config = null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("启动发生错误，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //if (Program.IsSilent == true)
            //{
            //    this.toolmenuSilentRun.Checked = true;
            //    this.staRunType.Image = Bitmap.FromFile(Program.getPrjPath() + "img\\A66.gif");
            //    this.staRunType.Text = rm.GetString("Info272");
            //}

            //设置菜单的风格
            SetStyle("Azure");

            #region 控制服务器版本菜单的显示状态
            if (Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                this.tab4.Visible = true;
                this.staRemote.Visible = true;
                if (isAllDistrib == true)
                {
                    this.staRemote.Text = "已连接:" + Program.ConnectServer;
                    this.staRemote.ToolTipText = "已连接:" + Program.ConnectServer + " 允许接收服务器采集请求";
                }
                else
                {
                    this.staRemote.Text = "已连接:" + Program.ConnectServer;
                    this.staRemote.ToolTipText = "已连接:" + Program.ConnectServer + " 禁止接收服务器采集请求";
                }
            }
            else
            {
                this.staRemote.Visible = false ;
                this.tab4.Visible = false;
            }

            //显示远程图标
           
            #endregion
          

            #region 将菜单全部设置成为黑色
            this.tab1.ForeColor = Color.Black;
            this.tab2.ForeColor = Color.Black;
            this.tab3.ForeColor = Color.Black;
            this.tab4.ForeColor = Color.Black;

            this.toolStartTask.ForeColor = Color.Black;
            this.toolStopTask.ForeColor = Color.Black;
            this.toolBrowserData.ForeColor = Color.Black;
            this.toolmenuEditData.ForeColor = Color.Black;
            this.toolmenuClearData.ForeColor = Color.Black;

            this.toolCopyTask.ForeColor = Color.Black;
            this.toolPasteTask.ForeColor = Color.Black;
            this.toolDelTask.ForeColor = Color.Black;
            this.toolFindTask.ForeColor = Color.Black;

            this.toolmenuNewTask.ForeColor = Color.Black;
            this.toolmenuNewRadarRule.ForeColor = Color.Black;
            this.toolMenuNewTaskPlan.ForeColor = Color.Black;
            this.toolmenuNewTaskClass.ForeColor = Color.Black;
            this.toolMenuPublishTemplate.ForeColor = Color.Black;

            this.toolMenuExit.ForeColor = Color.Black;

            this.toolRadar.ForeColor = Color.Black;
            this.toolmenuConfig.ForeColor = Color.Black;
            this.toolUrlEncoding.ForeColor = Color.Black;
            this.toolRegex.ForeColor = Color.Black;
            this.toolManageDict.ForeColor = Color.Black;

            this.toolDataProcess.ForeColor = Color.Black;
            this.toolStartLog.ForeColor = Color.Black;
            this.toolWebbrowser.ForeColor = Color.Black;
            this.toolmenuUpgradeTask.ForeColor = Color.Black;
            this.toolmenuImportSingleTask.ForeColor = Color.Black;
            this.toolmenuImportMoreTask.ForeColor = Color.Black;
            this.toolmenuBackupTask.ForeColor = Color.Black;

            this.toolMenuOnlineHelp.ForeColor = Color.Black;
            this.toolMenuVisityijie.ForeColor = Color.Black;
            this.toolMenuUpdate.ForeColor = Color.Black;
            this.toolMenuRegister.ForeColor = Color.Black;
            this.toolMenuAbout.ForeColor = Color.Black;

            this.toolLookInfo.ForeColor = Color.Black;
            this.toolOCR.ForeColor = Color.Black;
            this.toolSniffer.ForeColor = Color.Black;

            this.toolMenuConnect.ForeColor = Color.Black;
            this.toolmenuRegRemote.ForeColor = Color.Black;
            this.toolManageRemoteServer.ForeColor = Color.Black;
            this.toolDelRemoteData.ForeColor = Color.Black;
            this.toolMenuUploadTask.ForeColor = Color.Black;
            this.toolPlugin.ForeColor = Color.Black;
            this.toolPublishTemplate.ForeColor = Color.Black;
            this.toolSynDb.ForeColor = Color.Black;

            this.toolRemotePlan.ForeColor = Color.Black;
            this.toolmenuProxy.ForeColor = Color.Black;
            this.toolUploadDict.ForeColor = Color.Black;

            this.toolRemoteTaskClass.ForeColor = Color.Black;

            this.toolCloudGather.ForeColor = Color.Black;

            #endregion

          

            if (Program.SominerVersion == cGlobalParas.VersionType.Free)
            {
                this.toolManageDict.Enabled = false;
                this.toolSynDb.Enabled = false;
                this.toolCloudGather.Enabled = false;
            }

            if (Program.g_isLogin)
            {
                this.linkLabel1.Text = Program.RegisterUser;
            }
        }

        public void IniTaskData()
        {
            #region 加载树形菜单的窗口
            this.m_TreeMenu.TreeNodeMouseClick += this.on_TreeNodeClick;
            this.m_TreeMenu.SetControlProperty += this.on_SetControlProerty;
            this.m_TreeMenu.ExcuteFunction += this.on_ExcuteFunction;
            this.m_TreeMenu.DelInfo += this.on_DelInfo;
            this.m_TreeMenu.DelRow += this.on_DelRow;
            this.m_TreeMenu.IsAutoSaveLog = this.m_IsAutoSaveLog;
            this.m_TreeMenu.MaxLogNumber = this.m_MaxLogNumber;
            this.m_TreeMenu.IniData();
            this.m_TreeMenu.Show(this.dockPanel);
            #endregion

            //加载任务管理窗口
            this.m_TaskContent.ExcuteFunction += this.on_ExcuteFunction;
            this.m_TaskContent.DelInfo += this.on_DelInfo;
            this.m_TaskContent.SetControlProperty += this.on_SetControlProerty;
            this.m_TaskContent.SetToolbarState += this.on_SetToolbarState;

            //加载运行窗口
            this.m_frmRunningTask.MaxLogNumber = this.m_MaxLogNumber;
            this.m_frmRunningTask.SetToolbarState += this.on_SetToolbarState;
            this.m_frmRunningTask.ExcuteFunction += this.on_ExcuteFunction;
            this.m_frmRunningTask.ExportLog += this.on_ExportLog;
            this.m_frmRunningTask.ShowLogInfo += this.on_ShowInfo;
            this.m_frmRunningTask.SetControlProperty += this.on_SetControlProerty;
            this.m_frmRunningTask.DelInfo += this.on_DelInfo;
            this.m_frmRunningTask.dSpeedEvent += this.on_UpdateDSpeed;
            this.m_frmRunningTask.OpenDataEvent += this.on_OpenData;

            //加载完成任务的窗口
            this.m_frmCompletedTask.OpenDataEvent += this.on_OpenData;
            this.m_frmCompletedTask.ShowLogInfo += this.on_ShowInfo;
            this.m_frmRunningTask.IniData();

            //启动监听器,用于定制计划任务的执行
            this.m_frmRunningTask.StartListen();

            //设置taskcontent的树形节点，与Treemenu保持一致

            this.m_TaskContent.TreeNode = this.m_TreeMenu.SelectedNode;
            this.m_frmCompletedTask.IniData();
            this.m_TaskContent.LoadTask(this.m_TreeMenu.SelectedNode);


            this.m_TaskContent.Show(this.dockPanel);
            this.m_frmRunningTask.Show(this.dockPanel);
            this.m_frmCompletedTask.Show(this.dockPanel);
            this.m_frmRunningTask.Activate();
        }

        //public void IniMonitor()
        //{
        //    this.m_Monitor.IsAutoSaveRadarLog = this.m_IsAutoSaveRadarLog;
        //    this.m_Monitor.MaxLogNumber = this.m_MaxLogNumber;

        //    if (Program.GetCurrentVersion() == cGlobalParas.VersionType.Union ||
        //        Program.GetCurrentVersion() == cGlobalParas.VersionType.Enterprise ||
        //        Program.GetCurrentVersion() == cGlobalParas.VersionType.Ultimate)
        //    {
        //        this.m_Monitor.SetToolbarState += this.on_SetToolbarState;
        //        this.m_Monitor.ExcuteFunction += this.on_ExcuteFunction;
        //        this.m_Monitor.ExportLog += this.on_ExportLog;
        //        this.m_Monitor.ShowLogInfo += this.on_ShowInfo;
        //        this.m_Monitor.SetControlProperty += this.on_SetControlProerty;
        //        this.m_Monitor.DelInfo += this.on_DelInfo;

        //        this.m_Monitor.IniData();
        //        this.m_Monitor.Show(this.dockPanel);
        //    }
        //}

        public void IniHelp()
        {
            this.m_Help.ExcuteFunction += this.on_ExcuteFunction;
            m_Help.Show(this.dockPanel);
        }

        public void IniRemoteServer()
        {
            //if (Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            //{
            //    this.m_RServer.SetToolbarState += this.on_SetToolbarState1;
            //    m_RServer.Show(this.dockPanel);
            //}

            if (Program.SominerVersion != cGlobalParas.VersionType.Cloud )
            {
                this.m_TreeMenu.treeMenu.Nodes["nodTaskClass"].Nodes["nodCloud"].Remove();
            }
        }

        public void IniSetForm()
        {
            //当所有数据加载完毕后，在进行首页的初始化
            this.m_Help.IniData();

            this.m_Help.Activate();

            //判断雷达是否自动启动
            if (this.m_IsRadarStarted == true)
            {
                try
                {
                    this.m_frmRunningTask.StartRadar();
                    SetRadar(true);
                }
                catch (System.Exception)
                {
                    SetRadar(false);
                }
            }
            else
            {
                SetRadar(false);
            }

            this.StateInfo.Text = "欢迎使用";
        }

        //此方法主要是初始化系统对象,包括需要初始化消息事件,初始化采集
        //任务控制器,并加载运行区的数据,如果运行区无数据,则初始化一个空
        //的对象
        

        #endregion

        //设置雷达启动或停止的界面显示状态
        public void SetRadar(bool IsStarted)
        {
            if (IsStarted == true)
            {
                //this.m_TreeMenu.treeMenu.Nodes["nodAdvance"].Nodes["nodRadarRule"].ImageIndex = 16;
                //this.m_TreeMenu.treeMenu.Nodes["nodAdvance"].Nodes["nodRadarRule"].SelectedImageIndex = 16;
                //string ss = this.m_TreeMenu.treeMenu.Nodes["nodAdvance"].Nodes["nodRadarRule"].Text.Substring(0, this.m_TreeMenu.treeMenu.Nodes["nodAdvance"].Nodes["nodRadarRule"].Text.IndexOf("("));

                //this.m_TreeMenu.treeMenu.Nodes["nodAdvance"].Nodes["nodRadarRule"].Text = ss + "(" + rm.GetString("Info241") + ")";

                this.StaRadar.Image = imageList1.Images[16];
                this.StaRadar.Text = rm.GetString("Info241");

                this.toolRadar.Image = imageList1.Images[16];
                this.toolRadar.Text = rm.GetString("Info243");

                m_IsRadarStarted = true ;

                m_Help.SetRadarInfo(true);

            }
            else
            {
                //this.m_TreeMenu.treeMenu.Nodes["nodAdvance"].Nodes["nodRadarRule"].ImageIndex = 19;
                //this.m_TreeMenu.treeMenu.Nodes["nodAdvance"].Nodes["nodRadarRule"].SelectedImageIndex = 19;
                //string ss = this.m_TreeMenu.treeMenu.Nodes["nodAdvance"].Nodes["nodRadarRule"].Text.Substring(0, this.m_TreeMenu.treeMenu.Nodes["nodAdvance"].Nodes["nodRadarRule"].Text.IndexOf("("));

                //this.m_TreeMenu.treeMenu.Nodes["nodAdvance"].Nodes["nodRadarRule"].Text = ss + "(" + rm.GetString("Info240") + ")";

                this.StaRadar.Image = imageList1.Images[19];
                this.StaRadar.Text = rm.GetString("Info240");

                this.toolRadar.Image = imageList1.Images[19];
                this.toolRadar.Text = rm.GetString("Info242");

                m_IsRadarStarted = false  ;

                m_Help.SetRadarInfo(false);
            }
        }

        #region 响应子窗体时间内容
        private void on_SetControlProerty(object sender, SetControlPropertyEvent e)
        {
            switch (e.ControlName )
            {
                case "toolLookInfo":
                   
                    break;
                case "menuExportData":
                    //if (e.Value == "true" || e.Value == "false")
                    //    SetValue((this.menuExportData), e.Property, bool.Parse(e.Value));
                    //else
                    //    SetValue((this.menuExportData), e.Property, e.Value);
                    break;
                case "toolStartTask":
                    if (e.Value == "true" || e.Value == "false")
                        SetValue((this.toolStartTask), e.Property, bool.Parse(e.Value));
                    else
                        SetValue((this.toolStartTask), e.Property, e.Value);
                    break;
                case "toolStopTask":
                    if (e.Value == "true" || e.Value == "false")
                        SetValue((this.toolStopTask), e.Property, bool.Parse(e.Value));
                    else
                        SetValue((this.toolStopTask), e.Property, e.Value);
                    break;
               
                case "toolStripStatusLabel2":
                    if (e.Value == "true" || e.Value == "false")
                        SetValue((this.statusStrip1.Items["toolStripStatusLabel2"]), e.Property, bool.Parse(e.Value));
                    else
                        SetValue((this.statusStrip1.Items["toolStripStatusLabel2"]), e.Property, e.Value);
                    break;

                case "toolPasteTask":
                    if (e.Value == "true" || e.Value == "false")
                        SetValue((this.toolPasteTask), e.Property, bool.Parse(e.Value));
                    else
                        SetValue((this.toolPasteTask), e.Property, e.Value);
                    break;
                
            

                case "toolmenuRegRemote":
                    if (e.Value == "true" || e.Value == "false")
                        SetValue((this.toolmenuRegRemote), e.Property, bool.Parse(e.Value));
                    else
                        SetValue((this.toolmenuRegRemote), e.Property, e.Value);
                    break;
                case "toolManageRemoteServer":
                    if (e.Value == "true" || e.Value == "false")
                        SetValue((this.toolManageRemoteServer), e.Property, bool.Parse(e.Value));
                    else
                        SetValue((this.toolManageRemoteServer), e.Property, e.Value);
                    break;
                default :
                    SetValue(e.ControlName,e.Property ,e.Value );
                    break;
            }
        }

        /// <summary>
        /// 由两个窗体进行触发，因此需要处理队列的行为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void on_ExcuteFunction(object sender, ExcuteFunctionEvent e)
        {
            if (e.Parameters == null)
                InvokeMethod(this, e.FunctionName, null);
            else
            {
                if (e.Parameters[0].ToString() == "true" || e.Parameters[0].ToString() == "false")
                {
                    InvokeMethod(this, e.FunctionName, e.Parameters);
                }
                else
                {
                    InvokeMethod(this, e.FunctionName, e.Parameters);
                }
            }
        }

        private void on_TreeNodeClick(object sender, TreeNodeMouseClickEvent e)
        {
            treeMenuMouseClick(e.node);
        }

        private void on_SetToolbarState(object sender, SetToolbarStateEvent e)
        {
            SetToolbarState(e.gridrowCount,e.TaskState );
        }

        //用于远程服务器任务点击之后对主窗体按钮的控制
        private void on_SetToolbarState1(object sender, SetToolbarRemote e)
        {
            SetToolbarState1(e.gridrowCount, e.selectNode);
        }

        private void on_ExportLog(object sender, ExportLogEvent e)
        {
            InvokeMethod(this.m_TreeMenu, "ExportLog", new object[] {e.lType,e.lClass,e.Dtime , e.strLog});
        }

        private void on_ShowInfo(object sender, ShowInfoEvent e)
        {
            ShowInfo(e.Title, e.strInfo);
        }

        private void on_DelInfo(object sender, DelInfoEvent e)
        {
            DelName = e.DelName;
        }

        private void on_UpdateDSpeed(object sender, DownloadSpeedEvent e)
        {
            //SetValue((this.statusStrip1.Items["staDownRate"]), "Text", e.dSpeed.ToString ("f2") + "kb");
        }

        private void on_OpenData(object sender, OpenDataEvent e)
        {
            frmDataOp f = new frmDataOp();
            f.Show(dockPanel);
            f.LoadData(e.dType, e.strCon, e.sql,e.TaskName);
        }

        private void on_DelRow(object sender, DelDatagridRowEvent e)
        {
            this.m_TaskContent.dataTask.Rows.Remove(e.DelRow);
        }

        #endregion

        #region 响应远程操作窗体的内容
        private void on_RemoteTreeClick(object sender, TreeNodeMouseClickEvent e)
        {
            RemotetreeMenuMouseClick(e.node);
        }
        #endregion

        #region 菜单 工具条 树形结构 listview 等控件的 响应事件

        //启动任务，当前设计是只能启动一个任务，不支持启动多个任务
        private void toolStartTask_Click(object sender, EventArgs e)
        {
            this.m_TaskContent.Activate();
            this.m_TaskContent.StartTask();
            
        }

        #endregion

        #region 界面控件事件 所调用的方法
      

        private void CreateTaskIndex(string tPath)
        {
            oTaskIndex tIndex = new oTaskIndex(Program.getPrjPath());
            tIndex.NewIndexFile(tPath);
            tIndex.Dispose();
            tIndex = null;

        }
        
        private void CreateTaskComplete()
        {
            oTaskComplete t = new oTaskComplete(Program.getPrjPath());
            t.NewTaskCompleteFile();
            t = null;
        }

        #endregion

        private void treeMenuMouseClick(TreeNode eNode)
        {

            this.m_TreeMenu.treeMenu.SelectedNode = eNode;
            this.m_TaskContent.TreeNode = eNode;

            //控制节点是否可以编辑，只能编辑任务分类
            if (this.m_TreeMenu.SelectedNode.Name.Substring(0, 1) == "C" || this.m_TreeMenu.SelectedNode.Name == "nodTaskClass"
                || this.m_TreeMenu.SelectedNode.Name == "nodPlanRunning")
            {
                if (this.m_TreeMenu.SelectedNode.Name.Substring(0, 1) == "C")
                    this.m_TreeMenu.treeMenu.LabelEdit = true;
                else
                    this.m_TreeMenu.treeMenu.LabelEdit = false;

                this.m_TaskContent.dataTask.ReadOnly = false;
            }
            else
            {
                this.m_TreeMenu.treeMenu.LabelEdit = false;
                this.m_TaskContent.dataTask.ReadOnly = true;
            }

            switch (eNode.Name)
            {
                case "nodRadarRule":
                    this.m_TaskContent.Activate();
                    this.m_TaskContent.LoadRadarRule(eNode);
                    break;
                

                //case "nodPublish":

                //    this.m_TaskContent.Activate();

                //    this.m_TaskContent.LoadPublishTask(eNode );

                //    //启动时间器用于更新任务显示的进度
                //    this.m_frmRunningTask.timer1.Enabled = true;

                //    break;


                case "nodLog":

                    break;
        
                
                case "nodSnap":

                    this.m_TaskContent.Activate();

                    try
                    {
                        this.m_TaskContent.dataTask.Columns.Clear();
                        this.m_TaskContent.dataTask.Rows.Clear();
                    }
                    catch (System.Exception)
                    {
                    }

                    break;
                case "nodSoukey":

                    this.m_TaskContent.Activate();

                    try
                    {
                        //this.m_TaskContent.LoadSoukeyTask(eNode);
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show(rm.GetString("Info56"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }

                    break;
                case "nodTaskPlan":

                    this.m_TaskContent.Activate();

                    try
                    {
                        this.m_TaskContent.dataTask.Columns.Clear();
                        this.m_TaskContent.dataTask.Rows.Clear();
                    }
                    catch (System.Exception)
                    {
                    }

                    break;
                case "nodPlanRunning":

                    this.m_TaskContent.Activate();

                    try
                    {
                        this.m_TaskContent.LoadTaskPlan(eNode);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(rm.GetString("Info57") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }

                    break;
                case "nodPlanCompleted":

                    //this.m_TaskContent.Activate();

                    //try
                    //{
                    //    this.m_TaskContent.LoadPlanLog(eNode);
                    //}
                    //catch (System.Exception ex)
                    //{
                    //    MessageBox.Show(rm.GetString("Info58") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}

                    break;

                case "nodRadarData":

                    break;

                case "nodRemote":
                    if (Program.IsConnectRemote == cGlobalParas.RegResult.Faild)
                    {
                        MessageBox.Show("还未连接到远程服务器，请先进行远程服务器的配置！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        try
                        {
                            this.m_TaskContent.dataTask.Columns.Clear();
                            this.m_TaskContent.dataTask.Rows.Clear();
                        }
                        catch (System.Exception)
                        {
                        }

                        return;
                    }

                    break;
                case "nodRemoteRunning":
                    if (Program.IsConnectRemote == cGlobalParas.RegResult.Faild)
                    {
                        MessageBox.Show("还未连接到远程服务器，请先进行远程服务器的配置！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        try
                        {
                            this.m_TaskContent.dataTask.Columns.Clear();
                            this.m_TaskContent.dataTask.Rows.Clear();
                        }
                        catch (System.Exception)
                        {
                        }


                        return;
                    }
                    //this.m_RServer.TreeNode = eNode;
                    //this.m_RServer.Activate();
                    //this.m_RServer.LoadRunningList();
                    break;
                case "nodRemoteTask":
                    if (Program.IsConnectRemote == cGlobalParas.RegResult.Faild)
                    {
                        MessageBox.Show("还未连接到远程服务器，请先进行远程服务器的配置！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        try
                        {
                            this.m_TaskContent.dataTask.Columns.Clear();
                            this.m_TaskContent.dataTask.Rows.Clear();
                        }
                        catch (System.Exception)
                        {
                        }


                        return;
                    }
                    //this.m_RServer.TreeNode = eNode;
                    //this.m_RServer.Activate();
                    //this.m_RServer.LoadTask();
                    break;
                case "nodRemoteLog":
                    if (Program.IsConnectRemote == cGlobalParas.RegResult.Faild)
                    {
                        MessageBox.Show("还未连接到远程服务器，请先进行远程服务器的配置！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        try
                        {
                            this.m_TaskContent.dataTask.Columns.Clear();
                            this.m_TaskContent.dataTask.Rows.Clear();
                        }
                        catch (System.Exception)
                        {
                        }


                        return;
                    }
                    //this.m_RServer.TreeNode = eNode;
                    //this.m_RServer.Activate();
                    //this.m_RServer.LoadLog();
                    break;

                case "nodPublishTemplate":
                    this.m_TaskContent.Activate();

                    try
                    {
                        this.m_TaskContent.LoadPublishTemplate(eNode);
                    }
                    catch (System.Exception)
                    {
                    }
                    break;
                case "nodPublishByWeb":
                    this.m_TaskContent.Activate();

                    try
                    {
                        this.m_TaskContent.LoadPublishTemplate(eNode);
                    }
                    catch (System.Exception)
                    {
                    }
                    break;
                case "nodPublishByDB":
                    this.m_TaskContent.Activate();

                    try
                    {
                        this.m_TaskContent.LoadPublishTemplate(eNode);
                    }
                    catch (System.Exception)
                    {
                    }
                    break;
                case "nodCloud":

                    break;
                default:
                    this.m_TaskContent.Activate();

                    if (!eNode.Name.StartsWith("C") && eNode.Name != "nodTaskClass" && eNode.Name != "nodRemoteTaskClass")
                        return;
                    
                    

                    try
                    {
                        this.m_TaskContent.LoadOther(eNode);
                    }
                    catch (System.IO.IOException)
                    {
                        if (MessageBox.Show(this.m_TreeMenu.SelectedNode.Text + rm.GetString("Quaere20"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            CreateTaskIndex(this.m_TreeMenu.SelectedNode.Tag.ToString());
                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(rm.GetString("Info59") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }

                    break;
            }

            //无论点击树形结构的菜单都将按钮置为不可用
            this.toolStartTask.Enabled = false;
            this.toolStopTask.Enabled = false;
            //this.menuExportData.Enabled = false;
            this.toolMenuUploadTask.Enabled = false;
            this.toolDelRemoteData.Enabled = false;

            //this.toolData.Enabled = false;

            this.toolCopyTask.Enabled = false;
            this.toolPasteTask.Enabled = false;

            //置删除按钮为有效
            DelName = this.m_TreeMenu.treeMenu.Name; ;
            this.toolDelTask.Enabled = true;

            UpdateStatebarTaskState(rm.GetString("State7") + " " + eNode.Text);

        }

        //远程服务器左侧菜单响应时间
        private void RemotetreeMenuMouseClick(TreeNode eNode)
        {
            //this.m_RServer.Activate();
            //this.m_RServer.TreeNode = eNode;

            if (Program.IsConnectRemote == cGlobalParas.RegResult.Faild)
            {
                MessageBox.Show("还未连接到远程服务器，请先进行远程服务器的配置！","网络矿工 信息",MessageBoxButtons.OK ,MessageBoxIcon.Error );

                return;
            }

            switch (eNode.Name)
            {
                case "nodRunning":
                    //this.m_RServer.LoadRunningList ();
                    break;
              
                case "nodTask":
                    //this.m_RServer.LoadTask ();

                    break ;
                case "nodLog":
                    //this.m_RServer.LoadLog  ();
                    break;
            }

            //this.toolMenuStartRemoteGather.Enabled = false;
            //this.toolMenuStopRemoteGather.Enabled = false;
            this.toolMenuUploadTask.Enabled = false;
            this.toolDelRemoteData.Enabled = false;
        }

        #region 委托代理 用于后台线程调用 配置UI线程的方法、属性

        delegate void bindvalue(object Instance, string Property, object value);
        delegate object invokemethod(object Instance, string Method, object[] parameters);
        delegate object invokepmethod(object Instance, string Property, string Method, object[] parameters);
        delegate object invokechailmethod(object InstanceInvokeRequired, object Instance, string Method, object[] parameters);

        /// <summary>
        /// 委托设置对象属性
        /// </summary>
        /// <param name="Instance">对象</param>
        /// <param name="Property">属性名</param>
        /// <param name="value">属性值</param>
        private void SetValue(object Instance, string Property, object value)
        {
            Type iType = Instance.GetType();
            object inst=null;

            if (iType.ToString() == "System.Windows.Forms.ToolStripMenuItem" || iType .ToString ()=="System.Windows.Forms.ToolStripButton" ||
                iType.ToString ()==	"System.Windows.Forms.ToolStripSplitButton")
            {
                //inst = this.toolStrip1;
            }
            else if (iType.ToString() == "System.Windows.Forms.ToolStripStatusLabel")
            {
                inst =this.statusStrip1;
            }
            else
            {
                inst = Instance;
            }

            bool a = (bool)GetPropertyValue(inst, "InvokeRequired");

            if (a)
            {
                bindvalue d = new bindvalue(SetValue);
                this.Invoke(d, new object[] { Instance, Property, value });
            }
            else
            {
                SetPropertyValue(Instance, Property, value);
            }
        }
        /// <summary>
        /// 委托执行实例的方法，方法必须都是Public 否则会出错
        /// </summary>
        /// <param name="Instance">类实例</param>
        /// <param name="Method">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private object InvokeMethod(object Instance, string Method, object[] parameters)
        {
            if ((bool)GetPropertyValue(Instance, "InvokeRequired"))
            {
                invokemethod d = new invokemethod(InvokeMethod);
                return this.Invoke(d, new object[] { Instance, Method, parameters });
            }
            else
            {
                return MethodInvoke(Instance, Method, parameters);
            }
        }

        /// <summary>
        /// 委托执行实例的方法
        /// </summary>
        /// <param name="InstanceInvokeRequired">窗体控件对象</param>
        /// <param name="Instance">需要执行方法的对象</param>
        /// <param name="Method">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private object InvokeChailMethod(object InstanceInvokeRequired, object Instance, string Method, object[] parameters)
        {
            if ((bool)GetPropertyValue(InstanceInvokeRequired, "InvokeRequired"))
            {
                invokechailmethod d = new invokechailmethod(InvokeChailMethod);
                return this.Invoke(d, new object[] { InstanceInvokeRequired, Instance, Method, parameters });
            }
            else
            {
                return MethodInvoke(Instance, Method, parameters);
            }
        }
        /// <summary>
        /// 委托执行实例的属性的方法
        /// </summary>
        /// <param name="Instance">类实例</param>
        /// <param name="Property">属性名</param>
        /// <param name="Method">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private object InvokePMethod(object Instance, string Property, string Method, object[] parameters)
        {
            if ((bool)GetPropertyValue(Instance, "InvokeRequired"))
            {
                invokepmethod d = new invokepmethod(InvokePMethod);
                return this.Invoke(d, new object[] { Instance, Property, Method, parameters });
            }
            else
            {
                return MethodInvoke(GetPropertyValue(Instance, Property), Method, parameters);
            }
        }
        /// <summary>
        /// 获取实例的属性值
        /// </summary>
        /// <param name="ClassInstance">类实例</param>
        /// <param name="PropertyName">属性名</param>
        /// <returns>属性值</returns>
        private static object GetPropertyValue(object ClassInstance, string PropertyName)
        {
            Type myType = ClassInstance.GetType();
            PropertyInfo myPropertyInfo = myType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return myPropertyInfo.GetValue(ClassInstance, null);
        }
        /// <summary>
        /// 设置实例的属性值
        /// </summary>
        /// <param name="ClassInstance">类实例</param>
        /// <param name="PropertyName">属性名</param>
        private static void SetPropertyValue(object ClassInstance, string PropertyName, object PropertyValue)
        {
            Type myType = ClassInstance.GetType();
            PropertyInfo myPropertyInfo = myType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            myPropertyInfo.SetValue(ClassInstance, PropertyValue, null);
        }

        /// <summary>
        /// 执行实例的方法
        /// </summary>
        /// <param name="ClassInstance">类实例</param>
        /// <param name="MethodName">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private static object MethodInvoke(object ClassInstance, string MethodName, object[] parameters)
        {
            if (parameters == null)
            {
                parameters = new object[0];
            }
            Type myType = ClassInstance.GetType();
            Type[] types = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                types[i] = parameters[i].GetType();
            }
            MethodInfo myMethodInfo = myType.GetMethod(MethodName, types);
            return myMethodInfo.Invoke(ClassInstance, parameters);
        }

        #endregion

       

        #region 托盘的处理
        private void notifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_IsIconFlicker == true)
                return;

            //显示当前任务队列内容
            string s="";

            try
            {
                s = rm.GetString("TrayTitle").ToString() + "\n";
                s += rm.GetString("TrayInfo1").ToString() + ": " + this.m_frmRunningTask.m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count + "\n";
                s += rm.GetString("TrayInfo2").ToString() + ": " + this.m_frmRunningTask.m_PublishControl.PublishManage.ListPublish.Count;

                if (s.Length > 64)
                {
                    s = rm.GetString("TrayInfo1").ToString() + ": " + this.m_frmRunningTask.m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count + "\n";
                    s += rm.GetString("TrayInfo2").ToString() + ": " + this.m_frmRunningTask.m_PublishControl.PublishManage.ListPublish.Count;

                }
            }
            catch (System.Exception)
            {
                //捕获错误但不处理，让其继续显示信息
            }

          
            this.notifyIcon1.Text = s;
            

        }
        #endregion

        #region 界面操作控制

        ///根据菜单选择及点击的数据内容，自动控制工具栏的按钮状态
        ///因为DataGridView支持多选，所以可能存在多种按钮状态的情况
        ///针对这种情况，系统按照最后选择的内容进行按钮状态设置

        private void SetToolbarState(int gridRowCount,cGlobalParas.TaskState tState)
        {
            if (gridRowCount == 0)
            {
                ResetToolState();
                return;
            }

            try
            {
                switch (this.m_TreeMenu.SelectedNode.Name)
                {
                    case "nodRunning":

                        switch (tState)
                        {
                            case cGlobalParas.TaskState.Started:

                                this.toolStartTask.Enabled = false;
                                this.toolStopTask.Enabled = true;
                                break;

                            case cGlobalParas.TaskState.Running:

                                this.toolStartTask.Enabled = false;
                                this.toolStopTask.Enabled = true;
                                break;

                            case cGlobalParas.TaskState.Failed:

                                this.toolStartTask.Enabled = false;
                                this.toolStopTask.Enabled = false;
                                break;

                            default:

                                this.toolStartTask.Enabled = true;
                                this.toolStopTask.Enabled = false;
                                break;

                        }

                        UpdateStatebarTaskState(tState);


                        //只要有内容就可以删除
                        this.toolDelTask.Enabled = true;

                        if (int.Parse(this.m_TaskContent.dataTask.SelectedCells[9].Value.ToString()) > 0)
                        {
                            this.toolBrowserData.Enabled = true;
                            this.toolmenuEditData.Enabled = true;
                        }
                        else
                        {
                            this.toolBrowserData.Enabled = false;
                            this.toolmenuEditData.Enabled = false;
                        }

                        //t = null;

                        this.toolCopyTask.Enabled = false;
                        this.toolPasteTask.Enabled = false;

                        this.toolMenuUploadTask.Enabled = false;

                        break;
                    case "nodPublish":

                        switch (tState)
                        {
                            case cGlobalParas.TaskState.Publishing:
                                this.toolStartTask.Enabled = false;
                                this.toolStopTask.Enabled = true ;
                                this.toolDelTask.Enabled = false;
                                break;
                            case cGlobalParas.TaskState.PublishStop:
                                this.toolStartTask.Enabled = true;
                                this.toolStopTask.Enabled = false;
                                this.toolDelTask.Enabled = true ;
                                break;
                            default:
                                this.toolStartTask.Enabled = false;
                                this.toolStopTask.Enabled = false;
                                this.toolDelTask.Enabled = true;
                                break;
                        }

                        
                        this.toolBrowserData.Enabled = false;
                        this.toolmenuEditData.Enabled = false;

                        this.toolCopyTask.Enabled = false;
                        this.toolPasteTask.Enabled = false;

                        this.toolMenuUploadTask.Enabled = false;

                        UpdateStatebarTaskState(cGlobalParas.TaskState.Publishing);
                        break;
                    case "nodComplete":
                        this.toolStartTask.Enabled = false;
                        this.toolStopTask.Enabled = false;
                        this.toolDelTask.Enabled = true;
                        this.toolBrowserData.Enabled = true;
                        this.toolmenuEditData.Enabled = true ;

                        this.toolCopyTask.Enabled = false;
                        this.toolPasteTask.Enabled = false;

                        this.toolMenuUploadTask.Enabled = false;

                        UpdateStatebarTaskState(cGlobalParas.TaskState.Completed);

                        break;

                    case "nodTaskClass":

                        //只要有内容就可以删除
                        this.toolDelTask.Enabled = true;

                        if (tState == cGlobalParas.TaskState.Failed)
                            this.toolStartTask.Enabled = false;
                        else
                            this.toolStartTask.Enabled = true;

                        this.toolStopTask.Enabled = false;

                        this.toolCopyTask.Enabled = false;
                        this.toolPasteTask.Enabled = false;

                        this.toolMenuUploadTask.Enabled = true;

                        break;

                    case "nodPublishTemplate":
                        this.toolDelTask.Enabled = true;
                        this.toolStartTask.Enabled = false;
                        this.toolStopTask.Enabled = false;
                        this.toolCopyTask.Enabled = false;
                        this.toolPasteTask.Enabled = false;
                        this.toolMenuUploadTask.Enabled = false;
                        break;

                    case "nodPublishByWeb":
                        this.toolDelTask.Enabled = true;
                        this.toolStartTask.Enabled = false;
                        this.toolStopTask.Enabled = false;
                        this.toolCopyTask.Enabled = false;
                        this.toolPasteTask.Enabled = false;
                        this.toolMenuUploadTask.Enabled = false;
                        break;
                    case "nodPublishByDB":
                        this.toolDelTask.Enabled = true;
                        this.toolStartTask.Enabled = false;
                        this.toolStopTask.Enabled = false;
                        this.toolCopyTask.Enabled = false;
                        this.toolPasteTask.Enabled = false;
                        this.toolMenuUploadTask.Enabled = false;
                        break;

                    default:

                        //只要有内容就可以删除
                        this.toolDelTask.Enabled = true;

                        if (this.m_TreeMenu.SelectedNode.Name.Substring(0, 1) == "C")
                        {
                            if (tState == cGlobalParas.TaskState.Failed)
                                this.toolStartTask.Enabled = false;
                            else
                                this.toolStartTask.Enabled = true;

                            this.toolStopTask.Enabled = false;

                            this.toolCopyTask.Enabled = true;

                            if (!IsClipboardSoukeyData())
                            {
                                this.toolPasteTask.Enabled = false;
                            }
                            else
                            {
                                this.toolPasteTask.Enabled = true;
                            }

                            this.toolMenuUploadTask.Enabled = true;
                        }
                        else
                        {
                            this.toolCopyTask.Enabled = false;
                            this.toolPasteTask.Enabled = false;

                            this.toolMenuUploadTask.Enabled = false ;
                        }

                        break;
                }
            }
            catch (System.Exception)
            {

            }
        }

        //用于远程服务器任务点击之后对主窗体按钮的控制
        private void SetToolbarState1(int gridRowCount, string selectNode)
        {
            if (Program.IsConnectRemote == cGlobalParas.RegResult.Faild)
                return;

            if (Program.IsConnectRemote == cGlobalParas.RegResult.Faild || Program.IsConnectRemote ==cGlobalParas.RegResult.UnReg)
            {
                
                this.toolMenuUploadTask.Enabled = false;
                this.toolDelRemoteData.Enabled = false;
                return;
            }

            switch (selectNode)
            {
                case "nodRoot":
                    
                    this.toolMenuUploadTask.Enabled = false;
                    this.toolDelRemoteData.Enabled = false;
                    break;
                case "nodRunning":
                    
                    this.toolMenuUploadTask.Enabled = false;
                    this.toolDelRemoteData.Enabled = true;
                    break;
                case "nodComplete":
                   
                    this.toolMenuUploadTask.Enabled = false;
                    this.toolDelRemoteData.Enabled = true;
                    break;
                case "nodTask":
                   
                    this.toolMenuUploadTask.Enabled = false;
                    this.toolDelRemoteData.Enabled = true ;
                    break;
            }
        }

        private void ResetToolState()
        {
            //如果grid为空，则置按钮为原始状态
            this.toolStartTask.Enabled = false;
            this.toolStopTask.Enabled = false;
            this.toolCopyTask.Enabled = false;
            this.toolPasteTask.Enabled = false;
            //this.menuExportData.Enabled = false;
            this.toolDelTask.Enabled = false;
            this.toolCopyTask.Enabled = false;

            if (this.m_TreeMenu.SelectedNode.Name.Substring(0, 1) == "C" || this.m_TreeMenu.SelectedNode.Name == "nodTaskClass")
            {
                if (!IsClipboardSoukeyData())
                {
                    this.toolPasteTask.Enabled = false;
                }
                else
                {
                    this.toolPasteTask.Enabled = true;
                }
            }
            else
            {
                this.toolPasteTask.Enabled = false;
            }
        }

        private bool IsClipboardSoukeyData()
        {
            //判断数据是否为文本
            if (Clipboard.ContainsData(DataFormats.Text))
            {
                IDataObject cdata;
                cdata = Clipboard.GetDataObject();
                if (cdata.GetDataPresent(DataFormats.Text))
                {
                    string tInfo = cdata.GetData(DataFormats.Text).ToString();
                    if (tInfo.Length > 18)
                    {
                        if (tInfo.Substring(0, 17) == "SoukeyNetGetTask:")
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        private void toolStopTask_Click(object sender, EventArgs e)
        {
            this.m_frmRunningTask.Stop();
        }

        private void toolExportTxt_Click(object sender, EventArgs e)
        {
            this.m_frmRunningTask.ExportTxt();
        }

        private void toolExportExcel_Click(object sender, EventArgs e)
        {
            this.m_frmRunningTask.ExportExcel();
        }

        private void toolAbout_Click(object sender, EventArgs e)
        {
            frmAbout f = new frmAbout();
            f.ShowDialog();
            f.Dispose();
        }

        private void toolmenuNewTask_Click(object sender, EventArgs e)
        {
            NewTask();
        }

        private void toolmenuNewTaskClass_Click(object sender, EventArgs e)
        {
            NewTaskClass();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolDelTask_Click(object sender, EventArgs e)
        {
            //首先判断当前焦点的控件
            //然后再确定删除的是分类还是任务
            if (DelName == "treeMenu")
            {
                //删除的是分类
                this.m_TreeMenu .DelTaskClass();
            }
            else if (DelName == "GatherTask")
            {
                    this.m_TaskContent.Del();
            }
            else if (DelName == "MonitorTask")
            {
                this.m_TaskContent.Del();
            }

        }

     

        #endregion
       

        #region 改为气泡方式进行提示
        //定义一个集合来控制如果有多个消息窗口同时显示的情况


        public void ShowInfo(string Title,string Info)
        {

            InvokeMethod(this.m_TreeMenu, "ExportLog", new object[] { cGlobalParas.LogType.Info ,cGlobalParas.LogClass.System,System.DateTime.Now.ToString (),  Info + Title });

            this.notifyIcon1.ShowBalloonTip(2, Title, Info, ToolTipIcon.Info); 

        }

        #endregion

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //判断是否有frmtask存在
            foreach (Form f in Application.OpenForms)
            {
                if (f is frmTask)
                {
                    f.Activate();
                    MessageBox.Show("请先关闭采集任务编辑窗口，再退出网络矿工！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Cancel = true;
                    return;
                }
            }

            cXmlSConfig m_config=null;
            
            //初始化参数配置信息
            try
            {
                m_config = new cXmlSConfig(Program.getPrjPath());
                if (m_config.ExitIsShow == true)
                {
                    frmClose fc = new frmClose();
                    fc.RExitPara = new frmClose.ReturnExitPara(GetExitPara);
                    if (fc.ShowDialog() == DialogResult.Cancel)
                    {
                        fc.Dispose();
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
                        if (m_ePara == cGlobalParas.ExitPara.MinForm)
                        {
                            fc.Dispose();
                            e.Cancel = true;
                            this.Hide();
                            return;
                        }
                    }
                }
                else
                {
                    //判断是直接退出还是最小化窗体
                    if (m_config.ExitSelected == 0)
                    {
                        this.Hide();
                        e.Cancel = true;
                        return;
                    }
                }

                m_config = null;
            }
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString ("Info40"), rm.GetString ("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                frmClose fc = new frmClose();
                fc.RExitPara = new frmClose.ReturnExitPara(GetExitPara);
                if (fc.ShowDialog() == DialogResult.Cancel)
                {
                    fc.Dispose();
                    e.Cancel = true;
                    return;
                }
                else
                {
                    if (m_ePara == cGlobalParas.ExitPara.MinForm)
                    {
                        fc.Dispose();
                        e.Cancel = true;
                        this.Hide();
                        return;
                    }
                }
            }

           

            //判断是否存在运行的任务
            if (this.m_frmRunningTask.m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count != 0)
            {
                if (MessageBox.Show("当前有正在运行的采集任务，如果退出，则无法保存已经采集的数据，是否继续退出系统？", "网络矿工 询问",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    while (this.m_frmRunningTask.m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count > 0)
                    {
                        Int64 TaskID = this.m_frmRunningTask.m_GatherControl.TaskManage.TaskListControl.RunningTaskList[0].TaskID;

                        //销毁事件关联
                        this.m_frmRunningTask.m_GatherControl.TaskManage.TaskEventDispose(this.m_frmRunningTask.m_GatherControl.TaskManage.TaskListControl.RunningTaskList[0]);

                        this.m_frmRunningTask.m_GatherControl.Abort(this.m_frmRunningTask.m_GatherControl.TaskManage.TaskListControl.RunningTaskList[0]);

                        //SaveGatherTempData(TaskID);
                    }
                }
            }

            //因为采集和发布的结构不同，所以，在判断发布时有些累赘
            for (int i = 0; i < this.m_frmRunningTask.m_PublishControl.PublishManage.ListPublish.Count; i++)
            {
                if (this.m_frmRunningTask.m_PublishControl.PublishManage.ListPublish[i].pThreadState == cGlobalParas.PublishThreadState.Running)
                {
                   
                    if (MessageBox.Show("当前有正在运行的发布任务，如果退出，则发布会强制终止，导致数据丢失，是否继续？", "网络矿工 询问",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                    break;
                }
            }

            //开始销毁关于采集任务及发布任务的所有资源
            this.m_frmRunningTask.m_PublishControl.Abort();
            this.m_frmRunningTask.m_GatherControl.Dispose();
            this.m_frmRunningTask.m_PublishControl.Dispose();
            this.m_frmRunningTask.Dispose();
        }

        private void GetExitPara(cGlobalParas.ExitPara ePara)
        {
            m_ePara = ePara;
        }

        //private void toolRestartTask_Click(object sender, EventArgs e)
        //{
        //    this.m_TaskContent.ResetMultiTask();
        //}

       
        private void rMenuDelRow_Click(object sender, EventArgs e)
        {
            //DataGridView tmp = (DataGridView)this.tabControl1.SelectedTab.Controls[0].Controls[0].Controls[0];
            //tmp.Rows.Remove(tmp.SelectedRows[0]);
            //tmp = null;
        }

        private void dataTask_Enter(object sender, EventArgs e)
        {
            //DelName = this.dataTask.Name;

            ResetToolState();
        }

        public void UpdateStatebarTask()
        {
            string s = rm.GetString ("State1");

            try
            {
                //s += m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count + m_GatherControl.TaskManage.TaskListControl.StoppedTaskList.Count + m_PublishControl.PublishManage.ListPublish.Count + "-个任务  ";
                //s += m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count + "-" +  rm.GetString ("State2") + "  ";
                //s += m_GatherControl.TaskManage.TaskListControl.StoppedTaskList.Count + "-" + rm.GetString("State3") + "  ";
                //s += m_PublishControl.PublishManage.ListPublish.Count + "-" + rm.GetString("State4");

                this.toolStripStatusLabel2.Text = s;
            }
            catch (System.Exception)
            {
                //捕获错误不处理
            }

        }

        private void UpdateStatebarTaskState(string Info)
        {
            this.StateInfo.Text = Info;
        }

        private void UpdateStatebarTaskState(cGlobalParas.TaskState tState)
        {

            switch (tState)
            {
                case cGlobalParas.TaskState .UnStart :
                    this.StateInfo.Text = rm.GetString ("Label7");
                    break;
                case cGlobalParas.TaskState .Stopped :
                    this.StateInfo.Text = rm.GetString ("Label8");
                    break;
                case cGlobalParas.TaskState.Completed :
                    this.StateInfo.Text = rm.GetString ("Label9");
                    break;
                case cGlobalParas.TaskState.Failed :
                    this.StateInfo.Text = rm.GetString ("Label10");
                    break;
                case cGlobalParas.TaskState.Pause :
                    this.StateInfo.Text = rm.GetString ("Label11");
                    break;
                case cGlobalParas.TaskState.Running :
                    this.StateInfo.Text = rm.GetString ("Label12");
                    break;
                case cGlobalParas.TaskState.Started :
                    this.StateInfo.Text = rm.GetString ("Label13");
                    break;
                case cGlobalParas.TaskState.Waiting :
                    this.StateInfo.Text = rm.GetString ("Label14");
                    break;
                case cGlobalParas.TaskState.Publishing :
                    this.StateInfo.Text = rm.GetString ("Label15");
                    break;
                default:
                    break;
            }
        }
       

        //每半分钟进行一次刷新，查看当前的网络状态
        //如果网络状态发生改变需要网络连接状态
        private void timer2_Tick(object sender, EventArgs e)
        {
            //try
            //{
            //    if (cTool.IsLinkInternet() == false)
            //    {
            //        this.staIsInternet.Image = Bitmap.FromFile(Program.getPrjPath() + "img\\a08.gif");

            //        this.staIsInternet.Text = rm.GetString ("State6");

            //        //如果检测到离线则停止当前正在运行的任务

            //    }
            //    else
            //    {
            //        this.staIsInternet.Image = Bitmap.FromFile(Program.getPrjPath() + "img\\a07.gif");
            //        this.staIsInternet.Text = rm.GetString ("State5");

            //        //如果检测到在线，则启动已经停止的需要采集的任务
            //    }
            //}
            //catch (System.Exception )
            //{
            //    //捕获错误不进行处理
            //}

            if (this.m_CloseComputer == true && m_AutoClose==true)
            {
                bool isClose = true;
                if (this.m_frmRunningTask.m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count != 0)
                {
                    isClose = false;
                }

                //因为采集和发布的结构不同，所以，在判断发布时有些累赘
                for (int i = 0; i < this.m_frmRunningTask.m_PublishControl.PublishManage.ListPublish.Count; i++)
                {
                    if (this.m_frmRunningTask.m_PublishControl.PublishManage.ListPublish[i].pThreadState == cGlobalParas.PublishThreadState.Running)
                    {
                        isClose = false;
                        break;
                    }
                }

                if (isClose == true)
                {
                    System.Diagnostics.Process.Start("shutdown", "-s -t 0");
                }
            }
            else if (this.m_CloseComputer == true && m_AutoClose == false)
            {
                //系统自动维护m_AutoClose标记
                if (this.m_frmRunningTask.m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count != 0)
                {
                    m_AutoClose = true;
                    return;
                }

                //因为采集和发布的结构不同，所以，在判断发布时有些累赘
                for (int i = 0; i < this.m_frmRunningTask.m_PublishControl.PublishManage.ListPublish.Count; i++)
                {
                    if (this.m_frmRunningTask.m_PublishControl.PublishManage.ListPublish[i].pThreadState == cGlobalParas.PublishThreadState.Running)
                    {
                        m_AutoClose = true;
                        return;
                    }
                }

            }
        }


        


        //处理任务发布完成的工作
        

        private void toolMenuVisityijie_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.netminer.cn"); 
        }


        private void MenuOpenMainfrm_Click(object sender, EventArgs e)
        {
            this.Visible = true;

            this.WindowState = FormWindowState.Maximized;
            this.Activate();
        }

        private void MenuCloseSystem_Click(object sender, EventArgs e)
        {

            frmDispose fc = new frmDispose();
            fc.RExitPara = new frmDispose.ReturnExitPara(GetExitPara);
            if (fc.ShowDialog() == DialogResult.Cancel)
            {
                fc.Dispose();
                return;
            }
            else
            {
                if (m_ePara == cGlobalParas.ExitPara.MinForm)
                {
                    fc.Dispose();
                    this.Hide();
                    return;
                }
            }
          

            this.Dispose ();
        }

       
        public void ImportTask()
        {
            //支持同时导入多个任务

            this.openFileDialog1.Title = rm.GetString ("Info44");

            openFileDialog1.InitialDirectory = Program.getPrjPath() + "tasks";
            openFileDialog1.Filter = "SoukeyMiner Task Files(*.smt)|*.smt|XML File(*.xml)|*.xml";

            if (this.openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            string FileName = this.openFileDialog1.FileName;
            string TaskClass = this.m_TreeMenu.treeMenu.SelectedNode.Text;
            string TaskPath = this.m_TreeMenu.treeMenu.SelectedNode.Tag.ToString();
            string NewFileName = "";

            //验证任务格式是否正确
            try
            {
                oTask t = new oTask(Program.getPrjPath());
                t.LoadTask(FileName);

                if (t.TaskEntity.TaskName != "")
                {
                    NewFileName = t.TaskEntity.TaskName + ".smt";
                }

                TaskPath = Program.getPrjPath() + TaskPath;
                if (NewFileName == "")
                {
                    NewFileName = "task" + DateTime.Now.ToFileTime().ToString() + ".smt";
                }


                if (FileName == TaskPath + "\\" + NewFileName)
                {
                    MessageBox.Show(rm.GetString("Info45"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                bool isExistTask = false;
                string TargetFileName = TaskPath + "\\" + NewFileName;

                if (System.IO.File.Exists(TargetFileName))
                {
                    isExistTask = true ;
                    if (MessageBox.Show("已经存在相同名称的采集任务，是否覆盖？", "网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;
                }

                try
                {
                    System.IO.File.Copy(FileName, TaskPath + "\\" + NewFileName,true);
                }
                catch (System.IO.IOException ex)
                {
                    t = null;
                    MessageBox.Show("导入任务发生错误：" + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                //如果导入一个已经存在的采集任务，则不插入索引文件
                if (isExistTask == false)
                {
                    //插入任务索引文件
                    oTaskIndex ti = new oTaskIndex(Program.getPrjPath(),TaskPath+ "\\index.xml");
                    eTaskIndex ei = new eTaskIndex();
                    ei.ID = 0;
                    ei.TaskName = t.TaskEntity.TaskName;
                    ei.TaskType = t.TaskEntity.TaskType;
                    ei.TaskRunType = t.TaskEntity.RunType;
                    ei.TaskState = cGlobalParas.TaskState.UnStart;
                    ei.PublishType = t.TaskEntity.ExportType;
                    ei.ExportFile = t.TaskEntity.ExportFile;
                    ei.WebLinkCount = t.TaskEntity.UrlCount;
                    ti.InsertTaskIndex(ei);
                    ti.Dispose();
                    ti = null;
                }

                t = null;

                MessageBox.Show(rm.GetString ("Info47") + TaskClass, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (NetMinerException )
            {
                if (MessageBox.Show(rm.GetString("Quaere17"),  rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    frmUpgradeTask fu = new frmUpgradeTask(FileName);
                    fu.ShowDialog();
                    fu.Dispose();
                    fu = null;
                }

                return;
            }

            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info48") + ex.Message, rm.GetString ("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void AddTreeNode(ref TreeNode tNode,string[] tClasses,string tClassID, int Level)
        {
            string cID = tClassID.Substring(0,(Level+1) * 2);

            oTaskClass cTclass = new oTaskClass(Program.getPrjPath());
            TreeNode newNode = new TreeNode();
            newNode.Tag = cTclass.GetTaskClassPathByID(int.Parse (cID));
            newNode.Name = "C" + cID;
            newNode.Text = tClasses[Level];
            newNode.ImageIndex = 0;
            newNode.SelectedImageIndex = 0;

            tNode.Nodes.Add(newNode);       
            newNode = null;

            if(tClasses.Length >Level+1)
            {
                AddTreeNode(ref tNode, tClasses, tClassID, Level + 1);
            }
        }

        private void toolMenuAbout_Click(object sender, EventArgs e)
        {
            frmAbout f = new frmAbout();
            f.ShowDialog();
            f.Dispose();
        }

        private void sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.netminer.cn"); 
        }

        private void menuMailto_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:soukeyminer@gmail.com"); 
        }

        private void toolManageDict_Click(object sender, EventArgs e)
        {
            frmDict dfrm = new frmDict();
            dfrm.ShowDialog();
            dfrm.Dispose();
        }

        private void cmdCloseInfo_Click(object sender, EventArgs e)
        {
            //this.splitContainer3.SplitterDistance = this.splitContainer3.Height ;
            //this.toolLookInfo.Checked = false;
        }

    

        private void toolMenuUpdate_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Program.getPrjPath() + "\\upgrade\\autoupdate.exe"); 
        }
      
        private void toolWebbrowser_Click(object sender, EventArgs e)
        {
            frmBrowser fweb = new frmBrowser();
            fweb.getFlag = 4;
            fweb.ShowDialog();
            fweb.Dispose();
        }

        private void toolMenuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolMenuNewTaskPlan_Click(object sender, EventArgs e)
        {
            NewPlan();
        }


        private void rmenuDelPlan_Click(object sender, EventArgs e)
        {
            //DelPlan();
        }

        private void toolUpgradeTask_Click(object sender, EventArgs e)
        {
           
            
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //设置界面及线程语言
            Thread.CurrentThread.CurrentUICulture =Program.cLanguage;
            Thread.CurrentThread.CurrentCulture = Program.cLanguage;

            //Icon icon2 = new Icon("d:\\sominer\\images\\logo.ico");
            //ResourceManager r = new ResourceManager(SoukeyNetget.

            Icon ico1 = Properties.Resources.logo;
            Icon ico2 = Properties.Resources.logo16;
            this.notifyIcon1.Icon = ico2;
            this.Icon = ico1;

            cXmlSConfig m_config=null;
            
            //初始化参数配置信息
            try
            {
                m_config = new cXmlSConfig(Program.getPrjPath());

                if (m_config.IsFirstRun == true)
                {
                    frmFirst fh = new frmFirst();
                    fh.ShowDialog();
                    fh.Dispose();
                    fh = null;
                    //m_config.IsFirstRun = false;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString ("Info61") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            m_config = null;

            //则把信息提取出来，进行定时验证
            //cVersion v = new cVersion(Program.getPrjPath());
            //v.ReadRegisterInfo();
            //m_RegInfo = v.RegisterInfo;
            //v = null;

            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                //表示版本已经激活，如果版本已经激活，则定期向官方网站发送消息，从而验证版本的合法性
                //启动验证的定时器，由此定时器进行后台处理，如果检测为非法，则强制系统退出

                //启动合法性验证，定期去官网验证
                m_VerifyEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_VerifyEngine_CallBack), null, 12000, 1200000);
                m_IsVerifyFlag = false;

                this.Text += " [" + rm.GetString("Info225") + "]";
                this.toolMenuRegister.Enabled = false;
            }
            else if (Program.SominerVersion == cGlobalParas.VersionType.Free)
            {
                TimeSpan ts = Program.g_EndServiceDate - DateTime.Now  ;
                int d = ts.Days;

                this.Text += " [" + rm.GetString("Info226") + " " + d.ToString () + rm.GetString ("Day") + "]";

                ////启动试用期时间验证，每隔5分钟检测一次，并且提示用户此为试用版本，并中断正在运行的任务
                //m_VerifyEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_VerifyEngine_Date_CallBack), null, 12000, 300000);
                //m_IsVerifyFlag = false;
                //启动合法性验证，定期去官网验证
                m_VerifyEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_VerifyEngine_CallBack), null, 12000, 1200000);
                m_IsVerifyFlag = false;
            }

            //根据版本设置窗体显示
            //if (Program.GetCurrentVersion () == cGlobalParas.VersionType.Personal)
            //{
            //    this.Text = "网络矿工【SoukeyMiner】数据采集软件 " + Program.SominerVersionPersonal + " 致力于专业数据采集服务";

            //}

            if (Program.GetCurrentVersion() == cGlobalParas.VersionType.Enterprise)
            {
                this.Text = "网络矿工【SoukeyMiner】数据采集软件 " + Program.SominerVersionEnter + " 致力于专业数据采集服务";

            }
            else if (Program.GetCurrentVersion() == cGlobalParas.VersionType.Ultimate)
            {
                this.Text = "网络矿工【SoukeyMiner】数据采集软件 " + Program.SominerVersionUltimate + " 致力于专业数据采集服务";
            }
            else if (Program.GetCurrentVersion() == cGlobalParas.VersionType.Program)
            {
                this.Text = "网络矿工【SoukeyMiner】数据采集软件 " + Program.SominerVersionProgram + " 致力于专业数据采集服务";
            }
            else if(Program.GetCurrentVersion()==cGlobalParas.VersionType.Cloud)
            {
                this.Text="网络矿工【SoukeyMiner】数据采集软件 " + Program.SominerVersionCloud + " 致力于专业数据采集服务";
            }
            else
            {
                this.Text = "网络矿工【SoukeyMiner】数据采集软件 " + Program.SominerVersionFree + " 致力于专业数据采集服务";
            }
            
 
            //根据远程服务来进行相关的菜单设置
            if (Program.IsConnectRemote == cGlobalParas.RegResult.Faild || Program.IsConnectRemote == cGlobalParas.RegResult.UnReg)
            {
                if (Program.IsConnectRemote == cGlobalParas.RegResult.UnReg)
                {
                    this.toolManageRemoteServer.Enabled = false ;
                    this.toolmenuRegRemote.Enabled = true;
                }
                else
                {
                    this.toolmenuRegRemote.Enabled = false;
                    this.toolManageRemoteServer.Enabled = false;
                }

                this.toolRemotePlan.Enabled = false;
                
                this.toolDelRemoteData.Enabled = false;
                this.toolMenuUploadTask.Enabled = false;
                this.toolUploadDict.Enabled = false;
            }
            else
            {
                this.toolManageRemoteServer.Enabled = true ;
                this.toolmenuRegRemote.Enabled = false ;
                this.toolRemotePlan.Enabled = true;
                
                this.toolDelRemoteData.Enabled = true;
                this.toolMenuUploadTask.Enabled = true;
                this.toolUploadDict.Enabled = true;

                //localhost.NetMinerWebService sweb = new localhost.NetMinerWebService();
                //sweb.Url = Program.ConnectServer + "/NetMiner.WebService.asmx";

                //if (Program.g_IsAuthen == true)
                    //sweb.Credentials = new System.Net.NetworkCredential(Program.g_WindowsUser, Program.g_WindowsPwd);


                //cGlobalParas.ServerState sState = (cGlobalParas.ServerState)sweb.GetServerState();
                //if (sState == cGlobalParas.ServerState.Running)
                //{
                //    this.toolMenuStartServer.Enabled = false;
                //    this.toolMenuStopServer.Enabled = true;
                //}
                //else if (sState == cGlobalParas.ServerState.Stopped)
                //{
                //    this.toolMenuStartServer.Enabled = true;
                //    this.toolMenuStopServer.Enabled = false;
                //}
                //else if (sState == cGlobalParas.ServerState.UnSetup)
                //{
                //    this.toolMenuStartServer.Enabled = false;
                //    this.toolMenuStopServer.Enabled = false;
                //}
            }
        }

        private void toolmenuConfig_Click(object sender, EventArgs e)
        {
            frmConfig fc = new frmConfig();
            fc.ShowDialog();
            fc.Dispose();
        }

        private void toolCopyTask_Click(object sender, EventArgs e)
        {
            this.m_TaskContent.CopyTask();
        }

        private void dataTask_CopyTask(object sender, CopyTaskEventArgs e)
        {
            //CopyTask();
        }

        private void dataTask_PasteTask(object sender, PasteTaskEventArgs e)
        {
            //PasteTask();
        }


        private void toolUrlEncoding_Click(object sender, EventArgs e)
        {
            frmUrlEncoding f = new frmUrlEncoding();
            f.Show(this);
            //Application.DoEvents();
            //f.Dispose();
        }

        private void toolmenuAuto_Click(object sender, EventArgs e)
        {
            try
            {
                cXmlSConfig Config = new cXmlSConfig(Program.getPrjPath());
                Config.CurrentLanguage =cGlobalParas.CurLanguage.Auto ;
                Config = null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show (ex.Message ,rm.GetString ("MessageboxError"),MessageBoxButtons.OK ,MessageBoxIcon.Error );
                return ;
            }

            MessageBox.Show(rm.GetString("Info113"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolmenuEnglish_Click(object sender, EventArgs e)
        {
             try
            {
                cXmlSConfig Config = new cXmlSConfig(Program.getPrjPath());
                Config.CurrentLanguage = cGlobalParas.CurLanguage.enUS;
                Config = null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show (ex.Message ,rm.GetString ("MessageboxError"),MessageBoxButtons.OK ,MessageBoxIcon.Error );
                return ;
            }

            MessageBox.Show(rm.GetString("Info113"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolmenuCHS_Click(object sender, EventArgs e)
        {
             try
            {
                cXmlSConfig Config = new cXmlSConfig(Program.getPrjPath());
                Config.CurrentLanguage = cGlobalParas.CurLanguage.zhCN;
                Config = null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show (ex.Message ,rm.GetString ("MessageboxError"),MessageBoxButtons.OK ,MessageBoxIcon.Error );
                return ;
            }

            MessageBox.Show(rm.GetString("Info113"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolBrowserData_Click(object sender, EventArgs e)
        {
            this.m_frmRunningTask. BrowserMultiData();
        }

     

        private void toolDataProcess_Click(object sender, EventArgs e)
        {
            RunDataPublish();
        }

        public void RunDataPublish()
        {
            try
            {
                System.Diagnostics.Process.Start("SoukeyDataPublish.exe");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void toolMenuRegister_Click(object sender, EventArgs e)
        {
            frmRegister f = new frmRegister();
            f.RRResult = new frmRegister.ReturnRResult(this.GetrResult);
            f.ShowDialog();
            f.Dispose();

        }

        private void GetrResult(cGlobalParas.RegisterResult rResult)
        {
            m_rResult = rResult;
        }


        //定时去官网去验证版本的合法性
        private void m_VerifyEngine_CallBack(object State)
        {
            if (!m_IsVerifyFlag)
            {
                m_IsVerifyFlag = true;

                cVersion v = new cVersion(Program.getPrjPath());

                //定期验证在线合法性
                cGlobalParas.VerifyUserd IsUsed = (cGlobalParas.VerifyUserd)v.VerifyUerd(Program.g_RegInfo.User, Program.g_RegInfo.Keys, Program.g_RegInfo.CpuID, Program.SominerVersionCode);

                v = null;

                if (IsUsed != cGlobalParas.VerifyUserd.Available)
                {
                    //在此强制系统退出
                    InvokeMethod(this, "AutoExit", null);
                }

                m_IsVerifyFlag = false;
            }
        }

        public void StopAllTask()
        {
            this.m_frmRunningTask.StopAllTask();
        }

        public void AutoExit()
        {
            frmAutoExit f = new frmAutoExit();
            f.Show();
        }

        private void toolRegex_Click(object sender, EventArgs e)
        {
            bool IsStarted = false;

            foreach (Form f in Application.OpenForms )
            {
                if (f is frmRegex )
                {
                    IsStarted =true ;
                    f.Activate();
                    break ;
                }
            }

            if (IsStarted == false)
            {
                frmRegex f1 = new frmRegex();
                f1.Show();
            }
        }

        private void toolMenuOnlineHelp_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("http://www.netminer.cn/help/index.html");
            System.Diagnostics.Process.Start("SoMinerHelp.chm");
        }
     
        private void toolExportCSV_Click(object sender, EventArgs e)
        {
            //ExportCSV();
        }

        public void NewTaskClass()
        {
            this.m_TreeMenu.NewTaskClass();
        }

        public void NewTask()
        {
            

          
                    NewTaskByNormal();
              

        }

        public void CheckIsCloseComputer(bool isClose)
        {
            this.m_CloseComputer = isClose;
            if (this.m_CloseComputer == false)
                m_AutoClose = false;
        }

        private void SelectedNewTask(cGlobalParas.NewTaskType mType)
        {
            if (mType == cGlobalParas.NewTaskType.Wizard)
            {
                NewTaskByWizard();
            }
            else if (mType == cGlobalParas.NewTaskType.Normal)
            {
                NewTaskByNormal();
            }
            else if (mType == cGlobalParas.NewTaskType.Cancel)
            {
                return;
            }

        }

        private void NewTaskByNormal()
        {
            string TClass = string.Empty ;
            string tClassPath = string.Empty ;

            if (this.m_TreeMenu.SelectedNode.Name.Substring(0, 1) == "C")
            {
                //表示选择的是分类节点
                TClass = this.m_TreeMenu.SelectedNode.Text;
                tClassPath = this.m_TreeMenu.SelectedNode.Tag.ToString();
            }
            else
            {
                TClass = "";
                tClassPath = "tasks"; 
            }

            frmWaiting fWait = new frmWaiting("正在启动采集任务窗体，请等待...");
            new Thread((ThreadStart)delegate
            {
                Application.Run(fWait);
            }).Start();
            //fWait.Show(this);


            frmTask fTask = new frmTask();
            fTask.NewTask(tClassPath, TClass);
            fTask.FormState = cGlobalParas.FormState.New;
            //fTask.RShowWizard = this.ShowTaskWizard;
            fTask.rTClass = refreshNode;

            if (fWait.IsHandleCreated)
            {
                try
                {
                    fWait.Invoke((EventHandler)delegate { fWait.Close(); });
                }
                catch { }
            }
            fWait = null;


            fTask.Show();
            //fTask.Dispose();
        }

        private void NewTaskByWizard()
        {
            //string TClass = "";
            //if (this.m_TreeMenu.SelectedNode.Name.Substring(0, 1) == "C")
            //{
            //    //表示选择的是分类节点
            //    TClass = this.m_TreeMenu.SelectedNode.Text;
            //}

            //frmTaskWizard f = new frmTaskWizard();

            //f.NewTask(TClass);
            //f.FormState = cGlobalParas.FormState.New;

            //f.RShowNormal = this.ShowTaskNormal;
            //f.ShowDialog();
            //f.Dispose();
        }

        private void ShowTaskNormal(bool IsShow)
        {
            if (IsShow == true)
            {
                NewTaskByNormal ();
            }
        }

        private void ShowTaskWizard(bool IsShow)
        {
            if (IsShow == true)
            {
               NewTaskByWizard ();
            }
        }

        public void refreshNode(string TClass)
        {
            this.m_TreeMenu.refreshNode(TClass);
        }

        public  void NewPlan()
        {
            frmTaskPlan fTaskPlan = new frmTaskPlan();
            fTaskPlan.FormState = cGlobalParas.FormState.New;
            fTaskPlan.ShowDialog();
            fTaskPlan.Dispose();

            //判断如果是选择的任务计划节点，则进行刷新
            if (this.m_TreeMenu.SelectedNode.Name== "nodPlanRunning")
            {
                this.m_TaskContent.LoadTaskPlan(this.m_TreeMenu.SelectedNode);
            }

        }

        private void toolLookInfo_Click(object sender, EventArgs e)
        {
            
                this.m_TreeMenu.ShowLog(false );
                //this.toolLookInfo.Checked = false;
            

        }

        private void toolmenuNewRadarRule_Click(object sender, EventArgs e)
        {
            NewRadar();
        }

        public void NewRadar()
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
             Program.SominerVersion == cGlobalParas.VersionType.Enterprise )
            {
                frmRader f = new frmRader();
                f.FormState = cGlobalParas.FormState.New;
                f.ShowDialog();
                f.Dispose();

                this.m_TaskContent.LoadRadarRule(this.m_TreeMenu.SelectedNode);
            }
            else
            {
                MessageBox.Show("当前版本不支持网络雷达（数据监控）功能，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.toolmenuSilentRun.Checked = false;
                return;
            }
        }

        private void toolRadar_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
              Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                if (m_IsRadarStarted == true)
                {
                    SetRadarState(false);

                }
                else
                {
                    SetRadarState(true);
                }
            }
            else
            {
                MessageBox.Show("当前版本不支持网络雷达（数据监控）功能，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.toolmenuSilentRun.Checked = false;
                return;
            }
        }

        public void SetRadarState(bool IsStart)
        {
            if (IsStart == false)
            {
                this.m_frmRunningTask.StopRadar();
            }
            else
            {
                this.m_frmRunningTask.StartRadar();
            }
        }

        private void toolMenuOpenHome_Click(object sender, EventArgs e)
        {
            bool IsHelp = false;
            this.m_Help.Activate();
        }

        //雷达监控预警，通过托盘图标进行闪烁
        public void MonitorWarningByTrayIcon(string str)
        {
            m_IsIconFlicker = true;
            this.trayiconTime.Enabled = true;
            this.notifyIcon1.Text = str;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_IsIconFlicker == false)
                {
                    this.Visible = true;

                    this.WindowState = FormWindowState.Maximized;

                    this.Activate();

                    if (m_IsIconFlicker == false)
                        return;
                }
                else
                {

                    m_IsIconFlicker = false;
                    this.trayiconTime.Enabled = false;

                    //this.m_Monitor.Activate();

                    Icon icon1 = Properties.Resources.logo16;

                    this.notifyIcon1.Icon = icon1;


                    //frmMonitorInfo f = new frmMonitorInfo(this.notifyIcon1.Text);
                    //if (f.ShowDialog() == DialogResult.OK)
                    //{
                    //    this.m_Monitor.Activate();
                    //}
                    //f.Dispose();

                    //m_IsIconFlicker = false;

                }

            }
        }


        //定义一个值，表示当前托盘是空白还是图标
        private bool m_IsIcon = true;

        //控制托盘图片进行闪烁
        private void trayiconTime_Tick(object sender, EventArgs e)
        {
            if (m_IsIcon == true)
            {
                Icon icon2 = Properties.Resources.nullico;

                this.notifyIcon1.Icon = icon2;
                m_IsIcon = false;
            }
            else
            {
                Icon icon1 = Properties.Resources.logo16;
                this.notifyIcon1.Icon = icon1;
                m_IsIcon = true;
            }
        }



        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //在此销毁所有内容
            this.m_Help.Close();
            this.m_Help.Dispose();

            //this.m_Monitor.Close();
            //this.m_Monitor.Dispose();

            this.m_TaskContent.Close();
            this.m_TaskContent.Dispose();

            this.m_frmRunningTask.Close();
            this.m_frmRunningTask.Dispose();
            
            this.m_TreeMenu.Close();
            this.m_TreeMenu.Dispose();

            //判断是否有netminer.webengine,如果有，销毁掉
            if (System.Diagnostics.Process.GetProcessesByName("NetMiner.WebEngine").Length >= 1)
            {
                Process instance = RunningInstance();
                if (instance == null)
                {
                    //启动web引擎
                    //System.Diagnostics.Process.Start("NetMiner.WebEngine.exe", "yijie");
                    //System.Threading.Thread.Sleep(3000);
                }
                else
                    instance.Kill();
            }
            else
            {
                //System.Diagnostics.Process.Start("NetMiner.WebEngine.exe", "yijie");
                //System.Threading.Thread.Sleep(3000);
            }
        }

        private Process RunningInstance()
        {

            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName("NetMiner.WebEngine");
            foreach (Process process in processes)
            {
                
                        return process;
                
            }
            return null;
        }

        private void toolmenuImportMoreTask_Click(object sender, EventArgs e)
        {
            frmImportTask f = new frmImportTask();
            f.ShowDialog();
            f.Dispose();
        }

        private void toolmenuImportSingleTask_Click(object sender, EventArgs e)
        {
            ImportTask();
        }

        private void toolmenuUpgradeTask_Click(object sender, EventArgs e)
        {
            frmUpgradeTask fUt = new frmUpgradeTask();
            fUt.ShowDialog();
            fUt.Dispose();
        }

        private void toolmenuBackupTask_Click(object sender, EventArgs e)
        {
            frmBackupTask f = new frmBackupTask();
            f.ShowDialog();
            f.Dispose();
        }
      
        private void toolStartLog_Click(object sender, EventArgs e)
        {
            RunLogLook();
        }

        public void RunLogLook()
        {
            try
            {
                System.Diagnostics.Process.Start("SoukeyLog.exe");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void StaRadar_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
             Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                if (m_IsRadarStarted == true)
                {
                    SetRadarState(false);

                }
                else
                {
                    SetRadarState(true);
                }
            }
            else
            {
                MessageBox.Show("当前版本不支持网络雷达（数据监控）功能，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.toolmenuSilentRun.Checked = false;
                return;
            }
        }

        //private void staRunType_Click(object sender, EventArgs e)
        //{
        //    if (Program.SominerVersion == cGlobalParas.VersionType.Union ||
        //        Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
        //       Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
        //    {
        //        //if (Program.IsSilent == false)
        //        //{
        //        //    if (MessageBox.Show(rm.GetString("Info270"), rm.GetString("MessageboxQuaere"),
        //        //        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
        //        //    {
        //        //        this.toolmenuSilentRun.Checked = false;
        //        //        return;
        //        //    }

        //        //    this.staRunType.Image = Bitmap.FromFile(Program.getPrjPath() + "img\\A66.gif");
        //        //    this.staRunType.Text = rm.GetString("Info272");


        //        //    Program.IsSilent = true;

        //        //    this.toolmenuSilentRun.Checked = true;

        //        //}
        //        //else if (Program.IsSilent == true)
        //        //{
        //        //    this.staRunType.Image = Bitmap.FromFile(Program.getPrjPath() + "img\\A65.gif");
        //        //    this.staRunType.Text = rm.GetString("Info271");

        //        //    MessageBox.Show(rm.GetString("Info273"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);

        //        //    Program.IsSilent = false;

        //        //    this.toolmenuSilentRun.Checked = false;
        //        //}
        //    }
        //    else
        //    {
        //        MessageBox.Show("当前版本不支持静默运行模式，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        //this.toolmenuSilentRun.Checked = false;
        //        return;
        //    }
        //}

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmFindTask f = new frmFindTask();
            f.FTask = this.FindTask;
            f.Show();

            m_FindStart = true;
        }

        private bool LocateTask(string strKey)
        {
            bool isFind = false;

            treeMenuMouseClick(this.m_TreeMenu.SelectedNode);

            if (this.m_TreeMenu.SelectedNode.Name == "nodRemoteTaskClass")
                return isFind;

            int startIndex = 0;
            if (m_FindIndex>-1)
            {
                //表示已经找到的数据
                startIndex = m_FindIndex + 1;
                m_FindIndex = -1;
            }
            for (int j = startIndex; j < this.m_TaskContent.dataTask.Rows.Count; j++)
            {
                if (this.m_TaskContent.dataTask.Rows[j].Cells[4].Value.ToString().IndexOf (strKey,StringComparison.CurrentCultureIgnoreCase)>-1)
                {
                    this.m_TaskContent.dataTask.Rows[j].Selected = true;
                    isFind = true;
                    m_FindIndex = j;
                    break;
                }
                else
                {
                    this.m_TaskContent.dataTask.Rows[j].Selected = false ;
                }
                        
            }
            return isFind;

            //for (int j = 0; j < this.m_TaskContent.dataTask.Rows.Count; j++)
            //{
            //    if (TaskID == this.m_TaskContent.dataTask.Rows[j].Cells[1].Value.ToString())
            //    {
            //        this.m_TaskContent.dataTask.Rows[j].Selected = true;
            //        break;
            //    }
            //}
        }

        private void FindTask(string strKey)
        {

            //此次查询的计数指针
            TreeNode tNode = null;
            if (m_FindStart == true)
            {
                tNode = this.m_TreeMenu.treeMenu.Nodes["nodTaskClass"];
                m_FindStart = false;
            }
            else
                tNode = this.m_TreeMenu.treeMenu.SelectedNode;

            FindTask(tNode, strKey,true);

        }

        private bool FindTask(TreeNode tNode,string strKey,bool isUp)
        {
            bool isFind = false;

            //获取当前选中的条目
            this.m_TreeMenu.SelectedNode = tNode;
            Application.DoEvents();
            
            if (tNode.Name == "nodTaskClass")
            {
                FindTask(tNode.Nodes[0], strKey, false);
            }
            else
            {
                
                isFind = LocateTask(strKey);
                if (isFind == true)
                    return true;

                //搜索完成之后，看是否还包含子级分类
                if (tNode.Nodes.Count > 0)
                {
                    isFind=FindTask(tNode.Nodes[0], strKey, false);
                    if (isFind == true)
                        return true;
                }


                TreeNode nNode = tNode.NextNode;
                if (nNode != null)
                {
                    isFind =FindTask(nNode, strKey, isUp);
                    if (isFind == true)
                        return true;
                }
                else
                {
                    if (isUp == true)
                    {
                        TreeNode parentNode = tNode.Parent;
                        if (parentNode == null || parentNode.Name == "nodTaskClass")
                            return false;
                        else
                        {
                            TreeNode nNode1 = parentNode.NextNode;
                            if (nNode1 != null)
                            {
                                isFind = FindTask(nNode1, strKey, isUp);
                                if (isFind == true)
                                    return true;
                            }
                        }
                    }
                }
            }

            return isFind;
        }

        private void toolOcr_Click(object sender, EventArgs e)
        {
            //frmOCR f = new frmOCR();
            //f.ShowDialog();
            //f.Dispose();

        }

        #region 设置菜单的风格
        public void SetStyle(string Name)
        {
            Color HaloColor = Color.White;
            switch (Name)
            {
                case "Dark":
                    this.BackColor = Color.FromArgb(88, 77, 69);
                    HaloColor = Color.FromArgb(200, 200, 200);
                    SetBase(87, 61, 53, HaloColor);
                    break;
                case "Nature":
                    this.BackColor = Color.FromArgb(78, 127, 52);
                    HaloColor = Color.FromArgb(254, 209, 94);
                    SetBase(73, 118, 46, HaloColor);
                    break;
                case "Dawn":
                    this.BackColor = Color.FromArgb(177, 108, 45);
                    SetBase(172, 99, 39, Color.FromArgb(254, 209, 94));
                    break;
                case "Corn":
                    this.BackColor = Color.FromArgb(230, 193, 106);
                    SetBase(225, 184, 100, Color.FromArgb(191, 219, 255));
                    break;
                case "Chocolate":
                    this.BackColor = Color.FromArgb(87, 54, 34);
                    SetBase(82, 45, 28, Color.FromArgb(232, 80, 90));
                    break;
                case "Navy":
                    this.BackColor = Color.FromArgb(88, 121, 169);
                    SetBase(84, 112, 163, Color.FromArgb(254, 209, 94));
                    break;
                case "Ice":
                    this.BackColor = Color.FromArgb(235, 243, 236);
                    SetBase(228, 234, 230, Color.FromArgb(254, 209, 94));
                    break;
                case "Vanilla":
                    this.BackColor = Color.FromArgb(233, 243, 213);
                    SetBase(228, 234, 207, Color.FromArgb(254, 209, 94));
                    break;
                case "Canela":
                    this.BackColor = Color.FromArgb(235, 226, 197);
                    SetBase(228, 217, 191, Color.FromArgb(254, 209, 94));
                    break;
                case "Cake":
                    this.BackColor = Color.FromArgb(235, 213, 197);
                    SetBase(228, 204, 198, Color.FromArgb(254, 209, 94));
                    break;
                default:
                    this.BackColor = Color.FromArgb(191, 219, 255);
                    SetBase(215, 227, 242, Color.FromArgb(254, 209, 94));
                    break;
            }


        }

        public void SetBase(int R, int G, int B, Color HaloColor)
        {
            this.SuspendLayout();
            foreach (Control control in this.panel1.Controls)
            {
                if (typeof(DoeasyControl.Ribbon.TabStrip) == control.GetType())
                {
                    ((DoeasyControl.Ribbon.TabStripProfessionalRenderer)((DoeasyControl.Ribbon.TabStrip)control).Renderer).HaloColor = HaloColor;
                    ((DoeasyControl.Ribbon.TabStripProfessionalRenderer)((DoeasyControl.Ribbon.TabStrip)control).Renderer).BaseColor = Color.FromArgb(R + 4, G + 3, B + 3);
                    for (int i = 0; i < ((DoeasyControl.Ribbon.TabStrip)control).Items.Count; i++)
                    {
                        DoeasyControl.Ribbon.Tab _tab = (DoeasyControl.Ribbon.Tab)((DoeasyControl.Ribbon.TabStrip)control).Items[i];

                        #region Set Tab Colors
                        if (Color.FromArgb(R, G, B).GetBrightness() < 0.5)
                        {
                            try
                            {
                                _tab.ForeColor = Color.FromArgb(R + 76, G + 71, B + 66);
                            }
                            catch
                            {
                                _tab.ForeColor = Color.FromArgb(250, 250, 250);
                            }
                        }
                        else
                        {
                            try
                            {
                                _tab.ForeColor = Color.FromArgb(R - 96, G - 91, B - 86);
                            }
                            catch
                            {
                                _tab.ForeColor = Color.FromArgb(10, 10, 10);
                            }
                        }
                        #endregion
                    }

                    control.BackColor = Color.FromArgb(R - 24, G - 8, B + 12);

                }
                if (typeof(DoeasyControl.Ribbon.TabPageSwitcher) == control.GetType())
                {
                    control.BackColor = Color.FromArgb(R - 24, G - 8, B + 12);

                    foreach (Control _control in control.Controls)
                    {
                        if (typeof(DoeasyControl.Ribbon.TabStripPage) == _control.GetType())
                        {
                            ((DoeasyControl.Ribbon.TabStripPage)_control).BaseColor = Color.FromArgb(R, G, B);
                            ((DoeasyControl.Ribbon.TabStripPage)_control).BaseColorOn = Color.FromArgb(R, G, B);

                            foreach (Control __control in _control.Controls)
                            {
                                if (typeof(DoeasyControl.Ribbon.TabPanel) == __control.GetType())
                                {
                                    #region Set TabPanel Colors
                                    if (Color.FromArgb(R, G, B).GetBrightness() < 0.5)
                                    {
                                        try
                                        {
                                            __control.ForeColor = Color.FromArgb(R + 76, G + 71, B + 66);
                                        }
                                        catch
                                        {
                                            __control.ForeColor = Color.FromArgb(250, 250, 250);
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            __control.ForeColor = Color.FromArgb(R - 96, G - 91, B - 86);
                                        }
                                        catch
                                        {
                                            __control.ForeColor = Color.FromArgb(10, 10, 10);
                                        }
                                    }
                                    #endregion

                                    ((DoeasyControl.Ribbon.TabPanel)__control).BaseColor = Color.FromArgb(R, G, B);
                                    ((DoeasyControl.Ribbon.TabPanel)__control).BaseColorOn = Color.FromArgb(R + 16, G + 11, B + 6);

                                    foreach (Control ___control in __control.Controls)
                                    {
                                        if (typeof(DoeasyControl.Ribbon.RibbonButton) == ___control.GetType())
                                        {
                                            ((DoeasyControl.Ribbon.RibbonButton)___control).InfoColor = Color.FromArgb(R, G, B);

                                            DoeasyControl.Ribbon.RibbonButton _but = (DoeasyControl.Ribbon.RibbonButton)___control;

                                            #region Set Button Colors
                                            if (Color.FromArgb(R, G, B).GetBrightness() < 0.5)
                                            {
                                                try
                                                {
                                                    _but.ForeColor = Color.FromArgb(R + 76, G + 71, B + 66);
                                                }
                                                catch
                                                {
                                                    _but.ForeColor = Color.FromArgb(250, 250, 250);
                                                }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    _but.ForeColor = Color.FromArgb(R - 96, G - 91, B - 86);
                                                }
                                                catch
                                                {
                                                    _but.ForeColor = Color.FromArgb(10, 10, 10);
                                                }
                                            }
                                            #endregion

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.ResumeLayout(false);
        }
        #endregion

        //private void menuExportData_Click(object sender, EventArgs e)
        //{
        //    this.contextMenuStrip1.Show(this.menuExportData, 0, 15);
        //}

        private void toolFindTask_Click(object sender, EventArgs e)
        {
            FindTask();
        }

        private void FindTask()
        {
            frmFindTask f = new frmFindTask();
            f.FTask = this.FindTask;
            f.Show();
        }

        private void toolmenuEditData_Click(object sender, EventArgs e)
        {
            this.m_frmRunningTask.EditData();
        }

        private void toolPasteTask_Click(object sender, EventArgs e)
        {
            this.m_TaskContent.PasteTask();
        }

        public void OpenHelp()
        {
            frmTip f = new frmTip();
            f.LoadHelp("如何开始");
            f.Show();
        }

        private void toolOCR_Click_1(object sender, EventArgs e)
        {
            //frmOCR f = new frmOCR();
            //f.ShowDialog();
            //f.Dispose();
        }

        private void toolSniffer_Click(object sender, EventArgs e)
        {
            
        }

        private void toolMenuConnect_Click(object sender, EventArgs e)
        {
            frmConnectServer f = new frmConnectServer();
            f.RServer = this.GetRemoteServer;
            f.ShowDialog();
            f.Dispose();
        }

        private void GetRemoteServer(string server,bool isAuthen,string user,string pwd)
        {
            if (server.Trim() == "")
                return;

            cXmlSConfig cx = new cXmlSConfig(Program.getPrjPath());
            cx.RemoteServer = server;
            cx.IsAuthen = isAuthen;
            cx.windowsUser = user;
            cx.windowsPwd = pwd;
            cx = null;

            Program.ConnectServer = server;
            Program.g_IsAuthen = isAuthen;
            Program.g_WindowsUser = user;
            Program.g_WindowsPwd = pwd;

            //链接远程服务器
            //this.m_RemoteMenu.IniData();


            if (Program.IsConnectRemote == cGlobalParas.RegResult.Succeed)
            {
                //this.toolmenuRegRemote.Enabled = true;
                this.toolDelRemoteData.Enabled = true;

                //localhost.NetMinerWebService sweb = new localhost.NetMinerWebService();
                //sweb.Url = Program.ConnectServer + "/NetMiner.WebService.asmx";

                //if (isAuthen == true)
                //{
                //    sweb.Credentials = new System.Net.NetworkCredential(user, pwd);
                //}


                //cGlobalParas.ServerState sState = (cGlobalParas.ServerState)sweb.GetServerState();
                //if (sState == cGlobalParas.ServerState.Running)
                //{
                //    this.toolMenuStartServer.Enabled = false;
                //    this.toolMenuStopServer.Enabled = true;
                //}
                //else if (sState == cGlobalParas.ServerState.Stopped)
                //{
                //    this.toolMenuStartServer.Enabled = true;
                //    this.toolMenuStopServer.Enabled = false;
                //}
                //else if (sState == cGlobalParas.ServerState.UnSetup)
                //{
                //    this.toolMenuStartServer.Enabled = false;
                //    this.toolMenuStopServer.Enabled = false ;
                //}
            }
        }

        private void toolmenuRegRemote_Click(object sender, EventArgs e)
        {
            frmRegisterRemote f = new frmRegisterRemote();
            f.ShowDialog();
            f.Dispose();
        }

        private void toolMenuUploadTask_Click(object sender, EventArgs e)
        {
            UploadTask();
        }

        public void UploadTask()
        {
            try
            {
                this.m_TaskContent.UploadTask();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("上传任务发生错误，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void toolMenuStartRemoteGather_Click(object sender, EventArgs e)
        {
            //this.m_RServer.StartGather();
        }

        private void toolMenuStopRemoteGather_Click(object sender, EventArgs e)
        {
            //this.m_RServer.StopGather();
        }

        private void toolDelRemoteData_Click(object sender, EventArgs e)
        {
            //this.m_RServer.Del();
        }

        private void toolMenuStartServer_Click(object sender, EventArgs e)
        {
            //localhost.NetMiner.WebService sweb = new localhost.NetMiner.WebService();
            //sweb.Url = Program.ConnectServer + "/NetMiner.WebService.asmx";

            //bool isS=sweb.StartServer(Program.RegisterUser);

            //if (isS == true)
            //{
            //    this.toolMenuStartServer.Enabled = false;
            //    this.toolMenuStopServer.Enabled = true;
            //}
            //else
            //{
            //    this.toolMenuStartServer.Enabled = true;
            //    this.toolMenuStopServer.Enabled = false ;
            //}

        }

        private void toolMenuStopServer_Click(object sender, EventArgs e)
        {
            //localhost.NetMiner.WebService sweb = new localhost.NetMiner.WebService();
            //sweb.Url = Program.ConnectServer + "/NetMiner.WebService.asmx";

            //bool isS = sweb.StopServer (Program.RegisterUser);

            //if (isS == true)
            //{
            //    this.toolMenuStartServer.Enabled = true;
            //    this.toolMenuStopServer.Enabled = false ;
            //}
            //else
            //{
            //    this.toolMenuStartServer.Enabled = false;
            //    this.toolMenuStopServer.Enabled = true;
            //}
        }

        private void toolmenuPlugins_Click(object sender, EventArgs e)
        {
            frmPlugins f = new frmPlugins();
            f.ShowDialog();
            f.Dispose();
        }

        private void toolmenuSilentRun_CheckedChanged(object sender, EventArgs e)
        {

        }

        public void ClickRunningNode()
        {
            this.m_TreeMenu.treeMenu.SelectedNode = this.m_TreeMenu.treeMenu.Nodes["nodSnap"].Nodes["nodRunning"];
            treeMenuMouseClick(this.m_TreeMenu.treeMenu.Nodes["nodSnap"].Nodes["nodRunning"]);
        }

        public void ClickCompletedNode()
        {
            this.m_TreeMenu.treeMenu.SelectedNode = this.m_TreeMenu.treeMenu.Nodes["nodSnap"].Nodes["nodComplete"];
            treeMenuMouseClick(this.m_TreeMenu.treeMenu.Nodes["nodSnap"].Nodes["nodComplete"]);
        }

        private void toolmenuImportSingleTask_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void toolmenuImportMoreTask_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void toolPlugin_Click(object sender, EventArgs e)
        {
            frmPlugins f = new frmPlugins();
            f.ShowDialog();
            f.Dispose();
        }

        private void toolPublishTemplate_Click(object sender, EventArgs e)
        {
            RunPublishTemplate();
        }

        public void RunPublishTemplate()
        {
            try
            {
                System.Diagnostics.Process.Start("SoukeyPublishManage");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void sMenuFind_Click(object sender, EventArgs e)
        {
            FindTask();
        }

        private void toolmenuClearData_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.m_TaskContent.ClearData();
        }

        private void toolSynDb_Click(object sender, EventArgs e)
        {
            frmSynDB f = new frmSynDB();
            f.ShowDialog();
            f.Dispose();
        }

        private void toolRemotePlan_Click(object sender, EventArgs e)
        {
            //frmRemotePlan f = new frmRemotePlan();
            //f.ShowDialog();
            //f.Dispose();
        }

        private void toolManageRemoteServer_Click(object sender, EventArgs e)
        {
            if (this.m_MServer != null)
            {
                this.m_MServer.Dispose();
            }
            m_MServer = new frmManageRemoteServer();
            this.m_MServer.Show(this.dockPanel);
            this.m_MServer.Activate();
        }

        private void ribbonButton1_Click(object sender, EventArgs e)
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f is frmProxy)
                {
                    f.Activate();
                    return;
                }
            }

            frmProxy f1 = new frmProxy();
            f1.Show();
            
        }

        private void toolUploadDict_Click(object sender, EventArgs e)
        {
            oDict d = new oDict(Program.getPrjPath ());

            //DataView dClass = new DataView();
            

            //一个分类一个分类的上传
            //localhost.NetMinerWebService sweb = new localhost.NetMinerWebService();
            //sweb.Url = Program.ConnectServer + "/NetMiner.WebService.asmx";
            //if (Program.g_IsAuthen == true)
            //    sweb.Credentials = new System.Net.NetworkCredential(Program.g_WindowsUser, Program.g_WindowsPwd);

            IEnumerable<string> dClass = d.GetDictClass();
            foreach (string s in dClass)
            {
                string dClassName = s;
                string fileName = Program.getPrjPath() + "dict\\dic\\" + dClassName + ".dic";

                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                byte[] bytes = br.ReadBytes((int)fs.Length);
                fs.Flush();
                fs.Close();

                //bool isS = sweb.UploadDict(dClassName, bytes);
                
            }

            MessageBox.Show("字典数据上传成功！","网络矿工 信息",MessageBoxButtons.OK ,MessageBoxIcon.Information );
        }

        private void staAutoHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://robot.netminer.cn/chat/chat.aspx?key=121PF713FS193112"); 
        }

        private void toolCloudGather_Click(object sender, EventArgs e)
        {
            //this.m_Help.ExcuteFunction += this.on_ExcuteFunction;
            if (m_Cloud == null || m_Cloud.IsDisposed == true)
                m_Cloud = new frmCloudGather();

            m_Cloud.Show(this.dockPanel);
        }

        private void toolRemoteTaskClass_Click(object sender, EventArgs e)
        {
            //frmRemoteTaskClass f = new frmRemoteTaskClass();
            //f.ShowDialog();
            //f.Dispose();
        }

        /// <summary>
        /// 启动一个新的采集任务
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="TaskName"></param>
        /// <param name="taskClass"></param>
        /// <param name="taskPath"></param>
        public void Start(string taskID, string TaskName, string taskClass, string taskPath)
        {
            //string taskID = paras[0].ToString ();
            //string TaskName = paras[1].ToString();
            //string taskClass = paras[2].ToString();
            //string taskPath = paras[3].ToString();
            long TaskID = long.Parse(taskID);
            this.m_frmRunningTask.StartTask(TaskID, TaskName, taskClass, taskPath);
        }

        public void AddCompletedTask(eTaskCompleted ec)
        {
            this.m_frmCompletedTask.AddCompletedTask(ec);
        }

        private void toolMenuPublishTemplate_Click(object sender, EventArgs e)
        {
            frmSelectPublishTemplate f = new frmSelectPublishTemplate();
            f.RPType = this.CreatePublishTemplate;
            f.ShowDialog();
            f.Dispose();
        }

        private void CreatePublishTemplate(cGlobalParas.PublishTemplateType pType)
        {

            if (pType==cGlobalParas.PublishTemplateType.Web)
            {
                frmWebRule f = new frmWebRule();
                f.ShowDialog();
                f.Dispose();
            }
            else if (pType==cGlobalParas.PublishTemplateType.DB)
            {
                frmDBRule f = new frmDBRule();
                f.ShowDialog();
                f.Dispose();
            }
        }

        private void txtHelper_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                if (this.txtSearch.Text.Trim() == "")
                {
                    MessageBox.Show("请输入需要检索的关键词！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.txtSearch.Focus();
                    return;
                }

           

                string url = Program.g_SearchResult + System.Web.HttpUtility.UrlEncode(this.txtSearch.Text.Trim(), Encoding.UTF8);

                System.Diagnostics.Process.Start(url);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Program.g_isLogin == false)
            {
                frmLogin f = new frmLogin();
                f.rUser = this.refreshLogin;
                f.ShowDialog();
            }
            else
            {
                if (MessageBox.Show ("是否退出当前登录账户，退出当前用户，您当前使用的版本也会回退至免费版，如有授权账号，可激活使用，是否继续？","网络矿工 询问",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
                {
                    //退出
                   

                    //删除key文件
                    string keyFile = Program.getPrjPath() + "components\\sominer.key";
                    File.Delete(keyFile);

                    Program.g_isLogin = false;

                    this.linkLabel1.Text = "登录";
                }

            }
        }

        private void refreshLogin(string User)
        {
            if (!string.IsNullOrEmpty(User))
            {
                this.linkLabel1.Text = User;
                Program.RegisterUser = User;
                Program.g_isLogin = true;

              

                this.m_Help.UpdateVersion();
            }
        }
    }

}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Data;
//using Interop.Excel;
using System.Text.RegularExpressions;
using NetMiner.Common.HttpSocket;
using NetMiner.Resource;
using NetMiner.Publish.Rule;
using System.Collections;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;
using NetMiner.Core.pTask.Entity;
using NetMiner.Core.Event;

///功能：发布任务
///完成时间：2009-7-21
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.01.00
///修订：增加了发布类别，支持数据库、web及文件发布
///2013-2-16 进行修改，将矿工采集任务的发布和数据发布工具的发布
///进行整合，采用此方法进行，同时支持多线程，发布延时等操作
///同时删除触发器事件，触发器已经在V2012SP1中删除
namespace NetMiner.Publish
{
    public class cPublish
    {
        private ePublishTaskData m_pTaskData;
        private cPublishManage m_PublishManage;
        
        //定义一个datatable，用于复制需要发布的数据
        //并从发布数据中取出需要发布的数据内容
        private Queue<DataRow> UnPublishData;

        bool m_ThreadsRunning = false;
        private Thread[] threadsRun;

        //计数使用
        private int m_dCount = 0;

        private int SleepConnectTime = 1;
        private int SleepFetchTime = 1;

        //定义一个索取Row的数字游标
        private int m_CurRowIndex = 0;

        //发布数据的cookie，采用发布模版时使用
        private string m_Cookie=string.Empty;

        private Hashtable m_WebClass;
        private bool m_IsGetClass = false;

        //定期一个全局的参数变量
        private Hashtable m_globalPara;

        //定义一个值，判断系统是否登录成功
        private bool m_isLoginSucceed = false;

        private string m_workPath = string.Empty;

        //构造函数采用两种方式，支持采集任务的发布和数据发布工具的发布操作
        /// <summary>
        /// 初始化发布类
        /// </summary>
        /// <param name="taskManage">发布控制器</param>
        /// <param name="TaskID">发布任务ID</param>
        /// <param name="dData">待发布数据，这是一个完整的数据表</param>
        public cPublish(string workPath, cPublishManage taskManage,Int64 TaskID,DataTable dData)
        {
            m_workPath = workPath;

            m_PublishManage = taskManage;
            m_pTaskData = new ePublishTaskData();
            UnPublishData = new Queue<DataRow>();

            //初始化任务数据
            LoadTaskInfo(TaskID, dData);
            

            //初始化待发布数据量
            m_dCount = dData.Rows.Count;

            //获取未发布数据
            if (dData.Rows.Count > 0 && dData.Columns[dData.Columns.Count - 1].ColumnName == "isPublished")
            {
                DataRow[] rows = dData.Select("isPublished='" + cGlobalParas.PublishResult.UnPublished.ToString() + "'");
                AddQueue(rows);

                PublishedCount = dData.Select("isPublished='" + cGlobalParas.PublishResult.Succeed.ToString() + "'").Length;
                PublishErrCount = dData.Select("isPublished='" + cGlobalParas.PublishResult.Fail.ToString() + "'").Length;

            }

            this.pThreadState = cGlobalParas.PublishThreadState.UnStart;
        }

        /// <summary>
        /// 初始化发布类，发布系统专用
        /// </summary>
        /// <param name="taskManage">发布控制器</param>
        /// <param name="TaskID">发布任务ID</param>
        /// <param name="dData">待发布数据</param>
        public cPublish(string workPath, cPublishManage taskManage,ePublishTask cpTask, bool isErrlog, DataTable dData)
        {
            m_workPath = workPath;

            m_PublishManage = taskManage;
            m_pTaskData = new ePublishTaskData();
            UnPublishData = new Queue<DataRow>();

            //初始化数据
            LoadPublishInfo(cpTask,-1, dData, isErrlog);

            m_dCount = dData.Rows.Count;

            //获取未发布数据
            if (dData.Rows.Count > 0)
            {
                DataRow[] rows = dData.Select("isPublished='" + cGlobalParas.PublishResult.UnPublished.ToString() + "'");
                AddQueue(rows);

                PublishedCount = dData.Select("isPublished='" + cGlobalParas.PublishResult.Succeed.ToString() + "'").Length;
                PublishErrCount = dData.Select("isPublished='" + cGlobalParas.PublishResult.Fail.ToString() + "'").Length;
            }

            this.pThreadState = cGlobalParas.PublishThreadState.UnStart;
        }

        /// <summary>
        /// 初始化发布类，分布式采集数据合并专用
        /// </summary>
        /// <param name="workPath"></param>
        /// <param name="taskManage"></param>
        /// <param name="cpTask"></param>
        /// <param name="isErrlog"></param>
        /// <param name="dData"></param>
        public cPublish(string workPath, cPublishManage taskManage, ePublishTask cpTask, bool isErrlog, DataTable dData,Int64 TaskID)
        {
            m_workPath = workPath;

            m_PublishManage = taskManage;
            m_pTaskData = new ePublishTaskData();
            UnPublishData = new Queue<DataRow>();

            //初始化数据
            LoadPublishInfo(cpTask,TaskID, dData, isErrlog);

            m_dCount = dData.Rows.Count;

            //获取未发布数据
            if (dData.Rows.Count > 0)
            {
                DataRow[] rows = dData.Select("isPublished='" + cGlobalParas.PublishResult.UnPublished.ToString() + "'");
                AddQueue(rows);

                PublishedCount = dData.Select("isPublished='" + cGlobalParas.PublishResult.Succeed.ToString() + "'").Length;
                PublishErrCount = dData.Select("isPublished='" + cGlobalParas.PublishResult.Fail.ToString() + "'").Length;
            }

            this.pThreadState = cGlobalParas.PublishThreadState.UnStart;
        }

        ~cPublish()
        {
            UnPublishData = null;
        }

        #region 将待发布数据压入队列
        private void AddQueue(DataRow[] rows)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                UnPublishData.Enqueue(rows[i]);
            }
        }
        #endregion

        #region 加载数据
        private void LoadTaskInfo(Int64 TaskID, System.Data.DataTable dData)
        {
            //DataTable dt = new DataTable();
            oTask t = new oTask(this.m_workPath);

            t.LoadTask(this.m_workPath + "tasks\\run\\task" + TaskID + ".rst");

            string SysSaveFileName = t.TaskEntity.SavePath + "\\" + t.TaskEntity.TaskName + "-" + t.TaskEntity.TaskID + ".xml";

            m_pTaskData.TaskID = t.TaskEntity.TaskID;
            m_pTaskData.TaskName = t.TaskEntity.TaskName;
            m_pTaskData.ExportFile = t.TaskEntity.ExportFile;
            m_pTaskData.DataSource = t.TaskEntity.DataSource;
            m_pTaskData.IsSaveSingleFile = t.TaskEntity.IsSaveSingleFile;

            if (t.TaskEntity.IsSaveSingleFile)
            {
                m_pTaskData.TempDataFile = t.TaskEntity.SavePath + "\\" + t.TaskEntity.TempDataFile;
                m_pTaskData.SysSaveTempFileName = t.TaskEntity.SavePath + "\\" + t.TaskEntity.TempDataFile;
            }
            else
            {
                m_pTaskData.SysSaveTempFileName = SysSaveFileName;
            }

            m_pTaskData.IsDelTempData = t.TaskEntity.IsDelTempData;

            //dt.ReadXml(FileName);
            //需要保存的或者导出的数据还是传入，因为需要临时数据的保存
            //下一版需要将临时数据保存和发布数据进行分离
            m_pTaskData.PublishData = dData;
            m_pTaskData.PublishData.TableName = t.TaskEntity.TaskName + "-" + t.TaskEntity.TaskID + ".xml";

            m_pTaskData.PublishType = t.TaskEntity.ExportType;
            m_pTaskData.DataTableName = t.TaskEntity.DataTableName;

            m_pTaskData.PublishThread = t.TaskEntity.PublishThread;

            m_pTaskData.InsertSql = t.TaskEntity.InsertSql;
            m_pTaskData.IsSqlTrue = t.TaskEntity.IsSqlTrue;

            m_pTaskData.PIntervalTime = t.TaskEntity.PIntervalTime;
            m_pTaskData.PSucceedFlag = t.TaskEntity.PSucceedFlag;

            m_pTaskData.IsErrorLog = t.TaskEntity.IsErrorLog;

            m_pTaskData.IsTrigger = t.TaskEntity.IsTrigger;

            m_pTaskData.IsExportHeader = t.TaskEntity.IsExportHeader;
            m_pTaskData.IsRowFile = t.TaskEntity.IsRowFile;

            if (t.TaskEntity.IsTrigger == true)
            {
                m_pTaskData.TriggerType = t.TaskEntity.TriggerType;
                m_pTaskData.TriggerTask = t.TaskEntity.TriggerTask;
            }

            //加载是否自定义Header
            m_pTaskData.Headers = t.TaskEntity.Headers;

            //加载模版信息
            //加载模版信息
            m_pTaskData.TemplateName = t.TaskEntity.TemplateName;
            m_pTaskData.User = t.TaskEntity.User;
            m_pTaskData.Password = t.TaskEntity.Password;
            m_pTaskData.Domain = t.TaskEntity.Domain;
            m_pTaskData.TemplateDBConn = t.TaskEntity.TemplateDBConn;
            m_pTaskData.PublishParas = t.TaskEntity.PublishParas;

            //判断模版信息是否需要登录，如果需要，则马上开始登录
            if (m_pTaskData.TemplateName != null && m_pTaskData.TemplateName!="")
            {
                string tName = m_pTaskData.TemplateName.Substring(0, m_pTaskData.TemplateName.IndexOf("["));
                cGlobalParas.PublishTemplateType pType =EnumUtil.GetEnumName<cGlobalParas.PublishTemplateType>(m_pTaskData.TemplateName.Substring(m_pTaskData.TemplateName.IndexOf("[") + 1, 
                    m_pTaskData.TemplateName.IndexOf("]") - m_pTaskData.TemplateName.IndexOf("[") - 1));

                //分解名称了类型
                if (pType == cGlobalParas.PublishTemplateType.Web && this.m_Cookie == "")
                {
                    //进行登录操作
                    cTemplate template = new cTemplate(this.m_workPath);
                    template.LoadTemplate(tName);

                    //重新加载发布的延时操作
                    m_pTaskData.PIntervalTime = template.PublishInterval;
                    m_pTaskData.isPlugin = template.IsVCodePlugin;
                    m_pTaskData.strPlugin = template.VCodePlugin;

                    string webSource = "";
                    bool isLogin = false;

                    try
                    {
                        //开始进行登录操作
                        isLogin = cHttpSocket.Login(m_pTaskData.User, m_pTaskData.Password, template.uCode,
                            m_pTaskData.Domain, template.Domain, template.LoginUrl, template.LoginRUrl, template.LoginVCodeUrl, template.LoginSuccess, template.LoginFail,
                            template.LoginParas, template.IsVCodePlugin, template.VCodePlugin, out this.m_Cookie, out webSource);
                    }
                    catch (System.Exception ex)
                    {
                        WriteLog("登录失败，错误信息：" + ex.Message);
                        if (e_PublishError != null)
                            e_PublishError(this, new PublishErrorEventArgs(this.TaskID, this.TaskData.TaskName, ex));
                    }

                    if (template.pgPara.Count > 0)
                    {
                        m_globalPara = new Hashtable();

                        for (int i = 0; i < template.pgPara.Count; i++)
                        {
                            if (template.pgPara[i].pgPage == cGlobalParas.PublishGlobalParaPage.LoginPage)
                            {
                                string label = template.pgPara[i].Label;
                                string strReg = template.pgPara[i].Value;

                                Match a = Regex.Match(webSource, strReg, RegexOptions.IgnoreCase);
                                string value = a.Groups[0].Value.ToString();
                                m_globalPara.Add(label, value);
                            }
                        }
                    }
                    t = null;

                    this.m_isLoginSucceed = isLogin;
                }
                else
                    this.m_isLoginSucceed =true ;
            }
            else
                this.m_isLoginSucceed = true;

            t = null;
        }

        //加载数据加工发布工具的发布信息，因为数据加工发布工具
        //只支持数据库和web发布方式，不支持文件，同时没有任务的ID
        //所以，加载时部分数据需要手工指定
        private void LoadPublishInfo(ePublishTask cpTask, Int64 TaskID, DataTable dData, bool isErrlog)
        {
            m_pTaskData.TaskID = TaskID;
            m_pTaskData.TaskName = cpTask.pName;
            m_pTaskData.ExportFile = "";
            m_pTaskData.DataSource = cpTask.DataSource;
            m_pTaskData.IsSaveSingleFile = false;
            m_pTaskData.SysSaveTempFileName = "";

            m_pTaskData.IsDelTempData = true;

            m_pTaskData.isEnginPublish = cpTask.isEnginPublish;
          
            m_pTaskData.PublishData = dData;
            m_pTaskData.PublishData.TableName = "pData";

            if (cpTask.PublishType == cGlobalParas.PublishType.PublishData)
            {
                switch (cpTask.DataType)
                {
                    case cGlobalParas.DatabaseType.Access:
                        m_pTaskData.PublishType = cGlobalParas.PublishType.PublishAccess;
                        break;
                    case cGlobalParas.DatabaseType.MSSqlServer:
                        m_pTaskData.PublishType = cGlobalParas.PublishType.PublishMSSql;
                        break;
                    case cGlobalParas.DatabaseType.MySql :
                        m_pTaskData.PublishType = cGlobalParas.PublishType.PublishMySql;
                        break;
                    case cGlobalParas.DatabaseType.Oracle :
                        m_pTaskData.PublishType = cGlobalParas.PublishType.publishOracle;
                        break;
                }
                
            }
            else
                m_pTaskData.PublishType = cpTask.PublishType;

            m_pTaskData.DataTableName = cpTask.DataTable;

            m_pTaskData.PublishThread = cpTask.ThreadCount;

            m_pTaskData.InsertSql = cpTask.InsertSql;
            m_pTaskData.IsSqlTrue = cpTask.IsSqlTrue;
            m_pTaskData.ExportUrl = cpTask.PostUrl;
            m_pTaskData.ExportUrlCode = ((int)cpTask.UrlCode).ToString ();
            m_pTaskData.ExportCookie = cpTask.Cookie;
            m_pTaskData.PIntervalTime = cpTask.PIntervalTime;
            m_pTaskData.PSucceedFlag = cpTask.SucceedFlag;

            m_pTaskData.IsErrorLog = isErrlog;

            m_pTaskData.IsTrigger = false;

            m_pTaskData.IsExportHeader = false ;
            m_pTaskData.IsRowFile = false ;

            //加载是否自定义Header
            m_pTaskData.IsCustomHeader = cpTask.IsHeader;

            if (cpTask.IsHeader == true)
            {
                string strheader = cpTask.Header;
                foreach (string sc in strheader.Split('\r'))
                {
                    eHeader header = new eHeader();

                    string ss = sc.Trim();
                    if (ss != "")
                    {
                        header.Label = ss.Substring(0, ss.IndexOf(":"));
                        header.LabelValue = ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1);

                        m_pTaskData.Headers.Add(header);
                    }
                }
            }

            //加载模版信息
            m_pTaskData.TemplateName = cpTask.TemplateName;
            m_pTaskData.User = cpTask.User;
            m_pTaskData.Password = cpTask.Password;
            m_pTaskData.Domain = cpTask.Domain;
            m_pTaskData.TemplateDBConn = cpTask.TemplateDBConn;
            m_pTaskData.PublishParas = cpTask.PublishParas;

            //判断模版信息是否需要登录，如果需要，则马上开始登录
            if (m_pTaskData.TemplateName != null && m_pTaskData.TemplateName != "")
            {
                string tName = m_pTaskData.TemplateName.Substring(0, m_pTaskData.TemplateName.IndexOf("["));
                cGlobalParas.PublishTemplateType pType = EnumUtil.GetEnumName<cGlobalParas.PublishTemplateType>( m_pTaskData.TemplateName.Substring(m_pTaskData.TemplateName.IndexOf("[") + 1, m_pTaskData.TemplateName.IndexOf("]") - m_pTaskData.TemplateName.IndexOf("[") - 1));

                //分解名称了类型
                if (pType == cGlobalParas.PublishTemplateType.Web && this.m_Cookie == "")
                {
                    //进行登录操作
                    cTemplate t = new cTemplate(m_workPath);
                    t.LoadTemplate(tName);

                    //重新加载发布的延时操作
                    m_pTaskData.PIntervalTime = t.PublishInterval;

                    string webSource = "";

                    //开始进行登录操作
                    bool isLogin = false;
                    try
                    {
                        isLogin = cHttpSocket.Login(m_pTaskData.User, m_pTaskData.Password, t.uCode,
                            m_pTaskData.Domain, t.Domain, t.LoginUrl, t.LoginRUrl, t.LoginVCodeUrl, t.LoginSuccess, t.LoginFail,
                            t.LoginParas, t.IsVCodePlugin, t.VCodePlugin, out this.m_Cookie, out webSource);
                    }
                    catch (System.Exception ex)
                    {
                        WriteLog("登录失败，错误信息：" + ex.Message);

                        if (e_PublishError != null)
                            e_PublishError(this, new PublishErrorEventArgs(this.TaskID, this.TaskData.TaskName, ex));
                    }

                    if (t.pgPara.Count > 0)
                    {
                        m_globalPara = new Hashtable();

                        for (int i = 0; i < t.pgPara.Count; i++)
                        {
                            if (t.pgPara[i].pgPage == cGlobalParas.PublishGlobalParaPage.LoginPage)
                            {
                                string label = t.pgPara[i].Label;
                                string strReg = t.pgPara[i].Value;

                                Match a = Regex.Match(webSource, strReg, RegexOptions.IgnoreCase);
                                string value = a.Groups[0].Value.ToString();
                                m_globalPara.Add(label, value);
                            }
                        }
                    }
                    t = null;

                    this.m_isLoginSucceed = isLogin;
                }
                else
                    this.m_isLoginSucceed = true;
            }
            else
                this.m_isLoginSucceed = true;

        }

        private void GetVCode(string code)
        {
           
        }
        #endregion

        #region 属性

        public Int64 TaskID
        {
            get { return m_pTaskData.TaskID; }
        }

        public string TaskName
        {
            get { return m_pTaskData.TaskName; }
        }

        public ePublishTaskData TaskData
        {
            get { return m_pTaskData; }
        }

        public cPublishManage PublishManage
        {
            get { return m_PublishManage; }
        }

        public string SysSaveTempFileName
        {
            get { return m_pTaskData.SysSaveTempFileName; }
        }

        public cGlobalParas.PublishType PublishType
        {
            get { return m_pTaskData.PublishType; }
        }

        public int Count
        {
            get { return m_dCount; }
        }

        private int m_PublishedCount;
        public int PublishedCount
        {
            get { return m_PublishedCount; }
            set { m_PublishedCount = value; }
        }

        private int m_PublishErrCount;
        public int PublishErrCount
        {
            get { return m_PublishErrCount; }
            set { m_PublishErrCount = value; }
        }

        private int m_ThreadMaxCount;
        public int ThreadMaxCount
        {
            get { return m_ThreadCount; }
            set { m_ThreadMaxCount = value; }
        }

        private int m_ThreadCount;
        public int ThreadCount
        {
            get { return m_ThreadCount; }
            set
            {
                m_ThreadCount = value;

                try
                {
                    for (int nIndex = 0; nIndex < value; nIndex++)
                    {
                        if (threadsRun[nIndex] == null || threadsRun[nIndex].ThreadState != ThreadState.Suspended)
                        {
                            threadsRun[nIndex] = new Thread(new ThreadStart(ThreadRunFunction));
                            threadsRun[nIndex].Name = nIndex.ToString();
                            threadsRun[nIndex].Start();

                            //this.OnRunTimeMessageSend(this, new RunTimeEventArgs("创建线程成功：" + nIndex.ToString()));

                        }
                        else if (threadsRun[nIndex].ThreadState == ThreadState.Suspended)
                        {
                            threadsRun[nIndex].Resume();
                        }
                    }

                }
                catch (Exception)
                {
                }
            }
        }

        private cGlobalParas.PublishThreadState m_pThreadState;
        public cGlobalParas.PublishThreadState pThreadState
        {
            get { return m_pThreadState; }
            set { m_pThreadState = value; }
        }
        #endregion



        //根据提供的taskid加载任务信息
        //数据不应该是传进来,是读取文件的,但现在不支持事务处理,所以传进来
   

        #region 事件定义
        private Thread m_Thread;
        private readonly Object m_eventLock = new Object();

        /// 采集任务完成事件
        private event EventHandler<PublishCompletedEventArgs> e_PublishCompleted;
        internal event EventHandler<PublishCompletedEventArgs> PublishCompleted
        {
            add { lock (m_eventLock) { e_PublishCompleted += value; } }
            remove { lock (m_eventLock) { e_PublishCompleted -= value; } }
        }

        /// 采集任务采集失败事件
        private event EventHandler<PublishFailedEventArgs> e_PublishFailed;
        internal event EventHandler<PublishFailedEventArgs> PublishFailed
        {
            add { lock (m_eventLock) { e_PublishFailed += value; } }
            remove { lock (m_eventLock) { e_PublishFailed -= value; } }
        }

        /// 采集任务开始事件
        private event EventHandler<PublishStartedEventArgs> e_PublishStarted;
        internal event EventHandler<PublishStartedEventArgs> PublishStarted
        {
            add { lock (m_eventLock) { e_PublishStarted += value; } }
            remove { lock (m_eventLock) { e_PublishStarted -= value; } }
        }

        //发布任务停止事件
        private event EventHandler<PublishStopEventArgs> e_PublishStop;
        public event EventHandler<PublishStopEventArgs> PublishStop
        {
            add { e_PublishStop += value; }
            remove { e_PublishStop -= value; }
        }

        /// 采集任务错误事件
        private event EventHandler<PublishErrorEventArgs> e_PublishError;
        internal event EventHandler<PublishErrorEventArgs> PublishError
        {
            add { lock (m_eventLock) { e_PublishError += value; } }
            remove { lock (m_eventLock) { e_PublishError -= value; } }
        }

        /// 临时数据发布完成时间
        //private event EventHandler<PublishTempDataCompletedEventArgs> e_PublishTempDataCompleted;
        //internal event EventHandler<PublishTempDataCompletedEventArgs> PublishTempDataCompleted
        //{
        //    add { lock (m_eventLock) { e_PublishTempDataCompleted += value; } }
        //    remove { lock (m_eventLock) { e_PublishTempDataCompleted -= value; } }
        //}

        //发布日志事件
        private event EventHandler<PublishLogEventArgs> e_PublishLog;
        internal event EventHandler<PublishLogEventArgs> PublishLog
        {
            add { lock (m_eventLock) { e_PublishLog += value; } }
            remove { lock (m_eventLock) { e_PublishLog -= value; } }
        }

        /// <summary>
        /// 定义一个执行网络矿工任务的事件，用于响应触发器执行网络矿工任务时
        /// 的处理。
        /// </summary>
        private event EventHandler<cRunTaskEventArgs> e_RunTask;
        internal event EventHandler<cRunTaskEventArgs> RunTask
        {
            add { lock (m_eventLock) { e_RunTask += value; } }
            remove { lock (m_eventLock) { e_RunTask -= value; } }
        }

        /// <summary>
        /// 线程工作效率事件
        /// </summary>
        private event EventHandler<RunTimeEventArgs> e_RuntimeInfo;
        public event EventHandler<RunTimeEventArgs> RuntimeInfo
        {
            add { e_RuntimeInfo += value; }
            remove { e_RuntimeInfo -= value; }
        }

        /// <summary>
        /// 计数事件
        /// </summary>
        private event EventHandler<DoCountEventArgs> e_DoCount;
        public event EventHandler<DoCountEventArgs> DoCount
        {
            add { e_DoCount += value; }
            remove { e_DoCount -= value; }
        }

        /// <summary>
        /// 返回发布失败的错误数据事件
        /// </summary>
        private event EventHandler<PublishErrDataEventArgs> e_PublishErrorData;
        public event EventHandler<PublishErrDataEventArgs> PublishErrorData
        {
            add { e_PublishErrorData += value; }
            remove { e_PublishErrorData -= value; }
        }

        /// <summary>
        /// 返回最后一次web发布请求的网页源码
        /// </summary>
        private event EventHandler<PublishSourceEventArgs> e_PublishSource;
        public event EventHandler<PublishSourceEventArgs> PublishSource
        {
            add { e_PublishSource += value; }
            remove { e_PublishSource -= value; }
        }

        private event EventHandler<UpdateStateArgs> e_UpdateState;
        public event EventHandler<UpdateStateArgs> UpdateState
        {
            add { e_UpdateState += value; }
            remove { e_UpdateState -= value; }
        }
        #endregion

        private readonly Object m_threadLock = new Object();

        #region 用于临时存储发布数据
        //此方法用于临时发布数据使用，即如果用户终端了操作
        //需调用此方法对已经采集的数据进行临时发布，当前
        //仅临时进行保存
        //public void startSaveTempData()
        //{
        //    lock (m_threadLock)
        //    {
        //        m_Thread = new Thread(this.SaveTempData);

        //        //定义线程名称,用于调试使用
        //        m_Thread.Name = SysSaveTempFileName;

        //        m_Thread.Start();
        //    }
        //}

        //private readonly Object m_fileLock = new Object();

        //private void SaveTempData()
        //{
        //    //无论数据是否发布，都需要保存采集下来的数据
        //    //保存到本地磁盘
        //    string FileName = "";

        //    try
        //    {
        //        if (m_pTaskData.IsSaveSingleFile)
        //        {
        //            FileName = m_pTaskData.TempDataFile;
        //            if (File.Exists(FileName))
        //            {
        //                System.Data.DataTable tmp = new System.Data.DataTable();
        //                tmp.ReadXml(FileName);
        //                tmp.Merge(m_pTaskData.PublishData);
        //                tmp.AcceptChanges();
        //                tmp.WriteXml(FileName, XmlWriteMode.WriteSchema);
        //                tmp = null;
        //            }
        //            else
        //            {
        //                m_pTaskData.PublishData.WriteXml(FileName, XmlWriteMode.WriteSchema);
        //            }
        //        }
        //        else
        //        {
        //            FileName = m_pTaskData.SysSaveTempFileName;
                    
        //            if (File.Exists(FileName))
        //            {
        //                lock (m_fileLock)
        //                {
        //                    File.Delete(m_pTaskData.SysSaveTempFileName);
        //                }
        //            }

        //            m_pTaskData.PublishData.WriteXml(FileName, XmlWriteMode.WriteSchema);
        //        }

        //        //临时数据的保存由系统来维护，所以以下代码被注销
        //        //触发临时数据发布成功事件
        //        //e_PublishTempDataCompleted(this, new PublishTempDataCompletedEventArgs(this.TaskData.TaskID, this.TaskData.TaskName));
        //    }
        //    catch (System.Exception ex)
        //    {
        //        //存储临时数据时，有可能导致多个线程访问的失败操作，当前并没有
        //        //加锁控制，加锁控制在下一版提供
        //        if (e_PublishLog != null)
        //        {
        //            WriteLog(ex.Message);
        //            e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, cGlobalParas.LogType.Error , "任务：" + this.TaskData.TaskName + "临时存储失败，错误信息为：" + ex.Message  ,false));

        //        }
                
        //        if (e_PublishError != null)
        //        {
        //            e_PublishError(this, new PublishErrorEventArgs(this.TaskData.TaskID, this.TaskData.TaskName, ex));
        //        }
                
        //    }
        //}
        #endregion

        //此方法用于采集任务完成后，数据的发布操作
        public void startPublic()
        {
            
            m_ThreadsRunning = true;

            if (m_pTaskData.PublishType == cGlobalParas.PublishType.PublishAccess ||
                m_pTaskData.PublishType == cGlobalParas.PublishType.PublishMSSql ||
                m_pTaskData.PublishType == cGlobalParas.PublishType.PublishMySql ||
                m_pTaskData.PublishType ==cGlobalParas.PublishType.publishOracle)
            {
                cGlobalParas.DatabaseType dType=cGlobalParas.DatabaseType.SoukeyData;
                if (m_pTaskData.PublishType == cGlobalParas.PublishType.PublishMSSql)
                    dType = cGlobalParas.DatabaseType.MSSqlServer;
                else if (m_pTaskData.PublishType == cGlobalParas.PublishType.PublishAccess)
                    dType = cGlobalParas.DatabaseType.Access;
                else if (m_pTaskData.PublishType == cGlobalParas.PublishType.PublishMySql)
                    dType = cGlobalParas.DatabaseType.MySql;
                else if (m_pTaskData.PublishType == cGlobalParas.PublishType.publishOracle)
                    dType = cGlobalParas.DatabaseType.Oracle;

                NetMiner.Core.DB.oDBDeal dd = new NetMiner.Core.DB.oDBDeal(this.m_workPath);
                try
                {
                    dd.NewTable(m_pTaskData.PublishData.Columns, dType,  m_pTaskData.DataSource, m_pTaskData.DataTableName);
                }
                catch (System.Exception ex)
                {
                    if (e_PublishError != null)
                    {
                        e_PublishError(this, new PublishErrorEventArgs(this.TaskID, this.TaskData.TaskName, ex));
                    }
                }
                dd = null;
            }

            if (m_pTaskData.PublishType == cGlobalParas.PublishType.PublishCSV ||
                m_pTaskData.PublishType == cGlobalParas.PublishType.PublishExcel ||
                m_pTaskData.PublishType == cGlobalParas.PublishType.PublishTxt ||
                m_pTaskData.PublishType==cGlobalParas.PublishType.publishWord)
            {
                //如果发布时文件，则强制使用1个线程进行处理
                m_pTaskData.PublishThread = 1;
            }

            this.threadsRun = new Thread[m_pTaskData.PublishThread];

            //设置当前的线程数，同时启动线程
            ThreadCount = m_pTaskData.PublishThread;

            e_PublishStarted(this, new PublishStartedEventArgs(m_pTaskData.TaskID,m_pTaskData.TaskName));
            this.m_pThreadState = cGlobalParas.PublishThreadState.Running;

        }

        public void StopPublish()
        {
            m_ThreadsRunning = false;

            delStopTask();

        }

        public void OverPublish()
        {
            this.pThreadState = cGlobalParas.PublishThreadState.Complete;

            //获取临时数据保存的文件名
            string FileName = string.Empty;
            if (m_pTaskData.IsSaveSingleFile)
            {
                FileName = m_pTaskData.TempDataFile;
            }
            else
            {
                FileName = m_pTaskData.SysSaveTempFileName;
            }

            PublishCompletedEventArgs evt1 = new PublishCompletedEventArgs(this.TaskData.TaskID,
                this.TaskData.TaskName, m_pTaskData.IsDelTempData, FileName);

            e_PublishCompleted(this, evt1);

            if (e_PublishLog != null)
                e_PublishLog(this, new PublishLogEventArgs(this.TaskID, cGlobalParas.LogType.Info,
                    "数据发布完成!", false));
        }

        //用一个异步回调的函数来处理检测各个线程的状态，从而确定是否真正的停止发布
        private delegate void delegateStop();
        private void delStopTask()
        {
            delegateStop sd = new delegateStop(this.dStopTask);
            AsyncCallback callback = new AsyncCallback(CallbackStop);
            sd.BeginInvoke(callback, sd);
        }

        private void dStopTask()
        {
            //在此等待检测每个线程的状态，
            //如果都为停止状态则进行停止操作，否则进入死循环
            while (true)
            {
                bool isStop = true;
                for (int i = 0; i < threadsRun.Length; i++)
                {
                    if (threadsRun[i].ThreadState == ThreadState.Running)
                    {
                        isStop = false;
                    }
                }

                if (isStop == true)
                    break;
                else
                    System.Threading.Thread.Sleep(500);
            }
        }

        private void CallbackStop(IAsyncResult ir)
        {
            this.pThreadState = cGlobalParas.PublishThreadState.Stopped;

            if (e_PublishStop != null)
                e_PublishStop(this, new PublishStopEventArgs(m_pTaskData.TaskID, m_pTaskData.TaskName,
                    m_pTaskData.PublishData));
        }

        /// <summary>
        /// 强制终止发布的任务，并销毁所有数据
        /// </summary>
        public void Abort()
        {
            m_ThreadsRunning = false;
            m_Thread = null;
            if (this.threadsRun != null)
            {
                for (int i = 0; i < this.threadsRun.Length; i++)
                    this.threadsRun[i].Abort();
            }
            this.threadsRun = null; 

        }

        public void ThreadRunFunction()
        {

            while (m_ThreadsRunning && int.Parse(Thread.CurrentThread.Name) < this.ThreadCount)
            {
               
                DateTime mstartTick = DateTime.Now;

                cPublishData cp = new cPublishData(this.m_workPath);

                if (this.PublishType == cGlobalParas.PublishType.PublishCSV || this.PublishType == cGlobalParas.PublishType.PublishExcel
                    || this.PublishType == cGlobalParas.PublishType.PublishTxt || this.PublishType==cGlobalParas.PublishType.publishWord)
                {
                    #region 发布到文件是一个完整的事务，要不全部发布，要不全部失败
                    try
                    {
                        cp.ExportFile(this.PublishType, m_pTaskData.TaskName, m_pTaskData.ExportFile,
                                    m_pTaskData.PublishData, m_pTaskData.IsExportHeader, m_pTaskData.IsRowFile);


                        cp = null;

                        //直接退出循环，表示发布数据完成
                        //因为是发布文件，不存在多线程，同时也不存在计数问题，所以，为了
                        //可以完成发布操作，需要赋值donecount
                        m_PublishedCount = m_dCount;

                        if (e_PublishLog != null)
                            e_PublishLog(this, new PublishLogEventArgs(this.TaskID, cGlobalParas.LogType.Info, "采集数据已经发布到文件：" + m_pTaskData.ExportFile, false));
                    }
                    catch (System.Exception ex)
                    {
                        if (e_PublishError != null)
                        {
                            e_PublishError(this, new PublishErrorEventArgs(m_pTaskData.TaskID, m_pTaskData.TaskName, ex));
                        }
                        WriteLog(ex.Message);
                        m_PublishErrCount = m_dCount;
                    }

                    break;

                #endregion
                   
                }
                else
                {
                    if (m_pTaskData.isEnginPublish)
                    {
                        NetMiner.Core.DB.oDBDeal dd = new Core.DB.oDBDeal(this.m_workPath);
                        switch (this.PublishType)
                        {
                            case cGlobalParas.PublishType.PublishMySql:
                                dd.ExportMySqlALL(m_pTaskData.DataTableName, m_pTaskData.PublishData, m_pTaskData.DataSource, m_pTaskData.InsertSql, m_pTaskData.IsSqlTrue);
                                break;
                            case cGlobalParas.PublishType.PublishMSSql:
                                break;
                            case cGlobalParas.PublishType.publishOracle:
                                break;
                            case cGlobalParas.PublishType.PublishAccess:
                                break;
                         
                        }
                        m_PublishedCount = m_dCount;
                        m_ThreadsRunning = false;
                        dd = null;
                    }
                    else
                    {
                        object[] dRow = GetDataRow();

                        if (dRow != null)
                        {
                            if (this.PublishType == cGlobalParas.PublishType.publishTemplate)
                            {
                                #region 使用模板发布数据
                                try
                                {
                                    //在此处理发布模版的规则
                                    string tName = m_pTaskData.TemplateName.Substring(0, m_pTaskData.TemplateName.IndexOf("["));
                                    cGlobalParas.PublishTemplateType pType = EnumUtil.GetEnumName<cGlobalParas.PublishTemplateType>( m_pTaskData.TemplateName.Substring(m_pTaskData.TemplateName.IndexOf("[") + 1, m_pTaskData.TemplateName.IndexOf("]") - m_pTaskData.TemplateName.IndexOf("[") - 1));

                                    //模版发布
                                    if (pType == cGlobalParas.PublishTemplateType.Web)
                                    {
                                        //在此判断登录是否成功，如果不成功，则直接结束任务
                                        if (this.m_isLoginSucceed == false)
                                        {
                                            if (e_PublishLog != null)
                                                e_PublishLog(this, new PublishLogEventArgs(m_pTaskData.TaskID, cGlobalParas.LogType.Info, "登录失败，停止发布操作！"
                                                   , false));

                                            StopPublish();
                                            return;
                                        }

                                        //无论是否存在分类的信息都需要进行获取
                                        if (m_IsGetClass == false)
                                        {
                                            this.m_WebClass = cp.GetWebClass(tName, m_pTaskData.Domain, this.m_Cookie, this.m_globalPara);
                                            m_IsGetClass = true;
                                        }

                                        string rResource = string.Empty;

                                        cp.PublishByWebTemplate(m_pTaskData.PublishData.Columns, dRow, tName, this.m_Cookie,
                                            m_pTaskData.Domain, m_WebClass, m_pTaskData.PublishParas, this.m_globalPara, m_pTaskData.strPlugin, out rResource);

                                        if (m_pTaskData.PIntervalTime != 0)
                                        {
                                            //设置采集延时
                                            Thread.Sleep(m_pTaskData.PIntervalTime);
                                            if (e_PublishLog != null)
                                                e_PublishLog(this, new PublishLogEventArgs(m_pTaskData.TaskID, cGlobalParas.LogType.Info, "系统设置了发布延时，系统暂停"
                                                    + m_pTaskData.PIntervalTime + "毫秒", false));
                                        }

                                    }
                                    else if (pType == cGlobalParas.PublishTemplateType.DB)
                                    {
                                        //用数据库的模版发布

                                        cp.PublishByDbTemplate(m_pTaskData.PublishData.Columns, dRow, tName, this.PublishType,
                                            m_pTaskData.TemplateDBConn, m_pTaskData.PublishParas);
                                    }

                                    m_PublishedCount++;
                                    TimeSpan usticks = DateTime.Now - mstartTick;

                                    if (e_PublishLog != null && this.TaskData.IsErrorLog == false)
                                        e_PublishLog(this, new PublishLogEventArgs(this.TaskID, cGlobalParas.LogType.Info,
                                            "发布完成，数据：" + dRow[0] + "...", false));

                                    if (e_UpdateState != null)
                                        e_UpdateState(this, new UpdateStateArgs(dRow, this.TaskID, cGlobalParas.PublishResult.Succeed));
                                }
                                catch (System.Exception ex)
                                {
                                    if (e_PublishError != null)
                                    {
                                        e_PublishError(this, new PublishErrorEventArgs(m_pTaskData.TaskID, m_pTaskData.TaskName,
                                           new NetMinerException(ex.Message + " 数据：" + dRow[0] + "...")));

                                        WriteLog(ex.Message + " 数据：" + dRow[0] + "...");

                                    }
                                    if (e_UpdateState != null)
                                        e_UpdateState(this, new UpdateStateArgs(dRow, this.TaskID, cGlobalParas.PublishResult.Fail));
                                    m_PublishErrCount++;
                                    //InsertRow(dRow);
                                }
                                #endregion
                            }
                            else
                            {
                                #region 发布数据到数据库和网站，通过用户的规则
                                try
                                {
                                    NetMiner.Core.DB.oDBDeal dd = new Core.DB.oDBDeal(this.m_workPath);
                                    switch (this.PublishType)
                                    {
                                        case cGlobalParas.PublishType.PublishAccess:
                                            //dd.ExportAccess(m_pTaskData.PublishData.Columns, dRow,
                                                //m_pTaskData.DataSource, m_pTaskData.InsertSql, m_pTaskData.DataTableName, m_pTaskData.IsSqlTrue);
                                            m_PublishedCount++;
                                            break;
                                        case cGlobalParas.PublishType.PublishMSSql:
                                            dd.ExportMSSql(m_pTaskData.PublishData.Columns, dRow,
                                                m_pTaskData.DataSource, m_pTaskData.InsertSql, m_pTaskData.DataTableName, m_pTaskData.IsSqlTrue);
                                            m_PublishedCount++;
                                            break;
                                        case cGlobalParas.PublishType.PublishMySql:
                                            dd.ExportMySql(m_pTaskData.PublishData.Columns, dRow,
                                                m_pTaskData.DataSource, m_pTaskData.InsertSql, m_pTaskData.DataTableName, m_pTaskData.IsSqlTrue);

                                            m_PublishedCount++;

                                            break;
                                        case cGlobalParas.PublishType.publishOracle:
                                            dd.ExportOracle(m_pTaskData.PublishData.Columns, dRow,
                                                m_pTaskData.DataSource, m_pTaskData.InsertSql, m_pTaskData.DataTableName, m_pTaskData.IsSqlTrue);
                                            break;

                                    }
                                    TimeSpan usticks = DateTime.Now - mstartTick;

                                    if (e_PublishLog != null && this.TaskData.IsErrorLog == false)
                                        e_PublishLog(this, new PublishLogEventArgs(this.TaskID, cGlobalParas.LogType.Info,
                                            "发布完成，数据：" + ((object[])dRow)[0] + "...", false));

                                    if (e_UpdateState != null)
                                        e_UpdateState(this, new UpdateStateArgs((object[])dRow, this.TaskID, cGlobalParas.PublishResult.Succeed));

                                }
                                catch (System.Exception ex)
                                {
                                    if (e_PublishError != null)
                                    {
                                        e_PublishError(this, new PublishErrorEventArgs(m_pTaskData.TaskID, m_pTaskData.TaskName, ex));
                                    }
                                    if (e_UpdateState != null)
                                        e_UpdateState(this, new UpdateStateArgs((object[])dRow, this.TaskID, cGlobalParas.PublishResult.Fail));
                                    WriteLog(ex.Message);
                                    m_PublishErrCount++;
                                    //InsertRow(dRow);
                                }

                                #endregion
                            }

                        }
                        else
                        {
                            if (dRow == null && UnPublishData.Count == 0)
                            {
                                //把出错数据返回到界面
                                m_ThreadsRunning = false;
                            }
                            else
                                Thread.Sleep(SleepFetchTime * 1000);
                        }
                    }
                }
               

                if (e_DoCount!=null)
                    e_DoCount(this, new DoCountEventArgs(m_dCount, m_PublishedCount, m_PublishErrCount, m_CurRowIndex));
              
                
            }


            onComplete();

        }

        /// <summary>
        /// 判断是否已经完成发布操作，如果完成则进行完成事件的触发
        /// </summary>
        private void onComplete()
        {
            //数据发布完成
            if (m_dCount == m_PublishedCount + m_PublishErrCount && this.pThreadState!=cGlobalParas.PublishThreadState.Complete)
            {
               
                    this.pThreadState = cGlobalParas.PublishThreadState.Complete;

                    //m_pTaskData.PublishData = m_ErrData.Clone ();
            
                    //获取临时数据保存的文件名
                    string FileName = string.Empty;
                    if (m_pTaskData.IsSaveSingleFile)
                    {
                        FileName = m_pTaskData.TempDataFile;
                    }
                    else
                    {
                        FileName = m_pTaskData.SysSaveTempFileName;
                    }

                    PublishCompletedEventArgs evt1 = new PublishCompletedEventArgs(this.TaskData.TaskID,
                        this.TaskData.TaskName,  m_pTaskData.IsDelTempData, FileName);

                    e_PublishCompleted(this, evt1);

                    if (e_PublishLog != null)
                        e_PublishLog(this, new PublishLogEventArgs(this.TaskID, cGlobalParas.LogType.Info, 
                            "数据发布完成!", false));

            }
           
        }

        ///提取数据锁
        private readonly Object m_DataLock = new Object();

        //获取一条数据
        //只要是传入进来的数据，都必须加入一个状态值，
        //用于识别发布的状态
        private object[] GetDataRow()
        {
            lock (m_DataLock)
            {
                if (UnPublishData.Count >0)  //&& m_CurRowIndex <m_pData.Rows.Count )
                {
                    try
                    {
                        DataRow r = UnPublishData.Dequeue();
                        return r.ItemArray;
                    }
                    catch (System.Exception)
                    {
                        m_CurRowIndex++;
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        ///插入数据锁
        private readonly Object m_ErrDataLock = new Object();

        //private void InsertRow(object dRow)
        //{
        //    lock (m_ErrDataLock)
        //    {
        //        DataRow r = m_ErrData.NewRow();
        //        object[] Row = ((object[])dRow);

        //        for (int j = 0; j < Row.Length; j++)
        //        {
        //            r[j] = Row[j].ToString();
        //        }

        //        m_ErrData.Rows.Add(r);
        //    }
        //}

        //写错误日志
        private void WriteLog(string strMess)
        {
            //在此处理是否写入错误数据到日志的请求
            if (this.TaskData.IsErrorLog ==true)
            {
                try
                {
                    NetMiner.Core.Log.cSystemLog eLog = new NetMiner.Core.Log.cSystemLog(this.m_workPath);
                    eLog.WriteLog(this.TaskData.TaskName, cGlobalParas.LogType.PublishError, strMess);
                    eLog = null;
                }
                catch(System.Exception ex)
                {
                    //cTool.WriteSystemLog(ex.Message, EventLogEntryType.Error, "SMGatherService");
                    //出错了

                }
            }
        }

    }
}

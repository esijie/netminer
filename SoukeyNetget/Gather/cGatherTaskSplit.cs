using System;
using System.Collections.Generic;
using System.Text;
using System.Threading ;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using SoukeyNetget.Task;

///功能：采集任务 分解子任务处理
///完成时间：2009-6-1
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Gather
{

    //这是采集任务的最小单元,针对于一个采集任务而言,是一个URL地址集合
    //存在有多个甚至上千个URL地址,为了提高性能,对URl采集采用了多线程的
    //处理方式,所以针对一个任务要进行拆分,拆分的依据是此任务定制的线程
    //数,任务拆后开始执行.此类主要就是执行这么一个最小单元的任务.

    public class cGatherTaskSplit : IDisposable 
    {

        private delegate void work();
        
        private Thread m_Thread;
        private bool m_ThreadRunning = false;

        private string RegexNextPage = "";

        //设置一个参数值，判断，如果数据采集是采用了直接入库的方式
        //判断数据表是否需要新建，如果新建，则在第一次运行时，建立新表
        //成功后，并修改此时为false，如果已经存在此表，也许修改为false
        private bool IsNewTable = true;

        #region 构造类，并初始化相关数据

        ///修改了构造函数
        public cGatherTaskSplit()
        {
            //m_TaskManage = TaskManage;
            //m_TaskID = TaskID;
            //m_gStartPos = StartPos;
            //m_gEndPos = EndPos;
            //m_SavePath = sPath;
            //m_Cookie = strCookie;
            //m_WebCode = webCode;
            //m_IsUrlEncode = IsUrlEncode;
            //m_UrlEncode = UrlEncode;
            m_TaskSplitData = new cTaskSplitData ();
            m_ThreadState = cGlobalParas.GatherThreadState.Stopped;

            //m_TaskType = TaskType;

            m_GatherData = new DataTable();
        }

        public void UpdateCookie(string cookie)
        {
            m_Cookie = cookie;
        }

        public void UpdateUrl(string Url)
        {
            if (m_TaskSplitData.Weblink.Count > 0)
            {
                m_TaskSplitData.Weblink[0].Weblink = Url;
            }
        }

        ~cGatherTaskSplit()
        {
            m_GatherData = null;

            Dispose(false);
        }

        #endregion

        /// <summary>
        /// 事件 线程同步锁
        /// </summary>
        private readonly Object m_eventLock = new Object();
        /// <summary>
        /// 缓存 线程同步锁
        /// </summary>
        private readonly Object m_mstreamLock = new Object();

        #region 需要初始化类的属性

        private int m_ErrorCount;
        public int ErrorCount
        {
            get { return m_ErrorCount; }
        }

        private bool m_IsInitialized;
        internal bool IsInitialized
        {
            get { return m_IsInitialized; }
            set { m_IsInitialized = value; }
        }

        private int m_Waittime;
        internal int Waittime
        {
            get { return m_Waittime; }
            set { m_Waittime = value; }
        }

        private Int64 m_TaskID;
        public Int64 TaskID
        {
            get { return m_TaskID; }
            set { m_TaskID = value; }
        }

        private string m_gStartPos;
        public string StartPos
        {
            get { return m_gStartPos; }
            set { m_gStartPos = value; }
        }

        private string m_gEndPos;
        public string EndPos
        {
            get { return m_gEndPos; }
            set { m_gEndPos = value; }
        }
        private string m_Cookie;
        public string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }
        private cTaskSplitData m_TaskSplitData;
        public cTaskSplitData TaskSplitData
        {
            get { return m_TaskSplitData; }
            set { m_TaskSplitData = value; }
        }

        private cGlobalParas.TaskRunType m_TaskRunType;
        public cGlobalParas.TaskRunType TaskRunType
        {
            get { return m_TaskRunType; }
            set { m_TaskRunType = value; }
        }

        private cGlobalParas.TaskType m_TaskType;
        public cGlobalParas.TaskType TaskType
        {
            get { return m_TaskType; }
            set { m_TaskType = value; }
        }

        //增量采集判断的条件
        //private cGlobalParas.IncrByCondition m_IncrByCondition;
        //public cGlobalParas.IncrByCondition IncrByCondition
        //{
        //    get { return m_IncrByCondition; }
        //    set { m_IncrByCondition = value; }
        //}

        private cGlobalParas.WebCode m_WebCode;
        public cGlobalParas.WebCode WebCode
        {
            get { return m_WebCode; }
            set { m_WebCode = value; }
        }

        private bool m_IsUrlEncode;
        public bool IsUrlEncode
        {
            get { return m_IsUrlEncode; }
            set { m_IsUrlEncode = value; }
        }

        private string m_UrlEncode;
        public string UrlEncode
        {
            get { return m_UrlEncode; }
            set { m_UrlEncode = value; }
        }

        private cGatherManage m_TaskManage;
        public cGatherManage TaskManage
        {
            get { return m_TaskManage; }
            set { m_TaskManage = value; }
        }

        private DataTable m_GatherData;
        public DataTable GatherData
        {
            get { return m_GatherData; }
            set { m_GatherData = value; }
        }

        private string m_SavePath;
        public string SavePath
        {
            get { return m_SavePath; }
            set { m_SavePath = value; }
        }

        private int m_AgainNumber;
        public int AgainNumber
        {
            get { return m_AgainNumber; }
            set { m_AgainNumber = value; }
        }

        private bool m_Ignore404;
        public bool Ignore404
        {
            get { return m_Ignore404; }
            set { m_Ignore404 = value; }
        }

        private bool m_IsErrorLog;
        public bool IsErrorLog
        {
            get { return m_IsErrorLog; }
            set { m_IsErrorLog = value; }
        }

        //暂未用
        private bool m_IsDelRepRow;
        public bool IsDelRepRow
        {
            get { return m_IsDelRepRow; }
            set { m_IsDelRepRow = value; }
        }

        private bool m_IsExportGUrl;
        public bool IsExportGUrl
        {
            get { return m_IsExportGUrl; }
            set { m_IsExportGUrl = value; }
        }

        private bool m_IsExportGDateTime;
        public bool IsExportGDateTime
        {
            get { return m_IsExportGDateTime; }
            set { m_IsExportGDateTime = value; }
        }

        private cGlobalParas.PublishType m_PublishType;
        public cGlobalParas.PublishType PublishType
        {
            get { return m_PublishType; }
            set { m_PublishType = value; }
        }

        private string m_DataSource;
        public string DataSource
        {
            get { return m_DataSource; }
            set { m_DataSource = value; }
        }

        private string m_TableName;
        public string TableName
        {
            get { return m_TableName; }
            set { m_TableName = value; }
        }

        private string m_InsertSql;
        public string InsertSql
        {
            get { return m_InsertSql; }
            set { m_InsertSql = value; }
        }

        //根据传入的采集规则，判断自动翻页的网址是否需要进行数据合并
        private bool m_IsMergeData;
        public bool IsMergeData
        {
            get { return m_IsMergeData; }
            set { m_IsMergeData = value; }
        }

        //采集间隔延时
        private int m_GIntervalTime;
        public int GIntervalTime
        {
            get { return m_GIntervalTime; }
            set { m_GIntervalTime = value; }
        }

        //任务1.8增加，判断是否需要进行自定义的Header
        private bool m_IsCustomHeader;
        public bool IsCustomHeader
        {
            get { return m_IsCustomHeader; }
            set { m_IsCustomHeader = value; }
        }

        private bool m_IsPublishHeader;
        public bool IsPublishHeader
        {
            get { return m_IsPublishHeader; }
            set { m_IsPublishHeader = value; }
        }

        private List<cHeader> m_Headers;
        public List<cHeader> Headers
        {
            get { return m_Headers; }
            set { m_Headers = value; }
        }

        #endregion

        #region 任务运行状态属性

        /// <summary>
        /// 分解采集任务是否已经完成
        /// </summary>
        public bool IsCompleted
        {
            //get { return m_TaskSplitData.GatheredUrlCount == m_TaskSplitData.UrlCount ; }
            get { return m_TaskSplitData.UrlCount > 1 && m_TaskSplitData.GatheredUrlCount + m_TaskSplitData.GatheredErrUrlCount  == m_TaskSplitData.TrueUrlCount; }
        }
        /// <summary>
        /// 分解的采集任务是否已经采集完成
        /// </summary>
        public bool IsGathered
        {
            get { return m_TaskSplitData.UrlCount > 0 && m_TaskSplitData.GatheredUrlCount +m_TaskSplitData.GatheredErrUrlCount == m_TaskSplitData.TrueUrlCount ; }
        }
        /// <summary>
        /// 获取一个状态值表示当前线程是否处于合法运行状态 [子线程内部使用]
        /// </summary>
        private bool IsCurrentRunning
        {
            get { return ThreadState  ==cGlobalParas.GatherThreadState.Started  && Thread.CurrentThread.Equals(m_Thread); }
        }

        /// <summary>
        /// 获取当前线程是否已经停止，注意IsThreadRunning只是一个标志
        /// 并不能确定线程的状态，只是告诉线程需要停止了
        /// IsStop是用于外部任务检查此分解任务是否停止使用
        /// </summary>
        public bool IsStop
        {
            get { return m_ThreadRunning ==false && this.IsCurrentRunning==false;  }
        }
        

        /// <summary>
        /// 当前块的工作线程
        /// </summary>
        internal Thread WorkThread
        {
            get { return m_Thread; }
        }

        /// <summary>
        /// 获取当前线程的执行状态
        /// </summary>
        internal bool IsThreadAlive
        {
            get { return m_Thread != null && m_Thread.IsAlive; }
        }

        //public cTaskSplitData TaskSplitData
        //{
        //    get { return m_TaskSplitData; }
        //}
        /// <summary>
        /// 分解任务数据是否已初始化
        /// </summary>
        //public bool IsDataInitialized
        //{
        //    get { return m_IsDataInitialized; }
        //}

        /// <summary>
        /// 起始采集网页地址的索引
        /// </summary>
        public int BeginIndex
        {
            get { return m_TaskSplitData.BeginIndex; }
        }

        /// <summary>
        /// 结束采集网页地址的索引
        /// </summary>
        public int EndIndex
        {
            get { return m_TaskSplitData.EndIndex ; }
        }

        /// <summary>
        /// 当前正在采集地址的索引
        /// </summary>
        public int CurIndex
        {
            get { return m_TaskSplitData.CurIndex; }
        }

        /// <summary>
        /// 当前采集地址
        /// </summary>
        public string CurUrl
        {
            get { return m_TaskSplitData.CurUrl; }
        }

        /// <summary>
        /// 已采集网址数量
        /// </summary>
        public int GatheredUrlCount
        {
            get { return m_TaskSplitData.GatheredUrlCount; }
        }

        public int GatheredTrueUrlCount
        {
            get { return m_TaskSplitData.GatheredTrueUrlCount; }
        }

        /// <summary>
        /// 已采集但出错的网址数量
        /// </summary>
        public int GatherErrUrlCount
        {
            get { return m_TaskSplitData.GatheredErrUrlCount; }
        }

        public int GatheredTrueErrUrlCount
        {
            get { return m_TaskSplitData.GatheredTrueErrUrlCount; }
        }

        /// <summary>
        /// 一共需要采集的网址数量
        /// </summary>
        public int UrlCount
        {
            get { return m_TaskSplitData.UrlCount; }
        }

        public int TrueUrlCount
        {
            get { return m_TaskSplitData.TrueUrlCount; }
        }

        #endregion


        #region 事件
        /// <summary>
        /// 采集任务初始化事件
        /// </summary>
        private event EventHandler<TaskInitializedEventArgs> e_TaskInit;
        internal event EventHandler<TaskInitializedEventArgs> TaskInit
        {
            add { lock (m_eventLock) { e_TaskInit += value; } }
            remove { lock (m_eventLock) { e_TaskInit -= value; } }
        }

        /// <summary>
        /// 分解任务采集开始事件
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Started;
        internal event EventHandler<cTaskEventArgs> Started
        {
            add { lock (m_eventLock) { e_Started += value; } }
            remove { lock (m_eventLock) { e_Started -= value; } }
        }

        /// <summary>
        /// 分解任务停止事件
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Stopped;
        internal event EventHandler<cTaskEventArgs> Stopped
        {
            add { lock (m_eventLock) { e_Stopped += value; } }
            remove { lock (m_eventLock) { e_Stopped -= value; } }
        }

        /// <summary>
        /// 分解任务采集完成事件
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Completed;
        internal event EventHandler<cTaskEventArgs> Completed
        {
            add { lock (m_eventLock) { e_Completed += value; } }
            remove { lock (m_eventLock) { e_Completed -= value; } }
        }

        /// <summary>
        /// 分解任务采集错误事件
        /// </summary>       
        private event EventHandler<TaskThreadErrorEventArgs> e_Error;
        internal event EventHandler<TaskThreadErrorEventArgs> Error
        {
            add { lock (m_eventLock) { e_Error += value; } }
            remove { lock (m_eventLock) { e_Error -= value; } }
        }

        /// <summary>
        ///  分解任务采集失败事件
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Failed;
        internal event EventHandler<cTaskEventArgs> Failed
        {
            add { lock (m_eventLock) { e_Failed += value; } }
            remove { lock (m_eventLock) { e_Failed -= value; } }
        }

        /// <summary>
        /// 写日志事件
        /// </summary>
        private event EventHandler<cGatherTaskLogArgs> e_Log;
        internal event EventHandler<cGatherTaskLogArgs> Log
        {
            add { lock (m_eventLock) { e_Log += value; } }
            remove { lock (m_eventLock) { e_Log -= value; } }
        }
        
        /// <summary>
        /// 返回采集数据事件
        /// </summary>
        private event EventHandler<cGatherDataEventArgs> e_GData;
        internal event EventHandler<cGatherDataEventArgs> GData
        {
            add { lock (m_eventLock) { e_GData += value; } }
            remove { lock (m_eventLock) { e_GData -= value; } }
        }

        private event EventHandler<cGatherUrlCountArgs> e_GUrlCount;
        internal event EventHandler<cGatherUrlCountArgs> GUrlCount
        {
            add { lock (m_eventLock) { e_GUrlCount += value; } }
            remove { lock (m_eventLock) { e_GUrlCount -= value; } }
        }
        #endregion

        #region 事件触发操作
        //事件触发操作是通过改变任务状态来完成的
        //通过各种任务状态的变更，来触发事件，并最终反馈到界面
        //将控制权交给界面

        /// <summary>
        /// 获取当前线程状态
        /// </summary>    
        private cGlobalParas.GatherThreadState m_ThreadState;

        /// <summary>
        /// 设置/获取 线程状态 （仅内部使用，触发事件）
        /// </summary>
        protected cGlobalParas.GatherThreadState ThreadState
        {
            get { return m_ThreadState; }
            set
            {
                m_ThreadState = value;
                // 注意，所以涉及线程状态变更的事件都在此处理
                switch (m_ThreadState)
                {
                    case cGlobalParas.GatherThreadState.Completed:
                        if (e_Completed != null)
                        {
                            // 代理触发 采集下载完成 事件
                            m_TaskManage.EventProxy.AddEvent(delegate()
                            {
                                e_Completed(this, new cTaskEventArgs());
                            });
                        }
                        m_Thread = null;
                        break;
                    case cGlobalParas.GatherThreadState.Failed:
                        if (e_Failed != null)
                        {
                            // 代理触发 线程失败 事件
                            m_TaskManage.EventProxy.AddEvent(delegate()
                            {
                                e_Failed(this, new cTaskEventArgs());
                            });
                        }
                        m_Thread = null;
                        break;
                    case cGlobalParas.GatherThreadState.Started:
                        if (e_Started != null)
                        {
                            // 触发 线程开始 事件
                            e_Started(this, new cTaskEventArgs());
                        }
                        break;
                    case cGlobalParas.GatherThreadState.Stopped:
                        if (e_Stopped != null)
                        {
                            // 触发 线程停止 事件
                            e_Stopped(this, new cTaskEventArgs());
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 错误处理，发生错误后，并不停止任务，让任务继续进行
        /// 但需要反馈信息告诉用户有一个地址采集错误
        /// </summary>
        /// <param name="exp"></param>
        private void onError(Exception exp)
        {
            if (this.IsCurrentRunning)
            {
                // 不触发Stopped事件
                //m_ThreadState = cGlobalParas.GatherThreadState.Stopped;

                if (e_Error != null)
                {
                    // 代理触发 线程错误 事件
                    m_TaskManage.EventProxy.AddEvent(delegate()
                    {
                        m_ErrorCount++;
                        e_Error(this, new TaskThreadErrorEventArgs(exp));
                    });
                }

                //m_Thread = null;
            }
        }
        #endregion


        #region 线程控制 启动 停止 重启 重置

        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly Object m_threadLock = new Object();

        /// <summary>
        /// 启动工作线程
        /// </summary>
        public void Start()
        {
            if (m_ThreadState != cGlobalParas.GatherThreadState.Started && !IsThreadAlive && !IsCompleted)
            {
                lock (m_threadLock)
                {
                    //根据页面截取数据的输出规则，判断是否需要进行数据合并
                    for (int i = 0; i < this.m_TaskSplitData.CutFlag.Count; i++)
                    {
                        if (this.m_TaskSplitData.CutFlag[i].IsMergeData ==true )
                        {
                            m_IsMergeData = true;
                            break;
                        }
                    }

                    //取下一页翻页的正则规则
                    cXmlSConfig cCon = new cXmlSConfig();
                    RegexNextPage = cTool.ShowTrans( cCon.RegexNextPage);
                    cCon = null;

                    //设置线程运行标志，标识此线程运行
                    m_ThreadRunning = true; 

                    m_Thread = new Thread(this.ThreadWorkInit);

                    //定义线程名称,用于调试使用
                    m_Thread.Name =m_TaskID.ToString() + "-" + m_TaskSplitData.BeginIndex.ToString ();

                    m_Thread.Start();
                    m_ThreadState = cGlobalParas.GatherThreadState.Started;
                }
            }
        }

        /// <summary>
        /// 重新启动工作线程
        /// </summary>
        public void ReStart()
        {   // 仅在子线程外调用
            Stop();
            Start();
        }

        /// <summary>
        /// 停止当前线程
        /// </summary>
        public void Stop()
        {   
            //仅在子线程外调用
            //设置停止线程标志，线程停止只能等待一个地址采集完成后停止，不能
            //强行中断，否则会丢失数据

            ///注意：在此只是打了一个标记，线程并没有真正结束
            m_ThreadRunning = false;


            if (m_ThreadState == cGlobalParas.GatherThreadState.Started && IsThreadAlive)
            {
                lock (m_threadLock)
                {

                    //开始检测是否所有线程都以完成或退出
                    //bool isStop = false;

                    //while (!isStop)
                    //{
                    //    isStop = true;

                    //    if (m_ThreadState == cGlobalParas.GatherThreadState.Started && IsThreadAlive)
                    //        isStop = false;

                        
                    //}
                    
                    m_Thread = null;
                    if (m_ThreadState == cGlobalParas.GatherThreadState.Started)
                    {
                        m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
                    }
                }
            }
            else
            {

                m_Thread = null;
                if (m_ThreadState == cGlobalParas.GatherThreadState.Started)
                {
                    m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
                }
            }

            m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
        }

        public void Abort()
        {
            m_ThreadRunning = false;

            if (m_ThreadState == cGlobalParas.GatherThreadState.Started && IsThreadAlive)
            {
                lock (m_threadLock)
                {
                    if (m_ThreadState == cGlobalParas.GatherThreadState.Started && IsThreadAlive)
                    {
                        m_Thread.Abort();
                        m_Thread = null;
                        m_ThreadState = cGlobalParas.GatherThreadState.Aborted;
                    }
                    else
                    {
                        m_Thread = null;
                        if (m_ThreadState == cGlobalParas.GatherThreadState.Started)
                        {
                            m_ThreadState = cGlobalParas.GatherThreadState.Aborted; ;
                        }
                    }
                }
            }
            else
            {
                m_Thread = null;
                if (m_ThreadState == cGlobalParas.GatherThreadState.Started)
                {
                    m_ThreadState = cGlobalParas.GatherThreadState.Aborted; ;
                }
            }

            m_ThreadState = cGlobalParas.GatherThreadState.Aborted;
        }

        /// <summary>
        /// 重置线程块为未初始化状态
        /// </summary>
        internal void Reset()
        {
            e_Completed = null;
            e_Error = null;
            e_Started = null;
            e_Stopped = null;
            e_TaskInit = null;
            m_IsInitialized = false;

            e_Log = null;
            e_GData = null;
        }

        #endregion

        #region 任务线程处理（采集网页数据） 执行一个采集分解任务

        /// <summary>
        /// 采集初始化线程
        /// </summary>
        private void ThreadWorkInit()
        {
            if (!m_IsInitialized)
            {
                ///按当前的处理方式，只要任务可以启动，就已经初始化了任务信息
                ///但是有一种情况未做处理，就是带有导航的网址，导航网址需要根据实际
                ///的解析情况才可以获得真正的需要采集数据的网址，那么就需要对这些实时
                ///解析出来的网址再次根据线程数进行任务拆分，所以需要重新对任务进行
                ///初始化，此类情况当前未进行处理，特别主意
                e_TaskInit(this, new TaskInitializedEventArgs(m_TaskID));
               
            }
            else if (GatheredUrlCount !=TrueUrlCount )
            {
                ThreadWork();
            }
        }

        /// <summary>
        /// 采集任务
        /// </summary>
        private void ThreadWork()
        {
            //cGatherWeb gWeb = new cGatherWeb();

            //gWeb.CutFlag =m_TaskSplitData.CutFlag ;

            bool IsSucceed = false;

            for (int i = 0; i < m_TaskSplitData.Weblink.Count; i++)
            {
                if (m_ThreadRunning == true)
                {
                    switch (m_TaskSplitData.Weblink[i].IsGathered)
                    {
                        case (int) cGlobalParas.UrlGatherResult.UnGather:

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "正在采集：" + m_TaskSplitData.Weblink[i].Weblink + "\n",this.IsErrorLog ));

                            //判断此网址是否为导航网址，如果是导航网址则需要首先将需要采集的网址提取出来
                            //然后进行具体网址的采集
                            if (m_TaskSplitData.Weblink[i].IsNavigation == true)
                            {
                                IsSucceed = GatherNavigationUrl(m_TaskSplitData.Weblink[i].Weblink, m_TaskSplitData.Weblink[i].NavigRules , m_TaskSplitData.Weblink[i].IsNextpage, m_TaskSplitData.Weblink[i].NextPageRule,i);
                            }
                            else
                            {
                                //非导航页 采集页 导航级别默认为：0  同时传递进度的合并数据也为null
                                IsSucceed=GatherSingleUrl(m_TaskSplitData.Weblink[i].Weblink,0, m_TaskSplitData.Weblink[i].IsNextpage, m_TaskSplitData.Weblink[i].NextPageRule,i,null);
                            }

                            //如果采集发生错误，则直接调用了onError进行错误处理
                            //但对于带有导航的网址，一次采集是针对多个网址，如果系统
                            //停止任务，对于多个网址的采集那就是采集事务未成功，既然不成功
                            //则不需要增加GatheredUrlCount及其他处理
                            if (IsSucceed == true)
                            {
                                //每采集完成一个Url，都需要修改当前CurIndex的值，表示系统正在运行，并
                                //最终确定此分解任务是否已经完成,并且表示此网址已经采集完成
                                m_TaskSplitData.CurIndex++;
                                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.UrlCountAdd, 0));


                                //如果是增量任务，则采集标志永远是未采集完成，表示下次启动的时候还需要
                                //进行采集
                                m_TaskSplitData.Weblink[i].IsGathered = (int)cGlobalParas.UrlGatherResult.Succeed;

                                m_TaskSplitData.GatheredUrlCount++;
                            }
                            else
                            {
                                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd, 0));
                            }

                            break;

                        case (int)cGlobalParas.UrlGatherResult.Succeed:
                            m_TaskSplitData.CurIndex++;
                            m_TaskSplitData.GatheredUrlCount++;
                            m_TaskSplitData.GatheredTrueUrlCount++;
                            e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.UrlCountAdd, 0));
                            break;
                        case (int)cGlobalParas.UrlGatherResult.Error:
                            m_TaskSplitData.CurIndex++;
                            m_TaskSplitData.GatheredErrUrlCount++;
                            m_TaskSplitData.GatheredTrueErrUrlCount++;
                            e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd, 0));
                            break;
                        case (int)cGlobalParas.UrlGatherResult.Gathered:
                            m_TaskSplitData.CurIndex++;
                            m_TaskSplitData.GatheredUrlCount++;
                            m_TaskSplitData.GatheredTrueUrlCount++;
                            e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.UrlCountAdd, 0));
                            break;
                    }
                }
                else if (m_ThreadRunning == false)
                {
                    //标示线程需要终止，退出for循环
                    break;
                }


            }
           

            //每采集完成一个网址需要判断当前线程是否已经结束
            //需要判断已采集的网址和采集发生错误的网址
            if (UrlCount == GatheredUrlCount + GatherErrUrlCount && UrlCount != GatherErrUrlCount)
            {
                ThreadState = cGlobalParas.GatherThreadState.Completed;
            }
            else if (UrlCount == GatherErrUrlCount)
            {
                //表示采集失败，但对于一个任务而言，一个线程的完全失败并不代表
                //任务也采集完全失败，所以任务的安全失败需要到任务采集类中判断
                //所以，在此还是返回线程任务完成时间，不是失败时间。
                ThreadState = cGlobalParas.GatherThreadState.Completed;
            }
            else if (UrlCount<GatheredUrlCount + GatherErrUrlCount)
            {
                ThreadState = cGlobalParas.GatherThreadState.Completed;
            }
            else
            {
                ThreadState = cGlobalParas.GatherThreadState.Stopped;
            }

            m_ThreadRunning = false;

        }

        //用于采集一个网页的数据，即不带导航处理，但需要处理翻页
        private bool GatherSingleUrl(string Url,int Level,bool IsNext,string NextRule,int index,DataRow tRow)
        {
            cGatherWeb gWeb = new cGatherWeb();
            DataTable tmpData;
            DataTable tmpMergeData;
            

            //gWeb.CutFlag = m_TaskSplitData.CutFlag;

            bool IsAjax = false;

            if (m_TaskType == cGlobalParas.TaskType.AjaxHtmlByUrl)
                IsAjax = true;

            //在此处理Url编码的问题
            if (m_IsUrlEncode == true)
            {
                Url = cTool.UrlEncode(Url, (cGlobalParas.WebCode)int.Parse(m_UrlEncode));
            }

            //在此处理是否需要进行Base64编码的的问题
            if (Regex.IsMatch(Url, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
            {

                Match s = Regex.Match(Url, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
                string sBase64 = s.Groups[0].Value.ToString();
                sBase64 = cTool.Base64Encoding(sBase64);

                //将base64编码部分进行url替换
                Regex.Replace(Url, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            }

            string NextUrl = Url;
            string Old_Url = NextUrl;

            try
            {
                if (IsNext)
                {
                    #region 如果存在下一页规则，则开始进行下一页规则的解析和数据采集

                    tmpMergeData = new DataTable();

                    do
                    {
                        if (m_ThreadRunning == true)
                        {
                            Url = NextUrl;
                            Old_Url = NextUrl;

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "正在采集：" + Url + "\n", this.IsErrorLog));

                            tmpData = GetGatherData(Url, Level, m_WebCode, m_Cookie, m_gStartPos, m_gEndPos, m_SavePath, IsAjax);

                            ///不明代码 被注释，测试看是否存在问题
                            //if (tmpData != null)
                            //{
                            //    m_GatherData.Merge(tmpData);
                            //}

                            ///每次采集完成数据后，都需要判断是否进行数据合并
                            if (m_IsMergeData == true)
                            {
                                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "此页数据需要进行合并，等待下一页数据采集\n", this.IsErrorLog));
                                tmpMergeData.Merge(tmpData);
                            }
                            else
                            {

                                //触发日志及采集数据的事件
                                if (tmpData == null || tmpData.Rows.Count == 0)
                                {

                                }
                                else
                                {
                                    //合并上层传递来的数据
                                    if (tRow != null && tmpData != null)
                                    {
                                        //合并数据导航数据
                                        tmpData = MergeDataTable(tRow, tmpData);
                                    }


                                    e_GData(this, new cGatherDataEventArgs(m_TaskID, tmpData));
                                }
                            }

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "采集完成：" + Url + "\n", this.IsErrorLog));


                            e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "开始根据下一页规则获取下一页网址\n", this.IsErrorLog));


                            string webSource = gWeb.GetHtml(Url, m_WebCode, m_Cookie, "", "", true, IsAjax);


                            //string NRule = "((?<=href=[\'|\"])\\S[^#+$<>\\s]*(?=[\'|\"]))[^<]*" + "(?<=" + NextRule + ")";
                            
                            string NRule = RegexNextPage + "(?<=" + NextRule + ")";
                            Match charSetMatch = Regex.Match(webSource, NRule, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            string strNext = charSetMatch.Groups[1].Value;

                            if (strNext != "")
                            {
                                //判断获取的地址是否为相对地址
                                if (strNext.Substring(0, 1) == "/")
                                {
                                    string PreUrl = Url;
                                    PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                                    PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));
                                    PreUrl = "http://" + PreUrl;
                                    strNext = PreUrl + strNext;
                                }
                                else if (strNext.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    NextUrl = strNext;
                                }
                                else if (strNext.StartsWith("?", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Match aa = Regex.Match(Url, @".*(?=\?)");
                                    string PreUrl = aa.Groups[0].Value.ToString();
                                    if (PreUrl =="" )
                                        strNext = Url + strNext;
                                    else
                                        strNext = PreUrl + strNext;
                                }
                                else
                                {
                                    Match aa = Regex.Match(Url, ".*/");
                                    string PreUrl = aa.Groups[0].Value.ToString();
                                    strNext = PreUrl + strNext;
                                }

                                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "下一页网址获取成功：" + NextUrl + "\n", this.IsErrorLog));
                            }
                            else
                            {
                                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "已经到最终页：" + NextUrl + "\n", this.IsErrorLog));
                            }
                            NextUrl = strNext;

                            //更新地址，因为是自动翻页，所以采集任务无法感知翻页到了那里，所以需要
                            //在此进行更新
                            m_TaskSplitData.Weblink[index].NextPageUrl = NextUrl;

                        }
                        else if (m_ThreadRunning == false)
                        {
                            //标识要求终止线程，停止任务，退出do循环提前结束任务
                            if (NextUrl == "" || Old_Url == NextUrl)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        
                        
                    }
                    while (NextUrl != "" && Old_Url != NextUrl);

                    //判断是否为合并数据，如果是，则将合并数据向上返回
                    if (m_IsMergeData == true)
                    {
                        e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "开始进行数据合并\n", this.IsErrorLog));
                        tmpMergeData = MergeData(tmpMergeData);


                        if (tRow  != null && tmpMergeData != null)
                        {
                            //合并数据导航数据
                            tmpMergeData = MergeDataTable(tRow, tmpMergeData);
                        }


                        e_GData(this, new cGatherDataEventArgs(m_TaskID, tmpMergeData));
                        e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info,"数据合并成功\n", this.IsErrorLog));
                        tmpMergeData = null;
                    }

                    #endregion

                }
                else
                {

                    tmpData = GetGatherData(Url, Level, m_WebCode, m_Cookie, m_gStartPos, m_gEndPos, m_SavePath, IsAjax);

                    ///不明代码 被注释，测试看是否存在问题
                    //if (tmpData != null)
                    //{
                    //    m_GatherData.Merge(tmpData);
                    //}

                    //触发日志及采集数据的事件
                    if (tmpData == null || tmpData.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        if (tRow != null && tmpData != null)
                        {
                            //合并数据导航数据
                            tmpData = MergeDataTable(tRow, tmpData);
                        }


                        e_GData(this, new cGatherDataEventArgs(m_TaskID, tmpData));
                    }
                    e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "采集完成：" + Url + "\n", this.IsErrorLog));
                }
                

                //触发采集网址计数事件
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));

                m_TaskSplitData.GatheredTrueUrlCount++;

            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, Url + "采集发生错误：" + ex.Message + "\n", this.IsErrorLog));
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd, 0));
                m_TaskSplitData.GatheredTrueErrUrlCount++;
                m_TaskSplitData.GatheredErrUrlCount++;
                onError(ex);
                return false;
            }

            gWeb = null;
            tmpData = null;

            return true;
        }

        ///这是采集带有导航规则的网址数据的入口
        ///导航规则分为两类：一是下一页的导航规则；而是页面导航，
        ///此方法传入地址后，主要处理下一页的规则，然后调用ParseGatherNavigationUrl
        ///处理页面导航的问题
        private bool GatherNavigationUrl(string Url, List<Task.cNavigRule> nRules, bool IsNext, string NextRule,int index)
        {
            cGatherWeb gWeb = new cGatherWeb();
            //gWeb.CutFlag = m_TaskSplitData.CutFlag;
            
            bool IsSucceed = false;

            //在此处理网址编码解析
            if (m_IsUrlEncode == true)
            {
                Url = cTool.UrlEncode(Url, (cGlobalParas.WebCode)int.Parse(m_UrlEncode));
            }

            //在此处理是否需要进行Base64编码的的问题
            if (Regex.IsMatch(Url, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
            {

                Match s = Regex.Match(Url, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
                string sBase64 = s.Groups[0].Value.ToString();
                sBase64 = cTool.Base64Encoding(sBase64);

                //将base64编码部分进行url替换
                Regex.Replace(Url, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            }

            string NextUrl = Url;
            string Old_Url = NextUrl;

            try
            {

                if (IsNext)
                {
                    #region 处理导航 且具备下一页自动翻页的采集
                    do
                    {
                        if (m_ThreadRunning == true)
                        {
                            Url = NextUrl;
                            Old_Url = NextUrl;

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "正在采集：" + Url + "\n", this.IsErrorLog));

                            IsSucceed = ParseGatherNavigationUrl(Url,1,nRules,index,null) ; //, NagRule, IsOppPath);

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "采集完成：" + Url + "\n", this.IsErrorLog));

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "开始根据下一页规则获取下一页网址\n", this.IsErrorLog));

                            bool IsAjax = false;

                            if (m_TaskType == cGlobalParas.TaskType.AjaxHtmlByUrl)
                                IsAjax = true;

                            //开始分解下一页网址
                            string webSource = gWeb.GetHtml(Url, m_WebCode, m_Cookie, "", "",true,IsAjax );

                            //string NRule = "((?<=href=[\'|\"])\\S[^#+$<>\\s]*(?=[\'|\"]))[^<]*" + "(?<=" + NextRule + ")";

                            string NRule = RegexNextPage + "(?<=" + NextRule + ")";
                            Match charSetMatch = Regex.Match(webSource, NRule, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            string strNext = charSetMatch.Groups[1].Value;

                            if (strNext != "")
                            {
                                //判断获取的地址是否为相对地址
                                if (strNext.Substring(0, 1) == "/")
                                {
                                    string PreUrl = Url;
                                    PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                                    PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));
                                    PreUrl = "http://" + PreUrl;
                                    strNext = PreUrl + strNext;
                                }
                                else if (strNext.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    NextUrl = strNext;
                                }
                                else if (strNext.StartsWith("?", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Match aa = Regex.Match(Url, @".*(?=\?)");
                                    string PreUrl = aa.Groups[0].Value.ToString();

                                    if (PreUrl == "")
                                        strNext = Url + strNext;
                                    else
                                        strNext = PreUrl + strNext;

                                    strNext = PreUrl + strNext;
                                }
                                else
                                {
                                    Match aa = Regex.Match(Url, ".*/");
                                    string PreUrl = aa.Groups[0].Value.ToString();
                                    strNext = PreUrl + strNext;
                                }

                                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "下一页网址获取成功：" + NextUrl + "\n", this.IsErrorLog));
                            }
                            else
                            {
                                e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "已经到最终页" + "\n", this.IsErrorLog));
                            }

                            NextUrl = strNext;

                            //更新地址，因为是自动翻页，所以采集任务无法感知翻页到了那里，所以需要
                            //在此进行更新
                            m_TaskSplitData.Weblink[index].NextPageUrl  = NextUrl;

                        }
                        else if (m_ThreadRunning == false)
                        {
                            //标识要求终止线程，停止任务，退出do循环提前结束任务
                            if (NextUrl == "" || Old_Url == NextUrl)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                            //break;
                        }

                    }
                    while (NextUrl != "" && Old_Url != NextUrl);

                    #endregion
                }
                else
                {

                    IsSucceed = ParseGatherNavigationUrl(Url,1, nRules,index ,null ); //, NagRule, IsOppPath);
                }
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, Url + "采集发生错误：" + ex.Message + "\n", this.IsErrorLog));
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd, 0));
                m_TaskSplitData.GatheredTrueErrUrlCount++;
                m_TaskSplitData.GatheredErrUrlCount++;
                onError(ex);
                return false;
            }

            gWeb = null;

            return IsSucceed;
        }

        ///用于采集需要导航的网页，在此处理导航页规则，在2009-10-28日，增加了导航处理自动翻页的情况
        ///所以，在此需要进行递归调用，原有方式是将Url和导航规则（可能是多级）通过UrlAnalysis分析后
        ///得出所有的网址，但此种算法需要修改，即分级进行导航，将网址提取之后再进行分页处理，如果一个网址
        ///两种规则都处理结束后，再进行下一个网址的处理。
        private bool ParseGatherNavigationUrl(string Url,int level, List<Task.cNavigRule> nRules,int index, DataRow tRow)
        {
            //因为这是一个递归调用的函数，所以，首先需要判断当前是否已经要求停止线程工作
            //如果停止线程工作，则停止调用，开始返回
            if (m_ThreadRunning == false)
                return false;

            Task.cUrlAnalyze u = new Task.cUrlAnalyze();

            List<string> gUrls;                                          //记录导航返回的Url列表
            List<cNavigRule> tmpNRules = new List<cNavigRule>();         //仅记录当前导航级别的规则 是一个集合，因为是分层导航，所以一个集合仅记录一条
            cNavigRule tmpNRule = new cNavigRule();                      //记录当前导航级别的导航规则

            //发现当前导航级别的导航规则
            for (int i = 0; i < nRules.Count; i++)
            {
                if (level == nRules[i].Level)
                {
                    tmpNRule.Level = 1;
                    tmpNRule.NaviStartPos = nRules[i].NaviStartPos;
                    tmpNRule.NaviEndPos = nRules[i].NaviEndPos;
                    tmpNRule.NavigRule = nRules[i].NavigRule;

                    tmpNRule.IsNaviNextPage = nRules[i].IsNaviNextPage;
                    tmpNRule.NaviNextPage = nRules[i].NaviNextPage;
                    tmpNRule.IsGather = nRules[i].IsGather;
                    tmpNRule.GatherStartPos = nRules[i].GatherStartPos;
                    tmpNRule.GatherEndPos = nRules[i].GatherEndPos;

                    tmpNRules.Add(tmpNRule);
                    break;
                }
            }

            e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "开始根据导航规则获取网页地址，请等待......\n导航层级为：" + nRules.Count + " 层，正在进行第" + level + "层导航\n", this.IsErrorLog));

            //根据导航规则找到网址
            gUrls = u.ParseUrlRule(Url, tmpNRules, m_WebCode, m_Cookie);

            u = null;
            if (gUrls == null || gUrls.Count == 0)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, Url + " 导航解析失败，有可能是由于导航规则配置错误，也有可能是由于垃圾数据造成，如果是垃圾数据，则不影响系统对数据的采集\n", this.IsErrorLog));
                return false;
            }

            e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "成功根据导航规则获取" + gUrls.Count + "个网址\n", this.IsErrorLog));

            //更新实际采集网址总数，因是导航页面，所以实际采集网址总数发生了变化
            //通过事件触发更新任务的采集数量总数，同时更新子任务的采集总数
            //注意，仅更新实际采集网址的总数，但不更新网址总数，此是两个值，各自维护各自的业务逻辑处理

            //系统进行了任务导航的分解操作，此时，已经修改了需要采集任务的总数，所以，需要更新子任务的实际采集网址的数量
            //同时还需触发相应的事件修改整个任务的采集网址的总数
            m_TaskSplitData.TrueUrlCount += gUrls.Count - 1;
            e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ReIni, gUrls.Count));


            //判断是否为内容页，如果是内容页，则开始进行内容页的采集
            //同时如果不是内容页，且具备采集要求的导航页，需要首先先把此导航页的采集数据
            //采集出来，然后作为参数进行递归传递，一直到采集页进行最终的数据合并

            cGatherWeb gWeb = new cGatherWeb();
            DataTable tData=new DataTable ();

            //判断此导航页是否需要进行数据采集
            if (tmpNRule.IsGather == true)
            {
                bool IsAjax = false;

                if (m_TaskType == cGlobalParas.TaskType.AjaxHtmlByUrl)
                    IsAjax = true;

                tData = new DataTable();

                tData = GetGatherData(Url, level, m_WebCode, m_Cookie, tmpNRule.GatherStartPos, tmpNRule.GatherEndPos, m_SavePath, IsAjax);

                if (tRow != null && tData != null)
                {
                    //合并数据导航数据
                    tData = MergeDataTable(tRow, tData);
                }

            }

            if (level == nRules.Count)
            {
                GatherParsedUrl(gUrls,level , tmpNRule.IsNaviNextPage, tmpNRule.NaviNextPage,index,tData);
            }
            else
            {
               
                for (int j = 0; j < gUrls.Count; j++)
                {
                    DataRow dr;
                    if (tData != null && tData.Rows.Count != 0)
                    {
                        if (j > tData.Rows.Count && tData != null)
                        {
                            dr = tData.Rows[tData.Rows.Count - 1];
                        }
                        else
                        {
                            dr = tData.Rows[j];
                        }
                    }
                    else
                    {
                        dr = null;
                    }

                    //循环调用，需判断线程的终止状态
                    //在此处理自动翻页的规则

                    if (tmpNRule.IsNaviNextPage)
                    {
                        #region 在此处理分页导航的规则

                        string NextUrl = gUrls[j].ToString ();
                        string Old_Url = NextUrl;

                        do
                        {
                            
                            if (m_ThreadRunning == true)
                            {
                                Url = NextUrl;
                                Old_Url = NextUrl;

                                ParseGatherNavigationUrl(Url, level + 1, nRules, index, dr);

                                e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "开始根据下一页规则获取下一页网址\n", this.IsErrorLog));

                                bool IsAjax = false;

                                if (m_TaskType == cGlobalParas.TaskType.AjaxHtmlByUrl)
                                    IsAjax = true;

                                string webSource = gWeb.GetHtml(Url, m_WebCode, m_Cookie, "", "", true, IsAjax);

                                string NRule = "((?<=href=[\'|\"])\\S[^#+$<>\\s]*(?=[\'|\"]))[^<]*(?<=" + tmpNRule.NaviNextPage + ")";
                                Match charSetMatch = Regex.Match(webSource, NRule, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                string strNext = charSetMatch.Groups[1].Value;

                                if (strNext != "")
                                {
                                    //判断获取的地址是否为相对地址
                                    if (strNext.Substring(0, 1) == "/")
                                    {
                                        string PreUrl = Url;
                                        PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                                        PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));
                                        PreUrl = "http://" + PreUrl;
                                        strNext = PreUrl + strNext;
                                    }
                                    else if (strNext.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        NextUrl = strNext;
                                    }
                                    else
                                    {
                                        Match aa = Regex.Match(Url, ".*/");
                                        string PreUrl = aa.Groups[0].Value.ToString();
                                        strNext = PreUrl + strNext;
                                    }

                                    e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "下一页网址获取成功：" + NextUrl + "\n", this.IsErrorLog));
                                }
                                else
                                {
                                    e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "已经到最终页" + "\n", this.IsErrorLog));
                                }

                                NextUrl = strNext;


                            }
                            else if (m_ThreadRunning == false)
                            {
                               
                                return false;

                            }

                        }
                        while (NextUrl != "" && Old_Url != NextUrl);

                        #endregion
                    }
                    else
                    {
                        if (m_ThreadRunning == true)
                        {
                            ParseGatherNavigationUrl(gUrls[j].ToString(), level + 1, nRules, index, dr);
                        }
                        else if (m_ThreadRunning == false)
                        {
                            return false ;
                        }
                    }
                    dr = null;
                }
            }

            tData=null;

            gUrls = null;

            tmpNRule = null;
            tmpNRules = null;

            return true;
        }

        //用于采集导航网页分解后的集合网址，即最终的内容页
        private bool GatherParsedUrl(List<string> gUrls,int Level, bool IsNext,string NextPageRule,int index,DataTable tmpData)
        {
            bool IsSucceed = false;

            for (int j = 0; j < gUrls.Count; j++)
            {
                if (m_ThreadRunning == true)
                {
                    try
                    {
                        DataRow dr;
                        if (tmpData != null && tmpData.Rows.Count != 0)
                        {
                            if (j > tmpData.Rows.Count)
                            {
                                dr = tmpData.Rows[tmpData.Rows.Count - 1];
                            }
                            else
                            {
                                dr = tmpData.Rows[j];
                            }
                        }
                        else
                        {
                            dr = null;
                        }


                        //既然是最终页采集，那代表采集规则的导航层级别必然为0
                        IsSucceed = GatherSingleUrl(gUrls[j].ToString(), 0, IsNext, NextPageRule, index, dr);

                        //触发采集网址计数事件
                        e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));
                        m_TaskSplitData.GatheredTrueUrlCount++;

                        dr = null;

                    }
                    catch (System.Exception ex)
                    {
                        e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                        m_TaskSplitData.GatheredTrueErrUrlCount++;
                        onError(ex);
                    }

                }
                else if (m_ThreadRunning == false)
                {
                    //标识要求终止线程，停止任务，退出for循环提前结束任务
                    if (j == gUrls.Count)
                    {
                        //表示还是采集完成了
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }

            return true;
        }

        #endregion

        //这是一个通讯的接口方法，不做采集规则的处理，所有需要采集的网页均调用此防范
        //由此方法调用cGatherWeb.GetGatherData，做次方法的目的是为了可以处理错误重试

        private DataTable GetGatherData(string Url,int Level, cGlobalParas.WebCode webCode, string cookie, string startPos, string endPos, string sPath, bool IsAjax)
        {

            //让采集线程休眠
            if (m_GIntervalTime != 0)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Warning, "采集任务被人为设置每成功采集过一次，停止" + m_GIntervalTime.ToString() + "毫秒继续进行！\n", this.IsErrorLog));
                Thread.Sleep(m_GIntervalTime);
            }


            cGatherWeb gWeb = new cGatherWeb();

            //只有iscustomheader=true 并且 ispublishheader=false 才进行自定义的header
            if (this.IsCustomHeader == true && this.IsPublishHeader == false)
            {
                gWeb.IsCustomHeader = true;
                gWeb.Headers = this.Headers;
            }
            else
            {
                gWeb.IsCustomHeader = false;
            }
            
            ///在此处理传递进去的采集规则，是符合当前采集级别的规则，采集页默认为0
            ///当导航页的采集时，需要根据导航页的级别制定采集规则

            List<cWebpageCutFlag> CutFlags = new List<cWebpageCutFlag>();
            cWebpageCutFlag CutFlag = new cWebpageCutFlag();
            for (int i = 0; i < m_TaskSplitData.CutFlag.Count; i++)
            {
                if (m_TaskSplitData.CutFlag[i].NavLevel ==Level )
                    CutFlags.Add (m_TaskSplitData.CutFlag [i]);
            }

            gWeb.CutFlag = CutFlags;

            //设置当前的采集的网址
            m_TaskSplitData.CurUrl = Url;

            DataTable tmpData ;
            int AgainTime = 0;

            GatherAgain:

            try
            {
                tmpData = gWeb.GetGatherData(Url, webCode, cookie, startPos, endPos, m_SavePath, IsAjax, IsExportGUrl, IsExportGDateTime);
            }
            catch (System.Exception ex)
            {
                AgainTime++;
                
                if (AgainTime > m_AgainNumber)
                {
                    if (m_IsErrorLog == true)
                    {
                        //保存出错日志
                    }

                    throw ex;
                }
                else
                {
                    if (m_Ignore404 == true && ex.Message.Contains ("404"))
                    {
                        if (m_IsErrorLog == true)
                        {
                            //保存出错日志
                        }

                        throw ex;
                    }
                    else
                    {
                        e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, "网址：" + Url + "访问发生错，错误信息：" + ex.Message + "，等待3秒重试\n", this.IsErrorLog));
                       
                        Thread.Sleep(3000);

                        e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Warning,Url + "正在进行第" + AgainTime + "次重试\n", this.IsErrorLog));

                        //返回重试
                        goto GatherAgain;
                    }
                }
            }


            //在此处理是否直接入库，如果需要进行直接入库，则在此进行数据库的插入
            //插入后，不返回数据，直接将tempData置为空
            //如果用户选择了直接入库，则必须是采集页的内容，不能是导航页的内容 所以必须level=0

            if (this.TaskRunType == cGlobalParas.TaskRunType.OnlySave && tmpData!=null && Level ==0)
            {
                NewTable(tmpData.Columns);
                InsertData(tmpData);
                tmpData = null;
            }

            return tmpData;
        }

        #region 合并数据操作
        private DataTable MergeData(DataTable tmpData)
        {
            if (tmpData == null || tmpData.Rows.Count == 0)
                return null;

            object oldRow;
            object newRow;
            DataTable d1 = tmpData.Clone();
            object[] mRow;

            d1 = tmpData.Clone();

            oldRow = tmpData.Rows[0].ItemArray.Clone();

            

            for (int i = 0; i<tmpData.Rows.Count; i++)
            {
                newRow = tmpData.Rows[i].ItemArray.Clone();
                mRow = MergeRow(oldRow, newRow);

                DataRow r = d1.NewRow();

                if (mRow != null)
                {
                    oldRow = mRow.Clone();
                    r.ItemArray = ((object[])oldRow);
                    if (d1.Rows.Count > 0)
                    {
                        d1.Rows.Remove(d1.Rows[d1.Rows.Count - 1]);
                        d1.Rows.Add(r);
                    }
                    else
                    {
                        d1.Rows.Add(r);
                    }
                }
                else
                {
                    r.ItemArray =((object[]) oldRow);
                    d1.Rows.Add(r);
                    oldRow = newRow;
                }
            }

            return d1;
        }

        private object[] MergeRow(object row1, object row2)
        {
            object[] oldRow = ((object[])row1);
            object[] newRow = ((object[])row2);
            object[] mergeRow = new object [this.m_TaskSplitData.CutFlag.Count];

            for (int i = 0; i < this.m_TaskSplitData.CutFlag.Count; i++)
            {
                if (this.m_TaskSplitData.CutFlag[i].IsMergeData == false)
                {
                    if (oldRow[i].ToString() == newRow[0].ToString())
                    {
                        mergeRow[i] = oldRow[i].ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    mergeRow[i] = oldRow[i].ToString() + newRow[i].ToString ();
                }
            }
            return mergeRow;
        }
        #endregion

        #region 直接入库 代码
        private void InsertData(DataTable tmpData)
        {
            if (IsNewTable == true)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Error, "数据表不存在，无法保存采集的数据，请查阅日志！" + "\n", this.IsErrorLog));
                ThreadState = cGlobalParas.GatherThreadState.Failed;
            }

            //判断存储数据库的类别
            switch (this.PublishType)
            {
                case cGlobalParas.PublishType .PublishAccess :
                    ExportAccess(tmpData);
                    break;
                case cGlobalParas.PublishType .PublishMSSql  :
                    ExportMSSql(tmpData);
                    break;
                case cGlobalParas.PublishType .PublishMySql :
                    ExportMySql(tmpData);
                    break;
            }
        }

        #region 判断是否建立新表
        private void NewTable(DataColumnCollection dColumns)
        {
            //首先判断表是否存在，如果不存在则进行建立
            if (IsNewTable == true)
            {
                bool isSucceed = false;

                switch (this.PublishType)
                {
                    case cGlobalParas.PublishType.PublishAccess:
                        isSucceed = NewAccessTable(dColumns);
                        break;
                    case cGlobalParas.PublishType.PublishMSSql:
                        isSucceed = NewMSSqlServerTable(dColumns);
                        break;
                    case cGlobalParas.PublishType.PublishMySql:
                        isSucceed = NewMySqlTable(dColumns);
                        break;
                }

                if (isSucceed == true)
                    IsNewTable = false;
            }
           
        }

        private bool NewAccessTable(DataColumnCollection dColumns)
        {
            bool IsTable = false;

            OleDbConnection conn = new OleDbConnection();
            string connectionstring = this.DataSource;
            conn.ConnectionString = connectionstring;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Error, "数据直接入库，数据库连接出错，错误信息：" + ex.Message + "。无法进行入库操作！" + "\n", this.IsErrorLog));
                return false ;
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[3].ToString() == "TABLE")
                {
                    if (r[2].ToString() == this.TableName)
                    {
                        IsTable = true;
                        break;
                    }
                }

            }

            if (IsTable == false)
            {
                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.Access, "", dColumns);

                OleDbCommand com = new OleDbCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (System.Data.OleDb.OleDbException ex)
                {
                    if (ex.ErrorCode != -2147217900)
                    {
                        
                        e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, "数据库建表发生错误，错误信息：" + ex.Message + "\n", this.IsErrorLog));
                        conn.Close();
                        return false ;
                    }
                }

                IsTable = true;

            }

            conn.Close();

            return IsTable;
        }

        private bool NewMSSqlServerTable(DataColumnCollection dColumns)
        {
             bool IsTable = false;

            SqlConnection conn = new SqlConnection();
            string connectionstring = this.DataSource;
            conn.ConnectionString = connectionstring;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, "数据直接入库，数据库连接出错，错误信息：" + ex.Message  + "。无法进行入库操作！" + "\n", this.IsErrorLog));
                return false ;
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[2].ToString()==this.TableName )
                {
                    IsTable = true;
                    break;
                }
            }

            if (IsTable == false)
            {
                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.MSSqlServer, "", dColumns);

                SqlCommand com = new SqlCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    if (ex.ErrorCode != -2147217900)
                    {
                        e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, "数据库建表发生错误，错误信息：" + ex.Message + "\n", this.IsErrorLog));
                        conn.Close();
                        return false;
                    }
                }
                IsTable = true;
            }

            conn.Close();

            return IsTable;
        }

        private bool NewMySqlTable(DataColumnCollection dColumns)
        {
            bool IsTable = false;

            MySqlConnection conn = new MySqlConnection();
            string connectionstring = this.DataSource;

            conn.ConnectionString = connectionstring;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, "数据直接入库，数据库连接出错，错误信息：" + ex.Message + "。无法进行入库操作！" + "\n", this.IsErrorLog));
                return false;
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (string.Compare(r[2].ToString(), this.TableName, true) == 0)
                {
                    IsTable = true;
                    break;
                }
            }

            if (IsTable == false)
            {
                //通过连接字符串把编码获取出来，根据编码进行数据表的建立
                string strMatch = "(?<=character set=)[^\\s]*(?=[\\s;])";
                Match s = Regex.Match(connectionstring, strMatch, RegexOptions.IgnoreCase);
                string Encoding = s.Groups[0].Value;

                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.MySql, Encoding, dColumns);

                MySqlCommand com = new MySqlCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    if (ex.ErrorCode != -2147217900)
                    {
                        e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, "数据库建表发生错误，错误信息：" + ex.Message + "\n", this.IsErrorLog));
                        return false ;
                    }
                }

                IsTable = true;
            }

            return IsTable;
        }

        private string getCreateTablesql(cGlobalParas.DatabaseType dType, string Encoding, DataColumnCollection dColumns)
        {
            string strsql = "";

            strsql = "create table " + this.TableName + "(";
            for (int i = 0; i < dColumns.Count; i++)
            {
                switch (dType)
                {
                    case cGlobalParas.DatabaseType.Access:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                    case cGlobalParas.DatabaseType.MSSqlServer:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                    case cGlobalParas.DatabaseType.MySql:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                    default:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                }
            }
            strsql = strsql.Substring(0, strsql.Length - 1);
            strsql += ")";

            //如果是mysql数据库，需要根据连接串的字符集进行数据表的建立
            if (dType == cGlobalParas.DatabaseType.MySql)
            {
                if (Encoding == "" || Encoding == null)
                    Encoding = "utf8";

                strsql += " CHARACTER SET " + Encoding + " ";
            }

            return strsql;
        }
        #endregion

        #region 直接入库 access mssqlserver mysql
        private void ExportAccess(DataTable tmpData)
        {
            bool IsTable = false;

            OleDbConnection conn = new OleDbConnection();

            string connectionstring = this.DataSource;

            conn.ConnectionString = connectionstring;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Error, "数据直接入库，数据库连接出错，无法进行入库操作！" +  "\n", this.IsErrorLog));
                return;
            }

            //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
            System.Data.OleDb.OleDbCommand cm = new System.Data.OleDb.OleDbCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            //开始拼sql语句
            string sql = "";

            for (int i = 0; i < tmpData.Rows.Count; i++)
            {
                sql = this.InsertSql;

                for (int j = 0; j < tmpData.Columns.Count; j++)
                {
                    string strPara = "{" + tmpData.Columns[j].ColumnName + "}";
                    string strParaValue = tmpData.Rows[i][j].ToString().Replace("\"", "\"\"");
                    sql = sql.Replace(strPara, strParaValue);
                }

                try
                {
                    cm.CommandText = sql;
                    cm.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error,  tmpData.Rows.ToString () + "插入失败，错误信息：" + ex.Message  +"\n", this.IsErrorLog));
                }
            }

            conn.Close();

        }

        private void ExportMSSql(DataTable tmpData)
        {
            bool IsTable = false;

            SqlConnection conn = new SqlConnection();

            string connectionstring = this.DataSource;

            conn.ConnectionString = connectionstring;

           
        
            //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
           

            //开始拼sql语句
            string strInsertSql = this.InsertSql;

            //需要将双引号替换成单引号
            //strInsertSql = strInsertSql.Replace("\"", "'");

            string sql = "";

            for (int i = 0; i <tmpData.Rows.Count; i++)
            {
                sql = strInsertSql;

                for (int j = 0; j < tmpData.Columns.Count; j++)
                {
                    string strPara = "{" + tmpData.Columns[j].ColumnName + "}";
                    string strParaValue = tmpData.Rows[i][j].ToString().Replace("\"", "\"\"");
                    sql = sql.Replace(strPara, strParaValue);
                }
                
                try
                {
                    conn.Open();
                }
                catch (System.Exception ex)
                {

                    e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, "数据直接入库，数据库连接出错，无法进行入库操作！" + "\n", this.IsErrorLog));
                    return;
                }

                try
                {
                    SqlCommand cm = new SqlCommand();
                    cm.Connection = conn;
                    cm.CommandType = CommandType.Text;
                    cm.CommandTimeout = 10;

                    cm.CommandText = sql;
                    cm.ExecuteNonQuery();
                    conn.Close();
                }
                catch (System.Exception ex)
                {
                    e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, tmpData.Rows.ToString() + "插入失败，错误信息：" + ex.Message + "\n", this.IsErrorLog));

                }
            }
            
        }

        private void ExportMySql(DataTable tmpData)
        {
            bool IsTable = false;

            MySqlConnection conn = new MySqlConnection();

            string connectionstring = this.DataSource;

            conn.ConnectionString = connectionstring;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Error, "数据直接入库，数据库连接出错，无法进行入库操作！" + "\n", this.IsErrorLog));
                return;
            }

            //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
            MySqlCommand cm = new MySqlCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            //开始拼sql语句
            string strInsertSql = this.InsertSql;


            string sql = "";

            for (int i = 0; i < tmpData.Rows.Count; i++)
            {
                sql = strInsertSql;

                for (int j = 0; j < tmpData.Columns.Count; j++)
                {
                    string strPara = "{" + tmpData.Columns[j].ColumnName + "}";
                    string strParaValue = tmpData.Rows[i][j].ToString().Replace("\"", "\"\"");
                    sql = sql.Replace(strPara, strParaValue);
                }

                try
                {
                    cm.CommandText = sql;
                    cm.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, tmpData.Rows.ToString() + "插入失败，错误信息：" + ex.Message + "\n", this.IsErrorLog));
                }
            }

            conn.Close();
        }
        #endregion

        #endregion

        #region 将两个datatable的数据合并在一起
        //将datarow与datatable合并在一起 这是一个一对N的关系
        private DataTable MergeDataTable(DataRow dtr1, DataTable dt2)
        {
            DataTable dt1 = dtr1.Table.Clone();
            dt1.ImportRow(dtr1);

            DataTable dt3 = dt1.Clone();
            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                dt3.Columns.Add(dt2.Columns[i].ColumnName);
            }
            object[] obj = new object[dt3.Columns.Count];

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                dt3.Rows.Add(obj);
            }


            if (dt1.Rows.Count >= dt2.Rows.Count)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                    }
                }
            }
            else
            {
                DataRow dr3;
                for (int i = 0; i < dt2.Rows.Count - dt1.Rows.Count; i++)
                {
                    dr3 = dt3.NewRow();
                    dt3.Rows.Add(dr3);
                }
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                    }
                }
            }

            return dt3;
        }

        #endregion

        #region 设置任务分解数据，由外部调用

        /// <summary>
        /// 设置分解任务的数据,分解任务的数据主要包括了起始采集
        /// 的网页索引,终止采集的网页索引,一共需要采集的网页总数
        /// </summary>
        /// <param name="beginIndex">起始的采集网页地址</param>
        /// <param name="endIndex">终止的采集网页地址</param>
        public void SetSplitData(int beginIndex, int endIndex,List<Task.cWebLink> tUrl,List<Task.cWebpageCutFlag> tCutFlag)
        {
            lock (m_mstreamLock)
            {
                m_TaskSplitData.BeginIndex = beginIndex;
                m_TaskSplitData.CurIndex = beginIndex;
                m_TaskSplitData.EndIndex = endIndex;
                m_TaskSplitData.Weblink = tUrl;
                m_TaskSplitData.TrueUrlCount = tUrl.Count;
                m_TaskSplitData.CutFlag = tCutFlag;
                //m_IsDataInitialized = true;
            }
        }
       
        #endregion

        #region IDisposable 成员
        private bool m_disposed;
        /// <summary>
        /// 释放由 采集 的当前实例使用的所有资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {

                }

                m_disposed = true;
            }
        }

        #endregion

    }
}

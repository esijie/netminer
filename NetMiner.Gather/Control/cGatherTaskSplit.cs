using System;
using System.Collections.Generic;
using System.Threading;
using System.Data;
using System.Text.RegularExpressions;
using NetMiner.Core.Proxy;
using NetMiner.Common;
using NetMiner.Resource;
using System.Collections;
using NetMiner.Core.gTask.Entity;
using NetMiner.Core;
using NetMiner.Gather.Url;
using NetMiner.Core.Event;
using NetMiner.Net.Native;
using NetMiner.Core.Entity;
using NetMiner.Net.Common;

///功能：采集任务 分解子任务处理
///完成时间：2009-6-1
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
///修订：2010-12-11
///增加了导航自动翻页的功能，自动翻页通过不同级别的导航翻页规则进行
//////修订 2011年10月8日 增加了排重库的处理，任务版本升级为：V2.1 主要处理网址排重
///系统版本升级为：V2.6.0.03
///2012-2-11 重新修正采集网址计数问题，当进行导航采集，并且采集出错，或遇到意外，会导致
///计数错误，从而最终影响系统无法完成
///这是一个子线程运行的独立任务，包含了属性数据及操作方法，V5.5版本，对此类进行拆分，太臃肿。

namespace NetMiner.Gather.Control
{

    //这是采集任务的最小单元,针对于一个采集任务而言,是一个URL地址集合
    //存在有多个甚至上千个URL地址,为了提高性能,对URl采集采用了多线程的
    //处理方式,所以针对一个任务要进行拆分,拆分的依据是此任务定制的线程
    //数,任务拆后开始执行.此类主要就是执行这么一个最小单元的任务.
    //一个cGatherTaskSplit想当于一个线程，但后期会在此增加多线程的处理，因为
    //如果入口页面小于线程数，则系统默认就是单线程了。

    public class cGatherTaskSplit : IDisposable
    {

        private delegate void work();

        private Thread m_Thread;
        //private bool m_ThreadRunning = false;

        private string RegexNextPage = "";

        //设置一个参数值，判断，如果数据采集是采用了直接入库的方式
        //判断数据表是否需要新建，如果新建，则在第一次运行时，建立新表
        //成功后，并修改此时为false，如果已经存在此表，也许修改为false
        private bool IsNewTable = true;

        //定义一个代理信息控制类，并传引用给采集类，根据采集任务的
        //代理配置信息，去获取代理
        private cProxyControl m_ProxyControl;

        //定义一个排重库，进行url排重
        private NetMiner.Base.cHashTree m_Urls;

        //定义一个排重库，进行当前任务实例运行的排重，主要解决断点续采的问题
        private NetMiner.Base.cHashTree m_TempUrls;

        //定义一个排重库，进行采集数据的重复性判断
        private NetMiner.Base.cHashTree m_DataRepeat;

        //定义一个值，来记录连续出错的次数（注意：是连续）
        private int m_ContinuousErr;

        //定义一个值，来记录连续直接入库的错误次数（注意：是连续）
        private int m_ContinuousInsertErr;

        //定义一个值，表示记录当前的工作路径
        private string m_workPath = string.Empty;

        //定义一个值，存储Cookie
        private string m_tmpCookie = string.Empty;

        //定义一个值，判断是否进行全局排重，全局排重仅限采集引擎
        private bool m_isGlobalRepeat;
        public NetMiner.Base.cHashTree g_Urls;

        cGatherData gData = null;

        #region 构造类，并初始化相关数据

        /// <summary>
        /// 2011年10月8日修改了构造函数结构，将排重库引用传入进来
        /// </summary>
        /// <param name="ProxyControl"></param>
        internal cGatherTaskSplit(string workPath, ref cProxyControl ProxyControl, bool isGlobalRepeat, 
            ref NetMiner.Base.cHashTree gUrls, ref NetMiner.Base.cHashTree Urls, ref NetMiner.Base.cHashTree tUrls, 
            ref NetMiner.Base.cHashTree dRepeat)
        {
            this.m_workPath = workPath;

            m_ContinuousErr = 0;
            m_ContinuousInsertErr = 0;

            m_TaskSplitData = new cTaskSplitData();
            m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
            m_GatherData = new DataTable();

            m_ProxyControl = ProxyControl;
            m_Urls = Urls;
            m_TempUrls = tUrls;
            m_DataRepeat = dRepeat;

            m_isGlobalRepeat = isGlobalRepeat;
            g_Urls = gUrls;

            this.TaskEntity = new eTask();

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
            if (gData != null)
            {
                gData.Log -= this.on_Log;
                gData.GUrlCount -= this.on_GatherUrlCount;
                gData.Error -= this.onError;
                gData.GData -= this.on_GatherData;
                gData = null;
            }

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


        private cTaskSplitData m_TaskSplitData;
        public cTaskSplitData TaskSplitData
        {
            get { return m_TaskSplitData; }
            set { m_TaskSplitData = value; }
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

  
        //V3.0增加
        private string[] m_AutoID;
        public string[] AutoID
        {
            get { return m_AutoID; }
            set { m_AutoID = value; }
        }

        public cGlobalParas.ProxyType pType { get; set; }
        public string ProxyAddress { get; set; }
        public int ProxyPort { get; set; }

        public eTask TaskEntity { get; set; }

        public int cookieIndex { get; set; }
     
        #endregion

        #region 任务运行状态属性

        /// <summary>
        /// 分解采集任务是否已经完成
        /// </summary>
        public bool IsCompleted
        {
            //get { return m_TaskSplitData.GatheredUrlCount == m_TaskSplitData.UrlCount ; }
            //get { return m_TaskSplitData.UrlCount > 1 && m_TaskSplitData.GatheredUrlCount + m_TaskSplitData.GatheredErrUrlCount  == m_TaskSplitData.TrueUrlCount; }
            get
            {
                if (m_TaskSplitData.UrlCount == m_TaskSplitData.GatheredUrlCount + m_TaskSplitData.GatheredErrUrlCount &&
                    m_TaskSplitData.UrlNaviCount == m_TaskSplitData.GatheredUrlNaviCount + m_TaskSplitData.GatheredErrUrlNaviCount)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 分解的采集任务是否已经采集完成
        /// </summary>
        public bool IsGathered
        {
            //get { return m_TaskSplitData.UrlCount > 0 && m_TaskSplitData.GatheredUrlCount +m_TaskSplitData.GatheredErrUrlCount == m_TaskSplitData.TrueUrlCount ; }
            get
            {
                if (m_TaskSplitData.UrlCount == m_TaskSplitData.GatheredUrlCount + m_TaskSplitData.GatheredErrUrlCount &&
                    m_TaskSplitData.UrlNaviCount == m_TaskSplitData.GatheredUrlNaviCount + m_TaskSplitData.GatheredErrUrlNaviCount)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 获取一个状态值表示当前线程是否处于合法运行状态 [子线程内部使用]
        /// </summary>
        private bool IsCurrentRunning
        {
            get { return ThreadState == cGlobalParas.GatherThreadState.Started && Thread.CurrentThread.Equals(m_Thread); }
        }

        /// <summary>
        /// 获取当前线程是否已经停止，注意IsThreadRunning只是一个标志
        /// 并不能确定线程的状态，只是告诉线程需要停止了
        /// IsStop是用于外部任务检查此分解任务是否停止使用
        /// </summary>
        public bool IsStop
        {
            get { return gData.ThreadRunning == false && this.IsCurrentRunning == false; }
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
            get { return m_TaskSplitData.EndIndex; }
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

        public int GatheredUrlNaviCount
        {
            get { return m_TaskSplitData.GatheredUrlNaviCount; }
        }

        /// <summary>
        /// 已采集但出错的网址数量
        /// </summary>
        public int GatherErrUrlCount
        {
            get { return m_TaskSplitData.GatheredErrUrlCount; }
        }

        public int GatheredErrUrlNaviCount
        {
            get { return m_TaskSplitData.GatheredErrUrlNaviCount; }
        }

        /// <summary>
        /// 一共需要采集的网址数量
        /// </summary>
        public int UrlCount
        {
            get { return m_TaskSplitData.UrlCount; }
        }

        public int UrlNaviCount
        {
            get { return m_TaskSplitData.UrlNaviCount; }
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

        //请求停止采集任务事件
        private event EventHandler<cTaskEventArgs> e_RequestStopped;
        internal event EventHandler<cTaskEventArgs> RequestStopped
        {
            add { lock (m_eventLock) { e_RequestStopped += value; } }
            remove { lock (m_eventLock) { e_RequestStopped -= value; } }
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

        private event EventHandler<cUpdateCookieArgs> e_UpdateCookie;
        internal event EventHandler<cUpdateCookieArgs> UpdateCookie
        {
            add { lock (m_eventLock) { e_UpdateCookie += value; } }
            remove { lock (m_eventLock) { e_UpdateCookie -= value; } }
        }
        #endregion

        #region 事件触发操作
        //事件触发操作是通过改变任务状态来完成的
        //通过各种任务状态的变更，来触发事件，并最终反馈到界面
        //将控制权交给界面

        private delegate void delegateSetThreadState(cGlobalParas.GatherThreadState tState);
        private void SetThreadState(cGlobalParas.GatherThreadState tState)
        {
            ThreadState = tState;
        }

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

                            //更新Cookie
                            this.TaskEntity.Cookie = new cCookieManage(m_tmpCookie).getCookie().Value;

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

                        //在此检测线程是否停止，如果停止，则触发停止事件

                        //while(IsThreadAlive)
                        //{
                        //    Thread.Sleep(10);
                        //}


                        if (e_Stopped != null)
                        {
                            //更新Cookie
                            this.TaskEntity.Cookie = new cCookieManage(m_tmpCookie).getCookie().Value;

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
        private void onError(object sender, TaskThreadErrorEventArgs e)
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
                        e_Error(this, e);
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
            try
            {
                if (m_ThreadState != cGlobalParas.GatherThreadState.Started && !IsThreadAlive && !IsCompleted)
                {
                    lock (m_threadLock)
                    {
                        //根据页面截取数据的输出规则，判断是否需要进行数据合并
                        //for (int i = 0; i < this.m_TaskSplitData.CutFlag.Count; i++)
                        //{
                        //    if (this.m_TaskSplitData.CutFlag[i].IsMergeData == true)
                        //    {
                        //        m_IsMergeData = true;
                        //        break;
                        //    }
                        //}

                        //取下一页翻页的正则规则
                        cXmlSConfig cCon = new cXmlSConfig(this.m_workPath);
                        RegexNextPage = ToolUtil.ShowTrans(cCon.RegexNextPage);
                        cCon = null;

                        gData = new cGatherData(this.TaskEntity, this.TaskSplitData, this.m_workPath, ref m_ProxyControl);

                        gData.AutoID = this.AutoID;
                        gData.ProxyAddress = this.ProxyAddress;
                        gData.ProxyPort = this.ProxyPort;
                        gData.RegexNextPage = this.RegexNextPage;

                        gData.m_isGlobalRepeat = this.m_isGlobalRepeat;
                        gData.g_Urls = this.g_Urls;
                        gData.m_DataRepeat = this.m_DataRepeat;
                        gData.m_Urls = this.m_Urls;
                        gData.m_TempUrls = this.m_TempUrls;

                        gData.Log += this.on_Log;
                        gData.GUrlCount += this.on_GatherUrlCount;
                        gData.Error += this.onError;
                        gData.GData += this.on_GatherData;
                        gData.RequestStopped += this.on_ForceStop;

                        //设置线程运行标志，标识此线程运行
                        gData.ThreadRunning = true;

                        m_Thread = new Thread(this.ThreadWorkInit);

                        //在此设置线程的类型
                        if (this.TaskEntity .IsVisual==false)
                            m_Thread.SetApartmentState(ApartmentState.MTA);
                        else
                            m_Thread.SetApartmentState(ApartmentState.STA);

                        //定义线程名称,用于调试使用
                        m_Thread.Name = this.TaskEntity.TaskID.ToString() + "-" + m_TaskSplitData.BeginIndex.ToString();
                        
                        m_Thread.Start();
                        m_ThreadState = cGlobalParas.GatherThreadState.Started;
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
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
            gData.ThreadRunning = false;


            //if (m_ThreadState == cGlobalParas.GatherThreadState.Started && IsThreadAlive)
            //{
            //    lock (m_threadLock)
            //    {
            //        if (m_ThreadState == cGlobalParas.GatherThreadState.Started)
            //        {
            //            m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
            //        }
            //    }
            //}
            //else
            //{
            //    if (m_ThreadState == cGlobalParas.GatherThreadState.Started)
            //    {
            //        m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
            //    }
            //}

            //ThreadState = cGlobalParas.GatherThreadState.Stopped;
        }

        public void Abort()
        {
            gData.ThreadRunning = false;

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
        /// 暂停当前线程执行一个时间间隔
        /// </summary>
        /// <param name="interval"></param>
        public void Pause(float interval)
        {
            //Thread.Sleep((int)(interval * 1000));

           this.TaskEntity.GatherCountPauseInterval = interval;
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

        /// <summary>
        /// 强制停止任务
        /// </summary>
        private void StopTaskRule(cGlobalParas.StopRule sRule)
        {
            if (sRule == cGlobalParas.StopRule.StopUrlGather)
            {
                throw new NetMinerSkipUrlException("满足用户设置的跳过此网址采集的规则，可能是采集出错或采集到重复数据");
            }
            else if (sRule == cGlobalParas.StopRule.StopTaskGather)
            {
                if (e_RequestStopped != null)
                    e_RequestStopped(this, null);
            }
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
                e_TaskInit(this, new TaskInitializedEventArgs(this.TaskEntity.TaskID));

            }
            else //if (GatheredUrlCount !=TrueUrlCount )
            {
                ThreadWork();
            }
        }

        /// <summary>
        /// 采集任务
        /// </summary>
        private void ThreadWork()
        {
            

            cGlobalParas.GatherResult IsSucceed = cGlobalParas.GatherResult.UnGather;

            for (int i = 0; i < m_TaskSplitData.Weblink.Count; i++)
            {
                m_tmpCookie = this.TaskEntity.Cookie;

                if (gData.ThreadRunning == true)
                {

                    try
                    {
                        switch (m_TaskSplitData.Weblink[i].IsGathered)
                        {
                            case (int)cGlobalParas.UrlGatherResult.UnGather:

                                //e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "正在采集：" + m_TaskSplitData.Weblink[i].Weblink ,this.IsErrorLog ));

                                //判断此网址是否为导航网址，如果是导航网址则需要首先将需要采集的网址提取出来
                                //然后进行具体网址的采集
                                #region 处理采集的操作
                                if (m_TaskSplitData.Weblink[i].IsNavigation == true)
                                {
                                    List<eMultiPageRule> listMultiRules = null;
                                    //在此判断是否需要进行多页采集
                                    if (m_TaskSplitData.Weblink[i].IsMultiGather == true)
                                    {
                                        listMultiRules = m_TaskSplitData.Weblink[i].MultiPageRules;
                                    }

                                    IsSucceed = gData.GatherNavigationUrl(m_TaskSplitData.Weblink[i].Weblink, m_TaskSplitData.Weblink[i].NavigRules,
                                        i, m_TaskSplitData.Weblink[i].IsData121, listMultiRules,"");

                                }
                                else
                                {

                                    //在此判断是否需要进行多页采集
                                    if (m_TaskSplitData.Weblink[i].IsMultiGather == true)
                                    {
                                        List<eMultiPageRule> listMultiRules = null;
                                        listMultiRules = m_TaskSplitData.Weblink[i].MultiPageRules;
                                        IsSucceed = gData.GatherSingleUrl(m_TaskSplitData.Weblink[i].Weblink, 0, m_TaskSplitData.Weblink[i].IsNextpage, m_TaskSplitData.Weblink[i].NextPageRule, i, null, m_TaskSplitData.Weblink[i].IsData121, listMultiRules, "");

                                    }
                                    else
                                    {
                                        //非导航页 采集页 导航级别默认为：0  同时传递进度的合并数据也为null
                                        IsSucceed = gData.GatherSingleUrl(m_TaskSplitData.Weblink[i].Weblink, 0, 
                                            m_TaskSplitData.Weblink[i].IsNextpage, m_TaskSplitData.Weblink[i].NextPageRule, 
                                            m_TaskSplitData.Weblink[i].NextMaxPage,i, null, "","",0);
                                    }
                                }
                                #endregion

                                //采集网址计数处理
                                m_TaskSplitData.CurIndex++;

                                #region 处理计数的问题
                                if (IsSucceed == cGlobalParas.GatherResult.GatherSucceed
                                    || IsSucceed == cGlobalParas.GatherResult.GatherStoppedByUser)
                                {

                                    //每采集完成一个Url，都需要修改当前CurIndex的值，表示系统正在运行，并
                                    //最终确定此分解任务是否已经完成,并且表示此网址已经采集完成
                                    if (m_TaskSplitData.Weblink[i].IsNavigation == true)
                                    {
                                        //需要判断导航网址是否采集完成
                                        if (UrlNaviCount == GatheredUrlNaviCount + GatheredErrUrlNaviCount)
                                        {
                                            //导航采集完成
                                            m_TaskSplitData.Weblink[i].IsGathered = (int)cGlobalParas.UrlGatherResult.Succeed;
                                            m_TaskSplitData.GatheredUrlCount++;
                                            e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));
                                        }
                                        //else if (UrlNaviCount > GatheredUrlNaviCount + GatheredErrUrlNaviCount && IsSucceed == cGlobalParas.GatherResult.GatherSucceed)
                                        //{
                                        //    m_TaskSplitData.Weblink[i].IsGathered = (int)cGlobalParas.UrlGatherResult.Gathered;
                                        //    m_TaskSplitData.GatheredUrlCount++;
                                        //    e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));

                                        //}
                                        else
                                        {
                                            m_TaskSplitData.Weblink[i].IsGathered = (int)cGlobalParas.UrlGatherResult.UnGather;
                                            m_TaskSplitData.GatheredErrUrlCount++;
                                            e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                                        }
                                    }
                                    else
                                    {
                                        m_TaskSplitData.Weblink[i].IsGathered = (int)cGlobalParas.UrlGatherResult.Succeed;
                                        m_TaskSplitData.GatheredUrlCount++;
                                        e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));

                                    }

                                }
                                else if (IsSucceed == cGlobalParas.GatherResult.GatherFailed)
                                {
                                    e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));

                                    m_TaskSplitData.Weblink[i].IsGathered = (int)cGlobalParas.UrlGatherResult.Error;
                                    m_TaskSplitData.GatheredErrUrlCount++;
                                }
                                #endregion

                                break;

                            case (int)cGlobalParas.UrlGatherResult.Succeed:
                                m_TaskSplitData.CurIndex++;
                                m_TaskSplitData.GatheredUrlCount++;
                                e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));
                                break;
                            case (int)cGlobalParas.UrlGatherResult.Error:
                                m_TaskSplitData.CurIndex++;
                                m_TaskSplitData.GatheredErrUrlCount++;
                                e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                                break;
                            case (int)cGlobalParas.UrlGatherResult.Gathered:
                                m_TaskSplitData.CurIndex++;
                                m_TaskSplitData.GatheredUrlCount++;
                                e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));
                                break;
                        }
                    }
                    catch (NetMinerSkipUrlException)
                    {
                        //在此捕获停止当前Url采集进入下一条Url采集的操作，
                        //此为网络矿工特有的操作
                        //所以当采集成功处理
                        m_TaskSplitData.CurIndex++;
                        m_TaskSplitData.GatheredUrlCount++;
                        e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));
                    }
                    catch (System.Exception)
                    {
                        m_TaskSplitData.CurIndex++;
                        m_TaskSplitData.GatheredErrUrlCount++;
                        e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                    }
                }
                else if (gData.ThreadRunning == false)
                {
                    //标示线程需要终止，退出for循环
                    //任务被终止了
                    break;
                }

                if (e_UpdateCookie != null)
                    e_UpdateCookie(this, new cUpdateCookieArgs(cookieIndex, m_tmpCookie));

              
            }

            //每采集完成一个网址需要判断当前线程是否已经结束
            //需要判断已采集的网址和采集发生错误的网址
            //2013-1-24进行修改，增加了实际采集网址数量的判断，否则无法准确判断
            //采集任务时候已经完成，遇到导航就出错了

            delegateSetThreadState dSet = new delegateSetThreadState(SetThreadState);

            if (UrlCount == GatheredUrlCount + GatherErrUrlCount && UrlNaviCount == GatheredUrlNaviCount + GatheredErrUrlNaviCount)
            {
                //ThreadState = cGlobalParas.GatherThreadState.Completed;
                dSet.BeginInvoke(cGlobalParas.GatherThreadState.Completed, null, null);

            }
            else if (UrlCount == GatherErrUrlCount && UrlNaviCount == GatheredErrUrlNaviCount)
            {
                //表示采集失败，但对于一个任务而言，一个线程的完全失败并不代表
                //任务也采集完全失败，所以任务的安全失败需要到任务采集类中判断
                //所以，在此还是返回线程任务完成时间，不是失败时间。
                //ThreadState = cGlobalParas.GatherThreadState.Completed;

                //ThreadState = cGlobalParas.GatherThreadState.Completed;
                dSet.BeginInvoke(cGlobalParas.GatherThreadState.Completed, null, null);
            }
            else if (UrlCount < GatheredUrlCount + GatherErrUrlCount && UrlNaviCount <= GatheredUrlNaviCount + GatheredErrUrlNaviCount)
            {
                //ThreadState = cGlobalParas.GatherThreadState.Completed;

                //ThreadState = cGlobalParas.GatherThreadState.Completed;
                dSet.BeginInvoke(cGlobalParas.GatherThreadState.Completed, null, null);
            }
            else
            {
                //ThreadState = cGlobalParas.GatherThreadState.Stopped;

                //ThreadState = cGlobalParas.GatherThreadState.Completed;
                dSet.BeginInvoke(cGlobalParas.GatherThreadState.Stopped, null, null);
            }

            gData.ThreadRunning = false;

        }

        //用于采集一个网页的数据，专门用于处理多页采集数据，多页暂不处理分页
        /// <summary>
        /// 2012-9-27 对多页采集进行处理，原有方式是可以通过网页源码获取多页的网址
        /// 增加了可以通过网页源码采集参数，并传入到多页网址进行采集。
        /// </summary>
        /// 

    
        #endregion

        #region 设置任务分解数据，由外部调用

        /// <summary>
        /// 设置分解任务的数据,分解任务的数据主要包括了起始采集
        /// 的网页索引,终止采集的网页索引,一共需要采集的网页总数
        /// </summary>
        /// <param name="beginIndex">起始的采集网页地址</param>
        /// <param name="endIndex">终止的采集网页地址</param>
        public void SetSplitData(int beginIndex, int endIndex, List<eWebLink> tUrl, List<eWebpageCutFlag> tCutFlag)
        {
            lock (m_mstreamLock)
            {
                m_TaskSplitData.BeginIndex = beginIndex;
                m_TaskSplitData.CurIndex = beginIndex;
                m_TaskSplitData.EndIndex = endIndex;
                m_TaskSplitData.Weblink = tUrl;
                //m_TaskSplitData.TrueUrlCount = tUrl.Count;
                //m_TaskSplitData.UrlCount = tUrl.Count;
                m_TaskSplitData.UrlNaviCount = 0;
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

        #region 处理采集类的日志信息
        //处理日志事件,因为采集类事件无法明确采集的任务ID，因此在此进行补充
        public void on_Log(object sender, cGatherTaskLogArgs e)
        {
            if (e_Log != null)
                e_Log(sender, new cGatherTaskLogArgs(this.TaskEntity.TaskID, TaskEntity.TaskName, e.LogType, e.strLog, TaskEntity.IsErrorLog));
        }

        public void on_GatherUrlCount(object sender,cGatherUrlCountArgs e)
        {
            if (e_GUrlCount != null)
                e_GUrlCount(sender, e);
        }

        public void on_GatherData(object sender, cGatherDataEventArgs e)
        {
            if (e_GData != null)
                e_GData(sender, e);
        }

        /// <summary>
        /// 强制停止采集任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void on_ForceStop(object sender, cTaskEventArgs e)
        {
            if(e_RequestStopped !=null)
                e_RequestStopped(this, null);
        }

        #endregion

    }
}

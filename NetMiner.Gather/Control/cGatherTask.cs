using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using NetMiner.Core.Log;
using NetMiner.Gather.Listener;
using NetMiner.Core.Proxy;
using NetMiner.Common;
using NetMiner.Resource;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;
using NetMiner.Core.Plan.Entity;
using NetMiner.Core.Event;
using NetMiner.Net.Native;
using NetMiner.Core.Entity;

///功能：采集任务处理
///完成时间：2010-8-3
///作者：一孑
///遗留问题：无
///开发计划：无
///说明： 此类于2010年8月3日进行了版本升级，主要升级操作时修改了splitTask方法，使其支持直接从采集任务本身
/// 的文件提取数据，而并非只从run目录下提取数据，这样做的目的是为了支持雷达监控采集，普通采集会建立运行列表
/// 但雷达运行不会建立运行列表，所以根据TaskID从run目录下就无法提取数据，所以，需要修改接口，以适应雷达
/// 应用，版本为：2.0
///版本：02.10.00
///修订：无
///修订 2011年10月8日 增加了排重库的处理，任务版本升级为：V2.1 主要处理网址排重
///系统版本升级为：V2.6.0.03
namespace NetMiner.Gather.Control
{
    ///采集任务类,根据一个采集任务(即一个Task.xml文件)

    public class cGatherTask
    {
        private cTaskData m_TaskData;
        private cGlobalParas.TaskState m_State;
        private List<cGatherTaskSplit> m_list_GatherTaskSplit;
        private bool m_IsDataInitialized;
        private bool m_IsInitialized;
        private cGatherManage m_TaskManage;

        private bool m_ThreadsRunning = false;

        //定义一个变量，判断是否保存任务的运行状态
        //如果是正常任务则必须保存，如果是雷达监控任务
        //则无需保存
        private bool m_IsSaveTask = true ;

        //定义一个代理信息控制类，并传引用给采集类，根据采集任务的
        //代理配置信息，去获取代理
        private cProxyControl m_ProxyControl;

        //定义一个属性，进行Url排重
        private NetMiner.Base.cHashTree m_Urls;

        //定义一个属性，进行断点续采时采集网址的记录，就是一个排重库
        private NetMiner.Base.cHashTree m_TempUrls;

        //定义一个属性，判断采集数据是否重复
        private NetMiner.Base.cHashTree m_DataRepeat;

        //定义一个值，进行全局排重
        private bool m_isGlobalRepeat = false;
        private NetMiner.Base.cHashTree m_GlobalUrls;

        //定义一个值，判断当前的代理是如何分配
        private bool m_isProxySplit = false;

        private string m_workPath = string.Empty;

        /// <summary>
        /// 定义一个采集任务的实体类，用于获取采集任务的数据信息
        /// </summary>
        private eTask m_Task = null;

        #region 构造 析构 
  
        internal cGatherTask(string workPath, bool isGlobalRepeat, ref NetMiner.Base.cHashTree g_Urls,  cGatherManage taskManage,
            ref cProxyControl ProxyControl, cTaskData taskData)
        {
            m_workPath = workPath;

            m_TaskManage = taskManage;
            m_TaskData = taskData;
            m_ProxyControl = ProxyControl;
            m_State = TaskData.TaskState;

            m_isGlobalRepeat = isGlobalRepeat;
            m_GlobalUrls = g_Urls;

            m_list_GatherTaskSplit = new List<cGatherTaskSplit>();

            //根据传入的taskdata来加载采集任务的基础数据，因为采集任务此时已经拷贝至采集任务
            //运行区，因此从采集任务运行区加载此任务的基础数据
            oTask t = new oTask(this.m_workPath);
            t.LoadTask(TaskData.TaskID);
            this.m_Task = t.TaskEntity;
            t.Dispose();
            t = null;


            #region 定义一个采集数据的排重库
            m_DataRepeat = new NetMiner.Base.cHashTree();
            try
            {
                m_DataRepeat.Open(this.m_workPath + "tasks\\run\\data" + this.TaskName + ".db");
            }
            catch (System.Exception ex)
            {
                if (e_Log != null)
                {
                    e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID,this.m_TaskData.TaskName , cGlobalParas.LogType.Error, "数据文件打开失败！" + ex.Message, true));
                }
            }
            #endregion

            #region 新建一个排重库，用于当前任务执行时断点续采网址排重
            m_TempUrls = new NetMiner.Base.cHashTree();
            try
            {
                m_TempUrls.Open(m_workPath + "tasks\\run\\task" + this.TaskID + ".db");
            }
            catch (System.Exception ex)
            {
                if (e_Log != null)
                {
                    e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID,this.m_TaskData.TaskName, cGlobalParas.LogType.Error, "数据文件打开失败！" + ex.Message, true));
                }
            }
            #endregion

            //当任务数据传进来之后,直接对当前任务进行任务分解,
            //是否需要多线程进行,并初始化相关数据内容
            SplitTask();

            ////开始初始化任务
            //TaskInit();

            m_IsSaveTask = true;
        }

        /// <summary>
        /// 此方法专用于雷达监控和测试采集
        /// </summary>
        /// <param name="taskManage"></param>
        /// <param name="ProxyControl"></param>
        /// <param name="taskData"></param>
        /// <param name="TaskName"></param>
        /// <param name="isExportUrl">雷达监控强制输出采集地址，测试采集则根据配置进行传入</param>
        //internal cGatherTask(string workPath, cGatherManage taskManage, ref cProxyControl ProxyControl, 
        //    cTaskData taskData, string TaskName, bool isExportUrl)
        //{
        //    this.m_workPath = workPath;
        //    m_TaskManage = taskManage;
        //    m_TaskData = taskData;
        //    m_State = TaskData.TaskState;
        //    m_ProxyControl = ProxyControl;

        //    m_list_GatherTaskSplit = new List<cGatherTaskSplit>();

        //    //定义一个采集数据的排重库
        //    m_DataRepeat = new NetMiner.Base.cHashTree();
        //    try
        //    {
        //        m_DataRepeat.Open(m_workPath + "tasks\\run\\data" + this.TaskName + ".db");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        if (e_Log != null)
        //        {
        //            e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID,this.m_TaskData.TaskName, cGlobalParas.LogType.Error, "数据文件打开失败！" + ex.Message, true));
        //        }
        //    }

        //    //新建一个排重库，用于当前任务执行时断点续采网址排重
        //    m_TempUrls = new NetMiner.Base.cHashTree();
        //    try
        //    {
        //        m_TempUrls.Open(m_workPath + "tasks\\run\\task" + this.TaskID + ".db");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        if (e_Log != null)
        //        {
        //            e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID,this.m_TaskData.TaskName, cGlobalParas.LogType.Error, "数据文件打开失败！" + ex.Message, true));
        //        }
        //    }

        //    //当任务数据传进来之后,直接对当前任务进行任务分解,
        //    //是否需要多线程进行,并初始化相关数据内容
        //    SplitTask(TaskName, isExportUrl);

        //    //开始初始化任务
        //    TaskInit();

        //    m_IsSaveTask = false;
        //}

        ~cGatherTask()
        {
            m_DataRepeat = null;
            m_TempUrls = null;
            m_Urls = null;
        }

        #endregion


        #region 属性

        private string[] AutoID { get; set; }
        private cCookieManage cookieManage { get; set; }
        /// <summary>
        /// 事件 线程同步锁
        /// </summary>
        private readonly Object m_eventLock = new Object();
        /// <summary>
        /// 文件 线程同步锁
        /// </summary>
        private readonly Object m_fstreamLock = new Object();
        public cGatherManage TaskManage
        {
            get { return m_TaskManage; }
        }

        public bool IsInitialized
        {
            get { return m_IsInitialized; }
            set { m_IsInitialized = value; }
        }

        public bool ThreadsRunning
        {
            get { return m_ThreadsRunning; }
        }

        /// <summary>
        /// 设置/获取 当前任务数据对象
        /// </summary>
        public cTaskData TaskData
        {
            get { return m_TaskData; }
        }

        public eTask TaskEntity
        {
            get { return m_Task; }
        }
        /// <summary>
        /// 获取采集网址的个数
        /// </summary>
        public int UrlCount
        {
            get { return m_TaskData.UrlCount; }
        }

        /// <summary>
        /// 获取实际需要采集的网址数量
        /// </summary>
        public int UrlNaviCount
        {
            get { return m_TaskData.UrlNaviCount; }
        }

        /// <summary>
        /// 实际采集的线程数，因为入口地址如果是1，线程是3，系统还是会按照1个线程来执行。
        /// </summary>
        public int ThreadCount { get; set; }
        /// <summary>
        /// 获取已完成采集任务的数量
        /// </summary>
        public int GatheredUrlCount
        {
            get { return m_TaskData.GatheredUrlCount; }
        }

        public int GatheredUrlNaviCount
        {
            get { return m_TaskData.GatheredUrlNaviCount; }
        }

        /// <summary>
        /// 获取采集失败网址的数量
        /// </summary>
        public int GatherErrUrlCount
        {
            get { return m_TaskData.GatherErrUrlCount; }
        }

        public int GatheredErrUrlNaviCount
        {
            get { return m_TaskData.GatheredErrUrlNaviCount; }
        }

        /// <summary>
        /// 设置/获取 任务ID
        /// </summary>
        public Int64 TaskID
        {
            get { return m_TaskData.TaskID; }
        }

        /// <summary>
        /// 设置/获取 任务名
        /// </summary>
        public string TaskName
        {
            get { return m_TaskData.TaskName; }
        }
       

        public cGlobalParas.PublishType  PublishType
        {
            get { return m_TaskData.PublishType; }
        }

        public cGlobalParas.TaskType TaskType
        {
            get { return m_TaskData.TaskType; }
        }
       
        /// <summary>
        /// 获取采集任务的运行类型
        /// </summary>
        public cGlobalParas.TaskRunType RunType
        {
            get { return m_TaskData.RunType; }
        }
        /// <summary>
        /// 是否已经采集完成
        /// </summary>
        public bool IsCompleted
        {
            get { return GatheredUrlCount ==UrlCount ; }
        }

        /// <summary>
        /// 分解任务类 结合
        /// </summary>
        public List<cGatherTaskSplit> GatherTaskSplit
        {
            get { return m_list_GatherTaskSplit; }
            set { m_list_GatherTaskSplit = value; }
        }

        //V5.1增加

        public DateTime StartTimer
        {
            get { return m_TaskData.StartTimer; }
        }

        #endregion


        #region 事件触发 任务状态触发
        /// 任务状态改变的事件触发
        /// /// 设置/获取 任务状态 （仅内部使用，触发事件）
        /// 
        public cGlobalParas.TaskState TaskState
        {
            get { return m_State; }
        }

        public cGlobalParas.TaskState State
        {
            get { return m_State; }
            set
            {
                cGlobalParas.TaskState tmp = m_State;
                m_State = value;
                TaskStateChangedEventArgs evt = null;

                if (e_TaskStateChanged != null)
                {
                    
                    evt = new TaskStateChangedEventArgs(TaskID, tmp, m_State);
                    e_TaskStateChanged(this, evt);
                }

                // 注意，所以涉及任务状态变更的事件都在此处理
                bool cancel = (evt != null && evt.Cancel);

                switch (m_State)
                {
                    case cGlobalParas.TaskState.Aborted:
                        // 触发 任务强制停止取消 事件
                        //任务强制停止，任然保存数据，但有可能会丢失数据因为在此系统要推出，系统会忽略
                        //所有错误
                        if (e_TaskAborted != null)
                        {
                            Save();
                            e_TaskAborted(this, new cTaskEventArgs(TaskID,TaskName,cancel));
                        }
                        break;
                    case cGlobalParas.TaskState.Completed:
                        // 触发 任务完成 事件
                        if (e_TaskCompleted != null)
                        {

                            //当任务停止后，开始保存任务的执行状态
                            Save();

                            e_TaskCompleted(this, new cTaskEventArgs(TaskID, TaskName, cancel));
                        }
                        break;
                    case cGlobalParas.TaskState.Failed:
                        // 触发 任务失败 事件
                        if (e_TaskFailed != null)
                        {
                            //任务失败
                            Save();
                            e_TaskFailed(this, new cTaskEventArgs(TaskID, TaskName, cancel));
                        }
                        break;
                    case cGlobalParas.TaskState.Started:
                        // 触发 任务开始 事件
                        m_TaskManage.EventProxy.AddEvent(delegate()
                        {
                            if (e_TaskStarted != null)
                            {
                                e_TaskStarted(this, new cTaskEventArgs(TaskID, TaskName, cancel));
                            }
                        });
                        break;
                    case cGlobalParas.TaskState.Stopped:
                        m_TaskManage.EventProxy.AddEvent(delegate()
                        {
                            //WriteToFile();
                            // 触发 任务停止 事件
                            if (e_TaskStopped != null)
                            {
                                //当任务停止后，开始保存任务的执行状态
                                Save();

                                e_TaskStopped(this, new cTaskEventArgs(TaskID, TaskName, cancel));
                            }
                        });
                        break;
                    case cGlobalParas.TaskState.Waiting:
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

     

        #region 任务控制 启动 停止 重启 取消

        /// 开始任务
        public void Start()
        {
            
            // 确保位初始化的任务先进行初始化（包括从文件读取的任务信息）
            if (m_State !=cGlobalParas.TaskState.Started && m_TaskManage != null)
            {
                m_TaskData.GatheredUrlCount = 0;
                m_TaskData.GatherDataCount = 0;
                m_TaskData.GatherErrUrlCount = 0;
                m_TaskData.GatherTmpCount = 0;
                m_TaskData.StartTimer = System.DateTime.Now;

                m_TaskData.UrlNaviCount  = 0;
                m_TaskData.GatheredUrlNaviCount = 0;
                m_TaskData.GatheredErrUrlNaviCount = 0;

                m_ThreadsRunning = true;

                //TaskInit();

                StartAll();
            }
        }

        /// 启动所有采集任务线程 如果没有分解任务,则启动的单个线程进行采集
        private void StartAll()
        {
            foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
            {
                
                dtc.TaskSplitData.GatheredUrlCount = 0;
                dtc.TaskSplitData.GatheredUrlNaviCount = 0;
                dtc.TaskSplitData.UrlNaviCount = 0;
                dtc.TaskSplitData.GatheredErrUrlCount = 0;
                dtc.TaskSplitData.GatheredErrUrlNaviCount = 0;

                
                dtc.Start();
            }
            State = cGlobalParas.TaskState.Started;
        }

        /// 任务准备就绪（等待开始）
        public void ReadyToStart()
        {
            if (m_State != cGlobalParas.TaskState.Started && m_State != cGlobalParas.TaskState.Completed)
            {
                State = cGlobalParas.TaskState.Waiting;
            }
        }

        /// 停止任务
        public void Stop()
        {
            m_ThreadsRunning = false;

            foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
            {
                dtc.Stop();
            }

            //开始检测是否所有线程都以完成或退出
            //bool isStop = false;

            //while (!isStop)
            //{
            //    isStop = true;

            //    foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
            //    {
            //        //if (dtc.WorkThread.ThreadState == cGlobalParas.GatherThreadState.Started && dtc.WorkThread.IsAlive )
                    
            //        if (dtc.IsStop ==false )
            //            isStop = false;
            //    }
               
            //    Thread.Sleep(200);
            //}

            //State = cGlobalParas.TaskState.Stopped;


        }

        public void OverTask()
        {
            State = cGlobalParas.TaskState.Completed;
        }

        //停止任务，此停止任务与Stop不同的是，强制停止所有工作线程
        //Stop是属于执行完一个完整工作后停止，而Abort不论是否执行到
        //何种状态，必须强制停止，此种方式会导致数据丢失
        /// 取消任务（移除任务）
        public void Abort()
        {
            m_ThreadsRunning = false;

            foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
            {
                dtc.Abort();
            }
            State = cGlobalParas.TaskState.Aborted;
        }

        ///保存运行的任务，主要是保存当前运行的状态
        ///任务保存需要同时保存taskrun.xml，主要是保存采集数量
        ///注意，如果进行暂存后，任务的链接地址会发生变化，因为在任务新建时，任务链接地址有可能带有一定得参数
        ///但任务一旦开始执行，带有参数的网址就会进行解析，同时是按照解析后的网址进行是否采集的标识，所以，再次
        ///保存后，链接地址会很多
        public void Save()
        {
            if (m_IsSaveTask == false)
                return;

            //首先保存排重库信息
            //进行排重库的保存，如果需要排重的话
            if (this.m_Task.IsUrlNoneRepeat == true)
            {
                try
                {
                    m_Urls.Save(this.m_workPath + "Urls\\" + this.m_TaskData.TaskClass.Replace("/","-") + "-" + this.m_TaskData.TaskName + ".db");
                }
                catch (System.Exception ex)
                {
                    if (e_Log != null)
                        {
                            e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID,this.m_TaskData.TaskName ,cGlobalParas.LogType.Error, "排重库保存失败！" + ex.Message, true));
                        }
                }
                //m_Urls = null;
            }

            //保存临时排重库，临时排重库用于断点续采使用
            try
            {
                m_TempUrls.Save(this.m_workPath + "tasks\\run\\task" + this.TaskID + ".db");
                
            }
            catch (System.Exception ex)
            {
                if (e_Log != null)
                {
                    e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID,this.m_TaskData.TaskName, cGlobalParas.LogType.Error, "数据文件保存失败！" + ex.Message, true));
                }
            }

            //保存临时的数据排重库
            try
            {
                this.m_DataRepeat.Save(this.m_workPath + "tasks\\run\\data" + this.m_TaskData.TaskClass.Replace("/", "-") + "-"+ this.TaskName + ".db");

            }
            catch (System.Exception ex)
            {
                if (e_Log != null)
                {
                    e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID, this.m_TaskData.TaskName, cGlobalParas.LogType.Error, "数据文件保存失败！" + ex.Message, true));
                }
            }

            string FileName = this.m_workPath + "tasks\\run\\task" + this.TaskID + ".rst";
            string runFileindex = this.m_workPath + "tasks\\taskrun.xml";

            #region 开始保存Cookie
            string taskFile=this.m_workPath + "tasks\\" + m_TaskData.TaskClass + "\\" + m_TaskData.TaskName + ".smt";
            if (File.Exists(taskFile) && this.m_Task.Cookie != null)
            {

                oTask t = new oTask(m_workPath);
                t.LoadTask(taskFile);
                t.TaskEntity.Cookie = cookieManage.getCookie().Value;
                t.SaveTask(taskFile);
                t = null;
            }
          
            #endregion

        }

        //private string GetSaveTaskXml()
        //{
        //    StringBuilder tXml = new StringBuilder();
        //    StringBuilder tmpXml = new StringBuilder();

        //    try
        //    {
        //        for (int i = 0; i < m_TaskData.Weblink.Count; i++)
        //        {
        //            tXml.Append ( "<WebLink>");
        //            tXml.Append ("<Url>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].Weblink.ToString()) + "</Url>");
        //            tXml.Append ( "<IsNag>" + m_TaskData.Weblink[i].IsNavigation + "</IsNag>");
        //            tXml.Append ( "<IsMultiPageGather>" + m_TaskData.Weblink[i].IsMultiGather + "</IsMultiPageGather>");
        //            tXml.Append ( "<IsNextPage>" + m_TaskData.Weblink[i].IsNextpage + "</IsNextPage>");
        //            tXml.Append ( "<NextPageRule>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NextPageRule) + "</NextPageRule>");
        //            tXml.Append ( "<NextPageUrl>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NextPageUrl) + "</NextPageUrl>");
        //            tXml.Append ( "<NextMaxPage>" + m_TaskData.Weblink[i].NextMaxPage + "</NextMaxPage>");
        //            tXml.Append ( "<IsGathered>" + (int)m_TaskData.Weblink[i].IsGathered + "</IsGathered>");

        //            tmpXml = tXml;


        //            //保存采集地地址是否需要导航
        //            //插入此网址的导航规则
        //            try
        //            {
        //                if (m_TaskData.Weblink[i].IsNavigation == true)
        //                {
        //                    tXml.Append ( "<NavigationRules>");
        //                    for (int j = 0; j < m_TaskData.Weblink[i].NavigRules.Count; j++)
        //                    {
        //                        tXml.Append ( "<NavigationRule>");
        //                        tXml.Append ( "<Url>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].Url) + "</Url>");
        //                        tXml.Append ( "<Level>" + m_TaskData.Weblink[i].NavigRules[j].Level + "</Level>");
        //                        tXml.Append ( "<IsNext>" + m_TaskData.Weblink[i].NavigRules[j].IsNext + "</IsNext>");
        //                        tXml.Append ( "<NextRule>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].NextRule) + "</NextRule>");
        //                        tXml.Append ( "<NextMaxPage>" + m_TaskData.Weblink[i].NavigRules[j].NextMaxPage + "</NextMaxPage>");
        //                        tXml.Append ( "<NaviStartPos>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].NaviStartPos) + "</NaviStartPos>");
        //                        tXml.Append ( "<NaviEndPos>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].NaviEndPos) + "</NaviEndPos>");
        //                        tXml.Append ( "<NagRule>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].NavigRule) + "</NagRule>");
        //                        tXml.Append ( "<IsNextPage>" + m_TaskData.Weblink[i].NavigRules[j].IsNaviNextPage + "</IsNextPage>");
        //                        tXml.Append ( "<NextPageRule>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].NaviNextPage) + "</NextPageRule>");
        //                        tXml.Append ( "<NaviNextMaxPage>" + m_TaskData.Weblink[i].NavigRules[j].NaviNextMaxPage + "</NaviNextMaxPage>");
        //                        tXml.Append ( "<IsGather>" + m_TaskData.Weblink[i].NavigRules[j].IsGather + "</IsGather>");
        //                        tXml.Append ( "<GatherStartPos>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].GatherStartPos) + "</GatherStartPos>");
        //                        tXml.Append ( "<GatherEndPos>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].GatherEndPos) + "</GatherEndPos>");

        //                        tXml.Append ( "</NavigationRule>");
        //                    }
        //                    tXml.Append ( "</NavigationRules>");
        //                }

        //                //插入此网址的多页采集规则
        //                if (m_TaskData.Weblink[i].IsMultiGather == true)
        //                {
        //                    tXml.Append ( "<MultiPageRules>");
        //                    for (int j = 0; j < m_TaskData.Weblink[i].MultiPageRules.Count; j++)
        //                    {
        //                        tXml.Append ( "<MultiPageRule>");
        //                        tXml.Append ( "<MultiRuleName>" + m_TaskData.Weblink[i].MultiPageRules[j].RuleName + "</MultiRuleName>");
        //                        tXml.Append ( "<MultiRule>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].MultiPageRules[j].Rule) + "</MultiRule>");
        //                        tXml.Append ( "</MultiPageRule>");
        //                    }
        //                    tXml.Append ( "</MultiPageRules>");
        //                }

        //            }
        //            catch (System.Exception ex)
        //            {
        //                tXml = tmpXml;
        //            }

        //            tXml.Append ( "</WebLink>");
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
                
        //        if (e_Log != null )
        //            e_Log(this, new cGatherTaskLogArgs(this.TaskID,TaskName, cGlobalParas.LogType.Error, ex.Message, this.m_TaskData.IsErrorLog));

        //        return "";
        //    }

        //    return tXml.ToString ();
            
        //}


        #endregion

        #region  类方法 内部使用

        //根据指定的任务ID对当前的任务进行分解，如果有导航页，也需要在此进行
        //分解
        //并初始化此任务的关键数据
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TaskName">完整的任务路径及文件名</param>
        /// <param name="IsExportUrl">是否强制输出所采网址</param>
        //private void SplitTask(string TaskName,bool IsExportUrl)
        //{
        //    oTask t = new oTask(this.m_workPath);

        //    try
        //    {
        //        t.LoadTask(TaskName);

        //        //判断此任务是否需要进行排重处理
        //        if (t.TaskEntity.IsUrlNoneRepeat== true)
        //        {
        //            //初始化排重库
        //            m_Urls = new NetMiner.Base.cHashTree();
        //            try
        //            {
        //                m_Urls.Open(this.m_workPath + "urls\\" + this.m_TaskData.TaskClass + "-" + this.m_TaskData.TaskName + ".db");
        //            }
        //            catch (System.Exception ex)
        //            {
        //                if (e_Log != null)
        //                {
        //                    e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID, this.m_TaskData.TaskName, cGlobalParas.LogType.Error, "排重库打开失败！" + ex.Message, true));
        //                }
        //            }
        //        }
                
        //        SplitGatherTask(t);

        //    }
        //    catch (System.Exception)
        //    {
        //        //调试实体文件加载失败，有可能是文件丢失所造成
        //        //但还是需要加载一个空信息，以便界面可以显示此丢失的任务
        //        //这样用户可以通过界面操作删除此任务内容，这是一个针对
        //        //丢失文件的处理手段
        //        //m_TaskData.SavePath = "";
        //        m_TaskData.TaskDemo = "";
        //        //m_TaskData.StartPos = "";
        //        //m_TaskData.EndPos = "";
        //        //m_TaskData.Cookie = null;
        //        //m_TaskData.WebCode = cGlobalParas.WebCode.auto;
        //        m_TaskData.PublishType = cGlobalParas.PublishType.NoPublish;
        //        //m_TaskData.IsUrlEncode = false;
        //        //m_TaskData.IsTwoUrlCode = false;
        //        //m_TaskData.UrlEncode = cGlobalParas.WebCode.NoCoding;
        //        //m_TaskData.Weblink = null;
        //        //m_TaskData.CutFlag = null;

        //        return;

        //    }

        //    if (IsExportUrl == true)
        //        m_TaskData.IsExportGUrl = true;

        //    t = null;

        //}

        /// <summary>
        /// 初始化采集任务，主要用于分解采集任务线程
        /// </summary>
        private void SplitTask()
        {
            try
            {

                //判断此任务是否需要进行排重处理
                if (this.m_Task.IsUrlNoneRepeat == true)
                {
                    //初始化排重库
                    m_Urls = new NetMiner.Base.cHashTree();
                    try
                    {
                        m_Urls.Open(this.m_workPath + "urls\\" + this.m_TaskData.TaskClass + "-" + this.m_TaskData.TaskName + ".db");
                    }
                    catch (System.Exception ex)
                    {
                        if (e_Log != null)
                        {
                            e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID, this.m_TaskData.TaskName, cGlobalParas.LogType.Error, "排重库打开失败！" + ex.Message, true));
                        }
                    }
                }

                SplitGatherTask(this.m_Task);
            }
            catch (System.Exception ex)
            {
                
                m_TaskData.TaskDemo = "";
             
                return;

            }
            
        }

        private void SplitGatherTask(eTask t)
        {
            cGatherTaskSplit dtc;
            List<eWebLink> tWeblink;


            #region 初始化任务信息，加载到taskData中
            m_TaskData.TaskDemo = t.TaskDemo;
            m_TaskData.PublishType = t.ExportType;

            //if (t.ThreadProxy.Count > 0 && t.ThreadProxy[0].pType != cGlobalParas.ProxyType.TaskConfig)
            //    m_isProxySplit = false;

            if (t.ThreadProxy.Count == t.ThreadCount)
                m_isProxySplit = true;                      //表示用户为每个线程都设置了代理

            #endregion

                #region 加载网页地址数据，如果Url带有参数，在此进行分解

                ////加载网页地址数据及采集标志数据
                ////再次去处理如果带有参数的网址,则需要进行分解
                ////确保加载的网址肯定是一个有效的网址
                ////注意,此时由于有可能分解任务信息,所以,网址数量在此会发生变化,所以,最终还需修改网址数据
            List<eWebLink> webLinks = new List<eWebLink>();
            eWebLink w;
            //Task.cUrlAnalyze u = new Task.cUrlAnalyze();
            //在此使用urlAnalyze类，是用于分解字典，不用于分解网址，所以代理信息可以不用输入
            NetMiner.Core.Url.cUrlParse u = new NetMiner.Core.Url.cUrlParse(this.m_workPath);

            for (int i = 0; i < t.WebpageLink.Count; i++)
            {
                if (Regex.IsMatch(t.WebpageLink[i].Weblink.ToString(), "{.*}") ||
                     t.WebpageLink[i].Weblink.ToString().Contains("\r\n"))
                {
                    List<string> Urls;

                    //先分解网址，分解后添加到系统中
                    Urls = u.SplitWebUrl(t.WebpageLink[i].Weblink.ToString());

                    //开始添加m_TaskData.weblink数据
                    for (int j = 0; j < Urls.Count; j++)
                    {
                        w = new eWebLink();
                        w.IsGathered = t.WebpageLink[i].IsGathered;
                        w.IsNavigation = t.WebpageLink[i].IsNavigation;

                        w.IsNextpage = t.WebpageLink[i].IsNextpage;
                        w.NextPageRule = t.WebpageLink[i].NextPageRule;
                        w.NextMaxPage = t.WebpageLink[i].NextMaxPage;

                        w.NextPageUrl = t.WebpageLink[i].NextPageUrl;
                        w.Weblink = Urls[j].ToString();

                        //加载导航数据
                        if (t.WebpageLink[i].IsNavigation == true)
                        {
                            w.NavigRules = t.WebpageLink[i].NavigRules;
                        }

                        w.IsMultiGather = t.WebpageLink[i].IsMultiGather;


                        //此处加载多页采集规则
                        if (t.WebpageLink[i].IsMultiGather == true)
                        {
                            w.MultiPageRules = t.WebpageLink[i].MultiPageRules;
                            w.IsData121 = t.WebpageLink[i].IsData121;
                        }

                        webLinks.Add(w);
                        w = null;
                    }

                }
                else
                {

                    webLinks.Add(t.WebpageLink[i]);

                }

            }

            u = null;

            //将分解后的采集网址重新填充
            t.WebpageLink = webLinks;

            #endregion

            #region 加载采集规则

            //m_TaskData.CutFlag = t.TaskEntity.WebpageCutFlag;

            //判断采集规则中是否有自动编号，并复制
            for (int m = 0; m < this.m_Task.WebpageCutFlag.Count; m++)
            {
                if (this.m_Task.WebpageCutFlag[m].GatherRuleType == cGlobalParas.GatherRuleType.NonePage)
                {
                    for (int n = 0; n < this.m_Task.WebpageCutFlag[m].ExportRules.Count; n++)
                    {
                        if (this.m_Task.WebpageCutFlag[m].ExportRules[n].FieldRuleType == cGlobalParas.ExportLimit.ExportAutoCode)
                        {
                            try
                            {
                                this.AutoID= new string[] { this.m_Task.WebpageCutFlag[m].ExportRules[n].FieldRule };
                            }
                            catch (System.Exception)
                            {
                                this.AutoID = new string[] { "0" };
                            }
                            break;
                        }
                    }
                    break;
                }
            }

            #endregion

            //在此初始化文件下载的路径
            string sPath = t.SavePath + "\\" + m_TaskData.TaskName + "_file";

            if (m_list_GatherTaskSplit.Count > 0)
            {   // 清理可能存在的子线程
                foreach (cGatherTaskSplit gts in m_list_GatherTaskSplit)
                {
                    gts.Stop();
                }
                m_list_GatherTaskSplit.Clear();
            }

            if (IsCompleted)
            {
                // 修改此采集任务的状态为已采集完成,设置为状态为已完成，需要出发事件
                m_State = cGlobalParas.TaskState.Completed;

                //m_State = cGlobalParas.TaskState.Completed;

                //e_TaskCompleted(this, new cTaskEventArgs(m_TaskData.TaskID, false));
            }
            else
            {
                #region 在此处理登录插件的cookie获取
                ///在此处理登录插件cookie的获取
                if (this.m_Task.IsPluginsCookie == true)
                {
                    if (ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Cloud ||
                        ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Enterprise ||
                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Ultimate ||
                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Server ||
                        ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.DistriServer)
                    {
                        NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();

                        if (this.m_Task.PluginsCookie == "")
                        {
                            if (e_Log != null)
                            {
                                e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID, this.m_TaskData.TaskName, cGlobalParas.LogType.Warning, "配置了登录插件操作，却没有提供插件的具体文件！", false));
                            }
                        }
                        else
                        {
                            cookieManage = new cCookieManage(rPlugin.CallGetCookie(
                                this.m_Task.WebpageLink[0].Weblink, m_TaskData.TaskName, this.m_Task.PluginsCookie));
                        }
                        rPlugin = null;
                    }
                    else
                    {
                        if (e_Log != null)
                        {
                            e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID, this.m_TaskData.TaskName, cGlobalParas.LogType.Warning, "当前版本不支持插件功能，请获取正确版本！", false));
                        }
                    }
                }

                #endregion

                if (cookieManage==null)
                    cookieManage = new cCookieManage(t.Cookie);

                #region 根据线程数进行任务分解

                //重新初始化UrlCount
                m_TaskData.UrlCount = t.WebpageLink.Count;

                //开始进行任务分块,但此任务的Url数必须大于线程数,且线程数>1
                if (m_TaskData.UrlCount > m_TaskData.ThreadCount && m_TaskData.ThreadCount > 1)
                {
                    int SplitUrlCount = (int)Math.Ceiling((decimal)m_TaskData.UrlCount / (decimal)m_TaskData.ThreadCount);

                    //设置每个分解任务的起始Url索引和终止的Url索引
                    int StartIndex = 0;
                    int EndIndex = 0;
                    int j = 0;

                    #region 多线程的分解
                    for (int i = 1; i <= m_TaskData.ThreadCount; i++)
                    {
                        StartIndex = EndIndex;
                        if (i == m_TaskData.ThreadCount)
                        {
                            EndIndex = t.WebpageLink.Count;
                        }
                        else
                        {
                            //EndIndex = i * m_TaskData.ThreadCount;
                            EndIndex = i * SplitUrlCount;
                        }

                        //初始化分解采集任务类
                        dtc = new cGatherTaskSplit(this.m_workPath, ref this.m_ProxyControl, this.m_isGlobalRepeat,
                            ref this.m_GlobalUrls, ref this.m_Urls, ref this.m_TempUrls, ref this.m_DataRepeat);

                        dtc.TaskEntity = (eTask)t.DeepClone();
                        dtc.TaskEntity.WebpageLink.Clear();
                        dtc.AutoID = this.AutoID;
                        
                        dtc.TaskManage = m_TaskManage;

                        if (t.Cookie != null)
                        {
                            KeyValuePair<int, string> ck = cookieManage.getCookie();
                            dtc.TaskEntity.Cookie = ck.Value;
                            dtc.cookieIndex = int.Parse(ck.Key.ToString());
                        }
                        else
                        {
                            dtc.cookieIndex = 0;
                            dtc.TaskEntity.Cookie = "";
                        }

                     
                        if (m_isProxySplit == false)
                        {
                            dtc.TaskEntity.IsProxy = t.IsProxy;
                            dtc.TaskEntity.IsProxyFirst = t.IsProxyFirst;
                            dtc.pType = cGlobalParas.ProxyType.TaskConfig;
                        }
                        else
                        {
                            dtc.TaskEntity.IsProxy = true;
                            dtc.TaskEntity.IsProxyFirst = false;
                            dtc.pType = (cGlobalParas.ProxyType)t.ThreadProxy[i - 1].pType;
                            dtc.ProxyAddress = t.ThreadProxy[i - 1].Address;
                            dtc.ProxyPort = t.ThreadProxy[i - 1].Port; 
                        }

                        tWeblink = new List<eWebLink>();

                        for (j = StartIndex; j < EndIndex; j++)
                        {
                            tWeblink.Add(t.WebpageLink[j]);
                        }

                        dtc.TaskEntity.WebpageLink = tWeblink;

                        //初始化分解的子任务数据
                        dtc.SetSplitData(StartIndex, EndIndex - 1, tWeblink, t.WebpageCutFlag);

                        m_TaskData.TaskSplitData.Add(dtc.TaskSplitData);

                        m_list_GatherTaskSplit.Add(dtc);

                        tWeblink = null;
                        dtc = null;

                    }
                    #endregion
                    this.ThreadCount = m_TaskData.ThreadCount;
                }
                else
                {
                    #region 这里进入是因为入口地址小于线程，系统强制单线程处理了。
                    dtc = new cGatherTaskSplit(this.m_workPath, ref this.m_ProxyControl, this.m_isGlobalRepeat, ref m_GlobalUrls, ref this.m_Urls, ref this.m_TempUrls, ref this.m_DataRepeat);
                    dtc.AutoID = this.AutoID;
                    dtc.TaskManage = m_TaskManage;

                    dtc.TaskEntity.TaskID = m_TaskData.TaskID;
                    dtc.TaskEntity = t;
                    if (t.Cookie != null)
                    {
                        KeyValuePair<int, string> ck = cookieManage.getCookie();
                        dtc.TaskEntity.Cookie = ck.Value;
                        dtc.cookieIndex = int.Parse(ck.Key.ToString());
                    }
                    else
                    {
                        dtc.cookieIndex = 0;
                        dtc.TaskEntity.Cookie = "";
                    }


                    if (m_isProxySplit == false)
                    {
                        dtc.TaskEntity.IsProxy = t.IsProxy;
                        dtc.TaskEntity.IsProxyFirst = t.IsProxyFirst;
                        dtc.pType = cGlobalParas.ProxyType.TaskConfig;
                    }
                    else
                    {
                        dtc.TaskEntity.IsProxy = true;
                        dtc.TaskEntity.IsProxyFirst = false;
                        dtc.pType = (cGlobalParas.ProxyType)t.ThreadProxy[0].pType;
                        dtc.ProxyAddress = t.ThreadProxy[0].Address;
                        dtc.ProxyPort = t.ThreadProxy[0].Port;
                    }

                    dtc.SetSplitData(0, m_TaskData.UrlCount - 1, t.WebpageLink, t.WebpageCutFlag);
                    m_TaskData.TaskSplitData.Add(dtc.TaskSplitData);

                    m_list_GatherTaskSplit.Add(dtc);

                    this.ThreadCount = 1;
                }

                #endregion

                dtc = null;

                #endregion
            }

            foreach (cGatherTaskSplit TaskSplit in m_list_GatherTaskSplit)
            {
                //将分解的子线程进行事件绑定
                TaskEventInit(TaskSplit);
            }
        }

        //更新cookie的值，cookie的值会根据用户的输入情况发生变化
        //public void UpdateCookie(string cookie)
        //{
        //    this.TaskData.Cookie = cookie;

        //    foreach (cGatherTaskSplit tp in m_list_GatherTaskSplit)
        //    {
        //        tp.UpdateCookie(cookie);
        //    }

        //}

        //public void UpdateUrl(string Url)
        //{
        //    if (this.TaskData.Weblink.Count > 0)
        //    {
        //        this.TaskData.Weblink[0].Weblink = Url;
        //        foreach (cGatherTaskSplit tp in m_list_GatherTaskSplit)
        //        {
        //            tp.UpdateUrl(Url);
        //        }
        //    }

        //}

        /// 初始化采集任务线程
        //private void TaskInit()
        //{
        //    string sPath = this.m_Task.SavePath + "\\" + m_TaskData.TaskName + "_file";

        //    ///任务初始化分为两种情况，一种是未启动执行的任务，一种是已经启动但未执行完毕的任务
        //    ///
        //    //m_TaskData.GatheredUrlCount = 0;
        //    //m_TaskData.GatherErrUrlCount = 0;
        //    //m_TaskData.TrueUrlCount = m_TaskData.UrlCount;

        //    if (!m_IsDataInitialized)
        //    {
        //        if (m_list_GatherTaskSplit.Count > 0)
        //        {   // 清理可能存在的子线程
        //            foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
        //            {
        //                dtc.Stop();
        //            }
        //            m_list_GatherTaskSplit.Clear();
        //        }

        //        if (IsCompleted)
        //        {   
        //            // 修改此采集任务的状态为已采集完成,设置为状态为已完成，需要出发事件
        //            m_State = cGlobalParas.TaskState.Completed;

        //            //m_State = cGlobalParas.TaskState.Completed;

        //            //e_TaskCompleted(this, new cTaskEventArgs(m_TaskData.TaskID, false));
        //        }
        //        else
        //        {
        //            #region 在此处理登录插件的cookie获取
        //            ///在此处理登录插件cookie的获取
        //            if (this.m_Task.IsPluginsCookie == true)
        //            {
        //                if (ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Union ||
        //                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Enterprise ||
        //                ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Ultimate ||
        //                ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Server ||
        //                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.DistriServer)
        //                {
        //                    NetMiner.Core. Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();

        //                    if (this.m_Task.PluginsCookie == "")
        //                    {
        //                        if (e_Log != null)
        //                        {
        //                            e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID, this.m_TaskData.TaskName, cGlobalParas.LogType.Warning, "配置了登录插件操作，却没有提供插件的具体文件！", false));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        m_TaskData.Cookie = new cCookieManage (rPlugin.CallGetCookie(m_TaskData.Weblink[0].Weblink,m_TaskData.TaskName,  m_TaskData.PluginsCookie));
        //                    }
        //                    rPlugin = null;
        //                }
        //                else
        //                {
        //                    if (e_Log != null)
        //                    {
        //                        e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID, this.m_TaskData.TaskName, cGlobalParas.LogType.Warning, "当前版本不支持插件功能，请获取正确版本！", false));
        //                    }
        //                }
        //            }

        //            #endregion


        //            cGatherTaskSplit dtc;

        //            if (m_TaskData.TaskSplitData.Count  > 0)
        //            {
        //                int threadIndex = 0;
        //                foreach (cTaskSplitData configData in m_TaskData.TaskSplitData)
        //                {
        //                    //dtc = new cGatherTaskSplit();
        //                    dtc = new cGatherTaskSplit(this.m_workPath, ref this.m_ProxyControl,this.m_isGlobalRepeat,ref this.m_GlobalUrls, ref this.m_Urls, ref this.m_TempUrls, ref this.m_DataRepeat);
        //                    dtc.AutoID = this.AutoID;
        //                    dtc.TaskManage = m_TaskManage;
        //                    dtc.TaskID = m_TaskData.TaskID;
        //                    dtc.TaskName = m_TaskData.TaskName;
        //                    dtc.WebCode = this.m_Task.WebCode;
        //                    dtc.IsUrlEncode = this.m_Task.IsUrlEncode;
        //                    dtc.IsTwoUrlCode = this.m_Task.IsTwoUrlCode;
        //                    dtc.UrlEncode = this.m_Task.UrlEncode;
        //                    if (this.m_Task.Cookie != null)
        //                        dtc.Cookie = m_TaskData.Cookie.getCookie();
        //                    else
        //                        dtc.Cookie = new KeyValuePair<int, string>(0, "");
        //                    dtc.StartPos = this.m_Task.StartPos;
        //                    dtc.EndPos = this.m_Task.EndPos;
        //                    dtc.SavePath = sPath;
        //                    dtc.AgainNumber = this.m_Task.GatherAgainNumber;
        //                    dtc.Ignore404 = this.m_Task.IsIgnore404;
        //                    dtc.IsErrorLog = this.m_Task.IsErrorLog;
        //                    dtc.IsExportGUrl = this.m_Task.IsExportGUrl;
        //                    dtc.IsExportGDateTime = this.m_Task.IsExportGDateTime;
        //                    dtc.GIntervalTime = this.m_Task.GIntervalTime;
        //                    dtc.GIntervalTime1 = this.m_Task.GIntervalTime1;
        //                    dtc.IsMultiInterval = this.m_Task.IsMultiInterval;

        //                    dtc.IsCustomHeader = this.m_Task.IsCustomHeader;
        //                    dtc.Headers = this.m_Task.Headers;

        //                    if (m_isProxySplit == true)
        //                    {
        //                        dtc.IsProxy = this.m_Task.IsProxy;
        //                        dtc.IsProxyFirst = this.m_Task.IsProxyFirst;
        //                        dtc.pType = cGlobalParas.ProxyType.TaskConfig;
        //                    }
        //                    else
        //                    {
        //                        dtc.IsProxy = true;
        //                        dtc.IsProxyFirst = true;
        //                        dtc.pType = (cGlobalParas.ProxyType)this.m_Task.ThreadProxy[threadIndex ].pType;
        //                        dtc.ProxyAddress = this.m_Task.ThreadProxy[threadIndex].Address;
        //                        dtc.ProxyPort = this.m_Task.ThreadProxy[threadIndex].Port; 
        //                    }

        //                    //dtc.IsSilent = m_TaskData.IsSilent;

        //                    dtc.IsUrlNoneRepeat = this.m_Task.IsUrlNoneRepeat;
        //                    dtc.IsSucceedUrlRepeat = this.m_Task.IsSucceedUrlRepeat;

        //                    //V5增加
        //                    dtc.IsPluginsCookie = this.m_Task.IsPluginsCookie;
        //                    dtc.PluginsCookie = this.m_Task.PluginsCookie;
        //                    dtc.IsPluginsDeal = this.m_Task.IsPluginsDeal;
        //                    dtc.PluginsDeal = this.m_Task.PluginsDeal;
        //                    dtc.IsPluginsPublish = this.m_Task.IsPluginsPublish;
        //                    dtc.PluginsPublish = this.m_Task.PluginsPublish;

        //                    dtc.IsUrlAutoRedirect = this.m_Task.IsUrlAutoRedirect;

        //                    //V5.1增加
        //                    dtc.IsGatherErrStop = this.m_Task.IsGatherErrStop;
        //                    dtc.GatherErrStopCount = this.m_Task.GatherErrStopCount;
        //                    dtc.GatherErrStopRule = this.m_Task.GatherErrStopRule;
        //                    dtc.IsInsertDataErrStop = this.m_Task.IsInsertDataErrStop;
        //                    dtc.InsertDataErrStopConut = this.m_Task.InsertDataErrStopConut;
        //                    dtc.IsGatherRepeatStop = this.m_Task.IsGatherRepeatStop;
        //                    dtc.GatherRepeatStopRule = this.m_Task.GatherRepeatStopRule;

        //                    //V5.2增加
        //                    dtc.IsIgnoreErr = this.m_Task.IsIgnoreErr;
        //                    dtc.IsAutoUpdateHeader = this.m_Task.IsAutoUpdateHeader;

        //                    dtc.TaskRunType = m_TaskData.RunType;
        //                    dtc.PublishType = m_TaskData.PublishType;
        //                    if (m_TaskData.RunType == cGlobalParas.TaskRunType.OnlySave)
        //                    {
                                
        //                        dtc.DataSource = this.m_Task.DataSource;
        //                        dtc.TableName = this.m_Task.DataTableName;
        //                        dtc.InsertSql = this.m_Task.InsertSql;
        //                        dtc.IsSqlTrue = this.m_Task.IsSqlTrue;
        //                    }

        //                    //V5.31增加
        //                    dtc.isCookieList = this.m_Task.isCookieList;
        //                    dtc.GatherCount = m_TaskData.GatherCount;
        //                    dtc.RejectFlag = this.m_Task.RejectFlag;
        //                    dtc.RejectDeal = this.m_Task.RejectDeal;
        //                    dtc.isGatherCoding = this.m_Task.isGatherCoding;
        //                    dtc.GatherCodingFlag = this.m_Task.GatherCodingFlag;
        //                    dtc.CodeUrl = this.m_Task.CodeUrl;
        //                    dtc.GatherCodingPlugin = this.m_Task.GatherCodingPlugin;

        //                    //V5.5增加
        //                    dtc.IsVisual = this.m_Task.IsVisual;

        //                    dtc.TaskSplitData = configData;

        //                    m_list_GatherTaskSplit.Add(dtc);

        //                    dtc = null;

        //                    threadIndex++;
        //                }

        //            }
        //            else
        //            {
        //                #region 这是单线程的处理方式
        //                dtc = new cGatherTaskSplit(this.m_workPath, ref this.m_ProxyControl, this.m_isGlobalRepeat, ref this.m_GlobalUrls, ref this.m_Urls, ref this.m_TempUrls, ref this.m_DataRepeat);
        //                dtc.AutoID = this.AutoID; 
        //                dtc.TaskManage = m_TaskManage;
        //                dtc.TaskID = m_TaskData.TaskID;
        //                dtc.TaskName = m_TaskData.TaskName;
        //                dtc.WebCode = this.m_Task.WebCode;
        //                dtc.IsUrlEncode = this.m_Task.IsUrlEncode;
        //                dtc.IsTwoUrlCode = this.m_Task.IsTwoUrlCode;
        //                dtc.UrlEncode = this.m_Task.UrlEncode;

        //                if (this.m_Task.Cookie != null)
        //                    dtc.Cookie = m_TaskData.Cookie.getCookie();
        //                else
        //                    dtc.Cookie = new KeyValuePair<int, string>(0, "");
        //                dtc.StartPos = this.m_Task.StartPos;
        //                dtc.EndPos = this.m_Task.EndPos;
        //                dtc.SavePath = sPath;
        //                dtc.AgainNumber = this.m_Task.GatherAgainNumber;
        //                dtc.Ignore404 = this.m_Task.IsIgnore404;
        //                dtc.IsErrorLog = this.m_Task.IsErrorLog;
        //                dtc.IsExportGUrl = this.m_Task.IsExportGUrl;
        //                dtc.IsExportGDateTime = this.m_Task.IsExportGDateTime;
        //                dtc.GIntervalTime = this.m_Task.GIntervalTime;
        //                dtc.GIntervalTime1 = this.m_Task.GIntervalTime1;
        //                dtc.IsMultiInterval = this.m_Task.IsMultiInterval;

        //                dtc.IsCustomHeader = this.m_Task.IsCustomHeader;
        //                dtc.Headers = this.m_Task.Headers;

        //                if (m_isProxySplit == true)
        //                {
        //                    dtc.IsProxy = this.m_Task.IsProxy;
        //                    dtc.IsProxyFirst = this.m_Task.IsProxyFirst;
        //                    dtc.pType = cGlobalParas.ProxyType.TaskConfig;
        //                }
        //                else
        //                {
        //                    dtc.IsProxy = true;
        //                    dtc.IsProxyFirst = false;
        //                    dtc.pType = (cGlobalParas.ProxyType)this.m_Task.ThreadProxy[0].pType;
        //                    dtc.ProxyAddress = this.m_Task.ThreadProxy[0].Address;
        //                    dtc.ProxyPort = this.m_Task.ThreadProxy[0].Port; ;
        //                }



        //                //dtc.IsSilent = this.m_Task.IsSilent;

        //                dtc.IsUrlNoneRepeat = this.m_Task.IsUrlNoneRepeat;
        //                dtc.IsSucceedUrlRepeat = this.m_Task.IsSucceedUrlRepeat;

        //                //V5增加
        //                dtc.IsPluginsCookie = this.m_Task.IsPluginsCookie;
        //                dtc.PluginsCookie = this.m_Task.PluginsCookie;
        //                dtc.IsPluginsDeal = this.m_Task.IsPluginsDeal;
        //                dtc.PluginsDeal = this.m_Task.PluginsDeal;
        //                dtc.IsPluginsPublish = this.m_Task.IsPluginsPublish;
        //                dtc.PluginsPublish = this.m_Task.PluginsPublish;

        //                dtc.IsUrlAutoRedirect = this.m_Task.IsUrlAutoRedirect;

        //                //V5.1增加
        //                dtc.IsGatherErrStop = this.m_Task.IsGatherErrStop;
        //                dtc.GatherErrStopCount = this.m_Task.GatherErrStopCount;
        //                dtc.GatherErrStopRule = this.m_Task.GatherErrStopRule;
        //                dtc.IsInsertDataErrStop = this.m_Task.IsInsertDataErrStop;
        //                dtc.InsertDataErrStopConut = this.m_Task.InsertDataErrStopConut;
        //                dtc.IsGatherRepeatStop = this.m_Task.IsGatherRepeatStop;
        //                dtc.GatherRepeatStopRule = this.m_Task.GatherRepeatStopRule;

        //                //V5.2增加
        //                dtc.IsIgnoreErr = this.m_Task.IsIgnoreErr;
        //                dtc.IsAutoUpdateHeader = this.m_Task.IsAutoUpdateHeader;

        //                dtc.TaskRunType = m_TaskData.RunType;
        //                dtc.PublishType = m_TaskData.PublishType;
        //                if (m_TaskData.RunType == cGlobalParas.TaskRunType.OnlySave)
        //                {

        //                    dtc.DataSource = this.m_Task.DataSource;
        //                    dtc.TableName = this.m_Task.DataTableName;
        //                    dtc.InsertSql = this.m_Task.InsertSql;
        //                    dtc.IsSqlTrue = this.m_Task.IsSqlTrue;
        //                }

        //                //V5.31增加
        //                    dtc.isCookieList = this.m_Task.isCookieList;
        //                dtc.GatherCount = this.m_Task.GatherCount;
        //                dtc.RejectFlag = this.m_Task.RejectFlag;
        //                dtc.RejectDeal = this.m_Task.RejectDeal;
        //                dtc.isGatherCoding = this.m_Task.isGatherCoding;
        //                dtc.GatherCodingFlag = this.m_Task.GatherCodingFlag;
        //                dtc.CodeUrl = this.m_Task.CodeUrl;
        //                dtc.GatherCodingPlugin = this.m_Task.GatherCodingPlugin;

        //                //V5.5增加
        //                dtc.IsVisual = this.m_Task.IsVisual;

        //                // 新任务，则新建子线程
        //                m_list_GatherTaskSplit.Add(dtc);

        //                dtc = null;
        //            }


        //            foreach (cGatherTaskSplit TaskSplit in m_list_GatherTaskSplit)
        //            {   
        //                // 初始化所有子线程
        //                TaskEventInit(TaskSplit);
        //            }
        //        }

        //        m_IsDataInitialized = true;
        //    }
        //}

        //将分解任务事件进行绑定
        private void TaskEventInit(cGatherTaskSplit dtc)
        {
            if (!dtc.IsInitialized)
            {
                // 绑定 初始化事件、完成事件
                dtc.TaskInit += this.TaskWorkThreadInit;
                dtc.Completed += this.TaskWorkThreadCompleted;
                dtc.GUrlCount += this.onGUrlCount;
                dtc.Log += this.onLog;
                dtc.GData += this.onGData;
                dtc.Error += this.TaskThreadError;
                dtc.RequestStopped += this.onRequestStop;
                dtc.UpdateCookie += this.onUpdateCookie;
                dtc.Stopped += this.onStop;
                dtc.IsInitialized = true;
                
            }
        }

        /// 重置任务为未初始化状态
        internal void ResetTaskState()
        {
            e_TaskCompleted = null;
            e_TaskStarted = null;
            e_TaskError = null;
            e_TaskStateChanged = null;
            e_TaskStopped = null;
            e_TaskFailed = null;
            e_TaskAborted = null;
            e_TaskThreadInitialized = null;
            this.State = cGlobalParas.TaskState.UnStart;

            m_IsInitialized = false;

            e_Log = null;
            e_GData = null;
        }

        /// 重置采集任务为未启动状态
        //internal void ResetTaskData()
        //{
        //    // 停止任务
        //    //Stop();

        //    m_TaskData.GatheredUrlCount = 0;
        //    m_TaskData.GatherDataCount = 0;
        //    m_TaskData.GatherErrUrlCount = 0;
        //    m_TaskData.RowsCount = 0;
        //    m_TaskData.GatherTmpCount = 0;
        //    m_TaskData.StartTimer = System.DateTime.Now;

        //    m_TaskData.UrlNaviCount  = 0;
        //    m_TaskData.GatheredUrlNaviCount = 0;
        //    m_TaskData.GatheredErrUrlNaviCount = 0;

        //    //修改taskrun文件中，此文件索引的采集地址和出错地址为0
        //    string runFileindex = this.m_workPath + "tasks\\taskrun.xml";

        //    oTaskRun tr = new oTaskRun(this.m_workPath);
        //    tr.ResetTaskRun(this.TaskID, m_TaskData.UrlCount);
        //    tr.Dispose();
        //    tr = null;

        //    //cXmlIO cxml = new cXmlIO(runFileindex);
        //    //cxml = new cXmlIO(runFileindex);

        //    ////还原数据需要将实际需要采集的网址数量初始化为UrlCount
        //    //cxml.EditTaskrunValue(this.TaskID.ToString(),cGlobalParas.TaskState.UnStart , "0","0","0","0",m_TaskData.UrlCount.ToString () );
        //    //cxml.Save();
        //    //cxml = null;

        //    string tXml = "";

        //    for (int i = 0; i < m_TaskData.Weblink.Count; i++)
        //    {
        //        tXml += "<WebLink>";
        //        tXml += "<Url>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].Weblink.ToString()) + "</Url>";
        //        tXml += "<IsNag>" + m_TaskData.Weblink[i].IsNavigation + "</IsNag>";
        //        tXml += "<IsMultiPageGather>" + m_TaskData.Weblink[i].IsMultiGather + "</IsMultiPageGather>";
        //        tXml += "<IsNextPage>" + m_TaskData.Weblink[i].IsNextpage + "</IsNextPage>";
        //        tXml += "<NextPageRule>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NextPageRule) + "</NextPageRule>";
        //        tXml += "<NextPageUrl>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NextPageUrl) + "</NextPageUrl>";
        //        tXml += "<NextMaxPage>" + m_TaskData.Weblink[i].NextMaxPage + "</NextMaxPage>";
        //        tXml += "<IsGathered>" + (int)m_TaskData.Weblink[i].IsGathered + "</IsGathered>";

        //        string tmpXml = tXml;

        //        //保存采集地地址是否需要导航
        //        //插入此网址的导航规则
        //        try
        //        {
        //            if (m_TaskData.Weblink[i].IsNavigation == true)
        //            {
        //                tXml += "<NavigationRules>";
        //                for (int j = 0; j < m_TaskData.Weblink[i].NavigRules.Count; j++)
        //                {
        //                    tXml += "<NavigationRule>";
        //                    tXml += "<Url>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].Url) + "</Url>";
        //                    tXml += "<Level>" + m_TaskData.Weblink[i].NavigRules[j].Level + "</Level>";
        //                    tXml += "<IsNext>" + m_TaskData.Weblink[i].NavigRules[j].IsNext + "</IsNext>";
        //                    tXml += "<NextRule>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].NextRule) + "</NextRule>";
        //                    tXml += "<NextMaxPage>" + m_TaskData.Weblink[i].NavigRules[j].NextMaxPage + "</NextMaxPage>";
        //                    tXml += "<NaviStartPos>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].NaviStartPos) + "</NaviStartPos>";
        //                    tXml += "<NaviEndPos>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].NaviEndPos) + "</NaviEndPos>";
        //                    tXml += "<NagRule>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].NavigRule) + "</NagRule>";
        //                    tXml += "<IsNextPage>" + m_TaskData.Weblink[i].NavigRules[j].IsNaviNextPage + "</IsNextPage>";
        //                    tXml += "<NextPageRule>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].NaviNextPage) + "</NextPageRule>";
        //                    tXml += "<NaviNextMaxPage>" + m_TaskData.Weblink[i].NavigRules[j].NaviNextMaxPage + "</NaviNextMaxPage>";
        //                    tXml += "<IsGather>" + m_TaskData.Weblink[i].NavigRules[j].IsGather + "</IsGather>";
        //                    tXml += "<GatherStartPos>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].GatherStartPos) + "</GatherStartPos>";
        //                    tXml += "<GatherEndPos>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].NavigRules[j].GatherEndPos) + "</GatherEndPos>";

        //                    tXml += "</NavigationRule>";
        //                }
        //                tXml += "</NavigationRules>";
        //            }

        //            //插入此网址的多页采集规则
        //            if (m_TaskData.Weblink[i].IsMultiGather == true)
        //            {
        //                tXml += "<MultiPageRules>";
        //                for (int j = 0; j < m_TaskData.Weblink[i].MultiPageRules.Count; j++)
        //                {
        //                    tXml += "<MultiPageRule>";
        //                    tXml += "<MultiRuleName>" + m_TaskData.Weblink[i].MultiPageRules[j].RuleName + "</MultiRuleName>";
        //                    tXml += "<MultiRule>" + ToolUtil.ReplaceTrans(m_TaskData.Weblink[i].MultiPageRules[j].Rule) + "</MultiRule>";
        //                    tXml += "</MultiPageRule>";
        //                }
        //                tXml += "</MultiPageRules>";
        //            }

        //        }
        //        catch (System.Exception ex)
        //        {
        //            tXml = tmpXml;
        //        }

        //        tXml += "</WebLink>";

        //        m_TaskData.Weblink[i].IsGathered = (int) cGlobalParas.UrlGatherResult.UnGather ;
 
        //    }

        //    string FileName = this.m_workPath + "tasks\\run\\task" + m_TaskData.TaskID + ".rst";
        //    cXmlIO cxml1=null;
        //    try
        //    {
        //        cxml1 = new cXmlIO(FileName);
        //    }
        //    catch (System.Exception )
        //    {
        //        //捕获错误不做处理，因为有可能运行文件不存在。
        //    }

        //    //有可能运行文件不存在，所以需要判断是否为空
        //    if (cxml1 != null)
        //    {
        //        cxml1.DeleteNode("WebLinks");
        //        cxml1.InsertElement("Task", "WebLinks", tXml);
        //        cxml1.Save();
        //    }
        //    cxml1 = null;

        //    //删除临时存储的采集数据xml文件
        //    string tmpFileName = m_TaskData.SavePath + "\\" + m_TaskData.TaskName + "-" + m_TaskData.TaskID + ".xml";
        //    if (File.Exists(tmpFileName))
        //    {
        //        File.Delete(tmpFileName);
        //    }

        //    oTaskRun t = new oTaskRun(this.m_workPath);
        //    eTaskRun er= t.LoadSingleTask(m_TaskData.TaskID);
        //    m_TaskData.UrlCount = er.UrlCount;
        //    t = null;

        //    //m_TaskData.TaskSplitData.Clear ();
        //    //m_IsDataInitialized = false;
        //}

        /// 清理任务数据
        internal void Remove()
        {
            ResetTaskState();
        }

        //重置任务
        //public void ResetTask()
        //{
        //    ResetTaskData();
        //}

        #endregion

        #region 公有方法

        /// 将所有缓存数据写入文件
        public void WriteToFile()
        {

        }


        #endregion

        #region  响应分解采集任务(子线程)事件

        /// 任务初始化,由分解任务触发,
        private void TaskWorkThreadInit(object sender, TaskInitializedEventArgs e)
        {
            cGatherTaskSplit dtc = (cGatherTaskSplit)sender;
            m_TaskData.TaskID  =e.TaskID ;

            if (e_TaskThreadInitialized != null)
            {
                // 代理触发 任务初始化 事件
                m_TaskManage.EventProxy.AddEvent(delegate()
                {
                    e_TaskThreadInitialized(this, new TaskInitializedEventArgs(m_TaskData.TaskID));
                });
            }
            
        }

        /// 分解采集任务 线程完成 事件处理 判断的是独立线程，每个线程完成后
        /// 都需要触发任务完成事件，交由任务继续判断，如果完成则调用任务完成
        /// 事件，告诉程序此任务已经完成
        private void TaskWorkThreadCompleted(object sender, cTaskEventArgs e)
        {

            cGatherTaskSplit dtc = (cGatherTaskSplit)sender;
            if (dtc.UrlCount  == dtc.GatherErrUrlCount +dtc.GatheredUrlCount 
                && dtc.UrlNaviCount ==dtc.GatheredUrlNaviCount+dtc.GatheredErrUrlNaviCount)
            {  
                // 任务采集完成
                onTaskCompleted();
            }
        }

        /// 当某个线程采集完成后，会调用任务完成事件进行检测，如果任务完成则触发任务
        /// 完成事件。但在此判断时需要注意，如果任务采集失败的数量和采集数量相等，则判断
        /// 任务失败。且每次调用此事件时，都需要做一次检测，对每个子线程都检测一遍，看是否
        /// 存在已经完成，但未触发完成事件的自线程。
        private void onTaskCompleted()
        {
            if (m_TaskData.UrlCount == (m_TaskData.GatheredUrlCount + m_TaskData.GatherErrUrlCount) 
                && m_TaskData.UrlNaviCount ==m_TaskData.GatheredUrlNaviCount+m_TaskData.GatheredErrUrlNaviCount
                && m_State != cGlobalParas.TaskState.Completed)
            {
                if (m_TaskData.UrlCount  == m_TaskData.GatherErrUrlCount)
                {
                    //如果全部采集都发生了错误，则此任务为失败
                    State = cGlobalParas.TaskState.Failed ;
                }
                else
                {
                    // 设置为完成状态，触发任务完成事件
                    State = cGlobalParas.TaskState.Completed;
                }

                //无论失败还是成功都要进行触发器的触发
                if (this.m_Task.IsTrigger == true && this.m_Task.TriggerType == ((int)cGlobalParas.TriggerType.GatheredRun).ToString ())
                {
                    cRunTask rt = new cRunTask(this.m_workPath);
                    rt.RunSoukeyTaskEvent += this.onRunSoukeyTask;
                    rt.RunTaskLogEvent += this.onRunTaskLog;

                    eTaskPlan p;
                    
                    for (int i=0;i< this.m_Task.TriggerTask.Count ;i++)
                    {
                        p=new eTaskPlan ();

                        p.RunTaskType = this.m_Task.TriggerTask[i].RunTaskType ;
                        p.RunTaskName = this.m_Task.TriggerTask[i].RunTaskName ;
                        p.RunTaskPara = this.m_Task.TriggerTask[i].RunTaskPara ;

                        rt.AddTask (p);
                    }

                    rt.RunSoukeyTaskEvent -= this.onRunSoukeyTask;
                    rt.RunTaskLogEvent -=this.onRunTaskLog;
                    rt = null;
                    
                }
            }
        }

        //处理触发器执行网络矿工任务
        private void onRunSoukeyTask(object sender, cRunTaskEventArgs e)
        {
            e_RunTask(this, new cRunTaskEventArgs(e.MessType, e.RunName, e.RunPara));
        }

        private void onRunTaskLog(object sender, cRunTaskLogArgs e)
        {
            if (e_RunTaskLog != null)
            {
                e_RunTaskLog(sender, e);
            }
        }

        /// 处理 分解采集任务 错误事件
        private void TaskThreadError(object sender, TaskThreadErrorEventArgs e)
        {
            //当采集发生错误后，系统首先需要检测当前是否连接网络
            //如果没有连接网络，即无Internet，则系统停止此任务执行
            //if (cTool.IsLinkInternet() == false)
            //{
            //    Stop();

            //    m_State = cGlobalParas.TaskState.Failed;

            //    if (e_TaskFailed != null)
            //    {
            //        e_TaskFailed(this, new cTaskEventArgs(TaskID, TaskName, false));
            //    }

            //    return;

            //}


            cGatherTaskSplit gt = (cGatherTaskSplit)sender;

            if (e_TaskError != null)
            {
                e_TaskError(this, new TaskErrorEventArgs(TaskID ,TaskName, e.Error));
            }
            //}
        }

        //处理日志事件
        public void onLog(object sender, cGatherTaskLogArgs e)
        {
            //在此处理如果是直接入库，则不输出日志
            if (this.RunType == cGlobalParas.TaskRunType.OnlySave)
            {
                if (e_TaskStarted != null && !e.Cancel && e.LogType == cGlobalParas.LogType.Error)
                {
                    e_Log(sender, e);
                }
            }
            else
            {
                if (e_TaskStarted != null && !e.Cancel)
                {
                    e_Log(sender, e);
                }
            }

            //在此处理是否写入错误数据到日志的请求
            if ((e.IsSaveErrorLog == true && e.LogType ==cGlobalParas.LogType.Error ) ||
                (e.IsSaveErrorLog == true && e.LogType == cGlobalParas.LogType.GatherError) ||
                (e.IsSaveErrorLog == true && e.LogType == cGlobalParas.LogType.PublishError ))
            {
                NetMiner.Core.Log.cSystemLog eLog = new NetMiner.Core.Log.cSystemLog(this.m_workPath);
                eLog.WriteLog(this.TaskName, e.LogType , e.strLog);
                eLog = null;
            }

        }

        private readonly object m_AddCountLock = new object();
        //处理数据事件
        public void onGData(object sender, cGatherDataEventArgs e)
        {
            //如果是直接入库，则不返回数据。
            if (this.RunType == cGlobalParas.TaskRunType.OnlySave)
            {
            }
            else
            {
                if (e_TaskStarted != null && !e.Cancel)
                {
                    e_GData(sender, e);
                }
            }

            if (e.gData != null)
            {
                lock (m_AddCountLock)
                {
                    m_TaskData.GatherDataCount += e.gData.Rows.Count;
                    m_TaskData.GatherTmpCount +=e.gData.Rows.Count;
                }
            }

            //判断采集了多少条数据后，是否需要暂停
            if (m_TaskData.GatherCount >0)
            {
                if (m_TaskData.GatherTmpCount > m_TaskData.GatherCount)
                {
                    //开始暂停各个线程的工作
                    foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
                    {
                        dtc.Pause(this.m_Task.GatherCountPauseInterval);
                    }

                    //重置为零
                    m_TaskData.GatherTmpCount = 0;
                }
            }

        }

       //加一个同步锁，进行技术的增加
        private readonly Object m_AddLock = new Object();
        public void onGUrlCount(object sender,cGatherUrlCountArgs e)
        {
            Monitor.Enter(m_AddLock);
          
            try
            {
                switch (e.uType)
                {
                    case cGlobalParas.UpdateUrlCountType.Gathered:
                        m_TaskData.GatheredUrlCount++;
                        //testCountAdd();
                        break;
                    case cGlobalParas.UpdateUrlCountType.NaviGathered:
                        m_TaskData.GatheredUrlNaviCount++;
                        break;

                    case cGlobalParas.UpdateUrlCountType.NaviErr:
                        m_TaskData.GatheredErrUrlNaviCount++;
                        break;

                    case cGlobalParas.UpdateUrlCountType.Err:
                        m_TaskData.GatherErrUrlCount++;
                        break;

                    case cGlobalParas.UpdateUrlCountType.ReIni:
                        m_TaskData.UrlNaviCount += e.UrlCount;
                        break;

                }

                if (e_GUrlCount != null)
                {
                    int urlcount = m_TaskData.UrlCount + m_TaskData.UrlNaviCount;
                    int gurlcount = m_TaskData.GatheredUrlCount + m_TaskData.GatheredUrlNaviCount;
                    int errurlcount = m_TaskData.GatherErrUrlCount + m_TaskData.GatheredErrUrlNaviCount;

                    e_GUrlCount(sender, new cGatherUrlCounterArgs(TaskID,urlcount,gurlcount,errurlcount));
                }

            }
            catch
            {
            }
            finally
            {
                Monitor.Exit(m_AddLock);
            }
        }

        private void onRequestStop(object sender, cTaskEventArgs e)
        {
            Stop();

            if (e_Log!=null)
                e_Log(this,new cGatherTaskLogArgs (this.TaskID , TaskName, cGlobalParas.LogType.Warning,"由于满足了用户设置的停止采集条件，所以任务被强制停止！",false ));

            if (e_ShowLogInfo !=null)
                e_ShowLogInfo(this, new ShowInfoEventArgs("任务停止", "任务停止" + this.TaskName ));
        }

        private void onStop(object sender, cTaskEventArgs e )
        {
            bool isStop = true;

            //在此检测是否所有线程全部停止，如果全部停止，则触发停止事件
            foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
            {
                if (dtc.IsThreadAlive == true)
                    isStop = false;
            }

            if (isStop==true)
                State = cGlobalParas.TaskState.Stopped;

        }

        private void onUpdateCookie(object sender, cUpdateCookieArgs e)
        {
            if (cookieManage != null && cookieManage.CookieList.Length > e.Index)
            {
                cookieManage.CookieList[e.Index] = e.newCookie;
                if (cookieManage.CookieList.Length> 0)
                    ((cGatherTaskSplit)sender).TaskEntity.Cookie = cookieManage.getCookie().Value;
            }
        }
        #endregion

        #region 事件

        /// 采集任务完成事件
        private event EventHandler<cTaskEventArgs> e_TaskCompleted;
        internal event EventHandler<cTaskEventArgs> TaskCompleted
        {
            add { lock (m_eventLock) { e_TaskCompleted += value; } }
            remove { lock (m_eventLock) { e_TaskCompleted -= value; } }
        }

        /// 采集任务采集失败事件
        private event EventHandler<cTaskEventArgs> e_TaskFailed;
        internal event EventHandler<cTaskEventArgs> TaskFailed
        {
            add { lock (m_eventLock) { e_TaskFailed += value; } }
            remove { lock (m_eventLock) { e_TaskFailed -= value; } }
        }

        /// 采集任务开始事件
        private event EventHandler<cTaskEventArgs> e_TaskStarted;
        internal event EventHandler<cTaskEventArgs> TaskStarted
        {
            add { lock (m_eventLock) { e_TaskStarted += value; } }
            remove { lock (m_eventLock) { e_TaskStarted -= value; } }
        }

        /// 采集任务停止事件
        private event EventHandler<cTaskEventArgs> e_TaskStopped;
        internal event EventHandler<cTaskEventArgs> TaskStopped
        {
            add { lock (m_eventLock) { e_TaskStopped += value; } }
            remove { lock (m_eventLock) { e_TaskStopped -= value; } }
        }

        /// 采集任务取消事件
        private event EventHandler<cTaskEventArgs> e_TaskAborted;
        internal event EventHandler<cTaskEventArgs> TaskAborted
        {
            add { lock (m_eventLock) { e_TaskAborted += value; } }
            remove { lock (m_eventLock) { e_TaskAborted -= value; } }
        }

        /// 采集任务错误事件
        private event EventHandler<TaskErrorEventArgs> e_TaskError;
        internal event EventHandler<TaskErrorEventArgs> TaskError
        {
            add { lock (m_eventLock) { e_TaskError += value; } }
            remove { lock (m_eventLock) { e_TaskError -= value; } }
        }

        /// 任务状态变更事件,每当任务状态发生变更时进行处理,
        /// 并触发界面此事件,用于界面状态的改变
        private event EventHandler<TaskStateChangedEventArgs> e_TaskStateChanged;
        internal event EventHandler<TaskStateChangedEventArgs> TaskStateChanged
        {
            add { lock (m_eventLock) { e_TaskStateChanged += value; } }
            remove { lock (m_eventLock) { e_TaskStateChanged -= value; } }
        }


        /// 采集任务分解初始化完成事件
        private event EventHandler<TaskInitializedEventArgs> e_TaskThreadInitialized;
        internal event EventHandler<TaskInitializedEventArgs> TaskThreadInitialized
        {
            add { lock (m_eventLock) { e_TaskThreadInitialized += value; } }
            remove { lock (m_eventLock) { e_TaskThreadInitialized -= value; } }
        }

        /// <summary>
        /// 采集日志事件
        /// </summary>
        private event EventHandler<cGatherTaskLogArgs> e_Log;
        internal event EventHandler<cGatherTaskLogArgs> Log
        {
            add { e_Log += value; }
            remove { e_Log -= value; }
        }

        /// <summary>
        /// 更新网址时间
        /// </summary>
        private event EventHandler<cGatherUrlCounterArgs> e_GUrlCount;
        internal event EventHandler<cGatherUrlCounterArgs> GUrlCount
        {
            add { lock (m_eventLock) { e_GUrlCount += value; } }
            remove { lock (m_eventLock) { e_GUrlCount -= value; } }
        }


        /// <summary>
        /// 触发器执行时的日志事件
        /// </summary>
        private event EventHandler<cRunTaskLogArgs> e_RunTaskLog;
        internal event EventHandler<cRunTaskLogArgs> RunTaskLog
        {
            add { e_RunTaskLog += value; }
            remove { e_RunTaskLog -= value; }
        }

        /// <summary>
        /// 采集数据事件
        /// </summary>
        private event EventHandler<cGatherDataEventArgs> e_GData;
        internal event EventHandler<cGatherDataEventArgs> GData
        {
            add { e_GData += value; }
            remove { e_GData -= value; }
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
        /// 想界面的托盘图片及系统消息返回信息
        /// </summary>
        private event EventHandler<ShowInfoEventArgs> e_ShowLogInfo;
        public event EventHandler<ShowInfoEventArgs> ShowLogInfo
        {
            add { lock (m_eventLock) { e_ShowLogInfo += value; } }
            remove { lock (m_eventLock) { e_ShowLogInfo -= value; } }
        }

        #endregion
    }
}

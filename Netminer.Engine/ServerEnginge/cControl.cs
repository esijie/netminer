using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NetMiner.Gather.Control;
using NetMiner.Resource;
using MinerDistri.Distributed;
using System.Data;
using System.Diagnostics;
using NetMiner.Common.Tool;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;
using NetMiner.Core.Event;
using NetMiner.Core.Entity;
using NetMiner.Engine.ServerEngine;

///这是采集引擎，支持企业版及企业版+，即支持接受客户端发来的采集任务，进行数据采集
///同时也支持接受客户端发来的任务，分解采集任务，分布式采集，两个是并存的。

///2014-7-21重新开始对采集引擎进行优化处理
///优化在于两点：
///1、增加服务以外停止后采集任务的处理（实际已经处理），
///2、增加对最大执行任务数的控制，同时分布式可以将整个任务进行分布式执行（原有方式为未分解任务只能有采集引擎进行采集）；
///3、增加采集引擎分布式的能力，原有都是客户端进行处理，增加采集引擎的分布式能力
///2017-6-4 进行优化处理
///1、将分解采集任务及发布数据的最大控制，单独出来一个处理类来控制


namespace NetMiner.Engine.ServerEngine
{
    public class cControl
    {
        //性能检测
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;

        
        //服务引擎用的定时器
        private System.Threading.Timer m_ServerEngine;

        private System.Threading.Timer m_PublishInfoEngine;

        private bool m_isRunning = false;
        
        private int m_MaxRunCount=0;
        //private cClient m_Client;
        private bool m_isDo = false;
        
        private string m_TaskPath = cTool.getPrjPath() + "tasks\\";

       
        /// <summary>
        /// 最大同时分解采集任务的数量
        /// </summary>
        private int m_MaxSplitTaskCount = 2;

        /// <summary>
        /// 最大同时发布数据的数量（分布式发布数据）
        /// </summary>
        private int m_MaxPublishingCount = 2;

        /// <summary>
        /// 当前正在采集的数量
        /// </summary>
        private long m_SplitTaskCount = 0;
        /// <summary>
        /// 当前正在发布的数量
        /// </summary>
        private long m_PublishingCount = 0;

        public static string g_DbConn;
        public static cGlobalParas.DatabaseType g_DbType;

        //是否上传任务后，自动启动任务运行
        private bool m_isAutoStartTask = true;

        private bool m_isGlobalRepeat = false;

        /// <summary>
        /// 采集引擎运行采集任务的控制器（运行未分解任务的控制器）
        /// </summary>
        private  cGatherControl m_GatherControl;
        //private cPublishControl m_PControl;

        private cGlobalParas.VersionType m_EngineVersion;

        private int m_SplitUrls;
        private int m_SplitLevel;
        private int m_SplitTasks;

        private bool m_IsReport = false;
        private string m_ReportEmail = string.Empty;
        private string m_ReportTimer = string.Empty;

        //private bool m_isRemoteGahter = false;
        //private string m_RemoteServer = string.Empty;
        //private bool m_IsRemoteAuthen = false;
        //private string m_RemoteUser = string.Empty;
        //private string m_RemotePwd = string.Empty;
        //private string m_RemotePath = string.Empty;
        //private int m_RemoteMaxCount=0;

        private const string g_RemoteTaskClass = "远程";
        private const string g_RemoteTaskPath = "tasks\\RemoteTask";

        private string m_RegisterUser = string.Empty;

        private delegate cSplitedTask delegateSplitTask(int tID, string TaskName);

        //定义两个线程数组，用于进行任务分解和任务发布
        //private Thread[] m_SplitTaskThread;
        //private Thread[] m_PublishTaskThread;

        //定义一个控制器，用于管理分解采集任务和发布数据的操作，仅在分布式采集引擎中有效
        private cTaskControl m_TaskControl;

        //定义一个全局的排重库，用于整体项目的排重
        private NetMiner.Base.cHashTree g_Urls;

        ///// <summary>
        ///// 处理本采集引擎的分布式采集操作
        ///// </summary>
        //cSplitControl m_sControl = null;

        private string m_workPath = string.Empty;

        public cControl()
        {
           

        }

        ~cControl()
        {
            if (m_GatherControl!=null) 
            m_GatherControl.Dispose();
        }

        #region 启动和停止引擎
        /// <summary>
        /// 启动采集服务引擎
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            //初始化性能检测
            this.cpuCounter = new PerformanceCounter();
            this.ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            this.cpuCounter.CategoryName = "Processor";
            this.cpuCounter.CounterName = "% Processor Time";
            this.cpuCounter.InstanceName = "_Total"; 

            m_SplitTaskCount = 0;
            m_PublishingCount = 0;


            //先加载配置文件的信息
            //加载配置文件信息 每次服务启动时加载，确保可以修改规则有效
            #region 初始化服务器引擎的基本信息
            this.m_workPath = Application.StartupPath + "\\";
            //WriteSystemLog(m_workPath, EventLogEntryType.Error);

            cXmlSConfig sCon = new cXmlSConfig();

            this.m_MaxRunCount = sCon.MaxRunTask;
            string dbType=sCon.DataType;
            if (dbType.ToLower ()=="sqlserver")
            {
                g_DbType=cGlobalParas.DatabaseType.MSSqlServer;
            }
            else if (dbType.ToLower() == "mysql")
            {
                g_DbType = cGlobalParas.DatabaseType.MySql;
            }
            g_DbConn = NetMiner.Common.ToolUtil.DecodingDBCon(sCon.DataConnection);
            this.m_SplitUrls = sCon.SplitUrls;
            this.m_SplitLevel = sCon.SplitLevel;
            this.m_SplitTasks = sCon.SplitTasks;

            this.m_IsReport = sCon.IsReport;
            this.m_ReportEmail = sCon.ReportEmail;
            this.m_ReportTimer = sCon.ReportSendTime;
            this.m_MaxSplitTaskCount = sCon.MaxSplitUrlThread;
            this.m_MaxPublishingCount = sCon.MaxPublishDataThread;

            this.m_isAutoStartTask = sCon.isAutoStartTask;
            this.m_isGlobalRepeat = sCon.isGlobalRepeat;

            //加载分布式采集的操作
            sCon = null;

            if (this.m_isGlobalRepeat==true)
            {
                g_Urls = new NetMiner.Base. cHashTree();
                try
                {
                    g_Urls.Open(this.m_workPath + "urls\\globalUrls.db");
                }
                catch (System.Exception ex)
                {
                    if (e_Log != null)
                    {
                        cTool.WriteSystemLog("全局网址排重库加载失败！", EventLogEntryType.Error, "SMGatherService");
                    }
                }
            }

            //先进行版本验证
            //启动服务的时候开始验证版本
            try
            {
                cVersion v = new cVersion(this.m_workPath);

                if (v.ReadRegisterInfo(this.m_workPath))
                {
                    this.m_RegisterUser = v.RegisterInfo.User;
                    this.m_EngineVersion = v.SominerVersion;

                    v = null;
                }
                else
                {
                    v = null;
                    cTool.WriteSystemLog("版本未激活，请先激活版本", EventLogEntryType.Error, "SMGatherService");
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                cTool.WriteSystemLog("检查软件激活状态失败，可能您还未激活软件，请激活软件后重新启动服务！", EventLogEntryType.Error, "SMGatherService");
                return false;
            }

            if (this.m_EngineVersion != cGlobalParas.VersionType.Server && this.m_EngineVersion != cGlobalParas.VersionType.DistriServer)
            {
                cTool.WriteSystemLog("版本未激活，请先激活版本", EventLogEntryType.Error, "SMGatherService");
                return false ;
            }

            //采集控制器
            m_GatherControl = new cGatherControl(m_workPath,this.m_isGlobalRepeat,ref this.g_Urls);

            //采集控制器事件绑定,绑定后,页面可以响应采集任务的相关事件
            m_GatherControl.TaskManage.TaskCompleted += tManage_Completed;
            m_GatherControl.TaskManage.TaskStarted += tManage_TaskStart;
            m_GatherControl.TaskManage.TaskFailed += tManage_TaskFailed;
            m_GatherControl.TaskManage.TaskStopped += tManage_TaskStop;
            m_GatherControl.TaskManage.Log += tManage_Log;
            m_GatherControl.TaskManage.GData += tManage_GData;
            m_GatherControl.TaskManage.GUrlCount += tManage_GUrlCount;
            m_GatherControl.Completed += m_Gather_Completed;

            ////发布控制器
            //m_PControl = new cPublishControl(m_workPath);

            ////注册发布事件
            //m_PControl.PublishManage.PublishCompleted += this.Publish_Complete;
            //m_PControl.PublishManage.PublishError += this.Publish_Error;
            //m_PControl.PublishManage.PublishFailed += this.Publish_Failed;
            //m_PControl.PublishManage.PublishStarted += this.Publish_Started;
            //m_PControl.PublishManage.PublishLog += this.Publish_Log;
            //m_PControl.PublishManage.RuntimeInfo += this.Publish_RunTime;
            //m_PControl.PublishManage.PublishStop += this.Publish_Stop;

            #endregion

            //更新控制器中的代理IP
            m_GatherControl.TaskManage.UpdateProxy();

            //m_Client = new cClient();

            #region 处理上次由于停止采集引擎造成的异常任务

            //读取正在运行的任务
            //第一步先将所有running的任务职位started
            string sql = "update SM_TaskList set State=" + (int)cGlobalParas.TaskState.Started + ",TaskState=" + (int)cGlobalParas.TaskState.Stopped + " where State=" + (int)cGlobalParas.TaskState.Running;
            if (g_DbType == cGlobalParas.DatabaseType.MySql)
            {
                NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(g_DbConn ,sql);
            }
            else if (g_DbType==cGlobalParas.DatabaseType.MSSqlServer)
            {
                NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(g_DbConn, sql, null);
            }

            //第二步将所有的started和stoped的任务加载到运行区
            sql = "select * from SM_TaskList where State=" + (int)cGlobalParas.TaskState.Started;
            DataTable d = null;
            if (g_DbType == cGlobalParas.DatabaseType.MySql)
            {
                d=NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(g_DbConn, sql);
            }
            else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                d=NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(g_DbConn, sql, null);
            }

            try
            {
                for (int i = 0; i < d.Rows.Count; i++)
                {
                    cTaskData tData = new cTaskData();
                    tData = new cTaskData();
                    tData.TaskID = Int64.Parse(d.Rows[i]["TaskID"].ToString());
                    tData.TaskName = d.Rows[i]["TaskName"].ToString();
                    tData.TaskClass = "远程";
                    tData.TaskType = (cGlobalParas.TaskType)int.Parse(d.Rows[i]["TaskType"].ToString());
                    tData.RunType = (cGlobalParas.TaskRunType)int.Parse(d.Rows[i]["RunType"].ToString());
                    tData.TempDataFile = d.Rows[i]["tempFileName"].ToString();
                    tData.TaskState = (cGlobalParas.TaskState)int.Parse(d.Rows[i]["TaskState"].ToString()); ;
                    tData.UrlCount = int.Parse(d.Rows[i]["UrlCount"].ToString());
                    tData.UrlNaviCount = int.Parse(d.Rows[i]["UrlNaviCount"].ToString()); ;
                    tData.ThreadCount = int.Parse(d.Rows[i]["ThreadCount"].ToString()); ;
                    tData.GatheredUrlCount = int.Parse(d.Rows[i]["GatheredUrlCount"].ToString()); ;
                    tData.GatherErrUrlCount = int.Parse(d.Rows[i]["GatherErrUrlCount"].ToString()); ;

                    tData.GatherDataCount = int.Parse(d.Rows[i]["RowsCount"].ToString()); ;
                    tData.StartTimer = System.DateTime.Now;
                    //在此增加采集运行中的任务
                    m_GatherControl.AddGatherTask(tData);

                }

            }
            catch (System.Exception ex)
            {
                cTool.WriteSystemLog(ex.Message, EventLogEntryType.Error, "SMGatherService");
            }

            //第三步，判断是否有任务已经停止，如果有，则自动启动
             cRunningIndex rIndex = new cRunningIndex(g_DbType, g_DbConn);
            for (int i = 0; i < rIndex.TaskCount; i++)
            {
                if (rIndex.GetState(i) == cGlobalParas.TaskState.Stopped)
                {
                    StartTask(long.Parse(rIndex.GetTaskID(i)));
                    sql = "update SM_TaskList set State=" + (int)cGlobalParas.TaskState.Running + " where TaskID=" + rIndex.GetTaskID(i);
                    if (g_DbType == cGlobalParas.DatabaseType.MySql)
                    {
                        NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(g_DbConn, sql);
                    }
                    else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
                    {
                        NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(g_DbConn, sql, null);
                    }
                }
            }
            rIndex = null;

            #endregion

            #region 初始化分布式采集引擎的任务控制器

            m_TaskControl = new cTaskControl(this.m_workPath,g_DbType,g_DbConn,this.m_SplitUrls,this.m_SplitLevel, this.m_MaxSplitTaskCount, this.m_MaxPublishingCount);

            #endregion

            //启动自身的采集引擎
            m_ServerEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_ServerEngine_CallBack), null, 30000, 60000);

            m_isRunning = true;
            return true;
        }


        /// <summary>
        /// 停止采集服务引擎
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            if (m_isRunning == false)
                return true ;

            m_isRunning = false;

            //判断当前是否有正在运行的采集任务，如果有，则停止
            //先判断自身运行的采集任务
            cRunningIndex rIndex = new cRunningIndex(g_DbType, g_DbConn);
            for (int i = 0; i < rIndex.TaskCount; i++)
            {
                if (rIndex.GetState(i) == cGlobalParas.TaskState.Running)
                {
                    try
                    {

                        StopTask(long.Parse(rIndex.GetTaskID(i)));
                        string sql = "update SM_TaskList set State=" + (int)cGlobalParas.TaskState.Pause + " where TaskID=" + rIndex.GetTaskID(i);
                        if (g_DbType == cGlobalParas.DatabaseType.MySql)
                        {
                            NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(g_DbConn, sql);
                        }
                        else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
                        {
                            NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(g_DbConn, sql, null);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        cTool.WriteSystemLog("采集任务停止失败，错误信息：" + ex.Message, EventLogEntryType.Error, "SMGatherService");
                    }
                }
            }
            rIndex = null;

            m_TaskControl = null;

            if (this.m_isGlobalRepeat==true)
            {
                
                try
                {
                    g_Urls.Save(this.m_workPath + "Urls\\globalUrls.db");
                }
                catch (System.Exception ex)
                {
                    if (e_Log != null)
                    {
                        cTool.WriteSystemLog("全局网址排重库保存失败！", EventLogEntryType.Error, "SMGatherService");
                    }
                }
            }

            return true;
        }

        #endregion

        //服务引擎定时器
        private void m_ServerEngine_CallBack(object State)
        {
            
            if (m_isRunning && m_isDo==false)
            {
                m_isDo = true;

                if (e_Log != null)
                    e_Log(this, new cGatherTaskLogArgs(1, "", cGlobalParas.LogType.Info, "开始轮询", false));

                #region 处理工作日报


                DateTime sendTime = DateTime.Parse(System.DateTime.Now.Date.ToLongDateString() + " " + this.m_ReportTimer);

                TimeSpan tc = (System.DateTime.Now - sendTime);
                if (tc.TotalSeconds>0 && tc.TotalSeconds < 60 && m_IsReport==true)
                {
                    //发送工作日报
                    SendReport(sendTime.ToString ());
                }
                #endregion

                #region 处理采集任务的逻辑
                try
                {

                    #region 对主任务进行索引判断
                    cIndex tIndex = new cIndex(g_DbType, g_DbConn);
                    //tIndex.IniPending(g_DbType, g_DbConn);

                    if (e_Log != null)
                        e_Log(this, new cGatherTaskLogArgs(1, "", cGlobalParas.LogType.Info, "队列数据：" + tIndex.TaskCount.ToString (), false));

                    for (int i = 0; i < tIndex.TaskCount; i++)
                    {
                        if (tIndex.GetState(i) == cGlobalParas.TaskState.UnStart)
                        {
                            #region 当检测到状态是未启动状态，则需要做处理
                            //未运行状态下先进行分解
                            if (tIndex.GetTaskDistriState(i) == cGlobalParas.SplitTaskState.WithoutSplit)
                            {
                                #region 无需分解的采集任务，则需要启动采集
                                if (this.m_MaxRunCount > GetRunTaskCount())
                                {
                                    if (e_Log != null)
                                        e_Log(this, new cGatherTaskLogArgs(1, "", cGlobalParas.LogType.Info, "启动本地任务：" + tIndex.GetSavePath(i), false));

                                    //在此要进行一个判断，如果正在运行的任务大于指定的最大运行量，则不允许再运行采集任务

                                    try
                                    {
                                        //判断一下是否是第一次启动，判断依据是结束时间，如果有
                                        //结束时间，则不是第一次启动
                                        string eDate = tIndex.GetEndDate(i);
                                        if (string.IsNullOrEmpty(eDate))
                                        {
                                            //第一次启动
                                            if (m_isAutoStartTask==true)
                                                StartTask(tIndex.GetID(i), tIndex.GetSavePath(i));
                                        }
                                        else
                                            StartTask(tIndex.GetID(i), tIndex.GetSavePath(i));
                                    }
                                    catch (System.Exception ex)
                                    {
                                        //启动失败，必须将此任务的信息置为启动失败，否则会陷入死循环
                                        cCommon.SetTaskState(g_DbType, g_DbConn, tIndex.GetID(i), -1, tIndex.GetTaskName(i), cGlobalParas.GatherTaskType.Normal, cGlobalParas.TaskState.Failed, 0);

                                        throw ex;
                                    }
                                }
                                else
                                {
                                    if (this.m_EngineVersion == cGlobalParas.VersionType.DistriServer)
                                    {
                                        //如果本地引擎运行的采集任务大于了指定的数量，
                                        //则其他采集任务可以分布到其他客户端进行运行
                                        //将此任务状态修改为远程待运行状态

                                        //string sql = "update SM_MyTask set State="
                                        //   + (int)cGlobalParas.TaskState.RemoteUnstart + " where TaskName='" + tIndex.GetTaskName(i) + "'";
                                        //SQLHelper.ExecuteNonQuery(g_DbConn, sql);
                                        //tIndex.UpdateState(tIndex.GetID(i),cGlobalParas.TaskState.RemoteUnstart);
                                    }
                                }
                                #endregion
                            }
                            else if (tIndex.GetTaskDistriState(i) == cGlobalParas.SplitTaskState.Splited)
                            {
                                //启动分解的任务

                            }
                            else if (tIndex.GetTaskDistriState(i) == cGlobalParas.SplitTaskState.UnSplit)
                            {
                                #region 分解采集任务
                                if (this.m_EngineVersion == cGlobalParas.VersionType.DistriServer)
                                {
                                    if (e_Log != null)
                                        e_Log(this, new cGatherTaskLogArgs(1, "", cGlobalParas.LogType.Info, "分解采集任务：" + tIndex.GetTaskName(i), false));

                                    //分解此采集任务前，需要先判断一下，此任务是首次分解，还是由用户
                                    //重置任务之后带来的分解操作，如果是用户重置任务，则有可能分解队列中
                                    //还存在此采集任务的分解操作，需要将此进行重置

                                    #region 分解采集任务

                                    //修改采集任务中的自动保存日志为空，只有对分布式采集的任务才会这样操作
                                    //因为采集任务在别的计算机上运行，会产生日志，在本机只做发布，发布的错误
                                    //会直接入库。如果选择了自动保存日志，则会产生日志文件，会造成与分布式计算机
                                    //运行时的日志文件冲突。
                                    oTask t = new oTask(this.m_workPath);
                                    t.LoadTask(this.m_workPath + "tasks\\" + tIndex.GetTaskName(i) + ".smt");
                                    t.TaskEntity.IsErrorLog = false;
                                    bool isSameSubTask = t.TaskEntity.isSameSubTask;
                                    t.SaveTask(this.m_workPath + "tasks\\" + tIndex.GetTaskName(i) + ".smt");

                                    t = null;

                                    //修改是否允许进行但客户端请求多个子任务
                                    tIndex.UpdateIsSameSubTask(i, isSameSubTask);

                                    //先修改状态为正在分解
                                    tIndex.UpdateSplietState(i, 0, cGlobalParas.SplitTaskState.Spliting);


                                    //delegateSplitTask sd = new delegateSplitTask(this.SplitTaskUrl);
                                    //AsyncCallback callback = new AsyncCallback(CallbackSplitTask);
                                    //sd.BeginInvoke(tIndex.GetID(i),tIndex.GetTaskName(i),callback, sd);

                                    m_TaskControl.CreateSplitTask(tIndex.GetID(i), tIndex.GetTaskName(i));

                                    #endregion

                                    
                                }
                                else if (this.m_EngineVersion == cGlobalParas.VersionType.Server)
                                {
                                    //如果是企业版，则直接修改状态
                                    tIndex.UpdateSplietState(i, 0, cGlobalParas.SplitTaskState.WithoutSplit);
                                }

                                #endregion

                            }

                            #endregion
                        }
                        else if (tIndex.GetState(i) == cGlobalParas.TaskState.WaitingPublish 
                            && tIndex.GetTaskDistriState(i)==cGlobalParas.SplitTaskState.WithoutSplit)
                        {
                            //处理独立执行的任务开始进行发布操作
                            //分布式，但无需分解的采集任务
                            try
                            {
                                if (e_Log != null)
                                    e_Log(this, new cGatherTaskLogArgs(1, "", cGlobalParas.LogType.Info, "合并独立分布式采集任务数据：" + tIndex.GetTaskName(i), false));

                                bool isS= m_TaskControl.CreatePublishData(tIndex.GetID(i), tIndex.GetDbFile(i), tIndex.GetSavePath(i), tIndex.GetTaskName(i));
                                //MergerData(tIndex.GetID(i), tIndex.GetDbFile(i), tIndex.GetSavePath(i), tIndex.GetTaskName(i));

                                if (isS)
                                    cCommon.SetTaskState(g_DbType, g_DbConn, tIndex.GetID(i), 0, tIndex.GetTaskName(i), cGlobalParas.GatherTaskType.Normal, cGlobalParas.TaskState.Publishing, 0);

                            }
                            catch (System.Exception ex)
                            {
                                throw new Exception("合并数据错误" + ex.Message);
                            }
                        }
                        else if (tIndex.GetState(i)==cGlobalParas.TaskState.Completed)
                        {
                            //在此需要判断是否存在正在分解的操作，如果有，则停止
                            //只停止分解的操作，发布的操作不去处理
                            m_TaskControl.Clear(tIndex.GetID(i));

                        }
                    }
                    tIndex = null;
                    #endregion

                    #region 对分解后的任务进行判断
                     cSplitIndex sIndex = new cSplitIndex(g_DbType, g_DbConn);
                    for (int i = 0; i < sIndex.TaskCount; i++)
                    {
                        switch (sIndex.GetState(i))
                        {
                            case cGlobalParas.TaskState.UnStart :
                                //启动分解任务
                                //StartTask(sIndex.GetSavePath(i) + "\\" + sIndex.GetTaskName(i) + ".smt");

                                break ;
                            case cGlobalParas.TaskState.WaitingPublish:
                                //发布任务
                               
                                string dFile = sIndex.GetDbFile(i);
                                if (System.IO.File.Exists(dFile))
                                {

                                        //MergerSplitData(sIndex.GetTID(i), sIndex.GetTaskID(i), sIndex.GetTaskName(i), sIndex.GetDbFile(i), sIndex.GetSavePath(i));

                                    bool isS= m_TaskControl.CreatePublishData(sIndex.GetTID(i), sIndex.GetTaskID(i), sIndex.GetTaskName(i), sIndex.GetDbFile(i), sIndex.GetSavePath(i));

                                    if (isS)
                                    {
                                        cCommon.SetTaskState(g_DbType, g_DbConn, sIndex.GetTID(i), sIndex.GetTaskID(i), sIndex.GetTaskName(i), cGlobalParas.GatherTaskType.DistriGather, cGlobalParas.TaskState.Publishing, 0);

                                        if (e_Log != null)
                                            e_Log(this, new cGatherTaskLogArgs(1, "", cGlobalParas.LogType.Info, "合并分布式采集任务数据：" + sIndex.GetTaskName(i), false));
                                    }
                                    else
                                    {
                                        //if (e_Log != null)
                                        //    e_Log(this, new cGatherTaskLogArgs(1, "", cGlobalParas.LogType.Info, sIndex.GetTaskName(i) + "：超出最大发布任务队列，等待中......" , false));

                                    }
                                }
                                else
                                {
                                    cCommon.SetTaskState(g_DbType, g_DbConn, sIndex.GetTID(i), sIndex.GetTaskID(i), sIndex.GetTaskName(i), cGlobalParas.GatherTaskType.DistriGather, cGlobalParas.TaskState.PublishFailed, 0);

                                    //MergerSplitData(sIndex.GetTID(i), sIndex.GetTaskID(i), sIndex.GetTaskName(i), sIndex.GetDbFile(i), sIndex.GetSavePath(i));

                                    if (e_Log != null)
                                        e_Log(this, new cGatherTaskLogArgs(1, "", cGlobalParas.LogType.Info, sIndex.GetTaskName(i) + "的数据文件不存在，可能未采集到数据，请检查任务，必要情况下请进行补采工作！", false));
                                    //throw new NetMiner.Common.NetMinerException(sIndex.GetTaskName(i) + "的数据文件不存在，此任务执行失败！");
                                }
                                
                                break;
                        }
                       
                    }
                    sIndex = null;
                    #endregion

                    #region 在此处理用户请求停止采集任务和继续运行采集任务的处理
                    cRunningIndex rIndex = new cRunningIndex(g_DbType, g_DbConn);
                    for (int i = 0; i < rIndex.TaskCount; i++)
                    {
                        if (rIndex.GetState(i) == cGlobalParas.TaskState.Stopped)
                        {
                            StopTask(long.Parse(rIndex.GetTaskID(i)));
                        }
                        else if (rIndex.GetState(i) == cGlobalParas.TaskState.Started)
                        {
                            StartTask(long.Parse(rIndex.GetTaskID(i)));
                        }
                    }
                    rIndex = null;
                    #endregion

                    #region 根据定时计划来更新采集任务的状态
                    UpdatePlan();
                    #endregion
                }
                catch(System.Exception ex)
                {
                    cTool.WriteSystemLog(ex.Message, EventLogEntryType.Error, "SMGatherService");

                    if (e_Log != null)
                        e_Log(this, new cGatherTaskLogArgs(1, "", cGlobalParas.LogType.Error, "出错了，错误信息：" + ex.Message , false));

                }
                #endregion

                #region 写入性能数据，每整点写入
                //判断当前是否为整点
                if (System.DateTime.Now.Minute == 0)
                {
                    try
                    {
                        WritePer();
                    }
                    catch (System.Exception ex)
                    {
                        cTool.WriteSystemLog("服务器性能监测失败：" + ex.Message, EventLogEntryType.Error, "SMGatherService");
                    }
                }
                #endregion

                m_isDo = false;
            }
        }


        #region 当前服务进程性能检测
        private void WritePer()
        {
            string curTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //先判断当前整点是否已经写入
            string sql = "select count(*) from sm_performance where pTimer='" + curTime + "'";
            int count = 0;
            object c = null;

            if (g_DbType == cGlobalParas.DatabaseType.MySql)
            {
                c= NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(g_DbConn, sql);
            }
            else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                c=NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(g_DbConn, sql, null);
            }

            count = int.Parse(c.ToString());
            if (count == 0)
            {
                //插入性能数据
                //先获取性能数据指标

                float FreeMemory = ramCounter.NextValue();
                float CPUUsage = (int)cpuCounter.NextValue();

                sql = "insert into sm_performance (pTimer,CPU,Memory) values ('" + curTime + "'," + CPUUsage + "," + FreeMemory + ")";

                if (g_DbType == cGlobalParas.DatabaseType.MySql)
                {
                    c = NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(g_DbConn, sql);
                }
                else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    c = NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(g_DbConn, sql, null);
                }
            }
        }

        #endregion


        #region 用于分布式分解网址，采用多线程的处理方式，加快分解性能
        //private cSplitedTask SplitTaskUrl(int tID,string TaskName)
        //{
        //    Interlocked.Increment(ref m_SplitTaskCount);

        //    cSplitPlot tManage = new cSplitPlot(this.m_workPath, g_DbType, g_DbConn, this.m_SplitUrls, this.m_SplitLevel, this.m_TaskPath);
        //    cGlobalParas.SplitTaskState sState = cGlobalParas.SplitTaskState.UnSplit;
        //    List<cSplitTaskEntity> ts = null;

        //    cIndex tIndex = new cIndex();
        //    cSplitedTask sTask = new cSplitedTask();
        //    try
        //    {
        //        ts = tManage.SplitTask(TaskName, out sState);
               
        //        sTask.tID = tID;
        //        sTask.sState = sState;
        //        sTask.Tasks = ts;
        //    }
        //    catch(System.Exception ex)
        //    {
        //        sTask.tID = tID;
        //        sTask.sState = cGlobalParas.SplitTaskState.SplitFaild; ;
        //        sTask.Tasks = null;

        //        cTool.WriteSystemLog("分解采集任务失败：" + ex.Message, EventLogEntryType.Error, "SMGatherService");

        //        //分解出错，也需要递减
        //        Interlocked.Decrement(ref m_SplitTaskCount);
        //    }
        //    return sTask;
        // }

        //private void CallbackSplitTask(IAsyncResult ar)
        //{
        //    delegateSplitTask sd = (delegateSplitTask)ar.AsyncState;
        //    cSplitedTask sTask = sd.EndInvoke(ar);

        //    cIndex tIndex = new cIndex();

        //    if (sTask.sState == cGlobalParas.SplitTaskState.SplitFaild)
        //    {
        //        tIndex.UpdateSplietStateByConn(g_DbType, g_DbConn, sTask.tID, 0, cGlobalParas.SplitTaskState.SplitFaild);
        //    }
        //    else if (sTask.Tasks != null && sTask.sState == cGlobalParas.SplitTaskState.Splited)
        //    {

        //        //插入分解任务
        //        tIndex.InsertSplitTaskByConn(g_DbType,g_DbConn, sTask.tID, sTask.Tasks);

        //        int sCount = 0;
        //        if (sTask.Tasks != null)
        //            sCount = sTask.Tasks.Count;
        //        tIndex.UpdateSplietStateByConn(g_DbType,g_DbConn, sTask.tID, sCount, sTask.sState);
        //    }
        //    else
        //    {
        //        //无需分解，需将状态处理，否则无法继续采集
        //        tIndex.UpdateSplietStateByConn(g_DbType,g_DbConn, sTask.tID, 0, sTask.sState);
        //    }

        //    //分解结束后，需要递减
        //    Interlocked.Decrement(ref m_SplitTaskCount);

        //    tIndex=null;
        //}
        #endregion

        #region 定时计划的管理
        private void UpdatePlan()
        {
            string sql = "SELECT SM_TaskPlan.TID, SM_TaskPlan.PlanXML, SM_MyTask.State, SM_MyTask.EndDate FROM  SM_TaskPlan INNER JOIN"
                    + " SM_MyTask ON SM_TaskPlan.TID = SM_MyTask.ID where SM_MyTask.State=" + (int)cGlobalParas.TaskState.Completed ;

            DataTable d = null;
            if (g_DbType == cGlobalParas.DatabaseType.MySql)
            {
                d= NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(g_DbConn, sql);
            }
            else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                d=NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(g_DbConn, sql, null);
            }
            
            if (d == null || d.Rows.Count == 0)
                return;

            //时间间隔如果等于0，则表示无需多次运行。
            for (int i = 0; i < d.Rows.Count; i++)
            {
                string planXML = d.Rows[i]["planXML"].ToString();

                if (planXML.Trim ()!="" && planXML.Trim()!="0" )
                {
                    //开始计算是否已经过了最后一次运行的时间
                    DateTime eDate = DateTime.Parse(d.Rows[i]["EndDate"].ToString());
                    DateTime nDate = GetNextTime(planXML,eDate);

                    if ( nDate>eDate && nDate<=System.DateTime.Now.AddMinutes(1) )
                    {
                        //重置状态
                        string tID= d.Rows[i]["TID"].ToString();
                        sql="select State,DistriState from SM_MyTask where ID=" + tID;
                        DataTable d1 = null;
                        if (g_DbType == cGlobalParas.DatabaseType.MySql)
                        {
                            d1=NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(g_DbConn, sql);
                        }
                        else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
                        {
                            d1=NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(g_DbConn, sql, null);
                        }

                        if (int.Parse (d1.Rows[0]["State"].ToString ())==(int)cGlobalParas.TaskState.Completed )
                        {
                            
                            //删除子任务
                            sql = "select * from SM_SplitTask where TID=" + tID;
                            DataTable dt = null;
                            if (g_DbType == cGlobalParas.DatabaseType.MySql)
                            {
                                dt= NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(g_DbConn, sql);
                            }
                            else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
                            {
                                dt=NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(g_DbConn, sql, null);
                            }
                            for (int j = 0; j < dt.Rows.Count; j++)
                            {
                                string sTask = dt.Rows[j]["sPath"] + "\\" + dt.Rows[j]["TaskName"] + ".smt";
                                    File.Delete(sTask);
                            }

                            //删除数据库中的信息
                            sql = "delete from SM_SplitTask where TID=" + tID;
                            if (g_DbType == cGlobalParas.DatabaseType.MySql)
                            {
                                NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(g_DbConn, sql);
                            }
                            else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
                            {
                                NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(g_DbConn, sql, null);
                            }


                            //只有采集运行完毕才能重置
                            sql = "update SM_MyTask set StartDate='" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', State=" + (int)cGlobalParas.TaskState.UnStart + ",DistriState=" + (int)cGlobalParas.SplitTaskState.UnSplit + " where id=" + tID;
                            if (g_DbType == cGlobalParas.DatabaseType.MySql)
                            {
                                NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(g_DbConn, sql);
                            }
                            else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
                            {
                                NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(g_DbConn, sql, null);
                            }
                            
                        }
                        d1.Dispose();
                        
                    }
                }
            }
            
        }

        private DateTime GetNextTime(string planXML,DateTime eDate)
        {
            //运行类型：0-每天；1-工作日；2-每周；3-每月
            int runType = 0;
            bool isRange = false;
            string sTimer = string.Empty;
            string eTimer = string.Empty;
            bool isOnce = false;
            string OnceDay = string.Empty;
            string OnceTimer = string.Empty;
            bool isCircle = false;
            double Interval = 0;

            #region 加载定时计划信息
            Match charSetMatch = Regex.Match(planXML, "(?<=<RunCycle>).*?(?=</RunCycle>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string strRule = charSetMatch.Groups[0].ToString();

            if (strRule == "day")
            {
                runType = 0;
            }   
            else if (strRule == "workday")
            {
                runType = 1;
            }
            else if (strRule == "weekly")
            {
                runType = 2;
            }
            else if (strRule == "month")
            {
                runType = 3;
            }


            charSetMatch = Regex.Match(planXML, "(?<=<IsRange>).*?(?=</IsRange>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            strRule = charSetMatch.Groups[0].ToString();

            if (strRule == "True")
            {
                isRange = true;
                charSetMatch = Regex.Match(planXML, "(?<=<StartTime>).*?(?=</StartTime>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strRule = charSetMatch.Groups[0].ToString();
                sTimer = strRule.Substring(0, strRule.IndexOf(":"));
                sTimer += ":";
                sTimer += strRule.Substring(strRule.IndexOf(":") + 1, strRule.Length - strRule.IndexOf(":") - 1);
                sTimer += ":00";

                charSetMatch = Regex.Match(planXML, "(?<=<EndTime>).*?(?=</EndTime>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strRule = charSetMatch.Groups[0].ToString();
                eTimer = strRule.Substring(0, strRule.IndexOf(":"));
                eTimer += ":";
                eTimer += decimal.Parse(strRule.Substring(strRule.IndexOf(":") + 1, strRule.Length - strRule.IndexOf(":") - 1));
                eTimer += ":00";
            }
            else
            {
                isRange = false;
              
            }

            charSetMatch = Regex.Match(planXML, "(?<=<IsOnce>).*?(?=</IsOnce>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            strRule = charSetMatch.Groups[0].ToString();

            if (strRule == "True")
            {
                isOnce = true;
                charSetMatch = Regex.Match(planXML, "(?<=<RunTime>).*?(?=</RunTime>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strRule = charSetMatch.Groups[0].ToString();
                OnceDay = strRule.Substring(0, strRule.IndexOf(" "));
                OnceTimer = strRule.Substring(strRule.IndexOf(" ") + 1, strRule.IndexOf(":") - strRule.IndexOf(" ") - 1);
                OnceTimer += ":";
                OnceTimer += strRule.Substring(strRule.IndexOf(":") + 1, strRule.Length - strRule.IndexOf(":") - 1);
                OnceTimer += ":00";
            }
            else
            {
                isOnce = false;
            }

            charSetMatch = Regex.Match(planXML, "(?<=<Circle>).*?(?=</Circle>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            strRule = charSetMatch.Groups[0].ToString();

            if (strRule == "True")
            {
                isCircle = true;
                charSetMatch = Regex.Match(planXML, "(?<=<Interval>).*?(?=</Interval>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strRule = charSetMatch.Groups[0].ToString();
                Interval = double.Parse ( strRule);
            }
            else
            {
                isCircle = false;

            }
            #endregion

            DateTime nDate = System.DateTime.Now;

            //开始计算下一次运行的时间
            if (isOnce == true)
            {
                //先计算当天的时间，如果小于最后运行时间，再加1
                nDate = DateTime.Parse(System.DateTime.Now.ToLongDateString() + " " + OnceTimer);
                if (runType == 0)
                {
                    if (nDate < eDate)
                        nDate = nDate.AddDays(1);
                }
                else if (runType == 1)
                {
                    nDate = DateTime.Parse(System.DateTime.Now.ToLongDateString() + " " + OnceTimer);

                    switch (OnceDay)
                    {
                        case "星期一":
                            while (nDate.DayOfWeek != DayOfWeek.Monday)
                            {
                                nDate = nDate.AddDays(1);
                            }
                            break;
                        case "星期二":
                            while (nDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                nDate = nDate.AddDays(1);
                            }
                            break;
                        case "星期三":
                            while (nDate.DayOfWeek != DayOfWeek.Wednesday)
                            {
                                nDate = nDate.AddDays(1);
                            }
                            break;
                        case "星期四":
                            while (nDate.DayOfWeek != DayOfWeek.Tuesday)
                            {
                                nDate = nDate.AddDays(1);
                            }
                            break;
                        case "星期五":
                            while (nDate.DayOfWeek != DayOfWeek.Friday)
                            {
                                nDate = nDate.AddDays(1);
                            }
                            break;
                        case "星期六":
                            while (nDate.DayOfWeek != DayOfWeek.Saturday)
                            {
                                nDate = nDate.AddDays(1);
                            }
                            break;
                        case "星期日":
                            while (nDate.DayOfWeek != DayOfWeek.Sunday)
                            {
                                nDate = nDate.AddDays(1);
                            }
                            break;
                    }

                    //工作日
                    if (nDate < eDate)
                        nDate = nDate.AddDays(1);
                }
                else if (runType == 2)
                {
                    //每周
                    nDate = DateTime.Parse(System.DateTime.Now.ToLongDateString() + " " + OnceTimer);

                    switch (OnceDay)
                    {
                        case "星期一":
                            while (nDate.DayOfWeek != DayOfWeek.Monday)
                            {
                                nDate = nDate.AddDays(1);
                            }
                            break;
                        case "星期二":
                            while (nDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                nDate = nDate.AddDays(1);
                            }
                            break;
                        case "星期三":
                            while (nDate.DayOfWeek != DayOfWeek.Wednesday)
                            {
                                nDate = nDate.AddDays(1);
                            }
                            break;
                        case "星期四":
                            while (nDate.DayOfWeek != DayOfWeek.Tuesday)
                            {
                                nDate = nDate.AddDays(1);
                            }
                            break;
                        case "星期五":
                            while (nDate.DayOfWeek != DayOfWeek.Friday)
                            {
                                nDate = nDate.AddDays(1);
                            }
                            break;
                        case "星期六":
                            while (nDate.DayOfWeek != DayOfWeek.Saturday)
                            {
                                nDate = nDate.AddDays(1);
                            }
                            break;
                        case "星期日":
                            while (nDate.DayOfWeek != DayOfWeek.Sunday)
                            {
                                nDate = nDate.AddDays(1);
                            }
                            break;
                    }

                    //工作日
                    if (nDate < eDate)
                        nDate = nDate.AddDays(1);
                }
                else if (runType == 3)
                {
                    nDate = DateTime.Parse(System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" 
                            + OnceDay  + " " + OnceTimer);

                    if (System.DateTime.Now.Day < int.Parse(OnceDay))
                        nDate.AddMonths(1);

                }

                if (isRange == true)
                {
                    sTimer = System.DateTime.Now.ToLongDateString() + " " + sTimer;
                    eTimer = System.DateTime.Now.ToLongDateString() + " " + eTimer;
                    if (nDate > DateTime.Parse(sTimer) && nDate < DateTime.Parse(eTimer))
                    {
                    }
                    else
                        nDate = DateTime.Parse("1900-1-1");
                }
            }
            else if (isCircle == true)
            {
                nDate = eDate.AddMinutes(Interval);

                if (isRange == true)
                {
                    sTimer = System.DateTime.Now.ToLongDateString() + " " + sTimer;
                    eTimer = System.DateTime.Now.ToLongDateString() + " " + eTimer;
                    if (nDate > DateTime.Parse(sTimer) && nDate < DateTime.Parse(eTimer))
                    {
                    }
                    else
                        nDate = DateTime.Parse("1900-1-1");
                }
            }

            return nDate;
        }

        #endregion

        /// <summary>
        /// 获取当前正在运行任务的数量
        /// </summary>
        /// <returns></returns>
        private int GetRunTaskCount()
        {
            return m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count
                + m_GatherControl.TaskManage.TaskListControl.WaitingTaskList.Count;
        }

        #region 控制数据合并的操作，任务分解后，由客户端进行数据采集，采集结束后，进行数据合并
        /// <summary>
        /// 专门用于合并分解后的任务数据
        /// </summary>
        /// <param name="tID">分解后任务的ID</param>
        /// <param name="tName"></param>
        /// <param name="dFile"></param>
        /// <param name="tPath"></param>
        //private void MergerSplitData(int tID, long TaskID, string tName,string dFile,string tPath)
        //{
            

        //    int count = 0;
        //    string strTaskID = string.Empty;
        //    //Match charSetMatch = Regex.Match(dFile, "\\d{18}", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //    strTaskID = TaskID.ToString();

        //    if (dFile != "" && System.IO.File.Exists (dFile ))
        //    {
        //        //从采集任务的数据文件中，分离出来taskid

        //        cTaskManage tm = new cTaskManage(this.m_workPath,g_DbType, g_DbConn, this.m_SplitUrls, this.m_SplitLevel, this.m_TaskPath);
        //        cPublishTask p = tm.GetSplitPublishRule(tName);

        //        if (p == null)
        //        {
        //            //表示分解的任务出现了错误，可能是由于用户删除了任务导致
        //            return;
        //        }
        //        if (p.PublishType == cGlobalParas.PublishType.NoPublish)
        //        {
        //            //表示未配置发布数据的规则，不发布数据
        //            p = null;
        //            return;
        //        }
        //        DataTable d = new DataTable();

        //        try
        //        {
        //            d.ReadXml(dFile);
        //        }
        //        catch (System.Exception ex)
        //        {
        //            //读取文件失败
        //            cCommon.SetTaskState(g_DbType, g_DbConn, tID, long.Parse(strTaskID), tName, cGlobalParas.GatherTaskType.DistriGather, cGlobalParas.TaskState.PublishFailed, 0);
        //            throw ex;
        //        }

        //        count = d.Rows.Count;

        //        if (count > 0)
        //        {
        //            Interlocked.Increment(ref m_PublishingCount);

        //            if (d.Columns[d.Columns.Count - 1].ColumnName != "isPublished")
        //            {
        //                d.Columns.Add("isPublished");

        //                for (int i = 0; i < d.Rows.Count; i++)
        //                {
        //                    d.Rows[i]["isPublished"] = cGlobalParas.PublishResult.UnPublished.ToString();
        //                }

        //            }

        //            if (strTaskID != "")
        //            {
        //                cPublish pt = new cPublish(this.m_workPath, m_PControl.PublishManage, p, true, d, long.Parse(strTaskID));
        //                m_PControl.startPublish(pt);
        //            }
        //        }
               
        //    }
        //    else
        //    {
        //        ////如果没有数据，则直接完成
        //        //SetTaskState(tID, long.Parse(strTaskID), tName, cGlobalParas.GatherTaskType.DistriGather, cGlobalParas.TaskState.Completed, 0);
        //    }

        //    //更新分解后任务的采集数量
        //    string sql = "update SM_SplitTask set GatherCount=" + count + " where TaskID='" + strTaskID + "'";
        //    if (g_DbType == cGlobalParas.DatabaseType.MySql)
        //    {
        //        NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(g_DbConn, sql);
        //    }
        //    else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
        //    {
        //        NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(g_DbConn, sql, null);
        //    }

        //    //开始处理日志的问题
        //    string logFile = cTool.getPrjPath() + "log\\task" + tName + ".csv";
        //    if (File.Exists(logFile))
        //    {
        //        cLog log = new cLog();
        //        log.WriteLog(logFile, tName, g_DbConn);
        //        log = null;

        //        //删除日志文件
        //        File.Delete(logFile);
        //    }

        //    if (count == 0)
        //    {
        //        cCommon.SetTaskState(g_DbType, g_DbConn, tID, long.Parse(strTaskID), tName, cGlobalParas.GatherTaskType.DistriGather, cGlobalParas.TaskState.Completed, 0);
        //    }

        //    //删除数据文件，因为此时数据文件已经加载到了内存
        //    //File.Delete(dFile);

        //    cCommon. MergerImage(this.m_workPath, tPath + "\\" + tName + ".smt", tName);

        //}

       /// <summary>
       /// 处理独立执行任务的数据合并操作
       /// </summary>
       /// <param name="tID"></param>
       /// <param name="tName"></param>
       /// <param name="dFile">数据文件路径</param>
       /// <param name="TaskName">完成的采集任务路径</param>
        

        #endregion

        #region 发布任务的事件处理
        //private void Publish_Complete(object sender, PublishCompletedEventArgs e)
        //{

        //    int tID= GetSplitTID(e.TaskID);
        //    cCommon.SetTaskState(g_DbType, g_DbConn, tID, e.TaskID, e.TaskName, cGlobalParas.GatherTaskType.DistriGather, cGlobalParas.TaskState.Completed, 0);
            
        //    //发布完成后删除临时文件
        //    //File.Delete(e.TmpFileName);
        //    Interlocked.Decrement(ref m_PublishingCount);
        //}

        //private void Publish_Stop(object sender, PublishStopEventArgs e)
        //{
        //    int tID = GetSplitTID(e.TaskID);

        //    string dbFile=GetDBFile(e.TaskID ,e.TaskName );
        //    if (dbFile != "")
        //        //保存发布的数据到文件
        //        e.d.WriteXml(dbFile);

        //    //如果停止，则表示数据未发布
        //    cCommon.SetTaskState(g_DbType, g_DbConn, tID, e.TaskID, e.TaskName, cGlobalParas.GatherTaskType.DistriGather, cGlobalParas.TaskState.WaitingPublish, 0);
        //}

        //private void Publish_Started(object sender, PublishStartedEventArgs e)
        //{

        //}


        //private void Publish_Failed(object sender, PublishFailedEventArgs e)
        //{

        //}

        //private void Publish_Error(object sender, PublishErrorEventArgs e)
        //{
        //    //在此处理发布错误的问题
        //    cCommon.InsertErrLog(g_DbType,g_DbConn, e.TaskName, e.TaskID,e.Error.Message);

        //}

        //private void Publish_Log(object sender, PublishLogEventArgs e)
        //{
        //}

        //private void Publish_RunTime(object sender, RunTimeEventArgs e)
        //{
        //}

        #endregion

        #region 控制采集任务的操作
        /// <summary>
        /// 启动采集任务
        /// </summary>
        /// <param name="tName">完整的采集任务路径+名称</param>
        /// <returns></returns>
        private bool StartTask(int tID,string TaskName)
        {
            string tName = System.IO.Path.GetFileNameWithoutExtension(TaskName);

            oTask t1 = new oTask(this.m_workPath);
            t1.LoadTask(TaskName);
          
            t1.TaskEntity.IsExportGDateTime = true;
            t1.TaskEntity.TaskClass = cGlobalParas.TaskClass.Local;

            t1.SaveTask(TaskName);
            t1 = null;

            Int64 TaskID = Int64.Parse(DateTime.Now.ToFileTime().ToString());

            if (!System.IO.Directory.Exists(cTool.getPrjPath() + "tasks\\run"))
            {
                System.IO.Directory.CreateDirectory(cTool.getPrjPath() + "tasks\\run");
            }
            System.IO.File.Copy(TaskName,
                cTool.getPrjPath() + "tasks\\run\\task" + TaskID + ".rst", true);

            cGatherTask t = null;

            t = m_GatherControl.TaskManage.FindTask(TaskID);
            if (t == null)
            {
                t = AddRunTask(tID,TaskID, tName, TaskName);
            }

            m_GatherControl.Start(t);

            return true;
        }


        private void StartTask(Int64 TaskID)
        {
            cGatherTask t = null;

            t = m_GatherControl.TaskManage.FindTask(TaskID);
            if (t == null)
            {
                //表示任务启动失败

                cCommon. InsertErrLog(g_DbType,g_DbConn, "", TaskID, "未获取到此任务信息，可能任务文件丢失，请检查！");
                return;
            }

            try
            {
                //启动此任务
                m_GatherControl.Start(t);
            }
            catch (System.Exception ex)
            {
                cCommon.InsertErrLog(g_DbType, g_DbConn, t.TaskName, TaskID, ex.Message);
                return;
            }

            t = null;
        }


        private bool StopTask(Int64 TaskID)
        {
            cGatherTask t = null;

            //执行正在执行的任务
            t = m_GatherControl.TaskManage.FindTask(TaskID);

            if (t == null)
            {
                cCommon. InsertErrLog(g_DbType, g_DbConn, "", TaskID, "未查找到此任务的运行实例，可能任务实例文件被删除，请检查！");
                return false;
            }

            //停止此任务
            m_GatherControl.Stop(t);
            return true;
        }

        /// 服务器版本的任务运行，不再插入到TaskRun.xml文件中
        /// 而是直接插入到数据库中SM_TaskList
        private cGatherTask AddRunTask(int tID,Int64 TaskID, string tName, string tPath)
        {

            //将选择的任务添加到运行区
            //首先判断此任务是否已经添加到运行区,
            //如果已经添加到运行区则需要询问是否再起一个运行实例

            oTask t = new oTask(this.m_workPath);
            cTaskData tData = new cTaskData();

            string tFileName = tPath;
            //tPath = System.IO.Path.GetDirectoryName(tFileName) + "\\";

            //获取最大的执行ID
            try
            {
                t.LoadTask(tFileName);

                tData = new cTaskData();
                tData.TaskID = TaskID;
                tData.TaskName = tName;
                tData.TaskClass = "远程";
                tData.TaskType = t.TaskEntity.TaskType;
                tData.RunType = t.TaskEntity.RunType;
                tData.TempDataFile = t.TaskEntity.TempDataFile;
                tData.TaskState = cGlobalParas.TaskState.UnStart;
                tData.UrlCount = t.TaskEntity.UrlCount;
                tData.UrlNaviCount = 0;
                tData.ThreadCount = t.TaskEntity.ThreadCount;
                tData.GatheredUrlCount = 0;
                tData.GatherErrUrlCount = 0;
                tData.GatherDataCount = 0;

                tData.GatherDataCount = 0;
                tData.GatherTmpCount = 0;
                tData.StartTimer = System.DateTime.Now;

                //将数据插入到表
                string sql = string.Empty;
                if (g_DbType == cGlobalParas.DatabaseType.MySql)
                {
                    sql = "insert into SM_TaskList (TID,TaskID,TaskName,State,StartDate,TaskType,RunType,TaskState,"
                    + "tempFileName,UrlCount,UrlNaviCount,ThreadCount,GatheredUrlCount,GatherErrUrlCount,RowsCount) values ("
                    + tID + "," + TaskID + ",'" + tName + "'," + (int)cGlobalParas.TaskState.Started + ",'" + System.DateTime.Now.ToString ()
                    + "'," + t.TaskEntity.TaskType + "," + t.TaskEntity.RunType + "," + (int)cGlobalParas.TaskState.UnStart + ",''," + t.TaskEntity.UrlCount
                    + ",0," + t.TaskEntity.ThreadCount + ",0,0,0)";
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(g_DbConn, sql);
                }
                else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    sql = "insert into SM_TaskList (TID,TaskID,TaskName,[State],StartDate,TaskType,RunType,TaskState,"
                    + "tempFileName,UrlCount,UrlNaviCount,ThreadCount,GatheredUrlCount,GatherErrUrlCount,RowsCount) values ("
                    + tID + "," + TaskID + ",'" + tName + "'," + (int)cGlobalParas.TaskState.Started + ",'" + System.DateTime.Now.ToString ()
                    + "'," + t.TaskEntity.TaskType + "," + t.TaskEntity.RunType + "," + (int)cGlobalParas.TaskState.UnStart + ",''," + t.TaskEntity.UrlCount
                    + ",0," + t.TaskEntity.ThreadCount + ",0,0,0)";
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(g_DbConn, sql, null);
                }
                //添加任务到运行区
                m_GatherControl.AddGatherTask(tData);

                tData = null;

                //任务添加到运行区后,需要再添加到任务执行列表中
                t = null;
            }
            catch (System.Exception ex)
            {
                return null;
            }

            return m_GatherControl.TaskManage.FindTask(TaskID);

        }

        #endregion

        #region  采集事件的响应
        private void tManage_Completed(object sender, cTaskEventArgs e)
        {
            try
            {
                int tID = GetID(e.TaskID);
                cGatherTask t = m_GatherControl.TaskManage.FindTask(e.TaskID);

                cCommon.SetTaskState(g_DbType, g_DbConn, tID, e.TaskID, e.TaskName, cGlobalParas.GatherTaskType.Normal, cGlobalParas.TaskState.Completed, t.TaskData.GatherDataCount);

                //删除tasklist
                string sql = "delete from SM_TaskList where TaskID=" + e.TaskID;
                if (g_DbType == cGlobalParas.DatabaseType.MySql)
                {
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(g_DbConn, sql);
                }
                else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(g_DbConn, sql, null);
                }
            }
            catch (System.Exception ex)
            {
                cTool.WriteSystemLog("任务完成响应失败，任务编号：" + e.TaskID + "，错误信息：" + ex.Message, EventLogEntryType.Error, "SMGatherService");
            }
        }

        //单个Url采集发生错误，不进行界面响应，记录日志即可，日志由其他事件记录完成
        //当错误达到一定的数量后，会由后台线程触发任务失败的事件，由任务失败事件完成
        //临时数据的存储
        private void tManage_TaskError(object sender, TaskErrorEventArgs e)
        {

        }

        private void tManage_TaskFailed(object sender, cTaskEventArgs e)
        {
            try
            {
                //根据运行ID获取tID
                string sql = "select TID from SM_TaskList where TaskID=" + e.TaskID;
                object c = null;
                if (g_DbType == cGlobalParas.DatabaseType.MySql)
                {
                    c= NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(g_DbConn, sql);
                }
                else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    c=NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(g_DbConn, sql, null);
                }
                if (c != null)
                {
                    int tID = int.Parse(c.ToString());

                    cGatherTask t = m_GatherControl.TaskManage.FindTask(e.TaskID);

                    cCommon.SetTaskState(g_DbType,g_DbConn, tID, e.TaskID, e.TaskName, cGlobalParas.GatherTaskType.Normal, cGlobalParas.TaskState.Completed, t.TaskData.GatherDataCount);

                    //删除tasklist
                    sql = "delete from SM_TaskList where TaskID=" + e.TaskID;
                    if (g_DbType == cGlobalParas.DatabaseType.MySql)
                    {
                        NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(g_DbConn, sql);
                    }
                    else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
                    {
                        NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(g_DbConn, sql, null);
                    }

                    
                }
            }
            catch (System.Exception ex)
            {
                cTool.WriteSystemLog("任务失败响应失败，错误信息：" + ex.Message, EventLogEntryType.Error, "SMGatherService");
            }
        }

        private void tManage_TaskAbort(object sender, cTaskEventArgs e)
        {
           
        }

        //在这里执行的任务，只能是本地执行的采集任务，不可能是分布式任务
        private void tManage_TaskStart(object sender, cTaskEventArgs e)
        {
            try
            {
                //将数据插入tasklist数据表中
                string sql = string.Empty;
               
                //先根据运行任务队列获取任务的编号
                sql = "select TID from SM_TaskList where TaskID=" + e.TaskID ;
                object c = null;
                if (g_DbType == cGlobalParas.DatabaseType.MySql)
                {
                    c=NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(g_DbConn, sql);
                }
                else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    c=NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(g_DbConn, sql, null);
                }
                if (c != null)
                {
                    int tID = int.Parse(c.ToString());

                    //插入日志，这样任务在运行过程中的任务信息都可以看到
                    //InsertLog(tID, e.TaskID, 0, System.DateTime.Now.ToString(), 0);
                    cCommon.SetTaskState(g_DbType, g_DbConn, tID, e.TaskID, e.TaskName, cGlobalParas.GatherTaskType.Normal, cGlobalParas.TaskState.Running, 0);

                    //此处应该是update
                    sql = "update SM_TaskList set State=" + (int)cGlobalParas.TaskState.Running
                        + ",TaskState=" + (int)cGlobalParas.TaskState.Running + " where TaskID='" + e.TaskID + "'";
                    if (g_DbType == cGlobalParas.DatabaseType.MySql)
                    {
                        NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(g_DbConn, sql);
                    }
                    else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
                    {
                        NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(g_DbConn, sql, null);
                    }
                }
                
            }
            catch (System.Exception ex)
            {
                cTool.WriteSystemLog("任务启动响应失败，错误信息：" + ex.Message, EventLogEntryType.Error, "SMGatherService");
            }

        }

        private void tManage_TaskInitialized(object sender, TaskInitializedEventArgs e)
        {

        }

        private void tManage_TaskStateChanged(object sender, TaskStateChangedEventArgs e)
        {
           
        }

        private void tManage_TaskStop(object sender, cTaskEventArgs e)
        {
            try
            {
                int tID = GetID(e.TaskID);
                cGatherTask t = m_GatherControl.TaskManage.FindTask(e.TaskID);

                cCommon.SetTaskState(g_DbType, g_DbConn, tID, e.TaskID, e.TaskName, cGlobalParas.GatherTaskType.Normal, cGlobalParas.TaskState.Stopped, t.TaskData.GatherDataCount);
            }
            catch (System.Exception ex)
            {
                cTool.WriteSystemLog("任务停止响应失败，错误信息：" + ex.Message, EventLogEntryType.Error, "SMGatherService");
            }
        }

        private void m_Gather_Completed(object sender, EventArgs e)
        {


        }

        private void tManage_GData(object sender, cGatherDataEventArgs e)
        {

        }

        private void tManage_GUrlCount(object sender,cGatherUrlCounterArgs e)
        {

        }


        private void tManage_Log(object sender, cGatherTaskLogArgs e)
        {
            try
            {
                if (e.LogType == cGlobalParas.LogType.Error)
                {
                    //插入日志
                    cCommon. InsertErrLog(g_DbType, g_DbConn, e.TaskName, e.TaskID, e.strLog);
                }
            }
            catch (System.Exception ex)
            {
                cTool.WriteSystemLog("插入日志失败，错误信息：" + ex.Message, EventLogEntryType.Error, "SMGatherService");
            }
        }

        #endregion

       

        /// <summary>
        /// 判断分解后采集任务运行完成状态
        /// </summary>
        /// <param name="tID">MyTask中的ID，SpliteTask中的TID</param>
        /// <param name="tName"></param>
        

        #region 写日志 写日志原则：任务采集发布包括错误等日志写入数据库，系统运行错误写入windows日志
        /// <summary>
        /// 插入日志操作，仅用于独立运行的采集任务日志维护，只维护日志不做其他操作
        /// </summary>
        /// <param name="TaskName">任务名称</param>
        /// <param name="count">采集的数量</param>
        /// <param name="eDate">操作时间</param>
        /// <param name="type">0-开始执行；1-停止执行；2-执行结束</param>
        

        /// <summary>
        /// 专门用于分解采集任务的执行完毕的更新(注意：仅用于结束时日志更新)
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="eDate"></param>
        //private void InsertLog(Int64 TaskID, string eDate)
        //{

        //    try
        //    {
        //        int tID = GetSplitTID(TaskID);

        //        string sql = "select count(GatherCount) from SM_SplitTask where TID=" + tID;

        //        object c = null;
        //        if (g_DbType == cGlobalParas.DatabaseType.MySql)
        //        {
        //            c=NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(g_DbConn, sql);
        //        }
        //        else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
        //        {
        //            c=NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(g_DbConn, sql, null);
        //        }

        //        int count = int.Parse(c.ToString());

        
        //    }
        //    catch (System.Exception ex)
        //    {
        //        //如果插入日志错误，必须马上处理
        //        cTool.WriteSystemLog(ex.Message, EventLogEntryType.Error, "SMGatherService");
        //    }
        //}

        #endregion

        /// <summary>
        /// 根据运行任务ID，获取任务ID，仅限于自身引擎独立运行的任务，且TaskList中必须有数据
        /// </summary>
        /// <param name="TaskID"></param>
        private int GetID(long TaskID)
        {
            string sql = "select TID from SM_TaskList where TaskID=" + TaskID;
            object c = null;
            if (g_DbType == cGlobalParas.DatabaseType.MySql)
            {
                c = NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(g_DbConn, sql);
            }
            else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                c = NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(g_DbConn, sql, null);
            }
            if (c != null)
                return int.Parse(c.ToString());
            else
                return 0;
        }

        /// <summary>
        /// 获取分布式采集任务的ID，且SM_SplitTask中必须有数据
        /// </summary>
        /// <param name="TaskID"></param>
        /// <returns></returns>
        private int GetSplitTID(long TaskID)
        {
            string sql = "select TID from SM_SplitTask where TaskID=" + TaskID;
            object c = null;
            if (g_DbType == cGlobalParas.DatabaseType.MySql)
            {
                c = NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(g_DbConn, sql);
            }
            else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                c = NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(g_DbConn, sql, null);
            }
            if (c != null)
                return int.Parse(c.ToString());
            else
                return 0;
        }

        private string GetDBFile(long TaskID, string TaskName)
        {
            string sql = string.Empty;
            if (TaskName.IndexOf("___") > 0)
            {
                sql = "select TaskDataFile from SM_SplitTask where TaskID=" + TaskID;
            }
            else
            {
                sql = "select TaskDataFile from SM_MyTask where TaskID=" + TaskID;
            }

            object c = null;
            if (g_DbType == cGlobalParas.DatabaseType.MySql)
            {
                c = NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(g_DbConn, sql);
            }
            else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                c = NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(g_DbConn, sql, null);
            }
            if (c != null && c.ToString() != "")
                return c.ToString();
            else
                return "";
        }

        private void SendReport(string eDate)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(DateTime.Parse(eDate).ToLongDateString() + "：网络矿工采集引擎工作报告" + "\r\n");

            string sDate = DateTime.Parse(eDate).AddDays(-1).ToString();

            string sql = "select * from SM_TaskLog where StartDate>'" + sDate + "' and EndDate<'" + eDate + "'";
            DataTable d = null;

            if (g_DbType == cGlobalParas.DatabaseType.MySql)
            {
                d = NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(g_DbConn, sql);
            }
            else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                d = NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(g_DbConn, sql, null);
            }

            sb.Append("今天已经运行完成了" + d.Rows.Count + "个采集任务，明细如下：" + "<br/>");
            if (d.Rows.Count > 0)
            {
                for (int i = 0; i < d.Rows.Count; i++)
                {
                    sb.Append(d.Rows[i]["TaskName"].ToString() + " 共采集记录数：" + d.Rows[i]["GatherCount"].ToString() + "  启动时间：" + d.Rows[i]["StartDate"].ToString() + "——结束时间：" + d.Rows[i]["EndDate"].ToString() + "<br/>");
                }
            }

            d.Dispose();
            sql = "select * from SM_RunningLog where LogDate>'" + sDate + "' and Logdate<'" + eDate + "'";
            d.Dispose();
            if (g_DbType == cGlobalParas.DatabaseType.MySql)
            {
                d = NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(g_DbConn, sql);
            }
            else if (g_DbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                d = NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(g_DbConn, sql, null);
            }
            sb.Append("共发生了" + d.Rows.Count + "个错误，请登录网络矿工进行排错处理。" + "<br/>");

            //开始发送电子邮件
            cXmlSConfig cs = new cXmlSConfig();

            bool isemail = cs.IsEmail;

            if (isemail == false)
            {
                cs = null;
                return;
            }
            NetMiner.Common.Tools.cEmail e = new NetMiner.Common.Tools.cEmail();
            e.Title = "网络矿工工作日报【" + System.DateTime.Now.ToLongDateString() + "】——来自网路矿工" + "";
            e.ReceiveEmail = this.m_ReportEmail;
            e.Email = cs.Email;
            e.PopServer = cs.EmailPopServer;
            e.Port = cs.EmailPopPort.ToString();
            e.User = cs.EmailUser;
            e.Pwd = cs.EmailPwd;

            e.IsPopVerfy = cs.IsPopVerfy;


            e.Content = sb.ToString ();

            try
            {
                e.SendMail();
            }
            catch (System.Exception ex)
            {
                cTool.WriteSystemLog(ex.Message, EventLogEntryType.Error, "SMGatherService");
            }

            cs = null;
            e = null;



        }

        /// <summary>
        /// 采集日志事件，用于向定义了服务器的容器发送消息，以监控服务器运营状况
        /// </summary>
        private event EventHandler<cGatherTaskLogArgs> e_Log;
        public event EventHandler<cGatherTaskLogArgs> Log
        {
            add { e_Log += value; }
            remove { e_Log -= value; }
        }

        #region 服务引擎接口所用 
        public void StartTask(string tName)
        {
            cIndex tIndex = new cIndex(g_DbType, g_DbConn);
            for (int i = 0; i < tIndex.TaskCount; i++)
            {
                if (tIndex.GetTaskName(i) == tName)
                {
                    StartTask(tIndex.GetID(i), tIndex.GetSavePath(i));
                    break;
                }
            }
        }

        public void StopTask(string tName)
        {
            cRunningIndex rIndex = new cRunningIndex(g_DbType, g_DbConn);
            for (int i = 0; i < rIndex.TaskCount; i++)
            {
                if (rIndex.GetState(i) == cGlobalParas.TaskState.Running && rIndex.GetTaskName(i)==tName)
                {
                    StopTask(long.Parse(rIndex.GetTaskID(i)));
                }
            }
        }

        public void SetTaskExtPara(string taskName, string Paras)
        {
            List<string> tmpUrl = new List<string>();
            DataTable d = null;

            try
            { 
                d = JsonConvert.DeserializeObject<DataTable>(Paras);
            }
            catch (System.Exception e)
            {
                cTool.WriteSystemLog(e.Message, EventLogEntryType.Error, "SMGatherServer");
                return;
            }

            int tID = 0;
            int urlCount = 0;
            NetMiner.Core.Url.cUrlParse uAnaly = new NetMiner.Core.Url.cUrlParse(this.m_workPath);

            cIndex tIndex = new cIndex(g_DbType, g_DbConn);
            for (int i = 0; i < tIndex.TaskCount; i++)
            {
                if (tIndex.GetTaskName(i) == taskName)
                {
                    tID = tIndex.GetID(i);

                    List<string> urls = new List<string>();

                    oTask t = new oTask(m_workPath);
                    t.LoadTask(tIndex.GetSavePath(i));

                    List<eWebLink> webLinks = new List<eWebLink>();
                    
                    for (int j=0;j<t.TaskEntity.WebpageLink.Count;j++)
                    {
                        string url = t.TaskEntity.WebpageLink[j].Weblink;
                        tmpUrl = new List<string>();

                        tmpUrl.Add(url);

                    L:
                        if (tmpUrl[0].IndexOf("<EPara>") > 0)
                        {
                            urls = new List<string>();
                            for (int index = 0; index < tmpUrl.Count; index++)
                            {
                                Regex re = new Regex("<EPara>.+?</EPara>", RegexOptions.None);
                                MatchCollection mc = re.Matches(url);
                                foreach (Match ma in mc)
                                {
                                    string s = ma.Value.ToString().Replace("<EPara>", "").Replace("</EPara>", "");

                                    string[] paras = ReplaceExtPara(s, d);

                                    for (int m = 0; m < paras.Length; m++)
                                    {
                                        urls.Add(url.Replace("<EPara>" + s + "</EPara>", paras[m]));
                                    }
                                }
                            }

                            tmpUrl = new List<string>();
                            tmpUrl.AddRange(urls);
                            goto L;
                        }

                        for (int n = 0; n < tmpUrl.Count; n++)
                        {
                            eWebLink link = new eWebLink();
                            link.Weblink = tmpUrl[n];
                            link.id = t.TaskEntity.WebpageLink[j].id;
                            link.IsNavigation = t.TaskEntity.WebpageLink[j].IsNavigation;
                            link.NavigRules = t.TaskEntity.WebpageLink[j].NavigRules;
                            link.IsNextpage = t.TaskEntity.WebpageLink[j].IsNextpage;
                            link.NextPageRule = t.TaskEntity.WebpageLink[j].NextPageRule;
                            link.NextMaxPage = t.TaskEntity.WebpageLink[j].NextMaxPage;
                            link.NextPageUrl = t.TaskEntity.WebpageLink[j].NextPageUrl;
                            link.IsGathered = t.TaskEntity.WebpageLink[j].IsGathered;
                            link.CurrentRunning = t.TaskEntity.WebpageLink[j].CurrentRunning;
                            link.IsMultiGather = t.TaskEntity.WebpageLink[j].IsMultiGather;
                            link.IsData121 = t.TaskEntity.WebpageLink[j].IsData121;
                            link.MultiPageRules = t.TaskEntity.WebpageLink[j].MultiPageRules;

                            webLinks.Add(link);

                            urlCount += uAnaly.GetUrlCount(tmpUrl[n]);
                        }

                    }
                    t.TaskEntity.UrlCount = urlCount;
                    t.TaskEntity.WebpageLink = webLinks;

                    string tNamePath = tIndex.GetSavePath(i);
                    tNamePath = Path.GetDirectoryName(tNamePath) + "\\" + Path.GetFileNameWithoutExtension(tNamePath) + "_copy" + ".smt";

                    t.TaskEntity.TaskName = Path.GetFileNameWithoutExtension(tNamePath);

                    t.TaskEntity.IsExportGDateTime = true;
                    //t.TaskEntity.TaskClass = "";

                    t.SaveTask(tNamePath);

                    #region  下面是启动任务的代码
                    string tName = System.IO.Path.GetFileNameWithoutExtension(tNamePath);

                    //NetMiner.Gather.Task.oTask t1 = new NetMiner.Gather.Task.oTask(this.m_workPath);
                    //t1.LoadTask(tNamePath);

                    //t1.IsExportGDateTime = true;
                    //t1.TaskClass = "";

                    //t1.SaveTask(tNamePath);
                    //t1 = null;

                    Int64 TaskID = Int64.Parse(DateTime.Now.ToFileTime().ToString());

                    if (!System.IO.Directory.Exists(cTool.getPrjPath() + "tasks\\run"))
                    {
                        System.IO.Directory.CreateDirectory(cTool.getPrjPath() + "tasks\\run");
                    }
                    System.IO.File.Copy(tNamePath,
                        cTool.getPrjPath() + "tasks\\run\\task" + TaskID + ".rst", true);

                    cGatherTask t2 = null;

                    t2 = m_GatherControl.TaskManage.FindTask(TaskID);
                    if (t2 == null)
                    {
                        t2 = AddRunTask(tID, TaskID, tName, tNamePath);
                    }

                    m_GatherControl.Start(t2);
                    #endregion
                }
            }
            uAnaly = null;
        }

        private string[] ReplaceExtPara(string paraName,DataTable d)
        {
            ArrayList al = new ArrayList();
            for (int i=0;i<d.Columns.Count;i++)
            {
                if (paraName==d.Columns[i].ColumnName)
                {
                    for (int j=0;j<d.Rows.Count;j++)
                    {
                        if (!string.IsNullOrEmpty(d.Rows[j][i].ToString()))
                            al.Add(d.Rows[j][i].ToString());
                    }
                    break;
                }
            }

            return (string[])al.ToArray(typeof(string));
        }

        #endregion
    }
}

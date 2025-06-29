using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using NetMiner.Gather.Listener;
using NetMiner.Core.Proxy;
using NetMiner.Resource;
using NetMiner.Core.gTask;
using NetMiner.Core.Event;
using NetMiner.Core.Entity;

///功能：采集任务管理 管理队列 绑定 响应事件 控制任务 
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
///修订：2010-12-8 采集控制器加载的时候，会自动加载代理IP
///根据需要采集任务的配置情况，来实现代理轮询采集操作

namespace NetMiner.Gather.Control
{
    /// <summary>
    /// 采集任务管理器，管理器的目的主要是管理采集任务的队列
    /// 控制器的目的是为了管理运行
    /// </summary>
    public class cGatherManage
    {

        private List<cGatherTask> m_list_Task;
        private cTaskDataList m_TaskDataList;
        private cGatherTaskList m_GatherTaskList;
        private cEventProxy m_EventProxy;

        //定义一个代理信息控制类，并传引用给采集类，根据采集任务的
        //代理配置信息，去获取代理
        private cProxyControl m_ProxyControl;

        private string m_workPath = string.Empty;

        private bool m_isGlobalRepeat = false;
        private NetMiner.Base.cHashTree g_Urls;

        private readonly static object _MyLock = new object();

        #region 构造
        public cGatherManage(string workPath,bool isGlobalRepeat, NetMiner.Base.cHashTree gUrl)
        {
            m_workPath = workPath;
            m_isGlobalRepeat = isGlobalRepeat;
            g_Urls = gUrl;

            m_list_Task = new List<cGatherTask>();
            m_TaskDataList = new cTaskDataList();
            m_GatherTaskList = new cGatherTaskList();
            m_EventProxy = new cEventProxy();

            //加载代理服务器
            this.m_ProxyControl = new cProxyControl(m_workPath);

        }
        #endregion

        #region 代理IP信息的维护加载
        public void UpdateProxy()
        {
            this.m_ProxyControl.UpdateProxy();
        }
        #endregion

        #region 静态变量

        /// 线程最大错误次数，当达到这个数量则自动停止任务，
        /// 前提条件是系统必须检测是断网，如果没有断网，则引发错误事件
        /// 但不停止任务执行
        private static int s_MaxErrorCount = 10;
        public static int MaxErrorCount
        {
            get { return cGatherManage.s_MaxErrorCount; }
            set { cGatherManage.s_MaxErrorCount = value; }
        }

        #endregion

        #region 属性

        /// 获取当前事件代理对象
        internal cEventProxy EventProxy
        {
            get { return m_EventProxy; }
        }

        /// 事件 线程同步锁
        private readonly Object m_taskListFileLock = new Object();

        /// 文件 线程同步锁
        private readonly Object m_taskFileLock = new Object();

        /// <summary>
        /// 采集任务的队列，包括所有状态的任务信息
        /// </summary>
        public List<cGatherTask> TaskList
        {
            get { return m_list_Task; }
        }

        /// 获取当前 采集任务队列控制器
        public cGatherTaskList TaskListControl
        {
            get
            {
                if (m_GatherTaskList == null)
                {
                    m_GatherTaskList = new cGatherTaskList();
                }
                return m_GatherTaskList;
            }
        }

        #endregion

        //从当前的采集任务列表中,按照指定的TaskID查找
        //一个采集任务,并返回

        public cGatherTask FindTask(Int64 TaskID)
        {
            try
            {
                lock (_MyLock)
                {
                    foreach (cGatherTask gt in m_list_Task )
                    {
                        if (gt.TaskID ==TaskID )
                        {
                            return gt;
                        }
                    }
                }
                return null;
            }
            catch (System.Exception ex)
            {
                if (e_Log != null)
                    e_Log(this, new cGatherTaskLogArgs(TaskID, "", cGlobalParas.LogType.Error, "集合发生变化：" + TaskID + " " + ex.Message, true));
                throw new Exception("集合发生变化：" + TaskID + " " + ex.Message);

            }
        }

        #region 任务控制 增加任务 任务初始化
        /// 向采集任务队列中增加一个采集任务
        public void Add(cTaskData tData)
        {
            try
            {
                //新建一个采集任务,并把采集任务的数据传入此采集任务中
                cGatherTask gTask = new cGatherTask(m_workPath,this.m_isGlobalRepeat, ref g_Urls, this,ref this.m_ProxyControl , tData);

                //初始化此采集任务,主要是注册此任务的相关事件
                TaskInit(gTask,ref m_ProxyControl);

                //判断此任务是否已经加入此任务数据集合,如果没有加入,则加入集合
                if (!m_TaskDataList.TaskDataList.Contains(tData))
                {
                    m_TaskDataList.TaskDataList.Add(tData);
                }

                lock (_MyLock)
                {
                    //将此采集任务添加到采集任务队列中
                    m_list_Task.Add(gTask);
                }

                //根据添加的任务状态,自动维护队列的信息
                m_GatherTaskList.AutoList(gTask);

                //如果任务增加后就是完成的任务，则需要出发完成的
                //事件
                if (gTask.TaskState == cGlobalParas.TaskState.Completed)
                {
                    e_TaskCompleted(gTask, new cTaskEventArgs(gTask.TaskID, gTask.TaskName, false));
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public void AddMonitorTask(cTaskData tData, string TaskName,bool isExportUrl)
        {
            try
            {
                //新建一个采集任务,并把采集任务的数据传入此采集任务中
                cGatherTask gTask = new cGatherTask(this.m_workPath,false, ref g_Urls, this,ref this.m_ProxyControl ,tData);

                //初始化此采集任务,主要是注册此任务的相关事件
                TaskInit(gTask,ref m_ProxyControl);

                //判断此任务是否已经加入此任务数据集合,如果没有加入,则加入集合
                if (!m_TaskDataList.TaskDataList.Contains(tData))
                {
                    m_TaskDataList.TaskDataList.Add(tData);
                }

                lock (_MyLock)
                {
                    //将此采集任务添加到采集任务队列中
                    m_list_Task.Add(gTask);
                }

                //根据添加的任务状态,自动维护队列的信息
                m_GatherTaskList.AutoList(gTask);

                //如果任务增加后就是完成的任务，则需要出发完成的
                //事件
                if (gTask.TaskState == cGlobalParas.TaskState.Completed)
                {
                    e_TaskCompleted(gTask, new cTaskEventArgs(gTask.TaskID, gTask.TaskName, false));
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 任务初始化，主要的目的是注册事件
        /// </summary>
        /// <param name="gTask"></param>
        /// <param name="ProxyControl"></param>
        private void TaskInit(cGatherTask gTask,ref cProxyControl ProxyControl)
        {
            m_ProxyControl = ProxyControl;

            if (gTask.TaskManage.Equals(this))
            {
                if (!gTask.IsInitialized)
                {
                    gTask.TaskCompleted += this.onTaskCompleted;
                    gTask.TaskFailed += this.onTaskFailed;
                    gTask.TaskStopped += this.onTaskStopped;
                    gTask.TaskStarted += this.onTaskStarted;
                    gTask.TaskAborted += this.onTaskAborted;
                    gTask.Log += this.onLog;
                    gTask.GData += this.onGData;
                    gTask.GUrlCount += this.onGUrlCount;
                    gTask.TaskError += this.onTaskError;
                    gTask.TaskStateChanged += this.onTaskStateChanged;
                    gTask.TaskThreadInitialized += this.onTaskThreadInitialized;
                    gTask.ShowLogInfo += this.onShowInfo;

                    gTask.RunTaskLog += this.onRunTaskLog;

                    gTask.RunTask += this.onRunSoukeyTask;

                    gTask.IsInitialized = true;
                }
            }
        }

        //任务强制终止，销毁事件关联，不让其返回任何信息
        public void TaskEventDispose(cGatherTask gTask)
        {
            if (gTask.TaskManage.Equals(this))
            {
                
                    gTask.TaskCompleted -= this.onTaskCompleted;
                    gTask.TaskFailed -= this.onTaskFailed;
                    gTask.TaskStopped -= this.onTaskStopped;
                    gTask.TaskStarted -= this.onTaskStarted;
                    gTask.TaskAborted -= this.onTaskAborted;
                    gTask.Log -= this.onLog;
                    gTask.GData -= this.onGData;
                    gTask.GUrlCount += this.onGUrlCount;
                    gTask.TaskError -= this.onTaskError;
                    gTask.TaskStateChanged -= this.onTaskStateChanged;
                    gTask.RunTask -= this.onRunSoukeyTask;
                    gTask.RunTaskLog -= this.onRunTaskLog;
                    gTask.ShowLogInfo -= this.onShowInfo;

                    gTask.TaskThreadInitialized -= this.onTaskThreadInitialized;

               
            }
        }

        #endregion

        #region 事件

        /// 采集任务 完成事件
        private event EventHandler<cTaskEventArgs> e_TaskCompleted;
        public event EventHandler<cTaskEventArgs> TaskCompleted
        {
            add { e_TaskCompleted += value; }
            remove { e_TaskCompleted -= value; }
        }

        /// 采集任务 失败事件
        private event EventHandler<cTaskEventArgs> e_TaskFailed;
        public event EventHandler<cTaskEventArgs> TaskFailed
        {
            add { e_TaskFailed += value; }
            remove { e_TaskFailed -= value; }
        }

        /// 采集任务 开始采集事件
        private event EventHandler<cTaskEventArgs> e_TaskStarted;
        public event EventHandler<cTaskEventArgs> TaskStarted
        {
            add { e_TaskStarted += value; }
            remove { e_TaskStarted -= value; }
        }

        /// 采集任务 停止事件
        private event EventHandler<cTaskEventArgs> e_TaskStopped;
        public event EventHandler<cTaskEventArgs> TaskStopped
        {
            add { e_TaskStopped += value; }
            remove { e_TaskStopped -= value; }
        }

        /// 采集任务 取消事件
        private event EventHandler<cTaskEventArgs> e_TaskAborted;
        public event EventHandler<cTaskEventArgs> TaskAborted
        {
            add { e_TaskAborted += value; }
            remove { e_TaskAborted -= value; }
        }

        ///采集任务 错误事件
        private event EventHandler<TaskErrorEventArgs> e_TaskError;
        public event EventHandler<TaskErrorEventArgs> TaskError
        {
            add { e_TaskError += value; }
            remove { e_TaskError -= value; }
        }

        /// 采集任务状态 变更事件
        private event EventHandler<TaskStateChangedEventArgs> e_TaskStateChanged;

        public event EventHandler<TaskStateChangedEventArgs> TaskStateChanged
        {
            add { e_TaskStateChanged += value; }
            remove { e_TaskStateChanged -= value; }
        }

        /// 采集任务 初始化事件
        private event EventHandler<TaskInitializedEventArgs> e_TaskInitialized;
        public event EventHandler<TaskInitializedEventArgs> TaskInitialized
        {
            add { e_TaskInitialized += value; }
            remove { e_TaskInitialized -= value; }
        }

        /// <summary>
        /// 采集日志事件
        /// </summary>
        private event EventHandler<cGatherTaskLogArgs> e_Log;
        public event EventHandler<cGatherTaskLogArgs> Log
        {
            add {  e_Log += value;  }
            remove {  e_Log -= value;  }
        }

        /// <summary>
        /// 触发器执行时的日志事件
        /// </summary>
        private event EventHandler<cRunTaskLogArgs> e_RunTaskLog;
        public event EventHandler<cRunTaskLogArgs> RunTaskLog
        {
            add { e_RunTaskLog += value; }
            remove { e_RunTaskLog -= value; }
        }


        /// <summary>
        /// 采集数据事件
        /// </summary>
        private event EventHandler<cGatherDataEventArgs> e_GData;
        public event EventHandler<cGatherDataEventArgs> GData
        {
            add {  e_GData += value;  }
            remove { e_GData -= value;  }
        }

        public event EventHandler<cGatherUrlCounterArgs> e_GUrlCount;
        public event EventHandler<cGatherUrlCounterArgs> GUrlCount
        {
            add { e_GUrlCount += value; }
            remove { e_GUrlCount -= value; }
        }

        /// <summary>
        /// 触发器执行网络矿工任务时的事件响应
        /// </summary>
        private event EventHandler<cRunTaskEventArgs> e_RunTask;
        public event EventHandler<cRunTaskEventArgs> RunTask
        {
            add {  e_RunTask += value;  }
            remove {  e_RunTask -= value; }
        }

        /// <summary>
        /// 想界面的托盘图片及系统消息返回信息
        /// </summary>
        private event EventHandler<ShowInfoEventArgs> e_ShowLogInfo;
        public event EventHandler<ShowInfoEventArgs> ShowLogInfo
        {
            add { e_ShowLogInfo += value; } 
            remove { e_ShowLogInfo -= value; } 
        }
        #endregion

        #region 事件处理

        /// <summary>
        /// 处理任务触发器响应执行网络矿工任务时的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onRunSoukeyTask(object sender, cRunTaskEventArgs e)
        {
            e_RunTask(this, new cRunTaskEventArgs(e.MessType, e.RunName, e.RunPara));
        }

        /// 处理 任务完成 事件
        private void onTaskCompleted(object sender, cTaskEventArgs e)
        {
            if (e_TaskCompleted != null && !e.Cancel)
            {
                e_TaskCompleted(sender, e);
            }

            // 将任务对象添加到已完成的任务队列，等待任务管理器处理
            m_GatherTaskList.FinishTask((cGatherTask)sender);

            //将完成的信息写入taskrun
            //任务完成后，需要将更新的Cookie重新进行保存，以确保Cookie最新有效
            cGatherTask gTask = FindTask(e.TaskID);
            //string cookie=gTask.TaskData.Cookie 


        }

        /// <summary>
        /// 处理 任务失败 事件
        /// </summary>
        /// <param name="sender">触发事件的任务</param>
        /// <param name="e"></param>
        private void onTaskFailed(object sender, cTaskEventArgs e)
        {
            if (e_TaskFailed != null && !e.Cancel)
            {
                e_TaskFailed(sender, e);
            }
        }

        /// <summary>
        /// 采集错误 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void onTaskError(object sender, TaskErrorEventArgs e)
        {
            // 加入重试队列
            //m_GatherTaskList.AddWaitingWorkThread(e.ErrorThread);

            if (e_TaskError != null)
            {
                e_TaskError(sender, e);
            }
        }

        private void onTaskStarted(object sender, cTaskEventArgs e)
        {
            if (e_TaskStarted != null && !e.Cancel)
            {
                e_TaskStarted(sender, e);
            }
        }

        private void onTaskStopped(object sender, cTaskEventArgs e)
        {
            if (e_TaskStopped != null && !e.Cancel)
            {
                e_TaskStopped(sender, e);
            }
        }

        private void onTaskAborted(object sender, cTaskEventArgs e)
        {
            cGatherTask task = (cGatherTask)sender;

            lock (_MyLock)
            {
                // 从任务列表删除
                m_list_Task.Remove(task);
            }

            m_TaskDataList.TaskDataList.Remove(task.TaskData);

            if (e_TaskAborted != null && !e.Cancel)
            {
                e_TaskAborted(sender, e);
            }

        }

        /// <summary>
        /// 处理 任务状态变更 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onTaskStateChanged(object sender, TaskStateChangedEventArgs e)
        {
            //在此处理TaskRun的状态问题
            oTaskRun tRun = new oTaskRun(m_workPath);
            if (e.NewState == cGlobalParas.TaskState.Completed || e.NewState == cGlobalParas.TaskState.Pause
                || e.NewState == cGlobalParas.TaskState.Stopped || e.NewState == cGlobalParas.TaskState.Failed)
            {
                tRun.EditTaskState(e.TaskID, ((cGatherTask)sender).TaskData.GatherDataCount, e.NewState);
            }
            else
            {
                tRun.EditTaskState(e.TaskID, e.NewState);
            }
            tRun = null;

            if (e_TaskStateChanged != null && !e.Cancel)
            {
                e_TaskStateChanged(sender, e);
            }

            // 重要：此处处理所有状态变更后任务队列的变更
            m_GatherTaskList.AutoList((cGatherTask)sender);
        }

        /// <summary>
        /// 处理任务线程初始化完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onTaskThreadInitialized(object sender, TaskInitializedEventArgs e)
        {
            if (e_TaskInitialized != null)
            {
                e_TaskInitialized(sender, e);
            }
            // 保存当前任务采集的状态
            //this.SaveTaskList();
            //this.SaveTask((cGatherTask)sender);
        }

        //处理日志事件
        public void onLog(object sender, cGatherTaskLogArgs e)
        {
            if (e_TaskStarted != null && !e.Cancel)
            {
                e_Log(sender, e);
            }
        }

        //触发器执行日志事件
        public void onRunTaskLog(object sender, cRunTaskLogArgs e)
        {
            if (e_RunTaskLog != null)
            {
                e_RunTaskLog(sender, e);
            }
        }

        public void onShowInfo(object sender, ShowInfoEventArgs e)
        {
            if (e_ShowLogInfo != null)
                e_ShowLogInfo(sender, e);
        }

        //处理数据事件
        public void onGData(object sender, cGatherDataEventArgs e)
        {
            if (e_GData != null && !e.Cancel)
            {
                e_GData(sender, e);
            }
        }

        public void onGUrlCount(object sender,cGatherUrlCounterArgs e)
        {
            if (e_GUrlCount!=null && !e.Cancel)
            {
                e_GUrlCount(sender, e);
            }
        }

        #endregion
    }

}

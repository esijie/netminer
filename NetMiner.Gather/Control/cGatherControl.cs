using System;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Core;
using NetMiner.Core.Event;
using NetMiner.Core.Entity;

///功能：采集任务控制 启动 停止 暂停 重置
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace NetMiner.Gather.Control
{
    //采集任务的控制
    public class cGatherControl : IDisposable
    {

        //private Queue<long> m_DCountQueue;
        //private Queue<int> m_DTtimeQueue;
        private int m_CompletedCount;
        private bool m_IsInitialized;
        private int m_LastTime;
        private bool m_Isbusying;
        private System.Threading.Timer m_GatherEngine;

        //定义一个值，判断是否进行代理IP的更新
        private bool m_IsAutoUpadateProxy=false ;
        private DateTime m_LastUpdateProxyTime;
        #region 类构造

        //定义一个值，表示记录当前的工作路径
        private string m_workPath = string.Empty;

        public cGatherControl(string workPath,bool isGlobalRepeat, ref NetMiner.Base.cHashTree gUrls)
        {
            m_workPath = workPath;

            m_TaskManage = new cGatherManage(m_workPath,isGlobalRepeat,gUrls);

            if (ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Cloud ||
                ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Enterprise ||
                ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Ultimate ||
                ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Server ||
                ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.DistriServer)
            {
                cXmlSConfig scon = new cXmlSConfig(this.m_workPath);
                this.m_IsAutoUpadateProxy = scon.IsAutoUpdateProxy;
                scon.Dispose();
                scon = null;
            }

            m_LastUpdateProxyTime = System.DateTime.Now;
        }

        ~cGatherControl()
        {
            Dispose(false);
        }
        #endregion

        #region 属性
        private cGatherManage m_TaskManage;
        public cGatherManage TaskManage
        {
            get { return m_TaskManage; }
            set { m_TaskManage = value; }
        }
        #endregion

        #region 计时器
        /// <summary>
        /// 启动采集任务队列控制计时器，定期轮询每个状态的队列，当超过最大限定值，则不再加入，通过定时器轮询，由系统自动维护。
        /// 此计时器的作用是轮询当前的任务队列，看是否有已经完成的任务
        /// 及等待执行的任务，如果存在则执行它
        /// </summary>
        private void timerInit()
        {
            m_LastTime = System.Environment.TickCount;
            m_GatherEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_GatherEngine_CallBack), null, 0, 50);
            m_IsInitialized = true;
            m_Isbusying = false;
        }

        /// <summary>
        /// 这是一个自动化任务队列运行的操作，当采集任务超过了最大限制，系统为了
        /// 节约资源，则限定此任务不再运行，然后等其他任务运行完毕后，检测到有等待的
        /// 任务，则开始运行。
        /// </summary>
        /// <param name="State"></param>
        private void m_GatherEngine_CallBack(object State)
        {
            if (!m_Isbusying)
            {
                m_Isbusying = true;
               
                m_TaskManage.TaskListControl.TaskDispose();

                // 判断已完成任务数和总任务数相等则触发 所有任务完成 事件
                if (m_TaskManage.TaskListControl.CompletedTaskList.Count != m_CompletedCount)
                {
                    m_CompletedCount = m_TaskManage.TaskListControl.CompletedTaskList.Count;
                    //m_TaskManage.SaveTaskList();
                    if (m_CompletedCount == m_TaskManage.TaskList.Count)
                    {
                        onCompleted();

                        //全部任务采集完成后，可以进行代理IP的更改
                        if (this.m_IsAutoUpadateProxy == true && ((TimeSpan)(System.DateTime.Now - this.m_LastUpdateProxyTime)).TotalMinutes > 60)
                        {
                            //更新代理IP
                            this.m_TaskManage.UpdateProxy();
                            this.m_LastUpdateProxyTime = System.DateTime.Now;
                        }

                    }
                }
                else if (m_TaskManage.TaskListControl.RunningTaskList.Count == 0)
                {
                    //如果正在运行的队列等于0，也定期进行代理IP更新
                    if (this.m_IsAutoUpadateProxy == true && ((TimeSpan)(System.DateTime.Now - this.m_LastUpdateProxyTime)).TotalMinutes > 60)
                    {
                        //更新代理IP
                        this.m_TaskManage.UpdateProxy();
                        this.m_LastUpdateProxyTime = System.DateTime.Now;
                    }
                }

                // 检查并开始等待的任务
                m_TaskManage.TaskListControl.AutoNext();

                // 处理所有代理事件
                m_TaskManage.EventProxy.DoEvents();

                m_Isbusying = false;
            }
        }

        #endregion

    

        #region 采集任务控制操作

        /// <summary>
        /// 增加运行区所有的任务，当系统关闭后，有些任务会由于各种原因被停止，则这些任务就
        /// 停留在了运行区，每次系统启动后，要将这些任务加载到管理器的队列中，以供用户进行
        /// 操作，譬如：继续运行、删除等。此方法一般用于系统初始化使用。
        /// </summary>
        /// <param name="taskDataList"></param>
        /// <returns></returns>
        public bool AddGatherTask(cTaskDataList taskDataList)
        {
            //根据运行区数据进行采集任务的添加
            //如果有任务加载出错，则忽略错误，继续加载，确保所有的任务都可以加载成功
            bool IsSucceed = true;

            for (int i=0 ;i<taskDataList.TaskCount;i++)
            {
                //在此不能增加进入发布状态的采集任务
                if (taskDataList.TaskDataList[i].TaskState != cGlobalParas.TaskState.Publishing &&
                    taskDataList.TaskDataList[i].TaskState != cGlobalParas.TaskState.PublishStop &&
                    taskDataList.TaskDataList[i].TaskState != cGlobalParas.TaskState.PublishFailed)
                {
                    try
                    {
                        m_TaskManage.Add(taskDataList.TaskDataList[i]);
                    }
                    catch (System.Exception ex)
                    {
                        IsSucceed = false;
                    }
                }
            }

            return IsSucceed;
        }

        //增加单个任务
        public void AddGatherTask(cTaskData task)
        {
            m_TaskManage.Add(task);
        }

        //增加监控采集任务,与普通采集任务不同的是
        //采集任务的数据初始化方式不同
        public void AddMonitorTask(cTaskData task,string TaskName,bool isExortUrl)
        {
            m_TaskManage.AddMonitorTask(task, TaskName, isExortUrl);
        }

        
        /// 开始指定的采集任务
        public void Start(cGatherTask task)
        {
            if (!m_IsInitialized)
            {
                timerInit();
            }
            m_TaskManage.TaskListControl.StartTask(task);
        }
        
        /// 启动所有运行区的任务
        public void Start()
        {
            if (!m_IsInitialized)
            {
                timerInit();
            }
            m_TaskManage.TaskListControl.Start();
        }

        /// 停止所有运行区的任务
        public void Stop()
        {
            m_TaskManage.TaskListControl.Stop();
        }

        /// 停止指定的采集任务
        public void Stop(cGatherTask  task)
        {
            m_TaskManage.TaskListControl.StopTask(task);
        }

        public void Over(cGatherTask task)
        {
            m_TaskManage.TaskListControl.OverTask(task);
        }

        /// 重新开始指定的采集任务
        //public void ReStart(cGatherTask task)
        //{
        //    m_TaskManage.TaskListControl.ReStartTask(task);
        //}

        /// 删除指定的采集任务
        public void Remove(cGatherTask task)
        {
            m_TaskManage.TaskListControl.RemoveTask(task);
        }

        public void Abort()
        {
            while(m_TaskManage.TaskListControl.RunningTaskList.Count>0)
            {
                Abort(m_TaskManage.TaskListControl.RunningTaskList[0]);
            }

        }

        public void Abort(cGatherTask task)
        {
            m_TaskManage.TaskListControl.Abort(task);
            m_TaskManage.TaskListControl.AutoList(task);
        }

        #endregion


        #region 事件

        /// 全部采集完成事件
        private event EventHandler<cTaskEventArgs> e_Completed;
        public event EventHandler<cTaskEventArgs> Completed
        {
            add { e_Completed += value; }
            remove { e_Completed -= value; }
        }

        private void onCompleted()
        {
            if (e_Completed != null)
            {
                e_Completed.Invoke(this, new cTaskEventArgs());
            }
        }
        #endregion

        #region IDisposable 成员
        private bool m_disposed;
        /// <summary>
        /// 释放由 Download 的当前实例使用的所有资源
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
                    // 在此释放托管资源
                    //if (m_GatherWatch != null)
                    //{
                    //    m_GatherWatch.Dispose();
                    //}
                    if (m_GatherEngine != null)
                    {
                        m_GatherEngine.Dispose();
                    }
                }

                // 在此释放非托管资源

                m_disposed = true;
            }
        }


        #endregion
    }

    
}

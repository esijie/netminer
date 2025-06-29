using System;
using System.Collections.Generic;
using System.IO;
using NetMiner.Resource;
using NetMiner.Core.Plan;
using NetMiner.Core.Plan.Entity;
using NetMiner.Core.Event;

///功能：计划检测类，由外部调用，检测需要执行的任务 
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace NetMiner.Gather.Listener
{
    public class cCheckPlan
    {
        List<ePlan> m_runTasks;
        private bool m_IsReloading = false;
        private cFileMonitor m_FileMonitor;
        private string m_workPath = string.Empty;

        #region 构造和析构
        public cCheckPlan(string workPath)
        {
            m_workPath = workPath;
            if (!File.Exists(workPath + "tasks\\plan\\plan.xml"))
            {
                oPlans cs = new oPlans(workPath);
                cs.NewIndexFile();
                cs = null;
            }

            try
            {
                m_runTasks = new List<ePlan>();

                IniCheckPlan();
                      
                //初始化任务文件监听类,，根据文件变化不断更新任务监控信息
                m_FileMonitor = new cFileMonitor(workPath + "tasks\\plan\\plan.xml");
                m_FileMonitor.ReloadPlanFile += this.On_Reload;

                //系统默认为启动文件监听
                StartListenPlanFile();
            }
            catch (System.Exception ex)
            {
                if (e_ListenErrorEvent != null)
                {
                    e_ListenErrorEvent(this, new cListenErrorEventArgs(ex.Message));
                }
            }
        }

        ~cCheckPlan()
        {
            m_FileMonitor.Stop();
            m_FileMonitor = null;
        }
        #endregion 

        //控制文件监听
        public void StartListenPlanFile()
        {
           
            m_FileMonitor.Start();
        }

        public void StopListenPlanFile()
        {
            m_FileMonitor.Stop();
        }

        //定期由外部调用
        //检测需要执行的任务，如果需要执行，则把任务
        //压到执行的任务队列中
        public void CheckPlan()
        {
            //重新加载计划期间禁止运行计划检测操作
            if (m_IsReloading == false)
            {
                eTaskPlan tPlan;
                
 
                for (int i = 0; i < m_runTasks.Count; i++)
                {
                    //无效的计划不进行判断，计划的状态由执行时进行维护

                    if (m_runTasks[i].PlanState ==cGlobalParas.PlanState.Enabled)
                    {
                        if (DateTime.Compare(DateTime.Now, DateTime.Parse(m_runTasks[i].EnabledDateTime)) < 0)
                        {
                            //表示还未到生效时间，不启用此任务的检测
                            continue;
                        }

                        //需要重新进行计算
                        //if (m_runTasks[i].PlanRunTime == "" || m_runTasks[i].PlanRunTime == null)
                        //{
                            m_runTasks[i].PlanRunTime = m_runTasks[i].NextRunTime;
                        //}
                        //else
                        //{
                            double douTime = TimeSpan.Parse(DateTime.Now.Subtract(DateTime.Parse(m_runTasks[i].PlanRunTime)).ToString()).TotalSeconds;

                            //表示下次运行的时间已经到了，但还未超过分钟，如果超过5分钟，则默认系统没有执行此任务
                            //是否再次执行由配置参数决定：IsOverRun
                            if ((douTime > -30 && douTime < 30))
                                //if (douTime>0)
                            {
                                //将任务压入任务队列
                                for (int j = 0; j < m_runTasks[i].RunTasks.Count; j++)
                                {
                                    //在此需要重新初始化执行任务的数据，主要是要增加计划的ID和计划的名称
                                    tPlan = new eTaskPlan();

                                    tPlan.PlanID = m_runTasks[i].PlanID.ToString();
                                    tPlan.PlanName = m_runTasks[i].PlanName;
                                    tPlan.RunTaskType = m_runTasks[i].RunTasks[j].RunTaskType;
                                    tPlan.RunTaskName = m_runTasks[i].RunTasks[j].RunTaskName;
                                    tPlan.RunTaskPara = m_runTasks[i].RunTasks[j].RunTaskPara;

                                    e_AddRunTaskEvent(this, new cAddRunTaskEventArgs(tPlan));
                                }

                                m_runTasks[i].PlanRunTime = m_runTasks[i].NextRunTime;
                            //}
                        }
                    }
                }
            }
        }

        //加载计划,加载计划的时候需要对计划的状态进行维护
        private void IniCheckPlan()
        {
            try
            {
                oPlans op = new oPlans(this.m_workPath);
                IEnumerable<ePlan> eps = op.LoadPlans();
                
                foreach(ePlan p in eps)
                {
                    m_runTasks.Add(p);
                }

                //自动维护计划状态
                AutoState();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        //重新加载任务
        private void ReIniCheckPlan()
        {
            m_IsReloading = true;
            m_runTasks = null;
            m_runTasks = new List<ePlan>();
            IniCheckPlan();
            m_IsReloading = false;
        }

        private void AutoState()
        {
            oPlans cp =new oPlans(this.m_workPath);

            for (int i = 0; i < m_runTasks.Count; i++)
            {
                if (m_runTasks[i].IsDisabled == true)
                    cp.IfEnabled(m_runTasks[i].PlanID,true);
            }

            cp = null;
        }

        private void On_Reload(object sender, cCommandEventArgs e)
        {
            switch (e.MessType)
            {
                case cGlobalParas.MessageType.ReloadPlan :
                    ReIniCheckPlan();
                    break;
                case cGlobalParas.MessageType.MonitorFileFaild :
                    m_FileMonitor.Stop();
                    m_FileMonitor.Start();
                    break;
                default :
                    break;
            }
        }

        private readonly Object m_eventLock = new Object();

        #region 事件

        /// 采集任务完成事件
        private event EventHandler<cAddRunTaskEventArgs> e_AddRunTaskEvent;
        internal event EventHandler<cAddRunTaskEventArgs> AddRunTaskEvent
        {
            add { lock (m_eventLock) { e_AddRunTaskEvent += value; } }
            remove { lock (m_eventLock) { e_AddRunTaskEvent -= value; } }
        }

        private event EventHandler<cListenErrorEventArgs> e_ListenErrorEvent;
        internal event EventHandler<cListenErrorEventArgs> ListenErrorEvent
        {
            add { lock (m_eventLock) { e_ListenErrorEvent += value; } }
            remove { lock (m_eventLock) { e_ListenErrorEvent -= value; } }
        }

        #endregion
    
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using NetMiner.Resource;
using NetMiner.Core.Plan;
using NetMiner.Core.Plan.Entity;
using NetMiner.Core.Event;

///���ܣ��ƻ�����࣬���ⲿ���ã������Ҫִ�е����� 
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace NetMiner.Gather.Listener
{
    public class cCheckPlan
    {
        List<ePlan> m_runTasks;
        private bool m_IsReloading = false;
        private cFileMonitor m_FileMonitor;
        private string m_workPath = string.Empty;

        #region ���������
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
                      
                //��ʼ�������ļ�������,�������ļ��仯���ϸ�����������Ϣ
                m_FileMonitor = new cFileMonitor(workPath + "tasks\\plan\\plan.xml");
                m_FileMonitor.ReloadPlanFile += this.On_Reload;

                //ϵͳĬ��Ϊ�����ļ�����
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

        //�����ļ�����
        public void StartListenPlanFile()
        {
           
            m_FileMonitor.Start();
        }

        public void StopListenPlanFile()
        {
            m_FileMonitor.Stop();
        }

        //�������ⲿ����
        //�����Ҫִ�е����������Ҫִ�У��������
        //ѹ��ִ�е����������
        public void CheckPlan()
        {
            //���¼��ؼƻ��ڼ��ֹ���мƻ�������
            if (m_IsReloading == false)
            {
                eTaskPlan tPlan;
                
 
                for (int i = 0; i < m_runTasks.Count; i++)
                {
                    //��Ч�ļƻ��������жϣ��ƻ���״̬��ִ��ʱ����ά��

                    if (m_runTasks[i].PlanState ==cGlobalParas.PlanState.Enabled)
                    {
                        if (DateTime.Compare(DateTime.Now, DateTime.Parse(m_runTasks[i].EnabledDateTime)) < 0)
                        {
                            //��ʾ��δ����Чʱ�䣬�����ô�����ļ��
                            continue;
                        }

                        //��Ҫ���½��м���
                        //if (m_runTasks[i].PlanRunTime == "" || m_runTasks[i].PlanRunTime == null)
                        //{
                            m_runTasks[i].PlanRunTime = m_runTasks[i].NextRunTime;
                        //}
                        //else
                        //{
                            double douTime = TimeSpan.Parse(DateTime.Now.Subtract(DateTime.Parse(m_runTasks[i].PlanRunTime)).ToString()).TotalSeconds;

                            //��ʾ�´����е�ʱ���Ѿ����ˣ�����δ�������ӣ��������5���ӣ���Ĭ��ϵͳû��ִ�д�����
                            //�Ƿ��ٴ�ִ�������ò���������IsOverRun
                            if ((douTime > -30 && douTime < 30))
                                //if (douTime>0)
                            {
                                //������ѹ���������
                                for (int j = 0; j < m_runTasks[i].RunTasks.Count; j++)
                                {
                                    //�ڴ���Ҫ���³�ʼ��ִ����������ݣ���Ҫ��Ҫ���Ӽƻ���ID�ͼƻ�������
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

        //���ؼƻ�,���ؼƻ���ʱ����Ҫ�Լƻ���״̬����ά��
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

                //�Զ�ά���ƻ�״̬
                AutoState();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        //���¼�������
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

        #region �¼�

        /// �ɼ���������¼�
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

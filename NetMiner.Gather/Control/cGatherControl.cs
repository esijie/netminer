using System;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Core;
using NetMiner.Core.Event;
using NetMiner.Core.Entity;

///���ܣ��ɼ�������� ���� ֹͣ ��ͣ ����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace NetMiner.Gather.Control
{
    //�ɼ�����Ŀ���
    public class cGatherControl : IDisposable
    {

        //private Queue<long> m_DCountQueue;
        //private Queue<int> m_DTtimeQueue;
        private int m_CompletedCount;
        private bool m_IsInitialized;
        private int m_LastTime;
        private bool m_Isbusying;
        private System.Threading.Timer m_GatherEngine;

        //����һ��ֵ���ж��Ƿ���д���IP�ĸ���
        private bool m_IsAutoUpadateProxy=false ;
        private DateTime m_LastUpdateProxyTime;
        #region �๹��

        //����һ��ֵ����ʾ��¼��ǰ�Ĺ���·��
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

        #region ����
        private cGatherManage m_TaskManage;
        public cGatherManage TaskManage
        {
            get { return m_TaskManage; }
            set { m_TaskManage = value; }
        }
        #endregion

        #region ��ʱ��
        /// <summary>
        /// �����ɼ�������п��Ƽ�ʱ����������ѯÿ��״̬�Ķ��У�����������޶�ֵ�����ټ��룬ͨ����ʱ����ѯ����ϵͳ�Զ�ά����
        /// �˼�ʱ������������ѯ��ǰ��������У����Ƿ����Ѿ���ɵ�����
        /// ���ȴ�ִ�е��������������ִ����
        /// </summary>
        private void timerInit()
        {
            m_LastTime = System.Environment.TickCount;
            m_GatherEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_GatherEngine_CallBack), null, 0, 50);
            m_IsInitialized = true;
            m_Isbusying = false;
        }

        /// <summary>
        /// ����һ���Զ�������������еĲ��������ɼ����񳬹���������ƣ�ϵͳΪ��
        /// ��Լ��Դ�����޶������������У�Ȼ�����������������Ϻ󣬼�⵽�еȴ���
        /// ������ʼ���С�
        /// </summary>
        /// <param name="State"></param>
        private void m_GatherEngine_CallBack(object State)
        {
            if (!m_Isbusying)
            {
                m_Isbusying = true;
               
                m_TaskManage.TaskListControl.TaskDispose();

                // �ж��������������������������򴥷� ����������� �¼�
                if (m_TaskManage.TaskListControl.CompletedTaskList.Count != m_CompletedCount)
                {
                    m_CompletedCount = m_TaskManage.TaskListControl.CompletedTaskList.Count;
                    //m_TaskManage.SaveTaskList();
                    if (m_CompletedCount == m_TaskManage.TaskList.Count)
                    {
                        onCompleted();

                        //ȫ������ɼ���ɺ󣬿��Խ��д���IP�ĸ���
                        if (this.m_IsAutoUpadateProxy == true && ((TimeSpan)(System.DateTime.Now - this.m_LastUpdateProxyTime)).TotalMinutes > 60)
                        {
                            //���´���IP
                            this.m_TaskManage.UpdateProxy();
                            this.m_LastUpdateProxyTime = System.DateTime.Now;
                        }

                    }
                }
                else if (m_TaskManage.TaskListControl.RunningTaskList.Count == 0)
                {
                    //����������еĶ��е���0��Ҳ���ڽ��д���IP����
                    if (this.m_IsAutoUpadateProxy == true && ((TimeSpan)(System.DateTime.Now - this.m_LastUpdateProxyTime)).TotalMinutes > 60)
                    {
                        //���´���IP
                        this.m_TaskManage.UpdateProxy();
                        this.m_LastUpdateProxyTime = System.DateTime.Now;
                    }
                }

                // ��鲢��ʼ�ȴ�������
                m_TaskManage.TaskListControl.AutoNext();

                // �������д����¼�
                m_TaskManage.EventProxy.DoEvents();

                m_Isbusying = false;
            }
        }

        #endregion

    

        #region �ɼ�������Ʋ���

        /// <summary>
        /// �������������е����񣬵�ϵͳ�رպ���Щ��������ڸ���ԭ��ֹͣ������Щ�����
        /// ͣ��������������ÿ��ϵͳ������Ҫ����Щ������ص��������Ķ����У��Թ��û�����
        /// ������Ʃ�磺�������С�ɾ���ȡ��˷���һ������ϵͳ��ʼ��ʹ�á�
        /// </summary>
        /// <param name="taskDataList"></param>
        /// <returns></returns>
        public bool AddGatherTask(cTaskDataList taskDataList)
        {
            //�������������ݽ��вɼ���������
            //�����������س�������Դ��󣬼������أ�ȷ�����е����񶼿��Լ��سɹ�
            bool IsSucceed = true;

            for (int i=0 ;i<taskDataList.TaskCount;i++)
            {
                //�ڴ˲������ӽ��뷢��״̬�Ĳɼ�����
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

        //���ӵ�������
        public void AddGatherTask(cTaskData task)
        {
            m_TaskManage.Add(task);
        }

        //���Ӽ�زɼ�����,����ͨ�ɼ�����ͬ����
        //�ɼ���������ݳ�ʼ����ʽ��ͬ
        public void AddMonitorTask(cTaskData task,string TaskName,bool isExortUrl)
        {
            m_TaskManage.AddMonitorTask(task, TaskName, isExortUrl);
        }

        
        /// ��ʼָ���Ĳɼ�����
        public void Start(cGatherTask task)
        {
            if (!m_IsInitialized)
            {
                timerInit();
            }
            m_TaskManage.TaskListControl.StartTask(task);
        }
        
        /// ��������������������
        public void Start()
        {
            if (!m_IsInitialized)
            {
                timerInit();
            }
            m_TaskManage.TaskListControl.Start();
        }

        /// ֹͣ����������������
        public void Stop()
        {
            m_TaskManage.TaskListControl.Stop();
        }

        /// ָֹͣ���Ĳɼ�����
        public void Stop(cGatherTask  task)
        {
            m_TaskManage.TaskListControl.StopTask(task);
        }

        public void Over(cGatherTask task)
        {
            m_TaskManage.TaskListControl.OverTask(task);
        }

        /// ���¿�ʼָ���Ĳɼ�����
        //public void ReStart(cGatherTask task)
        //{
        //    m_TaskManage.TaskListControl.ReStartTask(task);
        //}

        /// ɾ��ָ���Ĳɼ�����
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


        #region �¼�

        /// ȫ���ɼ�����¼�
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

        #region IDisposable ��Ա
        private bool m_disposed;
        /// <summary>
        /// �ͷ��� Download �ĵ�ǰʵ��ʹ�õ�������Դ
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
                    // �ڴ��ͷ��й���Դ
                    //if (m_GatherWatch != null)
                    //{
                    //    m_GatherWatch.Dispose();
                    //}
                    if (m_GatherEngine != null)
                    {
                        m_GatherEngine.Dispose();
                    }
                }

                // �ڴ��ͷŷ��й���Դ

                m_disposed = true;
            }
        }


        #endregion
    }

    
}

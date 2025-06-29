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

///���ܣ��ɼ�������� ������� �� ��Ӧ�¼� �������� 
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
///�޶���2010-12-8 �ɼ����������ص�ʱ�򣬻��Զ����ش���IP
///������Ҫ�ɼ�����������������ʵ�ִ�����ѯ�ɼ�����

namespace NetMiner.Gather.Control
{
    /// <summary>
    /// �ɼ��������������������Ŀ����Ҫ�ǹ���ɼ�����Ķ���
    /// ��������Ŀ����Ϊ�˹�������
    /// </summary>
    public class cGatherManage
    {

        private List<cGatherTask> m_list_Task;
        private cTaskDataList m_TaskDataList;
        private cGatherTaskList m_GatherTaskList;
        private cEventProxy m_EventProxy;

        //����һ��������Ϣ�����࣬�������ø��ɼ��࣬���ݲɼ������
        //����������Ϣ��ȥ��ȡ����
        private cProxyControl m_ProxyControl;

        private string m_workPath = string.Empty;

        private bool m_isGlobalRepeat = false;
        private NetMiner.Base.cHashTree g_Urls;

        private readonly static object _MyLock = new object();

        #region ����
        public cGatherManage(string workPath,bool isGlobalRepeat, NetMiner.Base.cHashTree gUrl)
        {
            m_workPath = workPath;
            m_isGlobalRepeat = isGlobalRepeat;
            g_Urls = gUrl;

            m_list_Task = new List<cGatherTask>();
            m_TaskDataList = new cTaskDataList();
            m_GatherTaskList = new cGatherTaskList();
            m_EventProxy = new cEventProxy();

            //���ش��������
            this.m_ProxyControl = new cProxyControl(m_workPath);

        }
        #endregion

        #region ����IP��Ϣ��ά������
        public void UpdateProxy()
        {
            this.m_ProxyControl.UpdateProxy();
        }
        #endregion

        #region ��̬����

        /// �߳���������������ﵽ����������Զ�ֹͣ����
        /// ǰ��������ϵͳ�������Ƕ��������û�ж����������������¼�
        /// ����ֹͣ����ִ��
        private static int s_MaxErrorCount = 10;
        public static int MaxErrorCount
        {
            get { return cGatherManage.s_MaxErrorCount; }
            set { cGatherManage.s_MaxErrorCount = value; }
        }

        #endregion

        #region ����

        /// ��ȡ��ǰ�¼��������
        internal cEventProxy EventProxy
        {
            get { return m_EventProxy; }
        }

        /// �¼� �߳�ͬ����
        private readonly Object m_taskListFileLock = new Object();

        /// �ļ� �߳�ͬ����
        private readonly Object m_taskFileLock = new Object();

        /// <summary>
        /// �ɼ�����Ķ��У���������״̬��������Ϣ
        /// </summary>
        public List<cGatherTask> TaskList
        {
            get { return m_list_Task; }
        }

        /// ��ȡ��ǰ �ɼ�������п�����
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

        //�ӵ�ǰ�Ĳɼ������б���,����ָ����TaskID����
        //һ���ɼ�����,������

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
                    e_Log(this, new cGatherTaskLogArgs(TaskID, "", cGlobalParas.LogType.Error, "���Ϸ����仯��" + TaskID + " " + ex.Message, true));
                throw new Exception("���Ϸ����仯��" + TaskID + " " + ex.Message);

            }
        }

        #region ������� �������� �����ʼ��
        /// ��ɼ��������������һ���ɼ�����
        public void Add(cTaskData tData)
        {
            try
            {
                //�½�һ���ɼ�����,���Ѳɼ���������ݴ���˲ɼ�������
                cGatherTask gTask = new cGatherTask(m_workPath,this.m_isGlobalRepeat, ref g_Urls, this,ref this.m_ProxyControl , tData);

                //��ʼ���˲ɼ�����,��Ҫ��ע������������¼�
                TaskInit(gTask,ref m_ProxyControl);

                //�жϴ������Ƿ��Ѿ�������������ݼ���,���û�м���,����뼯��
                if (!m_TaskDataList.TaskDataList.Contains(tData))
                {
                    m_TaskDataList.TaskDataList.Add(tData);
                }

                lock (_MyLock)
                {
                    //���˲ɼ�������ӵ��ɼ����������
                    m_list_Task.Add(gTask);
                }

                //������ӵ�����״̬,�Զ�ά�����е���Ϣ
                m_GatherTaskList.AutoList(gTask);

                //����������Ӻ������ɵ���������Ҫ������ɵ�
                //�¼�
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
                //�½�һ���ɼ�����,���Ѳɼ���������ݴ���˲ɼ�������
                cGatherTask gTask = new cGatherTask(this.m_workPath,false, ref g_Urls, this,ref this.m_ProxyControl ,tData);

                //��ʼ���˲ɼ�����,��Ҫ��ע������������¼�
                TaskInit(gTask,ref m_ProxyControl);

                //�жϴ������Ƿ��Ѿ�������������ݼ���,���û�м���,����뼯��
                if (!m_TaskDataList.TaskDataList.Contains(tData))
                {
                    m_TaskDataList.TaskDataList.Add(tData);
                }

                lock (_MyLock)
                {
                    //���˲ɼ�������ӵ��ɼ����������
                    m_list_Task.Add(gTask);
                }

                //������ӵ�����״̬,�Զ�ά�����е���Ϣ
                m_GatherTaskList.AutoList(gTask);

                //����������Ӻ������ɵ���������Ҫ������ɵ�
                //�¼�
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
        /// �����ʼ������Ҫ��Ŀ����ע���¼�
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

        //����ǿ����ֹ�������¼������������䷵���κ���Ϣ
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

        #region �¼�

        /// �ɼ����� ����¼�
        private event EventHandler<cTaskEventArgs> e_TaskCompleted;
        public event EventHandler<cTaskEventArgs> TaskCompleted
        {
            add { e_TaskCompleted += value; }
            remove { e_TaskCompleted -= value; }
        }

        /// �ɼ����� ʧ���¼�
        private event EventHandler<cTaskEventArgs> e_TaskFailed;
        public event EventHandler<cTaskEventArgs> TaskFailed
        {
            add { e_TaskFailed += value; }
            remove { e_TaskFailed -= value; }
        }

        /// �ɼ����� ��ʼ�ɼ��¼�
        private event EventHandler<cTaskEventArgs> e_TaskStarted;
        public event EventHandler<cTaskEventArgs> TaskStarted
        {
            add { e_TaskStarted += value; }
            remove { e_TaskStarted -= value; }
        }

        /// �ɼ����� ֹͣ�¼�
        private event EventHandler<cTaskEventArgs> e_TaskStopped;
        public event EventHandler<cTaskEventArgs> TaskStopped
        {
            add { e_TaskStopped += value; }
            remove { e_TaskStopped -= value; }
        }

        /// �ɼ����� ȡ���¼�
        private event EventHandler<cTaskEventArgs> e_TaskAborted;
        public event EventHandler<cTaskEventArgs> TaskAborted
        {
            add { e_TaskAborted += value; }
            remove { e_TaskAborted -= value; }
        }

        ///�ɼ����� �����¼�
        private event EventHandler<TaskErrorEventArgs> e_TaskError;
        public event EventHandler<TaskErrorEventArgs> TaskError
        {
            add { e_TaskError += value; }
            remove { e_TaskError -= value; }
        }

        /// �ɼ�����״̬ ����¼�
        private event EventHandler<TaskStateChangedEventArgs> e_TaskStateChanged;

        public event EventHandler<TaskStateChangedEventArgs> TaskStateChanged
        {
            add { e_TaskStateChanged += value; }
            remove { e_TaskStateChanged -= value; }
        }

        /// �ɼ����� ��ʼ���¼�
        private event EventHandler<TaskInitializedEventArgs> e_TaskInitialized;
        public event EventHandler<TaskInitializedEventArgs> TaskInitialized
        {
            add { e_TaskInitialized += value; }
            remove { e_TaskInitialized -= value; }
        }

        /// <summary>
        /// �ɼ���־�¼�
        /// </summary>
        private event EventHandler<cGatherTaskLogArgs> e_Log;
        public event EventHandler<cGatherTaskLogArgs> Log
        {
            add {  e_Log += value;  }
            remove {  e_Log -= value;  }
        }

        /// <summary>
        /// ������ִ��ʱ����־�¼�
        /// </summary>
        private event EventHandler<cRunTaskLogArgs> e_RunTaskLog;
        public event EventHandler<cRunTaskLogArgs> RunTaskLog
        {
            add { e_RunTaskLog += value; }
            remove { e_RunTaskLog -= value; }
        }


        /// <summary>
        /// �ɼ������¼�
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
        /// ������ִ�����������ʱ���¼���Ӧ
        /// </summary>
        private event EventHandler<cRunTaskEventArgs> e_RunTask;
        public event EventHandler<cRunTaskEventArgs> RunTask
        {
            add {  e_RunTask += value;  }
            remove {  e_RunTask -= value; }
        }

        /// <summary>
        /// ����������ͼƬ��ϵͳ��Ϣ������Ϣ
        /// </summary>
        private event EventHandler<ShowInfoEventArgs> e_ShowLogInfo;
        public event EventHandler<ShowInfoEventArgs> ShowLogInfo
        {
            add { e_ShowLogInfo += value; } 
            remove { e_ShowLogInfo -= value; } 
        }
        #endregion

        #region �¼�����

        /// <summary>
        /// �������񴥷�����Ӧִ�����������ʱ���¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onRunSoukeyTask(object sender, cRunTaskEventArgs e)
        {
            e_RunTask(this, new cRunTaskEventArgs(e.MessType, e.RunName, e.RunPara));
        }

        /// ���� ������� �¼�
        private void onTaskCompleted(object sender, cTaskEventArgs e)
        {
            if (e_TaskCompleted != null && !e.Cancel)
            {
                e_TaskCompleted(sender, e);
            }

            // �����������ӵ�����ɵ�������У��ȴ��������������
            m_GatherTaskList.FinishTask((cGatherTask)sender);

            //����ɵ���Ϣд��taskrun
            //������ɺ���Ҫ�����µ�Cookie���½��б��棬��ȷ��Cookie������Ч
            cGatherTask gTask = FindTask(e.TaskID);
            //string cookie=gTask.TaskData.Cookie 


        }

        /// <summary>
        /// ���� ����ʧ�� �¼�
        /// </summary>
        /// <param name="sender">�����¼�������</param>
        /// <param name="e"></param>
        private void onTaskFailed(object sender, cTaskEventArgs e)
        {
            if (e_TaskFailed != null && !e.Cancel)
            {
                e_TaskFailed(sender, e);
            }
        }

        /// <summary>
        /// �ɼ����� �¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void onTaskError(object sender, TaskErrorEventArgs e)
        {
            // �������Զ���
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
                // �������б�ɾ��
                m_list_Task.Remove(task);
            }

            m_TaskDataList.TaskDataList.Remove(task.TaskData);

            if (e_TaskAborted != null && !e.Cancel)
            {
                e_TaskAborted(sender, e);
            }

        }

        /// <summary>
        /// ���� ����״̬��� �¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onTaskStateChanged(object sender, TaskStateChangedEventArgs e)
        {
            //�ڴ˴���TaskRun��״̬����
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

            // ��Ҫ���˴���������״̬�����������еı��
            m_GatherTaskList.AutoList((cGatherTask)sender);
        }

        /// <summary>
        /// ���������̳߳�ʼ������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onTaskThreadInitialized(object sender, TaskInitializedEventArgs e)
        {
            if (e_TaskInitialized != null)
            {
                e_TaskInitialized(sender, e);
            }
            // ���浱ǰ����ɼ���״̬
            //this.SaveTaskList();
            //this.SaveTask((cGatherTask)sender);
        }

        //������־�¼�
        public void onLog(object sender, cGatherTaskLogArgs e)
        {
            if (e_TaskStarted != null && !e.Cancel)
            {
                e_Log(sender, e);
            }
        }

        //������ִ����־�¼�
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

        //���������¼�
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

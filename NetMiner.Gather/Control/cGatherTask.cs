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

///���ܣ��ɼ�������
///���ʱ�䣺2010-8-3
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵���� ������2010��8��3�ս����˰汾��������Ҫ��������ʱ�޸���splitTask������ʹ��֧��ֱ�ӴӲɼ�������
/// ���ļ���ȡ���ݣ�������ֻ��runĿ¼����ȡ���ݣ���������Ŀ����Ϊ��֧���״��زɼ�����ͨ�ɼ��Ὠ�������б�
/// ���״����в��Ὠ�������б����Ը���TaskID��runĿ¼�¾��޷���ȡ���ݣ����ԣ���Ҫ�޸Ľӿڣ�����Ӧ�״�
/// Ӧ�ã��汾Ϊ��2.0
///�汾��02.10.00
///�޶�����
///�޶� 2011��10��8�� ���������ؿ�Ĵ�������汾����Ϊ��V2.1 ��Ҫ������ַ����
///ϵͳ�汾����Ϊ��V2.6.0.03
namespace NetMiner.Gather.Control
{
    ///�ɼ�������,����һ���ɼ�����(��һ��Task.xml�ļ�)

    public class cGatherTask
    {
        private cTaskData m_TaskData;
        private cGlobalParas.TaskState m_State;
        private List<cGatherTaskSplit> m_list_GatherTaskSplit;
        private bool m_IsDataInitialized;
        private bool m_IsInitialized;
        private cGatherManage m_TaskManage;

        private bool m_ThreadsRunning = false;

        //����һ���������ж��Ƿ񱣴����������״̬
        //�����������������뱣�棬������״�������
        //�����豣��
        private bool m_IsSaveTask = true ;

        //����һ��������Ϣ�����࣬�������ø��ɼ��࣬���ݲɼ������
        //����������Ϣ��ȥ��ȡ����
        private cProxyControl m_ProxyControl;

        //����һ�����ԣ�����Url����
        private NetMiner.Base.cHashTree m_Urls;

        //����һ�����ԣ����жϵ�����ʱ�ɼ���ַ�ļ�¼������һ�����ؿ�
        private NetMiner.Base.cHashTree m_TempUrls;

        //����һ�����ԣ��жϲɼ������Ƿ��ظ�
        private NetMiner.Base.cHashTree m_DataRepeat;

        //����һ��ֵ������ȫ������
        private bool m_isGlobalRepeat = false;
        private NetMiner.Base.cHashTree m_GlobalUrls;

        //����һ��ֵ���жϵ�ǰ�Ĵ�������η���
        private bool m_isProxySplit = false;

        private string m_workPath = string.Empty;

        /// <summary>
        /// ����һ���ɼ������ʵ���࣬���ڻ�ȡ�ɼ������������Ϣ
        /// </summary>
        private eTask m_Task = null;

        #region ���� ���� 
  
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

            //���ݴ����taskdata�����زɼ�����Ļ������ݣ���Ϊ�ɼ������ʱ�Ѿ��������ɼ�����
            //����������˴Ӳɼ��������������ش�����Ļ�������
            oTask t = new oTask(this.m_workPath);
            t.LoadTask(TaskData.TaskID);
            this.m_Task = t.TaskEntity;
            t.Dispose();
            t = null;


            #region ����һ���ɼ����ݵ����ؿ�
            m_DataRepeat = new NetMiner.Base.cHashTree();
            try
            {
                m_DataRepeat.Open(this.m_workPath + "tasks\\run\\data" + this.TaskName + ".db");
            }
            catch (System.Exception ex)
            {
                if (e_Log != null)
                {
                    e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID,this.m_TaskData.TaskName , cGlobalParas.LogType.Error, "�����ļ���ʧ�ܣ�" + ex.Message, true));
                }
            }
            #endregion

            #region �½�һ�����ؿ⣬���ڵ�ǰ����ִ��ʱ�ϵ�������ַ����
            m_TempUrls = new NetMiner.Base.cHashTree();
            try
            {
                m_TempUrls.Open(m_workPath + "tasks\\run\\task" + this.TaskID + ".db");
            }
            catch (System.Exception ex)
            {
                if (e_Log != null)
                {
                    e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID,this.m_TaskData.TaskName, cGlobalParas.LogType.Error, "�����ļ���ʧ�ܣ�" + ex.Message, true));
                }
            }
            #endregion

            //���������ݴ�����֮��,ֱ�ӶԵ�ǰ�����������ֽ�,
            //�Ƿ���Ҫ���߳̽���,����ʼ�������������
            SplitTask();

            ////��ʼ��ʼ������
            //TaskInit();

            m_IsSaveTask = true;
        }

        /// <summary>
        /// �˷���ר�����״��غͲ��Բɼ�
        /// </summary>
        /// <param name="taskManage"></param>
        /// <param name="ProxyControl"></param>
        /// <param name="taskData"></param>
        /// <param name="TaskName"></param>
        /// <param name="isExportUrl">�״���ǿ������ɼ���ַ�����Բɼ���������ý��д���</param>
        //internal cGatherTask(string workPath, cGatherManage taskManage, ref cProxyControl ProxyControl, 
        //    cTaskData taskData, string TaskName, bool isExportUrl)
        //{
        //    this.m_workPath = workPath;
        //    m_TaskManage = taskManage;
        //    m_TaskData = taskData;
        //    m_State = TaskData.TaskState;
        //    m_ProxyControl = ProxyControl;

        //    m_list_GatherTaskSplit = new List<cGatherTaskSplit>();

        //    //����һ���ɼ����ݵ����ؿ�
        //    m_DataRepeat = new NetMiner.Base.cHashTree();
        //    try
        //    {
        //        m_DataRepeat.Open(m_workPath + "tasks\\run\\data" + this.TaskName + ".db");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        if (e_Log != null)
        //        {
        //            e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID,this.m_TaskData.TaskName, cGlobalParas.LogType.Error, "�����ļ���ʧ�ܣ�" + ex.Message, true));
        //        }
        //    }

        //    //�½�һ�����ؿ⣬���ڵ�ǰ����ִ��ʱ�ϵ�������ַ����
        //    m_TempUrls = new NetMiner.Base.cHashTree();
        //    try
        //    {
        //        m_TempUrls.Open(m_workPath + "tasks\\run\\task" + this.TaskID + ".db");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        if (e_Log != null)
        //        {
        //            e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID,this.m_TaskData.TaskName, cGlobalParas.LogType.Error, "�����ļ���ʧ�ܣ�" + ex.Message, true));
        //        }
        //    }

        //    //���������ݴ�����֮��,ֱ�ӶԵ�ǰ�����������ֽ�,
        //    //�Ƿ���Ҫ���߳̽���,����ʼ�������������
        //    SplitTask(TaskName, isExportUrl);

        //    //��ʼ��ʼ������
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


        #region ����

        private string[] AutoID { get; set; }
        private cCookieManage cookieManage { get; set; }
        /// <summary>
        /// �¼� �߳�ͬ����
        /// </summary>
        private readonly Object m_eventLock = new Object();
        /// <summary>
        /// �ļ� �߳�ͬ����
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
        /// ����/��ȡ ��ǰ�������ݶ���
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
        /// ��ȡ�ɼ���ַ�ĸ���
        /// </summary>
        public int UrlCount
        {
            get { return m_TaskData.UrlCount; }
        }

        /// <summary>
        /// ��ȡʵ����Ҫ�ɼ�����ַ����
        /// </summary>
        public int UrlNaviCount
        {
            get { return m_TaskData.UrlNaviCount; }
        }

        /// <summary>
        /// ʵ�ʲɼ����߳�������Ϊ��ڵ�ַ�����1���߳���3��ϵͳ���ǻᰴ��1���߳���ִ�С�
        /// </summary>
        public int ThreadCount { get; set; }
        /// <summary>
        /// ��ȡ����ɲɼ����������
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
        /// ��ȡ�ɼ�ʧ����ַ������
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
        /// ����/��ȡ ����ID
        /// </summary>
        public Int64 TaskID
        {
            get { return m_TaskData.TaskID; }
        }

        /// <summary>
        /// ����/��ȡ ������
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
        /// ��ȡ�ɼ��������������
        /// </summary>
        public cGlobalParas.TaskRunType RunType
        {
            get { return m_TaskData.RunType; }
        }
        /// <summary>
        /// �Ƿ��Ѿ��ɼ����
        /// </summary>
        public bool IsCompleted
        {
            get { return GatheredUrlCount ==UrlCount ; }
        }

        /// <summary>
        /// �ֽ������� ���
        /// </summary>
        public List<cGatherTaskSplit> GatherTaskSplit
        {
            get { return m_list_GatherTaskSplit; }
            set { m_list_GatherTaskSplit = value; }
        }

        //V5.1����

        public DateTime StartTimer
        {
            get { return m_TaskData.StartTimer; }
        }

        #endregion


        #region �¼����� ����״̬����
        /// ����״̬�ı���¼�����
        /// /// ����/��ȡ ����״̬ �����ڲ�ʹ�ã������¼���
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

                // ע�⣬�����漰����״̬������¼����ڴ˴���
                bool cancel = (evt != null && evt.Cancel);

                switch (m_State)
                {
                    case cGlobalParas.TaskState.Aborted:
                        // ���� ����ǿ��ֹͣȡ�� �¼�
                        //����ǿ��ֹͣ����Ȼ�������ݣ����п��ܻᶪʧ������Ϊ�ڴ�ϵͳҪ�Ƴ���ϵͳ�����
                        //���д���
                        if (e_TaskAborted != null)
                        {
                            Save();
                            e_TaskAborted(this, new cTaskEventArgs(TaskID,TaskName,cancel));
                        }
                        break;
                    case cGlobalParas.TaskState.Completed:
                        // ���� ������� �¼�
                        if (e_TaskCompleted != null)
                        {

                            //������ֹͣ�󣬿�ʼ���������ִ��״̬
                            Save();

                            e_TaskCompleted(this, new cTaskEventArgs(TaskID, TaskName, cancel));
                        }
                        break;
                    case cGlobalParas.TaskState.Failed:
                        // ���� ����ʧ�� �¼�
                        if (e_TaskFailed != null)
                        {
                            //����ʧ��
                            Save();
                            e_TaskFailed(this, new cTaskEventArgs(TaskID, TaskName, cancel));
                        }
                        break;
                    case cGlobalParas.TaskState.Started:
                        // ���� ����ʼ �¼�
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
                            // ���� ����ֹͣ �¼�
                            if (e_TaskStopped != null)
                            {
                                //������ֹͣ�󣬿�ʼ���������ִ��״̬
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

     

        #region ������� ���� ֹͣ ���� ȡ��

        /// ��ʼ����
        public void Start()
        {
            
            // ȷ��λ��ʼ���������Ƚ��г�ʼ�����������ļ���ȡ��������Ϣ��
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

        /// �������вɼ������߳� ���û�зֽ�����,�������ĵ����߳̽��вɼ�
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

        /// ����׼���������ȴ���ʼ��
        public void ReadyToStart()
        {
            if (m_State != cGlobalParas.TaskState.Started && m_State != cGlobalParas.TaskState.Completed)
            {
                State = cGlobalParas.TaskState.Waiting;
            }
        }

        /// ֹͣ����
        public void Stop()
        {
            m_ThreadsRunning = false;

            foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
            {
                dtc.Stop();
            }

            //��ʼ����Ƿ������̶߳�����ɻ��˳�
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

        //ֹͣ���񣬴�ֹͣ������Stop��ͬ���ǣ�ǿ��ֹͣ���й����߳�
        //Stop������ִ����һ������������ֹͣ����Abort�����Ƿ�ִ�е�
        //����״̬������ǿ��ֹͣ�����ַ�ʽ�ᵼ�����ݶ�ʧ
        /// ȡ�������Ƴ�����
        public void Abort()
        {
            m_ThreadsRunning = false;

            foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
            {
                dtc.Abort();
            }
            State = cGlobalParas.TaskState.Aborted;
        }

        ///�������е�������Ҫ�Ǳ��浱ǰ���е�״̬
        ///���񱣴���Ҫͬʱ����taskrun.xml����Ҫ�Ǳ���ɼ�����
        ///ע�⣬��������ݴ����������ӵ�ַ�ᷢ���仯����Ϊ�������½�ʱ���������ӵ�ַ�п��ܴ���һ���ò���
        ///������һ����ʼִ�У����в�������ַ�ͻ���н�����ͬʱ�ǰ��ս��������ַ�����Ƿ�ɼ��ı�ʶ�����ԣ��ٴ�
        ///��������ӵ�ַ��ܶ�
        public void Save()
        {
            if (m_IsSaveTask == false)
                return;

            //���ȱ������ؿ���Ϣ
            //�������ؿ�ı��棬�����Ҫ���صĻ�
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
                            e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID,this.m_TaskData.TaskName ,cGlobalParas.LogType.Error, "���ؿⱣ��ʧ�ܣ�" + ex.Message, true));
                        }
                }
                //m_Urls = null;
            }

            //������ʱ���ؿ⣬��ʱ���ؿ����ڶϵ�����ʹ��
            try
            {
                m_TempUrls.Save(this.m_workPath + "tasks\\run\\task" + this.TaskID + ".db");
                
            }
            catch (System.Exception ex)
            {
                if (e_Log != null)
                {
                    e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID,this.m_TaskData.TaskName, cGlobalParas.LogType.Error, "�����ļ�����ʧ�ܣ�" + ex.Message, true));
                }
            }

            //������ʱ���������ؿ�
            try
            {
                this.m_DataRepeat.Save(this.m_workPath + "tasks\\run\\data" + this.m_TaskData.TaskClass.Replace("/", "-") + "-"+ this.TaskName + ".db");

            }
            catch (System.Exception ex)
            {
                if (e_Log != null)
                {
                    e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID, this.m_TaskData.TaskName, cGlobalParas.LogType.Error, "�����ļ�����ʧ�ܣ�" + ex.Message, true));
                }
            }

            string FileName = this.m_workPath + "tasks\\run\\task" + this.TaskID + ".rst";
            string runFileindex = this.m_workPath + "tasks\\taskrun.xml";

            #region ��ʼ����Cookie
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


        //            //����ɼ��ص�ַ�Ƿ���Ҫ����
        //            //�������ַ�ĵ�������
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

        //                //�������ַ�Ķ�ҳ�ɼ�����
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

        #region  �෽�� �ڲ�ʹ��

        //����ָ��������ID�Ե�ǰ��������зֽ⣬����е���ҳ��Ҳ��Ҫ�ڴ˽���
        //�ֽ�
        //����ʼ��������Ĺؼ�����
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TaskName">����������·�����ļ���</param>
        /// <param name="IsExportUrl">�Ƿ�ǿ�����������ַ</param>
        //private void SplitTask(string TaskName,bool IsExportUrl)
        //{
        //    oTask t = new oTask(this.m_workPath);

        //    try
        //    {
        //        t.LoadTask(TaskName);

        //        //�жϴ������Ƿ���Ҫ�������ش���
        //        if (t.TaskEntity.IsUrlNoneRepeat== true)
        //        {
        //            //��ʼ�����ؿ�
        //            m_Urls = new NetMiner.Base.cHashTree();
        //            try
        //            {
        //                m_Urls.Open(this.m_workPath + "urls\\" + this.m_TaskData.TaskClass + "-" + this.m_TaskData.TaskName + ".db");
        //            }
        //            catch (System.Exception ex)
        //            {
        //                if (e_Log != null)
        //                {
        //                    e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID, this.m_TaskData.TaskName, cGlobalParas.LogType.Error, "���ؿ��ʧ�ܣ�" + ex.Message, true));
        //                }
        //            }
        //        }
                
        //        SplitGatherTask(t);

        //    }
        //    catch (System.Exception)
        //    {
        //        //����ʵ���ļ�����ʧ�ܣ��п������ļ���ʧ�����
        //        //��������Ҫ����һ������Ϣ���Ա���������ʾ�˶�ʧ������
        //        //�����û�����ͨ���������ɾ�����������ݣ�����һ�����
        //        //��ʧ�ļ��Ĵ����ֶ�
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
        /// ��ʼ���ɼ�������Ҫ���ڷֽ�ɼ������߳�
        /// </summary>
        private void SplitTask()
        {
            try
            {

                //�жϴ������Ƿ���Ҫ�������ش���
                if (this.m_Task.IsUrlNoneRepeat == true)
                {
                    //��ʼ�����ؿ�
                    m_Urls = new NetMiner.Base.cHashTree();
                    try
                    {
                        m_Urls.Open(this.m_workPath + "urls\\" + this.m_TaskData.TaskClass + "-" + this.m_TaskData.TaskName + ".db");
                    }
                    catch (System.Exception ex)
                    {
                        if (e_Log != null)
                        {
                            e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID, this.m_TaskData.TaskName, cGlobalParas.LogType.Error, "���ؿ��ʧ�ܣ�" + ex.Message, true));
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


            #region ��ʼ��������Ϣ�����ص�taskData��
            m_TaskData.TaskDemo = t.TaskDemo;
            m_TaskData.PublishType = t.ExportType;

            //if (t.ThreadProxy.Count > 0 && t.ThreadProxy[0].pType != cGlobalParas.ProxyType.TaskConfig)
            //    m_isProxySplit = false;

            if (t.ThreadProxy.Count == t.ThreadCount)
                m_isProxySplit = true;                      //��ʾ�û�Ϊÿ���̶߳������˴���

            #endregion

                #region ������ҳ��ַ���ݣ����Url���в������ڴ˽��зֽ�

                ////������ҳ��ַ���ݼ��ɼ���־����
                ////�ٴ�ȥ����������в�������ַ,����Ҫ���зֽ�
                ////ȷ�����ص���ַ�϶���һ����Ч����ַ
                ////ע��,��ʱ�����п��ֽܷ�������Ϣ,����,��ַ�����ڴ˻ᷢ���仯,����,���ջ����޸���ַ����
            List<eWebLink> webLinks = new List<eWebLink>();
            eWebLink w;
            //Task.cUrlAnalyze u = new Task.cUrlAnalyze();
            //�ڴ�ʹ��urlAnalyze�࣬�����ڷֽ��ֵ䣬�����ڷֽ���ַ�����Դ�����Ϣ���Բ�������
            NetMiner.Core.Url.cUrlParse u = new NetMiner.Core.Url.cUrlParse(this.m_workPath);

            for (int i = 0; i < t.WebpageLink.Count; i++)
            {
                if (Regex.IsMatch(t.WebpageLink[i].Weblink.ToString(), "{.*}") ||
                     t.WebpageLink[i].Weblink.ToString().Contains("\r\n"))
                {
                    List<string> Urls;

                    //�ȷֽ���ַ���ֽ����ӵ�ϵͳ��
                    Urls = u.SplitWebUrl(t.WebpageLink[i].Weblink.ToString());

                    //��ʼ���m_TaskData.weblink����
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

                        //���ص�������
                        if (t.WebpageLink[i].IsNavigation == true)
                        {
                            w.NavigRules = t.WebpageLink[i].NavigRules;
                        }

                        w.IsMultiGather = t.WebpageLink[i].IsMultiGather;


                        //�˴����ض�ҳ�ɼ�����
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

            //���ֽ��Ĳɼ���ַ�������
            t.WebpageLink = webLinks;

            #endregion

            #region ���زɼ�����

            //m_TaskData.CutFlag = t.TaskEntity.WebpageCutFlag;

            //�жϲɼ��������Ƿ����Զ���ţ�������
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

            //�ڴ˳�ʼ���ļ����ص�·��
            string sPath = t.SavePath + "\\" + m_TaskData.TaskName + "_file";

            if (m_list_GatherTaskSplit.Count > 0)
            {   // ������ܴ��ڵ����߳�
                foreach (cGatherTaskSplit gts in m_list_GatherTaskSplit)
                {
                    gts.Stop();
                }
                m_list_GatherTaskSplit.Clear();
            }

            if (IsCompleted)
            {
                // �޸Ĵ˲ɼ������״̬Ϊ�Ѳɼ����,����Ϊ״̬Ϊ����ɣ���Ҫ�����¼�
                m_State = cGlobalParas.TaskState.Completed;

                //m_State = cGlobalParas.TaskState.Completed;

                //e_TaskCompleted(this, new cTaskEventArgs(m_TaskData.TaskID, false));
            }
            else
            {
                #region �ڴ˴����¼�����cookie��ȡ
                ///�ڴ˴����¼���cookie�Ļ�ȡ
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
                                e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID, this.m_TaskData.TaskName, cGlobalParas.LogType.Warning, "�����˵�¼���������ȴû���ṩ����ľ����ļ���", false));
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
                            e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID, this.m_TaskData.TaskName, cGlobalParas.LogType.Warning, "��ǰ�汾��֧�ֲ�����ܣ����ȡ��ȷ�汾��", false));
                        }
                    }
                }

                #endregion

                if (cookieManage==null)
                    cookieManage = new cCookieManage(t.Cookie);

                #region �����߳�����������ֽ�

                //���³�ʼ��UrlCount
                m_TaskData.UrlCount = t.WebpageLink.Count;

                //��ʼ��������ֿ�,���������Url����������߳���,���߳���>1
                if (m_TaskData.UrlCount > m_TaskData.ThreadCount && m_TaskData.ThreadCount > 1)
                {
                    int SplitUrlCount = (int)Math.Ceiling((decimal)m_TaskData.UrlCount / (decimal)m_TaskData.ThreadCount);

                    //����ÿ���ֽ��������ʼUrl��������ֹ��Url����
                    int StartIndex = 0;
                    int EndIndex = 0;
                    int j = 0;

                    #region ���̵߳ķֽ�
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

                        //��ʼ���ֽ�ɼ�������
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

                        //��ʼ���ֽ������������
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
                    #region �����������Ϊ��ڵ�ַС���̣߳�ϵͳǿ�Ƶ��̴߳����ˡ�
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
                //���ֽ�����߳̽����¼���
                TaskEventInit(TaskSplit);
            }
        }

        //����cookie��ֵ��cookie��ֵ������û���������������仯
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

        /// ��ʼ���ɼ������߳�
        //private void TaskInit()
        //{
        //    string sPath = this.m_Task.SavePath + "\\" + m_TaskData.TaskName + "_file";

        //    ///�����ʼ����Ϊ���������һ����δ����ִ�е�����һ�����Ѿ�������δִ����ϵ�����
        //    ///
        //    //m_TaskData.GatheredUrlCount = 0;
        //    //m_TaskData.GatherErrUrlCount = 0;
        //    //m_TaskData.TrueUrlCount = m_TaskData.UrlCount;

        //    if (!m_IsDataInitialized)
        //    {
        //        if (m_list_GatherTaskSplit.Count > 0)
        //        {   // ������ܴ��ڵ����߳�
        //            foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
        //            {
        //                dtc.Stop();
        //            }
        //            m_list_GatherTaskSplit.Clear();
        //        }

        //        if (IsCompleted)
        //        {   
        //            // �޸Ĵ˲ɼ������״̬Ϊ�Ѳɼ����,����Ϊ״̬Ϊ����ɣ���Ҫ�����¼�
        //            m_State = cGlobalParas.TaskState.Completed;

        //            //m_State = cGlobalParas.TaskState.Completed;

        //            //e_TaskCompleted(this, new cTaskEventArgs(m_TaskData.TaskID, false));
        //        }
        //        else
        //        {
        //            #region �ڴ˴����¼�����cookie��ȡ
        //            ///�ڴ˴����¼���cookie�Ļ�ȡ
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
        //                            e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID, this.m_TaskData.TaskName, cGlobalParas.LogType.Warning, "�����˵�¼���������ȴû���ṩ����ľ����ļ���", false));
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
        //                        e_Log(this, new cGatherTaskLogArgs(this.m_TaskData.TaskID, this.m_TaskData.TaskName, cGlobalParas.LogType.Warning, "��ǰ�汾��֧�ֲ�����ܣ����ȡ��ȷ�汾��", false));
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

        //                    //V5����
        //                    dtc.IsPluginsCookie = this.m_Task.IsPluginsCookie;
        //                    dtc.PluginsCookie = this.m_Task.PluginsCookie;
        //                    dtc.IsPluginsDeal = this.m_Task.IsPluginsDeal;
        //                    dtc.PluginsDeal = this.m_Task.PluginsDeal;
        //                    dtc.IsPluginsPublish = this.m_Task.IsPluginsPublish;
        //                    dtc.PluginsPublish = this.m_Task.PluginsPublish;

        //                    dtc.IsUrlAutoRedirect = this.m_Task.IsUrlAutoRedirect;

        //                    //V5.1����
        //                    dtc.IsGatherErrStop = this.m_Task.IsGatherErrStop;
        //                    dtc.GatherErrStopCount = this.m_Task.GatherErrStopCount;
        //                    dtc.GatherErrStopRule = this.m_Task.GatherErrStopRule;
        //                    dtc.IsInsertDataErrStop = this.m_Task.IsInsertDataErrStop;
        //                    dtc.InsertDataErrStopConut = this.m_Task.InsertDataErrStopConut;
        //                    dtc.IsGatherRepeatStop = this.m_Task.IsGatherRepeatStop;
        //                    dtc.GatherRepeatStopRule = this.m_Task.GatherRepeatStopRule;

        //                    //V5.2����
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

        //                    //V5.31����
        //                    dtc.isCookieList = this.m_Task.isCookieList;
        //                    dtc.GatherCount = m_TaskData.GatherCount;
        //                    dtc.RejectFlag = this.m_Task.RejectFlag;
        //                    dtc.RejectDeal = this.m_Task.RejectDeal;
        //                    dtc.isGatherCoding = this.m_Task.isGatherCoding;
        //                    dtc.GatherCodingFlag = this.m_Task.GatherCodingFlag;
        //                    dtc.CodeUrl = this.m_Task.CodeUrl;
        //                    dtc.GatherCodingPlugin = this.m_Task.GatherCodingPlugin;

        //                    //V5.5����
        //                    dtc.IsVisual = this.m_Task.IsVisual;

        //                    dtc.TaskSplitData = configData;

        //                    m_list_GatherTaskSplit.Add(dtc);

        //                    dtc = null;

        //                    threadIndex++;
        //                }

        //            }
        //            else
        //            {
        //                #region ���ǵ��̵߳Ĵ���ʽ
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

        //                //V5����
        //                dtc.IsPluginsCookie = this.m_Task.IsPluginsCookie;
        //                dtc.PluginsCookie = this.m_Task.PluginsCookie;
        //                dtc.IsPluginsDeal = this.m_Task.IsPluginsDeal;
        //                dtc.PluginsDeal = this.m_Task.PluginsDeal;
        //                dtc.IsPluginsPublish = this.m_Task.IsPluginsPublish;
        //                dtc.PluginsPublish = this.m_Task.PluginsPublish;

        //                dtc.IsUrlAutoRedirect = this.m_Task.IsUrlAutoRedirect;

        //                //V5.1����
        //                dtc.IsGatherErrStop = this.m_Task.IsGatherErrStop;
        //                dtc.GatherErrStopCount = this.m_Task.GatherErrStopCount;
        //                dtc.GatherErrStopRule = this.m_Task.GatherErrStopRule;
        //                dtc.IsInsertDataErrStop = this.m_Task.IsInsertDataErrStop;
        //                dtc.InsertDataErrStopConut = this.m_Task.InsertDataErrStopConut;
        //                dtc.IsGatherRepeatStop = this.m_Task.IsGatherRepeatStop;
        //                dtc.GatherRepeatStopRule = this.m_Task.GatherRepeatStopRule;

        //                //V5.2����
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

        //                //V5.31����
        //                    dtc.isCookieList = this.m_Task.isCookieList;
        //                dtc.GatherCount = this.m_Task.GatherCount;
        //                dtc.RejectFlag = this.m_Task.RejectFlag;
        //                dtc.RejectDeal = this.m_Task.RejectDeal;
        //                dtc.isGatherCoding = this.m_Task.isGatherCoding;
        //                dtc.GatherCodingFlag = this.m_Task.GatherCodingFlag;
        //                dtc.CodeUrl = this.m_Task.CodeUrl;
        //                dtc.GatherCodingPlugin = this.m_Task.GatherCodingPlugin;

        //                //V5.5����
        //                dtc.IsVisual = this.m_Task.IsVisual;

        //                // ���������½����߳�
        //                m_list_GatherTaskSplit.Add(dtc);

        //                dtc = null;
        //            }


        //            foreach (cGatherTaskSplit TaskSplit in m_list_GatherTaskSplit)
        //            {   
        //                // ��ʼ���������߳�
        //                TaskEventInit(TaskSplit);
        //            }
        //        }

        //        m_IsDataInitialized = true;
        //    }
        //}

        //���ֽ������¼����а�
        private void TaskEventInit(cGatherTaskSplit dtc)
        {
            if (!dtc.IsInitialized)
            {
                // �� ��ʼ���¼�������¼�
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

        /// ��������Ϊδ��ʼ��״̬
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

        /// ���òɼ�����Ϊδ����״̬
        //internal void ResetTaskData()
        //{
        //    // ֹͣ����
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

        //    //�޸�taskrun�ļ��У����ļ������Ĳɼ���ַ�ͳ����ַΪ0
        //    string runFileindex = this.m_workPath + "tasks\\taskrun.xml";

        //    oTaskRun tr = new oTaskRun(this.m_workPath);
        //    tr.ResetTaskRun(this.TaskID, m_TaskData.UrlCount);
        //    tr.Dispose();
        //    tr = null;

        //    //cXmlIO cxml = new cXmlIO(runFileindex);
        //    //cxml = new cXmlIO(runFileindex);

        //    ////��ԭ������Ҫ��ʵ����Ҫ�ɼ�����ַ������ʼ��ΪUrlCount
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

        //        //����ɼ��ص�ַ�Ƿ���Ҫ����
        //        //�������ַ�ĵ�������
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

        //            //�������ַ�Ķ�ҳ�ɼ�����
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
        //        //���������������Ϊ�п��������ļ������ڡ�
        //    }

        //    //�п��������ļ������ڣ�������Ҫ�ж��Ƿ�Ϊ��
        //    if (cxml1 != null)
        //    {
        //        cxml1.DeleteNode("WebLinks");
        //        cxml1.InsertElement("Task", "WebLinks", tXml);
        //        cxml1.Save();
        //    }
        //    cxml1 = null;

        //    //ɾ����ʱ�洢�Ĳɼ�����xml�ļ�
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

        /// ������������
        internal void Remove()
        {
            ResetTaskState();
        }

        //��������
        //public void ResetTask()
        //{
        //    ResetTaskData();
        //}

        #endregion

        #region ���з���

        /// �����л�������д���ļ�
        public void WriteToFile()
        {

        }


        #endregion

        #region  ��Ӧ�ֽ�ɼ�����(���߳�)�¼�

        /// �����ʼ��,�ɷֽ����񴥷�,
        private void TaskWorkThreadInit(object sender, TaskInitializedEventArgs e)
        {
            cGatherTaskSplit dtc = (cGatherTaskSplit)sender;
            m_TaskData.TaskID  =e.TaskID ;

            if (e_TaskThreadInitialized != null)
            {
                // ������ �����ʼ�� �¼�
                m_TaskManage.EventProxy.AddEvent(delegate()
                {
                    e_TaskThreadInitialized(this, new TaskInitializedEventArgs(m_TaskData.TaskID));
                });
            }
            
        }

        /// �ֽ�ɼ����� �߳���� �¼����� �жϵ��Ƕ����̣߳�ÿ���߳���ɺ�
        /// ����Ҫ������������¼���������������жϣ�������������������
        /// �¼������߳���������Ѿ����
        private void TaskWorkThreadCompleted(object sender, cTaskEventArgs e)
        {

            cGatherTaskSplit dtc = (cGatherTaskSplit)sender;
            if (dtc.UrlCount  == dtc.GatherErrUrlCount +dtc.GatheredUrlCount 
                && dtc.UrlNaviCount ==dtc.GatheredUrlNaviCount+dtc.GatheredErrUrlNaviCount)
            {  
                // ����ɼ����
                onTaskCompleted();
            }
        }

        /// ��ĳ���̲߳ɼ���ɺ󣬻������������¼����м�⣬�����������򴥷�����
        /// ����¼������ڴ��ж�ʱ��Ҫע�⣬�������ɼ�ʧ�ܵ������Ͳɼ�������ȣ����ж�
        /// ����ʧ�ܡ���ÿ�ε��ô��¼�ʱ������Ҫ��һ�μ�⣬��ÿ�����̶߳����һ�飬���Ƿ�
        /// �����Ѿ���ɣ���δ��������¼������̡߳�
        private void onTaskCompleted()
        {
            if (m_TaskData.UrlCount == (m_TaskData.GatheredUrlCount + m_TaskData.GatherErrUrlCount) 
                && m_TaskData.UrlNaviCount ==m_TaskData.GatheredUrlNaviCount+m_TaskData.GatheredErrUrlNaviCount
                && m_State != cGlobalParas.TaskState.Completed)
            {
                if (m_TaskData.UrlCount  == m_TaskData.GatherErrUrlCount)
                {
                    //���ȫ���ɼ��������˴����������Ϊʧ��
                    State = cGlobalParas.TaskState.Failed ;
                }
                else
                {
                    // ����Ϊ���״̬��������������¼�
                    State = cGlobalParas.TaskState.Completed;
                }

                //����ʧ�ܻ��ǳɹ���Ҫ���д������Ĵ���
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

        //��������ִ�����������
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

        /// ���� �ֽ�ɼ����� �����¼�
        private void TaskThreadError(object sender, TaskThreadErrorEventArgs e)
        {
            //���ɼ����������ϵͳ������Ҫ��⵱ǰ�Ƿ���������
            //���û���������磬����Internet����ϵͳֹͣ������ִ��
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

        //������־�¼�
        public void onLog(object sender, cGatherTaskLogArgs e)
        {
            //�ڴ˴��������ֱ����⣬�������־
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

            //�ڴ˴����Ƿ�д��������ݵ���־������
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
        //���������¼�
        public void onGData(object sender, cGatherDataEventArgs e)
        {
            //�����ֱ����⣬�򲻷������ݡ�
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

            //�жϲɼ��˶��������ݺ��Ƿ���Ҫ��ͣ
            if (m_TaskData.GatherCount >0)
            {
                if (m_TaskData.GatherTmpCount > m_TaskData.GatherCount)
                {
                    //��ʼ��ͣ�����̵߳Ĺ���
                    foreach (cGatherTaskSplit dtc in m_list_GatherTaskSplit)
                    {
                        dtc.Pause(this.m_Task.GatherCountPauseInterval);
                    }

                    //����Ϊ��
                    m_TaskData.GatherTmpCount = 0;
                }
            }

        }

       //��һ��ͬ���������м���������
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
                e_Log(this,new cGatherTaskLogArgs (this.TaskID , TaskName, cGlobalParas.LogType.Warning,"�����������û����õ�ֹͣ�ɼ���������������ǿ��ֹͣ��",false ));

            if (e_ShowLogInfo !=null)
                e_ShowLogInfo(this, new ShowInfoEventArgs("����ֹͣ", "����ֹͣ" + this.TaskName ));
        }

        private void onStop(object sender, cTaskEventArgs e )
        {
            bool isStop = true;

            //�ڴ˼���Ƿ������߳�ȫ��ֹͣ�����ȫ��ֹͣ���򴥷�ֹͣ�¼�
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

        #region �¼�

        /// �ɼ���������¼�
        private event EventHandler<cTaskEventArgs> e_TaskCompleted;
        internal event EventHandler<cTaskEventArgs> TaskCompleted
        {
            add { lock (m_eventLock) { e_TaskCompleted += value; } }
            remove { lock (m_eventLock) { e_TaskCompleted -= value; } }
        }

        /// �ɼ�����ɼ�ʧ���¼�
        private event EventHandler<cTaskEventArgs> e_TaskFailed;
        internal event EventHandler<cTaskEventArgs> TaskFailed
        {
            add { lock (m_eventLock) { e_TaskFailed += value; } }
            remove { lock (m_eventLock) { e_TaskFailed -= value; } }
        }

        /// �ɼ�����ʼ�¼�
        private event EventHandler<cTaskEventArgs> e_TaskStarted;
        internal event EventHandler<cTaskEventArgs> TaskStarted
        {
            add { lock (m_eventLock) { e_TaskStarted += value; } }
            remove { lock (m_eventLock) { e_TaskStarted -= value; } }
        }

        /// �ɼ�����ֹͣ�¼�
        private event EventHandler<cTaskEventArgs> e_TaskStopped;
        internal event EventHandler<cTaskEventArgs> TaskStopped
        {
            add { lock (m_eventLock) { e_TaskStopped += value; } }
            remove { lock (m_eventLock) { e_TaskStopped -= value; } }
        }

        /// �ɼ�����ȡ���¼�
        private event EventHandler<cTaskEventArgs> e_TaskAborted;
        internal event EventHandler<cTaskEventArgs> TaskAborted
        {
            add { lock (m_eventLock) { e_TaskAborted += value; } }
            remove { lock (m_eventLock) { e_TaskAborted -= value; } }
        }

        /// �ɼ���������¼�
        private event EventHandler<TaskErrorEventArgs> e_TaskError;
        internal event EventHandler<TaskErrorEventArgs> TaskError
        {
            add { lock (m_eventLock) { e_TaskError += value; } }
            remove { lock (m_eventLock) { e_TaskError -= value; } }
        }

        /// ����״̬����¼�,ÿ������״̬�������ʱ���д���,
        /// ������������¼�,���ڽ���״̬�ĸı�
        private event EventHandler<TaskStateChangedEventArgs> e_TaskStateChanged;
        internal event EventHandler<TaskStateChangedEventArgs> TaskStateChanged
        {
            add { lock (m_eventLock) { e_TaskStateChanged += value; } }
            remove { lock (m_eventLock) { e_TaskStateChanged -= value; } }
        }


        /// �ɼ�����ֽ��ʼ������¼�
        private event EventHandler<TaskInitializedEventArgs> e_TaskThreadInitialized;
        internal event EventHandler<TaskInitializedEventArgs> TaskThreadInitialized
        {
            add { lock (m_eventLock) { e_TaskThreadInitialized += value; } }
            remove { lock (m_eventLock) { e_TaskThreadInitialized -= value; } }
        }

        /// <summary>
        /// �ɼ���־�¼�
        /// </summary>
        private event EventHandler<cGatherTaskLogArgs> e_Log;
        internal event EventHandler<cGatherTaskLogArgs> Log
        {
            add { e_Log += value; }
            remove { e_Log -= value; }
        }

        /// <summary>
        /// ������ַʱ��
        /// </summary>
        private event EventHandler<cGatherUrlCounterArgs> e_GUrlCount;
        internal event EventHandler<cGatherUrlCounterArgs> GUrlCount
        {
            add { lock (m_eventLock) { e_GUrlCount += value; } }
            remove { lock (m_eventLock) { e_GUrlCount -= value; } }
        }


        /// <summary>
        /// ������ִ��ʱ����־�¼�
        /// </summary>
        private event EventHandler<cRunTaskLogArgs> e_RunTaskLog;
        internal event EventHandler<cRunTaskLogArgs> RunTaskLog
        {
            add { e_RunTaskLog += value; }
            remove { e_RunTaskLog -= value; }
        }

        /// <summary>
        /// �ɼ������¼�
        /// </summary>
        private event EventHandler<cGatherDataEventArgs> e_GData;
        internal event EventHandler<cGatherDataEventArgs> GData
        {
            add { e_GData += value; }
            remove { e_GData -= value; }
        }

        /// <summary>
        /// ����һ��ִ�������������¼���������Ӧ������ִ�����������ʱ
        /// �Ĵ���
        /// </summary>
        private event EventHandler<cRunTaskEventArgs> e_RunTask;
        internal event EventHandler<cRunTaskEventArgs> RunTask
        {
            add { lock (m_eventLock) { e_RunTask += value; } }
            remove { lock (m_eventLock) { e_RunTask -= value; } }
        }

        /// <summary>
        /// ����������ͼƬ��ϵͳ��Ϣ������Ϣ
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

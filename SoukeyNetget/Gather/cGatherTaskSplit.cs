using System;
using System.Collections.Generic;
using System.Text;
using System.Threading ;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using SoukeyNetget.Task;

///���ܣ��ɼ����� �ֽ���������
///���ʱ�䣺2009-6-1
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Gather
{

    //���ǲɼ��������С��Ԫ,�����һ���ɼ��������,��һ��URL��ַ����
    //�����ж��������ǧ��URL��ַ,Ϊ���������,��URl�ɼ������˶��̵߳�
    //����ʽ,�������һ������Ҫ���в��,��ֵ������Ǵ������Ƶ��߳�
    //��,������ʼִ��.������Ҫ����ִ����ôһ����С��Ԫ������.

    public class cGatherTaskSplit : IDisposable 
    {

        private delegate void work();
        
        private Thread m_Thread;
        private bool m_ThreadRunning = false;

        private string RegexNextPage = "";

        //����һ������ֵ���жϣ�������ݲɼ��ǲ�����ֱ�����ķ�ʽ
        //�ж����ݱ��Ƿ���Ҫ�½�������½������ڵ�һ������ʱ�������±�
        //�ɹ��󣬲��޸Ĵ�ʱΪfalse������Ѿ����ڴ˱�Ҳ���޸�Ϊfalse
        private bool IsNewTable = true;

        #region �����࣬����ʼ���������

        ///�޸��˹��캯��
        public cGatherTaskSplit()
        {
            //m_TaskManage = TaskManage;
            //m_TaskID = TaskID;
            //m_gStartPos = StartPos;
            //m_gEndPos = EndPos;
            //m_SavePath = sPath;
            //m_Cookie = strCookie;
            //m_WebCode = webCode;
            //m_IsUrlEncode = IsUrlEncode;
            //m_UrlEncode = UrlEncode;
            m_TaskSplitData = new cTaskSplitData ();
            m_ThreadState = cGlobalParas.GatherThreadState.Stopped;

            //m_TaskType = TaskType;

            m_GatherData = new DataTable();
        }

        public void UpdateCookie(string cookie)
        {
            m_Cookie = cookie;
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

            Dispose(false);
        }

        #endregion

        /// <summary>
        /// �¼� �߳�ͬ����
        /// </summary>
        private readonly Object m_eventLock = new Object();
        /// <summary>
        /// ���� �߳�ͬ����
        /// </summary>
        private readonly Object m_mstreamLock = new Object();

        #region ��Ҫ��ʼ���������

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

        private Int64 m_TaskID;
        public Int64 TaskID
        {
            get { return m_TaskID; }
            set { m_TaskID = value; }
        }

        private string m_gStartPos;
        public string StartPos
        {
            get { return m_gStartPos; }
            set { m_gStartPos = value; }
        }

        private string m_gEndPos;
        public string EndPos
        {
            get { return m_gEndPos; }
            set { m_gEndPos = value; }
        }
        private string m_Cookie;
        public string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }
        private cTaskSplitData m_TaskSplitData;
        public cTaskSplitData TaskSplitData
        {
            get { return m_TaskSplitData; }
            set { m_TaskSplitData = value; }
        }

        private cGlobalParas.TaskRunType m_TaskRunType;
        public cGlobalParas.TaskRunType TaskRunType
        {
            get { return m_TaskRunType; }
            set { m_TaskRunType = value; }
        }

        private cGlobalParas.TaskType m_TaskType;
        public cGlobalParas.TaskType TaskType
        {
            get { return m_TaskType; }
            set { m_TaskType = value; }
        }

        //�����ɼ��жϵ�����
        //private cGlobalParas.IncrByCondition m_IncrByCondition;
        //public cGlobalParas.IncrByCondition IncrByCondition
        //{
        //    get { return m_IncrByCondition; }
        //    set { m_IncrByCondition = value; }
        //}

        private cGlobalParas.WebCode m_WebCode;
        public cGlobalParas.WebCode WebCode
        {
            get { return m_WebCode; }
            set { m_WebCode = value; }
        }

        private bool m_IsUrlEncode;
        public bool IsUrlEncode
        {
            get { return m_IsUrlEncode; }
            set { m_IsUrlEncode = value; }
        }

        private string m_UrlEncode;
        public string UrlEncode
        {
            get { return m_UrlEncode; }
            set { m_UrlEncode = value; }
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

        private string m_SavePath;
        public string SavePath
        {
            get { return m_SavePath; }
            set { m_SavePath = value; }
        }

        private int m_AgainNumber;
        public int AgainNumber
        {
            get { return m_AgainNumber; }
            set { m_AgainNumber = value; }
        }

        private bool m_Ignore404;
        public bool Ignore404
        {
            get { return m_Ignore404; }
            set { m_Ignore404 = value; }
        }

        private bool m_IsErrorLog;
        public bool IsErrorLog
        {
            get { return m_IsErrorLog; }
            set { m_IsErrorLog = value; }
        }

        //��δ��
        private bool m_IsDelRepRow;
        public bool IsDelRepRow
        {
            get { return m_IsDelRepRow; }
            set { m_IsDelRepRow = value; }
        }

        private bool m_IsExportGUrl;
        public bool IsExportGUrl
        {
            get { return m_IsExportGUrl; }
            set { m_IsExportGUrl = value; }
        }

        private bool m_IsExportGDateTime;
        public bool IsExportGDateTime
        {
            get { return m_IsExportGDateTime; }
            set { m_IsExportGDateTime = value; }
        }

        private cGlobalParas.PublishType m_PublishType;
        public cGlobalParas.PublishType PublishType
        {
            get { return m_PublishType; }
            set { m_PublishType = value; }
        }

        private string m_DataSource;
        public string DataSource
        {
            get { return m_DataSource; }
            set { m_DataSource = value; }
        }

        private string m_TableName;
        public string TableName
        {
            get { return m_TableName; }
            set { m_TableName = value; }
        }

        private string m_InsertSql;
        public string InsertSql
        {
            get { return m_InsertSql; }
            set { m_InsertSql = value; }
        }

        //���ݴ���Ĳɼ������ж��Զ���ҳ����ַ�Ƿ���Ҫ�������ݺϲ�
        private bool m_IsMergeData;
        public bool IsMergeData
        {
            get { return m_IsMergeData; }
            set { m_IsMergeData = value; }
        }

        //�ɼ������ʱ
        private int m_GIntervalTime;
        public int GIntervalTime
        {
            get { return m_GIntervalTime; }
            set { m_GIntervalTime = value; }
        }

        //����1.8���ӣ��ж��Ƿ���Ҫ�����Զ����Header
        private bool m_IsCustomHeader;
        public bool IsCustomHeader
        {
            get { return m_IsCustomHeader; }
            set { m_IsCustomHeader = value; }
        }

        private bool m_IsPublishHeader;
        public bool IsPublishHeader
        {
            get { return m_IsPublishHeader; }
            set { m_IsPublishHeader = value; }
        }

        private List<cHeader> m_Headers;
        public List<cHeader> Headers
        {
            get { return m_Headers; }
            set { m_Headers = value; }
        }

        #endregion

        #region ��������״̬����

        /// <summary>
        /// �ֽ�ɼ������Ƿ��Ѿ����
        /// </summary>
        public bool IsCompleted
        {
            //get { return m_TaskSplitData.GatheredUrlCount == m_TaskSplitData.UrlCount ; }
            get { return m_TaskSplitData.UrlCount > 1 && m_TaskSplitData.GatheredUrlCount + m_TaskSplitData.GatheredErrUrlCount  == m_TaskSplitData.TrueUrlCount; }
        }
        /// <summary>
        /// �ֽ�Ĳɼ������Ƿ��Ѿ��ɼ����
        /// </summary>
        public bool IsGathered
        {
            get { return m_TaskSplitData.UrlCount > 0 && m_TaskSplitData.GatheredUrlCount +m_TaskSplitData.GatheredErrUrlCount == m_TaskSplitData.TrueUrlCount ; }
        }
        /// <summary>
        /// ��ȡһ��״ֵ̬��ʾ��ǰ�߳��Ƿ��ںϷ�����״̬ [���߳��ڲ�ʹ��]
        /// </summary>
        private bool IsCurrentRunning
        {
            get { return ThreadState  ==cGlobalParas.GatherThreadState.Started  && Thread.CurrentThread.Equals(m_Thread); }
        }

        /// <summary>
        /// ��ȡ��ǰ�߳��Ƿ��Ѿ�ֹͣ��ע��IsThreadRunningֻ��һ����־
        /// ������ȷ���̵߳�״̬��ֻ�Ǹ����߳���Ҫֹͣ��
        /// IsStop�������ⲿ������˷ֽ������Ƿ�ֹͣʹ��
        /// </summary>
        public bool IsStop
        {
            get { return m_ThreadRunning ==false && this.IsCurrentRunning==false;  }
        }
        

        /// <summary>
        /// ��ǰ��Ĺ����߳�
        /// </summary>
        internal Thread WorkThread
        {
            get { return m_Thread; }
        }

        /// <summary>
        /// ��ȡ��ǰ�̵߳�ִ��״̬
        /// </summary>
        internal bool IsThreadAlive
        {
            get { return m_Thread != null && m_Thread.IsAlive; }
        }

        //public cTaskSplitData TaskSplitData
        //{
        //    get { return m_TaskSplitData; }
        //}
        /// <summary>
        /// �ֽ����������Ƿ��ѳ�ʼ��
        /// </summary>
        //public bool IsDataInitialized
        //{
        //    get { return m_IsDataInitialized; }
        //}

        /// <summary>
        /// ��ʼ�ɼ���ҳ��ַ������
        /// </summary>
        public int BeginIndex
        {
            get { return m_TaskSplitData.BeginIndex; }
        }

        /// <summary>
        /// �����ɼ���ҳ��ַ������
        /// </summary>
        public int EndIndex
        {
            get { return m_TaskSplitData.EndIndex ; }
        }

        /// <summary>
        /// ��ǰ���ڲɼ���ַ������
        /// </summary>
        public int CurIndex
        {
            get { return m_TaskSplitData.CurIndex; }
        }

        /// <summary>
        /// ��ǰ�ɼ���ַ
        /// </summary>
        public string CurUrl
        {
            get { return m_TaskSplitData.CurUrl; }
        }

        /// <summary>
        /// �Ѳɼ���ַ����
        /// </summary>
        public int GatheredUrlCount
        {
            get { return m_TaskSplitData.GatheredUrlCount; }
        }

        public int GatheredTrueUrlCount
        {
            get { return m_TaskSplitData.GatheredTrueUrlCount; }
        }

        /// <summary>
        /// �Ѳɼ����������ַ����
        /// </summary>
        public int GatherErrUrlCount
        {
            get { return m_TaskSplitData.GatheredErrUrlCount; }
        }

        public int GatheredTrueErrUrlCount
        {
            get { return m_TaskSplitData.GatheredTrueErrUrlCount; }
        }

        /// <summary>
        /// һ����Ҫ�ɼ�����ַ����
        /// </summary>
        public int UrlCount
        {
            get { return m_TaskSplitData.UrlCount; }
        }

        public int TrueUrlCount
        {
            get { return m_TaskSplitData.TrueUrlCount; }
        }

        #endregion


        #region �¼�
        /// <summary>
        /// �ɼ������ʼ���¼�
        /// </summary>
        private event EventHandler<TaskInitializedEventArgs> e_TaskInit;
        internal event EventHandler<TaskInitializedEventArgs> TaskInit
        {
            add { lock (m_eventLock) { e_TaskInit += value; } }
            remove { lock (m_eventLock) { e_TaskInit -= value; } }
        }

        /// <summary>
        /// �ֽ�����ɼ���ʼ�¼�
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Started;
        internal event EventHandler<cTaskEventArgs> Started
        {
            add { lock (m_eventLock) { e_Started += value; } }
            remove { lock (m_eventLock) { e_Started -= value; } }
        }

        /// <summary>
        /// �ֽ�����ֹͣ�¼�
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Stopped;
        internal event EventHandler<cTaskEventArgs> Stopped
        {
            add { lock (m_eventLock) { e_Stopped += value; } }
            remove { lock (m_eventLock) { e_Stopped -= value; } }
        }

        /// <summary>
        /// �ֽ�����ɼ�����¼�
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Completed;
        internal event EventHandler<cTaskEventArgs> Completed
        {
            add { lock (m_eventLock) { e_Completed += value; } }
            remove { lock (m_eventLock) { e_Completed -= value; } }
        }

        /// <summary>
        /// �ֽ�����ɼ������¼�
        /// </summary>       
        private event EventHandler<TaskThreadErrorEventArgs> e_Error;
        internal event EventHandler<TaskThreadErrorEventArgs> Error
        {
            add { lock (m_eventLock) { e_Error += value; } }
            remove { lock (m_eventLock) { e_Error -= value; } }
        }

        /// <summary>
        ///  �ֽ�����ɼ�ʧ���¼�
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Failed;
        internal event EventHandler<cTaskEventArgs> Failed
        {
            add { lock (m_eventLock) { e_Failed += value; } }
            remove { lock (m_eventLock) { e_Failed -= value; } }
        }

        /// <summary>
        /// д��־�¼�
        /// </summary>
        private event EventHandler<cGatherTaskLogArgs> e_Log;
        internal event EventHandler<cGatherTaskLogArgs> Log
        {
            add { lock (m_eventLock) { e_Log += value; } }
            remove { lock (m_eventLock) { e_Log -= value; } }
        }
        
        /// <summary>
        /// ���زɼ������¼�
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
        #endregion

        #region �¼���������
        //�¼�����������ͨ���ı�����״̬����ɵ�
        //ͨ����������״̬�ı�����������¼��������շ���������
        //������Ȩ��������

        /// <summary>
        /// ��ȡ��ǰ�߳�״̬
        /// </summary>    
        private cGlobalParas.GatherThreadState m_ThreadState;

        /// <summary>
        /// ����/��ȡ �߳�״̬ �����ڲ�ʹ�ã������¼���
        /// </summary>
        protected cGlobalParas.GatherThreadState ThreadState
        {
            get { return m_ThreadState; }
            set
            {
                m_ThreadState = value;
                // ע�⣬�����漰�߳�״̬������¼����ڴ˴���
                switch (m_ThreadState)
                {
                    case cGlobalParas.GatherThreadState.Completed:
                        if (e_Completed != null)
                        {
                            // ������ �ɼ�������� �¼�
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
                            // ������ �߳�ʧ�� �¼�
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
                            // ���� �߳̿�ʼ �¼�
                            e_Started(this, new cTaskEventArgs());
                        }
                        break;
                    case cGlobalParas.GatherThreadState.Stopped:
                        if (e_Stopped != null)
                        {
                            // ���� �߳�ֹͣ �¼�
                            e_Stopped(this, new cTaskEventArgs());
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// ��������������󣬲���ֹͣ�����������������
        /// ����Ҫ������Ϣ�����û���һ����ַ�ɼ�����
        /// </summary>
        /// <param name="exp"></param>
        private void onError(Exception exp)
        {
            if (this.IsCurrentRunning)
            {
                // ������Stopped�¼�
                //m_ThreadState = cGlobalParas.GatherThreadState.Stopped;

                if (e_Error != null)
                {
                    // ������ �̴߳��� �¼�
                    m_TaskManage.EventProxy.AddEvent(delegate()
                    {
                        m_ErrorCount++;
                        e_Error(this, new TaskThreadErrorEventArgs(exp));
                    });
                }

                //m_Thread = null;
            }
        }
        #endregion


        #region �߳̿��� ���� ֹͣ ���� ����

        /// <summary>
        /// �߳���
        /// </summary>
        private readonly Object m_threadLock = new Object();

        /// <summary>
        /// ���������߳�
        /// </summary>
        public void Start()
        {
            if (m_ThreadState != cGlobalParas.GatherThreadState.Started && !IsThreadAlive && !IsCompleted)
            {
                lock (m_threadLock)
                {
                    //����ҳ���ȡ���ݵ���������ж��Ƿ���Ҫ�������ݺϲ�
                    for (int i = 0; i < this.m_TaskSplitData.CutFlag.Count; i++)
                    {
                        if (this.m_TaskSplitData.CutFlag[i].IsMergeData ==true )
                        {
                            m_IsMergeData = true;
                            break;
                        }
                    }

                    //ȡ��һҳ��ҳ���������
                    cXmlSConfig cCon = new cXmlSConfig();
                    RegexNextPage = cTool.ShowTrans( cCon.RegexNextPage);
                    cCon = null;

                    //�����߳����б�־����ʶ���߳�����
                    m_ThreadRunning = true; 

                    m_Thread = new Thread(this.ThreadWorkInit);

                    //�����߳�����,���ڵ���ʹ��
                    m_Thread.Name =m_TaskID.ToString() + "-" + m_TaskSplitData.BeginIndex.ToString ();

                    m_Thread.Start();
                    m_ThreadState = cGlobalParas.GatherThreadState.Started;
                }
            }
        }

        /// <summary>
        /// �������������߳�
        /// </summary>
        public void ReStart()
        {   // �������߳������
            Stop();
            Start();
        }

        /// <summary>
        /// ֹͣ��ǰ�߳�
        /// </summary>
        public void Stop()
        {   
            //�������߳������
            //����ֹͣ�̱߳�־���߳�ֹֻͣ�ܵȴ�һ����ַ�ɼ���ɺ�ֹͣ������
            //ǿ���жϣ�����ᶪʧ����

            ///ע�⣺�ڴ�ֻ�Ǵ���һ����ǣ��̲߳�û����������
            m_ThreadRunning = false;


            if (m_ThreadState == cGlobalParas.GatherThreadState.Started && IsThreadAlive)
            {
                lock (m_threadLock)
                {

                    //��ʼ����Ƿ������̶߳�����ɻ��˳�
                    //bool isStop = false;

                    //while (!isStop)
                    //{
                    //    isStop = true;

                    //    if (m_ThreadState == cGlobalParas.GatherThreadState.Started && IsThreadAlive)
                    //        isStop = false;

                        
                    //}
                    
                    m_Thread = null;
                    if (m_ThreadState == cGlobalParas.GatherThreadState.Started)
                    {
                        m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
                    }
                }
            }
            else
            {

                m_Thread = null;
                if (m_ThreadState == cGlobalParas.GatherThreadState.Started)
                {
                    m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
                }
            }

            m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
        }

        public void Abort()
        {
            m_ThreadRunning = false;

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
        /// �����߳̿�Ϊδ��ʼ��״̬
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

        #endregion

        #region �����̴߳����ɼ���ҳ���ݣ� ִ��һ���ɼ��ֽ�����

        /// <summary>
        /// �ɼ���ʼ���߳�
        /// </summary>
        private void ThreadWorkInit()
        {
            if (!m_IsInitialized)
            {
                ///����ǰ�Ĵ���ʽ��ֻҪ����������������Ѿ���ʼ����������Ϣ
                ///������һ�����δ���������Ǵ��е�������ַ��������ַ��Ҫ����ʵ��
                ///�Ľ�������ſ��Ի����������Ҫ�ɼ����ݵ���ַ����ô����Ҫ����Щʵʱ
                ///������������ַ�ٴθ����߳������������֣�������Ҫ���¶��������
                ///��ʼ�������������ǰδ���д����ر�����
                e_TaskInit(this, new TaskInitializedEventArgs(m_TaskID));
               
            }
            else if (GatheredUrlCount !=TrueUrlCount )
            {
                ThreadWork();
            }
        }

        /// <summary>
        /// �ɼ�����
        /// </summary>
        private void ThreadWork()
        {
            //cGatherWeb gWeb = new cGatherWeb();

            //gWeb.CutFlag =m_TaskSplitData.CutFlag ;

            bool IsSucceed = false;

            for (int i = 0; i < m_TaskSplitData.Weblink.Count; i++)
            {
                if (m_ThreadRunning == true)
                {
                    switch (m_TaskSplitData.Weblink[i].IsGathered)
                    {
                        case (int) cGlobalParas.UrlGatherResult.UnGather:

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "���ڲɼ���" + m_TaskSplitData.Weblink[i].Weblink + "\n",this.IsErrorLog ));

                            //�жϴ���ַ�Ƿ�Ϊ������ַ������ǵ�����ַ����Ҫ���Ƚ���Ҫ�ɼ�����ַ��ȡ����
                            //Ȼ����о�����ַ�Ĳɼ�
                            if (m_TaskSplitData.Weblink[i].IsNavigation == true)
                            {
                                IsSucceed = GatherNavigationUrl(m_TaskSplitData.Weblink[i].Weblink, m_TaskSplitData.Weblink[i].NavigRules , m_TaskSplitData.Weblink[i].IsNextpage, m_TaskSplitData.Weblink[i].NextPageRule,i);
                            }
                            else
                            {
                                //�ǵ���ҳ �ɼ�ҳ ��������Ĭ��Ϊ��0  ͬʱ���ݽ��ȵĺϲ�����ҲΪnull
                                IsSucceed=GatherSingleUrl(m_TaskSplitData.Weblink[i].Weblink,0, m_TaskSplitData.Weblink[i].IsNextpage, m_TaskSplitData.Weblink[i].NextPageRule,i,null);
                            }

                            //����ɼ�����������ֱ�ӵ�����onError���д�����
                            //�����ڴ��е�������ַ��һ�βɼ�����Զ����ַ�����ϵͳ
                            //ֹͣ���񣬶��ڶ����ַ�Ĳɼ��Ǿ��ǲɼ�����δ�ɹ�����Ȼ���ɹ�
                            //����Ҫ����GatheredUrlCount����������
                            if (IsSucceed == true)
                            {
                                //ÿ�ɼ����һ��Url������Ҫ�޸ĵ�ǰCurIndex��ֵ����ʾϵͳ�������У���
                                //����ȷ���˷ֽ������Ƿ��Ѿ����,���ұ�ʾ����ַ�Ѿ��ɼ����
                                m_TaskSplitData.CurIndex++;
                                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.UrlCountAdd, 0));


                                //���������������ɼ���־��Զ��δ�ɼ���ɣ���ʾ�´�������ʱ����Ҫ
                                //���вɼ�
                                m_TaskSplitData.Weblink[i].IsGathered = (int)cGlobalParas.UrlGatherResult.Succeed;

                                m_TaskSplitData.GatheredUrlCount++;
                            }
                            else
                            {
                                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd, 0));
                            }

                            break;

                        case (int)cGlobalParas.UrlGatherResult.Succeed:
                            m_TaskSplitData.CurIndex++;
                            m_TaskSplitData.GatheredUrlCount++;
                            m_TaskSplitData.GatheredTrueUrlCount++;
                            e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.UrlCountAdd, 0));
                            break;
                        case (int)cGlobalParas.UrlGatherResult.Error:
                            m_TaskSplitData.CurIndex++;
                            m_TaskSplitData.GatheredErrUrlCount++;
                            m_TaskSplitData.GatheredTrueErrUrlCount++;
                            e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd, 0));
                            break;
                        case (int)cGlobalParas.UrlGatherResult.Gathered:
                            m_TaskSplitData.CurIndex++;
                            m_TaskSplitData.GatheredUrlCount++;
                            m_TaskSplitData.GatheredTrueUrlCount++;
                            e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.UrlCountAdd, 0));
                            break;
                    }
                }
                else if (m_ThreadRunning == false)
                {
                    //��ʾ�߳���Ҫ��ֹ���˳�forѭ��
                    break;
                }


            }
           

            //ÿ�ɼ����һ����ַ��Ҫ�жϵ�ǰ�߳��Ƿ��Ѿ�����
            //��Ҫ�ж��Ѳɼ�����ַ�Ͳɼ������������ַ
            if (UrlCount == GatheredUrlCount + GatherErrUrlCount && UrlCount != GatherErrUrlCount)
            {
                ThreadState = cGlobalParas.GatherThreadState.Completed;
            }
            else if (UrlCount == GatherErrUrlCount)
            {
                //��ʾ�ɼ�ʧ�ܣ�������һ��������ԣ�һ���̵߳���ȫʧ�ܲ�������
                //����Ҳ�ɼ���ȫʧ�ܣ���������İ�ȫʧ����Ҫ������ɼ������ж�
                //���ԣ��ڴ˻��Ƿ����߳��������ʱ�䣬����ʧ��ʱ�䡣
                ThreadState = cGlobalParas.GatherThreadState.Completed;
            }
            else if (UrlCount<GatheredUrlCount + GatherErrUrlCount)
            {
                ThreadState = cGlobalParas.GatherThreadState.Completed;
            }
            else
            {
                ThreadState = cGlobalParas.GatherThreadState.Stopped;
            }

            m_ThreadRunning = false;

        }

        //���ڲɼ�һ����ҳ�����ݣ�������������������Ҫ����ҳ
        private bool GatherSingleUrl(string Url,int Level,bool IsNext,string NextRule,int index,DataRow tRow)
        {
            cGatherWeb gWeb = new cGatherWeb();
            DataTable tmpData;
            DataTable tmpMergeData;
            

            //gWeb.CutFlag = m_TaskSplitData.CutFlag;

            bool IsAjax = false;

            if (m_TaskType == cGlobalParas.TaskType.AjaxHtmlByUrl)
                IsAjax = true;

            //�ڴ˴���Url���������
            if (m_IsUrlEncode == true)
            {
                Url = cTool.UrlEncode(Url, (cGlobalParas.WebCode)int.Parse(m_UrlEncode));
            }

            //�ڴ˴����Ƿ���Ҫ����Base64����ĵ�����
            if (Regex.IsMatch(Url, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
            {

                Match s = Regex.Match(Url, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
                string sBase64 = s.Groups[0].Value.ToString();
                sBase64 = cTool.Base64Encoding(sBase64);

                //��base64���벿�ֽ���url�滻
                Regex.Replace(Url, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            }

            string NextUrl = Url;
            string Old_Url = NextUrl;

            try
            {
                if (IsNext)
                {
                    #region ���������һҳ������ʼ������һҳ����Ľ��������ݲɼ�

                    tmpMergeData = new DataTable();

                    do
                    {
                        if (m_ThreadRunning == true)
                        {
                            Url = NextUrl;
                            Old_Url = NextUrl;

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "���ڲɼ���" + Url + "\n", this.IsErrorLog));

                            tmpData = GetGatherData(Url, Level, m_WebCode, m_Cookie, m_gStartPos, m_gEndPos, m_SavePath, IsAjax);

                            ///�������� ��ע�ͣ����Կ��Ƿ��������
                            //if (tmpData != null)
                            //{
                            //    m_GatherData.Merge(tmpData);
                            //}

                            ///ÿ�βɼ�������ݺ󣬶���Ҫ�ж��Ƿ�������ݺϲ�
                            if (m_IsMergeData == true)
                            {
                                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "��ҳ������Ҫ���кϲ����ȴ���һҳ���ݲɼ�\n", this.IsErrorLog));
                                tmpMergeData.Merge(tmpData);
                            }
                            else
                            {

                                //������־���ɼ����ݵ��¼�
                                if (tmpData == null || tmpData.Rows.Count == 0)
                                {

                                }
                                else
                                {
                                    //�ϲ��ϲ㴫����������
                                    if (tRow != null && tmpData != null)
                                    {
                                        //�ϲ����ݵ�������
                                        tmpData = MergeDataTable(tRow, tmpData);
                                    }


                                    e_GData(this, new cGatherDataEventArgs(m_TaskID, tmpData));
                                }
                            }

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "�ɼ���ɣ�" + Url + "\n", this.IsErrorLog));


                            e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "��ʼ������һҳ�����ȡ��һҳ��ַ\n", this.IsErrorLog));


                            string webSource = gWeb.GetHtml(Url, m_WebCode, m_Cookie, "", "", true, IsAjax);


                            //string NRule = "((?<=href=[\'|\"])\\S[^#+$<>\\s]*(?=[\'|\"]))[^<]*" + "(?<=" + NextRule + ")";
                            
                            string NRule = RegexNextPage + "(?<=" + NextRule + ")";
                            Match charSetMatch = Regex.Match(webSource, NRule, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            string strNext = charSetMatch.Groups[1].Value;

                            if (strNext != "")
                            {
                                //�жϻ�ȡ�ĵ�ַ�Ƿ�Ϊ��Ե�ַ
                                if (strNext.Substring(0, 1) == "/")
                                {
                                    string PreUrl = Url;
                                    PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                                    PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));
                                    PreUrl = "http://" + PreUrl;
                                    strNext = PreUrl + strNext;
                                }
                                else if (strNext.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    NextUrl = strNext;
                                }
                                else if (strNext.StartsWith("?", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Match aa = Regex.Match(Url, @".*(?=\?)");
                                    string PreUrl = aa.Groups[0].Value.ToString();
                                    if (PreUrl =="" )
                                        strNext = Url + strNext;
                                    else
                                        strNext = PreUrl + strNext;
                                }
                                else
                                {
                                    Match aa = Regex.Match(Url, ".*/");
                                    string PreUrl = aa.Groups[0].Value.ToString();
                                    strNext = PreUrl + strNext;
                                }

                                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "��һҳ��ַ��ȡ�ɹ���" + NextUrl + "\n", this.IsErrorLog));
                            }
                            else
                            {
                                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "�Ѿ�������ҳ��" + NextUrl + "\n", this.IsErrorLog));
                            }
                            NextUrl = strNext;

                            //���µ�ַ����Ϊ���Զ���ҳ�����Բɼ������޷���֪��ҳ�������������Ҫ
                            //�ڴ˽��и���
                            m_TaskSplitData.Weblink[index].NextPageUrl = NextUrl;

                        }
                        else if (m_ThreadRunning == false)
                        {
                            //��ʶҪ����ֹ�̣߳�ֹͣ�����˳�doѭ����ǰ��������
                            if (NextUrl == "" || Old_Url == NextUrl)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        
                        
                    }
                    while (NextUrl != "" && Old_Url != NextUrl);

                    //�ж��Ƿ�Ϊ�ϲ����ݣ�����ǣ��򽫺ϲ��������Ϸ���
                    if (m_IsMergeData == true)
                    {
                        e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "��ʼ�������ݺϲ�\n", this.IsErrorLog));
                        tmpMergeData = MergeData(tmpMergeData);


                        if (tRow  != null && tmpMergeData != null)
                        {
                            //�ϲ����ݵ�������
                            tmpMergeData = MergeDataTable(tRow, tmpMergeData);
                        }


                        e_GData(this, new cGatherDataEventArgs(m_TaskID, tmpMergeData));
                        e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info,"���ݺϲ��ɹ�\n", this.IsErrorLog));
                        tmpMergeData = null;
                    }

                    #endregion

                }
                else
                {

                    tmpData = GetGatherData(Url, Level, m_WebCode, m_Cookie, m_gStartPos, m_gEndPos, m_SavePath, IsAjax);

                    ///�������� ��ע�ͣ����Կ��Ƿ��������
                    //if (tmpData != null)
                    //{
                    //    m_GatherData.Merge(tmpData);
                    //}

                    //������־���ɼ����ݵ��¼�
                    if (tmpData == null || tmpData.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        if (tRow != null && tmpData != null)
                        {
                            //�ϲ����ݵ�������
                            tmpData = MergeDataTable(tRow, tmpData);
                        }


                        e_GData(this, new cGatherDataEventArgs(m_TaskID, tmpData));
                    }
                    e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "�ɼ���ɣ�" + Url + "\n", this.IsErrorLog));
                }
                

                //�����ɼ���ַ�����¼�
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));

                m_TaskSplitData.GatheredTrueUrlCount++;

            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, Url + "�ɼ���������" + ex.Message + "\n", this.IsErrorLog));
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd, 0));
                m_TaskSplitData.GatheredTrueErrUrlCount++;
                m_TaskSplitData.GatheredErrUrlCount++;
                onError(ex);
                return false;
            }

            gWeb = null;
            tmpData = null;

            return true;
        }

        ///���ǲɼ����е����������ַ���ݵ����
        ///���������Ϊ���ࣺһ����һҳ�ĵ������򣻶���ҳ�浼����
        ///�˷��������ַ����Ҫ������һҳ�Ĺ���Ȼ�����ParseGatherNavigationUrl
        ///����ҳ�浼��������
        private bool GatherNavigationUrl(string Url, List<Task.cNavigRule> nRules, bool IsNext, string NextRule,int index)
        {
            cGatherWeb gWeb = new cGatherWeb();
            //gWeb.CutFlag = m_TaskSplitData.CutFlag;
            
            bool IsSucceed = false;

            //�ڴ˴�����ַ�������
            if (m_IsUrlEncode == true)
            {
                Url = cTool.UrlEncode(Url, (cGlobalParas.WebCode)int.Parse(m_UrlEncode));
            }

            //�ڴ˴����Ƿ���Ҫ����Base64����ĵ�����
            if (Regex.IsMatch(Url, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
            {

                Match s = Regex.Match(Url, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
                string sBase64 = s.Groups[0].Value.ToString();
                sBase64 = cTool.Base64Encoding(sBase64);

                //��base64���벿�ֽ���url�滻
                Regex.Replace(Url, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            }

            string NextUrl = Url;
            string Old_Url = NextUrl;

            try
            {

                if (IsNext)
                {
                    #region ������ �Ҿ߱���һҳ�Զ���ҳ�Ĳɼ�
                    do
                    {
                        if (m_ThreadRunning == true)
                        {
                            Url = NextUrl;
                            Old_Url = NextUrl;

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "���ڲɼ���" + Url + "\n", this.IsErrorLog));

                            IsSucceed = ParseGatherNavigationUrl(Url,1,nRules,index,null) ; //, NagRule, IsOppPath);

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "�ɼ���ɣ�" + Url + "\n", this.IsErrorLog));

                            e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "��ʼ������һҳ�����ȡ��һҳ��ַ\n", this.IsErrorLog));

                            bool IsAjax = false;

                            if (m_TaskType == cGlobalParas.TaskType.AjaxHtmlByUrl)
                                IsAjax = true;

                            //��ʼ�ֽ���һҳ��ַ
                            string webSource = gWeb.GetHtml(Url, m_WebCode, m_Cookie, "", "",true,IsAjax );

                            //string NRule = "((?<=href=[\'|\"])\\S[^#+$<>\\s]*(?=[\'|\"]))[^<]*" + "(?<=" + NextRule + ")";

                            string NRule = RegexNextPage + "(?<=" + NextRule + ")";
                            Match charSetMatch = Regex.Match(webSource, NRule, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            string strNext = charSetMatch.Groups[1].Value;

                            if (strNext != "")
                            {
                                //�жϻ�ȡ�ĵ�ַ�Ƿ�Ϊ��Ե�ַ
                                if (strNext.Substring(0, 1) == "/")
                                {
                                    string PreUrl = Url;
                                    PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                                    PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));
                                    PreUrl = "http://" + PreUrl;
                                    strNext = PreUrl + strNext;
                                }
                                else if (strNext.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    NextUrl = strNext;
                                }
                                else if (strNext.StartsWith("?", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    Match aa = Regex.Match(Url, @".*(?=\?)");
                                    string PreUrl = aa.Groups[0].Value.ToString();

                                    if (PreUrl == "")
                                        strNext = Url + strNext;
                                    else
                                        strNext = PreUrl + strNext;

                                    strNext = PreUrl + strNext;
                                }
                                else
                                {
                                    Match aa = Regex.Match(Url, ".*/");
                                    string PreUrl = aa.Groups[0].Value.ToString();
                                    strNext = PreUrl + strNext;
                                }

                                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "��һҳ��ַ��ȡ�ɹ���" + NextUrl + "\n", this.IsErrorLog));
                            }
                            else
                            {
                                e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "�Ѿ�������ҳ" + "\n", this.IsErrorLog));
                            }

                            NextUrl = strNext;

                            //���µ�ַ����Ϊ���Զ���ҳ�����Բɼ������޷���֪��ҳ�������������Ҫ
                            //�ڴ˽��и���
                            m_TaskSplitData.Weblink[index].NextPageUrl  = NextUrl;

                        }
                        else if (m_ThreadRunning == false)
                        {
                            //��ʶҪ����ֹ�̣߳�ֹͣ�����˳�doѭ����ǰ��������
                            if (NextUrl == "" || Old_Url == NextUrl)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                            //break;
                        }

                    }
                    while (NextUrl != "" && Old_Url != NextUrl);

                    #endregion
                }
                else
                {

                    IsSucceed = ParseGatherNavigationUrl(Url,1, nRules,index ,null ); //, NagRule, IsOppPath);
                }
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, Url + "�ɼ���������" + ex.Message + "\n", this.IsErrorLog));
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd, 0));
                m_TaskSplitData.GatheredTrueErrUrlCount++;
                m_TaskSplitData.GatheredErrUrlCount++;
                onError(ex);
                return false;
            }

            gWeb = null;

            return IsSucceed;
        }

        ///���ڲɼ���Ҫ��������ҳ���ڴ˴�����ҳ������2009-10-28�գ������˵��������Զ���ҳ�����
        ///���ԣ��ڴ���Ҫ���еݹ���ã�ԭ�з�ʽ�ǽ�Url�͵������򣨿����Ƕ༶��ͨ��UrlAnalysis������
        ///�ó����е���ַ���������㷨��Ҫ�޸ģ����ּ����е���������ַ��ȡ֮���ٽ��з�ҳ�������һ����ַ
        ///���ֹ��򶼴���������ٽ�����һ����ַ�Ĵ���
        private bool ParseGatherNavigationUrl(string Url,int level, List<Task.cNavigRule> nRules,int index, DataRow tRow)
        {
            //��Ϊ����һ���ݹ���õĺ��������ԣ�������Ҫ�жϵ�ǰ�Ƿ��Ѿ�Ҫ��ֹͣ�̹߳���
            //���ֹͣ�̹߳�������ֹͣ���ã���ʼ����
            if (m_ThreadRunning == false)
                return false;

            Task.cUrlAnalyze u = new Task.cUrlAnalyze();

            List<string> gUrls;                                          //��¼�������ص�Url�б�
            List<cNavigRule> tmpNRules = new List<cNavigRule>();         //����¼��ǰ��������Ĺ��� ��һ�����ϣ���Ϊ�Ƿֲ㵼��������һ�����Ͻ���¼һ��
            cNavigRule tmpNRule = new cNavigRule();                      //��¼��ǰ��������ĵ�������

            //���ֵ�ǰ��������ĵ�������
            for (int i = 0; i < nRules.Count; i++)
            {
                if (level == nRules[i].Level)
                {
                    tmpNRule.Level = 1;
                    tmpNRule.NaviStartPos = nRules[i].NaviStartPos;
                    tmpNRule.NaviEndPos = nRules[i].NaviEndPos;
                    tmpNRule.NavigRule = nRules[i].NavigRule;

                    tmpNRule.IsNaviNextPage = nRules[i].IsNaviNextPage;
                    tmpNRule.NaviNextPage = nRules[i].NaviNextPage;
                    tmpNRule.IsGather = nRules[i].IsGather;
                    tmpNRule.GatherStartPos = nRules[i].GatherStartPos;
                    tmpNRule.GatherEndPos = nRules[i].GatherEndPos;

                    tmpNRules.Add(tmpNRule);
                    break;
                }
            }

            e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, "��ʼ���ݵ��������ȡ��ҳ��ַ����ȴ�......\n�����㼶Ϊ��" + nRules.Count + " �㣬���ڽ��е�" + level + "�㵼��\n", this.IsErrorLog));

            //���ݵ��������ҵ���ַ
            gUrls = u.ParseUrlRule(Url, tmpNRules, m_WebCode, m_Cookie);

            u = null;
            if (gUrls == null || gUrls.Count == 0)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Info, Url + " ��������ʧ�ܣ��п��������ڵ����������ô���Ҳ�п�������������������ɣ�������������ݣ���Ӱ��ϵͳ�����ݵĲɼ�\n", this.IsErrorLog));
                return false;
            }

            e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "�ɹ����ݵ��������ȡ" + gUrls.Count + "����ַ\n", this.IsErrorLog));

            //����ʵ�ʲɼ���ַ���������ǵ���ҳ�棬����ʵ�ʲɼ���ַ���������˱仯
            //ͨ���¼�������������Ĳɼ�����������ͬʱ����������Ĳɼ�����
            //ע�⣬������ʵ�ʲɼ���ַ������������������ַ��������������ֵ������ά�����Ե�ҵ���߼�����

            //ϵͳ���������񵼺��ķֽ��������ʱ���Ѿ��޸�����Ҫ�ɼ���������������ԣ���Ҫ�����������ʵ�ʲɼ���ַ������
            //ͬʱ���败����Ӧ���¼��޸���������Ĳɼ���ַ������
            m_TaskSplitData.TrueUrlCount += gUrls.Count - 1;
            e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.ReIni, gUrls.Count));


            //�ж��Ƿ�Ϊ����ҳ�����������ҳ����ʼ��������ҳ�Ĳɼ�
            //ͬʱ�����������ҳ���Ҿ߱��ɼ�Ҫ��ĵ���ҳ����Ҫ�����ȰѴ˵���ҳ�Ĳɼ�����
            //�ɼ�������Ȼ����Ϊ�������еݹ鴫�ݣ�һֱ���ɼ�ҳ�������յ����ݺϲ�

            cGatherWeb gWeb = new cGatherWeb();
            DataTable tData=new DataTable ();

            //�жϴ˵���ҳ�Ƿ���Ҫ�������ݲɼ�
            if (tmpNRule.IsGather == true)
            {
                bool IsAjax = false;

                if (m_TaskType == cGlobalParas.TaskType.AjaxHtmlByUrl)
                    IsAjax = true;

                tData = new DataTable();

                tData = GetGatherData(Url, level, m_WebCode, m_Cookie, tmpNRule.GatherStartPos, tmpNRule.GatherEndPos, m_SavePath, IsAjax);

                if (tRow != null && tData != null)
                {
                    //�ϲ����ݵ�������
                    tData = MergeDataTable(tRow, tData);
                }

            }

            if (level == nRules.Count)
            {
                GatherParsedUrl(gUrls,level , tmpNRule.IsNaviNextPage, tmpNRule.NaviNextPage,index,tData);
            }
            else
            {
               
                for (int j = 0; j < gUrls.Count; j++)
                {
                    DataRow dr;
                    if (tData != null && tData.Rows.Count != 0)
                    {
                        if (j > tData.Rows.Count && tData != null)
                        {
                            dr = tData.Rows[tData.Rows.Count - 1];
                        }
                        else
                        {
                            dr = tData.Rows[j];
                        }
                    }
                    else
                    {
                        dr = null;
                    }

                    //ѭ�����ã����ж��̵߳���ֹ״̬
                    //�ڴ˴����Զ���ҳ�Ĺ���

                    if (tmpNRule.IsNaviNextPage)
                    {
                        #region �ڴ˴����ҳ�����Ĺ���

                        string NextUrl = gUrls[j].ToString ();
                        string Old_Url = NextUrl;

                        do
                        {
                            
                            if (m_ThreadRunning == true)
                            {
                                Url = NextUrl;
                                Old_Url = NextUrl;

                                ParseGatherNavigationUrl(Url, level + 1, nRules, index, dr);

                                e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "��ʼ������һҳ�����ȡ��һҳ��ַ\n", this.IsErrorLog));

                                bool IsAjax = false;

                                if (m_TaskType == cGlobalParas.TaskType.AjaxHtmlByUrl)
                                    IsAjax = true;

                                string webSource = gWeb.GetHtml(Url, m_WebCode, m_Cookie, "", "", true, IsAjax);

                                string NRule = "((?<=href=[\'|\"])\\S[^#+$<>\\s]*(?=[\'|\"]))[^<]*(?<=" + tmpNRule.NaviNextPage + ")";
                                Match charSetMatch = Regex.Match(webSource, NRule, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                string strNext = charSetMatch.Groups[1].Value;

                                if (strNext != "")
                                {
                                    //�жϻ�ȡ�ĵ�ַ�Ƿ�Ϊ��Ե�ַ
                                    if (strNext.Substring(0, 1) == "/")
                                    {
                                        string PreUrl = Url;
                                        PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                                        PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));
                                        PreUrl = "http://" + PreUrl;
                                        strNext = PreUrl + strNext;
                                    }
                                    else if (strNext.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        NextUrl = strNext;
                                    }
                                    else
                                    {
                                        Match aa = Regex.Match(Url, ".*/");
                                        string PreUrl = aa.Groups[0].Value.ToString();
                                        strNext = PreUrl + strNext;
                                    }

                                    e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "��һҳ��ַ��ȡ�ɹ���" + NextUrl + "\n", this.IsErrorLog));
                                }
                                else
                                {
                                    e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "�Ѿ�������ҳ" + "\n", this.IsErrorLog));
                                }

                                NextUrl = strNext;


                            }
                            else if (m_ThreadRunning == false)
                            {
                               
                                return false;

                            }

                        }
                        while (NextUrl != "" && Old_Url != NextUrl);

                        #endregion
                    }
                    else
                    {
                        if (m_ThreadRunning == true)
                        {
                            ParseGatherNavigationUrl(gUrls[j].ToString(), level + 1, nRules, index, dr);
                        }
                        else if (m_ThreadRunning == false)
                        {
                            return false ;
                        }
                    }
                    dr = null;
                }
            }

            tData=null;

            gUrls = null;

            tmpNRule = null;
            tmpNRules = null;

            return true;
        }

        //���ڲɼ�������ҳ�ֽ��ļ�����ַ�������յ�����ҳ
        private bool GatherParsedUrl(List<string> gUrls,int Level, bool IsNext,string NextPageRule,int index,DataTable tmpData)
        {
            bool IsSucceed = false;

            for (int j = 0; j < gUrls.Count; j++)
            {
                if (m_ThreadRunning == true)
                {
                    try
                    {
                        DataRow dr;
                        if (tmpData != null && tmpData.Rows.Count != 0)
                        {
                            if (j > tmpData.Rows.Count)
                            {
                                dr = tmpData.Rows[tmpData.Rows.Count - 1];
                            }
                            else
                            {
                                dr = tmpData.Rows[j];
                            }
                        }
                        else
                        {
                            dr = null;
                        }


                        //��Ȼ������ҳ�ɼ����Ǵ���ɼ�����ĵ����㼶���ȻΪ0
                        IsSucceed = GatherSingleUrl(gUrls[j].ToString(), 0, IsNext, NextPageRule, index, dr);

                        //�����ɼ���ַ�����¼�
                        e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));
                        m_TaskSplitData.GatheredTrueUrlCount++;

                        dr = null;

                    }
                    catch (System.Exception ex)
                    {
                        e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                        m_TaskSplitData.GatheredTrueErrUrlCount++;
                        onError(ex);
                    }

                }
                else if (m_ThreadRunning == false)
                {
                    //��ʶҪ����ֹ�̣߳�ֹͣ�����˳�forѭ����ǰ��������
                    if (j == gUrls.Count)
                    {
                        //��ʾ���ǲɼ������
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }

            return true;
        }

        #endregion

        //����һ��ͨѶ�Ľӿڷ����������ɼ�����Ĵ���������Ҫ�ɼ�����ҳ�����ô˷���
        //�ɴ˷�������cGatherWeb.GetGatherData�����η�����Ŀ����Ϊ�˿��Դ����������

        private DataTable GetGatherData(string Url,int Level, cGlobalParas.WebCode webCode, string cookie, string startPos, string endPos, string sPath, bool IsAjax)
        {

            //�òɼ��߳�����
            if (m_GIntervalTime != 0)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Warning, "�ɼ�������Ϊ����ÿ�ɹ��ɼ���һ�Σ�ֹͣ" + m_GIntervalTime.ToString() + "����������У�\n", this.IsErrorLog));
                Thread.Sleep(m_GIntervalTime);
            }


            cGatherWeb gWeb = new cGatherWeb();

            //ֻ��iscustomheader=true ���� ispublishheader=false �Ž����Զ����header
            if (this.IsCustomHeader == true && this.IsPublishHeader == false)
            {
                gWeb.IsCustomHeader = true;
                gWeb.Headers = this.Headers;
            }
            else
            {
                gWeb.IsCustomHeader = false;
            }
            
            ///�ڴ˴����ݽ�ȥ�Ĳɼ������Ƿ��ϵ�ǰ�ɼ�����Ĺ��򣬲ɼ�ҳĬ��Ϊ0
            ///������ҳ�Ĳɼ�ʱ����Ҫ���ݵ���ҳ�ļ����ƶ��ɼ�����

            List<cWebpageCutFlag> CutFlags = new List<cWebpageCutFlag>();
            cWebpageCutFlag CutFlag = new cWebpageCutFlag();
            for (int i = 0; i < m_TaskSplitData.CutFlag.Count; i++)
            {
                if (m_TaskSplitData.CutFlag[i].NavLevel ==Level )
                    CutFlags.Add (m_TaskSplitData.CutFlag [i]);
            }

            gWeb.CutFlag = CutFlags;

            //���õ�ǰ�Ĳɼ�����ַ
            m_TaskSplitData.CurUrl = Url;

            DataTable tmpData ;
            int AgainTime = 0;

            GatherAgain:

            try
            {
                tmpData = gWeb.GetGatherData(Url, webCode, cookie, startPos, endPos, m_SavePath, IsAjax, IsExportGUrl, IsExportGDateTime);
            }
            catch (System.Exception ex)
            {
                AgainTime++;
                
                if (AgainTime > m_AgainNumber)
                {
                    if (m_IsErrorLog == true)
                    {
                        //���������־
                    }

                    throw ex;
                }
                else
                {
                    if (m_Ignore404 == true && ex.Message.Contains ("404"))
                    {
                        if (m_IsErrorLog == true)
                        {
                            //���������־
                        }

                        throw ex;
                    }
                    else
                    {
                        e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, "��ַ��" + Url + "���ʷ�����������Ϣ��" + ex.Message + "���ȴ�3������\n", this.IsErrorLog));
                       
                        Thread.Sleep(3000);

                        e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Warning,Url + "���ڽ��е�" + AgainTime + "������\n", this.IsErrorLog));

                        //��������
                        goto GatherAgain;
                    }
                }
            }


            //�ڴ˴����Ƿ�ֱ����⣬�����Ҫ����ֱ����⣬���ڴ˽������ݿ�Ĳ���
            //����󣬲��������ݣ�ֱ�ӽ�tempData��Ϊ��
            //����û�ѡ����ֱ����⣬������ǲɼ�ҳ�����ݣ������ǵ���ҳ������ ���Ա���level=0

            if (this.TaskRunType == cGlobalParas.TaskRunType.OnlySave && tmpData!=null && Level ==0)
            {
                NewTable(tmpData.Columns);
                InsertData(tmpData);
                tmpData = null;
            }

            return tmpData;
        }

        #region �ϲ����ݲ���
        private DataTable MergeData(DataTable tmpData)
        {
            if (tmpData == null || tmpData.Rows.Count == 0)
                return null;

            object oldRow;
            object newRow;
            DataTable d1 = tmpData.Clone();
            object[] mRow;

            d1 = tmpData.Clone();

            oldRow = tmpData.Rows[0].ItemArray.Clone();

            

            for (int i = 0; i<tmpData.Rows.Count; i++)
            {
                newRow = tmpData.Rows[i].ItemArray.Clone();
                mRow = MergeRow(oldRow, newRow);

                DataRow r = d1.NewRow();

                if (mRow != null)
                {
                    oldRow = mRow.Clone();
                    r.ItemArray = ((object[])oldRow);
                    if (d1.Rows.Count > 0)
                    {
                        d1.Rows.Remove(d1.Rows[d1.Rows.Count - 1]);
                        d1.Rows.Add(r);
                    }
                    else
                    {
                        d1.Rows.Add(r);
                    }
                }
                else
                {
                    r.ItemArray =((object[]) oldRow);
                    d1.Rows.Add(r);
                    oldRow = newRow;
                }
            }

            return d1;
        }

        private object[] MergeRow(object row1, object row2)
        {
            object[] oldRow = ((object[])row1);
            object[] newRow = ((object[])row2);
            object[] mergeRow = new object [this.m_TaskSplitData.CutFlag.Count];

            for (int i = 0; i < this.m_TaskSplitData.CutFlag.Count; i++)
            {
                if (this.m_TaskSplitData.CutFlag[i].IsMergeData == false)
                {
                    if (oldRow[i].ToString() == newRow[0].ToString())
                    {
                        mergeRow[i] = oldRow[i].ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    mergeRow[i] = oldRow[i].ToString() + newRow[i].ToString ();
                }
            }
            return mergeRow;
        }
        #endregion

        #region ֱ����� ����
        private void InsertData(DataTable tmpData)
        {
            if (IsNewTable == true)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Error, "���ݱ����ڣ��޷�����ɼ������ݣ��������־��" + "\n", this.IsErrorLog));
                ThreadState = cGlobalParas.GatherThreadState.Failed;
            }

            //�жϴ洢���ݿ�����
            switch (this.PublishType)
            {
                case cGlobalParas.PublishType .PublishAccess :
                    ExportAccess(tmpData);
                    break;
                case cGlobalParas.PublishType .PublishMSSql  :
                    ExportMSSql(tmpData);
                    break;
                case cGlobalParas.PublishType .PublishMySql :
                    ExportMySql(tmpData);
                    break;
            }
        }

        #region �ж��Ƿ����±�
        private void NewTable(DataColumnCollection dColumns)
        {
            //�����жϱ��Ƿ���ڣ��������������н���
            if (IsNewTable == true)
            {
                bool isSucceed = false;

                switch (this.PublishType)
                {
                    case cGlobalParas.PublishType.PublishAccess:
                        isSucceed = NewAccessTable(dColumns);
                        break;
                    case cGlobalParas.PublishType.PublishMSSql:
                        isSucceed = NewMSSqlServerTable(dColumns);
                        break;
                    case cGlobalParas.PublishType.PublishMySql:
                        isSucceed = NewMySqlTable(dColumns);
                        break;
                }

                if (isSucceed == true)
                    IsNewTable = false;
            }
           
        }

        private bool NewAccessTable(DataColumnCollection dColumns)
        {
            bool IsTable = false;

            OleDbConnection conn = new OleDbConnection();
            string connectionstring = this.DataSource;
            conn.ConnectionString = connectionstring;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Error, "����ֱ����⣬���ݿ����ӳ���������Ϣ��" + ex.Message + "���޷�������������" + "\n", this.IsErrorLog));
                return false ;
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[3].ToString() == "TABLE")
                {
                    if (r[2].ToString() == this.TableName)
                    {
                        IsTable = true;
                        break;
                    }
                }

            }

            if (IsTable == false)
            {
                //��Ҫ�����±������±��ʱ�����ado.net�½��еķ�ʽ������������
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.Access, "", dColumns);

                OleDbCommand com = new OleDbCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (System.Data.OleDb.OleDbException ex)
                {
                    if (ex.ErrorCode != -2147217900)
                    {
                        
                        e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, "���ݿ⽨�������󣬴�����Ϣ��" + ex.Message + "\n", this.IsErrorLog));
                        conn.Close();
                        return false ;
                    }
                }

                IsTable = true;

            }

            conn.Close();

            return IsTable;
        }

        private bool NewMSSqlServerTable(DataColumnCollection dColumns)
        {
             bool IsTable = false;

            SqlConnection conn = new SqlConnection();
            string connectionstring = this.DataSource;
            conn.ConnectionString = connectionstring;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, "����ֱ����⣬���ݿ����ӳ���������Ϣ��" + ex.Message  + "���޷�������������" + "\n", this.IsErrorLog));
                return false ;
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[2].ToString()==this.TableName )
                {
                    IsTable = true;
                    break;
                }
            }

            if (IsTable == false)
            {
                //��Ҫ�����±������±��ʱ�����ado.net�½��еķ�ʽ������������
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.MSSqlServer, "", dColumns);

                SqlCommand com = new SqlCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    if (ex.ErrorCode != -2147217900)
                    {
                        e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, "���ݿ⽨�������󣬴�����Ϣ��" + ex.Message + "\n", this.IsErrorLog));
                        conn.Close();
                        return false;
                    }
                }
                IsTable = true;
            }

            conn.Close();

            return IsTable;
        }

        private bool NewMySqlTable(DataColumnCollection dColumns)
        {
            bool IsTable = false;

            MySqlConnection conn = new MySqlConnection();
            string connectionstring = this.DataSource;

            conn.ConnectionString = connectionstring;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, "����ֱ����⣬���ݿ����ӳ���������Ϣ��" + ex.Message + "���޷�������������" + "\n", this.IsErrorLog));
                return false;
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (string.Compare(r[2].ToString(), this.TableName, true) == 0)
                {
                    IsTable = true;
                    break;
                }
            }

            if (IsTable == false)
            {
                //ͨ�������ַ����ѱ����ȡ���������ݱ���������ݱ�Ľ���
                string strMatch = "(?<=character set=)[^\\s]*(?=[\\s;])";
                Match s = Regex.Match(connectionstring, strMatch, RegexOptions.IgnoreCase);
                string Encoding = s.Groups[0].Value;

                //��Ҫ�����±������±��ʱ�����ado.net�½��еķ�ʽ������������
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.MySql, Encoding, dColumns);

                MySqlCommand com = new MySqlCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    if (ex.ErrorCode != -2147217900)
                    {
                        e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, "���ݿ⽨�������󣬴�����Ϣ��" + ex.Message + "\n", this.IsErrorLog));
                        return false ;
                    }
                }

                IsTable = true;
            }

            return IsTable;
        }

        private string getCreateTablesql(cGlobalParas.DatabaseType dType, string Encoding, DataColumnCollection dColumns)
        {
            string strsql = "";

            strsql = "create table " + this.TableName + "(";
            for (int i = 0; i < dColumns.Count; i++)
            {
                switch (dType)
                {
                    case cGlobalParas.DatabaseType.Access:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                    case cGlobalParas.DatabaseType.MSSqlServer:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                    case cGlobalParas.DatabaseType.MySql:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                    default:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                }
            }
            strsql = strsql.Substring(0, strsql.Length - 1);
            strsql += ")";

            //�����mysql���ݿ⣬��Ҫ�������Ӵ����ַ����������ݱ�Ľ���
            if (dType == cGlobalParas.DatabaseType.MySql)
            {
                if (Encoding == "" || Encoding == null)
                    Encoding = "utf8";

                strsql += " CHARACTER SET " + Encoding + " ";
            }

            return strsql;
        }
        #endregion

        #region ֱ����� access mssqlserver mysql
        private void ExportAccess(DataTable tmpData)
        {
            bool IsTable = false;

            OleDbConnection conn = new OleDbConnection();

            string connectionstring = this.DataSource;

            conn.ConnectionString = connectionstring;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Error, "����ֱ����⣬���ݿ����ӳ����޷�������������" +  "\n", this.IsErrorLog));
                return;
            }

            //���轨���±���Ҫ����sql���ķ�ʽ���У�����Ҫ�滻sql����е�����
            System.Data.OleDb.OleDbCommand cm = new System.Data.OleDb.OleDbCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            //��ʼƴsql���
            string sql = "";

            for (int i = 0; i < tmpData.Rows.Count; i++)
            {
                sql = this.InsertSql;

                for (int j = 0; j < tmpData.Columns.Count; j++)
                {
                    string strPara = "{" + tmpData.Columns[j].ColumnName + "}";
                    string strParaValue = tmpData.Rows[i][j].ToString().Replace("\"", "\"\"");
                    sql = sql.Replace(strPara, strParaValue);
                }

                try
                {
                    cm.CommandText = sql;
                    cm.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error,  tmpData.Rows.ToString () + "����ʧ�ܣ�������Ϣ��" + ex.Message  +"\n", this.IsErrorLog));
                }
            }

            conn.Close();

        }

        private void ExportMSSql(DataTable tmpData)
        {
            bool IsTable = false;

            SqlConnection conn = new SqlConnection();

            string connectionstring = this.DataSource;

            conn.ConnectionString = connectionstring;

           
        
            //���轨���±���Ҫ����sql���ķ�ʽ���У�����Ҫ�滻sql����е�����
           

            //��ʼƴsql���
            string strInsertSql = this.InsertSql;

            //��Ҫ��˫�����滻�ɵ�����
            //strInsertSql = strInsertSql.Replace("\"", "'");

            string sql = "";

            for (int i = 0; i <tmpData.Rows.Count; i++)
            {
                sql = strInsertSql;

                for (int j = 0; j < tmpData.Columns.Count; j++)
                {
                    string strPara = "{" + tmpData.Columns[j].ColumnName + "}";
                    string strParaValue = tmpData.Rows[i][j].ToString().Replace("\"", "\"\"");
                    sql = sql.Replace(strPara, strParaValue);
                }
                
                try
                {
                    conn.Open();
                }
                catch (System.Exception ex)
                {

                    e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, "����ֱ����⣬���ݿ����ӳ����޷�������������" + "\n", this.IsErrorLog));
                    return;
                }

                try
                {
                    SqlCommand cm = new SqlCommand();
                    cm.Connection = conn;
                    cm.CommandType = CommandType.Text;
                    cm.CommandTimeout = 10;

                    cm.CommandText = sql;
                    cm.ExecuteNonQuery();
                    conn.Close();
                }
                catch (System.Exception ex)
                {
                    e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, tmpData.Rows.ToString() + "����ʧ�ܣ�������Ϣ��" + ex.Message + "\n", this.IsErrorLog));

                }
            }
            
        }

        private void ExportMySql(DataTable tmpData)
        {
            bool IsTable = false;

            MySqlConnection conn = new MySqlConnection();

            string connectionstring = this.DataSource;

            conn.ConnectionString = connectionstring;

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Error, "����ֱ����⣬���ݿ����ӳ����޷�������������" + "\n", this.IsErrorLog));
                return;
            }

            //���轨���±���Ҫ����sql���ķ�ʽ���У�����Ҫ�滻sql����е�����
            MySqlCommand cm = new MySqlCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            //��ʼƴsql���
            string strInsertSql = this.InsertSql;


            string sql = "";

            for (int i = 0; i < tmpData.Rows.Count; i++)
            {
                sql = strInsertSql;

                for (int j = 0; j < tmpData.Columns.Count; j++)
                {
                    string strPara = "{" + tmpData.Columns[j].ColumnName + "}";
                    string strParaValue = tmpData.Rows[i][j].ToString().Replace("\"", "\"\"");
                    sql = sql.Replace(strPara, strParaValue);
                }

                try
                {
                    cm.CommandText = sql;
                    cm.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Error, tmpData.Rows.ToString() + "����ʧ�ܣ�������Ϣ��" + ex.Message + "\n", this.IsErrorLog));
                }
            }

            conn.Close();
        }
        #endregion

        #endregion

        #region ������datatable�����ݺϲ���һ��
        //��datarow��datatable�ϲ���һ�� ����һ��һ��N�Ĺ�ϵ
        private DataTable MergeDataTable(DataRow dtr1, DataTable dt2)
        {
            DataTable dt1 = dtr1.Table.Clone();
            dt1.ImportRow(dtr1);

            DataTable dt3 = dt1.Clone();
            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                dt3.Columns.Add(dt2.Columns[i].ColumnName);
            }
            object[] obj = new object[dt3.Columns.Count];

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                dt3.Rows.Add(obj);
            }


            if (dt1.Rows.Count >= dt2.Rows.Count)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                    }
                }
            }
            else
            {
                DataRow dr3;
                for (int i = 0; i < dt2.Rows.Count - dt1.Rows.Count; i++)
                {
                    dr3 = dt3.NewRow();
                    dt3.Rows.Add(dr3);
                }
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                    }
                }
            }

            return dt3;
        }

        #endregion

        #region ��������ֽ����ݣ����ⲿ����

        /// <summary>
        /// ���÷ֽ����������,�ֽ������������Ҫ��������ʼ�ɼ�
        /// ����ҳ����,��ֹ�ɼ�����ҳ����,һ����Ҫ�ɼ�����ҳ����
        /// </summary>
        /// <param name="beginIndex">��ʼ�Ĳɼ���ҳ��ַ</param>
        /// <param name="endIndex">��ֹ�Ĳɼ���ҳ��ַ</param>
        public void SetSplitData(int beginIndex, int endIndex,List<Task.cWebLink> tUrl,List<Task.cWebpageCutFlag> tCutFlag)
        {
            lock (m_mstreamLock)
            {
                m_TaskSplitData.BeginIndex = beginIndex;
                m_TaskSplitData.CurIndex = beginIndex;
                m_TaskSplitData.EndIndex = endIndex;
                m_TaskSplitData.Weblink = tUrl;
                m_TaskSplitData.TrueUrlCount = tUrl.Count;
                m_TaskSplitData.CutFlag = tCutFlag;
                //m_IsDataInitialized = true;
            }
        }
       
        #endregion

        #region IDisposable ��Ա
        private bool m_disposed;
        /// <summary>
        /// �ͷ��� �ɼ� �ĵ�ǰʵ��ʹ�õ�������Դ
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

    }
}

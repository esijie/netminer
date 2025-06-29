using System;
using System.Collections.Generic;
using System.Threading;
using System.Data;
using System.Text.RegularExpressions;
using NetMiner.Core.Proxy;
using NetMiner.Common;
using NetMiner.Resource;
using System.Collections;
using NetMiner.Core.gTask.Entity;
using NetMiner.Core;
using NetMiner.Gather.Url;
using NetMiner.Core.Event;
using NetMiner.Net.Native;
using NetMiner.Core.Entity;
using NetMiner.Net.Common;

///���ܣ��ɼ����� �ֽ���������
///���ʱ�䣺2009-6-1
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
///�޶���2010-12-11
///�����˵����Զ���ҳ�Ĺ��ܣ��Զ���ҳͨ����ͬ����ĵ�����ҳ�������
//////�޶� 2011��10��8�� ���������ؿ�Ĵ�������汾����Ϊ��V2.1 ��Ҫ������ַ����
///ϵͳ�汾����Ϊ��V2.6.0.03
///2012-2-11 ���������ɼ���ַ�������⣬�����е����ɼ������Ҳɼ��������������⣬�ᵼ��
///�������󣬴Ӷ�����Ӱ��ϵͳ�޷����
///����һ�����߳����еĶ������񣬰������������ݼ�����������V5.5�汾���Դ�����в�֣�̫ӷ�ס�

namespace NetMiner.Gather.Control
{

    //���ǲɼ��������С��Ԫ,�����һ���ɼ��������,��һ��URL��ַ����
    //�����ж��������ǧ��URL��ַ,Ϊ���������,��URl�ɼ������˶��̵߳�
    //����ʽ,�������һ������Ҫ���в��,��ֵ������Ǵ������Ƶ��߳�
    //��,������ʼִ��.������Ҫ����ִ����ôһ����С��Ԫ������.
    //һ��cGatherTaskSplit�뵱��һ���̣߳������ڻ��ڴ����Ӷ��̵߳Ĵ�����Ϊ
    //������ҳ��С���߳�������ϵͳĬ�Ͼ��ǵ��߳��ˡ�

    public class cGatherTaskSplit : IDisposable
    {

        private delegate void work();

        private Thread m_Thread;
        //private bool m_ThreadRunning = false;

        private string RegexNextPage = "";

        //����һ������ֵ���жϣ�������ݲɼ��ǲ�����ֱ�����ķ�ʽ
        //�ж����ݱ��Ƿ���Ҫ�½�������½������ڵ�һ������ʱ�������±�
        //�ɹ��󣬲��޸Ĵ�ʱΪfalse������Ѿ����ڴ˱�Ҳ���޸�Ϊfalse
        private bool IsNewTable = true;

        //����һ��������Ϣ�����࣬�������ø��ɼ��࣬���ݲɼ������
        //����������Ϣ��ȥ��ȡ����
        private cProxyControl m_ProxyControl;

        //����һ�����ؿ⣬����url����
        private NetMiner.Base.cHashTree m_Urls;

        //����һ�����ؿ⣬���е�ǰ����ʵ�����е����أ���Ҫ����ϵ����ɵ�����
        private NetMiner.Base.cHashTree m_TempUrls;

        //����һ�����ؿ⣬���вɼ����ݵ��ظ����ж�
        private NetMiner.Base.cHashTree m_DataRepeat;

        //����һ��ֵ������¼��������Ĵ�����ע�⣺��������
        private int m_ContinuousErr;

        //����һ��ֵ������¼����ֱ�����Ĵ��������ע�⣺��������
        private int m_ContinuousInsertErr;

        //����һ��ֵ����ʾ��¼��ǰ�Ĺ���·��
        private string m_workPath = string.Empty;

        //����һ��ֵ���洢Cookie
        private string m_tmpCookie = string.Empty;

        //����һ��ֵ���ж��Ƿ����ȫ�����أ�ȫ�����ؽ��޲ɼ�����
        private bool m_isGlobalRepeat;
        public NetMiner.Base.cHashTree g_Urls;

        cGatherData gData = null;

        #region �����࣬����ʼ���������

        /// <summary>
        /// 2011��10��8���޸��˹��캯���ṹ�������ؿ����ô������
        /// </summary>
        /// <param name="ProxyControl"></param>
        internal cGatherTaskSplit(string workPath, ref cProxyControl ProxyControl, bool isGlobalRepeat, 
            ref NetMiner.Base.cHashTree gUrls, ref NetMiner.Base.cHashTree Urls, ref NetMiner.Base.cHashTree tUrls, 
            ref NetMiner.Base.cHashTree dRepeat)
        {
            this.m_workPath = workPath;

            m_ContinuousErr = 0;
            m_ContinuousInsertErr = 0;

            m_TaskSplitData = new cTaskSplitData();
            m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
            m_GatherData = new DataTable();

            m_ProxyControl = ProxyControl;
            m_Urls = Urls;
            m_TempUrls = tUrls;
            m_DataRepeat = dRepeat;

            m_isGlobalRepeat = isGlobalRepeat;
            g_Urls = gUrls;

            this.TaskEntity = new eTask();

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
            if (gData != null)
            {
                gData.Log -= this.on_Log;
                gData.GUrlCount -= this.on_GatherUrlCount;
                gData.Error -= this.onError;
                gData.GData -= this.on_GatherData;
                gData = null;
            }

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


        private cTaskSplitData m_TaskSplitData;
        public cTaskSplitData TaskSplitData
        {
            get { return m_TaskSplitData; }
            set { m_TaskSplitData = value; }
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

  
        //V3.0����
        private string[] m_AutoID;
        public string[] AutoID
        {
            get { return m_AutoID; }
            set { m_AutoID = value; }
        }

        public cGlobalParas.ProxyType pType { get; set; }
        public string ProxyAddress { get; set; }
        public int ProxyPort { get; set; }

        public eTask TaskEntity { get; set; }

        public int cookieIndex { get; set; }
     
        #endregion

        #region ��������״̬����

        /// <summary>
        /// �ֽ�ɼ������Ƿ��Ѿ����
        /// </summary>
        public bool IsCompleted
        {
            //get { return m_TaskSplitData.GatheredUrlCount == m_TaskSplitData.UrlCount ; }
            //get { return m_TaskSplitData.UrlCount > 1 && m_TaskSplitData.GatheredUrlCount + m_TaskSplitData.GatheredErrUrlCount  == m_TaskSplitData.TrueUrlCount; }
            get
            {
                if (m_TaskSplitData.UrlCount == m_TaskSplitData.GatheredUrlCount + m_TaskSplitData.GatheredErrUrlCount &&
                    m_TaskSplitData.UrlNaviCount == m_TaskSplitData.GatheredUrlNaviCount + m_TaskSplitData.GatheredErrUrlNaviCount)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// �ֽ�Ĳɼ������Ƿ��Ѿ��ɼ����
        /// </summary>
        public bool IsGathered
        {
            //get { return m_TaskSplitData.UrlCount > 0 && m_TaskSplitData.GatheredUrlCount +m_TaskSplitData.GatheredErrUrlCount == m_TaskSplitData.TrueUrlCount ; }
            get
            {
                if (m_TaskSplitData.UrlCount == m_TaskSplitData.GatheredUrlCount + m_TaskSplitData.GatheredErrUrlCount &&
                    m_TaskSplitData.UrlNaviCount == m_TaskSplitData.GatheredUrlNaviCount + m_TaskSplitData.GatheredErrUrlNaviCount)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// ��ȡһ��״ֵ̬��ʾ��ǰ�߳��Ƿ��ںϷ�����״̬ [���߳��ڲ�ʹ��]
        /// </summary>
        private bool IsCurrentRunning
        {
            get { return ThreadState == cGlobalParas.GatherThreadState.Started && Thread.CurrentThread.Equals(m_Thread); }
        }

        /// <summary>
        /// ��ȡ��ǰ�߳��Ƿ��Ѿ�ֹͣ��ע��IsThreadRunningֻ��һ����־
        /// ������ȷ���̵߳�״̬��ֻ�Ǹ����߳���Ҫֹͣ��
        /// IsStop�������ⲿ������˷ֽ������Ƿ�ֹͣʹ��
        /// </summary>
        public bool IsStop
        {
            get { return gData.ThreadRunning == false && this.IsCurrentRunning == false; }
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
            get { return m_TaskSplitData.EndIndex; }
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

        public int GatheredUrlNaviCount
        {
            get { return m_TaskSplitData.GatheredUrlNaviCount; }
        }

        /// <summary>
        /// �Ѳɼ����������ַ����
        /// </summary>
        public int GatherErrUrlCount
        {
            get { return m_TaskSplitData.GatheredErrUrlCount; }
        }

        public int GatheredErrUrlNaviCount
        {
            get { return m_TaskSplitData.GatheredErrUrlNaviCount; }
        }

        /// <summary>
        /// һ����Ҫ�ɼ�����ַ����
        /// </summary>
        public int UrlCount
        {
            get { return m_TaskSplitData.UrlCount; }
        }

        public int UrlNaviCount
        {
            get { return m_TaskSplitData.UrlNaviCount; }
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

        //����ֹͣ�ɼ������¼�
        private event EventHandler<cTaskEventArgs> e_RequestStopped;
        internal event EventHandler<cTaskEventArgs> RequestStopped
        {
            add { lock (m_eventLock) { e_RequestStopped += value; } }
            remove { lock (m_eventLock) { e_RequestStopped -= value; } }
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

        private event EventHandler<cUpdateCookieArgs> e_UpdateCookie;
        internal event EventHandler<cUpdateCookieArgs> UpdateCookie
        {
            add { lock (m_eventLock) { e_UpdateCookie += value; } }
            remove { lock (m_eventLock) { e_UpdateCookie -= value; } }
        }
        #endregion

        #region �¼���������
        //�¼�����������ͨ���ı�����״̬����ɵ�
        //ͨ����������״̬�ı�����������¼��������շ���������
        //������Ȩ��������

        private delegate void delegateSetThreadState(cGlobalParas.GatherThreadState tState);
        private void SetThreadState(cGlobalParas.GatherThreadState tState)
        {
            ThreadState = tState;
        }

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

                            //����Cookie
                            this.TaskEntity.Cookie = new cCookieManage(m_tmpCookie).getCookie().Value;

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

                        //�ڴ˼���߳��Ƿ�ֹͣ�����ֹͣ���򴥷�ֹͣ�¼�

                        //while(IsThreadAlive)
                        //{
                        //    Thread.Sleep(10);
                        //}


                        if (e_Stopped != null)
                        {
                            //����Cookie
                            this.TaskEntity.Cookie = new cCookieManage(m_tmpCookie).getCookie().Value;

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
        private void onError(object sender, TaskThreadErrorEventArgs e)
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
                        e_Error(this, e);
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
            try
            {
                if (m_ThreadState != cGlobalParas.GatherThreadState.Started && !IsThreadAlive && !IsCompleted)
                {
                    lock (m_threadLock)
                    {
                        //����ҳ���ȡ���ݵ���������ж��Ƿ���Ҫ�������ݺϲ�
                        //for (int i = 0; i < this.m_TaskSplitData.CutFlag.Count; i++)
                        //{
                        //    if (this.m_TaskSplitData.CutFlag[i].IsMergeData == true)
                        //    {
                        //        m_IsMergeData = true;
                        //        break;
                        //    }
                        //}

                        //ȡ��һҳ��ҳ���������
                        cXmlSConfig cCon = new cXmlSConfig(this.m_workPath);
                        RegexNextPage = ToolUtil.ShowTrans(cCon.RegexNextPage);
                        cCon = null;

                        gData = new cGatherData(this.TaskEntity, this.TaskSplitData, this.m_workPath, ref m_ProxyControl);

                        gData.AutoID = this.AutoID;
                        gData.ProxyAddress = this.ProxyAddress;
                        gData.ProxyPort = this.ProxyPort;
                        gData.RegexNextPage = this.RegexNextPage;

                        gData.m_isGlobalRepeat = this.m_isGlobalRepeat;
                        gData.g_Urls = this.g_Urls;
                        gData.m_DataRepeat = this.m_DataRepeat;
                        gData.m_Urls = this.m_Urls;
                        gData.m_TempUrls = this.m_TempUrls;

                        gData.Log += this.on_Log;
                        gData.GUrlCount += this.on_GatherUrlCount;
                        gData.Error += this.onError;
                        gData.GData += this.on_GatherData;
                        gData.RequestStopped += this.on_ForceStop;

                        //�����߳����б�־����ʶ���߳�����
                        gData.ThreadRunning = true;

                        m_Thread = new Thread(this.ThreadWorkInit);

                        //�ڴ������̵߳�����
                        if (this.TaskEntity .IsVisual==false)
                            m_Thread.SetApartmentState(ApartmentState.MTA);
                        else
                            m_Thread.SetApartmentState(ApartmentState.STA);

                        //�����߳�����,���ڵ���ʹ��
                        m_Thread.Name = this.TaskEntity.TaskID.ToString() + "-" + m_TaskSplitData.BeginIndex.ToString();
                        
                        m_Thread.Start();
                        m_ThreadState = cGlobalParas.GatherThreadState.Started;
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
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
            gData.ThreadRunning = false;


            //if (m_ThreadState == cGlobalParas.GatherThreadState.Started && IsThreadAlive)
            //{
            //    lock (m_threadLock)
            //    {
            //        if (m_ThreadState == cGlobalParas.GatherThreadState.Started)
            //        {
            //            m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
            //        }
            //    }
            //}
            //else
            //{
            //    if (m_ThreadState == cGlobalParas.GatherThreadState.Started)
            //    {
            //        m_ThreadState = cGlobalParas.GatherThreadState.Stopped;
            //    }
            //}

            //ThreadState = cGlobalParas.GatherThreadState.Stopped;
        }

        public void Abort()
        {
            gData.ThreadRunning = false;

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
        /// ��ͣ��ǰ�߳�ִ��һ��ʱ����
        /// </summary>
        /// <param name="interval"></param>
        public void Pause(float interval)
        {
            //Thread.Sleep((int)(interval * 1000));

           this.TaskEntity.GatherCountPauseInterval = interval;
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

        /// <summary>
        /// ǿ��ֹͣ����
        /// </summary>
        private void StopTaskRule(cGlobalParas.StopRule sRule)
        {
            if (sRule == cGlobalParas.StopRule.StopUrlGather)
            {
                throw new NetMinerSkipUrlException("�����û����õ���������ַ�ɼ��Ĺ��򣬿����ǲɼ������ɼ����ظ�����");
            }
            else if (sRule == cGlobalParas.StopRule.StopTaskGather)
            {
                if (e_RequestStopped != null)
                    e_RequestStopped(this, null);
            }
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
                e_TaskInit(this, new TaskInitializedEventArgs(this.TaskEntity.TaskID));

            }
            else //if (GatheredUrlCount !=TrueUrlCount )
            {
                ThreadWork();
            }
        }

        /// <summary>
        /// �ɼ�����
        /// </summary>
        private void ThreadWork()
        {
            

            cGlobalParas.GatherResult IsSucceed = cGlobalParas.GatherResult.UnGather;

            for (int i = 0; i < m_TaskSplitData.Weblink.Count; i++)
            {
                m_tmpCookie = this.TaskEntity.Cookie;

                if (gData.ThreadRunning == true)
                {

                    try
                    {
                        switch (m_TaskSplitData.Weblink[i].IsGathered)
                        {
                            case (int)cGlobalParas.UrlGatherResult.UnGather:

                                //e_Log(this, new cGatherTaskLogArgs(m_TaskID,cGlobalParas.LogType.Info, "���ڲɼ���" + m_TaskSplitData.Weblink[i].Weblink ,this.IsErrorLog ));

                                //�жϴ���ַ�Ƿ�Ϊ������ַ������ǵ�����ַ����Ҫ���Ƚ���Ҫ�ɼ�����ַ��ȡ����
                                //Ȼ����о�����ַ�Ĳɼ�
                                #region ����ɼ��Ĳ���
                                if (m_TaskSplitData.Weblink[i].IsNavigation == true)
                                {
                                    List<eMultiPageRule> listMultiRules = null;
                                    //�ڴ��ж��Ƿ���Ҫ���ж�ҳ�ɼ�
                                    if (m_TaskSplitData.Weblink[i].IsMultiGather == true)
                                    {
                                        listMultiRules = m_TaskSplitData.Weblink[i].MultiPageRules;
                                    }

                                    IsSucceed = gData.GatherNavigationUrl(m_TaskSplitData.Weblink[i].Weblink, m_TaskSplitData.Weblink[i].NavigRules,
                                        i, m_TaskSplitData.Weblink[i].IsData121, listMultiRules,"");

                                }
                                else
                                {

                                    //�ڴ��ж��Ƿ���Ҫ���ж�ҳ�ɼ�
                                    if (m_TaskSplitData.Weblink[i].IsMultiGather == true)
                                    {
                                        List<eMultiPageRule> listMultiRules = null;
                                        listMultiRules = m_TaskSplitData.Weblink[i].MultiPageRules;
                                        IsSucceed = gData.GatherSingleUrl(m_TaskSplitData.Weblink[i].Weblink, 0, m_TaskSplitData.Weblink[i].IsNextpage, m_TaskSplitData.Weblink[i].NextPageRule, i, null, m_TaskSplitData.Weblink[i].IsData121, listMultiRules, "");

                                    }
                                    else
                                    {
                                        //�ǵ���ҳ �ɼ�ҳ ��������Ĭ��Ϊ��0  ͬʱ���ݽ��ȵĺϲ�����ҲΪnull
                                        IsSucceed = gData.GatherSingleUrl(m_TaskSplitData.Weblink[i].Weblink, 0, 
                                            m_TaskSplitData.Weblink[i].IsNextpage, m_TaskSplitData.Weblink[i].NextPageRule, 
                                            m_TaskSplitData.Weblink[i].NextMaxPage,i, null, "","",0);
                                    }
                                }
                                #endregion

                                //�ɼ���ַ��������
                                m_TaskSplitData.CurIndex++;

                                #region �������������
                                if (IsSucceed == cGlobalParas.GatherResult.GatherSucceed
                                    || IsSucceed == cGlobalParas.GatherResult.GatherStoppedByUser)
                                {

                                    //ÿ�ɼ����һ��Url������Ҫ�޸ĵ�ǰCurIndex��ֵ����ʾϵͳ�������У���
                                    //����ȷ���˷ֽ������Ƿ��Ѿ����,���ұ�ʾ����ַ�Ѿ��ɼ����
                                    if (m_TaskSplitData.Weblink[i].IsNavigation == true)
                                    {
                                        //��Ҫ�жϵ�����ַ�Ƿ�ɼ����
                                        if (UrlNaviCount == GatheredUrlNaviCount + GatheredErrUrlNaviCount)
                                        {
                                            //�����ɼ����
                                            m_TaskSplitData.Weblink[i].IsGathered = (int)cGlobalParas.UrlGatherResult.Succeed;
                                            m_TaskSplitData.GatheredUrlCount++;
                                            e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));
                                        }
                                        //else if (UrlNaviCount > GatheredUrlNaviCount + GatheredErrUrlNaviCount && IsSucceed == cGlobalParas.GatherResult.GatherSucceed)
                                        //{
                                        //    m_TaskSplitData.Weblink[i].IsGathered = (int)cGlobalParas.UrlGatherResult.Gathered;
                                        //    m_TaskSplitData.GatheredUrlCount++;
                                        //    e_GUrlCount(this, new cGatherUrlCountArgs(m_TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));

                                        //}
                                        else
                                        {
                                            m_TaskSplitData.Weblink[i].IsGathered = (int)cGlobalParas.UrlGatherResult.UnGather;
                                            m_TaskSplitData.GatheredErrUrlCount++;
                                            e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                                        }
                                    }
                                    else
                                    {
                                        m_TaskSplitData.Weblink[i].IsGathered = (int)cGlobalParas.UrlGatherResult.Succeed;
                                        m_TaskSplitData.GatheredUrlCount++;
                                        e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));

                                    }

                                }
                                else if (IsSucceed == cGlobalParas.GatherResult.GatherFailed)
                                {
                                    e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));

                                    m_TaskSplitData.Weblink[i].IsGathered = (int)cGlobalParas.UrlGatherResult.Error;
                                    m_TaskSplitData.GatheredErrUrlCount++;
                                }
                                #endregion

                                break;

                            case (int)cGlobalParas.UrlGatherResult.Succeed:
                                m_TaskSplitData.CurIndex++;
                                m_TaskSplitData.GatheredUrlCount++;
                                e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));
                                break;
                            case (int)cGlobalParas.UrlGatherResult.Error:
                                m_TaskSplitData.CurIndex++;
                                m_TaskSplitData.GatheredErrUrlCount++;
                                e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                                break;
                            case (int)cGlobalParas.UrlGatherResult.Gathered:
                                m_TaskSplitData.CurIndex++;
                                m_TaskSplitData.GatheredUrlCount++;
                                e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));
                                break;
                        }
                    }
                    catch (NetMinerSkipUrlException)
                    {
                        //�ڴ˲���ֹͣ��ǰUrl�ɼ�������һ��Url�ɼ��Ĳ�����
                        //��Ϊ��������еĲ���
                        //���Ե��ɼ��ɹ�����
                        m_TaskSplitData.CurIndex++;
                        m_TaskSplitData.GatheredUrlCount++;
                        e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));
                    }
                    catch (System.Exception)
                    {
                        m_TaskSplitData.CurIndex++;
                        m_TaskSplitData.GatheredErrUrlCount++;
                        e_GUrlCount(this, new cGatherUrlCountArgs(this.TaskEntity.TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                    }
                }
                else if (gData.ThreadRunning == false)
                {
                    //��ʾ�߳���Ҫ��ֹ���˳�forѭ��
                    //������ֹ��
                    break;
                }

                if (e_UpdateCookie != null)
                    e_UpdateCookie(this, new cUpdateCookieArgs(cookieIndex, m_tmpCookie));

              
            }

            //ÿ�ɼ����һ����ַ��Ҫ�жϵ�ǰ�߳��Ƿ��Ѿ�����
            //��Ҫ�ж��Ѳɼ�����ַ�Ͳɼ������������ַ
            //2013-1-24�����޸ģ�������ʵ�ʲɼ���ַ�������жϣ������޷�׼ȷ�ж�
            //�ɼ�����ʱ���Ѿ���ɣ����������ͳ�����

            delegateSetThreadState dSet = new delegateSetThreadState(SetThreadState);

            if (UrlCount == GatheredUrlCount + GatherErrUrlCount && UrlNaviCount == GatheredUrlNaviCount + GatheredErrUrlNaviCount)
            {
                //ThreadState = cGlobalParas.GatherThreadState.Completed;
                dSet.BeginInvoke(cGlobalParas.GatherThreadState.Completed, null, null);

            }
            else if (UrlCount == GatherErrUrlCount && UrlNaviCount == GatheredErrUrlNaviCount)
            {
                //��ʾ�ɼ�ʧ�ܣ�������һ��������ԣ�һ���̵߳���ȫʧ�ܲ�������
                //����Ҳ�ɼ���ȫʧ�ܣ���������İ�ȫʧ����Ҫ������ɼ������ж�
                //���ԣ��ڴ˻��Ƿ����߳��������ʱ�䣬����ʧ��ʱ�䡣
                //ThreadState = cGlobalParas.GatherThreadState.Completed;

                //ThreadState = cGlobalParas.GatherThreadState.Completed;
                dSet.BeginInvoke(cGlobalParas.GatherThreadState.Completed, null, null);
            }
            else if (UrlCount < GatheredUrlCount + GatherErrUrlCount && UrlNaviCount <= GatheredUrlNaviCount + GatheredErrUrlNaviCount)
            {
                //ThreadState = cGlobalParas.GatherThreadState.Completed;

                //ThreadState = cGlobalParas.GatherThreadState.Completed;
                dSet.BeginInvoke(cGlobalParas.GatherThreadState.Completed, null, null);
            }
            else
            {
                //ThreadState = cGlobalParas.GatherThreadState.Stopped;

                //ThreadState = cGlobalParas.GatherThreadState.Completed;
                dSet.BeginInvoke(cGlobalParas.GatherThreadState.Stopped, null, null);
            }

            gData.ThreadRunning = false;

        }

        //���ڲɼ�һ����ҳ�����ݣ�ר�����ڴ����ҳ�ɼ����ݣ���ҳ�ݲ������ҳ
        /// <summary>
        /// 2012-9-27 �Զ�ҳ�ɼ����д���ԭ�з�ʽ�ǿ���ͨ����ҳԴ���ȡ��ҳ����ַ
        /// �����˿���ͨ����ҳԴ��ɼ������������뵽��ҳ��ַ���вɼ���
        /// </summary>
        /// 

    
        #endregion

        #region ��������ֽ����ݣ����ⲿ����

        /// <summary>
        /// ���÷ֽ����������,�ֽ������������Ҫ��������ʼ�ɼ�
        /// ����ҳ����,��ֹ�ɼ�����ҳ����,һ����Ҫ�ɼ�����ҳ����
        /// </summary>
        /// <param name="beginIndex">��ʼ�Ĳɼ���ҳ��ַ</param>
        /// <param name="endIndex">��ֹ�Ĳɼ���ҳ��ַ</param>
        public void SetSplitData(int beginIndex, int endIndex, List<eWebLink> tUrl, List<eWebpageCutFlag> tCutFlag)
        {
            lock (m_mstreamLock)
            {
                m_TaskSplitData.BeginIndex = beginIndex;
                m_TaskSplitData.CurIndex = beginIndex;
                m_TaskSplitData.EndIndex = endIndex;
                m_TaskSplitData.Weblink = tUrl;
                //m_TaskSplitData.TrueUrlCount = tUrl.Count;
                //m_TaskSplitData.UrlCount = tUrl.Count;
                m_TaskSplitData.UrlNaviCount = 0;
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

        #region ����ɼ������־��Ϣ
        //������־�¼�,��Ϊ�ɼ����¼��޷���ȷ�ɼ�������ID������ڴ˽��в���
        public void on_Log(object sender, cGatherTaskLogArgs e)
        {
            if (e_Log != null)
                e_Log(sender, new cGatherTaskLogArgs(this.TaskEntity.TaskID, TaskEntity.TaskName, e.LogType, e.strLog, TaskEntity.IsErrorLog));
        }

        public void on_GatherUrlCount(object sender,cGatherUrlCountArgs e)
        {
            if (e_GUrlCount != null)
                e_GUrlCount(sender, e);
        }

        public void on_GatherData(object sender, cGatherDataEventArgs e)
        {
            if (e_GData != null)
                e_GData(sender, e);
        }

        /// <summary>
        /// ǿ��ֹͣ�ɼ�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void on_ForceStop(object sender, cTaskEventArgs e)
        {
            if(e_RequestStopped !=null)
                e_RequestStopped(this, null);
        }

        #endregion

    }
}

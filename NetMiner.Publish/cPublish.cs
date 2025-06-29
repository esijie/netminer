using System;
using System.Collections.Generic;
using System.Threading;
using System.Data;
//using Interop.Excel;
using System.Text.RegularExpressions;
using NetMiner.Common.HttpSocket;
using NetMiner.Resource;
using NetMiner.Publish.Rule;
using System.Collections;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;
using NetMiner.Core.pTask.Entity;
using NetMiner.Core.Event;

///���ܣ���������
///���ʱ�䣺2009-7-21
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.01.00
///�޶��������˷������֧�����ݿ⡢web���ļ�����
///2013-2-16 �����޸ģ����󹤲ɼ�����ķ��������ݷ������ߵķ���
///�������ϣ����ô˷������У�ͬʱ֧�ֶ��̣߳�������ʱ�Ȳ���
///ͬʱɾ���������¼����������Ѿ���V2012SP1��ɾ��
namespace NetMiner.Publish
{
    public class cPublish
    {
        private ePublishTaskData m_pTaskData;
        private cPublishManage m_PublishManage;
        
        //����һ��datatable�����ڸ�����Ҫ����������
        //���ӷ���������ȡ����Ҫ��������������
        private Queue<DataRow> UnPublishData;

        bool m_ThreadsRunning = false;
        private Thread[] threadsRun;

        //����ʹ��
        private int m_dCount = 0;

        private int SleepConnectTime = 1;
        private int SleepFetchTime = 1;

        //����һ����ȡRow�������α�
        private int m_CurRowIndex = 0;

        //�������ݵ�cookie�����÷���ģ��ʱʹ��
        private string m_Cookie=string.Empty;

        private Hashtable m_WebClass;
        private bool m_IsGetClass = false;

        //����һ��ȫ�ֵĲ�������
        private Hashtable m_globalPara;

        //����һ��ֵ���ж�ϵͳ�Ƿ��¼�ɹ�
        private bool m_isLoginSucceed = false;

        private string m_workPath = string.Empty;

        //���캯���������ַ�ʽ��֧�ֲɼ�����ķ��������ݷ������ߵķ�������
        /// <summary>
        /// ��ʼ��������
        /// </summary>
        /// <param name="taskManage">����������</param>
        /// <param name="TaskID">��������ID</param>
        /// <param name="dData">���������ݣ�����һ�����������ݱ�</param>
        public cPublish(string workPath, cPublishManage taskManage,Int64 TaskID,DataTable dData)
        {
            m_workPath = workPath;

            m_PublishManage = taskManage;
            m_pTaskData = new ePublishTaskData();
            UnPublishData = new Queue<DataRow>();

            //��ʼ����������
            LoadTaskInfo(TaskID, dData);
            

            //��ʼ��������������
            m_dCount = dData.Rows.Count;

            //��ȡδ��������
            if (dData.Rows.Count > 0 && dData.Columns[dData.Columns.Count - 1].ColumnName == "isPublished")
            {
                DataRow[] rows = dData.Select("isPublished='" + cGlobalParas.PublishResult.UnPublished.ToString() + "'");
                AddQueue(rows);

                PublishedCount = dData.Select("isPublished='" + cGlobalParas.PublishResult.Succeed.ToString() + "'").Length;
                PublishErrCount = dData.Select("isPublished='" + cGlobalParas.PublishResult.Fail.ToString() + "'").Length;

            }

            this.pThreadState = cGlobalParas.PublishThreadState.UnStart;
        }

        /// <summary>
        /// ��ʼ�������࣬����ϵͳר��
        /// </summary>
        /// <param name="taskManage">����������</param>
        /// <param name="TaskID">��������ID</param>
        /// <param name="dData">����������</param>
        public cPublish(string workPath, cPublishManage taskManage,ePublishTask cpTask, bool isErrlog, DataTable dData)
        {
            m_workPath = workPath;

            m_PublishManage = taskManage;
            m_pTaskData = new ePublishTaskData();
            UnPublishData = new Queue<DataRow>();

            //��ʼ������
            LoadPublishInfo(cpTask,-1, dData, isErrlog);

            m_dCount = dData.Rows.Count;

            //��ȡδ��������
            if (dData.Rows.Count > 0)
            {
                DataRow[] rows = dData.Select("isPublished='" + cGlobalParas.PublishResult.UnPublished.ToString() + "'");
                AddQueue(rows);

                PublishedCount = dData.Select("isPublished='" + cGlobalParas.PublishResult.Succeed.ToString() + "'").Length;
                PublishErrCount = dData.Select("isPublished='" + cGlobalParas.PublishResult.Fail.ToString() + "'").Length;
            }

            this.pThreadState = cGlobalParas.PublishThreadState.UnStart;
        }

        /// <summary>
        /// ��ʼ�������࣬�ֲ�ʽ�ɼ����ݺϲ�ר��
        /// </summary>
        /// <param name="workPath"></param>
        /// <param name="taskManage"></param>
        /// <param name="cpTask"></param>
        /// <param name="isErrlog"></param>
        /// <param name="dData"></param>
        public cPublish(string workPath, cPublishManage taskManage, ePublishTask cpTask, bool isErrlog, DataTable dData,Int64 TaskID)
        {
            m_workPath = workPath;

            m_PublishManage = taskManage;
            m_pTaskData = new ePublishTaskData();
            UnPublishData = new Queue<DataRow>();

            //��ʼ������
            LoadPublishInfo(cpTask,TaskID, dData, isErrlog);

            m_dCount = dData.Rows.Count;

            //��ȡδ��������
            if (dData.Rows.Count > 0)
            {
                DataRow[] rows = dData.Select("isPublished='" + cGlobalParas.PublishResult.UnPublished.ToString() + "'");
                AddQueue(rows);

                PublishedCount = dData.Select("isPublished='" + cGlobalParas.PublishResult.Succeed.ToString() + "'").Length;
                PublishErrCount = dData.Select("isPublished='" + cGlobalParas.PublishResult.Fail.ToString() + "'").Length;
            }

            this.pThreadState = cGlobalParas.PublishThreadState.UnStart;
        }

        ~cPublish()
        {
            UnPublishData = null;
        }

        #region ������������ѹ�����
        private void AddQueue(DataRow[] rows)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                UnPublishData.Enqueue(rows[i]);
            }
        }
        #endregion

        #region ��������
        private void LoadTaskInfo(Int64 TaskID, System.Data.DataTable dData)
        {
            //DataTable dt = new DataTable();
            oTask t = new oTask(this.m_workPath);

            t.LoadTask(this.m_workPath + "tasks\\run\\task" + TaskID + ".rst");

            string SysSaveFileName = t.TaskEntity.SavePath + "\\" + t.TaskEntity.TaskName + "-" + t.TaskEntity.TaskID + ".xml";

            m_pTaskData.TaskID = t.TaskEntity.TaskID;
            m_pTaskData.TaskName = t.TaskEntity.TaskName;
            m_pTaskData.ExportFile = t.TaskEntity.ExportFile;
            m_pTaskData.DataSource = t.TaskEntity.DataSource;
            m_pTaskData.IsSaveSingleFile = t.TaskEntity.IsSaveSingleFile;

            if (t.TaskEntity.IsSaveSingleFile)
            {
                m_pTaskData.TempDataFile = t.TaskEntity.SavePath + "\\" + t.TaskEntity.TempDataFile;
                m_pTaskData.SysSaveTempFileName = t.TaskEntity.SavePath + "\\" + t.TaskEntity.TempDataFile;
            }
            else
            {
                m_pTaskData.SysSaveTempFileName = SysSaveFileName;
            }

            m_pTaskData.IsDelTempData = t.TaskEntity.IsDelTempData;

            //dt.ReadXml(FileName);
            //��Ҫ����Ļ��ߵ��������ݻ��Ǵ��룬��Ϊ��Ҫ��ʱ���ݵı���
            //��һ����Ҫ����ʱ���ݱ���ͷ������ݽ��з���
            m_pTaskData.PublishData = dData;
            m_pTaskData.PublishData.TableName = t.TaskEntity.TaskName + "-" + t.TaskEntity.TaskID + ".xml";

            m_pTaskData.PublishType = t.TaskEntity.ExportType;
            m_pTaskData.DataTableName = t.TaskEntity.DataTableName;

            m_pTaskData.PublishThread = t.TaskEntity.PublishThread;

            m_pTaskData.InsertSql = t.TaskEntity.InsertSql;
            m_pTaskData.IsSqlTrue = t.TaskEntity.IsSqlTrue;

            m_pTaskData.PIntervalTime = t.TaskEntity.PIntervalTime;
            m_pTaskData.PSucceedFlag = t.TaskEntity.PSucceedFlag;

            m_pTaskData.IsErrorLog = t.TaskEntity.IsErrorLog;

            m_pTaskData.IsTrigger = t.TaskEntity.IsTrigger;

            m_pTaskData.IsExportHeader = t.TaskEntity.IsExportHeader;
            m_pTaskData.IsRowFile = t.TaskEntity.IsRowFile;

            if (t.TaskEntity.IsTrigger == true)
            {
                m_pTaskData.TriggerType = t.TaskEntity.TriggerType;
                m_pTaskData.TriggerTask = t.TaskEntity.TriggerTask;
            }

            //�����Ƿ��Զ���Header
            m_pTaskData.Headers = t.TaskEntity.Headers;

            //����ģ����Ϣ
            //����ģ����Ϣ
            m_pTaskData.TemplateName = t.TaskEntity.TemplateName;
            m_pTaskData.User = t.TaskEntity.User;
            m_pTaskData.Password = t.TaskEntity.Password;
            m_pTaskData.Domain = t.TaskEntity.Domain;
            m_pTaskData.TemplateDBConn = t.TaskEntity.TemplateDBConn;
            m_pTaskData.PublishParas = t.TaskEntity.PublishParas;

            //�ж�ģ����Ϣ�Ƿ���Ҫ��¼�������Ҫ�������Ͽ�ʼ��¼
            if (m_pTaskData.TemplateName != null && m_pTaskData.TemplateName!="")
            {
                string tName = m_pTaskData.TemplateName.Substring(0, m_pTaskData.TemplateName.IndexOf("["));
                cGlobalParas.PublishTemplateType pType =EnumUtil.GetEnumName<cGlobalParas.PublishTemplateType>(m_pTaskData.TemplateName.Substring(m_pTaskData.TemplateName.IndexOf("[") + 1, 
                    m_pTaskData.TemplateName.IndexOf("]") - m_pTaskData.TemplateName.IndexOf("[") - 1));

                //�ֽ�����������
                if (pType == cGlobalParas.PublishTemplateType.Web && this.m_Cookie == "")
                {
                    //���е�¼����
                    cTemplate template = new cTemplate(this.m_workPath);
                    template.LoadTemplate(tName);

                    //���¼��ط�������ʱ����
                    m_pTaskData.PIntervalTime = template.PublishInterval;
                    m_pTaskData.isPlugin = template.IsVCodePlugin;
                    m_pTaskData.strPlugin = template.VCodePlugin;

                    string webSource = "";
                    bool isLogin = false;

                    try
                    {
                        //��ʼ���е�¼����
                        isLogin = cHttpSocket.Login(m_pTaskData.User, m_pTaskData.Password, template.uCode,
                            m_pTaskData.Domain, template.Domain, template.LoginUrl, template.LoginRUrl, template.LoginVCodeUrl, template.LoginSuccess, template.LoginFail,
                            template.LoginParas, template.IsVCodePlugin, template.VCodePlugin, out this.m_Cookie, out webSource);
                    }
                    catch (System.Exception ex)
                    {
                        WriteLog("��¼ʧ�ܣ�������Ϣ��" + ex.Message);
                        if (e_PublishError != null)
                            e_PublishError(this, new PublishErrorEventArgs(this.TaskID, this.TaskData.TaskName, ex));
                    }

                    if (template.pgPara.Count > 0)
                    {
                        m_globalPara = new Hashtable();

                        for (int i = 0; i < template.pgPara.Count; i++)
                        {
                            if (template.pgPara[i].pgPage == cGlobalParas.PublishGlobalParaPage.LoginPage)
                            {
                                string label = template.pgPara[i].Label;
                                string strReg = template.pgPara[i].Value;

                                Match a = Regex.Match(webSource, strReg, RegexOptions.IgnoreCase);
                                string value = a.Groups[0].Value.ToString();
                                m_globalPara.Add(label, value);
                            }
                        }
                    }
                    t = null;

                    this.m_isLoginSucceed = isLogin;
                }
                else
                    this.m_isLoginSucceed =true ;
            }
            else
                this.m_isLoginSucceed = true;

            t = null;
        }

        //�������ݼӹ��������ߵķ�����Ϣ����Ϊ���ݼӹ���������
        //ֻ֧�����ݿ��web������ʽ����֧���ļ���ͬʱû�������ID
        //���ԣ�����ʱ����������Ҫ�ֹ�ָ��
        private void LoadPublishInfo(ePublishTask cpTask, Int64 TaskID, DataTable dData, bool isErrlog)
        {
            m_pTaskData.TaskID = TaskID;
            m_pTaskData.TaskName = cpTask.pName;
            m_pTaskData.ExportFile = "";
            m_pTaskData.DataSource = cpTask.DataSource;
            m_pTaskData.IsSaveSingleFile = false;
            m_pTaskData.SysSaveTempFileName = "";

            m_pTaskData.IsDelTempData = true;

            m_pTaskData.isEnginPublish = cpTask.isEnginPublish;
          
            m_pTaskData.PublishData = dData;
            m_pTaskData.PublishData.TableName = "pData";

            if (cpTask.PublishType == cGlobalParas.PublishType.PublishData)
            {
                switch (cpTask.DataType)
                {
                    case cGlobalParas.DatabaseType.Access:
                        m_pTaskData.PublishType = cGlobalParas.PublishType.PublishAccess;
                        break;
                    case cGlobalParas.DatabaseType.MSSqlServer:
                        m_pTaskData.PublishType = cGlobalParas.PublishType.PublishMSSql;
                        break;
                    case cGlobalParas.DatabaseType.MySql :
                        m_pTaskData.PublishType = cGlobalParas.PublishType.PublishMySql;
                        break;
                    case cGlobalParas.DatabaseType.Oracle :
                        m_pTaskData.PublishType = cGlobalParas.PublishType.publishOracle;
                        break;
                }
                
            }
            else
                m_pTaskData.PublishType = cpTask.PublishType;

            m_pTaskData.DataTableName = cpTask.DataTable;

            m_pTaskData.PublishThread = cpTask.ThreadCount;

            m_pTaskData.InsertSql = cpTask.InsertSql;
            m_pTaskData.IsSqlTrue = cpTask.IsSqlTrue;
            m_pTaskData.ExportUrl = cpTask.PostUrl;
            m_pTaskData.ExportUrlCode = ((int)cpTask.UrlCode).ToString ();
            m_pTaskData.ExportCookie = cpTask.Cookie;
            m_pTaskData.PIntervalTime = cpTask.PIntervalTime;
            m_pTaskData.PSucceedFlag = cpTask.SucceedFlag;

            m_pTaskData.IsErrorLog = isErrlog;

            m_pTaskData.IsTrigger = false;

            m_pTaskData.IsExportHeader = false ;
            m_pTaskData.IsRowFile = false ;

            //�����Ƿ��Զ���Header
            m_pTaskData.IsCustomHeader = cpTask.IsHeader;

            if (cpTask.IsHeader == true)
            {
                string strheader = cpTask.Header;
                foreach (string sc in strheader.Split('\r'))
                {
                    eHeader header = new eHeader();

                    string ss = sc.Trim();
                    if (ss != "")
                    {
                        header.Label = ss.Substring(0, ss.IndexOf(":"));
                        header.LabelValue = ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1);

                        m_pTaskData.Headers.Add(header);
                    }
                }
            }

            //����ģ����Ϣ
            m_pTaskData.TemplateName = cpTask.TemplateName;
            m_pTaskData.User = cpTask.User;
            m_pTaskData.Password = cpTask.Password;
            m_pTaskData.Domain = cpTask.Domain;
            m_pTaskData.TemplateDBConn = cpTask.TemplateDBConn;
            m_pTaskData.PublishParas = cpTask.PublishParas;

            //�ж�ģ����Ϣ�Ƿ���Ҫ��¼�������Ҫ�������Ͽ�ʼ��¼
            if (m_pTaskData.TemplateName != null && m_pTaskData.TemplateName != "")
            {
                string tName = m_pTaskData.TemplateName.Substring(0, m_pTaskData.TemplateName.IndexOf("["));
                cGlobalParas.PublishTemplateType pType = EnumUtil.GetEnumName<cGlobalParas.PublishTemplateType>( m_pTaskData.TemplateName.Substring(m_pTaskData.TemplateName.IndexOf("[") + 1, m_pTaskData.TemplateName.IndexOf("]") - m_pTaskData.TemplateName.IndexOf("[") - 1));

                //�ֽ�����������
                if (pType == cGlobalParas.PublishTemplateType.Web && this.m_Cookie == "")
                {
                    //���е�¼����
                    cTemplate t = new cTemplate(m_workPath);
                    t.LoadTemplate(tName);

                    //���¼��ط�������ʱ����
                    m_pTaskData.PIntervalTime = t.PublishInterval;

                    string webSource = "";

                    //��ʼ���е�¼����
                    bool isLogin = false;
                    try
                    {
                        isLogin = cHttpSocket.Login(m_pTaskData.User, m_pTaskData.Password, t.uCode,
                            m_pTaskData.Domain, t.Domain, t.LoginUrl, t.LoginRUrl, t.LoginVCodeUrl, t.LoginSuccess, t.LoginFail,
                            t.LoginParas, t.IsVCodePlugin, t.VCodePlugin, out this.m_Cookie, out webSource);
                    }
                    catch (System.Exception ex)
                    {
                        WriteLog("��¼ʧ�ܣ�������Ϣ��" + ex.Message);

                        if (e_PublishError != null)
                            e_PublishError(this, new PublishErrorEventArgs(this.TaskID, this.TaskData.TaskName, ex));
                    }

                    if (t.pgPara.Count > 0)
                    {
                        m_globalPara = new Hashtable();

                        for (int i = 0; i < t.pgPara.Count; i++)
                        {
                            if (t.pgPara[i].pgPage == cGlobalParas.PublishGlobalParaPage.LoginPage)
                            {
                                string label = t.pgPara[i].Label;
                                string strReg = t.pgPara[i].Value;

                                Match a = Regex.Match(webSource, strReg, RegexOptions.IgnoreCase);
                                string value = a.Groups[0].Value.ToString();
                                m_globalPara.Add(label, value);
                            }
                        }
                    }
                    t = null;

                    this.m_isLoginSucceed = isLogin;
                }
                else
                    this.m_isLoginSucceed = true;
            }
            else
                this.m_isLoginSucceed = true;

        }

        private void GetVCode(string code)
        {
           
        }
        #endregion

        #region ����

        public Int64 TaskID
        {
            get { return m_pTaskData.TaskID; }
        }

        public string TaskName
        {
            get { return m_pTaskData.TaskName; }
        }

        public ePublishTaskData TaskData
        {
            get { return m_pTaskData; }
        }

        public cPublishManage PublishManage
        {
            get { return m_PublishManage; }
        }

        public string SysSaveTempFileName
        {
            get { return m_pTaskData.SysSaveTempFileName; }
        }

        public cGlobalParas.PublishType PublishType
        {
            get { return m_pTaskData.PublishType; }
        }

        public int Count
        {
            get { return m_dCount; }
        }

        private int m_PublishedCount;
        public int PublishedCount
        {
            get { return m_PublishedCount; }
            set { m_PublishedCount = value; }
        }

        private int m_PublishErrCount;
        public int PublishErrCount
        {
            get { return m_PublishErrCount; }
            set { m_PublishErrCount = value; }
        }

        private int m_ThreadMaxCount;
        public int ThreadMaxCount
        {
            get { return m_ThreadCount; }
            set { m_ThreadMaxCount = value; }
        }

        private int m_ThreadCount;
        public int ThreadCount
        {
            get { return m_ThreadCount; }
            set
            {
                m_ThreadCount = value;

                try
                {
                    for (int nIndex = 0; nIndex < value; nIndex++)
                    {
                        if (threadsRun[nIndex] == null || threadsRun[nIndex].ThreadState != ThreadState.Suspended)
                        {
                            threadsRun[nIndex] = new Thread(new ThreadStart(ThreadRunFunction));
                            threadsRun[nIndex].Name = nIndex.ToString();
                            threadsRun[nIndex].Start();

                            //this.OnRunTimeMessageSend(this, new RunTimeEventArgs("�����̳߳ɹ���" + nIndex.ToString()));

                        }
                        else if (threadsRun[nIndex].ThreadState == ThreadState.Suspended)
                        {
                            threadsRun[nIndex].Resume();
                        }
                    }

                }
                catch (Exception)
                {
                }
            }
        }

        private cGlobalParas.PublishThreadState m_pThreadState;
        public cGlobalParas.PublishThreadState pThreadState
        {
            get { return m_pThreadState; }
            set { m_pThreadState = value; }
        }
        #endregion



        //�����ṩ��taskid����������Ϣ
        //���ݲ�Ӧ���Ǵ�����,�Ƕ�ȡ�ļ���,�����ڲ�֧��������,���Դ�����
   

        #region �¼�����
        private Thread m_Thread;
        private readonly Object m_eventLock = new Object();

        /// �ɼ���������¼�
        private event EventHandler<PublishCompletedEventArgs> e_PublishCompleted;
        internal event EventHandler<PublishCompletedEventArgs> PublishCompleted
        {
            add { lock (m_eventLock) { e_PublishCompleted += value; } }
            remove { lock (m_eventLock) { e_PublishCompleted -= value; } }
        }

        /// �ɼ�����ɼ�ʧ���¼�
        private event EventHandler<PublishFailedEventArgs> e_PublishFailed;
        internal event EventHandler<PublishFailedEventArgs> PublishFailed
        {
            add { lock (m_eventLock) { e_PublishFailed += value; } }
            remove { lock (m_eventLock) { e_PublishFailed -= value; } }
        }

        /// �ɼ�����ʼ�¼�
        private event EventHandler<PublishStartedEventArgs> e_PublishStarted;
        internal event EventHandler<PublishStartedEventArgs> PublishStarted
        {
            add { lock (m_eventLock) { e_PublishStarted += value; } }
            remove { lock (m_eventLock) { e_PublishStarted -= value; } }
        }

        //��������ֹͣ�¼�
        private event EventHandler<PublishStopEventArgs> e_PublishStop;
        public event EventHandler<PublishStopEventArgs> PublishStop
        {
            add { e_PublishStop += value; }
            remove { e_PublishStop -= value; }
        }

        /// �ɼ���������¼�
        private event EventHandler<PublishErrorEventArgs> e_PublishError;
        internal event EventHandler<PublishErrorEventArgs> PublishError
        {
            add { lock (m_eventLock) { e_PublishError += value; } }
            remove { lock (m_eventLock) { e_PublishError -= value; } }
        }

        /// ��ʱ���ݷ������ʱ��
        //private event EventHandler<PublishTempDataCompletedEventArgs> e_PublishTempDataCompleted;
        //internal event EventHandler<PublishTempDataCompletedEventArgs> PublishTempDataCompleted
        //{
        //    add { lock (m_eventLock) { e_PublishTempDataCompleted += value; } }
        //    remove { lock (m_eventLock) { e_PublishTempDataCompleted -= value; } }
        //}

        //������־�¼�
        private event EventHandler<PublishLogEventArgs> e_PublishLog;
        internal event EventHandler<PublishLogEventArgs> PublishLog
        {
            add { lock (m_eventLock) { e_PublishLog += value; } }
            remove { lock (m_eventLock) { e_PublishLog -= value; } }
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
        /// �̹߳���Ч���¼�
        /// </summary>
        private event EventHandler<RunTimeEventArgs> e_RuntimeInfo;
        public event EventHandler<RunTimeEventArgs> RuntimeInfo
        {
            add { e_RuntimeInfo += value; }
            remove { e_RuntimeInfo -= value; }
        }

        /// <summary>
        /// �����¼�
        /// </summary>
        private event EventHandler<DoCountEventArgs> e_DoCount;
        public event EventHandler<DoCountEventArgs> DoCount
        {
            add { e_DoCount += value; }
            remove { e_DoCount -= value; }
        }

        /// <summary>
        /// ���ط���ʧ�ܵĴ��������¼�
        /// </summary>
        private event EventHandler<PublishErrDataEventArgs> e_PublishErrorData;
        public event EventHandler<PublishErrDataEventArgs> PublishErrorData
        {
            add { e_PublishErrorData += value; }
            remove { e_PublishErrorData -= value; }
        }

        /// <summary>
        /// �������һ��web�����������ҳԴ��
        /// </summary>
        private event EventHandler<PublishSourceEventArgs> e_PublishSource;
        public event EventHandler<PublishSourceEventArgs> PublishSource
        {
            add { e_PublishSource += value; }
            remove { e_PublishSource -= value; }
        }

        private event EventHandler<UpdateStateArgs> e_UpdateState;
        public event EventHandler<UpdateStateArgs> UpdateState
        {
            add { e_UpdateState += value; }
            remove { e_UpdateState -= value; }
        }
        #endregion

        private readonly Object m_threadLock = new Object();

        #region ������ʱ�洢��������
        //�˷���������ʱ��������ʹ�ã�������û��ն��˲���
        //����ô˷������Ѿ��ɼ������ݽ�����ʱ��������ǰ
        //����ʱ���б���
        //public void startSaveTempData()
        //{
        //    lock (m_threadLock)
        //    {
        //        m_Thread = new Thread(this.SaveTempData);

        //        //�����߳�����,���ڵ���ʹ��
        //        m_Thread.Name = SysSaveTempFileName;

        //        m_Thread.Start();
        //    }
        //}

        //private readonly Object m_fileLock = new Object();

        //private void SaveTempData()
        //{
        //    //���������Ƿ񷢲�������Ҫ����ɼ�����������
        //    //���浽���ش���
        //    string FileName = "";

        //    try
        //    {
        //        if (m_pTaskData.IsSaveSingleFile)
        //        {
        //            FileName = m_pTaskData.TempDataFile;
        //            if (File.Exists(FileName))
        //            {
        //                System.Data.DataTable tmp = new System.Data.DataTable();
        //                tmp.ReadXml(FileName);
        //                tmp.Merge(m_pTaskData.PublishData);
        //                tmp.AcceptChanges();
        //                tmp.WriteXml(FileName, XmlWriteMode.WriteSchema);
        //                tmp = null;
        //            }
        //            else
        //            {
        //                m_pTaskData.PublishData.WriteXml(FileName, XmlWriteMode.WriteSchema);
        //            }
        //        }
        //        else
        //        {
        //            FileName = m_pTaskData.SysSaveTempFileName;
                    
        //            if (File.Exists(FileName))
        //            {
        //                lock (m_fileLock)
        //                {
        //                    File.Delete(m_pTaskData.SysSaveTempFileName);
        //                }
        //            }

        //            m_pTaskData.PublishData.WriteXml(FileName, XmlWriteMode.WriteSchema);
        //        }

        //        //��ʱ���ݵı�����ϵͳ��ά�����������´��뱻ע��
        //        //������ʱ���ݷ����ɹ��¼�
        //        //e_PublishTempDataCompleted(this, new PublishTempDataCompletedEventArgs(this.TaskData.TaskID, this.TaskData.TaskName));
        //    }
        //    catch (System.Exception ex)
        //    {
        //        //�洢��ʱ����ʱ���п��ܵ��¶���̷߳��ʵ�ʧ�ܲ�������ǰ��û��
        //        //�������ƣ�������������һ���ṩ
        //        if (e_PublishLog != null)
        //        {
        //            WriteLog(ex.Message);
        //            e_PublishLog(this, new PublishLogEventArgs(this.TaskData.TaskID, cGlobalParas.LogType.Error , "����" + this.TaskData.TaskName + "��ʱ�洢ʧ�ܣ�������ϢΪ��" + ex.Message  ,false));

        //        }
                
        //        if (e_PublishError != null)
        //        {
        //            e_PublishError(this, new PublishErrorEventArgs(this.TaskData.TaskID, this.TaskData.TaskName, ex));
        //        }
                
        //    }
        //}
        #endregion

        //�˷������ڲɼ�������ɺ����ݵķ�������
        public void startPublic()
        {
            
            m_ThreadsRunning = true;

            if (m_pTaskData.PublishType == cGlobalParas.PublishType.PublishAccess ||
                m_pTaskData.PublishType == cGlobalParas.PublishType.PublishMSSql ||
                m_pTaskData.PublishType == cGlobalParas.PublishType.PublishMySql ||
                m_pTaskData.PublishType ==cGlobalParas.PublishType.publishOracle)
            {
                cGlobalParas.DatabaseType dType=cGlobalParas.DatabaseType.SoukeyData;
                if (m_pTaskData.PublishType == cGlobalParas.PublishType.PublishMSSql)
                    dType = cGlobalParas.DatabaseType.MSSqlServer;
                else if (m_pTaskData.PublishType == cGlobalParas.PublishType.PublishAccess)
                    dType = cGlobalParas.DatabaseType.Access;
                else if (m_pTaskData.PublishType == cGlobalParas.PublishType.PublishMySql)
                    dType = cGlobalParas.DatabaseType.MySql;
                else if (m_pTaskData.PublishType == cGlobalParas.PublishType.publishOracle)
                    dType = cGlobalParas.DatabaseType.Oracle;

                NetMiner.Core.DB.oDBDeal dd = new NetMiner.Core.DB.oDBDeal(this.m_workPath);
                try
                {
                    dd.NewTable(m_pTaskData.PublishData.Columns, dType,  m_pTaskData.DataSource, m_pTaskData.DataTableName);
                }
                catch (System.Exception ex)
                {
                    if (e_PublishError != null)
                    {
                        e_PublishError(this, new PublishErrorEventArgs(this.TaskID, this.TaskData.TaskName, ex));
                    }
                }
                dd = null;
            }

            if (m_pTaskData.PublishType == cGlobalParas.PublishType.PublishCSV ||
                m_pTaskData.PublishType == cGlobalParas.PublishType.PublishExcel ||
                m_pTaskData.PublishType == cGlobalParas.PublishType.PublishTxt ||
                m_pTaskData.PublishType==cGlobalParas.PublishType.publishWord)
            {
                //�������ʱ�ļ�����ǿ��ʹ��1���߳̽��д���
                m_pTaskData.PublishThread = 1;
            }

            this.threadsRun = new Thread[m_pTaskData.PublishThread];

            //���õ�ǰ���߳�����ͬʱ�����߳�
            ThreadCount = m_pTaskData.PublishThread;

            e_PublishStarted(this, new PublishStartedEventArgs(m_pTaskData.TaskID,m_pTaskData.TaskName));
            this.m_pThreadState = cGlobalParas.PublishThreadState.Running;

        }

        public void StopPublish()
        {
            m_ThreadsRunning = false;

            delStopTask();

        }

        public void OverPublish()
        {
            this.pThreadState = cGlobalParas.PublishThreadState.Complete;

            //��ȡ��ʱ���ݱ�����ļ���
            string FileName = string.Empty;
            if (m_pTaskData.IsSaveSingleFile)
            {
                FileName = m_pTaskData.TempDataFile;
            }
            else
            {
                FileName = m_pTaskData.SysSaveTempFileName;
            }

            PublishCompletedEventArgs evt1 = new PublishCompletedEventArgs(this.TaskData.TaskID,
                this.TaskData.TaskName, m_pTaskData.IsDelTempData, FileName);

            e_PublishCompleted(this, evt1);

            if (e_PublishLog != null)
                e_PublishLog(this, new PublishLogEventArgs(this.TaskID, cGlobalParas.LogType.Info,
                    "���ݷ������!", false));
        }

        //��һ���첽�ص��ĺ���������������̵߳�״̬���Ӷ�ȷ���Ƿ�������ֹͣ����
        private delegate void delegateStop();
        private void delStopTask()
        {
            delegateStop sd = new delegateStop(this.dStopTask);
            AsyncCallback callback = new AsyncCallback(CallbackStop);
            sd.BeginInvoke(callback, sd);
        }

        private void dStopTask()
        {
            //�ڴ˵ȴ����ÿ���̵߳�״̬��
            //�����Ϊֹͣ״̬�����ֹͣ���������������ѭ��
            while (true)
            {
                bool isStop = true;
                for (int i = 0; i < threadsRun.Length; i++)
                {
                    if (threadsRun[i].ThreadState == ThreadState.Running)
                    {
                        isStop = false;
                    }
                }

                if (isStop == true)
                    break;
                else
                    System.Threading.Thread.Sleep(500);
            }
        }

        private void CallbackStop(IAsyncResult ir)
        {
            this.pThreadState = cGlobalParas.PublishThreadState.Stopped;

            if (e_PublishStop != null)
                e_PublishStop(this, new PublishStopEventArgs(m_pTaskData.TaskID, m_pTaskData.TaskName,
                    m_pTaskData.PublishData));
        }

        /// <summary>
        /// ǿ����ֹ���������񣬲�������������
        /// </summary>
        public void Abort()
        {
            m_ThreadsRunning = false;
            m_Thread = null;
            if (this.threadsRun != null)
            {
                for (int i = 0; i < this.threadsRun.Length; i++)
                    this.threadsRun[i].Abort();
            }
            this.threadsRun = null; 

        }

        public void ThreadRunFunction()
        {

            while (m_ThreadsRunning && int.Parse(Thread.CurrentThread.Name) < this.ThreadCount)
            {
               
                DateTime mstartTick = DateTime.Now;

                cPublishData cp = new cPublishData(this.m_workPath);

                if (this.PublishType == cGlobalParas.PublishType.PublishCSV || this.PublishType == cGlobalParas.PublishType.PublishExcel
                    || this.PublishType == cGlobalParas.PublishType.PublishTxt || this.PublishType==cGlobalParas.PublishType.publishWord)
                {
                    #region �������ļ���һ������������Ҫ��ȫ��������Ҫ��ȫ��ʧ��
                    try
                    {
                        cp.ExportFile(this.PublishType, m_pTaskData.TaskName, m_pTaskData.ExportFile,
                                    m_pTaskData.PublishData, m_pTaskData.IsExportHeader, m_pTaskData.IsRowFile);


                        cp = null;

                        //ֱ���˳�ѭ������ʾ�����������
                        //��Ϊ�Ƿ����ļ��������ڶ��̣߳�ͬʱҲ�����ڼ������⣬���ԣ�Ϊ��
                        //������ɷ�����������Ҫ��ֵdonecount
                        m_PublishedCount = m_dCount;

                        if (e_PublishLog != null)
                            e_PublishLog(this, new PublishLogEventArgs(this.TaskID, cGlobalParas.LogType.Info, "�ɼ������Ѿ��������ļ���" + m_pTaskData.ExportFile, false));
                    }
                    catch (System.Exception ex)
                    {
                        if (e_PublishError != null)
                        {
                            e_PublishError(this, new PublishErrorEventArgs(m_pTaskData.TaskID, m_pTaskData.TaskName, ex));
                        }
                        WriteLog(ex.Message);
                        m_PublishErrCount = m_dCount;
                    }

                    break;

                #endregion
                   
                }
                else
                {
                    if (m_pTaskData.isEnginPublish)
                    {
                        NetMiner.Core.DB.oDBDeal dd = new Core.DB.oDBDeal(this.m_workPath);
                        switch (this.PublishType)
                        {
                            case cGlobalParas.PublishType.PublishMySql:
                                dd.ExportMySqlALL(m_pTaskData.DataTableName, m_pTaskData.PublishData, m_pTaskData.DataSource, m_pTaskData.InsertSql, m_pTaskData.IsSqlTrue);
                                break;
                            case cGlobalParas.PublishType.PublishMSSql:
                                break;
                            case cGlobalParas.PublishType.publishOracle:
                                break;
                            case cGlobalParas.PublishType.PublishAccess:
                                break;
                         
                        }
                        m_PublishedCount = m_dCount;
                        m_ThreadsRunning = false;
                        dd = null;
                    }
                    else
                    {
                        object[] dRow = GetDataRow();

                        if (dRow != null)
                        {
                            if (this.PublishType == cGlobalParas.PublishType.publishTemplate)
                            {
                                #region ʹ��ģ�巢������
                                try
                                {
                                    //�ڴ˴�����ģ��Ĺ���
                                    string tName = m_pTaskData.TemplateName.Substring(0, m_pTaskData.TemplateName.IndexOf("["));
                                    cGlobalParas.PublishTemplateType pType = EnumUtil.GetEnumName<cGlobalParas.PublishTemplateType>( m_pTaskData.TemplateName.Substring(m_pTaskData.TemplateName.IndexOf("[") + 1, m_pTaskData.TemplateName.IndexOf("]") - m_pTaskData.TemplateName.IndexOf("[") - 1));

                                    //ģ�淢��
                                    if (pType == cGlobalParas.PublishTemplateType.Web)
                                    {
                                        //�ڴ��жϵ�¼�Ƿ�ɹ���������ɹ�����ֱ�ӽ�������
                                        if (this.m_isLoginSucceed == false)
                                        {
                                            if (e_PublishLog != null)
                                                e_PublishLog(this, new PublishLogEventArgs(m_pTaskData.TaskID, cGlobalParas.LogType.Info, "��¼ʧ�ܣ�ֹͣ����������"
                                                   , false));

                                            StopPublish();
                                            return;
                                        }

                                        //�����Ƿ���ڷ������Ϣ����Ҫ���л�ȡ
                                        if (m_IsGetClass == false)
                                        {
                                            this.m_WebClass = cp.GetWebClass(tName, m_pTaskData.Domain, this.m_Cookie, this.m_globalPara);
                                            m_IsGetClass = true;
                                        }

                                        string rResource = string.Empty;

                                        cp.PublishByWebTemplate(m_pTaskData.PublishData.Columns, dRow, tName, this.m_Cookie,
                                            m_pTaskData.Domain, m_WebClass, m_pTaskData.PublishParas, this.m_globalPara, m_pTaskData.strPlugin, out rResource);

                                        if (m_pTaskData.PIntervalTime != 0)
                                        {
                                            //���òɼ���ʱ
                                            Thread.Sleep(m_pTaskData.PIntervalTime);
                                            if (e_PublishLog != null)
                                                e_PublishLog(this, new PublishLogEventArgs(m_pTaskData.TaskID, cGlobalParas.LogType.Info, "ϵͳ�����˷�����ʱ��ϵͳ��ͣ"
                                                    + m_pTaskData.PIntervalTime + "����", false));
                                        }

                                    }
                                    else if (pType == cGlobalParas.PublishTemplateType.DB)
                                    {
                                        //�����ݿ��ģ�淢��

                                        cp.PublishByDbTemplate(m_pTaskData.PublishData.Columns, dRow, tName, this.PublishType,
                                            m_pTaskData.TemplateDBConn, m_pTaskData.PublishParas);
                                    }

                                    m_PublishedCount++;
                                    TimeSpan usticks = DateTime.Now - mstartTick;

                                    if (e_PublishLog != null && this.TaskData.IsErrorLog == false)
                                        e_PublishLog(this, new PublishLogEventArgs(this.TaskID, cGlobalParas.LogType.Info,
                                            "������ɣ����ݣ�" + dRow[0] + "...", false));

                                    if (e_UpdateState != null)
                                        e_UpdateState(this, new UpdateStateArgs(dRow, this.TaskID, cGlobalParas.PublishResult.Succeed));
                                }
                                catch (System.Exception ex)
                                {
                                    if (e_PublishError != null)
                                    {
                                        e_PublishError(this, new PublishErrorEventArgs(m_pTaskData.TaskID, m_pTaskData.TaskName,
                                           new NetMinerException(ex.Message + " ���ݣ�" + dRow[0] + "...")));

                                        WriteLog(ex.Message + " ���ݣ�" + dRow[0] + "...");

                                    }
                                    if (e_UpdateState != null)
                                        e_UpdateState(this, new UpdateStateArgs(dRow, this.TaskID, cGlobalParas.PublishResult.Fail));
                                    m_PublishErrCount++;
                                    //InsertRow(dRow);
                                }
                                #endregion
                            }
                            else
                            {
                                #region �������ݵ����ݿ����վ��ͨ���û��Ĺ���
                                try
                                {
                                    NetMiner.Core.DB.oDBDeal dd = new Core.DB.oDBDeal(this.m_workPath);
                                    switch (this.PublishType)
                                    {
                                        case cGlobalParas.PublishType.PublishAccess:
                                            //dd.ExportAccess(m_pTaskData.PublishData.Columns, dRow,
                                                //m_pTaskData.DataSource, m_pTaskData.InsertSql, m_pTaskData.DataTableName, m_pTaskData.IsSqlTrue);
                                            m_PublishedCount++;
                                            break;
                                        case cGlobalParas.PublishType.PublishMSSql:
                                            dd.ExportMSSql(m_pTaskData.PublishData.Columns, dRow,
                                                m_pTaskData.DataSource, m_pTaskData.InsertSql, m_pTaskData.DataTableName, m_pTaskData.IsSqlTrue);
                                            m_PublishedCount++;
                                            break;
                                        case cGlobalParas.PublishType.PublishMySql:
                                            dd.ExportMySql(m_pTaskData.PublishData.Columns, dRow,
                                                m_pTaskData.DataSource, m_pTaskData.InsertSql, m_pTaskData.DataTableName, m_pTaskData.IsSqlTrue);

                                            m_PublishedCount++;

                                            break;
                                        case cGlobalParas.PublishType.publishOracle:
                                            dd.ExportOracle(m_pTaskData.PublishData.Columns, dRow,
                                                m_pTaskData.DataSource, m_pTaskData.InsertSql, m_pTaskData.DataTableName, m_pTaskData.IsSqlTrue);
                                            break;

                                    }
                                    TimeSpan usticks = DateTime.Now - mstartTick;

                                    if (e_PublishLog != null && this.TaskData.IsErrorLog == false)
                                        e_PublishLog(this, new PublishLogEventArgs(this.TaskID, cGlobalParas.LogType.Info,
                                            "������ɣ����ݣ�" + ((object[])dRow)[0] + "...", false));

                                    if (e_UpdateState != null)
                                        e_UpdateState(this, new UpdateStateArgs((object[])dRow, this.TaskID, cGlobalParas.PublishResult.Succeed));

                                }
                                catch (System.Exception ex)
                                {
                                    if (e_PublishError != null)
                                    {
                                        e_PublishError(this, new PublishErrorEventArgs(m_pTaskData.TaskID, m_pTaskData.TaskName, ex));
                                    }
                                    if (e_UpdateState != null)
                                        e_UpdateState(this, new UpdateStateArgs((object[])dRow, this.TaskID, cGlobalParas.PublishResult.Fail));
                                    WriteLog(ex.Message);
                                    m_PublishErrCount++;
                                    //InsertRow(dRow);
                                }

                                #endregion
                            }

                        }
                        else
                        {
                            if (dRow == null && UnPublishData.Count == 0)
                            {
                                //�ѳ������ݷ��ص�����
                                m_ThreadsRunning = false;
                            }
                            else
                                Thread.Sleep(SleepFetchTime * 1000);
                        }
                    }
                }
               

                if (e_DoCount!=null)
                    e_DoCount(this, new DoCountEventArgs(m_dCount, m_PublishedCount, m_PublishErrCount, m_CurRowIndex));
              
                
            }


            onComplete();

        }

        /// <summary>
        /// �ж��Ƿ��Ѿ���ɷ������������������������¼��Ĵ���
        /// </summary>
        private void onComplete()
        {
            //���ݷ������
            if (m_dCount == m_PublishedCount + m_PublishErrCount && this.pThreadState!=cGlobalParas.PublishThreadState.Complete)
            {
               
                    this.pThreadState = cGlobalParas.PublishThreadState.Complete;

                    //m_pTaskData.PublishData = m_ErrData.Clone ();
            
                    //��ȡ��ʱ���ݱ�����ļ���
                    string FileName = string.Empty;
                    if (m_pTaskData.IsSaveSingleFile)
                    {
                        FileName = m_pTaskData.TempDataFile;
                    }
                    else
                    {
                        FileName = m_pTaskData.SysSaveTempFileName;
                    }

                    PublishCompletedEventArgs evt1 = new PublishCompletedEventArgs(this.TaskData.TaskID,
                        this.TaskData.TaskName,  m_pTaskData.IsDelTempData, FileName);

                    e_PublishCompleted(this, evt1);

                    if (e_PublishLog != null)
                        e_PublishLog(this, new PublishLogEventArgs(this.TaskID, cGlobalParas.LogType.Info, 
                            "���ݷ������!", false));

            }
           
        }

        ///��ȡ������
        private readonly Object m_DataLock = new Object();

        //��ȡһ������
        //ֻҪ�Ǵ�����������ݣ����������һ��״ֵ̬��
        //����ʶ�𷢲���״̬
        private object[] GetDataRow()
        {
            lock (m_DataLock)
            {
                if (UnPublishData.Count >0)  //&& m_CurRowIndex <m_pData.Rows.Count )
                {
                    try
                    {
                        DataRow r = UnPublishData.Dequeue();
                        return r.ItemArray;
                    }
                    catch (System.Exception)
                    {
                        m_CurRowIndex++;
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        ///����������
        private readonly Object m_ErrDataLock = new Object();

        //private void InsertRow(object dRow)
        //{
        //    lock (m_ErrDataLock)
        //    {
        //        DataRow r = m_ErrData.NewRow();
        //        object[] Row = ((object[])dRow);

        //        for (int j = 0; j < Row.Length; j++)
        //        {
        //            r[j] = Row[j].ToString();
        //        }

        //        m_ErrData.Rows.Add(r);
        //    }
        //}

        //д������־
        private void WriteLog(string strMess)
        {
            //�ڴ˴����Ƿ�д��������ݵ���־������
            if (this.TaskData.IsErrorLog ==true)
            {
                try
                {
                    NetMiner.Core.Log.cSystemLog eLog = new NetMiner.Core.Log.cSystemLog(this.m_workPath);
                    eLog.WriteLog(this.TaskData.TaskName, cGlobalParas.LogType.PublishError, strMess);
                    eLog = null;
                }
                catch(System.Exception ex)
                {
                    //cTool.WriteSystemLog(ex.Message, EventLogEntryType.Error, "SMGatherService");
                    //������

                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NetMiner.Resource;
using NetMiner.Core.gTask.Entity;
using NetMiner.Core.pTask.Entity;

///���ܣ�������������
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace NetMiner.Core.pTask.Entity
{
    [Serializable]
    public class ePublishTaskData
    {
        
        public ePublishTaskData()
        {
            m_PublishData = new DataTable();
            m_TriggerTask = new List<eTriggerTask>();
            m_Headers = new List<eHeader>();
        }

        ~ePublishTaskData()
        {
            m_PublishData.Dispose();
            m_TriggerTask = null;
            m_Headers = null;
        }

        private Int64  m_TaskID;
        public Int64 TaskID
        {
            get { return m_TaskID; }
            set { m_TaskID = value; }
        }

        private string m_TaskName;
        public string TaskName
        {
            get { return m_TaskName; }
            set { m_TaskName = value; }
        }



        private string m_SysSaveTempFileName;
        public string SysSaveTempFileName
        {
            get { return m_SysSaveTempFileName; }
            set { m_SysSaveTempFileName = value; }
        }

        private bool m_IsSaveSingleFile;
        public bool IsSaveSingleFile
        {
            get { return m_IsSaveSingleFile; }
            set { m_IsSaveSingleFile = value; }
        }

        //��ʱ�洢���ݵ��ļ���
        private string m_TempDataFile;
        public string TempDataFile
        {
            get { return m_TempDataFile; }
            set { m_TempDataFile = value; }
        }

        private DataTable m_PublishData;
        public DataTable PublishData
        {
            get { return m_PublishData; }
            set { m_PublishData = value; }
        }

        public int Count
        {
            get { return m_PublishData.Rows.Count; }
        }

        private cGlobalParas.PublishType m_PublishType;
        public cGlobalParas.PublishType PublishType
        {
            get { return m_PublishType; }
            set { m_PublishType = value; }
        }

        private string m_ExportFile;
        public string ExportFile
        {
            get { return m_ExportFile; }
            set { m_ExportFile = value; }
        }

        private string m_DataSource;
        public string DataSource
        {
            get { return m_DataSource; }
            set { m_DataSource = value; }
        }

        #region ����1.2�汾�Ѿ�ȡ�������ݿ��û��������룬����������Ϊ�˼����ϰ汾
        private string m_DataUser;
        public string DataUser
        {
            get { return m_DataUser; }
            set { m_DataUser = value; }
        }

        private string m_DataPwd;
        public string DataPwd
        {
            get { return m_DataPwd; }
            set { m_DataPwd = value; }
        }
        #endregion

        private string m_DataTableName;
        public string DataTableName
        {
            get { return m_DataTableName; }
            set { m_DataTableName = value; }
        }

        //����������Ϊ�˿���֧������1.2�汾��ӵ����񷢲�����
        private string m_InsertSql;
        public string InsertSql
        {
            get { return m_InsertSql; }
            set { m_InsertSql = value; }
        }

        private string m_ExportUrl;
        public string ExportUrl
        {
            get { return m_ExportUrl; }
            set { m_ExportUrl = value; }
        }

        private string m_ExportUrlCode;
        public string ExportUrlCode
        {
            get { return m_ExportUrlCode; }
            set { m_ExportUrlCode = value; }
        }

        private string m_ExportCookie;
        public string ExportCookie
        {
            get { return m_ExportCookie; }
            set { m_ExportCookie = value; }
        }

        private bool m_IsErrorLog;
        public bool IsErrorLog
        {
            get { return m_IsErrorLog; }
            set { m_IsErrorLog = value; }
        }

        private bool m_IsTrigger;
        public bool IsTrigger
        {
            get { return m_IsTrigger; }
            set { m_IsTrigger = value; }
        }

        private string m_TriggerType;
        public string TriggerType
        {
            get { return m_TriggerType; }
            set { m_TriggerType = value; }
        }

        private List<eTriggerTask> m_TriggerTask;
        public List<eTriggerTask> TriggerTask
        {
            get { return m_TriggerTask; }
            set { m_TriggerTask = value; }
        }

        /// <summary>
        /// �����ı���Excel�ļ�ʱ���Ƿ������ͷ
        /// </summary>
        private bool m_IsExportHeader;
        public bool IsExportHeader
        {
            get { return m_IsExportHeader; }
            set { m_IsExportHeader = value; }
        }

        private bool m_IsDelTempData;
        public bool IsDelTempData
        {
            get { return m_IsDelTempData; }
            set { m_IsDelTempData = value; }
        }

        //������ϢΪ1.8�汾���ӣ��жϷ���ʱ�Ƿ���Ҫ�����Զ����header
        private bool m_IsCustomHeader;
        public bool IsCustomHeader
        {
            get { return m_IsCustomHeader; }
            set { m_IsCustomHeader = value; }
        }

        private List<eHeader> m_Headers;
        public List<eHeader> Headers
        {
            get { return m_Headers; }
            set { m_Headers = value; }
        }

        //V3.6�汾���ӣ�����web������ʱ�����ͳɹ���־
        private int m_PIntervalTime;
        public int PIntervalTime
        {
            get { return m_PIntervalTime; }
            set { m_PIntervalTime = value; }
        }

        private string m_PSucceedFlag;
        public string PSucceedFlag
        {
            get { return m_PSucceedFlag; }
            set { m_PSucceedFlag = value; }
        }

        private int m_PublishThread;
        public int PublishThread
        {
            get { return m_PublishThread; }
            set { m_PublishThread = value; }
        }

        //����ģ���������Ϣ
        private string m_TemplateName;
        public string TemplateName
        {
            get { return m_TemplateName; }
            set { m_TemplateName = value; }
        }

        private string m_User;
        public string User
        {
            get { return m_User; }
            set { m_User = value; }
        }

        private string m_Password;
        public string Password
        {
            get { return m_Password; }
            set { m_Password = value; }
        }

        private string m_Domain;
        public string Domain
        {
            get { return m_Domain; }
            set { m_Domain = value; }
        }

        private string m_TemplateDBConn;
        public string TemplateDBConn
        {
            get { return m_TemplateDBConn; }
            set { m_TemplateDBConn = value; }
        }

        private List<NetMiner.Core.pTask.Entity.ePublishData> m_PublishParas;
        public List<NetMiner.Core.pTask.Entity.ePublishData> PublishParas
        {
            get { return m_PublishParas; }
            set { m_PublishParas = value; }
        }

        //V5.0.1����
        private bool m_IsSqlTrue;
        public bool IsSqlTrue
        {
            get { return m_IsSqlTrue; }
            set { m_IsSqlTrue = value; }
        }

        //V5.1����
        private bool m_IsRowFile;
        public bool IsRowFile
        {
            get { return m_IsRowFile; }
            set { m_IsRowFile = value; }
        }

        //V5.2���ӣ����в����������
        private bool m_isPlugin;
        public bool isPlugin
        {
            get { return m_isPlugin; }
            set { m_isPlugin = value; }
        }

        private string m_strPlugin;
        public string strPlugin
        {
            get { return m_strPlugin; }
            set { m_strPlugin = value; }
        }

        private bool m_isEnginPublish;
        public bool isEnginPublish
        {
            get { return m_isEnginPublish; }
            set { m_isEnginPublish = value; }
        }
    }
}

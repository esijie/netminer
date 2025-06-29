using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.Core.pTask.Entity
{
    [Serializable]
    public class ePublishTask
    {
        public ePublishTask()
        {
            m_PublishParas = new List<Entity.ePublishData>();
        }

        ~ePublishTask()
        {
            m_PublishParas = null;
        }

        #region 属性
        private string m_pName;
        public string pName
        {
            get { return m_pName; }
            set { m_pName = value; }
        }

        private int m_ThreadCount;
        public int ThreadCount
        {
            get { return m_ThreadCount; }
            set { m_ThreadCount = value; }
        }

        private bool m_IsDelRepeatRow;
        public bool IsDelRepeatRow
        {
            get { return m_IsDelRepeatRow; }
            set { m_IsDelRepeatRow = value; }
        }

        private cGlobalParas.PublishType m_PublishType;
        public cGlobalParas.PublishType PublishType
        {
            get { return m_PublishType; }
            set { m_PublishType = value; }
        }

        private cGlobalParas.DatabaseType m_DataType;
        public cGlobalParas.DatabaseType DataType
        {
            get { return m_DataType; }
            set { m_DataType = value; }
        }

        private string m_DataSource;
        public string DataSource
        {
            get { return m_DataSource; }
            set { m_DataSource = value; }
        }

        private string m_DataTable;
        public string DataTable
        {
            get { return m_DataTable; }
            set { m_DataTable = value; }
        }

        private string m_InsertSql;
        public string InsertSql
        {
            get { return m_InsertSql; }
            set { m_InsertSql = value; }
        }

        private cGlobalParas.WebCode m_UrlCode;
        public cGlobalParas.WebCode UrlCode
        {
            get { return m_UrlCode; }
            set { m_UrlCode = value; }
        }

        private string m_PostUrl;
        public string PostUrl
        {
            get { return m_PostUrl; }
            set { m_PostUrl = value; }
        }

        private string m_Cookie;
        public string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }

        private string m_SucceedFlag;
        public string SucceedFlag
        {
            get { return m_SucceedFlag; }
            set { m_SucceedFlag = value; }
        }

        private int m_PIntervalTime;
        public int PIntervalTime
        {
            get { return m_PIntervalTime; }
            set { m_PIntervalTime = value; }
        }

        private bool m_IsHeader;
        public bool IsHeader
        {
            get { return m_IsHeader; }
            set { m_IsHeader = value; }
        }

        private string m_Header;
        public string Header
        {
            get { return m_Header; }
            set { m_Header = value; }
        }

        //发布模版的信息
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

        private List<ePublishData> m_PublishParas;
        public List<ePublishData> PublishParas
        {
            get { return m_PublishParas; }
            set { m_PublishParas = value; }
        }

        private bool m_IsSqlTrue;
        public bool IsSqlTrue
        {
            get { return m_IsSqlTrue; }
            set { m_IsSqlTrue = value; }
        }

        private bool m_isEnginePublish;
        public bool isEnginPublish
        {
            get { return m_isEnginePublish; }
            set { m_isEnginePublish = value; }
        }

        #endregion
    }
}

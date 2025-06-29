using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.Core.Radar.Entity
{
    public class eRadar
    {
        public eRadar()
        {
            m_MRule = new List<Entity.eRule>();
            m_Source = new List<Entity.eSource>();
        }

        ~eRadar()
        {
            m_MRule = null;
            m_Source = null;
        }

        #region 属性
        private string m_ID;
        public string ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private cGlobalParas.MonitorState m_MonitorState;
        public cGlobalParas.MonitorState MonitorState
        {
            get { return m_MonitorState; }
            set { m_MonitorState = value; }
        }

        private string m_TaskName;
        public string TaskName
        {
            get { return m_TaskName; }
            set { m_TaskName = value; }
        }

        private List<eSource> m_Source;
        public List<eSource> Source
        {
            get { return m_Source; }
            set { m_Source = value; }
        }

        private int m_MonitorInterval;
        public int MonitorInterval
        {
            get { return m_MonitorInterval; }
            set { m_MonitorInterval = value; }
        }

        private cGlobalParas.MonitorType m_MonitorType;
        public cGlobalParas.MonitorType MonitorType
        {
            get { return m_MonitorType; }
            set { m_MonitorType = value; }
        }

        private List<eRule> m_MRule;
        public List<eRule> MRule
        {
            get { return m_MRule; }
            set { m_MRule = value; }
        }

        private cGlobalParas.MonitorDealType m_DealType;
        public cGlobalParas.MonitorDealType DealType
        {
            get { return m_DealType; }
            set { m_DealType = value; }
        }

        private string m_TableName;
        public string TableName
        {
            get { return m_TableName; }
            set { m_TableName = value; }
        }

        private string m_Sql;
        public string Sql
        {
            get { return m_Sql; }
            set { m_Sql = value; }
        }

        private string m_SavePagePath;
        public string SavePagePath
        {
            get { return m_SavePagePath; }
            set { m_SavePagePath = value; }
        }

        private string m_ReceiveEmail;
        public string ReceiveEmail
        {
            get { return m_ReceiveEmail; }
            set { m_ReceiveEmail = value; }
        }

        private cGlobalParas.WarningType m_WaringType;
        public cGlobalParas.WarningType WaringType
        {
            get { return m_WaringType; }
            set { m_WaringType = value; }
        }

        private string m_WaringEmail;
        public string WaringEmail
        {
            get { return m_WaringEmail; }
            set { m_WaringEmail = value; }
        }

        private cGlobalParas.MonitorInvalidType m_InvalidType;
        public cGlobalParas.MonitorInvalidType InvalidType
        {
            get { return m_InvalidType; }
            set { m_InvalidType = value; }
        }

        private string m_InvalidDate;
        public string InvalidDate
        {
            get { return m_InvalidDate; }
            set { m_InvalidDate = value; }
        }

        #endregion
    }
}

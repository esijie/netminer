using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.Publish.Rule
{
    public class cSqlPara
    {
        private int m_Index;
        public int Index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

        private cGlobalParas.PublishSqlType m_SqlType;
        public cGlobalParas.PublishSqlType SqlType
        {
            get { return m_SqlType; }
            set { m_SqlType = value; }
        }

        private string m_Sql;
        public string Sql
        {
            get { return m_Sql; }
            set { m_Sql = value; }
        }

        private bool m_IsRepeat;
        public bool IsRepeat
        {
            get { return m_IsRepeat; }
            set { m_IsRepeat = value; }
        }

        private string m_PK;
        public string PK
        {
            get { return m_PK; }
            set { m_PK = value; }
        }
    }
}

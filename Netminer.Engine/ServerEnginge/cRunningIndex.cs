using System;
using System.Collections.Generic;
using System.Text;
using System.Data ;
using NetMiner.Data.SqlServer;
using NetMiner.Resource;

namespace NetMiner.Engine.ServerEngine
{
    public class cRunningIndex
    {
        private DataTable m_tData;
        private string m_conn;
        private cGlobalParas.DatabaseType m_dbType;

        public DataTable tData
        {
            get { return m_tData; }
            set { m_tData = value; }
        }

        public cRunningIndex(cGlobalParas.DatabaseType dbType, string conn)
        {
            m_conn =  conn;
            m_dbType = dbType;
            string sql = "select * from SM_TaskList";
            if (m_dbType == cGlobalParas.DatabaseType.MySql)
            {
                m_tData=NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(m_conn, sql);
            }
            else if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                m_tData=NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(m_conn, sql, null);
            }
        }

        ~cRunningIndex()
        {
            m_tData = null;
        }

        public int TaskCount
        {
            get
            {
                if (m_tData == null)
                    return 0;
                else
                    return m_tData.Rows.Count;
            }
        }

        public string GetTaskName(int index)
        {
            string TName = m_tData.Rows[index]["TaskName"].ToString();
            return TName;
        }

        public string GetTaskID(int index)
        {
            string TName = m_tData.Rows[index]["TaskID"].ToString();
            return TName;
        }

        public cGlobalParas.TaskState GetState(int index)
        {
            string s = m_tData.Rows[index]["State"].ToString();
            return (cGlobalParas.TaskState)int.Parse(s);
        }

    }
}

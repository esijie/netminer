using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Data.SqlServer;
using NetMiner.Resource;
using System.Data ;

//分解任务的index
namespace NetMiner.Engine.ServerEngine
{
    public class cSplitIndex
    {
        private string m_conn;
        private DataTable m_tData;
        private cGlobalParas.DatabaseType m_dbType;

        public DataTable tData
        {
            get { return m_tData; }
            set { m_tData = value; }
        }

        public cSplitIndex(cGlobalParas.DatabaseType dbType, string conn)
        {
            m_conn = conn;
            m_dbType = dbType;

            string sql = "select * from SM_SplitTask";
            if (m_dbType == cGlobalParas.DatabaseType.MySql)
            {
                m_tData = NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(m_conn, sql);
            }
            else if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                m_tData = NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(m_conn, sql, null);
            }
        }

        ~cSplitIndex()
        {
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

        public int GetID(int index)
        {
            int tid = int.Parse(m_tData.Rows[index]["ID"].ToString());
            return tid;
        }

        public int GetTID(int index)
        {
            int tid = int.Parse(m_tData.Rows[index]["TID"].ToString());
            return tid;
        }

        public long GetTaskID(int index)
        {
            long TaskID = long.Parse(m_tData.Rows[index]["TaskID"].ToString());
            return TaskID;
        }

        public string GetTaskName(int index)
        {
            string TName = m_tData.Rows[index]["TaskName"].ToString();
            return TName;
        }

        public cGlobalParas.TaskState GetState(int index)
        {
            string s = m_tData.Rows[index]["State"].ToString();
            return (cGlobalParas.TaskState)int.Parse(s);
        }

        public string GetDbFile(int index)
        {
            string s = m_tData.Rows[index]["TaskDataFile"].ToString();
            return s;
        }

        public string GetSavePath(int index)
        {
            string s = m_tData.Rows[index]["sPath"].ToString();
            return s;
        }
        
    }
}

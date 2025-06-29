using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO ;
using NetMiner.Resource;
using MinerDistri.Distributed;
using NetMiner.Data.SqlServer;

///采集引擎的采集任务索引类
namespace NetMiner.Engine.ServerEngine
{
    public class cIndex
    {
        private DataTable m_tData;
        private string m_conn;
        private cGlobalParas.DatabaseType m_dbType;

        public DataTable tData
        {
            get { return m_tData; }
            set { m_tData = value; }
        }

        public cIndex()
        {
        }

        /// <summary>
        /// 从mytask表中加载数据
        /// </summary>
        /// <param name="conn"></param>
        public cIndex(cGlobalParas.DatabaseType dbType, string conn)
        {
            m_conn =  conn;
            m_dbType = dbType;

            string sql = string.Empty;
            if (dbType==cGlobalParas.DatabaseType.MSSqlServer)
                sql = "select * from SM_Mytask order by StartDate";
            else
                sql = "select * from SM_Mytask order by StartDate";

            if (dbType == cGlobalParas.DatabaseType.MySql)
            {
                m_tData=NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(m_conn, sql);
            }
            else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                m_tData=NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(m_conn, sql, null);
            }
        }

        ~cIndex()
        {
            m_tData = null;
        }

        public void IniPending(cGlobalParas.DatabaseType dbType, string conn)
        {
            m_conn = conn;
            m_dbType = dbType;

            string sql = string.Empty;
            if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                sql = "select  * from SM_Mytask where State<>" + (int)cGlobalParas.TaskState.Completed + " order by DATEDIFF(\"ss\",ISNULL(EndDate,'1999-1-1'),getdate()) desc";
            else if (dbType == cGlobalParas.DatabaseType.MySql)
                sql = "select  * from SM_Mytask where State<>" + (int)cGlobalParas.TaskState.Completed + " order by DATEDIFF(IFNULL(EndDate,'1999-1-1') ,NOW()) desc";

            if (dbType == cGlobalParas.DatabaseType.MySql)
            {
                m_tData = NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(m_conn, sql);
            }
            else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                m_tData = NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(m_conn, sql, null);
            }
        }

        public int TaskCount
        {
            get{
                if (m_tData == null)
                    return 0;
                else
                    return m_tData.Rows.Count;
            }
        }

        #region 获取任务数据
        public int GetID(int index)
        {
            int tid = int.Parse(m_tData.Rows[index]["id"].ToString());
            return tid;
        }

        public string GetTaskName(int index)
        {
            string TName = m_tData.Rows[index]["TaskName"].ToString();
            return TName;
        }

        public string GetDbFile(int index)
        {
            string TName = m_tData.Rows[index]["TaskDataFile"].ToString();
            return TName;
        }

        /// <summary>
        /// 分解状态
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public cGlobalParas.SplitTaskState GetTaskDistriState(int index)
        {
            string TName = m_tData.Rows[index]["DistriState"].ToString();
            return (cGlobalParas.SplitTaskState)int.Parse(TName);
        }

        public cGlobalParas.TaskState GetState(int index)
        {
            string s = m_tData.Rows[index]["State"].ToString();
            return (cGlobalParas.TaskState)int.Parse(s);
        }

        public string GetEndDate(int index)
        {
            string eDate = string.Empty;
            if (m_tData.Rows[index]["EndDate"] != null)
                eDate = m_tData.Rows[index]["EndDate"].ToString();

            return eDate;
        }


        public DataTable GetDistriTask(int index)
        {
            return m_tData;
        }

        public string GetSavePath(int index)
        {
            string s = m_tData.Rows[index]["SavePath"].ToString();
            return s;
        }
        #endregion

        /// <summary>
        /// 插入一个新的任务
        /// </summary>
        /// <param name="TaskName"></param>
        /// <returns></returns>
        public bool InsertTask(string TaskName)
        {

            return true;
        }

        /// <summary>
        /// 插入一个分解的任务
        /// </summary>
        /// <returns></returns>
        //public bool InsertSpliteTask(int index, List<cSplitTask> ts)
        //{

        //    string tID = GetID(index).ToString ();

        //    string sql = "insert into SM_SplitTask (TID,TaskID,State,StartDate,EndDate,ClientCode,sPath,TaskName) values (";

        //    for (int i = 0; i < ts.Count; i++)
        //    {
        //        string sql1 = string.Empty;
        //        sql1 = sql + tID + ",0," + (int)cGlobalParas.TaskState.UnStart + ",'','',-1,'" + ts[i].sPath + "','" + ts[i].TaskName + "')";
        //        SQLHelper.ExecuteNonQuery(this.m_conn, sql1);
         
        //    }
        //    return true;
        //}

        /// <summary>
        /// 插入分解任务，当采集任务分解结束后，就进行数据插入
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tID"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        public bool InsertSplitTaskByConn(cGlobalParas.DatabaseType dbType, string conn,int tID, List<cSplitTaskEntity> ts)
        {

            m_conn = conn;
            m_dbType = dbType;

            string sql = "insert into SM_SplitTask (TID,TaskID,State,StartDate,EndDate,ClientCode,sPath,TaskName) values (";

            for (int i = 0; i < ts.Count; i++)
            {
                string sql1 = string.Empty;
                if (m_dbType == cGlobalParas.DatabaseType.MySql)
                {
                    sql1 = sql + tID + ",0," + (int)cGlobalParas.TaskState.UnStart + ",null,null,-1,'" + ts[i].sPath.Replace ("\\","\\\\") + "','" + ts[i].TaskName + "')";

                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql1);
                }
                else if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    sql1 = sql + tID + ",0," + (int)cGlobalParas.TaskState.UnStart + ",'','',-1,'" + ts[i].sPath + "','" + ts[i].TaskName + "')";

                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql1, null);
                }

            }
            return true;
        }

        public bool UpdateSplietState(int index, int disCount, cGlobalParas.SplitTaskState sState)
        {
            try
            {
                string sql = string.Empty;
                sql = "update sm_MyTask set DistriState=" + (int)sState + ",DistriCount=" + disCount + " where ID=" + GetID(index);
                if (m_dbType == cGlobalParas.DatabaseType.MySql)
                {
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
                }
                else if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql, null);
                }
            }
            catch
            {
                return false;
            }

            return true;
        
        }

        public bool UpdateSplietStateByConn(cGlobalParas.DatabaseType dbType, string conn, int tID, int disCount, cGlobalParas.SplitTaskState sState)
        {
            m_conn = conn;
            m_dbType = dbType;

            try
            {
                string sql = string.Empty;
                sql = "update sm_MyTask set DistriState=" + (int)sState + ",DistriCount=" + disCount + " where ID=" + tID;
                if (m_dbType == cGlobalParas.DatabaseType.MySql)
                {
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
                }
                else if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql, null);
                }
            }
            catch
            {
                return false;
            }

            return true;

        }

        /// <summary>
        /// 更新任务的状态，这个是更新MyTask中的状态信息
        /// </summary>
        /// <param name="tID">采集任务的编号</param>
        /// <param name="tState"></param>
        /// <returns></returns>
        public bool UpdateState(int tID, cGlobalParas.TaskState tState)
        {
            try
            {
                string sql = "update sm_MyTask set State=" + (int)tState + " where ID=" + tID;
                if (m_dbType == cGlobalParas.DatabaseType.MySql)
                {
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
                }
                else if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql, null);
                }
            }
            catch
            {
                return false;
            }

            return true;

        }

        public bool UpdateIsSameSubTask(int index, bool isSameSubTask)
        {
            int isS=0;
            if (isSameSubTask == true)
                isS = 1;
            else
                isS = 0;
            try
            {
                string sql = "update sm_mytask set IsSameSubTask=" + isS + " where id=" + GetID(index);
                if (m_dbType == cGlobalParas.DatabaseType.MySql)
                {
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
                }
                else if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql, null);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        
        //public bool UpdateState(string tID, string TaskName, cGlobalParas.TaskState tState)
        //{
        //    try
        //    {
        //        string sql = "update sm_MyTask set State=" + (int)tState + " where ID='" + tID + "'";
        //        SQLHelper.ExecuteNonQuery(this.m_conn, sql);
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //    return true;
        //}
    }
}

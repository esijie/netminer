using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Core.Plan;
using System.Data;
using NetMiner.Gather.Task;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using NetMiner.Core.Log;
using NetMiner.Common;
using NetMiner.Resource;
using NetMiner.Core.Plan.Entity;
using NetMiner.Core.Event;

namespace NetMiner.Gather.Listener
{
    public class cRunTask
    {
        
        Queue<eTaskPlan> m_runningTasks;
        private string m_workPath = string.Empty;

        public cRunTask(string workPath)
        {
            m_workPath = workPath;
            m_runningTasks = new Queue<eTaskPlan>();
        }

        ~cRunTask()
        {
        }

        public void AddTask(eTaskPlan task)
        {
            m_runningTasks.Enqueue(task);
            
            RunTask();
        }

        //执行任务
        private void RunTask()
        {
            cSystemLog cLog = new cSystemLog(m_workPath);

            while (m_runningTasks.Count > 0)
            {
                eTaskPlan tp = m_runningTasks.Dequeue();

                switch (tp.RunTaskType)
                {
                    case cGlobalParas.RunTaskType.SoukeyTask :
                        RunSoukeyTask(tp.RunTaskName);
                        break ;
                    case cGlobalParas.RunTaskType.OtherTask :
                        RunOtherTask(tp.RunTaskName, tp.RunTaskPara);
                        break ;
                    case cGlobalParas.RunTaskType.DataTask :
                        RunDataTask(tp.RunTaskName, tp.RunTaskPara);
                        break;
                }

                //写日志
                //rLog.InsertLog(cGlobalParas.LogType.RunPlanTask,tp.PlanID ,tp.PlanName , (cGlobalParas.RunTaskType)tp.RunTaskType, tp.RunTaskName, tp.RunTaskPara);
                cLog.WriteLog(cGlobalParas.LogType.Info, cGlobalParas.LogClass.RunTask, System.DateTime.Now.ToString(),"执行任务：" + tp.RunTaskName);
            }


            cLog = null;

        }

        //如果是网络矿工任务则需要将信息反馈到主界面，由主界面运行
        //当前支持临时方案，后期会提供专门运行NetMiner.Gather任务的引擎，可以进行
        //后台运行网络矿工任务。

        private void RunDataTask(string DataType, string Para)
        {
            try
            {
                switch (int.Parse(DataType))
                {
                    case (int)cGlobalParas.DatabaseType.Access:
                        ExecuteAccessQuery(Para);
                        break;
                    case (int)cGlobalParas.DatabaseType.MSSqlServer:
                        ExecuteMSSqlQuery(Para);
                        break;
                    case (int)cGlobalParas.DatabaseType.MySql:
                        ExecuteMySqlQuery(Para);
                        break;
                    default:
                        break;
                }

                e_RunTaskLogEvent(this, new cRunTaskLogArgs(DataType, cGlobalParas.LogType.Info, "成功启动触发器，触发器类型：数据库任务", false));

            }
            catch (System.Exception ex)
            {
                if (e_RunTaskLogEvent !=null)
                    e_RunTaskLogEvent (this,new cRunTaskLogArgs (Para ,cGlobalParas.LogType.Error,ex.Message ,false ));
            }
        }

        private void ExecuteAccessQuery(string Para)
        {

            string strconn = Para.Substring(0, Para.IndexOf("Para"));
            string QueryName = Para.Substring(Para.IndexOf("Para=") + 5, Para.Length - Para.IndexOf("Para=") - 5);

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (strconn);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            OleDbCommand com = new OleDbCommand();
            com.Connection = conn;
            com.CommandText = QueryName;
            com.CommandType = CommandType.StoredProcedure;
            try
            {
                int result = com.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            conn.Close();

           
        }

        private void ExecuteMSSqlQuery(string Para)
        {
            string strconn = Para.Substring(0, Para.IndexOf("Para"));
            string QueryName = Para.Substring(Para.IndexOf("Para=")+5, Para.Length - Para.IndexOf("Para=")-5);

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (strconn);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            SqlCommand com = new SqlCommand();
            com.Connection = conn;
            com.CommandText = QueryName;
            com.CommandType = CommandType.StoredProcedure;
            try
            {
                int result = com.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            conn.Close();
        }

        private void ExecuteMySqlQuery(string Para)
        {

            string strconn = Para.Substring(0, Para.IndexOf("Para"));
            string QueryName = Para.Substring(Para.IndexOf("Para=") + 5, Para.Length - Para.IndexOf("Para=") - 5);

            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (strconn);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                throw ex;
                
            }

            MySqlCommand com = new MySqlCommand();
            com.Connection = conn;
            com.CommandText = QueryName;
            com.CommandType = CommandType.StoredProcedure;
            try
            {
                int result = com.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            conn.Close();
          
        }

        private void RunSoukeyTask(string TaskName)
        {
            //获取TaskID
            //oTaskClass tClass = new oTaskClass();
            //string tClassPath=tClass.GetTaskClassPathByName();
            //tClass = null;

            //string tName = tClassPath + "\\" + TaskName.Substring(TaskName.IndexOf("\\"), TaskName.Length - TaskName.IndexOf("\\"));

            //oTask t = new oTask();
            //t.LoadTask(tName);
            //Int64 tID = t.TaskID;
            //t = null;

            //触发运行任务进行任务的执行操作
            e_RunSoukeyTaskEvent(this, new cRunTaskEventArgs(cGlobalParas.MessageType.RunSoukeyTask, TaskName ,""));

            if (e_RunTaskLogEvent != null)
            {
                string tName = TaskName.Substring(TaskName.LastIndexOf("\\")+1, TaskName.Length - TaskName.LastIndexOf("\\")-1);
                e_RunTaskLogEvent(this, new cRunTaskLogArgs(TaskName, cGlobalParas.LogType.Info, "成功启动采集任务：" + tName, false));
            }

        }

        //如果是其他任务，则直接运行
        private void RunOtherTask(string FileName, string Para)
        {
             System.Diagnostics.Process.Start (FileName, Para);

             e_RunTaskLogEvent(this, new cRunTaskLogArgs(FileName, cGlobalParas.LogType.Info, "成功启动触发器，触发器类型：外部可执行文件", false));

        }

        private readonly Object m_eventLock = new Object();

        #region 事件

        public event EventHandler<cRunTaskEventArgs> e_RunSoukeyTaskEvent;
        public event EventHandler<cRunTaskEventArgs> RunSoukeyTaskEvent
        {
            add { lock (m_eventLock) { e_RunSoukeyTaskEvent += value; } }
            remove { lock (m_eventLock) { e_RunSoukeyTaskEvent -= value; } }
        }

        //运行任务的日志事件
        public event EventHandler<cRunTaskLogArgs> e_RunTaskLogEvent;
        public event EventHandler<cRunTaskLogArgs> RunTaskLogEvent
        {
            add { lock (m_eventLock) { e_RunTaskLogEvent += value; } }
            remove { lock (m_eventLock) { e_RunTaskLogEvent -= value; } }
        }

        #endregion
    }
}

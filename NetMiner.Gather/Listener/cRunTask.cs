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

        //ִ������
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

                //д��־
                //rLog.InsertLog(cGlobalParas.LogType.RunPlanTask,tp.PlanID ,tp.PlanName , (cGlobalParas.RunTaskType)tp.RunTaskType, tp.RunTaskName, tp.RunTaskPara);
                cLog.WriteLog(cGlobalParas.LogType.Info, cGlobalParas.LogClass.RunTask, System.DateTime.Now.ToString(),"ִ������" + tp.RunTaskName);
            }


            cLog = null;

        }

        //������������������Ҫ����Ϣ�����������棬������������
        //��ǰ֧����ʱ���������ڻ��ṩר������NetMiner.Gather��������棬���Խ���
        //��̨�������������

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

                e_RunTaskLogEvent(this, new cRunTaskLogArgs(DataType, cGlobalParas.LogType.Info, "�ɹ����������������������ͣ����ݿ�����", false));

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
            //��ȡTaskID
            //oTaskClass tClass = new oTaskClass();
            //string tClassPath=tClass.GetTaskClassPathByName();
            //tClass = null;

            //string tName = tClassPath + "\\" + TaskName.Substring(TaskName.IndexOf("\\"), TaskName.Length - TaskName.IndexOf("\\"));

            //oTask t = new oTask();
            //t.LoadTask(tName);
            //Int64 tID = t.TaskID;
            //t = null;

            //��������������������ִ�в���
            e_RunSoukeyTaskEvent(this, new cRunTaskEventArgs(cGlobalParas.MessageType.RunSoukeyTask, TaskName ,""));

            if (e_RunTaskLogEvent != null)
            {
                string tName = TaskName.Substring(TaskName.LastIndexOf("\\")+1, TaskName.Length - TaskName.LastIndexOf("\\")-1);
                e_RunTaskLogEvent(this, new cRunTaskLogArgs(TaskName, cGlobalParas.LogType.Info, "�ɹ������ɼ�����" + tName, false));
            }

        }

        //���������������ֱ������
        private void RunOtherTask(string FileName, string Para)
        {
             System.Diagnostics.Process.Start (FileName, Para);

             e_RunTaskLogEvent(this, new cRunTaskLogArgs(FileName, cGlobalParas.LogType.Info, "�ɹ����������������������ͣ��ⲿ��ִ���ļ�", false));

        }

        private readonly Object m_eventLock = new Object();

        #region �¼�

        public event EventHandler<cRunTaskEventArgs> e_RunSoukeyTaskEvent;
        public event EventHandler<cRunTaskEventArgs> RunSoukeyTaskEvent
        {
            add { lock (m_eventLock) { e_RunSoukeyTaskEvent += value; } }
            remove { lock (m_eventLock) { e_RunSoukeyTaskEvent -= value; } }
        }

        //�����������־�¼�
        public event EventHandler<cRunTaskLogArgs> e_RunTaskLogEvent;
        public event EventHandler<cRunTaskLogArgs> RunTaskLogEvent
        {
            add { lock (m_eventLock) { e_RunTaskLogEvent += value; } }
            remove { lock (m_eventLock) { e_RunTaskLogEvent -= value; } }
        }

        #endregion
    }
}

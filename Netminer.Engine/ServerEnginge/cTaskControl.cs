using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MinerDistri.Distributed;
using NetMiner.Resource;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using NetMiner.Core.Event;
using NetMiner.Publish;
using NetMiner.Core.pTask.Entity;

///用于对分解采集任务、发布数据进行管理控制

///
namespace NetMiner.Engine.ServerEngine
{
    public class cTaskControl
    {
        private List<Thread> m_SplitTaskList;
        private List<Thread> m_PublishDataList;

        private int m_MaxSplitCount;
        private int m_MaxPublishCount;


        private string m_workPath;

        private cGlobalParas.DatabaseType m_DbType;
        private string m_DbConn;

        private int m_SplitUrls;
        private int m_SplitLevel;

        private string m_TaskPath = cTool.getPrjPath() + "tasks\\";

        private cPublishControl m_PControl;

        //定义事件来获取线程执行的情况
        public delegate void SplitCompletedEventHandler(SplitCompletedEventArgs e);
        public event SplitCompletedEventHandler SplitCompletedEventSend;
        public void OnSplitCompletedEventSend(SplitCompletedEventArgs e)
        {
            if (SplitCompletedEventSend != null)
                this.SplitCompletedEventSend(e);

            
        }

        public cTaskControl(string workPath,cGlobalParas.DatabaseType dbType,string dbConn, int splitUrls,int splitLevel, int maxSplitCount,int maxPublishCount)
        {
            this.m_MaxSplitCount = maxSplitCount;
            this.m_MaxPublishCount = maxPublishCount;
            this.m_workPath = workPath;
            this.m_DbType = dbType;
            this.m_DbConn = dbConn;
            this.m_SplitUrls = splitUrls;
            this.m_SplitLevel = splitLevel;

            m_SplitTaskList = new List<Thread>();
            m_PublishDataList = new List<Thread>();

            m_PControl = new cPublishControl(m_workPath);
            //注册发布事件
            m_PControl.PublishManage.PublishCompleted += this.Publish_Complete;
            m_PControl.PublishManage.PublishError += this.Publish_Error;
            m_PControl.PublishManage.PublishFailed += this.Publish_Failed;
            m_PControl.PublishManage.PublishStop += this.Publish_Stop;


            this.SplitCompletedEventSend += new SplitCompletedEventHandler(on_SplitCompleted);

        }

        ~cTaskControl()
        { }

        #region 基本操作
        public void CreateSplitTask(int tID, string TaskName)
        {
            CheckThread();
            //先判断一下，当前这个任务是否已经在分解中了，如果在，则强制停止，
            //开始分解
            
            for (int i = 0; i < m_SplitTaskList.Count;i++ )
            {
                if (m_SplitTaskList[i].Name==tID.ToString ())
                {
                    StopSplitTask(tID);
                }
            }

            if (m_SplitTaskList.Count < m_MaxSplitCount)
            {
                //可以创建分解任务的线程
                string[] ps = new string[] { tID.ToString(), TaskName };
                Thread t = new Thread(new ParameterizedThreadStart(SplitTaskUrl));
                t.SetApartmentState(ApartmentState.MTA);
                t.IsBackground = true;
                t.Name = tID.ToString();

                m_SplitTaskList.Add(t);
                m_SplitTaskList[m_SplitTaskList.Count - 1].Start(ps);
            }
        }

        public bool CreatePublishData(int tID, long TaskID, string tName, string dFile, string tPath)
        {
            CheckThread();

            //if (m_PublishDataList.Count < m_MaxPublishCount)
            if (m_PControl.PublishManage.ListPublish.Count< m_MaxPublishCount)
            {
                //可以创建分解任务的线程
                string[] ps = new string[] { tID.ToString(), TaskID.ToString(), tName, dFile, tPath };
                //Thread t = new Thread(new ParameterizedThreadStart(MergerSplitData));
                //t.SetApartmentState(ApartmentState.MTA);
                //t.IsBackground = true;
                //t.Name = TaskID.ToString();

                //m_PublishDataList.Add(t);
                //m_PublishDataList[m_PublishDataList.Count - 1].Start(ps);

                MergerSplitData(ps);

                return true;
            }
            else
                return false;
        }

        public bool CreatePublishData(int tID, string dFile, string TaskName, string tName)
        {
            CheckThread();

            if (m_PControl.PublishManage.ListPublish.Count < m_MaxPublishCount)
            {
                //可以创建分解任务的线程
                string[] ps = new string[] { tID.ToString(), dFile, TaskName, tName };
                //Thread t = new Thread(new ParameterizedThreadStart(MergerData));
                //t.SetApartmentState(ApartmentState.MTA);
                //t.IsBackground = true;
                //t.Name = tID.ToString();

                //m_PublishDataList.Add(t);
                //m_PublishDataList[m_PublishDataList.Count - 1].Start(ps);

                MergerData(ps);

                return true;
            }
            else
                return false;
        }

        public void StopSplitTask(int tID)
        {
            for (int i=0;i<this.m_SplitTaskList.Count ;i++)
            {
                if (m_SplitTaskList[i].Name ==tID.ToString ())
                {
                    //停止此线程工作
                    m_SplitTaskList[i].Abort();

                    m_SplitTaskList.Remove(m_SplitTaskList[i]);
                    break;
                }
            }
        }

        public void StopPublishData(long tID)
        {
            for (int i = 0; i < this.m_PublishDataList.Count; i++)
            {
                if (m_PublishDataList[i].Name == tID.ToString())
                {
                    //停止此线程工作
                    m_PublishDataList[i].Abort();
                    m_PublishDataList.Remove(m_PublishDataList[i]);
                    break;
                }
            }
        }

        //检查线程的运行情况，如果有子线程已经停止，则从队列中删除
        public void CheckThread()
        {
            for (int i = 0; i < this.m_SplitTaskList.Count; i++)
            {
                if (m_SplitTaskList[i].ThreadState == System.Threading.ThreadState.Stopped)
                {
                    //停止此线程工作
                    m_SplitTaskList.Remove(m_SplitTaskList[i]);
                    break;
                }
            }

            //for (int i = 0; i < this.m_PublishDataList.Count; i++)
            //{
            //    if (m_PublishDataList[i].ThreadState == System.Threading.ThreadState.Stopped)
            //    {
            //        //停止此线程工作
            //        m_PublishDataList.Remove(m_PublishDataList[i]);
            //        break;
            //    }
            //}

         
        }

        public void Clear(int tID)
        {
            int count= m_SplitTaskList.Where(p => p.Name == tID.ToString()).Count();
            if (count>0)
            {
                StopSplitTask(tID);
            }
        }
        #endregion

        #region 分解采集任务
        private void SplitTaskUrl(object paras)
        {
            string[] ps = (string[])paras;

            int tID = int.Parse(ps[0]);
            string TaskName = ps[1];

            cSplitPlot tManage = new cSplitPlot(this.m_workPath, this.m_DbType, this.m_DbConn, this.m_SplitUrls, 
                this.m_SplitLevel, this.m_TaskPath);
            cGlobalParas.SplitTaskState sState = cGlobalParas.SplitTaskState.UnSplit;
            List<cSplitTaskEntity> ts = null;

            cSplitedTask sTask = new cSplitedTask();
            try
            {
                ts = tManage.SplitTask(TaskName, out sState);

                sTask.tID = tID;
                sTask.sState = sState;
                sTask.Tasks = ts;
            }
            catch(ThreadAbortException te)
            {
                return;
            }
            catch (System.Exception ex)
            {
                sTask.tID = tID;
                sTask.sState = cGlobalParas.SplitTaskState.SplitFaild; ;
                sTask.Tasks = null;

                cTool.WriteSystemLog("分解采集任务失败：" + ex.Message, EventLogEntryType.Error, "SMGatherService");

                return;
            }
            //return sTask;

            //网址分解完成后，处理相应的状态数据，并分解后的数据插入到数据库
            cIndex tIndex = new cIndex();

            if (sTask.sState == cGlobalParas.SplitTaskState.SplitFaild)
            {
                tIndex.UpdateSplietStateByConn(this.m_DbType, this.m_DbConn, sTask.tID, 0, cGlobalParas.SplitTaskState.SplitFaild);
            }
            else if (sTask.Tasks != null && sTask.sState == cGlobalParas.SplitTaskState.Splited)
            {

                //插入分解任务
                tIndex.InsertSplitTaskByConn(this.m_DbType, this.m_DbConn, sTask.tID, sTask.Tasks);

                int sCount = 0;
                if (sTask.Tasks != null)
                    sCount = sTask.Tasks.Count;
                tIndex.UpdateSplietStateByConn(this.m_DbType, this.m_DbConn, sTask.tID, sCount, sTask.sState);
            }
            else
            {
                //无需分解，需将状态处理，否则无法继续采集
                tIndex.UpdateSplietStateByConn(this.m_DbType, this.m_DbConn, sTask.tID, 0, sTask.sState);
            }


            tIndex = null;

            OnSplitCompletedEventSend(new SplitCompletedEventArgs(tID));
        }

        private void on_SplitCompleted(SplitCompletedEventArgs e)
        {
            int tID = e.tID;

            for (int i = 0; i < this.m_SplitTaskList.Count; i++)
            {
                if (m_SplitTaskList[i].Name == tID.ToString())
                {
                    m_SplitTaskList.Remove(m_SplitTaskList[i]);
                    break;
                }
            }
        }
        #endregion

        #region 发布分解后采集任务采集到的数据
        //这个方法用于对分解后的采集任务进行数据入库操作
        private void MergerSplitData(object paras)
        {
            string[] ps = (string[])paras;

            int tID=int.Parse (ps[0]);
            long TaskID=long.Parse (ps[1]);
            string tName=ps[2];
            string dFile=ps[3];
            string tPath = ps[4];

            int count = 0;
            string strTaskID = string.Empty;
            //Match charSetMatch = Regex.Match(dFile, "\\d{18}", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            strTaskID = TaskID.ToString();

            if (dFile != "" && System.IO.File.Exists(dFile))
            {
                //从采集任务的数据文件中，分离出来taskid

                cTaskManage tm = new cTaskManage(this.m_workPath, this.m_DbType, this.m_DbConn, this.m_SplitUrls, this.m_SplitLevel, this.m_TaskPath);
                ePublishTask p = tm.GetSplitPublishRule(tName);

                if (p == null || string.IsNullOrEmpty (p.DataSource ))
                {
                    //表示分解的任务出现了错误，可能是由于用户删除了任务导致
                    //停止当前的线程
                    return;
                }

                if (p.PublishType == cGlobalParas.PublishType.NoPublish)
                {
                    //表示未配置发布数据的规则，不发布数据
                    p = null;
                    
                    return;
                }
                DataTable d = new DataTable();

                try
                {
                    d.ReadXml(dFile);
                }
                catch (System.Exception ex)
                {
                    //读取文件失败
                    cCommon.SetTaskState(this.m_DbType, this.m_DbConn, tID, long.Parse(strTaskID), tName, cGlobalParas.GatherTaskType.DistriGather, cGlobalParas.TaskState.PublishFailed, 0);
                    throw ex;
                }

                count = d.Rows.Count;

                if (count > 0)
                {

                    if (d.Columns[d.Columns.Count - 1].ColumnName != "isPublished")
                    {
                        //d.Columns.Add("isPublished");

                        DataColumn dc = new DataColumn();
                        dc.ColumnName = "isPublished";
                        dc.DataType = typeof(System.String);
                        //dc.DataType = System.Type.GetType("string");
                        dc.AllowDBNull = false;
                        dc.DefaultValue = cGlobalParas.PublishResult.UnPublished.ToString();

                        d.Columns.Add(dc);

                        //for (int i = 0; i < d.Rows.Count; i++)
                        //{
                        //    d.Rows[i]["isPublished"] = cGlobalParas.PublishResult.UnPublished.ToString();
                        //}

                    }

                    if (strTaskID != "")
                    {

                        p.isEnginPublish = true;

                        cPublish pt = new cPublish(this.m_workPath, m_PControl.PublishManage, p, true, d, long.Parse(strTaskID));
                        m_PControl.startPublish(pt);
                    }
                }

            }
            else
            {
                ////如果没有数据，则直接完成
                //SetTaskState(tID, long.Parse(strTaskID), tName, cGlobalParas.GatherTaskType.DistriGather, cGlobalParas.TaskState.Completed, 0);
            }

            //更新分解后任务的采集数量
            string sql = "update SM_SplitTask set GatherCount=" + count + " where TaskID='" + strTaskID + "'";
            if (this.m_DbType == cGlobalParas.DatabaseType.MySql)
            {
                NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(this.m_DbConn, sql);
            }
            else if (this.m_DbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(this.m_DbConn, sql, null);
            }

            //开始处理日志的问题
            string logFile = cTool.getPrjPath() + "log\\G-task" + tName + ".csv";
            if (File.Exists(logFile))
            {
                cLog log = new cLog();
                if (!log.WriteLog(logFile, tName, this.m_DbConn))
                {
                    cTool.WriteSystemLog(logFile + "写入失败！", EventLogEntryType.Error, "SMGatherService");
                }
                log = null;
                //删除日志文件
                File.Delete(logFile);
            }

            if (count == 0)
            {
                cCommon.SetTaskState(this.m_DbType, this.m_DbConn, tID, long.Parse(strTaskID), tName, cGlobalParas.GatherTaskType.DistriGather, cGlobalParas.TaskState.Completed, 0);
            }

            //删除数据文件，因为此时数据文件已经加载到了内存
            //File.Delete(dFile);

            cCommon.MergerImage(this.m_workPath, tPath + "\\" + tName + ".smt", tName);

        }

        //这个方法用于对无需分解的采集任务进行数据入库操作，无需分解，但属于分布式运行采集
        private void MergerData(object paras)
        {
            string[] ps = (string[])paras;
            int tID=int.Parse(ps[0]);
            string dFile=ps[1];
            string TaskName=ps[2];
            string tName = ps[3];

            int count = 0;

            string strTaskID = string.Empty;

            Match charSetMatch = Regex.Match(dFile, "\\d{18}", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            strTaskID = charSetMatch.Groups[0].ToString();

            if (dFile != "" && System.IO.File.Exists(dFile))
            {
                //从采集任务的数据文件中，分离出来taskid

                cTaskManage tm = new cTaskManage(this.m_workPath, this.m_DbType, this.m_DbConn, this.m_SplitUrls, this.m_SplitLevel, this.m_TaskPath);
                ePublishTask p = tm.GetPublishRule(tID);

                if (p == null)
                {
                    //表示分解的任务出现了错误，可能是由于用户删除了任务导致
                    return;
                }
                if (p.PublishType == cGlobalParas.PublishType.NoPublish)
                {
                    //表示未配置发布数据的规则，不发布数据
                    p = null;
                    return;
                }
                DataTable d = new DataTable();
                d.ReadXml(dFile);

                count = d.Rows.Count;

                if (count > 0)
                {

                    if (d.Columns[d.Columns.Count - 1].ColumnName != "isPublished")
                    {
                        //d.Columns.Add("isPublished");

                        DataColumn dc = new DataColumn();
                        dc.ColumnName = "isPublished";
                        dc.DataType = typeof(System.String);
                        //dc.DataType = System.Type.GetType("string");
                        dc.AllowDBNull = false;
                        dc.DefaultValue = cGlobalParas.PublishResult.UnPublished.ToString();

                        d.Columns.Add(dc);

                        //for (int i = 0; i < d.Rows.Count; i++)
                        //{
                        //    d.Rows[i]["isPublished"] = cGlobalParas.PublishResult.UnPublished.ToString();
                        //}

                    }

                    if (strTaskID != "")
                    {
                        cPublish pt = new cPublish(this.m_workPath, m_PControl.PublishManage, p, true, d, long.Parse(strTaskID));

                        m_PControl.startPublish(pt);
                    }
                }
            }
            else
            {
                //如果没有数据，则直接完成
                cCommon.SetTaskState(this.m_DbType, this.m_DbConn, tID, long.Parse(strTaskID), tName, cGlobalParas.GatherTaskType.Normal, cGlobalParas.TaskState.Completed, 0);
            }

            //更新任务的采集数量
            string sql = "update SM_SplitTask set GatherCount=" + count + " where TaskID='" + strTaskID + "'";
            if (this.m_DbType == cGlobalParas.DatabaseType.MySql)
            {
                NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(this.m_DbConn, sql);
            }
            else if (this.m_DbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(this.m_DbConn, sql, null);
            }
            //开始处理日志的问题
            string logFile = cTool.getPrjPath() + "log\\task" + tName + ".csv";
            if (File.Exists(logFile))
            {
                cLog log = new cLog();
                if (!log.WriteLog(logFile, tName, this.m_DbConn))
                {
                    cTool.WriteSystemLog(logFile + "写入失败！", EventLogEntryType.Error, "SMGatherService");
                }
                log = null;

                //删除日志
                File.Delete(logFile);
            }

            if (count == 0)
            {
                cCommon.SetTaskState(this.m_DbType, this.m_DbConn, tID, long.Parse(strTaskID), tName, cGlobalParas.GatherTaskType.DistriGather, cGlobalParas.TaskState.Completed, 0);

            }

            //删除数据文件，因为此时数据文件已经加载到了内存
            //不在删除临时数据了
            //File.Delete(dFile);

            cCommon.MergerImage(this.m_workPath, TaskName, tName);
        }
        #endregion

        #region 发布任务的事件处理
        private void Publish_Complete(object sender, PublishCompletedEventArgs e)
        {

            int tID = GetSplitTID(e.TaskID);

            cCommon. SetTaskState(this.m_DbType,this.m_DbConn, tID, e.TaskID, e.TaskName, cGlobalParas.GatherTaskType.DistriGather, cGlobalParas.TaskState.Completed, 0);

            for (int i = 0; i < this.m_PublishDataList.Count; i++)
            {
                if (m_PublishDataList[i].Name == e.TaskID.ToString())
                {
                    //停止此线程工作
                    m_PublishDataList[i].Abort();
                    m_PublishDataList.Remove(m_PublishDataList[i]);
                    break;
                }
            }
        }

        private void Publish_Stop(object sender, PublishStopEventArgs e)
        {
            int tID = GetSplitTID(e.TaskID);

            string dbFile = GetDBFile(e.TaskID, e.TaskName);
            if (dbFile != "")
                //保存发布的数据到文件
                e.d.WriteXml(dbFile);

            //如果停止，则表示数据未发布
            cCommon.SetTaskState(this.m_DbType,this.m_DbConn, tID, e.TaskID, e.TaskName, cGlobalParas.GatherTaskType.DistriGather, cGlobalParas.TaskState.WaitingPublish, 0);
        }

     

        private void Publish_Failed(object sender, PublishFailedEventArgs e)
        {

        }

        private void Publish_Error(object sender, PublishErrorEventArgs e)
        {
            //在此处理发布错误的问题
            cCommon.InsertErrLog(this.m_DbType,this.m_DbConn, e.TaskName, e.TaskID, e.Error.Message);

        }


        #endregion

        private int GetSplitTID(long TaskID)
        {
            string sql = "select TID from SM_SplitTask where TaskID=" + TaskID;
            object c = null;
            if (this.m_DbType == cGlobalParas.DatabaseType.MySql)
            {
                c = NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(this.m_DbConn, sql);
            }
            else if (this.m_DbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                c = NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(this.m_DbConn, sql, null);
            }
            if (c != null)
                return int.Parse(c.ToString());
            else
                return 0;
        }

        private string GetDBFile(long TaskID, string TaskName)
        {
            string sql = string.Empty;
            if (TaskName.IndexOf("___") > 0)
            {
                sql = "select TaskDataFile from SM_SplitTask where TaskID=" + TaskID;
            }
            else
            {
                sql = "select TaskDataFile from SM_MyTask where TaskID=" + TaskID;
            }

            object c = null;
            if (this.m_DbType == cGlobalParas.DatabaseType.MySql)
            {
                c = NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(this.m_DbConn, sql);
            }
            else if (this.m_DbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                c = NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(this.m_DbConn, sql, null);
            }
            if (c != null && c.ToString() != "")
                return c.ToString();
            else
                return "";
        }
    }
}

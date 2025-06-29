using System;
using System.Collections.Generic;
using NetMiner.Resource;
using System.Data;
using System.IO;
using NetMiner.Common.Tool;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;
using NetMiner.Core.gTask;

namespace MinerDistri.Management
{
    public  class cClient
    {
        private string m_conn=string.Empty ;
        private cGlobalParas.DatabaseType m_dbType;
        private string m_enginePath = string.Empty;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="conn">数据库连接字符串</param>
        /// <param name="engingPath">采集服务器的根目录</param>
        public cClient(cGlobalParas.DatabaseType dbType, string conn,string engingPath)
        {
            m_dbType = dbType;
            this.m_conn = conn;
            this.m_enginePath = engingPath;
        }

        ~cClient()
        {
        }

        /// <summary>
        /// 客户端想服务器通讯
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="IP"></param>
        /// <returns>0-通讯失败；1-通讯正常；2-要求客户端重置任务</returns>
        public int OnlineClient(string Client,string IP)
        {
            int resultID = 0;

            string sql = "select * from SM_ClientOnline where ClientCode='" + Client + "'";
            DataTable d=null;


            if (m_dbType==cGlobalParas.DatabaseType.MSSqlServer)
                 d = NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(m_conn, sql);
            else if (m_dbType==cGlobalParas.DatabaseType.MySql )
            {
                d = NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(m_conn,sql);
            }

            if (d == null || d.Rows.Count == 0)
            {
                sql = "insert into SM_ClientOnline (ClientCode,ClientIP,ActiveDate,State) values ('" + Client + "','" + IP + "','" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + (int)cGlobalParas.ClientState.Online + ")";
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);

                resultID = 1;
            }
            else
            {
                //获取状态
                int iState = int.Parse(d.Rows[0]["State"].ToString());

                sql = "update SM_ClientOnline set ClientIP='" + IP + "',ActiveDate='" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ClientCode='" + Client + "'";
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);

                if (iState == (int)cGlobalParas.ClientState.Reset)
                {
                    resultID = 2;

                    //如果是重置的情况，则返回消息后，更新为正常状态
                    sql="update SM_ClientOnline set State=" + (int)cGlobalParas.ClientState.Online + " where ClientCode='" + Client + "'";
                    if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                        NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                    else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                        NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
                }
                else
                    resultID = 1;
            }

            return resultID;
        }

        /// <summary>
        /// 获取分布式客户端的状态信息，超过5分钟未接收到信息的为失去联系。
        /// </summary>
        /// <returns>返回json格式的额数据</returns>
        public string getClientState()
        {
            DateTime lTimer = System.DateTime.Now.AddMinutes(-5);
            string sql = "select * from sm_clientonline where ActiveDate>'" + lTimer.ToString ("yyyy-MM-dd HH:mm:ss") + "'";
            DataSet ds = null;
            if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                ds = NetMiner.Data.SqlServer.SQLHelper.ExecuteDataSet(m_conn, sql);
            else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                ds = NetMiner.Data.Mysql.SQLHelper.ExecuteDataset(m_conn, sql);
            return JsonConvert.SerializeObject(ds, new DataSetConverter());
        }

        //请求获取一个采集任务
        /// <summary>
        /// 从采集引擎请求一个需要采集的任务，请求任务的时候，
        /// 第一步先请求分解任务；
        /// 第二步请求未分解，但未执行的完整采集任务
        /// 将需要分布式采集的任务最终压缩成一个zip文件，因为此任务有可能包含排重库
        /// </summary>
        /// <param name="ClientCode"></param>
        /// <returns>返回的是一个完整的任务名称（路径+任务名称）</returns>
        public cRemoteTaskEntity GetTask(string ClientCode)
        {
           
            string tName = string.Empty;

            //从分解后的子任务中去客户端需要采集的任务
            string sql = string.Empty;
            DataTable d = null;

            ///禁止执行同一个子任务
            bool isSameSubTask=false ;

            List<cRemoteTaskEntity> rTasks = new List<cRemoteTaskEntity>();

            #region 在此判断是否有正在请求的任务
            //先判断此任务是否存在request状态的任务，如果有，表明客户端请求了此任务，但没有下载，
            //则直接获取此任务
            sql = "select * from SM_MyTask where State=" + (int)cGlobalParas.TaskState.Request + " and Performer='" + ClientCode + "'";
            if (m_dbType == cGlobalParas.DatabaseType.MySql)
            {
                d= NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(m_conn, sql);
            }
            else if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                d = NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(m_conn, sql);
            }

            if (d != null && d.Rows.Count > 0)
            {
                cRemoteTaskEntity crt = new cRemoteTaskEntity();
                crt.GatherTaskType = (int)cGlobalParas.GatherTaskType.DistriTask;
                crt.TaskName = d.Rows[0]["TaskName"].ToString();
                crt.ID = int.Parse(d.Rows[0]["ID"].ToString());
                crt.TID = int.Parse(d.Rows[0]["ID"].ToString());
                crt.TaskFileName = d.Rows[0]["SavePath"].ToString();
                crt.isSubRun = d.Rows[0]["IsSameSubTask"].ToString() == "0" ? false : true;

                tName = crt.TaskFileName;
                tName = CompressZIPFile(tName);
                crt.TaskFileName = tName;

                return crt;
            }

            d.Dispose();
             
            //继续请求分布式任务
            sql = "select sm_splittask.*,sm_MyTask.IsSameSubTask from sm_splittask inner join sm_mytask on sm_splittask.TID=sm_mytask.ID"
                  + " where sm_splittask.State=" + (int)cGlobalParas.TaskState.Request + " and sm_splittask.ClientCode='" + ClientCode + "'";
            if (m_dbType == cGlobalParas.DatabaseType.MySql)
            {
                d = NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(m_conn, sql);
            }
            else if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                d = NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(m_conn, sql);
            }
            if (d != null && d.Rows.Count > 0)
            {
                cRemoteTaskEntity crt = new cRemoteTaskEntity();
                crt.GatherTaskType = (int)cGlobalParas.GatherTaskType.DistriSplitTask;
                crt.TaskName = d.Rows[0]["TaskName"].ToString();
                crt.ID = int.Parse(d.Rows[0]["ID"].ToString());
                crt.TID = int.Parse(d.Rows[0]["TID"].ToString());
                crt.TaskFileName = d.Rows[0]["sPath"].ToString() + "\\" + d.Rows[0]["TaskName"].ToString() + ".smt";
                crt.isSubRun = d.Rows[0]["IsSameSubTask"].ToString() == "0" ? false : true;

                tName = crt.TaskFileName;
                tName = CompressZIPFile(tName);
                crt.TaskFileName = tName;

                return crt;
            }
            #endregion

            d.Dispose();

            int taskIndex = 0;

            #region 从队列中获取任务
            //先取分布式任务
            if (m_dbType == cGlobalParas.DatabaseType.MySql)
            {
                sql = "select a.*,b.IsSameSubTask from SM_SplitTask a,sm_mytask b where a.TID=b.ID and a.State=" + (int)cGlobalParas.TaskState.UnStart;
                d = NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(m_conn, sql);
            }
            else if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                sql = "select sm_splittask.*,sm_mytask.IsSameSubTask from SM_SplitTask INNER JOIN sm_mytask on sm_splittask.TID=sm_mytask.ID where sm_splittask.State=" + (int)cGlobalParas.TaskState.UnStart;
                d = NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(m_conn, sql);
            }

            //将分布式任务压入队列
            if (d != null)
            {
                for (int i = 0; i < d.Rows.Count; i++)
                {
                    string ID = d.Rows[i]["ID"].ToString();
                    string tID = d.Rows[i]["TID"].ToString();

                    cRemoteTaskEntity rt = new cRemoteTaskEntity();

                    rt.GatherTaskType = (int)cGlobalParas.GatherTaskType.DistriSplitTask;
                    rt.TaskName = d.Rows[i]["TaskName"].ToString();

                    //tName = d.Rows[i]["sPath"].ToString() + "\\" + d.Rows[i]["TaskName"].ToString() + ".smt";
                    rt.ID = int.Parse(d.Rows[i]["ID"].ToString());
                    rt.TID = int.Parse (tID);
                    rt.TaskFileName = d.Rows[i]["sPath"].ToString() + "\\" + d.Rows[i]["TaskName"].ToString() + ".smt";
                    rt.isSubRun = d.Rows[i]["IsSameSubTask"].ToString() == "0" ? false : true;

                    rTasks.Add(rt);
                }
            }

            d.Dispose();

            //再开始获取可以分布式执行的独立任务
            if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                sql = "select * from SM_MyTask where State="
                    + (int)cGlobalParas.TaskState.UnStart
                    + " and DistriState=" + (int)cGlobalParas.SplitTaskState.WithoutSplit
                    + " order by DATEDIFF(\"ss\",ISNULL(EndDate,'1999-1-1'),getdate()) desc";

                d = NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(m_conn, sql);
            }
            else if (m_dbType == cGlobalParas.DatabaseType.MySql)
            {
                sql = "select * from SM_MyTask where State="
                    + (int)cGlobalParas.TaskState.UnStart
                    + " and DistriState=" + (int)cGlobalParas.SplitTaskState.WithoutSplit
                    + " order by DATEDIFF(IFNULL(EndDate,'1999-1-1'),now()) desc ";

                d = NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(m_conn, sql);
            }

            if (d != null && d.Rows.Count> 0)
            {

                for (int i = 0; i < d.Rows.Count; i++)
                {
                    cRemoteTaskEntity rt = new cRemoteTaskEntity();

                    rt.GatherTaskType = (int)cGlobalParas.GatherTaskType.DistriTask;
                    rt.TaskName = d.Rows[i]["TaskName"].ToString();

                    rt.ID = int.Parse(d.Rows[i]["ID"].ToString());
                    rt.TID = int.Parse(d.Rows[i]["ID"].ToString());
                    rt.TaskFileName = d.Rows[i]["SavePath"].ToString() ;
                    rt.isSubRun = d.Rows[i]["IsSameSubTask"].ToString() == "0" ? false : true;

                    rTasks.Add(rt);
                }
            }

            if (rTasks == null || rTasks.Count == 0)
            {
                return null;
            }

            #endregion


            #region 判断客户端是否取出同一个任务下的子任务，且此任务不允许重复请求
            //先把此客户端的数据提取出来
            //根据客户端来判断是否已经请求了相同的任务
            sql = "select TID,TaskName from sm_splittask where ClientCode='" + ClientCode + "' and State<>"
                + (int)cGlobalParas.TaskState.UnStart + " and State<>" + (int)cGlobalParas.TaskState.WaitingPublish
                + " and State<>" + (int)cGlobalParas.TaskState.Completed;

            DataTable d2 = null;

            if (m_dbType == cGlobalParas.DatabaseType.MySql)
            {
                d2 = NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(m_conn, sql);
            }
            else if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                d2 = NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(m_conn, sql);
            }

            Hashtable cHt = new Hashtable();
            for (int i = 0; i < d2.Rows.Count; i++)
            {
                if (!cHt.ContainsKey(d2.Rows[i]["TID"].ToString()))
                    cHt.Add(d2.Rows[i]["TID"].ToString(), d2.Rows[i]["TID"].ToString());
            }

            cRemoteTaskEntity rTask = new cRemoteTaskEntity ();

            //是否取出任务
            bool isGetTask = false;

            if (cHt.Count > 0)
            {
                //如果此客户端有任务，在进行判断，如果没有，则不用判断，直接获取任务
                for (int index = 0; index < rTasks.Count; index++)
                {
                    rTask = rTasks[index];

                    if (rTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriSplitTask)
                    {

                        if (rTask.isSubRun == true)  // d1.Rows[0]["isSameSubTask"].ToString() == "1")
                        {
                            //禁止一个客户端执行多个相同的任务
                            isSameSubTask = true;
                            bool isExist = true;

                            foreach (DictionaryEntry de in cHt)
                            {
                                if (de.Key.ToString() != rTask.TID.ToString())
                                {
                                    isExist = false;
                                    break;
                                }
                            }

                            if (isExist == false)
                            {
                                isGetTask = true;
                                break;
                            }

                        }
                        else
                        {
                            isSameSubTask = false;
                            isGetTask = true;
                            break;
                        }
                    }
                    else
                    {
                        isGetTask = true;
                        break;
                    }
                }
            }
            else
            {
                isGetTask = true;
                if (rTasks.Count > 0)
                    rTask = rTasks[0];
                else
                    return null;
            }

            #endregion

            if (isGetTask == false)
                return null;

            #region 修改已经成功获取任务的状态
            //获取到任务成功之后，马上进行状态修改
            if (rTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriSplitTask)
            {
                UpdateSplitState(rTask.ID, ClientCode, cGlobalParas.TaskState.Request, "");
                //fName=fName + "\\" + tName + ".smt";
            }
            else if (rTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriTask)
            {
                UpdateState(rTask.ID, ClientCode, cGlobalParas.TaskState.Request, "");
            }
            #endregion

            tName = rTask.TaskFileName;

            tName = CompressZIPFile(tName);

            //返回的是一个压缩文件
            rTask.TaskFileName = tName;

            return rTask;
        }

        private string CompressZIPFile(string tName)
        {
            #region 获取到任务后，开始将需要下载的任务进行压缩
            Dictionary<string,int> fNames = new Dictionary<string, int>();
            if (tName != "")
            {
                fNames.Add(tName, (int)cGlobalParas.FileType.File);
                oTask t = new oTask(m_enginePath);
                t.LoadTask(tName);
                if (t.TaskEntity.IsUrlNoneRepeat == true)
                {
                    //表示此采集任务需要进行排重处理
                    string urlDB = this.m_enginePath + "\\urls\\远程-" + Path.GetFileNameWithoutExtension(tName) + ".db";
                    if (File.Exists(urlDB))
                        fNames.Add(urlDB, (int)cGlobalParas.FileType.File);
                }

                //开始压缩文件
                cZipCompression zCompress = new cZipCompression();
                string tmpFile = this.m_enginePath + "\\tmp\\Down\\" + Path.GetFileNameWithoutExtension(tName) + ".zip";
                if (File.Exists(tmpFile))
                    File.Delete(tmpFile);

                zCompress.CompressZIP(fNames, tmpFile);
                zCompress = null;

                tName = tmpFile;
            }
            #endregion

            return tName;
        }

        /// <summary>
        /// 更新分解任务的状态
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="ClientCode"></param>
        /// <param name="tState"></param>
        /// <param name="dFile"></param>
        /// <returns></returns>
        public bool UpdateSplitState(int tsID, string ClientCode, cGlobalParas.TaskState tState, string dFile)
        {
            string sql = string.Empty;
            if (tState == cGlobalParas.TaskState.RemoteRunning || tState == cGlobalParas.TaskState.Request)
            {
                sql = "update SM_SplitTask set State=" + (int)tState + ",ClientCode='" + ClientCode + "',StartDate='" + System.DateTime.Now.ToString() + "' where ID='" + tsID + "'";
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);

                //修改主采集任务为正在运行
                sql = "select TID from SM_SplitTask where id='" + tsID + "'";
                string tID = string.Empty;

                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    tID=NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(m_conn, sql).ToString ();
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    tID=NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(m_conn, sql).ToString ();

                sql = "update SM_MyTask set State=" + (int)cGlobalParas.TaskState.Running + " where ID=" + tID;
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);

            }
            else if (tState == cGlobalParas.TaskState.Completed || tState == cGlobalParas.TaskState.Stopped)
            {
                sql = "update SM_SplitTask set State=" + (int)tState + ",EndDate='" + System.DateTime.Now.ToString() + "' where ID='" + tsID + "'";
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
            }
            else if (tState == cGlobalParas.TaskState.WaitingPublish)
            {
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    sql = "update SM_SplitTask set State=" + (int)tState + ",TaskDataFile='" + dFile + "' where ID='" + tsID + "'";

                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                }
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                {
                    sql = "update SM_SplitTask set State=" + (int)tState + ",TaskDataFile='" + dFile.Replace("\\","\\\\") + "' where ID='" + tsID + "'";

                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
                }
            }
            else
            {
                sql = "update SM_SplitTask set State=" + (int)tState + ",ClientCode='" + ClientCode + "' where ID='" + tsID + "'";
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
            }

            //插入日志
            sql = "select ID from SM_SplitTask where ID='" + tsID + "'";
            object c = null;

            if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                c=NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(m_conn, sql);
            else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                c=NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(m_conn, sql);

            if (c != null)
            {
                int sID = int.Parse(c.ToString());
                sql = "insert into SM_SplitRunningLog(SID,State,GatherCount,client,UpdateDate) values (" + sID + "," + (int)tState
                   + ",'0','" + ClientCode + "','" + System.DateTime.Now.ToString() + "')";
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
            }

            return true;
        }

        /// <summary>
        /// 这个方法的状态更新，仅用于采集任务分布式执行时的更新
        /// 其他状态的维护更新不能使用此方法
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="ClientCode"></param>
        /// <param name="tState"></param>
        /// <param name="dFile"></param>
        /// <returns></returns>
        public bool UpdateState(int tID, string ClientCode, cGlobalParas.TaskState tState, string dFile)
        {
            string sql = string.Empty;

            if (tState == cGlobalParas.TaskState.Request)
            {
                sql = "update SM_MyTask set State=" + (int)cGlobalParas.TaskState.Request + ",Performer='" + ClientCode + "' where ID='" + tID + "'";
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
            }
            else if (tState == cGlobalParas.TaskState.Running)
            {
                sql = "update SM_MyTask set State=" + (int)cGlobalParas.TaskState.Running + ",StartDate='"
                    + System.DateTime.Now.ToString() + "',Performer='" + ClientCode + "' where ID='" + tID + "'";
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
            }
            else if (tState == cGlobalParas.TaskState.Completed || tState == cGlobalParas.TaskState.Stopped)
            {
                sql = "update SM_MyTask set State=" + (int)tState + ",EndDate='" + System.DateTime.Now.ToString()
                    + "' where ID='" + tID + "'";
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
            }
            else if (tState == cGlobalParas.TaskState.WaitingPublish)
            {
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    sql = "update SM_MyTask set State=" + (int)tState + ",TaskDataFile='" + dFile + "' where ID='" + tID + "'";

                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                }
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                {
                    sql = "update SM_MyTask set State=" + (int)tState + ",TaskDataFile='" + dFile.Replace("\\","\\\\") + "' where ID='" + tID + "'";

                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
                }
            }
            else
            {
                sql = "update SM_MyTask set State=" + (int)tState + " where ID='" + tID + "'";
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
            }



            return true;
        }

        /// <summary>
        /// 更新TaskID
        /// </summary>
        /// <param name="tID"></param>
        /// <param name="TaskID"></param>
        public void UpdateSplitTaskID(int tID, Int64 TaskID)
        {
            string TaskName = string.Empty;

            string sql = "update SM_SplitTask set State=" + (int)cGlobalParas.TaskState.RemoteRunning + ", TaskID=" + TaskID + " where ID=" + tID;
            if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
            else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);

            sql="select TaskName from SM_SplitTask where ID=" + tID;
            object c = null;

            if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                c=NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(m_conn, sql);
            else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                c=NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(m_conn, sql);

            if (c != null && c.ToString() != "")
            {
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    TaskName=NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(m_conn, sql).ToString ();
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    TaskName=NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(m_conn, sql).ToString ();
            }

            InsertStartLog(tID, TaskID, TaskName);
        }

        /// <summary>
        /// 单独执行的分布式采集任务，因为不存在分解的情况，所以不会自动插入SM_SplitTask数据，因此
        /// 在客户端向服务器传递TaskID的时候，进行数据插入
        /// </summary>
        /// <param name="tID"></param>
        /// <param name="TaskID"></param>
        public void UpdateTaskID(int tID, Int64 TaskID,string ClientCode)
        {
            string sql = "select TaskName from SM_MyTask where ID=" + tID;
            object c = null;

            if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                c = NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(m_conn, sql).ToString();
            else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                c = NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(m_conn, sql).ToString();
            if (c != null && c.ToString() != "")
            {
                string TaskName = c.ToString();

                sql = "insert into SM_SplitTask (TID,TaskID,State,StartDate,ClientCode,TaskName) values ("
                    + tID + "," + TaskID + "," + (int)cGlobalParas.TaskState.RemoteRunning + ",'" + System.DateTime.Now.ToString()
                        + "','" + ClientCode + "','" + TaskName + "')";
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);

                //修改主采集任务为正在运行
                sql = "update SM_MyTask set State=" + (int)cGlobalParas.TaskState.RemoteRunning + " where ID=" + tID;
                if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
                else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);

                InsertStartLog(tID, TaskID, TaskName);
            }
            else
            {
                InsertErrLog(tID, TaskID, "", "任务已经被删除");
            }
        }

        /// <summary>
        /// 插入分布式采集任务运行的日志，属于insert类型
        /// </summary>
        private void InsertStartLog(int tID,Int64 TaskID,string TaskName)
        {
            //在此插入日志
            string sql = "insert into SM_TaskLog(TID,TaskID,TaskName,StartDate) values (" + tID + "," + TaskID
                + ",'" + TaskName + "','" + System.DateTime.Now.ToString() + "')";
            if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);
            else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);
        }

        private void InsertErrLog(int tID, Int64 TaskID, string TaskName,string ErrMessage)
        {
            if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                string sql = "insert into SM_TaskLog(TID,TaskID,TaskName,StartDate,EndDate,GatherCount) values (" + tID + "," + TaskID
                    + ",'" + TaskName + "','" + System.DateTime.Now.ToString() + "','" + System.DateTime.Now.ToString() + "',0)";
                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql);

                //插入错误的日志
                sql = "insert into SM_RunningLog(TaskID,TaskName,Type,Message,LogDate) values (@TaskID,@TaskName,@Type,@Message,@LogDate)";

                try
                {
                    SqlParameter[] parameters = {
                                       new SqlParameter("@TaskID" , SqlDbType.Decimal),
                                       new SqlParameter("@TaskName" , SqlDbType.VarChar,250),
                                       new SqlParameter("@Type" , SqlDbType.Int),
                                       new SqlParameter("@Message" , SqlDbType.VarChar,1000),
                                       new SqlParameter("@LogDate" , SqlDbType.DateTime)
                                        };
                    parameters[0].Value = TaskID;
                    parameters[1].Value = TaskName;
                    parameters[2].Value = (int)cGlobalParas.LogType.Error;
                    parameters[3].Value = ErrMessage;
                    parameters[4].Value = System.DateTime.Now.ToString();

                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(m_conn, sql, parameters);



                }
                catch (System.Exception ex)
                {

                }
            }
            else if (m_dbType == cGlobalParas.DatabaseType.MySql)
            {
                string sql = "insert into SM_TaskLog(TID,TaskID,TaskName,StartDate,EndDate,GatherCount) values (" + tID + "," + TaskID
                    + ",'" + TaskName + "','" + System.DateTime.Now.ToString() + "','" + System.DateTime.Now.ToString() + "',0)";
                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql);

                //插入错误的日志
                sql = "insert into SM_RunningLog(TaskID,TaskName,Type,Message,LogDate) values "
                        + " (?TaskID,?TaskName,?Type,?Message,?LogDate)";

                try
                {
                    MySqlParameter [] parameters = {
                                       new MySqlParameter("@TaskID" ,MySqlDbType.Decimal),
                                       new MySqlParameter("@TaskName" , MySqlDbType.VarChar,250),
                                       new MySqlParameter("@Type" , MySqlDbType.Int16),
                                       new MySqlParameter("@Message" , MySqlDbType.VarChar,1000),
                                       new MySqlParameter("@LogDate" , MySqlDbType.DateTime)
                                        };
                    parameters[0].Value = TaskID;
                    parameters[1].Value = TaskName;
                    parameters[2].Value = (int)cGlobalParas.LogType.Error;
                    parameters[3].Value = ErrMessage;
                    parameters[4].Value = System.DateTime.Now.ToString();

                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(m_conn, sql, parameters);



                }
                catch (System.Exception ex)
                {

                }
            }
        }
    }
}

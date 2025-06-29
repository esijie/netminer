
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Resource;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.IO;

namespace NetMiner.Engine.ServerEngine
{
    public class cCommon
    {
        public static void InsertErrLog(cGlobalParas.DatabaseType DbType, string DbConn, string TaskName, Int64 taskID, string mess)
        {

            string sql = string.Empty;

            try
            {
                if (DbType == cGlobalParas.DatabaseType.MySql)
                {
                    sql = "insert into SM_RunningLog(TaskID,TaskName,Type,Message,LogDate) values "
                        + " (?TaskID,?TaskName,?Type,?Message,?LogDate)";

                    MySql.Data.MySqlClient.MySqlParameter[] parameters = {
                                       new MySql.Data.MySqlClient.MySqlParameter("@TaskID" , MySqlDbType.Decimal),
                                       new MySql.Data.MySqlClient.MySqlParameter("@TaskName" , MySqlDbType.VarChar,250),
                                       new MySql.Data.MySqlClient.MySqlParameter("@Type" , MySqlDbType.Int32),
                                       new MySql.Data.MySqlClient.MySqlParameter("@Message" , MySqlDbType.VarChar,1000),
                                       new MySql.Data.MySqlClient.MySqlParameter("@LogDate" , MySqlDbType.DateTime)
                                        };
                    parameters[0].Value = taskID;
                    parameters[1].Value = TaskName;
                    parameters[2].Value = (int)cGlobalParas.LogType.Error;
                    parameters[3].Value = mess;
                    parameters[4].Value = System.DateTime.Now.ToString();

                    NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(DbConn, sql, parameters);
                }
                else if (DbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    sql = "insert into SM_RunningLog(TaskID,TaskName,Type,Message,LogDate) values (@TaskID,@TaskName,@Type,@Message,@LogDate)";
                    SqlParameter[] parameters = {
                                       new SqlParameter("@TaskID" , SqlDbType.Decimal),
                                       new SqlParameter("@TaskName" , SqlDbType.VarChar,250),
                                       new SqlParameter("@Type" , SqlDbType.Int),
                                       new SqlParameter("@Message" , SqlDbType.VarChar,1000),
                                       new SqlParameter("@LogDate" , SqlDbType.DateTime)
                                        };
                    parameters[0].Value = taskID;
                    parameters[1].Value = TaskName;
                    parameters[2].Value = (int)cGlobalParas.LogType.Error;
                    parameters[3].Value = mess;
                    parameters[4].Value = System.DateTime.Now.ToString();

                    NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(DbConn, sql, parameters);
                }



            }
            catch (System.Exception ex)
            {
                //如果插入日志错误，必须马上处理
                cTool.WriteSystemLog(ex.Message, EventLogEntryType.Error, "SMGatherService");
            }
        }

        /// <summary>
        /// 合并采集后的图片，按照任务名称合并，如果多个任务同名，则都会放在同一路径下
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="tName"></param>
        public static void MergerImage(string workPath, string TaskName, string tName)
        {
            //处理下载的图片
            NetMiner.Core.gTask.oTask t1 = new NetMiner.Core.gTask.oTask(workPath);
            t1.LoadTask(TaskName);

            bool isImage = false;
            string oldPath = string.Empty;
            string newPath = string.Empty;

            for (int i = 0; i < t1.TaskEntity.WebpageCutFlag.Count; i++)
            {
                if (t1.TaskEntity.WebpageCutFlag[i].DataType ==cGlobalParas.GDataType.Picture
                    || t1.TaskEntity.WebpageCutFlag[i].DataType == cGlobalParas.GDataType.File)
                {
                    isImage = true;
                    newPath = t1.TaskEntity.WebpageCutFlag[i].DownloadFileSavePath;

                    if (newPath == "")
                    {
                        //表示存储在默认路径
                        if (tName.IndexOf("___") > 0)
                        {
                            string tName1 = tName.Substring(0, tName.IndexOf("___"));
                            newPath = workPath + "data\\" + tName1 + "_file";
                        }
                        else
                        {
                            newPath = workPath + "data\\" + tName + "_file";
                        }
                    }
                    break;
                }
            }
            t1 = null;


            if (isImage == true)
            {
                oldPath = workPath + "tmp\\upload\\" + tName + "\\" + tName + "_file";

                //判断此目录是否存在，如果存在表示有图片
                if (Directory.Exists(oldPath))
                {
                    //Directory.Move(oldPath, newPath);
                    DirectoryInfo oldDir = new DirectoryInfo(oldPath);
                    DirectoryInfo newDir = new DirectoryInfo(newPath);
                    NetMiner.Common.ToolUtil.CopyDirectory(oldDir, newDir);
                }

            }

            if (oldPath != "" && Directory.Exists(oldPath))
                //当文件复制完毕后，清除图片目录
                Directory.Delete(oldPath, true);
        }

        /// <summary>
        /// 设置采集任务的状态，包括独立运行的任务和分解任务，同时记录日志，同时维护SM_MyTask中的状态
        /// </summary>
        /// <param name="tID">如果是独立运行的任务，则为SM_MyTask中的ID,如果是分解的采集任务则为SM_SplitTask中的ID</param>
        /// <param name="sState">任务分解状态，用于区分是独立任务还是分解任务</param>
        /// <param name="tState"></param>
        public static void SetTaskState(cGlobalParas.DatabaseType dbType,string dbConn, int tID, long TaskID, string TaskName,
            cGlobalParas.GatherTaskType gState, cGlobalParas.TaskState tState, int Count)
        {
            string sql = string.Empty;

            //if (TaskName.IndexOf("___") > 0)
            if (gState == cGlobalParas.GatherTaskType.DistriGather)
            {
                switch (tState)
                {
                    case cGlobalParas.TaskState.Completed:
                        sql = "update SM_SplitTask set State=" + (int)tState + ",EndDate='" + System.DateTime.Now.ToString() + "' where TaskID='" + TaskID + "'";
                        if (dbType == cGlobalParas.DatabaseType.MySql)
                        {
                            NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                        }
                        else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                        {
                            NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                        }
                        JudgeTaskComplete(dbType,dbConn, tID, TaskID, TaskName);
                        break;
                    default:
                        sql = "update SM_SplitTask set State=" + (int)tState + " where TaskID='" + TaskID + "'";
                        if (dbType == cGlobalParas.DatabaseType.MySql)
                        {
                            NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                        }
                        else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                        {
                            NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                        }
                        break;
                }

            }
            else if (gState == cGlobalParas.GatherTaskType.Normal)
            {
                switch (tState)
                {
                    case cGlobalParas.TaskState.Running:
                        InsertLog(dbType,dbConn, tID, TaskID, 0, System.DateTime.Now.ToString(), 0);
                        sql = "update SM_MyTask set State=" + (int)cGlobalParas.TaskState.Running + " where ID=" + tID;
                        if (dbType == cGlobalParas.DatabaseType.MySql)
                        {
                            NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                        }
                        else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                        {
                            NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                        }
                        break;
                    case cGlobalParas.TaskState.Completed:

                        //未分解任务
                        //每次执行完毕，必须更新SM_MyTask中的EndDate，这样做的目的是可以让定时计划监控
                        sql = "update SM_MyTask set State=" + (int)tState + ",EndDate='" + System.DateTime.Now.ToString() + "' where ID='" + tID + "'";
                        if (dbType == cGlobalParas.DatabaseType.MySql)
                        {
                            NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                        }
                        else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                        {
                            NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                        }

                        InsertLog(dbType,dbConn, tID, TaskID, Count, System.DateTime.Now.ToString(), 2);

                        break;
                    default:
                        //未分解任务
                        sql = "update SM_MyTask set State=" + (int)tState + " where ID='" + tID + "'";
                        if (dbType == cGlobalParas.DatabaseType.MySql)
                        {
                            NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                        }
                        else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                        {
                            NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                        }
                        break;
                }

            }
        }

        public static void JudgeTaskComplete(cGlobalParas.DatabaseType dbType,string dbConn, int tID, long TaskID, string TaskName)
        {
            //tName = tName.Substring (0, tName.IndexOf("___"));
            //string sql = "select ID from SM_SplitTask where ID='" + tsID + "'";
            //int tID = int.Parse (SQLHelper.ExecuteScalar(g_DbConn, sql).ToString());

            string sql = "select count(*) from SM_SplitTask where TID=" + tID + " and State<>" + (int)cGlobalParas.TaskState.Completed;
            object c1 = null;
            if (dbType == cGlobalParas.DatabaseType.MySql)
            {
                c1 = NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(dbConn, sql);
            }
            else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                c1 = NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(dbConn, sql, null);
            }

            int count = int.Parse(c1.ToString());

            if (count == 0)
            {
                if (TaskName.IndexOf("___") > 0)
                {
                    string eDate = System.DateTime.Now.ToString();

                    //表示分布式执行的分解任务
                    sql = "select GatherCount from SM_SplitTask where TaskID=" + TaskID;
                    object c = null;
                    if (dbType == cGlobalParas.DatabaseType.MySql)
                    {
                        c = NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(dbConn, sql);
                    }
                    else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    {
                        c = NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(dbConn, sql, null);
                    }

                    if (c != null)
                    {
                        int gCount = int.Parse(c.ToString());
                        sql = "update SM_TaskLog set Enddate='" + eDate + "', GatherCount=" + gCount + " where TaskID=" + TaskID;
                        if (dbType == cGlobalParas.DatabaseType.MySql)
                        {
                            NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                        }
                        else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                        {
                            NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                        }
                    }

                    sql = "update SM_MyTask set  State=" + (int)cGlobalParas.TaskState.Completed + ",EndDate='" + eDate + "' where ID=" + tID;
                    if (dbType == cGlobalParas.DatabaseType.MySql)
                    {
                        NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                    }
                    else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    {
                        NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                    }

                    //删除SplitTask中的运行实例
                    sql = "delete from SM_SplitTask where TID=" + tID;
                    if (dbType == cGlobalParas.DatabaseType.MySql)
                    {
                        NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                    }
                    else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    {
                        NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                    }

                }
                else
                {
                    //表示分布式执行的独立任务
                    //表示分解任务全部完成
                    sql = "select Sum(GatherCount) as c from SM_SplitTask where TID=" + tID;
                    object c = null;
                    if (dbType == cGlobalParas.DatabaseType.MySql)
                    {
                        c = NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(dbConn, sql);
                    }
                    else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    {
                        c = NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(dbConn, sql, null);
                    }
                    int gCount = 0;
                    if (c != null && c.ToString() != "")
                        gCount = int.Parse(c.ToString());

                    sql = "update SM_TaskLog set Enddate='" + System.DateTime.Now.ToString() + "', GatherCount=" + gCount + " where TaskID=" + TaskID;
                    if (dbType == cGlobalParas.DatabaseType.MySql)
                    {
                        NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                    }
                    else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    {
                        NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                    }

                    string eDate = System.DateTime.Now.ToString();
                    sql = "update SM_MyTask set  State=" + (int)cGlobalParas.TaskState.Completed + ",EndDate='" + eDate + "' where ID=" + tID;
                    if (dbType == cGlobalParas.DatabaseType.MySql)
                    {
                        NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                    }
                    else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    {
                        NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                    }

                    //删除SplitTask中的运行实例
                    sql = "delete from SM_SplitTask where TID=" + tID;
                    if (dbType == cGlobalParas.DatabaseType.MySql)
                    {
                        NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                    }
                    else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    {
                        NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                    }
                }
                //InsertLog(tID, eDate);
            }
            else
            {
                //更新日志的数据
                sql = "select GatherCount from SM_SplitTask where TaskID=" + TaskID;
                object c = null;
                if (dbType == cGlobalParas.DatabaseType.MySql)
                {
                    c = NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(dbConn, sql);
                }
                else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    c = NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(dbConn, sql, null);
                }
                if (c != null)
                {
                    int gCount = 0;
                    if (c.ToString() != "")
                        gCount = int.Parse(c.ToString());

                    string eDate = System.DateTime.Now.ToString();
                    sql = "update SM_TaskLog set Enddate='" + System.DateTime.Now.ToString() + "', GatherCount=" + gCount + " where TaskID=" + TaskID;
                    if (dbType == cGlobalParas.DatabaseType.MySql)
                    {
                        NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                    }
                    else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    {
                        NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                    }
                }

            }
        }

        public static void InsertLog(cGlobalParas.DatabaseType dbType, string dbConn, int tID, Int64 TaskID, int count, string oDate, int type)
        {
            string sql = "";
            try
            {
                sql = "select TaskName from SM_MyTask where ID='" + tID + "'";
                object c = null;
                if (dbType == cGlobalParas.DatabaseType.MySql)
                {
                    c = NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(dbConn, sql);
                }
                else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                {
                    c = NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(dbConn, sql, null);
                }
                if (c == null)
                    return;

                string TaskName = c.ToString();

                if (type == 0)
                {
                    sql = "insert into SM_TaskLog (TID,TaskID,TaskName,StartDate) values (" + tID + "," + TaskID + ",'" + TaskName + "','" + oDate + "')";
                    if (dbType == cGlobalParas.DatabaseType.MySql)
                    {
                        NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                    }
                    else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    {
                        NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                    }
                }
                else if (type == 1)
                {
                    sql = "update SM_TaskLog set EndDate='" + oDate + "',GatherCount=" + count + " where TaskID='" + TaskID + "'";
                    if (dbType == cGlobalParas.DatabaseType.MySql)
                    {
                        NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                    }
                    else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    {
                        NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                    }
                }
                else if (type == 2)
                {
                    sql = "update SM_TaskLog set EndDate='" + oDate + "',GatherCount=" + count + " where TaskID='" + TaskID + "'";
                    if (dbType == cGlobalParas.DatabaseType.MySql)
                    {
                        NetMiner.Data.Mysql.SQLHelper.ExecuteNonQuery(dbConn, sql);
                    }
                    else if (dbType == cGlobalParas.DatabaseType.MSSqlServer)
                    {
                        NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(dbConn, sql, null);
                    }


                }

            }
            catch (System.Exception ex)
            {
                //如果插入日志错误，必须马上处理
                cTool.WriteSystemLog(ex.Message, EventLogEntryType.Error, "SMGatherService");
            }
        }
    }
}

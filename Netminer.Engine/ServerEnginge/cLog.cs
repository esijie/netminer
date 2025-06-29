using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Data.Access;
using System.Data;
using System.Data.SqlClient;
using NetMiner.Resource;
using System.IO;

namespace NetMiner.Engine.ServerEngine
{
    public class cLog
    {
        //将分布式采集数据的日志写入到数据库
        public bool WriteLog(string logFile,string tName,string sqlCon)
        {
            //先将日志读取出来
            string fPath = System.IO.Path.GetDirectoryName(logFile);
            string fName = System.IO.Path.GetFileName(logFile);

            //string cnstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fPath +
            //    ";Extended Properties='text;HDR=Yes;FMT=Delimited;'";
            //string aSQL = "select * from " + fName;
            //DataTable d = SQLHelper.ExecuteDataTable(cnstring, aSQL);

            if (!File.Exists(logFile))
                return false;

            string str = string.Empty;

            FileStream myStream = null;
            StreamReader sr = null;
            try
            {
                myStream = File.Open(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(myStream, System.Text.Encoding.UTF8);

                str = sr.ReadToEnd();

              
            }
            catch (System.Exception ex)
            {
                cTool.WriteSystemLog(ex.Message, System.Diagnostics.EventLogEntryType.Error, "SMGatherService");
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }

                if (myStream != null)
                {
                    myStream.Close();
                    myStream.Dispose();
                }
            }

            string strLine = string.Empty;

            if (string.IsNullOrEmpty (str))
            {
                str = str.Trim();
                string[] Lines = str.Split('\n');

                for (int i = 0; i < Lines.Length; i++)
                {
                    strLine = Lines[i];

                    if (!string.IsNullOrEmpty(strLine))
                    {
                        strLine = strLine.Trim();
                        try
                        {
                            if (strLine.Length > 0)
                            {
                                string[] ss = strLine.Split(',');

                                string sql = "insert into SM_RunningLog(TaskID,TaskName,Type,Message,LogDate) values (@TaskID,@TaskName,@Type,@Message,@LogDate)";

                                SqlParameter[] parameters = {
                                       new SqlParameter("@TaskID" , SqlDbType.Decimal),
                                       new SqlParameter("@TaskName" , SqlDbType.VarChar,250),
                                       new SqlParameter("@Type" , SqlDbType.Int),
                                       new SqlParameter("@Message" , SqlDbType.VarChar,1000),
                                       new SqlParameter("@LogDate" , SqlDbType.DateTime)
                                        };
                                parameters[0].Value = ss[2].Trim();
                                parameters[1].Value = ss[0].Trim();
                                parameters[2].Value = (int)cGlobalParas.LogType.Error;
                                parameters[3].Value = ss[4].Trim();
                                parameters[4].Value = ss[1].Trim();

                                NetMiner.Data.SqlServer.SQLHelper.ExecuteNonQuery(sqlCon, sql, parameters);
                            }

                        }
                        catch (System.Exception ex)
                        {

                        }
                    }
                }
            }

         
            return true;

        }
    }
}

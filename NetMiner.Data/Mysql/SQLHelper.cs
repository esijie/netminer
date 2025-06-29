using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace NetMiner.Data.Mysql
{
    public class SQLHelper
    {

        #region 定义变量
        public static string MyCmsConn = "";
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
        private const int TIMEOUT = 999;
        #endregion

        #region
        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="commandText">命令语句</param>
        /// <returns>MySqlDataReader结果</returns>
        public static MySqlDataReader ExecuteReader(string cmdText)
        {
            return ExecuteReader(cmdText, null);
        }
        #endregion

        #region
        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="commandText">命令语句</param>
        /// <param name="commandParameters">命令的参数</param>
        /// <returns>MySqlDataReader结果</returns>
        public static MySqlDataReader ExecuteReader(string cmdText, params MySqlParameter[] commandParameters)
        {
            return ExecuteReader(MyCmsConn, CommandType.Text, cmdText, commandParameters);
        }
        #endregion

        #region
        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="commandType">MySql命令类型</param>
        /// <param name="commandText">命令语句</param>
        /// <param name="commandParameters">命令的参数</param>
        /// <returns>MySqlDataReader结果</returns>
        public static MySqlDataReader ExecuteReader(CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            return ExecuteReader(MyCmsConn, cmdType, cmdText, commandParameters);
        }
        #endregion

        #region
        /// <summary>
        /// 重载：ExecuteReader
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static MySqlDataReader ExecuteReader(string connString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlConnection conn = new MySqlConnection(connString);
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandTimeout = TIMEOUT;

                PrepareCommand(cmd, conn, cmdType, cmdText, commandParameters);
                MySqlDataReader rdr = null;
                try
                {
                    rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (MySqlException e)
                {
                    conn.Close();
                    conn.Dispose();

                    throw e;
                }
                cmd.Parameters.Clear();

                return rdr;
            }
        }
        #endregion



        #region

        /// <summary>
        /// ExecuteScalar
        /// </summary>
        /// <param name="commandText">命令语句</param>
        /// <returns>一个须转换成其它类型的值</returns>
        public static object ExecuteScalar(string cmdText)
        {
            return ExecuteScalar(MyCmsConn,CommandType.Text, cmdText, null);
        }

        /// <summary>
        /// ExecuteScalar
        /// </summary>
        /// <param name="commandText">命令语句</param>
        /// <param name="commandParameters">命令的参数</param>
        /// <returns>一个须转换成其它类型的值</returns>
        public static object ExecuteScalar(string cmdText, params MySqlParameter[] commandParameters)
        {
            return ExecuteScalar(MyCmsConn, CommandType.Text, cmdText, commandParameters);
        }

        public static object ExecuteScalar(string strConn, string cmdText, params MySqlParameter[] commandParameters)
        {
            return ExecuteScalar(strConn, CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// ExecuteScalar
        /// </summary>
        /// <param name="commandType">MySql命令类型</param>
        /// <param name="commandText">命令语句</param>
        /// <param name="commandParameters">命令的参数</param>
        /// <returns>一个须转换成其它类型的值</returns>
        public static object ExecuteScalar(CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            return ExecuteScalar(MyCmsConn, cmdType, cmdText, commandParameters);
        }

        /// <summary>
        /// 重载:ExecuteScalar
        /// </summary>
        /// <param name="connString">数据库连接字符串</param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string connString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            object val;
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandTimeout = TIMEOUT;

                PrepareCommand(cmd, conn, cmdType, cmdText, commandParameters);

                try
                {
                    val = cmd.ExecuteScalar();
                }
                catch (MySqlException e)
                {
                    conn.Close();
                    conn.Dispose();

                    throw e;
                }

                cmd.Parameters.Clear();
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }

            return val;


        }
        #endregion

        #region
      

        public static bool ExecuteNonQuery(string dbConn, string cmdText)
        {
            return ExecuteNonQuery(dbConn, CommandType.Text, cmdText, null);
        }

        public static bool ExecuteNonQuery(string dbConn, string cmsText, params MySqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(dbConn, CommandType.Text, cmsText, commandParameters);
        }

        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="commandText">命令语句</param>
        /// <param name="commandParameters">命令的参数</param>
        /// <returns>一个须转换成其它类型的值</returns>
        public static bool ExecuteNonQuery(string cmdText, params MySqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(MyCmsConn, CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="commandType">MySql命令类型</param>
        /// <param name="commandText">命令语句</param>
        /// <param name="commandParameters">命令的参数</param>
        /// <returns>一个须转换成其它类型的值</returns>
        public static bool ExecuteNonQuery(CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(MyCmsConn, cmdType, cmdText, commandParameters);
        }
        #endregion

        #region
        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="commandType">MySql命令类型</param>
        /// <param name="commandText">命令语句</param>
        /// <param name="commandParameters">命令的参数</param>
        /// <returns>一个须转换成其它类型的值</returns>
        public static bool ExecuteNonQuery(string connString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            int effectRows = 0;
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandTimeout = TIMEOUT ;

                PrepareCommand(cmd, conn, cmdType, cmdText, commandParameters);
                try
                {
                    effectRows = cmd.ExecuteNonQuery();
                }
                catch (MySqlException e)
                {
                    conn.Close();
                    conn.Dispose();

                    throw e;
                }
                cmd.Parameters.Clear();
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }

            return effectRows > 0;
        }
        #endregion

        #region
        /// <summary>
        /// 配置一个用来执行的Command对像
        /// </summary>
        /// <param name="cmd">Command对像,在本方法中被改变</param>
        /// <param name="conn">数据库连接对像</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令文本</param>
        /// <param name="cmdParms">命令的参数</param>
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        #endregion

        #region
        /// <summary>
        /// 为事务作准备
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        #endregion

        #region

        public static DataRow ExecuteRow(string commandText)
        {
            try
            {
                return ExecuteDataTable(commandText).Rows[0];
            }
            catch { }

            return null;
        }

        public static DataTable ExecuteDataTable(string conn,string commandText)
        {
            using (MySqlConnection con = new MySqlConnection(conn))
            {
                con.Open();

                MySqlCommand com = new MySqlCommand();
                com.Connection = con;
                com.CommandType = CommandType.Text;
                com.CommandTimeout = TIMEOUT;
                com.CommandText = commandText;

                //MySqlDataAdapter da = new MySqlDataAdapter(com);
                
                MySqlDataReader re = com.ExecuteReader();

                DataTable dt = new DataTable();

                try
                {
                    dt.Load(re);
                }
                catch (System.Data.ConstraintException)
                {
                }

                //da.Fill(dt);

                re.Close();
                com.Dispose();

                con.Close();
                con.Dispose();

                return dt;
            }
        }


        public static DataTable ExecuteDataTable(string commandText, params MySqlParameter[] commandParameters)
        {
            return ExecuteDataTable(CommandType.Text, commandText, commandParameters);
        }


        public static DataTable ExecuteDataTable(CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            return ExecuteDataTable(MyCmsConn, commandType, commandText, commandParameters);
        }

        public static DataTable ExecuteDataTable(string connString, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                MySqlCommand cmd = new MySqlCommand();

                cmd.CommandTimeout = TIMEOUT;

                bool mustCloseConnection = true;
                PrepareCommand(cmd, conn, commandType, commandText, commandParameters);

                using (MySqlDataAdapter da = new MySqlDataAdapter(commandText, conn))
                {

                    DataTable tb = new DataTable();

                    try
                    {
                        da.Fill(tb);
                    }
                    catch (MySqlException e)
                    {
                        conn.Close();
                        conn.Dispose();

                        throw e;
                    }

                    cmd.Parameters.Clear();

                    if (mustCloseConnection)
                        conn.Close();

                    return tb;
                }

            }
        }
        #endregion

        #region
        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataset(string commandText)
        {
            return ExecuteDataset(commandText, null);
        }

        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        //public static DataSet ExecuteDataset(string commandText, params MySqlParameter[] commandParameters)
        //{
        //    return ExecuteDataset(MyCmsConn, CommandType.Text, commandText, commandParameters);
        //}

        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataset(CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            return ExecuteDataset(MyCmsConn, commandType, commandText, commandParameters);
        }

        public static DataSet ExecuteDataset(string conn, string commandText)
        {
            return ExecuteDataset(conn, CommandType.Text, commandText, null);
        }
        #endregion

        #region
        /// <summary>
        /// 重载：返回DataSet
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataset(string connString, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                MySqlCommand cmd = new MySqlCommand();

                cmd.CommandTimeout = TIMEOUT;

                bool mustCloseConnection = true;
                PrepareCommand(cmd, conn, commandType, commandText, commandParameters);

                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds);
                    }
                    catch (MySqlException e)
                    {
                        conn.Close();
                        conn.Dispose();

                        throw e;
                    }

                    cmd.Parameters.Clear();

                    if (mustCloseConnection)
                        conn.Close();

                    return ds;
                }

            }
        }
        #endregion

        #region
        /// <summary>
        /// 返回读取适配器
        /// </summary>
        /// <param name="connectString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令文本</param>
        /// <param name="commandParameters">参数</param>
        /// <returns></returns>
        public static MySqlDataAdapter ExecuteAdapter(string connectString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlConnection conn = new MySqlConnection(connectString);
            MySqlCommand cmd = new MySqlCommand();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            if (commandParameters != null)
            {
                foreach (MySqlParameter parm in commandParameters)
                {
                    cmd.Parameters.Add(parm);
                }
            }



            return new MySqlDataAdapter(cmd);
        }

        /// <summary>
        /// 返回读取适配器
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令文本</param>
        /// <param name="commandParameters">参数</param>
        /// <returns></returns>
        public static MySqlDataAdapter ExecuteAdapter(CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            return ExecuteAdapter(MyCmsConn, cmdType, cmdText, commandParameters);
        }
        #endregion

        #region
        /// <summary>
        /// 执行带事务的操作
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(MySqlTransaction trans, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
        #endregion

        #region Sql分页方法
        /// <summary>
        /// 分页方法(可以选择是否返回总记录数)
        /// </summary>
        /// <param name="keyfield">主键</param>
        /// <param name="fields">要查询的字段</param>
        /// <param name="table">表名</param>
        /// <param name="orderField">排序字段(必须参数,可以多个字段  如: id,date desc  不能有order by)</param>
        /// <param name="sqlWhere">条件(不带 where)</param>
        /// <param name="pageSize">每页多少条记录</param>
        /// <param name="pageIndex">当前第几页(从1开始)</param>
        /// <param name="RowCount">要返回的总记录数</param>
        /// <param name="isRowCount">是否返回总页数</param>
        /// <param name="paramters">Sql语句参数</param>
        /// <returns></returns>
        public static DataTable SplitPage1(string keyfield, string fields, string table, string orderField, string sqlWhere, int pageSize, int pageIndex, out int RowCount, bool isRowCount, params MySqlParameter[] paramters)
        {
            if (sqlWhere.Trim().Length > 0)
            {
                sqlWhere = " AND " + sqlWhere;
            }
            int rowIndex = pageSize * (pageIndex - 1);

            string StrSql = "select " + fields + " from " + table + " Where 1=1 " + sqlWhere + " order by " + orderField;
            StrSql += " LIMIT " + rowIndex + "," + pageSize + " ; ";


            if (isRowCount)
            {
                StrSql = StrSql + " SELECT COUNT(*) FROM " + table + " Where 1=1 " + sqlWhere;
            }
            DataSet ds = ExecuteDataset(MyCmsConn, CommandType.Text, StrSql, paramters);
            DataTable dt = null;
            RowCount = 0;
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                if (isRowCount && ds.Tables.Count > 1)
                {

                    int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out RowCount);
                }
            }
            return dt;
        }

        #endregion

        //   #region 
        //   /// <summary>
        //   /// 直接返回列表
        //   /// </summary>
        //   /// <typeparam name="T"></typeparam>
        //   /// <param name="connString"></param>
        //   /// <param name="cmdType"></param>
        //   /// <param name="cmdText"></param>
        //   /// <param name="convert"></param>
        //   /// <param name="commandParameters"></param>
        //   /// <returns></returns>
        //   public static IList<T> ExecuteToList<T>(string connString, CommandType cmdType, string cmdText, Func<IDataReader, T> convert, params MySqlParameter[] commandParameters)
        //   {
        //       IList<T> list = new List<T>();

        //       using (IDataReader reader = ExecuteReader(connString, cmdType, cmdText, commandParameters))
        //       {
        //           while (reader.Read())
        //           {
        //               list.Add(convert(reader));
        //           }
        //       }

        //       return list;
        //   }
        //   #endregion

        //   #region 
        //   /// <summary>
        //   /// 直接返回列表
        //   /// </summary>
        //   /// <typeparam name="T"></typeparam>
        //   /// <param name="connString"></param>
        //   /// <param name="cmdType"></param>
        //   /// <param name="cmdText"></param>
        //   /// <param name="convert"></param>
        //   /// <param name="commandParameters"></param>
        //   /// <returns></returns>
        //   public static IList<T> ExecuteToList<T>(CommandType cmdType, string cmdText, Func<IDataReader, T> convert, params MySqlParameter[] commandParameters)
        //   {
        //       return ExecuteToList<T>(MyCmsConn, cmdType, cmdText, convert, commandParameters);
        //   }
        //#endregion

        //   #region 
        //   /// <summary>
        //   /// 直接返回实体
        //   /// </summary>
        //   /// <typeparam name="T"></typeparam>
        //   /// <param name="connString"></param>
        //   /// <param name="cmdType"></param>
        //   /// <param name="cmdText"></param>
        //   /// <param name="convert"></param>
        //   /// <param name="commandParameters"></param>
        //   /// <returns></returns>
        //   public static T ExecuteToEntity<T>(string connString, CommandType cmdType, string cmdText, Func<IDataReader, T> convert, params MySqlParameter[] commandParameters)
        //   {
        //       T t = default(T);

        //       using (IDataReader reader = ExecuteReader(connString, cmdType, cmdText, commandParameters))
        //       {
        //           if (reader.Read())
        //           {
        //               t = convert(reader);
        //           }
        //       }

        //       return t;
        //   }
        //   #endregion

        //   #region  
        //   /// <summary>
        //   /// 直接返回实体
        //   /// </summary>
        //   /// <typeparam name="T"></typeparam>
        //   /// <param name="cmdType"></param>
        //   /// <param name="cmdText"></param>
        //   /// <param name="convert"></param>
        //   /// <param name="commandParameters"></param>
        //   /// <returns></returns>
        //   public static T ExecuteToEntity<T>(CommandType cmdType, string cmdText, Func<IDataReader, T> convert, params MySqlParameter[] commandParameters)
        //   {
        //       T t = default(T);

        //       using (IDataReader reader = ExecuteReader(MyCmsConn, cmdType, cmdText, commandParameters))
        //       {
        //           if (reader.Read())
        //           {
        //               t = convert(reader);
        //           }
        //       }

        //       return t;
        //   }

        //   #endregion
    }
}

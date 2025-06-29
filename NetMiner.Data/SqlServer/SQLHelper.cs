using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Data.SqlServer
{
    public class SQLHelper
    {
        #region 定义变量
        public static readonly string MyCmsConn ="";
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
        #endregion

        #region 对连接执行查询并返回受影响的行数
       

        /// <summary>
        /// 对连接执行查询并返回受影响的行数
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="cmdType">类型</param>
        /// <param name="cmdText">存储过程名或sql语句</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }
        
        /// <summary>
        /// 对连接执行查询并返回受影响的行数(执行Sql语句)
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="cmdText">sql语句</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }


        /// <summary>
        ///  对连接执行查询并返回储存过程返回值
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="cmdType">类型</param>
        /// <param name="cmdText">存储过程名</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>返回值</returns>
        public static int RunProcedure(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                if (commandParameters != null)
                {
                    cmd.Parameters.Add(new SqlParameter("ReturnValue",
                SqlDbType.Int, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
                }

                cmd.ExecuteNonQuery();
                return (int)cmd.Parameters["ReturnValue"].Value;
            }
        }


        /// <summary>
        ///  对连接执行查询并返回储存过程返回值(执行Sql语句)
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="cmdText">Sql语句</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>返回值</returns>
        public static int RunProcedure(string connectionString, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, commandParameters);
                if (commandParameters != null)
                {
                    cmd.Parameters.Add(new SqlParameter("ReturnValue",
                SqlDbType.Int, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
                }

                cmd.ExecuteNonQuery();
                return (int)cmd.Parameters["ReturnValue"].Value;
            }
        }

        #endregion

        #region 对连接执行事物查询并返回受影响的行数
        /// <summary>
        /// 用默认的连接执行Sql事务
        /// </summary>
        /// <param name="cmdText">Sql语句</param>
        /// <param name="commandParameters">参数</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQueryTrans(string cmdText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection conn = new SqlConnection(MyCmsConn))
            {
                SqlCommand cmd = new SqlCommand();
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                int val = 0;
                PrepareCommand(cmd, trans.Connection, trans, CommandType.Text, cmdText, commandParameters);
                try
                {
                    val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    trans.Commit();

                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
                finally
                {
                    trans.Dispose();
                    trans = null;
                }
                return val;

            }
        }
        /// <summary>
        /// 用默认的连接执行事务
        /// </summary>
        /// <param name="cmdType">类型</param>
        /// <param name="cmdText">存储过程名或sql语句</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQueryTrans(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            using (SqlConnection conn = new SqlConnection(MyCmsConn))
            {
                SqlCommand cmd = new SqlCommand();
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                int val = 0;
                PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
                try
                {
                    val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    trans.Commit();

                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
                finally
                {
                    trans.Dispose();
                    trans = null;
                }
                return val;

            }
        }
        /// <summary>
        /// 对连接执行事物查询并返回受影响的行数
        /// </summary>
        /// <param name="trans">事物对象</param>
        /// <param name="cmdType">类型</param>
        /// <param name="cmdText">存储过程名或sql语句</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 对连接执行事物查询并返回受影响的行数(执行Sql语句)
        /// </summary>
        /// <param name="trans">事物对象</param>
        /// <param name="cmdText">sql语句</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, CommandType.Text, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
        #endregion

        #region 返回SqlDataReader对象
        /// <summary>
        /// 返回SqlDataReader对象
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="cmdType">类型</param>
        /// <param name="cmdText">存储过程名或sql语句</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);//这保证关闭连接
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        /// <summary>
        /// 返回SqlDataReader对象(执行Sql语句)
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="cmdText">sql语句</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(string connectionString, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);//这保证关闭连接
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        #endregion

        #region 返回DataSet对象
        /// <summary>
        /// 返回DataSet对象
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="cmdType">执行类型</param>
        /// <param name="cmdText">sql语句或存储过程名</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>DataSet对象</returns>
        public static DataSet ExecuteDataSet(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        da.Dispose();
                        cmd.Dispose();
                        connection.Close();
                    }
                    return ds;
                }
            }
        }


        /// <summary>
        /// 返回DataSet对象(执行Sql语句)
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="cmdText">sql语句</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>DataSet对象</returns>
        public static DataSet ExecuteDataSet(string connectionString, string cmdText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, CommandType.Text, cmdText, commandParameters);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        da.Dispose();
                        cmd.Dispose();
                        connection.Close();
                    }
                    return ds;
                }
            }
        }

        /// <summary>
        /// 返回DataSet对象
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="cmdText">sql语句</param>
        /// <param name="PageNow">当前页码</param>
        /// <param name="Rows">一页的行数</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>DataSet对象</returns>
        public static DataSet ExecuteDataSet(string connectionString, string cmdText, int PageNow, int Rows, params SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, CommandType.Text, cmdText, commandParameters);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    int StartRecod = (PageNow - 1) * Rows;
                    try
                    {
                        da.Fill(ds, StartRecod, Rows, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        da.Dispose();
                        cmd.Dispose();
                        connection.Close();
                    }
                    return ds;
                }
            }
        }
        #endregion

        #region 返回DataTable对象
        /// <summary>
        /// 返回DataTable对象
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="cmdType">执行类型</param>
        /// <param name="cmdText">sql语句或存储过程名</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>DataTable对象</returns>
        public static DataTable ExecuteDataTable(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        da.Fill(dt);
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        da.Dispose();
                        cmd.Dispose();
                        connection.Close();
                    }
                    return dt;
                }
            }
        }

        /// <summary>
        /// 返回DataTable对象(执行Sql语句)
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="cmdText">sql语句</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>DataTable对象</returns>
        public static DataTable ExecuteDataTable(string connectionString, string cmdText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, CommandType.Text, cmdText, commandParameters);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        da.Fill(dt);
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        da.Dispose();
                        cmd.Dispose();
                        connection.Close();
                    }
                    return dt;
                }
            }
        }
        #endregion

        #region 执行查询，并返回查询所返回的结果集中第一行的第一列
        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="cmdType">类型</param>
        /// <param name="cmdText">存储过程名或sql语句</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>结果集中第一行的第一列或空引用（如果结果集为空）。</returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。(执行Sql语句)
        /// </summary>
        /// <param name="connectionString">连接串</param>
        /// <param name="cmdText">sql语句</param>
        /// <param name="commandParameters">参数数组</param>
        /// <returns>结果集中第一行的第一列或空引用（如果结果集为空）。</returns>
        public static object ExecuteScalar(string connectionString, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, CommandType.Text, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }
        #endregion

        #region Sql分页方法


        /// <summary>
        /// 分页方法(可以选择是否返回总记录数)
        /// </summary>
        /// <param name="connectString">数据库连接字符串</param>
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
        public static DataTable Pagination(string connectString, string keyfield, string fields, string table, string orderField, string sqlWhere, int pageSize, int pageIndex, out int RowCount, bool isRowCount, params SqlParameter[] paramters)
        {
            if (sqlWhere.Trim().Length > 0)
            {
                sqlWhere = " AND " + sqlWhere;
            }
            int intTop = pageSize * (pageIndex - 1);
            string StrSql = "select top " + pageSize + " " + fields + " from " + table + " Where " + keyfield +
                " not in (select top " + intTop + " " + keyfield + " from " + table + " where 1=1 " + sqlWhere + " order by " + orderField + " ) " + sqlWhere +
                " order by " + orderField;
            if (isRowCount)
            {
                StrSql = StrSql + " SELECT COUNT (*) FROM " + table + " Where 1=1 " + sqlWhere;
            }
            DataSet ds = ExecuteDataSet(connectString, CommandType.Text, StrSql, paramters);
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
        /// <summary>
        /// 分页方法
        /// </summary>
        /// <param name="connectString">数据库连接字符串</param>
        /// <param name="keyfield">主键</param>
        /// <param name="fields">要查询的字段</param>
        /// <param name="table">表名</param>
        /// <param name="orderField">排序字段(必须参数,可以多个字段  如: id,date desc  不能有order by)</param>
        /// <param name="sqlWhere">条件(不带 where)</param>
        /// <param name="pageSize">每页多少条记录</param>
        /// <param name="pageIndex">当前第几页(从1开始)</param>
        /// <param name="paramters">Sql语句参数</param>
        /// <returns></returns>
        public static DataTable Pagination(string connectString, string keyfield, string fields, string table, string orderField, string sqlWhere, int pageSize, int pageIndex, params SqlParameter[] paramters)
        {
            if (sqlWhere.Trim().Length > 0)
            {
                sqlWhere = " AND " + sqlWhere;
            }
            int intTop = pageSize * (pageIndex - 1);
            string StrSql = "select top " + pageSize + " " + fields + " from " + table + " Where " + keyfield +
                " not in (select top " + intTop + " " + keyfield + " from " + table + " where 1=1 " + sqlWhere + " order by " + orderField + " ) " + sqlWhere +
                " order by " + orderField;
            DataSet ds = ExecuteDataSet(connectString, CommandType.Text, StrSql, paramters);
            DataTable dt = null;
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;

        }

        #region SqlServer2005分页
        /// <summary>
        /// 分页方法(可以选择是否返回总记录数)
        /// </summary>
        /// <param name="connectString">数据库连接字符串</param>
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
        public static DataTable Pagination2005(string connectString, string keyfield, string fields, string table, string orderField, string sqlWhere, int pageSize, int pageIndex, out int RowCount, bool isRowCount, params SqlParameter[] paramters)
        {
            string StrSql = "SELECT * ";
            int StartRecord = 0, EndRecord = 0;
            if (sqlWhere.Trim().Length > 0)
            {
                sqlWhere = " WHERE " + sqlWhere;
            }
            if (pageIndex > 0)
            {
                pageIndex--;
            }
            else
            {
                pageIndex = 0;
            }
            StartRecord = pageIndex * pageSize + 1;
            EndRecord = StartRecord + pageSize - 1;
            StrSql = StrSql + " FROM ( SELECT ROW_NUMBER() Over(order by " + orderField + " ) as rowId," + fields + " FROM " + table + sqlWhere + " ) AS a WHERE rowId BETWEEN  " + StartRecord.ToString() + " and " + EndRecord.ToString();
            if (isRowCount)
            {
                StrSql = StrSql + " SELECT COUNT (*) FROM " + table + sqlWhere;
            }
            DataSet ds = ExecuteDataSet(connectString, CommandType.Text, StrSql, paramters);
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
        /// <summary>
        /// 分页方法
        /// </summary>
        /// <param name="connectString">数据库连接字符串</param>
        /// <param name="fields">要查询的字段</param>
        /// <param name="table">表名</param>
        /// <param name="orderField">排序字段(必须参数,可以多个字段  如: id,date desc  不能有order by)</param>
        /// <param name="sqlWhere">条件(不带 where)</param>
        /// <param name="pageSize">每页多少条记录</param>
        /// <param name="pageIndex">当前第几页(从1开始)</param>
        /// <param name="paramters">Sql语句参数</param>
        /// <returns></returns>
        public static DataTable Pagination2005(string connectString, string fields, string table, string orderField, string sqlWhere, int pageSize, int pageIndex, params SqlParameter[] paramters)
        {
            string StrSql = "SELECT * ";
            int StartRecord = 0, EndRecord = 0;
            if (sqlWhere.Trim().Length > 0)
            {
                sqlWhere = " WHERE " + sqlWhere;
            }
            if (pageIndex > 0)
            {
                pageIndex--;
            }
            else
            {
                pageIndex = 0;
            }
            StartRecord = pageIndex * pageSize + 1;
            EndRecord = StartRecord + pageSize - 1;
            StrSql = StrSql + " FROM ( SELECT ROW_NUMBER() Over(order by " + orderField + " ) as rowId," + fields + " FROM " + table + sqlWhere + " ) AS a WHERE rowId BETWEEN  " + StartRecord.ToString() + " and " + EndRecord.ToString();

            DataSet ds = ExecuteDataSet(connectString, CommandType.Text, StrSql, paramters);
            DataTable dt = null;
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;

        }

        #endregion



        ///<summary>
        /// 分页方法(可以选择是否返回总记录数)
        /// </summary>
        /// <param name="connectString">数据库连接字符串</param>
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
        public static DataTable Pagination(string connectString, string keyfield, string fields, string table, string orderField, string sqlWhere, int pageSize, int pageIndex, out int RowCount, bool isRowCount, List<SqlParameter> paramters)
        {
            if (paramters != null && paramters.Count > 0)
                return Pagination(connectString, keyfield, fields, table, orderField, sqlWhere, pageSize, pageIndex, out RowCount, isRowCount, paramters.ToArray());
            return Pagination(connectString, keyfield, fields, table, orderField, sqlWhere, pageSize, pageIndex, out RowCount, isRowCount);
        }



        /// <summary>
        /// 分页方法
        /// </summary>
        /// <param name="connectString">数据库连接字符串</param>
        /// <param name="keyfield">主键</param>
        /// <param name="fields">要查询的字段</param>
        /// <param name="table">表名</param>
        /// <param name="orderField">排序字段(必须参数,可以多个字段  如: id,date desc  不能有order by)</param>
        /// <param name="sqlWhere">条件(不带 where)</param>
        /// <param name="pageSize">每页多少条记录</param>
        /// <param name="pageIndex">当前第几页(从1开始)</param>
        /// <param name="paramters">Sql语句参数</param>
        /// <returns></returns>
        public static DataTable Pagination(string connectString, string keyfield, string fields, string table, string orderField, string sqlWhere, int pageSize, int pageIndex, List<SqlParameter> paramters)
        {
            if (paramters != null && paramters.Count > 0)
                return Pagination(connectString, keyfield, fields, table, orderField, sqlWhere, pageSize, pageIndex, paramters.ToArray());
            return Pagination(connectString, keyfield, fields, table, orderField, sqlWhere, pageSize, pageIndex);
        }


        #endregion

        /// <summary>
        /// 将一行DataRow数据填充到键值对中
        /// </summary>
        /// <param name="TempRow">一行数据</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDataHash(DataRow TempRow)
        {
            Dictionary<string, string> tempD = null;
            if (TempRow != null && TempRow.Table.Columns.Count > 0)
            {
                tempD = new Dictionary<string, string>();
                foreach (DataColumn NewClumn in TempRow.Table.Columns)
                {

                    string ClName = NewClumn.ColumnName;
                    tempD.Add(ClName, TempRow[ClName].ToString());
                }
            }
            return tempD;
        }

        /// <summary>
        /// 将一行DataReader数据填充到键值对中
        /// </summary>
        /// <param name="TempRead">一行数据</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDataHash(SqlDataReader TempRead)
        {
            Dictionary<string, string> tempD = null;
            if (TempRead != null && TempRead.FieldCount > 0)
            {
                tempD = new Dictionary<string, string>();
                for (int i = 0; i < TempRead.FieldCount; i++)
                {
                    string ClName = TempRead.GetName(i);
                    tempD.Add(ClName, TempRead[ClName].ToString());
                }

            }
            return tempD;
        }


        #region 保留参数数组进入缓存的函数
        /// <summary>
        /// 保留参数数组进入缓存的函数
        /// </summary>
        /// <param name="cacheKey">缓存中参数数组的key</param>
        /// <param name="commandParameters">参数数租</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }
        #endregion

        #region 获取缓存中的参数数租的函数
        /// <summary>
        /// 获取缓存中的参数数租的函数
        /// </summary>
        /// <param name="cacheKey">缓存中参数数组的key</param>
        /// <returns>参数数组</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }
        #endregion

        #region 加参数函数
        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">SqlCommand object</param>
        /// <param name="conn">SqlConnection object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
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
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        #endregion
        /// <summary>
        /// 获取Sql缓存的key
        /// </summary>
        /// <param name="strSql">Sql语句或者存储过程名</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        private static string GetCacheKey(string strSql, SqlParameter[] parameters)
        {
            string strKey = strSql;
            if (parameters != null && parameters.Length > 0)
            {
                foreach (SqlParameter tempPara in parameters)
                {
                    strKey = strKey + "$$" + tempPara.ParameterName + "--" + tempPara.Value.ToString();
                }
            }
            return strKey;
        }
    
    }
}

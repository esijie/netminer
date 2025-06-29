using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using NetMiner.Data;
using NetMiner.Data.Access;
using NetMiner.Data.Sqlite;
using NetMiner.Data.Mysql;
using NetMiner.Data.SqlServer;
using System.Text;
using NetMiner.Resource;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data.OracleClient;
using System.Text.RegularExpressions;

namespace NetMiner.Core.DB
{
    public class oDBDeal
    {
        private string m_workPath = string.Empty;
        private DataTable m_accessDbType;
        private DataTable m_mssqlDbType;
        private DataTable m_mysqlDbType;

        public oDBDeal(string workPath)
        {
            this.m_workPath = workPath;
        }

        ~oDBDeal()
        {

        }


        #region 数据库建立新表处理
        public bool NewTable(DataColumnCollection dColumns, cGlobalParas.DatabaseType dType, string dSource, string TableName)
        {
            //首先判断表是否存在，如果不存在则进行建立

            bool isSucceed = false;

            switch (dType)
            {
                case cGlobalParas.DatabaseType.Access:
                    //isSucceed = NewAccessTable(dColumns, dSource, TableName);
                    break;
                case cGlobalParas.DatabaseType.MSSqlServer:
                    isSucceed = NewMSSqlServerTable(dColumns, dSource, TableName);
                    break;
                case cGlobalParas.DatabaseType.MySql:
                    isSucceed = NewMySqlTable(dColumns, dSource, TableName);
                    break;
                case cGlobalParas.DatabaseType.Oracle:
                    isSucceed = NewOracleTable(dColumns, dSource, TableName);
                    break;
                case cGlobalParas.DatabaseType.SqLite:
                    isSucceed = NewSqliteTable(dColumns, dSource, TableName);
                    break;
            }

            return isSucceed;

        }

        /// <summary>
        /// 传入的数据库连接串需要密码需明文
        /// </summary>
        /// <param name="dColumns"></param>
        /// <param name="dSource"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        private bool NewSqliteTable(DataColumnCollection dColumns, string dSource, string TableName)
        {
            bool IsTable = false;

            SQLiteConnection conn = new SQLiteConnection();
            string connectionstring = dSource;

            try
            {
                conn.ConnectionString = connectionstring;
                conn.Open();
            }
            catch (System.Exception)
            {
                throw new NetMinerException("数据库连接失败，请检查数据库连接信息");
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[3].ToString().ToUpper() == "TABLE")
                {
                    if (r[2].ToString() == TableName)
                    {
                        IsTable = true;
                        break;
                    }
                }

            }

            if (IsTable == false)
            {
                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.Access, "", dColumns, TableName);

                SQLiteCommand com = new SQLiteCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (System.Data.OleDb.OleDbException ex)
                {
                    if (ex.ErrorCode != -2147217900)
                    {

                        conn.Close();
                        return false;
                    }
                    else
                    {
                        throw ex;
                    }
                }

                IsTable = true;

            }

            conn.Close();

            return IsTable;
        }

        /// <summary>
        /// 传入的数据库连接串需要密码需明文
        /// </summary>
        /// <param name="dColumns"></param>
        /// <param name="dSource"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool NewMSSqlServerTable(DataColumnCollection dColumns, string dSource, string TableName)
        {
            bool IsTable = false;

            SqlConnection conn = new SqlConnection();
            string connectionstring = dSource;

            try
            {
                conn.ConnectionString = connectionstring;
                conn.Open();
            }
            catch (System.Exception)
            {
                throw new NetMinerException("数据库连接失败，请检查数据库连接信息");
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[2].ToString() == TableName)
                {
                    IsTable = true;
                    break;
                }
            }

            if (IsTable == false)
            {
                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.MSSqlServer, "", dColumns, TableName);

                SqlCommand com = new SqlCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    if (ex.ErrorCode != -2147217900)
                    {
                        conn.Close();
                        return false;
                    }
                    else
                        throw ex;
                }
                IsTable = true;
            }

            conn.Close();

            return IsTable;
        }

        /// <summary>
        /// 传入的数据库连接串需要密码需明文
        /// </summary>
        /// <param name="dColumns"></param>
        /// <param name="dSource"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool NewMySqlTable(DataColumnCollection dColumns, string dSource, string TableName)
        {
            bool IsTable = false;

            MySqlConnection conn = new MySqlConnection();
            string connectionstring = dSource;

            try
            {
                conn.ConnectionString = connectionstring;
                conn.Open();
            }
            catch (System.Exception)
            {
                throw new NetMinerException("数据库连接失败，请检查数据库连接信息");
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (string.Compare(r[2].ToString(), TableName, true) == 0)
                {
                    IsTable = true;
                    break;
                }
            }

            if (IsTable == false)
            {
                //通过连接字符串把编码获取出来，根据编码进行数据表的建立
                string strMatch = "(?<=character set=)[^\\s]*(?=[\\s;])";
                Match s = Regex.Match(connectionstring, strMatch, RegexOptions.IgnoreCase);
                string Encoding = s.Groups[0].Value;

                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.MySql, Encoding, dColumns, TableName);

                MySqlCommand com = new MySqlCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    if (ex.ErrorCode != -2147217900)
                    {
                        return false;
                    }
                    else
                        throw ex;
                }

                IsTable = true;
            }

            return IsTable;
        }

        /// <summary>
        /// 传入的数据库连接串需要密码需明文
        /// </summary>
        /// <param name="dColumns"></param>
        /// <param name="dSource"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool NewOracleTable(DataColumnCollection dColumns, string dSource, string TableName)
        {
            bool IsTable = false;

            OracleConnection conn = new OracleConnection();
            string connectionstring = dSource;

            try
            {
                conn.ConnectionString = connectionstring;
                conn.Open();
            }
            catch (System.Exception)
            {
                throw new NetMinerException("数据库连接失败，请检查数据库连接信息");
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (string.Compare(r[1].ToString(), TableName, true) == 0)
                {
                    IsTable = true;
                    break;
                }
            }

            if (IsTable == false)
            {

                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.Oracle, "", dColumns, TableName);

                //将表名称进行替换，加入数据库名
                //通过链接串讲数据库提取出来
                Match charSetMatch = Regex.Match(connectionstring, "(?<=SERVICE_NAME=).*?(?=\\))", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string sName = charSetMatch.Groups[0].ToString();
                CreateTablesql = CreateTablesql.Replace(TableName, "\"" + sName + "\"" + "." + "\"" + TableName + "\"");


                OracleCommand com = new OracleCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (OracleException ex)
                {
                    if (ex.ErrorCode != -2147217900)
                    {
                        return false;
                    }
                    else
                        throw ex;
                }

                IsTable = true;
            }

            return IsTable;
        }
        #endregion

        #region 发布数据到数据库处理

        /// <summary>
        /// 传入的数据库连接串需要密码需明文
        /// </summary>
        /// <param name="dCols"></param>
        /// <param name="dRow"></param>
        /// <param name="dSource"></param>
        /// <param name="sInsertSql"></param>
        /// <param name="tableName"></param>
        /// <param name="isSqlTrue"></param>
        public void ExportMSSql(DataColumnCollection dCols, object dRow, string dSource, string sInsertSql, string tableName, bool isSqlTrue)
        {
            if (isSqlTrue == true)
            {
                ExportMSSql(dCols, dRow, dSource, sInsertSql);
                return;
            }
            else
            {

                SqlConnection conn = new SqlConnection();

                string connectionstring = dSource;

                conn.ConnectionString = connectionstring;

                object[] Row = ((object[])dRow);

                try
                {
                    conn.Open();
                }
                catch (System.Exception ex)
                {

                    throw ex;
                }

                //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
                SqlCommand cm = new SqlCommand();
                cm.Connection = conn;
                cm.CommandType = CommandType.Text;



                //分析多sql语句
                string[] sqls = sInsertSql.Split(';');

                for (int i = 0; i < sqls.Length; i++)
                {
                    string sql = sqls[i];
                    if (!string.IsNullOrEmpty(sql))
                    {

                        //先对sql语句进行参数整合
                        int len = sql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase) + 7;

                        string sqlpre = sql.Substring(0, sql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase));
                        string sqlValue = sql.Substring(len, sql.Length - len);

                        string sqlpara = sqlpre.Substring(sqlpre.IndexOf("(") + 1, sqlpre.IndexOf(")") - sqlpre.IndexOf("(") - 1);

                        //开始拼接一个带有参数的sql语句
                        string[] sqlparas = sqlpara.Split(',');
                        for (int index = 0; index < sqlparas.Length; index++)
                        {
                            sqlparas[index] = sqlparas[index].Trim();
                        }

                        sqlpara = "";
                        for (int j = 0; j < sqlparas.Length; j++)
                        {
                            sqlpara += "@" + sqlparas[j] + ",";
                        }

                        //去掉最后一个,
                        sqlpara = sqlpara.Substring(0, sqlpara.Length - 1);

                        sql = sqlpre + " values (" + sqlpara + ")";

                        //给参数赋值
                        SqlParameter[] parameters = new SqlParameter[sqlparas.Length];
                        for (int j = 0; j < sqlparas.Length; j++)
                        {
                            int cLen = 0;
                            SqlDbType dType = GetMSSqlTableColumns(tableName, sqlparas[j], connectionstring, out cLen);
                            if (cLen == -1)
                                parameters[j] = new SqlParameter("@" + sqlparas[j], dType);
                            else
                                parameters[j] = new SqlParameter("@" + sqlparas[j], dType, cLen);
                        }

                        sqlValue = sqlValue.Substring(sqlValue.IndexOf("(") + 1, sqlValue.IndexOf(")") - sqlValue.IndexOf("(") - 1);
                        string[] sqlvalues = sqlValue.Split(',');
                        for (int j = 0; j < sqlvalues.Length; j++)
                        {
                            string sValue = sqlvalues[j];
                            sValue = Regex.Replace(sValue, "['|{|}]", "", RegexOptions.IgnoreCase).Trim();
                            for (int m = 0; m < dCols.Count; m++)
                            {
                                if (sValue == dCols[m].ColumnName)
                                {
                                    sValue = Row[m].ToString();
                                    break;
                                }
                            }
                            parameters[j].Value = sValue;
                        }



                        try
                        {
                            cm.CommandText = sql;
                            foreach (SqlParameter parm in parameters)
                                cm.Parameters.Add(parm);
                            cm.ExecuteNonQuery();
                        }
                        catch (System.Exception ex)
                        {
                            conn.Close();
                            throw ex;
                        }

                    }

                }

                conn.Close();
            }
        }

        /// <summary>
        /// 传入的数据库连接串需要密码需明文
        /// </summary>
        /// <param name="dCols"></param>
        /// <param name="dRow"></param>
        /// <param name="dSource"></param>
        /// <param name="sInsertSql"></param>
        /// <param name="tableName"></param>
        /// <param name="isSqlTrue"></param>
        public void ExportMySql(DataColumnCollection dCols, object dRow, string dSource, string sInsertSql, string tableName, bool isSqlTrue)
        {
            if (isSqlTrue == true)
            {
                ExportMySql(dCols, dRow, dSource, sInsertSql);
                return;
            }
            else
            {

                MySqlConnection conn = new MySqlConnection();

                string connectionstring = dSource;

                conn.ConnectionString = connectionstring;

                object[] Row = ((object[])dRow);

                try
                {
                    conn.Open();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

                //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
                MySqlCommand cm = new MySqlCommand();
                cm.Connection = conn;
                cm.CommandType = CommandType.Text;

                //开始拼sql语句

                //分析多sql语句
                string[] sqls = sInsertSql.Split(';');

                for (int i = 0; i < sqls.Length; i++)
                {
                    string sql = sqls[i];

                    if (!string.IsNullOrEmpty(sql))
                    {
                        //先对sql语句进行参数整合
                        int len = sql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase) + 7;

                        string sqlpre = sql.Substring(0, sql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase));
                        string sqlValue = sql.Substring(len, sql.Length - len);

                        string sqlpara = sqlpre.Substring(sqlpre.IndexOf("(") + 1, sqlpre.IndexOf(")") - sqlpre.IndexOf("(") - 1);

                        //开始拼接一个带有参数的sql语句
                        string[] sqlparas = sqlpara.Split(',');
                        for (int index = 0; index < sqlparas.Length; index++)
                        {
                            sqlparas[index] = sqlparas[index].Trim();
                        }
                        sqlpara = "";
                        for (int j = 0; j < sqlparas.Length; j++)
                        {
                            sqlpara += "?" + sqlparas[j] + ",";
                        }

                        //去掉最后一个,
                        sqlpara = sqlpara.Substring(0, sqlpara.Length - 1);

                        sql = sqlpre + " values (" + sqlpara + ")";

                        //给参数赋值
                        MySqlParameter[] parameters = new MySqlParameter[sqlparas.Length];
                        for (int j = 0; j < sqlparas.Length; j++)
                        {
                            int cLen = 0;
                            MySqlDbType dType = GetMysqlTableColumns(tableName, sqlparas[j], connectionstring, out cLen);
                            if (cLen == -1)
                                parameters[j] = new MySqlParameter("?" + sqlparas[j], dType);
                            else
                                parameters[j] = new MySqlParameter("?" + sqlparas[j], dType, cLen);
                        }

                        sqlValue = sqlValue.Substring(sqlValue.IndexOf("(") + 1, sqlValue.IndexOf(")") - sqlValue.IndexOf("(") - 1);
                        string[] sqlvalues = sqlValue.Split(',');
                        for (int j = 0; j < sqlvalues.Length; j++)
                        {
                            string sValue = sqlvalues[j];
                            sValue = Regex.Replace(sValue, "['|{|}]", "", RegexOptions.IgnoreCase).Trim();
                            for (int m = 0; m < dCols.Count; m++)
                            {
                                if (sValue == dCols[m].ColumnName)
                                {
                                    sValue = Row[m].ToString();
                                    break;
                                }
                            }
                            parameters[j].Value = sValue;
                        }

                        try
                        {
                            cm.CommandText = sql;
                            foreach (MySqlParameter parm in parameters)
                                cm.Parameters.Add(parm);
                            cm.ExecuteNonQuery();
                        }
                        catch (System.Exception ex)
                        {
                            conn.Close();
                            throw ex;
                        }
                    }

                }

                conn.Close();
            }
        }

        /// <summary>
        /// 传入的数据库连接串需要密码需明文
        /// </summary>
        /// <param name="dCols"></param>
        /// <param name="dRow"></param>
        /// <param name="dSource"></param>
        /// <param name="sInsertSql"></param>
        /// <param name="tableName"></param>
        /// <param name="isSqlTrue"></param>
        public void ExportOracle(DataColumnCollection dCols, object dRow, string dSource, string sInsertSql, string tableName, bool isSqlTrue)
        {
            if (isSqlTrue == true)
            {
                ExportOracle(dCols, dRow, dSource, sInsertSql);
                return;
            }
            else
            {

                OracleConnection conn = new OracleConnection();

                string connectionstring = dSource;

                conn.ConnectionString = connectionstring;

                object[] Row = ((object[])dRow);

                try
                {
                    conn.Open();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

                //获取数据库名
                Match charSetMatch = Regex.Match(connectionstring, "(?<=User Id=).*?(?=;)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string dName = charSetMatch.Groups[0].ToString();

                //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
                OracleCommand cm = new OracleCommand();
                cm.Connection = conn;
                cm.CommandType = CommandType.Text;

                //开始拼sql语句

                //分析多sql语句
                string[] sqls = sInsertSql.Split(';');

                for (int i = 0; i < sqls.Length; i++)
                {
                    string sql = sqls[i];

                    if (!string.IsNullOrEmpty(sql))
                    {
                        //先对sql语句进行参数整合
                        int len = sql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase) + 7;

                        string sqlpre = sql.Substring(0, sql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase));
                        string sqlpre1 = sqlpre.Substring(0, sqlpre.IndexOf("(", StringComparison.CurrentCultureIgnoreCase));

                        if (!string.IsNullOrEmpty(dName))
                            sqlpre1 = sqlpre1.Replace(tableName, "\"" + dName + "\"" + "." + "\"" + tableName + "\"");
                        sqlpre = sqlpre1 + sqlpre.Substring(sqlpre.IndexOf("(", StringComparison.CurrentCultureIgnoreCase), sqlpre.Length - sqlpre.IndexOf("(", StringComparison.CurrentCultureIgnoreCase));

                        string sqlValue = sql.Substring(len, sql.Length - len);

                        string sqlpara = sqlpre.Substring(sqlpre.IndexOf("(") + 1, sqlpre.IndexOf(")") - sqlpre.IndexOf("(") - 1);

                        //开始拼接一个带有参数的sql语句
                        string[] sqlparas = sqlpara.Split(',');
                        for (int index = 0; index < sqlparas.Length; index++)
                        {
                            sqlparas[index] = sqlparas[index].Trim();
                        }
                        sqlpara = "";
                        for (int j = 0; j < sqlparas.Length; j++)
                        {
                            sqlpara += ":" + sqlparas[j] + ",";
                        }

                        //去掉最后一个,
                        sqlpara = sqlpara.Substring(0, sqlpara.Length - 1);

                        sql = sqlpre + " values (" + sqlpara + ")";

                        //给参数赋值
                        OracleParameter[] parameters = new OracleParameter[sqlparas.Length];
                        for (int j = 0; j < sqlparas.Length; j++)
                        {
                            int cLen = 0;
                            OracleType dType = GetOracleTableColumns(tableName, sqlparas[j], connectionstring, out cLen);
                            if (cLen == -1)
                                parameters[j] = new OracleParameter(":" + sqlparas[j], dType);
                            else
                                parameters[j] = new OracleParameter(":" + sqlparas[j], dType, cLen);
                        }

                        sqlValue = sqlValue.Substring(sqlValue.IndexOf("(") + 1, sqlValue.IndexOf(")") - sqlValue.IndexOf("(") - 1);
                        string[] sqlvalues = sqlValue.Split(',');
                        for (int j = 0; j < sqlvalues.Length; j++)
                        {
                            string sValue = sqlvalues[j];
                            sValue = Regex.Replace(sValue, "['|{|}]", "", RegexOptions.IgnoreCase).Trim();
                            for (int m = 0; m < dCols.Count; m++)
                            {
                                if (sValue.ToLower().Trim() == dCols[m].ColumnName.ToLower().Trim())
                                {
                                    sValue = Row[m].ToString();
                                    break;
                                }
                            }
                            parameters[j].Value = sValue;
                        }



                        try
                        {
                            cm.CommandText = sql;
                            foreach (OracleParameter parm in parameters)
                                cm.Parameters.Add(parm);
                            cm.ExecuteNonQuery();
                        }
                        catch (System.Exception ex)
                        {
                            conn.Close();
                            throw ex;
                        }
                    }
                }

                conn.Close();
            }
        }

        /// <summary>
        /// 传入的数据库连接串需要密码需明文
        /// </summary>
        /// <param name="dCols"></param>
        /// <param name="dRow"></param>
        /// <param name="dSource"></param>
        /// <param name="sInsertSql"></param>
        /// <param name="tableName"></param>
        /// <param name="isSqlTrue"></param>
        public void ExportSqlite(DataColumnCollection dCols, object dRow, string dSource, string sInsertSql, string tableName, bool isSqlTrue)
        {

            if (isSqlTrue == true)
            {
                ExportSqlite(dCols, dRow, dSource, sInsertSql);
                return;
            }
            else
            {
                SQLiteConnection conn = new SQLiteConnection();

                string connectionstring = dSource;
                conn.ConnectionString = connectionstring;

                object[] Row = ((object[])dRow);

                try
                {
                    conn.Open();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

                //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
                SQLiteCommand cm = new SQLiteCommand();
                cm.Connection = conn;
                cm.CommandType = CommandType.Text;


                //分析多sql语句
                string[] sqls = sInsertSql.Split(';');
                for (int i = 0; i < sqls.Length; i++)
                {
                    string sql = sqls[i];
                    if (!string.IsNullOrEmpty(sql))
                    {
                        //先对sql语句进行参数整合
                        int len = sql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase) + 7;

                        string sqlpre = sql.Substring(0, sql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase));
                        string sqlValue = sql.Substring(len, sql.Length - len);

                        string sqlpara = sqlpre.Substring(sqlpre.IndexOf("(") + 1, sqlpre.IndexOf(")") - sqlpre.IndexOf("(") - 1);

                        //开始拼接一个带有参数的sql语句
                        string[] sqlparas = sqlpara.Split(',');
                        sqlpara = "";
                        for (int j = 0; j < sqlparas.Length; j++)
                        {
                            sqlpara += "@" + sqlparas[j] + ",";
                        }

                        //去掉最后一个,
                        sqlpara = sqlpara.Substring(0, sqlpara.Length - 1);

                        sql = sqlpre + " values (" + sqlpara + ")";

                        //给参数赋值
                        SQLiteParameter[] parameters = new SQLiteParameter[sqlparas.Length];
                        for (int j = 0; j < sqlparas.Length; j++)
                        {
                            int cLen = 0;
                            DbType dType = GetSqliteTableColumns(tableName, sqlparas[j], connectionstring, out cLen);
                            if (cLen == -1)
                                parameters[j] = new SQLiteParameter("@" + sqlparas[j], dType);
                            else
                                parameters[j] = new SQLiteParameter("@" + sqlparas[j], dType, cLen);
                        }

                        sqlValue = sqlValue.Substring(sqlValue.IndexOf("(") + 1, sqlValue.IndexOf(")") - sqlValue.IndexOf("(") - 1);
                        string[] sqlvalues = sqlValue.Split(',');
                        for (int j = 0; j < sqlvalues.Length; j++)
                        {
                            string sValue = sqlvalues[j];
                            sValue = Regex.Replace(sValue, "['|{|}]", "", RegexOptions.IgnoreCase).Trim();
                            for (int m = 0; m < dCols.Count; m++)
                            {
                                if (sValue == dCols[m].ColumnName)
                                {
                                    sValue = Row[m].ToString();
                                    break;
                                }
                            }
                            parameters[j].Value = sValue;
                        }



                        try
                        {
                            cm.CommandText = sql;
                            foreach (SQLiteParameter parm in parameters)
                                cm.Parameters.Add(parm);
                            cm.ExecuteNonQuery();
                        }
                        catch (System.Exception ex)
                        {
                            conn.Close();
                            throw ex;
                        }
                    }
                }

                conn.Close();
            }
        }

        /// <summary>
        /// 传入的数据库连接串需要密码需明文
        /// </summary>
        /// <param name="tName"></param>
        /// <param name="dData"></param>
        /// <param name="dSource"></param>
        /// <param name="sInserSql"></param>
        /// <param name="isSqlTrue"></param>
        public void ExportMySqlALL(string tName, DataTable dData, string dSource, string sInserSql, bool isSqlTrue)
        {
            //拼接sql语句
            int len = sInserSql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase) + 7;

            string sqlpre = sInserSql.Substring(0, sInserSql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase));
            string sqlValue = sInserSql.Substring(len, sInserSql.Length - len);

            string sqlpara = sqlpre.Substring(sqlpre.IndexOf("(") + 1, sqlpre.IndexOf(")") - sqlpre.IndexOf("(") - 1);

            //开始拼接一个带有参数的sql语句
            string[] sqlparas = sqlpara.Split(',');
            for (int index = 0; index < sqlparas.Length; index++)
            {
                sqlparas[index] = sqlparas[index].Trim();
            }
            sqlpara = "";
            for (int j = 0; j < sqlparas.Length; j++)
            {
                sqlpara += "?" + sqlparas[j] + ",";
            }

            //去掉最后一个,
            sqlpara = sqlpara.Substring(0, sqlpara.Length - 1);

            sInserSql = sqlpre + " values (" + sqlpara + ")";

            //给参数赋值
            MySqlParameter[] parameters = new MySqlParameter[sqlparas.Length];
            for (int j = 0; j < sqlparas.Length; j++)
            {
                int cLen = 0;
                MySqlDbType dType = GetMysqlTableColumns(tName, sqlparas[j], dSource, out cLen);
                if (cLen == -1)
                    parameters[j] = new MySqlParameter("?" + sqlparas[j], dType);
                else
                    parameters[j] = new MySqlParameter("?" + sqlparas[j], dType, cLen);
            }

            sqlValue = sqlValue.Substring(sqlValue.IndexOf("(") + 1, sqlValue.IndexOf(")") - sqlValue.IndexOf("(") - 1);

            //此处赋值
            string[] sqlvalues = sqlValue.Split(',');

            MySqlConnection conn = new MySqlConnection();
            string connectionstring = dSource;
            conn.ConnectionString = connectionstring;
            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            MySqlTransaction transaction = null;

            //每1000条数据，进行一次事务插入
            int tranIndex = 0;

            for (int i = 0; i < dData.Rows.Count; i++)
            {

                if (tranIndex % 1000 == 0)
                {
                    if (tranIndex > 0)
                        transaction.Commit();

                    transaction = conn.BeginTransaction();
                }

                object dRow = dData.Rows[i].ItemArray.Clone();

                object[] Row = ((object[])dRow);

                for (int j = 0; j < sqlvalues.Length; j++)
                {
                    string sValue = sqlvalues[j];
                    sValue = Regex.Replace(sValue, "['|{|}]", "", RegexOptions.IgnoreCase).Trim();
                    for (int m = 0; m < dData.Columns.Count; m++)
                    {
                        if (sValue == dData.Columns[m].ColumnName)
                        {
                            sValue = Row[m].ToString();
                            break;
                        }
                    }
                    parameters[j].Value = sValue;
                }

                MySqlCommand cm = new MySqlCommand();
                cm.Connection = conn;
                cm.Transaction = transaction;
                cm.CommandType = CommandType.Text;

                try
                {
                    cm.CommandText = sInserSql;
                    foreach (MySqlParameter parm in parameters)
                        cm.Parameters.Add(parm);

                    //cm.Parameters.AddRange(parameters);
                    cm.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    //conn.Close();
                    //throw ex;
                }
                cm.Dispose();

                tranIndex++;
            }

            transaction.Commit();

            conn.Close();
        }


        private SqlDbType GetMSSqlTableColumns(string tName, string fieldName, string strConn, out int charLen)
        {
            charLen = -1;
            try
            {
                if (m_mssqlDbType == null)
                {
                    m_mssqlDbType = new DataTable();

                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = strConn;

                    conn.Open();

                    string[] Restrictions = new string[4];
                    Restrictions[2] = tName;

                    m_mssqlDbType = conn.GetSchema("Columns", Restrictions);
                    conn.Close();
                    conn.Dispose();
                }

                for (int i = 0; i < m_mssqlDbType.Rows.Count; i++)
                {
                    if (fieldName == m_mssqlDbType.Rows[i]["COLUMN_NAME"].ToString())
                    {
                        string dbType = m_mssqlDbType.Rows[i]["DATA_TYPE"].ToString();
                        switch (dbType.ToLower())
                        {
                            case "varchar":
                                charLen = int.Parse(m_mssqlDbType.Rows[i]["CHARACTER_MAXIMUM_LENGTH"].ToString());
                                return SqlDbType.VarChar;
                            case "datetime":
                                return SqlDbType.DateTime;
                            case "smallint":
                                return SqlDbType.SmallInt;
                            case "bit":
                                return SqlDbType.Bit;
                            case "money":
                                return SqlDbType.Money;
                            case "char":
                                charLen = int.Parse(m_mssqlDbType.Rows[i]["CHARACTER_MAXIMUM_LENGTH"].ToString());
                                return SqlDbType.Char;
                            case "int":
                                return SqlDbType.Int;
                            case "float":
                                return SqlDbType.Float;
                            case "nvarchar":
                                charLen = int.Parse(m_mssqlDbType.Rows[i]["CHARACTER_MAXIMUM_LENGTH"].ToString());
                                return SqlDbType.NVarChar;
                            case "nchar":
                                charLen = int.Parse(m_mssqlDbType.Rows[i]["CHARACTER_MAXIMUM_LENGTH"].ToString());
                                return SqlDbType.NChar;
                            case "ntext":
                                return SqlDbType.NText;
                            case "text":
                                return SqlDbType.Text;

                        }
                        break;
                    }
                }
                return SqlDbType.Text;

            }
            catch (System.Exception)
            {
                return SqlDbType.Text;
            }


        }

        private MySqlDbType GetMysqlTableColumns(string tName, string fieldName, string strConn, out int charLen)
        {
            charLen = -1;

            try
            {
                if (m_mysqlDbType == null)
                {
                    m_mysqlDbType = new DataTable();

                    MySqlConnection conn = new MySqlConnection();
                    conn.ConnectionString = strConn;

                    conn.Open();

                    string[] Restrictions = new string[4];
                    Restrictions[2] = tName;

                    m_mysqlDbType = conn.GetSchema("Columns", Restrictions);

                    conn.Close();
                    conn.Dispose();
                }


                for (int i = 0; i < m_mysqlDbType.Rows.Count; i++)
                {
                    if (fieldName == m_mysqlDbType.Rows[i]["COLUMN_NAME"].ToString())
                    {
                        string dbType = m_mysqlDbType.Rows[i]["DATA_TYPE"].ToString();
                        switch (dbType.ToLower())
                        {
                            case "varchar":
                                charLen = int.Parse(m_mysqlDbType.Rows[i]["CHARACTER_MAXIMUM_LENGTH"].ToString());
                                return MySqlDbType.VarChar;
                            case "datetime":
                                return MySqlDbType.Datetime;
                            case "int16":
                                return MySqlDbType.Int16;
                            case "int32":
                                return MySqlDbType.Int32;
                            case "float":
                                return MySqlDbType.Float;
                            case "double":
                                return MySqlDbType.Double;
                            case "string":
                                return MySqlDbType.String;

                            case "text":
                                return MySqlDbType.Text;
                            case "bit":
                                return MySqlDbType.Bit;

                        }
                        break;
                    }
                }

                return MySqlDbType.Text;
            }
            catch (System.Exception)
            {
                return MySqlDbType.Text;
            }
        }

        private OracleType GetOracleTableColumns(string tName, string fieldName, string strConn, out int charLen)
        {
            charLen = -1;

            try
            {
                if (m_mysqlDbType == null)
                {
                    m_mysqlDbType = new DataTable();

                    OracleConnection conn = new OracleConnection();
                    conn.ConnectionString = strConn;

                    conn.Open();

                    string[] Restrictions = new string[3];
                    Restrictions[1] = tName;

                    m_mysqlDbType = conn.GetSchema("Columns", Restrictions);

                    conn.Close();
                    conn.Dispose();
                }


                for (int i = 0; i < m_mysqlDbType.Rows.Count; i++)
                {
                    if (fieldName.ToLower() == m_mysqlDbType.Rows[i]["COLUMN_NAME"].ToString().ToLower())
                    {
                        string dbType = m_mysqlDbType.Rows[i]["DATATYPE"].ToString();
                        switch (dbType.ToLower())
                        {
                            case "varchar":
                                charLen = int.Parse(m_mysqlDbType.Rows[i]["LENGTH"].ToString());
                                return OracleType.VarChar;
                            case "varchar2":
                                charLen = int.Parse(m_mysqlDbType.Rows[i]["LENGTH"].ToString());
                                return OracleType.VarChar;
                            case "datetime":
                                return OracleType.DateTime;
                            case "number":
                                return OracleType.Number;
                            case "int32":
                                return OracleType.Int32;
                            case "date":
                                return OracleType.DateTime;
                            case "String":
                                return OracleType.VarChar;
                            case "text":
                                return OracleType.VarChar;
                            case "bit":
                                return OracleType.Number;
                            case "clob":
                                return OracleType.Clob;
                            default:
                                charLen = 4000;
                                return OracleType.VarChar;

                        }
                        break;
                    }
                }

                return OracleType.VarChar;
            }
            catch (System.Exception)
            {
                return OracleType.VarChar;
            }
        }

        private DbType GetSqliteTableColumns(string tName, string fieldName, string strConn, out int charLen)
        {
            charLen = -1;
            try
            {
                if (m_accessDbType == null)
                {
                    m_accessDbType = new DataTable();

                    SQLiteConnection conn = new SQLiteConnection();
                    conn.ConnectionString = strConn;

                    conn.Open();

                    string[] Restrictions = new string[4];
                    Restrictions[2] = tName;

                    m_accessDbType = conn.GetSchema("Columns", Restrictions);

                    conn.Close();
                    conn.Dispose();
                }

                for (int i = 0; i < m_accessDbType.Rows.Count; i++)
                {
                    if (fieldName == m_accessDbType.Rows[i]["COLUMN_NAME"].ToString())
                    {
                        string dbType = m_accessDbType.Rows[i]["DATA_TYPE"].ToString();
                        return (DbType)(int.Parse(dbType));
                    }
                }



                return DbType.String;
            }
            catch (System.Exception)
            {
                return DbType.String;
            }
        }

        #endregion

        #region 数据库发布操作 直接写sql

        /// <summary>
        /// 传入的数据库连接串需要密码需明文
        /// </summary>
        /// <param name="dCols"></param>
        /// <param name="dRow"></param>
        /// <param name="dSource"></param>
        /// <param name="sInsertSql"></param>
        private void ExportMSSql(DataColumnCollection dCols, object dRow, string dSource, string sInsertSql)
        {
            //bool IsTable = false;

            SqlConnection conn = new SqlConnection();

            string connectionstring = dSource;

            conn.ConnectionString = connectionstring;

            object[] Row = ((object[])dRow);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

            //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
            SqlCommand cm = new SqlCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            //开始拼sql语句
            //string strInsertSql = sInsertSql;

            //需要将双引号替换成单引号
            //strInsertSql = strInsertSql.Replace("\"", "'");

            //分析多sql语句
            string[] sqls = sInsertSql.Split(';');

            for (int i = 0; i < sqls.Length; i++)
            {
                for (int j = 0; j < dCols.Count; j++)
                {
                    string strPara = "{" + dCols[j].ColumnName + "}";
                    string strParaValue = Row[j].ToString().Replace("\"", "\"\"");
                    sqls[i] = sqls[i].Replace(strPara, strParaValue);
                }
            }

            //执行sql语句
            for (int i = 0; i < sqls.Length; i++)
            {
                //分析sql语句是否可以执行，如果插入的值都是空的话，则无需插入
                string sql = sqls[i];
                if (!string.IsNullOrEmpty(sql))
                {
                    int len = sql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase) + 7;
                    string sql1 = sql.Substring(len, sql.Length - len);

                    //开始替换'  ,  还有 (  )
                    sql1 = Regex.Replace(sql1, @"[(|)|,|']", "").Trim();

                    if (sql1 != "")
                    {
                        //执行sql语句
                        try
                        {
                            cm.CommandText = sql;
                            cm.ExecuteNonQuery();
                        }
                        catch (System.Exception ex)
                        {
                            conn.Close();
                            throw ex;
                        }
                    }
                }
            }

            conn.Close();
        }

        /// <summary>
        /// 传入的数据库连接串需要密码需明文
        /// </summary>
        /// <param name="dCols"></param>
        /// <param name="dRow"></param>
        /// <param name="dSource"></param>
        /// <param name="sInsertSql"></param>
        private void ExportMySql(DataColumnCollection dCols, object dRow, string dSource, string sInsertSql)
        {
            //bool IsTable = false;

            MySqlConnection conn = new MySqlConnection();

            string connectionstring = dSource;

            conn.ConnectionString = connectionstring;

            object[] Row = ((object[])dRow);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
            MySqlCommand cm = new MySqlCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            //开始拼sql语句

            //分析多sql语句
            string[] sqls = sInsertSql.Split(';');

            for (int i = 0; i < sqls.Length; i++)
            {

                for (int j = 0; j < dCols.Count; j++)
                {
                    string strPara = "{" + dCols[j].ColumnName + "}";
                    string strParaValue = Row[j].ToString().Replace("\"", "\"\"");
                    sqls[i] = sqls[i].Replace(strPara, strParaValue);
                }
            }

            //执行sql语句
            for (int i = 0; i < sqls.Length; i++)
            {
                //分析sql语句是否可以执行，如果插入的值都是空的话，则无需插入
                string sql = sqls[i];

                if (!string.IsNullOrEmpty(sql))
                {

                    int len = sql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase) + 7;
                    string sql1 = sql.Substring(len, sql.Length - len);

                    //开始替换'  ,  还有 (  )
                    sql1 = Regex.Replace(sql1, @"[(|)|,|']", "").Trim();

                    if (sql1 != "")
                    {
                        //执行sql语句
                        try
                        {
                            cm.CommandText = sql;
                            cm.ExecuteNonQuery();


                        }
                        catch (System.Exception ex)
                        {
                            conn.Close();
                            throw ex;
                        }
                    }
                }
            }


            conn.Close();
        }

        /// <summary>
        /// 传入的数据库连接串需要密码需明文
        /// </summary>
        /// <param name="dCols"></param>
        /// <param name="dRow"></param>
        /// <param name="dSource"></param>
        /// <param name="sInsertSql"></param>
        private void ExportOracle(DataColumnCollection dCols, object dRow, string dSource, string sInsertSql)
        {
            //bool IsTable = false;

            OracleConnection conn = new OracleConnection();

            string connectionstring = dSource;

            conn.ConnectionString = connectionstring;

            object[] Row = ((object[])dRow);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }



            //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
            OracleCommand cm = new OracleCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            //开始拼sql语句

            //分析多sql语句
            string[] sqls = sInsertSql.Split(';');

            for (int i = 0; i < sqls.Length; i++)
            {

                for (int j = 0; j < dCols.Count; j++)
                {
                    string strPara = "{" + dCols[j].ColumnName + "}";
                    string strParaValue = Row[j].ToString().Replace("\"", "\"\"");
                    sqls[i] = sqls[i].Replace(strPara, strParaValue);
                }
            }

            //执行sql语句
            for (int i = 0; i < sqls.Length; i++)
            {
                //分析sql语句是否可以执行，如果插入的值都是空的话，则无需插入
                string sql = sqls[i];
                if (!string.IsNullOrEmpty(sql))
                {
                    int len = sql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase) + 7;
                    string sql1 = sql.Substring(len, sql.Length - len);

                    //开始替换'  ,  还有 (  )
                    sql1 = Regex.Replace(sql1, @"[(|)|,|']", "").Trim();

                    if (sql1 != "")
                    {
                        //执行sql语句
                        try
                        {
                            cm.CommandText = sql;
                            cm.ExecuteNonQuery();
                        }
                        catch (System.Exception ex)
                        {
                            conn.Close();
                            throw ex;
                        }
                    }
                }
            }


            conn.Close();
        }

        /// <summary>
        /// 传入的数据库连接串需要密码需明文
        /// </summary>
        /// <param name="dCols"></param>
        /// <param name="dRow"></param>
        /// <param name="dSource"></param>
        /// <param name="sInsertSql"></param>
        private void ExportSqlite(DataColumnCollection dCols, object dRow, string dSource, string sInsertSql)
        {
            //bool IsTable = false;

            SQLiteConnection conn = new SQLiteConnection();

            string connectionstring = dSource;
            conn.ConnectionString = connectionstring;

            object[] Row = ((object[])dRow);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
            SQLiteCommand cm = new SQLiteCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;


            //分析多sql语句
            string[] sqls = sInsertSql.Split(';');

            for (int i = 0; i < sqls.Length; i++)
            {
                for (int j = 0; j < dCols.Count; j++)
                {
                    string strPara = "{" + dCols[j].ColumnName + "}";
                    string strParaValue = Row[j].ToString().Replace("\"", "\"\"");
                    sqls[i] = sqls[i].Replace(strPara, strParaValue);
                }
            }

            //执行sql语句
            for (int i = 0; i < sqls.Length; i++)
            {
                //分析sql语句是否可以执行，如果插入的值都是空的话，则无需插入
                string sql = sqls[i];
                if (!string.IsNullOrEmpty(sql))
                {
                    int len = sql.IndexOf("values", StringComparison.CurrentCultureIgnoreCase) + 7;
                    string sql1 = sql.Substring(len, sql.Length - len);

                    //开始替换'  ,  还有 (  )
                    sql1 = Regex.Replace(sql1, @"[(|)|,|']", "").Trim();

                    if (sql1 != "")
                    {
                        //执行sql语句
                        try
                        {
                            cm.CommandText = sql;
                            cm.ExecuteNonQuery();
                        }
                        catch (System.Exception ex)
                        {
                            conn.Close();
                            throw ex;
                        }
                    }
                }
            }

            conn.Close();

        }

        #endregion

        public string getCreateTablesql(cGlobalParas.DatabaseType dType, string Encoding, DataColumnCollection dColumns, string TableName)
        {
            string strsql = "";

            switch (dType)
            {
                case cGlobalParas.DatabaseType.Access:
                    strsql = "create table " + TableName + "(";
                    break;
                case cGlobalParas.DatabaseType.MSSqlServer:
                    strsql = "create table [" + TableName + "](";
                    break;
                case cGlobalParas.DatabaseType.MySql:
                    strsql = "create table `" + TableName + "`(";
                    break;
                case cGlobalParas.DatabaseType.Oracle:
                    strsql = "create table " + TableName + "(";
                    break;
                default:
                    strsql = "create table " + TableName + "(";
                    break;
            }

            for (int i = 0; i < dColumns.Count; i++)
            {
                if (dColumns[i].ColumnName.ToLower() != "ispublished")
                {
                    switch (dType)
                    {
                        case cGlobalParas.DatabaseType.Access:
                            strsql += dColumns[i].ColumnName + " " + "text" + ",";
                            break;
                        case cGlobalParas.DatabaseType.MSSqlServer:
                            strsql += dColumns[i].ColumnName + " " + "ntext" + ",";
                            break;
                        case cGlobalParas.DatabaseType.MySql:
                            strsql += dColumns[i].ColumnName + " " + "text" + ",";
                            break;
                        case cGlobalParas.DatabaseType.Oracle:
                            strsql += dColumns[i].ColumnName + " " + "varchar(4000)" + ",";
                            break;
                        default:
                            strsql += dColumns[i].ColumnName + " " + "text" + ",";
                            break;
                    }
                }
            }
            strsql = strsql.Substring(0, strsql.Length - 1);
            strsql += ")";

            //如果是mysql数据库，需要根据连接串的字符集进行数据表的建立
            if (dType == cGlobalParas.DatabaseType.MySql)
            {
                if (Encoding == "" || Encoding == null)
                    Encoding = "utf8";

                strsql += " CHARACTER SET " + Encoding + " ";
            }

            return strsql;
        }
    }
}

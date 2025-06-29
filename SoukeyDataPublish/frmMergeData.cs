using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SoukeyDataPublish.Task;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using SoukeyDataPublish.db;
using NetMiner.Gather;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;

namespace SoukeyDataPublish
{
    public partial class frmMergeData : Form
    {
        private string m_SourceConn;
        private string m_TargetConn;

        public frmMergeData()
        {
            InitializeComponent();
        }

        private void frmMergeData_Load(object sender, EventArgs e)
        {
            this.comMySqlCode.Items.Add("utf8");
            this.comMySqlCode.Items.Add("big5");
            this.comMySqlCode.Items.Add("gb2312");
            this.comMySqlCode.Items.Add("gbk");
            this.comMySqlCode.Items.Add("latin1");
            this.comMySqlCode.Items.Add("latin2");
            this.comMySqlCode.Items.Add("ascii");

            this.comMySqlCode.SelectedIndex = 0;

            LoadSoukeyData();
        }

        private void LoadSoukeyData()
        {
            this.dataList.Items.Clear();

            oTaskComplete t = new oTaskComplete(Program.getPrjPath());
            IEnumerable<eTaskCompleted> ecs= t.LoadTaskData();

            foreach(eTaskCompleted ec in ecs)
            {
                this.dataList.Items.Add(ec.TaskName + "[" + ec.TaskID + "]");
            
            }

            t = null;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void butCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void butNext_Click(object sender, EventArgs e)
        {
            
            if (this.panel1.Visible == true)
            {
                if (this.MergeList.Items.Count == 0)
                {
                    MessageBox.Show("您还未选择需要合并的数据，请选择后再进行下一步操作！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                this.butPre.Enabled = true;
                this.panel1.Visible = false;
                this.panel2.Visible = true;

                this.butNext.Text = "开始合并";
            }
            else if (this.panel2.Visible == true)
            {
                string pk = "";

                if (this.MergeField.Items.Count == 0)
                {
                    MessageBox.Show("需要合并的数据没有可谓一识别的字段信息，请重新选择！","信息",MessageBoxButtons.OK ,MessageBoxIcon.Information );
                    return;
                }

                bool isSelected = false;
                for (int i = 0; i < this.MergeField.Items.Count; i++)
                {
                    if (this.MergeField.GetItemChecked(i))
                    {
                        isSelected = true;
                        pk = this.MergeField.Items[i].ToString();
                        break;
                    }
                }

                if (isSelected == false)
                {
                    MessageBox.Show("必须选择数据合并时数据唯一识别的字段！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (this.comDB.Text == "")
                {
                    MessageBox.Show("必须选择合并数据后存储的数据库！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.comDB.Focus();
                    return;
                }

                if (this.txtTableName.Text == "")
                {
                    MessageBox.Show("必须选择合并数据后存储的数据表名！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.txtTableName.Focus();
                    return;
                }

                this.butPre.Enabled = false;
                this.butNext.Enabled = false;
                this.butCancle.Enabled = false;

                try
                {
                    MergeTableDb(pk);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("数据合并发成错误，错误信息：" + ex.Message, "信息", MessageBoxButtons.OK, MessageBoxIcon.Error );

                    this.butPre.Enabled = true;
                    this.butNext.Enabled = true;
                    this.butCancle.Enabled = true;

                    return;
                }

                this.butCancle.Text = "退 出";
                this.butCancle.Enabled = true;

            }

        }

        #region 合并数据
        private void MergeTableDb(string PK)
        {
            List<cMyDataTable> tables = new List<cMyDataTable>();

            //开始初始化数据，先将数据都加载进来，开始进行合并操作
            if (this.raSoukey.Checked == true)
            {
                tables = IniSoukeyDb();
            }
            else if (this.raAccess.Checked == true)
            {
                tables = IniAccessDb();
            }
            else if (this.raSqlServer.Checked == true)
            {
                tables = IniMSSqlServerDb();
            }
            else if (this.raMySql.Checked == true)
            {
                tables = IniMySqlDb();
            }

            //开始将数据进行横向合并，以最多行数的数据为基准
            tables.Sort();

            //建立一个大表，涵盖所有的字段
            DataTable md = new DataTable();
            for (int i = 0; i < tables.Count; i++)
            {
                for (int j = 0; j < tables[i].Columns.Count; j++)
                {
                    DataColumn dc = new DataColumn();
                    dc.ColumnName = tables[i].Columns[j].ColumnName;

                    if (!md.Columns.Contains(tables[i].Columns[j].ColumnName))
                    {
                        md.Columns.Add(dc);
                    }

                }
            }

            //先将第一张表的数据插入进去，然后根据这张表的数据进行合并
            for (int j = 0; j < tables[0].Rows.Count; j++)
            {
                DataRow r = md.NewRow();

                for (int i = 0; i < tables[0].Columns.Count; i++)
                {
                    if (md.Columns.Contains(tables[0].Columns[i].ColumnName))
                    {
                        r[tables[0].Columns[i].ColumnName] = tables[0].Rows[j][tables[0].Columns[i].ColumnName].ToString();
                    }
                    
                }

                md.Rows.Add(r);
            }

            tables.RemoveAt(0);
            

            //开始合并数据
            for (int i = 0; i < tables.Count; i++)
            {
               
                //开始插入后续表的内容，在插入的时候，需要判断数据合并的内容
                for (int m = 0; m < tables[i].Rows.Count; m++)
                {
                    string pkValue = tables[i].Rows[m][PK].ToString();
                    int rowIndex = GetRowsIndex(ref md,PK, pkValue);

                    if (rowIndex != -1)
                    {
                        for (int j = 0; j < tables[i].Columns.Count ; j++)
                        {
                            if (md.Columns.Contains ( tables[i].Columns[j].ColumnName ))
                            {
                                md.Rows[rowIndex][tables[i].Columns[j].ColumnName] = tables[i].Rows[m][tables[i].Columns[j].ColumnName];
                            }
                        }
                    }
                }
                
            }

            //将合并好的数据插入到数据库
            ExportData(md);

        }

        private void ExportData(DataTable d)
        {
            string conn="";

            if (this.radioButton5.Checked == true)
            {
                conn=GetMsSql (this.comDB.Text);
                ExportMSSql(d,this.txtTableName.Text ,conn);
            }
            else if (this.radioButton6.Checked == true)
            {
                conn = GetMySql(this.comDB.Text);
                ExportMySql(d, this.txtTableName.Text, conn);
            }
            else if (this.radioButton7.Checked == true)
            {
                conn = GetAccessConn();
                ExportAccess(d, this.txtTableName.Text, conn);
            }
        }

        private void ExportMSSql(DataTable d,string tName,string dbConn)
        {
            bool IsTable = false;

            SqlConnection conn = new SqlConnection();

            string connectionstring = dbConn;

            conn.ConnectionString = ToolUtil.DecodingDBCon (connectionstring);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("数据库连接失败，错误信息：" + ex.Message);
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[2].ToString() == tName)
                {
                    IsTable = true;
                    break;
                }
            }

            if (IsTable == false)
            {
                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.MSSqlServer,"", d.Columns, tName);

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
                        throw new System.Exception ("数据表建立失败，错误信息：" + ex.Message);
                    }
                }

            }
            else
            {
                conn.Close();
                conn.Dispose();

                throw new System.Exception("已存在此表，请更换目标数据表名称！");
                return;
            }
          
            //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
            SqlCommand cm = new SqlCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            string sql = "insert into " + tName + " (";

            for (int i = 0; i < d.Columns.Count; i++)
            {
                sql += d.Columns[i].ColumnName + ",";
            }

            sql = sql.Substring(0, sql.Length - 1) + ") values (";

            for (int i = 0; i < d.Rows.Count; i++)
            {
                for (int j = 0; j < d.Columns.Count; j++)
                {
                    sql += "'" + d.Rows[i][d.Columns[j].ColumnName].ToString() + "',";
                }

                sql = sql.Substring(0, sql.Length - 1) + ")";

                cm.CommandText = sql;
                cm.ExecuteNonQuery();
            }

            cm.Dispose();
            conn.Close();
            conn.Dispose();
        }

        private void ExportMySql(DataTable d, string tName, string dbConn)
        {
            bool IsTable = false;

            MySqlConnection conn = new MySqlConnection();

            string connectionstring = dbConn;

            conn.ConnectionString = ToolUtil.DecodingDBCon (connectionstring);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception ("数据库连接失败，错误信息：" + ex.Message);
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (string.Compare(r[2].ToString(), tName, true) == 0)
                {
                    IsTable = true;
                    break;
                }
            }

            if (IsTable == false)
            {
                string encoding = this.comMySqlCode.Text;

                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.MySql, encoding,d.Columns,tName );

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
                        conn.Close();
                        conn.Dispose();

                        throw new System.Exception("数据表建立失败，错误信息：" + ex.Message);
                        return;
                    }
                }
            }
            else
            {
                conn.Close();
                conn.Dispose();

                throw new System.Exception("已存在此表，请更换目标数据表名称！");
                return;
            }


            //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
            MySqlCommand cm = new MySqlCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            string sql = "insert into " + tName + " (";

            for (int i = 0; i < d.Columns.Count; i++)
            {
                sql += d.Columns[i].ColumnName + "'";
            }

            sql = sql.Substring(0, sql.Length - 1) + ") values (";

            for (int i = 0; i < d.Rows.Count; i++)
            {
                for (int j = 0; j < d.Columns.Count; j++)
                {
                    sql += "'" + d.Rows[i][d.Columns[j].ColumnName].ToString() + "',";
                }

                sql = sql.Substring(0, sql.Length - 1) + ")";

                cm.CommandText = sql;
                cm.ExecuteNonQuery();
            }

            cm.Dispose();
            conn.Close();
            conn.Dispose();
        }

        private void ExportAccess(DataTable d, string tName, string dbConn)
        {
            bool IsTable = false;

            OleDbConnection conn = new OleDbConnection();

            string connectionstring = dbConn;

            conn.ConnectionString = ToolUtil.DecodingDBCon (connectionstring);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception ("数据库连接失败，错误信息：" + ex.Message);
                return;
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[3].ToString() == "TABLE")
                {
                    if (r[2].ToString() == tName)
                    {
                        IsTable = true;
                        break;
                    }
                }

            }

            if (IsTable == false)
            {
                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.Access, "",d.Columns,tName);

                OleDbCommand com = new OleDbCommand();
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
                        conn.Dispose();

                        throw new System.Exception("数据库连接失败，错误信息：" + ex.Message);
                        return;
                    }
                }

            }
            else
            {
                conn.Close();
                conn.Dispose();

                throw new System.Exception("已存在此表，请更换目标数据表名称！");
                return;
            }

            //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
            OleDbCommand cm = new OleDbCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            string sql = "insert into " + tName + " (";

            for (int i = 0; i < d.Columns.Count; i++)
            {
                sql += d.Columns[i].ColumnName + "'";
            }

            sql = sql.Substring(0, sql.Length - 1) + ") values (";

            for (int i = 0; i < d.Rows.Count; i++)
            {
                for (int j = 0; j < d.Columns.Count; j++)
                {
                    sql += "'" + d.Rows[i][d.Columns[j].ColumnName].ToString() + "',";
                }

                sql = sql.Substring(0, sql.Length - 1) + ")";

                cm.CommandText = sql;
                cm.ExecuteNonQuery();
            }

            cm.Dispose();
            conn.Close();
            conn.Dispose();
        }

        private string getCreateTablesql(cGlobalParas.DatabaseType dType, string Encoding, DataColumnCollection dColumns, string TableName)
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
                default:
                    strsql = "create table " + TableName + "(";
                    break;
            }
            
            for (int i = 0; i < dColumns.Count; i++)
            {
                switch (dType)
                {
                    case cGlobalParas.DatabaseType.Access:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                    case cGlobalParas.DatabaseType.MSSqlServer:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                    case cGlobalParas.DatabaseType.MySql:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                    default:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
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

        private int GetRowsIndex(ref DataTable t,string pk, string pkValue)
        {
            int index = -1;

            for (int i = 0; i < t.Rows.Count; i++)
            {
                if (t.Rows[i][pk].ToString() == pkValue)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private List<cMyDataTable> IniSoukeyDb()
        {
            List<cMyDataTable> tables = new List<cMyDataTable>();

            for (int i = 0; i < this.MergeList.Items.Count; i++)
            {
                cMyDataTable d = new cMyDataTable();

                string tName = this.MergeList.Items[i].Text;
                string ID=tName.Substring (tName.IndexOf ("[")+1,tName.IndexOf ("]")-tName.IndexOf ("[")-1);

                oTaskComplete t = new oTaskComplete(Program.getPrjPath());
                eTaskCompleted ec= t.LoadSingleTask(long.Parse(ID));
                d.ReadXml(ec.TempFile);
                t = null;

                tables.Add(d);
            }

            return tables;
        }

        private List<cMyDataTable> IniMSSqlServerDb()
        {
            List<cMyDataTable> tables = new List<cMyDataTable>();

            for (int i = 0; i < this.MergeList.Items.Count; i++)
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = ToolUtil.DecodingDBCon (this.m_SourceConn);
                conn.Open();

                SqlCommand com = new SqlCommand();
                com.Connection = conn;
                com.CommandType = CommandType.Text;

                //先把调拨明细数据查询出来
                string sql = "select * from " + this.MergeList.Items[i].Text;

                com.CommandText = sql;

                SqlDataAdapter da = new SqlDataAdapter(com);

                cMyDataTable dt = new cMyDataTable();
                da.Fill(dt);

                tables.Add(dt);

                dt = null;
                da.Dispose();
                com.Dispose();
                conn.Close();
                conn.Dispose();
            }

            return tables;
        }

        private List<cMyDataTable> IniMySqlDb()
        {
            List<cMyDataTable> tables = new List<cMyDataTable>();

            for (int i = 0; i < this.MergeList.Items.Count; i++)
            {
                MySqlConnection conn = new MySqlConnection();
                conn.ConnectionString = ToolUtil.DecodingDBCon (this.m_SourceConn);
                conn.Open();

                MySqlCommand com = new MySqlCommand();
                com.Connection = conn;
                com.CommandType = CommandType.Text;

                //先把调拨明细数据查询出来
                string sql = "select * from " + this.MergeList.Items[i].Text;

                com.CommandText = sql;

                MySqlDataAdapter da = new MySqlDataAdapter(com);

                cMyDataTable dt = new cMyDataTable();
                da.Fill(dt);

                tables.Add(dt);

                dt = null;
                da.Dispose();
                com.Dispose();
                conn.Close();
                conn.Dispose();
            }

            return tables;
        }

        private List<cMyDataTable> IniAccessDb()
        {
            List<cMyDataTable> tables = new List<cMyDataTable>();

            for (int i = 0; i < this.MergeList.Items.Count; i++)
            {
                OleDbConnection conn = new OleDbConnection();
                conn.ConnectionString = ToolUtil.DecodingDBCon (this.m_SourceConn);
                conn.Open();

                OleDbCommand com = new OleDbCommand();
                com.Connection = conn;
                com.CommandType = CommandType.Text;

                //先把调拨明细数据查询出来
                string sql = "select * from " + this.MergeList.Items[i].Text;

                com.CommandText = sql;

                OleDbDataAdapter da = new OleDbDataAdapter(com);

                cMyDataTable dt = new cMyDataTable();
                da.Fill(dt);

                tables.Add(dt);

                dt = null;
                da.Dispose();
                com.Dispose();
                conn.Close();
                conn.Dispose();
            }


            return tables;
        }

        #endregion

        private void butPre_Click(object sender, EventArgs e)
        {
            if (this.panel2.Visible == true)
            {
                this.butPre.Enabled = false ;
                
                this.panel1.Visible = true ;
                this.panel2.Visible = false ;

                this.butNext.Text = "下一步";
            }
        }

        private void GetDataInfo(int type)
        {
            this.dataList.Items.Clear();
            this.MergeList.Items.Clear();
            this.MergeField.Items.Clear();

            frmSetData fSD = new frmSetData();

            if (type==0)
                fSD.FormState = 0;
            else if (type == 1)
                fSD.FormState = 1;
            else if (type==2)
                fSD.FormState = 2;

            fSD.rDataSource = new frmSetData.ReturnDataSource(GetDataSource);
            fSD.ShowDialog();
            fSD.Dispose();
            
        }

        private void LoadAccess(string strCon)
        {
            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (strCon);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("数据库无法连接，错误信息" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");


            foreach (DataRow r in tb.Rows)
            {
                if (r[3].ToString() == "TABLE")
                {
                    this.dataList.Items.Add(r[2].ToString());
                }

            }

            conn.Close();
            conn.Dispose();

        }

        private void LoadMSSqlServer(string strCon)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString =ToolUtil.DecodingDBCon ( strCon);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("数据库无法连接，错误信息" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");

          

            foreach (DataRow r in tb.Rows)
            {

                this.dataList.Items.Add(r[2].ToString());

            }

            conn.Close();
            conn.Dispose();
        }

        private void LoadMySql(string strCon)
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (strCon);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("数据库无法连接，错误信息" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");
        

            foreach (DataRow r in tb.Rows)
            {

                this.dataList.Items.Add(r[2].ToString());

            }

            conn.Close();
            conn.Dispose();
        }

        private void GetDataSource(string strDataConn)
        {
            string conn = strDataConn;

            this.m_SourceConn = conn;

            if (this.raAccess.Checked ==true )
                LoadAccess(conn);
            else if (this.raSqlServer.Checked ==true )
                LoadMSSqlServer(conn);
            else if (this.raMySql.Checked ==true )
                LoadMySql(conn);
        }

        private void raSqlServer_Click(object sender, EventArgs e)
        {
            GetDataInfo(1);
        }

        private void raMySql_Click(object sender, EventArgs e)
        {
            GetDataInfo(2);
        }

        private void raAccess_Click(object sender, EventArgs e)
        {
            GetDataInfo(0);
        }

        private void raSoukey_Click(object sender, EventArgs e)
        {
            LoadSoukeyData();

            this.m_SourceConn = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.dataList.SelectedItems.Count == 0)
                return;

            string sText=this.dataList.SelectedItem.ToString ();

            ListViewItem cItem = new ListViewItem();
            cItem.Name = sText;
            cItem.Text = sText;

            try
            {
                List<string> f = new List<string>();
                f = GetField(sText);

                for (int i = 0; i < f.Count; i++)
                {
                    cItem.SubItems.Add(f[i].ToString());
                }

                this.MergeList.Items.Add(cItem);

                this.dataList.Items.Remove(this.dataList.SelectedItem);

                MergeFields();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("添加待合并数据失败，有可能是数据无效，或数据已经删除！错误信息：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> GetField(string sText)
        {
            List<string> Fields = new List<string>();

            if (this.raSoukey.Checked == true)
            {
                Fields=GetFieldSoukey(sText);
            }
            else if (this.raAccess.Checked == true)
            {
                Fields = GetFieldAccess(sText);
            }
            else if (this.raSqlServer.Checked == true)
            {
                Fields = GetFieldSqlServer(sText);
            }
            else if (this.raMySql.Checked == true)
            {
                Fields = GetFieldMySql(sText);
            }

            return Fields;
        }

        private void MergeFields()
        {
            List<string> r = new List<string>();
            List<string> r1 = null;

            for (int i = 0; i < this.MergeList.Items.Count; i++)
            {
                //string sc = this.listTask.Items[i].SubItems[2].Text.ToString();
                //sc += ",";
                r1 = new List<string>();

                for (int j=1;j<this.MergeList.Items[i].SubItems.Count ;j++)
                {
                    string ss = this.MergeList.Items[i].SubItems[j].Text.ToString();

                    if (r.Count == 0)
                    {
                        r1.Add(ss);
                    }
                    else
                    {
                        for (int m = 0; m < r.Count; m++)
                        {
                            if (ss == r[m].ToString())
                            {
                                r1.Add(ss);
                            }
                        }

                    }
                }

                r = r1;

                r1 = null;

            }

            r1 = null;

            this.MergeField.Items.Clear();

            for (int i = 0; i < r.Count; i++)
            {
                this.MergeField.Items.Add(r[i].ToString());
            }
           

        }

        private List<string> GetFieldSoukey(string tName)
        {
            string ID=tName.Substring (tName.IndexOf ("[")+1,tName.IndexOf ("]")-tName.IndexOf ("[")-1);

            oTaskComplete t = new oTaskComplete(Program.getPrjPath());

            eTaskCompleted ec= t.LoadSingleTask(long.Parse (ID));
            string fileName = ec.TempFile;
            t = null;

            DataTable d = new DataTable();
            d.ReadXml(fileName);

            List<string> ss = new List<string>();
            for (int i = 0; i< d.Columns.Count; i++)
            {
                ss.Add(d.Columns[i].ColumnName);
            }
            d = null;

            return ss;
        }

        private List<string> GetFieldSqlServer(string tName)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (this.m_SourceConn);

            conn.Open();

            string[] Restrictions = new string[4];
            Restrictions[2] = tName;

            DataTable tc = conn.GetSchema("Columns", Restrictions);

            List<string> strColumns = new List<string>();

            for (int i = 0; i < tc.Rows.Count; i++)
            {
                strColumns.Add(tc.Rows[i][3].ToString());
            }

            tc = null;

            return strColumns;

        }

        private List<string> GetFieldAccess(string tName)
        {
            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (this.m_SourceConn);

            conn.Open();

            string[] Restrictions = new string[4];
            Restrictions[2] = tName;

            DataTable tc = conn.GetSchema("Columns", Restrictions);

            List<string> strColumns = new List<string>();

            for (int i = 0; i < tc.Rows.Count; i++)
            {
                strColumns.Add(tc.Rows[i][3].ToString());
            }

            tc = null;

            return strColumns;
        }

        private List<string> GetFieldMySql(string tName)
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (this.m_SourceConn);

            conn.Open();

            string[] Restrictions = new string[4];
            Restrictions[2] = tName;

            DataTable tc = conn.GetSchema("Columns", Restrictions);

            List<string> strColumns = new List<string>();

            for (int i = 0; i < tc.Rows.Count; i++)
            {
                strColumns.Add(tc.Rows[i][3].ToString());
            }

            tc = null;

            return strColumns;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.MergeList.SelectedItems.Count == 0)
                return;

            this.dataList.Items.Add(this.MergeList.SelectedItems[0].Text);

            this.MergeList.Items.Remove(this.MergeList.SelectedItems[0]);

            MergeFields();

        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            this.txtMySqlNumber.Text = "1433";
            this.txtMySqlUser.Text = "sa";

            this.label13.Enabled = false;
            this.comMySqlCode.Enabled = false;

            this.label9.Enabled = true;
            this.txtMySqlNumber.Enabled = true;

            this.label4.Enabled = true;
            this.comDB.Enabled = true;

        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            this.txtMySqlNumber.Text = "3306";
            this.txtMySqlUser.Text = "root";

            this.label13.Enabled = true;
            this.comMySqlCode.Enabled = true ;

            this.label9.Enabled = true;
            this.txtMySqlNumber.Enabled = true;

            this.label4.Enabled = true;
            this.comDB.Enabled = true;

        }

        private void radioButton7_Click(object sender, EventArgs e)
        {
            this.label13.Enabled = false;
            this.comMySqlCode.Enabled = false;

            this.label9.Enabled = false;
            this.txtMySqlNumber.Enabled = false;

            this.label4.Enabled = false;
            this.comDB.Enabled = false;

            this.txtMySqlUser.Text = "";

            this.openFileDialog1.Title = "请选择Access数据库";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "Access Files(*.mdb)|*.mdb|All Files(*.*)|*.*";


            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtMySql.Text = this.openFileDialog1.FileName;
            }
        }

        private void FillMySqlDB()
        {

            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (GetMySql(""));

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("数据库连接失败，错误信息：" + ex.Message,"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Databases");

            foreach (DataRow r in tb.Rows)
            {

                this.comDB.Items.Add(r[1].ToString());

            }
        }

        private void FillSqlServerDB()
        {

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString =ToolUtil.DecodingDBCon ( GetMsSql(""));

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("数据库连接失败，错误信息：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Databases");

            foreach (DataRow r in tb.Rows)
            {

                this.comDB.Items.Add(r[0].ToString());

            }
        }

        private string GetMySql(string dbName)
        {
            string connectionstring = "";
            connectionstring = "Server=" + this.txtMySql.Text + ";";
            connectionstring += "Port=" + this.txtMySqlNumber.Text + ";";

            if (dbName == "")
            {
                connectionstring += " Database=mysql;User Id=" + this.txtMySqlUser.Text + ";password=" + this.txtMySqlPwd.Text + ";";
            }
            else
            {
                connectionstring += " Database=" + dbName + ";User Id=" + this.txtMySqlUser.Text + ";password=" + this.txtMySqlPwd.Text + ";";
            }

            
            connectionstring += " character set=" + this.comMySqlCode.SelectedItem.ToString() + ";";
            return connectionstring;
        }

        private string GetMsSql(string dbName)
        {
            string connectionstring = "";

            if (dbName == "")
            {
                connectionstring = "Data Source=" + this.txtMySql.Text + ";initial catalog=master ;";
            }
            else
            {
                connectionstring = "Data Source=" + this.txtMySql.Text + ";initial catalog=" + dbName + " ;";
            }

            connectionstring += "user id=" + this.txtMySqlUser.Text + ";password=" + this.txtMySqlPwd.Text;

            return connectionstring;
        }

        private string GetAccessConn()
        {

            string connectionstring = string.Empty;

            if (this.txtMySql.Text.Trim().EndsWith("accdb", StringComparison.CurrentCultureIgnoreCase))
                connectionstring = "provider=Microsoft.ACE.OLEDB.12.0;data source=";
            else
                connectionstring = "provider=microsoft.jet.oledb.4.0;data source=";
            connectionstring += this.txtMySql.Text + ";";

            //string connectionstring = "provider=microsoft.jet.oledb.4.0;data source=";
            //connectionstring += this.txtMySql.Text + ";";

            if (this.txtMySqlUser.Text.Trim() != "")
            {
                connectionstring += "User ID=" + this.txtMySqlUser.Text + ";Jet OLEDB:Database Password=" + this.txtMySqlPwd.Text + ";Persist Security Info=true;";


            }

            return connectionstring;
        }

        private void comDB_DropDown(object sender, EventArgs e)
        {
            this.comDB.Items.Clear();

            if (this.radioButton5.Checked == true)
            {
                FillSqlServerDB();
            }
            else if (this.radioButton6.Checked == true)
            {
                FillMySqlDB();
            }
        }

      
    
    }
}

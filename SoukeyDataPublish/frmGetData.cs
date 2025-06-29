using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Resources;
using System.Reflection;
using NetMiner.Gather;
using NetMiner.Resource;
using NetMiner.Common;

namespace SoukeyDataPublish
{
    public partial class frmGetData : Form
    {
        public delegate void ReturnDataSource(string strCon,string sql,cGlobalParas.DatabaseType dType);
        public ReturnDataSource rDataSource;

        private ResourceManager rm;

        public frmGetData()
        {
            InitializeComponent();
        }

        ///进入此窗体需要显示的数据信息
        ///0-Access; 1-MS SqlServer; 2-MySql
        private cGlobalParas.DatabaseType m_DataType;
        public cGlobalParas.DatabaseType DataType
        {
            get { return m_DataType; }
            set { m_DataType = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = rm.GetString ("Info84");

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "Access Files(*.mdb)|*.mdb|All Files(*.*)|*.*";


            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtAccessName.Text = this.openFileDialog1.FileName;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == true)
            {
                this.txtAccessUser.Enabled = true;
                this.txtAccessPwd.Enabled = true;
            }
            else
            {
                this.txtAccessUser.Enabled = false;
                this.txtAccessPwd.Enabled = false;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked == true)
            {
                this.txtSqlServerUser.Enabled = false;
                this.txtSqlServerPwd.Enabled = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton2.Checked == true)
            {
                this.txtSqlServerUser.Enabled = true;
                this.txtSqlServerPwd.Enabled = true;
            }
        }

        private void frmSetData_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            this.comMySqlCode.Items.Add("utf8");
            this.comMySqlCode.Items.Add("big5");
            this.comMySqlCode.Items.Add("gb2312");
            this.comMySqlCode.Items.Add("gbk");
            this.comMySqlCode.Items.Add("latin1");
            this.comMySqlCode.Items.Add("latin2");
            this.comMySqlCode.Items.Add("ascii");

            this.comMySqlCode.SelectedIndex = 0;

            switch (this.m_DataType)
            {
                case  cGlobalParas.DatabaseType .Access :
                    this.panel1.Visible = true;
                    this.panel2.Visible = false;
                    this.panel3.Visible = false;
                    break;
                case cGlobalParas.DatabaseType.MSSqlServer :
                    this.panel1.Visible = false;
                    this.panel2.Visible = true;
                    this.panel3.Visible = false;
                    break;
                case cGlobalParas.DatabaseType.MySql :
                    this.panel1.Visible = false;
                    this.panel2.Visible = false;
                    this.panel3.Visible = true;
                    break;
                default :
                    break ;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string conn="";

            if (this.panel1.Visible == true)
            {
                //access数据库
                conn = GetAccessConn();
            }
            else if (this.panel2.Visible == true)
            {
                //mssqlserver数据库
                if (this.comSqlServerData.Text.Trim() == "")
                {
                    this.comSqlServerData.Focus();
                    MessageBox.Show(rm.GetString ("Info85"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    return;
                }

                string connectionstring = "";

                connectionstring = "Data Source=" + this.txtSqlserver.Text + ";initial catalog=" + this.comSqlServerData.SelectedItem.ToString () + ";";

                if (this.radioButton1.Checked == true)
                    connectionstring += "Integrated Security=True;";
                else if (this.radioButton2.Checked == true)
                    connectionstring += "user id=" + this.txtSqlServerUser.Text + ";password=" + this.txtSqlServerPwd.Text;

                conn = connectionstring;

            }
            else if (this.panel3.Visible == true)
            {
                //mysql数据库
                if (this.comMySqlData.Text.Trim() == "")
                {
                    this.comMySqlData.Focus();
                    MessageBox.Show(rm.GetString ("Info86"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }

                string connectionstring = "";
                connectionstring = "Server=" + this.txtMySql.Text + ";";
                connectionstring += "Port=" + this.txtMySqlNumber.Text + ";";
                connectionstring += " Database=" + this.comMySqlData.SelectedItem.ToString () + ";";
                connectionstring +=" User Id=" + this.txtMySqlUser.Text + ";password=" + this.txtMySqlPwd.Text + ";";
                connectionstring += " character set=" + this.comMySqlCode.SelectedItem.ToString() + ";";
                conn = connectionstring;
            }

            if (conn=="" || this.txtdsql.Text.Trim () =="")
            {
                MessageBox.Show(rm.GetString("Info2243"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            rDataSource(conn,this.txtdsql .Text ,m_DataType );
            this.Close();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (this.panel1.Visible == true)
            {
                //access数据库
                TestAccess();
            }
            else if (this.panel2.Visible == true)
            {
                //mssqlserver数据库
                TestMsSql();
            }
            else if (this.panel3.Visible == true)
            {
                //mysql数据库
                TestMySql();
            }
        }

        private void TestAccess()
        {
            OleDbConnection conn = new OleDbConnection();


            conn.ConnectionString = ToolUtil.DecodingDBCon (GetAccessConn());

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info75") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            conn.Close ();

            MessageBox.Show(rm.GetString("Info87"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void TestMsSql()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (GetMsSql());

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info75") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            conn.Close();

            MessageBox.Show(rm.GetString("Info87"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TestMySql()
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString =ToolUtil.DecodingDBCon ( GetMySql());

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info75") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            conn.Close();

            MessageBox.Show(rm.GetString("Info87"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GetAccessConn()
        {

            string connectionstring = string.Empty;

            if (this.txtAccessName.Text.Trim().EndsWith("accdb", StringComparison.CurrentCultureIgnoreCase))
                connectionstring = "provider=Microsoft.ACE.OLEDB.12.0;data source=";
            else
                connectionstring = "provider=microsoft.jet.oledb.4.0;data source=";
            connectionstring += this.txtAccessName.Text + ";";

            //string connectionstring = "provider=microsoft.jet.oledb.4.0;data source=";
            //connectionstring += this.txtAccessName.Text + ";";

            if (this.txtAccessUser.Text.Trim() != "")
            {
                connectionstring += "User ID=" + this.txtAccessUser.Text + ";Jet OLEDB:Database Password=" + this.txtAccessPwd.Text + ";Persist Security Info=true;";


            }

            return connectionstring;
        }

        private string GetMsSql()
        {
            string connectionstring = "";

            if (this.comSqlServerData.SelectedItem ==null)
                connectionstring = "Data Source=" + this.txtSqlserver.Text + ";initial catalog=master ;";
            else
                connectionstring = "Data Source=" + this.txtSqlserver.Text + ";initial catalog=" + this.comSqlServerData.SelectedItem.ToString ()  + ";";

            if (this.radioButton1.Checked ==true )
                connectionstring += "Integrated Security=True;"; 
            else if (this.radioButton2.Checked ==true )
                connectionstring += "user id=" + this.txtSqlServerUser.Text + ";password=" + ToolUtil.Base64Encoding(this.txtSqlServerPwd.Text) + ";";

            return connectionstring;
        }

        private string GetMySql()
        {
            string connectionstring = "";
            connectionstring = "Server=" + this.txtMySql.Text + ";";
            connectionstring += "Port=" + this.txtMySqlNumber.Text + ";";
            if (this.comMySqlData.SelectedItem ==null)
                connectionstring +=" Database=mysql;User Id=" + this.txtMySqlUser.Text  + ";password=" + ToolUtil.Base64Encoding(this.txtMySqlPwd .Text) + ";" ;
            else
                connectionstring += " Database=" + this.comMySqlData.SelectedItem.ToString () + ";User Id=" + this.txtMySqlUser.Text + ";password=" + ToolUtil.Base64Encoding(this.txtMySqlPwd.Text) + ";";
                
            connectionstring += " character set=" + this.comMySqlCode.SelectedItem.ToString() + ";";
            return connectionstring;
        }

        private void comSqlServerData_DropDown(object sender, EventArgs e)
        {
            if (this.comSqlServerData.Items.Count != 0)
                return;

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString =ToolUtil.DecodingDBCon ( GetMsSql());

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info75") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Databases");

            foreach (DataRow r in tb.Rows)
            {
                
               this.comSqlServerData.Items.Add(r[0].ToString());
                
            }
        }

        private void comMySqlData_DropDown(object sender, EventArgs e)
        {
            if (this.comMySqlData.Items.Count != 0)
                return;

            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (GetMySql());

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info75") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Databases");

            foreach (DataRow r in tb.Rows)
            {

                this.comMySqlData.Items.Add(r[1].ToString());

            }
        }

        private void frmSetData_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void FillAccessTable()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (GetAccessConn());

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[3].ToString() == "TABLE")
                {
                    this.comTableName.Items.Add(r[2].ToString());
                }

            }

        }

        private void FillMSSqlTable()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (GetMsSql());

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {

                this.comTableName.Items.Add(r[2].ToString());

            }
        }

        private void FillMySql()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (GetMySql());

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {

                this.comTableName.Items.Add(r[2].ToString());

            }
        }

        private void comTableName_DropDown(object sender, EventArgs e)
        {
            switch (m_DataType)
            {
                case cGlobalParas.DatabaseType.Access :
                    FillAccessTable();
                    break;
                case cGlobalParas.DatabaseType.MSSqlServer :
                    FillMSSqlTable();
                    break;
                case cGlobalParas.DatabaseType.MySql :
                    FillMySql();
                    break;
                default :
                    break;
            }
        }

        private void comTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comTableName.SelectedItem!=null)
                this.txtdsql.Text = "select * from " + this.comTableName.SelectedItem.ToString();
        }

        private void comMySqlData_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NetMiner.Resource;

namespace MinerSpider
{
    public partial class frmAddUrlFromDb : Form
    {
        public delegate void ReturnGatherUrl(cGlobalParas.FormState fState, cGatherUrlFormConfig fgUrlRule);
        public ReturnGatherUrl rGUrl;

        public frmAddUrlFromDb()
        {
            InitializeComponent();
        }

        private cGlobalParas.FormState m_fState;
        public cGlobalParas.FormState fState
        {
            get { return m_fState; }
            set { m_fState = value; }
        }

        public void LoadInfo(string Url)
        {
            Match charSetMatch = Regex.Match(Url, "(?<=<DbType>)[\\s\\S]*?(?=</DbType>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string dType = charSetMatch.Groups[0].ToString();

            if (int.Parse(dType) == (int)cGlobalParas.DatabaseType.Access)
                this.raExportAccess.Checked = true;
            else if (int.Parse(dType) == (int)cGlobalParas.DatabaseType.MSSqlServer)
                this.raExportMSSQL.Checked = true;
            else if (int.Parse(dType) == (int)cGlobalParas.DatabaseType.MySql)
                this.raExportMySql.Checked = true;

            charSetMatch = Regex.Match(Url, "(?<=<Con>)[\\s\\S]*?(?=</Con>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
           this.txtDataSource.Text  = charSetMatch.Groups[0].ToString();

           charSetMatch = Regex.Match(Url, "(?<=<Sql>)[\\s\\S]*?(?=</Sql>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
           this.txtSql.Text = charSetMatch.Groups[0].ToString();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            frmSetData fSD = new frmSetData();

            if (this.raExportAccess.Checked == true)
                fSD.FormState = 0;
            else if (this.raExportMSSQL.Checked == true)
                fSD.FormState = 1;
            else if (this.raExportMySql.Checked == true)
                fSD.FormState = 2;

            fSD.rDataSource = new frmSetData.ReturnDataSource(GetDataSource);
            fSD.ShowDialog();
            fSD.Dispose();
        }

        private void GetDataSource(string strDataConn)
        {
            this.txtDataSource.Text = strDataConn;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {

            if (this.txtDataSource.Text.Trim() == "")
            {
                MessageBox.Show("请先选择数据库，并配置数据库连接字符串！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.button12.Focus();
                return;
            }

            if (this.txtSql.Text.Trim() == "")
            {
                MessageBox.Show("必须配置Sql语句，且返回的字段只能有一个，且必须是网址信息！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.button12.Focus();
                return;
            }

            cGatherUrlFormConfig cURule = new cGatherUrlFormConfig();

            cGlobalParas.DatabaseType dType = cGlobalParas.DatabaseType.Access;
            if (this.raExportAccess.Checked == true)
                dType = cGlobalParas.DatabaseType.Access;
            else if (this.raExportMSSQL.Checked == true)
                dType = cGlobalParas.DatabaseType.MSSqlServer;
            else if (this.raExportMySql.Checked == true)
                dType = cGlobalParas.DatabaseType.MySql;

            string url = "{DbUrl:<DbType>" + (int)dType + "</DbType><Con>" + this.txtDataSource.Text + "</Con><Sql>" + this.txtSql.Text + "</Sql>}";
            cURule.Url = url;
            cURule.IsNav = false;
            cURule.IsMulti = false;
            cURule.IsNext = false;
            cURule.isMulti121 = false;

            rGUrl(this.fState, cURule);
            this.Close();
        }
    }
}

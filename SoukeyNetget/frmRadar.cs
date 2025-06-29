using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using NetMiner.Gather;
using NetMiner.Gather.Task;
using NetMiner.Gather.Radar;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Core.gTask;
using NetMiner.Core.Radar;
using NetMiner.Core.Radar.Entity;

namespace MinerSpider
{
    public partial class frmRader : Form
    {
        private ResourceManager rm;

        //定义一个变量存储雷达数据库的类型
        private cGlobalParas.DatabaseType m_RadarDbType;

        private cGlobalParas.FormState m_FormState;
        public cGlobalParas.FormState FormState
        {
            get { return m_FormState; }
            set { m_FormState = value; }
        }
        

        public frmRader()
        {
            InitializeComponent();

            rm = new ResourceManager("SoRadat.Resources.globalUI", Assembly.Load("NetMiner.Resource"));
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

    
        private void frmRader_Load(object sender, EventArgs e)
        {
            this.comRule.Items.Add( cGlobalParas.MonitorRule.IncludeKeyword.GetDescription());
            this.comRule.Items.Add(cGlobalParas.MonitorRule.MoreThan.GetDescription());
            this.comRule.Items.Add(cGlobalParas.MonitorRule.LessThan.GetDescription());
            this.comRule.Items.Add(cGlobalParas.MonitorRule.NumRange.GetDescription());

            cXmlSConfig cs = new cXmlSConfig(Program.getPrjPath ());
            this.txtRadarDb.Text = cs.DataConnection;
            this.m_RadarDbType = (cGlobalParas.DatabaseType)cs.DataType;
            cs = null;

            //初始化资源文件的参数
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

        }

        private void cmdAddTask_Click(object sender, EventArgs e)
        {
            frmAddSominerTask f = new frmAddSominerTask();
            f.ShowDialog();
            f.Dispose();
        }

        private void frmRader_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdAddTask_Click_1(object sender, EventArgs e)
        {
            frmAddSominerTask f = new frmAddSominerTask();
            f.RTask = new frmAddSominerTask.ReturnTask(GetTaskInfo);
            f.ShowDialog();
            f.Dispose();
        }

        private void GetTaskInfo(string TaskClass,string TaskName, string Field)
        {
            ListViewItem cItem = new ListViewItem();

            cItem.Text = TaskClass;
            cItem.SubItems.Add(TaskName);
            cItem.SubItems.Add(Field);

            this.listTask.Items.Add(cItem);

            GetMonitorRange();
        }


        private void GetMonitorRange()
        {
            List<string> r = new List<string>();
            List<string> r1=null ;

            for (int i =0;i<this.listTask.Items.Count ;i++)
            {
                string sc = this.listTask.Items[i].SubItems[2].Text.ToString();
                //sc += ",";
                r1= new List<string>();

                foreach (string ss in sc.Split (','))
                {
                   

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

            ListViewItem citem;
            this.listRange.Items.Clear();
            this.comDataSource.Items.Clear();

            for (int j=0;j<r.Count ;j++)
            {
                citem = new ListViewItem();
                citem.Text = r[j].ToString();
                citem.ImageIndex = 1;
                this.listRange.Items.Add(citem);

                this.comDataSource.Items.Add(r[j].ToString());
            }

            this.IsSave.Text = "true";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void listTask_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && this.listTask.SelectedItems.Count > 0)
            {
                this.listTask.Items.Remove(this.listTask.SelectedItems[0]);

                GetMonitorRange();
            }
        }

        private void cmdDelTask_Click(object sender, EventArgs e)
        {
            if ( this.listTask.SelectedItems.Count > 0)
            {
                this.listTask.Items.Remove(this.listTask.SelectedItems[0]);

                GetMonitorRange();
            }
        }

        private void cmdAddRule_Click(object sender, EventArgs e)
        {
            if (this.comDataSource.SelectedIndex ==-1 || this.comRule.SelectedIndex == -1 || this.txtRuleContent.Text == "")
            {
                this.errorProvider1.Clear();
                this.errorProvider1.SetError(this.txtRuleContent, "请将监控规则输入完整");

                return;
            }
            
            ListViewItem cItem = new ListViewItem();
            cItem.Text = this.comDataSource.SelectedItem.ToString();
            cItem.SubItems.Add(this.comRule.SelectedItem.ToString());
            cItem.SubItems.Add(this.txtRuleContent.Text);
            cItem.SubItems.Add(this.RuleNum.Value.ToString());
            this.listRule.Items.Add(cItem);

            cItem = null;

            this.comRule.SelectedIndex = -1;
            this.txtRuleContent.Text = "";

            this.IsSave.Text = "true";
            
        }

        private void comRule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comRule.SelectedIndex == 0)
            {
                this.RuleNum.Enabled = true;
                this.RuleNum.Value = 1;
            }
            else
            {
                this.RuleNum.Value = 0;
                this.RuleNum.Enabled = false;
                
            }
        }

        private void raCustomRule_CheckedChanged(object sender, EventArgs e)
        {
            this.comRule.Items.Clear();
            this.comRule.Items.Add("包含关键词");
            this.comRule.Items.Add("数值大于");
            this.comRule.Items.Add("数值小于");
            this.comRule.Items.Add("数值范围");

            this.IsSave.Text = "true";
        }

        private void raSystemRule_CheckedChanged(object sender, EventArgs e)
        {
            
            this.comRule.Items.Clear();
            this.comRule.Items.Add("");

            this.IsSave.Text = "true";
        }

        private void IsSave_TextChanged(object sender, EventArgs e)
        {
            if (this.IsSave.Text == "true" && this.m_FormState != cGlobalParas.FormState.Browser)
            {
                this.cmdApply.Enabled = true;
            }
            else if (this.IsSave.Text == "false")
            {
                this.cmdApply.Enabled = false;
            }
        }

        private void raSendEmail_CheckedChanged(object sender, EventArgs e)
        {
            this.groupData.Enabled = false;
            this.groupPage.Enabled = false;
            this.groupEmail.Enabled = true;
            this.IsSave.Text = "true";
        }

        private void raNoDeal_CheckedChanged(object sender, EventArgs e)
        {
            this.groupData.Enabled = false;
            this.groupPage.Enabled = false;
            this.groupEmail.Enabled = false ;
            this.IsSave.Text = "true";
            
        }

        private void raSavePage_CheckedChanged(object sender, EventArgs e)
        {
            this.groupData.Enabled = true ;
            this.groupPage.Enabled = true;
            this.groupEmail.Enabled = false ;
            this.IsSave.Text = "true";

            if (this.txtSavePath.Text == "" && this.raSavePage.Checked ==true )
                this.txtSavePath.Text = Program.getPrjPath() + "webpage";

        }

        private void raSaveUrl_CheckedChanged(object sender, EventArgs e)
        {
            this.groupData.Enabled = true ;
            this.groupPage.Enabled = false;
            this.groupEmail.Enabled = false ;
            this.IsSave.Text = "true";
        }

        private void raNoWarning_CheckedChanged(object sender, EventArgs e)
        {
            this.label12.Enabled = false;
            this.txtWarningEmail.Enabled = false;
            this.IsSave.Text = "true";
        }

        private void raWarningTrayIcon_CheckedChanged(object sender, EventArgs e)
        {
            this.label12.Enabled = false  ;
            this.txtWarningEmail.Enabled = false;
            this.IsSave.Text = "true";
        }

        #region 其他操作 输入检查
        //检查用户输入内容的有效性，只要任务有名称就可以保存，降低使用难度
        //但保存的任务不一定可以执行，需要在执行前做进一步修改
        private bool CheckInputvalidity()
        {
            this.errorProvider1.Clear();

            if (this.RuleName.Text.ToString() == null || this.RuleName.Text.Trim().ToString() == "")
            {
                this.errorProvider1.SetError(this.RuleNum , "请输入监控规则的名称！");
                return false;
            }

            return true;
        }

        #endregion

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (!CheckInputvalidity())
            {
                return;
            }

            if (this.listRange.Items.Count == 0 || this.listRule.Items.Count == 0)
            {
                if (MessageBox.Show("监控范围或监控规则未进行配置，监控规则将无法执行，是否保存此监控规则？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            if (this.IsSave.Text == "true")
            {
                if (!SaveRule())
                {
                    return;
                }
            }

            this.IsSave.Text = "false";

            this.Close();

        }

        private bool SaveRule()
        {
            int i=0;
            eRadar r = new eRadar();

            //如果是编辑状态，则需要删除原有文件
            if (this.FormState == cGlobalParas.FormState.Edit)
            {
                r.TaskName = this.RuleName.Text;
               
                try
                {
                    oRadarIndex or = new oRadarIndex(Program.getPrjPath());
                    or.DeleTaskIndex(this.RuleName.Text);
                    or.Dispose();
                    or = null;

                    //删除文件
                    
                }
                catch (System.Exception ex)
                {
                    throw ex;

                }
               
            }

            try
            {

                r.TaskName = this.RuleName.Text;

                //当前都是自定义规则，没有系统规则，暂不提供系统规则
                r.MonitorType = cGlobalParas.MonitorType.CustomRule;
                r.MonitorState = cGlobalParas.MonitorState.Normal;

                eSource s;
                for (i = 0; i < this.listTask.Items.Count; i++)
                {
                    s = new eSource();
                    s.TaskClass = this.listTask.Items[i].Text;
                    s.TaskName = this.listTask.Items[i].SubItems[1].Text.ToString();
                    r.Source.Add(s);
                }

                //r.MonitorInterval = (int)this.MonitorInterval.Value;

                eRule rule;

                for (i = 0; i < this.listRule.Items.Count;i++ )
                {
                    rule = new eRule();
                    rule.Field = this.listRule.Items[i].Text;
                    if (this.listRule.Items[i].SubItems[1].Text.ToString() == cGlobalParas.MonitorRule.IncludeKeyword.GetDescription())
                        rule.Rule = cGlobalParas.MonitorRule.IncludeKeyword;
                    else if (this.listRule.Items[i].SubItems[1].Text.ToString() == cGlobalParas.MonitorRule.MoreThan.GetDescription())
                        rule.Rule = cGlobalParas.MonitorRule.MoreThan;
                    else if (this.listRule.Items[i].SubItems[1].Text.ToString() == cGlobalParas.MonitorRule.LessThan.GetDescription())
                        rule.Rule = cGlobalParas.MonitorRule.LessThan;
                    else if (this.listRule.Items[i].SubItems[1].Text.ToString() == cGlobalParas.MonitorRule.NumRange.GetDescription())
                        rule.Rule = cGlobalParas.MonitorRule.NumRange;

                    rule.Content = this.listRule.Items[i].SubItems[2].Text.ToString();
                    rule.Num = int.Parse(this.listRule.Items[i].SubItems[3].Text.ToString());
                    r.MRule.Add(rule);
                }

                if (this.raNoDeal.Checked == true)
                {
                    r.DealType = cGlobalParas.MonitorDealType.ByTaskConfig;
                }
                else if (this.raSendEmail.Checked == true)
                {
                    r.DealType = cGlobalParas.MonitorDealType.SendEmail;
                }
                else if (this.raSavePage.Checked == true)
                {
                    r.DealType = cGlobalParas.MonitorDealType.SaveUrlAndPage;
                }
                else if (this.raSaveUrl.Checked == true)
                {
                    r.DealType = cGlobalParas.MonitorDealType.SaveUrl;
                }

                if (this.comTableName.Text == "")
                    r.TableName = "";
                else
                    r.TableName = this.comTableName.Text;

                r.Sql = this.txtSql.Text ;
                r.SavePagePath = this.txtSavePath.Text ;
                r.ReceiveEmail = this.txtDealEmail.Text ;

                if (this.raNoWarning.Checked == true)
                {
                    r.WaringType = cGlobalParas.WarningType.NoWaring;
                }
                else if (this.raWarningTrayIcon.Checked == true)
                {
                    r.WaringType = cGlobalParas.WarningType.ByTrayIcon;
                }
                else if (this.raWarningEmail.Checked == true)
                {
                    r.WaringType = cGlobalParas.WarningType.ByEmail;
                }

                r.WaringEmail = this.txtWarningEmail.Text;

                if (this.raNoInvalid.Checked  == true)
                {
                    r.InvalidType = cGlobalParas.MonitorInvalidType.NoInvalid;
                }
                else if (this.raInvalidByDate.Checked == true)
                {
                    r.InvalidType = cGlobalParas.MonitorInvalidType.InvalidByDate;
                    r.InvalidDate = this.InvalidDate.Value.ToString();
                }
                else if (this.raInvalid.Checked == true)
                {
                    r.InvalidType = cGlobalParas.MonitorInvalidType.InvalidByChecked ;
                }
                else if (this.raTmpInvalid.Checked == true)
                {
                    r.InvalidType = cGlobalParas.MonitorInvalidType.Invalid;

                    //同时修改监控状态为无效
                    r.MonitorState = cGlobalParas.MonitorState.Invalid;
                }

                oRadar or = new oRadar(Program.getPrjPath());
                or.SaveRule(r);
                or.Dispose();
                or = null;
            }
            catch (System.Exception ex)
            {
                r = null;
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false ;
            }

            r = null;

            return true;

        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            if (!CheckInputvalidity())
            {
                return;
            }

            if (this.listRange.Items.Count == 0 || this.listRule.Items.Count == 0)
            {
                if (MessageBox.Show("监控范围或监控规则未进行配置，监控规则将无法执行，是否保存此监控规则？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            if (this.IsSave.Text == "true")
            {
                if (!SaveRule ())
                {
                    return;
                }
            }

            if (this.FormState == cGlobalParas.FormState.New)
            {
                this.FormState = cGlobalParas.FormState.Edit;
                this.RuleName.Enabled = false;
            }

            this.IsSave.Text = "false";
        }

        private void MonitorInterval_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cmdDelRule_Click(object sender, EventArgs e)
        {
            if (this.listRule.SelectedItems.Count > 0)
            {
                this.listRule.Items.Remove(this.listRule.SelectedItems[0]);

            }

            this.IsSave.Text = "true";
        }

        private void listRule_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && this.listRule.SelectedItems.Count > 0)
            {
                this.listRule.Items.Remove(this.listRule.SelectedItems[0]);

            }

            this.IsSave.Text = "true";
        }

        private void raWarningEmail_CheckedChanged(object sender, EventArgs e)
        {
            this.label12.Enabled = true;
            this.txtWarningEmail.Enabled = true;
            this.IsSave.Text = "true";
        }

        private void txtWarningEmail_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtSql_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtSavePath_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtDealEmail_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        public void LoadMonitorRule(string Name)
        {
            eRadar r = new eRadar();
            oRadar or = new oRadar(Program.getPrjPath());
            
            r= or.LoadRule(Name);
            

            this.RuleName.Text = r.TaskName;
            this.RuleName.Tag = r.ID;

            //编辑状态，置名称为无效，不能进行修改
            this.RuleName.Enabled = false;

            ListViewItem cItem;
            
            

            //增加监控采集任务
            for (int i = 0; i < r.Source.Count; i++)
            {
                cItem = new ListViewItem();
                cItem.Text = r.Source[i].TaskClass;
                cItem.SubItems.Add(r.Source[i].TaskName);

                oTaskClass tc = new oTaskClass(Program.getPrjPath());
                string ClassPath = Program.getPrjPath() + tc.GetTaskClassPathByName( r.Source[i].TaskClass);

                oTask t = new oTask(Program.getPrjPath());
                t.LoadTask(ClassPath + "\\" + r.Source[i].TaskName + ".smt");

                string strRule = "";
                for (int m = 0; m < t.TaskEntity.WebpageCutFlag.Count; m++)
                {
                    strRule += t.TaskEntity.WebpageCutFlag[m].Title.ToString() + ",";
                }

                //去掉最后的逗号
                strRule =strRule .Substring (0,strRule.Length -1);

                cItem.SubItems.Add(strRule);

                this.listTask.Items.Add (cItem );

                t = null;
                tc = null;

            }

            
            

            GetMonitorRange();

            //this.MonitorInterval.Value = r.MonitorInterval;

            for (int i = 0; i < r.MRule.Count; i++)
            {
                cItem = new ListViewItem();
                cItem.Text = r.MRule[i].Field;
                cItem.SubItems .Add (r.MRule[i].Rule.GetDescription( ));
                cItem.SubItems.Add (r.MRule[i].Content );
                cItem.SubItems.Add (r.MRule[i].Num.ToString () );

                this.listRule.Items.Add (cItem );
            }

            switch (r.DealType)
            {
                case cGlobalParas.MonitorDealType .ByTaskConfig :
                    this.raNoDeal .Checked =true ;
                    this.raSendEmail.Checked =false ;
                    this.raSavePage .Checked =false ;
                    this.raSaveUrl.Checked =false ;
                    break ;
                case cGlobalParas.MonitorDealType .SendEmail :
                    this.raNoDeal .Checked =false ;
                    this.raSendEmail.Checked =true ;
                    this.raSavePage .Checked =false ;
                    this.raSaveUrl.Checked =false ;
                    break ;
                case cGlobalParas.MonitorDealType .SaveUrlAndPage :
                    this.raNoDeal .Checked =false ;
                    this.raSendEmail.Checked =false ;
                    this.raSavePage .Checked =true  ;
                    this.raSaveUrl.Checked =false ;
                    break ;
                case cGlobalParas.MonitorDealType .SaveUrl :
                    this.raNoDeal .Checked =false ;
                    this.raSendEmail.Checked =false ;
                    this.raSavePage .Checked =false ;
                    this.raSaveUrl.Checked =true  ;
                    break ;
            }

            this.comTableName.Text =r.TableName ;
            this.txtSql.Text =r.Sql ;
            this.txtSavePath.Text =r.SavePagePath ;
            this.txtDealEmail.Text =r.ReceiveEmail ;

            switch (r.WaringType )
            {
                case cGlobalParas.WarningType.NoWaring  :
                    this.raNoWarning .Checked =true ;
                    this.raWarningEmail.Checked =false  ;
                    this.raWarningTrayIcon.Checked =false  ;
                    break ;
                case cGlobalParas.WarningType .ByEmail :
                    this.raNoWarning .Checked =false  ;
                    this.raWarningEmail.Checked =true   ;
                    this.raWarningTrayIcon.Checked =false  ;
                    break ;
                case cGlobalParas.WarningType .ByTrayIcon :
                    this.raNoWarning .Checked =false  ;
                    this.raWarningEmail.Checked =false  ;
                    this.raWarningTrayIcon.Checked =true   ;
                    break ;
            }

            this.txtWarningEmail.Text =r.WaringEmail ;

            if (r.InvalidType == cGlobalParas.MonitorInvalidType.NoInvalid)
                this.raNoInvalid.Checked = true;
            else if (r.InvalidType == cGlobalParas.MonitorInvalidType.InvalidByDate)
            {
                this.raInvalidByDate.Checked = true;
                this.InvalidDate.Value = DateTime.Parse(r.InvalidDate);
            }
            else if (r.InvalidType == cGlobalParas.MonitorInvalidType.InvalidByChecked)
                this.raInvalid.Checked = true;
            else if (r.InvalidType == cGlobalParas.MonitorInvalidType.Invalid)
                this.raTmpInvalid.Checked = true;

            r=null;

            this.IsSave.Text  ="false";

            this.cmdApply.Enabled =false;
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void comTableName_DropDown(object sender, EventArgs e)
        {
            if (this.comTableName.Items.Count == 0)
            {
                if (this.m_RadarDbType ==cGlobalParas.DatabaseType.Access )
                {
                    FillAccessTable();
                }
                else if (this.m_RadarDbType == cGlobalParas.DatabaseType.MSSqlServer )
                {
                    FillMSSqlTable();
                }
                else if (this.m_RadarDbType == cGlobalParas.DatabaseType.MySql )
                {
                    FillMySql();
                }

            }
        }
        #region
        private void FillAccessTable()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon ( this.txtRadarDb.Text);

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
            conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtRadarDb.Text);

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
            conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtRadarDb.Text);

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
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Show(this.button1, 0, 36);
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Match s;

            if (Regex.IsMatch(e.ClickedItem.ToString(), "[{].*[}]"))
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[{].*[}]");
            }
            else
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[<].*[>]");
            }

            int startPos = this.txtSql.SelectionStart;
            int l = this.txtSql.SelectionLength;

            this.txtSql.Text = this.txtSql.Text.Substring(0, startPos) + s.Groups[0].Value + this.txtSql.Text.Substring(startPos + l, this.txtSql.Text.Length - startPos - l);

            this.txtSql.SelectionStart = startPos + s.Groups[0].Value.Length;
            this.txtSql.ScrollToCaret();
        }

        private void raInvalidByDate_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raInvalidByDate.Checked == true)
                this.InvalidDate.Enabled = true;

            this.IsSave.Text = "true";
        }

        private void raNoInvalid_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raNoInvalid.Checked == true)
                this.InvalidDate.Enabled = false;
            this.IsSave.Text = "true";
        }

        private void raInvalid_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raInvalid.Checked==true )
                this.InvalidDate.Enabled = false;

            this.IsSave.Text = "true";
        }

        private void comTableName_TextChanged(object sender, EventArgs e)
        {
            string iSql = "insert into " + this.comTableName.Text + "(";
            iSql += "MonitorName,TaskName,Url,MonitorDate) values (";
            iSql += "'{MonitorName}','{TaskName}','{Url}','{MonitorDate}')";
            this.txtSql.Text = iSql;

        }

        private void InvalidDate_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.Description = rm.GetString("Info250");
            this.folderBrowserDialog1.SelectedPath = Program.getPrjPath();
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtSavePath.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

      

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Resources;
using System.Reflection;
using NetMiner.Resource;
using NetMiner.Publish;
using NetMiner.Publish.Rule;

namespace SoukeyPublishManage
{
    public partial class frmDBRule : Form
    {
        private ResourceManager rm;

        private cGlobalParas.FormState m_fState;
        private bool IsSaveTemp = false;
        private string m_TName;

        public delegate void ReturnTemplate(string tName, cGlobalParas.PublishTemplateType tType, string remark);
        public ReturnTemplate RTemplate;

        public frmDBRule()
        {
            InitializeComponent();

            //初始化资源文件的参数
            rm = new ResourceManager("NetMiner.Resource.Resources.globalPara", Assembly.Load("NetMiner.Resource"));

        }

        public void IniData(cGlobalParas.FormState fState, string tName)
        {
            this.m_fState = fState;

            if (fState == cGlobalParas.FormState.New)
            {
                return;
            }
            else
            {
                LoadTemplate(tName);
            }
        }

        private void cmdInsertPara_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Show(this.cmdInsertPara, 0,56);
        }

        private void frmDBRule_Load(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Items.Add("{系统参数:上条语句返回值}");
            this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
            this.contextMenuStrip1.Items.Add("{发布参数:标题}");
            this.contextMenuStrip1.Items.Add("{发布参数:正文}");
            this.contextMenuStrip1.Items.Add("{发布参数:发布时间}");
            this.contextMenuStrip1.Items.Add("{发布参数:来源}");
            this.contextMenuStrip1.Items.Add("{发布参数:作者}");
            this.contextMenuStrip1.Items.Add("{发布参数:点击量}");
            this.contextMenuStrip1.Items.Add("{发布参数:浏览量}");
            this.contextMenuStrip1.Items.Add("{发布参数:品名}");
            this.contextMenuStrip1.Items.Add("{发布参数:单位}");
            this.contextMenuStrip1.Items.Add("{发布参数:价格}");
            this.contextMenuStrip1.Items.Add("{发布参数:单价}");
            this.contextMenuStrip1.Items.Add("{发布参数:数量}");
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

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            if (IsSaveTemp == true)
            {
                RTemplate(this.txtName.Text, cGlobalParas.PublishTemplateType.DB, this.txtRemark.Text);
            }

            this.Close();
        }

      
        private void frmDBRule_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void cmdAddSql_Click(object sender, EventArgs e)
        {
            ListViewItem cItem = new ListViewItem();

            cItem.Text = (this.listSql.Items.Count + 1).ToString();
            cItem.SubItems.Add(this.IsRepeat.Checked.ToString());
            cItem.SubItems.Add(this.txtPK.Text.Trim());
            if (this.raSql.Checked == true)
                cItem.SubItems.Add(rm.GetString("PublishSqlType1"));
            else if (this.raUpdate.Checked == true)
                cItem.SubItems.Add(rm.GetString("PublishSqlType2"));
            else if (this.raGetValues.Checked == true)
                cItem.SubItems.Add(rm.GetString("PublishSqlType3"));
            else if (this.raGetLastID.Checked == true)
                cItem.SubItems.Add(rm.GetString("PublishSqlType4"));
            cItem.SubItems.Add(this.txtSql.Text);

            this.listSql.Items.Add(cItem);

            this.txtSql.Text = "";
            this.raSql.Checked = true;

            this.IsSave.Text = "True";
        }

        private void listSql_Click(object sender, EventArgs e)
        {
            if (this.listSql.SelectedItems.Count == 0)
            {
                this.cmdDelSql.Enabled = false;
                this.cmdEditSql.Enabled = false;
            }
            else
            {
                this.cmdEditSql.Enabled = true;
                this.cmdDelSql.Enabled = true;

                if (this.listSql.SelectedItems[0].SubItems[1].Text == "True")
                    this.IsRepeat.Checked = true;
                else
                    this.IsRepeat.Checked = false;

                this.txtPK.Text = this.listSql.SelectedItems[0].SubItems[2].Text;
                switch (EnumUtil.GetEnumName<cGlobalParas.PublishSqlType>( this.listSql.SelectedItems[0].SubItems[3].Text))
                {
                    case cGlobalParas.PublishSqlType.Common :
                        this.raSql.Checked = true;
                        break;
                    case cGlobalParas.PublishSqlType.UpdateByPK:
                        this.raUpdate.Checked = true;
                        break;
                    case cGlobalParas.PublishSqlType.GetValues:
                        this.raGetValues.Checked = true;
                        break;
                    case cGlobalParas.PublishSqlType.GetLastID:
                        this.raGetLastID.Checked = true;
                        break;

                }
                this.txtSql.Text = this.listSql.SelectedItems[0].SubItems[4].Text;
            }

            this.IsSave.Text = "false";
        }

        private void cmdDelSql_Click(object sender, EventArgs e)
        {
            if (this.listSql.SelectedItems.Count == 0)
                return;

            this.listSql.Items.Remove(this.listSql.SelectedItems[0]);

            for (int i = 0; i < this.listSql.Items.Count; i++)
            {
                this.listSql.Items[i].Text = (i + 1).ToString ();
            }
            this.IsSave.Text = "True";
        }

        private void listSql_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (this.listSql.SelectedItems.Count == 0)
                    return;

                this.listSql.Items.Remove(this.listSql.SelectedItems[0]);

                for (int i = 0; i < this.listSql.Items.Count; i++)
                {
                    this.listSql.Items[i].Text = (i + 1).ToString();
                }

                this.IsSave.Text = "True";
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void txtSql_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void raAccess_Click(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void raSqlserver_Click(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void raMySql_Click(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void raSql_Click(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void raUpdate_Click(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void raGetValues_Click(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void raGetLastID_Click(object sender, EventArgs e)
        {
            this.IsSave.Text = "True";
        }

        private void IsSave_TextChanged(object sender, EventArgs e)
        {
            if (this.IsSave.Text == "True" && this.m_fState != cGlobalParas.FormState.Browser)
            {
                this.cmdApply.Enabled = true;
            }
            else if (this.IsSave.Text == "false")
            {
                this.cmdApply.Enabled = false;
            }
        }

        private void frmDBRule_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.IsSave.Text == "True")
            {
                if (MessageBox.Show("已经对规则进行了修改，是否不保存直接退出？", "网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    e.Cancel = true;
                return;
            }
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            if (!CheckInputvalidity())
            {
                return;
            }

            try
            {

                if (!SaveTemplate())
                {
                    return;
                }

                this.IsSave.Text = "false";

                IsSaveTemp = true;

                if (this.m_fState == cGlobalParas.FormState.New)
                {
                    this.m_fState = cGlobalParas.FormState.Edit;
                    this.m_TName = this.txtName.Text.Trim();
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("保存发生错误，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private bool CheckInputvalidity()
        {
            if (this.txtName.Text.Trim() == "")
            {
                MessageBox.Show("模版名称不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        private bool SaveTemplate()
        {
            cDbTemplate temp = new cDbTemplate(Program.getPrjPath());

            if (this.m_fState == cGlobalParas.FormState.New)
            {
                this.m_TName = this.txtName.Text.Trim();
                this.m_fState = cGlobalParas.FormState.Edit;
            }
            else if (this.m_fState == cGlobalParas.FormState.Edit)
            {
                //删除原有任务的主要目的是为了备份，但如果发生错误，则忽略
                temp.DeleTemplate(this.m_TName);

            }

            temp.TempName = this.txtName.Text.Trim();
            temp.TempType = cGlobalParas.PublishTemplateType.DB;
            if (this.raAccess.Checked == true)
                temp.DbType = cGlobalParas.DatabaseType.Access;
            else if (this.raSqlserver.Checked == true)
                temp.DbType = cGlobalParas.DatabaseType.MSSqlServer;
            else if (this.raMySql.Checked == true)
                temp.DbType = cGlobalParas.DatabaseType.MySql;
            temp.Remark = this.txtRemark.Text;
           

            for (int i = 0; i < this.listSql.Items.Count; i++)
            {
                cSqlPara cp = new cSqlPara();
                cp.Index =int.Parse ( this.listSql.Items[i].Text);
                if (this.listSql.Items[i].SubItems[1].Text == "True")
                    cp.IsRepeat = true;
                else
                    cp.IsRepeat = false;

                cp.PK = this.listSql.Items[i].SubItems[2].Text;
                cp.SqlType =EnumUtil.GetEnumName<cGlobalParas.PublishSqlType> ( this.listSql.Items[i].SubItems[3].Text);
                cp.Sql = this.listSql.Items[i].SubItems[4].Text;

                temp.sqlParas.Add(cp);
            }

            temp.Save(this.txtName.Text.Trim());
            temp = null;

            return true;
        }

        private void LoadTemplate(string Name)
        {

            cDbTemplate temp = new cDbTemplate(Program.getPrjPath());
            temp.LoadTemplate(Name);

            //加载界面数据
            this.txtName.Text = temp.TempName;
            this.m_TName = temp.TempName;
            this.txtName.Tag = temp.TempType;
            this.txtRemark.Text = temp.Remark;

            switch (temp.DbType)
            {
                case  cGlobalParas.DatabaseType.Access:
                    this.raAccess.Checked = true;
                    break;
                case  cGlobalParas.DatabaseType.MSSqlServer:
                    this.raSqlserver.Checked = true;
                    break;
                case cGlobalParas.DatabaseType.MySql:
                    this.raMySql.Checked = true;
                    break;
            }
         
            for (int i = 0; i < temp.sqlParas.Count; i++)
            {
                ListViewItem cItem = new ListViewItem();
                cItem.Text = temp.sqlParas[i].Index.ToString () ;
                if (temp.sqlParas[i].IsRepeat == true)
                    cItem.SubItems.Add("True");
                else
                    cItem.SubItems.Add("False");
                cItem.SubItems.Add(temp.sqlParas[i].PK);
                cItem.SubItems.Add(temp.sqlParas[i].SqlType.GetDescription());

                cItem.SubItems.Add(temp.sqlParas[i].Sql);
                this.listSql.Items.Add(cItem);
            }

            temp = null;

            this.cmdApply.Enabled = false;
            this.IsSave.Text = "false";
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (!CheckInputvalidity())
            {
                return;
            }

            try
            {

                if (!SaveTemplate())
                {
                    return;
                }

                this.IsSave.Text = "false";

                IsSaveTemp = true;

                if (this.m_fState == cGlobalParas.FormState.New)
                {
                    this.m_fState = cGlobalParas.FormState.Edit;
                    this.m_TName = this.txtName.Text.Trim();
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("保存发生错误，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RTemplate(this.txtName.Text, cGlobalParas.PublishTemplateType.DB, this.txtRemark.Text);

            this.Close();
        }

        private void cmdEditSql_Click(object sender, EventArgs e)
        {

            //this.listSql.SelectedItems[0].Text = (this.listSql.Items.Count + 1).ToString();
            this.listSql.SelectedItems[0].SubItems[1].Text = this.IsRepeat.Checked.ToString();
            this.listSql.SelectedItems[0].SubItems[2].Text = this.txtPK.Text.Trim();
            if (this.raSql.Checked == true)
                this.listSql.SelectedItems[0].SubItems[3].Text = rm.GetString("PublishSqlType1");
            else if (this.raUpdate.Checked == true)
                this.listSql.SelectedItems[0].SubItems[3].Text = rm.GetString("PublishSqlType2");
            else if (this.raGetValues.Checked == true)
                this.listSql.SelectedItems[0].SubItems[3].Text = rm.GetString("PublishSqlType3");
            else if (this.raGetLastID.Checked == true)
                this.listSql.SelectedItems[0].SubItems[3].Text = rm.GetString("PublishSqlType4");

            this.listSql.SelectedItems[0].SubItems[4].Text =this.txtSql.Text;

    
            this.txtSql.Text = "";
            this.raSql.Checked = true;

            this.IsSave.Text = "True";
        }

        private void IsRepeat_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsRepeat.Checked == true)
                this.txtPK.Enabled = true;
            else
                this.txtPK.Enabled = false;
        }
    }
}

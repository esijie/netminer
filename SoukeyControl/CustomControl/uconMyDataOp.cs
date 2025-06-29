using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO;
using NetMiner.Core.Plugin;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Publish.Rule;
using System.Data.OracleClient;
using NetMiner.Core.Event;
using NetMiner.Core.pTask;
using NetMiner.Core.pTask.Entity;
using NetMiner.Publish;
using NetMiner.Base;

namespace SoukeyControl.CustomControl
{
    public partial class uconMyDataOp : UserControl
    {
        //定义一个访问资源文件的变量
        private ResourceManager rm;

        private BindingSource m_dataSource;
        private string m_CurDataSource = "";
        private cGlobalParas.DatabaseType m_CurDataType;
        private string m_CurSql;

        //定义一个发布控制器
        private cPublishControl m_PControl;

        private bool m_IsRunning = false;

        //全局的数据表，加载数据时，将数据加载到此表，然后进行绑定
        DataTable m_tmp;

        //定义一个表格，用于在发布数据时临时存储数据，并在发布后同步到当前的grid
        DataTable m_pData;

        //定义一个标志，表示当前是否已经加载了数据信息
        private bool m_IsLoadedData = false;

        private string m_FilterValue = "";

        private string m_PublishSource;

        private cGlobalParas.FormState m_fState;
        private string m_pName;

        private string m_workPath = string.Empty;
        private cGlobalParas.VersionType m_curVersion = cGlobalParas.VersionType.Free;

        public uconMyDataOp(string workPath,cGlobalParas.VersionType vType)
        {
            m_workPath = workPath;
            m_curVersion = vType;

            InitializeComponent();

            m_fState = cGlobalParas.FormState.New;
        }



        /// <summary>
        /// 初始化控件初识数据
        /// </summary>
        public void IniControl()
        {
            

            //初始化资源文件的参数
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            
            this.splitContainer3.Panel2Collapsed = true;

            IniTree();
            


            if (m_CurDataSource != "")
            {
                DataTable tmp = new DataTable();
                tmp.ReadXml(m_CurDataSource);

                m_dataSource = new BindingSource(tmp, null);
                this.dataData.DataSource = m_dataSource;

                this.dataNavi.BindingSource = m_dataSource;
            }

            //根据当前的区域进行显示信息的加载
            ResourceManager rmPara = new ResourceManager("NetMiner.Resource.Resources.globalPara", Assembly.Load("NetMiner.Resource"));
            this.comExportUrlCode.Items.Add(rmPara.GetString("WebCode1"));
            this.comExportUrlCode.Items.Add(rmPara.GetString("WebCode3"));
            this.comExportUrlCode.Items.Add(rmPara.GetString("WebCode4"));
            this.comExportUrlCode.Items.Add(rmPara.GetString("WebCode5"));
            this.comExportUrlCode.Items.Add(rmPara.GetString("WebCode6"));
            this.comExportUrlCode.SelectedIndex = 0;
            rmPara = null;

            this.e_EditByPluginEvent += this.editByRuleEvent;

            //初始化发布模版的信息
            cIndex tIndex = new cIndex(m_workPath);
            tIndex.GetData();
            int count = tIndex.GetCount();

            for (int i = 0; i < count; i++)
            {
                this.comTemplate.Items.Add(tIndex.GetTName(i) + "[" + tIndex.GetTType(i).GetDescription () + "]");
            }

            this.comTemplate.SelectedIndex = -1;

            if (m_curVersion == cGlobalParas.VersionType.Cloud ||
                m_curVersion == cGlobalParas.VersionType.Ultimate ||
                m_curVersion == cGlobalParas.VersionType.Enterprise)
                this.raExportOracle.Enabled = true;
            else
                this.raExportOracle.Enabled = false;
        }

        private void IniTree()
        {
            TreeNode newNode;

            this.treePublish.Nodes[0].Nodes.Clear();

            //加载任务发布模板信息
            oPublishTask cP = new oPublishTask(m_workPath );
            IEnumerable< ePublishTask> eps= cP.LoadPublishData();

            foreach(ePublishTask ep in eps)
            {
                newNode = new TreeNode();
                newNode.ImageIndex = 7;
                newNode.SelectedImageIndex = 7;
                newNode.Name = "P" + ep.pName;
                newNode.Text = ep.pName;
                this.treePublish.Nodes["nodPublish"].Nodes.Add(newNode);
                newNode = null;
            }
            cP = null;

            this.treePublish.ExpandAll();
            this.treePublish.SelectedNode = this.treePublish.Nodes[0];
        }

        /// <summary>
        /// 根据外部传入的信息，初始化空间数据信息
        /// </summary>
        /// <param name="m_dType">数据类型</param>
        /// <param name="m_dSource">具体数据调用信息</param>
        /// <param name="strSql">如果是数据库，则需要传递sql语句进行数据的加载，如果是SoukeyData，则为空</param>
        public void LoadData(cGlobalParas.DatabaseType m_dType,string m_dSource,string strSql)
        {
            try
            {
                OpenData(m_dType, m_dSource, strSql);
           
                m_CurDataType = m_dType;
                m_CurDataSource = m_dSource;
                m_CurSql = strSql;

                //表示已经加载了数据
                m_IsLoadedData = true;
                this.toolSaveData.Enabled = false;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("加载数据发生错误，错误信息:" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

           
        }

        #region 加载数据的代码

        //定义一个代理用于进行数据的导入
        private delegate bool delegateImportData(cGlobalParas.DatabaseType dType, string strDataSource, string sql);
        private void OpenData(cGlobalParas.DatabaseType dType, string strDataSource, string Sql)
        {
            //定义一个修改分类名称的委托
            delegateImportData sd = new delegateImportData(this.ImportData);

            ////显示等待的窗口 
            //frmWaiting fWait = new frmWaiting("正在加载数据请等待...");
            //new Thread((ThreadStart)delegate
            //{
            //    Application.Run(fWait);
            //}).Start();

            bool retValue = false;
            try
            {
                //开始调用函数,可以带参数 
                IAsyncResult ir = sd.BeginInvoke(dType, strDataSource, Sql, null, null);
                

                //循环检测是否完成了异步的操作 
                while (true)
                {
                    if (ir.IsCompleted)
                    {
                        //完成了操作则关闭窗口 
                        break;
                    }
                }

                //取函数的返回值 
                retValue = sd.EndInvoke(ir);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                //if (fWait.IsHandleCreated)
                //    fWait.Invoke((EventHandler)delegate { fWait.Close(); });
                //fWait = null;
            }

            if (retValue == false)
            {
                MessageBox.Show("加载数据发生错误，请检查数据连接是否正常，是否存在数据？", "网络矿工 错误", MessageBoxButtons.OK , MessageBoxIcon.Error);
                return;
            }

            this.dataData.DataSource = null;
            this.dataData.DataSource = m_dataSource;

            this.dataNavi.BindingSource = m_dataSource;

            this.staProgressbar.Value = 0;
            this.stateProgress.Text = "0/0";

            //this.staInfo.Text = "数据：" + this.treeMenu.SelectedNode.Text;
        }

        private bool ImportData(cGlobalParas.DatabaseType dType, string strDataSource, string sql)
        {
            m_dataSource = null;

            m_tmp = new DataTable();

            DataColumn dc = new DataColumn("isPublished", System.Type.GetType("System.String"));
            dc.DefaultValue = "UnPublished";

            try
            {
                switch (dType)
                {
                    case cGlobalParas.DatabaseType.SoukeyData:
                        if (System.IO.File.Exists(strDataSource))
                        {
                            m_tmp.ReadXml(strDataSource);
                            if (m_tmp.Columns[m_tmp.Columns.Count -1].ColumnName!="isPublished")
                                m_tmp.Columns.Add(dc);
                            m_dataSource = new BindingSource(m_tmp, null);
                        }
                        break;
                    case cGlobalParas.DatabaseType.Access:
                        m_tmp = GetAccessData(strDataSource, sql);
                        if (m_tmp.Columns[m_tmp.Columns.Count - 1].ColumnName != "isPublished")
                            m_tmp.Columns.Add(dc);
                        m_dataSource = new BindingSource(m_tmp, null);
                        break;
                    case cGlobalParas.DatabaseType.MSSqlServer:
                        m_tmp = GetMSSqlData(strDataSource, sql);
                        if (m_tmp.Columns[m_tmp.Columns.Count - 1].ColumnName != "isPublished")
                            m_tmp.Columns.Add(dc);
                        m_dataSource = new BindingSource(m_tmp, null);
                        break;
                    case cGlobalParas.DatabaseType.MySql:
                        m_tmp = GetMySqlData(strDataSource, sql);
                        if (m_tmp.Columns[m_tmp.Columns.Count - 1].ColumnName != "isPublished")
                            m_tmp.Columns.Add(dc);
                        m_dataSource = new BindingSource(m_tmp, null);
                        break;
                    default:
                        break;
                }
            }
            catch { return false; }
            return true;
        }

        private DataTable GetAccessData(string strCon, string sql)
        {
            OleDbConnection conn = new OleDbConnection();

            string connectionstring = strCon;

            conn.ConnectionString = ToolUtil.DecodingDBCon (connectionstring);

          
                conn.Open();
           

            try
            {

                System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter(sql, conn);
                System.Data.OleDb.OleDbCommandBuilder builder = new System.Data.OleDb.OleDbCommandBuilder(da);

                DataSet ds = new DataSet();
                da.Fill(ds);

                conn.Close();
                return ds.Tables[0];
            }
            catch (System.Exception )
            {
                conn.Close();
                return null;
            }

        }

        private DataTable GetMSSqlData(string strCon, string sql)
        {
            //bool IsTable = false;

            SqlConnection conn = new SqlConnection();

            string connectionstring = strCon;

            conn.ConnectionString = connectionstring;

            
                conn.Open();
           

            try
            {

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                SqlCommandBuilder builder = new SqlCommandBuilder(da);

                DataSet ds = new DataSet();
                da.Fill(ds);

                conn.Close();
                return ds.Tables[0];
            }
            catch (System.Exception )
            {
                conn.Close();
                return null;
            }
        }

        private DataTable GetMySqlData(string strCon, string sql)
        {
            //bool IsTable = false;

            MySqlConnection conn = new MySqlConnection();

            string connectionstring = strCon;

            conn.ConnectionString =connectionstring;

            
                conn.Open();
           

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                MySqlCommandBuilder builder = new MySqlCommandBuilder(da);

                DataSet ds = new DataSet();
                da.Fill(ds);

                conn.Close();
                return ds.Tables[0];
            }
            catch (System.Exception )
            {
                conn.Close();
                return null;
            }
        }

        #endregion

        private void toolIsPublish_Click(object sender, EventArgs e)
        {
            if (this.toolIsPublish.Checked == true)
            {
                this.splitContainer3.Panel2Collapsed = true;
                this.toolIsPublish.Checked = false;
                this.toolStartPublish.Enabled = false;
            }
            else
            {
                this.splitContainer3.Panel2Collapsed = false;
                this.toolIsPublish.Checked = true;
                if (this.dataData.Rows.Count > 0)
                    this.toolStartPublish.Enabled = true;
            }
        }

        private void toolStartPublish_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(rm.GetString("Quaere27"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            if (this.butSaveTemplate.Enabled ==true)
            {
                MessageBox.Show("请先保存发布规则，然后再进行数据发布！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            StartPublish();
        }

        private void StartPublish()
        {
            if (this.dataData.Rows.Count == 0)
            {
                MessageBox.Show(rm.GetString("Info2249"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ePublishTask p = new ePublishTask();
            p.pName = "Publishing";
            p.ThreadCount = int.Parse(this.udThread.Value.ToString());
            p.IsDelRepeatRow = this.IsDelRepRow.Checked;
            if (this.IsPublishByTemplate.Checked == true)
            {
                p.PublishType = cGlobalParas.PublishType.publishTemplate;
                p.TemplateName = this.comTemplate.Text;
                p.User = this.txtUser.Text;
                p.Password = this.txtPwd.Text;
                p.Domain = this.txtDomain.Text;
                p.TemplateDBConn = NetMiner.Common.ToolUtil.DecodingDBCon(this.txtTDbConn.Text);

                for (int i = 0; i < this.dataParas.Rows.Count; i++)
                {
                    ePublishData para = new ePublishData();
                    para.DataLabel = this.dataParas.Rows[i].Cells[0].Value.ToString();
                    if (this.dataParas.Rows[i].Cells[1].Value == null)
                    {
                        para.DataType = cGlobalParas.PublishParaType.NoData;
                        para.DataValue = "";
                    }
                    else
                    {
                        if ("{手工输入}" == this.dataParas.Rows[i].Cells[1].Value.ToString())
                        {
                            para.DataType = cGlobalParas.PublishParaType.Custom;
                            if (this.dataParas.Rows[i].Cells[2].Value == null)
                                para.DataValue = "";
                            else
                            para.DataValue = this.dataParas.Rows[i].Cells[2].Value.ToString();
                        }
                        else
                        {
                            para.DataValue = this.dataParas.Rows[i].Cells[1].Value.ToString();
                            para.DataType = cGlobalParas.PublishParaType.Gather;
                        }
                    }
                    p.PublishParas.Add(para);
                }
            }
            else
            {
                if (this.raPublishData.Checked == true)
                {
                    p.PublishType = cGlobalParas.PublishType.PublishData;
                    if (this.raExportAccess.Checked == true)
                        p.DataType = cGlobalParas.DatabaseType.Access;
                    else if (this.raExportMSSQL.Checked == true)
                        p.DataType = cGlobalParas.DatabaseType.MSSqlServer;
                    else if (this.raExportMySql.Checked == true)
                        p.DataType = cGlobalParas.DatabaseType.MySql;
                    else if (this.raExportOracle.Checked == true)
                        p.DataType = cGlobalParas.DatabaseType.Oracle;

                    p.DataSource = NetMiner.Common.ToolUtil.DecodingDBCon(this.txtDataSource.Text);
                    p.DataTable = this.comTableName.Text;
                    p.InsertSql = this.txtInsertSql.Text;
                    p.IsSqlTrue = this.IsSqlTrue.Checked;
                }
            }

            m_PControl = new cPublishControl(m_workPath);

            //注册发布事件
            m_PControl.PublishManage.PublishCompleted += this.Publish_Complete;
            m_PControl.PublishManage.PublishError += this.Publish_Error;
            m_PControl.PublishManage.PublishFailed += this.Publish_Failed;
            m_PControl.PublishManage.PublishStarted += this.Publish_Started;
            m_PControl.PublishManage.PublishLog += this.Publish_Log;
            m_PControl.PublishManage.RuntimeInfo += this.Publish_RunTime;
            m_PControl.PublishManage.DoCount += this.Publish_DoCount;
            m_PControl.PublishManage.PublishErrorData += this.Publish_ErrorData;
            m_PControl.PublishManage.PublishSource += this.Publish_Source;
            m_PControl.PublishManage.PublishStop += this.Publish_Stop;
            m_PControl.PublishManage.UpdateState += this.Publish_UpdateState;

            //禁用dataData_DataBindingComplete事件
            this.dataData.DataBindingComplete -= this.dataData_DataBindingComplete;

            //定义一个cpublish
            cPublish pt = new cPublish(m_workPath, m_PControl.PublishManage, p, this.IsErrLog.Checked, m_tmp);

            m_PControl.startPublish(pt);
            

            //将datagrid置为无效，禁止响应各种用户操作
            this.dataData.Enabled = false;

        }

        private void toolStopPublish_Click(object sender, EventArgs e)
        {
            if (m_PControl !=null)
                m_PControl.StopPublish ();

            ////注销发布事件
            //m_PControl.PublishManage.PublishCompleted -= this.Publish_Complete;
            //m_PControl.PublishManage.PublishError -= this.Publish_Error;
            //m_PControl.PublishManage.PublishFailed -= this.Publish_Failed;
            //m_PControl.PublishManage.PublishStarted -= this.Publish_Started;
            //m_PControl.PublishManage.PublishLog -= this.Publish_Log;
            //m_PControl.PublishManage.RuntimeInfo -= this.Publish_RunTime;
            //m_PControl.PublishManage.DoCount -= this.Publish_DoCount;
            //m_PControl.PublishManage.PublishErrorData -= this.Publish_ErrorData;
            //m_PControl.PublishManage.PublishedRow -= this.Published_Row;

            ////启用dataData_DataBindingComplete事件
            //this.dataData.DataBindingComplete += this.dataData_DataBindingComplete;

            m_PControl = null;
        }

        private void toolSaveData_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void SaveData()
        {
            if (this.m_CurDataType == cGlobalParas.DatabaseType.SoukeyData)
            {
                //直接存储
                string fileName = this.m_CurDataSource;
                DataTable tmp = ((System.Data.DataTable)(((System.Windows.Forms.BindingSource)(this.dataData.DataSource)).DataSource)).DefaultView.Table;
                tmp.TableName = "SoukeyData";
                tmp.WriteXml(fileName, XmlWriteMode.WriteSchema);
                tmp = null;
            }
            else
            {
                this.saveFileDialog1.Title = rm.GetString("Info2247");
                saveFileDialog1.InitialDirectory = m_workPath;
                saveFileDialog1.Filter = "xml Files(*.xml)|*.xml|All Files(*.*)|*.*";
                string FileName = "";

                if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    FileName = this.saveFileDialog1.FileName;

                    DataTable tmp = ((System.Data.DataTable)(((System.Windows.Forms.BindingSource)(this.dataData.DataSource)).DataSource)).DefaultView.Table;
                    tmp.WriteXml(FileName,XmlWriteMode.WriteSchema);
                    tmp = null;
                }

                //保存成功后，修改当前的数据类别和数据源
                this.m_CurDataType = cGlobalParas.DatabaseType.SoukeyData;
                this.m_CurDataSource=FileName;
            }

            this.toolSaveData.Enabled = false;
        }

        public void SaveSoukeyData()
        {
            //直接存储
            string fileName = this.m_CurDataSource;
            DataTable tmp = ((System.Data.DataTable)(((System.Windows.Forms.BindingSource)(this.dataData.DataSource)).DataSource)).DefaultView.Table;
            if (tmp == null || tmp.Rows.Count == 0)
                return;

            if (tmp.TableName == "")
            {
                string tname = Path.GetFileNameWithoutExtension(fileName);
                tmp.TableName = tname;
            }

            tmp.WriteXml(fileName, XmlWriteMode.WriteSchema);
            tmp = null;
        }

        private void toolAddField_Click(object sender, EventArgs e)
        {
            AddField();
        }

        private void AddField()
        {

            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Info200");
            ffc.label1.Text = rm.GetString("Info200");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                try
                {
                    ((DataTable)m_dataSource.DataSource).Columns.Add(m_FilterValue);
                    ((DataTable)m_dataSource.DataSource).Columns[((DataTable)m_dataSource.DataSource).Columns.Count - 2].SetOrdinal(((DataTable)m_dataSource.DataSource).Columns.Count - 1);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }

            ffc.Dispose();
        }

        private void GetFilterValue(string Value)
        {
            m_FilterValue = Value;
        }


        private void toolDelField_Click(object sender, EventArgs e)
        {
            DelField();
        }

        private void DelField()
        {
            if (this.dataData.Columns.Count == 0)
                return;

            int CurCol = this.dataData.CurrentCell.ColumnIndex;

            if (MessageBox.Show(rm.GetString("Info202") + this.dataData.Columns[CurCol].Name, rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            try
            {
                ((DataTable)m_dataSource.DataSource).Columns.Remove(this.dataData.Columns[CurCol].Name);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void toolNoFilter_Click(object sender, EventArgs e)
        {
            m_dataSource.RemoveFilter();

            this.staShowAll.Visible = false;
            stateFilter.Text = "无";
        }

        private void toolMore_Click(object sender, EventArgs e)
        {
            FilterMore();
        }

        private void toolLess_Click(object sender, EventArgs e)
        {
            FilterLess();
        }

        private void toolEqual_Click(object sender, EventArgs e)
        {
            FilterEqual();
        }

        private void toolNoEqual_Click(object sender, EventArgs e)
        {
            FilterNoEqual();
        }

        private void toolStart_Click(object sender, EventArgs e)
        {
            FilterStart();
        }

        private void toolStartNo_Click(object sender, EventArgs e)
        {
            FilterStartNo();
        }

        private void toolInclude_Click(object sender, EventArgs e)
        {
            FilterInclude();
        }

        private void toolEnd_Click(object sender, EventArgs e)
        {
            FilterEnd();
        }

        private void toolLenMore_Click(object sender, EventArgs e)
        {
            FilterLengthMore();
        }

        private void toolCustomFilter_Click(object sender, EventArgs e)
        {
            FilterCustom();
        }

        private void toolAutoID_Click(object sender, EventArgs e)
        {
            DataAutoID();
        }

        private void toolAddPre_Click(object sender, EventArgs e)
        {
            DataAddPrefix();
        }

        private void toolAddSuffix_Click(object sender, EventArgs e)
        {
            DataAddSuffix();
        }

        private void toolReplace_Click(object sender, EventArgs e)
        {
            DataReplace();
        }

        private void toolCutLeft_Click(object sender, EventArgs e)
        {
            DataCutLeft();
        }

        private void toolCutRight_Click(object sender, EventArgs e)
        {
            DataCutRight();
        }

        private void toolSetEmpty_Click(object sender, EventArgs e)
        {
            DataSetEmpty();
        }

        private void toolDelHtml_Click(object sender, EventArgs e)
        {
            DataDelHtml();
        }

        private void toolValue_Click(object sender, EventArgs e)
        {
            DataValue();
        }

        private void toolCustomData_Click(object sender, EventArgs e)
        {
            DataFunction();
        }

        #region 修改字段值代码

        private void DataAutoID()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info213");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info213");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                ffc.Dispose();

                if (m_FilterValue == "")
                    return;
            }
            else
            {
                ffc.Dispose();
                return;
            }

            int StartI = 0;

            try
            {
                StartI = int.Parse(m_FilterValue);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int CurCol = this.dataData.CurrentCell.ColumnIndex;
            for (int i = 0; i < this.dataData.Rows.Count; i++)
            {
                this.dataData[CurCol, i].Value = (StartI + i);
                int cRow = this.dataData.SelectedCells[i].RowIndex;

                //this.dataData[this.dataData.ColumnCount - 1, cRow].Value = true;
            }

        }

        private void DataAddPrefix()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info214");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info214");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                ffc.Dispose();

                if (m_FilterValue == "")
                    return;
            }
            else
            {
                ffc.Dispose();
                return;
            }

            for (int i = 0; i < this.dataData.SelectedCells.Count; i++)
            {
                this.dataData.SelectedCells[i].Value = m_FilterValue + this.dataData.SelectedCells[i].Value;
                int cRow = this.dataData.SelectedCells[i].RowIndex;

                //this.dataData[this.dataData.ColumnCount - 1, cRow].Value = true;
            }

        }

        private void DataAddSuffix()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info215");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info215");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                ffc.Dispose();

                if (m_FilterValue == "")
                    return;
            }
            else
            {
                ffc.Dispose();
                return;
            }

            for (int i = 0; i < this.dataData.SelectedCells.Count; i++)
            {
                this.dataData.SelectedCells[i].Value = this.dataData.SelectedCells[i].Value + m_FilterValue;
                int cRow = this.dataData.SelectedCells[i].RowIndex;

                //this.dataData[this.dataData.ColumnCount - 1, cRow].Value = true;
            }

        }

        private void DataReplace()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info216");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info216");
            ffc.txtValue.Text = "\"\",\"\"";
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                ffc.Dispose();

                if (m_FilterValue == "")
                    return;
            }
            else
            {
                ffc.Dispose();
                return;
            }

            for (int i = 0; i < this.dataData.SelectedCells.Count; i++)
            {
                if (this.dataData.SelectedCells[i].Value != null && this.dataData.SelectedCells[i].Value.ToString() != "")
                {
                    string oStr = m_FilterValue.Substring(1, m_FilterValue.IndexOf(",") - 2);
                    string nStr = m_FilterValue.Substring(m_FilterValue.IndexOf(",") + 2, m_FilterValue.Length - m_FilterValue.IndexOf(",") - 3);
                    this.dataData.SelectedCells[i].Value = Regex.Replace(this.dataData.SelectedCells[i].Value.ToString(), oStr, nStr, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                    int cRow = this.dataData.SelectedCells[i].RowIndex;

                    //this.dataData[this.dataData.ColumnCount - 1, cRow].Value = true;
                }
            }

        }

        private void DataCutLeft()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info216");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info216");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                ffc.Dispose();

                if (m_FilterValue == "")
                    return;
            }
            else
            {
                ffc.Dispose();
                return;
            }

            int lefti = 0;

            try
            {
                lefti = int.Parse(m_FilterValue);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            for (int i = 0; i < this.dataData.SelectedCells.Count; i++)
            {
                if (this.dataData.SelectedCells[i].Value != null)
                {
                    int len = this.dataData.SelectedCells[i].Value.ToString().Length;
                    if (len > lefti)
                    {
                        this.dataData.SelectedCells[i].Value = this.dataData.SelectedCells[i].Value.ToString().Substring(lefti, len - lefti);
                    }

                    int cRow = this.dataData.SelectedCells[i].RowIndex;

                    //this.dataData[this.dataData.ColumnCount - 1, cRow].Value = true;
                }
            }

        }

        private void DataCutRight()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info216");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info216");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                ffc.Dispose();

                if (m_FilterValue == "")
                    return;
            }
            else
            {
                ffc.Dispose();
                return;
            }

            int righti = 0;

            try
            {
                righti = int.Parse(m_FilterValue);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            for (int i = 0; i < this.dataData.SelectedCells.Count; i++)
            {
                if (this.dataData.SelectedCells[i].Value != null)
                {
                    int len = this.dataData.SelectedCells[i].Value.ToString().Length;
                    if (len > righti)
                    {
                        this.dataData.SelectedCells[i].Value = this.dataData.SelectedCells[i].Value.ToString().Substring(0, len - righti);
                    }

                    int cRow = this.dataData.SelectedCells[i].RowIndex;

                    //this.dataData[this.dataData.ColumnCount - 1, cRow].Value = true;
                }

            }

        }

        private void DataSetEmpty()
        {
            if (MessageBox.Show(rm.GetString("Quaere23"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            for (int i = 0; i < this.dataData.SelectedCells.Count; i++)
            {

                this.dataData.SelectedCells[i].Value = null;

                int cRow = this.dataData.SelectedCells[i].RowIndex;

                //this.dataData[this.dataData.ColumnCount - 1, cRow].Value = true;
            }
        }

        private void DataValue()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info219");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info219");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                ffc.Dispose();

                if (m_FilterValue == "")
                    return;
            }
            else
            {
                ffc.Dispose();
                return;
            }

            for (int i = 0; i < this.dataData.SelectedCells.Count; i++)
            {

                this.dataData.SelectedCells[i].Value = m_FilterValue;

                int cRow = this.dataData.SelectedCells[i].RowIndex;

                //this.dataData[this.dataData.ColumnCount - 1, cRow].Value = true;
            }
        }

        private void DataDelHtml()
        {
            if (MessageBox.Show(rm.GetString("Quaere29"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            try
            {
                for (int i = 0; i < this.dataData.SelectedCells.Count; i++)
                {
                    if (this.dataData.SelectedCells[i].Value != null)
                    {
                        this.dataData.SelectedCells[i].Value = getTxt(this.dataData.SelectedCells[i].Value.ToString());
                        int cRow = this.dataData.SelectedCells[i].RowIndex;

                        //this.dataData[this.dataData.ColumnCount - 1, cRow].Value = true;
                    }

                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }



        private void DataFunction()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info219");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info219");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                ffc.Dispose();

                if (m_FilterValue == "")
                    return;
            }
            else
            {
                ffc.Dispose();
                return;
            }

            //int lefti = 0;

            for (int i = 0; i < this.dataData.SelectedCells.Count; i++)
            {

                int cRow = this.dataData.SelectedCells[i].RowIndex;

                //this.dataData[this.dataData.ColumnCount - 1, cRow].Value = true;

            }
        }

        private void DataDelRepeatRow()
        {
            if (MessageBox.Show(rm.GetString("Quaere30"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            //去除重复行
            DataTable d1 = m_tmp;
            string[] strComuns = new string[d1.Columns.Count];

            for (int m = 0; m < d1.Columns.Count; m++)
            {
                strComuns[m] = d1.Columns[m].ColumnName;
            }

            DataView dv = new DataView(d1);

            DataTable d = dv.ToTable(true, strComuns);

            m_tmp = d;

            m_dataSource = new BindingSource(m_tmp, null);
            this.dataData.DataSource = m_dataSource;
        }

        #endregion

        #region 筛选代码
        private void FilterMore()
        {

            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info203");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info203");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                this.stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info203") + " " + m_FilterValue;
                m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " > '" + m_FilterValue + "'";

            }

            this.staShowAll.Visible = true;

            ffc.Dispose();
        }

        private void FilterLess()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info204");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info204");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + " " + rm.GetString("Info204") + " " + m_FilterValue;
                m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " < '" + m_FilterValue + "'";

            }

            this.staShowAll.Visible = true;
            ffc.Dispose();
        }

        private void FilterEqual()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info205");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info205");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info205") + " " + m_FilterValue;
                m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " = '" + m_FilterValue + "'";

            }

            this.staShowAll.Visible = true;
            ffc.Dispose();
        }

        private void FilterNoEqual()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info206");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info206");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info206") + " " + m_FilterValue;
                m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " <> '" + m_FilterValue + "'";

            }

            this.staShowAll.Visible = true;
            ffc.Dispose();
        }

        private void FilterStart()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info207");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info207");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info207") + " " + m_FilterValue;
                m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " like '" + m_FilterValue + "*'";

            }

            this.staShowAll.Visible = true;
            ffc.Dispose();
        }

        private void FilterInclude()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info209");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info209");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info209") + " " + m_FilterValue;
                m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " like '*" + m_FilterValue + "*'";

            }

            this.staShowAll.Visible = true;
            ffc.Dispose();
        }

        private void FilterEnd()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info211");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info211");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info211") + " " + m_FilterValue;
                m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " like '*" + m_FilterValue + "'";

            }

            this.staShowAll.Visible = true;
        }

        private void FilterStartNo()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info208");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info208");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                int l = m_FilterValue.Length;

                stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info208") + " " + m_FilterValue;
                m_dataSource.Filter = "Substring(" + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + ",1," + l.ToString() + ")<>'" + m_FilterValue + "'";

            }

            this.staShowAll.Visible = true;

            ffc.Dispose();
        }

        private void FilterLengthMore()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info220");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info220");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                int l = m_FilterValue.Length;

                stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info220") + " " + m_FilterValue;
                m_dataSource.Filter = "len(" + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + ")>'" + m_FilterValue + "'";

            }

            this.staShowAll.Visible = true;

            ffc.Dispose();
        }

        private void FilterCustom()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info221");
            ffc.label1.Text = rm.GetString("Label35") + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info221");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                stateFilter.Text = m_FilterValue;
                m_dataSource.Filter = m_FilterValue;
            }

            this.staShowAll.Visible = true;

            ffc.Dispose();
        }

        #endregion

        #region 发布任务的事件处理
        private void Publish_Complete(object sender, PublishCompletedEventArgs e)
        {
            SetValue(this.toolStartPublish, "Enabled", true);
            SetValue(this.toolStopPublish, "Enabled", false);
            SetValue(this.contextMenuStrip1, "Enabled", true);
            SetValue(this.dataData, "Enabled", true);
            SetValue(this.staProgressbar, "Visible", false);
            SetValue(this.staExportState, "Text", "停止");

            //注销发布事件
            //m_PControl.PublishManage.PublishCompleted -= this.Publish_Complete;
            //m_PControl.PublishManage.PublishError -= this.Publish_Error;
            //m_PControl.PublishManage.PublishFailed -= this.Publish_Failed;
            //m_PControl.PublishManage.PublishStarted -= this.Publish_Started;
            //m_PControl.PublishManage.PublishLog -= this.Publish_Log;
            //m_PControl.PublishManage.RuntimeInfo -= this.Publish_RunTime;
            //m_PControl.PublishManage.DoCount -= this.Publish_DoCount;
            //m_PControl.PublishManage.PublishErrorData -= this.Publish_ErrorData;
            //m_PControl.PublishManage.PublishSource -= this.Publish_Source;

            //调用保存事件

            //InvokeMethod(this, "ReBindData", new object[] { e.d });
            //InvokeMethod(this, "SaveSoukeyData", null);

            //启用dataData_DataBindingComplete事件
            this.dataData.DataBindingComplete += this.dataData_DataBindingComplete;
            this.dataData.Enabled = true;

            InvokeMethod(this, "ExportLog", new object[] { "发布任务已经完成，此次成功发布" + this.stateProgress.Text.Substring(0, this.stateProgress.Text.IndexOf("/")) });

            //InvokeMethod(this, "ReBindData1", new object []{e.d});

            m_IsRunning = false;
        }

        private void Publish_Stop(object sender, PublishStopEventArgs e)
        {
            SetValue(this.toolStartPublish, "Enabled", true);
            SetValue(this.toolStopPublish, "Enabled", false);
            SetValue(this.contextMenuStrip1, "Enabled", true);
            SetValue(this.dataData, "Enabled", true);
            SetValue(this.staProgressbar, "Visible", false);
            SetValue(this.staExportState, "Text", "停止");

            //InvokeMethod(this, "ReBindData", new object[] { e.d });

            //启用dataData_DataBindingComplete事件
            this.dataData.DataBindingComplete += this.dataData_DataBindingComplete;
            this.dataData.Enabled = true;

            InvokeMethod(this, "ExportLog", new object[] { "发布任务已经停止，此次成功发布" + this.stateProgress.Text.Substring(0, this.stateProgress.Text.IndexOf("/")) });

            InvokeMethod(this, "ReBindData1", new object []{e.d});

            m_IsRunning = false;
        }

        private void Publish_Started(object sender, PublishStartedEventArgs e)
        {
            //当发布任务启动后，将终止界面中的所有响应操作，
            //只能停止任务后方可进行
            m_IsRunning = true;
            this.toolStartPublish.Enabled = false;

            this.toolStopPublish.Enabled = true;

            this.toolAddField.Enabled = false;
            this.toolDelField.Enabled = false;
            this.toolFilter.Enabled = false;
            this.toolEdit.Enabled = false;

            //this.dataData.Enabled = false;

            this.contextMenuStrip1.Enabled = false;
            //this.contextMenuStrip2.Enabled = false;

            this.staProgressbar.Visible = true;

            this.staExportState.Text = "正在导出......";

            ExportLog("用户已经启动了数据发布，在数据发布期间，您将无法进行任何操作，请等待发布结束或手工停止发布操作.");

        }


        private void Publish_Failed(object sender, PublishFailedEventArgs e)
        {

        }

        private void Publish_Error(object sender, PublishErrorEventArgs e)
        {
            InvokeMethod(this, "ExportLog", new object[] { e.Error.Message });
        }

        private void Publish_Log(object sender, PublishLogEventArgs e)
        {
            InvokeMethod(this, "ExportLog", new object[] { e.strLog  });
        }

        private void Publish_RunTime(object sender, RunTimeEventArgs e)
        {
            InvokeMethod(this, "ExportLog", new object[] { e.str });
        }

        #endregion

        public void ReBindData(DataTable d)
        {
            if (d == null)
                return;

            string tName = this.m_tmp.TableName;
            d.TableName = tName;
            this.m_tmp = d;
            m_dataSource = new BindingSource(m_tmp, null);
            this.dataData.DataSource = m_dataSource;

            this.dataNavi.BindingSource = m_dataSource;
            

        }

        private void butSaveTemplate_Click(object sender, EventArgs e)
        {
            if (this.IsPublishByTemplate.Checked == false)
            {
                if (this.raPublishData.Checked == true)
                {
                    if (this.txtDataSource.Text == "" || this.txtInsertSql.Text == "")
                    {
                        MessageBox.Show(rm.GetString("Info2245"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }
                }
                else if (this.raPublishWeb.Checked == true)
                {
                    if (this.txtExportUrl.Text == "")
                    {
                        MessageBox.Show(rm.GetString("Info2246"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }
                }
            }
            else
            {
                if (this.comTemplate.SelectedIndex  == -1)
                {
                    MessageBox.Show("请选择发布模版！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.comTableName.Focus();
                    return;
                }
            }

            if (this.m_fState == cGlobalParas.FormState.Edit)
            {
                SavePublishInfo(this.m_pName, true);
            }
            else
            {
                frmAddPTask f = new frmAddPTask();
                f.rName = new frmAddPTask.ReturnName(this.SavePublishInfo);
                f.ShowDialog();
                f.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pName">发布规则名称</param>
        /// <param name="isOverwrite">是否覆盖</param>
        private void SavePublishInfo(string pName,bool isOverwrite)
        {
            if (pName == "")
                return;

            this.m_pName = pName;

            oPublishTask p = new oPublishTask(m_workPath);

            bool isExist = p.IsExist(pName);

            if (isExist)
            {
                if (isOverwrite == false)
                {
                    if (MessageBox.Show("您保存的数据发布规则已经存在，保存则覆盖原有规则，是否继续？", "网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;

                    //删除原有规则
                    p.DelTask(pName);
                }
                else
                {
                    p.DelTask(pName);
                }

            }

            ePublishTask ep = new NetMiner.Core.pTask.Entity.ePublishTask();
            ep.pName = pName;
            ep.ThreadCount = int.Parse(this.udThread.Value.ToString());
            ep.IsDelRepeatRow = this.IsDelRepRow.Checked;

            if (this.IsPublishByTemplate.Checked == true)
            {
                ep.PublishType = cGlobalParas.PublishType.publishTemplate;
                ep.TemplateName = this.comTemplate.Text;
                ep.User = this.txtUser.Text;
                ep.Password = this.txtPwd.Text;
                ep.Domain = this.txtDomain.Text;
                ep.TemplateDBConn = this.txtTDbConn.Text;

                for (int i = 0; i < this.dataParas.Rows.Count; i++)
                {
                    ePublishData para = new ePublishData();
                    para.DataLabel = this.dataParas.Rows[i].Cells[0].Value.ToString();
                    if (this.dataParas.Rows[i].Cells[1].Value == null)
                    {
                        para.DataType = cGlobalParas.PublishParaType.NoData ;
                        para.DataValue = "";
                    }
                    else
                    {
                        if ("{手工输入}" == this.dataParas.Rows[i].Cells[1].Value.ToString())
                        {
                            para.DataType = cGlobalParas.PublishParaType.Custom;
                            if (this.dataParas.Rows[i].Cells[2].Value == null)
                                para.DataValue = "";
                            else
                            para.DataValue = this.dataParas.Rows[i].Cells[2].Value.ToString();
                        }
                        else
                        {
                            para.DataValue = this.dataParas.Rows[i].Cells[1].Value.ToString();
                            para.DataType = cGlobalParas.PublishParaType.Gather;
                        }
                    }
                    ep.PublishParas.Add(para);
                }
            }
            else
            {
                if (this.raPublishData.Checked == true)
                {
                    ep.PublishType = cGlobalParas.PublishType.PublishData;
                    if (this.raExportAccess.Checked == true)
                        ep.DataType = cGlobalParas.DatabaseType.Access;
                    else if (this.raExportMSSQL.Checked == true)
                        ep.DataType = cGlobalParas.DatabaseType.MSSqlServer;
                    else if (this.raExportMySql.Checked == true)
                        ep.DataType = cGlobalParas.DatabaseType.MySql;
                    else if (this.raExportOracle.Checked == true)
                        ep.DataType = cGlobalParas.DatabaseType.Oracle;

                    ep.DataSource = this.txtDataSource.Text;
                    ep.DataTable = this.comTableName.Text;
                    ep.InsertSql = this.txtInsertSql.Text;
                    ep.IsSqlTrue = this.IsSqlTrue.Checked;
                }
            }

            p.InsertPublishInfo(ep);

            p = null;

            if (isExist == false)
            {
                TreeNode newNode = new TreeNode();
                newNode.Name = pName;
                newNode.Text = pName;
                newNode.ImageIndex = 7;
                newNode.SelectedImageIndex = 7;
                this.treePublish.Nodes["nodPublish"].Nodes.Add(newNode);
                newNode = null;
            }

            this.m_fState = cGlobalParas.FormState.Edit;

            this.butSaveTemplate.Enabled = false;

            
        }

        private void raPublishData_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsPublishByTemplate.Checked == false)
            {
                this.groupData.Visible = true;
                this.groupWeb.Visible = false;
                this.butSaveTemplate.Enabled = true;
            }
        }

        private void raPublishWeb_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsPublishByTemplate.Checked == false)
            {
                this.groupData.Visible = false;
                this.groupWeb.Visible = true;
                this.butSaveTemplate.Enabled = true;
            }
        }

        private void IsDelRepRow_CheckedChanged(object sender, EventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void udThread_ValueChanged(object sender, EventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void raExportAccess_CheckedChanged(object sender, EventArgs e)
        {
            this.txtDataSource.Text = "";
            this.comTableName.Items.Clear();
            this.butSaveTemplate.Enabled = true;
        }

        private void raExportMSSQL_CheckedChanged(object sender, EventArgs e)
        {
            this.txtDataSource.Text = "";
            this.comTableName.Items.Clear();
            this.butSaveTemplate.Enabled = true;
        }

        private void raExportMySql_CheckedChanged(object sender, EventArgs e)
        {
            this.txtDataSource.Text = "";
            this.comTableName.Items.Clear();
            this.butSaveTemplate.Enabled = true;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            frmSetData fSD = new frmSetData(m_workPath);

            if (this.raExportAccess.Checked == true)
                fSD.FormState = 0;
            else if (this.raExportMSSQL.Checked == true)
                fSD.FormState = 1;
            else if (this.raExportMySql.Checked == true)
                fSD.FormState = 2;
            else if (this.raExportOracle.Checked == true)
                fSD.FormState = 3;

            fSD.rDataSource = new frmSetData.ReturnDataSource(GetDataSource);
            fSD.ShowDialog();
            fSD.Dispose();
        }

        private void GetDataSource(string strDataConn)
        {
            this.txtDataSource.Text = strDataConn;
        }

        //private void getDataSource(string strCon, string sql, cGlobalParas.DatabaseType dType)
        //{
        //    if (strCon == "" || sql == "")
        //        return;

        //    TreeNode newNode = new TreeNode();
        //    newNode.Tag = sql;
        //    newNode.Text = strCon;
        //    newNode.Name = strCon;

        //    TreeNode newChildNode = new TreeNode();

        //    switch (dType)
        //    {
        //        case cGlobalParas.DatabaseType.Access:
        //            newNode.ImageIndex = 5;
        //            newNode.SelectedImageIndex = 5;

        //            newChildNode.Text = sql;
        //            newChildNode.Name = "OtherData:" + sql;
        //            newChildNode.ImageIndex = 5;
        //            newChildNode.SelectedImageIndex = 5;
        //            newNode.Nodes.Add(newChildNode);

        //            this.treeMenu.Nodes["OtherData"].Nodes["AccessData"].Nodes.Add(newNode);
        //            newNode = null;

        //            break;
        //        case cGlobalParas.DatabaseType.MSSqlServer:
        //            newNode.ImageIndex = 5;
        //            newNode.SelectedImageIndex = 5;

        //            newChildNode.Text = sql;
        //            newChildNode.Name = "OtherData:" + sql;
        //            newChildNode.ImageIndex = 5;
        //            newChildNode.SelectedImageIndex = 5;
        //            newNode.Nodes.Add(newChildNode);

        //            this.treeMenu.Nodes["OtherData"].Nodes["SqlServerData"].Nodes.Add(newNode);
        //            break;
        //        case cGlobalParas.DatabaseType.MySql:
        //            newNode.ImageIndex = 5;
        //            newNode.SelectedImageIndex = 5;

        //            newChildNode.Text = sql;
        //            newChildNode.Name = "OtherData:" + sql;
        //            newChildNode.ImageIndex = 5;
        //            newChildNode.SelectedImageIndex = 5;
        //            newNode.Nodes.Add(newChildNode);

        //            this.treeMenu.Nodes["OtherData"].Nodes["MySqlData"].Nodes.Add(newNode);
        //            break;
        //    }

        //    newNode = null;

        //    OpenData(dType, strCon, sql);
        //}

        private void comTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
                FillInsertSql(this.comTableName.SelectedItem.ToString(), this.raExportOracle.Checked);
        }

        private void FillInsertSql(string TableName,bool isOracle)
        {
            string iSql = "";
            string strColumns = "";

            iSql = "insert into " + TableName + " (";

            DataTable tc = GetTableColumns(TableName);

            for (int i = 0; i < tc.Rows.Count; i++)
            {
                if (isOracle==true )
                    strColumns += tc.Rows[i][2].ToString() + ",";
                else
                    strColumns += tc.Rows[i][3].ToString() + ",";
            }

            strColumns = strColumns.Substring(0, strColumns.Length - 1);

            iSql = iSql + strColumns + ") values ( ";

            string strColumnsValue = "";

            for (int j = 0; j < this.dataData.Columns.Count; j++)
            {
                if (this.dataData.Columns[j].Name!="isPublished")
                    strColumnsValue += "'{" + this.dataData.Columns[j].Name + "}',";

            }

            if (strColumnsValue != "")
                strColumnsValue = strColumnsValue.Substring(0, strColumnsValue.Length - 1);

            iSql = iSql + strColumnsValue + ")";

            this.txtInsertSql.Text = iSql;

        }

        private void comTableName_DropDown(object sender, EventArgs e)
        {
            if (this.comTableName.Items.Count == 0)
            {
                if (this.raExportAccess.Checked == true)
                {
                    FillAccessTable();
                }
                else if (this.raExportMSSQL.Checked == true)
                {
                    FillMSSqlTable();
                }
                else if (this.raExportMySql.Checked == true)
                {
                    FillMySql();
                }
                else if (this.raExportOracle.Checked == true)
                {
                    FillOracle();
                }
            }

        }

        private void FillOracle()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon(this.txtDataSource.Text);

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

                this.comTableName.Items.Add(r[1].ToString());

            }
        }

        private void FillAccessTable()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtDataSource.Text);

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
            conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtDataSource.Text);

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
            conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtDataSource.Text);

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


        private DataTable GetTableColumns(string tName)
        {
            DataTable tc = new DataTable();

            try
            {

                if (this.raExportAccess.Checked == true)
                {
                    OleDbConnection conn = new OleDbConnection();
                    conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtDataSource.Text);

                    conn.Open();

                    string[] Restrictions = new string[4];
                    Restrictions[2] = tName;

                    tc = conn.GetSchema("Columns", Restrictions);

                    return tc;

                }
                else if (this.raExportMSSQL.Checked == true)
                {
                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtDataSource.Text);

                    conn.Open();

                    string[] Restrictions = new string[4];
                    Restrictions[2] = tName;

                    tc = conn.GetSchema("Columns", Restrictions);

                    return tc;
                }
                else if (this.raExportMySql.Checked == true)
                {
                    MySqlConnection conn = new MySqlConnection();
                    conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtDataSource.Text);

                    conn.Open();

                    string[] Restrictions = new string[4];
                    Restrictions[2] = tName;

                    tc = conn.GetSchema("Columns", Restrictions);

                    return tc;
                }
                else if (this.raExportOracle.Checked == true)
                {
                    OracleConnection conn = new OracleConnection();
                    conn.ConnectionString = ToolUtil.DecodingDBCon(this.txtDataSource.Text);

                    conn.Open();

                    string[] Restrictions = new string[3];
                    Restrictions[1] = tName;

                    tc = conn.GetSchema("Columns", Restrictions);
                    conn.Close();

                    return tc;
                }

                return tc;

            }
            catch (System.Exception)
            {
                return null;
            }


        }

        private void comTableName_TextChanged(object sender, EventArgs e)
        {
            string iSql = "insert into " + this.comTableName.Text + "(";
            string strColumns = "";
            string strColumnsValue = "";

            for (int j = 0; j < this.dataData.Columns.Count; j++)
            {
                if (this.dataData.Columns[j].Name != "isPublished")
                {
                    //if (this.listWebGetFlag.Items[j].SubItems[1].Text == "文本" || this.listWebGetFlag.Items[j].SubItems[1].Text == "Text")
                    strColumns += this.dataData.Columns[j].Name + ",";
                    strColumnsValue += "'{" + this.dataData.Columns[j].Name + "}',";
                }

            }

            if (strColumns != "")
            {
                strColumns = strColumns.Substring(0, strColumns.Length - 1);
                strColumnsValue = strColumnsValue.Substring(0, strColumnsValue.Length - 1);
            }


            iSql = iSql + strColumns + ") values (" + strColumnsValue + ")";
            this.txtInsertSql.Text = iSql;
        }

        private void txtDataSource_TextChanged(object sender, EventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void txtInsertSql_TextChanged(object sender, EventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void IsErrLog_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comExportUrlCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void txtExportUrl_TextChanged(object sender, EventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.rmenuGetFormat.Show(this.button9, 0, 21);
        }

        private void txtExportCookie_TextChanged(object sender, EventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            frmBrowser wftm = new frmBrowser();
            wftm.getFlag = 2;
            wftm.rExportCookie = new frmBrowser.ReturnExportCookie(GetExportCookie);
            wftm.ShowDialog();
            wftm.Dispose();
        }

        private void GetExportCookie(string strCookie)
        {
            this.txtExportCookie.Text = strCookie;
        }


        private void txtSucceedFlag_TextChanged(object sender, EventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void rmenuAddFiled_Click(object sender, EventArgs e)
        {
            AddField();
        }

        private void rmenuDelField_Click(object sender, EventArgs e)
        {
            DelField();
        }

        private void rmenuSelectCol_Click(object sender, EventArgs e)
        {
            int curCol = this.dataData.CurrentCell.ColumnIndex;

            for (int i = 0; i < this.dataData.Rows.Count; i++)
            {
                this.dataData[curCol, i].Selected = true;
            }
        }

        private void rmenuMore_Click(object sender, EventArgs e)
        {
            FilterMore();
        }

        private void rmenuLess_Click(object sender, EventArgs e)
        {
            FilterLess();
        }

        private void rmenuTxtEqual_Click(object sender, EventArgs e)
        {
            FilterEqual();
        }

        private void rmenuTxtNoEqual_Click(object sender, EventArgs e)
        {
            FilterNoEqual();
        }

        private void rmenuTxtStart_Click(object sender, EventArgs e)
        {
            FilterStart();
        }

        private void rmenuTxtStartNo_Click(object sender, EventArgs e)
        {
            FilterStartNo();
        }

        private void rmenuTxtInclude_Click(object sender, EventArgs e)
        {
            FilterInclude();
        }

        private void rmenuTxtEnd_Click(object sender, EventArgs e)
        {
            FilterEnd();
        }

        private void rmenuLenMore_Click(object sender, EventArgs e)
        {
            FilterLengthMore();
        }

        private void rmenuCustomFilter_Click(object sender, EventArgs e)
        {
            FilterCustom();
        }

        private void rmenuAutoID_Click(object sender, EventArgs e)
        {
            DataAutoID();
        }

        private void rmenuAddPre_Click(object sender, EventArgs e)
        {
            DataAddPrefix();
        }

        private void rmenuAddSuffix_Click(object sender, EventArgs e)
        {
            DataAddSuffix();
        }

        private void rmenuReplace_Click(object sender, EventArgs e)
        {
            DataReplace();
        }

        private void rmenuCutLeft_Click(object sender, EventArgs e)
        {
            DataCutLeft();
        }

        private void rmenuCutRight_Click(object sender, EventArgs e)
        {
            DataCutRight();
        }

        private void rmenuSetEmpty_Click(object sender, EventArgs e)
        {
            DataSetEmpty();
        }

        private void rmenuDelHtml_Click(object sender, EventArgs e)
        {
            DataDelHtml();
        }

        private void rmenuValue_Click(object sender, EventArgs e)
        {
            DataValue();
        }

        private void rmenuCustomData_Click(object sender, EventArgs e)
        {
            DataFunction();
        }

        #region 代理事件 用于更新界面进度条
        //定义一个代理监控后台线程执行的效率
        public delegate void DoCountEventHandler(DoCountEventArgs e);
        private void Publish_DoCount(object sender, DoCountEventArgs e)
        {
            DoCountEventHandler handler = new DoCountEventHandler(UpdateDoCount);
            this.Invoke(handler, new object[] { e });
        }

        private void UpdateDoCount(DoCountEventArgs e)
        {
            this.staProgressbar.Maximum = e.Count;
            this.staProgressbar.Value = e.DoneCount + e.ErrCount;

            this.stateProgress.Text = e.DoneCount + "/" + e.Count;
            this.staError.Text = e.ErrCount.ToString();
            
            //根据传递的当前发布数字，标记数据
            //InvokeMethod(this, "UpdatePublished", new object[] { e.CurRowIndex }); 

            //this.dataData.Refresh();
            Application.DoEvents();
        }

        private void UpdatePublished(int m_CurRow)
        {
            this.dataData[this.dataData.ColumnCount - 1, m_CurRow].Value = false;
        }

        //定义一个代理监控后台线程执行的效率
        public delegate void PublishErrDataEventHandler(PublishErrDataEventArgs e);
        private void Publish_ErrorData(object sender, PublishErrDataEventArgs e)
        {
            PublishErrDataEventHandler handler = new PublishErrDataEventHandler(UpdateData);
            this.Invoke(handler, new object[] { e });
        }

        private void UpdateData(PublishErrDataEventArgs e)
        {
            if (e.ErrData != null && e.ErrData.Rows.Count != 0)
            {
                m_tmp = e.ErrData;
                m_dataSource = new BindingSource(m_tmp, null);
                this.dataData.DataSource = m_dataSource;
            }
            else
                m_tmp.Rows.Clear();

            this.dataData.Refresh();
            Application.DoEvents();
        }

        private void Publish_Source(object sender, PublishSourceEventArgs e)
        {
            this.m_PublishSource = e.HtmlSource;
        }

        private void Publish_UpdateState(object sender, UpdateStateArgs e)
        {
            InvokeMethod(this, "UpdatePublishDataState", new object[] { e.Row, e.isPublishSucceed.ToString() });
        }

        public void UpdatePublishDataState(object[] row,string pResult)
        {
            //object[] row = e.Row;
            int colCount = this.dataData.Columns.Count;
            DataGridViewRow r = FindRow(row);
            if (r != null)
                r.Cells[colCount - 1].Value = pResult;
        }

        public DataGridViewRow FindRow(object[] row)
        {
            for (int i = 0; i < this.dataData.Rows.Count; i++)
            {
                bool isExist = true;
                if (this.dataData.Rows[i].Cells[this.dataData.Columns.Count - 1].Value.ToString() == cGlobalParas.PublishResult.UnPublished.ToString())
                {
                    for (int j = 0; j < this.dataData.Columns.Count - 1; j++)
                    {
                        if (this.dataData.Rows[i].Cells[j].Value.ToString() != row[j].ToString())
                        {
                            isExist = false;
                            break;
                        }
                    }
                    if (isExist == true && this.dataData.Rows[i].Cells[this.dataData.Columns.Count - 1].Value.ToString() == cGlobalParas.PublishResult.UnPublished.ToString())
                        return this.dataData.Rows[i];
                }
            }
            return null;
        }

        #endregion

        //去除指定字符串的网页符号
        private string getTxt(string instr)
        {
            if (instr == null)
                return "";

            string m_outstr;

            m_outstr = instr.Clone() as string;
            System.Text.RegularExpressions.Regex objReg = new System.Text.RegularExpressions.Regex("(<[^>]+?>)|&nbsp;", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg.Replace(m_outstr, "");
            System.Text.RegularExpressions.Regex objReg2 = new System.Text.RegularExpressions.Regex("(\\s)+", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg2.Replace(m_outstr, " ");

            return m_outstr;
        }


        #region 委托代理 用于后台线程调用 配置UI线程的方法、属性

        delegate void bindvalue(object Instance, string Property, object value);
        delegate object invokemethod(object Instance, string Method, object[] parameters);
        delegate object invokepmethod(object Instance, string Property, string Method, object[] parameters);
        delegate object invokechailmethod(object InstanceInvokeRequired, object Instance, string Method, object[] parameters);

        /// <summary>
        /// 委托设置对象属性
        /// </summary>
        /// <param name="Instance">对象</param>
        /// <param name="Property">属性名</param>
        /// <param name="value">属性值</param>
        private void SetValue(object Instance, string Property, object value)
        {
            Type iType = Instance.GetType();
            object inst=null;

            if (iType.Name.ToString() == "ToolStripButton" || iType.Name.ToString() == "ToolStripStatusLabel" || iType.Name.ToString() == "DataGridView" || iType.Name.ToString() == "ToolStripProgressBar")
            {
                inst = this.dataNavi;
            }
            else
            {
                inst = Instance;
            }

            bool a = (bool)GetPropertyValue(inst, "InvokeRequired");

            if (a)
            {
                bindvalue d = new bindvalue(SetValue);
                this.Invoke(d, new object[] { Instance, Property, value });
            }
            else
            {
                SetPropertyValue(Instance, Property, value);
            }
        }
        /// <summary>
        /// 委托执行实例的方法，方法必须都是Public 否则会出错
        /// </summary>
        /// <param name="Instance">类实例</param>
        /// <param name="Method">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private object InvokeMethod(object Instance, string Method, object[] parameters)
        {
            if ((bool)GetPropertyValue(Instance, "InvokeRequired"))
            {
                invokemethod d = new invokemethod(InvokeMethod);
                return this.Invoke(d, new object[] { Instance, Method, parameters });
            }
            else
            {
                return MethodInvoke(Instance, Method, parameters);
            }
        }

        /// <summary>
        /// 委托执行实例的方法
        /// </summary>
        /// <param name="InstanceInvokeRequired">窗体控件对象</param>
        /// <param name="Instance">需要执行方法的对象</param>
        /// <param name="Method">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private object InvokeChailMethod(object InstanceInvokeRequired, object Instance, string Method, object[] parameters)
        {
            if ((bool)GetPropertyValue(InstanceInvokeRequired, "InvokeRequired"))
            {
                invokechailmethod d = new invokechailmethod(InvokeChailMethod);
                return this.Invoke(d, new object[] { InstanceInvokeRequired, Instance, Method, parameters });
            }
            else
            {
                return MethodInvoke(Instance, Method, parameters);
            }
        }
        /// <summary>
        /// 委托执行实例的属性的方法
        /// </summary>
        /// <param name="Instance">类实例</param>
        /// <param name="Property">属性名</param>
        /// <param name="Method">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private object InvokePMethod(object Instance, string Property, string Method, object[] parameters)
        {
            if ((bool)GetPropertyValue(Instance, "InvokeRequired"))
            {
                invokepmethod d = new invokepmethod(InvokePMethod);
                return this.Invoke(d, new object[] { Instance, Property, Method, parameters });
            }
            else
            {
                return MethodInvoke(GetPropertyValue(Instance, Property), Method, parameters);
            }
        }
        /// <summary>
        /// 获取实例的属性值
        /// </summary>
        /// <param name="ClassInstance">类实例</param>
        /// <param name="PropertyName">属性名</param>
        /// <returns>属性值</returns>
        private static object GetPropertyValue(object ClassInstance, string PropertyName)
        {
            Type myType = ClassInstance.GetType();
            PropertyInfo myPropertyInfo = myType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return myPropertyInfo.GetValue(ClassInstance, null);
        }
        /// <summary>
        /// 设置实例的属性值
        /// </summary>
        /// <param name="ClassInstance">类实例</param>
        /// <param name="PropertyName">属性名</param>
        private static void SetPropertyValue(object ClassInstance, string PropertyName, object PropertyValue)
        {
            Type myType = ClassInstance.GetType();
            PropertyInfo myPropertyInfo = myType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            myPropertyInfo.SetValue(ClassInstance, PropertyValue, null);
        }

        /// <summary>
        /// 执行实例的方法
        /// </summary>
        /// <param name="ClassInstance">类实例</param>
        /// <param name="MethodName">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private static object MethodInvoke(object ClassInstance, string MethodName, object[] parameters)
        {
            if (parameters == null)
            {
                parameters = new object[0];
            }
            Type myType = ClassInstance.GetType();
            Type[] types = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                types[i] = parameters[i].GetType();
            }
            MethodInfo myMethodInfo = myType.GetMethod(MethodName, types);
            return myMethodInfo.Invoke(ClassInstance, parameters);
        }

        #endregion

        public void ExportLog(string str)
        {
            this.txtLog.Text = str + "\r\n" + this.txtLog.Text;

        }

        private void staShowAll_Click(object sender, EventArgs e)
        {
            m_dataSource.RemoveFilter();

            this.staShowAll.Visible = false;
            stateFilter.Text = "无";
        }

        private void treePublish_DoubleClick(object sender, EventArgs e)
        {
            FillPublishInfo(this.treePublish.SelectedNode.Text);

            this.m_fState = cGlobalParas.FormState.Edit;
            this.m_pName = this.treePublish.SelectedNode.Text;

        }

        private void FillPublishInfo(string pName)
        {

            if (MessageBox.Show(rm.GetString("Info2248"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            oPublishTask p = new oPublishTask(m_workPath);
            ePublishTask ep= p.LoadSingleTask(pName);

            if (ep.PublishType== cGlobalParas.PublishType.publishTemplate)
            {
                this.udThread.Value = ep.ThreadCount;
                if ( ep.IsDelRepeatRow== true)
                    this.IsDelRepRow.Checked = true;
                else
                    this.IsDelRepRow.Checked = false;

                this.IsPublishByTemplate.Checked = true;
                this.raPublishData.Enabled  = false ;
                this.raPublishWeb.Enabled  = false ;

                this.comTemplate.Text =ep.TemplateName;
                this.txtUser.Text =ep.User;
                this.txtPwd.Text = ep.Password;
                this.txtDomain.Text = ep.Domain;
                this.txtTDbConn.Text = ep.TemplateDBConn;

                this.dataParas.Rows.Clear();

                for (int i = 0; i < ep.PublishParas.Count; i++)
                {
                    this.dataParas.Rows.Add();
                    this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[0].Value = ep.PublishParas[i].DataLabel;
                    cGlobalParas.PublishParaType pType = ep.PublishParas[i].DataType;
                    if (pType == cGlobalParas.PublishParaType.NoData)
                    {
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[2].Value = "";
                    }
                    if (pType == cGlobalParas.PublishParaType.Gather)
                    {
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[1].Value = ep.PublishParas[i].DataValue;
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[2].Value = "";
                    }
                    else if (pType == cGlobalParas.PublishParaType.Custom)
                    {
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[1].Value = "{手工输入}";
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[2].Value = ep.PublishParas[i].DataValue;
                    }
                }
            }
            else
            {
                this.IsPublishByTemplate.Checked = false;
                if (ep.PublishType == cGlobalParas.PublishType.PublishData)
                {
                    this.raPublishData.Checked = true;
                    this.udThread.Value = ep.ThreadCount;
                    if (ep.IsDelRepeatRow == true)
                        this.IsDelRepRow.Checked = true;
                    else
                        this.IsDelRepRow.Checked = false;

                    cGlobalParas.DatabaseType dType = ep.DataType;
                    if (dType == cGlobalParas.DatabaseType.Access)
                        this.raExportAccess.Checked = true;
                    else if (dType == cGlobalParas.DatabaseType.MSSqlServer)
                        this.raExportMSSQL.Checked = true;
                    else if (dType == cGlobalParas.DatabaseType.MySql)
                        this.raExportMySql.Checked = true;
                    else if (dType == cGlobalParas.DatabaseType.Oracle)
                        this.raExportOracle.Checked = true;

                    this.txtDataSource.Text = ep.DataSource;
                    this.comTableName.Text = ep.DataTable ;
                    this.txtInsertSql.Text = ep.InsertSql;
                    this.IsSqlTrue.Checked =ep.IsSqlTrue; 

                    this.comExportUrlCode.SelectedIndex = -1;
                    this.txtExportUrl.Text = "";
                    this.txtExportCookie.Text = "";
                    this.txtSucceedFlag.Text = "";
                }
            }
            p.Dispose();
            p = null;

            this.butSaveTemplate.Enabled = false;
        }

        private void rmenuImportPublishInfo_Click(object sender, EventArgs e)
        {
            FillPublishInfo(this.treePublish.SelectedNode.Text);
        }

        private void rmenuDelPublishInfo_Click(object sender, EventArgs e)
        {
            if (this.treePublish.SelectedNode.Name.Substring(0, 1) == "P")
            {
                DelPublishInfo(this.treePublish.SelectedNode.Text);
            }
        }

        private void DelPublishInfo(string pName)
        {
            if (MessageBox.Show(rm.GetString("Quaere30") + pName, rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            oPublishTask p = new oPublishTask(m_workPath);
            p.DelTask(pName);
            p = null;

            this.treePublish.Nodes.Remove(this.treePublish.SelectedNode);
        }

        private void dataData_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            String filterStatus = DataGridViewAutoFilterColumnHeaderCell.GetFilterStatus(dataData);

            this.stateRecords.Text = " " + rm.GetString("Info222") + " " + (this.dataData.Rows.Count - 1) + " " + rm.GetString("Info223");

            if (this.dataData.Columns.Count == 0)
            {
                this.toolSaveData.Enabled = false;

                this.toolAddField.Enabled = false;
                this.toolDelField.Enabled = false;
                this.toolFilter.Enabled = false;
                this.toolEdit.Enabled = false;
                this.toolStartPublish.Enabled = false;

                this.toolSaveData.Enabled = false;
                this.toolAddField.Enabled = false;
                this.toolDelField.Enabled = false;
            }
            else
            {
                this.toolAddField.Enabled = true;
                this.toolDelField.Enabled = true;
                this.toolAddField.Enabled = true;
                this.toolDelField.Enabled = true;

                if (this.dataData.Rows.Count == 0)
                {
                    this.toolFilter.Enabled = false;
                    this.toolEdit.Enabled = false;

                    this.toolStartPublish.Enabled = false;

                    this.toolSaveData.Enabled = false;
                }
                else
                {
                    //this.toolSaveData.Enabled = true;

                    this.toolFilter.Enabled = true;
                    this.toolEdit.Enabled = true;

                    if (this.toolIsPublish.Checked == true)
                        this.toolStartPublish.Enabled = true;
                    else
                        this.toolStartPublish.Enabled = false;

                }
            }

        }

        private void treePublish_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (this.treePublish.SelectedNode.Name.Substring(0, 1) == "P")
                    {
                        DelPublishInfo(this.treePublish.SelectedNode.Text);
                    }
                    break;
            }
        }

        private void rmenuGetFormat_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "rmenuPublishPostData")
            {
                frmBrowser wftm = new frmBrowser();
                wftm.getFlag = 3;
                wftm.rExportPData = new frmBrowser.ReturnExportPOST(GetExportpData);
                wftm.ShowDialog();
                wftm.Dispose();

                return;
            }

            Match s;

            if (Regex.IsMatch(e.ClickedItem.ToString(), "[{].*[}]"))
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[{].*[}]");
            }
            else
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[<].*[>]");
            }

            int startPos = this.txtExportUrl.SelectionStart;
            int l = this.txtExportUrl.SelectionLength;

            this.txtExportUrl.Text = this.txtExportUrl.Text.Substring(0, startPos) + s.Groups[0].Value + this.txtExportUrl.Text.Substring(startPos + l, this.txtExportUrl.Text.Length - startPos - l);

            this.txtExportUrl.SelectionStart = startPos + s.Groups[0].Value.Length;
            this.txtExportUrl.ScrollToCaret();
        }

        private void GetExportpData(string strCookie, string pData)
        {
            this.txtExportUrl.Text += "<POST:ASCII>" + pData + "</POST>";
        }

        private void rmenuGetFormat_Opening(object sender, CancelEventArgs e)
        {
            this.rmenuGetFormat.Items.Clear();
            this.rmenuGetFormat.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu5") + "<POST:ASCII> 可手动修改编码，譬如：UTF8", null, null, "rmenuPost1"));
            this.rmenuGetFormat.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu6") + "</POST>", null, null, "rmenuPost2"));
            this.rmenuGetFormat.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu7"), null, null, "rmenuPublishPostData"));
            this.rmenuGetFormat.Items.Add(new ToolStripSeparator());

            for (int i = 0; i < this.dataData.Columns.Count; i++)
            {
                this.rmenuGetFormat.Items.Add("{" + this.dataData.Columns[i].Name + "}");
            }
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string FilterCondition = "";

            switch (e.ClickedItem.Name)
            {
                case "rmenuEqual":
                    FilterCondition = e.ClickedItem.Text.Substring(e.ClickedItem.Text.IndexOf(" ") + 1, e.ClickedItem.Text.Length - e.ClickedItem.Text.IndexOf(" ") - 1);
                    this.stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info205") + " " + FilterCondition;
                    m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " = '" + FilterCondition + "'";

                    this.staShowAll.Visible = true;

                    break;
                case "rmenuUnEqual":
                    FilterCondition = e.ClickedItem.Text.Substring(e.ClickedItem.Text.IndexOf(" ") + 1, e.ClickedItem.Text.Length - e.ClickedItem.Text.IndexOf(" ") - 1);
                    this.stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info206") + " " + FilterCondition;
                    m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " <> '" + FilterCondition + "'";

                    this.staShowAll.Visible = true;

                    break;
                case "rmenuInclude":
                    FilterCondition = e.ClickedItem.Text.Substring(e.ClickedItem.Text.IndexOf(" ") + 1, e.ClickedItem.Text.Length - e.ClickedItem.Text.IndexOf(" ") - 1);
                    this.stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " " + rm.GetString("Info209") + " " + FilterCondition;
                    m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " like '*" + FilterCondition + "*'";

                    this.staShowAll.Visible = true;

                    break;
                default:
                    break;
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (m_IsRunning == true)
                return;

            if (m_curVersion == cGlobalParas.VersionType.Cloud ||
                m_curVersion == cGlobalParas.VersionType.Ultimate ||
                m_curVersion == cGlobalParas.VersionType.Enterprise)
            {
                NetMiner.Core.Plugin.cPlugin p = new NetMiner.Core.Plugin.cPlugin(m_workPath);
                int count = p.GetCount();
                this.rmenuPlugin.DropDownItems.Clear();
                for (int i = 0; i < count; i++)
                {
                    if (p.GetPluginType(i) == cGlobalParas.PluginsType.DealData)
                    {
                        this.rmenuPlugin.DropDownItems.Add(p.GetPluginName(i), null, e_EditByPluginEvent);
                    }

                }
                p = null;
            }
            else
            {
                this.rmenuPlugin.Visible = false;
            }

            //开始控制右键菜单的状态
            if (this.dataData.Columns.Count == 0)
            {
                this.rmenuAddFiled.Enabled = false;
                this.rmenuDelField.Enabled = false;
                this.rmenuSelectCol.Enabled = false;
                this.rmenuFilter.Enabled = false;
                this.rmenuEdit.Enabled = false;
                this.rmenuPlugin.Enabled = false;
            }
            else
            {
                this.rmenuAddFiled.Enabled = true;
                this.rmenuDelField.Enabled = true;
                this.rmenuSelectCol.Enabled = true;
                this.rmenuFilter.Enabled = true;
                this.rmenuEdit.Enabled = true;
                this.rmenuPlugin.Enabled = true;
            }

            if (this.dataData.Rows.Count != 0 && this.dataData.CurrentCell.Value != null)
            {
                if (this.contextMenuStrip1.Items.Count > 9)
                {
                    this.contextMenuStrip1.Items.Remove(this.contextMenuStrip1.Items["rmenuEqual"]);
                    this.contextMenuStrip1.Items.Remove(this.contextMenuStrip1.Items["rmenuUnEqual"]);
                    this.contextMenuStrip1.Items.Remove(this.contextMenuStrip1.Items["rmenuInclude"]);
                }

                this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu9") + " " + this.dataData.CurrentCell.Value.ToString(), null, null, "rmenuEqual"));
                this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu10") + " " + this.dataData.CurrentCell.Value.ToString(), null, null, "rmenuUnEqual"));
                this.contextMenuStrip1.Items.Add(new ToolStripMenuItem(rm.GetString("rmenu11") + " " + this.dataData.CurrentCell.Value.ToString(), null, null, "rmenuInclude"));

            }
        }

        private void dataData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            
            //if (this.dataData[this.dataData.ColumnCount - 1, e.RowIndex].Value.ToString ().StartsWith ("True",StringComparison.CurrentCultureIgnoreCase))
                this.toolSaveData.Enabled = true;
            
        }

        private void dataData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //string s="列：" + e.ColumnIndex + " 行：" + e.RowIndex + "发生错误:" + e.Exception.Message ;

            //ExportLog(s.ToString ());
            
        }

        private void toolDelRepeatRow_Click(object sender, EventArgs e)
        {
            DataDelRepeatRow();
        }

        private void rmenuDelRepeatRow_Click(object sender, EventArgs e)
        {
            DataDelRepeatRow();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmSetHeader f = new frmSetHeader();
            f.strHeader = this.txtHeader.Text;
            f.rHeader = GetHttpHeader;
            f.ShowDialog();
            f.Dispose();
        }

        private void GetHttpHeader(string strHeader)
        {
            this.txtHeader.Text = strHeader;
        }

        private void IsHeader_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsHeader.Checked == true)
            {
                //this.txtHeader.Enabled = true;
                this.button1.Enabled = true;
            }
            else
            {
                //this.txtHeader.Enabled = false;
                this.button1.Enabled = false;
            }
        }

        private void rmenuExport_Click(object sender, EventArgs e)
        {
            if (this.treePublish.Nodes.Count ==0)
                return ;

            this.saveFileDialog1.Title = "导出发布规则";
            saveFileDialog1.InitialDirectory = m_workPath;
            saveFileDialog1.Filter = "SominerPublish Files(*.smp)|*.smp";

            string pName=this.treePublish.SelectedNode.Text;

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string FileName = this.saveFileDialog1.FileName;

                oPublishTask p = new oPublishTask(m_workPath);

                ePublishTask ep = p.LoadSingleTask(pName);

                string sXML = NetMiner.Common.Tool.XMLUtil.Serializer(typeof(ePublishTask), ep);
                NetMiner.Common.Tool.cFile.SaveFileBinary(FileName, sXML, true);

                MessageBox.Show("导出成功！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void rmenuImport_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "导入发布规则";
            openFileDialog1.InitialDirectory = m_workPath;
            openFileDialog1.Filter = "SominerPublish Files(*.smp)|*.smp";

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = this.openFileDialog1.FileName;

                try
                {
                    oPublishTask p = new oPublishTask(m_workPath);

                    cXmlIO xmlConfig = new cXmlIO(fileName);
                    DataView PInfo = xmlConfig.GetData("PublishInfos");

                    string pName = PInfo[0].Row["Name"].ToString();
                    if (p.IsExist(pName))
                    {
                        if (MessageBox.Show("已经存在此采集规则，是否覆盖？", "网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                        {
                            p = null;
                            xmlConfig = null;
                            return;
                        }

                        //删除原有规则
                        p.DelTask(pName);
                    }

                    ePublishTask ep = new ePublishTask();
                    ep.pName = pName;
                    ep.ThreadCount = int.Parse(PInfo[0].Row["ThreadCount"].ToString());
                    if (PInfo[0].Row["IsDelRepeatRow"].ToString().ToLower() == "true")
                        ep.IsDelRepeatRow = true;
                    else
                        ep.IsDelRepeatRow = false;

                    ep.PublishType = (cGlobalParas.PublishType)int.Parse(PInfo[0].Row["PublishType"].ToString());
                    ep.DataType = (cGlobalParas.DatabaseType)int.Parse(PInfo[0].Row["DataType"].ToString());
                    ep.DataSource = PInfo[0].Row["DataSource"].ToString();
                    ep.DataTable = PInfo[0].Row["DataTable"].ToString();
                    ep.InsertSql = PInfo[0].Row["InsertSql"].ToString();
                    ep.PostUrl = PInfo[0].Row["PostUrl"].ToString();
                    ep.UrlCode = (cGlobalParas.WebCode)int.Parse(PInfo[0].Row["UrlCode"].ToString());
                    ep.Cookie = PInfo[0].Row["Cookie"].ToString();
                    ep.SucceedFlag = PInfo[0].Row["SucceedFlag"].ToString();
                    if (PInfo[0].Row["IsHeader"].ToString().ToLower() == "true")
                        ep.IsHeader = true;
                    else
                        ep.IsHeader = false;
                    ep.Header = PInfo[0].Row["Header"].ToString();
                    ep.PIntervalTime =int.Parse ( PInfo[0].Row["PublishIntervalTime"].ToString());

                    //开始导入发布模版的信息，如果不存在则为空
                    ep.TemplateName = PInfo[0].Row["TemplateName"].ToString();
                    ep.User = PInfo[0].Row["User"].ToString();
                    ep.Password = PInfo[0].Row["Password"].ToString();
                    ep.Domain = PInfo[0].Row["Domain"].ToString();
                    ep.TemplateDBConn = PInfo[0].Row["PublishDbConn"].ToString();

                    if (PInfo[0].DataView.Table.ChildRelations.Count > 0)
                    {
                         List<ePublishData> paras = new List<ePublishData>();
                         if (PInfo[0].CreateChildView("PublishInfos_Paras").Count > 0)
                         {
                             DataView dw = PInfo[0].CreateChildView("PublishInfos_Paras")[0].CreateChildView("Paras_Para");

                             for (int i = 0; i < dw.Count; i++)
                             {
                                ePublishData para = new ePublishData();
                                 para.DataLabel = dw[i].Row["Label"].ToString();
                                 para.DataType = (cGlobalParas.PublishParaType)int.Parse(dw[i].Row["Type"].ToString());
                                 para.DataValue = dw[i].Row["Value"].ToString();
                                 ep.PublishParas.Add(para);
                             }
                         }
                        //for (int i = 0; i < this.dataParas.Rows.Count; i++)
                        //{
                        //    cPublishPara para = new cPublishPara();
                        //    para.PublishPara = this.dataParas.Rows[i].Cells[0].Value.ToString();
                        //    if (this.dataParas.Rows[i].Cells[1].Value == null)
                        //    {
                        //        para.PublishParaType = cGlobalParas.PublishParaType.NoData;
                        //        para.PublishValue = "";
                        //    }
                        //    else
                        //    {
                        //        if ("{手工输入}" == this.dataParas.Rows[i].Cells[1].Value.ToString())
                        //        {
                        //            para.PublishParaType = cGlobalParas.PublishParaType.Custom;
                        //            if (this.dataParas.Rows[i].Cells[2].Value == null)
                        //                para.PublishValue = "";
                        //            else
                        //                para.PublishValue = this.dataParas.Rows[i].Cells[2].Value.ToString();
                        //        }
                        //        else
                        //        {
                        //            para.PublishValue = this.dataParas.Rows[i].Cells[1].Value.ToString();
                        //            para.PublishParaType = cGlobalParas.PublishParaType.Gather;
                        //        }
                        //    }
                        //    p.PublishParas.Add(para);
                        //}
                    }

                    p.InsertPublishInfo(ep);
                    p = null;
                    xmlConfig = null;

                    MessageBox.Show("导入成功！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    IniTree();

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("导入失败，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                   
                    return;
                }
            }

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Parent.Dispose();
        }

        private void toolmenuPostData_Click(object sender, EventArgs e)
        {

        }

        private void txtHeader_TextChanged(object sender, EventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void rmenuLookPublishData_Click(object sender, EventArgs e)
        {
            frmLookPResult f = new frmLookPResult();
            f.Loadweb(this.m_PublishSource,this.txtExportUrl.Text );
            f.Show();
        }

        //重新绑定数据
        public void ReBindData1(DataTable d)
        {
            string tableName = m_tmp.TableName;
            m_pData = d;
            m_tmp = m_pData;
            m_tmp.TableName = tableName;
            m_dataSource = new BindingSource(m_tmp, null);

            this.dataData.DataSource = null;
            this.dataData.DataSource = m_dataSource;

            this.dataNavi.BindingSource = m_dataSource;

            if (MessageBox.Show("发布成功的数据已从表格删除，但没有进行保存操作，是否进行保存？", "网络矿工 信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
                SaveData();

        }

        private void editByRuleEvent(object sender, EventArgs e)
        {
            string pName = ((ToolStripDropDownItem)sender).Text;
            string pDll = "";
            cPlugin p = new cPlugin(m_workPath);
            int count = p.GetCount();
            int pIndex = 0;
            for (pIndex = 0; pIndex < count; pIndex++)
            {
                if (p.GetPluginName(pIndex) == pName)
                {
                    pDll = p.GetPlugin(pIndex);
                    break;
                }
            }
            p = null;

            cRunPlugin rPlugin = new cRunPlugin();

            try
            {
                DataTable d = m_tmp.Copy();
                if (pDll != "")
                {
                    d = rPlugin.CallDealData(d, pDll);
                }
                
                m_dataSource.DataSource = d;

                this.toolSaveData.Enabled = true;
                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show ("编辑失败，失败信息：" + ex.Message,"网络矿工 信息",MessageBoxButtons.OK,MessageBoxIcon.Information );
            }

            rPlugin = null;
        }


        #region 事件定义
        private event EventHandler e_EditByPluginEvent;
        public event EventHandler EditByPluginEvent
        {
            add { e_EditByPluginEvent += value; }
            remove { e_EditByPluginEvent -= value; }
        }
        #endregion

        private void butNew_Click(object sender, EventArgs e)
        {
            this.m_fState = cGlobalParas.FormState.New;
            this.m_pName = "";

            this.udThread.Value = 5;
            this.IsDelRepRow.Checked = true;

            this.raPublishData.Checked = true;
            this.txtDataSource.Text = "";
            this.comTableName.Items.Clear();
            this.comTableName.Text = "";
            this.txtInsertSql.Text = "";

            this.comExportUrlCode.SelectedIndex = -1;
            this.udPIntervalTime.Value = 0;
            this.txtSucceedFlag.Text = "";
            this.txtExportUrl.Text = "";
            this.txtExportCookie.Text = "";
            this.txtHeader.Text = "";
            this.IsHeader.Checked = false;

            this.comTemplate.SelectedIndex = -1;
            this.txtUser.Text = "";
            this.txtPwd.Text = "";
            this.txtDomain.Text = "";
            this.txtTDbConn.Text = "";
            this.dataParas.Rows.Clear();
        }

        private void IsPublishByTemplate_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsPublishByTemplate.Checked == true)
            {
                this.groupData.Visible = false;
                this.groupWeb.Visible = false;
                this.groupTemplate.Visible = true;

                this.raPublishData.Enabled = false;
                this.raPublishWeb.Enabled = false;
            }
            else if (this.IsPublishByTemplate.Checked ==false)
            {
                if (this.raPublishData.Checked == true)
                {
                    this.groupData.Visible = true;
                    this.groupWeb.Visible = false;
                    this.groupTemplate.Visible = false ;
                }
                else if (this.raPublishWeb.Checked == true)
                {
                    this.groupData.Visible = false ;
                    this.groupWeb.Visible = true;
                    this.groupTemplate.Visible = false ;
                }

                this.raPublishData.Enabled = true;
                this.raPublishWeb.Enabled = true;
            }
            this.butSaveTemplate.Enabled = true;
        }

        private void comTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strType = this.comTemplate.Text;
            if (strType == "")
            {
                this.PDataColumn.Items.Clear();
                this.dataParas.Rows.Clear();
                return;
            }

            strType = strType.Substring(strType.IndexOf("[") + 1, strType.IndexOf("]") - strType.IndexOf("[") - 1);
            string tName = this.comTemplate.Text.Substring(0, this.comTemplate.Text.IndexOf("["));

            this.PDataColumn.Items.Clear();
            for (int i = 0; i < this.dataData.Columns.Count; i++)
            {
                this.PDataColumn.Items.Add(this.dataData.Columns[i].Name);
            }

            this.PDataColumn.Items.Add("{手工输入}");

            this.dataParas.Rows.Clear();

            if (EnumUtil.GetEnumName<cGlobalParas.PublishTemplateType>(strType) == cGlobalParas.PublishTemplateType.Web)
            {
                this.groupBox2.Enabled = true;
                this.groupBox3.Enabled = false;

                //加载参数
                cTemplate t = new cTemplate(m_workPath);
                t.LoadTemplate(tName);

                this.dataParas.Rows.Add();
                this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[0].Value = "系统分类";

                foreach (KeyValuePair<string, string> de in t.PublishParas)
                {
                    string ss = de.Value.ToString();
                    ss = ss.Replace("：", ":");
                    Regex re = new Regex("(?<={发布参数:).+?(?=})", RegexOptions.None);
                    MatchCollection mc = re.Matches(ss);
                    foreach (Match ma in mc)
                    {
                        this.dataParas.Rows.Add();
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[0].Value = ma.Value.ToString();
                    }
                }

                foreach (KeyValuePair<string, string> de in t.UploadParas)
                {
                    string ss = de.Value.ToString();
                    ss = ss.Replace("：", ":");
                    Regex re = new Regex("(?<={上传参数:).+?(?=})", RegexOptions.None);
                    MatchCollection mc = re.Matches(ss);
                    foreach (Match ma in mc)
                    {
                        if (ma.ToString() != "当前上传的文件名")
                        {
                            this.dataParas.Rows.Add();
                            this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[0].Value = ma.Value.ToString();
                        }
                    }
                }
                this.butSaveTemplate.Enabled = true;
            }
            else if (EnumUtil.GetEnumName<cGlobalParas.PublishTemplateType>(strType) == cGlobalParas.PublishTemplateType.DB)
            {
                this.groupBox2.Enabled = false ;
                this.groupBox3.Enabled = true;
                cDbTemplate t = new cDbTemplate(m_workPath);
                t.LoadTemplate(tName);

                this.txtTDbConn.Tag = ((int)t.DbType).ToString ();

                //分解sql语句中的参数
                for (int i = 0; i < t.sqlParas.Count; i++)
                {
                    string ss = t.sqlParas[i].Sql;
                    ss = ss.Replace("：", ":");
                    Regex re = new Regex("(?<={发布参数:).+?(?=})", RegexOptions.None);
                    MatchCollection mc = re.Matches(ss);
                    foreach (Match ma in mc)
                    {
                        this.dataParas.Rows.Add();
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[0].Value = ma.Value.ToString();
                    }
                }
                t = null;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmSetData fSD = new frmSetData(m_workPath );

            if (this.txtTDbConn.Tag == null)
            {
                MessageBox.Show("请先选择需要使用的发布模版 !", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            cGlobalParas.DatabaseType dType = (cGlobalParas.DatabaseType)int.Parse(this.txtTDbConn.Tag.ToString());


            if (dType==cGlobalParas.DatabaseType.Access)
                fSD.FormState = 0;
            else if (dType == cGlobalParas.DatabaseType.MSSqlServer)
                fSD.FormState = 1;
            else if (dType == cGlobalParas.DatabaseType.MySql)
                fSD.FormState = 2;

            fSD.rDataSource = new frmSetData.ReturnDataSource(GetTemplateConn);
            fSD.ShowDialog();
            fSD.Dispose();
        }



        private void GetTemplateConn(string strDataConn)
        {
            this.txtTDbConn.Text = strDataConn;
        }

        private void dataParas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void txtPwd_TextChanged(object sender, EventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void txtDomain_TextChanged(object sender, EventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void txtTDbConn_TextChanged(object sender, EventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void dataParas_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void dataParas_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            this.dataParas.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
            MessageBox.Show("您选择的发布规则与当前需要发布的数据不匹配，系统已经重置了此信息，请根据需要发布的数据重新进行配置！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void dataData_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            this.toolSaveData.Enabled = true;
        }

        private void dataData_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            this.toolSaveData.Enabled = true;
        }

        private void dataData_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            this.toolSaveData.Enabled = true;
        }

        private void dataData_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            this.toolSaveData.Enabled = true;
        }

        private void EditColHeader()
        {
            int CurCol = this.dataData.CurrentCell.ColumnIndex;
            string oldname = this.dataData.Columns[CurCol].HeaderText;

            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = rm.GetString("Info200");
            ffc.label1.Text = rm.GetString("Info200");
            ffc.txtValue.Text = oldname;
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                try
                {
                    this.dataData.Columns[CurCol].HeaderText = m_FilterValue;

                    for (int i = 0; i < ((DataTable)m_dataSource.DataSource).Columns.Count; i++)
                    {
                        if (((DataTable)m_dataSource.DataSource).Columns[i].ColumnName == oldname)
                        {
                            ((DataTable)m_dataSource.DataSource).Columns[i].ColumnName = m_FilterValue;
                            break;
                        }
                    }
                    
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }

            ffc.Dispose();

            //string colName = this.dataData.Columns[CurCol].HeaderText;
        }

        private void toolEditColHeader_Click(object sender, EventArgs e)
        {
            //非法字符特指数据库中的非法字符，包括()?!'",#@*^%.
            string strReg = @"\(|\)|\*|&|\^|%|\$|#|@|!|,|\.|\?|/|-";

            for (int i = 0; i < this.dataData.Columns.Count; i++)
            {
                string oldName = this.dataData.Columns[i].HeaderText;
                if (Regex.IsMatch(oldName, strReg))
                {
                    oldName = Regex.Replace(oldName, strReg, "", RegexOptions.IgnoreCase);
                    this.dataData.Columns[i].HeaderText = oldName;
                    ((DataTable)m_dataSource.DataSource).Columns[i].ColumnName = oldName;
                }
            }
        }

        private void rmenuEditColHeader_Click(object sender, EventArgs e)
        {
            EditColHeader();
        }

        private void IsSqlTrue_CheckedChanged(object sender, EventArgs e)
        {
            this.butSaveTemplate.Enabled = true;
        }

        private void raExportOracle_CheckedChanged(object sender, EventArgs e)
        {
            this.txtDataSource.Text = "";
            this.comTableName.Items.Clear();
            this.butSaveTemplate.Enabled = true;
        }


    }
}

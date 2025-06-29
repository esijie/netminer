using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using SoukeyNetget.CustomControl;
using System.Text.RegularExpressions;

namespace SoukeyNetget
{
    public partial class frmDataTool : Form
    {
        //定义一个访问资源文件的变量
        private ResourceManager rm;

        private BindingSource m_dataSource;

        private string m_DataFile = "";

        private string m_FilterValue = "";

        public frmDataTool()
        {
            InitializeComponent();
        }

        public frmDataTool(string DataFile)
        {
            InitializeComponent();
            m_DataFile = DataFile;
        }

        private void frmDataTool_Load(object sender, EventArgs e)
        {
            //初始化资源文件的参数
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());

            if (m_DataFile != "")
            {
                DataTable tmp = new DataTable();
                tmp.ReadXml(m_DataFile);

                m_dataSource = new BindingSource(tmp, null);
                this.dataData.DataSource = m_dataSource;
            }

        }

        private void frmDataTool_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
            this.Dispose();
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
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.toolSave.Enabled = true;
            }

            ffc.Dispose();
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

            this.toolSave.Enabled = true;
        }


        private void GetFilterValue(string Value)
        {
            m_FilterValue = Value;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.dataData.CurrentCell.Value !=null)
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

        private void rmenuSelectRow_Click(object sender, EventArgs e)
        {
            this.dataData.Rows[this.dataData.CurrentCell.RowIndex].Selected = true;
            
        }

        private void rmenuSelectCol_Click(object sender, EventArgs e)
        {
            int curCol=this.dataData.CurrentCell.ColumnIndex;

            for (int i = 0; i < this.dataData.Rows.Count; i++)
            {
                this.dataData[curCol, i].Selected = true;
            }
        }

        private void rmenuAddFiled_Click(object sender, EventArgs e)
        {
            AddField();
        }

        private void toolFilter_Click(object sender, EventArgs e)
        {
          
        }

        private void dataData_BindingContextChanged(object sender, EventArgs e)
        {
            // Continue only if the data source has been set.
            if (dataData.DataSource == null)
            {
                return;
            }

            // Add the AutoFilter header cell to each column.
            foreach (DataGridViewColumn col in dataData.Columns)
            {
                col.HeaderCell = new
                    DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
            }

            // Format the OrderTotal column as currency. 
            //dataData.Columns["OrderTotal"].DefaultCellStyle.Format = "c";

            // Resize the columns to fit their contents.
            dataData.AutoResizeColumns();
        
        }

        private void dataData_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            String filterStatus = DataGridViewAutoFilterColumnHeaderCell.GetFilterStatus(dataData);

            this.stateRecords.Text = " " + rm.GetString ("Info222") + (this.dataData.Rows.Count-1) + rm.GetString ("Info223");
           
            //if (String.IsNullOrEmpty(filterStatus))
            //{
            //    this.toolFilterInfo.Text = "All";

            //}
            //else
            //{
            //    this.toolFilterInfo.Text = filterStatus;
            //}
        }

        private void dataData_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            this.toolSave.Enabled = true;
        }

        private void toolDelField_Click(object sender, EventArgs e)
        {
            DelField();
        }

        private void toolAutoID_Click(object sender, EventArgs e)
        {
            DataAutoID();
        }

        #region 筛选代码
        private void FilterMore()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex ].Name + rm.GetString("Info203");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info203");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

               this.stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info203") + m_FilterValue ;
               m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " > '" + m_FilterValue + "'";

            }

            ffc.Dispose();
        }

        private void FilterLess()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info204");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info204");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                stateFilter.Text  = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info204") + m_FilterValue ;
                m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " < '" + m_FilterValue + "'";

            }

            ffc.Dispose();
        }

        private void FilterEqual()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info205");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info205");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

               stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info205") + m_FilterValue;
               m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " = '" + m_FilterValue + "'";

            }

            ffc.Dispose();
        }

        private void FilterNoEqual()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info206");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info206");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

               stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info206") + m_FilterValue;
               m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " <> '" + m_FilterValue + "'";

            }

            ffc.Dispose();
        }

        private void FilterStart()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info207");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info207");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info207") + m_FilterValue;
                m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " like '" + m_FilterValue + "*'";

            }

            ffc.Dispose();
        }

        private void FilterInclude()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info209");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info209");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

               stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info209") + m_FilterValue;
               m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " like '*" + m_FilterValue + "*'";

            }

            ffc.Dispose();
        }

        private void FilterEnd()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info211");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info211");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

               stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info211") + m_FilterValue;
               m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " like '*" + m_FilterValue + "'";

            }
        }

        private void FilterStartNo()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info208");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info208");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                int l = m_FilterValue.Length;

               stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info208") + m_FilterValue;
               m_dataSource.Filter = "Substring(" + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + ",1," + l.ToString() + ")<>'" + m_FilterValue + "'";

            }

            ffc.Dispose();
        }

        private void FilterLengthMore()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info220");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info220");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                int l = m_FilterValue.Length;

                stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info220") + m_FilterValue;
                m_dataSource.Filter = "len(" + this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + ")>'" + m_FilterValue + "'";

            }

            ffc.Dispose();
        }

        private void FilterCustom()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info221");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info221");
            ffc.rValue = new frmFilterCondition.ReturnValue(this.GetFilterValue);
            if (ffc.ShowDialog() == DialogResult.OK)
            {
                if (m_FilterValue == "")
                    return;

                stateFilter.Text = m_FilterValue;
                m_dataSource.Filter = m_FilterValue;
            }

            ffc.Dispose();
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

        private void toolNoEqual_Click(object sender, EventArgs e)
        {
            FilterNoEqual();
        }

        private void toolStart_Click(object sender, EventArgs e)
        {
            FilterStart();
        }

        private void rmenuTxtStart_Click(object sender, EventArgs e)
        {
            FilterStart();
        }

        private void toolInclude_Click(object sender, EventArgs e)
        {
            FilterInclude();
        }

        private void rmenuTxtInclude_Click(object sender, EventArgs e)
        {
            FilterInclude();
        }

        private void toolEnd_Click(object sender, EventArgs e)
        {
            FilterEnd();
        }

        private void rmenuTxtEnd_Click(object sender, EventArgs e)
        {
            FilterEnd();
        }

        private void rmenuTxtStartNo_Click(object sender, EventArgs e)
        {
            FilterStartNo();
        }

        private void toolStartNo_Click(object sender, EventArgs e)
        {
            FilterStartNo();
        }

        private void toolLenMore_Click(object sender, EventArgs e)
        {
            FilterLengthMore();
        }

        private void rmenuLenMore_Click(object sender, EventArgs e)
        {
            FilterLengthMore();
        }

        private void toolCustomFilter_Click(object sender, EventArgs e)
        {
            FilterCustom();
        }

        private void rmenuCustomFilter_Click(object sender, EventArgs e)
        {
            FilterCustom();
        }

        #endregion

        private void DataAutoID()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info213");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info213");
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

            int StartI=0;

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
                this.dataData[CurCol, i].Value =(StartI + i);
            }

        }

        private void DataAddPrefix()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info214");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info214");
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
                this.dataData.SelectedCells[i].Value =m_FilterValue + this.dataData.SelectedCells[i].Value;
            }

        }

        private void DataAddSuffix()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info215");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info215");
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
                this.dataData.SelectedCells[i].Value =this.dataData.SelectedCells[i].Value +  m_FilterValue ;
            }

        }

        private void DataReplace()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info216");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info216");
            ffc.txtValue.Text ="\"\",\"\"";
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
                string oStr = m_FilterValue.Substring(1, m_FilterValue.IndexOf(",") - 2);
                string nStr = m_FilterValue.Substring(m_FilterValue.IndexOf(",") + 2, m_FilterValue.Length - m_FilterValue.IndexOf(",") - 3);
                this.dataData.SelectedCells[i].Value = Regex.Replace(this.dataData.SelectedCells[i].Value.ToString (), oStr, nStr, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            }

        }

        private void DataCutLeft()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info216");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info216");
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
                }
            }

        }

        private void DataCutRight()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info216");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info216");
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
                }

            }

        }

        private void DataSetEmpty()
        {
            if (MessageBox.Show(rm.GetString("Quaere23"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            for (int i = 0; i < this.dataData.SelectedCells.Count; i++)
            {
               
                this.dataData.SelectedCells[i].Value = null ;

            }
        }

        private void DataValue()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info219");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info219");
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
                   
            }
        }

        private void DataFunction()
        {
            frmFilterCondition ffc = new frmFilterCondition();
            ffc.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info219");
            ffc.label1.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info219");
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

            for (int i = 0; i < this.dataData.SelectedCells.Count; i++)
            {

                //this.dataData.SelectedCells[i].Value = m_FilterValue

            }
        }

        private void rmenuAutoID_Click(object sender, EventArgs e)
        {
            DataAutoID();
        }

        private void toolAddPre_Click(object sender, EventArgs e)
        {
            DataAddPrefix();
        }

        private void rmenuAddPre_Click(object sender, EventArgs e)
        {
            DataAddPrefix();
        }

        private void rmenuAddSuffix_Click(object sender, EventArgs e)
        {
            DataAddSuffix();
        }

        private void toolAddSuffix_Click(object sender, EventArgs e)
        {
            DataAddSuffix();
        }

        private void toolReplace_Click(object sender, EventArgs e)
        {
            DataReplace();
        }

        private void rmenuReplace_Click(object sender, EventArgs e)
        {
            DataReplace();
        }

        private void toolCutLeft_Click(object sender, EventArgs e)
        {
            DataCutLeft();
        }

        private void rmenuCutLeft_Click(object sender, EventArgs e)
        {
            DataCutLeft();
        }

        private void toolCutRight_Click(object sender, EventArgs e)
        {
            DataCutRight();
        }

        private void rmenuCutRight_Click(object sender, EventArgs e)
        {
            DataCutRight();
        }

        private void rmenuDelField_Click(object sender, EventArgs e)
        {
            DelField();
        }

        private void toolSetEmpty_Click(object sender, EventArgs e)
        {
            DataSetEmpty();
        }

        private void rmenuSetEmpty_Click(object sender, EventArgs e)
        {
            DataSetEmpty();
        }

        private void dataData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void toolValue_Click(object sender, EventArgs e)
        {
            DataValue();
        }

        private void rmenuValue_Click(object sender, EventArgs e)
        {
            DataValue();
        }

        private void toolCustomData_Click(object sender, EventArgs e)
        {
            DataFunction();
        }

        private void rmenuCustomData_Click(object sender, EventArgs e)
        {
            DataFunction();
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string FilterCondition = "";

            switch (e.ClickedItem.Name )
            {
                case "rmenuEqual":
                    FilterCondition = e.ClickedItem.Text.Substring(e.ClickedItem.Text.IndexOf(" ") + 1, e.ClickedItem.Text.Length - e.ClickedItem.Text.IndexOf(" ")-1);
                    this.stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info205") + FilterCondition;
                    m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " = '" + FilterCondition + "'";

                    break;
                case "rmenuUnEqual":
                    FilterCondition = e.ClickedItem.Text.Substring(e.ClickedItem.Text.IndexOf(" ") + 1, e.ClickedItem.Text.Length - e.ClickedItem.Text.IndexOf(" ") - 1);
                    this.stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info206") + FilterCondition;
                    m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " <> '" + FilterCondition + "'";

                    break;
                case "rmenuInclude":
                    FilterCondition = e.ClickedItem.Text.Substring(e.ClickedItem.Text.IndexOf(" ") + 1, e.ClickedItem.Text.Length - e.ClickedItem.Text.IndexOf(" ") - 1);
                    this.stateFilter.Text = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + rm.GetString("Info209") + FilterCondition;
                    m_dataSource.Filter = this.dataData.Columns[this.dataData.CurrentCell.ColumnIndex].Name + " like '*" + FilterCondition + "*'";
                    break;
                default :
                    break;
            }
        }

        private void toolNoFilter_Click(object sender, EventArgs e)
        {
            m_dataSource.RemoveFilter();

            stateFilter.Text  = "无";
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolSave_Click(object sender, EventArgs e)
        {

        }

       

    }
}
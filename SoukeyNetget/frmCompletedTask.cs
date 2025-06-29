using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using NetMiner.Resource;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;
using System.Resources;
using System.Reflection;
using System.IO;
using SoukeyControl.CustomControl;
using System.Threading;
using NetMiner.Publish;

namespace MinerSpider
{
    public partial class frmCompletedTask : DockContent
    {
        private ResourceManager rm;
        public frmCompletedTask()
        {
            InitializeComponent();
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));
            this.DockAreas= DockAreas.DockRight;
        }

        public void IniData()
        {
            LoadCompleteTask();
        }

        public void LoadCompleteTask()
        {

            ShowCompletedTask();

            //从完成的任务中加载
            oTaskComplete t = new oTaskComplete(Program.getPrjPath());
            IEnumerable<eTaskCompleted> tTasks = t.LoadTaskData();

            foreach (eTaskCompleted ec in tTasks)
            {
                dataTask.Rows.Add(imageList1.Images["OK"], ec.TaskID, cGlobalParas.TaskState.Completed, ec.TaskClass,
                                   ec.TaskName, ec.StartDate.ToString("MM-dd HH:mm"), ec.CompleteDate.ToString("MM-dd HH:mm"), ec.RowsCount  );

                StringBuilder sb = new StringBuilder();
                sb.Append("执行类型：" + ec.TaskRunType.GetDescription() + "\r\n");
                sb.Append("发布类型：" + ec.PublishType.GetDescription() + "\r\n");
                sb.Append("采集网址数量：" + ec.GatheredUrlCount + "\r\n");
                sb.Append("采集结果" + ec.GatherResult.GetDescription() + "\r\n");
                this.dataTask[0, dataTask.Rows.Count - 1].ToolTipText = sb.ToString();
            }

            this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

            this.dataTask.ClearSelection();

        }

        public void AddCompletedTask(eTaskCompleted ec)
        {
            long TaskID = ec.TaskID;
            string tClass = ec.TaskClass;
            string tName = ec.TaskName;
            DateTime sDate = ec.StartDate;
            DateTime eDate = ec.CompleteDate;
            int rowCount = ec.RowsCount;

            dataTask.Rows.Add(imageList1.Images["OK"], TaskID, cGlobalParas.TaskState.Completed, tClass,
                                   tName, sDate.ToString("MM-dd HH:mm"), eDate.ToString("MM-dd HH:mm"), rowCount);
        }

        private void ShowCompletedTask()
        {
            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {
            }

            #region 此部分为固定显示
            DataGridViewImageColumn tStateImg = new DataGridViewImageColumn();
            tStateImg.HeaderText = rm.GetString("GridState");
            tStateImg.Width = 40;
            tStateImg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tStateImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(0, tStateImg);

            //任务编号,不显示此列
            DataGridViewTextBoxColumn tID = new DataGridViewTextBoxColumn();
            tID.Name = rm.GetString("GridTaskID");
            tID.Width = 0;
            tID.Visible = false;
            this.dataTask.Columns.Insert(1, tID);

            //任务状态,不显示此列
            DataGridViewTextBoxColumn tState = new DataGridViewTextBoxColumn();
            tState.Name = rm.GetString("GridState");
            tState.Width = 0;
            tState.Visible = false;
            this.dataTask.Columns.Insert(2, tState);

            #endregion

            DataGridViewTextBoxColumn tClass = new DataGridViewTextBoxColumn();
            tClass.HeaderText = "分类";
            tClass.Width = 60;
            this.dataTask.Columns.Insert(3, tClass);

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = rm.GetString("GridTaskName");
            tName.Width = 60;
            this.dataTask.Columns.Insert(4, tName);

            DataGridViewTextBoxColumn tType = new DataGridViewTextBoxColumn();
            tType.HeaderText = "启动时间";
            tType.MinimumWidth = 60;
            this.dataTask.Columns.Insert(5, tType);

            DataGridViewTextBoxColumn tcDate = new DataGridViewTextBoxColumn();
            tcDate.HeaderText = rm.GetString("GridTaskCompleteDate");
            tcDate.MinimumWidth = 60;
            this.dataTask.Columns.Insert(6, tcDate);

            DataGridViewTextBoxColumn tRowCount = new DataGridViewTextBoxColumn();
            tRowCount.HeaderText = "数据量";
            tRowCount.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            tRowCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(7, tRowCount);

            DataGridViewProgressBarColumn tPro = new DataGridViewProgressBarColumn();
            tPro.HeaderText = rm.GetString("GridProcess");
            tPro.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            tPro.Visible = false;
            this.dataTask.Columns.Insert(8, tPro);

        }

        private void frmCompletedTask_FormClosing(object sender, FormClosingEventArgs e)
        {
            rm = null;
        }

        private void dataTask_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;


            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (!DelCompletedTask())
                    {
                        e.SuppressKeyPress = true;
                        return;
                    }
                    break;

                case Keys.Enter:

                    break;
                default:

                    return;

            }
        }

        private bool DelCompletedTask()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return false;

            if (this.dataTask.SelectedRows.Count == 1)
            {
                if (MessageBox.Show(rm.GetString("Info29") + this.dataTask.SelectedCells[4].Value.ToString() + "\r\n" +
                    rm.GetString("Quaere10"), rm.GetString("MessageboxQuaere"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }
            else
            {
                if (MessageBox.Show(rm.GetString("Quaere11"),
                    rm.GetString("MessageboxQuaere"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }

            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {
                Int64 TaskID = Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString());
                string TaskName = this.dataTask.SelectedRows[index].Cells[4].Value.ToString();

                //删除taskcomplete节点
                oTaskComplete tc = new oTaskComplete(Program.getPrjPath());
                tc.LoadTaskData();
                tc.DelTask(TaskID);
                tc.Dispose();
                tc = null;

                //删除run中的任务实例文件
                string FileName = Program.getPrjPath() + "data\\" + TaskName + "-" + TaskID + ".xml";
                System.IO.File.Delete(FileName);
            }

            return true;

            //while (this.dataTask.SelectedRows.Count > 0)
            //{
            //    this.dataTask.Rows.Remove(this.dataTask.SelectedRows[0]);
            //}

        }

        private void dataTask_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditData();
        }

        private void rmenuEditData_Click(object sender, EventArgs e)
        {
            EditData();
        }

        public void EditData()
        {
            if (this.dataTask.Rows.Count == 0)
                return;

            Int64 TaskID = Int64.Parse(this.dataTask.SelectedRows[0].Cells[1].Value.ToString());
            string TaskName = this.dataTask.SelectedRows[0].Cells[4].Value.ToString();
            string dFile = "";
            DataTable tmp = new DataTable();

            //判断是浏览的那些数据：正在运行还是采集完成
        
                //oTaskComplete tc = new oTaskComplete(Program.getPrjPath());
                //tc.LoadSingleTask(TaskID);

                //dFile = tc.GetTempFile(0);
                //tc = null;

                //如果是已经完成的任务，则直接打开编辑器进行数据发布操作

                OpenCompletedData();
                return;
            

        }

        private void OpenCompletedData()
        {
            if (this.dataTask.Rows.Count == 0)
                return;

            Int64 TaskID = Int64.Parse(this.dataTask.SelectedRows[0].Cells[1].Value.ToString());
            string TaskName = this.dataTask.SelectedRows[0].Cells[4].Value.ToString();
            string dFile = "";
            DataTable tmp = new DataTable();
            oTaskComplete tc = new oTaskComplete(Program.getPrjPath());
            eTaskCompleted ec = tc.LoadSingleTask(TaskID);
            dFile = ec.TempFile;
            tc = null;

            if (e_OpenDataEvent != null)
            {
                e_OpenDataEvent(this, new OpenDataEvent(cGlobalParas.DatabaseType.SoukeyData, dFile, "", TaskName));
            }
        }

        #region 事件
        private readonly Object m_eventLock = new Object();
        private event EventHandler<OpenDataEvent> e_OpenDataEvent;
        public event EventHandler<OpenDataEvent> OpenDataEvent
        {
            add { lock (m_eventLock) { e_OpenDataEvent += value; } }
            remove { lock (m_eventLock) { e_OpenDataEvent -= value; } }
        }

        #endregion

        private void rMenuRefer_Click(object sender, EventArgs e)
        {
            LoadCompleteTask();
        }

        private void rmmenuDelTask_Click(object sender, EventArgs e)
        {
            this.dataTask.Focus();
            SendKeys.Send("{Del}");
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if (this.dataTask.SelectedRows.Count ==0)
            {
                this.rMenuExportCSV.Enabled = false;
                this.rMenuExportExcel.Enabled = false;
            }
            else
            {
                this.rMenuExportCSV.Enabled = true;
                this.rMenuExportExcel.Enabled = true;
            }
        }

        private void rMenuExportCSV_Click(object sender, EventArgs e)
        {

            string FileName;

            this.saveFileDialog1.OverwritePrompt = true;
            this.saveFileDialog1.Title = rm.GetString("Info36");
            saveFileDialog1.InitialDirectory = Program.getPrjPath();
            saveFileDialog1.Filter = "CSV Files(*.csv)|*.csv|All Files(*.*)|*.*";
            //saveFileDialog1.FileName = this.tabControl1.SelectedTab.Tag.ToString() + ".csv";

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileName = this.saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            Application.DoEvents();

            ExportData(FileName, cGlobalParas.PublishType.PublishCSV);
        }

        private void rMenuExportExcel_Click(object sender, EventArgs e)
        {

        }

        delegate void ShowProgressDelegate(int totalMessages, int messagesSoFar, long TaskID);
        private void ExportData(string FileName, cGlobalParas.PublishType pType)
        {
            cExport eTxt = new cExport(Program.getPrjPath());


            Int64 TaskID = Int64.Parse(this.dataTask.SelectedRows[0].Cells[1].Value.ToString());
            string TaskName = this.dataTask.SelectedRows[0].Cells[4].Value.ToString();
            string dFile = "";
            DataTable tmp = new DataTable();
            oTaskComplete tc = new oTaskComplete(Program.getPrjPath());
            eTaskCompleted ec = tc.LoadSingleTask(TaskID);
            dFile = ec.TempFile;
            tc = null;

            DataGridViewProgressBarCell pBar = (DataGridViewProgressBarCell)this.dataTask.SelectedRows[0].Cells[8];

            //将数据拷贝出来
            DataTable d = new DataTable();
            d.ReadXml(dFile);

            //定义一个后台线程用于导出数据操作
            ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);
            cExport eExcel = new cExport(this, showProgress, pType, FileName, d);
            string[] ps = new string[] { TaskID.ToString () };
            Thread t = new Thread(new ParameterizedThreadStart(eExcel.RunProcess));
            t.IsBackground = true;
            t.Start(ps);

         
        }

        private void ShowProgress(int total, int messagesSoFar,long TaskID)
        {
            
            //在此更新rowCount
            DataGridViewRow dRow = (from a in this.dataTask.Rows.Cast<DataGridViewRow>()
                                    where a.Cells[1].Value.Equals(TaskID)
                                    select a).FirstOrDefault();

            //dRow.Cells[7].Visible = false;
            //dRow.Cells[8].Visible = true;
            this.dataTask.Columns[8].Visible = true;
            ((DataGridViewProgressBarCell)dRow.Cells[8]).Maximum = total;
            ((DataGridViewProgressBarCell)dRow.Cells[8]).Mimimum = 0;
            ((DataGridViewProgressBarCell)dRow.Cells[8]).Value = messagesSoFar;

            if(total==messagesSoFar)
            {
                this.dataTask.Columns[8].Visible = false;

                if (e_ShowLogInfo!=null)
                {
                    e_ShowLogInfo(this, new ShowInfoEvent("导出完毕", dRow.Cells[4].Value.ToString() + " 数据导出完毕！"));
                }
                
            }
        }

        private event EventHandler<ShowInfoEvent> e_ShowLogInfo;
        public event EventHandler<ShowInfoEvent> ShowLogInfo
        {
            add { lock (m_eventLock) { e_ShowLogInfo += value; } }
            remove { lock (m_eventLock) { e_ShowLogInfo -= value; } }
        }

        private void rMenuExportTxt_Click(object sender, EventArgs e)
        {
            string FileName;

            this.saveFileDialog1.OverwritePrompt = true;
            this.saveFileDialog1.Title = rm.GetString("Info36");
            saveFileDialog1.InitialDirectory = Program.getPrjPath();
            saveFileDialog1.Filter = "Txt Files(*.txt)|*.txt|All Files(*.*)|*.*";
            //saveFileDialog1.FileName = this.tabControl1.SelectedTab.Tag.ToString() + ".txt";

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileName = this.saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            Application.DoEvents();

            ExportData(FileName, cGlobalParas.PublishType.PublishTxt);
        }

        private void rMenuExportWord_Click(object sender, EventArgs e)
        {
            string FileName;

            this.saveFileDialog1.OverwritePrompt = true;
            this.saveFileDialog1.Title = rm.GetString("Info36");
            saveFileDialog1.InitialDirectory = Program.getPrjPath();
            saveFileDialog1.Filter = "Word Files(*.docx)|*.docx|All Files(*.*)|*.*";
            //saveFileDialog1.FileName = this.tabControl1.SelectedTab.Tag.ToString() + ".docx";

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileName = this.saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            Application.DoEvents();

            ExportData(FileName, cGlobalParas.PublishType.publishWord);
        }
    }
}

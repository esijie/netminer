using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using System.IO;
using NetMiner.Gather.Control;
using SoukeyControl.CustomControl;
using NetMiner.Gather.Listener;
using NetMiner.Core.Plan;
using WeifenLuo.WinFormsUI.Docking;
using System.Threading;
using System.Diagnostics;
using NetMiner.Core.Log;
using NetMiner.Core.Plugin;
using NetMiner.Resource;
using NetMiner.Common;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NetMiner;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;
using NetMiner.Core.Event;
using NetMiner.Core.pTask;
using NetMiner.Core.pTask.Entity;
using NetMiner.Publish;
using NetMiner.Core.Entity;
using NetMiner.Core.Radar;
using NetMiner.Core.Radar.Entity;
using NetMiner.Gather.Radar;
using System.Text;
using System.Linq;

namespace MinerSpider
{
    public partial class frmRunningTask : DockContent 
    {
        private ResourceManager rm;
        private DataGridViewCellStyle m_RowStyleErr;
        private string OldName = "";

        public cGatherControl m_GatherControl;
        public cPublishControl m_PublishControl;
        private cListenControl m_ListenControl;

        //每次上传文件的数量
        private const int UploadFileLength = 5120000;

        private bool IsTimer = true;

        //是否保存系统日志标记,默认不保存
        private bool m_IsAutoSaveLog = false;

        //定义一个值判断当前是否有手工导出的数据正在进行
        private bool m_IsExportData = false;

        //定义一个值，用于记录手工导出记录的tab页是哪一个，当前仅允许手工同时导出一个数据集
        private string m_ExportPageName;
        
        //定义一个值，表示当前的计划执行的监听器是否在运行
        private bool m_IsRunListen = false;

        //定义一个值，记录当前日志输出的最大行数，系统控制最大行数主要是考虑到日志对性能的影响
        private int m_MaxLogNumber;

        //定义一个值，记录最大允许执行远程采集任务的数量
        private int m_MaxRemoteCount;

        private cRadarControl m_RadarControl;

        ToolTip HelpTip = new ToolTip();

        //定义一个定时器，定期与远程服务器进行通讯，并根据
        //当前的配置来判断是否索取采集任务进行数据采集操作
        private System.Threading.Timer m_RemoteEngine;
        private bool m_IsAllowRemoteGather;
        private string m_RemoteServer;
        private bool m_IsDo = false;

        //定义一个值，用于存储在上传采集任务时选择的远程任务分类编号
        private int m_RemoteTaskClassID = 0;

        public int MaxLogNumber
        {
            get { return m_MaxLogNumber; }
            set { m_MaxLogNumber = value; }
        }

        public frmRunningTask()
        {
            InitializeComponent();

            this.DockAreas = DockAreas.DockRight;

            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            this.tabControl1.CloseTab += this.CloseTab;
        }

        private void CloseTab(object sender, cCloseTabEvent e)
        {
            string TaskID = this.tabControl1.TabPages[e.Index].Name.Substring(4, this.tabControl1.SelectedTab.Name.Length - 4);
            for (int index = 0; index < m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count; index++)
            {
                if (m_GatherControl.TaskManage.TaskListControl.RunningTaskList[index].TaskID == long.Parse (TaskID))
                {
                    
                        MessageBox.Show(rm.GetString("Info34"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
            }

            //for (int i = 0; i < this.tabControl1.TabPages[e.Index].Controls.Count; i++)
            //{
            //    if (this.tabControl1.TabPages[e.Index].Controls[i] != null)
            //    {
            //        this.tabControl1.TabPages[e.Index].Controls[i].Dispose();
            //    }
            //}

            RemoveTab(e.Index);

            this.tabControl1.CloseTabPage(e.Index);

            //this.tabControl1.TabPages.Remove(this.tabControl1.SelectedTab);

            //判断是否已经没有选项卡，如果没有，则隐藏tab显示
            if (this.tabControl1.TabPages.Count == 0)
                this.splitContainer2.Panel2Collapsed = true;
        }

        delegate void MyDelegate();


        private void dataTask_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (this.dataTask.CurrentCell.Value == null)
                OldName = "";
            else
                OldName = this.dataTask.CurrentCell.Value.ToString();
        }

        private void dataTask_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SetFormToolbarState();

            if (this.dataTask.Rows.Count != 0 && this.dataTask.SelectedCells.Count != 0 
                && this.dataTask.SelectedCells[1].Value.ToString() != "" 
                )
            {
                
                Int64 TaskID = Int64.Parse(this.dataTask.SelectedCells[1].Value.ToString());
                string pageName = "page" + TaskID;

                for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                {
                    if (this.tabControl1.TabPages[i].Name == pageName)
                    {
                        this.tabControl1.SelectedIndex = i;
                        break;
                    }
                }

                //设置按钮状态
                //SetFormToolbarState();
                
                this.dataTask.Focus();

            }
        }

        private void dataTask_DoubleClick(object sender, EventArgs e)
        {
            if (this.dataTask.SelectedCells.Count == 0)
                return;

            BrowserMultiData();

            //switch (this.dataTask.SelectedCells[3].Value.ToString ())
            //{
            //    case "nodSnap":
            //        break;
            //    case "nodRunning":
            //        //if (e_ExcuteFunction != null)
            //        //    e_ExcuteFunction(this, new ExcuteFunctionEvent("EditTask", null));
            //        //EditTask();
            //        break;
            //    case "nodPublish":
            //        BrowserMultiData();
            //        break;
            //    case "nodComplete":
            //        OpenCompletedData();
            //        //BrowserMultiData();
            //        break;
            //    case "nodRadarRule":
            //        //EditRadar();
            //        break;
            //    case "nodTaskPlan":
            //        break;

            //    case "nodPlanCompleted":
            //        break;
            //    default:
            //        break;
            //}
        }

        private void dataTask_Enter(object sender, EventArgs e)
        {
            if (e_DelInfo != null)
                e_DelInfo(this, new DelInfoEvent("GatherTask"));

            if (e_ResetToolbarState != null)
                e_ResetToolbarState(this, new ResetToolbarStateEvent());

        }

        private void dataTask_KeyDown(object sender, KeyEventArgs e)
        {
    
            if (this.dataTask.SelectedRows.Count == 0)
                return;


            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if ( DelRunTask()==false)
                    {
                        e.SuppressKeyPress = true;
                        return;
                    }
                    break;
            
                case Keys.Enter :
                   
                    break;
                default:
                  
                    return;

            }
        }

        private void dataTask_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.dataTask.SelectedRows.Count == 0)
            {
                return;
            }

            //SetToolbarState();

            //判断是否为左键点击，如果是则需要启动拖放操作
            if (((e.Button & MouseButtons.Left) == MouseButtons.Left && this.dataTask.SelectedCells[3].Value.ToString() == "nodTaskClass") ||
                ((e.Button & MouseButtons.Left) == MouseButtons.Left && this.dataTask.SelectedCells[3].Value.ToString().Substring(0, 1) == "C"))
            {
                DataGridViewSelectedRowCollection dragData = this.dataTask.SelectedRows;
                //Size dragSize = SystemInformation.DragSize;
                //Rectangle dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
                this.dataTask.DoDragDrop(dragData, DragDropEffects.Copy);
            }
        }

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point pt = new Point(e.X, e.Y);
                Rectangle recTab = new Rectangle();
                for (int i = 0; i < tabControl1.TabCount; i++)
                {
                    recTab = tabControl1.GetTabRect(i);
                    if (recTab.Contains(pt))
                        this.tabControl1.SelectedIndex = i;
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == -1)
            {
                e_SetControlProperty(this, new SetControlPropertyEvent("menuExportData", "Enabled", "false"));
                return;
            }

            if (this.tabControl1.SelectedIndex == 0)
            {
                if (e_SetControlProperty != null)
                {
                    e_SetControlProperty(this, new SetControlPropertyEvent("menuExportData", "Enabled", "false"));
                    return;
                }
            }

            DataGridView tmp = (DataGridView)this.tabControl1.SelectedTab.Controls[0].Controls[0].Controls[0];
            if (tmp.Rows.Count > 0)
            {
                if (e_SetControlProperty != null)
                {
                    e_SetControlProperty(this, new SetControlPropertyEvent("menuExportData", "Enabled", "true"));
                }
            }
            else
            {
                if (e_SetControlProperty != null)
                {
                    e_SetControlProperty(this, new SetControlPropertyEvent("menuExportData", "Enabled", "false"));
                }
            }
        }

        private void frmTaskContent_Load(object sender, EventArgs e)
        {
            this.splitContainer2.Panel2Collapsed = true;
        }

        private void frmTaskContent_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        /// <summary>
        /// 继续执行采集任务，即当前采集任务停止后，tab页面也未关闭的情况下，继续执行
        /// </summary>
        /// <param name="TaskID"></param>
        public void StartTask(Int64 TaskID)
        {
            cGatherTask t = null;

            t = m_GatherControl.TaskManage.FindTask(TaskID);
            if (t == null)
            {
                //表示任务启动失败
                return;
            }

            //在此判断是否需要进行外部参数的传递，如果需要则打开fInputPara
            //如果任务已经运行，则表示任务中的网址已经分解出来了
            //if (t.TaskType == cGlobalParas.TaskType.ExternalPara)
            //{
            //    fInputPara fi = new fInputPara();
            //    fi.Owner = this;
            //    fi.rUrl = new fInputPara.ReturnUrl(GetUrlPara);
            //    fi.GetPara(t.Weblink[0].Weblink);

            //    if (fi.ShowDialog() == DialogResult.Cancel)
            //    {
            //        fi.Dispose();
            //        MessageBox.Show(rm.GetString("Info233"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);

            //        return;
            //    }
            //    fi.Dispose();

            //    t.UpdateUrl(this.UrlPara);
            //}

            try
            {
                //启动此任务
                m_GatherControl.Start(t);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("采集任务启动失败，可能由于采集任务配置发生了错误，请检查采集网址及采集规则是否已经配置，如还无法解决，请与网络矿工客服人员联系！", "网络矿工 错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            //任务启动成功显示消息
            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("TaskStarted"),t.TaskName ));

            t = null;
        }

        /// <summary>
        /// 启动采集任务，用于新启动一个任务
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="TaskName"></param>
        /// <param name="SelectedIndex"></param>
        public void StartTask(Int64 TaskID, string TaskName,string TaskClass,string TaskPath)
        {
            cGatherTask t = null;

            //判断当前选择的树节点
      
            ///如果是选择的任务分类节点，点击此按钮首先先将此任务加载到运行区，然后调用
            ///starttask方法，启动任务。
            string tClassName = TaskClass;
            string tClassPath = TaskPath;

            t = AddRunTask(tClassName,tClassPath, TaskName);

            //如果是新增的任务，则传进来的TaskID是任务的编号，并不是任务执行的编号（即不是由时间自动产生的任务）
            //是一个递增的整数，所以，需要重新更新传入的TaskID
    
            if (t == null)
            {
                //表示任务启动失败
                return;
            }

            TaskID = t.TaskID;

            //在此判断是否需要进行外部参数的传递，如果需要则打开fInputPara
            //if (t.TaskType == cGlobalParas.TaskType.ExternalPara)
            //{
            //    fInputPara fi = new fInputPara();
            //    fi.Owner = this;
            //    fi.rUrl = new fInputPara.ReturnUrl(GetUrlPara);
            //    fi.GetPara(t.Weblink[0].Weblink);

            //    if (fi.ShowDialog() == DialogResult.Cancel)
            //    {
            //        fi.Dispose();
            //        MessageBox.Show(rm.GetString("Info233"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);

            //        return;
            //    }
            //    fi.Dispose();

            //    t.UpdateUrl(this.UrlPara);
            //}

            //任务成功启动后，需要建立TabPage用于显示此任务的日志及采集数据的信息
            AddTab(TaskID, TaskName);

            //在列表中增加一条数据
            this.dataTask.Rows.Add(imageList1.Images["run"], TaskID, cGlobalParas.TaskState.Running, tClassPath,
                             tClassName, TaskName, (int)cGlobalParas.TaskProcess.Gather,
                             System.DateTime.Now.ToString("MM-dd HH:mm"), t.UrlCount ,
                             "0", "0", 0 / t.UrlCount);

            try
            {
                //启动此任务
                m_GatherControl.Start(t);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("采集任务启动失败，可能由于采集任务配置发生了错误，请检查采集网址及采集规则是否已经配置，如还无法解决，请与网络矿工客服人员联系！", "网络矿工 错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            //任务启动成功显示消息
            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("TaskStarted"),TaskName ));

            t = null;
        }

        /// <summary>
        /// 启动采集任务，继续执行已经停止的采集任务，即采集任务被停止，也关闭了相关的资源（Tab页面，日志及数据）
        /// </summary>
        private void StartTask(Int64 TaskID, string TaskName, int SelectedIndex)
        {
            cGatherTask t = null;

            //判断当前选择的树节点
           
                //执行正在执行的任务
            t = m_GatherControl.TaskManage.FindTask(TaskID);
            

            if (t == null)
            {
                //表示任务启动失败
                return;
            }

            //在此判断是否需要进行外部参数的传递，如果需要则打开fInputPara

            //if (t.TaskType == cGlobalParas.TaskType.ExternalPara)
            //{
            //    fInputPara fi = new fInputPara();
            //    fi.Owner = this;
            //    fi.rUrl = new fInputPara.ReturnUrl(GetUrlPara);
            //    fi.GetPara(t.Weblink[0].Weblink);

            //    if (fi.ShowDialog() == DialogResult.Cancel)
            //    {
            //        fi.Dispose();
            //        MessageBox.Show(rm.GetString("Info233"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);

            //        return;
            //    }
            //    fi.Dispose();

            //    t.UpdateUrl(this.UrlPara);
            //}

            //任务成功启动后，需要建立TabPage用于显示此任务的日志及采集数据的信息
            if (int.Parse(this.dataTask.SelectedRows[SelectedIndex].Cells[8].Value.ToString()) > 0)
            {
                int rowsCount = BrowserData(TaskID, TaskName);
            }
            else
            {
                AddTab(TaskID, TaskName);
            }

            try
            {
                //启动此任务
                m_GatherControl.Start(t);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("采集任务启动失败，可能由于采集任务配置发生了错误，请检查采集网址及采集规则是否已经配置，如还无法解决，请与网络矿工客服人员联系！", "网络矿工 错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            //任务启动成功显示消息
            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("TaskStarted"), TaskName));

            t = null;
        }
        /// <summary>
        /// 
        /// </summary>
        private void StartMultiTask()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {
                if ((cGlobalParas.TaskState)this.dataTask.SelectedRows[index].Cells[2].Value == cGlobalParas.TaskState.Failed)
                {
                    continue;
                }
                else
                {
                    StartTask(Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString()), 
                        this.dataTask.SelectedRows[index].Cells[5].Value.ToString(), index);
                }
            }
        }

        /// <summary>
        /// 启动发布任务
        /// </summary>
        private void StartMultiPublish()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {
                if ((cGlobalParas.TaskState)this.dataTask.SelectedRows[index].Cells[2].Value == cGlobalParas.TaskState.PublishFailed)
                {
                    continue;
                }
                else
                {
                    StartPublish(Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString()), this.dataTask.SelectedRows[index].Cells[4].Value.ToString(),true);
                }
            }
        }

        private void StartPublish(Int64 TaskID, string TaskName,bool isAddTab)
        {
            if (isAddTab ==true )
                BrowserData(TaskID, TaskName);

            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            DataTable d = (DataTable)((DataGridView)this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0]).DataSource;

            //先删除已有的，再进行发布
            cPublish pt = m_PublishControl.PublishManage.FindTask(TaskID);
            m_PublishControl.Remove(pt);
            pt = null;

            pt = new cPublish(Program.getPrjPath(),m_PublishControl.PublishManage, TaskID, d);
            m_PublishControl.startPublish(pt);

            //任务启动成功显示消息
            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent("发布已启动！",TaskName));
        }

        #region 自动添加控件用于显示任务执行的结果

        /// <summary>
        /// 启动运行任务的时候，增加现实的Tab
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="TaskName"></param>
        public void AddTab(Int64 TaskID, string TaskName)
        {

            if (this.splitContainer2.Panel2Collapsed == true)
                this.splitContainer2.Panel2Collapsed = false;

            bool IsExist = false;
            int j = 0;

            //判断此任务是否已经添加了Tab页
            for (j = 0; j < this.tabControl1.TabPages.Count; j++)
            {
                if (this.tabControl1.TabPages[j].Name == "page" + TaskID.ToString())
                {
                    IsExist = true;
                    break;
                }
            }

            if (IsExist == true)
            {
                this.tabControl1.SelectedTab = this.tabControl1.TabPages[j];
                return;
            }


            TabPage tPage = new TabPage();
            tPage.Name = "page" + TaskID.ToString();

            //附件任务名称的信息
            tPage.Tag = TaskName;

            tPage.Text = TaskName;
            tPage.ImageIndex = 19;
            

            SplitContainer sc = new SplitContainer();
            sc.Name = "sCon" + TaskID.ToString();
            sc.Orientation = Orientation.Horizontal;
            sc.Dock = DockStyle.Fill;

            //增加一个grid用于显示采集得到的数据
            cMyDataGridView d = new cMyDataGridView(Program.SominerVersion);
            d.Name = "grid" + TaskID.ToString();
            d.TaskRunID = TaskID;
            d.TaskName = TaskName;
            d.AllowUserToOrderColumns = true;
            d.AllowUserToDeleteRows = false;
            d.Dock = DockStyle.Fill;
            d.MouseDown += this.Tab_MouseDown;
            d.ContextMenuStrip = this.contextMenuStrip4;
            sc.Panel1.Controls.Add(d);

             //增加一个进度条，用于显示导出数据时的进度信息
            ProgressBar p = new ProgressBar();
            p.Name = "tPro" + TaskID.ToString();
            p.Dock = DockStyle.Bottom;
            p.Visible = false;
            sc.Panel2.Controls.Add(p);

            //增加一个文本框，用于显示采集输出日志
            cMyTextLog r = new cMyTextLog();
            r.Name = "tLog" + TaskID.ToString();
            r.ReadOnly = true;
            r.BorderStyle = BorderStyle.FixedSingle;
            r.BackColor = Color.White;
            r.DetectUrls = true;
            r.WordWrap = false;
            r.Dock = DockStyle.Fill;
            r.ContextMenuStrip = this.contextMenuStrip5;
            r.LinkClicked += this.TextLogLinkClick;

            sc.Panel2.Controls.Add(r);

            tPage.Controls.Add(sc);

            this.tabControl1.TabPages.Add(tPage);
            this.tabControl1.SelectedTab = tPage;

            

        }

        public void AddDataGridRow(long TaskID,string tClassPath,string tClassName,string TaskName,int UrlCount)
        {
            //增加显示操作
            this.dataTask.Rows.Add(imageList1.Images["run"], TaskID, cGlobalParas.TaskState.Running, tClassPath,
                             tClassName, TaskName, (int)cGlobalParas.TaskProcess.Gather,
                             System.DateTime.Now.ToString("MM-dd HH:mm"), UrlCount,
                             "0", "0", 0 / UrlCount);
        }

        private void RemoveTab(int Index)
        {
            RemoveTabControl(this.tabControl1.TabPages[Index]);

            this.tabControl1.TabPages[Index].Controls.Clear();
        }

        private void RemoveTabControl(Control c)
        {
            
            for (int i=0;i<c.Controls.Count ;i++)
            {
                if (c.Controls[i].Controls.Count > 0)
                {
                    RemoveTabControl(c.Controls[i]);
                    if (c.Controls[i] is cMyDataGridView)
                    {
                        ((cMyDataGridView)(c.Controls[i])).DataSource = null;
                        ((cMyDataGridView)(c.Controls[i])).Rows.Clear();
                        ((cMyDataGridView)(c.Controls[i])).Columns.Clear();
                        ((cMyDataGridView)(c.Controls[i])).Dispose();
                    }
                }
                else
                {
                    if (c.Controls[i] is cMyTextLog)
                    {
                        ((cMyTextLog)(c.Controls[i])).Clear();
                        ((cMyTextLog)(c.Controls[i])).Dispose();
                    }
                }
            }

            GC.Collect();
            
        }

        #endregion

        //处理tab页中需要点击超链打开页面的事件
        private void TextLogLinkClick(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void Tab_MouseDown(object sender, MouseEventArgs e)
        {
            //先取消选择
            DataGridView d = (DataGridView)sender;
            d.ClearSelection();

            if (d.Rows.Count == 0)
            {
                return;
            }

            DataGridView.HitTestInfo hittest = d.HitTest(e.X, e.Y);
            if (hittest.Type == DataGridViewHitTestType.Cell && e.Button == MouseButtons.Right)
            {
                d.Rows[hittest.RowIndex].Selected = true;
            }
            else
            {
                d.Rows[d.Rows.Count - 1].Selected = true;
            }
        }

        //通过用户登录获取cookie
        private string m_Cookie;
        private string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }

        //通过用户输入获取Url参数，并拼接成标准的Url
        private string m_UrlPara;
        public string UrlPara
        {
            get { return m_UrlPara; }
            set { m_UrlPara = value; }
        }

        private void GetCookie(string strCookie)
        {
            this.Cookie = strCookie;
        }

        private void GetUrlPara(string Url)
        {
            this.UrlPara = Url;
        }

        private void rmenuSaveLog_Click(object sender, EventArgs e)
        {
            string FileName = "";

            this.saveFileDialog1.OverwritePrompt = true;
            this.saveFileDialog1.Title = rm.GetString("Info60");
            saveFileDialog1.InitialDirectory = Program.getPrjPath();
            saveFileDialog1.Filter = "Text Files(*.txt)|*.txt|All Files(*.*)|*.*";
            saveFileDialog1.FileName = this.tabControl1.SelectedTab.Tag.ToString() + ".txt";

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileName = this.saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            Application.DoEvents();

            RichTextBox tmp = (RichTextBox)this.tabControl1.SelectedTab.Controls[0].Controls[1].Controls[1];

            tmp.SaveFile(FileName, RichTextBoxStreamType.TextTextOleObjs);

            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("LogSave"), rm.GetString("Task") + "：" + this.tabControl1.SelectedTab.Text + " " + rm.GetString("LogSaveSuccess")));

        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {

            if (this.dataTask.SelectedRows.Count == 0)
            {
                this.rmmenuStartTask.Enabled = false;
                this.rmmenuStopTask.Enabled = false;
                this.rmenuOverTask.Enabled = false;
                //this.rmmenuRestartTask.Enabled = false;
                this.rmmenuDelTask.Enabled = false;
                this.rmenuBrowserData.Enabled = false;
                this.rmenuEditData.Enabled = false;

                return;
            }

            cGlobalParas.TaskState tState = (cGlobalParas.TaskState)this.dataTask.SelectedCells[2].Value;

            if (int.Parse(dataTask.SelectedCells[8].Value.ToString()) > 0)
            {
                this.rmenuBrowserData.Enabled = true;
                this.rmenuEditData.Enabled = true;
            }
            else
            {
                this.rmenuBrowserData.Enabled = false;
                this.rmenuEditData.Enabled = false;
            }

            switch (tState)
            {
                case cGlobalParas.TaskState.Started:

                    this.rmmenuStartTask.Enabled = false;
                    this.rmmenuStopTask.Enabled = true;
                    this.rmmenuDelTask.Enabled = false ;
                    this.rmenuOverTask.Enabled = false;

                    break;
                case cGlobalParas.TaskState.Failed:

                    this.rmmenuStartTask.Enabled = false;
                    this.rmmenuStopTask.Enabled = false;
                    this.rmmenuDelTask.Enabled = true;
                    this.rmenuBrowserData.Enabled = false;
                    this.rmenuOverTask.Enabled = false;
                    break;
                case cGlobalParas.TaskState.Running:

                    this.rmmenuStartTask.Enabled = false;
                    this.rmmenuStopTask.Enabled = true;
                    this.rmmenuDelTask.Enabled = false ;
                    this.rmenuOverTask.Enabled = false;
                    break;
                default:

                    this.rmmenuStartTask.Enabled = true;
                    this.rmmenuStopTask.Enabled = false;
                    this.rmmenuDelTask.Enabled = true;
                    this.rmenuOverTask.Enabled = true ;
                    break;
            }


                   

        }

        private void rmmenuStartTask_Click(object sender, EventArgs e)
        {
            Start();
        }

        public void Start()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            //if (this.dataTask.SelectedCells[3].Value.ToString() == "nodRunning")
            //{
            //    StartMultiTask();
            //}
            //else if (this.dataTask.SelectedCells[3].Value.ToString().Substring(0, 1) == "C" || this.dataTask.SelectedCells[3].Value.ToString() == "nodTaskClass")
            //{
            //    StartMultiTask();
            //}
            //else if (this.dataTask.SelectedCells[3].Value.ToString() == "nodPublish")
            //{
            //    StartMultiPublish();
            //}
            StartMultiTask();


        }

        public void rmmenuStopTask_Click(object sender, EventArgs e)
        {
            Stop();
        }

        public void Stop()
        {
          
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            StopMultiTask();
            
            
        }

        private void rmenuBrowserData_Click(object sender, EventArgs e)
        {
            BrowserMultiData();
        }

        private void rmenuEditData_Click(object sender, EventArgs e)
        {
            //OpenData();
            EditData();
        }

        /// <summary>
        /// 打开已经采集完成的数据
        /// </summary>
        private void OpenCompletedData()
        {
            if (this.dataTask.Rows.Count == 0)
                return;

            Int64 TaskID = Int64.Parse(this.dataTask.SelectedRows[0].Cells[1].Value.ToString());
            string TaskName = this.dataTask.SelectedRows[0].Cells[4].Value.ToString();
            string dFile = "";
            DataTable tmp = new DataTable();
            oTaskComplete tc = new oTaskComplete(Program.getPrjPath());
            eTaskCompleted ec= tc.LoadSingleTask(TaskID);
            dFile = ec.TempFile;
            tc = null;

            if (e_OpenDataEvent != null)
            {
                e_OpenDataEvent(this, new OpenDataEvent(cGlobalParas.DatabaseType.SoukeyData, dFile, "",TaskName));
            }
        }

        private void OpenRunningData()
        {
            if (this.dataTask.Rows.Count == 0)
                return;

            Int64 TaskID = Int64.Parse(this.dataTask.SelectedRows[0].Cells[1].Value.ToString());
            string TaskName = this.dataTask.SelectedRows[0].Cells[4].Value.ToString();
            oTaskRun tr = new oTaskRun(Program.getPrjPath());
            eTaskRun er= tr.LoadSingleTask(TaskID);
            string dFile = er.TempFile;
            tr = null;

            if (e_OpenDataEvent != null)
            {
                e_OpenDataEvent(this, new OpenDataEvent(cGlobalParas.DatabaseType.SoukeyData, dFile, "", TaskName));
            }
        }

        private void rmmenuEditTask_Click(object sender, EventArgs e)
        {
            switch (this.dataTask.SelectedCells[3].Value.ToString())
            {
                case "nodSnap":
                    break;
                case "nodRunning":

                    break;
                case "nodPublish":
                    break;
                case "nodComplete":

                    break;
                case "nodRadarRule":
                    //EditRadar();
                    break;
                case "nodTaskPlan":

                    break;
             
         
                default:
                    break;
            }
        }

        private void rmmenuRenameTask_Click(object sender, EventArgs e)
        {
            this.dataTask.CurrentCell = this.dataTask[4, this.dataTask.CurrentCell.RowIndex];

            this.dataTask.BeginEdit(true);
        }

        private void rmmenuDelTask_Click(object sender, EventArgs e)
        {
            this.dataTask.Focus();
            SendKeys.Send("{Del}");
        }

        private void rmenuAddPlan_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("NewPlan", null));
        }

        private void contextMenuStrip4_Opening(object sender, CancelEventArgs e)
        {

            //加载发布规则和插件规则的菜单
            //oPublishTask cP = new oPublishTask(Program.getPrjPath());
            //IEnumerable< ePublishTask> eps= cP.LoadPublishData();

            //this.rMenuPublishDataByRule.DropDownItems.Clear();
            //foreach(ePublishTask ep in eps)
            //{
            //    this.rMenuPublishDataByRule.DropDownItems.Add(ep.pName, null, e_PublishByRuleEvent);
            //}
            //cP = null;

            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion==cGlobalParas.VersionType.Program ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise )
            {
                //cPlugin p = new cPlugin(Program.getPrjPath());
                //int count = p.GetCount();
                //this.rMenuPublishDataByPlugin.DropDownItems.Clear();
                //for (int i = 0; i < count; i++)
                //{
                //    if (p.GetPluginType(i) == cGlobalParas.PluginsType.PublishData)
                //    {
                //        this.rMenuPublishDataByPlugin.DropDownItems.Add(p.GetPluginName(i), null, e_PublishByPluginEvent);
                //    }

                //}
                //p = null;
            }
            else
            {
                //this.rMenuPublishDataByPlugin.Visible = false;
            }

            DataGridView tmp = (DataGridView)this.tabControl1.SelectedTab.Controls[0].Controls[0].Controls[0];
            string conName = this.tabControl1.SelectedTab.Controls[0].Controls[0].Controls[0].Name;
            Int64 TaskID = Int64.Parse(conName.Substring(4, conName.Length - 4));
           
                cPublish pt = m_PublishControl.PublishManage.FindTask(TaskID);
                if (pt != null)
                {
                    if (pt.pThreadState == cGlobalParas.PublishThreadState.Running)
                    {
                        this.rMenuStopTask.Enabled = true ;
                        this.rMenuStartTask.Enabled = false;
                        this.rMenuCloseTabPage.Enabled = false ;
                    }
                    else if (pt.pThreadState == cGlobalParas.PublishThreadState.Stopped || pt.pThreadState == cGlobalParas.PublishThreadState.UnStart)
                    {
                        this.rMenuStopTask.Enabled = false;
                        this.rMenuStartTask.Enabled = true;
                        this.rMenuCloseTabPage.Enabled = true;
                    }
                    else
                    {
                        this.rMenuStopTask.Enabled = false;
                        this.rMenuStartTask.Enabled = false ;
                        this.rMenuCloseTabPage.Enabled = true  ;
                    }
              
                }
                else
                {
                    cGatherTask t = m_GatherControl.TaskManage.FindTask(TaskID);
                    if (t != null)
                    {
                        if (t.State == cGlobalParas.TaskState.Running || t.State == cGlobalParas.TaskState.Started)
                        {
                            this.rMenuStartTask.Enabled = false;
                            this.rMenuStopTask.Enabled = true;
                            this.rMenuCloseTabPage.Enabled = false;
                        }
                        else if (t.State == cGlobalParas.TaskState.Completed)
                        {
                            this.rMenuStopTask.Enabled = false;
                            this.rMenuStartTask.Enabled = false;
                            this.rMenuCloseTabPage.Enabled = true;
                        }
                        else
                        {
                            this.rMenuStopTask.Enabled = false;
                            this.rMenuStartTask.Enabled = true;
                            this.rMenuCloseTabPage.Enabled = true;
                        }
                    }
                    else
                    {
                        this.rMenuStopTask.Enabled = false;
                        this.rMenuStartTask.Enabled = false;
                        this.rMenuCloseTabPage.Enabled = true;
                    }
                }
            

            if (tmp.Rows.Count == 0)
            {
                this.rMenuExportTxt.Enabled = false;
                this.rMenuExportExcel.Enabled = false;
                this.rMenuExportCSV.Enabled = false;
                this.rmenuPublishData.Enabled = false ;
                this.rMenuExportWord.Enabled = false;
            }
            else
            {
                this.rMenuExportCSV.Enabled = true;
                this.rMenuExportTxt.Enabled = true;
                this.rMenuExportExcel.Enabled = true;
                this.rmenuPublishData.Enabled = true;
                this.rMenuExportWord.Enabled = true ;
            }

            tmp = null;

            //if (Program.SominerVersion == cGlobalParas.VersionType.Trial)
            //{
            //    this.rMenuExportTxt.Enabled = false;
            //    this.rMenuExportExcel.Enabled = false;
            //    this.rMenuExportCSV.Enabled = false;
            //    this.rMenuExportWord.Enabled = false;
            //    this.rmenuPublishData.Enabled = false;
            //    this.rMenuPublishDataByPlugin.Enabled = false;
            //    this.rMenuPublishDataByRule.Enabled = false;
            //}
        }

        private void rMenuExportTxt_Click(object sender, EventArgs e)
        {
            //首先判断当前需要导出数据的Tab及datagridview
            ExportTxt();
        }

        private void rMenuExportExcel_Click(object sender, EventArgs e)
        {
            ExportExcel();
        }

        private void rMenuExportCSV_Click(object sender, EventArgs e)
        {
            ExportCSV();
        }

        private void rMenuCloseTabPage_Click(object sender, EventArgs e)
        {
            string TaskID = this.tabControl1.SelectedTab.Name.Substring(4, this.tabControl1.SelectedTab.Name.Length - 4);
            for (int index = 0; index < m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count ; index++)
            {
                if (m_GatherControl.TaskManage.TaskListControl.RunningTaskList[index].TaskID == long.Parse (TaskID))
                {
                    MessageBox.Show(rm.GetString("Info34"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            //for (int i = 0; i < this.tabControl1.SelectedTab.Controls.Count; i++)
            //{
            //    if (this.tabControl1.SelectedTab.Controls[i] != null)
            //        this.tabControl1.SelectedTab.Controls[i].Dispose();
            //}

            int j = 0;
            for (int i = 0; i < this.tabControl1.TabPages.Count;i++ )
            {
                if (this.tabControl1.TabPages[i].Name==this.tabControl1.SelectedTab.Name )
                {
                    j = i;
                    break;
                }
            }

            RemoveTab(j);
            this.tabControl1.SelectedTab.Dispose();

            //this.tabControl1.TabPages.Remove(this.tabControl1.SelectedTab);

            //判断是否已经没有选项卡，如果没有，则隐藏tab显示
            if (this.tabControl1.TabPages.Count ==0)
                this.splitContainer2.Panel2Collapsed = true ;
        }

        //设置加载错误的gridrows的现实样式
        private void SetRowErrStyle()
        {
            this.m_RowStyleErr = new DataGridViewCellStyle();
            this.m_RowStyleErr.Font = new Font(DefaultFont, FontStyle.Italic);
            this.m_RowStyleErr.ForeColor = Color.Red;
        }

         ///根据菜单选择及点击的数据内容，自动控制工具栏的按钮状态
        ///因为DataGridView支持多选，所以可能存在多种按钮状态的情况
        ///针对这种情况，系统按照最后选择的内容进行按钮状态设置

        private void SetFormToolbarState()
        {
            if (e_SetToolbarState != null && this.dataTask.SelectedRows.Count !=0)
            {
                e_SetToolbarState(this, new SetToolbarStateEvent(this.dataTask.Rows.Count, (cGlobalParas.TaskState)this.dataTask.SelectedCells[2].Value));
            }
                 
        }

        //设置显示任务数据Datalistview的列表头
        private void ShowRunTask()
        {
            this.Text = "正在运行";
            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {
            }

            #region 此部分为固定显示 任务类型的任务都必须固定显示此列
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

            //用于通过判断Datagridview的数据就可知道当前树形结构选择的节点
            //用于控制(更新)界面显示状态
            DataGridViewTextBoxColumn tTreeNode = new DataGridViewTextBoxColumn();
            tTreeNode.HeaderText = "taskClassPath";
            tTreeNode.Name = "TaskClassPath";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);

            #endregion

            DataGridViewTextBoxColumn tClass = new DataGridViewTextBoxColumn();
            tClass.HeaderText = rm.GetString("TaskClass");
            tClass.Name = "TaskClass";
            tClass.Width = 90;
            tClass.Visible = false;
            this.dataTask.Columns.Insert(4, tClass);

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = rm.GetString("GridTaskName");
            tName.Width = 150;
            this.dataTask.Columns.Insert(5, tName);

            DataGridViewTextBoxColumn curState = new DataGridViewTextBoxColumn();
            curState.HeaderText = "采集进程";
            curState.Width = 120;
            curState.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            curState.Visible = false;
            this.dataTask.Columns.Insert(6, curState);

            DataGridViewTextBoxColumn StartTimer = new DataGridViewTextBoxColumn();
            StartTimer.HeaderText = rm.GetString("StartTime");
            StartTimer.Width = 90;
            StartTimer.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(7, StartTimer);

            DataGridViewTextBoxColumn GatheredUrlCount = new DataGridViewTextBoxColumn();
            GatheredUrlCount.HeaderText = "网址数";
            GatheredUrlCount.Width = 50;
            GatheredUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            GatheredUrlCount.Visible = false;
            this.dataTask.Columns.Insert(8, GatheredUrlCount);

            DataGridViewTextBoxColumn GatheredErrUrlCount = new DataGridViewTextBoxColumn();
            GatheredErrUrlCount.HeaderText = rm.GetString("GridErrorCount");
            GatheredErrUrlCount.Width = 50;
            GatheredErrUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            GatheredErrUrlCount.Visible = false;
            this.dataTask.Columns.Insert(9, GatheredErrUrlCount);

            DataGridViewTextBoxColumn tUrlCount = new DataGridViewTextBoxColumn();
            tUrlCount.HeaderText = "数据量";
            tUrlCount.Width = 50;
            tUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tUrlCount.Visible = false;
            this.dataTask.Columns.Insert(10, tUrlCount);

            DataGridViewProgressBarColumn tPro = new DataGridViewProgressBarColumn();
            tPro.HeaderText = rm.GetString("GridProcess");
            tPro.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataTask.Columns.Insert(11, tPro);
           
        }

        private void SetDataShow()
        {
            splitContainer2.SplitterDistance = 150;
        }

        public void SetTaskShowState(Int64 TaskID, cGlobalParas.TaskState tState)
        {

            //查找当前的列表显示的任务
            //首先判断当前选中的树形节点是否是运行区的节点
            if (this.dataTask.Rows.Count > 0 && this.dataTask.Rows[0].Cells[3].Value.ToString() == "nodRunning")
            {
                for (int i = 0; i < this.dataTask.Rows.Count; i++)
                {
                    bool IsSetToolbutState = false;

                    if (this.dataTask.Rows[i].Cells[1].Value.ToString() == TaskID.ToString())
                    {
                        if (i == this.dataTask.CurrentRow.Index)
                        {
                            IsSetToolbutState = true;
                        }

                        switch (tState)
                        {
                            case cGlobalParas.TaskState.Started:

                                break;
                            case cGlobalParas.TaskState.Failed:
                                this.dataTask.Rows[i].Cells[0].Value = imageList1.Images["error"];
                                if (IsSetToolbutState == true)
                                {
                                    if (e_SetControlProperty != null)
                                    {
                                        e_SetControlProperty(this, new SetControlPropertyEvent("toolStartTask", "Enabled", "true"));
                                        //e_SetControlProperty(this, new SetControlPropertyEvent("toolRestartTask", "Enabled", "false"));
                                        e_SetControlProperty(this, new SetControlPropertyEvent("toolStopTask", "Enabled", "false"));

                                    }

                                }

                                break;
                            case cGlobalParas.TaskState.Completed:
                                this.dataTask.Rows[i].Cells[0].Value = imageList1.Images["stop"];
                                if (IsSetToolbutState == true)
                                {
                                    if (e_SetControlProperty != null)
                                    {
                                        e_SetControlProperty(this, new SetControlPropertyEvent("toolStartTask", "Enabled", "true"));
                                        //e_SetControlProperty(this, new SetControlPropertyEvent("toolRestartTask", "Enabled", "false"));
                                        e_SetControlProperty(this, new SetControlPropertyEvent("toolStopTask", "Enabled", "false"));

                                    }
                                }

                                break;
                            case cGlobalParas.TaskState.UnStart:

                                break;
                            case cGlobalParas.TaskState.Pause:

                                break;
                            default:

                                break;
                        }

                        break;
                    }
                }
            }

        }

   
     

        #region 根据选中的树形菜单信息加载数据
    
        //加载正在执行的任务，正在执行的任务记录在应用程序目录下的RunningTask.xml文件中
        public void LoadRunTask()
        {
          

            ShowRunTask();

            //开始初始化正在运行的任务
            //从m_TaskControl中读取
            //每次加载会加载正在运行、等待、停止队列中的任务
            List<cGatherTask> taskList = new List<cGatherTask>();
            taskList.AddRange(m_GatherControl.TaskManage.TaskListControl.RunningTaskList);
            taskList.AddRange(m_GatherControl.TaskManage.TaskListControl.StoppedTaskList);
            taskList.AddRange(m_GatherControl.TaskManage.TaskListControl.WaitingTaskList);
            //taskList.AddRange(m_GatherControl.TaskManage.TaskListControl.CompletedTaskList);

            for (int i = 0; i < taskList.Count; i++)
            {
                try
                {
                    int urlcount = taskList[i].UrlCount + taskList[i].UrlNaviCount ;
                    int gurlcount = taskList[i].GatheredUrlCount + taskList[i].GatheredUrlNaviCount;
                    int errurlcount = taskList[i].GatherErrUrlCount + taskList[i].GatheredErrUrlNaviCount;

                    Image ico = null;

                    switch (taskList[i].State)
                    {
                        case cGlobalParas.TaskState.Started:
                            ico = imageList1.Images["pause"];
                            break;

                        case cGlobalParas.TaskState.Stopped:
                            if ((gurlcount + errurlcount) > 0)
                            {
                                ico = imageList1.Images["pause"];
                            }
                            else
                            {
                                ico = imageList1.Images["stop"];
                            }
                            break;
                        case cGlobalParas.TaskState.UnStart:
                            ico = imageList1.Images["stop"];
                            break;
                        case cGlobalParas.TaskState.Failed:
                            ico = imageList1.Images["error"];
                            dataTask.Rows[dataTask.Rows.Count - 1].DefaultCellStyle = this.m_RowStyleErr;
                            break;
                        case cGlobalParas.TaskState.ErrStop:
                            ico = imageList1.Images["Error"];
                            break;
                        default:
                            ico = imageList1.Images["stop"];
                            break;
                    }

                    this.dataTask.Rows.Add(ico, taskList[i].TaskID, taskList[i].State, taskList[i].TaskData.TaskClassPath,
                                taskList[i].TaskData.TaskClass, taskList[i].TaskName, (int)cGlobalParas.TaskProcess.UnKnow,
                                taskList[i].StartTimer.ToString("MM-dd HH:mm"), urlcount, 
                                errurlcount, taskList[i].TaskData.GatherDataCount,
                                (gurlcount + errurlcount) * 100 / (urlcount == 0 ? 1 : urlcount));

                    //开始设置Tooltip信息
                    StringBuilder sb = new StringBuilder();
                    sb.Append("所属分类：" + taskList[i].TaskData.TaskClass + "\r\n");
                    sb.Append("执行类型：" + taskList[i].TaskData.RunType.GetDescription() + "\r\n");
                    sb.Append("采集行数：" + taskList[i].TaskData.GatherDataCount + "\r\n");
                    this.dataTask[0, dataTask.Rows.Count - 1].ToolTipText = sb.ToString();
                    //HelpTip.SetToolTip(this.dataTask.Rows[dataTask.Rows.Count - 1].Cells[1], @"输入任务名称，任务名称可以是"


                }
                catch (System.Exception ex)
                {
                    //捕获错误，不做处理，让信息继续加载
                    if (e_ExportLog != null)
                        e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error,cGlobalParas.LogClass.Task ,System.DateTime.Now.ToString (), ex.Message));

                }
            }

            this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

            this.dataTask.ClearSelection();

            taskList = null;

        }

      
  

        //此部分的数据是根据当前已经完成的导出任务
        //实时产生的数据
        public void LoadExportDataTask(TreeNode eNode)
        {
            //this.myListData.Items.Clear();
        }

        //加载任务，可以加载任务信息和正在运行的任务
        //注意，如果加载的是运行区的任务，则不可以进行任务的升级操作
        //当前判断的条件是：如果是编辑任务则判断任务版本是否符合当前版本
        //约束，如果不是，则进行任务升级。如果是浏览任务，则无论是否满足
        //版本约束，都不进行升级操作

        //此方法仅限于浏览，浏览只能通过原有的界面进行
        //private void LoadTaskInfo(string FilePath, string FileName, cGlobalParas.FormState fState)
        //{

        //    frmTask ft = null;

        //    string TClass = this.TreeNode.Text;
        //    string tClassPath = this.TreeNode.Tag.ToString();

        //    try
        //    {
        //        ft = new frmTask();
        //        ft.EditTask(tClassPath, TClass, FilePath, FileName);
        //        ft.FormState = fState;
        //        ft.rTClass = refreshNode;
        //        ft.Show();
        //    }
        //    catch (NetMinerException)
        //    {
        //        if (fState == cGlobalParas.FormState.Browser)
        //        {
        //            MessageBox.Show(rm.GetString("Info19"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        }

        //        if (MessageBox.Show(rm.GetString("Quaere4"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
        //        {
        //        }
        //        else
        //        {
        //            frmUpgradeTask fu = new frmUpgradeTask(FilePath + "\\" + FileName);
        //            fu.ShowDialog();
        //            fu.Dispose();
        //            fu = null;
        //            return;

        //        }
        //    }
        //    finally
        //    {
        //        ft = null;
        //    }


        //}


        //private void SelectedEditTask(cGlobalParas.NewTaskType mType)
        //{
        //    if (mType == cGlobalParas.NewTaskType.Wizard)
        //    {
        //        EditTaskByWizard();
        //    }
        //    else if (mType == cGlobalParas.NewTaskType.Normal)
        //    {
        //        EditTaskByNormal();
        //    }
        //    else if (mType == cGlobalParas.NewTaskType.Cancel)
        //    {
        //        return;
        //    }

        //}

        private void refreshNode(string TClass)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("refreshNode", new object[] { TClass }));
        }

        #endregion

        #region 删除的一些操作

        //public bool DelPlanLog()
        //{
        //    if (this.dataTask.SelectedRows.Count == 0)
        //    {
        //        return false;
        //    }

        //    if (MessageBox.Show(rm.GetString("Quaere7"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
        //    {
        //        return false;
        //    }

        //    cPlanRunLog rLog = new cPlanRunLog();
        //    rLog.DelLog();
        //    rLog = null;

        //    //清空显示
        //    try
        //    {
        //        this.dataTask.Rows.Clear();
        //    }
        //    catch (System.Exception)
        //    {
        //    }

        //    return true;
        //}

        public void Del()
        {
            this.dataTask.Focus();
            SendKeys.Send("{Del}");
        }

        /// <summary>
        /// 删除正在运行的任务
        /// </summary>
        private bool DelRunTask()
        {

            if (this.dataTask.SelectedRows.Count == 0)
                return false;

            if (this.dataTask.SelectedRows.Count == 1)
            {
                if (MessageBox.Show(rm.GetString("Info29") + this.dataTask.SelectedCells[4].Value.ToString() + "\r\n" +
                    rm.GetString("Quaere9"), rm.GetString("MessageboxQuaere"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }
            else
            {
                if (MessageBox.Show(rm.GetString("Quaere8"), rm.GetString("MessageboxQuaere"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }

            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {

                cGatherTask t = m_GatherControl.TaskManage.FindTask(Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString()));

                m_GatherControl.Remove(t);

                Int64 TaskID = Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString());
                string TaskName = this.dataTask.SelectedRows[index].Cells[5].Value.ToString();

                t = null;

                //删除taskrun节点
                oTaskRun tr = new oTaskRun(Program.getPrjPath());
                tr.DelTask(TaskID);
                tr = null;

                ////删除已经加载到采集任务控制器中的任务
                //m_GatherControl.TaskManage.TaskListControl.DelTask(t);

                //删除run中的任务实例文件
                string FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".rst";
                System.IO.File.Delete(FileName);

                //删除run中的任务实例排重库文件
                FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".db";
                System.IO.File.Delete(FileName);

                //删除run中的采集数据排重文件
                FileName = Program.getPrjPath() + "Tasks\\run\\" + "data" + TaskID + ".db";
                System.IO.File.Delete(FileName);

                //删除run中的任务实例文件
                FileName = Program.getPrjPath() + "data\\" + TaskName + "-" + TaskID + ".xml";
                System.IO.File.Delete(FileName);

                tr = null;

            }

            return true;

            //删除Datagridview中选中的数据

            //while(this.dataTask.SelectedRows.Count>0)
            //{
            //    this.dataTask.Rows.Remove(this.dataTask.SelectedRows[0]);
            //}


        }
      

        #endregion

        public void BrowserMultiData()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {
                if ((cGlobalParas.TaskState)this.dataTask.SelectedRows[index].Cells[2].Value == cGlobalParas.TaskState.Failed)
                {
                    continue;
                }
                else
                {
                    BrowserData(Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString()), this.dataTask.SelectedRows[index].Cells[5].Value.ToString());
                }
            }
        }

        /// <summary>
        /// 加载已经运行的采集任务，并打开已经采集的数据
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="TaskName"></param>
        /// <returns></returns>
        private int BrowserData(Int64 TaskID, string TaskName)
        {
            int rowCount = 0;
            if (this.dataTask.Rows.Count != 0)
            {
                //Int64 TaskID = Int64.Parse (this.dataTask.SelectedCells[1].Value.ToString ());
                DataTable tmp = new DataTable();
                string dFile = "";

                //判断是浏览的那些数据：正在运行还是采集完成

            
                    oTaskRun tr = new oTaskRun(Program.getPrjPath());
                    eTaskRun er= tr.LoadSingleTask(TaskID);

                    if (er != null)
                        dFile = er.TempFile;
                    else
                        //如果此时发现为空，表示任务执行完成已经转入了完成列表
                        //但界面还未刷新，导致这个错误
                        dFile = "";

                    tr = null;
             

                string conName = "sCon" + TaskID;
                string pageName = "page" + TaskID;

                AddTab(TaskID, TaskName);

                if (File.Exists(dFile))
                {
                    try
                    {
                        tmp.ReadXml(dFile);
                    }
                    catch (System.Exception)
                    {
                        //有可能在保存数据时发生了错误，因此需要忽略错误，直接进行,但需要通过系统信息显示错误
                        if (e_ExportLog != null)
                            e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error ,cGlobalParas.LogClass.Task , DateTime.Now.ToString () , TaskName + rm.GetString("Info43")));

                    }
                }

                ((cMyDataGridView)(this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0])).Clear();
                ((cMyDataGridView)(this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0])).gData = tmp;

                if (tmp.Rows.Count == 0)
                {
                    if (e_SetControlProperty != null)
                        e_SetControlProperty(this, new SetControlPropertyEvent("menuExportData", "Enabled", "false"));
                }
                else
                {
                    if (e_SetControlProperty != null)
                        e_SetControlProperty(this, new SetControlPropertyEvent("menuExportData", "Enabled", "true"));
                }
                rowCount = tmp.Rows.Count;
                tmp = null;

                this.tabControl1.TabPages[pageName].Text = TaskName + "[" + rowCount + "]";
            }

            return rowCount;
        }

        

        //定义一个代理用于修改任务的名称
        private delegate bool delegateRenameTaskName(string TaskClass, string OldName, string NewName);
        private bool RenameTaskName(string taskClassPath, string OldName, string NewName)
        {
           

            cTaskManage mTask = new cTaskManage(Program.getPrjPath());

            try
            {
                mTask.RenameTask(taskClassPath, OldName, NewName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info64") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                mTask = null;
            }

            return true;
        }

        //定义一个代理用于修改计划的名称
        private delegate bool delegateRenamePlanName(string OldName, string NewName);
        private bool RenamePlanName(string OldName, string NewName)
        {
            oPlans p = new oPlans(Program.getPrjPath());

            try
            {
                p.RenamePlanName(OldName, NewName);
            }
            catch (System.Exception ex)
            {
                p = null;
                MessageBox.Show(rm.GetString("Info65") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            p = null;

            return true;
        }


        public void EditData()
        {
            if (this.dataTask.Rows.Count == 0)
                return;

            Int64 TaskID = Int64.Parse(this.dataTask.SelectedRows[0].Cells[1].Value.ToString());
            string TaskName = this.dataTask.SelectedRows[0].Cells[4].Value.ToString();
            string dFile = "";
            DataTable tmp = new DataTable();

          
            //cTaskRun tr = new cTaskRun(Program.getPrjPath());
            //tr.LoadSingleTask(TaskID);
            //dFile = tr.GetTempFile(0);
            //tr = null;

            OpenRunningData();
            return;
            
          

            if (File.Exists(dFile))
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo("SoukeyDataPublish.exe");
			        startInfo.WindowStyle = ProcessWindowStyle.Maximized ;;
        			
                    //传入第三个参数，如果是soukeydata数据类型，则不是用，但为了保证系统不出错，加入了//
			        startInfo.Arguments= ((int)cGlobalParas.DatabaseType.SoukeyData).ToString () + " " + dFile + " " + "//";
        			
			        Process.Start(startInfo);


                    //System.Diagnostics.Process.Start("SoukeyDataPublish.exe",
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            {
                MessageBox.Show(rm.GetString("Info118"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void ExportGatherLog(string pageName, string conName, cGlobalParas.LogType lType, string lText)
        {

            lText = System.DateTime.Now.ToString() + " " + lText;

            ((cMyTextLog)this.tabControl1.TabPages[pageName].Controls[conName].Controls[1].Controls[1]).AppendText(lType, lText);


            int i = ((cMyTextLog)this.tabControl1.TabPages[pageName].Controls[conName].Controls[1].Controls[1]).Lines.Length;
            if (i > this.m_MaxLogNumber)
            {
                ((cMyTextLog)this.tabControl1.TabPages[pageName].Controls[conName].Controls[1].Controls[1]).Clear();
            }
            
        }

        public void ExportPublishLog(string pageName, string conName, cGlobalParas.LogType lType, string lText)
        {
            lText = System.DateTime.Now.ToString() + " " + lText;

            try
            {
                ((cMyTextLog)this.tabControl1.TabPages[pageName].Controls[conName].Controls[1].Controls[1]).AppendText(lType, lText);
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error, cGlobalParas.LogClass.Task, DateTime.Now.ToString(),"采集页面已经关闭，导致调用输出日志失败！"));
                return;

            }

        

            int i = ((cMyTextLog)this.tabControl1.TabPages[pageName].Controls[conName].Controls[1].Controls[1]).Lines.Length;
            if (i > this.m_MaxLogNumber)
            {
                ((cMyTextLog)this.tabControl1.TabPages[pageName].Controls[conName].Controls[1].Controls[1]).Clear();
            }
        }

        private void rmmenuNewTask_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("NewTask", null));
        }

        private void StopMultiTask()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {
                if ((cGlobalParas.TaskState)this.dataTask.SelectedRows[index].Cells[2].Value == cGlobalParas.TaskState.Failed)
                {
                    continue;
                }
                else
                {
                    StopTask(Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString()));
                }
            }
        }

        private void StopMultiPublish()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {
                if ((cGlobalParas.TaskState)this.dataTask.SelectedRows[index].Cells[2].Value == cGlobalParas.TaskState.Failed)
                {
                    continue;
                }
                else
                {
                    StopPublish(Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString()), this.dataTask.SelectedRows[index].Cells[4].Value.ToString());
                }
            }
        }

        private void StopPublish(Int64 TaskID,string TaskName)
        {
            cPublish pt = null;

       
            //执行正在执行的任务
            pt = m_PublishControl.PublishManage.FindTask(TaskID);

            if (pt == null)
                return;

            //停止此任务
            m_PublishControl.StopPublish(pt);

            //任务启动成功显示消息
            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent("发布已停止！",TaskName));

            //}
        }

        #region 任务管理 启动 停止
        //启动采集任务
        /// <summary>
        /// 将采集任务增加至运行队列，传入的是相对路径
        /// </summary>
        /// <param name="tClassName"></param>
        /// <param name="tClassPath"></param>
        /// <param name="tName"></param>
        /// <returns></returns>
        private cGatherTask AddRunTask(string tClassName, string tClassPath, string tName)
        {

            //将选择的任务添加到运行区
            //首先判断此任务是否已经添加到运行区,
            //如果已经添加到运行区则需要询问是否再起一个运行实例
            bool IsExist = false;
            //cGlobalParas.TaskType tType = cGlobalParas.TaskType.HtmlByUrl;

            //开始初始化正在运行的任务
            oTaskRun tr;
            try
            { 
                tr = new oTaskRun(Program.getPrjPath());
            }
            catch(System.IO.IOException ex)
            {
                MessageBox.Show("启动任务失败，可能由于启动任务数量较多引发冲突，请稍候重试！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            catch(System.Exception ex1)
            {
                MessageBox.Show("启动任务发生错误：" + ex1.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            tr.LoadTaskRunData();
            IsExist = tr.isExist(tClassName, tName);

            if (IsExist == true)
            {
                if (MyMessageBox(rm.GetString("Quaere12"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return null;
                }
            }

            
            cTaskData tData = new cTaskData();

            string tPath = "";
            tPath = Program.getPrjPath() + tClassPath;


            string tFileName = tName + ".smt";
            Int64 NewID;

            //获取最大的执行ID
            try
            {
                NewID = tr.InsertTaskRun(Program.getPrjPath (), tClassName, tPath, tFileName);


                eTaskRun er= tr.LoadSingleTask(NewID);

                tData = new cTaskData();
                tData.PublishType = er.PublishType;
                tData.TaskID = er.TaskID;
                tData.TaskName = er.TaskName;
                tData.TaskClass = er.TaskClass;
                tData.TaskType = er.TaskType;
                tData.RunType = er.TaskRunType;
                tData.TempDataFile = er.TempFile;
                tData.TaskState = er.TaskState;
                tData.UrlCount = er.UrlCount;
                tData.UrlNaviCount = er.UrlNaviCount;
                tData.ThreadCount = er.ThreadCount;
                tData.GatheredUrlCount = er.GatheredUrlCount;
                tData.GatherErrUrlCount = er.ErrUrlCount;
                tData.GatherDataCount = er.RowsCount;
                tData.StartTimer = er.StartDateTime;
                tData.EndTime=er.EndDateTime;
                tData.Process = er.Process;

                //添加任务到运行区
                m_GatherControl.AddGatherTask(tData);

                tData = null;

                //任务添加到运行区后,需要再添加到任务执行列表中
                tr = null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return m_GatherControl.TaskManage.FindTask(NewID);

        }

        public void StopAllTask()
        {
            //弹出试用版本的窗口
            frmTrialInfo f = new frmTrialInfo();
            f.Show();

            m_GatherControl.Stop();
        }

        private void StopTask(Int64 TaskID)
        {
            cGatherTask t = null;

            ///2013-1-14 修改，去掉左侧树形菜单选择的判断，删除
            ///菜单只要可以用，就可以删除，为了兼容其他的删除操作
            //判断当前选择的树节点
            //if (this.dataTask.SelectedCells[3].Value .ToString () == "nodRunning")
            //{
                //执行正在执行的任务
                t = m_GatherControl.TaskManage.FindTask(TaskID);

                if (t == null)
                    return;

                //停止此任务
                m_GatherControl.Stop(t);

                //任务启动成功显示消息
                if (e_ShowLogInfo !=null)
                    e_ShowLogInfo (this,new ShowInfoEvent (rm.GetString("TaskStoped"),t.TaskName));

            //}
        }

        #endregion

        public void ExportTxt()
        {
            if (IsExportData() == true)
            {
                MessageBox.Show(rm.GetString("Info35"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string FileName;

            this.saveFileDialog1.OverwritePrompt = true;
            this.saveFileDialog1.Title = rm.GetString("Info36");
            saveFileDialog1.InitialDirectory = Program.getPrjPath();
            saveFileDialog1.Filter = "Text Files(*.txt)|*.txt|All Files(*.*)|*.*";
            saveFileDialog1.FileName = this.tabControl1.SelectedTab.Tag.ToString() + ".txt";

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

        public void ExportExcel()
        {
            if (IsExportData() == true)
            {
                MessageBox.Show(rm.GetString("Info35"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string FileName;

            this.saveFileDialog1.OverwritePrompt = true;
            this.saveFileDialog1.Title = rm.GetString("Info36");
            saveFileDialog1.InitialDirectory = Program.getPrjPath();
            saveFileDialog1.Filter = "Excel Files(*.xlsx)|*.xlsx|All Files(*.*)|*.*";
            saveFileDialog1.FileName = this.tabControl1.SelectedTab.Tag.ToString() + ".xlsx";

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileName = this.saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            Application.DoEvents();

            ExportData(FileName, cGlobalParas.PublishType.PublishExcel);

        }

        public void ExportWord()
        {
            if (IsExportData() == true)
            {
                MessageBox.Show(rm.GetString("Info35"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string FileName;

            this.saveFileDialog1.OverwritePrompt = true;
            this.saveFileDialog1.Title = "请输入导出word的文件名";
            saveFileDialog1.InitialDirectory = Program.getPrjPath();
            saveFileDialog1.Filter = "Word Files(*.docx)|*.docx|All Files(*.*)|*.*";
            saveFileDialog1.FileName = this.tabControl1.SelectedTab.Tag.ToString() + ".docx";

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

        public void ExportCSV()
        {
            if (IsExportData() == true)
            {
                MessageBox.Show(rm.GetString("Info35"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string FileName;

            this.saveFileDialog1.OverwritePrompt = true;
            this.saveFileDialog1.Title = rm.GetString("Info36");
            saveFileDialog1.InitialDirectory = Program.getPrjPath();
            saveFileDialog1.Filter = "CSV Files(*.csv)|*.csv|All Files(*.*)|*.*";
            saveFileDialog1.FileName = this.tabControl1.SelectedTab.Tag.ToString() + ".csv";

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

        /// <summary>
        /// 代理用于更新当前界面的进度条
        /// </summary>
        /// <param name="totalMessages"></param>
        /// <param name="messagesSoFar"></param>
        /// <param name="statusDone"></param>
        delegate void ShowProgressDelegate(int totalMessages, int messagesSoFar,string message, bool statusDone);
        private void ExportData(string FileName, cGlobalParas.PublishType pType)
        {
            cExport eTxt = new cExport(Program.getPrjPath ());
            string TaskID = this.tabControl1.SelectedTab.Name.Replace("page", "");
            string tName = this.tabControl1.SelectedTab.Tag.ToString();
            DataGridView tmp = (DataGridView)this.tabControl1.SelectedTab.Controls[0].Controls[0].Controls[0];
            ProgressBar pBar = (ProgressBar)this.tabControl1.SelectedTab.Controls[0].Controls[1].Controls[0];
            pBar.Visible = true;

            //将数据拷贝出来
            DataTable d = ((DataTable)tmp.DataSource).Copy();

            m_ExportPageName = this.tabControl1.SelectedTab.Name;

            ExportGatherLog(this.tabControl1.SelectedTab.Name, this.tabControl1.SelectedTab.Controls[0].Name, cGlobalParas.LogType.Info, rm.GetString ("Info237") + "\n");
            ExportGatherLog(this.tabControl1.SelectedTab.Name, this.tabControl1.SelectedTab.Controls[0].Name, cGlobalParas.LogType.Info, FileName + "\n");

            //定义一个后台线程用于导出数据操作
            ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);
            cExport eExcel = new cExport(this, showProgress, pType, FileName, d);
            string[] ps = new string[] { TaskID.ToString() };
            Thread t = new Thread(new ParameterizedThreadStart(eExcel.RunProcess));
            t.IsBackground = true;
            t.Start(ps);

            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("Info38"), rm.GetString("Info37")));

            tName = null;
        }

        private void ShowProgress(int total, int messagesSoFar,string message, bool done)
        {
            ProgressBar pBar = (ProgressBar)this.tabControl1.TabPages[this.m_ExportPageName].Controls[0].Controls[1].Controls[0];

            if (pBar.Maximum != total)
            {
                pBar.Maximum = total;
            }

            pBar.Value = messagesSoFar;

            if (done)
            {
                if (e_ShowLogInfo != null)
                    e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("Info39"), rm.GetString("Info37")));

                ExportGatherLog(this.tabControl1.TabPages[this.m_ExportPageName].Name,
                    this.tabControl1.TabPages[this.m_ExportPageName].Controls[0].Name,
                    cGlobalParas.LogType.Info, rm.GetString("Info238") + "\n");

                pBar.Visible = false;
            }
            else
            {
                if (message!="")
                    ExportGatherLog(this.tabControl1.TabPages[this.m_ExportPageName].Name,
                        this.tabControl1.TabPages[this.m_ExportPageName].Controls[0].Name,
                        cGlobalParas.LogType.Warning, message + "\n");
            }
        }

        //手动导出数据同一时间只能导出一个任务，不能进行多任务的数据导出
        private bool IsExportData()
        {
            if (this.m_IsExportData == false )
            {
                return false;
            }
            return true;
        }

        //初始化采集任务
        public void IniData()
        {
            SetRowErrStyle();


            this.e_PublishByPluginEvent += this.publishByPluginEvent;
            this.e_PublishByRuleEvent += this.publishByRuleEvent;


            //初始化一个采集任务的控制器,采集任务由此控制器来负责采集任务
            //管理
            NetMiner.Base.cHashTree tmpUrls = null;
            m_GatherControl = new cGatherControl(Program.getPrjPath(),false,ref tmpUrls);

            //采集控制器事件绑定,绑定后,页面可以响应采集任务的相关事件
            m_GatherControl.TaskManage.TaskCompleted += tManage_Completed;
            m_GatherControl.TaskManage.TaskStarted += tManage_TaskStart;
            m_GatherControl.TaskManage.TaskInitialized += tManage_TaskInitialized;
            m_GatherControl.TaskManage.TaskStateChanged += tManage_TaskStateChanged;
            m_GatherControl.TaskManage.TaskStopped += tManage_TaskStop;
            m_GatherControl.TaskManage.TaskError += tManage_TaskError;
            m_GatherControl.TaskManage.TaskFailed += tManage_TaskFailed;
            m_GatherControl.TaskManage.TaskAborted += tManage_TaskAbort;
            m_GatherControl.TaskManage.Log += tManage_Log;
            m_GatherControl.TaskManage.GData += tManage_GData;
            m_GatherControl.TaskManage.GUrlCount += tManage_GUrlCount;
            m_GatherControl.TaskManage.RunTask += this.On_RunSoukeyTask;
            m_GatherControl.TaskManage.RunTaskLog += this.On_RunTaskLog;

            m_GatherControl.TaskManage.ShowLogInfo += this.on_ShowInfo;

            m_GatherControl.Completed += m_Gather_Completed;

            

            //加载运行区的数据,运行区的数据主要是根据taskrun.xml(默认在Tasks\\TaskRun.xml)文件中
            //的内容进行加载,
            cTaskDataList gList = new cTaskDataList();

            //根据加载的运行区的任务信息,开始初始化采集任务
            try
            {
                gList.LoadTaskRunData(Program.getPrjPath());

                //在此增加采集运行中的任务
                bool IsAddRTaskSucceed = m_GatherControl.AddGatherTask(gList);

                if (IsAddRTaskSucceed == false)
                    MessageBox.Show(rm.GetString("Error23"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Error15") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            //开始加载正在导出的任务信息
            m_PublishControl = new cPublishControl(Program.getPrjPath());

            //注册发布任务的事件
            m_PublishControl.PublishManage.PublishCompleted += this.Publish_Complete;
            m_PublishControl.PublishManage.PublishError += this.Publish_Error;
            m_PublishControl.PublishManage.PublishFailed += this.Publish_Failed;
            m_PublishControl.PublishManage.PublishStarted += this.Publish_Started;
            //m_PublishControl.PublishManage.PublishTempDataCompleted += this.Publish_TempDataCompleted;
            m_PublishControl.PublishManage.PublishLog += this.Publish_Log;
            m_PublishControl.PublishManage.UpdateState += this.Publish_UpdateState;
            m_PublishControl.PublishManage.PublishStop += this.Publish_Stop;
            m_PublishControl.PublishManage.DoCount += this.Publish_DoCount;

            //在此增加发布运行中的任务
            m_PublishControl.AddPublishTask(gList);

            //根据选择的“正在运行”树形节点，加载相应的信息
            try
            {
                LoadRunTask();
            }
           
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString("Error16"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

            SetDataShow();

            //加载是否自动保存系统日志标志
            try
            {
                cXmlSConfig Config = new cXmlSConfig(Program.getPrjPath ());
                m_IsAutoSaveLog = Config.AutoSaveLog;
                m_IsAllowRemoteGather = Config.IsAllowRemoteGather;
                m_RemoteServer = Config.RemoteServer;
                m_MaxRemoteCount = Config.MaxRemoteTaskCount;
                Config = null;

                if (Program.SominerVersion == cGlobalParas.VersionType.Cloud)
                {
                    m_IsAllowRemoteGather = true; 
                    m_RemoteServer = "http://spider.netminer.cn";
                    m_MaxRemoteCount = 1;
                }

            }
            catch (System.Exception)
            {
                //表示配置文件出错，但需要继续加载
            }

            //启动时间器用于更新任务显示的进度
            //this.timer1.Enabled = false;

            //更新状态条信息
            UpdateStatebarTask(0,cGlobalParas.TaskState.Request);

            if ((Program.SominerVersion == cGlobalParas.VersionType.Cloud && m_IsAllowRemoteGather == true) ||
                (Program.SominerVersion == cGlobalParas.VersionType.Enterprise && m_IsAllowRemoteGather == true))
            {
                m_RemoteEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_RemoteEngine_CallBack),
                    null, 60000, 60000);
            }

        }

        #region 监听器处理，启动 停止 响应事件

        /// 启动监听器，用于监听计划任务是否可以执行
        public void StartListen()
        {
            m_ListenControl = new cListenControl(Program.getPrjPath());
            m_ListenControl.ListenManage.RunTask += this.On_RunSoukeyTask;
            m_ListenControl.ListenManage.RunTaskLog += this.On_RunTaskLog;
            m_ListenControl.ListenManage.ListenError += this.On_ListenError;

            try
            {
                m_ListenControl.Start();
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error ,cGlobalParas.LogClass.ListenPlan ,System.DateTime.Now.ToString (),  rm.GetString("Info9") + ex.Message));
            }

            m_IsRunListen = true;
        }

        public void StopListen()
        {
            try
            {
                if (m_ListenControl.IsRunning == true)
                {
                    m_ListenControl.Stop();
                    m_ListenControl.ListenManage.RunTask -= this.On_RunSoukeyTask;
                    m_ListenControl.ListenManage.RunTaskLog -= this.On_RunTaskLog;
                    m_ListenControl.ListenManage.ListenError -= this.On_ListenError;
                    m_ListenControl = null;
                }
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error, cGlobalParas.LogClass.ListenPlan, System.DateTime.Now.ToString(), rm.GetString("Info10") + ex.Message));
            }
            m_IsRunListen = false;
        }

        //处理启动任务事件，由监听管理类通过事件进行触发
        private void On_RunSoukeyTask(object sender, cRunTaskEventArgs e)
        {

            string tClassName = e.RunName.Substring(0, e.RunName.LastIndexOf("\\"));
            string TaskName = e.RunName.Substring(e.RunName.LastIndexOf("\\") + 1, e.RunName.Length - e.RunName.LastIndexOf("\\") - 1);

            string tClassPath ="tasks\\" + tClassName.Replace("/","\\");

            cGatherTask t = AddRunTask(tClassName, tClassPath, TaskName);

            if (t == null)
                return;

            Int64 TaskID = t.TaskID;

            //增加Tab标签
            InvokeMethod(this, "AddTab", new object[] { TaskID, TaskName });


            InvokeMethod(this, "AddDataGridRow", new object[] { TaskID, tClassPath,tClassName, TaskName,t.UrlCount });

            //启动此任务
            m_GatherControl.Start(t);

            //任务启动成功显示消息
            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("TaskStarted"),TaskName));

            t = null;
        }

        

        private void On_RunTaskLog(object sender, cRunTaskLogArgs e)
        {
            if (e_ExportLog !=null)
                e_ExportLog(this, new ExportLogEvent(e.LogType ,cGlobalParas.LogClass.ListenPlan,System.DateTime.Now.ToString (), e.strLog ));
        }

        private void On_ListenError(object sender, cListenErrorEventArgs e)
        {
            if (e_ExportLog != null)
                e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error ,cGlobalParas.LogClass.ListenPlan,System.DateTime.Now.ToString (), e.Message));

        }

        private void on_ShowInfo(object sender ,ShowInfoEventArgs e)
        {
            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent(e.Title, e.strInfo));
        }
        #endregion

        #region 事件处理

        public event EventHandler<cTaskEventArgs> Completed
        {
            add
            {
                m_GatherControl.Completed += value;
            }
            remove
            {
                m_GatherControl.Completed -= value;
            }
        }

        public event EventHandler<cTaskEventArgs> TaskCompleted
        {
            add
            {
                m_GatherControl.TaskManage.TaskCompleted += value;
            }
            remove
            {
                m_GatherControl.TaskManage.TaskCompleted -= value;
            }
        }

        private void tManage_Completed(object sender, cTaskEventArgs e)
        {
            //任务执行完毕后，需要将任务移至已经完成的节点中，
            //在此如果选择的是nodRunning则删除datagridview的内容
            //然后添加到完成队列中

            try
            {
                cGatherTask t = (cGatherTask)sender;

                if (e_ShowLogInfo != null)
                    e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("TaskGCompleted"),e.TaskName));

                //任务完成后，无论是否发布都调用此方法，因为要进行临时数据保存
                InvokeMethod(this, "UpdateTaskPublish", new object[] { e.TaskID, t.TaskEntity.IsDelRepRow });

                t = null;

                InvokeMethod(this, "UpdateStatebarTask", new object[]{e.TaskID ,cGlobalParas.TaskState.Completed});

            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error ,cGlobalParas.LogClass.Task,System.DateTime.Now.ToString (), ex.Message));
            }


        }

        //保存采集任务的临时数据
        public void SaveGatherTempData(Int64 TaskID)
        {
            

            //将此任务添加到发布队列中

            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            if (this.tabControl1.TabPages[pageName] == null)
            {
                return;
            }

            string strLog = "开始进行采集数据的保存，以后可在“已经完成的任务”栏目中查看已经下载的数据，请等待......";

            InvokeMethod(this, "ExportGatherLog", new object[] { pageName, conName, cGlobalParas.LogType.Info, strLog });

            DataTable d = (DataTable)((DataGridView)this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0]).DataSource;

            StartSaveTemp(TaskID, d);

        }


        public void SavePublishTempData(Int64 TaskID)
        {
            //if (Program.SominerVersion == cGlobalParas.VersionType.Trial)
            //    return;

            //保存临时数据
            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;


            string strLog = "开始进行发布状态数据的保存，以后您可查看数据的发布情况信息，请等待......";

            InvokeMethod(this, "ExportGatherLog", new object[] { pageName, conName, cGlobalParas.LogType.Info, strLog });


            DataTable d = (DataTable)((DataGridView)this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0]).DataSource;

            StartSaveTemp(TaskID, d);
        }

        //处理任务采集完成的工作，注意任务无论是否发布都要执行
        //此方法，因为无论任务是否发布都需要进行临时数据保存
        //如果不发布，则不进行数据发布任务的启动。
        public void UpdateTaskPublish(Int64 TaskID, bool IsDelRepRow)
        {
            //将此任务添加到发布队列中

            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            if (this.tabControl1.TabPages[pageName] == null)
            {
                //表示采集任务是没有定义采集规则的，
                //所以，会导致任务直接完成，并未建立
                //任务输出的tab页
                UpdateTaskComplete(TaskID,0);
            }
            else
            {
                //表示采集有数据
                DataTable d;

                if (IsDelRepRow == true)
                {
                    //去除重复行
                    DataTable d1 = (DataTable)((DataGridView)this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0]).DataSource;
                    string[] strComuns = new string[d1.Columns.Count];

                    for (int m = 0; m < d1.Columns.Count; m++)
                    {
                        strComuns[m] = d1.Columns[m].ColumnName;
                    }

                    DataView dv = new DataView(d1);

                    d = dv.ToTable(true, strComuns);
                }
                else
                {
                    d = (DataTable)((DataGridView)this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0]).DataSource;
                }

                int rowCount = d.Rows.Count;

                if (d.Rows.Count > 0)
                {
                    NetMiner.Publish.cPublish pt = new NetMiner.Publish.cPublish(Program.getPrjPath(),m_PublishControl.PublishManage, TaskID, d);

                    SaveGatherTempData(TaskID );

                    if (pt.PublishType == cGlobalParas.PublishType.NoPublish)
                    {
                        //如果是不发布数据，则需要通过此方法完成任务采集后的处理工作
                        //删除taskrun中的数据等等，此部分如果是实现了任务数据发布，则
                        //由任务发布结束后触发事件完成，但在此需要手工完成
                        UpdateTaskComplete(TaskID,rowCount);
                    }
                    else
                    {
                        ExportGatherLog(pageName, conName, cGlobalParas.LogType.Info, "此任务配置了发布操作，开始进行数据发布");

                        //修改TaskRun中的状态
                        oTaskRun tr = new oTaskRun(Program.getPrjPath());
                        tr.EditTaskState(TaskID, cGlobalParas.TaskState.Publishing);
                        tr = null;

                        m_PublishControl.startPublish(pt);
                    }
                }
                else
                {
                    UpdateTaskComplete(TaskID,rowCount);
                }
            }

            #region 更新界面显示
          
         
                    for (int i = 0; i < this.dataTask.Rows.Count; i++)
                    {
                        if (this.dataTask.Rows[i].Cells[1].Value.ToString() == TaskID.ToString())
                        {
                            //只有非增量任务，才会删除，所以更新界面时需要判断任务的类型
                            this.dataTask.Rows.Remove(this.dataTask.Rows[i]);
                            break;
                        }

                    }
                
               
              
            
            #endregion

        }


        //处理任务采集完成后的工作，首先如果选择的正在运行的节点，则
        //删除此节点,然后从taskrun数据中删除,然后在删除实际的文件
        public void UpdateTaskComplete(long TaskID ,int rowCount)
        {
            //判断任务是否为增量任务，如果是增量任务，则不能删除taskrun中的信息
            //因为增量任务永远不会过时

            bool isCloseTab = false;
            oTaskRun tr = new oTaskRun(Program.getPrjPath());
            eTaskRun er= tr.LoadSingleTask(TaskID);
            eTaskCompleted ec = new eTaskCompleted();

            string TaskName = er.TaskName;
            ec.TaskID = er.TaskID;
            ec.TaskName = er.TaskName;
            ec.TaskClass = er.TaskClass;
            ec.TaskType = er.TaskType;
            ec.TaskRunType = er.TaskRunType;
            ec.TempFile = er.TempFile;
            ec.UrlCount = er.UrlCount;
            ec.PublishType = er.PublishType;
            ec.StartDate = er.StartDateTime;
            ec.CompleteDate = System.DateTime.Now;
            ec.ExportFile = er.ExportFile;
            ec.GatheredUrlCount = er.GatheredUrlCount;
            ec.GatherResult = cGlobalParas.GatherResult.GatherSucceed;
            ec.RowsCount = rowCount;
            
            //将已经完成的任务添加到完成任务的索引文件中
            oTaskComplete tc = new oTaskComplete(Program.getPrjPath());


            tc.InsertTaskComplete(ec, cGlobalParas.GatherResult.GatherSucceed);
            tc.Dispose();
            tc = null;

            //删除taskrun节点

            tr.LoadTaskRunData();
            tr.DelTask(TaskID);

            //删除run中的任务实例文件

            oTask t = new oTask(Program.getPrjPath());
            t.LoadTask(TaskID);
            isCloseTab = t.TaskEntity.isCloseTab;
            t.Dispose();
            t = null;

            string FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".rst";
            System.IO.File.Delete(FileName);

            //删除run中的任务实例排重库文件
            FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".db";
            System.IO.File.Delete(FileName);

            //删除run中的采集数据排重文件
            FileName = Program.getPrjPath() + "Tasks\\run\\" + "data" + TaskID + ".db";
            System.IO.File.Delete(FileName);

            if (isCloseTab)
                CloseTabBySilent(TaskID, TaskName);

            //开始出发操作，在已经完成的列表中，增加一条数据
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("AddCompletedTask",new object[] { ec}));
        }

        public void UpdateRunTaskState(Int64 TaskID, cGlobalParas.TaskState tState)
        {
            if (this.dataTask.Rows.Count > 0 )
            {
                for (int i = 0; i < this.dataTask.Rows.Count; i++)
                {
                    if (this.dataTask.Rows[i].Cells[1].Value.ToString() == TaskID.ToString())
                    {
                        switch (tState)
                        {
                            case cGlobalParas.TaskState.Running:
                                this.dataTask.Rows[i].Cells[0].Value = imageList1.Images["started"];
                                if (e_SetControlProperty != null)
                                {
                                    e_SetControlProperty(this, new SetControlPropertyEvent("toolStartTask", "Enabled", "false"));
                                    e_SetControlProperty(this, new SetControlPropertyEvent("toolStopTask", "Enabled", "true"));
                                }

                                break;
                            case cGlobalParas.TaskState.Stopped:
                              
                                this.dataTask.Rows[i].Cells[0].Value = imageList1.Images["stop"];
                                
                                if (e_SetControlProperty != null)
                                {
                                    e_SetControlProperty(this, new SetControlPropertyEvent("toolStartTask", "Enabled", "false"));
                                    e_SetControlProperty(this, new SetControlPropertyEvent("toolStopTask", "Enabled", "true"));
                                }
                                break;
                            default:
                                this.dataTask.Rows[i].Cells[0].Value = imageList1.Images["stop"];
                                break;
                        }
                        this.dataTask.Rows[i].Cells[2].Value = tState;
                        break;
                    }
                }

            }

            


        }

        public void UpdatePublishState(Int64 TaskID, cGlobalParas.TaskState tState)
        {
            if (this.dataTask.Rows.Count > 0 && this.dataTask.Rows[0].Cells[3].Value.ToString() == "nodPublish")
            {
                for (int i = 0; i < this.dataTask.Rows.Count; i++)
                {
                    if (this.dataTask.Rows[i].Cells[1].Value.ToString() == TaskID.ToString())
                    {
                        switch (tState)
                        {
                            case cGlobalParas.TaskState.Publishing:
                                this.dataTask.Rows[i].Cells[0].Value = imageList1.Images["export"];
                                if (e_SetControlProperty != null)
                                {
                                    e_SetControlProperty(this, new SetControlPropertyEvent("toolStartTask", "Enabled", "false"));
                                    e_SetControlProperty(this, new SetControlPropertyEvent("toolStopTask", "Enabled", "true"));
                                }

                                break;
                            case cGlobalParas.TaskState.PublishStop:
                                this.dataTask.Rows[i].Cells[0].Value = imageList1.Images["StopPublish"];
                                if (e_SetControlProperty != null)
                                {
                                    e_SetControlProperty(this, new SetControlPropertyEvent("toolStartTask", "Enabled", "false"));
                                    e_SetControlProperty(this, new SetControlPropertyEvent("toolStopTask", "Enabled", "true"));
                                }
                                break;
                            default:
                                this.dataTask.Rows[i].Cells[0].Value = imageList1.Images["StopPublish"];
                                break;
                        }
                        this.dataTask.Rows[i].Cells[2].Value = tState;
                        break;
                    }
                }
            }
        }

        private void tManage_TaskStart(object sender, cTaskEventArgs e)
        {
            //如果任务启动，则修改任务的图标，此事件是由点击按钮后任务
            //启动进行触发

            try
            {
                
                InvokeMethod(this, "UpdateRunTaskState", new object[] { e.TaskID, cGlobalParas.TaskState.Running });

                if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                    Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
                {
                    //需要判断当前执行的任务是不是远程任务，如果是远程任务，则需要修改状态
                    oTask t = new oTask(Program.getPrjPath());
                    t.LoadTask(e.TaskID);
                    cGlobalParas.TaskClass tClass = t.TaskEntity.TaskClass;
                    string TaskDemo = t.TaskEntity.TaskDemo;
                    t = null;
                    if (tClass == cGlobalParas.TaskClass.Remote)
                    {
                        //表示远程任务
                        //localhost.NetMinerWebService sweb = new localhost.NetMinerWebService();
                        //sweb.Url = Program.ConnectServer + "/NetMiner.WebService.asmx";
                        //if (Program.g_IsAuthen == true)
                        //    sweb.Credentials = new System.Net.NetworkCredential(Program.g_WindowsUser, Program.g_WindowsPwd);

                        
                        //localhost.cRemoteTaskEntity rTask = new localhost.cRemoteTaskEntity();
                        string[] TaskDemos = TaskDemo.Split(',');
                        //rTask.ID = int.Parse(TaskDemos[0]);
                        //rTask.TaskName = TaskDemos[1];
                        //rTask.GatherTaskType = int.Parse(TaskDemos[2]);
                        //rTask.TaskFileName = TaskDemos[3];

                        //if (rTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriTask)
                        //    sweb.UpdateTaskState(rTask.ID, Program.RegisterUser, (int)cGlobalParas.TaskState.RemoteRunning, e.TaskID);
                        //else if (rTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriSplitTask)
                        //    sweb.UpdateSplitTaskState(rTask.ID, Program.RegisterUser, (int)cGlobalParas.TaskState.RemoteRunning,e.TaskID);
                    }
                    

                }
                //SetValue(this.toolStrip1.Items["toolStartTask"], "Enabled", false);
                //SetValue(this.toolStrip1.Items["toolRestartTask"], "Enabled", false);
                //SetValue(this.toolStrip1.Items["toolStopTask"], "Enabled", false);

                //UpdateStatebarTask();
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error ,cGlobalParas.LogClass.Task,System.DateTime.Now.ToString (), ex.Message));
            }
        }

        private void tManage_TaskInitialized(object sender, TaskInitializedEventArgs e)
        {
            //暂不做任何处理

            try
            {
                UpdateStatebarTask(e.TaskID ,cGlobalParas.TaskState.UnStart);
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error ,cGlobalParas.LogClass.Task ,System.DateTime.Now.ToString (), ex.Message));
            }

        }

        private void tManage_TaskStateChanged(object sender, TaskStateChangedEventArgs e)
        {
            try
            {
                InvokeMethod(this, "SetTaskShowState", new object[] { e.TaskID, e.NewState });

                UpdateStatebarTask(e.TaskID ,e.NewState);
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error ,cGlobalParas.LogClass.Task,System.DateTime.Now.ToString (), ex.Message));
            }

        }

        private void tManage_TaskStop(object sender, cTaskEventArgs e)
        {

            //如果任务启动，则修改任务的图标，此事件是由点击按钮后任务
            //启动进行触发

            try
            {

                
                InvokeMethod(this, "UpdateRunTaskState", new object[] { e.TaskID, cGlobalParas.TaskState.Stopped });

                if (e_SetControlProperty != null)
                {
                    e_SetControlProperty (this,new SetControlPropertyEvent ("toolStartTask","Enabled","false"));
                    //e_SetControlProperty (this,new SetControlPropertyEvent ("toolRestartTask","Enabled","false"));
                    e_SetControlProperty (this,new SetControlPropertyEvent ("toolStopTask","Enabled","false"));
                }


                ////在此处处理任务由于用户中断，需要进行的必要保存工作
                ////任务中断后，系统需保存已经采集完成的数据，保存已经采集的网址记录，
                ////确保下次运行任务时，可以直接进行，即类似下载的断点功能操作


                SaveGatherTempData(e.TaskID);

                //UpdateStatebarTask();

              
                
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error ,cGlobalParas.LogClass.Task,System.DateTime.Now .ToString (), ex.Message));
            }


        }

        public void UpdateStatebarTask(Int64 TaskID,cGlobalParas.TaskState tState)
        {
            string s = rm.GetString("State1");

            try
            {
                //s += m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count + m_GatherControl.TaskManage.TaskListControl.StoppedTaskList.Count + m_PublishControl.PublishManage.ListPublish.Count + "-个任务  ";
                s += m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count + "-" + rm.GetString("State2") + "  ";
                s += m_GatherControl.TaskManage.TaskListControl.StoppedTaskList.Count + "-" + rm.GetString("State3") + "  ";
                s += m_PublishControl.PublishManage.ListPublish.Count + "-" + rm.GetString("State4");

                if (e_SetControlProperty != null)
                    e_SetControlProperty(this, new SetControlPropertyEvent("toolStripStatusLabel2", "Text", s));

                //更新tab的显示图标
                if (TaskID > 0)
                {
                    string pageName = "page" + TaskID;

                    switch (tState)
                    {
                        case cGlobalParas.TaskState.Running:
                            SetValue((this.tabControl1.TabPages[pageName]), "ImageIndex", 21);
                            //this.tabControl1.TabPages[pageName].ImageIndex = 21;
                            break;
                        case cGlobalParas.TaskState.Started:
                            SetValue((this.tabControl1.TabPages[pageName]), "ImageIndex", 21);
                            //this.tabControl1.TabPages[pageName].ImageIndex = 21;
                            break;
                        case cGlobalParas.TaskState.Stopped:
                            SetValue((this.tabControl1.TabPages[pageName]), "ImageIndex", 19);
                            //this.tabControl1.TabPages[pageName].ImageIndex = 19;
                            break;
                        default:
                            SetValue((this.tabControl1.TabPages[pageName]), "ImageIndex", 19);
                            //this.tabControl1.TabPages[pageName].ImageIndex = 19;
                            break;
                    }
                }

                Application.DoEvents();

            }
            catch (System.Exception ex)
            {
                //捕获错误不处理
            }

        }

        //单个Url采集发生错误，不进行界面响应，记录日志即可，日志由其他事件记录完成
        //当错误达到一定的数量后，会由后台线程触发任务失败的事件，由任务失败事件完成
        //临时数据的存储
        private void tManage_TaskError(object sender, TaskErrorEventArgs e)
        {
            Int64 TaskID = e.TaskID;
            string strLog = e.Error.Message.ToString();
            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            //不需要通过窗口通知告诉用户，直接写入采集日子，注销下面代码
            //InvokeMethod(this, "ShowInfo", new object[] {rm.GetString("TaskGError"), t.TaskName });

            try
            {
                InvokeMethod(this, "ExportGatherLog", new object[] { pageName, conName, cGlobalParas.LogType.GatherError, strLog });
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error, cGlobalParas.LogClass.Task, System.DateTime.Now.ToString(), ex.Message));
            }

        }

        private void tManage_TaskFailed(object sender, cTaskEventArgs e)
        {
            try
            {

                if (e_ShowLogInfo != null)
                    e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("TaskGFailed"),e.TaskName));

                //InvokeMethod(this, "SaveGatherTempData", new object[] { e.TaskID });
                
                cGatherTask t = (cGatherTask)sender;

                //任务完成后，无论是否发布都调用此方法，因为要进行临时数据保存
                InvokeMethod(this, "UpdateTaskPublish", new object[] { e.TaskID, t.TaskEntity.IsDelRepRow });


                InvokeMethod(this, "UpdateStatebarTask", new object[] { e.TaskID, cGlobalParas.TaskState.Failed });

                if (e_SetControlProperty != null)
                {
                    e_SetControlProperty(this, new SetControlPropertyEvent("toolStartTask", "Enabled", "false"));
                    //e_SetControlProperty(this, new SetControlPropertyEvent("toolRestartTask", "Enabled", "false"));
                    e_SetControlProperty(this, new SetControlPropertyEvent("toolStopTask", "Enabled", "false"));
                }

            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error,cGlobalParas.LogClass.Task ,System.DateTime.Now.ToString (), ex.Message));
            }
        }

        private void tManage_TaskAbort(object sender, cTaskEventArgs e)
        {

            //InvokeMethod(this, "SaveGatherTempData", new object[] { e.TaskID });

            //if (m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count > 0)
            //{
            //    IsExitting = true ;
            //}
            //else
            //{
            //    IsExitting = false;
            //}

        }

        private void m_Gather_Completed(object sender, EventArgs e)
        {
            //任务采集完成，则启动消息通知窗体，通知用户


        }

        //写日志事件
        private void tManage_Log(object sender, cGatherTaskLogArgs e)
        {
            //写日志
            Int64 TaskID = e.TaskID;
            string strLog = e.strLog;
            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            try
            {
                InvokeMethod(this, "ExportGatherLog", new object[] { pageName, conName, e.LogType, e.strLog });
                //SetValue(this.tabControl1.TabPages[pageName].Controls[conName].Controls[1].Controls[0], "Text", strLog);
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error,cGlobalParas.LogClass.Task ,System.DateTime.Now.ToString(), ex.Message));
            }

        }

        //写数据事件
        private void tManage_GData(object sender, cGatherDataEventArgs e)
        {
            try
            {
                //写采集数据到界面Datagridview


                InvokeMethod(this, "UpdateGatherData", new object[] { e.TaskID, e.TaskName, e.gData });
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error, cGlobalParas.LogClass.Task, System.DateTime.Now.ToString(), ex.Message));
            }
        }

        private void tManage_GUrlCount(object sender,cGatherUrlCounterArgs e)
        {
            
            try
            {
                //写采集数据到界面Datagridview


                InvokeMethod(this, "UpdateGatherUrl", new object[] { e.TaskID, e.UrlCount,e.GUrlCount,e.ErrCount });
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error, cGlobalParas.LogClass.Task, System.DateTime.Now.ToString(), ex.Message));
            }

        }

        public void UpdateGatherData(Int64 TaskID,string TaskName,DataTable d)
        {
            DataTable gData = d;
            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            ((cMyDataGridView)this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0]).gData= d;
            int count = ((cMyDataGridView)this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0]).gData.Rows.Count;

            this.tabControl1.TabPages[pageName].Text = TaskName + "[" + count + "]";

            //在此更新rowCount
            DataGridViewRow dRow = (from a in this.dataTask.Rows.Cast<DataGridViewRow>()
                                    where a.Cells[1].Value.Equals(TaskID)
                                    select a).FirstOrDefault();
            dRow.Cells[8].Value = count;

        }

        public void UpdateGatherUrl(Int64 TaskID,int UrlCount,int gUrlCount,int errUrlCount)
        {
            


            //在此更新rowCount
            DataGridViewRow dRow = (from a in this.dataTask.Rows.Cast<DataGridViewRow>()
                                    where a.Cells[1].Value.Equals(TaskID)
                                    select a).FirstOrDefault();
            //errurlcount, urlcount, "", (gurlcount + errurlcount) * 100 / (urlcount == 0 ? 1 : urlcount),
            
       
            dRow.Cells[9].Value = errUrlCount;

            //dRow.Cells[11].Value = (gUrlCount + errUrlCount)  / (UrlCount == 0 ? 1 : UrlCount);

            ((DataGridViewProgressBarCell)dRow.Cells[11]).Maximum = UrlCount;
            ((DataGridViewProgressBarCell)dRow.Cells[11]).Mimimum = 0;
            ((DataGridViewProgressBarCell)dRow.Cells[11]).Value = (gUrlCount + errUrlCount);

        }

        #endregion

        public void UpdateTaskPublished(Int64 TaskID, cGlobalParas.GatherResult tState,bool isDelData,string TmpFileName)
        {
            //判断任务是否为增量任务，如果是增量任务，则不能删除taskrun中的信息
            //因为增量任务永远不会过时

            //重新保存数据
            if (isDelData == false)
            {
                SavePublishTempData(TaskID);
            }
            else
            {

                string conName = "sCon" + TaskID;
                string pageName = "page" + TaskID;

                if (TmpFileName != "")
                    File.Delete(TmpFileName);

                InvokeMethod(this, "ExportPublishLog", new object[] { pageName, conName, cGlobalParas.LogType.Warning, "此采集任务配置了在发布完成后删数据，因此采集数据已经删除！" });

            }

            oTaskRun tr = new oTaskRun(Program.getPrjPath());
            eTaskRun er = tr.LoadSingleTask(TaskID);
            eTaskCompleted ec = new eTaskCompleted();

            ec.TaskID = er.TaskID;
            ec.TaskName = er.TaskName;
            ec.TaskType = er.TaskType;
            ec.TaskRunType = er.TaskRunType;
            ec.TempFile = er.TempFile;
            ec.UrlCount = er.UrlCount;
            ec.PublishType = er.PublishType;
            ec.CompleteDate = System.DateTime.Now;
            ec.ExportFile = er.ExportFile;
            ec.GatheredUrlCount = er.GatheredUrlCount;
            ec.GatherResult = tState;

            //将已经完成发布的任务添加到完成任务的索引文件中
            oTaskComplete t = new oTaskComplete(Program.getPrjPath());
            t.InsertTaskComplete(ec, tState);
            t = null;

            //删除taskrun节点
            tr.LoadTaskRunData();
            tr.DelTask(TaskID);

            //修改Tab页的名称


            //删除run中的任务实例文件
            string FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".rst";
            System.IO.File.Delete(FileName);

            //删除run中的任务实例排重库文件
            FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".db";
            System.IO.File.Delete(FileName);

            //删除run中的采集数据排重文件
            FileName = Program.getPrjPath() + "Tasks\\run\\" + "data" + TaskID + ".db";
            System.IO.File.Delete(FileName);

            for (int i = 0; i < this.dataTask.Rows.Count; i++)
            {
                if (this.dataTask.Rows[i].Cells[1].Value.ToString() == TaskID.ToString())
                {
                    this.dataTask.Rows.Remove(this.dataTask.Rows[i]);
                    break;
                }
            }
            
         
        }

        #region 发布任务的事件处理
        private void Publish_Complete(object sender, PublishCompletedEventArgs e)
        {

            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("TaskPulished"),e.TaskName));


            InvokeMethod(this, "UpdateTaskPublished", new object[] { e.TaskID, cGlobalParas.GatherResult.PublishSuccees,e.IsDel ,e.TmpFileName});

            InvokeMethod(this, "UpdateStatebarTask", new object[] {0, cGlobalParas.TaskState.Completed });

        }

        private void Publish_Started(object sender, PublishStartedEventArgs e)
        {
            try
            {
                InvokeMethod(this, "UpdatePublishState", new object[] { e.TaskID, cGlobalParas.TaskState.Publishing });
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error, cGlobalParas.LogClass.Task, System.DateTime.Now.ToString(), ex.Message));
            }

            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("TaskPublishing"), e.TaskName));

            InvokeMethod(this, "UpdateStatebarTask", new object[] {0,cGlobalParas.TaskState.Publishing });
        }

        private void Publish_Stop(object sender, PublishStopEventArgs e)
        {

            try
            {
                InvokeMethod(this, "SavePublishTempData", new object[] { e.TaskID});

                InvokeMethod(this, "UpdatePublishState", new object[] { e.TaskID, cGlobalParas.TaskState.PublishStop });
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error, cGlobalParas.LogClass.Task, System.DateTime.Now.ToString(), ex.Message));
            }

            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent("发布已停止", e.TaskName));

            InvokeMethod(this, "UpdateStatebarTask", new object[] {0,cGlobalParas.TaskState.PublishStop });
        }

        private void Publish_DoCount(object sender, DoCountEventArgs e)
        {
        }

        private void Publish_Error(object sender, PublishErrorEventArgs e)
        {
            //InvokeMethod(this, "UpdateTaskPublished", new object[] { e.TaskID, cGlobalParas.GatherResult.PublishFailed });

            //if (e_ShowLogInfo != null)
            //    e_ShowLogInfo(this, new ShowInfoEvent(e.TaskName, rm.GetString("TaskPublishFailed")));


            //InvokeMethod(this, "UpdateStatebarTask", null);

            Int64 TaskID = e.TaskID;
            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            //采集发生错误，仅进行日志输出
            InvokeMethod(this, "ExportPublishLog", new object[] { pageName, conName, cGlobalParas.LogType.Error, "发布出错，错误信息：" + e.Error.Message });


        }

        private void Publish_Failed(object sender, PublishFailedEventArgs e)
        {
            try
            {
                InvokeMethod(this, "SavePublishTempData", new object[] { e.TaskID });

                InvokeMethod(this, "UpdatePublishState", new object[] { e.TaskID, cGlobalParas.TaskState.PublishFailed });
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error, cGlobalParas.LogClass.Task, System.DateTime.Now.ToString(), ex.Message));
            }

            InvokeMethod(this, "UpdateTaskPublished", new object[] { e.TaskID, cGlobalParas.GatherResult.PublishFailed ,false,""});

            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("TaskPublishFailed"),e.TaskName));


            InvokeMethod(this, "UpdateStatebarTask", new object[] { e.TaskID, cGlobalParas.TaskState.PublishFailed });
        }

        //private void Publish_TempDataCompleted(object sender, PublishTempDataCompletedEventArgs e)
        //{
        //    Int64 TaskID = e.TaskID;
        //    string conName = "sCon" + TaskID;
        //    string pageName = "page" + TaskID;

        //    InvokeMethod(this, "ExportPublishLog", new object[] { pageName, conName,cGlobalParas.LogType.Info  , "数据保存成功！" });

        //}

        private void Publish_Log(object sender, PublishLogEventArgs e)
        {
            //写发布任务的日志
            Int64 TaskID = e.TaskID;
            string strLog = e.strLog;
            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            try
            {
                InvokeMethod(this, "ExportPublishLog", new object[] { pageName, conName, e.LogType, e.strLog });
                //SetValue(this.tabControl1.TabPages[pageName].Controls[conName].Controls[1].Controls[0], "Text", strLog);
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error ,cGlobalParas.LogClass.PublishLog ,System.DateTime.Now.ToString (), ex.Message));
            }

            //SetValue(this.tabControl1.TabPages[pageName].Controls[conName].Controls[1].Controls[0], "Text", strLog);
        }

        private void Publish_UpdateState(object sender, UpdateStateArgs e)
        {
            Int64 TaskID = e.TaskID;
            object[] row = e.Row;
            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            int colCount = 0;
            try
            {
                colCount = ((cMyDataGridView)(this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0])).Columns.Count;
            }
            catch 
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error, cGlobalParas.LogClass.Task, DateTime.Now.ToString(), "采集页面已经关闭，导致状态更新失败！"));
                return;
            }

            DataGridViewRow r= ((cMyDataGridView)(this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0]))
                .FindRow(row);
             r.Cells[colCount - 1].Value = e.isPublishSucceed.ToString();

        }

        #endregion

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

            if (iType.Name.ToString() == "ToolStripButton")
            {
                //inst = this.toolStrip1;
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

        //用于更新界面的显示状态
        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    int proI = 0;

        //    if (IsTimer == true)
        //    {
        //        IsTimer = false;

        //        if (this.dataTask.Rows.Count < 1)
        //        {
        //            IsTimer = true;
        //            return;
        //        }


        //        //判断当前显示的是什么信息

        //        try
        //        {
                   
        //                //如果当前选中则开始更新
        //                //按照m_GatherControl.TaskManage.TaskList进行更新
        //                for (int i = 0; i < m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count; i++)
        //                {
        //                    for (int j = 0; j < this.dataTask.Rows.Count; j++)
        //                    {
        //                        if (m_GatherControl.TaskManage.TaskListControl.RunningTaskList[i].TaskID.ToString() == this.dataTask.Rows[j].Cells[1].Value.ToString())
        //                        {
        //                            int urlcount = m_GatherControl.TaskManage.TaskListControl.RunningTaskList[i].UrlCount + m_GatherControl.TaskManage.TaskListControl.RunningTaskList[i].UrlNaviCount;
        //                            int gurlcount = m_GatherControl.TaskManage.TaskListControl.RunningTaskList[i].GatheredUrlCount + m_GatherControl.TaskManage.TaskListControl.RunningTaskList[i].GatheredUrlNaviCount;
        //                            int errurlcount = m_GatherControl.TaskManage.TaskListControl.RunningTaskList[i].GatherErrUrlCount + m_GatherControl.TaskManage.TaskListControl.RunningTaskList[i].GatheredErrUrlNaviCount;
        //                            proI = (gurlcount + errurlcount) * 100 / urlcount;
                                    
        //                            ((DataGridViewProgressBarCell)this.dataTask.Rows[j].Cells[11]).Maximum = urlcount;
        //                            ((DataGridViewProgressBarCell)this.dataTask.Rows[j].Cells[11]).Mimimum = 0;
        //                            ((DataGridViewProgressBarCell)this.dataTask.Rows[j].Cells[11]).Value = gurlcount+ errurlcount;
        //                            Application.DoEvents();

        //                        }
        //                    }
        //                }
                   
        //        }
        //        catch { }

        //        UpdateStatebarTask(0,cGlobalParas.TaskState.UnStart);

        //        //更新速率
        //        //if (e_dSpeedEvent != null)
        //        //    e_dSpeedEvent(this, new DownloadSpeedEvent(NetMiner.Gather.Gather.cWatchSpeedRate.DownloadSpeed));

        //        IsTimer = true;
        //    }
        //}

        #region 触发事件 用于更新主窗体容器信息
        private readonly Object m_eventLock = new Object();

        private event EventHandler<SetToolbarStateEvent> e_SetToolbarState;
        public event EventHandler<SetToolbarStateEvent> SetToolbarState
        {
            add { lock (m_eventLock) { e_SetToolbarState += value; } }
            remove { lock (m_eventLock) { e_SetToolbarState -= value; } }
        }

        private event EventHandler<ResetToolbarStateEvent> e_ResetToolbarState;
        public event EventHandler<ResetToolbarStateEvent> ResetToolbarState
        {
            add { lock (m_eventLock) { e_ResetToolbarState += value; } }
            remove { lock (m_eventLock) { e_ResetToolbarState -= value; } }
        }

        private event EventHandler<SetControlPropertyEvent> e_SetControlProperty;
        public event EventHandler<SetControlPropertyEvent> SetControlProperty
        {
            add { lock (m_eventLock) { e_SetControlProperty += value; } }
            remove { lock (m_eventLock) { e_SetControlProperty -= value; } }
        }

        private event EventHandler<ShowInfoEvent> e_ShowLogInfo;
        public event EventHandler<ShowInfoEvent> ShowLogInfo
        {
            add { lock (m_eventLock) { e_ShowLogInfo += value; } }
            remove { lock (m_eventLock) { e_ShowLogInfo -= value; } }
        }

        private event EventHandler<ExportLogEvent> e_ExportLog;
        public event EventHandler<ExportLogEvent> ExportLog
        {
            add { lock (m_eventLock) { e_ExportLog += value; } }
            remove { lock (m_eventLock) { e_ExportLog -= value; } }
        }

        private event EventHandler<ExcuteFunctionEvent> e_ExcuteFunction;
        public event EventHandler<ExcuteFunctionEvent> ExcuteFunction
        {
            add { lock (m_eventLock) { e_ExcuteFunction += value; } }
            remove { lock (m_eventLock) { e_ExcuteFunction -= value; } }
        }

        private event EventHandler<DelInfoEvent> e_DelInfo;
        public event EventHandler<DelInfoEvent> DelInfo
        {
            add { lock (m_eventLock) { e_DelInfo += value; } }
            remove { lock (m_eventLock) { e_DelInfo -= value; } }
        }

        private event EventHandler e_PublishByRuleEvent;
        public event EventHandler PublishByRuleEvent
        {
            add { e_PublishByRuleEvent += value; }
            remove { e_PublishByRuleEvent -= value; }
        }

        private event EventHandler e_PublishByPluginEvent;
        public event EventHandler PublishByPluginEvent
        {
            add { e_PublishByPluginEvent += value; }
            remove { e_PublishByPluginEvent -= value; }
        }

        private event EventHandler<DownloadSpeedEvent> e_dSpeedEvent;
        public event EventHandler<DownloadSpeedEvent> dSpeedEvent
        {
            add { lock (m_eventLock) { e_dSpeedEvent += value; } }
            remove { lock (m_eventLock) { e_dSpeedEvent -= value; } }
        }

        private event EventHandler<OpenDataEvent> e_OpenDataEvent;
        public event EventHandler<OpenDataEvent> OpenDataEvent
        {
            add { lock (m_eventLock) { e_OpenDataEvent += value; } }
            remove { lock (m_eventLock) { e_OpenDataEvent -= value; } }
        }
        #endregion

        //在此处理静默运行模式下的工作
        public  void CloseTabBySilent(Int64  TaskID,string TaskName)
        {
            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                if (this.tabControl1.TabPages[i].Name == pageName)
                {
                    //this.tabControl1.TabPages.Remove(this.tabControl1.TabPages[i]);

                    //for (int j = 0; j < this.tabControl1.TabPages[i].Controls.Count; j++)
                    //{
                    //    if (this.tabControl1.TabPages[i].Controls[j] != null)
                    //        this.tabControl1.TabPages[i].Controls[j].Dispose();
                    //}

                    RemoveTab(i);
                    this.tabControl1.TabPages[i].Dispose();

                    //判断是否已经没有选项卡，如果没有，则隐藏tab显示
                    if (this.tabControl1.TabPages.Count == 0)
                        this.splitContainer2.Panel2Collapsed = true;

                    break;
                }
            }

        }

        private void rMenuStopTask_Click(object sender, EventArgs e)
        {
            string TaskID = this.tabControl1.SelectedTab.Name.Substring(4, this.tabControl1.SelectedTab.Name.Length - 4);
            Int64 tID = Int64.Parse(TaskID);
            cPublish pt = m_PublishControl.PublishManage.FindTask(tID);
            if (pt != null)
            {
                pt.StopPublish();
            }
            else
            {
                cGatherTask t = m_GatherControl.TaskManage.FindTask(tID);
                if (t != null)
                {
                    StopTask(tID);
                }
            }

         
        }

        private void rmenuClearCompleted_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        public void ClearData()
        {
            //获取所有已经采集的任务，并对采集任务进行分析，如果下载数据已经为零，则删除
            oTaskComplete tc = new oTaskComplete(Program.getPrjPath());
            IEnumerable<eTaskCompleted> eTasks= tc.LoadTaskData();

            List<long> delTasks = new List<long>();

            foreach(eTaskCompleted ec in eTasks)
            {
                DataTable d = new DataTable();
                string tmpFile = ec.TempFile;

                if (System.IO.File.Exists(tmpFile))
                {
                    d.ReadXml(tmpFile);
                    if (d.Rows.Count == 0)
                    {
                        //表示没有数据，删除此采集任务的数据集文件
                        delTasks.Add(ec.TaskID);
                    }
                }
                else
                {
                    delTasks.Add(ec.TaskID);
                }

            }
           

            if (delTasks.Count == 0)
            {
                MessageBox.Show("清理完毕，未发现无效的采集任务数据！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (MessageBox.Show("共有" + delTasks.Count + "个已经完成的采集任务数据无效，是否删除？", "网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    for (int i = 0; i < delTasks.Count; i++)
                    {
                        long TaskID = delTasks[i];
                        string TaskName = tc.LoadSingleTask(TaskID).TaskName;
                        tc.DelTask(TaskID);
                        tc = null;

                        //删除run中的任务实例文件
                        string FileName = Program.getPrjPath() + "data\\" + TaskName + "-" + TaskID + ".xml";
                        System.IO.File.Delete(FileName);
                    }
                }
            }

            tc.Dispose();
            tc = null;
        }

        private void rmenuPublishData_Click(object sender, EventArgs e)
        {
            //OpenCompletedData();
            string dFile = "";
            DataTable tmp = new DataTable();

            //判断是浏览的那些数据：正在运行还是采集完成
            string TaskID = this.tabControl1.SelectedTab.Name.Substring(4, this.tabControl1.SelectedTab.Name.Length - 4);
            Int64 tID = Int64.Parse(TaskID);
            string TaskName = this.tabControl1.SelectedTab.Tag.ToString();

            oTaskRun tr = new oTaskRun(Program.getPrjPath());
            eTaskRun er= tr.LoadSingleTask(tID);
            if (tr.GetCount() != 0)
            {
                dFile = er.TempFile;
                OpenRunningData();
                return;
            }
            tr = null;

            if (dFile == "")
            {

                //oTaskComplete tc = new oTaskComplete(Program.getPrjPath());
                //tc.LoadSingleTask(tID);

                //dFile = tc.GetTempFile(0);
                //tc = null;
                //OpenCompletedData();
                oTaskComplete tc = new oTaskComplete(Program.getPrjPath());
                eTaskCompleted ec= tc.LoadSingleTask(tID);

                dFile = ec.TempFile;
                tc = null;

                if (e_OpenDataEvent != null)
                {
                    e_OpenDataEvent(this, new OpenDataEvent(cGlobalParas.DatabaseType.SoukeyData, dFile, "", TaskName));
                }
                return;
            }

         
            //if (File.Exists(dFile))
            //{
            //    try
            //    {
            //        ProcessStartInfo startInfo = new ProcessStartInfo("SoukeyDataPublish.exe");
            //        startInfo.WindowStyle = ProcessWindowStyle.Maximized; ;

            //        //传入第三个参数，如果是soukeydata数据类型，则不是用，但为了保证系统不出错，加入了//
            //        startInfo.Arguments = ((int)cGlobalParas.DatabaseType.SoukeyData).ToString() + " " + dFile + " " + "//";

            //        Process.Start(startInfo);


            //    }
            //    catch (System.Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);

            //    }
            //}
            //else
            //{
            //    MessageBox.Show(rm.GetString("Info118"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }

        private void dataTask_Click(object sender, EventArgs e)
        {

        }

        private void rMenuStartTask_Click(object sender, EventArgs e)
        {
            string TaskID = this.tabControl1.SelectedTab.Name.Substring(4, this.tabControl1.SelectedTab.Name.Length - 4);
            Int64 tID = Int64.Parse(TaskID);
            cPublish pt = m_PublishControl.PublishManage.FindTask(tID);
            if (pt != null)
            {
                StartPublish(tID, pt.TaskName,false );
            }
            else
            {
                cGatherTask t = m_GatherControl.TaskManage.FindTask(tID);
                if (t != null)
                {
                    StartTask(tID);
                }
            }
          
        }


        private void rmenuUploadTask_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("UploadTask", null));
        }

        private void publishByRuleEvent(object sender, EventArgs e)
        {
            if (MessageBox.Show("启动数据发布后，将无法停止，您也可使用“网络矿工发布工具”进行发布操作，是否继续？", "网络矿工 询问",
                MessageBoxButtons.YesNo , MessageBoxIcon.Question) == DialogResult.No)
                return;

            string pName= ((ToolStripDropDownItem)sender).Text ;

            if (IsExportData() == true)
            {
                MessageBox.Show(rm.GetString("Info35"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ExportByRule(pName);
        }

        private void publishByPluginEvent(object sender, EventArgs e)
        {
            if (MessageBox.Show("启动数据发布后，将无法停止，您也可使用“网络矿工发布工具”进行发布操作，是否继续？", "网络矿工 询问",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            string pName = ((ToolStripDropDownItem)sender).Text;

            if (IsExportData() == true)
            {
                MessageBox.Show(rm.GetString("Info35"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ExportByPlugin(pName);
        }

        private void ExportByRule(string pName)
        {
            string TaskID = this.tabControl1.SelectedTab.Name.Replace("page", "");
            string tName = this.tabControl1.SelectedTab.Tag.ToString();
            DataGridView tmp = (DataGridView)this.tabControl1.SelectedTab.Controls[0].Controls[0].Controls[0];
            ProgressBar pBar = (ProgressBar)this.tabControl1.SelectedTab.Controls[0].Controls[1].Controls[0];
            pBar.Visible = true;

            //将数据拷贝出来
            DataTable d = ((DataTable)tmp.DataSource).Copy();

            m_ExportPageName = this.tabControl1.SelectedTab.Name;

            ExportGatherLog(this.tabControl1.SelectedTab.Name, this.tabControl1.SelectedTab.Controls[0].Name, cGlobalParas.LogType.Info, rm.GetString("Info237") + "\n");

            oPublishTask p = new oPublishTask(Program.getPrjPath());
            ePublishTask ep= p.LoadSingleTask(pName);

            ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);

            if (ep.PublishType == cGlobalParas.PublishType.publishTemplate)
            {
                //处理发布模版的信息
                //string templateName=p.GetTemplateName(0);
                //templateName = templateName.Substring(0, templateName.IndexOf("["));
                //cGlobalParas.PublishTemplateType pType = (cGlobalParas.PublishTemplateType)cGlobalParas.ConvertID(templateName.Substring(templateName.IndexOf("[") + 1, templateName.IndexOf("]") - templateName.IndexOf("[") - 1));

                //模版发布
                cExport ce = new cExport(this, showProgress,ep.TemplateName,ep.PublishType,ep.User, ep.Password,ep.Domain, 
                     ep.TemplateDBConn, p.GetPublishParas(0),d);
                string[] ps = new string[] { TaskID.ToString() };
                Thread t = new Thread(new ParameterizedThreadStart(ce.RunProcess));
                t.IsBackground = true;
                t.Start(ps);

            }
            else if (ep.PublishType == cGlobalParas.PublishType.PublishData)
            {
                cExport eExcel = new cExport(this, showProgress,ep.DataType, ep.DataSource,ep.InsertSql, 
                   ep.DataTable, d,ep.IsSqlTrue);
                string[] ps = new string[] { TaskID.ToString() };
                Thread t = new Thread(new ParameterizedThreadStart(eExcel.RunProcess));
                t.IsBackground = true;
                t.Start(ps);
            }

            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("Info38"), rm.GetString("Info37")));

            tName = null;
            p = null;
            
        }

        private void ExportByPlugin(string pName)
        {
            string TaskID = this.tabControl1.SelectedTab.Name.Replace("page", "");
            string tName = this.tabControl1.SelectedTab.Tag.ToString();
            DataGridView tmp = (DataGridView)this.tabControl1.SelectedTab.Controls[0].Controls[0].Controls[0];
            ProgressBar pBar = (ProgressBar)this.tabControl1.SelectedTab.Controls[0].Controls[1].Controls[0];
            pBar.Visible = true;

            //将数据拷贝出来
            DataTable d = ((DataTable)tmp.DataSource).Copy();

            m_ExportPageName = this.tabControl1.SelectedTab.Name;

            ExportGatherLog(this.tabControl1.SelectedTab.Name, this.tabControl1.SelectedTab.Controls[0].Name, cGlobalParas.LogType.Info, rm.GetString("Info237") + "\n");

            cPlugin p = new cPlugin(Program.getPrjPath());
            int count = p.GetCount();
            int pIndex=0;
            for (pIndex = 0; pIndex < count; pIndex++)
            {
                if (p.GetPluginName(pIndex) == pName)
                    break;

            }

            ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);
            cExport eExcel = new cExport(this, showProgress, p.GetPlugin(pIndex),d);
            string[] ps = new string[] { TaskID.ToString() };
            Thread t = new Thread(new ParameterizedThreadStart(eExcel.RunProcess));
            t.IsBackground = true;
            t.Start(ps);
            

            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("Info38"), rm.GetString("Info37")));

            tName = null;
            p = null;
        }

        private DialogResult MyMessageBox(string Mess, string Title, MessageBoxButtons but, MessageBoxIcon icon)
        {
            frmMessageBox fm = new frmMessageBox();
            fm.MessageBox(Mess, Title, but, icon);
            DialogResult dr = fm.ShowDialog();
            fm.Dispose();

            return dr;
        }

        private void rmenuImportTask_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
            {
                e_ExcuteFunction(this, new ExcuteFunctionEvent("ImportTask", null));
            }
        }

        private void rMenuExportWord_Click(object sender, EventArgs e)
        {
            ExportWord();
        }

        #region 保存临时文件，异步函数
        private delegate string[] delegateSaveTempFile(Int64 TaskID, DataTable d, string TaskName, string TempDataFile,
            bool IsSaveSingleFile);
        private void StartSaveTemp(Int64 TaskID, DataTable d)
        {
            //先将需要的数据提取出来
            //oTask t = new oTask(Program.getPrjPath());
            //t.LoadTask(TaskID);
            //string TaskName = t.TaskEntity.TaskName;
            //string SavePath = t.TaskEntity.SavePath;
            //string TempDataFile = t.TaskEntity.TempDataFile;
            //bool isSaveSingleFile = t.TaskEntity.IsSaveSingleFile;
            //t = null;

            oTaskRun tr = new oTaskRun(Program.getPrjPath());
            eTaskRun er= tr.LoadSingleTask(TaskID);
            string TaskName = er.TaskName;
            string TempDataFile = er.TempFile;
            tr.Dispose();
            tr = null;

            delegateSaveTempFile sd = new delegateSaveTempFile(this.SaveTempData);
            AsyncCallback callback = new AsyncCallback(CallbackSaveTempFile);
            sd.BeginInvoke(TaskID, d,TaskName ,TempDataFile ,false , callback, sd);
        }

        /// <summary>
        /// 保存临时文件的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void CallbackSaveTempFile(IAsyncResult ar)
        {
           
            delegateSaveTempFile sd = (delegateSaveTempFile)ar.AsyncState;
            string[] str = sd.EndInvoke(ar);
            string conName = string.Empty;
            string pageName = string.Empty;

            string tID = str[0];
            string tName = str[1];
            string err = str[2];

            try
            {

           

                conName = "sCon" + tID;
                pageName = "page" + tID;


                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error, cGlobalParas.LogClass.PublishLog, System.DateTime.Now.ToString("yyyy-MM-dd HH:ss"), tName + err));
     

                

            }
            catch (System.Exception ex)
            {
                if (e_ShowLogInfo != null)
                    e_ShowLogInfo(this, new ShowInfoEvent("错误信息", tName +  "采集任务保存临时文件出错，错误信息：" + ex.Message));
            }
        
        }

        private readonly object m_fileLock = new object();
        /// <summary>
        /// 用于保存临时数据，包括采集和发布过程中的数据信息
        /// </summary>
        private string[] SaveTempData(Int64 TaskID,DataTable d,string TaskName,string TempDataFile,
            bool IsSaveSingleFile)
        {
            string FileName = TempDataFile;
            try
            {
              
                if (IsSaveSingleFile)
                {
                    //FileName = SavePath + "\\" + TempDataFile;

                    if (File.Exists(FileName))
                    {
                        System.Data.DataTable tmp = new System.Data.DataTable();
                        tmp.TableName = TaskID.ToString ();
                        tmp.ReadXml(FileName);
                        tmp.Merge(d);
                        tmp.AcceptChanges();
                        tmp.WriteXml(FileName, XmlWriteMode.WriteSchema);
                        tmp = null;
                    }
                    else
                    {
                        d.TableName = TaskID.ToString ();
                        d.WriteXml(FileName, XmlWriteMode.WriteSchema);
                    }
                }
                else
                {
                    //FileName = SavePath + "\\" + TaskName + "-" + TaskID + ".xml"; ;

                    if (File.Exists(FileName))
                    {
                        lock (m_fileLock)
                        {
                            File.Delete(FileName);
                        }
                    }
                    d.TableName = TaskID.ToString ();
                    d.WriteXml(FileName, XmlWriteMode.WriteSchema);
                }


            }
            catch (System.Exception ex)
            {
                return new string[] { TaskID.ToString (), TaskName, ex.Message };

            }
            finally
            {
                
            }
            return new string[] { TaskID.ToString(), TaskName, "数据保存至：" + Path.GetFileName(FileName) };
        }

        #endregion


        #region 分布式请求的操作
        /// <summary>
        /// 一个定时器，定期服务器进行通讯，表示自己的客户端是有效的，同时索取相应的采集任务进行采集操作
        /// </summary>
        /// <param name="State"></param>
        private void m_RemoteEngine_CallBack(object State)
        {
            if (this.m_IsDo == false)
            {
                m_IsDo = true;

                #region 获取采集任务及上传采集的数据
                try
                {

                    //localhost.NetMinerWebService sweb = new localhost.NetMinerWebService();
                    //sweb.Url = Program.ConnectServer + "/NetMiner.WebService.asmx";
                    //if (Program.g_IsAuthen == true)
                    //    sweb.Credentials = new System.Net.NetworkCredential(Program.g_WindowsUser, Program.g_WindowsPwd);

                    //try
                    //{
                    //int result= sweb.ActiveClient(Program.RegisterUser);
                    //if (result == 2)
                    //{
                    //    //清除本地的远程任务
                    //    ClearRemoteTask();
                    //}
                    //}
                    //catch { }

                    oTaskIndex xmlTasks = new oTaskIndex(Program.getPrjPath());
                    IEnumerable<NetMiner.Core.gTask.Entity.eTaskIndex> eIndexs = xmlTasks.GetTaskDataByClass(NetMiner.Constant.g_RemoteTaskClass);
                    int tCount = xmlTasks.GetTaskCount();
                    int rCount = 0;

                    if (tCount < m_MaxRemoteCount)
                    {
                        #region 从远程获取采集任务
                        //localhost.cRemoteTaskEntity getRemoteTask = sweb.GetTaskName(Program.RegisterUser);

                        //if (getRemoteTask != null)
                        //{
                        //    //获取的任务是个zip压缩文件
                        //    string fName = getRemoteTask.TaskFileName;
                        //    byte[] taskByte = sweb.GetTaskFile(fName);

                        //    //先将压缩文件存在本地临时目录
                        //    string tmpFile = Program.getPrjPath() + "tmp\\" + Path.GetFileName(fName);
                        //    if (!Directory.Exists(Program.getPrjPath() + "tmp\\"))
                        //        Directory.CreateDirectory(Program.getPrjPath() + "tmp\\");
                        //    if (File.Exists(tmpFile))
                        //    {
                        //        File.Delete(tmpFile);
                        //    }

                        //    MemoryStream ms = new MemoryStream(taskByte);
                        //    FileStream fs = new FileStream(tmpFile, FileMode.Create);
                        //    ms.WriteTo(fs);
                        //    fs.Flush();
                        //    ms.Flush();
                        //    ms.Close();
                        //    fs.Close();

                        //    //获取到文件后开始解压缩
                        //    NetMiner.Common.Tool.cZipCompression zCompress = new NetMiner.Common.Tool.cZipCompression();
                        //    zCompress.DeCompressZIP(tmpFile, Program.getPrjPath() + "tmp\\");

                        //    string FileName = Program.getPrjPath() + "tmp\\" + Path.GetFileNameWithoutExtension(tmpFile)
                        //        + "\\" + Path.GetFileNameWithoutExtension(tmpFile) + ".smt";

                        //    //插入index数据

                        //    oTask t = new oTask(Program.getPrjPath());
                        //    t.LoadTask(FileName);

                        //    oTaskIndex ti = new oTaskIndex(Program.getPrjPath());
                        //    //先判断索引文件中是否存在此任务，如果存在，则跳过，不进行下载操作
                        //    if (!ti.isExistTask(Path.GetFileNameWithoutExtension(fName)))
                        //    {
                        //        try
                        //        {
                        //            t.TaskEntity.TaskName = Path.GetFileNameWithoutExtension(fName);
                        //            t.TaskEntity.RunType = cGlobalParas.TaskRunType.OnlyGather;
                        //            t.TaskEntity.ExportFile = "";
                        //            t.TaskEntity.DataSource = "";
                        //            t.TaskEntity.ExportType =cGlobalParas.PublishType.NoPublish;
                        //            t.TaskEntity.DataSource = "";
                        //            t.TaskEntity.DataTableName = "";
                        //            t.TaskEntity.IsErrorLog = true;

                        //            //将获取任务的信息存储在采集任务备注中
                        //            t.TaskEntity.TaskDemo = getRemoteTask.ID + "," + getRemoteTask.TaskName + "," + getRemoteTask.GatherTaskType + "," + getRemoteTask.TaskFileName;

                        //            //处理图片路径的问题
                        //            for (int imgIndex = 0; imgIndex < t.TaskEntity.WebpageCutFlag.Count; imgIndex++)
                        //            {
                        //                if (t.TaskEntity.WebpageCutFlag[imgIndex].DataType == cGlobalParas.GDataType.File ||
                        //                    t.TaskEntity.WebpageCutFlag[imgIndex].DataType == cGlobalParas.GDataType.Picture)
                        //                {
                        //                    t.TaskEntity.WebpageCutFlag[imgIndex].DownloadFileSavePath = "";
                        //                }
                        //            }

                        //            t.Save(Program.getPrjPath() + Program.g_RemoteTaskPath + "\\", cGlobalParas.opType.Add, true);
                        //            t = null;

                        //            bool isUpdateSucc = false;
                        //            //if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriSplitTask)
                        //            //    isUpdateSucc=sweb.UpdateSplitTaskState(getRemoteTask.ID, Program.RegisterUser, (int)cGlobalParas.TaskState.RemoteUnstart, 0);
                        //            //else if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriTask)
                        //            //    isUpdateSucc=sweb.UpdateTaskState(getRemoteTask.ID, Program.RegisterUser, (int)cGlobalParas.TaskState.RemoteUnstart, 0);

                        //            if (isUpdateSucc == false)
                        //            {
                        //                if (e_ShowLogInfo != null)
                        //                    e_ShowLogInfo(this, new ShowInfoEvent("任务下载后更新状态失败" , Path.GetFileNameWithoutExtension(fName)));
                        //            }

                        //            //判断是否有排重库，如果有，则存入排重库
                        //            string urlDb = Program.getPrjPath() + "tmp\\" + Path.GetFileNameWithoutExtension(tmpFile)
                        //                + "\\远程-" + Path.GetFileNameWithoutExtension(tmpFile) + ".db";

                        //            if (File.Exists(urlDb))
                        //            {
                        //                //表示存在排重库
                        //                File.Copy(urlDb, Program.getPrjPath() + "urls\\" + Path.GetFileName(urlDb), true);
                        //            }
                        //            ////成功存储到本地
                        //            //if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.Normal)
                        //            //    sweb.UpdateTaskState(getRemoteTask.ID, Program.RegisterUser, (int)cGlobalParas.TaskState.RemoteUnstart);
                        //            //else if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.SplitTask)
                        //            //    sweb.UpdateSplitTaskState(getRemoteTask.ID, Program.RegisterUser, (int)cGlobalParas.TaskState.RemoteUnstart);
                        //         }
                        //         catch (System.Exception ex)
                        //         {
                        //             //重置状态
                        //             //if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriTask)
                        //             //    sweb.UpdateTaskState(getRemoteTask.ID, Program.RegisterUser, (int)cGlobalParas.TaskState.UnStart, 0);
                        //             //else if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriSplitTask)
                        //             //    sweb.UpdateSplitTaskState(getRemoteTask.ID, Program.RegisterUser, (int)cGlobalParas.TaskState.UnStart, 0);

                        //             //抛出错误
                        //             throw ex;

                        //         }
                        //    }
                        //    else
                        //    {
                        //        //表示索引文件已经存在此任务，将远程任务状态重置
                        //        //if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriTask)
                        //        //    sweb.UpdateTaskState(getRemoteTask.ID, Program.RegisterUser, (int)cGlobalParas.TaskState.RemoteUnstart, 0);
                        //        //else if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriSplitTask)
                        //        //    sweb.UpdateSplitTaskState(getRemoteTask.ID, Program.RegisterUser, (int)cGlobalParas.TaskState.RemoteUnstart, 0);

                        //    }

                        //    if (File.Exists(tmpFile))
                        //        File.Delete(tmpFile);

                        //    string tPath = Path.GetDirectoryName(tmpFile) + "\\" + Path.GetFileNameWithoutExtension(tmpFile);
                        //    if (Directory.Exists(tPath))
                        //        Directory.Delete(tPath,true);
                        //}
                        //getRemoteTask = null;
                        #endregion
                    }

                    //在此自动执行远程的采集任务，如果已经执行完毕，则自动上传

                    List<Int64> cCompleteTaskID = new List<long>();

                    //获取远程任务的名称
                    Dictionary<string, cGlobalParas.TaskState> rTask = new Dictionary<string, cGlobalParas.TaskState>();
                    #region 获取远程任务执行的状态
                    foreach (NetMiner.Core.gTask.Entity.eTaskIndex et in eIndexs)
                    {

                        bool isExist = false;
                        bool isExistComplete = false;    //此状态值是表示任务已经完成，但正在保存临时采集的数据，采集队列已经移到completed队列中，但还没保存到采集任务的taskcompleted中

                        //判断running的队列
                        for (int j = 0; j < m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count; j++)
                        {
                            if (m_GatherControl.TaskManage.TaskListControl.RunningTaskList[j].TaskName == et.TaskName)
                            {
                                isExist = true;
                                rCount++;
                                rTask.Add(et.TaskName, cGlobalParas.TaskState.Running);
                                break;
                            }
                        }

                        if (isExist == false)
                        {
                            //判断已经暂停的队列
                            for (int j = 0; j < m_GatherControl.TaskManage.TaskListControl.StoppedTaskList.Count; j++)
                            {
                                if (m_GatherControl.TaskManage.TaskListControl.StoppedTaskList[j].TaskName == et.TaskName)
                                {
                                    isExist = true;
                                    rCount++;
                                    rTask.Add(et.TaskName, cGlobalParas.TaskState.Stopped);
                                    break;
                                }
                            }
                        }

                        #region  判断已经完成的任务队列
                        if (isExist == false)
                        {

                            //开始判断是否已经执行结束
                            oTaskComplete tc = new oTaskComplete(Program.g_RemoteTaskPath);
                            IEnumerable<eTaskCompleted> eTasks = tc.LoadTaskData();

                            foreach (eTaskCompleted ec in eTasks)
                            {
                                if (ec.TaskName == et.TaskName)
                                {
                                    isExistComplete = true;
                                    //rTask.Add(xmlTasks.GetTaskName(i), cGlobalParas.TaskState.Completed);
                                    cCompleteTaskID.Add(ec.TaskID);
                                    break;
                                }
                            }
                            tc.Dispose();
                            tc = null;

                            if (isExistComplete == false)
                            {
                                //在此需要判断下是否在完成的队列中
                                bool isE = false;
                                for (int j = 0; j < m_GatherControl.TaskManage.TaskListControl.CompletedTaskList.Count; j++)
                                {
                                    if (m_GatherControl.TaskManage.TaskListControl.CompletedTaskList[j].TaskName == et.TaskName)
                                    {
                                        isE = true;
                                        rCount++;
                                        break;
                                    }
                                }
                                if (isE == false)
                                    rTask.Add(et.TaskName, cGlobalParas.TaskState.UnStart);
                                else
                                    rTask.Add(et.TaskName, cGlobalParas.TaskState.Running);
                            }
                            else
                                rTask.Add(et.TaskName, cGlobalParas.TaskState.Completed);
                        }

                        #endregion


                    }
                    #endregion

                    //在此执行采集任务
                    foreach (KeyValuePair<string, cGlobalParas.TaskState> kv in rTask)
                    {
                        if (kv.Value == cGlobalParas.TaskState.Running)
                        {
                            //可以不予理会。

                        }
                        else if (kv.Value == cGlobalParas.TaskState.Completed)
                        {
                            //处理已经完成的任务
                            #region 上传采集任务
                            for (int n = 0; n < cCompleteTaskID.Count; n++)
                            {
                                oTaskComplete tc = new oTaskComplete(Program.getPrjPath());
                                eTaskCompleted ec = tc.LoadSingleTask(cCompleteTaskID[n]);
                                string dFile = ec.TempFile;
                                string tName = ec.TaskName;
                                long uTaskID = ec.TaskID;
                                tc = null;

                                //获取任务的远程信息
                                oTask t = new oTask(Program.getPrjPath());
                                string FileName = Program.getPrjPath() + Program.g_RemoteTaskPath
                                    + "\\" + tName + ".smt";
                                t.LoadTask(FileName);
                                string TaskDemo = t.TaskEntity.TaskDemo;
                                t = null;

                                //localhost.cRemoteTaskEntity uRTask = new localhost.cRemoteTaskEntity();
                                string[] TaskDemos = TaskDemo.Split(',');
                                //uRTask.ID = int.Parse(TaskDemos[0]);
                                //uRTask.TaskName = TaskDemos[1];
                                //uRTask.GatherTaskType = int.Parse(TaskDemos[2]);
                                //uRTask.TaskFileName = TaskDemos[3];


                                //上传文件前先进行压缩处理
                                Dictionary<string, int> uFiles = new Dictionary<string, int>();
                                uFiles.Add(dFile, (int)cGlobalParas.FileType.File);

                                //获取日志文件
                                string logTask = Program.getPrjPath() + "log\\task" + tName + ".csv";
                                if (File.Exists(logTask))
                                    uFiles.Add(logTask, (int)cGlobalParas.FileType.File);

                                //获取排重库文件
                                string dbTask = Program.getPrjPath() + "urls\\远程-" + tName + ".db";
                                if (File.Exists(dbTask))
                                    uFiles.Add(dbTask, (int)cGlobalParas.FileType.File);

                                //获取下载图片的目录
                                string imgPath = Program.getPrjPath() + "data\\" + tName + "_file";
                                if (Directory.Exists(imgPath))
                                    uFiles.Add(imgPath, (int)cGlobalParas.FileType.Directory);

                                string tmpFile = Program.getPrjPath() + "tmp\\" + tName + ".zip";
                                NetMiner.Common.Tool.cZipCompression zCompress = new NetMiner.Common.Tool.cZipCompression();
                                zCompress.CompressZIP(uFiles, tmpFile);

                                //上传数据
                                //修改成分批传输，每次传输10MB的数据

                                FileStream fs = new FileStream(tmpFile, FileMode.Open, FileAccess.Read);
                                BinaryReader br = new BinaryReader(fs);
                                byte[] bytes = br.ReadBytes((int)fs.Length);
                                fs.Flush();
                                fs.Close();

                                int fileLength = bytes.Length;
                                int UploadCount = fileLength / UploadFileLength;
                                if ((fileLength % UploadFileLength) > 0)
                                    UploadCount++;
                                int startIndex = 0;
                                for (int i = 0; i < UploadCount; i++)
                                {
                                    //复制字节数
                                    byte[] tmpBytes = null;
                                    int byteLen = 0;
                                    if ((startIndex + UploadFileLength) < fileLength)
                                        byteLen = UploadFileLength;
                                    else
                                        byteLen = fileLength - startIndex;

                                    tmpBytes = new byte[byteLen];
                                    Array.Copy(bytes, startIndex, tmpBytes, 0, byteLen);

                                    //sweb.PushTaskResult(Program.RegisterUser, tmpBytes, tName, startIndex);

                                    startIndex += UploadFileLength;

                                }

                                //数据上传结束后，马上调用远程服务，进行数据处理
                                //sweb.DealUploadZIP(Program.RegisterUser, tName, uRTask.ID, uTaskID, uRTask.GatherTaskType);

                                //清理本地数据
                                //CloseTabBySilent(cCompleteTaskID[n], tName);

                                InvokeMethod(this, "CloseTabBySilent", new object[] { cCompleteTaskID[n], tName });

                                //删除taskcompleter
                                tc = new oTaskComplete(Program.getPrjPath());
                                tc.LoadTaskData();
                                tc.DelTask(cCompleteTaskID[n]);
                                tc.Dispose();
                                tc = null;

                                //删除数据文件
                                File.Delete(dFile);

                                File.Delete(dbTask);

                                //删除日志文件
                                File.Delete(logTask);

                                //删除图片文件
                                if (Directory.Exists(imgPath))
                                    Directory.Delete(imgPath, true);

                                File.Delete(tmpFile);

                                //删除remoteclass中的文件，表示已经运行
                                t = new oTask(Program.getPrjPath());
                                t.DeleTask(Program.getPrjPath() + Program.g_RemoteTaskPath, tName);
                                t = null;

                                //删除队列中的数据
                                cGatherTask gt = m_GatherControl.TaskManage.FindTask(uTaskID);
                                if (gt != null)
                                    this.m_GatherControl.TaskManage.TaskListControl.CompletedTaskList.Remove(gt);
                                gt = null;
                            }
                            #endregion
                        }
                        else if (kv.Value == cGlobalParas.TaskState.UnStart && rCount < m_MaxRemoteCount)
                        {
                            #region 启动远程采集任务
                            //启动任务，启动任务前，先将任务的发布类型置为空


                            cGatherTask gt = null;
                            string tClassName = Program.g_RemoteTaskClass;
                            string tClassPath = NetMiner.Constant.g_RemoteTaskPath;
                            gt = AddRunTask(tClassName, tClassPath, kv.Key);
                            if (gt == null)
                            {
                                //表示启动任务被用户中断，也有可能是因为错误造成
                                return;
                            }

                            Int64 TaskID = gt.TaskID;
                            //AddTab(TaskID, kv.Key);
                            InvokeMethod(this, "AddTab", new object[] { TaskID, kv.Key });
                            //启动此任务
                            m_GatherControl.Start(gt);
                            gt = null;
                            #endregion
                        }
                        else if (kv.Value == cGlobalParas.TaskState.UnStart && rCount < m_MaxRemoteCount)
                        {
                            //继续运行此任务

                        }

                    }

                    xmlTasks = null;


                }
                catch (System.Exception ex)
                {
                    if (e_ExportLog != null)
                        e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error, cGlobalParas.LogClass.System,
                            System.DateTime.Now.ToString(), ex.Message));

                }
            }
        }

                #endregion

                //m_IsDo = false;

                //    }
                //}

                #endregion

        private void rmenuOverTask_Click(object sender, EventArgs e)
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            if (this.dataTask.SelectedCells[3].Value.ToString() == "nodRunning")
            {
                //强制结束正在运行的采集任务
                OverMultiTask();
            }
            else if (this.dataTask.SelectedCells[3].Value.ToString() == "nodPublish")
            {
                //强制结束正在发布的采集任务
                OverMultiPublish();
            }
        }

        private void OverMultiTask()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {
                if ((cGlobalParas.TaskState)this.dataTask.SelectedRows[index].Cells[2].Value == cGlobalParas.TaskState.Failed)
                {
                    continue;
                }
                else
                {
                    OverTask(Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString()));
                }
            }
        }

        private void OverMultiPublish()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {
                if ((cGlobalParas.TaskState)this.dataTask.SelectedRows[index].Cells[2].Value == cGlobalParas.TaskState.Failed)
                {
                    continue;
                }
                else
                {
                    OverPublish(Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString()), this.dataTask.SelectedRows[index].Cells[4].Value.ToString());
                }
            }
        }

        private void OverTask(Int64 TaskID)
        {
            cGatherTask t = null;

            t = m_GatherControl.TaskManage.FindTask(TaskID);

            if (t == null)
                return;

            //停止此任务
            m_GatherControl.Over(t);

        }

        private void OverPublish(Int64 TaskID, string TaskName)
        {
            //先把下载的数据打开
            BrowserMultiData();


            cPublish pt = null;


            //执行正在执行的任务
            pt = m_PublishControl.PublishManage.FindTask(TaskID);

            if (pt == null)
                return;

            //停止此任务
            m_PublishControl.OverPublish(pt);

            //任务启动成功显示消息
            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent("发布已结束！", TaskName));

            //}
        }

        private void rMenuRefer_Click(object sender, EventArgs e)
        {
            LoadRunTask();
        }

        private void ReferShow(TreeNode eNode)
        {
            try
            {
                LoadRunTask();
            }
                    
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString("Info54") + Program.getPrjPath() + "tasks\\taskrun.xml", rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }


        }

  

        private void ClearRemoteTask()
        {
            oTaskIndex xmlTasks = new oTaskIndex(Program.getPrjPath());
            //这是一个特殊的默认分类，特指从服务器下载的采集任务
            IEnumerable< NetMiner.Core.gTask.Entity.eTaskIndex> eIndexs= xmlTasks.GetTaskDataByClass(NetMiner.Constant.g_RemoteTaskClass);

            //开始初始化此分类下的任务
            //int count = xmlTasks.GetTaskClassCount();

            foreach(NetMiner.Core.gTask.Entity.eTaskIndex et in eIndexs)
            {
                //开始逐一删除任务
                oTask t = new oTask(Program.getPrjPath());

                oTaskClass tc = new oTaskClass(Program.getPrjPath());
                string tPath = tc.GetTaskClassPathByName(Program.g_RemoteTaskClass);
                tc = null;
                t.DeleTask(tPath,et.TaskName);
                t = null;

                //判断此任务在正在运行的队列中是否存在，如果存在，停止，删除
            }
            xmlTasks = null;


        }

        private string GetTaskFullClassName(TreeNode eNode)
        {
            string cName = string.Empty;


            if (eNode.Name == "nodTaskClass")
            {
                return "";
            }
            else if (eNode.Parent.Name =="nodTaskClass")
            {
                return eNode.Text;
            }
            else
            {
                cName=GetTaskFullClassName(eNode.Parent) + "/" + eNode.Text;
            }

            return cName;
        }

        public void StartRadar()
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Program ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                //重新加载一遍雷达数据信息

                //LoadRadarRule();

                m_RadarControl.StartRadar();
            }
            else
            {
                MessageBox.Show("当前版本不支持网络雷达（数据监控）功能，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.toolmenuSilentRun.Checked = false;
                return;
            }
        }

        public void StopRadar()
        {
            m_RadarControl.StopRadar();
        }

   
    }
}
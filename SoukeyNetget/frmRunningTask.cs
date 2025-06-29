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

        //ÿ���ϴ��ļ�������
        private const int UploadFileLength = 5120000;

        private bool IsTimer = true;

        //�Ƿ񱣴�ϵͳ��־���,Ĭ�ϲ�����
        private bool m_IsAutoSaveLog = false;

        //����һ��ֵ�жϵ�ǰ�Ƿ����ֹ��������������ڽ���
        private bool m_IsExportData = false;

        //����һ��ֵ�����ڼ�¼�ֹ�������¼��tabҳ����һ������ǰ�������ֹ�ͬʱ����һ�����ݼ�
        private string m_ExportPageName;
        
        //����һ��ֵ����ʾ��ǰ�ļƻ�ִ�еļ������Ƿ�������
        private bool m_IsRunListen = false;

        //����һ��ֵ����¼��ǰ��־��������������ϵͳ�������������Ҫ�ǿ��ǵ���־�����ܵ�Ӱ��
        private int m_MaxLogNumber;

        //����һ��ֵ����¼�������ִ��Զ�̲ɼ����������
        private int m_MaxRemoteCount;

        private cRadarControl m_RadarControl;

        ToolTip HelpTip = new ToolTip();

        //����һ����ʱ����������Զ�̷���������ͨѶ��������
        //��ǰ���������ж��Ƿ���ȡ�ɼ�����������ݲɼ�����
        private System.Threading.Timer m_RemoteEngine;
        private bool m_IsAllowRemoteGather;
        private string m_RemoteServer;
        private bool m_IsDo = false;

        //����һ��ֵ�����ڴ洢���ϴ��ɼ�����ʱѡ���Զ�����������
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

            //�ж��Ƿ��Ѿ�û��ѡ������û�У�������tab��ʾ
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

                //���ð�ť״̬
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

            //�ж��Ƿ�Ϊ�����������������Ҫ�����ϷŲ���
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
        /// ����ִ�вɼ����񣬼���ǰ�ɼ�����ֹͣ��tabҳ��Ҳδ�رյ�����£�����ִ��
        /// </summary>
        /// <param name="TaskID"></param>
        public void StartTask(Int64 TaskID)
        {
            cGatherTask t = null;

            t = m_GatherControl.TaskManage.FindTask(TaskID);
            if (t == null)
            {
                //��ʾ��������ʧ��
                return;
            }

            //�ڴ��ж��Ƿ���Ҫ�����ⲿ�����Ĵ��ݣ������Ҫ���fInputPara
            //��������Ѿ����У����ʾ�����е���ַ�Ѿ��ֽ������
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
                //����������
                m_GatherControl.Start(t);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("�ɼ���������ʧ�ܣ��������ڲɼ��������÷����˴�������ɼ���ַ���ɼ������Ƿ��Ѿ����ã��绹�޷��������������󹤿ͷ���Ա��ϵ��", "����� ������Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            //���������ɹ���ʾ��Ϣ
            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("TaskStarted"),t.TaskName ));

            t = null;
        }

        /// <summary>
        /// �����ɼ���������������һ������
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="TaskName"></param>
        /// <param name="SelectedIndex"></param>
        public void StartTask(Int64 TaskID, string TaskName,string TaskClass,string TaskPath)
        {
            cGatherTask t = null;

            //�жϵ�ǰѡ������ڵ�
      
            ///�����ѡ����������ڵ㣬����˰�ť�����Ƚ���������ص���������Ȼ�����
            ///starttask��������������
            string tClassName = TaskClass;
            string tClassPath = TaskPath;

            t = AddRunTask(tClassName,tClassPath, TaskName);

            //����������������򴫽�����TaskID������ı�ţ�����������ִ�еı�ţ���������ʱ���Զ�����������
            //��һ�����������������ԣ���Ҫ���¸��´����TaskID
    
            if (t == null)
            {
                //��ʾ��������ʧ��
                return;
            }

            TaskID = t.TaskID;

            //�ڴ��ж��Ƿ���Ҫ�����ⲿ�����Ĵ��ݣ������Ҫ���fInputPara
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

            //����ɹ���������Ҫ����TabPage������ʾ���������־���ɼ����ݵ���Ϣ
            AddTab(TaskID, TaskName);

            //���б�������һ������
            this.dataTask.Rows.Add(imageList1.Images["run"], TaskID, cGlobalParas.TaskState.Running, tClassPath,
                             tClassName, TaskName, (int)cGlobalParas.TaskProcess.Gather,
                             System.DateTime.Now.ToString("MM-dd HH:mm"), t.UrlCount ,
                             "0", "0", 0 / t.UrlCount);

            try
            {
                //����������
                m_GatherControl.Start(t);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("�ɼ���������ʧ�ܣ��������ڲɼ��������÷����˴�������ɼ���ַ���ɼ������Ƿ��Ѿ����ã��绹�޷��������������󹤿ͷ���Ա��ϵ��", "����� ������Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            //���������ɹ���ʾ��Ϣ
            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("TaskStarted"),TaskName ));

            t = null;
        }

        /// <summary>
        /// �����ɼ����񣬼���ִ���Ѿ�ֹͣ�Ĳɼ����񣬼��ɼ�����ֹͣ��Ҳ�ر�����ص���Դ��Tabҳ�棬��־�����ݣ�
        /// </summary>
        private void StartTask(Int64 TaskID, string TaskName, int SelectedIndex)
        {
            cGatherTask t = null;

            //�жϵ�ǰѡ������ڵ�
           
                //ִ������ִ�е�����
            t = m_GatherControl.TaskManage.FindTask(TaskID);
            

            if (t == null)
            {
                //��ʾ��������ʧ��
                return;
            }

            //�ڴ��ж��Ƿ���Ҫ�����ⲿ�����Ĵ��ݣ������Ҫ���fInputPara

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

            //����ɹ���������Ҫ����TabPage������ʾ���������־���ɼ����ݵ���Ϣ
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
                //����������
                m_GatherControl.Start(t);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("�ɼ���������ʧ�ܣ��������ڲɼ��������÷����˴�������ɼ���ַ���ɼ������Ƿ��Ѿ����ã��绹�޷��������������󹤿ͷ���Ա��ϵ��", "����� ������Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            //���������ɹ���ʾ��Ϣ
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
        /// ������������
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

            //��ɾ�����еģ��ٽ��з���
            cPublish pt = m_PublishControl.PublishManage.FindTask(TaskID);
            m_PublishControl.Remove(pt);
            pt = null;

            pt = new cPublish(Program.getPrjPath(),m_PublishControl.PublishManage, TaskID, d);
            m_PublishControl.startPublish(pt);

            //���������ɹ���ʾ��Ϣ
            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent("������������",TaskName));
        }

        #region �Զ���ӿؼ�������ʾ����ִ�еĽ��

        /// <summary>
        /// �������������ʱ��������ʵ��Tab
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="TaskName"></param>
        public void AddTab(Int64 TaskID, string TaskName)
        {

            if (this.splitContainer2.Panel2Collapsed == true)
                this.splitContainer2.Panel2Collapsed = false;

            bool IsExist = false;
            int j = 0;

            //�жϴ������Ƿ��Ѿ������Tabҳ
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

            //�����������Ƶ���Ϣ
            tPage.Tag = TaskName;

            tPage.Text = TaskName;
            tPage.ImageIndex = 19;
            

            SplitContainer sc = new SplitContainer();
            sc.Name = "sCon" + TaskID.ToString();
            sc.Orientation = Orientation.Horizontal;
            sc.Dock = DockStyle.Fill;

            //����һ��grid������ʾ�ɼ��õ�������
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

             //����һ����������������ʾ��������ʱ�Ľ�����Ϣ
            ProgressBar p = new ProgressBar();
            p.Name = "tPro" + TaskID.ToString();
            p.Dock = DockStyle.Bottom;
            p.Visible = false;
            sc.Panel2.Controls.Add(p);

            //����һ���ı���������ʾ�ɼ������־
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
            //������ʾ����
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

        //����tabҳ����Ҫ���������ҳ����¼�
        private void TextLogLinkClick(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void Tab_MouseDown(object sender, MouseEventArgs e)
        {
            //��ȡ��ѡ��
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

        //ͨ���û���¼��ȡcookie
        private string m_Cookie;
        private string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }

        //ͨ���û������ȡUrl��������ƴ�ӳɱ�׼��Url
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
                e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("LogSave"), rm.GetString("Task") + "��" + this.tabControl1.SelectedTab.Text + " " + rm.GetString("LogSaveSuccess")));

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
        /// ���Ѿ��ɼ���ɵ�����
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

            //���ط�������Ͳ������Ĳ˵�
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
            //�����жϵ�ǰ��Ҫ�������ݵ�Tab��datagridview
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

            //�ж��Ƿ��Ѿ�û��ѡ������û�У�������tab��ʾ
            if (this.tabControl1.TabPages.Count ==0)
                this.splitContainer2.Panel2Collapsed = true ;
        }

        //���ü��ش����gridrows����ʵ��ʽ
        private void SetRowErrStyle()
        {
            this.m_RowStyleErr = new DataGridViewCellStyle();
            this.m_RowStyleErr.Font = new Font(DefaultFont, FontStyle.Italic);
            this.m_RowStyleErr.ForeColor = Color.Red;
        }

         ///���ݲ˵�ѡ�񼰵�����������ݣ��Զ����ƹ������İ�ť״̬
        ///��ΪDataGridView֧�ֶ�ѡ�����Կ��ܴ��ڶ��ְ�ť״̬�����
        ///������������ϵͳ�������ѡ������ݽ��а�ť״̬����

        private void SetFormToolbarState()
        {
            if (e_SetToolbarState != null && this.dataTask.SelectedRows.Count !=0)
            {
                e_SetToolbarState(this, new SetToolbarStateEvent(this.dataTask.Rows.Count, (cGlobalParas.TaskState)this.dataTask.SelectedCells[2].Value));
            }
                 
        }

        //������ʾ��������Datalistview���б�ͷ
        private void ShowRunTask()
        {
            this.Text = "��������";
            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {
            }

            #region �˲���Ϊ�̶���ʾ �������͵����񶼱���̶���ʾ����
            DataGridViewImageColumn tStateImg = new DataGridViewImageColumn();
            tStateImg.HeaderText = rm.GetString("GridState");
            tStateImg.Width = 40;
            tStateImg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tStateImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(0, tStateImg);

            //������,����ʾ����
            DataGridViewTextBoxColumn tID = new DataGridViewTextBoxColumn();
            tID.Name = rm.GetString("GridTaskID");
            tID.Width = 0;
            tID.Visible = false;
            this.dataTask.Columns.Insert(1, tID);

            //����״̬,����ʾ����
            DataGridViewTextBoxColumn tState = new DataGridViewTextBoxColumn();
            tState.Name = rm.GetString("GridState");
            tState.Width = 0;
            tState.Visible = false;
            this.dataTask.Columns.Insert(2, tState);

            //����ͨ���ж�Datagridview�����ݾͿ�֪����ǰ���νṹѡ��Ľڵ�
            //���ڿ���(����)������ʾ״̬
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
            curState.HeaderText = "�ɼ�����";
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
            GatheredUrlCount.HeaderText = "��ַ��";
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
            tUrlCount.HeaderText = "������";
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

            //���ҵ�ǰ���б���ʾ������
            //�����жϵ�ǰѡ�е����νڵ��Ƿ����������Ľڵ�
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

   
     

        #region ����ѡ�е����β˵���Ϣ��������
    
        //��������ִ�е���������ִ�е������¼��Ӧ�ó���Ŀ¼�µ�RunningTask.xml�ļ���
        public void LoadRunTask()
        {
          

            ShowRunTask();

            //��ʼ��ʼ���������е�����
            //��m_TaskControl�ж�ȡ
            //ÿ�μ��ػ�����������С��ȴ���ֹͣ�����е�����
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

                    //��ʼ����Tooltip��Ϣ
                    StringBuilder sb = new StringBuilder();
                    sb.Append("�������ࣺ" + taskList[i].TaskData.TaskClass + "\r\n");
                    sb.Append("ִ�����ͣ�" + taskList[i].TaskData.RunType.GetDescription() + "\r\n");
                    sb.Append("�ɼ�������" + taskList[i].TaskData.GatherDataCount + "\r\n");
                    this.dataTask[0, dataTask.Rows.Count - 1].ToolTipText = sb.ToString();
                    //HelpTip.SetToolTip(this.dataTask.Rows[dataTask.Rows.Count - 1].Cells[1], @"�����������ƣ��������ƿ�����"


                }
                catch (System.Exception ex)
                {
                    //������󣬲�����������Ϣ��������
                    if (e_ExportLog != null)
                        e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error,cGlobalParas.LogClass.Task ,System.DateTime.Now.ToString (), ex.Message));

                }
            }

            this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

            this.dataTask.ClearSelection();

            taskList = null;

        }

      
  

        //�˲��ֵ������Ǹ��ݵ�ǰ�Ѿ���ɵĵ�������
        //ʵʱ����������
        public void LoadExportDataTask(TreeNode eNode)
        {
            //this.myListData.Items.Clear();
        }

        //�������񣬿��Լ���������Ϣ���������е�����
        //ע�⣬������ص����������������򲻿��Խ����������������
        //��ǰ�жϵ������ǣ�����Ǳ༭�������ж�����汾�Ƿ���ϵ�ǰ�汾
        //Լ����������ǣ�����������������������������������Ƿ�����
        //�汾Լ��������������������

        //�˷�����������������ֻ��ͨ��ԭ�еĽ������
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

        #region ɾ����һЩ����

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

        //    //�����ʾ
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
        /// ɾ���������е�����
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

                //ɾ��taskrun�ڵ�
                oTaskRun tr = new oTaskRun(Program.getPrjPath());
                tr.DelTask(TaskID);
                tr = null;

                ////ɾ���Ѿ����ص��ɼ�����������е�����
                //m_GatherControl.TaskManage.TaskListControl.DelTask(t);

                //ɾ��run�е�����ʵ���ļ�
                string FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".rst";
                System.IO.File.Delete(FileName);

                //ɾ��run�е�����ʵ�����ؿ��ļ�
                FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".db";
                System.IO.File.Delete(FileName);

                //ɾ��run�еĲɼ����������ļ�
                FileName = Program.getPrjPath() + "Tasks\\run\\" + "data" + TaskID + ".db";
                System.IO.File.Delete(FileName);

                //ɾ��run�е�����ʵ���ļ�
                FileName = Program.getPrjPath() + "data\\" + TaskName + "-" + TaskID + ".xml";
                System.IO.File.Delete(FileName);

                tr = null;

            }

            return true;

            //ɾ��Datagridview��ѡ�е�����

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
        /// �����Ѿ����еĲɼ����񣬲����Ѿ��ɼ�������
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

                //�ж����������Щ���ݣ��������л��ǲɼ����

            
                    oTaskRun tr = new oTaskRun(Program.getPrjPath());
                    eTaskRun er= tr.LoadSingleTask(TaskID);

                    if (er != null)
                        dFile = er.TempFile;
                    else
                        //�����ʱ����Ϊ�գ���ʾ����ִ������Ѿ�ת��������б�
                        //�����滹δˢ�£������������
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
                        //�п����ڱ�������ʱ�����˴��������Ҫ���Դ���ֱ�ӽ���,����Ҫͨ��ϵͳ��Ϣ��ʾ����
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

        

        //����һ�����������޸����������
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

        //����һ�����������޸ļƻ�������
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
        			
                    //��������������������soukeydata�������ͣ������ã���Ϊ�˱�֤ϵͳ������������//
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
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error, cGlobalParas.LogClass.Task, DateTime.Now.ToString(),"�ɼ�ҳ���Ѿ��رգ����µ��������־ʧ�ܣ�"));
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

       
            //ִ������ִ�е�����
            pt = m_PublishControl.PublishManage.FindTask(TaskID);

            if (pt == null)
                return;

            //ֹͣ������
            m_PublishControl.StopPublish(pt);

            //���������ɹ���ʾ��Ϣ
            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent("������ֹͣ��",TaskName));

            //}
        }

        #region ������� ���� ֹͣ
        //�����ɼ�����
        /// <summary>
        /// ���ɼ��������������ж��У�����������·��
        /// </summary>
        /// <param name="tClassName"></param>
        /// <param name="tClassPath"></param>
        /// <param name="tName"></param>
        /// <returns></returns>
        private cGatherTask AddRunTask(string tClassName, string tClassPath, string tName)
        {

            //��ѡ���������ӵ�������
            //�����жϴ������Ƿ��Ѿ���ӵ�������,
            //����Ѿ���ӵ�����������Ҫѯ���Ƿ�����һ������ʵ��
            bool IsExist = false;
            //cGlobalParas.TaskType tType = cGlobalParas.TaskType.HtmlByUrl;

            //��ʼ��ʼ���������е�����
            oTaskRun tr;
            try
            { 
                tr = new oTaskRun(Program.getPrjPath());
            }
            catch(System.IO.IOException ex)
            {
                MessageBox.Show("��������ʧ�ܣ����������������������϶�������ͻ�����Ժ����ԣ�", "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            catch(System.Exception ex1)
            {
                MessageBox.Show("��������������" + ex1.Message, "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            //��ȡ����ִ��ID
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

                //�������������
                m_GatherControl.AddGatherTask(tData);

                tData = null;

                //������ӵ���������,��Ҫ����ӵ�����ִ���б���
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
            //�������ð汾�Ĵ���
            frmTrialInfo f = new frmTrialInfo();
            f.Show();

            m_GatherControl.Stop();
        }

        private void StopTask(Int64 TaskID)
        {
            cGatherTask t = null;

            ///2013-1-14 �޸ģ�ȥ��������β˵�ѡ����жϣ�ɾ��
            ///�˵�ֻҪ�����ã��Ϳ���ɾ����Ϊ�˼���������ɾ������
            //�жϵ�ǰѡ������ڵ�
            //if (this.dataTask.SelectedCells[3].Value .ToString () == "nodRunning")
            //{
                //ִ������ִ�е�����
                t = m_GatherControl.TaskManage.FindTask(TaskID);

                if (t == null)
                    return;

                //ֹͣ������
                m_GatherControl.Stop(t);

                //���������ɹ���ʾ��Ϣ
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
            this.saveFileDialog1.Title = "�����뵼��word���ļ���";
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
        /// �������ڸ��µ�ǰ����Ľ�����
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

            //�����ݿ�������
            DataTable d = ((DataTable)tmp.DataSource).Copy();

            m_ExportPageName = this.tabControl1.SelectedTab.Name;

            ExportGatherLog(this.tabControl1.SelectedTab.Name, this.tabControl1.SelectedTab.Controls[0].Name, cGlobalParas.LogType.Info, rm.GetString ("Info237") + "\n");
            ExportGatherLog(this.tabControl1.SelectedTab.Name, this.tabControl1.SelectedTab.Controls[0].Name, cGlobalParas.LogType.Info, FileName + "\n");

            //����һ����̨�߳����ڵ������ݲ���
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

        //�ֶ���������ͬһʱ��ֻ�ܵ���һ�����񣬲��ܽ��ж���������ݵ���
        private bool IsExportData()
        {
            if (this.m_IsExportData == false )
            {
                return false;
            }
            return true;
        }

        //��ʼ���ɼ�����
        public void IniData()
        {
            SetRowErrStyle();


            this.e_PublishByPluginEvent += this.publishByPluginEvent;
            this.e_PublishByRuleEvent += this.publishByRuleEvent;


            //��ʼ��һ���ɼ�����Ŀ�����,�ɼ������ɴ˿�����������ɼ�����
            //����
            NetMiner.Base.cHashTree tmpUrls = null;
            m_GatherControl = new cGatherControl(Program.getPrjPath(),false,ref tmpUrls);

            //�ɼ��������¼���,�󶨺�,ҳ�������Ӧ�ɼ����������¼�
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

            

            //����������������,��������������Ҫ�Ǹ���taskrun.xml(Ĭ����Tasks\\TaskRun.xml)�ļ���
            //�����ݽ��м���,
            cTaskDataList gList = new cTaskDataList();

            //���ݼ��ص���������������Ϣ,��ʼ��ʼ���ɼ�����
            try
            {
                gList.LoadTaskRunData(Program.getPrjPath());

                //�ڴ����Ӳɼ������е�����
                bool IsAddRTaskSucceed = m_GatherControl.AddGatherTask(gList);

                if (IsAddRTaskSucceed == false)
                    MessageBox.Show(rm.GetString("Error23"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Error15") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            //��ʼ�������ڵ�����������Ϣ
            m_PublishControl = new cPublishControl(Program.getPrjPath());

            //ע�ᷢ��������¼�
            m_PublishControl.PublishManage.PublishCompleted += this.Publish_Complete;
            m_PublishControl.PublishManage.PublishError += this.Publish_Error;
            m_PublishControl.PublishManage.PublishFailed += this.Publish_Failed;
            m_PublishControl.PublishManage.PublishStarted += this.Publish_Started;
            //m_PublishControl.PublishManage.PublishTempDataCompleted += this.Publish_TempDataCompleted;
            m_PublishControl.PublishManage.PublishLog += this.Publish_Log;
            m_PublishControl.PublishManage.UpdateState += this.Publish_UpdateState;
            m_PublishControl.PublishManage.PublishStop += this.Publish_Stop;
            m_PublishControl.PublishManage.DoCount += this.Publish_DoCount;

            //�ڴ����ӷ��������е�����
            m_PublishControl.AddPublishTask(gList);

            //����ѡ��ġ��������С����νڵ㣬������Ӧ����Ϣ
            try
            {
                LoadRunTask();
            }
           
            catch (System.Exception)
            {
                MessageBox.Show(rm.GetString("Error16"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

            SetDataShow();

            //�����Ƿ��Զ�����ϵͳ��־��־
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
                //��ʾ�����ļ���������Ҫ��������
            }

            //����ʱ�������ڸ���������ʾ�Ľ���
            //this.timer1.Enabled = false;

            //����״̬����Ϣ
            UpdateStatebarTask(0,cGlobalParas.TaskState.Request);

            if ((Program.SominerVersion == cGlobalParas.VersionType.Cloud && m_IsAllowRemoteGather == true) ||
                (Program.SominerVersion == cGlobalParas.VersionType.Enterprise && m_IsAllowRemoteGather == true))
            {
                m_RemoteEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_RemoteEngine_CallBack),
                    null, 60000, 60000);
            }

        }

        #region �������������� ֹͣ ��Ӧ�¼�

        /// ���������������ڼ����ƻ������Ƿ����ִ��
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

        //�������������¼����ɼ���������ͨ���¼����д���
        private void On_RunSoukeyTask(object sender, cRunTaskEventArgs e)
        {

            string tClassName = e.RunName.Substring(0, e.RunName.LastIndexOf("\\"));
            string TaskName = e.RunName.Substring(e.RunName.LastIndexOf("\\") + 1, e.RunName.Length - e.RunName.LastIndexOf("\\") - 1);

            string tClassPath ="tasks\\" + tClassName.Replace("/","\\");

            cGatherTask t = AddRunTask(tClassName, tClassPath, TaskName);

            if (t == null)
                return;

            Int64 TaskID = t.TaskID;

            //����Tab��ǩ
            InvokeMethod(this, "AddTab", new object[] { TaskID, TaskName });


            InvokeMethod(this, "AddDataGridRow", new object[] { TaskID, tClassPath,tClassName, TaskName,t.UrlCount });

            //����������
            m_GatherControl.Start(t);

            //���������ɹ���ʾ��Ϣ
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

        #region �¼�����

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
            //����ִ����Ϻ���Ҫ�����������Ѿ���ɵĽڵ��У�
            //�ڴ����ѡ�����nodRunning��ɾ��datagridview������
            //Ȼ����ӵ���ɶ�����

            try
            {
                cGatherTask t = (cGatherTask)sender;

                if (e_ShowLogInfo != null)
                    e_ShowLogInfo(this, new ShowInfoEvent(rm.GetString("TaskGCompleted"),e.TaskName));

                //������ɺ������Ƿ񷢲������ô˷�������ΪҪ������ʱ���ݱ���
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

        //����ɼ��������ʱ����
        public void SaveGatherTempData(Int64 TaskID)
        {
            

            //����������ӵ�����������

            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            if (this.tabControl1.TabPages[pageName] == null)
            {
                return;
            }

            string strLog = "��ʼ���вɼ����ݵı��棬�Ժ���ڡ��Ѿ���ɵ�������Ŀ�в鿴�Ѿ����ص����ݣ���ȴ�......";

            InvokeMethod(this, "ExportGatherLog", new object[] { pageName, conName, cGlobalParas.LogType.Info, strLog });

            DataTable d = (DataTable)((DataGridView)this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0]).DataSource;

            StartSaveTemp(TaskID, d);

        }


        public void SavePublishTempData(Int64 TaskID)
        {
            //if (Program.SominerVersion == cGlobalParas.VersionType.Trial)
            //    return;

            //������ʱ����
            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;


            string strLog = "��ʼ���з���״̬���ݵı��棬�Ժ����ɲ鿴���ݵķ��������Ϣ����ȴ�......";

            InvokeMethod(this, "ExportGatherLog", new object[] { pageName, conName, cGlobalParas.LogType.Info, strLog });


            DataTable d = (DataTable)((DataGridView)this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0]).DataSource;

            StartSaveTemp(TaskID, d);
        }

        //��������ɼ���ɵĹ�����ע�����������Ƿ񷢲���Ҫִ��
        //�˷�������Ϊ���������Ƿ񷢲�����Ҫ������ʱ���ݱ���
        //������������򲻽������ݷ��������������
        public void UpdateTaskPublish(Int64 TaskID, bool IsDelRepRow)
        {
            //����������ӵ�����������

            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            if (this.tabControl1.TabPages[pageName] == null)
            {
                //��ʾ�ɼ�������û�ж���ɼ�����ģ�
                //���ԣ��ᵼ������ֱ����ɣ���δ����
                //���������tabҳ
                UpdateTaskComplete(TaskID,0);
            }
            else
            {
                //��ʾ�ɼ�������
                DataTable d;

                if (IsDelRepRow == true)
                {
                    //ȥ���ظ���
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
                        //����ǲ��������ݣ�����Ҫͨ���˷����������ɼ���Ĵ�����
                        //ɾ��taskrun�е����ݵȵȣ��˲��������ʵ�����������ݷ�������
                        //�����񷢲������󴥷��¼���ɣ����ڴ���Ҫ�ֹ����
                        UpdateTaskComplete(TaskID,rowCount);
                    }
                    else
                    {
                        ExportGatherLog(pageName, conName, cGlobalParas.LogType.Info, "�����������˷�����������ʼ�������ݷ���");

                        //�޸�TaskRun�е�״̬
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

            #region ���½�����ʾ
          
         
                    for (int i = 0; i < this.dataTask.Rows.Count; i++)
                    {
                        if (this.dataTask.Rows[i].Cells[1].Value.ToString() == TaskID.ToString())
                        {
                            //ֻ�з��������񣬲Ż�ɾ�������Ը��½���ʱ��Ҫ�ж����������
                            this.dataTask.Rows.Remove(this.dataTask.Rows[i]);
                            break;
                        }

                    }
                
               
              
            
            #endregion

        }


        //��������ɼ���ɺ�Ĺ������������ѡ����������еĽڵ㣬��
        //ɾ���˽ڵ�,Ȼ���taskrun������ɾ��,Ȼ����ɾ��ʵ�ʵ��ļ�
        public void UpdateTaskComplete(long TaskID ,int rowCount)
        {
            //�ж������Ƿ�Ϊ�������������������������ɾ��taskrun�е���Ϣ
            //��Ϊ����������Զ�����ʱ

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
            
            //���Ѿ���ɵ�������ӵ��������������ļ���
            oTaskComplete tc = new oTaskComplete(Program.getPrjPath());


            tc.InsertTaskComplete(ec, cGlobalParas.GatherResult.GatherSucceed);
            tc.Dispose();
            tc = null;

            //ɾ��taskrun�ڵ�

            tr.LoadTaskRunData();
            tr.DelTask(TaskID);

            //ɾ��run�е�����ʵ���ļ�

            oTask t = new oTask(Program.getPrjPath());
            t.LoadTask(TaskID);
            isCloseTab = t.TaskEntity.isCloseTab;
            t.Dispose();
            t = null;

            string FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".rst";
            System.IO.File.Delete(FileName);

            //ɾ��run�е�����ʵ�����ؿ��ļ�
            FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".db";
            System.IO.File.Delete(FileName);

            //ɾ��run�еĲɼ����������ļ�
            FileName = Program.getPrjPath() + "Tasks\\run\\" + "data" + TaskID + ".db";
            System.IO.File.Delete(FileName);

            if (isCloseTab)
                CloseTabBySilent(TaskID, TaskName);

            //��ʼ�������������Ѿ���ɵ��б��У�����һ������
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
            //����������������޸������ͼ�꣬���¼����ɵ����ť������
            //�������д���

            try
            {
                
                InvokeMethod(this, "UpdateRunTaskState", new object[] { e.TaskID, cGlobalParas.TaskState.Running });

                if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                    Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
                {
                    //��Ҫ�жϵ�ǰִ�е������ǲ���Զ�����������Զ����������Ҫ�޸�״̬
                    oTask t = new oTask(Program.getPrjPath());
                    t.LoadTask(e.TaskID);
                    cGlobalParas.TaskClass tClass = t.TaskEntity.TaskClass;
                    string TaskDemo = t.TaskEntity.TaskDemo;
                    t = null;
                    if (tClass == cGlobalParas.TaskClass.Remote)
                    {
                        //��ʾԶ������
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
            //�ݲ����κδ���

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

            //����������������޸������ͼ�꣬���¼����ɵ����ť������
            //�������д���

            try
            {

                
                InvokeMethod(this, "UpdateRunTaskState", new object[] { e.TaskID, cGlobalParas.TaskState.Stopped });

                if (e_SetControlProperty != null)
                {
                    e_SetControlProperty (this,new SetControlPropertyEvent ("toolStartTask","Enabled","false"));
                    //e_SetControlProperty (this,new SetControlPropertyEvent ("toolRestartTask","Enabled","false"));
                    e_SetControlProperty (this,new SetControlPropertyEvent ("toolStopTask","Enabled","false"));
                }


                ////�ڴ˴��������������û��жϣ���Ҫ���еı�Ҫ���湤��
                ////�����жϺ�ϵͳ�豣���Ѿ��ɼ���ɵ����ݣ������Ѿ��ɼ�����ַ��¼��
                ////ȷ���´���������ʱ������ֱ�ӽ��У����������صĶϵ㹦�ܲ���


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
                //s += m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count + m_GatherControl.TaskManage.TaskListControl.StoppedTaskList.Count + m_PublishControl.PublishManage.ListPublish.Count + "-������  ";
                s += m_GatherControl.TaskManage.TaskListControl.RunningTaskList.Count + "-" + rm.GetString("State2") + "  ";
                s += m_GatherControl.TaskManage.TaskListControl.StoppedTaskList.Count + "-" + rm.GetString("State3") + "  ";
                s += m_PublishControl.PublishManage.ListPublish.Count + "-" + rm.GetString("State4");

                if (e_SetControlProperty != null)
                    e_SetControlProperty(this, new SetControlPropertyEvent("toolStripStatusLabel2", "Text", s));

                //����tab����ʾͼ��
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
                //������󲻴���
            }

        }

        //����Url�ɼ��������󣬲����н�����Ӧ����¼��־���ɣ���־�������¼���¼���
        //������ﵽһ���������󣬻��ɺ�̨�̴߳�������ʧ�ܵ��¼���������ʧ���¼����
        //��ʱ���ݵĴ洢
        private void tManage_TaskError(object sender, TaskErrorEventArgs e)
        {
            Int64 TaskID = e.TaskID;
            string strLog = e.Error.Message.ToString();
            string conName = "sCon" + TaskID;
            string pageName = "page" + TaskID;

            //����Ҫͨ������֪ͨ�����û���ֱ��д��ɼ����ӣ�ע���������
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

                //������ɺ������Ƿ񷢲������ô˷�������ΪҪ������ʱ���ݱ���
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
            //����ɼ���ɣ���������Ϣ֪ͨ���壬֪ͨ�û�


        }

        //д��־�¼�
        private void tManage_Log(object sender, cGatherTaskLogArgs e)
        {
            //д��־
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

        //д�����¼�
        private void tManage_GData(object sender, cGatherDataEventArgs e)
        {
            try
            {
                //д�ɼ����ݵ�����Datagridview


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
                //д�ɼ����ݵ�����Datagridview


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

            //�ڴ˸���rowCount
            DataGridViewRow dRow = (from a in this.dataTask.Rows.Cast<DataGridViewRow>()
                                    where a.Cells[1].Value.Equals(TaskID)
                                    select a).FirstOrDefault();
            dRow.Cells[8].Value = count;

        }

        public void UpdateGatherUrl(Int64 TaskID,int UrlCount,int gUrlCount,int errUrlCount)
        {
            


            //�ڴ˸���rowCount
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
            //�ж������Ƿ�Ϊ�������������������������ɾ��taskrun�е���Ϣ
            //��Ϊ����������Զ�����ʱ

            //���±�������
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

                InvokeMethod(this, "ExportPublishLog", new object[] { pageName, conName, cGlobalParas.LogType.Warning, "�˲ɼ������������ڷ�����ɺ�ɾ���ݣ���˲ɼ������Ѿ�ɾ����" });

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

            //���Ѿ���ɷ�����������ӵ��������������ļ���
            oTaskComplete t = new oTaskComplete(Program.getPrjPath());
            t.InsertTaskComplete(ec, tState);
            t = null;

            //ɾ��taskrun�ڵ�
            tr.LoadTaskRunData();
            tr.DelTask(TaskID);

            //�޸�Tabҳ������


            //ɾ��run�е�����ʵ���ļ�
            string FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".rst";
            System.IO.File.Delete(FileName);

            //ɾ��run�е�����ʵ�����ؿ��ļ�
            FileName = Program.getPrjPath() + "Tasks\\run\\" + "Task" + TaskID + ".db";
            System.IO.File.Delete(FileName);

            //ɾ��run�еĲɼ����������ļ�
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

        #region ����������¼�����
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
                e_ShowLogInfo(this, new ShowInfoEvent("������ֹͣ", e.TaskName));

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

            //�ɼ��������󣬽�������־���
            InvokeMethod(this, "ExportPublishLog", new object[] { pageName, conName, cGlobalParas.LogType.Error, "��������������Ϣ��" + e.Error.Message });


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

        //    InvokeMethod(this, "ExportPublishLog", new object[] { pageName, conName,cGlobalParas.LogType.Info  , "���ݱ���ɹ���" });

        //}

        private void Publish_Log(object sender, PublishLogEventArgs e)
        {
            //д�����������־
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
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error, cGlobalParas.LogClass.Task, DateTime.Now.ToString(), "�ɼ�ҳ���Ѿ��رգ�����״̬����ʧ�ܣ�"));
                return;
            }

            DataGridViewRow r= ((cMyDataGridView)(this.tabControl1.TabPages[pageName].Controls[conName].Controls[0].Controls[0]))
                .FindRow(row);
             r.Cells[colCount - 1].Value = e.isPublishSucceed.ToString();

        }

        #endregion

        #region ί�д��� ���ں�̨�̵߳��� ����UI�̵߳ķ���������

        delegate void bindvalue(object Instance, string Property, object value);
        delegate object invokemethod(object Instance, string Method, object[] parameters);
        delegate object invokepmethod(object Instance, string Property, string Method, object[] parameters);
        delegate object invokechailmethod(object InstanceInvokeRequired, object Instance, string Method, object[] parameters);

        /// <summary>
        /// ί�����ö�������
        /// </summary>
        /// <param name="Instance">����</param>
        /// <param name="Property">������</param>
        /// <param name="value">����ֵ</param>
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
        /// ί��ִ��ʵ���ķ������������붼��Public ��������
        /// </summary>
        /// <param name="Instance">��ʵ��</param>
        /// <param name="Method">������</param>
        /// <param name="parameters">�����б�</param>
        /// <returns>����ֵ</returns>
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
        /// ί��ִ��ʵ���ķ���
        /// </summary>
        /// <param name="InstanceInvokeRequired">����ؼ�����</param>
        /// <param name="Instance">��Ҫִ�з����Ķ���</param>
        /// <param name="Method">������</param>
        /// <param name="parameters">�����б�</param>
        /// <returns>����ֵ</returns>
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
        /// ί��ִ��ʵ�������Եķ���
        /// </summary>
        /// <param name="Instance">��ʵ��</param>
        /// <param name="Property">������</param>
        /// <param name="Method">������</param>
        /// <param name="parameters">�����б�</param>
        /// <returns>����ֵ</returns>
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
        /// ��ȡʵ��������ֵ
        /// </summary>
        /// <param name="ClassInstance">��ʵ��</param>
        /// <param name="PropertyName">������</param>
        /// <returns>����ֵ</returns>
        private static object GetPropertyValue(object ClassInstance, string PropertyName)
        {
            Type myType = ClassInstance.GetType();
            PropertyInfo myPropertyInfo = myType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return myPropertyInfo.GetValue(ClassInstance, null);
        }
        /// <summary>
        /// ����ʵ��������ֵ
        /// </summary>
        /// <param name="ClassInstance">��ʵ��</param>
        /// <param name="PropertyName">������</param>
        private static void SetPropertyValue(object ClassInstance, string PropertyName, object PropertyValue)
        {
            Type myType = ClassInstance.GetType();
            PropertyInfo myPropertyInfo = myType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            myPropertyInfo.SetValue(ClassInstance, PropertyValue, null);
        }

        /// <summary>
        /// ִ��ʵ���ķ���
        /// </summary>
        /// <param name="ClassInstance">��ʵ��</param>
        /// <param name="MethodName">������</param>
        /// <param name="parameters">�����б�</param>
        /// <returns>����ֵ</returns>
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

        //���ڸ��½������ʾ״̬
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


        //        //�жϵ�ǰ��ʾ����ʲô��Ϣ

        //        try
        //        {
                   
        //                //�����ǰѡ����ʼ����
        //                //����m_GatherControl.TaskManage.TaskList���и���
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

        //        //��������
        //        //if (e_dSpeedEvent != null)
        //        //    e_dSpeedEvent(this, new DownloadSpeedEvent(NetMiner.Gather.Gather.cWatchSpeedRate.DownloadSpeed));

        //        IsTimer = true;
        //    }
        //}

        #region �����¼� ���ڸ���������������Ϣ
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

        //�ڴ˴���Ĭ����ģʽ�µĹ���
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

                    //�ж��Ƿ��Ѿ�û��ѡ������û�У�������tab��ʾ
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
            //��ȡ�����Ѿ��ɼ������񣬲��Բɼ�������з�����������������Ѿ�Ϊ�㣬��ɾ��
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
                        //��ʾû�����ݣ�ɾ���˲ɼ���������ݼ��ļ�
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
                MessageBox.Show("������ϣ�δ������Ч�Ĳɼ��������ݣ�", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (MessageBox.Show("����" + delTasks.Count + "���Ѿ���ɵĲɼ�����������Ч���Ƿ�ɾ����", "����� ѯ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    for (int i = 0; i < delTasks.Count; i++)
                    {
                        long TaskID = delTasks[i];
                        string TaskName = tc.LoadSingleTask(TaskID).TaskName;
                        tc.DelTask(TaskID);
                        tc = null;

                        //ɾ��run�е�����ʵ���ļ�
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

            //�ж����������Щ���ݣ��������л��ǲɼ����
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

            //        //��������������������soukeydata�������ͣ������ã���Ϊ�˱�֤ϵͳ������������//
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
            if (MessageBox.Show("�������ݷ����󣬽��޷�ֹͣ����Ҳ��ʹ�á�����󹤷������ߡ����з����������Ƿ������", "����� ѯ��",
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
            if (MessageBox.Show("�������ݷ����󣬽��޷�ֹͣ����Ҳ��ʹ�á�����󹤷������ߡ����з����������Ƿ������", "����� ѯ��",
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

            //�����ݿ�������
            DataTable d = ((DataTable)tmp.DataSource).Copy();

            m_ExportPageName = this.tabControl1.SelectedTab.Name;

            ExportGatherLog(this.tabControl1.SelectedTab.Name, this.tabControl1.SelectedTab.Controls[0].Name, cGlobalParas.LogType.Info, rm.GetString("Info237") + "\n");

            oPublishTask p = new oPublishTask(Program.getPrjPath());
            ePublishTask ep= p.LoadSingleTask(pName);

            ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);

            if (ep.PublishType == cGlobalParas.PublishType.publishTemplate)
            {
                //������ģ�����Ϣ
                //string templateName=p.GetTemplateName(0);
                //templateName = templateName.Substring(0, templateName.IndexOf("["));
                //cGlobalParas.PublishTemplateType pType = (cGlobalParas.PublishTemplateType)cGlobalParas.ConvertID(templateName.Substring(templateName.IndexOf("[") + 1, templateName.IndexOf("]") - templateName.IndexOf("[") - 1));

                //ģ�淢��
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

            //�����ݿ�������
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

        #region ������ʱ�ļ����첽����
        private delegate string[] delegateSaveTempFile(Int64 TaskID, DataTable d, string TaskName, string TempDataFile,
            bool IsSaveSingleFile);
        private void StartSaveTemp(Int64 TaskID, DataTable d)
        {
            //�Ƚ���Ҫ��������ȡ����
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
        /// ������ʱ�ļ��Ļص�����
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
                    e_ShowLogInfo(this, new ShowInfoEvent("������Ϣ", tName +  "�ɼ����񱣴���ʱ�ļ�����������Ϣ��" + ex.Message));
            }
        
        }

        private readonly object m_fileLock = new object();
        /// <summary>
        /// ���ڱ�����ʱ���ݣ������ɼ��ͷ��������е�������Ϣ
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
            return new string[] { TaskID.ToString(), TaskName, "���ݱ�������" + Path.GetFileName(FileName) };
        }

        #endregion


        #region �ֲ�ʽ����Ĳ���
        /// <summary>
        /// һ����ʱ�������ڷ���������ͨѶ����ʾ�Լ��Ŀͻ�������Ч�ģ�ͬʱ��ȡ��Ӧ�Ĳɼ�������вɼ�����
        /// </summary>
        /// <param name="State"></param>
        private void m_RemoteEngine_CallBack(object State)
        {
            if (this.m_IsDo == false)
            {
                m_IsDo = true;

                #region ��ȡ�ɼ������ϴ��ɼ�������
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
                    //    //������ص�Զ������
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
                        #region ��Զ�̻�ȡ�ɼ�����
                        //localhost.cRemoteTaskEntity getRemoteTask = sweb.GetTaskName(Program.RegisterUser);

                        //if (getRemoteTask != null)
                        //{
                        //    //��ȡ�������Ǹ�zipѹ���ļ�
                        //    string fName = getRemoteTask.TaskFileName;
                        //    byte[] taskByte = sweb.GetTaskFile(fName);

                        //    //�Ƚ�ѹ���ļ����ڱ�����ʱĿ¼
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

                        //    //��ȡ���ļ���ʼ��ѹ��
                        //    NetMiner.Common.Tool.cZipCompression zCompress = new NetMiner.Common.Tool.cZipCompression();
                        //    zCompress.DeCompressZIP(tmpFile, Program.getPrjPath() + "tmp\\");

                        //    string FileName = Program.getPrjPath() + "tmp\\" + Path.GetFileNameWithoutExtension(tmpFile)
                        //        + "\\" + Path.GetFileNameWithoutExtension(tmpFile) + ".smt";

                        //    //����index����

                        //    oTask t = new oTask(Program.getPrjPath());
                        //    t.LoadTask(FileName);

                        //    oTaskIndex ti = new oTaskIndex(Program.getPrjPath());
                        //    //���ж������ļ����Ƿ���ڴ�����������ڣ������������������ز���
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

                        //            //����ȡ�������Ϣ�洢�ڲɼ�����ע��
                        //            t.TaskEntity.TaskDemo = getRemoteTask.ID + "," + getRemoteTask.TaskName + "," + getRemoteTask.GatherTaskType + "," + getRemoteTask.TaskFileName;

                        //            //����ͼƬ·��������
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
                        //                    e_ShowLogInfo(this, new ShowInfoEvent("�������غ����״̬ʧ��" , Path.GetFileNameWithoutExtension(fName)));
                        //            }

                        //            //�ж��Ƿ������ؿ⣬����У���������ؿ�
                        //            string urlDb = Program.getPrjPath() + "tmp\\" + Path.GetFileNameWithoutExtension(tmpFile)
                        //                + "\\Զ��-" + Path.GetFileNameWithoutExtension(tmpFile) + ".db";

                        //            if (File.Exists(urlDb))
                        //            {
                        //                //��ʾ�������ؿ�
                        //                File.Copy(urlDb, Program.getPrjPath() + "urls\\" + Path.GetFileName(urlDb), true);
                        //            }
                        //            ////�ɹ��洢������
                        //            //if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.Normal)
                        //            //    sweb.UpdateTaskState(getRemoteTask.ID, Program.RegisterUser, (int)cGlobalParas.TaskState.RemoteUnstart);
                        //            //else if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.SplitTask)
                        //            //    sweb.UpdateSplitTaskState(getRemoteTask.ID, Program.RegisterUser, (int)cGlobalParas.TaskState.RemoteUnstart);
                        //         }
                        //         catch (System.Exception ex)
                        //         {
                        //             //����״̬
                        //             //if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriTask)
                        //             //    sweb.UpdateTaskState(getRemoteTask.ID, Program.RegisterUser, (int)cGlobalParas.TaskState.UnStart, 0);
                        //             //else if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriSplitTask)
                        //             //    sweb.UpdateSplitTaskState(getRemoteTask.ID, Program.RegisterUser, (int)cGlobalParas.TaskState.UnStart, 0);

                        //             //�׳�����
                        //             throw ex;

                        //         }
                        //    }
                        //    else
                        //    {
                        //        //��ʾ�����ļ��Ѿ����ڴ����񣬽�Զ������״̬����
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

                    //�ڴ��Զ�ִ��Զ�̵Ĳɼ���������Ѿ�ִ����ϣ����Զ��ϴ�

                    List<Int64> cCompleteTaskID = new List<long>();

                    //��ȡԶ�����������
                    Dictionary<string, cGlobalParas.TaskState> rTask = new Dictionary<string, cGlobalParas.TaskState>();
                    #region ��ȡԶ������ִ�е�״̬
                    foreach (NetMiner.Core.gTask.Entity.eTaskIndex et in eIndexs)
                    {

                        bool isExist = false;
                        bool isExistComplete = false;    //��״ֵ̬�Ǳ�ʾ�����Ѿ���ɣ������ڱ�����ʱ�ɼ������ݣ��ɼ������Ѿ��Ƶ�completed�����У�����û���浽�ɼ������taskcompleted��

                        //�ж�running�Ķ���
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
                            //�ж��Ѿ���ͣ�Ķ���
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

                        #region  �ж��Ѿ���ɵ��������
                        if (isExist == false)
                        {

                            //��ʼ�ж��Ƿ��Ѿ�ִ�н���
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
                                //�ڴ���Ҫ�ж����Ƿ�����ɵĶ�����
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

                    //�ڴ�ִ�вɼ�����
                    foreach (KeyValuePair<string, cGlobalParas.TaskState> kv in rTask)
                    {
                        if (kv.Value == cGlobalParas.TaskState.Running)
                        {
                            //���Բ�����ᡣ

                        }
                        else if (kv.Value == cGlobalParas.TaskState.Completed)
                        {
                            //�����Ѿ���ɵ�����
                            #region �ϴ��ɼ�����
                            for (int n = 0; n < cCompleteTaskID.Count; n++)
                            {
                                oTaskComplete tc = new oTaskComplete(Program.getPrjPath());
                                eTaskCompleted ec = tc.LoadSingleTask(cCompleteTaskID[n]);
                                string dFile = ec.TempFile;
                                string tName = ec.TaskName;
                                long uTaskID = ec.TaskID;
                                tc = null;

                                //��ȡ�����Զ����Ϣ
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


                                //�ϴ��ļ�ǰ�Ƚ���ѹ������
                                Dictionary<string, int> uFiles = new Dictionary<string, int>();
                                uFiles.Add(dFile, (int)cGlobalParas.FileType.File);

                                //��ȡ��־�ļ�
                                string logTask = Program.getPrjPath() + "log\\task" + tName + ".csv";
                                if (File.Exists(logTask))
                                    uFiles.Add(logTask, (int)cGlobalParas.FileType.File);

                                //��ȡ���ؿ��ļ�
                                string dbTask = Program.getPrjPath() + "urls\\Զ��-" + tName + ".db";
                                if (File.Exists(dbTask))
                                    uFiles.Add(dbTask, (int)cGlobalParas.FileType.File);

                                //��ȡ����ͼƬ��Ŀ¼
                                string imgPath = Program.getPrjPath() + "data\\" + tName + "_file";
                                if (Directory.Exists(imgPath))
                                    uFiles.Add(imgPath, (int)cGlobalParas.FileType.Directory);

                                string tmpFile = Program.getPrjPath() + "tmp\\" + tName + ".zip";
                                NetMiner.Common.Tool.cZipCompression zCompress = new NetMiner.Common.Tool.cZipCompression();
                                zCompress.CompressZIP(uFiles, tmpFile);

                                //�ϴ�����
                                //�޸ĳɷ������䣬ÿ�δ���10MB������

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
                                    //�����ֽ���
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

                                //�����ϴ����������ϵ���Զ�̷��񣬽������ݴ���
                                //sweb.DealUploadZIP(Program.RegisterUser, tName, uRTask.ID, uTaskID, uRTask.GatherTaskType);

                                //����������
                                //CloseTabBySilent(cCompleteTaskID[n], tName);

                                InvokeMethod(this, "CloseTabBySilent", new object[] { cCompleteTaskID[n], tName });

                                //ɾ��taskcompleter
                                tc = new oTaskComplete(Program.getPrjPath());
                                tc.LoadTaskData();
                                tc.DelTask(cCompleteTaskID[n]);
                                tc.Dispose();
                                tc = null;

                                //ɾ�������ļ�
                                File.Delete(dFile);

                                File.Delete(dbTask);

                                //ɾ����־�ļ�
                                File.Delete(logTask);

                                //ɾ��ͼƬ�ļ�
                                if (Directory.Exists(imgPath))
                                    Directory.Delete(imgPath, true);

                                File.Delete(tmpFile);

                                //ɾ��remoteclass�е��ļ�����ʾ�Ѿ�����
                                t = new oTask(Program.getPrjPath());
                                t.DeleTask(Program.getPrjPath() + Program.g_RemoteTaskPath, tName);
                                t = null;

                                //ɾ�������е�����
                                cGatherTask gt = m_GatherControl.TaskManage.FindTask(uTaskID);
                                if (gt != null)
                                    this.m_GatherControl.TaskManage.TaskListControl.CompletedTaskList.Remove(gt);
                                gt = null;
                            }
                            #endregion
                        }
                        else if (kv.Value == cGlobalParas.TaskState.UnStart && rCount < m_MaxRemoteCount)
                        {
                            #region ����Զ�̲ɼ�����
                            //����������������ǰ���Ƚ�����ķ���������Ϊ��


                            cGatherTask gt = null;
                            string tClassName = Program.g_RemoteTaskClass;
                            string tClassPath = NetMiner.Constant.g_RemoteTaskPath;
                            gt = AddRunTask(tClassName, tClassPath, kv.Key);
                            if (gt == null)
                            {
                                //��ʾ���������û��жϣ�Ҳ�п�������Ϊ�������
                                return;
                            }

                            Int64 TaskID = gt.TaskID;
                            //AddTab(TaskID, kv.Key);
                            InvokeMethod(this, "AddTab", new object[] { TaskID, kv.Key });
                            //����������
                            m_GatherControl.Start(gt);
                            gt = null;
                            #endregion
                        }
                        else if (kv.Value == cGlobalParas.TaskState.UnStart && rCount < m_MaxRemoteCount)
                        {
                            //�������д�����

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
                //ǿ�ƽ����������еĲɼ�����
                OverMultiTask();
            }
            else if (this.dataTask.SelectedCells[3].Value.ToString() == "nodPublish")
            {
                //ǿ�ƽ������ڷ����Ĳɼ�����
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

            //ֹͣ������
            m_GatherControl.Over(t);

        }

        private void OverPublish(Int64 TaskID, string TaskName)
        {
            //�Ȱ����ص����ݴ�
            BrowserMultiData();


            cPublish pt = null;


            //ִ������ִ�е�����
            pt = m_PublishControl.PublishManage.FindTask(TaskID);

            if (pt == null)
                return;

            //ֹͣ������
            m_PublishControl.OverPublish(pt);

            //���������ɹ���ʾ��Ϣ
            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent("�����ѽ�����", TaskName));

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
            //����һ�������Ĭ�Ϸ��࣬��ָ�ӷ��������صĲɼ�����
            IEnumerable< NetMiner.Core.gTask.Entity.eTaskIndex> eIndexs= xmlTasks.GetTaskDataByClass(NetMiner.Constant.g_RemoteTaskClass);

            //��ʼ��ʼ���˷����µ�����
            //int count = xmlTasks.GetTaskClassCount();

            foreach(NetMiner.Core.gTask.Entity.eTaskIndex et in eIndexs)
            {
                //��ʼ��һɾ������
                oTask t = new oTask(Program.getPrjPath());

                oTaskClass tc = new oTaskClass(Program.getPrjPath());
                string tPath = tc.GetTaskClassPathByName(Program.g_RemoteTaskClass);
                tc = null;
                t.DeleTask(tPath,et.TaskName);
                t = null;

                //�жϴ��������������еĶ������Ƿ���ڣ�������ڣ�ֹͣ��ɾ��
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
                //���¼���һ���״�������Ϣ

                //LoadRadarRule();

                m_RadarControl.StartRadar();
            }
            else
            {
                MessageBox.Show("��ǰ�汾��֧�������״���ݼ�أ����ܣ����ȡ��ȷ�İ汾��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
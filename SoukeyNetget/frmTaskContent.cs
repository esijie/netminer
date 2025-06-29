using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using NetMiner.Resource;
using System.Resources;
using NetMiner.Core.gTask;
using NetMiner.Core.Radar;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using NetMiner.Core.gTask.Entity;
using System.Text.RegularExpressions;
using NetMiner.Core.Radar.Entity;
using SoukeyControl.CustomControl;
using NetMiner.Core.Plan;
using NetMiner.Core.Plan.Entity;
using Newtonsoft.Json;
using NetMiner;
using System.Threading;
using NetMiner.Publish.Rule;

namespace MinerSpider
{

    public partial class frmTaskContent : DockContent
    {
        private ResourceManager rm;
        private DataGridViewCellStyle m_RowStyleErr;
        private string OldName = "";

        //定义一个treenode，此值与左侧菜单当前选中的树形节点保持一致
        private TreeNode m_TreeNode;

        //定义一个值，用于存储在上传采集任务时选择的远程任务分类编号
        private int m_RemoteTaskClassID = 0;

        public frmTaskContent()
        {
            InitializeComponent();

            m_TreeNode = new TreeNode();

            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));
        }

        public TreeNode TreeNode
        {
            get { return m_TreeNode; }
            set { m_TreeNode = value; }
        }

        //设置加载错误的gridrows的现实样式
        private void SetRowErrStyle()
        {
            this.m_RowStyleErr = new DataGridViewCellStyle();
            this.m_RowStyleErr.Font = new Font(DefaultFont, FontStyle.Italic);
            this.m_RowStyleErr.ForeColor = Color.Red;
        }


        private void ShowPublishTemplate()
        {
            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {
            }

            DataGridViewImageColumn tStateImg = new DataGridViewImageColumn();
            tStateImg.HeaderText = rm.GetString("GridState");
            tStateImg.Width = 40;
            tStateImg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tStateImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(0, tStateImg);

            //任务编号,不显示此列
            DataGridViewTextBoxColumn tID = new DataGridViewTextBoxColumn();
            tID.Name = rm.GetString("GridID");
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
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = "模板名称";
            tName.Width = 120;
            this.dataTask.Columns.Insert(4, tName);

            DataGridViewTextBoxColumn tRemark = new DataGridViewTextBoxColumn();
            tRemark.HeaderText = "备注";
            tRemark.AutoSizeMode= DataGridViewAutoSizeColumnMode.Fill;
            this.dataTask.Columns.Insert(5, tRemark);

          
        }

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
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);

            #endregion

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = rm.GetString("GridTaskName");
            tName.Width = 150;
            this.dataTask.Columns.Insert(4, tName);

            DataGridViewTextBoxColumn tRunType = new DataGridViewTextBoxColumn();
            tRunType.HeaderText = rm.GetString("GridTaskRunType");
            tRunType.Width = 120;
            this.dataTask.Columns.Insert(5, tRunType);

            DataGridViewTextBoxColumn curState = new DataGridViewTextBoxColumn();
            curState.HeaderText = "当前状态";
            curState.Width = 120;
            curState.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(6, curState);

            DataGridViewTextBoxColumn StartTimer = new DataGridViewTextBoxColumn();
            StartTimer.HeaderText = rm.GetString("StartTime");
            StartTimer.Width = 120;
            StartTimer.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(7, StartTimer);

            DataGridViewTextBoxColumn GatheredUrlCount = new DataGridViewTextBoxColumn();
            GatheredUrlCount.HeaderText = rm.GetString("GridCompleteCount");
            GatheredUrlCount.Width = 50;
            GatheredUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(8, GatheredUrlCount);

            DataGridViewTextBoxColumn GatheredErrUrlCount = new DataGridViewTextBoxColumn();
            GatheredErrUrlCount.HeaderText = rm.GetString("GridErrorCount");
            GatheredErrUrlCount.Width = 50;
            GatheredErrUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(9, GatheredErrUrlCount);

            DataGridViewTextBoxColumn tUrlCount = new DataGridViewTextBoxColumn();
            tUrlCount.HeaderText = rm.GetString("GridUrlCount");
            tUrlCount.Width = 50;
            tUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(10, tUrlCount);

            DataGridViewProgressBarColumn tPro = new DataGridViewProgressBarColumn();
            tPro.HeaderText = rm.GetString("GridProcess");
            tPro.Width = 120;
            this.dataTask.Columns.Insert(11, tPro);



            DataGridViewTextBoxColumn tExportFile = new DataGridViewTextBoxColumn();
            tExportFile.HeaderText = rm.GetString("GridExportType");
            tExportFile.Width = 1900;
            this.dataTask.Columns.Insert(12, tExportFile);

            DataGridViewTextBoxColumn tPath = new DataGridViewTextBoxColumn();
            tPath.Name = "TaskPath";
            tPath.Width = 0;
            tPath.Visible = false;
            this.dataTask.Columns.Insert(13, tPath);
        }

        private void ShowPublishTask()
        {
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
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);

            #endregion

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = rm.GetString("GridTaskName");
            tName.Width = 150;
            this.dataTask.Columns.Insert(4, tName);

            DataGridViewTextBoxColumn PublishedCount = new DataGridViewTextBoxColumn();
            PublishedCount.HeaderText = rm.GetString("GridExportedCount");
            PublishedCount.Width = 80;
            PublishedCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(5, PublishedCount);

            DataGridViewTextBoxColumn PublishedErrCount = new DataGridViewTextBoxColumn();
            PublishedErrCount.HeaderText = "发布错误数量";
            PublishedErrCount.Width = 80;
            PublishedErrCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(6, PublishedErrCount);

            DataGridViewTextBoxColumn Count = new DataGridViewTextBoxColumn();
            Count.HeaderText = rm.GetString("GridUrlCount");
            Count.Width = 80;
            Count.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(7, Count);


            DataGridViewProgressBarColumn tPro = new DataGridViewProgressBarColumn();
            tPro.HeaderText = rm.GetString("GridProcess");
            tPro.Width = 120;
            this.dataTask.Columns.Insert(8, tPro);


            DataGridViewTextBoxColumn PublishType = new DataGridViewTextBoxColumn();
            PublishType.HeaderText = rm.GetString("GridExportType");
            PublishType.Width = 1900;
            this.dataTask.Columns.Insert(9, PublishType);
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

            //用于通过判断Datagridview的数据就可知道当前树形结构选择的节点
            //用于控制(更新)界面显示状态
            DataGridViewTextBoxColumn tTreeNode = new DataGridViewTextBoxColumn();
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);
            #endregion

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = rm.GetString("GridTaskName");
            tName.Width = 150;
            this.dataTask.Columns.Insert(4, tName);

            //DataGridViewTextBoxColumn tType = new DataGridViewTextBoxColumn();
            //tType.HeaderText = rm.GetString("GridTaskType");
            //tType.Width = 180;
            //this.dataTask.Columns.Insert(5, tType);

            DataGridViewTextBoxColumn tcDate = new DataGridViewTextBoxColumn();
            tcDate.HeaderText = rm.GetString("GridTaskCompleteDate");
            tcDate.Width = 120;
            this.dataTask.Columns.Insert(5, tcDate);

            //DataGridViewTextBoxColumn gatherUrlCount = new DataGridViewTextBoxColumn();
            //gatherUrlCount.HeaderText = "成功采集";
            //gatherUrlCount.Width = 80;
            //gatherUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //this.dataTask.Columns.Insert(6, gatherUrlCount);

            //DataGridViewTextBoxColumn errUrlCount = new DataGridViewTextBoxColumn();
            //errUrlCount.HeaderText = "失败数量";
            //errUrlCount.Width = 80;
            //errUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //this.dataTask.Columns.Insert(7, errUrlCount);

            DataGridViewTextBoxColumn tUrlCount = new DataGridViewTextBoxColumn();
            tUrlCount.HeaderText = rm.GetString("GridUrlCount");
            tUrlCount.Width = 80;
            tUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(6, tUrlCount);

            DataGridViewTextBoxColumn tRowCount = new DataGridViewTextBoxColumn();
            tRowCount.HeaderText = "数据条数";
            tRowCount.Width = 80;
            tRowCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(7, tRowCount);

            DataGridViewTextBoxColumn tPro = new DataGridViewTextBoxColumn();
            tPro.HeaderText = rm.GetString("GridTaskRunType");
            tPro.Width = 120;
            this.dataTask.Columns.Insert(8, tPro);

            DataGridViewTextBoxColumn tExportFile = new DataGridViewTextBoxColumn();
            tExportFile.HeaderText = rm.GetString("GridExportType");
            tExportFile.Width = 1900;
            this.dataTask.Columns.Insert(9, tExportFile);

        }

        private void ShowTaskInfo()
        {
            this.Text = "采集任务管理";
            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {
            }

            #region 比部分为固定显示
            DataGridViewImageColumn tStateImg = new DataGridViewImageColumn();
            tStateImg.HeaderText = rm.GetString("GridState");
            tStateImg.Width = 40;
            tStateImg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tStateImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(0, tStateImg);

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
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);

            DataGridViewTextBoxColumn taskClass = new DataGridViewTextBoxColumn();
            taskClass.HeaderText = "taskClass";
            taskClass.Visible = false;
            this.dataTask.Columns.Insert(4, taskClass);

            #endregion

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = rm.GetString("GridTaskName");
            tName.Width = 180;
            this.dataTask.Columns.Insert(5, tName);

            //DataGridViewTextBoxColumn tType = new DataGridViewTextBoxColumn();
            //tType.HeaderText = rm.GetString("GridTaskType");
            //tType.Width = 130;
            //this.dataTask.Columns.Insert(5, tType);

            DataGridViewTextBoxColumn tUrlCount = new DataGridViewTextBoxColumn();
            tUrlCount.HeaderText = rm.GetString("GridUrlCount");
            tUrlCount.Width = 80;
            tUrlCount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            this.dataTask.Columns.Insert(6, tUrlCount);

            //DataGridViewTextBoxColumn tRunType = new DataGridViewTextBoxColumn();
            //tRunType.HeaderText = rm.GetString("GridTaskRunType");
            //tRunType.Width = 120;
            //this.dataTask.Columns.Insert(6, tRunType);

            DataGridViewTextBoxColumn tExportFile = new DataGridViewTextBoxColumn();
            tExportFile.HeaderText = rm.GetString("GridExportType");
            tExportFile.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            tExportFile.Width = 1900;

            this.dataTask.Columns.Insert(7, tExportFile);


        }

        private void ShowTaskPlan()
        {
            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {
            }

            #region 比部分为固定显示
            DataGridViewImageColumn tStateImg = new DataGridViewImageColumn();
            tStateImg.HeaderText = rm.GetString("GridState");
            tStateImg.Width = 40;
            tStateImg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tStateImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(0, tStateImg);

            DataGridViewTextBoxColumn tID = new DataGridViewTextBoxColumn();
            tID.Name = rm.GetString("GridID");
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
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);

            #endregion

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = rm.GetString("GridPlanName");
            tName.Width = 150;
            this.dataTask.Columns.Insert(4, tName);

            //任务状态,不显示此列
            DataGridViewTextBoxColumn tState1 = new DataGridViewTextBoxColumn();
            tState1.Name = rm.GetString("GridState");
            tState1.Width = 80;
            this.dataTask.Columns.Insert(5, tState1);

            DataGridViewTextBoxColumn tIsDisabled = new DataGridViewTextBoxColumn();
            tIsDisabled.HeaderText = rm.GetString("GridIsOvertime");
            tIsDisabled.Width = 120;
            this.dataTask.Columns.Insert(6, tIsDisabled);

            DataGridViewTextBoxColumn tDisabledRule = new DataGridViewTextBoxColumn();
            tDisabledRule.HeaderText = rm.GetString("GridOvertimeRule");
            tDisabledRule.Width = 180;
            this.dataTask.Columns.Insert(7, tDisabledRule);

            DataGridViewTextBoxColumn tPlanning = new DataGridViewTextBoxColumn();
            tPlanning.HeaderText = rm.GetString("GridRunPlan");
            tPlanning.Width = 180;
            this.dataTask.Columns.Insert(8, tPlanning);

            DataGridViewTextBoxColumn RunDate = new DataGridViewTextBoxColumn();
            RunDate.HeaderText = rm.GetString("GridNextTime");
            RunDate.Width = 160;
            //RunDate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(9, RunDate);


            DataGridViewTextBoxColumn tRemark = new DataGridViewTextBoxColumn();
            tRemark.HeaderText = rm.GetString("GridRemark");
            tRemark.Width = 380;
            tRemark.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(10, tRemark);

        }

        private void ShowRuleInfo()
        {

            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {

            }

            #region 比部分为固定显示
            DataGridViewImageColumn tStateImg = new DataGridViewImageColumn();
            tStateImg.HeaderText = ""; ;
            tStateImg.Width = 26;
            tStateImg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tStateImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(0, tStateImg);

            DataGridViewTextBoxColumn tID = new DataGridViewTextBoxColumn();
            tID.Name = rm.GetString("GridTaskID");
            tID.Width = 0;
            tID.Visible = false;
            this.dataTask.Columns.Insert(1, tID);

            //任务状态,不显示此列
            DataGridViewTextBoxColumn tState = new DataGridViewTextBoxColumn();
            tState.Name = "状态";
            tState.Width = 60;
            tState.Visible = false;
            this.dataTask.Columns.Insert(2, tState);

            //用于通过判断Datagridview的数据就可知道当前树形结构选择的节点
            //用于控制(更新)界面显示状态
            DataGridViewTextBoxColumn tTreeNode = new DataGridViewTextBoxColumn();
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);

            #endregion

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = "规则名称";
            tName.Width = 80;
            this.dataTask.Columns.Insert(4, tName);

            DataGridViewTextBoxColumn tShowState = new DataGridViewTextBoxColumn();
            tShowState.HeaderText = "当前状态";
            tShowState.Width = 80;
            this.dataTask.Columns.Insert(5, tShowState);

            DataGridViewTextBoxColumn tRound = new DataGridViewTextBoxColumn();
            tRound.HeaderText = "已监控次数";
            tRound.Width = 80;
            tRound.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(6, tRound);

            DataGridViewTextBoxColumn tUrls = new DataGridViewTextBoxColumn();
            tUrls.HeaderText = "已处理网址";
            tUrls.Width = 80;
            tUrls.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(7, tUrls);

            DataGridViewTextBoxColumn tGetUrls = new DataGridViewTextBoxColumn();
            tGetUrls.HeaderText = "符合规则网址";
            tGetUrls.Width = 80;
            tGetUrls.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(8, tGetUrls);

            DataGridViewTextBoxColumn tInvaidType = new DataGridViewTextBoxColumn();
            tInvaidType.HeaderText = "失效规则";
            tInvaidType.Width = 180;
            this.dataTask.Columns.Insert(9, tInvaidType);

            DataGridViewTextBoxColumn tDealType = new DataGridViewTextBoxColumn();
            tDealType.HeaderText = "处理方式";
            tDealType.Width = 180;
            this.dataTask.Columns.Insert(10, tDealType);

            DataGridViewTextBoxColumn tWarningType = new DataGridViewTextBoxColumn();
            tWarningType.HeaderText = "预警规则";
            tWarningType.Width = 1100;

            this.dataTask.Columns.Insert(11, tWarningType);


        }

        //显示计划日志
        private void ShowPlanLog()
        {
            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {
            }

            #region 比部分为固定显示 实际在显示日志时此部分可以省略，但为了保证系统操作的统一性，还是加上了，默认值参看下面注释
            DataGridViewImageColumn tStateImg = new DataGridViewImageColumn();
            tStateImg.HeaderText = rm.GetString("GridState");
            tStateImg.Width = 40;
            tStateImg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tStateImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(0, tStateImg);

            //日志编号，永远为1
            DataGridViewTextBoxColumn tID = new DataGridViewTextBoxColumn();
            tID.Name = rm.GetString("GridID");
            tID.Width = 0;
            tID.Visible = false;
            this.dataTask.Columns.Insert(1, tID);

            //显示日志类型
            DataGridViewTextBoxColumn tState = new DataGridViewTextBoxColumn();
            tState.Name = rm.GetString("GridState");
            tState.Width = 0;
            tState.Visible = false;
            this.dataTask.Columns.Insert(2, tState);

            //用于通过判断Datagridview的数据就可知道当前树形结构选择的节点
            //用于控制(更新)界面显示状态
            DataGridViewTextBoxColumn tTreeNode = new DataGridViewTextBoxColumn();
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);

            #endregion

            DataGridViewTextBoxColumn pName = new DataGridViewTextBoxColumn();
            pName.Name = rm.GetString("GridPlanName");
            pName.Width = 220;
            this.dataTask.Columns.Insert(4, pName);

            DataGridViewTextBoxColumn pType = new DataGridViewTextBoxColumn();
            pType.HeaderText = rm.GetString("GridTaskType");
            pType.Width = 150;
            this.dataTask.Columns.Insert(5, pType);

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.Name = rm.GetString("GridTaskName");
            tName.Width = 120;
            this.dataTask.Columns.Insert(6, tName);


            DataGridViewTextBoxColumn tPara = new DataGridViewTextBoxColumn();
            tPara.HeaderText = rm.GetString("GridPara");
            tPara.Width = 120;
            this.dataTask.Columns.Insert(7, tPara);

            DataGridViewTextBoxColumn pRunTime = new DataGridViewTextBoxColumn();
            pRunTime.HeaderText = rm.GetString("GridRunPlan");
            pRunTime.Width = 580;
            this.dataTask.Columns.Insert(8, pRunTime);
        }

        //public void LoadPublishTask(TreeNode eNode)
        //{

        //    ShowPublishTask();

        //    ///任务发布做的很简单，当任务采集完成后，自动启动开始进行数据的发布，
        //    /// 不允许进行人工干预，当前认为此种发布方式不具备太大的实用性，所以
        //    /// 当前的作为是一种临时的做法，后期会逐步完善，希望可以找到合适的发布
        //    /// 方式
        //    ///需要发布的数据不进行本地文件的保存，直接保存在m_PublishControl中

        //    foreach (NetMiner.Publish.cPublish t in m_PublishControl.PublishManage.ListPublish)
        //    {
        //        try
        //        {
        //            if (t.pThreadState == cGlobalParas.PublishThreadState.Running)
        //                dataTask.Rows.Add(imageList1.Images["export"], t.TaskData.TaskID, cGlobalParas.TaskState.Publishing, eNode.Name,
        //                    t.TaskData.TaskName, t.PublishedCount, t.PublishErrCount,
        //                    t.Count, (t.Count == 0 ? 0 : (t.PublishedCount + t.PublishErrCount) * 100 / t.Count),
        //                                   t.PublishType.GetDescription());
        //            else
        //                dataTask.Rows.Add(imageList1.Images["StopPublish"], t.TaskData.TaskID, cGlobalParas.TaskState.PublishStop, eNode.Name,
        //               t.TaskData.TaskName, t.PublishedCount, t.PublishErrCount,
        //               t.Count, (t.Count == 0 ? 0 : (t.PublishedCount + t.PublishErrCount) * 100 / t.Count),
        //                              t.PublishType.GetDescription());
        //        }
        //        catch { }

        //    }

        //    this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

        //    this.dataTask.ClearSelection();

        //}

        private void LoadTaskInfo()
        {

            EditTaskByNormal();

        }

        private void EditTaskByNormal()
        {
            string FileName = this.dataTask.SelectedCells[5].Value.ToString() + ".smt";
            oTaskClass tc = new oTaskClass(Program.getPrjPath());

            string FilePath = "";

            if (this.dataTask.SelectedCells[3].Value.ToString() == "nodTaskClass")
            {
                //选择的是根目录
                FilePath = Program.getPrjPath() + "tasks";
            }
            else
            {
                int tID = int.Parse(this.dataTask.SelectedCells[3].Value.ToString().Substring(1, this.dataTask.SelectedCells[3].Value.ToString().Length - 1));
                FilePath = tc.GetTaskClassPathByID(tID);
            }

            tc = null;

            frmTask ft = null;
            frmWaiting fWait = new frmWaiting("正在加载采集任务，请等待...");

            try
            {
                //显示等待的窗口 

                new Thread((ThreadStart)delegate
                {
                    Application.Run(fWait);
                }).Start();
                //fWait.Show(this);

                //刷新这个等待的窗口 
                Application.DoEvents();

                string TClass = this.TreeNode.Text;
                string tClassPath = this.TreeNode.Tag.ToString();

                ft = new frmTask();

                ft.EditTask(tClassPath, TClass, FilePath, FileName);
                ft.FormState = cGlobalParas.FormState.Edit;

                //ft.RShowWizard = this.ShowTaskWizard;
                ft.rTClass = refreshNode;

                if (fWait.IsHandleCreated)
                    fWait.Invoke((EventHandler)delegate { fWait.Close(); });
                fWait = null;

                ft.Show();
                //ft.Dispose();
            }
            catch (NetMinerException ex)
            {
                if (fWait != null)
                {
                    if (fWait.IsHandleCreated)
                        fWait.Invoke((EventHandler)delegate { fWait.Close(); });
                    fWait = null;
                }
                if (ex.Message == "指定的字典参数不存在，请检查字典数据！")
                {
                    MessageBox.Show("此采集任务网址配置中采用了字典参数，但此字典参数丢失，导致无法加载此采集任务，请恢复字典参数，重新打开采集任务", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (MessageBox.Show(rm.GetString("Quaere4"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    ft = null;

                    return;
                }
                else
                {
                    ft = null;

                    frmUpgradeTask fu = new frmUpgradeTask(FilePath + "\\" + FileName);
                    fu.ShowDialog();
                    fu.Dispose();
                    fu = null;
                    return;

                }
            }
            catch (System.Runtime.Serialization.SerializationException ex)
            {
                if (fWait != null)
                {
                    if (fWait.IsHandleCreated)
                        fWait.Invoke((EventHandler)delegate { fWait.Close(); });
                    fWait = null;
                }

                if (MessageBox.Show(rm.GetString("Quaere4"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    ft = null;

                    return;
                }
                else
                {
                    ft = null;

                    frmUpgradeTask fu = new frmUpgradeTask(FilePath + "\\" + FileName);
                    fu.ShowDialog();
                    fu.Dispose();
                    fu = null;
                    return;

                }
            }
            catch (System.Exception ex)
            {
                if (fWait != null)
                {
                    if (fWait.IsHandleCreated)
                        fWait.Invoke((EventHandler)delegate { fWait.Close(); });
                    fWait = null;
                }
                MessageBox.Show(rm.GetString("Info32") + ex.Message + "\r\n" +
                        rm.GetString("Info33"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

        }

        public void LoadCompleteTask(TreeNode eNode)
        {

            ShowCompletedTask();

            //从完成的任务中加载
            oTaskComplete t = new oTaskComplete(Program.getPrjPath());
            IEnumerable<eTaskCompleted> tTasks = t.LoadTaskData();

            foreach (eTaskCompleted ec in tTasks)
            {
                dataTask.Rows.Add(imageList1.Images["OK"], ec.TaskID, cGlobalParas.TaskState.Completed, eNode.Name,
                                   ec.TaskName, ec.TaskType.GetDescription(), ec.CompleteDate,
                                   ec.UrlCount, ec.TaskRunType.GetDescription(), ec.PublishType.GetDescription());
            }

            this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

            this.dataTask.ClearSelection();

        }

        //加载正在执行的任务，正在执行的任务记录在应用程序目录下的RunningTask.xml文件中
        //public void LoadRunTask(TreeNode eNode)
        //{
        //    if (eNode == null)
        //    {
        //        eNode = new TreeNode();
        //        eNode.Name = "nodRunning";
        //    }


        //    ShowRunTask();

        //    //开始初始化正在运行的任务
        //    //从m_TaskControl中读取
        //    //每次加载会加载正在运行、等待、停止队列中的任务
        //    List<cGatherTask> taskList = new List<cGatherTask>();
        //    taskList.AddRange(m_GatherControl.TaskManage.TaskListControl.RunningTaskList);
        //    taskList.AddRange(m_GatherControl.TaskManage.TaskListControl.StoppedTaskList);
        //    taskList.AddRange(m_GatherControl.TaskManage.TaskListControl.WaitingTaskList);
        //    //taskList.AddRange(m_GatherControl.TaskManage.TaskListControl.CompletedTaskList);

        //    for (int i = 0; i < taskList.Count; i++)
        //    {
        //        try
        //        {
        //            int urlcount = taskList[i].UrlCount + taskList[i].UrlNaviCount;
        //            int gurlcount = taskList[i].GatheredUrlCount + taskList[i].GatheredUrlNaviCount;
        //            int errurlcount = taskList[i].GatherErrUrlCount + taskList[i].GatheredErrUrlNaviCount;

        //            switch (taskList[i].State)
        //            {
        //                case cGlobalParas.TaskState.Started:
        //                    dataTask.Rows.Add(imageList1.Images["started"], taskList[i].TaskID, taskList[i].State, eNode.Name,
        //                        taskList[i].TaskName, taskList[i].TaskType.GetDescription(),
        //                        taskList[i].StartTimer, gurlcount, errurlcount, urlcount, (gurlcount + errurlcount) * 100 / (urlcount == 0 ? 1 : urlcount), taskList[i].RunType.GetDescription(),
        //                        taskList[i].PublishType.GetDescription());
        //                    break;

        //                case cGlobalParas.TaskState.Stopped:
        //                    if ((gurlcount + errurlcount) > 0)
        //                    {
        //                        dataTask.Rows.Add(imageList1.Images["pause"], taskList[i].TaskID, taskList[i].State, eNode.Name,
        //                            taskList[i].TaskName, taskList[i].TaskType.GetDescription(),
        //                            taskList[i].StartTimer, gurlcount, errurlcount, urlcount, (gurlcount + errurlcount) * 100 / (urlcount == 0 ? 1 : urlcount), taskList[i].RunType.GetDescription(),
        //                            taskList[i].PublishType.GetDescription());
        //                    }
        //                    else
        //                    {
        //                        dataTask.Rows.Add(imageList1.Images["stop"], taskList[i].TaskID, taskList[i].State, eNode.Name,
        //                            taskList[i].TaskName, taskList[i].TaskType.GetDescription(),
        //                            taskList[i].StartTimer, gurlcount, errurlcount, urlcount, (gurlcount + errurlcount) * 100 / (urlcount == 0 ? 1 : urlcount), taskList[i].RunType.GetDescription(),
        //                            taskList[i].PublishType.GetDescription());
        //                    }
        //                    break;
        //                case cGlobalParas.TaskState.UnStart:
        //                    dataTask.Rows.Add(imageList1.Images["stop"], taskList[i].TaskID, taskList[i].State, eNode.Name,
        //                        taskList[i].TaskName, taskList[i].TaskType.GetDescription(),
        //                        taskList[i].StartTimer, gurlcount, errurlcount, urlcount, (gurlcount + errurlcount) * 100 / (urlcount == 0 ? 1 : urlcount), taskList[i].RunType.GetDescription(),
        //                        taskList[i].PublishType.GetDescription());
        //                    break;
        //                case cGlobalParas.TaskState.Failed:
        //                    dataTask.Rows.Add(imageList1.Images["error"], taskList[i].TaskID, taskList[i].State, eNode.Name,
        //                        taskList[i].TaskName, taskList[i].TaskType.GetDescription(),
        //                        taskList[i].StartTimer, gurlcount, errurlcount, urlcount, (gurlcount + errurlcount) * 100 / (urlcount == 0 ? 1 : urlcount), "",
        //                        taskList[i].RunType.GetDescription());
        //                    //rm.GetString("Info14"));
        //                    dataTask.Rows[dataTask.Rows.Count - 1].DefaultCellStyle = this.m_RowStyleErr;
        //                    break;
        //                default:
        //                    dataTask.Rows.Add(imageList1.Images["stop"], taskList[i].TaskID, taskList[i].State, eNode.Name,
        //                        taskList[i].TaskName, taskList[i].TaskType.GetDescription(),
        //                        taskList[i].StartTimer, gurlcount, errurlcount, urlcount, (gurlcount + errurlcount) * 100 / (urlcount == 0 ? 1 : urlcount), taskList[i].RunType.GetDescription(),
        //                        taskList[i].PublishType.GetDescription());
        //                    break;
        //            }
        //        }
        //        catch (System.Exception ex)
        //        {
        //            //捕获错误，不做处理，让信息继续加载
        //            if (e_ExportLog != null)
        //                e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error, cGlobalParas.LogClass.Task, System.DateTime.Now.ToString(), ex.Message));

        //        }
        //    }

        //    this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

        //    this.dataTask.ClearSelection();

        //    taskList = null;

        //}

        //加载任务执行计划
        public void LoadTaskPlan(TreeNode eNode)
        {
            this.Text = "采集任务执行计划";

            ShowTaskPlan();

            oPlans p = new oPlans(Program.getPrjPath());

            IEnumerable<NetMiner.Core.Plan.Entity.ePlan> Plans = p.LoadPlans();

            string runPlan = string.Empty;
            int count = 0;
            foreach (NetMiner.Core.Plan.Entity.ePlan Plan in Plans)
            {

                switch (Plan.RunTaskPlanType)
                {
                    case (int)cGlobalParas.RunTaskPlanType.Ones:
                        runPlan = Plan.RunOnesTime;
                        break;
                    case (int)cGlobalParas.RunTaskPlanType.DayOnes:
                        runPlan = rm.GetString("Everyday") + " " + Plan.RunDayTime;
                        break;
                    case (int)cGlobalParas.RunTaskPlanType.DayTwice:
                        runPlan = rm.GetString("AM") + " " + Plan.RunAMTime + "  " + rm.GetString("PM") + " " + Plan.RunPMTime + " " + rm.GetString("Run"); ;
                        break;
                    case (int)cGlobalParas.RunTaskPlanType.Weekly:
                        runPlan = rm.GetString("Weekly") + " ";

                        string rWeekly = Plan.RunWeekly;
                        foreach (string sc in rWeekly.Split(','))
                        {
                            string ss = sc.Trim();
                            switch (ss)
                            {
                                case "0":
                                    runPlan += rm.GetString("W0") + " ";
                                    break;
                                case "1":
                                    runPlan += rm.GetString("W1") + " ";
                                    break;
                                case "2":
                                    runPlan += rm.GetString("W2") + " ";
                                    break;
                                case "3":
                                    runPlan += rm.GetString("W3") + " ";
                                    break;
                                case "4":
                                    runPlan += rm.GetString("W4") + " ";
                                    break;
                                case "5":
                                    runPlan += rm.GetString("W5") + " ";
                                    break;
                                case "6":
                                    runPlan += rm.GetString("W6") + " ";
                                    break;
                                default:
                                    break;
                            }
                        }

                        runPlan += " " + Plan.RunWeeklyTime;
                        break;
                    case (int)cGlobalParas.RunTaskPlanType.Custom:
                        runPlan = rm.GetString("Info15") + " " + Plan.FirstRunTime + " " + rm.GetString("Info16") + " " + Plan.RunInterval;

                        break;
                }

                if (Plan.IsDisabled == true)
                {
                    string strDisabled = "";
                    if (Plan.DisabledType == cGlobalParas.PlanDisabledType.RunTime)
                        strDisabled = rm.GetString("Run") + Plan.DisabledTime;
                    else
                        strDisabled = rm.GetString("Info17") + Plan.DisabledDateTime;

                    if (Plan.PlanState == cGlobalParas.PlanState.Enabled)

                        dataTask.Rows.Add(imageList1.Images["taskplan"], Plan.PlanID, Plan.PlanState, "nodPlanRunning",
                            Plan.PlanName, Plan.PlanState.GetDescription(), Plan.DisabledType.GetDescription(),
                            strDisabled, runPlan,
                            Plan.NextRunTime, Plan.PlanRemark);
                    else
                        dataTask.Rows.Add(imageList1.Images["disabledplan"], Plan.PlanID, Plan.PlanState, "nodPlanRunning",
                            Plan.PlanName, Plan.PlanState.GetDescription(), Plan.DisabledType.GetDescription(),
                            strDisabled, runPlan,
                            Plan.NextRunTime, Plan.PlanRemark);
                }
                else
                {
                    dataTask.Rows.Add(imageList1.Images["taskplan"], Plan.PlanID, Plan.PlanState, "nodPlanRunning",
                         Plan.PlanName, Plan.PlanState.GetDescription(), rm.GetString("Info18"),
                        "", runPlan,
                        Plan.NextRunTime, Plan.PlanRemark);
                }

                count++;
            }

            p = null;



            this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

            this.dataTask.ClearSelection();

        }

        public void LoadRadarRule(TreeNode eNode)
        {
            this.Text = "监控雷达";
            ShowRuleInfo();


            this.dataTask.DataSource = null;

            oRadarIndex ci = new oRadarIndex(Program.getPrjPath());
            IEnumerable<eRadarIndex> ers = ci.GetRules();

            foreach (eRadarIndex er in ers)
            {

                try
                {
                    dataTask.Rows.Add(imageList1.Images["radar"], er.ID,
                        (int)er.State,
                        "nodRadarRule", er.Name, er.State.GetDescription(), "0", "0", "0",
                        er.InvalidType.GetDescription(),
                        er.DealType.GetDescription(),
                        er.WarningType.GetDescription());
                }
                catch (System.Exception ex)
                {
                    dataTask.Rows.Add(imageList1.Images["error"], er.ID, er.State.GetDescription(), "nodRadarRule", er.Name, "0", "0", "0",
                      "",
                     ex.Message);
                    dataTask.Rows[dataTask.Rows.Count - 1].DefaultCellStyle = this.m_RowStyleErr;
                }
            }

            ci = null;


            this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

            this.dataTask.ClearSelection();

        }

        //此部分的数据是根据当前已经完成的导出任务
        //实时产生的数据
        public void LoadExportDataTask(TreeNode eNode)
        {
            //this.myListData.Items.Clear();
        }

        public void LoadOther(TreeNode mNode)
        {
            this.m_TreeNode = mNode;

            //任务分类作为一个特殊的分类进行处理
            //此节点所有的内容全部为默认，不提供用户可操作的功能
            if (mNode.Name.Substring(0, 1) == "C" || mNode.Name == "nodTaskClass" || mNode.Name == "nodRemoteTaskClass")
            {
                //表示加载的是任务信息
                LoadTask(mNode);
            }
            else
            {

            }
        }


        public void LoadPublishTemplate(TreeNode eNode)
        {
            cGlobalParas.PublishTemplateType pType = cGlobalParas.PublishTemplateType.All;

            if (eNode.Name == "nodPublishByWeb")
                pType = cGlobalParas.PublishTemplateType.Web;
            else if (eNode.Name == "nodPublishByDB")
                pType = cGlobalParas.PublishTemplateType.DB;
            else if (eNode.Name == "nodPublishTemplate")
            {
                pType = cGlobalParas.PublishTemplateType.All;
            }

            this.Text = eNode.Text;
            ShowPublishTemplate();

            cIndex tIndex = new cIndex(Program.getPrjPath());
            tIndex.GetData();
            int count = tIndex.GetCount();

            for (int i = 0; i < count; i++)
            {
                if (pType == cGlobalParas.PublishTemplateType.All)
                {
                    if (tIndex.GetTType(i) == cGlobalParas.PublishTemplateType.DB)
                        dataTask.Rows.Add(imageList1.Images["PublishByDB"], (int)cGlobalParas.PublishTemplateType.DB, cGlobalParas.TaskState.Aborted, eNode.Name, tIndex.GetTName(i), tIndex.GetTRemark(i));
                    else if (tIndex.GetTType(i) == cGlobalParas.PublishTemplateType.Web)
                        dataTask.Rows.Add(imageList1.Images["PublishByWeb"], (int)cGlobalParas.PublishTemplateType.Web, cGlobalParas.TaskState.Aborted, eNode.Name, tIndex.GetTName(i), tIndex.GetTRemark(i));
                }
                else
                {
                    if (tIndex.GetTType(i) == pType)
                    {
                        if (pType == cGlobalParas.PublishTemplateType.DB)
                            dataTask.Rows.Add(imageList1.Images["PublishByDB"], (int)cGlobalParas.PublishTemplateType.DB, cGlobalParas.TaskState.Aborted, eNode.Name, tIndex.GetTName(i), tIndex.GetTRemark(i));
                        else if (pType == cGlobalParas.PublishTemplateType.Web)
                            dataTask.Rows.Add(imageList1.Images["PublishByWeb"], (int)cGlobalParas.PublishTemplateType.Web, cGlobalParas.TaskState.Aborted, eNode.Name, tIndex.GetTName(i), tIndex.GetTRemark(i));
                    }
                }
               
            }
        }

        public void LoadTask(TreeNode eNode)
        {
            this.Text = eNode.Name;

            ShowTaskInfo();


            oTaskIndex xmlTasks = new oTaskIndex(Program.getPrjPath());

            IEnumerable<NetMiner.Core.gTask.Entity.eTaskIndex> eIndexs = null;
            if (eNode.Name == "nodTaskClass")
            {
                eIndexs = xmlTasks.GetTaskDataByClass();
            }
            else if (eNode.Name == "nodRemoteTaskClass")
            {
                //这是一个特殊的默认分类，特指从服务器下载的采集任务
                eIndexs = xmlTasks.GetTaskDataByClass(NetMiner.Constant.g_RemoteTaskClass);
            }
            else
            {
                //因可能无法保证id的唯一性，所以，所有的内容全部都按照名称索取
                //string TaskClassName = eNode.Text;
                string TaskClassName = GetTaskFullClassName(eNode);
                eIndexs = xmlTasks.GetTaskDataByClass(TaskClassName);
            }

            //开始初始化此分类下的任务
            //int count = xmlTasks.GetTaskClassCount();

            foreach (NetMiner.Core.gTask.Entity.eTaskIndex et in eIndexs)
            {
                if (et.TaskState == cGlobalParas.TaskState.Failed)
                {

                    dataTask.Rows.Add(imageList1.Images["error"], et.ID, et.TaskState, eNode.Name,eNode.Tag.ToString(), et.TaskName, 
                         "",
                       "",
                       "任务加载失败，请删除后重建！");
                    dataTask.Rows[dataTask.Rows.Count - 1].DefaultCellStyle = this.m_RowStyleErr;


                }
                else
                {
                    dataTask.Rows.Add(imageList1.Images["task"], et.ID, et.TaskState, eNode.Name, eNode.Tag.ToString(), et.TaskName,

                       et.WebLinkCount.ToString(), 
                       et.PublishType.GetDescription());
                }
            }
            xmlTasks = null;


            this.dataTask.Sort(this.dataTask.Columns[5], ListSortDirection.Ascending);

            this.dataTask.ClearSelection();

        }

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
                && this.dataTask.SelectedCells[3].Value.ToString() == "nodRunning")
            {

                Int64 TaskID = Int64.Parse(this.dataTask.SelectedCells[1].Value.ToString());
                string pageName = "page" + TaskID;

                //for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                //{
                //    if (this.tabControl1.TabPages[i].Name == pageName)
                //    {
                //        this.tabControl1.SelectedIndex = i;
                //        break;
                //    }
                //}

                //设置按钮状态
                SetFormToolbarState();

                this.dataTask.Focus();

            }
        }

        private void dataTask_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataTask.CurrentCell.Value == null)
            {
                this.dataTask.CurrentCell.Value = OldName;
                return;
            }
            else if (this.dataTask.CurrentCell.Value.ToString().Trim() == "" || this.dataTask.CurrentCell.Value.ToString().Trim() == OldName)
            {
                this.dataTask.CurrentCell.Value = OldName;
                return;
            }

            try
            {

                //判断修改的是任务的名称还是计划的名称
                if (this.dataTask.SelectedCells[3].Value.ToString() == "nodPlanRunning")
                {

                    //定义一个修改计划名称的委托
                    delegateRenamePlanName sd = new delegateRenamePlanName(this.RenamePlanName);

                    //开始调用函数,可以带参数 
                    IAsyncResult ir = sd.BeginInvoke(OldName, this.dataTask.CurrentCell.Value.ToString(), null, null);

                    //显示等待的窗口 
                    frmWaiting fWait = new frmWaiting(rm.GetString("Info66"));
                    fWait.Text = rm.GetString("Info66");

                    fWait.Show(this);


                    //循环检测是否完成了异步的操作 
                    while (true)
                    {
                        if (ir.IsCompleted)
                        {
                            //完成了操作则关闭窗口
                            fWait.Close();
                            break;
                        }
                    }

                    //取函数的返回值 
                    bool retValue = sd.EndInvoke(ir);

                    if (retValue == false)
                        this.dataTask.CurrentCell.Value = OldName;

                }
                else
                {

                    //定义一个修改分类名称的委托
                    delegateRenameTaskName sd = new delegateRenameTaskName(this.RenameTaskName);
                    IAsyncResult ir;

                    //开始调用函数,可以带参数 
                    if (this.dataTask.SelectedCells[3].Value.ToString() == "nodTaskClass")
                        ir = sd.BeginInvoke("", OldName, this.dataTask.CurrentCell.Value.ToString(), null, null);
                    else
                        ir = sd.BeginInvoke(this.TreeNode.Tag.ToString(), OldName, this.dataTask.CurrentCell.Value.ToString(), null, null);

                    //显示等待的窗口
                    frmWaiting fWait = new frmWaiting(rm.GetString("Info67"));
                    fWait.Text = rm.GetString("Info67");

                    fWait.Show(this);
                    //刷新这个等待的窗口 
                    Application.DoEvents();

                    //循环检测是否完成了异步的操作 
                    while (true)
                    {
                        if (ir.IsCompleted)
                        {
                            //完成了操作则关闭窗口
                            fWait.Close();
                            break;
                        }
                    }

                    //取函数的返回值 
                    bool retValue = sd.EndInvoke(ir);

                    if (retValue == false)
                        this.dataTask.CurrentCell.Value = OldName;

                }
            }
            catch (System.Exception ex)
            {
                if (ex.Message.IndexOf("SetCurrentCellAddressCore") > 0)
                {
                }
                else
                {
                    MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataTask_DoubleClick(object sender, EventArgs e)
        {
            if (this.dataTask.SelectedCells.Count == 0)
                return;

            switch (this.dataTask.SelectedCells[3].Value.ToString())
            {
                case "nodSnap":
                    break;
                case "nodRunning":
                    //if (e_ExcuteFunction != null)
                    //    e_ExcuteFunction(this, new ExcuteFunctionEvent("EditTask", null));
                    EditTask();
                    break;
                case "nodPublish":
                    //BrowserMultiData();
                    break;
                case "nodComplete":
                    OpenCompletedData();
                    //BrowserMultiData();
                    break;
                case "nodRadarRule":
                    //EditRadar();
                    break;
                case "nodTaskPlan":
                    break;
                case "nodPlanRunning":
                    EditPlan();
                    break;
                case "nodPlanCompleted":
                    break;
                case "nodTaskClass":
                    EditTask();
                    break;
                case "nodPublishTemplate":
                    EditPublishTemplate();
                    break;
                case "nodPublishByWeb":
                    EditPublishTemplate();
                    break;
                case "nodPublishByDB":
                    EditPublishTemplate();
                    break;
                default:
                    EditTask();
                    break;
            }
        }

        private void dataTask_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox)
            {

                TextBox tb = e.Control as TextBox;

                tb.KeyPress -= new KeyPressEventHandler(tb_KeyPress);

                tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);

            }
        }

        void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar)))
            {

                Keys key = (Keys)e.KeyChar;

                //if (!(key == Keys.Back || key == Keys.Delete))
                //{
                //    e.Handled = true;
                //}
            }
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
            if (this.m_TreeNode == null)
                return;
            if (this.dataTask.SelectedRows.Count == 0)
                return;


            switch (e.KeyCode)
            {
                case Keys.Delete:
                    switch (this.dataTask.SelectedCells[3].Value.ToString())
                    {
                        case "nodSnap":
                            break;
                        case "nodTaskPlan":
                            break;
                        case "nodPlanRunning":
                            if (DelPlan() == false)
                            {
                                e.SuppressKeyPress = true;
                                return;
                            }
                            break;

                        case "nodRadarRule":
                            //if (DelRadarRule() == false)
                            //{
                            //    e.SuppressKeyPress = true;
                            //    return;
                            //}
                            break;
                        case "nodTaskClass":
                            if (DelTask("") == false)
                            {
                                e.SuppressKeyPress = true;
                                return;
                            }
                            break;
                        case "nodPublishTemplate":
                            if (DelPublishTemplate() == false)
                            {
                                e.SuppressKeyPress = true;
                                return;
                            }
                            break;
                        case "nodPublishByWeb":
                            if (DelPublishTemplate() == false)
                            {
                                e.SuppressKeyPress = true;
                                return;
                            }
                            break;
                        case "nodPublishByDB":
                            if (DelPublishTemplate() == false)
                            {
                                e.SuppressKeyPress = true;
                                return;
                            }
                            break;

                        default:

                            if (DelTask(GetTaskFullClassName(this.TreeNode)) == false)
                            {
                                e.SuppressKeyPress = true;
                                return;
                            }
                            break;
                    }
                    break;
                case Keys.F2:
                    if (this.dataTask.SelectedCells[3].Value.ToString().Substring(0, 1) == "C" || this.dataTask.SelectedCells[3].Value.ToString() == "nodTaskClass"
                        || this.dataTask.SelectedCells[3].Value.ToString() == "nodPlanRunning")
                    {
                        this.dataTask.CurrentCell = this.dataTask[5, this.dataTask.CurrentCell.RowIndex];

                        this.dataTask.BeginEdit(true);
                    }

                    break;
                case Keys.Enter:
                    switch (this.m_TreeNode.Name)
                    {
                        case "nodSnap":
                            break;
                        case "nodRunning":

                            break;
                        case "nodPublish":
                            break;
                        case "nodComplete":

                            break;
                        case "nodTaskPlan":
                            break;
                        case "nodPlanRunning":

                            break;

                        case "nodRadarRule":

                            break;
                        case "nodTaskClass":
                            EditTask();
                            break;

                        default:
                            EditTask();
                            break;
                    }

                    break;
                default:
                    if (this.m_TreeNode.Name.Substring(0, 1) == "C" || this.m_TreeNode.Name == "nodTaskClass")
                    {
                        if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.C)
                        {
                            CopyTask();
                        }
                        else if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.V)
                        {
                            PasteTask();
                        }
                    }
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

        private void dataTask_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point pt = new Point(e.X, e.Y);
                Rectangle recTab = new Rectangle();
                //for (int i = 0; i < tabControl1.TabCount; i++)
                //{
                //    recTab = tabControl1.GetTabRect(i);
                //    if (recTab.Contains(pt))
                //        this.tabControl1.SelectedIndex = i;
                //}
            }
        }

        private bool DelPublishTemplate()
        {
            if (this.dataTask.SelectedRows.Count ==0)
            {
                return false;
            }

            if (MessageBox.Show("确实删除发布模版：" + this.dataTask.SelectedCells[5].Value.ToString() + "？", "网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.No)
                return false;

            string tName = this.dataTask.SelectedCells[5].Value.ToString();
            cIndex tindex = new cIndex(Program.getPrjPath(), Program.getPrjPath() + "publish\\index.xml");
            tindex.DeleTemplateIndex(tName);
            tindex = null;

            string fName = Program.getPrjPath() + "publish\\" + tName + ".spt";
            System.IO.File.Delete(fName);

            //this.listTemplate.Items.Remove(this.listTemplate.SelectedItems[0]);

            return true;
        }

        private bool DelTask(string tClass)
        {
            if (this.dataTask.SelectedRows.Count == 0)
            {
                return false;
            }

            if (this.dataTask.SelectedRows.Count == 1)
            {
                if (MessageBox.Show(rm.GetString("Info28") + this.dataTask.SelectedCells[5].Value + "\r\n"
                    + rm.GetString("Quaere5"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }
            else
            {
                if (MessageBox.Show(rm.GetString("Quaere6"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }

            //表示选择的是任务节点
            try
            {
                for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
                {
                    oTask t = new oTask(Program.getPrjPath());

                    //oTaskClass tc = new oTaskClass(Program.getPrjPath());
                    //string tPath = tc.GetTaskClassPathByName(tClass);
                    //tc = null;

                    string tPath = this.TreeNode.Tag.ToString();

                    t.DeleTask(tPath, this.dataTask.SelectedRows[index].Cells[5].Value.ToString());
                    t = null;

                }
                return true;

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }


        }

        public void Del()
        {
            this.dataTask.Focus();
            SendKeys.Send("{Del}");
        }

       

        public bool DelRadarRule()
        {
            if (this.dataTask.SelectedRows.Count == 0)
            {
                return false;
            }

            if (this.dataTask.SelectedRows.Count == 1)
            {
                if (MessageBox.Show(rm.GetString("Quaere27") + this.dataTask.SelectedCells[5].Value + "\r\n",
                     rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }
            else
            {
                if (MessageBox.Show(rm.GetString("Quaere26"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }

            try
            {
                for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
                {
                    oRadar r = new oRadar(Program.getPrjPath());
                    r.DelRule(this.dataTask.SelectedRows[index].Cells[5].Value.ToString());
                    r = null;

                }
                return true;

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }



        }

        private void frmTaskContent_FormClosing(object sender, FormClosingEventArgs e)
        {
            rm = null;
        }

        //public void Start()
        //{
        //    if (this.dataTask.SelectedRows.Count == 0)
        //        return;

        //    if (this.dataTask.SelectedCells[3].Value.ToString().Substring(0, 1) == "C" || this.dataTask.SelectedCells[3].Value.ToString() == "nodTaskClass")
        //    {
        //        StartMultiTask();
        //    }
        //    else if (this.dataTask.SelectedCells[3].Value.ToString() == "nodPublish")
        //    {
        //        StartMultiPublish();
        //    }


        //}

        private void OpenCompletedData()
        {
            if (this.dataTask.Rows.Count == 0)
                return;

            Int64 TaskID = Int64.Parse(this.dataTask.SelectedRows[0].Cells[1].Value.ToString());
            string TaskName = this.dataTask.SelectedRows[0].Cells[5].Value.ToString();
            string dFile = "";
            DataTable tmp = new DataTable();
            oTaskComplete tc = new oTaskComplete(Program.getPrjPath());
            eTaskCompleted ec = tc.LoadSingleTask(TaskID);
            dFile = ec.TempFile;
            tc = null;

            if (e_OpenDataEvent != null)
            {
                e_OpenDataEvent(this, new OpenDataEvent(cGlobalParas.DatabaseType.SoukeyData, dFile, "",TaskName));
            }
        }

        private void rmenuCopyTask_Click(object sender, EventArgs e)
        {
            CopyTask();
        }

        private void rmenuPasteTask_Click(object sender, EventArgs e)
        {
            PasteTask();
        }

        private void rmmenuRenameTask_Click(object sender, EventArgs e)
        {
            this.dataTask.CurrentCell = this.dataTask[5, this.dataTask.CurrentCell.RowIndex];

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

        ///根据菜单选择及点击的数据内容，自动控制工具栏的按钮状态
        ///因为DataGridView支持多选，所以可能存在多种按钮状态的情况
        ///针对这种情况，系统按照最后选择的内容进行按钮状态设置

        private void SetFormToolbarState()
        {
            if (e_SetToolbarState != null && this.dataTask.SelectedRows.Count != 0)
            {
                e_SetToolbarState(this, new SetToolbarStateEvent(this.dataTask.Rows.Count, (cGlobalParas.TaskState)this.dataTask.SelectedCells[2].Value));
            }

        }

        private void EditPlan()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            try
            {
                Int64 pid = Int64.Parse(this.dataTask.SelectedCells[1].Value.ToString());

                frmTaskPlan fp = new frmTaskPlan();
                fp.LoadPlan(pid);
                fp.FormState = cGlobalParas.FormState.Edit;
                fp.ShowDialog();
                fp.Dispose();

                LoadTaskPlan(null);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info30") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditPublishTemplate()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            try
            {
                if (int.Parse(this.dataTask.SelectedCells[1].Value.ToString()) == (int)cGlobalParas.PublishTemplateType.Web)
                {
                    string tName = this.dataTask.SelectedCells[5].Value.ToString();
                    frmWebRule f = new frmWebRule();
                    f.IniData(NetMiner.Resource.cGlobalParas.FormState.Edit, tName);
                    f.RTemplate = GetEditTemplate;
                    f.ShowDialog();
                    f.Dispose();
                }
                else
                {
                    string tName = this.dataTask.SelectedCells[5].Value.ToString();
                    frmDBRule f = new frmDBRule();
                    f.IniData(NetMiner.Resource.cGlobalParas.FormState.Edit, tName);
                    f.RTemplate = GetEditTemplate;
                    f.ShowDialog();
                    f.Dispose();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("加载发布模板发生错误，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetEditTemplate(string tName, cGlobalParas.PublishTemplateType tType, string remark)
        {
            string oldName = this.dataTask.SelectedCells[5].Value.ToString();
            //this.listTemplate.SelectedItems[0].Name = tName;
            //this.listTemplate.SelectedItems[0].Text = tName;
            //this.listTemplate.SelectedItems[0].SubItems[1].Text = tType.GetDescription();
            //this.listTemplate.SelectedItems[0].SubItems[2].Text = remark;

            //修改index中的名称和备注
            cIndex tindex = new cIndex(Program.getPrjPath(), Program.getPrjPath() + "publish\\index.xml");
            tindex.EditName(oldName, tName, remark);
            tindex = null;

        }

        private void EditTask()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            if (this.dataTask.SelectedCells[3].Value.ToString().Substring(0, 1) == "C" || this.dataTask.SelectedCells[3].Value.ToString() == "nodTaskClass")
            {
                //表示选择的是任务节点
                try
                {
                    LoadTaskInfo();
                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show(rm.GetString("Info31"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(rm.GetString("Info32") + ex.Message + "\r\n" +
                        rm.GetString("Info33"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            }
            else if (this.dataTask.SelectedCells[3].Value.ToString() == "nodRunning")
            {
                cGlobalParas.TaskState tState = (cGlobalParas.TaskState)this.dataTask.SelectedCells[2].Value;
                if (tState == cGlobalParas.TaskState.Started || tState == cGlobalParas.TaskState.Running)
                    return;

                //如果双击选择了正在运行的任务，则默认打开此任务的数据
                //BrowserMultiData();

                //if (MessageBox.Show(rm.GetString("Quaere13"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                //{
                //    return;
                //}

                //string Filename = "Task" + this.dataTask.SelectedCells[1].Value.ToString() + ".rst";
                //string tPath = Program.getPrjPath() + "Tasks\\run\\";

                //try
                //{
                //    LoadTaskInfo(tPath, Filename, cGlobalParas.FormState.Browser);
                //}
                //catch (System.Exception ex)
                //{
                //    MessageBox.Show(rm.GetString("Info32") + ex.Message + "\r\n" +
                //        rm.GetString("Info268"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                //    return;
                //}
            }
        }

        private void refreshNode(string TClass)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("refreshNode", new object[] { TClass }));
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

        public void CopyTask()
        {
            if (this.dataTask.SelectedCells[3].Value.ToString() == "nodTaskClass")
            {
                Clipboard.SetDataObject("SoukeyNetGetTask:" + "" + "/" + this.dataTask.SelectedCells[5].Value.ToString());
            }
            else
            {
                oTaskClass tc = new oTaskClass(Program.getPrjPath());
                string tName = tc.GetTaskClassNameByID(this.dataTask.SelectedCells[3].Value.ToString().Substring(1, this.dataTask.SelectedCells[3].Value.ToString().Length - 1));

                tc = null;
                Clipboard.SetDataObject("SoukeyNetGetTask:" + tName + "/" + this.dataTask.SelectedCells[5].Value.ToString());
            }

            if (e_SetControlProperty != null)
                e_SetControlProperty(this, new SetControlPropertyEvent("toolPasteTask", "Enabled", "true"));

        }

        public void PasteTask()
        {

            IDataObject cdata;
            cdata = Clipboard.GetDataObject();

            string TaskClass;
            string TaskName;

            //判断数据是否为文本
            if (IsClipboardSoukeyData())
            {
                if (this.m_TreeNode.Name == "nodRemoteTaskClass")
                {
                    MessageBox.Show("不能将本地任务拷贝至远程服务器！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                    string tInfo = cdata.GetData(DataFormats.Text).ToString();
                tInfo = tInfo.Substring(17, tInfo.Length - 17);

                //尝试分解获取的文本，有可能剪贴板中的信息不是网络矿工的信息
                if (tInfo.IndexOf("/") >= 0)
                {
                    try
                    {
                        bool IsTaskClass = false;
                        bool IsTaskName = false;

                        TaskClass = tInfo.Substring(0, tInfo.LastIndexOf("/"));
                        TaskName = tInfo.Substring(tInfo.LastIndexOf("/") + 1, tInfo.Length - tInfo.LastIndexOf("/") - 1);

                        if (TaskClass == "" && TaskName == "")
                            return;

                        string tClassPath = string.Empty;
                        //信息分解后再次验证指定的任务分类是否存在
                        if (string.IsNullOrEmpty(TaskClass))
                        {
                            //根目录
                            tClassPath = NetMiner.Constant.g_TaskPath;
                        }
                        else
                        {
                            oTaskClass tc = new oTaskClass(Program.getPrjPath());
                            tClassPath = tc.GetTaskClassPathByName(TaskClass);
                           tc.Dispose();
                            tc = null;
                        }

                        //粘贴任务操作
                        string NewTClass = "";

                        if (this.m_TreeNode.Name == "nodTaskClass")
                            NewTClass =NetMiner.Constant.g_TaskPath;
                        else
                        {
                            NewTClass = this.m_TreeNode.Tag.ToString();
                        }

                        cTaskManage mTask = new cTaskManage(Program.getPrjPath());
                        string newTaskFile= mTask.CopyTask(tClassPath, NewTClass, TaskName,cGlobalParas.CopyType.Copy);
                        mTask = null;

                        //增加datagridview的行，表示拷贝成功
                        oTask t = new oTask(Program.getPrjPath());
                        t.LoadTask(newTaskFile);

                        //dataTask.Rows.Add(imageList1.Images["task"], t.TaskEntity.TaskID, cGlobalParas.TaskState.UnStart,
                        //    this.m_TreeNode.Name, t.TaskEntity.TaskName, t.TaskEntity.UrlCount.ToString(),
                        //    t.TaskEntity.ExportType.GetDescription());

                        dataTask.Rows.Add(imageList1.Images["task"], t.TaskEntity.TaskID, cGlobalParas.TaskState.UnStart,
                            this.m_TreeNode.Name, this.m_TreeNode.Tag.ToString(), t.TaskEntity.TaskName,
                            t.TaskEntity.UrlCount.ToString(),
                            t.TaskEntity.ExportType.GetDescription());


                        t.Dispose();
                        t = null;
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(rm.GetString("Info68") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            }
        }

        /// <summary>
        /// 判断当前剪切板中是否为网络矿工的任务
        /// </summary>
        /// <returns></returns>
        private bool IsClipboardSoukeyData()
        {
            //判断数据是否为文本
            if (Clipboard.ContainsData(DataFormats.Text))
            {
                IDataObject cdata;
                cdata = Clipboard.GetDataObject();
                if (cdata != null)
                {
                    if (cdata.GetDataPresent(DataFormats.Text))
                    {
                        string tInfo = cdata.GetData(DataFormats.Text).ToString();
                        if (tInfo.Length > 18)
                        {
                            if (tInfo.Substring(0, 17) == "SoukeyNetGetTask:")
                                return true;
                            else
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        private void rmmenuNewTask_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("NewTask", null));
        }

        private void rmenuExportTask_Click(object sender, EventArgs e)
        {
            if (this.dataTask.SelectedRows.Count == 0)
            {
                return;
            }


            if (this.dataTask.SelectedRows.Count > 1)
            {
                if (MessageBox.Show(rm.GetString("Info253"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }

            this.saveFileDialog1.OverwritePrompt = true;
            this.saveFileDialog1.Title = rm.GetString("Info251");
            saveFileDialog1.InitialDirectory = Program.getPrjPath();
            saveFileDialog1.Filter = "采集任务文件(*.smt)|*.smt";
            saveFileDialog1.FileName = this.dataTask.SelectedRows[0].Cells[5].Value.ToString() + ".smt";

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string FileName = this.saveFileDialog1.FileName;

                oTaskClass tc = new oTaskClass(Program.getPrjPath());
                string tPath = Program.getPrjPath() + tc.GetTaskClassPathByName(GetTaskFullClassName(this.m_TreeNode));
                tc = null;
                string OldName =   tPath + "\\" + this.dataTask.SelectedRows[0].Cells[5].Value.ToString() + ".smt";

                try
                {
                    File.Copy(OldName, FileName);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(rm.GetString("Info252") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private string GetTaskFullClassName(TreeNode eNode)
        {
            string cName = string.Empty;


            if (eNode.Name == "nodTaskClass")
            {
                return "";
            }
            else if (eNode.Parent.Name == "nodTaskClass")
            {
                return eNode.Text;
            }
            else if (eNode.Name== "nodPublishTemplate")
            {

            }
            else if (eNode.Name == "nodPublishByWeb")
            {

            }
            else if (eNode.Name == "nodPublishByDB")
            {

            }
            else
            {
                cName = GetTaskFullClassName(eNode.Parent) + "/" + eNode.Text;
            }

            return cName;
        }


        private void rmenuClearCompleted_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        public void ClearData()
        {
            //获取所有已经采集的任务，并对采集任务进行分析，如果下载数据已经为零，则删除
            oTaskComplete tc = new oTaskComplete(Program.getPrjPath());
            IEnumerable<eTaskCompleted> eTasks = tc.LoadTaskData();

            List<long> delTasks = new List<long>();

            foreach (eTaskCompleted ec in eTasks)
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

        public bool UploadTask()
        {
            string strMess = string.Empty;

            frmSelectRemoteTaskClass f = new frmSelectRemoteTaskClass();
            f.RTaskClass = this.GetRemoteTaskClassID;
            f.ShowDialog();
            f.Dispose();

            if (this.m_RemoteTaskClassID == -1)
            {
                //表示取消上传
                return false;
            }

            #region 上传任务
            for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
            {
                if ((this.dataTask.SelectedRows[index].Cells[1].Value.ToString() != ""
                    && this.dataTask.SelectedRows[index].Cells[3].Value.ToString() == "nodTaskClass") ||
                    (this.dataTask.SelectedRows[index].Cells[1].Value.ToString() != ""
                    && this.dataTask.SelectedRows[index].Cells[3].Value.ToString().StartsWith("C")))
                {
                    oTaskClass tc = new oTaskClass(Program.getPrjPath());
                    string tPath = string.Empty;

                    if (this.m_TreeNode.Name == "nodTaskClass")
                    {
                        tPath = Program.getPrjPath() + "tasks";
                    }
                    else
                    {
                        string tClassID = this.m_TreeNode.Name;
                        tClassID = tClassID.Substring(1, tClassID.Length - 1);
                        tPath = tc.GetTaskClassPathByID(int.Parse(tClassID));
                    }
                    tc = null;
                    string fileName = tPath + "\\" + this.dataTask.SelectedRows[index].Cells[5].Value + ".smt";
                    oTask t = new oTask(Program.getPrjPath());

                    t.LoadTask(fileName);

                    string taskName = t.TaskEntity.TaskName;

                    if (t.TaskEntity.RunType != cGlobalParas.TaskRunType.OnlySave
                        && t.TaskEntity.IsPluginsPublish != true && t.TaskEntity.ExportType != cGlobalParas.PublishType.publishTemplate)
                    {

                        //MessageBox.Show("上传的采集任务必须配置发布规则，或者直接入库或者采用插件发布数据或者配置发布模板，如为直接入库，则采集服务器必须可以访问数据库服务器，否则采集将为无效，请配置发布规则后重新上传！",
                        //    "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        strMess += "采集任务：" + t.TaskEntity.TaskName + " 上传失败，上传的采集任务必须配置发布规则（或直接入库或使用发布插件）！" + "\r\n";
                        t = null;
                        continue;
                    }

                    //判断网址中是否存在字典参数
                    bool isShowInfo = false;
                    bool isDict = false;
                    List<eWebLink> newWebLink = new List<eWebLink>();
                    int urlCount = 0;
                    try
                    {
                        for (int i = 0; i < t.TaskEntity.WebpageLink.Count; i++)
                        {
                            NetMiner.Core.Url.cUrlParse u = new NetMiner.Core.Url.cUrlParse(Program.getPrjPath());


                            if (Regex.IsMatch(t.TaskEntity.WebpageLink[i].Weblink, "(?<={Dict:)[^}]*(?=})"))
                            {
                                isDict = true;

                                //表示有字典数据
                                if (isShowInfo == false)
                                {
                                    if (MessageBox.Show("系统检测到您配置的任务中，存在字典参数，有可能采集服务器中并不存在您指定的参数，所以网络矿工需将参数解析后方可上传，是否解析网址并上传？",
                                   "网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                    {
                                        t = null;
                                        return false;
                                    }
                                }

                                isShowInfo = true;

                                //开始分解网址
                                //先将当前的weblink拷贝出来，注意是拷贝
                                string url = t.TaskEntity.WebpageLink[i].Weblink;
                                List<string> urls = u.SplitWebUrl(url);

                                for (int j = 0; j < urls.Count; j++)
                                {

                                    eWebLink link = new eWebLink();
                                    link.Weblink = urls[j];
                                    link.id = t.TaskEntity.WebpageLink[i].id;
                                    link.IsNavigation = t.TaskEntity.WebpageLink[i].IsNavigation;
                                    link.NavigRules = t.TaskEntity.WebpageLink[i].NavigRules;
                                    link.IsNextpage = t.TaskEntity.WebpageLink[i].IsNextpage;
                                    link.NextPageRule = t.TaskEntity.WebpageLink[i].NextPageRule;
                                    link.NextMaxPage = t.TaskEntity.WebpageLink[i].NextMaxPage;
                                    link.NextPageUrl = t.TaskEntity.WebpageLink[i].NextPageUrl;
                                    link.IsGathered = t.TaskEntity.WebpageLink[i].IsGathered;
                                    link.CurrentRunning = t.TaskEntity.WebpageLink[i].CurrentRunning;
                                    link.IsMultiGather = t.TaskEntity.WebpageLink[i].IsMultiGather;
                                    link.IsData121 = t.TaskEntity.WebpageLink[i].IsData121;
                                    link.MultiPageRules = t.TaskEntity.WebpageLink[i].MultiPageRules;

                                    newWebLink.Add(link);
                                    link = null;

                                    urlCount = urlCount + u.GetUrlCount(urls[j]);

                                }

                            }
                            else
                            {
                                newWebLink.Add(t.TaskEntity.WebpageLink[i]);
                                urlCount = urlCount + u.GetUrlCount(t.TaskEntity.WebpageLink[i].Weblink);
                            }

                            u = null;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        //MessageBox.Show("分解采集网址发生错误，请确定字典参数及采集任务是正确的！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        strMess = "采集任务：" + t.TaskEntity.TaskName + " 存在字典数据，并且分解字典失败，导致此任务上传失败！" + "\r\n";
                        continue;
                    }

                    if (isDict == true)
                    {
                        //更新网址
                        t.TaskEntity.UrlCount = urlCount;
                        t.TaskEntity.WebpageLink = null;
                        t.TaskEntity.WebpageLink = newWebLink;

                        string tmpPath = Program.getPrjPath() + "tasks\\tmp";
                        if (!Directory.Exists(tmpPath))
                        {
                            Directory.CreateDirectory(tmpPath);
                        }

                        string fName = tmpPath + "\\" + t.TaskEntity.TaskName + ".smt";
                        t.SaveTask(fName);
                        fileName = fName;
                    }

                    t = null;

                    //localhost.NetMinerWebService sweb = new localhost.NetMinerWebService();
                    //sweb.Url = Program.ConnectServer + "/NetMiner.WebService.asmx";
                    //if (Program.g_IsAuthen == true)
                    //    sweb.Credentials = new System.Net.NetworkCredential(Program.g_WindowsUser, Program.g_WindowsPwd);


                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    byte[] bytes = br.ReadBytes((int)fs.Length);
                    fs.Flush();
                    fs.Close();

                    try
                    {
                        string fName = Path.GetFileName(fileName);

                        //先判断采集任务是否存在
                        //int uState = sweb.PreUploadFile(fName);
                        //if (uState == (int)cGlobalParas.UploadTaskState.HaveRunning)
                        //{
                        //    //MessageBox.Show("上传的采集任务已经存在，并且正在运行，不能覆盖操作，请更换采集任务名称后重试！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    strMess = "采集任务：" + taskName + " 已经存在，并且正在运行，不能进行覆盖操作。" + "\r\n";
                        //    continue;
                        //}

                        //bool isCover = false;
                        //if (uState == (int)cGlobalParas.UploadTaskState.HaveUnrunning || uState == (int)cGlobalParas.UploadTaskState.HaveCompleted)
                        //{
                        //    if (MessageBox.Show("上传的采集任务已经存在，是否覆盖？", "网络矿工 错误", MessageBoxButtons.YesNo
                        //        , MessageBoxIcon.Question) == DialogResult.No)
                        //        continue;
                        //    else
                        //        isCover = true;
                        //}

                        //bool isS = sweb.UploadFile(Program.RegisterUser, bytes, fName, this.m_RemoteTaskClassID, isCover);
                        //if (isS == false)
                        //{
                        //    //MessageBox.Show("上传文件出错，有可能是已经存在此采集任务！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    strMess = "采集任务：" + taskName + " 上传文件出错，有可能是已经存在此采集任务！" + "\r\n";
                        //    continue;
                        //}
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("上传文件出错，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        strMess = "采集任务：" + taskName + " 上传文件出错，错误信息：" + ex.Message + "\r\n";
                        continue;
                    }

                    //MessageBox.Show("文件上传成功！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (isDict == true)
                    {
                        //删除临时文件
                        File.Delete(fileName);
                    }
                    //return true;

                }
            }
            #endregion

            if (strMess == "")
            {
                MessageBox.Show("文件上传成功！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(strMess, "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return true;
        }

        private void GetRemoteTaskClassID(int tcID)
        {
            this.m_RemoteTaskClassID = tcID;
        }

        private void rmenuUploadTask_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("UploadTask", null));
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

        private void rMenuRefer_Click(object sender, EventArgs e)
        {
            ReferShow(m_TreeNode);
        }

        private void ReferShow(TreeNode eNode)
        {

            switch (eNode.Name)
            {
                //case "nodRunning":   //运行区的任务


                //    try
                //    {
                //        LoadRunTask(eNode);
                //    }
                //    catch (System.IO.IOException)
                //    {
                //        if (MessageBox.Show(rm.GetString("Quaere18"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                //        {
                //            CreateTaskRun();
                //        }
                //    }
                //    catch (System.Exception)
                //    {
                //        MessageBox.Show(rm.GetString("Info54") + Program.getPrjPath() + "tasks\\taskrun.xml", rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                //    }

                //    break;

                //case "nodPublish":

                //    LoadPublishTask(eNode);

                //    break;

                case "nodComplete":    //已经完成采集的任务

                    try
                    {
                        LoadCompleteTask(eNode);
                    }
                    catch (System.IO.IOException)
                    {

                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show(rm.GetString("Info55") + Program.getPrjPath() + "data\\index.xml", rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    };

                    break;

                case "nodPlanRunning":

                    try
                    {
                        LoadTaskPlan(eNode);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(rm.GetString("Info57") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }

                    break;

                case "nodRadarRule":
                    //加载雷达数据
                    LoadRadarRule(eNode);
                    break;
                case "nodLog":
                    //加载日志数据

                    break;
                default:

                    if (!eNode.Name.StartsWith("C") && eNode.Name != "nodTaskClass" && eNode.Name != "nodRemoteTaskClass")
                        return;

                    try
                    {
                        LoadOther(eNode);
                    }
                    catch (System.IO.IOException)
                    {

                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(rm.GetString("Info59") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }

                    break;
            }

        }

        private void rmenuUploadWebTask_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Free)
            {
                MessageBox.Show("免费版不支持此功能！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //开始验证身份
            string url = "http://www.netminer.cn/user/hander/upload.ashx";
            string postData = "mode=CheckUser&u=" + Program.RegisterUser;
            string cookie = string.Empty;
            string header = "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0\r\n"
                + "Content-Type: application/x-www-form-urlencoded";

            string rHeader = string.Empty;

            string htmlSource = NetMiner.Net.Socket.HttpUnity.GetHtml(url, cGlobalParas.RequestMethod.Post, postData,
                cGlobalParas.WebCode.auto, ref cookie, header, cGlobalParas.ProxyType.None, "", 80,
                out rHeader);

            if (htmlSource.ToLower() == "false")
            {
                MessageBox.Show("您的产品未与网站账号绑定，请先登录网路矿工官网站点，注册账号，登录到会员中心，实现产品绑定。", "网络矿工 信息",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            postData = "mode=GetClass";
            htmlSource = NetMiner.Net.Socket.HttpUnity.GetHtml(url, cGlobalParas.RequestMethod.Post, postData,
                cGlobalParas.WebCode.auto, ref cookie, header, cGlobalParas.ProxyType.None, "", 80,
                out rHeader);

            DataTable d = JsonConvert.DeserializeObject<DataTable>(htmlSource);

            oTaskClass tc = new oTaskClass(Program.getPrjPath());
            string tPath = Program.getPrjPath() + tc.GetTaskClassPathByName(GetTaskFullClassName(this.TreeNode));
            tc = null;
            tPath = tPath + "\\" + this.dataTask.SelectedCells[5].Value + ".smt";
            frmUploadTask f = new frmUploadTask();
            f.IniData(tPath, this.dataTask.SelectedCells[5].Value.ToString(), d);
            f.ShowDialog();
            f.Dispose();
        }

        private void ClearRemoteTask()
        {
            oTaskIndex xmlTasks = new oTaskIndex(Program.getPrjPath());
            //这是一个特殊的默认分类，特指从服务器下载的采集任务
            IEnumerable<NetMiner.Core.gTask.Entity.eTaskIndex> eIndexs = xmlTasks.GetTaskDataByClass(NetMiner.Constant.g_RemoteTaskClass);

            //开始初始化此分类下的任务
            //int count = xmlTasks.GetTaskClassCount();

            foreach (NetMiner.Core.gTask.Entity.eTaskIndex et in eIndexs)
            {
                //开始逐一删除任务
                oTask t = new oTask(Program.getPrjPath());

                oTaskClass tc = new oTaskClass(Program.getPrjPath());
                string tPath = Program.getPrjPath() + tc.GetTaskClassPathByName(Program.g_RemoteTaskClass);
                tc = null;
                t.DeleTask(tPath, et.TaskName);
                t = null;

                //判断此任务在正在运行的队列中是否存在，如果存在，停止，删除
            }
            xmlTasks = null;
        }

        private bool DelPlan()
        {
            if (this.dataTask.SelectedRows.Count == 0)
            {
                return false;
            }

            if (this.dataTask.SelectedRows.Count == 1)
            {
                if (MessageBox.Show(rm.GetString("Info24") + this.dataTask.SelectedCells[5].Value.ToString() + "\r\n"
                    + rm.GetString("Info25"), rm.GetString("MessageboxQuaere"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }
            else
            {
                if (MessageBox.Show(rm.GetString("Info26"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }



            try
            {
                oPlans ps = new oPlans(Program.getPrjPath());

                for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
                {
                    ps.DelPlan(this.dataTask.SelectedRows[index].Cells[1].Value.ToString());
                }

                ps = null;

                return true;

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info27") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }

        }

        //public void StartTask(Int64 TaskID)
        //{
            
              
        //}

        //public void StartTask(Int64 TaskID, string TaskName, int SelectedIndex)
        //{
           
             
        //}


        /// <summary>
        /// 启动采集任务
        /// </summary>
        //private void StartMultiTask()
        //{
        //    if (this.dataTask.SelectedRows.Count == 0)
        //        return;

        //    //if (e_ExcuteFunction!=null)
        //    //    e_ExcuteFunction(this,new ExcuteFunctionEvent ())

        //    for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
        //    {
        //        if ((cGlobalParas.TaskState)this.dataTask.SelectedRows[index].Cells[2].Value == cGlobalParas.TaskState.Failed)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            StartTask(Int64.Parse(this.dataTask.SelectedRows[index].Cells[1].Value.ToString()), this.dataTask.SelectedRows[index].Cells[5].Value.ToString(), index);
        //        }
        //    }
        //}

        /// <summary>
        /// 启动发布任务
        /// </summary>
        //private void StartMultiPublish()
        //{
            
        //}

  


        #region 事件
        /// <summary>
        /// 加锁传递事件，这样做，事件则为单一处理
        /// </summary>
        private readonly Object m_eventLock = new Object();

        private event EventHandler<ExcuteFunctionEvent> e_ExcuteFunction;
        public event EventHandler<ExcuteFunctionEvent> ExcuteFunction
        {
            add { lock (m_eventLock) { e_ExcuteFunction += value; } }
            remove { lock (m_eventLock) { e_ExcuteFunction -= value; } }
        }

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

        private event EventHandler<DelInfoEvent> e_DelInfo;
        public event EventHandler<DelInfoEvent> DelInfo
        {
            add { lock (m_eventLock) { e_DelInfo += value; } }
            remove { lock (m_eventLock) { e_DelInfo -= value; } }
        }

        private event EventHandler<OpenDataEvent> e_OpenDataEvent;
        public event EventHandler<OpenDataEvent> OpenDataEvent
        {
            add { lock (m_eventLock) { e_OpenDataEvent += value; } }
            remove { lock (m_eventLock) { e_OpenDataEvent -= value; } }
        }
        #endregion

   

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if (m_TreeNode.Name == "nodRemoteTaskClass")
            {
                for (int i = 0; i < this.contextMenuStrip2.Items.Count; i++)
                {
                    this.contextMenuStrip2.Items[i].Enabled = false;
                }
                this.rmmenuDelTask.Enabled = true;
                return;
            }

            if (Program.IsConnectRemote == cGlobalParas.RegResult.Succeed && Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                this.rmenuUploadTask.Visible = true;
            }
            else
                this.rmenuUploadTask.Visible = false;

            //导入采集任务永远可用
            this.rmenuImportTask.Enabled = true;

            if (this.dataTask.SelectedRows.Count == 0)
            {
                this.rmmenuStartTask.Enabled = false;
                //this.rmmenuRestartTask.Enabled = false;
                this.rmmenuNewTask.Enabled = true;
                this.rmmenuEditTask.Enabled = false;
                this.rmmenuDelTask.Enabled = false;

                this.rmmenuClearTaskDB.Enabled = false;

                this.rmenuExportTask.Enabled = false;

                this.rmmenuRenameTask.Enabled = false;

                this.rmenuAddPlan.Enabled = true;

                this.rmenuCopyTask.Visible = false;
                this.rmenuPasteTask.Visible = false;
                this.toolStripSeparator18.Visible = false;

                this.rmenuUploadTask.Enabled = false;

                if (this.m_TreeNode == null)
                    this.rmenuPasteTask.Visible = false;
                else
                {
                    if (m_TreeNode.Name.Substring(0, 1) == "C" || m_TreeNode.Name == "nodTaskClass")
                    {
                        if (IsClipboardSoukeyData())
                        {
                            this.rmenuPasteTask.Visible = true;
                            this.rmenuPasteTask.Enabled = true;
                        }
                        else
                        {
                            this.rmenuPasteTask.Visible = false;
                        }
                    }
                    else
                    {
                        this.rmenuPasteTask.Visible = false;

                    }
                }
                return;
            }

            this.rmmenuClearTaskDB.Enabled = true;

            cGlobalParas.TaskState tState = (cGlobalParas.TaskState)this.dataTask.SelectedCells[2].Value;

            switch (this.dataTask.SelectedCells[3].Value.ToString())
            {
                
                case "nodPlanRunning":

                    this.rmmenuStartTask.Enabled = false;
                    this.rmmenuNewTask.Enabled = true;
                    this.rmmenuEditTask.Enabled = true;
                    this.rmmenuDelTask.Enabled = true;

                    this.rmenuAddPlan.Enabled = true;

                    this.rmmenuRenameTask.Enabled = true;

                    this.rmenuCopyTask.Visible = false;
                    this.rmenuPasteTask.Visible = false;
                    this.toolStripSeparator18.Visible = false;

                    this.rmenuExportTask.Enabled = false;

                    this.rmenuUploadTask.Enabled = false;

                    break;
                case "nodPublishTemplate":
                    this.toolStripSeparator18.Visible = false;
                    this.rmmenuStartTask.Visible = false;
                    this.rmmenuNewTask.Enabled = true;
                    this.rmmenuEditTask.Enabled = true;
                    this.rmmenuDelTask.Enabled = true;
                    this.rmenuAddPlan.Enabled = true;
                    this.rmmenuRenameTask.Enabled = false;
                    this.rmenuCopyTask.Visible = false;
                    this.rmenuPasteTask.Visible = false;
                    this.toolStripSeparator18.Visible = false;
                    this.rmenuExportTask.Enabled = false;
                    this.rmenuUploadTask.Enabled = false;
                    this.rmenuUploadWebTask.Enabled = false;
                    this.rMenuRefer.Visible = false;
                    break;

                case "nodPublishByWeb":
                    this.toolStripSeparator18.Visible = false;
                    this.rmmenuStartTask.Visible = false;
                    this.rmmenuNewTask.Enabled = true;
                    this.rmmenuEditTask.Enabled = true;
                    this.rmmenuDelTask.Enabled = true;
                    this.rmenuAddPlan.Enabled = true;
                    this.rmmenuRenameTask.Enabled = false;
                    this.rmenuCopyTask.Visible = false;
                    this.rmenuPasteTask.Visible = false;
                    this.toolStripSeparator18.Visible = false;
                    this.rmenuExportTask.Enabled = false;
                    this.rmenuUploadTask.Enabled = false;
                    this.rmenuUploadWebTask.Enabled = false;
                    this.rMenuRefer.Visible = false;
                    break;
                case "nodPublishByDB":
                    this.toolStripSeparator18.Visible = false;
                    this.rmmenuStartTask.Visible = false;
                    this.rmmenuNewTask.Enabled = true;
                    this.rmmenuEditTask.Enabled = true;
                    this.rmmenuDelTask.Enabled = true;
                    this.rmenuAddPlan.Enabled = true;
                    this.rmmenuRenameTask.Enabled = false;
                    this.rmenuCopyTask.Visible = false;
                    this.rmenuPasteTask.Visible = false;
                    this.toolStripSeparator18.Visible = false;
                    this.rmenuExportTask.Enabled = false;
                    this.rmenuUploadTask.Enabled = false;
                    this.rmenuUploadWebTask.Enabled = false;
                    this.rMenuRefer.Visible = false;
                    break;
                default:
                    if (tState == cGlobalParas.TaskState.Failed)
                    {
                        this.rmmenuStartTask.Enabled = false;
                        this.rmmenuEditTask.Enabled = false;
                    }
                    else
                    {
                        this.rmmenuStartTask.Enabled = true;
                        this.rmmenuEditTask.Enabled = true;
                    }

                    this.rmmenuNewTask.Enabled = true;
                    this.rmmenuDelTask.Enabled = true;

                    this.rmenuAddPlan.Enabled = true;

                    this.rmmenuRenameTask.Enabled = true;

                    this.rmenuCopyTask.Visible = true;
                    this.rmenuPasteTask.Visible = true;
                    this.toolStripSeparator18.Visible = true;

                    this.rmenuExportTask.Enabled = true;

                    this.rmenuUploadTask.Enabled = true;

                    if (!IsClipboardSoukeyData())
                    {
                        this.rmenuPasteTask.Enabled = false;
                    }
                    else
                    {
                        this.rmenuPasteTask.Enabled = true;
                    }

                    break;
            }
        }

        private void rmmenuStartTask_Click(object sender, EventArgs e)
        {
            StartTask();
        }

        public void StartTask()
        {
            if (this.dataTask.SelectedCells.Count==0)
            {
                MessageBox.Show("请选择需要启动采集的任务！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            oTaskClass tc = new oTaskClass(Program.getPrjPath());

            for (int i=0;i<this.dataTask.SelectedRows.Count;i++)
            {
                string tClassName = string.Empty;
                if (this.dataTask.SelectedRows[i].Cells[3].Value.ToString() == "nodTaskClass")
                    tClassName = "";
                else
                {
                    string tClassID = this.dataTask.SelectedRows[i].Cells[3].Value.ToString().Substring(1, this.dataTask.SelectedRows[i].Cells[3].Value.ToString().Length - 1);
                    tClassName = tc.GetTaskClassNameByID(tClassID);
                }

                object[] paras = new object[] { this.dataTask.SelectedRows[i].Cells[1].Value.ToString(),
                    this.dataTask.SelectedRows[i].Cells[5].Value.ToString(),tClassName,this.dataTask.SelectedRows[i].Cells[4].Value.ToString()};

                if (e_ExcuteFunction != null)
                    e_ExcuteFunction(this, new ExcuteFunctionEvent("Start", paras));
            }

            tc.Dispose();
            tc = null;
            
        }

        private void rmmenuEditTask_Click(object sender, EventArgs e)
        {
            switch (this.dataTask.SelectedCells[3].Value.ToString())
            {
                
                case "nodPlanRunning":
                    EditPlan();
                    break;
                case "nodPlanCompleted":

                    break;
                case "nodTaskClass":
                    EditTask();
                    break;
                case "nodPublishTemplate":
                    EditPublishTemplate();
                    break;
                case "nodPublishByWeb":
                    EditPublishTemplate();
                    break;
                case "nodPublishByDB":
                    EditPublishTemplate();
                    break;
                default:
                    EditTask();
                    break;
            }
        }

        private void rmmenuClearTaskDB_Click(object sender, EventArgs e)
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            string tName = this.dataTask.SelectedRows[0].Cells[5].Value.ToString();

            string tClassName = string.Empty;
            if (this.dataTask.SelectedRows[0].Cells[3].Value.ToString() == "nodTaskClass")
                tClassName = "";
            else
            {
                oTaskClass tc = new oTaskClass(Program.getPrjPath());
                string tClassID = this.dataTask.SelectedRows[0].Cells[3].Value.ToString().Substring(1, this.dataTask.SelectedRows[0].Cells[3].Value.ToString().Length - 1);
                tClassName = tc.GetTaskClassNameByID(tClassID);
                tc = null;
            }

            string FileName = Program.getPrjPath() + "urls\\" + tClassName.Replace("/","-") + "-" + tName + ".db";

            try
            {
                System.IO.File.Delete(FileName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("删除排重库发生错误，错误信息：" + ex.Message, "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("删除排重库成功！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //private void rmmenuEditTask_Click(object sender, EventArgs e)
        //{

        //}

    }

}

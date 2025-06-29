namespace SoukeyDataPublish.CustomControl
{
    partial class uconMyDataOp
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (m_PControl != null)
            {
                m_PControl.Abort();
                m_PControl = null;
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uconMyDataOp));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("选择数据发布规则");
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rmenuAddFiled = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuEditColHeader = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuDelField = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuSelectCol = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuMore = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuLess = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuTxtEqual = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuTxtNoEqual = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuTxtStart = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuTxtStartNo = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuTxtInclude = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuTxtEnd = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuLenMore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuCustomFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuAutoID = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuDelRepeatRow = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuAddPre = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuAddSuffix = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuReplace = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuCutLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuCutRight = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuSetEmpty = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuDelHtml = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuValue = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuCustomData = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuPlugin = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rmenuImportPublishInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuDelPublishInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuExport = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuImport = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.rmenuGetFormat = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolmenuPost1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolmenuPost2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolmenuPostData = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.treePublish = new System.Windows.Forms.TreeView();
            this.butNew = new System.Windows.Forms.Button();
            this.butSaveTemplate = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSucceedFlag = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.IsPublishByTemplate = new System.Windows.Forms.CheckBox();
            this.udPIntervalTime = new System.Windows.Forms.NumericUpDown();
            this.raPublishWeb = new System.Windows.Forms.RadioButton();
            this.raPublishData = new System.Windows.Forms.RadioButton();
            this.udThread = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.IsDelRepRow = new System.Windows.Forms.CheckBox();
            this.groupWeb = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtHeader = new System.Windows.Forms.TextBox();
            this.IsHeader = new System.Windows.Forms.CheckBox();
            this.button9 = new System.Windows.Forms.Button();
            this.comExportUrlCode = new System.Windows.Forms.ComboBox();
            this.label27 = new System.Windows.Forms.Label();
            this.button11 = new System.Windows.Forms.Button();
            this.txtExportCookie = new System.Windows.Forms.TextBox();
            this.label38 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.txtExportUrl = new System.Windows.Forms.TextBox();
            this.groupData = new System.Windows.Forms.GroupBox();
            this.raExportOracle = new System.Windows.Forms.RadioButton();
            this.IsSqlTrue = new System.Windows.Forms.CheckBox();
            this.comTableName = new System.Windows.Forms.ComboBox();
            this.txtInsertSql = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.button12 = new System.Windows.Forms.Button();
            this.txtDataSource = new System.Windows.Forms.TextBox();
            this.raExportMySql = new System.Windows.Forms.RadioButton();
            this.raExportMSSQL = new System.Windows.Forms.RadioButton();
            this.raExportAccess = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupTemplate = new System.Windows.Forms.GroupBox();
            this.dataParas = new System.Windows.Forms.DataGridView();
            this.Para = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PDataColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.txtTDbConn = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.comTemplate = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rmenuLookPublishData = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.IsErrLog = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataNavi = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.toolIsPublish = new System.Windows.Forms.ToolStripButton();
            this.toolStartPublish = new System.Windows.Forms.ToolStripButton();
            this.toolStopPublish = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolSaveData = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.toolAddField = new System.Windows.Forms.ToolStripButton();
            this.toolDelField = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolFilter = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolNoFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolMore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolLess = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolEqual = new System.Windows.Forms.ToolStripMenuItem();
            this.toolNoEqual = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStartNo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolInclude = new System.Windows.Forms.ToolStripMenuItem();
            this.toolEnd = new System.Windows.Forms.ToolStripMenuItem();
            this.toolLenMore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.toolCustomFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.toolEdit = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolAutoCutFlag = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.toolAutoID = new System.Windows.Forms.ToolStripMenuItem();
            this.toolDelRepeatRow = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.toolAddPre = new System.Windows.Forms.ToolStripMenuItem();
            this.toolAddSuffix = new System.Windows.Forms.ToolStripMenuItem();
            this.toolReplace = new System.Windows.Forms.ToolStripMenuItem();
            this.toolCutLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.toolCutRight = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.toolSetEmpty = new System.Windows.Forms.ToolStripMenuItem();
            this.toolDelHtml = new System.Windows.Forms.ToolStripMenuItem();
            this.toolValue = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.toolCustomData = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.dataData = new System.Windows.Forms.DataGridView();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.staFilterTitle = new System.Windows.Forms.ToolStripStatusLabel();
            this.stateFilter = new System.Windows.Forms.ToolStripStatusLabel();
            this.staShowAll = new System.Windows.Forms.ToolStripStatusLabel();
            this.stateRecords = new System.Windows.Forms.ToolStripStatusLabel();
            this.staExportState = new System.Windows.Forms.ToolStripStatusLabel();
            this.staProgressbar = new System.Windows.Forms.ToolStripProgressBar();
            this.staProgressTitle = new System.Windows.Forms.ToolStripStatusLabel();
            this.stateProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.staError = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewComboBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
            this.rmenuGetFormat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udPIntervalTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udThread)).BeginInit();
            this.groupWeb.SuspendLayout();
            this.groupData.SuspendLayout();
            this.groupTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataParas)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataNavi)).BeginInit();
            this.dataNavi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rmenuAddFiled,
            this.rmenuEditColHeader,
            this.rmenuDelField,
            this.toolStripSeparator3,
            this.rmenuSelectCol,
            this.rmenuFilter,
            this.rmenuEdit,
            this.toolStripSeparator4,
            this.rmenuPlugin,
            this.toolStripSeparator19});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(173, 176);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // rmenuAddFiled
            // 
            this.rmenuAddFiled.Image = ((System.Drawing.Image)(resources.GetObject("rmenuAddFiled.Image")));
            this.rmenuAddFiled.Name = "rmenuAddFiled";
            this.rmenuAddFiled.Size = new System.Drawing.Size(172, 22);
            this.rmenuAddFiled.Text = "添加列";
            this.rmenuAddFiled.Click += new System.EventHandler(this.rmenuAddFiled_Click);
            // 
            // rmenuEditColHeader
            // 
            this.rmenuEditColHeader.Name = "rmenuEditColHeader";
            this.rmenuEditColHeader.Size = new System.Drawing.Size(172, 22);
            this.rmenuEditColHeader.Text = "修改列头";
            this.rmenuEditColHeader.Click += new System.EventHandler(this.rmenuEditColHeader_Click);
            // 
            // rmenuDelField
            // 
            this.rmenuDelField.Name = "rmenuDelField";
            this.rmenuDelField.Size = new System.Drawing.Size(172, 22);
            this.rmenuDelField.Text = "删除列";
            this.rmenuDelField.Click += new System.EventHandler(this.rmenuDelField_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(169, 6);
            // 
            // rmenuSelectCol
            // 
            this.rmenuSelectCol.Name = "rmenuSelectCol";
            this.rmenuSelectCol.Size = new System.Drawing.Size(172, 22);
            this.rmenuSelectCol.Text = "选择整列";
            this.rmenuSelectCol.Click += new System.EventHandler(this.rmenuSelectCol_Click);
            // 
            // rmenuFilter
            // 
            this.rmenuFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rmenuMore,
            this.rmenuLess,
            this.toolStripSeparator5,
            this.rmenuTxtEqual,
            this.rmenuTxtNoEqual,
            this.toolStripSeparator7,
            this.rmenuTxtStart,
            this.rmenuTxtStartNo,
            this.rmenuTxtInclude,
            this.rmenuTxtEnd,
            this.rmenuLenMore,
            this.toolStripSeparator9,
            this.rmenuCustomFilter});
            this.rmenuFilter.Image = ((System.Drawing.Image)(resources.GetObject("rmenuFilter.Image")));
            this.rmenuFilter.Name = "rmenuFilter";
            this.rmenuFilter.Size = new System.Drawing.Size(172, 22);
            this.rmenuFilter.Text = "筛选";
            // 
            // rmenuMore
            // 
            this.rmenuMore.Name = "rmenuMore";
            this.rmenuMore.Size = new System.Drawing.Size(136, 22);
            this.rmenuMore.Text = "大于";
            this.rmenuMore.Click += new System.EventHandler(this.rmenuMore_Click);
            // 
            // rmenuLess
            // 
            this.rmenuLess.Name = "rmenuLess";
            this.rmenuLess.Size = new System.Drawing.Size(136, 22);
            this.rmenuLess.Text = "小于";
            this.rmenuLess.Click += new System.EventHandler(this.rmenuLess_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(133, 6);
            // 
            // rmenuTxtEqual
            // 
            this.rmenuTxtEqual.Name = "rmenuTxtEqual";
            this.rmenuTxtEqual.Size = new System.Drawing.Size(136, 22);
            this.rmenuTxtEqual.Text = "等于";
            this.rmenuTxtEqual.Click += new System.EventHandler(this.rmenuTxtEqual_Click);
            // 
            // rmenuTxtNoEqual
            // 
            this.rmenuTxtNoEqual.Name = "rmenuTxtNoEqual";
            this.rmenuTxtNoEqual.Size = new System.Drawing.Size(136, 22);
            this.rmenuTxtNoEqual.Text = "不等于";
            this.rmenuTxtNoEqual.Click += new System.EventHandler(this.rmenuTxtNoEqual_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(133, 6);
            // 
            // rmenuTxtStart
            // 
            this.rmenuTxtStart.Name = "rmenuTxtStart";
            this.rmenuTxtStart.Size = new System.Drawing.Size(136, 22);
            this.rmenuTxtStart.Text = "开头是";
            this.rmenuTxtStart.Click += new System.EventHandler(this.rmenuTxtStart_Click);
            // 
            // rmenuTxtStartNo
            // 
            this.rmenuTxtStartNo.Name = "rmenuTxtStartNo";
            this.rmenuTxtStartNo.Size = new System.Drawing.Size(136, 22);
            this.rmenuTxtStartNo.Text = "开头不是";
            this.rmenuTxtStartNo.Click += new System.EventHandler(this.rmenuTxtStartNo_Click);
            // 
            // rmenuTxtInclude
            // 
            this.rmenuTxtInclude.Name = "rmenuTxtInclude";
            this.rmenuTxtInclude.Size = new System.Drawing.Size(136, 22);
            this.rmenuTxtInclude.Text = "包含";
            this.rmenuTxtInclude.Click += new System.EventHandler(this.rmenuTxtInclude_Click);
            // 
            // rmenuTxtEnd
            // 
            this.rmenuTxtEnd.Name = "rmenuTxtEnd";
            this.rmenuTxtEnd.Size = new System.Drawing.Size(136, 22);
            this.rmenuTxtEnd.Text = "结尾是";
            this.rmenuTxtEnd.Click += new System.EventHandler(this.rmenuTxtEnd_Click);
            // 
            // rmenuLenMore
            // 
            this.rmenuLenMore.Name = "rmenuLenMore";
            this.rmenuLenMore.Size = new System.Drawing.Size(136, 22);
            this.rmenuLenMore.Text = "长度大于";
            this.rmenuLenMore.Click += new System.EventHandler(this.rmenuLenMore_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(133, 6);
            // 
            // rmenuCustomFilter
            // 
            this.rmenuCustomFilter.Name = "rmenuCustomFilter";
            this.rmenuCustomFilter.Size = new System.Drawing.Size(136, 22);
            this.rmenuCustomFilter.Text = "自定义函数";
            this.rmenuCustomFilter.Click += new System.EventHandler(this.rmenuCustomFilter_Click);
            // 
            // rmenuEdit
            // 
            this.rmenuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rmenuAutoID,
            this.rmenuDelRepeatRow,
            this.toolStripSeparator14,
            this.rmenuAddPre,
            this.rmenuAddSuffix,
            this.rmenuReplace,
            this.rmenuCutLeft,
            this.rmenuCutRight,
            this.toolStripSeparator15,
            this.rmenuSetEmpty,
            this.rmenuDelHtml,
            this.rmenuValue,
            this.toolStripSeparator17,
            this.rmenuCustomData});
            this.rmenuEdit.Name = "rmenuEdit";
            this.rmenuEdit.Size = new System.Drawing.Size(172, 22);
            this.rmenuEdit.Text = "修改";
            // 
            // rmenuAutoID
            // 
            this.rmenuAutoID.Name = "rmenuAutoID";
            this.rmenuAutoID.Size = new System.Drawing.Size(148, 22);
            this.rmenuAutoID.Text = "自动编号";
            this.rmenuAutoID.Click += new System.EventHandler(this.rmenuAutoID_Click);
            // 
            // rmenuDelRepeatRow
            // 
            this.rmenuDelRepeatRow.Name = "rmenuDelRepeatRow";
            this.rmenuDelRepeatRow.Size = new System.Drawing.Size(148, 22);
            this.rmenuDelRepeatRow.Text = "删除重复行";
            this.rmenuDelRepeatRow.Click += new System.EventHandler(this.rmenuDelRepeatRow_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(145, 6);
            // 
            // rmenuAddPre
            // 
            this.rmenuAddPre.Name = "rmenuAddPre";
            this.rmenuAddPre.Size = new System.Drawing.Size(148, 22);
            this.rmenuAddPre.Text = "增加前缀";
            this.rmenuAddPre.Click += new System.EventHandler(this.rmenuAddPre_Click);
            // 
            // rmenuAddSuffix
            // 
            this.rmenuAddSuffix.Name = "rmenuAddSuffix";
            this.rmenuAddSuffix.Size = new System.Drawing.Size(148, 22);
            this.rmenuAddSuffix.Text = "增加后缀";
            this.rmenuAddSuffix.Click += new System.EventHandler(this.rmenuAddSuffix_Click);
            // 
            // rmenuReplace
            // 
            this.rmenuReplace.Name = "rmenuReplace";
            this.rmenuReplace.Size = new System.Drawing.Size(148, 22);
            this.rmenuReplace.Text = "替换";
            this.rmenuReplace.Click += new System.EventHandler(this.rmenuReplace_Click);
            // 
            // rmenuCutLeft
            // 
            this.rmenuCutLeft.Name = "rmenuCutLeft";
            this.rmenuCutLeft.Size = new System.Drawing.Size(148, 22);
            this.rmenuCutLeft.Text = "从左截取字符";
            this.rmenuCutLeft.Click += new System.EventHandler(this.rmenuCutLeft_Click);
            // 
            // rmenuCutRight
            // 
            this.rmenuCutRight.Name = "rmenuCutRight";
            this.rmenuCutRight.Size = new System.Drawing.Size(148, 22);
            this.rmenuCutRight.Text = "从右截取字符";
            this.rmenuCutRight.Click += new System.EventHandler(this.rmenuCutRight_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(145, 6);
            // 
            // rmenuSetEmpty
            // 
            this.rmenuSetEmpty.Name = "rmenuSetEmpty";
            this.rmenuSetEmpty.Size = new System.Drawing.Size(148, 22);
            this.rmenuSetEmpty.Text = "清空";
            this.rmenuSetEmpty.Click += new System.EventHandler(this.rmenuSetEmpty_Click);
            // 
            // rmenuDelHtml
            // 
            this.rmenuDelHtml.Name = "rmenuDelHtml";
            this.rmenuDelHtml.Size = new System.Drawing.Size(148, 22);
            this.rmenuDelHtml.Text = "去除网页符号";
            this.rmenuDelHtml.Click += new System.EventHandler(this.rmenuDelHtml_Click);
            // 
            // rmenuValue
            // 
            this.rmenuValue.Name = "rmenuValue";
            this.rmenuValue.Size = new System.Drawing.Size(148, 22);
            this.rmenuValue.Text = "固定值";
            this.rmenuValue.Click += new System.EventHandler(this.rmenuValue_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(145, 6);
            // 
            // rmenuCustomData
            // 
            this.rmenuCustomData.Enabled = false;
            this.rmenuCustomData.Name = "rmenuCustomData";
            this.rmenuCustomData.Size = new System.Drawing.Size(148, 22);
            this.rmenuCustomData.Text = "自定义函数";
            this.rmenuCustomData.Click += new System.EventHandler(this.rmenuCustomData_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(169, 6);
            // 
            // rmenuPlugin
            // 
            this.rmenuPlugin.Image = ((System.Drawing.Image)(resources.GetObject("rmenuPlugin.Image")));
            this.rmenuPlugin.Name = "rmenuPlugin";
            this.rmenuPlugin.Size = new System.Drawing.Size(172, 22);
            this.rmenuPlugin.Text = "使用插件编辑数据";
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Size = new System.Drawing.Size(169, 6);
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rmenuImportPublishInfo,
            this.rmenuDelPublishInfo,
            this.toolStripSeparator18,
            this.rmenuExport,
            this.rmenuImport});
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(173, 98);
            // 
            // rmenuImportPublishInfo
            // 
            this.rmenuImportPublishInfo.Name = "rmenuImportPublishInfo";
            this.rmenuImportPublishInfo.Size = new System.Drawing.Size(172, 22);
            this.rmenuImportPublishInfo.Text = "加载发布规则信息";
            this.rmenuImportPublishInfo.Click += new System.EventHandler(this.rmenuImportPublishInfo_Click);
            // 
            // rmenuDelPublishInfo
            // 
            this.rmenuDelPublishInfo.Image = ((System.Drawing.Image)(resources.GetObject("rmenuDelPublishInfo.Image")));
            this.rmenuDelPublishInfo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rmenuDelPublishInfo.Name = "rmenuDelPublishInfo";
            this.rmenuDelPublishInfo.Size = new System.Drawing.Size(172, 22);
            this.rmenuDelPublishInfo.Text = "删除发布规则信息";
            this.rmenuDelPublishInfo.Click += new System.EventHandler(this.rmenuDelPublishInfo_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(169, 6);
            // 
            // rmenuExport
            // 
            this.rmenuExport.Image = ((System.Drawing.Image)(resources.GetObject("rmenuExport.Image")));
            this.rmenuExport.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rmenuExport.Name = "rmenuExport";
            this.rmenuExport.Size = new System.Drawing.Size(172, 22);
            this.rmenuExport.Text = "导出发布规则";
            this.rmenuExport.Click += new System.EventHandler(this.rmenuExport_Click);
            // 
            // rmenuImport
            // 
            this.rmenuImport.Name = "rmenuImport";
            this.rmenuImport.Size = new System.Drawing.Size(172, 22);
            this.rmenuImport.Text = "导入发布规则";
            this.rmenuImport.Click += new System.EventHandler(this.rmenuImport_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "A42.gif");
            this.imageList1.Images.SetKeyName(1, "A43.gif");
            this.imageList1.Images.SetKeyName(2, "logo.gif");
            this.imageList1.Images.SetKeyName(3, "A44.gif");
            this.imageList1.Images.SetKeyName(4, "data");
            this.imageList1.Images.SetKeyName(5, "task");
            this.imageList1.Images.SetKeyName(6, "A02.gif");
            this.imageList1.Images.SetKeyName(7, "A17.gif");
            this.imageList1.Images.SetKeyName(8, "A46.gif");
            this.imageList1.Images.SetKeyName(9, "A46.ico");
            // 
            // rmenuGetFormat
            // 
            this.rmenuGetFormat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolmenuPost1,
            this.toolmenuPost2,
            this.toolmenuPostData,
            this.toolStripSeparator20});
            this.rmenuGetFormat.Name = "rmenuGetFormat";
            this.rmenuGetFormat.Size = new System.Drawing.Size(186, 76);
            this.rmenuGetFormat.Opening += new System.ComponentModel.CancelEventHandler(this.rmenuGetFormat_Opening);
            this.rmenuGetFormat.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.rmenuGetFormat_ItemClicked);
            // 
            // toolmenuPost1
            // 
            this.toolmenuPost1.Name = "toolmenuPost1";
            this.toolmenuPost1.Size = new System.Drawing.Size(185, 22);
            this.toolmenuPost1.Text = "POST前缀<POST>";
            // 
            // toolmenuPost2
            // 
            this.toolmenuPost2.Name = "toolmenuPost2";
            this.toolmenuPost2.Size = new System.Drawing.Size(185, 22);
            this.toolmenuPost2.Text = "POST后缀</POST>";
            // 
            // toolmenuPostData
            // 
            this.toolmenuPostData.Name = "toolmenuPostData";
            this.toolmenuPostData.Size = new System.Drawing.Size(185, 22);
            this.toolmenuPostData.Text = "手工捕获POST数据";
            this.toolmenuPostData.Click += new System.EventHandler(this.toolmenuPostData_Click);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(182, 6);
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.Location = new System.Drawing.Point(0, 0);
            this.splitContainer5.Name = "splitContainer5";
            this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.splitContainer4);
            this.splitContainer5.Panel1MinSize = 0;
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.txtLog);
            this.splitContainer5.Panel2.Controls.Add(this.panel1);
            this.splitContainer5.Panel2MinSize = 0;
            this.splitContainer5.Size = new System.Drawing.Size(903, 395);
            this.splitContainer5.SplitterDistance = 340;
            this.splitContainer5.TabIndex = 43;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.treePublish);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.butNew);
            this.splitContainer4.Panel2.Controls.Add(this.butSaveTemplate);
            this.splitContainer4.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer4.Panel2.Controls.Add(this.groupWeb);
            this.splitContainer4.Panel2.Controls.Add(this.groupData);
            this.splitContainer4.Panel2.Controls.Add(this.groupTemplate);
            this.splitContainer4.Size = new System.Drawing.Size(903, 340);
            this.splitContainer4.SplitterDistance = 157;
            this.splitContainer4.TabIndex = 42;
            // 
            // treePublish
            // 
            this.treePublish.ContextMenuStrip = this.contextMenuStrip3;
            this.treePublish.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treePublish.HideSelection = false;
            this.treePublish.ImageIndex = 0;
            this.treePublish.ImageList = this.imageList1;
            this.treePublish.Location = new System.Drawing.Point(0, 0);
            this.treePublish.Name = "treePublish";
            treeNode1.Name = "nodPublish";
            treeNode1.Text = "选择数据发布规则";
            this.treePublish.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treePublish.SelectedImageIndex = 0;
            this.treePublish.ShowRootLines = false;
            this.treePublish.Size = new System.Drawing.Size(157, 340);
            this.treePublish.TabIndex = 0;
            this.treePublish.DoubleClick += new System.EventHandler(this.treePublish_DoubleClick);
            this.treePublish.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treePublish_KeyDown);
            // 
            // butNew
            // 
            this.butNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butNew.Image = ((System.Drawing.Image)(resources.GetObject("butNew.Image")));
            this.butNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.butNew.Location = new System.Drawing.Point(8, 3);
            this.butNew.Name = "butNew";
            this.butNew.Size = new System.Drawing.Size(58, 24);
            this.butNew.TabIndex = 44;
            this.butNew.Text = "新建";
            this.butNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.butNew.UseVisualStyleBackColor = true;
            this.butNew.Click += new System.EventHandler(this.butNew_Click);
            // 
            // butSaveTemplate
            // 
            this.butSaveTemplate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butSaveTemplate.Image = ((System.Drawing.Image)(resources.GetObject("butSaveTemplate.Image")));
            this.butSaveTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.butSaveTemplate.Location = new System.Drawing.Point(72, 3);
            this.butSaveTemplate.Name = "butSaveTemplate";
            this.butSaveTemplate.Size = new System.Drawing.Size(109, 24);
            this.butSaveTemplate.TabIndex = 43;
            this.butSaveTemplate.Text = "保存发布规则";
            this.butSaveTemplate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.butSaveTemplate.UseVisualStyleBackColor = true;
            this.butSaveTemplate.Click += new System.EventHandler(this.butSaveTemplate_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtSucceedFlag);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.IsPublishByTemplate);
            this.groupBox1.Controls.Add(this.udPIntervalTime);
            this.groupBox1.Controls.Add(this.raPublishWeb);
            this.groupBox1.Controls.Add(this.raPublishData);
            this.groupBox1.Controls.Add(this.udThread);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.IsDelRepRow);
            this.groupBox1.Location = new System.Drawing.Point(8, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(722, 66);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "基本配置";
            // 
            // txtSucceedFlag
            // 
            this.txtSucceedFlag.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSucceedFlag.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSucceedFlag.Location = new System.Drawing.Point(573, 38);
            this.txtSucceedFlag.Name = "txtSucceedFlag";
            this.txtSucceedFlag.Size = new System.Drawing.Size(126, 21);
            this.txtSucceedFlag.TabIndex = 41;
            this.txtSucceedFlag.TextChanged += new System.EventHandler(this.txtSucceedFlag_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(292, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 45;
            this.label3.Text = "发布间隔延时：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(485, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 40;
            this.label2.Text = "发布成功标志：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(450, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 47;
            this.label4.Text = "毫秒";
            // 
            // IsPublishByTemplate
            // 
            this.IsPublishByTemplate.AutoSize = true;
            this.IsPublishByTemplate.Location = new System.Drawing.Point(12, 18);
            this.IsPublishByTemplate.Name = "IsPublishByTemplate";
            this.IsPublishByTemplate.Size = new System.Drawing.Size(144, 16);
            this.IsPublishByTemplate.TabIndex = 38;
            this.IsPublishByTemplate.Text = "调用发布模版发布数据";
            this.IsPublishByTemplate.UseVisualStyleBackColor = true;
            this.IsPublishByTemplate.CheckedChanged += new System.EventHandler(this.IsPublishByTemplate_CheckedChanged);
            // 
            // udPIntervalTime
            // 
            this.udPIntervalTime.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.udPIntervalTime.Location = new System.Drawing.Point(386, 38);
            this.udPIntervalTime.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.udPIntervalTime.Name = "udPIntervalTime";
            this.udPIntervalTime.Size = new System.Drawing.Size(58, 21);
            this.udPIntervalTime.TabIndex = 46;
            // 
            // raPublishWeb
            // 
            this.raPublishWeb.AutoSize = true;
            this.raPublishWeb.Location = new System.Drawing.Point(267, 17);
            this.raPublishWeb.Name = "raPublishWeb";
            this.raPublishWeb.Size = new System.Drawing.Size(83, 16);
            this.raPublishWeb.TabIndex = 37;
            this.raPublishWeb.Text = "发布到网站";
            this.raPublishWeb.UseVisualStyleBackColor = true;
            this.raPublishWeb.CheckedChanged += new System.EventHandler(this.raPublishWeb_CheckedChanged);
            // 
            // raPublishData
            // 
            this.raPublishData.AutoSize = true;
            this.raPublishData.Checked = true;
            this.raPublishData.Location = new System.Drawing.Point(166, 17);
            this.raPublishData.Name = "raPublishData";
            this.raPublishData.Size = new System.Drawing.Size(95, 16);
            this.raPublishData.TabIndex = 36;
            this.raPublishData.TabStop = true;
            this.raPublishData.Text = "发布到数据库";
            this.raPublishData.UseVisualStyleBackColor = true;
            this.raPublishData.CheckedChanged += new System.EventHandler(this.raPublishData_CheckedChanged);
            // 
            // udThread
            // 
            this.udThread.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.udThread.Location = new System.Drawing.Point(77, 39);
            this.udThread.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udThread.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udThread.Name = "udThread";
            this.udThread.Size = new System.Drawing.Size(46, 21);
            this.udThread.TabIndex = 34;
            this.udThread.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.udThread.ValueChanged += new System.EventHandler(this.udThread_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(10, 41);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 35;
            this.label11.Text = "线 程 数：";
            // 
            // IsDelRepRow
            // 
            this.IsDelRepRow.AutoSize = true;
            this.IsDelRepRow.Checked = true;
            this.IsDelRepRow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IsDelRepRow.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.IsDelRepRow.Location = new System.Drawing.Point(142, 40);
            this.IsDelRepRow.Name = "IsDelRepRow";
            this.IsDelRepRow.Size = new System.Drawing.Size(144, 16);
            this.IsDelRepRow.TabIndex = 32;
            this.IsDelRepRow.Text = "数据发布时去除重复行";
            this.IsDelRepRow.UseVisualStyleBackColor = true;
            this.IsDelRepRow.CheckedChanged += new System.EventHandler(this.IsDelRepRow_CheckedChanged);
            // 
            // groupWeb
            // 
            this.groupWeb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupWeb.Controls.Add(this.button1);
            this.groupWeb.Controls.Add(this.txtHeader);
            this.groupWeb.Controls.Add(this.IsHeader);
            this.groupWeb.Controls.Add(this.button9);
            this.groupWeb.Controls.Add(this.comExportUrlCode);
            this.groupWeb.Controls.Add(this.label27);
            this.groupWeb.Controls.Add(this.button11);
            this.groupWeb.Controls.Add(this.txtExportCookie);
            this.groupWeb.Controls.Add(this.label38);
            this.groupWeb.Controls.Add(this.label29);
            this.groupWeb.Controls.Add(this.txtExportUrl);
            this.groupWeb.Location = new System.Drawing.Point(8, 101);
            this.groupWeb.Name = "groupWeb";
            this.groupWeb.Size = new System.Drawing.Size(722, 234);
            this.groupWeb.TabIndex = 41;
            this.groupWeb.TabStop = false;
            this.groupWeb.Text = "发布网站配置";
            this.groupWeb.Visible = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Enabled = false;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(629, 139);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 21);
            this.button1.TabIndex = 44;
            this.button1.Text = "设 置";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtHeader
            // 
            this.txtHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeader.BackColor = System.Drawing.Color.White;
            this.txtHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHeader.Location = new System.Drawing.Point(105, 139);
            this.txtHeader.Multiline = true;
            this.txtHeader.Name = "txtHeader";
            this.txtHeader.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtHeader.Size = new System.Drawing.Size(525, 91);
            this.txtHeader.TabIndex = 43;
            this.txtHeader.TextChanged += new System.EventHandler(this.txtHeader_TextChanged);
            // 
            // IsHeader
            // 
            this.IsHeader.Location = new System.Drawing.Point(35, 143);
            this.IsHeader.Name = "IsHeader";
            this.IsHeader.Size = new System.Drawing.Size(64, 47);
            this.IsHeader.TabIndex = 42;
            this.IsHeader.Text = "自定义HTTP Headers";
            this.IsHeader.UseVisualStyleBackColor = true;
            this.IsHeader.CheckedChanged += new System.EventHandler(this.IsHeader_CheckedChanged);
            // 
            // button9
            // 
            this.button9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button9.Location = new System.Drawing.Point(629, 40);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(80, 21);
            this.button9.TabIndex = 14;
            this.button9.Text = "插入参数";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // comExportUrlCode
            // 
            this.comExportUrlCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comExportUrlCode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comExportUrlCode.FormattingEnabled = true;
            this.comExportUrlCode.Location = new System.Drawing.Point(105, 12);
            this.comExportUrlCode.Name = "comExportUrlCode";
            this.comExportUrlCode.Size = new System.Drawing.Size(143, 20);
            this.comExportUrlCode.TabIndex = 12;
            this.comExportUrlCode.SelectedIndexChanged += new System.EventHandler(this.comExportUrlCode_SelectedIndexChanged);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label27.Location = new System.Drawing.Point(32, 16);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(65, 12);
            this.label27.TabIndex = 39;
            this.label27.Text = "网址编码：";
            // 
            // button11
            // 
            this.button11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button11.Location = new System.Drawing.Point(629, 115);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(80, 21);
            this.button11.TabIndex = 16;
            this.button11.Text = "获取Cookie";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // txtExportCookie
            // 
            this.txtExportCookie.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExportCookie.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtExportCookie.Location = new System.Drawing.Point(105, 115);
            this.txtExportCookie.Name = "txtExportCookie";
            this.txtExportCookie.Size = new System.Drawing.Size(525, 21);
            this.txtExportCookie.TabIndex = 15;
            this.txtExportCookie.TextChanged += new System.EventHandler(this.txtExportCookie_TextChanged);
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label38.Location = new System.Drawing.Point(48, 117);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(53, 12);
            this.label38.TabIndex = 36;
            this.label38.Text = "Cookie：";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label29.Location = new System.Drawing.Point(32, 42);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(65, 12);
            this.label29.TabIndex = 35;
            this.label29.Text = "目标地址：";
            // 
            // txtExportUrl
            // 
            this.txtExportUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExportUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtExportUrl.Location = new System.Drawing.Point(105, 40);
            this.txtExportUrl.Multiline = true;
            this.txtExportUrl.Name = "txtExportUrl";
            this.txtExportUrl.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExportUrl.Size = new System.Drawing.Size(525, 70);
            this.txtExportUrl.TabIndex = 13;
            this.txtExportUrl.TextChanged += new System.EventHandler(this.txtExportUrl_TextChanged);
            // 
            // groupData
            // 
            this.groupData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupData.Controls.Add(this.raExportOracle);
            this.groupData.Controls.Add(this.IsSqlTrue);
            this.groupData.Controls.Add(this.comTableName);
            this.groupData.Controls.Add(this.txtInsertSql);
            this.groupData.Controls.Add(this.label7);
            this.groupData.Controls.Add(this.button12);
            this.groupData.Controls.Add(this.txtDataSource);
            this.groupData.Controls.Add(this.raExportMySql);
            this.groupData.Controls.Add(this.raExportMSSQL);
            this.groupData.Controls.Add(this.raExportAccess);
            this.groupData.Controls.Add(this.label8);
            this.groupData.Controls.Add(this.label6);
            this.groupData.Location = new System.Drawing.Point(8, 101);
            this.groupData.Name = "groupData";
            this.groupData.Size = new System.Drawing.Size(722, 234);
            this.groupData.TabIndex = 42;
            this.groupData.TabStop = false;
            this.groupData.Text = "发布到数据库";
            // 
            // raExportOracle
            // 
            this.raExportOracle.AutoSize = true;
            this.raExportOracle.Location = new System.Drawing.Point(364, 18);
            this.raExportOracle.Name = "raExportOracle";
            this.raExportOracle.Size = new System.Drawing.Size(95, 16);
            this.raExportOracle.TabIndex = 39;
            this.raExportOracle.TabStop = true;
            this.raExportOracle.Text = "发布到Oracle";
            this.raExportOracle.UseVisualStyleBackColor = true;
            this.raExportOracle.CheckedChanged += new System.EventHandler(this.raExportOracle_CheckedChanged);
            // 
            // IsSqlTrue
            // 
            this.IsSqlTrue.AutoSize = true;
            this.IsSqlTrue.Location = new System.Drawing.Point(470, 18);
            this.IsSqlTrue.Name = "IsSqlTrue";
            this.IsSqlTrue.Size = new System.Drawing.Size(162, 16);
            this.IsSqlTrue.TabIndex = 38;
            this.IsSqlTrue.Text = "直接提交Sql语句发布数据";
            this.IsSqlTrue.UseVisualStyleBackColor = true;
            this.IsSqlTrue.CheckedChanged += new System.EventHandler(this.IsSqlTrue_CheckedChanged);
            // 
            // comTableName
            // 
            this.comTableName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comTableName.FormattingEnabled = true;
            this.comTableName.Location = new System.Drawing.Point(83, 61);
            this.comTableName.Name = "comTableName";
            this.comTableName.Size = new System.Drawing.Size(232, 20);
            this.comTableName.TabIndex = 9;
            this.comTableName.DropDown += new System.EventHandler(this.comTableName_DropDown);
            this.comTableName.SelectedIndexChanged += new System.EventHandler(this.comTableName_SelectedIndexChanged);
            this.comTableName.TextChanged += new System.EventHandler(this.comTableName_TextChanged);
            // 
            // txtInsertSql
            // 
            this.txtInsertSql.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInsertSql.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInsertSql.Location = new System.Drawing.Point(83, 86);
            this.txtInsertSql.Multiline = true;
            this.txtInsertSql.Name = "txtInsertSql";
            this.txtInsertSql.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInsertSql.Size = new System.Drawing.Size(626, 140);
            this.txtInsertSql.TabIndex = 10;
            this.txtInsertSql.TextChanged += new System.EventHandler(this.txtInsertSql_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(20, 86);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 37;
            this.label7.Text = "sql语句：";
            // 
            // button12
            // 
            this.button12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button12.Location = new System.Drawing.Point(629, 36);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(80, 21);
            this.button12.TabIndex = 8;
            this.button12.Text = "设 置";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // txtDataSource
            // 
            this.txtDataSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDataSource.Location = new System.Drawing.Point(83, 36);
            this.txtDataSource.Name = "txtDataSource";
            this.txtDataSource.Size = new System.Drawing.Size(547, 21);
            this.txtDataSource.TabIndex = 7;
            this.txtDataSource.TextChanged += new System.EventHandler(this.txtDataSource_TextChanged);
            // 
            // raExportMySql
            // 
            this.raExportMySql.AutoSize = true;
            this.raExportMySql.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.raExportMySql.Location = new System.Drawing.Point(265, 18);
            this.raExportMySql.Name = "raExportMySql";
            this.raExportMySql.Size = new System.Drawing.Size(89, 16);
            this.raExportMySql.TabIndex = 6;
            this.raExportMySql.Text = "发布到MySql";
            this.raExportMySql.UseVisualStyleBackColor = true;
            this.raExportMySql.CheckedChanged += new System.EventHandler(this.raExportMySql_CheckedChanged);
            // 
            // raExportMSSQL
            // 
            this.raExportMSSQL.AutoSize = true;
            this.raExportMSSQL.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.raExportMSSQL.Location = new System.Drawing.Point(122, 18);
            this.raExportMSSQL.Name = "raExportMSSQL";
            this.raExportMSSQL.Size = new System.Drawing.Size(131, 16);
            this.raExportMSSQL.TabIndex = 5;
            this.raExportMSSQL.Text = "发布到MS SqlServer";
            this.raExportMSSQL.UseVisualStyleBackColor = true;
            this.raExportMSSQL.CheckedChanged += new System.EventHandler(this.raExportMSSQL_CheckedChanged);
            // 
            // raExportAccess
            // 
            this.raExportAccess.AutoSize = true;
            this.raExportAccess.Checked = true;
            this.raExportAccess.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.raExportAccess.Location = new System.Drawing.Point(9, 18);
            this.raExportAccess.Name = "raExportAccess";
            this.raExportAccess.Size = new System.Drawing.Size(95, 16);
            this.raExportAccess.TabIndex = 4;
            this.raExportAccess.TabStop = true;
            this.raExportAccess.Text = "发布到Access";
            this.raExportAccess.UseVisualStyleBackColor = true;
            this.raExportAccess.CheckedChanged += new System.EventHandler(this.raExportAccess_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(22, 62);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 28;
            this.label8.Text = "数据表：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(22, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 21;
            this.label6.Text = "数据库：";
            // 
            // groupTemplate
            // 
            this.groupTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupTemplate.Controls.Add(this.dataParas);
            this.groupTemplate.Controls.Add(this.groupBox3);
            this.groupTemplate.Controls.Add(this.groupBox2);
            this.groupTemplate.Controls.Add(this.comTemplate);
            this.groupTemplate.Controls.Add(this.label5);
            this.groupTemplate.Location = new System.Drawing.Point(8, 101);
            this.groupTemplate.Name = "groupTemplate";
            this.groupTemplate.Size = new System.Drawing.Size(722, 234);
            this.groupTemplate.TabIndex = 48;
            this.groupTemplate.TabStop = false;
            this.groupTemplate.Text = "使用模版发布数据";
            this.groupTemplate.Visible = false;
            // 
            // dataParas
            // 
            this.dataParas.AllowUserToAddRows = false;
            this.dataParas.AllowUserToDeleteRows = false;
            this.dataParas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataParas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataParas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Para,
            this.PDataColumn,
            this.Column1});
            this.dataParas.Location = new System.Drawing.Point(22, 153);
            this.dataParas.Name = "dataParas";
            this.dataParas.RowTemplate.Height = 23;
            this.dataParas.Size = new System.Drawing.Size(687, 75);
            this.dataParas.TabIndex = 4;
            this.dataParas.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataParas_CellContentClick);
            this.dataParas.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataParas_CellEndEdit);
            this.dataParas.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataParas_DataError);
            // 
            // Para
            // 
            this.Para.HeaderText = "模版配置的参数";
            this.Para.Name = "Para";
            this.Para.Width = 120;
            // 
            // PDataColumn
            // 
            this.PDataColumn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.PDataColumn.HeaderText = "需要发布的数据";
            this.PDataColumn.Name = "PDataColumn";
            this.PDataColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.PDataColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.PDataColumn.Width = 200;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "手工输入发布数据";
            this.Column1.Name = "Column1";
            this.Column1.Width = 160;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.txtTDbConn);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Enabled = false;
            this.groupBox3.Location = new System.Drawing.Point(352, 45);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(357, 102);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "数据库发布信息配置";
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(265, 68);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(86, 24);
            this.button2.TabIndex = 2;
            this.button2.Text = "设置";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtTDbConn
            // 
            this.txtTDbConn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTDbConn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTDbConn.Location = new System.Drawing.Point(6, 40);
            this.txtTDbConn.Name = "txtTDbConn";
            this.txtTDbConn.Size = new System.Drawing.Size(345, 21);
            this.txtTDbConn.TabIndex = 1;
            this.txtTDbConn.TextChanged += new System.EventHandler(this.txtTDbConn_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 20);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(77, 12);
            this.label13.TabIndex = 0;
            this.label13.Text = "数据库连接：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtDomain);
            this.groupBox2.Controls.Add(this.txtPwd);
            this.groupBox2.Controls.Add(this.txtUser);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(22, 45);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(311, 102);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "web发布信息配置";
            // 
            // txtDomain
            // 
            this.txtDomain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDomain.Location = new System.Drawing.Point(75, 17);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(218, 21);
            this.txtDomain.TabIndex = 5;
            this.txtDomain.TextChanged += new System.EventHandler(this.txtDomain_TextChanged);
            // 
            // txtPwd
            // 
            this.txtPwd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPwd.Location = new System.Drawing.Point(75, 69);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.Size = new System.Drawing.Size(218, 21);
            this.txtPwd.TabIndex = 4;
            this.txtPwd.TextChanged += new System.EventHandler(this.txtPwd_TextChanged);
            // 
            // txtUser
            // 
            this.txtUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUser.Location = new System.Drawing.Point(75, 44);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(218, 21);
            this.txtUser.TabIndex = 3;
            this.txtUser.TextChanged += new System.EventHandler(this.txtUser_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(16, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 2;
            this.label12.Text = "发布网站：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 72);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 1;
            this.label10.Text = "密码：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 47);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "用户名：";
            // 
            // comTemplate
            // 
            this.comTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comTemplate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comTemplate.FormattingEnabled = true;
            this.comTemplate.Location = new System.Drawing.Point(83, 17);
            this.comTemplate.Name = "comTemplate";
            this.comTemplate.Size = new System.Drawing.Size(626, 20);
            this.comTemplate.TabIndex = 1;
            this.comTemplate.SelectedIndexChanged += new System.EventHandler(this.comTemplate_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "模版名称：";
            // 
            // txtLog
            // 
            this.txtLog.ContextMenuStrip = this.contextMenuStrip2;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(0, 19);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(903, 32);
            this.txtLog.TabIndex = 4;
            this.txtLog.Text = "";
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rmenuLookPublishData});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(197, 26);
            // 
            // rmenuLookPublishData
            // 
            this.rmenuLookPublishData.Name = "rmenuLookPublishData";
            this.rmenuLookPublishData.Size = new System.Drawing.Size(196, 22);
            this.rmenuLookPublishData.Text = "查看web发布返回数据";
            this.rmenuLookPublishData.Click += new System.EventHandler(this.rmenuLookPublishData_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.IsErrLog);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(903, 19);
            this.panel1.TabIndex = 3;
            // 
            // IsErrLog
            // 
            this.IsErrLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IsErrLog.AutoSize = true;
            this.IsErrLog.BackColor = System.Drawing.Color.Transparent;
            this.IsErrLog.Checked = true;
            this.IsErrLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IsErrLog.FlatAppearance.BorderSize = 0;
            this.IsErrLog.Location = new System.Drawing.Point(795, 3);
            this.IsErrLog.Name = "IsErrLog";
            this.IsErrLog.Size = new System.Drawing.Size(108, 16);
            this.IsErrLog.TabIndex = 1;
            this.IsErrLog.Text = "仅输出错误日志";
            this.IsErrLog.UseVisualStyleBackColor = false;
            this.IsErrLog.CheckedChanged += new System.EventHandler(this.IsErrLog_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(6, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "发布日志";
            // 
            // dataNavi
            // 
            this.dataNavi.AddNewItem = this.bindingNavigatorAddNewItem;
            this.dataNavi.CountItem = this.bindingNavigatorCountItem;
            this.dataNavi.DeleteItem = null;
            this.dataNavi.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolIsPublish,
            this.toolStartPublish,
            this.toolStopPublish,
            this.toolStripSeparator1,
            this.toolSaveData,
            this.toolStripSeparator23,
            this.bindingNavigatorAddNewItem,
            this.toolAddField,
            this.toolDelField,
            this.toolStripSeparator22,
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.toolFilter,
            this.toolEdit,
            this.toolStripSeparator2,
            this.toolStripButton1});
            this.dataNavi.Location = new System.Drawing.Point(0, 0);
            this.dataNavi.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.dataNavi.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.dataNavi.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.dataNavi.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.dataNavi.Name = "dataNavi";
            this.dataNavi.PositionItem = this.bindingNavigatorPositionItem;
            this.dataNavi.Size = new System.Drawing.Size(903, 25);
            this.dataNavi.TabIndex = 5;
            this.dataNavi.Text = "bindingNavigator1";
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorAddNewItem.Text = "新添记录";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(32, 22);
            this.bindingNavigatorCountItem.Text = "/ {0}";
            this.bindingNavigatorCountItem.ToolTipText = "总项数";
            // 
            // toolIsPublish
            // 
            this.toolIsPublish.Image = ((System.Drawing.Image)(resources.GetObject("toolIsPublish.Image")));
            this.toolIsPublish.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolIsPublish.Name = "toolIsPublish";
            this.toolIsPublish.Size = new System.Drawing.Size(76, 22);
            this.toolIsPublish.Text = "数据发布";
            this.toolIsPublish.Click += new System.EventHandler(this.toolIsPublish_Click);
            // 
            // toolStartPublish
            // 
            this.toolStartPublish.Image = ((System.Drawing.Image)(resources.GetObject("toolStartPublish.Image")));
            this.toolStartPublish.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStartPublish.Name = "toolStartPublish";
            this.toolStartPublish.Size = new System.Drawing.Size(52, 22);
            this.toolStartPublish.Text = "开始";
            this.toolStartPublish.Click += new System.EventHandler(this.toolStartPublish_Click);
            // 
            // toolStopPublish
            // 
            this.toolStopPublish.Enabled = false;
            this.toolStopPublish.Image = ((System.Drawing.Image)(resources.GetObject("toolStopPublish.Image")));
            this.toolStopPublish.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStopPublish.Name = "toolStopPublish";
            this.toolStopPublish.Size = new System.Drawing.Size(52, 22);
            this.toolStopPublish.Text = "停止";
            this.toolStopPublish.Click += new System.EventHandler(this.toolStopPublish_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolSaveData
            // 
            this.toolSaveData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolSaveData.Enabled = false;
            this.toolSaveData.Image = ((System.Drawing.Image)(resources.GetObject("toolSaveData.Image")));
            this.toolSaveData.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolSaveData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSaveData.Name = "toolSaveData";
            this.toolSaveData.Size = new System.Drawing.Size(23, 22);
            this.toolSaveData.Text = "保存数据";
            this.toolSaveData.Click += new System.EventHandler(this.toolSaveData_Click);
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            this.toolStripSeparator23.Size = new System.Drawing.Size(6, 25);
            // 
            // toolAddField
            // 
            this.toolAddField.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolAddField.Enabled = false;
            this.toolAddField.Image = ((System.Drawing.Image)(resources.GetObject("toolAddField.Image")));
            this.toolAddField.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAddField.Name = "toolAddField";
            this.toolAddField.Size = new System.Drawing.Size(23, 22);
            this.toolAddField.Text = "追加新列";
            this.toolAddField.Click += new System.EventHandler(this.toolAddField_Click);
            // 
            // toolDelField
            // 
            this.toolDelField.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolDelField.Enabled = false;
            this.toolDelField.Image = ((System.Drawing.Image)(resources.GetObject("toolDelField.Image")));
            this.toolDelField.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolDelField.Name = "toolDelField";
            this.toolDelField.Size = new System.Drawing.Size(23, 22);
            this.toolDelField.Text = "删除所选列";
            this.toolDelField.Click += new System.EventHandler(this.toolDelField_Click);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            this.toolStripSeparator22.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem.Text = "移到第一条记录";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem.Text = "移到上一条记录";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "位置";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 23);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "当前位置";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem.Text = "移到下一条记录";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem.Text = "移到最后一条记录";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolFilter
            // 
            this.toolFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolNoFilter,
            this.toolStripSeparator6,
            this.toolMore,
            this.toolLess,
            this.toolStripSeparator10,
            this.toolEqual,
            this.toolNoEqual,
            this.toolStripSeparator11,
            this.toolStart,
            this.toolStartNo,
            this.toolInclude,
            this.toolEnd,
            this.toolLenMore,
            this.toolStripSeparator8,
            this.toolCustomFilter});
            this.toolFilter.Enabled = false;
            this.toolFilter.Image = ((System.Drawing.Image)(resources.GetObject("toolFilter.Image")));
            this.toolFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolFilter.Name = "toolFilter";
            this.toolFilter.Size = new System.Drawing.Size(29, 22);
            this.toolFilter.Text = "筛选";
            // 
            // toolNoFilter
            // 
            this.toolNoFilter.Name = "toolNoFilter";
            this.toolNoFilter.Size = new System.Drawing.Size(136, 22);
            this.toolNoFilter.Text = "所有";
            this.toolNoFilter.Click += new System.EventHandler(this.toolNoFilter_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(133, 6);
            // 
            // toolMore
            // 
            this.toolMore.Name = "toolMore";
            this.toolMore.Size = new System.Drawing.Size(136, 22);
            this.toolMore.Text = "大于";
            this.toolMore.Click += new System.EventHandler(this.toolMore_Click);
            // 
            // toolLess
            // 
            this.toolLess.Name = "toolLess";
            this.toolLess.Size = new System.Drawing.Size(136, 22);
            this.toolLess.Text = "小于";
            this.toolLess.Click += new System.EventHandler(this.toolLess_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(133, 6);
            // 
            // toolEqual
            // 
            this.toolEqual.Name = "toolEqual";
            this.toolEqual.Size = new System.Drawing.Size(136, 22);
            this.toolEqual.Text = "等于";
            this.toolEqual.Click += new System.EventHandler(this.toolEqual_Click);
            // 
            // toolNoEqual
            // 
            this.toolNoEqual.Name = "toolNoEqual";
            this.toolNoEqual.Size = new System.Drawing.Size(136, 22);
            this.toolNoEqual.Text = "不等于";
            this.toolNoEqual.Click += new System.EventHandler(this.toolNoEqual_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(133, 6);
            // 
            // toolStart
            // 
            this.toolStart.Name = "toolStart";
            this.toolStart.Size = new System.Drawing.Size(136, 22);
            this.toolStart.Text = "开头是";
            this.toolStart.Click += new System.EventHandler(this.toolStart_Click);
            // 
            // toolStartNo
            // 
            this.toolStartNo.Name = "toolStartNo";
            this.toolStartNo.Size = new System.Drawing.Size(136, 22);
            this.toolStartNo.Text = "开头不是";
            this.toolStartNo.Click += new System.EventHandler(this.toolStartNo_Click);
            // 
            // toolInclude
            // 
            this.toolInclude.Name = "toolInclude";
            this.toolInclude.Size = new System.Drawing.Size(136, 22);
            this.toolInclude.Text = "包含";
            this.toolInclude.Click += new System.EventHandler(this.toolInclude_Click);
            // 
            // toolEnd
            // 
            this.toolEnd.Name = "toolEnd";
            this.toolEnd.Size = new System.Drawing.Size(136, 22);
            this.toolEnd.Text = "结尾是";
            this.toolEnd.Click += new System.EventHandler(this.toolEnd_Click);
            // 
            // toolLenMore
            // 
            this.toolLenMore.Name = "toolLenMore";
            this.toolLenMore.Size = new System.Drawing.Size(136, 22);
            this.toolLenMore.Text = "长度大于";
            this.toolLenMore.Click += new System.EventHandler(this.toolLenMore_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(133, 6);
            // 
            // toolCustomFilter
            // 
            this.toolCustomFilter.Name = "toolCustomFilter";
            this.toolCustomFilter.Size = new System.Drawing.Size(136, 22);
            this.toolCustomFilter.Text = "自定义函数";
            this.toolCustomFilter.Click += new System.EventHandler(this.toolCustomFilter_Click);
            // 
            // toolEdit
            // 
            this.toolEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolAutoCutFlag,
            this.toolStripSeparator24,
            this.toolAutoID,
            this.toolDelRepeatRow,
            this.toolStripSeparator12,
            this.toolAddPre,
            this.toolAddSuffix,
            this.toolReplace,
            this.toolCutLeft,
            this.toolCutRight,
            this.toolStripSeparator13,
            this.toolSetEmpty,
            this.toolDelHtml,
            this.toolValue,
            this.toolStripSeparator16,
            this.toolCustomData});
            this.toolEdit.Enabled = false;
            this.toolEdit.Image = ((System.Drawing.Image)(resources.GetObject("toolEdit.Image")));
            this.toolEdit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolEdit.Name = "toolEdit";
            this.toolEdit.Size = new System.Drawing.Size(25, 22);
            this.toolEdit.Text = "修改";
            // 
            // toolAutoCutFlag
            // 
            this.toolAutoCutFlag.Name = "toolAutoCutFlag";
            this.toolAutoCutFlag.Size = new System.Drawing.Size(196, 22);
            this.toolAutoCutFlag.Text = "自动去除列头非法字符";
            this.toolAutoCutFlag.Click += new System.EventHandler(this.toolEditColHeader_Click);
            // 
            // toolStripSeparator24
            // 
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            this.toolStripSeparator24.Size = new System.Drawing.Size(193, 6);
            // 
            // toolAutoID
            // 
            this.toolAutoID.Name = "toolAutoID";
            this.toolAutoID.Size = new System.Drawing.Size(196, 22);
            this.toolAutoID.Text = "自动编号";
            this.toolAutoID.Click += new System.EventHandler(this.toolAutoID_Click);
            // 
            // toolDelRepeatRow
            // 
            this.toolDelRepeatRow.Name = "toolDelRepeatRow";
            this.toolDelRepeatRow.Size = new System.Drawing.Size(196, 22);
            this.toolDelRepeatRow.Text = "删除重复行";
            this.toolDelRepeatRow.Click += new System.EventHandler(this.toolDelRepeatRow_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(193, 6);
            // 
            // toolAddPre
            // 
            this.toolAddPre.Name = "toolAddPre";
            this.toolAddPre.Size = new System.Drawing.Size(196, 22);
            this.toolAddPre.Text = "增加前缀";
            this.toolAddPre.Click += new System.EventHandler(this.toolAddPre_Click);
            // 
            // toolAddSuffix
            // 
            this.toolAddSuffix.Name = "toolAddSuffix";
            this.toolAddSuffix.Size = new System.Drawing.Size(196, 22);
            this.toolAddSuffix.Text = "增加后缀";
            this.toolAddSuffix.Click += new System.EventHandler(this.toolAddSuffix_Click);
            // 
            // toolReplace
            // 
            this.toolReplace.Name = "toolReplace";
            this.toolReplace.Size = new System.Drawing.Size(196, 22);
            this.toolReplace.Text = "替换";
            this.toolReplace.Click += new System.EventHandler(this.toolReplace_Click);
            // 
            // toolCutLeft
            // 
            this.toolCutLeft.Name = "toolCutLeft";
            this.toolCutLeft.Size = new System.Drawing.Size(196, 22);
            this.toolCutLeft.Text = "从左截取字符";
            this.toolCutLeft.Click += new System.EventHandler(this.toolCutLeft_Click);
            // 
            // toolCutRight
            // 
            this.toolCutRight.Name = "toolCutRight";
            this.toolCutRight.Size = new System.Drawing.Size(196, 22);
            this.toolCutRight.Text = "从右截取字符";
            this.toolCutRight.Click += new System.EventHandler(this.toolCutRight_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(193, 6);
            // 
            // toolSetEmpty
            // 
            this.toolSetEmpty.Name = "toolSetEmpty";
            this.toolSetEmpty.Size = new System.Drawing.Size(196, 22);
            this.toolSetEmpty.Text = "清空";
            this.toolSetEmpty.Click += new System.EventHandler(this.toolSetEmpty_Click);
            // 
            // toolDelHtml
            // 
            this.toolDelHtml.Name = "toolDelHtml";
            this.toolDelHtml.Size = new System.Drawing.Size(196, 22);
            this.toolDelHtml.Text = "去除网页符号";
            this.toolDelHtml.Click += new System.EventHandler(this.toolDelHtml_Click);
            // 
            // toolValue
            // 
            this.toolValue.Name = "toolValue";
            this.toolValue.Size = new System.Drawing.Size(196, 22);
            this.toolValue.Text = "固定值";
            this.toolValue.Click += new System.EventHandler(this.toolValue_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(193, 6);
            // 
            // toolCustomData
            // 
            this.toolCustomData.Enabled = false;
            this.toolCustomData.Name = "toolCustomData";
            this.toolCustomData.Size = new System.Drawing.Size(196, 22);
            this.toolCustomData.Text = "自定义函数";
            this.toolCustomData.Click += new System.EventHandler(this.toolCustomData_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(52, 22);
            this.toolStripButton1.Text = "退出";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // dataData
            // 
            this.dataData.AllowUserToAddRows = false;
            this.dataData.AllowUserToOrderColumns = true;
            this.dataData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataData.ContextMenuStrip = this.contextMenuStrip1;
            this.dataData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataData.Location = new System.Drawing.Point(0, 25);
            this.dataData.Name = "dataData";
            this.dataData.RowTemplate.Height = 23;
            this.dataData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataData.Size = new System.Drawing.Size(903, 124);
            this.dataData.TabIndex = 4;
            this.dataData.VirtualMode = true;
            this.dataData.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataData_CellValueChanged);
            this.dataData.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dataData_ColumnAdded);
            this.dataData.ColumnRemoved += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dataData_ColumnRemoved);
            this.dataData.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataData_DataBindingComplete);
            this.dataData.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataData_DataError);
            this.dataData.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataData_RowsAdded);
            this.dataData.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataData_RowsRemoved);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.dataData);
            this.splitContainer3.Panel1.Controls.Add(this.dataNavi);
            this.splitContainer3.Panel1MinSize = 0;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer5);
            this.splitContainer3.Panel2MinSize = 0;
            this.splitContainer3.Size = new System.Drawing.Size(903, 548);
            this.splitContainer3.SplitterDistance = 149;
            this.splitContainer3.TabIndex = 3;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.staFilterTitle,
            this.stateFilter,
            this.staShowAll,
            this.stateRecords,
            this.staExportState,
            this.staProgressbar,
            this.staProgressTitle,
            this.stateProgress,
            this.staError});
            this.statusStrip1.Location = new System.Drawing.Point(0, 548);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(903, 24);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // staFilterTitle
            // 
            this.staFilterTitle.Image = ((System.Drawing.Image)(resources.GetObject("staFilterTitle.Image")));
            this.staFilterTitle.Name = "staFilterTitle";
            this.staFilterTitle.Size = new System.Drawing.Size(84, 19);
            this.staFilterTitle.Text = "筛选条件：";
            // 
            // stateFilter
            // 
            this.stateFilter.Name = "stateFilter";
            this.stateFilter.Size = new System.Drawing.Size(20, 19);
            this.stateFilter.Text = "无";
            // 
            // staShowAll
            // 
            this.staShowAll.IsLink = true;
            this.staShowAll.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.staShowAll.LinkVisited = true;
            this.staShowAll.Name = "staShowAll";
            this.staShowAll.Size = new System.Drawing.Size(56, 19);
            this.staShowAll.Text = "显示全部";
            this.staShowAll.Visible = false;
            this.staShowAll.Click += new System.EventHandler(this.staShowAll_Click);
            // 
            // stateRecords
            // 
            this.stateRecords.AutoSize = false;
            this.stateRecords.Name = "stateRecords";
            this.stateRecords.Size = new System.Drawing.Size(150, 19);
            this.stateRecords.Text = " 0";
            this.stateRecords.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // staExportState
            // 
            this.staExportState.Image = ((System.Drawing.Image)(resources.GetObject("staExportState.Image")));
            this.staExportState.Name = "staExportState";
            this.staExportState.Size = new System.Drawing.Size(48, 19);
            this.staExportState.Text = "停止";
            // 
            // staProgressbar
            // 
            this.staProgressbar.Name = "staProgressbar";
            this.staProgressbar.Size = new System.Drawing.Size(100, 18);
            this.staProgressbar.Visible = false;
            // 
            // staProgressTitle
            // 
            this.staProgressTitle.Name = "staProgressTitle";
            this.staProgressTitle.Size = new System.Drawing.Size(44, 19);
            this.staProgressTitle.Text = "进度：";
            // 
            // stateProgress
            // 
            this.stateProgress.Name = "stateProgress";
            this.stateProgress.Size = new System.Drawing.Size(27, 19);
            this.stateProgress.Text = "0/0";
            // 
            // staError
            // 
            this.staError.AutoSize = false;
            this.staError.Image = ((System.Drawing.Image)(resources.GetObject("staError.Image")));
            this.staError.Name = "staError";
            this.staError.Size = new System.Drawing.Size(120, 19);
            this.staError.Text = "0";
            this.staError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "模版配置的参数";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 120;
            // 
            // dataGridViewComboBoxColumn1
            // 
            this.dataGridViewComboBoxColumn1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.dataGridViewComboBoxColumn1.HeaderText = "需要发布的数据";
            this.dataGridViewComboBoxColumn1.Name = "dataGridViewComboBoxColumn1";
            this.dataGridViewComboBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewComboBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dataGridViewComboBoxColumn1.Width = 200;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "需要发布的说";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 200;
            // 
            // uconMyDataOp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer3);
            this.Controls.Add(this.statusStrip1);
            this.Name = "uconMyDataOp";
            this.Size = new System.Drawing.Size(903, 572);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip3.ResumeLayout(false);
            this.rmenuGetFormat.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udPIntervalTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udThread)).EndInit();
            this.groupWeb.ResumeLayout(false);
            this.groupWeb.PerformLayout();
            this.groupData.ResumeLayout(false);
            this.groupData.PerformLayout();
            this.groupTemplate.ResumeLayout(false);
            this.groupTemplate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataParas)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.contextMenuStrip2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataNavi)).EndInit();
            this.dataNavi.ResumeLayout(false);
            this.dataNavi.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataData)).EndInit();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip rmenuGetFormat;
        private System.Windows.Forms.ToolStripMenuItem toolmenuPost1;
        private System.Windows.Forms.ToolStripMenuItem toolmenuPost2;
        private System.Windows.Forms.ToolStripMenuItem toolmenuPostData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem rmenuImportPublishInfo;
        private System.Windows.Forms.ToolStripMenuItem rmenuDelPublishInfo;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem rmenuAddFiled;
        private System.Windows.Forms.ToolStripMenuItem rmenuDelField;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem rmenuSelectCol;
        private System.Windows.Forms.ToolStripMenuItem rmenuFilter;
        private System.Windows.Forms.ToolStripMenuItem rmenuMore;
        private System.Windows.Forms.ToolStripMenuItem rmenuLess;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem rmenuTxtEqual;
        private System.Windows.Forms.ToolStripMenuItem rmenuTxtNoEqual;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem rmenuTxtStart;
        private System.Windows.Forms.ToolStripMenuItem rmenuTxtStartNo;
        private System.Windows.Forms.ToolStripMenuItem rmenuTxtInclude;
        private System.Windows.Forms.ToolStripMenuItem rmenuTxtEnd;
        private System.Windows.Forms.ToolStripMenuItem rmenuLenMore;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem rmenuCustomFilter;
        private System.Windows.Forms.ToolStripMenuItem rmenuEdit;
        private System.Windows.Forms.ToolStripMenuItem rmenuAutoID;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripMenuItem rmenuAddPre;
        private System.Windows.Forms.ToolStripMenuItem rmenuAddSuffix;
        private System.Windows.Forms.ToolStripMenuItem rmenuReplace;
        private System.Windows.Forms.ToolStripMenuItem rmenuCutLeft;
        private System.Windows.Forms.ToolStripMenuItem rmenuCutRight;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem rmenuSetEmpty;
        private System.Windows.Forms.ToolStripMenuItem rmenuDelHtml;
        private System.Windows.Forms.ToolStripMenuItem rmenuValue;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripMenuItem rmenuCustomData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.SplitContainer splitContainer5;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.TreeView treePublish;
        private System.Windows.Forms.Button butSaveTemplate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton raPublishWeb;
        private System.Windows.Forms.RadioButton raPublishData;
        private System.Windows.Forms.NumericUpDown udThread;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox IsDelRepRow;
        private System.Windows.Forms.GroupBox groupWeb;
        private System.Windows.Forms.TextBox txtSucceedFlag;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.ComboBox comExportUrlCode;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.TextBox txtExportCookie;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox txtExportUrl;
        private System.Windows.Forms.GroupBox groupData;
        private System.Windows.Forms.ComboBox comTableName;
        private System.Windows.Forms.TextBox txtInsertSql;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.TextBox txtDataSource;
        private System.Windows.Forms.RadioButton raExportMySql;
        private System.Windows.Forms.RadioButton raExportMSSQL;
        private System.Windows.Forms.RadioButton raExportAccess;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox IsErrLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.BindingNavigator dataNavi;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton toolIsPublish;
        private System.Windows.Forms.ToolStripButton toolStartPublish;
        private System.Windows.Forms.ToolStripButton toolStopPublish;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolSaveData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator23;
        private System.Windows.Forms.ToolStripButton toolAddField;
        private System.Windows.Forms.ToolStripButton toolDelField;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator22;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton toolFilter;
        private System.Windows.Forms.ToolStripMenuItem toolNoFilter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem toolMore;
        private System.Windows.Forms.ToolStripMenuItem toolLess;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem toolEqual;
        private System.Windows.Forms.ToolStripMenuItem toolNoEqual;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem toolStart;
        private System.Windows.Forms.ToolStripMenuItem toolStartNo;
        private System.Windows.Forms.ToolStripMenuItem toolInclude;
        private System.Windows.Forms.ToolStripMenuItem toolEnd;
        private System.Windows.Forms.ToolStripMenuItem toolLenMore;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem toolCustomFilter;
        private System.Windows.Forms.ToolStripDropDownButton toolEdit;
        private System.Windows.Forms.ToolStripMenuItem toolAutoID;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem toolAddPre;
        private System.Windows.Forms.ToolStripMenuItem toolAddSuffix;
        private System.Windows.Forms.ToolStripMenuItem toolReplace;
        private System.Windows.Forms.ToolStripMenuItem toolCutLeft;
        private System.Windows.Forms.ToolStripMenuItem toolCutRight;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem toolSetEmpty;
        private System.Windows.Forms.ToolStripMenuItem toolDelHtml;
        private System.Windows.Forms.ToolStripMenuItem toolValue;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem toolCustomData;
        private System.Windows.Forms.DataGridView dataData;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel staFilterTitle;
        private System.Windows.Forms.ToolStripStatusLabel stateFilter;
        private System.Windows.Forms.ToolStripStatusLabel staShowAll;
        private System.Windows.Forms.ToolStripStatusLabel stateRecords;
        private System.Windows.Forms.ToolStripStatusLabel staExportState;
        private System.Windows.Forms.ToolStripProgressBar staProgressbar;
        private System.Windows.Forms.ToolStripStatusLabel staProgressTitle;
        private System.Windows.Forms.ToolStripStatusLabel stateProgress;
        private System.Windows.Forms.ToolStripStatusLabel staError;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem rmenuDelRepeatRow;
        private System.Windows.Forms.ToolStripMenuItem toolDelRepeatRow;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtHeader;
        private System.Windows.Forms.CheckBox IsHeader;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem rmenuExport;
        private System.Windows.Forms.ToolStripMenuItem rmenuImport;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown udPIntervalTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem rmenuLookPublishData;
        private System.Windows.Forms.ToolStripMenuItem rmenuPlugin;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
        private System.Windows.Forms.Button butNew;
        private System.Windows.Forms.CheckBox IsPublishByTemplate;
        private System.Windows.Forms.GroupBox groupTemplate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comTemplate;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtTDbConn;
        private System.Windows.Forms.DataGridView dataParas;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Para;
        private System.Windows.Forms.DataGridViewComboBoxColumn PDataColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.CheckBox IsSqlTrue;
        private System.Windows.Forms.ToolStripMenuItem toolAutoCutFlag;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator24;
        private System.Windows.Forms.ToolStripMenuItem rmenuEditColHeader;
        private System.Windows.Forms.RadioButton raExportOracle;
    }
}

namespace MinerSpider
{
    partial class frmAddMultiPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddMultiPage));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtCustomLetter = new System.Windows.Forms.TextBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtGetEnd = new System.Windows.Forms.TextBox();
            this.txtGetStart = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.txtSufStr = new System.Windows.Forms.TextBox();
            this.txtPrestr = new System.Windows.Forms.TextBox();
            this.txtNRule = new System.Windows.Forms.TextBox();
            this.IsSufStr = new System.Windows.Forms.CheckBox();
            this.IsPreStr = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtUrlName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comMultiLevel = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtCustomLetter);
            this.groupBox1.Controls.Add(this.checkBox6);
            this.groupBox1.Controls.Add(this.checkBox5);
            this.groupBox1.Controls.Add(this.checkBox4);
            this.groupBox1.Controls.Add(this.checkBox3);
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.checkBox2);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtGetEnd);
            this.groupBox1.Controls.Add(this.txtGetStart);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.txtSufStr);
            this.groupBox1.Controls.Add(this.txtPrestr);
            this.groupBox1.Controls.Add(this.txtNRule);
            this.groupBox1.Controls.Add(this.IsSufStr);
            this.groupBox1.Controls.Add(this.IsPreStr);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(535, 205);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "解析规则可通过网页进行获取";
            // 
            // txtCustomLetter
            // 
            this.txtCustomLetter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCustomLetter.Enabled = false;
            this.txtCustomLetter.Location = new System.Drawing.Point(308, 101);
            this.txtCustomLetter.Name = "txtCustomLetter";
            this.txtCustomLetter.Size = new System.Drawing.Size(221, 21);
            this.txtCustomLetter.TabIndex = 10;
            this.txtCustomLetter.TextChanged += new System.EventHandler(this.txtCustomLetter_TextChanged);
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBox6.Location = new System.Drawing.Point(219, 102);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(81, 16);
            this.checkBox6.TabIndex = 9;
            this.checkBox6.Text = "自定义字符";
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox6.CheckedChanged += new System.EventHandler(this.checkBox6_CheckedChanged);
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBox5.Location = new System.Drawing.Point(185, 101);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(27, 16);
            this.checkBox5.TabIndex = 8;
            this.checkBox5.Text = "\"";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.checkBox5_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBox4.Location = new System.Drawing.Point(150, 101);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(27, 16);
            this.checkBox4.TabIndex = 7;
            this.checkBox4.Text = "\'";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBox3.Location = new System.Drawing.Point(114, 101);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(27, 16);
            this.checkBox3.TabIndex = 6;
            this.checkBox3.Text = "#";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButton3.Location = new System.Drawing.Point(128, 18);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(130, 16);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.Text = "使用自定义正则配置";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.Click += new System.EventHandler(this.radioButton3_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBox2.Location = new System.Drawing.Point(60, 101);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(45, 16);
            this.checkBox2.TabIndex = 5;
            this.checkBox2.Text = "空格";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBox1.Location = new System.Drawing.Point(22, 101);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(27, 16);
            this.checkBox1.TabIndex = 46;
            this.checkBox1.Text = ">";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(12, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 12);
            this.label1.TabIndex = 45;
            this.label1.Text = "选择不允许包含的字符：";
            // 
            // txtGetEnd
            // 
            this.txtGetEnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGetEnd.Location = new System.Drawing.Point(347, 40);
            this.txtGetEnd.Multiline = true;
            this.txtGetEnd.Name = "txtGetEnd";
            this.txtGetEnd.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGetEnd.Size = new System.Drawing.Size(182, 39);
            this.txtGetEnd.TabIndex = 4;
            this.txtGetEnd.TextChanged += new System.EventHandler(this.txtGetEnd_TextChanged);
            // 
            // txtGetStart
            // 
            this.txtGetStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGetStart.Location = new System.Drawing.Point(83, 40);
            this.txtGetStart.Multiline = true;
            this.txtGetStart.Name = "txtGetStart";
            this.txtGetStart.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGetStart.Size = new System.Drawing.Size(182, 39);
            this.txtGetStart.TabIndex = 3;
            this.txtGetStart.TextChanged += new System.EventHandler(this.txtGetStart_TextChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label16.Location = new System.Drawing.Point(12, 42);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(65, 12);
            this.label16.TabIndex = 43;
            this.label16.Text = "起始位置：";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label17.Location = new System.Drawing.Point(271, 42);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(65, 12);
            this.label17.TabIndex = 44;
            this.label17.Text = "终止位置：";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButton2.Location = new System.Drawing.Point(14, 18);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(82, 16);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "自定义配置";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.Click += new System.EventHandler(this.radioButton2_Click);
            // 
            // txtSufStr
            // 
            this.txtSufStr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSufStr.Enabled = false;
            this.txtSufStr.Location = new System.Drawing.Point(238, 177);
            this.txtSufStr.Name = "txtSufStr";
            this.txtSufStr.Size = new System.Drawing.Size(291, 21);
            this.txtSufStr.TabIndex = 13;
            // 
            // txtPrestr
            // 
            this.txtPrestr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPrestr.Enabled = false;
            this.txtPrestr.Location = new System.Drawing.Point(238, 153);
            this.txtPrestr.Name = "txtPrestr";
            this.txtPrestr.Size = new System.Drawing.Size(291, 21);
            this.txtPrestr.TabIndex = 12;
            // 
            // txtNRule
            // 
            this.txtNRule.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNRule.Enabled = false;
            this.txtNRule.Location = new System.Drawing.Point(83, 127);
            this.txtNRule.Multiline = true;
            this.txtNRule.Name = "txtNRule";
            this.txtNRule.Size = new System.Drawing.Size(446, 21);
            this.txtNRule.TabIndex = 11;
            // 
            // IsSufStr
            // 
            this.IsSufStr.AutoSize = true;
            this.IsSufStr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IsSufStr.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.IsSufStr.Location = new System.Drawing.Point(22, 175);
            this.IsSufStr.Name = "IsSufStr";
            this.IsSufStr.Size = new System.Drawing.Size(213, 16);
            this.IsSufStr.TabIndex = 31;
            this.IsSufStr.Text = "在多页规则匹配结果尾增加字符串：";
            this.IsSufStr.UseVisualStyleBackColor = true;
            this.IsSufStr.CheckedChanged += new System.EventHandler(this.IsSufStr_CheckedChanged);
            // 
            // IsPreStr
            // 
            this.IsPreStr.AutoSize = true;
            this.IsPreStr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IsPreStr.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.IsPreStr.Location = new System.Drawing.Point(22, 154);
            this.IsPreStr.Name = "IsPreStr";
            this.IsPreStr.Size = new System.Drawing.Size(213, 16);
            this.IsPreStr.TabIndex = 30;
            this.IsPreStr.Text = "在多页规则匹配结果前增加字符串：";
            this.IsPreStr.UseVisualStyleBackColor = true;
            this.IsPreStr.CheckedChanged += new System.EventHandler(this.IsPreStr_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(19, 131);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 29;
            this.label8.Text = "导航规则：";
            // 
            // txtUrlName
            // 
            this.txtUrlName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUrlName.Location = new System.Drawing.Point(86, 8);
            this.txtUrlName.Name = "txtUrlName";
            this.txtUrlName.Size = new System.Drawing.Size(446, 21);
            this.txtUrlName.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 53;
            this.label2.Text = "规则名称：";
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Image = ((System.Drawing.Image)(resources.GetObject("cmdCancel.Image")));
            this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(483, 318);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(65, 21);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "取  消";
            this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdOK.Image = ((System.Drawing.Image)(resources.GetObject("cmdOK.Image")));
            this.cmdOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(412, 318);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(65, 21);
            this.cmdOK.TabIndex = 0;
            this.cmdOK.Text = "确  定";
            this.cmdOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.ImageList = this.imageList1;
            this.tabControl1.Location = new System.Drawing.Point(8, 74);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(554, 238);
            this.tabControl1.TabIndex = 38;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.ImageIndex = 0;
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(546, 208);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "解析规则";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.ImageIndex = 1;
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(546, 208);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "参数规则";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtUrl);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(535, 205);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "参数规则为只能在源码中匹配网址参数";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(442, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "插入时间戳";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButton1.Location = new System.Drawing.Point(16, 19);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(166, 16);
            this.radioButton1.TabIndex = 3;
            this.radioButton1.Text = "使用参数设置获取多页网址";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.ClientSizeChanged += new System.EventHandler(this.radioButton1_ClientSizeChanged);
            this.radioButton1.Click += new System.EventHandler(this.radioButton1_Click);
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label4.Location = new System.Drawing.Point(13, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(516, 75);
            this.label4.TabIndex = 2;
            this.label4.Text = "通常情况下，多页采集的网址是由采集页中的某个参数数值作为多页网址参数值进行网页数据请求，所以，针对此种情况，用户可通过采集规则配置需要获取的参数数值，然后通过在此" +
                "配置参数信息，进行多页数据的请求。参数请用{}进行标识。\r\n\r\n注意：参数的值由采集页数据采集获取，注意：参数名称必须与配置的采集名称相同，否则无法替换。";
            // 
            // txtUrl
            // 
            this.txtUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUrl.Enabled = false;
            this.txtUrl.Location = new System.Drawing.Point(62, 45);
            this.txtUrl.Multiline = true;
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtUrl.Size = new System.Drawing.Size(455, 75);
            this.txtUrl.TabIndex = 1;
            this.txtUrl.Leave += new System.EventHandler(this.txtUrl_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "网址：";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "A31.png");
            this.imageList1.Images.SetKeyName(1, "superindex16.png");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(27, 322);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(330, 12);
            this.label5.TabIndex = 83;
            this.label5.Text = "小提示：将鼠标移动到相应的配置项，会有帮助说明哦！";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 84;
            this.label6.Text = "对应页面：";
            // 
            // comMultiLevel
            // 
            this.comMultiLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comMultiLevel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comMultiLevel.FormattingEnabled = true;
            this.comMultiLevel.Location = new System.Drawing.Point(86, 36);
            this.comMultiLevel.Name = "comMultiLevel";
            this.comMultiLevel.Size = new System.Drawing.Size(106, 20);
            this.comMultiLevel.TabIndex = 85;
            // 
            // label7
            // 
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(198, 33);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(334, 39);
            this.label7.TabIndex = 86;
            this.label7.Text = "默认多页采集是从采集页扩展，但也可从导航页扩展，采集页默认为：0。如果多页采集对应的是导航页，则导航页必须也采集数据，且强制为1*1数据关系，否则会失败。";
            // 
            // frmAddMultiPage
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(562, 350);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.comMultiLevel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtUrlName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddMultiPage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "增加多页采集网址";
            this.Load += new System.EventHandler(this.frmAddMultiPage_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtUrlName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCustomLetter;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtGetEnd;
        private System.Windows.Forms.TextBox txtGetStart;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.TextBox txtSufStr;
        private System.Windows.Forms.TextBox txtPrestr;
        private System.Windows.Forms.TextBox txtNRule;
        private System.Windows.Forms.CheckBox IsSufStr;
        private System.Windows.Forms.CheckBox IsPreStr;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comMultiLevel;
        private System.Windows.Forms.Label label7;
    }
}
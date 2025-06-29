using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using NetMiner.Gather;
using NetMiner.Resource;

namespace MinerSpider
{
    public partial class frmAddDataEditRule : Form
    {
        //定义一个访问资源文件的变量
        private ResourceManager rmPara;
        public delegate void ReturnEditRule(string eRule,string strRule);
        public ReturnEditRule rERule;
        public string m_TaskName;

        public frmAddDataEditRule(string tName)
        {
            InitializeComponent();

            rmPara = new ResourceManager("NetMiner.Resource.Resources.globalPara", Assembly.Load("NetMiner.Resource"));

            this.m_TaskName = tName;
        }

        private void frmAddDataEditRule_FormClosing(object sender, FormClosingEventArgs e)
        {
            rmPara = null;
        }

        private void frmAddDataEditRule_Load(object sender, EventArgs e)
        {
            //初始化网址加工操作
            this.raWebPage.Checked = true;

            this.ExportRuleType.Items.Clear();

            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit2"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit15"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit16"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit44"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit35"));
            

            if (Program.SominerVersion == cGlobalParas.VersionType.Free)
            {
                this.raDataEdit.Enabled = false;
                this.raDataGather.Enabled = false;
                this.raOther.Enabled = false;
                this.raCondition.Enabled = false;
            }



            if (Program.SominerVersion == cGlobalParas.VersionType.Program ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise )
            {
                this.raPlugin.Enabled = true;
                this.raDBRule.Enabled = true;
            }
            else
            {
                this.raPlugin.Enabled = false;
                this.raDBRule.Enabled = false;
            }

        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void raWebPage_CheckedChanged(object sender, EventArgs e)
        {
            if (raWebPage.Checked == true)
            {
                this.ExportRuleType.Items.Clear();

                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit2"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit15"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit16"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit44"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit35"));

                this.txtRule.Enabled = false;
                this.txtOldValue.Enabled = false;
                this.txtNewValue.Enabled =false;
            }
        }

        private void raString_CheckedChanged(object sender, EventArgs e)
        {
            if (raString.Checked == true)
            {
                this.ExportRuleType.Items.Clear();

                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit3"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit4"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit5"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit6"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit7"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit8"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit9"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit20"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit30"));

                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit37"));

                if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                    Program.SominerVersion == cGlobalParas.VersionType.Program ||
                    Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
                {
                    this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit32"));
                    this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit53"));
                }

                if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                    Program.SominerVersion == cGlobalParas.VersionType.Program ||
                    Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                    Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
                {
                    this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit40"));
                    
                }
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit47"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit48"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit49"));

                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit51"));

               
            }
        }

        private void raEncoding_CheckedChanged(object sender, EventArgs e)
        {
            if (raEncoding.Checked == true)
            {
                this.ExportRuleType.Items.Clear();

                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit13"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit14"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit25"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit39"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit33"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit45"));

                if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                    Program.SominerVersion == cGlobalParas.VersionType.Program ||
                    Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                    Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
                {
                    this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit50"));
                    this.ExportRuleType.Items.Add (rmPara.GetString("ExportLimit55"));
                }
                
            }
        }

        private void raCondition_CheckedChanged(object sender, EventArgs e)
        {
            if (raCondition.Checked == true)
            {
                this.ExportRuleType.Items.Clear();

                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit10"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit11"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit12"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit34"));

             
            }
        }

        private void raDataEdit_CheckedChanged(object sender, EventArgs e)
        {
            if (raDataEdit.Checked == true)
            {
                this.ExportRuleType.Items.Clear();

                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit18"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit19"));

                if (Program.SominerVersion != cGlobalParas.VersionType.Free )
                {
                    this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit46"));
                }

            }
        }

        private void raDataGather_CheckedChanged(object sender, EventArgs e)
        {
            if (raDataGather.Checked == true)
            {
                this.ExportRuleType.Items.Clear();

                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit22"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit23"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit24"));
                

            }
        }

        private void raValue_CheckedChanged(object sender, EventArgs e)
        {
            if (raValue.Checked == true)
            {
                this.ExportRuleType.Items.Clear();

                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit17"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit26"));
                if (Program.SominerVersion != cGlobalParas.VersionType.Free)
                {
                    this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit56"));
                }
            }
        }

        private void raOther_CheckedChanged(object sender, EventArgs e)
        {
            if (raOther.Checked == true)
            {
                this.ExportRuleType.Items.Clear();

                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit27"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit28"));

            }
        }

        private void ExportRuleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetEnabled(this.ExportRuleType.Text );
            SetComItems(this.ExportRuleType.Text);
        }

        private void SetEnabled(string str)
        {
           
            cGlobalParas.ExportLimit eLimit = EnumUtil.GetEnumName<cGlobalParas.ExportLimit>(str);
         
            switch (eLimit)
            {
                case cGlobalParas.ExportLimit.ExportNoLimit:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text  = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportDelData:
                    this.txtRule.Enabled = true;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "关键词：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportInclude:
                    this.txtRule.Enabled = true;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "关键词：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportNoWebSign:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportPrefix:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "前缀字符：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportReplace:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = true ;
                    this.txtNewValue.Enabled = true ;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportSuffix:
                   this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "后缀字符：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportTrimLeft:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "删除字符数：";
                    this.butSetWatermark.Enabled = false;
                    this.butInsertPara.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportTrimRight:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "删除字符数：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportTrim:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportRegexReplace:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = true ;
                    this.txtNewValue.Enabled = true ;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportSetEmpty:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "关键词：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportConvertUnicode:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportConvertHtml:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportHaveCRLF:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportReplaceCRLF:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportWrap:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "标识字符：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportGatherdata:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "正则表达式：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportFormatString:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.label4.Text = "格式化规则：";
                    this.txtRule.Text = "";
                    this.butInsertPara.Visible = true;
                    this.butInsertPara.Enabled = true ;
                    this.butSetWatermark.Visible = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportDownloadFilePath:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case  cGlobalParas.ExportLimit.ExportGatherUrl:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "正则表达式：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportGather:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "采集数据名称：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportMakeGather :
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "组合表达式：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportEncodingString :
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "编码：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportEncoding:
                    this.txtRule.Enabled = true;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "编码：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportValue :
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "固定值：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportAutoCode :
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "起始值：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportSynonymsReplace :
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = true ;
                    this.txtRule.Visible = false;
                    this.txtRule.Text = "";
                    this.label4.Text = "词库：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportMergeParagraphs:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true ;
                    this.txtRule.Text = "";
                    this.label4.Text = "字数小于：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportRenameDownloadFile :
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false ;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "重命名规则：";
                    this.butInsertPara.Visible = true;
                    this.butInsertPara.Enabled = true;
                    this.butSetWatermark.Visible = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportPY:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportDict:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false ;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "字典文件：";
                    this.butInsertPara.Enabled = false ;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportBase64:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false ;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "编码类型：";
                    this.butInsertPara.Enabled = false ;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportNumber:
                    this.txtRule.Enabled = true;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;

                case cGlobalParas.ExportLimit.ExportImgReplace:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = true;
                    this.txtNewValue.Enabled = true;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportToAbsoluteUrl:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportNoRepeat :
                    this.txtRule.Enabled = false ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportFormatXML:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportWatermark:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Visible = false;
                    this.butInsertPara.Enabled = false ;
                    this.butSetWatermark.Visible = true;
                    this.butSetWatermark.Enabled = true ;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                //case cGlobalParas.ExportLimit.ExportOCR:
                //    this.txtRule.Enabled = true;
                //    this.txtOldValue.Enabled = false;
                //    this.txtNewValue.Enabled = false;
                //    this.comSynDb.Visible = false;
                //    this.txtRule.Visible = true;
                //    this.txtRule.Text = "";
                //    this.label4.Text = "识别精度：";
                //    this.butInsertPara.Enabled = false;
                //    this.butInsertPara.Visible = false;
                //    this.butSetWatermark.Enabled = false ;
                //    this.butSetWatermark.Visible = false;
                //    this.numOcrScale.Visible = true;
                //    this.butSetOCR.Visible = true;
                //    this.butSetOCR.Enabled = true ;
                //    this.isOcrNumber.Visible = true;
                //    break;
                case cGlobalParas.ExportLimit.ExportRepeatNameDeal:
                    this.txtRule.Enabled = true;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = true ;
                    this.txtRule.Visible = false ;
                    this.txtRule.Text = "";
                    this.label4.Text = "处理规则：";
                    this.butInsertPara.Enabled = false;
                    this.butInsertPara.Visible = false;
                    this.butSetWatermark.Enabled = false;
                    this.butSetWatermark.Visible = false;
                    this.numOcrScale.Visible = false ;
                    this.butSetOCR.Visible = true  ;
                    this.butSetOCR.Enabled = false   ;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportHavePImgNoneCSS:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportBase64Encoding:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false ;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "编码类型：";
                    this.butInsertPara.Enabled = false ;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportConvertDateTime:
                    this.txtRule.Enabled = false ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false ;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false ;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportSubstring:
                    this.txtRule.Enabled = true ;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false ;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "字符数：";
                    this.butInsertPara.Enabled = false ;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportToLower:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text  = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportToUpper:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text  = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportToMD5:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportJsonToObject:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                case cGlobalParas.ExportLimit.ExportUUID:
                    this.txtRule.Enabled = false;
                    this.txtOldValue.Enabled = false;
                    this.txtNewValue.Enabled = false;
                    this.comSynDb.Visible = false;
                    this.txtRule.Visible = true;
                    this.txtRule.Text = "";
                    this.label4.Text = "规则：";
                    this.butInsertPara.Enabled = false;
                    this.butSetWatermark.Enabled = false;
                    this.numOcrScale.Visible = false;
                    this.butSetOCR.Visible = false;
                    this.butSetOCR.Enabled = false;
                    this.isOcrNumber.Visible = false;
                    break;
                default :
                    if (this.raPlugin.Checked == true)
                    {
                        this.txtRule.Enabled = false;
                        this.txtOldValue.Enabled = false;
                        this.txtNewValue.Enabled = false;
                        this.comSynDb.Visible = false;
                        this.txtRule.Visible = true;
                        this.txtRule.Text = "";
                        this.label4.Text = "规则：";
                        this.butInsertPara.Enabled = false;
                        string ss = this.ExportRuleType.SelectedItem.ToString();
                        this.txtRule.Text = ss.Substring(ss.IndexOf("-") + 1, ss.Length - ss.IndexOf("-") - 1);
                        this.numOcrScale.Visible = false;
                        this.butSetWatermark.Enabled = false;
                        this.butSetOCR.Visible = false;
                        this.butSetOCR.Enabled = false;

                    }
                    else
                    {
                        this.comSynDb.Visible = false;
                        this.txtRule.Visible = true;
                        this.txtRule.Text = "";
                        this.butInsertPara.Enabled = false;
                        this.numOcrScale.Visible = false;
                        this.butSetWatermark.Enabled = false;
                        this.butSetOCR.Visible = false;
                        this.butSetOCR.Enabled = false;
                    }
                    this.isOcrNumber.Visible = false;
                    break;
            }
           
        }

        private void SetComItems(string str)
        {
            cGlobalParas.ExportLimit eLimit =EnumUtil.GetEnumName<cGlobalParas.ExportLimit>(str);

            switch (eLimit)
            {
                case cGlobalParas.ExportLimit.ExportSynonymsReplace:
                    cSynDb sDb = new cSynDb();
                    int count = sDb.GetSynCount();

                    this.comSynDb.Items.Clear();
                    for (int i = 0; i < count; i++)
                    {
                        if (sDb.GetSynType(i)==cGlobalParas.SynonymType.Custom )
                            this.comSynDb.Items.Add(sDb.GetSynName(i));
                    }

                    sDb = null;
                    break;
                case cGlobalParas.ExportLimit.ExportRepeatNameDeal:
                    this.comSynDb.Items.Clear();
                    this.comSynDb.Items.Add(rmPara.GetString("DownloadFileDealType1"));
                    this.comSynDb.Items.Add(rmPara.GetString("DownloadFileDealType2"));
                    this.comSynDb.Items.Add(rmPara.GetString("DownloadFileDealType3"));
                    break;
                default :
                    this.comSynDb.Items.Clear();
                    break;
            }
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            if (this.ExportRuleType.SelectedIndex ==-1)
            {
                MessageBox.Show("请选择数据加工的规则！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string strRule = "";

            cGlobalParas.ExportLimit eLimit =EnumUtil.GetEnumName<cGlobalParas.ExportLimit>(this.ExportRuleType.Text);

            switch (eLimit)
            {
                case cGlobalParas.ExportLimit.ExportNoLimit:
                    strRule = "";
                    break;
                case cGlobalParas.ExportLimit.ExportDelData:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportInclude:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportNoWebSign:
                    strRule = "";
                    break;
                case cGlobalParas.ExportLimit.ExportPrefix:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportReplace:
                    strRule = "<OldValue:" + this.txtOldValue.Text  + "><NewValue:" + this.txtNewValue.Text + ">";
                    break;
                case cGlobalParas.ExportLimit.ExportSuffix:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportTrimLeft:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportTrimRight:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportTrim:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportRegexReplace:
                    strRule = "<OldValue:" + this.txtOldValue.Text  + "><NewValue:" + this.txtNewValue.Text + ">";
                    break;
                case cGlobalParas.ExportLimit.ExportSetEmpty:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportConvertUnicode:
                    strRule = "";
                    break;
                case cGlobalParas.ExportLimit.ExportConvertHtml:
                    strRule = "";
                    break;
                case cGlobalParas.ExportLimit.ExportHaveCRLF:
                    strRule = "";
                    break;
                case cGlobalParas.ExportLimit.ExportReplaceCRLF:
                    strRule = "";
                    break;
                case cGlobalParas.ExportLimit.ExportWrap:
                    strRule ="<Wrap:" + this.txtRule.Text + ">" ;
                    break;
                case cGlobalParas.ExportLimit.ExportGatherdata:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportFormatString:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportDownloadFilePath:
                    strRule = "";
                    break;
                case cGlobalParas.ExportLimit.ExportGatherUrl:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportGather:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportMakeGather:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportEncodingString:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportValue:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportAutoCode:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportSynonymsReplace :
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportMergeParagraphs:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportRenameDownloadFile :
                     strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportDict :
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportImgReplace:
                    strRule = "<OldValue:" + this.txtOldValue.Text + "><NewValue:" + this.txtNewValue.Text + ">";
                    break;
                case cGlobalParas.ExportLimit.ExportEncoding:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportFormatXML:
                    strRule = "";
                    break;
                case cGlobalParas.ExportLimit.ExportWatermark:
                    strRule = this.txtRule.Text;
                    break;
                //case cGlobalParas.ExportLimit.ExportOCR:
                //    strRule = "<Precision>" + this.txtRule.Text + "</Precision><IsNumber>" + this.isOcrNumber.Checked.ToString() + "</IsNumber>";
                //    break;
                case cGlobalParas.ExportLimit.ExportSubstring:
                    strRule = this.txtRule.Text;
                    break;
                case cGlobalParas.ExportLimit.ExportToLower:
                    strRule = "";
                    break;
                case cGlobalParas.ExportLimit.ExportToUpper:
                    strRule = "";
                    break;
                case cGlobalParas.ExportLimit.ExportToMD5:
                    strRule = "";
                    break;
                default :
                    strRule = this.txtRule.Text;
                    break;
            }

            if (this.raPlugin.Checked == true)
                rERule(rmPara.GetString("ExportLimit31"), strRule);
            else
                rERule(this.ExportRuleType.Text, strRule);

            this.Close();
        }

        private void comSynDb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(this.comSynDb.SelectedItem !=null)
                this.txtRule.Text = this.comSynDb.SelectedItem.ToString();
        }

        private void butInsertPara_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Show(this.butInsertPara, 0, 21);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.ExportRuleType.SelectedIndex ==-1)
            {
                MessageBox.Show("请选择数据加工的规则！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string strRule = "";

            cGlobalParas.ExportLimit eLimit = EnumUtil.GetEnumName<cGlobalParas.ExportLimit>(this.ExportRuleType.Text);
            this.contextMenuStrip1.Items.Clear();

            switch (eLimit)
            {
                case cGlobalParas.ExportLimit.ExportFormatString:
                    
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("格式化数字", null, null, "rmenuFormatNum"));
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("格式化货币", null, null, "rmenuFormatMoney"));
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("格式化日期", null, null, "rmenuFormatDate"));
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("格式化百分比", null, null, "rmenuFormatPer"));
                    this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("小提示：自定义格式化请通过手工输入格式化规则", 
                        null, null, "rmenuFormatDEMO"));
                    this.contextMenuStrip1.Items["rmenuFormatDEMO"].Enabled = false;

                    break;
                case cGlobalParas.ExportLimit.ExportRenameDownloadFile:
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("重命名参数:源文件名", null, null, "rmenuRenameOldName"));
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("重命名参数:当前网页标题", null, null, "rmenuRenameTitle"));
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("重命名参数:当前采集任务名称", null, null, "rmenuRenameTaskName"));
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("重命名参数:当前日期", null, null, "rmenuRenameDateTime"));
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("重命名参数:自动编号", null, null, "rmenuRenameAutoID"));
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("重命名参数:正则获取", null, null, "rmenuRenameRegex"));
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("重命名参数:采集数据规则名称", null, null, "rmenuRenameDataRule"));

                    this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                    this.contextMenuStrip1.Items.Add(new ToolStripMenuItem("小提示：可输入固定值",
                        null, null, "rmenuRenameDEMO"));
                    this.contextMenuStrip1.Items["rmenuRenameDEMO"].Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int startPos = this.txtRule.SelectionStart;
            int l = this.txtRule.SelectionLength;

            switch (e.ClickedItem.Name)
            {
                case "rmenuFormatNum":
                    this.txtRule.Text =  "{0:###0.00}" ;
                    break;
                case "rmenuFormatMoney":
                    this.txtRule.Text = "{0:C}" ;
                    break;
                case "rmenuFormatDate":
                    this.txtRule.Text =  "{0:yyyy-MM-dd}";
                    break;
                case "rmenuFormatPer":
                    this.txtRule.Text =  "{0:#%}";
                    break;
                case "rmenuRenameOldName":
                    this.txtRule.Text = this.txtRule.Text.Substring(0, startPos) + "{Rename:OldName}" + this.txtRule.Text.Substring(startPos + l, this.txtRule.Text.Length - startPos - l);
                    this.txtRule.SelectionStart = startPos + "{Rename:OldName}".Length;
                    break;
                case "rmenuRenameTitle":
                    this.txtRule.Text = this.txtRule.Text.Substring(0, startPos) + "{Rename:Title}" + this.txtRule.Text.Substring(startPos + l, this.txtRule.Text.Length - startPos - l);
                    this.txtRule.SelectionStart = startPos + "{Rename:Title}".Length;
                    break;
                case "rmenuRenameTaskName":
                    this.txtRule.Text = this.txtRule.Text.Substring(0, startPos) + this.m_TaskName + this.txtRule.Text.Substring(startPos + l, this.txtRule.Text.Length - startPos - l);
                    this.txtRule.SelectionStart = startPos + this.m_TaskName.Length;
                    break;
                case "rmenuRenameDateTime":
                    this.txtRule.Text = this.txtRule.Text.Substring(0, startPos) + "{Rename:CurrentDate}" + this.txtRule.Text.Substring(startPos + l, this.txtRule.Text.Length - startPos - l);
                    this.txtRule.SelectionStart = startPos + "{Rename:CurrentDate}".Length;
                    break;
                case "rmenuRenameAutoID":
                    this.txtRule.Text = this.txtRule.Text.Substring(0, startPos) + "{Rename:AutoID}" + this.txtRule.Text.Substring(startPos + l, this.txtRule.Text.Length - startPos - l);
                    this.txtRule.SelectionStart = startPos + "{Rename:AutoID}".Length;
                    break;
                case "rmenuRenameRegex":
                    //this.txtRule.Text = this.txtRule.Text.Substring(0, startPos) + "{RenameRegex:}" + this.txtRule.Text.Substring(startPos + l, this.txtRule.Text.Length - startPos - l);
                    //this.txtRule.SelectionStart = startPos + "{RenameRegex:}".Length;

                    frmInput f = new frmInput(true, "{RenameRegex:}");
                    f.Text = "请输入正则表达式";
                    f.label1.Text = "请输入正则表达式（根据正则从网页中获取）:";
                    f.rValue = this.GetValue;
                    f.ShowDialog();
                    f.Dispose();
                    break;
                case "rmenuRenameDataRule":
                    frmInput f1 = new frmInput(true, "{RenameDataRule:}");
                    f1.Text = "请输入采集数据规则的名称";
                    f1.label1.Text = "请输入采集数据规则的名称:";
                    f1.rValue = this.GetValue;
                    f1.ShowDialog();
                    f1.Dispose();
                    break;

            }

            this.txtRule.ScrollToCaret();

        }

        private void GetValue(string strValue)
        {
            int startPos = this.txtRule.SelectionStart;
            int l = this.txtRule.SelectionLength;

            this.txtRule.Text = this.txtRule.Text.Substring(0, startPos) + strValue + this.txtRule.Text.Substring(startPos + l, this.txtRule.Text.Length - startPos - l);
            this.txtRule.SelectionStart = startPos + strValue.Length;

        }



        private void raPlugin_CheckedChanged(object sender, EventArgs e)
        {
            if (raPlugin.Checked == true)
            {
                //加载插件
                this.ExportRuleType.Items.Clear();

                NetMiner.Core.Plugin.cPlugin p = new NetMiner.Core.Plugin.cPlugin(Program.getPrjPath());
                int count = p.GetCount();
                for (int i = 0; i < count; i++)
                {
                    if (cGlobalParas.PluginsType.DealData==p.GetPluginType(i))
                        this.ExportRuleType.Items.Add(p.GetPluginName(i) + "                                                    -" + 
                            p.GetPlugin (i));
                }
            }
        }

        private void raDBRule_CheckedChanged(object sender, EventArgs e)
        {
            if (raDBRule.Checked == true)
            {
                //加载插件
                this.ExportRuleType.Items.Clear();

                if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                    Program.SominerVersion == cGlobalParas.VersionType.Program ||
                    Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                    Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
                {
                    this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit38"));
                }
              
            }
        }

        private void raWatermark_CheckedChanged(object sender, EventArgs e)
        {
            if (raWatermark.Checked == true)
            {
                this.ExportRuleType.Items.Clear();

                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit41"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit42"));

                this.txtRule.Enabled = true ;
                this.txtOldValue.Enabled = false;
                this.txtNewValue.Enabled = false;
            }
        }

        private void butSetWatermark_Click(object sender, EventArgs e)
        {
            frmSetWatermark f = new frmSetWatermark();
            f.rWatermark = this.GetWatermark;
            f.ShowDialog();
            f.Dispose();
        }

        private void GetWatermark(string str)
        {
            this.txtRule.Text = str;
        }

        private void butSetOCR_Click(object sender, EventArgs e)
        {
            //frmOCR f = new frmOCR();
            //f.ShowDialog();
            //f.Dispose();
        }

        private void raDownload_CheckedChanged(object sender, EventArgs e)
        {
            if (raDownload.Checked == true)
            {
                this.ExportRuleType.Items.Clear();

                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit21"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit29"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit43"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit36"));

                if (Program.SominerVersion != cGlobalParas.VersionType.Free)
                {
                    this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit52"));
                    //this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit57"));
                }
                }
        }

    }
}

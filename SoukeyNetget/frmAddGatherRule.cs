using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using NetMiner.Gather;
using NetMiner.Resource;
using NetMiner.Gather.Task;
using System.IO;
using NetMiner.Core.gTask.Entity;

namespace MinerSpider
{
    public partial class frmAddGatherRule : Form
    {
        //定义一个访问资源文件的变量
        private ResourceManager rm;
        //定义一个ToolTip
        ToolTip HelpTip = new ToolTip();

        //private List<string> m_NaviLevel;
        //private List<string> m_MultiPage;
        private List<string> m_gNames;

        public delegate void ReturnGatherRule(cGatherRule gRule);
        public ReturnGatherRule rGatherRule;

        private cGlobalParas.FormState m_fState;
        public cGlobalParas.FormState fState
        {
            get { return m_fState; }
            set { m_fState = value; }
        }

        private string m_TestUrl;
        public string TestUrl
        {
            get { return m_TestUrl; }
            set { m_TestUrl = value; }
        }

        private string m_cookie;
        public string Cookie
        {
            get { return m_cookie; }
            set { m_cookie = value; }
        }

        private cGlobalParas.WebCode m_wCode;
        public cGlobalParas.WebCode wCode
        {
            get { return m_wCode; }
            set { m_wCode = value; }
        }

        private string m_TName;

        public frmAddGatherRule(string tName,string testUrl)
        {
            InitializeComponent();

            m_TestUrl = testUrl;
            m_TName = tName;
            m_gNames = new List<string>();

            IniData();
        }

        public void IniPage(List<string> gNames, List<string> NaviLeve, List<string> MultiPage)
        {
            m_gNames = gNames;

            for (int i = 0; i < NaviLeve.Count; i++)
            {
                this.comNavLevel.Items.Add(NaviLeve[i].ToString());
            }

            for (int i = 0; i < MultiPage.Count; i++)
            {
                this.comMultiPage.Items.Add(MultiPage[i].ToString());
            }

            if (this.fState == cGlobalParas.FormState.Edit)
            {
                //this.txtGetTitleName.Enabled = false;
            }
        }

        //string gName, cGlobalParas.GatherRuleByPage gRuleType,
        //    string gType, string getStart, string getEnd, string limitType, string strReg,
        //    string IsMData, string NLevel, string MPageName,
        //    string sPath, string fileDealType,string IsOcr
        public void IniPage(List<string> gNames, List<string> NaviLeve, List<string> MultiPage,cGatherRule gRule)
        {
            m_gNames = gNames;
            for (int i = 0; i < NaviLeve.Count; i++)
            {
                this.comNavLevel.Items.Add(NaviLeve[i].ToString());
            }

            for (int i = 0; i < MultiPage.Count; i++)
            {
                this.comMultiPage.Items.Add(MultiPage[i].ToString());
            }

            if (this.fState == cGlobalParas.FormState.Edit)
            {
                //this.txtGetTitleName.Enabled = false;
            }

            //填充数据
            this.txtGetTitleName.Text = gRule.gName;
            switch (gRule.gRuleByPage)
            {
                case  cGlobalParas.GatherRuleByPage.GatherPage :
                    this.raPage.Checked = true;
                    break;
                case cGlobalParas.GatherRuleByPage.NaviPage :
                    this.raNavPage.Checked = true;
                    break;
                case cGlobalParas.GatherRuleByPage.MultiPage :
                    this.raMultiPage.Checked = true;
                    break;
                //case cGlobalParas.GatherRuleByPage.NonePage :
                //    this.raNonePage.Checked = true;
                //    break;
            }

            if (gRule.gRuleType == cGlobalParas.GatherRuleType.Normal)
                this.raGather.Checked = true;
            else if (gRule.gRuleType == cGlobalParas.GatherRuleType.XPath)
                this.raXPath.Checked = true;
            else if (gRule.gRuleType == cGlobalParas.GatherRuleType.Smart)
                this.raSmart.Checked = true;
            else if (gRule.gRuleType == cGlobalParas.GatherRuleType.NonePage)
                this.raNonePage.Checked = true;

            this.comGetType.Text = gRule.gType;
            this.txtGetStart.Text = gRule.getStart;
            this.txtGetEnd.Text = gRule.getEnd;
            this.comLimit.Text = gRule.limitType;
            this.txtRegion.Text = gRule.strReg;
            this.IsMergeData.Checked = gRule.IsMergeData;
            this.comNavLevel.Text = gRule.NaviLevel;
            this.comMultiPage.Text = gRule.MultiPageName ;
            this.txtSaveFilePath.Text = gRule.sPath;
            //this.comFileDeal.Text = gRule.fileDealType;
            this.txtXPath.Text = gRule.xPath;
            this.comHNodeTextType.Text = gRule.gNodePrty;
            this.IsAutoDownloadFileImage.Checked = gRule.IsAutoDownloadFileImage;
            this.IsAutoDownloadOnlyImage.Checked = gRule.IsAutoDownloadOnlyImage;
            SetIsAutoDownloadImage();

            //加载数据加工规则
            for (int i = 0; i < gRule.fieldDealRules.FieldRule.Count; i++)
            {
                this.dataDataRules.Rows.Add();
                this.dataDataRules.Rows[this.dataDataRules.Rows.Count - 2].Cells[1].Value = 
                    gRule.fieldDealRules.FieldRule[i].FieldRuleType.GetDescription();
                this.dataDataRules.Rows[this.dataDataRules.Rows.Count - 2].Cells[2].Value = gRule.fieldDealRules.FieldRule[i].FieldRule;
            }
        }

        private void frmAddGatherRule_Load(object sender, EventArgs e)
        {
            //对Tooltip进行初始化设置
            HelpTip.ToolTipIcon = ToolTipIcon.Info;
            HelpTip.ForeColor = Color.YellowGreen;
            HelpTip.BackColor = Color.LightGray;
            HelpTip.AutoPopDelay = 20000;
            HelpTip.ShowAlways = true;
            HelpTip.ToolTipTitle = "小矿提示";

            SetHelpTip();

            if (Program.SominerVersion == cGlobalParas.VersionType.Free)
            {
                this.raNavPage.Enabled = false;
                this.raMultiPage.Enabled = false;
            }


            this.txtGetTitleName.Focus();

        }

        private void SetHelpTip()
        {
            HelpTip.SetToolTip(this.txtGetTitleName, @"输入采集数据项规则名称，不能以数字打头，不能包含?!@\^/%等字符");
            HelpTip.SetToolTip(this.raNonePage, "选中此选项，则不从网页采集数据，由用户自定义数据输出");
            HelpTip.SetToolTip(this.raPage, "选择此选项，从采集页中采集数据，属于默认采集的方式");
            HelpTip.SetToolTip(this.raNavPage, "选择此选项，则从导航页采集数据，同时需要在采集网址设置中设置导航页采集标记，" + "\r\n" + @"此时配置规则方可有效");
            HelpTip.SetToolTip(this.comNavLevel, "在此选择采集哪个导航页的数据，此级别与导航的级别对应，" + "\r\n" + @"同时导航页中需选中导航页采集选项");
            HelpTip.SetToolTip(this.raMultiPage, "选择此选项，则从多页配置获取的页面中采集数据，需要选择多页配置的名称，" + "\r\n" + @"以确定是从哪个页面采集数据");
            HelpTip.SetToolTip(this.comMultiPage, "在此选择多页配置的名称，从而确定是从哪个多页配置获取的页面中采集数据");
            HelpTip.SetToolTip(this.raGather, "选中此选项，则通过配置前置和后置标记来捕获数据");
            HelpTip.SetToolTip(this.raXPath, "选中此选项，则通过可视化的方式捕获数据，选择“可视化提取”按钮打开可视化捕获界面，进行配置");
            HelpTip.SetToolTip(this.raSmart, "选中此选项，则由系统自动提取文章的正文、标题及实现，" + "\r\n" + @"请通过下面的“采集数据类型”来选择自动捕获的内容");
            HelpTip.SetToolTip(this.comGetType, "通过此选项来选择采集数据的类型：文本则直接输出文字，图片、文件则直接下载文件，" + "\r\n" + @"智能提取，则为系统自动提取数据，但需选择上方的智能采集");
            HelpTip.SetToolTip(this.txtSaveFilePath, "如果当前选择了下载图片或文件，在此可以指定文件保存的路径，如果不输入，则系统默认保存在Data目录下");
            HelpTip.SetToolTip(this.butOpenPath, "点击此按钮，指定文件下载存储路径");
            HelpTip.SetToolTip(this.comHNodeTextType, "此选项仅对可视化配置有效，用于获取可视化数据节点的数据范围");
            HelpTip.SetToolTip(this.txtXPath, "在此输入可视化获取的XPath路径地址");
            HelpTip.SetToolTip(this.cmdVisual, "点击此按钮打开可视化捕获工具，来自动获取可视化捕获的XPath地址信息");
            HelpTip.SetToolTip(this.txtGetStart, "如果是常规配置，在此输入捕获数据的前置字符串，建议为一个完整的标签，" + "\r\n" + @"如果标签内部有变化字符，可点击右侧按钮使用通配符替代");
            HelpTip.SetToolTip(this.txtGetEnd, "如果是常规配置，在此输入捕获数据的后置字符串，建议为一个完整的标签，" + "\r\n" + @"如果标签内部有变化字符，可点击右侧按钮使用通配符替代");
            HelpTip.SetToolTip(this.cmdStartWildcard, "点击此按钮，选择通配符类型，插入通配符");
            HelpTip.SetToolTip(this.cmdEndWildcard, "点击此按钮，选择通配符类型，插入通配符");
            HelpTip.SetToolTip(this.IsAutoDownloadFileImage, "选中此选项，则系统会自动判断当前规则捕获的数据中是否包含图片，如果包含，则自动下载");
            HelpTip.SetToolTip(this.IsAutoDownloadOnlyImage, "选中此选项，仅会自动下载第一张图片，发现的其他图片不进行下载");
            HelpTip.SetToolTip(this.comLimit, "如果选择了常规配置，通过此选项可以选择匹配数据的类型，也可以通过此选择来自定义正则来捕获数据");
            HelpTip.SetToolTip(this.txtRegion, "如果左侧选项选择了“自定义正则匹配表达式”，则在此输入正则进行数据捕获");
            HelpTip.SetToolTip(this.IsMergeData, "选中此选项，则系统会自动根据连续翻页采集的数据进行合并操作，但前提是必须配置了翻页规则，否则此选项无效");

        }

        private void IniData()
        {
            iniCommonName();

            //初始化资源文件的参数
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            //根据当前的区域进行显示信息的加载
            ResourceManager rmPara = new ResourceManager("NetMiner.Resource.Resources.globalPara", Assembly.Load("NetMiner.Resource"));


            this.comLimit.Items.Add(rmPara.GetString("LimitSign1"));
            this.comLimit.Items.Add(rmPara.GetString("LimitSign2"));
            this.comLimit.Items.Add(rmPara.GetString("LimitSign3"));
            this.comLimit.Items.Add(rmPara.GetString("LimitSign4"));
            this.comLimit.Items.Add(rmPara.GetString("LimitSign5"));
            this.comLimit.Items.Add(rmPara.GetString("LimitSign6"));
            this.comLimit.Items.Add(rmPara.GetString("LimitSign7"));
            this.comLimit.Items.Add(rmPara.GetString("LimitSign8"));
            //this.comLimit.Items.Add(rmPara.GetString("LimitSign9"));
            //this.comLimit.Items.Add(rmPara.GetString("LimitSign10"));
            //this.comLimit.Items.Add(rmPara.GetString("LimitSign11"));
            //this.comLimit.Items.Add(rmPara.GetString("LimitSign12"));
            this.comLimit.SelectedIndex = 0;

            this.comGetType.Items.Add(rmPara.GetString("GDataType4"));
            this.comGetType.Items.Add(rmPara.GetString("GDataType3"));
            this.comGetType.Items.Add(rmPara.GetString("GDataType1"));
            this.comGetType.Items.Add(rmPara.GetString("GDataType8"));
            this.comGetType.SelectedIndex = 0;
            


            this.comHNodeTextType.Items.Add("InnerHtml");
            this.comHNodeTextType.Items.Add("InnerText");
            this.comHNodeTextType.Items.Add("OuterHtml");

            //this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit1"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit2"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit3"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit4"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit5"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit6"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit7"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit8"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit9"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit10"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit11"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit12"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit13"));

            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit34"));

            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit14"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit15"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit16"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit44"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit17"));
            if (Program.SominerVersion != cGlobalParas.VersionType.Free)
            {
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit19"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit18"));
            }

            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit20"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit21"));

            if (Program.SominerVersion != cGlobalParas.VersionType.Free)
            {
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit22"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit23"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit24"));
            }

            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit25"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit39"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit26"));
            if (Program.SominerVersion != cGlobalParas.VersionType.Free)
            {
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit56"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit27"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit28"));
            }
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit29"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit30"));

            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit31"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit40"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit50"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit55"));
            }
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit32"));
                
            }

            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit33"));

            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit45"));

            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit35"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit36"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit37"));

            if (Program.SominerVersion != cGlobalParas.VersionType.Free)
            {
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit38"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit42"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit46"));
                this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit53"));
            }

            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit41"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit43"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit47"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit48"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit49"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit51"));
            this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit52"));

            if (Program.SominerVersion != cGlobalParas.VersionType.Free)
            {
                //this.ExportRuleType.Items.Add(rmPara.GetString("ExportLimit57"));
            }
            rmPara = null;
            
        }

        private void raPage_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raPage.Checked == true)
            {
                this.comNavLevel.Enabled = false;
                this.comMultiPage.Enabled = false;

                SetGroup2(true);
            }
        }

        private void raNavPage_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raNavPage.Checked == true)
            {
                this.comNavLevel.Enabled = true;
                //this.comNavLevel.Items.Clear();
                this.comMultiPage.Enabled = false;


                SetGroup2(true);

                //计算导航级别
                if (this.comNavLevel.Items.Count ==0)
                {
                    return;
                }

                //int MaxLevel = 0;

                ////取导航深度最深的网址
                //for (int i = 0; i < this.comNavLevel.Items.Count; i++)
                //{
                //    if (MaxLevel < int.Parse(this.comNavLevel.Items[i].ToString()))
                //    {
                //        MaxLevel = int.Parse(this.comNavLevel.Items[i].ToString());
                //    }
                //}

                //if (MaxLevel == 0)
                //    return;

                //for (int j = 1; j <= MaxLevel; j++)
                //{
                //    this.comNavLevel.Items.Add(j.ToString());
                //}

                if (this.comNavLevel.Items.Count != 0)
                {
                    this.comNavLevel.SelectedIndex = 0;
                }
                
            }

        }

        private void raMultiPage_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raMultiPage.Checked == true)
            {
                this.comNavLevel.Enabled = false;
                this.comMultiPage.Enabled = true;

                SetGroup2(true);

               

                if (this.comMultiPage.Items.Count != 0)
                {
                    this.comMultiPage.SelectedIndex = 0;
                }
                
            }
        }

        private void comGetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.raSmart.Checked == false)
            {
                if (this.comGetType.SelectedIndex == 0)
                {
                    if (this.raNonePage.Checked == false)
                    {
                        this.comLimit.Enabled = true;
                        this.comLimit.SelectedIndex = 0;
                        this.IsAutoDownloadFileImage.Enabled = true;
                    }
                    else
                    {
                        this.comLimit.Enabled = false;
                        this.comLimit.SelectedIndex = 0;
                        this.IsAutoDownloadFileImage.Enabled = false;
                    }

                    this.txtSaveFilePath.Text = "";
                    this.txtSaveFilePath.Enabled = false;
                    this.label57.Enabled = false;
                    this.butOpenPath.Enabled = false;
                }
                else if (this.comGetType.SelectedIndex == 1)
                {
                    this.comLimit.SelectedIndex = 0;

                    if (this.raNonePage.Checked == false)
                    {
                        this.comLimit.Enabled = true;
                    }
                    else
                    {
                        this.comLimit.Enabled = false;
                    }

                    this.txtSaveFilePath.Enabled = true;
                    this.label57.Enabled = true;
                    this.butOpenPath.Enabled = true;
                    this.IsAutoDownloadFileImage.Enabled = false;
                }
                else if (this.comGetType.SelectedIndex == 2)
                {
                    this.comLimit.SelectedIndex = 0;

                    if (this.raNonePage.Checked == false)
                    {
                        this.comLimit.Enabled = true;
                    }
                    else
                    {
                        this.comLimit.Enabled = false;
                    }

                    this.txtSaveFilePath.Enabled = true;
                    this.label57.Enabled = true;
                    this.butOpenPath.Enabled = true;
                    this.IsAutoDownloadFileImage.Enabled = false;
                }
                else if (this.comGetType.SelectedIndex == 3 ||
                        this.comGetType.SelectedIndex == 4 ||
                        this.comGetType.SelectedIndex == 5 ||
                        this.comGetType.SelectedIndex == 6 ||
                        this.comGetType.SelectedIndex == 7)
                {

                    this.comLimit.SelectedIndex = 0;
                    this.comLimit.Enabled = false;

                    if (this.IsAutoDownloadFileImage.Checked == true)
                    {
                        this.txtSaveFilePath.Enabled = true;
                        this.label57.Enabled = true;
                        this.butOpenPath.Enabled = true;
                    }
                    else
                    {
                        this.txtSaveFilePath.Enabled = false;
                        this.label57.Enabled = false;
                        this.butOpenPath.Enabled = false;
                    }

                    this.IsAutoDownloadFileImage.Enabled = true;

                }
                else if (this.comGetType.SelectedIndex == 8)
                {
                    this.comLimit.SelectedIndex = 0;
                    this.comLimit.Enabled = false;

                    this.txtSaveFilePath.Enabled = true;
                    this.label57.Enabled = true;
                    this.butOpenPath.Enabled = true;
                    this.IsAutoDownloadFileImage.Enabled = false;

                }
            }
            
        }

        private void comLimit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comLimit.SelectedIndex < 6)
            {
                this.txtRegion.Enabled = false;
                this.txtGetStart.Enabled = true;
                this.txtGetEnd.Enabled = true;
            }
            else
            {
                switch (this.comLimit.SelectedIndex)
                {
                    case 6:
                        //自定义匹配字符
                        this.txtRegion.Enabled = true;
                        this.txtGetStart.Enabled = true;
                        this.txtGetEnd.Enabled = true;
                        this.txtRegion.Text = "[^>]";
                        break;
                    case 7:
                        //自定义正则
                        this.txtRegion.Enabled = true;
                        this.txtGetStart.Enabled = false;
                        this.txtGetEnd.Enabled = false;

                        this.txtGetStart.Text = "";
                        this.txtGetEnd.Text = "";

                        this.txtRegion.Text = "";
                        break;
                    case 8:
                        //文章标题
                        this.txtRegion.Enabled = false;
                        this.txtGetStart.Enabled = false;
                        this.txtGetEnd.Enabled = false;

                        this.txtGetStart.Text = "";
                        this.txtGetEnd.Text = "";

                        this.txtRegion.Text = "";
                        break;
                    case 9:
                        //文章正文
                        this.txtRegion.Enabled = false;
                        this.txtGetStart.Enabled = false;
                        this.txtGetEnd.Enabled = false;

                        this.txtGetStart.Text = "";
                        this.txtGetEnd.Text = "";

                        this.txtRegion.Text = "";
                        break;
                    case 10:
                        //文章发布时间
                        this.txtRegion.Enabled = false;
                        this.txtGetStart.Enabled = false;
                        this.txtGetEnd.Enabled = false;

                        this.txtGetStart.Text = "";
                        this.txtGetEnd.Text = "";

                        this.txtRegion.Text = "";
                        break;
                    case 11:
                        //文章来源
                        this.txtRegion.Enabled = false;
                        this.txtGetStart.Enabled = false;
                        this.txtGetEnd.Enabled = false;

                        this.txtGetStart.Text = "";
                        this.txtGetEnd.Text = "";

                        this.txtRegion.Text = "";
                        break;
                }
            }
        }

        private void rmenuStartWildcardNum_Click(object sender, EventArgs e)
        {
            InsertStartPosWildcard(@"<Wildcard>(\d+)</Wildcard>");
        }

        private void rmenuStartWildcardLetter_Click(object sender, EventArgs e)
        {
            InsertStartPosWildcard(@"<Wildcard>(\w+?)</Wildcard>");
        }

        private void rmenuStartWildcardAny_Click(object sender, EventArgs e)
        {
            InsertStartPosWildcard(@"<Wildcard>(.+?)</Wildcard>");
        }

        private void rmenuStartWildcardRegex_Click(object sender, EventArgs e)
        {
            InsertStartPosWildcard(@"<Wildcard></Wildcard>");
        }

        private void rmenuEndWildcardNum_Click(object sender, EventArgs e)
        {
            InsertEndPosWildcard(@"<Wildcard>(\d+)</Wildcard>");
        }

        private void rmenuEndWildcardLetter_Click(object sender, EventArgs e)
        {
            InsertEndPosWildcard(@"<Wildcard>(\w+?)</Wildcard>");
        }

        private void rmenuEndWildcardAny_Click(object sender, EventArgs e)
        {
            InsertEndPosWildcard(@"<Wildcard>(.+?)</Wildcard>");
        }

        private void rmenuEndWildcardRegex_Click(object sender, EventArgs e)
        {
            InsertEndPosWildcard(@"<Wildcard></Wildcard>");
        }

        private void InsertStartPosWildcard(string ss)
        {
            int startPos = this.txtGetStart.SelectionStart;
            int l = this.txtGetStart.SelectionLength;

            this.txtGetStart.Text = this.txtGetStart.Text.Substring(0, startPos)
                + ss + this.txtGetStart.Text.Substring(startPos + l, this.txtGetStart.Text.Length - startPos - l);

            this.txtGetStart.SelectionStart = startPos + ss.Length;
            this.txtGetStart.ScrollToCaret();
        }

        private void InsertEndPosWildcard(string ss)
        {
            int startPos = this.txtGetEnd.SelectionStart;
            int l = this.txtGetEnd.SelectionLength;

            this.txtGetEnd.Text = this.txtGetEnd.Text.Substring(0, startPos)
                + ss + this.txtGetEnd.Text.Substring(startPos + l, this.txtGetEnd.Text.Length - startPos - l);

            this.txtGetEnd.SelectionStart = startPos + ss.Length;
            this.txtGetEnd.ScrollToCaret();
        }

        private void butOpenPath_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.Description = "请选择保存下载文件的路径";
            this.folderBrowserDialog1.SelectedPath = Program.getPrjPath();
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtSaveFilePath.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void cmdStartWildcard_Click(object sender, EventArgs e)
        {
            this.rmenuStartWildcard.Show(this.cmdStartWildcard, 0, 42);
        }

        private void cmdEndWildcard_Click(object sender, EventArgs e)
        {
            this.rmenuEndWildcard.Show(this.cmdEndWildcard, 0, 42);
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            //判断合法性
            if (raSmart.Checked == true)
            {
                if (this.comGetType.SelectedIndex == 0 ||
                    this.comGetType.SelectedIndex == 1 ||
                    this.comGetType.SelectedIndex == 2)
                {
                    //this.comGetType.SelectedIndex = 4;
                    MessageBox.Show("您选择了智能采集，所以数据类型只能选择智能提取部分", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.comGetType.Focus();
                    return;
                }

            }
            else
            {
                if (this.comGetType.SelectedIndex == 3 ||
                    this.comGetType.SelectedIndex == 4 ||
                    this.comGetType.SelectedIndex == 5)
                {
                    //this.comGetType.SelectedIndex = 0;
                    MessageBox.Show("您选择了非智能采集，所以数据类型只能选择智能提取部分", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.comGetType.Focus();
                    return;
                }
            }

            if (this.txtGetTitleName.Text.Trim() == "")
            {
                MessageBox.Show("采集规则的名称不能为空，请输入采集规则名称！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtGetTitleName.Focus();
                return;
            }


            this.txtGetTitleName.Text = this.txtGetTitleName.Text.Trim();
            if (Regex.IsMatch (this.txtGetTitleName.Text ,"[\\(|\\)|\\?|\\!|#|/|\\\\|\\s]"))
            {
                MessageBox.Show("采集规则的名称不能包括( ) ! ? / \\ | # $及空格等符号，请检查！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtGetTitleName.Focus();
                return;
            }
            if (Regex.IsMatch(this.txtGetTitleName.Text.Substring(0, 1), "[1-9|0]"))
            {
                MessageBox.Show("采集规则的名称首字母不能为数字！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtGetTitleName.Focus();
                return;
            }

            if (this.raNavPage.Checked == true && this.comNavLevel.SelectedIndex == -1)
            {
                MessageBox.Show("如果选择采集规则所属导航页，则必须输入导航页的级别！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.comNavLevel.Focus();
                return;
            }

            if (this.raMultiPage.Checked == true && this.comMultiPage.SelectedIndex == -1)
            {
                MessageBox.Show("如果选择采集规则所属多页，则必须输入多页名称！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.comMultiPage.Focus();
                return;
            }

            if(this.txtXPath.Enabled ==true && this.txtXPath.Text =="")
            {
                MessageBox.Show("如果采用XPath匹配数据，则XPath表达式不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtXPath.Focus();
                return;
            }

            if (this.txtGetStart.Enabled == true && this.txtGetStart.Text == "" && this.comGetType.SelectedIndex !=6)
            {
                MessageBox.Show("如果采用常规配置，则起始位置不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtGetStart.Focus();
                return;
            }

            if (this.txtGetEnd.Enabled == true && this.txtGetEnd.Text == "" && this.comGetType.SelectedIndex != 6)
            {
                MessageBox.Show("如果采用常规配置，则终止位置不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtGetEnd.Focus();
                return;
            }

            if (this.txtRegion.Enabled == true && this.txtRegion.Text == "")
            {
                MessageBox.Show("如果选择采用正则匹配，则正则不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtRegion.Focus();
                return;
            }

            if (this.fState == cGlobalParas.FormState.New)
            {
                bool isExist = false;
                for (int i = 0; i < this.m_gNames.Count; i++)
                {
                    if (this.txtGetTitleName.Text == this.m_gNames[i].ToString())
                    {
                        isExist = true;
                        break;
                    }
                }

                if (isExist == true)
                {
                    MessageBox.Show("已经存在同样名称的采集规则，请更换采集规则的名称！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.txtGetTitleName.Focus();
                    return;
                }
            }

            cGlobalParas.GatherRuleByPage gRulePage = 0;

            if (raPage.Checked == true)
                gRulePage = cGlobalParas.GatherRuleByPage.GatherPage;
            else if (raNavPage.Checked == true)
                gRulePage = cGlobalParas.GatherRuleByPage.NaviPage;
            else if (raMultiPage.Checked == true)
                gRulePage = cGlobalParas.GatherRuleByPage.MultiPage;
            //else if (raNonePage.Checked == true)
            //    gRuleType = cGlobalParas.GatherRuleByPage.NonePage;
            
            string nLevel="0";
            if (this.comNavLevel.SelectedIndex !=-1)
                nLevel =this.comNavLevel.SelectedItem.ToString ();

            string mPage="";
            if (this.comMultiPage.SelectedIndex !=-1)
                mPage =this.comMultiPage.SelectedItem.ToString ();


            cGatherRule gRule = new cGatherRule();
            gRule.fState =this.fState;
            gRule.gName=this.txtGetTitleName.Text.Trim ();
            gRule.gType=this.comGetType.SelectedItem.ToString();
            gRule.getStart=this.txtGetStart.Text.Trim ();
            gRule.getEnd=this.txtGetEnd.Text.Trim ();
            gRule.limitType=this.comLimit.SelectedItem.ToString();
            gRule.strReg=this.txtRegion.Text;
            gRule.IsMergeData=this.IsMergeData.Checked;
            gRule.gRuleByPage = gRulePage;
            gRule.NaviLevel=nLevel;
            gRule.MultiPageName=mPage;
            gRule.sPath=this.txtSaveFilePath.Text;
            //gRule.fileDealType=filedType;

            if (this.raGather.Checked == true)
            {
                gRule.gRuleType = cGlobalParas.GatherRuleType.Normal;
                gRule.gNodePrty = "";
            }
            else if (this.raXPath.Checked == true)
            {
                gRule.gRuleType = cGlobalParas.GatherRuleType.XPath;
                gRule.gNodePrty = this.comHNodeTextType.Text;
            }
            else if (this.raSmart.Checked == true)
            {
                gRule.gRuleType = cGlobalParas.GatherRuleType.Smart ;
                gRule.gNodePrty = "";
            }
            else if (raNonePage.Checked == true)
            {
                gRule.gRuleType = cGlobalParas.GatherRuleType.NonePage;
                gRule.gNodePrty = "";
            }


            gRule.xPath=this.txtXPath.Text ;
            gRule.IsAutoDownloadFileImage=this.IsAutoDownloadFileImage.Checked;
            gRule.IsAutoDownloadOnlyImage = this.IsAutoDownloadOnlyImage.Checked;

            SetIsAutoDownloadImage();

            //加载数据加工规则
            eFieldRules fRules = new eFieldRules();
            fRules.Field = this.txtGetTitleName.Text;

            eFieldRule cf;
            List<eFieldRule> listcf = new List<eFieldRule>();
            for (int j = 0; j < this.dataDataRules.Rows.Count; j++)
            {
                if (this.dataDataRules.Rows[j].Cells[1].Value != null)
                {
                    cf = new eFieldRule();
                    cf.Field = this.txtGetTitleName.Text; 
                    cf.Level = int.Parse(this.dataDataRules.Rows[j].Cells[0].Value.ToString());
                    cf.FieldRuleType = EnumUtil.GetEnumName<cGlobalParas.ExportLimit> (this.dataDataRules.Rows[j].Cells[1].Value.ToString());
                    string ss = string.Empty;
                    if (this.dataDataRules.Rows[j].Cells[2].Value != null)
                    {
                        ss = this.dataDataRules.Rows[j].Cells[2].Value.ToString();
                        ss = ss.Replace("\r", "");
                        ss = ss.Replace("\n", "");
                    }

                    cf.FieldRule = (ss == null ? "" : ss);

                    listcf.Add(cf);
                }
            }
            fRules.Field = this.txtGetTitleName.Text; ;
            fRules.FieldRule = listcf;

            gRule.fieldDealRules = fRules;

            rGatherRule(gRule);

            this.Close();
        }

        private void raNonePage_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raNonePage.Checked == true)
            {
                this.comHNodeTextType.Enabled = false;
                this.txtXPath.Enabled = false;
                this.cmdVisual.Enabled = false;
                this.txtGetStart.Enabled = false;
                this.txtGetEnd.Enabled = false;
                this.cmdStartWildcard.Enabled = false;
                this.cmdEndWildcard.Enabled = false;
                this.IsAutoDownloadFileImage.Enabled = false;
                this.comLimit.Enabled = false;
                this.txtRegion.Enabled = false;
                this.IsMergeData.Enabled = false;

                ResourceManager rmPara = new ResourceManager("NetMiner.Resource.Resources.globalPara", Assembly.Load("NetMiner.Resource"));
                this.comGetType.Items.Clear();
                this.comGetType.Items.Add(rmPara.GetString("GDataType4"));
                this.comGetType.Items.Add(rmPara.GetString("GDataType3"));
                this.comGetType.Items.Add(rmPara.GetString("GDataType1"));
                this.comGetType.Items.Add(rmPara.GetString("GDataType8"));
                this.comGetType.SelectedIndex = 0;
                rmPara = null;

                this.label16.Visible = true;
                this.label3.Visible = true;
                this.label4.Visible = true;
                this.label17.Visible = true;
                this.label19.Visible = true;
                this.label35.Visible = true;
                this.comLimit.Visible = true;
                this.txtRegion.Visible = true;
                this.cmdStartWildcard.Visible = true;
                this.cmdEndWildcard.Visible = true;
                this.txtGetStart.Visible = true;
                this.txtGetEnd.Visible = true;

                this.label2.Visible = false;
                this.label1.Visible = false;
                this.comHNodeTextType.Visible = false;
                this.txtXPath.Visible = false;
                this.cmdVisual.Visible = false;
            }
        }

        private void SetGroup2(bool isEnable)
        {
            this.raGather.Enabled = isEnable;
            this.raXPath.Enabled = isEnable;
            this.raSmart.Enabled = isEnable;
            this.comHNodeTextType.Enabled = isEnable;
            this.txtXPath.Enabled = isEnable;
            this.cmdVisual.Enabled = isEnable;
            this.txtGetStart.Enabled = isEnable;
            this.txtGetEnd.Enabled = isEnable;
            this.cmdStartWildcard.Enabled = isEnable;
            this.cmdEndWildcard.Enabled = isEnable;
            this.IsAutoDownloadFileImage.Enabled = isEnable;
            this.comLimit.Enabled = isEnable;
            this.txtRegion.Enabled = isEnable;
            this.IsMergeData.Enabled = isEnable;

            if (isEnable == true)
            {
                if (this.raGather.Checked == true)
                {
                    SetRaGather();
                }
                else if (this.raXPath.Checked == true)
                {
                    SetRaXPath();
                }
                else if (this.raSmart.Checked == true)
                {
                    SetRaSmart();
                }
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void cmdVisual_Click(object sender, EventArgs e)
        {
            frmVisual f = new frmVisual(this.m_TestUrl,this.m_cookie);
            f.rxPath = this.GetXPath;
            f.ShowDialog();
            f.Dispose();
        }

        private void GetXPath(string xPath, string hNodeText)
        {
            this.txtXPath.Text = xPath;
            this.comHNodeTextType.Text = hNodeText;
        }

        private void raGather_CheckedChanged(object sender, EventArgs e)
        {
            SetRaGather();
        }

        private void SetRaGather()
        {
            if (this.raGather.Checked == true)
            {
                //this.comGetType.SelectedIndex = 0;

                this.txtXPath.Enabled = false;
                this.cmdVisual.Enabled = false;
                this.comHNodeTextType.Enabled = false;

                this.txtGetStart.Enabled = true;
                this.txtGetEnd.Enabled = true;
                this.cmdStartWildcard.Enabled = true;
                this.cmdEndWildcard.Enabled = true;
                this.comLimit.Enabled = true;
                this.comLimit.SelectedIndex = 0;
                this.txtRegion.Enabled = false ;

                ResourceManager rmPara = new ResourceManager("NetMiner.Resource.Resources.globalPara", Assembly.Load("NetMiner.Resource"));
                this.comGetType.Items.Clear();
                this.comGetType.Items.Add(rmPara.GetString("GDataType4"));
                this.comGetType.Items.Add(rmPara.GetString("GDataType3"));
                this.comGetType.Items.Add(rmPara.GetString("GDataType1"));
                this.comGetType.Items.Add(rmPara.GetString("GDataType8"));
                this.comGetType.SelectedIndex = 0;
                rmPara = null;

                this.label16.Visible = true;
                this.label3.Visible = true;
                this.label4.Visible = true;
                this.label17.Visible = true;
                this.label19.Visible = true;
                this.label35.Visible = true;
                this.comLimit.Visible = true;
                this.txtRegion.Visible = true;
                this.cmdStartWildcard.Visible = true;
                this.cmdEndWildcard.Visible = true;
                this.txtGetStart.Visible = true;
                this.txtGetEnd.Visible = true;

                this.label2.Visible = false;
                this.label1.Visible = false;
                this.comHNodeTextType.Visible = false;
                this.txtXPath.Visible = false;
                this.cmdVisual.Visible = false;

            }
        }

        private void raXPath_CheckedChanged(object sender, EventArgs e)
        {
            SetRaXPath();
        }

        private void SetRaXPath()
        {
            if (this.raXPath.Enabled == true)
            {
                //this.comGetType.SelectedIndex = 0;

                this.txtXPath.Enabled = true;
                this.cmdVisual.Enabled = true;
                this.comHNodeTextType.Enabled = true ;

                this.txtGetStart.Enabled = false ;
                this.txtGetEnd.Enabled = false ;
                this.cmdStartWildcard.Enabled = false ;
                this.cmdEndWildcard.Enabled = false ;
                this.comLimit.Enabled = false ;
                this.txtRegion.Enabled = false ;

                ResourceManager rmPara = new ResourceManager("NetMiner.Resource.Resources.globalPara", Assembly.Load("NetMiner.Resource"));
                this.comGetType.Items.Clear();
                this.comGetType.Items.Add(rmPara.GetString("GDataType4"));
                this.comGetType.Items.Add(rmPara.GetString("GDataType3"));
                this.comGetType.Items.Add(rmPara.GetString("GDataType1"));
                this.comGetType.SelectedIndex = 0;
                //this.comGetType.Items.Add(rmPara.GetString("GDataType9"));

                rmPara = null;

                this.comHNodeTextType.SelectedIndex = 1;

                this.label16.Visible = false;
                this.label3.Visible = false;
                this.label4.Visible = false;
                this.label17.Visible = false;
                this.label19.Visible = false;
                this.label35.Visible = false;
                this.comLimit.Visible = false;
                this.txtRegion.Visible = false;
                this.cmdStartWildcard.Visible = false;
                this.cmdEndWildcard.Visible = false;
                this.txtGetStart.Visible = false;
                this.txtGetEnd.Visible = false;

                this.label2.Visible = true;
                this.label1.Visible = true;
                this.comHNodeTextType.Visible = true;
                this.txtXPath.Visible = true;
                this.cmdVisual.Visible = true;
            }
        }

        private void IsOCRText_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void raSmart_CheckedChanged(object sender, EventArgs e)
        {
            SetRaSmart();
        }

        private void SetRaSmart()
        {
            if (raSmart.Checked == true)
            {
                //this.comGetType.SelectedIndex = 3;

                this.comLimit.Enabled = false;
                this.comLimit.SelectedIndex = 0;

                this.comHNodeTextType.Enabled = false;
                this.txtXPath.Enabled = false;
                this.cmdVisual.Enabled = false;
                this.txtGetStart.Enabled = false;
                this.txtGetEnd.Enabled = false;
                this.cmdStartWildcard.Enabled = false;
                this.cmdEndWildcard.Enabled = false;

                if (this.comGetType.SelectedIndex == 0)
                {
                    this.txtSaveFilePath.Enabled = false;
                    this.label57.Enabled = false;
                    this.butOpenPath.Enabled = false;
                }
                else
                {
                    this.txtSaveFilePath.Enabled = true ;
                    this.label57.Enabled = true ;
                    this.butOpenPath.Enabled = true ;
                }

                this.comGetType.Items.Clear();
                ResourceManager rmPara = new ResourceManager("NetMiner.Resource.Resources.globalPara", Assembly.Load("NetMiner.Resource"));

                this.comGetType.Items.Add(rmPara.GetString("GDataType5"));
                this.comGetType.Items.Add(rmPara.GetString("GDataType6"));
                this.comGetType.Items.Add(rmPara.GetString("GDataType7"));
                this.comGetType.Items.Add(rmPara.GetString("GDataType10"));
                this.comGetType.Items.Add(rmPara.GetString("GDataType9"));
                this.comGetType.SelectedIndex = 0;
                rmPara = null;

                this.txtSaveFilePath.Enabled = false;
                this.butOpenPath.Enabled = false;

                this.IsAutoDownloadFileImage.Enabled = true;

                this.label16.Visible = true;
                this.label3.Visible = true;
                this.label4.Visible = true;
                this.label17.Visible = true;
                this.label19.Visible = true;
                this.label35.Visible = true;
                this.comLimit.Visible = true;
                this.txtRegion.Visible = true;
                this.cmdStartWildcard.Visible = true;
                this.cmdEndWildcard.Visible = true;
                this.txtGetStart.Visible = true;
                this.txtGetEnd.Visible = true;

                this.label2.Visible = false;
                this.label1.Visible = false;
                this.comHNodeTextType.Visible = false;
                this.txtXPath.Visible = false;
                this.cmdVisual.Visible = false;
            }

        }


        private void IsAutoDownloadImage_CheckedChanged(object sender, EventArgs e)
        {
            if (IsAutoDownloadFileImage.Checked == true)
                IsAutoDownloadOnlyImage.Enabled = true;
            else if (IsAutoDownloadFileImage.Checked == false)
                IsAutoDownloadOnlyImage.Enabled = false;
        }

        private void IsAutoDownloadImage_Click(object sender, EventArgs e)
        {
            SetIsAutoDownloadImage();
        }

        private void SetIsAutoDownloadImage()
        {
            
                if (IsAutoDownloadFileImage.Checked == true)
                {
                    if (Program.SominerVersion != cGlobalParas.VersionType.Free)
                    {
                        this.txtSaveFilePath.Enabled = true;
                        this.label57.Enabled = true;
                        this.butOpenPath.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("当前版本不支持自动下载图片操作，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.IsAutoDownloadFileImage.Checked = false;
                        return;
                    }
                }
                else if (IsAutoDownloadFileImage.Checked == false && this.comGetType.SelectedIndex ==0)
                {
                    
                    this.txtSaveFilePath.Enabled = false;
                    this.label57.Enabled = false;
                    this.butOpenPath.Enabled = false;
                }
            
            
        }

        private void button17_Click(object sender, EventArgs e)
        {
            frmAddDataEditRule f = new frmAddDataEditRule(m_TName);
            f.rERule = this.GetEditRule;
            f.ShowDialog();
            f.Dispose();
        }

        private void GetEditRule(string eRule, string strRule)
        {
            this.dataDataRules.Rows.Add();
            this.dataDataRules.Rows[this.dataDataRules.Rows.Count - 2].Cells[1].Value = eRule;
            this.dataDataRules.Rows[this.dataDataRules.Rows.Count - 2].Cells[2].Value = strRule;
          
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.dataDataRules.Focus();
            SendKeys.Send("{Del}");
        }

        private void cmdUpDealRule_Click(object sender, EventArgs e)
        {
            if (this.dataDataRules.SelectedCells.Count == 0 || this.dataDataRules.Rows.Count == 1)
                return;

            int i = this.dataDataRules.SelectedCells[0].RowIndex;
            if (i == 0)
                return;

            if (this.dataDataRules.Rows[i].Cells[1].Value == null)
                return;

            string[] r = new string[3];
            r[0] = (i + 1).ToString();
            r[1] = this.dataDataRules.Rows[i].Cells[1].Value.ToString();
            r[2] = this.dataDataRules.Rows[i].Cells[2].Value.ToString();
            this.dataDataRules.Rows.Remove(this.dataDataRules.Rows[i]);

            this.dataDataRules.Rows.Insert(i - 1);
            this.dataDataRules.Rows[i - 1].Cells[1].Value = r[1];
            this.dataDataRules.Rows[i - 1].Cells[2].Value = r[2];

            for (int j = 0; j < this.dataDataRules.Rows.Count; j++)
            {
                this.dataDataRules.Rows[j].Selected = false;
            }
            this.dataDataRules.Rows[i - 1].Selected = true;

            this.dataDataRules.FirstDisplayedScrollingRowIndex = i - 1;

        }

        private void cmdDownDealRule_Click(object sender, EventArgs e)
        {
            if (this.dataDataRules.SelectedCells.Count == 0 || this.dataDataRules.Rows.Count == 1)
                return;


            int i = this.dataDataRules.SelectedCells[0].RowIndex;
            if (this.dataDataRules.Rows[i].Cells[1].Value == null)
                return;

            if ((i + 2) == this.dataDataRules.Rows.Count)
                return;
            string[] r = new string[3];
            r[0] = (i + 1).ToString();
            r[1] = this.dataDataRules.Rows[i].Cells[1].Value.ToString();
            r[2] = this.dataDataRules.Rows[i].Cells[2].Value.ToString();
            this.dataDataRules.Rows.Remove(this.dataDataRules.Rows[i]);

            this.dataDataRules.Rows.Insert(i + 1);
            this.dataDataRules.Rows[i + 1].Cells[1].Value = r[1];
            this.dataDataRules.Rows[i + 1].Cells[2].Value = r[2];

            for (int j = 0; j < this.dataDataRules.Rows.Count; j++)
            {
                this.dataDataRules.Rows[j].Selected = false;
            }
            this.dataDataRules.Rows[i + 1].Selected = true;

            this.dataDataRules.FirstDisplayedScrollingRowIndex = i + 1;

            //SaveExportRule();
            //this.IsSave.Text = "true";
        }

        private void dataDataRules_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = 0; i < this.dataDataRules.Rows.Count; i++)
            {
                this.dataDataRules.Rows[i].Cells[0].Value = i + 1;
            }
        }

        private void dataDataRules_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            for (int i = 0; i < this.dataDataRules.Rows.Count; i++)
            {
                this.dataDataRules.Rows[i].Cells[0].Value = i + 1;

            }
        }

        private void dataDataRules_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.dataDataRules.CurrentCell.ColumnIndex != 0)
            {

                if (e.Control is TextBox)
                {

                    TextBox tb = e.Control as TextBox;

                    tb.KeyPress -= new KeyPressEventHandler(tb_KeyPress);

                    tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);

                }
                else if (e.Control is ComboBox)
                {
                    ComboBox cb = e.Control as ComboBox;
                    cb.SelectedIndexChanged -= new EventHandler(cb_SelectedIndexChanged);
                    cb.SelectedIndexChanged += new EventHandler(cb_SelectedIndexChanged);

                }

            }
        }

        void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            int curRow = this.dataDataRules.CurrentCell.RowIndex;

            switch (((ComboBox)sender).SelectedIndex)
            {
            
              
                case 3:
                    this.dataDataRules.Rows[curRow].Cells[2].Value = "0";
                    break;
                case 4:
                    this.dataDataRules.Rows[curRow].Cells[2].Value = "0";
                    break;
                case 5:
                    this.dataDataRules.Rows[curRow].Cells[2].Value = "<OldValue:><NewValue:>";
                    break;
                case 7:
                    this.dataDataRules.Rows[curRow].Cells[2].Value = "<OldValue:><NewValue:>";
                    break;
                case 15:
                    this.dataDataRules.Rows[curRow].Cells[2].Value = "0";
                    break;
                case 19:
                    this.dataDataRules.Rows[curRow].Cells[2].Value = "<Wrap:>";
                    break;
                case 38:
                    this.dataDataRules.Rows[curRow].Cells[2].Value = "<OldValue:><NewValue:>";
                    break;
                default:
                    this.dataDataRules.Rows[curRow].Cells[2].Value = "";
                    break;
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

        private void txtGetTitleName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.txtGetTitleName.Text == "**编辑常用规则名称**")
            {
                //进行常用选项编辑
                frmCommonName f = new frmCommonName();
                f.ShowDialog();
                f.Dispose();

                iniCommonName();
            }
        }

        private void iniCommonName()
        {
            this.txtGetTitleName.Items.Clear();

            string fName = Program.getPrjPath() + "dict\\rulename.txt";
            if (File.Exists(fName))
            {
                //打开此文件
                StreamReader fileReader = new StreamReader(fName, System.Text.Encoding.UTF8);
                string strLine = "";
                StringBuilder sb = new StringBuilder();

                while (strLine != null)
                {
                    strLine = fileReader.ReadLine();
                    if (strLine != null && strLine.Length > 0)
                    {
                        this.txtGetTitleName.Items.Add(strLine);
                    }
                }

                fileReader.Close();
                fileReader = null;
            }
            this.txtGetTitleName.Items.Add("**编辑常用规则名称**");
        }
    }


    public class cGatherRule
    {
        public cGlobalParas.FormState fState;
        public string gName;
        /// <summary>
        /// 采集数据类型
        /// </summary>
        public string gType;
        public string getStart;
        public string getEnd;
        public string limitType;
        public string strReg;
        public bool IsMergeData;
        public cGlobalParas.GatherRuleByPage gRuleByPage;
        public string NaviLevel;
        public string MultiPageName;
        public string sPath;
        //public string fileDealType;
        /// <summary>
        /// 采集规则类型：常规采集；XPath采集
        /// </summary>
        public cGlobalParas.GatherRuleType gRuleType;
        public string xPath;
        public bool IsAutoDownloadFileImage;

        //V5.0增加
        public bool IsAutoDownloadOnlyImage; 
        /// <summary>
        /// xPath节点属性
        /// </summary>
        public string gNodePrty;

        public eFieldRules fieldDealRules;

        public cGatherRule Clone()
        {
            cGatherRule gRule = new cGatherRule();
            gRule.fState= this.fState;
            gRule.gName = this.gName;
            gRule.gType= this.gType;
            gRule.getStart = this.getStart;
            gRule.getEnd = this.getEnd;
            gRule.limitType= this.limitType;
            gRule.strReg= this.strReg;
            gRule.IsMergeData= this.IsMergeData;
            gRule.gRuleByPage= this.gRuleByPage;
            gRule.NaviLevel= this.NaviLevel;
            gRule.MultiPageName= this.MultiPageName;
            gRule.sPath= this.sPath;
            //gRule.fileDealType= this.fileDealType;
            gRule.gRuleType= this.gRuleType;
            gRule.xPath= this.xPath;
            gRule.IsAutoDownloadFileImage = this.IsAutoDownloadFileImage;
            gRule.IsAutoDownloadOnlyImage= this.IsAutoDownloadOnlyImage;
            gRule.gNodePrty= this.gNodePrty;
            gRule.fieldDealRules= this.fieldDealRules;

            return gRule;

        }
    }
}

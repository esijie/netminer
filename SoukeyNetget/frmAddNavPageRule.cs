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
using NetMiner.Common;
using System.Text.RegularExpressions;
using SoukeyControl;
using SoukeyControl.WebBrowser;

namespace MinerSpider
{
    public partial class frmAddNavPageRule : Form
    {
        private ResourceManager rm;

        public delegate void ReturnNavRule(cGlobalParas.FormState fState,cNaviRuleFormConfig fNaviRule);
        
        public ReturnNavRule rNavRule;

        //定义一个变量记录导航规则
        private string m_NaviRule = string.Empty;

        //定义一个变量记录翻页规则
        private string m_NextRule = string.Empty;

        //定义一个ToolTip
        ToolTip HelpTip = new ToolTip();
        private string m_getUrlPara = string.Empty;

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

        private string m_Cookie;
        public string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }



        public frmAddNavPageRule()
        {
            InitializeComponent();

            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
               Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                this.raNarmal.Enabled = true;
                this.raSkip.Enabled = true;
                this.raRegex.Enabled = true;
                this.txtNaviRegex.Enabled = false  ;
            }
            else
            {
                this.raNarmal.Enabled = false ;
                this.raSkip.Enabled = false ;
                this.raRegex.Enabled = false ;
                this.txtNaviRegex.Enabled = false;
            }


            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));
        }

        public void FillData(cNaviRuleFormConfig fRule)
        {
            //this.IsNext.Checked =fRule.IsNext;
            //this.txtNextRule.Text = fRule.NextRule;

            m_NaviRule = fRule.NavRule;
            this.txtNavStart.Text = fRule.SPos;
            this.txtNavEnd.Text = fRule.EPos;
            this.IsGather.Checked = fRule.IsGather;
            this.GStartPos.Text = fRule.GSPos;
            this.GEndPos.Text = fRule.GEPos;
            this.IsNaviNextPage.Checked = fRule.IsNaviNext;
            m_NextRule = fRule.NaviNextRule;
            if (this.IsNaviNextPage.Checked == true)
            {
                FillNextRule(m_NextRule);
            }

            if (fRule.NextMaxPage != "")
            {
                //this.numMaxNext.Value = int.Parse(fRule.NextMaxPage);
            }

            if (fRule.NaviNextMaxPage != "")
                this.numMaxNaviNext.Value = int.Parse(fRule.NaviNextMaxPage);

            //this.IsNextDoPostBack.Checked = fRule.IsNextDoPostBack;
            this.IsNaviNextDoPostBack.Checked = fRule.IsNaviNextDoPostBack;

            if (fRule.RunRule == cGlobalParas.NaviRunRule.Normal)
                this.raNarmal.Checked = true;
            else if (fRule.RunRule == cGlobalParas.NaviRunRule.Skip)
                this.raSkip.Checked = true;
            else if (fRule.RunRule == cGlobalParas.NaviRunRule.Other)
            {
                this.raRegex.Checked = true;
                this.txtNaviRegex.Text = fRule.OtherNaviRule;
            }

            ParseNaviRule(m_NaviRule);
        }

        private void FillNextRule(string strNRule)
        {
            if (strNRule == "")
                return;

            string nRule = strNRule;
            Match charSetMatch;

            if (Regex.IsMatch(strNRule, "<Prefix>"))
            {
                charSetMatch = Regex.Match(nRule, "(?<=<Prefix>).*?(?=</Prefix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.txtNextPrestr.Text = charSetMatch.Groups[0].ToString();

                //charSetMatch = Regex.Match(strNRule, "(?<=</Prefix>).*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //strNRule = charSetMatch.Groups[0].ToString();
                strNRule = Regex.Replace(strNRule, "<Prefix>.*?</Prefix>", "");
            }

            if (Regex.IsMatch(strNRule, "<Suffix>"))
            {
                charSetMatch = Regex.Match(nRule, "(?<=<Suffix>).*?(?=</Suffix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.txtNextSufStr.Text = charSetMatch.Groups[0].ToString();

                //charSetMatch = Regex.Match(strNRule, ".*?(?=<Suffix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //strNRule = charSetMatch.Groups[0].ToString();
                strNRule = Regex.Replace(strNRule, "<Suffix>.*?</Suffix>", "");
            }

            if (Regex.IsMatch(strNRule, "<Include>"))
            {
                charSetMatch = Regex.Match(nRule, "(?<=<Include>).*?(?=</Include>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.txtNextIncludeWord.Text = charSetMatch.Groups[0].ToString();

                //charSetMatch = Regex.Match(strNRule, "(?<=</Include>).*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //strNRule = charSetMatch.Groups[0].ToString();
                strNRule = Regex.Replace(strNRule, "<Include>.*?</Include>", "");
            }

            if (Regex.IsMatch(strNRule, "<NoInclude>"))
            {
                charSetMatch = Regex.Match(nRule, "(?<=<NoInclude>).*?(?=</NoInclude>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.txtNextNoIncludeWord.Text = charSetMatch.Groups[0].ToString();

                //charSetMatch = Regex.Match(strNRule, "(?<=</NoInclude>).*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //strNRule = charSetMatch.Groups[0].ToString();
                strNRule = Regex.Replace(strNRule, "<NoInclude>.*?</NoInclude>", "");
            }



            if (Regex.IsMatch(strNRule, "<RegexReplace>"))
            {
                this.isNextRegex.Checked = true;

                charSetMatch = Regex.Match(nRule, "(?<=<RegexReplace>).*?(?=</RegexReplace>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string ss = charSetMatch.Groups[0].ToString();

                this.txtNextOldValue.Text = ss.Substring(ss.IndexOf("<OldValue:") + 10, ss.IndexOf("><NewValue:") - 10);
                this.txtNextNewValue.Text = ss.Substring(ss.IndexOf("<NewValue:") + 10, ss.Length - ss.IndexOf("<NewValue:") - 11);

                //charSetMatch = Regex.Match(strNRule, ".*?(?=<RegexReplace>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //strNRule = charSetMatch.Groups[0].ToString();
                strNRule = Regex.Replace(strNRule, "<RegexReplace>.*?</RegexReplace>", "");
            }
            else if (Regex.IsMatch(strNRule, "<Replace>"))
            {
                this.isNextRegex.Checked = false;

                charSetMatch = Regex.Match(nRule, "(?<=<Replace>).*?(?=</Replace>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string ss = charSetMatch.Groups[0].ToString();

                this.txtNextOldValue.Text = ss.Substring(ss.IndexOf("<OldValue:") + 10, ss.IndexOf("><NewValue:") - 10);
                this.txtNextNewValue.Text = ss.Substring(ss.IndexOf("<NewValue:") + 10, ss.Length - ss.IndexOf("<NewValue:") - 11);

                //charSetMatch = Regex.Match(strNRule, ".*?(?=<Replace>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //strNRule = charSetMatch.Groups[0].ToString();
                strNRule = Regex.Replace(strNRule, "<Replace>.*?</Replace>", "");
            }

            //提取参数
            if (Regex.IsMatch(strNRule, "<Parameter>"))
            {
                this.raPara.Checked = true;

                this.groupBox6.Visible = false;
                this.groupBox7.Visible = false ;
                this.groupBox8.Visible = true ;

                this.raPara.Checked = true;
                SetPara();
                charSetMatch = Regex.Match(nRule, "(?<=<Parameter>)[\\s\\S]*?(?=</Parameter>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.txtNextParaUrl.Text = charSetMatch.Groups[0].ToString();

                charSetMatch = Regex.Match(strNRule, ".*?(?=<Parameter>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();
            }
            else if (strNRule.StartsWith("<Trait>"))
            {
                this.radioButton7.Checked = true;

                this.groupBox6.Visible = true  ;
                this.groupBox7.Visible = false ;
                this.groupBox8.Visible = false;

                this.groupBox6.Visible = true;
                this.groupBox7.Visible = false;
                this.groupBox8.Visible = false;

                this.txtNextGetStart.Enabled = false;
                this.txtNextGetEnd.Enabled = false;
                this.checkBox7.Enabled = false;
                this.checkBox8.Enabled = false;
                this.checkBox9.Enabled = false;
                this.checkBox10.Enabled = false;
                this.checkBox11.Enabled = false;
                this.checkBox12.Enabled = false;
                this.txtNextCustomLetter.Enabled = false;

                this.txtNextRule.Enabled = true;

                charSetMatch = Regex.Match(strNRule, "(?<=<Trait>).*?(?=</Trait>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();

            }
            else if (strNRule.StartsWith("<Common>"))
            {
                this.radioButton6.Checked = true;

                this.groupBox6.Visible = true;
                this.groupBox7.Visible = false;
                this.groupBox8.Visible = false;

                this.txtNextGetStart.Enabled = true;
                this.txtNextGetEnd.Enabled = true;
                this.checkBox12.Enabled = true;
                this.checkBox11.Enabled = true;
                this.checkBox10.Enabled = true;
                this.checkBox9.Enabled = true;
                this.checkBox8.Enabled = true;
                this.checkBox7.Enabled = true;
                this.txtNextCustomLetter.Enabled = true;

                this.txtNextRule.Enabled = false;

                charSetMatch = Regex.Match(strNRule, "(?<=<Common>).*?(?=</Common>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();

                //分解正则表达式
                string starts = "";
                string ends = "";

                charSetMatch = Regex.Match(strNRule, "(?<=\\(\\?<=).+?(?=\\)\\[)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                starts = charSetMatch.Groups[0].ToString();

                charSetMatch = Regex.Match(strNRule, "(?<=\\(\\?=).+?(?=\\)$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                ends = charSetMatch.Groups[0].ToString();

                //this.txtGetStart.Text = Regex.Unescape(starts);
                //this.txtGetEnd.Text = Regex.Unescape(ends);

                this.txtNextGetStart.Text = ToolUtil.RegexUnescape(starts);
                this.txtNextGetEnd.Text = ToolUtil.RegexUnescape(ends);

                charSetMatch = Regex.Match(strNRule, "(?<=\\)\\[).+?(?=\\]\\+\\?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string ss = charSetMatch.Groups[0].ToString();
                if (ss.StartsWith("^"))
                {
                    //判断是否有多个符号
                    if (ss.IndexOf("|") > 0)
                    {
                        string[] s1 = ss.Split('|');

                        for (int m = 0; m < s1.Length; m++)
                        {
                            string s = s1[m].Substring(1, s1[m].Length - 1);
                            switch (s)
                            {
                                case ">":
                                    this.checkBox12.Checked = true;
                                    break;
                                case @"\s":
                                    this.checkBox11.Checked = true;
                                    break;
                                case "#":
                                    this.checkBox10.Checked = true;
                                    break;
                                case "'":
                                    this.checkBox9.Checked = true;
                                    break;
                                case "\"":
                                    this.checkBox8.Checked = true;
                                    break;
                                default:
                                    this.checkBox7.Checked = true;
                                    this.txtNextCustomLetter.Text = s;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        string s = ss.Substring(1, ss.Length - 1);
                        switch (s)
                        {
                            case ">":
                                this.checkBox12.Checked = true;
                                break;
                            case @"\s":
                                this.checkBox11.Checked = true;
                                break;
                            case "#":
                                this.checkBox10.Checked = true;
                                break;
                            case "'":
                                this.checkBox9.Checked = true;
                                break;
                            case "\"":
                                this.checkBox8.Checked = true;
                                break;
                            default:
                                this.checkBox7.Checked = true;
                                this.txtNextCustomLetter.Text = s;
                                break;
                        }
                    }
                }

            }
            else if (strNRule.StartsWith("<Regex>"))
            {
                this.radioButton5.Checked = true;

                this.groupBox6.Visible = true;
                this.groupBox7.Visible = false;
                this.groupBox8.Visible = false;

                this.groupBox6.Visible = true;
                this.groupBox7.Visible = false;
                this.groupBox8.Visible = false;

                this.txtNextGetStart.Enabled = false;
                this.txtNextGetEnd.Enabled = false;
                this.checkBox7.Enabled = false;
                this.checkBox8.Enabled = false;
                this.checkBox9.Enabled = false;
                this.checkBox10.Enabled = false;
                this.checkBox11.Enabled = false;
                this.checkBox12.Enabled = false;
                this.txtNextCustomLetter.Enabled = false;

                this.txtNextRule.Enabled = true;

                charSetMatch = Regex.Match(strNRule, "(?<=<Regex>).*?(?=</Regex>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();

            }
            else if (strNRule.StartsWith("<XPath>"))
            {
                this.radioButton8.Checked = true;

                this.groupBox6.Visible = false;
                this.groupBox7.Visible = true ;
                this.groupBox8.Visible = false;

                charSetMatch = Regex.Match(strNRule, "(?<=<XPath>).*?(?=</XPath>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();
                this.txtNextXPath.Text = strNRule;
                return;
            }

            this.txtNextRule.Text = strNRule;
        }

        private void SetPara()
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
               Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
               Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                this.txtNextGetStart.Enabled = false;
                this.txtNextGetEnd.Enabled = false;
                this.checkBox12.Enabled = false;
                this.checkBox11.Enabled = false;
                this.checkBox10.Enabled = false;
                this.checkBox9.Enabled = false;
                this.checkBox8.Enabled = false;
                this.checkBox7.Enabled = false;
                this.txtNextCustomLetter.Enabled = false;
                this.checkBox12.Checked = false;

                this.txtNextRule.Enabled = false;

                this.txtNextRule.Text = "";

               
            }
            else
            {
                MessageBox.Show("当前版本不支持翻页参数配置，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.raPara.Checked = false;
                return;
            }
        }

        private void ParseNaviRule(string strNRule)
        {

            this.m_TestUrl = TestUrl;

            this.comParaType.Items.Add("自定义参数");
            //this.comParaType.Items.Add("自增变量{Num:1,10,1}");
            //this.comParaType.Items.Add("时间戳{Timestamp:" + cTool.GetTimestamp().ToString() + "}");

            string nRule = strNRule;
            Match charSetMatch;

            if (Regex.IsMatch(strNRule, "<Prefix>"))
            {
                charSetMatch = Regex.Match(nRule, "(?<=<Prefix>).*?(?=</Prefix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.txtPrestr.Text = charSetMatch.Groups[0].ToString();

                strNRule = Regex.Replace(strNRule, "<Prefix>.*?</Prefix>", "");
            }

            if (Regex.IsMatch(strNRule, "<Suffix>"))
            {
                charSetMatch = Regex.Match(nRule, "(?<=<Suffix>).*?(?=</Suffix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.txtSufStr.Text = charSetMatch.Groups[0].ToString();

                strNRule = Regex.Replace(strNRule, "<Suffix>.*?</Suffix>", "");
            }

            if (Regex.IsMatch(strNRule, "<Include>"))
            {
                charSetMatch = Regex.Match(nRule, "(?<=<Include>).*?(?=</Include>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.txtIncludeWord.Text = charSetMatch.Groups[0].ToString();

                strNRule = Regex.Replace(strNRule, "<Include>.*?</Include>", "");
            }

            if (Regex.IsMatch(strNRule, "<NoInclude>"))
            {
                charSetMatch = Regex.Match(nRule, "(?<=<NoInclude>).*?(?=</NoInclude>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.txtNoIncludeWord.Text = charSetMatch.Groups[0].ToString();

                strNRule = Regex.Replace(strNRule, "<NoInclude>.*?</NoInclude>", "");
            }

            if (Regex.IsMatch(strNRule, "<RegexReplace>"))
            {
                this.isRegex.Checked = true;

                charSetMatch = Regex.Match(nRule, "(?<=<RegexReplace>).*?(?=</RegexReplace>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string ss = charSetMatch.Groups[0].ToString();

                this.txtOldValue.Text = ss.Substring(ss.IndexOf("<OldValue:") + 10, ss.IndexOf("><NewValue:") - 10);
                this.txtNewValue.Text = ss.Substring(ss.IndexOf("<NewValue:") + 10, ss.Length - ss.IndexOf("<NewValue:") - 11);

                //charSetMatch = Regex.Match(strNRule, ".*?(?=<RegexReplace>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //strNRule = charSetMatch.Groups[0].ToString();
                strNRule = Regex.Replace(strNRule, "<RegexReplace>.*?</RegexReplace>", "");
            }
            else if (Regex.IsMatch(strNRule, "<Replace>"))
            {
                this.isRegex.Checked = false;

                charSetMatch = Regex.Match(nRule, "(?<=<Replace>).*?(?=</Replace>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string ss = charSetMatch.Groups[0].ToString();

                this.txtOldValue.Text = ss.Substring(ss.IndexOf("<OldValue:") + 10, ss.IndexOf("><NewValue:") - 10);
                this.txtNewValue.Text = ss.Substring(ss.IndexOf("<NewValue:") + 10, ss.Length - ss.IndexOf("<NewValue:") - 11);

                //charSetMatch = Regex.Match(strNRule, ".*?(?=<Replace>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //strNRule = charSetMatch.Groups[0].ToString();
                strNRule = Regex.Replace(strNRule, "<Replace>.*?</Replace>", "");
            }

            if (strNRule.StartsWith("<Trait>"))
            {
                this.radioButton1.Checked = true;

                charSetMatch = Regex.Match(strNRule, "(?<=<Trait>).*?(?=</Trait>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();

            }
            else if (strNRule.StartsWith("<Common>"))
            {
                this.radioButton2.Checked = true;

                this.txtGetStart.Enabled = true;
                this.txtGetEnd.Enabled = true;
                this.checkBox1.Enabled = true;
                this.checkBox2.Enabled = true;
                this.checkBox3.Enabled = true;
                this.checkBox4.Enabled = true;
                this.checkBox5.Enabled = true;
                this.checkBox6.Enabled = true;
                this.txtCustomLetter.Enabled = true;

                this.txtNRule.Enabled = false;

                charSetMatch = Regex.Match(strNRule, "(?<=<Common>).*?(?=</Common>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();

                //分解正则表达式
                string starts = "";
                string ends = "";

                charSetMatch = Regex.Match(strNRule, "(?<=\\(\\?<=).+?(?=\\)\\[)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                starts = charSetMatch.Groups[0].ToString();

                charSetMatch = Regex.Match(strNRule, "(?<=\\(\\?=).+?(?=\\)$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                ends = charSetMatch.Groups[0].ToString();

                //this.txtGetStart.Text = Regex.Unescape ( starts);
                //this.txtGetEnd.Text =Regex.Unescape ( ends);

                this.txtGetStart.Text = ToolUtil.RegexUnescape(starts);
                this.txtGetEnd.Text = ToolUtil.RegexUnescape(ends);

                charSetMatch = Regex.Match(strNRule, "(?<=\\)\\[).+?(?=\\]\\+\\?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string ss = charSetMatch.Groups[0].ToString();
                if (ss.StartsWith("^"))
                {
                    //判断是否有多个符号
                    if (ss.IndexOf("|") > 0)
                    {
                        string[] s1 = ss.Split('|');

                        for (int m = 0; m < s1.Length; m++)
                        {
                            string s = s1[m].Substring(1, s1[m].Length - 1);
                            switch (s)
                            {
                                case ">":
                                    this.checkBox1.Checked = true;
                                    break;
                                case @"\s":
                                    this.checkBox2.Checked = true;
                                    break;
                                case "#":
                                    this.checkBox3.Checked = true;
                                    break;
                                case "'":
                                    this.checkBox4.Checked = true;
                                    break;
                                case "\"":
                                    this.checkBox5.Checked = true;
                                    break;
                                default:
                                    this.checkBox6.Checked = true;
                                    this.txtCustomLetter.Text = s;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        string s = ss.Substring(1, ss.Length - 1);
                        switch (s)
                        {
                            case ">":
                                this.checkBox1.Checked = true;
                                break;
                            case @"\s":
                                this.checkBox2.Checked = true;
                                break;
                            case "#":
                                this.checkBox3.Checked = true;
                                break;
                            case "'":
                                this.checkBox4.Checked = true;
                                break;
                            case "\"":
                                this.checkBox5.Checked = true;
                                break;
                            default:
                                this.checkBox6.Checked = true;
                                this.txtCustomLetter.Text = s;
                                break;
                        }
                    }
                }

            }
            else if (strNRule.StartsWith("<Regex>"))
            {
                this.radioButton3.Checked = true;

                charSetMatch = Regex.Match(strNRule, "(?<=<Regex>).*?(?=</Regex>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();

            }
            else if (strNRule.StartsWith("<MySelf>"))
            {
                this.raMySelf.Checked = true;
                charSetMatch = Regex.Match(strNRule, "(?<=<MySelf>).*?(?=</MySelf>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();
                this.txtNavStart.Enabled = false;
                this.txtNavEnd.Enabled = false;
                this.label12.Text = "循环标记";
                //strNRule = "<MySelf/>";
            }
            else if (strNRule.StartsWith("<ParaNavi>"))
            {
                //参数导航 这部分处理的不是很好，后期有时间需要进行修改

                this.groupBox2.Visible = false;
                this.groupBox3.Visible = false ;
                this.groupBox4.Visible = true ;

                this.txtGetStart.Enabled = false;
                this.txtGetEnd.Enabled = false;
                this.checkBox1.Enabled = false;
                this.checkBox2.Enabled = false;
                this.checkBox3.Enabled = false;
                this.checkBox4.Enabled = false;
                this.checkBox5.Enabled = false;
                this.checkBox6.Enabled = false;
                this.txtCustomLetter.Enabled = false;
                this.txtNRule.Enabled = false;

                this.radioButton4.Checked = true;
                this.radioButton1.Checked = false;
                this.radioButton2.Checked = false;
                this.radioButton3.Checked = false;
                this.raMySelf.Checked = false;

                this.comParaType.Enabled = true;
                this.comParaType.SelectedIndex = 0;
                this.txtGetStartPara.Enabled = true;
                this.txtGetEndPara.Enabled = true;

                this.UrlPara.Enabled = true;
                this.cmdInsertPara.Enabled = true;

                this.cmdNextEndWildcard.Enabled = true;

                charSetMatch = Regex.Match(strNRule, "(?<=<ParaNavi>)[\\s\\S]*?(?=</ParaNavi>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();
                this.UrlPara.Text = strNRule;

                return;
            }
            else if (strNRule.StartsWith("<XPath>")) 
            {

                this.groupBox2.Visible = false;
                this.groupBox3.Visible = true;
                this.groupBox4.Visible = false;

                //XPath可视化匹配
                this.txtGetStart.Enabled = false;
                this.txtGetEnd.Enabled = false;
                this.checkBox1.Enabled = false;
                this.checkBox2.Enabled = false;
                this.checkBox3.Enabled = false;
                this.checkBox4.Enabled = false;
                this.checkBox5.Enabled = false;
                this.checkBox6.Enabled = false;
                this.txtCustomLetter.Enabled = false;
                this.txtNRule.Enabled = false;

                this.radioButton4.Checked = false;
                this.radioButton1.Checked = false;
                this.radioButton2.Checked = false;
                this.radioButton3.Checked = false;
                this.raMySelf.Checked = false;

                this.comParaType.Enabled = false;
                this.comParaType.SelectedIndex = 0;
                this.txtGetStartPara.Enabled = false;
                this.txtGetEndPara.Enabled = false;
                this.UrlPara.Enabled = false;
                this.cmdInsertPara.Enabled = false;

                this.raXPath.Checked = true;
                this.txtXPath.Enabled = true;
                this.cmdXpathPara.Enabled = true;
                this.cmdVisual.Enabled = true;

                charSetMatch = Regex.Match(strNRule, "(?<=<XPath>).*?(?=</XPath>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();
                this.txtXPath.Text = strNRule;

                return;
            }
            //else if (strNRule.StartsWith("<CodePlugin>"))
            //{
            //    this.tabControl1.SelectedIndex = 3;

            //    this.groupBox2.Visible = true;
            //    this.groupBox3.Visible = false;
            //    this.groupBox4.Visible = false;

            //    //XPath可视化匹配
            //    this.txtGetStart.Enabled = false;
            //    this.txtGetEnd.Enabled = false;
            //    this.checkBox1.Enabled = false;
            //    this.checkBox2.Enabled = false;
            //    this.checkBox3.Enabled = false;
            //    this.checkBox4.Enabled = false;
            //    this.checkBox5.Enabled = false;
            //    this.checkBox6.Enabled = false;
            //    this.txtCustomLetter.Enabled = false;
            //    this.txtNRule.Enabled = false;

            //    this.radioButton4.Checked = false;
            //    this.radioButton1.Checked = false;
            //    this.radioButton2.Checked = false;
            //    this.radioButton3.Checked = false;
            //    this.raMySelf.Checked = false;

            //    charSetMatch = Regex.Match(strNRule, "(?<=<CodePlugin>).*?(?=</CodePlugin>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //    string str = charSetMatch.Groups[0].ToString();

            //    string[] ss = str.Split(',');
            //    //this.txtDMUrl.Enabled = true;
            //    //this.txtDMUrl.Text = ss[0].Split(':')[1];
            //    //this.isAutoDM.Enabled = true;
            //    //if (ss[1].Split(':')[1].ToLower() == "true")
            //    //{
            //    //    this.isAutoDM.Checked = true;
            //    //    this.txtDMPlugin.Enabled = true;
            //    //    this.cmdBrowserPlugins1.Enabled = true;
            //    //    this.cmdSetPlugins1.Enabled = true;
            //    //}
            //    //else
            //    //{
            //    //    this.isAutoDM.Checked = false;
            //    //    this.txtDMPlugin.Enabled = false;
            //    //    this.cmdBrowserPlugins1.Enabled = false;
            //    //    this.cmdSetPlugins1.Enabled = false;
            //    //}
            //    //this.txtDMFlag.Enabled = true;
            //    //this.txtDMPlugin.Text = ss[2].Split(':')[1];
            //    //this.txtDMFlag.Text = ss[3].Split(':')[1];

            //    return;
            //}

            this.comParaType.SelectedIndex = 0;

            this.txtGetStartPara.Enabled = false;
            this.txtGetEndPara.Enabled = false;

            this.txtNRule.Text = strNRule;
        }

        //private void GetNavRule(string strNavRule)
        //{
        //    m_NaviRule = strNavRule;
        //}


        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void IsNaviNextPage_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsNaviNextPage.Checked == true)
            {
                this.numMaxNaviNext.Enabled = true;
                this.cmdNextEndWildcard.Enabled = true;
                this.IsNaviNextDoPostBack.Enabled = true;

                this.groupBox6.Enabled = true;
                this.groupBox7.Enabled = true;
                this.groupBox8.Enabled = true;
                this.groupBox9.Enabled = true;
                this.radioButton5.Enabled = true;
                this.radioButton6.Enabled = true;
                this.radioButton7.Enabled = true;
                this.radioButton8.Enabled = true;
                this.raPara.Enabled = true;
                this.label8.Enabled = true;
                this.label9.Enabled = true;
            }
            else
            {
                this.numMaxNaviNext.Enabled = false;
                this.cmdNextEndWildcard.Enabled = false;
                this.IsNaviNextDoPostBack.Enabled = false;

                this.groupBox6.Enabled = false;
                this.groupBox7.Enabled = false;
                this.groupBox8.Enabled = false;
                this.groupBox9.Enabled = false;
                this.radioButton5.Enabled = false;
                this.radioButton6.Enabled = false;
                this.radioButton7.Enabled = false;
                this.radioButton8.Enabled = false;
                this.raPara.Enabled = false;
                this.label8.Enabled = false ;
                this.label9.Enabled = false ;
            }
        }

        private void frmAddNavPageRule_Load(object sender, EventArgs e)
        {
            //对Tooltip进行初始化设置
            HelpTip.ToolTipIcon = ToolTipIcon.Info;
            HelpTip.ForeColor = Color.YellowGreen;
            HelpTip.BackColor = Color.LightGray;
            HelpTip.AutoPopDelay = 20000;
            HelpTip.ShowAlways = true;
            HelpTip.ToolTipTitle = "小矿提示";

            SetHelpTip();

            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            if (Program.SominerVersion == cGlobalParas.VersionType.Free)
            {
                this.txtPrestr.Enabled = false;
                this.txtSufStr.Enabled = false;
                this.txtIncludeWord.Enabled = false;
                this.txtNoIncludeWord.Enabled = false;
                this.isRegex.Enabled = false;
                this.txtOldValue.Enabled = false;
                this.txtNewValue.Enabled = false;
            }

            this.comParaType.Items.Add("自定义参数");
            this.comParaType.Items.Add("自增变量{Num:1,10,1}");
            this.comParaType.Items.Add("时间戳{Timestamp:" + ToolUtil.GetTimestamp().ToString() + "}");
        }

        private void SetHelpTip()
        {
            HelpTip.SetToolTip(this.IsGather, @"选中此选择，则表示通过当前导航出来的页面需要采集数据，采集数据规则请在采集规则中配置，并选择对应到当前的导航层级");
            HelpTip.SetToolTip(this.txtNavStart, @"如果采集导航页数据，在此可以控制导航页的采集范围");
            HelpTip.SetToolTip(this.txtNavEnd, @"如果采集导航页数据，在此可以控制导航页的采集范围");
            HelpTip.SetToolTip(this.IsNaviNextPage, @"选中此选项可以配置此导航页面的翻页规则");
            HelpTip.SetToolTip(this.IsNaviNextDoPostBack, @"在翻页操作中，aspx.net有一种特有的翻页操作就是DoPostBack，" + "\r\n" + "当您将鼠标移动到翻页链接是，查看链接包含了此函数，则选中此选项");
            HelpTip.SetToolTip(this.cmdNextEndWildcard, @"点击此按钮进行翻页规则的设置");
            HelpTip.SetToolTip(this.numMaxNaviNext, @"默认情况下系统会根据翻页规则一直翻页至最后一页，" + "\r\n" + "您也可在此输入一个最大限制的翻页页码，来控制翻页的操作");

        }

        private void frmAddNavPageRule_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void IsGather_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            m_NaviRule = CreateNaviRule();

            if (string.IsNullOrEmpty( m_NaviRule))
            {
                MessageBox.Show(rm.GetString("Error36"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cNaviRuleFormConfig fRule = new cNaviRuleFormConfig();
            fRule.IsNext = false;
            fRule.NextRule ="";
            fRule.NextMaxPage ="0";
            fRule.IsNextDoPostBack =false;
            fRule.NavRule = m_NaviRule;
            fRule.SPos =this.txtNavStart.Text;
            fRule.EPos =this.txtNavEnd.Text;
            fRule.IsGather =this.IsGather.Checked;
            fRule.GSPos =    this.GStartPos.Text;
            fRule.GEPos =this.GEndPos.Text;
            fRule.IsNaviNext = this.IsNaviNextPage.Checked;
            if (this.IsNaviNextPage.Checked == true)
                m_NextRule = CreateNextRule();
            fRule.NaviNextRule = m_NextRule;
            fRule.NaviNextMaxPage =this.numMaxNaviNext.Value.ToString ();
            fRule.IsNaviNextDoPostBack =this.IsNaviNextDoPostBack .Checked ;

            if (raNarmal.Checked == true)
            {
                fRule.RunRule = cGlobalParas.NaviRunRule.Normal;
                fRule.OtherNaviRule = "";
            }
            else if (raSkip.Checked == true)
            {
                fRule.RunRule = cGlobalParas.NaviRunRule.Skip;
                fRule.OtherNaviRule = "";
            }
            else if (raRegex.Checked == true)
            {
                fRule.RunRule = cGlobalParas.NaviRunRule.Other;
                fRule.OtherNaviRule = this.txtNaviRegex.Text;
            }

           
            rNavRule(fState, fRule);

            this.Close();

        }

        private string CreateNaviRule()
        {
            string str = "";
            if (this.txtPrestr.Text.Trim() != "")
                str = "<Prefix>" + this.txtPrestr.Text + "</Prefix>";

            if (this.radioButton1.Checked == true)
            {
                str += "<Trait>" + this.txtNRule.Text + "</Trait>";
            }
            else if (this.radioButton2.Checked == true)
            {
                str += "<Common>" + this.txtNRule.Text + "</Common>";
            }
            else if (this.radioButton3.Checked == true)
            {
                str += "<Regex>" + this.txtNRule.Text + "</Regex>";
            }
            else if (this.raMySelf.Checked == true)
            {
                str += "<MySelf>" + this.txtNRule.Text + "</MySelf>";
            }
            else if (this.radioButton4.Checked == true)
            {
                str = "<ParaNavi>" + this.UrlPara.Text + "</ParaNavi>";
                return str;
            }
            else if (this.raXPath.Checked == true)
            {
                str = "<XPath>" + this.txtXPath.Text + "</XPath>";
                return str;
            }
            //else if (this.radioButton9.Checked == true)
            //{
            //    str = "<CodePlugin>[Url:" + this.txtDMUrl.Text  + ",Auto:" + this.isAutoDM.Checked.ToString () + ",Plugin:" + this.txtDMPlugin.Text + ",Flag:" 
            //        + this.txtDMFlag.Text  +  "</CodePlugin>";
            //}

            //str += this.txtNRule.Text;

            if (this.txtSufStr.Text.Trim() != "")
                str += "<Suffix>" + this.txtSufStr.Text + "</Suffix>";

            if (this.txtIncludeWord.Text.Trim() != "")
                str += "<Include>" + this.txtIncludeWord.Text + "</Include>";

            if (this.txtNoIncludeWord.Text.Trim() != "")
                str += "<NoInclude>" + this.txtNoIncludeWord.Text + "</NoInclude>";

            if (this.txtOldValue.Text.Trim() != "")
            {
                if (this.isRegex.Checked == true)
                {
                    str += "<RegexReplace>" + "<OldValue:" + this.txtOldValue.Text + "><NewValue:" + this.txtNewValue.Text + "></RegexReplace>";
                }
                else
                {
                    str += "<Replace>" + "<OldValue:" + this.txtOldValue.Text + "><NewValue:" + this.txtNewValue.Text + "></Replace>";
                }
            }

            return str;
            
        }

        private string CreateNextRule()
        {
            string str = "";
            if (this.txtNextPrestr.Text.Trim() != "")
                str = "<Prefix>" + this.txtNextPrestr.Text + "</Prefix>";

            if (this.radioButton7.Checked == true)
            {
                str += "<Trait>" + this.txtNextRule.Text + "</Trait>";
            }
            else if (this.radioButton6.Checked == true)
            {
                str += "<Common>" + this.txtNextRule.Text + "</Common>";
            }
            else if (this.radioButton5.Checked == true)
            {
                str += "<Regex>" + this.txtNextRule.Text + "</Regex>";
            }
            else if (this.radioButton8.Checked == true)
            {
                str += "<XPath>" + this.txtNextXPath.Text + "</XPath>";
            }
            else if (this.raPara.Checked == true)
            {
                str += "<Parameter>" + this.txtNextParaUrl.Text + "</Parameter>";
            }

            //str += this.txtNRule.Text;

            if (this.txtNextSufStr.Text.Trim() != "")
                str += "<Suffix>" + this.txtNextSufStr.Text + "</Suffix>";


            if (this.txtNextIncludeWord.Text.Trim() != "")
                str += "<Include>" + this.txtNextIncludeWord.Text + "</Include>";

            if (this.txtNextNoIncludeWord.Text.Trim() != "")
                str += "<NoInclude>" + this.txtNextNoIncludeWord.Text + "</NoInclude>";

            if (this.txtNextOldValue.Text.Trim() != "")
            {
                if (this.isRegex.Checked == true)
                {
                    str += "<RegexReplace>" + "<OldValue:" + this.txtNextOldValue.Text + "><NewValue:" + this.txtNextNewValue.Text + "></RegexReplace>";
                }
                else
                {
                    str += "<Replace>" + "<OldValue:" + this.txtNextOldValue.Text + "><NewValue:" + this.txtNextNewValue.Text + "></Replace>";
                }
            }

            return str;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //frmAddNextRule f = new frmAddNextRule(this.txtNaviNextPage.Text );
            //f.TestUrl = this.TestUrl;
            //f.Cookie = this.m_Cookie;
            //f.rNavRule = new frmAddNextRule.ReturnNavRule(GetNavRule2);
            //f.ShowDialog();
            //f.Dispose();
        }

        private void GetNavRule2(string strNavRule)
        {
            //this.txtNaviNextPage.Text = strNavRule;
        }

        private void IsNaviNextDoPostBack_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
               Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
               Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
            }
            else
            {
                MessageBox.Show("当前版本不支持__doPostBack函数解析，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.IsNaviNextDoPostBack.Checked = false;
                return;
            }
        }

        private void IsGather_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion != cGlobalParas.VersionType.Free )
            {
                if (this.IsGather.Checked == true)
                {
                    this.GStartPos.Enabled = true;
                    this.GEndPos.Enabled = true;
                }
                else
                {
                    this.GStartPos.Enabled = false;
                    this.GEndPos.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("当前版本不支持跨层采集，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.IsGather.Checked = false;
                return;
            }
        }

        private void raNarmal_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raNarmal.Checked == true)
                this.txtNaviRegex.Enabled = false;
        }

        private void raSkip_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raSkip.Checked == true)
                this.txtNaviRegex.Enabled = false;
        }

        private void raRegex_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raRegex.Checked == true)
                this.txtNaviRegex.Enabled = true ;
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            this.groupBox2.Visible = true;
            this.groupBox3.Visible = false;
            this.groupBox4.Visible = false;

            this.txtNavStart.Enabled = true ;
            this.txtNavEnd.Enabled = true ;
            this.txtGetStart.Enabled = false;
            this.txtGetEnd.Enabled = false;
            this.checkBox1.Enabled = false;
            this.checkBox2.Enabled = false;
            this.checkBox3.Enabled = false;
            this.checkBox4.Enabled = false;
            this.checkBox5.Enabled = false;
            this.checkBox6.Enabled = false;
            this.txtCustomLetter.Enabled = false;

            this.txtNRule.Enabled = true;

            this.label2.Text = "填写网址的相同部分即可自动识别导航网址";
            this.label12.Text = "导航规则";
            this.txtNRule.Text = "";

            this.radioButton4.Checked = false;
            this.comParaType.Enabled = false;
            this.txtGetStartPara.Enabled = false;
            this.txtGetEndPara.Enabled = false;
            this.UrlPara.Enabled = false;
            this.cmdInsertPara.Enabled = false;

            this.raXPath.Checked = false;
            this.txtXPath.Enabled = false;
            this.cmdXpathPara.Enabled = false;
            this.cmdVisual.Enabled = false;

            this.cmdNextEndWildcard.Enabled = false;

            //this.txtDMUrl.Enabled = false;
            //this.txtDMPlugin.Enabled = false;
            //this.txtDMFlag.Enabled = false;
            //this.cmdBrowserPlugins1.Enabled = false;
            //this.cmdSetPlugins1.Enabled = false;
            //this.isAutoDM.Enabled = false;

        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            this.groupBox2.Visible = true;
            this.groupBox3.Visible = false;
            this.groupBox4.Visible = false;

            this.txtNavStart.Enabled = true;
            this.txtNavEnd.Enabled = true;
            this.txtGetStart.Enabled = true;
            this.txtGetEnd.Enabled = true;
            this.checkBox1.Enabled = true;
            this.checkBox2.Enabled = true;
            this.checkBox3.Enabled = true;
            this.checkBox4.Enabled = true;
            this.checkBox5.Enabled = true;
            this.checkBox6.Enabled = true;
            this.txtCustomLetter.Enabled = true;
            this.checkBox1.Checked = true;

            this.txtNRule.Enabled = false;

            this.label2.Text = "根据网址的起始标志和结束标志匹配导航网址";
            this.label12.Text = "导航规则";
            this.txtNRule.Text = "";

            this.radioButton4.Checked = false;
            this.comParaType.Enabled = false;
            this.txtGetStartPara.Enabled = false;
            this.txtGetEndPara.Enabled = false;
            this.UrlPara.Enabled = false;
            this.cmdInsertPara.Enabled = false;

            this.raXPath.Checked = false;
            this.txtXPath.Enabled = false;
            this.cmdXpathPara.Enabled = false;
            this.cmdVisual.Enabled = false;

            this.cmdNextEndWildcard.Enabled = false;

            //this.radioButton9.Checked = false;
            //this.txtDMUrl.Enabled = false;
            //this.txtDMPlugin.Enabled = false;
            //this.txtDMFlag.Enabled = false;
            //this.cmdBrowserPlugins1.Enabled = false;
            //this.cmdSetPlugins1.Enabled = false;
            //this.isAutoDM.Enabled = false;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            this.groupBox2.Visible = true;
            this.groupBox3.Visible = false;
            this.groupBox4.Visible = false;

            this.txtNavStart.Enabled = true;
            this.txtNavEnd.Enabled = true;
            this.txtGetStart.Enabled = false;
            this.txtGetEnd.Enabled = false;
            this.checkBox1.Enabled = false;
            this.checkBox2.Enabled = false;
            this.checkBox3.Enabled = false;
            this.checkBox4.Enabled = false;
            this.checkBox5.Enabled = false;
            this.checkBox6.Enabled = false;
            this.txtCustomLetter.Enabled = false;

            this.txtNRule.Enabled = true;

            this.label2.Text = "使用正则表达式匹配导航网址适合专业用户使用";
            this.label12.Text = "正则表达式";
            this.txtNRule.Text = "";

            this.radioButton4.Checked = false;
            this.comParaType.Enabled = false;
            this.txtGetStartPara.Enabled = false;
            this.txtGetEndPara.Enabled = false;
            this.UrlPara.Enabled = false;
            this.cmdInsertPara.Enabled = false;

            this.raXPath.Checked = false;
            this.txtXPath.Enabled = false;
            this.cmdXpathPara.Enabled = false;
            this.cmdVisual.Enabled = false;

            this.cmdNextEndWildcard.Enabled = false;

            //this.radioButton9.Checked = false;
            //this.txtDMUrl.Enabled = false;
            //this.txtDMPlugin.Enabled = false;
            //this.txtDMFlag.Enabled = false;
            //this.cmdBrowserPlugins1.Enabled = false;
            //this.cmdSetPlugins1.Enabled = false;
            //this.isAutoDM.Enabled = false;
        }

        private void raMySelf_Click(object sender, EventArgs e)
        {
            this.groupBox2.Visible = true;
            this.groupBox3.Visible = false;
            this.groupBox4.Visible = false;

            this.txtNavStart.Enabled = false;
            this.txtNavEnd.Enabled = false;
            this.txtGetStart.Enabled = false;
            this.txtGetEnd.Enabled = false;
            this.checkBox1.Enabled = false;
            this.checkBox2.Enabled = false;
            this.checkBox3.Enabled = false;
            this.checkBox4.Enabled = false;
            this.checkBox5.Enabled = false;
            this.checkBox6.Enabled = false;
            this.txtCustomLetter.Enabled = false;

            this.txtNRule.Enabled = true ;

            this.label2.Text = "表示导航出的页面还是当前页面，这样做是为了实现一个页面中一对多数据关系的采集，"
                    + "通常选择此选项是需要配置级联采集规则";
            this.label12.Text = "循环标记";
            this.txtNRule.Text = "";

            this.radioButton4.Checked = false;
            this.comParaType.Enabled = false;
            this.txtGetStartPara.Enabled = false;
            this.txtGetEndPara.Enabled = false;
            this.UrlPara.Enabled = false;
            this.cmdInsertPara.Enabled = false;

            this.raXPath.Checked = false;
            this.txtXPath.Enabled = false;
            this.cmdXpathPara.Enabled = false;
            this.cmdVisual.Enabled = false;

            this.cmdNextEndWildcard.Enabled = false;

            //this.radioButton9.Checked = false;
            //this.txtDMUrl.Enabled = false;
            //this.txtDMPlugin.Enabled = false;
            //this.txtDMFlag.Enabled = false;
            //this.cmdBrowserPlugins1.Enabled = false;
            //this.cmdSetPlugins1.Enabled = false;
            //this.isAutoDM.Enabled = false;
        }

        private void raXPath_Click(object sender, EventArgs e)
        {
            this.groupBox2.Visible = false ;
            this.groupBox3.Visible = true ;
            this.groupBox4.Visible = false;

            this.txtNavStart.Enabled = false ;
            this.txtNavEnd.Enabled = false ;
            this.txtGetStart.Enabled = false;
            this.txtGetEnd.Enabled = false;
            this.checkBox1.Enabled = false;
            this.checkBox2.Enabled = false;
            this.checkBox3.Enabled = false;
            this.checkBox4.Enabled = false;
            this.checkBox5.Enabled = false;
            this.checkBox6.Enabled = false;
            this.txtCustomLetter.Enabled = false;
            this.txtNRule.Enabled = false;

            this.radioButton4.Checked = false;
            this.radioButton1.Checked = false;
            this.radioButton2.Checked = false;
            this.radioButton3.Checked = false;
            this.raMySelf.Checked = false;

            this.comParaType.Enabled = false;
            this.comParaType.SelectedIndex = 0;
            this.txtGetStartPara.Enabled = false;
            this.txtGetEndPara.Enabled = false;
            this.UrlPara.Enabled = false;
            this.cmdInsertPara.Enabled = false;

            this.raXPath.Checked = true;
            this.txtXPath.Enabled = true;
            this.cmdXpathPara.Enabled = true;
            this.cmdVisual.Enabled = true;

            this.cmdNextEndWildcard.Enabled = false;

            //this.radioButton9.Checked = false;
            //this.txtDMUrl.Enabled = false;
            //this.txtDMPlugin.Enabled = false;
            //this.txtDMFlag.Enabled = false;
            //this.cmdBrowserPlugins1.Enabled = false;
            //this.cmdSetPlugins1.Enabled = false;
            //this.isAutoDM.Enabled = false;
        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
               Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
               Program.SominerVersion == cGlobalParas.VersionType.Enterprise )
            {

                this.groupBox2.Visible = false ;
                this.groupBox3.Visible = false;
                this.groupBox4.Visible = true ;

                this.txtNavStart.Enabled = false ;
                this.txtNavEnd.Enabled = false ;
                this.txtGetStart.Enabled = false;
                this.txtGetEnd.Enabled = false;
                this.checkBox1.Enabled = false;
                this.checkBox2.Enabled = false;
                this.checkBox3.Enabled = false;
                this.checkBox4.Enabled = false;
                this.checkBox5.Enabled = false;
                this.checkBox6.Enabled = false;
                this.txtCustomLetter.Enabled = false;
                this.txtNRule.Enabled = false;

                this.radioButton4.Checked = true;
                this.radioButton1.Checked = false;
                this.radioButton2.Checked = false;
                this.radioButton3.Checked = false;
                this.raMySelf.Checked = false;

                this.comParaType.Enabled = true;
                this.comParaType.SelectedIndex = 0;
                this.txtGetStartPara.Enabled = true;
                this.txtGetEndPara.Enabled = true;
                this.UrlPara.Enabled = true;
                this.cmdInsertPara.Enabled = true;

                this.raXPath.Checked = false;
                this.txtXPath.Enabled = false;
                this.cmdXpathPara.Enabled = false;
                this.cmdVisual.Enabled = false;

                this.cmdNextEndWildcard.Enabled = true;

                //this.radioButton9.Checked = false;
                //this.txtDMUrl.Enabled = false;
                //this.txtDMPlugin.Enabled = false;
                //this.txtDMFlag.Enabled = false;
                //this.cmdBrowserPlugins1.Enabled = false;
                //this.cmdSetPlugins1.Enabled = false;
                //this.isAutoDM.Enabled = false;
            }
            else
            {
                MessageBox.Show("当前版本不支持导航参数配置，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.radioButton4.Checked = false;
                return;
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Show(this.button1, 0, 21);
        }

        private void cmdVisual_Click(object sender, EventArgs e)
        {
            frmVisual f = new frmVisual(this.m_TestUrl, this.m_Cookie);

            f.rxPath = this.GetXPath;
            f.ShowDialog();
            f.Dispose();
        }

        private void GetXPath(string xPath, string hNodeText)
        {
            this.txtXPath.Text = xPath;
        }

        private void cmdXpathPara_Click(object sender, EventArgs e)
        {
            int startPos = this.txtXPath.SelectionStart;
            int l = this.txtXPath.SelectionLength;

            string strPara = "{Num:1,10,1}";

            this.txtXPath.Text = this.txtXPath.Text.Substring(0, startPos) + strPara + this.txtXPath.Text.Substring(startPos + l, this.txtXPath.Text.Length - startPos - l);

            this.txtXPath.SelectionStart = startPos + strPara.Length;
            this.txtXPath.ScrollToCaret();
        }

        private void label37_Click(object sender, EventArgs e)
        {

        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
       
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void raPara_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void radioButton7_Click(object sender, EventArgs e)
        {
            if (radioButton7.Checked == true)
            {
                this.groupBox6.Visible = true;
                this.groupBox7.Visible = false;
                this.groupBox8.Visible = false;

                this.txtNextGetStart.Enabled = false;
                this.txtNextGetEnd.Enabled = false;
                this.checkBox7.Enabled = false;
                this.checkBox8.Enabled = false;
                this.checkBox9.Enabled = false;
                this.checkBox10.Enabled = false;
                this.checkBox11.Enabled = false;
                this.checkBox12.Enabled = false;
                this.txtNextCustomLetter.Enabled = false;

                this.txtNextRule.Enabled = true;

                //this.label2.Text = "填写翻页的标签即可";
                this.txtNextRule.Text = "";

            }
        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            if (radioButton6.Checked == true)
            {
                this.groupBox6.Visible = true;
                this.groupBox7.Visible = false;
                this.groupBox8.Visible = false;

                this.txtNextGetStart.Enabled = true;
                this.txtNextGetEnd.Enabled = true;
                this.checkBox7.Enabled = true;
                this.checkBox8.Enabled = true;
                this.checkBox9.Enabled = true;
                this.checkBox10.Enabled = true;
                this.checkBox11.Enabled = true;
                this.checkBox12.Enabled = true;
                this.txtNextCustomLetter.Enabled = true;
                this.checkBox12.Checked = true;

                this.txtNextRule.Enabled = false;

                this.txtNextRule.Text = "";

            }
        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            if (radioButton5.Checked == true)
            {
                this.groupBox6.Visible = true;
                this.groupBox7.Visible = false;
                this.groupBox8.Visible = false;

                this.txtNextGetStart.Enabled = false;
                this.txtNextGetEnd.Enabled = false;
                this.checkBox7.Enabled = false;
                this.checkBox8.Enabled = false;
                this.checkBox9.Enabled = false;
                this.checkBox10.Enabled = false;
                this.checkBox11.Enabled = false;
                this.checkBox12.Enabled = false;
                this.txtNextCustomLetter.Enabled = false;

                this.txtNextRule.Enabled = true;

                //this.label2.Text = "使用正则表达式匹配下一页网址适合专业用户使用";
                this.txtNextRule.Text = "";
            }
        }

        private void radioButton8_Click(object sender, EventArgs e)
        {
            if (radioButton8.Checked == true)
            {
                this.groupBox6.Visible = false;
                this.groupBox7.Visible = true;
                this.groupBox8.Visible = false;
            }
        }

        private void raPara_Click(object sender, EventArgs e)
        {
            if (raPara.Checked == true)
            {
                this.groupBox6.Visible = false;
                this.groupBox7.Visible = false;
                this.groupBox8.Visible = true;
            }
        }

        private void cmdNextStartWildcard_Click(object sender, EventArgs e)
        {
            this.rmenuNextStartWildcard.Show(this.cmdNextStartWildcard, 0, 42);
        }

        private void cmdNextEndWildcard_Click(object sender, EventArgs e)
        {
            this.rmenuNextEndWildcard.Show(this.cmdNextEndWildcard, 0, 42);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip4.Show(this.button6, 0, 21);
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void txtGetStart_TextChanged(object sender, EventArgs e)
        {
            CreateNaviRule1();
        }

        private void CreateNaviRule1()
        {
            string reg = @"(?<=";

            this.txtGetStart.Text = ToolUtil.ClearFlag(this.txtGetStart.Text);
            //this.txtGetStart.SelectionStart = this.txtGetStart.Text.Length;
            //this.txtGetStart.ScrollToCaret();

            string s = ToolUtil.RegexReplaceTrans(txtGetStart.Text, false);

            s = Regex.Replace(s, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            s = s.Replace(@"\r\n", "");

            this.txtGetEnd.Text = ToolUtil.ClearFlag(this.txtGetEnd.Text);
            //this.txtGetEnd.SelectionStart = this.txtGetEnd.Text.Length;
            //this.txtGetEnd.ScrollToCaret();
            string e = ToolUtil.RegexReplaceTrans(txtGetEnd.Text, false);

            e = Regex.Replace(e, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            e = e.Replace(@"\r\n", "");

            string m = string.Empty;

            if (this.checkBox1.Checked == true)
            {
                if (m != "")
                {
                    m += "|^>";
                }
                else
                {
                    m += "^>";
                }
            }
            if (this.checkBox2.Checked == true)
            {
                if (m != "")
                {
                    m += @"|^\s";
                }
                else
                {
                    m += @"^\s";
                }
            }
            if (this.checkBox3.Checked == true)
            {
                if (m != "")
                {
                    m += "|^#";
                }
                else
                {
                    m += "^#";
                }
            }
            if (this.checkBox4.Checked == true)
            {
                if (m != "")
                {
                    m += "|^'";
                }
                else
                {
                    m += "^'";
                }
            }
            if (this.checkBox5.Checked == true)
            {
                if (m != "")
                {
                    m += "|^\"";
                }
                else
                {
                    m += "^\"";
                }
            }
            if (this.checkBox6.Checked == true)
            {
                if (m != "")
                {
                    m += "|^" + this.txtCustomLetter.Text;
                }
                else
                {
                    m += "^" + this.txtCustomLetter.Text;
                }
            }
            if (m == string.Empty)
            {
                m = ".";
            }

            reg += s + ")[" + m + "]+?(?=";
            reg += e + ")";

            this.txtNRule.Text = reg;
        }

        private void txtGetEnd_TextChanged(object sender, EventArgs e)
        {
            CreateNaviRule1();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CreateNaviRule1();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            CreateNaviRule1();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            CreateNaviRule1();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            CreateNaviRule1();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            CreateNaviRule1();
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            this.txtCustomLetter.Text = ">";
            CreateNaviRule1();
        }

        private void txtCustomLetter_TextChanged(object sender, EventArgs e)
        {
            CreateNaviRule1();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void raXPath_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comParaType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comParaType.SelectedIndex == 0)
            {
                this.txtGetStartPara.Enabled = true;
                this.txtGetEndPara.Enabled = true;
            }
            else if (this.comParaType.SelectedIndex == 1)
            {
                this.txtGetStartPara.Enabled = false;
                this.txtGetEndPara.Enabled = false;
            }
            else if (this.comParaType.SelectedIndex == 2)
            {
                this.txtGetStartPara.Enabled = false;
                this.txtGetEndPara.Enabled = false;
            }
        }

        private void cmdInsertPara_Click(object sender, EventArgs e)
        {
            int startPos = this.UrlPara.SelectionStart;
            int l = this.UrlPara.SelectionLength;

            if (this.comParaType.SelectedIndex == 0)
            {
                if (this.txtGetStartPara.Text.Trim() == "")
                {
                    MessageBox.Show("需要输入参数捕获的起始位置！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.txtGetStartPara.Focus();
                    return;
                }

                if (this.txtGetEndPara.Text.Trim() == "")
                {
                    MessageBox.Show("需要输入参数捕获的终止位置！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.txtGetEndPara.Focus();
                    return;
                }

                string reg = @"(?<=";

                string sstr = Regex.Escape(ToolUtil.ClearFlag(this.txtGetStartPara.Text));
                string estr = Regex.Escape(ToolUtil.ClearFlag(this.txtGetEndPara.Text));

                reg += sstr + ")[^>]+?(?=";
                reg += estr + ")";

                this.UrlPara.Text = this.UrlPara.Text.Substring(0, startPos) + "<Para>" + reg + "</Para>"
                    + this.UrlPara.Text.Substring(startPos + l, this.UrlPara.Text.Length - startPos - l);

                this.UrlPara.SelectionStart = startPos + reg.Length;
                this.UrlPara.ScrollToCaret();

                this.txtGetStartPara.Text = "";
                this.txtGetEndPara.Text = "";

            }
            else if (this.comParaType.SelectedIndex == 1)
            {
                Match s;

                string str = this.comParaType.Text;

                if (Regex.IsMatch(str, "[{].*[}]"))
                {
                    s = Regex.Match(str, "[{].*[}]");
                }
                else
                {
                    s = Regex.Match(str, "[<].*[>]");
                }

                this.UrlPara.Text = this.UrlPara.Text.Substring(0, startPos) + s.Groups[0].Value + this.UrlPara.Text.Substring(startPos + l, this.UrlPara.Text.Length - startPos - l);

                this.UrlPara.SelectionStart = startPos + s.Groups[0].Value.Length;
                this.UrlPara.ScrollToCaret();
            }
            else if (this.comParaType.SelectedIndex == 2)
            {
                Match s;

                string str = this.comParaType.Text;

                if (Regex.IsMatch(str, "[{].*[}]"))
                {
                    s = Regex.Match(str, "[{].*[}]");
                }
                else
                {
                    s = Regex.Match(str, "[<].*[>]");
                }

                this.UrlPara.Text = this.UrlPara.Text.Substring(0, startPos) + s.Groups[0].Value + this.UrlPara.Text.Substring(startPos + l, this.UrlPara.Text.Length - startPos - l);

                this.UrlPara.SelectionStart = startPos + s.Groups[0].Value.Length;
                this.UrlPara.ScrollToCaret();
            }
        }

        private void menuFormValue_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "rmenuGetPostData")
            {
                SoukeyControl.WebBrowser.frmBrowser wftm = new SoukeyControl.WebBrowser.frmBrowser();
                wftm.getFlag = 1;
                wftm.rPData = new SoukeyControl.WebBrowser.frmBrowser.ReturnPOST(GetPData);
                wftm.ShowDialog();
                wftm.Dispose();

                return;
            }
            
            Match s;
            string uPara = string.Empty;

            if (Regex.IsMatch(e.ClickedItem.ToString(), "[{].*[}]"))
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[{].*[}]");

                string str = s.Value.ToString();

                if (!str.StartsWith("{Dict", StringComparison.CurrentCultureIgnoreCase) &&
                    !str.StartsWith("{CurrentDate", StringComparison.CurrentCultureIgnoreCase) &&
                    !str.StartsWith("{Timestamp", StringComparison.CurrentCultureIgnoreCase) &&
                    !str.StartsWith("{Refer", StringComparison.CurrentCultureIgnoreCase) &&
                    !str.StartsWith("{UrlValue", StringComparison.CurrentCultureIgnoreCase) &&
                    !str.StartsWith ("{FormValue",StringComparison.CurrentCultureIgnoreCase) &&
                    !str.StartsWith("{Script", StringComparison.CurrentCultureIgnoreCase) &&
                    !str.StartsWith("{CodeValue", StringComparison.CurrentCultureIgnoreCase))
                {
                    frmAddUrlPara f = new frmAddUrlPara(str);
                    f.rUrlPara = this.GetUrlPara;
                    if (f.ShowDialog() == DialogResult.Cancel)
                    {
                        f.Dispose();
                        return;
                    }
                    f.Dispose();
                    uPara = this.m_getUrlPara;
                }
                else
                    uPara = s.Groups[0].Value;

            }
            else
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[<].*[>]");
                uPara = s.Groups[0].Value;
            }

            string s1 = s.Groups[0].Value;
            if (s1.StartsWith("{Script"))
            {
                frmAddScript f = new frmAddScript();
                f.rScript = this.GetScript;
                f.ShowDialog();
                f.Dispose();
            }
            else if (s1.StartsWith("{CodeValue"))
            {
                frmAddCodePara f = new frmAddCodePara();
                f.rCodeRule = this.GetCodeValue;
                f.ShowDialog();
                f.Dispose();
            }
            else
            {
                int startPos = this.UrlPara.SelectionStart;
                int l = this.UrlPara.SelectionLength;

                this.UrlPara.Text = this.UrlPara.Text.Substring(0, startPos) + uPara + this.UrlPara.Text.Substring(startPos + l, this.UrlPara.Text.Length - startPos - l);

                this.UrlPara.SelectionStart = startPos + uPara.Length;
                this.UrlPara.ScrollToCaret();
            }
        }

        private void GetUrlPara(string str)
        {
            this.m_getUrlPara = str;
        }

        private void GetPData(string strCookie, string pData)
        {
            //this.txtCookie.Text = strCookie;
            this.UrlPara.Text += "<POST:ASCII>" + pData + "</POST>";
        }

        private void GetScript(string str)
        {
            str = str.Replace("{", "\\{").Replace("}", "\\}");
            string ss = "{Script:" + str + "}";
            int startPos = this.UrlPara.SelectionStart;
            int l = this.UrlPara.SelectionLength;

            this.UrlPara.Text = this.UrlPara.Text.Substring(0, startPos) + ss + this.UrlPara.Text.Substring(startPos + l, this.UrlPara.Text.Length - startPos - l);

            this.UrlPara.SelectionStart = startPos + ss.Length;
            this.UrlPara.ScrollToCaret();
        }

        private void GetCodeValue(string str)
        {
            str = str.Replace("{", "\\{").Replace("}", "\\}");
            string ss = "{CodeValue:" + str + "}";
            int startPos = this.UrlPara.SelectionStart;
            int l = this.UrlPara.SelectionLength;

            this.UrlPara.Text = this.UrlPara.Text.Substring(0, startPos) + ss + this.UrlPara.Text.Substring(startPos + l, this.UrlPara.Text.Length - startPos - l);

            this.UrlPara.SelectionStart = startPos + ss.Length;
            this.UrlPara.ScrollToCaret();
        }

        private void txtNextGetStart_TextChanged(object sender, EventArgs e)
        {
            CreateNextRule1();
        }

        private void CreateNextRule1()
        {
            string reg = @"(?<=";

            this.txtNextGetStart.Text = ToolUtil.ClearFlag(this.txtNextGetStart.Text);
            this.txtNextGetStart.SelectionStart = this.txtNextGetStart.Text.Length;
            this.txtNextGetStart.ScrollToCaret();

            string s = ToolUtil.RegexReplaceTrans(txtNextGetStart.Text, false);

            s = Regex.Replace(s, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            s = s.Replace(@"\r\n", "");

            this.txtNextGetEnd.Text = ToolUtil.ClearFlag(this.txtNextGetEnd.Text);
            this.txtNextGetEnd.SelectionStart = this.txtNextGetEnd.Text.Length;
            this.txtNextGetEnd.ScrollToCaret();

            string e = ToolUtil.RegexReplaceTrans(txtNextGetEnd.Text, false);

            e = Regex.Replace(e, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            e = e.Replace(@"\r\n", "");

            string m = string.Empty;

            if (this.checkBox12.Checked == true)
            {
                if (m != "")
                {
                    m += "|^>";
                }
                else
                {
                    m += "^>";
                }
            }
            if (this.checkBox11.Checked == true)
            {
                if (m != "")
                {
                    m += @"|^\s";
                }
                else
                {
                    m += @"^\s";
                }
            }
            if (this.checkBox10.Checked == true)
            {
                if (m != "")
                {
                    m += "|^#";
                }
                else
                {
                    m += "^#";
                }
            }
            if (this.checkBox9.Checked == true)
            {
                if (m != "")
                {
                    m += "|^'";
                }
                else
                {
                    m += "^'";
                }
            }
            if (this.checkBox8.Checked == true)
            {
                if (m != "")
                {
                    m += "|^\"";
                }
                else
                {
                    m += "^\"";
                }
            }
            if (this.checkBox7.Checked == true)
            {
                if (m != "")
                {
                    m += "|^" + this.txtNextCustomLetter.Text;
                }
                else
                {
                    m += "^" + this.txtNextCustomLetter.Text;
                }
            }
            if (m == string.Empty)
            {
                m = ".";
            }

            reg += s + ")[" + m + "]+?(?=";
            reg += e + ")";

            this.txtNextRule.Text = reg;
        }

        private void txtNextGetEnd_TextChanged(object sender, EventArgs e)
        {
            CreateNextRule1();
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            CreateNextRule1();
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            CreateNextRule1();
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            CreateNextRule1();
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            CreateNextRule1();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            CreateNextRule1();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            CreateNextRule1();
        }

        private void txtNextCustomLetter_TextChanged(object sender, EventArgs e)
        {
            CreateNextRule1();
        }

        private void contextMenuStrip4_Opening(object sender, CancelEventArgs e)
        {
            this.menuTimestamp.Text = "时间戳{Timestamp:" + ToolUtil.GetTimestamp().ToString() + "}";
        }

        private void contextMenuStrip4_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Match s;

            if (Regex.IsMatch(e.ClickedItem.ToString(), "[{].*[}]"))
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[{].*[}]");
            }
            else
            {
                s = Regex.Match(e.ClickedItem.ToString(), "[<].*[>]");
            }

            int startPos = this.txtNextParaUrl.SelectionStart;
            int l = this.txtNextParaUrl.SelectionLength;

            this.txtNextParaUrl.Text = this.txtNextParaUrl.Text.Substring(0, startPos) + s.Groups[0].Value + this.txtNextParaUrl.Text.Substring(startPos + l, this.txtNextParaUrl.Text.Length - startPos - l);

            this.txtNextParaUrl.SelectionStart = startPos + s.Groups[0].Value.Length;
            this.txtNextParaUrl.ScrollToCaret();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int startPos = this.txtNextParaUrl.SelectionStart;
            int l = this.txtNextParaUrl.SelectionLength;


            if (this.txtNextGetStartPara.Text.Trim() == "")
            {
                MessageBox.Show("需要输入参数捕获的起始位置！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtNextGetStartPara.Focus();
                return;
            }

            if (this.txtNextGetEndPara.Text.Trim() == "")
            {
                MessageBox.Show("需要输入参数捕获的终止位置！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtNextGetEndPara.Focus();
                return;
            }

            string reg = @"(?<=";

            string sstr = Regex.Escape(ToolUtil.ClearFlag(this.txtNextGetStartPara.Text));
            string estr = Regex.Escape(ToolUtil.ClearFlag(this.txtNextGetEndPara.Text));

            reg += sstr + ")[^>]+?(?=";
            reg += estr + ")";

            this.txtNextParaUrl.Text = this.txtNextParaUrl.Text.Substring(0, startPos) + "<Para>" + reg + "</Para>"
                + this.txtNextParaUrl.Text.Substring(startPos + l, this.txtNextParaUrl.Text.Length - startPos - l);

            this.txtNextParaUrl.SelectionStart = startPos + reg.Length;
            this.txtNextParaUrl.ScrollToCaret();

            this.txtNextGetStartPara.Text = "";
            this.txtNextGetEndPara.Text = "";
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

        private void InsertEndPosWildcard(string ss)
        {
            int startPos = this.txtGetEnd.SelectionStart;
            int l = this.txtGetEnd.SelectionLength;

            this.txtGetEnd.Text = this.txtGetEnd.Text.Substring(0, startPos)
                + ss + this.txtGetEnd.Text.Substring(startPos + l, this.txtGetEnd.Text.Length - startPos - l);

            this.txtGetEnd.SelectionStart = startPos + ss.Length;
            this.txtGetEnd.ScrollToCaret();
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

        private void InsertNextStartPosWildcard(string ss)
        {
            int startPos = this.txtNextGetStart.SelectionStart;
            int l = this.txtNextGetStart.SelectionLength;

            this.txtNextGetStart.Text = this.txtNextGetStart.Text.Substring(0, startPos)
                + ss + this.txtNextGetStart.Text.Substring(startPos + l, this.txtNextGetStart.Text.Length - startPos - l);

            this.txtNextGetStart.SelectionStart = startPos + ss.Length;
            this.txtNextGetStart.ScrollToCaret();
        }

        private void rmenuNextStartWildcardNum_Click(object sender, EventArgs e)
        {
            InsertNextStartPosWildcard(@"<Wildcard>(\d+)</Wildcard>");
        }

        private void rmenuNextStartWildcardLetter_Click(object sender, EventArgs e)
        {
            InsertNextStartPosWildcard(@"<Wildcard>(\w+?)</Wildcard>");
        }

        private void rmenuNextStartWildcardAny_Click(object sender, EventArgs e)
        {
            InsertNextStartPosWildcard(@"<Wildcard>(.+?)</Wildcard>");
        }

        private void rmenuNextStartWildcardRegex_Click(object sender, EventArgs e)
        {
            InsertNextStartPosWildcard(@"<Wildcard></Wildcard>");
        }

        private void InsertNextEndPosWildcard(string ss)
        {
            int startPos = this.txtNextGetEnd.SelectionStart;
            int l = this.txtNextGetEnd.SelectionLength;

            this.txtNextGetEnd.Text = this.txtNextGetEnd.Text.Substring(0, startPos)
                + ss + this.txtNextGetEnd.Text.Substring(startPos + l, this.txtNextGetEnd.Text.Length - startPos - l);

            this.txtNextGetEnd.SelectionStart = startPos + ss.Length;
            this.txtNextGetEnd.ScrollToCaret();
        }

        private void rmenuNextEndWildcardNum_Click(object sender, EventArgs e)
        {
            InsertNextEndPosWildcard(@"<Wildcard>(\d+)</Wildcard>");
        }

        private void rmenuNextEndWildcardLetter_Click(object sender, EventArgs e)
        {
            InsertNextEndPosWildcard(@"<Wildcard>(\w+?)</Wildcard>");
        }

        private void rmenuNextEndWildcardAny_Click(object sender, EventArgs e)
        {
            InsertNextEndPosWildcard(@"<Wildcard>(.+?)</Wildcard>");
        }

        private void rmenuNextEndWildcardRegex_Click(object sender, EventArgs e)
        {
            InsertNextEndPosWildcard(@"<Wildcard></Wildcard>");
        }

        private void txtNRule_Leave(object sender, EventArgs e)
        {
            this.txtNRule.Text = ToolUtil.ClearFlag(this.txtNRule.Text);
        }

        //private void radioButton9_Click(object sender, EventArgs e)
        //{
        //    if (Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
        //     Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
        //    {

        //        this.groupBox2.Visible = true;
        //        this.groupBox3.Visible = false;
        //        this.groupBox4.Visible = false;

        //        this.txtNavStart.Enabled = false;
        //        this.txtNavEnd.Enabled = false;
        //        this.txtGetStart.Enabled = false;
        //        this.txtGetEnd.Enabled = false;
        //        this.checkBox1.Enabled = false;
        //        this.checkBox2.Enabled = false;
        //        this.checkBox3.Enabled = false;
        //        this.checkBox4.Enabled = false;
        //        this.checkBox5.Enabled = false;
        //        this.checkBox6.Enabled = false;
        //        this.txtCustomLetter.Enabled = false;
        //        this.txtNRule.Enabled = false;

        //        this.radioButton4.Checked = false;
        //        this.radioButton1.Checked = false;
        //        this.radioButton2.Checked = false;
        //        this.radioButton3.Checked = false;
        //        this.raMySelf.Checked = false;

        //        this.comParaType.Enabled = false;
        //        this.comParaType.SelectedIndex = 0;
        //        this.txtGetStartPara.Enabled = false;
        //        this.txtGetEndPara.Enabled = false;
        //        this.UrlPara.Enabled = false;
        //        this.cmdInsertPara.Enabled = false;

        //        this.raXPath.Checked = false;
        //        this.txtXPath.Enabled = false;
        //        this.cmdXpathPara.Enabled = false;
        //        this.cmdVisual.Enabled = false;

        //        this.cmdNextEndWildcard.Enabled = false;

        //        this.radioButton9.Checked = true;
        //        this.txtDMUrl.Enabled = true;
                
        //        this.txtDMFlag.Enabled = true;
        //        this.cmdBrowserPlugins1.Enabled = true;
        //        this.cmdSetPlugins1.Enabled = true;
        //        this.isAutoDM.Enabled = true;
        //        this.isAutoDM.Checked = false;
        //        this.txtDMPlugin.Enabled = false;
        //        this.cmdBrowserPlugins1.Enabled = false ;
        //        this.cmdSetPlugins1.Enabled = false ;
        //    }
        //    else
        //    {
        //        MessageBox.Show("当前版本不支持打码导航，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }
        //}

        private void cmdBrowserPlugins1_Click(object sender, EventArgs e)
        {
        }

        private void cmdSetPlugins1_Click(object sender, EventArgs e)
        {
            
        }

        private void isAutoDM_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void menuCalcNum_Click(object sender, EventArgs e)
        {

        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Core.Dict;

namespace MinerSpider
{
    public partial class frmAddNavRules : Form
    {
        private ResourceManager rm;
        public delegate void ReturnNavRule(string NavRule);
        public ReturnNavRule rNavRule;
        private string m_TestUrl;
        private string m_Cookie;

        private bool m_isIniDict = false;
        private Dictionary<string, string> m_dictMenu;

        //定义一个ToolTip
        ToolTip HelpTip = new ToolTip();

        public frmAddNavRules()
        {
            InitializeComponent();

            this.comParaType.Items.Add("自定义参数");
            this.comParaType.Items.Add("自增变量{Num:1,10,1}");
            this.comParaType.Items.Add("时间戳{Timestamp:" + ToolUtil.GetTimestamp().ToString() + "}");
        }

        public frmAddNavRules(string TestUrl,string strNRule,string cookie)
        {
            InitializeComponent();

            this.m_TestUrl = TestUrl;
            this.m_Cookie = cookie;

            this.comParaType.Items.Add("自定义参数");
            this.comParaType.Items.Add("自增变量{Num:1,10,1}");
            this.comParaType.Items.Add("时间戳{Timestamp:" + ToolUtil.GetTimestamp().ToString() + "}");

            string nRule = strNRule;
            Match charSetMatch;

            if (Regex.IsMatch(strNRule, "<Prefix>"))
            {
                charSetMatch = Regex.Match(nRule, "(?<=<Prefix>).*?(?=</Prefix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.txtPrestr.Text = charSetMatch.Groups[0].ToString();

                //charSetMatch = Regex.Match(strNRule, "(?<=</Prefix>).*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //strNRule = charSetMatch.Groups[0].ToString();
                strNRule = Regex.Replace(strNRule, "<Prefix>.*?</Prefix>", "");
            }

            if (Regex.IsMatch(strNRule, "<Suffix>"))
            {
                charSetMatch = Regex.Match(nRule, "(?<=<Suffix>).*?(?=</Suffix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.txtSufStr.Text = charSetMatch.Groups[0].ToString();

                //charSetMatch = Regex.Match(strNRule, "(?<=</Suffix>).*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //strNRule = charSetMatch.Groups[0].ToString();
                strNRule = Regex.Replace(strNRule, "<Suffix>.*?</Suffix>", "");
            }

            if (Regex.IsMatch(strNRule, "<Include>"))
            {
                charSetMatch = Regex.Match(nRule, "(?<=<Include>).*?(?=</Include>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.txtIncludeWord.Text = charSetMatch.Groups[0].ToString();

                //charSetMatch = Regex.Match(strNRule, "(?<=</Include>).*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //strNRule = charSetMatch.Groups[0].ToString();
                strNRule = Regex.Replace(strNRule, "<Include>.*?</Include>", "");
            }

            if (Regex.IsMatch(strNRule, "<NoInclude>"))
            {
                charSetMatch = Regex.Match(nRule, "(?<=<NoInclude>).*?(?=</NoInclude>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.txtNoIncludeWord.Text = charSetMatch.Groups[0].ToString();

                //charSetMatch = Regex.Match(strNRule, "(?<=</NoInclude>).*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //strNRule = charSetMatch.Groups[0].ToString();
                strNRule = Regex.Replace(strNRule, "<NoInclude>.*?</NoInclude>", "");
            }

            if (Regex.IsMatch(strNRule, "<RegexReplace>"))
            {
                this.isRegex.Checked = true;

                charSetMatch = Regex.Match(nRule, "(?<=<RegexReplace>).*?(?=</RegexReplace>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string ss = charSetMatch.Groups[0].ToString();

                this.txtOldValue.Text = ss.Substring(ss.IndexOf("<OldValue:") + 10, ss.IndexOf("><NewValue:") - 10);
                this.txtNewValue.Text  = ss.Substring(ss.IndexOf("<NewValue:") + 10, ss.Length - ss.IndexOf("<NewValue:") - 11);

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

            if (strNRule.StartsWith("<Trait>") )
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
                string ss= charSetMatch.Groups[0].ToString();
                if (ss.StartsWith ("^"))
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
                        string s = ss.Substring(1, ss.Length -1);
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
                                break ;
                            case "\"":
                                this.checkBox5.Checked = true;
                                break ;
                            default :
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
            else if (strNRule.StartsWith("<MySelf/>"))
            {
                this.raMySelf.Checked = true;
                strNRule = "<MySelf/>";
            }
            else if (strNRule.StartsWith("<ParaNavi>"))
            {
                //参数导航 这部分处理的不是很好，后期有时间需要进行修改
                this.tabControl1.SelectedIndex = 2;

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

                this.button2.Enabled = true;

                charSetMatch = Regex.Match(strNRule, "(?<=<ParaNavi>).*?(?=</ParaNavi>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();
                this.UrlPara.Text = strNRule;

                return;
            }
            else if (strNRule.StartsWith("<XPath>"))
            {
                this.tabControl1.SelectedIndex = 1;

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

            this.comParaType.SelectedIndex = 0;

            this.txtGetStartPara.Enabled = false;
            this.txtGetEndPara.Enabled = false;

            this.txtNRule.Text = strNRule;

        }

        private void frmAddNavRules_Load(object sender, EventArgs e)
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
            
        }

        private void SetHelpTip()
        {
            HelpTip.SetToolTip(this.radioButton1, @"选择此项则输入需要导航网址的相同部分即可，注意：一定要从http://开始输入");
            HelpTip.SetToolTip(this.radioButton2, @"选中此选项则通过配置导航地址的前置和后置字符串来获取导航地址信息");
            HelpTip.SetToolTip(this.radioButton3, @"选中此选项则通过自定义正则表达式来获取导航地址信息");
            HelpTip.SetToolTip(this.raMySelf, @"选中此选项则当前页面导航进入当前页面，这样操作的目的是可以对当前页面的数据进行二次采集，实现数据最终的合并操作");
            HelpTip.SetToolTip(this.txtNRule, @"如果选择了只输入导航网址特征信息，则在此输入网址的特征信息");
            HelpTip.SetToolTip(this.txtGetStart, @"如果选择了自定义配置，则在此输入导航地址的前置字符串，建议为一个完整的标签");
            HelpTip.SetToolTip(this.txtGetEnd , @"如果选择了自定义配置，则在此输入导航地址的后置字符串，建议为一个完整的标签");
            HelpTip.SetToolTip(this.cmdStartWildcard, "点击此按钮，选择通配符类型，插入通配符");
            HelpTip.SetToolTip(this.cmdEndWildcard, "点击此按钮，选择通配符类型，插入通配符");
            HelpTip.SetToolTip(this.checkBox1, @"选中此选项，则匹配导航地址的时候，会剔除包含>的地址信息");
            HelpTip.SetToolTip(this.checkBox2, @"选中此选项，则匹配导航地址的时候，会剔除包含空格的地址信息");
            HelpTip.SetToolTip(this.checkBox3, @"选中此选项，则匹配导航地址的时候，会剔除包含#的地址信息");
            HelpTip.SetToolTip(this.checkBox4, @"选中此选项，则匹配导航地址的时候，会剔除包含'的地址信息");
            HelpTip.SetToolTip(this.checkBox5, "选中此选项，则匹配导航地址的时候，会剔除包含\"的地址信息");
            HelpTip.SetToolTip(this.checkBox6, @"选中此选项，则匹配导航地址的时候，会剔除右侧输入框指定字符的地址信息");
            HelpTip.SetToolTip(this.txtCustomLetter, @"在此输入需要剔除指定字符的地址信息");
            HelpTip.SetToolTip(this.txtIncludeWord, @"在此输入必须包含的字符串，根据规则匹配导航网址后，会自动删除不包含此字符串的地址信息");
            HelpTip.SetToolTip(this.txtNoIncludeWord, @"在此输入不能包含的字符串，根据规则匹配导航网址后，会自动删除包含此字符串的地址信息");
            HelpTip.SetToolTip(this.txtPrestr, @"在此输入需要在导航地址前加入的字符串");
            HelpTip.SetToolTip(this.txtSufStr, @"在此输入需要在导航地址后加入的字符串");
            HelpTip.SetToolTip(this.isRegex, @"如果选择替换导航网址的字符，选中此选项则启用正则替换模式");
            HelpTip.SetToolTip(this.txtOldValue, @"在此输入导航地址中需要替换的字符串或正则信息");
            HelpTip.SetToolTip(this.txtNewValue, @"在此输入替换的结果信息");
            HelpTip.SetToolTip(this.raXPath, @"选中此选项，则通过可视化的方法配置导航规则");
            HelpTip.SetToolTip(this.txtXPath, "在此输入可视化获取的XPath路径地址");
            HelpTip.SetToolTip(this.cmdVisual, "点击此按钮打开可视化捕获工具，来自动获取可视化捕获的XPath地址信息");
            HelpTip.SetToolTip(this.cmdXpathPara, "点击此按钮可插入数字参数");
            HelpTip.SetToolTip(this.radioButton4, "选中此选项，则采用模拟导航网址的方式进行导航规则配置，" + "\r\n" + "模拟导航网址，并通过配置参数来模拟网址参数实现导航");
            HelpTip.SetToolTip(this.comParaType, "在此选择插入网址参数的类型");
            HelpTip.SetToolTip(this.txtGetStartPara, @"如果选择了自定义参数，系统则在导航入口页面中进行参数的捕获，在此输入捕获参数的前置字符串，建议为一个完整的标签");
            HelpTip.SetToolTip(this.txtGetEndPara, @"如果选择了自定义参数，系统则在导航入口页面中进行参数的捕获，在此输入捕获参数的后置字符串，建议为一个完整的标签");
            HelpTip.SetToolTip(this.UrlPara, "在此输入模拟的导航地址，注意：支持POST方式，POST提交数据请用<POST:ASCII></POST>标签表明识别");
            HelpTip.SetToolTip(this.cmdInsertPara , "点击此按钮插入已经设置好的参数，插入的位置在导航网址当前所选中的位置");


        }

        private void frmAddNavRules_FontChanged(object sender, EventArgs e)
        {
            rm = null;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            string str = "";
            if (this.txtPrestr.Text.Trim ()!="")
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
                str += this.txtNRule.Text;
            }
            else if (this.radioButton4.Checked == true)
            {
                str = "<ParaNavi>" + this.UrlPara.Text + "</ParaNavi>";
                rNavRule(str);
                this.Close();
                return;
            }
            else if (this.raXPath.Checked == true)
            {
                str = "<XPath>" + this.txtXPath.Text + "</XPath>";
                rNavRule(str);
                this.Close();
                return;
            }

            //str += this.txtNRule.Text;

            if (this.txtSufStr.Text.Trim () != "")
                str += "<Suffix>" + this.txtSufStr.Text + "</Suffix>";

            if(this.txtIncludeWord.Text.Trim ()!="")
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

            rNavRule(str);
            this.Close();
        }

        private void frmAddNavRules_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
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

            this.label2.Text = "填写网址的相同部分即可自动识别导航网址";
            this.txtNRule.Text = "";

            this.radioButton4.Checked = false;
            this.comParaType.Enabled = false ;
            this.txtGetStartPara.Enabled = false ;
            this.txtGetEndPara.Enabled = false ;
            this.UrlPara.Enabled = false ;
            this.cmdInsertPara.Enabled = false ;

            this.raXPath.Checked = false;
            this.txtXPath.Enabled = false;
            this.cmdXpathPara.Enabled = false;
            this.cmdVisual.Enabled = false;

            this.button2.Enabled = false;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            this.txtGetStart.Enabled = true ;
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

            this.button2.Enabled = false;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
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

            this.label2.Text = "使用正则表达式匹配导航网址适合专业用户使用";
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

            this.button2.Enabled = false;

        }

        private void txtGetStart_TextChanged(object sender, EventArgs e)
        {
            CreateNaviRule();
        }

        private void CreateNaviRule()
        {
            string reg = @"(?<=";

            this.txtGetStart.Text = ToolUtil.ClearFlag(this.txtGetStart.Text);
            //this.txtGetStart.SelectionStart = this.txtGetStart.Text.Length;
            //this.txtGetStart.ScrollToCaret();

            string s = ToolUtil.RegexReplaceTrans (txtGetStart.Text,false );

            s = Regex.Replace(s, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            s = s.Replace(@"\r", "");
            s = s.Replace(@"\n", "");

            this.txtGetEnd.Text = ToolUtil.ClearFlag(this.txtGetEnd.Text);
            //this.txtGetEnd.SelectionStart = this.txtGetEnd.Text.Length;
            //this.txtGetEnd.ScrollToCaret();
            string e = ToolUtil.RegexReplaceTrans (txtGetEnd.Text,false );

            e = Regex.Replace(e, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            e = e.Replace(@"\r", "");
            e = e.Replace(@"\n", "");

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
                    m += "|^" + this.txtCustomLetter.Text ;
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
            CreateNaviRule();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CreateNaviRule();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            CreateNaviRule();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            CreateNaviRule();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            CreateNaviRule();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            CreateNaviRule();
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            CreateNaviRule();
        }

        private void txtCustomLetter_TextChanged(object sender, EventArgs e)
        {
            CreateNaviRule();
        }

        private void raMySelf_Click(object sender, EventArgs e)
        {
            this.txtGetStart.Enabled = false;
            this.txtGetEnd.Enabled = false;
            this.checkBox1.Enabled = false;
            this.checkBox2.Enabled = false;
            this.checkBox3.Enabled = false;
            this.checkBox4.Enabled = false;
            this.checkBox5.Enabled = false;
            this.checkBox6.Enabled = false;
            this.txtCustomLetter.Enabled = false;

            this.txtNRule.Enabled = false ;

            this.label2.Text = "表示导航出的页面还是当前页面，这样做是为了实现一个页面中一对多数据关系的采集，"
                    + "通常选择此选项是需要配置级联采集规则";
            this.txtNRule.Text = "<MySelf/>";

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

            this.button2.Enabled = false;
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
                this.txtGetStartPara.Enabled = false ;
                this.txtGetEndPara.Enabled = false;
            }
            else if (this.comParaType.SelectedIndex == 2)
            {
                this.txtGetStartPara.Enabled = false;
                this.txtGetEndPara.Enabled = false;
            }
        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
               Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
               Program.SominerVersion == cGlobalParas.VersionType.Enterprise )
            {

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

                this.button2.Enabled = true;
            }
            else
            {
                MessageBox.Show("当前版本不支持导航参数配置，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.radioButton4.Checked = false;
                return;
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

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void raXPath_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void raXPath_Click(object sender, EventArgs e)
        {
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

            this.button2.Enabled = false ;
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

        private void cmdVisual_Click(object sender, EventArgs e)
        {
            frmVisual f = new frmVisual(this.m_TestUrl,this.m_Cookie);
            
            f.rxPath = this.GetXPath;
            f.ShowDialog();
            f.Dispose();
        }

        private void GetXPath(string xPath, string hNodeText)
        {
            this.txtXPath.Text = xPath;
        }

        private void cmdStartWildcard_Click(object sender, EventArgs e)
        {
            this.rmenuStartWildcard.Show(this.cmdStartWildcard, 0, 42);
        }

        private void cmdEndWildcard_Click(object sender, EventArgs e)
        {
            this.rmenuEndWildcard.Show(this.cmdEndWildcard, 0, 42);
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

        private void UrlPara_Leave(object sender, EventArgs e)
        {
            this.UrlPara.Text = this.UrlPara.Text.Replace("\r\n", "");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Show(this.button2, 0, 21);
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
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

            int startPos = this.UrlPara.SelectionStart;
            int l = this.UrlPara.SelectionLength;

            this.UrlPara.Text = this.UrlPara.Text.Substring(0, startPos) + s.Groups[0].Value + this.UrlPara.Text.Substring(startPos + l, this.UrlPara.Text.Length - startPos - l);

            this.UrlPara.SelectionStart = startPos + s.Groups[0].Value.Length;
            this.UrlPara.ScrollToCaret();
        }

        private void menuFormValue_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

            this.contextMenuStrip1.Items.RemoveByKey("rmenuDict");


            //初始化字典菜单的项目
            oDict d = new oDict(Program.getPrjPath());

            IEnumerable<string> dClass = d.GetDictClass();
            
            ToolStripMenuItem dictMenu = new ToolStripMenuItem(rm.GetString("rmenu8"), null, null, "rmenuDict");
            dictMenu.DropDownItemClicked += this.contextMenuStrip1_ItemClicked;

            if (this.m_isIniDict)
            {
                foreach (KeyValuePair<string,string> s in m_dictMenu)
                {
                    dictMenu.DropDownItems.Add(rm.GetString("rmenu8") + ":{Dict:" + s.Value + "}");
                }
            }
            else
            {
                m_dictMenu = new Dictionary<string, string>();

                foreach (string s in dClass)
                {
                    dictMenu.DropDownItems.Add(rm.GetString("rmenu8") + ":{Dict:" + s + "}");
                    m_dictMenu.Add(s, s);
                }

                this.m_isIniDict = true;
            }

            this.contextMenuStrip1.Items.Add(dictMenu);
            
        }

        private void txtIncludeWord_TextChanged(object sender, EventArgs e)
        {

        }


    }
}
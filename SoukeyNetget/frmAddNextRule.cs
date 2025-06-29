using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Text.RegularExpressions;
using NetMiner.Common;
using NetMiner.Resource;

namespace MinerSpider
{
    public partial class frmAddNextRule : Form
    {
        private ResourceManager rm;
        public delegate void ReturnNavRule(string NavRule);
        public ReturnNavRule rNavRule;
        private string m_getUrlPara = string.Empty;

        //定义一个ToolTip
        ToolTip HelpTip = new ToolTip();

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

        public frmAddNextRule()
        {
            InitializeComponent();
        }

        public frmAddNextRule(string strNRule)
        {
            InitializeComponent();

            if (strNRule == "")
                return;

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

                //charSetMatch = Regex.Match(strNRule, ".*?(?=<Suffix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
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

            //提取参数
            if (Regex.IsMatch(strNRule, "<Parameter>"))
            {
                this.raPara.Checked = true;
                this.tabControl1.SelectedIndex = 2;
                SetPara();
                charSetMatch = Regex.Match(nRule, "(?<=<Parameter>)[\\s\\S]*?(?=</Parameter>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.txtUrl.Text = charSetMatch.Groups[0].ToString();

                charSetMatch = Regex.Match(strNRule, ".*?(?=<Parameter>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();
            }
            else if (strNRule.StartsWith("<Trait>"))
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

                //this.txtGetStart.Text = Regex.Unescape(starts);
                //this.txtGetEnd.Text = Regex.Unescape(ends);

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
            else if (strNRule.StartsWith("<XPath>"))
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

                this.radioButton1.Checked = false;
                this.radioButton2.Checked = false;
                this.radioButton3.Checked = false;
                this.raXPath.Checked = true;

                this.txtXPath.Enabled = true;
                this.cmdVisual.Enabled = true;

                this.tabControl1.SelectedIndex = 1;

                charSetMatch = Regex.Match(strNRule, "(?<=<XPath>).*?(?=</XPath>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();
                this.txtXPath.Text = strNRule;
                return;
            }

            this.txtNRule.Text = strNRule;
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

            this.txtNRule.Enabled = true;

            this.label2.Text = "填写翻页的标签即可";
            this.txtNRule.Text = "";

            this.radioButton1.Checked = true ;
           
            this.raXPath.Checked = false ;

            this.txtXPath.Enabled = false;
            this.cmdVisual.Enabled = false;

            this.raPara.Checked = false;
            this.label3.Enabled = false;
            this.txtUrl.Enabled = false;
            this.button2.Enabled = false;

            this.txtGetStartPara.Enabled = false;
            this.txtGetEndPara.Enabled = false;
            this.cmdInsertPara.Enabled = false;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
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

            this.label2.Text = "根据网址的起始标志和结束标志匹配下一页网址";
            this.txtNRule.Text = "";

            this.radioButton2.Checked = true;

            this.raXPath.Checked = false;

            this.txtXPath.Enabled = false;
            this.cmdVisual.Enabled = false;

            this.raPara.Checked = false;
            this.label3.Enabled = false;
            this.txtUrl.Enabled = false;
            this.button2.Enabled = false;

            this.txtGetStartPara.Enabled = false;
            this.txtGetEndPara.Enabled = false;
            this.cmdInsertPara.Enabled = false;
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

            this.txtNRule.Enabled = true;

            this.label2.Text = "使用正则表达式匹配下一页网址适合专业用户使用";
            this.txtNRule.Text = "";

            this.radioButton3.Checked = true;

            this.raXPath.Checked = false;

            this.txtXPath.Enabled = false;
            this.cmdVisual.Enabled = false;

            this.raPara.Checked = false;
            this.label3.Enabled = false;
            this.txtUrl.Enabled = false;
            this.button2.Enabled = false;

            this.txtGetStartPara.Enabled = false;
            this.txtGetEndPara.Enabled = false;
            this.cmdInsertPara.Enabled = false;
        }

        private void txtGetStart_TextChanged(object sender, EventArgs e)
        {
            CreateNaviRule();
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

    


        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            string str = "";
            if (this.txtPrestr.Text.Trim () != "")
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
            else if (this.raXPath.Checked == true)
            {
                str += "<XPath>" + this.txtXPath.Text + "</XPath>";
            }
            else if (this.raPara.Checked == true)
            {
                str += "<Parameter>" + this.txtUrl.Text + "</Parameter>";
            }

            //str += this.txtNRule.Text;

            if (this.txtSufStr.Text.Trim () != "")
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

            rNavRule(str);
            this.Close();
        }

        private void CreateNaviRule()
        {
            string reg = @"(?<=";

            this.txtGetStart.Text = ToolUtil.ClearFlag(this.txtGetStart.Text);
            this.txtGetStart.SelectionStart = this.txtGetStart.Text.Length;
            this.txtGetStart.ScrollToCaret();

            string s = ToolUtil.RegexReplaceTrans ( txtGetStart.Text,false );

            s = Regex.Replace(s, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            s = s.Replace(@"\r\n", "");

            this.txtGetEnd.Text = ToolUtil.ClearFlag(this.txtGetEnd.Text);
            this.txtGetEnd.SelectionStart = this.txtGetEnd.Text.Length;
            this.txtGetEnd.ScrollToCaret();

            string e = ToolUtil.RegexReplaceTrans (txtGetEnd.Text,false );

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

        private void button2_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Show(this.button2, 0, 21);
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
                    !str.StartsWith("{FormValue", StringComparison.CurrentCultureIgnoreCase) &&
                    !str.StartsWith("{Script", StringComparison.CurrentCultureIgnoreCase) &&
                    !str.StartsWith("{CurrtentPageUrl", StringComparison.CurrentCultureIgnoreCase) &&
                    !str.StartsWith("{Formula", StringComparison.CurrentCultureIgnoreCase))
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
            else
            {
                int startPos = this.txtUrl.SelectionStart;
                int l = this.txtUrl.SelectionLength;

                this.txtUrl.Text = this.txtUrl.Text.Substring(0, startPos) + uPara + this.txtUrl.Text.Substring(startPos + l, this.txtUrl.Text.Length - startPos - l);

                this.txtUrl.SelectionStart = startPos + uPara.Length;
                this.txtUrl.ScrollToCaret();
            }

        }

        private void GetScript(string str)
        {
            str = str.Replace("{", "\\{").Replace("}", "\\}");
            string ss = "{Script:" + str + "}";
            int startPos = this.txtUrl.SelectionStart;
            int l = this.txtUrl.SelectionLength;

            this.txtUrl.Text = this.txtUrl.Text.Substring(0, startPos) + ss + this.txtUrl.Text.Substring(startPos + l, this.txtUrl.Text.Length - startPos - l);

            this.txtUrl.SelectionStart = startPos + ss.Length;
            this.txtUrl.ScrollToCaret();
        }


        private void GetUrlPara(string str)
        {
            this.m_getUrlPara = str;
        }

        private void GetPData(string strCookie, string pData)
        {
            //this.txtCookie.Text = strCookie;
            this.txtUrl.Text += "<POST:ASCII>" + pData + "</POST>";
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

            this.txtNRule.Enabled = false ;

            this.radioButton1.Checked = false;
            this.radioButton2.Checked = false;
            this.radioButton3.Checked = false;
            this.raXPath.Checked = true;

            this.txtXPath.Enabled = true;
            this.cmdVisual.Enabled = true;

            this.raPara.Checked = false;
            this.label3.Enabled = false;
            this.txtUrl.Enabled = false;
            this.button2.Enabled = false;

            this.txtGetStartPara.Enabled = false;
            this.txtGetEndPara.Enabled = false;
            this.cmdInsertPara.Enabled = false;
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


        private void raPara_Click(object sender, EventArgs e)
        {
            SetPara();
        }

        private void SetPara()
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
               Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
               Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
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
                this.checkBox1.Checked = false;

                this.txtNRule.Enabled = false;

                this.label2.Text = "根据网址的起始标志和结束标志匹配下一页网址";
                this.txtNRule.Text = "";

                this.raXPath.Checked = false;

                this.txtXPath.Enabled = false;
                this.cmdVisual.Enabled = false;

                this.label3.Enabled = true;
                this.txtUrl.Enabled = true;
                this.button2.Enabled = true;

                this.raPara.Checked = true;
                this.radioButton1.Checked = false;
                this.radioButton2.Checked = false;
                this.radioButton3.Checked = false;

                this.txtGetStartPara.Enabled = true ;
                this.txtGetEndPara.Enabled = true ;
                this.cmdInsertPara.Enabled = true ;
            }
            else
            {
                MessageBox.Show("当前版本不支持翻页参数配置，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.raPara.Checked = false;
                return;
            }
        }

        private void frmAddNextRule_Load(object sender, EventArgs e)
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
            HelpTip.SetToolTip(this.radioButton1, @"选择此项则输入需要翻页网址的相同部分即可，注意：一定要从http://开始输入");
            HelpTip.SetToolTip(this.radioButton2, @"选中此选项则通过配置翻页地址的前置和后置字符串来获取翻页地址信息");
            HelpTip.SetToolTip(this.radioButton3, @"选中此选项则通过自定义正则表达式来获取翻页地址信息");
            HelpTip.SetToolTip(this.txtNRule, @"如果选择了只输入翻页网址特征信息，则在此输入网址的特征信息");
            HelpTip.SetToolTip(this.txtGetStart, @"如果选择了自定义配置，则在此输入翻页地址的前置字符串，建议为一个完整的标签");
            HelpTip.SetToolTip(this.txtGetEnd, @"如果选择了自定义配置，则在此输入翻页地址的后置字符串，建议为一个完整的标签");
            HelpTip.SetToolTip(this.cmdStartWildcard, "点击此按钮，选择通配符类型，插入通配符");
            HelpTip.SetToolTip(this.cmdEndWildcard, "点击此按钮，选择通配符类型，插入通配符");
            HelpTip.SetToolTip(this.checkBox1, @"选中此选项，则匹配翻页地址的时候，会剔除包含>的地址信息");
            HelpTip.SetToolTip(this.checkBox2, @"选中此选项，则匹配翻页地址的时候，会剔除包含空格的地址信息");
            HelpTip.SetToolTip(this.checkBox3, @"选中此选项，则匹配翻页地址的时候，会剔除包含#的地址信息");
            HelpTip.SetToolTip(this.checkBox4, @"选中此选项，则匹配翻页地址的时候，会剔除包含'的地址信息");
            HelpTip.SetToolTip(this.checkBox5, "选中此选项，则匹配翻页地址的时候，会剔除包含\"的地址信息");
            HelpTip.SetToolTip(this.checkBox6, @"选中此选项，则匹配翻页地址的时候，会剔除右侧输入框指定字符的地址信息");
            HelpTip.SetToolTip(this.txtCustomLetter, @"在此输入需要剔除指定字符的地址信息");
            HelpTip.SetToolTip(this.txtIncludeWord, @"在此输入必须包含的字符串，根据规则匹配翻页网址后，会自动删除不包含此字符串的地址信息");
            HelpTip.SetToolTip(this.txtNoIncludeWord, @"在此输入不能包含的字符串，根据规则匹配翻页网址后，会自动删除包含此字符串的地址信息");
            HelpTip.SetToolTip(this.txtPrestr, @"在此输入需要在翻页地址前加入的字符串");
            HelpTip.SetToolTip(this.txtSufStr, @"在此输入需要在翻页地址后加入的字符串");
            HelpTip.SetToolTip(this.isRegex, @"如果选择替换翻页网址的字符，选中此选项则启用正则替换模式");
            HelpTip.SetToolTip(this.txtOldValue, @"在此输入翻页地址中需要替换的字符串或正则信息");
            HelpTip.SetToolTip(this.txtNewValue, @"在此输入替换的结果信息");
            HelpTip.SetToolTip(this.raXPath, @"选中此选项，则通过可视化的方法配置翻页规则");
            HelpTip.SetToolTip(this.txtXPath, "在此输入可视化获取的XPath路径地址");
            HelpTip.SetToolTip(this.cmdVisual, "点击此按钮打开可视化捕获工具，来自动获取可视化捕获的XPath地址信息");
            HelpTip.SetToolTip(this.raPara, "选中此选项，则采用模拟翻页网址的方式进行翻页规则配置，" + "\r\n" + "模拟翻页网址，并通过配置参数来模拟网址参数实现翻页");
            HelpTip.SetToolTip(this.txtGetStartPara, @"系统在翻页入口页面中进行参数的捕获，在此输入捕获参数的前置字符串，建议为一个完整的标签");
            HelpTip.SetToolTip(this.txtGetEndPara, @"系统则翻页入口页面中进行参数的捕获，在此输入捕获参数的后置字符串，建议为一个完整的标签");
            HelpTip.SetToolTip(this.txtUrl, "在此输入模拟的翻页地址，注意：支持POST方式，POST提交数据请用<POST:ASCII></POST>标签表明识别");
            HelpTip.SetToolTip(this.cmdInsertPara, "点击此按钮插入已经设置好的参数，插入的位置在导航网址当前所选中的位置");
            HelpTip.SetToolTip(this.button2, "点击此按钮插入网址参数");

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            this.menuTimestamp.Text = "时间戳{Timestamp:" + ToolUtil.GetTimestamp().ToString() + "}";
        }

        private void raPara_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cmdInsertPara_Click(object sender, EventArgs e)
        {
            int startPos = this.txtUrl.SelectionStart;
            int l = this.txtUrl.SelectionLength;

           
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

                this.txtUrl.Text = this.txtUrl.Text.Substring(0, startPos) + "<Para>" + reg + "</Para>"
                    + this.txtUrl.Text.Substring(startPos + l, this.txtUrl.Text.Length - startPos - l);

                this.txtUrl.SelectionStart = startPos + reg.Length;
                this.txtUrl.ScrollToCaret();

                this.txtGetStartPara.Text = "";
                this.txtGetEndPara.Text = "";

            
           
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

        private void txtUrl_Leave(object sender, EventArgs e)
        {
            this.txtUrl.Text = this.txtUrl.Text.Replace("\r\n", "");
        }

        private void menuTimestamp_Click(object sender, EventArgs e)
        {

        }

        private void menuFormula_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

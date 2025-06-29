using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using NetMiner.Gather;
using NetMiner.Resource;
using NetMiner.Common;

namespace MinerSpider
{
    public partial class frmAddMultiPage : Form
    {
        public delegate void ReturnNavRule(cGlobalParas.FormState fState, string[] NavRule);
        public ReturnNavRule rNavRule;

        //定义一个ToolTip
        ToolTip HelpTip = new ToolTip();

        private cGlobalParas.FormState m_fState;
        public cGlobalParas.FormState fState
        {
            get { return m_fState; }
            set { m_fState = value; }
        }

        public frmAddMultiPage()
        {
            InitializeComponent();

            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
               Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                this.comMultiLevel.Enabled = true;
            }
            else
            {
                this.comMultiLevel.Enabled = false ;
            }
        }

        public frmAddMultiPage(string rName,string mLevel, string strNRule,int pageLevel)
        {
            InitializeComponent();

            this.comMultiLevel.Items.Clear();
            for (int i = 0; i <= pageLevel; i++)
            {
                this.comMultiLevel.Items.Add(i.ToString());
            }

            this.txtUrlName.Text = rName;
            if (mLevel == "")
            {
                this.comMultiLevel.SelectedIndex = 0;
            }
            else
            {
                for (int i = 0; i < this.comMultiLevel.Items.Count; i++)
                {
                    this.comMultiLevel.SelectedIndex = i;
                    if (this.comMultiLevel.Text == mLevel)
                        break;
                }
            }

            string nRule = strNRule;
            Match charSetMatch;

            if (Regex.IsMatch(strNRule, "<Prefix>"))
            {
                charSetMatch = Regex.Match(nRule, "(?<=<Prefix>).*?(?=</Prefix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.IsPreStr.Checked = true;
                this.txtPrestr.Text = charSetMatch.Groups[0].ToString();

                charSetMatch = Regex.Match(strNRule, "(?<=</Prefix>).*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();
            }

            if (Regex.IsMatch(strNRule, "<Suffix>"))
            {
                charSetMatch = Regex.Match(nRule, "(?<=<Suffix>).*?(?=</Suffix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.IsSufStr.Checked = true;
                this.txtSufStr.Text = charSetMatch.Groups[0].ToString();

                charSetMatch = Regex.Match(strNRule, ".*?(?=<Suffix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();
            }

            if (strNRule.StartsWith("<Common>"))
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

                this.txtGetStart.Text = ToolUtil.RegexUnescape (starts);
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

                this.txtNRule.Text = strNRule;
            }
            else if (strNRule.StartsWith("<Regex>"))
            {
                this.radioButton3.Checked = true;
                SetCustom();

                this.txtGetStart.Enabled = false;
                this.txtGetEnd.Enabled = false ;
                this.checkBox1.Enabled = false ;
                this.checkBox2.Enabled = false ;
                this.checkBox3.Enabled = false ;
                this.checkBox4.Enabled = false ;
                this.checkBox5.Enabled = false ;
                this.checkBox6.Enabled = false ;
                this.txtCustomLetter.Enabled = false ;

                this.txtNRule.Enabled = true;

                charSetMatch = Regex.Match(strNRule, "(?<=<Regex>).*?(?=</Regex>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();
                this.txtNRule.Text = strNRule;
            }
            else if (strNRule.StartsWith("<ParaUrl>"))
            {
                this.radioButton1.Checked = true;
                SetParaEnabled();

                this.radioButton2.Checked = false;
                this.radioButton3.Checked = false;

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

                this.tabControl1.SelectedIndex = 1;

                charSetMatch = Regex.Match(strNRule, "(?<=<ParaUrl>).*?(?=</ParaUrl>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strNRule = charSetMatch.Groups[0].ToString();
                this.txtUrl.Text = strNRule;
            }

            
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
            this.checkBox1.Checked = true;
            this.txtCustomLetter.Enabled = false ;

            this.checkBox1.Checked = true;

            this.button1.Enabled = false;
            this.txtNRule.Enabled = false;

            this.radioButton1.Checked = false;

            this.txtUrl.Enabled = false;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            SetCustom();
        }
        
        private void SetCustom()
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

            this.radioButton1.Checked = false;

            this.button1.Enabled = false;
            this.txtUrl.Enabled = false;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (this.txtUrlName.Text.Trim() == "")
            {
                MessageBox.Show("多页采集规则的名称不能为空，请填写！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtUrlName.Focus();
                return;
            }

            string str = "";
            if (this.IsPreStr.Checked == true)
                str = "<Prefix>" + this.txtPrestr.Text + "</Prefix>";

            if (this.radioButton2.Checked == true)
            {
                str += "<Common>" + this.txtNRule.Text + "</Common>";
            }
            else if (this.radioButton3.Checked == true)
            {
                str += "<Regex>" + this.txtNRule.Text + "</Regex>";
            }
            

            //str += this.txtNRule.Text;

            if (this.IsSufStr.Checked == true)
                str += "<Suffix>" + this.txtSufStr.Text + "</Suffix>";

            if (this.radioButton1.Checked == true)
            {
                str = "<ParaUrl>" + this.txtUrl.Text + "</ParaUrl>";
            }

            string[] strs = new string[3];
            strs[0] = this.txtUrlName.Text;
            strs[1] = this.comMultiLevel.Text;
            strs[2] = str;
            rNavRule(this.fState , strs);
            this.Close();
        }

        private void txtGetStart_TextChanged(object sender, EventArgs e)
        {
            CreateNaviRule();
        }

        private void txtGetEnd_TextChanged(object sender, EventArgs e)
        {
            CreateNaviRule();
        }

        private void CreateNaviRule()
        {
            string reg = @"(?<=";

            string s = ToolUtil.RegexReplaceTrans (txtGetStart.Text,false );

            s = Regex.Replace(s, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            s = s.Replace(@"\r\n", "");


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

        private void IsPreStr_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsPreStr.Checked == true)
                this.txtPrestr.Enabled = true;
            else
                this.txtPrestr.Enabled = false;
        }

        private void IsSufStr_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsSufStr.Checked == true)
                this.txtSufStr.Enabled = true;
            else
                this.txtSufStr.Enabled = false;
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
            if (this.checkBox6.Checked == true)
                this.txtCustomLetter.Enabled = true;
            else
                this.txtCustomLetter.Enabled = false;

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

    
      

        private void radioButton1_Click(object sender, EventArgs e)
        {
            SetParaEnabled();
        }

        private void SetParaEnabled()
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

                this.button1.Enabled = true;
                this.txtUrl.Enabled = true;

                this.radioButton2.Checked = false;
                this.radioButton3.Checked = false;
            }
            else
            {
                MessageBox.Show("当前版本不支持多页参数配置，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.radioButton1.Checked = false;
                return;
            }
        }

        private void frmAddMultiPage_Load(object sender, EventArgs e)
        {
            //对Tooltip进行初始化设置
            HelpTip.ToolTipIcon = ToolTipIcon.Info;
            HelpTip.ForeColor = Color.YellowGreen;
            HelpTip.BackColor = Color.LightGray;
            HelpTip.AutoPopDelay = 20000;
            HelpTip.ShowAlways = true;
            HelpTip.ToolTipTitle = "小矿提示";

            SetHelpTip();
        }

        private void SetHelpTip()
        {
            HelpTip.SetToolTip(this.txtUrlName, @"在此输入需要多页规则的名称，在配置采集规则数据项时，会要求对应此名称");
            HelpTip.SetToolTip(this.radioButton2, @"选中此选项则通过配置多页地址的前置和后置字符串来获取多页的地址信息");
            HelpTip.SetToolTip(this.radioButton3, @"选中此选项则通过自定义正则表达式来获取多页地址信息");
            HelpTip.SetToolTip(this.txtGetStart, @"在此输入多页地址的前置字符串，建议为一个完整的标签");
            HelpTip.SetToolTip(this.txtGetEnd, @"在此输入多页地址的后置字符串，建议为一个完整的标签");
            HelpTip.SetToolTip(this.checkBox1, @"选中此选项，则匹配多页地址的时候，会剔除包含>的地址信息");
            HelpTip.SetToolTip(this.checkBox2, @"选中此选项，则匹配多页地址的时候，会剔除包含空格的地址信息");
            HelpTip.SetToolTip(this.checkBox3, @"选中此选项，则匹配多页地址的时候，会剔除包含#的地址信息");
            HelpTip.SetToolTip(this.checkBox4, @"选中此选项，则匹配多页地址的时候，会剔除包含'的地址信息");
            HelpTip.SetToolTip(this.checkBox5, "选中此选项，则匹配多页地址的时候，会剔除包含\"的地址信息");
            HelpTip.SetToolTip(this.checkBox6, @"选中此选项，则匹配多页地址的时候，会剔除右侧输入框指定字符的地址信息");
            HelpTip.SetToolTip(this.txtCustomLetter, @"在此输入需要剔除指定字符的地址信息");
            HelpTip.SetToolTip(this.txtNRule, @"如果选择了正则匹配，则在此输入正则表达式");
            HelpTip.SetToolTip(this.IsPreStr, @"如果选择了此选项，则在匹配结果前，会加入右侧输入的指定字符串");
            HelpTip.SetToolTip(this.IsSufStr, @"如果选择了此选项，则在匹配结果后，会加入右侧输入的指定字符串");
            HelpTip.SetToolTip(this.txtPrestr, @"在此输入需要在匹配结果前加入的字符串");
            HelpTip.SetToolTip(this.txtSufStr, @"在此输入需要在匹配结果后加入的字符串");
            HelpTip.SetToolTip(this.radioButton1, @"选中此选项，则通过模拟地址的方式来获取多页地址");
            HelpTip.SetToolTip(this.button1, @"在多页地址中插入时间戳");
            HelpTip.SetToolTip(this.txtUrl, @"在此输入模拟的多页地址信息，可以是一个完整的Url地址");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Match s;

            string str = "时间戳{Timestamp:" + ToolUtil.GetTimestamp().ToString() + "}";
            if (Regex.IsMatch(str, "[{].*[}]"))
            {
                s = Regex.Match(str, "[{].*[}]");
            }
            else
            {
                s = Regex.Match(str, "[<].*[>]");
            }

            int startPos = this.txtUrl.SelectionStart;
            int l = this.txtUrl.SelectionLength;

            this.txtUrl.Text = this.txtUrl.Text.Substring(0, startPos) + s.Groups[0].Value + this.txtUrl.Text.Substring(startPos + l, this.txtUrl.Text.Length - startPos - l);

            this.txtUrl.SelectionStart = startPos + s.Groups[0].Value.Length;
            this.txtUrl.ScrollToCaret();
        }

        private void radioButton1_ClientSizeChanged(object sender, EventArgs e)
        {

        }

        private void txtUrl_Leave(object sender, EventArgs e)
        {
            this.txtUrl.Text = this.txtUrl.Text.Replace("\r\n", "");
        }
    }
}

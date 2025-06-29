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

        //����һ��ToolTip
        ToolTip HelpTip = new ToolTip();

        public frmAddNavRules()
        {
            InitializeComponent();

            this.comParaType.Items.Add("�Զ������");
            this.comParaType.Items.Add("��������{Num:1,10,1}");
            this.comParaType.Items.Add("ʱ���{Timestamp:" + ToolUtil.GetTimestamp().ToString() + "}");
        }

        public frmAddNavRules(string TestUrl,string strNRule,string cookie)
        {
            InitializeComponent();

            this.m_TestUrl = TestUrl;
            this.m_Cookie = cookie;

            this.comParaType.Items.Add("�Զ������");
            this.comParaType.Items.Add("��������{Num:1,10,1}");
            this.comParaType.Items.Add("ʱ���{Timestamp:" + ToolUtil.GetTimestamp().ToString() + "}");

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

                //�ֽ�������ʽ
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
                    //�ж��Ƿ��ж������
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
                //�������� �ⲿ�ִ���Ĳ��Ǻܺã�������ʱ����Ҫ�����޸�
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

                //XPath���ӻ�ƥ��
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

            //��Tooltip���г�ʼ������
            HelpTip.ToolTipIcon = ToolTipIcon.Info;
            HelpTip.ForeColor = Color.YellowGreen;
            HelpTip.BackColor = Color.LightGray;
            HelpTip.AutoPopDelay = 20000;
            HelpTip.ShowAlways = true;
            HelpTip.ToolTipTitle = "С����ʾ";

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
            HelpTip.SetToolTip(this.radioButton1, @"ѡ�������������Ҫ������ַ����ͬ���ּ��ɣ�ע�⣺һ��Ҫ��http://��ʼ����");
            HelpTip.SetToolTip(this.radioButton2, @"ѡ�д�ѡ����ͨ�����õ�����ַ��ǰ�úͺ����ַ�������ȡ������ַ��Ϣ");
            HelpTip.SetToolTip(this.radioButton3, @"ѡ�д�ѡ����ͨ���Զ���������ʽ����ȡ������ַ��Ϣ");
            HelpTip.SetToolTip(this.raMySelf, @"ѡ�д�ѡ����ǰҳ�浼�����뵱ǰҳ�棬����������Ŀ���ǿ��ԶԵ�ǰҳ������ݽ��ж��βɼ���ʵ���������յĺϲ�����");
            HelpTip.SetToolTip(this.txtNRule, @"���ѡ����ֻ���뵼����ַ������Ϣ�����ڴ�������ַ��������Ϣ");
            HelpTip.SetToolTip(this.txtGetStart, @"���ѡ�����Զ������ã����ڴ����뵼����ַ��ǰ���ַ���������Ϊһ�������ı�ǩ");
            HelpTip.SetToolTip(this.txtGetEnd , @"���ѡ�����Զ������ã����ڴ����뵼����ַ�ĺ����ַ���������Ϊһ�������ı�ǩ");
            HelpTip.SetToolTip(this.cmdStartWildcard, "����˰�ť��ѡ��ͨ������ͣ�����ͨ���");
            HelpTip.SetToolTip(this.cmdEndWildcard, "����˰�ť��ѡ��ͨ������ͣ�����ͨ���");
            HelpTip.SetToolTip(this.checkBox1, @"ѡ�д�ѡ���ƥ�䵼����ַ��ʱ�򣬻��޳�����>�ĵ�ַ��Ϣ");
            HelpTip.SetToolTip(this.checkBox2, @"ѡ�д�ѡ���ƥ�䵼����ַ��ʱ�򣬻��޳������ո�ĵ�ַ��Ϣ");
            HelpTip.SetToolTip(this.checkBox3, @"ѡ�д�ѡ���ƥ�䵼����ַ��ʱ�򣬻��޳�����#�ĵ�ַ��Ϣ");
            HelpTip.SetToolTip(this.checkBox4, @"ѡ�д�ѡ���ƥ�䵼����ַ��ʱ�򣬻��޳�����'�ĵ�ַ��Ϣ");
            HelpTip.SetToolTip(this.checkBox5, "ѡ�д�ѡ���ƥ�䵼����ַ��ʱ�򣬻��޳�����\"�ĵ�ַ��Ϣ");
            HelpTip.SetToolTip(this.checkBox6, @"ѡ�д�ѡ���ƥ�䵼����ַ��ʱ�򣬻��޳��Ҳ������ָ���ַ��ĵ�ַ��Ϣ");
            HelpTip.SetToolTip(this.txtCustomLetter, @"�ڴ�������Ҫ�޳�ָ���ַ��ĵ�ַ��Ϣ");
            HelpTip.SetToolTip(this.txtIncludeWord, @"�ڴ��������������ַ��������ݹ���ƥ�䵼����ַ�󣬻��Զ�ɾ�����������ַ����ĵ�ַ��Ϣ");
            HelpTip.SetToolTip(this.txtNoIncludeWord, @"�ڴ����벻�ܰ������ַ��������ݹ���ƥ�䵼����ַ�󣬻��Զ�ɾ���������ַ����ĵ�ַ��Ϣ");
            HelpTip.SetToolTip(this.txtPrestr, @"�ڴ�������Ҫ�ڵ�����ַǰ������ַ���");
            HelpTip.SetToolTip(this.txtSufStr, @"�ڴ�������Ҫ�ڵ�����ַ�������ַ���");
            HelpTip.SetToolTip(this.isRegex, @"���ѡ���滻������ַ���ַ���ѡ�д�ѡ�������������滻ģʽ");
            HelpTip.SetToolTip(this.txtOldValue, @"�ڴ����뵼����ַ����Ҫ�滻���ַ�����������Ϣ");
            HelpTip.SetToolTip(this.txtNewValue, @"�ڴ������滻�Ľ����Ϣ");
            HelpTip.SetToolTip(this.raXPath, @"ѡ�д�ѡ���ͨ�����ӻ��ķ������õ�������");
            HelpTip.SetToolTip(this.txtXPath, "�ڴ�������ӻ���ȡ��XPath·����ַ");
            HelpTip.SetToolTip(this.cmdVisual, "����˰�ť�򿪿��ӻ����񹤾ߣ����Զ���ȡ���ӻ������XPath��ַ��Ϣ");
            HelpTip.SetToolTip(this.cmdXpathPara, "����˰�ť�ɲ������ֲ���");
            HelpTip.SetToolTip(this.radioButton4, "ѡ�д�ѡ������ģ�⵼����ַ�ķ�ʽ���е����������ã�" + "\r\n" + "ģ�⵼����ַ����ͨ�����ò�����ģ����ַ����ʵ�ֵ���");
            HelpTip.SetToolTip(this.comParaType, "�ڴ�ѡ�������ַ����������");
            HelpTip.SetToolTip(this.txtGetStartPara, @"���ѡ�����Զ��������ϵͳ���ڵ������ҳ���н��в����Ĳ����ڴ����벶�������ǰ���ַ���������Ϊһ�������ı�ǩ");
            HelpTip.SetToolTip(this.txtGetEndPara, @"���ѡ�����Զ��������ϵͳ���ڵ������ҳ���н��в����Ĳ����ڴ����벶������ĺ����ַ���������Ϊһ�������ı�ǩ");
            HelpTip.SetToolTip(this.UrlPara, "�ڴ�����ģ��ĵ�����ַ��ע�⣺֧��POST��ʽ��POST�ύ��������<POST:ASCII></POST>��ǩ����ʶ��");
            HelpTip.SetToolTip(this.cmdInsertPara , "����˰�ť�����Ѿ����úõĲ����������λ���ڵ�����ַ��ǰ��ѡ�е�λ��");


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

            this.label2.Text = "��д��ַ����ͬ���ּ����Զ�ʶ�𵼺���ַ";
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

            this.label2.Text = "������ַ����ʼ��־�ͽ�����־ƥ�䵼����ַ";
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

            this.label2.Text = "ʹ��������ʽƥ�䵼����ַ�ʺ�רҵ�û�ʹ��";
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

            this.label2.Text = "��ʾ��������ҳ�滹�ǵ�ǰҳ�棬��������Ϊ��ʵ��һ��ҳ����һ�Զ����ݹ�ϵ�Ĳɼ���"
                    + "ͨ��ѡ���ѡ������Ҫ���ü����ɼ�����";
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
                MessageBox.Show("��ǰ�汾��֧�ֵ����������ã����ȡ��ȷ�İ汾��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    MessageBox.Show("��Ҫ��������������ʼλ�ã�", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.txtGetStartPara.Focus();
                    return;
                }

                if (this.txtGetEndPara.Text.Trim() == "")
                {
                    MessageBox.Show("��Ҫ��������������ֹλ�ã�", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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


            //��ʼ���ֵ�˵�����Ŀ
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
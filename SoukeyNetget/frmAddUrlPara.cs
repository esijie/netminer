using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MinerSpider
{
    public partial class frmAddUrlPara : Form
    {
        public delegate void ReturnUrlPara(string uPara);
        public ReturnUrlPara rUrlPara;
        private string ParaType = string.Empty;
        private string _activeControl = string.Empty;

        public frmAddUrlPara()
        {
            InitializeComponent();
        }

        public frmAddUrlPara(string strPara)
        {
            InitializeComponent();

            int step;
            int startI;
            int endI;
            int i = 0;

            string startstrI = string.Empty;
            string endstrI = string.Empty;

            int startYear = 0;
            int startMonth = 0;
            int startDay = 0;

            int endYear = 0;
            int endMonth = 0;
            int endDay = 0;
            Regex re;
            MatchCollection aa;

            strPara = strPara.Substring(1, strPara.Length - 2);

            switch (strPara.Substring(0, strPara.IndexOf(":")))
            {
                case "Num":
                    if (strPara.IndexOf("<Formula:>") > -1)
                    {
                        ParaType = "Num";

                        //需要计算结束翻页的页码
                        this.txtMaxCount.Enabled = true;
                        this.txtPageSize.Enabled = true;
                        this.button1.Enabled = true;
                        this.button2.Enabled = true;
                        this.txtEnd.Enabled = false;

                        this.txtStart.Text = "1";
                        this.txtStep.Text = "1";
                        this.txtMaxCount.Text = "0";
                        this.txtPageSize.Text = "0";
                    }
                    else
                    {
                        ParaType = "Num";
                        re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        aa = re.Matches(strPara);

                        startI = int.Parse(aa[0].Groups[0].Value.ToString());
                        endI = int.Parse(aa[1].Groups[0].Value.ToString());
                        step = int.Parse(aa[2].Groups[0].Value.ToString());

                        this.txtStart.Text = startI.ToString();
                        this.txtEnd.Text = endI.ToString();
                        this.txtStep.Text = step.ToString();
                    }
                    break;
                case "Syn":
                    ParaType = "Syn";
                    re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    aa = re.Matches(strPara);

                    startI = int.Parse(aa[0].Groups[0].Value.ToString());
                    endI = int.Parse(aa[1].Groups[0].Value.ToString());
                    step = int.Parse(aa[2].Groups[0].Value.ToString());

                    this.txtStart.Text = startI.ToString ();
                    this.txtEnd.Text = endI.ToString ();
                    this.txtStep.Text  = step.ToString ();

                    break;
                case "NumZero":
                    ParaType = "NumZero";
                    re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    aa = re.Matches(strPara);

                    startstrI = aa[0].Groups[0].Value.ToString();
                    endstrI = aa[1].Groups[0].Value.ToString();
                    step = int.Parse(aa[2].Groups[0].Value.ToString());
                    startI = int.Parse(aa[0].Groups[0].Value.ToString());
                    endI = int.Parse(aa[1].Groups[0].Value.ToString());

                    int l = endI.ToString().Length;
                    string strStart = startI.ToString();

                    while (strStart.Length < l)
                    {
                        strStart = "0" + strStart;
                    }

                    this.txtStart.Text = startI.ToString ();
                    this.txtEnd.Text = endI.ToString ();
                    this.txtStep.Text  = step.ToString ();

                    break;
                case "Letter":
                    ParaType = "Letter";
                  
                    this.txtStart.Text = strPara.Substring(strPara.IndexOf(":") + 1, 1);
                    this.txtEnd.Text = strPara.Substring(strPara.IndexOf(",") + 1, 1);
                    this.txtStep.Enabled = false;

                    break;
          
            }
        }

        private void frmAddUrlPara_Load(object sender, EventArgs e)
        {

        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private int getAsc(string s)
        {
            byte[] array = new byte[1];
            array = System.Text.Encoding.ASCII.GetBytes(s);
            int asciicode = (int)(array[0]);
            return asciicode;
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            if (this.txtStart.Text.Trim() == "")
            {
                MessageBox.Show("起始值不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtStart.Focus();
                return;
            }

            if (this.txtEnd.Text.Trim() == "")
            {
                MessageBox.Show("结束值不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtEnd.Focus();
                return;
            }

            if (this.txtStep.Text.Trim() == "")
            {
                if (ParaType != "ShortDate" && ParaType != "LongDate" && ParaType != "8Date" && ParaType != "Letter")
                {
                    MessageBox.Show("每次递增不能为空！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.txtStep.Focus();
                    return;
                }
            }

            string str = string.Empty;

            switch (ParaType)
            {
                case "Num":
                    str = "{Num:" + this.txtStart.Text + "," + this.txtEnd.Text + "," + this.txtStep.Text + "}";
                    break;
                case "Syn":
                    str = "{Syn:" + this.txtStart.Text + "," + this.txtEnd.Text + "," + this.txtStep.Text + "}";
                    break;
                case "NumZero":
                    str = "{NumZero:" + this.txtStart.Text + "," + this.txtEnd.Text + "," + this.txtStep.Text + "}";
                    break;
                case "Letter":
                    str = "{Letter:" + this.txtStart.Text + "," + this.txtEnd.Text + "}";
                    break;
                case "ShortDate":
                    str = "{ShortDate:" + this.txtStart.Text + "," + this.txtEnd.Text + "}";
                    break;
                case "LongDate":
                    str = "{LongDate:" + this.txtStart.Text + "," + this.txtEnd.Text + "}";
                    break;
                case "8Date":
                    str = "{8Date:" + this.txtStart.Text + "," + this.txtEnd.Text + "}";
                    break;
            }

            rUrlPara(str);
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this._activeControl = "txtMaxCount";
            this.contextMenuStrip1.Show(this.button1, 0, 21);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this._activeControl = "txtPageSize";
            this.contextMenuStrip1.Show(this.button2, 0, 21);
        }

        private void txtMaxCount_Enter(object sender, EventArgs e)
        {
            this._activeControl = "txtMaxCount";
        }

        private void txtPageSize_Enter(object sender, EventArgs e)
        {
            this._activeControl = "txtPageSize";
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string uPara = string.Empty;

            Match s = Regex.Match(e.ClickedItem.ToString(), "[{].*[}]");
            uPara = s.Groups[0].Value;

            string s1 = s.Groups[0].Value;
         
            int startPos = ((TextBox)this.Controls[this._activeControl]).SelectionStart;
            int l = ((TextBox)this.Controls[this._activeControl]).SelectionLength;

            this.Controls[this._activeControl].Text = this.Controls[this._activeControl].Text.Substring(0, startPos) + uPara + this.Controls[this._activeControl].Text.Substring(startPos + l, this.Controls[this._activeControl].Text.Length - startPos - l);

            ((TextBox)this.Controls[this._activeControl]).SelectionStart = startPos + uPara.Length;
            ((TextBox)this.Controls[this._activeControl]).ScrollToCaret();
            
        }

        private void txtMaxCount_TextChanged(object sender, EventArgs e)
        {
            this.txtEnd.Text = this.txtMaxCount.Text + "/" + this.txtPageSize.Text ;
        }

        private void txtPageSize_TextChanged(object sender, EventArgs e)
        {
            this.txtEnd.Text = this.txtMaxCount.Text + "/" + this.txtPageSize.Text;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics ;
using System.Text.RegularExpressions ;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Resources;

namespace MinerSpider
{
    public partial class frmRegex : Form
    {
        private ResourceManager rm;

        public frmRegex()
        {
            InitializeComponent();

        }
     
        private void frmRegex_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
            this.Dispose();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Excute();
           
        }

        private void Excute()
        {
            RegexOptions rOptions = new RegexOptions();

            if (this.IsCase.Checked == true)
                rOptions = RegexOptions.IgnoreCase;
            if (this.IsCulture.Checked == true)
                rOptions = rOptions ^ RegexOptions.CultureInvariant;
            if (this.IsMultiline.Checked == true)
                rOptions = rOptions ^ RegexOptions.Multiline;
            if (this.IsRightToLeft.Checked == true)
                rOptions = rOptions ^ RegexOptions.RightToLeft;

            //List<Match> matchList = new List<Match>();

            try
            {
                Regex re = new Regex(this.rtbRegex.Text, rOptions);
                MatchCollection mc = re.Matches(this.rtbSource.Text);

                this.lstResult.Items.Clear();

                if (mc.Count > 10000)
                {
                    if (MessageBox.Show("规则在网页内匹配出了上万条数据，有可能是规则错误，继续加载将可能导致系统长时间不响应，是否继续？", "网络矿工 询问",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;
                }

                ListViewItem lItem;
                //string ss = "";
                //System.Collections.Hashtable ht = new System.Collections.Hashtable();
                foreach (Match ma in mc)
                {
                    lItem = new ListViewItem();
                    lItem.Text = ma.Value;
                    lItem.SubItems.Add(ma.Index.ToString());
                    lItem.SubItems.Add(ma.Length.ToString());
                    //lItem.Tag = new NodeInfo(ma, i.ToString());
                    this.lstResult.Items.Add(lItem);
                    lItem = null;



                    //if (!ht.ContainsValue(ma.Value.ToString()))
                    //{
                    //    ht.Add(ma.Value.ToString(), ma.Value.ToString());
                    //    ss += ma.Value + "\r\n";
                    //}

                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
       
        }

        private void frmRegex_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void lstResult_Click(object sender, EventArgs e)
        {
            if (lstResult.SelectedItems.Count == 0) return;

            rtbSource.Select(0, 0);

            int position = Convert.ToInt32(lstResult.SelectedItems[0].SubItems[1].Text);
            int length = Convert.ToInt32(lstResult.SelectedItems[0].SubItems[2].Text);

            rtbSource.Select(position, length);
            rtbSource.ScrollToCaret();
        }

        private void lstResult_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void frmRegex_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));
            this.splitContainer2.Panel2Collapsed = true;
        }

        private string old_strWebData;
        private void IsWordwrap_CheckedChanged(object sender, EventArgs e)
        {
            string strWebData="";
            

            if (this.IsWordwrap.Checked == true)
            {
                old_strWebData = this.rtbSource.Text;
                strWebData = Regex.Replace(this.rtbSource.Text, "([\\r\\n])[\\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strWebData = Regex.Replace(strWebData, "\\n", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strWebData.Replace("\\r\\n", "");
                this.rtbSource.Text = strWebData;
            }
            else
            {
                this.rtbSource.Text = old_strWebData;
            }
        }

        private void cmdFindNext_Click(object sender, EventArgs e)
        {

            int fResult=FindString(false);
            if (fResult == -1)
            {
                this.labInfo.Text = rm.GetString("Info231");
                this.rtbSource.Select(this.rtbSource.Text.Length ,0);
            }
            else
            {
                this.labInfo.Text = "";
                this.rtbSource.Select(fResult, this.txtFindStr.Text.Length);
            }
            

        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolIsFind_Click(object sender, EventArgs e)
        {
            if (this.toolIsFind.Checked == true)
            {
                this.splitContainer2.Panel2Collapsed = true;
                this.toolIsFind.Checked = false;
            }
            else
            {
                this.splitContainer2.Panel2Collapsed = false ;
                this.toolIsFind.Checked = true;
                this.txtFindStr.Focus();
            }
        }

        private void cmdFindPre_Click(object sender, EventArgs e)
        {
            int fResult=FindString(true);

            if (fResult == -1)
            {
                this.labInfo.Text = rm.GetString("Info232");
                this.rtbSource.Select(0, 0);
            }
            else
            {
                this.labInfo.Text = "";
                this.rtbSource.Select(fResult, this.txtFindStr.Text.Length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Reverse">false为从头至尾 True为从尾至头</param>
        /// <returns></returns>
        private int FindString(bool Reverse)
        {
            RichTextBoxFinds findOptions;
            int start = 0;
            int end = -1;

            if (Reverse == true)
            {
                //方向搜索
                findOptions = RichTextBoxFinds.Reverse;
                start = 0;
                end = this.rtbSource.SelectionStart;
            }
            else
            {
                //正向搜索
                findOptions = RichTextBoxFinds.None;
                start = this.rtbSource.SelectionStart;
                start += this.rtbSource.SelectionLength;
            }

            this.rtbSource.SelectionStart = start;

            this.rtbSource.SelectionLength = 0;

            if (this.IsFindCase.Checked == false)
            {
                findOptions |= RichTextBoxFinds.MatchCase;
            }

            return this.rtbSource.Find(this.txtFindStr.Text, start, end, findOptions);
        }

        private void toolExcute_Click(object sender, EventArgs e)
        {
            Excute();
        }

    }

   class NodeInfo
   {
        public readonly Capture Item;
        public readonly string Name;

       public NodeInfo(Capture item, string name)
       {
          this.Item = item;
          this.Name = name;
       }
   }


}
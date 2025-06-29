using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using NetMiner.Resource;
using System.Drawing.Text;
using NetMiner.Common.Tool;

namespace MinerSpider
{
    public partial class frmSetWatermark : Form
    {
        public delegate void ReturnWatermark(string DataSource);
        public ReturnWatermark rWatermark;

        public frmSetWatermark()
        {
            InitializeComponent();
        }

        private void frmSetWatermark_Load(object sender, EventArgs e)
        {
            //根据当前的区域进行显示信息的加载
            ResourceManager rmPara = new ResourceManager("NetMiner.Resource.Resources.globalPara", Assembly.Load("NetMiner.Resource"));

            this.comWatermarkPos.Items.Add(rmPara.GetString("WatermarkPos1"));
            this.comWatermarkPos.Items.Add(rmPara.GetString("WatermarkPos2"));
            this.comWatermarkPos.Items.Add(rmPara.GetString("WatermarkPos3"));
            this.comWatermarkPos.Items.Add(rmPara.GetString("WatermarkPos4"));
            this.comWatermarkPos.Items.Add(rmPara.GetString("WatermarkPos5"));
            this.comWatermarkPos.Items.Add(rmPara.GetString("WatermarkPos6"));
            this.comWatermarkPos.Items.Add(rmPara.GetString("WatermarkPos7"));
            this.comWatermarkPos.Items.Add(rmPara.GetString("WatermarkPos8"));
            this.comWatermarkPos.Items.Add(rmPara.GetString("WatermarkPos9"));

            this.comWatermarkPos.SelectedIndex = 8;

            //获取字体
            InstalledFontCollection fc = new InstalledFontCollection();
            foreach (FontFamily font in fc.Families)
            {
                this.comFontType.Items.Add(font.Name);
            }

            try
            {
                this.comFontType.Text = "宋体";

            }
            catch { }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (this.txtWatermark.Text.Trim() == "")
            {
                MessageBox.Show("请输入水印文字！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtWatermark.Focus();
                return;
            }
            System.Drawing.Image myImage = (Image)this.pictureBox1.Image.Clone();

            this.pictureBox2.Image = null;

            Watermark wmark = new Watermark(myImage);
            wmark.FontColor = this.cmdFontColor.ForeColor;
            FontStyle fstyle = new FontStyle();
            if (this.IsFontWeight.Checked == true && this.IsFontItalic.Checked == true)
                fstyle = FontStyle.Bold | FontStyle.Italic;
            else if (this.IsFontItalic.Checked == true)
                fstyle = FontStyle.Italic;
            else if (this.IsFontWeight.Checked == true)
                fstyle = FontStyle.Bold;


            Font f = new System.Drawing.Font(this.comFontType.Text, float.Parse(this.upFontSize.Value.ToString()), fstyle);
            wmark.Font = f;
            wmark.Position = EnumUtil.GetEnumName<cGlobalParas.WatermarkPOS>(this.comWatermarkPos.Text);
            wmark.ResetImage();
            wmark.DrawText(this.txtWatermark.Text);
            this.pictureBox2.Image = wmark.Image;
            wmark = null;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StringBuilder strWatermark = new StringBuilder ();

            strWatermark.Append("<Text>" + this.txtWatermark.Text + "</Text>");
            strWatermark.Append("<FontFamily>" + this.comFontType.Text + "</FontFamily>");
            strWatermark.Append("<FontSize>" + this.upFontSize.Value + "</FontSize>");
            strWatermark.Append("<FontBold>" + this.IsFontWeight.Checked + "</FontBold>");
            strWatermark.Append("<FontItalic>" + this.IsFontItalic.Checked + "</FontItalic>");
            strWatermark.Append("<FontColor>" + this.cmdFontColor.ForeColor.ToArgb().ToString() + "</FontColor>");
            strWatermark.Append("<POS>" + this.comWatermarkPos.Text + "</POS>");

            rWatermark(strWatermark.ToString());

            this.Close();
        }

        private void cmdFontColor_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                // 将先中的颜色设置为窗体的背景色
                this.cmdFontColor.ForeColor = colorDialog1.Color;
            }

        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "请选择一张图片";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "图片文件(*.jpg,*.png,*.gif)|*.jpg;*.png;*.gif";


            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.pictureBox1.Image = Image.FromFile(this.openFileDialog1.FileName);
            }
        }
    }
}

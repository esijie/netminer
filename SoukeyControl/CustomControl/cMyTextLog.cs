using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms ;
using NetMiner.Resource;

///���ܣ��Զ����ı��ؼ�����Ҫ���ڲɼ�������־����ʾ
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyControl.CustomControl
{
    public class cMyTextLog : RichTextBox 
    {
        public cMyTextLog()
        {
            this.Text = "";
        }

        public void AppendText(cGlobalParas.LogType lType,string lText)
        {
            try
            {
                switch (lType)
                {
                    case cGlobalParas.LogType.Error:
                        base.SelectionFont = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Bold);
                        base.SelectionColor = System.Drawing.Color.Red;
                        break;
                    case cGlobalParas.LogType.Info:
                        base.SelectionFont = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Regular);
                        base.SelectionColor = System.Drawing.Color.Black;
                        break;
                    case cGlobalParas.LogType.Warning:
                        base.SelectionFont = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Bold);
                        base.SelectionColor = System.Drawing.Color.Orange;
                        break;
                    default:
                        base.SelectionFont = new System.Drawing.Font(DefaultFont, System.Drawing.FontStyle.Regular);
                        base.SelectionColor = System.Drawing.Color.Black;
                        break;
                }
                //this.Text += "\r\n" + lText;
                this.AppendText(lText + "\r\n");
                //base.AppendText(lText + "\r\n");
                this.SelectionStart = int.MaxValue;
                this.ScrollToCaret();
                //Application.DoEvents();
            }
            catch (System.Exception)
            {

            }
        }
       
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using NetMiner.Common;

namespace SoukeyControl
{
    public partial class frmLookPResult : Form
    {
        public frmLookPResult()
        {
            InitializeComponent();
        }

        private void frmLookPResult_Load(object sender, EventArgs e)
        {

        }

        public void Loadweb(string source,string url)
        {
            if (source == "" || source ==null)
                return;

            source = new Regex(@"(?m)<script[^>]*>(\w|\W)*?</script[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(source, "");


            source = ToolUtil.ConvertToAbsoluteUrls(source, new Uri(url));

            this.webBrowser1.DocumentText = source;
        }
    }
}

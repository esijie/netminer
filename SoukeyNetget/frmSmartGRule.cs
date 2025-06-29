using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using NetMiner.Gather;
using HtmlAgilityPack;
using NetMiner.Gather.Task;
using NetMiner.Core.gTask.Entity;
using NetMiner.Gather.Control;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Net;
using NetMiner.Net.Common;

namespace MinerSpider
{
    

    public partial class frmSmartGRule : Form
    {
        HtmlAgilityPack.HtmlDocument m_Doc;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern bool InvalidateRect(IntPtr hwnd, ref tagRECT lpRect, bool bErase);

        IntPtr m_hwnd;
        Pen m_browserPen;

        Rectangle m_elemRect;

        HtmlElement m_curElem;
        HtmlElement m_curFatherElem;
        HtmlElement m_curPreElem;
        HtmlElement m_curNextElem;
        private string m_HtmlSource;

        //页面加载超时时间，每次间隔等待0.5秒，设置为10秒
        private int m_Timeout = 100;
        private const int m_IntervalTime = 500;

        //定义一个值，确定当前捕获的xpath是属于什么xpath
        private string activeControl;

        //定义一个值，存储查找的树形节点表格
        private int m_tableNodeIndex;

        //记录鼠标点击次数，在多条捕获下有效
        private int m_MouseDownCount = 0;


        public delegate void ReturnGatherRule(List<cGatherRule> gRule);
        public ReturnGatherRule rGatherRule;

        public frmSmartGRule()
        {
            InitializeComponent();
        }

        private string m_Cookie;
        public string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GotoUrl(this.txtUrl.Text);
        }

        private void GotoUrl(string url)
        {
            try
            {
                url = url.Trim();
                if (url == "")
                    return;

                if (!url.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) &&
                    !url.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                {
                    url = "http://" + url;
                }

                //NetMiner.Gather.Control.cGatherWeb gWeb = new NetMiner.Gather.Control.cGatherWeb(Program.getPrjPath());

                string cookie = this.Cookie;
                //cGlobalParas.WebCode wCode = cGlobalParas.WebCode.auto;

                //string htmlsource = gWeb.GetHtml(url, wCode, false, false,cGlobalParas.WebCode.auto, ref cookie, "", "", false, false, "", false, "", "", "");

                eRequest request = NetMiner.Core.Url.UrlPack.GetRequest(url,cookie, cGlobalParas.WebCode.auto,
                    false, false, cGlobalParas.WebCode.auto, "",
                                    null, "", true);

                eResponse response = NetMiner.Net.Unity.RequestUri(Program.getPrjPath(), request, false);
                string htmlsource = response.Body;

                htmlsource = new Regex(@"(?m)<script[^>]*>(\w|\W)*?</script[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(htmlsource, "");

                //gWeb = null;

                this.m_HtmlSource = ToolUtil.ConvertToAbsoluteUrls(htmlsource, new Uri(url));
                HtmlAgilityPack.HtmlDocument mDoc = new HtmlAgilityPack.HtmlDocument();
                mDoc.LoadHtml(m_HtmlSource);
                this.m_HtmlSource = mDoc.DocumentNode.OuterHtml;

                htmlsource = this.m_HtmlSource;

                this.webBrowser1.DocumentText = htmlsource;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("网址有错误，" + ex.Message, "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                GotoUrl(this.txtUrl.Text);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (((this.webBrowser1.ReadyState == WebBrowserReadyState.Complete) ? 0 : 1) != 0)
            {
                return;
            }
            DoDocComplete();
        }

        private void DoDocComplete()
        {

            m_Doc = new HtmlAgilityPack.HtmlDocument();

            this.txtSource.Text = this.m_HtmlSource;
            string htmlSource = this.m_HtmlSource;
            Match s = Regex.Match(htmlSource, @"<body[\S\s]*</body>", RegexOptions.IgnoreCase);
            htmlSource = s.Groups[0].Value.ToString();

            m_Doc.LoadHtml(htmlSource);

            this.DomTree.Nodes.Clear();

            //先建立一个根节点
            TreeNode tNode = new TreeNode();
            tNode.Text = "DOMTree";
            tNode.Name = "nodRoot";
            tNode.Tag = "DOMTree";

            BuildTree(m_Doc.DocumentNode, tNode);
            this.DomTree.Nodes.Add(tNode);

            m_tableNodeIndex = 0;
            
        }

        private void BuildTree(HtmlAgilityPack.HtmlNode hNode, TreeNode tNode)
        {

            for (int i = 0; i < hNode.ChildNodes.Count; i++)
            {
                if (hNode.ChildNodes[i].NodeType == HtmlNodeType.Element)
                {
                    TreeNode Node = new TreeNode();


                    if (hNode.ChildNodes[i].ChildNodes.Count == 1 && hNode.ChildNodes[i].ChildNodes[0].NodeType == HtmlNodeType.Text)
                    {
                        Node.Text = hNode.ChildNodes[i].OuterHtml;
                        Node.Tag = hNode.ChildNodes[i].OuterHtml;

                    }
                    else if (hNode.ChildNodes[i].ChildNodes.Count != 0 )// && hNode.ChildNodes[i].ChildNodes[0].NodeType != HtmlNodeType.Text) 
                    {
                        string ss = hNode.ChildNodes[i].OuterHtml;
                        ss = ss.Substring(0, ss.IndexOf(">") + 1);
                        Node.Text = ss;
                        Node.Tag = hNode.ChildNodes[i].OuterHtml;
                        BuildTree(hNode.ChildNodes[i], Node);
                    }
                    else
                    {
                        Node.Text = hNode.ChildNodes[i].OuterHtml;
                        Node.Tag = hNode.ChildNodes[i].OuterHtml;
                    }

                    if (Node.Text != "" && Node.Text != null)
                    {
                        //this.textBox2.Text += hNode.ChildNodes[i].InnerHtml + "\r\n";
                        tNode.Nodes.Add(Node);
                    }
                }
                //else
                //{
                //    TreeNode Node = new TreeNode();
                //    Node.Text = hNode.ChildNodes[i].OuterHtml;
                //        Node.Tag = hNode.ChildNodes[i].OuterHtml;
                //}
            }

        }

        private void webBrowser1_DocumentCompleted_1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void toolAllow_Click(object sender, EventArgs e)
        {
            if (this.webBrowser1.Document == null)
                return;

            if (this.toolAllow.Checked == false)
            {

                this.webBrowser1.Document.MouseOver += new HtmlElementEventHandler(Document_MouseOver);
                this.webBrowser1.Document.MouseMove += new HtmlElementEventHandler(Document_MouseMove);
                this.webBrowser1.Document.MouseDown += new HtmlElementEventHandler(Document_MouseDown);

                m_hwnd = webBrowser1.Handle;
                m_hwnd = GetWindow(m_hwnd, (uint)5); // shell
                m_hwnd = GetWindow(m_hwnd, (uint)5); // doc obj
                m_hwnd = GetWindow(m_hwnd, (uint)5); // window
                m_browserPen = new Pen(Color.Navy, 1);

                this.toolAllow.Checked = true;
                //this.webBrowser1.AllowNavigation = false;

            }
            else
            {
                this.webBrowser1.Document.MouseOver -= new HtmlElementEventHandler(Document_MouseOver);
                this.webBrowser1.Document.MouseMove -= new HtmlElementEventHandler(Document_MouseMove);
                this.webBrowser1.Document.MouseDown -= new HtmlElementEventHandler(Document_MouseDown);

                this.toolAllow.Checked = false;
                //this.webBrowser1.AllowNavigation = true;
            }
        }

        void Document_MouseMove(object sender, HtmlElementEventArgs e)
        {
            // 得到当前鼠标所在元素的位置
            if (m_elemRect.Width == 0)
                m_elemRect = getElementPosFromPoint(webBrowser1.Document, m_curElem, e);
            // 画框
            Graphics g = Graphics.FromHwnd(m_hwnd);
            g.DrawRectangle(m_browserPen, m_elemRect);
            g.Dispose();
        }

        void Document_MouseOver(object sender, HtmlElementEventArgs e)
        {
            // 让browser控件绘图，删除框框           
            tagRECT rect = new tagRECT(0, 0, webBrowser1.Width, webBrowser1.Height);
            InvalidateRect(m_hwnd, ref rect, false);

            // 保存当前的element
            m_elemRect.Width = m_elemRect.Height = 0;
            m_curElem = e.ToElement;

        }

        void Document_MouseDown(object sender, HtmlElementEventArgs e)
        {
            HtmlElement elem = m_curElem;

            if (elem.OuterHtml.IndexOf("<body",StringComparison.CurrentCultureIgnoreCase) > -1)
                return;

            //记录父元素，前一个元素，下一个元素
            m_curFatherElem = m_curElem.Parent;
            m_curNextElem = m_curElem.NextSibling;

            HtmlElement felem = m_curElem.Parent;
            HtmlElement nelem = m_curElem.NextSibling;

            #region
       
            

            #endregion

            string s1 = elem.OuterHtml;
            s1 = Regex.Replace(s1, "([\\r\\n])[\\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            s1 = s1.Replace("\\r\\n", "");
            s1 = HtmlExtract.Utility.HtmlHelper.HtmlFormat(s1);

            TreeNode hNode1 = SearchTreeNode(this.DomTree.Nodes[0], s1);
            if (hNode1 != null)
            {
                this.DomTree.SelectedNode = hNode1;
            }

            if (this.isMatchUrl.Checked == true)
            {
                if (hNode1 == null)
                    return;

                string strXPath = searchNode(this.m_Doc.DocumentNode, elem);

                if (strXPath != "" && strXPath.ToLower().StartsWith("/html[1]"))
                {
                    strXPath = strXPath.Substring(8, strXPath.Length - 8);
                }

              
            }
            else
            {
                if (this.raXPath.Checked == true)
                {
                    string strXPath = searchNode(this.m_Doc.DocumentNode, elem);

                    if (strXPath != "" && strXPath.ToLower().StartsWith("/html[1]"))
                    {
                        strXPath = strXPath.Substring(8, strXPath.Length - 8);
                    }

                    if (this.isMulti.Checked == true)
                    {
                        if (m_MouseDownCount == 0)
                        {
                            m_MouseDownCount++;

                            if (this.dataRule.SelectedCells.Count > 0)
                            {
                                int rowIndex = this.dataRule.SelectedCells[0].RowIndex;
                                this.dataRule.Rows[rowIndex].Cells[1].Value = "XPath";
                                this.dataRule.Rows[rowIndex].Cells[2].Value = strXPath;
                                return;
                            }
                        }
                        else if (m_MouseDownCount > 0)
                        {

                            if (this.dataRule.SelectedCells.Count > 0)
                            {
                                int rowIndex = this.dataRule.SelectedCells[0].RowIndex;

                                this.dataRule.Rows[rowIndex].Cells[1].Value = "XPath";
                                this.dataRule.Rows[rowIndex].Cells[2].Value = ToolUtil.GetXpathParaString(this.dataRule.Rows[rowIndex].Cells[2].Value.ToString(), strXPath); ;

                            }

                            m_MouseDownCount = 0;
                            this.isMulti.Checked = false;
                        }
                    }
                    else
                    {
                        if (this.dataRule.SelectedCells.Count > 0)
                        {
                            int rowIndex = this.dataRule.SelectedCells[0].RowIndex;

                            this.dataRule.Rows[rowIndex].Cells[1].Value = "XPath";
                            this.dataRule.Rows[rowIndex].Cells[2].Value = strXPath;
                        }
                    }
                }
                else if (this.raCustom.Checked == true)
                {
                    //处理自定义的数据
                    if (hNode1 == null)
                        return;

                    string strTag = hNode1.Tag.ToString();

                    string strStart = strTag.Substring(0, strTag.IndexOf(">") + 1);
                    string strEnd = strTag.Substring(strTag.LastIndexOf("</"), strTag.Length - strTag.LastIndexOf("</"));

                    if (this.dataRule.SelectedCells.Count > 0)
                    {
                        int rowIndex = this.dataRule.SelectedCells[0].RowIndex;

                        
                        this.dataRule.Rows[rowIndex].Cells[1].Value = "自定义";

                        string strReg="(?<=" + strStart + ").*?(?=" + ToolUtil.RegexReplaceTrans(strEnd, true) + ")";
                        strReg = Regex.Replace(strReg, "([\\r\\n])[\\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        strReg = Regex.Replace(strReg, "\\n", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                        this.dataRule.Rows[rowIndex].Cells[2].Value = strReg; 

                    }


                }
            }

            //捕获完成，则取消
            this.webBrowser1.Document.MouseOver -= new HtmlElementEventHandler(Document_MouseOver);
            this.webBrowser1.Document.MouseMove -= new HtmlElementEventHandler(Document_MouseMove);
            this.webBrowser1.Document.MouseDown -= new HtmlElementEventHandler(Document_MouseDown);

            this.toolAllow.Checked = false;
            //this.webBrowser1.AllowNavigation = true;
        }


        private string searchNode(HtmlAgilityPack.HtmlNode pNode, HtmlElement cElem)
        {
            if (pNode == null)
                return "";

            //计算xpath
            //在树结构中查找符合条件的节点
            string strHtml = HtmlExtract.Utility.HtmlHelper.HtmlFormat(cElem.OuterHtml);
            strHtml = Regex.Replace(strHtml, "([\\r\\n])[\\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            strHtml = Regex.Replace(strHtml, "\\n", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            strHtml = strHtml.Replace("\\r\\n", "").ToLower();

            string s1 = strHtml.ToLower();

            string xPath = "";

            bool isImg = false;
            if (ToolUtil.getTxt(s1) == "")
            {
                //有可能匹配的是图片
                isImg = true;
            }

            for (int i = 0; i < pNode.ChildNodes.Count; i++)
            {
                string str = pNode.ChildNodes[i].OuterHtml;
                str = Regex.Replace(str, "([\\r\\n])", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                str = str.Replace("\\r\\n", "");

                //都转成小写字符
                str = HtmlExtract.Utility. HtmlHelper.HtmlFormat(str.ToLower());

                if (isImg == false)
                {
                    if (ToolUtil.getTxtByVisual(str) == ToolUtil.getTxtByVisual(s1))
                    {
                        //开始校验父节点和下一个节点是否相等，如果相等，则匹配
                        if (pNode.ChildNodes[i].XPath.IndexOf("#") > 0)
                        {
                        }
                        else
                        {
                            string ss1 = ToolUtil.getTxtByVisual(str);
                            string ss2 = ToolUtil.getTxtByVisual(s1);

                            if (ss1.IndexOf("href=") > -1 && ss2.IndexOf("href=") > -1)
                            {
                                Match s = Regex.Match(ss1, "(?<=href=)[^>]+?(?=[>|'|\"|\\s])", RegexOptions.IgnoreCase);
                                ss1 = s.Groups[0].Value.ToString();

                                s = Regex.Match(ss2, "(?<=href=)[^>]+?(?=[>|'|\"|\\s])", RegexOptions.IgnoreCase);
                                ss2 = s.Groups[0].Value.ToString();
                                if (ss1 == ss2)
                                {
                                    return pNode.ChildNodes[i].XPath;
                                }
                            }
                            else
                            {
                                if (ss1.IndexOf("href=") > -1 || ss2.IndexOf("href=") > -1)
                                {
                                }
                                else
                                {
                                    return pNode.ChildNodes[i].XPath;
                                }
                            }


                        }
                    }
                    else
                    {
                        xPath = searchNode(pNode.ChildNodes[i], cElem);
                        if (xPath != "")
                            return xPath;
                    }
                }
                else
                {
                    if (str == s1)
                    {
                        //开始校验父节点和下一个节点是否相等，如果相等，则匹配

                        return pNode.ChildNodes[i].XPath;
                    }
                    else
                    {
                        xPath = searchNode(pNode.ChildNodes[i], cElem);
                        if (xPath != "")
                            return xPath;
                    }
                }
            }

            return xPath;

        }

        /// <summary>
        /// 精确查找树形节点
        /// </summary>
        /// <param name="hNode"></param>
        /// <param name="ss"></param>
        /// <returns></returns>
        private TreeNode SearchTreeNode(TreeNode hNode, string ss)
        {
            if (hNode == null) return null;

            ss = ss.Replace("\"", "");
            ss = ss.Replace("'", "");

            string s1 = hNode.Tag.ToString ();
            s1 = s1.Replace("\"", "");
            s1 = s1.Replace("'", "");


            //if (string.Compare(s1, ss, true) == 0) 
            if (string.Compare(ToolUtil.getTxt ( s1),ToolUtil.getTxt ( ss), true) == 0)
                return hNode;

            TreeNode tnRet = null;
            foreach (TreeNode tn in hNode.Nodes)
            {
                tnRet = SearchTreeNode(tn, ss);
                if (tnRet != null) break;
            }
            return tnRet;
        }

        /// <summary>
        /// 模糊查找树形节点，根据起始字符查找
        /// </summary>
        /// <param name="hNode"></param>
        /// <param name="ss"></param>
        /// <returns></returns>
        private TreeNode SearchTreeNode1(TreeNode hNode, string ss,ref int index)
        {
            if (hNode == null) return null;

            index++;
            
            ss = ss.Replace("\"", "");
            ss = ss.Replace("'", "");

            string s1 = hNode.Tag.ToString();
            s1 = s1.Replace("\"", "");
            s1 = s1.Replace("'", "");


            //if (string.Compare(s1, ss, true) == 0) return hNode;
            if (index > this.m_tableNodeIndex)
            {
                if (s1.StartsWith(ss, StringComparison.CurrentCultureIgnoreCase)) return hNode;
            }

            TreeNode tnRet = null;
            foreach (TreeNode tn in hNode.Nodes)
            {
                tnRet = SearchTreeNode1(tn, ss,ref index);
                if (tnRet != null) break;
            }
            return tnRet;
        }

        private TreeNode SearchTreeNode2(TreeNode hNode, string ss,int startIndex,ref int index)
        {
            if (hNode == null) return null;

            index++;

            ss = ss.Replace("\"", "");
            ss = ss.Replace("'", "");

            string s1 = hNode.Tag.ToString();
            s1 = s1.Replace("\"", "");
            s1 = s1.Replace("'", "");


            if (index > startIndex)
            {
                if (s1.StartsWith(ss, StringComparison.CurrentCultureIgnoreCase)) return hNode;
            }

            TreeNode tnRet = null;
            foreach (TreeNode tn in hNode.Nodes)
            {
                tnRet = SearchTreeNode2(tn, ss, startIndex ,ref index);
                if (tnRet != null) break;
            }
            return tnRet;
        }

        Rectangle getElementPosFromPoint(System.Windows.Forms.HtmlDocument doc, HtmlElement elem, HtmlElementEventArgs e)
        {

            Rectangle ret = new Rectangle();

            if (elem == null)
                return ret;

            // 为A 或者只有文本的节点，判断边界（TODO: 最好用二分法判断）
            if (/*elem.TagName.Equals("A") || */(elem.Children.Count == 0 && elem.InnerText != null && elem.InnerText.Length > 0))
            {
                int lowX = e.ClientMousePosition.X - elem.OffsetRectangle.Width;
                if (lowX < 0)
                    lowX = 0;
                int highX = e.ClientMousePosition.X + elem.OffsetRectangle.Width;

                int lowY = e.ClientMousePosition.Y - elem.OffsetRectangle.Height;
                if (lowY < 0)
                    lowY = 0;
                int highY = e.ClientMousePosition.Y + elem.OffsetRectangle.Height;

                Point p = new Point();
                for (int i = e.ClientMousePosition.X; i >= lowX; i--)
                {
                    p.X = i;
                    p.Y = e.ClientMousePosition.Y;
                    if (elem != doc.GetElementFromPoint(p))
                        break;
                }
                int realLowX = p.X;

                for (int i = e.ClientMousePosition.X; i < highX; i++)
                {
                    p.X = i;
                    p.Y = e.ClientMousePosition.Y;
                    if (elem != doc.GetElementFromPoint(p))
                        break;
                }
                int realHighX = p.X;

                for (int j = e.ClientMousePosition.Y; j >= lowY; j--)
                {
                    p.X = e.ClientMousePosition.X;
                    p.Y = j;
                    if (elem != doc.GetElementFromPoint(p))
                        break;
                }
                int realLowY = p.Y;

                for (int j = e.ClientMousePosition.Y; j <= highY; j++)
                {
                    p.X = e.ClientMousePosition.X;
                    p.Y = j;
                    if (elem != doc.GetElementFromPoint(p))
                        break;
                }
                int realHighY = p.Y;

                ret.X = realLowX;
                ret.Y = realLowY;
                ret.Width = realHighX - realLowX;
                ret.Height = realHighY - realLowY;
            }
            else
            {
                // 采用mouse off set判断边界
                int x = e.ClientMousePosition.X - e.OffsetMousePosition.X - elem.ClientRectangle.Left;
                int y = e.ClientMousePosition.Y - e.OffsetMousePosition.Y - elem.ClientRectangle.Top;
                int w = elem.OffsetRectangle.Width; // notice：w可能比上面计算出来的ret.Width小；
                int h = elem.OffsetRectangle.Height; //notice：h可能比上面计算出来的ret.Height小；

                // 计算出错（即不包含鼠标点击的位置），则采用标准的排版计算方式
                // TODO: 如果可以最好计算上面已经计算的ret结果是否包含在新范围内
                //if (x > ret.X + 2 || y > ret.Y + 2 || x + w < ret.X + ret.Width - 2 || y + h < ret.Y + ret.Height - 2)
                if (x + w < e.ClientMousePosition.X ||
                    y + h < e.ClientMousePosition.Y)
                {
                    HtmlElement cur = elem;
                    x = 0;
                    y = 0;
                    while (cur != null)
                    {
                        x += cur.OffsetRectangle.Left - cur.ScrollLeft; // TODO: 需要加上style.borderLeftWidth
                        y += cur.OffsetRectangle.Top - cur.ScrollTop;

                        cur = cur.OffsetParent;
                    }

                    // 把坐标映射到当前窗口
                    //mshtml.IHTMLDocument3 doc3 = doc.DomDocument as mshtml.IHTMLDocument3;
                    //mshtml.IHTMLElement2 docElem2 = doc3.documentElement as mshtml.IHTMLElement2;
                    //x -= docElem2.scrollLeft;
                    //y -= docElem2.scrollTop;

                    x -= doc.Body.Parent.ScrollLeft;
                    y -= doc.Body.Parent.ScrollTop;
                }

                // 计算出错（即不包含鼠标点击的位置），采用已经计算的结果
                // TODO: 如果可以最好计算上面已经计算的ret结果是否包含在新范围内
                //if (x > ret.X + 2 || y > ret.Y + 2 || x + w < ret.X + ret.Width - 2 || y + h < ret.Y + ret.Height - 2)
                if (x + w < e.ClientMousePosition.X ||
                    y + h < e.ClientMousePosition.Y)
                {

                }
                else
                {
                    ret.X = x;
                    ret.Y = y;
                    ret.Width = w;
                    ret.Height = h;
                }
            }

            return ret;
        }

        private void frmSmartGRule_Load(object sender, EventArgs e)
        {
            this.gType.Items.Add("自动采集标题");
            this.gType.Items.Add("自动采集发布时间");
            this.gType.Items.Add("自动采集正文");
            this.gType.Items.Add("XPath");
            this.gType.Items.Add("自定义");

            this.splitContainer1.SplitterDistance = 300;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Test(this.txtSource.Text);
        }

        //只能匹配只能匹配文章，返回的数据只能是一行
        private void Test(string source)
        {
            if (this.txtUrl.Text.Trim() == "")
            {
                MessageBox.Show("请先输入网址！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtUrl.Focus();
                return;
            }

            NetMiner.Gather.Control.cGatherWeb gWeb = new NetMiner.Gather.Control.cGatherWeb(Program.getPrjPath());

            List<eWebpageCutFlag> CutFlags = new List<eWebpageCutFlag>();
            

            for (int i = 0; i < this.dataRule.Rows.Count ; i++)
            {
                if (this.dataRule.Rows[i].Cells[0].Value != null &&
                    this.dataRule.Rows[i].Cells[1].Value != null &&
                    this.dataRule.Rows[i].Cells[2].Value != null)
                {
                    eWebpageCutFlag c = new eWebpageCutFlag();
                    c.id = i;
                    c.Title = this.dataRule.Rows[i].Cells[0].Value.ToString();
                    if (c.Title == "")
                    {
                        MessageBox.Show("请输入采集数据的名称！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    c.RuleByPage = cGlobalParas.GatherRuleByPage.GatherPage;


                    switch (this.dataRule.Rows[i].Cells[1].Value.ToString())
                    {
                        case "自动采集标题":
                            c.GatherRuleType = cGlobalParas.GatherRuleType.Smart;
                            c.DataType =cGlobalParas.GDataType.AutoTitle;
                            break;
                        case "自动采集发布时间":
                            c.GatherRuleType = cGlobalParas.GatherRuleType.Smart;
                            c.DataType = cGlobalParas.GDataType.AutoPublishDate;
                            break;
                        case "自动采集正文":
                            c.GatherRuleType = cGlobalParas.GatherRuleType.Smart;
                            c.DataType = cGlobalParas.GDataType.AutoContent;
                            break;
                        case "XPath":
                            c.GatherRuleType = cGlobalParas.GatherRuleType.XPath;
                            c.DataType =cGlobalParas.GDataType.Txt;
                            break;
                        case "自定义":
                            c.GatherRuleType = cGlobalParas.GatherRuleType.Normal;
                            c.DataType =cGlobalParas.GDataType.Txt;
                            break;
                    }


                    c.XPath = this.dataRule.Rows[i].Cells[2].Value.ToString();
                    c.NodePrty = "InnerHtml";
                    c.StartPos = "";
                    c.EndPos = "";

                    if (this.dataRule.Rows[i].Cells[1].Value.ToString() == "自定义")
                        c.LimitSign =cGlobalParas.LimitSign.Custom;
                    else
                        c.LimitSign = cGlobalParas.LimitSign.NoLimit;
                    c.RegionExpression = this.dataRule.Rows[i].Cells[2].Value.ToString();
                    c.IsMergeData = false;
                    c.NavLevel = 0;

                    //多页名称
                    c.MultiPageName = "";
                    c.DownloadFileSavePath = "";
                    //c.DownloadFileDealType = "";
                    c.IsAutoDownloadFileImage = false;

                    if (this.IsWebCode.Checked ==true )
                    {
                        eFieldRule fRule=new eFieldRule ();
                        fRule.FieldRuleType=cGlobalParas.ExportLimit.ExportNoWebSign;
                        fRule.FieldRule="";
                        c.ExportRules.Add(fRule);
                    }
                    CutFlags.Add(c);
                }

            }

            gWeb.CutFlag = CutFlags;

            string cookie = "";
            string sPath = Program.getPrjPath() + "\\data";
            DataTable d=null;
            try
            {
                d= gWeb.GetGatherData(this.txtUrl.Text,"","","",false,false,null,"",0,"",cGlobalParas.RejectDeal.None);
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("采集数据发生错误，错误信息：" + ex.Message, "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtResult.Text = "采集数据发生错误，错误信息：" + ex.Message;
                return;
            }

            this.txtResult.Text = "";
            if (d == null || d.Rows.Count ==0)
                return;

            string ss="";
            for (int i = 0; i < d.Rows.Count; i++)
            {
                ss = ss + "\r\n---------第" + (i + 1) + "条数据-------------\r\n";

                for (int j = 0; j < d.Columns.Count; j++)
                {
                    ss += "【" + d.Columns[j].ColumnName + "】" + d.Rows[i][j].ToString() + "\r\n";
                }
            }
            this.txtResult.Text = ss;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.dataRule.Rows.Add("标题", "自动采集标题", "");
            this.dataRule.Rows.Add("发布时间", "自动采集发布时间", "");
            this.dataRule.Rows.Add("正文", "自动采集正文", "");
        }

        private void toolCancleExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //先查询Node的节点
            try
            {
                int index = 0;
                if (this.m_HtmlSource == null || this.DomTree.Nodes.Count == 0)
                    return;

                TreeNode hNode1 = SearchTreeNode1(this.DomTree.Nodes[0], "<Table", ref index);
                if (hNode1 == null)
                {
                    this.m_tableNodeIndex = 0;
                    return;
                }
                else
                    this.m_tableNodeIndex = index;


                string source = hNode1.Tag.ToString();

                bool isTTitle = false;

                //获取<th>标签
                int thIndex = 0;
                TreeNode thNode = SearchTreeNode1(hNode1, "<th", ref thIndex);
                if (thNode != null)
                {

                    this.dataRule.Rows.Clear();

                    //找到了tr节点
                    for (int i = 0; i < thNode.Nodes.Count; i++)
                    {
                        this.dataRule.Rows.Add(ToolUtil.getTxt(thNode.Nodes[i].Tag.ToString()), "自定义", "");

                        isTTitle = true;
                    }

                }

                //获取<tr>标签
                int index1 = 0;
                int trIndex = 0;
                if (isTTitle == false)
                {
                    TreeNode trNode = SearchTreeNode2(hNode1, "<tr", trIndex, ref index1);
                    if (trNode != null)
                    {

                        this.dataRule.Rows.Clear();

                        //找到了tr节点
                        for (int i = 0; i < trNode.Nodes.Count; i++)
                        {
                            this.dataRule.Rows.Add(ToolUtil.getTxt(trNode.Nodes[i].Tag.ToString()), "自定义", "");
                        }
                        trIndex = index1;
                    }
                }

                //在此tr节点基础上，再查找另一个tr，进行规则获取
                index1 = 0;
                TreeNode trNode1 = SearchTreeNode2(hNode1, "<tr", trIndex, ref index1);
                if (trNode1 != null)
                {


                    //找到了tr节点
                    for (int i = 0; i < trNode1.Nodes.Count; i++)
                    {
                        string strStart = trNode1.Nodes[i].Tag.ToString();
                        strStart = strStart.Substring(0, strStart.IndexOf(">") + 1);
                        this.dataRule.Rows[i].Cells[2].Value = "(?<=" + strStart + ").*?(?=</td>)";
                    }
                    trIndex = index1;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("自动获取表格出错，请手工配置！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //string strCut = "(?<=<tr.+?>).+?(?=</tr>)";
            //Regex re = new Regex(@strCut, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //MatchCollection mc = re.Matches(source);


            //if (mc.Count > 0)
            //{
            //    //表示匹配到了表格，通常第一个为表头，先获取表头的信息
            //    string tTitle = mc[0].ToString();

            //    string strCut1 = "(?<=<td.+?>).+?(?=</td>)";
            //    Regex re1 = new Regex(strCut1, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //    MatchCollection mc1 = re.Matches(source);
            //}
        }

        private string Analy(string str)
        {
           
            return str;

        }

        private void raXPath_CheckedChanged(object sender, EventArgs e)
        {
            if (raXPath.Checked == true)
            {
                this.isMulti.Enabled = true;
            }
        }

        private void raCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (raCustom.Checked == true)
                this.isMulti.Enabled = false;
        }

        private void toolOkExit_Click(object sender, EventArgs e)
        {
          
            List<cGatherRule> rules = new List<cGatherRule>();
 
            for (int i = 0; i < this.dataRule.Rows.Count; i++)
            {
                if (this.dataRule.Rows[i].Cells[0].Value != null &&
                     this.dataRule.Rows[i].Cells[1].Value != null &&
                     this.dataRule.Rows[i].Cells[2].Value != null)
                {
                    cGatherRule gRule = new cGatherRule();
                    gRule.fState = cGlobalParas.FormState.New;
                    if (this.dataRule.Rows[i].Cells[0].Value.ToString() == "")
                        gRule.gName  = "采集数据" + i;
                    else
                        gRule.gName = this.dataRule.Rows[i].Cells[0].Value.ToString();

                    gRule.getStart = "";
                    gRule.getEnd = "";
                    
                    gRule.strReg = this.dataRule.Rows[i].Cells[2].Value.ToString();
                    gRule.IsMergeData = false;
                    gRule.gRuleByPage = cGlobalParas.GatherRuleByPage.GatherPage;
                    gRule.NaviLevel = "-1";
                    gRule.MultiPageName = "";
                    gRule.sPath = "";
                    //gRule.fileDealType = "";

                    if (this.dataRule.Rows[i].Cells[1].Value.ToString() == "自定义")
                    {
                        gRule.gRuleType = cGlobalParas.GatherRuleType.Normal;
                        gRule.gNodePrty = "";
                        gRule.gType = "文本";
                        gRule.limitType = "自定义正则匹配表达式";
                    }
                    else if (this.dataRule.Rows[i].Cells[1].Value.ToString() == "XPath")
                    {
                        gRule.gRuleType = cGlobalParas.GatherRuleType.XPath;
                        gRule.gNodePrty = "InnerHtml";
                        gRule.xPath = this.dataRule.Rows[i].Cells[2].Value.ToString();
                        gRule.gType = "文本";
                        gRule.limitType = "";
                    }
                    else
                    {
                        gRule.limitType = "";
                        gRule.gRuleType = cGlobalParas.GatherRuleType.Smart;
                        gRule.gNodePrty = "";
                        if (this.dataRule.Rows[i].Cells[1].Value.ToString() == "自动采集标题")
                            gRule.gType = "智能提取页面标题";
                        else if (this.dataRule.Rows[i].Cells[1].Value.ToString() == "自动采集正文")
                            gRule.gType = "智能提取文章正文";
                        else if (this.dataRule.Rows[i].Cells[1].Value.ToString() == "自动采集发布时间")
                            gRule.gType = "智能提取发布时间";
                    }

                    gRule.IsAutoDownloadFileImage = false;
                    gRule.IsAutoDownloadOnlyImage = false;

                    eFieldRules fRules = new eFieldRules();
                    fRules.Field = gRule.gName;

                    List<eFieldRule> listcf = new List<eFieldRule>();
                    fRules.Field = gRule.gName;
                    fRules.FieldRule = listcf;

                    gRule.fieldDealRules = fRules;

                    rules.Add(gRule);
                }
            }

            rGatherRule(rules);
            this.Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (this.txtSource.Text.Trim() == "")
                return;

            this.txtSource.Text = ToolUtil.convertU2CN(this.txtSource.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //先查询Node的节点，查询节点有<li <tr <td
            try
            {
                int index = 0;
                if (this.m_HtmlSource == null || this.DomTree.Nodes.Count == 0)
                    return;

                TreeNode hNode1 = SearchTreeNode1(this.DomTree.Nodes[0], "<li", ref index);
                if (hNode1 == null)
                {
                    this.m_tableNodeIndex = 0;
                    return;
                }
                else
                    this.m_tableNodeIndex = index;
            }
            catch (System.Exception ex)
            {

            }
        }

        private void webBrowser1_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void isMatchUrl_CheckedChanged(object sender, EventArgs e)
        {
            if (this.isMatchUrl.Checked == true)
            {
                this.raXPath.Enabled = false;
                this.raCustom.Enabled = false;
                this.isMulti.Enabled = false;
                this.button3.Enabled = false;
                this.button4.Enabled = false;
            }
            else
            {
                this.raXPath.Enabled = true;
                this.raCustom.Enabled = true;
                this.isMulti.Enabled = true;
                this.button3.Enabled = true;
                this.button4.Enabled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //查找网页代码中存在的 {  } 对，并对其内部包含的数据进行识别
            string strCut = "(?<={)[^}]+?(?=})";
            Regex re = new Regex(@strCut, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            MatchCollection mc = re.Matches(this.m_HtmlSource);

            foreach (Match ma in mc)
            {

            }
        }

       

        private void webBrowser1_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            if (((System.Windows.Forms.WebBrowser)sender).ReadyState == WebBrowserReadyState.Complete)
            {
                DoDocComplete();
            }
        }
    }
}

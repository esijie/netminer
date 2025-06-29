using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using NetMiner.Gather;
using NetMiner.Resource;
using NetMiner.Common;
using Gecko;
using HtmlAgilityPack;

namespace MinerSpider
{
    [ComVisible(true), StructLayout(LayoutKind.Sequential)]
    public struct tagRECT
    {
        [MarshalAs(UnmanagedType.I4)]
        public int Left;
        [MarshalAs(UnmanagedType.I4)]
        public int Top;
        [MarshalAs(UnmanagedType.I4)]
        public int Right;
        [MarshalAs(UnmanagedType.I4)]
        public int Bottom;

        public tagRECT(int left_, int top_, int right_, int bottom_)
        {
            Left = left_;
            Top = top_;
            Right = right_;
            Bottom = bottom_;
        }

    }


    public partial class frmVisual : Form
    {
        public delegate void RuturnxPath(string xPath, string hNodeText);
        public RuturnxPath rxPath;

        private string m_workPath;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern bool InvalidateRect(IntPtr hwnd, ref tagRECT lpRect, bool bErase);

        IntPtr m_hwnd;
        Pen m_browserPen;

        Rectangle m_elemRect;
        HtmlElement m_curElem;

        private string m_HtmlSource;

        HtmlAgilityPack.HtmlDocument m_Doc;
        //cHtmlTree m_hTree;

        private bool m_IsManual = false;

        /// <summary>
        /// 阻止浏览器跳转操作
        /// </summary>
        private bool m_isBlocking = false;

        /// <summary>
        /// 标识当前的选择模式是两次选择，通过选择第一个和最后一个实现多条规则获取
        /// </summary>
        private bool m_isDoubleSelect = false;


        //记录鼠标点击次数，在多条捕获下有效
        private int m_MouseDownCount = 0;

        private string m_Cookie="";

        public frmVisual(string url,string cookie)
        {
            InitializeComponent();

            IniBrowser(m_workPath, Program.g_xulPath);

            if (!string.IsNullOrEmpty(url))
            {
                NetMiner.Core.Url.cUrlParse cu = new NetMiner.Core.Url.cUrlParse(Program.getPrjPath());
                List<string> urls = cu.SplitWebUrl(url);

                if (urls == null || urls.Count == 0)
                    this.txtUrl.Text = url;
                else
                    this.txtUrl.Text = urls[0];
                m_Cookie = cookie;
            }

            m_workPath = Program.getPrjPath();

            

        }

        #region 浏览器操作部分
        public void IniBrowser(string workPath, string xulrunnerPath)
        {

            if (!Gecko.Xpcom.IsInitialized)
                Gecko.Xpcom.Initialize(xulrunnerPath);
            //GeckoPreferences.User["gfx.font_rendering.graphite.enabled"] = true;

            this.tabPage3.Controls.Add(this._ActiveWebBrowser);
            //this._ActiveWebBrowser.is isDocumentCompleted = false;
            this._ActiveWebBrowser.DocumentCompleted += new EventHandler<Gecko.Events.GeckoDocumentCompletedEventArgs>(MyBrowser_DocumentCompleted);
            this._ActiveWebBrowser.NavigationError += new EventHandler<Gecko.Events.GeckoNavigationErrorEventArgs>(MyBrowser_NavigationError);
            this._ActiveWebBrowser.ProgressChanged += new EventHandler<GeckoProgressEventArgs>(MyBrowser_ProgressChanged);
            this._ActiveWebBrowser.DocumentTitleChanged += new EventHandler(MyBrowser_DocumentTitleChanged);
            this._ActiveWebBrowser.StatusTextChanged += new EventHandler(MyBrowser_StatusTextChanged);
            this._ActiveWebBrowser.Navigating += new EventHandler<Gecko.Events.GeckoNavigatingEventArgs>(MyBrowser_Navigating);
            this._ActiveWebBrowser.CreateWindow += new EventHandler<GeckoCreateWindowEventArgs>(MyBrowser_CreateNewWindows);
            //Gecko.LauncherDialog.Download += new EventHandler<LauncherDialogEvent>(Download_File);

            //m_tmpGData = new DataSet();
            //m_tmpGData.Tables.Add("gData");

        }

        #endregion

        #region 浏览器事件
        private void MyBrowser_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        {
            if (_ActiveWebBrowser.IsBusy == false && _ActiveWebBrowser.IsAjaxBusy == false)
            {
                //_ActiveWebBrowser.isDocumentCompleted = true;
                ProgressBar1.Value = 0;
                ProgressBar1.Visible = false;
                this.txtUrl.Text = e.Uri.ToString();
            }
        }

        private void MyBrowser_NavigationError(object sender, Gecko.Events.GeckoNavigationErrorEventArgs e)
        {
            //HE_WebBrowserTag _HE_WebBrowserTag = (HE_WebBrowserTag)_ActiveWebBrowser.Tag;
            //_ActiveWebBrowser.isDocumentCompleted = true;
            ProgressBar1.Value = 0;
            ProgressBar1.Visible = false;
            this.txtUrl.Text = e.Uri.ToString();
        }

        private void MyBrowser_ProgressChanged(object sender, GeckoProgressEventArgs e)
        {
            GeckoWebBrowser axWebBrowser1 = (GeckoWebBrowser)sender;
            //HE_WebBrowserTag _HE_WebBrowserTag = (HE_WebBrowserTag)axWebBrowser1.Tag;

            if (e.CurrentProgress == e.MaximumProgress)
                return;

            ProgressBar1.Visible = true;
            if ((e.CurrentProgress > 0) && (e.MaximumProgress > 0))
            {
                ProgressBar1.Maximum = (int)e.MaximumProgress;
                ProgressBar1.Step = (int)e.CurrentProgress;
                ProgressBar1.PerformStep();
            }

        }

        private void MyBrowser_DocumentTitleChanged(object sender, EventArgs e)
        {
            GeckoWebBrowser br = (GeckoWebBrowser)sender;
            this.WebBrowserTab.Text = br.DocumentTitle;
        }

        private void MyBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            GeckoWebBrowser br = (GeckoWebBrowser)sender;
            this.staInfo.Text = br.StatusText;
        }

        private void MyBrowser_Navigating(object sender, Gecko.Events.GeckoNavigatingEventArgs e)
        {

            GeckoWebBrowser br = (GeckoWebBrowser)sender;
            //br.isDocumentCompleted = false;
            if (m_isBlocking == true)
                e.Cancel = true;
        }

        private void MyBrowser_CreateNewWindows(object sender, GeckoCreateWindowEventArgs e)
        {
            e.Cancel = true;
            GeckoWebBrowser br = (GeckoWebBrowser)sender;
            br.Navigate(e.Uri.ToString());
        }
        #endregion

        /// <summary>
        /// 通过已有元素和事件信息计算节点的位置        /// 
        /// </summary>
        /// <param name="doc">document对象</param>
        /// <param name="elem">当前元素</param>
        /// <param name="e">鼠标事件</param>
        /// <returns></returns>
        Rectangle getElementPosFromPoint(System.Windows.Forms.HtmlDocument doc, HtmlElement elem, HtmlElementEventArgs e)
        {
            Rectangle ret = new Rectangle();

            if (elem == null)
                return ret;
            try
            {
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
            }
            catch (System.Exception ex)
            {
                return ret;
            }

            return ret;
        }

        private void Document_MouseDown(object sender, HtmlElementEventArgs e)
        {

            HtmlElement cElem = m_curElem;

            if (cElem.OuterHtml.IndexOf("<body", StringComparison.CurrentCultureIgnoreCase) > -1)
                return;

            //记录父元素和下一个元素
            HtmlElement felem = m_curElem.Parent;
            HtmlElement nelem = m_curElem.NextSibling;

            string xPath = "";
            try
            {
                xPath = searchNode(m_Doc.DocumentNode, cElem);

            }
            catch (System.Exception ex)
            {
                return;
            }
           if (this.isMulti.Checked == true)
           {
               if (m_MouseDownCount == 0)
               {
                   m_MouseDownCount++;
                   this.txtxPath.Text = xPath;
               }
               else if (m_MouseDownCount>0)
               {
                   this.txtxPath.Text = ToolUtil.GetXpathParaString(this.txtxPath.Text, xPath);

                   //捕获完成，则取消

                   this.toolAllowLink.Checked = false;
                   //this.webBrowser1.AllowNavigation = true;

                   m_MouseDownCount = 0;
                   this.isMulti.Checked = false;
               }
           }
           else
           {
               this.txtxPath.Text = xPath;

               //捕获完成，则取消

               this.toolAllowLink.Checked = false;
               //this.webBrowser1.AllowNavigation = true;
           }

            
        }

        
        private string searchNode(HtmlAgilityPack.HtmlNode pNode, HtmlElement cElem)
        {
            if (pNode == null)
                return "";

            //计算xpath
            //在树结构中查找符合条件的节点
            string strHtml = HtmlExtract.Utility. HtmlHelper.HtmlFormat(cElem.OuterHtml);
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
                string str = pNode.ChildNodes[i].OuterHtml ;
                str = Regex.Replace(str, "([\\r\\n])", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                str = str.Replace("\\r\\n", "");

                //都转成小写字符
                str =HtmlExtract.Utility. HtmlHelper.HtmlFormat(str.ToLower());

                if (isImg == false)
                {
                    if (ToolUtil.getTxtByVisual(str) == ToolUtil.getTxtByVisual (s1))
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
                                ss1= s.Groups[0].Value.ToString();

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
                        xPath = searchNode(pNode.ChildNodes[i],cElem);
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

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //if (this.webBrowser1.ReadyState != WebBrowserReadyState.Complete )
            //{
            //    return;
            //}

            DoDocComplete();
        }

        private void DoDocComplete()
        {
            //Encoding encoding = Encoding.GetEncoding(webBrowser1.Document.Encoding);
            //StreamReader stream = new StreamReader(webBrowser1.DocumentStream, encoding);
            //string htmlMessage = stream.ReadToEnd();

            //System.Windows.Forms.HtmlDocument htmlDoc = webBrowser1.Document;
            //HtmlElement btnElement = htmlDoc.Body;

            //string htmlMessage = btnElement.OuterHtml;

            string htmlSource = this.m_HtmlSource;
            Match s = Regex.Match(htmlSource, @"<body[\S\s]*</body>", RegexOptions.IgnoreCase);
            htmlSource = s.Groups[0].Value.ToString();

            m_Doc = new HtmlAgilityPack.HtmlDocument();
            m_Doc.LoadHtml(htmlSource);



            //定义一颗树
            //m_hTree = new cHtmlTree();
            //m_hTree.BuildTree(m_Doc.DocumentNode);

            //加载树
        }
  
        private void butUrl_Click(object sender, EventArgs e)
        {
            GotoUrl(this.txtUrl.Text );
        }

        private void txtUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GotoUrl(this.txtUrl.Text);
            }
        }

        private void GotoUrl(string url)
        {
            if (url.Trim() == "")
            {
                MessageBox.Show("请输入需要采集数据的网址！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtUrl.Focus();
                return;
            }

            if (this.toolAllowLink.Checked == true)
                this.toolAllowLink.Checked = false;

            //if (!url.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    url = "http://" + url;
            //}

            //this.m_HtmlSource = cTool.ConvertToAbsoluteUrls(this.m_HtmlSource, new Uri(url));
            //NetMiner.Gather.HtmlAgilityPack.HtmlDocument mDoc = new NetMiner.Gather.HtmlAgilityPack.HtmlDocument();
            //mDoc.LoadHtml(m_HtmlSource);
            //this.m_HtmlSource = mDoc.DocumentNode.OuterHtml;

            this.m_IsManual = true;
            
            this.txtUrl.Text =url ;

            this._ActiveWebBrowser.Navigate(url);

        }

        private void toolAllowLink_Click(object sender, EventArgs e)
        {

            if (this.toolAllowLink.Checked == false)
            {
                SelectXPath();
            }
            else
            {
                CancelCapture();
            }
        }

        private void SelectXPath()
        {
            if (this._ActiveWebBrowser.Document == null)
                return;

            this._ActiveWebBrowser.DomMouseMove += new EventHandler<DomMouseEventArgs>(Document_MouseMove);
            this._ActiveWebBrowser.DomMouseOver += new EventHandler<DomMouseEventArgs>(Document_MouseOver);
            this._ActiveWebBrowser.DomMouseDown += new EventHandler<DomMouseEventArgs>(Document_MouseDown);

            m_hwnd = _ActiveWebBrowser.Handle;
            m_hwnd = GetWindow(m_hwnd, (uint)5); // shell
            m_hwnd = GetWindow(m_hwnd, (uint)5); // doc obj
            m_browserPen = new Pen(Color.Navy, 2);

            this.toolAllowLink.Checked = true;

            m_isBlocking = true;
            this._ActiveWebBrowser.BlockEvent = true;
        }

        private void CancelCapture()
        {
            this._ActiveWebBrowser.DomMouseMove -= new EventHandler<DomMouseEventArgs>(Document_MouseOver);
            this._ActiveWebBrowser.DomMouseMove -= new EventHandler<DomMouseEventArgs>(Document_MouseMove);
            this._ActiveWebBrowser.DomMouseDown -= new EventHandler<DomMouseEventArgs>(Document_MouseDown);

            this.toolAllowLink.Checked = false;

            m_MouseDownCount = 0;
            m_isDoubleSelect = false;
            m_isBlocking = false;
            this._ActiveWebBrowser.BlockEvent = false;
        }

        private void Document_MouseMove(object sender, DomMouseEventArgs e)
        {

            GeckoWebBrowser br = (GeckoWebBrowser)sender;

            GeckoElement elem = e.Target.CastToGeckoElement();
            GeckoHtmlElement hElem = (GeckoHtmlElement)elem;

            string Xpath = ShowXPath(elem);

            Point cursorP = new Point(e.ScreenX, e.ScreenY);
            Point p = br.PointToClient(cursorP);

            string iFrameIndex = string.Empty;

            int offLeft = 0;
            int offTop = 0;

            int iFrameCount = 0;
            if (br.Document.GetElementsByTagName("iframe") != null)
                iFrameCount = br.Document.GetElementsByTagName("iframe").Length;

            if (iFrameCount > 0)
            {
                for (int i = 0; i < iFrameCount; i++)
                {

                    Gecko.DOM.GeckoIFrameElement iElem = (Gecko.DOM.GeckoIFrameElement)br.Document.GetElementsByTagName("iframe")[i];

                    //先计算当前IFrame的偏移量，并传入参数
                    int oLeft = 0;
                    int oTop = 0;
                    oLeft = iElem.OffsetLeft;
                    oTop = iElem.OffsetTop;

                    GeckoHtmlElement tmpElem = null;
                    tmpElem = (GeckoHtmlElement)iElem.OffsetParent;
                    while (tmpElem != null)
                    {
                        if (tmpElem.OffsetLeft > 0)
                            oLeft += tmpElem.OffsetLeft;
                        if (tmpElem.OffsetTop > 0)
                            oTop += tmpElem.OffsetTop;
                        tmpElem = (GeckoHtmlElement)tmpElem.OffsetParent; ;
                    }

                    //iFrameIndex = GetIframe(i.ToString(), iElem, oLeft, oTop, out oLeft, out oTop,
                    //    Xpath, hElem, cursorP, br.Window.Frames[(uint)i]);

                    //if (!string.IsNullOrEmpty(iFrameIndex))
                    //{
                    //    offLeft = oLeft;
                    //    offTop = oTop;
                    //    break;
                    //}
                }
            }

            //if (sXpath == cGlobal.SelectXpath.input)
            //{
            //    System.Windows.Forms.Control c = FindControl(this, this.activeControl);
            //    string cName = c.Name;
            //    c = FindControl(this, cName + "IFrame");
            //    if (c != null)
            //        c.Text = iFrameIndex;
            //}

            if (hElem == null)
                return;

            if (hElem.ClientRects.Length > 0)
            {
                m_elemRect.X = hElem.ClientRects[0].X + offLeft;
                m_elemRect.Y = hElem.ClientRects[0].Y + offTop;
                m_elemRect.Width = hElem.ClientRects[0].Width;
                m_elemRect.Height = hElem.ClientRects[0].Height;
            }


            // 画框
            Graphics g = Graphics.FromHwnd(m_hwnd);
            g.DrawRectangle(m_browserPen, m_elemRect);
            g.Dispose();
        }

        void Document_MouseOver(object sender, DomMouseEventArgs e)
        {
            tagRECT rect = new tagRECT(0, 0, this._ActiveWebBrowser.Width, _ActiveWebBrowser.Height);
            InvalidateRect(m_hwnd, ref rect, false);
        }

        void Document_MouseDown(object sender, DomMouseEventArgs e)
        {
            GeckoElement elem = e.Target.CastToGeckoElement();
            string strXpath = GetXpath(e.Target.CastToGeckoElement());

            m_MouseDownCount++;

            
            if (m_isDoubleSelect == false)
            {
                   
                    System.Windows.Forms.Control c = this.txtxPath;
                    c.Text = strXpath;
                    CancelCapture();
                    
            }
            else if (m_isDoubleSelect == true && m_MouseDownCount == 2)
            {
                    System.Windows.Forms.Control c = this.txtxPath;
                  
                        string ss = c.Text;
                        c.Text = NetMiner.Common.ToolUtil.GetXpathParaString(ss, strXpath);
                    
                    

                //捕获完成取消
                CancelCapture();
            }
            else if (m_isDoubleSelect == true && m_MouseDownCount < 2)
            {
                    System.Windows.Forms.Control c = this.txtxPath;
                    c.Text = strXpath;
            }
            


        }

        private string ShowXPath(GeckoNode node)
        {
            System.Windows.Forms.Control c = this.txtxPath;
            if (c != null)
            {
                if (m_isDoubleSelect == true && m_MouseDownCount == 2)
                {
                    string strXpath = c.Text;
                    string ss = GetXpath(node);
                    c.Text = NetMiner.Common.ToolUtil.GetXpathParaString(strXpath, ss);
                }
                else if (m_isDoubleSelect == true && m_MouseDownCount == 0)
                    c.Text = GetXpath(node);
                else if (m_isDoubleSelect == false)
                    c.Text = GetXpath(node);

                return c.Text;
            }
            else
            {
                string ss = GetXpath(node);
                return ss;
            }
        }

        public string GetXpath(GeckoNode node)
        {
            if (node == null)
                return "";

            if (node.NodeType == NodeType.Attribute)
            {

                return String.Format("{0}/@{1}", GetXpath(((GeckoAttribute)node).OwnerDocument), node.LocalName);
            }
            if (node.ParentNode == null)
            {

                return "";
            }


            int indexInParent = 1;
            GeckoNode siblingNode = node.PreviousSibling;

            while (siblingNode != null)
            {

                if (siblingNode.LocalName == node.LocalName)
                {
                    indexInParent++;
                }
                siblingNode = siblingNode.PreviousSibling;
            }

            return String.Format("{0}/{1}[{2}]", GetXpath(node.ParentNode), node.LocalName, indexInParent);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.txtxPath.Text == "")
                return;

            //根据获取的源码进行匹配，有可能会出现问题

            //this._ActiveWebBrowser.Document.SelectSingle
            //string htmlSource1 = this.m_HtmlSource;

            HtmlAgilityPack.HtmlDocument hDoc = new HtmlAgilityPack.HtmlDocument();
            hDoc.LoadHtml("<html>" + this._ActiveWebBrowser.Document.Body.OuterHtml + "</html>");

            try
            {
                //分解xpath路径，有可能是带有参数的
                NetMiner.Core.Url.cUrlParse u = new NetMiner.Core.Url.cUrlParse(Program.getPrjPath());
                List<string> xpaths = u.SplitWebUrl(this.txtxPath.Text);

                for (int i = 0; i < xpaths.Count; i++)
                {
                    //GeckoNode gNode= this._ActiveWebBrowser.Document.SelectSingle(xpaths[i]);

                    HtmlNodeCollection ss = hDoc.DocumentNode.SelectNodes(xpaths[i]);

                    //if (gNode == null)
                    //    return;


                    if (this.raInnerHtml.Checked == true)
                    {
                        //this.txtResult.Text = ((GeckoHtmlElement)gNode).InnerHtml + "\r\n";
                        this.txtResult.Text = ss[0].InnerHtml + "\r\n";
                    }
                    else if (this.raInnerText.Checked == true)
                    {
                        //this.txtResult.Text = ((GeckoHtmlElement)gNode).TextContent + "\r\n";
                        this.txtResult.Text = ss[0].InnerText + "\r\n";
                    }
                    else if (this.raOuterHtml.Checked == true)
                    {
                        //this.txtResult.Text = ((GeckoHtmlElement)gNode).OuterHtml + "\r\n";
                        this.txtResult.Text = ss[0].OuterHtml + "\r\n";
                    }

                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("测试采集数据出错，请验证xPath是否准确！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
        }

        private void toolCancleExit_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void toolOkExit_Click(object sender, EventArgs e)
        {
            string hNodeText="";

             if (this.raInnerHtml.Checked == true)
                hNodeText= "InnerHtml";
            else if (this.raInnerText.Checked == true)
                hNodeText= "InnerText";
            else if (this.raOuterHtml.Checked ==true )
                hNodeText= "OuterHtml";

             rxPath(this.txtxPath.Text, hNodeText);

             this.Close();

        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
         
        }

        private void isMulti_CheckedChanged(object sender, EventArgs e)
        {
            m_MouseDownCount = 0;
            if (isMulti.Checked == true)
                this.m_isDoubleSelect = true;
            else
                this.m_isDoubleSelect = false;
        }

        private void webBrowser1_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            if (((System.Windows.Forms.WebBrowser)sender).ReadyState == WebBrowserReadyState.Complete)
            {
                DoDocComplete();
            }
        }

        private string GetImage(string str)
        {
            str = str.Replace("'", "\"");
            Regex re = new Regex("(?<=src=\").+?(?=\")", RegexOptions.None);
            MatchCollection mc = re.Matches(str);

            foreach (Match ma in mc)
            {
                str = ma.Value.ToString();
                break;
            }

            return str;
        }

        private void frmVisual_Load(object sender, EventArgs e)
        {
            if (this.txtUrl.Text == "")
                this._ActiveWebBrowser.Navigate("about:blank");
            else
                this._ActiveWebBrowser.Navigate(this.txtUrl.Text);
        }

        private void raUrl_Click(object sender, EventArgs e)
        {
            if(this.raUrl.Checked==true)
            {
                this.isMulti.Enabled = true;
                this.raInnerHtml.Enabled = true;
                this.raInnerText.Enabled = true;
                this.raOuterHtml.Enabled = true;
            }
        }

        private void raClick_Click(object sender, EventArgs e)
        {
            if (raClick.Checked==true )
            {
                this.isMulti.Enabled = false;
                this.raInnerHtml.Enabled = false;
                this.raInnerText.Enabled = false;
                this.raOuterHtml.Enabled = false;
            }
        }

        private void raPage_Click(object sender, EventArgs e)
        {
            if (this.raPage.Checked==true )
            {
                this.isMulti.Enabled = false;
                this.raInnerHtml.Enabled = false;
                this.raInnerText.Enabled = false;
                this.raOuterHtml.Enabled = false;
            }
        }
    }
}

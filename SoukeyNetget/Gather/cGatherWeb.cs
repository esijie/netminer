using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using SoukeyNetget.HttpSocket;
using System.IO.Compression;
using SoukeyNetget.Task ;

///功能：采集数据处理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Gather
{

    class cGatherWeb
    {
        DataTable tempData ;
        private static readonly int DEF_PACKET_LENGTH = 2048;
        private WebProxy m_wProxy;
        private bool m_IsProxy = false;
        
        public cGatherWeb()
        {
            this.CutFlag = new List<SoukeyNetget.Task.cWebpageCutFlag>();

            //默认重试次数为3，且忽略404错误
            //m_IsIgnore404 = true;
            //m_AgainNumber = 3;

            //初始化的时候需要判断，当前连接是否采用了代理
            //如果采用了代理，则加载代理信息
            cXmlSConfig Config = new cXmlSConfig();
            m_IsProxy = Config.IsProxy;
            if (m_IsProxy == true)
            {
                if (Config.ProxyServer != "" && Config.ProxyPort != "")
                {
                    m_wProxy = new WebProxy(Config.ProxyServer, int.Parse(Config.ProxyPort));
                    m_wProxy.BypassProxyOnLocal = true;
                    if (Config.ProxyUser != "")
                    {
                        m_wProxy.Credentials = new NetworkCredential(Config.ProxyUser, Config.ProxyPwd);
                    }
                }
            }

        }

        ~cGatherWeb()
        {
            m_wProxy = null;
            this.CutFlag = null;
        }

        #region 属性
        private List<Task.cWebpageCutFlag> m_CutFlag; 
        public List<Task.cWebpageCutFlag> CutFlag
        {
            get { return m_CutFlag; }
            set { m_CutFlag = value; }
        }

        private string m_WebpageSource;
        protected string WebpageSource
        {
            get { return this.m_WebpageSource; }
            set { this.m_WebpageSource = value; }
        }

        //记录每次通讯返回后的cookie，如果此网站的cookie是动态变化，则下次请求
        //则按照更新后的cookie进行通讯
        private string m_UCookie;
        public string UCookie
        {
            get { return m_UCookie; }
            set { m_UCookie = value; }
        }

        //只要此值为true，就采用自定义头
        private bool m_IsCustomHeader;
        public bool IsCustomHeader
        {
            get { return m_IsCustomHeader; }
            set { m_IsCustomHeader = value; }
        }

        private List<cHeader> m_Headers;
        public List<cHeader> Headers
        {
            get { return m_Headers; }
            set { m_Headers = value; }
        }

        #endregion 

        /// <summary>
        /// 获取指定网址源码
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="webCode">网页编码</param>
        /// <param name="cookie">网页cookie</param>
        /// <param name="startPos">获取网页源码的起始位置</param>
        /// <param name="endPos">获取网页源码的终止位置</param>
        /// <param name="IsCutnr">是否截取回车换行符，默认为true，截取</param>
        /// <param name="IsAjax">是否为Ajax页面</param>
        /// <returns></returns>

        public string GetHtml(string url, cGlobalParas.WebCode webCode, string cookie, string startPos, string endPos,bool IsCutnr,bool IsAjax)   
        {
            //判断网页编码
            Encoding wCode;
            string PostPara = "";

            CookieContainer CookieCon = new CookieContainer();

            HttpWebRequest wReq ;

            Uri m_Uri = new Uri(url);


            if (Regex.IsMatch(url, @"<POST>[\S\s]*</POST>", RegexOptions.IgnoreCase))
            {
                wReq = (HttpWebRequest)WebRequest.Create(@url.Substring (0,url.IndexOf ("<POST>")));
            }
            else
            {
                wReq = (HttpWebRequest)WebRequest.Create(@url );
            }

            if (this.IsCustomHeader == true)
            {
                for (int i = 0; i < this.Headers.Count; i++)
                {
                    switch (this.Headers[i].Label)
                    {

                        case "Accept":
                            wReq.Accept = this.Headers[i].LabelValue;
                            break;
                        case "User-Agent":
                            wReq.UserAgent = this.Headers[i].LabelValue;
                            break;
                        case "Connection":
                            wReq.KeepAlive = true;
                            break;
                        case "Content-Type":
                            wReq.ContentType = this.Headers[i].LabelValue;
                            break;
                        case "Referer":
                            wReq.Referer = this.Headers[i].LabelValue;
                            break;
                        default:
                            wReq.Headers.Add(this.Headers[i].Label, this.Headers[i].LabelValue);
                            break;
                    }
                   
                }
            }
            else
            {

                wReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50215;)";
                wReq.Headers.Add("Accept-Language", "zh-cn,en-us;");
                wReq.KeepAlive = true;
                wReq.AllowAutoRedirect = true;
                wReq.Headers.Add("Accept-Encoding", "gzip, deflate");
                //wReq.SendChunked = true;
                //wReq.TransferEncoding = "utf-8";
            
                Match a = Regex.Match(url, @"(http://).[^/]*[?=/]", RegexOptions.IgnoreCase);
                //Match a = Regex.Match(url, @"(http://).[^/].*/", RegexOptions.IgnoreCase);

                string url1 = a.Groups[0].Value.ToString();
                wReq.Referer = url1;
            }
            
            //判断是否需要代理
            if (m_IsProxy == true)
                wReq.Proxy = m_wProxy;

            #region cookie

            //判断是否有cookie
            if (cookie != "")
            {
                CookieCollection cl = new CookieCollection();

                //foreach (string sc in cookie.Split(';'))
                //{
                //    string ss = sc.Trim();
                //    cl.Add(new Cookie(ss.Split('=')[0].Trim(), ss.Split('=')[1].Trim(), "/"));
                //}

                foreach (string sc in cookie.Split(';'))
                {
                    string ss = sc.Trim();
                    if (ss.IndexOf("&") > 0)
                    {
                        foreach (string s1 in ss.Split('&'))
                        {
                            string s2 = s1.Trim();
                            string s4 = s2.Substring(s2.IndexOf("=")+1, s2.Length - s2.IndexOf("=")-1);

                            cl.Add(new Cookie(s2.Split('=')[0].Trim(), s4, "/"));
                        }
                    }
                    else
                    {
                        string s3 = sc.Trim();
                        cl.Add(new Cookie(s3.Split('=')[0].Trim(), s3.Split('=')[1].Trim(), "/"));
                    }
                }


                CookieCon.Add(new Uri(url), cl);
                wReq.CookieContainer = CookieCon;
            }

            #endregion

            #region POST数据

            //判断是否含有POST参数
            if (Regex.IsMatch(url, @"(?<=<POST>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase))
            {

                Match s = Regex.Match(url, @"(?<=<POST>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase);
                PostPara = s.Groups[0].Value.ToString();
                byte[] pPara = Encoding.ASCII.GetBytes(PostPara);

                wReq.ContentType = "application/x-www-form-urlencoded";
                wReq.ContentLength = pPara.Length;

                wReq.Method = "POST";

                System.IO.Stream reqStream = wReq.GetRequestStream();
                reqStream.Write(pPara, 0, pPara.Length);
                reqStream.Close();
                
            }
            else
            {
                wReq.Method = "GET";

            }

            #endregion

            //设置页面超时时间为12秒
            wReq.Timeout = 12000;


            #region  开始请求代码

            HttpWebResponse wResp = (HttpWebResponse)wReq.GetResponse();

            System.IO.Stream respStream = wResp.GetResponseStream();
            string strWebData = "";

            switch (webCode)
            {
                case cGlobalParas.WebCode.auto:
                    try
                    {
                        wCode = Encoding.Default;
                        string cType = wResp.ContentType.ToLower();
                        //Match charSetMatch = Regex.Match(cType, "(?<=charset=|charset:)([^<]*)*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        Match charSetMatch = Regex.Match(cType, "(?<=charset=)([^<]*)*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string webCharSet = charSetMatch.ToString ();
                        wCode = System.Text.Encoding.GetEncoding(webCharSet);
                    }
                    catch
                    {
                        wCode = Encoding.Default;
                    }
                    
                    break;
                case cGlobalParas.WebCode.gb2312:
                    wCode = Encoding.GetEncoding("gb2312");
                    break;
                case cGlobalParas.WebCode.gbk:
                    wCode = Encoding.GetEncoding("gbk");
                    break;
                case cGlobalParas.WebCode.utf8:
                    wCode = Encoding.UTF8;
                    break;
                
                default:
                    wCode = Encoding.UTF8;
                    break;
            }

            //string charSet = wResp.CharacterSet.ToString();


            if (wResp.ContentEncoding == "gzip")
            {
                GZipStream myGZip = new GZipStream(respStream, CompressionMode.Decompress);
                System.IO.StreamReader reader;
                reader = new System.IO.StreamReader(myGZip, wCode);
                strWebData = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
            }
            else if (wResp.ContentEncoding.StartsWith("deflate"))
            {
                DeflateStream myDeflate = new DeflateStream(respStream, CompressionMode.Decompress);
                System.IO.StreamReader reader;
                reader = new System.IO.StreamReader(myDeflate, wCode);
                strWebData = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
            }
            else
            {
                System.IO.StreamReader reader;
                reader = new System.IO.StreamReader(respStream, wCode);
                strWebData = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
            }

            #endregion

            //去除回车换行符号
            if (IsCutnr == true)
            {
                strWebData = Regex.Replace(strWebData, "([\\r\\n])[\\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strWebData = Regex.Replace(strWebData, "\\n", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strWebData.Replace("\\r\\n", "");
            }



            //获取此页面的编码格式,并对源码进行一次判断,无论用户是否指定了网页代码
            //Match charSetMatch = Regex.Match(strWebData, "<meta([^<]*)charset=([^<]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //string webCharSet = charSetMatch.Groups[2].Value;
            //string charSet = webCharSet;

            ///去除以下转码过程****************************************************************************************
            //if (charSet != null && charSet != "" && Encoding.GetEncoding(charSet) != wCode)
            //{
            //    byte[] myDataBuffer;

            //    myDataBuffer = System.Text.Encoding.GetEncoding(charSet).GetBytes(strWebData);
            //    strWebData = Encoding.GetEncoding(charSet).GetString(myDataBuffer);

            //}
            ///去除以上转码过程****************************************************************************************

            //按照截取网页的起始标志和终止标志进行截取
            //如果起始或终止截取标识有一个为空，则不进行截取
            if (startPos != "" && endPos != "")
            {
                string Splitstr = "(" + startPos + ").*?(" + endPos + ")";

                Match aa = Regex.Match(strWebData, Splitstr);
                strWebData = aa.Groups[0].ToString();
            }

            if (IsAjax == true)
            {
                strWebData = System.Web.HttpUtility.UrlDecode(strWebData, Encoding.UTF8);
            }

            m_Uri = null;

            this.m_WebpageSource = strWebData;
            return strWebData;

         }

        /// <summary>
        /// 采集网页数据
        /// </summary>
        /// <param name="Url">网页地址</param>
        /// <param name="StartPos">起始采集位置</param>
        /// <param name="EndPos">终止采集位置</param>
        /// <returns></returns>
        public DataTable  GetGatherData(string Url,cGlobalParas.WebCode webCode, string cookie, string startPos,string endPos,string sPath,bool IsAjax,bool IsExportGUrl,bool IsExportGDateTime)
        {
            tempData = new DataTable("tempData");
            int i ;
            int j;
            string strCut="";
            bool IsDownloadFile = false;

            #region 构建表结构，并构建截取正则

            //根据页面截取的标志构建表结构
            for (i = 0; i < this.CutFlag.Count; i++)
            {
                tempData.Columns.Add(new DataColumn(this.CutFlag[i].Title, typeof(string)));
                
                if (this.CutFlag[i].DataType !=(int) cGlobalParas.GDataType.Txt && IsDownloadFile ==false)
                {
                    IsDownloadFile = true;
                }
            }

            if (IsExportGUrl == true)
            {
                tempData.Columns.Add(new DataColumn("CollectionUrl", typeof(string)));
            }
            if (IsExportGDateTime == true)
                tempData.Columns.Add(new DataColumn("CollectionDateTime", typeof(string)));
            
            //根据用户指定的页面截取位置构造正则表达式
            for (i = 0; i < this.CutFlag.Count; i++)
            {
                strCut += "(?<" + this.CutFlag[i].Title + ">" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + ")";

                //strCut += "(?<=" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + ")";

                switch (this.CutFlag[i].LimitSign )
                {
                    case (int)cGlobalParas.LimitSign.NoLimit : 
                        strCut += ".*?";
                        break;
                    case (int)cGlobalParas.LimitSign.NoWebSign:
                        strCut += "[^<>]*?";
                        break;
                    case (int)cGlobalParas.LimitSign.OnlyCN:
                        strCut += "[\\u4e00-\\u9fa5]*?";
                        break;
                    case (int)cGlobalParas.LimitSign.OnlyDoubleByte:
                        strCut += "[^\\x00-\\xff]*?";
                        break;
                    case (int)cGlobalParas.LimitSign.OnlyNumber:
                        strCut += "[\\d]*?";
                        break;
                    case (int)cGlobalParas.LimitSign.OnlyChar:
                        strCut += "[\\x00-\\xff]*?";
                        break;
                    case (int)cGlobalParas.LimitSign.Custom:
                        //strCut += cTool.RegexReplaceTrans(this.CutFlag[i].RegionExpression.ToString());
                        strCut += this.CutFlag[i].RegionExpression.ToString();
                        break;
                    default:
                        strCut += "[\\S\\s]*?";
                        break;
                }
                strCut += "(?=" +  cTool.RegexReplaceTrans(this.CutFlag[i].EndPos) + ")|";
            }

            #endregion

            #region 获取网页源码

            int rowCount = this.CutFlag.Count;

            //去掉最后一个“|”
            strCut = strCut.Substring(0, strCut.Length - 1);

            //获取网页信息
            //判断传入的Url是否正确，如果不正确，则返回空数据
            if (Regex.IsMatch(Url, "[\"\\s]"))
            {
                Match aa = Regex.Match(Url, "[\"\\s]");

                tempData = null;
                return tempData;
            }

            try
            {
                GetHtml(Url, webCode, cookie, startPos, endPos, true, IsAjax);
            }
            catch (System.Web.HttpException ex)
            {
                throw ex;
            }

            #endregion

            //开始获取截取内容
            Regex re = new Regex(@strCut, RegexOptions.IgnoreCase | RegexOptions.Multiline );
            MatchCollection mc = re.Matches(this.WebpageSource);

            if (mc.Count == 0)
            {
                tempData = null;
                return tempData;
            }

            DataRow drNew=null ;

            i = 0;

            #region 开始输出截取字符，并拼成一个表

            //开始根据采集的数据构造数据表进行输出
            //在此需要处理采集数据有可能错行的问题
            //下面被注释的代码是最初构建数据表的代码，但会出现错行现象

            //Match ma;
            
            int rows = 0; //统计共采集了多少行
            int m = 0;   //计数使用

            try
             {

                while (m < mc.Count)
                {
                    //新建新行
                    drNew = tempData.NewRow();
                    rows++;

                    for (i = 0; i < this.CutFlag.Count; i++)
                    {

                        if (m < mc.Count)
                        {
                            if (i == 0)
                            {
                                while (!mc[m].Value.StartsWith(this.CutFlag[i].StartPos, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    m++;
                                    if (m >= mc.Count)
                                    {
                                        //退出所有循环
                                        goto ExitWhile;
                                    }
                                }

                                drNew[i] = mc[m].Value.Substring(this.CutFlag[i].StartPos.Length, mc[m].Value.Length - this.CutFlag[i].StartPos.Length);

                                m++;
                            }
                            else
                            {
                                if (mc[m].Value.StartsWith(this.CutFlag[i].StartPos, StringComparison.CurrentCultureIgnoreCase))
                                {

                                    drNew[i] = mc[m].Value.Substring(this.CutFlag[i].StartPos.Length, mc[m].Value.Length - this.CutFlag[i].StartPos.Length);

                                    m++;
                                }
                                else
                                {
                                    if (mc[m].Value.StartsWith(this.CutFlag[i - 1].StartPos, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        m++;
                                        i--;
                                    }
                                    else
                                    {
                                        if (i < this.CutFlag.Count - 1)
                                        {
                                            if (mc[m].Value.StartsWith(this.CutFlag[i + 1].StartPos, StringComparison.CurrentCultureIgnoreCase))
                                            {

                                            }
                                            else
                                            {
                                                m++;
                                                i--;
                                            }
                                        }
                                        else
                                        {
                                            m++;
                                            i--;
                                        }
                                        //当采集时发生了缺少采集内容，采用此方法进行采集内容补空
                                        //drNew[i] = "";
                                        //continue;
                                    }
                                }
                            }
                        }
                    }

                    if (IsExportGUrl == true && IsExportGDateTime == true)
                        drNew[drNew.ItemArray.Length-2] = Url;
                    else if (IsExportGUrl == true && IsExportGDateTime == false)
                        drNew[drNew.ItemArray.Length-1] = Url;

                    if (IsExportGDateTime == true)
                        drNew[drNew.ItemArray.Length-1] = DateTime.Now.ToString();

                    tempData.Rows.Add(drNew);
                    drNew = null;

                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            #endregion

            #region 开始进行输出控制，进行获取数据加工

        ExitWhile:

            //在此判断是否需要在输出时进行数据的限制,根据任务版本1.2增加了数据输出的限制

            for (i = 0; i < this.CutFlag.Count; i++)
            {
                for (j = 0; j< this.CutFlag[i].ExportRules.Count; j++)
                {
                    switch (this.CutFlag[i].ExportRules[j].FieldRuleType)
                    {
                        case (int)cGlobalParas.ExportLimit.ExportNoLimit:

                            break;

                        //先进行删除操作
                        case (int)cGlobalParas.ExportLimit.ExportDelData:
                            for (int index = 0; index < tempData.Rows.Count; index++)
                            {

                                if (Regex.IsMatch(tempData.Rows[index][i].ToString(), this.CutFlag[i].ExportRules[j].FieldRule.ToString()))
                                {
                                    tempData.Rows.Remove(tempData.Rows[index]);
                                    index--;
                                }
                                
                            }
                            break;
                        case (int)cGlobalParas.ExportLimit.ExportInclude:
                            for (int index = 0; index < tempData.Rows.Count; index++)
                            {
                                if (!Regex.IsMatch(tempData.Rows[index][i].ToString (), this.CutFlag[i].ExportRules[j].FieldRule.ToString ()))
                                {
                                    tempData.Rows.Remove(tempData.Rows[index]);
                                    index--;
                                }
                            }
                            break;
                        case (int)cGlobalParas.ExportLimit.ExportNoWebSign:
                            for (int index = 0; index < tempData.Rows.Count; index++)
                            {
                                tempData.Rows[index][i] = getTxt(tempData.Rows[index][i].ToString());
                            }
                            break;
                        case (int)cGlobalParas.ExportLimit.ExportPrefix:
                            for (int index = 0; index < tempData.Rows.Count; index++)
                            {
                                tempData.Rows[index][i] = this.CutFlag[i].ExportRules[j].FieldRule + tempData.Rows[index][i].ToString();
                            }
                            break;
                        case (int)cGlobalParas.ExportLimit.ExportReplace:
                            for (int index = 0; index < tempData.Rows.Count; index++)
                            {
                                //替换的格式是：<OldValue:><NewValue:>，所以首先判断必须字符串长度大于22
                                if (this.CutFlag[i].ExportRules[j].FieldRule.Length >22)
                                {
                                    string sRule=this.CutFlag[i].ExportRules[j].FieldRule;
                                    string oStr = sRule.Substring(sRule.IndexOf("<OldValue:") + 10, sRule.IndexOf("><NewValue:") - 10);
                                    string nStr = sRule.Substring(sRule.IndexOf("<NewValue:") + 10, sRule.Length - sRule.IndexOf("<NewValue:")-11);
                                    tempData.Rows[index][i] = tempData.Rows[index][i].ToString().Replace(oStr, nStr);
                                }
                            }
                            break;
                        case (int)cGlobalParas.ExportLimit.ExportSuffix:
                            for (int index = 0; index < tempData.Rows.Count; index++)
                            {
                                tempData.Rows[index][i] = tempData.Rows[index][i].ToString() + this.CutFlag[i].ExportRules[j].FieldRule;
                            }
                            break;
                        case (int)cGlobalParas.ExportLimit.ExportTrimLeft:
                            for (int index = 0; index < tempData.Rows.Count; index++)
                            {
                                int len = tempData.Rows[index][i].ToString().Length;
                                int lefti = int.Parse(this.CutFlag[i].ExportRules[j].FieldRule.ToString());
                                if (tempData.Rows[index][i].ToString().Length > lefti)
                                {
                                    tempData.Rows[index][i] = tempData.Rows[index][i].ToString().Substring(lefti, len - lefti);
                                }
                            }
                            break;
                        case (int)cGlobalParas.ExportLimit.ExportTrimRight:
                            for (int index = 0; index < tempData.Rows.Count; index++)
                            {
                                int len = tempData.Rows[index][i].ToString().Length;
                                int righti = int.Parse(this.CutFlag[i].ExportRules[j].FieldRule.ToString());
                                if (tempData.Rows[index][i].ToString().Length > righti)
                                {
                                    tempData.Rows[index][i] = tempData.Rows[index][i].ToString().Substring(0, len - righti);
                                }
                            }
                            break;
                        case (int)cGlobalParas.ExportLimit.ExportTrim:
                            for (int index = 0; index < tempData.Rows.Count; index++)
                            {
                                tempData.Rows[index][i] = tempData.Rows[index][i].ToString().Trim();
                            }
                            break;
                        case (int)cGlobalParas.ExportLimit.ExportRegexReplace:
                            for (int index = 0; index < tempData.Rows.Count; index++)
                            {
                                //替换的格式是：<OldValue:><NewValue:>，所以首先判断必须字符串长度大于22
                                if (this.CutFlag[i].ExportRules[j].FieldRule.Length > 22)
                                {
                                    string sRule = this.CutFlag[i].ExportRules[j].FieldRule;
                                    string oStr = sRule.Substring(sRule.IndexOf("<OldValue:") + 10, sRule.IndexOf("><NewValue:") - 10);
                                    string nStr = sRule.Substring(sRule.IndexOf("<NewValue:") + 10, sRule.Length - sRule.IndexOf("<NewValue:") - 11);
                                    tempData.Rows[index][i] = Regex.Replace(tempData.Rows[index][i].ToString(), oStr, nStr, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                }

                                //string oStr = this.CutFlag[i].ExportRules[j].FieldRule.Substring(1, this.CutFlag[i].ExportRules[j].FieldRule.IndexOf(",") - 2);
                                //string nStr = this.CutFlag[i].ExportRules[j].FieldRule.Substring(this.CutFlag[i].ExportRules[j].FieldRule.IndexOf(",") + 2, this.CutFlag[i].ExportRules[j].FieldRule.Length - this.CutFlag[i].ExportRules[j].FieldRule.IndexOf(",") - 3);
                                //tempData.Rows[index][i] = Regex.Replace(tempData.Rows[index][i].ToString(), oStr, nStr, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            }
                            break;
                        
                        case (int)cGlobalParas.ExportLimit.ExportSetEmpty :
                            for (int index = 0; index < tempData.Rows.Count; index++)
                            {

                                if (Regex.IsMatch(tempData.Rows[index][i].ToString(), this.CutFlag[i].ExportRules[j].FieldRule.ToString()))
                                {
                                    tempData.Rows[index][i] = "";
                                }

                            }
                            break;
                        case (int)cGlobalParas.ExportLimit.ExportConvertUnicode :

                            Match mUni;
                            Regex r = new Regex("(?<code>\\\\u[a-z0-9]{4})", RegexOptions.IgnoreCase);

                            for (int index = 0; index < tempData.Rows.Count; index++)
                            {
                                string strUnicode=tempData.Rows[index][i].ToString().Trim();
                                for (mUni = r.Match(strUnicode); mUni.Success; mUni = mUni.NextMatch())
                                {
                                    string strValue = mUni.Result("${code}");   //代码
                                    int CharNum = Int32.Parse(strValue.Substring(2, 4), System.Globalization.NumberStyles.HexNumber);
                                    string ch = string.Format("{0}", (char)CharNum);
                                    strUnicode = strUnicode.Replace(strValue, ch);
                                }

                                tempData.Rows[index][i] = strUnicode;
                            }

                            break;

                        default:

                            break;
                    }
                }

            }

            #endregion

            #region 针对采集需要下载文件的字段进行文件下载处理
            //判断是否存在有下载文件的任务，如果有，则开始下载，因为此功能设计最初是下载图片使用
            //并非是专用的下载工具，所以对下载处理并没有单独进行线程处理
     
            try
            {
                if (IsDownloadFile == true)
                {
                    if (sPath == "")
                    {
                        sPath = Program.getPrjPath() + "data\\tem_file";
                    }

                    if (!Directory.Exists(sPath))
                    {
                        Directory.CreateDirectory(sPath);
                    }

                    string FileUrl = "";
                    string DownloadFileName = "";

                    for (i = 0; i < rows; i++)
                    {
                        for (j = 0; j < this.CutFlag.Count; j++)
                        {
                            if (this.CutFlag[j].DataType != (int)cGlobalParas.GDataType.Txt)
                            {
                                FileUrl = tempData.Rows[i][j].ToString();

                                //开始获取下载文件名称
                                Regex s = new Regex(@"(?<=/)[^/]*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                MatchCollection urlstr = s.Matches(FileUrl);
                                if (urlstr.Count == 0)
                                    DownloadFileName = FileUrl;
                                else
                                    DownloadFileName = urlstr[urlstr.Count - 1].ToString();
                                DownloadFileName = sPath + "\\" + DownloadFileName;

                                if (string.Compare ( FileUrl.Substring(0, 4) , "http",true )==0)
                                {
                                    DownloadFile(FileUrl, DownloadFileName);
                                }
                                else
                                {
                                    if (FileUrl.Substring(0, 1) == "/")
                                    {
                                        Url = Url.Substring(7, Url.Length - 7);
                                        Url = Url.Substring(0, Url.IndexOf("/"));
                                        Url = "http://" + Url;

                                        if (!Url.EndsWith("/"))
                                            Url += "/";

                                        FileUrl = Url + FileUrl.Substring (1,FileUrl.Length -1);
                                    }
                                    else if (FileUrl.IndexOf("/") <= 0)
                                    {
                                        Url = Url.Substring(0, Url.LastIndexOf("/") + 1);
                                        FileUrl = Url + FileUrl;
                                    }
                                    else
                                    {
                                        Url = Url.Substring(0, Url.LastIndexOf("/") + 1);
                                        FileUrl = Url + FileUrl;
                                    }

                                    DownloadFile(FileUrl, DownloadFileName);
                                }
                            }
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            #endregion

            return tempData;
        }

        //下载文件，是一个单线程的方法，适用于小文件下载，仅支持http方式
        private cGlobalParas.DownloadResult DownloadFile(string url, string path)
        {

            HttpWebRequest wReq = null;
            HttpWebResponse wRep = null;
            FileStream SaveFileStream = null;

            int startingPoint = 0;

            try
            {
                //For using untrusted SSL Certificates

                wReq = (HttpWebRequest)HttpWebRequest.Create(url);
                wReq.AddRange(startingPoint);

                wRep = (HttpWebResponse)wReq.GetResponse();
                Stream responseSteam = wRep.GetResponseStream();

                if (startingPoint == 0)
                {
                    SaveFileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                }
                else
                {
                    SaveFileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                }

                int bytesSize;
                long fileSize = wRep.ContentLength;
                byte[] downloadBuffer = new byte[DEF_PACKET_LENGTH];

                while ((bytesSize = responseSteam.Read(downloadBuffer, 0, downloadBuffer.Length)) > 0)
                {
                    SaveFileStream.Write(downloadBuffer, 0, bytesSize);
                }

                SaveFileStream.Close();
                SaveFileStream.Dispose();

                wRep.Close();

                return cGlobalParas.DownloadResult.Succeed;

                

            }
            catch (System.Exception )
            {
                return cGlobalParas.DownloadResult.Err;
            }

        }

        //获取采集内容的纯文本
        private string getTxt(string instr)
        {
            string m_outstr;

            m_outstr = instr.Clone() as string;
            System.Text.RegularExpressions.Regex objReg = new System.Text.RegularExpressions.Regex("(<[^>]+?>)|&nbsp;", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg.Replace(m_outstr, "");
            System.Text.RegularExpressions.Regex objReg2 = new System.Text.RegularExpressions.Regex("(\\s)+", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg2.Replace(m_outstr, " ");

            return m_outstr;
        }

    }
}

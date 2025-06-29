using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;
using NetMiner.Gather.Control;
using NetMiner.Core.Proxy;
using NetMiner.Common;
using NetMiner.Resource;
using System.Threading;
using NetMiner.Core.gTask.Entity;
using NetMiner.Core.Url;
using HtmlAgilityPack;

///功能：任务中URL规则解析处理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
///修订：2010-12-9 处理代理的问题 
///修订：2012-1-30 修改了网址导航的处理，取消了原有的简单导航，全部改为参数导航，实际就是在界面化
///部分进行用户体验处理，实际处理的核心并没有变化，但标签发生了变化
///2013-4-14 修改bug，导航解析的时候也需要对head进行判断，如果配置了此数据，应该传入此head并进行网页的请求，否则
///会导致采集失败。
///从V5.5开始，此类中不再获取网页源码，所有传入的方法，必须将网页数据传入。这个类不允许进行网页的请求操作
namespace NetMiner.Gather.Url
{
   /// <summary>
   /// 常规采集所用的Url解析类，负责根据配置的规则获取网址，包括导航、翻页、多页等操作。
   /// </summary>
    public class cUrlAnalyze
    {
        private bool m_IsProxy;
        private bool m_IsProxyFirst;
        private cProxyControl m_ProxyControl;
        private string m_workPath = string.Empty;

        private cGlobalParas.ProxyType m_pType = cGlobalParas.ProxyType.TaskConfig;
        private string m_pAddress = string.Empty;
        private int m_pPort = 0;

        private string m_vCode = string.Empty;
        private string m_ImgCookie = string.Empty;

        /// <summary>
        /// 调用浏览器进行脚本执行时的线程等待时间
        /// </summary>
        public static ManualResetEvent ScriptWait = new ManualResetEvent(false);

        private string m_ScriptValue = string.Empty;

        private string m_xulPath = string.Empty;

        public cUrlAnalyze(string workPath)
        {
            m_workPath = workPath;
            m_ProxyControl = null;
            this.m_IsProxy = false;
            this.m_IsProxyFirst = false;

            bool is64;
            is64 = Environment.Is64BitOperatingSystem;

            if (is64)
            {
                m_xulPath = m_workPath + "\\xulrunner\\64";
            }
            else
                m_xulPath = m_workPath + "\\xulrunner\\32";
        }

        public cUrlAnalyze(string workPath,ref cProxyControl PControl, bool IsProxy, bool IsProxyFirst,cGlobalParas.ProxyType pType,string proxyAddress,int proxyPort)
        {
            m_IsProxy = IsProxy;
            m_IsProxyFirst = IsProxyFirst;
            this.m_ProxyControl = PControl;
            m_pType = pType;
            m_pAddress = proxyAddress;
            m_pPort = proxyPort;

            m_workPath = workPath;

            bool is64;
            is64 = Environment.Is64BitOperatingSystem;

            if (is64)
            {
                m_xulPath = m_workPath + "\\xulrunner\\64";
            }
            else
                m_xulPath = m_workPath + "\\xulrunner\\32";
        }

        ~cUrlAnalyze()
        {

        }

        /// <summary>
        ///根据指定的导航规则进行页面导航，在1.6版本中，增加了多层导航的功能
        ///网址导航是属于一对多的关系，即每一级别的导航都是属于一对多（也会是一对一的关系）
        ///在此无论是几级导航，返回的都是最终的需要采集内容的网址
        ///因为是多层导航，所以是属于递归的一种算法
        ///解析网址后返回的都是标准网址，不会存在相对网址的情况
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="nRules"></param>
        /// <param name="webCode"></param>
        /// <param name="isUrlCode"></param>
        /// <param name="isTwoUrlCode"></param>
        /// <param name="urlCode"></param>
        /// <param name="cookie"></param>
        /// <param name="headers"></param>
        /// <param name="referUrl"></param>
        /// <param name="isAutoUpdateHeader"></param>
        /// <param name="isUrlAutoRedirect"></param>
        /// <returns></returns>
        public List<string> ParseUrlRule(string Url, string htmlSource, List<eNavigRule> nRules,List<eHeader> Headers)
        {
            List<string> pUrls = new List<string>();
            List<string> Urls = new List<string>();

            Dictionary<string, string> urls = new Dictionary<string, string>();
            urls.Add(Url, htmlSource);


            //第一层导航分解都是从一个单一网址进行，之所以
            //选择集合，是为了统一调用接口参数
            try
            {
                Urls = PUrlRule(urls, 1, nRules,Headers);
            }
            catch (NetMinerException ex1)
            {
                throw ex1;
            }
            catch (System.Exception ex)
            {
                //导航失败，无法解析导航规则
                throw new NetMinerException("导航发生了未知错误，错误信息为：" + ex.Message );
                return null;
            }

            return Urls;
        }

        ///解析导航网页
        ///判断是否为最后一个级别，在这里需要注意一个问题，因为有可能
        ///存储的级别并不是按照顺序进行的，所以，要根据传入的级别Level进行
        ///判断，否则会出现错误，导航网页的解析必须是按照顺序的，否则会
        ///无法解析
        private List<string> PUrlRule(Dictionary<string, string> pUrl, int Level, List<eNavigRule> nRules,List<eHeader> Headers)
        {
            List<string> tmpUrls;
            List<string> Urls = new List<string>();
            string oneUrl = string.Empty;
            string oneSource = string.Empty;

            if (pUrl.Count >0)
            {
                foreach (KeyValuePair<string, string> k in pUrl)
                {
                    oneUrl = k.Key.ToString();
                    oneSource = k.Value.ToString();
                    break;
                }
            }

            if (nRules.Count == 0)
            {
                Urls.Add(oneUrl);
                return Urls;
            }

            string UrlRule = string.Empty;
            string HtmlStartPos = string.Empty;
            string HtmlEndPos = string.Empty;
            cGlobalParas.NaviRunRule runType = cGlobalParas.NaviRunRule.Normal;
            string OtherRule = string.Empty;
            int i;

            //根据Level得到需要导航级别的导航规则
            for (i = 0; i < nRules.Count; i++)
            {
                if (Level == nRules[i].Level)
                {
                    UrlRule = nRules[i].NavigRule;
                    HtmlStartPos = nRules[i].NaviStartPos;
                    HtmlEndPos = nRules[i].NaviEndPos;
                    runType=nRules[i].RunRule;
                    OtherRule = nRules[i].OtherNaviRule;
                    break;
                }
            }


            //for (i = 0; i < pUrl.Count; i++)
            foreach(KeyValuePair<string,string> ke in pUrl)
            {
                tmpUrls = new List<string>();

                string url = ke.Key.ToString();
                string urlSource = ke.Value.ToString();

                tmpUrls = GetUrlsByRule(url, urlSource, UrlRule, Level,  runType,OtherRule,"",Headers);

                if (tmpUrls != null)
                {
                    //判断是否为最底级的导航，如果是则返回，如果不是则继续导航
                    if (Level == nRules.Count)
                    {
                        Urls.AddRange(tmpUrls);
                    }
                    else
                    {
                        //把urls转成Dictionary

                        Dictionary<string, string> dUrls = new Dictionary<string, string>();
                        for (int j=0;j<Urls.Count ;j++)
                        {
                            dUrls.Add(Urls[j], "");
                        }

                        List<string> rUrls = PUrlRule(dUrls, Level + 1, nRules,Headers);
                        Urls.AddRange(tmpUrls);
                    }
                }
            }
            return Urls;

        }

        /// <summary>
        /// 根据导航或多页规则，获取网页地址，是一个集合,V5.2增加导航执行规则，如果是多页，导航执行规则默认为正常
        /// 系统支持打码导航，譬如：信用网站每次查询都需要打码方可正常执行，所以需要传入cookie和header
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="webSource"></param>
        /// <param name="UrlRule"></param>
        /// <param name="NavLevel"></param>
        /// <param name="runType"></param>
        /// <param name="OtherRule"></param>
        /// <param name="cookie"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public List<string> GetUrlsByRule(string Url, string webSource, string UrlRule,
            int NavLevel, cGlobalParas.NaviRunRule runType, string OtherRule,string cookie,List<eHeader> headers)
        {

            //先对UrlRule进行转移，否则会匹配错误
            UrlRule = ToolUtil.CutRegexWildcard(UrlRule);

            string Url1;
            List<string> Urls = new List<string>();

            if (UrlRule.Trim() == "" || UrlRule == "<MySelf/>")
            {
                Urls.Add(Url);
                return Urls;
            }

            bool isMySelfLoop = false;
            string strLoop = string.Empty;

            if (UrlRule.StartsWith("<MySelf>"))
            {
                Match strMatchLoop = Regex.Match(UrlRule, "(?<=<MySelf>).*?(?=</MySelf>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strLoop = strMatchLoop.Groups[0].ToString();
                if (!string.IsNullOrEmpty(strLoop))
                    isMySelfLoop = true;

                if (isMySelfLoop == false) 
                {
                    Urls.Add(Url);
                    return Urls;
                }
            }

            //判断网址是否存在参数，如果存在参数则取出第一个可用网址
            //因为加入了转移，所以需要解析转义字符，否则请求会失败，转义字符 \
            if (Regex.IsMatch(Url, "{.*}"))
            //if (!Regex.IsMatch(Url, "[^\\\\]{.*[^\\\\]}"))
            {
                cUrlParse up = new cUrlParse(this.m_workPath);
                List<string> Urls1 = up.SplitWebUrl(Url);  //,IsUrlEncode ,UrlEncode
                up = null;
                Url1 = Urls1[0].ToString();
            }
            else
            {
                Url1 = Url;
            }

            #region 可视化处理方法
            //在此判断是否为可视化导航，如果是，则调用可视化方法
            if (UrlRule.StartsWith("<XPath>"))
            {
                Match charSetMatch1 = Regex.Match(UrlRule, "(?<=<XPath>).*?(?=</XPath>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string xPathRule = charSetMatch1.Groups[0].ToString();


                //Urls = GetUrlsByRule(Url, UrlSource, xPathRule);

                Urls = GetUrlsByRuleByXPath(Url,webSource, xPathRule,0);

                return Urls;
            }
            #endregion

            #region 获取网页源码
            //返回网址的源码，并根据提取规则提取导航的网址
            //string UrlSource= cTool.GetHtmlSource(Url1,true );
            string UrlSource = string.Empty;
            if (webSource == "")
            {
                throw new NetMinerException("网页为空，请重新调用！");
                //cGatherWeb gW = new cGatherWeb(this.m_workPath, ref this.m_ProxyControl, this.m_IsProxy, this.m_IsProxyFirst, this.m_pType, this.m_pAddress, this.m_pPort);


                //try
                //{
                //    if (headers == null)
                //    {
                //        gW.IsCustomHeader = false;
                //    }
                //    else
                //    {
                //        gW.IsCustomHeader = true;

                //        //开始计算header
                //        for (int i = 0; i < headers.Count; i++)
                //        {
                //            if (headers[i].Range == "[All]")
                //                gW.Headers.Add(headers[i]);
                //            else if (headers[i].Range.Contains(NavLevel.ToString()))
                //                gW.Headers.Add(headers[i]);
                //            //gW.Headers = headers;
                //        }
                //    }

                //    gW.IsUrlAutoRedirect = isUrlAutoRedirect;
                //    UrlSource = gW.GetHtml(Url1, webCode, isUrlCode, isTwoUrlCode, UrlCode, ref cookie,
                //        strStartPos, strEndPos, true, isAutoUpdateHeader, referUrl, isGatherCoding, CodingFlag, CodingUrl, Plugin);
                //}
                //catch (System.Exception ex)
                //{
                //    throw new NetMinerException("获取网页源码出错，请检查网页地址、网页编码、网址编码格式，错误信息为：" + ex.Message);
                //}
                //gW = null;
            }
            else
                UrlSource = webSource;

            #endregion

            if (UrlSource == "")
            {
                return null;
            }

            #region 处理自我导航循环的问题
            if (isMySelfLoop == true)
            {
                Regex re = new Regex(strLoop, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                MatchCollection mc = re.Matches(UrlSource);
                int count = mc.Count;

                for (int index = 0; index < count; index++)
                {
                    Urls.Add(Url);
                }

                return Urls;
            }
            #endregion

            //string Rule=@"(?<=href=[\W])" + cTool.RegexReplaceTrans(UrlRule) + @"(\S[^'"">]*)(?=[\s'""])";
            string sRule = "";

            string strPrefix = "";
            string strSuffix = "";
            string strIncludeWord = string.Empty;
            string strNoIncludeWord = string.Empty;
            string strOldValue = string.Empty;
            string strNewValue = string.Empty;
            bool isRegex = false;

            #region 获取正则表达式，匹配导航网址

            //判断导航规则中是否存在导航结果附加前缀字符串和尾部增加字符串的操作，如果系统没有
            //配置则为空

            #region  加工规则
            Match charSetMatch = Regex.Match(UrlRule, "(?<=<Prefix>).*?(?=</Prefix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            strPrefix = charSetMatch.Groups[0].ToString();
            UrlRule = Regex.Replace(UrlRule, "(<Prefix>).*?(</Prefix>)", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            charSetMatch = Regex.Match(UrlRule, "(?<=<Suffix>).*?(?=</Suffix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            strSuffix = charSetMatch.Groups[0].ToString();
            UrlRule = Regex.Replace(UrlRule, "(<Suffix>).*?(</Suffix>)", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            charSetMatch = Regex.Match(UrlRule, "(?<=<Include>).*?(?=</Include>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            strIncludeWord = charSetMatch.Groups[0].ToString();
            UrlRule = Regex.Replace(UrlRule, "<Include>.*?</Include>", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            charSetMatch = Regex.Match(UrlRule, "(?<=<NoInclude>).*?(?=</NoInclude>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            strNoIncludeWord = charSetMatch.Groups[0].ToString();
            UrlRule = Regex.Replace(UrlRule, "<NoInclude>.*?</NoInclude>", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            if (Regex.IsMatch(UrlRule, "<RegexReplace>"))
            {
                isRegex = true;

                charSetMatch = Regex.Match(UrlRule, "(?<=<RegexReplace>).*?(?=</RegexReplace>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string ss = charSetMatch.Groups[0].ToString();

                strOldValue = ss.Substring(ss.IndexOf("<OldValue:") + 10, ss.IndexOf("><NewValue:") - 10);
                strNewValue = ss.Substring(ss.IndexOf("<NewValue:") + 10, ss.Length - ss.IndexOf("<NewValue:") - 11);

                UrlRule = Regex.Replace(UrlRule, "<RegexReplace>.*?</RegexReplace>", "");
            }
            else if (Regex.IsMatch(UrlRule, "<Replace>"))
            {
                isRegex = false;

                charSetMatch = Regex.Match(UrlRule, "(?<=<Replace>).*?(?=</Replace>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string ss = charSetMatch.Groups[0].ToString();

                strOldValue = ss.Substring(ss.IndexOf("<OldValue:") + 10, ss.IndexOf("><NewValue:") - 10);
                strNewValue = ss.Substring(ss.IndexOf("<NewValue:") + 10, ss.Length - ss.IndexOf("<NewValue:") - 11);
                UrlRule = Regex.Replace(UrlRule, "<Replace>.*?</Replace>", "");
            }
            #endregion

            if (UrlRule.StartsWith("<Common>") || UrlRule.StartsWith("<Regex>"))
            {
                #region 复杂导航，即是正则来处理导航规则
                //复杂的导航规则
                if (Regex.IsMatch(UrlRule, "<Common>"))
                {
                    //Rule = @"(?<=[href=|src=|open(][\W])";

                    charSetMatch = Regex.Match(UrlRule, "(?<=<Common>).*?(?=</Common>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string strtrule = charSetMatch.Groups[0].ToString();
                    sRule = strtrule;

                }
                else if (Regex.IsMatch(UrlRule, "<Regex>"))
                {
                    charSetMatch = Regex.Match(UrlRule, "(?<=<Regex>).*?(?=</Regex>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string strPre = charSetMatch.Groups[0].ToString();
                    sRule = strPre;
                }
                #endregion
            }
            else if (UrlRule.StartsWith("<Trait>"))
            {
                #region 简单导航
                //简单规则导航，增加了标签

                charSetMatch = Regex.Match(UrlRule, "(?<=<Trait>).*?(?=</Trait>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string trait = charSetMatch.Groups[0].ToString();

                sRule = @"(?<=[url"":""|url':'|href=|src=|open(][\W])" + ToolUtil.RegexReplaceTrans(trait) + @"(\S[^'"">]*)(?=[\s'""])";
                #endregion
            }
            else if (UrlRule.StartsWith("<ParaNavi>"))
            {
                //以下代码被注释，由于原来的方法在进行参数值获取的时候
                //需要按照顺序来进行替换，修改此方法，按照下一页的方法进行
                //这样就可以不用按照顺序来进行替换了，增强灵活性
                //参数导航
                //先将所有的参数提取出来，拼接一个正则表达式，然后进行参数提取
                string paraReg = "(?<=<Para>).+?(?=</Para>)";

                Regex pReg = new Regex(paraReg, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //MatchCollection paras = pReg.Matches(UrlSource);
                MatchCollection paras = pReg.Matches(UrlRule);

                int groutID = 1;
                foreach (Match para in paras)
                {
                    //系统默认组名需要价格字母，进行字母和数字的区分
                    if (sRule == "")
                    {
                        sRule += "(?<G" + groutID + ">";
                        sRule += para.ToString();
                        sRule += ")";
                    }
                    else
                    {
                        sRule += @"(([\s\S]+?)" + "(?<G" + groutID + ">";
                        sRule += para.ToString();
                        sRule += "))";
                    }

                    groutID++;
                }

                //这个又该回去了，还得按照原有方式进行，否则会出问题，
                //1、无法区分前后规则，多个参数的时候
                //2、无法进行多条网址导航
                //2014-3-10修改，更新了原有参数正则匹配的方法，可以不按照网页顺序进行参数的匹配
                //while (Regex.IsMatch(UrlRule, "<Para>"))
                //{
                //    //处理参数，替换成匹配后的结果
                //    charSetMatch = Regex.Match(UrlRule, "(?<=<Para>).*?(?=</Para>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //    string strRegex = charSetMatch.Groups[0].ToString();

                //    charSetMatch = Regex.Match(UrlSource, strRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //    string pValue = charSetMatch.Groups[0].ToString();

                //    //替换
                //    UrlRule = UrlRule.Replace("<Para>" + strRegex + "</Para>", pValue);
                //}


                #region 处理表单值获取的问题
                //在此处理时间戳和页面表单的数据
                bool isFormValue = false;
                Hashtable rFormValue = new Hashtable();

                int maxLoop = 0;
                while (Regex.IsMatch(UrlRule, "{FormValue}") && maxLoop<5000)
                {

                    maxLoop++;

                    #region 处理来路页面的信息
                    //建立一个来路页面的表单数据值
                    if (isFormValue == false)
                    {

                        //先获取form
                        Regex reForm = new Regex("<input[^>]+?>", RegexOptions.IgnoreCase);
                        MatchCollection mcForm = reForm.Matches(UrlSource);
                        foreach (Match ma in mcForm)
                        {
                            //判断Type是否为hidden，如果是，则添加到表单表中
                            Match a = Regex.Match(ma.ToString(), @"type=.+?\s", RegexOptions.IgnoreCase);
                            string strT = a.Groups[0].Value.ToString();
                            if (strT.IndexOf("hidden", StringComparison.CurrentCultureIgnoreCase) > -1)
                            {
                                try
                                {
                                    string n = "";
                                    string v = "";
                                    string ss1 = ma.ToString();
                                    if (ss1.EndsWith("/>"))
                                    {
                                        ss1 = ss1.Substring(0, ss1.Length - 2) + " />";
                                    }
                                    else
                                    {
                                        if (ss1.EndsWith(">"))
                                            ss1 = ss1.Substring(0, ss1.Length - 1) + " >";
                                    }


                                    Match a1 = Regex.Match(ss1, @"(?<=name=).+?\s", RegexOptions.IgnoreCase);
                                    n = a1.Groups[0].Value.ToString();

                                    a1 = Regex.Match(ss1, @"(?<=value=).+?\s", RegexOptions.IgnoreCase);
                                    v = a1.Groups[0].Value.ToString();

                                    n = n.Replace("'", "");
                                    n = n.Replace("\"", "").Trim();

                                    v = v.Replace("'", "");
                                    v = v.Replace("\"", "").Trim();


                                    rFormValue.Add(n, v);
                                }
                                catch { }
                            }

                        }
                        isFormValue = true;
                    }
                    
                    #endregion

                    charSetMatch = Regex.Match(UrlRule, "(?<=[&|\\?|>|/])[^=|^>|^&]+?(?=={FormValue})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string label = charSetMatch.Groups[0].ToString();

                    foreach (DictionaryEntry de1 in rFormValue)
                    {
                        if (label.ToLower() == de1.Key.ToString().ToLower())
                        {
                            string ss=de1.Value.ToString();

                            //if (isUrlCode == true)
                            //{
                            //    ss = ToolUtil.UrlParaEncode(ss, UrlCode);
                            //}

                            UrlRule = UrlRule.Replace(label + "={FormValue}", label + "=" + ss);
                            break;
                        }
                    }
                }
                #endregion 

                if (Regex.IsMatch(UrlRule, "{Timestamp:[\\d]*?}"))
                {
                    UrlRule = Regex.Replace(UrlRule, "{Timestamp:[\\d]*?}", ToolUtil.GetTimestamp().ToString());
                }

                //从传入的网址中获取值
                while (Regex.IsMatch(UrlRule, "{UrlValue:.*?}"))
                {
                    charSetMatch = Regex.Match(UrlRule, "(?<={UrlValue:).*?(?=})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string urlPara = charSetMatch.Groups[0].ToString();

                    charSetMatch = Regex.Match(Url, urlPara, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string urlValue = charSetMatch.Groups[0].ToString();

                    //对url需要进行转义
                    urlValue = ToolUtil.ConvertEncodingUrl(urlValue);

                    //UrlRule = Regex.Replace(UrlRule, "{UrlValue:}", urlValue);
                    UrlRule = UrlRule.Replace("{UrlValue:" + urlPara + "}", urlValue);
                }

                //在此处理脚本参数问题，脚本参数通过浏览器执行
                if (Regex.IsMatch(UrlRule, "{Script:.*}"))
                {
                    charSetMatch = Regex.Match(UrlRule, "(?<={Script:).*(?=})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string urlPara = charSetMatch.Groups[0].ToString();

                    urlPara = urlPara.Replace("\\{", "{").Replace("\\}", "}");

                    ScriptWait = new ManualResetEvent(false);
                    string[] ps = new string[] { urlPara, UrlSource };
                    Thread getScriptValue = new Thread(new ParameterizedThreadStart(GetScriptValue));
                    getScriptValue.SetApartmentState(ApartmentState.STA);
                    getScriptValue.IsBackground = true;
                    getScriptValue.Start(ps);
                    
                    //进入等待线程阶段
                    ScriptWait.WaitOne();

                    UrlRule = Regex.Replace(UrlRule, "{Script:.*}", m_ScriptValue);
                }

                //处理来路页面的数值
                while (Regex.IsMatch(UrlRule, "{FromValue:.*?}"))
                {
                    charSetMatch = Regex.Match(UrlRule, "(?<={FromValue:).*?(?=})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string urlPara = charSetMatch.Groups[0].ToString();

                    charSetMatch = Regex.Match(UrlSource, urlPara, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string urlValue = charSetMatch.Groups[0].ToString();

                    //UrlRule = Regex.Replace(UrlRule, "{FromValue:}", urlValue);
                    UrlRule = UrlRule.Replace("{FromValue:" + urlPara + "}", urlValue);
                }

                if (Regex.IsMatch(UrlRule, ("{CodeValue:")))
                {
                    string vCode = string.Empty;
                    Match codeMatch = Regex.Match(UrlRule, "(?<={CodeValue:).*?(?=})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string str = codeMatch.Groups[0].ToString();

                    string[] ss = str.Split(',');
                    string imgUrl = ss[0].Replace("Url:", "").Replace ("[","");
                    string strPlugin = ss[2].Replace("Plugin:", "").Replace("]","");
                    if (ss[1].Split(':')[1].ToLower() == "true")
                    {
                        //打码导航
                        string tmpCookie = string.Empty;
                        string imgName = NetMiner.Common.HttpSocket.cHttpSocket.GetImage(System.IO.Path.GetTempPath().ToString(), imgUrl, ref cookie);

                        //开始获取验证码
                        NetMiner.Common.HttpSocket.cRunPlugin rPlugin = new NetMiner.Common.HttpSocket.cRunPlugin();

                        if (strPlugin != "")
                        {
                            vCode = rPlugin.CallVerifyCode(imgName, imgUrl, strPlugin);
                        }
                        rPlugin = null;


                    }
                    else
                    {
                        //手动打码
                        NetMiner.Common.HttpSocket.frmInputVCode f = new NetMiner.Common.HttpSocket.frmInputVCode();
                        f.RVCode = GetVCode;
                        f.iniData(imgUrl, cookie);
                        if (f.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                        {
                            //取消登录
                            cookie = "";
                        }
                        f.Dispose();

                        //如果图片返回了Cookie，则使用图片的cookie
                        if (m_ImgCookie != "")
                        {
                            cookie = m_ImgCookie;
                        }
                        vCode = m_vCode;
                    }


                    UrlRule = Regex.Replace(UrlRule, "{CodeValue:.*?}", vCode);
                }

            }

            #endregion

            DataTable d1 = new DataTable();
            d1.Columns.Add("Name");

            AgainL:

            #region 根据正则匹配网址
             
             if (sRule != "")
             {
                 Regex re = new Regex(sRule, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                 MatchCollection aa = re.Matches(UrlSource);

                 foreach (Match ma in aa)
                 {
                     //Urls.Add(ma.Value.ToString());
                     //在此处理，如果导航出来的网址中包含空格，则需要转换成%20，这个问题在google
                     //网址查询中会经常出现

                     string tUrl = "";

                     if (UrlRule.StartsWith("<ParaNavi>"))
                     {
                         //导航网址多参数处理，主要是根据url进行替换
                         tUrl = UrlRule;
                         tUrl = tUrl.Replace("<ParaNavi>", "");
                         tUrl = tUrl.Replace("</ParaNavi>", "");

                         for (int n = 0; n < ma.Groups.Count; n++)
                         {
                             if (tUrl.IndexOf("<Para>") > 0)
                             {
                                 string gname = "G" + (n + 1);
                                 int startI = tUrl.IndexOf("<Para>");
                                 int endI = tUrl.IndexOf("</Para>") + 7;
                                 tUrl = tUrl.Substring(0, startI) +
                                     System.Web.HttpUtility.HtmlDecode(ma.Groups[gname].ToString())
                                     + tUrl.Substring(endI, tUrl.Length - endI);
                             }
                         }

                        cUrlParse up = new cUrlParse(this.m_workPath);
                         List<string> tNPUrls = up.SplitUrl(tUrl);
                        up = null;
                         for (int ti = 0; ti < tNPUrls.Count; ti++)
                         {
                             d1.Rows.Add(tNPUrls[ti].Trim());
                         }

                     }
                     else
                     {
                         tUrl = System.Web.HttpUtility.HtmlDecode(ma.Value.ToString().Trim());
                         tUrl = Regex.Replace(tUrl, " ", "%20");
                         d1.Rows.Add( tUrl );
                     }
                 }
             }
             else
             {
                 if (UrlRule.StartsWith("<ParaNavi>"))
                 {
                     //导航网址多参数处理，主要是根据url进行替换
                     string tUrl = UrlRule;
                     tUrl = tUrl.Replace("<ParaNavi>", "");
                     tUrl = tUrl.Replace("</ParaNavi>", "");
                    cUrlParse u = new cUrlParse(this.m_workPath);
                     List<string> tNPUrls = u.SplitUrl(tUrl);
                    u = null;
                     for (int ti = 0; ti < tNPUrls.Count; ti++)
                     {
                         d1.Rows.Add( tNPUrls[ti].Trim());
                     }
                 }
                 else
                 {
                    
                     d1.Rows.Add( Url );
                 }
             }


            #endregion

            #region 在此处理Header需要从来路地址获取数据的头信息
            for (int i = 0; i < headers.Count; i++)
            {
                if (headers[i].LabelValue.StartsWith("{ReferData:"))
                {
                    string ss = headers[i].LabelValue.Substring(11, headers[i].LabelValue.Length - 12);

                    charSetMatch = Regex.Match(UrlSource, ss, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string s1 = charSetMatch.Groups[0].ToString();
                    if (!string.IsNullOrEmpty(s1))
                    {
                        headers[i].LabelValue = s1;
                    }
                }
            }

            #endregion

            #region 在此处理导航执行判断
            if (d1.Rows.Count == 0)
             {
                 //表示没有匹配到导航网址数据
                 switch (runType)
                 {
                     case cGlobalParas.NaviRunRule.Normal:
                         break;
                     case cGlobalParas.NaviRunRule.Skip:
                         d1.Rows.Add(Url);
                         break;
                     case cGlobalParas.NaviRunRule.Other:
                         sRule = OtherRule;
                         runType = cGlobalParas.NaviRunRule.Normal;
                         goto AgainL;
                         break;
                 }
             }
             #endregion


             //导航时可能会获取重复网址的列表，所以需要去重
            //去除重复行


            string[] strComuns = new string[d1.Columns.Count];

            for (int m = 0; m < d1.Columns.Count; m++)
            {
                strComuns[m] = d1.Columns[m].ColumnName;
            }

            DataView dv = new DataView(d1);

            DataTable d2 = dv.ToTable(true, strComuns);

            #region 处理匹配结果，构建一个合格的网址
            for (int i = 0; i < d2.Rows.Count; i++)
            {

                string strUrl = d2.Rows[i][0].ToString();
                strUrl = EditUrl(strUrl, strPrefix, strSuffix, strIncludeWord, strNoIncludeWord, strOldValue, strNewValue, isRegex);
                if (strUrl!="")
                {
                    //只删除头和尾的引号
                    strUrl = Regex.Replace(strUrl, "^[\"|']", "");
                    strUrl = Regex.Replace(strUrl, "[\"|']$", "");

                    //需要处理转义，导航的网址中如果出现{ }，需要转义
                    strUrl = strUrl.Replace("{", "\\{");
                    strUrl = strUrl.Replace("}", "\\}");
                    strUrl = ToolUtil.ConvertJsonUrl(strUrl);
                    strUrl = ToolUtil.ConvertUnicodeUrl(strUrl);

                    strUrl = ToolUtil.RelativeToAbsoluteUrl(Url, strUrl);
                    Urls.Add(strUrl);

                    //if (strUrl != "#")
                    //{
                    //    if (strUrl.StartsWith("//"))
                    //    {
                    //        if (Url.StartsWith("https", StringComparison.CurrentCultureIgnoreCase))
                    //            strUrl = "https:" + strUrl;
                    //        else
                    //            strUrl = "http:" + strUrl;

                    //        string newUrl = strUrl;

                    //        newUrl = newUrl.Replace("{", "\\{");
                    //        newUrl = newUrl.Replace("}", "\\}");
                    //        newUrl = ToolUtil.ConvertJsonUrl(newUrl);
                    //        newUrl = ToolUtil.ConvertUnicodeUrl(newUrl);
                    //        Urls.Add(newUrl);
                    //    }
                    //    else if (!strUrl.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                    //    {
                    //        string PreUrl = Url;

                    //        //判断Url是否存在POST参数，如果存在，则先去掉POST参数
                    //        //然后再进行网址计算，否则会在网址匹配时出错
                    //        if (Regex.IsMatch(PreUrl, @"(?<=<POST[^>]*>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase))
                    //        {
                    //            PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("<POST"));
                    //        }

                    //        PreUrl = NetMiner.Common.ToolUtil.RelativeToAbsoluteUrl(Url, PreUrl);

                    //        //增加导航规则探测结果的网址 需要附加的 前缀和后缀，如果为空，也增加
                    //        string newUrl = PreUrl + strUrl;
                    //        //newUrl = newUrl.Replace("'", "");
                    //        //newUrl = newUrl.Replace("\"", "");
                    //        //只删除头和尾的引号
                    //        newUrl = Regex.Replace(newUrl, "^[\"|']", "");
                    //        newUrl = Regex.Replace(newUrl, "[\"|']$", "");

                    //        //需要处理转义，导航的网址中如果出现{ }，需要转义
                    //        newUrl = newUrl.Replace("{", "\\{");
                    //        newUrl = newUrl.Replace("}", "\\}");
                    //        newUrl = ToolUtil.ConvertJsonUrl(newUrl);
                    //        newUrl = ToolUtil.ConvertUnicodeUrl(newUrl);
                    //        Urls.Add(newUrl);
                    //    }
                    //    else
                    //    {
                    //        //增加导航规则探测结果的网址 需要附加的 前缀和后缀，如果为空，也增加

                    //        string newUrl = strUrl;
                    //        //newUrl = newUrl.Replace("'", "");
                    //        //newUrl = newUrl.Replace("\"", "");
                    //        //只删除头和尾的引号
                    //        newUrl = Regex.Replace(newUrl, "^[\"|']", "");
                    //        newUrl = Regex.Replace(newUrl, "[\"|']$", "");

                          
                    //    }
                    //}
                }

            }
            #endregion

            return Urls;
        }

        private void GetVCode(string code, string cookie)
        {
            m_vCode = code;
            m_ImgCookie = cookie;
        }

        private void GetScriptValue(object paras)
        {
            string[] ps = (string[])paras;
            frmExcuteScript f = new frmExcuteScript(ps[0]);
            object c = f.GotoUrl(ps[1]);
            f.Dispose();

            if (c != null)
            {
                m_ScriptValue = c.ToString();
            }
            ScriptWait.Set();
        }

        /// <summary>
        /// 根据导航规则获取网页地址，此方法专门用于处理可视化导航
        /// </summary>
        /// <param name="NavUrl"></param>
        /// <param name="htmlSource"></param>
        /// <param name="UrlRule"></param>
        /// <returns></returns>
        public List<string> GetUrlsByRule(string NavUrl, string htmlSource, string UrlRule)
        {
            List<string> urls = GetXPathData(htmlSource, UrlRule);

            //通过正则将url地址匹配出来
            for (int i = 0; i < urls.Count; i++)
            {
                string url = urls[i];
                Match s = Regex.Match(url, "(?<=href=)[^>]+?(?=[>|'|\"|\\s])", RegexOptions.IgnoreCase);
                url = s.Groups[0].Value.ToString();
                //url = url.Replace("'", "");
                //url = url.Replace("\"", "");
                //只删除头和尾的引号
                url = Regex.Replace(url, "^[\"|']", "");
                url = Regex.Replace(url, "[\"|']$", "");

                if (!url.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                {
                    string PreUrl = NavUrl;

                    url = NetMiner.Common.ToolUtil.RelativeToAbsoluteUrl(NavUrl, url);

                    //需要处理转义，导航的网址中如果出现{ }，需要转义
                    url = url.Replace("{", "\\{");
                    url = url.Replace("}", "\\}");
                    url = ToolUtil.ConvertUnicodeUrl(url);
                    url = ToolUtil.ConvertJsonUrl(url);

                }

                urls[i] = url;
            }

            return urls;
        }

        /// <summary>
        /// 根据导航规则获取网页地址，此方法专用于可视化操作，从V5.5开始，可视化进入浏览器模式
        /// </summary>
        /// <param name="NavUrl"></param>
        /// <param name="UrlRule"></param>
        /// <returns></returns>
        public List<string> GetUrlsByRuleByXPath(string NavUrl , string webSource, string XPath, int pageScrollIndex)
        {

            string[] urls = null;
            string ss = string.Empty;
            List<string> tmpUrls = new List<string>();
            string tmpXpath = string.Empty;
            string str = string.Empty;

            cUrlParse up = new cUrlParse(this.m_workPath);

            if (Regex.IsMatch(XPath, "{Num:\\d+,\\d+,\\d+}"))
            {

                //获取第一个数字参数
                if (pageScrollIndex > 1)
                {
                    Match charSetMatch = Regex.Match(XPath, "{Num:\\d+,\\d+,\\d+}", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string numPara = charSetMatch.Groups[0].ToString();

                    //根据页面滚动的次数来重新进行数字参数的值修改
                    string s = numPara.Replace("{Num:", "").Replace("}", "");
                    string[] sss = s.Split(',');
                    if (sss.Length == 3)
                    {
                        //分解争取，开始处理
                        sss[0] = (int.Parse(sss[0]) + int.Parse(sss[1]) * (pageScrollIndex - 1)).ToString();
                        sss[1] = (int.Parse(sss[1]) * (pageScrollIndex)).ToString();
                        XPath = XPath.Replace(numPara, "{Num:" + sss[0] + "," + sss[1] + "," + sss[2] + "}");
                    }
                }

                cUrlParse cu = new cUrlParse(this.m_workPath);
                List<string> rules = cu.SplitWebUrl(XPath);
                cu = null;

                for (int i = 0; i < rules.Count; i++)
                {
                    tmpXpath = rules[i];

                    str = Regex.Replace(tmpXpath, "\\[\\d*\\]", "", RegexOptions.IgnoreCase);
                    if (str.EndsWith("img", StringComparison.CurrentCultureIgnoreCase))
                    {
                        tmpXpath = tmpXpath.Substring(0, tmpXpath.LastIndexOf("/"));
                    }

                    ss = FindByxPath(webSource, tmpXpath);
                    if (!string.IsNullOrEmpty(ss))
                    {
                        ss = up.GetUrlsByRule(NavUrl, ss);
                        tmpUrls.Add(ss);
                    }
                }
                urls = tmpUrls.ToArray();

            }
            else
            {
                str = Regex.Replace(XPath, "\\[\\d*\\]", "", RegexOptions.IgnoreCase);
                if (str.EndsWith("img", StringComparison.CurrentCultureIgnoreCase))
                {
                    XPath = XPath.Substring(0, XPath.LastIndexOf("/"));
                }

                ss = FindByxPath(webSource, XPath);
                urls = new string[1];
                urls[0] = up.GetUrlsByRule(NavUrl, ss);

            }


            tmpUrls = new List<string>(urls);
            return tmpUrls;

        }

        private string FindByxPath(string htmlSource, string xPath)
        {

            HtmlDocument hDoc = new HtmlDocument();
            hDoc.LoadHtml("<Html>" + htmlSource + "</Html>");

            HtmlNodeCollection ss = hDoc.DocumentNode.SelectNodes(xPath);
            string s = string.Empty;

            if (ss!=null && ss.Count >0)
            {
                s = ss[0].OuterHtml;
            }

            return s;

        }

        /// <summary>
        /// 根据xPath获取导航网址
        /// </summary>
        /// <param name="HtmlSource"></param>
        /// <param name="xPaths"></param>
        /// <returns></returns>
        private List<string> GetXPathData(string HtmlSource, string xPathExpression)
        {
            //构建一个doc
            //System.Windows.Forms.HtmlDocument htmlDoc = new System.Windows.Forms.HtmlDocument();
            //HtmlElement btnElement = htmlDoc.Body;

            //htmlMessage = btnElement.OuterHtml;

            List<string> urls = new List<string>();

            //取源代码中body中间的部分
            HtmlSource = new Regex(@"(?m)<script[^>]*>(\w|\W)*?</script[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(HtmlSource, "");

            //尝试进行格式化操作，主要是为了完善标签
            HtmlSource = HtmlExtract.Utility.HtmlHelper.HtmlFormat (HtmlSource);
            
            Match s = Regex.Match(HtmlSource, @"<body[\S\s]*</body>", RegexOptions.IgnoreCase);

            HtmlSource = s.Groups[0].Value.ToString();

            HtmlAgilityPack.HtmlDocument hDoc = new HtmlAgilityPack.HtmlDocument();
            hDoc.LoadHtml("<Html>" + HtmlSource + "</Html>");

            //分解xPath，因为xPath可能是一个参数的列表
            cUrlParse u = new cUrlParse(this.m_workPath);
            List<string> xExpressions = u.SplitWebUrl(xPathExpression);
            u = null;


            for (int l = 0; l < xExpressions.Count; l++)
            {
                //开始检索数据
                HtmlAgilityPack.HtmlNodeCollection ss = hDoc.DocumentNode.SelectNodes(xExpressions[l].ToString());

                if (ss != null)
                {
                    foreach (HtmlAgilityPack.HtmlNode hNode in ss)
                    {
                        string strxPahtvalue = "";

                        //由于是导航网址，所以必须获取outerhtml，包含链接地址
                        strxPahtvalue = hNode.OuterHtml;
                        urls.Add(strxPahtvalue);
                        break;
                    }
                }
            }

            return urls;
        }

        //拆分网址
        /// <summary>
        /// 根据网址提供的参数对网址进行分解处理,增加了多条网址的处理，用;分割
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        //public List<string> SplitWebUrl(string Url)         //, bool IsUrlEncode
        //{
        //    List<string> Urls = new List<string>();

        //    try
        //    {
        //        //string[] sUrls = Regex.Split(Url, "\r\n");
        //        string[] sUrls = null;

        //        if (Url.IndexOf("<POST") > 0 && Url.IndexOf("</POST>") > 0)
        //        {
        //            sUrls = new string[1];
        //            sUrls[0] = Url;
        //        }
        //        else
        //            sUrls = Regex.Split(Url, "\r\n");

        //        for (int i = 0; i < sUrls.Length; i++)
        //        {
        //            List<string> tUrls = SplitUrl(sUrls[i]);
        //            Urls.AddRange(tUrls);
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw new NetMinerException(ex.Message);
        //    }

        //    return Urls;                                  //, IsUrlEncode, ""
        //}

     

        //private List<string> SplitUrl(string Url)
        //{
        //    List<string> list_Url = new List<string>();

        //    if (Url.Contains("{DbUrl:"))
        //    {
        //        #region 数据库提取地址数据

        //        //提取dburl参数
        //        Match charSetMatch = Regex.Match(Url, "(?<={DbUrl:)[\\s\\S]*?(?=})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //        string dbUrl = charSetMatch.Groups[0].ToString();

        //        string dbPreUrl = Url.Substring(0, Url.IndexOf("{DbUrl:"));
        //        string dbSuffUrl = string.Empty;

        //        if (string.IsNullOrEmpty(dbPreUrl))
        //            dbSuffUrl = Url.Replace("{DbUrl:" + dbUrl + "}", "");
        //        else
        //            dbSuffUrl = Url.Replace(dbPreUrl, "").Replace("{DbUrl:" + dbUrl + "}", "");


        //        charSetMatch = Regex.Match(dbUrl, "(?<=<DbType>)[\\s\\S]*?(?=</DbType>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //        string dType = charSetMatch.Groups[0].ToString();

        //        charSetMatch = Regex.Match(dbUrl, "(?<=<Con>)[\\s\\S]*?(?=</Con>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //        string strCon = cTool.DecodingDBCon(charSetMatch.Groups[0].ToString());

        //        charSetMatch = Regex.Match(dbUrl, "(?<=<Sql>)[\\s\\S]*?(?=</Sql>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //        string sql = charSetMatch.Groups[0].ToString();

        //        DataTable d = null;
        //        switch (int.Parse(dType))
        //        {
        //            case (int)cGlobalParas.DatabaseType.Access:
        //                d = NetMiner.Data.Access.SQLHelper.ExecuteDataTable(strCon, sql);
        //                break;
        //            case (int)cGlobalParas.DatabaseType.MSSqlServer:
        //                d = NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(strCon, sql);
        //                break;
        //            case (int)cGlobalParas.DatabaseType.MySql:
        //                d = NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(strCon, sql);
        //                break;
        //        }

        //        for (int i = 0; i < d.Rows.Count; i++)
        //        {
        //            //根据数据库调用，合成Url
        //            list_Url.Add(dbPreUrl + d.Rows[i][0].ToString() + dbSuffUrl);
        //        }

        //        #endregion

        //    }
        //    else
        //        list_Url.Add(Url);

        //    List<string> r_Urls = new List<string>();

        //    for (int i=0;i<list_Url.Count ;i++)
        //    {
        //        List<string> urls = SplitParaUrl(list_Url[i]);
        //        r_Urls.AddRange(urls);
        //    }

        //    return r_Urls;
        //}

        //private List<string> SplitParaUrl(string Url)   //, bool IsUrlEncode, string UrlEncode
        //{
        //    List<string> tmp_list_Url = new List<string>();
        //    List<string> list_Url;
        //    List<string> g_Url = new List<string>();
        //    string Para = "";
        //    string oldUrl = Url;
        //    bool isSynPara=false ;

        //    //在此先判断是否为db提取网址的参数
        //    //从5.41版本开始，数据库参数为动态参数
           
          

        //        #region 此部分分解url参数
        //        if (Url.IndexOf("{Syn:") > -1)
        //            isSynPara = true;

        //        //加了转义符号的识别，如果有转义符，则按照正常的{}处理，转义符为\
        //        //if (!Regex.IsMatch(Url, "{.*}"))
        //        //判断是否有参数，如果没有参数，则直接返回
        //        if (!Regex.IsMatch(Url, "[^\\\\]{.*[^\\\\]}",RegexOptions.Multiline ))
        //        {
        //            if (Regex.IsMatch(Url, "\\\\{"))
        //            {
        //                //表明是转义符号，处理
        //                Url = Url.Replace("\\{", "{");
        //                Url = Url.Replace("\\}", "}");
        //            }

        //            tmp_list_Url.Add(Url);
        //            return tmp_list_Url;
        //        }

        //        //增加tmp_list_Url的初始值
        //        //初始值为Url第一个参数前所有字符
        //        //应该以{为准
        //        //通过正则获取，改变以前的的获取方式，否则如果有大括号转义会出错
        //        string strPreMatch = "[\\s\\S]+?[^\\\\](?={)";
        //        Match urlPre = Regex.Match(Url, strPreMatch, RegexOptions.IgnoreCase );
        //        string strUrlPre = urlPre.Groups[0].Value;
        //        tmp_list_Url.Add(strUrlPre);

        //        int first = 0;
        //        while (Regex.IsMatch(Url, "[^\\\\]?{.*[^\\\\]?}"))
        //        {
        //            //提取参数内容
        //            //string strMatch = "(?<={)[^}]*(?=})";
        //            //加了转义符号的识别，如果有转义符，则按照正常的{}处理，转义符为\
        //            //获取参数
        //            string strMatch = "(?<={)[^{]*?[^\\\\](?=})";
        //            Match s = Regex.Match(Url, strMatch, RegexOptions.IgnoreCase);
        //            string UrlPara = s.Groups[0].Value;

        //            if (string.IsNullOrEmpty(UrlPara))
        //            {
        //                break;
        //            }

        //            g_Url = getListUrl(UrlPara); //,IsUrlEncode ,UrlEncode 

        //            list_Url = new List<string>();

        //            for (int j = 0; j < tmp_list_Url.Count; j++)
        //            {
        //                if (g_Url != null && g_Url.Count >0)
        //                {
        //                    for (int i = 0; i < g_Url.Count; i++)
        //                    {
        //                        try
        //                        {
        //                            list_Url.Add(tmp_list_Url[j].ToString() + Para + g_Url[i].ToString());
        //                        }
        //                        catch (System.Exception ex)
        //                        {
        //                            throw ex;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    list_Url.Add(tmp_list_Url[j].ToString() + Para + "{" + UrlPara + "}");

        //                }
        //            }

        //            tmp_list_Url = list_Url;
        //            list_Url = null;

        //            string strSufMatch = "(?<=[^\\\\]})[\\s\\S]*";
        //            Match urlSuf = Regex.Match(Url, strSufMatch, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //            string strUrlSuf = urlSuf.Groups[0].Value;
        //            Url = strUrlSuf;

        //            //判断是否还有参数，如果有，则截取中间部分以拼接网址
        //            if (Regex.IsMatch(Url, "[^\\\\]?{.*[^\\\\]?}"))
        //            {
        //                //Para = Url.Substring(Url.IndexOf("&"), Url.IndexOf("=") - Url.IndexOf("&") + 1);
        //                //Para = Url.Substring(0, Url.IndexOf("{"));

        //                string strMidMatch = "[^{]*[^\\\\](?={)";
        //                Match urlMid = Regex.Match(Url, strMidMatch, RegexOptions.IgnoreCase);
        //                string strUrlMid = urlMid.Groups[0].Value;

        //                Para = strUrlMid;
        //            }

        //            first++;
        //        }

        //        #region 处理同步参数
        //        if (isSynPara == true)
        //        {
        //            //提取一个实例网址出来，判断是否有同步参数

        //            //存在同步参数
        //            string strMatch = "(?<={Syn:)[^}]*(?=})";
        //            Match s = Regex.Match(oldUrl, strMatch, RegexOptions.IgnoreCase);
        //            string UrlPara = s.Groups[0].Value;

        //            List<string> s_Url = getSynUrl(UrlPara);
        //            int sI = 0;
        //            for (int i = 0; i < tmp_list_Url.Count; i++)
        //            {
        //                int sIndex = tmp_list_Url[i].IndexOf("{Syn:");
        //                string s1 = tmp_list_Url[i].Substring(0, sIndex);
        //                string s2 = tmp_list_Url[i].Substring(tmp_list_Url[i].IndexOf("}", sIndex) + 1, tmp_list_Url[i].Length - tmp_list_Url[i].IndexOf("}", sIndex) - 1);

        //                if (sI >= s_Url.Count)
        //                    sI = 0;
        //                tmp_list_Url[i] = s1 + s_Url[sI] + s2;
        //                sI++;
        //            }

        //        }
        //        #endregion

        //        list_Url = new List<string>();

        //        for (int m = 0; m < tmp_list_Url.Count; m++)
        //        {
        //            list_Url.Add(tmp_list_Url[m].ToString() + Url);
        //        }

        //        tmp_list_Url = null;
        //        g_Url = null;
        //        #endregion
            


        //    return list_Url;

        //}

        //判断当前所含参数的数量,其中包括字典的内容
        //public int GetUrlCount(string Url)
        //{
        //    if (Url == "")
        //        return 0;

        //    int UrlCount = 1;
        //    int SumUrlCount = 0;
        //    List<string> g_Url = null;

        //    string[] sUrls = null; 

        //    //在此处理如果post数据中，有可能存在换行的情况，因此需要判断
        //    //这是一个网址，不能切分成多个网址，如果存在此情况，按照一个网址处理
        //    if (Url.IndexOf ("<POST")>0 && Url.IndexOf ("</POST>")>0)
        //    {
        //        sUrls = new string[1];
        //        sUrls[0] = Url;
        //    }
        //    else
        //        sUrls = Regex.Split(Url, "\r\n");

        //    for (int index = 0; index < sUrls.Length; index++)
        //    {
        //        g_Url= new List<string>();
        //        Url = sUrls[index];
        //        UrlCount = 1;

        //        #region 计算数据库网址
        //        if (Url.Contains("{DbUrl:"))
        //        {
        //            Match charSetMatch = Regex.Match(Url, "(?<=<DbType>)[\\s\\S]*?(?=</DbType>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //            string dType = charSetMatch.Groups[0].ToString();

        //            charSetMatch = Regex.Match(Url, "(?<=<Con>)[\\s\\S]*?(?=</Con>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //            string strCon = cTool.DecodingDBCon(charSetMatch.Groups[0].ToString());

        //            charSetMatch = Regex.Match(Url, "(?<=<Sql>)[\\s\\S]*?(?=</Sql>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //            string sql = charSetMatch.Groups[0].ToString();

        //            try
        //            {
        //                if ((sql.ToLower().Contains(" top ") && int.Parse(dType) == (int)cGlobalParas.DatabaseType.Access)
        //                    || (sql.ToLower().Contains(" top ") && int.Parse(dType) == (int)cGlobalParas.DatabaseType.MSSqlServer))
        //                {
        //                    //提取top后面的数字
        //                    Match m = Regex.Match(sql, "(?<=top\\s*)\\d+?(?=\\s)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //                    if (m != null)
        //                        UrlCount = int.Parse(m.Groups[0].ToString());
        //                }
        //                else if (sql.ToLower().Contains(" limit ") && int.Parse(dType) == (int)cGlobalParas.DatabaseType.MySql)
        //                {
        //                    Match m = Regex.Match(sql, "(?<=limit\\s*\\d*\\s*,\\s*)\\d+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //                    if (m != null)
        //                        UrlCount = int.Parse(m.Groups[0].ToString());
        //                }
        //                else
        //                {
        //                    //将sql转换成Count（*）
        //                    //string[] ss = sql.Split(' ');
        //                    //string sql1 = string.Empty;
        //                    //for (int i = 0; i < ss.Length; i++)
        //                    //{
        //                    //    if (i == 0)
        //                    //    {
        //                    //        sql1 += "select count(";
        //                    //    }
        //                    //    else if (i == 1)
        //                    //    {
        //                    //        sql1 += ss[i].Trim() + ") ";
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        sql1 += ss[i] + " ";
        //                    //    }
        //                    //}

        //                    sql = sql.ToLower();
        //                    sql = "select count(1) " + sql.Substring(sql.IndexOf("from"), sql.Length - sql.IndexOf("from"));

        //                    //sql = sql1.Trim();

        //                    switch (int.Parse(dType))
        //                    {
        //                        case (int)cGlobalParas.DatabaseType.Access:
        //                            UrlCount = int.Parse(NetMiner.Data.Access.SQLHelper.ExecuteScalar(strCon, sql).ToString());
        //                            break;
        //                        case (int)cGlobalParas.DatabaseType.MSSqlServer:
        //                            UrlCount = int.Parse(NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(strCon, sql).ToString());
        //                            break;
        //                        case (int)cGlobalParas.DatabaseType.MySql:
        //                            UrlCount = int.Parse(NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(strCon, sql).ToString());
        //                            break;
        //                    }
        //                }
        //            }
        //            catch
        //            {
        //                UrlCount = 0;
        //            }

        //        }
        //        #endregion


        //        #region  计算参数类型
        //        //加了转义符号的识别，如果有转义符，则按照正常的{}处理，转义符为\
        //        //if (!Regex.IsMatch(Url, "{.*}"))
        //        if (!Regex.IsMatch(Url, "[^\\\\]{.*[^\\\\]}"))
        //            {
        //                if (Regex.IsMatch(Url, "\\\\{"))
        //                {
        //                    //表明是转义符号，处理
        //                    Url = Url.Replace("\\{", "{");
        //                    Url = Url.Replace("\\}", "}");
        //                }

        //                g_Url.Add(Url);
        //                UrlCount= g_Url.Count;
        //            }
        //            else
        //            {
        //                while (Regex.IsMatch(Url, "[^\\\\]?{.*[^\\\\]?}"))
        //                {
        //                    //提取参数内容
        //                    //string strMatch = "(?<={)[^}]*(?=})";
        //                    //加了转义符号的识别，如果有转义符，则按照正常的{}处理，转义符为\
        //                    string strMatch = "(?<={)[^{]*[^\\\\](?=})";
        //                    Match s = Regex.Match(Url, strMatch, RegexOptions.IgnoreCase);
        //                    string UrlPara = s.Groups[0].Value;

        //                    //因为仅计算网址的数量，所以不需要对url进行编码转换，如果有需要转换的话，也不需要
        //                    g_Url = getListUrl(UrlPara);

        //                    //if (g_Url == null)
        //                    //    return 0;

        //                    if (g_Url != null)
        //                        UrlCount = UrlCount * g_Url.Count;

        //                    Url = Url.Substring(Url.IndexOf("}") + 1, Url.Length - Url.IndexOf("}") - 1);

        //                }
        //            }
        //        #endregion

        //        if (UrlCount == 0)
        //        {
        //            UrlCount = 1;
        //        }

        //        SumUrlCount += UrlCount;
        //    }

        //    return SumUrlCount;
        //}

        /// <summary>
        /// 分解指定的参数，与业务逻辑无关
        /// </summary>
        /// <param name="dicPre"></param>
        /// <returns></returns>
        //public List<string> getListUrl(string dicPre)//,bool IsUrlEncode,string UrlEncode
        //{
        //    List<string> list_Para = new List<string>();
        //    Regex re;
        //    MatchCollection aa;
        //    int step;
        //    int startI;
        //    int endI;
        //    int i = 0;

        //    string split1 =string.Empty ;
        //    string split2 = string.Empty;
        //    string tmpDic = string.Empty;

        //    string startstrI = string.Empty;
        //    string endstrI = string.Empty;

        //    int startYear = 0;
        //    int startMonth = 0;
        //    int startDay = 0;

        //    int endYear = 0;
        //    int endMonth = 0;
        //    int endDay = 0;

        //    if (dicPre.IndexOf(":") < 0)
        //    {
        //        list_Para.Add(dicPre);
        //    }
        //    else
        //    {
        //        if (dicPre.StartsWith("Date("))
        //        {
        //            try
        //            {
        //                //提取出日期的格式
        //                string dateFormat = dicPre.Substring(dicPre.IndexOf("(") + 1, dicPre.IndexOf(")") - dicPre.IndexOf("(") - 1);

        //                //提取日期的范围
        //                string sDate = dicPre.Substring(dicPre.IndexOf(":") + 1, dicPre.IndexOf(",") - dicPre.IndexOf(":") - 1);
        //                string eDate = dicPre.Substring(dicPre.IndexOf(",") + 1, dicPre.Length - dicPre.IndexOf(",") - 1);

        //                for (DateTime iDate = DateTime.Parse(sDate); iDate <= DateTime.Parse(eDate); iDate = iDate.AddDays(1))
        //                {
        //                    list_Para.Add(iDate.ToString(dateFormat));
        //                }
        //            }
        //            catch(System.Exception ex)
        //            {
        //                return list_Para;
        //            }

        //        }
        //        else
        //        {
        //            switch (dicPre.Substring(0, dicPre.IndexOf(":")))
        //            {

        //                case "Num":

        //                    if (dicPre.IndexOf("/") > 0)
        //                    {
        //                        //表示为计算最大值得处理
        //                        string[] ss = dicPre.Split(',');
        //                        startI = int.Parse(ss[0].Replace("Num:", ""));
        //                        step = int.Parse(ss[2]);

        //                        try
        //                        {
        //                            string[] ss1 = ss[1].Split('/');
        //                            if (string.IsNullOrEmpty(ss1[0]))
        //                            {
        //                                endI = startI;
        //                            }
        //                            else
        //                            {
        //                                endI = int.Parse(ss1[0]) / int.Parse(ss1[1]);

        //                                if (int.Parse(ss1[0]) % int.Parse(ss1[1]) > 0)
        //                                    endI = endI + 1;

        //                                if (endI < startI)
        //                                    endI = startI;
        //                            }
        //                        }
        //                        catch { endI = 0; }
        //                    }
        //                    else
        //                    {
        //                        re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //                        aa = re.Matches(dicPre);

        //                        startI = int.Parse(aa[0].Groups[0].Value.ToString());
        //                        endI = int.Parse(aa[1].Groups[0].Value.ToString());
        //                        step = int.Parse(aa[2].Groups[0].Value.ToString());
        //                    }

        //                    if (step > 0)
        //                    {
        //                        for (i = startI; i <= endI; i = i + step)
        //                        {
        //                            list_Para.Add(i.ToString());
        //                        }
        //                    }
        //                    else
        //                    {
        //                        for (i = startI; i >= endI; i = i + step)
        //                        {
        //                            list_Para.Add(i.ToString());
        //                        }
        //                    }



        //                    break;

        //                case "NumZero":
        //                    re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //                    aa = re.Matches(dicPre);

        //                    startstrI = aa[0].Groups[0].Value.ToString();
        //                    endstrI = aa[1].Groups[0].Value.ToString();
        //                    step = int.Parse(aa[2].Groups[0].Value.ToString());
        //                    startI = int.Parse(aa[0].Groups[0].Value.ToString());
        //                    endI = int.Parse(aa[1].Groups[0].Value.ToString());

        //                    int NumLen = 0;

        //                    if (step > 0)
        //                    {
        //                        NumLen = endstrI.Length;
        //                        for (i = startI; i <= endI; i = i + step)
        //                        {
        //                            string tempI = i.ToString();
        //                            while (tempI.Length < NumLen)
        //                            {
        //                                tempI = "0" + tempI;
        //                            }
        //                            list_Para.Add(tempI);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        NumLen = startstrI.ToString().Length;

        //                        for (i = startI; i >= endI; i = i + step)
        //                        {
        //                            string tempI = i.ToString();
        //                            while (tempI.Length < NumLen)
        //                            {
        //                                tempI = "0" + tempI;
        //                            }
        //                            list_Para.Add(tempI);
        //                        }
        //                    }
        //                    break;



        //                case "Letter":
        //                    startI = getAsc(dicPre.Substring(dicPre.IndexOf(":") + 1, 1));
        //                    endI = getAsc(dicPre.Substring(dicPre.IndexOf(",") + 1, 1));

        //                    if (startI > endI)
        //                    {
        //                        step = -1;
        //                    }
        //                    else
        //                    {
        //                        step = 1;
        //                    }

        //                    for (i = startI; i <= endI; i = i + step)
        //                    {
        //                        char s;
        //                        s = Convert.ToChar(i);
        //                        list_Para.Add(s.ToString());
        //                    }

        //                    break;

        //                case "CurrentDate":
        //                    dicPre = dicPre.Replace("CurrentDate:", "");
        //                    list_Para.Add(System.DateTime.Now.ToString(dicPre));
        //                    break;

        //                case "Timestamp":
        //                    dicPre = dicPre.Replace("CurrentDate:", "");
        //                    list_Para.Add(cTool.GetTimestamp().ToString());
        //                    break;

        //                case "Dict":
        //                    oDict d = new oDict(m_workPath);
        //                    string tClass = dicPre.Substring(dicPre.IndexOf(":") + 1, dicPre.Length - dicPre.IndexOf(":") - 1);
        //                    ArrayList dName = d.GetDict(tClass);

        //                    if (dName != null)
        //                    {
        //                        for (i = 0; i < dName.Count; i++)
        //                        {
        //                            list_Para.Add(dName[i].ToString());
        //                        }
        //                    }

        //                    if (list_Para.Count > 0)
        //                    {
        //                        if (list_Para[0].IndexOf("{") > -1)
        //                        {
        //                            //表明还有参数
        //                            List<string> list_para1 = new List<string>();
        //                            for (i = 0; i < list_Para.Count; i++)
        //                            {
        //                                List<string> paras = SplitUrl(list_Para[i]);
        //                                list_para1.AddRange(paras);
        //                            }
        //                            list_Para = list_para1;
        //                        }
        //                    }

        //                    break;

        //                default:
        //                    list_Para.Add("{" + dicPre + "}");
        //                    break;
        //            }
        //        }
        //    }

        //    return list_Para;
        //}

        //public List<string> getSynUrl(string dicPre)
        //{
        //    List<string> list_Para = new List<string>();
        //    Regex re;
        //    MatchCollection aa;
        //    int step;
        //    int startI;
        //    int endI;
        //    int i = 0;

        //    string startstrI = string.Empty;
        //    string endstrI = string.Empty;

        //    re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //    aa = re.Matches(dicPre);

        //    startI = int.Parse(aa[0].Groups[0].Value.ToString());
        //    endI = int.Parse(aa[1].Groups[0].Value.ToString());
        //    step = int.Parse(aa[2].Groups[0].Value.ToString());

        //    if (step > 0)
        //    {
        //        for (i = startI; i <= endI; i = i + step)
        //        {
        //            list_Para.Add(i.ToString());
        //        }
        //    }
        //    else
        //    {
        //        for (i = startI; i >= endI; i = i + step)
        //        {
        //            list_Para.Add(i.ToString());
        //        }
        //    }

        //    return list_Para;
        //}

        //public string getSynUrl(string dicPre,int firstIndex)
        //{
        //    List<string> list_Para = new List<string>();
        //    Regex re;
        //    MatchCollection aa;
        //    int step;
        //    int startI;
        //    int endI;
        //    int i = 0;

        //    string startstrI = string.Empty;
        //    string endstrI = string.Empty;
        //    int dValue = 0;

        //    re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //    aa = re.Matches(dicPre);

        //    startI = int.Parse(aa[0].Groups[0].Value.ToString());
        //    endI = int.Parse(aa[1].Groups[0].Value.ToString());
        //    step = int.Parse(aa[2].Groups[0].Value.ToString());

        //    if (firstIndex > endI)  //标识循环已经超过了指定的最大范围
        //        return "";

        //    dValue = 0;
        //    try
        //    {
                
        //        dValue = startI + firstIndex * step;
              
        //        firstIndex++;

        //        if (dValue > endI)
        //            dValue = endI;



                

        //    }
        //    catch (System.Exception)
        //    {


        //    }

        //    return dValue.ToString ();
        //}


        private string GetNextPara(string Url, string dicPre, string paraValue, ref int firstIndex)
        {
            Regex re;
            MatchCollection aa;
            int step;
            int startI;
            int endI;
            int loop;

            string strNext = "";

            int i1 = 0;
            int i2 = 0;

            string strPre = "";
            string strSuf = "";

            int dValue = 0;

            switch (dicPre.Substring(0, dicPre.IndexOf(":")))
            {
                case "Num":

                    re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    aa = re.Matches(dicPre);

                    startI = int.Parse(aa[0].Groups[0].Value.ToString());
                    endI = int.Parse(aa[1].Groups[0].Value.ToString());
                    step = int.Parse(aa[2].Groups[0].Value.ToString());

                    i1 = paraValue.IndexOf("{");
                    i2 = paraValue.IndexOf("}");

                    strPre = paraValue.Substring(0, i1);
                    strSuf = paraValue.Substring(i2 + 1, paraValue.Length - i2 - 1);

                    if (firstIndex > endI)  //标识循环已经超过了指定的最大范围
                        return "";

                    dValue = 0;
                    try
                    {
                        //if (firstIndex != -1)
                        //{
                            dValue = startI + firstIndex * step;
                        //}
                        //else
                        //    dValue = startI;

                        firstIndex ++;

                        if (dValue > endI)
                            dValue = endI;

                        

                        strNext = strPre + dValue.ToString() + strSuf;

                    }
                    catch (System.Exception)
                    {


                    }

                    break;

                case "NumLoop":

                    re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    aa = re.Matches(dicPre);

                    startI = int.Parse(aa[0].Groups[0].Value.ToString());
                    endI = int.Parse(aa[1].Groups[0].Value.ToString());
                    step = int.Parse(aa[2].Groups[0].Value.ToString());
                    loop = int.Parse(aa[3].Groups[0].Value.ToString());

                    i1 = paraValue.IndexOf("{");
                    i2 = paraValue.IndexOf("}");

                    strPre = paraValue.Substring(0, i1);
                    strSuf = paraValue.Substring(i2 + 1, paraValue.Length - i2 - 1);

                    if (firstIndex > endI)  //标识循环已经超过了指定的最大范围
                        return "";

                    dValue = 0;

                    //开始处理循环参数
                    try
                    {
                        //if (firstIndex != -1)
                        //{
                            dValue = startI + firstIndex * step;
                        //}
                        //else
                        //    dValue = startI;

                        firstIndex ++;

                        if (dValue > endI * loop)
                            dValue = endI * loop;

                        

                        if (dValue <= endI)
                            strNext = strPre + dValue.ToString() + strSuf;
                        else
                            strNext = strPre + (dValue % endI).ToString() + strSuf;

                    }
                    catch (System.Exception)
                    {


                    }
                    break;
                case "Timestamp":
                    re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    aa = re.Matches(dicPre);

                    startI = int.Parse(aa[0].Groups[0].Value.ToString());
                    endI = int.Parse(aa[1].Groups[0].Value.ToString());
                    step = int.Parse(aa[2].Groups[0].Value.ToString());

                    i1 = paraValue.IndexOf("{");
                    i2 = paraValue.IndexOf("}");

                    strPre = paraValue.Substring(0, i1);
                    strSuf = paraValue.Substring(i2 + 1, paraValue.Length - i2 - 1);

                    strNext = strPre + ToolUtil.GetTimestamp().ToString () + strSuf;
                    break;
                default:
                    break;
            }

            return strNext;
        }

        private int getAsc(string s)
        {
            byte[] array = new byte[1];
            array = System.Text.Encoding.ASCII.GetBytes(s);
            int asciicode = (int)(array[0]);
            return asciicode;
        }

        public string GetNextUrl(string Url, string UrlSource, string UrlRule, string RegexNextPage,
            ref int firstIndex,   int NavLevel)
        {
            int oldIndex = firstIndex;

            //先对UrlRule进行转移
            UrlRule = ToolUtil.CutRegexWildcard(UrlRule);

            string strNext = "";
            //先默认post请求的方式
            string postPre = "<POST:ASCII>";

            if (UrlRule == "")
                return "";

            //先处理当前网址参数
            if (UrlRule.IndexOf("{CurrtentPageUrl}")>-1)
            {
                UrlRule = UrlRule.Replace("{CurrtentPageUrl}", Url);
            }

            if (UrlSource == null || UrlSource == "")
            {
                throw new NetMinerException("网页为空，请重新调用！");
            }
                //{
                //    #region  获取源码
                //    cGatherWeb gW = new cGatherWeb(this.m_workPath, ref this.m_ProxyControl, this.m_IsProxy, this.m_IsProxyFirst, this.m_pType, this.m_pAddress, this.m_pPort);

                //    try
                //    {
                //        if (headers == null)
                //        {
                //            gW.IsCustomHeader = false;
                //        }
                //        else
                //        {
                //            gW.IsCustomHeader = true;

                //            //开始计算header
                //            for (int i = 0; i < headers.Count; i++)
                //            {
                //                if (headers[i].Range == "[All]")
                //                    gW.Headers.Add(headers[i]);
                //                else if (headers[i].Range.Contains(NavLevel.ToString()))
                //                    gW.Headers.Add(headers[i]);
                //            }
                //        }
                //        UrlSource = gW.GetHtml(Url, webCode, isUrlCode, isTwoUrlCode, UrlCode , ref cookie, "", "",
                //            true, isAutoUpdateHeader, referUrl, isGatherCoding,CodingFlag, CodingUrl, Plugin);
                //    }
                //    catch (System.Exception ex)
                //    {
                //        throw new NetMinerException("获取网页源码出错，请检查网页地址、网页编码、网址编码格式，错误信息为：" + ex.Message);
                //    }
                //    gW = null;

                //    #endregion
                //}

                //在此判断是否为可视化导航，如果是，则调用可视化方法
                if (UrlRule.StartsWith("<XPath>"))
            {
                Match charSetMatch1 = Regex.Match(UrlRule, "(?<=<XPath>).*?(?=</XPath>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string xPathRule = charSetMatch1.Groups[0].ToString();


                List<string> Urls = GetUrlsByRuleByXPath(Url, UrlSource, xPathRule,0);
                if (Urls != null && Urls.Count > 0)
                    strNext = Urls[0].ToString();

                return strNext;
            }

            #region  加工规则
            Match charSetMatch = Regex.Match(UrlRule, "(?<=<Prefix>).*?(?=</Prefix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string strPrefix = charSetMatch.Groups[0].ToString();
            UrlRule = Regex.Replace(UrlRule, "(<Prefix>).*?(</Prefix>)", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);


            charSetMatch = Regex.Match(UrlRule, "(?<=<Suffix>).*?(?=</Suffix>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string strSuffix = charSetMatch.Groups[0].ToString();
            UrlRule = Regex.Replace(UrlRule, "(<Suffix>).*?(</Suffix>)", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            charSetMatch = Regex.Match(UrlRule, "(?<=<Include>).*?(?=</Include>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string strIncludeWord = charSetMatch.Groups[0].ToString();
            UrlRule = Regex.Replace(UrlRule, "<Include>.*?</Include>", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            charSetMatch = Regex.Match(UrlRule, "(?<=<NoInclude>).*?(?=</NoInclude>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string strNoIncludeWord = charSetMatch.Groups[0].ToString();
            UrlRule = Regex.Replace(UrlRule, "<NoInclude>.*?</NoInclude>", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            bool isRegex = false;
            string strOldValue = string.Empty;
            string strNewValue = string.Empty;

            if (Regex.IsMatch(UrlRule, "<RegexReplace>"))
            {
                isRegex = true;

                charSetMatch = Regex.Match(UrlRule, "(?<=<RegexReplace>).*?(?=</RegexReplace>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string ss = charSetMatch.Groups[0].ToString();

                strOldValue = ss.Substring(ss.IndexOf("<OldValue:") + 10, ss.IndexOf("><NewValue:") - 10);
                strNewValue = ss.Substring(ss.IndexOf("<NewValue:") + 10, ss.Length - ss.IndexOf("<NewValue:") - 11);

                UrlRule = Regex.Replace(UrlRule, "<RegexReplace>.*?</RegexReplace>", "");
            }
            else if (Regex.IsMatch(UrlRule, "<Replace>"))
            {
                isRegex = false;

                charSetMatch = Regex.Match(UrlRule, "(?<=<Replace>).*?(?=</Replace>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string ss = charSetMatch.Groups[0].ToString();

                strOldValue = ss.Substring(ss.IndexOf("<OldValue:") + 10, ss.IndexOf("><NewValue:") - 10);
                strNewValue = ss.Substring(ss.IndexOf("<NewValue:") + 10, ss.Length - ss.IndexOf("<NewValue:") - 11);
                
                UrlRule = Regex.Replace(UrlRule, "<Replace>.*?</Replace>", "");

                //UrlRule = Regex.Replace(UrlRule, strOldValue, strNewValue);
                
            }
            #endregion

            //处理参数
            charSetMatch = Regex.Match(UrlRule, "(?<=<Parameter>).*?(?=</Parameter>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string nextPara = charSetMatch.Groups[0].ToString();
            UrlRule = Regex.Replace(UrlRule, "(<Parameter>).*?(</Parameter>)", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            string sRule = "";

            if (nextPara == "")
            {
                if (UrlRule.StartsWith("<Common>") || UrlRule.StartsWith("<Regex>"))
                {
                    //复杂的导航规则

                    if (Regex.IsMatch(UrlRule, "<Common>"))
                    {
                        //Rule = @"(?<=[href=|src=|open(][\W])";

                        charSetMatch = Regex.Match(UrlRule, "(?<=<Common>).*?(?=</Common>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string strtrule = charSetMatch.Groups[0].ToString();
                        sRule = strtrule;

                    }
                    else if (Regex.IsMatch(UrlRule, "<Regex>"))
                    {
                        charSetMatch = Regex.Match(UrlRule, "(?<=<Regex>).*?(?=</Regex>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string strPre = charSetMatch.Groups[0].ToString();
                        sRule = strPre;
                    }
                }
                else if (UrlRule.StartsWith("<Trait>"))
                {
                    //简单规则导航，增加了标签

                    charSetMatch = Regex.Match(UrlRule, "(?<=<Trait>).*?(?=</Trait>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string trait = charSetMatch.Groups[0].ToString();

                    sRule = RegexNextPage + "(?=" + ToolUtil.RegexReplaceTrans(trait) + ")";
                }

                #region 匹配下一页的数据
                Regex re = new Regex(sRule, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                MatchCollection aa = re.Matches(UrlSource);


                foreach (Match ma in aa)
                {
                    strNext = ma.Value.ToString();
                    strNext = System.Web.HttpUtility.HtmlDecode(strNext);
                    strNext = Regex.Replace(strNext, " ", "%20");
                    break;
                }

                #endregion
            }
            else
            {
                //在此处理参数的问题
                if (nextPara != "")
                {
                    #region 处理表单值
                    bool isFormValue = false;
                    Hashtable rFormValue = new Hashtable();
                    while (Regex.IsMatch(nextPara, "{FormValue}"))
                    {

                        #region 处理来路页面的信息
                        //建立一个来路页面的表单数据值
                        if (isFormValue == false)
                        {

                            //先获取form
                            Regex reForm = new Regex("<input[^>]+?>", RegexOptions.IgnoreCase);
                            MatchCollection mcForm = reForm.Matches(UrlSource);
                            foreach (Match ma in mcForm)
                            {
                                //判断Type是否为hidden，如果是，则添加到表单表中
                                Match a = Regex.Match(ma.ToString(), @"type=.+?\s", RegexOptions.IgnoreCase);
                                string strT = a.Groups[0].Value.ToString();
                                //if (strT.IndexOf("hidden", StringComparison.CurrentCultureIgnoreCase) > -1)
                                //{
                                    try
                                    {
                                        string n = "";
                                        string v = "";
                                        string ss1 = ma.ToString();
                                        if (ss1.EndsWith("/>"))
                                        {
                                            ss1 = ss1.Substring(0, ss1.Length - 2) + " />";
                                        }
                                        else
                                        {
                                            if (ss1.EndsWith(">"))
                                                ss1 = ss1.Substring(0, ss1.Length - 1) + " >";
                                        }


                                        Match a1 = Regex.Match(ss1, @"(?<=name=).+?\s", RegexOptions.IgnoreCase);
                                        n = a1.Groups[0].Value.ToString();

                                        a1 = Regex.Match(ss1, @"(?<=value=).+?\s", RegexOptions.IgnoreCase);
                                        v = a1.Groups[0].Value.ToString();

                                        n = n.Replace("'", "");
                                        n = n.Replace("\"", "").Trim();

                                        v = v.Replace("'", "");
                                        v = v.Replace("\"", "").Trim();


                                        rFormValue.Add(n, v);
                                    }
                                    catch { }
                                //}

                            }
                            isFormValue = true;
                        }

                        #endregion

                        //charSetMatch = Regex.Match(nextPara, "[^/|^?|^&|^>]+?(?=={FormValue})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        //charSetMatch = Regex.Match(nextPara, "(?<=[&|\\?|>|/])[^=]+?(?=={FormValue})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        charSetMatch = Regex.Match(nextPara, "(?<=[&|\\?|>|/])[^=|^>|^&]+?(?=={FormValue})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string label = charSetMatch.Groups[0].ToString();

                        if (rFormValue == null || rFormValue.Count == 0)
                            break;

                        foreach (DictionaryEntry de1 in rFormValue)
                        {
                            if (label.ToLower() == de1.Key.ToString().ToLower())
                            {
                                string ss = de1.Value.ToString();

                                //解析出来的网址不用做编码处理，因为再次请求时，系统会处理编码的问题
                                //if (isUrlCode == true)
                                //{
                                //    ss = ToolUtil.UrlParaEncode(ss, UrlCode);
                                //}

                                nextPara = nextPara.Replace(label + "={FormValue}", label + "=" + ss);
                                break;
                            }
                        }
                    }
                    #endregion

                    if (Regex.IsMatch(nextPara, "{Timestamp:[\\d]*?}"))
                    {
                        nextPara = Regex.Replace(nextPara, "{Timestamp:[\\d]*?}", ToolUtil.GetTimestamp().ToString());
                    }

                    //从传入的网址中获取值
                    while (Regex.IsMatch(nextPara, "{UrlValue:.*?}"))
                    {
                        charSetMatch = Regex.Match(nextPara, "(?<={UrlValue:).*?(?=})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string urlPara = charSetMatch.Groups[0].ToString();

                        charSetMatch = Regex.Match(Url, urlPara, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string urlValue = charSetMatch.Groups[0].ToString();

                        //nextPara = Regex.Replace(nextPara, "{UrlValue:.*?}", urlValue);
                        nextPara = nextPara.Replace("{UrlValue:" + urlPara + "}", urlValue);
                    }

                    //处理来路页面的数值
                    //2016年3月22日 将UrlRuleF爱卫了nextPara
                    while (Regex.IsMatch(nextPara, "{FromValue:.*?}"))
                    {
                        charSetMatch = Regex.Match(nextPara, "(?<={FromValue:).*?(?=})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string urlPara = charSetMatch.Groups[0].ToString();

                        charSetMatch = Regex.Match(UrlSource, urlPara, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string urlValue = charSetMatch.Groups[0].ToString();

                        //UrlRule = Regex.Replace(UrlRule, "{FromValue:}", urlValue);
                        nextPara = nextPara.Replace("{FromValue:" + urlPara + "}", urlValue);
                    }

                    //再判断是否有参数的正则
                    //此处于2013-9-13进行修改，增加了多参数的处理
                    while (Regex.IsMatch(nextPara, "<Para>"))
                    {
                        //处理参数，替换成匹配后的结果
                        charSetMatch = Regex.Match(nextPara, "(?<=<Para>).*?(?=</Para>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string strRegex = charSetMatch.Groups[0].ToString();

                        charSetMatch = Regex.Match(UrlSource, strRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string pValue = charSetMatch.Groups[0].ToString();

                        //不用编码处理，因为再次请求还会进行编码
                        //if (isUrlCode == true)
                        //{
                        //    pValue = ToolUtil.UrlParaEncode(pValue, UrlCode);
                        //}

                        //替换
                        nextPara = nextPara.Replace("<Para>" + strRegex + "</Para>", pValue);
                    }

                    //在此处理页码相加想减的问题
                    if (Regex.IsMatch(nextPara, "{Formula:.*?}"))
                    {
                        charSetMatch = Regex.Match(nextPara, "(?<={Formula:).*?(?=})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string urlPara = charSetMatch.Groups[0].ToString();

                        //开始提取公式前面的数字
                        charSetMatch = Regex.Match(nextPara, "\\d*(?={Formula)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string pageNum = charSetMatch.Groups[0].ToString();
                        if (pageNum != "")
                        {
                            int cutPage=0;
                            if (urlPara.Trim().StartsWith("+"))
                            {
                                charSetMatch = Regex.Match(urlPara, "\\d+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                string page1 = charSetMatch.Groups[0].ToString();
                                cutPage = int.Parse(pageNum) + int.Parse(page1);
                            }
                            else if (urlPara.Trim().StartsWith("-"))
                            {
                                charSetMatch = Regex.Match(urlPara, "\\d+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                string page1 = charSetMatch.Groups[0].ToString();
                                cutPage = int.Parse(pageNum) - int.Parse(page1);
                            }

                            //替换
                            nextPara = Regex.Replace(nextPara, "\\d*(?:{Formula:.*})", cutPage.ToString ());
                        }
                    }


                    strNext = nextPara;

                    if (Regex.IsMatch(nextPara, "{.*}"))
                    {
                        //提取参数内容
                        string strMatch = "(?<={)[^}]*(?=})";
                        Match s = Regex.Match(nextPara, strMatch, RegexOptions.IgnoreCase);
                        string UrlPara = s.Groups[0].Value;

                        strNext = GetNextPara(strNext, UrlPara, nextPara, ref firstIndex); //,IsUrlEncode ,UrlEncode 

                        

                        //判断是否有同步参数，如果有进行解析
                        if (strNext.IndexOf("{Syn:") > -1)
                        {
                            string strMatch1 = "(?<={Syn:)[^}]*(?=})";
                            Match s1 = Regex.Match(strNext, strMatch1, RegexOptions.IgnoreCase);
                            string UrlPara1 = s1.Groups[0].Value;

                            cUrlParse u = new cUrlParse(this.m_workPath);
                            string st_url = u.getSynUrl(UrlPara1, oldIndex);
                            u = null;

                            int sIndex = strNext.IndexOf("{Syn:");
                            string ss1 = strNext.Substring(0, sIndex);
                            string ss2 = strNext.Substring(strNext.IndexOf("}", sIndex) + 1, strNext.Length - strNext.IndexOf("}", sIndex) - 1);

                            strNext = ss1 + st_url + ss2;


                        }

                        //nextPara = strNext;

                    }

                }
            }

            //在此处理网址的加工
            //if (!string.IsNullOrEmpty (strNewValue ))

            //strNext = strNext.Replace("'", "");
            //strNext = strNext.Replace("\"", "");
            //只删除头和尾的引号
            strNext = Regex.Replace(strNext, "^[\"|']", "");
            strNext = Regex.Replace(strNext, "[\"|']$", "");

        
            strNext = EditUrl(strNext, strPrefix, strSuffix, strIncludeWord, strNoIncludeWord, strOldValue, strNewValue, isRegex);

            #region  判断下一页获取的地址的合法格式
            if (strNext != "" && strNext != Url && strNext != "#")
            {
                //判断获取的地址是否为相对地址
                if (strNext.StartsWith("//"))
                {
                    strNext = "http:" + strNext;
                }
                else if (strNext.Substring(0, 1) == "/" || strNext.Substring(0, 1) == "\\")
                {
                    string PreUrl = Url;
                    PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                    if (PreUrl.IndexOf("/") > 0)
                        PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));
                    PreUrl = "http://" + PreUrl;
                    strNext = PreUrl + strNext;
                }
                else if (strNext.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || strNext.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                {

                }
                else if (strNext.StartsWith("?", StringComparison.CurrentCultureIgnoreCase))
                {
                    Match ma = Regex.Match(Url, @".*(?=\?)");
                    string PreUrl = ma.Groups[0].Value.ToString();
                    if (PreUrl == "")
                        strNext = Url + strNext;
                    else
                        strNext = PreUrl + strNext;
                }
                else
                {
                    Match ma = Regex.Match(Url, ".*/");
                    string PreUrl = ma.Groups[0].Value.ToString();
                    strNext = PreUrl + strNext;
                }
            }
            #endregion
            

            
            return strNext;
        }

   
        /// <summary>
        /// 根据前后缀 字符串替换等操作进行网址的加工处理
        /// </summary>
        /// <param name="url"></param>
        /// <param name="strPre"></param>
        /// <param name="strSuf"></param>
        /// <param name="iWord"></param>
        /// <param name="niWord"></param>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        /// <param name="isRegex"></param>
        /// <returns></returns>
        private string EditUrl(string url,string strPre, string strSuf, string iWord, string niWord, string oValue, string nValue, bool isRegex)
        {
            if (iWord != "")
            {
                if (!(url.IndexOf(iWord, StringComparison.CurrentCultureIgnoreCase) > -1))
                {
                    return "";
                }
            }

            if (niWord != "")
            {
                if (url.IndexOf(niWord, StringComparison.CurrentCultureIgnoreCase) > -1)
                {
                    return "";
                }
            }

            if (oValue.Trim() != "")
            {
                if (isRegex)
                {
                    url = Regex.Replace(url, oValue, nValue, RegexOptions.IgnoreCase);
                }
                else
                {
                    url = url.Replace(oValue, nValue);
                }
            }

            url = strPre + url + strSuf;

            return url;
        }

    
    }
}

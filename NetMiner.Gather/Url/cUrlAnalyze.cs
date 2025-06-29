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

///���ܣ�������URL�����������
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
///�޶���2010-12-9 ������������ 
///�޶���2012-1-30 �޸�����ַ�����Ĵ���ȡ����ԭ�еļ򵥵�����ȫ����Ϊ����������ʵ�ʾ����ڽ��滯
///���ֽ����û����鴦��ʵ�ʴ���ĺ��Ĳ�û�б仯������ǩ�����˱仯
///2013-4-14 �޸�bug������������ʱ��Ҳ��Ҫ��head�����жϣ���������˴����ݣ�Ӧ�ô����head��������ҳ�����󣬷���
///�ᵼ�²ɼ�ʧ�ܡ�
///��V5.5��ʼ�������в��ٻ�ȡ��ҳԴ�룬���д���ķ��������뽫��ҳ���ݴ��롣����಻���������ҳ���������
namespace NetMiner.Gather.Url
{
   /// <summary>
   /// ����ɼ����õ�Url�����࣬����������õĹ����ȡ��ַ��������������ҳ����ҳ�Ȳ�����
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
        /// ������������нű�ִ��ʱ���̵߳ȴ�ʱ��
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
        ///����ָ���ĵ����������ҳ�浼������1.6�汾�У������˶�㵼���Ĺ���
        ///��ַ����������һ�Զ�Ĺ�ϵ����ÿһ����ĵ�����������һ�ԶࣨҲ����һ��һ�Ĺ�ϵ��
        ///�ڴ������Ǽ������������صĶ������յ���Ҫ�ɼ����ݵ���ַ
        ///��Ϊ�Ƕ�㵼�������������ڵݹ��һ���㷨
        ///������ַ�󷵻صĶ��Ǳ�׼��ַ��������������ַ�����
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


            //��һ�㵼���ֽⶼ�Ǵ�һ����һ��ַ���У�֮����
            //ѡ�񼯺ϣ���Ϊ��ͳһ���ýӿڲ���
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
                //����ʧ�ܣ��޷�������������
                throw new NetMinerException("����������δ֪���󣬴�����ϢΪ��" + ex.Message );
                return null;
            }

            return Urls;
        }

        ///����������ҳ
        ///�ж��Ƿ�Ϊ���һ��������������Ҫע��һ�����⣬��Ϊ�п���
        ///�洢�ļ��𲢲��ǰ���˳����еģ����ԣ�Ҫ���ݴ���ļ���Level����
        ///�жϣ��������ִ��󣬵�����ҳ�Ľ��������ǰ���˳��ģ������
        ///�޷�����
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

            //����Level�õ���Ҫ��������ĵ�������
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
                    //�ж��Ƿ�Ϊ��׼��ĵ�����������򷵻أ�����������������
                    if (Level == nRules.Count)
                    {
                        Urls.AddRange(tmpUrls);
                    }
                    else
                    {
                        //��urlsת��Dictionary

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
        /// ���ݵ������ҳ���򣬻�ȡ��ҳ��ַ����һ������,V5.2���ӵ���ִ�й�������Ƕ�ҳ������ִ�й���Ĭ��Ϊ����
        /// ϵͳ֧�ִ��뵼����Ʃ�磺������վÿ�β�ѯ����Ҫ���뷽������ִ�У�������Ҫ����cookie��header
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

            //�ȶ�UrlRule����ת�ƣ������ƥ�����
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

            //�ж���ַ�Ƿ���ڲ�����������ڲ�����ȡ����һ��������ַ
            //��Ϊ������ת�ƣ�������Ҫ����ת���ַ������������ʧ�ܣ�ת���ַ� \
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

            #region ���ӻ�������
            //�ڴ��ж��Ƿ�Ϊ���ӻ�����������ǣ�����ÿ��ӻ�����
            if (UrlRule.StartsWith("<XPath>"))
            {
                Match charSetMatch1 = Regex.Match(UrlRule, "(?<=<XPath>).*?(?=</XPath>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string xPathRule = charSetMatch1.Groups[0].ToString();


                //Urls = GetUrlsByRule(Url, UrlSource, xPathRule);

                Urls = GetUrlsByRuleByXPath(Url,webSource, xPathRule,0);

                return Urls;
            }
            #endregion

            #region ��ȡ��ҳԴ��
            //������ַ��Դ�룬��������ȡ������ȡ��������ַ
            //string UrlSource= cTool.GetHtmlSource(Url1,true );
            string UrlSource = string.Empty;
            if (webSource == "")
            {
                throw new NetMinerException("��ҳΪ�գ������µ��ã�");
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

                //        //��ʼ����header
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
                //    throw new NetMinerException("��ȡ��ҳԴ�����������ҳ��ַ����ҳ���롢��ַ�����ʽ��������ϢΪ��" + ex.Message);
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

            #region �������ҵ���ѭ��������
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

            #region ��ȡ������ʽ��ƥ�䵼����ַ

            //�жϵ����������Ƿ���ڵ����������ǰ׺�ַ�����β�������ַ����Ĳ��������ϵͳû��
            //������Ϊ��

            #region  �ӹ�����
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
                #region ���ӵ�������������������������
                //���ӵĵ�������
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
                #region �򵥵���
                //�򵥹��򵼺��������˱�ǩ

                charSetMatch = Regex.Match(UrlRule, "(?<=<Trait>).*?(?=</Trait>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string trait = charSetMatch.Groups[0].ToString();

                sRule = @"(?<=[url"":""|url':'|href=|src=|open(][\W])" + ToolUtil.RegexReplaceTrans(trait) + @"(\S[^'"">]*)(?=[\s'""])";
                #endregion
            }
            else if (UrlRule.StartsWith("<ParaNavi>"))
            {
                //���´��뱻ע�ͣ�����ԭ���ķ����ڽ��в���ֵ��ȡ��ʱ��
                //��Ҫ����˳���������滻���޸Ĵ˷�����������һҳ�ķ�������
                //�����Ϳ��Բ��ð���˳���������滻�ˣ���ǿ�����
                //��������
                //�Ƚ����еĲ�����ȡ������ƴ��һ��������ʽ��Ȼ����в�����ȡ
                string paraReg = "(?<=<Para>).+?(?=</Para>)";

                Regex pReg = new Regex(paraReg, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //MatchCollection paras = pReg.Matches(UrlSource);
                MatchCollection paras = pReg.Matches(UrlRule);

                int groutID = 1;
                foreach (Match para in paras)
                {
                    //ϵͳĬ��������Ҫ�۸���ĸ��������ĸ�����ֵ�����
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

                //����ָû�ȥ�ˣ����ð���ԭ�з�ʽ���У����������⣬
                //1���޷�����ǰ����򣬶��������ʱ��
                //2���޷����ж�����ַ����
                //2014-3-10�޸ģ�������ԭ�в�������ƥ��ķ��������Բ�������ҳ˳����в�����ƥ��
                //while (Regex.IsMatch(UrlRule, "<Para>"))
                //{
                //    //����������滻��ƥ���Ľ��
                //    charSetMatch = Regex.Match(UrlRule, "(?<=<Para>).*?(?=</Para>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //    string strRegex = charSetMatch.Groups[0].ToString();

                //    charSetMatch = Regex.Match(UrlSource, strRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                //    string pValue = charSetMatch.Groups[0].ToString();

                //    //�滻
                //    UrlRule = UrlRule.Replace("<Para>" + strRegex + "</Para>", pValue);
                //}


                #region �����ֵ��ȡ������
                //�ڴ˴���ʱ�����ҳ���������
                bool isFormValue = false;
                Hashtable rFormValue = new Hashtable();

                int maxLoop = 0;
                while (Regex.IsMatch(UrlRule, "{FormValue}") && maxLoop<5000)
                {

                    maxLoop++;

                    #region ������·ҳ�����Ϣ
                    //����һ����·ҳ��ı�����ֵ
                    if (isFormValue == false)
                    {

                        //�Ȼ�ȡform
                        Regex reForm = new Regex("<input[^>]+?>", RegexOptions.IgnoreCase);
                        MatchCollection mcForm = reForm.Matches(UrlSource);
                        foreach (Match ma in mcForm)
                        {
                            //�ж�Type�Ƿ�Ϊhidden������ǣ�����ӵ�������
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

                //�Ӵ������ַ�л�ȡֵ
                while (Regex.IsMatch(UrlRule, "{UrlValue:.*?}"))
                {
                    charSetMatch = Regex.Match(UrlRule, "(?<={UrlValue:).*?(?=})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string urlPara = charSetMatch.Groups[0].ToString();

                    charSetMatch = Regex.Match(Url, urlPara, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string urlValue = charSetMatch.Groups[0].ToString();

                    //��url��Ҫ����ת��
                    urlValue = ToolUtil.ConvertEncodingUrl(urlValue);

                    //UrlRule = Regex.Replace(UrlRule, "{UrlValue:}", urlValue);
                    UrlRule = UrlRule.Replace("{UrlValue:" + urlPara + "}", urlValue);
                }

                //�ڴ˴���ű��������⣬�ű�����ͨ�������ִ��
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
                    
                    //����ȴ��߳̽׶�
                    ScriptWait.WaitOne();

                    UrlRule = Regex.Replace(UrlRule, "{Script:.*}", m_ScriptValue);
                }

                //������·ҳ�����ֵ
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
                        //���뵼��
                        string tmpCookie = string.Empty;
                        string imgName = NetMiner.Common.HttpSocket.cHttpSocket.GetImage(System.IO.Path.GetTempPath().ToString(), imgUrl, ref cookie);

                        //��ʼ��ȡ��֤��
                        NetMiner.Common.HttpSocket.cRunPlugin rPlugin = new NetMiner.Common.HttpSocket.cRunPlugin();

                        if (strPlugin != "")
                        {
                            vCode = rPlugin.CallVerifyCode(imgName, imgUrl, strPlugin);
                        }
                        rPlugin = null;


                    }
                    else
                    {
                        //�ֶ�����
                        NetMiner.Common.HttpSocket.frmInputVCode f = new NetMiner.Common.HttpSocket.frmInputVCode();
                        f.RVCode = GetVCode;
                        f.iniData(imgUrl, cookie);
                        if (f.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                        {
                            //ȡ����¼
                            cookie = "";
                        }
                        f.Dispose();

                        //���ͼƬ������Cookie����ʹ��ͼƬ��cookie
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

            #region ��������ƥ����ַ
             
             if (sRule != "")
             {
                 Regex re = new Regex(sRule, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                 MatchCollection aa = re.Matches(UrlSource);

                 foreach (Match ma in aa)
                 {
                     //Urls.Add(ma.Value.ToString());
                     //�ڴ˴������������������ַ�а����ո�����Ҫת����%20�����������google
                     //��ַ��ѯ�лᾭ������

                     string tUrl = "";

                     if (UrlRule.StartsWith("<ParaNavi>"))
                     {
                         //������ַ�����������Ҫ�Ǹ���url�����滻
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
                     //������ַ�����������Ҫ�Ǹ���url�����滻
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

            #region �ڴ˴���Header��Ҫ����·��ַ��ȡ���ݵ�ͷ��Ϣ
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

            #region �ڴ˴�����ִ���ж�
            if (d1.Rows.Count == 0)
             {
                 //��ʾû��ƥ�䵽������ַ����
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


             //����ʱ���ܻ��ȡ�ظ���ַ���б�������Ҫȥ��
            //ȥ���ظ���


            string[] strComuns = new string[d1.Columns.Count];

            for (int m = 0; m < d1.Columns.Count; m++)
            {
                strComuns[m] = d1.Columns[m].ColumnName;
            }

            DataView dv = new DataView(d1);

            DataTable d2 = dv.ToTable(true, strComuns);

            #region ����ƥ����������һ���ϸ����ַ
            for (int i = 0; i < d2.Rows.Count; i++)
            {

                string strUrl = d2.Rows[i][0].ToString();
                strUrl = EditUrl(strUrl, strPrefix, strSuffix, strIncludeWord, strNoIncludeWord, strOldValue, strNewValue, isRegex);
                if (strUrl!="")
                {
                    //ֻɾ��ͷ��β������
                    strUrl = Regex.Replace(strUrl, "^[\"|']", "");
                    strUrl = Regex.Replace(strUrl, "[\"|']$", "");

                    //��Ҫ����ת�壬��������ַ���������{ }����Ҫת��
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

                    //        //�ж�Url�Ƿ����POST������������ڣ�����ȥ��POST����
                    //        //Ȼ���ٽ�����ַ���㣬���������ַƥ��ʱ����
                    //        if (Regex.IsMatch(PreUrl, @"(?<=<POST[^>]*>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase))
                    //        {
                    //            PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("<POST"));
                    //        }

                    //        PreUrl = NetMiner.Common.ToolUtil.RelativeToAbsoluteUrl(Url, PreUrl);

                    //        //���ӵ�������̽��������ַ ��Ҫ���ӵ� ǰ׺�ͺ�׺�����Ϊ�գ�Ҳ����
                    //        string newUrl = PreUrl + strUrl;
                    //        //newUrl = newUrl.Replace("'", "");
                    //        //newUrl = newUrl.Replace("\"", "");
                    //        //ֻɾ��ͷ��β������
                    //        newUrl = Regex.Replace(newUrl, "^[\"|']", "");
                    //        newUrl = Regex.Replace(newUrl, "[\"|']$", "");

                    //        //��Ҫ����ת�壬��������ַ���������{ }����Ҫת��
                    //        newUrl = newUrl.Replace("{", "\\{");
                    //        newUrl = newUrl.Replace("}", "\\}");
                    //        newUrl = ToolUtil.ConvertJsonUrl(newUrl);
                    //        newUrl = ToolUtil.ConvertUnicodeUrl(newUrl);
                    //        Urls.Add(newUrl);
                    //    }
                    //    else
                    //    {
                    //        //���ӵ�������̽��������ַ ��Ҫ���ӵ� ǰ׺�ͺ�׺�����Ϊ�գ�Ҳ����

                    //        string newUrl = strUrl;
                    //        //newUrl = newUrl.Replace("'", "");
                    //        //newUrl = newUrl.Replace("\"", "");
                    //        //ֻɾ��ͷ��β������
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
        /// ���ݵ��������ȡ��ҳ��ַ���˷���ר�����ڴ�����ӻ�����
        /// </summary>
        /// <param name="NavUrl"></param>
        /// <param name="htmlSource"></param>
        /// <param name="UrlRule"></param>
        /// <returns></returns>
        public List<string> GetUrlsByRule(string NavUrl, string htmlSource, string UrlRule)
        {
            List<string> urls = GetXPathData(htmlSource, UrlRule);

            //ͨ������url��ַƥ�����
            for (int i = 0; i < urls.Count; i++)
            {
                string url = urls[i];
                Match s = Regex.Match(url, "(?<=href=)[^>]+?(?=[>|'|\"|\\s])", RegexOptions.IgnoreCase);
                url = s.Groups[0].Value.ToString();
                //url = url.Replace("'", "");
                //url = url.Replace("\"", "");
                //ֻɾ��ͷ��β������
                url = Regex.Replace(url, "^[\"|']", "");
                url = Regex.Replace(url, "[\"|']$", "");

                if (!url.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                {
                    string PreUrl = NavUrl;

                    url = NetMiner.Common.ToolUtil.RelativeToAbsoluteUrl(NavUrl, url);

                    //��Ҫ����ת�壬��������ַ���������{ }����Ҫת��
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
        /// ���ݵ��������ȡ��ҳ��ַ���˷���ר���ڿ��ӻ���������V5.5��ʼ�����ӻ����������ģʽ
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

                //��ȡ��һ�����ֲ���
                if (pageScrollIndex > 1)
                {
                    Match charSetMatch = Regex.Match(XPath, "{Num:\\d+,\\d+,\\d+}", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string numPara = charSetMatch.Groups[0].ToString();

                    //����ҳ������Ĵ��������½������ֲ�����ֵ�޸�
                    string s = numPara.Replace("{Num:", "").Replace("}", "");
                    string[] sss = s.Split(',');
                    if (sss.Length == 3)
                    {
                        //�ֽ���ȡ����ʼ����
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
        /// ����xPath��ȡ������ַ
        /// </summary>
        /// <param name="HtmlSource"></param>
        /// <param name="xPaths"></param>
        /// <returns></returns>
        private List<string> GetXPathData(string HtmlSource, string xPathExpression)
        {
            //����һ��doc
            //System.Windows.Forms.HtmlDocument htmlDoc = new System.Windows.Forms.HtmlDocument();
            //HtmlElement btnElement = htmlDoc.Body;

            //htmlMessage = btnElement.OuterHtml;

            List<string> urls = new List<string>();

            //ȡԴ������body�м�Ĳ���
            HtmlSource = new Regex(@"(?m)<script[^>]*>(\w|\W)*?</script[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(HtmlSource, "");

            //���Խ��и�ʽ����������Ҫ��Ϊ�����Ʊ�ǩ
            HtmlSource = HtmlExtract.Utility.HtmlHelper.HtmlFormat (HtmlSource);
            
            Match s = Regex.Match(HtmlSource, @"<body[\S\s]*</body>", RegexOptions.IgnoreCase);

            HtmlSource = s.Groups[0].Value.ToString();

            HtmlAgilityPack.HtmlDocument hDoc = new HtmlAgilityPack.HtmlDocument();
            hDoc.LoadHtml("<Html>" + HtmlSource + "</Html>");

            //�ֽ�xPath����ΪxPath������һ���������б�
            cUrlParse u = new cUrlParse(this.m_workPath);
            List<string> xExpressions = u.SplitWebUrl(xPathExpression);
            u = null;


            for (int l = 0; l < xExpressions.Count; l++)
            {
                //��ʼ��������
                HtmlAgilityPack.HtmlNodeCollection ss = hDoc.DocumentNode.SelectNodes(xExpressions[l].ToString());

                if (ss != null)
                {
                    foreach (HtmlAgilityPack.HtmlNode hNode in ss)
                    {
                        string strxPahtvalue = "";

                        //�����ǵ�����ַ�����Ա����ȡouterhtml���������ӵ�ַ
                        strxPahtvalue = hNode.OuterHtml;
                        urls.Add(strxPahtvalue);
                        break;
                    }
                }
            }

            return urls;
        }

        //�����ַ
        /// <summary>
        /// ������ַ�ṩ�Ĳ�������ַ���зֽ⴦��,�����˶�����ַ�Ĵ�����;�ָ�
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
        //        #region ���ݿ���ȡ��ַ����

        //        //��ȡdburl����
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
        //            //�������ݿ���ã��ϳ�Url
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

        //    //�ڴ����ж��Ƿ�Ϊdb��ȡ��ַ�Ĳ���
        //    //��5.41�汾��ʼ�����ݿ����Ϊ��̬����
           
          

        //        #region �˲��ַֽ�url����
        //        if (Url.IndexOf("{Syn:") > -1)
        //            isSynPara = true;

        //        //����ת����ŵ�ʶ�������ת���������������{}����ת���Ϊ\
        //        //if (!Regex.IsMatch(Url, "{.*}"))
        //        //�ж��Ƿ��в��������û�в�������ֱ�ӷ���
        //        if (!Regex.IsMatch(Url, "[^\\\\]{.*[^\\\\]}",RegexOptions.Multiline ))
        //        {
        //            if (Regex.IsMatch(Url, "\\\\{"))
        //            {
        //                //������ת����ţ�����
        //                Url = Url.Replace("\\{", "{");
        //                Url = Url.Replace("\\}", "}");
        //            }

        //            tmp_list_Url.Add(Url);
        //            return tmp_list_Url;
        //        }

        //        //����tmp_list_Url�ĳ�ʼֵ
        //        //��ʼֵΪUrl��һ������ǰ�����ַ�
        //        //Ӧ����{Ϊ׼
        //        //ͨ�������ȡ���ı���ǰ�ĵĻ�ȡ��ʽ����������д�����ת������
        //        string strPreMatch = "[\\s\\S]+?[^\\\\](?={)";
        //        Match urlPre = Regex.Match(Url, strPreMatch, RegexOptions.IgnoreCase );
        //        string strUrlPre = urlPre.Groups[0].Value;
        //        tmp_list_Url.Add(strUrlPre);

        //        int first = 0;
        //        while (Regex.IsMatch(Url, "[^\\\\]?{.*[^\\\\]?}"))
        //        {
        //            //��ȡ��������
        //            //string strMatch = "(?<={)[^}]*(?=})";
        //            //����ת����ŵ�ʶ�������ת���������������{}����ת���Ϊ\
        //            //��ȡ����
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

        //            //�ж��Ƿ��в���������У����ȡ�м䲿����ƴ����ַ
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

        //        #region ����ͬ������
        //        if (isSynPara == true)
        //        {
        //            //��ȡһ��ʵ����ַ�������ж��Ƿ���ͬ������

        //            //����ͬ������
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

        //�жϵ�ǰ��������������,���а����ֵ������
        //public int GetUrlCount(string Url)
        //{
        //    if (Url == "")
        //        return 0;

        //    int UrlCount = 1;
        //    int SumUrlCount = 0;
        //    List<string> g_Url = null;

        //    string[] sUrls = null; 

        //    //�ڴ˴������post�����У��п��ܴ��ڻ��е�����������Ҫ�ж�
        //    //����һ����ַ�������зֳɶ����ַ��������ڴ����������һ����ַ����
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

        //        #region �������ݿ���ַ
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
        //                    //��ȡtop���������
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
        //                    //��sqlת����Count��*��
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


        //        #region  �����������
        //        //����ת����ŵ�ʶ�������ת���������������{}����ת���Ϊ\
        //        //if (!Regex.IsMatch(Url, "{.*}"))
        //        if (!Regex.IsMatch(Url, "[^\\\\]{.*[^\\\\]}"))
        //            {
        //                if (Regex.IsMatch(Url, "\\\\{"))
        //                {
        //                    //������ת����ţ�����
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
        //                    //��ȡ��������
        //                    //string strMatch = "(?<={)[^}]*(?=})";
        //                    //����ת����ŵ�ʶ�������ת���������������{}����ת���Ϊ\
        //                    string strMatch = "(?<={)[^{]*[^\\\\](?=})";
        //                    Match s = Regex.Match(Url, strMatch, RegexOptions.IgnoreCase);
        //                    string UrlPara = s.Groups[0].Value;

        //                    //��Ϊ��������ַ�����������Բ���Ҫ��url���б���ת�����������Ҫת���Ļ���Ҳ����Ҫ
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
        /// �ֽ�ָ���Ĳ�������ҵ���߼��޹�
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
        //                //��ȡ�����ڵĸ�ʽ
        //                string dateFormat = dicPre.Substring(dicPre.IndexOf("(") + 1, dicPre.IndexOf(")") - dicPre.IndexOf("(") - 1);

        //                //��ȡ���ڵķ�Χ
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
        //                        //��ʾΪ�������ֵ�ô���
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
        //                            //�������в���
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

        //    if (firstIndex > endI)  //��ʶѭ���Ѿ�������ָ�������Χ
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

                    if (firstIndex > endI)  //��ʶѭ���Ѿ�������ָ�������Χ
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

                    if (firstIndex > endI)  //��ʶѭ���Ѿ�������ָ�������Χ
                        return "";

                    dValue = 0;

                    //��ʼ����ѭ������
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

            //�ȶ�UrlRule����ת��
            UrlRule = ToolUtil.CutRegexWildcard(UrlRule);

            string strNext = "";
            //��Ĭ��post����ķ�ʽ
            string postPre = "<POST:ASCII>";

            if (UrlRule == "")
                return "";

            //�ȴ���ǰ��ַ����
            if (UrlRule.IndexOf("{CurrtentPageUrl}")>-1)
            {
                UrlRule = UrlRule.Replace("{CurrtentPageUrl}", Url);
            }

            if (UrlSource == null || UrlSource == "")
            {
                throw new NetMinerException("��ҳΪ�գ������µ��ã�");
            }
                //{
                //    #region  ��ȡԴ��
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

                //            //��ʼ����header
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
                //        throw new NetMinerException("��ȡ��ҳԴ�����������ҳ��ַ����ҳ���롢��ַ�����ʽ��������ϢΪ��" + ex.Message);
                //    }
                //    gW = null;

                //    #endregion
                //}

                //�ڴ��ж��Ƿ�Ϊ���ӻ�����������ǣ�����ÿ��ӻ�����
                if (UrlRule.StartsWith("<XPath>"))
            {
                Match charSetMatch1 = Regex.Match(UrlRule, "(?<=<XPath>).*?(?=</XPath>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string xPathRule = charSetMatch1.Groups[0].ToString();


                List<string> Urls = GetUrlsByRuleByXPath(Url, UrlSource, xPathRule,0);
                if (Urls != null && Urls.Count > 0)
                    strNext = Urls[0].ToString();

                return strNext;
            }

            #region  �ӹ�����
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

            //�������
            charSetMatch = Regex.Match(UrlRule, "(?<=<Parameter>).*?(?=</Parameter>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string nextPara = charSetMatch.Groups[0].ToString();
            UrlRule = Regex.Replace(UrlRule, "(<Parameter>).*?(</Parameter>)", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            string sRule = "";

            if (nextPara == "")
            {
                if (UrlRule.StartsWith("<Common>") || UrlRule.StartsWith("<Regex>"))
                {
                    //���ӵĵ�������

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
                    //�򵥹��򵼺��������˱�ǩ

                    charSetMatch = Regex.Match(UrlRule, "(?<=<Trait>).*?(?=</Trait>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string trait = charSetMatch.Groups[0].ToString();

                    sRule = RegexNextPage + "(?=" + ToolUtil.RegexReplaceTrans(trait) + ")";
                }

                #region ƥ����һҳ������
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
                //�ڴ˴������������
                if (nextPara != "")
                {
                    #region �����ֵ
                    bool isFormValue = false;
                    Hashtable rFormValue = new Hashtable();
                    while (Regex.IsMatch(nextPara, "{FormValue}"))
                    {

                        #region ������·ҳ�����Ϣ
                        //����һ����·ҳ��ı�����ֵ
                        if (isFormValue == false)
                        {

                            //�Ȼ�ȡform
                            Regex reForm = new Regex("<input[^>]+?>", RegexOptions.IgnoreCase);
                            MatchCollection mcForm = reForm.Matches(UrlSource);
                            foreach (Match ma in mcForm)
                            {
                                //�ж�Type�Ƿ�Ϊhidden������ǣ�����ӵ�������
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

                                //������������ַ���������봦����Ϊ�ٴ�����ʱ��ϵͳ�ᴦ����������
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

                    //�Ӵ������ַ�л�ȡֵ
                    while (Regex.IsMatch(nextPara, "{UrlValue:.*?}"))
                    {
                        charSetMatch = Regex.Match(nextPara, "(?<={UrlValue:).*?(?=})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string urlPara = charSetMatch.Groups[0].ToString();

                        charSetMatch = Regex.Match(Url, urlPara, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string urlValue = charSetMatch.Groups[0].ToString();

                        //nextPara = Regex.Replace(nextPara, "{UrlValue:.*?}", urlValue);
                        nextPara = nextPara.Replace("{UrlValue:" + urlPara + "}", urlValue);
                    }

                    //������·ҳ�����ֵ
                    //2016��3��22�� ��UrlRuleF������nextPara
                    while (Regex.IsMatch(nextPara, "{FromValue:.*?}"))
                    {
                        charSetMatch = Regex.Match(nextPara, "(?<={FromValue:).*?(?=})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string urlPara = charSetMatch.Groups[0].ToString();

                        charSetMatch = Regex.Match(UrlSource, urlPara, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string urlValue = charSetMatch.Groups[0].ToString();

                        //UrlRule = Regex.Replace(UrlRule, "{FromValue:}", urlValue);
                        nextPara = nextPara.Replace("{FromValue:" + urlPara + "}", urlValue);
                    }

                    //���ж��Ƿ��в���������
                    //�˴���2013-9-13�����޸ģ������˶�����Ĵ���
                    while (Regex.IsMatch(nextPara, "<Para>"))
                    {
                        //����������滻��ƥ���Ľ��
                        charSetMatch = Regex.Match(nextPara, "(?<=<Para>).*?(?=</Para>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string strRegex = charSetMatch.Groups[0].ToString();

                        charSetMatch = Regex.Match(UrlSource, strRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string pValue = charSetMatch.Groups[0].ToString();

                        //���ñ��봦����Ϊ�ٴ����󻹻���б���
                        //if (isUrlCode == true)
                        //{
                        //    pValue = ToolUtil.UrlParaEncode(pValue, UrlCode);
                        //}

                        //�滻
                        nextPara = nextPara.Replace("<Para>" + strRegex + "</Para>", pValue);
                    }

                    //�ڴ˴���ҳ��������������
                    if (Regex.IsMatch(nextPara, "{Formula:.*?}"))
                    {
                        charSetMatch = Regex.Match(nextPara, "(?<={Formula:).*?(?=})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string urlPara = charSetMatch.Groups[0].ToString();

                        //��ʼ��ȡ��ʽǰ�������
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

                            //�滻
                            nextPara = Regex.Replace(nextPara, "\\d*(?:{Formula:.*})", cutPage.ToString ());
                        }
                    }


                    strNext = nextPara;

                    if (Regex.IsMatch(nextPara, "{.*}"))
                    {
                        //��ȡ��������
                        string strMatch = "(?<={)[^}]*(?=})";
                        Match s = Regex.Match(nextPara, strMatch, RegexOptions.IgnoreCase);
                        string UrlPara = s.Groups[0].Value;

                        strNext = GetNextPara(strNext, UrlPara, nextPara, ref firstIndex); //,IsUrlEncode ,UrlEncode 

                        

                        //�ж��Ƿ���ͬ������������н��н���
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

            //�ڴ˴�����ַ�ļӹ�
            //if (!string.IsNullOrEmpty (strNewValue ))

            //strNext = strNext.Replace("'", "");
            //strNext = strNext.Replace("\"", "");
            //ֻɾ��ͷ��β������
            strNext = Regex.Replace(strNext, "^[\"|']", "");
            strNext = Regex.Replace(strNext, "[\"|']$", "");

        
            strNext = EditUrl(strNext, strPrefix, strSuffix, strIncludeWord, strNoIncludeWord, strOldValue, strNewValue, isRegex);

            #region  �ж���һҳ��ȡ�ĵ�ַ�ĺϷ���ʽ
            if (strNext != "" && strNext != Url && strNext != "#")
            {
                //�жϻ�ȡ�ĵ�ַ�Ƿ�Ϊ��Ե�ַ
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
        /// ����ǰ��׺ �ַ����滻�Ȳ���������ַ�ļӹ�����
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

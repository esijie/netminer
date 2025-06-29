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

///���ܣ��ɼ����ݴ���
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
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

            //Ĭ�����Դ���Ϊ3���Һ���404����
            //m_IsIgnore404 = true;
            //m_AgainNumber = 3;

            //��ʼ����ʱ����Ҫ�жϣ���ǰ�����Ƿ�����˴���
            //��������˴�������ش�����Ϣ
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

        #region ����
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

        //��¼ÿ��ͨѶ���غ��cookie���������վ��cookie�Ƕ�̬�仯�����´�����
        //���ո��º��cookie����ͨѶ
        private string m_UCookie;
        public string UCookie
        {
            get { return m_UCookie; }
            set { m_UCookie = value; }
        }

        //ֻҪ��ֵΪtrue���Ͳ����Զ���ͷ
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
        /// ��ȡָ����ַԴ��
        /// </summary>
        /// <param name="url">��ַ</param>
        /// <param name="webCode">��ҳ����</param>
        /// <param name="cookie">��ҳcookie</param>
        /// <param name="startPos">��ȡ��ҳԴ�����ʼλ��</param>
        /// <param name="endPos">��ȡ��ҳԴ�����ֹλ��</param>
        /// <param name="IsCutnr">�Ƿ��ȡ�س����з���Ĭ��Ϊtrue����ȡ</param>
        /// <param name="IsAjax">�Ƿ�ΪAjaxҳ��</param>
        /// <returns></returns>

        public string GetHtml(string url, cGlobalParas.WebCode webCode, string cookie, string startPos, string endPos,bool IsCutnr,bool IsAjax)   
        {
            //�ж���ҳ����
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
            
            //�ж��Ƿ���Ҫ����
            if (m_IsProxy == true)
                wReq.Proxy = m_wProxy;

            #region cookie

            //�ж��Ƿ���cookie
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

            #region POST����

            //�ж��Ƿ���POST����
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

            //����ҳ�泬ʱʱ��Ϊ12��
            wReq.Timeout = 12000;


            #region  ��ʼ�������

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

            //ȥ���س����з���
            if (IsCutnr == true)
            {
                strWebData = Regex.Replace(strWebData, "([\\r\\n])[\\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strWebData = Regex.Replace(strWebData, "\\n", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strWebData.Replace("\\r\\n", "");
            }



            //��ȡ��ҳ��ı����ʽ,����Դ�����һ���ж�,�����û��Ƿ�ָ������ҳ����
            //Match charSetMatch = Regex.Match(strWebData, "<meta([^<]*)charset=([^<]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //string webCharSet = charSetMatch.Groups[2].Value;
            //string charSet = webCharSet;

            ///ȥ������ת�����****************************************************************************************
            //if (charSet != null && charSet != "" && Encoding.GetEncoding(charSet) != wCode)
            //{
            //    byte[] myDataBuffer;

            //    myDataBuffer = System.Text.Encoding.GetEncoding(charSet).GetBytes(strWebData);
            //    strWebData = Encoding.GetEncoding(charSet).GetString(myDataBuffer);

            //}
            ///ȥ������ת�����****************************************************************************************

            //���ս�ȡ��ҳ����ʼ��־����ֹ��־���н�ȡ
            //�����ʼ����ֹ��ȡ��ʶ��һ��Ϊ�գ��򲻽��н�ȡ
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
        /// �ɼ���ҳ����
        /// </summary>
        /// <param name="Url">��ҳ��ַ</param>
        /// <param name="StartPos">��ʼ�ɼ�λ��</param>
        /// <param name="EndPos">��ֹ�ɼ�λ��</param>
        /// <returns></returns>
        public DataTable  GetGatherData(string Url,cGlobalParas.WebCode webCode, string cookie, string startPos,string endPos,string sPath,bool IsAjax,bool IsExportGUrl,bool IsExportGDateTime)
        {
            tempData = new DataTable("tempData");
            int i ;
            int j;
            string strCut="";
            bool IsDownloadFile = false;

            #region ������ṹ����������ȡ����

            //����ҳ���ȡ�ı�־������ṹ
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
            
            //�����û�ָ����ҳ���ȡλ�ù���������ʽ
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

            #region ��ȡ��ҳԴ��

            int rowCount = this.CutFlag.Count;

            //ȥ�����һ����|��
            strCut = strCut.Substring(0, strCut.Length - 1);

            //��ȡ��ҳ��Ϣ
            //�жϴ����Url�Ƿ���ȷ���������ȷ���򷵻ؿ�����
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

            //��ʼ��ȡ��ȡ����
            Regex re = new Regex(@strCut, RegexOptions.IgnoreCase | RegexOptions.Multiline );
            MatchCollection mc = re.Matches(this.WebpageSource);

            if (mc.Count == 0)
            {
                tempData = null;
                return tempData;
            }

            DataRow drNew=null ;

            i = 0;

            #region ��ʼ�����ȡ�ַ�����ƴ��һ����

            //��ʼ���ݲɼ������ݹ������ݱ�������
            //�ڴ���Ҫ����ɼ������п��ܴ��е�����
            //���汻ע�͵Ĵ���������������ݱ�Ĵ��룬������ִ�������

            //Match ma;
            
            int rows = 0; //ͳ�ƹ��ɼ��˶�����
            int m = 0;   //����ʹ��

            try
             {

                while (m < mc.Count)
                {
                    //�½�����
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
                                        //�˳�����ѭ��
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
                                        //���ɼ�ʱ������ȱ�ٲɼ����ݣ����ô˷������вɼ����ݲ���
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

            #region ��ʼ����������ƣ����л�ȡ���ݼӹ�

        ExitWhile:

            //�ڴ��ж��Ƿ���Ҫ�����ʱ�������ݵ�����,��������汾1.2�������������������

            for (i = 0; i < this.CutFlag.Count; i++)
            {
                for (j = 0; j< this.CutFlag[i].ExportRules.Count; j++)
                {
                    switch (this.CutFlag[i].ExportRules[j].FieldRuleType)
                    {
                        case (int)cGlobalParas.ExportLimit.ExportNoLimit:

                            break;

                        //�Ƚ���ɾ������
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
                                //�滻�ĸ�ʽ�ǣ�<OldValue:><NewValue:>�����������жϱ����ַ������ȴ���22
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
                                //�滻�ĸ�ʽ�ǣ�<OldValue:><NewValue:>�����������жϱ����ַ������ȴ���22
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
                                    string strValue = mUni.Result("${code}");   //����
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

            #region ��Բɼ���Ҫ�����ļ����ֶν����ļ����ش���
            //�ж��Ƿ�����������ļ�����������У���ʼ���أ���Ϊ�˹���������������ͼƬʹ��
            //������ר�õ����ع��ߣ����Զ����ش���û�е��������̴߳���
     
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

                                //��ʼ��ȡ�����ļ�����
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

        //�����ļ�����һ�����̵߳ķ�����������С�ļ����أ���֧��http��ʽ
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

        //��ȡ�ɼ����ݵĴ��ı�
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using NetMiner.Resource;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Collections;
using System.Net.Sockets;
using HtmlExtract.Utility;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;


///���ܣ�����С���� ��̬
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺����ϵͳ���Ʋ���
///�����ƻ�������
///˵������
///�汾��01.10.00
///�޶�����
namespace NetMiner.Common
{
    public static class ToolUtil
    {

        private const int INTERNET_CONNECTION_MODEM =1;
        private const int INTERNET_CONNECTION_LAN= 2;
        private const int INTERNET_CONNECTION_PROXY=4;
        private const int INTERNET_CONNECTION_MODEM_BUSY = 8;

        /// <summary>
        /// ��֧��get��ʽ����֧��POST
        /// </summary>
        /// <param name="url">ָ����Url��ַ</param>
        /// <param name="Isbool">�Ƿ�ȥ����ҳԴ���еĻس����з���true ȥ��</param>
        /// <returns></returns>
        static public string GetHtmlSource(string url, bool Isbool)
        {
            if (url == "")
                return "";

            string charSet = "";
            WebClient myWebClient = new WebClient();


            //��ȡ���������ڶ��� Internet ��Դ��������������֤������ƾ�ݡ�   
            myWebClient.Credentials = CredentialCache.DefaultCredentials;


            byte[] myDataBuffer;
            string strWebData;

            try
            {
                //����Դ�������ݲ������ֽ����顣����@����Ϊ��ַ�м���"/"���ţ�   
                myDataBuffer = myWebClient.DownloadData(@url);
                strWebData = Encoding.Default.GetString(myDataBuffer);
            }
            catch (System.Net.WebException)
            {
                return "";
            }

            //��ȡ��ҳ��ı����ʽ
            Match charSetMatch = Regex.Match(strWebData, "<meta([^<]*)charset=([^<]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string webCharSet = charSetMatch.Groups[2].Value;
            if (charSet == null || charSet == "")
                charSet = webCharSet;

            if (charSet != null && charSet != "" && Encoding.GetEncoding(charSet) != Encoding.Default)
                strWebData = Encoding.GetEncoding(charSet).GetString(myDataBuffer);

            if (Isbool == true)
            {
                strWebData = Regex.Replace(strWebData, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strWebData.Replace(@"\r\n", "");
            }

            return strWebData;

        }

        /// <summary>
        /// ��ȡ��ҳԴ�룬��֧��POST����
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postString"></param>
        /// <returns></returns>
        public static string GetHtmlSource(string url, string postString)
        {
            if (url == "")
                return "";

            string charSet = "";
            WebClient myWebClient = new WebClient();
            myWebClient.Proxy = null;

            //��ȡ���������ڶ��� Internet ��Դ��������������֤������ƾ�ݡ�   
            myWebClient.Credentials = CredentialCache.DefaultCredentials;


            byte[] myDataBuffer;
            string strWebData;

            try
            {
                //����Դ�������ݲ������ֽ����顣����@����Ϊ��ַ�м���"/"���ţ�   

                if (!string.IsNullOrEmpty(postString))
                {
                    byte[] postData = Encoding.ASCII.GetBytes(postString);
                    myWebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    strWebData = myWebClient.UploadString(@url, "POST", postString);
                }
                else
                {
                    myDataBuffer = myWebClient.DownloadData(@url);

                    strWebData = Encoding.UTF8.GetString(myDataBuffer);
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //��ȡ��ҳ��ı����ʽ
            //Match charSetMatch = Regex.Match(strWebData, "<meta([^<]*)charset=([^<]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //string webCharSet = charSetMatch.Groups[2].Value;
            //if (charSet == null || charSet == "")
            //    charSet = webCharSet;

            //if (charSet != null && charSet != "" && Encoding.GetEncoding(charSet) != Encoding.Default)
            //    strWebData = Encoding.GetEncoding(charSet).GetString(myDataBuffer);

            return strWebData;

        }

        //ȥ���ַ����Ļس����з���
        static public string ClearFlag(string str)
        {
            str = Regex.Replace(str, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            str.Replace(@"\r\n", "");

            return str;
        }

        //����ָ����ַ�жϵ�ǰҳ��ı���
        static public string GetWebpageCode(string url,cGlobalParas.WebCode WebCode)
        {
            string charSet = "";

            WebClient myWebClient = new WebClient();    

            myWebClient.Credentials = CredentialCache.DefaultCredentials;

            //����Դ�������ݲ������ֽ����顣����@����Ϊ��ַ�м���"/"���ţ� 
            byte[] myDataBuffer = myWebClient.DownloadData(url);
            string strWebData = Encoding.Default.GetString(myDataBuffer);

            //��ȡ��ҳ�ַ�����������Ϣ 
            Match charSetMatch = Regex.Match(strWebData, "<meta([^<]*)charset=([^<]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string webCharSet = charSetMatch.Groups[2].Value;
            if (charSet == null || charSet == "")
                charSet = webCharSet;
            
            return charSet;

        }

        static private int CountUrl(string WebUrl)
        {
            int Count = 0;
            Match Para = Regex.Match(WebUrl, "{.*}");
            
            return Count;
        }

        //����д��ĸת��Сд��ĸ
        static public string LetterToLower(string str)
        {
            if (str == "" || str == null)
                return "";

            string lowerStr="";
            char c;

            for (int i=0;i<str.Length ;i++)
            {
                c =char .Parse ( str.Substring(i, 1));

                if (Char.IsUpper(c))
                {
                    c = Char.ToLower(c);
                }
                lowerStr += c;
            }

            return lowerStr;
           
        }

        //���ַ����е��ַ���ת��ģ������滻
        //����滻����е�ͷ�Σ���������⻹����λ����֪���Ƿ����һ�ΰ��շ�����������ʽ��Ӧ�����滻
        //������޸Ĵ��࣬����дʵ���ǲ����ѣ��Ǻ�
        static public string ReplaceTrans(string str)
        {
            if (str == "" || str==null )
                return "";

            byte[] b1 = { 0x01 };
            string str1 = Encoding.ASCII.GetString(b1);

            string conStr="";
            if (Regex.IsMatch(str, "['\"<>&]"))
            {
                Regex re = new Regex("&", RegexOptions.IgnoreCase);
                str = re.Replace(str, "&amp;");
                re = null;

                re = new Regex("<", RegexOptions.IgnoreCase);
                str = re.Replace(str, "&lt;");
                re = null;

                re = new Regex(">", RegexOptions.IgnoreCase);
                str = re.Replace(str, "&gt;");
                re = null;

                re = new Regex("'", RegexOptions.IgnoreCase);
                str = re.Replace(str, "&apos;");
                re = null;

                re = new Regex("\"", RegexOptions.IgnoreCase);
                str = re.Replace(str, "&quot;");
                re = null;
                conStr = str;

                re = new Regex(str1, RegexOptions.IgnoreCase);
                str = re.Replace(str, "0x01");
                re = null;
                conStr = str;

            }
            else
            {
                conStr = str;
            }
            return conStr;
        }

        static public string ShowTrans(string str)
        {
            Regex re = new Regex("&amp;", RegexOptions.IgnoreCase);
            str = re.Replace(str, "&");
            re = null;

            re = new Regex("&lt;", RegexOptions.IgnoreCase);
            str = re.Replace(str, "<");
            re = null;

            re = new Regex("&gt;", RegexOptions.IgnoreCase);
            str = re.Replace(str, ">");
            re = null;

            re = new Regex("&apos;", RegexOptions.IgnoreCase);
            str = re.Replace(str, "'");
            re = null;

            re = new Regex("&quot;", RegexOptions.IgnoreCase);
            str = re.Replace(str, "\"");
            re = null;

            byte[] b1 = { 0x01 };
            string str1 = Encoding.ASCII.GetString(b1);

            re = new Regex("0x01", RegexOptions.IgnoreCase);
            str = re.Replace(str, str1);
            re = null;

            string conStr = str;

            return conStr;

        }

        //������ʽת�壬ϵͳ���Զ�ȥ��<Wildcard></Wildcard>��ʶ
        static public string RegexReplaceTrans(string str)
        {
            if (str == "" || str == null)
                return "";

            string conStr = "";

            if (Regex.IsMatch(str, "<Wildcard>"))
            {
                string[] strSS = Regex.Split(str, "</Wildcard>");
                for (int i = 0; i < strSS.Length; i++)
                {
                    string ss = strSS[i];
                    if (Regex.IsMatch(ss, "<Wildcard>"))
                    {
                        string strPre = Regex.Escape(ss.Substring(0, ss.IndexOf("<Wildcard>")));
                        string strWildcard = ss.Substring(ss.IndexOf("<Wildcard>") + 10, ss.Length - ss.IndexOf("<Wildcard>") - 10);
                        conStr += strPre + strWildcard;
                    }
                    else
                    {
                        conStr += Regex.Escape(ss);
                    }
                }
            }
            else
            {
                conStr = Regex.Escape(str);
            }

            return conStr;

        }

        static public string RegexUnescape(string str)
        {
            if (str == "" || str == null)
                return "";

            string conStr = "";

            if (Regex.IsMatch(str, "<Wildcard>"))
            {
                string[] strSS = Regex.Split(str, "</Wildcard>");
                for (int i = 0; i < strSS.Length; i++)
                {
                    string ss = strSS[i];
                    if (Regex.IsMatch(ss, "<Wildcard>"))
                    {
                        string strPre = Regex.Unescape(ss.Substring(0, ss.IndexOf("<Wildcard>")));
                        string strWildcard = ss.Substring(ss.IndexOf("<Wildcard>") + 10, ss.Length - ss.IndexOf("<Wildcard>") - 10);
                        conStr += strPre + "<Wildcard>" + strWildcard + "</Wildcard>";
                    }
                    else
                    {
                        conStr += Regex.Unescape(ss);
                    }
                }
            }
            else
            {
                conStr = Regex.Unescape(str);
            }

            return conStr;
        }

        static public string CutRegexWildcard(string str)
        {
            string conStr = "";
            if (Regex.IsMatch(str, "<Wildcard>"))
            {
                string[] strSS = Regex.Split(str, "</Wildcard>");
                for (int i = 0; i < strSS.Length; i++)
                {
                    string ss = strSS[i];
                    if (Regex.IsMatch(ss, "<Wildcard>"))
                    {
                        string strPre = ss.Substring(0, ss.IndexOf("<Wildcard>"));
                        string strWildcard = ss.Substring(ss.IndexOf("<Wildcard>") + 10, ss.Length - ss.IndexOf("<Wildcard>") - 10);
                        conStr += strPre + strWildcard;
                    }
                    else
                    {
                        conStr += ss;
                    }
                }

            }
            else
                conStr = str;

            return conStr;
        }

        //���ڽ��ַ���ת����UTF-8����
        static public string ToUtf8(string str)
        {
            if (str == null)
            {
                return string.Empty;
            }
            else
            {
                char[] hexDigits = {  '0', '1', '2', '3', '4','5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

                Encoding utf8 = Encoding.UTF8;
                StringBuilder result = new StringBuilder();

                for (int i = 0; i < str.Length; i++)
                {
                    string sub = str.Substring(i, 1);
                    byte[] bytes = utf8.GetBytes(sub);

                    for (int j = 0; j < bytes.Length; j++)
                    {
                        result.Append("%" + hexDigits[bytes[j] >> 4] + hexDigits[bytes[j] & 0XF]);
                    }
                }

                return result.ToString();
            }
        }

        //��UTF-8����ת�����ַ���
        static public string FromUtf8(string str)
        {
            char[] hexDigits = {  '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
            List<byte> byteList = new List<byte>(str.Length / 3);

            if (str != null)
            {
                List<string> strList = new List<string>();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < str.Length; ++i)
                {
                    if (str[i] == '%')
                    {
                        strList.Add(str.Substring(i, 3));
                    }
                }

                foreach (string tempStr in strList)
                {
                    int num = 0;
                    int temp = 0;
                    for (int j = 0; j < hexDigits.Length; ++j)
                    {
                        if (hexDigits[j].Equals(tempStr[1]))
                        {
                            temp = j;
                            num = temp << 4;
                        }
                    }

                    for (int j = 0; j < hexDigits.Length; ++j)
                    {
                        if (hexDigits[j].Equals(tempStr[2]))
                        {
                            num += j;
                        }
                    }

                    byteList.Add((byte)num);
                }
            }

            return Encoding.UTF8.GetString(byteList.ToArray());
        }

        static public string UTF8ToGB2312(string str)
        {
            try
            {
                Encoding utf8 = Encoding.GetEncoding(65001);
                Encoding gb2312 = Encoding.GetEncoding("gb2312");//Encoding.Default ,936
                byte[] temp = utf8.GetBytes(str);
                byte[] temp1 = Encoding.Convert(utf8, gb2312, temp);
                string result = gb2312.GetString(temp1);
                return result;
            }
            catch (Exception)//(UnsupportedEncodingException ex)
            {
                return null;
            }
        }

        /// <summary>
        /// �������������󹤵����·��
        /// </summary>
        /// <param name="workPath"></param>
        /// <param name="absPath"></param>
        /// <returns></returns>
        static public string GetRelativePath(string workPath, string absPath)
        {
            string mainDir = workPath;

            //if (!mainDir.EndsWith("\\"))
            //{
            //    mainDir += "\\";
            //}

            int intIndex = -1, intPos = mainDir.IndexOf('\\');

            while (intPos >= 0)
            {
                intPos++;
                if (string.Compare(mainDir, 0, absPath, 0, intPos, true) != 0) break;
                intIndex = intPos;
                intPos = mainDir.IndexOf('\\', intPos);
            }

            if (intIndex >= 0)
            {
                absPath = absPath.Substring(intIndex);
                intPos = mainDir.IndexOf("\\", intIndex);
                while (intPos >= 0)
                {
                    absPath = "..\\" + absPath;
                    intPos = mainDir.IndexOf("\\", intPos + 1);
                }
            }

            return absPath;
        }

        //��Url���Ĳ��ֽ��б��룬���ر�����Url��
        //ע�⣺ֻ�����Ľ��б���
        static public string UrlEncode(string Url, cGlobalParas.WebCode uEncoding)
        {
            string DemoUrl = Url;

            //Regex re = new Regex("[\\u4e00-\\u9fa5]", RegexOptions.None);
            //MatchCollection mc = re.Matches(DemoUrl);

            //��ȡ��ַ������
            Regex re = new Regex("[^\\x00-\\xff]", RegexOptions.None);
            MatchCollection mc = re.Matches(DemoUrl);

            switch (uEncoding)
            {
                case cGlobalParas.WebCode.utf8:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.UTF8));
                    }
                    break;
                case cGlobalParas.WebCode.gb2312:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.GetEncoding("gb2312")));
                    }
                    break;
                case cGlobalParas.WebCode.gbk:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.GetEncoding("gbk")));
                    }
                    break;
                case cGlobalParas.WebCode.big5:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.GetEncoding("big5")));
                    }
                    break;
                case cGlobalParas.WebCode.Unicode:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncodeUnicode(ma.Value.ToString()));
                    }
                    break;
                default:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.UTF8));
                    }
                    break;
            }

            //��ȡ��ַ����
            //re = new Regex(".+?=.*?(?=[<|&])", RegexOptions.None);
            //mc = re.Matches(DemoUrl);
            //string NewUrl = string.Empty;

            //if (mc.Count > 0)
            //{
            //    foreach (Match ma in mc)
            //    {
            //        string ss = ma.Value.ToString();
            //        string s1 = string.Empty;
            //        string s2 = ss.Substring(ss.IndexOf("=") + 1, ss.Length - ss.IndexOf("=") - 1);

            //        if (s2.Length > 0)
            //        {
            //            switch (uEncoding)
            //            {
            //                case cGlobalParas.WebCode.utf8:
            //                    //s1 = HttpUtility.UrlEncode(s2, Encoding.UTF8);
            //                    s1 = HttpUtility.UrlEncodeUnicode(s2);
            //                    break;
            //                case cGlobalParas.WebCode.gb2312:
            //                    s1 = HttpUtility.UrlEncode(s2, Encoding.GetEncoding("gb2312"));
            //                    break;
            //                case cGlobalParas.WebCode.gbk:
            //                    s1 = HttpUtility.UrlEncode(s2, Encoding.GetEncoding("gbk"));
            //                    break;
            //                case cGlobalParas.WebCode.big5:
            //                    s1 = HttpUtility.UrlEncode(s2, Encoding.GetEncoding("big5"));
            //                    break;
            //                default:
            //                    s1 = HttpUtility.UrlEncode(s2, Encoding.UTF8);
            //                    break;
            //            }

            //            DemoUrl = DemoUrl.Replace(s2, s1);
            //        }
            //    }

            //    //DemoUrl = NewUrl;
            //}

           
            return DemoUrl;
          
        }

        /// <summary>
        /// url���α��룬ֻ�Բ������б���
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="uEncoding"></param>
        /// <returns></returns>
        static public string UrlTwoEncode(string Url, cGlobalParas.WebCode uEncoding)
        {
            string DemoUrl = Url;

            //��ȡ��ַ������
            Regex re = new Regex("(?<=\\=)[^&]*?(?=&|<|$)", RegexOptions.None);
            MatchCollection mc = re.Matches(DemoUrl);

            switch (uEncoding)
            {
                case cGlobalParas.WebCode.utf8:
                    foreach (Match ma in mc)
                    {
                        if (ma.Value.ToString ()!="")
                            DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.UTF8));
                    }
                    break;
                case cGlobalParas.WebCode.gb2312:
                    foreach (Match ma in mc)
                    {
                        if (ma.Value.ToString() != "")
                            DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.GetEncoding("gb2312")));
                    }
                    break;
                case cGlobalParas.WebCode.gbk:
                    foreach (Match ma in mc)
                    {
                        if (ma.Value.ToString() != "")
                            DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.GetEncoding("gbk")));
                    }
                    break;
                case cGlobalParas.WebCode.big5:
                    foreach (Match ma in mc)
                    {
                        if (ma.Value.ToString() != "")
                            DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.GetEncoding("big5")));
                    }
                    break;
                case cGlobalParas.WebCode.Unicode:
                    foreach (Match ma in mc)
                    {
                        if (ma.Value.ToString() != "")
                            DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncodeUnicode(ma.Value.ToString()));
                    }
                    break;
                default:
                    foreach (Match ma in mc)
                    {
                        if (ma.Value.ToString() != "")
                            DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.UTF8));
                    }
                    break;
            }
            
            return DemoUrl;

        }

        /// <summary>
        /// ����ַ����unicode����
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        //static public string UrlUnicodeEncode(string url)
        //{

        //    string DemoUrl = url;

        //    //Regex re = new Regex("[\\u4e00-\\u9fa5]", RegexOptions.None);
        //    //MatchCollection mc = re.Matches(DemoUrl);

        //    //��ȡ��ַ������
        //    Regex re = new Regex("[^\\x00-\\xff]", RegexOptions.None);
        //    MatchCollection mc = re.Matches(DemoUrl);

        //    foreach (Match ma in mc)
        //    {
        //        DemoUrl = DemoUrl.Replace(ma.Value.ToString(),ConvertUnicode(ma.Value.ToString()).Replace("\\u","%25u"));
        //    }

        //    return DemoUrl;
        //}

        /// <summary>
        /// ��url�Ĳ������б���
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="uEncoding"></param>
        /// <returns></returns>
        static public string UrlParaEncode(string Url, cGlobalParas.WebCode uEncoding)
        {
            string DemoUrl = Url;

            //��ȡ��ַ�Ĳ���
            Regex re = new Regex("[/|?|+|=]", RegexOptions.None);
            MatchCollection mc = re.Matches(DemoUrl);

            switch (uEncoding)
            {
                case cGlobalParas.WebCode.utf8:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.UTF8));
                    }
                    break;
                case cGlobalParas.WebCode.gb2312:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.GetEncoding("gb2312")));
                    }
                    break;
                case cGlobalParas.WebCode.gbk:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.GetEncoding("gbk")));
                    }
                    break;
                case cGlobalParas.WebCode.big5:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.GetEncoding("big5")));
                    }
                    break;
                case cGlobalParas.WebCode.Unicode:
                     foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncodeUnicode(ma.Value.ToString()));
                    }
                    break;
                default:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.UTF8));
                    }
                    break;
            }

            DemoUrl= UrlEncode(DemoUrl, uEncoding);

            return DemoUrl;
        }


        //��Json��ʽ����ַת��
        static public string ConvertJsonUrl(string url)
        {
            //�Ƚ��룬�����ڶ�: / & ? = ���н���
            //%3a+%2f+%26+%3f+%3d
            url = Regex.Replace(url, "%3a", ":",RegexOptions.IgnoreCase);
            url = Regex.Replace(url, "%2f", "/", RegexOptions.IgnoreCase);
            url = Regex.Replace(url, "%26", "&", RegexOptions.IgnoreCase);
            url = Regex.Replace(url, "%3f", "?", RegexOptions.IgnoreCase);
            url = Regex.Replace(url, "%3d", "=", RegexOptions.IgnoreCase);
            url=url.Replace ("\\/","/");
            return url;
        }

        /// <summary>
        /// ��url���б��봦��
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static public string ConvertEncodingUrl(string url)
        {
            //�Ƚ��룬�����ڶ�: / & ? = ���н���
            url = Regex.Replace(url, ":","%3a", RegexOptions.IgnoreCase);
            url = Regex.Replace(url, "/", "%2f", RegexOptions.IgnoreCase);
            url = Regex.Replace(url, "&", "%26", RegexOptions.IgnoreCase);
            url = Regex.Replace(url, "\\?", "%3f", RegexOptions.IgnoreCase);
            url = Regex.Replace(url, "=", "%3d", RegexOptions.IgnoreCase);
            return url;
        }

        static public string ConvertUnicodeUrl(string url)
        {
            Regex r = new Regex("(?<code>\\\\u[a-z0-9]{4})", RegexOptions.IgnoreCase);

            string strUnicode = url;
            for (Match mUni = r.Match(strUnicode); mUni.Success; mUni = mUni.NextMatch())
            {
                string strValue = mUni.Result("${code}");   //����
                int CharNum = Int32.Parse(strValue.Substring(2, 4), System.Globalization.NumberStyles.HexNumber);
                string ch = string.Format("{0}", (char)CharNum);
                strUnicode = strUnicode.Replace(strValue, ch);
            }

           url = strUnicode;
           return url;
            
        }

        //�ж�ָ�����ļ����ڵ�Ŀ¼�Ƿ���ڣ��������������
        //����Ĳ����������ļ�����������ļ���������"\"��β
        static public void CreateDirectory(string strDir)
        {

            //��Ҫ��ȡ�ļ�Ŀ¼
            strDir = Path.GetDirectoryName(strDir);

            if (!Directory.Exists(strDir))
            {
                //������Ŀ¼
                Directory.CreateDirectory(strDir);
            }


        }

        static public string Base64Encoding(string str)
        {
            string orgStr = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(str));
            return orgStr;
        }

        static public string Base64Decoding(string str)
        {
            byte[] outputb = Convert.FromBase64String(str);
            string orgStr = Encoding.ASCII.GetString(outputb);
            return orgStr; 
        }


        //public static string getPrjPath()
        //{
        //    return Application.StartupPath + "\\";

        //}

        /// <summary>
        /// �����ܺ�����ݿ������ַ������������
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        static public string DecodingDBCon(string con)
        {
            if (!con.EndsWith(";"))
                con += ";";

            con = Regex.Replace(con, "'|\"", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            if (Regex.IsMatch(con, "password=",RegexOptions.IgnoreCase))
            {
                Match charSetMatch = Regex.Match(con, "(?<=password=).*?(?=;)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string pwd = charSetMatch.Groups[0].ToString();

                if (pwd != "")
                {
                    //string strPre = con.Substring(0, con.ToLower().IndexOf("password="));
                    //int startI = con.ToLower().IndexOf("password=");
                    //startI += pwd.Length + 9;
                    //string strSuffix = con.Substring(startI, con.Length - startI);

                    pwd = ToolUtil.Base64Decoding(pwd);
                    //con = strPre + "password=" + pwd + strSuffix;
                    con = Regex.Replace(con, "(?<=password=).*?(?=;)", pwd, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                }
            }

            return con;
        }

        /// <summary>
        /// ��ָ��ҳ���е���Ե�ַת���Ե�ַ
        /// </summary>
        /// <param name="html"></param>
        /// <param name="relativeLocation"></param>
        /// <returns></returns>
        static public string ConvertToAbsoluteUrls(string html, Uri relativeLocation)
        {
            string strReg = @"(?:href=[\s'""]?)(\S[^'"">]*)(?:[\s'""])";
            MatchCollection matchs = new Regex(strReg).Matches(html);

            Hashtable ht = new Hashtable();
            bool isUrl = false;
            bool isImg = false;


            foreach (Match ma in matchs)
            {
                string tmpUrl = ma.Value.ToString();

                if (!tmpUrl.ToLower().Contains("http://") && !ht.ContainsKey(tmpUrl))
                {

                    ht.Add(tmpUrl, tmpUrl);
                    tmpUrl = Regex.Replace(tmpUrl, "href=|'|\"", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                    try
                    {
                        Uri absoluteUri = new Uri(relativeLocation, tmpUrl);
                        absoluteUri.ToString();

                        html = html.Replace(ma.Value.ToString(), "href=\"" + absoluteUri.ToString() + "\"");

                        absoluteUri = null;

                        isUrl = true;
                    }
                    catch (System.Exception ex)
                    {

                    }
                }

            }

            ht.Clear();

            strReg = @"(?:src=[\s'""]?)(\S[^'"">]*)(?:[\s'""])";

            matchs = new Regex(strReg).Matches(html);

            foreach (Match ma in matchs)
            {
                string tmpUrl = ma.Value.ToString();

                if (!tmpUrl.ToLower().Contains("http://") && !ht.ContainsKey(tmpUrl))
                {
                    ht.Add(tmpUrl, tmpUrl);
                    tmpUrl = Regex.Replace(tmpUrl, "src=|'|\"", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                    try
                    {
                        Uri absoluteUri = new Uri(relativeLocation, tmpUrl);
                        absoluteUri.ToString();

                        html = html.Replace(ma.Value.ToString(), "src=\"" + absoluteUri.ToString() + "\"");

                        absoluteUri = null;
                    }
                    catch (System.Exception ex)
                    {

                    }
                }

            }

            ////����һ�ֿ��ܾ���ֱ�Ӵ������һ����ַ
            //if (isUrl == false && isImg == false)
            //{
            //    Uri absoluteUri = new Uri(relativeLocation, html);
            //    absoluteUri.ToString();

            //    html = absoluteUri.ToString();

            //    absoluteUri = null;
            //}

            return html;

        }

        /// <summary>
        /// ��Ե�ַת���Ե�ַ
        /// </summary>
        /// <param name="url">Url��ַ</param>
        /// <param name="rUrl">��ת���Ե�ַ��Url</param>
        /// <returns></returns>
        static public string RelativeToAbsoluteUrl(string url,string rUrl)
        {
            if (rUrl.Contains("</POST>"))
            {
                //��һ��post��������ȡ��ַ
                string rUrl1 = rUrl.Substring(0, rUrl.IndexOf("<POST"));

                Uri u = new Uri(url);
                Uri absoluteUri = new Uri(u, rUrl1);
                rUrl1= absoluteUri.ToString();

                Match s = Regex.Match(rUrl, @"<POST[^>]*>[\S\s]*</POST>", RegexOptions.IgnoreCase);
                string PostPara = s.Groups[0].Value.ToString();

                rUrl1 = rUrl1 + PostPara;
                return rUrl1;

            }
            else
            {
                Uri u = new Uri(url);
                Uri absoluteUri = new Uri(u, rUrl);
                return absoluteUri.ToString();
            }
        }

        /// <summary>
        /// ��ȡ��ǰ��dll�汾
        /// </summary>
        /// <returns></returns>
        static public cGlobalParas.VersionType GetCurrentVersion()
        {
            return cGlobalParas.VersionType.Enterprise;
        }

        static public cGlobalParas.VersionType GetRegisterVersion(string workPath)
        {
            Tool.cVersion sV = new Tool.cVersion(workPath);
            sV.ReadRegisterInfo();

            cGlobalParas.VersionType SominerVersion = (cGlobalParas.VersionType)sV.SominerVersion;
            sV=null;

            return SominerVersion;
        }

        //������ʽת��
        /// <summary>
        /// ����ת��
        /// </summary>
        /// <param name="str">��Ҫת����ַ�</param>
        /// <param name="isCut">�Ƿ�ȥ��<Wildcard></param>
        /// <returns></returns>
        static public string RegexReplaceTrans(string str, bool isCut)
        {
            if (str == "" || str == null)
                return "";

            string conStr = "";

            if (Regex.IsMatch(str, "<Wildcard>"))
            {
                if (isCut == true)
                {
                    string[] strSS = Regex.Split(str, "</Wildcard>");
                    for (int i = 0; i < strSS.Length; i++)
                    {
                        string ss = strSS[i];
                        if (Regex.IsMatch(ss, "<Wildcard>"))
                        {
                            string strPre = Regex.Escape(ss.Substring(0, ss.IndexOf("<Wildcard>")));
                            string strWildcard = ss.Substring(ss.IndexOf("<Wildcard>") + 10, ss.Length - ss.IndexOf("<Wildcard>") - 10);
                            conStr += strPre + strWildcard;
                        }
                        else
                        {
                            conStr += Regex.Escape(ss);
                        }
                    }

                }
                else if (isCut == false)
                {
                    string[] strSS = Regex.Split(str, "</Wildcard>");
                    for (int i = 0; i < strSS.Length; i++)
                    {
                        string ss = strSS[i];
                        if (Regex.IsMatch(ss, "<Wildcard>"))
                        {
                            string strPre = Regex.Escape(ss.Substring(0, ss.IndexOf("<Wildcard>")));
                            string strWildcard = ss.Substring(ss.IndexOf("<Wildcard>") + 10, ss.Length - ss.IndexOf("<Wildcard>") - 10);
                            conStr += strPre + "<Wildcard>" + strWildcard + "</Wildcard>";
                        }
                        else
                        {
                            conStr += Regex.Escape(ss);
                        }
                    }
                }

            }
            else
            {
                conStr = Regex.Escape(str);
            }

            return conStr;

        }

        static public string convertU2CN(string ss)
        {
            Match mUni;
            Regex r = new Regex("(?<code>\\\\u[a-z0-9]{4})", RegexOptions.IgnoreCase);

            string strUnicode = ss;
            for (mUni = r.Match(strUnicode); mUni.Success; mUni = mUni.NextMatch())
            {
                string strValue = mUni.Result("${code}");   //����
                int CharNum = Int32.Parse(strValue.Substring(2, 4), System.Globalization.NumberStyles.HexNumber);
                string ch = string.Format("{0}", (char)CharNum);
                strUnicode = strUnicode.Replace(strValue, ch);
            }

            return strUnicode;
        }

        static public string ConvertUnicode(string str)
        {
            byte[] bts = Encoding.Unicode.GetBytes(str);
            string r = "";
            for (int i = 0; i < bts.Length; i += 2) r += "\\u" + bts[i + 1].ToString("x").PadLeft(2, '0') + bts[i].ToString("x").PadLeft(2, '0');



            return r;
        }

        static public byte[] Decompress(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream(data);
                Stream zipStream = null;
                zipStream = new GZipStream(ms, CompressionMode.Decompress);
                byte[] dc_data = null;
                dc_data = ExtractBytesFromStream(zipStream, data.Length);
                return dc_data;
            }
            catch
            {
                return null;
            }
        }

        static public byte[] ExtractBytesFromStream(Stream zipStream, int dataBlock)
        {
            byte[] data = null;
            int totalBytesRead = 0;
            try
            {
                while (true)
                {
                    Array.Resize(ref data, totalBytesRead + dataBlock + 1);
                    int bytesRead = zipStream.Read(data, totalBytesRead, dataBlock);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    totalBytesRead += bytesRead;
                }
                Array.Resize(ref data, totalBytesRead);
                return data;
            }
            catch
            {
                return null;
            }
        }

        ////��ȡ�ɼ����ݵĴ��ı�
        //static public string getTxt(string instr)
        //{
        //    string m_outstr;

        //    m_outstr = instr.Clone() as string;
        //    System.Text.RegularExpressions.Regex objReg = new System.Text.RegularExpressions.Regex("(<[^>]+?>)|&nbsp;", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        //    m_outstr = objReg.Replace(m_outstr, "");
        //    System.Text.RegularExpressions.Regex objReg2 = new System.Text.RegularExpressions.Regex("(\\s)+", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        //    m_outstr = objReg2.Replace(m_outstr, " ");

        //    //ȥ��ǰ��Ŀո�
        //    m_outstr = m_outstr.Trim();

        //    return m_outstr;
        //}
        /// <summary>
        /// ��ȡ�ɼ�����Ϊ���ı�
        /// </summary>
        /// <param name="instr"></param>
        /// <returns></returns>
        static public string getTxt(string instr)
        {
            string m_outstr;

            m_outstr = instr.Clone() as string;

            Regex objReg1 = new Regex(@"(?m)<script[^>]*>(\w|\W)*?</script[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg1.Replace(m_outstr, "");

            m_outstr = new Regex(@"(?m)<style[^>]*>(\w|\W)*?</style[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(m_outstr, "");

            Regex objReg = new Regex("(<[^>]+?>)|&nbsp;", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg.Replace(m_outstr, "");

            Regex objReg2 = new Regex("(\\s)+", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg2.Replace(m_outstr, " ");

            return m_outstr;
        }

        /// <summary>
        /// �Ƴ���ҳ���ţ�����Ҫ����ͼƬ
        /// </summary>
        /// <param name="instr"></param>
        /// <returns></returns>
        static public string getTxtAndImage(string instr)
        {
            string m_outstr;

            m_outstr = instr.Clone() as string;

            Regex objReg1 = new Regex(@"(?m)<script[^>]*>(\w|\W)*?</script[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg1.Replace(m_outstr, "");

            m_outstr = new Regex(@"(?m)<style[^>]*>(\w|\W)*?</style[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(m_outstr, "");

            m_outstr = Regex.Replace(m_outstr, @"<(?!img)[^>]*.|</[^>]*.", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return m_outstr;
        }

        //��ȡ��ҳ���ı���������ͼƬ �� ���
        /// <summary>
        /// �Ƴ���ҳ���ţ�������ͼƬ�����
        /// </summary>
        /// <param name="HtmlSource"></param>
        /// <returns></returns>
        static public string GetTextImage(string HtmlSource)
        {
            if (HtmlSource == "")
                return "";

            //ȥ���ı�ͷ���������ı�ǩ
            int index = HtmlSource.IndexOf(">");
            if (index < 120)
            {
                string str1 = HtmlSource.Substring(0, index + 1);

                Regex r = new Regex("[\u4e00-\u9fa5]");
                int f1 = r.Matches(str1).Count;
                if (f1 <= 0)
                {
                    HtmlSource = HtmlSource.Substring(index + 1, HtmlSource.Length - index - 1);
                }
            }

            string instr = HtmlHelper.HtmlFormat(HtmlSource);

            string m_outstr;

            m_outstr = instr.Clone() as string;
            m_outstr = new Regex(@"(?m)<script[^>]*>(\w|\W)*?</script[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(m_outstr, "");
            m_outstr = new Regex(@"(?m)<style[^>]*>(\w|\W)*?</style[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(m_outstr, "");
            m_outstr = new Regex(@"(?m)<select[^>]*>(\w|\W)*?</select[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(m_outstr, "");
            m_outstr = new Regex(@"(?m)<iframe[^>]*>(\w|\W)*?</iframe[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(m_outstr, "");

            //�滻ת���Ŀո�
            Regex objReg1 = new System.Text.RegularExpressions.Regex("&amp;nbsp;", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg1.Replace(m_outstr, " ");


            //�Ȱ�<P><br>�滻��\r\n 
            m_outstr = new Regex(@"<p>|</p>|<br/>", RegexOptions.IgnoreCase | RegexOptions.Multiline).Replace(m_outstr, @"{Enter}");


            Regex objReg = new Regex(@"(<(?![img|\/t])[^>]+?>)|(</(?!t)[^>]+?>)|&nbsp;", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg.Replace(m_outstr, "");

            //�滻���ĸ�ʽ
            objReg = new Regex("(<table[^>]+?>)", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            string strTable = @"<table border='0' cellspacing='1' cellpadding='0' width='100%' align='center' bgcolor='#FFFFFF'>";
            m_outstr = objReg.Replace(m_outstr, strTable);

            //�ٰ�\r\n�滻��<br>
            m_outstr = new Regex(@"{Enter}", RegexOptions.IgnoreCase | RegexOptions.Multiline).Replace(m_outstr, @"<br/>");

            //�滻ת���
            m_outstr = System.Web.HttpUtility.HtmlDecode(m_outstr);

            //ȥ����β�ո�
            m_outstr = m_outstr.Trim();

            return m_outstr;
        }

        //ר�����ڿ��ӻ��ַ����Ƚ�ʹ��
        static public string getTxtByVisual(string instr)
        {
            string m_outstr;

            m_outstr = instr.Clone() as string;
            System.Text.RegularExpressions.Regex objReg = new System.Text.RegularExpressions.Regex("(<[^>]+?>)|&nbsp;", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg.Replace(m_outstr, "");
            System.Text.RegularExpressions.Regex objReg2 = new System.Text.RegularExpressions.Regex("(\\s)+", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg2.Replace(m_outstr, " ");

            //��html���Ž��н���
            m_outstr = System.Web.HttpUtility.HtmlDecode(m_outstr);

            //ȥ��ǰ��Ŀո�
            m_outstr = m_outstr.Trim();

            //ȡ���ַ����еĿո�
            m_outstr = Regex.Replace(m_outstr, "\\s", "", RegexOptions.IgnoreCase);

            return m_outstr;
        }

        static public string GetXpathParaString(string startStr, string endStr)
        {
            if (startStr == "" || endStr == "")
                return "";

            string[] s1 = startStr.Split('/');
            string[] s2 = endStr.Split('/');

            if (s1.Length != s2.Length)
                return "";

            string returnstr = "";
            for (int i = 0; i < s1.Length; i++)
            {
                string s3 = s1[i];
                string s4 = s2[i];

                if (s3 != "" && s4 != "")
                {

                    string p1 = s3.Substring(0, s3.IndexOf("["));
                    string p2 = s4.Substring(0, s4.IndexOf("["));

                    if (p1 != p2)
                        return "";
                    else
                    {
                        returnstr += "/" + p1;

                        string p3 = s3.Substring(s3.IndexOf("[") + 1, s3.IndexOf("]") - s3.IndexOf("[") - 1);
                        string p4 = s4.Substring(s4.IndexOf("[") + 1, s4.IndexOf("]") - s4.IndexOf("[") - 1);

                        if (p3 == p4)
                            returnstr += "[" + p3 + "]";
                        else
                            returnstr += "[{Num:" + p3 + "," + p4 + ",1}]";
                    }
                }

            }

            return returnstr;
        }

        static public string GetDomain(string str)
        {
            Uri u = new Uri(str);

            string strWebSite = u.Host;
            u = null;

            string wSite = "";

            if (strWebSite.EndsWith(".com.cn"))
            {
                wSite = strWebSite.Substring(0, strWebSite.Length - 7);
                int i = wSite.LastIndexOf(".");

                wSite = wSite.Substring(i + 1, wSite.Length - i - 1);
                wSite = wSite + ".com.cn";

            }
            else if (strWebSite.EndsWith(".gov.cn"))
            {
                wSite = strWebSite.Substring(0, strWebSite.Length - 7);
                int i = wSite.LastIndexOf(".");

                wSite = wSite.Substring(i + 1, wSite.Length - i - 1);
                wSite = wSite + ".gov.cn";
            }
            else if (strWebSite.EndsWith(".com"))
            {
                wSite = strWebSite.Substring(0, strWebSite.Length - 4);
                int i = wSite.LastIndexOf(".");

                wSite = wSite.Substring(i + 1, wSite.Length - i - 1);
                wSite = wSite + ".com";
            }
            else if (strWebSite.EndsWith(".cn"))
            {
                wSite = strWebSite.Substring(0, strWebSite.Length - 3);
                int i = wSite.LastIndexOf(".");

                wSite = wSite.Substring(i + 1, wSite.Length - i - 1);
                wSite = wSite + ".cn";
            }
            else if (strWebSite.EndsWith(".net"))
            {
                wSite = strWebSite.Substring(0, strWebSite.Length - 4);
                int i = wSite.LastIndexOf(".");

                wSite = wSite.Substring(i + 1, wSite.Length - i - 1);
                wSite = wSite + ".net";
            }
            else if (strWebSite.EndsWith(".org"))
            {
                wSite = strWebSite.Substring(0, strWebSite.Length - 4);
                int i = wSite.LastIndexOf(".");

                wSite = wSite.Substring(i + 1, wSite.Length - i - 1);
                wSite = wSite + ".org";
            }
            else if (strWebSite.EndsWith(".gov"))
            {
                wSite = strWebSite.Substring(0, strWebSite.Length - 4);
                int i = wSite.LastIndexOf(".");

                wSite = wSite.Substring(i + 1, wSite.Length - i - 1);
                wSite = wSite + ".gov";
            }
            else if (strWebSite.EndsWith(".cc"))
            {
                wSite = strWebSite.Substring(0, strWebSite.Length - 3);
                int i = wSite.LastIndexOf(".");

                wSite = wSite.Substring(i + 1, wSite.Length - i - 1);
                wSite = wSite + ".cc";
            }

            if (wSite == "")
            {
                Uri cu = new Uri(str);
                wSite = cu.Host;
                cu = null;
            }


            return wSite;

        }

        static public string Mearger(string code, string code1)
        {
            code = code.Substring(0, code.LastIndexOf("."));
            code1 = code1.Substring(code1.LastIndexOf(".") + 1, code1.Length - code1.LastIndexOf(".") - 1);
            code = code + "." + code1;
            return code;
        }

        //static public DialogResult MyMessageBox(string Mess, string Title, MessageBoxButtons but, MessageBoxIcon icon)
        //{
        //    frmMessageBox fm = new frmMessageBox();
        //    fm.MessageBox(Mess, Title, but, icon);
        //    DialogResult dr = fm.ShowDialog();
        //    fm.Dispose();

        //    return dr;
        //}

        static public string Encrypt(string str)
        {
            string password = "Sominer";
            string salt = "20101105";
            string plainText = str;
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);

            //Rfc2898DeriveBytes: Used to Generate Strong Keys
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password,
                Encoding.ASCII.GetBytes(salt));//Non-English Alfhabets Will not Work on ASCII Encoding

            SymmetricAlgorithm Alg = new DESCryptoServiceProvider();

            Alg.Key = rfc.GetBytes(Alg.KeySize / 8);
            Alg.IV = rfc.GetBytes(Alg.BlockSize / 8);

            MemoryStream strCiphered = new MemoryStream();//To Store Encrypted Data

            CryptoStream strCrypto = new CryptoStream(strCiphered,
                Alg.CreateEncryptor(), CryptoStreamMode.Write);

            strCrypto.Write(plainBytes, 0, plainBytes.Length);
            strCrypto.Close();
            string rStr = Convert.ToBase64String(strCiphered.ToArray());
            strCiphered.Close();

            return rStr;
        }

        static public string Decrypt(string str)
        {
            string password = "Sominer";
            string salt = "20101105";
            string cipheredText = str;
            byte[] cipheredBytes = Convert.FromBase64String(cipheredText);

            //Rfc2898DeriveBytes: Used to Generate Strong Keys
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password,
                Encoding.ASCII.GetBytes(salt));//Non-English Alfhabets Will not Work on ASCII Encoding

            SymmetricAlgorithm Alg = new DESCryptoServiceProvider();

            Alg.Key = rfc.GetBytes(Alg.KeySize / 8);
            Alg.IV = rfc.GetBytes(Alg.BlockSize / 8);

            MemoryStream strDeciphered = new MemoryStream();//To Store Decrypted Data

            CryptoStream strCrypto = new CryptoStream(strDeciphered,
                Alg.CreateDecryptor(), CryptoStreamMode.Write);

            strCrypto.Write(cipheredBytes, 0, cipheredBytes.Length);
            strCrypto.Close();
            string rStr = Encoding.ASCII.GetString(strDeciphered.ToArray());
            strDeciphered.Close();

            return rStr;
        
        }

        private static char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        //'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'  
        //};
        static public string GenerateRandomNumber(int Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(10)]);
            }
            return newRandom.ToString();
        }


        private static readonly Dictionary<int, string> CodeCollections = new Dictionary<int, string> {  
     { -20319, "a" }, { -20317, "ai" }, { -20304, "an" }, { -20295, "ang" }, { -20292, "ao" }, { -20283, "ba" }, { -20265, "bai" },   
    { -20257, "ban" }, { -20242, "bang" }, { -20230, "bao" }, { -20051, "bei" }, { -20036, "ben" }, { -20032, "beng" }, { -20026, "bi" }  
    , { -20002, "bian" }, { -19990, "biao" }, { -19986, "bie" }, { -19982, "bin" }, { -19976, "bing" }, { -19805, "bo" },   
    { -19784, "bu" }, { -19775, "ca" }, { -19774, "cai" }, { -19763, "can" }, { -19756, "cang" }, { -19751, "cao" }, { -19746, "ce" },  
     { -19741, "ceng" }, { -19739, "cha" }, { -19728, "chai" }, { -19725, "chan" }, { -19715, "chang" }, { -19540, "chao" },   
    { -19531, "che" }, { -19525, "chen" }, { -19515, "cheng" }, { -19500, "chi" }, { -19484, "chong" }, { -19479, "chou" },   
    { -19467, "chu" }, { -19289, "chuai" }, { -19288, "chuan" }, { -19281, "chuang" }, { -19275, "chui" }, { -19270, "chun" },  
     { -19263, "chuo" }, { -19261, "ci" }, { -19249, "cong" }, { -19243, "cou" }, { -19242, "cu" }, { -19238, "cuan" },   
    { -19235, "cui" }, { -19227, "cun" }, { -19224, "cuo" }, { -19218, "da" }, { -19212, "dai" }, { -19038, "dan" }, { -19023, "dang" },  
     { -19018, "dao" }, { -19006, "de" }, { -19003, "deng" }, { -18996, "di" }, { -18977, "dian" }, { -18961, "diao" }, { -18952, "die" }  
    , { -18783, "ding" }, { -18774, "diu" }, { -18773, "dong" }, { -18763, "dou" }, { -18756, "du" }, { -18741, "duan" },   
    { -18735, "dui" }, { -18731, "dun" }, { -18722, "duo" }, { -18710, "e" }, { -18697, "en" }, { -18696, "er" }, { -18526, "fa" },  
     { -18518, "fan" }, { -18501, "fang" }, { -18490, "fei" }, { -18478, "fen" }, { -18463, "feng" }, { -18448, "fo" }, { -18447, "fou" }  
    , { -18446, "fu" }, { -18239, "ga" }, { -18237, "gai" }, { -18231, "gan" }, { -18220, "gang" }, { -18211, "gao" }, { -18201, "ge" },  
     { -18184, "gei" }, { -18183, "gen" }, { -18181, "geng" }, { -18012, "gong" }, { -17997, "gou" }, { -17988, "gu" }, { -17970, "gua" }  
    , { -17964, "guai" }, { -17961, "guan" }, { -17950, "guang" }, { -17947, "gui" }, { -17931, "gun" }, { -17928, "guo" },  
    { -17922, "ha" }, { -17759, "hai" }, { -17752, "han" }, { -17733, "hang" }, { -17730, "hao" }, { -17721, "he" }, { -17703, "hei" },  
     { -17701, "hen" }, { -17697, "heng" }, { -17692, "hong" }, { -17683, "hou" }, { -17676, "hu" }, { -17496, "hua" },   
    { -17487, "huai" }, { -17482, "huan" }, { -17468, "huang" }, { -17454, "hui" }, { -17433, "hun" }, { -17427, "huo" },   
    { -17417, "ji" }, { -17202, "jia" }, { -17185, "jian" }, { -16983, "jiang" }, { -16970, "jiao" }, { -16942, "jie" },   
    { -16915, "jin" }, { -16733, "jing" }, { -16708, "jiong" }, { -16706, "jiu" }, { -16689, "ju" }, { -16664, "juan" },   
    { -16657, "jue" }, { -16647, "jun" }, { -16474, "ka" }, { -16470, "kai" }, { -16465, "kan" }, { -16459, "kang" }, { -16452, "kao" },  
     { -16448, "ke" }, { -16433, "ken" }, { -16429, "keng" }, { -16427, "kong" }, { -16423, "kou" }, { -16419, "ku" }, { -16412, "kua" }  
    , { -16407, "kuai" }, { -16403, "kuan" }, { -16401, "kuang" }, { -16393, "kui" }, { -16220, "kun" }, { -16216, "kuo" },   
    { -16212, "la" }, { -16205, "lai" }, { -16202, "lan" }, { -16187, "lang" }, { -16180, "lao" }, { -16171, "le" }, { -16169, "lei" },   
    { -16158, "leng" }, { -16155, "li" }, { -15959, "lia" }, { -15958, "lian" }, { -15944, "liang" }, { -15933, "liao" },   
    { -15920, "lie" }, { -15915, "lin" }, { -15903, "ling" }, { -15889, "liu" }, { -15878, "long" }, { -15707, "lou" }, { -15701, "lu" },  
     { -15681, "lv" }, { -15667, "luan" }, { -15661, "lue" }, { -15659, "lun" }, { -15652, "luo" }, { -15640, "ma" }, { -15631, "mai" },  
     { -15625, "man" }, { -15454, "mang" }, { -15448, "mao" }, { -15436, "me" }, { -15435, "mei" }, { -15419, "men" },   
    { -15416, "meng" }, { -15408, "mi" }, { -15394, "mian" }, { -15385, "miao" }, { -15377, "mie" }, { -15375, "min" },   
    { -15369, "ming" }, { -15363, "miu" }, { -15362, "mo" }, { -15183, "mou" }, { -15180, "mu" }, { -15165, "na" }, { -15158, "nai" },   
    { -15153, "nan" }, { -15150, "nang" }, { -15149, "nao" }, { -15144, "ne" }, { -15143, "nei" }, { -15141, "nen" }, { -15140, "neng" }  
    , { -15139, "ni" }, { -15128, "nian" }, { -15121, "niang" }, { -15119, "niao" }, { -15117, "nie" }, { -15110, "nin" },   
    { -15109, "ning" }, { -14941, "niu" }, { -14937, "nong" }, { -14933, "nu" }, { -14930, "nv" }, { -14929, "nuan" }, { -14928, "nue" }  
    , { -14926, "nuo" }, { -14922, "o" }, { -14921, "ou" }, { -14914, "pa" }, { -14908, "pai" }, { -14902, "pan" }, { -14894, "pang" },  
     { -14889, "pao" }, { -14882, "pei" }, { -14873, "pen" }, { -14871, "peng" }, { -14857, "pi" }, { -14678, "pian" },   
    { -14674, "piao" }, { -14670, "pie" }, { -14668, "pin" }, { -14663, "ping" }, { -14654, "po" }, { -14645, "pu" }, { -14630, "qi" },  
     { -14594, "qia" }, { -14429, "qian" }, { -14407, "qiang" }, { -14399, "qiao" }, { -14384, "qie" }, { -14379, "qin" },  
     { -14368, "qing" }, { -14355, "qiong" }, { -14353, "qiu" }, { -14345, "qu" }, { -14170, "quan" }, { -14159, "que" },   
    { -14151, "qun" }, { -14149, "ran" }, { -14145, "rang" }, { -14140, "rao" }, { -14137, "re" }, { -14135, "ren" }, { -14125, "reng" }  
    , { -14123, "ri" }, { -14122, "rong" }, { -14112, "rou" }, { -14109, "ru" }, { -14099, "ruan" }, { -14097, "rui" }, { -14094, "run" }  
    , { -14092, "ruo" }, { -14090, "sa" }, { -14087, "sai" }, { -14083, "san" }, { -13917, "sang" }, { -13914, "sao" }, { -13910, "se" }  
    , { -13907, "sen" }, { -13906, "seng" }, { -13905, "sha" }, { -13896, "shai" }, { -13894, "shan" }, { -13878, "shang" },   
    { -13870, "shao" }, { -13859, "she" }, { -13847, "shen" }, { -13831, "sheng" }, { -13658, "shi" }, { -13611, "shou" },  
     { -13601, "shu" }, { -13406, "shua" }, { -13404, "shuai" }, { -13400, "shuan" }, { -13398, "shuang" }, { -13395, "shui" },  
     { -13391, "shun" }, { -13387, "shuo" }, { -13383, "si" }, { -13367, "song" }, { -13359, "sou" }, { -13356, "su" },   
    { -13343, "suan" }, { -13340, "sui" }, { -13329, "sun" }, { -13326, "suo" }, { -13318, "ta" }, { -13147, "tai" }, { -13138, "tan" },  
     { -13120, "tang" }, { -13107, "tao" }, { -13096, "te" }, { -13095, "teng" }, { -13091, "ti" }, { -13076, "tian" },   
    { -13068, "tiao" }, { -13063, "tie" }, { -13060, "ting" }, { -12888, "tong" }, { -12875, "tou" }, { -12871, "tu" },   
    { -12860, "tuan" }, { -12858, "tui" }, { -12852, "tun" }, { -12849, "tuo" }, { -12838, "wa" }, { -12831, "wai" }, { -12829, "wan" }  
    , { -12812, "wang" }, { -12802, "wei" }, { -12607, "wen" }, { -12597, "weng" }, { -12594, "wo" }, { -12585, "wu" }, { -12556, "xi" }  
    , { -12359, "xia" }, { -12346, "xian" }, { -12320, "xiang" }, { -12300, "xiao" }, { -12120, "xie" }, { -12099, "xin" },   
    { -12089, "xing" }, { -12074, "xiong" }, { -12067, "xiu" }, { -12058, "xu" }, { -12039, "xuan" }, { -11867, "xue" },   
    { -11861, "xun" }, { -11847, "ya" }, { -11831, "yan" }, { -11798, "yang" }, { -11781, "yao" }, { -11604, "ye" }, { -11589, "yi" },  
     { -11536, "yin" }, { -11358, "ying" }, { -11340, "yo" }, { -11339, "yong" }, { -11324, "you" }, { -11303, "yu" },   
    { -11097, "yuan" }, { -11077, "yue" }, { -11067, "yun" }, { -11055, "za" }, { -11052, "zai" }, { -11045, "zan" },  
     { -11041, "zang" }, { -11038, "zao" }, { -11024, "ze" }, { -11020, "zei" }, { -11019, "zen" }, { -11018, "zeng" },   
    { -11014, "zha" }, { -10838, "zhai" }, { -10832, "zhan" }, { -10815, "zhang" }, { -10800, "zhao" }, { -10790, "zhe" },   
    { -10780, "zhen" }, { -10764, "zheng" }, { -10587, "zhi" }, { -10544, "zhong" }, { -10533, "zhou" }, { -10519, "zhu" },   
    { -10331, "zhua" }, { -10329, "zhuai" }, { -10328, "zhuan" }, { -10322, "zhuang" }, { -10315, "zhui" }, { -10309, "zhun" },   
    { -10307, "zhuo" }, { -10296, "zi" }, { -10281, "zong" }, { -10274, "zou" }, { -10270, "zu" }, { -10262, "zuan" }, { -10260, "zui" }  
    , { -10256, "zun" }, { -10254, "zuo" } };
        
        public static string ToPinYin(string txt)
        {
            txt = txt.Trim();
            byte[] arr = new byte[2];   //ÿ������Ϊ2�ֽ�   
            StringBuilder result = new StringBuilder();//ʹ��StringBuilder�Ż��ַ�������  
            int charCode = 0;
            int arr1 = 0;
            int arr2 = 0;
            char[] arrChar = txt.ToCharArray();
            for (int j = 0; j < arrChar.Length; j++)   //����������ַ�   
            {
                arr = System.Text.Encoding.Default.GetBytes(arrChar[j].ToString());//����ϵͳĬ�ϱ���õ��ֽ���   
                if (arr.Length == 1)//���ֻ��1�ֽ�˵�����ַ����Ǻ��֣���������ѭ��   
                {
                    result.Append(arrChar[j].ToString());
                    continue;

                }
                arr1 = (short)(arr[0]);   //ȡ�ֽ�1   
                arr2 = (short)(arr[1]);   //ȡ�ֽ�2   
                charCode = arr1 * 256 + arr2 - 65536;//���㺺�ֵı���   

                if (charCode > -10254 || charCode < -20319)  //������ں��ֱ��뷶Χ���򲻸ı�   
                {
                    result.Append(arrChar[j]);
                }
                else
                {
                    //���ݺ��ֱ��뷶Χ���Ҷ�Ӧ��ƴ�������浽�����   
                    //����charCode�ļ���һ�����ڣ�����Ҫ�ұ�����С�ļ���һ����  
                    if (!CodeCollections.ContainsKey(charCode))
                    {
                        for (int i = charCode; i <= 0; --i)
                        {
                            if (CodeCollections.ContainsKey(i))
                            {
                                result.Append( CodeCollections[i] + " ");
                                break;
                            }
                        }
                    }
                    else
                    {
                        result.Append( CodeCollections[charCode] + " ");
                    }
                }
            }
            return result.ToString();
        }

        public static string MergerCookie(string oCookie, CookieCollection newCookie)
        {
            Hashtable cookieTable = new Hashtable();
            string[] oldCookie = oCookie.Trim().Split(';');
            for (int j = 0; j < oldCookie.Length; j++)
            {
                if (oldCookie[j].ToString() != "")
                {
                    //try
                    //{
                        string label = oldCookie[j].Substring(0, oldCookie[j].IndexOf("="));
                        string value = oldCookie[j].Substring(oldCookie[j].IndexOf("=") + 1, oldCookie[j].Length - oldCookie[j].IndexOf("=") - 1);
                        if (!cookieTable.ContainsKey(label))
                            cookieTable.Add(label, value);
                    //}
                    //catch (System.Exception ex)
                    //{
                        
                    //}
                }
            }

            for (int i = 0; i < newCookie.Count; i++)
            {
                if (newCookie[i].ToString() != "")
                {
                    string nCookie = newCookie[i].ToString();

                    string[] cc = nCookie.Split(';');
                    for (int m = 0; m < cc.Length; m++)
                    {
                        string ss = cc[m].Trim ();
                        if (!ss.StartsWith("$"))
                        {
                            string label = ss.Substring(0, ss.IndexOf("="));
                            string value = ss.Substring(ss.IndexOf("=") + 1, ss.Length - ss.IndexOf("=") - 1);

                            if (cookieTable.ContainsKey(label))
                            {
                                cookieTable.Remove(label);
                                cookieTable.Add(label, value);
                            }
                            else
                            {
                                cookieTable.Add(label, value);
                            }
                        }
                    }
                }
            }

            //��cookieת�����ַ���
            string rCookie = "";
            foreach (DictionaryEntry de in cookieTable)
            {
                rCookie += de.Key + "=" + de.Value + ";";
            }
            if (rCookie != "")
                rCookie = rCookie.Substring(0, rCookie.Length - 1);

            return rCookie;
        }

        public static long GetTimestamp()
        {
            long epoch = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            return epoch;
        }

        public static DateTime ConvertTimestamp(string now)
        {
            string timeStamp = now;
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult;
        } 

        public static string TextToRtf(string AText)
        {
            string vReturn = "";
            foreach (char vChar in AText)
            {
                switch (vChar)
                {
                    case '/':
                        vReturn += @"//";
                        break;
                    case '{':
                        vReturn += @"/{";
                        break;
                    case '}':
                        vReturn += @"/}";
                        break;
                    default:
                        if (vChar > (char)127)
                            vReturn += @"/u" + ((int)vChar).ToString() + "?";
                        else vReturn += vChar;
                        break;
                }
            }
            return vReturn;

        }

        static public string GetIP()
        {

            IPAddress[] arrIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in arrIPAddresses)
            {
                if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))
                {
                    return ip.ToString();
                }
            }

            return "";
        }

        /// <summary>
        /// ��������Ŀ¼�����Ŀ��Ŀ¼�����ڣ����Զ�����
        /// </summary>
        /// <param name="OldDirectory"></param>
        /// <param name="NewDirectory"></param>
        static public void CopyDirectory(DirectoryInfo OldDirectory, DirectoryInfo NewDirectory)
        {
            string NewDirectoryFullName = NewDirectory.FullName ;

            if (!Directory.Exists(NewDirectoryFullName))
                Directory.CreateDirectory(NewDirectoryFullName);

            FileInfo[] OldFileAry = OldDirectory.GetFiles();
            foreach (FileInfo aFile in OldFileAry)
                File.Copy(aFile.FullName, NewDirectoryFullName + @"\" + aFile.Name, true);

            DirectoryInfo[] OldDirectoryAry = OldDirectory.GetDirectories();
            foreach (DirectoryInfo aOldDirectory in OldDirectoryAry)
            {
                DirectoryInfo aNewDirectory = new DirectoryInfo(NewDirectoryFullName);
                CopyDirectory(aOldDirectory, aNewDirectory);
            }
        }

        static public string ConvertDateTime(string pDate)
        {
            DateTime pd = System.DateTime.Now;

            if (Regex.IsMatch(pDate, "\\d{9,10}"))
            {
                //�п�����ʱ���
                if (pDate.Length == 13)
                    pDate = pDate.Substring(0, 10);

                pd = ConvertTimestamp(pDate);
            }
            else
            {

                try
                {
                    //��������
                    if (pDate.IndexOf("Сʱǰ") > 0)
                    {
                        int s = GetNumber(pDate);
                        s = 0 - s;

                        pd = pd.AddHours(s);
                    }
                    else if (pDate.IndexOf("����ǰ") > 0)
                    {
                        int s = GetNumber(pDate);
                        s = 0 - s;

                        pd = pd.AddMinutes(s);

                    }
                    else if (pDate.IndexOf("����") > -1)
                    {
                        pDate = pDate.Replace("����", "").Trim();
                        pDate = System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + " " + pDate;
                        pd = DateTime.Parse(pDate);
                    }
                    else if (pDate.IndexOf("����") > -1)
                    {
                        pDate = pDate.Replace("����", "").Trim();
                        DateTime dt = System.DateTime.Now.AddDays(-1);

                        pDate = dt.Year + "-" + dt.Month + "-" + dt.Day + " " + pDate;
                        pd = DateTime.Parse(pDate);
                    }
                    else if (pDate == "�ո�")
                    {
                        pd = System.DateTime.Now;
                    }
                    else if (pDate == "")
                    {
                        pDate = System.DateTime.Now.ToString();
                    }
                    else if (pDate.IndexOf("��") > 0)
                    {
                        pDate = System.DateTime.Now.ToString();
                    }
                    else
                    {
                        pd = DateTime.Parse(pDate);
                    }
                }
                catch (System.Exception ex)
                {
                    return pDate;
                }
            }

            return pd.ToString("yyyy-MM-dd HH:mm:ss");
        }

        static public string CleanInvalidXmlChars(string text)
        {
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return System.Text.RegularExpressions.Regex.Replace(text, re, "");
        }

        static public int GetNumber(string ss)
        {
            Match aa = Regex.Match(ss, @"\d+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            ss = aa.Value.ToString();

            try
            {
                int count = int.Parse(ss);
                return count;
            }
            catch (System.Exception)
            {
                return 0;
            }

        }

        #region �ֵ��Ӧ���������滻
        /// <summary>
        /// ����ָ�����ֵ䣬���������滻
        /// </summary>
        /// <param name="fName"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string ReplaceDict(string fName, string str)
        {
            FileStream myStream = File.Open(fName, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(myStream, System.Text.Encoding.UTF8);

            string strLine = "";
            while (strLine != null)
            {
                strLine = sr.ReadLine();
                if (strLine != null && strLine.Length > 0)
                {
                    string[] ss = strLine.Split(',');
                    if (ss.Length > 1)
                    {
                        if (str.IndexOf (ss[0])>-1)
                        {
                            str = str.Replace( ss[0],ss[1]);
                        }
                    }
                }
            }

            sr.Close();
            sr.Dispose();
            myStream.Close();
            myStream.Dispose();
            return str;
        }
        #endregion

        /// <summary>
        /// �Ƴ��ַ����е�css��ʽ
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string RemoveCSS(string str)
        {
            str = Regex.Replace(str, "class=[^>]+?(?=[>|'|\"])", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "style=[^>]+?(?=[>|'|\"])", "", RegexOptions.IgnoreCase);
            return str;
        }

        public static string GetExecutableOutput(string sExecute, string sParams, out int iExitCode)
        {
            iExitCode = -999;
            StringBuilder builder = new StringBuilder();
            builder.Append("Results from " + sExecute + " " + sParams + "\r\n\r\n");
            try
            {
                string str;
                Process process = new Process
                {
                    StartInfo = { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = false, CreateNoWindow = true, FileName = sExecute, Arguments = sParams }
                };
                process.Start();
                while ((str = process.StandardOutput.ReadLine()) != null)
                {
                    str = str.TrimEnd(new char[0]);
                    if (str != string.Empty)
                    {
                        builder.Append(str + "\r\n");
                    }
                }
                iExitCode = process.ExitCode;
                process.Dispose();
            }
            catch (Exception exception)
            {
                builder.Append("Exception thrown: " + exception.ToString() + "\r\n" + exception.StackTrace.ToString());
            }
            builder.Append("-------------------------------------------\r\n");
            return builder.ToString();
        }


        public static DataTable CopyToDataTable<T>(this IEnumerable<T> array)
        {
            var ret = new DataTable();
            foreach (PropertyDescriptor dp in TypeDescriptor.GetProperties(typeof(T)))
                //if (!dp.IsReadOnly)
                    ret.Columns.Add(dp.Name, dp.PropertyType);
            foreach (T item in array)
            {
                var Row = ret.NewRow();
                foreach (PropertyDescriptor dp in TypeDescriptor.GetProperties(typeof(T)))
                    //if (!dp.IsReadOnly)
                        Row[dp.Name] = dp.GetValue(item);
                ret.Rows.Add(Row);
            }
            return ret;
        }

        public static string AesDecrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Convert.FromBase64String(str);

            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7
            };

            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }

    }
}

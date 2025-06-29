using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using Soukey;
using Soukey.Tools;
using SoukeyResource;

namespace SoukeyDataPublish
{
    class cTool
    {
        static public string ReplaceTrans(string str)
        {
            if (str == "" || str == null)
                return "";

            string conStr = "";
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

            }
            else
            {
                conStr = str;
            }
            return conStr;
        }

        //对Url中文部分进行编码，返回编码后的Url，
        //注意：只对中文进行编码
        static public string UrlEncode(string Url, cGlobalParas.WebCode uEncoding)
        {
            string DemoUrl = Url;

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
                default:
                    foreach (Match ma in mc)
                    {
                        DemoUrl = DemoUrl.Replace(ma.Value.ToString(), HttpUtility.UrlEncode(ma.Value.ToString(), Encoding.UTF8));
                    }
                    break;
            }

            return DemoUrl;
        }

        static public string Base64Encoding(string str)
        {
            //first get the bytes for the original

            byte[] data = System.Text.UnicodeEncoding.UTF8.GetBytes(str);
            Base64Encoder myEncoder = new Base64Encoder(data);
            StringBuilder sb = new StringBuilder();

            sb.Append(myEncoder.GetEncoded());

            return sb.ToString();
        }

        static public string Base64Decoding(string str)
        {
            char[] data = str.ToCharArray();
            Base64Decoder myDecoder = new Base64Decoder(data);
            StringBuilder sb = new StringBuilder();

            byte[] temp = myDecoder.GetDecoded();
            sb.Append(System.Text.UTF8Encoding.UTF8.GetChars(temp));

            return sb.ToString();
        }

        static public string GetHtmlSource(string url, bool Isbool)
        {
            if (url == "")
                return "";

            string charSet = "";
            WebClient myWebClient = new WebClient();


            //获取或设置用于对向 Internet 资源的请求进行身份验证的网络凭据。   
            myWebClient.Credentials = CredentialCache.DefaultCredentials;


            byte[] myDataBuffer;
            string strWebData;

            try
            {
                //从资源下载数据并返回字节数组。（加@是因为网址中间有"/"符号）   
                myDataBuffer = myWebClient.DownloadData(@url);
                strWebData = Encoding.Default.GetString(myDataBuffer);
            }
            catch (System.Net.WebException)
            {
                return "";
            }

            //获取此页面的编码格式
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
        /// 将加密后的数据库连接字符串的密码解码
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        static public string DecodingDBCon(string con)
        {
            if (Regex.IsMatch(con, "password="))
            {
                Match charSetMatch = Regex.Match(con, "(?<=password=).*?(?=;)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string pwd = charSetMatch.Groups[0].ToString();

                if (pwd != "")
                {
                    string strPre = con.Substring(0, con.IndexOf("password="));
                    int startI = con.IndexOf("password=");
                    startI += pwd.Length + 9;
                    string strSuffix = con.Substring(startI, con.Length - startI);

                    pwd = cTool.Base64Decoding(pwd);
                    con = strPre + "password=" + pwd + strSuffix;
                }
            }

            return con;
        }
    }
}

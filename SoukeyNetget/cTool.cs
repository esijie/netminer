using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.IO;
using System.Web;
using System.Windows.Forms;
using Soukey;
using Soukey.Tools;
using System.IO.Compression;
using SoukeyResource;

///���ܣ�����С���� ��̬
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺����ϵͳ���Ʋ���
///�����ƻ�������
///˵������
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget
{
    static class cTool
    {
        private const int INTERNET_CONNECTION_MODEM =1;
        private const int INTERNET_CONNECTION_LAN= 2;
        private const int INTERNET_CONNECTION_PROXY=4;
        private const int INTERNET_CONNECTION_MODEM_BUSY = 8;
           
         //���壨���ã�API����  
        //[DllImport("wininet.dll")]

        //private static   extern   bool   InternetGetConnectedState   (out int lpdwFlags ,int dwReserved );  
           
        //�жϵ�ǰ�Ƿ�����Internet
        //static public  bool IsLinkInternet ()
        //{  
        //    int   lfag=0;
        //    bool IsInternet;

        //    if (InternetGetConnectedState(out lfag, 0))
        //        IsInternet = true;
        //    else
        //        IsInternet = false;

        //    return IsInternet;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">ָ����Url��ַ</param>
        /// <param name="Isbool">�Ƿ�ȥ����ҳԴ���еĻس����з���true ȥ��</param>
        /// <returns></returns>
        static  public string GetHtmlSource(string url,bool Isbool)
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
            catch (System.Net.WebException  ) 
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

        //ȥ���ַ����Ļس����з���
        /// <summary>
        /// ȥ���ַ����Ļس����з���
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
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
            string conStr = str;

            return conStr;

        }

        //������ʽת��
        /// <summary>
        /// ����ת��
        /// </summary>
        /// <param name="str">��Ҫת����ַ�</param>
        /// <param name="isCut">�Ƿ�ȥ��<Wildcard></param>
        /// <returns></returns>
        static public string RegexReplaceTrans(string str,bool isCut)
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
                else if (isCut ==false )
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

        //�������������󹤵����·��
        static public string GetRelativePath(string absPath)
        {
            string mainDir = Program.getPrjPath();

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

            Regex re = new Regex("[\\u4e00-\\u9fa5]", RegexOptions.None);
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

            return  DemoUrl;
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

        static public DialogResult MyMessageBox(string Mess, string Title, MessageBoxButtons but, MessageBoxIcon icon)
        {
            frmMessageBox fm = new frmMessageBox();
            fm.MessageBox(Mess, Title, but, icon);
            DialogResult dr= fm.ShowDialog();
            fm.Dispose();

            return dr;
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

        /// <summary>
        /// �����ܺ�����ݿ������ַ������������
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        static public string DecodingDBCon(string con)
        {
            if (Regex.IsMatch(con, "password="))
            {
                Match charSetMatch = Regex.Match(con, "(?<=password=).*?(?=;)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string pwd= charSetMatch.Groups[0].ToString();

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

        //��ȡ�ɼ����ݵĴ��ı�
        static public string getTxt(string instr)
        {
            string m_outstr;

            m_outstr = instr.Clone() as string;
            System.Text.RegularExpressions.Regex objReg = new System.Text.RegularExpressions.Regex("(<[^>]+?>)|&nbsp;", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg.Replace(m_outstr, "");
            System.Text.RegularExpressions.Regex objReg2 = new System.Text.RegularExpressions.Regex("(\\s)+", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg2.Replace(m_outstr, " ");

            //ȥ��ǰ��Ŀո�
            m_outstr = m_outstr.Trim();

            return m_outstr;
        }

        static public string ConvertToAbsoluteUrls(string html, Uri relativeLocation)
        {

            //string strReg = @"(?<=href=[\s'""]?)(\S[^'"">]*)(?=[\s'""])";

            string PreUrl = relativeLocation.ToString();
            PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
            if (PreUrl.IndexOf("/") > 0)
                PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));
            PreUrl = "http://" + PreUrl + "/";

            Match ma = Regex.Match(relativeLocation.ToString(), ".*/");
            string PreUrl1 = ma.Groups[0].Value.ToString();

            html = Regex.Replace(html, "href=['\"]?/", "href=\"" + PreUrl, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            html = Regex.Replace(html, "src=['\"]?/", "src=\"" + PreUrl, RegexOptions.IgnoreCase | RegexOptions.Multiline);


            html = Regex.Replace(html, "href=['\"]?(?=[^\"http|http|'http])", "href=\"" + PreUrl1, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            html = Regex.Replace(html, "src=['\"]?(?=[^\"http|http|'http])", "src=\"" + PreUrl1, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return html;

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

                        string p3 = s3.Substring(s3.IndexOf("[") + 1, s3.IndexOf("]") - s3.IndexOf("[")-1);
                        string p4 = s4.Substring(s4.IndexOf("[") + 1, s4.IndexOf("]") - s4.IndexOf("[")-1);

                        if (p3 == p4)
                            returnstr += "[" + p3 + "]";
                        else
                            returnstr += "[{Num:" + p3 + "," + p4 + ",1}]";
                    }
                }
                
            }

            return returnstr;
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
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;
using NetMiner.Resource;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;

namespace NetMiner.Common.HttpSocket
{
    public static class cHttpSocket
    {
        private static string m_vCode;
        private static string m_ImgCookie=string.Empty ;
        private static readonly int DEF_PACKET_LENGTH = 2048;

        /// <summary>
        /// 实现登录，获取登录后的Cookie
        /// </summary>
        /// <param name="User">用户名</param>
        /// <param name="Pwd">密码</param>
        /// <param name="uCode">Url编码</param>
        /// <param name="Domain">需要登录的地址域名</param>
        /// <param name="oldDomain">模板配置的登录域名</param>
        /// <param name="LoginUrl">登录地址</param>
        /// <param name="LoginRurl">登录来源地址</param>
        /// <param name="loginCodeUrl">登录验证码地址</param>
        /// <param name="successFlag">登录成功标记</param>
        /// <param name="FailFlag">登录失败标记</param>
        /// <param name="loginParas">登录参数</param>
        /// <param name="cookie">返回登录后的Cookie</param>
        /// <param name="wSource">返回登陆后的源码</param>
        /// <returns></returns>
        public static bool Login(string User, string Pwd,cGlobalParas.WebCode uCode, string Domain, string oldDomain,
          string LoginUrl,string LoginRurl,string loginCodeUrl, string successFlag, 
            string FailFlag, Dictionary<string, string> loginParas,bool isPluginCode, string strPlugin, out string cookie,out string wSource)
        {
            //替换url
            LoginUrl = LoginUrl.Replace(oldDomain.ToLower(), Domain.ToLower());
            LoginRurl = LoginRurl.Replace(oldDomain.ToLower(), Domain.ToLower());

            //首先先获取一次打开页面的cookie
            string webSource = "";

            #region 处理来路页面的信息
            Hashtable rFormValue = new Hashtable();

            string strCookie = "";

            if (LoginRurl != "")
            {
                string strLoginHeader = "";
                
                    strLoginHeader = "User-Agent: Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0\r\n";
                    strLoginHeader += "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\r\n";
                    //strLoginHeader += "Content-Type:text/html\r\n";
                    strLoginHeader += "Accept-Language:zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3\r\n";
                    strLoginHeader += "Accept-Encoding:gzip,deflate\r\n";
                    strLoginHeader += "\r\n";
               

                webSource = cHttpSocket.GetUrl(LoginRurl, strLoginHeader, "GET", cGlobalParas.WebCode.NoCoding,"",0 ,  out cookie);
                webSource = webSource.Replace("\r", "");
                webSource = webSource.Replace("\n", "");
                strCookie = cookie.Trim ();

                //先获取form
                Regex reForm = new Regex("<input[^>]+?>",RegexOptions.IgnoreCase);
                MatchCollection mcForm = reForm.Matches(webSource);
                foreach (Match ma in mcForm)
                {
                    //判断Type是否为hidden，如果是，则添加到表单表中
                    Match a = Regex.Match(ma.ToString (), @"type=.+?\s", RegexOptions.IgnoreCase);
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
                            Match a1 = Regex.Match(ss1, @"(?<=name=).+?\s", RegexOptions.IgnoreCase);
                            n = a1.Groups[0].Value.ToString();

                            a1 = Regex.Match(ss1, @"(?<=value=).+?\s", RegexOptions.IgnoreCase);
                            v = a1.Groups[0].Value.ToString();

                            n = n.Replace("'", "");
                            n = n.Replace("\"", "").Trim ();

                            v = v.Replace("'", "");
                            v = v.Replace("\"", "").Trim ();

                        
                            rFormValue.Add(n, v);
                        }
                        catch { }
                    }

                }
            }
            #endregion

            #region  判断是否有验证码，如果有，则提取验证码信息
            if (loginCodeUrl != "")
            {
                loginCodeUrl = loginCodeUrl.Replace(oldDomain.ToLower(), Domain.ToLower());

                if (isPluginCode == true)
                {
                    //开始获取图片

                    string imgName = NetMiner.Common.HttpSocket.cHttpSocket.GetImage(Path.GetTempPath().ToString (), loginCodeUrl, ref m_ImgCookie);

                    //开始获取验证码
                    cRunPlugin rPlugin = new cRunPlugin();
                    if (strPlugin != "")
                    {
                        m_vCode = rPlugin.CallVerifyCode(imgName, loginCodeUrl, strPlugin);
                    }
                    rPlugin = null;

                    //如果图片返回了Cookie，则使用图片的cookie
                    if (m_ImgCookie != "")
                    {
                        strCookie = m_ImgCookie;
                        cookie = strCookie;
                    }

                    
                }
                else
                {
                    //获取验证码的图片，并交由用户数据
                    frmInputVCode f = new frmInputVCode();
                    f.RVCode = GetVCode;
                    f.iniData(loginCodeUrl, strCookie);
                    if (f.ShowDialog() == DialogResult.Cancel)
                    {
                        //取消登录
                        cookie = "";
                        wSource = "";
                        return false;
                    }
                    f.Dispose();

                    //如果图片返回了Cookie，则使用图片的cookie
                    if (m_ImgCookie != "")
                    {
                        strCookie = m_ImgCookie;
                        cookie = strCookie;
                    }
                }
            }
            #endregion

            bool isMutilData = false;

            #region 判断表单是否为 multipart/form-data 类型
            Regex re = new Regex("(?<=<form).+?(?=>)", RegexOptions.IgnoreCase);
            MatchCollection mc = re.Matches(webSource);
            foreach (Match ma in mc)
            {
                if (ma.ToString().ToLower().IndexOf("multipart/form-data") > 0)
                {
                    //取表单的action，判断url是否和发布的url相同
                    Match a = Regex.Match(ma.ToString(), @"(?<=action=).+?\s", RegexOptions.IgnoreCase);
                    string temppUrl = a.Groups[0].Value.ToString();

                    temppUrl = System.Web.HttpUtility.UrlDecode(temppUrl);
                    temppUrl = temppUrl.Replace("'", "");
                    temppUrl = temppUrl.Replace("\"", "").Trim();

                    if (!temppUrl.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string PreUrl = LoginRurl;

                        if (temppUrl.Substring(0, 1) == "/" || temppUrl.Substring(0, 1) == "\\")
                        {
                            PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                            if (PreUrl.IndexOf("/") > 0)
                                PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));

                            PreUrl = "http://" + PreUrl;
                        }
                        else if (temppUrl.StartsWith("?"))
                        {
                            PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                            if (PreUrl.IndexOf("?") > 0)
                                PreUrl = PreUrl.Substring(0, PreUrl.LastIndexOf("?"));

                            PreUrl = "http://" + PreUrl;
                        }
                        else
                        {
                            Match a1 = Regex.Match(PreUrl, ".*/");
                            PreUrl = a1.Groups[0].Value.ToString();
                        }
                        temppUrl = PreUrl + temppUrl;
                    }

                    if (temppUrl == LoginUrl)
                    {
                        isMutilData = true;
                        break;
                    }


                }
            }

            #endregion

            #region  如果是multipart/form-data 类型，则重新构造头部
            string boundaryValue = "";

            if (isMutilData == true)
            {
                //随机产生一个boundary的值
                boundaryValue = "---------------------------7d" + ToolUtil.GenerateRandomNumber(12);

            }
            #endregion

            #region 开始登录操作
            string pData = "";

            //替换登录操作中的参数，并构造post参数
            foreach (KeyValuePair<string, string> de in loginParas)
            {
                string label = de.Key.ToString();
                string value = de.Value.ToString();

                //在此需要判断提供的登录参数中是否有动态的参数名，
                //如果有，则需要解析
                if (label.StartsWith("{登录动态参数"))
                {
                    string dLabelReg = label.Substring(8, label.IndexOf("}") - 8);
                    Match a = Regex.Match(webSource , dLabelReg, RegexOptions.IgnoreCase);
                    label = a.Groups[0].Value.ToString();
                }

                if (value == "{登录参数:用户名}")
                {
                    value = User;
                }
                else if (value == "{登录参数:密码}")
                {
                    value = Pwd;
                }
                else if (value == "{登录参数:验证码}")
                {
                    value = m_vCode;

                }
                else if (value == "{系统参数:取来路页面表单值}")
                {
                    foreach (DictionaryEntry de1 in rFormValue)
                    {
                        if (label == de1.Key.ToString())
                        {
                            value = de1.Value.ToString();
                            break;
                        }
                    }
                }
                if (isMutilData == true)
                {
                    if (value.IndexOf("Content-Type", StringComparison.CurrentCultureIgnoreCase) > -1)
                    {
                        pData += "--" + boundaryValue + "\r\n";
                        pData += "Content-Disposition: form-data; name=\"" + label + "\";filename=\"\"\r\n";
                        pData += UrlCharCode(value, uCode); 
                        pData += "\r\n";
                        pData += "\r\n";
                    }
                    else
                    {
                        pData += "--" + boundaryValue + "\r\n";
                        pData += "Content-Disposition: form-data; name=\"" + label + "\"\r\n";
                        pData += "\r\n";
                        pData += UrlCharCode(value, uCode) + "\r\n";
                    }
                }
                else
                {
                    pData += label + "=" + UrlCharCode(value,uCode) + "&";
                }
            }

            if (isMutilData == true)
            {
                pData += "--" + boundaryValue + "--\r\n";
            }
            else
            {
                if (isMutilData == false && pData.EndsWith("&"))
                    pData = pData.Substring(0, pData.Length - 1);
            }

            //pData = pData.Substring(0, pData.Length - 1);

            

            string login_Url = LoginUrl;

            string strHeader = "";
            
            strHeader = "User-Agent: Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0\r\n";
            strHeader += "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\r\n";
            if (isMutilData==true )
                strHeader += "Content-Type: multipart/form-data; boundary=" + boundaryValue + "\r\n";
            else
                strHeader += "Content-Type:application/x-www-form-urlencoded\r\n";
            strHeader += "Accept-Language:zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3\r\n";
            strHeader += "Accept-Encoding:gzip,deflate\r\n";
            strHeader += "Connection: Keep-Alive\r\n";
            strHeader += "Pragma: no-cache\r\n";
            strHeader += "Cache-Control: no-cache\r\n";

            int pDataLen = 0;

            switch (uCode)
            {
                case NetMiner.Resource.cGlobalParas.WebCode.auto:
                    pDataLen = Encoding.ASCII.GetBytes(pData).Length;
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.big5:
                    pDataLen = Encoding.GetEncoding("big5").GetBytes(pData).Length;
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.gb2312:
                    pDataLen = Encoding.GetEncoding("gb2312").GetBytes(pData).Length;
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.gbk:
                    pDataLen = Encoding.GetEncoding("gbk").GetBytes(pData).Length;
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.NoCoding:
                    pDataLen = Encoding.ASCII.GetBytes(pData).Length;
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.utf8:
                    pDataLen = Encoding.UTF8.GetBytes(pData).Length;
                    break;
            }

            if (LoginRurl != "")
                strHeader += "Referer: " + LoginRurl + "\r\n";
            if (strCookie != "")
                strHeader += "Cookie:" + strCookie + "\r\n";
            strHeader += "Content-length: " + pDataLen + "\r\n\r\n";
            strHeader += pData;  // +"\r\n";

            

            webSource = GetUrl(login_Url, strHeader, "POST", uCode ,"",0, out  cookie);
            strCookie = cookie.Trim ();
            wSource = webSource;

            //根据源码判断是否登录成功
            bool isS = false;
            if (webSource.ToLower().Contains(successFlag.ToLower()))
                isS = true;
            else
            {
                isS = false;
                return isS;
            }

            #endregion

            //根据任务判断是否需要进行解析全局参数


            #region 解析是否要进行跳转操作 跳转的目的是为了获取cookie

            //解析返回的websource中是否有url地址，如果存在，则需要再次解析，进行cookie的获取
            webSource =webSource .Replace ("\\/","/");
            webSource = webSource.Replace("\0", "");

            //判断源码中是否有地址，通过扩展名进行判断吧，一个是aspx、php、jsp、asp
            if (webSource.Length > 1024 && isS ==true )
            {
                //表明返回的是源码，无需跳转
                return isS;
            }

            //开始进行跳转操作
            //先将单引号替换成双引号
            webSource =webSource .Replace ("'","\"");
            string reg1="";
            if (webSource.IndexOf("aspx")>0)
                reg1 = "[\"][^\"]+?aspx[^\"]*[\"]";
            else if (webSource .IndexOf ("php")>0)
                reg1 = "[\"][^\"]+?php[^\"]*[\"]";
            else if (webSource.IndexOf("jsp") > 0)
                reg1 = "[\"][^\"]+?jsp[^\"]*[\"]";
            else if (webSource.IndexOf("asp") > 0)
                reg1 = "[\"][^\"]+?asp[^\"]*[\"]";

            Match a2 = Regex.Match(webSource, reg1, RegexOptions.IgnoreCase);
            string url1 = a2.Groups[0].Value.ToString();
            url1=url1.Replace ("\"","");

            if (url1 == "")
                return isS;

            UrlAnaly:
            if (!url1.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
            {
                string PreUrl = login_Url;


                if (url1.Substring(0, 1) == "/" || url1.Substring(0, 1) == "\\")
                {
                    PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                    if (PreUrl.IndexOf("/") > 0)
                        PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));

                    PreUrl = "http://" + PreUrl;
                }
                else if (url1.StartsWith("?"))
                {
                    PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                    if (PreUrl.IndexOf("?") > 0)
                        PreUrl = PreUrl.Substring(0, PreUrl.LastIndexOf("?"));

                    PreUrl = "http://" + PreUrl;
                }
                else
                {
                    Match a1 = Regex.Match(PreUrl, ".*/");
                    PreUrl = a1.Groups[0].Value.ToString();
                }
                url1 = PreUrl + url1;
            }
            else
            {
                if (url1.StartsWith("http:///", StringComparison.CurrentCultureIgnoreCase) ||
                    url1.StartsWith("http://.", StringComparison.CurrentCultureIgnoreCase))
                {
                    url1 = url1.Substring(8, url1.Length - 8);
                    goto UrlAnaly;
                }
            }

            strHeader = "User-Agent: Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0\r\n";
            strHeader += "Accept: */*; q=0.01\r\n";
            strHeader += "Accept-Language:zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3\r\n";
            strHeader += "Accept-Encoding:gzip,deflate\r\n";
           
            if (strCookie != "")
                strHeader += "Cookie:" + strCookie + "\r\n";

            strHeader += "\r\n";

            //此次登录的主要目的是更新cookie
            string webSource1= GetUrl(url1, strHeader, "GET", cGlobalParas.WebCode.NoCoding,"",0, out  cookie);
            wSource = webSource;
            #endregion

            return isS;
        }

        public static string GetUrl1(string url, string strheaders, string method, cGlobalParas.WebCode uCode,string HttpPeoxy, out string cookie)
        {

            MyWebRequest request = null;
            request = MyWebRequest.Create(new Uri(url), request, true,false ,ProxyType.None);
            request.Method = method;
            request.Header = strheaders;

            //request.Timeout = 30;

            MyWebResponse response = request.GetResponse(uCode);

            //获取cookie
            string cEncoding = "utf-8";
            string webCode = "utf-8";
            bool isChunked = false;

            string strCookie = "";
            if (response.Header != null)
            {
                string[] headers = Regex.Split(response.Header, "\r\n");
                for (int i = 0; i < headers.Length; i++)
                {

                    if (headers[i].IndexOf("cookie", StringComparison.CurrentCultureIgnoreCase) > -1)
                    {
                        string tmpStr1 = headers[i];
                        if (tmpStr1.IndexOf(";") < 0)
                            tmpStr1 += ";";
                        strCookie += tmpStr1.Substring(tmpStr1.IndexOf(":") + 1, tmpStr1.IndexOf(";") - tmpStr1.IndexOf(":"));

                    }
                }
                if (strCookie.EndsWith(";"))
                {
                    strCookie = strCookie.Substring(0, strCookie.Length - 1);
                }

                //开始接收源码信息
                for (int i = 0; i < headers.Length; i++)
                {
                    string ss = headers[i].ToString();
                    if (ss.StartsWith("Content-Encoding"))
                    {
                        cEncoding = ss.Substring(17, ss.Length - 17);

                    }
                    if (ss.StartsWith("Content-Type"))
                    {
                        Match charSetMatch = Regex.Match(ss, "(?<=charset=)([^<]+?)(?=$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        webCode = charSetMatch.ToString();
                    }
                    if (ss.StartsWith("Transfer-Encoding"))
                    {
                        string s1 = ss.Substring(18, ss.Length - 18);
                        if (s1.IndexOf("chunked") > -1)
                        {
                            isChunked = true;
                        }
                    }
                }
            }

            //开始接收网页数据
            string webSource = "";
            //byte[] RecvBuffer = new byte[response.ContentLength ];
            //response.socket.Receive(RecvBuffer, 0, response.ContentLength, System.Net.Sockets.SocketFlags.None);

            byte[] RecvBuffer = new byte[1024];
            int nBytes, nTotalBytes = 0;

            byte[] sourceByte = new byte[response.ContentLength + 1024];

            while ((nBytes = response.socket.Receive(RecvBuffer, 0, 1024, System.Net.Sockets.SocketFlags.None)) > 0)
            {
                //System.Threading.Thread.Sleep(500);

                try
                {
                    RecvBuffer.CopyTo(sourceByte, nTotalBytes);
                }
                catch
                {
                    break;
                }

                nTotalBytes += nBytes;



                if ( nTotalBytes >= response.ContentLength && response.ContentLength > 0)
                    break;
            }


            webSource += ConvertSource(sourceByte, cEncoding, webCode, isChunked);


            //webSource = ConvertSource(RecvBuffer, cEncoding, webCode, isChunked);

            response.Close();
            request = null;

            string c1 = "";
            string[] inHeaders = Regex.Split(strheaders, "\r\n");
            for (int i = 0; i < inHeaders.Length; i++)
            {
                if (inHeaders[i] != "")
                {
                    if (inHeaders[i].Trim().StartsWith("cookie", StringComparison.CurrentCultureIgnoreCase))
                    {
                        c1 += inHeaders[i].Substring(inHeaders[i].IndexOf(":") + 1, inHeaders[i].Length
                            - inHeaders[i].IndexOf(":") - 1);
                    }
                }
            }

            string[] oldCookie = c1.Split(';');
            string[] newCookie = strCookie.Split(';');


            //先把旧cookie添加到一个hashtable中
            Hashtable cookieTable = new Hashtable();
            for (int j = 0; j < oldCookie.Length; j++)
            {
                if (oldCookie[j].ToString() != "")
                {
                    string label = oldCookie[j].Substring(0, oldCookie[j].IndexOf("="));
                    string value = oldCookie[j].Substring(oldCookie[j].IndexOf("=") + 1, oldCookie[j].Length - oldCookie[j].IndexOf("=") - 1);
                    cookieTable.Add(label, value);
                }
            }

            for (int i = 0; i < newCookie.Length; i++)
            {
                if (newCookie[i].ToString() != "")
                {
                    string label = newCookie[i].Substring(0, newCookie[i].IndexOf("="));
                    string value = newCookie[i].Substring(newCookie[i].IndexOf("=") + 1, newCookie[i].Length - newCookie[i].IndexOf("=") - 1);
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

            //将cookie转换成字符串
            string rCookie = "";
            foreach (DictionaryEntry de in cookieTable)
            {
                rCookie += de.Key + "=" + de.Value + ";";
            }
            if (rCookie != "")
                rCookie = rCookie.Substring(0, rCookie.Length - 1);

            cookie = rCookie;
            return webSource;
        }

        public static string GetUrl(string url, string strheaders, string method, cGlobalParas.WebCode uCode, string HttpProxy,int HttpProxyPort, out string cookie)
        {

            if (strheaders == "")
            {
                strheaders = "User-Agent: Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0\r\n\r\n";
            }

            Uri u = null;
            MyWebRequest request = null;
            if (HttpProxy != "")
            {
                
                //表示的是http代理访问
                u = new Uri(url);
                request = MyWebRequest.Create(u, request, true, true, ProxyType.HttpProxy);
                request.httpProxyAddress = HttpProxy;
                request.httpProxyPort = HttpProxyPort;

            }
            else
            {
                u = new Uri(url);
                request = MyWebRequest.Create(u, request, true, false, ProxyType.None);
            }

            
            
            request.Method = method;
            request.Header = strheaders;

            //request.Timeout = 30;

            MyWebResponse response = request.GetResponse(uCode);

            //获取cookie
            string cEncoding = "utf-8";
            string webCode = "utf-8";
            bool isChunked = false;

            string strCookie = "";

            //string ss = System.Text.Encoding.ASCII.GetString(Fiddler.Utilities.doUnchunk(response.HeaderBytes));

            if (response.Header != null)
            {
                string[] headers = Regex.Split(response.Header, "\r\n");
                for (int i = 0; i < headers.Length; i++)
                {

                    if (headers[i].IndexOf("cookie", StringComparison.CurrentCultureIgnoreCase) > -1)
                    {
                        string tmpStr1 = headers[i];
                        if (tmpStr1.IndexOf("expires") > 0)
                        {
                            //表示此cookie设置了过期时间，将时间提取出来

                            Match charSetMatch = Regex.Match(tmpStr1 + ";", "(?<=expires=)([^;]+?)(?=;)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            string estrDate = charSetMatch.ToString();

                            try
                            {
                                DateTime eDate = DateTime.Parse(estrDate);
                                if (eDate > System.DateTime.Now)
                                {
                                    if (tmpStr1.IndexOf(";") < 0)
                                        tmpStr1 += ";";

                                    strCookie += tmpStr1.Substring(tmpStr1.IndexOf(":") + 1, tmpStr1.IndexOf(";") - tmpStr1.IndexOf(":"));
                                }
                            }
                            catch 
                            {
                                
                            }
                        }
                        else
                        {
                            if (tmpStr1.IndexOf(";") < 0)
                                tmpStr1 += ";";

                            strCookie += tmpStr1.Substring(tmpStr1.IndexOf(":") + 1, tmpStr1.IndexOf(";") - tmpStr1.IndexOf(":"));
                        }
                        

                    }
                }
                if (strCookie.EndsWith(";"))
                {
                    strCookie = strCookie.Substring(0, strCookie.Length - 1);
                }

                //开始接收源码信息
                for (int i = 0; i < headers.Length; i++)
                {
                    string ss = headers[i].ToString();
                    if (ss.StartsWith("Content-Encoding",StringComparison.CurrentCultureIgnoreCase))
                    {
                        cEncoding = ss.Substring(17, ss.Length - 17);

                    }
                    if (ss.StartsWith("Content-Type", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Match charSetMatch = Regex.Match(ss, "(?<=charset=)([^<]+?)(?=$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        webCode = charSetMatch.ToString();
                    }
                    if (ss.StartsWith("Transfer-Encoding", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string s1 = ss.Substring(18, ss.Length - 18);
                        if (s1.IndexOf("chunked") > -1)
                        {
                            isChunked = true;
                        }
                    }
                }
            }

            //开始接收网页数据
            string webSource = "";

            byte[] RecvBuffer = new byte[1024];
            int nBytes, nTotalBytes = 0;

            byte[] sourceByte = null;

            if (isChunked == true)
            {
                byte[] b1 = new byte[1];
                string sLen = "";
                List<byte> ResponseBytesList = new List<byte>();

                while (response.socket.Receive(b1, 0, 1, System.Net.Sockets.SocketFlags.None) > 0)
                {
                    sLen += Encoding.ASCII.GetString(b1, 0, 1);

                    if (b1[0] == '\n' && sLen.EndsWith("0\r\n\r\n"))
                        break;

                    if (b1[0] == '\n' && sLen.EndsWith("\r\n"))
                    {
                        if (sLen == "0\r\n")
                        {
                            //表示有可能是结束符
                        }
                        else
                        {
                            sLen = sLen.Substring(0, sLen.Length - 2);

                            if (sLen != "")
                            {
                                int iLen = Convert.ToInt32(sLen, 16);

                                byte[] tByte = new byte[iLen];
                                //System.Threading.Thread.Sleep(1000);

                                //if (response.socket.Receive(tByte,0, (int)iLen, System.Net.Sockets.SocketFlags.None) < 0)
                                //    break;
                                //tByte = GetReceiveByte(iLen, ref response);
                                for (int ti = 0; ti < iLen; ti++)
                                {
                                    byte[] b2 = new byte[1];
                                    response.socket.Receive(b2, 0, 1, System.Net.Sockets.SocketFlags.None);
                                    tByte[ti] = b2[0];
                                }


                                if (tByte == null)
                                    break;

                                ResponseBytesList.AddRange(tByte);
                                sLen = "";
                            }
                        }
                    }


                }

                sourceByte = new byte[ResponseBytesList.Count];
                sourceByte = ResponseBytesList.ToArray();

            }
            else
            {
                int cLen = response.ContentLength;
                if (cLen == 0)
                {
                    sourceByte = new byte[1];
                    RecvBuffer = new byte[1];
                    List<byte> tmpBuffer = new List<byte>();
                    while ((nBytes = response.socket.Receive(RecvBuffer, 0, 1, System.Net.Sockets.SocketFlags.None)) > 0)
                    {
                        if (response.socket.Connected==false )
                            break;

                        tmpBuffer.AddRange(RecvBuffer);
                    }
                    sourceByte = new byte[tmpBuffer.Count];
                    sourceByte = tmpBuffer.ToArray();
                }
                else
                {
                    sourceByte = new byte[cLen];
                    int iLen = 1024;
                    if (iLen > cLen)
                    {
                        iLen = cLen;
                        RecvBuffer = new byte[iLen];
                    }

                    while ((nBytes = response.socket.Receive(RecvBuffer, 0, iLen, System.Net.Sockets.SocketFlags.None)) > 0)
                    {
                        //System.Threading.Thread.Sleep(500);

                        try
                        {
                            RecvBuffer.CopyTo(sourceByte, nTotalBytes);
                        }
                        catch
                        {
                            break;
                        }

                        nTotalBytes += nBytes;

                        if (nTotalBytes >= response.ContentLength && response.ContentLength > 0)
                            break;

                        if (cLen - nTotalBytes < iLen)
                        {
                            iLen = cLen - nTotalBytes;
                            RecvBuffer = new byte[iLen];
                        }

                    }
                }
            }

            webSource += ConvertSource(sourceByte, cEncoding, webCode, false);


            //webSource = ConvertSource(RecvBuffer, cEncoding, webCode, isChunked);

            response.Close();
            request = null;

            string c1 = "";
            string[] inHeaders = Regex.Split(strheaders, "\r\n");
            for (int i = 0; i < inHeaders.Length; i++)
            {
                if (inHeaders[i] != "")
                {
                    if (inHeaders[i].Trim().StartsWith("cookie", StringComparison.CurrentCultureIgnoreCase))
                    {
                        c1 += inHeaders[i].Substring(inHeaders[i].IndexOf(":") + 1, inHeaders[i].Length
                            - inHeaders[i].IndexOf(":") - 1);
                    }
                }
            }

            string[] oldCookie = c1.Split(';');
            string[] newCookie = strCookie.Split(';');


            //先把旧cookie添加到一个hashtable中
            Hashtable cookieTable = new Hashtable();
            for (int j = 0; j < oldCookie.Length; j++)
            {
                if (oldCookie[j].ToString() != "")
                {
                    string label = oldCookie[j].Substring(0, oldCookie[j].IndexOf("="));
                    if (label.ToLower ().Trim()!="no-cache")
                    {
                        string value = oldCookie[j].Substring(oldCookie[j].IndexOf("=") + 1, oldCookie[j].Length - oldCookie[j].IndexOf("=") - 1);
                        cookieTable.Add(label, value);
                    }
                }
            }

            for (int i = 0; i < newCookie.Length; i++)
            {
                if (newCookie[i].ToString() != "")
                {
                    string label = newCookie[i].Substring(0, newCookie[i].IndexOf("="));
                    if (label.ToLower().Trim() != "no-cache")
                    {
                        string value = newCookie[i].Substring(newCookie[i].IndexOf("=") + 1, newCookie[i].Length - newCookie[i].IndexOf("=") - 1);
                        if (cookieTable.ContainsKey(label.Trim()))
                        {
                            cookieTable.Remove(label.Trim ());
                            cookieTable.Add(label, value);
                        }
                        else
                        {
                            cookieTable.Add(label, value);
                        }
                    }
                }
            }

            //将cookie转换成字符串
            string rCookie = "";
            foreach (DictionaryEntry de in cookieTable)
            {
                rCookie += de.Key + "=" + de.Value + ";";
            }
            if (rCookie != "")
                rCookie = rCookie.Substring(0, rCookie.Length - 1);

            cookie = rCookie;
            return webSource;
        }

        private static string ConvertSource(byte[] bytes, string cEncoding, string webCode, bool isChunked)
        {
            Encoding wCode;
            string strWebData = "";

            try
            {
                if (isChunked == true)
                    bytes = doUnchunk(bytes);

                Stream respStream = new MemoryStream(bytes);

                if (cEncoding.ToLower().IndexOf("gzip") > -1)
                {

                    GZipStream myGZip = new GZipStream(respStream, CompressionMode.Decompress);

                    //定义一个字节数组
                    byte[] buffer = new byte[0x400];

                    //定义一个流，将数据读出来
                    MemoryStream mReader = new MemoryStream();
                    for (int i = myGZip.Read(buffer, 0, buffer.Length); i > 0; i = myGZip.Read(buffer, 0, buffer.Length))
                    {
                        mReader.Write(buffer, 0, i);
                    }
                    myGZip.Close();

                    string strChar = Encoding.ASCII.GetString(mReader.ToArray());
                    if (webCode == "")
                    {
                        Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string webCharSet = charSetMatch.ToString();
                        if (webCharSet != "")
                            wCode = System.Text.Encoding.GetEncoding(webCharSet);
                        else
                            wCode = Encoding.Default;
                    }
                    else
                    {
                        wCode = Encoding.GetEncoding(webCode);
                    }

                    mReader.Seek((long)0, SeekOrigin.Begin);
                    StreamReader reader = new StreamReader(mReader, wCode);
                    strWebData = reader.ReadToEnd();

                    mReader.Close();
                    mReader.Dispose();
                    reader.Close();
                    reader.Dispose();


                }
                else if (cEncoding.ToLower().IndexOf("deflate") > -1)
                {

                    DeflateStream myDeflate = new DeflateStream(respStream, CompressionMode.Decompress);

                    //定义一个字节数组
                    byte[] buffer = new byte[0x400];

                    //定义一个流，将数据读出来
                    MemoryStream mReader = new MemoryStream();
                    for (int i = myDeflate.Read(buffer, 0, buffer.Length); i > 0; i = myDeflate.Read(buffer, 0, buffer.Length))
                    {
                        mReader.Write(buffer, 0, i);
                    }
                    myDeflate.Close();

                    string strChar = Encoding.ASCII.GetString(mReader.ToArray());

                    if (webCode == "")
                    {
                        Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string webCharSet = charSetMatch.ToString();
                        if (webCharSet != "")
                            wCode = System.Text.Encoding.GetEncoding(webCharSet);
                        else
                            wCode = Encoding.Default;
                    }
                    else
                    {
                        wCode = Encoding.GetEncoding(webCode);
                    }

                    mReader.Seek((long)0, SeekOrigin.Begin);
                    StreamReader reader = new StreamReader(mReader, wCode);
                    strWebData = reader.ReadToEnd();

                    mReader.Close();
                    mReader.Dispose();
                    reader.Close();
                    reader.Dispose();

                }
                else
                {

                    //定义一个字节数组
                    byte[] buffer = new byte[0x400];

                    //定义一个流，将数据读出来
                    MemoryStream mReader = new MemoryStream();
                    for (int i = respStream.Read(buffer, 0, buffer.Length); i > 0; i = respStream.Read(buffer, 0, buffer.Length))
                    {
                        mReader.Write(buffer, 0, i);
                    }

                    string strChar = Encoding.ASCII.GetString(mReader.ToArray());

                    if (webCode == "")
                    {
                        Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        string webCharSet = charSetMatch.ToString();
                        webCharSet = Regex.Replace(webCharSet, "[\"|']", "");
                        if (webCharSet != "")
                            wCode = System.Text.Encoding.GetEncoding(webCharSet);
                        else
                            wCode = Encoding.Default;
                    }
                    else
                    {
                        wCode = Encoding.GetEncoding(webCode);
                    }

                    mReader.Seek((long)0, SeekOrigin.Begin);
                    StreamReader reader = new StreamReader(mReader, wCode);
                    strWebData = reader.ReadToEnd();

                    mReader.Close();
                    mReader.Dispose();
                    reader.Close();
                    reader.Dispose();

                }
            }
            catch
            {
            }

            return strWebData;
        }

        public static string UploadFile(string url,string strHeader,cGlobalParas.WebCode uCode,string FileName)
        {
            MyWebRequest request = null;
            request = MyWebRequest.Create(new Uri(url), request, true, false, ProxyType.None);
            request.Method = "POST";
            request.Header = strHeader;
            request.Timeout = 30;

            //request.IsProxy = true;
            //request.httpProxyAddress = "127.0.0.1";
            //request.httpProxyPort = 8888;

            MyWebResponse response = request.GetResponseUpload(uCode, FileName);

            //获取cookie
            string cEncoding = "utf-8";
            string webCode = "utf-8";
            bool isChunked = false;

            string strCookie = "";
            if (response.Header != null)
            {
                string[] headers = Regex.Split(response.Header, "\r\n");
                for (int i = 0; i < headers.Length; i++)
                {

                    if (headers[i].IndexOf("cookie", StringComparison.CurrentCultureIgnoreCase) > 0)
                    {
                        strCookie += headers[i].Substring(headers[i].IndexOf(":") + 1, headers[i].IndexOf(";") - headers[i].IndexOf(":"));
                    }
                }
                if (strCookie.EndsWith(";"))
                {
                    strCookie = strCookie.Substring(0, strCookie.Length - 1);
                }

                //开始接收源码信息
                for (int i = 0; i < headers.Length; i++)
                {
                    string ss = headers[i].ToString();
                    if (ss.StartsWith("Content-Encoding"))
                    {
                        cEncoding = ss.Substring(17, ss.Length - 17);

                    }
                    if (ss.StartsWith("Content-Type"))
                    {
                        Match charSetMatch = Regex.Match(ss, "(?<=charset=)([^<]+?)(?=$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        webCode = charSetMatch.ToString();
                    }
                    if (ss.StartsWith("Transfer-Encoding"))
                    {
                        string s1 = ss.Substring(18, ss.Length - 18);
                        if (s1.IndexOf("chunked") > -1)
                        {
                            isChunked = true;
                        }
                    }
                }
            }
            byte[] RecvBuffer = new byte[response.socket.ReceiveBufferSize];
            //response.socket.re
            response.socket.Receive(RecvBuffer, 0, response.socket.ReceiveBufferSize, System.Net.Sockets.SocketFlags.None);


            string webSource = ConvertSource(RecvBuffer, cEncoding, webCode, isChunked);

            response.Close();
            request = null;

            return webSource;
        }

        public static byte[] doUnchunk(byte[] writeData)
        {
            if (writeData == null || writeData.Length == 0)
            {
                return new byte[0];
            }

            MemoryStream memoryStream = new MemoryStream(writeData.Length);
            int num = 0;
            bool flag = false;
            while (!flag && num <= writeData.Length - 3)
            {
                string @string = Encoding.ASCII.GetString(writeData, num, Math.Min(64, writeData.Length - num));
                int num2 = @string.IndexOf("\r\n", StringComparison.Ordinal);
                if (num2 > 0)
                {
                    num += num2 + 2;
                    @string = @string.Substring(0, num2);
                    num2 = @string.IndexOf(';');
                    if (num2 > 0)
                    {
                        @string = @string.Substring(0, num2);
                    }
                   
                    if (!int.TryParse(@string, out var iOutput))
                    {
                        throw new InvalidDataException("HTTP Error: The chunked content is corrupt. Chunk Length was malformed. Offset: " + num);
                    }

                    if (iOutput == 0)
                    {
                        flag = true;
                        continue;
                    }

                    if (writeData.Length < iOutput + num)
                    {
                        throw new InvalidDataException("HTTP Error: The chunked entity body is corrupt. The final chunk length is greater than the number of bytes remaining.");
                    }

                    memoryStream.Write(writeData, num, iOutput);
                    num += iOutput + 2;
                    continue;
                }

                throw new InvalidDataException("HTTP Error: The chunked content is corrupt. Cannot find Chunk-Length in expected location. Offset: " + num);
            }

            byte[] array = new byte[memoryStream.Length];
            Buffer.BlockCopy(memoryStream.GetBuffer(), 0, array, 0, array.Length);
            return array;
        }

        //public static string GetImage(string workPath, string url,ref string cookie)
        //{
        //    string strheaders = "";
        //    strheaders = "User-Agent: Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0\r\n";
        //    if (cookie != "")
        //        strheaders += "Cookie:" + cookie + "\r\n";
        //    strheaders += "\r\n";

        //    MyWebRequest request = null;
        //    request = MyWebRequest.Create(new Uri(url), request, true, false, ProxyType.None);
        //    request.Method = "GET";
        //    request.Header = strheaders;

        //    MyWebResponse response = request.GetResponse(cGlobalParas.WebCode.NoCoding );

        //    //获取cookie
        //    string cEncoding = "utf-8";
        //    string webCode = "utf-8";
        //    bool isChunked = false;

        //    string strCookie = "";
        //    if (response.Header != null)
        //    {
        //        string[] headers = Regex.Split(response.Header, "\r\n");
        //        for (int i = 0; i < headers.Length; i++)
        //        {

        //            if (headers[i].IndexOf("cookie", StringComparison.CurrentCultureIgnoreCase) > -1)
        //            {
        //                strCookie += headers[i].Substring(headers[i].IndexOf(":") + 1, headers[i].IndexOf(";") - headers[i].IndexOf(":"));

        //            }
        //        }
        //        if (strCookie.EndsWith(";"))
        //        {
        //            strCookie = strCookie.Substring(0, strCookie.Length - 1);
        //        }

        //        //开始接收源码信息
        //        for (int i = 0; i < headers.Length; i++)
        //        {
        //            string ss = headers[i].ToString();
        //            if (ss.StartsWith("Content-Encoding"))
        //            {
        //                cEncoding = ss.Substring(17, ss.Length - 17);

        //            }
        //            if (ss.StartsWith("Content-Type"))
        //            {
        //                Match charSetMatch = Regex.Match(ss, "(?<=charset=)([^<]+?)(?=$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //                webCode = charSetMatch.ToString();
        //            }
        //            if (ss.StartsWith("Transfer-Encoding"))
        //            {
        //                string s1 = ss.Substring(18, ss.Length - 18);
        //                if (s1.IndexOf("chunked") > -1)
        //                {
        //                    isChunked = true;
        //                }
        //            }
        //        }
        //    }

        //    //开始接收网页数据
        //    string webSource = "";
        //    //byte[] RecvBuffer = new byte[response.ContentLength ];
        //    //response.socket.Receive(RecvBuffer, 0, response.ContentLength, System.Net.Sockets.SocketFlags.None);

        //    byte[] RecvBuffer = new byte[1024];

        //    if (!Directory.Exists(workPath + "data\\"))
        //    {
        //        Directory.CreateDirectory(workPath + "data\\");
        //    }

        //    string fName = workPath + "data\\" + "~vCode.png";


        //    int nBytes, nTotalBytes = 0;
        //    byte[] imgBytes;

        //    if (response.ContentLength == 0)
        //    {
        //        List<byte> tmpBuffer = new List<byte>();

        //        if (isChunked == true)
        //        {
        //            byte[] b1 = new byte[1];
        //            string sLen = "";

        //            while (response.socket.Receive(b1, 0, 1, System.Net.Sockets.SocketFlags.None) > 0)
        //            {
        //                sLen += Encoding.ASCII.GetString(b1, 0, 1);

        //                if (b1[0] == '\n' && sLen.EndsWith("0\r\n\r\n"))
        //                    break;

        //                if (b1[0] == '\n' && sLen.EndsWith("\r\n"))
        //                {
        //                    if (sLen == "0\r\n")
        //                    {
        //                        //表示有可能是结束符
        //                    }
        //                    else
        //                    {
        //                        sLen = sLen.Substring(0, sLen.Length - 2);

        //                        if (sLen != "")
        //                        {
        //                            int iLen = Convert.ToInt32(sLen, 16);

        //                            byte[] tByte = new byte[iLen];
        //                            for (int ti = 0; ti < iLen; ti++)
        //                            {
        //                                byte[] b2 = new byte[1];
        //                                response.socket.Receive(b2, 0, 1, System.Net.Sockets.SocketFlags.None);
        //                                tByte[ti] = b2[0];
        //                            }


        //                            if (tByte == null)
        //                                break;

        //                            tmpBuffer.AddRange(tByte);
        //                            sLen = "";
        //                        }
        //                    }
        //                }


        //            }

        //        }
        //        else
        //        {
        //            RecvBuffer = new byte[1];

        //            while ((nBytes = response.socket.Receive(RecvBuffer, 0, 1, System.Net.Sockets.SocketFlags.None)) > 0)
        //            {
        //                if (response.socket.Connected == false)
        //                    break;

        //                tmpBuffer.AddRange(RecvBuffer);

        //            }

        //        }
        //        imgBytes = new byte[tmpBuffer.Count];
        //        imgBytes = tmpBuffer.ToArray();
        //    }
        //    else
        //    {
        //        #region  接受content-length可以表明数量的图片
        //        int cLen = response.ContentLength;

        //        imgBytes = new byte[cLen];
        //        int iLen = 1024;
        //        if (iLen > cLen)
        //        {
        //            iLen = cLen;
        //            RecvBuffer = new byte[iLen];
        //        }

        //        while ((nBytes = response.socket.Receive(RecvBuffer, 0, iLen, System.Net.Sockets.SocketFlags.None)) > 0)
        //        {

        //            //开始解析成源码
        //            RecvBuffer.CopyTo(imgBytes, nTotalBytes);

        //            nTotalBytes += nBytes;

        //            if (nTotalBytes >= response.ContentLength && response.ContentLength > 0)
        //                break;

        //            if (cLen - nTotalBytes < iLen)
        //            {
        //                iLen = cLen - nTotalBytes;
        //                RecvBuffer = new byte[iLen];
        //            }

        //        }
        //        #endregion
        //    }

        //    //if (isChunked == true)
        //    //{
        //    //   imgBytes= Fiddler.Utilities.doUnchunk(imgBytes);

        //    //}

        //    FileStream fs = new FileStream(fName, FileMode.Create, FileAccess.Write);
        //    BinaryWriter bw = new BinaryWriter(fs);
        //    bw.Write(imgBytes);

        //    bw.Close();
        //    fs.Close();
        //    fs.Dispose();

        //    response.Close();
        //    request = null;

        //    string c1 = cookie;
        //    string[] oldCookie = c1.Split(';');
        //    string[] newCookie = strCookie.Split(';');

        //    //先把旧cookie添加到一个hashtable中
        //    Hashtable cookieTable = new Hashtable();
        //    for (int j = 0; j < oldCookie.Length; j++)
        //    {
        //        if (oldCookie[j].ToString() != "")
        //        {
        //            string label = oldCookie[j].Substring(0, oldCookie[j].IndexOf("="));
        //            string value = oldCookie[j].Substring(oldCookie[j].IndexOf("=") + 1, oldCookie[j].Length - oldCookie[j].IndexOf("=") - 1);
        //            cookieTable.Add(label, value);
        //        }
        //    }

        //    for (int i = 0; i < newCookie.Length; i++)
        //    {
        //        if (newCookie[i].ToString() != "")
        //        {
        //            string label = newCookie[i].Substring(0, newCookie[i].IndexOf("="));
        //            string value = newCookie[i].Substring(newCookie[i].IndexOf("=") + 1, newCookie[i].Length - newCookie[i].IndexOf("=") - 1);
        //            if (cookieTable.ContainsKey(label))
        //            {
        //                cookieTable.Remove(label);
        //                cookieTable.Add(label, value);
        //            }
        //            else
        //            {
        //                cookieTable.Add(label, value);
        //            }
        //        }
        //    }

        //    //将cookie转换成字符串
        //    string rCookie = "";
        //    foreach (DictionaryEntry de in cookieTable)
        //    {
        //        rCookie += de.Key + "=" + de.Value + ";";
        //    }
        //    if (rCookie != "")
        //        rCookie = rCookie.Substring(0, rCookie.Length - 1);



        //    cookie = rCookie;

        //    return fName;
        //}

        public static string GetImage(string workPath, string url, ref string cookie)
        {
            string fName = workPath + "data\\" + "~vCode.png";
            string strCookie=string.Empty ;

            System.Net.HttpWebRequest wReq = null;
            System.Net.HttpWebResponse wRep = null;
            FileStream SaveFileStream = null;

            int startingPoint = 0;

            //判断目录是否存在
            string fPath = System.IO.Path.GetDirectoryName(fName);
            string fName1 = Path.GetFileName(fName);
            fName1 = Regex.Replace(fName1, "[/|\\|:|*|\\?|\"|<|>|\\|]", "");
            fName = fPath + "\\" + fName1;
            if (Directory.Exists(fPath) == false)
            {
                Directory.CreateDirectory(fPath);
            }

            try
            {
                //For using untrusted SSL Certificates
                string strReferer = "";

                if (url.IndexOf("<POST") > 0)
                {
                    string url1 = url.Substring(0, url.IndexOf("<POST"));
                    wReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url1);
                }
                else
                    wReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);

                //wReq.AddRange(startingPoint);xdwsdcfbgzx  

                #region 通讯header
              
                wReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50215;)";
                wReq.Headers.Add("Accept-Language", "zh-cn,en-us;");
                wReq.KeepAlive = true;
                wReq.Headers.Add("Accept-Encoding", "gzip, deflate");
                Match a = Regex.Match(url, @"(http://).[^/]*[?=/]", RegexOptions.IgnoreCase);

                //string url1 = a.Groups[0].Value.ToString();
                //wReq.Referer = url1;

                #endregion

                wReq.AllowAutoRedirect = true;

                CookieContainer CookieCon = new CookieContainer();
                if (cookie != "")
                {
                    CookieCollection cl = new CookieCollection();

                    foreach (string sc in cookie.Split(';'))
                    {
                        string ss = sc.Trim();


                        string s1 = ss.Substring(0, ss.IndexOf("="));
                        string s2 = ss.Substring(ss.IndexOf("=") + 1, ss.Length - ss.IndexOf("=") - 1);

                        if (s2.IndexOf(",") > 0 || s2.IndexOf(";") > 0)
                        {
                            s2 = s2.Replace(",", "%2c");
                            s2 = s2.Replace(";", "%3b");
                        }

                        cl.Add(new Cookie(s1, s2, "/"));
                    }


                    CookieCon.Add(new Uri(url), cl);
                    wReq.CookieContainer = CookieCon;
                }

                #region POST数据

                //判断是否含有POST参数
                if (Regex.IsMatch(url, @"(?<=<POST[^>]*>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase))
                {

                    Match s = Regex.Match(url, @"(?<=<POST[^>]*>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase);
                    string PostPara = s.Groups[0].Value.ToString();
                    byte[] pPara;

                    s = Regex.Match(url, @"(?<=<POST)[^>]*(?=>)", RegexOptions.IgnoreCase);
                    string postCode = s.Groups[0].Value.ToString();

                    if (postCode != "")
                        postCode = postCode.Substring(1, postCode.Length - 1).ToLower();

                    if (postCode == "" || postCode == "ascii")
                        pPara = Encoding.ASCII.GetBytes(PostPara);
                    else if (postCode == "utf8")
                        pPara = Encoding.UTF8.GetBytes(PostPara);
                    else
                        pPara = Encoding.GetEncoding(postCode).GetBytes(PostPara);


                    if (wReq.ContentType == "")
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

                wRep = (System.Net.HttpWebResponse)wReq.GetResponse();

                Stream responseSteam = wRep.GetResponseStream();

                if (startingPoint == 0)
                {
                    SaveFileStream = new FileStream(fName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                }
                else
                {
                    SaveFileStream = new FileStream(fName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                }

                #region 请求数据的编码、压缩格式转换
                System.IO.StreamReader reader;
                if (wRep.ContentEncoding == "gzip")
                {
                    GZipStream myGZip = new GZipStream(responseSteam, CompressionMode.Decompress);
                    reader = new System.IO.StreamReader(myGZip, Encoding.ASCII);
                    //myGZip.Close();
                }
                else if (wRep.ContentEncoding.StartsWith("deflate"))
                {
                    DeflateStream myDeflate = new DeflateStream(responseSteam, CompressionMode.Decompress);
                    reader = new System.IO.StreamReader(myDeflate, Encoding.ASCII);
                    //myDeflate.Close();

                }
                else
                {

                    reader = new System.IO.StreamReader(responseSteam, Encoding.ASCII);

                }



                #endregion


                


                int bytesSize;
                long fileSize = wRep.ContentLength;
                byte[] downloadBuffer = new byte[DEF_PACKET_LENGTH];

                try
                {
                    while ((bytesSize = reader.BaseStream.Read(downloadBuffer, 0, downloadBuffer.Length)) > 0)
                    {

                        SaveFileStream.Write(downloadBuffer, 0, bytesSize);
                    }
                }
                catch { }

                reader.Close();
                reader.Dispose();

                //获取响应的Cookie
                if (wRep.Cookies!=null)
                {
                    for(int i=0;i< wRep.Cookies.Count;i++ )
                    {
                        strCookie += wRep.Cookies[i].Name + "=" + wRep.Cookies[i].Value + ";";
                    }

                    if (!string.IsNullOrEmpty (strCookie))
                        strCookie = strCookie.Substring(0, strCookie.Length - 1);
                }


            }
            catch (System.Exception ex)
            {
                return "";
            }
            finally
            {
                if (SaveFileStream != null)
                {
                    SaveFileStream.Close();
                    SaveFileStream.Dispose();
                }

                if (wRep != null)
                    wRep.Close();
            }


            string c1 = cookie;
            string[] oldCookie = c1.Split(';');
            string[] newCookie = strCookie.Split(';');

            //先把旧cookie添加到一个hashtable中
            Hashtable cookieTable = new Hashtable();
            for (int j = 0; j < oldCookie.Length; j++)
            {
                if (oldCookie[j].ToString() != "")
                {
                    string label = oldCookie[j].Substring(0, oldCookie[j].IndexOf("="));
                    string value = oldCookie[j].Substring(oldCookie[j].IndexOf("=") + 1, oldCookie[j].Length - oldCookie[j].IndexOf("=") - 1);
                    cookieTable.Add(label, value);
                }
            }

            for (int i = 0; i < newCookie.Length; i++)
            {
                if (newCookie[i].ToString() != "")
                {
                    string label = newCookie[i].Substring(0, newCookie[i].IndexOf("="));
                    string value = newCookie[i].Substring(newCookie[i].IndexOf("=") + 1, newCookie[i].Length - newCookie[i].IndexOf("=") - 1);
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

            //将cookie转换成字符串
            string rCookie = "";
            foreach (DictionaryEntry de in cookieTable)
            {
                rCookie += de.Key + "=" + de.Value + ";";
            }
            if (rCookie != "")
                rCookie = rCookie.Substring(0, rCookie.Length - 1);



            cookie = rCookie;

            //FileStream fs = new FileStream(fName, FileMode.Create, FileAccess.Write);
            //BinaryWriter bw = new BinaryWriter(fs);
            //bw.Write(imgBytes);

            //bw.Close();
            //fs.Close();
            //fs.Dispose();

            return fName;

        }


        //internal static bool RemoteCertificateValidationCallback(Object sender,
        // X509Certificate certificate,
        // X509Chain chain,
        // SslPolicyErrors sslPolicyErrors)
        //{
        //    //return true;

        //    #region Validated Message
        //    //如果没有错就表示验证成功
        //    if (sslPolicyErrors == SslPolicyErrors.None)
        //        return true;
        //    else
        //    {
        //        if ((SslPolicyErrors.RemoteCertificateNameMismatch & sslPolicyErrors) == SslPolicyErrors.RemoteCertificateNameMismatch)
        //        {
        //            string errMsg = "证书名称不匹配{0}" + sslPolicyErrors;
        //            Console.WriteLine(errMsg);
        //            throw new NetCredentialException(errMsg);
        //        }

        //        if ((SslPolicyErrors.RemoteCertificateChainErrors & sslPolicyErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
        //        {
        //            string msg = "";
        //            foreach (X509ChainStatus status in chain.ChainStatus)
        //            {
        //                msg += "status code ={0} " + status.Status;
        //                msg += "Status info = " + status.StatusInformation + " ";
        //            }
        //            string errMsg = "证书链错误{0}" + msg;
        //            throw new NetCredentialException(errMsg);
        //        }
        //        string errorMsg = "证书验证失败{0}" + sslPolicyErrors;
        //        Console.WriteLine(errorMsg);
        //        throw new NetCredentialException(errorMsg);
        //    }
        //    #endregion
        //}

        private static void GetVCode(string code,string cookie)
        {
            m_vCode = code;
            m_ImgCookie = cookie;
        }

        /// <summary>
        /// 对特殊字符进行编码
        /// </summary>
        /// <param name="value"></param>
        /// <param name="uCode"></param>
        /// <returns></returns>
        private static string UrlCharCode(string value,cGlobalParas.WebCode uCode)
        {
            value = value.Replace("/", "%2f");
            value = value.Replace("?", "%3f");
            value = value.Replace("\\", "%5c");
            value = value.Replace("+", "%2b");
            value = value.Replace("=", "%3d");
            value = value.Replace("|", "%7c");
            value = value.Replace("&", "%26");
            value = value.Replace("\"", "%22");
            return value;
        }
    }
}

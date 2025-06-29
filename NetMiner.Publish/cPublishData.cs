using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data.OleDb;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
using NetMiner.Common;
using NetMiner.Common.HttpSocket;
using NetMiner.Resource;
using NetMiner.Publish.Rule;
using System.Collections;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Threading;
using NPOI.XWPF.UserModel;
using NetMiner.Core.pTask.Entity;

///修改数据库入库方式，改成参数的形式
///2013-7-7 修改，每次web发布请求url后，都根据返回的头部信息进行cookie的更新
namespace NetMiner.Publish
{
    public class cPublishData
    {
        DataTable m_accessDbType;
        DataTable m_mssqlDbType;
        DataTable m_mysqlDbType;
        private string m_workPath = string.Empty;

        private static char[] constant =   
      { '0','1','2','3','4','5','6','7','8','9'};
        //'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'  
      //};

        public cPublishData(string workPath)
        {
            m_workPath = workPath;
        }

        ~cPublishData()
        {
        }

        #region 导出文件处理
      
        public bool ExportFile(cGlobalParas.PublishType pType, string tName, string fName, DataTable d, bool isHead, bool isRowFile)
        {
            bool isS = true;

            if (isRowFile == true)
            {
                string path = Path.GetDirectoryName(fName);
                string f1 = Path.GetFileNameWithoutExtension(fName);

                for (int i=0;i<d.Rows.Count ;i++)
                {
                    DataTable d1 = d.Clone();
                    object newRow = d.Rows[i].ItemArray.Clone();
                    DataRow r=d1.NewRow ();
                    r.ItemArray =((object[])newRow);
                    d1.Rows.Add (r);

                    int index = 0;
                    while (File.Exists(fName))
                    {
                        fName = path + "\\" + f1 + index.ToString() + Path.GetExtension(fName);
                        index++;
                    }

                    switch (pType)
                    {
                        case cGlobalParas.PublishType.PublishCSV:
                            isS = ExportCSV(tName, fName, d1, isHead);

                            break;
                        case cGlobalParas.PublishType.PublishExcel:
                            isS = ExportExcel(tName, fName, d1, isHead);

                            break;
                        case cGlobalParas.PublishType.PublishTxt:
                            isS = ExportTxt(tName, fName, d1, isHead);

                            break;
                        case cGlobalParas.PublishType.publishWord:
                            isS = ExportWord(tName, fName, d1, isHead);

                            break;
                    }
                }
            }
            else
            {
                switch (pType)
                {
                    case cGlobalParas.PublishType.PublishCSV:
                        isS = ExportCSV(tName, fName, d, isHead);

                        break;
                    case cGlobalParas.PublishType.PublishExcel:
                        isS = ExportExcel(tName, fName, d, isHead);

                        break;
                    case cGlobalParas.PublishType.PublishTxt:
                        isS = ExportTxt(tName, fName, d, isHead);

                        break;
                    case cGlobalParas.PublishType.publishWord:
                        isS = ExportWord(tName, fName, d, isHead);

                        break;
                }

            }

            return isS;
        }

        private bool ExportExcel(string tName, string fName, DataTable d, bool isHead)
        {
            fName = ParseFile(fName);

            if (d.Columns[d.Columns.Count - 1].ColumnName == "isPublished")
            {
                d.Columns.Remove("isPublished");
            }

            try
            {
                IWorkbook workbook = new XSSFWorkbook();

                ISheet sheet1 = workbook.CreateSheet(tName);
                int step = 0;
                if (isHead == true)
                {
                    //插入表头
                    IRow row = sheet1.CreateRow(0);
                    for (int j = 0; j < d.Columns.Count; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.SetCellValue(d.Columns[j].ColumnName);
                    }
                    step = 1;
                }

                
                for (int i = 0; i <d.Rows.Count; i++)
                {
                    IRow row = sheet1.CreateRow(i + step);
                    for (int j = 0; j < d.Columns.Count; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.SetCellValue(d.Rows[i][j].ToString());
                    }
                    
                }

                FileStream myStream = new FileStream(fName, FileMode.Create);
                workbook.Write(myStream);

                myStream.Close();
                myStream.Dispose();
            }
            catch (System.Exception)
            {
                return false;
            }

        
            return true;
        }

        private bool ExportWord(string tName, string fName, DataTable d, bool isHead)
        {
            fName=ParseFile(fName);

            if (d.Columns[d.Columns.Count - 1].ColumnName == "isPublished")
            {
                d.Columns.Remove("isPublished");
            }

            XWPFDocument doc = new XWPFDocument();

            try
            {
                for (int i = 0; i < d.Rows.Count; i++)
                {
                    for (int j = 0; j < d.Columns.Count; j++)
                    {
                        XWPFParagraph p0 = doc.CreateParagraph();
                        XWPFRun r0 = p0.CreateRun();
                        if (isHead ==true )
                            r0.SetText(d.Columns[j].ColumnName + ":" +  d.Rows[i][j].ToString());
                        else
                            r0.SetText(d.Rows[i][j].ToString());
                        r0.SetBold(false);
                        r0.FontFamily="宋体";
                        r0.FontSize=12;
                    }

                    XWPFParagraph p1 = doc.CreateParagraph();
                    XWPFRun r1 = p1.CreateRun();
                    r1.SetText("\r\n");
                }

                FileStream sw = new FileStream(fName, FileMode.Create);
                doc.Write(sw);
                sw.Close();
            }
            catch
            {
                return false;
            }
          
            return true;
        }

        private bool ExportTxt(string tName, string fName, DataTable dData, bool isHead)
        {
            string TaskName = tName;
            string FileName = ParseFile(fName);
            System.Data.DataTable gData = dData;

            //判断目录根据结果并创建
            ToolUtil.CreateDirectory(FileName);

            if (dData.Columns[dData.Columns.Count - 1].ColumnName == "isPublished")
            {
                dData.Columns.Remove("isPublished");
            }


            FileStream myStream = File.Open(FileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            string str = "";
            string tempStr = "";

            try
            {
                if (isHead == true)
                {
                    //写标题 
                    for (int i = 0; i < gData.Columns.Count; i++)
                    {
                        str += gData.Columns[i].ColumnName;
                        str += "\t";
                    }

                    //去掉最后一个制表符
                    str = str.Substring(0, str.Length - 1);

                    sw.WriteLine(str);
                }

                //写内容 
                for (int i = 0; i < gData.Rows.Count; i++)
                {
                    for (int j = 0; j < gData.Columns.Count; j++)
                    {
                        tempStr += gData.Rows[i][j];
                        tempStr += "\t";
                    }

                    //去掉最后一个制表符
                    tempStr = tempStr.Substring(0, tempStr.Length - 1);

                    sw.WriteLine(tempStr);
                    tempStr = "";
                }


                sw.Close();
                myStream.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }


            return true;

        }

        private  bool ExportCSV(string tName, string fName, DataTable dData, bool isHead)
        {
            string TaskName = tName;
            string FileName = ParseFile(fName);
            System.Data.DataTable gData = dData;

            //判断目录根据结果并创建
            ToolUtil.CreateDirectory(FileName);

            if (dData.Columns[dData.Columns.Count - 1].ColumnName == "isPublished")
            {
                dData.Columns.Remove("isPublished");
            }


            FileStream myStream = File.Open(FileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            string str = "";
            string tempStr = "";

            try
            {
                if (isHead == true)
                {
                    //写标题 
                    for (int i = 0; i < gData.Columns.Count; i++)
                    {
                        str += gData.Columns[i].ColumnName;
                        str += ",";
                    }

                    //去掉最后一个分隔符
                    str = str.Substring(0, str.Length - 1);

                    sw.WriteLine(str);
                }

                //写内容 
                for (int i = 0; i < gData.Rows.Count; i++)
                {
                    for (int j = 0; j < gData.Columns.Count; j++)
                    {
                        tempStr += gData.Rows[i][j];
                        tempStr += ",";
                    }

                    //去掉最后一个分隔符
                    tempStr = tempStr.Substring(0, tempStr.Length - 1);

                    sw.WriteLine(tempStr);
                    tempStr = "";
                }


                sw.Close();
                myStream.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }


            return true;
        }

        public string ParseFile(string fName)
        {
            if (fName.IndexOf("{FileCurrentDateTime}") > 0)
            {
                string paraDate = System.DateTime.Now.ToFileTimeUtc().ToString();

                fName = fName.Replace("{FileCurrentDateTime}", paraDate);
                return fName;
            }
            else
            {
                return fName;
            }
        }
        #endregion

        #region 发布到web网站
        /// <summary>
        /// 发布数据到网站，返回请求回来的网页源码
        /// </summary>
        /// <param name="dCols"></param>
        /// <param name="dRow"></param>
        /// <param name="PostUrl"></param>
        /// <param name="UrlCode"></param>
        /// <param name="cookie"></param>
        /// <param name="IsHeader"></param>
        /// <param name="Header"></param>
        /// <returns></returns>
        public string ExportWeb(DataColumnCollection dCols, object dRow, string PostUrl, cGlobalParas.WebCode UrlCode, 
            ref string cookie, bool IsHeader, string Header)
        {
            //分解url，如果是多url，则多次发布
            string strWebSource = "";

            string[] pUrls = Regex.Split(PostUrl, "##NextUrl##\r\n");
            for (int i = 0; i < pUrls.Length; i++)
            {
                strWebSource=ExportWebUrl(dCols, dRow, pUrls[i], UrlCode, ref cookie, IsHeader, Header);
            }

            return strWebSource;
        }

        private string ExportWebUrl(DataColumnCollection dCols, object dRow, string PostUrl, cGlobalParas.WebCode UrlCode, 
            ref string cookie, bool IsHeader, string Header)
        {
            //开始循环发布数据
            //这是一个大循环

            object[] Row = ((object[])dRow);

            string PostPara = "";
            string url = PostUrl;

            CookieContainer CookieCon;

            HttpWebRequest wReq;

            CookieCon = new CookieContainer();

            //if (Regex.IsMatch(url, @"<POST>.*</POST>", RegexOptions.IgnoreCase))
            if (Regex.IsMatch(url, @"<POST[^>]*>[\S\s]*</POST>", RegexOptions.IgnoreCase))
            {
                wReq = (HttpWebRequest)WebRequest.Create(@url.Substring(0, url.IndexOf("<POST")));
            }
            else
            {
                wReq = (HttpWebRequest)WebRequest.Create(@url);
            }

            //2013-3-25增加的一个标志
            wReq.ServicePoint.Expect100Continue = false;
            bool isMutileData = false;

            if (IsHeader == true)
            {
                foreach (string sc in Header.Split('\r'))
                {
                    string ss = sc.Trim();
                    //wReq.Headers.Add(ss.Substring(0, ss.IndexOf(":")), ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1));

                    switch (ss.Substring(0, ss.IndexOf(":")))
                    {
                        case "Accept":
                            wReq.Accept = ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1);
                            break;
                        case "User-Agent":
                            wReq.UserAgent = ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1);
                            break;
                        case "Connection":
                            wReq.KeepAlive = true;
                            break;
                        case "Content-Type":
                            wReq.ContentType = ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1);
                            if (ss.IndexOf("multipart/form-data", StringComparison.CurrentCultureIgnoreCase) > 0)
                                isMutileData = true;
                            break;
                        case "Referer":
                            wReq.Referer = ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1);
                            break;
                        default:
                            wReq.Headers.Add(ss.Substring(0, ss.IndexOf(":")), ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1));
                            break;
                    }
                }
            }
            else
            {
                wReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50215;)";
                Match a = Regex.Match(url, @"(http://).[^/]*[?=/]", RegexOptions.IgnoreCase);
                string url1 = a.Groups[0].Value.ToString();
                wReq.Referer = url1;
            }

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

            //设置页面超时时间为8秒
            wReq.Timeout = 8000;

            //string ExportUrl=url;

            //替换Url地址中的参数
            for (int j = 0; j < dCols.Count; j++)
            {
                string strPara = "{" + dCols[j].ColumnName + "}";
                string strParaValue = Row[j].ToString();
                if (isMutileData == true)
                    url = url.Replace(strPara, strParaValue);
                else
                {
                    if (UrlCode == cGlobalParas.WebCode.NoCoding)
                    {
                        url = url.Replace(strPara, HttpUtility.UrlEncode ( strParaValue,Encoding.ASCII ) );
                    }
                    else if (UrlCode == cGlobalParas.WebCode.utf8)
                    {
                        url = url.Replace(strPara, HttpUtility.UrlEncode(strParaValue, Encoding.UTF8));
                    }
                    else if (UrlCode == cGlobalParas.WebCode.gb2312)
                    {
                        url = url.Replace(strPara, HttpUtility.UrlEncode(strParaValue, Encoding.GetEncoding ("gb2312")));
                    }
                    else if (UrlCode == cGlobalParas.WebCode.gbk)
                    {
                        url = url.Replace(strPara, HttpUtility.UrlEncode(strParaValue, Encoding.GetEncoding("gbk")));
                    }
                    else if (UrlCode == cGlobalParas.WebCode.big5)
                    {
                        url = url.Replace(strPara, HttpUtility.UrlEncode(strParaValue, Encoding.GetEncoding("big5")));
                    }
                    else
                    {
                        url = url.Replace(strPara, HttpUtility.UrlEncode(strParaValue, Encoding.UTF8));
                    }
                }
            }


            //开始发布数据，首先判断是否为POST方式进行数据发布
            //判断是否含有POST参数
            if (Regex.IsMatch(url, @"(?<=<POST[^>]*>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase))
            {

                Match s = Regex.Match(url, @"(?<=<POST[^>]*>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase);
                PostPara = s.Groups[0].Value.ToString();

                byte[] pPara;

                if (UrlCode == cGlobalParas.WebCode.NoCoding)
                {
                    pPara = Encoding.ASCII.GetBytes(PostPara);
                }
                else if (UrlCode == cGlobalParas.WebCode.utf8)
                {
                    pPara = Encoding.UTF8.GetBytes(PostPara);
                }
                else if (UrlCode == cGlobalParas.WebCode.gb2312)
                {
                    pPara = Encoding.GetEncoding("gb2312").GetBytes(PostPara);
                }
                else if (UrlCode == cGlobalParas.WebCode.gbk)
                {
                    pPara = Encoding.GetEncoding("gbk").GetBytes(PostPara);
                }
                else if (UrlCode == cGlobalParas.WebCode.big5)
                {
                    pPara = Encoding.GetEncoding("big5").GetBytes(PostPara);
                }
                else
                {
                    pPara = Encoding.ASCII.GetBytes(PostPara);
                }

                //wReq.ContentType = "application/x-www-form-urlencoded";
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

            HttpWebResponse wResp = (HttpWebResponse)wReq.GetResponse();
            System.IO.Stream respStream = wResp.GetResponseStream();

            string strWebData = "";

            string RedirectUrl = "";
            if (wResp.StatusCode.ToString() == "302")
            {
                RedirectUrl = wResp.Headers["Location"].ToString ();
            }

            //获取返回的cookie
            if (wResp.Cookies.Count > 0)
            {
                CookieCollection wCookie = wResp.Cookies;   //得到新的cookie：注意这里没考虑cookie合并的情况  

                //开始合并cookie
                cookie = ToolUtil.MergerCookie(cookie, wCookie);
            }

            #region 请求数据的编码、压缩格式转换
            cGlobalParas.WebCode webCode=cGlobalParas.WebCode.auto;
            Encoding wCode;

            bool IsAutoCharset = false;
            switch (webCode)
            {
                case cGlobalParas.WebCode.auto:
                    try
                    {
                        string autoCode = wResp.CharacterSet;
                        wCode = System.Text.Encoding.GetEncoding(wResp.CharacterSet);
                        if (autoCode.ToLower() == "iso-8859-1" || autoCode == "")
                        {
                            wCode = Encoding.Default;
                            IsAutoCharset = true;
                        }
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
                
                case cGlobalParas.WebCode.big5 :
                    wCode = Encoding.GetEncoding("big5");
                    break;
                default:
                    wCode = Encoding.UTF8;
                    break;
            }

            //string charSet = wResp.CharacterSet.ToString();


            if (wResp.ContentEncoding == "gzip")
            {
                if (IsAutoCharset == true)
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
                    Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string webCharSet = charSetMatch.ToString();
                    webCharSet = Regex.Replace(webCharSet, "[\"|']", "");
                    if (webCharSet != "")
                        wCode = System.Text.Encoding.GetEncoding(webCharSet);
                    else
                        wCode = Encoding.Default;

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
                    GZipStream myGZip = new GZipStream(respStream, CompressionMode.Decompress);
                    System.IO.StreamReader reader;
                    reader = new System.IO.StreamReader(myGZip, wCode);
                    strWebData = reader.ReadToEnd();

                    reader.Close();
                    reader.Dispose();
                    myGZip.Close();
                }

            }
            else if (wResp.ContentEncoding.StartsWith("deflate"))
            {
                if (IsAutoCharset == true)
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
                    Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string webCharSet = charSetMatch.ToString();
                    webCharSet = Regex.Replace(webCharSet, "[\"|']", "");
                    if (webCharSet != "")
                        wCode = System.Text.Encoding.GetEncoding(webCharSet);
                    else
                        wCode = Encoding.Default;

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
                    DeflateStream myDeflate = new DeflateStream(respStream, CompressionMode.Decompress);
                    System.IO.StreamReader reader;
                    reader = new System.IO.StreamReader(myDeflate, wCode);
                    strWebData = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                }
            }
            else
            {
                if (IsAutoCharset == true)
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
                    Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string webCharSet = charSetMatch.ToString();
                    webCharSet = Regex.Replace(webCharSet, "[\"|']", "");
                    if (webCharSet != "")
                        wCode = System.Text.Encoding.GetEncoding(webCharSet);
                    else
                        wCode = Encoding.Default;

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
                    System.IO.StreamReader reader;
                    reader = new System.IO.StreamReader(respStream, wCode);
                    strWebData = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                }
            }

            respStream.Close();
            respStream.Dispose();
            wResp.Close();

            #endregion
            return strWebData;

          
        }

        #endregion

        #region 模版Web发布操作
        /// <summary>
        /// 发布数据操作
        /// </summary>
        /// <param name="dCols">发布数据的列</param>
        /// <param name="dRow">行数据</param>
        /// <param name="tName">模板名称</param>
        /// <param name="cookie">发布网站Cookie</param>
        /// <param name="domain">发布网站</param>
        /// <param name="classTable">发布的分类数据</param>
        /// <param name="publishParas">发布参数</param>
        /// <param name="gParas">全局参数</param>
        /// <param name="returnSource">获取发布后返回的页面数据</param>
        /// <returns>获取回链的地址，如果为空，则发布成功，错误信息则直接触发错误</returns>
        public string PublishByWebTemplate(DataColumnCollection dCols, object dRow, string tName, string cookie, 
            string domain, Hashtable classTable,List<ePublishData> publishParas,Hashtable gParas,string strPlugin, out string returnSource)
        {
            returnSource = String.Empty;

            int rowCount = ((object[])dRow).Length;
            object[] Row = new object[rowCount];

            ((object[])dRow).CopyTo(Row, 0);


            string strHeader="";
            bool isMutilData = false;
            string boundaryValue = "";

            cTemplate t = new cTemplate(m_workPath);
            t.LoadTemplate(tName);

            cGlobalParas.RUrlPageType rUrlType = t.RUrlPageType;
            string rUrlPage = t.RUrlPage;
            string rUrlRule = t.RUrlRule;

            string RUrl = string.Empty;

            if (t.IsHeader)
            {
                strHeader = t.Headers;
                if (!strHeader.EndsWith("\r\n"))
                {
                    strHeader += "\r\n";
                }
            }
            else
            {
                strHeader = "User-Agent: Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0\r\n";
                strHeader += "Accept-Language:zh-cn\r\n";
                strHeader += "Accept-Encoding:gzip,deflate\r\n";
                strHeader += "Connection: Keep-Alive\r\n";
            }
            strHeader += "Cookie:" + cookie + "\r\n";

            string getStrHeader = strHeader + "\r\n";

          

            #region 处理数据发布的问题
            string pUrl = t.PublishUrl.Trim ();

            if (pUrl == "")
            {
                t = null;
                throw new Exception ("发布模板中发布地址无效导致发布失败！") ;
            }
            pUrl = pUrl.Replace(t.Domain, domain);
            string pRurl = t.PublishRUrl;
            pRurl = pRurl.Replace(t.Domain, domain);

            pUrl = ReplacePara(pUrl, gParas, classTable, dCols, dRow,  publishParas);
            pRurl = ReplacePara(pRurl, gParas, classTable, dCols, dRow,  publishParas);

            Dictionary<string, string> pParas = t.PublishParas;

            //获取来路页面的信息
            string webSource = "";

            #region 处理来路页面的信息
            //建立一个来路页面的表单数据值
            Hashtable rFormValue = new Hashtable();

            if (pRurl != "")
            {
                webSource = cHttpSocket.GetUrl(pRurl, getStrHeader, "GET", t.uCode,"", 0,out cookie);

                //先获取form
                Regex reForm = new Regex("<input[^>]+?>",RegexOptions.IgnoreCase);
                MatchCollection mcForm = reForm.Matches(webSource);
                foreach (Match ma in mcForm)
                {
                    //判断Type是否为hidden，如果是，则添加到表单表中
                    Match a = Regex.Match(ma.ToString (), @"type=.+?\s", RegexOptions.IgnoreCase);
                    string strT = a.Groups[0].Value.ToString();
                    if (strT.IndexOf("hidden", StringComparison.CurrentCultureIgnoreCase) > -1 || strT.IndexOf("text", StringComparison.CurrentCultureIgnoreCase) > -1)
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
                                if (ss1.EndsWith (">"))
                                    ss1 = ss1.Substring(0, ss1.Length - 1) + " >";
                            }
                            

                            Match a1 = Regex.Match(ss1, @"(?<=name=).+?\s", RegexOptions.IgnoreCase);
                            n = a1.Groups[0].Value.ToString();

                            //判断value=后面是什么，单引号？多引号还是什么都没有
                            string valueReg = string.Empty;

                            if (ss1.Substring(ss1.IndexOf("value=", StringComparison.CurrentCultureIgnoreCase) + 6, 1) == "'")
                                valueReg = "(?<=value=').+?'";
                            else if (ss1.Substring(ss1.IndexOf("value=", StringComparison.CurrentCultureIgnoreCase) + 6, 1) == "\"")
                                valueReg = "(?<=value=\").+?\"";
                            else
                                valueReg = "(?<=value=).+?\\s";
                            a1 = Regex.Match(ss1, valueReg, RegexOptions.IgnoreCase);
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

            #region 在此处理上传的文件
            Hashtable uFile = new Hashtable();

            for (int m = 0; m < publishParas.Count; m++)
            {
                string uUrl = t.UploadUrl;
                string uRUrl = t.UploadRUrl;
                uUrl = uUrl.Replace(t.Domain, domain);
                uRUrl = uRUrl.Replace(t.Domain, domain);

                uUrl = ReplacePara(uUrl, gParas, classTable, dCols, dRow, publishParas);
                uRUrl = ReplacePara(uRUrl, gParas, classTable, dCols, dRow, publishParas);

                if (publishParas[m].DataLabel == "上传文件目录")
                {
                    #region  上传整个目录
                    if (publishParas[m].DataValue != "")
                    {
                        System.IO.DirectoryInfo dir = null;

                        if (publishParas[m].DataType == cGlobalParas.PublishParaType.Gather)
                        {
                            string dirField = publishParas[m].DataValue;

                            for (int j = 0; j < dCols.Count; j++)
                            {
                                if (dirField == dCols[j].ColumnName)
                                {
                                    dir = new System.IO.DirectoryInfo(Row[j].ToString());
                                    break;
                                }
                            }
                        }
                        else
                        {
                            dir = new System.IO.DirectoryInfo(publishParas[m].DataValue);
                        }

                        if (dir != null)
                        {
                            if (dir.Exists)
                            {
                                foreach (System.IO.FileInfo file in dir.GetFiles())
                                {
                                    //则表示有上传的文件，需要进行上传操作，并返回有效的数据，进行替换

                                    string wSource = UploadFile(uUrl, uRUrl, file.FullName, strHeader, t.uCode, cookie, t.UploadParas,
                                        gParas, rFormValue, classTable, publishParas, dCols, Row, webSource);
                                    string uploadValue = "";

                                    //先把正则获取出来
                                    string strMatch = "(?<={替换参数:).+?(?=})";
                                    Match s = Regex.Match(t.UploadReplace, strMatch, RegexOptions.IgnoreCase);
                                    string uRegex = s.Groups[0].Value;

                                    if (uRegex == "")
                                    {
                                        uploadValue = wSource;
                                    }
                                    else
                                    {
                                        //开始捕获源码中返回的数值
                                        s = Regex.Match(wSource, uRegex, RegexOptions.IgnoreCase);
                                        uploadValue = s.Groups[0].Value;

                                        //进行一次转吗
                                        uploadValue = uploadValue.Replace("\\/", "/");
                                    }

                                    //将返回值加入到hashtable中
                                    try
                                    {
                                        uFile.Add(uploadValue, wSource);
                                    }
                                    catch { }

                                    uploadValue = Regex.Replace(t.UploadReplace, "({替换参数:).+?(\\)})", uploadValue);

                                    //获取源码，根据替换规则开始将原来的数据进行替换
                                    for (int n = 0; n < Row.Length; n++)
                                    {
                                        if (Row[n].ToString().IndexOf(file.Name) > 0)
                                        {
                                            //标识包含此文件名，需要进行替换
                                            string strReg = "<img[^>]+?" + file.Name + "[^>]*/>";
                                            Row[n] = Regex.Replace(Row[n].ToString(), strReg, uploadValue);

                                        }
                                        else if (Row[n].ToString().IndexOf("{系统参数:取上传第一张图片}") > -1)
                                        {
                                            Row[n] = Regex.Replace(Row[n].ToString(), "{系统参数:取上传第一张图片}", uploadValue);
                                        }
                                    }


                                }
                            }
                        }
                    }

                    #endregion
                }
                else if (publishParas[m].DataLabel == "上传文件")
                {
                    #region 上传单个文件
                    string uploadFileName = "";

                    if (publishParas[m].DataType == cGlobalParas.PublishParaType.Gather)
                    {
                        string dirField = publishParas[m].DataValue;

                        for (int j = 0; j < dCols.Count; j++)
                        {
                            if (dirField == dCols[j].ColumnName)
                            {
                                uploadFileName = Row[j].ToString();
                                break;
                            }
                        }
                    }
                    else
                    {
                        uploadFileName = publishParas[m].DataValue;
                    }

                    if (uploadFileName != "")
                    {

                        //则表示有上传的文件，需要进行上传操作，并返回有效的数据，进行替换

                        string wSource = UploadFile(uUrl, uRUrl, uploadFileName, strHeader, t.uCode, cookie,
                            t.UploadParas, gParas, rFormValue, classTable, publishParas, dCols, Row, webSource);
                        string uploadValue = "";

                        //先把正则获取出来
                        string strMatch = "(?<={替换参数:).*(?=})";
                        Match s = Regex.Match(t.UploadReplace, strMatch, RegexOptions.IgnoreCase);
                        string uRegex = s.Groups[0].Value;

                        if (uRegex == "")
                        {
                            uploadValue = wSource;
                        }
                        else
                        {
                            //开始捕获源码中返回的数值
                            s = Regex.Match(wSource, uRegex, RegexOptions.IgnoreCase);
                            uploadValue = s.Groups[0].Value;

                            //进行一次转吗
                            uploadValue = uploadValue.Replace("\\/", "/");
                        }

                        //将返回值加入到hashtable中
                        try
                        {
                            uFile.Add(uploadValue, "");
                        }
                        catch { }

                        uploadValue = Regex.Replace(t.UploadReplace, "({替换参数:).+?(\\)})", uploadValue);

                        //获取源码，根据替换规则开始将原来的数据进行替换
                        for (int n = 0; n < Row.Length; n++)
                        {
                            if (Row[n].ToString().IndexOf(Path.GetFileName(uploadFileName)) > 0)
                            {
                                //标识包含此文件名，需要进行替换
                                string strReg = "<img[^>]+?" + Path.GetFileName(uploadFileName) + "[^>]*/>";
                                Row[n] = Regex.Replace(Row[n].ToString(), strReg, uploadValue);

                            }
                            else if (Row[n].ToString().IndexOf("{系统参数:取上传第一张图片}") > -1)
                            {
                                Row[n] = Regex.Replace(Row[n].ToString(), "{系统参数:取上传第一张图片}", uploadValue);
                            }
                        }

                    }
                    #endregion


                }
            }

            #endregion

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

                    temppUrl = System.Web.HttpUtility.HtmlDecode(temppUrl);
                    temppUrl = temppUrl.Replace("'", "");
                    temppUrl = temppUrl.Replace("\"", "").Trim ();

                    if (temppUrl!="" && !temppUrl.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                    {
                        temppUrl = NetMiner.Common.ToolUtil.RelativeToAbsoluteUrl(pUrl, temppUrl);
                    }

                    temppUrl = temppUrl.Replace("127.0.0.1", "localhost");
                    pUrl = pUrl.Replace("127.0.0.1", "localhost");
                    if (temppUrl==pUrl)
                    {
                        isMutilData = true;
                        break;
                    }
                    isMutilData = true;
                    
                }
            }

            #endregion

            #region  如果是multipart/form-data 类型，则重新构造头部
            if (isMutilData == true)
            {
                //重新构造头部信息
                string[] headers = Regex.Split(strHeader, "\r\n");
                strHeader = "";

                //随机产生一个boundary的值
                boundaryValue = "---------------------------7d" + GenerateRandomNumber(12);

                bool isContentTyp = false;
                for (int i = 0; i < headers.Length; i++)
                {
                    if (headers[i] != "")
                    {
                        if (headers[i].StartsWith("Content-Type", StringComparison.CurrentCultureIgnoreCase))
                        {
                            strHeader += "Content-Type: multipart/form-data; boundary=" + boundaryValue + "\r\n";
                            isContentTyp = true;
                        }
                        else
                        {
                            strHeader += headers[i] + "\r\n";
                        }
                    }
                }
                if (isContentTyp == false)
                {
                    strHeader += "Content-Type: multipart/form-data; boundary=" + boundaryValue + "\r\n";
                }
            }
            #endregion

            string pData="";

            #region 开始替换参数，并构建post数据
            //开始替换其中的参数信息,并构造post数据

            foreach (KeyValuePair<string, string> de in pParas)
            {
                string label = de.Key.ToString();
                string value = de.Value.ToString();

                if (label.StartsWith("{动态参数"))
                {
                    //先将参数名称替换掉
                    //先处理掉\r\n，以前没处理
                    string wSource = webSource.Replace("\r", "").Replace ("\n","");
                    string dLabelReg = label.Substring(6, label.IndexOf("}") - 6);
                    Match a = Regex.Match(wSource, dLabelReg, RegexOptions.IgnoreCase);
                    label = a.Groups[0].Value.ToString();
                }

                if (value.IndexOf("{系统参数:取上传第一张图片}") > -1)
                {
                    //需要处理上传参数的值
                    foreach (DictionaryEntry de3 in uFile)
                    {
                        string ss2 = value.Replace("{系统参数:取上传第一张图片}", de3.Key.ToString());
                        //循环构造post数据
                        if (isMutilData == true)
                        {
                            pData += "--" + boundaryValue + "\r\n";
                            pData += "Content-Disposition: form-data; name=\"" + label + "\"\r\n";
                            //pData += "Content-Type: application/octet-stream" + "\r\n";
                            pData += "\r\n";
                            pData += ss2 + "\r\n";
                        }
                        else
                        {
                            pData += label + "=" + ss2 + "&";
                        }
                        break;
                    }
                }
                else if (value.IndexOf("{系统参数上传页面正则") > -1)
                {
                    string valuesRegex = value.Substring(value.IndexOf("{系统参数上传页面正则:") + 12, value.IndexOf("}") - value.IndexOf("{系统参数正则:") - 11);
                    //Match svaluesRegex = Regex.Match(webSource, valuesRegex, RegexOptions.IgnoreCase);
                    //value = svaluesRegex.Groups[0].Value.ToString();
                    foreach (DictionaryEntry de3 in uFile)
                    {
                        Match svaluesRegex = Regex.Match(de3.Value.ToString (), valuesRegex, RegexOptions.IgnoreCase);
                        value = svaluesRegex.Groups[0].Value.ToString();
                        value=value.Replace ("\\/","/");
                        value =value.Replace ("\\\"","\"");

                        if (isMutilData == true)
                        {
                            pData += "--" + boundaryValue + "\r\n";
                            pData += "Content-Disposition: form-data; name=\"" + label + "\"\r\n";
                            pData += "\r\n";
                            pData += value + "\r\n";
                        }
                        else
                        {
                            pData += label + "=" + value + "&";
                        }
                        break;
                    }
                }
                else
                {
                    if (value.StartsWith("{发布参数"))
                    {
                        string pName = value.Substring(value.IndexOf("{发布参数:") + 6, value.IndexOf("}") - value.IndexOf("{发布参数:") - 6);

                        //找到了发布模版中配置的参数，然后开始在发布规则中对应参数的规则
                        for (int i = 0; i < publishParas.Count; i++)
                        {
                            if (pName == publishParas[i].DataLabel)
                            {
                                if (publishParas[i].DataType == cGlobalParas.PublishParaType.Gather)
                                {
                                    //开始在采集的数据中进行匹配
                                    for (int j = 0; j < dCols.Count; j++)
                                    {
                                        if (publishParas[i].DataValue == dCols[j].ColumnName)
                                        {
                                            value = Row[j].ToString();
                                            break;

                                        }
                                    }
                                }
                                else if (publishParas[i].DataType == cGlobalParas.PublishParaType.Custom)
                                {
                                    value = publishParas[i].DataValue;
                                }
                                else if (publishParas[i].DataType == cGlobalParas.PublishParaType.NoData)
                                {
                                    value = "";
                                }
                            }
                        }

                    }
                    else if (value.IndexOf("{上传参数") > -1)
                    {
                        //需要处理上传参数的值
                        string pName = value.Substring(value.IndexOf("{上传参数:") + 6, value.IndexOf("}") - value.IndexOf("{上传参数:") - 6);

                        //找到了发布模版中配置的参数，然后开始在发布规则中对应参数的规则
                        for (int i = 0; i < publishParas.Count; i++)
                        {
                            if (pName == publishParas[i].DataLabel)
                            {
                                if (publishParas[i].DataType == cGlobalParas.PublishParaType.Gather)
                                {
                                    //开始在采集的数据中进行匹配
                                    for (int j = 0; j < dCols.Count; j++)
                                    {
                                        if (publishParas[i].DataValue == dCols[j].ColumnName)
                                        {
                                            value = Row[j].ToString();
                                            break;

                                        }
                                    }
                                }
                                else if (publishParas[i].DataType == cGlobalParas.PublishParaType.Custom)
                                {
                                    value = publishParas[i].DataValue;
                                }
                                else if (publishParas[i].DataType == cGlobalParas.PublishParaType.NoData)
                                {
                                    value = "";
                                }
                            }
                        }
                        value = "Content-Type: application/octet-stream" + "\r\n" + value;
                    }
                    else if (value.StartsWith("{系统参数"))
                    {
                        if (value.StartsWith("{系统参数正则"))
                        {
                            //正则匹配
                            string valuesRegex = value.Substring(value.IndexOf("{系统参数正则:") + 8, value.IndexOf("}") - value.IndexOf("{系统参数正则:") - 8);
                            Match svaluesRegex = Regex.Match(webSource, valuesRegex, RegexOptions.IgnoreCase);
                            value = svaluesRegex.Groups[0].Value.ToString();
                        }
                        else
                        {
                            string pName = value.Substring(value.IndexOf("{系统参数:") + 6, value.IndexOf("}") - value.IndexOf("{系统参数:") - 6);

                            if (pName == "分类")
                            {
                                //现在发布规则中找分类对应的数据值，并取出
                                for (int i = 0; i < publishParas.Count; i++)
                                {
                                    if ("系统分类" == publishParas[i].DataLabel)
                                    {
                                        if (publishParas[i].DataType == cGlobalParas.PublishParaType.Gather)
                                        {
                                            //开始在采集的数据中进行匹配
                                            for (int j = 0; j < dCols.Count; j++)
                                            {
                                                if (publishParas[i].DataValue == dCols[j].ColumnName)
                                                {
                                                    value = Row[j].ToString();
                                                    break;
                                                }
                                            }
                                        }
                                        else if (publishParas[i].DataType == cGlobalParas.PublishParaType.Custom)
                                        {
                                            value = publishParas[i].DataValue;
                                        }
                                        else if (publishParas[i].DataType == cGlobalParas.PublishParaType.NoData)
                                        {
                                            value = "";
                                        }

                                        //开始将分类替换成ID，或者将分类ID替换成分类名称
                                        //判断依据为是否为数字
                                        foreach (DictionaryEntry de1 in classTable)
                                        {
                                            if (Regex.IsMatch(value, "^[0-9]*$"))
                                            {
                                                if (value == de1.Key.ToString())
                                                {
                                                    value = de1.Value.ToString();
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (value == de1.Value.ToString())
                                                {
                                                    value = de1.Key.ToString();
                                                    break;
                                                }
                                            }
                                        }
                                    }


                                }


                            }
                            else if (pName == "时间")
                            {
                                value = System.DateTime.Now.ToString();
                            }
                            else if (pName == "取来路页面表单值")
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
                        }

                    }
                    else if (value.StartsWith("{打码参数"))
                    {
                        //在此处理打码的问题
                        //提取打码的图片
                        string valuesRegex = value.Substring(value.IndexOf("{打码参数:") + 6, value.IndexOf("}") - value.IndexOf("{打码参数:") - 6);
                        Match svaluesRegex = Regex.Match(webSource, valuesRegex, RegexOptions.IgnoreCase);
                        string imgUrl = svaluesRegex.Groups[0].Value.ToString();

                        string tmpCookie = string.Empty;
                        string imgName = NetMiner.Common.HttpSocket.cHttpSocket.GetImage(Path.GetTempPath().ToString(), imgUrl, ref tmpCookie);

                        //开始获取验证码
                        cRunPlugin rPlugin = new cRunPlugin();
                        string vCode = string.Empty;
                        if (strPlugin != "")
                        {
                            vCode = rPlugin.CallVerifyCode(imgName, imgUrl, strPlugin);
                        }
                        rPlugin = null;

                        value = vCode;
                    }

                    if (isMutilData == true)
                    {
                        if (value.IndexOf("Content-Type", StringComparison.CurrentCultureIgnoreCase) > -1)
                        {
                            pData += "--" + boundaryValue + "\r\n";
                            pData += "Content-Disposition: form-data; name=\"" + label + "\";filename=\"\"\r\n";
                            pData += value;
                            pData += "\r\n";
                            pData += "\r\n";
                        }
                        else
                        {
                            pData += "--" + boundaryValue + "\r\n";
                            pData += "Content-Disposition: form-data; name=\"" + label + "\"\r\n";
                            pData += "\r\n";
                            pData += value + "\r\n";
                        }
                    }
                    else
                    {
                        //修改加入了编码操作
                        //pData += label + "=" + value + "&";
                        value = value.Replace("\\r", "\r");
                        value = value.Replace("\\n", "\n");

                        switch (t.uCode)
                        {
                            case NetMiner.Resource.cGlobalParas.WebCode.auto:
                                pData += label + "=" + value + "&";
                                break;
                            case NetMiner.Resource.cGlobalParas.WebCode.big5:
                                pData += label + "=" + System.Web.HttpUtility.UrlEncode(value, Encoding.GetEncoding("big5")) + "&";
                                break;
                            case NetMiner.Resource.cGlobalParas.WebCode.gb2312:
                                pData += label + "=" + System.Web.HttpUtility.UrlEncode(value, Encoding.GetEncoding("gb2312")) + "&";
                                break;
                            case NetMiner.Resource.cGlobalParas.WebCode.gbk:
                                pData += label + "=" + System.Web.HttpUtility.UrlEncode(value, Encoding.GetEncoding("gbk")) + "&";
                                break;
                            case NetMiner.Resource.cGlobalParas.WebCode.NoCoding:
                                pData += label + "=" + value + "&";
                                break;
                            case NetMiner.Resource.cGlobalParas.WebCode.utf8:

                                pData += label + "=" + System.Web.HttpUtility.UrlEncode(value, Encoding.UTF8) + "&";
                                break;
                        }
                    }
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

            #endregion

            #region 发布操作

            if (pRurl != "")
                strHeader += "Referer: " + pRurl + "\r\n";

            int pDataLen = 0;
            switch (t.uCode)
            {
                case NetMiner.Resource.cGlobalParas.WebCode.auto:
                    pDataLen=Encoding.ASCII.GetBytes(pData).Length ;
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.big5:
                    pDataLen = Encoding.GetEncoding ("big5").GetBytes(pData).Length;
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

            //strHeader += "Content-length: " + pData.Length + "\r\n\r\n";
            strHeader += "Content-length: " + pDataLen + "\r\n\r\n";
            strHeader += pData ;

            webSource = cHttpSocket.GetUrl(pUrl, strHeader, "POST",t.uCode, "",0,out  cookie);

            returnSource = webSource;

            bool isS = false;
            if (webSource.IndexOf(t.PublishSuccess) > -1)
            {
                isS = true;
            }
            else
            {
                isS = false;
            }

            #endregion 

            #endregion

            if (isS == false)
            {
                string errMess = Regex.Replace(webSource, @"<[pbr][^>]*.|</[pbr][^>]*.", "\r\n ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                
                throw new Exception(errMess);
            }

            #region 开始获取回链

            if (rUrlRule.Trim() == "")
                return "";                //无回链规则

            //开始处理回链
            string rSource = string.Empty;
            if (rUrlType == cGlobalParas.RUrlPageType.CurrentPage)
            {
                rSource = webSource;
            }
            else if (rUrlType == cGlobalParas.RUrlPageType.CustomPage && rUrlPage.Trim ()!="")
            {
                int ai = 0;
            Aragin:
                string rUrlCookie = string.Empty;
                rUrlPage = rUrlPage.Replace(t.Domain, domain);

                //替换全局参数
                if (gParas != null)
                {
                    foreach (DictionaryEntry de in gParas)
                    {
                        if (rUrlPage.IndexOf("{全局参数:") > 0)
                        {

                            Match aaa1 = Regex.Match(rUrlPage, @"(?<={全局参数:).*(?=})", RegexOptions.IgnoreCase);
                            string p1 = aaa1.Groups[0].Value.ToString();
                            if (p1.Trim() == de.Key.ToString())
                            {
                                //替换
                                rUrlPage = Regex.Replace(rUrlPage, "{全局参数:.*}", de.Value.ToString(), RegexOptions.IgnoreCase);
                            }
                        }
                    }
                }

                rSource=cHttpSocket.GetUrl(rUrlPage, getStrHeader, "GET", t.uCode, "", 0, out cookie);

                ai++;

                if (rSource == "" && ai<3)
                {
                    Thread.Sleep(3000);
                    goto Aragin;
                }
            }

            if (rSource == "")
            {
                throw new Exception("获取回链失败，请检查回链地址或查看网站是否响应，或是否已经登录");
            }

            //根据规则获取回链
            Match aa1 = Regex.Match(rUrlRule, @"(?<={RUrlPara:).*(?=})", RegexOptions.IgnoreCase);
            string rUrlPara = aa1.Groups[0].Value.ToString();
            string rUrlParaValue = string.Empty;

            //先通过模板参数对应到发布参数
            foreach (KeyValuePair<string, string> de in pParas)
            {
                if (rUrlPara == de.Key.ToString ())
                {
                    rUrlPara = de.Value.ToString();
                    break;
                }
            }

            aa1 = Regex.Match(rUrlPara, @"(?<=参数:).*(?=})", RegexOptions.IgnoreCase);
            rUrlPara = aa1.Groups[0].Value.ToString();

            for (int m = 0; m < dCols.Count; m++)
            {
                if (rUrlPara == dCols[m].ColumnName)
                {
                    rUrlParaValue = Row[m].ToString();
                    break;
                }
            }

            rUrlRule = Regex.Replace(rUrlRule, "{RUrlPara:.*}", rUrlParaValue, RegexOptions.IgnoreCase);
            if (rUrlParaValue == rUrlRule)
            {
                //表示无规则，只是一个值，则系统获取此值得链接地址
                string regRurl = "(?<=href=['|\"])[^>]+?(?=['|\"].+?" + rUrlParaValue + ")";
                aa1 = Regex.Match(rSource, rUrlRule, RegexOptions.IgnoreCase);
                RUrl = aa1.Groups[0].Value.ToString();
            }
            else
            {
                //正则获取
                aa1 = Regex.Match(rSource, rUrlRule, RegexOptions.IgnoreCase);
                RUrl = aa1.Groups[0].Value.ToString();
            }

            if (RUrl == "")
            {
                RUrl ="http://" + (new Uri(pUrl)).Authority;
            }
            else if (!RUrl.ToLower().StartsWith("http"))
            {
                //转换绝对地址
                Uri aUri=null;
                if(rUrlType == cGlobalParas.RUrlPageType.CurrentPage)
                    aUri = new Uri(new Uri(pUrl), RUrl);
                else if (rUrlType == cGlobalParas.RUrlPageType.CustomPage)
                    aUri = new Uri(new Uri(rUrlPage), RUrl);
                RUrl = aUri.AbsoluteUri;
                aUri = null;
            }
            
            t = null;
            #endregion

            return RUrl;
        }

        public string UploadFile1(string uUrl, string uRurl, string fName, string strHeader, cGlobalParas.WebCode uCode, string cookie, Dictionary<string, string> uParas)
        {
            int returnValue = 0;

            // 要上传的文件 
            FileStream fs = new FileStream(fName, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);

            //时间戳 
            string strBoundary = "----------" + GenerateRandomNumber(12); 
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");

            //请求头部信息 

            string strPostHeader = strHeader;

            byte[] postHeaderBytes = null;

            switch (uCode)
            {
                case NetMiner.Resource.cGlobalParas.WebCode.auto:
                    postHeaderBytes=Encoding.ASCII.GetBytes(strPostHeader);
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.big5:
                    postHeaderBytes=Encoding.GetEncoding("big5").GetBytes(strPostHeader);
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.gb2312:
                    postHeaderBytes=Encoding.GetEncoding("gb2312").GetBytes(strPostHeader);
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.gbk:
                    postHeaderBytes=Encoding.GetEncoding("gbk").GetBytes(strPostHeader);
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.NoCoding:
                    postHeaderBytes=Encoding.ASCII.GetBytes(strPostHeader);
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.utf8:
                    postHeaderBytes=Encoding.UTF8.GetBytes(strPostHeader);
                    break;
            }

            // 根据uri创建HttpWebRequest对象 
            CookieContainer CookieCon = new CookieContainer();
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(uUrl));
            httpReq.Method = "POST";

            //对发送的数据不使用缓存 
            httpReq.AllowWriteStreamBuffering = false;

            //设置获得响应的超时时间（300秒） 
            httpReq.Timeout = 300000;

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


            CookieCon.Add(new Uri(uUrl), cl);
            httpReq.CookieContainer = CookieCon;

            httpReq.ContentType = "multipart/form-data; boundary=" + strBoundary;
            long length = fs.Length + postHeaderBytes.Length + boundaryBytes.Length;
            long fileLength = fs.Length;
            httpReq.ContentLength = length;
            try
            {
               
                //每次上传4k 
                int bufferLength = 4096;
                byte[] buffer = new byte[bufferLength];

                //已上传的字节数 
                long offset = 0;

                //开始上传时间 
                DateTime startTime = DateTime.Now;
                int size = r.Read(buffer, 0, bufferLength);
                Stream postStream = httpReq.GetRequestStream();

                //发送请求头部消息 
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    offset += size;
                    TimeSpan span = DateTime.Now - startTime;
                    double second = span.TotalSeconds;
                 
                    size = r.Read(buffer, 0, bufferLength);
                }
                //添加尾部的时间戳 
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                postStream.Close();

                //获取服务器端的响应 
                WebResponse webRespon = httpReq.GetResponse();
                Stream s = webRespon.GetResponseStream();
                StreamReader sr = new StreamReader(s);

                //读取服务器端返回的消息 
                String sReturnString = sr.ReadLine();
                s.Close();
                sr.Close();
                if (sReturnString == "Success")
                {
                    returnValue = 1;
                }
                else if (sReturnString == "Error")
                {
                    returnValue = 0;
                }

            }
            catch
            {
                returnValue = 0;
            }
            finally
            {
                fs.Close();
                r.Close();
            }

            return returnValue.ToString ();
        }

        /// <summary>
        /// 2014-5-4增加了上传文件的处理，主要的变化在于增加了上传图片与分类的对应，增加了4个参数
        /// </summary>
        /// <param name="uUrl">上传地址</param>
        /// <param name="uRurl">上传来路地址</param>
        /// <param name="fName"></param>
        /// <param name="strHeader"></param>
        /// <param name="uCode"></param>
        /// <param name="cookie"></param>
        /// <param name="uParas"></param>
        /// <param name="gParas"></param>
        /// <param name="rFormValue"></param>
        /// <param name="classTable"></param>
        /// <param name="publishParas"></param>
        /// <param name="dCols"></param>
        /// <param name="dRow"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        private string UploadFile(string uUrl, string uRurl,  string fName, string strHeader,cGlobalParas.WebCode uCode,
            string cookie, Dictionary<string, string> uParas, Hashtable gParas, Hashtable rFormValue,
            Hashtable classTable, List<ePublishData> publishParas, DataColumnCollection dCols, object[] Row, string ReferSource)
        {

            
          
            string boundaryValue = "";
            string webSource="";

            try
            {
                    
                string[] headers = Regex.Split(strHeader, "\r\n");
                string UploadHeader = "";

                //进行上传
                boundaryValue = "---------------------------7d" + GenerateRandomNumber(12);
                bool isContentTyp = false;
                for (int j = 0; j < headers.Length; j++)
                {
                    if (headers[j] != "")
                    {
                        if (headers[j].StartsWith("Content-Type", StringComparison.CurrentCultureIgnoreCase))
                        {
                            UploadHeader += "Content-Type: multipart/form-data; boundary=" + boundaryValue + "\r\n";
                            isContentTyp = true;
                        }
                        else
                        {
                            UploadHeader += headers[j] + "\r\n";
                        }
                    }
                }
                if (isContentTyp == false)
                {
                    UploadHeader += "Content-Type: multipart/form-data; boundary=" + boundaryValue + "\r\n";
                }

                //开始处理上传的参数
                string pData = "";
                foreach (KeyValuePair<string, string> de in uParas)
                {
                    string label = de.Key.ToString();
                    string value = de.Value.ToString();

                    if (value.StartsWith("{上传参数"))
                    {
                        if (value.IndexOf("当前上传的文件名")>0)
                        {
                            pData += "--" + boundaryValue + "\r\n";
                            pData += "Content-Disposition: form-data; name=\"" + label + "\"\r\n";
                            pData += "\r\n";
                            pData += Path.GetFileName (fName) + "\r\n";
                        }
                        else
                        {
                            value = fName;
                            pData += "--" + boundaryValue + "\r\n";
                            pData += "Content-Disposition: form-data; name=\"" + label + "\"; filename=\"" + Path.GetFileName(fName) + "\"\r\n";
                            if (Path.GetExtension(fName.Trim ()).ToLower().EndsWith("jpg"))
                                pData += "Content-Type: image/jpeg";
                            else if (Path.GetExtension(fName.Trim()).ToLower().EndsWith("png"))
                                pData += "Content-Type: image/png";
                            else if (Path.GetExtension(fName.Trim()).ToLower().EndsWith("gif"))
                                pData += "Content-Type: image/gif";
                            else if (Path.GetExtension(fName.Trim()).ToLower().EndsWith("rar"))
                                pData += "Content-Type: application/octet-stream";
                            else if (Path.GetExtension(fName.Trim()).ToLower().EndsWith("zip"))
                                pData += "Content-Type: application/octet-stream";
                            else if (Path.GetExtension(fName.Trim()).ToLower().EndsWith("txt"))
                                pData += "Content-Type: application/octet-stream";
                            pData += "\r\n";
                            pData += "\r\n";
                        }
                    }
                    else if (value == "{系统参数:取来路页面表单值}")
                    {
                        foreach (DictionaryEntry de1 in rFormValue)
                        {
                            if (label == de1.Key.ToString())
                            {
                                pData += "--" + boundaryValue + "\r\n";
                                pData += "Content-Disposition: form-data; name=\"" + label + "\"\r\n";
                                pData += "\r\n";
                                pData += de1.Value.ToString () + "\r\n";
                                break;
                            }
                        }
                    }
                    else if (value.StartsWith("{系统参数"))
                    {
                        if (value.StartsWith("{系统参数正则"))
                        {
                            //正则匹配
                            string valuesRegex = value.Substring(value.IndexOf("{系统参数正则:") + 8, value.IndexOf("}") - value.IndexOf("{系统参数正则:") - 8);
                            Match svaluesRegex = Regex.Match(ReferSource, valuesRegex, RegexOptions.IgnoreCase);
                            value = svaluesRegex.Groups[0].Value.ToString();

                            pData += "--" + boundaryValue + "\r\n";
                            pData += "Content-Disposition: form-data; name=\"" + label + "\"\r\n";
                            pData += "\r\n";
                            pData += value + "\r\n";
                        }
                        else
                        {
                            string pName = value.Substring(value.IndexOf("{系统参数:") + 6, value.IndexOf("}") - value.IndexOf("{系统参数:") - 6);

                            if (pName == "分类")
                            {
                                //现在发布规则中找分类对应的数据值，并取出
                                for (int i = 0; i < publishParas.Count; i++)
                                {
                                    if ("系统分类" == publishParas[i].DataLabel)
                                    {
                                        if (publishParas[i].DataType == cGlobalParas.PublishParaType.Gather)
                                        {
                                            //开始在采集的数据中进行匹配
                                            for (int j = 0; j < dCols.Count; j++)
                                            {
                                                if (publishParas[i].DataValue == dCols[j].ColumnName)
                                                {
                                                    value = Row[j].ToString();
                                                    break;
                                                }
                                            }
                                        }
                                        else if (publishParas[i].DataType == cGlobalParas.PublishParaType.Custom)
                                        {
                                            value = publishParas[i].DataValue;
                                        }
                                        else if (publishParas[i].DataType == cGlobalParas.PublishParaType.NoData)
                                        {
                                            value = "";
                                        }

                                        //开始将分类替换成ID，或者将分类ID替换成分类名称
                                        //判断依据为是否为数字
                                        foreach (DictionaryEntry de1 in classTable)
                                        {
                                            if (Regex.IsMatch(value, "^[0-9]*$"))
                                            {
                                                if (value == de1.Key.ToString())
                                                {
                                                    value = de1.Value.ToString();
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (value == de1.Value.ToString())
                                                {
                                                    value = de1.Key.ToString();
                                                    break;
                                                }
                                            }
                                        }
                                    }


                                }

                                pData += "--" + boundaryValue + "\r\n";
                                pData += "Content-Disposition: form-data; name=\"" + label + "\"\r\n";
                                pData += "\r\n";
                                pData += value + "\r\n";
                            }
                        }
                    }
                    else
                    {
                        pData += "--" + boundaryValue + "\r\n";
                        pData += "Content-Disposition: form-data; name=\"" + label + "\"\r\n";
                        pData += "\r\n";
                        pData += value + "\r\n";
                    }
                }

                pData += "--" + boundaryValue + "--\r\n";
               
                if (uRurl != "")
                    UploadHeader += "Referer:" + uRurl + "\r\n";

                FileInfo fInfo = new FileInfo(fName);
                UploadHeader += "Content-length: " + (fInfo.Length + pData.Length) + "\r\n\r\n";

                UploadHeader += pData;

                webSource = cHttpSocket.UploadFile(uUrl, UploadHeader, uCode, fName);

            }
            //捕获错误不处理
            catch { }

            return webSource;

        }

        public  string GenerateRandomNumber(int Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(10)]);
            }
            return newRandom.ToString();
        }

        public Hashtable GetWebClass(string tName,string domain,string cookie ,Hashtable gParas)
        {
            //先进行分类替换
            cTemplate t = new cTemplate(m_workPath);
            t.LoadTemplate(tName);

            string classUrl = t.ClassUrl;
            string classRurl = t.ClassRUrl;
            classUrl = classUrl.Replace(t.Domain, domain);
            classRurl = classRurl.Replace(t.Domain, domain);

            classUrl = ReplacePara(classUrl, gParas,null,null,null,null);
            classRurl = ReplacePara(classRurl, gParas, null, null,  null, null);
            

            if (classUrl == "")
            {
                t = null;
                return null;
            }

            

            string strHeader = "";

            //构建header头部
            if (t.IsHeader==false )
            {
                strHeader = "User-Agent: Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0\r\n";
                strHeader += "Accept-Language:zh-cn\r\n";
                strHeader += "Accept-Encoding:gzip,deflate\r\n";
                strHeader += "Connection: Keep-Alive\r\n";
            }
            else
            {
                strHeader = t.Headers + "\r\n";
            }

            strHeader += "Cookie:" + cookie + "\r\n";
            if (t.ClassRUrl != "")
                strHeader = "Referer:" + classRurl + "\r\n";

            strHeader += "\r\n";

            string strCookie;
            string webSource = cHttpSocket.GetUrl(classUrl, strHeader,  "GET",t.uCode,"",0, out strCookie);

            //将源码中的回车换行去掉
            webSource = Regex.Replace(webSource, "([\\r\\n])[\\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            webSource = Regex.Replace(webSource, "\\n", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            //拼接正则进行分类及ID的捕获
            if (t.ClassStartCut != "" && t.ClassEndCut != "" )
            {
                string Splitstr = "(" + t.ClassStartCut + ").*?(" + t.ClassEndCut + ")";

                Match aa = Regex.Match(webSource, Splitstr);
                webSource = aa.Groups[0].ToString();
            }

            string cRegex = t.ClassRegex;
            string conStr = "";

            if (cRegex.IndexOf("</Wildcard>") > 0)
            {
                string[] strSS = Regex.Split(cRegex, "</Wildcard>");
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

                cRegex = conStr;
            }

            //解析正则进行分类的获取
            cRegex=cRegex.Replace ("{Para:ClassID}","(?<ClassID>\\d+?)");
            cRegex =cRegex.Replace ("{Para:Class}","(?<Class>[^>]+?)");

            

            Hashtable classTable = new Hashtable();
            Regex re = new Regex(cRegex, RegexOptions.None);
            MatchCollection mc = re.Matches(webSource);
            foreach (Match ma in mc)
            {
                
                classTable.Add(ma.Groups["ClassID"].ToString(), ma.Groups["Class"].ToString());
            }

            t = null;
            return classTable;
        }

        private string ReplacePara(string oldStr, Hashtable gParas,Hashtable classPara,DataColumnCollection dCols,
            object dRow,List<ePublishData> publishParas)
        {
            if (oldStr == "")
                return oldStr;
         
            object[] Row = ((object[])dRow);

            #region 先替换全局参数
            if (gParas != null)
            {
                foreach (DictionaryEntry de in gParas)
                {
                    if (oldStr.IndexOf("{全局参数:") > 0)
                    {

                        Match aa1 = Regex.Match(oldStr, @"(?<={全局参数:).*(?=})", RegexOptions.IgnoreCase);
                        string p1 = aa1.Groups[0].Value.ToString();
                        if (p1.Trim() == de.Key.ToString())
                        {
                            //替换
                            oldStr = Regex.Replace(oldStr, "{全局参数:.*}", de.Value.ToString(), RegexOptions.IgnoreCase);
                        }
                    }
                }
            }
            #endregion 

            #region 开始替换分类信息
            if (oldStr.IndexOf ("{系统参数:分类编号")>0 || oldStr.IndexOf ("{系统参数:分类名称")>0  )
            {
                
                Match aa1 = Regex.Match(oldStr, @"(?<={系统参数:).*(?=})", RegexOptions.IgnoreCase);
                string p1 = aa1.Groups[0].Value.ToString();
                string value = "";
                
                //现在发布规则中找分类对应的数据值，并取出
                bool ismatch = false;
                for (int i = 0; i < publishParas.Count; i++)
                {
                    if ("系统分类" == publishParas[i].DataLabel)
                    {
                        if (publishParas[i].DataType == cGlobalParas.PublishParaType.Gather)
                        {
                            //开始在采集的数据中进行匹配
                            for (int j = 0; j < dCols.Count; j++)
                            {
                                if (publishParas[i].DataValue == dCols[j].ColumnName)
                                {
                                    value = Row[j].ToString();
                                    break;
                                }
                            }
                        }
                        else if (publishParas[i].DataType == cGlobalParas.PublishParaType.Custom)
                        {
                            value = publishParas[i].DataValue;
                        }
                        else if (publishParas[i].DataType == cGlobalParas.PublishParaType.NoData)
                        {
                            value = "";
                        }
                    }

                    //开始将分类替换成ID，或者将分类ID替换成分类名称
                    //判断依据为是否为数字
                    foreach (DictionaryEntry de1 in classPara)
                    {
                        if (p1 == "分类名称")
                        {
                            if (value == de1.Key.ToString())
                            {
                                value = de1.Value.ToString();
                                oldStr = Regex.Replace(oldStr, "{系统参数:分类名称}", value, RegexOptions.IgnoreCase);
                                ismatch = true;
                                break;
                            }
                        }
                        else if (p1 == "分类编号")
                        {
                            if (value == de1.Value.ToString())
                            {
                                value = de1.Key.ToString();
                                oldStr = Regex.Replace(oldStr, "{系统参数:分类编号}", value, RegexOptions.IgnoreCase);
                                ismatch = true;
                                break;
                            }
                        }
                    }

                    if (ismatch == true)
                        break;

                }

            }
            #endregion

            #region 替换发布参数
            while (oldStr.IndexOf ("{发布参数")>0)
            {
                Match aa2 = Regex.Match(oldStr, @"(?<={发布参数:).*(?=})", RegexOptions.IgnoreCase);
                string p2 = aa2.Groups[0].Value.ToString();

                for (int i = 0; i < publishParas.Count; i++)
                {
                    if (p2 == publishParas[i].DataLabel)
                        oldStr = oldStr.Replace("{发布参数:" + p2 + "}", publishParas[i].DataValue);
                }
            }
            #endregion

            return oldStr;
        }
        #endregion


        #region 模版数据库发布操作
        public bool PublishByDbTemplate(DataColumnCollection dCols, object dRow, string tName, cGlobalParas.PublishType dType,
            string strConn,List<ePublishData> publishParas)
        {
            object[] Row = ((object[])dRow);
            string lastValue = "";

            //定义一个列表进行值得存储
            //ArrayList sqlValues = new ArrayList();

            cDbTemplate t = new cDbTemplate(m_workPath);
            t.LoadTemplate(tName);
            

            ////获取sql参数
            List<cSqlPara> sParas = t.sqlParas;

            for (int m = 0; m < sParas.Count; m++)
            {
                string sql = sParas[m].Sql;

                Regex re = new Regex("(?<={).+?(?=})", RegexOptions.None);
                MatchCollection mc = re.Matches(sql);

                foreach (Match ma in mc)
                {
                    string ss = ma.ToString();
                    if (ss.StartsWith ("发布参数"))
                    {
                        string para = ss.Substring(5, ss.Length - 5);
                        string value = "";

                        //现在发布规则中找对应关系
                        for (int i = 0; i < publishParas.Count; i++)
                        {
                            if (para == publishParas[i].DataLabel)
                            {
                                if (publishParas[i].DataType == cGlobalParas.PublishParaType.Gather)
                                {
                                    //开始在采集的数据中进行匹配
                                    for (int j = 0; j < dCols.Count; j++)
                                    {
                                        if (publishParas[i].DataValue == dCols[j].ColumnName)
                                        {
                                            value = Row[j].ToString();
                                            break;
                                        }
                                    }
                                }
                                else if (publishParas[i].DataType == cGlobalParas.PublishParaType.Custom)
                                {
                                    value = publishParas[i].DataValue;
                                }
                                else if (publishParas[i].DataType == cGlobalParas.PublishParaType.NoData)
                                {
                                    value = "";
                                }
                            }
                        }

                        sql = sql.Replace("{发布参数:" + para + "}", value);
                    }
                    else if (ss.StartsWith ("系统参数"))
                    {
                        string para = ss.Substring(5, ss.Length - 5);
                        string value = "";
                        if (para.StartsWith("当前时间"))
                        {
                            value = System.DateTime.Now.ToString();
                        }
                        else if (para.StartsWith("上条语句返回值"))
                        {
                            value = lastValue;
                        }
                        sql = sql.Replace("{系统参数:" + para + "}", value);
                    }
                   
                }

                string lValue= ExcuteSql(sql, strConn, sParas[m].IsRepeat,sParas[m].PK,  t.DbType, (cGlobalParas.PublishSqlType)sParas[m].SqlType);

                if (lValue != null)
                    lastValue = lValue;
            }

            t = null;

            return true;
        }

        private string ExcuteSql(string sql,string strConn,bool isRepeat,string strPK, cGlobalParas.DatabaseType pType, cGlobalParas.PublishSqlType sType)
        {
            string lastValue = null;
            bool isRowsRepeat = false;

            if (pType == cGlobalParas.DatabaseType.Access)
            {
                OleDbConnection conn = new OleDbConnection();
                conn.ConnectionString = ToolUtil.DecodingDBCon(strConn);
                conn.Open();

                OleDbCommand com = new OleDbCommand();
                com.Connection = conn;
                com.CommandType = CommandType.Text;

                try
                {
                    if (isRepeat == true)
                    {
                        //将insert解析成select
                        string sql1 = ConvertSelect(sql, false, "");
                        com.CommandText = sql1;

                        int count = int.Parse(com.ExecuteScalar().ToString());
                        if (count > 0)
                        {
                            isRowsRepeat = true;

                            //获取主键
                             //获取主键
                            if (strPK != "")
                            {
                                string sql2 = ConvertSelect(sql, true, strPK);
                                com.CommandText = sql2;

                                lastValue = com.ExecuteScalar().ToString();
                            }

                        }
                    }

                    if (isRowsRepeat == false)
                    {
                        com.CommandText = sql;
                        if (sType == cGlobalParas.PublishSqlType.Common)
                            com.ExecuteNonQuery();
                        else if (sType == cGlobalParas.PublishSqlType.GetLastID)
                        {
                            com.ExecuteNonQuery();
                            com.CommandText = "SELECT @@IDENTITY";
                            lastValue = com.ExecuteScalar().ToString();
                        }
                        else if (sType == cGlobalParas.PublishSqlType.GetValues)
                        {
                            object c = com.ExecuteScalar();
                            if (c == null)
                                lastValue = null;
                            else
                                lastValue = c.ToString();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    com.Dispose();

                    conn.Close();
                    conn.Dispose();
                }
            }
            else if (pType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = ToolUtil.DecodingDBCon(strConn);
                conn.Open();

                SqlCommand com = new SqlCommand();
                com.Connection = conn;
                com.CommandType = CommandType.Text;
                try
                {
                    if (isRepeat == true)
                    {
                        //将insert解析成select
                        string sql1 = ConvertSelect(sql, false, "");
                        com.CommandText = sql1;

                        int count = 0;
                        try
                        {
                            count = int.Parse(com.ExecuteScalar().ToString());
                        }
                        catch (System.Exception ex)
                        {
                            throw new NetMinerException(ex.Message + " sql语句：" + sql1);
                        }
                        if (count > 0)
                        {
                            isRowsRepeat = true;

                            //获取主键
                             //获取主键
                            if (strPK != "")
                            {
                                string sql2 = ConvertSelect(sql, true, strPK);
                                com.CommandText = sql2;

                                lastValue = com.ExecuteScalar().ToString();
                            }

                        }
                    }

                    if (isRowsRepeat == false)
                    {

                        com.CommandText = sql;

                        if (sType == cGlobalParas.PublishSqlType.Common)
                            com.ExecuteNonQuery();
                        else if (sType == cGlobalParas.PublishSqlType.GetLastID)
                        {
                            com.CommandText += ";" + "SELECT @@IDENTITY";
                            lastValue = com.ExecuteScalar().ToString();
                        }
                        else if (sType == cGlobalParas.PublishSqlType.GetValues)
                        {
                            object c = com.ExecuteScalar();
                            if (c == null)
                                lastValue = null;
                            else
                                lastValue = c.ToString();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    com.Dispose();

                    conn.Close();
                    conn.Dispose();
                }
            }
            else if (pType == cGlobalParas.DatabaseType.MySql)
            {
               
                MySqlConnection conn = new MySqlConnection();
                conn.ConnectionString = ToolUtil.DecodingDBCon(strConn);
                conn.Open();

                MySqlCommand com = new MySqlCommand();
                com.Connection = conn;
                com.CommandType = CommandType.Text;
                try
                {
                    if (isRepeat == true)
                    {
                        //将insert解析成select
                        string sql1 = ConvertSelect(sql, false, "");
                        com.CommandText = sql1;

                        int count = int.Parse(com.ExecuteScalar().ToString());
                        if (count > 0)
                        {
                            isRowsRepeat = true;

                            //获取主键
                            if (strPK != "")
                            {
                                string sql2 = ConvertSelect(sql, true, strPK);
                                com.CommandText = sql2;

                                lastValue = com.ExecuteScalar().ToString();
                            }

                        }
                    }

                    if (isRowsRepeat == false)
                    {

                        com.CommandText = sql;

                        if (sType == cGlobalParas.PublishSqlType.Common)
                            com.ExecuteNonQuery();
                        else if (sType == cGlobalParas.PublishSqlType.GetLastID)
                        {
                            com.CommandText += ";" + "SELECT @@IDENTITY";
                            lastValue = com.ExecuteScalar().ToString();
                        }
                        else if (sType == cGlobalParas.PublishSqlType.GetValues)
                        {
                            object c = com.ExecuteScalar();
                            if (c == null)
                                lastValue = null;
                            else
                                lastValue = c.ToString();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    com.Dispose();

                    conn.Close();
                    conn.Dispose();
                }
            }

            return lastValue;
        }

        /// <summary>
        /// 将insert into 语句替换成select
        /// </summary>
        /// <param name="sql">insert sql</param>
        /// <param name="isCount">替换成select后，是否获取ID</param>
        /// <returns></returns>
        private string ConvertSelect(string sql,bool isID,string ID)
        {
            //sql=sql.ToLower ();

            string field = sql.Substring(sql.IndexOf("(") + 1, sql.IndexOf(")") - sql.IndexOf("(") - 1);
            string[] fields = field.Split(',');

            string value = sql.Substring(sql.LastIndexOf("(") + 1, sql.LastIndexOf(")") - sql.LastIndexOf("(") - 1);
            string[] values = value.Split(',');

            ArrayList newValues = new ArrayList();

            //值有可能分的不对
            if (fields.Length != values.Length)
            {
                //对值的部分进行重新划分
                for (int i = 0; i < values.Length; i++)
                {
                    //判断依据为是否为一个完成的单引号
                    string ss = values[i];
                    if (ss.StartsWith("'") && ss.EndsWith("'"))
                    {
                        //完整的值
                        newValues.Add(ss);
                    }
                    else if (ss.StartsWith("'") && !ss.EndsWith("'"))
                    {
                        //标识要和后一个进行合并
                        if (i - 1 == values.Length)
                        {
                            //标识的是最后一个字段
                            newValues.Add(ss + "'");
                        }
                        else
                        {
                            string s1 = values[1 + 1];
                            if (!s1.StartsWith("'"))
                            {
                                newValues.Add(ss + s1);
                            }
                            else
                            {
                                newValues.Add(ss + "'");
                            }
                        }
                    }

                }

                if (fields.Length != newValues.Count)
                {
                    return "";
                }
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                    newValues.Add(values[i]);
            }

            //开始进行select的拼接
            string newSql = "";

            Match a = Regex.Match(sql, @"(?<=insert\s+into).+?(?=\()", RegexOptions.IgnoreCase);
            string TableName = a.Groups[0].Value.ToString();

            if (isID ==true )
                newSql = "select " + ID + " from " + TableName
                    + " where ";
            else
                newSql = "select count(*) from " + TableName
                    + " where ";

            for (int i = 0; i < fields.Length; i++)
            {
                newSql += " " + fields[i] + "=" + newValues[i].ToString() + " and";
            }

            if (newSql.EndsWith("and"))
            {
                newSql = newSql.Substring(0, newSql.Length - 3);
            }
            return newSql;
        }

        #endregion


      
    }
}

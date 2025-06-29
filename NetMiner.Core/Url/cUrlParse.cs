using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NetMiner.Resource;
using System.Data;
using Gecko;
using NetMiner.Core.Dict;
using NetMiner.Common;

namespace NetMiner.Core.Url
{
    /// <summary>
    /// Url解析类，主要用于url的参数解析，网址总数的计算。支持数据库参数。
    /// </summary>
    public class cUrlParse
    {

        private string m_workPath;

        public cUrlParse (string workPath)
        {
            m_workPath = workPath;
        }

        /// <summary>
        /// 从一段html代码中获取网址
        /// </summary>
        /// <param name="NavUrl"></param>
        /// <param name="htmlSource"></param>
        /// <param name="UrlRule"></param>
        /// <returns></returns>
        public string GetUrlsByRule(string inputurl, string html)
        {
            if (!inputurl.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
                inputurl = "http://" + inputurl;

            string url = html;
            Match s = Regex.Match(url, "(?<=href=)[^>]+?(?=[>|'|\"|\\s])", RegexOptions.IgnoreCase);
            url = s.Groups[0].Value.ToString();
            //只删除头和尾的引号
            url = Regex.Replace(url, "^[\"|']", "");
            url = Regex.Replace(url, "[\"|']$", "");

            if (!string.IsNullOrEmpty(url) && !url.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
            {
                //string PreUrl = inputurl;

                url = NetMiner.Common.ToolUtil.RelativeToAbsoluteUrl(inputurl, url);

                //url = PreUrl + url;

                //需要处理转义，导航的网址中如果出现{ }，需要转义
                url = url.Replace("{", "\\{");
                url = url.Replace("}", "\\}");
                url = NetMiner.Common.ToolUtil.ConvertJsonUrl(url);

            }

            url = System.Web.HttpUtility.HtmlDecode(url);

            return url;
        }


        //拆分网址
        /// <summary>
        /// 根据网址提供的参数对网址进行分解处理,增加了多条网址的处理，用;分割
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public List<string> SplitWebUrl(string Url)         //, bool IsUrlEncode
        {
            List<string> Urls = new List<string>();

            try
            {
                //string[] sUrls = Regex.Split(Url, "\r\n");
                string[] sUrls = null;

                if (Url.IndexOf("<POST") > 0 && Url.IndexOf("</POST>") > 0)
                {
                    sUrls = new string[1];
                    sUrls[0] = Url;
                }
                else
                    sUrls = Regex.Split(Url, "\r\n");

                for (int i = 0; i < sUrls.Length; i++)
                {
                    List<string> tUrls = SplitUrl(sUrls[i]);
                    Urls.AddRange(tUrls);
                }
            }
            catch (System.Exception ex)
            {
                throw new NetMinerException(ex.Message);
            }

            return Urls;                                  //, IsUrlEncode, ""
        }

        public List<string> SplitUrl(string Url)
        {
            List<string> list_Url = new List<string>();

            if (Url.Contains("{DbUrl:"))
            {
                #region 数据库提取地址数据

                //提取dburl参数
                Match charSetMatch = Regex.Match(Url, "(?<={DbUrl:)[\\s\\S]*?(?=})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string dbUrl = charSetMatch.Groups[0].ToString();

                string dbPreUrl = Url.Substring(0, Url.IndexOf("{DbUrl:"));
                string dbSuffUrl = string.Empty;

                if (string.IsNullOrEmpty(dbPreUrl))
                    dbSuffUrl = Url.Replace("{DbUrl:" + dbUrl + "}", "");
                else
                    dbSuffUrl = Url.Replace(dbPreUrl, "").Replace("{DbUrl:" + dbUrl + "}", "");


                charSetMatch = Regex.Match(dbUrl, "(?<=<DbType>)[\\s\\S]*?(?=</DbType>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string dType = charSetMatch.Groups[0].ToString();

                charSetMatch = Regex.Match(dbUrl, "(?<=<Con>)[\\s\\S]*?(?=</Con>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string strCon = ToolUtil.DecodingDBCon(charSetMatch.Groups[0].ToString());

                charSetMatch = Regex.Match(dbUrl, "(?<=<Sql>)[\\s\\S]*?(?=</Sql>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string sql = charSetMatch.Groups[0].ToString();

                DataTable d = null;
                switch (int.Parse(dType))
                {
                    case (int)cGlobalParas.DatabaseType.Access:
                        d = NetMiner.Data.Access.SQLHelper.ExecuteDataTable(strCon, sql);
                        break;
                    case (int)cGlobalParas.DatabaseType.MSSqlServer:
                        d = NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(strCon, sql);
                        break;
                    case (int)cGlobalParas.DatabaseType.MySql:
                        d = NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(strCon, sql);
                        break;
                }

                for (int i = 0; i < d.Rows.Count; i++)
                {
                    //根据数据库调用，合成Url
                    list_Url.Add(dbPreUrl + d.Rows[i][0].ToString() + dbSuffUrl);
                }

                #endregion

            }
            else
                list_Url.Add(Url);

            List<string> r_Urls = new List<string>();

            for (int i = 0; i < list_Url.Count; i++)
            {
                List<string> urls = SplitParaUrl(list_Url[i]);
                r_Urls.AddRange(urls);
            }

            return r_Urls;
        }

        private List<string> SplitParaUrl(string Url)   //, bool IsUrlEncode, string UrlEncode
        {
            List<string> tmp_list_Url = new List<string>();
            List<string> list_Url;
            List<string> g_Url = new List<string>();
            string Para = "";
            string oldUrl = Url;
            bool isSynPara = false;

            //在此先判断是否为db提取网址的参数
            //从5.41版本开始，数据库参数为动态参数



            #region 此部分分解url参数
            if (Url.IndexOf("{Syn:") > -1)
                isSynPara = true;

            //加了转义符号的识别，如果有转义符，则按照正常的{}处理，转义符为\
            //if (!Regex.IsMatch(Url, "{.*}"))
            //判断是否有参数，如果没有参数，则直接返回
            if (!Regex.IsMatch(Url, "[^\\\\]{.*[^\\\\]}", RegexOptions.Multiline))
            {
                if (Regex.IsMatch(Url, "\\\\{"))
                {
                    //表明是转义符号，处理
                    Url = Url.Replace("\\{", "{");
                    Url = Url.Replace("\\}", "}");
                }

                tmp_list_Url.Add(Url);
                return tmp_list_Url;
            }

            //增加tmp_list_Url的初始值
            //初始值为Url第一个参数前所有字符
            //应该以{为准
            //通过正则获取，改变以前的的获取方式，否则如果有大括号转义会出错
            string strPreMatch = "[\\s\\S]+?[^\\\\](?={)";
            Match urlPre = Regex.Match(Url, strPreMatch, RegexOptions.IgnoreCase);
            string strUrlPre = urlPre.Groups[0].Value;
            tmp_list_Url.Add(strUrlPre);

            int first = 0;
            while (Regex.IsMatch(Url, "[^\\\\]?{.*[^\\\\]?}"))
            {
                //提取参数内容
                //string strMatch = "(?<={)[^}]*(?=})";
                //加了转义符号的识别，如果有转义符，则按照正常的{}处理，转义符为\
                //获取参数
                string strMatch = "(?<={)[^{]*?[^\\\\](?=})";
                Match s = Regex.Match(Url, strMatch, RegexOptions.IgnoreCase);
                string UrlPara = s.Groups[0].Value;

                if (string.IsNullOrEmpty(UrlPara))
                {
                    break;
                }

                g_Url = getListUrl(UrlPara); //,IsUrlEncode ,UrlEncode 

                list_Url = new List<string>();

                for (int j = 0; j < tmp_list_Url.Count; j++)
                {
                    if (g_Url != null && g_Url.Count > 0)
                    {
                        for (int i = 0; i < g_Url.Count; i++)
                        {
                            try
                            {
                                list_Url.Add(tmp_list_Url[j].ToString() + Para + g_Url[i].ToString());
                            }
                            catch (System.Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                    else
                    {
                        list_Url.Add(tmp_list_Url[j].ToString() + Para + "{" + UrlPara + "}");

                    }
                }

                tmp_list_Url = list_Url;
                list_Url = null;

                string strSufMatch = "(?<=[^\\\\]})[\\s\\S]*";
                Match urlSuf = Regex.Match(Url, strSufMatch, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string strUrlSuf = urlSuf.Groups[0].Value;
                Url = strUrlSuf;

                //判断是否还有参数，如果有，则截取中间部分以拼接网址
                if (Regex.IsMatch(Url, "[^\\\\]?{.*[^\\\\]?}"))
                {
                    //Para = Url.Substring(Url.IndexOf("&"), Url.IndexOf("=") - Url.IndexOf("&") + 1);
                    //Para = Url.Substring(0, Url.IndexOf("{"));

                    string strMidMatch = "[^{]*[^\\\\](?={)";
                    Match urlMid = Regex.Match(Url, strMidMatch, RegexOptions.IgnoreCase);
                    string strUrlMid = urlMid.Groups[0].Value;

                    Para = strUrlMid;
                }

                first++;
            }

            #region 处理同步参数
            if (isSynPara == true)
            {
                //提取一个实例网址出来，判断是否有同步参数

                //存在同步参数
                string strMatch = "(?<={Syn:)[^}]*(?=})";
                Match s = Regex.Match(oldUrl, strMatch, RegexOptions.IgnoreCase);
                string UrlPara = s.Groups[0].Value;

                List<string> s_Url = getSynUrl(UrlPara);
                int sI = 0;
                for (int i = 0; i < tmp_list_Url.Count; i++)
                {
                    int sIndex = tmp_list_Url[i].IndexOf("{Syn:");
                    string s1 = tmp_list_Url[i].Substring(0, sIndex);
                    string s2 = tmp_list_Url[i].Substring(tmp_list_Url[i].IndexOf("}", sIndex) + 1, tmp_list_Url[i].Length - tmp_list_Url[i].IndexOf("}", sIndex) - 1);

                    if (sI >= s_Url.Count)
                        sI = 0;
                    tmp_list_Url[i] = s1 + s_Url[sI] + s2;
                    sI++;
                }

            }
            #endregion

            list_Url = new List<string>();

            for (int m = 0; m < tmp_list_Url.Count; m++)
            {
                list_Url.Add(tmp_list_Url[m].ToString() + Url);
            }

            tmp_list_Url = null;
            g_Url = null;
            #endregion



            return list_Url;

        }

        /// <summary>
        /// 分解指定的参数，与业务逻辑无关
        /// </summary>
        /// <param name="dicPre"></param>
        /// <returns></returns>
        public List<string> getListUrl(string dicPre)//,bool IsUrlEncode,string UrlEncode
        {
            List<string> list_Para = new List<string>();
            Regex re;
            MatchCollection aa;
            int step;
            int startI;
            int endI;
            int i = 0;

            string split1 = string.Empty;
            string split2 = string.Empty;
            string tmpDic = string.Empty;

            string startstrI = string.Empty;
            string endstrI = string.Empty;

            if (dicPre.IndexOf(":") < 0)
            {
                list_Para.Add(dicPre);
            }
            else
            {
                if (dicPre.StartsWith("Date("))
                {
                    try
                    {
                        //提取出日期的格式
                        string dateFormat = dicPre.Substring(dicPre.IndexOf("(") + 1, dicPre.IndexOf(")") - dicPre.IndexOf("(") - 1);

                        //提取日期的范围
                        string sDate = dicPre.Substring(dicPre.IndexOf(":") + 1, dicPre.IndexOf(",") - dicPre.IndexOf(":") - 1);
                        string eDate = dicPre.Substring(dicPre.IndexOf(",") + 1, dicPre.Length - dicPre.IndexOf(",") - 1);

                        for (DateTime iDate = DateTime.Parse(sDate); iDate <= DateTime.Parse(eDate); iDate = iDate.AddDays(1))
                        {
                            list_Para.Add(iDate.ToString(dateFormat));
                        }
                    }
                    catch (System.Exception ex)
                    {
                        return list_Para;
                    }

                }
                else
                {
                    switch (dicPre.Substring(0, dicPre.IndexOf(":")))
                    {

                        case "Num":

                            if (dicPre.IndexOf("/") > 0)
                            {
                                //表示为计算最大值得处理
                                string[] ss = dicPre.Split(',');
                                startI = int.Parse(ss[0].Replace("Num:", ""));
                                step = int.Parse(ss[2]);

                                try
                                {
                                    string[] ss1 = ss[1].Split('/');
                                    if (string.IsNullOrEmpty(ss1[0]))
                                    {
                                        endI = startI;
                                    }
                                    else
                                    {
                                        endI = int.Parse(ss1[0]) / int.Parse(ss1[1]);

                                        if (int.Parse(ss1[0]) % int.Parse(ss1[1]) > 0)
                                            endI = endI + 1;

                                        if (endI < startI)
                                            endI = startI;
                                    }
                                }
                                catch { endI = 0; }
                            }
                            else
                            {
                                re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                aa = re.Matches(dicPre);

                                startI = int.Parse(aa[0].Groups[0].Value.ToString());
                                endI = int.Parse(aa[1].Groups[0].Value.ToString());
                                step = int.Parse(aa[2].Groups[0].Value.ToString());
                            }

                            if (step > 0)
                            {
                                for (i = startI; i <= endI; i = i + step)
                                {
                                    list_Para.Add(i.ToString());
                                }
                            }
                            else
                            {
                                for (i = startI; i >= endI; i = i + step)
                                {
                                    list_Para.Add(i.ToString());
                                }
                            }



                            break;

                        case "NumZero":
                            re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            aa = re.Matches(dicPre);

                            startstrI = aa[0].Groups[0].Value.ToString();
                            endstrI = aa[1].Groups[0].Value.ToString();
                            step = int.Parse(aa[2].Groups[0].Value.ToString());
                            startI = int.Parse(aa[0].Groups[0].Value.ToString());
                            endI = int.Parse(aa[1].Groups[0].Value.ToString());

                            int NumLen = 0;

                            if (step > 0)
                            {
                                NumLen = endstrI.Length;
                                for (i = startI; i <= endI; i = i + step)
                                {
                                    string tempI = i.ToString();
                                    while (tempI.Length < NumLen)
                                    {
                                        tempI = "0" + tempI;
                                    }
                                    list_Para.Add(tempI);
                                }
                            }
                            else
                            {
                                NumLen = startstrI.ToString().Length;

                                for (i = startI; i >= endI; i = i + step)
                                {
                                    string tempI = i.ToString();
                                    while (tempI.Length < NumLen)
                                    {
                                        tempI = "0" + tempI;
                                    }
                                    list_Para.Add(tempI);
                                }
                            }
                            break;



                        case "Letter":
                            startI = getAsc(dicPre.Substring(dicPre.IndexOf(":") + 1, 1));
                            endI = getAsc(dicPre.Substring(dicPre.IndexOf(",") + 1, 1));

                            if (startI > endI)
                            {
                                step = -1;
                            }
                            else
                            {
                                step = 1;
                            }

                            for (i = startI; i <= endI; i = i + step)
                            {
                                char s;
                                s = Convert.ToChar(i);
                                list_Para.Add(s.ToString());
                            }

                            break;

                        case "CurrentDate":
                            dicPre = dicPre.Replace("CurrentDate:", "");
                            list_Para.Add(System.DateTime.Now.ToString(dicPre));
                            break;

                        case "Timestamp":
                            dicPre = dicPre.Replace("CurrentDate:", "");
                            list_Para.Add(ToolUtil.GetTimestamp().ToString());
                            break;

                        case "Dict":
                            oDict d = new oDict(m_workPath);
                            string tClass = dicPre.Substring(dicPre.IndexOf(":") + 1, dicPre.Length - dicPre.IndexOf(":") - 1);
                            ArrayList dName = d.GetDict(tClass);

                            if (dName != null)
                            {
                                for (i = 0; i < dName.Count; i++)
                                {
                                    list_Para.Add(dName[i].ToString());
                                }
                            }

                            if (list_Para.Count > 0)
                            {
                                if (list_Para[0].IndexOf("{") > -1)
                                {
                                    //表明还有参数
                                    List<string> list_para1 = new List<string>();
                                    for (i = 0; i < list_Para.Count; i++)
                                    {
                                        List<string> paras = SplitUrl(list_Para[i]);
                                        list_para1.AddRange(paras);
                                    }
                                    list_Para = list_para1;
                                }
                            }

                            break;

                        default:
                            list_Para.Add("{" + dicPre + "}");
                            break;
                    }
                }
            }

            return list_Para;
        }

        public List<string> getSynUrl(string dicPre)
        {
            List<string> list_Para = new List<string>();
            Regex re;
            MatchCollection aa;
            int step;
            int startI;
            int endI;
            int i = 0;

            string startstrI = string.Empty;
            string endstrI = string.Empty;

            re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            aa = re.Matches(dicPre);

            startI = int.Parse(aa[0].Groups[0].Value.ToString());
            endI = int.Parse(aa[1].Groups[0].Value.ToString());
            step = int.Parse(aa[2].Groups[0].Value.ToString());

            if (step > 0)
            {
                for (i = startI; i <= endI; i = i + step)
                {
                    list_Para.Add(i.ToString());
                }
            }
            else
            {
                for (i = startI; i >= endI; i = i + step)
                {
                    list_Para.Add(i.ToString());
                }
            }

            return list_Para;
        }

        public string getSynUrl(string dicPre, int firstIndex)
        {
            List<string> list_Para = new List<string>();
            Regex re;
            MatchCollection aa;
            int step;
            int startI;
            int endI;
            int i = 0;

            string startstrI = string.Empty;
            string endstrI = string.Empty;
            int dValue = 0;

            re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            aa = re.Matches(dicPre);

            startI = int.Parse(aa[0].Groups[0].Value.ToString());
            endI = int.Parse(aa[1].Groups[0].Value.ToString());
            step = int.Parse(aa[2].Groups[0].Value.ToString());

            if (firstIndex > endI)  //标识循环已经超过了指定的最大范围
                return "";

            dValue = 0;
            try
            {

                dValue = startI + firstIndex * step;

                firstIndex++;

                if (dValue > endI)
                    dValue = endI;





            }
            catch (System.Exception)
            {


            }

            return dValue.ToString();
        }

        private int getAsc(string s)
        {
            byte[] array = new byte[1];
            array = System.Text.Encoding.ASCII.GetBytes(s);
            int asciicode = (int)(array[0]);
            return asciicode;
        }

        //判断当前所含参数的数量,其中包括字典的内容
        public int GetUrlCount(string Url)
        {
            if (Url == "")
                return 0;

            int UrlCount = 1;
            int SumUrlCount = 0;
            List<string> g_Url = null;

            string[] sUrls = null;

            //在此处理如果post数据中，有可能存在换行的情况，因此需要判断
            //这是一个网址，不能切分成多个网址，如果存在此情况，按照一个网址处理
            if (Url.IndexOf("<POST") > 0 && Url.IndexOf("</POST>") > 0)
            {
                sUrls = new string[1];
                sUrls[0] = Url;
            }
            else
                sUrls = Regex.Split(Url, "\r\n");

            for (int index = 0; index < sUrls.Length; index++)
            {
                g_Url = new List<string>();
                Url = sUrls[index];
                UrlCount = 1;

                #region 计算数据库网址
                if (Url.Contains("{DbUrl:"))
                {
                    Match charSetMatch = Regex.Match(Url, "(?<=<DbType>)[\\s\\S]*?(?=</DbType>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string dType = charSetMatch.Groups[0].ToString();

                    charSetMatch = Regex.Match(Url, "(?<=<Con>)[\\s\\S]*?(?=</Con>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string strCon = ToolUtil.DecodingDBCon(charSetMatch.Groups[0].ToString());

                    charSetMatch = Regex.Match(Url, "(?<=<Sql>)[\\s\\S]*?(?=</Sql>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string sql = charSetMatch.Groups[0].ToString();

                    try
                    {
                        if ((sql.ToLower().Contains(" top ") && int.Parse(dType) == (int)cGlobalParas.DatabaseType.Access)
                            || (sql.ToLower().Contains(" top ") && int.Parse(dType) == (int)cGlobalParas.DatabaseType.MSSqlServer))
                        {
                            //提取top后面的数字
                            Match m = Regex.Match(sql, "(?<=top\\s*)\\d+?(?=\\s)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            if (m != null)
                                UrlCount = int.Parse(m.Groups[0].ToString());
                        }
                        else if (sql.ToLower().Contains(" limit ") && int.Parse(dType) == (int)cGlobalParas.DatabaseType.MySql)
                        {
                            Match m = Regex.Match(sql, "(?<=limit\\s*\\d*\\s*,\\s*)\\d+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            if (m != null)
                                UrlCount = int.Parse(m.Groups[0].ToString());
                        }
                        else
                        {
                            //将sql转换成Count（*）
                            //string[] ss = sql.Split(' ');
                            //string sql1 = string.Empty;
                            //for (int i = 0; i < ss.Length; i++)
                            //{
                            //    if (i == 0)
                            //    {
                            //        sql1 += "select count(";
                            //    }
                            //    else if (i == 1)
                            //    {
                            //        sql1 += ss[i].Trim() + ") ";
                            //    }
                            //    else
                            //    {
                            //        sql1 += ss[i] + " ";
                            //    }
                            //}

                            sql = sql.ToLower();
                            sql = "select count(1) " + sql.Substring(sql.IndexOf("from"), sql.Length - sql.IndexOf("from"));

                            //sql = sql1.Trim();

                            switch (int.Parse(dType))
                            {
                                case (int)cGlobalParas.DatabaseType.Access:
                                    UrlCount = int.Parse(NetMiner.Data.Access.SQLHelper.ExecuteScalar(strCon, sql).ToString());
                                    break;
                                case (int)cGlobalParas.DatabaseType.MSSqlServer:
                                    UrlCount = int.Parse(NetMiner.Data.SqlServer.SQLHelper.ExecuteScalar(strCon, sql).ToString());
                                    break;
                                case (int)cGlobalParas.DatabaseType.MySql:
                                    UrlCount = int.Parse(NetMiner.Data.Mysql.SQLHelper.ExecuteScalar(strCon, sql).ToString());
                                    break;
                            }
                        }
                    }
                    catch
                    {
                        UrlCount = 0;
                    }

                }
                #endregion


                #region  计算参数类型
                //加了转义符号的识别，如果有转义符，则按照正常的{}处理，转义符为\
                //if (!Regex.IsMatch(Url, "{.*}"))
                if (!Regex.IsMatch(Url, "[^\\\\]{.*[^\\\\]}"))
                {
                    if (Regex.IsMatch(Url, "\\\\{"))
                    {
                        //表明是转义符号，处理
                        Url = Url.Replace("\\{", "{");
                        Url = Url.Replace("\\}", "}");
                    }

                    g_Url.Add(Url);
                    UrlCount = g_Url.Count;
                }
                else
                {
                    while (Regex.IsMatch(Url, "[^\\\\]?{.*[^\\\\]?}"))
                    {
                        //提取参数内容
                        //string strMatch = "(?<={)[^}]*(?=})";
                        //加了转义符号的识别，如果有转义符，则按照正常的{}处理，转义符为\
                        string strMatch = "(?<={)[^{]*[^\\\\](?=})";
                        Match s = Regex.Match(Url, strMatch, RegexOptions.IgnoreCase);
                        string UrlPara = s.Groups[0].Value;

                        //因为仅计算网址的数量，所以不需要对url进行编码转换，如果有需要转换的话，也不需要
                        g_Url = getListUrl(UrlPara);

                        //if (g_Url == null)
                        //    return 0;

                        if (g_Url != null)
                            UrlCount = UrlCount * g_Url.Count;

                        Url = Url.Substring(Url.IndexOf("}") + 1, Url.Length - Url.IndexOf("}") - 1);

                    }
                }
                #endregion

                if (UrlCount == 0)
                {
                    UrlCount = 1;
                }

                SumUrlCount += UrlCount;
            }

            return SumUrlCount;
        }
    }
}

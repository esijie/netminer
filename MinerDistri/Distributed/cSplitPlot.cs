using System;
using System.Collections;
using System.Collections.Generic;
using NetMiner.Gather.Task;
using NetMiner.Gather.Control;
using NetMiner.Core.Proxy;
using NetMiner.Resource;
using System.IO;
using System.Data;
using NetMiner.Common;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using NetMiner.Core;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;
using NetMiner.Base;
using NetMiner.Gather.Url;
using NetMiner.Net;
using NetMiner.Net.Common;
using NetMiner.Core.Proxy.Entity;
using System.Net;

///采集任务分解策略维护类
namespace MinerDistri.Distributed
{
    /// <summary>
    /// 采集任务分解网址类，用于将一个采集任务分解成为多个子任务，进行分布式采集操作
    /// </summary>
    /// 
    public class cSplitPlot
    {
        private int m_MaxSplitMaxUrlsCount = 200;
        private string m_TaskPath = string.Empty;
        private string m_Conn = string.Empty;
        private cGlobalParas.DatabaseType m_dbType;
        private int m_MaxSplitLevel;
        public string m_workPath = string.Empty;

        private Queue queueURLS;
        private Queue queueURLSSpliting;
        /// <summary>
        /// 用于存储已经完成分解的网址（包括翻页和导航），这样做的目的是可以记录数据，防止翻页的时候陷入死循环
        /// </summary>
        private Hashtable hOverUrls;

        private List<eWebLink> m_gUrls;

        private Thread[] threadsRun;
        private string m_RegexNextPage;
        private cProxyControl pControl = null;
        private const int ThreadCount = 3;
        private const int MaxErrCount = 10;

        private bool m_IsRunning = false;

        public cSplitPlot(string workPath, cGlobalParas.DatabaseType dbType, string con, int MaxSplitUrls, int MaxSplitLevel, string taskPath)
        {
            m_workPath = workPath;

            m_Conn = con;
            m_dbType = dbType;

            this.m_MaxSplitMaxUrlsCount = MaxSplitUrls;
            this.m_MaxSplitLevel = MaxSplitLevel;
            this.m_TaskPath = taskPath;

            m_gUrls = new List<eWebLink>();
            queueURLS = new Queue();
            queueURLSSpliting = new Queue();
            hOverUrls = new Hashtable();

            cXmlSConfig cCon = new cXmlSConfig(this.m_workPath);
            m_RegexNextPage = ToolUtil.ShowTrans(cCon.RegexNextPage);
            cCon = null;

            pControl = new cProxyControl(this.m_workPath);

            onErrorSend += new onErrorEventHandler(splitUrl_onError);
        }

        ~cSplitPlot()
        {
            m_gUrls = null;
            queueURLS = null;
            queueURLSSpliting = null;
            hOverUrls = null;
        }

        private void splitUrl_onError(object sender, onErrorEventArgs e)
        {
           //处理分解的错误
            if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
            {
                string sql = "insert into SM_RunningLog(TaskID,TaskName,Type,Message,LogDate) values (@TaskID,@TaskName,@Type,@Message,@LogDate)";

                try
                {
                    SqlParameter[] parameters = {
                                       new SqlParameter("@TaskID" , SqlDbType.Decimal),
                                       new SqlParameter("@TaskName" , SqlDbType.VarChar,250),
                                       new SqlParameter("@Type" , SqlDbType.Int),
                                       new SqlParameter("@Message" , SqlDbType.VarChar,1000),
                                       new SqlParameter("@LogDate" , SqlDbType.DateTime)
                                        };
                    parameters[0].Value = "-1";
                    parameters[1].Value = e.TaskName;
                    parameters[2].Value = (int)cGlobalParas.LogType.Error;
                    parameters[3].Value = e.Url + ":" + e.errMess;
                    parameters[4].Value = System.DateTime.Now.ToString();

                    NetMiner.Data.SqlServer. SQLHelper.ExecuteNonQuery(m_Conn, sql, parameters);
                }
                catch (System.Exception ex)
                {

                }
            }
            else if (m_dbType == cGlobalParas.DatabaseType.MySql)
            {
                string sql = "insert into SM_RunningLog(TaskID,TaskName,Type,Message,LogDate) values"
                    + " (?TaskID,?TaskName,?Type,?Message,?LogDate)";

                try
                {
                    MySqlParameter[] parameters = {
                                       new MySqlParameter("@TaskID" ,MySqlDbType.Decimal),
                                       new MySqlParameter("@TaskName" , MySqlDbType.VarChar,250),
                                       new MySqlParameter("@Type" , MySqlDbType.Int16),
                                       new MySqlParameter("@Message" , MySqlDbType.VarChar,1000),
                                       new MySqlParameter("@LogDate" , MySqlDbType.DateTime)
                                        };
                    parameters[0].Value = "-1";
                    parameters[1].Value = e.TaskName;
                    parameters[2].Value = (int)cGlobalParas.LogType.Error;
                    parameters[3].Value = e.Url + ":" + e.errMess;
                    parameters[4].Value = System.DateTime.Now.ToString();

                    NetMiner.Data.Mysql. SQLHelper.ExecuteNonQuery(m_Conn, sql, parameters);
                }
                catch (System.Exception ex)
                {

                }
            }
        
        }


        //定义代理用于新增一个需要检索的网址
        public delegate void onErrorEventHandler(object sender, onErrorEventArgs e);
        public event onErrorEventHandler onErrorSend;
        public void OnonErrorSend(object sender, onErrorEventArgs e)
        {
            if (onErrorSend != null)
                this.onErrorSend(sender, e);
        }


        //启动线程
        private List<cSplitTaskEntity> SplitTask(oTask t)
        {
            int startTaskIndex = 1;
            List<cSplitTaskEntity> sTasks = new List<cSplitTaskEntity>();

            //采用10个线程进行网址分解
            threadsRun = new Thread[ThreadCount];

            NetMiner.Core.Url.cUrlParse cu = new NetMiner.Core.Url.cUrlParse(this.m_workPath);

            //先将地址压入待分解队列
            for (int i = 0; i < t.TaskEntity.WebpageLink.Count; i++)
            {
                string url = t.TaskEntity.WebpageLink[i].Weblink;

                //只有入口地址才可能存在base64的编码
                if (Regex.IsMatch(url, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
                {

                    Match s = Regex.Match(url, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
                    string sBase64 = s.Groups[0].Value.ToString();
                    sBase64 = ToolUtil.Base64Encoding(sBase64);

                    //将base64编码部分进行url替换
                    url = Regex.Replace(url, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                }

                //如果网址中有参数，现将其分解
                List<string> tmpUrl = cu.SplitWebUrl(url);

                for (int j = 0; j < tmpUrl.Count; j++)
                {

                    eWebLink wLink = CopyWeblink(tmpUrl[j], t.TaskEntity.WebpageLink[i], 0);

                    //cSplitUrl su = new cSplitUrl();
                    //su.link = wLink;
                    //su.sState = cGlobalParas.SplitTaskState.UnSplit;

                    EnqueueUri(wLink);
                }

            }

            cu = null;

            if (t.TaskEntity.IsSplitDbUrls == true)
            {
                
                eWebLink wLink = (eWebLink)queueURLS.Dequeue();
                while (wLink != null)
                {
                    try
                    {
                            m_gUrls.Add(wLink);
                            if (queueURLS.Count == 0)
                                break;
                            wLink = (eWebLink)queueURLS.Dequeue();
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                }
               
            }
            else
            {
                m_IsRunning = true;
                //按照规定的线程数进行线程创建
                for (int nIndex = 0; nIndex < ThreadCount; nIndex++)
                {
                    if (threadsRun[nIndex] == null || threadsRun[nIndex].ThreadState != System.Threading.ThreadState.Suspended)
                    {
                        threadsRun[nIndex] = new Thread(new ParameterizedThreadStart(SplitUrl));
                        threadsRun[nIndex].Name = nIndex.ToString();
                        threadsRun[nIndex].Start(t);
                    }
                }

                while (m_IsRunning)
                {
                    Thread.Sleep(50);
                }

                for (int nIndex = 0; nIndex < ThreadCount; nIndex++)
                {
                    threadsRun[nIndex].Join();
                }
            }

            List<cSplitTaskEntity> ts = SaveSplitTask(m_gUrls, t, ref startTaskIndex);
            sTasks.AddRange(ts);

            return sTasks;
        }

        /// <summary>
        /// 分解网址
        /// </summary>
        /// <param name="t1"></param>
        private void SplitUrl(object t1)
        {
            oTask t = (oTask)t1;
            while (m_IsRunning && int.Parse(Thread.CurrentThread.Name) < ThreadCount)
            {
                string ss = string.Empty;

                eWebLink wLink = PeekUri();

                if (wLink != null)
                {
                    if (wLink.NavigRules.Count == 0 && wLink.IsNavigation == false)
                    {
                        #region 没有导航，直接退出了
                        try
                        {
                            //表示没有导航了，则直接退出
                         
                            AddWebLinks(wLink);
                        }
                        catch (System.Exception ex)
                        {
                            OnonErrorSend(this, new onErrorEventArgs(t.TaskEntity.TaskName, wLink.Weblink, ex.Message));
                        }
                        finally
                        {
                            DequeueUri();
                        }
                        #endregion
                    }
                    else
                    {
                        #region 处理导航及翻页的分解
                        try
                        {
                            //先把源码获取出来
                            //cGatherWeb gWeb = new cGatherWeb(this.m_workPath,ref pControl, t.TaskEntity.IsProxy, 
                            //    t.TaskEntity.IsProxyFirst, cGlobalParas.ProxyType.TaskConfig, "", 0);
                            //if (t.TaskEntity.Headers == null)
                            //{
                            //    gWeb.IsCustomHeader = false;
                            //}
                            //else
                            //{
                            //    gWeb.IsCustomHeader = true;
                            //    gWeb.Headers = t.TaskEntity.Headers;
                            //}
                            //string cookie = t.TaskEntity.Cookie;
                            //string UrlSource = gWeb.GetHtml(wLink.Weblink, t.TaskEntity.WebCode, t.TaskEntity.IsUrlEncode,t.TaskEntity.IsTwoUrlCode,
                            //    t.TaskEntity.UrlEncode, ref cookie, "", "", true, 
                            //    t.TaskEntity.IsAutoUpdateHeader,wLink.referUrl,t.TaskEntity.isGatherCoding, t.TaskEntity.GatherCodingFlag,t.TaskEntity.CodeUrl,t.TaskEntity.GatherCodingPlugin);
                            //gWeb = null;

                            string UrlSource = GetHtml(wLink.Weblink, t.TaskEntity, wLink.referUrl);

                            //先翻页，再导航,进入的永远都是第1层，因为导航之后，会逐步删除导航规则
                            if (wLink.NavigRules[0].IsNext)
                            {
                                ss = "翻页";
                                #region 翻页
                                //处理翻页，注意：只有入口地址才是isnext的翻页，其他的翻页都是IsNaviNextPage翻页
                                //每次翻页只翻页一个页面
                                int NextIndex = wLink.NavigRules[0].NextCurrentPage;

                                //翻页操作，肯定返回的是一个网址
                                List<string> tmpUrls = getNextPage(wLink.Weblink, UrlSource, ref pControl, t.TaskEntity.IsProxy, t.TaskEntity.IsProxyFirst, t.TaskEntity.WebCode,
                                    t.TaskEntity.IsUrlEncode,t.TaskEntity.IsTwoUrlCode, t.TaskEntity.UrlEncode, t.TaskEntity.Cookie,
                                    t.TaskEntity.IsAutoUpdateHeader, wLink.NavigRules[0].NextRule, m_RegexNextPage, 
                                    t.TaskEntity.Headers,  ref NextIndex, wLink.NavigRules[0].NextMaxPage,wLink.referUrl,
                                    1,t.TaskEntity.isGatherCoding, t.TaskEntity.GatherCodingFlag,t.TaskEntity.CodeUrl,t.TaskEntity.GatherCodingPlugin);
                                
                                wLink.NavigRules[0].NextCurrentPage += tmpUrls.Count;
                                
                                //判断获取的翻页地址是否已经处理，如果已经处理，则表示翻页陷入了死循环
                                if (tmpUrls != null && tmpUrls.Count > 0 && !hOverUrls.ContainsKey (tmpUrls[0]))
                                {
                                    
                                    

                                    if (wLink.NavigRules[0].NextMaxPage != "0" && wLink.NavigRules[0].NextCurrentPage >= int.Parse(wLink.NavigRules[0].NextMaxPage))
                                    {

                                    }
                                    else
                                    {
                                        if (tmpUrls.Count > 0)
                                        {
                                            eWebLink tmpWlink = CopyWeblink(tmpUrls[0], wLink, 0);

                                            EnqueueUri(tmpWlink);
                                        }
                                    }
                                }

                                #endregion
                            }

                            ss = "导航";

                            //在开始处理导航
                            if (wLink.NavigRules.Count > 0)
                            {
                                #region 导航
                                //拷贝导航规则
                                List<eNavigRule> tmpNRules = new List<eNavigRule>();         //仅记录当前导航级别的规则 是一个集合，因为是分层导航，所以一个集合仅记录一条
                                eNavigRule tmpNRule = new eNavigRule();                      //记录当前导航级别的导航规则

                                tmpNRule.Level = 1;
                                tmpNRule.NaviStartPos = wLink.NavigRules[0].NaviStartPos;
                                tmpNRule.NaviEndPos = wLink.NavigRules[0].NaviEndPos;
                                tmpNRule.NavigRule = wLink.NavigRules[0].NavigRule;

                                tmpNRule.IsNaviNextPage = wLink.NavigRules[0].IsNaviNextPage;
                                tmpNRule.NaviNextPage = wLink.NavigRules[0].NaviNextPage;
                                tmpNRule.NaviNextMaxPage = wLink.NavigRules[0].NaviNextMaxPage;

                                tmpNRule.IsGather = wLink.NavigRules[0].IsGather;
                                tmpNRule.GatherStartPos = wLink.NavigRules[0].GatherStartPos;
                                tmpNRule.GatherEndPos = wLink.NavigRules[0].GatherEndPos;

                                tmpNRule.IsNext = wLink.NavigRules[0].IsNext;
                                tmpNRule.NextRule = wLink.NavigRules[0].NextRule;
                                tmpNRule.NextMaxPage = wLink.NavigRules[0].NextMaxPage;

                                //tmpNRule.IsNextDoPostBack = wLink.NavigRules[0].IsNextDoPostBack;
                                //tmpNRule.IsNaviNextDoPostBack = wLink.NavigRules[0].IsNaviNextDoPostBack;

                                tmpNRules.Add(tmpNRule);


                                List<string> tmpUrls = getNaviUrls(wLink.Weblink, UrlSource, tmpNRules, ref pControl, t.TaskEntity.IsProxy, t.TaskEntity.IsProxyFirst, t.TaskEntity.WebCode, t.TaskEntity.IsUrlEncode,t.TaskEntity.IsTwoUrlCode,
                                    t.TaskEntity.UrlEncode, t.TaskEntity.Cookie, 
                                    t.TaskEntity.Headers,wLink.Weblink, t.TaskEntity.IsAutoUpdateHeader,t.TaskEntity.IsUrlAutoRedirect,
                                    t.TaskEntity.isGatherCoding, t.TaskEntity.GatherCodingFlag,t.TaskEntity.CodeUrl,t.TaskEntity.GatherCodingPlugin);


                                for (int i = 0; i < tmpUrls.Count; i++)
                                {
                                    eWebLink tmpWlink = CopyWeblink(tmpUrls[i], wLink, 1);

                                    if (tmpWlink.NavigRules == null || tmpWlink.NavigRules.Count == 0)
                                    {
                                        //表示导航结束
                                       
                                        //m_gUrls.Add(tmpWlink);
                                        AddWebLinks(tmpWlink);
                                    }
                                    else
                                        EnqueueUri(tmpWlink);
                                }

                                #endregion
                            }


                        }
                        catch (System.Exception ex)
                        {
                            OnonErrorSend(this, new onErrorEventArgs(t.TaskEntity.TaskName, wLink.Weblink, ex.Message));
                        }
                        finally
                        {
                            DequeueUri();
                        }
                        #endregion

                    }
                }
                else
                {

                    ////表示没有取出网址
                    //m_ErrCount++;
                    //if (m_ErrCount > ThreadCount)
                    //    m_IsRunning = false;
                    if (queueURLS.Count == 0 && queueURLSSpliting.Count == 0)
                        m_IsRunning = false;
                    else
                        Thread.Sleep(1);
                }


            }

        }

        private List<string> getNextPage(string NextUrl, string webSource, ref cProxyControl pControl, bool IsProxy, bool IsProxyFirst, cGlobalParas.WebCode webCode,
            bool isUrlCode,bool isTwoUrlCode, cGlobalParas.WebCode urlCode, string cookie, bool isAutoUpdateHeader, string NextRule, string RegexNextPage,
             List<eHeader> cHeader, ref int FirstNextIndex,
            string NextMaxPage, string referUrl, int naviLevel,bool isGatherCodeing, string codingFlag, string CodingUrl, string CodingPlugin)
        {

            List<string> gUrls = new List<string>();

            //int curNextPage = 0;
            string Url = string.Empty;
            string Old_Url = string.Empty;
            string Cookie = cookie;
            cUrlAnalyze cu = new cUrlAnalyze(this.m_workPath ,ref pControl, IsProxy, IsProxyFirst, cGlobalParas.ProxyType.TaskConfig, "", 0);

            Url = NextUrl;
            Old_Url = NextUrl;
            string strNext = string.Empty;


            strNext = cu.GetNextUrl(Url, webSource, NextRule, RegexNextPage, 
                ref FirstNextIndex,  naviLevel);

            referUrl = Url;

            Cookie = cookie;

            NextUrl = strNext;


            if (NextUrl != "" && Old_Url != NextUrl && NextUrl != "#")
            {
                gUrls.Add(NextUrl);
            }
            else
            {

            }

            cu = null;

            return gUrls;

        }

        private List<string> getNaviUrls(string Url, string webSource, List<eNavigRule> tmpNRules, 
            ref cProxyControl pControl, bool IsProxy, bool IsProxyFirst, cGlobalParas.WebCode webCode, bool isUrlCode,bool isTwoUrlCode,
            cGlobalParas.WebCode urlCode, string cookie, List<eHeader> headers, string referUrl, bool isAutoUpdateHeader,
            bool isUrlAutoRedirect, bool isGatherCoding, string codingFlag, string CodingUrl, string CodingPlugin)
        {
            cUrlAnalyze cu = new cUrlAnalyze(this.m_workPath, ref pControl, IsProxy, IsProxyFirst, cGlobalParas.ProxyType.TaskConfig, "", 0);
            string Cookie = cookie;

            //List<string> tmpUrls = cu.ParseUrlRule(Url, tmpNRules, webCode, isUrlCode,
            //        urlCode, ref Cookie, headers, referUrl, isAutoUpdateHeader);

            List<string> tmpUrls = cu.GetUrlsByRule(Url, webSource,  tmpNRules[0].NavigRule,
                0,  tmpNRules[0].RunRule, tmpNRules[0].OtherNaviRule,cookie,headers);

            return tmpUrls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="oldWebLink"></param>
        /// <param name="type">0-全部拷贝；1-删除首层导航网址，但同时要保留翻页的配置，将上一层层翻页isnext移到下一层的IsNaviNextPage；2-不拷贝翻页规则</param>
        /// <returns></returns>
        private eWebLink CopyWeblink(string url, eWebLink oldWebLink, int type)
        {
            eWebLink wLink = new eWebLink();
            wLink.Weblink = url;
            wLink.CurrentRunning = oldWebLink.CurrentRunning;
            wLink.id = oldWebLink.id;
            wLink.IsData121 = oldWebLink.IsData121;
            wLink.IsGathered = oldWebLink.IsGathered;
            wLink.IsMultiGather = oldWebLink.IsMultiGather;
            wLink.IsNavigation = oldWebLink.IsNavigation;
            wLink.IsNextpage = oldWebLink.IsNextpage;
            wLink.MultiPageRules = oldWebLink.MultiPageRules;

            if (type == 0)
                wLink.NavigRules = oldWebLink.NavigRules;
            else if (type == 1)
            {
                wLink.NavigRules = CopyNaviRule(oldWebLink.NavigRules, 1);
                if (wLink.NavigRules.Count == 0)
                    wLink.IsNavigation = false;
            }
            else if (type == 2)
                wLink.NavigRules = CopyNaviRule(oldWebLink.NavigRules, 2);

            wLink.NextMaxPage = oldWebLink.NextMaxPage;
            wLink.NextPageRule = oldWebLink.NextPageRule;
            wLink.NextPageUrl = oldWebLink.NextPageUrl;
            wLink.referUrl = url;
            return wLink;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nRule"></param>
        /// <param name="type">1-去掉最顶层导航；2-不拷贝翻页规则</param>
        /// <returns></returns>
        private List<eNavigRule> CopyNaviRule(List<eNavigRule> nRule, int type)
        {
            List<eNavigRule> newRule = new List<eNavigRule>();
            int startIndex = 0;
            eNavigRule tmpRule = new eNavigRule();  //临时保存首层导航地址
            if (type == 1)
            {
                startIndex = 1;
                tmpRule = nRule[0];
            }


            if (startIndex >= nRule.Count)
            {
                //表示是最低层网址，什么都不拷贝
                return newRule;
            }

            for (int i = startIndex; i < nRule.Count; i++)
            {
                eNavigRule rule = new eNavigRule();

                rule.GatherEndPos = nRule[i].GatherEndPos;
                rule.GatherStartPos = nRule[i].GatherStartPos;
                rule.IsGather = nRule[i].IsGather;


                //rule.IsNaviNextDoPostBack = nRule[i].IsNaviNextDoPostBack;
                rule.IsNaviNextPage = nRule[i].IsNaviNextPage;
                rule.NaviNextMaxPage = nRule[i].NaviNextMaxPage;
                rule.NaviNextPage = nRule[i].NaviNextPage;
                rule.NextCurrentPage = nRule[i].NextCurrentPage;
                rule.NaviNextCurrentPage = nRule[i].NaviNextCurrentPage;

                if (type == 1 && i == startIndex)
                {
                    rule.IsNext = tmpRule.IsNaviNextPage;
                    //rule.IsNextDoPostBack = tmpRule.IsNaviNextDoPostBack;
                    rule.NextMaxPage = tmpRule.NaviNextMaxPage;
                    rule.NextRule = tmpRule.NaviNextPage;
                }
                else if (type == 2)
                {
                    rule.IsNext = false;
                    rule.NextMaxPage = "";
                    rule.NextRule = "";
                }
                else
                {
                    rule.IsNext = nRule[i].IsNext;
                    rule.NextMaxPage = nRule[i].NextMaxPage;
                    rule.NextRule = nRule[i].NextRule;
                }

                rule.Level = nRule[i].Level;
                rule.NaviStartPos = nRule[i].NaviStartPos;
                rule.NaviEndPos = nRule[i].NaviEndPos;
                rule.NavigRule = nRule[i].NavigRule;

                rule.Url = nRule[i].Url;
                newRule.Add(rule);
            }

            return newRule;
        }

        eWebLink PeekUri()
        {
            eWebLink uri = null;

            try
            {
                Monitor.Enter(queueURLS);
                uri = (eWebLink)queueURLS.Dequeue();

            }
            catch (Exception)
            {
            }
            finally
            {
                Monitor.Exit(queueURLS);
            }

            if (uri != null)
            {


                try
                {
                    Monitor.Enter(queueURLSSpliting);
                    queueURLSSpliting.Enqueue(uri.Weblink);
                }
                catch (Exception)
                {
                }
                finally
                {
                    Monitor.Exit(queueURLSSpliting);
                }
            }
            return uri;
        }

        bool DequeueUri()
        {

            try
            {
                Monitor.Enter(queueURLSSpliting);
                string url =(string)queueURLSSpliting.Dequeue();
                try
                {
                    hOverUrls.Add(url, url);
                }
                catch { }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                Monitor.Exit(queueURLSSpliting);
            }
            return true;
        }

        bool EnqueueUri(eWebLink wLink)
        {
            try
            {
                Monitor.Enter(queueURLS);
                queueURLS.Enqueue(wLink);
            }
            catch (Exception)
            {
            }
            finally
            {
                Monitor.Exit(queueURLS);
            }
            return true;
        }

        bool AddWebLinks(eWebLink wLink)
        {
            try
            {
                Monitor.Enter(m_gUrls);
                m_gUrls.Add(wLink);
            }
            catch (System.Exception)
            {
                return false;
            }
            finally
            {
                Monitor.Exit(m_gUrls);
            }
            return true;
        }

        /// <summary>
        /// 分解采集任务
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="sState"></param>
        /// <returns></returns>
        public List<cSplitTaskEntity> SplitTask(string TaskName, out cGlobalParas.SplitTaskState sState)
        {


            oTask t = new oTask(m_workPath);
            t.LoadTask(m_TaskPath + TaskName + ".smt");

            bool isS = isSplit(t);

            if (isS == true)
            {
                List<cSplitTaskEntity> ts = SplitTask(t);

                sState = cGlobalParas.SplitTaskState.Splited;
                return ts;
            }
            else
            {
                sState = cGlobalParas.SplitTaskState.WithoutSplit;
                return null;
            }

        }

        /// <summary>
        /// 保存分解后的采集任务，注意：只保存最后一层网址，所以如果是导航采集则保存会出错
        /// </summary>
        /// <param name="gUrls"></param>s
        /// <param name="webLink"></param>
        /// <param name="t"></param>
        /// <param name="startTaskIndex"></param>
        /// <returns></returns>
        private List<cSplitTaskEntity> SaveSplitTask(List<eWebLink> gUrls, oTask t, ref int startTaskIndex)
        {
            List<cSplitTaskEntity> ts = new List<cSplitTaskEntity>();

            int tCount = gUrls.Count / this.m_MaxSplitMaxUrlsCount;
            int tRem = gUrls.Count % this.m_MaxSplitMaxUrlsCount;

            string TaskName = t.TaskEntity.TaskName;

            if (tRem > 0)
                tCount++;

            int startIndex = 0;

            for (int j = 0; j < tCount; j++)
            {
                try
                {
                    //保存分解后的采集任务
                    oTask ct = new oTask(m_workPath);
                    ct.LoadTask(m_TaskPath + TaskName + ".smt");

                    //清空网址
                    ct.TaskEntity.WebpageLink.Clear();
                    List<eWebLink> wLinks = new List<eWebLink>();

                    for (int m = 0; m < m_MaxSplitMaxUrlsCount; m++)
                    {
                        if (startIndex < gUrls.Count)
                        {
                            
                                eWebLink wLink = new eWebLink();
                                //wLink.Weblink = gUrls[startIndex];
                                wLink = gUrls[startIndex];
                                startIndex++;
                                if (wLink != null)
                                    wLinks.Add(wLink);
                                else
                                    throw new Exception("有网址分解后为空！"); ;
                                   
                           
                        }
                        else
                            break;

                    }

                    
                        if (!Directory.Exists(m_TaskPath + TaskName))
                            Directory.CreateDirectory(m_TaskPath + TaskName);
                        ct.TaskEntity.WebpageLink.AddRange(wLinks);
                        string newTaskName = TaskName + "___" + startTaskIndex.ToString("00");
                        ct.TaskEntity.TaskName = newTaskName;
                        ct.TaskEntity.UrlCount = ct.TaskEntity.WebpageLink.Count;
                        ct.SaveTask(m_TaskPath + TaskName + "\\" + newTaskName + ".smt");
                        ct = null;

                        //插入分解任务信息
                        cSplitTaskEntity st = new cSplitTaskEntity();
                        st.TaskName = newTaskName;
                        st.StartDate = "";
                        st.EndDate = "";
                        st.ClientID = "";
                        st.tState = cGlobalParas.TaskState.UnStart;
                        st.sPath = m_TaskPath + TaskName;
                        ts.Add(st);

                    startTaskIndex++;
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }

            return ts;

        }

        /// <summary>
        /// 判断是否分解采集任务
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isSplit(oTask t)
        {
            if (t.TaskEntity.IsNoneAllowSplit == true)
            {
                return false;
            }

            if (t.TaskEntity.IsSplitDbUrls == true)
            {
                return true;
            }

            //先判断入口网址是多少
            if (t.TaskEntity.WebpageLink.Count != t.TaskEntity.UrlCount)
            {
                //表示有参数
                if (t.TaskEntity.UrlCount > this.m_MaxSplitMaxUrlsCount)
                {
                    return true;
                }
            }
            else
            {
                if (t.TaskEntity.UrlCount > this.m_MaxSplitMaxUrlsCount)
                {
                    return true;
                }
            }

            if (t.TaskEntity.WebpageLink[0].NavigRules.Count > m_MaxSplitLevel)
            {
                //是否配置了导航采集，如果配置，则不进行分解
                bool isNaviGather = false;
                for (int i = 0; i < t.TaskEntity.WebpageCutFlag.Count; i++)
                {
                    if (t.TaskEntity.WebpageCutFlag[i].NavLevel > 0)
                    {
                        isNaviGather = true;
                        break;
                    }
                }
                if (isNaviGather == true)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        private string GetHtml(string Url,eTask t, string referUrl)
        {
            //在此控制代理的问题
            eRequest request = NetMiner.Core.Url.UrlPack.GetRequest(Url, t.Cookie, t.WebCode, t.IsUrlEncode,
                                    t.IsTwoUrlCode, t.UrlEncode, "",
                                    t.Headers, referUrl, t.IsUrlAutoRedirect);

            WebProxy wProxy = null;
            //计算此次请求是否需要代理
            if (t.IsProxy)
            {
                //需要代理，再判断是否为线程指定的代理
                
                    //通过proxycontrol获取代理
                    if (t.IsProxyFirst)
                    {
                        eProxy proxy = pControl.GetFirstProxy();
                        wProxy = new WebProxy(proxy.ProxyServer, int.Parse(proxy.ProxyPort));
                        wProxy.BypassProxyOnLocal = true;
                        if (proxy.User != "")
                        {
                            wProxy.Credentials = new NetworkCredential(proxy.User, proxy.Password);
                        }
                    }
                    else
                    {
                        eProxy proxy = pControl.GetProxy();
                        wProxy = new WebProxy(proxy.ProxyServer, int.Parse(proxy.ProxyPort));
                        wProxy.BypassProxyOnLocal = true;
                        if (proxy.User != "")
                        {
                            wProxy.Credentials = new NetworkCredential(proxy.User, proxy.Password);
                        }
                    }

                    request.ProxyType = cGlobalParas.ProxyType.HttpProxy;
                    request.webProxy = wProxy;
                

            }
            else
            {
                request.ProxyType = cGlobalParas.ProxyType.SystemProxy;
            }

            eResponse response = NetMiner.Net.Unity.RequestUri(this.m_workPath, request, false);
            //更新cookie
            //this.m_tmpCookie = ToolUtil.MergerCookie(this.m_tmpCookie, response.cookie);

            string htmlSource = response.Body;

            //去除\r\n
            htmlSource = Regex.Replace(htmlSource, "([\\r\\n])[\\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            htmlSource = Regex.Replace(htmlSource, "\\n", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            htmlSource.Replace("\\r\\n", "");

            return htmlSource;
        }
    }


    //定义一个消息的事件，用于传递需要更新的Url地址信息
    public class onErrorEventArgs : EventArgs
    {
        public onErrorEventArgs(string tName, string url,string errMessage)
        {
            m_TaskName = tName;
            m_url = url;
            m_errMess = errMessage;
        }

        private string m_TaskName;
        public string TaskName
        {
            get { return m_TaskName; }
            set { m_TaskName = value; }
        }

        private string m_url;
        public string Url
        {
            get { return m_url; }
            set { m_url = value; }
        }

        private string m_errMess;
        public string errMess
        {
            get { return m_errMess; }
            set { m_errMess = value; }
        }

    }



    //public class cSplitPlot
    //{
    //    private int m_MaxSplitMaxUrlsCount = 200;
    //    private string m_TaskPath = string.Empty;
    //    private string m_Conn = string.Empty;
    //    private int m_MaxSplitLevel;
    //    public string m_workPath = string.Empty;

    //    private Queue queueURLS;
    //    private Queue queueURLSSpliting;

    //    private List<cWebLink> m_gUrls;

    //    private Thread[] threadsRun;
    //    private string m_RegexNextPage;
    //    private cProxyControl pControl = null;
    //    private const int ThreadCount = 10;
    //    private const int MaxErrCount = 10;

    //    private bool m_IsRunning = false;

    //    public cSplitPlot(string workPath, string con, int MaxSplitUrls, int MaxSplitLevel, string taskPath)
    //    {
    //        m_workPath = workPath;

    //        m_Conn = con;
    //        this.m_MaxSplitMaxUrlsCount = MaxSplitUrls;
    //        this.m_MaxSplitLevel = MaxSplitLevel;
    //        this.m_TaskPath = taskPath;

    //        m_gUrls = new List<cWebLink>();
    //        queueURLS = new Queue();
    //        queueURLSSpliting = new Queue();

    //        cXmlSConfig cCon = new cXmlSConfig(this.m_workPath);
    //        m_RegexNextPage = cTool.ShowTrans(cCon.RegexNextPage);
    //        cCon = null;

    //        pControl = new cProxyControl(this.m_workPath);
    //    }

    //    ~cSplitPlot()
    //    {
    //    }

    //    //启动线程
    //    private List<cSplitTask> SplitTask(oTask t)
    //    {
    //        int startTaskIndex = 1;
    //        List<cSplitTask> sTasks = new List<cSplitTask>();

    //        //采用10个线程进行网址分解
    //        threadsRun = new Thread[ThreadCount];

    //        cUrlAnalyze cu = new cUrlAnalyze(ref pControl, t.IsProxy, t.IsProxyFirst);

    //        //先将地址压入待分解队列
    //        for (int i = 0; i < t.WebpageLink.Count; i++)
    //        {
    //            string url = t.WebpageLink[i].Weblink;

    //            //只有入口地址才可能存在base64的编码
    //            if (Regex.IsMatch(url, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
    //            {

    //                Match s = Regex.Match(url, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
    //                string sBase64 = s.Groups[0].Value.ToString();
    //                sBase64 = cTool.Base64Encoding(sBase64);

    //                //将base64编码部分进行url替换
    //                url = Regex.Replace(url, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

    //            }

    //            //如果网址中有参数，现将其分解
    //            List<string> tmpUrl = cu.SplitWebUrl(url);

    //            for (int j = 0; j < tmpUrl.Count; j++)
    //            {

    //                cWebLink wLink = CopyWeblink(tmpUrl[j], t.WebpageLink[i], 0);

    //                //cSplitUrl su = new cSplitUrl();
    //                //su.link = wLink;
    //                //su.sState = cGlobalParas.SplitTaskState.UnSplit;

    //                EnqueueUri(wLink);
    //            }

    //        }

    //        m_IsRunning = true;
    //        //按照规定的线程数进行线程创建
    //        for (int nIndex = 0; nIndex < ThreadCount; nIndex++)
    //        {
    //            if (threadsRun[nIndex] == null || threadsRun[nIndex].ThreadState != System.Threading.ThreadState.Suspended)
    //            {
    //                threadsRun[nIndex] = new Thread(new ParameterizedThreadStart(SplitUrl));
    //                threadsRun[nIndex].Name = nIndex.ToString();
    //                threadsRun[nIndex].Start(t);
    //            }
    //        }

    //        //调用开始后，开始阻塞当前的线程
    //        //bool isStop = false;
    //        //while (!isStop)
    //        //{
    //        //    bool isS = true;
    //        //    for (int nIndex = 0; nIndex < ThreadCount; nIndex++)
    //        //    {
    //        //        if (threadsRun[nIndex].ThreadState != System.Threading.ThreadState.Stopped)
    //        //        {
    //        //            isS = false;
    //        //            break;
    //        //        }
    //        //    }
    //        //    if (isS == true)
    //        //        isStop = true;
    //        //}
    //        while (m_IsRunning)
    //        {
    //            Thread.Sleep(50);
    //        }

    //        for (int nIndex = 0; nIndex < ThreadCount; nIndex++)
    //        {
    //            threadsRun[nIndex].Join();
    //        }

    //        List<cSplitTask> ts = SaveSplitTask(m_gUrls, t, ref startTaskIndex);
    //        sTasks.AddRange(ts);

    //        return sTasks;
    //    }

    //    private void SplitUrl(object t1)
    //    {
    //        oTask t = (oTask)t1;
    //        while (m_IsRunning && int.Parse(Thread.CurrentThread.Name) < ThreadCount)
    //        {
    //            //Stopwatch sw = new Stopwatch();
    //            //Stopwatch sw1 = new Stopwatch();
    //            //Stopwatch sw2 = new Stopwatch();
    //            //sw.Start();

    //            cWebLink wLink = PeekUri();

    //            if (wLink != null)
    //            {
    //                if (wLink.NavigRules.Count == 0)
    //                {
    //                    //表示没有导航了，则直接退出
    //                    return;
    //                }

    //                //先翻页，再导航,进入的永远都是第1层，因为导航之后，会逐步删除导航规则
    //                if (wLink.NavigRules[0].IsNext)
    //                {

    //                    //sw1.Start();
    //                    #region 翻页
    //                    try
    //                    {
    //                        //处理翻页，注意：只有入口地址才是isnext的翻页，其他的翻页都是IsNaviNextPage翻页
    //                        List<string> tmpUrls = getNextPage(wLink.Weblink, ref pControl, t.IsProxy, t.IsProxyFirst, (cGlobalParas.WebCode)int.Parse(t.WebCode), t.IsUrlEncode, (cGlobalParas.WebCode)int.Parse(t.UrlEncode), t.Cookie,
    //                            t.IsAutoUpdateHeader, wLink.NavigRules[0].NextRule, m_RegexNextPage, wLink.NavigRules[0].IsNextDoPostBack,
    //                            t.Headers, t.IsCustomHeader, wLink.NavigRules[0].NextMaxPage);


    //                        for (int i = 0; i < tmpUrls.Count; i++)
    //                        {
    //                            cWebLink tmpWlink = CopyWeblink(tmpUrls[i], wLink, 2);

    //                            EnqueueUri(tmpWlink);
    //                        }

    //                        //sw1.Stop();
    //                        //删除此地址信息，表示翻页已经结束
    //                        DequeueUri();
    //                    }
    //                    catch (System.Exception ex)
    //                    {
 
    //                    }

    //                    #endregion
    //                }
    //                else
    //                {


    //                    //在开始处理导航
    //                    if (wLink.NavigRules.Count > 0)
    //                    {
                            
    //                        //sw2.Start();
    //                        #region 导航

    //                        try
    //                        {

    //                            //拷贝导航规则
    //                            List<cNavigRule> tmpNRules = new List<cNavigRule>();         //仅记录当前导航级别的规则 是一个集合，因为是分层导航，所以一个集合仅记录一条
    //                            cNavigRule tmpNRule = new cNavigRule();                      //记录当前导航级别的导航规则

    //                            tmpNRule.Level = 1;
    //                            tmpNRule.NaviStartPos = wLink.NavigRules[0].NaviStartPos;
    //                            tmpNRule.NaviEndPos = wLink.NavigRules[0].NaviEndPos;
    //                            tmpNRule.NavigRule = wLink.NavigRules[0].NavigRule;

    //                            tmpNRule.IsNaviNextPage = wLink.NavigRules[0].IsNaviNextPage;
    //                            tmpNRule.NaviNextPage = wLink.NavigRules[0].NaviNextPage;
    //                            tmpNRule.NaviNextMaxPage = wLink.NavigRules[0].NaviNextMaxPage;

    //                            tmpNRule.IsGather = wLink.NavigRules[0].IsGather;
    //                            tmpNRule.GatherStartPos = wLink.NavigRules[0].GatherStartPos;
    //                            tmpNRule.GatherEndPos = wLink.NavigRules[0].GatherEndPos;

    //                            tmpNRule.IsNext = wLink.NavigRules[0].IsNext;
    //                            tmpNRule.NextRule = wLink.NavigRules[0].NextRule;
    //                            tmpNRule.NextMaxPage = wLink.NavigRules[0].NextMaxPage;

    //                            tmpNRule.IsNextDoPostBack = wLink.NavigRules[0].IsNextDoPostBack;
    //                            tmpNRule.IsNaviNextDoPostBack = wLink.NavigRules[0].IsNaviNextDoPostBack;

    //                            tmpNRules.Add(tmpNRule);


    //                            List<string> tmpUrls = getNaviUrls(wLink.Weblink, tmpNRules, ref pControl, t.IsProxy, t.IsProxyFirst, (cGlobalParas.WebCode)int.Parse(t.WebCode), t.IsUrlEncode,
    //                                (cGlobalParas.WebCode)int.Parse(t.UrlEncode), t.Cookie, t.Headers, wLink.Weblink, t.IsAutoUpdateHeader);


    //                            for (int i = 0; i < tmpUrls.Count; i++)
    //                            {
    //                                cWebLink tmpWlink = CopyWeblink(tmpUrls[i], wLink, 1);

    //                                if (tmpWlink.NavigRules == null || tmpWlink.NavigRules.Count == 0)
    //                                {
    //                                    //表示导航结束
    //                                    m_gUrls.Add(tmpWlink);
    //                                }
    //                                else
    //                                    EnqueueUri(tmpWlink);
    //                            }

    //                            //删除此地址信息，表示翻页已经结束
    //                            DequeueUri();
    //                        }
    //                        catch(System.Exception ex)
    //                        {

    //                        }
    //                        //sw2.Stop();

    //                        #endregion
    //                    }
    //                    else
    //                    {
    //                        //即没有翻页，也没有导航，则直接压入待采队列
    //                        m_gUrls.Add(wLink);
    //                    }
    //                }

    //                //sw.Stop();
    //                //Console.WriteLine(Thread.CurrentThread.Name + ":" + sw.ElapsedMilliseconds.ToString() 
    //                //    + "  " + queueURLS.Count + "/" + queueURLSSpliting.Count + "/" + m_gUrls.Count + "\r\n" + wLink.Weblink
    //                //    + "   " + sw1.ElapsedMilliseconds.ToString () + "?" + sw2.ElapsedMilliseconds.ToString () );
    //            }
    //            else
    //            {

    //                ////表示没有取出网址
    //                //m_ErrCount++;
    //                //if (m_ErrCount > ThreadCount)
    //                //    m_IsRunning = false;
    //                if (queueURLS.Count == 0 && queueURLSSpliting.Count == 0)
    //                    m_IsRunning = false;
    //                else
    //                    Thread.Sleep(1);
    //            }


    //        }

    //    }

    //    private List<string> getNextPage(string NextUrl, ref cProxyControl pControl, bool IsProxy, bool IsProxyFirst, cGlobalParas.WebCode webCode,
    //        bool isUrlCode, cGlobalParas.WebCode urlCode, string cookie, bool isAutoUpdateHeader, string NextRule, string RegexNextPage,
    //        bool IsNextDoPostBack, List<cHeader> cHeader, bool isCustomHeader, string NextMaxPage)
    //    {

    //        int FirstNextIndex = 0;
    //        List<string> gUrls = new List<string>();

    //        int curNextPage = 0;
    //        string Url = string.Empty;
    //        string Old_Url = string.Empty;
    //        string Cookie = cookie;
    //        cUrlAnalyze cu = new cUrlAnalyze(ref pControl, IsProxy, IsProxyFirst);

    //        do
    //        {

    //            Url = NextUrl;
    //            Old_Url = NextUrl;

    //            gUrls.Add(Url);

    //            string strNext = cu.GetNextUrl(Url, "", NextRule, RegexNextPage, webCode,
    //                isUrlCode, urlCode, ref cookie, IsNextDoPostBack, ref FirstNextIndex, cHeader);
    //            Cookie = cookie;

    //            NextUrl = strNext;


    //            //翻页成功
    //            curNextPage++;
    //            if (NextMaxPage != "0" && curNextPage >= int.Parse(NextMaxPage))
    //                break;

    //        }
    //        while (NextUrl != "" && Old_Url != NextUrl && NextUrl != "#");
    //        cu = null;

    //        return gUrls;

    //    }

    //    private List<string> getNaviUrls(string Url, List<cNavigRule> tmpNRules, ref cProxyControl pControl, bool IsProxy, bool IsProxyFirst, cGlobalParas.WebCode webCode, bool isUrlCode,
    //        cGlobalParas.WebCode urlCode, string cookie, List<cHeader> headers, string referUrl, bool isAutoUpdateHeader)
    //    {
    //        cUrlAnalyze cu = new cUrlAnalyze(ref pControl, IsProxy, IsProxyFirst);
    //        string Cookie = cookie;

    //        List<string> tmpUrls = cu.ParseUrlRule(Url, tmpNRules, webCode, isUrlCode,
    //                urlCode, ref Cookie, headers, referUrl, isAutoUpdateHeader);

    //        return tmpUrls;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="url"></param>
    //    /// <param name="oldWebLink"></param>
    //    /// <param name="type">0-全部拷贝；1-删除首层导航网址，但同时要保留翻页的配置，将上一层层翻页isnext移到下一层的IsNaviNextPage；2-不拷贝翻页规则</param>
    //    /// <returns></returns>
    //    private cWebLink CopyWeblink(string url, cWebLink oldWebLink, int type)
    //    {
    //        cWebLink wLink = new cWebLink();
    //        wLink.Weblink = url;
    //        wLink.CurrentRunning = oldWebLink.CurrentRunning;
    //        wLink.id = oldWebLink.id;
    //        wLink.IsData121 = oldWebLink.IsData121;
    //        wLink.IsDoPostBack = oldWebLink.IsDoPostBack;
    //        wLink.IsGathered = oldWebLink.IsGathered;
    //        wLink.IsMultiGather = oldWebLink.IsMultiGather;
    //        wLink.IsNavigation = oldWebLink.IsNavigation;
    //        wLink.IsNextpage = oldWebLink.IsNextpage;
    //        wLink.MultiPageRules = oldWebLink.MultiPageRules;

    //        if (type == 0)
    //            wLink.NavigRules = oldWebLink.NavigRules;
    //        else if (type == 1)
    //            wLink.NavigRules = CopyNaviRule(oldWebLink.NavigRules, 1);
    //        else if (type == 2)
    //            wLink.NavigRules = CopyNaviRule(oldWebLink.NavigRules, 2);

    //        wLink.NextMaxPage = oldWebLink.NextMaxPage;
    //        wLink.NextPageRule = oldWebLink.NextPageRule;
    //        wLink.NextPageUrl = oldWebLink.NextPageUrl;

    //        return wLink;

    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="nRule"></param>
    //    /// <param name="type">1-去掉最顶层导航；2-不拷贝翻页规则</param>
    //    /// <returns></returns>
    //    private List<cNavigRule> CopyNaviRule(List<cNavigRule> nRule, int type)
    //    {
    //        List<cNavigRule> newRule = new List<cNavigRule>();
    //        int startIndex = 0;
    //        cNavigRule tmpRule = new cNavigRule();  //临时保存首层导航地址
    //        if (type == 1)
    //        {
    //            startIndex = 1;
    //            tmpRule = nRule[0];
    //        }


    //        if (startIndex >= nRule.Count)
    //        {
    //            //表示是最低层网址，什么都不拷贝
    //            return newRule;
    //        }

    //        for (int i = startIndex; i < nRule.Count; i++)
    //        {
    //            cNavigRule rule = new cNavigRule();

    //            rule.GatherEndPos = nRule[i].GatherEndPos;
    //            rule.GatherStartPos = nRule[i].GatherStartPos;
    //            rule.IsGather = nRule[i].IsGather;


    //            rule.IsNaviNextDoPostBack = nRule[i].IsNaviNextDoPostBack;
    //            rule.IsNaviNextPage = nRule[i].IsNaviNextPage;
    //            rule.NaviNextMaxPage = nRule[i].NaviNextMaxPage;
    //            rule.NaviNextPage = nRule[i].NaviNextPage;

    //            if (type == 1 && i == startIndex)
    //            {
    //                rule.IsNext = tmpRule.IsNaviNextPage;
    //                rule.IsNextDoPostBack = tmpRule.IsNaviNextDoPostBack;
    //                rule.NextMaxPage = tmpRule.NaviNextMaxPage;
    //                rule.NextRule = tmpRule.NaviNextPage;
    //            }
    //            else if (type == 2)
    //            {
    //                rule.IsNext = false;
    //                rule.IsNextDoPostBack = false;
    //                rule.NextMaxPage = "";
    //                rule.NextRule = "";
    //            }
    //            else
    //            {
    //                rule.IsNext = nRule[i].IsNext;
    //                rule.IsNextDoPostBack = nRule[i].IsNextDoPostBack;
    //                rule.NextMaxPage = nRule[i].NextMaxPage;
    //                rule.NextRule = nRule[i].NextRule;
    //            }

    //            rule.Level = nRule[i].Level;
    //            rule.NaviStartPos = nRule[i].NaviStartPos;
    //            rule.NaviEndPos = nRule[i].NaviEndPos;
    //            rule.NavigRule = nRule[i].NavigRule;

    //            rule.Url = nRule[i].Url;
    //            newRule.Add(rule);
    //        }

    //        return newRule;
    //    }

    //    cWebLink PeekUri()
    //    {
    //        cWebLink uri = null;
    //        lock (((ICollection)queueURLS).SyncRoot)
    //        {
    //            try
    //            {
    //                uri = (cWebLink)queueURLS.Dequeue();
    //                queueURLSSpliting.Enqueue(uri.Weblink);
    //            }
    //            catch (Exception)
    //            {
    //            }

    //        }

    //        return uri;
    //    }

    //    bool DequeueUri()
    //    {
    //        lock (((ICollection)queueURLSSpliting).SyncRoot)
    //        {

    //            try
    //            {
    //                queueURLSSpliting.Dequeue();
    //            }
    //            catch (Exception)
    //            {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }

    //    bool EnqueueUri(cWebLink wLink)
    //    {
    //        lock (((ICollection)queueURLS).SyncRoot)
    //        {
    //            try
    //            {

    //                queueURLS.Enqueue(wLink);
    //            }
    //            catch (Exception)
    //            {
    //            }
    //        }
    //        return true;
    //    }

    //    /// <summary>
    //    /// 分解采集任务
    //    /// </summary>
    //    /// <param name="TaskName"></param>
    //    /// <param name="sState"></param>
    //    /// <returns></returns>
    //    public List<cSplitTask> SplitTask(string TaskName, out cGlobalParas.SplitTaskState sState)
    //    {


    //        oTask t = new oTask(m_workPath);
    //        t.LoadTask(m_TaskPath + TaskName + ".smt");

    //        bool isS = isSplit(t);

    //        if (isS == true)
    //        {
    //            List<cSplitTask> ts = SplitTask(t);

    //            sState = cGlobalParas.SplitTaskState.Splited;
    //            return ts;
    //        }
    //        else
    //        {
    //            sState = cGlobalParas.SplitTaskState.WithoutSplit;
    //            return null;
    //        }

    //    }

    //    /// <summary>
    //    /// 保存分解后的采集任务，注意：只保存最后一层网址，所以如果是导航采集则保存会出错
    //    /// </summary>
    //    /// <param name="gUrls"></param>s
    //    /// <param name="webLink"></param>
    //    /// <param name="t"></param>
    //    /// <param name="startTaskIndex"></param>
    //    /// <returns></returns>
    //    private List<cSplitTask> SaveSplitTask(List<cWebLink> gUrls, oTask t, ref int startTaskIndex)
    //    {
    //        List<cSplitTask> ts = new List<cSplitTask>();

    //        int tCount = gUrls.Count / this.m_MaxSplitMaxUrlsCount;
    //        int tRem = gUrls.Count % this.m_MaxSplitMaxUrlsCount;

    //        string TaskName = t.TaskName;

    //        if (tRem > 0)
    //            tCount++;

    //        int startIndex = 0;

    //        for (int j = 0; j < tCount; j++)
    //        {
    //            //保存分解后的采集任务
    //            oTask ct = new oTask(m_workPath);
    //            ct.LoadTask(m_TaskPath + TaskName + ".smt");

    //            //清空网址
    //            ct.WebpageLink.Clear();
    //            List<cWebLink> wLinks = new List<cWebLink>();

    //            for (int m = 0; m < m_MaxSplitMaxUrlsCount; m++)
    //            {
    //                if (startIndex < gUrls.Count)
    //                {
    //                    cWebLink wLink = new cWebLink();
    //                    //wLink.Weblink = gUrls[startIndex];
    //                    wLink = gUrls[startIndex];
    //                    startIndex++;
    //                    wLinks.Add(wLink);
    //                }

    //            }

    //            if (!Directory.Exists(m_TaskPath + TaskName))
    //                Directory.CreateDirectory(m_TaskPath + TaskName);
    //            ct.WebpageLink.AddRange(wLinks);
    //            string newTaskName = TaskName + "___" + startTaskIndex.ToString("00");
    //            ct.TaskName = newTaskName;
    //            ct.UrlCount = ct.WebpageLink.Count;
    //            ct.SaveTask(m_TaskPath + TaskName + "\\" + newTaskName + ".smt");
    //            ct = null;

    //            //插入分解任务信息
    //            cSplitTask st = new cSplitTask();
    //            st.TaskName = newTaskName;
    //            st.StartDate = "";
    //            st.EndDate = "";
    //            st.ClientID = "";
    //            st.tState = cGlobalParas.TaskState.UnStart;
    //            st.sPath = m_TaskPath + TaskName;

    //            ts.Add(st);

    //            startTaskIndex++;
    //        }

    //        return ts;

    //    }

    //    /// <summary>
    //    /// 判断是否分解采集任务
    //    /// </summary>
    //    /// <param name="t"></param>
    //    /// <returns></returns>
    //    private bool isSplit(oTask t)
    //    {

    //        //先判断入口网址是多少
    //        if (t.WebpageLink.Count != t.UrlCount)
    //        {
    //            //表示有参数
    //            if (t.UrlCount > this.m_MaxSplitMaxUrlsCount)
    //            {
    //                return true;
    //            }
    //        }

    //        if (t.WebpageLink[0].NavigRules.Count > m_MaxSplitLevel)
    //        {
    //            //是否配置了导航采集，如果配置，则不进行分解
    //            bool isNaviGather = false;
    //            for (int i = 0; i < t.WebpageCutFlag.Count; i++)
    //            {
    //                if (t.WebpageCutFlag[i].NavLevel > 0)
    //                {
    //                    isNaviGather = true;
    //                    break;
    //                }
    //            }
    //            if (isNaviGather == true)
    //                return false;
    //            else
    //                return true;
    //        }
    //        else
    //            return false;
    //    }
    //}



  
}

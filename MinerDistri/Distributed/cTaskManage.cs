using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Gather;
using NetMiner.Gather.Task;
using NetMiner.Gather.Control;
using NetMiner.Core.Proxy;
using NetMiner.Resource;
using System.IO;
using System.Data;
using NetMiner.Data;
using NetMiner.Common;
using System.Text.RegularExpressions;
using System.Threading;
using NetMiner.Core;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;
using NetMiner.Gather.Url;
using NetMiner.Core.pTask.Entity;
using NetMiner.Net;
using NetMiner.Net.Common;

///分布式采集的采集任务分解类，支持多种采集任务分解策略
///1、根据当前网络环境进行分解
///2、根据当前的采集任务执行时长进行分解
namespace MinerDistri.Distributed
{
    public class cTaskManage
    {
        //private int m_MaxDSpeed;
        //private int m_MaxGCount;
        private int m_MaxSplitMaxUrlsCount = 200;
        //private string m_TaskPath = cTool.getPrjPath() + "tasks\\";
        private string m_TaskPath = string.Empty;
        private string m_Conn=string.Empty ;
        private cGlobalParas.DatabaseType m_dbType;
        private int m_MaxSplitLevel;

        public string m_workPath = string.Empty;

        //定义一个线程池进行采集网址的分解

        public cTaskManage(string workPath, cGlobalParas.DatabaseType dbType, string con, int MaxSplitUrls, int MaxSplitLevel, string taskPath)
        {
            m_workPath = workPath;

            m_Conn = con;
            m_dbType = dbType;

            this.m_MaxSplitMaxUrlsCount = MaxSplitUrls;
            this.m_MaxSplitLevel = MaxSplitLevel;
            this.m_TaskPath = taskPath;

            
        }


        ~cTaskManage()
        {
        }

        /// <summary>
        /// 分解采集任务，有几种情况是不允许分解的：1、导航采集；2：分页不进行拆分处理
        /// </summary>
        public List<cSplitTaskEntity> SplitTask(string TaskName,out cGlobalParas.SplitTaskState sState)
        {


            oTask t = new oTask(m_workPath);
            t.LoadTask(m_TaskPath + TaskName + ".smt");

            bool isS=isSplit(t);

            if (isS == true)
            {
                List<cSplitTaskEntity> ts = SplitTask(t, TaskName);
                
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
        /// 分解采集网址，分解采集网址需要分解到最终层，即采集页，不能保留中间层，否则会出错
        /// </summary>
        /// <param name="t"></param>
        /// <param name="TaskName"></param>
        /// <returns></returns>
        private List<cSplitTaskEntity> SplitTask(oTask t, string TaskName)
        {
            //先分解采集网址
            List<string> gUrls = new List<string>();  //存储解析后的地址信息
            List<cSplitTaskEntity> sTasks = new List<cSplitTaskEntity>();
            int startTaskIndex = 1;

            cXmlSConfig cCon = new cXmlSConfig(this.m_workPath);
            string RegexNextPage = ToolUtil.ShowTrans(cCon.RegexNextPage);
            cCon = null;

            cProxyControl PControl = new cProxyControl(this.m_workPath, 0);

            string cookie = t.TaskEntity.Cookie;
            for (int i = 0; i < t.TaskEntity.WebpageLink.Count; i++)
            {
                List<string> tmpUrls = new List<string>();
                if (t.TaskEntity.WebpageLink[i].NavigRules.Count == 0)
                {
                    //不存在导航，直接分解网址，有可能带有网址参数
                    NetMiner.Core.Url.cUrlParse u = new NetMiner.Core.Url.cUrlParse(this.m_workPath);
                    tmpUrls = u.SplitWebUrl(t.TaskEntity.WebpageLink[i].Weblink);
                    u = null;
                }
                else
                {
                    //存在导航
                    tmpUrls = GatherNavigationUrl(t.TaskEntity.WebpageLink[i].Weblink, t.TaskEntity.WebpageLink[i].NavigRules,
                                           i, t.TaskEntity.IsProxy, t.TaskEntity.IsProxyFirst, t.TaskEntity.WebpageLink[i].Weblink, ref PControl,  t.TaskEntity.Headers, t.TaskEntity.IsUrlEncode,t.TaskEntity.IsTwoUrlCode,
                                           t.TaskEntity.WebCode,t.TaskEntity.UrlEncode, t.TaskEntity.IsAutoUpdateHeader,
                                           ref cookie, RegexNextPage, t.TaskEntity.IsUrlAutoRedirect,t.TaskEntity.isGatherCoding, t.TaskEntity.GatherCodingFlag,t.TaskEntity.CodeUrl,
                                           t.TaskEntity.GatherCodingPlugin);
                }

                gUrls.AddRange(tmpUrls);
            }


            List<cSplitTaskEntity> ts = SaveSplitTask(gUrls, t, ref startTaskIndex);
            sTasks.AddRange(ts);
             

            return sTasks;

        }

        #region 根据规则解析采集任务的网址
        private List<string> GatherNavigationUrl(string Url, List<eNavigRule> nRules, int index,
        bool IsProxy, bool IsProxyFirst, string referUrl, ref cProxyControl ProxyControl, List<eHeader> headers,
            bool isUrlCode,bool isTwoUrlCode, cGlobalParas.WebCode webCode,cGlobalParas.WebCode urlCode,bool isAutoUpdateHeader,ref string Cookie,
            string RegexNextPage, bool isUrlAutoRedirect,bool isGatherCoding, string codingFlag,string CodingUrl,string CodingPlugin )
        {

            List<string> gUrls = new List<string>();

            cGatherWeb gWeb = new cGatherWeb(this.m_workPath);

     

            int FirstNextIndex = 0;

            //在此处理是否需要进行Base64编码的的问题
            if (Regex.IsMatch(Url, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
            {

                Match s = Regex.Match(Url, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
                string sBase64 = s.Groups[0].Value.ToString();
                sBase64 = ToolUtil.Base64Encoding(sBase64);

                //将base64编码部分进行url替换
                Url = Regex.Replace(Url, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            }

            string NextUrl = Url;
            string Old_Url = NextUrl;

            try
            {
                //导航进入的肯定是第一层，所以在此处理第一层的翻页规则
                if (nRules[0].IsNext)
                {
                    #region 处理导航 且具备下一页自动翻页的采集

                    int curNextPage = 0;

                    do
                    {
                       
                            Url = NextUrl;
                            Old_Url = NextUrl;
                            
                            List<string> tmpUrls= ParseGatherNavigationUrl(Url, 1, nRules, index,IsProxy ,IsProxyFirst,referUrl,ref ProxyControl,headers,
                                isUrlCode,isTwoUrlCode, webCode, urlCode, isAutoUpdateHeader,
                                ref Cookie, RegexNextPage, isUrlAutoRedirect,isGatherCoding, codingFlag, CodingUrl, CodingPlugin); //, NagRule, IsOppPath);

                            if (nRules.Count > 1)
                                gUrls.AddRange(tmpUrls);
                            else
                                gUrls.Add(Url);

                            //开始分解下一页网址
                            int ANum = 0;
                            string webSource = string.Empty;

                        ANumber:
                            try
                            {

                           

                            //webSource = gWeb.GetHtml(Url, webCode, isUrlCode,isTwoUrlCode, urlCode, ref Cookie, "", "",
                            //        true, isAutoUpdateHeader, referUrl, isGatherCoding, codingFlag, CodingUrl, CodingPlugin);
                                ANum++;
                            }
                            catch (System.Exception ex)
                            {
                                if (ANum > 3)
                                    throw ex;
                                else
                                    goto ANumber;
                            }

                        cUrlAnalyze cu = new cUrlAnalyze(this.m_workPath , ref ProxyControl, IsProxy, IsProxyFirst, cGlobalParas.ProxyType.TaskConfig, "", 0);
                            string cookie = Cookie;
                            string strNext = cu.GetNextUrl(Url, webSource, nRules[0].NextRule, RegexNextPage, 
                                ref FirstNextIndex,1);

                            referUrl = Url;

                            Cookie = cookie;
                            cu = null;

                            //e_Log(this, new cGatherTaskLogArgs(m_TaskID, TaskName, cGlobalParas.LogType.Info, "下一页网址获取成功：" + NextUrl, this.IsErrorLog));

                            NextUrl = strNext;

                            //更新地址，因为是自动翻页，所以采集任务无法感知翻页到了那里，所以需要
                            //在此进行更新
                            //m_TaskSplitData.Weblink[index].NextPageUrl = NextUrl;

                            //翻页成功
                            curNextPage++;
                            if (nRules[0].NextMaxPage != "0" && curNextPage >= int.Parse(nRules[0].NextMaxPage))
                                break;

                       

                    }
                    while (NextUrl != "" && Old_Url != NextUrl && NextUrl != "#");

                    //e_Log(this, new cGatherTaskLogArgs(m_TaskID, TaskName, cGlobalParas.LogType.Info, "已经到最终页", this.IsErrorLog));

                    #endregion
                }
                else
                {

                    gUrls = ParseGatherNavigationUrl(Url, 1, nRules, index,IsProxy,IsProxyFirst,referUrl,ref ProxyControl,
                        headers, isUrlCode,isTwoUrlCode,  webCode, urlCode, isAutoUpdateHeader,
                        ref Cookie, RegexNextPage, isUrlAutoRedirect,isGatherCoding, codingFlag, CodingUrl, CodingPlugin); //, NagRule, IsOppPath);
                }
            }
            catch (System.Exception ex)
            {
                //e_Log(this, new cGatherTaskLogArgs(m_TaskID, TaskName, cGlobalParas.LogType.Error, Url + "采集发生错误：" + ex.Message, this.IsErrorLog));
                return gUrls;
            }

            gWeb = null;

            return gUrls;
        }

       
      
        private List<string> ParseGatherNavigationUrl(string Url, int level, List<eNavigRule> nRules,
            int index, bool IsProxy, bool IsProxyFirst, string referUrl, ref cProxyControl ProxyControl,  List<eHeader> headers,
            bool isUrlCode, bool isTwoUrlCode, cGlobalParas.WebCode webCode,cGlobalParas.WebCode urlCode,bool isAutoUpdateHeader,ref string Cookie,
            string RegexNextPage, bool isUrlAutoRedirect,bool isGatherCoding, string codingFlag, string CodingUrl, string CodingPlugin)
        {

            string iUrl = Url;
            int FirstNextIndex = 0;


            NetMiner.Gather.Url.cUrlAnalyze u = new NetMiner.Gather.Url.cUrlAnalyze(this.m_workPath , ref ProxyControl, IsProxy, IsProxyFirst, cGlobalParas.ProxyType.TaskConfig, "", 0);

            List<string> gUrls = new List<string>();                                       //记录导航返回的Url列表
            List<eNavigRule> tmpNRules = new List<eNavigRule>();         //仅记录当前导航级别的规则 是一个集合，因为是分层导航，所以一个集合仅记录一条
            eNavigRule tmpNRule = new eNavigRule();                      //记录当前导航级别的导航规则


            #region 根据传入的级别获取导航级别信息

            //发现当前导航级别的导航规则
            for (int i = 0; i < nRules.Count; i++)
            {
                if (level == nRules[i].Level)
                {
                    tmpNRule.Level = 1;
                    tmpNRule.NaviStartPos = nRules[i].NaviStartPos;
                    tmpNRule.NaviEndPos = nRules[i].NaviEndPos;
                    tmpNRule.NavigRule = nRules[i].NavigRule;

                    tmpNRule.IsNaviNextPage = nRules[i].IsNaviNextPage;
                    tmpNRule.NaviNextPage = nRules[i].NaviNextPage;
                    tmpNRule.NaviNextMaxPage = nRules[i].NaviNextMaxPage;

                    tmpNRule.IsGather = nRules[i].IsGather;
                    tmpNRule.GatherStartPos = nRules[i].GatherStartPos;
                    tmpNRule.GatherEndPos = nRules[i].GatherEndPos;

                    tmpNRule.IsNext = nRules[i].IsNext;
                    tmpNRule.NextRule = nRules[i].NextRule;
                    tmpNRule.NextMaxPage = nRules[i].NextMaxPage;

                    //tmpNRule.IsNextDoPostBack = nRules[i].IsNextDoPostBack;
                    //tmpNRule.IsNaviNextDoPostBack = nRules[i].IsNaviNextDoPostBack;

                    tmpNRules.Add(tmpNRule);
                    break;
                }
            }

            #endregion


            //根据导航规则找到网址
            List<string> gTempUrls = null;
            try
            {
                gTempUrls = u.ParseUrlRule(Url,"", tmpNRules,headers);
            }
            catch (System.Exception ex)
            {
                return null;
            }

            if (gTempUrls == null)
            {
                return gUrls;
            }

            u = null;


            cGatherWeb gWeb = new cGatherWeb(this.m_workPath);

            if (level == nRules.Count)
            {
                gUrls = gTempUrls;
            }
            else
            {
                //如果不是内容页，则表示为多级导航，在此需要处理导航级别的翻页规则
                //同时需要根据上层导航出的网址进行逐个采集
                #region
                for (int j = 0; j < gTempUrls.Count; j++)
                {

                    if (tmpNRule.IsNaviNextPage)
                    {
                        #region 在此处理分页导航的规则

                        string NextUrl = gTempUrls[j].ToString();
                        string Old_Url = NextUrl;

                        int curNextPage = 0;

                        do
                        {
                            
                                Url = NextUrl;
                                Old_Url = NextUrl;

                                List<string> tmpUrls = ParseGatherNavigationUrl(Url, level + 1, nRules, index,IsProxy,IsProxy,referUrl,ref ProxyControl,
                                    headers, isUrlCode,isTwoUrlCode, webCode, urlCode,
                                    isAutoUpdateHeader, ref Cookie, iUrl, isUrlAutoRedirect,
                                    isGatherCoding, codingFlag, CodingUrl, CodingPlugin);

                                if (nRules.Count > level)
                                    gUrls.AddRange(tmpUrls);
                                else
                                    gUrls.Add(Url);

                                cUrlAnalyze cu = new cUrlAnalyze(this.m_workPath , ref ProxyControl, IsProxy, IsProxyFirst, cGlobalParas.ProxyType.TaskConfig, "", 0);

                                string strNext = cu.GetNextUrl(Url, "", tmpNRule.NaviNextPage, RegexNextPage, 
                                    ref FirstNextIndex, level);

                                referUrl = Url;
                                cu = null;

                                NextUrl = strNext;

                                //翻页成功
                                curNextPage++;
                                if (tmpNRule.NaviNextMaxPage != "0" && curNextPage >= int.Parse(tmpNRule.NaviNextMaxPage))
                                    break;

                         

                        }
                        while (NextUrl != "" && Old_Url != NextUrl && NextUrl != "#");
                        //e_Log(this, new cGatherTaskLogArgs(m_TaskID, TaskName, cGlobalParas.LogType.Info, "已经到最终页", this.IsErrorLog));
                        #endregion
                    }
                    else
                    {
                            try
                            {
                                List<string> tmpUrls = ParseGatherNavigationUrl(gTempUrls[j].ToString(), level + 1, nRules, index, IsProxy, IsProxyFirst, referUrl, ref ProxyControl,
                                    headers, isUrlCode,isTwoUrlCode, webCode, urlCode, 
                                    isAutoUpdateHeader, ref Cookie, RegexNextPage, isUrlAutoRedirect,
                                    isGatherCoding,codingFlag, CodingUrl, CodingPlugin);
                                gUrls.AddRange(tmpUrls);
                            }
                            catch (System.Exception ex)
                            {
                                
                            }
                      
                    }
                    
                }
                #endregion
            }

           

            gTempUrls = null;

            tmpNRule = null;
            tmpNRules = null;

            return gUrls;
        }

        #endregion

        /// <summary>
        /// 保存分解后的采集任务，注意：只保存最后一层网址，所以如果是导航采集则保存会出错
        /// </summary>
        /// <param name="gUrls"></param>s
        /// <param name="webLink"></param>
        /// <param name="t"></param>
        /// <param name="startTaskIndex"></param>
        /// <returns></returns>
        private List<cSplitTaskEntity> SaveSplitTask(List<string> gUrls, oTask t,ref int startTaskIndex)
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
                        wLink.Weblink = gUrls[startIndex];

                        //for (int n = 1; n < webLink.NavigRules.Count; n++)
                        //{
                        //    wLink.NavigRules.Add(webLink.NavigRules[n]);
                        //}

                        startIndex++;
                        wLinks.Add(wLink);
                    }

                }

                if (!Directory.Exists(m_TaskPath + TaskName))
                    Directory.CreateDirectory(m_TaskPath + TaskName);
                ct.TaskEntity.WebpageLink.AddRange(wLinks);
                string newTaskName= TaskName + "___" + startTaskIndex.ToString("00");
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

            return ts;
          
        }

        /// <summary>
        /// 判断是否分解采集任务
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool isSplit(oTask t)
        {

            //先判断入口网址是多少
            if (t.TaskEntity.WebpageLink.Count != t.TaskEntity.UrlCount)
            {
                //表示有参数
                if (t.TaskEntity.UrlCount >this.m_MaxSplitMaxUrlsCount)
                {
                    return true ;
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

        /// <summary>
        /// 获取分解后任务的发布规则
        /// </summary>
        /// <param name="tName"></param>
        /// <returns></returns>
        public ePublishTask GetSplitPublishRule(string tName)
        {
            string sql = "SELECT SM_MyTask.SavePath, SM_SplitTask.TID FROM         SM_MyTask INNER JOIN"
                + " SM_SplitTask ON SM_MyTask.ID = SM_SplitTask.TID where SM_SplitTask.TaskName='" + tName + "'";

            DataTable d = null;

            if (m_dbType ==cGlobalParas.DatabaseType.MSSqlServer)
                d=NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(this.m_Conn, sql);
            else if (m_dbType ==cGlobalParas.DatabaseType.MySql)
                d = NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(this.m_Conn, sql);

            if (d == null || d.Rows.Count == 0)
                return null;

            string TaskName = d.Rows[0]["SavePath"].ToString();
            string tID = d.Rows[0]["TID"].ToString();


            oTask t = new oTask(m_workPath);
            t.LoadTask(TaskName);


            //先建立一个发布的任务，是一个虚拟的，主要用于发布信息的传递
            #region 先初始化一个cpublishtask，将发布规则导入
            ePublishTask p = new ePublishTask();
            p.pName = tName;
            p.ThreadCount = t.TaskEntity.PublishThread;
            p.IsDelRepeatRow = t.TaskEntity.IsDelRepRow;

            if (t.TaskEntity.ExportType  == cGlobalParas.PublishType.publishTemplate)
            {
                //以模板发布数据
                p.PublishType = cGlobalParas.PublishType.publishTemplate;
                p.TemplateName = t.TaskEntity.TemplateName;
                p.User = t.TaskEntity.User;
                p.Password = t.TaskEntity.Password;
                p.Domain = t.TaskEntity.Domain;
                p.TemplateDBConn = t.TaskEntity.TemplateDBConn;
                p.PublishParas = t.TaskEntity.PublishParas;
                
            }
            else
            {
                if (t.TaskEntity.ExportType == cGlobalParas.PublishType.PublishAccess ||
                    t.TaskEntity.ExportType == cGlobalParas.PublishType.PublishMSSql ||
                    t.TaskEntity.ExportType ==cGlobalParas.PublishType.PublishMySql ||
                    t.TaskEntity.ExportType == cGlobalParas.PublishType.publishOracle)
                {
                    //发布到数据库
                    p.PublishType = cGlobalParas.PublishType.PublishData;
                    if (t.TaskEntity.ExportType == cGlobalParas.PublishType.PublishAccess)
                        p.DataType = cGlobalParas.DatabaseType.Access;
                    else if (t.TaskEntity.ExportType ==cGlobalParas.PublishType.PublishMSSql)
                        p.DataType = cGlobalParas.DatabaseType.MSSqlServer;
                    else if (t.TaskEntity.ExportType == cGlobalParas.PublishType.PublishMySql)
                        p.DataType = cGlobalParas.DatabaseType.MySql;
                    else if (t.TaskEntity.ExportType ==cGlobalParas.PublishType.publishOracle)
                        p.DataType = cGlobalParas.DatabaseType.Oracle;


                    p.DataSource = t.TaskEntity.DataSource;
                    p.DataTable = t.TaskEntity.DataTableName;
                    p.InsertSql = t.TaskEntity.InsertSql;
                    p.IsSqlTrue = t.TaskEntity.IsSqlTrue;
                }
                else
                    p.PublishType = cGlobalParas.PublishType.NoPublish;
            }

            #endregion 

            t = null;

            return p;
        }

        /// <summary>
        /// 获取采集任务的发布规则
        /// </summary>
        /// <param name="tName"></param>
        /// <returns></returns>
        public ePublishTask GetPublishRule(int tID)
        {
            string sql = "SELECT SM_MyTask.TaskName, SM_MyTask.SavePath FROM  SM_MyTask  where SM_MyTask.ID='" + tID + "'";

            DataTable d = null;
            if (m_dbType == cGlobalParas.DatabaseType.MSSqlServer)
                d = NetMiner.Data.SqlServer.SQLHelper.ExecuteDataTable(this.m_Conn, sql);
            else if (m_dbType == cGlobalParas.DatabaseType.MySql)
                d = NetMiner.Data.Mysql.SQLHelper.ExecuteDataTable(this.m_Conn, sql);

            if (d == null || d.Rows.Count == 0)
                return null;

            string TaskName = d.Rows[0]["SavePath"].ToString();
            string tName = d.Rows[0]["TaskName"].ToString();

            oTask t = new oTask(m_workPath);
            t.LoadTask(TaskName);


            //先建立一个发布的任务，是一个虚拟的，主要用于发布信息的传递
            #region 先初始化一个cpublishtask，将发布规则导入
            ePublishTask p = new ePublishTask();
            p.pName = tName;
            p.ThreadCount = t.TaskEntity.PublishThread;
            p.IsDelRepeatRow = t.TaskEntity.IsDelRepRow;

            if (t.TaskEntity.ExportType == cGlobalParas.PublishType.publishTemplate)
            {
                //以模板发布数据
                p.PublishType = cGlobalParas.PublishType.publishTemplate;
                p.TemplateName = t.TaskEntity.TemplateName;
                p.User = t.TaskEntity.User;
                p.Password = t.TaskEntity.Password;
                p.Domain = t.TaskEntity.Domain;
                p.TemplateDBConn = t.TaskEntity.TemplateDBConn;
                p.PublishParas = t.TaskEntity.PublishParas;

            }
            else
            {
                if (t.TaskEntity.ExportType ==cGlobalParas.PublishType.PublishAccess ||
                    t.TaskEntity.ExportType == cGlobalParas.PublishType.PublishMSSql ||
                    t.TaskEntity.ExportType ==cGlobalParas.PublishType.PublishMySql ||
                    t.TaskEntity.ExportType == cGlobalParas.PublishType.publishOracle)
                {
                    //发布到数据库
                    p.PublishType = cGlobalParas.PublishType.PublishData;
                    if (t.TaskEntity.ExportType == cGlobalParas.PublishType.PublishAccess)
                        p.DataType = cGlobalParas.DatabaseType.Access;
                    else if (t.TaskEntity.ExportType == cGlobalParas.PublishType.PublishMSSql)
                        p.DataType = cGlobalParas.DatabaseType.MSSqlServer;
                    else if (t.TaskEntity.ExportType == cGlobalParas.PublishType.PublishMySql)
                        p.DataType = cGlobalParas.DatabaseType.MySql;
                    else if (t.TaskEntity.ExportType == cGlobalParas.PublishType.publishOracle)
                        p.DataType = cGlobalParas.DatabaseType.Oracle;


                    p.DataSource = t.TaskEntity.DataSource;
                    p.DataTable = t.TaskEntity.DataTableName;
                    p.InsertSql = t.TaskEntity.InsertSql;
                    p.IsSqlTrue = t.TaskEntity.IsSqlTrue;
                }
                else
                    p.PublishType = cGlobalParas.PublishType.NoPublish;
            }

            #endregion

            t = null;

            return p;
        }
       
    }
}

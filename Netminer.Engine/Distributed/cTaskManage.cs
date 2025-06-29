using System;
using System.Collections.Generic;
using System.Text;
using Soukey.Task;
using Soukey;
using Soukey.Proxy;
using SoukeyResource;
using System.IO;
using System.Data;
using SominerData.SqlServer;

///分布式采集的采集任务分解类，支持多种采集任务分解策略
///1、根据当前网络环境进行分解
///2、根据当前的采集任务执行时长进行分解
namespace SoukeyEngine.Distributed
{
    public class cTaskManage
    {
        //private int m_MaxDSpeed;
        //private int m_MaxGCount;
        private int m_MaxSplitMaxUrlsCount = 200;
        private string m_TaskPath = cTool.getPrjPath() + "tasks\\";
        private string m_Conn;
        private int m_MaxSplitLevel;

        public cTaskManage(string con,int MaxSplitUrls,int MaxSplitLevel)
        {
            m_Conn = con;
            this.m_MaxSplitMaxUrlsCount = MaxSplitUrls;
            this.m_MaxSplitLevel = MaxSplitLevel;
        }


        ~cTaskManage()
        {
        }

        /// <summary>
        /// 分解采集任务，有几种情况是不允许分解的：1、导航采集；2：分页不进行拆分处理
        /// </summary>
        public List<cSplitTask> SplitTask(string TaskName,out cGlobalParas.SplitTaskState sState)
        {
           

            cTask t = new cTask();
            t.LoadTask(m_TaskPath + TaskName + ".smt");

            bool isS=isSplit(t);

            if (isS == true)
            {
                List<cSplitTask> ts = SplitTask(t, TaskName);
                
                sState = cGlobalParas.SplitTaskState.Splited;
                return ts;
            }
            else
            {
                sState = cGlobalParas.SplitTaskState.WithoutSplit;
                return null;
            }

        }

        private List<cSplitTask> SplitTask(cTask t, string TaskName)
        {
            List<string> gUrls = new List<string>();                                     //记录导航返回的Url列表
            List<cNavigRule> tmpNRules = new List<cNavigRule>();         //仅记录当前导航级别的规则 是一个集合，因为是分层导航，所以一个集合仅记录一条
            cNavigRule tmpNRule = new cNavigRule();
            cProxyControl pControl = new cProxyControl();
            List<cSplitTask> sTasks = new List<cSplitTask>();

            int startTaskIndex = 1;

            for (int i = 0; i < t.WebpageLink.Count; i++)
            {
                if (t.WebpageLink[i].NavigRules.Count > m_MaxSplitLevel)
                {
                    if (t.WebpageLink[i].NavigRules[0].IsGather == false)
                    {
                        //只有导航层级大于2，且不是导航采集时方可分解
                        List<cNavigRule> nRules = t.WebpageLink[0].NavigRules;

                        //只提取第一层导航
                        tmpNRule.Level = 1;
                        tmpNRule.NaviStartPos = nRules[0].NaviStartPos;
                        tmpNRule.NaviEndPos = nRules[0].NaviEndPos;
                        tmpNRule.NavigRule = nRules[0].NavigRule;

                        tmpNRule.IsNaviNextPage = nRules[0].IsNaviNextPage;
                        tmpNRule.NaviNextPage = nRules[0].NaviNextPage;
                        tmpNRule.NaviNextMaxPage = nRules[0].NaviNextMaxPage;

                        tmpNRule.IsGather = nRules[0].IsGather;
                        tmpNRule.GatherStartPos = nRules[0].GatherStartPos;
                        tmpNRule.GatherEndPos = nRules[0].GatherEndPos;

                        tmpNRule.IsNext = nRules[0].IsNext;
                        tmpNRule.NextRule = nRules[0].NextRule;
                        tmpNRule.NextMaxPage = nRules[0].NextMaxPage;

                        tmpNRule.IsNextDoPostBack = nRules[0].IsNextDoPostBack;
                        tmpNRule.IsNaviNextDoPostBack = nRules[0].IsNaviNextDoPostBack;

                        tmpNRules.Add(tmpNRule);

                        cUrlAnalyze u = new cUrlAnalyze(ref pControl, t.IsProxy, t.IsProxyFirst);

                        string cookie = t.Cookie;
                        List<string> tmpUrls = u.ParseUrlRule(t.WebpageLink[0].Weblink, tmpNRules, (cGlobalParas.WebCode)int.Parse(t.WebCode), 
                            t.IsUrlEncode, (cGlobalParas.WebCode)int.Parse(t.UrlEncode),ref cookie, t.Headers,"",t.IsAutoUpdateHeader);
                        t.Cookie = cookie;
                        List<cSplitTask> ts= SaveSplitTask(tmpUrls,t.WebpageLink[i], t, ref startTaskIndex);
                        sTasks.AddRange(ts);
                    }
                }
                else if (t.UrlCount != t.WebpageLink.Count)
                {
                    //表示有参数，需要分解url

                    cUrlAnalyze cu = new cUrlAnalyze(ref pControl, t.IsProxy, t.IsProxyFirst);
                    List<string> tmpUrls = cu.SplitWebUrl(t.WebpageLink[i].Weblink);
                    cu = null;

                    List<cSplitTask> ts = SaveSplitTask(tmpUrls, t.WebpageLink[i], t, ref startTaskIndex);
                    sTasks.AddRange(ts);
                }
            }

            return sTasks;
        }

        private List<cSplitTask> SaveSplitTask(List<string> gUrls, cWebLink webLink, cTask t,ref int startTaskIndex)
        {
            List<cSplitTask> ts = new List<cSplitTask>();

            int tCount = gUrls.Count / this.m_MaxSplitMaxUrlsCount;
            int tRem = gUrls.Count % this.m_MaxSplitMaxUrlsCount;

            string TaskName = t.TaskName;

            if (tRem > 0)
                tCount++;

            int startIndex = 0;

            for (int j = 0; j < tCount; j++)
            {
                //保存分解后的采集任务
                cTask ct = new cTask();
                ct.LoadTask(m_TaskPath + TaskName + ".smt");

                //清空网址
                ct.WebpageLink.Clear();
                List<cWebLink> wLinks = new List<cWebLink>();

                for (int m = 0; m < m_MaxSplitMaxUrlsCount; m++)
                {
                    if (startIndex < gUrls.Count)
                    {
                        cWebLink wLink = new cWebLink();
                        wLink.Weblink = gUrls[startIndex];

                        for (int n = 1; n < webLink.NavigRules.Count; n++)
                        {
                            wLink.NavigRules.Add(webLink.NavigRules[n]);
                        }

                        startIndex++;
                        wLinks.Add(wLink);
                    }

                }

                if (!Directory.Exists(m_TaskPath + TaskName))
                    Directory.CreateDirectory(m_TaskPath + TaskName);
                ct.WebpageLink.AddRange(wLinks);
                string newTaskName= TaskName + "___" + startTaskIndex.ToString("00");
                ct.TaskName = newTaskName;
                ct.UrlCount = ct.WebpageLink.Count;
                ct.SaveTask(m_TaskPath + TaskName + "\\" + newTaskName + ".smt");
                ct = null;

                //插入分解任务信息
                cSplitTask st = new cSplitTask();
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
        private bool isSplit(cTask t)
        {
            //先判断入口网址是多少
            if (t.WebpageLink.Count != t.UrlCount)
            {
                //表示有参数
                if (t.UrlCount >this.m_MaxSplitMaxUrlsCount)
                {
                    return true ;
                }
            }

            if (t.WebpageLink[0].NavigRules.Count > m_MaxSplitLevel && t.WebpageLink[0].NavigRules[0].IsGather == false)
            {
                return true;
            }

            return false;
        }

        public cPublishTask GetPublishRule(string tName)
        {
            
            string sql = "SELECT SM_MyTask.SavePath, SM_SplitTask.TID FROM         SM_MyTask INNER JOIN"
                + " SM_SplitTask ON SM_MyTask.ID = SM_SplitTask.TID where SM_SplitTask.TaskName='" + tName + "'";

            DataTable d = SQLHelper.ExecuteDataTable(this.m_Conn, sql);
            if (d == null || d.Rows.Count == 0)
                return null;

            string TaskName = d.Rows[0]["SavePath"].ToString();
            string tID = d.Rows[0]["TID"].ToString();
            
            
            cTask t = new cTask();
            t.LoadTask(TaskName);


            //先建立一个发布的任务，是一个虚拟的，主要用于发布信息的传递
            #region 先初始化一个cpublishtask，将发布规则导入
            cPublishTask p = new cPublishTask();
            p.pName = tName;
            p.ThreadCount = t.PublishThread;
            p.IsDelRepeatRow = t.IsDelRepRow;

            if (t.ExportType  == ((int)cGlobalParas.PublishType.publishTemplate).ToString())
            {
                //以模板发布数据
                p.PublishType = cGlobalParas.PublishType.publishTemplate;
                p.TemplateName = t.TemplateName;
                p.User = t.User;
                p.Password = t.Password;
                p.Domain = t.Domain;
                p.TemplateDBConn = t.TemplateDBConn;
                p.PublishParas = t.PublishParas;
                
            }
            else
            {
                if (t.ExportType == ((int)cGlobalParas.PublishType.PublishAccess).ToString() ||
                    t.ExportType == ((int)cGlobalParas.PublishType.PublishMSSql).ToString() ||
                    t.ExportType == ((int)cGlobalParas.PublishType.PublishMySql).ToString() ||
                    t.ExportType == ((int)cGlobalParas.PublishType.publishOracle).ToString())
                {
                    //发布到数据库
                    p.PublishType = cGlobalParas.PublishType.PublishData;
                    if (t.ExportType == ((int)cGlobalParas.PublishType.PublishAccess).ToString())
                        p.DataType = cGlobalParas.DatabaseType.Access;
                    else if (t.ExportType == ((int)cGlobalParas.PublishType.PublishMSSql).ToString())
                        p.DataType = cGlobalParas.DatabaseType.MSSqlServer;
                    else if (t.ExportType == ((int)cGlobalParas.PublishType.PublishMySql).ToString())
                        p.DataType = cGlobalParas.DatabaseType.MySql;
                    else if (t.ExportType == ((int)cGlobalParas.PublishType.publishOracle).ToString())
                        p.DataType = cGlobalParas.DatabaseType.Oracle;


                    p.DataSource = t.DataSource;
                    p.DataTable = t.DataTableName;
                    p.InsertSql = t.InsertSql;
                    p.IsSqlTrue = t.IsSqlTrue;
                }
                else if (t.ExportType == ((int)cGlobalParas.PublishType.PublishWeb).ToString())
                {
                    //发布到网站
                    p.PublishType = cGlobalParas.PublishType.PublishWeb;
                    p.PostUrl = t.ExportUrl;
                    p.UrlCode = (cGlobalParas.WebCode)int.Parse(t.ExportUrlCode);
                    p.Cookie = t.ExportCookie;
                    p.SucceedFlag = t.PSucceedFlag;
                    p.IsHeader = t.IsExportHeader;

                    StringBuilder strHeader = new StringBuilder();
                    for (int index = 0; index < t.Headers.Count; index++)
                    {
                        strHeader.Append(t.Headers[index].Label + ":" + t.Headers[index].LabelValue);
                    }
                    p.Header = strHeader.ToString();

                    p.PIntervalTime = t.PIntervalTime;
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

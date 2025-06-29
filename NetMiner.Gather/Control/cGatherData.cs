using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NetMiner.Core.gTask.Entity;
using NetMiner.Resource;
using NetMiner.Core.Event;
using System.Text.RegularExpressions;
using NetMiner.Core.Entity;
using NetMiner.Gather.Url;
using NetMiner.Net.Common;
using NetMiner.Net.Native;
using NetMiner.Common;
using System.Threading;
using System.Collections;
using NetMiner.Core.Proxy;
using System.Net;
using NetMiner.Core.Proxy.Entity;

/// <summary>
/// 从V5.5开始，启用此类，原有模式全部作废。单独出来的目的是为了解耦。同时将获取网页的操作，支持双引擎
/// </summary>
namespace NetMiner.Gather.Control
{
    /// <summary>
    /// 数据采集的核心类，此类由cGatherTaskSplit中的线程创建完成一个地址的获取操作
    /// </summary>
    internal class cGatherData
    {
        private readonly Object m_RepeatUrlsLock = new Object();

        private cTaskSplitData m_TaskSplitData;
        private eTask m_Task;
        //定义一个值，来记录连续出错的次数（注意：是连续）
        private int m_ContinuousErr;

        //定义一个值，来记录连续直接入库的错误次数（注意：是连续）
        private int m_ContinuousInsertErr;
        private string m_workPath = string.Empty;

        //设置一个参数值，判断，如果数据采集是采用了直接入库的方式
        //判断数据表是否需要新建，如果新建，则在第一次运行时，建立新表
        //成功后，并修改此时为false，如果已经存在此表，也许修改为false
        private bool IsNewTable = true;

        private bool m_ThreadRunning = false;

        //定义一个值，存储Cookie
        private string m_tmpCookie = string.Empty;

        private bool m_IsMergeData = false;
        private cProxyControl m_ProxyControl;

        /// <summary>
        /// 这是一个具体的采集数据的操作类
        /// </summary>
        private cGatherWeb m_gWeb;

        public cGatherData(eTask t, cTaskSplitData TaskSplitData,string workPath, ref cProxyControl ProxyControl)
        {
            m_TaskSplitData = TaskSplitData;
            m_Task = t;
            m_workPath = workPath;
            m_ProxyControl = ProxyControl;

            m_gWeb = new cGatherWeb(workPath);
            m_gWeb.Log += this.onLog;
            m_gWeb.ForcedStopped += this.onForcedStopped;

            for (int i = 0; i < this.m_TaskSplitData.CutFlag.Count; i++)
            {
                if (this.m_TaskSplitData.CutFlag[i].IsMergeData == true)
                {
                    m_IsMergeData = true;
                    break;
                }
            }
        }

        ~cGatherData()
        {
            m_gWeb.Log -= this.onLog;
            m_gWeb.ForcedStopped -= this.onForcedStopped;
            m_gWeb =null;
        }

        #region 事件
        private void onLog(object sender, cGatherTaskLogArgs e)
        {
            if (e_Log != null)
                e_Log(sender, e);
        }

        private void onForcedStopped(object sender, cTaskEventArgs e)
        {
            if (e_RequestStopped != null)
                e_RequestStopped(sender, e);
        }
        #endregion

        #region 必须要进行初始化的属性
        public bool ThreadRunning
        {
            get { return m_ThreadRunning; }
            set { m_ThreadRunning = value; }
        }
        public string[] AutoID { get; set; }
        public bool m_isGlobalRepeat { get; set; }
        public NetMiner.Base.cHashTree g_Urls { get; set; }
        /// <summary>
        /// 定义一个排重库，进行url排重
        /// </summary>
        public NetMiner.Base.cHashTree m_Urls;
        /// <summary>
        /// 定义一个排重库，进行当前任务实例运行的排重，主要解决断点续采的问题
        /// </summary>
        public NetMiner.Base.cHashTree m_TempUrls { get; set; }
        /// <summary>
        /// 定义一个排重库，进行采集数据的重复性判断
        /// </summary>
        public NetMiner.Base.cHashTree m_DataRepeat { get; set; }
        public cGlobalParas.ProxyType pType { get; set; }
        public string ProxyAddress { get; set; }
        public int ProxyPort { get; set; }

        public string RegexNextPage { get; set; }

        #endregion

        #region 
        internal cGlobalParas.GatherResult GatherSingleUrl(string Url, int Level, bool IsNext, string NextRule, int index,
            DataRow tRow, bool isMulti121, List<eMultiPageRule> listMultiRules, string referUrl)
        {
            string hUrl = Url;

            //在此处理断点续采的问题
            if (!this.m_TempUrls.Add(ref Url, false))
            {
                e_Log(this, new cGatherTaskLogArgs( this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "网址：" + Url + "已经采集，无需再采", this.m_Task.IsErrorLog));

                return cGlobalParas.GatherResult.GatherSucceed;
            }

            //在此处理用户选择了排重库的问题
            if (this.m_Task.IsUrlNoneRepeat == true)
            {

                if (!this.m_Urls.Add(ref Url, false))
                {
                    e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "此任务设置了排重，网址：" + Url + "已经采集，无需再采", this.m_Task.IsErrorLog));
                    return cGlobalParas.GatherResult.GatherSucceed;
                }
            }

            //在此处理全局排重，全局排重需要进行加锁，仅限于采集引擎使用
            if (this.m_isGlobalRepeat == true)
            {
                lock (m_RepeatUrlsLock)
                {
                    if (!this.g_Urls.Add(ref Url, false))
                    {
                        e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "此任务设置了排重，网址：" + Url + "已经采集，无需再采", this.m_Task.IsErrorLog));
                        return cGlobalParas.GatherResult.GatherSucceed;
                    }
                }
            }

            string hSource=string.Empty;

            //在此处理是否需要进行Base64编码的的问题
            if (Regex.IsMatch(Url, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
            {

                Match s = Regex.Match(Url, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
                string sBase64 = s.Groups[0].Value.ToString();
                sBase64 = ToolUtil.Base64Encoding(sBase64);

                //将base64编码部分进行url替换
                Url = Regex.Replace(Url, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            }

            //获取此网页的源代码
            bool isVisual = false;
            

            try
            {

                //开始分级多页数据，并实现与界面的对应关系，完成后，调用gathersingleurl进行数据采集
                List<eMultiPage> listMultiPage = new List<eMultiPage>();

                #region 先增加采集页的数据
                List<eWebpageCutFlag> gPageCutFlags = new List<eWebpageCutFlag>();

                for (int j = 0; j < m_TaskSplitData.CutFlag.Count; j++)
                {
                    eWebpageCutFlag CutFlag = new eWebpageCutFlag();
                    if (m_TaskSplitData.CutFlag[j].NavLevel == 0)
                    {
                        if (isVisual == false && !string.IsNullOrEmpty(m_TaskSplitData.CutFlag[j].XPath))
                            isVisual = true;
                        gPageCutFlags.Add(m_TaskSplitData.CutFlag[j]);
                    }
                    
                }

                //先进行采集页数据的采集工作
                m_TaskSplitData.CurUrl = Url;

                DataTable gtmpData;

                eResponse response = GetHtml(Url, referUrl,isVisual);
                hSource = response.Body;
                gtmpData = GetGatherData(Url, Level, ref m_tmpCookie,gPageCutFlags, response, true, referUrl, tRow, "", 0, false);


                #endregion


                #region 开始多页采集并合并

                DataTable dt = new DataTable();

                //增加多页采集的网址和规则,数据可能是多对多，但关系
                //还是按照一对多的处理规则进行
                if (gtmpData == null)
                {
                    throw new NetMinerException("您设置了多页采集，但由于采集页未能采集到数据，所以终止了多页的采集。");
                }
                for (int m = 0; m < gtmpData.Rows.Count; m++)
                {
                    DataTable dtmp = gtmpData.Clone();
                    DataRow dr = gtmpData.Rows[m];
                    dtmp.ImportRow(dr);

                    DataTable dgTemp = new DataTable();

                    for (int i = 0; i < listMultiRules.Count; i++)
                    {
                        if (listMultiRules[i].mLevel == 0)
                        {
                            eMultiPage cM = new eMultiPage();
                            string UrlRule = listMultiRules[i].Rule;

                            //先替换时间戳参数
                            if (Regex.IsMatch(UrlRule, "{Timestamp:[\\d]*?}"))
                            {
                                UrlRule = Regex.Replace(UrlRule, "{Timestamp:[\\d]*?}", ToolUtil.GetTimestamp().ToString());
                            }

                            #region 处理多页规则
                            if (UrlRule.StartsWith("<ParaUrl>"))
                            {
                                Match charSetMatch1 = Regex.Match(UrlRule, "(?<=<ParaUrl>).*?(?=</ParaUrl>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                string mUrl = charSetMatch1.Groups[0].ToString();

                                //替换参数
                                //while (mUrl.IndexOf("{") > 0)
                                while (Regex.IsMatch(mUrl, "[^\\\\]{.*[^\\\\]}"))
                                {
                                    string strMatch = "(?<={)[^{]*[^\\\\](?=})";
                                    Match s = Regex.Match(mUrl, strMatch, RegexOptions.IgnoreCase);
                                    string p = s.Groups[0].Value;

                                    string pValue = gtmpData.Rows[m][p].ToString();
                                    mUrl = mUrl.Replace("{" + p + "}", pValue); // strPre + pValue + strSuf;
                                }

                                cM.Url = mUrl;

                                for (int j = 0; j < m_TaskSplitData.CutFlag.Count; j++)
                                {
                                    if (m_TaskSplitData.CutFlag[j].MultiPageName == listMultiRules[i].RuleName)
                                        cM.WebPageCutFlags.Add(m_TaskSplitData.CutFlag[j]);
                                }

                                #region 在此处理Header需要从来路地址获取数据的头信息
                                for (int hIndex = 0; hIndex < this.m_Task.Headers.Count; hIndex++)
                                {
                                    if (this.m_Task.Headers[hIndex].LabelValue.StartsWith("{ReferData:"))
                                    {
                                        string ss = this.m_Task.Headers[hIndex].LabelValue.Substring(11, this.m_Task.Headers[hIndex].LabelValue.Length - 12);

                                        Match charSetMatch5 = Regex.Match(hSource, ss, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                        string s1 = charSetMatch5.Groups[0].ToString();
                                        if (!string.IsNullOrEmpty(s1))
                                        {
                                            this.m_Task.Headers[hIndex].LabelValue = s1;
                                        }
                                    }
                                }

                                #endregion

                            }
                            else
                            {
                                //根据多页采集的规则获取多页的页面，注意，有可能通过配置的多页规则会提取出多个
                                //网址，如果是多个网址，则去第一个Url即可
                                cUrlAnalyze cu = new cUrlAnalyze(this.m_workPath, ref this.m_ProxyControl, this.m_Task.IsProxy, this.m_Task.IsProxyFirst, this.pType, this.ProxyAddress, this.ProxyPort);

                                List<string> mUrls = cu.GetUrlsByRule(Url, hSource,  UrlRule, Level, cGlobalParas.NaviRunRule.Normal, "",this.m_tmpCookie,this.m_Task.Headers );
                                cu = null;

                                if (mUrls != null && mUrls.Count > 0)
                                {
                                    cM.Url = mUrls[0];

                                    for (int j = 0; j < m_TaskSplitData.CutFlag.Count; j++)
                                    {
                                        if (m_TaskSplitData.CutFlag[j].MultiPageName == listMultiRules[i].RuleName)
                                            cM.WebPageCutFlags.Add(m_TaskSplitData.CutFlag[j]);

                                    }

                                }
                            }
                            #endregion

                            if (cM.Url == null)
                            {
                                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning,
                                    "解析多页规则：" + listMultiRules[i].RuleName + "出错，未能获取到多页地址。", this.m_Task.IsErrorLog));

                                continue;
                            }

                            #region 开始采集多页并进行数据合并
                            //开始采集多页的数据
                            m_TaskSplitData.CurUrl = cM.Url;

                            DataTable mtmpData;

                            mtmpData = GetGatherData(cM.Url, Level,  ref m_tmpCookie, cM.WebPageCutFlags, response, false, Url, tRow, "", 0, true);


                            if (mtmpData == null)
                            {
                                for (int mIndex = 0; mIndex < cM.WebPageCutFlags.Count; mIndex++)
                                {
                                    mtmpData = new DataTable();
                                    mtmpData.Columns.Add(cM.WebPageCutFlags[mIndex].Title);
                                }

                                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning, "多页规则未能采集到数据，有可能是无此数据，也可能是多页规则本身错误造成！多页地址：" + cM.Url + "，多页规则为：" + listMultiRules[i].RuleName, this.m_Task.IsErrorLog));

                                //throw new NetMinerException("多页采集规则可能有错，未能采集到数据，请检查多页信息的配置！");
                            }
                            //开始处理采集页和分页数据的合并

                            //如果多页没有配置规则，则还为null
                            if (mtmpData != null)
                            {
                                if (isMulti121 == false)
                                {
                                    DataTable dt1 = MergeTable(dtmp, mtmpData);
                                    dgTemp.Merge(dt1);

                                    dtmp = dgTemp.Copy();
                                    dgTemp.Reset();
                                }
                                else
                                {
                                    dgTemp = MergeTable121(dgTemp, mtmpData);
                                }
                            }

                            #endregion

                        }
                    }

                    if (isMulti121 == false)
                    {
                        dt.Merge(dtmp);
                    }
                    else
                    {
                        dt = MergeTable121(gtmpData, dgTemp);
                        break;
                    }
                }
                #endregion


                if (dt == null || dt.Rows.Count == 0)
                {
                }
                else
                {
                    if (tRow != null && dt != null)
                    {
                        //合并数据导航数据
                        e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "合并上一层采集数据", this.m_Task.IsErrorLog));

                        dt = MergeDataTable(tRow, dt);
                    }
                }

                //在此处理直接入库的问题
                if (this.m_Task.RunType == cGlobalParas.TaskRunType.OnlySave && dt != null && Level == 0)
                {
                    NewTable(dt.Columns);
                    InsertData(dt);
                }

                #region 在此处理插件数据发布的规则
                if (this.m_Task.IsPluginsPublish == true && dt != null && Level == 0)
                {
                    if (ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Cloud  ||
                        ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Program ||
                        ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Enterprise ||
                        ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Ultimate ||
                        ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Server ||
                        ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.DistriServer)
                    {
                        NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
                        if (this.m_Task.PluginsPublish == "")
                        {
                            if (e_Log != null)
                            {
                                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning, "配置了发布插件，却没有提供插件地址！", this.m_Task.IsErrorLog));
                            }
                        }
                        else
                        {
                            rPlugin.CallPublishData(dt, this.m_Task.PluginsPublish);
                        }
                        rPlugin = null;
                    }
                    else
                    {
                        if (e_Log != null)
                        {
                            e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning, "当前版本不支持插件功能，请获取正确版本！", this.m_Task.IsErrorLog));
                        }
                    }
                }
                #endregion

                e_GData(this, new cGatherDataEventArgs(this.m_Task.TaskID, this.m_Task.TaskName, dt, (this.m_Task.ExportType == cGlobalParas.PublishType.NoPublish ? false : true)));

                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "采集完成：" + Url, this.m_Task.IsErrorLog));


                listMultiPage = null;
                //gWeb = null;
            }
            catch (System.Exception ex)
            {
                //如果采集出错，则判断是否只记录成功的网址排重
                if (this.m_Task.IsUrlNoneRepeat == true && this.m_Task.IsSucceedUrlRepeat == true)
                {
                    //删除此排重信息
                    m_Urls.Del(hUrl, false);
                }
                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Error, Url + "采集发生错误：" + ex.Message, this.m_Task.IsErrorLog));
                //e_GUrlCount(this, new cGatherUrlCountArgs(this.m_Task.TaskID, cGlobalParas.UpdateUrlCountType.NaviErr, 0));
                //e_GUrlCount(this, new cGatherUrlCountArgs(this.m_Task.TaskID, cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd, 0));
                //m_TaskSplitData.GatheredTrueErrUrlCount++;
                //m_TaskSplitData.GatheredErrUrlCount++;
                //onError(ex);
                if (e_Error != null)
                    e_Error(this, new TaskThreadErrorEventArgs(ex));
                return cGlobalParas.GatherResult.GatherFailed;
            }

            return cGlobalParas.GatherResult.GatherSucceed;
        }

        /// <summary>
        /// 用于采集一个网页的数据，即不带导航处理，但需要处理翻页
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Level"></param>
        /// <param name="IsNext"></param>
        /// <param name="NextRule"></param>
        /// <param name="NextMaxPage"></param>
        /// <param name="IsDoPostBack"></param>
        /// <param name="index"></param>
        /// <param name="tRow"></param>
        /// <param name="referUrl"></param>
        /// <returns></returns>
        internal cGlobalParas.GatherResult GatherSingleUrl(string Url, int Level, bool IsNext, string NextRule, string NextMaxPage,
            int index, DataRow tRow, string referUrl, string loopFlag, int loopIndex)
        {
            string hUrl = Url;

            //在此处理断点续采的问题
            if (!this.m_TempUrls.Add(ref Url, false) && string.IsNullOrEmpty(loopFlag))
            {
                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "网址：" + Url + "已经采集", this.m_Task.IsErrorLog));

                return cGlobalParas.GatherResult.GatherSucceed;
            }

            //Pause();

            //在此处理用户选择了排重库的问题
            if (this.m_Task.IsUrlNoneRepeat == true)
            {
                if (!this.m_Urls.Add(ref Url, false))
                {
                    e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "此任务设置了排重，网址：" + Url + "已经采集，无需再采", this.m_Task.IsErrorLog));
                    return cGlobalParas.GatherResult.GatherSucceed;
                }
            }

            //在此处理全局排重，全局排重需要进行加锁，仅限于采集引擎使用
            if (this.m_isGlobalRepeat == true)
            {
                lock (m_RepeatUrlsLock)
                {
                    if (!this.g_Urls.Add(ref Url, false))
                    {
                        e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "此任务设置了排重，网址：" + Url + "已经采集，无需再采", this.m_Task.IsErrorLog));
                        return cGlobalParas.GatherResult.GatherSucceed;
                    }
                }
            }


            string htmlSource = string.Empty;
            eResponse response = null;

            DataTable tmpData = null;
            DataTable tmpMergeData;

            //记录自动翻页如果相加的原始值
            int FirstNextIndex = 0;

            string NextUrl = Url;
            string Old_Url = NextUrl;

            try
            {
                if (IsNext)
                {
                    #region 如果存在下一页规则，则开始进行下一页规则的解析和数据采集

                    #region 在此先对翻页规则中的数据参数进行替换
                    if (Regex.IsMatch(NextRule, @"{Num:\d+,.+/.+,\d+}"))
                    {

                        //开始替换相关的参数
                        string strMatch = @"{Num:\d+,.+/.+,\d+}";
                        Match s = Regex.Match(NextRule, strMatch, RegexOptions.IgnoreCase);
                        string strNaviNum = s.Groups[0].Value;
                        string str = strNaviNum.Replace("{Num:", "");
                        str = str.Substring(0, str.Length - 1);
                        string[] ss = str.Split(',');
                        str = ss[1];

                        //计算数字吧
                        string[] ss1 = new string[2];
                        if (str.IndexOf("{") > -1)
                        {
                            ss1[0] = str.Substring(0, str.IndexOf("}") + 1);
                            ss1[1] = str.Replace(ss1[0], "");
                            ss1[1] = ss1[1].Substring(1, ss1[1].Length - 1);
                        }
                        else
                        {
                            ss1 = str.Split('/');
                        }

                        //处理最大值
                        if (ss1[0].StartsWith("{UrlValue:"))
                        {
                            //不处理，由导航网址分解的时候处理
                        }
                        else if (ss1[0].StartsWith("{FromValue:"))
                        {
                            //不处理，由导航网址分解的时候处理
                        }
                        else if (ss1[0].StartsWith("{") && !ss1[0].StartsWith("{FromValue:") && !ss1[0].StartsWith("{UrlValue:"))
                        {
                            //替换具体的数值

                            string maxValueName = ss1[0].Replace("{", "").Replace("}", "");
                            string maxValue = string.Empty;
                            if (tRow != null)
                            {
                                maxValue = tRow[maxValueName].ToString();
                            }
                            else
                                maxValue = "0";
                            ss1[0] = maxValue;
                        }

                        //处理平均值
                        if (ss1[1].StartsWith("{UrlValue:"))
                        {
                            //不处理，由导航网址分解的时候处理
                        }
                        else if (ss1[1].StartsWith("{FromValue:"))
                        {
                            //不处理，由导航网址分解的时候处理
                        }
                        else if (ss1[0].StartsWith("{") && !ss1[0].StartsWith("{FromValue:") && !ss1[0].StartsWith("{UrlValue:"))
                        {
                            //替换具体的数值

                            string maxValueName = ss1[1].Replace("{", "").Replace("}", "");
                            string maxValue = string.Empty;
                            if (tRow != null)
                            {
                                maxValue = tRow[maxValueName].ToString();
                            }
                            else
                                maxValue = "0";
                            ss1[1] = maxValue;
                        }

                        //重新合成导航规则
                        NextRule = NextRule.Replace(strNaviNum, "{Num:" + ss[0] + "," + ss1[0] + "/" + ss1[1] + "," + ss[2] + "}");
                    }
                    #endregion

                    tmpMergeData = new DataTable();

                    //定义一个值，确定自动翻页不能翻过最大页面
                    int curNextPage = 0;

                    //定义一个hashtable记录翻页的地址，并进行排重处理，这样
                    //做的目的是为了防止翻页进入循环翻页状态，即，从最后一页
                    //又翻到了第一页
                    Hashtable nextUrls = new Hashtable();
                    nextUrls.Add(nextUrls, nextUrls);

                    do
                    {
                        if (m_ThreadRunning == true)
                        {
                            Url = NextUrl;
                            Old_Url = NextUrl;

                            e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "设置了翻页规则，开始翻页采集：" + Url, this.m_Task.IsErrorLog));

                            tmpData = GetGatherData(Url, Level, ref m_tmpCookie,null, response, true, referUrl, tRow, "", 0, false);

                            ///每次采集完成数据后，都需要判断是否进行数据合并
                            if ( m_IsMergeData == true)
                            {
                                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "此页数据需要进行合并，等待下一页数据采集", this.m_Task.IsErrorLog));
                                if (tmpData != null)
                                    tmpMergeData.Merge(tmpData);
                            }
                            else
                            {

                                //触发日志及采集数据的事件
                                if (tmpData == null || tmpData.Rows.Count == 0)
                                {

                                }
                                else
                                {
                                    //合并上层传递来的数据
                                    if (tRow != null && tmpData != null)
                                    {
                                        //合并数据导航数据
                                        tmpData = MergeDataTable(tRow, tmpData);
                                    }

                                    //在此处理直接入库的问题
                                    if (this.m_Task.RunType == cGlobalParas.TaskRunType.OnlySave && tmpData != null && Level == 0)
                                    {
                                        NewTable(tmpData.Columns);
                                        InsertData(tmpData);
                                    }

                                    #region 在此处理插件数据发布的规则
                                    if (this.m_Task.IsPluginsPublish == true && tmpData != null && Level == 0)
                                    {
                                        if (ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Cloud ||
                                            ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Program ||
                                            ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Enterprise ||
                                            ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Ultimate ||
                                            ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Server ||
                                            ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.DistriServer)
                                        {
                                            NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
                                            if (this.m_Task.PluginsPublish == "")
                                            {
                                                if (e_Log != null)
                                                {
                                                    e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning, "配置了发布插件，却没有提供插件地址！", this.m_Task.IsErrorLog));
                                                }
                                            }
                                            else
                                            {
                                                rPlugin.CallPublishData(tmpData, this.m_Task.PluginsPublish);
                                            }
                                            rPlugin = null;
                                        }
                                        else
                                        {
                                            if (e_Log != null)
                                            {
                                                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning, "当前版本不支持插件功能，请获取正确版本！", this.m_Task.IsErrorLog));
                                            }
                                        }
                                    }
                                    #endregion

                                    e_GData(this, new cGatherDataEventArgs(this.m_Task.TaskID, this.m_Task.TaskName, tmpData, (this.m_Task.ExportType == cGlobalParas.PublishType.NoPublish ? false : true)));
                                }
                            }

                            e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "采集完成：" + Url, this.m_Task.IsErrorLog));

                            cUrlAnalyze cu = new cUrlAnalyze(this.m_workPath, ref m_ProxyControl, this.m_Task.IsProxy, this.m_Task.IsProxyFirst, this.pType, this.ProxyAddress, this.ProxyPort);
                            response = GetHtml(Url, referUrl, NextRule.Contains("<XPath>"));
                            htmlSource = response==null ?"": response.Body;
                            string strNext = cu.GetNextUrl(Url, htmlSource, NextRule, RegexNextPage, 
                                ref FirstNextIndex, Level);

                            //翻页后更新referurl，为当前页，翻页是一页一页的翻啊
                            referUrl = Url;

                            cu = null;

                            e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "下一页网址获取成功：" + NextUrl, this.m_Task.IsErrorLog));

                            NextUrl = strNext;

                            //更新地址，因为是自动翻页，所以采集任务无法感知翻页到了那里，所以需要
                            //在此进行更新
                            m_TaskSplitData.Weblink[index].NextPageUrl = NextUrl;

                            //翻页成功
                            curNextPage++;
                            if (NextMaxPage != "0" && curNextPage >= int.Parse(NextMaxPage))
                                break;

                        }
                        else if (m_ThreadRunning == false)
                        {
                            //标识要求终止线程，停止任务，退出do循环提前结束任务
                            if (NextUrl == "" || Old_Url == NextUrl)
                            {
                                return cGlobalParas.GatherResult.GatherSucceed;
                            }
                            else
                            {
                                return cGlobalParas.GatherResult.GatherStoppedByUser;
                            }
                        }

                        if (NextUrl != "")
                        {
                            if (nextUrls.ContainsKey(NextUrl))
                            {
                                break;
                            }

                            nextUrls.Add(NextUrl, NextUrl);
                        }
                    }
                    while (NextUrl != "" && Old_Url != NextUrl && NextUrl != "#");

                    nextUrls = null;

                    e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "已经到最终页：" + NextUrl, this.m_Task.IsErrorLog));


                    //判断是否为合并数据，如果是，则将合并数据向上返回
                    if (m_IsMergeData == true)
                    {
                        e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "开始进行数据合并", this.m_Task.IsErrorLog));
                        tmpMergeData = MergeData(tmpMergeData);


                        if (tRow != null && tmpMergeData != null)
                        {
                            //合并数据导航数据
                            e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "合并上一层采集数据", this.m_Task.IsErrorLog));

                            tmpMergeData = MergeDataTable(tRow, tmpMergeData);
                        }

                        //在此处理直接入库的问题
                        if (this.m_Task.RunType == cGlobalParas.TaskRunType.OnlySave && tmpMergeData != null && Level == 0)
                        {
                            NewTable(tmpMergeData.Columns);
                            InsertData(tmpMergeData);
                        }

                        #region 在此处理插件数据发布的规则
                        if (this.m_Task.IsPluginsPublish == true && tmpMergeData != null && Level == 0)
                        {
                            if (ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Cloud ||
                                ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Program ||
                                ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Enterprise ||
                                ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Ultimate ||
                                ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Server ||
                                ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.DistriServer)
                            {
                                NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
                                if (this.m_Task.PluginsPublish == "")
                                {
                                    if (e_Log != null)
                                    {
                                        e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning, "配置了发布插件，却没有提供插件地址！", this.m_Task.IsErrorLog));
                                    }
                                }
                                else
                                {
                                    rPlugin.CallPublishData(tmpMergeData, this.m_Task.PluginsPublish);
                                }
                                rPlugin = null;
                            }
                            else
                            {
                                if (e_Log != null)
                                {
                                    e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning, "当前版本不支持插件功能，请获取正确版本！", this.m_Task.IsErrorLog));
                                }
                            }
                        }
                        #endregion

                        e_GData(this, new cGatherDataEventArgs(this.m_Task.TaskID, this.m_Task.TaskName, tmpMergeData, (this.m_Task.ExportType == cGlobalParas.PublishType.NoPublish ? false : true)));
                        e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "数据合并成功", this.m_Task.IsErrorLog));

                        tmpMergeData.Dispose();
                        tmpMergeData = null;
                    }

                    #endregion

                }
                else
                {


                    try
                    {
                        tmpData = GetGatherData(Url, Level, ref m_tmpCookie, null, response, true, referUrl, tRow, loopFlag, loopIndex, false);
                    }
                    catch (NetMinerSkipUrlException ex)
                    {
                        throw ex;
                    }
                    catch (System.Exception ex)
                    {
                        throw new Exception("采集数据错误" + ex.Message);
                    }

                    //触发日志及采集数据的事件
                    if (tmpData == null || tmpData.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        try
                        {
                            if (tRow != null && tmpData != null)
                            {
                                //合并数据导航数据
                                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "合并上一层采集数据", this.m_Task.IsErrorLog));

                                tmpData = MergeDataTable(tRow, tmpData);
                            }

                            //在此处理直接入库的问题
                            if (this.m_Task.RunType == cGlobalParas.TaskRunType.OnlySave && tmpData != null && Level == 0)
                            {

                                bool isS = NewTable(tmpData.Columns);
                                if (isS == true)
                                {
                                    InsertData(tmpData);
                                }
                                else
                                {
                                    throw new NetMinerException("数据表创建失败导致数据无法插入到数据库，请检查数据库链接及数据配置！");
                                }
                            }

                            #region 在此处理插件数据发布的规则
                            if (this.m_Task.IsPluginsPublish == true && tmpData != null && Level == 0)
                            {
                                if (ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Cloud ||
                                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Program ||
                                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Enterprise ||
                                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Ultimate ||
                                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Server ||
                                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.DistriServer)
                                {
                                    NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
                                    if (this.m_Task.PluginsPublish == "")
                                    {
                                        if (e_Log != null)
                                        {
                                            e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning, "配置了发布插件，却没有提供插件地址！", this.m_Task.IsErrorLog));
                                        }
                                    }
                                    else
                                    {
                                        rPlugin.CallPublishData(tmpData, this.m_Task.PluginsPublish);
                                    }
                                    rPlugin = null;
                                }
                                else
                                {
                                    if (e_Log != null)
                                    {
                                        e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning, "当前版本不支持插件功能，请获取正确版本！", this.m_Task.IsErrorLog));
                                    }
                                }
                            }
                            #endregion

                            e_GData(this, new cGatherDataEventArgs(this.m_Task.TaskID, 
                                this.m_Task.TaskName, tmpData, 
                                (this.m_Task.ExportType == cGlobalParas.PublishType.NoPublish ? false : true)));

                            //tmpData = null;
                            tmpData.Dispose();
                        }
                        catch (System.Exception ex)
                        {
                            throw ex;
                        }
                    }
                    e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "采集完成：" + Url, this.m_Task.IsErrorLog));
                }


            }
            catch (NetMinerSkipUrlException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                if (this.m_Task.IsUrlNoneRepeat == true && this.m_Task.IsSucceedUrlRepeat == true)
                {
                    //删除此排重信息
                    m_Urls.Del(hUrl, false);
                }

                if (ex.Message.Contains("0x81000100"))
                    throw ex;

                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Error, Url + "采集发生错误：" + ex.Message, this.m_Task.IsErrorLog));

                //onError(ex);
                if (e_Error != null)
                    e_Error(this, new TaskThreadErrorEventArgs(ex));
                return cGlobalParas.GatherResult.GatherFailed;
            }

            //gWeb = null;
            tmpData = null;

            return cGlobalParas.GatherResult.GatherSucceed;

        }

        ///这是采集带有导航规则的网址数据的入口
        ///导航规则分为两类：一是下一页的导航规则；而是页面导航，
        ///此方法传入地址后，主要处理下一页的规则，然后调用ParseGatherNavigationUrl
        ///处理页面导航的问题
        /// 修订此方法：2010-12-11  处理导航翻页的规则
        ///修改入口规则 只要进入导航网址解析，其翻页规则必然包括再导航规则中，不会单独进行设置
        ///
        internal cGlobalParas.GatherResult GatherNavigationUrl(string Url, List<eNavigRule> nRules, int index,
            bool isMulti121, List<eMultiPageRule> listMultiRules, string referUrl)
        {

            cGlobalParas.GatherResult IsSucceed = cGlobalParas.GatherResult.UnGather;

            int FirstNextIndex = 0;

            #region 注销代码
            ////在此处理是否需要进行Base64编码的的问题
            //if (Regex.IsMatch(Url, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
            //{

            //    Match s = Regex.Match(Url, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
            //    string sBase64 = s.Groups[0].Value.ToString();
            //    sBase64 = ToolUtil.Base64Encoding(sBase64);

            //    //将base64编码部分进行url替换
            //    Url = Regex.Replace(Url, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            //}
            #endregion

            string NextUrl = Url;
            string Old_Url = NextUrl;

            try
            {
                //导航进入的肯定是第一层，所以在此处理第一层的翻页规则
                if (nRules[0].IsNext)
                {
                    #region 处理导航 且具备下一页自动翻页的采集

                    //入口地址，不对自动计算翻页的公示中的数据参数进行替换
                    //因为，在此还没有采集到相关的数据，但可以通过从地址中获取或者是表单
                    int curNextPage = 0;

                    do
                    {
                        if (m_ThreadRunning == true)
                        {
                            Url = NextUrl;
                            Old_Url = NextUrl;

                            IsSucceed = ParseGatherNavigationUrl(Url, 1, nRules, index, null, isMulti121,
                                listMultiRules, referUrl); //, NagRule, IsOppPath);

                            e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "采集完成：" + Url, this.m_Task.IsErrorLog));

                            //开始分解下一页网址
                            int ANum = 0;
                            string webSource = string.Empty;
                            eResponse response = null;

                        ANumber:
                            try
                            {

                                ANum++;



                                //webSource = gWeb.GetHtml(Url, m_WebCode, m_IsUrlEncode, m_IsTwoUrlCode, m_UrlEncode, ref  m_tmpCookie, "", "",
                                //    true, this.m_IsAutoUpdateHeader, referUrl,this.isGatherCoding,this.GatherCodingFlag,
                                //    this.CodeUrl ,this.GatherCodingPlugin);


                                bool isVisual = false;
                                if (nRules[0].NextRule.Contains("<XPath>"))
                                    isVisual = true;
                                response = GetHtml(Url, referUrl, isVisual);
                                webSource = response==null ?"": response.Body;

                            }
                            catch (System.Exception ex)
                            {
                                if (ANum > 3)
                                    throw ex;
                                else
                                    goto ANumber;
                            }

                            cUrlAnalyze cu = new cUrlAnalyze(this.m_workPath, ref this.m_ProxyControl, this.m_Task.IsProxy, this.m_Task.IsProxyFirst, this.pType, this.ProxyAddress, this.ProxyPort);
                            string strNext = cu.GetNextUrl(Url, webSource, nRules[0].NextRule, RegexNextPage, 
                                ref FirstNextIndex,  1);

                            referUrl = Url;

                            cu = null;

                            e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "下一页网址获取成功：" + NextUrl, this.m_Task.IsErrorLog));

                            NextUrl = strNext;

                            //更新地址，因为是自动翻页，所以采集任务无法感知翻页到了那里，所以需要
                            //在此进行更新
                            m_TaskSplitData.Weblink[index].NextPageUrl = NextUrl;

                            //翻页成功
                            curNextPage++;
                            if (nRules[0].NextMaxPage != "0" && curNextPage >= int.Parse(nRules[0].NextMaxPage))
                                break;

                        }
                        else if (m_ThreadRunning == false)
                        {
                            //标识要求终止线程，停止任务，退出do循环提前结束任务
                            if (NextUrl == "" || Old_Url == NextUrl)
                            {
                                return cGlobalParas.GatherResult.GatherSucceed;
                            }
                            else
                            {
                                return cGlobalParas.GatherResult.GatherStoppedByUser;
                            }
                            //break;
                        }

                    }
                    while (NextUrl != "" && Old_Url != NextUrl && NextUrl != "#");

                    e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "已经到最终页", this.m_Task.IsErrorLog));

                    #endregion
                }
                else
                {

                    IsSucceed = ParseGatherNavigationUrl(Url, 1, nRules, index, null,
                        isMulti121, listMultiRules, referUrl); //, NagRule, IsOppPath);
                }
            }
            catch (NetMinerSkipUrlException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Error, Url + "采集发生错误：" + ex.Message, this.m_Task.IsErrorLog));
                //e_GUrlCount(this, new cGatherUrlCountArgs(this.m_Task.TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                //e_GUrlCount(this, new cGatherUrlCountArgs(this.m_Task.TaskID, cGlobalParas.UpdateUrlCountType.ErrUrlCountAdd, 0));
                //m_TaskSplitData.GatheredTrueErrUrlCount++;
                //m_TaskSplitData.GatheredErrUrlCount++;
                //onError(ex);
                if (e_Error != null)
                    e_Error(this, new TaskThreadErrorEventArgs(ex));
                return cGlobalParas.GatherResult.GatherFailed;
            }

            //gWeb = null;

            //return cGlobalParas.GatherResult.GatherSucceed;
            return IsSucceed;
        }

        ///用于采集需要导航的网页，在此处理导航页规则，在2009-10-28日，增加了导航处理自动翻页的情况
        ///所以，在此需要进行递归调用，原有方式是将Url和导航规则（可能是多级）通过UrlAnalysis分析后
        ///得出所有的网址，但此种算法需要修改，即分级进行导航，将网址提取之后再进行分页处理，如果一个网址
        ///两种规则都处理结束后，再进行下一个网址的处理。
        private cGlobalParas.GatherResult ParseGatherNavigationUrl(string Url, int level, List<eNavigRule> nRules,
            int index, DataRow tRow, bool isMulti121, List<eMultiPageRule> listMultiRules, string referUrl)
        {
            string iUrl = Url;
            int FirstNextIndex = 0;

            cGlobalParas.GatherResult isSucceed = cGlobalParas.GatherResult.UnGather;

            //因为这是一个递归调用的函数，所以，首先需要判断当前是否已经要求停止线程工作
            //如果停止线程工作，则停止调用，开始返回
            if (m_ThreadRunning == false)
                return cGlobalParas.GatherResult.GatherStoppedByUser;

            

            cUrlAnalyze u = new cUrlAnalyze(this.m_workPath, ref this.m_ProxyControl, this.m_Task.IsProxy, 
                this.m_Task.IsProxyFirst, this.pType, this.ProxyAddress, this.ProxyPort);

            List<string> gUrls;                                          //记录导航返回的Url列表
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

                    tmpNRules.Add(tmpNRule);
                    break;
                }
            }

            #endregion

            bool isMySelfLoop = false;
            string strLoopFlag = string.Empty;

            #region 判断当前的导航级别是否有循环标记
            string MySelfRule = ToolUtil.CutRegexWildcard(tmpNRule.NavigRule);

            if (MySelfRule.StartsWith("<MySelf>"))
            {
                Match strMatchLoop = Regex.Match(MySelfRule, "(?<=<MySelf>).*?(?=</MySelf>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strLoopFlag = strMatchLoop.Groups[0].ToString();
                if (!string.IsNullOrEmpty(strLoopFlag))
                    isMySelfLoop = true;
            }
            #endregion

            e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "导航层级为：" + nRules.Count + " 层，正在进行第" + level + "层导航 网址：" + Url, this.m_Task.IsErrorLog));

            //判断是否为内容页，如果是内容页，则开始进行内容页的采集
            //同时如果不是内容页，且具备采集要求的导航页，需要首先先把此导航页的采集数据
            //采集出来，然后作为参数进行递归传递，一直到采集页进行最终的数据合并

            bool isVisual = false;

            string htmlSource = string.Empty;
            eResponse response = null;

            #region 进行导航页数据采集操作，获取一个datatable出来

            DataTable tData = new DataTable();

            //判断此导航页是否需要进行数据采集
            if (tmpNRule.IsGather == true)
            {

                tData = new DataTable();

                tData = GetGatherData(Url, level,ref m_tmpCookie, null, null, true, Url, tRow, "", 0, false);

                if (tRow != null && tData != null)
                {
                    //合并数据导航数据
                    tData = MergeDataTable(tRow, tData);
                }

            }

            #endregion

            #region 进行多页采集，并和导航采集的数据进行合并，但导航页必须采集，且必须有数据
            if (listMultiRules != null && tData != null && tData.Rows.Count > 0)
            {
                for (int mIndex = 0; mIndex < listMultiRules.Count; mIndex++)
                {
                    if (level == listMultiRules[mIndex].mLevel)
                    {
                        #region 这里是采集多页数据用的

                        //采集多页的数据
                        eMultiPage cM = new eMultiPage();
                        string UrlRule = listMultiRules[mIndex].Rule;


                        //先替换时间戳参数
                        if (Regex.IsMatch(UrlRule, "{Timestamp:[\\d]*?}"))
                        {
                            UrlRule = Regex.Replace(UrlRule, "{Timestamp:[\\d]*?}", ToolUtil.GetTimestamp().ToString());
                        }

                        #region 处理多页规则
                        if (UrlRule.StartsWith("<ParaUrl>"))
                        {
                            Match charSetMatch1 = Regex.Match(UrlRule, "(?<=<ParaUrl>).*?(?=</ParaUrl>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            string mUrl = charSetMatch1.Groups[0].ToString();

                            //替换参数
                            //while (mUrl.IndexOf("{") > 0)
                            while (Regex.IsMatch(mUrl, "[^\\\\]{.*[^\\\\]}"))
                            {

                                string strMatch = "(?<={)[^{]*[^\\\\](?=})";
                                Match s = Regex.Match(mUrl, strMatch, RegexOptions.IgnoreCase);
                                string p = s.Groups[0].Value;

                                //与导航页的数据合并强制为1对1
                                string pValue = tData.Rows[0][p].ToString();
                                mUrl = mUrl.Replace("{" + p + "}", pValue); // strPre + pValue + strSuf;
                            }

                            cM.Url = mUrl;

                            for (int j = 0; j < m_TaskSplitData.CutFlag.Count; j++)
                            {
                                if (m_TaskSplitData.CutFlag[j].MultiPageName == listMultiRules[mIndex].RuleName)
                                    cM.WebPageCutFlags.Add(m_TaskSplitData.CutFlag[j]);

                            }

                        }
                        else
                        {
                            //根据多页采集的规则获取多页的页面，注意，有可能通过配置的多页规则会提取出多个
                            //网址，如果是多个网址，则去第一个Url即可
                            cUrlAnalyze cu = new cUrlAnalyze(this.m_workPath, ref this.m_ProxyControl, this.m_Task.IsProxy, this.m_Task.IsProxyFirst, this.pType, this.ProxyAddress, this.ProxyPort);

                            response=GetHtml(Url, referUrl,false);   //多页不存在可视化的问题
                            htmlSource = response.Body;
                            List<string> mUrls = cu.GetUrlsByRule(Url, htmlSource,  UrlRule,level,cGlobalParas.NaviRunRule.Normal, "",this.m_tmpCookie,this.m_Task.Headers);
                            cu = null;

                            if (mUrls != null && mUrls.Count > 0)
                            {
                                cM.Url = mUrls[0];

                                for (int j = 0; j < m_TaskSplitData.CutFlag.Count; j++)
                                {
                                    if (m_TaskSplitData.CutFlag[j].MultiPageName == listMultiRules[mIndex].RuleName)
                                        cM.WebPageCutFlags.Add(m_TaskSplitData.CutFlag[j]);

                                }

                            }
                        }
                        #endregion

                        if (cM.Url == null)
                        {
                            e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning,
                                "解析多页规则：" + listMultiRules[mIndex].RuleName + "出错，未能获取到多页地址。", this.m_Task.IsErrorLog));

                            continue;
                        }

                        #region 开始采集多页并进行数据合并
                        //开始采集多页的数据
                        m_TaskSplitData.CurUrl = cM.Url;

                        DataTable mtmpData;
                        response = GetHtml(Url, referUrl,false);
                        string hSource = response.Body;
                        mtmpData = GetGatherData(cM.Url, level,ref m_tmpCookie, cM.WebPageCutFlags, response, false, Url, tRow, "", 0, false);

                        #endregion

                        if (mtmpData != null)
                            //开始与多页采集的数据进行合并
                            tData = MergeTable(tData, mtmpData);

                        #endregion
                    }
                }
            }
            #endregion

            #region 在此对导航规则及翻页规则中的最大页码计算公式中的参数进行替换
            if (tmpNRules.Count > 0 && Regex.IsMatch(tmpNRules[0].NavigRule, @"{Num:\d+,[^}]+/.+,\d+}"))
            {
                string strnaviRule = tmpNRules[0].NavigRule;
                string strnextRule = tmpNRules[0].NextRule;

                //开始替换相关的参数
                string strMatch = @"{Num:\d+,[^}]+/.+,\d+}";
                Match s = Regex.Match(strnaviRule, strMatch, RegexOptions.IgnoreCase);
                string strNaviNum = s.Groups[0].Value;
                string str = strNaviNum.Replace("{Num:", "");
                str = str.Substring(0, str.Length - 1);
                string[] ss = str.Split(',');
                str = ss[1];

                //计算数字吧
                string[] ss1 = new string[2];
                if (str.IndexOf("{") > -1)
                {
                    ss1[0] = str.Substring(0, str.IndexOf("}") + 1);
                    ss1[1] = str.Replace(ss1[0], "");
                    ss1[1] = ss1[1].Substring(1, ss1[1].Length - 1);
                }
                else
                {
                    ss1 = str.Split('/');
                }
                //处理最大值
                if (ss1[0].StartsWith("{UrlValue:"))
                {
                    //不处理，由导航网址分解的时候处理
                }
                else if (ss1[0].StartsWith("{FromValue:"))
                {
                    //不处理，由导航网址分解的时候处理
                }
                else if (ss1[0].StartsWith("{") && !ss1[0].StartsWith("{FromValue:") && !ss1[0].StartsWith("{UrlValue:"))
                {
                    //替换具体的数值

                    string maxValueName = ss1[0].Replace("{", "").Replace("}", "");
                    string maxValue = string.Empty;
                    if (tData != null && tData.Rows.Count > 0)
                    {
                        maxValue = tData.Rows[0][maxValueName].ToString();
                    }
                    else
                        maxValue = "0";
                    ss1[0] = maxValue;
                }

                //处理平均值
                if (ss1[1].StartsWith("{UrlValue:"))
                {
                    //不处理，由导航网址分解的时候处理
                }
                else if (ss1[1].StartsWith("{FromValue:"))
                {
                    //不处理，由导航网址分解的时候处理
                }
                else if (ss1[0].StartsWith("{") && !ss1[0].StartsWith("{FromValue:") && !ss1[0].StartsWith("{UrlValue:"))
                {
                    //替换具体的数值

                    string maxValueName = ss1[1].Replace("{", "").Replace("}", "");
                    string maxValue = string.Empty;
                    if (tData != null && tData.Rows.Count > 0)
                    {
                        maxValue = tData.Rows[0][maxValueName].ToString();
                    }
                    else
                        maxValue = "0";
                    ss1[1] = maxValue;
                }

                //重新合成导航规则
                strnaviRule = strnaviRule.Replace(strNaviNum, "{Num:" + ss[0] + "," + ss1[0] + "/" + ss1[1] + "," + ss[2] + "}");
                strnextRule = strnextRule.Replace(strNaviNum, "{Num:" + ss[0] + "," + ss1[0] + "/" + ss1[1] + "," + ss[2] + "}");
                tmpNRules[0].NavigRule = strnaviRule;
                tmpNRules[0].NextRule = strnextRule;
            }
            #endregion

            #region  根据导航规则找到网址
            List<string> gTempUrls = null;
            try
            {
                bool isV= false;
                if (tmpNRules[0].NavigRule.Contains("<XPath>"))
                    isV = true;
                response= GetHtml(Url, referUrl, isV);
                htmlSource = response.Body;
                gTempUrls = u.ParseUrlRule(Url, htmlSource, tmpNRules,this.m_Task.Headers);
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Error, Url + ":" + ex.Message, this.m_Task.IsErrorLog));
                return cGlobalParas.GatherResult.GatherFailed;
            }
            #endregion

            if (gTempUrls == null)
            {
                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, Url + " 导航解析失败，有可能是由于导航规则配置错误，也有可能是由于垃圾数据造成，如果是垃圾数据，则不影响系统对数据的采集", this.m_Task.IsErrorLog));
                return cGlobalParas.GatherResult.GatherFailed;
            }
            if (gTempUrls.Count == 0)
            {

            }
            else
            {
                ////在此处理Url编码的问题
                //if (m_IsUrlEncode == true)
                //{
                //    for (int i = 0; i < gTempUrls.Count; i++)
                //    {
                //        gTempUrls[i] = cTool.UrlEncode(gTempUrls[i], (cGlobalParas.WebCode)int.Parse(m_UrlEncode));
                //    }
                //}
            }

            try
            {
                gUrls = new List<string>();
                gUrls = gTempUrls;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            u = null;

            e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "成功根据导航规则获取" + gUrls.Count + "个网址", this.m_Task.IsErrorLog));

            //cGatherWeb gWeb = new cGatherWeb(this.m_workPath);

            if (level == nRules.Count)
            {
                //如果是最后一层，则为内容页，直接采集即可，但在此需要将内容页的翻页规则
                //传递过去。
                e_GUrlCount(this, new cGatherUrlCountArgs(this.m_Task.TaskID, cGlobalParas.UpdateUrlCountType.ReIni, gUrls.Count));

                //根据导航的网址修改导航网址采集数量
                m_TaskSplitData.UrlNaviCount += gUrls.Count;

                isSucceed = GatherParsedUrl(gUrls, level, tmpNRule.IsNaviNextPage, tmpNRule.NaviNextPage,
                    tmpNRule.NaviNextMaxPage, index, tData, isMulti121, listMultiRules, Url, isMySelfLoop, strLoopFlag);
            }
            else
            {
                //如果不是内容页，则表示为多级导航，在此需要处理导航级别的翻页规则
                //同时需要根据上层导航出的网址进行逐个采集
                #region
                for (int j = 0; j < gUrls.Count; j++)
                {
                    #region 分级采集 数据合并处理，提取导航页数据，并进行传递

                    DataRow dr;
                    if (tData != null && tData.Rows.Count != 0)
                    {
                        if (j >= tData.Rows.Count)
                        {
                            dr = tData.Rows[tData.Rows.Count - 1];
                        }
                        else
                        {
                            dr = tData.Rows[j];
                        }
                    }
                    else
                    {
                        dr = null;
                    }

                    #endregion

                    //循环调用，需判断线程的终止状态
                    //在此处理自动翻页的规则

                    if (tmpNRule.IsNaviNextPage)
                    {
                        #region 在此处理分页导航的规则

                        string NextUrl = gUrls[j].ToString();
                        string Old_Url = NextUrl;

                        int curNextPage = 0;
                        referUrl = iUrl;

                        do
                        {

                            if (m_ThreadRunning == true)
                            {
                                Url = NextUrl;
                                Old_Url = NextUrl;

                                isSucceed = ParseGatherNavigationUrl(Url, level + 1, nRules, index, dr,
                                    isMulti121, listMultiRules, referUrl);

                                //e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID,cGlobalParas.LogType.Info, "开始根据下一页规则获取下一页网址", this.IsErrorLog));

                                //bool IsAjax = false;

                                //if (m_TaskType == cGlobalParas.TaskType.AjaxHtmlByUrl)
                                //    IsAjax = true;

                                cUrlAnalyze cu = new cUrlAnalyze(this.m_workPath, ref this.m_ProxyControl, this.m_Task.IsProxy, this.m_Task.IsProxyFirst, this.pType, this.ProxyAddress, this.ProxyPort);

                                bool isV = false;
                                if (tmpNRule.NaviNextPage.Contains("<XPath>"))
                                    isV = true;
                                response = GetHtml(Url, referUrl, isV);
                                string hSource = response.Body;
                                string strNext = cu.GetNextUrl(Url, hSource, tmpNRule.NaviNextPage, RegexNextPage, 
                                     ref FirstNextIndex,  level);

                                referUrl = Url;

                                cu = null;

                                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "下一页网址获取成功：" + strNext, this.m_Task.IsErrorLog));

                                NextUrl = strNext;

                                //翻页成功
                                curNextPage++;
                                if (tmpNRule.NaviNextMaxPage != "0" && curNextPage >= int.Parse(tmpNRule.NaviNextMaxPage))
                                    break;

                            }
                            else if (m_ThreadRunning == false)
                            {
                                return cGlobalParas.GatherResult.GatherStoppedByUser;
                            }

                        }
                        while (NextUrl != "" && Old_Url != NextUrl && NextUrl != "#");
                        e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Info, "已经到最终页", this.m_Task.IsErrorLog));
                        isSucceed = cGlobalParas.GatherResult.GatherSucceed;
                        #endregion
                    }
                    else
                    {
                        if (m_ThreadRunning == true)
                        {
                            try
                            {
                                isSucceed = ParseGatherNavigationUrl(gUrls[j].ToString(), level + 1, nRules, index,
                                    dr, isMulti121, listMultiRules, Url);
                            }
                            catch (System.Exception ex)
                            {
                                //onError(ex);
                                if (e_Error != null)
                                    e_Error(this, new TaskThreadErrorEventArgs(ex));
                            }
                        }
                        else if (m_ThreadRunning == false)
                        {
                            return cGlobalParas.GatherResult.GatherStoppedByUser;
                        }
                    }
                    dr = null;
                }
                #endregion
            }

            tData = null;

            gUrls = null;
            gTempUrls = null;

            tmpNRule = null;
            tmpNRules = null;

            if (isSucceed == cGlobalParas.GatherResult.UnGather)
                return cGlobalParas.GatherResult.GatherSucceed;
            else
                return isSucceed;
        }

        /// <summary>
        /// 用于采集导航网页分解后的集合网址，即最终的内容页
        /// </summary>
        /// <param name="gUrls"></param>
        /// <param name="Level"></param>
        /// <param name="IsNext"></param>
        /// <param name="NextPageRule"></param>
        /// <param name="IsDoPostBack"></param>
        /// <param name="naviNextMaxPage"></param>
        /// <param name="index"></param>
        /// <param name="tmpData">导航页采集的数据</param>
        /// <param name="isMulti121"></param>
        /// <param name="listMultiRules"></param>
        /// <param name="referUrl"></param>
        /// <returns></returns>
        internal cGlobalParas.GatherResult GatherParsedUrl(List<string> gUrls, int Level, bool IsNext, string NextPageRule,
            string naviNextMaxPage,
            int index, DataTable tmpData, bool isMulti121, List<eMultiPageRule> listMultiRules, string referUrl,
            bool isMySelfLoop, string strLoopFlag)
        {
            cGlobalParas.GatherResult IsSucceed = cGlobalParas.GatherResult.UnGather;


            for (int j = 0; j < gUrls.Count; j++)
            {
                if (m_ThreadRunning == true)
                {
                    //Pause();

                    try
                    {
                        DataRow dr;
                        if (tmpData != null && tmpData.Rows.Count != 0)
                        {
                            if (j >= tmpData.Rows.Count)
                            {
                                dr = tmpData.Rows[tmpData.Rows.Count - 1];
                            }
                            else
                            {
                                dr = tmpData.Rows[j];
                            }
                        }
                        else
                        {
                            dr = null;
                        }

                        if (listMultiRules != null)
                        {
                            //既然是最终页采集，那代表采集规则的导航层级别必然为0
                            //多页的采集
                            IsSucceed = GatherSingleUrl(gUrls[j].ToString(), 0, IsNext, NextPageRule, index, dr, isMulti121, listMultiRules, referUrl);
                        }
                        else
                        {
                            IsSucceed = GatherSingleUrl(gUrls[j].ToString(), 0, IsNext, NextPageRule, naviNextMaxPage,
                                 index, dr, referUrl, strLoopFlag, j);
                        }

                        //触发采集网址计数事件
                        //e_GUrlCount(this, new cGatherUrlCountArgs(this.m_Task.TaskID, cGlobalParas.UpdateUrlCountType.Gathered, 0));
                        //m_TaskSplitData.GatheredTrueUrlCount++;
                        if (IsSucceed == cGlobalParas.GatherResult.GatherSucceed || IsSucceed == cGlobalParas.GatherResult.GatherStoppedByUser)
                        {
                            m_TaskSplitData.GatheredUrlNaviCount++;
                            e_GUrlCount(this, new cGatherUrlCountArgs(this.m_Task.TaskID, cGlobalParas.UpdateUrlCountType.NaviGathered, 0));

                        }
                        else if (IsSucceed == cGlobalParas.GatherResult.GatherFailed)
                        {
                            m_TaskSplitData.GatheredErrUrlNaviCount++;
                            e_GUrlCount(this, new cGatherUrlCountArgs(this.m_Task.TaskID, cGlobalParas.UpdateUrlCountType.NaviErr, 0));
                        }

                        dr = null;

                    }
                    catch (NetMinerSkipUrlException ex)
                    {
                        throw ex;
                    }
                    catch (System.Exception ex)
                    {
                        //e_GUrlCount(this, new cGatherUrlCountArgs(this.m_Task.TaskID, cGlobalParas.UpdateUrlCountType.Err, 0));
                        //m_TaskSplitData.GatheredTrueErrUrlCount++;
                        //onError(ex);
                        if (e_Error != null)
                            e_Error(this, new TaskThreadErrorEventArgs(ex));
                    }

                }
                else if (m_ThreadRunning == false)
                {
                    //标识要求终止线程，停止任务，退出for循环提前结束任务
                    if (j == gUrls.Count)
                    {
                        //表示还是采集完成了
                        return cGlobalParas.GatherResult.GatherSucceed;
                    }
                    else
                    {
                        return cGlobalParas.GatherResult.GatherStoppedByUser;
                    }
                }
            }

            return cGlobalParas.GatherResult.GatherSucceed;
        }


        #region 采集数据的统一入口，由此方法进行统一采集调用
        //这是一个通讯的接口方法，不做采集规则的处理，所有需要采集的网页均调用此防范
        //由此方法调用cGatherWeb.GetGatherData，做次方法的目的是为了可以处理错误重试
        private DataTable GetGatherData(string Url, int Level, ref string cookie, List<eWebpageCutFlag> CutFlags,
            eResponse response, bool IsGahterPage, string referUrl, DataRow ndRow,
            string loopFlag, int loopIndex, bool isMultiPage)
        {

            ///在此处理传递进去的采集规则，是符合当前采集级别的规则，采集页默认为0
            ///当导航页的采集时，需要根据导航页的级别制定采集规则

            bool isVisual = false;

            if (CutFlags == null)
            {
                CutFlags = new List<eWebpageCutFlag>();
                eWebpageCutFlag CutFlag = new eWebpageCutFlag();
                for (int i = 0; i < m_TaskSplitData.CutFlag.Count; i++)
                {
                    if (m_TaskSplitData.CutFlag[i].NavLevel == Level)
                    {
                        if (isVisual == false && !string.IsNullOrEmpty(m_TaskSplitData.CutFlag[i].XPath))
                            isVisual = true;
                        CutFlags.Add(m_TaskSplitData.CutFlag[i]);
                    }
                }
            }

            if (CutFlags == null || CutFlags.Count == 0)
                return null;

            if (response==null)
                response = GetHtml(Url, referUrl,isVisual);

            string htmlSource = response.Body;

            if (!(isMultiPage && this.m_Task.IsMultiInterval == true))
                Pause();

            if (this.m_Task.GatherCountPauseInterval > 0)
            {
                if (e_Log != null)
                    e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning, "系统检测到指定采集一定数量后，停止" + (this.m_Task.GatherCountPauseInterval * 1000) + "秒后继续采集，如果设置了多线程则此消息可能出现多次，是由于各个线程发出的消息。", this.m_Task.IsErrorLog));

                Thread.Sleep((int)(this.m_Task.GatherCountPauseInterval * 1000));

                //重置，下次不再停止
                this.m_Task.GatherCountPauseInterval= 0;
            }

            m_gWeb.WebpageSource = htmlSource;
            m_gWeb.CutFlag = CutFlags;
            //设置自动编号
            m_gWeb.AutoID = this.AutoID;

            //设置当前的采集的网址
            m_TaskSplitData.CurUrl = Url;

            DataTable tmpData = null;
            int AgainTime = 0;

        GatherAgain:

            try
            {
                m_gWeb.DataRepeat = this.m_DataRepeat;
                if (IsGahterPage == true)
                {
                    tmpData = m_gWeb.GetGatherData(Url, this.m_Task.StartPos, this.m_Task.EndPos, this.m_Task.SavePath,
                        this.m_Task.IsExportGUrl, this.m_Task.IsExportGDateTime, ndRow, loopFlag, loopIndex,
                        this.m_Task.RejectFlag, this.m_Task.RejectDeal);
                }
                else
                {
                    tmpData = m_gWeb.GetGatherData(Url, this.m_Task.StartPos, this.m_Task.EndPos, this.m_Task.SavePath,
                         false, false, ndRow, loopFlag, loopIndex, this.m_Task.RejectFlag,
                        this.m_Task.RejectDeal);
                }

                if (this.m_Task.IsGatherRepeatStop == true && m_gWeb.IsDataRepeat == true)
                {
                    StopTaskRule((cGlobalParas.StopRule)this.m_Task.GatherRepeatStopRule);
                }

            }
            catch (System.Exception ex)
            {
                if (this.m_Task.IsIgnoreErr == true)
                {
                    e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Error, "网址：" + Url + "采集发生错误请根据错误信息排错，错误信息：" + ex.Message + "", this.m_Task.IsErrorLog));
                    return null;
                }
                else
                {
                    AgainTime++;

                    if (AgainTime > this.m_Task.GatherAgainNumber)
                    {
                        if (this.m_Task.IsErrorLog == true)
                        {
                            //保存出错日志
                        }
                        this.m_ContinuousErr++;
                        if (this.m_ContinuousErr > this.m_Task.GatherErrStopCount && this.m_Task.IsGatherErrStop == true)
                        {
                            StopTaskRule((cGlobalParas.StopRule)this.m_Task.GatherErrStopRule);
                        }

                        throw ex;
                    }
                    else
                    {
                        if (this.m_Task.IsIgnore404 == true && ex.Message.Contains("404"))
                        {
                            if (this.m_Task.IsErrorLog == true)
                            {
                                //保存出错日志
                            }

                            throw ex;
                        }
                        else
                        {
                            e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Error, "网址：" + Url + "访问发生错误，错误信息：" + ex.Message + "，等待3秒重试", this.m_Task.IsErrorLog));

                            Thread.Sleep(3000);

                            e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning, Url + "正在进行第" + AgainTime + "次重试", this.m_Task.IsErrorLog));

                            //返回重试
                            goto GatherAgain;
                        }
                    }
                }
            }


            if (tmpData == null)
            {
                this.m_ContinuousErr++;
                if (this.m_ContinuousErr > this.m_Task.GatherErrStopCount && this.m_Task.IsGatherErrStop == true)
                    StopTaskRule((cGlobalParas.StopRule)this.m_Task.GatherErrStopRule);
            }
            else
                this.m_ContinuousErr = 0;

            #region 在此处理数据加工插件的调用
            ///在此处理数据加工插件的调用
            if (this.m_Task.IsPluginsDeal == true)
            {
                if (ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Program ||
                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Enterprise ||
                        ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Ultimate ||
                        ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Server ||
                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.DistriServer)
                {
                    NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
                    if (this.m_Task.PluginsDeal == "")
                    {
                        if (e_Log != null)
                        {
                            e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning, "配置了数据加工规则插件的调用，但是没有提供插件文件地址！", this.m_Task.IsErrorLog));
                        }
                    }
                    else
                    {
                        tmpData = rPlugin.CallDealData(tmpData, this.m_Task.PluginsDeal);
                    }
                    rPlugin = null;
                }
                else
                {
                    if (e_Log != null)
                    {
                        e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning, "当前版本不支持插件功能，请获取正确版本！", this.m_Task.IsErrorLog));
                    }
                }
            }

            #endregion

            return tmpData;
        }

        private void Pause()
        {
            //让采集线程休眠
            if (this.m_Task.GIntervalTime != 0)
            {
                int git = 0;

                if (this.m_Task.GIntervalTime == this.m_Task.GIntervalTime1)
                    git = (int)(this.m_Task.GIntervalTime * 1000);
                else
                {
                    Random random = new Random();
                    git = random.Next((int)(this.m_Task.GIntervalTime * 1000), (int)(this.m_Task.GIntervalTime1 * 1000));
                }
                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Warning, "采集任务被人为设置每成功采集过一次，停止" + git + "毫秒继续进行！", this.m_Task.IsErrorLog));

                Thread.Sleep(git);
            }
        }
        #endregion

        #endregion

     

        #region 合并数据操作

        /// <summary>
        /// 即根据用户定义的采集规则，进行数据多行的合并，譬如：分页数据采集
        /// </summary>
        /// <param name="tmpData"></param>
        /// <returns></returns>
        private DataTable MergeData(DataTable tmpData)
        {
            if (tmpData == null || tmpData.Rows.Count == 0)
                return null;

            object oldRow;
            object newRow;
            DataTable d1 = tmpData.Clone();
            object[] mRow;

            d1 = tmpData.Clone();

            oldRow = null;

            for (int i = 0; i < tmpData.Rows.Count; i++)
            {
                newRow = tmpData.Rows[i].ItemArray.Clone();
                mRow = MergeRow(oldRow, newRow);

                DataRow r = d1.NewRow();

                if (mRow != null)
                {
                    oldRow = mRow.Clone();
                    r.ItemArray = ((object[])oldRow);

                    if (d1.Rows.Count > 0)
                    {
                        d1.Rows.Remove(d1.Rows[d1.Rows.Count - 1]);
                        d1.Rows.Add(r);
                    }
                    else
                    {
                        d1.Rows.Add(r);
                    }
                }
                else
                {
                    r.ItemArray = ((object[])oldRow);
                    d1.Rows.Add(r);
                    oldRow = newRow;
                }
            }

            if (this.m_Task.IsExportGDateTime == true)
            {
                for (int i = 0; i < d1.Columns.Count; i++)
                {
                    if (d1.Columns[i].ColumnName == "CollectionDateTime")
                    {
                        d1.Rows[0][i] = System.DateTime.Now.ToString();
                    }
                }
            }
            return d1;
        }

        /// <summary>
        /// 将两行数据合并成一行进行输出，合并规则是采集规则配置的是否合并条件
        /// </summary>
        /// <param name="row1"></param>
        /// <param name="row2"></param>
        /// <returns></returns>
        private object[] MergeRow(object row1, object row2)
        {
            if (row1 == null && row2 == null)
                return null;

            if (row1 == null)
            {
                object[] oBjrow2 = ((object[])row2);
                return oBjrow2;
            }

            if (row2 == null)
            {
                object[] oBjrow1 = ((object[])row1);
                return oBjrow1;
            }

            object[] oldRow = ((object[])row1);
            object[] newRow = ((object[])row2);
            int maxColCount = 0;
            if (oldRow.Length > newRow.Length)
                maxColCount = oldRow.Length;
            else
                maxColCount = newRow.Length;

            object[] mergeRow = new object[maxColCount];



            for (int i = 0; i < maxColCount; i++)
            {
                if (i < this.m_TaskSplitData.CutFlag.Count)
                {
                    if (this.m_TaskSplitData.CutFlag[i].IsMergeData == false)
                    {
                        if (oldRow[i].ToString() == newRow[i].ToString())
                        {
                            mergeRow[i] = oldRow[i].ToString();
                        }
                        else
                        {
                            string ss = GetStringSamePart(oldRow[i].ToString(), newRow[i].ToString());
                            mergeRow[i] = ss;
                        }
                    }
                    else
                    {
                        mergeRow[i] = oldRow[i].ToString() + newRow[i].ToString();
                    }
                }
                else
                {
                    if (oldRow[i].ToString() == newRow[i].ToString())
                    {
                        mergeRow[i] = oldRow[i].ToString();
                    }
                    else
                    {
                        string ss = GetStringSamePart(oldRow[i].ToString(), newRow[i].ToString());
                        mergeRow[i] = ss;
                    }
                }
            }
            return mergeRow;
        }

        private string GetStringSamePart(string a, string b)
        {
            string sameStr = string.Empty;
            int sameLen = 0;

            int len = a.Length;

            for (int i = 0; i < len; i++)
            {
                string s = a.Substring(i, 1);

                if (b.IndexOf(s) > -1)
                {
                    for (int j = 1; j < len - i + 1; j++)
                    {
                        string s1 = a.Substring(i, j);
                        if (b.IndexOf(s1) > -1 && s1.Length > sameLen)
                        {
                            sameStr = s1;
                            sameLen = s1.Length;
                        }
                    }
                }
            }

            if (sameStr.Trim() == "")
            {
                if (a.Length > b.Length)
                    sameStr = a;
                else
                    sameStr = b;
            }

            return sameStr;
        }

        #endregion

        #region 直接入库 代码

        private bool NewTable(DataColumnCollection dColumns)
        {
            //首先判断表是否存在，如果不存在则进行建立
            if (IsNewTable == true)
            {
                bool isSucceed = false;

                NetMiner.Core.DB.oDBDeal pData = new NetMiner.Core.DB.oDBDeal(this.m_workPath);
                switch (this.m_Task.ExportType)
                {
                    case cGlobalParas.PublishType.PublishAccess:
                        //isSucceed = pData.NewAccessTable(dColumns, this.DataSource, this.TableName);
                        break;
                    case cGlobalParas.PublishType.PublishMSSql:
                        isSucceed = pData.NewMSSqlServerTable(dColumns, NetMiner.Common.ToolUtil.DecodingDBCon(this.m_Task.DataSource), this.m_Task.DataTableName);
                        break;
                    case cGlobalParas.PublishType.PublishMySql:
                        isSucceed = pData.NewMySqlTable(dColumns, NetMiner.Common.ToolUtil.DecodingDBCon(this.m_Task.DataSource), this.m_Task.DataTableName);
                        break;
                    case cGlobalParas.PublishType.publishOracle:
                        isSucceed = pData.NewOracleTable(dColumns, NetMiner.Common.ToolUtil.DecodingDBCon(this.m_Task.DataSource), this.m_Task.DataTableName);
                        break;
                }
                pData = null;

                if (isSucceed == true)
                    IsNewTable = false;

                return isSucceed;

            }

            return true;

        }
        private void InsertData(DataTable tmpData)
        {
            if (IsNewTable == true)
            {
                e_Log(this, new cGatherTaskLogArgs(this.m_Task.TaskID, this.m_Task.TaskName, cGlobalParas.LogType.Error, "数据表不存在，无法保存采集的数据，请查阅日志！", this.m_Task.IsErrorLog));
                //ThreadState = cGlobalParas.GatherThreadState.Failed;
                return;
            }


            //判断存储数据库的类别
            try
            {
                NetMiner.Core.DB.oDBDeal pData = new NetMiner.Core.DB.oDBDeal(this.m_workPath);

                for (int i = 0; i < tmpData.Rows.Count; i++)
                {
                    object r = tmpData.Rows[i].ItemArray.Clone();

                    switch (this.m_Task.ExportType)
                    {
                        case cGlobalParas.PublishType.PublishAccess:
                            //pData.ExportAccess(tmpData.Columns, r, this.DataSource, this.InsertSql, this.TableName, this.IsSqlTrue);
                            break;
                        case cGlobalParas.PublishType.PublishMSSql:
                            pData.ExportMSSql(tmpData.Columns, r, NetMiner.Common.ToolUtil.DecodingDBCon(this.m_Task.DataSource), this.m_Task.InsertSql, this.m_Task.DataTableName, this.m_Task.IsSqlTrue);
                            break;
                        case cGlobalParas.PublishType.PublishMySql:
                            pData.ExportMySql(tmpData.Columns, r, NetMiner.Common.ToolUtil.DecodingDBCon(this.m_Task.DataSource), this.m_Task.InsertSql, this.m_Task.DataTableName, this.m_Task.IsSqlTrue);
                            break;
                        case cGlobalParas.PublishType.publishOracle:
                            pData.ExportOracle(tmpData.Columns, r, NetMiner.Common.ToolUtil.DecodingDBCon(this.m_Task.DataSource), this.m_Task.InsertSql, this.m_Task.DataTableName, this.m_Task.IsSqlTrue);
                            break;
                    }
                }
                pData = null;
            }
            catch (System.Exception ex)
            {
                m_ContinuousInsertErr++;
                if (m_ContinuousInsertErr > this.m_Task.InsertDataErrStopConut && this.m_Task.IsInsertDataErrStop == true)
                {
                    if (e_RequestStopped != null)
                        e_RequestStopped(this, null);
                }
                throw new NetMinerException("直接入库发生错误，可能由于sql语句不合法造成，详情如下：" + ex.Message);
            }
        }

        #endregion

        #region 将两个datatable的数据合并在一起
        //将datarow与datatable合并在一起 这是一个一对N的关系
        //理论上来讲，dtr1和dt2不会存在重复的列，因为这是一个级联采集
        //才会用到的操作，级联采集肯定是在一个采集任务中的，一个采集任务
        //不允许出现重复采集字段，所以不会存在问题，但如果用户选择了输出
        //采集网址（CollectionUrl），则会出现合并错误，即dtr1和dt2中都有
        //CollectionUrl列，造成合并失败。所以，如果有重复字段，则系统默认
        //删除重复列的第二个，保留第一个
        private DataTable MergeDataTable(DataRow dtr1, DataTable dt2)
        {
            //将dtr1 datarow 转换成Table
            DataTable dt1 = dtr1.Table.Clone();
            dt1.ImportRow(dtr1);

            //判断dt2中是否有与dtr1重复的列，如果有，则删除，删除目标为dt1

            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                bool isExit = false;
                string cName = "";

                for (int j = 0; j < dt1.Columns.Count; j++)
                {
                    if (dt2.Columns[i].ColumnName == dt1.Columns[j].ColumnName)
                    {
                        isExit = true;
                        cName = dt1.Columns[j].ColumnName;
                        break;
                    }
                }

                if (isExit == true)
                {
                    dt1.Columns.Remove(cName);
                }
            }

            //建立一个整合的数据表
            DataTable dt3 = dt1.Clone();

            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                dt3.Columns.Add(dt2.Columns[i].ColumnName);
            }



            object[] obj = new object[dt3.Columns.Count];

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                dt3.Rows.Add(obj);
            }

            if (dt1.Rows.Count >= dt2.Rows.Count)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                    }
                }
            }
            else
            {
                DataRow dr3;
                for (int i = 0; i < dt2.Rows.Count - dt1.Rows.Count; i++)
                {
                    dr3 = dt3.NewRow();
                    for (int m = 0; m < dt1.Columns.Count; m++)
                    {
                        dr3[m] = dt1.Rows[0][m].ToString();
                    }
                    dt3.Rows.Add(dr3);
                }
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                    }
                }
            }

            return dt3;
        }

        //合并两个数据表，注意合并方式为，合并列，并且按照第一个表和第二个表
        //1对多的关系进行数据合并
        private DataTable MergeTable(DataTable d1, DataTable d2)
        {
            DataTable dm = new DataTable();

            for (int i = 0; i < d1.Rows.Count; i++)
            {
                DataRow dr = d1.Rows[i];
                DataTable d = MergeDataTable(dr, d2);
                dm.Merge(d);
            }
            return dm;
        }

        //合并两个表格，按照1对1的方式合并
        private DataTable MergeTable121(DataTable d1, DataTable d2)
        {
            if (d1 == null)
                return d2;
            if (d2 == null)
                return d1;

            DataTable d = d1.Clone();
            DataTable dd1 = d2.Clone();

            d.Merge(dd1);

            int count = 0;
            if (d1.Rows.Count > d2.Rows.Count)
                count = d1.Rows.Count;
            else
                count = d2.Rows.Count;

            for (int i = 0; i < count; i++)
            {
                DataRow dr = d.NewRow();
                for (int j = 0; j < d1.Columns.Count; j++)
                {
                    if (i < d1.Rows.Count)
                    {
                        dr[j] = d1.Rows[i][j].ToString();
                    }

                }

                for (int m = 0; m < d2.Columns.Count; m++)
                {
                    if (i < d2.Rows.Count)
                    {
                        dr[m + d1.Columns.Count] = d2.Rows[i][m].ToString();
                    }
                }

                d.Rows.Add(dr);
            }

            return d;

        }
        #endregion

        /// <summary>
        /// 强制停止任务
        /// </summary>
        private void StopTaskRule(cGlobalParas.StopRule sRule)
        {
            if (sRule == cGlobalParas.StopRule.StopUrlGather)
            {
                throw new NetMinerSkipUrlException("满足用户设置的跳过此网址采集的规则，可能是采集出错或采集到重复数据");
            }
            else if (sRule == cGlobalParas.StopRule.StopTaskGather)
            {
                if (e_RequestStopped != null)
                    e_RequestStopped(this, null);
            }
        }

        private eResponse GetHtml(string Url,string referUrl,bool isVisual)
        {
            //在此控制代理的问题
            eRequest request = NetMiner.Core.Url.UrlPack.GetRequest(Url, this.m_Task.Cookie, this.m_Task.WebCode, this.m_Task.IsUrlEncode,
                                    this.m_Task.IsTwoUrlCode, this.m_Task.UrlEncode,  "",
                                    this.m_Task.Headers, referUrl, this.m_Task.IsUrlAutoRedirect);

            WebProxy wProxy = null;
            //计算此次请求是否需要代理
            if (this.m_Task.IsProxy)
            {
                //需要代理，再判断是否为线程指定的代理
                if(this.pType==cGlobalParas.ProxyType.TaskConfig)
                {
                    //通过proxycontrol获取代理
                    if (this.m_Task.IsProxyFirst)
                    {
                        eProxy proxy= this.m_ProxyControl.GetFirstProxy();
                        wProxy = new WebProxy(proxy.ProxyServer, int.Parse(proxy.ProxyPort));
                        wProxy.BypassProxyOnLocal = true;
                        if (proxy.User != "")
                        {
                            wProxy.Credentials = new NetworkCredential(proxy.User, proxy.Password);
                        }
                    }
                    else
                    {
                        eProxy proxy = this.m_ProxyControl.GetProxy();
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
                else if (this.pType == cGlobalParas.ProxyType.HttpProxy)
                {

                    wProxy = new WebProxy(this.ProxyAddress, this.ProxyPort);
                    wProxy.BypassProxyOnLocal = true;

                    request.ProxyType = cGlobalParas.ProxyType.HttpProxy;
                    request.webProxy = wProxy;

                }
                else if (this.pType==cGlobalParas.ProxyType.Socket5)
                {
                    //请求Scoket

                    request.ProxyType = cGlobalParas.ProxyType.Socket5;
                    request.webProxy = wProxy;
                }

            }
            else
            {
                request.ProxyType = cGlobalParas.ProxyType.SystemProxy;
            }

            eResponse response = NetMiner.Net.Unity.RequestUri(this.m_workPath, request, isVisual);
            //更新cookie
            this.m_tmpCookie = ToolUtil.MergerCookie(this.m_tmpCookie, response.cookie);

            //string htmlSource = response.Body;

            if (response == null)
                response = new eResponse();
            else
            {
                //去除\r\n
                response.Body = Regex.Replace(response.Body, "([\\r\\n])[\\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                response.Body = Regex.Replace(response.Body, "\\n", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                response.Body.Replace("\\r\\n", "");
            }
            return response;
        }

        #region 事件
        private readonly object m_eventLock = new object();

        /// <summary>
        /// 采集任务初始化事件
        /// </summary>
        private event EventHandler<TaskInitializedEventArgs> e_TaskInit;
        internal event EventHandler<TaskInitializedEventArgs> TaskInit
        {
            add { lock (m_eventLock) { e_TaskInit += value; } }
            remove { lock (m_eventLock) { e_TaskInit -= value; } }
        }

        /// <summary>
        /// 分解任务采集开始事件
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Started;
        internal event EventHandler<cTaskEventArgs> Started
        {
            add { lock (m_eventLock) { e_Started += value; } }
            remove { lock (m_eventLock) { e_Started -= value; } }
        }

        /// <summary>
        /// 分解任务停止事件
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Stopped;
        internal event EventHandler<cTaskEventArgs> Stopped
        {
            add { lock (m_eventLock) { e_Stopped += value; } }
            remove { lock (m_eventLock) { e_Stopped -= value; } }
        }

        //请求停止采集任务事件
        private event EventHandler<cTaskEventArgs> e_RequestStopped;
        internal event EventHandler<cTaskEventArgs> RequestStopped
        {
            add { lock (m_eventLock) { e_RequestStopped += value; } }
            remove { lock (m_eventLock) { e_RequestStopped -= value; } }
        }

        /// <summary>
        /// 分解任务采集完成事件
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Completed;
        internal event EventHandler<cTaskEventArgs> Completed
        {
            add { lock (m_eventLock) { e_Completed += value; } }
            remove { lock (m_eventLock) { e_Completed -= value; } }
        }

        /// <summary>
        /// 分解任务采集错误事件
        /// </summary>       
        private event EventHandler<TaskThreadErrorEventArgs> e_Error;
        internal event EventHandler<TaskThreadErrorEventArgs> Error
        {
            add { lock (m_eventLock) { e_Error += value; } }
            remove { lock (m_eventLock) { e_Error -= value; } }
        }

        /// <summary>
        ///  分解任务采集失败事件
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_Failed;
        internal event EventHandler<cTaskEventArgs> Failed
        {
            add { lock (m_eventLock) { e_Failed += value; } }
            remove { lock (m_eventLock) { e_Failed -= value; } }
        }

        /// <summary>
        /// 写日志事件
        /// </summary>
        private event EventHandler<cGatherTaskLogArgs> e_Log;
        internal event EventHandler<cGatherTaskLogArgs> Log
        {
            add { lock (m_eventLock) { e_Log += value; } }
            remove { lock (m_eventLock) { e_Log -= value; } }
        }

        /// <summary>
        /// 返回采集数据事件
        /// </summary>
        private event EventHandler<cGatherDataEventArgs> e_GData;
        internal event EventHandler<cGatherDataEventArgs> GData
        {
            add { lock (m_eventLock) { e_GData += value; } }
            remove { lock (m_eventLock) { e_GData -= value; } }
        }

        private event EventHandler<cGatherUrlCountArgs> e_GUrlCount;
        internal event EventHandler<cGatherUrlCountArgs> GUrlCount
        {
            add { lock (m_eventLock) { e_GUrlCount += value; } }
            remove { lock (m_eventLock) { e_GUrlCount -= value; } }
        }

        private event EventHandler<cUpdateCookieArgs> e_UpdateCookie;
        internal event EventHandler<cUpdateCookieArgs> UpdateCookie
        {
            add { lock (m_eventLock) { e_UpdateCookie += value; } }
            remove { lock (m_eventLock) { e_UpdateCookie -= value; } }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using NetMiner.Common;
using NetMiner.Common.Tool;
using NetMiner.Resource;
using NetMiner.Core.gTask.Entity;
using NetMiner.Base;
using System.Xml;
using System.Xml.Linq;
using NetMiner.Core.pTask.Entity;

///功能：采集任务类，当前版本为1.3 注意：从当前的版本开始不再对以前旧版本任务进行兼容
/// 任务版本于2009-10-10再次进行升级，主要是增加数据输出控制，可以进行多种规则组合控制
/// 任务版本号为：1.6，从此次升级后，任务版本与系统版本保持统一
///完成时间：2009-10-10
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：2009-10-27 修改，增加了导航网址的自动翻页标识，任务版本号升级为1.61
///
///修订：2010-04-15 修改，增加了HTTP Header，任务版本号升级为1.8
///修订：2010-11-19 修改，将采集任务的扩展名修改为：smt
///修订：2010-12-2 升级采集任务版本为2.0
///修订：2011-10-7 升级采集任务版本为2.1 增加url排重选项
///修订：2012-1-30 升级采集任务版本为3.0 增加下载文件保存的地址和重名处理规则


namespace NetMiner.Core.gTask
{
    //[Serializable]

    ///这个类是一个多功能的类，应该重新设计，采用集成的方式来进行
    ///此问题在下一个版本中修改
    ///此类的设计应该是一个任务的基类（可能会做成抽象类），由任务基类完成响应的派生，派生出任务执行类，及各种任务类别
    /// 当前采集任务仅提供了一种，后期会提供多种采集任务，所以，对此类还暂不修改
    ///现在先说当前的问题，此类主要做任务类，同时将任务执行的信息合并到此，这点需要注意，注释中会作出说明

    public class oTask:XmlUnity
    {
        //cXmlIO xmlConfig;
        private Single m_SupportTaskVersion = Single.Parse("5.5");
        private string m_workPath = string.Empty;
        private eTask m_TaskEntity;

        //此类别可处理的任务版本号，注意从1.3开始，任务处理类不再向前兼容
        public Single SupportTaskVersion
        {
            get { return m_SupportTaskVersion; }
        }

        public eTask TaskEntity
        {
            get { return m_TaskEntity; }
            set { m_TaskEntity = value; }
        }


        #region 类的构造和销毁
        public oTask(string workPath)
        {
            m_workPath = workPath;
            TaskEntity = new Entity.eTask();
        }

        ~oTask()
        {
            base.Dispose();
        }
        #endregion


        #region 加载任务
        //加载一个任务到此类中
        public void LoadTask(String FileName)
        {
            if (File.Exists(FileName))
            {
                string strXML = cFile.ReadFileBinary(FileName);
                LoadTaskInfo(strXML);
            }
            else
            {
                throw new Exception("您指定的采集任务文件不存在！");
            }

        }

        //加载一个运行区的任务到此类中，返回此类信息
        //此方法由taskrun专用
        public void LoadTask(Int64 TaskID)
        {
            string FileName = this.m_workPath + NetMiner.Constant.g_TaskRunPath + "\\task" + TaskID + ".rst";

            if (File.Exists(FileName))
            {
                string strXML = cFile.ReadFileBinary(FileName);
                LoadTaskInfo(strXML);
            }
            else
            {
                throw new NetMinerException("您指定的采集任务文件不存在！");
            }
        }

        /// <summary>
        /// 加载任务的字符串xml格式
        /// </summary>
        /// <param name="strXML"></param>
        public void LoadTaskInfo(string strXML)
        {
            bool isDBUrl = false;

            //根据一个任务名称装载一个任务
            try
            {
                TextReader tReader = new StringReader(strXML);

                base.LoadXML(tReader);

            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            #region 基础信息
            //加载任务信息
            m_TaskEntity.TaskID = Int64.Parse(base.GetValue("/Task/BaseInfo/ID"));
            m_TaskEntity.TaskName = base.GetValue("/Task/BaseInfo/Name");
            m_TaskEntity.TaskVersion = Single.Parse(base.GetValue("/Task/BaseInfo/Version"));

            if (m_TaskEntity.TaskVersion != SupportTaskVersion)
            {
                throw new NetMinerException("您加载任务与当前版本不匹配，请核对后重新加载，如低于版本要求，可进行升级操作！");
            }

            m_TaskEntity.TaskDemo = base.GetValue("/Task/BaseInfo/TaskDemo");
            m_TaskEntity.TaskType = (cGlobalParas.TaskType)int.Parse(base.GetValue("/Task/BaseInfo/Type"));
            m_TaskEntity.IsVisual = (base.GetValue("/Task/BaseInfo/Visual") == "True" ? true : false);
            m_TaskEntity.RunType = (cGlobalParas.TaskRunType)int.Parse(base.GetValue("/Task/BaseInfo/RunType"));

            //因存的是相对路径，所以要加上系统路径
            m_TaskEntity.SavePath = this.m_workPath + base.GetValue("/Task/BaseInfo/SavePath");
            m_TaskEntity.UrlCount = int.Parse(base.GetValue("/Task/BaseInfo/UrlCount").ToString());
            m_TaskEntity.ThreadCount = int.Parse(base.GetValue("/Task/BaseInfo/ThreadCount"));
            m_TaskEntity.Cookie = base.GetValue("/Task/BaseInfo/Cookie");
            m_TaskEntity.DemoUrl = base.GetValue("/Task/BaseInfo/DemoUrl");
            m_TaskEntity.StartPos = base.GetValue("/Task/BaseInfo/StartPos");
            m_TaskEntity.EndPos = base.GetValue("/Task/BaseInfo/EndPos");
            m_TaskEntity.WebCode = (cGlobalParas.WebCode)int.Parse(base.GetValue("/Task/BaseInfo/WebCode"));
            m_TaskEntity.IsUrlEncode = (base.GetValue("/Task/BaseInfo/IsUrlEncode") == "True" ? true : false);
            m_TaskEntity.UrlEncode = (cGlobalParas.WebCode)int.Parse(base.GetValue("/Task/BaseInfo/UrlEncode"));
            m_TaskEntity.IsTwoUrlCode = (base.GetValue("/Task/BaseInfo/IsTwoUrlCode") == "True" ? true : false);
            m_TaskEntity.ExportType = (cGlobalParas.PublishType)int.Parse(base.GetValue("/Task/Result/ExportType"));
            m_TaskEntity.ExportFile = base.GetValue("/Task/Result/ExportFileName");
            m_TaskEntity.DataSource = base.GetValue("/Task/Result/DataSource");
            m_TaskEntity.isUserServerDB = (base.GetValue("/Task/Result/IsUserServerDB") == "True" ? true : false); ;
            m_TaskEntity.DataTableName = base.GetValue("/Task/Result/DataTableName");
            m_TaskEntity.IsSqlTrue = (base.GetValue("/Task/Result/IsSqlTrue") == "True" ? true : false);
            m_TaskEntity.InsertSql = base.GetValue("/Task/Result/InsertSql");
            m_TaskEntity.PIntervalTime = int.Parse(base.GetValue("/Task/Result/PublishIntervalTime"));
            m_TaskEntity.PSucceedFlag = base.GetValue("/Task/Result/PublishSucceedFlag");
            m_TaskEntity.PublishThread = int.Parse(base.GetValue("/Task/Result/PublishThread"));

            //开始加载发布模版信息
            m_TaskEntity.TemplateName = base.GetValue("/Task/Result/TemplateName");
            m_TaskEntity.User = base.GetValue("/Task/Result/User");
            m_TaskEntity.Password = base.GetValue("/Task/Result/Password");
            m_TaskEntity.Domain = base.GetValue("/Task/Result/Domain");
            m_TaskEntity.TemplateDBConn = base.GetValue("/Task/Result/PublishDbConn");

            #endregion

            IEnumerable<XElement> paraXE = base.GetElement("/Task/Result/Paras").Elements();
            foreach(XElement xe in paraXE)
            {
                ePublishData pPara = new ePublishData();
                pPara.DataLabel = xe.Element("Label").Value.ToString();
                pPara.DataValue = xe.Element("Value").Value.ToString();
                pPara.DataType = (cGlobalParas.PublishParaType)int.Parse(xe.Element("Type").Value.ToString());
                m_TaskEntity.PublishParas.Add(pPara);
            }

            #region 高级设置

            //加载高级配置信息
            m_TaskEntity.GatherAgainNumber = int.Parse(base.GetValue("/Task/Advance/GatherAgainNumber"));
            m_TaskEntity.IsIgnore404 = (base.GetValue("/Task/Advance/IsIgnore404") == "True" ? true : false);
            m_TaskEntity.IsErrorLog = (base.GetValue("/Task/Advance/IsErrorLog") == "True" ? true : false);
            m_TaskEntity.IsDelRepRow = (base.GetValue("/Task/Advance/IsDelRepeatRow") == "True" ? true : false);
            m_TaskEntity.IsDelTempData = (base.GetValue("/Task/Advance/IsDelTempData") == "True" ? true : false);
            m_TaskEntity.IsSaveSingleFile = (base.GetValue("/Task/Advance/IsSaveSingleFile") == "True" ? true : false);
            m_TaskEntity.TempDataFile = base.GetValue("/Task/Advance/TempFileName");
            m_TaskEntity.IsDataProcess = (base.GetValue("/Task/Advance/IsDataProcess") == "True" ? true : false);
            m_TaskEntity.IsExportGUrl = (base.GetValue("/Task/Advance/IsExportGUrl") == "True" ? true : false);
            m_TaskEntity.IsExportGDateTime = (base.GetValue("/Task/Advance/IsExportGDateTime") == "True" ? true : false);
            m_TaskEntity.IsExportHeader = (base.GetValue("/Task/Advance/IsExportHeader") == "True" ? true : false);

            //这样做的目的是因为版本没有升级，但采集任务格式变化了，加载的时候会出错，所以做了出错的维护
            //请在下一个版本中将此问题修正，升级采集任务即可
            
            m_TaskEntity.IsRowFile = (base.GetValue("/Task/Advance/IsRowFile") == "True" ? true : false);
       
            m_TaskEntity.IsTrigger = (base.GetValue("/Task/Advance/IsTrigger") == "True" ? true : false);
            m_TaskEntity.TriggerType = base.GetValue("/Task/Advance/TriggerType");
            m_TaskEntity.GIntervalTime = float.Parse(base.GetValue("/Task/Advance/GatherIntervalTime"));
            m_TaskEntity.GIntervalTime1 = float.Parse(base.GetValue("/Task/Advance/GatherIntervalTime1"));
            m_TaskEntity.IsMultiInterval = (base.GetValue("/Task/Advance/IsMultiInterval") == "True" ? true : false);

            //V2.0增加
            m_TaskEntity.IsProxy = (base.GetValue("/Task/Advance/IsProxy") == "True" ? true : false);
            m_TaskEntity.IsProxyFirst = (base.GetValue("/Task/Advance/IsProxyFirst") == "True" ? true : false);

            //V2.1增加
            m_TaskEntity.IsUrlNoneRepeat = (base.GetValue("/Task/Advance/IsUrlNoneRepeat") == "True" ? true : false);

            //这样做的目的是因为版本没有升级，但采集任务格式变化了，加载的时候会出错，所以做了出错的维护
            //请在下一个版本中将此问题修正，升级采集任务即可
           
            m_TaskEntity.IsSucceedUrlRepeat = (base.GetValue("/Task/Advance/IsUrlSucceedRepeat") == "True" ? true : false);
          

            //V5增加
            m_TaskEntity.IsUrlAutoRedirect = (base.GetValue("/Task/Advance/IsUrlAutoRedirect") == "True" ? true : false);

            //V5.1增加
            m_TaskEntity.IsGatherErrStop = (base.GetValue("/Task/Advance/IsGatherErrStop") == "True" ? true : false);
            m_TaskEntity.GatherErrStopCount = int.Parse(base.GetValue("/Task/Advance/GatherErrStopCount"));
            m_TaskEntity.GatherErrStopRule = (cGlobalParas.StopRule)int.Parse(base.GetValue("/Task/Advance/GatherErrStopRule"));
            m_TaskEntity.IsInsertDataErrStop = (base.GetValue("/Task/Advance/IsInsertDataErrStop") == "True" ? true : false);
            m_TaskEntity.InsertDataErrStopConut = int.Parse(base.GetValue("/Task/Advance/InsertDataErrStopConut"));
            m_TaskEntity.IsGatherRepeatStop = (base.GetValue("/Task/Advance/IsGatherRepeatStop") == "True" ? true : false);
            m_TaskEntity.GatherRepeatStopRule = (cGlobalParas.StopRule)int.Parse(base.GetValue("/Task/Advance/IsGatherRepeatStopRule"));

            //V5.2增加
            m_TaskEntity.IsIgnoreErr = (base.GetValue("/Task/Advance/IsIgnoreErr") == "True" ? true : false);
            m_TaskEntity.IsAutoUpdateHeader = (base.GetValue("/Task/Advance/IsAutoUpdateHeader") == "True" ? true : false);
            m_TaskEntity.IsNoneAllowSplit = (base.GetValue("/Task/Advance/IsNoneAllowSplit") == "True" ? true : false);
            m_TaskEntity.IsSplitDbUrls = (base.GetValue("/Task/Advance/IsSplitDbUrls") == "True" ? true : false);

            //V5.3增加
            m_TaskEntity.isCookieList = (base.GetValue("/Task/Advance/IsCookieList") == "True" ? true : false);
            m_TaskEntity.GatherCount = int.Parse(base.GetValue("/Task/Advance/GatherCount"));
            m_TaskEntity.GatherCountPauseInterval = int.Parse(base.GetValue("/Task/Advance/GatherCountPauseInterval"));
            m_TaskEntity.RejectFlag = base.GetValue("/Task/Advance/RejectFlag");
            m_TaskEntity.RejectDeal = (cGlobalParas.RejectDeal)int.Parse (base.GetValue("/Task/Advance/RejectDeal") );

            m_TaskEntity.isSameSubTask = (base.GetValue("/Task/Advance/IsSameSubTask") == "True" ? true : false);
            //V5.33
            m_TaskEntity.isGatherCoding = (base.GetValue("/Task/Advance/IsGatherCoding") == "True" ? true : false);
            m_TaskEntity.GatherCodingFlag = base.GetValue("/Task/Advance/GatherCodingFlag");
            m_TaskEntity.GatherCodingPlugin = base.GetValue("/Task/Advance/GatherCodingPlugin");
            m_TaskEntity.CodeUrl = base.GetValue("/Task/Advance/CodingUrl");

            //V5.5增加
            try
            {
                m_TaskEntity.isCloseTab = (base.GetValue("/Task/Advance/IsCloseTab") == "True" ? true : false);
            }
            catch
            {
                m_TaskEntity.isCloseTab = false;
            }
            #endregion

            //以下是V5.32搞出的问题，任务版本变了，但版本号未升级，造成加载
            //失败，所以，先这样处理

            IEnumerable<XElement> proxyXE = base.GetElement("/Task/ThreadProxy").Elements();
          
            foreach(XElement xe in proxyXE )
            {
                eThreadProxy tProxy = new eThreadProxy();
                tProxy.Index = int.Parse(xe.Element("Index").Value.ToString());
                tProxy.pType = (cGlobalParas.ProxyType)int.Parse(xe.Element("pType").Value.ToString());
                tProxy.Address = xe.Element("Address").Value.ToString();
                tProxy.Port = int.Parse(xe.Element("Port").Value.ToString());
                m_TaskEntity.ThreadProxy.Add(tProxy);
            }


            //加载HTTP Header信息
            IEnumerable<XElement> headerXE = base.GetElement("/Task/HttpHeaders").Elements();
            eHeader header;

            foreach (XElement xe in headerXE)
            {
                header = new eHeader();
                header.Label = xe.Element("Label").Value.ToString();
                header.LabelValue = xe.Element("LabelValue").Value.ToString();
                header.Range = xe.Element("Range").Value.ToString();
                m_TaskEntity.Headers.Add(header);
            }

            //加载Trigger信息
            IEnumerable<XElement> triggerXE = base.GetElement("/Task/Trigger").Elements();
            foreach (XElement xe in triggerXE)
            { 
                eTriggerTask tt = new eTriggerTask();
                tt.RunTaskType = (cGlobalParas.RunTaskType)int.Parse(xe.Element("RunTaskType").Value.ToString());
                tt.RunTaskName = xe.Element("RunTaskName").Value.ToString();
                tt.RunTaskPara = xe.Element("RunTaskPara").Value.ToString();

                m_TaskEntity.TriggerTask.Add(tt);
                
            }

            //加载插件的信息
            m_TaskEntity.IsPluginsCookie = (base.GetValue("/Task/Plugins/IsPluginsCookie") == "True" ? true : false);
            m_TaskEntity.PluginsCookie = base.GetValue("/Task/Plugins/PluginsCookie");
            m_TaskEntity.IsPluginsDeal = (base.GetValue("/Task/Plugins/IsPluginsDeal") == "True" ? true : false);
            m_TaskEntity.PluginsDeal = base.GetValue("/Task/Plugins/PluginsDeal");
            m_TaskEntity.IsPluginsPublish = (base.GetValue("/Task/Plugins/IsPluginsPublish") == "True" ? true : false);
            m_TaskEntity.PluginsPublish = base.GetValue("/Task/Plugins/PluginsPublish");

            IEnumerable<XElement> weblinksXE = base.GetElement("/Task/WebLinks").Elements();

            int index = 0;
            foreach(XElement xe in weblinksXE)
            {
                eWebLink w = new eWebLink();
                w.id = index;
                w.Weblink = xe.Element("Url").Value.ToString();

                if (w.Weblink.StartsWith("{DbUrl"))
                {
                    isDBUrl = true;
                }
                w.IsNavigation = xe.Element("IsNag").Value.ToString() == "True" ? true : false;
                w.IsNextpage= xe.Element("IsNextPage").Value.ToString() == "True" ? true : false;
                w.NextPageRule = xe.Element("NextPageRule").Value.ToString();
                w.NextMaxPage = xe.Element("NextMaxPage").Value.ToString();
                w.NextPageUrl = xe.Element("NextPageUrl").Value.ToString();

                w.IsGathered = int.Parse((xe.Element("IsGathered").Value.ToString() == null 
                    || xe.Element("IsGathered").Value.ToString() == "") ? "2031" : xe.Element("IsGathered").Value.ToString());

                #region 加载导航规则
                IEnumerable<XElement> navXE = xe.Element("NavigationRules").Elements("NavigationRule");

                foreach(XElement xe1 in navXE)
                {
                    eNavigRule nRule = new eNavigRule();
                    nRule.Url = xe1.Element("Url").Value.ToString();
                    nRule.Level = int.Parse(xe1.Element("Level").Value.ToString());
                    nRule.IsNext = xe1.Element("IsNext").Value.ToString() == "True" ? true : false;
                    nRule.NextRule = xe1.Element("NextRule").Value.ToString();
                    nRule.NextMaxPage = xe1.Element("NextMaxPage").Value.ToString();
                    nRule.NavigRule = xe1.Element("NagRule").Value.ToString();
                    nRule.NaviStartPos = xe1.Element("NaviStartPos").Value.ToString();
                    nRule.NaviEndPos = xe1.Element("NaviEndPos").Value.ToString();
                    nRule.IsNaviNextPage = xe1.Element("IsNextPage").Value.ToString() == "True" ? true : false;
                    nRule.NaviNextPage = xe1.Element("NextPageRule").Value.ToString();
                    nRule.NaviNextMaxPage = xe1.Element("NaviNextMaxPage").Value.ToString();
                    nRule.IsGather = xe1.Element("IsGather").Value.ToString() == "True" ? true : false;
                    nRule.GatherStartPos = xe1.Element("GatherStartPos").Value.ToString();
                    nRule.GatherEndPos = xe1.Element("GatherEndPos").Value.ToString();
                    nRule.RunRule = (NetMiner.Resource.cGlobalParas.NaviRunRule)int.Parse(xe1.Element("RunType").Value.ToString());
                    nRule.OtherNaviRule = xe1.Element("OtherNaviRule").Value.ToString();
                    w.NavigRules.Add(nRule);
                }
                #endregion
                w.IsMultiGather = xe.Element("IsMultiPageGather").Value.ToString() == "True" ? true : false;
                w.IsData121 = xe.Element("IsMulti121").Value.ToString() == "True" ? true : false;


                //加载多页采集规则
                if (w.IsMultiGather == true)
                {
                    IEnumerable<XElement> multiXE = xe.Element("MultiPageRules").Elements("MultiPageRule");

                    //dn = dw[i].CreateChildView("WebLink_MultiPageRules")[0].CreateChildView("MultiPageRules_MultiPageRule");

                    foreach(XElement xe1 in multiXE)
                    {
                        eMultiPageRule nRule = new eMultiPageRule();
                        nRule.RuleName = xe1.Element("MultiRuleName").Value.ToString();
                        nRule.mLevel = int.Parse(xe1.Element("MultiLevel").Value.ToString());
                        nRule.Rule = xe1.Element("MultiRule").Value.ToString();
                        w.MultiPageRules.Add(nRule);
                    }
                }

                m_TaskEntity.WebpageLink.Add(w);
                w = null;
                
            }

            IEnumerable<XElement> gRuleXes = base.GetElement("/Task/GatherRules").Elements();
            foreach(XElement xe in gRuleXes)
            {
                eWebpageCutFlag c = new eWebpageCutFlag();
                c.Title = xe.Element("Title").Value.ToString();
                c.RuleByPage = (cGlobalParas.GatherRuleByPage)int.Parse((xe.Element("RuleByPage").Value.ToString() == null || xe.Element("RuleByPage").Value.ToString() == "") ? "0" : xe.Element("RuleByPage").Value.ToString());
                c.DataType = (cGlobalParas.GDataType)int.Parse((xe.Element("DataType").Value.ToString() == null || xe.Element("DataType").Value.ToString() == "") ? "0" : xe.Element("DataType").Value.ToString());

                c.GatherRuleType = (cGlobalParas.GatherRuleType)int.Parse((xe.Element("GatherRuleType").Value.ToString() == null || xe.Element("GatherRuleType").Value.ToString() == "") ? "0" :xe.Element("GatherRuleType").Value.ToString());
                c.XPath = xe.Element("XPath").Value.ToString();
                c.NodePrty = xe.Element("NodePrty").Value.ToString();

                c.StartPos = xe.Element("StartFlag").Value.ToString();
                c.EndPos = xe.Element("EndFlag").Value.ToString();
                c.LimitSign = (cGlobalParas.LimitSign)int.Parse((xe.Element("LimitSign").Value.ToString() == null || xe.Element("LimitSign").Value.ToString() == "") ? "0" : xe.Element("LimitSign").Value.ToString());
                c.RegionExpression = xe.Element("RegionExpression").Value.ToString();
                c.IsMergeData = xe.Element("IsMergeData").Value.ToString() == "True" ? true : false;
                c.NavLevel = int.Parse(xe.Element("NavLevel").Value.ToString());

                //采集规则V2.6增加，处理现在文件的存储路径及重名规则
                c.DownloadFileSavePath = xe.Element("DownloadFileSavePath").Value.ToString();
                //c.DownloadFileDealType = dw[i].Row["DownloadFileDealType"].ToString();
                c.MultiPageName = xe.Element("MultiPageName").Value.ToString();

                //3.1所加
                c.IsAutoDownloadFileImage = xe.Element("IsAutoDownloadFileImage").Value.ToString() == "True" ? true : false;
                c.IsAutoDownloadOnlyImage = xe.Element("IsAutoDownloadOnlyImage").Value.ToString() == "True" ? true : false;

                //加载数据输出规则

                IEnumerable<XElement> eRules = xe.Element("ExportRules").Elements("ExportRule");
                foreach(XElement xe1 in eRules)
                {
                    eFieldRule fRule = new eFieldRule();
                    fRule.Field = xe1.Element("ExortField").Value.ToString();
                    fRule.FieldRuleType = (cGlobalParas.ExportLimit)int.Parse(xe1.Element("ExortRuleType").Value.ToString());
                    fRule.FieldRule = xe1.Element("ExortRuleCondition").Value.ToString();

                    c.ExportRules.Add(fRule);
                }

                m_TaskEntity.WebpageCutFlag.Add(c);
                c = null;
             
            }

            //如果网址是dburl则重新更新urlcount
            if (isDBUrl == true)
            {
                int urlsCount = 0;
                //更新urlcount
                for (int urlIndex = 0; urlIndex < m_TaskEntity.WebpageLink.Count; urlIndex++)
                {
                    NetMiner.Core.Url.cUrlParse gUrl = new NetMiner.Core.Url.cUrlParse(this.m_workPath);

                    urlsCount += gUrl.GetUrlCount(m_TaskEntity.WebpageLink[urlIndex].Weblink.ToString());
                }

                m_TaskEntity.UrlCount = urlsCount;

            }
        }

        #endregion


        //保存任务信息，在保存任务信息的同时会自动维护任务分类数据
        /// <summary>
        /// 保存任务文件，采集任务名称从taskname获取,注意此方法会自动维护分类下的index文件
        /// </summary>
        /// <param name="TaskPath">传入的指定路径，是一个相对路径</param>
        /// <param name="indexOP">指示对Index的维护规则</param>
        /// <param name="isCheckRepeat">是否判断任务已经存在</param>
        public void Save(string TaskPath, cGlobalParas.opType indexOP, bool isCheckRepeat)
        {
            //获取需要保存任务的路径
            string tPath = "";

            if (TaskPath == "" || TaskPath == null)
                tPath = NetMiner.Constant.g_RemoteTaskPath + "\\";
            else
                tPath = TaskPath + "\\";

            //判断此路径下是否已经存在了此任务，如果存在则返回错误信息
            if (File.Exists(this.m_workPath +  tPath + m_TaskEntity.TaskName + ".smt") && isCheckRepeat == true)
            {
                throw new NetMinerException("任务已经存在，不能建立");
            }

            oTaskIndex ti = new oTaskIndex(this.m_workPath, this.m_workPath + tPath + "\\index.xml");

            if (indexOP==cGlobalParas.opType.Add)
            {
                //维护任务的Index.xml文件
                eTaskIndex ei = new Entity.eTaskIndex();
                ei.ID = 0;
                ei.TaskName = this.TaskEntity.TaskName;
                ei.TaskType = this.TaskEntity.TaskType;
                ei.TaskRunType = this.TaskEntity.RunType;
                ei.TaskState = cGlobalParas.TaskState.UnStart;
                ei.WebLinkCount = this.TaskEntity.UrlCount;
                ei.ExportFile = this.TaskEntity.ExportFile;
                ei.PublishType = this.TaskEntity.ExportType;

                int TaskID = ti.InsertTaskIndex(ei);
            }
            else if (indexOP==cGlobalParas.opType.Edit)
            {
                eTaskIndex ei = new Entity.eTaskIndex();
                ei.ID = 0;
                ei.TaskName = this.TaskEntity.TaskName;
                ei.TaskType = this.TaskEntity.TaskType;
                ei.TaskRunType = this.TaskEntity.RunType;
                ei.TaskState = cGlobalParas.TaskState.UnStart;
                ei.WebLinkCount = this.TaskEntity.UrlCount;
                ei.ExportFile = this.TaskEntity.ExportFile;
                ei.PublishType = this.TaskEntity.ExportType;

                ti.EditIndexTask(this.TaskEntity.TaskName, ei);
            }

            ti.Dispose();
            ti = null;

            string tXml = GetTaskXML();

            cFile.SaveFileBinary(this.m_workPath + tPath + "\\" + m_TaskEntity.TaskName + ".smt", tXml.ToString(), true);

        }

        //仅根据任务信息保存任务文件，不做其他数据的问题，此方法
        //主要用于支持任务升级使用


        /// <summary>
        /// 将采集任务保存到指定的文件，且不维护index文件，如果任务文件已经存在，则覆盖
        /// </summary>
        /// <param name="TaskName">完整的路径+任务名称</param>
        public void SaveTask(string TaskName)
        {
            string tXml = GetTaskXML();
            cFile.SaveFileBinary(TaskName, tXml.ToString(), true);
        }

        /// <summary>
        /// 获取采集任务的xml文件
        /// </summary>
        /// <returns></returns>
        public string GetTaskXML()
        {
            int i = 0;
            #region 构造任务通用信息

            //开始增加Task任务
            //构造Task任务的XML文档格式
            //当前构造xml文件全部采用的拼写字符串的形式,并没有采用xml构造函数
            StringBuilder tXml = new StringBuilder();
            tXml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                "<Task>" +
                "<State></State>" +       ///此状态值当前无效,用于将来扩充使用
                "<BaseInfo>" +
                "<Version>" + SupportTaskVersion.ToString() + "</Version>" +
                "<ID>" + m_TaskEntity.TaskID + "</ID>" +
                "<Name>" + m_TaskEntity.TaskName + "</Name>" +
                "<TaskDemo>" + m_TaskEntity.TaskDemo + "</TaskDemo>" +
                "<Type>" + (int)m_TaskEntity.TaskType + "</Type>" +

                "<Visual>" + m_TaskEntity.IsVisual + "</Visual>" +                 //此为5.5增加

                "<RunType>" + (int)m_TaskEntity.RunType + "</RunType>" +

                //选哟转换成相对路径
                "<SavePath>" + ToolUtil.GetRelativePath(this.m_workPath, m_TaskEntity.SavePath) + "</SavePath>" +
                "<ThreadCount>" + m_TaskEntity.ThreadCount + "</ThreadCount>" +
                "<UrlCount>" + m_TaskEntity.UrlCount + "</UrlCount>" +
                "<StartPos><![CDATA[" + m_TaskEntity.StartPos + "]]></StartPos>" +
                "<EndPos><![CDATA[" + m_TaskEntity.EndPos + "]]></EndPos>" +
                "<DemoUrl><![CDATA[" + m_TaskEntity.DemoUrl + "]]></DemoUrl>" +
                "<Cookie><![CDATA[" + m_TaskEntity.Cookie + "]]></Cookie>" +
                "<WebCode>" + (int)m_TaskEntity.WebCode + "</WebCode>" +
                "<IsUrlEncode>" + m_TaskEntity.IsUrlEncode + "</IsUrlEncode>" +
                "<UrlEncode>" + (int)m_TaskEntity.UrlEncode + "</UrlEncode>" +
                "<IsTwoUrlCode>" + m_TaskEntity.IsTwoUrlCode + "</IsTwoUrlCode>" +
                "</BaseInfo>" +
                "<Result>" +
                "<ExportType>" + (int)m_TaskEntity.ExportType + "</ExportType>" +
                "<ExportFileName>" + m_TaskEntity.ExportFile + "</ExportFileName>" +
                "<DataSource>" + m_TaskEntity.DataSource + "</DataSource>" +
                "<IsUserServerDB>" + m_TaskEntity.isUserServerDB + "</IsUserServerDB>" +
                "<DataTableName>" + m_TaskEntity.DataTableName + "</DataTableName>" +
                "<IsSqlTrue>" + m_TaskEntity.IsSqlTrue + "</IsSqlTrue>" +

                "<InsertSql>" + m_TaskEntity.InsertSql + "</InsertSql>" +

                "<PublishIntervalTime>" + m_TaskEntity.PIntervalTime + "</PublishIntervalTime>" +
                "<PublishSucceedFlag>" + m_TaskEntity.PSucceedFlag + "</PublishSucceedFlag>" +
                "<PublishThread>" + m_TaskEntity.PublishThread + "</PublishThread>" +

                "<TemplateName>" + m_TaskEntity.TemplateName + "</TemplateName>" +
                "<User>" + m_TaskEntity.User + "</User>" +
                "<Password>" + m_TaskEntity.Password + "</Password>" +
                "<Domain>" + m_TaskEntity.Domain + "</Domain>" +
                "<PublishDbConn>" + m_TaskEntity.TemplateDBConn + "</PublishDbConn>" +
                "<Paras>");

            for (int pi = 0; pi < m_TaskEntity.PublishParas.Count; pi++)
            {
                tXml.Append("<Para>");
                tXml.Append("<Label>" + m_TaskEntity.PublishParas[pi].DataLabel + "</Label>");
                tXml.Append("<Value>" + m_TaskEntity.PublishParas[pi].DataValue + "</Value>");
                tXml.Append("<Type>" + (int)m_TaskEntity.PublishParas[pi].DataType + "</Type>");
                tXml.Append("</Para>");
            }
            tXml.Append("</Paras>" + "</Result>");
            tXml.Append("<Advance>" +
                "<GatherAgainNumber>" + m_TaskEntity.GatherAgainNumber + "</GatherAgainNumber>" +
                "<IsIgnore404>" + m_TaskEntity.IsIgnore404 + "</IsIgnore404>" +
                "<IsErrorLog>" + m_TaskEntity.IsErrorLog + "</IsErrorLog>" +
                "<IsExportHeader>" + m_TaskEntity.IsExportHeader + "</IsExportHeader>" +
                "<IsRowFile>" + m_TaskEntity.IsRowFile + "</IsRowFile>" +
                "<IsDelRepeatRow>" + m_TaskEntity.IsDelRepRow + "</IsDelRepeatRow>" +
                "<IsDelTempData>" + m_TaskEntity.IsDelTempData + "</IsDelTempData>" +
                "<IsSaveSingleFile>" + m_TaskEntity.IsSaveSingleFile + "</IsSaveSingleFile>" +
                "<TempFileName>" + m_TaskEntity.TempDataFile + "</TempFileName>" +
                "<IsDataProcess>" + m_TaskEntity.IsDataProcess + "</IsDataProcess>" +
                "<IsExportGUrl><![CDATA[" + m_TaskEntity.IsExportGUrl + "]]></IsExportGUrl>" +
                "<IsExportGDateTime>" + m_TaskEntity.IsExportGDateTime + "</IsExportGDateTime>" +
                "<IsTrigger>" + m_TaskEntity.IsTrigger + "</IsTrigger>" +
                "<TriggerType>" + m_TaskEntity.TriggerType + "</TriggerType>" +
                "<GatherIntervalTime>" + m_TaskEntity.GIntervalTime + "</GatherIntervalTime>" +
                "<GatherIntervalTime1>" + m_TaskEntity.GIntervalTime1 + "</GatherIntervalTime1>" +
                "<IsMultiInterval>" + m_TaskEntity.IsMultiInterval + "</IsMultiInterval>" +
                "<IsProxy>" + m_TaskEntity.IsProxy + "</IsProxy>" +
                "<IsProxyFirst>" + m_TaskEntity.IsProxyFirst + "</IsProxyFirst>" +
                "<IsUrlNoneRepeat>" + m_TaskEntity.IsUrlNoneRepeat + "</IsUrlNoneRepeat>" +
                "<IsUrlSucceedRepeat>" + m_TaskEntity.IsSucceedUrlRepeat + "</IsUrlSucceedRepeat>" +
                "<IsUrlAutoRedirect>" + m_TaskEntity.IsUrlAutoRedirect + "</IsUrlAutoRedirect>" +
                "<IsGatherErrStop>" + m_TaskEntity.IsGatherErrStop + "</IsGatherErrStop>" +
                "<GatherErrStopCount>" + m_TaskEntity.GatherErrStopCount + "</GatherErrStopCount>" +
                "<GatherErrStopRule>" + (int)m_TaskEntity.GatherErrStopRule + "</GatherErrStopRule>" +
                "<IsInsertDataErrStop>" + m_TaskEntity.IsInsertDataErrStop + "</IsInsertDataErrStop>" +
                "<InsertDataErrStopConut>" + m_TaskEntity.InsertDataErrStopConut + "</InsertDataErrStopConut>" +
                "<IsGatherRepeatStop>" + m_TaskEntity.IsGatherRepeatStop + "</IsGatherRepeatStop>" +
                "<IsGatherRepeatStopRule>" + (int)m_TaskEntity.GatherRepeatStopRule + "</IsGatherRepeatStopRule>" +
                "<IsIgnoreErr>" + m_TaskEntity.IsIgnoreErr + "</IsIgnoreErr>" +
                "<IsAutoUpdateHeader>" + m_TaskEntity.IsAutoUpdateHeader + "</IsAutoUpdateHeader>" +
                "<IsNoneAllowSplit>" + m_TaskEntity.IsNoneAllowSplit + "</IsNoneAllowSplit>" +
                "<IsSplitDbUrls>" + m_TaskEntity.IsSplitDbUrls + "</IsSplitDbUrls>" +
                "<IsCookieList>" + m_TaskEntity.isCookieList + "</IsCookieList>" +
                "<GatherCount>" + m_TaskEntity.GatherCount + "</GatherCount>" +
                "<GatherCountPauseInterval>" + m_TaskEntity.GatherCountPauseInterval + "</GatherCountPauseInterval>" +
                "<RejectFlag><![CDATA[" + m_TaskEntity.RejectFlag + "]]></RejectFlag>" +
                "<RejectDeal>" + (int)m_TaskEntity.RejectDeal + "</RejectDeal>" +
                "<IsSameSubTask>" + m_TaskEntity.isSameSubTask + "</IsSameSubTask>" +
                "<IsGatherCoding>" + m_TaskEntity.isGatherCoding + "</IsGatherCoding>" +
                "<GatherCodingFlag>" + m_TaskEntity.GatherCodingFlag + "</GatherCodingFlag>" +
                "<GatherCodingPlugin>" + m_TaskEntity.GatherCodingPlugin + "</GatherCodingPlugin>" +
                "<CodingUrl><![CDATA[" + m_TaskEntity.CodeUrl + "]]></CodingUrl>" +
                "<IsCloseTab><![CDATA[" + m_TaskEntity.isCloseTab + "]]></IsCloseTab>" +
                "</Advance>");


            //增加每个线程的代理信息
            tXml.Append("<ThreadProxy>");
            for (i = 0; i < m_TaskEntity.ThreadProxy.Count; i++)
            {
                tXml.Append("<Proxy>");
                tXml.Append("<Index>" + m_TaskEntity.ThreadProxy[i].Index + "</Index>");
                tXml.Append("<pType>" + (int)m_TaskEntity.ThreadProxy[i].pType + "</pType>");
                tXml.Append("<Address>" + m_TaskEntity.ThreadProxy[i].Address + "</Address>");
                tXml.Append("<Port>" + m_TaskEntity.ThreadProxy[i].Port + "</Port>");
                tXml.Append("</Proxy>");
            }
            tXml.Append("</ThreadProxy>");

            //增加HTTP Header信息
            tXml.Append("<HttpHeaders>");

            for (i = 0; i < m_TaskEntity.Headers.Count; i++)
            {
                tXml.Append("<Header>");
                tXml.Append("<Label>" + m_TaskEntity.Headers[i].Label + "</Label>");
                tXml.Append("<LabelValue><![CDATA[" + m_TaskEntity.Headers[i].LabelValue + "]]></LabelValue>");
                tXml.Append("<Range><![CDATA[" + m_TaskEntity.Headers[i].Range + "]]></Range>");
                tXml.Append("</Header>");
            }

            tXml.Append("</HttpHeaders>");

            tXml.Append("<Trigger>");
            for (i = 0; i < m_TaskEntity.TriggerTask.Count; i++)
            {
                tXml.Append("<Task>");
                tXml.Append("<RunTaskType>" + m_TaskEntity.TriggerTask[i].RunTaskType + "</RunTaskType>");
                tXml.Append("<RunTaskName>" + m_TaskEntity.TriggerTask[i].RunTaskName + "</RunTaskName>");
                tXml.Append("<RunTaskPara>" + m_TaskEntity.TriggerTask[i].RunTaskPara + "</RunTaskPara>");
                tXml.Append("</Task>");
            }
            tXml.Append("</Trigger>");

            //插件的功能
            tXml.Append("<Plugins>");
            tXml.Append("<IsPluginsCookie>" + m_TaskEntity.IsPluginsCookie + "</IsPluginsCookie>" +
                "<PluginsCookie>" + m_TaskEntity.PluginsCookie + "</PluginsCookie>" +
                "<IsPluginsDeal>" + m_TaskEntity.IsPluginsDeal + "</IsPluginsDeal>" +
                "<PluginsDeal>" + m_TaskEntity.PluginsDeal + "</PluginsDeal>" +
                "<IsPluginsPublish>" + m_TaskEntity.IsPluginsPublish + "</IsPluginsPublish>" +
                "<PluginsPublish>" + m_TaskEntity.PluginsPublish + "</PluginsPublish>");

            tXml.Append("</Plugins>");

            tXml.Append("<WebLinks>");

            if (m_TaskEntity.WebpageLink != null)
            {
                for (i = 0; i < m_TaskEntity.WebpageLink.Count; i++)
                {
                    tXml.Append("<WebLink>");
                    tXml.Append("<Url><![CDATA[" + m_TaskEntity.WebpageLink[i].Weblink.ToString() + "]]></Url>");
                    tXml.Append("<IsNag>" + m_TaskEntity.WebpageLink[i].IsNavigation + "</IsNag>");
                    tXml.Append("<IsMultiPageGather>" + m_TaskEntity.WebpageLink[i].IsMultiGather + "</IsMultiPageGather>");
                    tXml.Append("<IsMulti121>" + m_TaskEntity.WebpageLink[i].IsData121 + "</IsMulti121>");
                    tXml.Append("<IsNextPage>" + m_TaskEntity.WebpageLink[i].IsNextpage + "</IsNextPage>");
                    tXml.Append("<NextPageRule><![CDATA[" + m_TaskEntity.WebpageLink[i].NextPageRule + "]]></NextPageRule>");
                    tXml.Append("<NextMaxPage>" + m_TaskEntity.WebpageLink[i].NextMaxPage + "</NextMaxPage>");

                    tXml.Append("<NextPageUrl></NextPageUrl>");


                    //默认插入一个节点，表示此链接地址还未进行采集，因为是系统添加任务，所以默认为UnGather
                    tXml.Append("<IsGathered>" + (int)cGlobalParas.UrlGatherResult.UnGather + "</IsGathered>");
                    tXml.Append("<NavigationRules>");
                    //插入此网址的导航规则
                    if (m_TaskEntity.WebpageLink[i].IsNavigation == true)
                    {
                        
                        for (int j = 0; j < m_TaskEntity.WebpageLink[i].NavigRules.Count; j++)
                        {
                            tXml.Append("<NavigationRule>");
                            tXml.Append("<Url><![CDATA[" + m_TaskEntity.WebpageLink[i].NavigRules[j].Url + "]]></Url>");
                            tXml.Append("<Level>" + m_TaskEntity.WebpageLink[i].NavigRules[j].Level + "</Level>");
                            tXml.Append("<IsNext>" + m_TaskEntity.WebpageLink[i].NavigRules[j].IsNext + "</IsNext>");
                            tXml.Append("<NextRule><![CDATA[" + m_TaskEntity.WebpageLink[i].NavigRules[j].NextRule + "]]></NextRule>");
                            tXml.Append("<NextMaxPage>" + m_TaskEntity.WebpageLink[i].NavigRules[j].NextMaxPage + "</NextMaxPage>");
                            tXml.Append("<NaviStartPos><![CDATA[" + m_TaskEntity.WebpageLink[i].NavigRules[j].NaviStartPos + "]]></NaviStartPos>");
                            tXml.Append("<NaviEndPos><![CDATA[" + m_TaskEntity.WebpageLink[i].NavigRules[j].NaviEndPos + "]]></NaviEndPos>");
                            tXml.Append("<NagRule><![CDATA[" + m_TaskEntity.WebpageLink[i].NavigRules[j].NavigRule + "]]></NagRule>");
                            tXml.Append("<IsNextPage>" + m_TaskEntity.WebpageLink[i].NavigRules[j].IsNaviNextPage + "</IsNextPage>");
                            tXml.Append("<NextPageRule><![CDATA[" + m_TaskEntity.WebpageLink[i].NavigRules[j].NaviNextPage + "]]></NextPageRule>");
                            tXml.Append("<NaviNextMaxPage>" + m_TaskEntity.WebpageLink[i].NavigRules[j].NaviNextMaxPage + "</NaviNextMaxPage>");
                            tXml.Append("<IsGather>" + m_TaskEntity.WebpageLink[i].NavigRules[j].IsGather + "</IsGather>");
                            tXml.Append("<GatherStartPos><![CDATA[" + m_TaskEntity.WebpageLink[i].NavigRules[j].GatherStartPos + "]]></GatherStartPos>");
                            tXml.Append("<GatherEndPos><![CDATA[" + m_TaskEntity.WebpageLink[i].NavigRules[j].GatherEndPos + "]]></GatherEndPos>");

                            tXml.Append("<RunType><![CDATA[" + (int)m_TaskEntity.WebpageLink[i].NavigRules[j].RunRule + "]]></RunType>");
                            tXml.Append("<OtherNaviRule><![CDATA[" + m_TaskEntity.WebpageLink[i].NavigRules[j].OtherNaviRule + "]]></OtherNaviRule>");


                            tXml.Append("</NavigationRule>");
                        }
                        
                    }
                    tXml.Append("</NavigationRules>");

                    tXml.Append("<MultiPageRules>");
                    //插入此网址的多页采集规则
                    if (m_TaskEntity.WebpageLink[i].IsMultiGather == true)
                    {
                        
                        for (int j = 0; j < m_TaskEntity.WebpageLink[i].MultiPageRules.Count; j++)
                        {
                            tXml.Append("<MultiPageRule>");
                            tXml.Append("<MultiRuleName>" + m_TaskEntity.WebpageLink[i].MultiPageRules[j].RuleName + "</MultiRuleName>");

                            //V5.2增加
                            tXml.Append("<MultiLevel><![CDATA[" + m_TaskEntity.WebpageLink[i].MultiPageRules[j].mLevel + "]]></MultiLevel>");

                            tXml.Append("<MultiRule><![CDATA[" + m_TaskEntity.WebpageLink[i].MultiPageRules[j].Rule + "]]></MultiRule>");
                            tXml.Append("</MultiPageRule>");
                        }
                        
                    }
                    tXml.Append("</MultiPageRules>");
                    tXml.Append("</WebLink>");
                }
            }

            tXml.Append("</WebLinks>");
            tXml.Append("<GatherRules>");

            if (m_TaskEntity.WebpageCutFlag != null)
            {
                for (i = 0; i < m_TaskEntity.WebpageCutFlag.Count; i++)
                {
                    tXml.Append("<GatherRule>");
                    tXml.Append("<Title><![CDATA[" + m_TaskEntity.WebpageCutFlag[i].Title + "]]></Title>");
                    tXml.Append("<RuleByPage>" + (int)m_TaskEntity.WebpageCutFlag[i].RuleByPage + "</RuleByPage>");
                    tXml.Append("<DataType>" + (int)m_TaskEntity.WebpageCutFlag[i].DataType + "</DataType>");

                    tXml.Append("<GatherRuleType>" + (int)m_TaskEntity.WebpageCutFlag[i].GatherRuleType + "</GatherRuleType>");
                    tXml.Append("<XPath><![CDATA[" + m_TaskEntity.WebpageCutFlag[i].XPath + "]]></XPath>");
                    tXml.Append("<NodePrty>" + m_TaskEntity.WebpageCutFlag[i].NodePrty + "</NodePrty>");

                    tXml.Append("<StartFlag><![CDATA[" + m_TaskEntity.WebpageCutFlag[i].StartPos + "]]></StartFlag>");
                    tXml.Append("<EndFlag><![CDATA[" + m_TaskEntity.WebpageCutFlag[i].EndPos + "]]></EndFlag>");
                    tXml.Append("<LimitSign>" + (int)m_TaskEntity.WebpageCutFlag[i].LimitSign + "</LimitSign>");
                    tXml.Append("<RegionExpression><![CDATA[" + m_TaskEntity.WebpageCutFlag[i].RegionExpression + "]]></RegionExpression>");
                    tXml.Append("<IsMergeData>" + m_TaskEntity.WebpageCutFlag[i].IsMergeData + "</IsMergeData>");
                    tXml.Append("<NavLevel>" + m_TaskEntity.WebpageCutFlag[i].NavLevel + "</NavLevel>");

                    //采集规则V3.0增加，处理下载文件存储路径及重名规则
                    tXml.Append("<MultiPageName>" + m_TaskEntity.WebpageCutFlag[i].MultiPageName + "</MultiPageName>");
                    tXml.Append("<DownloadFileSavePath>" + m_TaskEntity.WebpageCutFlag[i].DownloadFileSavePath + "</DownloadFileSavePath>");
                    //tXml.Append ( "<DownloadFileDealType>" + m_TaskEntity.WebpageCutFlag[i].DownloadFileDealType + "</DownloadFileDealType>");
                    //tXml.Append ( "<IsOcrText>" + m_TaskEntity.WebpageCutFlag[i].IsOcrText + "</IsOcrText>");

                    ////3.1所加
                    //tXml.Append ( "<OcrScale>" + m_TaskEntity.WebpageCutFlag[i].OcrScale + "</OcrScale>");

                    tXml.Append("<IsAutoDownloadFileImage>" + m_TaskEntity.WebpageCutFlag[i].IsAutoDownloadFileImage + "</IsAutoDownloadFileImage>");

                    //V5.0增加
                    tXml.Append("<IsAutoDownloadOnlyImage>" + m_TaskEntity.WebpageCutFlag[i].IsAutoDownloadOnlyImage + "</IsAutoDownloadOnlyImage>");


                    //插入数据输出规则

                    tXml.Append("<ExportRules>");

                    for (int m = 0; m < m_TaskEntity.WebpageCutFlag[i].ExportRules.Count; m++)
                    {
                        tXml.Append("<ExportRule>");
                        tXml.Append("<ExortField><![CDATA[" + m_TaskEntity.WebpageCutFlag[i].ExportRules[m].Field + "]]></ExortField>");
                        tXml.Append("<ExortRuleType>" + (int)m_TaskEntity.WebpageCutFlag[i].ExportRules[m].FieldRuleType + "</ExortRuleType>");
                        tXml.Append("<ExortRuleCondition><![CDATA[" + m_TaskEntity.WebpageCutFlag[i].ExportRules[m].FieldRule + "]]></ExortRuleCondition>");
                        tXml.Append("</ExportRule>");

                    }
                    tXml.Append("</ExportRules>");

                    tXml.Append("</GatherRule>");
                }
            }
            tXml.Append("</GatherRules>" +
               "</Task>");
            #endregion

            return tXml.ToString();
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="TaskName">任务名称</param>
        ///// <param name="OldTaskClass">原有任务分类</param>
        ///// <param name="NewTaskClass">新任务分类</param>
        ///// 
        //public void ChangeTaskClass(string TaskName, string OldTaskClass, string NewTaskClass)
        //{
        //    oTaskClass tc = new oTaskClass(this.m_workPath);
        //    string oldPath = "";
        //    string NewPath = "";

        //    if (OldTaskClass == "")
        //        oldPath = this.m_workPath + "tasks";
        //    else
        //        oldPath = tc.GetTaskClassPathByName(OldTaskClass);

        //    if (NewTaskClass == "")
        //        NewPath = this.m_workPath + "tasks";
        //    else
        //        NewPath = tc.GetTaskClassPathByName(NewTaskClass);

        //    string FileName = TaskName + ".smt";

        //    System.IO.File.Copy(oldPath + "\\" + FileName, NewPath + "\\" + FileName);

        //    LoadTask(NewPath + "\\" + FileName);

        //    if (NewTaskClass == "")
        //        m_TaskEntity.TaskClass = "";
        //    else
        //        m_TaskEntity.TaskClass = NewTaskClass;

        //    Save("", false);

        //    DeleTask(oldPath, TaskName);

        //    tc = null;
        //}

        ////拷贝任务操作，将一个任务从原有分类拷贝到另一个分类下
        //public void CopyTask(string TaskName, string OldTaskClass, string NewTaskClass)
        //{
        //    oTaskClass tc = new oTaskClass(this.m_workPath);
        //    string oldPath = "";
        //    string NewPath = "";

        //    if (OldTaskClass == "")
        //        oldPath = this.m_workPath + "tasks";
        //    else
        //        oldPath = tc.GetTaskClassPathByName(OldTaskClass);

        //    if (NewTaskClass == "")
        //        NewPath = this.m_workPath + "tasks";
        //    else
        //        NewPath = tc.GetTaskClassPathByName(NewTaskClass);

        //    tc = null;

        //    string FileName = "";

        //    if (OldTaskClass == NewTaskClass || (File.Exists(NewPath + "\\" + TaskName + ".smt")))
        //    {
        //        FileName = TaskName + "-复制.smt";

        //        System.IO.File.Copy(oldPath + "\\" + TaskName + ".smt", NewPath + "\\" + FileName);
        //        TaskName = TaskName + "-复制";

        //    }
        //    else
        //    {
        //        FileName = TaskName + ".smt";
        //        System.IO.File.Copy(oldPath + "\\" + FileName, NewPath + "\\" + FileName);
        //    }

        //    Save("", false);
        //    //SaveTask(NewPath + "\\" + FileName);

        //}

        //插入任务信息到任务索引文件，返回新建任务索引的任务id
        //public int InsertTaskIndex(string TaskClass, string tPath)
        //{

        //    oTaskIndex tIndex;

        //    string FileName;
        //    FileName = tPath + "\\index.xml";
        //    bool IsExists = System.IO.File.Exists(FileName);
        //    int MaxTaskID = 0;

        //    //判断此路径下是否存在任务的索引文件
        //    if (!IsExists)
        //    {
        //        //如果不存在索引文件，则需要建立一个文件
        //        tIndex = new oTaskIndex(this.m_workPath);
        //        tIndex.NewIndexFile(tPath);
        //        MaxTaskID = 1;
        //    }
        //    else
        //    {
        //        tIndex = new oTaskIndex(this.m_workPath, tPath + "\\index.xml");
        //        tIndex.GetTaskDataByClass(TaskClass);
        //        MaxTaskID = tIndex.GetTaskCount();
        //    }

        //    NetMiner.Core.gTask.Entity.eTaskIndex eIndex = new Core.Task.Entity.eTaskIndex();
        //    eIndex.ID = MaxTaskID + 1;
        //    eIndex.TaskName = m_TaskEntity.TaskName;
        //    eIndex.TaskType = m_TaskEntity.TaskType;
        //    eIndex.TaskRunType = m_TaskEntity.RunType;
        //    eIndex.ExportFile = m_TaskEntity.ExportFile;
        //    eIndex.WebLinkCount = m_TaskEntity.UrlCount;
        //    eIndex.PublishType = m_TaskEntity.ExportType;
        
        //    tIndex.InsertTaskIndex(eIndex);
        //    tIndex.Dispose();
        //    tIndex = null;

        //    return MaxTaskID;

        //}

        //当新建一个任务时，调用此方法
        public void New()
        {
           if (base.xDoc!=null)
           {
                base.Save();
                base.xDoc = null;
           }
        }

        /// <summary>
        /// 传入的是相对地址
        /// </summary>
        /// <param name="TaskPath"></param>
        /// <param name="TaskName"></param>
        /// <returns></returns>
        public bool DeleTask(string TaskPath, string TaskName)
        {
            //首先删除此任务所在分类下的index.xml中的索引内容然后再删除具体的任务文件
            string tPath = "";

            if (TaskPath == "")
            {
                tPath = this.m_workPath + "Tasks";
                TaskPath = this.m_workPath + "Tasks";
            }
            else
            {
                tPath = this.m_workPath + TaskPath;
            }

            //先删除索引文件中的任务索引内容
            oTaskIndex tIndex = new oTaskIndex(this.m_workPath, tPath + "\\index.xml");
            tIndex.DeleTaskIndex(TaskName);
            tIndex.Dispose();
            tIndex = null;

            //如果是编辑状态，为了防止删除了文件后，任务保存失败，则
            //任务文件将丢失的问题，首先先不删除此文件，只是将其改名

            //删除任务的物理文件
            string FileName = this.m_workPath + TaskPath + "\\" + TaskName + ".smt";
            string tmpFileName = this.m_workPath + TaskPath + "\\~" + TaskName + ".smt";

            try
            {
                //删除物理临时文件
                if (File.Exists(tmpFileName))
                {
                    //File.SetAttributes(tmpFileName, System.IO.FileAttributes.Normal );
                    System.IO.File.Delete(tmpFileName);
                }

                System.IO.File.Move(FileName, tmpFileName);
                File.SetAttributes(tmpFileName, System.IO.FileAttributes.Hidden);

            }
            catch (System.Exception)
            {
                //如果出现临时文件备份操作失败，则继续进行，不能影响到最终的文件保存
                //但如果文件保存也失败，那只能报错了
            }

            //删除物理任务文件
            if (File.Exists(FileName))
            {
                File.SetAttributes(FileName, System.IO.FileAttributes.Normal);
                System.IO.File.Delete(FileName);
            }

            //将文件设置为隐藏
            //System.IO.File.SetAttributes(tmpFileName, System.IO.FileAttributes.Hidden);
            return true;
        }

    
    }
}

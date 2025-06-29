using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using NetMiner.Common;
using NetMiner.Resource;
using NetMiner.Core.gTask.Entity;
using NetMiner.Core.gTask;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NetMiner.Core.pTask.Entity;

///���ܣ��������������࣬��ǰ֧�ֵ�����汾ʱ1.61 ����汾����������ǰһ���汾������汾
/// ������Ծʽ���������ܻᷢ������
///���ʱ�䣺2009-8-30
///���ߣ�һ��
///�������⣺��
///�����ƻ�����һ���ֵ䲿��Ҫǿ��
///˵����
///�汾��01.60.00
///�޶�����
///
///�޶� 2010-04-15  ����汾����Ϊ��1.8
///�޶� 2010-12-5 ����汾����Ϊ��2.0 ����֧��1.8�Զ�������
///�޶� 2012-2-9 ����汾����Ϊ��2012��3.0 ����֧�ִ�2.0����
///������汾5.0��ʼ��ֻ��ǰ����һ���汾
namespace NetMiner.Gather.Task
{
    public class cUpgradeTask
    {
        private string m_workPath = string.Empty;

        public cUpgradeTask(string workPath)
        {
            m_workPath = workPath;
        }

        ~cUpgradeTask()
        {

        }

        private Single m_SupportTaskVersion = Single.Parse("5.5");

        //�����ɴ��������汾�ţ�ע���1.3��ʼ���������಻����ǰ����
        public Single SupportTaskVersion
        {
            get { return m_SupportTaskVersion; }
        }

        //ָ�����������������
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="FileName">�������������ļ�</param>
        /// <param name="IsBackup">�Ƿ񱸷�</param>
        /// <param name="IsTask">�Ƿ�Ϊϵͳ���������ϵͳ��������Ҫά��������Ϣ��������Ǿ�ֻ�����ļ�</param>
        public void UpdradeTask(string FileName ,bool IsBackup,bool IsSystemTask)
        {
            bool isDBUrl = false;

            //�ж��Ƿ���б���
            if (IsBackup == true)
            {
                if (File.Exists(FileName + ".bak"))
                    File.Delete(FileName + ".bak");

                File.Copy(FileName, FileName + ".bak");
            }

            //���������ļ�

            string strXML= NetMiner.Common.Tool.cFile.ReadFileBinary(FileName);

            oTask t = new oTask(m_workPath);
            XDocument doc = XDocument.Load(new StringReader(strXML));

            #region ������Ϣ
            //����������Ϣ
            t.TaskEntity.TaskID =  Int64.Parse(doc.Root.XPathSelectElement("/Task/BaseInfo/ID").Value.ToString());
            t.TaskEntity.TaskName = doc.Root.XPathSelectElement("/Task/BaseInfo/Name").Value.ToString(); 
            t.TaskEntity.TaskVersion = Single.Parse(doc.Root.XPathSelectElement("/Task/BaseInfo/Version").Value.ToString());

            //�ڴ˿���ֻ������5.33�İ汾
            if (t.TaskEntity.TaskVersion < 5.33F)
            {
                throw new NetMinerException("�����������뵱ǰ�汾��ƥ�䣬��˶Ժ����¼��أ�����ڰ汾Ҫ�󣬿ɽ�������������");
            }

            t.TaskEntity.TaskDemo = doc.Root.XPathSelectElement("/Task/BaseInfo/TaskDemo").Value.ToString();
            t.TaskEntity.TaskType = (cGlobalParas.TaskType)int.Parse(doc.Root.XPathSelectElement("/Task/BaseInfo/Type").Value.ToString());
            t.TaskEntity.IsVisual = false;
            t.TaskEntity.RunType = (cGlobalParas.TaskRunType)int.Parse(doc.Root.XPathSelectElement("/Task/BaseInfo/RunType").Value.ToString());

            //���������·��������Ҫ����ϵͳ·��
            t.TaskEntity.SavePath = this.m_workPath + doc.Root.XPathSelectElement("/Task/BaseInfo/SavePath").Value.ToString();
            t.TaskEntity.UrlCount = int.Parse(doc.Root.XPathSelectElement("/Task/BaseInfo/UrlCount").Value.ToString());
            t.TaskEntity.ThreadCount = int.Parse(doc.Root.XPathSelectElement("/Task/BaseInfo/ThreadCount").Value.ToString());
            t.TaskEntity.Cookie = doc.Root.XPathSelectElement("/Task/BaseInfo/Cookie").Value.ToString();
            t.TaskEntity.DemoUrl = doc.Root.XPathSelectElement("/Task/BaseInfo/DemoUrl").Value.ToString();
            t.TaskEntity.StartPos = doc.Root.XPathSelectElement("/Task/BaseInfo/StartPos").Value.ToString();
            t.TaskEntity.EndPos = doc.Root.XPathSelectElement("/Task/BaseInfo/EndPos").Value.ToString();
            t.TaskEntity.WebCode = (cGlobalParas.WebCode)int.Parse(doc.Root.XPathSelectElement("/Task/BaseInfo/WebCode").Value.ToString());
            t.TaskEntity.IsUrlEncode = (doc.Root.XPathSelectElement("/Task/BaseInfo/IsUrlEncode").Value.ToString() == "True" ? true : false);
            t.TaskEntity.UrlEncode = (cGlobalParas.WebCode)int.Parse(doc.Root.XPathSelectElement("/Task/BaseInfo/UrlEncode").Value.ToString());
            t.TaskEntity.IsTwoUrlCode = (doc.Root.XPathSelectElement("/Task/BaseInfo/IsTwoUrlCode").Value.ToString() == "True" ? true : false);
            t.TaskEntity.ExportType = (cGlobalParas.PublishType)int.Parse(doc.Root.XPathSelectElement("/Task/Result/ExportType").Value.ToString());
            t.TaskEntity.ExportFile = doc.Root.XPathSelectElement("/Task/Result/ExportFileName").Value.ToString();
            t.TaskEntity.DataSource = doc.Root.XPathSelectElement("/Task/Result/DataSource").Value.ToString();
            t.TaskEntity.isUserServerDB = (doc.Root.XPathSelectElement("/Task/Result/IsUserServerDB").Value.ToString() == "True" ? true : false); ;
            t.TaskEntity.DataTableName = doc.Root.XPathSelectElement("/Task/Result/DataTableName").Value.ToString();
            t.TaskEntity.IsSqlTrue = (doc.Root.XPathSelectElement("/Task/Result/IsSqlTrue").Value.ToString() == "True" ? true : false);
            t.TaskEntity.InsertSql = doc.Root.XPathSelectElement("/Task/Result/InsertSql").Value.ToString();
            t.TaskEntity.PIntervalTime = int.Parse(doc.Root.XPathSelectElement("/Task/Result/PublishIntervalTime").Value.ToString());
            t.TaskEntity.PSucceedFlag = doc.Root.XPathSelectElement("/Task/Result/PublishSucceedFlag").Value.ToString();
            t.TaskEntity.PublishThread = int.Parse(doc.Root.XPathSelectElement("/Task/Result/PublishThread").Value.ToString());

            //��ʼ���ط���ģ����Ϣ
            t.TaskEntity.TemplateName = doc.Root.XPathSelectElement("/Task/Result/TemplateName").Value.ToString();
            t.TaskEntity.User = doc.Root.XPathSelectElement("/Task/Result/User").Value.ToString();
            t.TaskEntity.Password = doc.Root.XPathSelectElement("/Task/Result/Password").Value.ToString();
            t.TaskEntity.Domain = doc.Root.XPathSelectElement("/Task/Result/Domain").Value.ToString();
            t.TaskEntity.TemplateDBConn = doc.Root.XPathSelectElement("/Task/Result/PublishDbConn").Value.ToString();
            #endregion

            IEnumerable<XElement> paraXE = doc.Root.XPathSelectElement("/Task/Result/Paras").Elements();
            foreach (XElement xe in paraXE)
            {
                ePublishData pPara = new ePublishData();
                pPara.DataLabel = xe.Element("Label").Value.ToString();
                pPara.DataValue = xe.Element("Value").Value.ToString();
                pPara.DataType = (cGlobalParas.PublishParaType)int.Parse(xe.Element("Type").Value.ToString());
                t.TaskEntity.PublishParas.Add(pPara);
            }

            #region �߼�����
            //���ظ߼�������Ϣ
            t.TaskEntity.GatherAgainNumber = int.Parse(doc.Root.XPathSelectElement("/Task/Advance/GatherAgainNumber").Value.ToString());
            t.TaskEntity.IsIgnore404 = (doc.Root.XPathSelectElement("/Task/Advance/IsIgnore404").Value.ToString() == "True" ? true : false);
            t.TaskEntity.IsErrorLog = (doc.Root.XPathSelectElement("/Task/Advance/IsErrorLog").Value.ToString() == "True" ? true : false);
            t.TaskEntity.IsDelRepRow = (doc.Root.XPathSelectElement("/Task/Advance/IsDelRepeatRow").Value.ToString() == "True" ? true : false);
            t.TaskEntity.IsDelTempData = (doc.Root.XPathSelectElement("/Task/Advance/IsDelTempData").Value.ToString() == "True" ? true : false);
            t.TaskEntity.IsSaveSingleFile = (doc.Root.XPathSelectElement("/Task/Advance/IsSaveSingleFile").Value.ToString() == "True" ? true : false);
            t.TaskEntity.TempDataFile = doc.Root.XPathSelectElement("/Task/Advance/TempFileName").Value.ToString();
            t.TaskEntity.IsDataProcess = (doc.Root.XPathSelectElement("/Task/Advance/IsDataProcess").Value.ToString() == "True" ? true : false);
            t.TaskEntity.IsExportGUrl = (doc.Root.XPathSelectElement("/Task/Advance/IsExportGUrl").Value.ToString() == "True" ? true : false);
            t.TaskEntity.IsExportGDateTime = (doc.Root.XPathSelectElement("/Task/Advance/IsExportGDateTime").Value.ToString() == "True" ? true : false);
            t.TaskEntity.IsExportHeader = (doc.Root.XPathSelectElement("/Task/Advance/IsExportHeader").Value.ToString() == "True" ? true : false);

            //��������Ŀ������Ϊ�汾û�����������ɼ������ʽ�仯�ˣ����ص�ʱ�������������˳����ά��
            //������һ���汾�н������������������ɼ����񼴿�
            t.TaskEntity.IsRowFile = (doc.Root.XPathSelectElement("/Task/Advance/IsRowFile").Value.ToString() == "True" ? true : false);
            t.TaskEntity.IsTrigger = (doc.Root.XPathSelectElement("/Task/Advance/IsTrigger").Value.ToString() == "True" ? true : false);
            t.TaskEntity.TriggerType = doc.Root.XPathSelectElement("/Task/Advance/TriggerType").Value.ToString();
            t.TaskEntity.GIntervalTime = float.Parse(doc.Root.XPathSelectElement("/Task/Advance/GatherIntervalTime").Value.ToString());
            t.TaskEntity.GIntervalTime1 = float.Parse(doc.Root.XPathSelectElement("/Task/Advance/GatherIntervalTime1").Value.ToString());
            t.TaskEntity.IsMultiInterval = (doc.Root.XPathSelectElement("/Task/Advance/IsMultiInterval").Value.ToString() == "True" ? true : false);

            //V2.0����
            t.TaskEntity.IsProxy = (doc.Root.XPathSelectElement("/Task/Advance/IsProxy").Value.ToString() == "True" ? true : false);
            t.TaskEntity.IsProxyFirst = (doc.Root.XPathSelectElement("/Task/Advance/IsProxyFirst").Value.ToString() == "True" ? true : false);

            //V2.1����
            t.TaskEntity.IsUrlNoneRepeat = (doc.Root.XPathSelectElement("/Task/Advance/IsUrlNoneRepeat").Value.ToString() == "True" ? true : false);

            //��������Ŀ������Ϊ�汾û�����������ɼ������ʽ�仯�ˣ����ص�ʱ�������������˳����ά��
            //������һ���汾�н������������������ɼ����񼴿�

            t.TaskEntity.IsSucceedUrlRepeat = (doc.Root.XPathSelectElement("/Task/Advance/IsUrlSucceedRepeat").Value.ToString() == "True" ? true : false);


            //V5����
            t.TaskEntity.IsUrlAutoRedirect = (doc.Root.XPathSelectElement("/Task/Advance/IsUrlAutoRedirect").Value.ToString() == "True" ? true : false);

            //V5.1����
            t.TaskEntity.IsGatherErrStop = (doc.Root.XPathSelectElement("/Task/Advance/IsGatherErrStop").Value.ToString() == "True" ? true : false);
            t.TaskEntity.GatherErrStopCount = int.Parse(doc.Root.XPathSelectElement("/Task/Advance/GatherErrStopCount").Value.ToString());
            t.TaskEntity.GatherErrStopRule = (cGlobalParas.StopRule)int.Parse(doc.Root.XPathSelectElement("/Task/Advance/GatherErrStopRule").Value.ToString());
            t.TaskEntity.IsInsertDataErrStop = (doc.Root.XPathSelectElement("/Task/Advance/IsInsertDataErrStop").Value.ToString() == "True" ? true : false);
            t.TaskEntity.InsertDataErrStopConut = int.Parse(doc.Root.XPathSelectElement("/Task/Advance/InsertDataErrStopConut").Value.ToString());
            t.TaskEntity.IsGatherRepeatStop = (doc.Root.XPathSelectElement("/Task/Advance/IsGatherRepeatStop").Value.ToString() == "True" ? true : false);
            t.TaskEntity.GatherRepeatStopRule = (cGlobalParas.StopRule)int.Parse(doc.Root.XPathSelectElement("/Task/Advance/IsGatherRepeatStopRule").Value.ToString());

            //V5.2����
            t.TaskEntity.IsIgnoreErr = (doc.Root.XPathSelectElement("/Task/Advance/IsIgnoreErr").Value.ToString() == "True" ? true : false);
            t.TaskEntity.IsAutoUpdateHeader = (doc.Root.XPathSelectElement("/Task/Advance/IsAutoUpdateHeader").Value.ToString() == "True" ? true : false);
            t.TaskEntity.IsNoneAllowSplit = (doc.Root.XPathSelectElement("/Task/Advance/IsNoneAllowSplit").Value.ToString() == "True" ? true : false);
            t.TaskEntity.IsSplitDbUrls = (doc.Root.XPathSelectElement("/Task/Advance/IsSplitDbUrls").Value.ToString() == "True" ? true : false);

            //V5.3����
            t.TaskEntity.isCookieList = (doc.Root.XPathSelectElement("/Task/Advance/IsCookieList").Value.ToString() == "True" ? true : false);
            t.TaskEntity.GatherCount = int.Parse(doc.Root.XPathSelectElement("/Task/Advance/GatherCount").Value.ToString());
            t.TaskEntity.GatherCountPauseInterval = int.Parse(doc.Root.XPathSelectElement("/Task/Advance/GatherCountPauseInterval").Value.ToString());
            t.TaskEntity.RejectFlag = doc.Root.XPathSelectElement("/Task/Advance/StopFlag").Value.ToString();
            t.TaskEntity.RejectDeal = cGlobalParas.RejectDeal.None;

            t.TaskEntity.isSameSubTask = (doc.Root.XPathSelectElement("/Task/Advance/IsSameSubTask").Value.ToString() == "True" ? true : false);
            //V5.33
            t.TaskEntity.isGatherCoding = (doc.Root.XPathSelectElement("/Task/Advance/IsGatherCoding").Value.ToString() == "True" ? true : false);
            t.TaskEntity.GatherCodingFlag = doc.Root.XPathSelectElement("/Task/Advance/GatherCodingFlag").Value.ToString();
            t.TaskEntity.GatherCodingPlugin = doc.Root.XPathSelectElement("/Task/Advance/GatherCodingPlugin").Value.ToString();
            t.TaskEntity.CodeUrl = doc.Root.XPathSelectElement("/Task/Advance/CodingUrl").Value.ToString();

            //V5.5����
            t.TaskEntity.isCloseTab = false;
            #endregion

            //������V5.32��������⣬����汾���ˣ����汾��δ��������ɼ���
            //ʧ�ܣ����ԣ�����������

            IEnumerable<XElement> proxyXE = doc.Root.XPathSelectElement("/Task/ThreadProxy").Elements();

            foreach (XElement xe in proxyXE)
            {
                eThreadProxy tProxy = new eThreadProxy();
                tProxy.Index = int.Parse(xe.Element("Index").Value.ToString());
                tProxy.pType = (cGlobalParas.ProxyType)int.Parse(xe.Element("pType").Value.ToString());
                tProxy.Address = xe.Element("Address").Value.ToString();
                tProxy.Port = int.Parse(xe.Element("Port").Value.ToString());
                t.TaskEntity.ThreadProxy.Add(tProxy);
            }


            //����HTTP Header��Ϣ
            IEnumerable<XElement> headerXE = doc.Root.XPathSelectElement("/Task/HttpHeaders").Elements();
            eHeader header;

            foreach (XElement xe in headerXE)
            {
                header = new eHeader();
                header.Label = xe.Element("Label").Value.ToString();
                header.LabelValue = xe.Element("LabelValue").Value.ToString();
                header.Range = xe.Element("Range").Value.ToString();
                t.TaskEntity.Headers.Add(header);
            }

            //����Trigger��Ϣ
            IEnumerable<XElement> triggerXE = doc.Root.XPathSelectElement("/Task/Trigger").Elements();
            foreach (XElement xe in triggerXE)
            {
                eTriggerTask tt = new eTriggerTask();
                tt.RunTaskType = (cGlobalParas.RunTaskType)int.Parse(xe.Element("RunTaskType").Value.ToString());
                tt.RunTaskName = xe.Element("RunTaskName").Value.ToString();
                tt.RunTaskPara = xe.Element("RunTaskPara").Value.ToString();

                t.TaskEntity.TriggerTask.Add(tt);

            }

            //���ز������Ϣ
            t.TaskEntity.IsPluginsCookie = (doc.Root.XPathSelectElement("/Task/Plugins/IsPluginsCookie").Value.ToString() == "True" ? true : false);
            t.TaskEntity.PluginsCookie = doc.Root.XPathSelectElement("/Task/Plugins/PluginsCookie").Value.ToString();
            t.TaskEntity.IsPluginsDeal = (doc.Root.XPathSelectElement("/Task/Plugins/IsPluginsDeal").Value.ToString() == "True" ? true : false);
            t.TaskEntity.PluginsDeal = doc.Root.XPathSelectElement("/Task/Plugins/PluginsDeal").Value.ToString();
            t.TaskEntity.IsPluginsPublish = (doc.Root.XPathSelectElement("/Task/Plugins/IsPluginsPublish").Value.ToString() == "True" ? true : false);
            t.TaskEntity.PluginsPublish = doc.Root.XPathSelectElement("/Task/Plugins/PluginsPublish").Value.ToString();

            IEnumerable<XElement> weblinksXE = doc.Root.XPathSelectElement("/Task/WebLinks").Elements();

            int index = 0;
            foreach (XElement xe in weblinksXE)
            {
                eWebLink w = new eWebLink();
                w.id = index;
                w.Weblink = xe.Element("Url").Value.ToString();

                if (w.Weblink.StartsWith("{DbUrl"))
                {
                    isDBUrl = true;
                }
                w.IsNavigation = xe.Element("IsNag").Value.ToString() == "True" ? true : false;
                w.IsNextpage = xe.Element("IsNextPage").Value.ToString() == "True" ? true : false;
                w.NextPageRule = xe.Element("NextPageRule").Value.ToString();
                w.NextMaxPage = xe.Element("NextMaxPage").Value.ToString();
                w.NextPageUrl = xe.Element("NextPageUrl").Value.ToString();

                w.IsGathered = int.Parse((xe.Element("IsGathered").Value.ToString() == null
                    || xe.Element("IsGathered").Value.ToString() == "") ? "2031" : xe.Element("IsGathered").Value.ToString());

                #region ���ص�������
                if (xe.Element("NavigationRules") != null && xe.Element("NavigationRules").Elements("NavigationRule") != null)
                {
                    IEnumerable<XElement> navXE = xe.Element("NavigationRules").Elements("NavigationRule");

                    foreach (XElement xe1 in navXE)
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
                }
                #endregion
                w.IsMultiGather = xe.Element("IsMultiPageGather").Value.ToString() == "True" ? true : false;
                w.IsData121 = xe.Element("IsMulti121").Value.ToString() == "True" ? true : false;


                //���ض�ҳ�ɼ�����
                if (w.IsMultiGather == true)
                {
                    IEnumerable<XElement> multiXE = xe.Element("MultiPageRules").Elements("MultiPageRule");

                    //dn = dw[i].CreateChildView("WebLink_MultiPageRules")[0].CreateChildView("MultiPageRules_MultiPageRule");

                    foreach (XElement xe1 in multiXE)
                    {
                        eMultiPageRule nRule = new eMultiPageRule();
                        nRule.RuleName = xe1.Element("MultiRuleName").Value.ToString();
                        nRule.mLevel = int.Parse(xe1.Element("MultiLevel").Value.ToString());
                        nRule.Rule = xe1.Element("MultiRule").Value.ToString();
                        w.MultiPageRules.Add(nRule);
                    }
                }
                t.TaskEntity.WebpageLink.Add(w);
                w = null;
            }

            IEnumerable<XElement> gRuleXes = doc.Root.XPathSelectElement("/Task/GatherRules").Elements();
            foreach (XElement xe in gRuleXes)
            {
                eWebpageCutFlag c = new eWebpageCutFlag();
                c.Title = xe.Element("Title").Value.ToString();
                c.RuleByPage = (cGlobalParas.GatherRuleByPage)int.Parse((xe.Element("RuleByPage").Value.ToString() == null || xe.Element("RuleByPage").Value.ToString() == "") ? "0" : xe.Element("RuleByPage").Value.ToString());
                c.DataType = (cGlobalParas.GDataType)int.Parse((xe.Element("DataType").Value.ToString() == null || xe.Element("DataType").Value.ToString() == "") ? "0" : xe.Element("DataType").Value.ToString());

                c.GatherRuleType = (cGlobalParas.GatherRuleType)int.Parse((xe.Element("GatherRuleType").Value.ToString() == null || xe.Element("GatherRuleType").Value.ToString() == "") ? "0" : xe.Element("GatherRuleType").Value.ToString());
                c.XPath = xe.Element("XPath").Value.ToString();
                c.NodePrty = xe.Element("NodePrty").Value.ToString();

                c.StartPos = xe.Element("StartFlag").Value.ToString();
                c.EndPos = xe.Element("EndFlag").Value.ToString();
                c.LimitSign = (cGlobalParas.LimitSign)int.Parse((xe.Element("LimitSign").Value.ToString() == null || xe.Element("LimitSign").Value.ToString() == "") ? "0" : xe.Element("LimitSign").Value.ToString());
                c.RegionExpression = xe.Element("RegionExpression").Value.ToString();
                c.IsMergeData = xe.Element("IsMergeData").Value.ToString() == "True" ? true : false;
                c.NavLevel = int.Parse(xe.Element("NavLevel").Value.ToString());

                //�ɼ�����V2.6���ӣ����������ļ��Ĵ洢·������������
                c.DownloadFileSavePath = xe.Element("DownloadFileSavePath").Value.ToString();
                //c.DownloadFileDealType = dw[i].Row["DownloadFileDealType"].ToString();
                c.MultiPageName = xe.Element("MultiPageName").Value.ToString();

                //3.1����
                c.IsAutoDownloadFileImage = xe.Element("IsAutoDownloadImage").Value.ToString() == "True" ? true : false;
                c.IsAutoDownloadOnlyImage = xe.Element("IsAutoDownloadFirstImage").Value.ToString() == "True" ? true : false;

                //���������������

                IEnumerable<XElement> eRules = xe.Element("ExportRules").Elements("ExportRule");
                foreach (XElement xe1 in eRules)
                {
                    eFieldRule fRule = new eFieldRule();
                    fRule.Field = xe1.Element("ExortField").Value.ToString();
                    fRule.FieldRuleType = (cGlobalParas.ExportLimit)int.Parse(xe1.Element("ExortRuleType").Value.ToString());
                    fRule.FieldRule = xe1.Element("ExortRuleCondition").Value.ToString();

                    c.ExportRules.Add(fRule);
                }

                t.TaskEntity.WebpageCutFlag.Add(c);
                c = null;
            }

            //�����ַ��dburl�����¸���urlcount
            if (isDBUrl == true)
            {
                int urlsCount = 0;
                //����urlcount
                for (int urlIndex = 0; urlIndex < t.TaskEntity.WebpageLink.Count; urlIndex++)
                {
                    NetMiner.Core.Url.cUrlParse gUrl = new NetMiner.Core.Url.cUrlParse(this.m_workPath);

                    urlsCount += gUrl.GetUrlCount(t.TaskEntity.WebpageLink[urlIndex].Weblink.ToString());
                }

                t.TaskEntity.UrlCount = urlsCount;

            }

            t.SaveTask(FileName);
            t.Dispose();

            t = null;

        }

    }
}

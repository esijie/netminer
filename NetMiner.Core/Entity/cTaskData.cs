using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;
using NetMiner.Core.gTask.Entity;
using NetMiner.Net.Native;

///功能：采集任务数据
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
///这个是对应于UI的采集任务数据，将TaskRun的数据映射到里面来。
namespace NetMiner.Core.Entity
{ 
    /// <summary>
    /// 这是一个任务的基本数据，主要用于标识任务，与TaskRun中的一个数据进行映射
    /// 这个只是任务的索引，不是任务的信息。采集的时候，不以此数据为准
    /// </summary>
    public class cTaskData
    {
        public cTaskData()
        {
            TaskSplitData = new List<cTaskSplitData>();
        }

        ~cTaskData()
        {
            TaskSplitData = null;
        }

   
        public List<cTaskSplitData> TaskSplitData { get; set; }

        #region 核心数据
        /// <summary>
        /// 返回此采集任务的文件，是相对路径
        /// </summary>
        public string TaskFileName { get { return TaskClassPath + "\\" + TaskName + ".smt"; } }
        public Int64 TaskID { get; set; }
        public string TaskName { get; set; }
        public string TaskClass { get; set; }
        public string TaskClassPath  { get; set; }
        public string TaskDemo { get; set; }
        public cGlobalParas.TaskType TaskType { get; set; }
        public cGlobalParas.TaskRunType RunType { get; set; }
        public cGlobalParas.PublishType PublishType { get; set; }
        public cGlobalParas.TaskState TaskState { get; set; }
        /// <summary>
        /// 记录当前任务的执行进程，执行进程包括三个部分：采集、清洗、发布
        /// </summary>
        public cGlobalParas.TaskProcess Process { get; set; }
        #endregion

      
        /// <summary>
        /// 采集网页的数量，即配置的网址数量
        /// </summary>
        public int UrlCount { get; set; }
        /// <summary>
        /// 已经采集的数量，仅限于配置的网址，不包含导航出来的网址数量
        /// </summary>
        public int GatheredUrlCount { get; set; }

        /// <summary>
        /// 采集错误的网址，仅限于配置的网址，不包括导航出来的网址
        /// </summary>
        public int GatherErrUrlCount { get; set; }

        /// <summary>
        /// 根据导航页导航出来的网址数量，需要采集
        /// </summary>
        public int UrlNaviCount { get; set; }

        /// <summary>
        /// 根据导航规则，导航出来的网址，已经采集的数量
        /// </summary>
        public int GatheredUrlNaviCount { get; set; }

        /// <summary>
        /// 根据导航规则，导航出来的网址，采集发生错误的数量
        /// </summary>
        public int GatheredErrUrlNaviCount { get; set; }
    
        /// <summary>
        /// 记录采集的数量
        /// </summary>
        public int GatherDataCount { get; set; }

        public int ThreadCount { get; set; }

        public string TempDataFile { get; set; }

        public DateTime StartTimer { get; set; }

        public DateTime EndTime { get; set; }

        public int GatherTmpCount { get; set; }

        public int GatherCount { get; set; }



        //private cCookieManage m_Cookie;
        //public cCookieManage Cookie
        //{
        //    get { return m_Cookie; }
        //    set { m_Cookie = value; }
        //}

        //private cGlobalParas.WebCode m_WebCode;
        //public cGlobalParas.WebCode WebCode
        //{
        //    get { return m_WebCode; }
        //    set { m_WebCode = value; }
        //}

        //private bool m_IsUrlEncode;
        //public bool IsUrlEncode
        //{
        //    get { return m_IsUrlEncode; }
        //    set { m_IsUrlEncode = value; }
        //}

        //private bool m_IsTwoUrlCode;
        //public bool IsTwoUrlCode
        //{
        //    get { return m_IsTwoUrlCode; }
        //    set { m_IsTwoUrlCode = value; }
        //}

        ////为何要用string，因为有可能为空
        //private cGlobalParas.WebCode m_UrlEncode;
        //public cGlobalParas.WebCode UrlEncode
        //{
        //    get { return m_UrlEncode; }
        //    set { m_UrlEncode = value; }
        //}

        //private cGlobalParas.PublishType m_PublishType;
        //public cGlobalParas.PublishType PublishType
        //{
        //    get { return m_PublishType; }
        //    set { m_PublishType = value; }
        //}

        //private string m_DataSource;
        //public string DataSource
        //{
        //    get { return m_DataSource; }
        //    set { m_DataSource = value; }
        //}

        //private string m_TableName;
        //public string TableName
        //{
        //    get { return m_TableName; }
        //    set { m_TableName = value; }
        //}

        //private string m_InsertSql;
        //public string InsertSql
        //{
        //    get { return m_InsertSql; }
        //    set { m_InsertSql = value; }
        //}

        ////临时数据存储的位置
        ////此属性可用于扩展,即采集即存储,保存临时文件,
        ////这样做可以支持任务中断,重启之后即可实现继续采集
        ////但这样做可能会占用大量的资源,需要实际测试

        //private string m_SavePath;
        //public string SavePath
        //{
        //    get { return m_SavePath; }
        //    set { m_SavePath = value; }
        //}


        //重试次数
        //private int m_GatherAgainNumber;
        //public int GatherAgainNumber
        //{
        //    get { return m_GatherAgainNumber; }
        //    set { m_GatherAgainNumber = value; }
        //}

        //private bool m_IsIgnore404;
        //public bool IsIgnore404
        //{
        //    get { return m_IsIgnore404; }
        //    set { m_IsIgnore404 = value; }
        //}

        //private bool m_IsErrorLog;
        //public bool IsErrorLog
        //{
        //    get { return m_IsErrorLog; }
        //    set { m_IsErrorLog = value; }
        //}

        //private bool m_IsDelRepRow;
        //public bool IsDelRepRow
        //{
        //    get { return m_IsDelRepRow; }
        //    set { m_IsDelRepRow = value; }
        //}

        //private bool m_IsTrigger;
        //public bool IsTrigger
        //{
        //    get { return m_IsTrigger; }
        //    set { m_IsTrigger = value; }
        //}

        ////采集间隔延时
        //private float m_GIntervalTime;
        //public float GIntervalTime
        //{
        //    get { return m_GIntervalTime; }
        //    set { m_GIntervalTime = value; }
        //}

        //private float m_GIntervalTime1;
        //public float GIntervalTime1
        //{
        //    get { return m_GIntervalTime1; }
        //    set { m_GIntervalTime1 = value; }
        //}

        //private bool m_isMultiInterval;
        //public bool IsMultiInterval
        //{
        //    get { return m_isMultiInterval; }
        //    set { m_isMultiInterval = value; }
        //}

        //private string m_TriggerType;
        //public string TriggerType
        //{
        //    get { return m_TriggerType; }
        //    set { m_TriggerType = value; }
        //}

        //private List<eTriggerTask> m_TriggerTask;
        //public List<eTriggerTask> TriggerTask
        //{
        //    get { return m_TriggerTask; }
        //    set { m_TriggerTask = value; }
        //}

        /////以下任务信息是任务版本1.6新增
        //private bool m_IsDelTempData;
        //public bool IsDelTempData
        //{
        //    get { return m_IsDelTempData; }
        //    set { m_IsDelTempData = value; }
        //}

        //private bool m_IsSaveSingleFile;
        //public bool IsSaveSingleFile
        //{
        //    get { return m_IsSaveSingleFile; }
        //    set { m_IsSaveSingleFile = value; }
        //}


        //private bool m_IsExportGUrl;
        //public bool IsExportGUrl
        //{
        //    get { return m_IsExportGUrl; }
        //    set { m_IsExportGUrl = value; }
        //}

        //private bool m_IsExportGDateTime;
        //public bool IsExportGDateTime
        //{
        //    get { return m_IsExportGDateTime; }
        //    set { m_IsExportGDateTime = value; }
        //}

        ////以下信息为1.8版本增加
        //private bool m_IsCustomHeader;
        //public bool IsCustomHeader
        //{
        //    get { return m_IsCustomHeader; }
        //    set { m_IsCustomHeader = value; }
        //}

        //private bool m_IsPublishHeader;
        //public bool IsPublishHeader
        //{
        //    get { return m_IsPublishHeader; }
        //    set { m_IsPublishHeader = value; }
        //}

        //private List<eHeader> m_Headers;
        //public List<eHeader> Headers
        //{
        //    get { return m_Headers; }
        //    set { m_Headers = value; }
        //}

        ////以下信息是V2.0增加的
        //private bool m_IsProxy;
        //public bool IsProxy
        //{
        //    get { return m_IsProxy; }
        //    set { m_IsProxy = value; }
        //}

        //private bool m_IsProxyFirst;
        //public bool IsProxyFirst
        //{
        //    get { return m_IsProxyFirst; }
        //    set { m_IsProxyFirst = value; }
        //}

        //private bool m_IsSilent;
        //public bool IsSilent
        //{
        //    get { return m_IsSilent; }
        //    set { m_IsSilent = value; }
        //}

        ////以下为V2.1增加
        //private bool m_IsUrlNoneRepeat;
        //public bool IsUrlNoneRepeat
        //{
        //    get { return m_IsUrlNoneRepeat; }
        //    set { m_IsUrlNoneRepeat = value; }
        //}

        ////以下为V5增加内容
        //private bool m_IsPluginsCookie;
        //public bool IsPluginsCookie
        //{
        //    get { return m_IsPluginsCookie; }
        //    set { m_IsPluginsCookie = value; }
        //}

        //private string m_PluginsCookie;
        //public string PluginsCookie
        //{
        //    get { return m_PluginsCookie; }
        //    set { m_PluginsCookie = value; }
        //}

        //private bool m_IsPluginsDeal;
        //public bool IsPluginsDeal
        //{
        //    get { return m_IsPluginsDeal; }
        //    set { m_IsPluginsDeal = value; }
        //}

        //private string m_PluginsDeal;
        //public string PluginsDeal
        //{
        //    get { return m_PluginsDeal; }
        //    set { m_PluginsDeal = value; }
        //}

        //private bool m_IsPluginsPublish;
        //public bool IsPluginsPublish
        //{
        //    get { return m_IsPluginsPublish; }
        //    set { m_IsPluginsPublish = value; }
        //}

        //private string m_PluginsPublish;
        //public string PluginsPublish
        //{
        //    get { return m_PluginsPublish; }
        //    set { m_PluginsPublish = value; }
        //}

        //private bool m_IsUrlAutoRedirect;
        //public bool IsUrlAutoRedirect
        //{
        //    get { return m_IsUrlAutoRedirect; }
        //    set { m_IsUrlAutoRedirect = value; }
        //}

        ////以下为V5.0.1增加
        //private bool m_IsSqlTrue;
        //public bool IsSqlTrue
        //{
        //    get { return m_IsSqlTrue; }
        //    set { m_IsSqlTrue = value; }
        //}

        ////以下为V5.1增加
        //private bool m_IsGatherErrStop;
        //public bool IsGatherErrStop
        //{
        //    get { return m_IsGatherErrStop; }
        //    set { m_IsGatherErrStop = value; }
        //}

        //private int m_GatherErrStopConut;
        //public int GatherErrStopCount
        //{
        //    get { return m_GatherErrStopConut; }
        //    set { m_GatherErrStopConut = value; }
        //}

        //private cGlobalParas.StopRule m_GatherErrStopRule;
        //public cGlobalParas.StopRule GatherErrStopRule
        //{
        //    get { return m_GatherErrStopRule; }
        //    set { m_GatherErrStopRule = value; }
        //}

        //private bool m_IsInsertDataErrStop;
        //public bool IsInsertDataErrStop
        //{
        //    get { return m_IsInsertDataErrStop; }
        //    set { m_IsInsertDataErrStop = value; }
        //}

        //private int m_InsertDataErrStopConut;
        //public int InsertDataErrStopConut
        //{
        //    get { return m_InsertDataErrStopConut; }
        //    set { m_InsertDataErrStopConut = value; }
        //}

        //private bool m_IsGatherRepeatStop;
        //public bool IsGatherRepeatStop
        //{
        //    get { return m_IsGatherRepeatStop; }
        //    set { m_IsGatherRepeatStop = value; }
        //}

        //private cGlobalParas.StopRule m_GatherRepeatStopRule;
        //public cGlobalParas.StopRule GatherRepeatStopRule
        //{
        //    get { return m_GatherRepeatStopRule; }
        //    set { m_GatherRepeatStopRule = value; }
        //}

        ////以下为V5.2增加
        //private bool m_IsIgnoreErr;
        //public bool IsIgnoreErr
        //{
        //    get { return m_IsIgnoreErr; }
        //    set { m_IsIgnoreErr = value; }
        //}

        //private bool m_IsAutoUpdateHeader;
        //public bool IsAutoUpdateHeader
        //{
        //    get { return m_IsAutoUpdateHeader; }
        //    set { m_IsAutoUpdateHeader = value; }
        //}


        //#region 网页采集的地址和规则

        //private List<eWebLink> m_Weblink;
        //public List<eWebLink> Weblink
        //{
        //    get { return m_Weblink; }
        //    set { m_Weblink = value; }
        //}

        //private List<eWebpageCutFlag> m_CutFlag;
        //public List<eWebpageCutFlag> CutFlag
        //{
        //    get { return m_CutFlag; }
        //    set { m_CutFlag = value; }
        //}

        ////任务分解数据
        //private List<cTaskSplitData> m_TaskSplitData;
        //public List<cTaskSplitData> TaskSplitData
        //{
        //    get { return m_TaskSplitData; }
        //    set { m_TaskSplitData = value; }
        //}

        ////以下为V3.0增加
        //private int m_AutoID;
        //public int AutoID
        //{
        //    get { return m_AutoID; }
        //    set { m_AutoID = value; }
        //}

        //private bool m_IsSucceedUrlRepeat;
        //public bool IsSucceedUrlRepeat
        //{
        //    get { return m_IsSucceedUrlRepeat; }
        //    set { m_IsSucceedUrlRepeat = value; }
        //}

        //#endregion

        //#region 网页采集的起始和终止地址

        //private string m_StartPos;
        //public string StartPos
        //{
        //    get { return m_StartPos; }
        //    set { m_StartPos = value; }
        //}

        //private string m_EndPos;
        //public string EndPos
        //{
        //    get { return m_EndPos; }
        //    set { m_EndPos = value; }
        //}

        //#endregion





        //临时计数器，用于每次记录采集多少条数据后，停止一段
        //时间的采集，并清空，重新记录



        //以下为5.31增加
        //private bool m_isCookieList;
        //public bool isCookieList
        //{
        //    get { return m_isCookieList; }
        //    set { m_isCookieList = value; }
        //}

        //private float m_GatherCountPauseInterval;
        //public float GatherCountPauseInterval
        //{
        //    get { return m_GatherCountPauseInterval; }
        //    set { m_GatherCountPauseInterval = value; }
        //}

        //private string m_StopFlag;
        //public string StopFlag
        //{
        //    get { return m_StopFlag; }
        //    set { m_StopFlag = value; }
        //}

        //private bool m_IsThrowGatherErr;
        //public bool IsThrowGatherErr
        //{
        //    get { return m_IsThrowGatherErr; }
        //    set { m_IsThrowGatherErr = value; }
        //}

        ////v5.33增加
        //private List<eThreadProxy> m_ThreadProxy;
        //public List<eThreadProxy> ThreadProxy
        //{
        //    get { return m_ThreadProxy; }
        //    set { m_ThreadProxy = value; }
        //}

        //private bool m_isSameSubTask;
        //public bool isSameSubTask
        //{
        //    get { return m_isSameSubTask; }
        //    set { m_isSameSubTask = value; }
        //}

        //private bool m_isUserServerDB;
        //public bool isUserServerDB
        //{
        //    get { return m_isUserServerDB; }
        //    set { m_isUserServerDB = value; }
        //}

        //private bool m_isGatherCoding;
        //public bool isGatherCoding
        //{
        //    get { return m_isGatherCoding; }
        //    set { m_isGatherCoding = value; }
        //}

        //private string m_GatherCodingFlag;
        //public string GatherCodingFlag
        //{
        //    get { return m_GatherCodingFlag; }
        //    set { m_GatherCodingFlag = value; }
        //}

        //private string m_GatherCodinPlugin;
        //public string GatherCodingPlugin
        //{
        //    get { return m_GatherCodinPlugin; }
        //    set { m_GatherCodinPlugin = value; }
        //}

        //private string m_codeUrl;
        //public string CodeUrl
        //{
        //    get { return m_codeUrl; }
        //    set { m_codeUrl = value; }
        //}

        //private bool m_IsVisual;
        //public bool IsVisual
        //{
        //    get { return m_IsVisual; }
        //    set { m_IsVisual = value; }
        //}

        //private string m_RejectFlag;
        //public string RejectFlag
        //{
        //    get { return m_RejectFlag; }
        //    set { m_RejectFlag = value; }
        //}

        //public cGlobalParas.RejectDeal RejectDeal { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Resource;
using NetMiner.Core.Entity;
using System.Data;
using NetMiner.Core.gTask.Entity;

/// <summary>
/// 这个类是一个子线程执行时的数据类
/// </summary>
namespace NetMiner.Gather.Control
{
    public class eGatherTaskSplitData
    {
        #region 需要初始化类的属性

        private int m_ErrorCount;
        public int ErrorCount
        {
            get { return m_ErrorCount; }
        }

        private bool m_IsInitialized;
        internal bool IsInitialized
        {
            get { return m_IsInitialized; }
            set { m_IsInitialized = value; }
        }

        private int m_Waittime;
        internal int Waittime
        {
            get { return m_Waittime; }
            set { m_Waittime = value; }
        }

        private Int64 m_TaskID;
        public Int64 TaskID
        {
            get { return m_TaskID; }
            set { m_TaskID = value; }
        }

        private string m_TaskName;
        public string TaskName
        {
            get { return m_TaskName; }
            set { m_TaskName = value; }
        }

        private string m_gStartPos;
        public string StartPos
        {
            get { return m_gStartPos; }
            set { m_gStartPos = value; }
        }

        private string m_gEndPos;
        public string EndPos
        {
            get { return m_gEndPos; }
            set { m_gEndPos = value; }
        }
        private KeyValuePair<int, string> m_Cookie;
        public KeyValuePair<int, string> Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }

        private cTaskSplitData m_TaskSplitData;
        public cTaskSplitData TaskSplitData
        {
            get { return m_TaskSplitData; }
            set { m_TaskSplitData = value; }
        }

        private cGlobalParas.TaskRunType m_TaskRunType;
        public cGlobalParas.TaskRunType TaskRunType
        {
            get { return m_TaskRunType; }
            set { m_TaskRunType = value; }
        }

        private cGlobalParas.TaskType m_TaskType;
        public cGlobalParas.TaskType TaskType
        {
            get { return m_TaskType; }
            set { m_TaskType = value; }
        }

        private cGlobalParas.WebCode m_WebCode;
        public cGlobalParas.WebCode WebCode
        {
            get { return m_WebCode; }
            set { m_WebCode = value; }
        }

        private bool m_IsUrlEncode;
        public bool IsUrlEncode
        {
            get { return m_IsUrlEncode; }
            set { m_IsUrlEncode = value; }
        }

        private bool m_IsTwoUrlCode;
        public bool IsTwoUrlCode
        {
            get { return m_IsTwoUrlCode; }
            set { m_IsTwoUrlCode = value; }
        }

        private cGlobalParas.WebCode m_UrlEncode;
        public cGlobalParas.WebCode UrlEncode
        {
            get { return m_UrlEncode; }
            set { m_UrlEncode = value; }
        }

        private cGatherManage m_TaskManage;
        public cGatherManage TaskManage
        {
            get { return m_TaskManage; }
            set { m_TaskManage = value; }
        }

        private DataTable m_GatherData;
        public DataTable GatherData
        {
            get { return m_GatherData; }
            set { m_GatherData = value; }
        }

        private string m_SavePath;
        public string SavePath
        {
            get { return m_SavePath; }
            set { m_SavePath = value; }
        }

        private int m_AgainNumber;
        public int AgainNumber
        {
            get { return m_AgainNumber; }
            set { m_AgainNumber = value; }
        }

        private bool m_Ignore404;
        public bool Ignore404
        {
            get { return m_Ignore404; }
            set { m_Ignore404 = value; }
        }

        private bool m_IsErrorLog;
        public bool IsErrorLog
        {
            get { return m_IsErrorLog; }
            set { m_IsErrorLog = value; }
        }

        //暂未用
        private bool m_IsDelRepRow;
        public bool IsDelRepRow
        {
            get { return m_IsDelRepRow; }
            set { m_IsDelRepRow = value; }
        }

        private bool m_IsExportGUrl;
        public bool IsExportGUrl
        {
            get { return m_IsExportGUrl; }
            set { m_IsExportGUrl = value; }
        }

        private bool m_IsExportGDateTime;
        public bool IsExportGDateTime
        {
            get { return m_IsExportGDateTime; }
            set { m_IsExportGDateTime = value; }
        }

        private cGlobalParas.PublishType m_PublishType;
        public cGlobalParas.PublishType PublishType
        {
            get { return m_PublishType; }
            set { m_PublishType = value; }
        }

        private string m_DataSource;
        public string DataSource
        {
            get { return m_DataSource; }
            set { m_DataSource = value; }
        }

        private string m_TableName;
        public string TableName
        {
            get { return m_TableName; }
            set { m_TableName = value; }
        }

        private string m_InsertSql;
        public string InsertSql
        {
            get { return m_InsertSql; }
            set { m_InsertSql = value; }
        }


        //根据传入的采集规则，判断自动翻页的网址是否需要进行数据合并
        private bool m_IsMergeData;
        public bool IsMergeData
        {
            get { return m_IsMergeData; }
            set { m_IsMergeData = value; }
        }

        //采集间隔延时
        private float m_GIntervalTime;
        public float GIntervalTime
        {
            get { return m_GIntervalTime; }
            set { m_GIntervalTime = value; }
        }

        private float m_GIntervalTime1;
        public float GIntervalTime1
        {
            get { return m_GIntervalTime1; }
            set { m_GIntervalTime1 = value; }
        }

        private bool m_isMultiInterval;
        public bool IsMultiInterval
        {
            get { return m_isMultiInterval; }
            set { m_isMultiInterval = value; }
        }


        //任务1.8增加，判断是否需要进行自定义的Header
        private bool m_IsCustomHeader;
        public bool IsCustomHeader
        {
            get { return m_IsCustomHeader; }
            set { m_IsCustomHeader = value; }
        }

        private bool m_IsPublishHeader;
        public bool IsPublishHeader
        {
            get { return m_IsPublishHeader; }
            set { m_IsPublishHeader = value; }
        }

        private List<eHeader> m_Headers;
        public List<eHeader> Headers
        {
            get { return m_Headers; }
            set { m_Headers = value; }
        }

        //V2.0增加的属性
        private bool m_IsProxy;
        public bool IsProxy
        {
            get { return m_IsProxy; }
            set { m_IsProxy = value; }
        }

        private bool m_IsProxyFirst;
        public bool IsProxyFirst
        {
            get { return m_IsProxyFirst; }
            set { m_IsProxyFirst = value; }
        }

        private bool m_IsSilent;
        public bool IsSilent
        {
            get { return m_IsSilent; }
            set { m_IsSilent = value; }
        }

        //V2.1增加的属性
        private bool m_IsUrlNoneRepeat;
        public bool IsUrlNoneRepeat
        {
            get { return m_IsUrlNoneRepeat; }
            set { m_IsUrlNoneRepeat = value; }
        }

        //V3.0增加
        private int m_AutoID;
        public int AutoID
        {
            get { return m_AutoID; }
            set { m_AutoID = value; }
        }

        //以下为V5增加内容
        private bool m_IsPluginsCookie;
        public bool IsPluginsCookie
        {
            get { return m_IsPluginsCookie; }
            set { m_IsPluginsCookie = value; }
        }

        private string m_PluginsCookie;
        public string PluginsCookie
        {
            get { return m_PluginsCookie; }
            set { m_PluginsCookie = value; }
        }

        private bool m_IsPluginsDeal;
        public bool IsPluginsDeal
        {
            get { return m_IsPluginsDeal; }
            set { m_IsPluginsDeal = value; }
        }

        private string m_PluginsDeal;
        public string PluginsDeal
        {
            get { return m_PluginsDeal; }
            set { m_PluginsDeal = value; }
        }

        private bool m_IsPluginsPublish;
        public bool IsPluginsPublish
        {
            get { return m_IsPluginsPublish; }
            set { m_IsPluginsPublish = value; }
        }

        private string m_PluginsPublish;
        public string PluginsPublish
        {
            get { return m_PluginsPublish; }
            set { m_PluginsPublish = value; }
        }

        private bool m_IsUrlAutoRedirect;
        public bool IsUrlAutoRedirect
        {
            get { return m_IsUrlAutoRedirect; }
            set { m_IsUrlAutoRedirect = value; }
        }

        //以下为V5.0.1增加
        private bool m_IsSqlTrue;
        public bool IsSqlTrue
        {
            get { return m_IsSqlTrue; }
            set { m_IsSqlTrue = value; }
        }

        //以下为V5.1增加
        private bool m_IsGatherErrStop;
        public bool IsGatherErrStop
        {
            get { return m_IsGatherErrStop; }
            set { m_IsGatherErrStop = value; }
        }

        private int m_GatherErrStopConut;
        public int GatherErrStopCount
        {
            get { return m_GatherErrStopConut; }
            set { m_GatherErrStopConut = value; }
        }

        private cGlobalParas.StopRule m_GatherErrStopRule;
        public cGlobalParas.StopRule GatherErrStopRule
        {
            get { return m_GatherErrStopRule; }
            set { m_GatherErrStopRule = value; }
        }

        private bool m_IsInsertDataErrStop;
        public bool IsInsertDataErrStop
        {
            get { return m_IsInsertDataErrStop; }
            set { m_IsInsertDataErrStop = value; }
        }

        private int m_InsertDataErrStopConut;
        public int InsertDataErrStopConut
        {
            get { return m_InsertDataErrStopConut; }
            set { m_InsertDataErrStopConut = value; }
        }

        private bool m_IsGatherRepeatStop;
        public bool IsGatherRepeatStop
        {
            get { return m_IsGatherRepeatStop; }
            set { m_IsGatherRepeatStop = value; }
        }

        private cGlobalParas.StopRule m_GatherRepeatStopRule;
        public cGlobalParas.StopRule GatherRepeatStopRule
        {
            get { return m_GatherRepeatStopRule; }
            set { m_GatherRepeatStopRule = value; }
        }

        private bool m_IsSucceedUrlRepeat;
        public bool IsSucceedUrlRepeat
        {
            get { return m_IsSucceedUrlRepeat; }
            set { m_IsSucceedUrlRepeat = value; }
        }

        //以下为V5.2增加
        private bool m_IsIgnoreErr;
        public bool IsIgnoreErr
        {
            get { return m_IsIgnoreErr; }
            set { m_IsIgnoreErr = value; }
        }

        private bool m_IsAutoUpdateHeader;
        public bool IsAutoUpdateHeader
        {
            get { return m_IsAutoUpdateHeader; }
            set { m_IsAutoUpdateHeader = value; }
        }

        //以下为5.31增加
        private bool m_isCookieList;
        public bool isCookieList
        {
            get { return m_isCookieList; }
            set { m_isCookieList = value; }
        }

        private int m_GatherCount;
        public int GatherCount
        {
            get { return m_GatherCount; }
            set { m_GatherCount = value; }
        }

        private float m_GatherCountPauseInterval;
        private float GatherCountPauseInterval
        {
            get { return m_GatherCountPauseInterval; }
        }

        private string m_RejectFlag;
        public string RejectFlag
        {
            get { return m_RejectFlag; }
            set { m_RejectFlag = value; }
        }

        private cGlobalParas.RejectDeal m_RejectDeal;
        public cGlobalParas.RejectDeal RejectDeal
        {
            get { return m_RejectDeal; }
            set { m_RejectDeal = value; }
        }

        #region 每个线程的代理设置,这个信息在任务信息中是以list保存，在分解采集任务时，进行分配
        //为每个线程独立设置的代理信息
        private cGlobalParas.ProxyType m_pType;
        public cGlobalParas.ProxyType pType
        {
            get { return m_pType; }
            set { m_pType = value; }
        }

        private string m_ProxyAddress;
        public string ProxyAddress
        {
            get { return m_ProxyAddress; }
            set { m_ProxyAddress = value; }
        }

        private int m_ProxyPort;
        public int ProxyPort
        {
            get { return m_ProxyPort; }
            set { m_ProxyPort = value; }
        }

        private bool m_isGatherCoding;
        public bool isGatherCoding
        {
            get { return m_isGatherCoding; }
            set { m_isGatherCoding = value; }
        }

        private string m_GatherCodingFlag;
        public string GatherCodingFlag
        {
            get { return m_GatherCodingFlag; }
            set { m_GatherCodingFlag = value; }
        }

        private string m_GatherCodinPlugin;
        public string GatherCodingPlugin
        {
            get { return m_GatherCodinPlugin; }
            set { m_GatherCodinPlugin = value; }
        }

        private string m_codeUrl;
        public string CodeUrl
        {
            get { return m_codeUrl; }
            set { m_codeUrl = value; }
        }
        #endregion


        //5.5增加
        private bool m_IsVisual;
        public bool IsVisual
        {
            get { return m_IsVisual; }
            set { m_IsVisual = value; }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Resource;

/// <summary>
/// 2018-1-29
/// 从5.5版本，开始分离实体类与操作类，同时此版本结构发生了较大的变化。
/// 主要取掉了前版本无需或很少使用的标记，增加了功能操作，双引擎操作。
/// </summary>
namespace NetMiner.Gather.Task.Entity
{
    public class eTask
    {
        private string m_workPath = string.Empty;

        #region 类的构造和销毁
        public eTask(string workPath)
        {
            m_workPath = workPath;
            this.WebpageLink = new List<cWebLink>();
            this.WebpageCutFlag = new List<cWebpageCutFlag>();
            this.TriggerTask = new List<cTriggerTask>();
            this.Headers = new List<eHeader>();
            this.PublishParas = new List<cPublishPara>();
            this.ThreadProxy = new List<cThreadProxy>();
        }

        ~eTask()
        {
            this.WebpageLink = null;
            this.WebpageCutFlag = null;
            this.TriggerType = null;
            this.Headers = null;
            this.PublishParas = null;
        }
        #endregion

        #region TaskProperty

        //以下为任务当前状态的属性，如果是新建任务，则应该为未启动
        //此属性有待考虑，当前未用
        public int TaskState
        {
            get { return this.TaskState; }
            set { this.TaskState = value; }
        }

        //*****************************************************************************************************************
        //以下定义为Task属性

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

        /// <summary>
        /// 任务版本信息，当软件升级时，任务的版本也会升级，所以需要对每一个任务
        /// 版本进行识别，便于任务数据的迁移，理论上是兼容上一版本的任务数据信息，
        /// 但如果版本跨度太大，则不会进行兼容，由专用工具实现任务数据迁移
        /// 此版本任务版本号为：1.2，比较1.0版本，主要是增加了数数据加工及导出操作。
        /// </summary>
        private Single m_TaskVersion;
        public Single TaskVersion
        {
            get { return m_TaskVersion; }
            set { m_TaskVersion = value; }
        }

        private string m_TaskDemo;
        public string TaskDemo
        {
            get { return m_TaskDemo; }
            set { m_TaskDemo = value; }
        }

        private string m_TaskClass;
        public string TaskClass
        {
            get { return m_TaskClass; }
            set { m_TaskClass = value; }
        }

        private cGlobalParas.TaskType m_TaskType;
        public cGlobalParas.TaskType TaskType
        {
            get { return m_TaskType; }
            set { m_TaskType = value; }
        }

        private string m_SavePath;
        public string SavePath
        {
            get { return m_SavePath; }
            set { m_SavePath = value; }
        }

        private string m_TaskTemplate;
        public string TaskTemplate
        {
            get { return m_TaskTemplate; }
            set { m_TaskTemplate = value; }
        }

        private cGlobalParas.TaskRunType m_RunType;
        public cGlobalParas.TaskRunType RunType
        {
            get { return m_RunType; }
            set { m_RunType = value; }
        }

        private int m_UrlCount;
        public int UrlCount
        {
            get { return m_UrlCount; }
            set { m_UrlCount = value; }
        }

        private string m_DemoUrl;
        public string DemoUrl
        {
            get { return m_DemoUrl; }
            set { m_DemoUrl = value; }
        }

        private string m_Cookie;
        public string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
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

        private cGlobalParas.WebCode m_UrlEncode;
        public cGlobalParas.WebCode UrlEncode
        {
            get { return m_UrlEncode; }
            set { m_UrlEncode = value; }
        }

        //导出数据仅支持Access与MSsqlserver
        private cGlobalParas.PublishType m_ExportType;
        public cGlobalParas.PublishType ExportType
        {
            get { return m_ExportType; }
            set { m_ExportType = value; }
        }

        private string m_ExportFile;
        public string ExportFile
        {
            get { return m_ExportFile; }
            set { m_ExportFile = value; }
        }

        private string m_DataSource;
        public string DataSource
        {
            get { return m_DataSource; }
            set { m_DataSource = value; }
        }

        private string m_DataTableName;
        public string DataTableName
        {
            get { return m_DataTableName; }
            set { m_DataTableName = value; }
        }

        private string m_InsertSql;
        public string InsertSql
        {
            get { return m_InsertSql; }
            set { m_InsertSql = value; }
        }

        private int m_ThreadCount;
        public int ThreadCount
        {
            get { return m_ThreadCount; }
            set { m_ThreadCount = value; }
        }

        //采集页面内容的起始位置
        private string m_StartPos;
        public string StartPos
        {
            get { return m_StartPos; }
            set { m_StartPos = value; }
        }

        private string m_EndPos;
        public string EndPos
        {
            get { return m_EndPos; }
            set { m_EndPos = value; }
        }

        private List<cWebLink> m_WebpageLink;
        public List<cWebLink> WebpageLink
        {
            get { return m_WebpageLink; }
            set { m_WebpageLink = value; }
        }

        private List<cWebpageCutFlag> m_WebpageCutFlag;
        public List<cWebpageCutFlag> WebpageCutFlag
        {
            get { return m_WebpageCutFlag; }
            set { m_WebpageCutFlag = value; }
        }

        //以下为新增内容，主要是任务版本升级，为1.3
        private int m_GatherAgainNumber;
        public int GatherAgainNumber
        {
            get { return m_GatherAgainNumber; }
            set { m_GatherAgainNumber = value; }
        }

        private bool m_IsIgnore404;
        public bool IsIgnore404
        {
            get { return m_IsIgnore404; }
            set { m_IsIgnore404 = value; }
        }

        private bool m_IsExportHeader;
        public bool IsExportHeader
        {
            get { return m_IsExportHeader; }
            set { m_IsExportHeader = value; }
        }

        //是否输出错误信息
        private bool m_IsErrorLog;
        public bool IsErrorLog
        {
            get { return m_IsErrorLog; }
            set { m_IsErrorLog = value; }
        }

        //是否去除重复的行
        private bool m_IsDelRepRow;
        public bool IsDelRepRow
        {
            get { return m_IsDelRepRow; }
            set { m_IsDelRepRow = value; }
        }

        private bool m_IsTrigger;
        public bool IsTrigger
        {
            get { return m_IsTrigger; }
            set { m_IsTrigger = value; }
        }

        private string m_TriggerType;
        public string TriggerType
        {
            get { return m_TriggerType; }
            set { m_TriggerType = value; }
        }

        private List<cTriggerTask> m_TriggerTask;
        public List<cTriggerTask> TriggerTask
        {
            get { return m_TriggerTask; }
            set { m_TriggerTask = value; }
        }

        ///以下任务信息是任务版本1.6新增
        private bool m_IsDelTempData;
        public bool IsDelTempData
        {
            get { return m_IsDelTempData; }
            set { m_IsDelTempData = value; }
        }

        private bool m_IsSaveSingleFile;
        public bool IsSaveSingleFile
        {
            get { return m_IsSaveSingleFile; }
            set { m_IsSaveSingleFile = value; }
        }

        private string m_TempDataFile;
        public string TempDataFile
        {
            get { return m_TempDataFile; }
            set { m_TempDataFile = value; }
        }

        private bool m_IsDataProcess;
        public bool IsDataProcess
        {
            get { return m_IsDataProcess; }
            set { m_IsDataProcess = value; }
        }

        //是否导出采集Url地址
        private bool m_IsExportGUrl;
        public bool IsExportGUrl
        {
            get { return m_IsExportGUrl; }
            set { m_IsExportGUrl = value; }
        }

        //是否导出采集的时间
        private bool m_IsExportGDateTime;
        public bool IsExportGDateTime
        {
            get { return m_IsExportGDateTime; }
            set { m_IsExportGDateTime = value; }
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


        //采集任务V1.8 增加属性 HTTP Header
        private bool m_IsCustomHeader;
        public bool IsCustomHeader
        {
            get { return m_IsCustomHeader; }
            set { m_IsCustomHeader = value; }
        }

        private List<eHeader> m_Headers;
        public List<eHeader> Headers
        {
            get { return m_Headers; }
            set { m_Headers = value; }
        }

        //采集任务V2.0增加，代理和静默的设置
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

        //采集任务V2.1增加，是否进行Url排重
        private bool m_IsUrlNoneRepeat;
        public bool IsUrlNoneRepeat
        {
            get { return m_IsUrlNoneRepeat; }
            set { m_IsUrlNoneRepeat = value; }
        }

        //采集任务V3.6增加
        private int m_PIntervalTime;
        public int PIntervalTime
        {
            get { return m_PIntervalTime; }
            set { m_PIntervalTime = value; }
        }

        private string m_PSucceedFlag;
        public string PSucceedFlag
        {
            get { return m_PSucceedFlag; }
            set { m_PSucceedFlag = value; }
        }

        private int m_PublishThread;
        public int PublishThread
        {
            get { return m_PublishThread; }
            set { m_PublishThread = value; }
        }


        //V5增加的功能
        //旗舰版和企业版才具备的功能
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

        #region 水印设置
        //private bool m_IsWatermark;
        //public bool IsWatermark
        //{
        //    get { return m_IsWatermark; }
        //    set { m_IsWatermark = value; }
        //}

        //private string m_WatermarkText;
        //public string WatermarkText
        //{
        //    get { return m_WatermarkText; }
        //    set { m_WatermarkText = value; }
        //}

        //private string m_FontFamily;
        //public string FontFamily
        //{
        //    get { return m_FontFamily; }
        //    set { m_FontFamily = value; }
        //}

        //private int m_FontSize;
        //public int FontSize
        //{
        //    get { return m_FontSize; }
        //    set { m_FontSize = value; }
        //}

        //private bool m_IsFontWeight;
        //public bool IsFontWeight
        //{
        //    get { return m_IsFontWeight; }
        //    set { m_IsFontWeight = value; }
        //}

        //private bool m_IsFontItalic;
        //public bool IsFontItalic
        //{
        //    get { return m_IsFontItalic; }
        //    set { m_IsFontItalic = value; }
        //}

        //private string m_FontColor;
        //public string FontColor
        //{
        //    get { return m_FontColor; }
        //    set { m_FontColor = value; }
        //}

        //private int m_WatermarkPOS;
        //public int WatermarkPOS
        //{
        //    get { return m_WatermarkPOS; }
        //    set { m_WatermarkPOS = value; }
        //}
        #endregion

        //发布模版的信息
        private string m_TemplateName;
        public string TemplateName
        {
            get { return m_TemplateName; }
            set { m_TemplateName = value; }
        }

        private string m_User;
        public string User
        {
            get { return m_User; }
            set { m_User = value; }
        }

        private string m_Password;
        public string Password
        {
            get { return m_Password; }
            set { m_Password = value; }
        }

        private string m_Domain;
        public string Domain
        {
            get { return m_Domain; }
            set { m_Domain = value; }
        }

        private string m_TemplateDBConn;
        public string TemplateDBConn
        {
            get { return m_TemplateDBConn; }
            set { m_TemplateDBConn = value; }
        }

        private List<cPublishPara> m_PublishParas;
        public List<cPublishPara> PublishParas
        {
            get { return m_PublishParas; }
            set { m_PublishParas = value; }
        }

        //V5.0.1增加
        private bool m_IsSqlTrue;
        public bool IsSqlTrue
        {
            get { return m_IsSqlTrue; }
            set { m_IsSqlTrue = value; }
        }

        //V5.1增加
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

        private bool m_IsNoneAllowSplit;
        public bool IsNoneAllowSplit
        {
            get { return m_IsNoneAllowSplit; }
            set { m_IsNoneAllowSplit = value; }
        }

        private cGlobalParas.StopRule m_GatherRepeatStopRule;
        public cGlobalParas.StopRule GatherRepeatStopRule
        {
            get { return m_GatherRepeatStopRule; }
            set { m_GatherRepeatStopRule = value; }
        }

        private bool m_IsRowFile;
        public bool IsRowFile
        {
            get { return m_IsRowFile; }
            set { m_IsRowFile = value; }
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

        private bool m_IsSplitDbUrls;
        /// <summary>
        /// 如分布式执行，则仅分解由数据库提取的网站，这样做的目的，是确保如果有导航采集及分页采集时，数据的准确性。
        /// </summary>
        public bool IsSplitDbUrls
        {
            get { return m_IsSplitDbUrls; }
            set { m_IsSplitDbUrls = value; }
        }

        private bool m_IsTwoUrlCode;
        public bool IsTwoUrlCode
        {
            get { return m_IsTwoUrlCode; }
            set { m_IsTwoUrlCode = value; }
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
        public float GatherCountPauseInterval
        {
            get { return m_GatherCountPauseInterval; }
            set { m_GatherCountPauseInterval = value; }
        }

        private string m_StopFlag;
        public string StopFlag
        {
            get { return m_StopFlag; }
            set { m_StopFlag = value; }
        }

        private bool m_IsThrowGatherErr;
        public bool IsThrowGatherErr
        {
            get { return m_IsThrowGatherErr; }
            set { m_IsThrowGatherErr = value; }
        }

        //v5.33增加
        private List<cThreadProxy> m_ThreadProxy;
        public List<cThreadProxy> ThreadProxy
        {
            get { return m_ThreadProxy; }
            set { m_ThreadProxy = value; }
        }

        //v5.33增加
        private bool m_IsMultiInterval;
        public bool IsMultiInterval
        {
            get { return m_IsMultiInterval; }
            set { m_IsMultiInterval = value; }
        }

        private bool m_isSameSubTask;
        public bool isSameSubTask
        {
            get { return m_isSameSubTask; }
            set { m_isSameSubTask = value; }
        }

        private bool m_isUserServerDB;
        public bool isUserServerDB
        {
            get { return m_isUserServerDB; }
            set { m_isUserServerDB = value; }
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

        //以下为5.5新增
        private bool m_isVisual;
        public bool IsVisual
        {
            get { return m_isVisual; }
            set { m_isVisual = value; }
        }

        //private bool m_isAutoCode;
        //public bool isAutoCode
        //{
        //    get { return m_isAutoCode; }
        //    set { m_isAutoCode = value; }
        //}

        //private string m_codePlugin;
        //public string CodePlugin
        //{
        //    get { return m_codePlugin; }
        //    set { m_codePlugin = value; }
        //}

        //private string m_codeSucceedFlag;
        //public string CodeSucceedFlag
        //{
        //    get { return m_codeSucceedFlag; }
        //    set { m_codeSucceedFlag = value; }
        //}

        #endregion
    }
}

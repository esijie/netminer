using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using NetMiner.Common;
using NetMiner.Common.Tool;
using NetMiner.Resource;
using NetMiner.Gather.Task.Entity;
using NetMiner.Core.Task;

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


namespace NetMiner.Gather.Task
{
    //[Serializable]
    
    ///这个类是一个多功能的类，应该重新设计，采用集成的方式来进行
    ///此问题在下一个版本中修改
    ///此类的设计应该是一个任务的基类（可能会做成抽象类），由任务基类完成响应的派生，派生出任务执行类，及各种任务类别
    /// 当前采集任务仅提供了一种，后期会提供多种采集任务，所以，对此类还暂不修改
    ///现在先说当前的问题，此类主要做任务类，同时将任务执行的信息合并到此，这点需要注意，注释中会作出说明

    public class cTask
    {
        cXmlIO xmlConfig;
        private Single m_SupportTaskVersion = Single.Parse ("5.5");
        private string m_workPath = string.Empty;

        //此类别可处理的任务版本号，注意从1.3开始，任务处理类不再向前兼容
        public Single SupportTaskVersion
        {
            get { return m_SupportTaskVersion; }
        }


        #region 类的构造和销毁
        public cTask(string workPath)
        {
            m_workPath = workPath;
            this.WebpageLink = new List<cWebLink>();
            this.WebpageCutFlag = new List<cWebpageCutFlag>();
            this.TriggerTask = new List<cTriggerTask>();
            this.Headers = new List<eHeader>();
            this.PublishParas = new List<cPublishPara>();
            this.ThreadProxy = new List<cThreadProxy>();
        }

        ~cTask()
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
            set { m_InsertSql =value ;}
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
        private  bool m_IsSqlTrue;
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

        #region 用于为类方法
        
        /// <summary>
        /// 判断此任务所属分类下的索引文件是否存在
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        private bool IsExistTaskIndex(string Path)
        {
            string FileName;
            FileName = Path + "\\index.xml";
            bool IsExists = System.IO.File.Exists(FileName);
            return IsExists;
        }

        private string GetTaskClassPath()
        {
            string TClassName = this.TaskClass;
            string Path;

            if (TClassName == null || TClassName == "" || TClassName =="任务分类")
            {
                Path = m_workPath + "Tasks";
            }
            else
            {
                oTaskClass tClass = new oTaskClass(this.m_workPath);
                Path = tClass.GetTaskClassPathByName(TClassName);
                tClass = null;
            }
            return Path;
        }

        /// <summary>
        /// 判断任务文件是否存在，这里判断的是实际的物理文件
        /// </summary>
        /// <param name="TaskName"></param>
        /// <returns></returns>
        public bool IsExistTaskFile(string TaskName)
        {
            string Path = GetTaskClassPath();
            string File = Path + "\\" + TaskName + ".smt";
            bool IsExists = System.IO.File.Exists(File);

            //在判断索引文件是否存在此任务
            //cTaskIndex tIndex = new cTaskIndex(this.m_workPath);
            //tIndex.GetTaskDataByClass (this.TaskClass);

            //bool isexist=false ;
            //for (int i = 0; i < tIndex.GetTaskClassCount(); i++)
            //{
            //    if (tIndex.GetTaskName(i)==TaskName )

            //}

            return IsExists;
        }

        /// <summary>
        /// 判断采集任务是否存在，这里判断的是此任务是否则index文件中存在
        /// </summary>
        /// <param name="TaskName"></param>
        /// <returns></returns>
        public bool IsExistTaskOnIndex(string TaskName)
        {

            //在判断索引文件是否存在此任务
            oTaskIndex tIndex = new oTaskIndex(this.m_workPath);
            tIndex.LoadIndexDocument(this.TaskClass);
            bool isexist = tIndex.isExistTask(TaskName);
            tIndex = null;

            return isexist;
        }

        //保存任务信息，在保存任务信息的同时会自动维护任务分类数据
        /// <summary>
        /// 保存任务文件，采集任务名称从taskname获取,注意此方法会自动维护分类下的index文件
        /// </summary>
        /// <param name="TaskPath">传入的指定路径</param>
        /// <param name="isCheckRepeat">是否判断任务已经存在</param>
        public void Save(string TaskPath,bool isCheckRepeat)
        {
            //获取需要保存任务的路径
            string tPath = "";

            if (TaskPath == "" || TaskPath == null)
            {
                tPath = GetTaskClassPath() + "\\";
            }
            else
            {
                tPath = TaskPath;
            }
            int i=0;

            //判断此路径下是否已经存在了此任务，如果存在则返回错误信息
            if (IsExistTaskFile( this.TaskName) && isCheckRepeat==true )
            {
                 throw new NetMinerException ("任务已经存在，不能建立");
            }

            //维护任务的Index.xml文件
            int TaskID=InsertTaskIndex(tPath);

            //开始增加Task任务
            //构造Task任务的XML文档格式
            //当前构造xml文件全部采用的拼写字符串的形式,并没有采用xml构造函数
            StringBuilder tXml = new StringBuilder();
            tXml.Append ("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                "<Task>" +
                "<State></State>" +       ///此状态值当前无效,用于将来扩充使用
                "<BaseInfo>" +
                "<Version>" + SupportTaskVersion.ToString () + "</Version>" +  
                "<ID>" + TaskID + "</ID>" +
                "<Name>" + this.TaskName + "</Name>" +
                "<TaskDemo>" + this.TaskDemo + "</TaskDemo>" +
                "<Class>" + this.TaskClass + "</Class>" +
                "<Type>" + this.TaskType + "</Type>" +

                "<Visual>" + this.IsVisual + "</Visual>" +                 //此为5.5增加
                "<RunType>" + this.RunType + "</RunType>" +

                //选哟转换成相对路径
                "<SavePath>" + cTool.GetRelativePath(this.m_workPath, this.SavePath) + "</SavePath>" +
                "<ThreadCount>" + this.ThreadCount + "</ThreadCount>" +
                "<UrlCount>" + this.UrlCount + "</UrlCount>" +
                "<StartPos><![CDATA[" + this.StartPos + "]]></StartPos>" +
                "<EndPos><![CDATA[" + this.EndPos + "]]></EndPos>" +
                "<DemoUrl><![CDATA[" + this.DemoUrl + "]]></DemoUrl>" +
                "<Cookie><![CDATA[" + this.Cookie + "]]></Cookie>" +
                "<WebCode>" + this.WebCode + "</WebCode>" +
                "<IsUrlEncode>" + this.IsUrlEncode + "</IsUrlEncode>" +
                "<UrlEncode>" + this.UrlEncode + "</UrlEncode>" +
                "<IsTwoUrlCode>" + this.IsTwoUrlCode + "</IsTwoUrlCode>" +
                "</BaseInfo>" +

                "<Result>" +

                "<ExportType>" + this.ExportType + "</ExportType>" +
                "<ExportFileName>" + this.ExportFile + "</ExportFileName>" +
                "<DataSource>" + this.DataSource + "</DataSource>" +
                "<IsUserServerDB>" + this.isUserServerDB + "</IsUserServerDB>" +
                "<DataTableName>" + this.DataTableName + "</DataTableName>" +
                "<IsSqlTrue>" + this.IsSqlTrue + "</IsSqlTrue>" +

                "<InsertSql>" + this.InsertSql + "</InsertSql>" +

                "<PublishIntervalTime>" + this.PIntervalTime + "</PublishIntervalTime>" +
                "<PublishSucceedFlag>" + this.PSucceedFlag + "</PublishSucceedFlag>" +
                "<PublishThread>" + this.PublishThread + "</PublishThread>" +

                "<TemplateName>" + this.TemplateName + "</TemplateName>" +
                "<User>" + this.User + "</User>" +
                "<Password>" + this.Password + "</Password>" +
                "<Domain>" + this.Domain + "</Domain>" +
                "<PublishDbConn>" + this.TemplateDBConn + "</PublishDbConn>" +
                "<Paras>");

                for (int pi = 0; pi < this.PublishParas.Count; pi++)
                {
                    tXml.Append ("<Para>");
                    tXml.Append ( "<Label>" + this.PublishParas[pi].PublishPara + "</Label>)");
                    tXml.Append ( "<Value>" + this.PublishParas[pi].PublishValue + "</Value>)");
                    tXml.Append ( "<Type>" + (int)this.PublishParas[pi].PublishParaType + "</Type>");
                    tXml.Append ( "</Para>");
                }
                tXml.Append ("</Paras>" +
                "</Result>");

            tXml.Append ("<Advance>" +
                "<GatherAgainNumber>" + this.GatherAgainNumber + "</GatherAgainNumber>" +
                "<IsIgnore404>" + this.IsIgnore404 + "</IsIgnore404>" +
                "<IsErrorLog>" + this.IsErrorLog + "</IsErrorLog>" +
                "<IsExportHeader>" + this.IsExportHeader + "</IsExportHeader>" +
                "<IsRowFile>" + this.IsRowFile + "</IsRowFile>" +
                "<IsDelRepeatRow>" + this.IsDelRepRow + "</IsDelRepeatRow>" +
                "<IsDelTempData>" + this.IsDelTempData + "</IsDelTempData>" +
                "<IsSaveSingleFile>" + this.IsSaveSingleFile + "</IsSaveSingleFile>" +
                "<TempFileName>" + this.TempDataFile + "</TempFileName>" +
                "<IsDataProcess>" + this.IsDataProcess + "</IsDataProcess>" +
                "<IsExportGUrl>" + this.IsExportGUrl + "</IsExportGUrl>" +
                "<IsExportGDateTime>" + this.IsExportGDateTime + "</IsExportGDateTime>" +
                "<IsTrigger>" + this.IsTrigger + "</IsTrigger>" +
                "<TriggerType>" + this.TriggerType + "</TriggerType>" +
                "<GatherIntervalTime>" + this.GIntervalTime + "</GatherIntervalTime>" +
                "<GatherIntervalTime1>" + this.GIntervalTime1 + "</GatherIntervalTime1>" +
                "<IsMultiInterval>" + this.IsMultiInterval + "</IsMultiInterval>" +
                "<IsCustomHeader>" + this.IsCustomHeader + "</IsCustomHeader>" +
                "<IsProxy>" + this.IsProxy + "</IsProxy>" +
                "<IsProxyFirst>" + this.IsProxyFirst + "</IsProxyFirst>" +
                "<IsUrlNoneRepeat>" + this.IsUrlNoneRepeat + "</IsUrlNoneRepeat>" +
                "<IsUrlSucceedRepeat>" + this.IsSucceedUrlRepeat + "</IsUrlSucceedRepeat>" +
                "<IsUrlAutoRedirect>" + this.IsUrlAutoRedirect + "</IsUrlAutoRedirect>" +
                "<IsGatherErrStop>" + this.IsGatherErrStop + "</IsGatherErrStop>" +
                "<GatherErrStopCount>" + this.GatherErrStopCount + "</GatherErrStopCount>" +
                "<GatherErrStopRule>" + this.GatherErrStopRule + "</GatherErrStopRule>" +
                "<IsInsertDataErrStop>" + this.IsInsertDataErrStop + "</IsInsertDataErrStop>" +
                "<InsertDataErrStopConut>" + this.InsertDataErrStopConut + "</InsertDataErrStopConut>" +
                "<IsGatherRepeatStop>" + this.IsGatherRepeatStop + "</IsGatherRepeatStop>" +
                "<IsGatherRepeatStopRule>" + this.GatherRepeatStopRule + "</IsGatherRepeatStopRule>" +
                "<IsIgnoreErr>" + this.IsIgnoreErr + "</IsIgnoreErr>" +
                "<IsAutoUpdateHeader>" + this.IsAutoUpdateHeader + "</IsAutoUpdateHeader>" +
                "<IsNoneAllowSplit>" + this.IsNoneAllowSplit + "</IsNoneAllowSplit>" +
                "<IsSplitDbUrls>" + this.IsSplitDbUrls + "</IsSplitDbUrls>" +
                "<IsCookieList>" + this.isCookieList + "</IsCookieList>" +
                "<GatherCount>" + this.GatherCount + "</GatherCount>" +
                "<GatherCountPauseInterval>" + this.GatherCountPauseInterval + "</GatherCountPauseInterval>" +
                "<StopFlag><![CDATA[" + this.StopFlag + "]]></StopFlag>" +
                "<IsThrowGatherErr>" + this.IsThrowGatherErr + "</IsThrowGatherErr>" +
                "<IsSameSubTask>" + this.isSameSubTask + "</IsSameSubTask>" +
                "<IsGatherCoding>" + this.isGatherCoding + "</IsGatherCoding>" +
                "<GatherCodingFlag>" + this.GatherCodingFlag + "</GatherCodingFlag>" +
                "<GatherCodingPlugin>" + this.GatherCodingPlugin + "</GatherCodingPlugin>" +
                "<CodingUrl><![CDATA[" + this.CodeUrl + "]]></CodingUrl>" + 
                "</Advance>");

            //增加每个线程的代理信息
            tXml.Append("<ThreadProxy>");
            for (i = 0; i < this.ThreadProxy.Count; i++)
            {
                tXml.Append("<Proxy>");
                tXml.Append("<Index>" + this.ThreadProxy[i].Index + "</Index>");
                tXml.Append("<pType>" + this.ThreadProxy[i].pType + "</pType>");
                tXml.Append("<Address>" + this.ThreadProxy[i].Address + "</Address>");
                tXml.Append("<Port>" + this.ThreadProxy[i].Port + "</Port>");
                tXml.Append("</Proxy>");
            }
            tXml.Append("</ThreadProxy>");

            //增加HTTP Header信息
            tXml.Append ("<HttpHeaders>");

            for (i = 0; i < this.m_Headers.Count; i++)
            {
                tXml.Append ("<Header>");
                tXml.Append ("<Label>" + this.m_Headers[i].Label + "</Label>");
                tXml.Append ("<LabelValue><![CDATA[" + this.m_Headers[i].LabelValue + "]]></LabelValue>");
                tXml.Append("<Range><![CDATA[" + this.m_Headers[i].Range + "]]></Range>");
                tXml.Append ("</Header>");
            }

            tXml.Append ( "</HttpHeaders>");

            tXml.Append ("<Trigger>");
            for (i = 0; i < this.m_TriggerTask.Count; i++)
            {
                tXml.Append ("<Task>");
                tXml.Append ( "<RunTaskType>" + this.m_TriggerTask[i].RunTaskType + "</RunTaskType>");
                tXml.Append ("<RunTaskName>" + this.m_TriggerTask[i].RunTaskName + "</RunTaskName>");
                tXml.Append ("<RunTaskPara>" + this.m_TriggerTask[i].RunTaskPara + "</RunTaskPara>");
                tXml.Append ( "</Task>");
            }
            tXml.Append ("</Trigger>");

            //插件的功能
            tXml.Append ("<Plugins>");
            tXml.Append ("<IsPluginsCookie>" + this.IsPluginsCookie + "</IsPluginsCookie>" +
                "<PluginsCookie>" + this.PluginsCookie + "</PluginsCookie>" +
                "<IsPluginsDeal>" + this.IsPluginsDeal + "</IsPluginsDeal>" +
                "<PluginsDeal>" + this.PluginsDeal + "</PluginsDeal>" +
                "<IsPluginsPublish>" + this.IsPluginsPublish + "</IsPluginsPublish>" +
                "<PluginsPublish>" + this.PluginsPublish + "</PluginsPublish>");

            tXml.Append ("</Plugins>");

            tXml.Append ("<WebLinks>");

            if (this.WebpageLink != null)
            {
                for (i = 0; i < this.WebpageLink.Count; i++)
                {
                    tXml.Append ("<WebLink>");
                    tXml.Append ( "<Url><![CDATA[" + this.WebpageLink[i].Weblink.ToString() + "]]></Url>");
                    tXml.Append ( "<IsNag>" + this.WebpageLink[i].IsNavigation + "</IsNag>");
                    tXml.Append ( "<IsMultiPageGather>" + this.WebpageLink[i].IsMultiGather + "</IsMultiPageGather>");
                    tXml.Append ( "<IsMulti121>" + this.WebpageLink[i].IsData121 + "</IsMulti121>");
                    tXml.Append ( "<IsNextPage>" + this.WebpageLink[i].IsNextpage + "</IsNextPage>");
                    tXml.Append ( "<NextPageRule><![CDATA[" + this.WebpageLink[i].NextPageRule + "]]></NextPageRule>");
                    tXml.Append ( "<NextMaxPage>" + this.WebpageLink[i].NextMaxPage + "</NextMaxPage>");

                    //因为要处理自动翻页的断点续采，所以要记录下一页的Url，但作为任务新建，此值永远为空，此值由采集任务维护
                    tXml.Append ( "<NextPageUrl></NextPageUrl>"); 

                    //默认插入一个节点，表示此链接地址还未进行采集，因为是系统添加任务，所以默认为UnGather
                    tXml.Append ( "<IsGathered>" + (int)cGlobalParas.UrlGatherResult.UnGather + "</IsGathered>");

                    //插入此网址的导航规则
                    if (this.WebpageLink[i].IsNavigation ==true)
                    {
                        tXml.Append ( "<NavigationRules>");
                        for (int j = 0; j < this.WebpageLink[i].NavigRules.Count; j++)
                        {
                            tXml.Append ( "<NavigationRule>");
                            tXml.Append ( "<Url><![CDATA[" + this.WebpageLink[i].NavigRules[j].Url + "]]></Url>");
                            tXml.Append ( "<Level>" + this.WebpageLink[i].NavigRules[j].Level + "</Level>");
                            tXml.Append ( "<IsNext>" + this.WebpageLink[i].NavigRules[j].IsNext + "</IsNext>");
                            tXml.Append ( "<NextRule><![CDATA[" + this.WebpageLink[i].NavigRules[j].NextRule + "]]></NextRule>");
                            tXml.Append ( "<NextMaxPage>" + this.WebpageLink[i].NavigRules[j].NextMaxPage + "</NextMaxPage>");
                            tXml.Append ( "<NaviStartPos><![CDATA[" + this.WebpageLink[i].NavigRules[j].NaviStartPos + "]]></NaviStartPos>");
                            tXml.Append ( "<NaviEndPos><![CDATA[" + this.WebpageLink[i].NavigRules[j].NaviEndPos + "]]></NaviEndPos>");
                            tXml.Append ( "<NagRule><![CDATA[" + this.WebpageLink[i].NavigRules[j].NavigRule + "]]></NagRule>");
                            tXml.Append ( "<IsNextPage>" + this.WebpageLink[i].NavigRules[j].IsNaviNextPage + "</IsNextPage>");
                            tXml.Append ( "<NextPageRule><![CDATA[" + this.WebpageLink[i].NavigRules[j].NaviNextPage + "]]></NextPageRule>");
                            tXml.Append ( "<NaviNextMaxPage>" + this.WebpageLink[i].NavigRules[j].NaviNextMaxPage + "</NaviNextMaxPage>");
                            tXml.Append ( "<IsGather>" + this.WebpageLink[i].NavigRules[j].IsGather + "</IsGather>");
                            tXml.Append ( "<GatherStartPos><![CDATA[" + this.WebpageLink[i].NavigRules[j].GatherStartPos + "]]></GatherStartPos>");
                            tXml.Append ( "<GatherEndPos><![CDATA[" + this.WebpageLink[i].NavigRules[j].GatherEndPos + "]]></GatherEndPos>");
                            
                            //V5.2增加
                            tXml.Append("<RunType><![CDATA[" + (int)this.WebpageLink[i].NavigRules[j].RunRule + "]]></RunType>");
                            tXml.Append("<OtherNaviRule><![CDATA[" + this.WebpageLink[i].NavigRules[j].OtherNaviRule + "]]></OtherNaviRule>");
                            tXml.Append ( "</NavigationRule>");
                        }
                        tXml.Append ("</NavigationRules>");
                    }

                    //插入此网址的多页采集规则
                    if (this.WebpageLink[i].IsMultiGather == true)
                    {
                        
                        tXml.Append ( "<MultiPageRules>");
                        for (int j = 0; j < this.WebpageLink[i].MultiPageRules.Count; j++)
                        {
                            tXml.Append ( "<MultiPageRule>");
                            tXml.Append ( "<MultiRuleName>" + this.WebpageLink[i].MultiPageRules[j].RuleName + "</MultiRuleName>");
                            
                            //V5.2增加
                            tXml.Append("<MultiLevel><![CDATA[" + this.WebpageLink[i].MultiPageRules[j].mLevel + "]]></MultiLevel>");

                            tXml.Append ( "<MultiRule><![CDATA[" + this.WebpageLink[i].MultiPageRules[j].Rule + "]]></MultiRule>");
                            tXml.Append ( "</MultiPageRule>");
                        }
                        tXml.Append ( "</MultiPageRules>");
                    }

                    tXml.Append ( "</WebLink>");
                }
            }
                 
		    tXml.Append ("</WebLinks>" + "<GatherRules>" );
            if (this.WebpageCutFlag != null)
            {
                for (i = 0; i < this.WebpageCutFlag.Count; i++)
                {
                    tXml.Append ( "<GatherRule>");
                    tXml.Append ( "<Title><![CDATA[" + this.WebpageCutFlag[i].Title + "]]></Title>");
                    tXml.Append ( "<RuleByPage>" + this.WebpageCutFlag[i].RuleByPage + "</RuleByPage>");
                    tXml.Append ( "<DataType>" + this.WebpageCutFlag[i].DataType + "</DataType>");

                    tXml.Append ( "<GatherRuleType>" + this.WebpageCutFlag[i].GatherRuleType + "</GatherRuleType>");
                    tXml.Append ( "<XPath><![CDATA[" + this.WebpageCutFlag[i].XPath + "]]></XPath>");
                    tXml.Append ( "<NodePrty>" + this.WebpageCutFlag[i].NodePrty + "</NodePrty>");

                    tXml.Append ( "<StartFlag><![CDATA[" + this.WebpageCutFlag[i].StartPos + "]]></StartFlag>");
                    tXml.Append ( "<EndFlag><![CDATA[" + this.WebpageCutFlag[i].EndPos + "]]></EndFlag>");
                    tXml.Append ( "<LimitSign>" + this.WebpageCutFlag[i].LimitSign + "</LimitSign>");
                    tXml.Append ( "<RegionExpression><![CDATA[" + this.WebpageCutFlag[i].RegionExpression + "]]></RegionExpression>");
                    tXml.Append ( "<IsMergeData>" + this.WebpageCutFlag[i].IsMergeData + "</IsMergeData>");
                    tXml.Append ( "<NavLevel>" + this.WebpageCutFlag[i].NavLevel + "</NavLevel>");

                    //采集规则V3.0增加，处理下载文件存储路径及重名规则
                    tXml.Append ( "<MultiPageName>" + this.WebpageCutFlag[i].MultiPageName + "</MultiPageName>");
                    tXml.Append ( "<DownloadFileSavePath>" + this.WebpageCutFlag[i].DownloadFileSavePath + "</DownloadFileSavePath>");
                    //tXml.Append ( "<DownloadFileDealType>" + this.WebpageCutFlag[i].DownloadFileDealType + "</DownloadFileDealType>");
                    //tXml.Append ( "<IsOcrText>" + this.WebpageCutFlag[i].IsOcrText + "</IsOcrText>");

                    //3.1所加
                    //tXml.Append ( "<OcrScale>" + this.WebpageCutFlag[i].OcrScale + "</OcrScale>");

                    tXml.Append ( "<IsAutoDownloadImage>" + this.WebpageCutFlag[i].IsAutoDownloadImage + "</IsAutoDownloadImage>");

                    //V5.0增加
                    tXml.Append ( "<IsAutoDownloadFirstImage>" + this.WebpageCutFlag[i].IsAuthoFirstImage + "</IsAutoDownloadFirstImage>");

                    //插入数据输出规则

                    tXml.Append ( "<ExportRules>");

                    for (int m = 0; m < this.WebpageCutFlag[i].ExportRules.Count; m++)
                    {
                        tXml.Append ( "<ExportRule>");
                        tXml.Append("<ExortField><![CDATA[" + this.WebpageCutFlag[i].ExportRules[m].Field + "]]></ExortField>");
                        tXml.Append ( "<ExortRuleType>" + this.WebpageCutFlag[i].ExportRules[m].FieldRuleType + "</ExortRuleType>");
                        tXml.Append ( "<ExortRuleCondition><![CDATA[" + this.WebpageCutFlag[i].ExportRules[m].FieldRule + "]]></ExortRuleCondition>");
                        tXml.Append ( "</ExportRule>");

                    }
                    tXml.Append ( "</ExportRules>");

                    tXml.Append ( "</GatherRule>");
                }
            }
             tXml.Append ("</GatherRules>" +
                "</Task>");
            
            //xmlConfig =new cXmlIO ();
            //xmlConfig.NewXmlFile (tPath + this.TaskName + ".smt",tXml );
            //xmlConfig = null;

            cFile.SaveFileBinary(tPath + this.TaskName + ".smt", tXml.ToString (), true);

        }

        //仅根据任务信息保存任务文件，不做其他数据的问题，此方法
        //主要用于支持任务升级使用
        /// <summary>
        /// 将当前加载的任务保存到指定的路径，注意任务名称不会发生变化，且不维护index文件，但会
        /// 判断此任务文件是否存在，如果存在则报错
        /// </summary>
        /// <param name="TaskPath"></param>
        public void SaveTaskFile(string TaskPath)
        {
            //获取需要保存任务的路径
            string tPath = "";

            if (TaskPath == "" || TaskPath == null)
            {
                tPath = GetTaskClassPath() + "\\";
            }
            else
            {
                tPath = TaskPath;
            }
           

            //判断此路径下是否已经存在了此任务，如果存在则返回错误信息
            if (IsExistTaskFile( this.TaskName))
            {
                throw new NetMinerException("任务已经存在，不能建立");
            }

            string tXml = GetTaskXML();
            cFile.SaveFileBinary(tPath + this.TaskName + ".smt", tXml.ToString(), true);

        }

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
            StringBuilder  tXml=new StringBuilder ();
            tXml.Append ( "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                "<Task>" +
                "<State></State>" +       ///此状态值当前无效,用于将来扩充使用
                "<BaseInfo>" +
                "<Version>" + SupportTaskVersion.ToString() + "</Version>" +
                "<ID>" + TaskID + "</ID>" +
                "<Name>" + this.TaskName + "</Name>" +
                "<TaskDemo>" + this.TaskDemo + "</TaskDemo>" +
                "<Class>" + this.TaskClass + "</Class>" +
                "<Type>" + this.TaskType + "</Type>" +

                "<Visual>" + this.IsVisual + "</Visual>" +                 //此为5.5增加

                "<RunType>" + this.RunType + "</RunType>" +

                //选哟转换成相对路径
                "<SavePath>" + cTool.GetRelativePath(this.m_workPath, this.SavePath) + "</SavePath>" +
                "<ThreadCount>" + this.ThreadCount + "</ThreadCount>" +
                "<UrlCount>" + this.UrlCount + "</UrlCount>" +
                "<StartPos><![CDATA[" + this.StartPos + "]]></StartPos>" +
                "<EndPos><![CDATA[" + this.EndPos + "]]></EndPos>" +
                "<DemoUrl><![CDATA[" + this.DemoUrl + "]]></DemoUrl>" +
                "<Cookie><![CDATA[" + this.Cookie + "]]></Cookie>" +
                "<WebCode>" + this.WebCode + "</WebCode>" +
                "<IsUrlEncode>" + this.IsUrlEncode + "</IsUrlEncode>" +
                "<UrlEncode>" + this.UrlEncode + "</UrlEncode>" +
                "<IsTwoUrlCode>" + this.IsTwoUrlCode + "</IsTwoUrlCode>" +
                "</BaseInfo>" +
                "<Result>" +
                "<ExportType>" + this.ExportType + "</ExportType>" +
                "<ExportFileName>" + this.ExportFile + "</ExportFileName>" +
                "<DataSource>" + this.DataSource + "</DataSource>" +
                "<IsUserServerDB>" + this.isUserServerDB + "</IsUserServerDB>" +
                "<DataTableName>" + this.DataTableName + "</DataTableName>" +
                "<IsSqlTrue>" + this.IsSqlTrue + "</IsSqlTrue>" +

                "<InsertSql>" + this.InsertSql + "</InsertSql>" +

                "<PublishIntervalTime>" + this.PIntervalTime + "</PublishIntervalTime>" +
                "<PublishSucceedFlag>" + this.PSucceedFlag + "</PublishSucceedFlag>" +
                "<PublishThread>" + this.PublishThread + "</PublishThread>" +

                "<TemplateName>" + this.TemplateName + "</TemplateName>" +
                "<User>" + this.User + "</User>" +
                "<Password>" + this.Password + "</Password>" +
                "<Domain>" + this.Domain + "</Domain>" +
                "<PublishDbConn>" + this.TemplateDBConn + "</PublishDbConn>" +
                "<Paras>");

                for (int pi = 0; pi < this.PublishParas.Count; pi++)
                {
                    tXml.Append ( "<Para>");
                    tXml.Append ( "<Label>" + this.PublishParas[pi].PublishPara + "</Label>");
                    tXml.Append ( "<Value>" + this.PublishParas[pi].PublishValue + "</Value>");
                    tXml.Append ( "<Type>" + (int)this.PublishParas[pi].PublishParaType + "</Type>");
                    tXml.Append ( "</Para>");
                }
            tXml.Append ( "</Paras>" +


                "</Result>");

            tXml.Append ( "<Advance>" +
                "<GatherAgainNumber>" + this.GatherAgainNumber + "</GatherAgainNumber>" +
                "<IsIgnore404>" + this.IsIgnore404 + "</IsIgnore404>" +
                "<IsErrorLog>" + this.IsErrorLog + "</IsErrorLog>" +
                "<IsExportHeader>" + this.IsExportHeader + "</IsExportHeader>" +
                "<IsRowFile>" + this.IsRowFile + "</IsRowFile>" +
                "<IsDelRepeatRow>" + this.IsDelRepRow + "</IsDelRepeatRow>" +
                "<IsDelTempData>" + this.IsDelTempData + "</IsDelTempData>" +
                "<IsSaveSingleFile>" + this.IsSaveSingleFile + "</IsSaveSingleFile>" +
                "<TempFileName>" + this.TempDataFile + "</TempFileName>" +
                "<IsDataProcess>" + this.IsDataProcess + "</IsDataProcess>" +
                "<IsExportGUrl><![CDATA[" + this.IsExportGUrl + "]]></IsExportGUrl>" +
                "<IsExportGDateTime>" + this.IsExportGDateTime + "</IsExportGDateTime>" +
                "<IsTrigger>" + this.IsTrigger + "</IsTrigger>" +
                "<TriggerType>" + this.TriggerType + "</TriggerType>" +
                "<GatherIntervalTime>" + this.GIntervalTime + "</GatherIntervalTime>" +
                "<GatherIntervalTime1>" + this.GIntervalTime1 + "</GatherIntervalTime1>" +
                "<IsMultiInterval>" + this.IsMultiInterval + "</IsMultiInterval>" +
                "<IsCustomHeader>" + this.IsCustomHeader + "</IsCustomHeader>" +
                "<IsProxy>" + this.IsProxy + "</IsProxy>" +
                "<IsProxyFirst>" + this.IsProxyFirst + "</IsProxyFirst>" +
                "<IsUrlNoneRepeat>" + this.IsUrlNoneRepeat + "</IsUrlNoneRepeat>" +
                "<IsUrlSucceedRepeat>" + this.IsSucceedUrlRepeat + "</IsUrlSucceedRepeat>" +
                "<IsUrlAutoRedirect>" + this.IsUrlAutoRedirect + "</IsUrlAutoRedirect>" +
                "<IsGatherErrStop>" + this.IsGatherErrStop + "</IsGatherErrStop>" +
                "<GatherErrStopCount>" + this.GatherErrStopCount + "</GatherErrStopCount>" +
                "<GatherErrStopRule>" + this.GatherErrStopRule + "</GatherErrStopRule>" +
                "<IsInsertDataErrStop>" + this.IsInsertDataErrStop + "</IsInsertDataErrStop>" +
                "<InsertDataErrStopConut>" + this.InsertDataErrStopConut + "</InsertDataErrStopConut>" +
                "<IsGatherRepeatStop>" + this.IsGatherRepeatStop + "</IsGatherRepeatStop>" +
                "<IsGatherRepeatStopRule>" + this.GatherRepeatStopRule + "</IsGatherRepeatStopRule>" +
                "<IsIgnoreErr>" + this.IsIgnoreErr + "</IsIgnoreErr>" +
                "<IsAutoUpdateHeader>" + this.IsAutoUpdateHeader + "</IsAutoUpdateHeader>" +
                "<IsNoneAllowSplit>" + this.IsNoneAllowSplit + "</IsNoneAllowSplit>" +
                "<IsSplitDbUrls>" + this.IsSplitDbUrls + "</IsSplitDbUrls>" +
                "<IsCookieList>" + this.isCookieList + "</IsCookieList>" +
                "<GatherCount>" + this.GatherCount + "</GatherCount>" +
                "<GatherCountPauseInterval>" + this.GatherCountPauseInterval + "</GatherCountPauseInterval>" +
                "<StopFlag><![CDATA[" + this.StopFlag + "]]></StopFlag>" +
                "<IsThrowGatherErr>" + this.IsThrowGatherErr + "</IsThrowGatherErr>" +
                "<IsSameSubTask>" + this.isSameSubTask + "</IsSameSubTask>" +
                "<IsGatherCoding>" + this.isGatherCoding + "</IsGatherCoding>" +
                "<GatherCodingFlag>" + this.GatherCodingFlag + "</GatherCodingFlag>" +
                "<GatherCodingPlugin>" + this.GatherCodingPlugin + "</GatherCodingPlugin>" +
                "<CodingUrl><![CDATA[" + this.CodeUrl + "]]></CodingUrl>" + 

                "</Advance>");


            //增加每个线程的代理信息
            tXml.Append("<ThreadProxy>");
            for (i = 0; i < this.ThreadProxy.Count; i++)
            {
                tXml.Append("<Proxy>");
                tXml.Append("<Index>" + this.ThreadProxy[i].Index + "</Index>");
                tXml.Append("<pType>" + this.ThreadProxy[i].pType + "</pType>");
                tXml.Append("<Address>" + this.ThreadProxy[i].Address + "</Address>");
                tXml.Append("<Port>" + this.ThreadProxy[i].Port + "</Port>");
                tXml.Append("</Proxy>");
            }
            tXml.Append("</ThreadProxy>");

            //增加HTTP Header信息
            tXml.Append ( "<HttpHeaders>");

            for (i = 0; i < this.m_Headers.Count; i++)
            {
                tXml.Append ( "<Header>");
                tXml.Append ( "<Label>" + this.m_Headers[i].Label + "</Label>");
                tXml.Append ( "<LabelValue><![CDATA[" + this.m_Headers[i].LabelValue + "]]></LabelValue>");
                tXml.Append ( "<Range><![CDATA[" + this.m_Headers[i].Range + "]]></Range>");
                tXml.Append ( "</Header>");
            }

            tXml.Append ( "</HttpHeaders>");

            tXml.Append ( "<Trigger>");
            for (i = 0; i < this.m_TriggerTask.Count; i++)
            {
                tXml.Append ( "<Task>");
                tXml.Append ( "<RunTaskType>" + this.m_TriggerTask[i].RunTaskType + "</RunTaskType>");
                tXml.Append ( "<RunTaskName>" + this.m_TriggerTask[i].RunTaskName + "</RunTaskName>");
                tXml.Append ( "<RunTaskPara>" + this.m_TriggerTask[i].RunTaskPara + "</RunTaskPara>");
                tXml.Append ( "</Task>");
            }
            tXml.Append ( "</Trigger>");

            //插件的功能
            tXml.Append ( "<Plugins>");
            tXml.Append ( "<IsPluginsCookie>" + this.IsPluginsCookie + "</IsPluginsCookie>" +
                "<PluginsCookie>" + this.PluginsCookie + "</PluginsCookie>" +
                "<IsPluginsDeal>" + this.IsPluginsDeal + "</IsPluginsDeal>" +
                "<PluginsDeal>" + this.PluginsDeal + "</PluginsDeal>" +
                "<IsPluginsPublish>" + this.IsPluginsPublish + "</IsPluginsPublish>" +
                "<PluginsPublish>" + this.PluginsPublish + "</PluginsPublish>");

            tXml.Append ( "</Plugins>");

            tXml.Append ( "<WebLinks>");

            if (this.WebpageLink != null)
            {
                for (i = 0; i < this.WebpageLink.Count; i++)
                {
                    tXml.Append ( "<WebLink>");
                    tXml.Append ( "<Url><![CDATA[" + this.WebpageLink[i].Weblink.ToString() + "]]></Url>");
                    tXml.Append ( "<IsNag>" + this.WebpageLink[i].IsNavigation + "</IsNag>");
                    tXml.Append ( "<IsMultiPageGather>" + this.WebpageLink[i].IsMultiGather + "</IsMultiPageGather>");
                    tXml.Append ( "<IsMulti121>" + this.WebpageLink[i].IsData121 + "</IsMulti121>");
                    tXml.Append ( "<IsNextPage>" + this.WebpageLink[i].IsNextpage + "</IsNextPage>");
                    tXml.Append ( "<NextPageRule><![CDATA[" + this.WebpageLink[i].NextPageRule + "]]></NextPageRule>");
                    tXml.Append ( "<NextMaxPage>" + this.WebpageLink[i].NextMaxPage + "</NextMaxPage>");

                    tXml.Append ( "<NextPageUrl></NextPageUrl>");


                    //默认插入一个节点，表示此链接地址还未进行采集，因为是系统添加任务，所以默认为UnGather
                    tXml.Append ( "<IsGathered>" + (int)cGlobalParas.UrlGatherResult.UnGather + "</IsGathered>");

                    //插入此网址的导航规则
                    if (this.WebpageLink[i].IsNavigation == true)
                    {
                        tXml.Append ( "<NavigationRules>");
                        for (int j = 0; j < this.WebpageLink[i].NavigRules.Count; j++)
                        {
                            tXml.Append ( "<NavigationRule>");
                            tXml.Append ( "<Url><![CDATA[" + this.WebpageLink[i].NavigRules[j].Url + "]]></Url>");
                            tXml.Append ( "<Level>" + this.WebpageLink[i].NavigRules[j].Level + "</Level>");
                            tXml.Append ( "<IsNext>" + this.WebpageLink[i].NavigRules[j].IsNext + "</IsNext>");
                            tXml.Append ( "<NextRule><![CDATA[" + this.WebpageLink[i].NavigRules[j].NextRule + "]]></NextRule>");
                            tXml.Append ( "<NextMaxPage>" + this.WebpageLink[i].NavigRules[j].NextMaxPage + "</NextMaxPage>");
                            tXml.Append ( "<NaviStartPos><![CDATA[" + this.WebpageLink[i].NavigRules[j].NaviStartPos + "]]></NaviStartPos>");
                            tXml.Append ( "<NaviEndPos><![CDATA[" + this.WebpageLink[i].NavigRules[j].NaviEndPos + "]]></NaviEndPos>");
                            tXml.Append ( "<NagRule><![CDATA[" + this.WebpageLink[i].NavigRules[j].NavigRule + "]]></NagRule>");
                            tXml.Append ( "<IsNextPage>" + this.WebpageLink[i].NavigRules[j].IsNaviNextPage + "</IsNextPage>");
                            tXml.Append ( "<NextPageRule><![CDATA[" + this.WebpageLink[i].NavigRules[j].NaviNextPage + "]]></NextPageRule>");
                            tXml.Append ( "<NaviNextMaxPage>" + this.WebpageLink[i].NavigRules[j].NaviNextMaxPage + "</NaviNextMaxPage>");
                            tXml.Append ( "<IsGather>" + this.WebpageLink[i].NavigRules[j].IsGather + "</IsGather>");
                            tXml.Append ( "<GatherStartPos><![CDATA[" +this.WebpageLink[i].NavigRules[j].GatherStartPos + "]]></GatherStartPos>");
                            tXml.Append ( "<GatherEndPos><![CDATA[" + this.WebpageLink[i].NavigRules[j].GatherEndPos + "]]></GatherEndPos>");

                            tXml.Append("<RunType><![CDATA[" + (int)this.WebpageLink[i].NavigRules[j].RunRule + "]]></RunType>");
                            tXml.Append("<OtherNaviRule><![CDATA[" + this.WebpageLink[i].NavigRules[j].OtherNaviRule + "]]></OtherNaviRule>");


                            tXml.Append ( "</NavigationRule>");
                        }
                        tXml.Append ( "</NavigationRules>");
                    }

                    //插入此网址的多页采集规则
                    if (this.WebpageLink[i].IsMultiGather == true)
                    {
                        tXml.Append ( "<MultiPageRules>");
                        for (int j = 0; j < this.WebpageLink[i].MultiPageRules.Count; j++)
                        {
                            tXml.Append ( "<MultiPageRule>");
                            tXml.Append ( "<MultiRuleName>" + this.WebpageLink[i].MultiPageRules[j].RuleName + "</MultiRuleName>");

                            //V5.2增加
                            tXml.Append("<MultiLevel><![CDATA[" + this.WebpageLink[i].MultiPageRules[j].mLevel + "]]></MultiLevel>");

                            tXml.Append ( "<MultiRule><![CDATA[" + this.WebpageLink[i].MultiPageRules[j].Rule + "]]></MultiRule>");
                            tXml.Append ( "</MultiPageRule>");
                        }
                        tXml.Append ( "</MultiPageRules>");
                    }

                    tXml.Append ( "</WebLink>");
                }
            }

            tXml.Append ( "</WebLinks>" + "<GatherRules>");
            if (this.WebpageCutFlag != null)
            {
                for (i = 0; i < this.WebpageCutFlag.Count; i++)
                {
                    tXml.Append ( "<GatherRule>");
                    tXml.Append ( "<Title><![CDATA[" + this.WebpageCutFlag[i].Title + "]]></Title>");
                    tXml.Append ( "<RuleByPage>" + this.WebpageCutFlag[i].RuleByPage + "</RuleByPage>");
                    tXml.Append ( "<DataType>" + this.WebpageCutFlag[i].DataType + "</DataType>");

                    tXml.Append ( "<GatherRuleType>" + this.WebpageCutFlag[i].GatherRuleType + "</GatherRuleType>");
                    tXml.Append ( "<XPath><![CDATA[" + this.WebpageCutFlag[i].XPath + "]]></XPath>");
                    tXml.Append ( "<NodePrty>" + this.WebpageCutFlag[i].NodePrty + "</NodePrty>");

                    tXml.Append ( "<StartFlag><![CDATA[" + this.WebpageCutFlag[i].StartPos + "]]></StartFlag>");
                    tXml.Append ( "<EndFlag><![CDATA[" + this.WebpageCutFlag[i].EndPos + "]]></EndFlag>");
                    tXml.Append ( "<LimitSign>" + this.WebpageCutFlag[i].LimitSign + "</LimitSign>");
                    tXml.Append ( "<RegionExpression><![CDATA[" + this.WebpageCutFlag[i].RegionExpression + "]]></RegionExpression>");
                    tXml.Append ( "<IsMergeData>" + this.WebpageCutFlag[i].IsMergeData + "</IsMergeData>");
                    tXml.Append ( "<NavLevel>" + this.WebpageCutFlag[i].NavLevel + "</NavLevel>");

                    //采集规则V3.0增加，处理下载文件存储路径及重名规则
                    tXml.Append ( "<MultiPageName>" + this.WebpageCutFlag[i].MultiPageName + "</MultiPageName>");
                    tXml.Append ( "<DownloadFileSavePath>" + this.WebpageCutFlag[i].DownloadFileSavePath + "</DownloadFileSavePath>");
                    //tXml.Append ( "<DownloadFileDealType>" + this.WebpageCutFlag[i].DownloadFileDealType + "</DownloadFileDealType>");
                    //tXml.Append ( "<IsOcrText>" + this.WebpageCutFlag[i].IsOcrText + "</IsOcrText>");

                    ////3.1所加
                    //tXml.Append ( "<OcrScale>" + this.WebpageCutFlag[i].OcrScale + "</OcrScale>");

                    tXml.Append ( "<IsAutoDownloadImage>" + this.WebpageCutFlag[i].IsAutoDownloadImage + "</IsAutoDownloadImage>");

                    //V5.0增加
                    tXml.Append("<IsAutoDownloadFirstImage>" + this.WebpageCutFlag[i].IsAuthoFirstImage + "</IsAutoDownloadFirstImage>");


                    //插入数据输出规则

                    tXml.Append ( "<ExportRules>");

                    for (int m = 0; m < this.WebpageCutFlag[i].ExportRules.Count; m++)
                    {
                        tXml.Append ( "<ExportRule>");
                        tXml.Append("<ExortField><![CDATA[" + this.WebpageCutFlag[i].ExportRules[m].Field + "]]></ExortField>");
                        tXml.Append ( "<ExortRuleType>" + this.WebpageCutFlag[i].ExportRules[m].FieldRuleType + "</ExortRuleType>");
                        tXml.Append ( "<ExortRuleCondition><![CDATA[" + this.WebpageCutFlag[i].ExportRules[m].FieldRule + "]]></ExortRuleCondition>");
                        tXml.Append ( "</ExportRule>");

                    }
                    tXml.Append ( "</ExportRules>");

                    tXml.Append ( "</GatherRule>");
                }
            }
            tXml.Append ( "</GatherRules>" +
               "</Task>");
            #endregion

            return tXml.ToString ();
        }

        //更改任务的所属分类
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TaskName">任务名称</param>
        /// <param name="OldTaskClass">原有任务分类</param>
        /// <param name="NewTaskClass">新任务分类</param>
        /// 
        public void ChangeTaskClass(string TaskName, string OldTaskClass, string NewTaskClass)
        {
            oTaskClass tc = new oTaskClass(this.m_workPath);
            string oldPath="";
            string NewPath="";

            if (OldTaskClass == "")
                oldPath = this.m_workPath + "tasks";
            else
                oldPath = tc.GetTaskClassPathByName(OldTaskClass);

            if (NewTaskClass =="")
                NewPath = this.m_workPath + "tasks";
            else
                NewPath = tc.GetTaskClassPathByName( NewTaskClass);

            string FileName = TaskName + ".smt";

            System.IO.File.Copy(oldPath + "\\" + FileName, NewPath + "\\" + FileName);

            LoadTask(NewPath + "\\" + FileName);

            if (NewTaskClass =="")
                this.TaskClass ="";
            else
                this.TaskClass = NewTaskClass;

            Save("",false);

            DeleTask(oldPath, TaskName);

            tc = null;
        }

        //拷贝任务操作，将一个任务从原有分类拷贝到另一个分类下
        public void CopyTask(string TaskName, string OldTaskClass, string NewTaskClass)
        {
            oTaskClass tc = new oTaskClass(this.m_workPath);
            string oldPath = "";
            string NewPath = "";

            if (OldTaskClass == "")
                oldPath = this.m_workPath + "tasks";
            else
                oldPath = tc.GetTaskClassPathByName(OldTaskClass);

            if (NewTaskClass == "")
                NewPath = this.m_workPath + "tasks";
            else
                NewPath = tc.GetTaskClassPathByName(NewTaskClass);

            tc = null;

            string FileName = "";

            if (OldTaskClass == NewTaskClass || (File.Exists(NewPath + "\\" + TaskName + ".smt")))
            {
                FileName = TaskName + "-复制.smt";
                
                System.IO.File.Copy(oldPath + "\\" + TaskName + ".smt", NewPath + "\\" + FileName);
                TaskName = TaskName + "-复制";
                
            }
            else
            {
                FileName = TaskName + ".smt";
                System.IO.File.Copy(oldPath + "\\" + FileName, NewPath + "\\" + FileName);
            }

            LoadTask(NewPath + "\\" + FileName);

            //修改任务的名称
            this.TaskName = TaskName;

            if (NewTaskClass == "")
                this.TaskClass = "";
            else
                this.TaskClass = NewTaskClass;

            Save("",false );
            //SaveTask(NewPath + "\\" + FileName);
            
        }

        //插入任务信息到任务索引文件，返回新建任务索引的任务id
        public int InsertTaskIndex(string tPath)
        {

            oTaskIndex tIndex;

            //判断此路径下是否存在任务的索引文件
            if (!IsExistTaskIndex(tPath))
            {
                //如果不存在索引文件，则需要建立一个文件
                tIndex = new oTaskIndex(this.m_workPath);
                tIndex.NewIndexFile(tPath);
            }
            else
            {
                tIndex = new oTaskIndex(this.m_workPath, tPath + "\\index.xml");
            }

            tIndex.GetTaskDataByClass(this.TaskClass);

            int MaxTaskID = tIndex.GetTaskClassCount();

            //构造TaskIndex文件内容,此部分内容应该包含在TaskIndex类中
            //string indexXml = "<id>" + MaxTaskID + "</id>" +
            //        "<Name>" + this.TaskName + "</Name>" +
            //        "<Type>" + this.TaskType + "</Type>" +
            //        "<RunType>" + this.RunType + "</RunType>" +
            //        "<ExportFile>" + this.ExportFile + "</ExportFile>" +
            //        "<WebLinkCount>" + this.UrlCount + "</WebLinkCount>" +
            //        "<PublishType>" + this.ExportType + "</PublishType>";

            NetMiner.Core.Task.Entity.eTaskIndex eIndex = new Core.Task.Entity.eTaskIndex();
            eIndex.ID = MaxTaskID + 1;
            eIndex.TaskName = this.TaskName;
            eIndex.TaskType = this.TaskType;
            eIndex.TaskRunType = this.RunType;
            eIndex.ExportFile = this.ExportFile;
            eIndex.WebLinkCount = this.UrlCount;
            eIndex.PublishType = this.ExportType;
            //XElement xe = new XElement("Task");
            //xe.Add(new XElement("id", MaxTaskID));
            //xe.Add(new XElement("Name", this.TaskName));
            //xe.Add(new XElement("Type"), this.TaskType);
            //xe.Add(new XElement("RunType", this.RunType));
            //xe.Add(new XElement("ExportFile", this.ExportFile));
            //xe.Add(new XElement("WebLinkCount", this.UrlCount));
            //xe.Add(new XElement("PublishType", this.ExportType));

            tIndex.InsertTaskIndex(eIndex);
            tIndex = null;

            return MaxTaskID;

        }

        //当新建一个任务时，调用此方法
        public void New()
        {
            //this.TaskState =(int) cGlobalParas.TaskState.TaskUnStart;

            if (xmlConfig != null)
            {
                xmlConfig = null;
            }
        }

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
        public void LoadTask(Int64  TaskID)
        {
            string FileName = this.m_workPath + "Tasks\\run\\task" + TaskID + ".rst";

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
                
                xmlConfig = new cXmlIO();
                xmlConfig.LoadXML(strXML);

                //获取TaskClass节点
                //TaskClass = xmlConfig.GetData("descendant::TaskClasses");
            }
            catch (System.Exception ex)
            {
             
                    throw ex;
                
            }

            //加载任务信息
            this.TaskID =Int64.Parse ( xmlConfig.GetNodeValue("Task/BaseInfo/ID"));
            this.TaskName = xmlConfig.GetNodeValue("Task/BaseInfo/Name");

            ///加载任务版本信息，注意：1.0中是不存在版本信息描述的，所以如果加载1.0的任务
            ///会出错
            try 
            {
                this.TaskVersion = Single.Parse(xmlConfig.GetNodeValue("Task/BaseInfo/Version"));
            }
            catch (System.Exception )
            {
                this.TaskVersion =Single.Parse ("1.0");
            }

            if (TaskVersion != SupportTaskVersion)
            {
                throw new NetMinerException("您加载任务的版本低于系统要求的版本，请对任务进行升级后重试！");
            }


            this.TaskDemo = xmlConfig.GetNodeValue("Task/BaseInfo/TaskDemo");
            this.TaskClass = xmlConfig.GetNodeValue("Task/BaseInfo/Class");
            this.TaskType=(cGlobalParas.TaskType) int.Parse (xmlConfig.GetNodeValue("Task/BaseInfo/Type"));

            this.IsVisual = (xmlConfig.GetNodeValue("Task/BaseInfo/Visual") == "True" ? true : false);

            this.RunType = (cGlobalParas.TaskRunType)int.Parse (xmlConfig.GetNodeValue("Task/BaseInfo/RunType"));

            //因存的是相对路径，所以要加上系统路径
            this.SavePath = this.m_workPath + xmlConfig.GetNodeValue("Task/BaseInfo/SavePath");
            this.UrlCount =int.Parse (xmlConfig.GetNodeValue("Task/BaseInfo/UrlCount").ToString ());
            this.ThreadCount = int.Parse (xmlConfig.GetNodeValue("Task/BaseInfo/ThreadCount"));
            this.Cookie = xmlConfig.GetNodeValue("Task/BaseInfo/Cookie");
            this.DemoUrl = xmlConfig.GetNodeValue("Task/BaseInfo/DemoUrl");
            this.StartPos = xmlConfig.GetNodeValue("Task/BaseInfo/StartPos");
            this.EndPos = xmlConfig.GetNodeValue("Task/BaseInfo/EndPos");
            this.WebCode = (cGlobalParas.WebCode)int.Parse ( xmlConfig.GetNodeValue("Task/BaseInfo/WebCode"));
            this.IsUrlEncode = (xmlConfig.GetNodeValue("Task/BaseInfo/IsUrlEncode") == "True" ? true : false);
            this.UrlEncode =  (cGlobalParas.WebCode)int.Parse ( xmlConfig.GetNodeValue("Task/BaseInfo/UrlEncode"));
            this.IsTwoUrlCode = (xmlConfig.GetNodeValue("Task/BaseInfo/IsTwoUrlCode") == "True" ? true : false);

            this.ExportType =(cGlobalParas.PublishType)int.Parse ( xmlConfig.GetNodeValue("Task/Result/ExportType") );
            this.ExportFile = xmlConfig.GetNodeValue("Task/Result/ExportFileName");
            this.DataSource = xmlConfig.GetNodeValue("Task/Result/DataSource");
            this.isUserServerDB = (xmlConfig.GetNodeValue("Task/Result/IsUserServerDB")== "True" ? true : false);;
            this.DataTableName = xmlConfig.GetNodeValue("Task/Result/DataTableName");
            this.IsSqlTrue = (xmlConfig.GetNodeValue("Task/Result/IsSqlTrue")=="True"?true:false);

            this.InsertSql = xmlConfig.GetNodeValue("Task/Result/InsertSql");

            this.PIntervalTime = int.Parse (xmlConfig.GetNodeValue("Task/Result/PublishIntervalTime"));
            this.PSucceedFlag = xmlConfig.GetNodeValue("Task/Result/PublishSucceedFlag");
            this.PublishThread= int.Parse (xmlConfig.GetNodeValue("Task/Result/PublishThread"));

            //开始加载发布模版信息
            this.TemplateName = xmlConfig.GetNodeValue("Task/Result/TemplateName");
            this.User = xmlConfig.GetNodeValue("Task/Result/User");
            this.Password = xmlConfig.GetNodeValue("Task/Result/Password");
            this.Domain = xmlConfig.GetNodeValue("Task/Result/Domain");
            this.TemplateDBConn = xmlConfig.GetNodeValue("Task/Result/PublishDbConn");

            DataView dw = new DataView();
            int i;

            dw = xmlConfig.GetData("descendant::Result/Paras");
            if (dw != null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    cPublishPara pPara = new cPublishPara();
                    pPara.PublishPara = dw[i].Row["Label"].ToString();
                    pPara.PublishValue = dw[i].Row["Value"].ToString();
                    pPara.PublishParaType = (cGlobalParas.PublishParaType)int.Parse (dw[i].Row["Type"].ToString());
                    this.PublishParas.Add(pPara);
                }
            }
            dw = null;

            //加载高级配置信息
            this.GatherAgainNumber= int.Parse (xmlConfig.GetNodeValue("Task/Advance/GatherAgainNumber"));
            this.IsIgnore404 = (xmlConfig.GetNodeValue("Task/Advance/IsIgnore404") == "True" ? true : false);
            this.IsErrorLog = (xmlConfig.GetNodeValue("Task/Advance/IsErrorLog") == "True" ? true : false);
            this.IsDelRepRow = (xmlConfig.GetNodeValue("Task/Advance/IsDelRepeatRow") == "True" ? true : false);
            this.IsDelTempData = (xmlConfig.GetNodeValue("Task/Advance/IsDelTempData") == "True" ? true : false);
            this.IsSaveSingleFile = (xmlConfig.GetNodeValue("Task/Advance/IsSaveSingleFile") == "True" ? true : false);
            this.TempDataFile = xmlConfig.GetNodeValue("Task/Advance/TempFileName");
            this.IsDataProcess = (xmlConfig.GetNodeValue("Task/Advance/IsDataProcess") == "True" ? true : false);
            this.IsExportGUrl = (xmlConfig.GetNodeValue("Task/Advance/IsExportGUrl") == "True" ? true : false);
            this.IsExportGDateTime = (xmlConfig.GetNodeValue("Task/Advance/IsExportGDateTime") == "True" ? true : false); 
            this.IsExportHeader =( xmlConfig.GetNodeValue("Task/Advance/IsExportHeader") == "True" ? true : false);
            
            //这样做的目的是因为版本没有升级，但采集任务格式变化了，加载的时候会出错，所以做了出错的维护
            //请在下一个版本中将此问题修正，升级采集任务即可
            try
            {
                this.IsRowFile = (xmlConfig.GetNodeValue("Task/Advance/IsRowFile") == "True" ? true : false);
            }
            catch
            {
                this.IsRowFile = false;
            }
            this.IsTrigger =( xmlConfig.GetNodeValue("Task/Advance/IsTrigger") == "True" ? true : false);
            this.TriggerType = xmlConfig.GetNodeValue("Task/Advance/TriggerType");
            this.GIntervalTime =float.Parse ( xmlConfig.GetNodeValue("Task/Advance/GatherIntervalTime"));
            this.GIntervalTime1 = float.Parse(xmlConfig.GetNodeValue("Task/Advance/GatherIntervalTime1"));
            this.IsMultiInterval = (xmlConfig.GetNodeValue("Task/Advance/IsMultiInterval") == "True" ? true : false);
            this.IsCustomHeader = (xmlConfig.GetNodeValue("Task/Advance/IsCustomHeader") == "True" ? true : false);

            //V2.0增加
            this.IsProxy = (xmlConfig.GetNodeValue("Task/Advance/IsProxy") == "True" ? true : false);
            this.IsProxyFirst = (xmlConfig.GetNodeValue("Task/Advance/IsProxyFirst") == "True" ? true : false);

            //V2.1增加
            this.IsUrlNoneRepeat = (xmlConfig.GetNodeValue("Task/Advance/IsUrlNoneRepeat") == "True" ? true : false);
            
            //这样做的目的是因为版本没有升级，但采集任务格式变化了，加载的时候会出错，所以做了出错的维护
            //请在下一个版本中将此问题修正，升级采集任务即可
            try
            {
                this.IsSucceedUrlRepeat = (xmlConfig.GetNodeValue("Task/Advance/IsUrlSucceedRepeat") == "True" ? true : false);
            }
            catch
            {
                this.IsSucceedUrlRepeat = false;
            }

            //V5增加
            this.IsUrlAutoRedirect = (xmlConfig.GetNodeValue("Task/Advance/IsUrlAutoRedirect") == "True" ? true : false);

            //V5.1增加
            this.IsGatherErrStop =(xmlConfig.GetNodeValue("Task/Advance/IsGatherErrStop") == "True" ? true : false);
            this.GatherErrStopCount = int.Parse(xmlConfig.GetNodeValue("Task/Advance/GatherErrStopCount"));
            this.GatherErrStopRule = (cGlobalParas.StopRule)int.Parse(xmlConfig.GetNodeValue("Task/Advance/GatherErrStopRule"));
            this.IsInsertDataErrStop = (xmlConfig.GetNodeValue("Task/Advance/IsInsertDataErrStop") == "True" ? true : false);
            this.InsertDataErrStopConut = int.Parse(xmlConfig.GetNodeValue("Task/Advance/InsertDataErrStopConut"));
            this.IsGatherRepeatStop = (xmlConfig.GetNodeValue("Task/Advance/IsGatherRepeatStop") == "True" ? true : false);
            this.GatherRepeatStopRule = (cGlobalParas.StopRule)int.Parse(xmlConfig.GetNodeValue("Task/Advance/IsGatherRepeatStopRule"));
            
            //V5.2增加
            this.IsIgnoreErr = (xmlConfig.GetNodeValue("Task/Advance/IsIgnoreErr") == "True" ? true : false);
            this.IsAutoUpdateHeader = (xmlConfig.GetNodeValue("Task/Advance/IsAutoUpdateHeader") == "True" ? true : false);
            this.IsNoneAllowSplit = (xmlConfig.GetNodeValue("Task/Advance/IsNoneAllowSplit") == "True" ? true : false);
            this.IsSplitDbUrls = (xmlConfig.GetNodeValue("Task/Advance/IsSplitDbUrls") == "True" ? true : false);

            //V5.3增加
            this.isCookieList = (xmlConfig.GetNodeValue("Task/Advance/IsCookieList") == "True" ? true : false);
            this.GatherCount = int.Parse ( xmlConfig.GetNodeValue("Task/Advance/GatherCount"));
            this.GatherCountPauseInterval = int.Parse(xmlConfig.GetNodeValue("Task/Advance/GatherCountPauseInterval"));
            this.StopFlag = xmlConfig.GetNodeValue("Task/Advance/StopFlag");
            this.IsThrowGatherErr = (xmlConfig.GetNodeValue("Task/Advance/IsThrowGatherErr")== "True" ? true : false);

            this.isSameSubTask = (xmlConfig.GetNodeValue("Task/Advance/IsSameSubTask") == "True" ? true : false);
            //V5.33
            this.isGatherCoding=(xmlConfig.GetNodeValue("Task/Advance/IsGatherCoding") == "True" ? true : false);  
            this.GatherCodingFlag = xmlConfig.GetNodeValue("Task/Advance/GatherCodingFlag") ;
            this.GatherCodingPlugin =xmlConfig.GetNodeValue("Task/Advance/GatherCodingPlugin") ;
            this.CodeUrl = xmlConfig.GetNodeValue("Task/Advance/CodingUrl");

            //以下是V5.32搞出的问题，任务版本变了，但版本号未升级，造成加载
            //失败，所以，先这样处理
            try
            {
                dw = xmlConfig.GetData("descendant::ThreadProxy");
                if (dw != null)
                {
                    for (i = 0; i < dw.Count; i++)
                    {
                        cThreadProxy tProxy = new cThreadProxy();
                        tProxy.Index = int.Parse(dw[i].Row["Index"].ToString());
                        tProxy.pType = (cGlobalParas.ProxyType)int.Parse(dw[i].Row["pType"].ToString());
                        tProxy.Address = dw[i].Row["Address"].ToString();
                        tProxy.Port = int.Parse(dw[i].Row["Port"].ToString());
                        this.ThreadProxy.Add(tProxy);
                    }
                }
                dw = null;
            }
            catch { }

            //加载HTTP Header信息
            dw = xmlConfig.GetData("descendant::HttpHeaders");
            eHeader header;

            if (dw != null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    header = new eHeader();
                    header.Label = dw[i].Row["Label"].ToString();
                    header.LabelValue = dw[i].Row["LabelValue"].ToString();
                    header.Range = dw[i].Row["Range"].ToString();
                    this.Headers.Add(header);
                }
            }

            dw = null;
            
            //加载Trigger信息
            dw = xmlConfig.GetData("descendant::Trigger");
            cTriggerTask tt;

            if (dw != null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    tt = new cTriggerTask();
                    tt.RunTaskType = (cGlobalParas.RunTaskType)int.Parse ( dw[i].Row["RunTaskType"].ToString());
                    tt.RunTaskName = dw[i].Row["RunTaskName"].ToString();
                    tt.RunTaskPara = dw[i].Row["RunTaskPara"].ToString();

                    this.TriggerTask.Add(tt);
                }
            }

            //加载插件的信息
            this.IsPluginsCookie = (xmlConfig.GetNodeValue("Task/Plugins/IsPluginsCookie") == "True" ? true : false);
            this.PluginsCookie = xmlConfig.GetNodeValue("Task/Plugins/PluginsCookie");
            this.IsPluginsDeal = (xmlConfig.GetNodeValue("Task/Plugins/IsPluginsDeal") == "True" ? true : false);
            this.PluginsDeal = xmlConfig.GetNodeValue("Task/Plugins/PluginsDeal");
            this.IsPluginsPublish = (xmlConfig.GetNodeValue("Task/Plugins/IsPluginsPublish") == "True" ? true : false);
            this.PluginsPublish = xmlConfig.GetNodeValue("Task/Plugins/PluginsPublish");

            dw = null;
            dw = new DataView();
            
            dw = xmlConfig.GetData("descendant::WebLinks");
            cWebLink w;

            DataView dn;

            if (dw!=null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    w = new cWebLink();
                    w.id = i;
                    w.Weblink  = dw[i].Row["Url"].ToString();

                    if (w.Weblink.StartsWith("{DbUrl"))
                    {
                        isDBUrl = true;
                    }

                    if (dw[i].Row["IsNag"].ToString() == "True")
                        w.IsNavigation = true;
                    else
                        w.IsNavigation = false;

                    if (dw[i].Row["IsNextPage"].ToString() == "True")
                        w.IsNextpage = true;
                    else
                        w.IsNextpage = false;

                    w.NextPageRule = dw[i].Row["NextPageRule"].ToString();
                    w.NextMaxPage =dw[i].Row ["NextMaxPage"].ToString ();

              

                    w.NextPageUrl = dw[i].Row["NextPageUrl"].ToString();

                    w.IsGathered = int.Parse((dw[i].Row["IsGathered"].ToString() == null || dw[i].Row["IsGathered"].ToString() == "") ? "2031" : dw[i].Row["IsGathered"].ToString());
                    
                    //加载导航规则
                    if (w.IsNavigation == true && dw[i].DataView.Table.ChildRelations.Count!=0)
                    {
                        dn = dw[i].CreateChildView("WebLink_NavigationRules")[0].CreateChildView("NavigationRules_NavigationRule");
                        cNavigRule nRule;

                        for (int m = 0; m < dn.Count; m++)
                        {
                            nRule = new cNavigRule();
                            nRule.Url = dn[m].Row["Url"].ToString();
                            nRule.Level = int.Parse(dn[m].Row["Level"].ToString());
                            if (dn[m].Row["IsNext"].ToString() == "True")
                                nRule.IsNext = true;
                            else
                                nRule.IsNext = false;

                            nRule.NextRule = dn[m].Row["NextRule"].ToString();
                            nRule.NextMaxPage =dn[m].Row["NextMaxPage"].ToString ();

                 
                            nRule.NavigRule = dn[m].Row["NagRule"].ToString();
                            nRule.NaviStartPos = dn[m].Row["NaviStartPos"].ToString();
                            nRule.NaviEndPos = dn[m].Row["NaviEndPos"].ToString();

                            if (dn[m].Row["IsNextPage"].ToString() == "True")
                                nRule.IsNaviNextPage = true;
                            else
                                nRule.IsNaviNextPage = false;

                            nRule.NaviNextPage = dn[m].Row["NextPageRule"].ToString();
                            nRule.NaviNextMaxPage = dn[m].Row["NaviNextMaxPage"].ToString();


                            if (dn[m].Row["IsGather"].ToString() == "True")
                                nRule.IsGather = true;
                            else
                                nRule.IsGather = false;

                            nRule.GatherStartPos = dn[m].Row["GatherStartPos"].ToString();
                            nRule.GatherEndPos = dn[m].Row["GatherEndPos"].ToString();

                            nRule.RunRule =(NetMiner.Resource.cGlobalParas.NaviRunRule)int.Parse(dn[m].Row["RunType"].ToString());
                            nRule.OtherNaviRule = dn[m].Row["OtherNaviRule"].ToString();

                            w.NavigRules.Add(nRule);
                        }
                    }

                    if (dw[i].Row["IsMultiPageGather"].ToString() == "True")
                        w.IsMultiGather  = true;
                    else
                        w.IsMultiGather = false;

                    if (dw[i].Row["IsMulti121"].ToString() == "True")
                        w.IsData121 = true;
                    else
                        w.IsData121 = false;


                    //加载多页采集规则
                    if (w.IsMultiGather == true)
                    {
                        dn = dw[i].CreateChildView("WebLink_MultiPageRules")[0].CreateChildView("MultiPageRules_MultiPageRule");

                        for (int m = 0; m < dn.Count; m++)
                        {
                            cMultiPageRule nRule = new cMultiPageRule();
                            nRule.RuleName  = dn[m].Row["MultiRuleName"].ToString();
                            nRule.mLevel =int.Parse ( dn[m].Row["MultiLevel"].ToString());
                            nRule.Rule = dn[m].Row["MultiRule"].ToString();
                            w.MultiPageRules.Add(nRule);
                        }
                    }

                    this.WebpageLink.Add(w);
                    w = null;
                }
            }

            dw = null;
            dn=null;

            dw = new DataView();
            dw = xmlConfig.GetData("descendant::GatherRules");
            cWebpageCutFlag c;
            if (dw != null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    c = new cWebpageCutFlag();
                    c.Title = dw[i].Row["Title"].ToString();
                    c.RuleByPage = (cGlobalParas.GatherRuleByPage)int.Parse((dw[i].Row["RuleByPage"].ToString() == null || dw[i].Row["RuleByPage"].ToString() == "") ? "0" : dw[i].Row["RuleByPage"].ToString());
                    c.DataType =(cGlobalParas.GDataType) int.Parse((dw[i].Row["DataType"].ToString() == null || dw[i].Row["DataType"].ToString() == "") ? "0" : dw[i].Row["DataType"].ToString());

                    c.GatherRuleType = (cGlobalParas.GatherRuleType)int.Parse((dw[i].Row["GatherRuleType"].ToString() == null || dw[i].Row["GatherRuleType"].ToString() == "") ? "0" : dw[i].Row["GatherRuleType"].ToString());
                    c.XPath = dw[i].Row["XPath"].ToString(); 
                    c.NodePrty = dw[i].Row["NodePrty"].ToString();
                    
                    c.StartPos = dw[i].Row["StartFlag"].ToString();
                    c.EndPos =  dw[i].Row["EndFlag"].ToString();
                    c.LimitSign = (cGlobalParas.LimitSign)int.Parse((dw[i].Row["LimitSign"].ToString() == null || dw[i].Row["LimitSign"].ToString() == "") ? "0" : dw[i].Row["LimitSign"].ToString());
                    c.RegionExpression = dw[i].Row["RegionExpression"].ToString();
                    if (dw[i].Row["IsMergeData"].ToString() == "True")
                        c.IsMergeData = true;
                    else
                        c.IsMergeData = false;

                    c.NavLevel = int.Parse (dw[i].Row["NavLevel"].ToString());

                    //采集规则V2.6增加，处理现在文件的存储路径及重名规则
                    c.DownloadFileSavePath = dw[i].Row["DownloadFileSavePath"].ToString();
                    //c.DownloadFileDealType = dw[i].Row["DownloadFileDealType"].ToString();
                    c.MultiPageName = dw[i].Row["MultiPageName"].ToString();
                   
                    //3.1所加
                    if (dw[i].Row["IsAutoDownloadImage"].ToString() == "True")
                        c.IsAutoDownloadImage = true;
                    else
                        c.IsAutoDownloadImage = false;

                    if (dw[i].Row["IsAutoDownloadFirstImage"].ToString() == "True")
                        c.IsAuthoFirstImage = true;
                    else
                        c.IsAuthoFirstImage = false;

                    //加载数据输出规则
                    if (dw[i].DataView.Table.ChildRelations.Count!=0)
                    {
                        dn = dw[i].CreateChildView("GatherRule_ExportRules")[0].CreateChildView("ExportRules_ExportRule");

                        cFieldRule fRule;

                        for (int n = 0; n < dn.Count; n++)
                        {
                            fRule = new cFieldRule();
                            fRule.Field = dn[n].Row["ExortField"].ToString();
                            fRule.FieldRuleType = (cGlobalParas.ExportLimit) int.Parse(dn[n].Row["ExortRuleType"].ToString());
                            fRule.FieldRule = dn[n].Row["ExortRuleCondition"].ToString();

                            c.ExportRules.Add(fRule);
                        }
                    }

                    this.WebpageCutFlag.Add(c);
                    c = null;
                }
            }
            dw=null;

            //如果网址是dburl则重新更新urlcount
            if (isDBUrl == true)
            {
                int urlsCount=0;
                //更新urlcount
                for (int index = 0; index<this.WebpageLink.Count; index++)
                {
                    cUrlAnalyze gUrl=new cUrlAnalyze(this.m_workPath);

                    urlsCount += gUrl.GetUrlCount(this.WebpageLink[index].Weblink.ToString());
                }

                this.UrlCount=urlsCount ;

            }
        }

        /// <summary>
        /// 加载V5.1以前版本的采集任务，纯xml格式
        /// </summary>
        /// <param name="FileName"></param>
        //private void LoadOldTaskInfo(string FileName)
        //{
        //    //根据一个任务名称装载一个任务
        //    try
        //    {
        //        xmlConfig = new cXmlIO(FileName);

        //        //获取TaskClass节点
        //        //TaskClass = xmlConfig.GetData("descendant::TaskClasses");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        if (!File.Exists(FileName))
        //        {
        //            throw new System.IO.IOException("您指定的任务文件不存在！");
        //        }
        //        else
        //        {
        //            throw ex;
        //        }
        //    }

        //    //加载任务信息
        //    this.TaskID = Int64.Parse(xmlConfig.GetNodeValue("Task/BaseInfo/ID"));
        //    this.TaskName = xmlConfig.GetNodeValue("Task/BaseInfo/Name");

        //    ///加载任务版本信息，注意：1.0中是不存在版本信息描述的，所以如果加载1.0的任务
        //    ///会出错
        //    try
        //    {
        //        this.TaskVersion = Single.Parse(xmlConfig.GetNodeValue("Task/BaseInfo/Version"));
        //    }
        //    catch (System.Exception)
        //    {
        //        this.TaskVersion = Single.Parse("1.0");
        //    }

        //    if (TaskVersion != SupportTaskVersion)
        //    {
        //        throw new NetMinerException("您加载任务的版本低于系统要求的版本，请对任务进行升级后重试！");
        //    }


        //    this.TaskDemo = xmlConfig.GetNodeValue("Task/BaseInfo/TaskDemo");
        //    this.TaskClass = xmlConfig.GetNodeValue("Task/BaseInfo/Class");
        //    this.TaskType = xmlConfig.GetNodeValue("Task/BaseInfo/Type");
        //    this.RunType = xmlConfig.GetNodeValue("Task/BaseInfo/RunType");

        //    //因存的是相对路径，所以要加上系统路径
        //    this.SavePath = this.m_workPath + xmlConfig.GetNodeValue("Task/BaseInfo/SavePath");
        //    this.UrlCount = int.Parse(xmlConfig.GetNodeValue("Task/BaseInfo/UrlCount").ToString());
        //    this.ThreadCount = int.Parse(xmlConfig.GetNodeValue("Task/BaseInfo/ThreadCount"));
        //    this.Cookie = xmlConfig.GetNodeValue("Task/BaseInfo/Cookie");
        //    this.DemoUrl = xmlConfig.GetNodeValue("Task/BaseInfo/DemoUrl");
        //    this.StartPos = xmlConfig.GetNodeValue("Task/BaseInfo/StartPos");
        //    this.EndPos = xmlConfig.GetNodeValue("Task/BaseInfo/EndPos");
        //    this.WebCode = xmlConfig.GetNodeValue("Task/BaseInfo/WebCode");
        //    this.IsUrlEncode = (xmlConfig.GetNodeValue("Task/BaseInfo/IsUrlEncode") == "True" ? true : false);
        //    this.UrlEncode = xmlConfig.GetNodeValue("Task/BaseInfo/UrlEncode");

        //    this.ExportType = xmlConfig.GetNodeValue("Task/Result/ExportType");
        //    this.ExportFile = xmlConfig.GetNodeValue("Task/Result/ExportFileName");
        //    this.DataSource = xmlConfig.GetNodeValue("Task/Result/DataSource");
        //    this.DataTableName = xmlConfig.GetNodeValue("Task/Result/DataTableName");
        //    this.IsSqlTrue = (xmlConfig.GetNodeValue("Task/Result/IsSqlTrue") == "True" ? true : false);

        //    this.InsertSql = xmlConfig.GetNodeValue("Task/Result/InsertSql");

        //    this.PIntervalTime = int.Parse(xmlConfig.GetNodeValue("Task/Result/PublishIntervalTime"));
        //    this.PSucceedFlag = xmlConfig.GetNodeValue("Task/Result/PublishSucceedFlag");
        //    this.PublishThread = int.Parse(xmlConfig.GetNodeValue("Task/Result/PublishThread"));

        //    //开始加载发布模版信息
        //    this.TemplateName = xmlConfig.GetNodeValue("Task/Result/TemplateName");
        //    this.User = xmlConfig.GetNodeValue("Task/Result/User");
        //    this.Password = xmlConfig.GetNodeValue("Task/Result/Password");
        //    this.Domain = xmlConfig.GetNodeValue("Task/Result/Domain");
        //    this.TemplateDBConn = xmlConfig.GetNodeValue("Task/Result/PublishDbConn");

        //    DataView dw = new DataView();
        //    int i;

        //    dw = xmlConfig.GetData("descendant::Result/Paras");
        //    if (dw != null)
        //    {
        //        for (i = 0; i < dw.Count; i++)
        //        {
        //            cPublishPara pPara = new cPublishPara();
        //            pPara.PublishPara = dw[i].Row["Label"].ToString();
        //            pPara.PublishValue = dw[i].Row["Value"].ToString();
        //            pPara.PublishParaType = (cGlobalParas.PublishParaType)int.Parse(dw[i].Row["Type"].ToString());
        //            this.PublishParas.Add(pPara);
        //        }
        //    }
        //    dw = null;

        //    //加载高级配置信息
        //    this.GatherAgainNumber = int.Parse(xmlConfig.GetNodeValue("Task/Advance/GatherAgainNumber"));
        //    this.IsIgnore404 = (xmlConfig.GetNodeValue("Task/Advance/IsIgnore404") == "True" ? true : false);
        //    this.IsErrorLog = (xmlConfig.GetNodeValue("Task/Advance/IsErrorLog") == "True" ? true : false);
        //    this.IsDelRepRow = (xmlConfig.GetNodeValue("Task/Advance/IsDelRepeatRow") == "True" ? true : false);
        //    this.IsDelTempData = (xmlConfig.GetNodeValue("Task/Advance/IsDelTempData") == "True" ? true : false);
        //    this.IsSaveSingleFile = (xmlConfig.GetNodeValue("Task/Advance/IsSaveSingleFile") == "True" ? true : false);
        //    this.TempDataFile = xmlConfig.GetNodeValue("Task/Advance/TempFileName");
        //    this.IsDataProcess = (xmlConfig.GetNodeValue("Task/Advance/IsDataProcess") == "True" ? true : false);
        //    this.IsExportGUrl = (xmlConfig.GetNodeValue("Task/Advance/IsExportGUrl") == "True" ? true : false);
        //    this.IsExportGDateTime = (xmlConfig.GetNodeValue("Task/Advance/IsExportGDateTime") == "True" ? true : false);
        //    this.IsExportHeader = (xmlConfig.GetNodeValue("Task/Advance/IsExportHeader") == "True" ? true : false);
        //    this.IsTrigger = (xmlConfig.GetNodeValue("Task/Advance/IsTrigger") == "True" ? true : false);
        //    this.TriggerType = xmlConfig.GetNodeValue("Task/Advance/TriggerType");
        //    this.GIntervalTime = float.Parse(xmlConfig.GetNodeValue("Task/Advance/GatherIntervalTime"));
        //    this.GIntervalTime1 = float.Parse(xmlConfig.GetNodeValue("Task/Advance/GatherIntervalTime1"));
        //    this.IsCustomHeader = (xmlConfig.GetNodeValue("Task/Advance/IsCustomHeader") == "True" ? true : false);

        //    //V2.0增加
        //    this.IsProxy = (xmlConfig.GetNodeValue("Task/Advance/IsProxy") == "True" ? true : false);
        //    this.IsProxyFirst = (xmlConfig.GetNodeValue("Task/Advance/IsProxyFirst") == "True" ? true : false);

        //    //V2.1增加
        //    this.IsUrlNoneRepeat = (xmlConfig.GetNodeValue("Task/Advance/IsUrlNoneRepeat") == "True" ? true : false);

        //    //V5增加
        //    this.IsUrlAutoRedirect = (xmlConfig.GetNodeValue("Task/Advance/IsUrlAutoRedirect") == "True" ? true : false);

        //    //加载HTTP Header信息
        //    dw = xmlConfig.GetData("descendant::HttpHeaders");
        //    cHeader header;

        //    if (dw != null)
        //    {
        //        for (i = 0; i < dw.Count; i++)
        //        {
        //            header = new cHeader();
        //            header.Label = dw[i].Row["Label"].ToString();
        //            header.LabelValue = dw[i].Row["LabelValue"].ToString();

        //            this.Headers.Add(header);
        //        }
        //    }

        //    dw = null;

        //    //加载Trigger信息
        //    dw = xmlConfig.GetData("descendant::Trigger");
        //    cTriggerTask tt;

        //    if (dw != null)
        //    {
        //        for (i = 0; i < dw.Count; i++)
        //        {
        //            tt = new cTriggerTask();
        //            tt.RunTaskType = int.Parse(dw[i].Row["RunTaskType"].ToString());
        //            tt.RunTaskName = dw[i].Row["RunTaskName"].ToString();
        //            tt.RunTaskPara = dw[i].Row["RunTaskPara"].ToString();

        //            this.TriggerTask.Add(tt);
        //        }
        //    }

        //    //加载插件的信息
        //    this.IsPluginsCookie = (xmlConfig.GetNodeValue("Task/Plugins/IsPluginsCookie") == "True" ? true : false);
        //    this.PluginsCookie = xmlConfig.GetNodeValue("Task/Plugins/PluginsCookie");
        //    this.IsPluginsDeal = (xmlConfig.GetNodeValue("Task/Plugins/IsPluginsDeal") == "True" ? true : false);
        //    this.PluginsDeal = xmlConfig.GetNodeValue("Task/Plugins/PluginsDeal");
        //    this.IsPluginsPublish = (xmlConfig.GetNodeValue("Task/Plugins/IsPluginsPublish") == "True" ? true : false);
        //    this.PluginsPublish = xmlConfig.GetNodeValue("Task/Plugins/PluginsPublish");

        //    dw = null;
        //    dw = new DataView();

        //    dw = xmlConfig.GetData("descendant::WebLinks");
        //    cWebLink w;

        //    DataView dn;

        //    if (dw != null)
        //    {
        //        for (i = 0; i < dw.Count; i++)
        //        {
        //            w = new cWebLink();
        //            w.id = i;
        //            w.Weblink = dw[i].Row["Url"].ToString();

        //            if (dw[i].Row["IsNag"].ToString() == "True")
        //                w.IsNavigation = true;
        //            else
        //                w.IsNavigation = false;

        //            if (dw[i].Row["IsNextPage"].ToString() == "True")
        //                w.IsNextpage = true;
        //            else
        //                w.IsNextpage = false;

        //            w.NextPageRule = dw[i].Row["NextPageRule"].ToString();
        //            w.NextMaxPage = dw[i].Row["NextMaxPage"].ToString();

        //            if (dw[i].Row["IsDoPostBack"].ToString() == "True")
        //                w.IsDoPostBack = true;
        //            else
        //                w.IsDoPostBack = false;

        //            w.NextPageUrl = dw[i].Row["NextPageUrl"].ToString();

        //            w.IsGathered = int.Parse((dw[i].Row["IsGathered"].ToString() == null || dw[i].Row["IsGathered"].ToString() == "") ? "2031" : dw[i].Row["IsGathered"].ToString());

        //            //加载导航规则
        //            if (w.IsNavigation == true && dw[i].DataView.Table.ChildRelations.Count != 0)
        //            {
        //                dn = dw[i].CreateChildView("WebLink_NavigationRules")[0].CreateChildView("NavigationRules_NavigationRule");
        //                cNavigRule nRule;

        //                for (int m = 0; m < dn.Count; m++)
        //                {
        //                    nRule = new cNavigRule();
        //                    nRule.Url = dn[m].Row["Url"].ToString();
        //                    nRule.Level = int.Parse(dn[m].Row["Level"].ToString());
        //                    if (dn[m].Row["IsNext"].ToString() == "True")
        //                        nRule.IsNext = true;
        //                    else
        //                        nRule.IsNext = false;

        //                    nRule.NextRule = dn[m].Row["NextRule"].ToString();
        //                    nRule.NextMaxPage = dn[m].Row["NextMaxPage"].ToString();

        //                    if (dn[m].Row["IsNextDoPostBack"].ToString() == "True")
        //                        nRule.IsNextDoPostBack = true;
        //                    else
        //                        nRule.IsNextDoPostBack = false;

        //                    nRule.NavigRule = dn[m].Row["NagRule"].ToString();
        //                    nRule.NaviStartPos = dn[m].Row["NaviStartPos"].ToString();
        //                    nRule.NaviEndPos = dn[m].Row["NaviEndPos"].ToString();

        //                    if (dn[m].Row["IsNextPage"].ToString() == "True")
        //                        nRule.IsNaviNextPage = true;
        //                    else
        //                        nRule.IsNaviNextPage = false;

        //                    nRule.NaviNextPage = dn[m].Row["NextPageRule"].ToString();
        //                    nRule.NaviNextMaxPage = dn[m].Row["NaviNextMaxPage"].ToString();

        //                    if (dn[m].Row["IsNaviNextDoPostBack"].ToString() == "True")
        //                        nRule.IsNaviNextDoPostBack = true;
        //                    else
        //                        nRule.IsNaviNextDoPostBack = false;

        //                    if (dn[m].Row["IsGather"].ToString() == "True")
        //                        nRule.IsGather = true;
        //                    else
        //                        nRule.IsGather = false;

        //                    nRule.GatherStartPos = dn[m].Row["GatherStartPos"].ToString();
        //                    nRule.GatherEndPos = dn[m].Row["GatherEndPos"].ToString();

        //                    w.NavigRules.Add(nRule);
        //                }
        //            }

        //            if (dw[i].Row["IsMultiPageGather"].ToString() == "True")
        //                w.IsMultiGather = true;
        //            else
        //                w.IsMultiGather = false;

        //            if (dw[i].Row["IsMulti121"].ToString() == "True")
        //                w.IsData121 = true;
        //            else
        //                w.IsData121 = false;


        //            //加载多页采集规则
        //            if (w.IsMultiGather == true)
        //            {
        //                dn = dw[i].CreateChildView("WebLink_MultiPageRules")[0].CreateChildView("MultiPageRules_MultiPageRule");

        //                for (int m = 0; m < dn.Count; m++)
        //                {
        //                    cMultiPageRule nRule = new cMultiPageRule();
        //                    nRule.RuleName = dn[m].Row["MultiRuleName"].ToString();
        //                    nRule.Rule = dn[m].Row["MultiRule"].ToString();
        //                    w.MultiPageRules.Add(nRule);
        //                }
        //            }

        //            this.WebpageLink.Add(w);
        //            w = null;
        //        }
        //    }

        //    dw = null;
        //    dn = null;

        //    dw = new DataView();
        //    dw = xmlConfig.GetData("descendant::GatherRules");
        //    Task.cWebpageCutFlag c;
        //    if (dw != null)
        //    {
        //        for (i = 0; i < dw.Count; i++)
        //        {
        //            c = new Task.cWebpageCutFlag();
        //            c.Title = dw[i].Row["Title"].ToString();
        //            c.RuleByPage = int.Parse((dw[i].Row["RuleByPage"].ToString() == null || dw[i].Row["RuleByPage"].ToString() == "") ? "0" : dw[i].Row["RuleByPage"].ToString());
        //            c.DataType = int.Parse((dw[i].Row["DataType"].ToString() == null || dw[i].Row["DataType"].ToString() == "") ? "0" : dw[i].Row["DataType"].ToString());

        //            c.GatherRuleType = int.Parse((dw[i].Row["GatherRuleType"].ToString() == null || dw[i].Row["GatherRuleType"].ToString() == "") ? "0" : dw[i].Row["GatherRuleType"].ToString());
        //            c.XPath = dw[i].Row["XPath"].ToString();
        //            c.NodePrty = dw[i].Row["NodePrty"].ToString();

        //            c.StartPos = dw[i].Row["StartFlag"].ToString();
        //            c.EndPos = dw[i].Row["EndFlag"].ToString();
        //            c.LimitSign = int.Parse((dw[i].Row["LimitSign"].ToString() == null || dw[i].Row["LimitSign"].ToString() == "") ? "0" : dw[i].Row["LimitSign"].ToString());
        //            c.RegionExpression = dw[i].Row["RegionExpression"].ToString();
        //            if (dw[i].Row["IsMergeData"].ToString() == "True")
        //                c.IsMergeData = true;
        //            else
        //                c.IsMergeData = false;

        //            c.NavLevel = int.Parse(dw[i].Row["NavLevel"].ToString());

        //            //采集规则V2.6增加，处理现在文件的存储路径及重名规则
        //            c.DownloadFileSavePath = dw[i].Row["DownloadFileSavePath"].ToString();
        //            //c.DownloadFileDealType = dw[i].Row["DownloadFileDealType"].ToString();
        //            c.MultiPageName = dw[i].Row["MultiPageName"].ToString();
                  
        //            if (dw[i].Row["IsAutoDownloadImage"].ToString() == "True")
        //                c.IsAutoDownloadImage = true;
        //            else
        //                c.IsAutoDownloadImage = false;

        //            //加载数据输出规则
        //            if (dw[i].DataView.Table.ChildRelations.Count != 0)
        //            {
        //                dn = dw[i].CreateChildView("GatherRule_ExportRules")[0].CreateChildView("ExportRules_ExportRule");

        //                cFieldRule fRule;

        //                for (int n = 0; n < dn.Count; n++)
        //                {
        //                    fRule = new cFieldRule();
        //                    fRule.Field = dn[n].Row["ExortField"].ToString();
        //                    fRule.FieldRuleType = int.Parse(dn[n].Row["ExortRuleType"].ToString());
        //                    fRule.FieldRule = dn[n].Row["ExortRuleCondition"].ToString();

        //                    c.ExportRules.Add(fRule);
        //                }
        //            }

        //            this.WebpageCutFlag.Add(c);
        //            c = null;
        //        }
        //    }
        //    dw = null;

        //}

        //删除一个任务
        //删除任务的时候，系统会做一个处理，就是自动备份一个
        //任务文件，已~
        public bool DeleTask(string TaskPath,string TaskName)
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
                tPath = TaskPath;
            }

            //先删除索引文件中的任务索引内容
            oTaskIndex tIndex = new oTaskIndex(this.m_workPath , tPath + "\\index.xml");
            tIndex.DeleTaskIndex(TaskName);
            tIndex =null;

            //如果是编辑状态，为了防止删除了文件后，任务保存失败，则
            //任务文件将丢失的问题，首先先不删除此文件，只是将其改名

            //删除任务的物理文件
            string FileName =TaskPath   + "\\" + TaskName + ".smt" ;
            string tmpFileName=TaskPath   + "\\~" + TaskName + ".smt" ;

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
            catch (System.Exception )
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

        //根据taskid修改任务的名称
        public bool RenameTask(string TClass,string OldTaskName, string NewTaskName)
        {
            try
            {
                //根据任务分类获取任务的所属路径
                oTaskClass tc = new oTaskClass(this.m_workPath);
                string tClassPath = "";

                //判断新的任务路径是否存在，如果存在则报错
                if (TClass == "")
                {
                    tClassPath = this.m_workPath  + "tasks";
                }
                else
                {
                    tClassPath = tc.GetTaskClassPathByName(TClass);
                }

                tc = null;

                if (File.Exists(tClassPath + "\\" + NewTaskName + ".smt"))
                    throw new NetMinerException("您修改的任务名称已经存在，请重新修改！");

                oTaskIndex xmlTasks = new oTaskIndex(this.m_workPath);
                xmlTasks.LoadIndexDocument(TClass);

                bool isExistTask = xmlTasks.isExistTask(NewTaskName);
           
                if (isExistTask)
                {
                    xmlTasks = null;
                    throw new NetMinerException("您修改的任务名称已经存在，请重新修改！");
                }
                
                xmlTasks = null;

                //开始修改任务的名称
                //先开始修改index.xml的名称
                cXmlIO xmlIndex = new cXmlIO(tClassPath + "\\index.xml");
                xmlIndex.EditNodeValue("TaskIndex", "Name", OldTaskName, "Name", NewTaskName);
                xmlIndex.Save();
                xmlIndex = null;

                //开始修改任务的名称
                
                string strXML = cFile.ReadFileBinary(tClassPath + "\\" + OldTaskName + ".smt");
                cXmlIO xmlTask = new cXmlIO();
                xmlTask.LoadXML(strXML);
                xmlTask.EditNodeValue("Task/BaseInfo/Name", NewTaskName);
                //xmlTask.Save();
                cFile.SaveFileBinary(tClassPath + "\\" + NewTaskName + ".smt", xmlTask.InnerXml, true);
                xmlTask = null;

                File.Delete(tClassPath + "\\" + OldTaskName + ".smt");
                //File.SetAttributes(tClassPath + "\\" + OldTaskName + ".smt", System.IO.FileAttributes.Normal);
                //File.Move(tClassPath + "\\" + OldTaskName + ".smt", tClassPath + "\\" + NewTaskName + ".smt");
            }
            catch (System.Exception ex)
            {
                throw ex;
                //return false;
            }

            return true;
        }

         #endregion

    }
}

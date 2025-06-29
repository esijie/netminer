using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;
using NetMiner.Core.gTask.Entity;
using NetMiner.Net.Native;

///���ܣ��ɼ���������
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
///����Ƕ�Ӧ��UI�Ĳɼ��������ݣ���TaskRun������ӳ�䵽��������
namespace NetMiner.Core.Entity
{ 
    /// <summary>
    /// ����һ������Ļ������ݣ���Ҫ���ڱ�ʶ������TaskRun�е�һ�����ݽ���ӳ��
    /// ���ֻ������������������������Ϣ���ɼ���ʱ�򣬲��Դ�����Ϊ׼
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

        #region ��������
        /// <summary>
        /// ���ش˲ɼ�������ļ��������·��
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
        /// ��¼��ǰ�����ִ�н��̣�ִ�н��̰����������֣��ɼ�����ϴ������
        /// </summary>
        public cGlobalParas.TaskProcess Process { get; set; }
        #endregion

      
        /// <summary>
        /// �ɼ���ҳ�������������õ���ַ����
        /// </summary>
        public int UrlCount { get; set; }
        /// <summary>
        /// �Ѿ��ɼ������������������õ���ַ��������������������ַ����
        /// </summary>
        public int GatheredUrlCount { get; set; }

        /// <summary>
        /// �ɼ��������ַ�����������õ���ַ��������������������ַ
        /// </summary>
        public int GatherErrUrlCount { get; set; }

        /// <summary>
        /// ���ݵ���ҳ������������ַ��������Ҫ�ɼ�
        /// </summary>
        public int UrlNaviCount { get; set; }

        /// <summary>
        /// ���ݵ������򣬵�����������ַ���Ѿ��ɼ�������
        /// </summary>
        public int GatheredUrlNaviCount { get; set; }

        /// <summary>
        /// ���ݵ������򣬵�����������ַ���ɼ��������������
        /// </summary>
        public int GatheredErrUrlNaviCount { get; set; }
    
        /// <summary>
        /// ��¼�ɼ�������
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

        ////Ϊ��Ҫ��string����Ϊ�п���Ϊ��
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

        ////��ʱ���ݴ洢��λ��
        ////�����Կ�������չ,���ɼ����洢,������ʱ�ļ�,
        ////����������֧�������ж�,����֮�󼴿�ʵ�ּ����ɼ�
        ////�����������ܻ�ռ�ô�������Դ,��Ҫʵ�ʲ���

        //private string m_SavePath;
        //public string SavePath
        //{
        //    get { return m_SavePath; }
        //    set { m_SavePath = value; }
        //}


        //���Դ���
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

        ////�ɼ������ʱ
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

        /////����������Ϣ������汾1.6����
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

        ////������ϢΪ1.8�汾����
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

        ////������Ϣ��V2.0���ӵ�
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

        ////����ΪV2.1����
        //private bool m_IsUrlNoneRepeat;
        //public bool IsUrlNoneRepeat
        //{
        //    get { return m_IsUrlNoneRepeat; }
        //    set { m_IsUrlNoneRepeat = value; }
        //}

        ////����ΪV5��������
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

        ////����ΪV5.0.1����
        //private bool m_IsSqlTrue;
        //public bool IsSqlTrue
        //{
        //    get { return m_IsSqlTrue; }
        //    set { m_IsSqlTrue = value; }
        //}

        ////����ΪV5.1����
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

        ////����ΪV5.2����
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


        //#region ��ҳ�ɼ��ĵ�ַ�͹���

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

        ////����ֽ�����
        //private List<cTaskSplitData> m_TaskSplitData;
        //public List<cTaskSplitData> TaskSplitData
        //{
        //    get { return m_TaskSplitData; }
        //    set { m_TaskSplitData = value; }
        //}

        ////����ΪV3.0����
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

        //#region ��ҳ�ɼ�����ʼ����ֹ��ַ

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





        //��ʱ������������ÿ�μ�¼�ɼ����������ݺ�ֹͣһ��
        //ʱ��Ĳɼ�������գ����¼�¼



        //����Ϊ5.31����
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

        ////v5.33����
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

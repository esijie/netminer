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

///���ܣ��ɼ������࣬��ǰ�汾Ϊ1.3 ע�⣺�ӵ�ǰ�İ汾��ʼ���ٶ���ǰ�ɰ汾������м���
/// ����汾��2009-10-10�ٴν�����������Ҫ����������������ƣ����Խ��ж��ֹ�����Ͽ���
/// ����汾��Ϊ��1.6���Ӵ˴�����������汾��ϵͳ�汾����ͳһ
///���ʱ�䣺2009-10-10
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶���2009-10-27 �޸ģ������˵�����ַ���Զ���ҳ��ʶ������汾������Ϊ1.61
///
///�޶���2010-04-15 �޸ģ�������HTTP Header������汾������Ϊ1.8
///�޶���2010-11-19 �޸ģ����ɼ��������չ���޸�Ϊ��smt
///�޶���2010-12-2 �����ɼ�����汾Ϊ2.0
///�޶���2011-10-7 �����ɼ�����汾Ϊ2.1 ����url����ѡ��
///�޶���2012-1-30 �����ɼ�����汾Ϊ3.0 ���������ļ�����ĵ�ַ�������������


namespace NetMiner.Gather.Task
{
    //[Serializable]
    
    ///�������һ���๦�ܵ��࣬Ӧ��������ƣ����ü��ɵķ�ʽ������
    ///����������һ���汾���޸�
    ///��������Ӧ����һ������Ļ��ࣨ���ܻ����ɳ����ࣩ����������������Ӧ������������������ִ���࣬�������������
    /// ��ǰ�ɼ�������ṩ��һ�֣����ڻ��ṩ���ֲɼ��������ԣ��Դ��໹�ݲ��޸�
    ///������˵��ǰ�����⣬������Ҫ�������࣬ͬʱ������ִ�е���Ϣ�ϲ����ˣ������Ҫע�⣬ע���л�����˵��

    public class cTask
    {
        cXmlIO xmlConfig;
        private Single m_SupportTaskVersion = Single.Parse ("5.5");
        private string m_workPath = string.Empty;

        //�����ɴ��������汾�ţ�ע���1.3��ʼ���������಻����ǰ����
        public Single SupportTaskVersion
        {
            get { return m_SupportTaskVersion; }
        }


        #region ��Ĺ��������
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

        //����Ϊ����ǰ״̬�����ԣ�������½�������Ӧ��Ϊδ����
        //�������д����ǣ���ǰδ��
        public int TaskState
        {
            get { return this.TaskState; }
            set { this.TaskState = value; }
        }

        //*****************************************************************************************************************
        //���¶���ΪTask����

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
        /// ����汾��Ϣ�����������ʱ������İ汾Ҳ��������������Ҫ��ÿһ������
        /// �汾����ʶ�𣬱����������ݵ�Ǩ�ƣ��������Ǽ�����һ�汾������������Ϣ��
        /// ������汾���̫���򲻻���м��ݣ���ר�ù���ʵ����������Ǩ��
        /// �˰汾����汾��Ϊ��1.2���Ƚ�1.0�汾����Ҫ�������������ݼӹ�������������
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

        //�������ݽ�֧��Access��MSsqlserver
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

        //�ɼ�ҳ�����ݵ���ʼλ��
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

        //����Ϊ�������ݣ���Ҫ������汾������Ϊ1.3
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

        //�Ƿ����������Ϣ
        private bool m_IsErrorLog;
        public bool IsErrorLog
        {
            get { return m_IsErrorLog; }
            set { m_IsErrorLog = value; }
        }

        //�Ƿ�ȥ���ظ�����
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

        ///����������Ϣ������汾1.6����
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

        //�Ƿ񵼳��ɼ�Url��ַ
        private bool m_IsExportGUrl;
        public bool IsExportGUrl
        {
            get { return m_IsExportGUrl; }
            set { m_IsExportGUrl = value; }
        }

        //�Ƿ񵼳��ɼ���ʱ��
        private bool m_IsExportGDateTime;
        public bool IsExportGDateTime
        {
            get { return m_IsExportGDateTime; }
            set { m_IsExportGDateTime = value; }
        }

        //�ɼ������ʱ
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


        //�ɼ�����V1.8 �������� HTTP Header
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

        //�ɼ�����V2.0���ӣ�����;�Ĭ������
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

        //�ɼ�����V2.1���ӣ��Ƿ����Url����
        private bool m_IsUrlNoneRepeat;
        public bool IsUrlNoneRepeat
        {
            get { return m_IsUrlNoneRepeat; }
            set { m_IsUrlNoneRepeat = value; }
        }

        //�ɼ�����V3.6����
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


        //V5���ӵĹ���
        //�콢�����ҵ��ž߱��Ĺ���
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

        #region ˮӡ����
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

        //����ģ�����Ϣ
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

        //V5.0.1����
        private  bool m_IsSqlTrue;
        public bool IsSqlTrue
        {
            get { return m_IsSqlTrue; }
            set { m_IsSqlTrue = value; }
        }

        //V5.1����
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

        //����ΪV5.2����
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
        /// ��ֲ�ʽִ�У�����ֽ������ݿ���ȡ����վ����������Ŀ�ģ���ȷ������е����ɼ�����ҳ�ɼ�ʱ�����ݵ�׼ȷ�ԡ�
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

        //����Ϊ5.31����
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

        //v5.33����
        private List<cThreadProxy> m_ThreadProxy;
        public List<cThreadProxy> ThreadProxy
        {
            get { return m_ThreadProxy; }
            set { m_ThreadProxy = value; }
        }

        //v5.33����
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

        //����Ϊ5.5����
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

        #region ����Ϊ�෽��
        
        /// <summary>
        /// �жϴ��������������µ������ļ��Ƿ����
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

            if (TClassName == null || TClassName == "" || TClassName =="�������")
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
        /// �ж������ļ��Ƿ���ڣ������жϵ���ʵ�ʵ������ļ�
        /// </summary>
        /// <param name="TaskName"></param>
        /// <returns></returns>
        public bool IsExistTaskFile(string TaskName)
        {
            string Path = GetTaskClassPath();
            string File = Path + "\\" + TaskName + ".smt";
            bool IsExists = System.IO.File.Exists(File);

            //���ж������ļ��Ƿ���ڴ�����
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
        /// �жϲɼ������Ƿ���ڣ������жϵ��Ǵ������Ƿ���index�ļ��д���
        /// </summary>
        /// <param name="TaskName"></param>
        /// <returns></returns>
        public bool IsExistTaskOnIndex(string TaskName)
        {

            //���ж������ļ��Ƿ���ڴ�����
            oTaskIndex tIndex = new oTaskIndex(this.m_workPath);
            tIndex.LoadIndexDocument(this.TaskClass);
            bool isexist = tIndex.isExistTask(TaskName);
            tIndex = null;

            return isexist;
        }

        //����������Ϣ���ڱ���������Ϣ��ͬʱ���Զ�ά�������������
        /// <summary>
        /// ���������ļ����ɼ��������ƴ�taskname��ȡ,ע��˷������Զ�ά�������µ�index�ļ�
        /// </summary>
        /// <param name="TaskPath">�����ָ��·��</param>
        /// <param name="isCheckRepeat">�Ƿ��ж������Ѿ�����</param>
        public void Save(string TaskPath,bool isCheckRepeat)
        {
            //��ȡ��Ҫ���������·��
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

            //�жϴ�·�����Ƿ��Ѿ������˴�������������򷵻ش�����Ϣ
            if (IsExistTaskFile( this.TaskName) && isCheckRepeat==true )
            {
                 throw new NetMinerException ("�����Ѿ����ڣ����ܽ���");
            }

            //ά�������Index.xml�ļ�
            int TaskID=InsertTaskIndex(tPath);

            //��ʼ����Task����
            //����Task�����XML�ĵ���ʽ
            //��ǰ����xml�ļ�ȫ�����õ�ƴд�ַ�������ʽ,��û�в���xml���캯��
            StringBuilder tXml = new StringBuilder();
            tXml.Append ("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                "<Task>" +
                "<State></State>" +       ///��״ֵ̬��ǰ��Ч,���ڽ�������ʹ��
                "<BaseInfo>" +
                "<Version>" + SupportTaskVersion.ToString () + "</Version>" +  
                "<ID>" + TaskID + "</ID>" +
                "<Name>" + this.TaskName + "</Name>" +
                "<TaskDemo>" + this.TaskDemo + "</TaskDemo>" +
                "<Class>" + this.TaskClass + "</Class>" +
                "<Type>" + this.TaskType + "</Type>" +

                "<Visual>" + this.IsVisual + "</Visual>" +                 //��Ϊ5.5����
                "<RunType>" + this.RunType + "</RunType>" +

                //ѡӴת�������·��
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

            //����ÿ���̵߳Ĵ�����Ϣ
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

            //����HTTP Header��Ϣ
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

            //����Ĺ���
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

                    //��ΪҪ�����Զ���ҳ�Ķϵ����ɣ�����Ҫ��¼��һҳ��Url������Ϊ�����½�����ֵ��ԶΪ�գ���ֵ�ɲɼ�����ά��
                    tXml.Append ( "<NextPageUrl></NextPageUrl>"); 

                    //Ĭ�ϲ���һ���ڵ㣬��ʾ�����ӵ�ַ��δ���вɼ�����Ϊ��ϵͳ�����������Ĭ��ΪUnGather
                    tXml.Append ( "<IsGathered>" + (int)cGlobalParas.UrlGatherResult.UnGather + "</IsGathered>");

                    //�������ַ�ĵ�������
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
                            
                            //V5.2����
                            tXml.Append("<RunType><![CDATA[" + (int)this.WebpageLink[i].NavigRules[j].RunRule + "]]></RunType>");
                            tXml.Append("<OtherNaviRule><![CDATA[" + this.WebpageLink[i].NavigRules[j].OtherNaviRule + "]]></OtherNaviRule>");
                            tXml.Append ( "</NavigationRule>");
                        }
                        tXml.Append ("</NavigationRules>");
                    }

                    //�������ַ�Ķ�ҳ�ɼ�����
                    if (this.WebpageLink[i].IsMultiGather == true)
                    {
                        
                        tXml.Append ( "<MultiPageRules>");
                        for (int j = 0; j < this.WebpageLink[i].MultiPageRules.Count; j++)
                        {
                            tXml.Append ( "<MultiPageRule>");
                            tXml.Append ( "<MultiRuleName>" + this.WebpageLink[i].MultiPageRules[j].RuleName + "</MultiRuleName>");
                            
                            //V5.2����
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

                    //�ɼ�����V3.0���ӣ����������ļ��洢·������������
                    tXml.Append ( "<MultiPageName>" + this.WebpageCutFlag[i].MultiPageName + "</MultiPageName>");
                    tXml.Append ( "<DownloadFileSavePath>" + this.WebpageCutFlag[i].DownloadFileSavePath + "</DownloadFileSavePath>");
                    //tXml.Append ( "<DownloadFileDealType>" + this.WebpageCutFlag[i].DownloadFileDealType + "</DownloadFileDealType>");
                    //tXml.Append ( "<IsOcrText>" + this.WebpageCutFlag[i].IsOcrText + "</IsOcrText>");

                    //3.1����
                    //tXml.Append ( "<OcrScale>" + this.WebpageCutFlag[i].OcrScale + "</OcrScale>");

                    tXml.Append ( "<IsAutoDownloadImage>" + this.WebpageCutFlag[i].IsAutoDownloadImage + "</IsAutoDownloadImage>");

                    //V5.0����
                    tXml.Append ( "<IsAutoDownloadFirstImage>" + this.WebpageCutFlag[i].IsAuthoFirstImage + "</IsAutoDownloadFirstImage>");

                    //���������������

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

        //������������Ϣ���������ļ��������������ݵ����⣬�˷���
        //��Ҫ����֧����������ʹ��
        /// <summary>
        /// ����ǰ���ص����񱣴浽ָ����·����ע���������Ʋ��ᷢ���仯���Ҳ�ά��index�ļ�������
        /// �жϴ������ļ��Ƿ���ڣ���������򱨴�
        /// </summary>
        /// <param name="TaskPath"></param>
        public void SaveTaskFile(string TaskPath)
        {
            //��ȡ��Ҫ���������·��
            string tPath = "";

            if (TaskPath == "" || TaskPath == null)
            {
                tPath = GetTaskClassPath() + "\\";
            }
            else
            {
                tPath = TaskPath;
            }
           

            //�жϴ�·�����Ƿ��Ѿ������˴�������������򷵻ش�����Ϣ
            if (IsExistTaskFile( this.TaskName))
            {
                throw new NetMinerException("�����Ѿ����ڣ����ܽ���");
            }

            string tXml = GetTaskXML();
            cFile.SaveFileBinary(tPath + this.TaskName + ".smt", tXml.ToString(), true);

        }

        /// <summary>
        /// ���ɼ����񱣴浽ָ�����ļ����Ҳ�ά��index�ļ�����������ļ��Ѿ����ڣ��򸲸�
        /// </summary>
        /// <param name="TaskName">������·��+��������</param>
        public void SaveTask(string TaskName)
        {
            string tXml = GetTaskXML();
            cFile.SaveFileBinary(TaskName, tXml.ToString(), true);
        }

        /// <summary>
        /// ��ȡ�ɼ������xml�ļ�
        /// </summary>
        /// <returns></returns>
        public string GetTaskXML()
        {
            int i = 0;
            #region ��������ͨ����Ϣ

            //��ʼ����Task����
            //����Task�����XML�ĵ���ʽ
            //��ǰ����xml�ļ�ȫ�����õ�ƴд�ַ�������ʽ,��û�в���xml���캯��
            StringBuilder  tXml=new StringBuilder ();
            tXml.Append ( "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                "<Task>" +
                "<State></State>" +       ///��״ֵ̬��ǰ��Ч,���ڽ�������ʹ��
                "<BaseInfo>" +
                "<Version>" + SupportTaskVersion.ToString() + "</Version>" +
                "<ID>" + TaskID + "</ID>" +
                "<Name>" + this.TaskName + "</Name>" +
                "<TaskDemo>" + this.TaskDemo + "</TaskDemo>" +
                "<Class>" + this.TaskClass + "</Class>" +
                "<Type>" + this.TaskType + "</Type>" +

                "<Visual>" + this.IsVisual + "</Visual>" +                 //��Ϊ5.5����

                "<RunType>" + this.RunType + "</RunType>" +

                //ѡӴת�������·��
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


            //����ÿ���̵߳Ĵ�����Ϣ
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

            //����HTTP Header��Ϣ
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

            //����Ĺ���
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


                    //Ĭ�ϲ���һ���ڵ㣬��ʾ�����ӵ�ַ��δ���вɼ�����Ϊ��ϵͳ�����������Ĭ��ΪUnGather
                    tXml.Append ( "<IsGathered>" + (int)cGlobalParas.UrlGatherResult.UnGather + "</IsGathered>");

                    //�������ַ�ĵ�������
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

                    //�������ַ�Ķ�ҳ�ɼ�����
                    if (this.WebpageLink[i].IsMultiGather == true)
                    {
                        tXml.Append ( "<MultiPageRules>");
                        for (int j = 0; j < this.WebpageLink[i].MultiPageRules.Count; j++)
                        {
                            tXml.Append ( "<MultiPageRule>");
                            tXml.Append ( "<MultiRuleName>" + this.WebpageLink[i].MultiPageRules[j].RuleName + "</MultiRuleName>");

                            //V5.2����
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

                    //�ɼ�����V3.0���ӣ����������ļ��洢·������������
                    tXml.Append ( "<MultiPageName>" + this.WebpageCutFlag[i].MultiPageName + "</MultiPageName>");
                    tXml.Append ( "<DownloadFileSavePath>" + this.WebpageCutFlag[i].DownloadFileSavePath + "</DownloadFileSavePath>");
                    //tXml.Append ( "<DownloadFileDealType>" + this.WebpageCutFlag[i].DownloadFileDealType + "</DownloadFileDealType>");
                    //tXml.Append ( "<IsOcrText>" + this.WebpageCutFlag[i].IsOcrText + "</IsOcrText>");

                    ////3.1����
                    //tXml.Append ( "<OcrScale>" + this.WebpageCutFlag[i].OcrScale + "</OcrScale>");

                    tXml.Append ( "<IsAutoDownloadImage>" + this.WebpageCutFlag[i].IsAutoDownloadImage + "</IsAutoDownloadImage>");

                    //V5.0����
                    tXml.Append("<IsAutoDownloadFirstImage>" + this.WebpageCutFlag[i].IsAuthoFirstImage + "</IsAutoDownloadFirstImage>");


                    //���������������

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

        //�����������������
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TaskName">��������</param>
        /// <param name="OldTaskClass">ԭ���������</param>
        /// <param name="NewTaskClass">���������</param>
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

        //���������������һ�������ԭ�з��࿽������һ��������
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
                FileName = TaskName + "-����.smt";
                
                System.IO.File.Copy(oldPath + "\\" + TaskName + ".smt", NewPath + "\\" + FileName);
                TaskName = TaskName + "-����";
                
            }
            else
            {
                FileName = TaskName + ".smt";
                System.IO.File.Copy(oldPath + "\\" + FileName, NewPath + "\\" + FileName);
            }

            LoadTask(NewPath + "\\" + FileName);

            //�޸����������
            this.TaskName = TaskName;

            if (NewTaskClass == "")
                this.TaskClass = "";
            else
                this.TaskClass = NewTaskClass;

            Save("",false );
            //SaveTask(NewPath + "\\" + FileName);
            
        }

        //����������Ϣ�����������ļ��������½���������������id
        public int InsertTaskIndex(string tPath)
        {

            oTaskIndex tIndex;

            //�жϴ�·�����Ƿ��������������ļ�
            if (!IsExistTaskIndex(tPath))
            {
                //��������������ļ�������Ҫ����һ���ļ�
                tIndex = new oTaskIndex(this.m_workPath);
                tIndex.NewIndexFile(tPath);
            }
            else
            {
                tIndex = new oTaskIndex(this.m_workPath, tPath + "\\index.xml");
            }

            tIndex.GetTaskDataByClass(this.TaskClass);

            int MaxTaskID = tIndex.GetTaskClassCount();

            //����TaskIndex�ļ�����,�˲�������Ӧ�ð�����TaskIndex����
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

        //���½�һ������ʱ�����ô˷���
        public void New()
        {
            //this.TaskState =(int) cGlobalParas.TaskState.TaskUnStart;

            if (xmlConfig != null)
            {
                xmlConfig = null;
            }
        }

        //����һ�����񵽴�����
        public void LoadTask(String FileName)
        {
            if (File.Exists(FileName))
            {
                string strXML = cFile.ReadFileBinary(FileName);
                LoadTaskInfo(strXML);
            }
            else
            {
                throw new Exception("��ָ���Ĳɼ������ļ������ڣ�");
            }
           
        }

        //����һ�������������񵽴����У����ش�����Ϣ
        //�˷�����taskrunר��
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
                throw new Exception("��ָ���Ĳɼ������ļ������ڣ�");
            }
        }

        /// <summary>
        /// ����������ַ���xml��ʽ
        /// </summary>
        /// <param name="strXML"></param>
        public void LoadTaskInfo(string strXML)
        {
            bool isDBUrl = false;

            //����һ����������װ��һ������
            try
            {
                
                xmlConfig = new cXmlIO();
                xmlConfig.LoadXML(strXML);

                //��ȡTaskClass�ڵ�
                //TaskClass = xmlConfig.GetData("descendant::TaskClasses");
            }
            catch (System.Exception ex)
            {
             
                    throw ex;
                
            }

            //����������Ϣ
            this.TaskID =Int64.Parse ( xmlConfig.GetNodeValue("Task/BaseInfo/ID"));
            this.TaskName = xmlConfig.GetNodeValue("Task/BaseInfo/Name");

            ///��������汾��Ϣ��ע�⣺1.0���ǲ����ڰ汾��Ϣ�����ģ������������1.0������
            ///�����
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
                throw new NetMinerException("����������İ汾����ϵͳҪ��İ汾���������������������ԣ�");
            }


            this.TaskDemo = xmlConfig.GetNodeValue("Task/BaseInfo/TaskDemo");
            this.TaskClass = xmlConfig.GetNodeValue("Task/BaseInfo/Class");
            this.TaskType=(cGlobalParas.TaskType) int.Parse (xmlConfig.GetNodeValue("Task/BaseInfo/Type"));

            this.IsVisual = (xmlConfig.GetNodeValue("Task/BaseInfo/Visual") == "True" ? true : false);

            this.RunType = (cGlobalParas.TaskRunType)int.Parse (xmlConfig.GetNodeValue("Task/BaseInfo/RunType"));

            //���������·��������Ҫ����ϵͳ·��
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

            //��ʼ���ط���ģ����Ϣ
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

            //���ظ߼�������Ϣ
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
            
            //��������Ŀ������Ϊ�汾û�����������ɼ������ʽ�仯�ˣ����ص�ʱ�������������˳����ά��
            //������һ���汾�н������������������ɼ����񼴿�
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

            //V2.0����
            this.IsProxy = (xmlConfig.GetNodeValue("Task/Advance/IsProxy") == "True" ? true : false);
            this.IsProxyFirst = (xmlConfig.GetNodeValue("Task/Advance/IsProxyFirst") == "True" ? true : false);

            //V2.1����
            this.IsUrlNoneRepeat = (xmlConfig.GetNodeValue("Task/Advance/IsUrlNoneRepeat") == "True" ? true : false);
            
            //��������Ŀ������Ϊ�汾û�����������ɼ������ʽ�仯�ˣ����ص�ʱ�������������˳����ά��
            //������һ���汾�н������������������ɼ����񼴿�
            try
            {
                this.IsSucceedUrlRepeat = (xmlConfig.GetNodeValue("Task/Advance/IsUrlSucceedRepeat") == "True" ? true : false);
            }
            catch
            {
                this.IsSucceedUrlRepeat = false;
            }

            //V5����
            this.IsUrlAutoRedirect = (xmlConfig.GetNodeValue("Task/Advance/IsUrlAutoRedirect") == "True" ? true : false);

            //V5.1����
            this.IsGatherErrStop =(xmlConfig.GetNodeValue("Task/Advance/IsGatherErrStop") == "True" ? true : false);
            this.GatherErrStopCount = int.Parse(xmlConfig.GetNodeValue("Task/Advance/GatherErrStopCount"));
            this.GatherErrStopRule = (cGlobalParas.StopRule)int.Parse(xmlConfig.GetNodeValue("Task/Advance/GatherErrStopRule"));
            this.IsInsertDataErrStop = (xmlConfig.GetNodeValue("Task/Advance/IsInsertDataErrStop") == "True" ? true : false);
            this.InsertDataErrStopConut = int.Parse(xmlConfig.GetNodeValue("Task/Advance/InsertDataErrStopConut"));
            this.IsGatherRepeatStop = (xmlConfig.GetNodeValue("Task/Advance/IsGatherRepeatStop") == "True" ? true : false);
            this.GatherRepeatStopRule = (cGlobalParas.StopRule)int.Parse(xmlConfig.GetNodeValue("Task/Advance/IsGatherRepeatStopRule"));
            
            //V5.2����
            this.IsIgnoreErr = (xmlConfig.GetNodeValue("Task/Advance/IsIgnoreErr") == "True" ? true : false);
            this.IsAutoUpdateHeader = (xmlConfig.GetNodeValue("Task/Advance/IsAutoUpdateHeader") == "True" ? true : false);
            this.IsNoneAllowSplit = (xmlConfig.GetNodeValue("Task/Advance/IsNoneAllowSplit") == "True" ? true : false);
            this.IsSplitDbUrls = (xmlConfig.GetNodeValue("Task/Advance/IsSplitDbUrls") == "True" ? true : false);

            //V5.3����
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

            //������V5.32��������⣬����汾���ˣ����汾��δ��������ɼ���
            //ʧ�ܣ����ԣ�����������
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

            //����HTTP Header��Ϣ
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
            
            //����Trigger��Ϣ
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

            //���ز������Ϣ
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
                    
                    //���ص�������
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


                    //���ض�ҳ�ɼ�����
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

                    //�ɼ�����V2.6���ӣ����������ļ��Ĵ洢·������������
                    c.DownloadFileSavePath = dw[i].Row["DownloadFileSavePath"].ToString();
                    //c.DownloadFileDealType = dw[i].Row["DownloadFileDealType"].ToString();
                    c.MultiPageName = dw[i].Row["MultiPageName"].ToString();
                   
                    //3.1����
                    if (dw[i].Row["IsAutoDownloadImage"].ToString() == "True")
                        c.IsAutoDownloadImage = true;
                    else
                        c.IsAutoDownloadImage = false;

                    if (dw[i].Row["IsAutoDownloadFirstImage"].ToString() == "True")
                        c.IsAuthoFirstImage = true;
                    else
                        c.IsAuthoFirstImage = false;

                    //���������������
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

            //�����ַ��dburl�����¸���urlcount
            if (isDBUrl == true)
            {
                int urlsCount=0;
                //����urlcount
                for (int index = 0; index<this.WebpageLink.Count; index++)
                {
                    cUrlAnalyze gUrl=new cUrlAnalyze(this.m_workPath);

                    urlsCount += gUrl.GetUrlCount(this.WebpageLink[index].Weblink.ToString());
                }

                this.UrlCount=urlsCount ;

            }
        }

        /// <summary>
        /// ����V5.1��ǰ�汾�Ĳɼ����񣬴�xml��ʽ
        /// </summary>
        /// <param name="FileName"></param>
        //private void LoadOldTaskInfo(string FileName)
        //{
        //    //����һ����������װ��һ������
        //    try
        //    {
        //        xmlConfig = new cXmlIO(FileName);

        //        //��ȡTaskClass�ڵ�
        //        //TaskClass = xmlConfig.GetData("descendant::TaskClasses");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        if (!File.Exists(FileName))
        //        {
        //            throw new System.IO.IOException("��ָ���������ļ������ڣ�");
        //        }
        //        else
        //        {
        //            throw ex;
        //        }
        //    }

        //    //����������Ϣ
        //    this.TaskID = Int64.Parse(xmlConfig.GetNodeValue("Task/BaseInfo/ID"));
        //    this.TaskName = xmlConfig.GetNodeValue("Task/BaseInfo/Name");

        //    ///��������汾��Ϣ��ע�⣺1.0���ǲ����ڰ汾��Ϣ�����ģ������������1.0������
        //    ///�����
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
        //        throw new NetMinerException("����������İ汾����ϵͳҪ��İ汾���������������������ԣ�");
        //    }


        //    this.TaskDemo = xmlConfig.GetNodeValue("Task/BaseInfo/TaskDemo");
        //    this.TaskClass = xmlConfig.GetNodeValue("Task/BaseInfo/Class");
        //    this.TaskType = xmlConfig.GetNodeValue("Task/BaseInfo/Type");
        //    this.RunType = xmlConfig.GetNodeValue("Task/BaseInfo/RunType");

        //    //���������·��������Ҫ����ϵͳ·��
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

        //    //��ʼ���ط���ģ����Ϣ
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

        //    //���ظ߼�������Ϣ
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

        //    //V2.0����
        //    this.IsProxy = (xmlConfig.GetNodeValue("Task/Advance/IsProxy") == "True" ? true : false);
        //    this.IsProxyFirst = (xmlConfig.GetNodeValue("Task/Advance/IsProxyFirst") == "True" ? true : false);

        //    //V2.1����
        //    this.IsUrlNoneRepeat = (xmlConfig.GetNodeValue("Task/Advance/IsUrlNoneRepeat") == "True" ? true : false);

        //    //V5����
        //    this.IsUrlAutoRedirect = (xmlConfig.GetNodeValue("Task/Advance/IsUrlAutoRedirect") == "True" ? true : false);

        //    //����HTTP Header��Ϣ
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

        //    //����Trigger��Ϣ
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

        //    //���ز������Ϣ
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

        //            //���ص�������
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


        //            //���ض�ҳ�ɼ�����
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

        //            //�ɼ�����V2.6���ӣ����������ļ��Ĵ洢·������������
        //            c.DownloadFileSavePath = dw[i].Row["DownloadFileSavePath"].ToString();
        //            //c.DownloadFileDealType = dw[i].Row["DownloadFileDealType"].ToString();
        //            c.MultiPageName = dw[i].Row["MultiPageName"].ToString();
                  
        //            if (dw[i].Row["IsAutoDownloadImage"].ToString() == "True")
        //                c.IsAutoDownloadImage = true;
        //            else
        //                c.IsAutoDownloadImage = false;

        //            //���������������
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

        //ɾ��һ������
        //ɾ�������ʱ��ϵͳ����һ�����������Զ�����һ��
        //�����ļ�����~
        public bool DeleTask(string TaskPath,string TaskName)
        {
            //����ɾ�����������ڷ����µ�index.xml�е���������Ȼ����ɾ������������ļ�
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

            //��ɾ�������ļ��е�������������
            oTaskIndex tIndex = new oTaskIndex(this.m_workPath , tPath + "\\index.xml");
            tIndex.DeleTaskIndex(TaskName);
            tIndex =null;

            //����Ǳ༭״̬��Ϊ�˷�ֹɾ�����ļ������񱣴�ʧ�ܣ���
            //�����ļ�����ʧ�����⣬�����Ȳ�ɾ�����ļ���ֻ�ǽ������

            //ɾ������������ļ�
            string FileName =TaskPath   + "\\" + TaskName + ".smt" ;
            string tmpFileName=TaskPath   + "\\~" + TaskName + ".smt" ;

            try
            {
                //ɾ��������ʱ�ļ�
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
                //���������ʱ�ļ����ݲ���ʧ�ܣ���������У�����Ӱ�쵽���յ��ļ�����
                //������ļ�����Ҳʧ�ܣ���ֻ�ܱ�����
            }

            //ɾ�����������ļ�
            if (File.Exists(FileName))
            {
                File.SetAttributes(FileName, System.IO.FileAttributes.Normal);
                System.IO.File.Delete(FileName);
            }

            //���ļ�����Ϊ����
            //System.IO.File.SetAttributes(tmpFileName, System.IO.FileAttributes.Hidden);
            return true;
        }

        //����taskid�޸����������
        public bool RenameTask(string TClass,string OldTaskName, string NewTaskName)
        {
            try
            {
                //������������ȡ���������·��
                oTaskClass tc = new oTaskClass(this.m_workPath);
                string tClassPath = "";

                //�ж��µ�����·���Ƿ���ڣ���������򱨴�
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
                    throw new NetMinerException("���޸ĵ����������Ѿ����ڣ��������޸ģ�");

                oTaskIndex xmlTasks = new oTaskIndex(this.m_workPath);
                xmlTasks.LoadIndexDocument(TClass);

                bool isExistTask = xmlTasks.isExistTask(NewTaskName);
           
                if (isExistTask)
                {
                    xmlTasks = null;
                    throw new NetMinerException("���޸ĵ����������Ѿ����ڣ��������޸ģ�");
                }
                
                xmlTasks = null;

                //��ʼ�޸����������
                //�ȿ�ʼ�޸�index.xml������
                cXmlIO xmlIndex = new cXmlIO(tClassPath + "\\index.xml");
                xmlIndex.EditNodeValue("TaskIndex", "Name", OldTaskName, "Name", NewTaskName);
                xmlIndex.Save();
                xmlIndex = null;

                //��ʼ�޸����������
                
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

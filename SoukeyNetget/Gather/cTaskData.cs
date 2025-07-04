using System;
using System.Collections.Generic;
using System.Text;
using SoukeyNetget.Task;

///功能：采集任务数据
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Gather
{
    public class cTaskData
    {
        public cTaskData()
        {
            m_TaskSplitData = new List<cTaskSplitData>();
            m_Weblink=new List<Task.cWebLink> ();
            m_CutFlag = new List<Task.cWebpageCutFlag>();
        }

        ~cTaskData()
        {
            m_TaskSplitData = null;
        }

        /// <summary>
        /// 任务编号
        /// </summary>
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

        private string m_TaskDemo;
        public string TaskDemo
        {
            get { return m_TaskDemo; }
            set { m_TaskDemo = value; }
        }

        private cGlobalParas.TaskType m_TaskType;
        public cGlobalParas.TaskType TaskType
        {
            get { return m_TaskType; }
            set { m_TaskType = value; }
        }

        //增量采集任务的判断条件
        //private cGlobalParas.IncrByCondition m_IncrByCondition;
        //public cGlobalParas.IncrByCondition IncrByCondition
        //{
        //    get { return m_IncrByCondition; }
        //    set { m_IncrByCondition = value; }
        //}

        private cGlobalParas.TaskRunType m_RunType;
        public cGlobalParas.TaskRunType RunType
        {
            get { return m_RunType; }
            set { m_RunType = value; }
        }

        private cGlobalParas.TaskState m_TaskState;
        public cGlobalParas.TaskState TaskState
        {
            get { return m_TaskState; }
            set { m_TaskState = value; }
        }

        //需要采集的网页数量
        private int m_UrlCount;
        public int UrlCount
        {
            get { return m_UrlCount; }
            set { m_UrlCount = value; }
        }

        //实际需要采集的网址数量
        private int m_TrueUrlCount;
        public int TrueUrlCount
        {
            get { return m_TrueUrlCount; }
            set { m_TrueUrlCount = value; }
        }

        //已经采集的数量
        private int m_GatheredUrlCount;
        public int GatheredUrlCount
        {
            get { return m_GatheredUrlCount; }
            set { m_GatheredUrlCount = value; }
        }

        //已采集真实的网址数量
        private int m_GatheredTrueUrlCount;
        public int GatheredTrueUrlCount
        {
            get { return m_GatheredTrueUrlCount; }
            set { m_GatheredTrueUrlCount = value; }
        }

        /// 采集错误的url数量
        private int m_GatherErrUrlCount;
        public int GatherErrUrlCount
        {
            get { return m_GatherErrUrlCount; }
            set { m_GatherErrUrlCount = value; }
        }

        //已经采集真实发生错误的数量
        private int m_GatheredTrueErrUrlCount;
        public int GatheredTrueErrUrlCount
        {
            get { return m_GatheredTrueErrUrlCount; }
            set { m_GatheredTrueErrUrlCount = value; }
        }

        private string m_tempFileName;
        public string tempFileName
        {
            get { return m_tempFileName; }
            set { m_tempFileName = value; }
        }

        private string m_ExportFileName;
        public string ExportFileName
        {
            get { return m_ExportFileName; }
            set { m_ExportFileName = value; }
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

        private bool m_IsLogin;
        public bool IsLogin
        {
            get { return m_IsLogin; }
            set { m_IsLogin = value; }
        }

        private string m_LoginUrl;
        public string LoginUrl
        {
            get { return m_LoginUrl; }
            set { m_LoginUrl = value; }
        }

        private bool m_IsUrlEncode;
        public bool IsUrlEncode
        {
            get { return m_IsUrlEncode; }
            set { m_IsUrlEncode = value; }
        }

        //为何要用string，因为有可能为空
        private string m_UrlEncode;
        public string UrlEncode
        {
            get { return m_UrlEncode; }
            set { m_UrlEncode = value; }
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

        //临时数据存储的位置
        //此属性可用于扩展,即采集即存储,保存临时文件,
        //这样做可以支持任务中断,重启之后即可实现继续采集
        //但这样做可能会占用大量的资源,需要实际测试

        private string m_SavePath;
        public string SavePath
        {
            get { return m_SavePath; }
            set { m_SavePath = value; }
        }

        private int m_ThreadCount;
        public int ThreadCount
        {
            get { return m_ThreadCount; }
            set { m_ThreadCount = value; }
        }

        //重试次数
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

        private bool m_IsErrorLog;
        public bool IsErrorLog
        {
            get { return m_IsErrorLog; }
            set { m_IsErrorLog = value; }
        }

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

        //采集间隔延时
        private int m_GIntervalTime;
        public int GIntervalTime
        {
            get { return m_GIntervalTime; }
            set { m_GIntervalTime = value; }
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

        //以下信息为1.8版本增加
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

        private List<cHeader> m_Headers;
        public List<cHeader> Headers
        {
            get { return m_Headers; }
            set { m_Headers = value; }
        }

        #region 网页采集的地址和规则

        private List<Task.cWebLink> m_Weblink;
        public List<Task.cWebLink> Weblink
        {
            get { return m_Weblink; }
            set { m_Weblink = value; }
        }

        private List<Task.cWebpageCutFlag> m_CutFlag;
        public List<Task.cWebpageCutFlag> CutFlag
        {
            get { return m_CutFlag; }
            set { m_CutFlag = value; }
        }

        //任务分解数据
        private List<cTaskSplitData> m_TaskSplitData;
        public List<cTaskSplitData> TaskSplitData
        {
            get { return m_TaskSplitData; }
            set { m_TaskSplitData = value; }
        }

        #endregion

        #region 网页采集的起始和终止地址

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

        #endregion

    }
}

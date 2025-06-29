using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

///功能：采集任务 采集标志 存储管理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace NetMiner.Core.gTask.Entity
{
    [Serializable]
    public class eWebpageCutFlag
    {
        #region 构造和析构
        public eWebpageCutFlag()
        {
            m_ExportRules = new List<eFieldRule>();
        }

        ~eWebpageCutFlag()
        {
        }
        #endregion

        private int m_id;
        public int id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private string m_Title;
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

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

        private bool m_loopFlag;
        public bool loopFlag
        {
            get { return m_loopFlag; }
            set { m_loopFlag = value; }
        }

        private string m_Content;
        public string Content
        {
            get { return m_Content; }
            set { m_Content = value; }
        }

        //网页采集数据的限定标识
        private cGlobalParas.LimitSign m_LimitSign;
        public cGlobalParas.LimitSign LimitSign
        {
            get { return m_LimitSign; }
            set { m_LimitSign = value; }
        }

        private string m_RegionExpression;
        public string RegionExpression
        {
            get { return m_RegionExpression; }
            set { m_RegionExpression = value; }
        }

        //判断此字段是否需要进行数据合并，如果合并，则必须是其他字段相同才会进行合并
        private bool m_IsMergeData;
        public bool IsMergeData
        {
            get { return m_IsMergeData; }
            set { m_IsMergeData = value; }
        }

        //采集数据的类型
        private cGlobalParas.GDataType m_DataType;
        public cGlobalParas.GDataType DataType
        {
            get { return m_DataType; }
            set { m_DataType = value; }
        }

       
        ///以下信息是任务版本1.6新增，主要是扩展了数据输出的控制规则
        ///原有规则属性作废，但不删除
        private List<eFieldRule> m_ExportRules;
        public List<eFieldRule> ExportRules
        {
            get { return m_ExportRules; }
            set { m_ExportRules = value; }
        }

        //此属性是在1.63中增加，目的是为了支持多层级的数据采集
        //导航层级，默认为：0 ，即采集最终页数据
        private int m_NavLevel;
        public int NavLevel
        {
            get { return m_NavLevel; }
            set { m_NavLevel = value; }
        }

        //采集任务V2.6增加，处理下载文件的地址及重名规则
        //下载文件保存的地址
        private string m_DownloadFileSavePath;
        public string DownloadFileSavePath
        {
            get { return m_DownloadFileSavePath; }
            set { m_DownloadFileSavePath = value; }
        }

        //private string m_DownloadFileDealType;
        //public string  DownloadFileDealType
        //{
        //    get { return m_DownloadFileDealType; }
        //    set { m_DownloadFileDealType = value; }
        //}

        private string m_MultiPageName;
        public string MultiPageName
        {
            get { return m_MultiPageName; }
            set { m_MultiPageName = value; }
        }

        //以下为3.1所加

        /// <summary>
        /// 采集规则所属页面类型
        /// </summary>
        private cGlobalParas.GatherRuleByPage m_RuleByPage;
        public cGlobalParas.GatherRuleByPage RuleByPage
        {
            get { return m_RuleByPage; }
            set { m_RuleByPage = value; }
        }

        /// <summary>
        /// 采集规则类别：Normal-常规采集；XPath-xPath采集
        /// </summary>
        private cGlobalParas.GatherRuleType m_GatherRuleType;
        public cGlobalParas.GatherRuleType GatherRuleType
        {
            get { return m_GatherRuleType; }
            set { m_GatherRuleType = value; }
        }

        private string m_XPath;
        public string XPath
        {
            get { return m_XPath; }
            set { m_XPath = value; }
        }

        /// <summary>
        /// 采集节点的属性，只有才XPath时生效
        /// </summary>
        private string m_NodePrty;
        public string NodePrty
        {
            get { return m_NodePrty; }
            set { m_NodePrty = value; }
        }

        private bool m_IsAutoDownloadFileImage;
        public bool IsAutoDownloadFileImage
        {
            get { return m_IsAutoDownloadFileImage; }
            set { m_IsAutoDownloadFileImage = value; }
        }

        //以下为3.11所加

        //以下为V5.0增加
        private bool m_IsAutoDownloadOnlyImage;
        public bool IsAutoDownloadOnlyImage
        {
            get { return m_IsAutoDownloadOnlyImage; }
            set { m_IsAutoDownloadOnlyImage = value; }
        }
    }
}

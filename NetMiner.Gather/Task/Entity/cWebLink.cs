using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

///功能：采集任务 URL存储 管理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace NetMiner.Gather.Task
{
    public class cWebLink
    {

        #region 构造 析构
        public cWebLink()
        {
            m_IsGathered =(int) cGlobalParas.UrlGatherResult.UnGather;
            m_NavigRules = new List<cNavigRule>();
            m_MultiPageRules = new List<cMultiPageRule>();
        }

        ~cWebLink()
        {
            m_NavigRules = null;
            m_MultiPageRules = null;
        }

        #endregion

        #region 属性
        private int m_id;
        public int id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private string m_Weblink;
        /// <summary>
        /// 配置的采集网址
        /// </summary>
        public string Weblink
        {
            get { return m_Weblink; }
            set { m_Weblink = value; }
        }

        //是否为导航页，如果是导航页则需要根据导航规则来进行内容也的提取
        private bool m_IsNavigation;
        public bool IsNavigation
        {
            get { return m_IsNavigation; }
            set { m_IsNavigation = value; }
        }

        //多层导航规则，是一个集合类
        private List<cNavigRule> m_NavigRules;
        public List<cNavigRule> NavigRules
        {
            get { return m_NavigRules; }
            set { m_NavigRules = value; }
        }

        //是否提取下一页标识
        //注意：在此的下一页标识是指非导航页的下一页翻页表示
        //如果是导航页，则下一页标志记录在导航页的类中
        private bool m_IsNextPage;
        public bool IsNextpage
        {
            get { return m_IsNextPage; }
            set { m_IsNextPage = value; }
        }

        //下一页标识
        private string m_NextPageRule;
        public string NextPageRule
        {
            get { return m_NextPageRule; }
            set { m_NextPageRule = value; }
        }

        //采集任务V3.0增加，控制自动翻页的最大页码
        private string m_NextMaxPage;
        public string  NextMaxPage
        {
            get { return m_NextMaxPage; }
            set { m_NextMaxPage = value; }
        }

        //记录探测到的下一页的Url
        private string m_NextPageUrl;
        public string NextPageUrl
        {
            get { return m_NextPageUrl; }
            set { m_NextPageUrl = value; }
        }

        //标识当前网页地址是否已经采集,默认cGlobalParas.UrlGatherResult.UnGather
        private int m_IsGathered;
        public int IsGathered
        {
            get { return m_IsGathered; }
            set { m_IsGathered = value; }
        }

        private string m_CurrentRunning;
        public string CurrentRunning
        {
            get { return m_CurrentRunning; }
            set { m_CurrentRunning = value; }
        }

        //网络矿工采集任务V3.0增加，多页采集，增加一个标志
        private bool m_IsMultiGather;
        public bool IsMultiGather
        {
            get { return m_IsMultiGather; }
            set { m_IsMultiGather = value; }
        }

        //V5.0.1增加，进行多页数据的1对1的强制合并
        private bool m_IsData121;
        public bool IsData121
        {
            get { return m_IsData121; }
            set { m_IsData121 = value; }
        }


        //网址的多页采集规则
        private List<cMultiPageRule> m_MultiPageRules;
        public List<cMultiPageRule> MultiPageRules
        {
            get { return m_MultiPageRules; }
            set { m_MultiPageRules = value; }
        }

        /// <summary>
        /// 记录当前地址的来路页面，当前仅在分布式采集中使用
        /// </summary>
        private string m_referUrl;
        public string referUrl
        {
            get { return m_referUrl; }
            set { m_referUrl = value; }
        }

        #endregion

        

    }
}

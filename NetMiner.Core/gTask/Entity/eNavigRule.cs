using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

///修订：2010-12-5 采集任务升级为V2/0，增加了导航页翻页的功能，命名上
///与原有的内容页翻页命名冲突，所以，新命名有些不合适，请注意！
namespace NetMiner.Core.gTask.Entity
{
    [Serializable]
    /// <summary>
    /// 导航规则类
    /// </summary>
    public class eNavigRule
    {

        //所对应的Url地址
        private string m_Url;
        public string Url
        {
            get { return m_Url; }
            set { m_Url = value; }
        }

        private int m_Level;
        public int Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        #region 此为采集任务V2.0新增
        //导航页本身的翻页规则
        private bool m_IsNext;
        public bool IsNext
        {
            get { return m_IsNext; }
            set { m_IsNext = value; }
        }

        private string m_NextRule;
        public string NextRule
        {
            get { return m_NextRule; }
            set { m_NextRule = value; }
        }

        //采集任务V3.0增加，限定自动翻页的最大页码
        private string m_NextMaxPage;
        public string NextMaxPage
        {
            get { return m_NextMaxPage; }
            set { m_NextMaxPage = value; }
        }

        //private bool m_IsNextDoPostBack;
        //public bool IsNextDoPostBack
        //{
        //    get { return m_IsNextDoPostBack; }
        //    set { m_IsNextDoPostBack = value; }
        //}

        #endregion

        //导航出来的页面翻页规则
        private bool m_IsNaviNextPage;
        public bool IsNaviNextPage
        {
            get { return m_IsNaviNextPage; }
            set { m_IsNaviNextPage = value; }
        }

        private string m_NaviNextPage;
        public string NaviNextPage
        {
            get { return m_NaviNextPage; }
            set { m_NaviNextPage = value; }
        }

        //采集任务V3.0增加，限定自动翻页的最大页码
        private string m_NaviNextMaxPage;
        public string NaviNextMaxPage
        {
            get { return m_NaviNextMaxPage; }
            set { m_NaviNextMaxPage = value; }
        }

        //private bool m_IsNaviNextDoPostBack;
        //public bool IsNaviNextDoPostBack
        //{
        //    get { return m_IsNaviNextDoPostBack; }
        //    set { m_IsNaviNextDoPostBack = value; }
        //}

        //导航规则提取范围，特别注意：不同层级的导航提取的范围也是不一样的。
        private string m_NaviStartPos;
        public string NaviStartPos
        {
            get { return m_NaviStartPos; }
            set { m_NaviStartPos = value; }
        }

        private string m_NaviEndPos;
        public string NaviEndPos
        {
            get { return m_NaviEndPos; }
            set { m_NaviEndPos = value; }
        }

        private string m_NavigRule;
        public string NavigRule
        {
            get { return m_NavigRule; }
            set { m_NavigRule = value; }
        }

        //在任务1.63版本中，导航增加了采集功能，即实现了分页采集
        private bool m_IsGather;
        public bool IsGather
        {
            get { return m_IsGather; }
            set { m_IsGather = value; }
        }

        private string m_GatherStartPos;
        public string GatherStartPos
        {
            get { return m_GatherStartPos; }
            set { m_GatherStartPos = value; }
        }

        private string m_GatherEndPos;
        public string GatherEndPos
        {
            get { return m_GatherEndPos; }
            set { m_GatherEndPos = value; }
        }

        //增加两个翻页时候记录当前页码的参数，这两个参数只在企业版网址解析的时候使用，其他地方都不
        //使用这两个参数
        private int m_NextCurrentPage;
        public int NextCurrentPage
        {
            get { return m_NextCurrentPage; }
            set { m_NextCurrentPage = value; }
        }

        private int m_NaviNextCurrentPage;
        public int NaviNextCurrentPage
        {
            get { return m_NaviNextCurrentPage; }
            set { m_NaviNextCurrentPage = value; }
        }

        //V5.2增加
        private cGlobalParas.NaviRunRule m_RunRule;
        public cGlobalParas.NaviRunRule RunRule
        {
            get { return m_RunRule; }
            set { m_RunRule = value; }
        }

        private string m_OtherNaviRule;
        public string OtherNaviRule
        {
            get { return m_OtherNaviRule; }
            set { m_OtherNaviRule = value; }
        }
    }
}

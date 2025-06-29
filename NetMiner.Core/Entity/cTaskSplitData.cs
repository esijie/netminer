using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;
using NetMiner.Core.gTask.Entity;

///功能：采集任务 分解子任务 数据
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace NetMiner.Core.Entity
{
    //任务分解数据,分解依据是一个完整任务按照所执行的线程数进行分解
    //用于记录采集过程中的实时进度数据
    /// <summary>
    /// 分解后的任务队列数据
    /// </summary>
    public class cTaskSplitData
    {

        #region 构造 析构
        public cTaskSplitData()
        {
            m_CurIndex = 0;
            m_GatheredUrlCount = 0;
            m_GatheredErrUrlCount = 0;
        }

        ~cTaskSplitData()
        {
        }
        #endregion

        #region 采集分解任务属性

        //起始采集的位置索引
        private int m_BeginIndex;
        public int BeginIndex
        {
            get { return m_BeginIndex; }
            set { m_BeginIndex = value; }
        }

        //结束采集的位置索引
        private int m_EndIndex;
        public int EndIndex
        {
            get { return m_EndIndex; }
            set { m_EndIndex = value; }
        }

        private int m_CurIndex;
        public int CurIndex
        {
            get { return m_CurIndex; }
            set { m_CurIndex = value; }
        }

        //当前正在采集的Url地址
        private string m_CurUrl;
        public string CurUrl
        {
            get { return m_CurUrl; }
            set { m_CurUrl = value; }
        }
        
        /// <summary>
        /// 分解任务后，获取的网址数量，此网址数量不能修改，只能
        /// 通过分类来进行
        /// </summary>
        public int UrlCount
        {
            get { return (EndIndex - BeginIndex+1); }
        }

        /// <summary>
        /// 已经采集的数量，仅限于配置的网址，不包含导航出来的网址数量
        /// </summary>
        private int m_GatheredUrlCount;
        public int GatheredUrlCount
        {
            get { return m_GatheredUrlCount; }
            set { m_GatheredUrlCount = value; }
        }

        /// <summary>
        /// 采集错误的网址，仅限于配置的网址，不包括导航出来的网址
        /// </summary>
        private int m_GatheredErrUrlCount;
        public int GatheredErrUrlCount
        {
            get { return m_GatheredErrUrlCount; }
            set { m_GatheredErrUrlCount = value; }
        }

        /// <summary>
        /// 根据导航页导航出来的网址数量，需要采集
        /// </summary>
        private int m_UrlNaviCount;
        public int UrlNaviCount
        {
            get { return m_UrlNaviCount; }
            set { m_UrlNaviCount = value; }
        }

        /// <summary>
        /// 根据导航规则，导航出来的网址，已经采集的数量
        /// </summary>
        private int m_GatheredUrlNaviCount;
        public int GatheredUrlNaviCount
        {
            get { return m_GatheredUrlNaviCount; }
            set { m_GatheredUrlNaviCount = value; }
        }

        /// <summary>
        /// 根据导航规则，导航出来的网址，采集发生错误的数量
        /// </summary>
        private int m_GatheredErrUrlNaviCount;
        public int GatheredErrUrlNaviCount
        {
            get { return m_GatheredErrUrlNaviCount; }
            set { m_GatheredErrUrlNaviCount = value; }
        }

        //此分解任务需要采集的网页地址
        private List<eWebLink> m_Weblink;
        public List<eWebLink> Weblink
        {
            get { return m_Weblink; }
            set { m_Weblink = value; }
        }

        //此分解任务采集网页地址数据的截取标识
        private List<eWebpageCutFlag> m_CutFlag;
        public List<eWebpageCutFlag> CutFlag
        {
            get { return m_CutFlag; }
            set { m_CutFlag = value; }
        }
        #endregion

    }
}

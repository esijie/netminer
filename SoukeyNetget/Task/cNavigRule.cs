using System;
using System.Collections.Generic;
using System.Text;

namespace SoukeyNetget.Task
{
    public class cNavigRule
    {
        public cNavigRule()
        {
        }

        ~cNavigRule()
        {
        }

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
    }
}

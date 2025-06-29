using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace MinerSpider
{
    public class cNaviRuleFormConfig
    {
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

        private string m_NextMaxPage;
        public string NextMaxPage
        {
            get { return m_NextMaxPage; }
            set { m_NextMaxPage = value; }
        }

        private string m_NavRule;
        public string NavRule
        {
            get { return m_NavRule; }
            set { m_NavRule = value; }
        }

        private string m_SPos;
        public string SPos
        {
            get { return m_SPos; }
            set { m_SPos = value; }
        }

        private string m_EPos;
        public string EPos
        {
            get { return m_EPos; }
            set { m_EPos = value; }
        }

        private bool m_IsGather;
        public bool IsGather
        {
            get { return m_IsGather; }
            set { m_IsGather = value; }
        }

        private string m_GSPos;
        public string GSPos
        {
            get { return m_GSPos; }
            set { m_GSPos = value; }
        }

        private string m_GEPos;
        public string GEPos
        {
            get { return m_GEPos; }
            set { m_GEPos = value; }
        }

        private bool m_IsNaviNext;
        public bool IsNaviNext
        {
            get { return m_IsNaviNext; }
            set { m_IsNaviNext = value; }
        }

        private string m_NaviNextRule;
        public string NaviNextRule
        {
            get { return m_NaviNextRule; }
            set { m_NaviNextRule = value; }
        }

        private string m_NaviNextMaxPage;
        public string NaviNextMaxPage
        {
            get { return m_NaviNextMaxPage; }
            set { m_NaviNextMaxPage = value; }
        }

        private bool m_IsNextDoPostBack;
        public bool IsNextDoPostBack
        {
            get { return m_IsNextDoPostBack; }
            set { m_IsNextDoPostBack = value; }
        }

        private bool m_IsNaviNextDoPostBack;
        public bool IsNaviNextDoPostBack
        {
            get { return m_IsNaviNextDoPostBack; }
            set { m_IsNaviNextDoPostBack = value; }
        }

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

        //private string m_codeUrl;
        //public string CodeUrl
        //{
        //    get { return m_codeUrl; }
        //    set { m_codeUrl = value; }
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
    }
}

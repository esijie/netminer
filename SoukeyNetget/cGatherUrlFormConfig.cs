using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Gather;
using NetMiner.Gather.Task ;
using NetMiner.Core.gTask.Entity;

namespace MinerSpider
{
    public class cGatherUrlFormConfig
    {
        public cGatherUrlFormConfig()
        {
            m_nRules = new eNavigRules();
            m_MRules = new eMultiPageRules();

        }

        ~cGatherUrlFormConfig()
        {
            m_nRules = null;
            m_MRules = null;
        }


        private  string m_Url;
        public string Url
        {
            get { return m_Url; }
            set { m_Url = value; }
        }

        private bool m_IsNav;
        public bool IsNav
        {
            get { return m_IsNav; }
            set { m_IsNav = value; }
        }

        

        private eNavigRules m_nRules;
        public eNavigRules nRules
        {
            get { return m_nRules; }
            set { m_nRules = value; }
        }

        private bool m_IsNext;
        public bool IsNext
        {
            get { return m_IsNext; }
            set { m_IsNext = value; }
        }

        //private bool m_IsAspx;
        //public bool IsAspx
        //{
        //    get { return m_IsAspx; }
        //    set { m_IsAspx = value; }
        //}

        private string m_NextRule;
        public string NextRule
        {
            get { return m_NextRule; }
            set { m_NextRule = value; }
        }

        private int m_MaxPageCount;
        public int MaxPageCount
        {
            get { return m_MaxPageCount; }
            set { m_MaxPageCount = value; }
        }

        private bool m_IsMulti;
        public bool IsMulti
        {
            get { return m_IsMulti; }
            set { m_IsMulti = value; }
        }

        private bool m_isMulti121;
        public bool isMulti121
        {
            get { return m_isMulti121; }
            set { m_isMulti121 = value; }
        }

        private eMultiPageRules m_MRules;
        public eMultiPageRules MRules
        {
            get { return m_MRules; }
            set { m_MRules = value; }
        }
    }
}

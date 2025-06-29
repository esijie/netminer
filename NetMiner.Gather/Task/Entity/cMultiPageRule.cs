using System;
using System.Collections.Generic;
using System.Text;


///功能：采集任务 多页采集规则存储
///完成时间：2012-2-5
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：3.00.00.03
///修订：无
namespace NetMiner.Gather.Task
{
    public class cMultiPageRule
    {

        #region 属性
        private string m_RuleName;
        public string RuleName
        {
            get { return m_RuleName; }
            set { m_RuleName = value; }
        }

        //V5.2增加，多页对应采集页的级别
        private int m_mLevel;
        public int mLevel
        {
            get { return m_mLevel; }
            set { m_mLevel = value; }
        }

        private string m_Rule;
        public string Rule
        {
            get { return m_Rule; }
            set { m_Rule = value; }
        }

        #endregion

    }
}

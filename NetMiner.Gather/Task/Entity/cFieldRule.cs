using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.Gather.Task
{
    public class cFieldRule
    {
        public cFieldRule()
        {
        }

        ~cFieldRule()
        {
            
        }

        private string m_Field;
        public string Field
        {
            get { return m_Field; }
            set { m_Field = value; }
        }

        private int m_Level;
        public int Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        private string m_FieldRule;
        public string FieldRule
        {
            get { return m_FieldRule; }
            set { m_FieldRule = value; }
        }

        private cGlobalParas.ExportLimit m_FieldRuleType;
        public cGlobalParas.ExportLimit FieldRuleType
        {
            get { return m_FieldRuleType; }
            set { m_FieldRuleType = value; }
        }
    }
}

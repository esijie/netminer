using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Core.gTask.Entity
{
    [Serializable]
    public class eFieldRules
    {
        public eFieldRules()
        {
            m_FieldRule = new List<eFieldRule>();
        }

        ~eFieldRules()
        {
            m_FieldRule = null;
        }

        private string m_Field;
        public string Field
        {
            get { return m_Field; }
            set { m_Field = value; }
        }

        private List<eFieldRule> m_FieldRule;
        public List<eFieldRule> FieldRule
        {
            get { return m_FieldRule; }
            set { m_FieldRule = value; }
        }
    }
}

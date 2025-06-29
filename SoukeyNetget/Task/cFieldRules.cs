using System;
using System.Collections.Generic;
using System.Text;

namespace SoukeyNetget.Task
{
    public class cFieldRules
    {
        public cFieldRules()
        {
            m_FieldRule = new List<cFieldRule>();
        }

        ~cFieldRules()
        {
            m_FieldRule = null;
        }

        private string m_Field;
        public string Field
        {
            get { return m_Field; }
            set { m_Field = value; }
        }

        private List<cFieldRule> m_FieldRule;
        public List<cFieldRule> FieldRule
        {
            get { return m_FieldRule; }
            set { m_FieldRule = value; }
        }
    }
}

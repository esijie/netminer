using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Gather;
using NetMiner.Resource;

namespace NetMiner.Core.Radar.Entity
{
    public class eRule
    {
        public eRule()
        {
        }

        ~eRule()
        {
        }

        private string m_Field;
        public string Field
        {
            get { return m_Field; }
            set { m_Field = value; }
        }

        private cGlobalParas.MonitorRule m_Rule;
        public cGlobalParas.MonitorRule Rule
        {
            get { return m_Rule; }
            set { m_Rule = value; }
        }

        private string m_Content;
        public string Content
        {
            get {return m_Content ;}
            set {m_Content =value ;}
        }

        private int m_Num;
        public int Num
        {
            get {return m_Num ;}
            set {m_Num =value ;}
        }

    }
}

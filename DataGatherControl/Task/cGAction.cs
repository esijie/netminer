using System;
using System.Collections.Generic;
using System.Text;
using DataGatherControl;

namespace DataGatherControl.Task
{
    public class cGAction
    {
        private string m_Para;
        public string Para
        {
            get { return m_Para; }
            set { m_Para = value; }
        }

        private string m_xPath;
        public string xPath
        {
            get { return m_xPath; }
            set { m_xPath = value; }
        }

        private string m_ElemType;
        public string ElemType
        {
            get { return m_ElemType; }
            set { m_ElemType = value; }
        }

        private string m_ActionType;
        public string ActionType
        {
            get { return m_ActionType; }
            set { m_ActionType = value; }
        }

        private cGlobal.ParaValueType m_ParaValueType;
        public cGlobal.ParaValueType ParaValueType
        {
            get { return m_ParaValueType; }
            set { m_ParaValueType = value; }
        }

        private string m_ParaValue;
        public string ParaValue
        {
            get { return m_ParaValue; }
            set { m_ParaValue = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SoukeyNetget.Task
{
    public class cHeader
    {
        public cHeader()
        {
        }

        ~cHeader()
        {
        }

        private string m_Label;
        public string Label
        {
            get { return m_Label; }
            set { m_Label = value; }
        }

        private string m_LabelValue;
        public string LabelValue
        {
            get { return m_LabelValue; }
            set { m_LabelValue = value; }
        }
    }
}

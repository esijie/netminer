using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Gather.Task.Entity
{
    public class eHeader
    {
        public eHeader()
        {
        }

        ~eHeader()
        {
        }

        /// <summary>
        /// Ӧ�÷�Χ
        /// </summary>
        public string Range { get; set; }
       
        /// <summary>
        /// ��ǩ
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// ֵ
        /// </summary>
        public string LabelValue { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Core.gTask.Entity
{
    [Serializable]
    public class eHeader
    {


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

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
        /// 应用范围
        /// </summary>
        public string Range { get; set; }
       
        /// <summary>
        /// 标签
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string LabelValue { get; set; }
    }
}

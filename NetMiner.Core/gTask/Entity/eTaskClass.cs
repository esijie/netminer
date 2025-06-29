using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Core.gTask.Entity
{
    [Serializable]
    public class eTaskClass
    {
        public eTaskClass()
        {
        }
        ~eTaskClass()
        {
        }
        public int ID { get; set; }
        /// <summary>
        /// 采集任务的名称，注意是单名称，不是完整的分级名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 采集任务分类的相对路径，带有tasks
        /// </summary>
        public string tPath { get; set; }
        public List<eTaskClass> Children { get; set; }
    }


}

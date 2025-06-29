using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.Core.pTask.Entity
{
    [Serializable]
    /// <summary>
    /// 发布数据实体类
    /// </summary>
    public class ePublishData
    {
        /// <summary>
        /// 数据的名称
        /// </summary>
        public string DataLabel { get; set; }
        /// <summary>
        /// 数据值
        /// </summary>
        public string DataValue { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public cGlobalParas.PublishParaType DataType { get; set; }

  
    }
}

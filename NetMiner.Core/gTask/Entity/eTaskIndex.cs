using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.Core.gTask.Entity
{
    [Serializable]
    public class eTaskIndex
    {
        public int ID { get; set; }
        public string TaskName { get; set; }
        public cGlobalParas.TaskType TaskType { get; set; }
        public cGlobalParas.TaskRunType TaskRunType { get; set; }
        public string ExportFile { get; set; }
        public cGlobalParas.TaskState TaskState { get; set; }
        public int WebLinkCount { get; set; }
        public cGlobalParas.PublishType PublishType { get; set; }
    }
}

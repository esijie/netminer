using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.Core.Radar.Entity
{
    public class eRadarIndex
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public cGlobalParas.MonitorState State{get;set;}
        public cGlobalParas.MonitorType MonitorType { get; set; }
        public cGlobalParas.MonitorDealType DealType { get; set; }
        public cGlobalParas.WarningType WarningType { get; set; }
        public cGlobalParas.MonitorInvalidType InvalidType { get; set; }
    }
}

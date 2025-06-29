using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NetMiner.Gather;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Base;
using System.Xml.Linq;
using NetMiner.Core.Radar.Entity;
using System.Linq;
using System.IO;

namespace NetMiner.Core.Radar
{
    public class oRadarIndex : XmlUnity
    {
        private string m_workPath = string.Empty;

        public oRadarIndex(string workPath)
        {
            m_workPath = workPath;

            if (!File.Exists(m_workPath + NetMiner.Constant.g_RadarFile))
                NewIndexFile();

            base.LoadXML(m_workPath + NetMiner.Constant.g_RadarFile);
        }

        ~oRadarIndex()
        {
            base.Dispose();
        }

        #region 新建一个index文件,及在此文件下新建一个任务信息
        public void NewIndexFile()
        {

            XElement xe = new XElement("TaskIndex");
            base.NewXML(m_workPath + NetMiner.Constant.g_RadarFile, xe);

        }

        public int InsertTaskIndex(eRadarIndex er)
        {

            int MaxID = base.GetMaxID(base.xDoc.Root, "Task");
            MaxID = MaxID + 1;
            //求最大ID
            XElement xe = new XElement("Task");
            xe.Add(new XElement("ID", MaxID));
            xe.Add(new XElement("Name", er.Name));
            xe.Add(new XElement("State", (int)er.State));
            xe.Add(new XElement("MonitorType", (int)er.MonitorType));
            xe.Add(new XElement("DealType", (int)er.DealType));
            xe.Add(new XElement("WarningType", (int)er.WarningType));
            xe.Add(new XElement("InvalidType", (int)er.InvalidType));

            base.AddElement(base.xDoc.Root, xe);
            base.Save();

            return MaxID;


        }

        public void DeleTaskIndex(string TaskName)
        {
            XElement xe= base.SearchElement("Task", "Name", TaskName);
            base.RemoveElement(xe);
            base.Save();
        }


        #endregion


        public IEnumerable<eRadarIndex> GetRules()
        {

            IEnumerable<XElement> xes = base.GetAllElement("Task");
            IEnumerable<eRadarIndex> eTasks = xes.Select<XElement, eRadarIndex>(
                    s => Convert(s));

            return eTasks;

        }

        private eRadarIndex Convert(XElement xe)
        {
            eRadarIndex er = new Entity.eRadarIndex();
            er.ID = int.Parse (xe.Element("ID").Value.ToString());
            er.Name = xe.Element("Name").Value.ToString();
            er.State = (cGlobalParas.MonitorState)int.Parse (xe.Element("State").Value.ToString());
            er.MonitorType =(cGlobalParas.MonitorType) int.Parse(xe.Element("MonitorType").Value.ToString());
            er.DealType = (cGlobalParas.MonitorDealType)int.Parse(xe.Element("DealType").Value.ToString());
            er.WarningType = (cGlobalParas.WarningType)int.Parse(xe.Element("WarningType").Value.ToString());
            er.InvalidType = (cGlobalParas.MonitorInvalidType)int.Parse(xe.Element("InvalidType").Value.ToString());

            return er;
        }

     
        //设置某个监控规则为无效
        public void InvalidRule(string RuleName)
        {
            XElement xe=base.SearchElement("Task", "Name", RuleName);
            base.EditValue(xe.Element("State"), ((int)cGlobalParas.MonitorState.Invalid).ToString());
            base.Save();
          
        }

    }
}

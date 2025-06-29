using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Gather;
using System.Data;
using NetMiner.Common;
using NetMiner.Resource;
using NetMiner.Core.Radar;
using NetMiner.Core.Radar.Entity;
using NetMiner.Base;
using System.Xml.Linq;
using System.Linq;
using System.IO;

///监控规则类
///监控规则不区分类别，所有规则只存储在Monitor目录下
namespace NetMiner.Core.Radar
{
    /// <summary>
    /// 管理一个监控规则
    /// </summary>
    public class oRadar:XmlUnity
    {
        private string m_Path = string.Empty;
        private string m_workPath = string.Empty;

        public oRadar(string workPath)
        {
            m_workPath = workPath;
            m_Path = workPath + "Monitor";
           
        }

        ~oRadar()
        {
            base.Dispose();

        }

   

        #region 方法

   

        //判断监控规则文件是否存在
        public bool IsExistTaskFile(string FileName)
        {
            bool IsExists = System.IO.File.Exists(FileName);
            return IsExists;
        }

        //插入任务信息到任务索引文件，返回新建任务索引的任务id
        private int InsertTaskIndex(eRadar radar)
        {

            oRadarIndex tIndex;

            //判断此路径下是否存在任务的索引文件
            tIndex = new oRadarIndex(m_workPath);

            eRadarIndex er = new eRadarIndex();
            er.ID = 0;
            er.Name = radar.TaskName;
            er.State = (cGlobalParas.MonitorState)(int)radar.MonitorState;
            er.MonitorType = (cGlobalParas.MonitorType)(int)radar.MonitorType;
            er.DealType = (cGlobalParas.MonitorDealType) (int)radar.DealType;
            er.WarningType = (cGlobalParas.WarningType)(int)radar.WaringType;
            er.InvalidType = (cGlobalParas.MonitorInvalidType) (int)radar.InvalidType;

            int MaxTaskID= tIndex.InsertTaskIndex(er);
            tIndex.Dispose();
            tIndex = null;

            return MaxTaskID;

        }

        //任务的状态是在索引文件中进行记录
        public void InvalidRule(string RuleName)
        {
            oRadarIndex ci = new oRadarIndex(m_workPath);
            ci.InvalidRule(RuleName);
            ci = null;

        }


        public eRadar LoadRule(string Name)
        {
            string FileName = this.m_Path + "\\" + Name + ".xml";

            

            try
            {
                base.LoadXML(FileName);
            }
            catch (System.Exception ex)
            {
                if (!System.IO.File.Exists(FileName))
                {
                    throw new System.IO.IOException("您指定的任务文件不存在！");
                }
                else
                {
                    throw ex;
                }
            }

            eRadar er = new Entity.eRadar();

            //加载基础信息
            er.ID = base.GetValue("/Monitor/BaseInfo/ID");
            er.TaskName = base.GetValue("/Monitor/BaseInfo/Name");
            er.MonitorInterval = int.Parse (base.GetValue("/Monitor/BaseInfo/MonitorInterval"));
            er.DealType=( cGlobalParas.MonitorDealType ) (int.Parse (base.GetValue("/Monitor/BaseInfo/DealType")));
            er.TableName= base.GetValue("/Monitor/BaseInfo/TableName");
            er.Sql= base.GetValue("/Monitor/BaseInfo/Sql");
            er.SavePagePath= base.GetValue("/Monitor/BaseInfo/SavePagePath");
            er.ReceiveEmail= base.GetValue("/Monitor/BaseInfo/ReceiveEmail");
            er.WaringType= (cGlobalParas.WarningType ) (int.Parse (base.GetValue("/Monitor/BaseInfo/WarningType")));
            er.WaringEmail  = base.GetValue("/Monitor/BaseInfo/WarningEmail");
            er.InvalidType=(cGlobalParas.MonitorInvalidType)(int.Parse(base.GetValue("/Monitor/BaseInfo/InvalidType")));
            er.InvalidDate = base.GetValue("/Monitor/BaseInfo/InvalidDate");

            //加载任务信息
            IEnumerable<XElement> tasks= base.GetElement("/Monitor/MonitorTasks").Elements();

             foreach(XElement task in tasks)
            {
                eSource cs = new eSource ();
                cs.TaskClass = task.Element("TaskClass").Value.ToString();
                cs.TaskName  = task.Element("TaskName").Value.ToString();

                er.Source.Add(cs);
            }

            IEnumerable<XElement> Rules = base.GetElement("/Monitor/MonitorRules").Elements();

            foreach(XElement rule in Rules)
            {
                eRule cr = new eRule();
                cr.Field = rule.Element("Field").Value.ToString();
                cr.Rule  = (cGlobalParas.MonitorRule )( int.Parse (rule.Element("Rule").Value.ToString()));
                cr.Content  = rule.Element("Content").Value.ToString();
                cr.Num  = int.Parse (rule.Element("Num").Value.ToString());

                er.MRule.Add(cr);
            }

            return er;

        }

        //当新建一个任务时，调用此方法
        public void DelRule(string TName)
        {
            oRadarIndex ci = new oRadarIndex(m_workPath);
            ci.DeleTaskIndex(TName);
            ci = null;

            //删除物理文件
            System.IO.File.Delete(this.m_Path + "\\" + TName + ".xml");
        }

        public void DelRuleDB(string tName)
        {
            string fName = this.m_Path + "\\" + tName + ".db";
            System.IO.File.Delete(fName);
        }

        public void SaveRule(eRadar radar)
        {
            //判断此路径下是否已经存在了此任务，如果存在则返回错误信息
            if (IsExistTaskFile(this.m_Path + "\\" + radar.TaskName + ".xml"))
            {
                //throw new NetMinerException("监控规则已经存在，不能建立");
                File.Delete(this.m_Path + "\\" + radar.TaskName + ".xml");
            }

            //插入一个index节点

            int ID = InsertTaskIndex(radar);

            //开始增加监控规则，构造一个xml文件
            string tXml;
            //tXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
             tXml = "<Monitor>" +
                "<BaseInfo>" +
                "<ID>" + ID + "</ID>" +
                "<Name>" + radar.TaskName + "</Name>" +
                "<MonitorInterval>" + radar.MonitorInterval + "</MonitorInterval>" +
                "<DealType>" + (int)radar.DealType + "</DealType>" +
                "<TableName>" + radar.TableName + "</TableName>" +
                "<Sql>" + radar.Sql + "</Sql>" +
                "<SavePagePath><![CDATA[" + radar.SavePagePath + "]]></SavePagePath>" +
                "<ReceiveEmail>" + radar.ReceiveEmail + "</ReceiveEmail>" +
                "<WarningType>" + (int)radar.WaringType + "</WarningType>" +
                "<WarningEmail>" + radar.WaringEmail + "</WarningEmail>" +
                "<InvalidType>" + (int)radar.InvalidType + "</InvalidType>" +
                "<InvalidDate>" + radar.InvalidDate + "</InvalidDate>" +
                "</BaseInfo>" +
                "<MonitorTasks>" ;

            for (int i=0 ;i< radar.Source.Count ;i++)
            {
                tXml +="<Task>";
                tXml +="<TaskClass>" + radar.Source[i].TaskClass + "</TaskClass>";
                tXml +="<TaskName>" + radar.Source[i].TaskName + "</TaskName>";
                tXml +="</Task>";
            }

            tXml +="</MonitorTasks>";
            tXml += "<MonitorRules>";

            for (int j = 0; j < radar.MRule.Count; j++)
            {
                tXml += "<MonitorRule>";
                tXml +="<Field>" + radar.MRule[j].Field + "</Field>";
                tXml += "<Rule>" + (int)radar.MRule[j].Rule + "</Rule>";
                tXml += "<Content>" + radar.MRule[j].Content + "</Content>";
                tXml += "<Num>" + radar.MRule[j].Num + "</Num>";
                tXml += "</MonitorRule>";
            }

            tXml += "</MonitorRules>";
            tXml += "</Monitor>";

            TextReader tr = new StringReader(tXml);
            XElement xe = XElement.Load(tr);
            base.NewXML(this.m_Path + "\\" + radar.TaskName + ".xml", xe);
            base.Save();

        }

        #endregion


    }
}

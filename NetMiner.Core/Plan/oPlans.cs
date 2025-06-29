using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using NetMiner.Common;
using NetMiner.Resource;
using NetMiner.Core.Plan.Entity;
using NetMiner.Base;
using System.Xml.Linq;
using System.Linq;

///功能：计划类
///完成时间：2009-8-21
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：所有计划任务都存储在一个文件中，系统根路径\tasks\plan ，这样做的目的是为了可以更好的
///监听计划的执行情况，系统在启动时，将自动加载此文件，进行监听。
///
///版本：01.10.00
///修订：无
namespace NetMiner.Core.Plan
{
    public class oPlans:XmlUnity
    {
        //cXmlIO xmlPlan;
        List<ePlan> m_Plans;
        private string m_workPath = string.Empty;

        #region 构造和析构
        public oPlans(string workPath)
        {
            m_workPath = workPath;
            m_Plans = new List<ePlan>();

            if (!File.Exists(m_workPath + NetMiner.Constant.g_PlanFile))
                NewIndexFile();

            base.LoadXML(m_workPath + NetMiner.Constant.g_PlanFile);
        }

        ~oPlans()
        {
            base.Dispose();
        }
        #endregion

        public List<ePlan> Plans
        {
            get { return m_Plans; }
            set { m_Plans = value; }
        }

        #region 
        public void NewIndexFile()
        {
            XElement xe = new XElement("Plans");
            base.NewXML(m_workPath + NetMiner.Constant.g_PlanFile, xe);

        }

        #endregion

        #region 方法

        //加载计划
        //只加载计划的摘要信息，即只会从文件中加载需要列表显示的
        //计划内容，不会完整的加载计划信息
        public IEnumerable<ePlan> LoadPlans()
        {
            IEnumerable<XElement> xes = base.GetAllElement("Plan");
            IEnumerable<ePlan> ePlans = xes.Select<XElement, ePlan>(
                    s => Convert(s));
            return ePlans;
        }

        private ePlan Convert(XElement s)
        { 

            ePlan p;

          
            p = new ePlan();
            p.PlanID =long.Parse (s.Element("ID").Value.ToString());
            p.PlanName = s.Element("PlanName").Value.ToString();
            p.PlanState =(cGlobalParas.PlanState) int.Parse (s.Element("PlanState").Value.ToString());
            p.IsOverRun = s.Element("IsOverRun").Value.ToString() == "True" ? true : false;
            p.IsDisabled = s.Element("IsDisabled").Value.ToString() == "True" ? true : false;
            p.DisabledType =(cGlobalParas.PlanDisabledType) int.Parse (s.Element("DisabledType").Value.ToString());
            p.DisabledTime = int.Parse (s.Element("DisabledTime").Value.ToString());
            p.DisabledDateTime = DateTime.Parse (s.Element("DisabledDateTime").Value.ToString());
            p.RunTaskPlanType = int.Parse (s.Element("RunTaskPlanType").Value.ToString());
            p.EnabledDateTime = s.Element("EnabledDateTime").Value.ToString();
            p.RunOnesTime = s.Element("RunOnesTime").Value.ToString();
            p.RunDayTime = s.Element("RunDayTime").Value.ToString();
            p.RunAMTime = s.Element("RunAMTime").Value.ToString();
            p.RunPMTime = s.Element("RunPMTime").Value.ToString();
            p.RunWeeklyTime = s.Element("RunWeeklyTime").Value.ToString();
            p.RunWeekly = s.Element("RunWeekly").Value.ToString();
            p.RunTimeCount = s.Element("RunTimeCount").Value.ToString();
            p.FirstRunTime = s.Element("FirstRunTime").Value.ToString();
            p.RunInterval = s.Element("RunInterval").Value.ToString();
            p.IsTimeRange = s.Element("IsTimeRange").Value.ToString() == "True" ? true : false;
            p.StartTime = s.Element("StartTime").Value.ToString();
            p.EndTime = s.Element("EndTime").Value.ToString();

            IEnumerable<XElement> tasks = s.Element("Tasks").Elements("Task");

            foreach(XElement xe in tasks)
            {
                eTaskPlan tp = new eTaskPlan();
                tp.RunTaskType = (cGlobalParas.RunTaskType)int.Parse(xe.Element("RunTaskType").Value.ToString());
                tp.RunTaskName = xe.Element("RunTaskName").Value.ToString();
                tp.RunTaskPara=xe.Element("RunTaskPara").Value.ToString();

                p.RunTasks.Add(tp);
            }

            return p;

        }

        //获取一个执行的计划
        public ePlan LoadSinglePlan(Int64 PlanID)
        {
            XElement xe = base.SearchElement("Plan", "ID", PlanID.ToString());
            return Convert(xe);
        }

        //public int GetPlanCount()
        //{
        //    return 0;
        //}

        //插入一个计划,计划不能重名
        public void InsertPlan(ePlan NewPlan)
        {
            string pXML = CreatePlanXml( NewPlan );
            XElement xe= XElement.Load(new StringReader(pXML));
            base.AddElement(base.xDoc.Root, xe);
            base.Save();
        }

        public void EditPlan(ePlan ePlan)
        {
            XElement xe= base.SearchElement("Plan", "ID", ePlan.PlanID.ToString());
            xe.Remove();
            InsertPlan(ePlan);

        }

        private string CreatePlanXml(ePlan NewPlan)
        {
            //构造xml文件
            string pXml = "<Plan>";

            pXml += "<ID>" + NewPlan.PlanID + "</ID>" +
                "<PlanName>" + NewPlan.PlanName + "</PlanName>" +
                "<PlanState>" + (int)NewPlan.PlanState + "</PlanState>" +
                "<PlanRemark>" + NewPlan.PlanRemark + "</PlanRemark>" +
                "<IsOverRun>" + NewPlan.IsOverRun + "</IsOverRun>" +
                "<IsDisabled>" + NewPlan.IsDisabled + "</IsDisabled>" +
                "<DisabledType>" + (int)NewPlan.DisabledType + "</DisabledType>" +
                "<DisabledTime>" + NewPlan.DisabledTime + "</DisabledTime>" +
                "<DisabledDateTime>" + NewPlan.DisabledDateTime + "</DisabledDateTime>" +
                "<RunTaskPlanType>" + NewPlan.RunTaskPlanType + "</RunTaskPlanType>" +
                "<EnabledDateTime>" + NewPlan.EnabledDateTime + "</EnabledDateTime>" +
                "<RunOnesTime>" + NewPlan.RunOnesTime + "</RunOnesTime>" +
                "<RunDayTime>" + NewPlan.RunDayTime + "</RunDayTime>" +
                "<RunAMTime>" + NewPlan.RunAMTime + "</RunAMTime>" +
                "<RunPMTime>" + NewPlan.RunPMTime + "</RunPMTime>" +
                "<RunWeeklyTime>" + NewPlan.RunWeeklyTime + "</RunWeeklyTime>" +
                "<RunWeekly>" + NewPlan.RunWeekly + "</RunWeekly>" +
                "<FirstRunTime>" + NewPlan.FirstRunTime + "</FirstRunTime>" +
                "<RunInterval>" + NewPlan .RunInterval + "</RunInterval>" +
                "<IsTimeRange>" + NewPlan.IsTimeRange + "</IsTimeRange>" +
                "<StartTime>" + NewPlan.StartTime + "</StartTime>" +
                "<EndTime>" + NewPlan.EndTime + "</EndTime>" +

                //任务运行次数，只要任务进行修改，表示就是一个新任务，则此值修改为零
                "<RunTimeCount>0</RunTimeCount>" +                            
                "<Tasks>";

            for (int i = 0; i < NewPlan.RunTasks.Count; i++)
            {
                pXml += "<Task>";
                pXml += "<RunTaskType>" + (int)NewPlan.RunTasks[i].RunTaskType + "</RunTaskType>";
                pXml += "<RunTaskName>" + NewPlan.RunTasks[i].RunTaskName + "</RunTaskName>";
                pXml += "<RunTaskPara>" + NewPlan.RunTasks[i].RunTaskPara + "</RunTaskPara>";
                pXml += "</Task>";
            }

            pXml += "</Tasks>";

            pXml += "</Plan>";
            return pXml;
        }

        public void DelPlan(string PlanID)
        {
            //判断计划是否重名，如果重名则需要进行名称修改
            XElement xe = base.SearchElement("Plan", "ID", PlanID.ToString());
            //xe.Remove();
            base.RemoveElement(xe);
            base.Save();

        }

        //判断指定的计划是否过期，如果过期则修改计划状态
        public void IfEnabled(Int64 PlanID,bool OnlyState)
        {
            ePlan p = LoadSinglePlan(PlanID);

            if (p.IsDisabled == false )
            {
                p=null;
                return;
            }

            if (p.DisabledType == cGlobalParas.PlanDisabledType.RunTime)
            {
                //按运行次数失效
                if (p.DisabledTime < int.Parse(p.RunTimeCount) + 1)
                {
                    //表示失效
                    ModifyPlanState(PlanID, cGlobalParas.PlanState.Disabled);
                }
                else if (OnlyState ==false )
                {
                    //表示没有失效，修改运行次数递增1
                    ModifyPlanRunTime(PlanID, int.Parse(p.RunTimeCount) + 1);

                }
            }
            else if (p.DisabledType == cGlobalParas.PlanDisabledType.RunDateTime)
            {
                //按日期失效
                if (DateTime.Compare(DateTime.Now, p.DisabledDateTime) < 0)
                {
                    //修改此任务为失效
                    ModifyPlanState(PlanID, cGlobalParas.PlanState.Disabled);
                }
            }
        }

        private void ModifyPlanState(Int64 PlanID, cGlobalParas.PlanState pState)
        {
            XElement xe = base.SearchElement("Plan", "ID", PlanID.ToString());
            xe.Element("PlanState").Value = ((int)pState).ToString ();
            base.Save();

        }

        private void ModifyPlanRunTime(Int64 PlanID, int RunTime)
        {
           
            XElement xe = base.SearchElement("Plan", "ID", PlanID.ToString());
            xe.Element("RunTimeCount").Value = RunTime.ToString();
            base.Save();


        }

        //修改制定计划的名称
        public bool RenamePlanName(string OldPlanName, string NewPlanName)
        {
            //判断计划文件是否存在，如果不存在则新建

            XElement xe = base.SearchElement("Plan", "PlanName", OldPlanName);
            xe.Element("PlanName").Value = NewPlanName;
            base.Save();

            return true;
        }

        #endregion
    }
}

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

///���ܣ��ƻ���
///���ʱ�䣺2009-8-21
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵�������мƻ����񶼴洢��һ���ļ��У�ϵͳ��·��\tasks\plan ����������Ŀ����Ϊ�˿��Ը��õ�
///�����ƻ���ִ�������ϵͳ������ʱ�����Զ����ش��ļ������м�����
///
///�汾��01.10.00
///�޶�����
namespace NetMiner.Core.Plan
{
    public class oPlans:XmlUnity
    {
        //cXmlIO xmlPlan;
        List<ePlan> m_Plans;
        private string m_workPath = string.Empty;

        #region ���������
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

        #region ����

        //���ؼƻ�
        //ֻ���ؼƻ���ժҪ��Ϣ����ֻ����ļ��м�����Ҫ�б���ʾ��
        //�ƻ����ݣ����������ļ��ؼƻ���Ϣ
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

        //��ȡһ��ִ�еļƻ�
        public ePlan LoadSinglePlan(Int64 PlanID)
        {
            XElement xe = base.SearchElement("Plan", "ID", PlanID.ToString());
            return Convert(xe);
        }

        //public int GetPlanCount()
        //{
        //    return 0;
        //}

        //����һ���ƻ�,�ƻ���������
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
            //����xml�ļ�
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

                //�������д�����ֻҪ��������޸ģ���ʾ����һ�����������ֵ�޸�Ϊ��
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
            //�жϼƻ��Ƿ������������������Ҫ���������޸�
            XElement xe = base.SearchElement("Plan", "ID", PlanID.ToString());
            //xe.Remove();
            base.RemoveElement(xe);
            base.Save();

        }

        //�ж�ָ���ļƻ��Ƿ���ڣ�����������޸ļƻ�״̬
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
                //�����д���ʧЧ
                if (p.DisabledTime < int.Parse(p.RunTimeCount) + 1)
                {
                    //��ʾʧЧ
                    ModifyPlanState(PlanID, cGlobalParas.PlanState.Disabled);
                }
                else if (OnlyState ==false )
                {
                    //��ʾû��ʧЧ���޸����д�������1
                    ModifyPlanRunTime(PlanID, int.Parse(p.RunTimeCount) + 1);

                }
            }
            else if (p.DisabledType == cGlobalParas.PlanDisabledType.RunDateTime)
            {
                //������ʧЧ
                if (DateTime.Compare(DateTime.Now, p.DisabledDateTime) < 0)
                {
                    //�޸Ĵ�����ΪʧЧ
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

        //�޸��ƶ��ƻ�������
        public bool RenamePlanName(string OldPlanName, string NewPlanName)
        {
            //�жϼƻ��ļ��Ƿ���ڣ�������������½�

            XElement xe = base.SearchElement("Plan", "PlanName", OldPlanName);
            xe.Element("PlanName").Value = NewPlanName;
            base.Save();

            return true;
        }

        #endregion
    }
}

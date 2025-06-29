using System;
using System.Collections.Generic;
using System.Data;
using NetMiner.Resource;
using NetMiner.Base;
using NetMiner.Core.gTask.Entity;
using System.Xml.Linq;
using System.Linq;
using System.Text;

///功能：采集任务完成索引文件管理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace NetMiner.Core.gTask
{
    public class oTaskComplete:XmlUnity
    {

        //cXmlIO xmlConfig;
        private string m_workPath = string.Empty;
        private List<eTaskCompleted> m_TaskCompletedList;

        public oTaskComplete(string workPath)
        {
            m_workPath = workPath;

            base.LoadXML(workPath + NetMiner.Constant.g_CompletedFile);

        }

        ~oTaskComplete()
        {
            base.Dispose();
        }

        public List<eTaskCompleted> TaskCompletedList
        {
            get { return m_TaskCompletedList; }
            set { m_TaskCompletedList = value; }
        }



        //返回的是已经完成的任务的集合信息
        public IEnumerable<eTaskCompleted> LoadTaskData()
        {

            IEnumerable<XElement> xes = base.GetAllElement("Task", "StartDate DESc");
            IEnumerable<eTaskCompleted> eTasks = xes.Select<XElement, eTaskCompleted>(
                    s => Convert(s));
                   

            return eTasks;
        }

        //根据完成任务ID加载任务信息
        public eTaskCompleted LoadSingleTask(Int64  TaskID)
        {

            XElement xe= base.SearchElement("Task", "TaskID", TaskID.ToString ());

            return Convert(xe);
        }

        private eTaskCompleted Convert(XElement s)
        {
            eTaskCompleted ec = new eTaskCompleted();
            ec.TaskID = long.Parse(s.Element("TaskID").Value.ToString());
            ec.TaskName = s.Element("TaskName").Value.ToString();
            ec.TaskClass = s.Element("TaskClass").Value.ToString();
            ec.GatherResult = (cGlobalParas.GatherResult)int.Parse(s.Element("GatherResult").Value.ToString());
            ec.TaskType = (cGlobalParas.TaskType)int.Parse(s.Element("TaskType").Value.ToString());
            ec.TaskRunType = (cGlobalParas.TaskRunType)int.Parse(s.Element("RunType").Value.ToString());
            ec.ExportFile = s.Element("ExportFile").Value.ToString();
            ec.TempFile = s.Element("tempFile").Value.ToString();
            ec.UrlCount = int.Parse(s.Element("UrlCount").Value.ToString());
            ec.PublishType = (cGlobalParas.PublishType)int.Parse(s.Element("PublishType").Value.ToString());
            ec.GatheredUrlCount = int.Parse(s.Element("GatheredUrlCount").Value.ToString());
            ec.StartDate= DateTime.Parse(s.Element("StartDate").Value.ToString());
            ec.CompleteDate = DateTime.Parse(s.Element("CompleterDate").Value.ToString());
            ec.RowsCount= int.Parse(s.Element("RowCount").Value.ToString());
            return ec;
        }

        public void Clear()
        {
            base.xDoc.Root.Elements("Tasks").Remove();
            base.Save();
        }


        //计算当前共有多少个任务已经完成
        public int GetTaskCount()
        {
            return base.GetNodesCount("Task");
        }

        //public bool GetIsLogin(int index)
        //{
        //    bool Isl = (Tasks[index].Row["IsLogin"].ToString() == "True" ? true : false);
        //    return Isl;
        //}

        //共有多少个已经完成的任务
        //public int GetCount()
        //{
        //    int RunCount;

        //    try
        //    {
        //        RunCount = Tasks.Count;
        //    }
        //    catch
        //    {
        //        RunCount = 0;
        //    }
        //    return RunCount;
        //}

        public void NewTaskCompleteFile()
        {
            XElement xe = new XElement("Tasks");
            base.NewXML(m_workPath + NetMiner.Constant.g_CompletedFile, xe);
            base.Save();
        }

        /// <summary>
        /// 采集任务完成后，插入到完成队列中
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="tSate"></param>
        /// <returns></returns>
        public void InsertTaskComplete(eTaskCompleted ec,cGlobalParas.GatherResult gResult)
        {

            XElement xe = new XElement("Task");
            xe.Add(new XElement("TaskID", ec.TaskID));
            xe.Add(new XElement("TaskName", ec.TaskName));
            xe.Add(new XElement("TaskClass", ec.TaskClass));
            xe.Add(new XElement("GatherResult", (int)gResult));
            xe.Add(new XElement("TaskType", (int)ec.TaskType));
            xe.Add(new XElement("RunType", (int)ec.TaskRunType));
            xe.Add(new XElement("ExportFile", ec.ExportFile));
            xe.Add(new XElement("tempFile", ec.TempFile));
            xe.Add(new XElement("UrlCount", ec.UrlCount));
            xe.Add(new XElement("GatheredUrlCount", ec.GatheredUrlCount));
            xe.Add(new XElement("PublishType", (int)ec.PublishType));
            xe.Add(new XElement("StartDate", ec.StartDate));
            xe.Add(new XElement("CompleterDate", ec.CompleteDate));
            xe.Add(new XElement("RowCount", ec.RowsCount));
            //base.xDoc.Root.Element("Task").Add(xe);
            base.AddElement(base.xDoc.Root, xe);
            base.Save();

        }

        public void DelTask(Int64  TaskID)
        {
            XElement xe = base.SearchElement("Task", "TaskID", TaskID.ToString());

            base.RemoveElement(xe);
            base.Save();
        }
    }
}

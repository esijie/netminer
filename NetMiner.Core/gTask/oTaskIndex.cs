using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using NetMiner.Resource;
using NetMiner.Core.gTask.Entity;
using NetMiner.Base;
using System.Xml.Linq;
using System.Linq;

///功能：任务索引文件管理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace NetMiner.Core.gTask
{

    public class oTaskIndex:XmlUnity
    {
        private string m_workPath = string.Empty;

        #region 构造和析构
        public oTaskIndex(string workPath)
        {
            m_workPath = workPath;
        }

        public oTaskIndex(string workPath,string xmlFile)
        {
            m_workPath = workPath;
            base.LoadXML(xmlFile);
        }

        ~oTaskIndex()
        {
            base.Dispose();
        }

        #endregion

        #region 新建 新建一个index文件,及在此文件下新建一个任务信息
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path">完整路径</param>
        public void NewIndexFile(string Path)
        {
            XElement xe = new XElement("TaskIndex");
            base.NewXML(Path + "\\index.xml", xe);
            base.Save();
        }

        public int InsertTaskIndex(eTaskIndex eIndex)
        {
            int MaxID= base.GetMaxID(base.xDoc.Root, "Task");

            MaxID = MaxID + 1;

            XElement xe = new XElement("Task");
            xe.Add(new XElement("ID", MaxID));
            xe.Add(new XElement("Name", eIndex.TaskName));
            xe.Add(new XElement("Type", (int)eIndex.TaskType));
            xe.Add(new XElement("RunType", (int)eIndex.TaskRunType));
            xe.Add(new XElement("ExportFile", eIndex.ExportFile));
            xe.Add(new XElement("WebLinkCount", eIndex.WebLinkCount));
            xe.Add(new XElement("PublishType", (int)eIndex.PublishType));

            base.AddElement(xDoc.Root,xe);
            base.Save();

            return MaxID;

        }

        public eTaskIndex DeleTaskIndex(string TaskName)
        {
            XElement xe=base.SearchElement("Task", "Name", TaskName);
            eTaskIndex ei = Convert(xe);
            base.RemoveElement(xe);
            base.Save();
            return ei;
        }
        #endregion

        #region 根据指定的任务分类,查找index.xml文件,并根据指定的index返回任务信息

        public void LoadIndexDocument(string ClassName)
        {
            string ClassPath = string.Empty;
            if (string.IsNullOrEmpty(ClassName))
            {
                ClassPath = m_workPath + "Tasks";
            }
            else if (ClassName == NetMiner.Constant.g_RemoteTaskClass)
            {
                ClassPath = m_workPath + NetMiner.Constant.g_RemoteTaskPath;
            }
            else
            {
                oTaskClass tClass = new oTaskClass(this.m_workPath);
                ClassPath = this.m_workPath + tClass.GetTaskClassPathByName(ClassName);
                tClass = null;
            }

            m_TaskPath = ClassPath;

            string indexFile = ClassPath + "\\index.xml";

            base.RefreshXML(indexFile);

        }

     

        /// <summary>
        /// 获取指定任务分类下的任务索引信息
        /// </summary>
        /// <param name="ClassName">采集任务分类的全路径</param>
        public IEnumerable<eTaskIndex> GetTaskDataByClass(string ClassName="")
        {
            this.LoadIndexDocument(ClassName);

            IEnumerable<XElement> xes = base.GetAllElement("Task");
            IEnumerable<eTaskIndex> eTasks = xes.Select<XElement, eTaskIndex>(
                    s=>Convert(s)
                );

            return eTasks;

        }

        public eTaskIndex GetTaskIndex(string TaskName)
        {
            XElement xe = base.SearchElement("Task", "Name", TaskName);
            return Convert(xe);
        }

        private eTaskIndex Convert(XElement s)
        {
            

            eTaskIndex ei = new eTaskIndex();
            if (s.ToString().Contains("<id>"))
            {
                //兼容以前版本所需要的处理，此段代码在5.5以后的升级中可以去掉，5.5版本后将全部统一
                ei.ID= int.Parse(s.Element("id").Value.ToString());
            }
            else
                ei.ID = int.Parse(s.Element("ID").Value.ToString());
            ei.TaskName = s.Element("Name").Value.ToString();
            ei.TaskType = (cGlobalParas.TaskType)int.Parse(s.Element("Type").Value.ToString());
            ei.TaskRunType = (cGlobalParas.TaskRunType)int.Parse(s.Element("RunType").Value.ToString());
            ei.ExportFile = s.Element("ExportFile").Value.ToString();
            ei.WebLinkCount = int.Parse(s.Element("WebLinkCount").Value.ToString());
            ei.PublishType = (cGlobalParas.PublishType)int.Parse(s.Element("PublishType").Value.ToString());
            return ei;
        }


        public void EditIndexTaskName(string oldTaskName,string NewTaskName)
        {
            XElement xe = base.SearchElement("Task", "Name", oldTaskName);
            base.EditValue(xe.Element("Name"), NewTaskName);
            base.Save();
        }

        public void EditIndexTask(string TaskName,eTaskIndex eIndex)
        {
            XElement xe = base.SearchElement("Task", "Name", TaskName);

            base.EditValue(xe.Element("Type"), ((int)eIndex.TaskType).ToString()) ;
            base.EditValue(xe.Element("RunType"), ((int)eIndex.TaskRunType).ToString());
            base.EditValue(xe.Element("ExportFile"), eIndex.ExportFile);
            base.EditValue(xe.Element("WebLinkCount"), eIndex.WebLinkCount.ToString ());
            base.EditValue(xe.Element("PublishType"), ((int)eIndex.PublishType).ToString());
            base.Save();
        }

        public bool isExistTask(string TaskName)
        {
            XElement xe= base.SearchElement("TaskIndex", "Name", TaskName);
            if (xe == null)
                return false;
            else
                return true;
        }

        public IEnumerable<eTaskIndex> GetTaskDataByClass(int ClassID)
        {
            oTaskClass tClass = new oTaskClass(this.m_workPath);
            //string ClassPath = tClass.GetTaskClassPathByID(ClassID);
            string ClassName=tClass.GetTaskClassNameByID(ClassID.ToString());
            //m_TaskPath = ClassPath;
            tClass = null;

            return GetTaskDataByClass(ClassName);

        }

        private string m_TaskPath;
        private string TaskPath
        {
            get { return m_TaskPath; }
        }

        /// <summary>
        /// 返回当前分类下的任务数量
        /// </summary>
        /// <returns></returns>
        public int GetTaskCount()
        {
            return base.GetNodesCount("Task");
        }


        #endregion



    }
}

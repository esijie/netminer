using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using Soukey.Resource;
using Soukey.Common;

///功能：任务索引文件管理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace Soukey.Gather.Task
{

    //TaskIndex是用于索引任务摘要信息的类，主要用于对页面的摘要内容进行显示
    //这样做的目的是可以快速索引任务，同时如果用户误删除了任务，也可对此进行提醒
    //每一个任务分类都存储在一个路径下，在此路径下默认建立一个TaskIndex.xml文件，记录任务索引
    //这个类设计有问题,应该是包含一个任务的集合类,将新建任务索引的功能拆分出来,
    //但现在就这样做了,下一版进行修正.
    public class cTaskIndex
    {
        cXmlIO xmlConfig;
        DataView Tasks;
        private string m_workPath = string.Empty;

        #region 构造和析构
        public cTaskIndex(string workPath)
        {
            m_workPath = workPath;
            xmlConfig = new cXmlIO();
        }

        public cTaskIndex (string workPath,string xmlFile)
        {
            m_workPath = workPath;
            xmlConfig =new cXmlIO (xmlFile );
        }

        ~cTaskIndex ()
        {
            xmlConfig =null;
        }

        #endregion

        #region 新建 新建一个index文件,及在此文件下新建一个任务信息
        public void NewIndexFile(string Path)
        {

            string  strXml="<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                       "<TaskIndex>" +
                       "</TaskIndex>";
            xmlConfig.NewXmlFile(Path + "\\index.xml",strXml );
            
        }

        public void InsertTaskIndex(string strXml)
        {
            xmlConfig.InsertElement("TaskIndex", "Task", strXml);
            xmlConfig.Save();
        }

        public void DeleTaskIndex(string TaskName)
        {
            xmlConfig.DeleteChildNodes("TaskIndex","Name",TaskName);
            xmlConfig.Save();
        }


        #endregion

        #region 根据指定的任务分类,查找index.xml文件,并根据指定的index返回任务信息


        //获取任务分类根目录下的任务索引，此目录为应用程序路径\\tasks，
        //此分类为固定内容，不提供用户可操作的任何处理。
        public void GetTaskDataByClass()
        {

            string ClassPath = m_workPath + "tasks";
            m_TaskPath = ClassPath;

            xmlConfig = new cXmlIO(ClassPath + "\\index.xml");

            //获取TaskClass节点
            Tasks = xmlConfig.GetData("TaskIndex");
        }

        /// <summary>
        /// 特指获取特殊分类“远程任务”的信息
        /// </summary>
        public void GetTaskDataByRemoteClass()
        {
            string ClassPath = m_workPath + cTool.g_RemoteTaskPath;
            m_TaskPath = ClassPath;

            if (!File.Exists(ClassPath + "\\index.xml"))
            {
                NewIndexFile(ClassPath);
            }

            xmlConfig = new cXmlIO(ClassPath + "\\index.xml");

            //获取TaskClass节点
            Tasks = xmlConfig.GetData("TaskIndex");
        }

        public void GetTaskDataByClass(int ClassID)
        {
            Task.cTaskClass tClass = new Task.cTaskClass(this.m_workPath);

            string ClassPath = tClass.GetTaskClassPathByID(ClassID);
            m_TaskPath = ClassPath;

            tClass = null;

            try
            {
                xmlConfig = new cXmlIO(ClassPath + "\\index.xml");
            }
            catch 
            {
                return ;
            }

            //获取TaskClass节点
            Tasks = xmlConfig.GetData("TaskIndex");


        }

        private string m_TaskPath;
        private string TaskPath
        {
            get { return m_TaskPath; }
        }

        public void GetTaskDataByClass(string ClassName)
        {
            string ClassPath ;

            if (ClassName == "")
            {
                ClassPath = m_workPath + "Tasks";
            }
            else if (ClassName == cTool.g_RemoteTaskClass)
            {
                ClassPath = m_workPath + cTool.g_RemoteTaskPath;
            }
            else
            {
                Task.cTaskClass tClass = new Task.cTaskClass(this.m_workPath);

                ClassPath = tClass.GetTaskClassPathByName( ClassName);

                tClass = null;
            }

            m_TaskPath = ClassPath;

            xmlConfig = new cXmlIO(ClassPath + "\\index.xml");

            //获取TaskClass节点
            Tasks = xmlConfig.GetData("TaskIndex");


        }

        /// <summary>
        /// 返回当前分类下的任务数量
        /// </summary>
        /// <returns></returns>
        public int GetTaskClassCount()
        {
            int tCount;
            if (Tasks == null)
            {
                tCount = 0;
            }
            else
            {
                tCount = Tasks.Count;
            }
            return tCount;
        }

        public int GetTaskID(int index)
        {
            int tid = int.Parse (Tasks[index].Row["id"].ToString());
            return tid;
        }

        public string GetTaskName(int index)
        {
            string TName = Tasks[index].Row["Name"].ToString();
            return TName;
        }

        public cGlobalParas.TaskType GetTaskType(int index)
        {
            string TType = Tasks[index].Row["Type"].ToString();
            return (cGlobalParas.TaskType)int.Parse (TType);
        }

        public cGlobalParas.TaskRunType GetTaskRunType(int index)
        {
            string TRunType = Tasks[index].Row["RunType"].ToString();
            return (cGlobalParas.TaskRunType)int.Parse (TRunType);
        }

        public string GetExportFile(int index)
        {
            string ExportFile = Tasks[index].Row["ExportFile"].ToString();
            return ExportFile;
        }

        public cGlobalParas.TaskState GetTaskState(int index)
        {
            string fName = TaskPath + "\\" + GetTaskName(index) + ".smt";
            if (File.Exists(fName))
            {
                return cGlobalParas.TaskState.UnStart;
            }
            else
            {
                return cGlobalParas.TaskState.Failed;
            }
        }

        public int  GetWebLinkCount(int index)
        {
            int WebLinkCount;
            try
            {
                WebLinkCount = int.Parse(Tasks[index].Row["WebLinkCount"].ToString());
            }
            catch
            {
                WebLinkCount = 0;
            }
            return WebLinkCount;
        }

        public cGlobalParas.PublishType GetPublishType(int index)
        {
            cGlobalParas.PublishType pType = (cGlobalParas.PublishType)int.Parse(Tasks[index].Row["PublishType"].ToString());
            return pType;
        }

        #endregion



    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using Soukey.Resource;
using Soukey.Common;

///���ܣ����������ļ�����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace Soukey.Gather.Task
{

    //TaskIndex��������������ժҪ��Ϣ���࣬��Ҫ���ڶ�ҳ���ժҪ���ݽ�����ʾ
    //��������Ŀ���ǿ��Կ�����������ͬʱ����û���ɾ��������Ҳ�ɶԴ˽�������
    //ÿһ��������඼�洢��һ��·���£��ڴ�·����Ĭ�Ͻ���һ��TaskIndex.xml�ļ�����¼��������
    //��������������,Ӧ���ǰ���һ������ļ�����,���½����������Ĺ��ܲ�ֳ���,
    //�����ھ���������,��һ���������.
    public class cTaskIndex
    {
        cXmlIO xmlConfig;
        DataView Tasks;
        private string m_workPath = string.Empty;

        #region ���������
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

        #region �½� �½�һ��index�ļ�,���ڴ��ļ����½�һ��������Ϣ
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

        #region ����ָ�����������,����index.xml�ļ�,������ָ����index����������Ϣ


        //��ȡ��������Ŀ¼�µ�������������Ŀ¼ΪӦ�ó���·��\\tasks��
        //�˷���Ϊ�̶����ݣ����ṩ�û��ɲ������κδ���
        public void GetTaskDataByClass()
        {

            string ClassPath = m_workPath + "tasks";
            m_TaskPath = ClassPath;

            xmlConfig = new cXmlIO(ClassPath + "\\index.xml");

            //��ȡTaskClass�ڵ�
            Tasks = xmlConfig.GetData("TaskIndex");
        }

        /// <summary>
        /// ��ָ��ȡ������ࡰԶ�����񡱵���Ϣ
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

            //��ȡTaskClass�ڵ�
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

            //��ȡTaskClass�ڵ�
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

            //��ȡTaskClass�ڵ�
            Tasks = xmlConfig.GetData("TaskIndex");


        }

        /// <summary>
        /// ���ص�ǰ�����µ���������
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using NetMiner.Gather;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Base;

///���ܣ��ɼ�������������ļ�����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyControl.Task
{
    class oTaskComplete
    {

        cXmlIO xmlConfig;
        DataView Tasks;
        private string m_workPath = string.Empty;

        public oTaskComplete(string workPath)
        {
            m_workPath = workPath;
        }

        ~oTaskComplete()
        {
        }

      
        //���ص����Ѿ���ɵ�����ļ�����Ϣ
        public void LoadTaskData()
        {
            string fileName = m_workPath + "Data\\index.xml";
            if (!File.Exists(fileName))
                NewTaskCompleteFile();

            try
            {
                xmlConfig = new cXmlIO(m_workPath  + "Data\\index.xml");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //��ȡTaskClass�ڵ�
            Tasks = xmlConfig.GetData("Tasks");
        }

        private void NewTaskCompleteFile()
        {
            xmlConfig = new cXmlIO();
            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                      "<Tasks>" +
                      "</Tasks>";
            xmlConfig.NewXmlFile(m_workPath + "data\\index.xml", strXml);
        }

        //���ü���������Ϣ��,���ô˷�������һ�������dataview
        public DataView GetTasks()
        {
            return Tasks;
        }

        //�����������ID����������Ϣ
        public void LoadSingleTask(Int64  TaskID)
        {
            try
            {
                xmlConfig = new cXmlIO(m_workPath + "Data\\index.xml");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //��ȡTaskClass�ڵ�,���ܷ��ص�Ҳ��DataView,������ֻ����һ����¼
            //��������Ϊ�˸��õļ��ݷ��ʲ���
            Tasks = xmlConfig.GetData("Tasks","TaskID",TaskID.ToString () );
        }


        //���㵱ǰ���ж��ٸ������Ѿ����
        public int GetTaskCount()
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

        public Int64 GetTaskID(int index)
        {

            Int64 tid = 0;
            try
            {
                tid = Int64.Parse(Tasks[index].Row["TaskID"].ToString());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return tid;
        }

        public string GetTaskName(int index)
        {
            string TName = Tasks[index].Row["TaskName"].ToString();
            return TName;
        }

        public cGlobalParas.TaskType GetTaskType(int index)
        {
            cGlobalParas.TaskType tType = (cGlobalParas.TaskType)int.Parse(Tasks[index].Row["TaskType"].ToString());
            return tType;
        }

        public cGlobalParas.TaskRunType GetTaskRunType(int index)
        {
            cGlobalParas.TaskRunType TRunType =(cGlobalParas.TaskRunType)int.Parse (Tasks[index].Row["RunType"].ToString());
            return TRunType;
        }

        public string GetTempFile(int index)
        {
            string tempData = Tasks[index].Row["tempFile"].ToString();
            return tempData;
        }

        public string GetExportFile(int index)
        {
            string ExportFile = Tasks[index].Row["ExportFile"].ToString();
            return ExportFile;
        }

        public bool GetIsLogin(int index)
        {
            bool Isl = (Tasks[index].Row["IsLogin"].ToString() == "True" ? true : false);
            return Isl;
        }

        public cGlobalParas.PublishType GetPublishType(int index)
        {
            cGlobalParas.PublishType pType = (cGlobalParas.PublishType)int.Parse(Tasks[index].Row["PublishType"].ToString());
            return pType;
        }

        //���ش�������Ҫ�ɼ�����ҳ��ַ����
        public int GetUrlCount(int index)
        {
            int WebLinkCount;
            try
            {
                WebLinkCount = int.Parse(Tasks[index].Row["UrlCount"].ToString());
            }
            catch (System.Exception ex)
            {
                if (Tasks[index].Row["UrlCount"].ToString() == "")
                {
                    WebLinkCount = 0;
                }
                else
                {
                    throw ex;
                }
            }
            return WebLinkCount;
        }

        //���ش������Ѿ��ɼ���ҳ�ĵ�ַ����
        public int GetGatheredUrlCount(int index)
        {
            int WebLinkCount;
            try
            {
                WebLinkCount = int.Parse(Tasks[index].Row["GatheredUrlCount"].ToString());
            }
            catch
            {
                WebLinkCount = 0;
            }
            return WebLinkCount;
        }
        
        //���ж��ٸ��Ѿ���ɵ�����
        public int GetCount()
        {
            int RunCount;

            try
            {
                RunCount = Tasks.Count;
            }
            catch
            {
                RunCount = 0;
            }
            return RunCount;
        }
     
    }
}

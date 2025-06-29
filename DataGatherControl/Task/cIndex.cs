using System;
using System.Collections.Generic;
using System.Text;
using DataGatherControl.xml;
using System.Data ;
using System.IO;

namespace DataGatherControl.Task
{
    public class cIndex
    {
        cXmlIO xmlConfig;
        DataView Tasks;
        private string m_fName;

        #region
        public cIndex()
        {
            m_fName = cTool.getPrjPath() + "TaskIndex.xml";

            if (!File.Exists(m_fName))
            {
                xmlConfig = new cXmlIO();
                NewIndexFile();
            }
            else
            {
                xmlConfig = new cXmlIO(m_fName);
            }
       
        }
     
        ~cIndex()
        {
            xmlConfig =null;
        }
        #endregion

        #region 新建 新建一个index文件,及在此文件下新建一个任务信息
        private void NewIndexFile()
        {

            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                       "<TaskIndex>" +
                       "</TaskIndex>";
            xmlConfig.NewXmlFile(cTool.getPrjPath () + "TaskIndex.xml", strXml);

        }

        public void InsertTaskIndex(string strXml)
        {
            xmlConfig.InsertElement("TaskIndex", "Task", strXml);
            xmlConfig.Save();
        }

        public void DeleTaskIndex(string TaskName)
        {
            xmlConfig.DeleteChildNodes("TaskIndex", "Name", TaskName);
            xmlConfig.Save();
        }

       


        #endregion

        #region 根据指定的任务分类,查找index.xml文件,并根据指定的index返回任务信息


        public void GetTaskData()
        {
            //获取TaskClass节点
            Tasks = xmlConfig.GetData("TaskIndex");
        }

        //计算当前共有多少个TaskClass
        public int TaskCount
        {
            get
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
        }

        public string GetID(int index)
        {
            string tID = Tasks[index].Row["ID"].ToString();
            return tID;
        }

        public string GetTName(int index)
        {
            string TName = Tasks[index].Row["Name"].ToString();
            return TName;
        }

        public string GetWebSite(int index)
        {
            string sWebSite = Tasks[index].Row["WebSite"].ToString();
            return sWebSite;
        }

        public bool GetIsUpload(int index)
        {
            string isUpload = Tasks[index].Row["IsUpload"].ToString();
            if (isUpload == "True")
                return true;
            else
                return false;
        }

        #endregion

        public void EditIsUpdate(int index)
        {
            xmlConfig.EditNodeValue("Task", "Name", index.ToString(), "IsUpdate", "True");
            xmlConfig.Save();
        }
    }
}

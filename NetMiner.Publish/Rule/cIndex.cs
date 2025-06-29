using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Common;
using System.Data ;
using NetMiner.Resource;
using NetMiner.Base;

namespace NetMiner.Publish.Rule
{
    public class cIndex
    {
        cXmlIO xmlConfig;
        DataView Templates;
        private string m_workPath = string.Empty;

        #region 构造和析构
        public cIndex(string workPath)
        {
            m_workPath = workPath;
            xmlConfig = new cXmlIO();
        }

        public cIndex (string workPath,string xmlFile)
        {
            m_workPath = workPath;
            xmlConfig =new cXmlIO (xmlFile );
        }

        ~cIndex()
        {
            xmlConfig =null;
        }

        #endregion

        #region 新建 新建一个index文件,及在此文件下新建一个任务信息
        public void NewIndexFile(string Path)
        {

            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                       "<PublishTemplates>" +
                       "</PublishTemplates>";
            xmlConfig.NewXmlFile(Path + "\\index.xml", strXml);

        }

        public void InsertTemplateIndex(string strXml)
        {
            xmlConfig.InsertElement("PublishTemplates", "Template", strXml);
            xmlConfig.Save();
        }

        public void DeleTemplateIndex(string TaskName)
        {
            xmlConfig.DeleteChildNodes("PublishTemplates", "Name", TaskName);
            xmlConfig.Save();
        }

        #endregion

        #region 根据指定的任务分类,查找index.xml文件,并根据指定的index返回任务信息


        //获取任务分类根目录下的任务索引，此目录为应用程序路径\\tasks，
        //此分类为固定内容，不提供用户可操作的任何处理。

        public void GetData()
        {

            string ClassPath = m_workPath + "publish";

            xmlConfig = new cXmlIO(ClassPath + "\\index.xml");

            //获取TaskClass节点
            Templates = xmlConfig.GetData("PublishTemplates");
        }

        //计算当前共有多少个TaskClass
        public int GetCount()
        {
            int tCount;
            if (Templates == null)
            {
                tCount = 0;
            }
            else
            {
                tCount = Templates.Count;
            }
            return tCount;
        }

        public string GetTName(int index)
        {
            string tName = Templates[index].Row["Name"].ToString();
            return tName;
        }

        public cGlobalParas.PublishTemplateType GetTType(int index)
        {
            string TaskType = Templates[index].Row["Type"].ToString();
            
            return (cGlobalParas.PublishTemplateType)int.Parse (TaskType);
        }

        public string GetTRemark(int index)
        {
            string TType = Templates[index].Row["Remark"].ToString();
            return TType;
        }

        public  void EditName(string oldName,string Name, string remark)
        {
            xmlConfig.EditNodeValue("PublishTemplates", "Name", oldName, "Name", Name);
            xmlConfig.EditNodeValue("PublishTemplates", "Name", oldName, "Remark", remark);
            xmlConfig.Save();
        }

        #endregion
    }
}

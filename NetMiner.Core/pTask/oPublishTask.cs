using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NetMiner.Common;
using NetMiner.Resource;
using NetMiner.Core.gTask.Entity;
using NetMiner.Base;
using NetMiner.Core.pTask.Entity;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.IO;

/// <summary>
/// 从5.4.1开始，增加了isEnginePublish属性，用于识别分布式采集引擎的发布数据规则
/// 从后台发布数据，提升数据插入的效率
/// </summary>
namespace NetMiner.Core.pTask
{
    public class oPublishTask:XmlUnity
    {

        cXmlIO xmlConfig;
        DataView m_PublishInfos;
        private string m_workPath = string.Empty;

        public oPublishTask(string workPath)
        {
            m_workPath = workPath;
            string pFile = m_workPath + "publish\\PublishInfo.xml";
            if (!File.Exists(pFile))
                NewPublishFile();
            base.LoadXML(m_workPath + "publish\\PublishInfo.xml");
        }

        ~oPublishTask()
        {
            if (xmlConfig != null)
                xmlConfig = null;

            base.Dispose();
        }

        //新建一个数据源存储文件
        public void NewPublishFile()
        {
            XElement xe = new XElement("PublishInfos");
            base.NewXML(m_workPath + NetMiner.Constant.g_PublishInfoFile, xe);
        }

        //返回的是数据源的集合信息
        public IEnumerable<ePublishTask> LoadPublishData()
        {
            IEnumerable<XElement> xes = base.GetAllElement("Publish");
            IEnumerable<ePublishTask> eTasks = xes.Select<XElement, ePublishTask>(
                    s => Convert(s));


            return eTasks;
        }

        public ePublishTask LoadSingleTask(string pName)
        {

            XElement xe = base.SearchElement("Publish", "Name", pName);

            return Convert(xe);
        }
        private ePublishTask Convert(XElement s)
        {
            ePublishTask et = new ePublishTask();
            et.pName = s.Element("Name").Value.ToString();
            et.ThreadCount = int.Parse (s.Element("ThreadCount").Value.ToString());
            et.IsDelRepeatRow = s.Element("IsDelRepeatRow").Value.ToString()=="true"?true:false;
            et.PublishType = (cGlobalParas.PublishType) int.Parse (s.Element("PublishType").Value.ToString());
            et.DataType = (cGlobalParas.DatabaseType)int.Parse ( s.Element("DataType").Value.ToString());
            et.DataSource = s.Element("DataSource").Value.ToString();
            et.DataTable = s.Element("DataTable").Value.ToString();
            et.InsertSql = s.Element("InsertSql").Value.ToString();
            et.UrlCode = (cGlobalParas.WebCode)int.Parse ( s.Element("UrlCode").Value.ToString());
            et.PostUrl = s.Element("PostUrl").Value.ToString();
            et.Cookie = s.Element("Cookie").Value.ToString();
            et.SucceedFlag = s.Element("SucceedFlag").Value.ToString();
            et.PIntervalTime = int.Parse ( s.Element("PublishIntervalTime").Value.ToString());
            et.IsHeader = s.Element("IsHeader").Value.ToString()=="True"?true:false;
            et.Header = s.Element("Header").Value.ToString();
            if (s.Element("TemplateName")!=null)
                et.TemplateName = s.Element("TemplateName").Value.ToString();
            if (s.Element("User") !=null)
                et.User = s.Element("User").Value.ToString();
            if (s.Element("Password")!=null)
                et.Password = s.Element("Password").Value.ToString();
            if (s.Element("Domain") != null)
                et.Domain = s.Element("Domain").Value.ToString();
            if (s.Element("PublishDbConn") != null)
                et.TemplateDBConn = s.Element("PublishDbConn").Value.ToString();

            if (s.Element("Paras") != null)
            {
                IEnumerable<XElement> paras = s.Element("Paras").Elements();

                foreach (XElement xe in paras)
                {
                    ePublishData ep = new ePublishData();
                    ep.DataLabel = xe.Element("Label").Value.ToString();
                    ep.DataValue = xe.Element("Value").Value.ToString();
                    ep.DataType = (cGlobalParas.PublishParaType)int.Parse(xe.Element("Type").Value.ToString());
                    et.PublishParas.Add(ep);
                }
            }

            if (s.Element("IsSqlTrue") != null)
                et.IsSqlTrue = s.Element("IsSqlTrue").Value.ToString()=="True"?true:false;
            if (s.Element("isEnginPublish") != null)
                et.isEnginPublish = s.Element("isEnginPublish").Value.ToString()=="True"?true:false;

            return et;
        }
        


        public List<ePublishData> GetPublishParas(int index)
        {
            if (m_PublishInfos[index].DataView.Table.ChildRelations.Count>0)
            {
                List<ePublishData> paras = new List<ePublishData>();
                if (m_PublishInfos[index].CreateChildView("Publish_Paras").Count>0)
                {
                    DataView dw = m_PublishInfos[index].CreateChildView("Publish_Paras")[0].CreateChildView("Paras_Para");

                    for (int i = 0; i < dw.Count; i++)
                    {
                        ePublishData para = new ePublishData();
                        para.DataLabel = dw[i].Row["Label"].ToString();
                        para.DataType = (cGlobalParas.PublishParaType)int.Parse(dw[i].Row["Type"].ToString());
                        para.DataValue = dw[i].Row["Value"].ToString();
                        paras.Add(para);
                    }
                }
                return paras;
            }
            else
                return null;
        }



        //插入一个新的数据源，包括表
        public void InsertPublishInfo(ePublishTask et)
        {
            ///首先判断存放任务执行的目录是否存在
            ///此目录是固定目录，存放在系统\\publish
            XElement xe = new XElement("Publish");
            xe.Add(new XElement("Name", et.pName));
            xe.Add(new XElement("ThreadCount", et.ThreadCount.ToString ()));
            xe.Add(new XElement("IsDelRepeatRow" ,et.IsDelRepeatRow.ToString ()));
            xe.Add(new XElement("PublishType", ((int)et.PublishType).ToString ()));
            xe.Add(new XElement("DataType" , ((int)et.DataType).ToString ()));
            xe.Add(new XElement("DataSource" , et.DataSource));
            xe.Add(new XElement("DataTable",et.DataTable));
            xe.Add(new XElement("InsertSql",et.InsertSql));
            xe.Add(new XElement("UrlCode",((int)et.UrlCode ).ToString ()));
            xe.Add(new XElement("PostUrl", et.PostUrl));
            xe.Add(new XElement("Cookie",et.Cookie));
            xe.Add(new XElement("SucceedFlag",et.SucceedFlag));
            xe.Add(new XElement("PublishIntervalTime", et.PIntervalTime.ToString ()));
            xe.Add(new XElement("IsHeader",et.IsHeader.ToString()));
            xe.Add(new XElement("Header",et.Header));
            xe.Add(new XElement("TemplateName",et.TemplateName));
            xe.Add(new XElement("User",et.User));
            xe.Add(new XElement("Password",et.Password));
            xe.Add(new XElement("Domain",et.Domain));
            xe.Add(new XElement("PublishDbConn",et.TemplateDBConn));

            XElement paras= new XElement("Paras");
            for (int i=0;i<et.PublishParas.Count;i++)
            {
                XElement xe1 = new XElement("Para");
                xe1.Add(new XElement("Label", et.PublishParas[0].DataLabel));
                xe1.Add(new XElement("Value", et.PublishParas[0].DataValue));
                xe1.Add(new XElement("Type", et.PublishParas[0].DataType));

                paras.Add(xe1);
            }

            xe.Add(paras);

            xe.Add(new XElement("IsSqlTrue" , et.IsSqlTrue.ToString()));
            xe.Add(new XElement("isEnginPublish" ,et.isEnginPublish.ToString()));

            //base.xDoc.Root.Add(xe);
            //base.Save();

            base.AddElement(base.xDoc.Root, xe);
            base.Save();

        }

        public bool IsExist(string pName)
        {
            return base.isExist("Publish", "Name", pName);
        }

        public void DelTask(string pName)
        {
            xmlConfig = new cXmlIO(this.m_workPath + "publish\\PublishInfo.xml");
            xmlConfig.DeleteChildNodes("PublishInfos", "Name", pName);
            xmlConfig.Save();
            xmlConfig = null;

            XElement xe= base.SearchElement("Publish", "Name", pName);
            xe.Remove();
            base.Save();
        }

     
    
    }


}

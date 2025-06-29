using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Common;
using NetMiner.Resource;
using System.Data;
using NetMiner.Base;

namespace MinerSpider
{
    
    public class cSynDb
    {
        cXmlIO xmlConfig;
        DataView m_SynDb;

        public cSynDb ()
        {
            try
            {
                xmlConfig = new cXmlIO(Program.getPrjPath () + "dict\\index.xml");

                //获取TaskClass节点
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        ~cSynDb ()
        {
            xmlConfig = null;
        }

        public int GetSynCount()
        {
            m_SynDb = new DataView();
            m_SynDb = xmlConfig.GetData("descendant::SynDb");
            int tCount = m_SynDb.Count;
            return tCount;
        }

        public string GetSynName(int index)
        {
            string dName = m_SynDb[index].Row["Name"].ToString();
            return dName;
        }

        public cGlobalParas.SynonymType GetSynType(int index)
        {
            string dType = m_SynDb[index].Row["Type"].ToString();
            return (cGlobalParas.SynonymType)int.Parse(dType);
        }

        public void SetNull()
        {
            m_SynDb = null;
        }

        public DataView GetSynDb()
        {
            DataView SynDb = new DataView();
            SynDb = xmlConfig.GetData("descendant::SynDb");
            return SynDb;
        }

        public void AddSynDb(string DbName)
        {
            //添加分类的节点
            string str;

            str = "<Name>" + DbName + "</Name>";
            str += "<Type>" + (int)cGlobalParas.SynonymType.Custom + "</Type>";
            xmlConfig.InsertElement("SynDb", "Db", str);

            xmlConfig.Save();


        }

        //删除分类节点
        public void DelSynDb(string DbName)
        {

            xmlConfig.DeleteChildNodes("SynDb", "Name", DbName);
            xmlConfig.Save();
        }

    }
}

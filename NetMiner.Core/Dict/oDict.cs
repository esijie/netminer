using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Collections;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NetMiner.Base;

/// <summary>
/// 从5.5开始，此类迁入NetMiner.Common包中
/// </summary>
///功能：字典处理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：下一版字典部分要强化
///说明：
///版本：01.10.00
///修订：无
///2013-9-18对此类进行修改，字典内容不再存储到dict.xml文件中，
///而是以单独的文本文件进行存储，文本文件编码为utf8
namespace NetMiner.Core.Dict
{
    public class oDict:XmlUnity
    {
        private string m_workPath = string.Empty;
        private string m_DictName = string.Empty;
        private XDocument m_xDoc;

        #region 构造 析构 函数
        public oDict(string workPath)
        {
            string dictName = workPath + "dict\\dict.xml";

            this.m_DictName = dictName;

            if (!File.Exists(dictName))
            {
                CreateIndex(dictName);
            }

            m_xDoc = XDocument.Load(dictName);
        }

        ~oDict()
        {
            m_xDoc = null;
        }

        #endregion

        #region IDisposable 成员
        private bool m_disposed;
        /// <summary>
        /// 释放由 Download 的当前实例使用的所有资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                  
                    if (m_xDoc != null)
                    {
                        m_xDoc = null;
                    }
                }

                // 在此释放非托管资源

                m_disposed = true;
            }
        }


        #endregion

        /// <summary>
        /// 创建dict.xml文件
        /// </summary>
        /// <param name="dictName"></param>
        private void CreateIndex(string dictName)
        {
         
            XDocument xdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("DictConfig",
                        new XElement("DictClasses"),
                        new XElement("Dict")
                ));

            xdoc.Save(dictName);

            xdoc = null;
        }

        //必须首先调用GetDictClassCount
        public int GetDictClassCount()
        {
          
            int count = (
                     from ele in
                     m_xDoc.Root.Descendants("DictClass")
                     select ele
                     ).Count();

            return count;
        }

        public IEnumerable<string> GetDictClass()
        {
            //var dName = (
            //                from ele in
            //                     m_xDoc.Root.Elements("DictClass")
            //                select ele
            //             ).Select(s => s.Element("Name").Value);

            var dName = (from ele in
                                 m_xDoc.Root.Descendants("DictClass")
             select ele).Select(s => s.Element("Name").Value);
            return dName;
        }

        public bool isExitDict(string dName)
        {
            int count = (from ele in m_xDoc.Root.Elements("DictClasses")
                         where ele.Element("Name").Value.ToString().Equals(dName)
                         select ele
                         ).Count();

            if (count > 0)
                return true;
            else
                return false;
        }

        //根据指定的字典名称返回字典内容
        public ArrayList GetDict(string DictName)
        {
            string dictName = m_workPath + "dict\\dic\\" + DictName + ".dic";

            if (!File.Exists(dictName))
            {
                //throw new NetMinerException("指定的字典参数不存在，请检查字典数据！");
                return null;
            }

            ArrayList aList = new ArrayList();

            StreamReader fileReader = new StreamReader(dictName, System.Text.Encoding.UTF8);
            string strLine = "";

            while (strLine != null)
            {
                strLine = fileReader.ReadLine();
                if (strLine != null && strLine.Length > 0)
                {
                    aList.Add(strLine.Trim());
                }
            }

            fileReader.Close();
            fileReader = null;

            return aList;
        }

        #region 字典分类维护
        public void AddDictClass(string DictClassName)
        {
            XElement xe = new XElement("DictClass");
            xe.Add(new XElement("Name"));
            xe.Element("Name").Value = DictClassName;

            m_xDoc.Root.Element("DictClasses").Add(xe);

            this.Save();

        }

        //删除分类节点
        public void DelDictClass(string DictClass)
        {
            XElement xe = (from ele in m_xDoc.Root.Descendants("DictClass")
                              where ele.Element("Name").Value.ToString().Equals(DictClass)
                              select ele
                         ).SingleOrDefault();

            xe.Remove();

            //删除字典文件
            string dictName = m_workPath + "dict\\dic\\" + DictClass + ".dic";
            File.Delete(dictName);

        }
        #endregion

        #region 字典的维护
        public void AddDict(ref DataTable d, string DictName)
        {
            d.Rows.Add(DictName);
        }

        //编辑字典内容
        public void EditDict(DataTable d, string Old_Name, string DictName)
        {
            for (int i = 0; i < d.Rows.Count; i++)
            {
                if (d.Rows[i][0].ToString() == Old_Name)
                    d.Rows[i][0] = DictName;
            }
        }

        public void Save(string dName, ArrayList d)
        {
            string dictName = m_workPath + "dict\\dic\\" + dName + ".dic";

            File.Delete(dictName);

            StreamWriter sw = new StreamWriter(dictName, true, System.Text.Encoding.UTF8);
            for (int i = 0; i < d.Count; i++)
            {
                sw.WriteLine(d[i].ToString());
            }

            sw.Close();
            sw.Dispose();
        }

        private  new void Save()
        {
            m_xDoc.Save(m_DictName);
        }


        #endregion

    }
}

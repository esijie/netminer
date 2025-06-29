using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Xml.XPath;
using System.Runtime.InteropServices;
using System.Threading;

namespace NetMiner.Base
{
    public class XmlUnity:IDisposable
    {

        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);
        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public readonly IntPtr HFILE_ERROR = new IntPtr(-1);


        private XDocument m_xDoc=null;
        private bool m_isSave = false;
        private string m_xmlName = string.Empty;
        private string m_workPath = string.Empty;

        #region 构造和析构
        public XmlUnity()
        {

        }

        /// <summary>
        /// 系统工作路径
        /// </summary>
        /// <param name="workPath"></param>
        public XmlUnity(string workPath)
        {
            m_workPath = workPath;
        }

        ~XmlUnity()
        {
            if (this.m_isSave && m_xDoc!=null)
                m_xDoc.Save(m_xmlName);
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
                        if (this.m_isSave)
                            m_xDoc.Save(m_xmlName);
                        m_xDoc = null;
                    }
                }

                // 在此释放非托管资源

                m_disposed = true;
            }
        }


        #endregion

        protected XDocument xDoc
        {
            get { return m_xDoc; }
            set { m_xDoc = value; }
        }

        protected void LoadXML(string xmlFile)
        {
            m_xmlName = xmlFile;


            IntPtr vHandle = _lopen(xmlFile, OF_READWRITE | OF_SHARE_DENY_NONE);

            int i = 0;

            while (vHandle == HFILE_ERROR && i<10)
            {
                Thread.Sleep(300);
                i++;
                
            }
            CloseHandle(vHandle);

            m_xDoc = XDocument.Load(xmlFile);
            m_isSave = false;
        }

        protected void LoadXML(TextReader tReader)
        {
            m_xDoc=XDocument.Load(tReader, LoadOptions.None);
        }


        protected void RefreshXML(string xmlFile)
        {
            if (this.m_isSave == true && m_xDoc!=null)
                m_xDoc.Save(m_xmlName);

            
            m_xDoc = null;

            m_xDoc = XDocument.Load(xmlFile);
            m_xmlName = xmlFile;
            m_isSave = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pEle">父节点</param>
        /// <param name="xe">子节点</param>
        protected void AddElement(XElement pEle,XElement xe)
        {
            pEle.Add(xe);
            this.m_isSave = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlName"></param>
        /// <param name="xe"></param>
        protected void NewXML(string xmlName, XElement xe)
        {
            XDocument xdoc = new XDocument( new XDeclaration("1.0", "utf-8", "yes"),xe);

            string xmlPath = Path.GetDirectoryName(xmlName);
            if (!Directory.Exists(xmlPath))
                Directory.CreateDirectory(xmlPath);

            xdoc.Save(xmlName);
            xdoc = null;
        }

        /// <summary>
        /// 获取指定节点的数量
        /// </summary>
        protected int GetNodesCount(string nodeName)
        {
            int count = (from e in m_xDoc.Root.Descendants(nodeName)
                         select e).Count();
            return count;
        }

        protected int GetNodesCount(XElement xe,string nodeName)
        {
            int count= (from e in xe.Descendants(nodeName)
                        select e).Count();
            return count;
        }

        protected bool isExist(string searchNodeName, string nodeName, string nodeValue)
        {
            int count = (from e in m_xDoc.Descendants(searchNodeName)
                         where e.Element(nodeName).Value.ToString().Equals(nodeValue)
                         select e).Count();

            if (count == 0)
                return false;
            else
                return true;
            
        }

        protected int GetMaxID(XElement xe,string nodeName)
        {
            if (xe.XPathSelectElement(nodeName) == null)
                return 1;
            var MaxID = (from e1 in xe.Elements(nodeName)
                            select e1).Select(s => int.Parse(s.Element("ID").Value.ToString())).Max();
            return MaxID;
        }

        /// <summary>
        /// 根据xpath获取指定节点的值
        /// </summary>
        /// <param name="xPath"></param>
        /// <returns></returns>
        protected string GetValue(string xPath)
        {
            return m_xDoc.Root.XPathSelectElement(xPath).Value.ToString();
        }

        protected void EditValue(string xPath,string NewValue)
        {
            XElement xe = m_xDoc.Root.XPathSelectElement(xPath);
            xe.Value = NewValue;
            this.m_isSave = true;
        }

        protected void EditValue(XElement xe,string NewValue)
        {
            xe.Value = NewValue;
            this.m_isSave = true;
        }

        protected XElement SearchElement(string rootName, string nodeName,string nodeValue)
        {
            XElement xe = (from e in m_xDoc.Root.Descendants(rootName)
                           where e.Element(nodeName).Value.Equals(nodeValue)
                           select e).SingleOrDefault();

            //XElement xe1 = (from e1 in m_xDoc.Root.Descendants("Task")
            //               where e1.Element("Name").Value.Equals("test")
            //               select e1).SingleOrDefault();
            return xe;
        }

        protected IEnumerable<XElement> SearchAllElement(string rootName,string nodeName,string nodeValue)
        {
            IEnumerable<XElement> xes = (from e in m_xDoc.Root.Descendants(rootName)
                           where e.Element(nodeName).Value.Equals(nodeValue)
                           select e).ToList();

            return xes;
        }

        protected XElement GetRdElement(string rootName)
        {
            XElement xe = (from e in m_xDoc.Root.Descendants(rootName)
                           select e).SingleOrDefault();
            return xe;
        }

        protected void RemoveElement(XElement xe)
        {
            if (xe != null)
            {
                xe.Remove();
                m_isSave = true;
            }
        }

        protected void RemoveElements(IEnumerable<XElement> xes)
        {
            if (xes != null)
            {
                xes.Remove();
                m_isSave = true;
            }
        }

        protected IEnumerable<XElement> GetAllElement(string nodeName)
        {
            var xes = (from e in m_xDoc.Root.Descendants(nodeName)
                       select e);
            return xes;
        }

        protected IEnumerable<XElement> GetAllElement(string nodeName,string strOrder)
        {
            var xes = (from e in m_xDoc.Root.Descendants(nodeName) 
                       orderby strOrder
                       select e);
            return xes;
        }

        protected IEnumerable<XElement> GetElements(XElement xe,string eName)
        {
            var xes = (from e in xe.Elements(eName)
                       select e);
            return xes;
        }

        protected XElement GetElement(string xPath)
        {
            return m_xDoc.Root.XPathSelectElement(xPath);
        }

        public void Save()
        {
            if (this.m_isSave == true)
            {
                m_xDoc.Save(this.m_xmlName);
                this.m_isSave = false;
            }
        }

        protected void Save(string fileName)
        {

        }

        protected void SaveBinary()
        {

        }
    }
}

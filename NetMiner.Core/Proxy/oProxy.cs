using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NetMiner.Base;
using NetMiner.Core.Proxy.Entity;
using System.IO;

namespace NetMiner.Core.Proxy
{
    public class oProxy : XmlUnity
    {

        private string m_workPath = string.Empty;

        public oProxy(string workPath)
        {
            m_workPath = workPath;
            string fileName = this.m_workPath + NetMiner.Constant.g_ProxyFile;
            if (!File.Exists(fileName))
            {
                NewProxyFile();
            }
            //string sXML = NetMiner.Common.Tool.cFile.ReadFileBinary(fileName);
            //TextReader tReader = new StringReader(sXML);
            base.LoadXML(fileName);
        }

        ~oProxy()
        {
            base.Dispose();
        }

        private void NewProxyFile()
        {
            XElement xe = new XElement("Proxys");
            base.NewXML(this.m_workPath + NetMiner.Constant.g_ProxyFile, xe);
        }

        public IEnumerable<eProxy> LoadProxyData()
        {
            IEnumerable<XElement> xes = base.GetAllElement("Proxy");
            IEnumerable<eProxy> eProxys = xes.Select<XElement, eProxy>(
                    s => Convert(s));

            return eProxys;
        }

        public eProxy LoadFirstProxy()
        {
            XElement xe = base.GetRdElement("Proxy");
            if (xe != null)
                return Convert(xe);
            else
                return null;
        }

        private eProxy Convert(XElement xe)
        {
            eProxy ep = new Entity.eProxy();
            ep.ProxyServer = xe.Element("ProxyServer").Value.ToString();
            ep.ProxyPort = xe.Element("ProxyPort").Value.ToString();
            ep.User = xe.Element("User").Value.ToString();
            ep.Password = xe.Element("PWD").Value.ToString();

            return ep;
        }

        public void InsertProxy(eProxy ep)
        {
            XElement xe = new XElement("Proxy");
            xe.Add(new XElement("ProxyServer", ep.ProxyServer));
            xe.Add(new XElement("ProxyPort", ep.ProxyPort));
            xe.Add(new XElement("User", ep.User));
            xe.Add(new XElement("PWD", ep.Password));

            base.AddElement(base.xDoc.Root, xe);
            base.Save();
        }

        /// <summary>
        /// 删除所有代理信息
        /// </summary>
        public void Dele()
        {
            base.RemoveElements(base.xDoc.Root.Elements("Proxy"));
            base.Save();
        }


    }
}

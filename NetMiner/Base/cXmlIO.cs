using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using NetMiner;

///���ܣ�xml�ļ�����
///���ʱ�䣺2009��3��2
///���ߣ�һ��
///�������⣺��ǰ��xml�������Ǻܲ����㣬��xml�ļ���ʽ֧�ֵĲ�����
/// ������Ҫ����һ�׶����½����޸�
///�����ƻ�����
///˵�����μ�ע��
///�汾��01.10.00
///�޶�����
///����һ��XML�����࣬��������ʹ�õ����������V5.5��ʼ���𲽿�ʼ�滻��Linq for xml����������Ŀ���ǿ���
///ʹ��Linq��ͨ�ýӿڣ������������XML���ݺܶ࣬��û����ȫ�滻����ˣ����໹�����Ա���ʹ�á�
namespace NetMiner.Base
{
    public class cXmlIO
    {
        protected XmlDocument objXmlDoc;
        protected string strXmlFile;


        public XmlDocument XmlDoc
        {
            get{ return objXmlDoc;}
        }
        public string InnerXml
        {
            get { return objXmlDoc.InnerXml; }
        }

        public cXmlIO()
        {
            objXmlDoc = new XmlDocument();
        }

        public cXmlIO(string XmlFile)
        {
            objXmlDoc = new XmlDocument();

            try
            {
                if (File.Exists(XmlFile))
                {
                    FileStream fs = File.Open(XmlFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
                    string strXml = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();
                    fs.Close();
                    fs.Dispose();
                    objXmlDoc.LoadXml(strXml);
                }
                else
                {
                    objXmlDoc = null;
                    throw new NetMinerException(XmlFile + " �����ڣ�");
                }

            }
            catch (System.IO.IOException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                objXmlDoc = null;
                throw new Exception(ex.Message + XmlFile);
            }

            strXmlFile = XmlFile;
         }

        public void LoadXML(string strXML)
        {
            objXmlDoc.LoadXml(strXML);
            strXmlFile = strXML;
        }

        ~cXmlIO()
        {
            objXmlDoc = null;
        }

        public void NewXmlFile(string FileName,string strXml )
        {
            //��ȡ·��

            //string s = "\\b.*(?=\\\\)\\b";
            //Match m = Regex.Match(FileName, s);
            //string fPath = m.Groups[0].Value.ToString();

            string fPath = Path.GetDirectoryName(FileName);

            if (!System.IO.Directory.Exists(fPath))
            {
                //Ŀ¼�����ڣ�������Ҫ������Ŀ¼
                System.IO.Directory.CreateDirectory(fPath);
            }

            objXmlDoc = new XmlDocument();
            objXmlDoc.LoadXml(strXml);

            if (File.Exists(FileName))
            {
                File.SetAttributes(FileName, System.IO.FileAttributes.Normal);
            }

            objXmlDoc.Save(FileName);

            strXmlFile = FileName;

        }

        //����ָ����·����ȡһ��ֵ
        public string GetNodeValue(string nodPath)
        {
            XmlNode gNode = objXmlDoc.SelectSingleNode(nodPath);

            if (gNode == null)
                return "";
            else
                return  gNode.InnerText.ToString ();
        }

        //���ݽڵ㷵������,����Ϊdataview
        public DataView GetData(string XmlPathNode)
        {
            DataSet ds = new DataSet();
            StringReader read = new StringReader(objXmlDoc.SelectSingleNode(XmlPathNode).OuterXml);
            ds.ReadXml(read);
            if (ds.Tables.Count == 0)
            {
                return null;
            }
            else
            {
                return ds.Tables[0].DefaultView;
            }
        }

        //ɾ��ָ���ڵ��µ������ӽڵ�
        public void DeleteNodeChile(string Node)
        {
            XmlNodeList nodes = objXmlDoc.GetElementsByTagName(Node);
            XmlNode delNode = nodes[0];
            if (delNode != null)
                delNode.RemoveAll();
        }

        //����ָ���ڵ�ı��,�����ؽڵ�����,���ܷ��ص���һ����¼,
        //��������ΪDataView���з���,��������Ŀ����Ϊ�˸��õķ�����
        public DataView GetData(string NodeCollection, string Node, string content)
        {
            XmlNodeList fathernode = objXmlDoc.GetElementsByTagName(NodeCollection);
            XmlNodeList nodes = fathernode[0].ChildNodes;

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes[i].ChildNodes.Count; j++)
                {
                    //for (int m=0;
                    if (nodes[i].ChildNodes[j].Name == Node && nodes[i].ChildNodes[j].InnerText == content)
                    {
                        StringReader read = new StringReader(nodes[i].OuterXml);
                        DataSet ds = new DataSet();
                        ds.ReadXml(read);
                        if (ds.Tables.Count == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return ds.Tables[0].DefaultView;
                        }

                    }
                }

            }
            return null;
           
        }

        //ɾ���ڵ�
        //����ָ���Ľڵ�ɾ���˽ڵ��Լ��˽ڵ�һ�µ�����
        public void DeleteNode(string Node)
        {
            XmlNodeList nodes = objXmlDoc.GetElementsByTagName(Node);
            XmlNode delNode = nodes[0];
            if (delNode !=null)
                delNode.ParentNode.RemoveChild(delNode);
        }

        //����ָ���Ľڵ㣬ɾ���ӽڵ����content���ݵ��ӽڵ�
        //�˷����Ƚ����⣬���������������е�xml�����в���������֧�����е�xml�ļ�
        //��������е�xml�ļ��У�ͨ��������1�Զ�Ĺ�ϵ�����ֹ�ϵͨ��һ����
        //�ظ��Ľڵ�����ʾ������ɾ����ʱ�򣬲���ָ������ڵ㣬����ָ������ڵ�ĸ��ڵ�
        //��ΪҪѭ�������е����ݣ����ݼ����е�һ���ڵ㣬���µ�����������ɾ��
        //����<tasks><task><id>1</id><name>soukey</name></task><task><id>2</id><name>��ժ</name></task></tasks>
        //ɾ���ӽڵ���ָɾ��task�ڵ㣬�����ݵ�������ָ����id����name����content�����ݣ�
        //���Ե��÷�����DeleteChildNodes("tasks","name","soukey")
        //���ú󣬽�ɾ��task��name��soukey��task�ڵ�,�����MainNode������һ�����ϣ�����������һ������ӽڵ㣬
        //�����´���
        public void DeleteChildNodes(string NodeCollection,string Node, string content)
        {
            XmlNodeList fathernode = objXmlDoc.GetElementsByTagName(NodeCollection);
            if (fathernode.Count == 0)
                return;

            XmlNodeList nodes = fathernode[0].ChildNodes;

            for (int i=0;i< nodes.Count; i++)
            {
                for (int j = 0; j < nodes[i].ChildNodes.Count; j++)
                {
                    //for (int m=0;
                    if (nodes[i].ChildNodes[j].Name ==Node &&  nodes[i].ChildNodes[j].InnerText == content)
                    {
                        fathernode[0].RemoveChild(nodes[i]);
                        return;
                    }
                }

            }

        }

        //����һ���ڵ�ʹ˽ڵ��һ�ӽڵ�
        public void InsertNode(string MainNode,string ChildNode,string Element,string Content)
        {
           XmlNode objRootNode = objXmlDoc.SelectSingleNode(MainNode);
           XmlElement objChildNode = objXmlDoc.CreateElement(ChildNode);
           objRootNode.AppendChild(objChildNode);
           XmlElement objElement = objXmlDoc.CreateElement(Element);
           objElement.InnerText = Content;
           objChildNode.AppendChild(objElement);

        }

        //�޸�һ���ڵ��������Ϣ��Ϣ
        public void EditNode(string Element, string Old_Content,string Content)
        {

            XmlNodeList nodes = objXmlDoc.GetElementsByTagName(Element);

            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                if (nodes[i].ChildNodes[0].InnerText == Old_Content) 
                {
                    nodes[i].ChildNodes[0].InnerText = Content;
                }

            }
        }

        //�޸�һ���ڵ㱾���ֵ
        public void EditNodeName(string nodPath, string OldName, string NewName)
        {
            XmlNode Nod = objXmlDoc.SelectSingleNode(nodPath);
            string xml = Nod.InnerXml;

            DeleteNode(OldName);

            nodPath = nodPath.Substring(0, nodPath.LastIndexOf("/"));

            InsertElement(nodPath, NewName, xml);

        }

        //����ָ���Ľڵ��޸���ֵ
        public void EditNodeValue(string nodPath,string NewValue)
        {
            XmlNode Nod= objXmlDoc.SelectSingleNode(nodPath);
            Nod.InnerText = NewValue;
        }

        /// <summary>
        /// ר���ڲɼ������xml�ĵ�ά��������һ���ݹ�Ĳ���������xml�ĵ����ô˷���
        /// ����IDƥ��ķ�����ƥ��ɹ����ڴ˽ڵ��²�������
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="label"></param>
        /// <param name="Value"></param>
        /// <param name="Element"></param>
        /// <param name="Content"></param>
        public void InsertElement(string nodeName,string label,string Value ,string Element,string Content)
        {
            XmlNodeList nodes = objXmlDoc.GetElementsByTagName(nodeName);

            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < nodes[i].ChildNodes.Count; j++)
                {
                    if (nodes[i].ChildNodes[j].Name.ToString () == label.ToLower().ToString () && nodes[i].ChildNodes[j].InnerText == Value)
                    {
                        XmlElement objElement = objXmlDoc.CreateElement(Element);
                        objElement.InnerXml  = Content;
                        nodes[i].AppendChild(objElement);

                        return;
                    }

                    if (nodes[i].ChildNodes[j].Name=="Childrens")
                    {
                        InsertElement(nodes[i].ChildNodes[j].ChildNodes, label, Value, Element, Content);
                    }
                }
            }
        }

        /// <summary>
        /// ����һ���ݹ�Ĳ��룬ר���ڲɼ��������
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="label"></param>
        /// <param name="Value"></param>
        /// <param name="Element"></param>
        /// <param name="Content"></param>
        private void InsertElement(XmlNodeList nodes,string label,string Value,string Element,string Content)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < nodes[i].ChildNodes.Count; j++)
                {
                    if (nodes[i].ChildNodes[j].Name.ToString() == label.ToLower().ToString() && nodes[i].ChildNodes[j].InnerText == Value)
                    {
                        XmlElement objElement = objXmlDoc.CreateElement(Element);
                        objElement.InnerXml = Content;
                        nodes[i].AppendChild(objElement);

                        return;
                    }

                    if (nodes[i].ChildNodes[j].Name == "Childrens")
                    {
                        InsertElement(nodes[i].ChildNodes[j].ChildNodes, label, Value, Element, Content);
                    }
                }
            }
        }

        public void AppendElement(string nodeName, string label, string Value,string Element, string Content)
        {
            XmlNodeList nodes = objXmlDoc.GetElementsByTagName("TaskClass");

            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < nodes[i].ChildNodes.Count; j++)
                {
                    if (nodes[i].ChildNodes[j].Name.ToString() == label.ToLower().ToString() && nodes[i].ChildNodes[j].InnerText == Value)
                    {
                       XmlNode xnode12 = nodes[i].SelectSingleNode("Childrens");
                       XmlElement objElement = objXmlDoc.CreateElement(Element);
                       objElement.InnerXml = Content;
                       xnode12.AppendChild(objElement);

                        return;
                    }
                }
            }
        }


        //����һ���ڵ�
        public void InsertElement(string MainNode,string Element,string Content)
        {
           XmlNode objNode = objXmlDoc.SelectSingleNode(MainNode);
           XmlElement objElement = objXmlDoc.CreateElement(Element);
           objElement.InnerXml  = Content;
           objNode.AppendChild(objElement);
        }

        //����xml�ļ�
        public void Save()
        {
           try
           {
                   if (File.Exists(strXmlFile))
                   {
                       FileStream myStream = File.Open(strXmlFile, FileMode.Create, FileAccess.Write, FileShare.Write);
                       StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.UTF8);
                       sw.Write(objXmlDoc.InnerXml);
                       sw.Close();
                       sw.Dispose();
                       myStream.Close();
                       myStream.Dispose();
                       //objXmlDoc.Save(strXmlFile);
                   }
           }
           catch (System.Exception ex)
           {
                throw ex;
           }
           
        }

        public bool EditNodeValue(string NodeCollection, string Node, string condition, string ValueName,string value)
        {
            bool isModify = false;

            XmlNodeList fathernode = objXmlDoc.GetElementsByTagName(NodeCollection);
            XmlNodeList nodes = fathernode[0].ChildNodes;

            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes[i].ChildNodes.Count; j++)
                {
                    //for (int m=0;
                    if (nodes[i].ChildNodes[j].Name == Node && nodes[i].ChildNodes[j].InnerText == condition)
                    {
                        XmlNode nod = nodes[i].SelectSingleNode(ValueName);
                        nod.InnerText = value;
                        isModify = true;
                        return isModify;
                    }
                    
                   
                }

            }

            return isModify;
        }

      
        public void EditNodeValue(XmlNodeList nodes, string Node, string condition, string ValueName, string value)
        {

            for (int i = 0; i < nodes.Count; i++)
            {

                try
                {
                    if (nodes[i].Name == Node && nodes[i].InnerText == condition)
                    {
                        XmlNode nod = nodes[i].ParentNode.SelectSingleNode(ValueName);
                        nod.InnerText = value;
                    }

                    if (nodes[i].ChildNodes.Count > 0)
                    {
                        EditNodeValue(nodes[i].ChildNodes, Node, condition, ValueName, value);
                    }
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

            }

           
        }

       
    }
      
}

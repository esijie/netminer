using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;


namespace NetMiner.Base
{
    /// <summary>
    /// 此结构用于排重库使用，不做它用，排重库后续可支持Berkeley DB
    /// </summary>
    public class cHashTree
    {
        public HashTreeNode Root;
        public bool Modified;
        public int Count;

        public cHashTree()
        {
        }

        public void Clear()
        {
            Root = null;
            Count = 0;
            Modified = false;
        }

        //增加一个地址，增加成功返回 true ，增加失败 返回 false
        public bool Add(ref string str, bool IsHash)
        {
            int Code = 0;

            if (IsHash)
                Code = int.Parse(str);
            else
                Code = str.GetHashCode();

            HashTreeNode node;
            if (Root == null)
            {
                Root = new HashTreeNode();
                node = Root;
            }
            else
            {
                node = Root;
                while (true)
                {
                    if (node.Code == Code)
                        return false;
                    if (Code < node.Code)
                    {	// add the node at the small branch
                        if (node.Small == null)
                        {
                            node.Small = new HashTreeNode();
                            node.Small.Parent = node;
                            node = node.Small;
                            break;
                        }
                        node = node.Small;
                    }
                    else
                    {	// add the node at the great branch
                        if (node.Great == null)
                        {
                            node.Great = new HashTreeNode();
                            node.Great.Parent = node;
                            node = node.Great;
                            break;
                        }
                        node = node.Great;
                    }
                }
            }
            node.Code = Code;
            this.Modified = true;

            Root.Count++;
            int id = this.Count++;
            node.ID = id;

            return true;
        }

        public bool Del(string str, bool IsHash)
        {
            int Code = 0;

            if (IsHash)
                Code = int.Parse(str);
            else
                Code = str.GetHashCode();

            HashTreeNode node;
            if (Root == null)
            {
                Root = new HashTreeNode();
                node = Root;
                Root = null;
                return true;
            }
            else
            {
                node = Root;
                int i=0;
                while (true && i<Root.Count )
                {
                    if (node.Code == Code)
                    {
                        if (node.Parent == null)
                        {
                            Root = null;
                        }
                        else
                        {
                            if (Code < node.Parent.Code)
                            {	// add the node at the small branch
                                node.Parent.Small = null;
                            }
                            else
                            {	// add the node at the great branch
                                node.Parent.Great = null;
                            }
                        }

                        return true;
                    }
                    else
                    {
                        if (Code < node.Code)
                        {	
                           
                            node = node.Small;
                        }
                        else
                        {	
                           
                            node = node.Great;
                        }
                    }
                    i++;
                }
            }
          
            return false ;
        }

        public void Open(string FileName)
        {
            if (!System.IO.File.Exists(FileName))
                return;

            this.Root = new HashTreeNode();


            FileStream fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            
            BinaryFormatter b = new BinaryFormatter();
            this.Root = b.Deserialize(fileStream) as HashTreeNode;
            
            fileStream.Close();
            fileStream.Dispose();
            b = null;

        }

        public void Save(string fileName)
        {
            if (this.Root == null)
                return;

            SaveFile(fileName,this.Root);

            Modified = false;
        }

        private void SaveFile(string fileName, HashTreeNode node)
        {
            FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

            //MemoryStream stream = new MemoryStream();

            ////二进制序列化
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(stream, node);

            stream.Close();
        }

        
    }

    [Serializable]
    public class HashTreeNode
    {
        public HashTreeNode Small;
        public HashTreeNode Great;
        public HashTreeNode Parent;
        public int Code;

        //只有根节点此值才有效，主要用于计算当前共有多少个节点
        public int Count;
        public int ID;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Seg;
using NetMiner.Seg.Segment;
using NetMiner.Seg.Dict;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace SoMinerSegCustomRule
{
    public class PickupCNEN : ICustomRule 
    {
        public static bool g_Ini = false;
        public static Hashtable g_CE;
        private string _Text;
        public string Text
        {
            get{return _Text;}
            set
            {
                ///这是一个临时的处理方法，因为没有接口提供
                ///所以，占用了这个属性进行调用
                string ss = value;
                if (ss == "sominerReIni")
                    g_Ini = false;
                else
                    _Text = value;
            }
        }

        public void AfterSegment(SuperLinkedList<WordInfo> result)
        {
            if (g_Ini == false)
            {
                IniCEWords();
                g_Ini = true;
            }

            bool isMatch = false;
            string ceWord = string.Empty;

            if (g_CE == null)
                return;

            foreach (DictionaryEntry de in g_CE)
            {
                ceWord = de.Key.ToString();
                int index = 0;
                SuperLinkedListNode<WordInfo> node = result.First;
                SuperLinkedListNode<WordInfo> vWordNode = null;
                SuperLinkedListNode<WordInfo> lastNode = null;

                if (ceWord.Length > 1)
                {
                    while (node != null && index<ceWord.Length )
                    {
                        try
                        {
                            string Letter = ceWord.Substring(index, 1);
                            if (vWordNode == null)
                            {
                                if (node.Value.Word.Length >= 1)
                                {
                                    if (node.Value.Word[0].ToString().StartsWith(Letter, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        string mWord = node.Value.Word.ToLower();
                                        bool isM = true;
                                        //开始匹配这个词语的其他字符
                                        for (int i = 0; i < mWord.Length; i++)
                                        {
                                            if (index < ceWord.Length)
                                            {
                                                Letter = ceWord.Substring(index, 1).ToLower();
                                                if (Letter != mWord.Substring(i, 1).ToLower())
                                                {
                                                    isM = false;
                                                    break;
                                                }

                                                index++;
                                            }
                                        }

                                        if (isM == true)
                                        {
                                            vWordNode = node;
                                            lastNode = node;
                                        }
                                        else
                                        {
                                            index = 0;
                                        }

                                    }
                                }
                            }
                            else if (vWordNode != null)
                            {
                                //如果V有多元分词情况，忽略，跳到下一个
                                if (node.Value.Position == vWordNode.Value.Position)
                                {
                                    node = node.Next;
                                    continue;
                                }

                                ////开始匹配
                                //if (node.Value.Word.ToLower() == Letter.ToLower())
                                //{
                                //    index++;
                                //    isMatch = true;
                                //}
                                //else
                                //    isMatch = false;


                                string mWord = node.Value.Word.ToLower();
                                bool isM = true;
                                for (int i = 0; i < mWord.Length; i++)
                                {
                                    if (index < ceWord.Length)
                                    {
                                        Letter = ceWord.Substring(index, 1).ToLower();
                                        if (Letter != mWord.Substring(i, 1).ToLower())
                                        {
                                            isM = false;
                                            break;
                                        }
                                        index++;
                                    }
                                }

                                if (isM == true)
                                {
                                    isMatch = true;
                                }
                                else
                                {
                                    isMatch = false;
                                    vWordNode = null;
                                    index = 0;
                                }

                                if (index == ceWord.Length && isMatch == true)
                                {
                                    index = 0;
                                    WordInfo wordinfo = new WordInfo(ceWord, node.Value.Position, node.Value.Pos,
                                        node.Value.Frequency, node.Value.Rank, node.Value.WordType, node.Value.OriginalWordType, 0);
                                    node = result.AddAfter(node, wordinfo);
                                    isMatch = false;
                                    break;
                                }
                            }

                            node = node.Next;
                        }
                        catch (System.Exception ex)
                        {
                            throw ex;
                        }
                    }

                }
                
            }

           
        }

        

        private void IniCEWords()
        {
            string fileName =Application.StartupPath  + "\\dict\\ce.txt";
            if (!System.IO.File.Exists(fileName))
            {
                return;
            }

            FileStream myStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(myStream, System.Text.Encoding.UTF8);

            g_CE = new Hashtable();

            string strLine = string.Empty;
            while (strLine != null)
            {
                strLine = sr.ReadLine();
                if (!string.IsNullOrEmpty (strLine ) && strLine.Length > 0)
                {
                    if (!g_CE.Contains(strLine))
                    {
                        g_CE.Add(strLine, null);
                    }
                }
            }


            sr.Close();
            sr.Dispose();
            myStream.Close();
            myStream.Dispose();

        }
    }
}

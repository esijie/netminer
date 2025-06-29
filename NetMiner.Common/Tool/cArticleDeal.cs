using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Seg;
using NetMiner.Seg.Segment;
using NetMiner.Seg.Dict;
using System.Text.RegularExpressions;
using System.IO;

namespace NetMiner.Common.Tool
{

    public class cArticleDeal
    {
        public static bool m_IsIniSeg = false;
        private MatchOptions _Options;
        private MatchParameter _Parameters;

        private static bool m_IsIniSynDb = false;
        private static string[] m_SynDb;
        private string[] m_CustomSynDb;

        private string m_workPath = string.Empty;

        public cArticleDeal (string workPath)
        {
            m_workPath = workPath;

            _Options = new MatchOptions();
            _Parameters = new MatchParameter();

            _Options.FrequencyFirst = true;
            _Options.FilterStopWords = false;
            _Options.ChineseNameIdentify = true ;
            _Options.MultiDimensionality = true ;
            _Options.EnglishMultiDimensionality = false  ;
            _Options.ForceSingleWord = false ;
            _Options.TraditionalChineseEnabled = true ;
            _Options.OutputSimplifiedTraditional = false ;
            _Options.UnknownWordIdentify = true ;
            _Options.FilterEnglish = false ;
            _Options.FilterNumeric = false ;
            _Options.IgnoreCapital = true ;
            _Options.EnglishSegment =true ;
            _Options.SynonymOutput = false ;
            _Options.WildcardOutput =false ;
            _Options.WildcardSegment =false ;
            _Options.CustomRule = false ;

            _Parameters.Redundancy = 0;
            _Parameters.FilterEnglishLength =0;
            _Parameters.FilterNumericLength = 0;
        }

        ~cArticleDeal ()
        {
            
        }

        public static void IniSeg()
        {

            NetMiner.Seg.Segment.cSeg.Init("dict");

            m_IsIniSeg = true;
        }

        public static void IniSynDb(string workPath)
        {
            string fileName = workPath + "dict\\Synonym.txt";
            if (!System.IO.File.Exists(fileName))
            {
                return;
            }

            FileStream myStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(myStream, System.Text.Encoding.UTF8);

            string str = "";
            string strLine = "";
            while (strLine != null)
            {
                strLine = sr.ReadLine();
                if (strLine != null && strLine.Length > 0)
                {
                        str += strLine + "\r\n";
                }
            }


            sr.Close();
            sr.Dispose();
            myStream.Close();
            myStream.Dispose();

            m_SynDb = Regex.Split(str, "\r\n");

        }

        private void IniCustomSynDb( string dbName)
        {
            string fileName = this.m_workPath + "dict\\" + dbName + ".txt";
            if (!System.IO.File.Exists(fileName))
            {
                return;
            }

            FileStream myStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(myStream, System.Text.Encoding.UTF8);

            string str = "";
            string strLine = "";
            while (strLine != null)
            {
                strLine = sr.ReadLine();
                if (strLine != null && strLine.Length > 0)
                {
                    str += strLine + "\r\n";
                }
            }


            sr.Close();
            sr.Dispose();
            myStream.Close();
            myStream.Dispose();

            m_CustomSynDb = Regex.Split(str, "\r\n");
        }

        private string SynReplace(string str)
        {
            //首先进行系统词库替换
            for (int i = 0; i < m_SynDb.Length; i++)
            {
                string[] ss = m_SynDb[i].Split(',');
                for (int j = 0; j < ss.Length; j++)
                {
                    if (str == ss[j])
                    {
                        if (j >= ss.Length)
                            str = ss[0];
                        else
                            str = ss[ss.Length - 1];

                        return str;
                    }
                }
            }

            return str;
        }

        public string SynonymsReplace(string article, string DbName)
        {
            if (m_IsIniSeg == false)
                IniSeg();

            if (m_IsIniSynDb == false)
                IniSynDb(this.m_workPath);

            IniCustomSynDb(DbName);

            //将文章的空格全部替换成&nbsp;，防止分词时出现错误
            article=Regex.Replace (article,"\\s","&nbsp;");

            cSeg segment = new cSeg();

            ICollection<WordInfo> words = segment.DoSegment(article, _Options);

            StringBuilder wordsString = new StringBuilder();
            foreach (WordInfo wordInfo in words)
            {
                if (wordInfo == null)
                {
                    continue;
                }

                string ss = wordInfo.Word;
                if (ss.Length > 2)
                {
                    //进行系统词替换
                    ss=SynReplace(ss);

                }
                wordsString.AppendFormat("{0}", ss);
                
            }

            string newArticle = wordsString.ToString();

            //进行自定义词库替换
            for (int j = 0; j < m_CustomSynDb.Length; j++)
            {
                string[] ss=m_CustomSynDb[j].Split (',');
                if (ss.Length >=2)
                {
                    newArticle = newArticle.Replace(ss[0], ss[1]);
                }
            }

            //将空格再替换回来
            newArticle=Regex.Replace (newArticle ,"&nbsp;"," ");

            return newArticle;
        }

        public string MergeParagraphs(string article ,int charCount)
        {
            string[] ps;
            bool isP = false;

            //先进行段落拆分，段落划分有两种形式：</p>及\r\n
            if (article.IndexOf("\r\n", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                ps = Regex.Split(article, "\r\n", RegexOptions.IgnoreCase);
                isP = false;
            }
            else
            {
                ps = Regex.Split(article, "</p>", RegexOptions.IgnoreCase);
                isP = true;
            }

            string newArticle = "";
            string tempps = "";

            //合并并还原文章
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i] = Regex.Replace(ps[i], "<p.+?>", "", RegexOptions.IgnoreCase);

                if (ps[i].Trim() != "")
                {
                    if (ps[i].Length < charCount)
                        tempps = ps[i];
                    else
                    {
                        if (isP == true)
                        {
                            

                            
                            if (tempps != "")
                            {
                                newArticle += "<p>" + tempps + ps[i] + "</p>";
                                tempps = "";
                            }
                            else
                                newArticle += "<p>" + ps[i] + "</p>";
                        }
                        else
                        {
                            if (tempps != "")
                            {
                                newArticle += tempps + ps[i] + "\r\n";
                                tempps = "";
                            }
                            else
                                newArticle += ps[i] + "\r\n";
                        }
                    }
                }
            }

            return newArticle;
        }
    }
}

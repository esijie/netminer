using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Seg.Dict;
using Microsoft.VisualBasic;
using NetMiner.Seg.xml;
using System.IO;
using System.Collections ;
using System.Windows.Forms;

namespace NetMiner.Seg.Segment
{
    public class cSeg
    {
        const string PATTERNS = @"([０-９\d]+)|([ａ-ｚＡ-Ｚa-zA-Z_]+)";

        static object m_LockObj = new object();
        static bool m_Inited = false;
        internal static Dict.WordDictionary m_WordDictionary = null;
        internal static Dict.ChsName m_ChsName = null;
        internal static Dict.StopWord m_StopWord = null;
        internal static Dict.Synonym m_Synonym = null;
        internal static Hashtable m_KeywordStop = null;
        private MatchOptions m_Options;
        private MatchParameter m_Parameters;
        internal static string m_SegWorkPath = string.Empty;

        public cSeg()
        {
        }

        ~cSeg()
        {
        }

        #region 加载字典数据

        static private void LoadDictionary(string sPath)
        {
            m_WordDictionary = new Dict.WordDictionary();
            m_SegWorkPath = Application.StartupPath + "\\"; 
            string dictFile = sPath + "\\dict.dic";
            m_WordDictionary.Load(dictFile);

            m_ChsName = new Dict.ChsName();
            m_ChsName.LoadChsName(sPath);

            m_WordDictionary.ChineseName = m_ChsName;

            m_StopWord = new Dict.StopWord();
            m_StopWord.LoadStopwordsDict(sPath + "\\Stopword.txt");

            m_Synonym = new Dict.Synonym();

            //是否加载同义词词库

            //if ()
            //{
            //    _Synonym.Load(dir);
            //}

            //这是一个目录监控的后台监控器，以用于监控词库的变化，并定期进行词库信息的加载，可暂时不用

            //m_DictLoader = new Dict.DictionaryLoader(Setting.PanGuSettings.Config.GetDictionaryPath());
        }

        //private static void InitInfinitiveVerbTable()
        //{
        //    if (_InfinitiveVerbTable != null)
        //    {
        //        return;
        //    }

        //    _InfinitiveVerbTable = new Dictionary<string, string>();

        //    using (System.IO.StringReader sr = new System.IO.StringReader(AnalyzerResource.INFINITIVE))
        //    {

        //        string line = sr.ReadLine();

        //        while (!string.IsNullOrEmpty(line))
        //        {
        //            string[] strs = Framework.Regex.Split(line, "\t+");

        //            if (strs.Length != 3)
        //            {
        //                continue;
        //            }

        //            for (int i = 1; i < 3; i++)
        //            {
        //                string key = strs[i].ToLower().Trim();

        //                if (!_InfinitiveVerbTable.ContainsKey(key))
        //                {
        //                    _InfinitiveVerbTable.Add(key, strs[0].Trim().ToLower());
        //                }
        //            }

        //            line = sr.ReadLine();
        //        }
        //    }

        //}

        public static void Init()
        {
            Init(null);
        }

        /// <summary>
        /// 字典目录
        /// </summary>
        /// <param name="sPath"></param>
        public static void Init(string sPath)
        {
            
            lock (m_LockObj)
            {
                if (m_Inited==true)
                {
                    return;
                }

                //InitInfinitiveVerbTable();

                LoadDictionary(sPath);

                IniKeywordStop(sPath);

                m_Inited = true;
            }
        }

        public static void InsertWord(string word,double fre,POS pos)
        {
            m_WordDictionary.InsertWord(word, fre, pos);
        }

        //初始化关键词提取的停用词库
        private static void IniKeywordStop(string sPath)
        {
            string fileName = sPath + "\\KeywordStop.dic";
            if (!System.IO.File.Exists(fileName))
            {
                return;
            }

            FileStream myStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(myStream, System.Text.Encoding.UTF8);

            m_KeywordStop = new Hashtable();

            string strLine = "";
            while (strLine != null)
            {
                strLine = sr.ReadLine();
                if (strLine != null && strLine.Length > 0)
                {
                    if (!m_KeywordStop.Contains(strLine))
                    {
                        m_KeywordStop.Add(strLine, null);

                    }
                }
            }


            sr.Close();
            sr.Dispose();
            myStream.Close();
            myStream.Dispose();

        }

        #endregion

        #region 分词
        public List<WordSort> DoSegmentByFrequency(string text)
        {
            ICollection<WordInfo> words = DoSegment(text);

            List<WordSort> wSort = new List<WordSort>();

            foreach (WordInfo wordInfo in words)
            {
               
                WordSort w = new WordSort();
                w.Word = wordInfo.Word;
                w.WordCount = 1;
                w.Pos = wordInfo.Pos;
                w.Frequency = wordInfo.Frequency;

                if (wSort.Contains(w))
                {
                    int i = wSort.IndexOf(w);
                    wSort[i].WordCount++;
                }
                else
                {
                    wSort.Add(w);
                }
                w = null;
            
            }

            words = null;

            wSort.Sort();

            return wSort;
        }

        public List<WordSort> DoSegmentByFrequency(string text ,MatchOptions oprions)
        {
            ICollection<WordInfo> words = DoSegment(text, oprions);

            List<WordSort> wSort = new List<WordSort>();

            foreach (WordInfo wordInfo in words)
            {

                WordSort w = new WordSort();
                w.Word = wordInfo.Word;
                w.WordCount = 1;
                w.Pos = wordInfo.Pos;
                w.Frequency = wordInfo.Frequency;

                if (wSort.Contains(w))
                {
                    int i = wSort.IndexOf(w);
                    wSort[i].WordCount++;
                }
                else
                {
                    wSort.Add(w);
                }
                w = null;

            }

            words = null;

            wSort.Sort();

            return wSort;
        }

        public ICollection<WordInfo> DoSegment(string text)
        {
            MatchOptions options = new MatchOptions();
            options.ChineseNameIdentify = true;
            options.FrequencyFirst = true;
            options.FilterStopWords = true;
            //options.FilterEnglish = true;
            //options.FilterNumeric = false;
            //options.EnglishMultiDimensionality = true;
            //options.CustomRule = true;

            MatchParameter p = new MatchParameter();
            p.CustomRuleFullClassName = "SoMinerSegCustomRule.PickupCNEN";
            p.CustomRuleAssemblyFileName = "SoMinerSegCustomRule.dll";

            m_Parameters = p.Clone();

            return DoSegment(text, options);
        }

        public ICollection<WordInfo> DoSegment(string text, MatchOptions options)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new SuperLinkedList<WordInfo>();
            }

            try
            {
                m_Options = options;

                Init();

                SuperLinkedList<WordInfo> result = PreSegment(text);

                if (m_Options.FilterStopWords)
                {
                    FilterStopWord(result);
                }

                ProcessAfterSegment(text, result);

                return result;
            }
            finally
            {
                
            }
        }

        private SuperLinkedList<WordInfo> GetInitSegment(string text)
        {
            SuperLinkedList<WordInfo> result = new SuperLinkedList<WordInfo>();

            Lexical lexical = new Lexical(text);

            DFAResult dfaResult;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                dfaResult = lexical.Input(c, i);

                switch (dfaResult)
                {
                    case DFAResult.Continue:
                        continue;
                    case DFAResult.Quit:
                        result.AddLast(lexical.OutputToken);
                        break;
                    case DFAResult.ElseQuit:
                        result.AddLast(lexical.OutputToken);
                        if (lexical.OldState != 255)
                        {
                            i--;
                        }

                        break;
                }

            }

            dfaResult = lexical.Input(0, text.Length);

            switch (dfaResult)
            {
                case DFAResult.Continue:
                    break;
                case DFAResult.Quit:
                    result.AddLast(lexical.OutputToken);
                    break;
                case DFAResult.ElseQuit:
                    result.AddLast(lexical.OutputToken);
                    break;
            }

            return result;
        }

        private string ConvertChineseCapitalToAsiic(string text)
        {
            StringBuilder sb = null;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                bool needReplace = false;

                //[０-９\d]+)|([ａ-ｚＡ-Ｚa-zA-Z_]+)";
                if (c >= '０' && text[i] <= '９')
                {
                    c -= '０';
                    c += '0';
                    needReplace = true;
                }
                else if (c >= 'ａ' && text[i] <= 'ｚ')
                {
                    c -= 'ａ';
                    c += 'a';
                    needReplace = true;
                }
                else if (c >= 'Ａ' && text[i] <= 'Ｚ')
                {
                    c -= 'Ａ';
                    c += 'A';
                    needReplace = true;
                }

                if (needReplace)
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder();
                        sb.Append(text.Substring(0, i));
                    }
                }

                if (sb != null)
                {
                    sb.Append(c);
                }

            }

            if (sb == null)
            {
                return text;
            }
            else
            {
                return sb.ToString();
            }
        }

        //private string GetStem(string word)
        //{
        //    string stem;
        //    if (_InfinitiveVerbTable.TryGetValue(word, out stem))
        //    {
        //        return stem;
        //    }

        //    porter.Stemmer s = new porter.Stemmer();

        //    foreach (char ch in word)
        //    {
        //        if (char.IsLetter((char)ch))
        //        {
        //            s.add(ch);
        //        }
        //    }

        //    s.stem();

        //    return s.ToString();

        //}

        private SuperLinkedList<WordInfo> PreSegment(String text)
        {
            SuperLinkedList<WordInfo> result = GetInitSegment(text);

            SuperLinkedListNode<WordInfo> cur = result.First;

            while (cur != null)
            {
                if (m_Options.IgnoreSpace)
                {
                    if (cur.Value.WordType == WordType.Space)
                    {
                        SuperLinkedListNode<WordInfo> lst = cur;
                        cur = cur.Next;
                        result.Remove(lst);
                        continue;
                    }
                }

                switch (cur.Value.WordType)
                {
                    case WordType.SimplifiedChinese:

                        string inputText = cur.Value.Word;

                        WordType originalWordType = WordType.SimplifiedChinese;

                        if (m_Options.TraditionalChineseEnabled)
                        {
                            string simplified = Microsoft.VisualBasic.Strings.StrConv(cur.Value.Word, Microsoft.VisualBasic.VbStrConv.SimplifiedChinese, 0);

                            if (simplified != cur.Value.Word)
                            {
                                originalWordType = WordType.TraditionalChinese;
                                inputText = simplified;
                            }
                        }

                        AppendList<Dict.PositionLength> pls = m_WordDictionary.GetAllMatchs(inputText, m_Options.ChineseNameIdentify);

                        ChsFullTextMatch chsMatch = new ChsFullTextMatch(m_WordDictionary);
                        chsMatch.Options = m_Options;

                        SuperLinkedList<WordInfo> chsMatchWords = chsMatch.Match(pls.Items, cur.Value.Word, pls.Count);

                        SuperLinkedListNode<WordInfo> curChsMatch = chsMatchWords.First;
                        while (curChsMatch != null)
                        {
                            WordInfo wi = curChsMatch.Value;

                            wi.Position += cur.Value.Position;
                            wi.OriginalWordType = originalWordType;
                            wi.WordType = originalWordType;

                            if (m_Options.OutputSimplifiedTraditional)
                            {
                                if (m_Options.TraditionalChineseEnabled)
                                {
                                    string newWord;
                                    WordType wt;

                                    if (originalWordType == WordType.SimplifiedChinese)
                                    {
                                        newWord = Microsoft.VisualBasic.Strings.StrConv(wi.Word,
                                            Microsoft.VisualBasic.VbStrConv.TraditionalChinese, 0);
                                        wt = WordType.TraditionalChinese;
                                    }
                                    else
                                    {
                                        newWord = Microsoft.VisualBasic.Strings.StrConv(wi.Word,
                                            Microsoft.VisualBasic.VbStrConv.SimplifiedChinese, 0);
                                        wt = WordType.SimplifiedChinese;
                                    }

                                    if (newWord != wi.Word)
                                    {
                                        WordInfo newWordInfo = new WordInfo(wi);
                                        newWordInfo.Word = newWord;
                                        newWordInfo.OriginalWordType = originalWordType;
                                        newWordInfo.WordType = wt;
                                        newWordInfo.Rank = 1;
                                        newWordInfo.Position = wi.Position;
                                        chsMatchWords.AddBefore(curChsMatch, newWordInfo);
                                    }
                                }
                            }

                            curChsMatch = curChsMatch.Next;
                        }

                        SuperLinkedListNode<WordInfo> lst = result.AddAfter(cur, chsMatchWords);
                        SuperLinkedListNode<WordInfo> removeItem = cur;
                        cur = lst.Next;
                        result.Remove(removeItem);
                        break;
                    case WordType.English:
                        cur.Value.Rank = 5;
                        List<string> output;
                        cur.Value.Word = ConvertChineseCapitalToAsiic(cur.Value.Word);

                        if (m_Options.IgnoreCapital)
                        {
                            cur.Value.Word = cur.Value.Word.ToLower();
                        }

                        if (m_Options.EnglishSegment)
                        {
                            string lower = cur.Value.Word.ToLower();

                            if (lower != cur.Value.Word)
                            {
                                result.AddBefore(cur, new WordInfo(lower, cur.Value.Position, POS.POS_A_NX, 1,
                                    5, WordType.English, WordType.English,0));
                            }

                            string stem = lower.ToLower();

                            if (!string.IsNullOrEmpty(stem))
                            {
                                if (lower != stem)
                                {
                                    result.AddBefore(cur, new WordInfo(stem, cur.Value.Position, POS.POS_A_NX, 1,
                                        5, WordType.English, WordType.English,0));
                                }
                            }
                        }

                        if (m_Options.EnglishMultiDimensionality)
                        {
                            bool needSplit = false;

                            foreach (char c in cur.Value.Word)
                            {
                                if ((c >= '0' && c <= '9') || (c == '_'))
                                {
                                    needSplit = true;
                                    break;
                                }
                            }

                            if (needSplit)
                            {
                                if (cTool.GetMatchStrings(cur.Value.Word, PATTERNS, true, out output))
                                {
                                    int outputCount = 0;

                                    foreach (string str in output)
                                    {
                                        if (!string.IsNullOrEmpty(str))
                                        {
                                            outputCount++;

                                            if (outputCount > 1)
                                            {
                                                break;
                                            }
                                        }
                                    }


                                    if (outputCount > 1)
                                    {
                                        int position = cur.Value.Position;

                                        foreach (string splitWord in output)
                                        {
                                            if (string.IsNullOrEmpty(splitWord))
                                            {
                                                continue;
                                            }

                                            WordInfo wi;

                                            if (splitWord[0] >= '0' && splitWord[0] <= '9')
                                            {
                                                wi = new WordInfo(splitWord, POS.POS_A_M, 1);
                                                wi.Position = position;
                                                wi.Rank = 1;
                                                wi.OriginalWordType = WordType.English;
                                                wi.WordType = WordType.Numeric;
                                            }
                                            else
                                            {
                                                wi = new WordInfo(splitWord, POS.POS_A_NX, 1);
                                                wi.Position = position;
                                                wi.Rank = 5;
                                                wi.OriginalWordType = WordType.English;
                                                wi.WordType = WordType.English;
                                            }

                                            result.AddBefore(cur, wi);
                                            position += splitWord.Length;
                                        }
                                    }
                                }
                            }
                        }

                        if (!MergeEnglishSpecialWord(text, result, ref cur))
                        {
                            cur = cur.Next;
                        }

                        break;
                    case WordType.Numeric:
                        cur.Value.Word = ConvertChineseCapitalToAsiic(cur.Value.Word);
                        cur.Value.Rank = 1;

                        if (!MergeEnglishSpecialWord(text, result, ref cur))
                        {
                            cur = cur.Next;
                        }

                        //cur = cur.Next;
                        break;
                    case WordType.Symbol:
                        cur.Value.Rank = 1;
                        cur = cur.Next;
                        break;
                    default:
                        cur = cur.Next;
                        break;
                }

            }


            return result;

        }

        private void FilterStopWord(SuperLinkedList<WordInfo> wordInfoList)
        {
            if (wordInfoList == null)
            {
                return;
            }

            SuperLinkedListNode<WordInfo> cur = wordInfoList.First;

            while (cur != null)
            {
                if (m_StopWord.IsStopWord(cur.Value.Word,
                    m_Options.FilterEnglish, 0,
                    m_Options.FilterNumeric,0))
                {
                    SuperLinkedListNode<WordInfo> removeItem = cur;
                    cur = cur.Next;
                    wordInfoList.Remove(removeItem);
                }
                else
                {
                    cur = cur.Next;
                }
            }
        }

        private void ProcessAfterSegment(string orginalText, SuperLinkedList<WordInfo> result)
        {
            //匹配同义词

            if (m_Options.SynonymOutput)
            {
                SuperLinkedListNode<WordInfo> node = result.First;

                while (node != null)
                {
                    List<string> synonyms = m_Synonym.GetSynonyms(node.Value.Word);

                    if (synonyms != null)
                    {
                        foreach (string word in synonyms)
                        {
                            node = result.AddAfter(node, new WordInfo(word, node.Value.Position,
                                node.Value.Pos, node.Value.Frequency, 1,
                                WordType.Synonym, node.Value.WordType,0));
                        }
                    }

                    node = node.Next;
                }
            }
            

            //用户自定义规则

            if (m_Options.CustomRule)
            {
                string customRuleFile = m_SegWorkPath + "\\" + m_Parameters.CustomRuleAssemblyFileName;
                ICustomRule rule = CustomRule.GetCustomRule(customRuleFile, m_Parameters.CustomRuleFullClassName);

                if (rule != null)
                {
                    rule.Text = orginalText;
                    rule.AfterSegment(result);
                }

            }
        }
        #endregion

        public void ReIniCnEn()
        {
            MatchParameter p = new MatchParameter();
            p.CustomRuleFullClassName = "SoMinerSegCustomRule.PickupCNEN";
            p.CustomRuleAssemblyFileName = "SoMinerSegCustomRule.dll";

            string customRuleFile = m_SegWorkPath + "\\" + p.CustomRuleAssemblyFileName;
            ICustomRule rule = CustomRule.GetCustomRule(customRuleFile, p.CustomRuleFullClassName);

            if (rule != null)
            {
                rule.Text = "sominerReIni";
            }
        }

        /// <summary>
        /// 是否为停用的关键词：true-是；false-否；
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool IsStopKeyword(string word)
        {
            word = word.Trim();
            return m_KeywordStop.ContainsKey(word);
        }

        /// <summary>
        /// 词库中是否包含指定的词汇
        /// </summary>
        /// <param name="word"></param>
        /// <returns>true-包含；false-不包含</returns>
        public bool IsIncludeWord(string word)
        {
            word = word.Trim();
            List<SearchWordResult> sResult = m_WordDictionary.Search(word);
            if (sResult == null || sResult.Count ==0)
                return false;
            else
                return true;
        }

        private bool MergeEnglishSpecialWord(string orginalText, SuperLinkedList<WordInfo> wordInfoList, ref SuperLinkedListNode<WordInfo> current)
        {
            SuperLinkedListNode<WordInfo> cur = current;

            cur = cur.Next;

            int last = -1;

            while (cur != null)
            {
                if (cur.Value.WordType == WordType.Symbol || cur.Value.WordType == WordType.English)
                {
                    last = cur.Value.Position + cur.Value.Word.Length;
                    cur = cur.Next;
                }
                else
                {
                    break;
                }
            }


            if (last >= 0)
            {
                int first = current.Value.Position;

                string newWord = orginalText.Substring(first, last - first);

                WordAttribute wa = m_WordDictionary.GetWordAttr(newWord);

                if (wa == null)
                {
                    return false;
                }

                while (current != cur)
                {
                    SuperLinkedListNode<WordInfo> removeItem = current;
                    current = current.Next;
                    wordInfoList.Remove(removeItem);
                }

                WordInfo newWordInfo = new WordInfo(new Dict.PositionLength(first, last - first,
                    wa), orginalText);

                newWordInfo.WordType = WordType.English;
                newWordInfo.Rank =1;

                if (current == null)
                {
                    wordInfoList.AddLast(newWordInfo);
                }
                else
                {
                    wordInfoList.AddBefore(current, newWordInfo);
                }

                return true;
            }


            return false;

        }
    }
}

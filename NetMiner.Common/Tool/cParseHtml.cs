using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data ;
using System.IO;
using System.Web;
using System.Net;

///��ҳ������
namespace NetMiner.Common.Tool
{
    class cParseHtml
    {
        public cParseHtml()
        {

        }

        ~cParseHtml()
        {

        }

        //��ȡ��ҳtitile
        public string GetHtmlTitle(string HtmlSource)
        {
            string Splitstr = "(?<=<title>).*(?=</title>)";

            if (HtmlSource == "")
                return "";

            Match aa = Regex.Match(HtmlSource, Splitstr, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string Title = aa.Groups[0].ToString();

            return Title;
        }

        //��ȡ��ҳ���ı�
        public string GetFullText(string HtmlSource)
        {
            string instr = HtmlSource;

            string m_outstr;

            m_outstr = instr.Clone() as string;
            m_outstr = new Regex(@"(?m)<script[^>]*>(\w|\W)*?</script[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(m_outstr, "");
            m_outstr = new Regex(@"(?m)<style[^>]*>(\w|\W)*?</style[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(m_outstr, "");
            m_outstr = new Regex(@"(?m)<select[^>]*>(\w|\W)*?</select[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(m_outstr, "");
            //if (!withLink)
            //    m_outstr = new Regex(@"(?m)<a[^>]*>(\w|\W)*?</a[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(m_outstr, "");
            Regex objReg = new System.Text.RegularExpressions.Regex("(<[^>]+?>)|&nbsp;", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg.Replace(m_outstr, "");
            Regex objReg2 = new System.Text.RegularExpressions.Regex("(\\s)+", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg2.Replace(m_outstr, " ");

            return m_outstr;
        }

        //��ȡ��ҳ�еĵ����ʼ�
        public DataTable GetEmail(string HtmlSource)
        {

            if (HtmlSource == "")
                return null;

            Regex re = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",RegexOptions.IgnoreCase | RegexOptions.Multiline );
            MatchCollection mc = re.Matches(HtmlSource);

            DataTable t = new DataTable();
            t.Columns.Add("Email");

            foreach (Match ma in mc)
            {
                t.Rows.Add(ma.ToString());
            }

            return t;

        }
       
        public int CompareDinosByChineseLength(string x, string y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    Regex r = new Regex("[\u4e00-\u9fa5]");
                    float xCount = (float)(r.Matches(x).Count) / (float)x.Length;
                    float yCount = (float)(r.Matches(y).Count) / (float)y.Length;

                    int retval = xCount.CompareTo(yCount);

                    if (retval != 0)
                    {
                        return retval;
                    }
                    else
                    {
                        return x.CompareTo(y);
                    }
                }
            }
        }

        ///<summary>
        /// ��ȡһ����ҳԴ���еı�ǩ�б�֧��Ƕ�ף�һ���ȥdiv��td������
        ///</summary>
        ///<param name="input"></param>
        ///<param name="tag"></param>
        ///<returns></returns>
        public List<string> GetTags(string input, string tag)
        {
            StringReader strReader = new StringReader(input);
            int lowerThanCharCounter = 0;
            int lowerThanCharPos = 0;
            Stack<int> tagPos = new Stack<int>();
            List<string> taglist = new List<string>();
            int i = 0;
            while (true)
            {
                try
                {
                    int intCharacter = strReader.Read();
                    if (intCharacter == -1) break;

                    char convertedCharacter = Convert.ToChar(intCharacter);

                    if (lowerThanCharCounter > 0)
                    {
                        if (convertedCharacter == '>')
                        {
                            lowerThanCharCounter--;

                            string biaoqian = input.Substring(lowerThanCharPos, i - lowerThanCharPos + 1);
                            if (biaoqian.StartsWith(string.Format("<{0}", tag)))
                            {
                                tagPos.Push(lowerThanCharPos);
                            }
                            if (biaoqian.StartsWith(string.Format("</{0}", tag)))
                            {
                                if (tagPos.Count < 1)
                                    continue;
                                int tempTagPos = tagPos.Pop();
                                string strdiv = input.Substring(tempTagPos, i - tempTagPos + 1);
                                taglist.Add(strdiv);
                            }
                        }
                    }

                    if (convertedCharacter == '<')
                    {
                        lowerThanCharCounter++;
                        lowerThanCharPos = i;
                    }
                }
                finally
                {
                    i++;
                }
            }
            return taglist;
        }

        ///<summary>
        /// ��ȡָ����ҳ��Դ�룬֧�ֱ����Զ�ʶ��
        ///</summary>
        ///<param name="url"></param>
        ///<returns></returns>
        private string getDataFromUrl(string url)
        {
            string str = string.Empty;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            //����httpͷ
            request.AllowAutoRedirect = true;
            request.AllowWriteStreamBuffering = true;
            request.Referer = "";
            request.Timeout = 10 * 1000;
            request.UserAgent = "";

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //����httpӦ���httpͷ���жϱ���
                    string characterSet = response.CharacterSet;
                    Encoding encode;
                    if (characterSet != "")
                    {
                        if (characterSet == "ISO-8859-1")
                        {
                            characterSet = "gb2312";
                        }
                        encode = Encoding.GetEncoding(characterSet);
                    }
                    else
                    {
                        encode = Encoding.Default;
                    }

                    //����һ���ڴ���������httpӦ����
                    Stream receiveStream = response.GetResponseStream();
                    MemoryStream mStream = new MemoryStream();

                    byte[] bf = new byte[255];
                    int count = receiveStream.Read(bf, 0, 255);
                    while (count > 0)
                    {
                        mStream.Write(bf, 0, count);
                        count = receiveStream.Read(bf, 0, 255);
                    }
                    receiveStream.Close();

                    mStream.Seek(0, SeekOrigin.Begin);

                    //���ڴ������ȡ�ַ���
                    StreamReader reader = new StreamReader(mStream, encode);
                    char[] buffer = new char[1024];
                    count = reader.Read(buffer, 0, 1024);
                    while (count > 0)
                    {
                        str += new String(buffer, 0, count);
                        count = reader.Read(buffer, 0, 1024);
                    }

                    //�ӽ��������ַ������ж�charset�������httpӦ��ı��벻һֱ
                    //��ô��ҳ��������Ϊ׼���ٴδ��ڴ��������¶�ȡ�ı�
                    Regex reg =
                        new Regex(@"<meta[\s\S]+?charset=(.*)""[\s\S]+?>",
                                  RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    MatchCollection mc = reg.Matches(str);
                    if (mc.Count > 0)
                    {
                        string tempCharSet = mc[0].Result("$1");
                        if (string.Compare(tempCharSet, characterSet, true) != 0)
                        {
                            encode = Encoding.GetEncoding(tempCharSet);
                            str = string.Empty;
                            mStream.Seek(0, SeekOrigin.Begin);
                            reader = new StreamReader(mStream, encode);
                            buffer = new char[255];
                            count = reader.Read(buffer, 0, 255);
                            while (count > 0)
                            {
                                str += new String(buffer, 0, count);
                                count = reader.Read(buffer, 0, 255);
                            }
                        }
                    }
                    reader.Close();
                    mStream.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
            return str;
        }

        ///<summary>
        /// ��һ����ҳԴ���л�ȡ����
        ///</summary>
        ///<param name="input"></param>
        ///<returns></returns>
        public string GetMainContent(string input)
        {
            string reg1 = @"<(p|br)[^<]*>";
            string reg2 =
                @"(\[([^=]*)(=[^\]]*)?\][\s\S]*?\[/\1\])|(?<lj>(?=[^\u4E00-\u9FA5\uFE30-\uFFA0,."");])<a\s+[^>]*>[^<]{2,}</a>(?=[^\u4E00-\u9FA5\uFE30-\uFFA0,."");]))|(?<Style><style[\s\S]+?/style>)|(?<select><select[\s\S]+?/select>)|(?<Script><script[\s\S]*?/script>)|(?<Explein><\!\-\-[\s\S]*?\-\->)|(?<li><li(\s+[^>]+)?>[\s\S]*?/li>)|(?<Html></?\s*[^> ]+(\s*[^=>]+?=['""]?[^""']+?['""]?)*?[^\[<]*>)|(?<Other>&[a-zA-Z]+;)|(?<Other2>\#[a-z0-9]{6})|(?<Space>\s+)|(\&\#\d+\;)";

            //1����ȡ��ҳ������div��ǩ
            List<string> list = GetTags(input, "div");

            //2��ȥ����������200�ֵ�div
            List<string> needToRemove = new List<string>();
            foreach (string s in list)
            {
                Regex r = new Regex("[\u4e00-\u9fa5]");
                if (r.Matches(s).Count < 300)
                {
                    needToRemove.Add(s);
                }
            }
            foreach (string s in needToRemove)
            {
                list.Remove(s);
            }

            //3����ʣ�µ�div�����ֱ������ٵ�������,
            list.Sort(CompareDinosByChineseLength);
            if (list.Count < 1)
            {
                return "";
            }
            input = list[list.Count - 1];

            //4����p��br�滻�������ռλ��[p][br]
            input = new Regex(reg1, RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "[$1]");

            //5��ȥ��HTML��ǩ����������
            input = new Regex(reg2, RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "");

            //6��������ռά���滻�ɻس��ͻ���
            input = new Regex("\\[p]", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "\r\n ");
            input = new Regex("\\[br]", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "\r\n");
            return input;
        }

        public string GetArticlePublishDate(string input)
        {
            string mDate = @"\d\d\d\d[-|��]\d\d[-|��]\d\d(��)?[\s]+?\d\d[:]\d\d([:]\d\d)?";

            Match charSetMatch = Regex.Match(input, mDate, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string strDate = charSetMatch.Groups[0].ToString();

            return strDate;
        }

        public string GetArticleSource(string input)
        {
            string mDate = @"��Դ[^<>]+?((<a).+?(>))?(?<Source>[^<>\s]+)(</a>)?";

            Match charSetMatch = Regex.Match(input, mDate, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string strSource = charSetMatch.Groups["Source"].ToString();

            return strSource;
        }
    }
}

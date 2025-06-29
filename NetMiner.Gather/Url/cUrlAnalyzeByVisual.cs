using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NetMiner.Visual.Url
{
    public class cUrlAnalyzeByVisual
    {
        public cUrlAnalyzeByVisual()
        {

        }

        ~cUrlAnalyzeByVisual()
        {

        }

        /// <summary>
        /// 根据网页文档获取地址，XPath调用模式
        /// </summary>
        /// <param name="inputUrl">当前网页文档的Url</param>
        /// <param name="gDoc">网页文档</param>
        /// <param name="gType">获取地址的类型</param>
        /// <param name="XPath">XPath信息</param>
        /// <param name="loopIndex">页面滚动次数，因为只有在页面滚动的情况下，当前的网页文档才会发生变化</param>
        /// <returns></returns>
        public string[] GetUrls(string inputUrl, GeckoDocument gDoc, string XPath, int pageScrollIndex)
        {
            string[] urls = null;
            string ss = string.Empty;
            List<string> tmpUrls = new List<string>();
            string tmpXpath = string.Empty;
            string str = string.Empty;

            if (Regex.IsMatch(XPath, "{Num:\\d+,\\d+,\\d+}"))
            {

                //获取第一个数字参数
                if (pageScrollIndex > 1)
                {
                    Match charSetMatch = Regex.Match(XPath, "{Num:\\d+,\\d+,\\d+}", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string numPara = charSetMatch.Groups[0].ToString();

                    //根据页面滚动的次数来重新进行数字参数的值修改
                    string s = numPara.Replace("{Num:", "").Replace("}", "");
                    string[] sss = s.Split(',');
                    if (sss.Length == 3)
                    {
                        //分解争取，开始处理
                        sss[0] = (int.Parse(sss[0]) + int.Parse(sss[1]) * (pageScrollIndex - 1)).ToString();
                        sss[1] = (int.Parse(sss[1]) * (pageScrollIndex)).ToString();
                        XPath = XPath.Replace(numPara, "{Num:" + sss[0] + "," + sss[1] + "," + sss[2] + "}");
                    }
                }

                cUrlParse cu = new cUrlParse(this.m_workPath);
                List<string> rules = cu.SplitWebUrl(XPath);
                cu = null;

                for (int i = 0; i < rules.Count; i++)
                {
                    tmpXpath = rules[i];

                    str = Regex.Replace(tmpXpath, "\\[\\d*\\]", "", RegexOptions.IgnoreCase);
                    if (str.EndsWith("img", StringComparison.CurrentCultureIgnoreCase))
                    {
                        tmpXpath = tmpXpath.Substring(0, tmpXpath.LastIndexOf("/"));
                    }

                    ss = FindByxPath(gDoc, tmpXpath);
                    if (!string.IsNullOrEmpty(ss))
                    {
                        ss = GetUrlsByRule(inputUrl, ss);
                        tmpUrls.Add(ss);
                    }
                }
                urls = tmpUrls.ToArray();

            }
            else
            {
                str = Regex.Replace(XPath, "\\[\\d*\\]", "", RegexOptions.IgnoreCase);
                if (str.EndsWith("img", StringComparison.CurrentCultureIgnoreCase))
                {
                    XPath = XPath.Substring(0, XPath.LastIndexOf("/"));
                }

                ss = FindByxPath(gDoc, XPath);
                urls = new string[1];
                urls[0] = GetUrlsByRule(inputUrl, ss);

            }



            return urls;
        }



        /// <summary>
        /// 根据XPath查找网址，从5.5开始，采用了Gecko浏览器
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string FindByxPath(GeckoDocument gDoc, string xPath)
        {

            IEnumerable<GeckoNode> nodes = gDoc.EvaluateXPath(xPath).GetNodes();

            string ss = string.Empty;

            foreach (GeckoNode node in nodes)
            {
                ss = ((Gecko.GeckoHtmlElement)(node)).OuterHtml;
            }

            return ss;

        }
    }
}

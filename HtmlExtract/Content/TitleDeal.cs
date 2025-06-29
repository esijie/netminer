using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using HtmlExtract.Entities;
using System.Linq;

namespace HtmlExtract.Content
{
    /// <summary>
    /// 标题
    /// </summary>
    public class TitleDeal : IDeal<TitlePart>
    {
        private string _sourceTitle = string.Empty;
        private string[] _dealTitleTags = new string[] { "div", "td", "h1", "h2", "h3","p" };
        private List<TitlePart> _titles = new List<TitlePart>();

        public TitleDeal()
        {
        }

        ~TitleDeal()
        {
            _titles = null;
        }

        #region 属性
        /// <summary>
        /// 源标题
        /// </summary>
        public string SourceTitle
        {
            get { return _sourceTitle; }
            set { _sourceTitle = value; }
        }
        /// <summary>
        /// 需要处理的标签(标题)
        /// </summary>
        public string[] DealTitleTags
        {
            get { return _dealTitleTags; }
            set { _dealTitleTags = value; }
        }
        /// <summary>
        /// 标题列表
        /// </summary>
        public List<TitlePart> Titles
        {
            get { return _titles; }
            set { _titles = value; }
        }
        #endregion

        #region 接口
        /// <summary>
        /// 标题处理
        /// </summary>
        /// <param name="hnode"></param>
        public void Process(HtmlFragment htmlFragment)
        {
            //不在处理标签内，返回
            if (!DealTitleTags.Contains(htmlFragment.TagName))
            {
                return;
            }
            //字符数等于0或者大于100的，返回
            if (htmlFragment.CharsCount == 0 || htmlFragment.CharsCount > 150)
            {
                return;
            }
            //定义标题对象
            TitlePart title = new TitlePart(htmlFragment.Sequence, htmlFragment);

            //节点是h标签的，加权
            if (htmlFragment.TagName == "h1")
            {
                title.Power += 3;
            }
            if ( htmlFragment.TagName == "h2" || htmlFragment.TagName == "h3")
            {
                title.Power += 2;
            }
            //加权操作
            title.Power += TagPower(htmlFragment.HNode.InnerHtml);
            title.Power += NodeClassPower(htmlFragment.HNode);
            title.Power += CharsCountPower(htmlFragment.CharsCount,htmlFragment.CNCharsCount );
            title.Power += SourceTitlePower(SourceTitle, htmlFragment.HNode.InnerText);
            //权值必须大于2，才进入候选队列
            if (title.Power > 2)
            {
                Titles.Add(title);
            }

        }
        /// <summary>
        /// 递归完成后的处理(标题)
        /// </summary>
        public void ProcessEnd(params object[] args)
        {
            
        }
        /// <summary>
        /// 获取标题
        /// </summary>
        /// <returns></returns>
        public TitlePart GetModel()
        {
            if (Titles == null || Titles.Count == 0)
            {
                return null;
            }
            //排序规则：权值降序，字符数升序
            TitlePart title = Titles.Where(t => t.Power > 2).OrderByDescending(t => t.Power).ThenBy(t => t.CharsCount).FirstOrDefault();
            return title;
        }
        #endregion

        #region 公共
        /// <summary>
        /// 对源标题处理
        /// </summary>
        /// <param name="sourceTitle"></param>
        public string getTitle(string sourceTitle)
        {
            if (string.IsNullOrEmpty(sourceTitle))
            {
                return string.Empty;
            }
            string[] titles = sourceTitle.Split(new char[] { '-', '_', '|' });
            return titles[0].TrimStart().TrimEnd();
        }
        #endregion

        #region 加权 
        /// <summary>
        /// 标签权限
        /// </summary>
        /// <param name="sourceHtml"></param>
        /// <returns></returns>
        private double TagPower(string sourceHtml)
        {
            if (string.IsNullOrEmpty(sourceHtml))
            {
                return 0;
            }
            string pattern = @"(?i)(<h1>|<h2>|<h3>|<h4>)";
            string pattern2 = @"(?i)(<b>|<strong>)";
            //string sourceLower=sourceHtml;
            if (Regex.Match(sourceHtml, pattern).Success)
            {
                return 2;
            }
            else if (Regex.Match(sourceHtml, pattern2).Success)
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// 节点样式权限
        /// </summary>
        /// <param name="hnode"></param>
        /// <returns></returns>
        private double NodeClassPower(HtmlNode hnode)
        {
            HtmlAttribute nodeClass = hnode.Attributes["class"];
            if (nodeClass==null)
            {
                return 0;
            }
            string classValue = nodeClass.Value.ToLower();
            if (classValue.Contains("title") || classValue.Contains("head") || classValue.Contains("biaoti"))
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// 总文字数权限
        /// </summary>
        /// <param name="charsCount"></param>
        /// <returns></returns>
        private double CharsCountPower(int charsCount,int CNCharsCount)
        {
            if (CNCharsCount == 0)
                return 0;

            if (charsCount >= 3 && charsCount <= 50)
            {
                if (CNCharsCount > 4 && CNCharsCount < 20)
                    return 2;
                else
                    return 1;
            }
            return 0;
        }

        /// <summary>
        /// 源标题匹配权限
        /// </summary>
        /// <param name="sourceTitle"></param>
        /// <param name="sourceText">源文本，不包含html标签</param>
        /// <returns></returns>
        private double SourceTitlePower(string sourceTitle, string sourceText)
        {
            if (string.IsNullOrEmpty(sourceTitle) || string.IsNullOrEmpty(sourceText))
            {
                return 0;
            }
            sourceTitle=sourceTitle.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            sourceText=sourceText.Replace("\r", "").Replace("\n", "").Replace("\t", "").TrimStart().TrimEnd();

            if (sourceTitle == sourceText)//页面标题与节点内容匹配
            {
                return 5;
            }
            if (sourceTitle.Contains(sourceText))//页面标题包含节点内容
            {
                return 4;
            }
            if(sourceText.Contains(sourceTitle))//节点内容包含标题
            {
                return 3;
            }
            return 0;
        }
        #endregion
    }
}

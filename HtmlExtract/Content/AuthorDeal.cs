using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HtmlExtract.Entities;
using HtmlAgilityPack;
using System.Linq;

namespace HtmlExtract.Content
{
    public class AuthorDeal: IDeal<AuthorPart>
    {
        private string[] _dealAuthorTags = new string[] { "div", "td", "span", "li", "h1", "h2", "h3", "p", "strong" };
        private List<AuthorPart> _Authors = new List<AuthorPart>();

        public AuthorDeal()
        {
        }

        ~AuthorDeal()
        {
            _Authors = null;
        }

        #region 属性
        /// <summary>
        /// 需要处理的标签(发表时间)
        /// </summary>
        public string[] DealAuthorTags
        {
            get { return _dealAuthorTags; }
            set { _dealAuthorTags = value; }
        }
        /// <summary>
        /// 作者列表
        /// </summary>
        public List<AuthorPart> Authors
        {
            get { return _Authors; }
            set { _Authors = value; }
        }
        #endregion

        #region 接口
        /// <summary>
        /// 发表时间处理，入口
        /// </summary>
        /// <param name="hnode"></param>
        public void Process(HtmlFragment htmlFragment)
        {
            //不在处理标签内，返回
            if (!DealAuthorTags.Contains(htmlFragment.TagName))
            {
                return;
            }

            //定义发表时间对象
            AuthorPart author = new AuthorPart(htmlFragment.Sequence, htmlFragment);

            //加权操作
            author.Power += CharsCountPower(htmlFragment);
            author.Power += IncludeWordsPower(htmlFragment.HNode.InnerHtml);
            author.Power += TagPower(htmlFragment.HNode.InnerHtml);
            author.Power += NodeClassPower(htmlFragment.HNode);
            Authors.Add(author);
        }
      
        /// <summary>
        /// 递归完成后的处理(发表时间)
        /// </summary>
        public void ProcessEnd(params object[] args)
        {
            
        }
        /// <summary>
        /// 获取发表时间
        /// </summary>
        /// <returns></returns>
        public AuthorPart GetModel()
        {
            if (Authors == null || Authors.Count == 0)
            {
                return null;
            }
            //排序规则：权值降序，序列号升序
            AuthorPart author = Authors.OrderByDescending(t => t.Power).ThenBy(t => t.Sequence).FirstOrDefault();
            return author;
        }
        #endregion

        #region 加权

        /// <summary>
        /// 文字总数权限
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
       private double CharsCountPower(HtmlFragment HF)
        {
            if (HF.CharsCount > 30)
                return 0;
            else if (HF.CharsCount > 20 && HF.CharsCount <= 30)
                return 1;
            else if (HF.CharsCount > 10 && HF.CharsCount <= 20)
                return 2;
            else if (HF.CharsCount > 4 && HF.CharsCount <= 10)
                return 3;
            return 0;
        }
        /// <summary>
        /// 包含关键字权限
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
       private double IncludeWordsPower(string sourceHtml)
        {
            if (string.IsNullOrEmpty(sourceHtml))
            {
                return 0;
            }
            string pattern = @"(?i)(作者|楼主|责任编辑|发布人|发布者|在线|online|编辑)";
            string pattern1 = @"(?i)(发布)";
            if (Regex.Match(sourceHtml, pattern).Success)
            {
                return 2;
            }
            else if (Regex.Match(sourceHtml, pattern1).Success)
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
           if (nodeClass == null)
           {
               return 0;
           }
           string classValue = nodeClass.Value.ToLower();
           if (classValue.Contains("uid") || classValue.Contains("name") || classValue.Contains("author") || classValue.Contains("poster"))
           {
               return 1;
           }

           return 0;
       }

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
           string pattern2 = @"(?i)(<li>|<strong>|<p>|<span>)";
           string pattern1 = @"(?i)(|poster|author)";
           //string sourceLower=sourceHtml;
           if (Regex.Match(sourceHtml, pattern1).Success)
           {
               return 1.5;
           }
           else if (Regex.Match(sourceHtml, pattern2).Success)
           {
               return 1;
           }
           else if (Regex.Match(sourceHtml, pattern).Success)
           {
               return 0.5;
           }
           
           return 0;
       }
        #endregion
    }
}

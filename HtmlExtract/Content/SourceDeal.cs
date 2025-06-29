using System;
using System.Collections.Generic;
using System.Text;
using HtmlExtract.Entities;
using System.Text.RegularExpressions;
using System.Linq;
using HtmlAgilityPack;

namespace HtmlExtract.Content
{
    public class SourceDeal : IDeal<SourcePart>
    {
        private string[] _dealAuthorTags = new string[] { "div", "td", "span"};
        private List<SourcePart> _Source = new List<SourcePart>();

        public SourceDeal()
        {
        }

        ~SourceDeal()
        {
            _Source = null;
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
        public List<SourcePart> Sources
        {
            get { return _Source; }
            set { _Source = value; }
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
            SourcePart source = new SourcePart(htmlFragment.Sequence, htmlFragment);

            //加权操作
            source.Power += CharsCountPower(htmlFragment);
            source.Power += IncludeWordsPower(htmlFragment.HNode.InnerHtml);
            //source.Power += TagPower(htmlFragment.HNode.InnerHtml);
            source.Power += NodeClassPower(htmlFragment.HNode);
            Sources.Add(source);
        }

        private bool HasAuthor(string sourceHtml)
        {
            if (string.IsNullOrEmpty(sourceHtml))
            {
                return false;
            }
            string pattern = @"来源[:|：].+?\s";

            if (Regex.Match(sourceHtml, pattern).Success)
            {
                return true;
            }

            return false;
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
        public SourcePart GetModel()
        {
            if (Sources == null || Sources.Count == 0)
            {
                return null;
            }
            //排序规则：权值降序，序列号升序
            SourcePart author = Sources.OrderByDescending(t => t.Power).ThenBy(t => t.Sequence).FirstOrDefault();
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
            if (HF.CharsCount > 20)
                return 0;
            else if (HF.CharsCount > 10 && HF.CharsCount <= 20)
                return 1;
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
            string pattern = @"(?i)(来源)";
            string pattern1 = @"(?i)(\d{4}([^\da-zA-Z]{1})\d{1,2}([^\da-zA-Z]{1})\d{1,2})";
            if (Regex.Match(sourceHtml, pattern).Success)
            {
                return 4;
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
           if (classValue.Contains("uid") || classValue.Contains("name") || classValue.Contains("source") || classValue.Contains("laiyuan"))
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

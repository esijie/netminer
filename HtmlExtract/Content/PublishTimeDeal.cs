using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HtmlExtract.Entities;
using System.Linq;

namespace HtmlExtract.Content
{
    /// <summary>
    /// 发表时间
    /// </summary>
    public class PublishTimeDeal : IDeal<PublishTimePart>
    {
        private string[] _dealPublishTimeTags = new string[] { "div", "td", "span" };
        private List<PublishTimePart> _publishTimes = new List<PublishTimePart>();

        public PublishTimeDeal()
        {
        }

        ~PublishTimeDeal()
        {
            _publishTimes = null;
        }

        #region 属性
        /// <summary>
        /// 需要处理的标签(发表时间)
        /// </summary>
        public string[] DealPublishTimeTags
        {
            get { return _dealPublishTimeTags; }
            set { _dealPublishTimeTags = value; }
        }
        /// <summary>
        /// 发表时间列表
        /// </summary>
        public List<PublishTimePart> PublishTimes
        {
            get { return _publishTimes; }
            set { _publishTimes = value; }
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
            if (!DealPublishTimeTags.Contains(htmlFragment.TagName))
            {
                return;
            }
            //没有包含日期字符串
            if (!HasDateTime(htmlFragment.HNode.InnerText))
            {
                return;
            }

            //定义发表时间对象
            PublishTimePart publishTime = new PublishTimePart(htmlFragment.Sequence, htmlFragment);

            //加权操作
            publishTime.Power += CharsCountPower(htmlFragment);
            publishTime.Power += IncludeWordsPower(htmlFragment.HNode.InnerHtml);

            PublishTimes.Add(publishTime);
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
        public PublishTimePart GetModel()
        {
            if (PublishTimes == null || PublishTimes.Count == 0)
            {
                return null;
            }
            //排序规则：权值降序，序列号升序
            PublishTimePart publishTime = PublishTimes.OrderByDescending(t => t.Power).ThenBy(t => t.Sequence).FirstOrDefault();
            return publishTime;
        }
        #endregion

        #region 公共
        /// <summary>
        /// 获取日期时间
        /// </summary>
        /// <param name="sourceHtml"></param>
        /// <returns></returns>
        public string GetDateTime(string sourceHtml)
        {
            if (string.IsNullOrEmpty(sourceHtml))
            {
                return string.Empty;
            }
            string pattern = @"(?i)(?<dateTime>\d{2,4}([^\da-zA-Z]{1})\d{1,2}([^\da-zA-Z]{1})\d{1,2}([\s|\S]{0,3})\d{1,2}:\d{1,2})";
            string pattern1 = @"(?i)(?<dateTime>\d{1,2}([^\da-zA-Z]{1})\d{1,2}([^\da-zA-Z]{1})\d{2,4}([\s|\S]))";
            string pattern2 = @"(?i)(?<dateTime>\d{2,4}([^\da-zA-Z]{1})\d{1,2}([^\da-zA-Z]{1})\d{1,2}([\s|\S]))";

            Match mc = Regex.Match(sourceHtml, pattern);
            if (mc.Success)
            {
                return mc.Groups["dateTime"].Value;
            }

            mc = Regex.Match(sourceHtml, pattern1);
            if (mc.Success)
            {
                return mc.Groups["dateTime"].Value;
            }

            mc = Regex.Match(sourceHtml, pattern2);
            if (mc.Success)
            {
                return mc.Groups["dateTime"].Value;
            }

           

            return string.Empty;
        }
        #endregion

        #region 加权
        /// <summary>
        /// 是否包含日期字符串
        /// </summary>
        /// <param name="sourceHtml"></param>
        /// <returns></returns>
       private bool HasDateTime(string sourceHtml)
        {
            if (string.IsNullOrEmpty(sourceHtml))
            {
                return false;
            }
            string pattern = @"(?i)(\d{4}([^\da-zA-Z]{1})\d{1,2}([^\da-zA-Z]{1})\d{1,2})";

            if (Regex.Match(sourceHtml, pattern).Success)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 文字总数权限
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
       private double CharsCountPower(HtmlFragment HF)
        {
            if (HF.CharsCount<50)
            {
                return 1;
            }
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
            string pattern = @"(?i)(来源|发表|发布)";
            if (Regex.Match(sourceHtml, pattern).Success)
            {
                return 2;
            }
            return 0;
        }
        #endregion
    }
}

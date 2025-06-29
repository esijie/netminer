using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HtmlExtract.Entities;
using HtmlAgilityPack;
using System.Linq;

namespace HtmlExtract.Content
{
    /// <summary>
    /// 主题分页
    /// </summary>
    public class SubjectPagerDeal : IDeal<SubjectPagerPart>
    {
        private string[] _dealSubjectPagerTags = new string[] { "div", "p", "span", "td" };
        private List<SubjectPagerPart> _subjectPagers = new List<SubjectPagerPart>();

        public SubjectPagerDeal()
        {
        }

        ~SubjectPagerDeal()
        {
            _subjectPagers = null;
        }

        #region 属性
        /// <summary>
        /// 需要处理的标签(主题)
        /// </summary>
        public string[] DealSubjectPagerTags
        {
            get { return _dealSubjectPagerTags; }
            set { _dealSubjectPagerTags = value; }
        }
        /// <summary>
        /// 主题列表
        /// </summary>
        public List<SubjectPagerPart> SubjectPagers
        {
            get { return _subjectPagers; }
            set { _subjectPagers = value; }
        }
        #endregion

        #region 接口
        /// <summary>
        /// 内容处理，入口
        /// </summary>
        /// <param name="hnode"></param>
        public void Process(HtmlFragment htmlFragment)
        {
            if (!DealSubjectPagerTags.Contains(htmlFragment.TagName))
            {
                return;
            }
            if (htmlFragment.CharsCount > 100 || htmlFragment.LinkCount > 30 || htmlFragment.LinkCharsAvg > 5)
            {
                return;
            }
            string pattern = @"(?i)(页|\d+)";
            if (!Regex.Match(htmlFragment.HNode.InnerText, pattern).Success)
            {
                return;
            }

            SubjectPagerPart pager = new SubjectPagerPart(htmlFragment.Sequence, htmlFragment);

            pager.Power += NodeClassPower(htmlFragment.HNode);
            pager.Power += LinkDensityPower(htmlFragment);
            pager.Power += LinkCharsAvgPower(htmlFragment);
            pager.Power += LinkFeature(htmlFragment);
            pager.Power += TagDensityPower(htmlFragment);
            pager.Power += IncludeWordsPower(htmlFragment);
            pager.Power += ExcludeWordsPower(htmlFragment.HtmlSource);

            if (pager.Power > 3)
            {
                SubjectPagers.Add(pager);
            }
        }
        /// <summary>
        /// 递归完成后的处理(主题分页)
        /// </summary>
        public void ProcessEnd(params object[] args)
        {
            
        }
        /// <summary>
        /// 获取主题分页
        /// </summary>
        /// <returns></returns>
        public SubjectPagerPart GetModel()
        {
            if (SubjectPagers == null || SubjectPagers.Count == 0)
            {
                return null;
            }
            SubjectPagerPart pager = SubjectPagers.OrderByDescending(t => t.Power).ThenBy(t => t.hFragment.LinkCharsCount).FirstOrDefault();
            return pager;
        }
        #endregion

        #region 加权
        /// <summary>
        /// 节点样式权限
        /// </summary>
        /// <param name="hnode"></param>
        /// <returns></returns>
        private double NodeClassPower(HtmlNode hnode)
        {
            HtmlAttribute nodeId = hnode.Attributes["id"];
            HtmlAttribute nodeClass = hnode.Attributes["class"];
            double power = 0;
            if (nodeId != null)
            {
                string idValue = nodeId.Value.ToLower();
                if (idValue.Contains("page"))
                {
                    power += 1;
                }
            }

            if (nodeClass != null)
            {
                string classValue = nodeClass.Value.ToLower();
                if (classValue.Contains("page"))
                {
                    power += 1;
                }
            }
            return power;
        }

        /// <summary>
        /// 文字链接密度权限
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
        private double LinkDensityPower(HtmlFragment HF)
        {
            if (HF.LinkDensity > 0.2)
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// 链接平均文字数权限
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
        private double LinkCharsAvgPower(HtmlFragment HF)
        {
            if (HF.LinkCount > 3 && HF.LinkCharsAvg > 3)
            {
                return -2;
            }
            return 0;
        }
        /// <summary>
        /// 标签密度权限
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
        private double TagDensityPower(HtmlFragment HF)
        {
            if (HF.TagDensity > 500 && HF.TagDensity < 3000)
            {
                return 1;
            }

            return 0;
        }
        /// <summary>
        /// 链接特征权限
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
        private double LinkFeature(HtmlFragment HF)
        {
            double power = 0;
            string pattern1 = @"(?i)(<a[^>]*?>(<.*>)?([[])?(?<name>[\d]+?)([]])?(<.*>)?</a>)";
            string pattern2 = @"(?i)(<a[^>]*?>(<.*>)?(?<name>[上一页]+?)(<.*>)?</a>)";
            string pattern3 = @"(?i)(<a[^>]*?>(<.*>)?(?<name>[下一页|尾页]+?)(<.*>)?</a>)";
            string pattern4 = @"(?i)共(.*) \d+(.*)[条|页]";
            if (Regex.Match(HF.HtmlSource, pattern1).Success)
            {
                power += 2;
            }
            if (Regex.Match(HF.HtmlSource, pattern2).Success)
            {
                power += 1;
            }
            if (Regex.Match(HF.HtmlSource, pattern3).Success)
            {
                power += 1;
            }
            if (Regex.Match(HF.HtmlSource, pattern4).Success)
            {
                power += 1;
            }
            return power;
        }

        /// <summary>
        /// 包含关键字权限
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
        private double IncludeWordsPower(HtmlFragment HF)
        {
            if (string.IsNullOrEmpty(HF.HNode.InnerText))
            {
                return 0;
            }
            double power = 0;
            string pattern1 = @"(?i)(Total|上一页|上页)";
            string pattern2 = @"(?i)(尾页|下一页|下页)";
            if (Regex.Match(HF.HNode.InnerText, pattern1).Success)
            {
                power += 1;
            }
            if (Regex.Match(HF.HNode.InnerText, pattern2).Success)
            {
                power += 1;
            }
            return power;
        }
        /// <summary>
        /// 排除关键字权限
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
        private double ExcludeWordsPower(string sourceHtml)
        {
            if (string.IsNullOrEmpty(sourceHtml))
            {
                return 0;
            }
            double power = 0;
            string pattern1 = @"(?i)(首页|联系|版权|Copyright|关注)";
            string pattern2 = @"(?i)(ICP备\d+号)";
            //string sourceLower=sourceHtml;
            if (Regex.Match(sourceHtml, pattern1).Success)
            {
                power += -5;
            }
            if (Regex.Match(sourceHtml, pattern2).Success)
            {
                power += -5;
            }

            return power;
        }
        #endregion
    }
}

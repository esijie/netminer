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
    /// 主题
    /// </summary>
    public class SubjectDeal : IDeal<SubjectPart>
    {
        private string[] _dealSubjectTags = new string[] { "div", "td" };
        private List<SubjectPart> _subjects = new List<SubjectPart>();

        public SubjectDeal()
        {
        }

        ~SubjectDeal()
        {
            _subjects = null;
        }

        #region 属性
        /// <summary>
        /// 需要处理的标签(主题)
        /// </summary>
        public string[] DealSubjectTags
        {
            get { return _dealSubjectTags; }
            set { _dealSubjectTags = value; }
        }
        /// <summary>
        /// 主题列表
        /// </summary>
        public List<SubjectPart> Subjects
        {
            get { return _subjects; }
            set { _subjects = value; }
        }
        #endregion

        #region 接口
        /// <summary>
        /// 内容处理，入口
        /// </summary>
        /// <param name="hnode"></param>
        public void Process(HtmlFragment htmlFragment)
        {
            //不在处理标签内，返回
            if (!DealSubjectTags.Contains(htmlFragment.TagName))
            {
                return;
            }
            //字符数等于0的，返回
            if (htmlFragment.CharsCount == 0)
            {
                return;
            }
            //定义主题实体
            SubjectPart subject = new SubjectPart(htmlFragment.Sequence, htmlFragment);
            
            //加权操作
            subject.Power += TagPower(htmlFragment.HNode.InnerHtml);
            subject.Power += NodeClassPower(htmlFragment.HNode);
            subject.Power += NodeIdPower(htmlFragment.HNode);
            subject.Power += NoLinkCharsCountPower(htmlFragment);
            subject.Power += PunctuationCountPower(htmlFragment);
            subject.Power += PunctuationDensityPower(htmlFragment);
            subject.Power += TagDensityPower(htmlFragment);
            subject.Power += CharsDensityPower(htmlFragment);
            subject.Power += HtmlCharsDensityPower(htmlFragment);
            subject.Power += LinkDensityPower(htmlFragment);
            subject.Power += ExcludeWordsPower(htmlFragment.HNode.InnerHtml);

            //权值必须大于5，才进入候选队列
            if (subject.Power > 5)
            {
                Subjects.Add(subject);
            }
        }
        /// <summary>
        /// 递归完成后的处理(主题)
        /// </summary>
        public void ProcessEnd(params object[] args)
        {
            
        }
        /// <summary>
        /// 获取主题
        /// </summary>
        /// <returns></returns>
        public SubjectPart GetModel()
        {
            if (Subjects == null || Subjects.Count == 0)
            {
                return null;
            }
            //排序方式：权值降序，Html文字密度降序
            SubjectPart subject = Subjects.Where(t => t.Power > 7).OrderByDescending(t => t.Power).ThenByDescending(t => t.HtmlCharsDensity).FirstOrDefault();
            return subject;
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
            string pattern = @"(?i)(<p>|<br(\s?)(/?)>)";

            if (Regex.Match(sourceHtml, pattern).Success)
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

            //sina的正文样式为articalContent
            if (classValue.Contains("articalcontent"))
            {
                return 2;
            }

            if (classValue.Contains("body") || classValue.Contains("content"))
            {
                return 1;
            }

            return 0;
        }
        /// <summary>
        /// 节点ID权限
        /// </summary>
        /// <param name="hnode"></param>
        /// <returns></returns>
        private double NodeIdPower(HtmlNode hnode)
        {
            HtmlAttribute nodeId = hnode.Attributes["id"];
            if (nodeId == null)
            {
                return 0;
            }
            string idValue = nodeId.Value.ToLower();
            if (idValue.Contains("post") || idValue.Contains("content"))
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// 非链接文字总数权限
        /// (HF.CharsCount-HF.LinkCharsCount)/200 权值取其整数部分，最大为5
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
        private double NoLinkCharsCountPower(HtmlFragment HF)
        {
            int multiple = (HF.CharsCount - HF.LinkCharsCount) / 200;
            if (multiple > 0)
            {
                multiple = multiple + 1;
                return multiple >= 5 ? 5 : multiple;
            }
            if ((HF.CharsCount - HF.LinkCharsCount) > 100)
            {
                return 1;
            }
            //else if ((HF.CharsCount - HF.LinkCharsCount) < 30)
            //{
            //    return -1;
            //}
            return 0;
        }

        /// <summary>
        /// 标点符号数权限
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
        private double PunctuationCountPower(HtmlFragment HF)
        {
            if (HF.PunctuationCount > 5)
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// 标点密度权限
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
        private double PunctuationDensityPower(HtmlFragment HF)
        {
            if (HF.PunctuationCount < 5)
            {
                return 0;
            }
            if (HF.PunctuationDensity > 0.03)
            {
                return 1;
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
            if (HF.CharsCount < 100)
            {
                return 0;
            }
            if (HF.TagDensity > 10 && HF.TagDensity <= 70)
            {
                return 3;
            }
            else if (HF.TagDensity > 70 && HF.TagDensity <= 140)
            {
                return 2;
            }
            else if (HF.TagDensity > 140 && HF.TagDensity <= 200)
            {
                return 1;
            }
            else if (HF.TagDensity > 300)
            {
                return -1;
            }
            return 0;
        }
        /// <summary>
        /// 非链接文字密度权限
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
        private double CharsDensityPower(HtmlFragment HF)
        {
            if (HF.CharsDensity > 0.7)
            {
                return 1;
            }
            return 0;
        }
        /// <summary>
        /// Html文字密度权限
        /// HF.HtmlCharsDensity*10 权值取其整数部分(四舍五入)，最大为3。
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
        private double HtmlCharsDensityPower(HtmlFragment HF)
        {
            if (HF.CharsCount < 100)
            {
                return 0;
            }
            int temp = (int)Math.Round((HF.HtmlCharsDensity*10),0);
            if (temp > 0)
            {
                return temp >= 3 ? 3 : temp;
            }
            return 0;
        }
        /// <summary>
        /// 文字链接密度权限
        /// </summary>
        /// <param name="HF"></param>
        /// <returns></returns>
        private double LinkDensityPower(HtmlFragment HF)
        {
            if (HF.LinkDensity < 0.08)
            {
                return 1;
            }
            return 0;
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
            string pattern = @"(?i)(登录名|匿名评论|更多|首页|博文|精彩)";
            string pattern2 = @"(?i)(版权所有|Copyright)";
            string pattern3 = @"(?i)(ICP备\d+号)";

            if (Regex.Match(sourceHtml, pattern).Success)
            {
                power += -2;
            }
            if (Regex.Match(sourceHtml, pattern2).Success)
            {
                power += -1;
            }
            if (Regex.Match(sourceHtml, pattern3).Success)
            {
                power += -2;
            }
            return power;
        }
        #endregion
    }
}

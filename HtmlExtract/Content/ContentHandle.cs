using System;
using System.Collections.Generic;
using System.Text;
using HtmlExtract.Utility;
using HtmlAgilityPack;
using HtmlExtract.Entities;
using System.Text.RegularExpressions;
using System.Linq;

namespace HtmlExtract.Content
{
    /// <summary>
    /// 内容处理入口
    /// </summary>
    public class ContentHandle
    {
        private string _htmlSource=string.Empty;
        private string _cleanSource = string.Empty;
        private int _sequence = 1;
        private string[] _dealTags=new string[]{"div","td","span","p","h1","h2","h3"};


        private TitlePart _reserveTitle = null;
        private PublishTimePart _reservePublishTime = null;
        private SubjectPart _reserveSubject = null;
        private SubjectPagerPart _reserveSubjectPager = null;
        private AuthorPart _reserverAuthor = null;
        private SourcePart _reserverSource = null;

        TitleDeal titleDeal;
        SubjectDeal subjectDeal;
        SubjectPagerDeal subjectPagerDeal;
        PublishTimeDeal publishTimeDeal;
        SourceDeal sourceDeal;
        AuthorDeal authorDeal;

        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="htmlSource">页面Html源码</param>
        public ContentHandle(string htmlSource)
        {
            _htmlSource = htmlSource;
            titleDeal = new TitleDeal();
            subjectDeal = new SubjectDeal();
            subjectPagerDeal = new SubjectPagerDeal();
            publishTimeDeal = new PublishTimeDeal();
            authorDeal = new AuthorDeal();
            sourceDeal = new SourceDeal();
        }

        ~ContentHandle()
        {
            _htmlSource = "";
            titleDeal = null;
            subjectDeal = null;
            subjectPagerDeal = null;
            publishTimeDeal = null;
            sourceDeal = null;
        }

        #region 属性
        /// <summary>
        /// Html源码
        /// </summary>
        public string HtmlSource
        {
            get { return _htmlSource; }
            set { _htmlSource = value; }
        }
        /// <summary>
        /// 格式化后的Html源码
        /// </summary>
        public string CleanSource
        {
            get { return _cleanSource; }
            set { _cleanSource = value; }
        }
        /// <summary>
        /// 序号
        /// </summary>
        public int Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }
        /// <summary>
        /// 需要处理的标签
        /// </summary>
        public string[] DealTags
        {
            get { return _dealTags; }
            set { _dealTags = value; }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public TitlePart ReserveTitle
        {
            get { return _reserveTitle; }
            set { _reserveTitle = value; }
        }
       
        /// <summary>
        /// 发表时间
        /// </summary>
        public PublishTimePart ReservePublishTime
        {
            get { return _reservePublishTime; }
            set { _reservePublishTime = value; }
        }
        /// <summary>
        /// 主题
        /// </summary>
        public SubjectPart ReserveSubject
        {
            get { return _reserveSubject; }
            set { _reserveSubject = value; }
        }
        /// <summary>
        /// 主题分页
        /// </summary>
        public SubjectPagerPart ReserveSubjectPager
        {
            get { return _reserveSubjectPager; }
            set { _reserveSubjectPager = value; }
        }

        public AuthorPart ReserverAuthor
        {
            get { return _reserverAuthor; }
            set { _reserverAuthor = value; }
        }

        public SourcePart ReserverSource
        {
            get { return _reserverSource; }
            set { _reserverSource = value; }
        }
        #endregion

        #region 入口
        /// <summary>
        /// 源码处理
        /// </summary>
        public void Process()
        {
            if (string.IsNullOrEmpty(HtmlSource))
            {
                return;
            }
            //清洗标签
            CleanSource = HtmlHelper.HtmlCleanTag(HtmlSource);

            //格式化Html，并将其专为一棵Dom树
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionOutputAsXml = true;
            htmlDoc.LoadHtml(CleanSource);

            //获取页面标题
            HtmlNode Title = htmlDoc.DocumentNode.SelectSingleNode("//title");
            string sourceTitle = Title == null ? "" : titleDeal.getTitle(Title.InnerText);
            titleDeal.SourceTitle = sourceTitle;

            //获取Body节点，并开始处理
            HtmlNode Body = htmlDoc.DocumentNode.SelectSingleNode("//body");
            if (Body != null)
            {
                NodeLoopProcess(Body,1);
            }

            //所有节点处理结束后
            ProcessEnd();
        }

        /// <summary>
        /// 循环处理Html节点
        /// </summary>
        /// <param name="hnode"></param>
        /// <param name="lp">递归次数</param>
        private void NodeLoopProcess(HtmlNode hnode,int lp)
        {
            //获取该节点下得子节点列表
            HtmlNodeCollection childrens = hnode.ChildNodes;
            //循环处理子节点
            foreach (HtmlNode cNode in childrens)
            {
                if (Sequence > 200)
                    break;

                //该节点是否在需要处理的标签内
                if (DealTags.Contains(cNode.Name.ToLower()))
                {
                   
                    try
                    {
                        //处理节点，获取其特征值，并专为可操作的对象
                        HtmlFragment HF = new HtmlFragment(Sequence++, cNode);

                        titleDeal.Process(HF);//标题 
                        publishTimeDeal.Process(HF);//发表时间
                        subjectDeal.Process(HF);//主题
                        //subjectPagerDeal.Process(HF);//主题分页
                        authorDeal.Process(HF);
                        sourceDeal.Process(HF);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                //如果子节点还有下级节点，并且递归级别小于30的，递归处理。
                if (cNode.ChildNodes != null && lp<15)
                {
                    NodeLoopProcess(cNode, lp+1);
                }
            }
        }

        /// <summary>
        /// 递归处理所有节点，完成后的处理
        /// </summary>
        private void ProcessEnd()
        {
            //标题
            titleDeal.ProcessEnd(null);
            ReserveTitle = titleDeal.GetModel();

            //发表时间
            publishTimeDeal.ProcessEnd(null);
            ReservePublishTime = publishTimeDeal.GetModel();

            //主题
            subjectDeal.ProcessEnd();
            ReserveSubject = subjectDeal.GetModel();

            //主题分页
            subjectPagerDeal.ProcessEnd();
            ReserveSubjectPager = subjectPagerDeal.GetModel();

            //作者
            authorDeal.ProcessEnd();
            ReserverAuthor = authorDeal.GetModel();

            //来源
            sourceDeal.ProcessEnd();
            ReserverSource = sourceDeal.GetModel();

        }
        #endregion

        #region 标题
        /// <summary>
        /// 获取标题
        /// </summary>
        /// <returns></returns>
        public string GetTitle()
        {
            string returnTitle = string.Empty;
            if (ReserveTitle != null)
            {
                returnTitle = ReserveTitle.hFragment.HNode.InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("&nbsp;", " ").TrimStart().TrimEnd();
                //如果包含源Title，则直接将源Title当做标题
                if (!string.IsNullOrEmpty(titleDeal.SourceTitle) && returnTitle.Contains(titleDeal.SourceTitle))
                {
                    //returnTitle = titleDeal.SourceTitle;

                }
            }
            return returnTitle;
        }
        #endregion

        #region 发表时间
        /// <summary>
        /// 获取发表时间
        /// </summary>
        /// <returns></returns>
        public string GetPublishTime()
        {
            if (ReservePublishTime != null)
            {
                return publishTimeDeal.GetDateTime(ReservePublishTime.hFragment.HNode.InnerText);
            }
            return string.Empty;
        }

        public string GetAuthor()
        {
            if (ReserverAuthor != null)
            {
                return _reserverAuthor.hFragment.HNode.InnerText;
            }
            return string.Empty;
        }

        public string GetSource()
        {
            if (ReserverSource != null)
            {
                return _reserverSource.hFragment.HNode.InnerText;
            }
            return string.Empty;
        }

        #endregion

        #region 内容
        /// <summary>
        /// 获取主题(未去噪)
        /// </summary>
        /// <returns></returns>
        public string GetSubject()
        {
            if (ReserveSubject != null)
            {
                return ReserveSubject.hFragment.HNode.InnerHtml.Replace("&amp;", "&");
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取去噪后的内容
        /// </summary>
        /// <returns></returns>
        public string GetClearSubject()
        {
            string clearSubject=string.Empty;
            if (ReserveSubject == null)
            {
                return string.Empty;
            }

            HtmlNode subjectNode = ReserveSubject.hFragment.HNode;

            clearSubject = NodeLoopSubjectProcess(subjectNode, 1).InnerHtml;
            
            clearSubject =clearSubject.Replace("&amp;", "&");

            //将分页下面的所有内容去除
            string pagerHtml=GetSubjectPager();
            if(!string.IsNullOrEmpty(pagerHtml))
            {
                int pagerIndex = clearSubject.IndexOf(pagerHtml);
                if (pagerIndex > 0)
                {
                    string subContent = clearSubject.Substring(0, pagerIndex);

                    string subText = Regex.Replace(subContent, @"<[^>]+>", "", RegexOptions.IgnoreCase).Trim();
                    string fullText = Regex.Replace(clearSubject, @"<[^>]+>", "", RegexOptions.IgnoreCase).Trim();

                    if (!string.IsNullOrEmpty(subText) && !string.IsNullOrEmpty(fullText))
                    {
                        double sf = Math.Round((double)subContent.Length / (double)fullText.Length,2);
                        if (sf > 0.4)//分页前部分文字内容占全部文字内容的40%以上
                        {
                            clearSubject = subContent;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(clearSubject))//标签补全
            {
                clearSubject = HtmlHelper.HtmlFormat(clearSubject);
                clearSubject = clearSubject.Replace("<?xml version=\"1.0\" encoding=\"gb2312\"?>","");
            }

            clearSubject=clearSubject.Replace("&amp;", "&");
            clearSubject = clearSubject.Replace("&lt;/form&gt;", "</form>");
            
            //移除不需要的标签
            clearSubject = Regex.Replace(clearSubject, @"(?is)(<form[^>]*>(.*?)</form>)",string.Empty, RegexOptions.IgnoreCase);
            clearSubject = Regex.Replace(clearSubject, @"(?is)(<(input).*?/>)", string.Empty, RegexOptions.IgnoreCase);

            return clearSubject;
        }
        #endregion

        #region 内容去噪
        /// <summary>
        /// 循环处理正文Html节点
        /// </summary>
        /// <param name="hnode"></param>
        /// <param name="lp">递归次数</param>
        private HtmlNode NodeLoopSubjectProcess(HtmlNode hnode, int lp)
        {
            //需要处理的标签块
            string[] dealTags = new string[] { "div" };

            //获取该节点下得子节点列表
            HtmlNodeCollection childrens = hnode.ChildNodes;
            
            int len=childrens.Count;
            //循环处理子节点
            for (int i=0;i<len;i++)
            {
                HtmlNode cNode = childrens[i];
                
                if (cNode.NodeType == HtmlNodeType.Text)
                {
                    continue;
                }
                //该节点是否在需要处理的标签内
                if (dealTags.Contains(cNode.Name.ToLower()))
                {
                    try
                    {
                        //处理节点，获取其特征值，并专为可操作的对象
                        HtmlFragment HF = new HtmlFragment(Sequence++, cNode);

                        bool a = SubjectNoiseCheck(HF);//内容噪点检测
                        if (!a)
                        {
                            hnode.RemoveChild(cNode);//移除噪点
                            len--;
                            i--;
                            continue;
                        } 
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                //如果子节点还有下级节点，并且递归级别小于5的，递归处理。
                if (cNode.ChildNodes != null && lp < 5)
                {
                    HtmlNode rNode = NodeLoopSubjectProcess(cNode, lp + 1);
                    hnode.ReplaceChild(rNode,cNode);
                }
            }
            return hnode;
        }

        /// <summary>
        /// 内容噪点检测
        /// </summary>
        /// <param name="htmlFragment"></param>
        /// <returns></returns>
        public bool SubjectNoiseCheck(HtmlFragment htmlFragment)
        {
            if (htmlFragment.LinkCharsAvg > 5 && htmlFragment.CharsDensity<0.2)
            {
                return false;
            }
            //string pattern1 = @"(?i)<a[^>]*?>.*(评论|留言).*?</a>";
            //if (htmlFragment.LinkCharsAvg > 3 && Regex.Match(htmlFragment.HNode.InnerHtml, pattern1).Success)
            //{
            //    return false;
            //}
            HtmlAttribute nodeId = htmlFragment.HNode.Attributes["id"];
            if (nodeId != null)
            {
                string idValue = nodeId.Value.ToLower();
                if (idValue.Contains("comment"))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 内容分页
        /// <summary>
        /// 获取主题分页
        /// </summary>
        /// <returns></returns>
        public string GetSubjectPager()
        {
            if (ReserveSubjectPager != null)
            {
                return ReserveSubjectPager.hFragment.HNode.OuterHtml.Replace("&amp;", "&");
            }
            return string.Empty;
        }
        #endregion
        
    }
}

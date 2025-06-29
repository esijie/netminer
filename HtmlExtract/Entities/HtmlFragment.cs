using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using System.Collections;
using HtmlExtract.Utility;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlExtract.Entities
{
    public class HtmlFragment
    {
        private int _sequence = 0;
        private HtmlNode _hNode;
        private string _tagName=string.Empty;
        private string _htmlSource = string.Empty;

        private int _htmlCount=0; 
        private int _charsCount=0;
        private int _CNCharsCount = 0;
        private int _tagCount=0; 
        private int _linkCount=0; 
        private int _linkCharsCount=0;
        private int _punctuationCount = 0;

        private double _charsDensity = 0.0;
        private double _htmlCharsDensity = 0.0;
        private double _punctuationDensity = 0.0;
        private double _linkDensity = 0.0;

        public HtmlFragment(int i, HtmlNode hNode)
        {
            _sequence = i;
            _hNode = hNode;
            Init();
        }

        #region 属性
        /// <summary>
        /// 序号
        /// </summary>
        public int Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }
        /// <summary>
        /// Html节点
        /// </summary>
        public HtmlNode HNode
        {
            get { return _hNode; }
            set { _hNode = value; }
        }
        /// <summary>
        /// Html节点的标签名
        /// </summary>
        public string TagName
        {
            get { return _tagName; }
            set { _tagName = value; }
        }
        /// <summary>
        /// Html内容
        /// </summary>
        public string HtmlSource
        {
            get { return _htmlSource; }
            set { _htmlSource = value; }
        }
        /// <summary>
        /// 总字符（包括html字符）
        /// </summary>
        public int HtmlCount
        {
            get { return _htmlCount; }
            set { _htmlCount = value; }
        }
        /// <summary>
        /// 总文字数（不包括html字符，但包括a连接内的文字）
        /// </summary>
        public int CharsCount
        {
            get { return _charsCount; }
            set { _charsCount = value; }
        }

        /// <summary>
        /// 包含的中文字数
        /// </summary>
        public int CNCharsCount
        {
            get { return _CNCharsCount; }
            set { _CNCharsCount = value; }
        }

        /// <summary>
        /// 总标签数
        /// </summary>
        public int TagCount
        {
            get { return _tagCount; }
            set { _tagCount = value; }
        }
        /// <summary>
        /// 总链接数
        /// </summary>
        public int LinkCount
        {
            get { return _linkCount; }
            set { _linkCount = value; }
        }
        /// <summary>
        /// 总链接文字数
        /// </summary>
        public int LinkCharsCount
        {
            get { return _linkCharsCount; }
            set { _linkCharsCount = value; }
        }
        /// <summary>
        /// 标点符号数,包括如下：， 。、 , .
        /// </summary>
        public int PunctuationCount 
        {
            get { return _punctuationCount; }
            set { _punctuationCount = value; }
        }

        /// <summary>
        /// 非链接文字密度
        /// </summary>
        public double CharsDensity
        {
            get { return _charsDensity; }
            set { _charsDensity = value; }

        }
        /// <summary>
        /// Html文字密度
        /// </summary>
        public double HtmlCharsDensity
        {
            get { return _htmlCharsDensity; }
            set { _htmlCharsDensity = value; }
        }
        /// <summary>
        /// 标点密度
        /// </summary>
        public double PunctuationDensity 
        {
            get { return _punctuationDensity; }
            set { _punctuationDensity = value; }

        }
        /// <summary>
        /// 标签密度
        /// </summary>
        public double TagDensity 
        {
            get
            {
                if (CharsCount == 0)
                {
                    return 0;
                }
                return 1000 * TagCount / CharsCount;
            }

        }
        /// <summary>
        /// 文字链接密度
        /// </summary>
        public double LinkDensity 
        {
            get { return _linkDensity; }
            set { _linkDensity = value; }
        }

        /// <summary>
        /// 链接平均文字数
        /// </summary>
        public double LinkCharsAvg
        {
            get
            {
                if (LinkCount == 0)
                {
                    return 0;
                }
                return LinkCharsCount / LinkCount;
            }

        }

        #endregion

        #region 方法
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            //节点标签
            TagName = HNode.Name.ToLower();
            //节点源Html
            HtmlSource = HNode.OuterHtml;
            //源Htm总字符数
            HtmlCount = HtmlSource.Length;

            //去除特殊字符
            string innetText=HtmlHelper.RemoveSpecialChars(HNode.InnerText);

            //总文字数（不包括html字符，但包括a连接内的文字）
            CharsCount = string.IsNullOrEmpty(innetText)?0: innetText.Length;

            //包含的中文字数，标题一般都需要包含中文
            Regex re = new Regex("[\\u4e00-\\u9fa5]", RegexOptions.None);
            MatchCollection mc = re.Matches(innetText);
            CNCharsCount = mc.Count;

            //总标签数
            TagCount = HtmlHelper.GetTagCount(HNode.InnerHtml);

            //获取链接处理
            ArrayList linkInfo = HtmlHelper.GetLinkInfo(HNode.InnerHtml);

            //总链接数
            LinkCount = (int)linkInfo[0];
            //总链接文字数
            LinkCharsCount = (int)linkInfo[1];
            //获取标点符号数
            PunctuationCount = HtmlHelper.GetPunctuationCount(HNode.InnerText);

            //非链接文字密度
            CharsDensity = CharsCount == 0 ? 0 : Math.Round(((CharsCount - LinkCharsCount) / (double)CharsCount), 2);
            //Html文字密度
            HtmlCharsDensity = HtmlCount == 0 ? 0 : Math.Round((CharsCount / (double)HtmlCount), 4);
            //标点密度
            PunctuationDensity=CharsCount == 0?0:Math.Round((PunctuationCount / (double)CharsCount),2);
            //文字链接密度
            LinkDensity=CharsCount == 0?0:Math.Round((LinkCount / (double)CharsCount),2);
        }
        #endregion
    }
}

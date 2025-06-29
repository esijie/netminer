using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlExtract.Entities
{
    /// <summary>
    /// 标题实体
    /// </summary>
    public class TitlePart:BasePart
    {
        private int _sequence = 0;
        private double _power=1;
        private int _charsCount = 0;
        private HtmlFragment _hFragment=null;

        public TitlePart(int sequence, HtmlFragment hFragment)
        {
            _sequence = sequence;
            _hFragment = hFragment;
            _charsCount = hFragment.CharsCount;
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
        /// 权值
        /// </summary>
        public double Power
        {
            get { return _power; }
            set { _power = value; }
        }
        /// <summary>
        /// Html片段实体
        /// </summary>
        public HtmlFragment hFragment
        {
            get { return _hFragment; }
            set { _hFragment = value; }
        }
        /// <summary>
        /// 总文字数
        /// </summary>
        public int CharsCount
        {
            get { return _charsCount; }
            set { _charsCount = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlExtract.Entities
{
    /// <summary>
    /// 主题实体
    /// </summary>
    public class SubjectPart : BasePart
    {
        private int _sequence = 0;
        private double _power=5;
        private double _htmlCharsDensity = 0.0;

        private HtmlFragment _hFragment=null;

        public SubjectPart(int sequence, HtmlFragment hFragment)
        {
            _sequence = sequence;
            _hFragment = hFragment;
            _htmlCharsDensity = hFragment.HtmlCharsDensity;
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
        /// Html文字密度
        /// </summary>
        public double HtmlCharsDensity
        {
            get { return _htmlCharsDensity; }
            set { _htmlCharsDensity = value; }
        }
        /// <summary>
        /// Html片段实体
        /// </summary>
        public HtmlFragment hFragment
        {
            get { return _hFragment; }
            set { _hFragment = value; }
        }
    }
}

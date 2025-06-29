using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlExtract.Entities
{
    public class AuthorPart : BasePart
    {
        private int _sequence = 0;
        private double _power=1;
        private HtmlFragment _hFragment=null;

        public AuthorPart(int sequence, HtmlFragment hFragment)
        {
            _sequence = sequence;
            _hFragment = hFragment;
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
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Seg.Dict
{
    public enum WordType
    {
        None = 0,
        English = 1,
        SimplifiedChinese = 2,
        TraditionalChinese = 3,
        Numeric = 4,
        Symbol = 5,
        Space = 6,
        Synonym = 7, //同义词
        PhraseChinese=8,             //中文短语
    }


    public class WordInfo : WordAttribute, IComparable<WordInfo>
    {
        /// <summary>
        /// Current word type
        /// </summary>
        public WordType WordType;

        /// <summary>
        /// Original word type
        /// </summary>
        public WordType OriginalWordType;

        /// <summary>
        /// Word position
        /// </summary>
        public int Position;

        /// <summary>
        /// Rank for this word
        /// 单词权重
        /// </summary>
        public int Rank;

        /// <summary>
        /// 单词所属行业，如果是通用则默认为 0
        /// 有些词语分类较为显著：譬如：奥迪 为汽车品牌 但有些词语会有歧义，譬如：大众 为汽车品牌，同时也为常用词语
        /// 针对有歧义的词语，建议进行二期扩展，用于识别词语在行业出现的词频，来进行分类处理
        /// </summary>
        public int Industry;

        public WordInfo()
        {
        }

        public WordInfo(string word, int position, POS pos, double frequency, int rank, WordType wordTye, WordType originalWordType,int industry)
            : base(word, pos, frequency)
        {
            Position = position;
            WordType = wordTye;
            OriginalWordType = originalWordType;
            Rank = rank;
            Industry = industry;
        }

        public WordInfo(string word, POS pos, double frequency)
            :base(word, pos, frequency)
        {
        }

        public WordInfo(WordAttribute wordAttr)
        {
            this.Word = wordAttr.Word;
            this.Pos = wordAttr.Pos;
            this.Frequency = wordAttr.Frequency;
        }

        public WordInfo(Dict.PositionLength pl, string oringinalText)
        {
            this.Word = oringinalText.Substring(pl.Position, pl.Length);
            this.Pos = pl.WordAttr.Pos;
            this.Frequency = pl.WordAttr.Frequency;
            this.WordType = WordType.SimplifiedChinese;
            this.Position = pl.Position;

            switch (pl.Level)
            {
                case 0:
                    this.Rank = 5;
                    break;
                case 1:
                    this.Rank = 3;
                    break;
                case 2:
                    this.Rank = 2;
                    break;
                case 3:
                    this.Rank =1;
                    break;
                default:
                    this.Rank = 5;
                    break;
            }

        }

        public int GetEndPositon()
        {
            return this.Position + this.Word.Length;
        }

        #region IComparable<WordInfo> Members

        public int CompareTo(WordInfo other)
        {
            if (other == null)
            {
                return -1;
            }

            if (this.Position != other.Position)
            {
                return this.Position.CompareTo(other.Position);
            }

            if (other.Word == null)
            {
                return -1;
            }

            return this.Word.Length.CompareTo(other.Word.Length);
        }

        #endregion
    }
}

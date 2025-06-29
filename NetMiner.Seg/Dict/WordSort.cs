using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Seg.Dict
{
    public class WordSort : WordAttribute, IComparable<WordSort>
    {
        public string Word;

        /// <summary>
        /// Original word type
        /// </summary>
        public int WordCount;

        public override bool Equals(System.Object obj)
        {
          // If parameter is null return false.
          if (obj == null)
          {
            return false;
          }

          // If parameter cannot be cast to Mail_DATA return false.
          WordSort c = obj as WordSort;

          if ((System.Object)c == null)
          {
            return false;
          }

          return (this.Word== c.Word);//这里根据需要确定，哪些属性能够确定两个对象equal的列出
        } 

        #region IComparable Members

        public int CompareTo(WordSort obj)
        {
            int res = 0;
            try
            {
                WordSort sObj = (WordSort)obj;

                if (this.WordCount > sObj.WordCount)
                {
                    res = -1;
                }
                else if (this.WordCount < sObj.WordCount)
                {
                    res = 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

        #endregion

    }
}

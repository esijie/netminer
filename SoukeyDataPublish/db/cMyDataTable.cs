using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SoukeyDataPublish.db
{
    public class cMyDataTable : DataTable, IComparable<cMyDataTable>
    {


        #region IComparable<cMyDataTable> Members

        public int CompareTo(cMyDataTable obj)
        {
            int res = 0;
            try
            {
                cMyDataTable sObj = (cMyDataTable)obj;

                if (this.Rows.Count  > sObj.Rows.Count)
                {
                    res = -1;
                }
                else if (this.Rows.Count < sObj.Rows.Count)
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

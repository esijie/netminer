using System;
using System.Collections.Generic;
using System.Text;

namespace DataGatherControl
{
    public static class cGlobal
    {
        public enum ParaValueType
        {
            None = 1001,
            Custom = 1002,
            Selected = 1003,
        }

        public enum GatherDataType
        {
            Text=1101,
            Image=1102,
        }
    }
}

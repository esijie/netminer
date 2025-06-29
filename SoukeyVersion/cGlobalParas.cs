using System;
using System.Collections.Generic;
using System.Text;

namespace SoukeyVersion
{
    public static class cGlobalParas
    {
        public enum VersionType
        {
            Business = 3051,    //正式版本，已注册激活
            Trial = 3052,      //30天试用版本
            Beta = 3053,        //可对外发行的测试版本，由key来控制最终使用时间
        }
    }
}

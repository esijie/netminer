using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Common;

namespace NetMiner.ETL
{
    /// <summary>
    /// 通用数据加工规则处理类
    /// </summary>
    public class CommonRule
    {
        public string ClearHtml(string webSource)
        {
            return ToolUtil.getTxt(webSource);
        }
    }
}

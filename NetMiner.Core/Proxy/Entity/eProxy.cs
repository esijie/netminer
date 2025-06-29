using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NetMiner.Common;
using NetMiner.Common.Tool;

///功能：代理信息的维护
///完成时间：2010-12-1
///作者：一孑
///遗留问题：无
///说明：
///版本：02.10.0.03
///修订：无
namespace NetMiner.Core.Proxy.Entity
{
    public class eProxy
    {

        public string ProxyServer { get; set; }
        public string ProxyPort { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
   
    }
}

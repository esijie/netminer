using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NetMiner.Common;
using NetMiner.Common.Tool;

///���ܣ�������Ϣ��ά��
///���ʱ�䣺2010-12-1
///���ߣ�һ��
///�������⣺��
///˵����
///�汾��02.10.0.03
///�޶�����
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

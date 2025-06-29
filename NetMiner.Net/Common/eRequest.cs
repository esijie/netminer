using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Resource;
using System.Net;

namespace NetMiner.Net.Common
{
    public class eRequest
    {
        public eRequest()
        {
            rHeaders = new Hashtable();
        }

        ~eRequest()
        {

        }

        public Uri uri { get; set; }
        public int ContentLength { get; set; }
        public cGlobalParas.RequestMethod Method { get; set; }
        public string referUrl { get; set; }
        public CookieContainer Cookie { get; set; }
        public cGlobalParas.WebCode WebCode { get; set; }
        public bool AllAllowAutoRedirect { get; set; }
    
        public Hashtable rHeaders { get; set; }
        public byte[] Body { get; set; }
        public string ContentType { get; set; }

        public cGlobalParas.ProxyType ProxyType { get; set; }
        public WebProxy webProxy { get; set; }
        public string socket5Server { get; set; }
        public int socket5Port { get; set; }
    }
}

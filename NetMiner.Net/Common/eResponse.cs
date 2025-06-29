using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace NetMiner.Net.Common
{
    public class eResponse
    {
        public eResponse()
        {
            eHeaders = new Hashtable();
        }

        ~eResponse()
        {
            eHeaders = null;
        }

        public Uri rUri { get; set; }
        public CookieCollection cookie { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Hashtable eHeaders { get; set; }
        public string Body { get; set; }
        public DateTime rTime { get; set; }

    }
}

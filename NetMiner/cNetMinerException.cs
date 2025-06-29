using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner
{
    public class NetMinerException : System.ApplicationException
    {
        public NetMinerException()
            : base("exception message")
        {
            
        }

        public NetMinerException(string message)
            : base(message)
        {

        }

        public NetMinerException(string message, System.Exception inner)
            : base(message, inner)
        {

        }
    }

    public class NetMinerSkipUrlException : System.ApplicationException
    {
        const int SkipUrlHResult = unchecked((int)0x81000100);
        public NetMinerSkipUrlException(string message)
            : base(string.Format("(HRESULT:0x{1:X8}) {0}", message, SkipUrlHResult))
        {
            HResult = SkipUrlHResult;
        }
    }
}

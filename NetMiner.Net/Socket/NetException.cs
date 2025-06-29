using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Net.Socket
{
    public class NetException : System.ApplicationException
    {


        public NetException()
            : base("exception message")
        {

        }

        public NetException(string message)
            : base(message)
        {

        }

        public NetException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class NetCredentialException : NetException
    {


        public NetCredentialException()
            : base("exception message")
        {

        }

        public NetCredentialException(string message)
            : base(message)
        {

        }

        public NetCredentialException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }


}

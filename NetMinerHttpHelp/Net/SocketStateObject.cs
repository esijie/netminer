using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetMinerHttpHelp.Net
{
    internal class SocketStateObject
    {
        private Socket _workSocket;
        public byte[] buffer;
        public StringBuilder sb = new StringBuilder();
        public bool complete;
        public const int BUFFER_SIZE = 4096;

        public SocketStateObject(Socket _socket)
        {
            buffer = new byte[BUFFER_SIZE];
            complete = false;
            _workSocket = _socket;
        }

        public Socket WorkSocket
        {
            get { return _workSocket; }
        }
    }
}

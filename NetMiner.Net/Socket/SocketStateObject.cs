using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetMiner.Net.Socket
{
    internal class SocketStateObject
    {
        private System.Net.Sockets.Socket _workSocket;
        public byte[] buffer;
        public StringBuilder sb = new StringBuilder();
        public bool complete;
        public const int BUFFER_SIZE = 4096;

        public SocketStateObject(System.Net.Sockets.Socket _socket)
        {
            buffer = new byte[BUFFER_SIZE];
            complete = false;
            _workSocket = _socket;
        }

        public System.Net.Sockets.Socket WorkSocket
        {
            get { return _workSocket; }
        }
    }
}

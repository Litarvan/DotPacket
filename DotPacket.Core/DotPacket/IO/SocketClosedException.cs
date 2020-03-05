using System;

namespace DotPacket.IO
{
    public class SocketClosedException : Exception
    {
        public SocketClosedException() : base("Socket closed")
        {
        }
    }
}
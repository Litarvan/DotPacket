using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DotPacket.IO
{
    public class SocketIOStream : IOStream
    {
        private Socket _socket;

        public SocketIOStream(Socket socket)
        {
            _socket = socket;
        }

        public async Task<uint> ReadBytes(byte[] bytes, uint offset, uint count)
        {
            var segment = new ArraySegment<byte>(bytes, (int) offset, (int) count);
            int read = _socket.Receive(new List<ArraySegment<byte>>(new []{segment}), SocketFlags.None);

            if (read == 0)
            {
                throw new SocketClosedException();
            }

            return (uint) Math.Max(0, read);
        }

        public async Task<uint> WriteBytes(byte[] bytes, uint offset, uint count)
        {
            var segment = new ArraySegment<byte>(bytes, (int) offset, (int) count);
            int written = _socket.Send(new List<ArraySegment<byte>>(new []{segment}), SocketFlags.None);

            return (uint) Math.Max(0, written);
        }

        public void Close()
        {
            _socket.Close();
        }
    }
}
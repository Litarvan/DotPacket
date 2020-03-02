using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using DotPacket.IO;

namespace DotPacket
{
    public class SocketPacketServer
    {
        public const uint DefaultBufferSize = 4096;

        private readonly IPEndPoint _endPoint;
        private readonly Socket _socket;
        private readonly uint _bufferSize;

        private bool _isBound;
        
        public SocketPacketServer(string host, int port)
        {
            IPAddress address = Dns.GetHostEntry(host).AddressList[0];

            _endPoint = new IPEndPoint(address, port);
            _socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _bufferSize = DefaultBufferSize;
        }

        public SocketPacketServer(IPEndPoint endPoint, Socket socket, uint bufferSize)
        {
            _endPoint = endPoint;
            _socket = socket;
            _bufferSize = bufferSize;
        }

        public async Task<NetworkConnection> Accept()
        {
            if (!_isBound)
            {
                _isBound = true;
                
                _socket.Bind(_endPoint);
                _socket.Listen(5);
            }
            
            var client = await _socket.AcceptAsync();
            return new NetworkConnection(NetworkSide.Server, new SocketIOStream(client), _bufferSize);
        }
    }
}
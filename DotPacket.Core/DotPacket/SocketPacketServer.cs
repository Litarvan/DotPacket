using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using DotPacket.IO;
using DotPacket.Registry;

namespace DotPacket
{
    public class SocketPacketServer
    {
        private readonly PacketRegistry _registry;
        private readonly IPEndPoint _endPoint;
        private readonly Socket _socket;
        private readonly uint _bufferSize;

        private bool _isBound;
        
        public NetContext Context { get; }
        public ContextFactory ContextFactory { get; set; }
        
        public SocketPacketServer(PacketRegistry registry, string host, int port, NetContext context)
        {
            var address = Dns.GetHostEntry(host).AddressList[0];

            _registry = registry;
            _endPoint = new IPEndPoint(address, port);
            _socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _bufferSize = DotPacket.DefaultBufferSize;

            Context = context;
            ContextFactory = DotPacket.DefaultContextFactory;
        }

        public SocketPacketServer(PacketRegistry registry, IPEndPoint endPoint, Socket socket, uint bufferSize, NetContext context)
        {
            _registry = registry;
            _endPoint = endPoint;
            _socket = socket;
            _bufferSize = bufferSize;

            Context = context;
            ContextFactory = DotPacket.DefaultContextFactory;
        }

        public async Task<NetworkConnection> Accept()
        {
            if (!_isBound)
            {
                _isBound = true;
                
                _registry.SetupFor(NetworkSide.Server);
                
                _socket.Bind(_endPoint);
                _socket.Listen(5);
            }
            
            var client = await _socket.AcceptAsync();
            return new NetworkConnection(_registry, new SocketIOStream(client), Context, ContextFactory, _bufferSize);
        }
    }
}
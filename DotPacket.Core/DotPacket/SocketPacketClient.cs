using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using DotPacket.IO;
using DotPacket.Registry;

namespace DotPacket
{
    public class SocketPacketClient
    {
        private readonly PacketRegistry _registry;
        private readonly IPEndPoint _endPoint;
        private readonly Socket _socket;
        private readonly uint _bufferSize;

        private bool _isConnected;
        
        public NetContext Context { get; }
        public ContextFactory ContextFactory { get; set; }
        
        public SocketPacketClient(PacketRegistry registry, string host, int port, NetContext context)
        {
            var address = Dns.GetHostEntry(host).AddressList[0];

            _registry = registry;
            _endPoint = new IPEndPoint(address, port);
            _socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _bufferSize = DotPacket.DefaultBufferSize;

            Context = context;
            ContextFactory = DotPacket.DefaultContextFactory;
        }

        public SocketPacketClient(PacketRegistry registry, IPEndPoint endPoint, Socket socket, uint bufferSize, NetContext context)
        {
            _registry = registry;
            _endPoint = endPoint;
            _socket = socket;
            _bufferSize = bufferSize;

            Context = context;
            ContextFactory = DotPacket.DefaultContextFactory;
        }

        public async Task<NetworkConnection> Connect()
        {
            if (!_isConnected)
            {
                _isConnected = true;
                
                _registry.SetupFor(NetworkSide.Client);
                await _socket.ConnectAsync(_endPoint);
            }
            
            return new NetworkConnection(_registry, new SocketIOStream(_socket), Context, ContextFactory, _bufferSize);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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
        private bool _isRunning;

        private readonly List<NetworkConnection> _connections;
        
        public NetContext Context { get; }
        public ContextFactory ContextFactory { get; set; }
        public event OnConnectionClose OnConnectionClose;

        public SocketPacketServer(PacketRegistry registry, string host, int port, NetContext context)
        {
            var address = Dns.GetHostEntry(host).AddressList[0];

            _registry = registry;
            _endPoint = new IPEndPoint(address, port);
            _socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _bufferSize = DotPacket.DefaultBufferSize;

            _connections = new List<NetworkConnection>();

            Context = context;
            ContextFactory = DotPacket.DefaultContextFactory;
        }

        public SocketPacketServer(PacketRegistry registry, IPEndPoint endPoint, Socket socket, uint bufferSize, NetContext context)
        {
            _registry = registry;
            _endPoint = endPoint;
            _socket = socket;
            _bufferSize = bufferSize;

            _connections = new List<NetworkConnection>();

            Context = context;
            ContextFactory = DotPacket.DefaultContextFactory;
        }

        public NetworkConnection Accept()
        {
            if (!_isBound)
            {
                _isBound = true;
                
                _registry.SetupFor(NetworkSide.Server);
                
                _socket.Bind(_endPoint);
                _socket.Listen(5);
            }
            
            var client = _socket.Accept();
            var conn = new NetworkConnection(_registry, new SocketIOStream(client), Context, ContextFactory, _bufferSize);
            
            var pos = _connections.Count;
            _connections.Add(conn);
            
            conn.OnClose += (context, e) =>
            {
                if (e != null)
                {
                    Console.Error.WriteLine(e);
                }
                
                _connections.RemoveAt(pos);

                if (OnConnectionClose != null)
                {
                    OnConnectionClose(context, e);   
                }
            };

            return conn;
        }

        public void RunInBackground()
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;

            var t = new Thread(() =>
            {
                while (_isRunning)
                {
                    try
                    {
                        var conn = Accept();
                        conn.ProcessInBackground();
                    }
                    catch (Exception)
                    {
                        _isRunning = false;
                        throw;
                    }
                } 
            });
            t.Start();

            _isRunning = false;
        }

        public void Close()
        {
            _isRunning = false;

            try
            {
                foreach (var conn in _connections)
                {
                    conn.Close();
                }
            }
            finally
            {
                _socket.Close();
            }
        }
    }
}
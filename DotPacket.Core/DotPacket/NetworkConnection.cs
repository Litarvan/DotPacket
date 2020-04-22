using System;
using System.Collections.Generic;
using System.Threading;

using DotPacket.IO;
using DotPacket.Registry;

namespace DotPacket
{
    public class NetworkConnection
    {
        private readonly PacketRegistry _registry;
        private readonly IOStream _stream;

        private readonly StreamReader _in;
        private readonly StreamWriter _out;

        private readonly ManualResetEvent _readLock;
        private readonly ManualResetEvent _writeLock;
        private readonly ManualResetEvent _queueLock;

        private readonly List<object> _packetQueue;

        private bool _isRunning;

        public ConnectionContext Context { get; }
        public event OnConnectionClose OnClose;
        
        public NetworkConnection(
            PacketRegistry registry,
            IOStream stream,
            NetContext globalContext,
            ContextFactory contextFactory,
            uint bufferSize
        ) {
            _registry = registry;
            _stream = stream;
            
            _in = new StreamReader(_stream, bufferSize);
            _out = new StreamWriter(_stream, bufferSize);

            _readLock = new ManualResetEvent(true);
            _writeLock = new ManualResetEvent(true);
            _queueLock = new ManualResetEvent(true);
            
            _packetQueue = new List<object>();
            
            Context = contextFactory(this, globalContext);
        }
        
        public void ProcessNextPacket()
        {
            _readLock.WaitOne();
            
            var id = _in.ReadByte();
            var size = _in.ReadUnsignedShort();

            var data = _in.ReadBytes(size);
            _registry.Input(Context, id, data);
            
            _readLock.Set();
        }

        public void SendPacket(object packet)
        {
            SendPacket(packet, _out);
        }

        protected void SendPacket(object packet, StreamWriter to)
        {
            _writeLock.WaitOne();

            var result = new ByteArrayOutputStream();
            var output = new StreamWriter(result, DotPacket.DefaultBufferSize);
            
            var (id, data) = _registry.Output(Context.State, packet);
            
            output.WriteByte(id);
            output.WriteUnsignedShort((ushort) data.Length);
            output.WriteBytes(data);
            
            to.WriteBytes(result.GetBytes());
            
            _writeLock.Set();
        }

        public void AddToSendQueue(object packet)
        {
            _queueLock.WaitOne();

            _packetQueue.Add(packet);
            
            _queueLock.Set();
        }

        public void SendQueue()
        {
            _queueLock.WaitOne();

            var result = new ByteArrayOutputStream();
            var output = new StreamWriter(result, DotPacket.DefaultBufferSize);
            
            foreach (var packet in _packetQueue)
            {
                SendPacket(packet, output);
            }
            
            _out.WriteBytes(result.GetBytes());
            
            _queueLock.Set();
        }

        public void ProcessInBackground()
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
                        ProcessNextPacket();
                    }
                    catch (Exception e)
                    {
                        if (OnClose != null)
                        {
                            OnClose(Context, e is SocketClosedException ? null : e);
                        }
                    
                        break;
                    }
                }
            
                _isRunning = false;
            });
            t.Start();
        }

        public void Close()
        {
            _isRunning = false;
            
            if (OnClose != null)
            {
                OnClose(Context, null);
            }
        }
    }

    public delegate void OnConnectionClose(ConnectionContext context, Exception error); 
}
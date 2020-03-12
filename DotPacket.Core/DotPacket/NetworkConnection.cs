using System;
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
            _writeLock.WaitOne();

            var (id, data) = _registry.Output(Context.State, packet);
            
            _out.WriteByte(id);
            _out.WriteUnsignedShort((ushort) data.Length);
            _out.WriteBytes(data);
            
            _writeLock.Set();
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
            });
            t.Start();
            
            _isRunning = false;
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
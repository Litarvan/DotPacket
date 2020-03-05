using System;
using System.Threading;
using System.Threading.Tasks;

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
        
        public async Task ProcessNextPacket()
        {
            _readLock.WaitOne();
            
            var id = await _in.ReadByte();
            var size = await _in.ReadUnsignedShort();

            var data = await _in.ReadBytes(size);
            await _registry.Input(Context, id, data);
            
            _readLock.Set();
        }

        public async Task SendPacket(object packet)
        {
            _writeLock.WaitOne();

            var (id, task) = _registry.Output(Context.State, packet);
            await _out.WriteByte(id);

            var data = await task;
            await _out.WriteUnsignedShort((ushort) data.Length);
            await _out.WriteBytes(data);
            
            _writeLock.Set();
        }

        public async Task ProcessInBackground()
        {
            if (_isRunning)
            {
                return;
            }
            
            _isRunning = true;
            
            while (_isRunning)
            {
                try
                {
                    await ProcessNextPacket();
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
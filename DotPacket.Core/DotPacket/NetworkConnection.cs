using System.Threading.Tasks;

using DotPacket.IO;

namespace DotPacket
{
    public class NetworkConnection
    {
        private readonly NetworkSide _side;
        private readonly IOStream _stream;

        private readonly StreamReader _in;
        private readonly StreamWriter _out;
        
        private int _state;

        // TODO: Network connection
        
        public NetworkConnection(NetworkSide side, IOStream stream, uint bufferSize)
        {
            _side = side;
            _stream = stream;
            
            _in = new StreamReader(_stream, bufferSize);
            _out = new StreamWriter(_stream, bufferSize);
        }
        
        public async Task ProcessNextPacket<P>()
        {
            var id = await _in.ReadByte();
            var size = await _in.ReadUnsignedShort();

            var data = await _in.ReadBytes(size);
            var dataStream = new ByteArrayInputStream(data);
            
            
        }
    }
}
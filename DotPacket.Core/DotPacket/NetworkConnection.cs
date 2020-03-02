using System.Threading.Tasks;

using DotPacket.IO;

namespace DotPacket
{
    public class NetworkConnection
    {
        private NetworkSide _side;
        private IOStream _stream;

        private StreamReader _in;
        private StreamWriter _out;
        
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
            uint id = await _in.ReadUnsignedInt();
            uint size = await _in.ReadUnsignedInt();

            byte[] data = await _in.ReadBytes(size);
            var dataStream = new ByteArrayInputStream(data);
            
            
        }
    }
}
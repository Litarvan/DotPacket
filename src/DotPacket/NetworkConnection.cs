using System.Threading.Tasks;

using DotPacket.IO;

namespace DotPacket
{
    public class NetworkConnection
    {
        private Side _side;
        private IOStream _stream;

        private StreamReader _in;
        private StreamWriter _out;
        
        private int _state;

        public NetworkConnection(Side side, IOStream stream, uint bufferSize)
        {
            _side = side;
            _stream = stream;
            
            _in = new StreamReader(_stream, bufferSize);
            _out = new StreamWriter(_stream, bufferSize);
        }
        
        public async Task ProcessNextPacket<P>()
        {
            
        }
    }
}
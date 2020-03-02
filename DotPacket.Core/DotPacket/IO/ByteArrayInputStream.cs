using System;
using System.Threading.Tasks;

namespace DotPacket.IO
{
    public class ByteArrayInputStream : IOStream
    {
        private readonly byte[] _bytes;

        public ByteArrayInputStream(byte[] bytes)
        {
            _bytes = bytes;
        }

        public async Task<uint> ReadBytes(byte[] bytes, uint offset, uint count)
        {
            uint toRead = Math.Min((uint) bytes.Length - offset, count);
            
            for (uint i = offset; i < offset + toRead; i++)
            {
                bytes[i] = _bytes[i - offset];
            }

            return toRead;
        }

        public async Task<uint> WriteBytes(byte[] bytes, uint offset, uint count)
        {
            return 0;
        }
    }
}
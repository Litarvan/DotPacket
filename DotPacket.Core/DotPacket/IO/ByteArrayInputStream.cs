using System;
using System.Threading.Tasks;

namespace DotPacket.IO
{
    public class ByteArrayInputStream : IOStream
    {
        private readonly byte[] _bytes;
        private uint _cursor;
        // TODO: Utiliser Array.Copy ?

        public ByteArrayInputStream(byte[] bytes)
        {
            _bytes = bytes;
        }

        public Task<uint> ReadBytes(byte[] bytes, uint offset, uint count)
        {
            var toRead = Math.Min((uint) bytes.Length - offset, count);
            
            for (var i = offset; i < offset + toRead; i++)
            {
                bytes[i] = _bytes[i - offset + _cursor];
            }
            
            _cursor += toRead;

            return Task.FromResult(toRead);
        }

        public Task<uint> WriteBytes(byte[] bytes, uint offset, uint count)
        {
            return Task.FromResult((uint) 0);
        }
    }
}
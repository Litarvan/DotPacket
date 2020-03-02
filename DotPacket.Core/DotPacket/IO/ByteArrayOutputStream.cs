using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotPacket.IO
{
    public class ByteArrayOutputStream : IOStream
    {
        private List<byte> _bytes;
        
        public ByteArrayOutputStream()
        {
            _bytes = new List<byte>();
        }
        
        public async Task<uint> ReadBytes(byte[] bytes, uint offset, uint count)
        {
            return 0;
        }

        public async Task<uint> WriteBytes(byte[] bytes, uint offset, uint count)
        {
            uint toWrite = Math.Min((uint) bytes.Length - offset, count);
            
            for (uint i = offset; i < offset + toWrite; i++)
            {
                _bytes.Add(bytes[i]);
            }

            return toWrite;
        }

        public byte[] GetBytes()
        {
            return _bytes.ToArray();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotPacket.IO
{
    public class ByteArrayOutputStream : IOStream
    {
        private readonly List<byte> _bytes;
        // TODO: Utiliser Array.Copy ?
 
        public ByteArrayOutputStream()
        {
            _bytes = new List<byte>();
        }
        
        public Task<uint> ReadBytes(byte[] bytes, uint offset, uint count)
        {
            return Task.FromResult((uint) 0);
        }

        public Task<uint> WriteBytes(byte[] bytes, uint offset, uint count)
        {
            var toWrite = Math.Min((uint) bytes.Length - offset, count);
            
            for (var i = offset; i < offset + toWrite; i++)
            {
                _bytes.Add(bytes[i]);
            }

            return Task.FromResult(toWrite);
        }

        public byte[] GetBytes()
        {
            return _bytes.ToArray();
        }
    }
}
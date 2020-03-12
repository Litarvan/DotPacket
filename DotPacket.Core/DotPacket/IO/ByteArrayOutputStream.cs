using System;
using System.Collections.Generic;

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
        
        public uint ReadBytes(byte[] bytes, uint offset, uint count)
        {
            return 0;
        }

        public uint WriteBytes(byte[] bytes, uint offset, uint count)
        {
            var toWrite = Math.Min((uint) bytes.Length - offset, count);
            
            for (var i = offset; i < offset + toWrite; i++)
            {
                _bytes.Add(bytes[i]);
            }

            return toWrite;
        }

        public byte[] GetBytes()
        {
            return _bytes.ToArray();
        }

        public void Close()
        {
        }
    }
}
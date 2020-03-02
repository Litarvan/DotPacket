using System;
using System.Threading.Tasks;

namespace DotPacket.IO
{
    public class StreamWriter
    {
        private IOStream _stream;
        private uint _bufferSize;

        public StreamWriter(IOStream socket, uint bufferSize)
        {
            _stream = socket;
            _bufferSize = bufferSize;
        }
        
        public async Task WriteBytes(byte[] bytes)
        {
            uint counter = 0;
            
            while (counter < bytes.Length)
            {
                counter += await _stream.WriteBytes(bytes, counter, _bufferSize);
            }
        }

        public Task WriteByte(byte b)
        {
            return WriteBytes(new [] {b});
        }

        public Task WriteBool(bool b)
        {
            return WriteByte(b ? (byte) 1 : (byte) 0);
        }

        public Task WriteShort(short s)
        {
            return WriteBytes(BitConverter.GetBytes(s));
        }

        public Task WriteUnsignedShort(ushort s)
        {
            return WriteBytes(BitConverter.GetBytes(s));
        }

        public Task WriteInt(int i)
        {
            return WriteBytes(BitConverter.GetBytes(i));
        }

        public Task WriteUnsignedInt(uint i)
        {
            return WriteBytes(BitConverter.GetBytes(i));
        }

        public Task WriteLong(long l)
        {
            return WriteBytes(BitConverter.GetBytes(l));
        }

        public Task WriteUnsignedLong(ulong l)
        {
            return WriteBytes(BitConverter.GetBytes(l));
        }
    }
}
using System;
using System.Threading.Tasks;

namespace DotPacket.IO
{
    public class StreamWriter
    {
        private readonly IOStream _stream;
        private readonly uint _bufferSize;

        public StreamWriter(IOStream stream, uint bufferSize)
        {
            _stream = stream;
            _bufferSize = bufferSize;
        }
        
        public async Task WriteBytes(byte[] bytes)
        {
            uint counter = 0;
            
            while (counter < bytes.Length)
            {
                counter += await _stream.WriteBytes(bytes, counter, Math.Min((uint) bytes.Length - counter, _bufferSize));
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

        public Task WriteFloat(float f)
        {
            return WriteBytes(BitConverter.GetBytes(f));
        }

        public Task WriteDouble(double d)
        {
            return WriteBytes(BitConverter.GetBytes(d));
        }

        public Task WriteChar(char c)
        {
            return WriteBytes(BitConverter.GetBytes(c));
        }

        public async Task WriteString(string str)
        {
            await WriteUnsignedShort((ushort) str.Length);
            
            foreach (var c in str)
            {
                await WriteChar(c);
            }
        }
    }
}
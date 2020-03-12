using System;

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
        
        public void WriteBytes(byte[] bytes)
        {
            uint counter = 0;
            
            while (counter < bytes.Length)
            {
                counter += _stream.WriteBytes(bytes, counter, Math.Min((uint) bytes.Length - counter, _bufferSize));
            }
        }

        public void WriteByte(byte b)
        {
            WriteBytes(new [] {b});
        }

        public void WriteBool(bool b)
        {
            WriteByte(b ? (byte) 1 : (byte) 0);
        }

        public void WriteShort(short s)
        {
            WriteBytes(BitConverter.GetBytes(s));
        }

        public void WriteUnsignedShort(ushort s)
        {
            WriteBytes(BitConverter.GetBytes(s));
        }

        public void WriteInt(int i)
        {
            WriteBytes(BitConverter.GetBytes(i));
        }

        public void WriteUnsignedInt(uint i)
        {
            WriteBytes(BitConverter.GetBytes(i));
        }

        public void WriteLong(long l)
        {
            WriteBytes(BitConverter.GetBytes(l));
        }

        public void WriteUnsignedLong(ulong l)
        {
            WriteBytes(BitConverter.GetBytes(l));
        }

        public void WriteFloat(float f)
        {
            WriteBytes(BitConverter.GetBytes(f));
        }

        public void WriteDouble(double d)
        {
            WriteBytes(BitConverter.GetBytes(d));
        }

        public void WriteChar(char c)
        {
            WriteBytes(BitConverter.GetBytes(c));
        }

        public void WriteString(string str)
        {
            WriteUnsignedShort((ushort) str.Length);
            
            foreach (var c in str)
            { 
                WriteChar(c);
            }
        }
    }
}
using System;

namespace DotPacket.IO
{
    public class StreamReader
    {
        private readonly IOStream _stream;
        private readonly uint _bufferSize;

        public StreamReader(IOStream stream, uint bufferSize)
        {
            _stream = stream;
            _bufferSize = bufferSize;
        }

        public byte[] ReadBytes(uint count)
        {
            uint counter = 0;
            var result = new byte[count];
            
            while (counter < count)
            {
                counter += _stream.ReadBytes(result, counter, Math.Min(count - counter, _bufferSize));
            }

            return result;
        }

        public byte ReadByte()
        {
            return ReadBytes(1)[0];
        }

        public bool ReadBool()
        {
            return ReadByte() == 1;
        }

        public short ReadShort()
        {
            return BitConverter.ToInt16(ReadBytes(2), 0);
        }

        public ushort ReadUnsignedShort()
        {
            return BitConverter.ToUInt16(ReadBytes(2), 0);
        }

        public int ReadInt()
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }

        public uint ReadUnsignedInt()
        {
            return BitConverter.ToUInt32(ReadBytes(4), 0);
        }

        public long ReadLong()
        {
            return BitConverter.ToInt64(ReadBytes(8), 0);
        }

        public ulong ReadUnsignedLong()
        {
            return BitConverter.ToUInt64(ReadBytes(8), 0);
        }

        public float ReadFloat()
        {
            return BitConverter.ToSingle(ReadBytes(4), 0);
        }

        public double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBytes(8), 0);
        }

        public char ReadChar()
        {
            return BitConverter.ToChar(ReadBytes(2), 0);
        }

        public string ReadString()
        {
            var size = ReadUnsignedShort();
            var result = "";
            
            for (var i = 0; i < size; i++)
            {
                result += ReadChar();
            }

            return result;
        }
     }
}
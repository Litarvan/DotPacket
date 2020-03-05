using System;
using System.Threading.Tasks;

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

        public async Task<byte[]> ReadBytes(uint count)
        {
            uint counter = 0;
            var result = new byte[count];
            
            while (counter < count)
            {
                counter += await _stream.ReadBytes(result, counter, Math.Min(count - counter, _bufferSize));
            }

            return result;
        }

        public async Task<byte> ReadByte()
        {
            return (await ReadBytes(1))[0];
        }

        public async Task<bool> ReadBool()
        {
            return (await ReadByte()) == 1;
        }

        public async Task<short> ReadShort()
        {
            return BitConverter.ToInt16(await ReadBytes(2), 0);
        }

        public async Task<ushort> ReadUnsignedShort()
        {
            return BitConverter.ToUInt16(await ReadBytes(2), 0);
        }

        public async Task<int> ReadInt()
        {
            return BitConverter.ToInt32(await ReadBytes(4), 0);
        }

        public async Task<uint> ReadUnsignedInt()
        {
            return BitConverter.ToUInt32(await ReadBytes(4), 0);
        }

        public async Task<long> ReadLong()
        {
            return BitConverter.ToInt64(await ReadBytes(8), 0);
        }

        public async Task<ulong> ReadUnsignedLong()
        {
            return BitConverter.ToUInt64(await ReadBytes(8), 0);
        }

        public async Task<float> ReadFloat()
        {
            return BitConverter.ToSingle(await ReadBytes(4), 0);
        }

        public async Task<double> ReadDouble()
        {
            return BitConverter.ToDouble(await ReadBytes(8), 0);
        }

        public async Task<char> ReadChar()
        {
            return BitConverter.ToChar(await ReadBytes(2), 0);
        }

        public async Task<string> ReadString()
        {
            var size = await ReadUnsignedShort();
            var result = "";
            
            for (var i = 0; i < size; i++)
            {
                result += await ReadChar();
            }

            return result;
        }
     }
}
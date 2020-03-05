using System.Threading.Tasks;

namespace DotPacket.IO
{
    public interface IOStream
    {
        Task<uint> ReadBytes(byte[] bytes, uint offset, uint count);
        Task<uint> WriteBytes(byte[] bytes, uint offset, uint count);

        void Close();
    }
}
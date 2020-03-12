namespace DotPacket.IO
{
    public interface IOStream
    {
        uint ReadBytes(byte[] bytes, uint offset, uint count);
        uint WriteBytes(byte[] bytes, uint offset, uint count);

        void Close();
    }
}
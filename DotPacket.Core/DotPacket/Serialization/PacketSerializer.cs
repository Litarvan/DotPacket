using System;
using System.Threading.Tasks;

namespace DotPacket.Serialization
{
    public abstract class PacketSerializer : PacketProcessor
    {
        protected PacketSerializer(Type type) : base(type)
        {
        }

        public abstract Task<byte[]> Serialize(object packet);
    }
}
using System;

namespace DotPacket.Serialization
{
    public abstract class PacketSerializer : PacketProcessor
    {
        protected PacketSerializer(Type type) : base(type)
        {
        }

        public abstract byte[] Serialize(object packet);
    }
}
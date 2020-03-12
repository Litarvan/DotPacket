using System;

namespace DotPacket.Serialization
{
    public abstract class PacketDeserializer : PacketProcessor
    {
        protected PacketDeserializer(Type type) : base(type)
        {
        }

        public abstract object Deserialize(byte[] data);
    }
}
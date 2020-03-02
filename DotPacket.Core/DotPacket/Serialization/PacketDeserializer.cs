using System;
using System.Threading.Tasks;

namespace DotPacket.Serialization
{
    public abstract class PacketDeserializer : PacketProcessor
    {
        protected PacketDeserializer(Type type) : base(type)
        {
        }

        public abstract Task<object> Deserialize(byte[] data);
    }
}
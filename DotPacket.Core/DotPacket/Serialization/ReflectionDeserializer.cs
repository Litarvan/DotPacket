using System;
using System.Threading.Tasks;

namespace DotPacket.Serialization
{
    public class ReflectionDeserializer : PacketDeserializer
    {
        // TODO: Reflection deserializer

        public ReflectionDeserializer(Type type) : base(type)
        {
        }

        public override void Prepare()
        {
            base.Prepare();
            
            
        }

        public override async Task<object> Deserialize(byte[] data)
        {
            return null;
        }
    }
}
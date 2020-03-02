using System;
using System.Threading.Tasks;

namespace DotPacket.Serialization
{
    public class ReflectionSerializer : PacketSerializer
    {
        // TODO: Reflection serializer
        
        public ReflectionSerializer(Type type) : base(type)
        {
        }
        
        public override void Prepare()
        {
            base.Prepare();
            
            
        }

        public override async Task<byte[]> Serialize(object packet)
        {
            return new byte[0];
        }
    }
}
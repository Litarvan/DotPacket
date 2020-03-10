using System;
using System.Reflection;
using System.Threading.Tasks;
using DotPacket.IO;
using DotPacket.Serialization;

namespace DotPacket.Registry
{
    public class MethodPacketSerializer : PacketSerializer
    {
        private MethodInfo _method;
        
        public MethodPacketSerializer(MethodInfo method, Type type) : base(type)
        {
            _method = method;
        }

        public override async Task<byte[]> Serialize(object packet)
        {
            var stream = new ByteArrayOutputStream();
            var writer = new StreamWriter(stream, DotPacket.DefaultBufferSize);

            var result = _method.Invoke(packet, new object[] {writer});
            if (result is Task task)
            {
                await task;
            }

            return stream.GetBytes();
        }
    }
}
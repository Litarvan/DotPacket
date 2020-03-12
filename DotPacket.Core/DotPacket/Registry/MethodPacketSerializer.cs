using System;
using System.Reflection;

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

        public override byte[] Serialize(object packet)
        {
            var stream = new ByteArrayOutputStream();
            var writer = new StreamWriter(stream, DotPacket.DefaultBufferSize);

            _method.Invoke(packet, new object[] {writer});

            return stream.GetBytes();
        }
    }
}
using System;
using System.Threading.Tasks;
using DotPacket.Serialization;

namespace DotPacket.Registry
{
    public class OutputPacketBinding : PacketBinding
    {
        public PacketSerializer Serializer { get; }

        public OutputPacketBinding(Type packet, PacketSerializer serializer) : base(packet)
        {
            Serializer = serializer;
        }

        public Task<byte[]> Serialize(object packet)
        {
            return Serializer.Serialize(packet);
        }
    }
}
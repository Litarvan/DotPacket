using System;
using System.Threading.Tasks;
using DotPacket.Serialization;

namespace DotPacket.Registry
{
    public class OutputPacketBinding : PacketBinding
    {
        private PacketSerializer _serializer;

        public PacketSerializer Serializer => _serializer;

        public OutputPacketBinding(Type packet, PacketSerializer serializer) : base(packet)
        {
            _serializer = serializer;
        }

        public Task<byte[]> Serialize(object packet)
        {
            return _serializer.Serialize(packet);
        }
    }
}
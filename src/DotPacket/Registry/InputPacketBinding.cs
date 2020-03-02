using System;
using System.Threading.Tasks;

using DotPacket.Serialization;

namespace DotPacket.Registry
{
    public class InputPacketBinding : PacketBinding
    {
        private PacketDeserializer _deserializer;

        public PacketDeserializer Deserializer => _deserializer;

        public InputPacketBinding(Type packet, PacketDeserializer deserializer) : base(packet)
        {
            _deserializer = deserializer;
        }

        public async Task ReceiveAndHandle(byte[] data)
        {
            await Handler(await _deserializer.Deserialize(data));
        }
    }
}
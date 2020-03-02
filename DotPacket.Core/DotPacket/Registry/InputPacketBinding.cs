using System;
using System.Threading.Tasks;

using DotPacket.Serialization;

namespace DotPacket.Registry
{
    public class InputPacketBinding : PacketBinding
    {
        public PacketDeserializer Deserializer { get; }

        public InputPacketBinding(Type packet, PacketDeserializer deserializer) : base(packet)
        {
            Deserializer = deserializer;
        }

        public async Task ReceiveAndHandle(byte[] data)
        {
            await Handler(await Deserializer.Deserialize(data));
        }
    }
}
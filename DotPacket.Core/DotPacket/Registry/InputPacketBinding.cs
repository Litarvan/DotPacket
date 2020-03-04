using System;
using System.Threading.Tasks;

using DotPacket.Serialization;

namespace DotPacket.Registry
{
    public class InputPacketBinding : PacketBinding
    {
        public PacketDeserializer Deserializer { get; }
        public Handler Handler { get; set; }

        public InputPacketBinding(byte id, Type packet, PacketDeserializer deserializer) : base(id, packet)
        {
            Deserializer = deserializer;
        }

        public async Task ReceiveAndHandle(ConnectionContext context, byte[] data)
        {
            if (Handler != null)
            {
                await Handler(context, await Deserializer.Deserialize(data));
            }
        }
    }
    
    public delegate Task Handler(ConnectionContext context, object packet);
}
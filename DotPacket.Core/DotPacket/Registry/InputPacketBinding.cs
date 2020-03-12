using System;

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

        public void ReceiveAndHandle(ConnectionContext context, byte[] data)
        {
            if (Handler != null)
            {
                Handler(context, Deserializer.Deserialize(data));
            }
        }
    }
    
    public delegate void Handler(ConnectionContext context, object packet);
}